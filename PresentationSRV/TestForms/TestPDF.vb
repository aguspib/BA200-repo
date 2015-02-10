Imports System.IO
Imports Biosystems.Ax00.Global

Public NotInheritable Class TestPDF

    Private Sub TestPDF_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

    End Sub



    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click

        Try

           
            'Me.BsWebBrowser1.Navigate("C:\Prueba7.pdf")
            'Me.BsWebBrowser1.ScrollBarsEnabled = False
            'Me.BsWebBrowser1.AutoScrollOffset = New Point(0, 0)

            'Dim myUtil As New Utilities.

            ExtractFromZip("C:\Prueba3.zip", "C:\tmp", "AX00")

            Application.DoEvents()

            If Directory.Exists("C:\tmp") Then
                Dim myFiles As String() = Directory.GetFiles("C:\tmp", "*.rtf")
                If myFiles.Length > 0 Then
                    Me.BsRichTextBox1.Enabled = False
                    Me.BsRichTextBox1.ScrollBars = RichTextBoxScrollBars.Horizontal
                    Me.BsRichTextBox1.LoadFile(myFiles(0))

                    Application.DoEvents()

                    If File.Exists(myFiles(0)) Then
                        File.Delete(myFiles(0))
                    End If
                    If Directory.Exists("C:\tmp") Then
                        Directory.Delete("C:\tmp")
                    End If
                End If
            End If

            


        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
End Class
