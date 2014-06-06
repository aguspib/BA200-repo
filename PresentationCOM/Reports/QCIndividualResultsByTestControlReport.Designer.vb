<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class QCIndividualResultsByTestControlReport
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(QCIndividualResultsByTestControlReport))
        Dim XyDiagram1 As DevExpress.XtraCharts.XYDiagram = New DevExpress.XtraCharts.XYDiagram()
        Dim RectangleGradientFillOptions1 As DevExpress.XtraCharts.RectangleGradientFillOptions = New DevExpress.XtraCharts.RectangleGradientFillOptions()
        Dim Series1 As DevExpress.XtraCharts.Series = New DevExpress.XtraCharts.Series()
        Dim PointSeriesLabel1 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel()
        Dim PointOptions1 As DevExpress.XtraCharts.PointOptions = New DevExpress.XtraCharts.PointOptions()
        Dim PointOptions2 As DevExpress.XtraCharts.PointOptions = New DevExpress.XtraCharts.PointOptions()
        Dim LineSeriesView1 As DevExpress.XtraCharts.LineSeriesView = New DevExpress.XtraCharts.LineSeriesView()
        Dim PointSeriesLabel2 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel()
        Dim PointOptions3 As DevExpress.XtraCharts.PointOptions = New DevExpress.XtraCharts.PointOptions()
        Dim LineSeriesView2 As DevExpress.XtraCharts.LineSeriesView = New DevExpress.XtraCharts.LineSeriesView()
        Dim ChartTitle1 As DevExpress.XtraCharts.ChartTitle = New DevExpress.XtraCharts.ChartTitle()
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand()
        Me.XrTableReport = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableReportRow = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrCellIncludedInMean = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrCellCalcRunNumber = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrCellResultDateTime = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrCellVisibleResultValue = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrCellMeasureUnit = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrCellABSError = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrCellRELErrorPercent = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrCellAlarmsList = New DevExpress.XtraReports.UI.XRTableCell()
        Me.QcResultsDS1 = New Biosystems.Ax00.Types.QCResultsDS()
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.ReportHeader = New DevExpress.XtraReports.UI.ReportHeaderBand()
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrLabelABSError = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelMeasureUnit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelVisibleResultValue = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelAlarmsList = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelRELErrorPercent = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelResultDateTime = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelCalcRunNumber = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrPanel4 = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrRanges = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelRanges = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelCV = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrCV = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrSD = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelSD = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrUnit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelUnit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrMean = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLotNumber = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelMean = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelLotNumber = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrControlName = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelControlName = New DevExpress.XtraReports.UI.XRLabel()
        Me.GroupFooter1 = New DevExpress.XtraReports.UI.GroupFooterBand()
        Me.XrPnlLegend = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrPicWarning = New DevExpress.XtraReports.UI.XRPictureBox()
        Me.XrLabelWarning = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelError = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrPicError = New DevExpress.XtraReports.UI.XRPictureBox()
        Me.XrLJGraph = New DevExpress.XtraReports.UI.XRChart()
        Me.XrPanel3 = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrGraphHeaderLabel = New DevExpress.XtraReports.UI.XRLabel()
        Me.ControlLotID = New DevExpress.XtraReports.Parameters.Parameter()
        Me.QcIndividualResultsByTestReport1 = New Biosystems.Ax00.PresentationCOM.QCIndividualResultsByTestReport()
        Me.QcIndividualResultsByTestReport2 = New Biosystems.Ax00.PresentationCOM.QCIndividualResultsByTestReport()
        Me.QcIndividualResultsByTestReport3 = New Biosystems.Ax00.PresentationCOM.QCIndividualResultsByTestReport()
        Me.QcIndividualResultsByTestReport4 = New Biosystems.Ax00.PresentationCOM.QCIndividualResultsByTestReport()
        Me.QcIndividualResultsByTestReport5 = New Biosystems.Ax00.PresentationCOM.QCIndividualResultsByTestReport()
        CType(Me.XrTableReport, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.QcResultsDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrLJGraph, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(LineSeriesView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(LineSeriesView2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.QcIndividualResultsByTestReport1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.QcIndividualResultsByTestReport2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.QcIndividualResultsByTestReport3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.QcIndividualResultsByTestReport4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.QcIndividualResultsByTestReport5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTableReport})
        Me.Detail.HeightF = 22.91667!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrTableReport
        '
        Me.XrTableReport.LocationFloat = New DevExpress.Utils.PointFloat(10.00023!, 0.0!)
        Me.XrTableReport.Name = "XrTableReport"
        Me.XrTableReport.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableReportRow})
        Me.XrTableReport.SizeF = New System.Drawing.SizeF(680.9998!, 22.0!)
        '
        'XrTableReportRow
        '
        Me.XrTableReportRow.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrCellIncludedInMean, Me.XrCellCalcRunNumber, Me.XrCellResultDateTime, Me.XrCellVisibleResultValue, Me.XrCellMeasureUnit, Me.XrCellABSError, Me.XrCellRELErrorPercent, Me.XrCellAlarmsList})
        Me.XrTableReportRow.Name = "XrTableReportRow"
        Me.XrTableReportRow.Weight = 0.88R
        '
        'XrCellIncludedInMean
        '
        Me.XrCellIncludedInMean.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcResults.IncludedInMean")})
        Me.XrCellIncludedInMean.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellIncludedInMean.Name = "XrCellIncludedInMean"
        Me.XrCellIncludedInMean.StylePriority.UseFont = False
        Me.XrCellIncludedInMean.Text = "XrCellIncludedInMean"
        Me.XrCellIncludedInMean.Weight = 0.10476190476190478R
        '
        'XrCellCalcRunNumber
        '
        Me.XrCellCalcRunNumber.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcResults.CalcRunNumber")})
        Me.XrCellCalcRunNumber.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellCalcRunNumber.Name = "XrCellCalcRunNumber"
        Me.XrCellCalcRunNumber.StylePriority.UseFont = False
        Me.XrCellCalcRunNumber.StylePriority.UseTextAlignment = False
        Me.XrCellCalcRunNumber.Text = "XrCellCalcRunNumber"
        Me.XrCellCalcRunNumber.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        Me.XrCellCalcRunNumber.Weight = 0.15238095238095242R
        '
        'XrCellResultDateTime
        '
        Me.XrCellResultDateTime.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcResults.ResultDateTime")})
        Me.XrCellResultDateTime.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellResultDateTime.Name = "XrCellResultDateTime"
        Me.XrCellResultDateTime.StylePriority.UseFont = False
        Me.XrCellResultDateTime.Text = "XrCellGetCurrentColumnValue(""ResultDate"")"
        Me.XrCellResultDateTime.Weight = 0.61822902179864236R
        '
        'XrCellVisibleResultValue
        '
        Me.XrCellVisibleResultValue.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcResults.VisibleResultValue")})
        Me.XrCellVisibleResultValue.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellVisibleResultValue.Name = "XrCellVisibleResultValue"
        Me.XrCellVisibleResultValue.StylePriority.UseFont = False
        Me.XrCellVisibleResultValue.StylePriority.UseTextAlignment = False
        Me.XrCellVisibleResultValue.Text = "XrCellVisibleResultValue"
        Me.XrCellVisibleResultValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrCellVisibleResultValue.Weight = 0.37224724133844833R
        '
        'XrCellMeasureUnit
        '
        Me.XrCellMeasureUnit.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcResults.MeasureUnit")})
        Me.XrCellMeasureUnit.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellMeasureUnit.Name = "XrCellMeasureUnit"
        Me.XrCellMeasureUnit.StylePriority.UseFont = False
        Me.XrCellMeasureUnit.StylePriority.UseTextAlignment = False
        Me.XrCellMeasureUnit.Text = "XrCellMeasureUnit"
        Me.XrCellMeasureUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        Me.XrCellMeasureUnit.Weight = 0.38129582315253657R
        '
        'XrCellABSError
        '
        Me.XrCellABSError.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcResults.ABSError")})
        Me.XrCellABSError.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellABSError.Name = "XrCellABSError"
        Me.XrCellABSError.StylePriority.UseFont = False
        Me.XrCellABSError.StylePriority.UseTextAlignment = False
        Me.XrCellABSError.Text = "XrCellABSError"
        Me.XrCellABSError.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        Me.XrCellABSError.Weight = 0.34600400001862325R
        '
        'XrCellRELErrorPercent
        '
        Me.XrCellRELErrorPercent.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcResults.RELErrorPercent")})
        Me.XrCellRELErrorPercent.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellRELErrorPercent.Name = "XrCellRELErrorPercent"
        Me.XrCellRELErrorPercent.StylePriority.UseFont = False
        Me.XrCellRELErrorPercent.StylePriority.UseTextAlignment = False
        Me.XrCellRELErrorPercent.Text = "XrCellRELErrorPercent"
        Me.XrCellRELErrorPercent.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        Me.XrCellRELErrorPercent.Weight = 0.39464055302812462R
        '
        'XrCellAlarmsList
        '
        Me.XrCellAlarmsList.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tqcResults.AlarmsList")})
        Me.XrCellAlarmsList.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrCellAlarmsList.Name = "XrCellAlarmsList"
        Me.XrCellAlarmsList.Padding = New DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0, 100.0!)
        Me.XrCellAlarmsList.StylePriority.UseFont = False
        Me.XrCellAlarmsList.StylePriority.UsePadding = False
        Me.XrCellAlarmsList.StylePriority.UseTextAlignment = False
        Me.XrCellAlarmsList.Text = "XrCellAlarmsList"
        Me.XrCellAlarmsList.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrCellAlarmsList.Weight = 0.63044050352076741R
        '
        'QcResultsDS1
        '
        Me.QcResultsDS1.DataSetName = "QCResultsDS"
        Me.QcResultsDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'TopMargin
        '
        Me.TopMargin.HeightF = 38.0!
        Me.TopMargin.Name = "TopMargin"
        Me.TopMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'BottomMargin
        '
        Me.BottomMargin.HeightF = 25.0!
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'ReportHeader
        '
        Me.ReportHeader.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel2, Me.XrPanel4})
        Me.ReportHeader.HeightF = 89.99999!
        Me.ReportHeader.Name = "ReportHeader"
        '
        'XrPanel2
        '
        Me.XrPanel2.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel2.BorderWidth = 2
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelABSError, Me.XrLabelMeasureUnit, Me.XrLabelVisibleResultValue, Me.XrLabelAlarmsList, Me.XrLabelRELErrorPercent, Me.XrLabelResultDateTime, Me.XrLabelCalcRunNumber})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(10.00023!, 66.87498!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(680.9998!, 23.125!)
        Me.XrPanel2.StylePriority.UseBorders = False
        Me.XrPanel2.StylePriority.UseBorderWidth = False
        '
        'XrLabelABSError
        '
        Me.XrLabelABSError.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelABSError.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelABSError.LocationFloat = New DevExpress.Utils.PointFloat(369.7635!, 0.0!)
        Me.XrLabelABSError.Name = "XrLabelABSError"
        Me.XrLabelABSError.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelABSError.SizeF = New System.Drawing.SizeF(78.22507!, 19.99995!)
        Me.XrLabelABSError.StylePriority.UseBorders = False
        Me.XrLabelABSError.StylePriority.UseFont = False
        Me.XrLabelABSError.StylePriority.UsePadding = False
        Me.XrLabelABSError.StylePriority.UseTextAlignment = False
        Me.XrLabelABSError.Text = "ABSError"
        Me.XrLabelABSError.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrLabelMeasureUnit
        '
        Me.XrLabelMeasureUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelMeasureUnit.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelMeasureUnit.LocationFloat = New DevExpress.Utils.PointFloat(283.2095!, 0.0!)
        Me.XrLabelMeasureUnit.Name = "XrLabelMeasureUnit"
        Me.XrLabelMeasureUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelMeasureUnit.SizeF = New System.Drawing.SizeF(84.84799!, 20.00001!)
        Me.XrLabelMeasureUnit.StylePriority.UseBorders = False
        Me.XrLabelMeasureUnit.StylePriority.UseFont = False
        Me.XrLabelMeasureUnit.StylePriority.UsePadding = False
        Me.XrLabelMeasureUnit.StylePriority.UseTextAlignment = False
        Me.XrLabelMeasureUnit.Text = "Unit"
        Me.XrLabelMeasureUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrLabelVisibleResultValue
        '
        Me.XrLabelVisibleResultValue.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelVisibleResultValue.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelVisibleResultValue.LocationFloat = New DevExpress.Utils.PointFloat(198.7094!, 0.0!)
        Me.XrLabelVisibleResultValue.Name = "XrLabelVisibleResultValue"
        Me.XrLabelVisibleResultValue.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 5, 2, 2, 100.0!)
        Me.XrLabelVisibleResultValue.SizeF = New System.Drawing.SizeF(84.50012!, 20.00002!)
        Me.XrLabelVisibleResultValue.StylePriority.UseBorders = False
        Me.XrLabelVisibleResultValue.StylePriority.UseFont = False
        Me.XrLabelVisibleResultValue.StylePriority.UsePadding = False
        Me.XrLabelVisibleResultValue.StylePriority.UseTextAlignment = False
        Me.XrLabelVisibleResultValue.Text = "Value"
        Me.XrLabelVisibleResultValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelAlarmsList
        '
        Me.XrLabelAlarmsList.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelAlarmsList.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelAlarmsList.LocationFloat = New DevExpress.Utils.PointFloat(537.8899!, 0.0!)
        Me.XrLabelAlarmsList.Name = "XrLabelAlarmsList"
        Me.XrLabelAlarmsList.Padding = New DevExpress.XtraPrinting.PaddingInfo(5, 2, 2, 2, 100.0!)
        Me.XrLabelAlarmsList.SizeF = New System.Drawing.SizeF(112.5259!, 20.00001!)
        Me.XrLabelAlarmsList.StylePriority.UseBorders = False
        Me.XrLabelAlarmsList.StylePriority.UseFont = False
        Me.XrLabelAlarmsList.StylePriority.UsePadding = False
        Me.XrLabelAlarmsList.Text = "AlarmsList"
        '
        'XrLabelRELErrorPercent
        '
        Me.XrLabelRELErrorPercent.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelRELErrorPercent.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelRELErrorPercent.LocationFloat = New DevExpress.Utils.PointFloat(447.9886!, 0.0!)
        Me.XrLabelRELErrorPercent.Name = "XrLabelRELErrorPercent"
        Me.XrLabelRELErrorPercent.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelRELErrorPercent.SizeF = New System.Drawing.SizeF(89.90109!, 20.00001!)
        Me.XrLabelRELErrorPercent.StylePriority.UseBorders = False
        Me.XrLabelRELErrorPercent.StylePriority.UseFont = False
        Me.XrLabelRELErrorPercent.StylePriority.UsePadding = False
        Me.XrLabelRELErrorPercent.StylePriority.UseTextAlignment = False
        Me.XrLabelRELErrorPercent.Text = "RELErrorPercent"
        Me.XrLabelRELErrorPercent.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrLabelResultDateTime
        '
        Me.XrLabelResultDateTime.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelResultDateTime.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelResultDateTime.LocationFloat = New DevExpress.Utils.PointFloat(52.41661!, 0.0!)
        Me.XrLabelResultDateTime.Name = "XrLabelResultDateTime"
        Me.XrLabelResultDateTime.Padding = New DevExpress.XtraPrinting.PaddingInfo(5, 2, 2, 2, 100.0!)
        Me.XrLabelResultDateTime.SizeF = New System.Drawing.SizeF(141.0832!, 20.00001!)
        Me.XrLabelResultDateTime.StylePriority.UseBorders = False
        Me.XrLabelResultDateTime.StylePriority.UseFont = False
        Me.XrLabelResultDateTime.StylePriority.UsePadding = False
        Me.XrLabelResultDateTime.Text = "Date"
        '
        'XrLabelCalcRunNumber
        '
        Me.XrLabelCalcRunNumber.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalcRunNumber.Font = New System.Drawing.Font("Verdana", 8.0!, System.Drawing.FontStyle.Bold)
        Me.XrLabelCalcRunNumber.LocationFloat = New DevExpress.Utils.PointFloat(20.41664!, 0.0!)
        Me.XrLabelCalcRunNumber.Name = "XrLabelCalcRunNumber"
        Me.XrLabelCalcRunNumber.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelCalcRunNumber.SizeF = New System.Drawing.SizeF(32.0!, 20.0!)
        Me.XrLabelCalcRunNumber.StylePriority.UseBorders = False
        Me.XrLabelCalcRunNumber.StylePriority.UseFont = False
        Me.XrLabelCalcRunNumber.StylePriority.UsePadding = False
        Me.XrLabelCalcRunNumber.StylePriority.UseTextAlignment = False
        Me.XrLabelCalcRunNumber.Text = "n"
        Me.XrLabelCalcRunNumber.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrPanel4
        '
        Me.XrPanel4.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel4.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel4.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel4.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrRanges, Me.XrLabelRanges, Me.XrLabelCV, Me.XrCV, Me.XrSD, Me.XrLabelSD, Me.XrUnit, Me.XrLabelUnit, Me.XrMean, Me.XrLotNumber, Me.XrLabelMean, Me.XrLabelLotNumber, Me.XrControlName, Me.XrLabelControlName})
        Me.XrPanel4.LocationFloat = New DevExpress.Utils.PointFloat(0.0001907349!, 0.8333365!)
        Me.XrPanel4.Name = "XrPanel4"
        Me.XrPanel4.SizeF = New System.Drawing.SizeF(690.9998!, 49.79167!)
        Me.XrPanel4.StylePriority.UseBackColor = False
        Me.XrPanel4.StylePriority.UseBorderColor = False
        Me.XrPanel4.StylePriority.UseBorders = False
        '
        'XrRanges
        '
        Me.XrRanges.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrRanges.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrRanges.LocationFloat = New DevExpress.Utils.PointFloat(547.8899!, 25.00002!)
        Me.XrRanges.Name = "XrRanges"
        Me.XrRanges.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrRanges.SizeF = New System.Drawing.SizeF(92.10992!, 23.0!)
        Me.XrRanges.StylePriority.UseBorders = False
        Me.XrRanges.StylePriority.UseFont = False
        Me.XrRanges.StylePriority.UseTextAlignment = False
        Me.XrRanges.Text = "XrRanges"
        Me.XrRanges.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrLabelRanges
        '
        Me.XrLabelRanges.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelRanges.Font = New System.Drawing.Font("Verdana", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelRanges.LocationFloat = New DevExpress.Utils.PointFloat(547.8899!, 5.000035!)
        Me.XrLabelRanges.Name = "XrLabelRanges"
        Me.XrLabelRanges.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelRanges.SizeF = New System.Drawing.SizeF(92.10992!, 19.99998!)
        Me.XrLabelRanges.StylePriority.UseBorders = False
        Me.XrLabelRanges.StylePriority.UseFont = False
        Me.XrLabelRanges.StylePriority.UseTextAlignment = False
        Me.XrLabelRanges.Text = "Ranges"
        Me.XrLabelRanges.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrLabelCV
        '
        Me.XrLabelCV.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCV.Font = New System.Drawing.Font("Verdana", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCV.LocationFloat = New DevExpress.Utils.PointFloat(503.0982!, 5.000035!)
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
        Me.XrCV.LocationFloat = New DevExpress.Utils.PointFloat(503.0982!, 25.00002!)
        Me.XrCV.Name = "XrCV"
        Me.XrCV.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrCV.SizeF = New System.Drawing.SizeF(44.79169!, 23.0!)
        Me.XrCV.StylePriority.UseBorders = False
        Me.XrCV.StylePriority.UseFont = False
        Me.XrCV.StylePriority.UseTextAlignment = False
        Me.XrCV.Text = "XrCV"
        Me.XrCV.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrSD
        '
        Me.XrSD.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrSD.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrSD.LocationFloat = New DevExpress.Utils.PointFloat(458.3064!, 25.00002!)
        Me.XrSD.Name = "XrSD"
        Me.XrSD.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrSD.SizeF = New System.Drawing.SizeF(44.79166!, 23.0!)
        Me.XrSD.StylePriority.UseBorders = False
        Me.XrSD.StylePriority.UseFont = False
        Me.XrSD.StylePriority.UseTextAlignment = False
        Me.XrSD.Text = "XrSD"
        Me.XrSD.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelSD
        '
        Me.XrLabelSD.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelSD.Font = New System.Drawing.Font("Verdana", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelSD.LocationFloat = New DevExpress.Utils.PointFloat(458.3064!, 5.000035!)
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
        Me.XrUnit.LocationFloat = New DevExpress.Utils.PointFloat(399.9732!, 25.00002!)
        Me.XrUnit.Name = "XrUnit"
        Me.XrUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrUnit.SizeF = New System.Drawing.SizeF(58.33331!, 23.0!)
        Me.XrUnit.StylePriority.UseBorders = False
        Me.XrUnit.StylePriority.UseFont = False
        Me.XrUnit.StylePriority.UseTextAlignment = False
        Me.XrUnit.Text = "XrUnit"
        Me.XrUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrLabelUnit
        '
        Me.XrLabelUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelUnit.Font = New System.Drawing.Font("Verdana", 8.5!, System.Drawing.FontStyle.Bold)
        Me.XrLabelUnit.LocationFloat = New DevExpress.Utils.PointFloat(399.9732!, 5.000035!)
        Me.XrLabelUnit.Name = "XrLabelUnit"
        Me.XrLabelUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelUnit.SizeF = New System.Drawing.SizeF(58.33331!, 19.99998!)
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
        Me.XrMean.LocationFloat = New DevExpress.Utils.PointFloat(327.0566!, 25.00002!)
        Me.XrMean.Name = "XrMean"
        Me.XrMean.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrMean.SizeF = New System.Drawing.SizeF(72.91666!, 23.0!)
        Me.XrMean.StylePriority.UseBorders = False
        Me.XrMean.StylePriority.UseFont = False
        Me.XrMean.StylePriority.UseTextAlignment = False
        Me.XrMean.Text = "XrMean"
        Me.XrMean.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLotNumber
        '
        Me.XrLotNumber.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLotNumber.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrLotNumber.LocationFloat = New DevExpress.Utils.PointFloat(188.6459!, 25.00003!)
        Me.XrLotNumber.Name = "XrLotNumber"
        Me.XrLotNumber.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLotNumber.SizeF = New System.Drawing.SizeF(129.1667!, 20.0!)
        Me.XrLotNumber.StylePriority.UseBorders = False
        Me.XrLotNumber.StylePriority.UseFont = False
        Me.XrLotNumber.Text = "XrLotNumber"
        '
        'XrLabelMean
        '
        Me.XrLabelMean.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelMean.Font = New System.Drawing.Font("Verdana", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelMean.LocationFloat = New DevExpress.Utils.PointFloat(327.0566!, 5.000035!)
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
        Me.XrLabelLotNumber.Font = New System.Drawing.Font("Verdana", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelLotNumber.LocationFloat = New DevExpress.Utils.PointFloat(188.6459!, 4.000012!)
        Me.XrLabelLotNumber.Name = "XrLabelLotNumber"
        Me.XrLabelLotNumber.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelLotNumber.SizeF = New System.Drawing.SizeF(129.1667!, 20.0!)
        Me.XrLabelLotNumber.StylePriority.UseBorders = False
        Me.XrLabelLotNumber.StylePriority.UseFont = False
        Me.XrLabelLotNumber.Text = "Lot Number"
        '
        'XrControlName
        '
        Me.XrControlName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrControlName.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrControlName.LocationFloat = New DevExpress.Utils.PointFloat(10.00004!, 25.00002!)
        Me.XrControlName.Name = "XrControlName"
        Me.XrControlName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrControlName.SizeF = New System.Drawing.SizeF(173.9583!, 20.00001!)
        Me.XrControlName.StylePriority.UseBorders = False
        Me.XrControlName.StylePriority.UseFont = False
        Me.XrControlName.Text = "XrControlName"
        '
        'XrLabelControlName
        '
        Me.XrLabelControlName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelControlName.Font = New System.Drawing.Font("Verdana", 8.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelControlName.LocationFloat = New DevExpress.Utils.PointFloat(10.00004!, 4.000012!)
        Me.XrLabelControlName.Name = "XrLabelControlName"
        Me.XrLabelControlName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelControlName.SizeF = New System.Drawing.SizeF(173.9583!, 20.0!)
        Me.XrLabelControlName.StylePriority.UseBorders = False
        Me.XrLabelControlName.StylePriority.UseFont = False
        Me.XrLabelControlName.Text = "Control"
        '
        'GroupFooter1
        '
        Me.GroupFooter1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPnlLegend, Me.XrLJGraph, Me.XrPanel3})
        Me.GroupFooter1.HeightF = 385.0!
        Me.GroupFooter1.Name = "GroupFooter1"
        '
        'XrPnlLegend
        '
        Me.XrPnlLegend.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPnlLegend.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPnlLegend.Borders = CType(((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPnlLegend.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPicWarning, Me.XrLabelWarning, Me.XrLabelError, Me.XrPicError})
        Me.XrPnlLegend.LocationFloat = New DevExpress.Utils.PointFloat(10.00023!, 351.6666!)
        Me.XrPnlLegend.Name = "XrPnlLegend"
        Me.XrPnlLegend.SizeF = New System.Drawing.SizeF(680.9998!, 31.25!)
        Me.XrPnlLegend.StylePriority.UseBackColor = False
        Me.XrPnlLegend.StylePriority.UseBorderColor = False
        Me.XrPnlLegend.StylePriority.UseBorders = False
        '
        'XrPicWarning
        '
        Me.XrPicWarning.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrPicWarning.Image = CType(resources.GetObject("XrPicWarning.Image"), System.Drawing.Image)
        Me.XrPicWarning.LocationFloat = New DevExpress.Utils.PointFloat(177.4998!, 6.250031!)
        Me.XrPicWarning.Name = "XrPicWarning"
        Me.XrPicWarning.SizeF = New System.Drawing.SizeF(16.0!, 16.0!)
        Me.XrPicWarning.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage
        Me.XrPicWarning.StylePriority.UseBorders = False
        '
        'XrLabelWarning
        '
        Me.XrLabelWarning.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelWarning.Font = New System.Drawing.Font("Verdana", 9.0!)
        Me.XrLabelWarning.LocationFloat = New DevExpress.Utils.PointFloat(193.4998!, 6.250031!)
        Me.XrLabelWarning.Name = "XrLabelWarning"
        Me.XrLabelWarning.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelWarning.SizeF = New System.Drawing.SizeF(114.3128!, 19.99997!)
        Me.XrLabelWarning.StylePriority.UseBorders = False
        Me.XrLabelWarning.StylePriority.UseFont = False
        Me.XrLabelWarning.Text = "XrLabelWarning"
        '
        'XrLabelError
        '
        Me.XrLabelError.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelError.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelError.LocationFloat = New DevExpress.Utils.PointFloat(346.8481!, 6.250031!)
        Me.XrLabelError.Name = "XrLabelError"
        Me.XrLabelError.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelError.SizeF = New System.Drawing.SizeF(117.0834!, 19.99997!)
        Me.XrLabelError.StylePriority.UseBorders = False
        Me.XrLabelError.StylePriority.UseFont = False
        Me.XrLabelError.Text = "XrLabelError"
        '
        'XrPicError
        '
        Me.XrPicError.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrPicError.Image = CType(resources.GetObject("XrPicError.Image"), System.Drawing.Image)
        Me.XrPicError.LocationFloat = New DevExpress.Utils.PointFloat(330.848!, 7.333374!)
        Me.XrPicError.Name = "XrPicError"
        Me.XrPicError.SizeF = New System.Drawing.SizeF(16.0!, 16.0!)
        Me.XrPicError.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage
        Me.XrPicError.StylePriority.UseBorders = False
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
        Me.XrLJGraph.LocationFloat = New DevExpress.Utils.PointFloat(9.999974!, 55.2083!)
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
        Me.XrLJGraph.SizeF = New System.Drawing.SizeF(681.0!, 296.4583!)
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
        Me.XrPanel3.LocationFloat = New DevExpress.Utils.PointFloat(9.999974!, 10.00001!)
        Me.XrPanel3.Name = "XrPanel3"
        Me.XrPanel3.SizeF = New System.Drawing.SizeF(681.0!, 29.99999!)
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
        'ControlLotID
        '
        Me.ControlLotID.Name = "ControlLotID"
        Me.ControlLotID.Type = GetType(Integer)
        Me.ControlLotID.Value = 0
        Me.ControlLotID.Visible = False
        '
        'QCIndividualResultsByTestControlReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.ReportHeader, Me.GroupFooter1})
        Me.DataMember = "tqcResults"
        Me.DataSource = Me.QcResultsDS1
        Me.FilterString = "[QCControlLotID] = ?ControlLotID"
        Me.Margins = New System.Drawing.Printing.Margins(63, 63, 38, 25)
        Me.PageHeight = 1169
        Me.PageWidth = 827
        Me.PaperKind = System.Drawing.Printing.PaperKind.A4
        Me.Parameters.AddRange(New DevExpress.XtraReports.Parameters.Parameter() {Me.ControlLotID})
        Me.RequestParameters = False
        Me.Version = "10.2"
        CType(Me.XrTableReport, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.QcResultsDS1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(LineSeriesView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(LineSeriesView2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrLJGraph, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.QcIndividualResultsByTestReport1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.QcIndividualResultsByTestReport2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.QcIndividualResultsByTestReport3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.QcIndividualResultsByTestReport4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.QcIndividualResultsByTestReport5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents ReportHeader As DevExpress.XtraReports.UI.ReportHeaderBand
    Friend WithEvents XrPanel4 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLotNumber As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelMean As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelLotNumber As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrControlName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelControlName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents GroupFooter1 As DevExpress.XtraReports.UI.GroupFooterBand
    Friend WithEvents QcResultsDS1 As Biosystems.Ax00.Types.QCResultsDS
    Friend WithEvents XrTableReport As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableReportRow As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrCellIncludedInMean As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellCalcRunNumber As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellResultDateTime As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellVisibleResultValue As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellMeasureUnit As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellABSError As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellRELErrorPercent As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrCellAlarmsList As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrPanel2 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelABSError As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelVisibleResultValue As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelAlarmsList As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelRELErrorPercent As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelResultDateTime As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCalcRunNumber As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLJGraph As DevExpress.XtraReports.UI.XRChart
    Friend WithEvents XrPanel3 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrGraphHeaderLabel As DevExpress.XtraReports.UI.XRLabel
    Public WithEvents ControlLotID As DevExpress.XtraReports.Parameters.Parameter
    Friend WithEvents XrMean As DevExpress.XtraReports.UI.XRLabel
    Private WithEvents QcIndividualResultsByTestReport1 As Biosystems.Ax00.PresentationCOM.QCIndividualResultsByTestReport
    Private WithEvents QcIndividualResultsByTestReport2 As Biosystems.Ax00.PresentationCOM.QCIndividualResultsByTestReport
    Private WithEvents QcIndividualResultsByTestReport3 As Biosystems.Ax00.PresentationCOM.QCIndividualResultsByTestReport
    Private WithEvents QcIndividualResultsByTestReport4 As Biosystems.Ax00.PresentationCOM.QCIndividualResultsByTestReport
    Private WithEvents QcIndividualResultsByTestReport5 As Biosystems.Ax00.PresentationCOM.QCIndividualResultsByTestReport
    Friend WithEvents XrUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrRanges As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelRanges As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCV As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrCV As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrSD As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelSD As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPnlLegend As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrPicWarning As DevExpress.XtraReports.UI.XRPictureBox
    Friend WithEvents XrLabelWarning As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelError As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPicError As DevExpress.XtraReports.UI.XRPictureBox
    Friend WithEvents XrLabelMeasureUnit As DevExpress.XtraReports.UI.XRLabel
End Class
