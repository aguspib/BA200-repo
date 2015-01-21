

Public Class RotorContentPositionsReport
    Public Sub SetHeaderLabel(ByVal aText As String)
        XrHeaderLabel.Text = aText
    End Sub

    Private Sub XrTableCell2_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableCell2.BeforePrint
        'Added treatement of coloring cell based con statuscolor field
        Dim vble As String = CType(GetCurrentColumnValue("StatusColor"), String)
        If (vble = "FEW") Then
            XrTableCell2.BackColor = System.Drawing.Color.FromArgb(169, 138, 225)
        ElseIf (vble = "DEPLETED") Then
            XrTableCell2.BackColor = System.Drawing.Color.Salmon
        Else
            XrTableCell2.BackColor = System.Drawing.Color.White
        End If
    End Sub

    Private Sub DetailReport_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs)

    End Sub

    Private Sub Detail1_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs)

    End Sub
End Class