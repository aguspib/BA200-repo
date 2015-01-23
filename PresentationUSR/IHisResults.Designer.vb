<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiHisResults
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
        Me.bsSpecimenIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSpecimenIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.subtitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.testTypesChkComboBox = New DevExpress.XtraEditors.CheckedComboBoxEdit()
        Me.sampleTypesChkComboBox = New DevExpress.XtraEditors.CheckedComboBoxEdit()
        Me.sampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTestNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsPatientDataTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.analyzerIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.testTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.statFlagLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsAnalyzersComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsStatFlagComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.testNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.dateToDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.bsPatientDataLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.dateToLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDateFromDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
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
        CType(Me.testTypesChkComboBox.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.sampleTypesChkComboBox.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.exitButton.TabIndex = 16
        Me.exitButton.UseVisualStyleBackColor = True
        '
        'searchGroup
        '
        Me.searchGroup.Controls.Add(Me.bsSpecimenIDTextBox)
        Me.searchGroup.Controls.Add(Me.bsSpecimenIDLabel)
        Me.searchGroup.Controls.Add(Me.subtitleLabel)
        Me.searchGroup.Controls.Add(Me.testTypesChkComboBox)
        Me.searchGroup.Controls.Add(Me.sampleTypesChkComboBox)
        Me.searchGroup.Controls.Add(Me.sampleTypeLabel)
        Me.searchGroup.Controls.Add(Me.bsTestNameTextBox)
        Me.searchGroup.Controls.Add(Me.bsPatientDataTextBox)
        Me.searchGroup.Controls.Add(Me.analyzerIDLabel)
        Me.searchGroup.Controls.Add(Me.testTypeLabel)
        Me.searchGroup.Controls.Add(Me.statFlagLabel)
        Me.searchGroup.Controls.Add(Me.bsAnalyzersComboBox)
        Me.searchGroup.Controls.Add(Me.bsStatFlagComboBox)
        Me.searchGroup.Controls.Add(Me.testNameLabel)
        Me.searchGroup.Controls.Add(Me.dateToDateTimePick)
        Me.searchGroup.Controls.Add(Me.bsPatientDataLabel)
        Me.searchGroup.Controls.Add(Me.dateToLabel)
        Me.searchGroup.Controls.Add(Me.bsDateFromDateTimePick)
        Me.searchGroup.Controls.Add(Me.dateFromLabel)
        Me.searchGroup.Controls.Add(Me.searchButton)
        Me.searchGroup.ForeColor = System.Drawing.Color.Black
        Me.searchGroup.Location = New System.Drawing.Point(10, 10)
        Me.searchGroup.Name = "searchGroup"
        Me.searchGroup.Size = New System.Drawing.Size(958, 130)
        Me.searchGroup.TabIndex = 26
        Me.searchGroup.TabStop = False
        '
        'bsSpecimenIDTextBox
        '
        Me.bsSpecimenIDTextBox.BackColor = System.Drawing.Color.White
        Me.bsSpecimenIDTextBox.DecimalsValues = False
        Me.bsSpecimenIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSpecimenIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSpecimenIDTextBox.IsNumeric = False
        Me.bsSpecimenIDTextBox.Location = New System.Drawing.Point(289, 101)
        Me.bsSpecimenIDTextBox.Mandatory = False
        Me.bsSpecimenIDTextBox.MaxLength = 30
        Me.bsSpecimenIDTextBox.Name = "bsSpecimenIDTextBox"
        Me.bsSpecimenIDTextBox.Size = New System.Drawing.Size(170, 21)
        Me.bsSpecimenIDTextBox.TabIndex = 7
        '
        'bsSpecimenIDLabel
        '
        Me.bsSpecimenIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSpecimenIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSpecimenIDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSpecimenIDLabel.Location = New System.Drawing.Point(289, 83)
        Me.bsSpecimenIDLabel.Name = "bsSpecimenIDLabel"
        Me.bsSpecimenIDLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsSpecimenIDLabel.TabIndex = 86
        Me.bsSpecimenIDLabel.Text = "* Barcode:"
        Me.bsSpecimenIDLabel.Title = False
        '
        'subtitleLabel
        '
        Me.subtitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.subtitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.subtitleLabel.ForeColor = System.Drawing.Color.Black
        Me.subtitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.subtitleLabel.Name = "subtitleLabel"
        Me.subtitleLabel.Size = New System.Drawing.Size(938, 20)
        Me.subtitleLabel.TabIndex = 26
        Me.subtitleLabel.Text = "*Historic Patient Results"
        Me.subtitleLabel.Title = True
        '
        'testTypesChkComboBox
        '
        Me.testTypesChkComboBox.EditValue = ""
        Me.testTypesChkComboBox.Location = New System.Drawing.Point(707, 57)
        Me.testTypesChkComboBox.Name = "testTypesChkComboBox"
        Me.testTypesChkComboBox.Properties.Appearance.BackColor = System.Drawing.Color.White
        Me.testTypesChkComboBox.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.testTypesChkComboBox.Properties.Appearance.ForeColor = System.Drawing.Color.Black
        Me.testTypesChkComboBox.Properties.Appearance.Options.UseBackColor = True
        Me.testTypesChkComboBox.Properties.Appearance.Options.UseFont = True
        Me.testTypesChkComboBox.Properties.Appearance.Options.UseForeColor = True
        Me.testTypesChkComboBox.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.testTypesChkComboBox.Properties.HideSelection = False
        Me.testTypesChkComboBox.Properties.LookAndFeel.UseDefaultLookAndFeel = False
        Me.testTypesChkComboBox.Properties.LookAndFeel.UseWindowsXPTheme = True
        Me.testTypesChkComboBox.Properties.PopupFormSize = New System.Drawing.Size(110, 110)
        Me.testTypesChkComboBox.Properties.PopupSizeable = False
        Me.testTypesChkComboBox.Properties.SelectAllItemCaption = "All"
        Me.testTypesChkComboBox.Properties.ShowButtons = False
        Me.testTypesChkComboBox.Properties.ShowPopupCloseButton = False
        Me.testTypesChkComboBox.Size = New System.Drawing.Size(170, 20)
        Me.testTypesChkComboBox.TabIndex = 5
        '
        'sampleTypesChkComboBox
        '
        Me.sampleTypesChkComboBox.EditValue = ""
        Me.sampleTypesChkComboBox.Location = New System.Drawing.Point(498, 101)
        Me.sampleTypesChkComboBox.Name = "sampleTypesChkComboBox"
        Me.sampleTypesChkComboBox.Properties.Appearance.BackColor = System.Drawing.Color.White
        Me.sampleTypesChkComboBox.Properties.Appearance.ForeColor = System.Drawing.Color.Black
        Me.sampleTypesChkComboBox.Properties.Appearance.Options.UseBackColor = True
        Me.sampleTypesChkComboBox.Properties.Appearance.Options.UseForeColor = True
        Me.sampleTypesChkComboBox.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.sampleTypesChkComboBox.Properties.DropDownRows = 15
        Me.sampleTypesChkComboBox.Properties.HideSelection = False
        Me.sampleTypesChkComboBox.Properties.LookAndFeel.UseDefaultLookAndFeel = False
        Me.sampleTypesChkComboBox.Properties.LookAndFeel.UseWindowsXPTheme = True
        Me.sampleTypesChkComboBox.Properties.PopupFormSize = New System.Drawing.Size(110, 150)
        Me.sampleTypesChkComboBox.Properties.PopupSizeable = False
        Me.sampleTypesChkComboBox.Properties.SelectAllItemCaption = "All"
        Me.sampleTypesChkComboBox.Properties.ShowButtons = False
        Me.sampleTypesChkComboBox.Properties.ShowPopupCloseButton = False
        Me.sampleTypesChkComboBox.Size = New System.Drawing.Size(170, 20)
        Me.sampleTypesChkComboBox.TabIndex = 8
        '
        'sampleTypeLabel
        '
        Me.sampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.sampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.sampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.sampleTypeLabel.Location = New System.Drawing.Point(498, 83)
        Me.sampleTypeLabel.Name = "sampleTypeLabel"
        Me.sampleTypeLabel.Size = New System.Drawing.Size(170, 13)
        Me.sampleTypeLabel.TabIndex = 83
        Me.sampleTypeLabel.Text = "* Sample Type:"
        Me.sampleTypeLabel.Title = False
        '
        'bsTestNameTextBox
        '
        Me.bsTestNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsTestNameTextBox.DecimalsValues = False
        Me.bsTestNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestNameTextBox.IsNumeric = False
        Me.bsTestNameTextBox.Location = New System.Drawing.Point(707, 101)
        Me.bsTestNameTextBox.Mandatory = False
        Me.bsTestNameTextBox.MaxLength = 16
        Me.bsTestNameTextBox.Name = "bsTestNameTextBox"
        Me.bsTestNameTextBox.Size = New System.Drawing.Size(170, 21)
        Me.bsTestNameTextBox.TabIndex = 9
        '
        'bsPatientDataTextBox
        '
        Me.bsPatientDataTextBox.BackColor = System.Drawing.Color.White
        Me.bsPatientDataTextBox.DecimalsValues = False
        Me.bsPatientDataTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPatientDataTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsPatientDataTextBox.IsNumeric = False
        Me.bsPatientDataTextBox.Location = New System.Drawing.Point(289, 57)
        Me.bsPatientDataTextBox.Mandatory = False
        Me.bsPatientDataTextBox.MaxLength = 30
        Me.bsPatientDataTextBox.Name = "bsPatientDataTextBox"
        Me.bsPatientDataTextBox.Size = New System.Drawing.Size(170, 21)
        Me.bsPatientDataTextBox.TabIndex = 3
        '
        'analyzerIDLabel
        '
        Me.analyzerIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.analyzerIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.analyzerIDLabel.ForeColor = System.Drawing.Color.Black
        Me.analyzerIDLabel.Location = New System.Drawing.Point(10, 83)
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
        Me.testTypeLabel.Location = New System.Drawing.Point(707, 39)
        Me.testTypeLabel.Name = "testTypeLabel"
        Me.testTypeLabel.Size = New System.Drawing.Size(170, 13)
        Me.testTypeLabel.TabIndex = 78
        Me.testTypeLabel.Text = "* Test Type:"
        Me.testTypeLabel.Title = False
        '
        'statFlagLabel
        '
        Me.statFlagLabel.BackColor = System.Drawing.Color.Transparent
        Me.statFlagLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.statFlagLabel.ForeColor = System.Drawing.Color.Black
        Me.statFlagLabel.Location = New System.Drawing.Point(498, 39)
        Me.statFlagLabel.Name = "statFlagLabel"
        Me.statFlagLabel.Size = New System.Drawing.Size(170, 13)
        Me.statFlagLabel.TabIndex = 78
        Me.statFlagLabel.Text = "* Stat Flag:"
        Me.statFlagLabel.Title = False
        '
        'bsAnalyzersComboBox
        '
        Me.bsAnalyzersComboBox.BackColor = System.Drawing.Color.White
        Me.bsAnalyzersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsAnalyzersComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAnalyzersComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsAnalyzersComboBox.FormattingEnabled = True
        Me.bsAnalyzersComboBox.Location = New System.Drawing.Point(10, 101)
        Me.bsAnalyzersComboBox.Name = "bsAnalyzersComboBox"
        Me.bsAnalyzersComboBox.Size = New System.Drawing.Size(240, 21)
        Me.bsAnalyzersComboBox.TabIndex = 6
        '
        'bsStatFlagComboBox
        '
        Me.bsStatFlagComboBox.BackColor = System.Drawing.Color.White
        Me.bsStatFlagComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsStatFlagComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsStatFlagComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsStatFlagComboBox.FormattingEnabled = True
        Me.bsStatFlagComboBox.Location = New System.Drawing.Point(498, 57)
        Me.bsStatFlagComboBox.Name = "bsStatFlagComboBox"
        Me.bsStatFlagComboBox.Size = New System.Drawing.Size(170, 21)
        Me.bsStatFlagComboBox.TabIndex = 4
        '
        'testNameLabel
        '
        Me.testNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.testNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.testNameLabel.ForeColor = System.Drawing.Color.Black
        Me.testNameLabel.Location = New System.Drawing.Point(707, 83)
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
        Me.dateToDateTimePick.Location = New System.Drawing.Point(142, 57)
        Me.dateToDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.dateToDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.dateToDateTimePick.Name = "dateToDateTimePick"
        Me.dateToDateTimePick.Size = New System.Drawing.Size(108, 21)
        Me.dateToDateTimePick.TabIndex = 2
        '
        'bsPatientDataLabel
        '
        Me.bsPatientDataLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsPatientDataLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPatientDataLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPatientDataLabel.Location = New System.Drawing.Point(289, 39)
        Me.bsPatientDataLabel.Name = "bsPatientDataLabel"
        Me.bsPatientDataLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsPatientDataLabel.TabIndex = 66
        Me.bsPatientDataLabel.Text = "* Patient data:"
        Me.bsPatientDataLabel.Title = False
        '
        'dateToLabel
        '
        Me.dateToLabel.BackColor = System.Drawing.Color.Transparent
        Me.dateToLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.dateToLabel.ForeColor = System.Drawing.Color.Black
        Me.dateToLabel.Location = New System.Drawing.Point(142, 39)
        Me.dateToLabel.Name = "dateToLabel"
        Me.dateToLabel.Size = New System.Drawing.Size(108, 13)
        Me.dateToLabel.TabIndex = 66
        Me.dateToLabel.Text = "* To:"
        Me.dateToLabel.Title = False
        '
        'bsDateFromDateTimePick
        '
        Me.bsDateFromDateTimePick.CalendarFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDateFromDateTimePick.CalendarForeColor = System.Drawing.Color.Black
        Me.bsDateFromDateTimePick.CalendarMonthBackground = System.Drawing.Color.White
        Me.bsDateFromDateTimePick.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.bsDateFromDateTimePick.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.bsDateFromDateTimePick.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.bsDateFromDateTimePick.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDateFromDateTimePick.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.bsDateFromDateTimePick.Location = New System.Drawing.Point(10, 57)
        Me.bsDateFromDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsDateFromDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsDateFromDateTimePick.Name = "bsDateFromDateTimePick"
        Me.bsDateFromDateTimePick.Size = New System.Drawing.Size(108, 21)
        Me.bsDateFromDateTimePick.TabIndex = 1
        '
        'dateFromLabel
        '
        Me.dateFromLabel.BackColor = System.Drawing.Color.Transparent
        Me.dateFromLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.dateFromLabel.ForeColor = System.Drawing.Color.Black
        Me.dateFromLabel.Location = New System.Drawing.Point(10, 39)
        Me.dateFromLabel.Name = "dateFromLabel"
        Me.dateFromLabel.Size = New System.Drawing.Size(108, 13)
        Me.dateFromLabel.TabIndex = 65
        Me.dateFromLabel.Text = "*From:"
        Me.dateFromLabel.Title = False
        '
        'searchButton
        '
        Me.searchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.searchButton.Location = New System.Drawing.Point(916, 49)
        Me.searchButton.Name = "searchButton"
        Me.searchButton.Size = New System.Drawing.Size(32, 32)
        Me.searchButton.TabIndex = 10
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
        Me.historyGroup.Location = New System.Drawing.Point(10, 140)
        Me.historyGroup.Name = "historyGroup"
        Me.historyGroup.Size = New System.Drawing.Size(958, 465)
        Me.historyGroup.TabIndex = 27
        Me.historyGroup.TabStop = False
        '
        'CompactPrintButton
        '
        Me.CompactPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.CompactPrintButton.Location = New System.Drawing.Point(916, 129)
        Me.CompactPrintButton.Name = "CompactPrintButton"
        Me.CompactPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.CompactPrintButton.TabIndex = 15
        Me.CompactPrintButton.UseVisualStyleBackColor = True
        '
        'PrintButton
        '
        Me.PrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PrintButton.Location = New System.Drawing.Point(916, 91)
        Me.PrintButton.Name = "PrintButton"
        Me.PrintButton.Size = New System.Drawing.Size(32, 32)
        Me.PrintButton.TabIndex = 14
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
        Me.historyGrid.Location = New System.Drawing.Point(10, 15)
        Me.historyGrid.LookAndFeel.UseWindowsXPTheme = True
        Me.historyGrid.MainView = Me.historyGridView
        Me.historyGrid.Name = "historyGrid"
        Me.historyGrid.Size = New System.Drawing.Size(900, 444)
        Me.historyGrid.TabIndex = 11
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
        Me.exportButton.Location = New System.Drawing.Point(916, 53)
        Me.exportButton.Name = "exportButton"
        Me.exportButton.Size = New System.Drawing.Size(32, 32)
        Me.exportButton.TabIndex = 13
        Me.exportButton.UseVisualStyleBackColor = True
        '
        'historyDeleteButton
        '
        Me.historyDeleteButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.historyDeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.historyDeleteButton.Location = New System.Drawing.Point(916, 15)
        Me.historyDeleteButton.Name = "historyDeleteButton"
        Me.historyDeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.historyDeleteButton.TabIndex = 12
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
        Me.Text = "IHisResults"
        Me.searchGroup.ResumeLayout(False)
        Me.searchGroup.PerformLayout()
        CType(Me.testTypesChkComboBox.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.sampleTypesChkComboBox.Properties, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents bsDateFromDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents dateFromLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents analyzerIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents statFlagLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAnalyzersComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsStatFlagComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsPatientDataTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsPatientDataLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents sampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents sampleTypesChkComboBox As DevExpress.XtraEditors.CheckedComboBoxEdit
    Friend WithEvents testTypesChkComboBox As DevExpress.XtraEditors.CheckedComboBoxEdit
    Friend WithEvents testTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTestNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents testNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents exportButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents PrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents CompactPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSpecimenIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSpecimenIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
End Class
