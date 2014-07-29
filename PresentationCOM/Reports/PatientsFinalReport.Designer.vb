<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class PatientsFinalReport
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
        Me.XrPanel1 = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrLabel_Gender = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel_Age = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel_DateBirth = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel_PatientID = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel_PatientName = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel8 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel5 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel4 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel3 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel2 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrLabelFlags = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelUnit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelRefranges = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelConc = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelType = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelTest = New DevExpress.XtraReports.UI.XRLabel()
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.XrPageInfo_DateTime = New DevExpress.XtraReports.UI.XRPageInfo()
        Me.XrPageInfo_TotalPages = New DevExpress.XtraReports.UI.XRPageInfo()
        Me.XrLabel_ReportDate = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrControlStyle1 = New DevExpress.XtraReports.UI.XRControlStyle()
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrLabel_DateTime_Test = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrWSStartDateTimeLabel = New DevExpress.XtraReports.UI.XRLabel()
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand()
        Me.Detail1 = New DevExpress.XtraReports.UI.DetailBand()
        Me.XrTableDetails = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableDetailsRow = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCellTestName = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell2 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell5 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell6 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell1 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell7 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.ResultsDS1 = New Biosystems.Ax00.Types.ResultsDS()
        Me.XrLabel_PerformedBy = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel1 = New DevExpress.XtraReports.UI.XRLabel()
        CType(Me.XrTableDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ResultsDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel1, Me.XrPanel2})
        Me.Detail.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail.HeightF = 194.7915!
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
        Me.XrPanel1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel1, Me.XrLabel_PerformedBy, Me.XrLabel_Gender, Me.XrLabel_Age, Me.XrLabel_DateBirth, Me.XrLabel_PatientID, Me.XrLabel_PatientName, Me.XrLabel8, Me.XrLabel5, Me.XrLabel4, Me.XrLabel3, Me.XrLabel2})
        Me.XrPanel1.LocationFloat = New DevExpress.Utils.PointFloat(5.000003!, 0.0!)
        Me.XrPanel1.Name = "XrPanel1"
        Me.XrPanel1.SizeF = New System.Drawing.SizeF(673.0!, 165.0!)
        Me.XrPanel1.StylePriority.UseBackColor = False
        Me.XrPanel1.StylePriority.UseBorderColor = False
        Me.XrPanel1.StylePriority.UseBorders = False
        '
        'XrLabel_Gender
        '
        Me.XrLabel_Gender.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel_Gender.Font = New System.Drawing.Font("Verdana", 9.0!)
        Me.XrLabel_Gender.LocationFloat = New DevExpress.Utils.PointFloat(9.999993!, 115.0!)
        Me.XrLabel_Gender.Name = "XrLabel_Gender"
        Me.XrLabel_Gender.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel_Gender.SizeF = New System.Drawing.SizeF(203.75!, 20.00001!)
        Me.XrLabel_Gender.StylePriority.UseBorders = False
        Me.XrLabel_Gender.StylePriority.UseFont = False
        Me.XrLabel_Gender.StylePriority.UseTextAlignment = False
        Me.XrLabel_Gender.Text = "*Gender:"
        Me.XrLabel_Gender.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabel_Age
        '
        Me.XrLabel_Age.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel_Age.Font = New System.Drawing.Font("Verdana", 9.0!)
        Me.XrLabel_Age.LocationFloat = New DevExpress.Utils.PointFloat(9.999978!, 89.99996!)
        Me.XrLabel_Age.Name = "XrLabel_Age"
        Me.XrLabel_Age.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel_Age.SizeF = New System.Drawing.SizeF(203.75!, 20.00002!)
        Me.XrLabel_Age.StylePriority.UseBorders = False
        Me.XrLabel_Age.StylePriority.UseFont = False
        Me.XrLabel_Age.StylePriority.UseTextAlignment = False
        Me.XrLabel_Age.Text = "*Age:"
        Me.XrLabel_Age.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabel_DateBirth
        '
        Me.XrLabel_DateBirth.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel_DateBirth.Font = New System.Drawing.Font("Verdana", 9.0!)
        Me.XrLabel_DateBirth.LocationFloat = New DevExpress.Utils.PointFloat(9.999993!, 64.99999!)
        Me.XrLabel_DateBirth.Name = "XrLabel_DateBirth"
        Me.XrLabel_DateBirth.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel_DateBirth.SizeF = New System.Drawing.SizeF(203.75!, 20.00002!)
        Me.XrLabel_DateBirth.StylePriority.UseBorders = False
        Me.XrLabel_DateBirth.StylePriority.UseFont = False
        Me.XrLabel_DateBirth.StylePriority.UseTextAlignment = False
        Me.XrLabel_DateBirth.Text = "*Date of birth:"
        Me.XrLabel_DateBirth.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabel_PatientID
        '
        Me.XrLabel_PatientID.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel_PatientID.Font = New System.Drawing.Font("Verdana", 9.0!)
        Me.XrLabel_PatientID.LocationFloat = New DevExpress.Utils.PointFloat(9.999993!, 37.99998!)
        Me.XrLabel_PatientID.Name = "XrLabel_PatientID"
        Me.XrLabel_PatientID.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel_PatientID.SizeF = New System.Drawing.SizeF(203.75!, 20.00002!)
        Me.XrLabel_PatientID.StylePriority.UseBorders = False
        Me.XrLabel_PatientID.StylePriority.UseFont = False
        Me.XrLabel_PatientID.StylePriority.UseTextAlignment = False
        Me.XrLabel_PatientID.Text = "*Patient ID:"
        Me.XrLabel_PatientID.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabel_PatientName
        '
        Me.XrLabel_PatientName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel_PatientName.Font = New System.Drawing.Font("Verdana", 9.0!)
        Me.XrLabel_PatientName.LocationFloat = New DevExpress.Utils.PointFloat(9.999993!, 10.00001!)
        Me.XrLabel_PatientName.Name = "XrLabel_PatientName"
        Me.XrLabel_PatientName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel_PatientName.SizeF = New System.Drawing.SizeF(203.75!, 20.00002!)
        Me.XrLabel_PatientName.StylePriority.UseBorders = False
        Me.XrLabel_PatientName.StylePriority.UseFont = False
        Me.XrLabel_PatientName.StylePriority.UseTextAlignment = False
        Me.XrLabel_PatientName.Text = "*Patient Name:"
        Me.XrLabel_PatientName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabel8
        '
        Me.XrLabel8.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel8.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.FullName")})
        Me.XrLabel8.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel8.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 10.00001!)
        Me.XrLabel8.Name = "XrLabel8"
        Me.XrLabel8.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel8.SizeF = New System.Drawing.SizeF(454.25!, 20.00001!)
        Me.XrLabel8.StylePriority.UseBorders = False
        Me.XrLabel8.StylePriority.UseFont = False
        Me.XrLabel8.StylePriority.UsePadding = False
        Me.XrLabel8.Text = "XrLabel8"
        '
        'XrLabel5
        '
        Me.XrLabel5.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.AgeWithUnit")})
        Me.XrLabel5.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel5.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 89.99996!)
        Me.XrLabel5.Name = "XrLabel5"
        Me.XrLabel5.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel5.SizeF = New System.Drawing.SizeF(454.25!, 20.00002!)
        Me.XrLabel5.StylePriority.UseBorders = False
        Me.XrLabel5.StylePriority.UseFont = False
        Me.XrLabel5.StylePriority.UsePadding = False
        Me.XrLabel5.Text = "XrLabel5"
        '
        'XrLabel4
        '
        Me.XrLabel4.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel4.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.FormatedDateOfBirth")})
        Me.XrLabel4.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel4.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 64.99999!)
        Me.XrLabel4.Name = "XrLabel4"
        Me.XrLabel4.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel4.SizeF = New System.Drawing.SizeF(454.25!, 20.00002!)
        Me.XrLabel4.StylePriority.UseBorders = False
        Me.XrLabel4.StylePriority.UseFont = False
        Me.XrLabel4.StylePriority.UsePadding = False
        Me.XrLabel4.Text = "XrLabel4"
        '
        'XrLabel3
        '
        Me.XrLabel3.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel3.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.Gender")})
        Me.XrLabel3.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel3.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 115.0!)
        Me.XrLabel3.Name = "XrLabel3"
        Me.XrLabel3.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel3.SizeF = New System.Drawing.SizeF(454.25!, 20.00001!)
        Me.XrLabel3.StylePriority.UseBorders = False
        Me.XrLabel3.StylePriority.UseFont = False
        Me.XrLabel3.StylePriority.UsePadding = False
        Me.XrLabel3.Text = "XrLabel3"
        '
        'XrLabel2
        '
        Me.XrLabel2.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.FullID")})
        Me.XrLabel2.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrLabel2.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 37.99998!)
        Me.XrLabel2.Name = "XrLabel2"
        Me.XrLabel2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel2.SizeF = New System.Drawing.SizeF(454.25!, 20.00001!)
        Me.XrLabel2.StylePriority.UseBorders = False
        Me.XrLabel2.StylePriority.UseFont = False
        Me.XrLabel2.StylePriority.UsePadding = False
        Me.XrLabel2.Text = "XrLabel2"
        '
        'XrPanel2
        '
        Me.XrPanel2.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel2.BorderWidth = 1
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelFlags, Me.XrLabelUnit, Me.XrLabelRefranges, Me.XrLabelConc, Me.XrLabelType, Me.XrLabelTest})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(5.000003!, 171.1249!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(668.0!, 22.0!)
        Me.XrPanel2.StylePriority.UseBorders = False
        Me.XrPanel2.StylePriority.UseBorderWidth = False
        '
        'XrLabelFlags
        '
        Me.XrLabelFlags.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelFlags.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelFlags.LocationFloat = New DevExpress.Utils.PointFloat(448.0833!, 0.0!)
        Me.XrLabelFlags.Name = "XrLabelFlags"
        Me.XrLabelFlags.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 5, 0, 0, 100.0!)
        Me.XrLabelFlags.SizeF = New System.Drawing.SizeF(56.3331!, 20.00002!)
        Me.XrLabelFlags.StylePriority.UseBorders = False
        Me.XrLabelFlags.StylePriority.UseFont = False
        Me.XrLabelFlags.StylePriority.UsePadding = False
        Me.XrLabelFlags.StylePriority.UseTextAlignment = False
        Me.XrLabelFlags.Text = "*Flags"
        Me.XrLabelFlags.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        '
        'XrLabelUnit
        '
        Me.XrLabelUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelUnit.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelUnit.LocationFloat = New DevExpress.Utils.PointFloat(374.3333!, 0.0!)
        Me.XrLabelUnit.Name = "XrLabelUnit"
        Me.XrLabelUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelUnit.SizeF = New System.Drawing.SizeF(73.74994!, 20.00002!)
        Me.XrLabelUnit.StylePriority.UseBorders = False
        Me.XrLabelUnit.StylePriority.UseFont = False
        Me.XrLabelUnit.StylePriority.UseTextAlignment = False
        Me.XrLabelUnit.Text = "*Unit"
        Me.XrLabelUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelRefranges
        '
        Me.XrLabelRefranges.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelRefranges.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelRefranges.LocationFloat = New DevExpress.Utils.PointFloat(504.4164!, 0.0!)
        Me.XrLabelRefranges.Name = "XrLabelRefranges"
        Me.XrLabelRefranges.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 5, 0, 0, 100.0!)
        Me.XrLabelRefranges.SizeF = New System.Drawing.SizeF(163.5836!, 20.0!)
        Me.XrLabelRefranges.StylePriority.UseBorders = False
        Me.XrLabelRefranges.StylePriority.UseFont = False
        Me.XrLabelRefranges.StylePriority.UsePadding = False
        Me.XrLabelRefranges.StylePriority.UseTextAlignment = False
        Me.XrLabelRefranges.Text = "*Ref. Ranges"
        Me.XrLabelRefranges.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelConc
        '
        Me.XrLabelConc.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelConc.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelConc.LocationFloat = New DevExpress.Utils.PointFloat(299.5833!, 0.0!)
        Me.XrLabelConc.Name = "XrLabelConc"
        Me.XrLabelConc.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelConc.SizeF = New System.Drawing.SizeF(74.74994!, 20.00002!)
        Me.XrLabelConc.StylePriority.UseBorders = False
        Me.XrLabelConc.StylePriority.UseFont = False
        Me.XrLabelConc.StylePriority.UseTextAlignment = False
        Me.XrLabelConc.Text = "*Conc."
        Me.XrLabelConc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelType
        '
        Me.XrLabelType.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelType.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelType.LocationFloat = New DevExpress.Utils.PointFloat(247.0833!, 0.0!)
        Me.XrLabelType.Name = "XrLabelType"
        Me.XrLabelType.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelType.SizeF = New System.Drawing.SizeF(52.5!, 20.00002!)
        Me.XrLabelType.StylePriority.UseBorders = False
        Me.XrLabelType.StylePriority.UseFont = False
        Me.XrLabelType.StylePriority.UseTextAlignment = False
        Me.XrLabelType.Text = "*Type"
        Me.XrLabelType.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelTest
        '
        Me.XrLabelTest.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelTest.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelTest.LocationFloat = New DevExpress.Utils.PointFloat(0.00001589457!, 0.0!)
        Me.XrLabelTest.Name = "XrLabelTest"
        Me.XrLabelTest.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelTest.SizeF = New System.Drawing.SizeF(247.0833!, 20.00002!)
        Me.XrLabelTest.StylePriority.UseBorders = False
        Me.XrLabelTest.StylePriority.UseFont = False
        Me.XrLabelTest.StylePriority.UseTextAlignment = False
        Me.XrLabelTest.Text = "*Test"
        Me.XrLabelTest.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
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
        Me.BottomMargin.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPageInfo_DateTime, Me.XrPageInfo_TotalPages, Me.XrLabel_ReportDate})
        Me.BottomMargin.HeightF = 25.0!
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.StylePriority.UseTextAlignment = False
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrPageInfo_DateTime
        '
        Me.XrPageInfo_DateTime.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrPageInfo_DateTime.Format = "{0:dd/MM/yyyy HH:mm}"
        Me.XrPageInfo_DateTime.LocationFloat = New DevExpress.Utils.PointFloat(124.5833!, 0.0!)
        Me.XrPageInfo_DateTime.Name = "XrPageInfo_DateTime"
        Me.XrPageInfo_DateTime.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrPageInfo_DateTime.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime
        Me.XrPageInfo_DateTime.SizeF = New System.Drawing.SizeF(242.7083!, 20.00002!)
        Me.XrPageInfo_DateTime.StylePriority.UseFont = False
        Me.XrPageInfo_DateTime.StylePriority.UseTextAlignment = False
        Me.XrPageInfo_DateTime.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft
        Me.XrPageInfo_DateTime.Visible = False
        '
        'XrPageInfo_TotalPages
        '
        Me.XrPageInfo_TotalPages.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrPageInfo_TotalPages.LocationFloat = New DevExpress.Utils.PointFloat(583.0!, 0.0!)
        Me.XrPageInfo_TotalPages.Name = "XrPageInfo_TotalPages"
        Me.XrPageInfo_TotalPages.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrPageInfo_TotalPages.SizeF = New System.Drawing.SizeF(100.0!, 20.00002!)
        Me.XrPageInfo_TotalPages.StylePriority.UseFont = False
        Me.XrPageInfo_TotalPages.StylePriority.UseTextAlignment = False
        Me.XrPageInfo_TotalPages.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight
        Me.XrPageInfo_TotalPages.Visible = False
        '
        'XrLabel_ReportDate
        '
        Me.XrLabel_ReportDate.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel_ReportDate.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel_ReportDate.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrLabel_ReportDate.Name = "XrLabel_ReportDate"
        Me.XrLabel_ReportDate.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel_ReportDate.SizeF = New System.Drawing.SizeF(124.5833!, 20.00002!)
        Me.XrLabel_ReportDate.StylePriority.UseBorders = False
        Me.XrLabel_ReportDate.StylePriority.UseFont = False
        Me.XrLabel_ReportDate.StylePriority.UseTextAlignment = False
        Me.XrLabel_ReportDate.Text = "*Report Date:"
        Me.XrLabel_ReportDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft
        Me.XrLabel_ReportDate.Visible = False
        '
        'XrControlStyle1
        '
        Me.XrControlStyle1.BackColor = System.Drawing.Color.Gainsboro
        Me.XrControlStyle1.Name = "XrControlStyle1"
        Me.XrControlStyle1.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        '
        'GroupHeader1
        '
        Me.GroupHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel_DateTime_Test, Me.XrWSStartDateTimeLabel})
        Me.GroupHeader1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupHeader1.HeightF = 24.0!
        Me.GroupHeader1.Name = "GroupHeader1"
        Me.GroupHeader1.RepeatEveryPage = True
        Me.GroupHeader1.StylePriority.UseFont = False
        '
        'XrLabel_DateTime_Test
        '
        Me.XrLabel_DateTime_Test.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel_DateTime_Test.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel_DateTime_Test.LocationFloat = New DevExpress.Utils.PointFloat(208.4166!, 0.0!)
        Me.XrLabel_DateTime_Test.Name = "XrLabel_DateTime_Test"
        Me.XrLabel_DateTime_Test.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel_DateTime_Test.SizeF = New System.Drawing.SizeF(293.3333!, 20.00002!)
        Me.XrLabel_DateTime_Test.StylePriority.UseBorders = False
        Me.XrLabel_DateTime_Test.StylePriority.UseFont = False
        Me.XrLabel_DateTime_Test.StylePriority.UseTextAlignment = False
        Me.XrLabel_DateTime_Test.Text = "*Date and time of test:"
        Me.XrLabel_DateTime_Test.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        Me.XrLabel_DateTime_Test.Visible = False
        '
        'XrWSStartDateTimeLabel
        '
        Me.XrWSStartDateTimeLabel.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrWSStartDateTimeLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrWSStartDateTimeLabel.LocationFloat = New DevExpress.Utils.PointFloat(501.7499!, 0.0!)
        Me.XrWSStartDateTimeLabel.Name = "XrWSStartDateTimeLabel"
        Me.XrWSStartDateTimeLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrWSStartDateTimeLabel.SizeF = New System.Drawing.SizeF(176.2501!, 20.0!)
        Me.XrWSStartDateTimeLabel.StylePriority.UseBorders = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseFont = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseTextAlignment = False
        Me.XrWSStartDateTimeLabel.Text = "WSStartDateTime"
        Me.XrWSStartDateTimeLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'DetailReport
        '
        Me.DetailReport.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail1})
        Me.DetailReport.DataMember = "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails"
        Me.DetailReport.DataSource = Me.ResultsDS1
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
        Me.DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand
        '
        'Detail1
        '
        Me.Detail1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTableDetails})
        Me.Detail1.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail1.HeightF = 22.0!
        Me.Detail1.Name = "Detail1"
        Me.Detail1.StylePriority.UseFont = False
        '
        'XrTableDetails
        '
        Me.XrTableDetails.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableDetails.LocationFloat = New DevExpress.Utils.PointFloat(10.0!, 0.0!)
        Me.XrTableDetails.Name = "XrTableDetails"
        Me.XrTableDetails.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableDetailsRow})
        Me.XrTableDetails.SizeF = New System.Drawing.SizeF(668.0!, 18.0!)
        Me.XrTableDetails.StylePriority.UseFont = False
        '
        'XrTableDetailsRow
        '
        Me.XrTableDetailsRow.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCellTestName, Me.XrTableCell2, Me.XrTableCell5, Me.XrTableCell6, Me.XrTableCell1, Me.XrTableCell7})
        Me.XrTableDetailsRow.Name = "XrTableDetailsRow"
        Me.XrTableDetailsRow.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 0, 2, 0, 100.0!)
        Me.XrTableDetailsRow.StylePriority.UsePadding = False
        Me.XrTableDetailsRow.Weight = 1.0R
        '
        'XrTableCellTestName
        '
        Me.XrTableCellTestName.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.TestName")})
        Me.XrTableCellTestName.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold)
        Me.XrTableCellTestName.Name = "XrTableCellTestName"
        Me.XrTableCellTestName.StylePriority.UseFont = False
        Me.XrTableCellTestName.Text = "XrTableCellTestName"
        Me.XrTableCellTestName.Weight = 1.012053369549079R
        '
        'XrTableCell2
        '
        Me.XrTableCell2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.SampleType")})
        Me.XrTableCell2.Name = "XrTableCell2"
        Me.XrTableCell2.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 2, 0, 100.0!)
        Me.XrTableCell2.StylePriority.UsePadding = False
        Me.XrTableCell2.Text = "SER"
        Me.XrTableCell2.Weight = 0.215040148278481R
        '
        'XrTableCell5
        '
        Me.XrTableCell5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.CONC_Value")})
        Me.XrTableCell5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell5.Name = "XrTableCell5"
        Me.XrTableCell5.Padding = New DevExpress.XtraPrinting.PaddingInfo(5, 2, 2, 0, 100.0!)
        Me.XrTableCell5.StylePriority.UseFont = False
        Me.XrTableCell5.StylePriority.UsePadding = False
        Me.XrTableCell5.StylePriority.UseTextAlignment = False
        Me.XrTableCell5.Text = "XrTableCell5"
        Me.XrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell5.Weight = 0.30617565697588744R
        '
        'XrTableCell6
        '
        Me.XrTableCell6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.Unit")})
        Me.XrTableCell6.Name = "XrTableCell6"
        Me.XrTableCell6.StylePriority.UseTextAlignment = False
        Me.XrTableCell6.Text = "XrTableCell6"
        Me.XrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell6.Weight = 0.2816000691186612R
        '
        'XrTableCell1
        '
        Me.XrTableCell1.CanGrow = False
        Me.XrTableCell1.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.Remarks")})
        Me.XrTableCell1.Name = "XrTableCell1"
        Me.XrTableCell1.StylePriority.UseTextAlignment = False
        Me.XrTableCell1.Text = "XrTableCell1"
        Me.XrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        Me.XrTableCell1.Weight = 0.25122039381762445R
        Me.XrTableCell1.WordWrap = False
        '
        'XrTableCell7
        '
        Me.XrTableCell7.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.ReferenceRanges")})
        Me.XrTableCell7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold)
        Me.XrTableCell7.Name = "XrTableCell7"
        Me.XrTableCell7.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 5, 2, 0, 100.0!)
        Me.XrTableCell7.StylePriority.UseFont = False
        Me.XrTableCell7.StylePriority.UsePadding = False
        Me.XrTableCell7.StylePriority.UseTextAlignment = False
        Me.XrTableCell7.Text = "XrTableCell7"
        Me.XrTableCell7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell7.Weight = 0.67003850178448321R
        '
        'ResultsDS1
        '
        Me.ResultsDS1.DataSetName = "ResultsDS"
        Me.ResultsDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'XrLabel_PerformedBy
        '
        Me.XrLabel_PerformedBy.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel_PerformedBy.Font = New System.Drawing.Font("Verdana", 9.0!)
        Me.XrLabel_PerformedBy.LocationFloat = New DevExpress.Utils.PointFloat(10.00001!, 140.8333!)
        Me.XrLabel_PerformedBy.Name = "XrLabel_PerformedBy"
        Me.XrLabel_PerformedBy.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel_PerformedBy.SizeF = New System.Drawing.SizeF(203.75!, 20.00001!)
        Me.XrLabel_PerformedBy.StylePriority.UseBorders = False
        Me.XrLabel_PerformedBy.StylePriority.UseFont = False
        Me.XrLabel_PerformedBy.StylePriority.UseTextAlignment = False
        Me.XrLabel_PerformedBy.Text = "*Performed By:"
        Me.XrLabel_PerformedBy.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabel1
        '
        Me.XrLabel1.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel1.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.PerformedBy")})
        Me.XrLabel1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel1.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 140.8333!)
        Me.XrLabel1.Name = "XrLabel1"
        Me.XrLabel1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel1.SizeF = New System.Drawing.SizeF(454.25!, 20.00001!)
        Me.XrLabel1.StylePriority.UseBorders = False
        Me.XrLabel1.StylePriority.UseFont = False
        Me.XrLabel1.StylePriority.UsePadding = False
        Me.XrLabel1.Text = "XrLabel1"
        '
        'PatientsFinalReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport})
        Me.DataMember = "ReportSampleMaster"
        Me.DataSource = Me.ResultsDS1
        Me.Margins = New System.Drawing.Printing.Margins(63, 77, 96, 25)
        Me.PageHeight = 1169
        Me.PageWidth = 827
        Me.PaperKind = System.Drawing.Printing.PaperKind.A4
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
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents DetailReport As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents Detail1 As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents XrPanel2 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelTest As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelType As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelRefranges As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelConc As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel1 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents ResultsDS1 As Biosystems.Ax00.Types.ResultsDS
    Friend WithEvents XrLabel2 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTableDetails As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableDetailsRow As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCellTestName As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell2 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell5 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell6 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell7 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrWSStartDateTimeLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel5 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel4 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel3 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelFlags As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel8 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTableCell1 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrLabel_Gender As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_Age As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_DateBirth As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_PatientID As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_PatientName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_DateTime_Test As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_ReportDate As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPageInfo_TotalPages As DevExpress.XtraReports.UI.XRPageInfo
    Friend WithEvents XrPageInfo_DateTime As DevExpress.XtraReports.UI.XRPageInfo
    Friend WithEvents XrLabel1 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_PerformedBy As DevExpress.XtraReports.UI.XRLabel
End Class
