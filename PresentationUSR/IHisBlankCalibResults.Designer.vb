<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IHisBlankCalibResults
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
        Me.analyzerIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.analyzerIDComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.testNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.testNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.dateToDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.dateToLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.dateFromDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.dateFromLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.searchButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.historyGroup = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsCalibsGridLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsBlanksGridLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.xtraCalibratorsGrid = New DevExpress.XtraGrid.GridControl()
        Me.CalibratorGridView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.xtraBlanksGrid = New DevExpress.XtraGrid.GridControl()
        Me.BlankGridView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.printButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.searchGroup.SuspendLayout()
        Me.historyGroup.SuspendLayout()
        CType(Me.xtraCalibratorsGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CalibratorGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.xtraBlanksGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BlankGridView, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.searchGroup.Controls.Add(Me.analyzerIDLabel)
        Me.searchGroup.Controls.Add(Me.analyzerIDComboBox)
        Me.searchGroup.Controls.Add(Me.testNameTextBox)
        Me.searchGroup.Controls.Add(Me.testNameLabel)
        Me.searchGroup.Controls.Add(Me.dateToDateTimePick)
        Me.searchGroup.Controls.Add(Me.dateToLabel)
        Me.searchGroup.Controls.Add(Me.dateFromDateTimePick)
        Me.searchGroup.Controls.Add(Me.dateFromLabel)
        Me.searchGroup.Controls.Add(Me.searchButton)
        Me.searchGroup.ForeColor = System.Drawing.Color.Black
        Me.searchGroup.Location = New System.Drawing.Point(11, 10)
        Me.searchGroup.Name = "searchGroup"
        Me.searchGroup.Size = New System.Drawing.Size(957, 95)
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
        Me.subtitleLabel.Text = "*Historical Results"
        Me.subtitleLabel.Title = True
        '
        'analyzerIDLabel
        '
        Me.analyzerIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.analyzerIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.analyzerIDLabel.ForeColor = System.Drawing.Color.Black
        Me.analyzerIDLabel.Location = New System.Drawing.Point(549, 47)
        Me.analyzerIDLabel.Name = "analyzerIDLabel"
        Me.analyzerIDLabel.Size = New System.Drawing.Size(173, 13)
        Me.analyzerIDLabel.TabIndex = 81
        Me.analyzerIDLabel.Text = "* Analyzer:"
        Me.analyzerIDLabel.Title = False
        '
        'analyzerIDComboBox
        '
        Me.analyzerIDComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.analyzerIDComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.analyzerIDComboBox.ForeColor = System.Drawing.Color.Black
        Me.analyzerIDComboBox.FormattingEnabled = True
        Me.analyzerIDComboBox.Location = New System.Drawing.Point(549, 62)
        Me.analyzerIDComboBox.Name = "analyzerIDComboBox"
        Me.analyzerIDComboBox.Size = New System.Drawing.Size(170, 21)
        Me.analyzerIDComboBox.TabIndex = 3
        '
        'testNameTextBox
        '
        Me.testNameTextBox.BackColor = System.Drawing.Color.White
        Me.testNameTextBox.DecimalsValues = False
        Me.testNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.testNameTextBox.IsNumeric = False
        Me.testNameTextBox.Location = New System.Drawing.Point(338, 62)
        Me.testNameTextBox.Mandatory = False
        Me.testNameTextBox.MaxLength = 16
        Me.testNameTextBox.Name = "testNameTextBox"
        Me.testNameTextBox.Size = New System.Drawing.Size(170, 21)
        Me.testNameTextBox.TabIndex = 4
        '
        'testNameLabel
        '
        Me.testNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.testNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.testNameLabel.ForeColor = System.Drawing.Color.Black
        Me.testNameLabel.Location = New System.Drawing.Point(338, 47)
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
        Me.dateToDateTimePick.Location = New System.Drawing.Point(159, 62)
        Me.dateToDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.dateToDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.dateToDateTimePick.Name = "dateToDateTimePick"
        Me.dateToDateTimePick.Size = New System.Drawing.Size(108, 21)
        Me.dateToDateTimePick.TabIndex = 2
        '
        'dateToLabel
        '
        Me.dateToLabel.BackColor = System.Drawing.Color.Transparent
        Me.dateToLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.dateToLabel.ForeColor = System.Drawing.Color.Black
        Me.dateToLabel.Location = New System.Drawing.Point(159, 47)
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
        Me.dateFromDateTimePick.TabIndex = 1
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
        Me.searchButton.Location = New System.Drawing.Point(912, 51)
        Me.searchButton.Name = "searchButton"
        Me.searchButton.Size = New System.Drawing.Size(32, 32)
        Me.searchButton.TabIndex = 5
        Me.searchButton.UseVisualStyleBackColor = True
        '
        'historyGroup
        '
        Me.historyGroup.Controls.Add(Me.BsCalibsGridLabel1)
        Me.historyGroup.Controls.Add(Me.BsBlanksGridLabel1)
        Me.historyGroup.Controls.Add(Me.xtraCalibratorsGrid)
        Me.historyGroup.Controls.Add(Me.xtraBlanksGrid)
        Me.historyGroup.ForeColor = System.Drawing.Color.Black
        Me.historyGroup.Location = New System.Drawing.Point(11, 104)
        Me.historyGroup.Name = "historyGroup"
        Me.historyGroup.Size = New System.Drawing.Size(957, 505)
        Me.historyGroup.TabIndex = 27
        Me.historyGroup.TabStop = False
        '
        'BsCalibsGridLabel1
        '
        Me.BsCalibsGridLabel1.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsCalibsGridLabel1.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsCalibsGridLabel1.ForeColor = System.Drawing.Color.Black
        Me.BsCalibsGridLabel1.Location = New System.Drawing.Point(10, 265)
        Me.BsCalibsGridLabel1.Name = "BsCalibsGridLabel1"
        Me.BsCalibsGridLabel1.Size = New System.Drawing.Size(934, 20)
        Me.BsCalibsGridLabel1.TabIndex = 28
        Me.BsCalibsGridLabel1.Text = "*Calibration Historical Results"
        Me.BsCalibsGridLabel1.Title = True
        '
        'BsBlanksGridLabel1
        '
        Me.BsBlanksGridLabel1.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsBlanksGridLabel1.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsBlanksGridLabel1.ForeColor = System.Drawing.Color.Black
        Me.BsBlanksGridLabel1.Location = New System.Drawing.Point(10, 15)
        Me.BsBlanksGridLabel1.Name = "BsBlanksGridLabel1"
        Me.BsBlanksGridLabel1.Size = New System.Drawing.Size(934, 20)
        Me.BsBlanksGridLabel1.TabIndex = 27
        Me.BsBlanksGridLabel1.Text = "*Blank Historical Results"
        Me.BsBlanksGridLabel1.Title = True
        '
        'xtraCalibratorsGrid
        '
        Me.xtraCalibratorsGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.xtraCalibratorsGrid.EmbeddedNavigator.Buttons.Append.Visible = False
        Me.xtraCalibratorsGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = False
        Me.xtraCalibratorsGrid.EmbeddedNavigator.Buttons.Edit.Visible = False
        Me.xtraCalibratorsGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = False
        Me.xtraCalibratorsGrid.EmbeddedNavigator.Buttons.Next.Visible = False
        Me.xtraCalibratorsGrid.EmbeddedNavigator.Buttons.Prev.Visible = False
        Me.xtraCalibratorsGrid.EmbeddedNavigator.Buttons.Remove.Visible = False
        Me.xtraCalibratorsGrid.EmbeddedNavigator.TextStringFormat = "{0} / {1}"
        Me.xtraCalibratorsGrid.Location = New System.Drawing.Point(10, 288)
        Me.xtraCalibratorsGrid.LookAndFeel.UseWindowsXPTheme = True
        Me.xtraCalibratorsGrid.MainView = Me.CalibratorGridView
        Me.xtraCalibratorsGrid.Name = "xtraCalibratorsGrid"
        Me.xtraCalibratorsGrid.Size = New System.Drawing.Size(934, 210)
        Me.xtraCalibratorsGrid.TabIndex = 4
        Me.xtraCalibratorsGrid.UseEmbeddedNavigator = True
        Me.xtraCalibratorsGrid.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.CalibratorGridView})
        '
        'CalibratorGridView
        '
        Me.CalibratorGridView.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.CalibratorGridView.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.CalibratorGridView.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.CalibratorGridView.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.CalibratorGridView.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.CalibratorGridView.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.CalibratorGridView.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.CalibratorGridView.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.CalibratorGridView.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.CalibratorGridView.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.CalibratorGridView.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.CalibratorGridView.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.CalibratorGridView.Appearance.Empty.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.CalibratorGridView.Appearance.EvenRow.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.CalibratorGridView.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.CalibratorGridView.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.CalibratorGridView.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.CalibratorGridView.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.CalibratorGridView.Appearance.FilterPanel.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.FilterPanel.Options.UseForeColor = True
        Me.CalibratorGridView.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent
        Me.CalibratorGridView.Appearance.FocusedCell.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.FocusedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.CalibratorGridView.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.CalibratorGridView.Appearance.FocusedRow.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.FocusedRow.Options.UseForeColor = True
        Me.CalibratorGridView.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.CalibratorGridView.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.CalibratorGridView.Appearance.FooterPanel.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.CalibratorGridView.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.CalibratorGridView.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.CalibratorGridView.Appearance.GroupButton.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.GroupButton.Options.UseBorderColor = True
        Me.CalibratorGridView.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.CalibratorGridView.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.CalibratorGridView.Appearance.GroupFooter.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.CalibratorGridView.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.CalibratorGridView.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.CalibratorGridView.Appearance.GroupPanel.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.GroupPanel.Options.UseForeColor = True
        Me.CalibratorGridView.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.CalibratorGridView.Appearance.GroupRow.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CalibratorGridView.Appearance.GroupRow.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.GroupRow.Options.UseFont = True
        Me.CalibratorGridView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.CalibratorGridView.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.CalibratorGridView.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.CalibratorGridView.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.CalibratorGridView.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.CalibratorGridView.Appearance.HorzLine.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.OddRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.CalibratorGridView.Appearance.OddRow.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.CalibratorGridView.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.CalibratorGridView.Appearance.Preview.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.Preview.Options.UseForeColor = True
        Me.CalibratorGridView.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.CalibratorGridView.Appearance.Row.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.CalibratorGridView.Appearance.RowSeparator.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.SelectedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.CalibratorGridView.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White
        Me.CalibratorGridView.Appearance.SelectedRow.Options.UseBackColor = True
        Me.CalibratorGridView.Appearance.SelectedRow.Options.UseForeColor = True
        Me.CalibratorGridView.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.CalibratorGridView.Appearance.VertLine.Options.UseBackColor = True
        Me.CalibratorGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.CalibratorGridView.GridControl = Me.xtraCalibratorsGrid
        Me.CalibratorGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.CalibratorGridView.Name = "CalibratorGridView"
        Me.CalibratorGridView.OptionsCustomization.AllowColumnMoving = False
        Me.CalibratorGridView.OptionsCustomization.AllowFilter = False
        Me.CalibratorGridView.OptionsCustomization.AllowQuickHideColumns = False
        Me.CalibratorGridView.OptionsCustomization.AllowSort = False
        Me.CalibratorGridView.OptionsFind.AllowFindPanel = False
        Me.CalibratorGridView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.CalibratorGridView.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.CalibratorGridView.OptionsView.ShowGroupPanel = False
        Me.CalibratorGridView.PaintStyleName = "WindowsXP"
        '
        'xtraBlanksGrid
        '
        Me.xtraBlanksGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.xtraBlanksGrid.EmbeddedNavigator.Buttons.Append.Visible = False
        Me.xtraBlanksGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = False
        Me.xtraBlanksGrid.EmbeddedNavigator.Buttons.Edit.Visible = False
        Me.xtraBlanksGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = False
        Me.xtraBlanksGrid.EmbeddedNavigator.Buttons.Next.Visible = False
        Me.xtraBlanksGrid.EmbeddedNavigator.Buttons.Prev.Visible = False
        Me.xtraBlanksGrid.EmbeddedNavigator.Buttons.Remove.Visible = False
        Me.xtraBlanksGrid.EmbeddedNavigator.TextStringFormat = "{0} / {1}"
        Me.xtraBlanksGrid.Location = New System.Drawing.Point(10, 38)
        Me.xtraBlanksGrid.LookAndFeel.UseWindowsXPTheme = True
        Me.xtraBlanksGrid.MainView = Me.BlankGridView
        Me.xtraBlanksGrid.Name = "xtraBlanksGrid"
        Me.xtraBlanksGrid.Size = New System.Drawing.Size(934, 210)
        Me.xtraBlanksGrid.TabIndex = 3
        Me.xtraBlanksGrid.UseEmbeddedNavigator = True
        Me.xtraBlanksGrid.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.BlankGridView})
        '
        'BlankGridView
        '
        Me.BlankGridView.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.BlankGridView.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.BlankGridView.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.BlankGridView.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.BlankGridView.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.BlankGridView.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.BlankGridView.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.BlankGridView.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.BlankGridView.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.BlankGridView.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.BlankGridView.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.BlankGridView.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.BlankGridView.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.BlankGridView.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.BlankGridView.Appearance.Empty.Options.UseBackColor = True
        Me.BlankGridView.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.BlankGridView.Appearance.EvenRow.Options.UseBackColor = True
        Me.BlankGridView.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.BlankGridView.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.BlankGridView.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.BlankGridView.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.BlankGridView.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.BlankGridView.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.BlankGridView.Appearance.FilterPanel.Options.UseBackColor = True
        Me.BlankGridView.Appearance.FilterPanel.Options.UseForeColor = True
        Me.BlankGridView.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent
        Me.BlankGridView.Appearance.FocusedCell.Options.UseBackColor = True
        Me.BlankGridView.Appearance.FocusedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.BlankGridView.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.BlankGridView.Appearance.FocusedRow.Options.UseBackColor = True
        Me.BlankGridView.Appearance.FocusedRow.Options.UseForeColor = True
        Me.BlankGridView.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.BlankGridView.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.BlankGridView.Appearance.FooterPanel.Options.UseBackColor = True
        Me.BlankGridView.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.BlankGridView.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.BlankGridView.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.BlankGridView.Appearance.GroupButton.Options.UseBackColor = True
        Me.BlankGridView.Appearance.GroupButton.Options.UseBorderColor = True
        Me.BlankGridView.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.BlankGridView.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.BlankGridView.Appearance.GroupFooter.Options.UseBackColor = True
        Me.BlankGridView.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.BlankGridView.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.BlankGridView.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.BlankGridView.Appearance.GroupPanel.Options.UseBackColor = True
        Me.BlankGridView.Appearance.GroupPanel.Options.UseForeColor = True
        Me.BlankGridView.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.BlankGridView.Appearance.GroupRow.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BlankGridView.Appearance.GroupRow.Options.UseBackColor = True
        Me.BlankGridView.Appearance.GroupRow.Options.UseFont = True
        Me.BlankGridView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.BlankGridView.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.BlankGridView.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.BlankGridView.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.BlankGridView.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.BlankGridView.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.BlankGridView.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.BlankGridView.Appearance.HorzLine.Options.UseBackColor = True
        Me.BlankGridView.Appearance.OddRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BlankGridView.Appearance.OddRow.Options.UseBackColor = True
        Me.BlankGridView.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.BlankGridView.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.BlankGridView.Appearance.Preview.Options.UseBackColor = True
        Me.BlankGridView.Appearance.Preview.Options.UseForeColor = True
        Me.BlankGridView.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.BlankGridView.Appearance.Row.Options.UseBackColor = True
        Me.BlankGridView.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.BlankGridView.Appearance.RowSeparator.Options.UseBackColor = True
        Me.BlankGridView.Appearance.SelectedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.BlankGridView.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White
        Me.BlankGridView.Appearance.SelectedRow.Options.UseBackColor = True
        Me.BlankGridView.Appearance.SelectedRow.Options.UseForeColor = True
        Me.BlankGridView.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.BlankGridView.Appearance.VertLine.Options.UseBackColor = True
        Me.BlankGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.BlankGridView.GridControl = Me.xtraBlanksGrid
        Me.BlankGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.BlankGridView.Name = "BlankGridView"
        Me.BlankGridView.OptionsCustomization.AllowColumnMoving = False
        Me.BlankGridView.OptionsCustomization.AllowFilter = False
        Me.BlankGridView.OptionsCustomization.AllowQuickHideColumns = False
        Me.BlankGridView.OptionsCustomization.AllowSort = False
        Me.BlankGridView.OptionsFind.AllowFindPanel = False
        Me.BlankGridView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.BlankGridView.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.BlankGridView.OptionsView.ShowGroupPanel = False
        Me.BlankGridView.PaintStyleName = "WindowsXP"
        '
        'printButton
        '
        Me.printButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.printButton.Location = New System.Drawing.Point(21, 614)
        Me.printButton.Name = "printButton"
        Me.printButton.Size = New System.Drawing.Size(32, 32)
        Me.printButton.TabIndex = 28
        Me.printButton.UseVisualStyleBackColor = True
        '
        'IHisBlankCalibResults
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
        Me.Controls.Add(Me.printButton)
        Me.Controls.Add(Me.searchGroup)
        Me.Controls.Add(Me.historyGroup)
        Me.Controls.Add(Me.exitButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IHisBlankCalibResults"
        Me.ShowInTaskbar = False
        Me.Text = "IHisBlankCalibResults"
        Me.searchGroup.ResumeLayout(False)
        Me.searchGroup.PerformLayout()
        Me.historyGroup.ResumeLayout(False)
        CType(Me.xtraCalibratorsGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CalibratorGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.xtraBlanksGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BlankGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents exitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents searchGroup As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents historyGroup As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents searchButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents subtitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents xtraBlanksGrid As DevExpress.XtraGrid.GridControl
    Friend WithEvents BlankGridView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents dateToDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents dateToLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents dateFromDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents dateFromLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents testNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents testNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents xtraCalibratorsGrid As DevExpress.XtraGrid.GridControl
    Friend WithEvents CalibratorGridView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents BsBlanksGridLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsCalibsGridLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents analyzerIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents analyzerIDComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents printButton As Biosystems.Ax00.Controls.UserControls.BSButton
End Class
