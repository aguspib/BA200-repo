Imports System.IO
Imports System.Drawing

Namespace Biosystems.Ax00.Global

    Public Class ImageUtilities

        Public Shared Function ImageFromFile(path As String) As Bitmap

            Dim fs = New FileStream(path, FileMode.Open, FileAccess.Read)
            Dim img = Image.FromStream(fs)
            fs.close()

            Return img

        End Function

    End Class

End Namespace
