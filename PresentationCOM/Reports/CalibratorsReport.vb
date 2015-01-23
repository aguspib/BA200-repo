Imports DevExpress.XtraReports.UI

Public Class CalibratorsReport

    Private LastCalib As String = String.Empty
    Private LastNo As String = String.Empty

    Public Sub SetHeaderLabel(ByVal aText As String)
        XrHeaderLabel.Text = aText
    End Sub

    Private Sub XrTableRowDetails_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles XrTableRowDetails.BeforePrint
        Dim Row As XRTableRow = TryCast(sender, XRTableRow)

        Dim TreatDuplicates = Sub(assignment As ProcessDuplicatesMode)
                                  XrTableCellTest.ProcessDuplicatesMode = assignment
                                  XrTableCellSampleType.ProcessDuplicatesMode = assignment
                                  XrTableCellCurveType1.ProcessDuplicatesMode = assignment
                                  XrTableCellCurveType2.ProcessDuplicatesMode = assignment
                                  XrTableCellXAxis.ProcessDuplicatesMode = assignment
                                  XrTableCellYAxis.ProcessDuplicatesMode = assignment
                              End Sub

        If LastNo = String.Empty Then
            LastNo = XrTableCellNo.Text
            LastCalib = XrLabelCalibName.Text
            Row.Borders = DevExpress.XtraPrinting.BorderSide.None
            TreatDuplicates(ProcessDuplicatesMode.Leave)
        Else
            If LastCalib = XrLabelCalibName.Text Then
                If LastNo = XrTableCellNo.Text Then
                    Row.Borders = DevExpress.XtraPrinting.BorderSide.None
                    TreatDuplicates(ProcessDuplicatesMode.Leave)
                Else
                    If LastNo = "1" Then
                        Row.Borders = DevExpress.XtraPrinting.BorderSide.Top
                        TreatDuplicates(ProcessDuplicatesMode.Leave)
                    Else
                        Row.Borders = DevExpress.XtraPrinting.BorderSide.None
                        TreatDuplicates(ProcessDuplicatesMode.Leave)
                    End If

                    LastNo = XrTableCellNo.Text
                End If
            Else
                LastCalib = XrLabelCalibName.Text
                LastNo = XrTableCellNo.Text
                Row.Borders = DevExpress.XtraPrinting.BorderSide.None
                TreatDuplicates(ProcessDuplicatesMode.Leave)
            End If
        End If
    End Sub

End Class