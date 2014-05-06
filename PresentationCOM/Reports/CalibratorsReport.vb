Imports DevExpress.XtraReports.UI

Public Class CalibratorsReport

    Private LastCalib As String = String.Empty
    Private LastNo As String = String.Empty

    Public Sub SetHeaderLabel(ByVal aText As String)
        XrHeaderLabel.Text = aText
    End Sub

    Private Sub XrTableRowDetails_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableRowDetails.BeforePrint
        Dim Row As XRTableRow = sender

        If LastNo = String.Empty Then
            LastNo = XrTableCellNo.Text
            LastCalib = XrLabelCalibName.Text
            Row.Borders = DevExpress.XtraPrinting.BorderSide.None
            XrTableCellTest.ProcessDuplicates = ValueSuppressType.Leave
            XrTableCellSampleType.ProcessDuplicates = ValueSuppressType.Leave
            XrTableCellCurveType1.ProcessDuplicates = ValueSuppressType.Leave
            XrTableCellCurveType2.ProcessDuplicates = ValueSuppressType.Leave
            XrTableCellXAxis.ProcessDuplicates = ValueSuppressType.Leave
            XrTableCellYAxis.ProcessDuplicates = ValueSuppressType.Leave
        Else
            If LastCalib = XrLabelCalibName.Text Then
                If LastNo = XrTableCellNo.Text Then
                    Row.Borders = DevExpress.XtraPrinting.BorderSide.None
                    XrTableCellTest.ProcessDuplicates = ValueSuppressType.Leave
                    XrTableCellSampleType.ProcessDuplicates = ValueSuppressType.Leave
                    XrTableCellCurveType1.ProcessDuplicates = ValueSuppressType.Leave
                    XrTableCellCurveType2.ProcessDuplicates = ValueSuppressType.Leave
                    XrTableCellXAxis.ProcessDuplicates = ValueSuppressType.Leave
                    XrTableCellYAxis.ProcessDuplicates = ValueSuppressType.Leave
                Else
                    If LastNo = "1" Then
                        Row.Borders = DevExpress.XtraPrinting.BorderSide.Top
                        XrTableCellTest.ProcessDuplicates = ValueSuppressType.Leave
                        XrTableCellSampleType.ProcessDuplicates = ValueSuppressType.Leave
                        XrTableCellCurveType1.ProcessDuplicates = ValueSuppressType.Leave
                        XrTableCellCurveType2.ProcessDuplicates = ValueSuppressType.Leave
                        XrTableCellXAxis.ProcessDuplicates = ValueSuppressType.Leave
                        XrTableCellYAxis.ProcessDuplicates = ValueSuppressType.Leave
                    Else
                        Row.Borders = DevExpress.XtraPrinting.BorderSide.None
                        XrTableCellTest.ProcessDuplicates = ValueSuppressType.Suppress
                        XrTableCellSampleType.ProcessDuplicates = ValueSuppressType.Suppress
                        XrTableCellCurveType1.ProcessDuplicates = ValueSuppressType.Suppress
                        XrTableCellCurveType2.ProcessDuplicates = ValueSuppressType.Suppress
                        XrTableCellXAxis.ProcessDuplicates = ValueSuppressType.Suppress
                        XrTableCellYAxis.ProcessDuplicates = ValueSuppressType.Suppress
                    End If

                    LastNo = XrTableCellNo.Text
                End If
            Else
                LastCalib = XrLabelCalibName.Text
                LastNo = XrTableCellNo.Text
                Row.Borders = DevExpress.XtraPrinting.BorderSide.None
                XrTableCellTest.ProcessDuplicates = ValueSuppressType.Leave
                XrTableCellSampleType.ProcessDuplicates = ValueSuppressType.Leave
                XrTableCellCurveType1.ProcessDuplicates = ValueSuppressType.Leave
                XrTableCellCurveType2.ProcessDuplicates = ValueSuppressType.Leave
                XrTableCellXAxis.ProcessDuplicates = ValueSuppressType.Leave
                XrTableCellYAxis.ProcessDuplicates = ValueSuppressType.Leave
            End If
        End If
    End Sub

End Class