<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class ResultsByTestReportCompact
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
        Me.XrPanel1 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrLabel2 = New DevExpress.XtraReports.UI.XRLabel
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrLabelUnit = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelDate = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelFactor = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelConc = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelAbs = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelNumber = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelType = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelName = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelClass = New DevExpress.XtraReports.UI.XRLabel
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
        Me.XrTableCell3 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell4 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell5 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell6 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell1 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell7 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell8 = New DevExpress.XtraReports.UI.XRTableCell
        Me.ResultsDS1 = New Biosystems.Ax00.Types.ResultsDS
        CType(Me.XrTableDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ResultsDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel1, Me.XrPanel2})
        Me.Detail.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail.HeightF = 56.45835!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.StylePriority.UseFont = False
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrPanel1
        '
        Me.XrPanel1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel1.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel1.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel2})
        Me.XrPanel1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrPanel1.Name = "XrPanel1"
        Me.XrPanel1.SizeF = New System.Drawing.SizeF(650.0!, 30.0!)
        Me.XrPanel1.StylePriority.UseBackColor = False
        Me.XrPanel1.StylePriority.UseBorderColor = False
        Me.XrPanel1.StylePriority.UseBorders = False
        '
        'XrLabel2
        '
        Me.XrLabel2.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTestMaster.TestName")})
        Me.XrLabel2.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel2.LocationFloat = New DevExpress.Utils.PointFloat(3.0!, 6.0!)
        Me.XrLabel2.Name = "XrLabel2"
        Me.XrLabel2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel2.SizeF = New System.Drawing.SizeF(630.0!, 20.0!)
        Me.XrLabel2.StylePriority.UseBorders = False
        Me.XrLabel2.StylePriority.UseFont = False
        Me.XrLabel2.Text = "XrLabel2"
        '
        'XrPanel2
        '
        Me.XrPanel2.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel2.BorderWidth = 2
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelUnit, Me.XrLabelDate, Me.XrLabelFactor, Me.XrLabelConc, Me.XrLabelAbs, Me.XrLabelNumber, Me.XrLabelType, Me.XrLabelName, Me.XrLabelClass})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(3.0!, 30.0!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(644.0!, 23.0!)
        Me.XrPanel2.StylePriority.UseBorders = False
        Me.XrPanel2.StylePriority.UseBorderWidth = False
        '
        'XrLabelUnit
        '
        Me.XrLabelUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelUnit.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelUnit.LocationFloat = New DevExpress.Utils.PointFloat(397.0002!, 0.0!)
        Me.XrLabelUnit.Name = "XrLabelUnit"
        Me.XrLabelUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelUnit.SizeF = New System.Drawing.SizeF(55.0!, 20.0!)
        Me.XrLabelUnit.StylePriority.UseBorders = False
        Me.XrLabelUnit.StylePriority.UseFont = False
        Me.XrLabelUnit.StylePriority.UsePadding = False
        Me.XrLabelUnit.StylePriority.UseTextAlignment = False
        Me.XrLabelUnit.Text = "Unidad"
        Me.XrLabelUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelDate
        '
        Me.XrLabelDate.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelDate.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelDate.LocationFloat = New DevExpress.Utils.PointFloat(520.0!, 0.0!)
        Me.XrLabelDate.Name = "XrLabelDate"
        Me.XrLabelDate.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 2, 0, 0, 100.0!)
        Me.XrLabelDate.SizeF = New System.Drawing.SizeF(123.0001!, 20.00002!)
        Me.XrLabelDate.StylePriority.UseBorders = False
        Me.XrLabelDate.StylePriority.UseFont = False
        Me.XrLabelDate.StylePriority.UsePadding = False
        Me.XrLabelDate.StylePriority.UseTextAlignment = False
        Me.XrLabelDate.Text = "Date"
        Me.XrLabelDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelFactor
        '
        Me.XrLabelFactor.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelFactor.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelFactor.LocationFloat = New DevExpress.Utils.PointFloat(452.0001!, 0.0!)
        Me.XrLabelFactor.Name = "XrLabelFactor"
        Me.XrLabelFactor.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 3, 0, 0, 100.0!)
        Me.XrLabelFactor.SizeF = New System.Drawing.SizeF(68.0!, 20.00002!)
        Me.XrLabelFactor.StylePriority.UseBorders = False
        Me.XrLabelFactor.StylePriority.UseFont = False
        Me.XrLabelFactor.StylePriority.UsePadding = False
        Me.XrLabelFactor.StylePriority.UseTextAlignment = False
        Me.XrLabelFactor.Text = "Factor"
        Me.XrLabelFactor.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelConc
        '
        Me.XrLabelConc.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelConc.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelConc.LocationFloat = New DevExpress.Utils.PointFloat(330.0002!, 0.0!)
        Me.XrLabelConc.Name = "XrLabelConc"
        Me.XrLabelConc.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 3, 0, 0, 100.0!)
        Me.XrLabelConc.SizeF = New System.Drawing.SizeF(66.0!, 20.0!)
        Me.XrLabelConc.StylePriority.UseBorders = False
        Me.XrLabelConc.StylePriority.UseFont = False
        Me.XrLabelConc.StylePriority.UsePadding = False
        Me.XrLabelConc.StylePriority.UseTextAlignment = False
        Me.XrLabelConc.Text = "Conc."
        Me.XrLabelConc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelAbs
        '
        Me.XrLabelAbs.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelAbs.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelAbs.LocationFloat = New DevExpress.Utils.PointFloat(264.0002!, 0.0!)
        Me.XrLabelAbs.Name = "XrLabelAbs"
        Me.XrLabelAbs.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 3, 0, 0, 100.0!)
        Me.XrLabelAbs.SizeF = New System.Drawing.SizeF(66.0!, 20.0!)
        Me.XrLabelAbs.StylePriority.UseBorders = False
        Me.XrLabelAbs.StylePriority.UseFont = False
        Me.XrLabelAbs.StylePriority.UsePadding = False
        Me.XrLabelAbs.StylePriority.UseTextAlignment = False
        Me.XrLabelAbs.Text = "Abs."
        Me.XrLabelAbs.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelNumber
        '
        Me.XrLabelNumber.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelNumber.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelNumber.LocationFloat = New DevExpress.Utils.PointFloat(236.0002!, 0.0!)
        Me.XrLabelNumber.Name = "XrLabelNumber"
        Me.XrLabelNumber.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelNumber.SizeF = New System.Drawing.SizeF(28.0!, 20.0!)
        Me.XrLabelNumber.StylePriority.UseBorders = False
        Me.XrLabelNumber.StylePriority.UseFont = False
        Me.XrLabelNumber.StylePriority.UseTextAlignment = False
        Me.XrLabelNumber.Text = "No."
        Me.XrLabelNumber.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelType
        '
        Me.XrLabelType.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelType.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelType.LocationFloat = New DevExpress.Utils.PointFloat(196.0002!, 0.0!)
        Me.XrLabelType.Name = "XrLabelType"
        Me.XrLabelType.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelType.SizeF = New System.Drawing.SizeF(40.0!, 20.00002!)
        Me.XrLabelType.StylePriority.UseBorders = False
        Me.XrLabelType.StylePriority.UseFont = False
        Me.XrLabelType.Text = "Type"
        '
        'XrLabelName
        '
        Me.XrLabelName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelName.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelName.LocationFloat = New DevExpress.Utils.PointFloat(70.00021!, 0.0!)
        Me.XrLabelName.Name = "XrLabelName"
        Me.XrLabelName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelName.SizeF = New System.Drawing.SizeF(126.0!, 20.0!)
        Me.XrLabelName.StylePriority.UseBorders = False
        Me.XrLabelName.StylePriority.UseFont = False
        Me.XrLabelName.Text = "Name"
        '
        'XrLabelClass
        '
        Me.XrLabelClass.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelClass.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelClass.LocationFloat = New DevExpress.Utils.PointFloat(0.0001907349!, 0.0!)
        Me.XrLabelClass.Name = "XrLabelClass"
        Me.XrLabelClass.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelClass.SizeF = New System.Drawing.SizeF(70.0!, 20.0!)
        Me.XrLabelClass.StylePriority.UseBorders = False
        Me.XrLabelClass.StylePriority.UseFont = False
        Me.XrLabelClass.Text = "Class"
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
        Me.XrWSStartDateTimeLabel.LocationFloat = New DevExpress.Utils.PointFloat(16.0!, 35.0!)
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
        Me.DetailReport.DataMember = "ReportTestMaster.ReportTestMaster_ReportTestDetails"
        Me.DetailReport.DataSource = Me.ResultsDS1
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
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
        Me.XrTableDetails.LocationFloat = New DevExpress.Utils.PointFloat(3.0!, 0.0!)
        Me.XrTableDetails.Name = "XrTableDetails"
        Me.XrTableDetails.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableDetailsRow})
        Me.XrTableDetails.SizeF = New System.Drawing.SizeF(643.0!, 18.0!)
        Me.XrTableDetails.StylePriority.UseFont = False
        '
        'XrTableDetailsRow
        '
        Me.XrTableDetailsRow.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCellClass, Me.XrTableCell2, Me.XrTableCell3, Me.XrTableCell4, Me.XrTableCell5, Me.XrTableCell6, Me.XrTableCell1, Me.XrTableCell7, Me.XrTableCell8})
        Me.XrTableDetailsRow.Name = "XrTableDetailsRow"
        Me.XrTableDetailsRow.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 3, 2, 0, 100.0!)
        Me.XrTableDetailsRow.StylePriority.UsePadding = False
        Me.XrTableDetailsRow.Weight = 1
        '
        'XrTableCellClass
        '
        Me.XrTableCellClass.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTestMaster.ReportTestMaster_ReportTestDetails.SampleClass")})
        Me.XrTableCellClass.Name = "XrTableCellClass"
        Me.XrTableCellClass.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 0, 2, 0, 100.0!)
        Me.XrTableCellClass.StylePriority.UsePadding = False
        Me.XrTableCellClass.Text = "XrTableCellClass"
        Me.XrTableCellClass.Weight = 0.33333329700288339
        '
        'XrTableCell2
        '
        Me.XrTableCell2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTestMaster.ReportTestMaster_ReportTestDetails.Name")})
        Me.XrTableCell2.Name = "XrTableCell2"
        Me.XrTableCell2.Text = "XrTableCell2"
        Me.XrTableCell2.Weight = 0.60000014532180046
        '
        'XrTableCell3
        '
        Me.XrTableCell3.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTestMaster.ReportTestMaster_ReportTestDetails.SampleType")})
        Me.XrTableCell3.Name = "XrTableCell3"
        Me.XrTableCell3.StylePriority.UseTextAlignment = False
        Me.XrTableCell3.Text = "XrTableCell3"
        Me.XrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell3.Weight = 0.19047611781529028
        '
        'XrTableCell4
        '
        Me.XrTableCell4.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTestMaster.ReportTestMaster_ReportTestDetails.ReplicateNumber")})
        Me.XrTableCell4.Name = "XrTableCell4"
        Me.XrTableCell4.StylePriority.UseTextAlignment = False
        Me.XrTableCell4.Text = "XrTableCell4"
        Me.XrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell4.Weight = 0.13333333333333336
        '
        'XrTableCell5
        '
        Me.XrTableCell5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTestMaster.ReportTestMaster_ReportTestDetails.ABSValue")})
        Me.XrTableCell5.Name = "XrTableCell5"
        Me.XrTableCell5.StylePriority.UseTextAlignment = False
        Me.XrTableCell5.Text = "XrTableCell5"
        Me.XrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell5.Weight = 0.31428572336832661
        '
        'XrTableCell6
        '
        Me.XrTableCell6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTestMaster.ReportTestMaster_ReportTestDetails.CONC_Value")})
        Me.XrTableCell6.Name = "XrTableCell6"
        Me.XrTableCell6.StylePriority.UseTextAlignment = False
        Me.XrTableCell6.Text = "XrTableCell6"
        Me.XrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell6.Weight = 0.31428571201506111
        '
        'XrTableCell1
        '
        Me.XrTableCell1.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTestMaster.ReportTestMaster_ReportTestDetails.MeasureUnit")})
        Me.XrTableCell1.Name = "XrTableCell1"
        Me.XrTableCell1.Text = "XrTableCell1"
        Me.XrTableCell1.Weight = 0.26666680971781409
        '
        'XrTableCell7
        '
        Me.XrTableCell7.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTestMaster.ReportTestMaster_ReportTestDetails.CalibratorFactor")})
        Me.XrTableCell7.Name = "XrTableCell7"
        Me.XrTableCell7.StylePriority.UseTextAlignment = False
        Me.XrTableCell7.Text = "XrTableCell7"
        Me.XrTableCell7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell7.Weight = 0.32380923089526947
        '
        'XrTableCell8
        '
        Me.XrTableCell8.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportTestMaster.ReportTestMaster_ReportTestDetails.ResultDate")})
        Me.XrTableCell8.Name = "XrTableCell8"
        Me.XrTableCell8.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 2, 0, 100.0!)
        Me.XrTableCell8.StylePriority.UsePadding = False
        Me.XrTableCell8.StylePriority.UseTextAlignment = False
        Me.XrTableCell8.Text = "XrTableCell8"
        Me.XrTableCell8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell8.Weight = 0.585714392434983
        '
        'ResultsDS1
        '
        Me.ResultsDS1.DataSetName = "ResultsDS"
        Me.ResultsDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'ResultsByTestReportCompact
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport})
        Me.DataMember = "ReportTestMaster"
        Me.DataSource = Me.ResultsDS1
        Me.Margins = New System.Drawing.Printing.Margins(100, 100, 96, 100)
        Me.StyleSheet.AddRange(New DevExpress.XtraReports.UI.XRControlStyle() {Me.XrControlStyle1})
        Me.Version = "10.2"
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
    Friend WithEvents XrLabelName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelNumber As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelType As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelDate As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelFactor As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelClass As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelConc As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelAbs As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel1 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents ResultsDS1 As Biosystems.Ax00.Types.ResultsDS
    Friend WithEvents XrLabel2 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTableDetails As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableDetailsRow As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCellClass As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell2 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell3 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell4 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell5 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell6 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell7 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell8 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrWSStartDateTimeLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTableCell1 As DevExpress.XtraReports.UI.XRTableCell
End Class
