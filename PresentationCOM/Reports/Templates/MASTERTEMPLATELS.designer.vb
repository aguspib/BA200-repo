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
        Dim ShapeRectangle1 As DevExpress.XtraPrinting.Shape.ShapeRectangle = New DevExpress.XtraPrinting.Shape.ShapeRectangle
        Me.PageFooter = New DevExpress.XtraReports.UI.PageFooterBand
        Me.FormattingRule1 = New DevExpress.XtraReports.UI.FormattingRule
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand
        Me.XrLabel1 = New DevExpress.XtraReports.UI.XRLabel
        Me.XrShape1 = New DevExpress.XtraReports.UI.XRShape
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand
        Me.PageHeader = New DevExpress.XtraReports.UI.PageHeaderBand
        Me.XrControlStyle1 = New DevExpress.XtraReports.UI.XRControlStyle
        Me.Detail = New DevExpress.XtraReports.UI.DetailBand
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'PageFooter
        '
        Me.PageFooter.HeightF = 80.0!
        Me.PageFooter.Name = "PageFooter"
        Me.PageFooter.PrintOn = CType((DevExpress.XtraReports.UI.PrintOnPages.NotWithReportHeader Or DevExpress.XtraReports.UI.PrintOnPages.NotWithReportFooter), DevExpress.XtraReports.UI.PrintOnPages)
        '
        'FormattingRule1
        '
        Me.FormattingRule1.Name = "FormattingRule1"
        '
        'TopMargin
        '
        Me.TopMargin.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabel1, Me.XrShape1})
        Me.TopMargin.HeightF = 80.0!
        Me.TopMargin.Name = "TopMargin"
        Me.TopMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabel1
        '
        Me.XrLabel1.LocationFloat = New DevExpress.Utils.PointFloat(947.25!, 36.83336!)
        Me.XrLabel1.Name = "XrLabel1"
        Me.XrLabel1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabel1.SizeF = New System.Drawing.SizeF(43.75!, 23.0!)
        Me.XrLabel1.Text = "LOGO"
        '
        'XrShape1
        '
        Me.XrShape1.LocationFloat = New DevExpress.Utils.PointFloat(927.7499!, 28.83336!)
        Me.XrShape1.Name = "XrShape1"
        Me.XrShape1.Shape = ShapeRectangle1
        Me.XrShape1.SizeF = New System.Drawing.SizeF(81.24994!, 41.16665!)
        '
        'BottomMargin
        '
        Me.BottomMargin.HeightF = 80.0!
        Me.BottomMargin.LockedInUserDesigner = True
        Me.BottomMargin.Name = "BottomMargin"
        Me.BottomMargin.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.BottomMargin.Visible = False
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
        Me.Margins = New System.Drawing.Printing.Margins(80, 80, 80, 80)
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
    Friend WithEvents XrLabel1 As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrShape1 As DevExpress.XtraReports.UI.XRShape
End Class
