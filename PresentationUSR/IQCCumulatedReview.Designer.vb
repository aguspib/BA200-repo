<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiQCCumulatedReview
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                ReleaseElements()
                components.Dispose()
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim SwiftPlotSeriesView1 As DevExpress.XtraCharts.SwiftPlotSeriesView = New DevExpress.XtraCharts.SwiftPlotSeriesView()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiQCCumulatedReview))
        Me.bsTestSampleListView = New Biosystems.Ax00.Controls.UserControls.BSListView()
        Me.bsTestSampleGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsTestSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsCalculationCriteriaGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsCumulatedXtraTab = New DevExpress.XtraTab.XtraTabControl()
        Me.ValuesXtraTab = New DevExpress.XtraTab.XtraTabPage()
        Me.bsDeleteCumulateSeries = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsResultsDetailsGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.GraphXtraTab = New DevExpress.XtraTab.XtraTabPage()
        Me.bsLegendGB = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsThirdCtrlLotLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsThirdCtrlLotPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsSecondCtrlLotLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSecondCtrlLotPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsFirstCtrlLotPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsFirstCtrlLotLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsMeanChartControl = New DevExpress.XtraCharts.ChartControl()
        Me.bsResultControlLotGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsSearchButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsDateToDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.bsDateToLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsCumulatedSeriesLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDateFromDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.bsDateFromLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsCumulatedResultsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsScreenToolTips = New DevExpress.Utils.ToolTipController(Me.components)
        Me.bsTestSampleGroupBox.SuspendLayout()
        Me.bsCalculationCriteriaGroupBox.SuspendLayout()
        CType(Me.bsCumulatedXtraTab, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsCumulatedXtraTab.SuspendLayout()
        Me.ValuesXtraTab.SuspendLayout()
        CType(Me.bsResultsDetailsGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GraphXtraTab.SuspendLayout()
        Me.bsLegendGB.SuspendLayout()
        CType(Me.bsThirdCtrlLotPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsSecondCtrlLotPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsFirstCtrlLotPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsMeanChartControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(SwiftPlotSeriesView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsResultControlLotGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsTestSampleListView
        '
        Me.bsTestSampleListView.AllowColumnReorder = True
        Me.bsTestSampleListView.AutoArrange = False
        Me.bsTestSampleListView.FullRowSelect = True
        Me.bsTestSampleListView.HideSelection = False
        Me.bsTestSampleListView.Location = New System.Drawing.Point(5, 40)
        Me.bsTestSampleListView.MultiSelect = False
        Me.bsTestSampleListView.Name = "bsTestSampleListView"
        Me.bsTestSampleListView.Size = New System.Drawing.Size(224, 550)
        Me.bsTestSampleListView.TabIndex = 0
        Me.bsTestSampleListView.UseCompatibleStateImageBehavior = False
        Me.bsTestSampleListView.View = System.Windows.Forms.View.Details
        '
        'bsTestSampleGroupBox
        '
        Me.bsTestSampleGroupBox.Controls.Add(Me.bsTestSampleTypeLabel)
        Me.bsTestSampleGroupBox.Controls.Add(Me.bsTestSampleListView)
        Me.bsTestSampleGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestSampleGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsTestSampleGroupBox.Name = "bsTestSampleGroupBox"
        Me.bsTestSampleGroupBox.Size = New System.Drawing.Size(234, 598)
        Me.bsTestSampleGroupBox.TabIndex = 1
        Me.bsTestSampleGroupBox.TabStop = False
        '
        'bsTestSampleTypeLabel
        '
        Me.bsTestSampleTypeLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTestSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTestSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestSampleTypeLabel.Location = New System.Drawing.Point(5, 15)
        Me.bsTestSampleTypeLabel.Name = "bsTestSampleTypeLabel"
        Me.bsTestSampleTypeLabel.Size = New System.Drawing.Size(224, 20)
        Me.bsTestSampleTypeLabel.TabIndex = 1
        Me.bsTestSampleTypeLabel.Text = "*Tests/Sample Type"
        Me.bsTestSampleTypeLabel.Title = True
        '
        'bsCalculationCriteriaGroupBox
        '
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsCumulatedXtraTab)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsResultControlLotGridView)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsSearchButton)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsDateToDateTimePick)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsDateToLabel)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsCumulatedSeriesLabel)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsDateFromDateTimePick)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsDateFromLabel)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsCumulatedResultsLabel)
        Me.bsCalculationCriteriaGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsCalculationCriteriaGroupBox.Location = New System.Drawing.Point(249, 10)
        Me.bsCalculationCriteriaGroupBox.Name = "bsCalculationCriteriaGroupBox"
        Me.bsCalculationCriteriaGroupBox.Size = New System.Drawing.Size(719, 598)
        Me.bsCalculationCriteriaGroupBox.TabIndex = 2
        Me.bsCalculationCriteriaGroupBox.TabStop = False
        '
        'bsCumulatedXtraTab
        '
        Me.bsCumulatedXtraTab.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.bsCumulatedXtraTab.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCumulatedXtraTab.Appearance.Options.UseBackColor = True
        Me.bsCumulatedXtraTab.Appearance.Options.UseFont = True
        Me.bsCumulatedXtraTab.AppearancePage.Header.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCumulatedXtraTab.AppearancePage.Header.Options.UseFont = True
        Me.bsCumulatedXtraTab.Location = New System.Drawing.Point(10, 265)
        Me.bsCumulatedXtraTab.LookAndFeel.UseDefaultLookAndFeel = False
        Me.bsCumulatedXtraTab.LookAndFeel.UseWindowsXPTheme = True
        Me.bsCumulatedXtraTab.Name = "bsCumulatedXtraTab"
        Me.bsCumulatedXtraTab.SelectedTabPage = Me.ValuesXtraTab
        Me.bsCumulatedXtraTab.Size = New System.Drawing.Size(703, 325)
        Me.bsCumulatedXtraTab.TabIndex = 12
        Me.bsCumulatedXtraTab.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.ValuesXtraTab, Me.GraphXtraTab})
        '
        'ValuesXtraTab
        '
        Me.ValuesXtraTab.Appearance.PageClient.BackColor = System.Drawing.Color.Gainsboro
        Me.ValuesXtraTab.Appearance.PageClient.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.ValuesXtraTab.Appearance.PageClient.Options.UseBackColor = True
        Me.ValuesXtraTab.Appearance.PageClient.Options.UseFont = True
        Me.ValuesXtraTab.Controls.Add(Me.bsDeleteCumulateSeries)
        Me.ValuesXtraTab.Controls.Add(Me.bsResultsDetailsGridView)
        Me.ValuesXtraTab.Name = "ValuesXtraTab"
        Me.ValuesXtraTab.Size = New System.Drawing.Size(695, 296)
        Me.ValuesXtraTab.Text = "*Values"
        '
        'bsDeleteCumulateSeries
        '
        Me.bsDeleteCumulateSeries.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsDeleteCumulateSeries.Location = New System.Drawing.Point(662, 263)
        Me.bsDeleteCumulateSeries.Name = "bsDeleteCumulateSeries"
        Me.bsDeleteCumulateSeries.Size = New System.Drawing.Size(32, 32)
        Me.bsDeleteCumulateSeries.TabIndex = 11
        Me.bsDeleteCumulateSeries.UseVisualStyleBackColor = True
        '
        'bsResultsDetailsGridView
        '
        Me.bsResultsDetailsGridView.AllowUserToAddRows = False
        Me.bsResultsDetailsGridView.AllowUserToDeleteRows = False
        Me.bsResultsDetailsGridView.AllowUserToResizeColumns = False
        Me.bsResultsDetailsGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.bsResultsDetailsGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsResultsDetailsGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsResultsDetailsGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsResultsDetailsGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultsDetailsGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsResultsDetailsGridView.ColumnHeadersHeight = 20
        Me.bsResultsDetailsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultsDetailsGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsResultsDetailsGridView.EnterToTab = False
        Me.bsResultsDetailsGridView.GridColor = System.Drawing.Color.Silver
        Me.bsResultsDetailsGridView.Location = New System.Drawing.Point(1, 6)
        Me.bsResultsDetailsGridView.Name = "bsResultsDetailsGridView"
        Me.bsResultsDetailsGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultsDetailsGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsResultsDetailsGridView.RowHeadersVisible = False
        Me.bsResultsDetailsGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        Me.bsResultsDetailsGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsResultsDetailsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsResultsDetailsGridView.ShowCellErrors = False
        Me.bsResultsDetailsGridView.Size = New System.Drawing.Size(693, 253)
        Me.bsResultsDetailsGridView.TabIndex = 3
        Me.bsResultsDetailsGridView.TabToEnter = False
        '
        'GraphXtraTab
        '
        Me.GraphXtraTab.Appearance.PageClient.BackColor = System.Drawing.Color.Gainsboro
        Me.GraphXtraTab.Appearance.PageClient.Options.UseBackColor = True
        Me.GraphXtraTab.Controls.Add(Me.bsLegendGB)
        Me.GraphXtraTab.Controls.Add(Me.bsMeanChartControl)
        Me.GraphXtraTab.Name = "GraphXtraTab"
        Me.GraphXtraTab.Size = New System.Drawing.Size(695, 296)
        Me.GraphXtraTab.Text = "*Graph"
        '
        'bsLegendGB
        '
        Me.bsLegendGB.Controls.Add(Me.bsThirdCtrlLotLabel)
        Me.bsLegendGB.Controls.Add(Me.bsThirdCtrlLotPictureBox)
        Me.bsLegendGB.Controls.Add(Me.bsSecondCtrlLotLabel)
        Me.bsLegendGB.Controls.Add(Me.bsSecondCtrlLotPictureBox)
        Me.bsLegendGB.Controls.Add(Me.bsFirstCtrlLotPictureBox)
        Me.bsLegendGB.Controls.Add(Me.bsFirstCtrlLotLabel)
        Me.bsLegendGB.ForeColor = System.Drawing.Color.Black
        Me.bsLegendGB.Location = New System.Drawing.Point(16, 240)
        Me.bsLegendGB.Name = "bsLegendGB"
        Me.bsLegendGB.Size = New System.Drawing.Size(666, 55)
        Me.bsLegendGB.TabIndex = 4
        Me.bsLegendGB.TabStop = False
        Me.bsLegendGB.Text = "*Legend"
        '
        'bsThirdCtrlLotLabel
        '
        Me.bsThirdCtrlLotLabel.AutoSize = True
        Me.bsThirdCtrlLotLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsThirdCtrlLotLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsThirdCtrlLotLabel.ForeColor = System.Drawing.Color.Black
        Me.bsThirdCtrlLotLabel.Location = New System.Drawing.Point(508, 21)
        Me.bsThirdCtrlLotLabel.Name = "bsThirdCtrlLotLabel"
        Me.bsThirdCtrlLotLabel.Size = New System.Drawing.Size(104, 13)
        Me.bsThirdCtrlLotLabel.TabIndex = 10
        Me.bsThirdCtrlLotLabel.Text = "Third Control/Lot"
        Me.bsThirdCtrlLotLabel.Title = False
        '
        'bsThirdCtrlLotPictureBox
        '
        Me.bsThirdCtrlLotPictureBox.Location = New System.Drawing.Point(486, 21)
        Me.bsThirdCtrlLotPictureBox.Name = "bsThirdCtrlLotPictureBox"
        Me.bsThirdCtrlLotPictureBox.PositionNumber = 0
        Me.bsThirdCtrlLotPictureBox.Size = New System.Drawing.Size(16, 16)
        Me.bsThirdCtrlLotPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsThirdCtrlLotPictureBox.TabIndex = 9
        Me.bsThirdCtrlLotPictureBox.TabStop = False
        '
        'bsSecondCtrlLotLabel
        '
        Me.bsSecondCtrlLotLabel.AutoSize = True
        Me.bsSecondCtrlLotLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSecondCtrlLotLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSecondCtrlLotLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSecondCtrlLotLabel.Location = New System.Drawing.Point(277, 21)
        Me.bsSecondCtrlLotLabel.Name = "bsSecondCtrlLotLabel"
        Me.bsSecondCtrlLotLabel.Size = New System.Drawing.Size(117, 13)
        Me.bsSecondCtrlLotLabel.TabIndex = 8
        Me.bsSecondCtrlLotLabel.Text = "Second Control/Lot"
        Me.bsSecondCtrlLotLabel.Title = False
        '
        'bsSecondCtrlLotPictureBox
        '
        Me.bsSecondCtrlLotPictureBox.ErrorImage = Nothing
        Me.bsSecondCtrlLotPictureBox.Location = New System.Drawing.Point(255, 21)
        Me.bsSecondCtrlLotPictureBox.Name = "bsSecondCtrlLotPictureBox"
        Me.bsSecondCtrlLotPictureBox.PositionNumber = 0
        Me.bsSecondCtrlLotPictureBox.Size = New System.Drawing.Size(16, 16)
        Me.bsSecondCtrlLotPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsSecondCtrlLotPictureBox.TabIndex = 7
        Me.bsSecondCtrlLotPictureBox.TabStop = False
        '
        'bsFirstCtrlLotPictureBox
        '
        Me.bsFirstCtrlLotPictureBox.Location = New System.Drawing.Point(25, 21)
        Me.bsFirstCtrlLotPictureBox.Name = "bsFirstCtrlLotPictureBox"
        Me.bsFirstCtrlLotPictureBox.PositionNumber = 0
        Me.bsFirstCtrlLotPictureBox.Size = New System.Drawing.Size(16, 16)
        Me.bsFirstCtrlLotPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.bsFirstCtrlLotPictureBox.TabIndex = 6
        Me.bsFirstCtrlLotPictureBox.TabStop = False
        '
        'bsFirstCtrlLotLabel
        '
        Me.bsFirstCtrlLotLabel.AutoSize = True
        Me.bsFirstCtrlLotLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsFirstCtrlLotLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFirstCtrlLotLabel.ForeColor = System.Drawing.Color.Black
        Me.bsFirstCtrlLotLabel.Location = New System.Drawing.Point(47, 21)
        Me.bsFirstCtrlLotLabel.Name = "bsFirstCtrlLotLabel"
        Me.bsFirstCtrlLotLabel.Size = New System.Drawing.Size(99, 13)
        Me.bsFirstCtrlLotLabel.TabIndex = 5
        Me.bsFirstCtrlLotLabel.Text = "First Control/Lot"
        Me.bsFirstCtrlLotLabel.Title = False
        '
        'bsMeanChartControl
        '
        Me.bsMeanChartControl.Location = New System.Drawing.Point(16, 11)
        Me.bsMeanChartControl.Name = "bsMeanChartControl"
        Me.bsMeanChartControl.SeriesSerializable = New DevExpress.XtraCharts.Series(-1) {}
        Me.bsMeanChartControl.SeriesTemplate.View = SwiftPlotSeriesView1
        Me.bsMeanChartControl.Size = New System.Drawing.Size(666, 223)
        Me.bsMeanChartControl.TabIndex = 0
        '
        'bsResultControlLotGridView
        '
        Me.bsResultControlLotGridView.AllowUserToAddRows = False
        Me.bsResultControlLotGridView.AllowUserToDeleteRows = False
        Me.bsResultControlLotGridView.AllowUserToResizeColumns = False
        Me.bsResultControlLotGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.bsResultControlLotGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle6
        Me.bsResultControlLotGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsResultControlLotGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsResultControlLotGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultControlLotGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.bsResultControlLotGridView.ColumnHeadersHeight = 20
        Me.bsResultControlLotGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultControlLotGridView.DefaultCellStyle = DataGridViewCellStyle8
        Me.bsResultControlLotGridView.EnterToTab = False
        Me.bsResultControlLotGridView.GridColor = System.Drawing.Color.Silver
        Me.bsResultControlLotGridView.Location = New System.Drawing.Point(10, 84)
        Me.bsResultControlLotGridView.Name = "bsResultControlLotGridView"
        Me.bsResultControlLotGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle9.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultControlLotGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.bsResultControlLotGridView.RowHeadersVisible = False
        Me.bsResultControlLotGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle10.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White
        Me.bsResultControlLotGridView.RowsDefaultCellStyle = DataGridViewCellStyle10
        Me.bsResultControlLotGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsResultControlLotGridView.ShowCellErrors = False
        Me.bsResultControlLotGridView.ShowEditingIcon = False
        Me.bsResultControlLotGridView.ShowRowErrors = False
        Me.bsResultControlLotGridView.Size = New System.Drawing.Size(703, 143)
        Me.bsResultControlLotGridView.TabIndex = 11
        Me.bsResultControlLotGridView.TabToEnter = False
        '
        'bsSearchButton
        '
        Me.bsSearchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSearchButton.Location = New System.Drawing.Point(677, 44)
        Me.bsSearchButton.Name = "bsSearchButton"
        Me.bsSearchButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSearchButton.TabIndex = 10
        Me.bsSearchButton.UseVisualStyleBackColor = True
        '
        'bsDateToDateTimePick
        '
        Me.bsDateToDateTimePick.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateToDateTimePick.CalendarForeColor = System.Drawing.Color.Black
        Me.bsDateToDateTimePick.CalendarMonthBackground = System.Drawing.Color.White
        Me.bsDateToDateTimePick.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.bsDateToDateTimePick.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.bsDateToDateTimePick.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.bsDateToDateTimePick.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateToDateTimePick.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.bsDateToDateTimePick.Location = New System.Drawing.Point(184, 55)
        Me.bsDateToDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsDateToDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsDateToDateTimePick.Name = "bsDateToDateTimePick"
        Me.bsDateToDateTimePick.Size = New System.Drawing.Size(103, 21)
        Me.bsDateToDateTimePick.TabIndex = 4
        '
        'bsDateToLabel
        '
        Me.bsDateToLabel.AutoSize = True
        Me.bsDateToLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDateToLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateToLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDateToLabel.Location = New System.Drawing.Point(181, 39)
        Me.bsDateToLabel.Name = "bsDateToLabel"
        Me.bsDateToLabel.Size = New System.Drawing.Size(33, 13)
        Me.bsDateToLabel.TabIndex = 3
        Me.bsDateToLabel.Text = "*To:"
        Me.bsDateToLabel.Title = False
        '
        'bsCumulatedSeriesLabel
        '
        Me.bsCumulatedSeriesLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsCumulatedSeriesLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsCumulatedSeriesLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCumulatedSeriesLabel.Location = New System.Drawing.Point(9, 237)
        Me.bsCumulatedSeriesLabel.Name = "bsCumulatedSeriesLabel"
        Me.bsCumulatedSeriesLabel.Size = New System.Drawing.Size(703, 20)
        Me.bsCumulatedSeriesLabel.TabIndex = 3
        Me.bsCumulatedSeriesLabel.Text = "*Accumulated Series Details"
        Me.bsCumulatedSeriesLabel.Title = True
        '
        'bsDateFromDateTimePick
        '
        Me.bsDateFromDateTimePick.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateFromDateTimePick.CalendarForeColor = System.Drawing.Color.Black
        Me.bsDateFromDateTimePick.CalendarMonthBackground = System.Drawing.Color.White
        Me.bsDateFromDateTimePick.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.bsDateFromDateTimePick.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.bsDateFromDateTimePick.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.bsDateFromDateTimePick.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateFromDateTimePick.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.bsDateFromDateTimePick.Location = New System.Drawing.Point(13, 55)
        Me.bsDateFromDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsDateFromDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsDateFromDateTimePick.Name = "bsDateFromDateTimePick"
        Me.bsDateFromDateTimePick.Size = New System.Drawing.Size(103, 21)
        Me.bsDateFromDateTimePick.TabIndex = 2
        '
        'bsDateFromLabel
        '
        Me.bsDateFromLabel.AutoSize = True
        Me.bsDateFromLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDateFromLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateFromLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDateFromLabel.Location = New System.Drawing.Point(10, 39)
        Me.bsDateFromLabel.Name = "bsDateFromLabel"
        Me.bsDateFromLabel.Size = New System.Drawing.Size(48, 13)
        Me.bsDateFromLabel.TabIndex = 1
        Me.bsDateFromLabel.Text = "*From:"
        Me.bsDateFromLabel.Title = False
        '
        'bsCumulatedResultsLabel
        '
        Me.bsCumulatedResultsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsCumulatedResultsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsCumulatedResultsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCumulatedResultsLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsCumulatedResultsLabel.Name = "bsCumulatedResultsLabel"
        Me.bsCumulatedResultsLabel.Size = New System.Drawing.Size(703, 20)
        Me.bsCumulatedResultsLabel.TabIndex = 0
        Me.bsCumulatedResultsLabel.Text = "*Accumulated Results by Control/Lot"
        Me.bsCumulatedResultsLabel.Title = True
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Location = New System.Drawing.Point(926, 613)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 3
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Location = New System.Drawing.Point(212, 613)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 5
        Me.bsPrintButton.UseVisualStyleBackColor = True
        '
        'bsScreenToolTips
        '
        Me.bsScreenToolTips.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip
        '
        'IQCCumulatedReview
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsPrintButton)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsCalculationCriteriaGroupBox)
        Me.Controls.Add(Me.bsTestSampleGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IQCCumulatedReview"
        Me.ShowInTaskbar = False
        Me.Text = ""
        Me.bsTestSampleGroupBox.ResumeLayout(False)
        Me.bsCalculationCriteriaGroupBox.ResumeLayout(False)
        Me.bsCalculationCriteriaGroupBox.PerformLayout()
        CType(Me.bsCumulatedXtraTab, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsCumulatedXtraTab.ResumeLayout(False)
        Me.ValuesXtraTab.ResumeLayout(False)
        CType(Me.bsResultsDetailsGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GraphXtraTab.ResumeLayout(False)
        Me.bsLegendGB.ResumeLayout(False)
        Me.bsLegendGB.PerformLayout()
        CType(Me.bsThirdCtrlLotPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsSecondCtrlLotPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsFirstCtrlLotPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(SwiftPlotSeriesView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsMeanChartControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsResultControlLotGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsTestSampleListView As Biosystems.Ax00.Controls.UserControls.BSListView
    Friend WithEvents bsTestSampleGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTestSampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsCalculationCriteriaGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsDateFromDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents bsDateFromLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsCumulatedResultsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDateToDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents bsDateToLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSearchButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCumulatedSeriesLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsResultControlLotGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsCumulatedXtraTab As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents ValuesXtraTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents bsResultsDetailsGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents GraphXtraTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents bsDeleteCumulateSeries As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsMeanChartControl As DevExpress.XtraCharts.ChartControl
    Friend WithEvents bsLegendGB As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsFirstCtrlLotPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsFirstCtrlLotLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSecondCtrlLotLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSecondCtrlLotPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsThirdCtrlLotLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsThirdCtrlLotPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsScreenToolTips As DevExpress.Utils.ToolTipController

End Class
