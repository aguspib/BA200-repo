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

Imports System.ComponentModel

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class BsScadaPumpControl
        Inherits BsScadaControl

        Private PictureElementName As String = "Picture"
        Private PictureSetElementName As String = "PictureSet"

        Public Sub New()
            Try
                ' This call is required by the Windows Form Designer.
                InitializeComponent()

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
        Public Property DutyValue() As Integer
            Get
                Return DutyAttr
            End Get
            Set(ByVal value As Integer)
                DutyAttr = value
            End Set
        End Property

        Public Property ProgressiveStart() As Integer
            Get
                Return ProgressiveStartAttr
            End Get
            Set(ByVal value As Integer)
                ProgressiveStartAttr = value
            End Set
        End Property

        Public Overloads Property ToolTipText() As String
            Get
                Return MyBase.ToolTipText
            End Get
            Set(ByVal value As String)
                MyBase.ToolTipText = value
                MyBase.ToolTip.SetToolTip(Me.ActuatorWidget, value)
                MyBase.ToolTip.SetToolTip(Me.ControlWidget, value)
            End Set
        End Property

        ''' <summary>
        ''' Sets (ONLY RUNTIME) or returns the image list used for displaying different states
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Description("Sets (ONLY RUNTIME) or returns the image list used for displaying different states")> _
        Public Property ImageList() As List(Of System.Drawing.Image)
            Get
                Dim myImageList As New List(Of Image)
                Dim myPicSet As Model.PictureSet = CType(GetElement(Me.ControlWidget, PictureSetElementName), Model.PictureSet)
                If myPicSet IsNot Nothing Then
                    For Each W As Model.ImageWrap In myPicSet.Images
                        myImageList.Add(W.Image)
                    Next
                    Return myImageList
                Else
                    Return myImageList
                End If
            End Get
            Set(ByVal value As List(Of System.Drawing.Image))
                Try
                    Dim myPicSet As Model.PictureSet = CType(GetElement(Me.ControlWidget, PictureSetElementName), Model.PictureSet)
                    If myPicSet IsNot Nothing Then
                        myPicSet.Images.Clear()
                        For Each I As Image In value
                            Dim myBitMap As New System.Drawing.Bitmap(I)
                            Dim myImageWrap As New Model.ImageWrap(myBitMap)
                            myPicSet.Images.Add(myImageWrap)
                        Next
                    End If
                Catch ex As Exception
                    Throw ex
                End Try
            End Set
        End Property

#End Region

#Region "Inherited Properties"


        Public Overloads Property Orientation() As Orientations
            Get
                Return MyBase.Orientation
            End Get
            Set(ByVal value As Orientations)


                SetOrientation(value)

                MyBase.Orientation = value

            End Set
        End Property

        Public Overloads Property ActivationState() As States
            Get
                Return MyBase.ActivationState
            End Get
            Set(ByVal value As States)
                MyClass.SetActivationState(value)
                MyBase.ActivationState = value
                MyClass.FluidColor = MyBase.FluidColor
                MyBase.Refresh()

            End Set
        End Property

        Public Overloads Property IsAlarm() As Boolean
            Get
                Return MyBase.IsAlarm
            End Get
            Set(ByVal value As Boolean)
                If MyBase.IsAlarm <> value Then
                    MyBase.IsAlarm = value
                End If
            End Set
        End Property

#End Region



#Region "Attributes"
        Private DutyAttr As Integer = 100
        Private ProgressiveStartAttr As Integer = 1
#End Region


#Region "Private Methods"


        Private Sub SetOrientation(ByVal pOrientation As BsScadaControl.Orientations)
            Try
                Dim myControl As Instrument = CType(GetElement(Me.ControlWidget, "Instrument"), Instrument)
                Dim myPic As Model.Picture = CType(GetElement(Me.ControlWidget, PictureElementName), Model.Picture)

                If myControl IsNot Nothing And myPic IsNot Nothing Then
                    Select Case pOrientation
                        Case Orientations._0

                            With Me.ActuatorWidget
                                .Top = 0
                                .Left = 0
                                .Width = Me.Width
                                .Height = ActuatorWidth
                            End With

                            With Me.ControlWidget
                                .Top = Me.ActuatorWidget.Height
                                .Left = 0
                                .Width = Me.Width
                                .Height = Me.Height - Me.ActuatorWidget.Height
                            End With

                            myPic.Angle = 0


                        Case Orientations._90

                            With Me.ActuatorWidget
                                .Top = 0
                                .Left = Me.Width - ActuatorWidth
                                .Width = ActuatorWidth
                                .Height = Me.Height
                            End With

                            With Me.ControlWidget
                                .Top = 0
                                .Left = 0
                                .Width = Me.Width - Me.ActuatorWidget.Width
                                .Height = Me.Height
                            End With

                            myPic.Angle = 90

                        Case Orientations._180

                            With Me.ActuatorWidget
                                .Top = Me.Height - ActuatorWidth
                                .Left = 0
                                .Width = Me.Width
                                .Height = ActuatorWidth
                            End With

                            With Me.ControlWidget
                                .Top = 0
                                .Left = 0
                                .Width = Me.Width
                                .Height = Me.Height - Me.ActuatorWidget.Height
                            End With

                            myPic.Angle = 180


                        Case Orientations._270

                            With Me.ActuatorWidget
                                .Top = 0
                                .Left = 0
                                .Width = ActuatorWidth
                                .Height = Me.Height
                            End With

                            With Me.ControlWidget
                                .Top = 0
                                .Left = Me.ActuatorWidget.Width
                                .Width = Me.Width - Me.ActuatorWidget.Width
                                .Height = Me.Height
                            End With

                            myPic.Angle = 270

                    End Select

                    myPic.Recalculate()

                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetActivationState(ByVal pState As States)
            Try
                Dim myPic As Model.Picture = CType(GetElement(Me.ControlWidget, PictureElementName), Model.Picture)
                Dim myPicSet As Model.PictureSet = CType(GetElement(Me.ControlWidget, PictureSetElementName), Model.PictureSet)
                Dim myLabel As Model.Label = CType(GetElement(Me.ActuatorWidget, LabelElementName), Model.Label)

                If myPic IsNot Nothing And myPicSet IsNot Nothing And myLabel IsNot Nothing Then

                    Select Case pState
                        Case States._OFF
                            myPic.Image = myPicSet.Images(0).Image 'OFF

                        Case States._ON
                            myPic.Image = myPicSet.Images(1).Image 'ON

                    End Select
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub


#End Region

#Region "Private Event Handlers"



        Private Sub BsValveControl_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Try
                Me.ActuatorWidget.Height = 0
                MyBase.ActivatorVisible = False

                If Me.Height < 40 Then
                    Me.Height = 40
                End If

                If Me.Width < 40 Then
                    Me.Width = 40
                End If

                Me.Height = Me.Width
                SetActivationState(MyBase.ActivationState)

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