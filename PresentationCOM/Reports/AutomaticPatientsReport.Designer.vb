<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class AutomaticPatientsReport
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
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand
        Me.XrPageInfo1 = New DevExpress.XtraReports.UI.XRPageInfo
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel
        Me.XrLabelTest = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelRemarks = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelUnit = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelRefranges = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelConc = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelType = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelPatientID = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabelDate = New DevExpress.XtraReports.UI.XRLabel
        Me.XrWSStartDateTimeLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.XrHeaderLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.ResultsDS1 = New Biosystems.Ax00.Types.ResultsDS
        Me.XrTableCell9 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell8 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell7 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell6 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell5 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell4 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell3 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableCell2 = New DevExpress.XtraReports.UI.XRTableCell
        Me.XrTableRow1 = New DevExpress.XtraReports.UI.XRTableRow
        Me.XrTable1 = New DevExpress.XtraReports.UI.XRTable
        Me.Detail1 = New DevExpress.XtraReports.UI.DetailBand
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand
        CType(Me.ResultsDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.XrTable1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.HeightF = 1.041667!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'TopMargin
        '
        Me.TopMargin.Name = "TopMargin"
        Me.TopMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'BottomMargin
        '
        Me.BottomMargin.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPageInfo1})
        Me.BottomMargin.HeightF = 45.41664!
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrPageInfo1
        '
        Me.XrPageInfo1.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold)
        Me.XrPageInfo1.LocationFloat = New DevExpress.Utils.PointFloat(667.0!, 10.00001!)
        Me.XrPageInfo1.Name = "XrPageInfo1"
        Me.XrPageInfo1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrPageInfo1.SizeF = New System.Drawing.SizeF(100.0!, 23.0!)
        Me.XrPageInfo1.StylePriority.UseFont = False
        '
        'GroupHeader1
        '
        Me.GroupHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel2, Me.XrWSStartDateTimeLabel, Me.XrHeaderLabel})
        Me.GroupHeader1.HeightF = 68.75001!
        Me.GroupHeader1.Name = "GroupHeader1"
        '
        'XrPanel2
        '
        Me.XrPanel2.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel2.BorderWidth = 2
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelTest, Me.XrLabelRemarks, Me.XrLabelUnit, Me.XrLabelRefranges, Me.XrLabelConc, Me.XrLabelType, Me.XrLabelPatientID, Me.XrLabelDate})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 45.62499!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(766.9999!, 23.12502!)
        Me.XrPanel2.StylePriority.UseBorders = False
        Me.XrPanel2.StylePriority.UseBorderWidth = False
        '
        'XrLabelTest
        '
        Me.XrLabelTest.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelTest.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelTest.LocationFloat = New DevExpress.Utils.PointFloat(161.0417!, 0.0!)
        Me.XrLabelTest.Name = "XrLabelTest"
        Me.XrLabelTest.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelTest.SizeF = New System.Drawing.SizeF(96.20839!, 20.00001!)
        Me.XrLabelTest.StylePriority.UseBorders = False
        Me.XrLabelTest.StylePriority.UseFont = False
        Me.XrLabelTest.Text = "Test"
        '
        'XrLabelRemarks
        '
        Me.XrLabelRemarks.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelRemarks.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelRemarks.LocationFloat = New DevExpress.Utils.PointFloat(510.0835!, 0.0!)
        Me.XrLabelRemarks.Name = "XrLabelRemarks"
        Me.XrLabelRemarks.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 5, 0, 0, 100.0!)
        Me.XrLabelRemarks.SizeF = New System.Drawing.SizeF(127.5527!, 20.00002!)
        Me.XrLabelRemarks.StylePriority.UseBorders = False
        Me.XrLabelRemarks.StylePriority.UseFont = False
        Me.XrLabelRemarks.StylePriority.UsePadding = False
        Me.XrLabelRemarks.StylePriority.UseTextAlignment = False
        Me.XrLabelRemarks.Text = "Remarks"
        Me.XrLabelRemarks.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelUnit
        '
        Me.XrLabelUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelUnit.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelUnit.LocationFloat = New DevExpress.Utils.PointFloat(334.375!, 0.0!)
        Me.XrLabelUnit.Name = "XrLabelUnit"
        Me.XrLabelUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelUnit.SizeF = New System.Drawing.SizeF(59.91663!, 20.00001!)
        Me.XrLabelUnit.StylePriority.UseBorders = False
        Me.XrLabelUnit.StylePriority.UseFont = False
        Me.XrLabelUnit.StylePriority.UseTextAlignment = False
        Me.XrLabelUnit.Text = "Unit"
        Me.XrLabelUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelRefranges
        '
        Me.XrLabelRefranges.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelRefranges.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelRefranges.LocationFloat = New DevExpress.Utils.PointFloat(394.2916!, 0.0!)
        Me.XrLabelRefranges.Name = "XrLabelRefranges"
        Me.XrLabelRefranges.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 0, 0, 0, 100.0!)
        Me.XrLabelRefranges.SizeF = New System.Drawing.SizeF(115.7919!, 20.0!)
        Me.XrLabelRefranges.StylePriority.UseBorders = False
        Me.XrLabelRefranges.StylePriority.UseFont = False
        Me.XrLabelRefranges.StylePriority.UsePadding = False
        Me.XrLabelRefranges.StylePriority.UseTextAlignment = False
        Me.XrLabelRefranges.Text = "Ref. Ranges"
        Me.XrLabelRefranges.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelConc
        '
        Me.XrLabelConc.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelConc.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelConc.LocationFloat = New DevExpress.Utils.PointFloat(257.2501!, 0.0!)
        Me.XrLabelConc.Name = "XrLabelConc"
        Me.XrLabelConc.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelConc.SizeF = New System.Drawing.SizeF(77.12491!, 20.00001!)
        Me.XrLabelConc.StylePriority.UseBorders = False
        Me.XrLabelConc.StylePriority.UseFont = False
        Me.XrLabelConc.StylePriority.UseTextAlignment = False
        Me.XrLabelConc.Text = "Conc."
        Me.XrLabelConc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabelType
        '
        Me.XrLabelType.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelType.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelType.LocationFloat = New DevExpress.Utils.PointFloat(99.16668!, 0.0!)
        Me.XrLabelType.Name = "XrLabelType"
        Me.XrLabelType.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelType.SizeF = New System.Drawing.SizeF(61.87501!, 20.00002!)
        Me.XrLabelType.StylePriority.UseBorders = False
        Me.XrLabelType.StylePriority.UseFont = False
        Me.XrLabelType.Text = "Type"
        '
        'XrLabelPatientID
        '
        Me.XrLabelPatientID.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelPatientID.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelPatientID.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrLabelPatientID.Name = "XrLabelPatientID"
        Me.XrLabelPatientID.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelPatientID.SizeF = New System.Drawing.SizeF(99.16666!, 20.00002!)
        Me.XrLabelPatientID.StylePriority.UseBorders = False
        Me.XrLabelPatientID.StylePriority.UseFont = False
        Me.XrLabelPatientID.Text = "PatientID"
        '
        'XrLabelDate
        '
        Me.XrLabelDate.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelDate.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelDate.LocationFloat = New DevExpress.Utils.PointFloat(637.6362!, 0.0!)
        Me.XrLabelDate.Name = "XrLabelDate"
        Me.XrLabelDate.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 5, 0, 0, 100.0!)
        Me.XrLabelDate.SizeF = New System.Drawing.SizeF(119.3638!, 20.00002!)
        Me.XrLabelDate.StylePriority.UseBorders = False
        Me.XrLabelDate.StylePriority.UseFont = False
        Me.XrLabelDate.StylePriority.UsePadding = False
        Me.XrLabelDate.StylePriority.UseTextAlignment = False
        Me.XrLabelDate.Text = "Date"
        Me.XrLabelDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrWSStartDateTimeLabel
        '
        Me.XrWSStartDateTimeLabel.BackColor = System.Drawing.Color.LightGray
        Me.XrWSStartDateTimeLabel.BorderColor = System.Drawing.Color.Empty
        Me.XrWSStartDateTimeLabel.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrWSStartDateTimeLabel.Font = New System.Drawing.Font("Verdana", 11.25!, System.Drawing.FontStyle.Bold)
        Me.XrWSStartDateTimeLabel.LocationFloat = New DevExpress.Utils.PointFloat(334.375!, 10.00001!)
        Me.XrWSStartDateTimeLabel.Multiline = True
        Me.XrWSStartDateTimeLabel.Name = "XrWSStartDateTimeLabel"
        Me.XrWSStartDateTimeLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(10, 2, 4, 2, 100.0!)
        Me.XrWSStartDateTimeLabel.SizeF = New System.Drawing.SizeF(432.6249!, 25.0!)
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
        Me.XrHeaderLabel.Font = New System.Drawing.Font("Verdana", 11.25!, System.Drawing.FontStyle.Bold)
        Me.XrHeaderLabel.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 10.00001!)
        Me.XrHeaderLabel.Multiline = True
        Me.XrHeaderLabel.Name = "XrHeaderLabel"
        Me.XrHeaderLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(10, 2, 4, 2, 100.0!)
        Me.XrHeaderLabel.SizeF = New System.Drawing.SizeF(334.375!, 25.0!)
        Me.XrHeaderLabel.StylePriority.UseBackColor = False
        Me.XrHeaderLabel.StylePriority.UseBorderColor = False
        Me.XrHeaderLabel.StylePriority.UseBorders = False
        Me.XrHeaderLabel.StylePriority.UseFont = False
        Me.XrHeaderLabel.StylePriority.UsePadding = False
        Me.XrHeaderLabel.StylePriority.UseTextAlignment = False
        Me.XrHeaderLabel.Text = "XrHeaderLabel"
        Me.XrHeaderLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'ResultsDS1
        '
        Me.ResultsDS1.DataSetName = "ResultsDS"
        Me.ResultsDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'XrTableCell9
        '
        Me.XrTableCell9.CanShrink = True
        Me.XrTableCell9.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleDetails.ResultDate")})
        Me.XrTableCell9.Name = "XrTableCell9"
        Me.XrTableCell9.StylePriority.UseTextAlignment = False
        Me.XrTableCell9.Text = "XrTableCell9"
        Me.XrTableCell9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell9.Weight = 0.21989839373563633
        '
        'XrTableCell8
        '
        Me.XrTableCell8.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleDetails.Remarks")})
        Me.XrTableCell8.Name = "XrTableCell8"
        Me.XrTableCell8.Text = "XrTableCell8"
        Me.XrTableCell8.Weight = 0.2168198060973088
        '
        'XrTableCell7
        '
        Me.XrTableCell7.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleDetails.ReferenceRanges")})
        Me.XrTableCell7.Name = "XrTableCell7"
        Me.XrTableCell7.Text = "XrTableCell7"
        Me.XrTableCell7.Weight = 0.19682816209612486
        '
        'XrTableCell6
        '
        Me.XrTableCell6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleDetails.Unit")})
        Me.XrTableCell6.Name = "XrTableCell6"
        Me.XrTableCell6.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 0, 2, 0, 100.0!)
        Me.XrTableCell6.StylePriority.UsePadding = False
        Me.XrTableCell6.StylePriority.UseTextAlignment = False
        Me.XrTableCell6.Text = "XrTableCell6"
        Me.XrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell6.Weight = 0.10184902241228111
        '
        'XrTableCell5
        '
        Me.XrTableCell5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleDetails.CONC_Value")})
        Me.XrTableCell5.Name = "XrTableCell5"
        Me.XrTableCell5.Padding = New DevExpress.XtraPrinting.PaddingInfo(5, 2, 2, 0, 100.0!)
        Me.XrTableCell5.StylePriority.UsePadding = False
        Me.XrTableCell5.StylePriority.UseTextAlignment = False
        Me.XrTableCell5.Text = "XrTableCell5"
        Me.XrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrTableCell5.Weight = 0.1311003282084916
        '
        'XrTableCell4
        '
        Me.XrTableCell4.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleDetails.TestName")})
        Me.XrTableCell4.Name = "XrTableCell4"
        Me.XrTableCell4.Text = "XrTableCell4"
        Me.XrTableCell4.Weight = 0.16353937473277244
        '
        'XrTableCell3
        '
        Me.XrTableCell3.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleDetails.SampleType")})
        Me.XrTableCell3.Name = "XrTableCell3"
        Me.XrTableCell3.Text = "XrTableCell3"
        Me.XrTableCell3.Weight = 0.1051779283830683
        '
        'XrTableCell2
        '
        Me.XrTableCell2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleDetails.PatientID")})
        Me.XrTableCell2.Name = "XrTableCell2"
        Me.XrTableCell2.Text = "XrTableCell2"
        Me.XrTableCell2.Weight = 0.16856794913223055
        '
        'XrTableRow1
        '
        Me.XrTableRow1.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell2, Me.XrTableCell3, Me.XrTableCell4, Me.XrTableCell5, Me.XrTableCell6, Me.XrTableCell7, Me.XrTableCell8, Me.XrTableCell9})
        Me.XrTableRow1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrTableRow1.Name = "XrTableRow1"
        Me.XrTableRow1.StylePriority.UseFont = False
        Me.XrTableRow1.Weight = 11.5
        '
        'XrTable1
        '
        Me.XrTable1.Font = New System.Drawing.Font("Times New Roman", 9.75!)
        Me.XrTable1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrTable1.Name = "XrTable1"
        Me.XrTable1.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow1})
        Me.XrTable1.SizeF = New System.Drawing.SizeF(767.0!, 25.0!)
        Me.XrTable1.StylePriority.UseFont = False
        '
        'Detail1
        '
        Me.Detail1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTable1})
        Me.Detail1.HeightF = 25.0!
        Me.Detail1.Name = "Detail1"
        '
        'DetailReport
        '
        Me.DetailReport.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail1})
        Me.DetailReport.DataMember = "ReportSampleDetails"
        Me.DetailReport.DataSource = Me.ResultsDS1
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
        '
        'AutomaticPatientsReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport})
        Me.DataMember = "ReportSampleDetails"
        Me.DataSource = Me.ResultsDS1
        Me.Font = New System.Drawing.Font("Times New Roman", 9.75!, System.Drawing.FontStyle.Italic)
        Me.Margins = New System.Drawing.Printing.Margins(41, 42, 100, 45)
        Me.Version = "10.2"
        CType(Me.ResultsDS1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.XrTable1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents ResultsDS1 As Biosystems.Ax00.Types.ResultsDS
    Protected WithEvents XrHeaderLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel2 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelRemarks As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelRefranges As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelConc As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelType As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelPatientID As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelTest As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelDate As DevExpress.XtraReports.UI.XRLabel
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
End Class
