Imports DevExpress.XtraReports.UI

Public Class ResultsByPatientSampleReport

    Public Sub SetHeaderLabel(ByVal aText As String)
        ' XrHeaderLabel.Text = aText
    End Sub

    Private Sub XrTableDetailsRow_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableDetailsRow.BeforePrint
        If Not String.IsNullOrEmpty(XrTableCellTestName.Text) Then
            XrTableDetailsRow.BackColor = Drawing.Color.WhiteSmoke
        Else
            XrTableDetailsRow.BackColor = Drawing.Color.Transparent
        End If
    End Sub

End Class