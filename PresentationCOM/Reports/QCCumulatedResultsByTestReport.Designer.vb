<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class QCCumulatedResultsByTestReport
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
        Me.XrDateRange = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrSample = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelDateRange = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelSample = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrTestName = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelTestName = New DevExpress.XtraReports.UI.XRLabel()
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.GroupHeader1 = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrHeaderLabel = New DevExpress.XtraReports.UI.XRLabel()
        Me.DetailReport = New DevExpress.XtraReports.UI.DetailReportBand()
        Me.Detail1 = New DevExpress.XtraReports.UI.DetailBand()
        Me.XrLabelControls = New DevExpress.XtraReports.UI.XRLabel()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrPanel1})
        Me.Detail.HeightF = 78.125!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrPanel1
        '
        Me.XrPanel1.BackColor = System.Drawing.Color.White
        Me.XrPanel1.BorderColor = System.Drawing.Color.DarkGray
        Me.XrPanel1.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrPanel1.BorderWidth = 2
        Me.XrPanel1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrDateRange, Me.XrSample, Me.XrLabelDateRange, Me.XrLabelSample, Me.XrTestName, Me.XrLabelTestName})
        Me.XrPanel1.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 10.0!)
        Me.XrPanel1.Name = "XrPanel1"
        Me.XrPanel1.SizeF = New System.Drawing.SizeF(649.9998!, 49.79167!)
        Me.XrPanel1.StylePriority.UseBackColor = False
        Me.XrPanel1.StylePriority.UseBorderColor = False
        Me.XrPanel1.StylePriority.UseBorders = False
        Me.XrPanel1.StylePriority.UseBorderWidth = False
        '
        'XrDateRange
        '
        Me.XrDateRange.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrDateRange.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrDateRange.LocationFloat = New DevExpress.Utils.PointFloat(462.0564!, 25.00006!)
        Me.XrDateRange.Name = "XrDateRange"
        Me.XrDateRange.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrDateRange.SizeF = New System.Drawing.SizeF(183.9433!, 20.00002!)
        Me.XrDateRange.StylePriority.UseBorders = False
        Me.XrDateRange.StylePriority.UseFont = False
        Me.XrDateRange.StylePriority.UseTextAlignment = False
        Me.XrDateRange.Text = "XrDateRange"
        Me.XrDateRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrSample
        '
        Me.XrSample.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrSample.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrSample.LocationFloat = New DevExpress.Utils.PointFloat(194.2079!, 25.00006!)
        Me.XrSample.Name = "XrSample"
        Me.XrSample.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrSample.SizeF = New System.Drawing.SizeF(111.4584!, 20.00001!)
        Me.XrSample.StylePriority.UseBorders = False
        Me.XrSample.StylePriority.UseFont = False
        Me.XrSample.Text = "XrSample"
        '
        'XrLabelDateRange
        '
        Me.XrLabelDateRange.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelDateRange.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelDateRange.LocationFloat = New DevExpress.Utils.PointFloat(462.0564!, 4.000025!)
        Me.XrLabelDateRange.Name = "XrLabelDateRange"
        Me.XrLabelDateRange.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelDateRange.SizeF = New System.Drawing.SizeF(183.9433!, 19.99998!)
        Me.XrLabelDateRange.StylePriority.UseBorders = False
        Me.XrLabelDateRange.StylePriority.UseFont = False
        Me.XrLabelDateRange.StylePriority.UseTextAlignment = False
        Me.XrLabelDateRange.Text = "Date range"
        Me.XrLabelDateRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrLabelSample
        '
        Me.XrLabelSample.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelSample.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelSample.LocationFloat = New DevExpress.Utils.PointFloat(194.2079!, 4.000028!)
        Me.XrLabelSample.Name = "XrLabelSample"
        Me.XrLabelSample.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelSample.SizeF = New System.Drawing.SizeF(111.4584!, 20.0!)
        Me.XrLabelSample.StylePriority.UseBorders = False
        Me.XrLabelSample.StylePriority.UseFont = False
        Me.XrLabelSample.Text = "Sample Type"
        '
        'XrTestName
        '
        Me.XrTestName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrTestName.Font = New System.Drawing.Font("Verdana", 9.75!)
        Me.XrTestName.LocationFloat = New DevExpress.Utils.PointFloat(10.00004!, 25.00003!)
        Me.XrTestName.Name = "XrTestName"
        Me.XrTestName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTestName.SizeF = New System.Drawing.SizeF(179.1667!, 20.00001!)
        Me.XrTestName.StylePriority.UseBorders = False
        Me.XrTestName.StylePriority.UseFont = False
        Me.XrTestName.Text = "XrTestName"
        '
        'XrLabelTestName
        '
        Me.XrLabelTestName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelTestName.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelTestName.LocationFloat = New DevExpress.Utils.PointFloat(10.00004!, 4.000025!)
        Me.XrLabelTestName.Name = "XrLabelTestName"
        Me.XrLabelTestName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelTestName.SizeF = New System.Drawing.SizeF(179.1666!, 20.0!)
        Me.XrLabelTestName.StylePriority.UseBorders = False
        Me.XrLabelTestName.StylePriority.UseFont = False
        Me.XrLabelTestName.Text = "Test"
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
        'GroupHeader1
        '
        Me.GroupHeader1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrHeaderLabel})
        Me.GroupHeader1.HeightF = 55.20833!
        Me.GroupHeader1.Name = "GroupHeader1"
        '
        'XrHeaderLabel
        '
        Me.XrHeaderLabel.BackColor = System.Drawing.Color.LightGray
        Me.XrHeaderLabel.BorderColor = System.Drawing.Color.DarkGray
        Me.XrHeaderLabel.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrHeaderLabel.Font = New System.Drawing.Font("Verdana", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrHeaderLabel.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 10.00001!)
        Me.XrHeaderLabel.Name = "XrHeaderLabel"
        Me.XrHeaderLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(10, 2, 3, 3, 100.0!)
        Me.XrHeaderLabel.SizeF = New System.Drawing.SizeF(650.0!, 25.0!)
        Me.XrHeaderLabel.StylePriority.UseBackColor = False
        Me.XrHeaderLabel.StylePriority.UseBorderColor = False
        Me.XrHeaderLabel.StylePriority.UseBorders = False
        Me.XrHeaderLabel.StylePriority.UseFont = False
        Me.XrHeaderLabel.StylePriority.UsePadding = False
        Me.XrHeaderLabel.StylePriority.UseTextAlignment = False
        Me.XrHeaderLabel.Text = "Cumulated QC Results by Test / Sample type"
        Me.XrHeaderLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'DetailReport
        '
        Me.DetailReport.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail1})
        Me.DetailReport.Level = 0
        Me.DetailReport.Name = "DetailReport"
        '
        'Detail1
        '
        Me.Detail1.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelControls})
        Me.Detail1.HeightF = 52.08333!
        Me.Detail1.Name = "Detail1"
        '
        'XrLabelControls
        '
        Me.XrLabelControls.BackColor = System.Drawing.Color.Gainsboro
        Me.XrLabelControls.BorderColor = System.Drawing.Color.DarkGray
        Me.XrLabelControls.Borders = CType((((DevExpress.XtraPrinting.BorderSide.Left Or DevExpress.XtraPrinting.BorderSide.Top) _
            Or DevExpress.XtraPrinting.BorderSide.Right) _
            Or DevExpress.XtraPrinting.BorderSide.Bottom), DevExpress.XtraPrinting.BorderSide)
        Me.XrLabelControls.Font = New System.Drawing.Font("Verdana", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelControls.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrLabelControls.Name = "XrLabelControls"
        Me.XrLabelControls.Padding = New DevExpress.XtraPrinting.PaddingInfo(10, 2, 3, 3, 100.0!)
        Me.XrLabelControls.SizeF = New System.Drawing.SizeF(650.0!, 25.0!)
        Me.XrLabelControls.StylePriority.UseBackColor = False
        Me.XrLabelControls.StylePriority.UseBorderColor = False
        Me.XrLabelControls.StylePriority.UseBorders = False
        Me.XrLabelControls.StylePriority.UseFont = False
        Me.XrLabelControls.StylePriority.UsePadding = False
        Me.XrLabelControls.StylePriority.UseTextAlignment = False
        Me.XrLabelControls.Text = "Controls"
        Me.XrLabelControls.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'QCCumulatedResultsByTestReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader1, Me.DetailReport})
        Me.Margins = New System.Drawing.Printing.Margins(63, 63, 59, 25)
        Me.PageHeight = 1169
        Me.PageWidth = 827
        Me.PaperKind = System.Drawing.Printing.PaperKind.A4
        Me.Version = "10.2"
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents GroupHeader1 As DevExpress.XtraReports.UI.GroupHeaderBand
    Protected WithEvents XrHeaderLabel As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrPanel1 As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrDateRange As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrSample As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelDateRange As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelSample As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrTestName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelTestName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents DetailReport As DevExpress.XtraReports.UI.DetailReportBand
    Friend WithEvents Detail1 As DevExpress.XtraReports.UI.DetailBand
    Protected WithEvents XrLabelControls As DevExpress.XtraReports.UI.XRLabel
End Class
