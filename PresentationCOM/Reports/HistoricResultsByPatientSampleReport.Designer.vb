<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class HistoricResultsByPatientSampleReport
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
        Me.XrLabelFlags = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelUnit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelRefRanges = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelConc = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelType = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelTest = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrLabel_PerformedBy = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel2 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel_Gender = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel_Age = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel_DateBirth = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel_PatientID = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel_PatientName = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel13 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel14 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel15 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel16 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel6 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrWSStartDateTimeLabel = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabel11 = New DevExpress.XtraReports.UI.XRLabel()
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.XrLabel7 = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrPageInfo_DateTime = New DevExpress.XtraReports.UI.XRPageInfo()
        Me.XrPageInfo_TotalPages = New DevExpress.XtraReports.UI.XRPageInfo()
        Me.XrControlStyle1 = New DevExpress.XtraReports.UI.XRControlStyle()
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand()
        Me.Detail1 = New DevExpress.XtraReports.UI.DetailBand()
        Me.XrTableDetails = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableDetailsRow = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCellTestName = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell2 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell4 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell5 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell6 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell8 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.ResultsDS1 = New Biosystems.Ax00.Types.ResultsDS()
        CType(Me.XrTableDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ResultsDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel1, Me.XrPanel2, Me.XrWSStartDateTimeLabel, Me.XrLabel11})
        Me.Detail.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail.HeightF = 233.23!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.StylePriority.UseFont = False
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrPanel1
        '
        Me.XrPanel1.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel1.BorderWidth = 1
        Me.XrPanel1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelFlags, Me.XrLabelUnit, Me.XrLabelRefRanges, Me.XrLabelConc, Me.XrLabelType, Me.XrLabelTest})
        Me.XrPanel1.LocationFloat = New DevExpress.Utils.PointFloat(5.000003!, 208.23!)
        Me.XrPanel1.Name = "XrPanel1"
        Me.XrPanel1.SizeF = New System.Drawing.SizeF(673.0!, 25.0!)
        Me.XrPanel1.StylePriority.UseBorders = False
        Me.XrPanel1.StylePriority.UseBorderWidth = False
        '
        'XrLabelFlags
        '
        Me.XrLabelFlags.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelFlags.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelFlags.LocationFloat = New DevExpress.Utils.PointFloat(448.1134!, 2.000014!)
        Me.XrLabelFlags.Name = "XrLabelFlags"
        Me.XrLabelFlags.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 5, 0, 0, 100.0!)
        Me.XrLabelFlags.SizeF = New System.Drawing.SizeF(56.33!, 20.0!)
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
        Me.XrLabelUnit.LocationFloat = New DevExpress.Utils.PointFloat(374.3333!, 1.999982!)
        Me.XrLabelUnit.Name = "XrLabelUnit"
        Me.XrLabelUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelUnit.SizeF = New System.Drawing.SizeF(73.75!, 20.00001!)
        Me.XrLabelUnit.StylePriority.UseBorders = False
        Me.XrLabelUnit.StylePriority.UseFont = False
        Me.XrLabelUnit.StylePriority.UseTextAlignment = False
        Me.XrLabelUnit.Text = "*Unit"
        Me.XrLabelUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelRefRanges
        '
        Me.XrLabelRefRanges.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelRefRanges.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelRefRanges.LocationFloat = New DevExpress.Utils.PointFloat(504.4434!, 1.999985!)
        Me.XrLabelRefRanges.Name = "XrLabelRefRanges"
        Me.XrLabelRefRanges.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 5, 0, 0, 100.0!)
        Me.XrLabelRefRanges.SizeF = New System.Drawing.SizeF(163.5566!, 19.99998!)
        Me.XrLabelRefRanges.StylePriority.UseBorders = False
        Me.XrLabelRefRanges.StylePriority.UseFont = False
        Me.XrLabelRefRanges.StylePriority.UsePadding = False
        Me.XrLabelRefRanges.StylePriority.UseTextAlignment = False
        Me.XrLabelRefRanges.Text = "*Ref. Ranges"
        Me.XrLabelRefRanges.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelConc
        '
        Me.XrLabelConc.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelConc.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelConc.LocationFloat = New DevExpress.Utils.PointFloat(299.5834!, 1.999982!)
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
        Me.XrLabelType.LocationFloat = New DevExpress.Utils.PointFloat(247.0833!, 1.999982!)
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
        Me.XrLabelTest.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 1.999982!)
        Me.XrLabelTest.Name = "XrLabelTest"
        Me.XrLabelTest.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelTest.SizeF = New System.Drawing.SizeF(247.0833!, 20.00002!)
        Me.XrLabelTest.StylePriority.UseBorders = False
        Me.XrLabelTest.StylePriority.UseFont = False
        Me.XrLabelTest.StylePriority.UseTextAlignment = False
        Me.XrLabelTest.Text = "*Test"
        Me.XrLabelTest.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrPanel2
        '
        Me.XrPanel2.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel2.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel2.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel_PerformedBy, Me.XrLabel2, Me.XrLabel_Gender, Me.XrLabel_Age, Me.XrLabel_DateBirth, Me.XrLabel_PatientID, Me.XrLabel_PatientName, Me.XrLabel13, Me.XrLabel14, Me.XrLabel15, Me.XrLabel16, Me.XrLabel6})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(5.0!, 37.11002!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(673.0!, 165.0!)
        Me.XrPanel2.StylePriority.UseBackColor = False
        Me.XrPanel2.StylePriority.UseBorderColor = False
        Me.XrPanel2.StylePriority.UseBorders = False
        '
        'XrLabel_PerformedBy
        '
        Me.XrLabel_PerformedBy.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel_PerformedBy.Font = New System.Drawing.Font("Verdana", 9.0!)
        Me.XrLabel_PerformedBy.LocationFloat = New DevExpress.Utils.PointFloat(9.999974!, 140.75!)
        Me.XrLabel_PerformedBy.Name = "XrLabel_PerformedBy"
        Me.XrLabel_PerformedBy.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel_PerformedBy.SizeF = New System.Drawing.SizeF(203.75!, 20.00001!)
        Me.XrLabel_PerformedBy.StylePriority.UseBorders = False
        Me.XrLabel_PerformedBy.StylePriority.UseFont = False
        Me.XrLabel_PerformedBy.StylePriority.UseTextAlignment = False
        Me.XrLabel_PerformedBy.Text = "*PerformedBy:"
        Me.XrLabel_PerformedBy.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabel2
        '
        Me.XrLabel2.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.PerformedBy")})
        Me.XrLabel2.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel2.LocationFloat = New DevExpress.Utils.PointFloat(213.7501!, 140.75!)
        Me.XrLabel2.Name = "XrLabel2"
        Me.XrLabel2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel2.SizeF = New System.Drawing.SizeF(454.2499!, 20.0!)
        Me.XrLabel2.StylePriority.UseBorders = False
        Me.XrLabel2.StylePriority.UseFont = False
        Me.XrLabel2.StylePriority.UsePadding = False
        Me.XrLabel2.Text = "XrLabel2"
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
        'XrLabel13
        '
        Me.XrLabel13.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel13.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.FullName")})
        Me.XrLabel13.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel13.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 10.00004!)
        Me.XrLabel13.Name = "XrLabel13"
        Me.XrLabel13.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel13.SizeF = New System.Drawing.SizeF(454.2499!, 20.00003!)
        Me.XrLabel13.StylePriority.UseBorders = False
        Me.XrLabel13.StylePriority.UseFont = False
        Me.XrLabel13.StylePriority.UsePadding = False
        Me.XrLabel13.Text = "XrLabel8"
        '
        'XrLabel14
        '
        Me.XrLabel14.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel14.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.AgeWithUnit")})
        Me.XrLabel14.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel14.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 89.99996!)
        Me.XrLabel14.Name = "XrLabel14"
        Me.XrLabel14.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel14.SizeF = New System.Drawing.SizeF(454.2498!, 20.00003!)
        Me.XrLabel14.StylePriority.UseBorders = False
        Me.XrLabel14.StylePriority.UseFont = False
        Me.XrLabel14.StylePriority.UsePadding = False
        Me.XrLabel14.Text = "XrLabel5"
        '
        'XrLabel15
        '
        Me.XrLabel15.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel15.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.FormatedDateOfBirth")})
        Me.XrLabel15.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel15.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 64.99996!)
        Me.XrLabel15.Name = "XrLabel15"
        Me.XrLabel15.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel15.SizeF = New System.Drawing.SizeF(454.25!, 20.00003!)
        Me.XrLabel15.StylePriority.UseBorders = False
        Me.XrLabel15.StylePriority.UseFont = False
        Me.XrLabel15.StylePriority.UsePadding = False
        Me.XrLabel15.Text = "XrLabel4"
        '
        'XrLabel16
        '
        Me.XrLabel16.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel16.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.Gender")})
        Me.XrLabel16.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel16.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 115.0!)
        Me.XrLabel16.Name = "XrLabel16"
        Me.XrLabel16.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel16.SizeF = New System.Drawing.SizeF(454.2498!, 20.00001!)
        Me.XrLabel16.StylePriority.UseBorders = False
        Me.XrLabel16.StylePriority.UseFont = False
        Me.XrLabel16.StylePriority.UsePadding = False
        Me.XrLabel16.Text = "XrLabel3"
        '
        'XrLabel6
        '
        Me.XrLabel6.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.FullID")})
        Me.XrLabel6.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrLabel6.LocationFloat = New DevExpress.Utils.PointFloat(213.75!, 37.99998!)
        Me.XrLabel6.Name = "XrLabel6"
        Me.XrLabel6.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel6.SizeF = New System.Drawing.SizeF(454.2499!, 20.00002!)
        Me.XrLabel6.StylePriority.UseBorders = False
        Me.XrLabel6.StylePriority.UseFont = False
        Me.XrLabel6.StylePriority.UsePadding = False
        Me.XrLabel6.Text = "XrLabel6"
        '
        'XrWSStartDateTimeLabel
        '
        Me.XrWSStartDateTimeLabel.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrWSStartDateTimeLabel.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportDate", "{0:dd/MM/yyyy HH:mm}")})
        Me.XrWSStartDateTimeLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrWSStartDateTimeLabel.LocationFloat = New DevExpress.Utils.PointFloat(506.9583!, 10.0!)
        Me.XrWSStartDateTimeLabel.Name = "XrWSStartDateTimeLabel"
        Me.XrWSStartDateTimeLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrWSStartDateTimeLabel.SizeF = New System.Drawing.SizeF(171.0417!, 20.0!)
        Me.XrWSStartDateTimeLabel.StylePriority.UseBorders = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseFont = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseTextAlignment = False
        Me.XrWSStartDateTimeLabel.Text = "XrWSStartDateTimeLabel"
        Me.XrWSStartDateTimeLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'XrLabel11
        '
        Me.XrLabel11.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel11.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel11.LocationFloat = New DevExpress.Utils.PointFloat(219.4583!, 10.0!)
        Me.XrLabel11.Name = "XrLabel11"
        Me.XrLabel11.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel11.SizeF = New System.Drawing.SizeF(287.5!, 20.00002!)
        Me.XrLabel11.StylePriority.UseBorders = False
        Me.XrLabel11.StylePriority.UseFont = False
        Me.XrLabel11.StylePriority.UseTextAlignment = False
        Me.XrLabel11.Text = "*Date and time of test:"
        Me.XrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
        '
        'TopMargin
        '
        Me.TopMargin.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TopMargin.HeightF = 96.0!
        Me.TopMargin.Name = "TopMargin"
        Me.TopMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.TopMargin.StylePriority.UseFont = False
        Me.TopMargin.StylePriority.UseTextAlignment = False
        Me.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopJustify
        '
        'BottomMargin
        '
        Me.BottomMargin.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel7, Me.XrPageInfo_DateTime, Me.XrPageInfo_TotalPages})
        Me.BottomMargin.HeightF = 26.95834!
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabel7
        '
        Me.XrLabel7.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabel7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabel7.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrLabel7.Name = "XrLabel7"
        Me.XrLabel7.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel7.SizeF = New System.Drawing.SizeF(124.5833!, 20.00002!)
        Me.XrLabel7.StylePriority.UseBorders = False
        Me.XrLabel7.StylePriority.UseFont = False
        Me.XrLabel7.StylePriority.UseTextAlignment = False
        Me.XrLabel7.Text = "*Report Date:"
        Me.XrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft
        Me.XrLabel7.Visible = False
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
        Me.XrPageInfo_TotalPages.LocationFloat = New DevExpress.Utils.PointFloat(585.0001!, 0.0!)
        Me.XrPageInfo_TotalPages.Name = "XrPageInfo_TotalPages"
        Me.XrPageInfo_TotalPages.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrPageInfo_TotalPages.SizeF = New System.Drawing.SizeF(100.0!, 20.00002!)
        Me.XrPageInfo_TotalPages.StylePriority.UseFont = False
        Me.XrPageInfo_TotalPages.StylePriority.UseTextAlignment = False
        Me.XrPageInfo_TotalPages.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight
        Me.XrPageInfo_TotalPages.Visible = False
        '
        'XrControlStyle1
        '
        Me.XrControlStyle1.BackColor = System.Drawing.Color.Transparent
        Me.XrControlStyle1.Name = "XrControlStyle1"
        Me.XrControlStyle1.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        '
        'GroupHeader1
        '
        Me.GroupHeader1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupHeader1.HeightF = 24.0!
        Me.GroupHeader1.Name = "GroupHeader1"
        Me.GroupHeader1.RepeatEveryPage = True
        Me.GroupHeader1.StylePriority.UseFont = False
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
        Me.Detail1.BackColor = System.Drawing.Color.White
        Me.Detail1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTableDetails})
        Me.Detail1.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail1.HeightF = 18.0!
        Me.Detail1.Name = "Detail1"
        '
        'XrTableDetails
        '
        Me.XrTableDetails.BackColor = System.Drawing.Color.White
        Me.XrTableDetails.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableDetails.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrTableDetails.Name = "XrTableDetails"
        Me.XrTableDetails.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableDetailsRow})
        Me.XrTableDetails.SizeF = New System.Drawing.SizeF(685.0!, 18.0!)
        Me.XrTableDetails.StylePriority.UseFont = False
        '
        'XrTableDetailsRow
        '
        Me.XrTableDetailsRow.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCellTestName, Me.XrTableCell2, Me.XrTableCell4, Me.XrTableCell5, Me.XrTableCell6, Me.XrTableCell8})
        Me.XrTableDetailsRow.Name = "XrTableDetailsRow"
        Me.XrTableDetailsRow.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 0, 2, 0, 100.0!)
        Me.XrTableDetailsRow.StylePriority.UsePadding = False
        Me.XrTableDetailsRow.Weight = 1.0R
        '
        'XrTableCellTestName
        '
        Me.XrTableCellTestName.BackColor = System.Drawing.Color.White
        Me.XrTableCellTestName.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.TestName")})
        Me.XrTableCellTestName.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCellTestName.Name = "XrTableCellTestName"
        Me.XrTableCellTestName.StylePriority.UseBackColor = False
        Me.XrTableCellTestName.StylePriority.UseFont = False
        Me.XrTableCellTestName.Text = "[ReportSampleMaster_ReportSampleDetails.TestName]"
        Me.XrTableCellTestName.Weight = 1.1765872047061012R
        '
        'XrTableCell2
        '
        Me.XrTableCell2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.SampleType")})
        Me.XrTableCell2.Name = "XrTableCell2"
        Me.XrTableCell2.Text = "XrTableCell2"
        Me.XrTableCell2.Weight = 0.25000010899135039R
        '
        'XrTableCell4
        '
        Me.XrTableCell4.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.CONC_Value")})
        Me.XrTableCell4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCell4.Name = "XrTableCell4"
        Me.XrTableCell4.StylePriority.UseFont = False
        Me.XrTableCell4.StylePriority.UseTextAlignment = False
        Me.XrTableCell4.Text = "XrTableCell4"
        Me.XrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell4.Weight = 0.35595207214355479R
        '
        'XrTableCell5
        '
        Me.XrTableCell5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.Unit")})
        Me.XrTableCell5.Name = "XrTableCell5"
        Me.XrTableCell5.Padding = New DevExpress.XtraPrinting.PaddingInfo(3, 2, 2, 0, 100.0!)
        Me.XrTableCell5.StylePriority.UsePadding = False
        Me.XrTableCell5.StylePriority.UseTextAlignment = False
        Me.XrTableCell5.Text = "XrTableCell5"
        Me.XrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell5.Weight = 0.35119060811568026R
        '
        'XrTableCell6
        '
        Me.XrTableCell6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.Remarks")})
        Me.XrTableCell6.Name = "XrTableCell6"
        Me.XrTableCell6.StylePriority.UseTextAlignment = False
        Me.XrTableCell6.Text = "XrTableCell6"
        Me.XrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
        Me.XrTableCell6.Weight = 0.26838117040024384R
        '
        'XrTableCell8
        '
        Me.XrTableCell8.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReportSampleMaster.ReportSampleMaster_ReportSampleDetails.ReferenceRanges")})
        Me.XrTableCell8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCell8.Name = "XrTableCell8"
        Me.XrTableCell8.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 2, 0, 100.0!)
        Me.XrTableCell8.StylePriority.UseFont = False
        Me.XrTableCell8.StylePriority.UsePadding = False
        Me.XrTableCell8.StylePriority.UseTextAlignment = False
        Me.XrTableCell8.Text = "XrTableCell8"
        Me.XrTableCell8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell8.Weight = 0.859793439943365R
        '
        'ResultsDS1
        '
        Me.ResultsDS1.DataSetName = "ResultsDS"
        Me.ResultsDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'HistoricResultsByPatientSampleReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport})
        Me.DataMember = "ReportSampleMaster"
        Me.DataSource = Me.ResultsDS1
        Me.Margins = New System.Drawing.Printing.Margins(63, 76, 96, 27)
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
    Friend WithEvents ResultsDS1 As Biosystems.Ax00.Types.ResultsDS
    Friend WithEvents XrTableDetails As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableDetailsRow As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCellTestName As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell2 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell4 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell5 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell6 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell8 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrLabel11 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrWSStartDateTimeLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel2 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabel_Gender As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_Age As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_DateBirth As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_PatientID As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel_PatientName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel13 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel14 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel15 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel16 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel6 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel1 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelFlags As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelRefRanges As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelConc As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelType As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelTest As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel7 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPageInfo_DateTime As DevExpress.XtraReports.UI.XRPageInfo
    Friend WithEvents XrPageInfo_TotalPages As DevExpress.XtraReports.UI.XRPageInfo
    Friend WithEvents XrLabel_PerformedBy As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel2 As DevExpress.XtraReports.UI.XRLabel
End Class
