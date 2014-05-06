<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IHisResults
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
        Me.exitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.searchGroup = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.subtitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.testTypeChkComboBox = New DevExpress.XtraEditors.CheckedComboBoxEdit()
        Me.sampleTypeChkComboBox = New DevExpress.XtraEditors.CheckedComboBoxEdit()
        Me.sampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.testNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.sampleIdTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.analyzerIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.testTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.statFlagLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.analyzerIDComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.statFlagComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.testNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.dateToDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.sampleIdLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.dateToLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.dateFromDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.dateFromLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.searchButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.historyGroup = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.CompactPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.PrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.historyGrid = New DevExpress.XtraGrid.GridControl()
        Me.historyGridView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.exportButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.historyDeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.searchGroup.SuspendLayout()
        CType(Me.testTypeChkComboBox.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sampleTypeChkComboBox.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.historyGroup.SuspendLayout()
        CType(Me.historyGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.historyGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'exitButton
        '
        Me.exitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.exitButton.Location = New System.Drawing.Point(923, 614)
        Me.exitButton.Name = "exitButton"
        Me.exitButton.Size = New System.Drawing.Size(32, 32)
        Me.exitButton.TabIndex = 0
        Me.exitButton.UseVisualStyleBackColor = True
        '
        'searchGroup
        '
        Me.searchGroup.Controls.Add(Me.subtitleLabel)
        Me.searchGroup.Controls.Add(Me.testTypeChkComboBox)
        Me.searchGroup.Controls.Add(Me.sampleTypeChkComboBox)
        Me.searchGroup.Controls.Add(Me.sampleTypeLabel)
        Me.searchGroup.Controls.Add(Me.testNameTextBox)
        Me.searchGroup.Controls.Add(Me.sampleIdTextBox)
        Me.searchGroup.Controls.Add(Me.analyzerIDLabel)
        Me.searchGroup.Controls.Add(Me.testTypeLabel)
        Me.searchGroup.Controls.Add(Me.statFlagLabel)
        Me.searchGroup.Controls.Add(Me.analyzerIDComboBox)
        Me.searchGroup.Controls.Add(Me.statFlagComboBox)
        Me.searchGroup.Controls.Add(Me.testNameLabel)
        Me.searchGroup.Controls.Add(Me.dateToDateTimePick)
        Me.searchGroup.Controls.Add(Me.sampleIdLabel)
        Me.searchGroup.Controls.Add(Me.dateToLabel)
        Me.searchGroup.Controls.Add(Me.dateFromDateTimePick)
        Me.searchGroup.Controls.Add(Me.dateFromLabel)
        Me.searchGroup.Controls.Add(Me.searchButton)
        Me.searchGroup.ForeColor = System.Drawing.Color.Black
        Me.searchGroup.Location = New System.Drawing.Point(11, 10)
        Me.searchGroup.Name = "searchGroup"
        Me.searchGroup.Size = New System.Drawing.Size(957, 136)
        Me.searchGroup.TabIndex = 26
        Me.searchGroup.TabStop = False
        '
        'subtitleLabel
        '
        Me.subtitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.subtitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.subtitleLabel.ForeColor = System.Drawing.Color.Black
        Me.subtitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.subtitleLabel.Name = "subtitleLabel"
        Me.subtitleLabel.Size = New System.Drawing.Size(934, 20)
        Me.subtitleLabel.TabIndex = 26
        Me.subtitleLabel.Text = "*Historical Results patient"
        Me.subtitleLabel.Title = True
        '
        'testTypeChkComboBox
        '
        Me.testTypeChkComboBox.EditValue = ""
        Me.testTypeChkComboBox.Location = New System.Drawing.Point(701, 62)
        Me.testTypeChkComboBox.Name = "testTypeChkComboBox"
        Me.testTypeChkComboBox.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.testTypeChkComboBox.Properties.HideSelection = False
        Me.testTypeChkComboBox.Properties.LookAndFeel.UseDefaultLookAndFeel = False
        Me.testTypeChkComboBox.Properties.LookAndFeel.UseWindowsXPTheme = True
        Me.testTypeChkComboBox.Properties.PopupFormSize = New System.Drawing.Size(110, 110)
        Me.testTypeChkComboBox.Properties.PopupSizeable = False
        Me.testTypeChkComboBox.Properties.SelectAllItemCaption = "All"
        Me.testTypeChkComboBox.Properties.ShowButtons = False
        Me.testTypeChkComboBox.Properties.ShowPopupCloseButton = False
        Me.testTypeChkComboBox.Size = New System.Drawing.Size(170, 20)
        Me.testTypeChkComboBox.TabIndex = 85
        '
        'sampleTypeChkComboBox
        '
        Me.sampleTypeChkComboBox.EditValue = ""
        Me.sampleTypeChkComboBox.Location = New System.Drawing.Point(512, 106)
        Me.sampleTypeChkComboBox.Name = "sampleTypeChkComboBox"
        Me.sampleTypeChkComboBox.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.sampleTypeChkComboBox.Properties.DropDownRows = 15
        Me.sampleTypeChkComboBox.Properties.HideSelection = False
        Me.sampleTypeChkComboBox.Properties.LookAndFeel.UseDefaultLookAndFeel = False
        Me.sampleTypeChkComboBox.Properties.LookAndFeel.UseWindowsXPTheme = True
        Me.sampleTypeChkComboBox.Properties.PopupFormSize = New System.Drawing.Size(110, 150)
        Me.sampleTypeChkComboBox.Properties.PopupSizeable = False
        Me.sampleTypeChkComboBox.Properties.SelectAllItemCaption = "All"
        Me.sampleTypeChkComboBox.Properties.ShowButtons = False
        Me.sampleTypeChkComboBox.Properties.ShowPopupCloseButton = False
        Me.sampleTypeChkComboBox.Size = New System.Drawing.Size(157, 20)
        Me.sampleTypeChkComboBox.TabIndex = 84
        '
        'sampleTypeLabel
        '
        Me.sampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.sampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.sampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.sampleTypeLabel.Location = New System.Drawing.Point(512, 91)
        Me.sampleTypeLabel.Name = "sampleTypeLabel"
        Me.sampleTypeLabel.Size = New System.Drawing.Size(157, 13)
        Me.sampleTypeLabel.TabIndex = 83
        Me.sampleTypeLabel.Text = "* Sample Type:"
        Me.sampleTypeLabel.Title = False
        '
        'testNameTextBox
        '
        Me.testNameTextBox.BackColor = System.Drawing.Color.White
        Me.testNameTextBox.DecimalsValues = False
        Me.testNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.testNameTextBox.IsNumeric = False
        Me.testNameTextBox.Location = New System.Drawing.Point(701, 106)
        Me.testNameTextBox.Mandatory = False
        Me.testNameTextBox.Name = "testNameTextBox"
        Me.testNameTextBox.Size = New System.Drawing.Size(170, 21)
        Me.testNameTextBox.TabIndex = 79
        '
        'sampleIdTextBox
        '
        Me.sampleIdTextBox.BackColor = System.Drawing.Color.White
        Me.sampleIdTextBox.DecimalsValues = False
        Me.sampleIdTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.sampleIdTextBox.IsNumeric = False
        Me.sampleIdTextBox.Location = New System.Drawing.Point(309, 62)
        Me.sampleIdTextBox.Mandatory = False
        Me.sampleIdTextBox.Name = "sampleIdTextBox"
        Me.sampleIdTextBox.Size = New System.Drawing.Size(170, 21)
        Me.sampleIdTextBox.TabIndex = 79
        '
        'analyzerIDLabel
        '
        Me.analyzerIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.analyzerIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.analyzerIDLabel.ForeColor = System.Drawing.Color.Black
        Me.analyzerIDLabel.Location = New System.Drawing.Point(306, 91)
        Me.analyzerIDLabel.Name = "analyzerIDLabel"
        Me.analyzerIDLabel.Size = New System.Drawing.Size(173, 13)
        Me.analyzerIDLabel.TabIndex = 77
        Me.analyzerIDLabel.Text = "* Analyzer:"
        Me.analyzerIDLabel.Title = False
        '
        'testTypeLabel
        '
        Me.testTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.testTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.testTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.testTypeLabel.Location = New System.Drawing.Point(698, 47)
        Me.testTypeLabel.Name = "testTypeLabel"
        Me.testTypeLabel.Size = New System.Drawing.Size(173, 13)
        Me.testTypeLabel.TabIndex = 78
        Me.testTypeLabel.Text = "* Test Type:"
        Me.testTypeLabel.Title = False
        '
        'statFlagLabel
        '
        Me.statFlagLabel.BackColor = System.Drawing.Color.Transparent
        Me.statFlagLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.statFlagLabel.ForeColor = System.Drawing.Color.Black
        Me.statFlagLabel.Location = New System.Drawing.Point(509, 47)
        Me.statFlagLabel.Name = "statFlagLabel"
        Me.statFlagLabel.Size = New System.Drawing.Size(160, 13)
        Me.statFlagLabel.TabIndex = 78
        Me.statFlagLabel.Text = "* Stat Flag:"
        Me.statFlagLabel.Title = False
        '
        'analyzerIDComboBox
        '
        Me.analyzerIDComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.analyzerIDComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.analyzerIDComboBox.ForeColor = System.Drawing.Color.Black
        Me.analyzerIDComboBox.FormattingEnabled = True
        Me.analyzerIDComboBox.Location = New System.Drawing.Point(309, 106)
        Me.analyzerIDComboBox.Name = "analyzerIDComboBox"
        Me.analyzerIDComboBox.Size = New System.Drawing.Size(170, 21)
        Me.analyzerIDComboBox.TabIndex = 75
        '
        'statFlagComboBox
        '
        Me.statFlagComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.statFlagComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.statFlagComboBox.ForeColor = System.Drawing.Color.Black
        Me.statFlagComboBox.FormattingEnabled = True
        Me.statFlagComboBox.Location = New System.Drawing.Point(512, 62)
        Me.statFlagComboBox.Name = "statFlagComboBox"
        Me.statFlagComboBox.Size = New System.Drawing.Size(157, 21)
        Me.statFlagComboBox.TabIndex = 76
        '
        'testNameLabel
        '
        Me.testNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.testNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.testNameLabel.ForeColor = System.Drawing.Color.Black
        Me.testNameLabel.Location = New System.Drawing.Point(701, 91)
        Me.testNameLabel.Name = "testNameLabel"
        Me.testNameLabel.Size = New System.Drawing.Size(170, 13)
        Me.testNameLabel.TabIndex = 66
        Me.testNameLabel.Text = "* Test Name:"
        Me.testNameLabel.Title = False
        '
        'dateToDateTimePick
        '
        Me.dateToDateTimePick.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
        Me.dateToDateTimePick.CalendarForeColor = System.Drawing.Color.Black
        Me.dateToDateTimePick.CalendarMonthBackground = System.Drawing.Color.White
        Me.dateToDateTimePick.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.dateToDateTimePick.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.dateToDateTimePick.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.dateToDateTimePick.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.dateToDateTimePick.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dateToDateTimePick.Location = New System.Drawing.Point(162, 62)
        Me.dateToDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.dateToDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.dateToDateTimePick.Name = "dateToDateTimePick"
        Me.dateToDateTimePick.Size = New System.Drawing.Size(108, 21)
        Me.dateToDateTimePick.TabIndex = 64
        '
        'sampleIdLabel
        '
        Me.sampleIdLabel.BackColor = System.Drawing.Color.Transparent
        Me.sampleIdLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.sampleIdLabel.ForeColor = System.Drawing.Color.Black
        Me.sampleIdLabel.Location = New System.Drawing.Point(309, 47)
        Me.sampleIdLabel.Name = "sampleIdLabel"
        Me.sampleIdLabel.Size = New System.Drawing.Size(170, 13)
        Me.sampleIdLabel.TabIndex = 66
        Me.sampleIdLabel.Text = "* Patient ID\Sample ID:"
        Me.sampleIdLabel.Title = False
        '
        'dateToLabel
        '
        Me.dateToLabel.BackColor = System.Drawing.Color.Transparent
        Me.dateToLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.dateToLabel.ForeColor = System.Drawing.Color.Black
        Me.dateToLabel.Location = New System.Drawing.Point(162, 47)
        Me.dateToLabel.Name = "dateToLabel"
        Me.dateToLabel.Size = New System.Drawing.Size(108, 13)
        Me.dateToLabel.TabIndex = 66
        Me.dateToLabel.Text = "* To:"
        Me.dateToLabel.Title = False
        '
        'dateFromDateTimePick
        '
        Me.dateFromDateTimePick.CalendarFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dateFromDateTimePick.CalendarForeColor = System.Drawing.Color.Black
        Me.dateFromDateTimePick.CalendarMonthBackground = System.Drawing.Color.White
        Me.dateFromDateTimePick.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.dateFromDateTimePick.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.dateFromDateTimePick.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.dateFromDateTimePick.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dateFromDateTimePick.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dateFromDateTimePick.Location = New System.Drawing.Point(33, 62)
        Me.dateFromDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.dateFromDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.dateFromDateTimePick.Name = "dateFromDateTimePick"
        Me.dateFromDateTimePick.Size = New System.Drawing.Size(108, 21)
        Me.dateFromDateTimePick.TabIndex = 63
        '
        'dateFromLabel
        '
        Me.dateFromLabel.BackColor = System.Drawing.Color.Transparent
        Me.dateFromLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.dateFromLabel.ForeColor = System.Drawing.Color.Black
        Me.dateFromLabel.Location = New System.Drawing.Point(33, 47)
        Me.dateFromLabel.Name = "dateFromLabel"
        Me.dateFromLabel.Size = New System.Drawing.Size(108, 13)
        Me.dateFromLabel.TabIndex = 65
        Me.dateFromLabel.Text = "*From:"
        Me.dateFromLabel.Title = False
        '
        'searchButton
        '
        Me.searchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.searchButton.Location = New System.Drawing.Point(912, 50)
        Me.searchButton.Name = "searchButton"
        Me.searchButton.Size = New System.Drawing.Size(32, 32)
        Me.searchButton.TabIndex = 4
        Me.searchButton.UseVisualStyleBackColor = True
        '
        'historyGroup
        '
        Me.historyGroup.Controls.Add(Me.CompactPrintButton)
        Me.historyGroup.Controls.Add(Me.PrintButton)
        Me.historyGroup.Controls.Add(Me.historyGrid)
        Me.historyGroup.Controls.Add(Me.exportButton)
        Me.historyGroup.Controls.Add(Me.historyDeleteButton)
        Me.historyGroup.ForeColor = System.Drawing.Color.Black
        Me.historyGroup.Location = New System.Drawing.Point(11, 144)
        Me.historyGroup.Name = "historyGroup"
        Me.historyGroup.Size = New System.Drawing.Size(957, 465)
        Me.historyGroup.TabIndex = 27
        Me.historyGroup.TabStop = False
        '
        'CompactPrintButton
        '
        Me.CompactPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.CompactPrintButton.Location = New System.Drawing.Point(912, 134)
        Me.CompactPrintButton.Name = "CompactPrintButton"
        Me.CompactPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.CompactPrintButton.TabIndex = 29
        Me.CompactPrintButton.UseVisualStyleBackColor = True
        '
        'PrintButton
        '
        Me.PrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PrintButton.Location = New System.Drawing.Point(912, 96)
        Me.PrintButton.Name = "PrintButton"
        Me.PrintButton.Size = New System.Drawing.Size(32, 32)
        Me.PrintButton.TabIndex = 28
        Me.PrintButton.UseVisualStyleBackColor = True
        '
        'historyGrid
        '
        Me.historyGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.historyGrid.EmbeddedNavigator.Buttons.Append.Visible = False
        Me.historyGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = False
        Me.historyGrid.EmbeddedNavigator.Buttons.Edit.Visible = False
        Me.historyGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = False
        Me.historyGrid.EmbeddedNavigator.Buttons.Next.Visible = False
        Me.historyGrid.EmbeddedNavigator.Buttons.Prev.Visible = False
        Me.historyGrid.EmbeddedNavigator.Buttons.Remove.Visible = False
        Me.historyGrid.EmbeddedNavigator.TextStringFormat = "{0} / {1}"
        Me.historyGrid.Location = New System.Drawing.Point(10, 20)
        Me.historyGrid.LookAndFeel.UseWindowsXPTheme = True
        Me.historyGrid.MainView = Me.historyGridView
        Me.historyGrid.Name = "historyGrid"
        Me.historyGrid.Size = New System.Drawing.Size(893, 439)
        Me.historyGrid.TabIndex = 3
        Me.historyGrid.UseEmbeddedNavigator = True
        Me.historyGrid.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.historyGridView})
        '
        'historyGridView
        '
        Me.historyGridView.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.historyGridView.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.historyGridView.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.historyGridView.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.historyGridView.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.historyGridView.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.historyGridView.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.historyGridView.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.historyGridView.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.historyGridView.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.historyGridView.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.historyGridView.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.historyGridView.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.historyGridView.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.historyGridView.Appearance.Empty.Options.UseBackColor = True
        Me.historyGridView.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.historyGridView.Appearance.EvenRow.Options.UseBackColor = True
        Me.historyGridView.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.historyGridView.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.historyGridView.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.historyGridView.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.historyGridView.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.historyGridView.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.historyGridView.Appearance.FilterPanel.Options.UseBackColor = True
        Me.historyGridView.Appearance.FilterPanel.Options.UseForeColor = True
        Me.historyGridView.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent
        Me.historyGridView.Appearance.FocusedCell.Options.UseBackColor = True
        Me.historyGridView.Appearance.FocusedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.historyGridView.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.historyGridView.Appearance.FocusedRow.Options.UseBackColor = True
        Me.historyGridView.Appearance.FocusedRow.Options.UseForeColor = True
        Me.historyGridView.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.historyGridView.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.historyGridView.Appearance.FooterPanel.Options.UseBackColor = True
        Me.historyGridView.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.historyGridView.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.historyGridView.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.historyGridView.Appearance.GroupButton.Options.UseBackColor = True
        Me.historyGridView.Appearance.GroupButton.Options.UseBorderColor = True
        Me.historyGridView.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.historyGridView.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.historyGridView.Appearance.GroupFooter.Options.UseBackColor = True
        Me.historyGridView.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.historyGridView.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.historyGridView.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.historyGridView.Appearance.GroupPanel.Options.UseBackColor = True
        Me.historyGridView.Appearance.GroupPanel.Options.UseForeColor = True
        Me.historyGridView.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.historyGridView.Appearance.GroupRow.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.historyGridView.Appearance.GroupRow.Options.UseBackColor = True
        Me.historyGridView.Appearance.GroupRow.Options.UseFont = True
        Me.historyGridView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.historyGridView.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.historyGridView.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.historyGridView.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.historyGridView.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.historyGridView.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.historyGridView.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.historyGridView.Appearance.HorzLine.Options.UseBackColor = True
        Me.historyGridView.Appearance.OddRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.historyGridView.Appearance.OddRow.Options.UseBackColor = True
        Me.historyGridView.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.historyGridView.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.historyGridView.Appearance.Preview.Options.UseBackColor = True
        Me.historyGridView.Appearance.Preview.Options.UseForeColor = True
        Me.historyGridView.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.historyGridView.Appearance.Row.Options.UseBackColor = True
        Me.historyGridView.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.historyGridView.Appearance.RowSeparator.Options.UseBackColor = True
        Me.historyGridView.Appearance.SelectedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.historyGridView.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White
        Me.historyGridView.Appearance.SelectedRow.Options.UseBackColor = True
        Me.historyGridView.Appearance.SelectedRow.Options.UseForeColor = True
        Me.historyGridView.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.historyGridView.Appearance.VertLine.Options.UseBackColor = True
        Me.historyGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.historyGridView.GridControl = Me.historyGrid
        Me.historyGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.historyGridView.Name = "historyGridView"
        Me.historyGridView.OptionsCustomization.AllowColumnMoving = False
        Me.historyGridView.OptionsCustomization.AllowFilter = False
        Me.historyGridView.OptionsCustomization.AllowQuickHideColumns = False
        Me.historyGridView.OptionsCustomization.AllowSort = False
        Me.historyGridView.OptionsFind.AllowFindPanel = False
        Me.historyGridView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.historyGridView.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.historyGridView.OptionsView.ShowGroupPanel = False
        Me.historyGridView.PaintStyleName = "WindowsXP"
        '
        'exportButton
        '
        Me.exportButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.exportButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.exportButton.Location = New System.Drawing.Point(912, 58)
        Me.exportButton.Name = "exportButton"
        Me.exportButton.Size = New System.Drawing.Size(32, 32)
        Me.exportButton.TabIndex = 2
        Me.exportButton.UseVisualStyleBackColor = True
        '
        'historyDeleteButton
        '
        Me.historyDeleteButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.historyDeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.historyDeleteButton.Location = New System.Drawing.Point(912, 20)
        Me.historyDeleteButton.Name = "historyDeleteButton"
        Me.historyDeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.historyDeleteButton.TabIndex = 2
        Me.historyDeleteButton.UseVisualStyleBackColor = True
        '
        'IHisResults
        '
        Me.AcceptButton = Me.exitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.Controls.Add(Me.searchGroup)
        Me.Controls.Add(Me.historyGroup)
        Me.Controls.Add(Me.exitButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IHisResults"
        Me.ShowInTaskbar = False
        Me.Text = "IHisAlarms"
        Me.searchGroup.ResumeLayout(False)
        Me.searchGroup.PerformLayout()
        CType(Me.testTypeChkComboBox.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sampleTypeChkComboBox.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.historyGroup.ResumeLayout(False)
        CType(Me.historyGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.historyGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents exitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents searchGroup As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents historyGroup As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents searchButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents historyDeleteButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents subtitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents historyGrid As DevExpress.XtraGrid.GridControl
    Friend WithEvents historyGridView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents dateToDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents dateToLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents dateFromDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents dateFromLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents analyzerIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents statFlagLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents analyzerIDComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents statFlagComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents sampleIdTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents sampleIdLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents sampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents sampleTypeChkComboBox As DevExpress.XtraEditors.CheckedComboBoxEdit
    Friend WithEvents testTypeChkComboBox As DevExpress.XtraEditors.CheckedComboBoxEdit
    Friend WithEvents testTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents testNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents testNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents exportButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents PrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents CompactPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
End Class
