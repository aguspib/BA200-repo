<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class SummaryResultsReport
    Inherits DevExpress.XtraReports.UI.XtraReport

    'XtraReport overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Designer
    'It can be modified using the Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand()
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.XrControlStyle1 = New DevExpress.XtraReports.UI.XRControlStyle()
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand()
        Me.DetailChild = New DevExpress.XtraReports.UI.DetailBand()
        Me.GroupHeader2 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrWSStartDateTimeLabel = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrTableHeader = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRowHeader = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableHeaderCell = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableRowDetails = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableDetails = New DevExpress.XtraReports.UI.XRTable()
        CType(Me.XrTableHeader, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTableDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail.HeightF = 24.0!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.StylePriority.UseFont = False
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'TopMargin
        '
        Me.TopMargin.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TopMargin.HeightF = 96.0!
        Me.TopMargin.Name = "TopMargin"
        Me.TopMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.TopMargin.StylePriority.UseFont = False
        Me.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'BottomMargin
        '
        Me.BottomMargin.HeightF = 25.0!
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrControlStyle1
        '
        Me.XrControlStyle1.BackColor = System.Drawing.Color.Gainsboro
        Me.XrControlStyle1.Name = "XrControlStyle1"
        Me.XrControlStyle1.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        '
        'GroupHeader1
        '
        Me.GroupHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTableHeader})
        Me.GroupHeader1.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupHeader1.HeightF = 24.0!
        Me.GroupHeader1.Name = "GroupHeader1"
        Me.GroupHeader1.RepeatEveryPage = True
        Me.GroupHeader1.StylePriority.UseFont = False
        '
        'DetailReport
        '
        Me.DetailReport.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.DetailChild})
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
        Me.DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand
        '
        'DetailChild
        '
        Me.DetailChild.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTableDetails})
        Me.DetailChild.HeightF = 22.0!
        Me.DetailChild.KeepTogether = True
        Me.DetailChild.MultiColumn.ColumnCount = 6
        Me.DetailChild.MultiColumn.Mode = DevExpress.XtraReports.UI.MultiColumnMode.UseColumnWidth
        Me.DetailChild.Name = "DetailChild"
        '
        'GroupHeader2
        '
        Me.GroupHeader2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrWSStartDateTimeLabel})
        Me.GroupHeader2.HeightF = 24.0!
        Me.GroupHeader2.Level = 1
        Me.GroupHeader2.Name = "GroupHeader2"
        '
        'XrWSStartDateTimeLabel
        '
        Me.XrWSStartDateTimeLabel.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrWSStartDateTimeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrWSStartDateTimeLabel.LocationFloat = New DevExpress.Utils.PointFloat(501.0!, 0.0!)
        Me.XrWSStartDateTimeLabel.Name = "XrWSStartDateTimeLabel"
        Me.XrWSStartDateTimeLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrWSStartDateTimeLabel.SizeF = New System.Drawing.SizeF(171.2499!, 20.0!)
        Me.XrWSStartDateTimeLabel.StylePriority.UseBorders = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseFont = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseTextAlignment = False
        Me.XrWSStartDateTimeLabel.Text = "WSStartDateTime"
        Me.XrWSStartDateTimeLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrTableHeader
        '
        Me.XrTableHeader.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Top
        Me.XrTableHeader.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrTableHeader.BorderWidth = 2
        Me.XrTableHeader.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold)
        Me.XrTableHeader.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 4.0!)
        Me.XrTableHeader.Name = "XrTableHeader"
        Me.XrTableHeader.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRowHeader})
        Me.XrTableHeader.SizeF = New System.Drawing.SizeF(672.9999!, 20.0!)
        Me.XrTableHeader.StylePriority.UseBorders = False
        Me.XrTableHeader.StylePriority.UseBorderWidth = False
        Me.XrTableHeader.StylePriority.UseFont = False
        '
        'XrTableRowHeader
        '
        Me.XrTableRowHeader.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableHeaderCell})
        Me.XrTableRowHeader.Name = "XrTableRowHeader"
        Me.XrTableRowHeader.Weight = 1.0R
        '
        'XrTableHeaderCell
        '
        Me.XrTableHeaderCell.Name = "XrTableHeaderCell"
        Me.XrTableHeaderCell.Text = "XrTableHeaderCell"
        Me.XrTableHeaderCell.Weight = 3.514285423642113R
        '
        'XrTableCell
        '
        Me.XrTableCell.Name = "XrTableCell"
        Me.XrTableCell.Text = "XrTableCell"
        Me.XrTableCell.Weight = 3.5142857142857142R
        '
        'XrTableRowDetails
        '
        Me.XrTableRowDetails.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrTableRowDetails.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell})
        Me.XrTableRowDetails.Name = "XrTableRowDetails"
        Me.XrTableRowDetails.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 3, 0, 100.0!)
        Me.XrTableRowDetails.StylePriority.UseBorders = False
        Me.XrTableRowDetails.StylePriority.UsePadding = False
        Me.XrTableRowDetails.StylePriority.UseTextAlignment = False
        Me.XrTableRowDetails.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableRowDetails.Weight = 1.1R
        '
        'XrTableDetails
        '
        Me.XrTableDetails.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Top
        Me.XrTableDetails.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrTableDetails.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrTableDetails.Name = "XrTableDetails"
        Me.XrTableDetails.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRowDetails})
        Me.XrTableDetails.SizeF = New System.Drawing.SizeF(673.0!, 22.0!)
        Me.XrTableDetails.StylePriority.UseBorders = False
        '
        'SummaryResultsReport2
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport, Me.GroupHeader2})
        Me.Margins = New System.Drawing.Printing.Margins(63, 81, 96, 25)
        Me.PageHeight = 1169
        Me.PageWidth = 827
        Me.PaperKind = System.Drawing.Printing.PaperKind.A4
        Me.StyleSheet.AddRange(New DevExpress.XtraReports.UI.XRControlStyle() {Me.XrControlStyle1})
        Me.Version = "10.2"
        CType(Me.XrTableHeader, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTableDetails, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents XrControlStyle1 As DevExpress.XtraReports.UI.XRControlStyle
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents DetailReport As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents DetailChild As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents XrTableHeader As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRowHeader As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableHeaderCell As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents GroupHeader2 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents XrWSStartDateTimeLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTableDetails As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRowDetails As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell As DevExpress.XtraReports.UI.XRTableCell
End Class
