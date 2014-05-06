<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class QCCumulatedResultsByTestControlReport
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
        Dim XyDiagram1 As DevExpress.XtraCharts.XYDiagram = New DevExpress.XtraCharts.XYDiagram
        Dim RectangleGradientFillOptions1 As DevExpress.XtraCharts.RectangleGradientFillOptions = New DevExpress.XtraCharts.RectangleGradientFillOptions
        Dim Series1 As DevExpress.XtraCharts.Series = New DevExpress.XtraCharts.Series
        Dim PointSeriesLabel1 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel
        Dim PointOptions1 As DevExpress.XtraCharts.PointOptions = New DevExpress.XtraCharts.PointOptions
        Dim PointOptions2 As DevExpress.XtraCharts.PointOptions = New DevExpress.XtraCharts.PointOptions
        Dim LineSeriesView1 As DevExpress.XtraCharts.LineSeriesView = New DevExpress.XtraCharts.LineSeriesView
        Dim PointSeriesLabel2 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel
        Dim PointOptions3 As DevExpress.XtraCharts.PointOptions = New DevExpress.XtraCharts.PointOptions
        Dim LineSeriesView2 As DevExpress.XtraCharts.LineSeriesView = New DevExpress.XtraCharts.LineSeriesView
        Dim ChartTitle1 As DevExpress.XtraCharts.ChartTitle = New DevExpress.XtraCharts.ChartTitle
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand
        Me.XrTableReport = New DevExpress.XtraReports.UI.XRTable
        Me.XrTableReportRow = New DevExpress.XtraReports.UI.XRTableRow
        Me.XrCellVisibleCumResultsNum = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrCellCumDateTime = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrCellTotalRuns = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrCellMean = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrCellMeasureUnit = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrCellSD = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrCellCV = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrCellRange = New DevExpress.XtraReports.UI.XRTableCell
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand
        Me.GroupFooter1 = New DevExpress.XtraReports.UI.GroupFooterBand
        Me.XrLJGraph = New DevExpress.XtraReports.UI.XRChart
        Me.XrPanel3 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrGraphHeaderLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.ReportHeader = New DevExpress.XtraReports.UI.ReportHeaderBand
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrLabelCellCV = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCellMean = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCellMeasureUnit = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCellTotalRuns = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCellRange = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCellSD = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCellCumDateTime = New DevExpress.XtraReports.UI.XRLabel
        Me.XrPanel4 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrN = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelN = New DevExpress.XtraReports.UI.XRLabel
        Me.XrRanges = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelRanges = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCV = New DevExpress.XtraReports.UI.XRLabel
        Me.XrCV = New DevExpress.XtraReports.UI.XRLabel
        Me.XrSD = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelSD = New DevExpress.XtraReports.UI.XRLabel
        Me.XrUnit = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelUnit = New DevExpress.XtraReports.UI.XRLabel
        Me.XrMean = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLotNumber = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelMean = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelLotNumber = New DevExpress.XtraReports.UI.XRLabel
        Me.XrControlName = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelControlName = New DevExpress.XtraReports.UI.XRLabel
        Me.CumulatedResultsDS1 = New Biosystems.Ax00.Types.CumulatedResultsDS
        Me.ControlLotID = New DevExpress.XtraReports.Parameters.Parameter
        CType(Me.XrTableReport, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrLJGraph, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(LineSeriesView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(LineSeriesView2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CumulatedResultsDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTableReport})
        Me.Detail.HeightF = 22.0!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrTableReport
        '
        Me.XrTableReport.LocationFloat = New DevExpress.Utils.PointFloat(12.5!, 0.0!)
        Me.XrTableReport.Name = "XrTableReport"
        Me.XrTableReport.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableReportRow})
        Me.XrTableReport.SizeF = New System.Drawing.SizeF(630.0!, 22.0!)
        '
        'XrTableReportRow
        '
        Me.XrTableReportRow.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrCellVisibleCumResultsNum, Me.XrCellCumDateTime, Me.XrCellTotalRuns, Me.XrCellMean, Me.XrCellMeasureUnit, Me.XrCellSD, Me.XrCellCV, Me.XrCellRange})
        Me.XrTableReportRow.Name = "XrTableReportRow"
        Me.XrTableReportRow.Weight = 0.88
        '
        'XrCellVisibleCumResultsNum
        '
        Me.XrCellVisibleCumResultsNum.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcCumulatedResults.VisibleCumResultNumber")})
        Me.XrCellVisibleCumResultsNum.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellVisibleCumResultsNum.Name = "XrCellVisibleCumResultsNum"
        Me.XrCellVisibleCumResultsNum.StylePriority.UseFont = False
        Me.XrCellVisibleCumResultsNum.Text = "XrCellVisibleCumResultsNum"
        Me.XrCellVisibleCumResultsNum.Weight = 0.10476190476190478
        '
        'XrCellCumDateTime
        '
        Me.XrCellCumDateTime.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcCumulatedResults.CumDateTime")})
        Me.XrCellCumDateTime.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellCumDateTime.Name = "XrCellCumDateTime"
        Me.XrCellCumDateTime.StylePriority.UseFont = False
        Me.XrCellCumDateTime.Text = "XrCellCumDateTime"
        Me.XrCellCumDateTime.Weight = 0.77380850655691946
        '
        'XrCellTotalRuns
        '
        Me.XrCellTotalRuns.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcCumulatedResults.TotalRuns")})
        Me.XrCellTotalRuns.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellTotalRuns.Name = "XrCellTotalRuns"
        Me.XrCellTotalRuns.StylePriority.UseFont = False
        Me.XrCellTotalRuns.StylePriority.UseTextAlignment = False
        Me.XrCellTotalRuns.Text = "XrCellTotalRuns"
        Me.XrCellTotalRuns.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrCellTotalRuns.Weight = 0.076389785039992747
        '
        'XrCellMean
        '
        Me.XrCellMean.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcCumulatedResults.Mean")})
        Me.XrCellMean.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellMean.Name = "XrCellMean"
        Me.XrCellMean.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 2, 0, 0, 100.0!)
        Me.XrCellMean.StylePriority.UseFont = False
        Me.XrCellMean.StylePriority.UsePadding = False
        Me.XrCellMean.StylePriority.UseTextAlignment = False
        Me.XrCellMean.Text = "XrCellMean"
        Me.XrCellMean.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrCellMean.Weight = 0.42271895635695672
        '
        'XrCellMeasureUnit
        '
        Me.XrCellMeasureUnit.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcCumulatedResults.MeasureUnit")})
        Me.XrCellMeasureUnit.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellMeasureUnit.Name = "XrCellMeasureUnit"
        Me.XrCellMeasureUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0, 100.0!)
        Me.XrCellMeasureUnit.StylePriority.UseFont = False
        Me.XrCellMeasureUnit.StylePriority.UsePadding = False
        Me.XrCellMeasureUnit.StylePriority.UseTextAlignment = False
        Me.XrCellMeasureUnit.Text = "XrCellMeasureUnit"
        Me.XrCellMeasureUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrCellMeasureUnit.Weight = 0.32961308615548279
        '
        'XrCellSD
        '
        Me.XrCellSD.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcCumulatedResults.SD")})
        Me.XrCellSD.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellSD.Name = "XrCellSD"
        Me.XrCellSD.StylePriority.UseFont = False
        Me.XrCellSD.StylePriority.UseTextAlignment = False
        Me.XrCellSD.Text = "XrCellSD"
        Me.XrCellSD.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrCellSD.Weight = 0.377301465897333
        '
        'XrCellCV
        '
        Me.XrCellCV.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcCumulatedResults.CV")})
        Me.XrCellCV.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellCV.Name = "XrCellCV"
        Me.XrCellCV.Padding = New DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0, 100.0!)
        Me.XrCellCV.StylePriority.UseFont = False
        Me.XrCellCV.StylePriority.UsePadding = False
        Me.XrCellCV.StylePriority.UseTextAlignment = False
        Me.XrCellCV.Text = "XrCellCV"
        Me.XrCellCV.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrCellCV.Weight = 0.364287315096174
        '
        'XrCellRange
        '
        Me.XrCellRange.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcCumulatedResults.Range")})
        Me.XrCellRange.Name = "XrCellRange"
        Me.XrCellRange.Padding = New DevExpress.XtraPrinting.PaddingInfo(20, 0, 0, 0, 100.0!)
        Me.XrCellRange.StylePriority.UsePadding = False
        Me.XrCellRange.Text = "XrCellRange"
        Me.XrCellRange.Weight = 0.5511189801352363
        '
        'TopMargin
        '
        Me.TopMargin.HeightF = 35.0!
        Me.TopMargin.Name = "TopMargin"
        Me.TopMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'BottomMargin
        '
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'GroupFooter1
        '
        Me.GroupFooter1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLJGraph, Me.XrPanel3})
        Me.GroupFooter1.HeightF = 348.9583!
        Me.GroupFooter1.Name = "GroupFooter1"
        '
        'XrLJGraph
        '
        Me.XrLJGraph.AppearanceName = "Light"
        Me.XrLJGraph.BorderColor = System.Drawing.Color.DarkGray
        Me.XrLJGraph.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        XyDiagram1.AxisX.GridLines.Visible = True
        XyDiagram1.AxisX.Range.SideMarginsEnabled = True
        XyDiagram1.AxisX.VisibleInPanesSerializable = "-1"
        XyDiagram1.AxisY.Range.SideMarginsEnabled = True
        XyDiagram1.AxisY.VisibleInPanesSerializable = "-1"
        Me.XrLJGraph.Diagram = XyDiagram1
        Me.XrLJGraph.EmptyChartText.Font = New System.Drawing.Font("Verdana", 12.0!)
        Me.XrLJGraph.FillStyle.FillMode = DevExpress.XtraCharts.FillMode.Gradient
        RectangleGradientFillOptions1.Color2 = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        RectangleGradientFillOptions1.GradientMode = DevExpress.XtraCharts.RectangleGradientMode.TopLeftToBottomRight
        Me.XrLJGraph.FillStyle.Options = RectangleGradientFillOptions1
        Me.XrLJGraph.IndicatorsPaletteName = "Default"
        Me.XrLJGraph.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Center
        Me.XrLJGraph.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Bottom
        Me.XrLJGraph.Legend.Direction = DevExpress.XtraCharts.LegendDirection.LeftToRight
        Me.XrLJGraph.Legend.EquallySpacedItems = False
        Me.XrLJGraph.Legend.Visible = False
        Me.XrLJGraph.LocationFloat = New DevExpress.Utils.PointFloat(12.5!, 52.50003!)
        Me.XrLJGraph.Name = "XrLJGraph"
        Me.XrLJGraph.PaletteBaseColorNumber = 4
        Me.XrLJGraph.PaletteName = "Black and White"
        Series1.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Numerical
        PointSeriesLabel1.LineVisible = True
        PointSeriesLabel1.Visible = False
        Series1.Label = PointSeriesLabel1
        Series1.LegendPointOptions = PointOptions1
        Series1.Name = "Series 1"
        PointOptions2.PointView = DevExpress.XtraCharts.PointView.ArgumentAndValues
        Series1.PointOptions = PointOptions2
        Series1.ShowInLegend = False
        Series1.SynchronizePointOptions = False
        LineSeriesView1.LineMarkerOptions.BorderVisible = False
        LineSeriesView1.LineMarkerOptions.Size = 1
        Series1.View = LineSeriesView1
        Me.XrLJGraph.SeriesSerializable = New DevExpress.XtraCharts.Series() {Series1}
        PointSeriesLabel2.LineVisible = True
        Me.XrLJGraph.SeriesTemplate.Label = PointSeriesLabel2
        Me.XrLJGraph.SeriesTemplate.LegendPointOptions = PointOptions3
        Me.XrLJGraph.SeriesTemplate.SynchronizePointOptions = False
        Me.XrLJGraph.SeriesTemplate.View = LineSeriesView2
        Me.XrLJGraph.SizeF = New System.Drawing.SizeF(627.4996!, 296.4583!)
        Me.XrLJGraph.StylePriority.UseBackColor = False
        Me.XrLJGraph.StylePriority.UseBorderColor = False
        Me.XrLJGraph.StylePriority.UseBorders = False
        ChartTitle1.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ChartTitle1.Text = "Linear"
        ChartTitle1.Visible = False
        Me.XrLJGraph.Titles.AddRange(New DevExpress.XtraCharts.ChartTitle() {ChartTitle1})
        '
        'XrPanel3
        '
        Me.XrPanel3.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel3.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel3.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel3.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrGraphHeaderLabel})
        Me.XrPanel3.LocationFloat = New DevExpress.Utils.PointFloat(12.5!, 10.0!)
        Me.XrPanel3.Name = "XrPanel3"
        Me.XrPanel3.SizeF = New System.Drawing.SizeF(627.4996!, 29.99999!)
        Me.XrPanel3.StylePriority.UseBackColor = False
        Me.XrPanel3.StylePriority.UseBorderColor = False
        Me.XrPanel3.StylePriority.UseBorders = False
        '
        'XrGraphHeaderLabel
        '
        Me.XrGraphHeaderLabel.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrGraphHeaderLabel.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrGraphHeaderLabel.LocationFloat = New DevExpress.Utils.PointFloat(9.999974!, 6.00001!)
        Me.XrGraphHeaderLabel.Name = "XrGraphHeaderLabel"
        Me.XrGraphHeaderLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrGraphHeaderLabel.SizeF = New System.Drawing.SizeF(607.4996!, 20.00001!)
        Me.XrGraphHeaderLabel.StylePriority.UseBorders = False
        Me.XrGraphHeaderLabel.StylePriority.UseFont = False
        Me.XrGraphHeaderLabel.Text = "XrLabelTestName"
        '
        'ReportHeader
        '
        Me.ReportHeader.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel2, Me.XrPanel4})
        Me.ReportHeader.HeightF = 92.70834!
        Me.ReportHeader.Name = "ReportHeader"
        '
        'XrPanel2
        '
        Me.XrPanel2.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel2.BorderWidth = 2
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelCellCV, Me.XrLabelCellMean, Me.XrLabelCellMeasureUnit, Me.XrLabelCellTotalRuns, Me.XrLabelCellRange, Me.XrLabelCellSD, Me.XrLabelCellCumDateTime})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(12.5!, 69.58333!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(630.0!, 23.12501!)
        Me.XrPanel2.StylePriority.UseBorders = False
        Me.XrPanel2.StylePriority.UseBorderWidth = False
        '
        'XrLabelCellCV
        '
        Me.XrLabelCellCV.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCellCV.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCellCV.LocationFloat = New DevExpress.Utils.PointFloat(437.7647!, 0.00005340576!)
        Me.XrLabelCellCV.Name = "XrLabelCellCV"
        Me.XrLabelCellCV.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.XrLabelCellCV.SizeF = New System.Drawing.SizeF(76.5004!, 20.00002!)
        Me.XrLabelCellCV.StylePriority.UseBorders = False
        Me.XrLabelCellCV.StylePriority.UseFont = False
        Me.XrLabelCellCV.StylePriority.UsePadding = False
        Me.XrLabelCellCV.StylePriority.UseTextAlignment = False
        Me.XrLabelCellCV.Text = "CV"
        Me.XrLabelCellCV.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelCellMean
        '
        Me.XrLabelCellMean.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCellMean.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCellMean.LocationFloat = New DevExpress.Utils.PointFloat(200.5416!, 0.00005340576!)
        Me.XrLabelCellMean.Name = "XrLabelCellMean"
        Me.XrLabelCellMean.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 5, 0, 0, 100.0!)
        Me.XrLabelCellMean.SizeF = New System.Drawing.SizeF(88.771!, 19.99995!)
        Me.XrLabelCellMean.StylePriority.UseBorders = False
        Me.XrLabelCellMean.StylePriority.UseFont = False
        Me.XrLabelCellMean.StylePriority.UsePadding = False
        Me.XrLabelCellMean.StylePriority.UseTextAlignment = False
        Me.XrLabelCellMean.Text = "Mean"
        Me.XrLabelCellMean.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelCellMeasureUnit
        '
        Me.XrLabelCellMeasureUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCellMeasureUnit.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCellMeasureUnit.LocationFloat = New DevExpress.Utils.PointFloat(289.3126!, 0.0!)
        Me.XrLabelCellMeasureUnit.Name = "XrLabelCellMeasureUnit"
        Me.XrLabelCellMeasureUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0, 100.0!)
        Me.XrLabelCellMeasureUnit.SizeF = New System.Drawing.SizeF(69.21878!, 20.00002!)
        Me.XrLabelCellMeasureUnit.StylePriority.UseBorders = False
        Me.XrLabelCellMeasureUnit.StylePriority.UseFont = False
        Me.XrLabelCellMeasureUnit.StylePriority.UsePadding = False
        Me.XrLabelCellMeasureUnit.StylePriority.UseTextAlignment = False
        Me.XrLabelCellMeasureUnit.Text = "Unit"
        Me.XrLabelCellMeasureUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelCellTotalRuns
        '
        Me.XrLabelCellTotalRuns.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCellTotalRuns.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCellTotalRuns.LocationFloat = New DevExpress.Utils.PointFloat(184.4997!, 0.00005340576!)
        Me.XrLabelCellTotalRuns.Name = "XrLabelCellTotalRuns"
        Me.XrLabelCellTotalRuns.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.XrLabelCellTotalRuns.SizeF = New System.Drawing.SizeF(16.04195!, 20.00002!)
        Me.XrLabelCellTotalRuns.StylePriority.UseBorders = False
        Me.XrLabelCellTotalRuns.StylePriority.UseFont = False
        Me.XrLabelCellTotalRuns.StylePriority.UsePadding = False
        Me.XrLabelCellTotalRuns.StylePriority.UseTextAlignment = False
        Me.XrLabelCellTotalRuns.Text = "n"
        Me.XrLabelCellTotalRuns.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelCellRange
        '
        Me.XrLabelCellRange.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCellRange.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold)
        Me.XrLabelCellRange.LocationFloat = New DevExpress.Utils.PointFloat(514.265!, 0.0!)
        Me.XrLabelCellRange.Name = "XrLabelCellRange"
        Me.XrLabelCellRange.Padding = New DevExpress.XtraPrinting.PaddingInfo(20, 0, 0, 0, 100.0!)
        Me.XrLabelCellRange.SizeF = New System.Drawing.SizeF(113.235!, 20.00002!)
        Me.XrLabelCellRange.StylePriority.UseBorders = False
        Me.XrLabelCellRange.StylePriority.UseFont = False
        Me.XrLabelCellRange.StylePriority.UsePadding = False
        Me.XrLabelCellRange.StylePriority.UseTextAlignment = False
        Me.XrLabelCellRange.Text = "Ranges"
        Me.XrLabelCellRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelCellSD
        '
        Me.XrLabelCellSD.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCellSD.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCellSD.LocationFloat = New DevExpress.Utils.PointFloat(358.5314!, 0.0!)
        Me.XrLabelCellSD.Name = "XrLabelCellSD"
        Me.XrLabelCellSD.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.XrLabelCellSD.SizeF = New System.Drawing.SizeF(79.23331!, 20.00002!)
        Me.XrLabelCellSD.StylePriority.UseBorders = False
        Me.XrLabelCellSD.StylePriority.UseFont = False
        Me.XrLabelCellSD.StylePriority.UsePadding = False
        Me.XrLabelCellSD.StylePriority.UseTextAlignment = False
        Me.XrLabelCellSD.Text = "SD"
        Me.XrLabelCellSD.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelCellCumDateTime
        '
        Me.XrLabelCellCumDateTime.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCellCumDateTime.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCellCumDateTime.LocationFloat = New DevExpress.Utils.PointFloat(22.0!, 0.0!)
        Me.XrLabelCellCumDateTime.Name = "XrLabelCellCumDateTime"
        Me.XrLabelCellCumDateTime.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.XrLabelCellCumDateTime.SizeF = New System.Drawing.SizeF(162.4997!, 20.00002!)
        Me.XrLabelCellCumDateTime.StylePriority.UseBorders = False
        Me.XrLabelCellCumDateTime.StylePriority.UseFont = False
        Me.XrLabelCellCumDateTime.StylePriority.UsePadding = False
        Me.XrLabelCellCumDateTime.Text = "Date"
        '
        'XrPanel4
        '
        Me.XrPanel4.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel4.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel4.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel4.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrN, Me.XrLabelN, Me.XrRanges, Me.XrLabelRanges, Me.XrLabelCV, Me.XrCV, Me.XrSD, Me.XrLabelSD, Me.XrUnit, Me.XrLabelUnit, Me.XrMean, Me.XrLotNumber, Me.XrLabelMean, Me.XrLabelLotNumber, Me.XrControlName, Me.XrLabelControlName})
        Me.XrPanel4.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrPanel4.Name = "XrPanel4"
        Me.XrPanel4.SizeF = New System.Drawing.SizeF(649.9998!, 49.79167!)
        Me.XrPanel4.StylePriority.UseBackColor = False
        Me.XrPanel4.StylePriority.UseBorderColor = False
        Me.XrPanel4.StylePriority.UseBorders = False
        '
        'XrN
        '
        Me.XrN.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrN.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrN.LocationFloat = New DevExpress.Utils.PointFloat(303.8127!, 25.0!)
        Me.XrN.Name = "XrN"
        Me.XrN.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrN.SizeF = New System.Drawing.SizeF(21.78537!, 20.0!)
        Me.XrN.StylePriority.UseBorders = False
        Me.XrN.StylePriority.UseFont = False
        Me.XrN.StylePriority.UseTextAlignment = False
        Me.XrN.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrLabelN
        '
        Me.XrLabelN.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelN.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelN.LocationFloat = New DevExpress.Utils.PointFloat(303.8126!, 5.000051!)
        Me.XrLabelN.Name = "XrLabelN"
        Me.XrLabelN.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelN.SizeF = New System.Drawing.SizeF(21.78549!, 19.99998!)
        Me.XrLabelN.StylePriority.UseBorders = False
        Me.XrLabelN.StylePriority.UseFont = False
        Me.XrLabelN.StylePriority.UseTextAlignment = False
        Me.XrLabelN.Text = "N"
        Me.XrLabelN.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrRanges
        '
        Me.XrRanges.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrRanges.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrRanges.LocationFloat = New DevExpress.Utils.PointFloat(557.8899!, 25.0!)
        Me.XrRanges.Name = "XrRanges"
        Me.XrRanges.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrRanges.SizeF = New System.Drawing.SizeF(90.10992!, 20.00002!)
        Me.XrRanges.StylePriority.UseBorders = False
        Me.XrRanges.StylePriority.UseFont = False
        Me.XrRanges.StylePriority.UseTextAlignment = False
        Me.XrRanges.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelRanges
        '
        Me.XrLabelRanges.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelRanges.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelRanges.LocationFloat = New DevExpress.Utils.PointFloat(557.8899!, 5.000035!)
        Me.XrLabelRanges.Name = "XrLabelRanges"
        Me.XrLabelRanges.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelRanges.SizeF = New System.Drawing.SizeF(90.10992!, 19.99998!)
        Me.XrLabelRanges.StylePriority.UseBorders = False
        Me.XrLabelRanges.StylePriority.UseFont = False
        Me.XrLabelRanges.StylePriority.UseTextAlignment = False
        Me.XrLabelRanges.Text = "Ranges"
        Me.XrLabelRanges.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelCV
        '
        Me.XrLabelCV.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCV.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCV.LocationFloat = New DevExpress.Utils.PointFloat(510.0982!, 5.000035!)
        Me.XrLabelCV.Name = "XrLabelCV"
        Me.XrLabelCV.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCV.SizeF = New System.Drawing.SizeF(44.79169!, 19.99998!)
        Me.XrLabelCV.StylePriority.UseBorders = False
        Me.XrLabelCV.StylePriority.UseFont = False
        Me.XrLabelCV.StylePriority.UseTextAlignment = False
        Me.XrLabelCV.Text = "CV"
        Me.XrLabelCV.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrCV
        '
        Me.XrCV.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrCV.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrCV.LocationFloat = New DevExpress.Utils.PointFloat(510.0982!, 25.0!)
        Me.XrCV.Name = "XrCV"
        Me.XrCV.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrCV.SizeF = New System.Drawing.SizeF(44.79166!, 19.99998!)
        Me.XrCV.StylePriority.UseBorders = False
        Me.XrCV.StylePriority.UseFont = False
        Me.XrCV.StylePriority.UseTextAlignment = False
        Me.XrCV.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrSD
        '
        Me.XrSD.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrSD.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrSD.LocationFloat = New DevExpress.Utils.PointFloat(465.3064!, 25.0!)
        Me.XrSD.Name = "XrSD"
        Me.XrSD.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrSD.SizeF = New System.Drawing.SizeF(44.79172!, 20.0!)
        Me.XrSD.StylePriority.UseBorders = False
        Me.XrSD.StylePriority.UseFont = False
        Me.XrSD.StylePriority.UseTextAlignment = False
        Me.XrSD.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelSD
        '
        Me.XrLabelSD.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelSD.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelSD.LocationFloat = New DevExpress.Utils.PointFloat(465.3064!, 5.000035!)
        Me.XrLabelSD.Name = "XrLabelSD"
        Me.XrLabelSD.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelSD.SizeF = New System.Drawing.SizeF(44.79166!, 19.99998!)
        Me.XrLabelSD.StylePriority.UseBorders = False
        Me.XrLabelSD.StylePriority.UseFont = False
        Me.XrLabelSD.StylePriority.UseTextAlignment = False
        Me.XrLabelSD.Text = "SD"
        Me.XrLabelSD.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrUnit
        '
        Me.XrUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrUnit.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrUnit.LocationFloat = New DevExpress.Utils.PointFloat(398.5147!, 25.0!)
        Me.XrUnit.Name = "XrUnit"
        Me.XrUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrUnit.SizeF = New System.Drawing.SizeF(66.79172!, 20.0!)
        Me.XrUnit.StylePriority.UseBorders = False
        Me.XrUnit.StylePriority.UseFont = False
        Me.XrUnit.StylePriority.UseTextAlignment = False
        Me.XrUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrLabelUnit
        '
        Me.XrLabelUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelUnit.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelUnit.LocationFloat = New DevExpress.Utils.PointFloat(398.5147!, 5.000051!)
        Me.XrLabelUnit.Name = "XrLabelUnit"
        Me.XrLabelUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelUnit.SizeF = New System.Drawing.SizeF(66.79172!, 19.99998!)
        Me.XrLabelUnit.StylePriority.UseBorders = False
        Me.XrLabelUnit.StylePriority.UseFont = False
        Me.XrLabelUnit.StylePriority.UseTextAlignment = False
        Me.XrLabelUnit.Text = "Unit"
        Me.XrLabelUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrMean
        '
        Me.XrMean.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrMean.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrMean.LocationFloat = New DevExpress.Utils.PointFloat(325.598!, 25.0!)
        Me.XrMean.Name = "XrMean"
        Me.XrMean.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrMean.SizeF = New System.Drawing.SizeF(72.91669!, 20.00002!)
        Me.XrMean.StylePriority.UseBorders = False
        Me.XrMean.StylePriority.UseFont = False
        Me.XrMean.StylePriority.UseTextAlignment = False
        Me.XrMean.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLotNumber
        '
        Me.XrLotNumber.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLotNumber.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrLotNumber.LocationFloat = New DevExpress.Utils.PointFloat(175.6459!, 25.00003!)
        Me.XrLotNumber.Name = "XrLotNumber"
        Me.XrLotNumber.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLotNumber.SizeF = New System.Drawing.SizeF(126.1667!, 20.0!)
        Me.XrLotNumber.StylePriority.UseBorders = False
        Me.XrLotNumber.StylePriority.UseFont = False
        '
        'XrLabelMean
        '
        Me.XrLabelMean.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelMean.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelMean.LocationFloat = New DevExpress.Utils.PointFloat(325.5981!, 5.000051!)
        Me.XrLabelMean.Name = "XrLabelMean"
        Me.XrLabelMean.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelMean.SizeF = New System.Drawing.SizeF(72.9166!, 19.99998!)
        Me.XrLabelMean.StylePriority.UseBorders = False
        Me.XrLabelMean.StylePriority.UseFont = False
        Me.XrLabelMean.StylePriority.UseTextAlignment = False
        Me.XrLabelMean.Text = "Mean"
        Me.XrLabelMean.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelLotNumber
        '
        Me.XrLabelLotNumber.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelLotNumber.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelLotNumber.LocationFloat = New DevExpress.Utils.PointFloat(175.6459!, 4.000012!)
        Me.XrLabelLotNumber.Name = "XrLabelLotNumber"
        Me.XrLabelLotNumber.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelLotNumber.SizeF = New System.Drawing.SizeF(126.1667!, 20.0!)
        Me.XrLabelLotNumber.StylePriority.UseBorders = False
        Me.XrLabelLotNumber.StylePriority.UseFont = False
        Me.XrLabelLotNumber.Text = "Lot Number"
        '
        'XrControlName
        '
        Me.XrControlName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrControlName.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrControlName.LocationFloat = New DevExpress.Utils.PointFloat(1.999974!, 25.00002!)
        Me.XrControlName.Name = "XrControlName"
        Me.XrControlName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrControlName.SizeF = New System.Drawing.SizeF(171.9583!, 20.00001!)
        Me.XrControlName.StylePriority.UseBorders = False
        Me.XrControlName.StylePriority.UseFont = False
        '
        'XrLabelControlName
        '
        Me.XrLabelControlName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelControlName.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelControlName.LocationFloat = New DevExpress.Utils.PointFloat(1.999974!, 4.000012!)
        Me.XrLabelControlName.Name = "XrLabelControlName"
        Me.XrLabelControlName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelControlName.SizeF = New System.Drawing.SizeF(170.9583!, 20.0!)
        Me.XrLabelControlName.StylePriority.UseBorders = False
        Me.XrLabelControlName.StylePriority.UseFont = False
        Me.XrLabelControlName.Text = "Control"
        '
        'CumulatedResultsDS1
        '
        Me.CumulatedResultsDS1.DataSetName = "CumulatedResultsDS"
        Me.CumulatedResultsDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ControlLotID
        '
        Me.ControlLotID.Name = "ControlLotID"
        Me.ControlLotID.Type = GetType(Integer)
        Me.ControlLotID.Value = 0
        Me.ControlLotID.Visible = False
        '
        'QCCumulatedResultsByTestControlReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupFooter1, Me.ReportHeader})
        Me.DataMember = "tqcCumulatedResults"
        Me.DataSource = Me.CumulatedResultsDS1
        Me.FilterString = "[QCControlLotID] = ?ControlLotID"
        Me.Margins = New System.Drawing.Printing.Margins(100, 100, 35, 100)
        Me.Parameters.AddRange(New DevExpress.XtraReports.Parameters.Parameter() {Me.ControlLotID})
        Me.Version = "10.2"
        CType(Me.XrTableReport, System.ComponentModel.ISupportInitialize).EndInit()
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(LineSeriesView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(LineSeriesView2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrLJGraph, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CumulatedResultsDS1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents GroupFooter1 As DevExpress.XtraReports.UI.GroupFooterBand
    Friend WithEvents XrPanel3 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrGraphHeaderLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLJGraph As DevExpress.XtraReports.UI.XRChart
    Friend WithEvents ReportHeader As DevExpress.XtraReports.UI.ReportHeaderBand
    Friend WithEvents XrPanel4 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrRanges As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelRanges As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCV As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrCV As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrSD As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelSD As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrMean As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLotNumber As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelMean As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelLotNumber As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrControlName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelControlName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents CumulatedResultsDS1 As Biosystems.Ax00.Types.CumulatedResultsDS
    Public WithEvents ControlLotID As DevExpress.XtraReports.Parameters.Parameter
    Friend WithEvents XrN As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelN As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTableReport As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableReportRow As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrCellVisibleCumResultsNum As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellCumDateTime As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellTotalRuns As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellMean As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellMeasureUnit As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellSD As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellCV As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrPanel2 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelCellMean As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCellMeasureUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCellTotalRuns As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCellRange As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCellSD As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCellCumDateTime As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrCellRange As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrLabelCellCV As DevExpress.XtraReports.UI.XRLabel
End Class
