

Public Class RotorContentPositionsReport
    Public Sub SetHeaderLabel(ByVal aText As String)
        XrHeaderLabel.Text = aText
    End Sub

    Private Sub XrTableCell2_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableCell2.BeforePrint

    End Sub

    Private Sub DetailReport_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles DetailReport.BeforePrint
        
    End Sub

    Private Sub Detail1_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles Detail1.BeforePrint

    End Sub
End Class