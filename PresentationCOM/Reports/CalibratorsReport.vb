Imports DevExpress.XtraReports.UI

Public Class CalibratorsReport

    Private LastCalib As String = String.Empty
    Private LastNo As String = String.Empty

    Public Sub SetHeaderLabel(ByVal aText As String)
        XrHeaderLabel.Text = aText
    End Sub

    Private Sub XrTableRowDetails_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableRowDetails.BeforePrint
        Dim Row As XRTableRow = sender

        Dim TreatDuplicates = Sub(assignment As ValueSuppressType)
                                  XrTableCellTest.ProcessDuplicates = assignment
                                  XrTableCellSampleType.ProcessDuplicates = assignment
                                  XrTableCellCurveType1.ProcessDuplicates = assignment
                                  XrTableCellCurveType2.ProcessDuplicates = assignment
                                  XrTableCellXAxis.ProcessDuplicates = assignment
                                  XrTableCellYAxis.ProcessDuplicates = assignment
                              End Sub

        If LastNo = String.Empty Then
            LastNo = XrTableCellNo.Text
            LastCalib = XrLabelCalibName.Text
            Row.Borders = DevExpress.XtraPrinting.BorderSide.None
            TreatDuplicates(ValueSuppressType.Leave)
        Else
            If LastCalib = XrLabelCalibName.Text Then
                If LastNo = XrTableCellNo.Text Then
                    Row.Borders = DevExpress.XtraPrinting.BorderSide.None
                    TreatDuplicates(ValueSuppressType.Leave)
                Else
                    If LastNo = "1" Then
                        Row.Borders = DevExpress.XtraPrinting.BorderSide.Top
                        TreatDuplicates(ValueSuppressType.Leave)
                    Else
                        Row.Borders = DevExpress.XtraPrinting.BorderSide.None
                        TreatDuplicates(ValueSuppressType.Leave)
                    End If

                    LastNo = XrTableCellNo.Text
                End If
            Else
                LastCalib = XrLabelCalibName.Text
                LastNo = XrTableCellNo.Text
                Row.Borders = DevExpress.XtraPrinting.BorderSide.None
                TreatDuplicates(ValueSuppressType.Leave)
            End If
        End If
    End Sub

End Class