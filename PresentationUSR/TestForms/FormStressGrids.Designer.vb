<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormStressGrids
    Inherits System.Windows.Forms.Form

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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.grpDataTable = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.numRows = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.lblNumRows = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.dxpGrid1 = New DevExpress.XtraGrid.GridControl
        Me.dxpGrid1View = New DevExpress.XtraGrid.Views.Grid.GridView
        Me.lblElapsedText1 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.lblTime1 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.lblAction = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.pbRegisters = New Biosystems.Ax00.Controls.UserControls.BSProgressBar
        Me.lblRegisters = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.butBindGrid1 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.pnlGrid1 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.lblDescGrid1 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.pnlGrid2 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ctrlNavGrid2 = New DevExpress.XtraEditors.ControlNavigator
        Me.dxpGrid2 = New DevExpress.XtraGrid.GridControl
        Me.dxpGrid2View = New DevExpress.XtraGrid.Views.Grid.GridView
        Me.butBindGrid2 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.lblDescGrid2 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.lblElapsedText2 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.lblTime2 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.pnlGrid3 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.dgrGrid3 = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.butBindGrid3 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.lblDescGrid3 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.lblElapsedText3 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.lblTime3 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.grpDataTable.SuspendLayout()
        CType(Me.numRows, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dxpGrid1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dxpGrid1View, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlGrid1.SuspendLayout()
        Me.pnlGrid2.SuspendLayout()
        CType(Me.dxpGrid2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dxpGrid2View, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlGrid3.SuspendLayout()
        CType(Me.dgrGrid3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'grpDataTable
        '
        Me.grpDataTable.Controls.Add(Me.numRows)
        Me.grpDataTable.Controls.Add(Me.lblNumRows)
        Me.grpDataTable.ForeColor = System.Drawing.Color.Black
        Me.grpDataTable.Location = New System.Drawing.Point(13, 13)
        Me.grpDataTable.Name = "grpDataTable"
        Me.grpDataTable.Size = New System.Drawing.Size(261, 71)
        Me.grpDataTable.TabIndex = 1
        Me.grpDataTable.TabStop = False
        Me.grpDataTable.Text = "Data Table properties"
        '
        'numRows
        '
        Me.numRows.Location = New System.Drawing.Point(131, 27)
        Me.numRows.Maximum = New Decimal(New Integer() {-727379968, 232, 0, 0})
        Me.numRows.Name = "numRows"
        Me.numRows.Size = New System.Drawing.Size(88, 20)
        Me.numRows.TabIndex = 1
        Me.numRows.Value = New Decimal(New Integer() {10000, 0, 0, 0})
        '
        'lblNumRows
        '
        Me.lblNumRows.AutoSize = True
        Me.lblNumRows.BackColor = System.Drawing.Color.Transparent
        Me.lblNumRows.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblNumRows.ForeColor = System.Drawing.Color.Black
        Me.lblNumRows.Location = New System.Drawing.Point(9, 29)
        Me.lblNumRows.Name = "lblNumRows"
        Me.lblNumRows.Size = New System.Drawing.Size(106, 13)
        Me.lblNumRows.TabIndex = 0
        Me.lblNumRows.Text = "Number of Rows:"
        Me.lblNumRows.Title = False
        '
        'dxpGrid1
        '
        Me.dxpGrid1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dxpGrid1.Location = New System.Drawing.Point(12, 34)
        Me.dxpGrid1.LookAndFeel.UseWindowsXPTheme = True
        Me.dxpGrid1.MainView = Me.dxpGrid1View
        Me.dxpGrid1.Name = "dxpGrid1"
        Me.dxpGrid1.Size = New System.Drawing.Size(578, 230)
        Me.dxpGrid1.TabIndex = 2
        Me.dxpGrid1.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.dxpGrid1View})
        '
        'dxpGrid1View
        '
        Me.dxpGrid1View.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.dxpGrid1View.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.dxpGrid1View.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.dxpGrid1View.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.dxpGrid1View.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.dxpGrid1View.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.dxpGrid1View.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.dxpGrid1View.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.dxpGrid1View.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.dxpGrid1View.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.dxpGrid1View.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.dxpGrid1View.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.dxpGrid1View.Appearance.Empty.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.dxpGrid1View.Appearance.EvenRow.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.dxpGrid1View.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.dxpGrid1View.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.dxpGrid1View.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.dxpGrid1View.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.dxpGrid1View.Appearance.FilterPanel.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.FilterPanel.Options.UseForeColor = True
        Me.dxpGrid1View.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent
        Me.dxpGrid1View.Appearance.FocusedCell.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.FocusedRow.BackColor = System.Drawing.Color.Black
        Me.dxpGrid1View.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.dxpGrid1View.Appearance.FocusedRow.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.FocusedRow.Options.UseForeColor = True
        Me.dxpGrid1View.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.dxpGrid1View.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.dxpGrid1View.Appearance.FooterPanel.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.dxpGrid1View.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.dxpGrid1View.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.dxpGrid1View.Appearance.GroupButton.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.GroupButton.Options.UseBorderColor = True
        Me.dxpGrid1View.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.dxpGrid1View.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.dxpGrid1View.Appearance.GroupFooter.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.dxpGrid1View.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.dxpGrid1View.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.dxpGrid1View.Appearance.GroupPanel.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.GroupPanel.Options.UseForeColor = True
        Me.dxpGrid1View.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.dxpGrid1View.Appearance.GroupRow.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dxpGrid1View.Appearance.GroupRow.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.GroupRow.Options.UseFont = True
        Me.dxpGrid1View.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.dxpGrid1View.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.dxpGrid1View.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.dxpGrid1View.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.dxpGrid1View.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.dxpGrid1View.Appearance.HorzLine.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.OddRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.dxpGrid1View.Appearance.OddRow.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.dxpGrid1View.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.dxpGrid1View.Appearance.Preview.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.Preview.Options.UseForeColor = True
        Me.dxpGrid1View.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.dxpGrid1View.Appearance.Row.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.dxpGrid1View.Appearance.RowSeparator.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.SelectedRow.BackColor = System.Drawing.Color.DimGray
        Me.dxpGrid1View.Appearance.SelectedRow.Options.UseBackColor = True
        Me.dxpGrid1View.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.dxpGrid1View.Appearance.VertLine.Options.UseBackColor = True
        Me.dxpGrid1View.BestFitMaxRowCount = 2
        Me.dxpGrid1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.dxpGrid1View.GridControl = Me.dxpGrid1
        Me.dxpGrid1View.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.dxpGrid1View.Name = "dxpGrid1View"
        Me.dxpGrid1View.OptionsCustomization.AllowFilter = False
        Me.dxpGrid1View.OptionsFind.AllowFindPanel = False
        Me.dxpGrid1View.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.dxpGrid1View.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.dxpGrid1View.OptionsView.EnableAppearanceEvenRow = True
        Me.dxpGrid1View.OptionsView.EnableAppearanceOddRow = True
        Me.dxpGrid1View.OptionsView.ShowGroupPanel = False
        Me.dxpGrid1View.PaintStyleName = "WindowsXP"
        '
        'lblElapsedText1
        '
        Me.lblElapsedText1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblElapsedText1.AutoSize = True
        Me.lblElapsedText1.BackColor = System.Drawing.Color.Transparent
        Me.lblElapsedText1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblElapsedText1.ForeColor = System.Drawing.Color.Black
        Me.lblElapsedText1.Location = New System.Drawing.Point(9, 267)
        Me.lblElapsedText1.Name = "lblElapsedText1"
        Me.lblElapsedText1.Size = New System.Drawing.Size(85, 13)
        Me.lblElapsedText1.TabIndex = 0
        Me.lblElapsedText1.Text = "Elapsed time:"
        Me.lblElapsedText1.Title = False
        '
        'lblTime1
        '
        Me.lblTime1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblTime1.AutoSize = True
        Me.lblTime1.BackColor = System.Drawing.Color.Transparent
        Me.lblTime1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblTime1.ForeColor = System.Drawing.Color.Black
        Me.lblTime1.Location = New System.Drawing.Point(95, 267)
        Me.lblTime1.Name = "lblTime1"
        Me.lblTime1.Size = New System.Drawing.Size(31, 13)
        Me.lblTime1.TabIndex = 0
        Me.lblTime1.Text = "0ms"
        Me.lblTime1.Title = False
        '
        'lblAction
        '
        Me.lblAction.BackColor = System.Drawing.Color.Transparent
        Me.lblAction.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblAction.ForeColor = System.Drawing.Color.Black
        Me.lblAction.Location = New System.Drawing.Point(10, 106)
        Me.lblAction.Name = "lblAction"
        Me.lblAction.Size = New System.Drawing.Size(264, 13)
        Me.lblAction.TabIndex = 0
        Me.lblAction.Text = "Ready"
        Me.lblAction.Title = False
        '
        'pbRegisters
        '
        Me.pbRegisters.Location = New System.Drawing.Point(13, 90)
        Me.pbRegisters.Name = "pbRegisters"
        Me.pbRegisters.Size = New System.Drawing.Size(147, 13)
        Me.pbRegisters.TabIndex = 3
        '
        'lblRegisters
        '
        Me.lblRegisters.BackColor = System.Drawing.Color.Transparent
        Me.lblRegisters.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblRegisters.ForeColor = System.Drawing.Color.Black
        Me.lblRegisters.Location = New System.Drawing.Point(166, 90)
        Me.lblRegisters.Name = "lblRegisters"
        Me.lblRegisters.Size = New System.Drawing.Size(108, 13)
        Me.lblRegisters.TabIndex = 0
        Me.lblRegisters.Text = "0/0"
        Me.lblRegisters.Title = False
        '
        'butBindGrid1
        '
        Me.butBindGrid1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butBindGrid1.Location = New System.Drawing.Point(12, 3)
        Me.butBindGrid1.Name = "butBindGrid1"
        Me.butBindGrid1.Size = New System.Drawing.Size(119, 25)
        Me.butBindGrid1.TabIndex = 0
        Me.butBindGrid1.Text = "Bind Data to Grid"
        Me.butBindGrid1.UseVisualStyleBackColor = True
        '
        'pnlGrid1
        '
        Me.pnlGrid1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlGrid1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlGrid1.Controls.Add(Me.butBindGrid1)
        Me.pnlGrid1.Controls.Add(Me.dxpGrid1)
        Me.pnlGrid1.Controls.Add(Me.lblDescGrid1)
        Me.pnlGrid1.Controls.Add(Me.lblElapsedText1)
        Me.pnlGrid1.Controls.Add(Me.lblTime1)
        Me.pnlGrid1.Location = New System.Drawing.Point(280, 3)
        Me.pnlGrid1.Name = "pnlGrid1"
        Me.pnlGrid1.Size = New System.Drawing.Size(604, 291)
        Me.pnlGrid1.TabIndex = 4
        '
        'lblDescGrid1
        '
        Me.lblDescGrid1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblDescGrid1.AutoSize = True
        Me.lblDescGrid1.BackColor = System.Drawing.Color.Transparent
        Me.lblDescGrid1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblDescGrid1.ForeColor = System.Drawing.Color.Black
        Me.lblDescGrid1.Location = New System.Drawing.Point(150, 7)
        Me.lblDescGrid1.Name = "lblDescGrid1"
        Me.lblDescGrid1.Size = New System.Drawing.Size(277, 13)
        Me.lblDescGrid1.TabIndex = 0
        Me.lblDescGrid1.Text = "DevExpress Grid with BestFitMaxRowCount = 2"
        Me.lblDescGrid1.Title = False
        '
        'pnlGrid2
        '
        Me.pnlGrid2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlGrid2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlGrid2.Controls.Add(Me.ctrlNavGrid2)
        Me.pnlGrid2.Controls.Add(Me.butBindGrid2)
        Me.pnlGrid2.Controls.Add(Me.dxpGrid2)
        Me.pnlGrid2.Controls.Add(Me.lblDescGrid2)
        Me.pnlGrid2.Controls.Add(Me.lblElapsedText2)
        Me.pnlGrid2.Controls.Add(Me.lblTime2)
        Me.pnlGrid2.Location = New System.Drawing.Point(280, 296)
        Me.pnlGrid2.Name = "pnlGrid2"
        Me.pnlGrid2.Size = New System.Drawing.Size(604, 291)
        Me.pnlGrid2.TabIndex = 4
        '
        'ctrlNavGrid2
        '
        Me.ctrlNavGrid2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ctrlNavGrid2.Buttons.Append.Visible = False
        Me.ctrlNavGrid2.Buttons.CancelEdit.Visible = False
        Me.ctrlNavGrid2.Buttons.Edit.Visible = False
        Me.ctrlNavGrid2.Buttons.EndEdit.Visible = False
        Me.ctrlNavGrid2.Buttons.Next.Visible = False
        Me.ctrlNavGrid2.Buttons.Prev.Visible = False
        Me.ctrlNavGrid2.Buttons.Remove.Visible = False
        Me.ctrlNavGrid2.Location = New System.Drawing.Point(444, 264)
        Me.ctrlNavGrid2.Name = "ctrlNavGrid2"
        Me.ctrlNavGrid2.NavigatableControl = Me.dxpGrid2
        Me.ctrlNavGrid2.Size = New System.Drawing.Size(146, 22)
        Me.ctrlNavGrid2.TabIndex = 5
        '
        'dxpGrid2
        '
        Me.dxpGrid2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dxpGrid2.Location = New System.Drawing.Point(12, 34)
        Me.dxpGrid2.LookAndFeel.UseWindowsXPTheme = True
        Me.dxpGrid2.MainView = Me.dxpGrid2View
        Me.dxpGrid2.Name = "dxpGrid2"
        Me.dxpGrid2.Size = New System.Drawing.Size(578, 230)
        Me.dxpGrid2.TabIndex = 2
        Me.dxpGrid2.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.dxpGrid2View})
        '
        'dxpGrid2View
        '
        Me.dxpGrid2View.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.dxpGrid2View.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.dxpGrid2View.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.dxpGrid2View.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.dxpGrid2View.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.dxpGrid2View.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.dxpGrid2View.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.dxpGrid2View.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.dxpGrid2View.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.dxpGrid2View.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.dxpGrid2View.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.dxpGrid2View.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.dxpGrid2View.Appearance.Empty.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.dxpGrid2View.Appearance.EvenRow.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.dxpGrid2View.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.dxpGrid2View.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.dxpGrid2View.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.dxpGrid2View.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.dxpGrid2View.Appearance.FilterPanel.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.FilterPanel.Options.UseForeColor = True
        Me.dxpGrid2View.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent
        Me.dxpGrid2View.Appearance.FocusedCell.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.FocusedRow.BackColor = System.Drawing.Color.Black
        Me.dxpGrid2View.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.dxpGrid2View.Appearance.FocusedRow.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.FocusedRow.Options.UseForeColor = True
        Me.dxpGrid2View.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.dxpGrid2View.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.dxpGrid2View.Appearance.FooterPanel.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.dxpGrid2View.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.dxpGrid2View.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.dxpGrid2View.Appearance.GroupButton.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.GroupButton.Options.UseBorderColor = True
        Me.dxpGrid2View.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.dxpGrid2View.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.dxpGrid2View.Appearance.GroupFooter.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.dxpGrid2View.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.dxpGrid2View.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.dxpGrid2View.Appearance.GroupPanel.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.GroupPanel.Options.UseForeColor = True
        Me.dxpGrid2View.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.dxpGrid2View.Appearance.GroupRow.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dxpGrid2View.Appearance.GroupRow.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.GroupRow.Options.UseFont = True
        Me.dxpGrid2View.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.dxpGrid2View.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.dxpGrid2View.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.dxpGrid2View.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.dxpGrid2View.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.dxpGrid2View.Appearance.HorzLine.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.OddRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.dxpGrid2View.Appearance.OddRow.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.dxpGrid2View.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.dxpGrid2View.Appearance.Preview.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.Preview.Options.UseForeColor = True
        Me.dxpGrid2View.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.dxpGrid2View.Appearance.Row.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.dxpGrid2View.Appearance.RowSeparator.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.SelectedRow.BackColor = System.Drawing.Color.DimGray
        Me.dxpGrid2View.Appearance.SelectedRow.Options.UseBackColor = True
        Me.dxpGrid2View.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.dxpGrid2View.Appearance.VertLine.Options.UseBackColor = True
        Me.dxpGrid2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.dxpGrid2View.GridControl = Me.dxpGrid2
        Me.dxpGrid2View.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.dxpGrid2View.Name = "dxpGrid2View"
        Me.dxpGrid2View.OptionsCustomization.AllowFilter = False
        Me.dxpGrid2View.OptionsFind.AllowFindPanel = False
        Me.dxpGrid2View.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.dxpGrid2View.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.dxpGrid2View.OptionsView.AllowCellMerge = True
        Me.dxpGrid2View.OptionsView.EnableAppearanceEvenRow = True
        Me.dxpGrid2View.OptionsView.EnableAppearanceOddRow = True
        Me.dxpGrid2View.PaintStyleName = "WindowsXP"
        '
        'butBindGrid2
        '
        Me.butBindGrid2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butBindGrid2.Location = New System.Drawing.Point(12, 3)
        Me.butBindGrid2.Name = "butBindGrid2"
        Me.butBindGrid2.Size = New System.Drawing.Size(119, 25)
        Me.butBindGrid2.TabIndex = 0
        Me.butBindGrid2.Text = "Bind Data to Grid"
        Me.butBindGrid2.UseVisualStyleBackColor = True
        '
        'lblDescGrid2
        '
        Me.lblDescGrid2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblDescGrid2.AutoSize = True
        Me.lblDescGrid2.BackColor = System.Drawing.Color.Transparent
        Me.lblDescGrid2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblDescGrid2.ForeColor = System.Drawing.Color.Black
        Me.lblDescGrid2.Location = New System.Drawing.Point(150, 7)
        Me.lblDescGrid2.Name = "lblDescGrid2"
        Me.lblDescGrid2.Size = New System.Drawing.Size(281, 13)
        Me.lblDescGrid2.TabIndex = 0
        Me.lblDescGrid2.Text = "Default DevExpress Grid with ControlNavigation"
        Me.lblDescGrid2.Title = False
        '
        'lblElapsedText2
        '
        Me.lblElapsedText2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblElapsedText2.AutoSize = True
        Me.lblElapsedText2.BackColor = System.Drawing.Color.Transparent
        Me.lblElapsedText2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblElapsedText2.ForeColor = System.Drawing.Color.Black
        Me.lblElapsedText2.Location = New System.Drawing.Point(9, 267)
        Me.lblElapsedText2.Name = "lblElapsedText2"
        Me.lblElapsedText2.Size = New System.Drawing.Size(85, 13)
        Me.lblElapsedText2.TabIndex = 0
        Me.lblElapsedText2.Text = "Elapsed time:"
        Me.lblElapsedText2.Title = False
        '
        'lblTime2
        '
        Me.lblTime2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblTime2.AutoSize = True
        Me.lblTime2.BackColor = System.Drawing.Color.Transparent
        Me.lblTime2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblTime2.ForeColor = System.Drawing.Color.Black
        Me.lblTime2.Location = New System.Drawing.Point(95, 267)
        Me.lblTime2.Name = "lblTime2"
        Me.lblTime2.Size = New System.Drawing.Size(31, 13)
        Me.lblTime2.TabIndex = 0
        Me.lblTime2.Text = "0ms"
        Me.lblTime2.Title = False
        '
        'pnlGrid3
        '
        Me.pnlGrid3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlGrid3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlGrid3.Controls.Add(Me.dgrGrid3)
        Me.pnlGrid3.Controls.Add(Me.butBindGrid3)
        Me.pnlGrid3.Controls.Add(Me.lblDescGrid3)
        Me.pnlGrid3.Controls.Add(Me.lblElapsedText3)
        Me.pnlGrid3.Controls.Add(Me.lblTime3)
        Me.pnlGrid3.Location = New System.Drawing.Point(280, 589)
        Me.pnlGrid3.Name = "pnlGrid3"
        Me.pnlGrid3.Size = New System.Drawing.Size(604, 291)
        Me.pnlGrid3.TabIndex = 4
        '
        'dgrGrid3
        '
        Me.dgrGrid3.AllowUserToOrderColumns = True
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.dgrGrid3.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.dgrGrid3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgrGrid3.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgrGrid3.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.dgrGrid3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgrGrid3.DefaultCellStyle = DataGridViewCellStyle3
        Me.dgrGrid3.EnterToTab = False
        Me.dgrGrid3.GridColor = System.Drawing.Color.Silver
        Me.dgrGrid3.Location = New System.Drawing.Point(12, 34)
        Me.dgrGrid3.Name = "dgrGrid3"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgrGrid3.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.dgrGrid3.Size = New System.Drawing.Size(578, 230)
        Me.dgrGrid3.TabIndex = 1
        Me.dgrGrid3.TabToEnter = False
        '
        'butBindGrid3
        '
        Me.butBindGrid3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.butBindGrid3.Location = New System.Drawing.Point(12, 3)
        Me.butBindGrid3.Name = "butBindGrid3"
        Me.butBindGrid3.Size = New System.Drawing.Size(119, 25)
        Me.butBindGrid3.TabIndex = 0
        Me.butBindGrid3.Text = "Bind Data to Grid"
        Me.butBindGrid3.UseVisualStyleBackColor = True
        '
        'lblDescGrid3
        '
        Me.lblDescGrid3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblDescGrid3.AutoSize = True
        Me.lblDescGrid3.BackColor = System.Drawing.Color.Transparent
        Me.lblDescGrid3.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblDescGrid3.ForeColor = System.Drawing.Color.Black
        Me.lblDescGrid3.Location = New System.Drawing.Point(150, 7)
        Me.lblDescGrid3.Name = "lblDescGrid3"
        Me.lblDescGrid3.Size = New System.Drawing.Size(144, 13)
        Me.lblDescGrid3.TabIndex = 0
        Me.lblDescGrid3.Text = "Default BsDataGridView"
        Me.lblDescGrid3.Title = False
        '
        'lblElapsedText3
        '
        Me.lblElapsedText3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblElapsedText3.AutoSize = True
        Me.lblElapsedText3.BackColor = System.Drawing.Color.Transparent
        Me.lblElapsedText3.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblElapsedText3.ForeColor = System.Drawing.Color.Black
        Me.lblElapsedText3.Location = New System.Drawing.Point(9, 267)
        Me.lblElapsedText3.Name = "lblElapsedText3"
        Me.lblElapsedText3.Size = New System.Drawing.Size(85, 13)
        Me.lblElapsedText3.TabIndex = 0
        Me.lblElapsedText3.Text = "Elapsed time:"
        Me.lblElapsedText3.Title = False
        '
        'lblTime3
        '
        Me.lblTime3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblTime3.AutoSize = True
        Me.lblTime3.BackColor = System.Drawing.Color.Transparent
        Me.lblTime3.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.lblTime3.ForeColor = System.Drawing.Color.Black
        Me.lblTime3.Location = New System.Drawing.Point(95, 267)
        Me.lblTime3.Name = "lblTime3"
        Me.lblTime3.Size = New System.Drawing.Size(31, 13)
        Me.lblTime3.TabIndex = 0
        Me.lblTime3.Text = "0ms"
        Me.lblTime3.Title = False
        '
        'FormStressGrids_JBL
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(886, 885)
        Me.Controls.Add(Me.pnlGrid3)
        Me.Controls.Add(Me.pnlGrid2)
        Me.Controls.Add(Me.pnlGrid1)
        Me.Controls.Add(Me.pbRegisters)
        Me.Controls.Add(Me.lblRegisters)
        Me.Controls.Add(Me.lblAction)
        Me.Controls.Add(Me.grpDataTable)
        Me.Name = "FormStressGrids_JBL"
        Me.Text = "Binding grids test"
        Me.grpDataTable.ResumeLayout(False)
        Me.grpDataTable.PerformLayout()
        CType(Me.numRows, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dxpGrid1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dxpGrid1View, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlGrid1.ResumeLayout(False)
        Me.pnlGrid1.PerformLayout()
        Me.pnlGrid2.ResumeLayout(False)
        Me.pnlGrid2.PerformLayout()
        CType(Me.dxpGrid2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dxpGrid2View, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlGrid3.ResumeLayout(False)
        Me.pnlGrid3.PerformLayout()
        CType(Me.dgrGrid3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents grpDataTable As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents numRows As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents lblNumRows As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents dxpGrid1 As DevExpress.XtraGrid.GridControl
    Friend WithEvents dxpGrid1View As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents lblElapsedText1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents lblTime1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents lblAction As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents pbRegisters As Biosystems.Ax00.Controls.UserControls.BSProgressBar
    Friend WithEvents lblRegisters As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents butBindGrid1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents pnlGrid1 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents lblDescGrid1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents pnlGrid2 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents butBindGrid2 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents dxpGrid2 As DevExpress.XtraGrid.GridControl
    Friend WithEvents dxpGrid2View As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents lblDescGrid2 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents lblElapsedText2 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents lblTime2 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents ctrlNavGrid2 As DevExpress.XtraEditors.ControlNavigator
    Friend WithEvents pnlGrid3 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents butBindGrid3 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents lblDescGrid3 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents lblElapsedText3 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents lblTime3 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents dgrGrid3 As Biosystems.Ax00.Controls.UserControls.BSDataGridView
End Class
