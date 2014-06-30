<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class MasterTemplateLS
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
        Me.PageFooter = New DevExpress.XtraReports.UI.PageFooterBand()
        Me.XrPageInfo1 = New DevExpress.XtraReports.UI.XRPageInfo()
        Me.XrPageInfo = New DevExpress.XtraReports.UI.XRPageInfo()
        Me.FormattingRule1 = New DevExpress.XtraReports.UI.FormattingRule()
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.PageHeader = New DevExpress.XtraReports.UI.PageHeaderBand()
        Me.XrControlStyle1 = New DevExpress.XtraReports.UI.XRControlStyle()
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'PageFooter
        '
        Me.PageFooter.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPageInfo1, Me.XrPageInfo})
        Me.PageFooter.HeightF = 80.00002!
        Me.PageFooter.Name = "PageFooter"
        Me.PageFooter.PrintOn = CType((DevExpress.XtraReports.UI.PrintOnPages.NotWithReportHeader Or DevExpress.XtraReports.UI.PrintOnPages.NotWithReportFooter), DevExpress.XtraReports.UI.PrintOnPages)
        '
        'XrPageInfo1
        '
        Me.XrPageInfo1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrPageInfo1.Format = "{0:dd/MM/yyyy HH:mm}"
        Me.XrPageInfo1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 65.00002!)
        Me.XrPageInfo1.Name = "XrPageInfo1"
        Me.XrPageInfo1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrPageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime
        Me.XrPageInfo1.SizeF = New System.Drawing.SizeF(270.8333!, 14.99999!)
        Me.XrPageInfo1.StylePriority.UseFont = False
        Me.XrPageInfo1.StylePriority.UseTextAlignment = False
        Me.XrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomLeft
        Me.XrPageInfo1.Visible = False
        '
        'XrPageInfo
        '
        Me.XrPageInfo.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrPageInfo.LocationFloat = New DevExpress.Utils.PointFloat(943.0001!, 65.00002!)
        Me.XrPageInfo.Name = "XrPageInfo"
        Me.XrPageInfo.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrPageInfo.SizeF = New System.Drawing.SizeF(100.0!, 15.0!)
        Me.XrPageInfo.StylePriority.UseFont = False
        Me.XrPageInfo.StylePriority.UseTextAlignment = False
        Me.XrPageInfo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomRight
        Me.XrPageInfo.Visible = False
        '
        'FormattingRule1
        '
        Me.FormattingRule1.Name = "FormattingRule1"
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
        Me.BottomMargin.HeightF = 25.0!
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'PageHeader
        '
        Me.PageHeader.HeightF = 80.0!
        Me.PageHeader.Name = "PageHeader"
        '
        'XrControlStyle1
        '
        Me.XrControlStyle1.Name = "XrControlStyle1"
        '
        'Detail
        '
        Me.Detail.HeightF = 85.0!
        Me.Detail.LockedInUserDesigner = True
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'MasterTemplateLS
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.BottomMargin, Me.PageFooter, Me.Detail, Me.TopMargin, Me.PageHeader})
        Me.DesignerOptions.ShowExportWarnings = False
        Me.DesignerOptions.ShowPrintingWarnings = False
        Me.DisplayName = " "
        Me.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormattingRuleSheet.AddRange(New DevExpress.XtraReports.UI.FormattingRule() {Me.FormattingRule1})
        Me.Landscape = True
        Me.Margins = New System.Drawing.Printing.Margins(63, 63, 59, 25)
        Me.PageHeight = 827
        Me.PageWidth = 1169
        Me.PaperKind = System.Drawing.Printing.PaperKind.A4
        Me.ScriptLanguage = DevExpress.XtraReports.ScriptLanguage.VisualBasic
        Me.StyleSheet.AddRange(New DevExpress.XtraReports.UI.XRControlStyle() {Me.XrControlStyle1})
        Me.Version = "10.2"
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents PageFooter As DevExpress.XtraReports.UI.PageFooterBand
    Friend WithEvents FormattingRule1 As DevExpress.XtraReports.UI.FormattingRule
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents PageHeader As DevExpress.XtraReports.UI.PageHeaderBand
    Friend WithEvents XrControlStyle1 As DevExpress.XtraReports.UI.XRControlStyle
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents XrPageInfo1 As DevExpress.XtraReports.UI.XRPageInfo
    Friend WithEvents XrPageInfo As DevExpress.XtraReports.UI.XRPageInfo
End Class
