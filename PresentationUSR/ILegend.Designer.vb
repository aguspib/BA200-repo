<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiLegend
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim GridView1 As DevExpress.XtraGrid.Views.Grid.GridView
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiLegend))
        Me.Cycle = New DevExpress.XtraGrid.Columns.GridColumn
        Me.Abs1 = New DevExpress.XtraGrid.Columns.GridColumn
        Me.Abs2 = New DevExpress.XtraGrid.Columns.GridColumn
        Me.Diff = New DevExpress.XtraGrid.Columns.GridColumn
        Me.imageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.bsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.bsProgTestToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.OrderToExportCheckBox = New DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit
        Me.OrderToPrintCheckBox = New DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit
        Me.STATImage = New DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
        Me.GridView2 = New DevExpress.XtraGrid.Views.Grid.GridView
        Me.bsSamplesListDataGridView = New DevExpress.XtraGrid.GridControl
        Me.LegendGridView = New DevExpress.XtraGrid.Views.Grid.GridView
        Me.IconColumn = New DevExpress.XtraGrid.Columns.GridColumn
        Me.LegendPictureEdit = New DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
        Me.Description = New DevExpress.XtraGrid.Columns.GridColumn
        Me.DescriptionRichText = New DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit
        Me.Position = New DevExpress.XtraGrid.Columns.GridColumn
        Me.Group = New DevExpress.XtraGrid.Columns.GridColumn
        Me.ExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.RepositoryItemMemoEdit1 = New DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit
        GridView1 = New DevExpress.XtraGrid.Views.Grid.GridView
        CType(GridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.OrderToExportCheckBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.OrderToPrintCheckBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.STATImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsSamplesListDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LegendGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LegendPictureEdit, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DescriptionRichText, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RepositoryItemMemoEdit1, System.ComponentModel.ISupportInitialize).BeginInit()
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
        GridView1.OptionsFind.AllowFindPanel = False
        GridView1.OptionsSelection.EnableAppearanceFocusedCell = False
        GridView1.OptionsView.EnableAppearanceEvenRow = True
        GridView1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow
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
        'imageList1
        '
        Me.imageList1.ImageStream = CType(resources.GetObject("imageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imageList1.TransparentColor = System.Drawing.Color.Magenta
        Me.imageList1.Images.SetKeyName(0, "Exporthead.png")
        Me.imageList1.Images.SetKeyName(1, "PrintHead.png")
        Me.imageList1.Images.SetKeyName(2, "PatientRoutine.png")
        '
        'bsErrorProvider1
        '
        Me.bsErrorProvider1.ContainerControl = Me
        '
        'Timer1
        '
        Me.Timer1.Interval = 500
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
        'GridView2
        '
        Me.GridView2.Name = "GridView2"
        Me.GridView2.OptionsFind.AllowFindPanel = False
        '
        'bsSamplesListDataGridView
        '
        Me.bsSamplesListDataGridView.AllowRestoreSelectionAndFocusedRow = DevExpress.Utils.DefaultBoolean.[False]
        Me.bsSamplesListDataGridView.Location = New System.Drawing.Point(12, 50)
        Me.bsSamplesListDataGridView.MainView = Me.LegendGridView
        Me.bsSamplesListDataGridView.Margin = New System.Windows.Forms.Padding(3, 3, 5, 3)
        Me.bsSamplesListDataGridView.Name = "bsSamplesListDataGridView"
        Me.bsSamplesListDataGridView.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {Me.LegendPictureEdit, Me.DescriptionRichText, Me.RepositoryItemMemoEdit1})
        Me.bsSamplesListDataGridView.Size = New System.Drawing.Size(524, 470)
        Me.bsSamplesListDataGridView.TabIndex = 166
        Me.bsSamplesListDataGridView.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.LegendGridView})
        '
        'LegendGridView
        '
        Me.LegendGridView.Appearance.FocusedRow.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegendGridView.Appearance.FocusedRow.Options.UseFont = True
        Me.LegendGridView.Appearance.GroupButton.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegendGridView.Appearance.GroupButton.Options.UseFont = True
        Me.LegendGridView.Appearance.GroupPanel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LegendGridView.Appearance.GroupPanel.ForeColor = System.Drawing.Color.Gray
        Me.LegendGridView.Appearance.GroupPanel.Options.UseFont = True
        Me.LegendGridView.Appearance.GroupPanel.Options.UseForeColor = True
        Me.LegendGridView.Appearance.GroupRow.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LegendGridView.Appearance.GroupRow.ForeColor = System.Drawing.Color.Silver
        Me.LegendGridView.Appearance.GroupRow.Options.UseFont = True
        Me.LegendGridView.Appearance.GroupRow.Options.UseForeColor = True
        Me.LegendGridView.Appearance.HeaderPanel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegendGridView.Appearance.HeaderPanel.Options.UseFont = True
        Me.LegendGridView.Appearance.Preview.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LegendGridView.Appearance.Preview.Options.UseFont = True
        Me.LegendGridView.Appearance.Row.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LegendGridView.Appearance.Row.Options.UseFont = True
        Me.LegendGridView.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {Me.IconColumn, Me.Description, Me.Position, Me.Group})
        Me.LegendGridView.GridControl = Me.bsSamplesListDataGridView
        Me.LegendGridView.GroupCount = 1
        Me.LegendGridView.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden
        Me.LegendGridView.GroupFormat = "[#image]{1} {2}"
        Me.LegendGridView.GroupPanelText = "Legend"
        Me.LegendGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never
        Me.LegendGridView.Name = "LegendGridView"
        Me.LegendGridView.OptionsBehavior.AutoExpandAllGroups = True
        Me.LegendGridView.OptionsCustomization.AllowFilter = False
        Me.LegendGridView.OptionsCustomization.AllowGroup = False
        Me.LegendGridView.OptionsCustomization.AllowSort = False
        Me.LegendGridView.OptionsFilter.AllowColumnMRUFilterList = False
        Me.LegendGridView.OptionsFilter.AllowFilterEditor = False
        Me.LegendGridView.OptionsFilter.AllowMRUFilterList = False
        Me.LegendGridView.OptionsFind.AllowFindPanel = False
        Me.LegendGridView.OptionsLayout.Columns.AddNewColumns = False
        Me.LegendGridView.OptionsLayout.Columns.RemoveOldColumns = False
        Me.LegendGridView.OptionsLayout.Columns.StoreLayout = False
        Me.LegendGridView.OptionsMenu.EnableColumnMenu = False
        Me.LegendGridView.OptionsMenu.EnableFooterMenu = False
        Me.LegendGridView.OptionsMenu.EnableGroupPanelMenu = False
        Me.LegendGridView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.LegendGridView.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.LegendGridView.OptionsSelection.EnableAppearanceHideSelection = False
        Me.LegendGridView.OptionsSelection.UseIndicatorForSelection = False
        Me.LegendGridView.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office2003
        Me.LegendGridView.OptionsView.RowAutoHeight = True
        Me.LegendGridView.OptionsView.ShowChildrenInGroupPanel = True
        Me.LegendGridView.OptionsView.ShowColumnHeaders = False
        Me.LegendGridView.OptionsView.ShowDetailButtons = False
        Me.LegendGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never
        Me.LegendGridView.OptionsView.ShowFooter = True
        Me.LegendGridView.OptionsView.ShowGroupExpandCollapseButtons = False
        Me.LegendGridView.OptionsView.ShowGroupPanel = False
        Me.LegendGridView.OptionsView.ShowIndicator = False
        Me.LegendGridView.OptionsView.ShowPreviewRowLines = DevExpress.Utils.DefaultBoolean.False
        Me.LegendGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False
        Me.LegendGridView.PaintStyleName = "Web"
        Me.LegendGridView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveVertScroll
        Me.LegendGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.[Default]
        Me.LegendGridView.SortInfo.AddRange(New DevExpress.XtraGrid.Columns.GridColumnSortInfo() {New DevExpress.XtraGrid.Columns.GridColumnSortInfo(Me.Group, DevExpress.Data.ColumnSortOrder.Ascending), New DevExpress.XtraGrid.Columns.GridColumnSortInfo(Me.Position, DevExpress.Data.ColumnSortOrder.Ascending)})
        Me.LegendGridView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        '
        'IconColumn
        '
        Me.IconColumn.AppearanceCell.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IconColumn.AppearanceCell.Options.UseFont = True
        Me.IconColumn.ColumnEdit = Me.LegendPictureEdit
        Me.IconColumn.FieldName = "Picture"
        Me.IconColumn.ImageAlignment = System.Drawing.StringAlignment.Center
        Me.IconColumn.MinWidth = 32
        Me.IconColumn.Name = "IconColumn"
        Me.IconColumn.OptionsColumn.AllowEdit = False
        Me.IconColumn.OptionsColumn.AllowFocus = False
        Me.IconColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[True]
        Me.IconColumn.OptionsColumn.AllowIncrementalSearch = False
        Me.IconColumn.OptionsColumn.AllowMove = False
        Me.IconColumn.OptionsColumn.AllowShowHide = False
        Me.IconColumn.OptionsColumn.AllowSize = False
        Me.IconColumn.OptionsColumn.FixedWidth = True
        Me.IconColumn.OptionsColumn.ShowCaption = False
        Me.IconColumn.OptionsColumn.TabStop = False
        Me.IconColumn.OptionsFilter.AllowAutoFilter = False
        Me.IconColumn.OptionsFilter.AllowFilter = False
        Me.IconColumn.Visible = True
        Me.IconColumn.VisibleIndex = 0
        Me.IconColumn.Width = 50
        '
        'LegendPictureEdit
        '
        Me.LegendPictureEdit.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LegendPictureEdit.Appearance.Options.UseFont = True
        Me.LegendPictureEdit.LookAndFeel.UseDefaultLookAndFeel = False
        Me.LegendPictureEdit.Name = "LegendPictureEdit"
        '
        'Description
        '
        Me.Description.AppearanceCell.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Description.AppearanceCell.Options.UseFont = True
        Me.Description.AppearanceHeader.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.Description.AppearanceHeader.Options.UseFont = True
        Me.Description.Caption = "Description"
        Me.Description.ColumnEdit = Me.RepositoryItemMemoEdit1
        Me.Description.FieldName = "Description"
        Me.Description.MaxWidth = 460
        Me.Description.MinWidth = 460
        Me.Description.Name = "Description"
        Me.Description.OptionsColumn.AllowEdit = False
        Me.Description.OptionsColumn.AllowFocus = False
        Me.Description.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
        Me.Description.OptionsColumn.AllowIncrementalSearch = False
        Me.Description.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.[False]
        Me.Description.OptionsColumn.AllowMove = False
        Me.Description.OptionsColumn.AllowShowHide = False
        Me.Description.OptionsColumn.AllowSize = False
        Me.Description.OptionsColumn.FixedWidth = True
        Me.Description.OptionsColumn.ReadOnly = True
        Me.Description.OptionsColumn.ShowCaption = False
        Me.Description.OptionsColumn.TabStop = False
        Me.Description.OptionsFilter.AllowAutoFilter = False
        Me.Description.OptionsFilter.AllowFilter = False
        Me.Description.OptionsFilter.ImmediateUpdateAutoFilter = False
        Me.Description.UnboundType = DevExpress.Data.UnboundColumnType.[String]
        Me.Description.Visible = True
        Me.Description.VisibleIndex = 1
        Me.Description.Width = 460
        '
        'DescriptionRichText
        '
        Me.DescriptionRichText.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DescriptionRichText.Appearance.Options.UseFont = True
        Me.DescriptionRichText.LookAndFeel.UseDefaultLookAndFeel = False
        Me.DescriptionRichText.Name = "DescriptionRichText"
        '
        'Position
        '
        Me.Position.Caption = "Position"
        Me.Position.FieldName = "Position"
        Me.Position.MaxWidth = 80
        Me.Position.Name = "Position"
        Me.Position.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value
        Me.Position.Width = 80
        '
        'Group
        '
        Me.Group.AppearanceCell.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Group.AppearanceCell.ForeColor = System.Drawing.Color.Silver
        Me.Group.AppearanceCell.Options.UseFont = True
        Me.Group.AppearanceCell.Options.UseForeColor = True
        Me.Group.AppearanceHeader.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Group.AppearanceHeader.ForeColor = System.Drawing.Color.Silver
        Me.Group.AppearanceHeader.Options.UseFont = True
        Me.Group.AppearanceHeader.Options.UseForeColor = True
        Me.Group.FieldName = "Group"
        Me.Group.GroupFormat.FormatString = "ICON"
        Me.Group.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.Value
        Me.Group.Name = "Group"
        Me.Group.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value
        Me.Group.Width = 111
        '
        'ExitButton
        '
        Me.ExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ExitButton.Location = New System.Drawing.Point(504, 525)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(32, 32)
        Me.ExitButton.TabIndex = 167
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'bsTitleLabel
        '
        Me.bsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTitleLabel.Location = New System.Drawing.Point(15, 15)
        Me.bsTitleLabel.Name = "bsTitleLabel"
        Me.bsTitleLabel.Size = New System.Drawing.Size(521, 20)
        Me.bsTitleLabel.TabIndex = 168
        Me.bsTitleLabel.Text = "*Summary of status and additional functions"
        Me.bsTitleLabel.Title = True
        '
        'RepositoryItemMemoEdit1
        '
        Me.RepositoryItemMemoEdit1.Name = "RepositoryItemMemoEdit1"
        '
        'ILegend
        '
        Me.AllowDrop = False
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.ClientSize = New System.Drawing.Size(544, 563)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsTitleLabel)
        Me.Controls.Add(Me.ExitButton)
        Me.Controls.Add(Me.bsSamplesListDataGridView)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ILegend"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        CType(GridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.OrderToExportCheckBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.OrderToPrintCheckBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.STATImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsSamplesListDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LegendGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LegendPictureEdit, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DescriptionRichText, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RepositoryItemMemoEdit1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsProgTestToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents Cycle As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Abs1 As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Abs2 As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Diff As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents imageList1 As System.Windows.Forms.ImageList
    Friend WithEvents OrderToExportCheckBox As DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit
    Friend WithEvents OrderToPrintCheckBox As DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit
    Friend WithEvents STATImage As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
    Friend WithEvents GridView2 As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents bsSamplesListDataGridView As DevExpress.XtraGrid.GridControl
    Friend WithEvents LegendPictureEdit As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
    Friend WithEvents LegendGridView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents IconColumn As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Description As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Group As DevExpress.XtraGrid.Columns.GridColumn
    Friend DescriptionRichText As DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit
    Friend WithEvents Position As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents ExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents RepositoryItemMemoEdit1 As DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit

End Class
