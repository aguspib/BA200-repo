﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IResults
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                ReleaseElement()
                components.Dispose()
                'GC.SuppressFinalize(Me)
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim GridView1 As DevExpress.XtraGrid.Views.Grid.GridView
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IResults))
        Dim DataGridViewCellStyle61 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle62 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle63 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle64 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle65 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle66 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle67 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle68 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle69 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle70 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle71 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle72 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle73 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle74 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle75 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle76 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle77 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle78 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle79 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle80 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle81 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle82 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle83 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle84 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle85 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle86 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle87 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle88 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle89 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle90 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.Cycle = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.Abs1 = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.Abs2 = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.Diff = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.bsPanel4 = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.SendManRepButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.OffSystemResultsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSamplesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.PrintCompactReportButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.PrintReportButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.SummaryButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.PrintSampleButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.ExportButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsXlsresults = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsPanel2 = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.ExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.bsProgTestToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.OrderToExportCheckBox = New DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit()
        Me.OrderToPrintCheckBox = New DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit()
        Me.STATImage = New DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit()
        Me.bsResultsFormLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTestDetailsTabControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl()
        Me.bsSamplesTab = New System.Windows.Forms.TabPage()
        Me.bsSamplesListDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsTestsTabTage = New System.Windows.Forms.TabPage()
        Me.bsTestsListDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsResultsTabControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl()
        Me.bsBlanksTabPage = New System.Windows.Forms.TabPage()
        Me.bsBlanksDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsCalibratorsTabPage = New System.Windows.Forms.TabPage()
        Me.bsCalibratorsDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsControlsTabPage = New System.Windows.Forms.TabPage()
        Me.bsControlsDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.XtraSamplesTabPage = New System.Windows.Forms.TabPage()
        Me.SamplesXtraGrid = New DevExpress.XtraGrid.GridControl()
        Me.SamplesXtraGridView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.ToolTipController1 = New DevExpress.Utils.ToolTipController(Me.components)
        Me.bsResultFormGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsTestPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.PrintTestBlankButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.PrintTestCtrlButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.PrintTestButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSamplesResultsTabControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl()
        Me.bsExperimentalsTabPage = New System.Windows.Forms.TabPage()
        Me.bsExperimentalsDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.AlarmsDS1 = New Biosystems.Ax00.Types.AlarmsDS()
        GridView1 = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(GridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsPanel4.SuspendLayout()
        Me.bsSamplesPanel.SuspendLayout()
        Me.bsPanel2.SuspendLayout()
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.OrderToExportCheckBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.OrderToPrintCheckBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.STATImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsTestDetailsTabControl.SuspendLayout()
        Me.bsSamplesTab.SuspendLayout()
        CType(Me.bsSamplesListDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsTestsTabTage.SuspendLayout()
        CType(Me.bsTestsListDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsResultsTabControl.SuspendLayout()
        Me.bsBlanksTabPage.SuspendLayout()
        CType(Me.bsBlanksDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsCalibratorsTabPage.SuspendLayout()
        CType(Me.bsCalibratorsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsControlsTabPage.SuspendLayout()
        CType(Me.bsControlsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.XtraSamplesTabPage.SuspendLayout()
        CType(Me.SamplesXtraGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SamplesXtraGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsResultFormGroupBox.SuspendLayout()
        Me.bsTestPanel.SuspendLayout()
        Me.bsSamplesResultsTabControl.SuspendLayout()
        Me.bsExperimentalsTabPage.SuspendLayout()
        CType(Me.bsExperimentalsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AlarmsDS1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GridView1
        '
        GridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.MintCream
        GridView1.Appearance.FocusedRow.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        GridView1.Appearance.FocusedRow.Options.UseBackColor = True
        GridView1.Appearance.FocusedRow.Options.UseFont = True
        GridView1.Appearance.SelectedRow.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        GridView1.Appearance.SelectedRow.Options.UseFont = True
        GridView1.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {Me.Cycle, Me.Abs1, Me.Abs2, Me.Diff})
        GridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None
        GridView1.GroupPanelText = "Cycles"
        GridView1.Name = "GridView1"
        GridView1.OptionsSelection.EnableAppearanceFocusedCell = False
        GridView1.OptionsView.EnableAppearanceEvenRow = True
        GridView1.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow
        '
        'Cycle
        '
        Me.Cycle.Caption = "Cycle"
        Me.Cycle.FieldName = "Cycle"
        Me.Cycle.Name = "Cycle"
        Me.Cycle.OptionsColumn.AllowEdit = False
        Me.Cycle.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
        Me.Cycle.OptionsColumn.AllowMove = False
        Me.Cycle.OptionsColumn.AllowSize = False
        Me.Cycle.Visible = True
        Me.Cycle.VisibleIndex = 0
        Me.Cycle.Width = 35
        '
        'Abs1
        '
        Me.Abs1.AppearanceCell.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Abs1.AppearanceCell.Options.UseFont = True
        Me.Abs1.Caption = "Abs1"
        Me.Abs1.DisplayFormat.FormatString = "0.0000"
        Me.Abs1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.Abs1.FieldName = "Abs1"
        Me.Abs1.Name = "Abs1"
        Me.Abs1.OptionsColumn.AllowEdit = False
        Me.Abs1.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
        Me.Abs1.OptionsColumn.AllowMove = False
        Me.Abs1.OptionsColumn.AllowSize = False
        Me.Abs1.Visible = True
        Me.Abs1.VisibleIndex = 1
        Me.Abs1.Width = 53
        '
        'Abs2
        '
        Me.Abs2.Caption = "Abs2"
        Me.Abs2.DisplayFormat.FormatString = "0.0000"
        Me.Abs2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.Abs2.FieldName = "Abs2"
        Me.Abs2.Name = "Abs2"
        Me.Abs2.OptionsColumn.AllowEdit = False
        Me.Abs2.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
        Me.Abs2.OptionsColumn.AllowMove = False
        Me.Abs2.OptionsColumn.AllowSize = False
        Me.Abs2.Visible = True
        Me.Abs2.VisibleIndex = 2
        Me.Abs2.Width = 54
        '
        'Diff
        '
        Me.Diff.Caption = "Diff"
        Me.Diff.DisplayFormat.FormatString = "0.0000"
        Me.Diff.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.Diff.FieldName = "Diff"
        Me.Diff.Name = "Diff"
        Me.Diff.OptionsColumn.AllowEdit = False
        Me.Diff.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
        Me.Diff.OptionsColumn.AllowMove = False
        Me.Diff.OptionsColumn.AllowSize = False
        Me.Diff.Visible = True
        Me.Diff.VisibleIndex = 3
        Me.Diff.Width = 57
        '
        'bsPanel4
        '
        Me.bsPanel4.Controls.Add(Me.SendManRepButton)
        Me.bsPanel4.Controls.Add(Me.OffSystemResultsButton)
        Me.bsPanel4.Controls.Add(Me.ExportButton)
        Me.bsPanel4.Controls.Add(Me.bsXlsresults)
        Me.bsPanel4.Location = New System.Drawing.Point(239, 572)
        Me.bsPanel4.Name = "bsPanel4"
        Me.bsPanel4.Size = New System.Drawing.Size(719, 32)
        Me.bsPanel4.TabIndex = 42
        '
        'SendManRepButton
        '
        Me.SendManRepButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.SendManRepButton.Location = New System.Drawing.Point(687, 0)
        Me.SendManRepButton.Name = "SendManRepButton"
        Me.SendManRepButton.Size = New System.Drawing.Size(32, 32)
        Me.SendManRepButton.TabIndex = 170
        Me.SendManRepButton.UseVisualStyleBackColor = True
        '
        'OffSystemResultsButton
        '
        Me.OffSystemResultsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.OffSystemResultsButton.Location = New System.Drawing.Point(650, 0)
        Me.OffSystemResultsButton.Name = "OffSystemResultsButton"
        Me.OffSystemResultsButton.Size = New System.Drawing.Size(32, 32)
        Me.OffSystemResultsButton.TabIndex = 169
        Me.OffSystemResultsButton.UseVisualStyleBackColor = True
        '
        'bsSamplesPanel
        '
        Me.bsSamplesPanel.Controls.Add(Me.PrintCompactReportButton)
        Me.bsSamplesPanel.Controls.Add(Me.PrintReportButton)
        Me.bsSamplesPanel.Controls.Add(Me.SummaryButton)
        Me.bsSamplesPanel.Controls.Add(Me.PrintSampleButton)
        Me.bsSamplesPanel.Location = New System.Drawing.Point(10, 572)
        Me.bsSamplesPanel.Name = "bsSamplesPanel"
        Me.bsSamplesPanel.Size = New System.Drawing.Size(223, 32)
        Me.bsSamplesPanel.TabIndex = 39
        '
        'PrintCompactReportButton
        '
        Me.PrintCompactReportButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PrintCompactReportButton.Location = New System.Drawing.Point(38, 0)
        Me.PrintCompactReportButton.Name = "PrintCompactReportButton"
        Me.PrintCompactReportButton.Size = New System.Drawing.Size(32, 32)
        Me.PrintCompactReportButton.TabIndex = 170
        Me.PrintCompactReportButton.UseVisualStyleBackColor = True
        '
        'PrintReportButton
        '
        Me.PrintReportButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PrintReportButton.Location = New System.Drawing.Point(0, 0)
        Me.PrintReportButton.Name = "PrintReportButton"
        Me.PrintReportButton.Size = New System.Drawing.Size(32, 32)
        Me.PrintReportButton.TabIndex = 169
        Me.PrintReportButton.UseVisualStyleBackColor = True
        '
        'SummaryButton
        '
        Me.SummaryButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.SummaryButton.Location = New System.Drawing.Point(153, 0)
        Me.SummaryButton.Name = "SummaryButton"
        Me.SummaryButton.Size = New System.Drawing.Size(32, 32)
        Me.SummaryButton.TabIndex = 168
        Me.SummaryButton.UseVisualStyleBackColor = True
        '
        'PrintSampleButton
        '
        Me.PrintSampleButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PrintSampleButton.Location = New System.Drawing.Point(191, 0)
        Me.PrintSampleButton.Name = "PrintSampleButton"
        Me.PrintSampleButton.Size = New System.Drawing.Size(32, 32)
        Me.PrintSampleButton.TabIndex = 167
        Me.PrintSampleButton.UseVisualStyleBackColor = True
        '
        'ExportButton
        '
        Me.ExportButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExportButton.Location = New System.Drawing.Point(613, 0)
        Me.ExportButton.Name = "ExportButton"
        Me.ExportButton.Size = New System.Drawing.Size(32, 32)
        Me.ExportButton.TabIndex = 168
        Me.ExportButton.UseVisualStyleBackColor = True
        '
        'bsXlsresults
        '
        Me.bsXlsresults.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsXlsresults.Location = New System.Drawing.Point(0, 0)
        Me.bsXlsresults.Name = "bsXlsresults"
        Me.bsXlsresults.Size = New System.Drawing.Size(32, 32)
        Me.bsXlsresults.TabIndex = 167
        Me.bsXlsresults.UseVisualStyleBackColor = True
        '
        'bsPanel2
        '
        Me.bsPanel2.Controls.Add(Me.ExitButton)
        Me.bsPanel2.Location = New System.Drawing.Point(895, 617)
        Me.bsPanel2.Name = "bsPanel2"
        Me.bsPanel2.Size = New System.Drawing.Size(69, 32)
        Me.bsPanel2.TabIndex = 34
        '
        'ExitButton
        '
        Me.ExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExitButton.Location = New System.Drawing.Point(37, 0)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(32, 32)
        Me.ExitButton.TabIndex = 167
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'bsErrorProvider1
        '
        Me.bsErrorProvider1.ContainerControl = Me
        '
        'OrderToExportCheckBox
        '
        Me.OrderToExportCheckBox.AutoHeight = False
        Me.OrderToExportCheckBox.Caption = ""
        Me.OrderToExportCheckBox.Name = "OrderToExportCheckBox"
        '
        'OrderToPrintCheckBox
        '
        Me.OrderToPrintCheckBox.AutoHeight = False
        Me.OrderToPrintCheckBox.Caption = ""
        Me.OrderToPrintCheckBox.Name = "OrderToPrintCheckBox"
        '
        'STATImage
        '
        Me.STATImage.InitialImage = CType(resources.GetObject("STATImage.InitialImage"), System.Drawing.Image)
        Me.STATImage.Name = "STATImage"
        '
        'bsResultsFormLabel
        '
        Me.bsResultsFormLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsResultsFormLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsResultsFormLabel.ForeColor = System.Drawing.Color.Black
        Me.bsResultsFormLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsResultsFormLabel.Name = "bsResultsFormLabel"
        Me.bsResultsFormLabel.Size = New System.Drawing.Size(948, 20)
        Me.bsResultsFormLabel.TabIndex = 0
        Me.bsResultsFormLabel.Text = "Test Results"
        Me.bsResultsFormLabel.Title = True
        '
        'bsTestDetailsTabControl
        '
        Me.bsTestDetailsTabControl.Controls.Add(Me.bsSamplesTab)
        Me.bsTestDetailsTabControl.Controls.Add(Me.bsTestsTabTage)
        Me.bsTestDetailsTabControl.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestDetailsTabControl.Location = New System.Drawing.Point(10, 40)
        Me.bsTestDetailsTabControl.Name = "bsTestDetailsTabControl"
        Me.bsTestDetailsTabControl.SelectedIndex = 0
        Me.bsTestDetailsTabControl.Size = New System.Drawing.Size(225, 524)
        Me.bsTestDetailsTabControl.TabIndex = 0
        '
        'bsSamplesTab
        '
        Me.bsSamplesTab.BackColor = System.Drawing.Color.Gainsboro
        Me.bsSamplesTab.Controls.Add(Me.bsSamplesListDataGridView)
        Me.bsSamplesTab.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesTab.Location = New System.Drawing.Point(4, 22)
        Me.bsSamplesTab.Name = "bsSamplesTab"
        Me.bsSamplesTab.Padding = New System.Windows.Forms.Padding(3)
        Me.bsSamplesTab.Size = New System.Drawing.Size(217, 498)
        Me.bsSamplesTab.TabIndex = 0
        Me.bsSamplesTab.Text = "Samples"
        '
        'bsSamplesListDataGridView
        '
        Me.bsSamplesListDataGridView.AllowUserToAddRows = False
        Me.bsSamplesListDataGridView.AllowUserToDeleteRows = False
        Me.bsSamplesListDataGridView.AllowUserToResizeColumns = False
        Me.bsSamplesListDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle61.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle61.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle61.SelectionBackColor = System.Drawing.Color.DodgerBlue
        Me.bsSamplesListDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle61
        Me.bsSamplesListDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.bsSamplesListDataGridView.BackgroundColor = System.Drawing.Color.White
        Me.bsSamplesListDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsSamplesListDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        Me.bsSamplesListDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle62.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle62.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle62.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle62.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle62.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle62.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsSamplesListDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle62
        Me.bsSamplesListDataGridView.ColumnHeadersHeight = 20
        Me.bsSamplesListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle63.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle63.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle63.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle63.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle63.SelectionBackColor = System.Drawing.Color.DodgerBlue
        DataGridViewCellStyle63.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle63.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsSamplesListDataGridView.DefaultCellStyle = DataGridViewCellStyle63
        Me.bsSamplesListDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsSamplesListDataGridView.EnterToTab = False
        Me.bsSamplesListDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsSamplesListDataGridView.Location = New System.Drawing.Point(5, 5)
        Me.bsSamplesListDataGridView.MultiSelect = False
        Me.bsSamplesListDataGridView.Name = "bsSamplesListDataGridView"
        Me.bsSamplesListDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle64.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle64.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle64.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle64.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle64.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle64.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsSamplesListDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle64
        Me.bsSamplesListDataGridView.RowHeadersVisible = False
        Me.bsSamplesListDataGridView.RowHeadersWidth = 20
        Me.bsSamplesListDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle65.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle65.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle65.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle65.SelectionBackColor = System.Drawing.Color.DodgerBlue
        DataGridViewCellStyle65.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.bsSamplesListDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle65
        Me.bsSamplesListDataGridView.RowTemplate.Height = 30
        Me.bsSamplesListDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsSamplesListDataGridView.Size = New System.Drawing.Size(208, 488)
        Me.bsSamplesListDataGridView.TabIndex = 40
        Me.bsSamplesListDataGridView.TabToEnter = False
        '
        'bsTestsTabTage
        '
        Me.bsTestsTabTage.BackColor = System.Drawing.Color.Gainsboro
        Me.bsTestsTabTage.Controls.Add(Me.bsTestsListDataGridView)
        Me.bsTestsTabTage.ForeColor = System.Drawing.Color.Black
        Me.bsTestsTabTage.Location = New System.Drawing.Point(4, 22)
        Me.bsTestsTabTage.Name = "bsTestsTabTage"
        Me.bsTestsTabTage.Padding = New System.Windows.Forms.Padding(3)
        Me.bsTestsTabTage.Size = New System.Drawing.Size(217, 498)
        Me.bsTestsTabTage.TabIndex = 1
        Me.bsTestsTabTage.Text = "Tests"
        '
        'bsTestsListDataGridView
        '
        Me.bsTestsListDataGridView.AllowUserToAddRows = False
        Me.bsTestsListDataGridView.AllowUserToDeleteRows = False
        Me.bsTestsListDataGridView.AllowUserToResizeColumns = False
        Me.bsTestsListDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle66.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle66.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle66.SelectionBackColor = System.Drawing.Color.DodgerBlue
        Me.bsTestsListDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle66
        Me.bsTestsListDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.bsTestsListDataGridView.BackgroundColor = System.Drawing.Color.White
        Me.bsTestsListDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsTestsListDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        Me.bsTestsListDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle67.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle67.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle67.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle67.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle67.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle67.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestsListDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle67
        Me.bsTestsListDataGridView.ColumnHeadersHeight = 20
        Me.bsTestsListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle68.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle68.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle68.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle68.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle68.SelectionBackColor = System.Drawing.Color.DodgerBlue
        DataGridViewCellStyle68.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle68.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestsListDataGridView.DefaultCellStyle = DataGridViewCellStyle68
        Me.bsTestsListDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsTestsListDataGridView.EnterToTab = False
        Me.bsTestsListDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsTestsListDataGridView.Location = New System.Drawing.Point(5, 5)
        Me.bsTestsListDataGridView.MultiSelect = False
        Me.bsTestsListDataGridView.Name = "bsTestsListDataGridView"
        Me.bsTestsListDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle69.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestsListDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle69
        Me.bsTestsListDataGridView.RowHeadersVisible = False
        Me.bsTestsListDataGridView.RowHeadersWidth = 20
        Me.bsTestsListDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle70.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle70.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle70.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle70.SelectionBackColor = System.Drawing.Color.DodgerBlue
        DataGridViewCellStyle70.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.bsTestsListDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle70
        Me.bsTestsListDataGridView.RowTemplate.Height = 30
        Me.bsTestsListDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsTestsListDataGridView.Size = New System.Drawing.Size(208, 488)
        Me.bsTestsListDataGridView.TabIndex = 6
        Me.bsTestsListDataGridView.TabToEnter = False
        '
        'bsResultsTabControl
        '
        Me.bsResultsTabControl.Controls.Add(Me.bsBlanksTabPage)
        Me.bsResultsTabControl.Controls.Add(Me.bsCalibratorsTabPage)
        Me.bsResultsTabControl.Controls.Add(Me.bsControlsTabPage)
        Me.bsResultsTabControl.Controls.Add(Me.XtraSamplesTabPage)
        Me.bsResultsTabControl.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsResultsTabControl.Location = New System.Drawing.Point(239, 40)
        Me.bsResultsTabControl.Name = "bsResultsTabControl"
        Me.bsResultsTabControl.SelectedIndex = 0
        Me.bsResultsTabControl.Size = New System.Drawing.Size(720, 524)
        Me.bsResultsTabControl.TabIndex = 10
        Me.bsResultsTabControl.Visible = False
        '
        'bsBlanksTabPage
        '
        Me.bsBlanksTabPage.BackColor = System.Drawing.Color.Gainsboro
        Me.bsBlanksTabPage.Controls.Add(Me.bsBlanksDataGridView)
        Me.bsBlanksTabPage.ForeColor = System.Drawing.Color.Black
        Me.bsBlanksTabPage.Location = New System.Drawing.Point(4, 22)
        Me.bsBlanksTabPage.Name = "bsBlanksTabPage"
        Me.bsBlanksTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.bsBlanksTabPage.Size = New System.Drawing.Size(712, 498)
        Me.bsBlanksTabPage.TabIndex = 0
        Me.bsBlanksTabPage.Text = "Blanks"
        '
        'bsBlanksDataGridView
        '
        Me.bsBlanksDataGridView.AllowUserToAddRows = False
        Me.bsBlanksDataGridView.AllowUserToDeleteRows = False
        DataGridViewCellStyle71.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle71.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle71.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.bsBlanksDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle71
        Me.bsBlanksDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.bsBlanksDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsBlanksDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsBlanksDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle72.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle72.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle72.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle72.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle72.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle72.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsBlanksDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle72
        Me.bsBlanksDataGridView.ColumnHeadersHeight = 30
        Me.bsBlanksDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle73.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle73.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle73.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle73.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle73.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle73.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle73.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsBlanksDataGridView.DefaultCellStyle = DataGridViewCellStyle73
        Me.bsBlanksDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsBlanksDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsBlanksDataGridView.EnterToTab = False
        Me.bsBlanksDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsBlanksDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.bsBlanksDataGridView.Name = "bsBlanksDataGridView"
        Me.bsBlanksDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle74.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle74.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle74.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle74.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle74.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle74.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsBlanksDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle74
        Me.bsBlanksDataGridView.RowHeadersVisible = False
        Me.bsBlanksDataGridView.RowHeadersWidth = 20
        Me.bsBlanksDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle75.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle75.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle75.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle75.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle75.SelectionForeColor = System.Drawing.Color.Black
        Me.bsBlanksDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle75
        Me.bsBlanksDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsBlanksDataGridView.Size = New System.Drawing.Size(706, 492)
        Me.bsBlanksDataGridView.TabIndex = 7
        Me.bsBlanksDataGridView.TabToEnter = False
        '
        'bsCalibratorsTabPage
        '
        Me.bsCalibratorsTabPage.BackColor = System.Drawing.Color.Gainsboro
        Me.bsCalibratorsTabPage.Controls.Add(Me.bsCalibratorsDataGridView)
        Me.bsCalibratorsTabPage.ForeColor = System.Drawing.Color.Black
        Me.bsCalibratorsTabPage.Location = New System.Drawing.Point(4, 22)
        Me.bsCalibratorsTabPage.Name = "bsCalibratorsTabPage"
        Me.bsCalibratorsTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.bsCalibratorsTabPage.Size = New System.Drawing.Size(712, 498)
        Me.bsCalibratorsTabPage.TabIndex = 1
        Me.bsCalibratorsTabPage.Text = "Calibrators"
        '
        'bsCalibratorsDataGridView
        '
        Me.bsCalibratorsDataGridView.AllowUserToAddRows = False
        Me.bsCalibratorsDataGridView.AllowUserToDeleteRows = False
        DataGridViewCellStyle76.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle76.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle76.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.bsCalibratorsDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle76
        Me.bsCalibratorsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.bsCalibratorsDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsCalibratorsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsCalibratorsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle77.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle77.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle77.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle77.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle77.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle77.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCalibratorsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle77
        Me.bsCalibratorsDataGridView.ColumnHeadersHeight = 30
        Me.bsCalibratorsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle78.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle78.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle78.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle78.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle78.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle78.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle78.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCalibratorsDataGridView.DefaultCellStyle = DataGridViewCellStyle78
        Me.bsCalibratorsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsCalibratorsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsCalibratorsDataGridView.EnterToTab = False
        Me.bsCalibratorsDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsCalibratorsDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.bsCalibratorsDataGridView.Name = "bsCalibratorsDataGridView"
        Me.bsCalibratorsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle79.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle79.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle79.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle79.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle79.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle79.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCalibratorsDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle79
        Me.bsCalibratorsDataGridView.RowHeadersVisible = False
        Me.bsCalibratorsDataGridView.RowHeadersWidth = 20
        Me.bsCalibratorsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle80.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle80.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle80.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle80.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle80.SelectionForeColor = System.Drawing.Color.Black
        Me.bsCalibratorsDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle80
        Me.bsCalibratorsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsCalibratorsDataGridView.Size = New System.Drawing.Size(706, 492)
        Me.bsCalibratorsDataGridView.TabIndex = 8
        Me.bsCalibratorsDataGridView.TabToEnter = False
        '
        'bsControlsTabPage
        '
        Me.bsControlsTabPage.BackColor = System.Drawing.Color.Gainsboro
        Me.bsControlsTabPage.Controls.Add(Me.bsControlsDataGridView)
        Me.bsControlsTabPage.ForeColor = System.Drawing.Color.Black
        Me.bsControlsTabPage.Location = New System.Drawing.Point(4, 22)
        Me.bsControlsTabPage.Name = "bsControlsTabPage"
        Me.bsControlsTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.bsControlsTabPage.Size = New System.Drawing.Size(712, 498)
        Me.bsControlsTabPage.TabIndex = 2
        Me.bsControlsTabPage.Text = "Controls"
        '
        'bsControlsDataGridView
        '
        Me.bsControlsDataGridView.AllowUserToAddRows = False
        Me.bsControlsDataGridView.AllowUserToDeleteRows = False
        DataGridViewCellStyle81.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle81.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle81.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.bsControlsDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle81
        Me.bsControlsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.bsControlsDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsControlsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsControlsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle82.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle82.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle82.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle82.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle82.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle82.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsControlsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle82
        Me.bsControlsDataGridView.ColumnHeadersHeight = 30
        Me.bsControlsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle83.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle83.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle83.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle83.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle83.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle83.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle83.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsControlsDataGridView.DefaultCellStyle = DataGridViewCellStyle83
        Me.bsControlsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsControlsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsControlsDataGridView.EnterToTab = False
        Me.bsControlsDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsControlsDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.bsControlsDataGridView.Name = "bsControlsDataGridView"
        Me.bsControlsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle84.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle84.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle84.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle84.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle84.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle84.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsControlsDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle84
        Me.bsControlsDataGridView.RowHeadersVisible = False
        Me.bsControlsDataGridView.RowHeadersWidth = 20
        Me.bsControlsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle85.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle85.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle85.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle85.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle85.SelectionForeColor = System.Drawing.Color.Black
        Me.bsControlsDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle85
        Me.bsControlsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsControlsDataGridView.Size = New System.Drawing.Size(706, 492)
        Me.bsControlsDataGridView.TabIndex = 9
        Me.bsControlsDataGridView.TabToEnter = False
        '
        'XtraSamplesTabPage
        '
        Me.XtraSamplesTabPage.BackColor = System.Drawing.Color.Gainsboro
        Me.XtraSamplesTabPage.Controls.Add(Me.SamplesXtraGrid)
        Me.XtraSamplesTabPage.Location = New System.Drawing.Point(4, 22)
        Me.XtraSamplesTabPage.Name = "XtraSamplesTabPage"
        Me.XtraSamplesTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.XtraSamplesTabPage.Size = New System.Drawing.Size(712, 498)
        Me.XtraSamplesTabPage.TabIndex = 4
        Me.XtraSamplesTabPage.Text = "XtraSamples"
        '
        'SamplesXtraGrid
        '
        Me.SamplesXtraGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SamplesXtraGrid.Location = New System.Drawing.Point(3, 3)
        Me.SamplesXtraGrid.LookAndFeel.UseWindowsXPTheme = True
        Me.SamplesXtraGrid.MainView = Me.SamplesXtraGridView
        Me.SamplesXtraGrid.Name = "SamplesXtraGrid"
        Me.SamplesXtraGrid.Size = New System.Drawing.Size(706, 492)
        Me.SamplesXtraGrid.TabIndex = 0
        Me.SamplesXtraGrid.ToolTipController = Me.ToolTipController1
        Me.SamplesXtraGrid.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.SamplesXtraGridView})
        '
        'SamplesXtraGridView
        '
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.SamplesXtraGridView.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.SamplesXtraGridView.Appearance.Empty.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.SamplesXtraGridView.Appearance.EvenRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.SamplesXtraGridView.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.SamplesXtraGridView.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.SamplesXtraGridView.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.SamplesXtraGridView.Appearance.FilterPanel.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FilterPanel.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent
        Me.SamplesXtraGridView.Appearance.FocusedCell.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FocusedRow.BackColor = System.Drawing.Color.Black
        Me.SamplesXtraGridView.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.SamplesXtraGridView.Appearance.FocusedRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FocusedRow.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.FooterPanel.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.SamplesXtraGridView.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.SamplesXtraGridView.Appearance.GroupButton.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.GroupButton.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.SamplesXtraGridView.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.SamplesXtraGridView.Appearance.GroupFooter.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.SamplesXtraGridView.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.SamplesXtraGridView.Appearance.GroupPanel.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.GroupPanel.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.SamplesXtraGridView.Appearance.GroupRow.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SamplesXtraGridView.Appearance.GroupRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.GroupRow.Options.UseFont = True
        Me.SamplesXtraGridView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.SamplesXtraGridView.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.SamplesXtraGridView.Appearance.HorzLine.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.OddRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.SamplesXtraGridView.Appearance.OddRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.SamplesXtraGridView.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.SamplesXtraGridView.Appearance.Preview.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.Preview.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.SamplesXtraGridView.Appearance.Row.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.SamplesXtraGridView.Appearance.RowSeparator.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.SelectedRow.BackColor = System.Drawing.Color.DimGray
        Me.SamplesXtraGridView.Appearance.SelectedRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.SamplesXtraGridView.Appearance.VertLine.Options.UseBackColor = True
        Me.SamplesXtraGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.SamplesXtraGridView.GridControl = Me.SamplesXtraGrid
        Me.SamplesXtraGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.SamplesXtraGridView.Name = "SamplesXtraGridView"
        Me.SamplesXtraGridView.OptionsCustomization.AllowColumnMoving = False
        Me.SamplesXtraGridView.OptionsCustomization.AllowFilter = False
        Me.SamplesXtraGridView.OptionsCustomization.AllowQuickHideColumns = False
        Me.SamplesXtraGridView.OptionsCustomization.AllowSort = False
        Me.SamplesXtraGridView.OptionsFind.AllowFindPanel = False
        Me.SamplesXtraGridView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.SamplesXtraGridView.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.SamplesXtraGridView.OptionsView.EnableAppearanceEvenRow = True
        Me.SamplesXtraGridView.OptionsView.EnableAppearanceOddRow = True
        Me.SamplesXtraGridView.OptionsView.ShowGroupPanel = False
        Me.SamplesXtraGridView.PaintStyleName = "WindowsXP"
        '
        'bsResultFormGroupBox
        '
        Me.bsResultFormGroupBox.Controls.Add(Me.bsResultsTabControl)
        Me.bsResultFormGroupBox.Controls.Add(Me.bsPanel4)
        Me.bsResultFormGroupBox.Controls.Add(Me.bsSamplesPanel)
        Me.bsResultFormGroupBox.Controls.Add(Me.bsTestPanel)
        Me.bsResultFormGroupBox.Controls.Add(Me.bsSamplesResultsTabControl)
        Me.bsResultFormGroupBox.Controls.Add(Me.bsTestDetailsTabControl)
        Me.bsResultFormGroupBox.Controls.Add(Me.bsResultsFormLabel)
        Me.bsResultFormGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsResultFormGroupBox.Location = New System.Drawing.Point(5, 0)
        Me.bsResultFormGroupBox.Name = "bsResultFormGroupBox"
        Me.bsResultFormGroupBox.Size = New System.Drawing.Size(968, 612)
        Me.bsResultFormGroupBox.TabIndex = 38
        Me.bsResultFormGroupBox.TabStop = False
        '
        'bsTestPanel
        '
        Me.bsTestPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.bsTestPanel.Controls.Add(Me.PrintTestBlankButton)
        Me.bsTestPanel.Controls.Add(Me.PrintTestCtrlButton)
        Me.bsTestPanel.Controls.Add(Me.PrintTestButton)
        Me.bsTestPanel.Location = New System.Drawing.Point(10, 572)
        Me.bsTestPanel.Name = "bsTestPanel"
        Me.bsTestPanel.Size = New System.Drawing.Size(223, 32)
        Me.bsTestPanel.TabIndex = 44
        '
        'PrintTestBlankButton
        '
        Me.PrintTestBlankButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PrintTestBlankButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PrintTestBlankButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PrintTestBlankButton.Location = New System.Drawing.Point(0, 0)
        Me.PrintTestBlankButton.Name = "PrintTestBlankButton"
        Me.PrintTestBlankButton.Size = New System.Drawing.Size(32, 32)
        Me.PrintTestBlankButton.TabIndex = 9
        Me.PrintTestBlankButton.Text = "BT"
        Me.PrintTestBlankButton.UseVisualStyleBackColor = True
        '
        'PrintTestCtrlButton
        '
        Me.PrintTestCtrlButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PrintTestCtrlButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PrintTestCtrlButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PrintTestCtrlButton.Location = New System.Drawing.Point(38, 0)
        Me.PrintTestCtrlButton.Name = "PrintTestCtrlButton"
        Me.PrintTestCtrlButton.Size = New System.Drawing.Size(32, 32)
        Me.PrintTestCtrlButton.TabIndex = 8
        Me.PrintTestCtrlButton.Text = "CT"
        Me.PrintTestCtrlButton.UseVisualStyleBackColor = True
        '
        'PrintTestButton
        '
        Me.PrintTestButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PrintTestButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PrintTestButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PrintTestButton.Location = New System.Drawing.Point(191, 0)
        Me.PrintTestButton.Name = "PrintTestButton"
        Me.PrintTestButton.Size = New System.Drawing.Size(32, 32)
        Me.PrintTestButton.TabIndex = 7
        Me.PrintTestButton.UseVisualStyleBackColor = True
        '
        'bsSamplesResultsTabControl
        '
        Me.bsSamplesResultsTabControl.Controls.Add(Me.bsExperimentalsTabPage)
        Me.bsSamplesResultsTabControl.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSamplesResultsTabControl.Location = New System.Drawing.Point(239, 40)
        Me.bsSamplesResultsTabControl.Name = "bsSamplesResultsTabControl"
        Me.bsSamplesResultsTabControl.SelectedIndex = 0
        Me.bsSamplesResultsTabControl.Size = New System.Drawing.Size(720, 524)
        Me.bsSamplesResultsTabControl.TabIndex = 37
        '
        'bsExperimentalsTabPage
        '
        Me.bsExperimentalsTabPage.BackColor = System.Drawing.Color.Gainsboro
        Me.bsExperimentalsTabPage.Controls.Add(Me.bsExperimentalsDataGridView)
        Me.bsExperimentalsTabPage.ForeColor = System.Drawing.Color.Black
        Me.bsExperimentalsTabPage.Location = New System.Drawing.Point(4, 22)
        Me.bsExperimentalsTabPage.Name = "bsExperimentalsTabPage"
        Me.bsExperimentalsTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.bsExperimentalsTabPage.Size = New System.Drawing.Size(712, 498)
        Me.bsExperimentalsTabPage.TabIndex = 0
        Me.bsExperimentalsTabPage.Text = "Experimentals"
        '
        'bsExperimentalsDataGridView
        '
        Me.bsExperimentalsDataGridView.AllowUserToAddRows = False
        Me.bsExperimentalsDataGridView.AllowUserToDeleteRows = False
        DataGridViewCellStyle86.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle86.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle86.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.bsExperimentalsDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle86
        Me.bsExperimentalsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.bsExperimentalsDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsExperimentalsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsExperimentalsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle87.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle87.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle87.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle87.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle87.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle87.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsExperimentalsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle87
        Me.bsExperimentalsDataGridView.ColumnHeadersHeight = 30
        Me.bsExperimentalsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle88.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle88.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle88.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle88.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle88.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle88.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle88.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsExperimentalsDataGridView.DefaultCellStyle = DataGridViewCellStyle88
        Me.bsExperimentalsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsExperimentalsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsExperimentalsDataGridView.EnterToTab = False
        Me.bsExperimentalsDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsExperimentalsDataGridView.Location = New System.Drawing.Point(3, 3)
        Me.bsExperimentalsDataGridView.Name = "bsExperimentalsDataGridView"
        Me.bsExperimentalsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle89.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle89.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle89.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle89.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle89.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle89.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsExperimentalsDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle89
        Me.bsExperimentalsDataGridView.RowHeadersVisible = False
        Me.bsExperimentalsDataGridView.RowHeadersWidth = 20
        Me.bsExperimentalsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle90.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle90.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle90.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle90.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle90.SelectionForeColor = System.Drawing.Color.Black
        Me.bsExperimentalsDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle90
        Me.bsExperimentalsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsExperimentalsDataGridView.Size = New System.Drawing.Size(706, 492)
        Me.bsExperimentalsDataGridView.TabIndex = 2
        Me.bsExperimentalsDataGridView.TabToEnter = False
        '
        'AlarmsDS1
        '
        Me.AlarmsDS1.DataSetName = "AlarmsDS"
        Me.AlarmsDS1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'IResults
        '
        Me.AcceptButton = Me.ExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.Controls.Add(Me.bsResultFormGroupBox)
        Me.Controls.Add(Me.bsPanel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IResults"
        Me.Text = "Results Screen"
        CType(GridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsPanel4.ResumeLayout(False)
        Me.bsSamplesPanel.ResumeLayout(False)
        Me.bsPanel2.ResumeLayout(False)
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.OrderToExportCheckBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.OrderToPrintCheckBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.STATImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsTestDetailsTabControl.ResumeLayout(False)
        Me.bsSamplesTab.ResumeLayout(False)
        CType(Me.bsSamplesListDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsTestsTabTage.ResumeLayout(False)
        CType(Me.bsTestsListDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsResultsTabControl.ResumeLayout(False)
        Me.bsBlanksTabPage.ResumeLayout(False)
        CType(Me.bsBlanksDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsCalibratorsTabPage.ResumeLayout(False)
        CType(Me.bsCalibratorsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsControlsTabPage.ResumeLayout(False)
        CType(Me.bsControlsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.XtraSamplesTabPage.ResumeLayout(False)
        CType(Me.SamplesXtraGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SamplesXtraGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsResultFormGroupBox.ResumeLayout(False)
        Me.bsTestPanel.ResumeLayout(False)
        Me.bsSamplesResultsTabControl.ResumeLayout(False)
        Me.bsExperimentalsTabPage.ResumeLayout(False)
        CType(Me.bsExperimentalsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AlarmsDS1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsProgTestToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsPanel2 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsPanel4 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents Cycle As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Abs1 As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Abs2 As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Diff As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents OrderToExportCheckBox As DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit
    Friend WithEvents OrderToPrintCheckBox As DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit
    Friend WithEvents STATImage As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
    Friend WithEvents ExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsXlsresults As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ExportButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents OffSystemResultsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents SendManRepButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsResultFormGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsSamplesResultsTabControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents bsExperimentalsTabPage As System.Windows.Forms.TabPage
    Friend WithEvents bsExperimentalsDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsResultsTabControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents bsBlanksTabPage As System.Windows.Forms.TabPage
    Friend WithEvents bsBlanksDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsCalibratorsTabPage As System.Windows.Forms.TabPage
    Friend WithEvents bsCalibratorsDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsControlsTabPage As System.Windows.Forms.TabPage
    Friend WithEvents bsControlsDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsTestDetailsTabControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents bsSamplesTab As System.Windows.Forms.TabPage
    Friend WithEvents bsSamplesListDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsTestsTabTage As System.Windows.Forms.TabPage
    Friend WithEvents bsTestsListDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsResultsFormLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTestPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents PrintTestButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSamplesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents PrintReportButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents SummaryButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents PrintSampleButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents XtraSamplesTabPage As System.Windows.Forms.TabPage
    Friend WithEvents SamplesXtraGrid As DevExpress.XtraGrid.GridControl
    Friend WithEvents SamplesXtraGridView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents ToolTipController1 As DevExpress.Utils.ToolTipController
    Friend WithEvents AlarmsDS1 As Biosystems.Ax00.Types.AlarmsDS
    Friend WithEvents PrintCompactReportButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents PrintTestBlankButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents PrintTestCtrlButton As Biosystems.Ax00.Controls.UserControls.BSButton

End Class
