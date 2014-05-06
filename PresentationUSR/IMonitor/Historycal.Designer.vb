<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Historycal
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
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
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle15 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.MonitorTabs = New DevExpress.XtraTab.XtraTabControl
        Me.MainTab = New DevExpress.XtraTab.XtraTabPage
        Me.AlarmsTab = New DevExpress.XtraTab.XtraTabPage
        Me.splitContainerControl1 = New DevExpress.XtraEditors.SplitContainerControl
        Me.bsAlarmsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsAlarmsDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.BsGroupBox12 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsMaxDateLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsAnalyzerIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsMinDateLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsActiveLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsLabel17 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsMaxDateDateTimePicker = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
        Me.bsMinDateDateTimePicker = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
        Me.bsAnalyzerIDComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsTypeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsActiveComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsWorkSessionIDComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.BsLabel25 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsPanel8 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsClearButton = New DevExpress.XtraEditors.SimpleButton
        Me.StatesTab = New DevExpress.XtraTab.XtraTabPage
        Me.SamplesTab = New DevExpress.XtraTab.XtraTabPage
        Me.ReagentsTab = New DevExpress.XtraTab.XtraTabPage
        Me.ReactionsTab = New DevExpress.XtraTab.XtraTabPage
        Me.ISETab = New DevExpress.XtraTab.XtraTabPage
        Me.bsPanel2 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ExitButton = New DevExpress.XtraEditors.SimpleButton
        CType(Me.MonitorTabs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MonitorTabs.SuspendLayout()
        Me.AlarmsTab.SuspendLayout()
        CType(Me.splitContainerControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainerControl1.SuspendLayout()
        Me.bsAlarmsGroupBox.SuspendLayout()
        CType(Me.bsAlarmsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsGroupBox12.SuspendLayout()
        Me.BsPanel8.SuspendLayout()
        Me.bsPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'MonitorTabs
        '
        Me.MonitorTabs.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
        Me.MonitorTabs.Appearance.Options.UseBackColor = True
        Me.MonitorTabs.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.MonitorTabs.Location = New System.Drawing.Point(3, 1)
        Me.MonitorTabs.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003
        Me.MonitorTabs.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MonitorTabs.LookAndFeel.UseWindowsXPTheme = True
        Me.MonitorTabs.Name = "MonitorTabs"
        Me.MonitorTabs.SelectedTabPage = Me.MainTab
        Me.MonitorTabs.Size = New System.Drawing.Size(973, 610)
        Me.MonitorTabs.TabIndex = 17
        Me.MonitorTabs.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.AlarmsTab, Me.MainTab, Me.StatesTab, Me.SamplesTab, Me.ReagentsTab, Me.ReactionsTab, Me.ISETab})
        '
        'MainTab
        '
        Me.MainTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.MainTab.Appearance.PageClient.Options.UseBackColor = True
        Me.MainTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.MainTab.Name = "MainTab"
        Me.MainTab.Size = New System.Drawing.Size(965, 581)
        Me.MainTab.Text = "Tab2"
        '
        'AlarmsTab
        '
        Me.AlarmsTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.AlarmsTab.Appearance.PageClient.Options.UseBackColor = True
        Me.AlarmsTab.Controls.Add(Me.splitContainerControl1)
        Me.AlarmsTab.Name = "AlarmsTab"
        Me.AlarmsTab.Size = New System.Drawing.Size(965, 581)
        Me.AlarmsTab.Text = "Alarms"
        '
        'splitContainerControl1
        '
        Me.splitContainerControl1.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.splitContainerControl1.Appearance.Options.UseBackColor = True
        Me.splitContainerControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003
        Me.splitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2
        Me.splitContainerControl1.Horizontal = False
        Me.splitContainerControl1.Location = New System.Drawing.Point(2, 2)
        Me.splitContainerControl1.Name = "splitContainerControl1"
        Me.splitContainerControl1.Panel1.Controls.Add(Me.bsAlarmsGroupBox)
        Me.splitContainerControl1.Panel1.Text = "Panel1"
        Me.splitContainerControl1.Panel2.Controls.Add(Me.BsGroupBox12)
        Me.splitContainerControl1.Panel2.Text = "Panel2"
        Me.splitContainerControl1.Size = New System.Drawing.Size(960, 579)
        Me.splitContainerControl1.SplitterPosition = 487
        Me.splitContainerControl1.TabIndex = 1
        Me.splitContainerControl1.Text = "splitContainerControl1"
        '
        'bsAlarmsGroupBox
        '
        Me.bsAlarmsGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.bsAlarmsGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsAlarmsGroupBox.Controls.Add(Me.bsAlarmsDataGridView)
        Me.bsAlarmsGroupBox.Location = New System.Drawing.Point(2, -3)
        Me.bsAlarmsGroupBox.Margin = New System.Windows.Forms.Padding(4, 3, 3, 3)
        Me.bsAlarmsGroupBox.Name = "bsAlarmsGroupBox"
        Me.bsAlarmsGroupBox.Size = New System.Drawing.Size(952, 490)
        Me.bsAlarmsGroupBox.TabIndex = 50
        Me.bsAlarmsGroupBox.TabStop = False
        '
        'bsTitleLabel
        '
        Me.bsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTitleLabel.Location = New System.Drawing.Point(5, 12)
        Me.bsTitleLabel.Name = "bsTitleLabel"
        Me.bsTitleLabel.Size = New System.Drawing.Size(942, 19)
        Me.bsTitleLabel.TabIndex = 13
        Me.bsTitleLabel.Text = "Alarms List"
        Me.bsTitleLabel.Title = True
        '
        'bsAlarmsDataGridView
        '
        Me.bsAlarmsDataGridView.AllowUserToAddRows = False
        Me.bsAlarmsDataGridView.AllowUserToDeleteRows = False
        Me.bsAlarmsDataGridView.AllowUserToResizeColumns = False
        Me.bsAlarmsDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle11.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle11.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle11.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsAlarmsDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle11
        Me.bsAlarmsDataGridView.BackgroundColor = System.Drawing.Color.WhiteSmoke
        Me.bsAlarmsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsAlarmsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle12.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle12.Font = New System.Drawing.Font("Verdana", 10.0!)
        DataGridViewCellStyle12.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsAlarmsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle12
        Me.bsAlarmsDataGridView.ColumnHeadersHeight = 20
        Me.bsAlarmsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle13.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle13.Font = New System.Drawing.Font("Verdana", 10.0!)
        DataGridViewCellStyle13.ForeColor = System.Drawing.Color.FromArgb(CType(CType(32, Byte), Integer), CType(CType(31, Byte), Integer), CType(CType(53, Byte), Integer))
        DataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle13.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsAlarmsDataGridView.DefaultCellStyle = DataGridViewCellStyle13
        Me.bsAlarmsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsAlarmsDataGridView.EnableHeadersVisualStyles = True
        Me.bsAlarmsDataGridView.EnterToTab = False
        Me.bsAlarmsDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsAlarmsDataGridView.Location = New System.Drawing.Point(5, 34)
        Me.bsAlarmsDataGridView.Name = "bsAlarmsDataGridView"
        Me.bsAlarmsDataGridView.ReadOnly = True
        Me.bsAlarmsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle14.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle14.Font = New System.Drawing.Font("Verdana", 10.0!)
        DataGridViewCellStyle14.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle14.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle14.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsAlarmsDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle14
        Me.bsAlarmsDataGridView.RowHeadersWidth = 20
        Me.bsAlarmsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle15.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle15.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle15.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsAlarmsDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle15
        Me.bsAlarmsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsAlarmsDataGridView.Size = New System.Drawing.Size(942, 450)
        Me.bsAlarmsDataGridView.TabIndex = 9
        Me.bsAlarmsDataGridView.TabToEnter = False
        '
        'BsGroupBox12
        '
        Me.BsGroupBox12.BackColor = System.Drawing.Color.Transparent
        Me.BsGroupBox12.Controls.Add(Me.bsMaxDateLabel)
        Me.BsGroupBox12.Controls.Add(Me.bsAnalyzerIDLabel)
        Me.BsGroupBox12.Controls.Add(Me.bsMinDateLabel)
        Me.BsGroupBox12.Controls.Add(Me.bsTypeLabel)
        Me.BsGroupBox12.Controls.Add(Me.bsActiveLabel)
        Me.BsGroupBox12.Controls.Add(Me.BsLabel17)
        Me.BsGroupBox12.Controls.Add(Me.bsMaxDateDateTimePicker)
        Me.BsGroupBox12.Controls.Add(Me.bsMinDateDateTimePicker)
        Me.BsGroupBox12.Controls.Add(Me.bsAnalyzerIDComboBox)
        Me.BsGroupBox12.Controls.Add(Me.bsTypeComboBox)
        Me.BsGroupBox12.Controls.Add(Me.bsActiveComboBox)
        Me.BsGroupBox12.Controls.Add(Me.bsWorkSessionIDComboBox)
        Me.BsGroupBox12.Controls.Add(Me.BsLabel25)
        Me.BsGroupBox12.Controls.Add(Me.BsPanel8)
        Me.BsGroupBox12.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsGroupBox12.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox12.Location = New System.Drawing.Point(2, -6)
        Me.BsGroupBox12.Name = "BsGroupBox12"
        Me.BsGroupBox12.Size = New System.Drawing.Size(952, 85)
        Me.BsGroupBox12.TabIndex = 49
        Me.BsGroupBox12.TabStop = False
        '
        'bsMaxDateLabel
        '
        Me.bsMaxDateLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsMaxDateLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsMaxDateLabel.ForeColor = System.Drawing.Color.Black
        Me.bsMaxDateLabel.Location = New System.Drawing.Point(208, 39)
        Me.bsMaxDateLabel.Name = "bsMaxDateLabel"
        Me.bsMaxDateLabel.Size = New System.Drawing.Size(110, 13)
        Me.bsMaxDateLabel.TabIndex = 76
        Me.bsMaxDateLabel.Text = "Max. Date:"
        Me.bsMaxDateLabel.Title = False
        '
        'bsAnalyzerIDLabel
        '
        Me.bsAnalyzerIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsAnalyzerIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsAnalyzerIDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsAnalyzerIDLabel.Location = New System.Drawing.Point(399, 39)
        Me.bsAnalyzerIDLabel.Name = "bsAnalyzerIDLabel"
        Me.bsAnalyzerIDLabel.Size = New System.Drawing.Size(130, 13)
        Me.bsAnalyzerIDLabel.TabIndex = 71
        Me.bsAnalyzerIDLabel.Text = "AnalyzerID:"
        Me.bsAnalyzerIDLabel.Title = False
        '
        'bsMinDateLabel
        '
        Me.bsMinDateLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsMinDateLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsMinDateLabel.ForeColor = System.Drawing.Color.Black
        Me.bsMinDateLabel.Location = New System.Drawing.Point(92, 39)
        Me.bsMinDateLabel.Name = "bsMinDateLabel"
        Me.bsMinDateLabel.Size = New System.Drawing.Size(110, 13)
        Me.bsMinDateLabel.TabIndex = 75
        Me.bsMinDateLabel.Text = "Min. Date:"
        Me.bsMinDateLabel.Title = False
        '
        'bsTypeLabel
        '
        Me.bsTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTypeLabel.Location = New System.Drawing.Point(5, 39)
        Me.bsTypeLabel.Name = "bsTypeLabel"
        Me.bsTypeLabel.Size = New System.Drawing.Size(81, 13)
        Me.bsTypeLabel.TabIndex = 74
        Me.bsTypeLabel.Text = "Type:"
        Me.bsTypeLabel.Title = False
        '
        'bsActiveLabel
        '
        Me.bsActiveLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsActiveLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsActiveLabel.ForeColor = System.Drawing.Color.Black
        Me.bsActiveLabel.Location = New System.Drawing.Point(324, 39)
        Me.bsActiveLabel.Name = "bsActiveLabel"
        Me.bsActiveLabel.Size = New System.Drawing.Size(66, 13)
        Me.bsActiveLabel.TabIndex = 73
        Me.bsActiveLabel.Text = "Active:"
        Me.bsActiveLabel.Title = False
        '
        'BsLabel17
        '
        Me.BsLabel17.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel17.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel17.ForeColor = System.Drawing.Color.Black
        Me.BsLabel17.Location = New System.Drawing.Point(547, 39)
        Me.BsLabel17.Name = "BsLabel17"
        Me.BsLabel17.Size = New System.Drawing.Size(125, 13)
        Me.BsLabel17.TabIndex = 72
        Me.BsLabel17.Text = "WorkSessionID:"
        Me.BsLabel17.Title = False
        '
        'bsMaxDateDateTimePicker
        '
        Me.bsMaxDateDateTimePicker.CalendarFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsMaxDateDateTimePicker.CalendarForeColor = System.Drawing.Color.Black
        Me.bsMaxDateDateTimePicker.CalendarMonthBackground = System.Drawing.Color.White
        Me.bsMaxDateDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.bsMaxDateDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.bsMaxDateDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.bsMaxDateDateTimePicker.Checked = False
        Me.bsMaxDateDateTimePicker.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsMaxDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.bsMaxDateDateTimePicker.Location = New System.Drawing.Point(211, 55)
        Me.bsMaxDateDateTimePicker.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsMaxDateDateTimePicker.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsMaxDateDateTimePicker.Name = "bsMaxDateDateTimePicker"
        Me.bsMaxDateDateTimePicker.ShowCheckBox = True
        Me.bsMaxDateDateTimePicker.Size = New System.Drawing.Size(110, 21)
        Me.bsMaxDateDateTimePicker.TabIndex = 69
        '
        'bsMinDateDateTimePicker
        '
        Me.bsMinDateDateTimePicker.CalendarFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsMinDateDateTimePicker.CalendarForeColor = System.Drawing.Color.Black
        Me.bsMinDateDateTimePicker.CalendarMonthBackground = System.Drawing.Color.White
        Me.bsMinDateDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.bsMinDateDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.bsMinDateDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.bsMinDateDateTimePicker.Checked = False
        Me.bsMinDateDateTimePicker.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsMinDateDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.bsMinDateDateTimePicker.Location = New System.Drawing.Point(95, 55)
        Me.bsMinDateDateTimePicker.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsMinDateDateTimePicker.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsMinDateDateTimePicker.Name = "bsMinDateDateTimePicker"
        Me.bsMinDateDateTimePicker.ShowCheckBox = True
        Me.bsMinDateDateTimePicker.Size = New System.Drawing.Size(110, 21)
        Me.bsMinDateDateTimePicker.TabIndex = 67
        '
        'bsAnalyzerIDComboBox
        '
        Me.bsAnalyzerIDComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsAnalyzerIDComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAnalyzerIDComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsAnalyzerIDComboBox.FormattingEnabled = True
        Me.bsAnalyzerIDComboBox.Location = New System.Drawing.Point(399, 55)
        Me.bsAnalyzerIDComboBox.Name = "bsAnalyzerIDComboBox"
        Me.bsAnalyzerIDComboBox.Size = New System.Drawing.Size(142, 21)
        Me.bsAnalyzerIDComboBox.TabIndex = 59
        '
        'bsTypeComboBox
        '
        Me.bsTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsTypeComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsTypeComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsTypeComboBox.FormattingEnabled = True
        Me.bsTypeComboBox.Location = New System.Drawing.Point(8, 55)
        Me.bsTypeComboBox.Name = "bsTypeComboBox"
        Me.bsTypeComboBox.Size = New System.Drawing.Size(81, 21)
        Me.bsTypeComboBox.TabIndex = 65
        '
        'bsActiveComboBox
        '
        Me.bsActiveComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsActiveComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsActiveComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsActiveComboBox.FormattingEnabled = True
        Me.bsActiveComboBox.Location = New System.Drawing.Point(327, 55)
        Me.bsActiveComboBox.Name = "bsActiveComboBox"
        Me.bsActiveComboBox.Size = New System.Drawing.Size(66, 21)
        Me.bsActiveComboBox.TabIndex = 63
        '
        'bsWorkSessionIDComboBox
        '
        Me.bsWorkSessionIDComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsWorkSessionIDComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsWorkSessionIDComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsWorkSessionIDComboBox.FormattingEnabled = True
        Me.bsWorkSessionIDComboBox.Location = New System.Drawing.Point(547, 55)
        Me.bsWorkSessionIDComboBox.Name = "bsWorkSessionIDComboBox"
        Me.bsWorkSessionIDComboBox.Size = New System.Drawing.Size(125, 21)
        Me.bsWorkSessionIDComboBox.TabIndex = 61
        '
        'BsLabel25
        '
        Me.BsLabel25.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsLabel25.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsLabel25.ForeColor = System.Drawing.Color.Black
        Me.BsLabel25.Location = New System.Drawing.Point(5, 12)
        Me.BsLabel25.Name = "BsLabel25"
        Me.BsLabel25.Size = New System.Drawing.Size(942, 19)
        Me.BsLabel25.TabIndex = 25
        Me.BsLabel25.Text = "Filters"
        Me.BsLabel25.Title = True
        '
        'BsPanel8
        '
        Me.BsPanel8.Controls.Add(Me.bsClearButton)
        Me.BsPanel8.Location = New System.Drawing.Point(903, 44)
        Me.BsPanel8.Name = "BsPanel8"
        Me.BsPanel8.Size = New System.Drawing.Size(44, 32)
        Me.BsPanel8.TabIndex = 42
        '
        'bsClearButton
        '
        Me.bsClearButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.bsClearButton.Location = New System.Drawing.Point(11, 0)
        Me.bsClearButton.Name = "bsClearButton"
        Me.bsClearButton.Size = New System.Drawing.Size(32, 32)
        Me.bsClearButton.TabIndex = 71
        '
        'StatesTab
        '
        Me.StatesTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.StatesTab.Appearance.PageClient.Options.UseBackColor = True
        Me.StatesTab.Name = "StatesTab"
        Me.StatesTab.Size = New System.Drawing.Size(965, 623)
        Me.StatesTab.Text = "Tab3"
        '
        'SamplesTab
        '
        Me.SamplesTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.SamplesTab.Appearance.PageClient.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.SamplesTab.Appearance.PageClient.Options.UseBackColor = True
        Me.SamplesTab.Name = "SamplesTab"
        Me.SamplesTab.Size = New System.Drawing.Size(965, 623)
        Me.SamplesTab.Text = "Tab4"
        '
        'ReagentsTab
        '
        Me.ReagentsTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ReagentsTab.Appearance.PageClient.Options.UseBackColor = True
        Me.ReagentsTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ReagentsTab.Name = "ReagentsTab"
        Me.ReagentsTab.Size = New System.Drawing.Size(965, 623)
        Me.ReagentsTab.Text = "Tab5"
        '
        'ReactionsTab
        '
        Me.ReactionsTab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ReactionsTab.Appearance.PageClient.Options.UseBackColor = True
        Me.ReactionsTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.ReactionsTab.Name = "ReactionsTab"
        Me.ReactionsTab.Size = New System.Drawing.Size(965, 623)
        Me.ReactionsTab.Text = "Tab6"
        '
        'ISETab
        '
        Me.ISETab.Appearance.PageClient.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ISETab.Appearance.PageClient.Options.UseBackColor = True
        Me.ISETab.Name = "ISETab"
        Me.ISETab.Size = New System.Drawing.Size(965, 623)
        Me.ISETab.Text = "Tab7"
        '
        'bsPanel2
        '
        Me.bsPanel2.Controls.Add(Me.ExitButton)
        Me.bsPanel2.Location = New System.Drawing.Point(899, 617)
        Me.bsPanel2.Name = "bsPanel2"
        Me.bsPanel2.Size = New System.Drawing.Size(69, 32)
        Me.bsPanel2.TabIndex = 35
        '
        'ExitButton
        '
        Me.ExitButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter
        Me.ExitButton.Location = New System.Drawing.Point(37, 0)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(32, 32)
        Me.ExitButton.TabIndex = 16
        '
        'Historycal
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.Controls.Add(Me.bsPanel2)
        Me.Controls.Add(Me.MonitorTabs)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Historycal"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Monitor"
        CType(Me.MonitorTabs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MonitorTabs.ResumeLayout(False)
        Me.AlarmsTab.ResumeLayout(False)
        CType(Me.splitContainerControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainerControl1.ResumeLayout(False)
        Me.bsAlarmsGroupBox.ResumeLayout(False)
        CType(Me.bsAlarmsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsGroupBox12.ResumeLayout(False)
        Me.BsPanel8.ResumeLayout(False)
        Me.bsPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents MonitorTabs As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents MainTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents StatesTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents SamplesTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents ReagentsTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents ReactionsTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents ISETab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents AlarmsTab As DevExpress.XtraTab.XtraTabPage
    Private WithEvents splitContainerControl1 As DevExpress.XtraEditors.SplitContainerControl
    Friend WithEvents bsAlarmsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAlarmsDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents BsGroupBox12 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsMaxDateDateTimePicker As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents bsMinDateDateTimePicker As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents bsAnalyzerIDComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsTypeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsActiveComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsWorkSessionIDComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsLabel25 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsPanel8 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsClearButton As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents bsMaxDateLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAnalyzerIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsMinDateLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsActiveLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel17 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsPanel2 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents ExitButton As DevExpress.XtraEditors.SimpleButton
End Class
