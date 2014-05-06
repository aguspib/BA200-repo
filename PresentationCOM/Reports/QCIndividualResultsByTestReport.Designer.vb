<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class QCIndividualResultsByTestReport
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(QCIndividualResultsByTestReport))
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
        Me.XrPanel1 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrLabelCalcultationMode = New DevExpress.XtraReports.UI.XRLabel
        Me.XrDateRange = New DevExpress.XtraReports.UI.XRLabel
        Me.XrSample = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelDateRange = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelSample = New DevExpress.XtraReports.UI.XRLabel
        Me.XrTestName = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelTestName = New DevExpress.XtraReports.UI.XRLabel
        Me.XrMode = New DevExpress.XtraReports.UI.XRLabel
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand
        Me.XrHeaderLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand
        Me.Detail1 = New DevExpress.XtraReports.UI.DetailBand
        Me.XrLabelControls = New DevExpress.XtraReports.UI.XRLabel
        Me.GroupFooter1 = New DevExpress.XtraReports.UI.GroupFooterBand
        Me.XrPnlLegend = New DevExpress.XtraReports.UI.XRPanel
        Me.XrPicLast = New DevExpress.XtraReports.UI.XRPictureBox
        Me.XrLabelLast = New DevExpress.XtraReports.UI.XRLabel
        Me.XrPanel3 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrGraphHeaderLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.XrYoudenGraph = New DevExpress.XtraReports.UI.XRChart
        CType(Me.XrYoudenGraph, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(LineSeriesView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(LineSeriesView2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel1})
        Me.Detail.HeightF = 78.00005!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrPanel1
        '
        Me.XrPanel1.BackColor = System.Drawing.Color.White
        Me.XrPanel1.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel1.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel1.BorderWidth = 2
        Me.XrPanel1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelCalcultationMode, Me.XrDateRange, Me.XrSample, Me.XrLabelDateRange, Me.XrLabelSample, Me.XrTestName, Me.XrLabelTestName, Me.XrMode})
        Me.XrPanel1.LocationFloat = New DevExpress.Utils.PointFloat(0.0001907349!, 10.00001!)
        Me.XrPanel1.Name = "XrPanel1"
        Me.XrPanel1.SizeF = New System.Drawing.SizeF(649.9998!, 49.79167!)
        Me.XrPanel1.StylePriority.UseBackColor = False
        Me.XrPanel1.StylePriority.UseBorderColor = False
        Me.XrPanel1.StylePriority.UseBorders = False
        Me.XrPanel1.StylePriority.UseBorderWidth = False
        '
        'XrLabelCalcultationMode
        '
        Me.XrLabelCalcultationMode.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalcultationMode.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCalcultationMode.LocationFloat = New DevExpress.Utils.PointFloat(313.6663!, 4.000025!)
        Me.XrLabelCalcultationMode.Name = "XrLabelCalcultationMode"
        Me.XrLabelCalcultationMode.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCalcultationMode.SizeF = New System.Drawing.SizeF(138.8904!, 19.99998!)
        Me.XrLabelCalcultationMode.StylePriority.UseBorders = False
        Me.XrLabelCalcultationMode.StylePriority.UseFont = False
        Me.XrLabelCalcultationMode.StylePriority.UseTextAlignment = False
        Me.XrLabelCalcultationMode.Text = "Mode"
        Me.XrLabelCalcultationMode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrDateRange
        '
        Me.XrDateRange.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrDateRange.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrDateRange.LocationFloat = New DevExpress.Utils.PointFloat(462.0564!, 25.00006!)
        Me.XrDateRange.Name = "XrDateRange"
        Me.XrDateRange.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrDateRange.SizeF = New System.Drawing.SizeF(183.9433!, 20.00002!)
        Me.XrDateRange.StylePriority.UseBorders = False
        Me.XrDateRange.StylePriority.UseFont = False
        Me.XrDateRange.StylePriority.UseTextAlignment = False
        Me.XrDateRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrSample
        '
        Me.XrSample.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrSample.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrSample.LocationFloat = New DevExpress.Utils.PointFloat(194.2079!, 25.00006!)
        Me.XrSample.Name = "XrSample"
        Me.XrSample.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrSample.SizeF = New System.Drawing.SizeF(111.4584!, 20.00001!)
        Me.XrSample.StylePriority.UseBorders = False
        Me.XrSample.StylePriority.UseFont = False
        '
        'XrLabelDateRange
        '
        Me.XrLabelDateRange.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelDateRange.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelDateRange.LocationFloat = New DevExpress.Utils.PointFloat(462.0564!, 4.000025!)
        Me.XrLabelDateRange.Name = "XrLabelDateRange"
        Me.XrLabelDateRange.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelDateRange.SizeF = New System.Drawing.SizeF(183.9433!, 19.99998!)
        Me.XrLabelDateRange.StylePriority.UseBorders = False
        Me.XrLabelDateRange.StylePriority.UseFont = False
        Me.XrLabelDateRange.StylePriority.UseTextAlignment = False
        Me.XrLabelDateRange.Text = "Date range"
        Me.XrLabelDateRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelSample
        '
        Me.XrLabelSample.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelSample.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelSample.LocationFloat = New DevExpress.Utils.PointFloat(194.2079!, 4.000028!)
        Me.XrLabelSample.Name = "XrLabelSample"
        Me.XrLabelSample.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelSample.SizeF = New System.Drawing.SizeF(111.4584!, 20.0!)
        Me.XrLabelSample.StylePriority.UseBorders = False
        Me.XrLabelSample.StylePriority.UseFont = False
        Me.XrLabelSample.Text = "Sample Type"
        '
        'XrTestName
        '
        Me.XrTestName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrTestName.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrTestName.LocationFloat = New DevExpress.Utils.PointFloat(10.00004!, 25.00003!)
        Me.XrTestName.Name = "XrTestName"
        Me.XrTestName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTestName.SizeF = New System.Drawing.SizeF(179.1667!, 20.00001!)
        Me.XrTestName.StylePriority.UseBorders = False
        Me.XrTestName.StylePriority.UseFont = False
        '
        'XrLabelTestName
        '
        Me.XrLabelTestName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelTestName.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelTestName.LocationFloat = New DevExpress.Utils.PointFloat(10.00004!, 4.000025!)
        Me.XrLabelTestName.Name = "XrLabelTestName"
        Me.XrLabelTestName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelTestName.SizeF = New System.Drawing.SizeF(179.1666!, 20.0!)
        Me.XrLabelTestName.StylePriority.UseBorders = False
        Me.XrLabelTestName.StylePriority.UseFont = False
        Me.XrLabelTestName.Text = "Test"
        '
        'XrMode
        '
        Me.XrMode.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrMode.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrMode.LocationFloat = New DevExpress.Utils.PointFloat(313.6663!, 25.00006!)
        Me.XrMode.Name = "XrMode"
        Me.XrMode.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrMode.SizeF = New System.Drawing.SizeF(138.8905!, 20.00002!)
        Me.XrMode.StylePriority.UseBorders = False
        Me.XrMode.StylePriority.UseFont = False
        '
        'TopMargin
        '
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
        'XrHeaderLabel
        '
        Me.XrHeaderLabel.BackColor = System.Drawing.Color.LightGray
        Me.XrHeaderLabel.BorderColor = System.Drawing.Color.DarkGray
        Me.XrHeaderLabel.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrHeaderLabel.Font = New System.Drawing.Font("Verdana", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrHeaderLabel.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 10.00001!)
        Me.XrHeaderLabel.Name = "XrHeaderLabel"
        Me.XrHeaderLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(10, 2, 3, 3, 100.0!)
        Me.XrHeaderLabel.SizeF = New System.Drawing.SizeF(650.0!, 25.0!)
        Me.XrHeaderLabel.StylePriority.UseBackColor = False
        Me.XrHeaderLabel.StylePriority.UseBorderColor = False
        Me.XrHeaderLabel.StylePriority.UseBorders = False
        Me.XrHeaderLabel.StylePriority.UseFont = False
        Me.XrHeaderLabel.StylePriority.UsePadding = False
        Me.XrHeaderLabel.StylePriority.UseTextAlignment = False
        Me.XrHeaderLabel.Text = "Individual QC Results by Test / Sample type"
        Me.XrHeaderLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'GroupHeader1
        '
        Me.GroupHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrHeaderLabel})
        Me.GroupHeader1.HeightF = 51.04167!
        Me.GroupHeader1.Name = "GroupHeader1"
        '
        'DetailReport
        '
        Me.DetailReport.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail1})
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
        '
        'Detail1
        '
        Me.Detail1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelControls})
        Me.Detail1.HeightF = 46.54166!
        Me.Detail1.Name = "Detail1"
        '
        'XrLabelControls
        '
        Me.XrLabelControls.BackColor = System.Drawing.Color.Gainsboro
        Me.XrLabelControls.BorderColor = System.Drawing.Color.DarkGray
        Me.XrLabelControls.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrLabelControls.Font = New System.Drawing.Font("Verdana", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelControls.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrLabelControls.Name = "XrLabelControls"
        Me.XrLabelControls.Padding = New DevExpress.XtraPrinting.PaddingInfo(10, 2, 3, 3, 100.0!)
        Me.XrLabelControls.SizeF = New System.Drawing.SizeF(650.0!, 25.0!)
        Me.XrLabelControls.StylePriority.UseBackColor = False
        Me.XrLabelControls.StylePriority.UseBorderColor = False
        Me.XrLabelControls.StylePriority.UseBorders = False
        Me.XrLabelControls.StylePriority.UseFont = False
        Me.XrLabelControls.StylePriority.UsePadding = False
        Me.XrLabelControls.StylePriority.UseTextAlignment = False
        Me.XrLabelControls.Text = "Controls"
        Me.XrLabelControls.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'GroupFooter1
        '
        Me.GroupFooter1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPnlLegend, Me.XrPanel3, Me.XrYoudenGraph})
        Me.GroupFooter1.HeightF = 458.0833!
        Me.GroupFooter1.Name = "GroupFooter1"
        '
        'XrPnlLegend
        '
        Me.XrPnlLegend.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPnlLegend.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPnlLegend.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPnlLegend.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPicLast, Me.XrLabelLast})
        Me.XrPnlLegend.LocationFloat = New DevExpress.Utils.PointFloat(10.00023!, 426.8333!)
        Me.XrPnlLegend.Name = "XrPnlLegend"
        Me.XrPnlLegend.SizeF = New System.Drawing.SizeF(627.4994!, 31.25!)
        Me.XrPnlLegend.StylePriority.UseBackColor = False
        Me.XrPnlLegend.StylePriority.UseBorderColor = False
        Me.XrPnlLegend.StylePriority.UseBorders = False
        '
        'XrPicLast
        '
        Me.XrPicLast.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrPicLast.Image = CType(resources.GetObject("XrPicLast.Image"), System.Drawing.Image)
        Me.XrPicLast.LocationFloat = New DevExpress.Utils.PointFloat(240.0!, 5.25!)
        Me.XrPicLast.Name = "XrPicLast"
        Me.XrPicLast.SizeF = New System.Drawing.SizeF(16.0!, 16.0!)
        Me.XrPicLast.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage
        Me.XrPicLast.StylePriority.UseBorders = False
        '
        'XrLabelLast
        '
        Me.XrLabelLast.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelLast.Font = New System.Drawing.Font("Verdana", 9.0!)
        Me.XrLabelLast.LocationFloat = New DevExpress.Utils.PointFloat(260.0!, 5.249969!)
        Me.XrLabelLast.Name = "XrLabelLast"
        Me.XrLabelLast.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelLast.SizeF = New System.Drawing.SizeF(192.9733!, 19.99997!)
        Me.XrLabelLast.StylePriority.UseBorders = False
        Me.XrLabelLast.StylePriority.UseFont = False
        Me.XrLabelLast.Text = "XrLabelLast"
        '
        'XrPanel3
        '
        Me.XrPanel3.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel3.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel3.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel3.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrGraphHeaderLabel})
        Me.XrPanel3.LocationFloat = New DevExpress.Utils.PointFloat(9.999974!, 5.0!)
        Me.XrPanel3.Name = "XrPanel3"
        Me.XrPanel3.SizeF = New System.Drawing.SizeF(627.4996!, 32.08329!)
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
        Me.XrGraphHeaderLabel.Text = "XrGraphHeaderLabel"
        '
        'XrYoudenGraph
        '
        Me.XrYoudenGraph.AppearanceName = "Light"
        Me.XrYoudenGraph.BorderColor = System.Drawing.Color.DarkGray
        Me.XrYoudenGraph.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        XyDiagram1.AxisX.GridLines.Visible = True
        XyDiagram1.AxisX.Range.SideMarginsEnabled = True
        XyDiagram1.AxisX.VisibleInPanesSerializable = "-1"
        XyDiagram1.AxisY.Range.SideMarginsEnabled = True
        XyDiagram1.AxisY.VisibleInPanesSerializable = "-1"
        Me.XrYoudenGraph.Diagram = XyDiagram1
        Me.XrYoudenGraph.FillStyle.FillMode = DevExpress.XtraCharts.FillMode.Gradient
        RectangleGradientFillOptions1.Color2 = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        RectangleGradientFillOptions1.GradientMode = DevExpress.XtraCharts.RectangleGradientMode.TopLeftToBottomRight
        Me.XrYoudenGraph.FillStyle.Options = RectangleGradientFillOptions1
        Me.XrYoudenGraph.IndicatorsPaletteName = "Default"
        Me.XrYoudenGraph.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.Bottom
        Me.XrYoudenGraph.Legend.Direction = DevExpress.XtraCharts.LegendDirection.LeftToRight
        Me.XrYoudenGraph.Legend.EquallySpacedItems = False
        Me.XrYoudenGraph.LocationFloat = New DevExpress.Utils.PointFloat(9.999974!, 42.04162!)
        Me.XrYoudenGraph.Name = "XrYoudenGraph"
        Me.XrYoudenGraph.PaletteBaseColorNumber = 4
        Me.XrYoudenGraph.PaletteName = "Black and White"
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
        Me.XrYoudenGraph.SeriesSerializable = New DevExpress.XtraCharts.Series() {Series1}
        PointSeriesLabel2.LineVisible = True
        Me.XrYoudenGraph.SeriesTemplate.Label = PointSeriesLabel2
        Me.XrYoudenGraph.SeriesTemplate.LegendPointOptions = PointOptions3
        Me.XrYoudenGraph.SeriesTemplate.SynchronizePointOptions = False
        Me.XrYoudenGraph.SeriesTemplate.View = LineSeriesView2
        Me.XrYoudenGraph.SizeF = New System.Drawing.SizeF(627.4996!, 384.7917!)
        Me.XrYoudenGraph.StylePriority.UseBackColor = False
        Me.XrYoudenGraph.StylePriority.UseBorderColor = False
        Me.XrYoudenGraph.StylePriority.UseBorders = False
        ChartTitle1.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ChartTitle1.Text = "Linear"
        ChartTitle1.Visible = False
        Me.XrYoudenGraph.Titles.AddRange(New DevExpress.XtraCharts.ChartTitle() {ChartTitle1})
        '
        'QCIndividualResultsByTestReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport, Me.GroupFooter1})
        Me.ScriptsSource = "" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.Version = "10.2"
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(LineSeriesView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(LineSeriesView2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrYoudenGraph, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Protected WithEvents XrHeaderLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel1 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrDateRange As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrSample As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelDateRange As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelSample As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTestName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelTestName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents DetailReport As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents Detail1 As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents GroupFooter1 As DevExpress.XtraReports.UI.GroupFooterBand
    Friend WithEvents XrLabelCalcultationMode As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrMode As DevExpress.XtraReports.UI.XRLabel
    Protected WithEvents XrLabelControls As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrYoudenGraph As DevExpress.XtraReports.UI.XRChart
    Friend WithEvents XrPanel3 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrGraphHeaderLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPnlLegend As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrPicLast As DevExpress.XtraReports.UI.XRPictureBox
    Friend WithEvents XrLabelLast As DevExpress.XtraReports.UI.XRLabel
End Class
