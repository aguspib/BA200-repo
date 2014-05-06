Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraCharts

Public Class ResultsCalibCurveReport

    Public Sub SetHeaderLabel(ByVal aText As String)
        XrHeaderLabel.Text = aText
    End Sub

    Private Sub XrChart1_CustomDrawSeriesPoint(ByVal sender As Object, ByVal e As CustomDrawSeriesPointEventArgs) Handles XrChart1.CustomDrawSeriesPoint
        If e.Series.Name = XrChart1.Series(1).Name Then
            If Not e.SeriesPoint.Tag Is Nothing Then e.LabelText = e.SeriesPoint.Tag.ToString()
        End If
    End Sub

End Class