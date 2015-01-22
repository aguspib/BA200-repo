Option Strict On
Option Explicit On
Option Infer On

Imports System
' For Missing.Value and BindingFlags
' For COMException
Imports System.IO
''''''
Imports Biosystems.Ax00.Global
'Imports History.Biosystems.Ax00.BL


Public Class XmlEncrypt
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    Private Function FileEncription(pFileToEncript As String, pEncriptedFile As String) As String
        Dim results As String = String.Empty

        Try
            'Dim Utilities As New Utilities
            Dim myGlobalDataTO As New GlobalDataTO
            myGlobalDataTO = Utilities.EncryptFile(pFileToEncript, pEncriptedFile)

            If myGlobalDataTO.HasError Then
                MessageBox.Show(myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        Return results
    End Function

    Private Function FileDeCript(pFileCript As String, pDecriptFile As String) As String
        Dim results As String = String.Empty

        Try
            'Dim Utilities As New Utilities
            Dim myGlobalDataTO As New GlobalDataTO
            myGlobalDataTO = Utilities.DecryptFile(pFileCript, pDecriptFile)

            If Not myGlobalDataTO.HasError Then
                MessageBox.Show(myGlobalDataTO.ErrorMessage)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        Return results
    End Function

    Private Sub EncriptButton_Click(sender As Object, e As EventArgs) Handles EncriptButton.Click
        OpenFileDialog1.InitialDirectory = Application.StartupPath
        OpenFileDialog1.FileName = String.Empty
        OpenFileDialog1.ShowDialog()

        If Not OpenFileDialog1.FileName.Trim() = String.Empty AndAlso File.Exists(OpenFileDialog1.FileName.Trim()) Then

            Dim Textreader As New StreamReader(OpenFileDialog1.FileName)
            RichTextBox1.Text = Textreader.ReadToEnd()
            Textreader.Close()

        End If
    End Sub

    Private Sub EncripFileButton_Click(sender As Object, e As EventArgs) Handles EncrypFileButton.Click
        Try
            ErrorProvider1.Clear()
            'Validate there'is a file loaded to encrypt
            If Not RichTextBox1.Text.Trim() = String.Empty Then
                If Not EncriptedFileName.Text.Trim() = String.Empty Then
                    Dim CriptFilePath As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    FileEncription(OpenFileDialog1.FileName, Path.GetDirectoryName(OpenFileDialog1.FileName) & "\" & EncriptedFileName.Text)
                    Dim Textreader As New StreamReader(Path.GetDirectoryName(OpenFileDialog1.FileName) & "\" & EncriptedFileName.Text)

                    RichTextBox2.Text = Textreader.ReadToEnd()
                    Textreader.Close()
                Else
                    ErrorProvider1.SetError(EncriptedFileName, "Need a file Name.")
                End If
            Else
                ErrorProvider1.SetError(RichTextBox1, "No file loaded to encrypt.")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        RichTextBox1.Clear()
        RichTextBox2.Clear()
        ErrorProvider1.Clear()
        OpenFileDialog1.FileName = String.Empty
    End Sub
End Class


