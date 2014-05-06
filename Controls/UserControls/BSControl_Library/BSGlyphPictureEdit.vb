Imports DevExpress.XtraEditors
Imports System.Drawing

''' <summary>
''' created by dl 27/03/2012
''' </summary>
''' <remarks></remarks>
Public Class BSGlyphPictureEdit
    Inherits PictureEdit

    Public Sub New()
        MyBase.New()
        BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
    End Sub

    ''' <summary>
    ''' This only works if the control is the size of the image, and has no border.
    ''' Ie the image is exactly what is in the control, with no resampling.
    ''' </summary>
    ''' <param name="pe"></param>
    ''' <returns></returns>
    Private Function ExactClippingRegion(ByVal pe As PictureEdit) As Region
        Dim result As Region = Nothing
        Dim image As Image = pe.Image
        If image Is Nothing Then
            Return result
        End If
        Dim bitmap As Bitmap = DirectCast(image, Bitmap)
        Dim rects As New List(Of Rectangle)()
        For y As Integer = 0 To image.Height - 1
            For x As Integer = 0 To image.Width - 1
                If bitmap.GetPixel(x, y).A <> 0 Then
                    ' Alpha = 0 => transparent
                    Dim rect As New Rectangle(x, y, 1, 1)
                    ' 1 pixel rectangle
                    rects.Add(rect)
                End If
            Next
        Next
        If rects.Count > 0 Then
            result = New Region(rects(0))
            For Each rect As Rectangle In rects
                result.Union(rect)
            Next
        End If
        Return result
    End Function

    Protected Overrides Sub OnEditValueChanged()
        MyBase.OnEditValueChanged()
        If Image Is Nothing Then
            ' no clipping
            Region = New Region()
        Else
            Width = Image.Width
            Height = Image.Height
            Region = ExactClippingRegion(Me)
        End If
    End Sub

End Class


