Imports DevExpress.XtraReports.UI

Public Class SummaryResultsReport

    Public Sub SetHeaderLabel(ByVal aText As String)
        XrHeaderLabel.Text = aText
    End Sub

    Public Sub SetDataSource(ByVal aDataSource As DataTable)
        DataSource = aDataSource
    End Sub

    Public Sub AddTableRowCells(ByVal Cells As XRTableCell())
        XrTableRowDetails.Cells.Clear()
        XrTableRowDetails.Cells.AddRange(Cells)
    End Sub

    Public Sub AddTableHeaderCells(ByVal Cells As XRTableCell())
        XrTableRowHeader.Cells.Clear()
        XrTableRowHeader.Cells.AddRange(Cells)
    End Sub

    Private Sub XtraSubReportTable_BeforePrint(ByVal sender As System.Object, ByVal e As System.Drawing.Printing.PrintEventArgs) Handles MyBase.BeforePrint
        Dim Width As Integer = PageWidth - Margins.Left - Margins.Right
        Dim Margin As Single = 20.0!
        Dim Height As Single = 20.0!

        XrHeaderLabel.SizeF = New System.Drawing.SizeF(Width, Height)
        XrTableHeader.SizeF = New System.Drawing.SizeF(Width - Margin, Height)
        XrTableDetails.SizeF = New System.Drawing.SizeF(Width - Margin, Height)
        XrWSStartDateTimeLabel.SizeF = New System.Drawing.SizeF(Width - Margin, Height)
    End Sub

End Class