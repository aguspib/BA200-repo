Imports DevExpress.XtraReports.UI

Public Class ResultsByTestReportCompact

    Public Sub SetHeaderLabel(ByVal aText As String)
        'XrHeaderLabel.Text = aText  'EF 06/06/2014 #1649
    End Sub

    Private Sub XrTableDetailsRow_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableDetailsRow.BeforePrint
        If Not String.IsNullOrEmpty(XrTableCellClass.Text) Then
            XrTableDetailsRow.BackColor = Drawing.Color.WhiteSmoke
        Else
            XrTableDetailsRow.BackColor = Drawing.Color.Transparent
        End If
    End Sub

End Class