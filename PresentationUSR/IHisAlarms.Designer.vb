<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IHisAlarms
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
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSearchGroup = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsSubtitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsAnalyzerIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsAnalyzerIDComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsTypeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsDateToDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.bsDateToLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDateFromDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.bsDateFromLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSearchButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsHistoryGroup = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.xtraHistoryGrid = New DevExpress.XtraGrid.GridControl()
        Me.xtraHistoryGridView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.bsHistoryDelete = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSearchGroup.SuspendLayout()
        Me.bsHistoryGroup.SuspendLayout()
        CType(Me.xtraHistoryGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.xtraHistoryGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Location = New System.Drawing.Point(923, 614)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 0
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsSearchGroup
        '
        Me.bsSearchGroup.Controls.Add(Me.bsSubtitleLabel)
        Me.bsSearchGroup.Controls.Add(Me.bsAnalyzerIDLabel)
        Me.bsSearchGroup.Controls.Add(Me.bsTypeLabel)
        Me.bsSearchGroup.Controls.Add(Me.bsAnalyzerIDComboBox)
        Me.bsSearchGroup.Controls.Add(Me.bsTypeComboBox)
        Me.bsSearchGroup.Controls.Add(Me.bsDateToDateTimePick)
        Me.bsSearchGroup.Controls.Add(Me.bsDateToLabel)
        Me.bsSearchGroup.Controls.Add(Me.bsDateFromDateTimePick)
        Me.bsSearchGroup.Controls.Add(Me.bsDateFromLabel)
        Me.bsSearchGroup.Controls.Add(Me.bsSearchButton)
        Me.bsSearchGroup.ForeColor = System.Drawing.Color.Black
        Me.bsSearchGroup.Location = New System.Drawing.Point(11, 10)
        Me.bsSearchGroup.Name = "bsSearchGroup"
        Me.bsSearchGroup.Size = New System.Drawing.Size(957, 95)
        Me.bsSearchGroup.TabIndex = 26
        Me.bsSearchGroup.TabStop = False
        '
        'bsSubtitleLabel
        '
        Me.bsSubtitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSubtitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsSubtitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSubtitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsSubtitleLabel.Name = "bsSubtitleLabel"
        Me.bsSubtitleLabel.Size = New System.Drawing.Size(934, 20)
        Me.bsSubtitleLabel.TabIndex = 26
        Me.bsSubtitleLabel.Text = "*Alarms History"
        Me.bsSubtitleLabel.Title = True
        '
        'bsAnalyzerIDLabel
        '
        Me.bsAnalyzerIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsAnalyzerIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsAnalyzerIDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsAnalyzerIDLabel.Location = New System.Drawing.Point(549, 47)
        Me.bsAnalyzerIDLabel.Name = "bsAnalyzerIDLabel"
        Me.bsAnalyzerIDLabel.Size = New System.Drawing.Size(183, 13)
        Me.bsAnalyzerIDLabel.TabIndex = 77
        Me.bsAnalyzerIDLabel.Text = "* Analyzer:"
        Me.bsAnalyzerIDLabel.Title = False
        '
        'bsTypeLabel
        '
        Me.bsTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTypeLabel.Location = New System.Drawing.Point(338, 47)
        Me.bsTypeLabel.Name = "bsTypeLabel"
        Me.bsTypeLabel.Size = New System.Drawing.Size(123, 13)
        Me.bsTypeLabel.TabIndex = 78
        Me.bsTypeLabel.Text = "* Type:"
        Me.bsTypeLabel.Title = False
        '
        'bsAnalyzerIDComboBox
        '
        Me.bsAnalyzerIDComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsAnalyzerIDComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAnalyzerIDComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsAnalyzerIDComboBox.FormattingEnabled = True
        Me.bsAnalyzerIDComboBox.Location = New System.Drawing.Point(549, 62)
        Me.bsAnalyzerIDComboBox.Name = "bsAnalyzerIDComboBox"
        Me.bsAnalyzerIDComboBox.Size = New System.Drawing.Size(183, 21)
        Me.bsAnalyzerIDComboBox.TabIndex = 75
        '
        'bsTypeComboBox
        '
        Me.bsTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsTypeComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsTypeComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsTypeComboBox.FormattingEnabled = True
        Me.bsTypeComboBox.Location = New System.Drawing.Point(338, 62)
        Me.bsTypeComboBox.Name = "bsTypeComboBox"
        Me.bsTypeComboBox.Size = New System.Drawing.Size(123, 21)
        Me.bsTypeComboBox.TabIndex = 76
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
        Me.bsDateToDateTimePick.Location = New System.Drawing.Point(159, 62)
        Me.bsDateToDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsDateToDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsDateToDateTimePick.Name = "bsDateToDateTimePick"
        Me.bsDateToDateTimePick.Size = New System.Drawing.Size(108, 21)
        Me.bsDateToDateTimePick.TabIndex = 64
        '
        'bsDateToLabel
        '
        Me.bsDateToLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDateToLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateToLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDateToLabel.Location = New System.Drawing.Point(159, 47)
        Me.bsDateToLabel.Name = "bsDateToLabel"
        Me.bsDateToLabel.Size = New System.Drawing.Size(108, 13)
        Me.bsDateToLabel.TabIndex = 66
        Me.bsDateToLabel.Text = "* To:"
        Me.bsDateToLabel.Title = False
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
        Me.bsDateFromDateTimePick.Location = New System.Drawing.Point(33, 62)
        Me.bsDateFromDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsDateFromDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsDateFromDateTimePick.Name = "bsDateFromDateTimePick"
        Me.bsDateFromDateTimePick.Size = New System.Drawing.Size(108, 21)
        Me.bsDateFromDateTimePick.TabIndex = 63
        '
        'bsDateFromLabel
        '
        Me.bsDateFromLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDateFromLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateFromLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDateFromLabel.Location = New System.Drawing.Point(33, 47)
        Me.bsDateFromLabel.Name = "bsDateFromLabel"
        Me.bsDateFromLabel.Size = New System.Drawing.Size(111, 13)
        Me.bsDateFromLabel.TabIndex = 65
        Me.bsDateFromLabel.Text = "*From:"
        Me.bsDateFromLabel.Title = False
        '
        'bsSearchButton
        '
        Me.bsSearchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSearchButton.Location = New System.Drawing.Point(912, 51)
        Me.bsSearchButton.Name = "bsSearchButton"
        Me.bsSearchButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSearchButton.TabIndex = 4
        Me.bsSearchButton.UseVisualStyleBackColor = True
        '
        'bsHistoryGroup
        '
        Me.bsHistoryGroup.Controls.Add(Me.xtraHistoryGrid)
        Me.bsHistoryGroup.Controls.Add(Me.bsHistoryDelete)
        Me.bsHistoryGroup.ForeColor = System.Drawing.Color.Black
        Me.bsHistoryGroup.Location = New System.Drawing.Point(11, 104)
        Me.bsHistoryGroup.Name = "bsHistoryGroup"
        Me.bsHistoryGroup.Size = New System.Drawing.Size(957, 505)
        Me.bsHistoryGroup.TabIndex = 27
        Me.bsHistoryGroup.TabStop = False
        '
        'xtraHistoryGrid
        '
        Me.xtraHistoryGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.xtraHistoryGrid.EmbeddedNavigator.Buttons.Append.Visible = False
        Me.xtraHistoryGrid.EmbeddedNavigator.Buttons.CancelEdit.Visible = False
        Me.xtraHistoryGrid.EmbeddedNavigator.Buttons.Edit.Visible = False
        Me.xtraHistoryGrid.EmbeddedNavigator.Buttons.EndEdit.Visible = False
        Me.xtraHistoryGrid.EmbeddedNavigator.Buttons.Next.Visible = False
        Me.xtraHistoryGrid.EmbeddedNavigator.Buttons.Prev.Visible = False
        Me.xtraHistoryGrid.EmbeddedNavigator.Buttons.Remove.Visible = False
        Me.xtraHistoryGrid.EmbeddedNavigator.TextStringFormat = "{0} / {1}"
        Me.xtraHistoryGrid.Location = New System.Drawing.Point(10, 20)
        Me.xtraHistoryGrid.LookAndFeel.UseWindowsXPTheme = True
        Me.xtraHistoryGrid.MainView = Me.xtraHistoryGridView
        Me.xtraHistoryGrid.Name = "xtraHistoryGrid"
        Me.xtraHistoryGrid.Size = New System.Drawing.Size(896, 477)
        Me.xtraHistoryGrid.TabIndex = 3
        Me.xtraHistoryGrid.UseEmbeddedNavigator = True
        Me.xtraHistoryGrid.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.xtraHistoryGridView})
        '
        'xtraHistoryGridView
        '
        Me.xtraHistoryGridView.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.xtraHistoryGridView.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.xtraHistoryGridView.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.xtraHistoryGridView.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.xtraHistoryGridView.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.xtraHistoryGridView.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.xtraHistoryGridView.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.xtraHistoryGridView.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.xtraHistoryGridView.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.xtraHistoryGridView.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.xtraHistoryGridView.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.xtraHistoryGridView.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.xtraHistoryGridView.Appearance.Empty.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.xtraHistoryGridView.Appearance.EvenRow.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.xtraHistoryGridView.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.xtraHistoryGridView.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.xtraHistoryGridView.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.xtraHistoryGridView.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.xtraHistoryGridView.Appearance.FilterPanel.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.FilterPanel.Options.UseForeColor = True
        Me.xtraHistoryGridView.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent
        Me.xtraHistoryGridView.Appearance.FocusedCell.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.FocusedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.xtraHistoryGridView.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.xtraHistoryGridView.Appearance.FocusedRow.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.FocusedRow.Options.UseForeColor = True
        Me.xtraHistoryGridView.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.xtraHistoryGridView.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.xtraHistoryGridView.Appearance.FooterPanel.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.xtraHistoryGridView.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.xtraHistoryGridView.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.xtraHistoryGridView.Appearance.GroupButton.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.GroupButton.Options.UseBorderColor = True
        Me.xtraHistoryGridView.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.xtraHistoryGridView.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.xtraHistoryGridView.Appearance.GroupFooter.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.xtraHistoryGridView.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.xtraHistoryGridView.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.xtraHistoryGridView.Appearance.GroupPanel.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.GroupPanel.Options.UseForeColor = True
        Me.xtraHistoryGridView.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.xtraHistoryGridView.Appearance.GroupRow.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.xtraHistoryGridView.Appearance.GroupRow.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.GroupRow.Options.UseFont = True
        Me.xtraHistoryGridView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.xtraHistoryGridView.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.xtraHistoryGridView.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.xtraHistoryGridView.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.xtraHistoryGridView.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.xtraHistoryGridView.Appearance.HorzLine.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.OddRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.xtraHistoryGridView.Appearance.OddRow.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.xtraHistoryGridView.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.xtraHistoryGridView.Appearance.Preview.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.Preview.Options.UseForeColor = True
        Me.xtraHistoryGridView.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.xtraHistoryGridView.Appearance.Row.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.xtraHistoryGridView.Appearance.RowSeparator.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.SelectedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.xtraHistoryGridView.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White
        Me.xtraHistoryGridView.Appearance.SelectedRow.Options.UseBackColor = True
        Me.xtraHistoryGridView.Appearance.SelectedRow.Options.UseForeColor = True
        Me.xtraHistoryGridView.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.xtraHistoryGridView.Appearance.VertLine.Options.UseBackColor = True
        Me.xtraHistoryGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.xtraHistoryGridView.GridControl = Me.xtraHistoryGrid
        Me.xtraHistoryGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.xtraHistoryGridView.Name = "xtraHistoryGridView"
        Me.xtraHistoryGridView.OptionsCustomization.AllowColumnMoving = False
        Me.xtraHistoryGridView.OptionsCustomization.AllowFilter = False
        Me.xtraHistoryGridView.OptionsCustomization.AllowQuickHideColumns = False
        Me.xtraHistoryGridView.OptionsCustomization.AllowSort = False
        Me.xtraHistoryGridView.OptionsFind.AllowFindPanel = False
        Me.xtraHistoryGridView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.xtraHistoryGridView.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.xtraHistoryGridView.OptionsView.ShowGroupPanel = False
        Me.xtraHistoryGridView.PaintStyleName = "WindowsXP"
        '
        'bsHistoryDelete
        '
        Me.bsHistoryDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsHistoryDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsHistoryDelete.Location = New System.Drawing.Point(912, 20)
        Me.bsHistoryDelete.Name = "bsHistoryDelete"
        Me.bsHistoryDelete.Size = New System.Drawing.Size(32, 32)
        Me.bsHistoryDelete.TabIndex = 2
        Me.bsHistoryDelete.UseVisualStyleBackColor = True
        '
        'IHisAlarms
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.Controls.Add(Me.bsSearchGroup)
        Me.Controls.Add(Me.bsHistoryGroup)
        Me.Controls.Add(Me.bsExitButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IHisAlarms"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "IHisAlarms"
        Me.bsSearchGroup.ResumeLayout(False)
        Me.bsHistoryGroup.ResumeLayout(False)
        CType(Me.xtraHistoryGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.xtraHistoryGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSearchGroup As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsHistoryGroup As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsSearchButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsHistoryDelete As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSubtitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents xtraHistoryGrid As DevExpress.XtraGrid.GridControl
    Friend WithEvents xtraHistoryGridView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents bsDateToDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents bsDateToLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDateFromDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents bsDateFromLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAnalyzerIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAnalyzerIDComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsTypeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
End Class
