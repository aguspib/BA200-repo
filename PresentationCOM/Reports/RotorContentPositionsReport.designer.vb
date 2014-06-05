<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class RotorContentPositionsReport
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
        Dim XrSummary1 As DevExpress.XtraReports.UI.XRSummary = New DevExpress.XtraReports.UI.XRSummary()
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand()
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.XrPageInfo1 = New DevExpress.XtraReports.UI.XRPageInfo()
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrLabelRemainingTests = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelBottle = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelVolume = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelExpDate = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelName = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelCell = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelStatus = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelBarcode = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel1 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrWSStartDateTimeLabel = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrHeaderLabel = New DevExpress.XtraReports.UI.XRLabel()
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand()
        Me.Detail1 = New DevExpress.XtraReports.UI.DetailBand()
        Me.XrTable1 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow1 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell2 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.Few = New DevExpress.XtraReports.UI.FormattingRule()
        Me.CellPositionInformationDS1 = New Biosystems.Ax00.Types.CellPositionInformationDS()
        Me.Depleted = New DevExpress.XtraReports.UI.FormattingRule()
        Me.XrTableCell3 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell4 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell1 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell5 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell6 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell7 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell8 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell9 = New DevExpress.XtraReports.UI.XRTableCell()
        CType(Me.XrTable1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CellPositionInformationDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.HeightF = 0.0!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'TopMargin
        '
        Me.TopMargin.HeightF = 59.0!
        Me.TopMargin.Name = "TopMargin"
        Me.TopMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'BottomMargin
        '
        Me.BottomMargin.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPageInfo1})
        Me.BottomMargin.HeightF = 33.0!
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrPageInfo1
        '
        Me.XrPageInfo1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrPageInfo1.LocationFloat = New DevExpress.Utils.PointFloat(634.0!, 10.00001!)
        Me.XrPageInfo1.Name = "XrPageInfo1"
        Me.XrPageInfo1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrPageInfo1.SizeF = New System.Drawing.SizeF(100.0!, 13.0!)
        Me.XrPageInfo1.StylePriority.UseFont = False
        Me.XrPageInfo1.StylePriority.UseTextAlignment = False
        Me.XrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight
        Me.XrPageInfo1.Visible = False
        '
        'GroupHeader1
        '
        Me.GroupHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrWSStartDateTimeLabel, Me.XrPanel2, Me.XrHeaderLabel})
        Me.GroupHeader1.Font = New System.Drawing.Font("Times New Roman", 9.75!, System.Drawing.FontStyle.Italic)
        Me.GroupHeader1.HeightF = 68.75001!
        Me.GroupHeader1.Name = "GroupHeader1"
        Me.GroupHeader1.StylePriority.UseFont = False
        '
        'XrPanel2
        '
        Me.XrPanel2.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel2.BorderWidth = 1
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelRemainingTests, Me.XrLabelBottle, Me.XrLabelVolume, Me.XrLabelExpDate, Me.XrLabelName, Me.XrLabelCell, Me.XrLabelStatus, Me.XrLabelBarcode, Me.XrLabel1})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 45.62499!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(734.0001!, 21.5!)
        Me.XrPanel2.StylePriority.UseBorders = False
        Me.XrPanel2.StylePriority.UseBorderWidth = False
        '
        'XrLabelRemainingTests
        '
        Me.XrLabelRemainingTests.BackColor = System.Drawing.Color.Transparent
        Me.XrLabelRemainingTests.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelRemainingTests.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelRemainingTests.LocationFloat = New DevExpress.Utils.PointFloat(579.0833!, 0.0!)
        Me.XrLabelRemainingTests.Name = "XrLabelRemainingTests"
        Me.XrLabelRemainingTests.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelRemainingTests.SizeF = New System.Drawing.SizeF(50.79169!, 20.0!)
        Me.XrLabelRemainingTests.StylePriority.UseBackColor = False
        Me.XrLabelRemainingTests.StylePriority.UseBorders = False
        Me.XrLabelRemainingTests.StylePriority.UseFont = False
        Me.XrLabelRemainingTests.StylePriority.UsePadding = False
        Me.XrLabelRemainingTests.StylePriority.UseTextAlignment = False
        Me.XrLabelRemainingTests.Text = "Tests"
        Me.XrLabelRemainingTests.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelBottle
        '
        Me.XrLabelBottle.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelBottle.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelBottle.LocationFloat = New DevExpress.Utils.PointFloat(464.2082!, 0.00003051758!)
        Me.XrLabelBottle.Name = "XrLabelBottle"
        Me.XrLabelBottle.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelBottle.SizeF = New System.Drawing.SizeF(46.87497!, 20.0!)
        Me.XrLabelBottle.StylePriority.UseBorders = False
        Me.XrLabelBottle.StylePriority.UseFont = False
        Me.XrLabelBottle.StylePriority.UseTextAlignment = False
        Me.XrLabelBottle.Text = "Bottle"
        Me.XrLabelBottle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelVolume
        '
        Me.XrLabelVolume.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelVolume.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelVolume.LocationFloat = New DevExpress.Utils.PointFloat(511.0832!, 0.0!)
        Me.XrLabelVolume.Name = "XrLabelVolume"
        Me.XrLabelVolume.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelVolume.SizeF = New System.Drawing.SizeF(68.00006!, 20.0!)
        Me.XrLabelVolume.StylePriority.UseBorders = False
        Me.XrLabelVolume.StylePriority.UseFont = False
        Me.XrLabelVolume.StylePriority.UsePadding = False
        Me.XrLabelVolume.StylePriority.UseTextAlignment = False
        Me.XrLabelVolume.Text = "Volume"
        Me.XrLabelVolume.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelExpDate
        '
        Me.XrLabelExpDate.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelExpDate.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelExpDate.LocationFloat = New DevExpress.Utils.PointFloat(390.7082!, 0.00003178914!)
        Me.XrLabelExpDate.Name = "XrLabelExpDate"
        Me.XrLabelExpDate.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelExpDate.SizeF = New System.Drawing.SizeF(73.5!, 20.00001!)
        Me.XrLabelExpDate.StylePriority.UseBorders = False
        Me.XrLabelExpDate.StylePriority.UseFont = False
        Me.XrLabelExpDate.StylePriority.UseTextAlignment = False
        Me.XrLabelExpDate.Text = "ExpDate"
        Me.XrLabelExpDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelName
        '
        Me.XrLabelName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelName.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelName.LocationFloat = New DevExpress.Utils.PointFloat(30.0!, 0.0!)
        Me.XrLabelName.Name = "XrLabelName"
        Me.XrLabelName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelName.SizeF = New System.Drawing.SizeF(137.5!, 20.00001!)
        Me.XrLabelName.StylePriority.UseBorders = False
        Me.XrLabelName.StylePriority.UseFont = False
        Me.XrLabelName.StylePriority.UseTextAlignment = False
        Me.XrLabelName.Text = "Name"
        Me.XrLabelName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelCell
        '
        Me.XrLabelCell.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCell.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCell.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrLabelCell.Name = "XrLabelCell"
        Me.XrLabelCell.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCell.SizeF = New System.Drawing.SizeF(30.0!, 20.00001!)
        Me.XrLabelCell.StylePriority.UseBorders = False
        Me.XrLabelCell.StylePriority.UseFont = False
        Me.XrLabelCell.StylePriority.UseTextAlignment = False
        Me.XrLabelCell.Text = "Cell"
        Me.XrLabelCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelStatus
        '
        Me.XrLabelStatus.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelStatus.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelStatus.LocationFloat = New DevExpress.Utils.PointFloat(629.8749!, 0.0!)
        Me.XrLabelStatus.Name = "XrLabelStatus"
        Me.XrLabelStatus.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelStatus.SizeF = New System.Drawing.SizeF(104.1252!, 20.00001!)
        Me.XrLabelStatus.StylePriority.UseBorders = False
        Me.XrLabelStatus.StylePriority.UseFont = False
        Me.XrLabelStatus.StylePriority.UsePadding = False
        Me.XrLabelStatus.StylePriority.UseTextAlignment = False
        Me.XrLabelStatus.Text = "Status"
        Me.XrLabelStatus.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelBarcode
        '
        Me.XrLabelBarcode.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelBarcode.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelBarcode.LocationFloat = New DevExpress.Utils.PointFloat(167.5!, 0.0!)
        Me.XrLabelBarcode.Name = "XrLabelBarcode"
        Me.XrLabelBarcode.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelBarcode.SizeF = New System.Drawing.SizeF(160.2082!, 20.00004!)
        Me.XrLabelBarcode.StylePriority.UseBorders = False
        Me.XrLabelBarcode.StylePriority.UseFont = False
        Me.XrLabelBarcode.StylePriority.UsePadding = False
        Me.XrLabelBarcode.StylePriority.UseTextAlignment = False
        Me.XrLabelBarcode.Text = "Barcode"
        Me.XrLabelBarcode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabel1
        '
        Me.XrLabel1.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel1.LocationFloat = New DevExpress.Utils.PointFloat(327.7082!, 0.0!)
        Me.XrLabel1.Name = "XrLabel1"
        Me.XrLabel1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel1.SizeF = New System.Drawing.SizeF(63.0!, 20.00001!)
        Me.XrLabel1.StylePriority.UseBorders = False
        Me.XrLabel1.StylePriority.UseFont = False
        Me.XrLabel1.StylePriority.UseTextAlignment = False
        Me.XrLabel1.Text = "Lot"
        Me.XrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrWSStartDateTimeLabel
        '
        Me.XrWSStartDateTimeLabel.BackColor = System.Drawing.Color.LightGray
        Me.XrWSStartDateTimeLabel.BorderColor = System.Drawing.Color.Empty
        Me.XrWSStartDateTimeLabel.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrWSStartDateTimeLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrWSStartDateTimeLabel.LocationFloat = New DevExpress.Utils.PointFloat(346.3751!, 10.00001!)
        Me.XrWSStartDateTimeLabel.Multiline = True
        Me.XrWSStartDateTimeLabel.Name = "XrWSStartDateTimeLabel"
        Me.XrWSStartDateTimeLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(10, 2, 4, 2, 100.0!)
        Me.XrWSStartDateTimeLabel.SizeF = New System.Drawing.SizeF(387.625!, 25.0!)
        Me.XrWSStartDateTimeLabel.StylePriority.UseBackColor = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseBorderColor = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseBorders = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseFont = False
        Me.XrWSStartDateTimeLabel.StylePriority.UsePadding = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseTextAlignment = False
        Me.XrWSStartDateTimeLabel.Text = "WSStartDateTime"
        Me.XrWSStartDateTimeLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
        '
        'XrHeaderLabel
        '
        Me.XrHeaderLabel.BackColor = System.Drawing.Color.LightGray
        Me.XrHeaderLabel.BorderColor = System.Drawing.Color.Empty
        Me.XrHeaderLabel.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrHeaderLabel.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrHeaderLabel.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 10.00001!)
        Me.XrHeaderLabel.Multiline = True
        Me.XrHeaderLabel.Name = "XrHeaderLabel"
        Me.XrHeaderLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(10, 2, 4, 2, 100.0!)
        Me.XrHeaderLabel.SizeF = New System.Drawing.SizeF(734.0!, 25.0!)
        Me.XrHeaderLabel.StylePriority.UseBackColor = False
        Me.XrHeaderLabel.StylePriority.UseBorderColor = False
        Me.XrHeaderLabel.StylePriority.UseBorders = False
        Me.XrHeaderLabel.StylePriority.UseFont = False
        Me.XrHeaderLabel.StylePriority.UsePadding = False
        Me.XrHeaderLabel.StylePriority.UseTextAlignment = False
        Me.XrHeaderLabel.Text = "XrHeaderLabel"
        Me.XrHeaderLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'DetailReport
        '
        Me.DetailReport.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail1})
        Me.DetailReport.DataMember = "ReportTable"
        Me.DetailReport.DataSource = Me.CellPositionInformationDS1
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
        '
        'Detail1
        '
        Me.Detail1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTable1})
        Me.Detail1.HeightF = 25.0!
        Me.Detail1.Name = "Detail1"
        '
        'XrTable1
        '
        Me.XrTable1.Font = New System.Drawing.Font("Times New Roman", 9.75!)
        Me.XrTable1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrTable1.Name = "XrTable1"
        Me.XrTable1.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow1})
        Me.XrTable1.SizeF = New System.Drawing.SizeF(734.0001!, 25.0!)
        Me.XrTable1.StylePriority.UseFont = False
        '
        'XrTableRow1
        '
        Me.XrTableRow1.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell2, Me.XrTableCell3, Me.XrTableCell4, Me.XrTableCell1, Me.XrTableCell5, Me.XrTableCell6, Me.XrTableCell7, Me.XrTableCell8, Me.XrTableCell9})
        Me.XrTableRow1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrTableRow1.Name = "XrTableRow1"
        Me.XrTableRow1.StylePriority.UseFont = False
        Me.XrTableRow1.Weight = 11.5R
        '
        'XrTableCell2
        '
        Me.XrTableCell2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTable.CellNumber")})
        Me.XrTableCell2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell2.FormattingRules.Add(Me.Few)
        Me.XrTableCell2.FormattingRules.Add(Me.Depleted)
        Me.XrTableCell2.Name = "XrTableCell2"
        Me.XrTableCell2.StylePriority.UseFont = False
        Me.XrTableCell2.StylePriority.UseTextAlignment = False
        XrSummary1.Func = DevExpress.XtraReports.UI.SummaryFunc.Custom
        XrSummary1.IgnoreNullValues = True
        Me.XrTableCell2.Summary = XrSummary1
        Me.XrTableCell2.Text = "XrTableCell2"
        Me.XrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        Me.XrTableCell2.Weight = 0.050995330993891655R
        '
        'Few
        '
        Me.Few.Condition = "[StatusColor] == 'FEW'"
        Me.Few.DataMember = "ReportTable"
        Me.Few.DataSource = Me.CellPositionInformationDS1
        '
        '
        '
        Me.Few.Formatting.BackColor = System.Drawing.Color.FromArgb(CType(CType(169, Byte), Integer), CType(CType(138, Byte), Integer), CType(CType(225, Byte), Integer))
        Me.Few.Formatting.ForeColor = System.Drawing.Color.Black
        Me.Few.Name = "Few"
        '
        'CellPositionInformationDS1
        '
        Me.CellPositionInformationDS1.DataSetName = "CellPositionInformationDS"
        Me.CellPositionInformationDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'Depleted
        '
        Me.Depleted.Condition = "[StatusColor] == 'DEPLETED'"
        Me.Depleted.DataMember = "ReportTable"
        Me.Depleted.DataSource = Me.CellPositionInformationDS1
        '
        '
        '
        Me.Depleted.Formatting.BackColor = System.Drawing.Color.Salmon
        Me.Depleted.Formatting.ForeColor = System.Drawing.Color.Black
        Me.Depleted.Name = "Depleted"
        '
        'XrTableCell3
        '
        Me.XrTableCell3.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTable.ReagentName")})
        Me.XrTableCell3.Name = "XrTableCell3"
        Me.XrTableCell3.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell3.StylePriority.UsePadding = False
        Me.XrTableCell3.StylePriority.UseTextAlignment = False
        Me.XrTableCell3.Text = "XrTableCell3"
        Me.XrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        Me.XrTableCell3.Weight = 0.23372861698951536R
        '
        'XrTableCell4
        '
        Me.XrTableCell4.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTable.BarcodeInfo")})
        Me.XrTableCell4.Name = "XrTableCell4"
        Me.XrTableCell4.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell4.StylePriority.UsePadding = False
        Me.XrTableCell4.StylePriority.UseTextAlignment = False
        Me.XrTableCell4.Text = "XrTableCell4"
        Me.XrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        Me.XrTableCell4.Weight = 0.27232904031013505R
        '
        'XrTableCell1
        '
        Me.XrTableCell1.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTable.LotNumber")})
        Me.XrTableCell1.Name = "XrTableCell1"
        Me.XrTableCell1.StylePriority.UseTextAlignment = False
        Me.XrTableCell1.Text = "XrTableCell1"
        Me.XrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        Me.XrTableCell1.Weight = 0.10709030858894109R
        '
        'XrTableCell5
        '
        Me.XrTableCell5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTable.ExpDateFormated")})
        Me.XrTableCell5.Name = "XrTableCell5"
        Me.XrTableCell5.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell5.StylePriority.UsePadding = False
        Me.XrTableCell5.StylePriority.UseTextAlignment = False
        Me.XrTableCell5.Text = "XrTableCell5"
        Me.XrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        Me.XrTableCell5.Weight = 0.12493857558961578R
        '
        'XrTableCell6
        '
        Me.XrTableCell6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTable.BottleCode")})
        Me.XrTableCell6.Name = "XrTableCell6"
        Me.XrTableCell6.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell6.StylePriority.UsePadding = False
        Me.XrTableCell6.StylePriority.UseTextAlignment = False
        Me.XrTableCell6.Text = "XrTableCell6"
        Me.XrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        Me.XrTableCell6.Weight = 0.079680106572170714R
        '
        'XrTableCell7
        '
        Me.XrTableCell7.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTable.RealVolume")})
        Me.XrTableCell7.Name = "XrTableCell7"
        Me.XrTableCell7.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell7.StylePriority.UsePadding = False
        Me.XrTableCell7.StylePriority.UseTextAlignment = False
        Me.XrTableCell7.Text = "XrTableCell7"
        Me.XrTableCell7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        Me.XrTableCell7.Weight = 0.11558947802557962R
        '
        'XrTableCell8
        '
        Me.XrTableCell8.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTable.RemainingTests")})
        Me.XrTableCell8.Name = "XrTableCell8"
        Me.XrTableCell8.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell8.StylePriority.UsePadding = False
        Me.XrTableCell8.StylePriority.UseTextAlignment = False
        Me.XrTableCell8.Text = "XrTableCell8"
        Me.XrTableCell8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        Me.XrTableCell8.Weight = 0.0863379780379438R
        '
        'XrTableCell9
        '
        Me.XrTableCell9.CanShrink = True
        Me.XrTableCell9.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTable.Status")})
        Me.XrTableCell9.Name = "XrTableCell9"
        Me.XrTableCell9.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell9.StylePriority.UsePadding = False
        Me.XrTableCell9.StylePriority.UseTextAlignment = False
        Me.XrTableCell9.Text = "XrTableCell9"
        Me.XrTableCell9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        Me.XrTableCell9.Weight = 0.1769965726447344R
        '
        'RotorContentPositionsReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport})
        Me.Font = New System.Drawing.Font("Times New Roman", 9.75!, System.Drawing.FontStyle.Italic)
        Me.FormattingRuleSheet.AddRange(New DevExpress.XtraReports.UI.FormattingRule() {Me.Few, Me.Depleted})
        Me.Margins = New System.Drawing.Printing.Margins(38, 48, 59, 33)
        Me.PageHeight = 1169
        Me.PageWidth = 827
        Me.PaperKind = System.Drawing.Printing.PaperKind.A4
        Me.Version = "13.1"
        CType(Me.XrTable1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CellPositionInformationDS1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Protected WithEvents XrHeaderLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel2 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelRemainingTests As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelBottle As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelVolume As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelExpDate As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCell As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelBarcode As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelStatus As DevExpress.XtraReports.UI.XRLabel
    Public WithEvents XrWSStartDateTimeLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPageInfo1 As DevExpress.XtraReports.UI.XRPageInfo
    Friend WithEvents XrTableCell9 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell8 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell7 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell6 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell5 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell4 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell3 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell2 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableRow1 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTable1 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents Detail1 As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents DetailReport As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents CellPositionInformationDS1 As Biosystems.Ax00.Types.CellPositionInformationDS
    Friend WithEvents Few As DevExpress.XtraReports.UI.FormattingRule
    Friend WithEvents Depleted As DevExpress.XtraReports.UI.FormattingRule
    Friend WithEvents XrLabel1 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTableCell1 As DevExpress.XtraReports.UI.XRTableCell
End Class
