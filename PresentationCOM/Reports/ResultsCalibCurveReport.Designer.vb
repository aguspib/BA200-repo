<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class ResultsCalibCurveReport
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
        Dim LineSeriesView1 As DevExpress.XtraCharts.LineSeriesView = New DevExpress.XtraCharts.LineSeriesView
        Dim Series2 As DevExpress.XtraCharts.Series = New DevExpress.XtraCharts.Series
        Dim PointSeriesLabel2 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel
        Dim PointOptions2 As DevExpress.XtraCharts.PointOptions = New DevExpress.XtraCharts.PointOptions
        Dim PointSeriesView1 As DevExpress.XtraCharts.PointSeriesView = New DevExpress.XtraCharts.PointSeriesView
        Dim PointSeriesLabel3 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel
        Dim PointOptions3 As DevExpress.XtraCharts.PointOptions = New DevExpress.XtraCharts.PointOptions
        Dim LineSeriesView2 As DevExpress.XtraCharts.LineSeriesView = New DevExpress.XtraCharts.LineSeriesView
        Dim ChartTitle1 As DevExpress.XtraCharts.ChartTitle = New DevExpress.XtraCharts.ChartTitle
        Dim ChartTitle2 As DevExpress.XtraCharts.ChartTitle = New DevExpress.XtraCharts.ChartTitle
        Dim ChartTitle3 As DevExpress.XtraCharts.ChartTitle = New DevExpress.XtraCharts.ChartTitle
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand
        Me.XrLabelNoData = New DevExpress.XtraReports.UI.XRLabel
        Me.XrChart1 = New DevExpress.XtraReports.UI.XRChart
        Me.XrPanel3 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrLabelUnitField = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCurveTypeField = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCurveGrowthField = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelYAxisField = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelXAxisField = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelUnit = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCurveType = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCurveGrowth = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelYAxis = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelXAxis = New DevExpress.XtraReports.UI.XRLabel
        Me.XrPanel1 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrLabelTestName = New DevExpress.XtraReports.UI.XRLabel
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrLabelCalcConc = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelError = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelTheorConc = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelAbs = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCalibNo = New DevExpress.XtraReports.UI.XRLabel
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand
        Me.XrControlStyle1 = New DevExpress.XtraReports.UI.XRControlStyle
        Me.XrHeaderLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand
        Me.XrWSStartDateTimeLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand
        Me.Detail1 = New DevExpress.XtraReports.UI.DetailBand
        Me.XrTableDetails = New DevExpress.XtraReports.UI.XRTable
        Me.XrTableDetailsRow = New DevExpress.XtraReports.UI.XRTableRow
        Me.XrTableCellClass = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell2 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell5 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell6 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell8 = New DevExpress.XtraReports.UI.XRTableCell
        Me.ResultsDS1 = New Biosystems.Ax00.Types.ResultsDS
        CType(Me.XrChart1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(LineSeriesView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Series2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(LineSeriesView2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTableDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ResultsDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelNoData, Me.XrChart1, Me.XrPanel3, Me.XrPanel1, Me.XrPanel2})
        Me.Detail.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail.HeightF = 555.0!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.StylePriority.UseFont = False
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelNoData
        '
        Me.XrLabelNoData.Font = New System.Drawing.Font("Verdana", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelNoData.LocationFloat = New DevExpress.Utils.PointFloat(95.0!, 200.0!)
        Me.XrLabelNoData.Name = "XrLabelNoData"
        Me.XrLabelNoData.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelNoData.SizeF = New System.Drawing.SizeF(500.0!, 65.70834!)
        Me.XrLabelNoData.StylePriority.UseFont = False
        Me.XrLabelNoData.StylePriority.UseTextAlignment = False
        Me.XrLabelNoData.Text = "XrLabelNoData"
        Me.XrLabelNoData.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        Me.XrLabelNoData.Visible = False
        '
        'XrChart1
        '
        Me.XrChart1.AppearanceName = "Light"
        Me.XrChart1.BorderColor = System.Drawing.Color.DarkGray
        Me.XrChart1.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        XyDiagram1.AxisX.GridLines.Visible = True
        XyDiagram1.AxisX.Range.SideMarginsEnabled = True
        XyDiagram1.AxisX.VisibleInPanesSerializable = "-1"
        XyDiagram1.AxisY.Range.SideMarginsEnabled = True
        XyDiagram1.AxisY.VisibleInPanesSerializable = "-1"
        Me.XrChart1.Diagram = XyDiagram1
        Me.XrChart1.FillStyle.FillMode = DevExpress.XtraCharts.FillMode.Gradient
        RectangleGradientFillOptions1.Color2 = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        RectangleGradientFillOptions1.GradientMode = DevExpress.XtraCharts.RectangleGradientMode.TopLeftToBottomRight
        Me.XrChart1.FillStyle.Options = RectangleGradientFillOptions1
        Me.XrChart1.IndicatorsPaletteName = "Default"
        Me.XrChart1.Legend.Visible = False
        Me.XrChart1.LocationFloat = New DevExpress.Utils.PointFloat(20.0!, 40.0!)
        Me.XrChart1.Name = "XrChart1"
        Me.XrChart1.PaletteBaseColorNumber = 4
        Me.XrChart1.PaletteName = "Black and White"
        Series1.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Numerical
        PointSeriesLabel1.LineVisible = True
        PointSeriesLabel1.Visible = False
        Series1.Label = PointSeriesLabel1
        Series1.LegendPointOptions = PointOptions1
        Series1.Name = "Series 1"
        Series1.ShowInLegend = False
        Series1.SynchronizePointOptions = False
        LineSeriesView1.LineMarkerOptions.BorderVisible = False
        LineSeriesView1.LineMarkerOptions.Size = 1
        Series1.View = LineSeriesView1
        Series2.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Numerical
        PointSeriesLabel2.Angle = 90
        PointSeriesLabel2.BackColor = System.Drawing.Color.Transparent
        PointSeriesLabel2.Border.Visible = False
        PointSeriesLabel2.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        PointSeriesLabel2.LineVisible = False
        PointSeriesLabel2.ResolveOverlappingMode = DevExpress.XtraCharts.ResolveOverlappingMode.JustifyAroundPoint
        PointSeriesLabel2.TextColor = System.Drawing.Color.Black
        Series2.Label = PointSeriesLabel2
        Series2.LegendPointOptions = PointOptions2
        Series2.Name = "Series 2"
        Series2.ShowInLegend = False
        Series2.SynchronizePointOptions = False
        PointSeriesView1.PointMarkerOptions.BorderColor = System.Drawing.Color.Black
        PointSeriesView1.PointMarkerOptions.Kind = DevExpress.XtraCharts.MarkerKind.Cross
        PointSeriesView1.PointMarkerOptions.Size = 10
        Series2.View = PointSeriesView1
        Me.XrChart1.SeriesSerializable = New DevExpress.XtraCharts.Series() {Series1, Series2}
        PointSeriesLabel3.LineVisible = True
        Me.XrChart1.SeriesTemplate.Label = PointSeriesLabel3
        Me.XrChart1.SeriesTemplate.LegendPointOptions = PointOptions3
        Me.XrChart1.SeriesTemplate.SynchronizePointOptions = False
        Me.XrChart1.SeriesTemplate.View = LineSeriesView2
        Me.XrChart1.SizeF = New System.Drawing.SizeF(610.0!, 410.0!)
        Me.XrChart1.StylePriority.UseBackColor = False
        Me.XrChart1.StylePriority.UseBorderColor = False
        Me.XrChart1.StylePriority.UseBorders = False
        ChartTitle1.Dock = DevExpress.XtraCharts.ChartTitleDockStyle.Left
        ChartTitle1.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ChartTitle1.Indent = 0
        ChartTitle1.Text = "Absorbance"
        ChartTitle2.Dock = DevExpress.XtraCharts.ChartTitleDockStyle.Bottom
        ChartTitle2.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ChartTitle2.Indent = 0
        ChartTitle2.Text = "Concentration"
        ChartTitle3.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ChartTitle3.Text = "Linear"
        ChartTitle3.Visible = False
        Me.XrChart1.Titles.AddRange(New DevExpress.XtraCharts.ChartTitle() {ChartTitle1, ChartTitle2, ChartTitle3})
        '
        'XrPanel3
        '
        Me.XrPanel3.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel3.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel3.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel3.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelUnitField, Me.XrLabelCurveTypeField, Me.XrLabelCurveGrowthField, Me.XrLabelYAxisField, Me.XrLabelXAxisField, Me.XrLabelUnit, Me.XrLabelCurveType, Me.XrLabelCurveGrowth, Me.XrLabelYAxis, Me.XrLabelXAxis})
        Me.XrPanel3.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 465.0!)
        Me.XrPanel3.Name = "XrPanel3"
        Me.XrPanel3.SizeF = New System.Drawing.SizeF(650.0001!, 50.0!)
        Me.XrPanel3.StylePriority.UseBackColor = False
        Me.XrPanel3.StylePriority.UseBorderColor = False
        Me.XrPanel3.StylePriority.UseBorders = False
        '
        'XrLabelUnitField
        '
        Me.XrLabelUnitField.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelUnitField.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelUnitField.LocationFloat = New DevExpress.Utils.PointFloat(540.0!, 26.0!)
        Me.XrLabelUnitField.Name = "XrLabelUnitField"
        Me.XrLabelUnitField.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelUnitField.SizeF = New System.Drawing.SizeF(100.0!, 19.99997!)
        Me.XrLabelUnitField.StylePriority.UseBorders = False
        Me.XrLabelUnitField.StylePriority.UseFont = False
        Me.XrLabelUnitField.Text = "Unit"
        '
        'XrLabelCurveTypeField
        '
        Me.XrLabelCurveTypeField.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCurveTypeField.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCurveTypeField.LocationFloat = New DevExpress.Utils.PointFloat(392.0!, 26.0!)
        Me.XrLabelCurveTypeField.Name = "XrLabelCurveTypeField"
        Me.XrLabelCurveTypeField.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCurveTypeField.SizeF = New System.Drawing.SizeF(130.0!, 19.99997!)
        Me.XrLabelCurveTypeField.StylePriority.UseBorders = False
        Me.XrLabelCurveTypeField.StylePriority.UseFont = False
        Me.XrLabelCurveTypeField.Text = "Curve Type"
        '
        'XrLabelCurveGrowthField
        '
        Me.XrLabelCurveGrowthField.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCurveGrowthField.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCurveGrowthField.LocationFloat = New DevExpress.Utils.PointFloat(244.7917!, 26.0!)
        Me.XrLabelCurveGrowthField.Name = "XrLabelCurveGrowthField"
        Me.XrLabelCurveGrowthField.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCurveGrowthField.SizeF = New System.Drawing.SizeF(133.3334!, 19.99997!)
        Me.XrLabelCurveGrowthField.StylePriority.UseBorders = False
        Me.XrLabelCurveGrowthField.StylePriority.UseFont = False
        Me.XrLabelCurveGrowthField.Text = "Curve Growth"
        '
        'XrLabelYAxisField
        '
        Me.XrLabelYAxisField.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelYAxisField.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelYAxisField.LocationFloat = New DevExpress.Utils.PointFloat(128.125!, 26.0!)
        Me.XrLabelYAxisField.Name = "XrLabelYAxisField"
        Me.XrLabelYAxisField.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelYAxisField.SizeF = New System.Drawing.SizeF(100.0!, 19.99997!)
        Me.XrLabelYAxisField.StylePriority.UseBorders = False
        Me.XrLabelYAxisField.StylePriority.UseFont = False
        Me.XrLabelYAxisField.Text = "Y Axis"
        '
        'XrLabelXAxisField
        '
        Me.XrLabelXAxisField.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelXAxisField.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelXAxisField.LocationFloat = New DevExpress.Utils.PointFloat(10.00001!, 26.0!)
        Me.XrLabelXAxisField.Name = "XrLabelXAxisField"
        Me.XrLabelXAxisField.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelXAxisField.SizeF = New System.Drawing.SizeF(100.0!, 19.99997!)
        Me.XrLabelXAxisField.StylePriority.UseBorders = False
        Me.XrLabelXAxisField.StylePriority.UseFont = False
        Me.XrLabelXAxisField.Text = "X Axis"
        '
        'XrLabelUnit
        '
        Me.XrLabelUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelUnit.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelUnit.LocationFloat = New DevExpress.Utils.PointFloat(540.0!, 5.0!)
        Me.XrLabelUnit.Name = "XrLabelUnit"
        Me.XrLabelUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelUnit.SizeF = New System.Drawing.SizeF(100.0!, 19.99997!)
        Me.XrLabelUnit.StylePriority.UseBorders = False
        Me.XrLabelUnit.StylePriority.UseFont = False
        Me.XrLabelUnit.Text = "Unit"
        '
        'XrLabelCurveType
        '
        Me.XrLabelCurveType.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCurveType.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCurveType.LocationFloat = New DevExpress.Utils.PointFloat(392.0!, 5.0!)
        Me.XrLabelCurveType.Name = "XrLabelCurveType"
        Me.XrLabelCurveType.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCurveType.SizeF = New System.Drawing.SizeF(130.0!, 19.99997!)
        Me.XrLabelCurveType.StylePriority.UseBorders = False
        Me.XrLabelCurveType.StylePriority.UseFont = False
        Me.XrLabelCurveType.Text = "Curve Type"
        '
        'XrLabelCurveGrowth
        '
        Me.XrLabelCurveGrowth.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCurveGrowth.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCurveGrowth.LocationFloat = New DevExpress.Utils.PointFloat(244.7917!, 5.0!)
        Me.XrLabelCurveGrowth.Name = "XrLabelCurveGrowth"
        Me.XrLabelCurveGrowth.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCurveGrowth.SizeF = New System.Drawing.SizeF(133.3334!, 19.99997!)
        Me.XrLabelCurveGrowth.StylePriority.UseBorders = False
        Me.XrLabelCurveGrowth.StylePriority.UseFont = False
        Me.XrLabelCurveGrowth.Text = "Curve Growth"
        '
        'XrLabelYAxis
        '
        Me.XrLabelYAxis.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelYAxis.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelYAxis.LocationFloat = New DevExpress.Utils.PointFloat(128.125!, 5.0!)
        Me.XrLabelYAxis.Name = "XrLabelYAxis"
        Me.XrLabelYAxis.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelYAxis.SizeF = New System.Drawing.SizeF(100.0!, 19.99997!)
        Me.XrLabelYAxis.StylePriority.UseBorders = False
        Me.XrLabelYAxis.StylePriority.UseFont = False
        Me.XrLabelYAxis.Text = "Y Axis"
        '
        'XrLabelXAxis
        '
        Me.XrLabelXAxis.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelXAxis.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelXAxis.LocationFloat = New DevExpress.Utils.PointFloat(10.00001!, 5.0!)
        Me.XrLabelXAxis.Name = "XrLabelXAxis"
        Me.XrLabelXAxis.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelXAxis.SizeF = New System.Drawing.SizeF(100.0!, 19.99997!)
        Me.XrLabelXAxis.StylePriority.UseBorders = False
        Me.XrLabelXAxis.StylePriority.UseFont = False
        Me.XrLabelXAxis.Text = "X Axis"
        '
        'XrPanel1
        '
        Me.XrPanel1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel1.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel1.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelTestName})
        Me.XrPanel1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrPanel1.Name = "XrPanel1"
        Me.XrPanel1.SizeF = New System.Drawing.SizeF(650.0!, 30.0!)
        Me.XrPanel1.StylePriority.UseBackColor = False
        Me.XrPanel1.StylePriority.UseBorderColor = False
        Me.XrPanel1.StylePriority.UseBorders = False
        '
        'XrLabelTestName
        '
        Me.XrLabelTestName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelTestName.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelTestName.LocationFloat = New DevExpress.Utils.PointFloat(10.0!, 6.0!)
        Me.XrLabelTestName.Name = "XrLabelTestName"
        Me.XrLabelTestName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelTestName.SizeF = New System.Drawing.SizeF(630.0!, 20.0!)
        Me.XrLabelTestName.StylePriority.UseBorders = False
        Me.XrLabelTestName.StylePriority.UseFont = False
        Me.XrLabelTestName.Text = "XrLabelTestName"
        '
        'XrPanel2
        '
        Me.XrPanel2.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel2.BorderWidth = 2
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelCalcConc, Me.XrLabelError, Me.XrLabelTheorConc, Me.XrLabelAbs, Me.XrLabelCalibNo})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(10.00001!, 530.0!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(630.0!, 23.12501!)
        Me.XrPanel2.StylePriority.UseBorders = False
        Me.XrPanel2.StylePriority.UseBorderWidth = False
        '
        'XrLabelCalcConc
        '
        Me.XrLabelCalcConc.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalcConc.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCalcConc.LocationFloat = New DevExpress.Utils.PointFloat(382.0!, 0.0!)
        Me.XrLabelCalcConc.Name = "XrLabelCalcConc"
        Me.XrLabelCalcConc.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCalcConc.SizeF = New System.Drawing.SizeF(130.0!, 20.0!)
        Me.XrLabelCalcConc.StylePriority.UseBorders = False
        Me.XrLabelCalcConc.StylePriority.UseFont = False
        Me.XrLabelCalcConc.StylePriority.UseTextAlignment = False
        Me.XrLabelCalcConc.Text = "Calc. Conc."
        Me.XrLabelCalcConc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelError
        '
        Me.XrLabelError.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelError.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelError.LocationFloat = New DevExpress.Utils.PointFloat(512.0!, 0.0!)
        Me.XrLabelError.Name = "XrLabelError"
        Me.XrLabelError.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelError.SizeF = New System.Drawing.SizeF(118.0!, 20.00002!)
        Me.XrLabelError.StylePriority.UseBorders = False
        Me.XrLabelError.StylePriority.UseFont = False
        Me.XrLabelError.StylePriority.UseTextAlignment = False
        Me.XrLabelError.Text = "% Error"
        Me.XrLabelError.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelTheorConc
        '
        Me.XrLabelTheorConc.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelTheorConc.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelTheorConc.LocationFloat = New DevExpress.Utils.PointFloat(252.0!, 0.0!)
        Me.XrLabelTheorConc.Name = "XrLabelTheorConc"
        Me.XrLabelTheorConc.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelTheorConc.SizeF = New System.Drawing.SizeF(130.0!, 20.0!)
        Me.XrLabelTheorConc.StylePriority.UseBorders = False
        Me.XrLabelTheorConc.StylePriority.UseFont = False
        Me.XrLabelTheorConc.StylePriority.UseTextAlignment = False
        Me.XrLabelTheorConc.Text = "Theor. Conc."
        Me.XrLabelTheorConc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelAbs
        '
        Me.XrLabelAbs.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelAbs.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelAbs.LocationFloat = New DevExpress.Utils.PointFloat(142.0!, 0.0!)
        Me.XrLabelAbs.Name = "XrLabelAbs"
        Me.XrLabelAbs.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelAbs.SizeF = New System.Drawing.SizeF(110.0!, 20.0!)
        Me.XrLabelAbs.StylePriority.UseBorders = False
        Me.XrLabelAbs.StylePriority.UseFont = False
        Me.XrLabelAbs.StylePriority.UseTextAlignment = False
        Me.XrLabelAbs.Text = "Absorbance"
        Me.XrLabelAbs.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelCalibNo
        '
        Me.XrLabelCalibNo.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalibNo.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCalibNo.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.00001144409!)
        Me.XrLabelCalibNo.Name = "XrLabelCalibNo"
        Me.XrLabelCalibNo.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCalibNo.SizeF = New System.Drawing.SizeF(142.0!, 20.00001!)
        Me.XrLabelCalibNo.StylePriority.UseBorders = False
        Me.XrLabelCalibNo.StylePriority.UseFont = False
        Me.XrLabelCalibNo.StylePriority.UseTextAlignment = False
        Me.XrLabelCalibNo.Text = "Calibrator Number"
        Me.XrLabelCalibNo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
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
        'XrHeaderLabel
        '
        Me.XrHeaderLabel.BackColor = System.Drawing.Color.LightGray
        Me.XrHeaderLabel.BorderColor = System.Drawing.Color.DarkGray
        Me.XrHeaderLabel.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrHeaderLabel.Font = New System.Drawing.Font("Verdana", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrHeaderLabel.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 6.0!)
        Me.XrHeaderLabel.Name = "XrHeaderLabel"
        Me.XrHeaderLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(10, 2, 3, 3, 100.0!)
        Me.XrHeaderLabel.SizeF = New System.Drawing.SizeF(650.0!, 25.0!)
        Me.XrHeaderLabel.StylePriority.UseBackColor = False
        Me.XrHeaderLabel.StylePriority.UseBorderColor = False
        Me.XrHeaderLabel.StylePriority.UseBorders = False
        Me.XrHeaderLabel.StylePriority.UseFont = False
        Me.XrHeaderLabel.StylePriority.UsePadding = False
        Me.XrHeaderLabel.StylePriority.UseTextAlignment = False
        Me.XrHeaderLabel.Text = "XrHeaderLabel"
        Me.XrHeaderLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'GroupHeader1
        '
        Me.GroupHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrWSStartDateTimeLabel, Me.XrHeaderLabel})
        Me.GroupHeader1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupHeader1.HeightF = 73.0!
        Me.GroupHeader1.Name = "GroupHeader1"
        Me.GroupHeader1.RepeatEveryPage = True
        Me.GroupHeader1.StylePriority.UseFont = False
        '
        'XrWSStartDateTimeLabel
        '
        Me.XrWSStartDateTimeLabel.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrWSStartDateTimeLabel.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrWSStartDateTimeLabel.LocationFloat = New DevExpress.Utils.PointFloat(10.0!, 35.0!)
        Me.XrWSStartDateTimeLabel.Name = "XrWSStartDateTimeLabel"
        Me.XrWSStartDateTimeLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrWSStartDateTimeLabel.SizeF = New System.Drawing.SizeF(630.0!, 20.0!)
        Me.XrWSStartDateTimeLabel.StylePriority.UseBorders = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseFont = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseTextAlignment = False
        Me.XrWSStartDateTimeLabel.Text = "WSStartDateTime"
        Me.XrWSStartDateTimeLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'DetailReport
        '
        Me.DetailReport.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail1})
        Me.DetailReport.DataMember = "ReportCalibCurve"
        Me.DetailReport.DataSource = Me.ResultsDS1
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
        Me.DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand
        '
        'Detail1
        '
        Me.Detail1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTableDetails})
        Me.Detail1.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail1.HeightF = 24.0!
        Me.Detail1.Name = "Detail1"
        Me.Detail1.StylePriority.UseFont = False
        '
        'XrTableDetails
        '
        Me.XrTableDetails.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableDetails.LocationFloat = New DevExpress.Utils.PointFloat(10.0!, 0.0!)
        Me.XrTableDetails.Name = "XrTableDetails"
        Me.XrTableDetails.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableDetailsRow})
        Me.XrTableDetails.SizeF = New System.Drawing.SizeF(630.0!, 18.0!)
        Me.XrTableDetails.StylePriority.UseFont = False
        '
        'XrTableDetailsRow
        '
        Me.XrTableDetailsRow.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCellClass, Me.XrTableCell2, Me.XrTableCell5, Me.XrTableCell6, Me.XrTableCell8})
        Me.XrTableDetailsRow.Name = "XrTableDetailsRow"
        Me.XrTableDetailsRow.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 3, 2, 0, 100.0!)
        Me.XrTableDetailsRow.StylePriority.UsePadding = False
        Me.XrTableDetailsRow.Weight = 1
        '
        'XrTableCellClass
        '
        Me.XrTableCellClass.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportCalibCurve.MultiPointNumber")})
        Me.XrTableCellClass.Name = "XrTableCellClass"
        Me.XrTableCellClass.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 5, 2, 0, 100.0!)
        Me.XrTableCellClass.StylePriority.UsePadding = False
        Me.XrTableCellClass.StylePriority.UseTextAlignment = False
        Me.XrTableCellClass.Text = "XrTableCellClass"
        Me.XrTableCellClass.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCellClass.Weight = 0.67619069417317723
        '
        'XrTableCell2
        '
        Me.XrTableCell2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportCalibCurve.ABSValue")})
        Me.XrTableCell2.Name = "XrTableCell2"
        Me.XrTableCell2.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 5, 2, 0, 100.0!)
        Me.XrTableCell2.StylePriority.UsePadding = False
        Me.XrTableCell2.StylePriority.UseTextAlignment = False
        Me.XrTableCell2.Text = "XrTableCell2"
        Me.XrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell2.Weight = 0.52380930582682272
        '
        'XrTableCell5
        '
        Me.XrTableCell5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportCalibCurve.TheoricalConcentration")})
        Me.XrTableCell5.Name = "XrTableCell5"
        Me.XrTableCell5.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 5, 2, 0, 100.0!)
        Me.XrTableCell5.StylePriority.UsePadding = False
        Me.XrTableCell5.StylePriority.UseTextAlignment = False
        Me.XrTableCell5.Text = "XrTableCell5"
        Me.XrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell5.Weight = 0.6190474828084308
        '
        'XrTableCell6
        '
        Me.XrTableCell6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportCalibCurve.CONC_Value")})
        Me.XrTableCell6.Name = "XrTableCell6"
        Me.XrTableCell6.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 5, 2, 0, 100.0!)
        Me.XrTableCell6.StylePriority.UsePadding = False
        Me.XrTableCell6.StylePriority.UseTextAlignment = False
        Me.XrTableCell6.Text = "XrTableCell6"
        Me.XrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell6.Weight = 0.61904790514991392
        '
        'XrTableCell8
        '
        Me.XrTableCell8.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportCalibCurve.RelativeErrorCurve")})
        Me.XrTableCell8.Name = "XrTableCell8"
        Me.XrTableCell8.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 5, 2, 0, 100.0!)
        Me.XrTableCell8.StylePriority.UsePadding = False
        Me.XrTableCell8.StylePriority.UseTextAlignment = False
        Me.XrTableCell8.Text = "XrTableCell8"
        Me.XrTableCell8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell8.Weight = 0.561904612041655
        '
        'ResultsDS1
        '
        Me.ResultsDS1.DataSetName = "ResultsDS"
        Me.ResultsDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ResultsCalibCurveReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport})
        Me.DataMember = "ReportTestMaster"
        Me.DataSource = Me.ResultsDS1
        Me.Margins = New System.Drawing.Printing.Margins(100, 100, 96, 100)
        Me.StyleSheet.AddRange(New DevExpress.XtraReports.UI.XRControlStyle() {Me.XrControlStyle1})
        Me.Version = "10.2"
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(LineSeriesView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Series2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(LineSeriesView2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrChart1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTableDetails, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ResultsDS1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents XrControlStyle1 As DevExpress.XtraReports.UI.XRControlStyle
    Protected WithEvents XrHeaderLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents DetailReport As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents Detail1 As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents XrPanel2 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelError As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCalibNo As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelTheorConc As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelAbs As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel1 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents ResultsDS1 As Biosystems.Ax00.Types.ResultsDS
    Friend WithEvents XrLabelTestName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTableDetails As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableDetailsRow As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCellClass As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell2 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell5 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell6 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell8 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrWSStartDateTimeLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCalcConc As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel3 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelYAxis As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelXAxis As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCurveGrowth As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCurveType As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelXAxisField As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCurveGrowthField As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelYAxisField As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCurveTypeField As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelUnitField As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrChart1 As DevExpress.XtraReports.UI.XRChart
    Friend WithEvents XrLabelNoData As DevExpress.XtraReports.UI.XRLabel
End Class
