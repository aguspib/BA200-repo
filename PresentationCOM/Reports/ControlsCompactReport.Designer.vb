<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class ControlsCompactReport
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
        Me.XrTable = New DevExpress.XtraReports.UI.XRTable()
        Me.XrTableRow = New DevExpress.XtraReports.UI.XRTableRow()
        Me.XrTableCell1 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell2 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell3 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell4 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell5 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell6 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.XrTableCell7 = New DevExpress.XtraReports.UI.XRTableCell()
        Me.TopMargin = New DevExpress.XtraReports.UI.TopMarginBand()
        Me.BottomMargin = New DevExpress.XtraReports.UI.BottomMarginBand()
        Me.reportsDS = New Biosystems.Ax00.Types.ReportsDS()
        Me.GroupHeader = New DevExpress.XtraReports.UI.GroupHeaderBand()
        Me.XrWSStartDateTimeLabel = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrPanel = New DevExpress.XtraReports.UI.XRPanel()
        Me.XrLabelFlag = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelConcLimit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelTest = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelName = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelConc = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelUnit = New DevExpress.XtraReports.UI.XRLabel()
        Me.XrLabelRemarks = New DevExpress.XtraReports.UI.XRLabel()
        CType(Me.XrTable, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.reportsDS, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'Detail
        '
        Me.Detail.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrTable})
        Me.Detail.HeightF = 25.0!
        Me.Detail.Name = "Detail"
        Me.Detail.Padding = New DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100.0!)
        Me.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        '
        'XrTable
        '
        Me.XrTable.Font = New System.Drawing.Font("Times New Roman", 9.75!)
        Me.XrTable.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 0.0!)
        Me.XrTable.Name = "XrTable"
        Me.XrTable.Rows.AddRange(New DevExpress.XtraReports.UI.XRTableRow() {Me.XrTableRow})
        Me.XrTable.SizeF = New System.Drawing.SizeF(725.9999!, 25.0!)
        Me.XrTable.StylePriority.UseFont = False
        '
        'XrTableRow
        '
        Me.XrTableRow.Cells.AddRange(New DevExpress.XtraReports.UI.XRTableCell() {Me.XrTableCell1, Me.XrTableCell2, Me.XrTableCell3, Me.XrTableCell4, Me.XrTableCell5, Me.XrTableCell6, Me.XrTableCell7})
        Me.XrTableRow.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrTableRow.Name = "XrTableRow"
        Me.XrTableRow.StylePriority.UseFont = False
        Me.XrTableRow.Weight = 11.5R
        '
        'XrTableCell1
        '
        Me.XrTableCell1.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ControlsResultsDetails.Name")})
        Me.XrTableCell1.Name = "XrTableCell1"
        Me.XrTableCell1.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell1.StylePriority.UsePadding = False
        Me.XrTableCell1.StylePriority.UseTextAlignment = False
        Me.XrTableCell1.Text = "XrTableCell1"
        Me.XrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell1.Weight = 0.3346567942873564R
        '
        'XrTableCell2
        '
        Me.XrTableCell2.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ControlsResultsDetails.TestName")})
        Me.XrTableCell2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrTableCell2.Name = "XrTableCell2"
        Me.XrTableCell2.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell2.StylePriority.UseFont = False
        Me.XrTableCell2.StylePriority.UsePadding = False
        Me.XrTableCell2.StylePriority.UseTextAlignment = False
        Me.XrTableCell2.Text = "XrTableCell2"
        Me.XrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell2.Weight = 0.24321936018449314R
        '
        'XrTableCell3
        '
        Me.XrTableCell3.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ControlsResultsDetails.Concentration")})
        Me.XrTableCell3.Name = "XrTableCell3"
        Me.XrTableCell3.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell3.StylePriority.UsePadding = False
        Me.XrTableCell3.StylePriority.UseTextAlignment = False
        Me.XrTableCell3.Text = "XrTableCell3"
        Me.XrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell3.Weight = 0.11898913647051736R
        '
        'XrTableCell4
        '
        Me.XrTableCell4.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ControlsResultsDetails.MeasureUnit")})
        Me.XrTableCell4.Name = "XrTableCell4"
        Me.XrTableCell4.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell4.StylePriority.UsePadding = False
        Me.XrTableCell4.StylePriority.UseTextAlignment = False
        Me.XrTableCell4.Text = "XrTableCell4"
        Me.XrTableCell4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell4.Weight = 0.11396046400019109R
        '
        'XrTableCell5
        '
        Me.XrTableCell5.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ControlsResultsDetails.Flags")})
        Me.XrTableCell5.Name = "XrTableCell5"
        Me.XrTableCell5.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell5.StylePriority.UsePadding = False
        Me.XrTableCell5.StylePriority.UseTextAlignment = False
        Me.XrTableCell5.Text = "XrTableCell5"
        Me.XrTableCell5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell5.Weight = 0.025497673359373946R
        '
        'XrTableCell6
        '
        Me.XrTableCell6.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ControlsResultsDetails.ConcentrationLimits")})
        Me.XrTableCell6.Name = "XrTableCell6"
        Me.XrTableCell6.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrTableCell6.StylePriority.UsePadding = False
        Me.XrTableCell6.StylePriority.UseTextAlignment = False
        Me.XrTableCell6.Text = "XrTableCell6"
        Me.XrTableCell6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell6.Weight = 0.16658479258396358R
        '
        'XrTableCell7
        '
        Me.XrTableCell7.CanShrink = True
        Me.XrTableCell7.DataBindings.AddRange(New DevExpress.XtraReports.UI.XRBinding() {New DevExpress.XtraReports.UI.XRBinding("Text", Nothing, "ControlsResultsDetails.Remarks")})
        Me.XrTableCell7.Font = New System.Drawing.Font("Verdana", 7.0!)
        Me.XrTableCell7.Name = "XrTableCell7"
        Me.XrTableCell7.StylePriority.UseFont = False
        Me.XrTableCell7.StylePriority.UseTextAlignment = False
        Me.XrTableCell7.Text = "XrTableCell7"
        Me.XrTableCell7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft
        Me.XrTableCell7.Weight = 0.23117878709636366R
        '
        'TopMargin
        '
        Me.TopMargin.HeightF = 86.0!
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
        'reportsDS
        '
        Me.reportsDS.DataSetName = "ReportsDS"
        Me.reportsDS.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'GroupHeader
        '
        Me.GroupHeader.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrWSStartDateTimeLabel, Me.XrPanel})
        Me.GroupHeader.HeightF = 60.0!
        Me.GroupHeader.Name = "GroupHeader"
        Me.GroupHeader.RepeatEveryPage = True
        '
        'XrWSStartDateTimeLabel
        '
        Me.XrWSStartDateTimeLabel.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrWSStartDateTimeLabel.CanGrow = False
        Me.XrWSStartDateTimeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XrWSStartDateTimeLabel.LocationFloat = New DevExpress.Utils.PointFloat(499.92!, 0.0!)
        Me.XrWSStartDateTimeLabel.Name = "XrWSStartDateTimeLabel"
        Me.XrWSStartDateTimeLabel.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrWSStartDateTimeLabel.SizeF = New System.Drawing.SizeF(227.08!, 15.0!)
        Me.XrWSStartDateTimeLabel.StylePriority.UseBorders = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseFont = False
        Me.XrWSStartDateTimeLabel.StylePriority.UseTextAlignment = False
        Me.XrWSStartDateTimeLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
        '
        'XrPanel
        '
        Me.XrPanel.Borders = DevExpress.XtraPrinting.BorderSide.Bottom
        Me.XrPanel.BorderWidth = 1
        Me.XrPanel.Controls.AddRange(New DevExpress.XtraReports.UI.XRControl() {Me.XrLabelFlag, Me.XrLabelConcLimit, Me.XrLabelTest, Me.XrLabelName, Me.XrLabelConc, Me.XrLabelUnit, Me.XrLabelRemarks})
        Me.XrPanel.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 33.0!)
        Me.XrPanel.Name = "XrPanel"
        Me.XrPanel.SizeF = New System.Drawing.SizeF(726.0!, 21.5!)
        Me.XrPanel.StylePriority.UseBorders = False
        Me.XrPanel.StylePriority.UseBorderWidth = False
        '
        'XrLabelFlag
        '
        Me.XrLabelFlag.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelFlag.CanGrow = False
        Me.XrLabelFlag.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelFlag.LocationFloat = New DevExpress.Utils.PointFloat(476.9999!, 1.499962!)
        Me.XrLabelFlag.Name = "XrLabelFlag"
        Me.XrLabelFlag.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelFlag.SizeF = New System.Drawing.SizeF(15.0!, 20.0!)
        Me.XrLabelFlag.StylePriority.UseBorders = False
        Me.XrLabelFlag.StylePriority.UseFont = False
        Me.XrLabelFlag.StylePriority.UsePadding = False
        Me.XrLabelFlag.StylePriority.UseTextAlignment = False
        Me.XrLabelFlag.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelConcLimit
        '
        Me.XrLabelConcLimit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelConcLimit.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelConcLimit.LocationFloat = New DevExpress.Utils.PointFloat(491.9999!, 1.499962!)
        Me.XrLabelConcLimit.Name = "XrLabelConcLimit"
        Me.XrLabelConcLimit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelConcLimit.SizeF = New System.Drawing.SizeF(98.0!, 20.00001!)
        Me.XrLabelConcLimit.StylePriority.UseBorders = False
        Me.XrLabelConcLimit.StylePriority.UseFont = False
        Me.XrLabelConcLimit.StylePriority.UsePadding = False
        Me.XrLabelConcLimit.StylePriority.UseTextAlignment = False
        Me.XrLabelConcLimit.Text = "Límites Conc."
        Me.XrLabelConcLimit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelTest
        '
        Me.XrLabelTest.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelTest.CanGrow = False
        Me.XrLabelTest.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelTest.LocationFloat = New DevExpress.Utils.PointFloat(196.8749!, 1.500017!)
        Me.XrLabelTest.Name = "XrLabelTest"
        Me.XrLabelTest.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelTest.SizeF = New System.Drawing.SizeF(143.0833!, 20.00001!)
        Me.XrLabelTest.StylePriority.UseBorders = False
        Me.XrLabelTest.StylePriority.UseFont = False
        Me.XrLabelTest.StylePriority.UsePadding = False
        Me.XrLabelTest.StylePriority.UseTextAlignment = False
        Me.XrLabelTest.Text = "Técnica"
        Me.XrLabelTest.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelName
        '
        Me.XrLabelName.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelName.CanGrow = False
        Me.XrLabelName.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelName.LocationFloat = New DevExpress.Utils.PointFloat(0.0!, 1.500017!)
        Me.XrLabelName.Multiline = True
        Me.XrLabelName.Name = "XrLabelName"
        Me.XrLabelName.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelName.SizeF = New System.Drawing.SizeF(196.8749!, 20.00003!)
        Me.XrLabelName.StylePriority.UseBorders = False
        Me.XrLabelName.StylePriority.UseFont = False
        Me.XrLabelName.StylePriority.UsePadding = False
        Me.XrLabelName.StylePriority.UseTextAlignment = False
        Me.XrLabelName.Text = "Nombre (Lote)"
        Me.XrLabelName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelConc
        '
        Me.XrLabelConc.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelConc.CanGrow = False
        Me.XrLabelConc.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelConc.LocationFloat = New DevExpress.Utils.PointFloat(339.9582!, 1.499987!)
        Me.XrLabelConc.Name = "XrLabelConc"
        Me.XrLabelConc.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelConc.SizeF = New System.Drawing.SizeF(70.0!, 20.00001!)
        Me.XrLabelConc.StylePriority.UseBorders = False
        Me.XrLabelConc.StylePriority.UseFont = False
        Me.XrLabelConc.StylePriority.UsePadding = False
        Me.XrLabelConc.StylePriority.UseTextAlignment = False
        Me.XrLabelConc.Text = "Conc."
        Me.XrLabelConc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelUnit
        '
        Me.XrLabelUnit.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelUnit.CanGrow = False
        Me.XrLabelUnit.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelUnit.LocationFloat = New DevExpress.Utils.PointFloat(409.9582!, 1.500003!)
        Me.XrLabelUnit.Name = "XrLabelUnit"
        Me.XrLabelUnit.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelUnit.SizeF = New System.Drawing.SizeF(67.04175!, 20.00001!)
        Me.XrLabelUnit.StylePriority.UseBorders = False
        Me.XrLabelUnit.StylePriority.UseFont = False
        Me.XrLabelUnit.StylePriority.UsePadding = False
        Me.XrLabelUnit.StylePriority.UseTextAlignment = False
        Me.XrLabelUnit.Text = "Unidad"
        Me.XrLabelUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'XrLabelRemarks
        '
        Me.XrLabelRemarks.Borders = DevExpress.XtraPrinting.BorderSide.None
        Me.XrLabelRemarks.CanGrow = False
        Me.XrLabelRemarks.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.XrLabelRemarks.LocationFloat = New DevExpress.Utils.PointFloat(589.9999!, 1.500003!)
        Me.XrLabelRemarks.Name = "XrLabelRemarks"
        Me.XrLabelRemarks.Padding = New DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100.0!)
        Me.XrLabelRemarks.SizeF = New System.Drawing.SizeF(136.0!, 21.62498!)
        Me.XrLabelRemarks.StylePriority.UseBorders = False
        Me.XrLabelRemarks.StylePriority.UseFont = False
        Me.XrLabelRemarks.StylePriority.UseTextAlignment = False
        Me.XrLabelRemarks.Text = "Observaciones"
        Me.XrLabelRemarks.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
        '
        'ControlsCompactReport
        '
        Me.Bands.AddRange(New DevExpress.XtraReports.UI.Band() {Me.Detail, Me.TopMargin, Me.BottomMargin, Me.GroupHeader})
        Me.DataMember = "ControlsResultsDetails"
        Me.DataSource = Me.reportsDS
        Me.Font = New System.Drawing.Font("Times New Roman", 9.75!, System.Drawing.FontStyle.Italic)
        Me.Margins = New System.Drawing.Printing.Margins(50, 50, 86, 25)
        Me.PageHeight = 1169
        Me.PageWidth = 827
        Me.PaperKind = System.Drawing.Printing.PaperKind.A4
        Me.Version = "10.2"
        CType(Me.XrTable, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.reportsDS, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub
    Friend WithEvents Detail As DevExpress.XtraReports.UI.DetailBand
    Friend WithEvents TopMargin As DevExpress.XtraReports.UI.TopMarginBand
    Friend WithEvents BottomMargin As DevExpress.XtraReports.UI.BottomMarginBand
    Friend WithEvents reportsDS As Biosystems.Ax00.Types.ReportsDS
    Friend WithEvents XrTable As DevExpress.XtraReports.UI.XRTable
    Friend WithEvents XrTableRow As DevExpress.XtraReports.UI.XRTableRow
    Friend WithEvents XrTableCell1 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell2 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell3 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell4 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell5 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell6 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents XrTableCell7 As DevExpress.XtraReports.UI.XRTableCell
    Friend WithEvents GroupHeader As DevExpress.XtraReports.UI.GroupHeaderBand
    Friend WithEvents XrPanel As DevExpress.XtraReports.UI.XRPanel
    Friend WithEvents XrLabelFlag As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelConcLimit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelTest As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelName As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelConc As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelUnit As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrLabelRemarks As DevExpress.XtraReports.UI.XRLabel
    Friend WithEvents XrWSStartDateTimeLabel As DevExpress.XtraReports.UI.XRLabel
End Class
