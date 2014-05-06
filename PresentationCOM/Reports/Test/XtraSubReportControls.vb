Imports DevExpress.XtraReports.UI
Imports Biosystems.Ax00.Types

Public Class XtraSubReportControls

    Private Sub XtraSubReportControls_BeforePrint(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles Me.BeforePrint

        'Dim mycontrol As New TestControlsDS
        'Dim mycontrolRow As TestControlsDS.tparTestControlsRow

        'mycontrolRow = mycontrol.tparTestControls.NewtparTestControlsRow
        'mycontrolRow.TestID = 1
        'mycontrolRow.ControlID = 1
        'mycontrolRow.ControlName = "c1"
        'mycontrolRow.MinConcentration = 1
        'mycontrolRow.MaxConcentration = 3
        'mycontrolRow.TargetMean = 2
        'mycontrolRow.TargetSD = 5

        'mycontrol.tparTestControls.AddtparTestControlsRow(mycontrolRow)

        'Me.DataSource = mycontrol
        'DirectCast(sender, XRSubreport).ReportSource.DataSource = mycontrol 'DirectCast(sender, XRSubreport).Report.DataSource

        'If Not GetCurrentColumnValue("TestID") Is Nothing Then
        'CType((CType(sender, XRSubreport)).ReportSource, XtraSubReportTest).TESTID.Value = GetCurrentColumnValue("TestID").ToString
        'CType((CType(sender, XRSubreport)).ReportSource, XtraSubReportTest).SAMPLETYPE.Value = GetCurrentColumnValue("SampleType").ToString
        'End If

    End Sub
End Class