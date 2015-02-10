Option Explicit On
Option Strict On

Imports System.Drawing
Imports System.Windows.Forms
Imports Biosystems.Ax00.Global

Public Class BsGIFViewer

#Region "Public Properties"
    Public Property VerticalMarginWidth() As Integer
        Get
            Return VerticalMarginWidthAttr
        End Get
        Set(ByVal value As Integer)
            VerticalMarginWidthAttr = value
        End Set
    End Property
    Public Property HorizontalMarginWidth() As Integer
        Get
            Return HorizontalMarginWidthAttr
        End Get
        Set(ByVal value As Integer)
            HorizontalMarginWidthAttr = value
        End Set
    End Property

    Private Property Zoom() As Integer
        Get
            Return ZoomAttr
        End Get
        Set(ByVal value As Integer)
            ZoomAttr = value
        End Set
    End Property
    Private VerticalMarginWidthAttr As Integer = 10
    Private HorizontalMarginWidthAttr As Integer = 10
    Private ZoomAttr As Integer = 100

#End Region

#Region "public Methods"
    Public Sub OpenGIFFile(ByVal pPath As String)
        Try
            If System.IO.File.Exists(pPath) Then
                Dim myImage As Image = ImageUtilities.ImageFromFile(pPath)

                myImage = ResizeImage(myImage, New Size(CInt(myImage.Width * ZoomAttr / 100), CInt(myImage.Height * ZoomAttr / 100)))
                Me.myPicBox.Size = New Size(myImage.Size.Width + HorizontalMarginWidthAttr, myImage.Size.Height + VerticalMarginWidthAttr)

                Me.myPicBox.BackgroundImageLayout = ImageLayout.Center
                Me.myPicBox.BackgroundImage = myImage
            Else
                Throw New System.IO.FileNotFoundException
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Sub Clear()
        Try
            Me.myPicBox.BackgroundImage = Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
#End Region



#Region "private Methods"
    Private Function ResizeImage(ByVal pImage As System.Drawing.Image, ByVal pNewSize As System.Drawing.Size) As Image
        Try
            If pImage IsNot Nothing Then
                Dim myNewImage As New System.Drawing.Bitmap(pNewSize.Width, pNewSize.Height)
                Dim GraphicsImage As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(myNewImage)
                GraphicsImage.DrawImage(pImage, 0, 0, myNewImage.Width + 1, myNewImage.Height + 1)
                Return myNewImage
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region
End Class
