<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IBarCodeAdjustments
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
        Me.components = New System.ComponentModel.Container
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IBarCodeAdjustments))
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ProgressBar1 = New Biosystems.Ax00.Controls.UserControls.BSProgressBar
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsCancelButtonNOUSED = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsSaveButtonNOUSED = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustButtonNOUSED = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.TestGroupBox = New System.Windows.Forms.GroupBox
        Me.BarCodeDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.TestButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.ReadingBCGroupBox = New System.Windows.Forms.GroupBox
        Me.StopButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.ReadedValueLabel = New System.Windows.Forms.Label
        Me.StartReadingButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.CenterRotorGroupBox = New System.Windows.Forms.GroupBox
        Me.CancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.SaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjust = New Biosystems.Ax00.Controls.UserControls.BSAdjustControl
        Me.AdjustButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.SelectRotorGroupBox = New System.Windows.Forms.GroupBox
        Me.ReagentRotorRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.SampleRotorRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.BsButton6 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.BsSubtitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsDemoModeInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoXPSViewer = New BsXPSViewer
        Me.BsInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsTitleLabel = New System.Windows.Forms.Label
        Me.TestProcessTimer = New System.Windows.Forms.Timer(Me.components)
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsButtonsPanel.SuspendLayout()
        Me.BsAdjustPanel.SuspendLayout()
        Me.TestGroupBox.SuspendLayout()
        CType(Me.BarCodeDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ReadingBCGroupBox.SuspendLayout()
        Me.CenterRotorGroupBox.SuspendLayout()
        Me.SelectRotorGroupBox.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsDemoModeInfoPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'BsMessagesPanel
        '
        Me.BsMessagesPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessagesPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsMessagesPanel.Controls.Add(Me.ProgressBar1)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageImage)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageLabel)
        Me.BsMessagesPanel.Location = New System.Drawing.Point(0, 557)
        Me.BsMessagesPanel.Name = "BsMessagesPanel"
        Me.BsMessagesPanel.Size = New System.Drawing.Size(811, 35)
        Me.BsMessagesPanel.TabIndex = 19
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(490, 8)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(180, 18)
        Me.ProgressBar1.TabIndex = 8
        Me.ProgressBar1.Visible = False
        '
        'BsMessageImage
        '
        Me.BsMessageImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
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
        Me.BsMessageLabel.Size = New System.Drawing.Size(762, 13)
        Me.BsMessageLabel.TabIndex = 1
        Me.BsMessageLabel.Title = False
        '
        'BsButtonsPanel
        '
        Me.BsButtonsPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButtonsPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsButtonsPanel.Controls.Add(Me.BsExitButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsCancelButtonNOUSED)
        Me.BsButtonsPanel.Controls.Add(Me.BsSaveButtonNOUSED)
        Me.BsButtonsPanel.Controls.Add(Me.BsAdjustButtonNOUSED)
        Me.BsButtonsPanel.Location = New System.Drawing.Point(810, 557)
        Me.BsButtonsPanel.Name = "BsButtonsPanel"
        Me.BsButtonsPanel.Size = New System.Drawing.Size(168, 35)
        Me.BsButtonsPanel.TabIndex = 18
        '
        'BsExitButton
        '
        Me.BsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsExitButton.Location = New System.Drawing.Point(134, 1)
        Me.BsExitButton.Name = "BsExitButton"
        Me.BsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.BsExitButton.TabIndex = 17
        Me.BsExitButton.UseVisualStyleBackColor = True
        '
        'BsCancelButtonNOUSED
        '
        Me.BsCancelButtonNOUSED.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCancelButtonNOUSED.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsCancelButtonNOUSED.Enabled = False
        Me.BsCancelButtonNOUSED.Location = New System.Drawing.Point(98, 1)
        Me.BsCancelButtonNOUSED.Name = "BsCancelButtonNOUSED"
        Me.BsCancelButtonNOUSED.Size = New System.Drawing.Size(32, 32)
        Me.BsCancelButtonNOUSED.TabIndex = 16
        Me.BsCancelButtonNOUSED.UseVisualStyleBackColor = True
        Me.BsCancelButtonNOUSED.Visible = False
        '
        'BsSaveButtonNOUSED
        '
        Me.BsSaveButtonNOUSED.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsSaveButtonNOUSED.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsSaveButtonNOUSED.Enabled = False
        Me.BsSaveButtonNOUSED.Location = New System.Drawing.Point(62, 1)
        Me.BsSaveButtonNOUSED.Name = "BsSaveButtonNOUSED"
        Me.BsSaveButtonNOUSED.Size = New System.Drawing.Size(32, 32)
        Me.BsSaveButtonNOUSED.TabIndex = 1
        Me.BsSaveButtonNOUSED.UseVisualStyleBackColor = True
        Me.BsSaveButtonNOUSED.Visible = False
        '
        'BsAdjustButtonNOUSED
        '
        Me.BsAdjustButtonNOUSED.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustButtonNOUSED.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsAdjustButtonNOUSED.Enabled = False
        Me.BsAdjustButtonNOUSED.Location = New System.Drawing.Point(26, 1)
        Me.BsAdjustButtonNOUSED.Name = "BsAdjustButtonNOUSED"
        Me.BsAdjustButtonNOUSED.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjustButtonNOUSED.TabIndex = 0
        Me.BsAdjustButtonNOUSED.UseVisualStyleBackColor = True
        Me.BsAdjustButtonNOUSED.Visible = False
        '
        'BsAdjustPanel
        '
        Me.BsAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsAdjustPanel.Controls.Add(Me.TestGroupBox)
        Me.BsAdjustPanel.Controls.Add(Me.ReadingBCGroupBox)
        Me.BsAdjustPanel.Controls.Add(Me.CenterRotorGroupBox)
        Me.BsAdjustPanel.Controls.Add(Me.SelectRotorGroupBox)
        Me.BsAdjustPanel.Controls.Add(Me.PictureBox1)
        Me.BsAdjustPanel.Controls.Add(Me.BsSubtitleLabel)
        Me.BsAdjustPanel.Location = New System.Drawing.Point(234, 25)
        Me.BsAdjustPanel.Name = "BsAdjustPanel"
        Me.BsAdjustPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsAdjustPanel.TabIndex = 64
        '
        'TestGroupBox
        '
        Me.TestGroupBox.Controls.Add(Me.BarCodeDataGridView)
        Me.TestGroupBox.Controls.Add(Me.TestButton)
        Me.TestGroupBox.Location = New System.Drawing.Point(9, 337)
        Me.TestGroupBox.Name = "TestGroupBox"
        Me.TestGroupBox.Size = New System.Drawing.Size(723, 178)
        Me.TestGroupBox.TabIndex = 76
        Me.TestGroupBox.TabStop = False
        Me.TestGroupBox.Text = "4. Test"
        '
        'BarCodeDataGridView
        '
        Me.BarCodeDataGridView.AllowUserToAddRows = False
        Me.BarCodeDataGridView.AllowUserToDeleteRows = False
        Me.BarCodeDataGridView.AllowUserToResizeColumns = False
        Me.BarCodeDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.BarCodeDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.BarCodeDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.BarCodeDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
        Me.BarCodeDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.BarCodeDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.BarCodeDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BarCodeDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.BarCodeDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BarCodeDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.BarCodeDataGridView.EnterToTab = False
        Me.BarCodeDataGridView.GridColor = System.Drawing.Color.Silver
        Me.BarCodeDataGridView.Location = New System.Drawing.Point(24, 20)
        Me.BarCodeDataGridView.MultiSelect = False
        Me.BarCodeDataGridView.Name = "BarCodeDataGridView"
        Me.BarCodeDataGridView.ReadOnly = True
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BarCodeDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.BarCodeDataGridView.RowHeadersVisible = False
        Me.BarCodeDataGridView.Size = New System.Drawing.Size(476, 152)
        Me.BarCodeDataGridView.TabIndex = 63
        Me.BarCodeDataGridView.TabToEnter = False
        '
        'TestButton
        '
        Me.TestButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TestButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.TestButton.Location = New System.Drawing.Point(687, 142)
        Me.TestButton.Name = "TestButton"
        Me.TestButton.Size = New System.Drawing.Size(32, 32)
        Me.TestButton.TabIndex = 14
        Me.TestButton.UseVisualStyleBackColor = True
        '
        'ReadingBCGroupBox
        '
        Me.ReadingBCGroupBox.Controls.Add(Me.StopButton)
        Me.ReadingBCGroupBox.Controls.Add(Me.ReadedValueLabel)
        Me.ReadingBCGroupBox.Controls.Add(Me.StartReadingButton)
        Me.ReadingBCGroupBox.Location = New System.Drawing.Point(9, 246)
        Me.ReadingBCGroupBox.Name = "ReadingBCGroupBox"
        Me.ReadingBCGroupBox.Size = New System.Drawing.Size(723, 85)
        Me.ReadingBCGroupBox.TabIndex = 75
        Me.ReadingBCGroupBox.TabStop = False
        Me.ReadingBCGroupBox.Text = "3. Reading Test/Adjustment"
        '
        'StopButton
        '
        Me.StopButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.StopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.StopButton.Location = New System.Drawing.Point(687, 42)
        Me.StopButton.Name = "StopButton"
        Me.StopButton.Size = New System.Drawing.Size(32, 32)
        Me.StopButton.TabIndex = 106
        Me.StopButton.UseVisualStyleBackColor = True
        Me.StopButton.Visible = False
        '
        'ReadedValueLabel
        '
        Me.ReadedValueLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ReadedValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ReadedValueLabel.Font = New System.Drawing.Font("Digiface", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ReadedValueLabel.ForeColor = System.Drawing.Color.Black
        Me.ReadedValueLabel.Location = New System.Drawing.Point(26, 33)
        Me.ReadedValueLabel.Name = "ReadedValueLabel"
        Me.ReadedValueLabel.Size = New System.Drawing.Size(120, 34)
        Me.ReadedValueLabel.TabIndex = 105
        Me.ReadedValueLabel.Text = "100 %"
        Me.ReadedValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'StartReadingButton
        '
        Me.StartReadingButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.StartReadingButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.StartReadingButton.Location = New System.Drawing.Point(687, 42)
        Me.StartReadingButton.Name = "StartReadingButton"
        Me.StartReadingButton.Size = New System.Drawing.Size(30, 30)
        Me.StartReadingButton.TabIndex = 14
        Me.StartReadingButton.UseVisualStyleBackColor = True
        '
        'CenterRotorGroupBox
        '
        Me.CenterRotorGroupBox.Controls.Add(Me.CancelButton)
        Me.CenterRotorGroupBox.Controls.Add(Me.SaveButton)
        Me.CenterRotorGroupBox.Controls.Add(Me.BsAdjust)
        Me.CenterRotorGroupBox.Controls.Add(Me.AdjustButton)
        Me.CenterRotorGroupBox.Location = New System.Drawing.Point(9, 94)
        Me.CenterRotorGroupBox.Name = "CenterRotorGroupBox"
        Me.CenterRotorGroupBox.Size = New System.Drawing.Size(723, 146)
        Me.CenterRotorGroupBox.TabIndex = 74
        Me.CenterRotorGroupBox.TabStop = False
        Me.CenterRotorGroupBox.Text = "2. Center Rotor"
        '
        'CancelButton
        '
        Me.CancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.CancelButton.Location = New System.Drawing.Point(687, 102)
        Me.CancelButton.Name = "CancelButton"
        Me.CancelButton.Size = New System.Drawing.Size(32, 32)
        Me.CancelButton.TabIndex = 17
        Me.CancelButton.UseVisualStyleBackColor = True
        '
        'SaveButton
        '
        Me.SaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.SaveButton.Location = New System.Drawing.Point(651, 102)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(32, 32)
        Me.SaveButton.TabIndex = 16
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'BsAdjust
        '
        Me.BsAdjust.AdjustButtonMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.AdjustButtonModes.LeftRight
        Me.BsAdjust.AdjustingEnabled = True
        Me.BsAdjust.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjust.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjust.CurrentStepValue = 1
        Me.BsAdjust.CurrentValue = 0.0!
        Me.BsAdjust.DisplayBackColor = System.Drawing.Color.Black
        Me.BsAdjust.DisplayEditingForeColor = System.Drawing.Color.White
        Me.BsAdjust.DisplayForeColor = System.Drawing.Color.LightGreen
        Me.BsAdjust.EditingEnabled = True
        Me.BsAdjust.EditionMode = False
        Me.BsAdjust.Enabled = False
        Me.BsAdjust.FocusedBackColor = System.Drawing.Color.Blue
        Me.BsAdjust.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAdjust.HomingEnabled = True
        Me.BsAdjust.IncreaseMode = Biosystems.Ax00.Controls.UserControls.BSAdjustControl.IncreaseModes.Direct
        Me.BsAdjust.InfoBackColor = System.Drawing.Color.LightSteelBlue
        Me.BsAdjust.InfoTitlesForeColor = System.Drawing.Color.Black
        Me.BsAdjust.InfoValuesForeColor = System.Drawing.Color.Black
        Me.BsAdjust.IsFocused = False
        Me.BsAdjust.LastValueSavedTitle = "Last:"
        Me.BsAdjust.Location = New System.Drawing.Point(24, 26)
        Me.BsAdjust.MaximumLimit = 50000.0!
        Me.BsAdjust.MaximumSize = New System.Drawing.Size(153, 127)
        Me.BsAdjust.MaxNumDecimals = 0
        Me.BsAdjust.MinimumLimit = 0.0!
        Me.BsAdjust.MinimumSize = New System.Drawing.Size(153, 87)
        Me.BsAdjust.Name = "BsAdjust"
        Me.BsAdjust.RangeTitle = "Range:"
        Me.BsAdjust.SimulationMode = False
        Me.BsAdjust.Size = New System.Drawing.Size(153, 106)
        Me.BsAdjust.StepValues = CType(resources.GetObject("BsAdjust.StepValues"), System.Collections.Generic.List(Of Integer))
        Me.BsAdjust.TabIndex = 15
        Me.BsAdjust.UnFocusedBackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjust.UnitsCaption = "steps"
        '
        'AdjustButton
        '
        Me.AdjustButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AdjustButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.AdjustButton.Location = New System.Drawing.Point(615, 102)
        Me.AdjustButton.Name = "AdjustButton"
        Me.AdjustButton.Size = New System.Drawing.Size(32, 32)
        Me.AdjustButton.TabIndex = 14
        Me.AdjustButton.UseVisualStyleBackColor = True
        '
        'SelectRotorGroupBox
        '
        Me.SelectRotorGroupBox.Controls.Add(Me.ReagentRotorRadioButton)
        Me.SelectRotorGroupBox.Controls.Add(Me.SampleRotorRadioButton)
        Me.SelectRotorGroupBox.Controls.Add(Me.BsButton6)
        Me.SelectRotorGroupBox.Location = New System.Drawing.Point(9, 29)
        Me.SelectRotorGroupBox.Name = "SelectRotorGroupBox"
        Me.SelectRotorGroupBox.Size = New System.Drawing.Size(723, 60)
        Me.SelectRotorGroupBox.TabIndex = 73
        Me.SelectRotorGroupBox.TabStop = False
        Me.SelectRotorGroupBox.Text = "1. Select Rotor"
        '
        'ReagentRotorRadioButton
        '
        Me.ReagentRotorRadioButton.AutoSize = True
        Me.ReagentRotorRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ReagentRotorRadioButton.Location = New System.Drawing.Point(161, 27)
        Me.ReagentRotorRadioButton.Name = "ReagentRotorRadioButton"
        Me.ReagentRotorRadioButton.Size = New System.Drawing.Size(72, 17)
        Me.ReagentRotorRadioButton.TabIndex = 107
        Me.ReagentRotorRadioButton.TabStop = True
        Me.ReagentRotorRadioButton.Text = "Reagent"
        Me.ReagentRotorRadioButton.UseVisualStyleBackColor = True
        '
        'SampleRotorRadioButton
        '
        Me.SampleRotorRadioButton.AutoSize = True
        Me.SampleRotorRadioButton.Checked = True
        Me.SampleRotorRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SampleRotorRadioButton.ForeColor = System.Drawing.Color.Black
        Me.SampleRotorRadioButton.Location = New System.Drawing.Point(24, 27)
        Me.SampleRotorRadioButton.Name = "SampleRotorRadioButton"
        Me.SampleRotorRadioButton.Size = New System.Drawing.Size(68, 17)
        Me.SampleRotorRadioButton.TabIndex = 108
        Me.SampleRotorRadioButton.TabStop = True
        Me.SampleRotorRadioButton.Text = "Sample"
        Me.SampleRotorRadioButton.UseVisualStyleBackColor = True
        '
        'BsButton6
        '
        Me.BsButton6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton6.Location = New System.Drawing.Point(688, 228)
        Me.BsButton6.Name = "BsButton6"
        Me.BsButton6.Size = New System.Drawing.Size(30, 30)
        Me.BsButton6.TabIndex = 106
        Me.BsButton6.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Transparent
        Me.PictureBox1.Location = New System.Drawing.Point(3, 23)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(736, 501)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 61
        Me.PictureBox1.TabStop = False
        '
        'BsSubtitleLabel
        '
        Me.BsSubtitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsSubtitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsSubtitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsSubtitleLabel.Location = New System.Drawing.Point(0, 0)
        Me.BsSubtitleLabel.Name = "BsSubtitleLabel"
        Me.BsSubtitleLabel.Size = New System.Drawing.Size(739, 20)
        Me.BsSubtitleLabel.TabIndex = 59
        Me.BsSubtitleLabel.Text = "Bar Code Adjustment/Test"
        Me.BsSubtitleLabel.Title = True
        '
        'BsDemoModeInfoPanel
        '
        Me.BsDemoModeInfoPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsDemoModeInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsDemoModeInfoPanel.Controls.Add(Me.BsInfoXPSViewer)
        Me.BsDemoModeInfoPanel.Controls.Add(Me.BsInfoTitle)
        Me.BsDemoModeInfoPanel.Location = New System.Drawing.Point(4, 25)
        Me.BsDemoModeInfoPanel.Name = "BsDemoModeInfoPanel"
        Me.BsDemoModeInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsDemoModeInfoPanel.TabIndex = 65
        '
        'BsInfoXPSViewer
        '
        Me.BsInfoXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoXPSViewer.CopyButtonVisible = True
        Me.BsInfoXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoXPSViewer.HorizontalPageMargin = 0
        Me.BsInfoXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoXPSViewer.IsLoaded = False
        Me.BsInfoXPSViewer.IsScrollable = False
        Me.BsInfoXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoXPSViewer.MenuBarVisible = False
        Me.BsInfoXPSViewer.Name = "BsInfoXPSViewer"
        Me.BsInfoXPSViewer.PopupMenuEnabled = True
        Me.BsInfoXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoXPSViewer.PrintButtonVisible = True
        Me.BsInfoXPSViewer.SearchBarVisible = False
        Me.BsInfoXPSViewer.Size = New System.Drawing.Size(230, 510)
        Me.BsInfoXPSViewer.TabIndex = 35
        Me.BsInfoXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoXPSViewer.VerticalPageMargin = 0
        Me.BsInfoXPSViewer.Visible = False
        Me.BsInfoXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoXPSViewer.WholePageButtonVisible = True
        '
        'BsInfoTitle
        '
        Me.BsInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsInfoTitle.Name = "BsInfoTitle"
        Me.BsInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsInfoTitle.TabIndex = 22
        Me.BsInfoTitle.Text = "Information"
        Me.BsInfoTitle.Title = True
        '
        'BsTitleLabel
        '
        Me.BsTitleLabel.AutoSize = True
        Me.BsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsTitleLabel.Location = New System.Drawing.Point(2, 2)
        Me.BsTitleLabel.Name = "BsTitleLabel"
        Me.BsTitleLabel.Size = New System.Drawing.Size(68, 17)
        Me.BsTitleLabel.TabIndex = 67
        Me.BsTitleLabel.Text = "BarCode"
        '
        'TestProcessTimer
        '
        '
        'IBarCodeAdjustments
        '
        Me.AcceptButton = Me.BsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(978, 593)
        Me.Controls.Add(Me.BsTitleLabel)
        Me.Controls.Add(Me.BsDemoModeInfoPanel)
        Me.Controls.Add(Me.BsAdjustPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "IBarCodeAdjustments"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = ""
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsButtonsPanel.ResumeLayout(False)
        Me.BsAdjustPanel.ResumeLayout(False)
        Me.TestGroupBox.ResumeLayout(False)
        CType(Me.BarCodeDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ReadingBCGroupBox.ResumeLayout(False)
        Me.CenterRotorGroupBox.ResumeLayout(False)
        Me.SelectRotorGroupBox.ResumeLayout(False)
        Me.SelectRotorGroupBox.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsDemoModeInfoPanel.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents ProgressBar1 As Biosystems.Ax00.Controls.UserControls.BSProgressBar
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsCancelButtonNOUSED As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsSaveButtonNOUSED As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjustButtonNOUSED As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Private WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents BsSubtitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsDemoModeInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsInfoXPSViewer As BsXPSViewer
    Friend WithEvents BsInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTitleLabel As System.Windows.Forms.Label
    Friend WithEvents SelectRotorGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsButton6 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ReagentRotorRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents SampleRotorRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents CenterRotorGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents AdjustButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjust As Biosystems.Ax00.Controls.UserControls.BSAdjustControl
    Friend WithEvents CancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents SaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ReadingBCGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents StartReadingButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents TestGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents TestButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BarCodeDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents ReadedValueLabel As System.Windows.Forms.Label
    Friend WithEvents StopButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents TestProcessTimer As System.Windows.Forms.Timer
End Class
