Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Imports PerpetuumSoft.Instrumentation
Imports PerpetuumSoft.Instrumentation.Model
Imports PerpetuumSoft.Framework.Drawing

Imports System.ComponentModel

Namespace Biosystems.Ax00.Controls.UserControls

    Public Class BsScadaValveControl
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


#Region "Public Events"


#End Region


#Region "Enumerates"
        Public Enum WayTypes
            Normally_Closed
            Normally_Open
        End Enum

        Public Enum PhysicalStates
            Open
            Closed
        End Enum
#End Region

#Region "Public Properties"

        Public Property WayType() As WayTypes
            Get
                Return WayTypeAttr
            End Get
            Set(ByVal value As WayTypes)
                WayTypeAttr = value
                MyClass.SetActivationState(MyBase.ActivationState)
            End Set
        End Property

        Public ReadOnly Property PhysicalState() As PhysicalStates
            Get
                If MyClass.ActivationState = States._ON And MyClass.WayType = WayTypes.Normally_Closed Or _
                MyClass.ActivationState = States._OFF And MyClass.WayType = WayTypes.Normally_Open Then
                    Return PhysicalStates.Open
                Else
                    Return PhysicalStates.Closed
                End If
            End Get
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

            End Set
        End Property

        Public Overloads Property ActivatorVisible() As Boolean
            Get
                Return MyBase.ActivatorVisible
            End Get
            Set(ByVal value As Boolean)

                Me.ActuatorWidget.Visible = value

                MyBase.ActivatorVisible = value

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
        Private WayTypeAttr As WayTypes = WayTypes.Normally_Open
        Private ToolTipTextAttr As String = ""
#End Region


#Region "Private Methods"



        Private Sub SetOrientation(ByVal pOrientation As BsScadaControl.Orientations)
            Try
                Dim myControl As Instrument = CType(GetElement(Me.ControlWidget, "Instrument"), Instrument)
                Dim myActuator As Instrument = CType(GetElement(Me.ActuatorWidget, "Instrument"), Instrument)
                Dim myCirc As Circle = CType(GetElement(Me.ActuatorWidget, CircleElementName), Circle)
                Dim myLabel As Model.Label = CType(GetElement(Me.ActuatorWidget, LabelElementName), Model.Label)
                Dim myPic As Model.Picture = CType(GetElement(Me.ControlWidget, PictureElementName), Model.Picture)

                If myControl IsNot Nothing And myActuator IsNot Nothing And myCirc IsNot Nothing And myPic IsNot Nothing Then
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
                            myCirc.Radius = 0.9 * myActuator.Size.Y / 2
                            myPic.Size = New Vector(myControl.Size.X, myControl.Size.Y)

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
                            myCirc.Radius = 0.9 * myActuator.Size.X / 2
                            myPic.Size = New Vector(myControl.Size.Y, myControl.Size.X)



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
                            myCirc.Radius = 0.9 * myActuator.Size.Y / 2
                            myPic.Size = New Vector(myControl.Size.X, myControl.Size.Y)

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
                            myCirc.Radius = 0.9 * myActuator.Size.X / 2
                            myPic.Size = New Vector(myControl.Size.Y, myControl.Size.X)
                    End Select


                    myCirc.Recalculate()
                    myLabel.Recalculate()
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
                            SetElementSolidColor(Me.ActuatorWidget, CircleElementName, MyBase.DeactivatedColor)
                            myLabel.Text = "OFF"
                            Select Case MyClass.WayType
                                Case WayTypes.Normally_Closed
                                    myPic.Image = myPicSet.Images(0).Image 'CLOSED
                                Case WayTypes.Normally_Open
                                    myPic.Image = myPicSet.Images(1).Image 'OPEN
                            End Select

                        Case States._ON
                            SetElementSolidColor(Me.ActuatorWidget, CircleElementName, MyBase.ActivatedColor)
                            myLabel.Text = "ON"
                            Select Case MyClass.WayType
                                Case WayTypes.Normally_Closed
                                    myPic.Image = myPicSet.Images(1).Image 'OPEN
                                Case WayTypes.Normally_Open
                                    myPic.Image = myPicSet.Images(0).Image 'CLOSED
                            End Select

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
                If Me.Height < 1.7 * ActuatorWidth Then
                    Me.Height = CInt(1.7 * ActuatorWidth)
                End If

                If Me.Width < 1.7 * ActuatorWidth Then
                    Me.Width = CInt(1.7 * ActuatorWidth)
                End If

                SetOrientation(MyBase.Orientation)
                SetActivationState(MyBase.ActivationState)

            Catch ex As Exception
                MessageBox.Show(ex.Message)
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