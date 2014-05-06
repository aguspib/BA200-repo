''' <summary>
''' Historic Results by Patient Sample Report
''' </summary>
''' <remarks>
''' Created by DL: 24/10/2012
''' </remarks>


Public Class HistoricResultsByPatientSampleReport

    Public Sub SetHeaderLabel(ByVal aText As String)
        XrHeaderLabel.Text = aText
    End Sub

    '    Private Sub XrTableDetailsRow_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableDetailsRow.BeforePrint
    ''If Not String.IsNullOrEmpty(XrTableCellTestName.Text) Then
    '' XrTableDetailsRow.BackColor = Drawing.Color.WhiteSmoke
    ''Else
    '    XrTableDetailsRow.BackColor = Drawing.Color.White
    ''End If
    'End Sub

End Class