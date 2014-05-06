<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class bsResultsChart
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.components = New System.ComponentModel.Container
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.bsReplicateLabel = New System.Windows.Forms.Label
        Me.bsWellLabel = New System.Windows.Forms.Label
        Me.bsReplicateTitle = New System.Windows.Forms.Label
        Me.bsWellTitle = New System.Windows.Forms.Label
        Me.bsHeaderPanel = New System.Windows.Forms.Panel
        Me.bsCalibNumLabel = New System.Windows.Forms.Label
        Me.bsCalibNumTitle = New System.Windows.Forms.Label
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsIconPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.bsTestLabel = New System.Windows.Forms.Label
        Me.bsTestTitle = New System.Windows.Forms.Label
        Me.bsSampleLabel = New System.Windows.Forms.Label
        Me.bsSampleTitle = New System.Windows.Forms.Label
        Me.RecoverCycleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.bsChartToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.RemoveMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.bsChartContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.bsAbsorbanceLabel = New System.Windows.Forms.Label
        Me.bsCyclesLabel = New System.Windows.Forms.Label
        Me.bsChartPanel = New System.Windows.Forms.Panel
        Me.bsChartPictureBox = New System.Windows.Forms.PictureBox
        Me.bsResultDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.CycleVisible = New System.Windows.Forms.DataGridViewCheckBoxColumn
        Me.Cycle = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Absorbance1 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Absorbance2 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Difference = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.bsChartInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsDiffPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsDiffLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsDiffPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.bsAbs2Panel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsAbs2Label = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsAbs2PictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.bsModoComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsAbs1Panel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsAbs1Label = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsAbs1PictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.bsHeaderPanel.SuspendLayout()
        CType(Me.bsIconPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsChartContextMenu.SuspendLayout()
        Me.bsChartPanel.SuspendLayout()
        CType(Me.bsChartPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsResultDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsChartInfoPanel.SuspendLayout()
        Me.bsDiffPanel.SuspendLayout()
        CType(Me.BsDiffPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsAbs2Panel.SuspendLayout()
        CType(Me.bsAbs2PictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsAbs1Panel.SuspendLayout()
        CType(Me.BsAbs1PictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsReplicateLabel
        '
        Me.bsReplicateLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsReplicateLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReplicateLabel.ForeColor = System.Drawing.Color.Black
        Me.bsReplicateLabel.Location = New System.Drawing.Point(790, 23)
        Me.bsReplicateLabel.Name = "bsReplicateLabel"
        Me.bsReplicateLabel.Size = New System.Drawing.Size(35, 13)
        Me.bsReplicateLabel.TabIndex = 8
        Me.bsReplicateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'bsWellLabel
        '
        Me.bsWellLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsWellLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsWellLabel.ForeColor = System.Drawing.Color.Black
        Me.bsWellLabel.Location = New System.Drawing.Point(790, 5)
        Me.bsWellLabel.Name = "bsWellLabel"
        Me.bsWellLabel.Size = New System.Drawing.Size(35, 13)
        Me.bsWellLabel.TabIndex = 6
        Me.bsWellLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'bsReplicateTitle
        '
        Me.bsReplicateTitle.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsReplicateTitle.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsReplicateTitle.ForeColor = System.Drawing.Color.Black
        Me.bsReplicateTitle.Location = New System.Drawing.Point(710, 23)
        Me.bsReplicateTitle.Name = "bsReplicateTitle"
        Me.bsReplicateTitle.Size = New System.Drawing.Size(75, 13)
        Me.bsReplicateTitle.TabIndex = 7
        Me.bsReplicateTitle.Text = "Replicate:"
        Me.bsReplicateTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'bsWellTitle
        '
        Me.bsWellTitle.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsWellTitle.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsWellTitle.ForeColor = System.Drawing.Color.Black
        Me.bsWellTitle.Location = New System.Drawing.Point(710, 5)
        Me.bsWellTitle.Name = "bsWellTitle"
        Me.bsWellTitle.Size = New System.Drawing.Size(75, 13)
        Me.bsWellTitle.TabIndex = 5
        Me.bsWellTitle.Text = "Well:"
        Me.bsWellTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'bsHeaderPanel
        '
        Me.bsHeaderPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsHeaderPanel.BackColor = System.Drawing.Color.LightGray
        Me.bsHeaderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsHeaderPanel.Controls.Add(Me.bsCalibNumLabel)
        Me.bsHeaderPanel.Controls.Add(Me.bsCalibNumTitle)
        Me.bsHeaderPanel.Controls.Add(Me.bsExitButton)
        Me.bsHeaderPanel.Controls.Add(Me.bsIconPictureBox)
        Me.bsHeaderPanel.Controls.Add(Me.bsReplicateLabel)
        Me.bsHeaderPanel.Controls.Add(Me.bsReplicateTitle)
        Me.bsHeaderPanel.Controls.Add(Me.bsWellLabel)
        Me.bsHeaderPanel.Controls.Add(Me.bsWellTitle)
        Me.bsHeaderPanel.Controls.Add(Me.bsTestLabel)
        Me.bsHeaderPanel.Controls.Add(Me.bsTestTitle)
        Me.bsHeaderPanel.Controls.Add(Me.bsSampleLabel)
        Me.bsHeaderPanel.Controls.Add(Me.bsSampleTitle)
        Me.bsHeaderPanel.Location = New System.Drawing.Point(4, 4)
        Me.bsHeaderPanel.Name = "bsHeaderPanel"
        Me.bsHeaderPanel.Size = New System.Drawing.Size(867, 41)
        Me.bsHeaderPanel.TabIndex = 16
        '
        'bsCalibNumLabel
        '
        Me.bsCalibNumLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsCalibNumLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCalibNumLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCalibNumLabel.Location = New System.Drawing.Point(450, 5)
        Me.bsCalibNumLabel.Name = "bsCalibNumLabel"
        Me.bsCalibNumLabel.Size = New System.Drawing.Size(25, 13)
        Me.bsCalibNumLabel.TabIndex = 28
        Me.bsCalibNumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'bsCalibNumTitle
        '
        Me.bsCalibNumTitle.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsCalibNumTitle.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCalibNumTitle.ForeColor = System.Drawing.Color.Black
        Me.bsCalibNumTitle.Location = New System.Drawing.Point(370, 5)
        Me.bsCalibNumTitle.Name = "bsCalibNumTitle"
        Me.bsCalibNumTitle.Size = New System.Drawing.Size(75, 13)
        Me.bsCalibNumTitle.TabIndex = 27
        Me.bsCalibNumTitle.Text = "Calib No:"
        Me.bsCalibNumTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'bsExitButton
        '
        Me.bsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(830, 4)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 26
        Me.bsExitButton.Text = "X"
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsIconPictureBox
        '
        Me.bsIconPictureBox.Location = New System.Drawing.Point(5, 4)
        Me.bsIconPictureBox.Name = "bsIconPictureBox"
        Me.bsIconPictureBox.PositionNumber = 0
        Me.bsIconPictureBox.Size = New System.Drawing.Size(32, 32)
        Me.bsIconPictureBox.TabIndex = 10
        Me.bsIconPictureBox.TabStop = False
        '
        'bsTestLabel
        '
        Me.bsTestLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsTestLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsTestLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestLabel.Location = New System.Drawing.Point(565, 23)
        Me.bsTestLabel.Name = "bsTestLabel"
        Me.bsTestLabel.Size = New System.Drawing.Size(140, 13)
        Me.bsTestLabel.TabIndex = 4
        Me.bsTestLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'bsTestTitle
        '
        Me.bsTestTitle.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsTestTitle.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestTitle.ForeColor = System.Drawing.Color.Black
        Me.bsTestTitle.Location = New System.Drawing.Point(480, 23)
        Me.bsTestTitle.Name = "bsTestTitle"
        Me.bsTestTitle.Size = New System.Drawing.Size(75, 13)
        Me.bsTestTitle.TabIndex = 3
        Me.bsTestTitle.Text = "Test:"
        Me.bsTestTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'bsSampleLabel
        '
        Me.bsSampleLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsSampleLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSampleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleLabel.Location = New System.Drawing.Point(565, 5)
        Me.bsSampleLabel.Name = "bsSampleLabel"
        Me.bsSampleLabel.Size = New System.Drawing.Size(122, 13)
        Me.bsSampleLabel.TabIndex = 2
        Me.bsSampleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'bsSampleTitle
        '
        Me.bsSampleTitle.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsSampleTitle.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTitle.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTitle.Location = New System.Drawing.Point(480, 5)
        Me.bsSampleTitle.Name = "bsSampleTitle"
        Me.bsSampleTitle.Size = New System.Drawing.Size(75, 13)
        Me.bsSampleTitle.TabIndex = 1
        Me.bsSampleTitle.Text = "Sample:"
        Me.bsSampleTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'RecoverCycleToolStripMenuItem
        '
        Me.RecoverCycleToolStripMenuItem.Name = "RecoverCycleToolStripMenuItem"
        Me.RecoverCycleToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
        Me.RecoverCycleToolStripMenuItem.Text = "Recover cycle"
        '
        'bsChartToolTip
        '
        Me.bsChartToolTip.AutomaticDelay = 2000
        '
        'RemoveMenuItem
        '
        Me.RemoveMenuItem.Name = "RemoveMenuItem"
        Me.RemoveMenuItem.Size = New System.Drawing.Size(122, 22)
        Me.RemoveMenuItem.Text = "Remove cycle"
        '
        'bsChartContextMenu
        '
        Me.bsChartContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RemoveMenuItem, Me.RecoverCycleToolStripMenuItem})
        Me.bsChartContextMenu.Name = "BsGridContextMenu"
        Me.bsChartContextMenu.ShowImageMargin = False
        Me.bsChartContextMenu.Size = New System.Drawing.Size(123, 48)
        '
        'bsAbsorbanceLabel
        '
        Me.bsAbsorbanceLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bsAbsorbanceLabel.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAbsorbanceLabel.ForeColor = System.Drawing.Color.Black
        Me.bsAbsorbanceLabel.Location = New System.Drawing.Point(4, 7)
        Me.bsAbsorbanceLabel.Name = "bsAbsorbanceLabel"
        Me.bsAbsorbanceLabel.Size = New System.Drawing.Size(14, 319)
        Me.bsAbsorbanceLabel.TabIndex = 2
        Me.bsAbsorbanceLabel.Text = "LABEL"
        Me.bsAbsorbanceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'bsCyclesLabel
        '
        Me.bsCyclesLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsCyclesLabel.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCyclesLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCyclesLabel.Location = New System.Drawing.Point(32, 338)
        Me.bsCyclesLabel.Name = "bsCyclesLabel"
        Me.bsCyclesLabel.Size = New System.Drawing.Size(625, 14)
        Me.bsCyclesLabel.TabIndex = 1
        Me.bsCyclesLabel.Text = "Label1"
        Me.bsCyclesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'bsChartPanel
        '
        Me.bsChartPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsChartPanel.BackColor = System.Drawing.Color.White
        Me.bsChartPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsChartPanel.Controls.Add(Me.bsChartPictureBox)
        Me.bsChartPanel.Controls.Add(Me.bsAbsorbanceLabel)
        Me.bsChartPanel.Controls.Add(Me.bsCyclesLabel)
        Me.bsChartPanel.Location = New System.Drawing.Point(4, 50)
        Me.bsChartPanel.Name = "bsChartPanel"
        Me.bsChartPanel.Size = New System.Drawing.Size(662, 359)
        Me.bsChartPanel.TabIndex = 15
        '
        'bsChartPictureBox
        '
        Me.bsChartPictureBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsChartPictureBox.BackColor = System.Drawing.Color.White
        Me.bsChartPictureBox.Location = New System.Drawing.Point(19, 3)
        Me.bsChartPictureBox.Name = "bsChartPictureBox"
        Me.bsChartPictureBox.Size = New System.Drawing.Size(642, 335)
        Me.bsChartPictureBox.TabIndex = 0
        Me.bsChartPictureBox.TabStop = False
        '
        'bsResultDataGridView
        '
        Me.bsResultDataGridView.AllowUserToAddRows = False
        Me.bsResultDataGridView.AllowUserToDeleteRows = False
        Me.bsResultDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsResultDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsResultDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.bsResultDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsResultDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsResultDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsResultDataGridView.ColumnHeadersHeight = 20
        Me.bsResultDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.bsResultDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.CycleVisible, Me.Cycle, Me.Absorbance1, Me.Absorbance2, Me.Difference})
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsResultDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsResultDataGridView.EnterToTab = False
        Me.bsResultDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsResultDataGridView.Location = New System.Drawing.Point(671, 50)
        Me.bsResultDataGridView.MultiSelect = False
        Me.bsResultDataGridView.Name = "bsResultDataGridView"
        Me.bsResultDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsResultDataGridView.RowHeadersVisible = False
        Me.bsResultDataGridView.RowHeadersWidth = 20
        Me.bsResultDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsResultDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsResultDataGridView.Size = New System.Drawing.Size(200, 405)
        Me.bsResultDataGridView.TabIndex = 18
        Me.bsResultDataGridView.TabToEnter = False
        '
        'CycleVisible
        '
        Me.CycleVisible.HeaderText = "Visible"
        Me.CycleVisible.Name = "CycleVisible"
        Me.CycleVisible.Visible = False
        '
        'Cycle
        '
        Me.Cycle.HeaderText = "Cycle"
        Me.Cycle.Name = "Cycle"
        '
        'Absorbance1
        '
        Me.Absorbance1.HeaderText = "Abs1"
        Me.Absorbance1.Name = "Absorbance1"
        '
        'Absorbance2
        '
        Me.Absorbance2.HeaderText = "Abs2"
        Me.Absorbance2.Name = "Absorbance2"
        '
        'Difference
        '
        Me.Difference.HeaderText = "Diff"
        Me.Difference.Name = "Difference"
        '
        'bsChartInfoPanel
        '
        Me.bsChartInfoPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsChartInfoPanel.BackColor = System.Drawing.Color.LightGray
        Me.bsChartInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsChartInfoPanel.Controls.Add(Me.bsDiffPanel)
        Me.bsChartInfoPanel.Controls.Add(Me.bsAbs2Panel)
        Me.bsChartInfoPanel.Controls.Add(Me.bsModoComboBox)
        Me.bsChartInfoPanel.Controls.Add(Me.bsAbs1Panel)
        Me.bsChartInfoPanel.Location = New System.Drawing.Point(4, 414)
        Me.bsChartInfoPanel.Name = "bsChartInfoPanel"
        Me.bsChartInfoPanel.Size = New System.Drawing.Size(662, 41)
        Me.bsChartInfoPanel.TabIndex = 17
        '
        'bsDiffPanel
        '
        Me.bsDiffPanel.BackColor = System.Drawing.Color.Transparent
        Me.bsDiffPanel.Controls.Add(Me.bsDiffLabel)
        Me.bsDiffPanel.Controls.Add(Me.BsDiffPictureBox)
        Me.bsDiffPanel.Location = New System.Drawing.Point(317, 10)
        Me.bsDiffPanel.Name = "bsDiffPanel"
        Me.bsDiffPanel.Size = New System.Drawing.Size(157, 21)
        Me.bsDiffPanel.TabIndex = 7
        Me.bsDiffPanel.Visible = False
        '
        'bsDiffLabel
        '
        Me.bsDiffLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDiffLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDiffLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDiffLabel.Location = New System.Drawing.Point(5, 4)
        Me.bsDiffLabel.Name = "bsDiffLabel"
        Me.bsDiffLabel.Size = New System.Drawing.Size(85, 13)
        Me.bsDiffLabel.TabIndex = 4
        Me.bsDiffLabel.Text = "BsLabel1"
        Me.bsDiffLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsDiffLabel.Title = False
        '
        'BsDiffPictureBox
        '
        Me.BsDiffPictureBox.BackColor = System.Drawing.Color.SlateGray
        Me.BsDiffPictureBox.Location = New System.Drawing.Point(95, 9)
        Me.BsDiffPictureBox.MaximumSize = New System.Drawing.Size(50, 4)
        Me.BsDiffPictureBox.MinimumSize = New System.Drawing.Size(50, 4)
        Me.BsDiffPictureBox.Name = "BsDiffPictureBox"
        Me.BsDiffPictureBox.PositionNumber = 0
        Me.BsDiffPictureBox.Size = New System.Drawing.Size(50, 4)
        Me.BsDiffPictureBox.TabIndex = 1
        Me.BsDiffPictureBox.TabStop = False
        '
        'bsAbs2Panel
        '
        Me.bsAbs2Panel.BackColor = System.Drawing.Color.Transparent
        Me.bsAbs2Panel.Controls.Add(Me.bsAbs2Label)
        Me.bsAbs2Panel.Controls.Add(Me.bsAbs2PictureBox)
        Me.bsAbs2Panel.Location = New System.Drawing.Point(161, 10)
        Me.bsAbs2Panel.Name = "bsAbs2Panel"
        Me.bsAbs2Panel.Size = New System.Drawing.Size(157, 21)
        Me.bsAbs2Panel.TabIndex = 6
        Me.bsAbs2Panel.Visible = False
        '
        'bsAbs2Label
        '
        Me.bsAbs2Label.BackColor = System.Drawing.Color.Transparent
        Me.bsAbs2Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsAbs2Label.ForeColor = System.Drawing.Color.Black
        Me.bsAbs2Label.Location = New System.Drawing.Point(5, 4)
        Me.bsAbs2Label.Name = "bsAbs2Label"
        Me.bsAbs2Label.Size = New System.Drawing.Size(85, 13)
        Me.bsAbs2Label.TabIndex = 4
        Me.bsAbs2Label.Text = "BsLabel1"
        Me.bsAbs2Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsAbs2Label.Title = False
        '
        'bsAbs2PictureBox
        '
        Me.bsAbs2PictureBox.BackColor = System.Drawing.Color.DeepSkyBlue
        Me.bsAbs2PictureBox.Location = New System.Drawing.Point(94, 9)
        Me.bsAbs2PictureBox.MaximumSize = New System.Drawing.Size(50, 4)
        Me.bsAbs2PictureBox.MinimumSize = New System.Drawing.Size(50, 4)
        Me.bsAbs2PictureBox.Name = "bsAbs2PictureBox"
        Me.bsAbs2PictureBox.PositionNumber = 0
        Me.bsAbs2PictureBox.Size = New System.Drawing.Size(50, 4)
        Me.bsAbs2PictureBox.TabIndex = 2
        Me.bsAbs2PictureBox.TabStop = False
        '
        'bsModoComboBox
        '
        Me.bsModoComboBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsModoComboBox.BackColor = System.Drawing.Color.White
        Me.bsModoComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsModoComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsModoComboBox.FormattingEnabled = True
        Me.bsModoComboBox.Location = New System.Drawing.Point(557, 10)
        Me.bsModoComboBox.Name = "bsModoComboBox"
        Me.bsModoComboBox.Size = New System.Drawing.Size(100, 21)
        Me.bsModoComboBox.TabIndex = 4
        Me.bsModoComboBox.Visible = False
        '
        'bsAbs1Panel
        '
        Me.bsAbs1Panel.BackColor = System.Drawing.Color.Transparent
        Me.bsAbs1Panel.Controls.Add(Me.bsAbs1Label)
        Me.bsAbs1Panel.Controls.Add(Me.BsAbs1PictureBox)
        Me.bsAbs1Panel.Location = New System.Drawing.Point(5, 10)
        Me.bsAbs1Panel.Name = "bsAbs1Panel"
        Me.bsAbs1Panel.Size = New System.Drawing.Size(157, 21)
        Me.bsAbs1Panel.TabIndex = 5
        Me.bsAbs1Panel.Visible = False
        '
        'bsAbs1Label
        '
        Me.bsAbs1Label.BackColor = System.Drawing.Color.Transparent
        Me.bsAbs1Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsAbs1Label.ForeColor = System.Drawing.Color.Black
        Me.bsAbs1Label.Location = New System.Drawing.Point(5, 4)
        Me.bsAbs1Label.Name = "bsAbs1Label"
        Me.bsAbs1Label.Size = New System.Drawing.Size(85, 13)
        Me.bsAbs1Label.TabIndex = 3
        Me.bsAbs1Label.Text = "BsLabel1"
        Me.bsAbs1Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsAbs1Label.Title = False
        '
        'BsAbs1PictureBox
        '
        Me.BsAbs1PictureBox.BackColor = System.Drawing.Color.Red
        Me.BsAbs1PictureBox.Location = New System.Drawing.Point(96, 9)
        Me.BsAbs1PictureBox.MaximumSize = New System.Drawing.Size(50, 4)
        Me.BsAbs1PictureBox.MinimumSize = New System.Drawing.Size(50, 4)
        Me.BsAbs1PictureBox.Name = "BsAbs1PictureBox"
        Me.BsAbs1PictureBox.PositionNumber = 0
        Me.BsAbs1PictureBox.Size = New System.Drawing.Size(50, 4)
        Me.BsAbs1PictureBox.TabIndex = 2
        Me.BsAbs1PictureBox.TabStop = False
        '
        'bsResultsChart
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Gainsboro
        Me.Controls.Add(Me.bsHeaderPanel)
        Me.Controls.Add(Me.bsResultDataGridView)
        Me.Controls.Add(Me.bsChartInfoPanel)
        Me.Controls.Add(Me.bsChartPanel)
        Me.Name = "bsResultsChart"
        Me.Size = New System.Drawing.Size(875, 459)
        Me.bsHeaderPanel.ResumeLayout(False)
        CType(Me.bsIconPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsChartContextMenu.ResumeLayout(False)
        Me.bsChartPanel.ResumeLayout(False)
        CType(Me.bsChartPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsResultDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsChartInfoPanel.ResumeLayout(False)
        Me.bsDiffPanel.ResumeLayout(False)
        CType(Me.BsDiffPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsAbs2Panel.ResumeLayout(False)
        CType(Me.bsAbs2PictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsAbs1Panel.ResumeLayout(False)
        CType(Me.BsAbs1PictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsIconPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsReplicateLabel As System.Windows.Forms.Label
    Friend WithEvents bsWellLabel As System.Windows.Forms.Label
    Friend WithEvents bsResultDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents CycleVisible As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents Cycle As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Absorbance1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Absorbance2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Difference As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents bsReplicateTitle As System.Windows.Forms.Label
    Friend WithEvents bsWellTitle As System.Windows.Forms.Label
    Friend WithEvents bsHeaderPanel As System.Windows.Forms.Panel
    Friend WithEvents bsTestLabel As System.Windows.Forms.Label
    Friend WithEvents bsTestTitle As System.Windows.Forms.Label
    Friend WithEvents bsSampleLabel As System.Windows.Forms.Label
    Friend WithEvents bsSampleTitle As System.Windows.Forms.Label
    Friend WithEvents bsChartInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsDiffPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsDiffLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsDiffPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsAbs2Panel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsAbs2Label As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAbs2PictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsModoComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsAbs1Panel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsAbs1Label As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsAbs1PictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents RecoverCycleToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents bsChartToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents RemoveMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents bsChartPictureBox As System.Windows.Forms.PictureBox
    Friend WithEvents bsChartContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents bsAbsorbanceLabel As System.Windows.Forms.Label
    Friend WithEvents bsCyclesLabel As System.Windows.Forms.Label
    Friend WithEvents bsChartPanel As System.Windows.Forms.Panel
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCalibNumLabel As System.Windows.Forms.Label
    Friend WithEvents bsCalibNumTitle As System.Windows.Forms.Label

End Class
