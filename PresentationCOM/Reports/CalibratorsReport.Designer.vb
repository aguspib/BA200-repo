<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class CalibratorsReport
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
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrLabelUnit = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelConc = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelNum = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelYAxis = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelXAxis = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCurveType = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelSampleType = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelTest = New DevExpress.XtraReports.UI.XRLabel
        Me.XrPanel1 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrLabelExpirationDateValue = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabel12 = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabel11 = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelExpirationDate = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCalibratorsNum = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelLotNumber = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelCalibName = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelName = New DevExpress.XtraReports.UI.XRLabel
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand
        Me.XrControlStyle1 = New DevExpress.XtraReports.UI.XRControlStyle
        Me.XrHeaderLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand
        Me.CalibratorsDS1 = New Biosystems.Ax00.Types.CalibratorsDS
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand
        Me.Detail1 = New DevExpress.XtraReports.UI.DetailBand
        Me.XrTableDetails = New DevExpress.XtraReports.UI.XRTable
        Me.XrTableRowDetails = New DevExpress.XtraReports.UI.XRTableRow
        Me.XrTableCellTest = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCellSampleType = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCellNo = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell4 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCellMeasureUnit = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCellCurveType1 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCellCurveType2 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCellXAxis = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCellYAxis = New DevExpress.XtraReports.UI.XRTableCell
        Me.GroupHeader2 = New DevExpress.XtraReports.UI.GroupHeaderBand
        CType(Me.CalibratorsDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTableDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail.HeightF = 0.0!
        Me.Detail.KeepTogether = True
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.StylePriority.UseFont = False
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrPanel2
        '
        Me.XrPanel2.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel2.BorderWidth = 2
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelUnit, Me.XrLabelConc, Me.XrLabelNum, Me.XrLabelYAxis, Me.XrLabelXAxis, Me.XrLabelCurveType, Me.XrLabelSampleType, Me.XrLabelTest})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(10.0!, 76.0!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(630.0!, 23.12501!)
        Me.XrPanel2.StylePriority.UseBorders = False
        Me.XrPanel2.StylePriority.UseBorderWidth = False
        '
        'XrLabelUnit
        '
        Me.XrLabelUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelUnit.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelUnit.LocationFloat = New DevExpress.Utils.PointFloat(344.6249!, 0.0!)
        Me.XrLabelUnit.Name = "XrLabelUnit"
        Me.XrLabelUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelUnit.SizeF = New System.Drawing.SizeF(54.16666!, 19.99997!)
        Me.XrLabelUnit.StylePriority.UseBorders = False
        Me.XrLabelUnit.StylePriority.UseFont = False
        Me.XrLabelUnit.StylePriority.UsePadding = False
        Me.XrLabelUnit.Text = "Unit"
        '
        'XrLabelConc
        '
        Me.XrLabelConc.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelConc.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelConc.LocationFloat = New DevExpress.Utils.PointFloat(281.6249!, 0.0!)
        Me.XrLabelConc.Name = "XrLabelConc"
        Me.XrLabelConc.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelConc.SizeF = New System.Drawing.SizeF(60.0!, 20.00001!)
        Me.XrLabelConc.StylePriority.UseBorders = False
        Me.XrLabelConc.StylePriority.UseFont = False
        Me.XrLabelConc.StylePriority.UsePadding = False
        Me.XrLabelConc.StylePriority.UseTextAlignment = False
        Me.XrLabelConc.Text = "Conc."
        Me.XrLabelConc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelNum
        '
        Me.XrLabelNum.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelNum.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelNum.LocationFloat = New DevExpress.Utils.PointFloat(253.6249!, 0.0!)
        Me.XrLabelNum.Name = "XrLabelNum"
        Me.XrLabelNum.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelNum.SizeF = New System.Drawing.SizeF(28.0!, 20.00001!)
        Me.XrLabelNum.StylePriority.UseBorders = False
        Me.XrLabelNum.StylePriority.UseFont = False
        Me.XrLabelNum.StylePriority.UsePadding = False
        Me.XrLabelNum.StylePriority.UseTextAlignment = False
        Me.XrLabelNum.Text = "No."
        Me.XrLabelNum.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelYAxis
        '
        Me.XrLabelYAxis.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelYAxis.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelYAxis.LocationFloat = New DevExpress.Utils.PointFloat(584.0001!, 0.0!)
        Me.XrLabelYAxis.Name = "XrLabelYAxis"
        Me.XrLabelYAxis.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelYAxis.SizeF = New System.Drawing.SizeF(46.0!, 20.0!)
        Me.XrLabelYAxis.StylePriority.UseBorders = False
        Me.XrLabelYAxis.StylePriority.UseFont = False
        Me.XrLabelYAxis.StylePriority.UsePadding = False
        Me.XrLabelYAxis.Text = "Y-Axis"
        '
        'XrLabelXAxis
        '
        Me.XrLabelXAxis.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelXAxis.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelXAxis.LocationFloat = New DevExpress.Utils.PointFloat(536.0001!, 0.00003178914!)
        Me.XrLabelXAxis.Name = "XrLabelXAxis"
        Me.XrLabelXAxis.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelXAxis.SizeF = New System.Drawing.SizeF(48.0!, 20.0!)
        Me.XrLabelXAxis.StylePriority.UseBorders = False
        Me.XrLabelXAxis.StylePriority.UseFont = False
        Me.XrLabelXAxis.StylePriority.UsePadding = False
        Me.XrLabelXAxis.Text = "X-Axis"
        '
        'XrLabelCurveType
        '
        Me.XrLabelCurveType.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCurveType.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCurveType.LocationFloat = New DevExpress.Utils.PointFloat(418.1669!, 0.0!)
        Me.XrLabelCurveType.Name = "XrLabelCurveType"
        Me.XrLabelCurveType.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelCurveType.SizeF = New System.Drawing.SizeF(100.0!, 20.00001!)
        Me.XrLabelCurveType.StylePriority.UseBorders = False
        Me.XrLabelCurveType.StylePriority.UseFont = False
        Me.XrLabelCurveType.StylePriority.UsePadding = False
        Me.XrLabelCurveType.Text = "Curve Type"
        '
        'XrLabelSampleType
        '
        Me.XrLabelSampleType.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelSampleType.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelSampleType.LocationFloat = New DevExpress.Utils.PointFloat(140.0!, 0.0!)
        Me.XrLabelSampleType.Name = "XrLabelSampleType"
        Me.XrLabelSampleType.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelSampleType.SizeF = New System.Drawing.SizeF(113.6249!, 20.00001!)
        Me.XrLabelSampleType.StylePriority.UseBorders = False
        Me.XrLabelSampleType.StylePriority.UseFont = False
        Me.XrLabelSampleType.StylePriority.UsePadding = False
        Me.XrLabelSampleType.Text = "Sample Type"
        '
        'XrLabelTest
        '
        Me.XrLabelTest.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelTest.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelTest.LocationFloat = New DevExpress.Utils.PointFloat(0.00000667572!, 0.0!)
        Me.XrLabelTest.Name = "XrLabelTest"
        Me.XrLabelTest.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 2, 2, 100.0!)
        Me.XrLabelTest.SizeF = New System.Drawing.SizeF(140.0!, 20.00002!)
        Me.XrLabelTest.StylePriority.UseBorders = False
        Me.XrLabelTest.StylePriority.UseFont = False
        Me.XrLabelTest.StylePriority.UsePadding = False
        Me.XrLabelTest.Text = "Test"
        '
        'XrPanel1
        '
        Me.XrPanel1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel1.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel1.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelExpirationDateValue, Me.XrLabel12, Me.XrLabel11, Me.XrLabelExpirationDate, Me.XrLabelCalibratorsNum, Me.XrLabelLotNumber, Me.XrLabelCalibName, Me.XrLabelName})
        Me.XrPanel1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 16.0!)
        Me.XrPanel1.Name = "XrPanel1"
        Me.XrPanel1.SizeF = New System.Drawing.SizeF(650.0001!, 45.0!)
        Me.XrPanel1.StylePriority.UseBackColor = False
        Me.XrPanel1.StylePriority.UseBorderColor = False
        Me.XrPanel1.StylePriority.UseBorders = False
        '
        'XrLabelExpirationDateValue
        '
        Me.XrLabelExpirationDateValue.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelExpirationDateValue.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.ExpirationDate")})
        Me.XrLabelExpirationDateValue.LocationFloat = New DevExpress.Utils.PointFloat(460.0!, 25.0!)
        Me.XrLabelExpirationDateValue.Name = "XrLabelExpirationDateValue"
        Me.XrLabelExpirationDateValue.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelExpirationDateValue.SizeF = New System.Drawing.SizeF(179.9997!, 20.0!)
        Me.XrLabelExpirationDateValue.StylePriority.UseBorders = False
        Me.XrLabelExpirationDateValue.StylePriority.UseTextAlignment = False
        Me.XrLabelExpirationDateValue.Text = "XrLabelExpirationDateValue"
        Me.XrLabelExpirationDateValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabel12
        '
        Me.XrLabel12.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel12.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.NumberOfCalibrators")})
        Me.XrLabel12.LocationFloat = New DevExpress.Utils.PointFloat(370.0!, 25.00001!)
        Me.XrLabel12.Name = "XrLabel12"
        Me.XrLabel12.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel12.SizeF = New System.Drawing.SizeF(81.70844!, 20.0!)
        Me.XrLabel12.StylePriority.UseBorders = False
        Me.XrLabel12.StylePriority.UseTextAlignment = False
        Me.XrLabel12.Text = "XrLabel12"
        Me.XrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabel11
        '
        Me.XrLabel11.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel11.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.LotNumber")})
        Me.XrLabel11.LocationFloat = New DevExpress.Utils.PointFloat(215.0!, 25.00005!)
        Me.XrLabel11.Name = "XrLabel11"
        Me.XrLabel11.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel11.SizeF = New System.Drawing.SizeF(129.1667!, 20.0!)
        Me.XrLabel11.StylePriority.UseBorders = False
        Me.XrLabel11.Text = "XrLabel11"
        '
        'XrLabelExpirationDate
        '
        Me.XrLabelExpirationDate.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelExpirationDate.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelExpirationDate.LocationFloat = New DevExpress.Utils.PointFloat(460.0!, 4.000019!)
        Me.XrLabelExpirationDate.Name = "XrLabelExpirationDate"
        Me.XrLabelExpirationDate.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelExpirationDate.SizeF = New System.Drawing.SizeF(179.9998!, 19.99999!)
        Me.XrLabelExpirationDate.StylePriority.UseBorders = False
        Me.XrLabelExpirationDate.StylePriority.UseFont = False
        Me.XrLabelExpirationDate.StylePriority.UseTextAlignment = False
        Me.XrLabelExpirationDate.Text = "Expiration Date"
        Me.XrLabelExpirationDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelCalibratorsNum
        '
        Me.XrLabelCalibratorsNum.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalibratorsNum.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCalibratorsNum.LocationFloat = New DevExpress.Utils.PointFloat(370.0!, 4.000019!)
        Me.XrLabelCalibratorsNum.Name = "XrLabelCalibratorsNum"
        Me.XrLabelCalibratorsNum.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCalibratorsNum.SizeF = New System.Drawing.SizeF(81.70844!, 20.0!)
        Me.XrLabelCalibratorsNum.StylePriority.UseBorders = False
        Me.XrLabelCalibratorsNum.StylePriority.UseFont = False
        Me.XrLabelCalibratorsNum.StylePriority.UseTextAlignment = False
        Me.XrLabelCalibratorsNum.Text = "No."
        Me.XrLabelCalibratorsNum.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelLotNumber
        '
        Me.XrLabelLotNumber.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelLotNumber.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelLotNumber.LocationFloat = New DevExpress.Utils.PointFloat(215.0!, 4.000019!)
        Me.XrLabelLotNumber.Name = "XrLabelLotNumber"
        Me.XrLabelLotNumber.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelLotNumber.SizeF = New System.Drawing.SizeF(129.1667!, 20.0!)
        Me.XrLabelLotNumber.StylePriority.UseBorders = False
        Me.XrLabelLotNumber.StylePriority.UseFont = False
        Me.XrLabelLotNumber.Text = "Lot Number"
        '
        'XrLabelCalibName
        '
        Me.XrLabelCalibName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalibName.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.CalibratorName")})
        Me.XrLabelCalibName.LocationFloat = New DevExpress.Utils.PointFloat(10.00001!, 25.00002!)
        Me.XrLabelCalibName.Name = "XrLabelCalibName"
        Me.XrLabelCalibName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCalibName.SizeF = New System.Drawing.SizeF(189.5833!, 20.00001!)
        Me.XrLabelCalibName.StylePriority.UseBorders = False
        Me.XrLabelCalibName.Text = "XrLabelCalibName"
        '
        'XrLabelName
        '
        Me.XrLabelName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelName.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelName.LocationFloat = New DevExpress.Utils.PointFloat(10.00001!, 4.000019!)
        Me.XrLabelName.Name = "XrLabelName"
        Me.XrLabelName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelName.SizeF = New System.Drawing.SizeF(189.5833!, 20.0!)
        Me.XrLabelName.StylePriority.UseBorders = False
        Me.XrLabelName.StylePriority.UseFont = False
        Me.XrLabelName.Text = "Name"
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
        Me.GroupHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrHeaderLabel})
        Me.GroupHeader1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupHeader1.HeightF = 48.04166!
        Me.GroupHeader1.Name = "GroupHeader1"
        Me.GroupHeader1.RepeatEveryPage = True
        Me.GroupHeader1.StylePriority.UseFont = False
        '
        'CalibratorsDS1
        '
        Me.CalibratorsDS1.DataSetName = "CalibratorsDS"
        Me.CalibratorsDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'DetailReport
        '
        Me.DetailReport.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail1, Me.GroupHeader2})
        Me.DetailReport.DataMember = "tparCalibrators.tparCalibrators_tparCalibratorsTests"
        Me.DetailReport.DataSource = Me.CalibratorsDS1
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
        '
        'Detail1
        '
        Me.Detail1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTableDetails})
        Me.Detail1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail1.HeightF = 23.0!
        Me.Detail1.Name = "Detail1"
        Me.Detail1.StylePriority.UseFont = False
        '
        'XrTableDetails
        '
        Me.XrTableDetails.AnchorVertical = DevExpress.XtraReports.UI.VerticalAnchorStyles.Top
        Me.XrTableDetails.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrTableDetails.LocationFloat = New DevExpress.Utils.PointFloat(10.0!, 0.0!)
        Me.XrTableDetails.Name = "XrTableDetails"
        Me.XrTableDetails.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRowDetails})
        Me.XrTableDetails.SizeF = New System.Drawing.SizeF(630.0!, 22.0!)
        Me.XrTableDetails.StylePriority.UseBorders = False
        '
        'XrTableRowDetails
        '
        Me.XrTableRowDetails.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrTableRowDetails.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCellTest, Me.XrTableCellSampleType, Me.XrTableCellNo, Me.XrTableCell4, Me.XrTableCellMeasureUnit, Me.XrTableCellCurveType1, Me.XrTableCellCurveType2, Me.XrTableCellXAxis, Me.XrTableCellYAxis})
        Me.XrTableRowDetails.Name = "XrTableRowDetails"
        Me.XrTableRowDetails.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 3, 0, 100.0!)
        Me.XrTableRowDetails.StylePriority.UseBorders = False
        Me.XrTableRowDetails.StylePriority.UsePadding = False
        Me.XrTableRowDetails.StylePriority.UseTextAlignment = False
        Me.XrTableRowDetails.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableRowDetails.Weight = 1.1
        '
        'XrTableCellTest
        '
        Me.XrTableCellTest.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.tparCalibrators_tparCalibratorsTests.TestName")})
        Me.XrTableCellTest.Name = "XrTableCellTest"
        Me.XrTableCellTest.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 0, 3, 0, 100.0!)
        Me.XrTableCellTest.StylePriority.UsePadding = False
        Me.XrTableCellTest.Text = "XrTableCellTest"
        Me.XrTableCellTest.Weight = 0.66190476190476188
        '
        'XrTableCellSampleType
        '
        Me.XrTableCellSampleType.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.tparCalibrators_tparCalibratorsTests.SampleType")})
        Me.XrTableCellSampleType.Name = "XrTableCellSampleType"
        Me.XrTableCellSampleType.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 0, 3, 0, 100.0!)
        Me.XrTableCellSampleType.StylePriority.UsePadding = False
        Me.XrTableCellSampleType.Text = "XrTableCellSampleType"
        Me.XrTableCellSampleType.Weight = 0.54285714285714293
        '
        'XrTableCellNo
        '
        Me.XrTableCellNo.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.tparCalibrators_tparCalibratorsTests.CalibratorNum")})
        Me.XrTableCellNo.Name = "XrTableCellNo"
        Me.XrTableCellNo.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 3, 0, 100.0!)
        Me.XrTableCellNo.StylePriority.UsePadding = False
        Me.XrTableCellNo.StylePriority.UseTextAlignment = False
        Me.XrTableCellNo.Text = "XrTableCellNo"
        Me.XrTableCellNo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCellNo.Weight = 0.13333333333333336
        '
        'XrTableCell4
        '
        Me.XrTableCell4.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.tparCalibrators_tparCalibratorsTests.ConcentrationWithDecimals")})
        Me.XrTableCell4.Name = "XrTableCell4"
        Me.XrTableCell4.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 3, 3, 0, 100.0!)
        Me.XrTableCell4.StylePriority.UsePadding = False
        Me.XrTableCell4.StylePriority.UseTextAlignment = False
        Me.XrTableCell4.Text = "XrTableCell4"
        Me.XrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell4.Weight = 0.30297575451078868
        '
        'XrTableCellMeasureUnit
        '
        Me.XrTableCellMeasureUnit.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.tparCalibrators_tparCalibratorsTests.MeasureUnit")})
        Me.XrTableCellMeasureUnit.Name = "XrTableCellMeasureUnit"
        Me.XrTableCellMeasureUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 0, 3, 0, 100.0!)
        Me.XrTableCellMeasureUnit.StylePriority.UsePadding = False
        Me.XrTableCellMeasureUnit.Text = "XrTableCellMeasureUnit"
        Me.XrTableCellMeasureUnit.Weight = 0.25793645949590766
        '
        'XrTableCellCurveType1
        '
        Me.XrTableCellCurveType1.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.tparCalibrators_tparCalibratorsTests.CurveType")})
        Me.XrTableCellCurveType1.Name = "XrTableCellCurveType1"
        Me.XrTableCellCurveType1.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 2, 3, 0, 100.0!)
        Me.XrTableCellCurveType1.StylePriority.UsePadding = False
        Me.XrTableCellCurveType1.StylePriority.UseTextAlignment = False
        Me.XrTableCellCurveType1.Text = "XrTableCellCurveType1"
        Me.XrTableCellCurveType1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCellCurveType1.Weight = 0.30337350936163038
        '
        'XrTableCellCurveType2
        '
        Me.XrTableCellCurveType2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.tparCalibrators_tparCalibratorsTests.CurveGrowthType")})
        Me.XrTableCellCurveType2.Name = "XrTableCellCurveType2"
        Me.XrTableCellCurveType2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 0, 3, 0, 100.0!)
        Me.XrTableCellCurveType2.StylePriority.UsePadding = False
        Me.XrTableCellCurveType2.Text = "XrTableCellCurveType2"
        Me.XrTableCellCurveType2.Weight = 0.35337365922473712
        '
        'XrTableCellXAxis
        '
        Me.XrTableCellXAxis.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.tparCalibrators_tparCalibratorsTests.CurveAxisXType")})
        Me.XrTableCellXAxis.Name = "XrTableCellXAxis"
        Me.XrTableCellXAxis.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 0, 3, 0, 100.0!)
        Me.XrTableCellXAxis.StylePriority.UsePadding = False
        Me.XrTableCellXAxis.Text = "XrTableCellXAxis"
        Me.XrTableCellXAxis.Weight = 0.21711262975420265
        '
        'XrTableCellYAxis
        '
        Me.XrTableCellYAxis.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "tparCalibrators.tparCalibrators_tparCalibratorsTests.CurveAxisYType")})
        Me.XrTableCellYAxis.Name = "XrTableCellYAxis"
        Me.XrTableCellYAxis.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 0, 3, 0, 100.0!)
        Me.XrTableCellYAxis.StylePriority.UsePadding = False
        Me.XrTableCellYAxis.Text = "XrTableCellYAxis"
        Me.XrTableCellYAxis.Weight = 0.22713274955749513
        '
        'GroupHeader2
        '
        Me.GroupHeader2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel1, Me.XrPanel2})
        Me.GroupHeader2.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupHeader2.HeightF = 99.12501!
        Me.GroupHeader2.KeepTogether = True
        Me.GroupHeader2.Name = "GroupHeader2"
        Me.GroupHeader2.RepeatEveryPage = True
        Me.GroupHeader2.StylePriority.UseFont = False
        '
        'CalibratorsReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport})
        Me.DataMember = "tparCalibrators"
        Me.DataSource = Me.CalibratorsDS1
        Me.Margins = New System.Drawing.Printing.Margins(100, 100, 96, 100)
        Me.StyleSheet.AddRange(New DevExpress.XtraReports.UI.XRControlStyle() {Me.XrControlStyle1})
        Me.Version = "10.2"
        CType(Me.CalibratorsDS1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTableDetails, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents XrControlStyle1 As DevExpress.XtraReports.UI.XRControlStyle
    Protected WithEvents XrHeaderLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents CalibratorsDS1 As Biosystems.Ax00.Types.CalibratorsDS
    Friend WithEvents DetailReport As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents Detail1 As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents XrPanel1 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelCalibName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel2 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelTest As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCurveType As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelSampleType As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelConc As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelNum As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelYAxis As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelXAxis As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelExpirationDate As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCalibratorsNum As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelLotNumber As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelExpirationDateValue As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel12 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel11 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents GroupHeader2 As DevExpress.XtraReports.UI.GroupHeaderBand
    Protected WithEvents XrTableDetails As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRowDetails As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCellTest As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCellSampleType As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCellNo As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell4 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCellCurveType1 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCellCurveType2 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCellXAxis As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCellYAxis As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCellMeasureUnit As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrLabelUnit As DevExpress.XtraReports.UI.XRLabel
End Class
