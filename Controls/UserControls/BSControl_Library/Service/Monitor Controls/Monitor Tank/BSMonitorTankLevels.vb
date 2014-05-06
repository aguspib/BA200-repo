Option Strict On
Option Explicit On

Imports PerpetuumSoft.Instrumentation
Imports PerpetuumSoft.Instrumentation.Windows.Forms
Imports PerpetuumSoft.Instrumentation.Model
Imports PerpetuumSoft.Framework.Drawing


Namespace Biosystems.Ax00.Controls.UserControls

    Public Class BSMonitorTankLevels
        Inherits BSMonitorTank

        Public Sub New()

            MyBase.New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            MyBase.InstrumentPanel = MyClass.InstrumentPanel
            MyBase.InstrumentationControl = MyClass.IndicatorWidget1


            MyBase.TextValueElementName = "LabelValue"

            MyBase.TitleHeight = 0

            Me.Refresh()

        End Sub

        Private SharpImageElementName As String = "SharpImage"
        Private TopLevelImageElementName As String = "TopImage"
        Private BottomLevelImageElementName As String = "BottomImage"
        Private TopStopImageElementName As String = "TopStopImage"
        Private BottomStopImageElementName As String = "BottomStopImage"

        Public Enum TankLevels
            TOP
            MIDDLE
            BOTTOM
            UNDEFINED
        End Enum

        Public Property BackImage() As System.Drawing.Bitmap
            Get
                Return BackImageAttr
            End Get
            Set(ByVal value As System.Drawing.Bitmap)
                If value IsNot Nothing Then
                    SetBackImage(value)
                    BackImageAttr = value
                End If
            End Set
        End Property

        Public Property SharpImage() As System.Drawing.Bitmap
            Get
                Return SharpImageAttr
            End Get
            Set(ByVal value As System.Drawing.Bitmap)
                If value IsNot Nothing Then
                    SetSharpImage(value)
                    SharpImageAttr = value
                End If
            End Set
        End Property

        Public Property TopLevelImage() As System.Drawing.Bitmap
            Get
                Return TopLevelImageAttr
            End Get
            Set(ByVal value As System.Drawing.Bitmap)
                If value IsNot Nothing Then
                    SetTopLevelImage(value)
                    TopLevelImageAttr = value
                End If
            End Set
        End Property

        Public Property BottomLevelImage() As System.Drawing.Bitmap
            Get
                Return BottomLevelImageAttr
            End Get
            Set(ByVal value As System.Drawing.Bitmap)
                If value IsNot Nothing Then
                    SetBottomLevelImage(value)
                    BottomLevelImageAttr = value
                End If
            End Set
        End Property

        Public Property TopStopImage() As System.Drawing.Bitmap
            Get
                Return TopStopImageAttr
            End Get
            Set(ByVal value As System.Drawing.Bitmap)
                If value IsNot Nothing Then
                    SetTopStopImage(value)
                    TopStopImageAttr = value
                End If
            End Set
        End Property

        Public Property BottomStopImage() As System.Drawing.Bitmap
            Get
                Return BottomStopImageAttr
            End Get
            Set(ByVal value As System.Drawing.Bitmap)
                If value IsNot Nothing Then
                    SetBottomStopImage(value)
                    BottomStopImageAttr = value
                End If
            End Set
        End Property

        Public Property TankLevel() As TankLevels
            Get
                Return TankLevelAttr
            End Get
            Set(ByVal value As TankLevels)
                SetTankLevel(value)
                TankLevelAttr = value

            End Set
        End Property


        Private BackImageAttr As System.Drawing.Bitmap = Nothing
        Private SharpImageAttr As System.Drawing.Bitmap = Nothing
        Private TopLevelImageAttr As System.Drawing.Bitmap = Nothing
        Private BottomLevelImageAttr As System.Drawing.Bitmap = Nothing
        Private TopStopImageAttr As System.Drawing.Bitmap = Nothing
        Private BottomStopImageAttr As System.Drawing.Bitmap = Nothing
        Private TankLevelAttr As TankLevels = TankLevels.MIDDLE

        Private Sub SetBackImage(ByVal pImage As System.Drawing.Bitmap)
            Try
                Dim myImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyBase.BackImageElementName), Model.Picture)

                If myImage IsNot Nothing Then
                    myImage.Image = New System.Drawing.Bitmap(pImage)
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetSharpImage(ByVal pImage As System.Drawing.Bitmap)
            Try
                Dim myImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyClass.SharpImageElementName), Model.Picture)

                If myImage IsNot Nothing Then
                    myImage.Image = New System.Drawing.Bitmap(pImage)
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetTopLevelImage(ByVal pImage As System.Drawing.Bitmap)
            Try
                Dim myImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyClass.TopLevelImageElementName), Model.Picture)

                If myImage IsNot Nothing Then
                    myImage.Image = New System.Drawing.Bitmap(pImage)
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetBottomLevelImage(ByVal pImage As System.Drawing.Bitmap)
            Try
                Dim myImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyClass.BottomLevelImageElementName), Model.Picture)

                If myImage IsNot Nothing Then
                    myImage.Image = New System.Drawing.Bitmap(pImage)
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetTopStopImage(ByVal pImage As System.Drawing.Bitmap)
            Try
                Dim myImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyClass.TopStopImageElementName), Model.Picture)

                If myImage IsNot Nothing Then
                    myImage.Image = New System.Drawing.Bitmap(pImage)
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetBottomStopImage(ByVal pImage As System.Drawing.Bitmap)
            Try
                Dim myImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyClass.BottomStopImageElementName), Model.Picture)

                If myImage IsNot Nothing Then
                    myImage.Image = New System.Drawing.Bitmap(pImage)
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Private Sub SetTankLevel(ByVal pLevel As TankLevels)
            Try
                Dim myTopImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyClass.TopLevelImageElementName), Model.Picture)
                Dim myBottomImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyClass.BottomLevelImageElementName), Model.Picture)
                Dim mySharpImage As Model.Picture = CType(GetElement(MyBase.InstrumentationControl, MyClass.SharpImageElementName), Model.Picture)

                If myTopImage IsNot Nothing And myBottomImage IsNot Nothing And mySharpImage IsNot Nothing Then
                    Dim myPosTop As Single
                    Dim myPosBottom As Single
                    Select Case pLevel
                        Case TankLevels.TOP
                            myPosTop = 0.6
                            myPosBottom = 1.4
                            MyBase.LevelValue = 0.9 * (MyBase.UpperLevelValue - MyBase.LowerLevelValue) + MyBase.LowerLevelValue

                        Case TankLevels.MIDDLE
                            myPosTop = 0.8
                            myPosBottom = 1.4
                            MyBase.LevelValue = 0.5 * (MyBase.UpperLevelValue - MyBase.LowerLevelValue) + MyBase.LowerLevelValue

                        Case TankLevels.BOTTOM
                            myPosTop = 0.8
                            myPosBottom = 1.6
                            MyBase.LevelValue = 0.1 * (MyBase.UpperLevelValue - MyBase.LowerLevelValue) + MyBase.LowerLevelValue

                    End Select

                    myTopImage.Center = New Vector(mySharpImage.Center.X, myPosTop * mySharpImage.Center.Y)
                    myBottomImage.Center = New Vector(mySharpImage.Center.X, myPosBottom * mySharpImage.Center.Y)

                End If


            Catch ex As Exception
                Throw ex
            End Try
        End Sub



        Private Sub BSMonitor_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
            Try
                SetTankLevel(MyClass.TankLevel)
            Catch ex As Exception
                Throw ex
            End Try
        End Sub


        Private Sub BSMonitorTankLevels_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                MyClass.SetTankLevel(MyClass.TankLevel)
                MyClass.IndicatorWidget1.Refresh()
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
    End Class

End Namespace