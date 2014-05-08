<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IHistoricalReports
    Inherits PesentationLayer.BSAdjustmentBaseForm

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
        Me.components = New System.ComponentModel.Container()
        Me.RepositoryItemRichTextEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit()
        Me.TasksCombo = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.TaskLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.ActionsCombo = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.ActionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.DateToDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.DateToLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.DateFromDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.DateFromLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.AnalyzerCombo = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.AnalyzerLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.SearchButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.SaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.PrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.DeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.ExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.CancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.SRVAdjustmentsDSBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.SearchGroupBox = New System.Windows.Forms.GroupBox()
        Me.ActivityGroupBox = New System.Windows.Forms.GroupBox()
        Me.GridControl1 = New DevExpress.XtraGrid.GridControl()
        Me.GridView1 = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.BsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        CType(Me.RepositoryItemRichTextEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SRVAdjustmentsDSBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SearchGroupBox.SuspendLayout()
        Me.ActivityGroupBox.SuspendLayout()
        CType(Me.GridControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsButtonsPanel.SuspendLayout()
        CType(Me.BsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RepositoryItemRichTextEdit1
        '
        Me.RepositoryItemRichTextEdit1.Appearance.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RepositoryItemRichTextEdit1.Appearance.Options.UseFont = True
        Me.RepositoryItemRichTextEdit1.AppearanceDisabled.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RepositoryItemRichTextEdit1.AppearanceDisabled.Options.UseFont = True
        Me.RepositoryItemRichTextEdit1.AppearanceFocused.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RepositoryItemRichTextEdit1.AppearanceFocused.Options.UseFont = True
        Me.RepositoryItemRichTextEdit1.AppearanceReadOnly.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RepositoryItemRichTextEdit1.AppearanceReadOnly.Options.UseFont = True
        Me.RepositoryItemRichTextEdit1.Name = "RepositoryItemRichTextEdit1"
        Me.RepositoryItemRichTextEdit1.ShowCaretInReadOnly = False
        '
        'TasksCombo
        '
        Me.TasksCombo.BackColor = System.Drawing.Color.White
        Me.TasksCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.TasksCombo.ForeColor = System.Drawing.Color.Black
        Me.TasksCombo.FormattingEnabled = True
        Me.TasksCombo.Location = New System.Drawing.Point(564, 20)
        Me.TasksCombo.Name = "TasksCombo"
        Me.TasksCombo.Size = New System.Drawing.Size(120, 21)
        Me.TasksCombo.TabIndex = 4
        '
        'TaskLabel
        '
        Me.TaskLabel.BackColor = System.Drawing.Color.Transparent
        Me.TaskLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.TaskLabel.ForeColor = System.Drawing.Color.Black
        Me.TaskLabel.Location = New System.Drawing.Point(517, 24)
        Me.TaskLabel.Name = "TaskLabel"
        Me.TaskLabel.Size = New System.Drawing.Size(47, 17)
        Me.TaskLabel.TabIndex = 8
        Me.TaskLabel.Text = "Task:"
        Me.TaskLabel.Title = False
        '
        'ActionsCombo
        '
        Me.ActionsCombo.BackColor = System.Drawing.Color.White
        Me.ActionsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ActionsCombo.ForeColor = System.Drawing.Color.Black
        Me.ActionsCombo.FormattingEnabled = True
        Me.ActionsCombo.Location = New System.Drawing.Point(744, 20)
        Me.ActionsCombo.Name = "ActionsCombo"
        Me.ActionsCombo.Size = New System.Drawing.Size(171, 21)
        Me.ActionsCombo.TabIndex = 2
        '
        'ActionLabel
        '
        Me.ActionLabel.BackColor = System.Drawing.Color.Transparent
        Me.ActionLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.ActionLabel.ForeColor = System.Drawing.Color.Black
        Me.ActionLabel.Location = New System.Drawing.Point(686, 24)
        Me.ActionLabel.Name = "ActionLabel"
        Me.ActionLabel.Size = New System.Drawing.Size(55, 17)
        Me.ActionLabel.TabIndex = 5
        Me.ActionLabel.Text = "Action:"
        Me.ActionLabel.Title = False
        '
        'DateToDateTimePick
        '
        Me.DateToDateTimePick.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
        Me.DateToDateTimePick.CalendarForeColor = System.Drawing.Color.Black
        Me.DateToDateTimePick.CalendarMonthBackground = System.Drawing.Color.White
        Me.DateToDateTimePick.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.DateToDateTimePick.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.DateToDateTimePick.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.DateToDateTimePick.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.DateToDateTimePick.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DateToDateTimePick.Location = New System.Drawing.Point(420, 20)
        Me.DateToDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.DateToDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.DateToDateTimePick.Name = "DateToDateTimePick"
        Me.DateToDateTimePick.Size = New System.Drawing.Size(91, 21)
        Me.DateToDateTimePick.TabIndex = 1
        '
        'DateToLabel
        '
        Me.DateToLabel.BackColor = System.Drawing.Color.Transparent
        Me.DateToLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.DateToLabel.ForeColor = System.Drawing.Color.Black
        Me.DateToLabel.Location = New System.Drawing.Point(371, 24)
        Me.DateToLabel.Name = "DateToLabel"
        Me.DateToLabel.Size = New System.Drawing.Size(49, 18)
        Me.DateToLabel.TabIndex = 3
        Me.DateToLabel.Text = "To:"
        Me.DateToLabel.Title = False
        '
        'DateFromDateTimePick
        '
        Me.DateFromDateTimePick.CalendarFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DateFromDateTimePick.CalendarForeColor = System.Drawing.Color.Black
        Me.DateFromDateTimePick.CalendarMonthBackground = System.Drawing.Color.White
        Me.DateFromDateTimePick.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.DateFromDateTimePick.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.DateFromDateTimePick.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.DateFromDateTimePick.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DateFromDateTimePick.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DateFromDateTimePick.Location = New System.Drawing.Point(279, 20)
        Me.DateFromDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.DateFromDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.DateFromDateTimePick.Name = "DateFromDateTimePick"
        Me.DateFromDateTimePick.Size = New System.Drawing.Size(91, 21)
        Me.DateFromDateTimePick.TabIndex = 0
        '
        'DateFromLabel
        '
        Me.DateFromLabel.BackColor = System.Drawing.Color.Transparent
        Me.DateFromLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.DateFromLabel.ForeColor = System.Drawing.Color.Black
        Me.DateFromLabel.Location = New System.Drawing.Point(227, 24)
        Me.DateFromLabel.Name = "DateFromLabel"
        Me.DateFromLabel.Size = New System.Drawing.Size(51, 17)
        Me.DateFromLabel.TabIndex = 1
        Me.DateFromLabel.Text = "From:"
        Me.DateFromLabel.Title = False
        '
        'BsTitleLabel
        '
        Me.BsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTitleLabel.Location = New System.Drawing.Point(10, 10)
        Me.BsTitleLabel.Name = "BsTitleLabel"
        Me.BsTitleLabel.Size = New System.Drawing.Size(963, 20)
        Me.BsTitleLabel.TabIndex = 18
        Me.BsTitleLabel.Text = "Historical Reports"
        Me.BsTitleLabel.Title = True
        '
        'AnalyzerCombo
        '
        Me.AnalyzerCombo.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.AnalyzerCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.AnalyzerCombo.ForeColor = System.Drawing.Color.Black
        Me.AnalyzerCombo.FormattingEnabled = True
        Me.AnalyzerCombo.Location = New System.Drawing.Point(81, 20)
        Me.AnalyzerCombo.Name = "AnalyzerCombo"
        Me.AnalyzerCombo.Size = New System.Drawing.Size(138, 21)
        Me.AnalyzerCombo.TabIndex = 47
        '
        'AnalyzerLabel
        '
        Me.AnalyzerLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.AnalyzerLabel.BackColor = System.Drawing.Color.Transparent
        Me.AnalyzerLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.AnalyzerLabel.ForeColor = System.Drawing.Color.Black
        Me.AnalyzerLabel.Location = New System.Drawing.Point(10, 24)
        Me.AnalyzerLabel.Margin = New System.Windows.Forms.Padding(3, 2, 3, 0)
        Me.AnalyzerLabel.Name = "AnalyzerLabel"
        Me.AnalyzerLabel.Size = New System.Drawing.Size(71, 13)
        Me.AnalyzerLabel.TabIndex = 46
        Me.AnalyzerLabel.Text = "Analyzer:"
        Me.AnalyzerLabel.Title = False
        '
        'SearchButton
        '
        Me.SearchButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.SearchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.SearchButton.Enabled = False
        Me.SearchButton.ForeColor = System.Drawing.Color.Black
        Me.SearchButton.Location = New System.Drawing.Point(924, 14)
        Me.SearchButton.Name = "SearchButton"
        Me.SearchButton.Size = New System.Drawing.Size(32, 32)
        Me.SearchButton.TabIndex = 43
        Me.SearchButton.UseVisualStyleBackColor = True
        '
        'SaveButton
        '
        Me.SaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.SaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.SaveButton.Enabled = False
        Me.SaveButton.ForeColor = System.Drawing.Color.Black
        Me.SaveButton.Location = New System.Drawing.Point(12, 1)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(32, 32)
        Me.SaveButton.TabIndex = 44
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'PrintButton
        '
        Me.PrintButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PrintButton.Enabled = False
        Me.PrintButton.ForeColor = System.Drawing.Color.Black
        Me.PrintButton.Location = New System.Drawing.Point(121, 1)
        Me.PrintButton.Name = "PrintButton"
        Me.PrintButton.Size = New System.Drawing.Size(32, 32)
        Me.PrintButton.TabIndex = 52
        Me.PrintButton.UseVisualStyleBackColor = True
        '
        'DeleteButton
        '
        Me.DeleteButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.DeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.DeleteButton.Enabled = False
        Me.DeleteButton.ForeColor = System.Drawing.Color.Black
        Me.DeleteButton.Location = New System.Drawing.Point(85, 1)
        Me.DeleteButton.Name = "DeleteButton"
        Me.DeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.DeleteButton.TabIndex = 53
        Me.DeleteButton.UseVisualStyleBackColor = True
        '
        'ExitButton
        '
        Me.ExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExitButton.ForeColor = System.Drawing.Color.Black
        Me.ExitButton.Location = New System.Drawing.Point(157, 1)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(32, 32)
        Me.ExitButton.TabIndex = 54
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'CancelButton
        '
        Me.CancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.CancelButton.Enabled = False
        Me.CancelButton.ForeColor = System.Drawing.Color.Black
        Me.CancelButton.Location = New System.Drawing.Point(48, 1)
        Me.CancelButton.Name = "CancelButton"
        Me.CancelButton.Size = New System.Drawing.Size(32, 32)
        Me.CancelButton.TabIndex = 55
        Me.CancelButton.UseVisualStyleBackColor = True
        '
        'SRVAdjustmentsDSBindingSource
        '
        Me.SRVAdjustmentsDSBindingSource.DataSource = GetType(Biosystems.Ax00.Types.SRVAdjustmentsDS)
        Me.SRVAdjustmentsDSBindingSource.Position = 0
        '
        'SearchGroupBox
        '
        Me.SearchGroupBox.Controls.Add(Me.AnalyzerLabel)
        Me.SearchGroupBox.Controls.Add(Me.DateFromDateTimePick)
        Me.SearchGroupBox.Controls.Add(Me.AnalyzerCombo)
        Me.SearchGroupBox.Controls.Add(Me.ActionLabel)
        Me.SearchGroupBox.Controls.Add(Me.DateFromLabel)
        Me.SearchGroupBox.Controls.Add(Me.SearchButton)
        Me.SearchGroupBox.Controls.Add(Me.ActionsCombo)
        Me.SearchGroupBox.Controls.Add(Me.DateToDateTimePick)
        Me.SearchGroupBox.Controls.Add(Me.TasksCombo)
        Me.SearchGroupBox.Controls.Add(Me.DateToLabel)
        Me.SearchGroupBox.Controls.Add(Me.TaskLabel)
        Me.SearchGroupBox.Location = New System.Drawing.Point(10, 33)
        Me.SearchGroupBox.Name = "SearchGroupBox"
        Me.SearchGroupBox.Size = New System.Drawing.Size(963, 51)
        Me.SearchGroupBox.TabIndex = 68
        Me.SearchGroupBox.TabStop = False
        Me.SearchGroupBox.Text = "Search Configuration"
        '
        'ActivityGroupBox
        '
        Me.ActivityGroupBox.Controls.Add(Me.GridControl1)
        Me.ActivityGroupBox.Location = New System.Drawing.Point(8, 90)
        Me.ActivityGroupBox.Name = "ActivityGroupBox"
        Me.ActivityGroupBox.Size = New System.Drawing.Size(965, 462)
        Me.ActivityGroupBox.TabIndex = 69
        Me.ActivityGroupBox.TabStop = False
        Me.ActivityGroupBox.Text = "Historical Activity"
        '
        'GridControl1
        '
        Me.GridControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GridControl1.Location = New System.Drawing.Point(3, 17)
        Me.GridControl1.LookAndFeel.SkinName = "Seven"
        Me.GridControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003
        Me.GridControl1.MainView = Me.GridView1
        Me.GridControl1.Name = "GridControl1"
        Me.GridControl1.Size = New System.Drawing.Size(959, 442)
        Me.GridControl1.TabIndex = 1
        Me.GridControl1.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridView1})
        '
        'GridView1
        '
        Me.GridView1.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.GridView1.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.GridView1.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.GridView1.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.GridView1.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.GridView1.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.GridView1.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.GridView1.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.GridView1.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.GridView1.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.GridView1.Appearance.Empty.Options.UseBackColor = True
        Me.GridView1.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.GridView1.Appearance.EvenRow.Options.UseBackColor = True
        Me.GridView1.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.GridView1.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.GridView1.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.GridView1.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.GridView1.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.GridView1.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.GridView1.Appearance.FilterPanel.Options.UseBackColor = True
        Me.GridView1.Appearance.FilterPanel.Options.UseForeColor = True
        Me.GridView1.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent
        Me.GridView1.Appearance.FocusedCell.Options.UseBackColor = True
        Me.GridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.Black
        Me.GridView1.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.GridView1.Appearance.FocusedRow.Options.UseBackColor = True
        Me.GridView1.Appearance.FocusedRow.Options.UseForeColor = True
        Me.GridView1.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.FooterPanel.Options.UseBackColor = True
        Me.GridView1.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.GridView1.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.GridView1.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.GridView1.Appearance.GroupButton.Options.UseBackColor = True
        Me.GridView1.Appearance.GroupButton.Options.UseBorderColor = True
        Me.GridView1.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.GridView1.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.GridView1.Appearance.GroupFooter.Options.UseBackColor = True
        Me.GridView1.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.GridView1.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.GridView1.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.GridView1.Appearance.GroupPanel.Options.UseBackColor = True
        Me.GridView1.Appearance.GroupPanel.Options.UseForeColor = True
        Me.GridView1.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.GridView1.Appearance.GroupRow.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.GridView1.Appearance.GroupRow.Options.UseBackColor = True
        Me.GridView1.Appearance.GroupRow.Options.UseFont = True
        Me.GridView1.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.GridView1.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.GridView1.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.GridView1.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.GridView1.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.GridView1.Appearance.HorzLine.Options.UseBackColor = True
        Me.GridView1.Appearance.OddRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.GridView1.Appearance.OddRow.Options.UseBackColor = True
        Me.GridView1.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.GridView1.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.GridView1.Appearance.Preview.Options.UseBackColor = True
        Me.GridView1.Appearance.Preview.Options.UseForeColor = True
        Me.GridView1.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.GridView1.Appearance.Row.Options.UseBackColor = True
        Me.GridView1.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.GridView1.Appearance.RowSeparator.Options.UseBackColor = True
        Me.GridView1.Appearance.SelectedRow.BackColor = System.Drawing.Color.DimGray
        Me.GridView1.Appearance.SelectedRow.Options.UseBackColor = True
        Me.GridView1.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.GridView1.Appearance.VertLine.Options.UseBackColor = True
        Me.GridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.GridView1.GridControl = Me.GridControl1
        Me.GridView1.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.GridView1.Name = "GridView1"
        Me.GridView1.OptionsCustomization.AllowFilter = False
        Me.GridView1.OptionsCustomization.AllowSort = False
        Me.GridView1.OptionsFind.AllowFindPanel = False
        Me.GridView1.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.GridView1.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.GridView1.OptionsView.EnableAppearanceEvenRow = True
        Me.GridView1.OptionsView.EnableAppearanceOddRow = True
        Me.GridView1.OptionsView.ShowGroupPanel = False
        Me.GridView1.PaintStyleName = "WindowsXP"
        '
        'BsMessagesPanel
        '
        Me.BsMessagesPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessagesPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageImage)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageLabel)
        Me.BsMessagesPanel.Location = New System.Drawing.Point(0, 555)
        Me.BsMessagesPanel.Name = "BsMessagesPanel"
        Me.BsMessagesPanel.Size = New System.Drawing.Size(785, 35)
        Me.BsMessagesPanel.TabIndex = 70
        '
        'BsMessageImage
        '
        Me.BsMessageImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsMessageImage.Location = New System.Drawing.Point(3, 1)
        Me.BsMessageImage.Name = "BsMessageImage"
        Me.BsMessageImage.PositionNumber = 0
        Me.BsMessageImage.Size = New System.Drawing.Size(32, 32)
        Me.BsMessageImage.TabIndex = 3
        Me.BsMessageImage.TabStop = False
        '
        'BsMessageLabel
        '
        Me.BsMessageLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsMessageLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsMessageLabel.ForeColor = System.Drawing.Color.Black
        Me.BsMessageLabel.Location = New System.Drawing.Point(41, 11)
        Me.BsMessageLabel.Name = "BsMessageLabel"
        Me.BsMessageLabel.Size = New System.Drawing.Size(729, 18)
        Me.BsMessageLabel.TabIndex = 1
        Me.BsMessageLabel.Title = False
        '
        'BsButtonsPanel
        '
        Me.BsButtonsPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButtonsPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsButtonsPanel.Controls.Add(Me.ExitButton)
        Me.BsButtonsPanel.Controls.Add(Me.PrintButton)
        Me.BsButtonsPanel.Controls.Add(Me.SaveButton)
        Me.BsButtonsPanel.Controls.Add(Me.DeleteButton)
        Me.BsButtonsPanel.Controls.Add(Me.CancelButton)
        Me.BsButtonsPanel.Location = New System.Drawing.Point(784, 555)
        Me.BsButtonsPanel.Name = "BsButtonsPanel"
        Me.BsButtonsPanel.Size = New System.Drawing.Size(194, 35)
        Me.BsButtonsPanel.TabIndex = 71
        '
        'BsErrorProvider1
        '
        Me.BsErrorProvider1.ContainerControl = Me
        '
        'IHistoricalReports
        '
        Me.AcceptButton = Me.ExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(978, 593)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.Controls.Add(Me.ActivityGroupBox)
        Me.Controls.Add(Me.SearchGroupBox)
        Me.Controls.Add(Me.BsTitleLabel)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "IHistoricalReports"
        Me.Text = "HistoricalReports"
        CType(Me.RepositoryItemRichTextEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SRVAdjustmentsDSBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SearchGroupBox.ResumeLayout(False)
        Me.ActivityGroupBox.ResumeLayout(False)
        CType(Me.GridControl1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsButtonsPanel.ResumeLayout(False)
        CType(Me.BsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TasksCombo As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents TaskLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents ActionsCombo As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents ActionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents DateToDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents DateToLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents DateFromDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents DateFromLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents AnalyzerCombo As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents AnalyzerLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents SearchButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents SaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents PrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents DeleteButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents CancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents SRVAdjustmentsDSBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents SearchGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents ActivityGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents RepositoryItemRichTextEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents GridControl1 As DevExpress.XtraGrid.GridControl
    Friend WithEvents GridView1 As DevExpress.XtraGrid.Views.Grid.GridView
End Class
