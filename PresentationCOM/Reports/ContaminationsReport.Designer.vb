<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class ContaminationsReport
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
        Me.XrControlStyle1 = New DevExpress.XtraReports.UI.XRControlStyle
        Me.XrHeaderLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand
        Me.XrPanel4 = New DevExpress.XtraReports.UI.XRPanel
        Me.Step2Label = New DevExpress.XtraReports.UI.XRLabel
        Me.Step1Label = New DevExpress.XtraReports.UI.XRLabel
        Me.CuvetteContaminatorLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.XrPanel3 = New DevExpress.XtraReports.UI.XRPanel
        Me.CuvettesContaminationsLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.XrPanel2 = New DevExpress.XtraReports.UI.XRPanel
        Me.WashingSolutionLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.ReagentContaminatedLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.ReagentContaminatorLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.XrPanel1 = New DevExpress.XtraReports.UI.XRPanel
        Me.ReagentsContaminationsLabel = New DevExpress.XtraReports.UI.XRLabel
        Me.ContaminationsDS1 = New Biosystems.Ax00.Types.ContaminationsDS
        Me.XrLabel7 = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabel6 = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabel5 = New DevExpress.XtraReports.UI.XRLabel
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand
        Me.Detail1 = New DevExpress.XtraReports.UI.DetailBand
        Me.GroupHeader2 = New DevExpress.XtraReports.UI.GroupHeaderBand
        Me.DetailReport1 = New DevExpress.XtraReports.UI.DetailReportBand
        Me.Detail2 = New DevExpress.XtraReports.UI.DetailBand
        Me.XrLabel2 = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabel3 = New DevExpress.XtraReports.UI.XRLabel
        Me.XrLabel4 = New DevExpress.XtraReports.UI.XRLabel
        Me.ReportHeader1 = New DevExpress.XtraReports.UI.ReportHeaderBand
        CType(Me.ContaminationsDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail.HeightF = 0.0!
        Me.Detail.KeepTogetherWithDetailReports = True
        Me.Detail.MultiColumn.Mode = DevExpress.XtraReports.UI.MultiColumnMode.UseColumnCount
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.StylePriority.UseFont = False
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
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
        Me.GroupHeader1.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupHeader1.HeightF = 44.91663!
        Me.GroupHeader1.Name = "GroupHeader1"
        Me.GroupHeader1.RepeatEveryPage = True
        Me.GroupHeader1.StylePriority.UseFont = False
        '
        'XrPanel4
        '
        Me.XrPanel4.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel4.BorderWidth = 2
        Me.XrPanel4.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.Step2Label, Me.Step1Label, Me.CuvetteContaminatorLabel})
        Me.XrPanel4.LocationFloat = New DevExpress.Utils.PointFloat(10.0!, 42.5!)
        Me.XrPanel4.Name = "XrPanel4"
        Me.XrPanel4.SizeF = New System.Drawing.SizeF(629.9999!, 23.12501!)
        Me.XrPanel4.StylePriority.UseBorders = False
        Me.XrPanel4.StylePriority.UseBorderWidth = False
        '
        'Step2Label
        '
        Me.Step2Label.LocationFloat = New DevExpress.Utils.PointFloat(430.0!, 0.0!)
        Me.Step2Label.Name = "Step2Label"
        Me.Step2Label.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.Step2Label.SizeF = New System.Drawing.SizeF(199.9998!, 23.00002!)
        Me.Step2Label.Text = "Step 2"
        '
        'Step1Label
        '
        Me.Step1Label.LocationFloat = New DevExpress.Utils.PointFloat(215.0002!, 0.0!)
        Me.Step1Label.Name = "Step1Label"
        Me.Step1Label.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.Step1Label.SizeF = New System.Drawing.SizeF(205.0!, 23.00002!)
        Me.Step1Label.Text = "Step 1"
        '
        'CuvetteContaminatorLabel
        '
        Me.CuvetteContaminatorLabel.LocationFloat = New DevExpress.Utils.PointFloat(0.00009536743!, 0.0!)
        Me.CuvetteContaminatorLabel.Name = "CuvetteContaminatorLabel"
        Me.CuvetteContaminatorLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.CuvetteContaminatorLabel.SizeF = New System.Drawing.SizeF(205.0!, 23.00002!)
        Me.CuvetteContaminatorLabel.Text = "Contaminator"
        '
        'XrPanel3
        '
        Me.XrPanel3.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel3.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel3.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel3.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.CuvettesContaminationsLabel})
        Me.XrPanel3.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrPanel3.Name = "XrPanel3"
        Me.XrPanel3.SizeF = New System.Drawing.SizeF(650.0!, 30.0!)
        Me.XrPanel3.StylePriority.UseBackColor = False
        Me.XrPanel3.StylePriority.UseBorderColor = False
        Me.XrPanel3.StylePriority.UseBorders = False
        '
        'CuvettesContaminationsLabel
        '
        Me.CuvettesContaminationsLabel.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.CuvettesContaminationsLabel.Font = New System.Drawing.Font("Verdana", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CuvettesContaminationsLabel.LocationFloat = New DevExpress.Utils.PointFloat(10.0!, 6.0!)
        Me.CuvettesContaminationsLabel.Name = "CuvettesContaminationsLabel"
        Me.CuvettesContaminationsLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.CuvettesContaminationsLabel.SizeF = New System.Drawing.SizeF(629.9999!, 20.00001!)
        Me.CuvettesContaminationsLabel.StylePriority.UseBorders = False
        Me.CuvettesContaminationsLabel.StylePriority.UseFont = False
        Me.CuvettesContaminationsLabel.StylePriority.UseTextAlignment = False
        Me.CuvettesContaminationsLabel.Text = "Cuvettes Contaminations"
        Me.CuvettesContaminationsLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'XrPanel2
        '
        Me.XrPanel2.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel2.BorderWidth = 2
        Me.XrPanel2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.WashingSolutionLabel, Me.ReagentContaminatedLabel, Me.ReagentContaminatorLabel})
        Me.XrPanel2.LocationFloat = New DevExpress.Utils.PointFloat(10.0001!, 42.49999!)
        Me.XrPanel2.Name = "XrPanel2"
        Me.XrPanel2.SizeF = New System.Drawing.SizeF(629.9999!, 23.12501!)
        Me.XrPanel2.StylePriority.UseBorders = False
        Me.XrPanel2.StylePriority.UseBorderWidth = False
        '
        'WashingSolutionLabel
        '
        Me.WashingSolutionLabel.LocationFloat = New DevExpress.Utils.PointFloat(429.9999!, 0.0!)
        Me.WashingSolutionLabel.Name = "WashingSolutionLabel"
        Me.WashingSolutionLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.WashingSolutionLabel.SizeF = New System.Drawing.SizeF(200.0!, 23.00002!)
        Me.WashingSolutionLabel.Text = "Washing Solution"
        '
        'ReagentContaminatedLabel
        '
        Me.ReagentContaminatedLabel.LocationFloat = New DevExpress.Utils.PointFloat(215.0!, 0.1249949!)
        Me.ReagentContaminatedLabel.Name = "ReagentContaminatedLabel"
        Me.ReagentContaminatedLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.ReagentContaminatedLabel.SizeF = New System.Drawing.SizeF(205.0!, 23.00002!)
        Me.ReagentContaminatedLabel.Text = "Contaminated"
        '
        'ReagentContaminatorLabel
        '
        Me.ReagentContaminatorLabel.LocationFloat = New DevExpress.Utils.PointFloat(0.00003178914!, 0.1249949!)
        Me.ReagentContaminatorLabel.Name = "ReagentContaminatorLabel"
        Me.ReagentContaminatorLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.ReagentContaminatorLabel.SizeF = New System.Drawing.SizeF(205.0!, 23.00002!)
        Me.ReagentContaminatorLabel.Text = "Contaminator"
        '
        'XrPanel1
        '
        Me.XrPanel1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.XrPanel1.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel1.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
                    Or DevExpress.XtraPrinting.BorderSide.Right) _
                    Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.ReagentsContaminationsLabel})
        Me.XrPanel1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrPanel1.Name = "XrPanel1"
        Me.XrPanel1.SizeF = New System.Drawing.SizeF(650.0!, 30.0!)
        Me.XrPanel1.StylePriority.UseBackColor = False
        Me.XrPanel1.StylePriority.UseBorderColor = False
        Me.XrPanel1.StylePriority.UseBorders = False
        '
        'ReagentsContaminationsLabel
        '
        Me.ReagentsContaminationsLabel.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.ReagentsContaminationsLabel.Font = New System.Drawing.Font("Verdana", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ReagentsContaminationsLabel.LocationFloat = New DevExpress.Utils.PointFloat(10.0!, 6.0!)
        Me.ReagentsContaminationsLabel.Name = "ReagentsContaminationsLabel"
        Me.ReagentsContaminationsLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.ReagentsContaminationsLabel.SizeF = New System.Drawing.SizeF(630.0!, 20.00001!)
        Me.ReagentsContaminationsLabel.StylePriority.UseBorders = False
        Me.ReagentsContaminationsLabel.StylePriority.UseFont = False
        Me.ReagentsContaminationsLabel.StylePriority.UseTextAlignment = False
        Me.ReagentsContaminationsLabel.Text = "Reagents Contaminations"
        Me.ReagentsContaminationsLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter
        '
        'ContaminationsDS1
        '
        Me.ContaminationsDS1.DataSetName = "ContaminationsDS"
        Me.ContaminationsDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'XrLabel7
        '
        Me.XrLabel7.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReagentsContaminations.WashDesc")})
        Me.XrLabel7.LocationFloat = New DevExpress.Utils.PointFloat(440.0!, 0.0!)
        Me.XrLabel7.Name = "XrLabel7"
        Me.XrLabel7.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel7.SizeF = New System.Drawing.SizeF(200.0!, 22.99999!)
        Me.XrLabel7.Text = "XrLabel7"
        '
        'XrLabel6
        '
        Me.XrLabel6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReagentsContaminations.Contaminated")})
        Me.XrLabel6.LocationFloat = New DevExpress.Utils.PointFloat(225.0001!, 0.0!)
        Me.XrLabel6.Name = "XrLabel6"
        Me.XrLabel6.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel6.SizeF = New System.Drawing.SizeF(204.9999!, 22.99999!)
        Me.XrLabel6.Text = "XrLabel6"
        '
        'XrLabel5
        '
        Me.XrLabel5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ReagentsContaminations.Contaminator")})
        Me.XrLabel5.LocationFloat = New DevExpress.Utils.PointFloat(10.0001!, 0.0!)
        Me.XrLabel5.Name = "XrLabel5"
        Me.XrLabel5.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel5.SizeF = New System.Drawing.SizeF(205.0!, 22.99999!)
        Me.XrLabel5.Text = "XrLabel5"
        '
        'DetailReport
        '
        Me.DetailReport.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail1, Me.GroupHeader2})
        Me.DetailReport.DataMember = "ReagentsContaminations"
        Me.DetailReport.DataSource = Me.ContaminationsDS1
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
        '
        'Detail1
        '
        Me.Detail1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel5, Me.XrLabel7, Me.XrLabel6})
        Me.Detail1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail1.HeightF = 23.0!
        Me.Detail1.Name = "Detail1"
        Me.Detail1.StylePriority.UseFont = False
        '
        'GroupHeader2
        '
        Me.GroupHeader2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel1, Me.XrPanel2})
        Me.GroupHeader2.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupHeader2.HeightF = 70.0!
        Me.GroupHeader2.KeepTogether = True
        Me.GroupHeader2.Name = "GroupHeader2"
        Me.GroupHeader2.RepeatEveryPage = True
        Me.GroupHeader2.StylePriority.UseFont = False
        '
        'DetailReport1
        '
        Me.DetailReport1.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail2, Me.ReportHeader1})
        Me.DetailReport1.DataMember = "CuvettesContaminations"
        Me.DetailReport1.DataSource = Me.ContaminationsDS1
        Me.DetailReport1.Level = 1
        Me.DetailReport1.Name = "DetailReport1"
        Me.DetailReport1.PageBreak = DevExpress.XtraReports.UI.PageBreak.BeforeBand
        '
        'Detail2
        '
        Me.Detail2.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel2, Me.XrLabel3, Me.XrLabel4})
        Me.Detail2.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Detail2.HeightF = 23.00002!
        Me.Detail2.Name = "Detail2"
        Me.Detail2.StylePriority.UseFont = False
        '
        'XrLabel2
        '
        Me.XrLabel2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CuvettesContaminations.Contaminators")})
        Me.XrLabel2.LocationFloat = New DevExpress.Utils.PointFloat(10.00001!, 0.0!)
        Me.XrLabel2.Name = "XrLabel2"
        Me.XrLabel2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel2.SizeF = New System.Drawing.SizeF(205.0002!, 23.00002!)
        Me.XrLabel2.Text = "XrLabel2"
        '
        'XrLabel3
        '
        Me.XrLabel3.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CuvettesContaminations.Step1")})
        Me.XrLabel3.LocationFloat = New DevExpress.Utils.PointFloat(225.0002!, 0.0!)
        Me.XrLabel3.Name = "XrLabel3"
        Me.XrLabel3.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel3.SizeF = New System.Drawing.SizeF(205.0!, 23.00002!)
        Me.XrLabel3.Text = "XrLabel3"
        '
        'XrLabel4
        '
        Me.XrLabel4.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "CuvettesContaminations.Step2")})
        Me.XrLabel4.LocationFloat = New DevExpress.Utils.PointFloat(440.0!, 0.0!)
        Me.XrLabel4.Name = "XrLabel4"
        Me.XrLabel4.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel4.SizeF = New System.Drawing.SizeF(199.9999!, 23.00002!)
        Me.XrLabel4.Text = "XrLabel4"
        '
        'ReportHeader1
        '
        Me.ReportHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel3, Me.XrPanel4})
        Me.ReportHeader1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ReportHeader1.HeightF = 70.0!
        Me.ReportHeader1.KeepTogether = True
        Me.ReportHeader1.Name = "ReportHeader1"
        Me.ReportHeader1.StylePriority.UseFont = False
        '
        'ContaminationsReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport, Me.DetailReport1})
        Me.DataSource = Me.ContaminationsDS1
        Me.Margins = New System.Drawing.Printing.Margins(100, 100, 96, 100)
        Me.StyleSheet.AddRange(New DevExpress.XtraReports.UI.XRControlStyle() {Me.XrControlStyle1})
        Me.Version = "10.2"
        CType(Me.ContaminationsDS1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents XrControlStyle1 As DevExpress.XtraReports.UI.XRControlStyle
    Protected WithEvents XrHeaderLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents XrPanel3 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents CuvettesContaminationsLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel2 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrPanel1 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents ReagentsContaminationsLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel4 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents ContaminationsDS1 As Biosystems.Ax00.Types.ContaminationsDS
    Friend WithEvents XrLabel5 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel7 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel6 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents DetailReport As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents Detail1 As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents DetailReport1 As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents Detail2 As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents ReportHeader1 As DevExpress.XtraReports.UI.ReportHeaderBand
    Friend WithEvents XrLabel4 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel2 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabel3 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents Step2Label As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents Step1Label As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents CuvetteContaminatorLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents WashingSolutionLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents ReagentContaminatedLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents ReagentContaminatorLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents GroupHeader2 As DevExpress.XtraReports.UI.GroupHeaderBand
End Class
