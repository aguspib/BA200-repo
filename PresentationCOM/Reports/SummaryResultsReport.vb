Imports DevExpress.XtraReports.UI

Public Class SummaryResultsReport

    Public Sub SetHeaderLabel(ByVal aText As String)
        'XrHeaderLabel.Text = aText
    End Sub

    Public Overloads Sub SetDataSource(ByVal aDataSource As DataTable)
        DataSource = aDataSource
        Dim detailDataMember As String = String.Format("{0}.{1}", aDataSource.TableName,
                aDataSource.ChildRelations("Values").RelationName)

        DetailReport.DataSource = aDataSource.ChildRelations("Values").ChildTable
        DetailReport.DataMember = detailDataMember
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

        'XrTableHeader.SizeF = New System.Drawing.SizeF(120.0F * 7, Height)
        'XrTableDetails.SizeF = New System.Drawing.SizeF(120.0F * 7, Height)

        XrWSStartDateTimeLabel.LocationF = New System.Drawing.PointF(PageWidth - (XrWSStartDateTimeLabel.WidthF + Margins.Right + Margins.Left), Height)
    End Sub

    Private Sub DetailReport_BeforePrint(sender As Object, e As Drawing.Printing.PrintEventArgs) Handles DetailReport.BeforePrint

        Dim groupId As Integer = Convert.ToInt32(Me.GetCurrentColumnValue("GroupId"))
        Dim detailReportBand As DetailReportBand = CType(sender, DetailReportBand)
        detailReportBand.FilterString = String.Format("[GroupId] = {0}", groupId)

    End Sub
End Class