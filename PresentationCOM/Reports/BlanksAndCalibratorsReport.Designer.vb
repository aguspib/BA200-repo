<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class BlanksAndCalibratorsReport
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
        Me.XrWSStartDateTimeLabel = New DevExpress.XtraReports.UI.XRLabel()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.DetailBlanks = New DevExpress.XtraReports.UI.DetailBand()
        Me.XrTable1 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow1 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell3 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell5 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell6 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell8 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell2 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell1 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell4 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell9 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell10 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.DetailReportBlanks = New DevExpress.XtraReports.UI.DetailReportBand()
        Me.GroupHeaderBlanks = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrPanel1 = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrLabelBlankAbs = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelReagentAbs = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelInitialAbs = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelBlankLimit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelBlankRemarks = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelBlankAbsLimit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelBlankTest = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelMainFilterAbs = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelBlankDate = New DevExpress.XtraReports.UI.XRLabel()
        Me.reportsDS = New Biosystems.Ax00.Types.ReportsDS()
        Me.DetailReportCalibrators = New DevExpress.XtraReports.UI.DetailReportBand()
        Me.DetailCalibrators = New DevExpress.XtraReports.UI.DetailBand()
        Me.XrTable2 = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow2 = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell7 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell11 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell12 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell14 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell15 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell16 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell17 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell19 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell20 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell21 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell18 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.GroupHeaderCalibrators = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrLabelFactorLimit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelFactor = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelUnit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelCalibratorAbs = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelMultipointNumber = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelSampleType = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelTheoricalConc = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelCalibratorRemarks = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelCalibratorTest = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelCalibratorName = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelCalibratorDate = New DevExpress.XtraReports.UI.XRLabel()
        CType(Me.XrTable1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.reportsDS, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTable2, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.TopMargin.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrWSStartDateTimeLabel})
        Me.TopMargin.HeightF = 86.0!
        Me.TopMargin.Name = "TopMargin"
        Me.TopMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrWSStartDateTimeLabel
        '
        Me.XrWSStartDateTimeLabel.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrWSStartDateTimeLabel.CanGrow = False
        Me.XrWSStartDateTimeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrWSStartDateTimeLabel.LocationFloat = New DevExpress.Utils.PointFloat(841.92!, 61.0!)
        Me.XrWSStartDateTimeLabel.Name = "XrWSStartDateTimeLabel"
        Me.XrWSStartDateTimeLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrWSStartDateTimeLabel.SizeF = New System.Drawing.SizeF(227.08!, 15.0!)
        Me.XrWSStartDateTimeLabel.StylePriority.UseBorders = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseFont = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseTextAlignment = False
        Me.XrWSStartDateTimeLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
        '
        'BottomMargin
        '
        Me.BottomMargin.HeightF = 25.0!
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'DetailBlanks
        '
        Me.DetailBlanks.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTable1})
        Me.DetailBlanks.HeightF = 25.0!
        Me.DetailBlanks.Name = "DetailBlanks"
        '
        'XrTable1
        '
        Me.XrTable1.Font = New System.Drawing.Font("Times New Roman", 9.75!)
        Me.XrTable1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrTable1.Name = "XrTable1"
        Me.XrTable1.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow1})
        Me.XrTable1.SizeF = New System.Drawing.SizeF(1069.0!, 25.0!)
        Me.XrTable1.StylePriority.UseFont = False
        '
        'XrTableRow1
        '
        Me.XrTableRow1.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell3, Me.XrTableCell5, Me.XrTableCell6, Me.XrTableCell8, Me.XrTableCell2, Me.XrTableCell1, Me.XrTableCell4, Me.XrTableCell9, Me.XrTableCell10})
        Me.XrTableRow1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrTableRow1.Name = "XrTableRow1"
        Me.XrTableRow1.StylePriority.UseFont = False
        Me.XrTableRow1.Weight = 11.5R
        '
        'XrTableCell3
        '
        Me.XrTableCell3.CanGrow = False
        Me.XrTableCell3.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "BlanksResultsDetails.TestName")})
        Me.XrTableCell3.Name = "XrTableCell3"
        Me.XrTableCell3.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell3.StylePriority.UsePadding = False
        Me.XrTableCell3.StylePriority.UseTextAlignment = False
        Me.XrTableCell3.Text = "ALBUMIN"
        Me.XrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell3.Weight = 0.24321940494816768R
        '
        'XrTableCell5
        '
        Me.XrTableCell5.CanGrow = False
        Me.XrTableCell5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "BlanksResultsDetails.Absorbance")})
        Me.XrTableCell5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCell5.Name = "XrTableCell5"
        Me.XrTableCell5.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell5.StylePriority.UseFont = False
        Me.XrTableCell5.StylePriority.UsePadding = False
        Me.XrTableCell5.StylePriority.UseTextAlignment = False
        Me.XrTableCell5.Text = "-0.0042"
        Me.XrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell5.Weight = 0.17168437648463289R
        Me.XrTableCell5.WordWrap = False
        '
        'XrTableCell6
        '
        Me.XrTableCell6.CanGrow = False
        Me.XrTableCell6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "BlanksResultsDetails.ReagentAbsorbance")})
        Me.XrTableCell6.Name = "XrTableCell6"
        Me.XrTableCell6.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell6.StylePriority.UsePadding = False
        Me.XrTableCell6.StylePriority.UseTextAlignment = False
        Me.XrTableCell6.Text = "-0.0042"
        Me.XrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell6.Weight = 0.17168432502671863R
        '
        'XrTableCell8
        '
        Me.XrTableCell8.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "BlanksResultsDetails.KineticBlankLimit")})
        Me.XrTableCell8.Name = "XrTableCell8"
        Me.XrTableCell8.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell8.StylePriority.UsePadding = False
        Me.XrTableCell8.StylePriority.UseTextAlignment = False
        Me.XrTableCell8.Text = "0,3000"
        Me.XrTableCell8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell8.Weight = 0.17168411798244415R
        '
        'XrTableCell2
        '
        Me.XrTableCell2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "BlanksResultsDetails.InitialAbsorbance")})
        Me.XrTableCell2.Name = "XrTableCell2"
        Me.XrTableCell2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell2.StylePriority.UsePadding = False
        Me.XrTableCell2.StylePriority.UseTextAlignment = False
        Me.XrTableCell2.Text = "-0.0042"
        Me.XrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell2.Weight = 0.1716843125142426R
        '
        'XrTableCell1
        '
        Me.XrTableCell1.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "BlanksResultsDetails.MainFilterAbsorbance")})
        Me.XrTableCell1.Name = "XrTableCell1"
        Me.XrTableCell1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell1.StylePriority.UsePadding = False
        Me.XrTableCell1.StylePriority.UseTextAlignment = False
        Me.XrTableCell1.Text = "-0.0042"
        Me.XrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell1.Weight = 0.17168452001482756R
        '
        'XrTableCell4
        '
        Me.XrTableCell4.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "BlanksResultsDetails.BlankAbsorbanceLimit")})
        Me.XrTableCell4.Name = "XrTableCell4"
        Me.XrTableCell4.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell4.StylePriority.UsePadding = False
        Me.XrTableCell4.StylePriority.UseTextAlignment = False
        Me.XrTableCell4.Text = "0,3000"
        Me.XrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell4.Weight = 0.17848369742850742R
        '
        'XrTableCell9
        '
        Me.XrTableCell9.CanShrink = True
        Me.XrTableCell9.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "BlanksResultsDetails.Remarks")})
        Me.XrTableCell9.Font = New System.Drawing.Font("Verdana", 7.0!)
        Me.XrTableCell9.Name = "XrTableCell9"
        Me.XrTableCell9.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell9.StylePriority.UseFont = False
        Me.XrTableCell9.StylePriority.UsePadding = False
        Me.XrTableCell9.StylePriority.UseTextAlignment = False
        Me.XrTableCell9.Text = "Sample Abs < Blank Abs, Conc. not calculated"
        Me.XrTableCell9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell9.Weight = 0.33472779864246865R
        '
        'XrTableCell10
        '
        Me.XrTableCell10.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "BlanksResultsDetails.ResultDate")})
        Me.XrTableCell10.Name = "XrTableCell10"
        Me.XrTableCell10.StylePriority.UseTextAlignment = False
        Me.XrTableCell10.Text = "26/09/2014 12:30"
        Me.XrTableCell10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell10.Weight = 0.20228152975086738R
        '
        'DetailReportBlanks
        '
        Me.DetailReportBlanks.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.DetailBlanks, Me.GroupHeaderBlanks})
        Me.DetailReportBlanks.DataMember = "BlanksResultsDetails"
        Me.DetailReportBlanks.DataSource = Me.reportsDS
        Me.DetailReportBlanks.Level = 0
        Me.DetailReportBlanks.Name = "DetailReportBlanks"
        Me.DetailReportBlanks.PrintOnEmptyDataSource = False
        '
        'GroupHeaderBlanks
        '
        Me.GroupHeaderBlanks.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel1})
        Me.GroupHeaderBlanks.HeightF = 27.0!
        Me.GroupHeaderBlanks.Name = "GroupHeaderBlanks"
        Me.GroupHeaderBlanks.RepeatEveryPage = True
        '
        'XrPanel1
        '
        Me.XrPanel1.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel1.BorderWidth = 1
        Me.XrPanel1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelBlankAbs, Me.XrLabelReagentAbs, Me.XrLabelInitialAbs, Me.XrLabelBlankLimit, Me.XrLabelBlankRemarks, Me.XrLabelBlankAbsLimit, Me.XrLabelBlankTest, Me.XrLabelMainFilterAbs, Me.XrLabelBlankDate})
        Me.XrPanel1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrPanel1.Name = "XrPanel1"
        Me.XrPanel1.SizeF = New System.Drawing.SizeF(1069.0!, 21.5!)
        Me.XrPanel1.StylePriority.UseBorders = False
        Me.XrPanel1.StylePriority.UseBorderWidth = False
        '
        'XrLabelBlankAbs
        '
        Me.XrLabelBlankAbs.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelBlankAbs.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelBlankAbs.LocationFloat = New DevExpress.Utils.PointFloat(143.0833!, 1.499931!)
        Me.XrLabelBlankAbs.Name = "XrLabelBlankAbs"
        Me.XrLabelBlankAbs.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelBlankAbs.SizeF = New System.Drawing.SizeF(101.0!, 20.0!)
        Me.XrLabelBlankAbs.StylePriority.UseBorders = False
        Me.XrLabelBlankAbs.StylePriority.UseFont = False
        Me.XrLabelBlankAbs.StylePriority.UsePadding = False
        Me.XrLabelBlankAbs.StylePriority.UseTextAlignment = False
        Me.XrLabelBlankAbs.Text = "Abs."
        Me.XrLabelBlankAbs.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelReagentAbs
        '
        Me.XrLabelReagentAbs.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelReagentAbs.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelReagentAbs.LocationFloat = New DevExpress.Utils.PointFloat(244.0833!, 1.499931!)
        Me.XrLabelReagentAbs.Name = "XrLabelReagentAbs"
        Me.XrLabelReagentAbs.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelReagentAbs.SizeF = New System.Drawing.SizeF(101.0!, 20.0!)
        Me.XrLabelReagentAbs.StylePriority.UseBorders = False
        Me.XrLabelReagentAbs.StylePriority.UseFont = False
        Me.XrLabelReagentAbs.StylePriority.UsePadding = False
        Me.XrLabelReagentAbs.StylePriority.UseTextAlignment = False
        Me.XrLabelReagentAbs.Text = "Abs. Reactivo"
        Me.XrLabelReagentAbs.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelInitialAbs
        '
        Me.XrLabelInitialAbs.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelInitialAbs.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelInitialAbs.LocationFloat = New DevExpress.Utils.PointFloat(446.0833!, 1.499931!)
        Me.XrLabelInitialAbs.Name = "XrLabelInitialAbs"
        Me.XrLabelInitialAbs.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelInitialAbs.SizeF = New System.Drawing.SizeF(101.0!, 20.0!)
        Me.XrLabelInitialAbs.StylePriority.UseBorders = False
        Me.XrLabelInitialAbs.StylePriority.UseFont = False
        Me.XrLabelInitialAbs.StylePriority.UsePadding = False
        Me.XrLabelInitialAbs.StylePriority.UseTextAlignment = False
        Me.XrLabelInitialAbs.Text = "Abs. Inicial"
        Me.XrLabelInitialAbs.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelBlankLimit
        '
        Me.XrLabelBlankLimit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelBlankLimit.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelBlankLimit.LocationFloat = New DevExpress.Utils.PointFloat(345.0833!, 1.499931!)
        Me.XrLabelBlankLimit.Name = "XrLabelBlankLimit"
        Me.XrLabelBlankLimit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelBlankLimit.SizeF = New System.Drawing.SizeF(101.0!, 20.0!)
        Me.XrLabelBlankLimit.StylePriority.UseBorders = False
        Me.XrLabelBlankLimit.StylePriority.UseFont = False
        Me.XrLabelBlankLimit.StylePriority.UsePadding = False
        Me.XrLabelBlankLimit.StylePriority.UseTextAlignment = False
        Me.XrLabelBlankLimit.Text = "Límite Blanco Cinético"
        Me.XrLabelBlankLimit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelBlankRemarks
        '
        Me.XrLabelBlankRemarks.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelBlankRemarks.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelBlankRemarks.LocationFloat = New DevExpress.Utils.PointFloat(753.0834!, 1.499939!)
        Me.XrLabelBlankRemarks.Name = "XrLabelBlankRemarks"
        Me.XrLabelBlankRemarks.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelBlankRemarks.SizeF = New System.Drawing.SizeF(196.9166!, 20.0!)
        Me.XrLabelBlankRemarks.StylePriority.UseBorders = False
        Me.XrLabelBlankRemarks.StylePriority.UseFont = False
        Me.XrLabelBlankRemarks.StylePriority.UseTextAlignment = False
        Me.XrLabelBlankRemarks.Text = "Observaciones"
        Me.XrLabelBlankRemarks.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelBlankAbsLimit
        '
        Me.XrLabelBlankAbsLimit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelBlankAbsLimit.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelBlankAbsLimit.LocationFloat = New DevExpress.Utils.PointFloat(648.0833!, 1.499962!)
        Me.XrLabelBlankAbsLimit.Name = "XrLabelBlankAbsLimit"
        Me.XrLabelBlankAbsLimit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelBlankAbsLimit.SizeF = New System.Drawing.SizeF(101.0!, 20.0!)
        Me.XrLabelBlankAbsLimit.StylePriority.UseBorders = False
        Me.XrLabelBlankAbsLimit.StylePriority.UseFont = False
        Me.XrLabelBlankAbsLimit.StylePriority.UsePadding = False
        Me.XrLabelBlankAbsLimit.StylePriority.UseTextAlignment = False
        Me.XrLabelBlankAbsLimit.Text = "Límite Abs. Blanco"
        Me.XrLabelBlankAbsLimit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelBlankTest
        '
        Me.XrLabelBlankTest.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelBlankTest.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelBlankTest.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 1.500017!)
        Me.XrLabelBlankTest.Name = "XrLabelBlankTest"
        Me.XrLabelBlankTest.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelBlankTest.SizeF = New System.Drawing.SizeF(143.0833!, 20.00001!)
        Me.XrLabelBlankTest.StylePriority.UseBorders = False
        Me.XrLabelBlankTest.StylePriority.UseFont = False
        Me.XrLabelBlankTest.StylePriority.UsePadding = False
        Me.XrLabelBlankTest.StylePriority.UseTextAlignment = False
        Me.XrLabelBlankTest.Text = "Técnica"
        Me.XrLabelBlankTest.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelMainFilterAbs
        '
        Me.XrLabelMainFilterAbs.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelMainFilterAbs.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelMainFilterAbs.LocationFloat = New DevExpress.Utils.PointFloat(547.0833!, 1.499987!)
        Me.XrLabelMainFilterAbs.Name = "XrLabelMainFilterAbs"
        Me.XrLabelMainFilterAbs.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelMainFilterAbs.SizeF = New System.Drawing.SizeF(101.0!, 20.0!)
        Me.XrLabelMainFilterAbs.StylePriority.UseBorders = False
        Me.XrLabelMainFilterAbs.StylePriority.UseFont = False
        Me.XrLabelMainFilterAbs.StylePriority.UsePadding = False
        Me.XrLabelMainFilterAbs.StylePriority.UseTextAlignment = False
        Me.XrLabelMainFilterAbs.Text = "Abs. Filtro Principal"
        Me.XrLabelMainFilterAbs.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelBlankDate
        '
        Me.XrLabelBlankDate.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelBlankDate.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelBlankDate.LocationFloat = New DevExpress.Utils.PointFloat(950.0!, 1.500027!)
        Me.XrLabelBlankDate.Name = "XrLabelBlankDate"
        Me.XrLabelBlankDate.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelBlankDate.SizeF = New System.Drawing.SizeF(119.0!, 20.0!)
        Me.XrLabelBlankDate.StylePriority.UseBorders = False
        Me.XrLabelBlankDate.StylePriority.UseFont = False
        Me.XrLabelBlankDate.StylePriority.UseTextAlignment = False
        Me.XrLabelBlankDate.Text = "Fecha"
        Me.XrLabelBlankDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'reportsDS
        '
        Me.reportsDS.DataSetName = "ReportsDS"
        Me.reportsDS.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'DetailReportCalibrators
        '
        Me.DetailReportCalibrators.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.DetailCalibrators, Me.GroupHeaderCalibrators})
        Me.DetailReportCalibrators.DataMember = "CalibratorsResultsDetails"
        Me.DetailReportCalibrators.DataSource = Me.reportsDS
        Me.DetailReportCalibrators.Level = 1
        Me.DetailReportCalibrators.Name = "DetailReportCalibrators"
        Me.DetailReportCalibrators.PrintOnEmptyDataSource = False
        '
        'DetailCalibrators
        '
        Me.DetailCalibrators.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTable2})
        Me.DetailCalibrators.HeightF = 25.0!
        Me.DetailCalibrators.Name = "DetailCalibrators"
        '
        'XrTable2
        '
        Me.XrTable2.Font = New System.Drawing.Font("Times New Roman", 9.75!)
        Me.XrTable2.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrTable2.Name = "XrTable2"
        Me.XrTable2.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow2})
        Me.XrTable2.SizeF = New System.Drawing.SizeF(1069.0!, 25.0!)
        Me.XrTable2.StylePriority.UseFont = False
        '
        'XrTableRow2
        '
        Me.XrTableRow2.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell7, Me.XrTableCell11, Me.XrTableCell12, Me.XrTableCell14, Me.XrTableCell15, Me.XrTableCell16, Me.XrTableCell17, Me.XrTableCell19, Me.XrTableCell20, Me.XrTableCell21, Me.XrTableCell18})
        Me.XrTableRow2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrTableRow2.Name = "XrTableRow2"
        Me.XrTableRow2.StylePriority.UseFont = False
        Me.XrTableRow2.Weight = 11.5R
        '
        'XrTableCell7
        '
        Me.XrTableCell7.CanGrow = False
        Me.XrTableCell7.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.TestName")})
        Me.XrTableCell7.Name = "XrTableCell7"
        Me.XrTableCell7.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell7.StylePriority.UsePadding = False
        Me.XrTableCell7.StylePriority.UseTextAlignment = False
        Me.XrTableCell7.Text = "ALBUMIN"
        Me.XrTableCell7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell7.Weight = 0.24321940494816768R
        '
        'XrTableCell11
        '
        Me.XrTableCell11.CanGrow = False
        Me.XrTableCell11.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.SampleType")})
        Me.XrTableCell11.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCell11.Name = "XrTableCell11"
        Me.XrTableCell11.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell11.StylePriority.UseFont = False
        Me.XrTableCell11.StylePriority.UsePadding = False
        Me.XrTableCell11.StylePriority.UseTextAlignment = False
        Me.XrTableCell11.Text = "SER"
        Me.XrTableCell11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell11.Weight = 0.064594076767375413R
        Me.XrTableCell11.WordWrap = False
        '
        'XrTableCell12
        '
        Me.XrTableCell12.CanGrow = False
        Me.XrTableCell12.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.Name")})
        Me.XrTableCell12.Name = "XrTableCell12"
        Me.XrTableCell12.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell12.StylePriority.UsePadding = False
        Me.XrTableCell12.StylePriority.UseTextAlignment = False
        Me.XrTableCell12.Text = "CALIB ASO (7889ABC)"
        Me.XrTableCell12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell12.Weight = 0.33996914101114756R
        '
        'XrTableCell14
        '
        Me.XrTableCell14.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.MultipointNumber")})
        Me.XrTableCell14.Name = "XrTableCell14"
        Me.XrTableCell14.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell14.StylePriority.UsePadding = False
        Me.XrTableCell14.StylePriority.UseTextAlignment = False
        Me.XrTableCell14.Text = "1"
        Me.XrTableCell14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell14.Weight = 0.044195862178814944R
        '
        'XrTableCell15
        '
        Me.XrTableCell15.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.Absorbance")})
        Me.XrTableCell15.Name = "XrTableCell15"
        Me.XrTableCell15.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell15.StylePriority.UsePadding = False
        Me.XrTableCell15.StylePriority.UseTextAlignment = False
        Me.XrTableCell15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell15.Weight = 0.1189891385479262R
        '
        'XrTableCell16
        '
        Me.XrTableCell16.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.TheoreticalConcentration")})
        Me.XrTableCell16.Name = "XrTableCell16"
        Me.XrTableCell16.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell16.StylePriority.UsePadding = False
        Me.XrTableCell16.StylePriority.UseTextAlignment = False
        Me.XrTableCell16.Text = "XrTableCell16"
        Me.XrTableCell16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell16.Weight = 0.11898918806330357R
        '
        'XrTableCell17
        '
        Me.XrTableCell17.CanShrink = True
        Me.XrTableCell17.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.Unit")})
        Me.XrTableCell17.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrTableCell17.Name = "XrTableCell17"
        Me.XrTableCell17.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell17.StylePriority.UseFont = False
        Me.XrTableCell17.StylePriority.UsePadding = False
        Me.XrTableCell17.StylePriority.UseTextAlignment = False
        Me.XrTableCell17.Text = "IU/mL"
        Me.XrTableCell17.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell17.Weight = 0.11396046274826055R
        '
        'XrTableCell19
        '
        Me.XrTableCell19.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.CalibratorFactor")})
        Me.XrTableCell19.Name = "XrTableCell19"
        Me.XrTableCell19.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell19.StylePriority.UsePadding = False
        Me.XrTableCell19.StylePriority.UseTextAlignment = False
        Me.XrTableCell19.Text = "1111,9999"
        Me.XrTableCell19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell19.Weight = 0.11721842895095132R
        '
        'XrTableCell20
        '
        Me.XrTableCell20.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.FactorLimit")})
        Me.XrTableCell20.Name = "XrTableCell20"
        Me.XrTableCell20.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell20.StylePriority.UsePadding = False
        Me.XrTableCell20.StylePriority.UseTextAlignment = False
        Me.XrTableCell20.Text = "1"
        Me.XrTableCell20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell20.Weight = 0.11898913532117172R
        '
        'XrTableCell21
        '
        Me.XrTableCell21.CanShrink = True
        Me.XrTableCell21.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.Remarks")})
        Me.XrTableCell21.Font = New System.Drawing.Font("Verdana", 7.0!)
        Me.XrTableCell21.Name = "XrTableCell21"
        Me.XrTableCell21.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell21.StylePriority.UseFont = False
        Me.XrTableCell21.StylePriority.UsePadding = False
        Me.XrTableCell21.StylePriority.UseTextAlignment = False
        Me.XrTableCell21.Text = "Sample Abs < Blank Abs, Conc. not calculated"
        Me.XrTableCell21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell21.Weight = 0.3347277270245746R
        '
        'XrTableCell18
        '
        Me.XrTableCell18.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CalibratorsResultsDetails.ResultDate")})
        Me.XrTableCell18.Name = "XrTableCell18"
        Me.XrTableCell18.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell18.StylePriority.UsePadding = False
        Me.XrTableCell18.StylePriority.UseTextAlignment = False
        Me.XrTableCell18.Text = "26/09/2014 12:30"
        Me.XrTableCell18.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell18.Weight = 0.20228151723118348R
        '
        'GroupHeaderCalibrators
        '
        Me.GroupHeaderCalibrators.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel2})
        Me.GroupHeaderCalibrators.HeightF = 37.8334!
        Me.GroupHeaderCalibrators.Name = "GroupHeaderCalibrators"
        Me.GroupHeaderCalibrators.RepeatEveryPage = True
        '
        'XrPanel2
        '
        Me.XrPanel2.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel2.BorderWidth = 1
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelFactorLimit, Me.XrLabelFactor, Me.XrLabelUnit, Me.XrLabelCalibratorAbs, Me.XrLabelMultipointNumber, Me.XrLabelSampleType, Me.XrLabelTheoricalConc, Me.XrLabelCalibratorRemarks, Me.XrLabelCalibratorTest, Me.XrLabelCalibratorName, Me.XrLabelCalibratorDate})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 10.0!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(1069.0!, 21.5!)
        Me.XrPanel2.StylePriority.UseBorders = False
        Me.XrPanel2.StylePriority.UseBorderWidth = False
        '
        'XrLabelFactorLimit
        '
        Me.XrLabelFactorLimit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelFactorLimit.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelFactorLimit.LocationFloat = New DevExpress.Utils.PointFloat(683.0833!, 1.499922!)
        Me.XrLabelFactorLimit.Name = "XrLabelFactorLimit"
        Me.XrLabelFactorLimit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelFactorLimit.SizeF = New System.Drawing.SizeF(70.0!, 20.0!)
        Me.XrLabelFactorLimit.StylePriority.UseBorders = False
        Me.XrLabelFactorLimit.StylePriority.UseFont = False
        Me.XrLabelFactorLimit.StylePriority.UsePadding = False
        Me.XrLabelFactorLimit.StylePriority.UseTextAlignment = False
        Me.XrLabelFactorLimit.Text = "Límites de Factor"
        Me.XrLabelFactorLimit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelFactor
        '
        Me.XrLabelFactor.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelFactor.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelFactor.LocationFloat = New DevExpress.Utils.PointFloat(613.0833!, 1.499907!)
        Me.XrLabelFactor.Name = "XrLabelFactor"
        Me.XrLabelFactor.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelFactor.SizeF = New System.Drawing.SizeF(70.0!, 20.0!)
        Me.XrLabelFactor.StylePriority.UseBorders = False
        Me.XrLabelFactor.StylePriority.UseFont = False
        Me.XrLabelFactor.StylePriority.UsePadding = False
        Me.XrLabelFactor.StylePriority.UseTextAlignment = False
        Me.XrLabelFactor.Text = "Factor"
        Me.XrLabelFactor.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelUnit
        '
        Me.XrLabelUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelUnit.CanGrow = False
        Me.XrLabelUnit.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelUnit.LocationFloat = New DevExpress.Utils.PointFloat(547.0833!, 1.499922!)
        Me.XrLabelUnit.Name = "XrLabelUnit"
        Me.XrLabelUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelUnit.SizeF = New System.Drawing.SizeF(67.04175!, 20.00001!)
        Me.XrLabelUnit.StylePriority.UseBorders = False
        Me.XrLabelUnit.StylePriority.UseFont = False
        Me.XrLabelUnit.StylePriority.UsePadding = False
        Me.XrLabelUnit.StylePriority.UseTextAlignment = False
        Me.XrLabelUnit.Text = "Unidad"
        Me.XrLabelUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelCalibratorAbs
        '
        Me.XrLabelCalibratorAbs.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalibratorAbs.CanGrow = False
        Me.XrLabelCalibratorAbs.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCalibratorAbs.LocationFloat = New DevExpress.Utils.PointFloat(407.0833!, 1.499931!)
        Me.XrLabelCalibratorAbs.Name = "XrLabelCalibratorAbs"
        Me.XrLabelCalibratorAbs.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCalibratorAbs.SizeF = New System.Drawing.SizeF(70.0!, 20.0!)
        Me.XrLabelCalibratorAbs.StylePriority.UseBorders = False
        Me.XrLabelCalibratorAbs.StylePriority.UseFont = False
        Me.XrLabelCalibratorAbs.StylePriority.UsePadding = False
        Me.XrLabelCalibratorAbs.StylePriority.UseTextAlignment = False
        Me.XrLabelCalibratorAbs.Text = "Abs."
        Me.XrLabelCalibratorAbs.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelMultipointNumber
        '
        Me.XrLabelMultipointNumber.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelMultipointNumber.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelMultipointNumber.LocationFloat = New DevExpress.Utils.PointFloat(381.0833!, 1.499946!)
        Me.XrLabelMultipointNumber.Name = "XrLabelMultipointNumber"
        Me.XrLabelMultipointNumber.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelMultipointNumber.SizeF = New System.Drawing.SizeF(25.99997!, 20.0!)
        Me.XrLabelMultipointNumber.StylePriority.UseBorders = False
        Me.XrLabelMultipointNumber.StylePriority.UseFont = False
        Me.XrLabelMultipointNumber.StylePriority.UsePadding = False
        Me.XrLabelMultipointNumber.StylePriority.UseTextAlignment = False
        Me.XrLabelMultipointNumber.Text = "Nº Kit"
        Me.XrLabelMultipointNumber.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelSampleType
        '
        Me.XrLabelSampleType.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelSampleType.CanGrow = False
        Me.XrLabelSampleType.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelSampleType.LocationFloat = New DevExpress.Utils.PointFloat(143.0833!, 1.499931!)
        Me.XrLabelSampleType.Name = "XrLabelSampleType"
        Me.XrLabelSampleType.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelSampleType.SizeF = New System.Drawing.SizeF(38.0!, 20.0!)
        Me.XrLabelSampleType.StylePriority.UseBorders = False
        Me.XrLabelSampleType.StylePriority.UseFont = False
        Me.XrLabelSampleType.StylePriority.UsePadding = False
        Me.XrLabelSampleType.StylePriority.UseTextAlignment = False
        Me.XrLabelSampleType.Text = "Tipo"
        Me.XrLabelSampleType.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelTheoricalConc
        '
        Me.XrLabelTheoricalConc.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelTheoricalConc.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelTheoricalConc.LocationFloat = New DevExpress.Utils.PointFloat(477.0833!, 1.499931!)
        Me.XrLabelTheoricalConc.Name = "XrLabelTheoricalConc"
        Me.XrLabelTheoricalConc.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelTheoricalConc.SizeF = New System.Drawing.SizeF(70.0!, 20.0!)
        Me.XrLabelTheoricalConc.StylePriority.UseBorders = False
        Me.XrLabelTheoricalConc.StylePriority.UseFont = False
        Me.XrLabelTheoricalConc.StylePriority.UsePadding = False
        Me.XrLabelTheoricalConc.StylePriority.UseTextAlignment = False
        Me.XrLabelTheoricalConc.Text = "Conc. Teórica"
        Me.XrLabelTheoricalConc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelCalibratorRemarks
        '
        Me.XrLabelCalibratorRemarks.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalibratorRemarks.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCalibratorRemarks.LocationFloat = New DevExpress.Utils.PointFloat(753.0833!, 1.499939!)
        Me.XrLabelCalibratorRemarks.Name = "XrLabelCalibratorRemarks"
        Me.XrLabelCalibratorRemarks.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCalibratorRemarks.SizeF = New System.Drawing.SizeF(196.9167!, 20.0!)
        Me.XrLabelCalibratorRemarks.StylePriority.UseBorders = False
        Me.XrLabelCalibratorRemarks.StylePriority.UseFont = False
        Me.XrLabelCalibratorRemarks.StylePriority.UsePadding = False
        Me.XrLabelCalibratorRemarks.StylePriority.UseTextAlignment = False
        Me.XrLabelCalibratorRemarks.Text = "Observaciones"
        Me.XrLabelCalibratorRemarks.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelCalibratorTest
        '
        Me.XrLabelCalibratorTest.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalibratorTest.CanGrow = False
        Me.XrLabelCalibratorTest.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCalibratorTest.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 1.500017!)
        Me.XrLabelCalibratorTest.Name = "XrLabelCalibratorTest"
        Me.XrLabelCalibratorTest.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCalibratorTest.SizeF = New System.Drawing.SizeF(143.0833!, 20.00001!)
        Me.XrLabelCalibratorTest.StylePriority.UseBorders = False
        Me.XrLabelCalibratorTest.StylePriority.UseFont = False
        Me.XrLabelCalibratorTest.StylePriority.UsePadding = False
        Me.XrLabelCalibratorTest.StylePriority.UseTextAlignment = False
        Me.XrLabelCalibratorTest.Text = "Técnica"
        Me.XrLabelCalibratorTest.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelCalibratorName
        '
        Me.XrLabelCalibratorName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalibratorName.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCalibratorName.LocationFloat = New DevExpress.Utils.PointFloat(181.0833!, 1.500003!)
        Me.XrLabelCalibratorName.Name = "XrLabelCalibratorName"
        Me.XrLabelCalibratorName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCalibratorName.SizeF = New System.Drawing.SizeF(200.0!, 20.0!)
        Me.XrLabelCalibratorName.StylePriority.UseBorders = False
        Me.XrLabelCalibratorName.StylePriority.UseFont = False
        Me.XrLabelCalibratorName.StylePriority.UsePadding = False
        Me.XrLabelCalibratorName.StylePriority.UseTextAlignment = False
        Me.XrLabelCalibratorName.Text = "Nombre (Lote)"
        Me.XrLabelCalibratorName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelCalibratorDate
        '
        Me.XrLabelCalibratorDate.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelCalibratorDate.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelCalibratorDate.LocationFloat = New DevExpress.Utils.PointFloat(950.0!, 1.5!)
        Me.XrLabelCalibratorDate.Name = "XrLabelCalibratorDate"
        Me.XrLabelCalibratorDate.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelCalibratorDate.SizeF = New System.Drawing.SizeF(119.0!, 20.0!)
        Me.XrLabelCalibratorDate.StylePriority.UseBorders = False
        Me.XrLabelCalibratorDate.StylePriority.UseFont = False
        Me.XrLabelCalibratorDate.StylePriority.UsePadding = False
        Me.XrLabelCalibratorDate.StylePriority.UseTextAlignment = False
        Me.XrLabelCalibratorDate.Text = "Fecha"
        Me.XrLabelCalibratorDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'BlanksAndCalibratorsReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.DetailReportBlanks, Me.DetailReportCalibrators})
        Me.DataSource = Me.reportsDS
        Me.Font = New System.Drawing.Font("Times New Roman", 9.75!, System.Drawing.FontStyle.Italic)
        Me.Landscape = True
        Me.Margins = New System.Drawing.Printing.Margins(50, 50, 86, 25)
        Me.PageHeight = 827
        Me.PageWidth = 1169
        Me.PaperKind = System.Drawing.Printing.PaperKind.A4
        Me.Version = "10.2"
        CType(Me.XrTable1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.reportsDS, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTable2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents DetailBlanks As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents DetailReportBlanks As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents GroupHeaderBlanks As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents DetailReportCalibrators As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents DetailCalibrators As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents GroupHeaderCalibrators As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents XrPanel2 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelCalibratorAbs As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelMultipointNumber As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelSampleType As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelTheoricalConc As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCalibratorRemarks As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCalibratorTest As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCalibratorName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelCalibratorDate As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelFactorLimit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelFactor As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTable2 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow2 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell7 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell11 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell12 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell14 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell15 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell16 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell17 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell19 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell20 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell21 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell18 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents reportsDS As Biosystems.Ax00.Types.ReportsDS
    Friend WithEvents XrTable1 As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow1 As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell3 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell5 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell6 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell8 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell2 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell1 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell4 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell9 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell10 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrPanel1 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelBlankAbs As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelReagentAbs As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelInitialAbs As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelBlankLimit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelBlankRemarks As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelBlankAbsLimit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelBlankTest As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelMainFilterAbs As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelBlankDate As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrWSStartDateTimeLabel As DevExpress.XtraReports.UI.XRLabel
End Class
