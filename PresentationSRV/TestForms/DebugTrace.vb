Public Class DebugTrace

    Public Sub WriteLine(ByVal pLine As String)
        Try
            Me.ConsoleLabel.Text &= DateTime.Now.ToString("hh:mm:ss") & vbTab & pLine & vbCrLf
        Catch ex As Exception
            Me.Close()
        End Try
    End Sub

    Private Sub DebugTrace_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Me.ConsoleLabel.Text = "DEBUG TRACE" & vbCrLf
        Catch ex As Exception
            Throw ex
        End Try
    End Sub
End Class