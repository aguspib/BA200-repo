<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiLevelDetectionTest
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiLevelDetectionTest))
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTitleLabel = New System.Windows.Forms.Label
        Me.BsLevelDetectionInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsLevelDetectionInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoXPSViewer = New BsXPSViewer
        Me.BsInfoExpandButton = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsFrequencyLayoutPanel = New System.Windows.Forms.TableLayoutPanel
        Me.BsFreqReagent2ValueLabel = New System.Windows.Forms.Label
        Me.BsFreqReagent1ValueLabel = New System.Windows.Forms.Label
        Me.BsFreqSampleValueLabel = New System.Windows.Forms.Label
        Me.BsFreqReagent2Label = New System.Windows.Forms.Label
        Me.BsFreqButtonPanel = New System.Windows.Forms.Panel
        Me.BsFrequencyButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsFreqReagent1Label = New System.Windows.Forms.Label
        Me.BsFreqSampleLabel = New System.Windows.Forms.Label
        Me.BsDetectionLayoutPanel = New System.Windows.Forms.TableLayoutPanel
        Me.BsDetectedLabel = New System.Windows.Forms.Label
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.BsDetectionButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsDetectionPosLabel = New System.Windows.Forms.Label
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.BsDetectionPosUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.BsDetectionArmLabel = New System.Windows.Forms.Label
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.BsDetectionMonitorLED = New Biosystems.Ax00.Controls.UserControls.BSMonitorLED
        Me.Panel4 = New System.Windows.Forms.Panel
        Me.BsDetectionArmComboBox = New System.Windows.Forms.ComboBox
        Me.BsTestTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsLevelDetectionTestPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsDetectionGroupBox = New System.Windows.Forms.GroupBox
        Me.BsFrequencyReadGroupBox = New System.Windows.Forms.GroupBox
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsButtonsPanel.SuspendLayout()
        Me.BsLevelDetectionInfoPanel.SuspendLayout()
        Me.BsFrequencyLayoutPanel.SuspendLayout()
        Me.BsFreqButtonPanel.SuspendLayout()
        Me.BsDetectionLayoutPanel.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.BsDetectionPosUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.BsLevelDetectionTestPanel.SuspendLayout()
        Me.BsDetectionGroupBox.SuspendLayout()
        Me.BsFrequencyReadGroupBox.SuspendLayout()
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
        Me.BsMessagesPanel.TabIndex = 35
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(429, 8)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(180, 18)
        Me.ProgressBar1.TabIndex = 4
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
        Me.BsButtonsPanel.Location = New System.Drawing.Point(810, 557)
        Me.BsButtonsPanel.Name = "BsButtonsPanel"
        Me.BsButtonsPanel.Size = New System.Drawing.Size(168, 35)
        Me.BsButtonsPanel.TabIndex = 34
        '
        'BsExitButton
        '
        Me.BsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsExitButton.Enabled = False
        Me.BsExitButton.Location = New System.Drawing.Point(134, 1)
        Me.BsExitButton.Name = "BsExitButton"
        Me.BsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.BsExitButton.TabIndex = 3
        Me.BsExitButton.UseVisualStyleBackColor = True
        '
        'BsTitleLabel
        '
        Me.BsTitleLabel.AutoSize = True
        Me.BsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsTitleLabel.Location = New System.Drawing.Point(2, 2)
        Me.BsTitleLabel.Name = "BsTitleLabel"
        Me.BsTitleLabel.Size = New System.Drawing.Size(115, 17)
        Me.BsTitleLabel.TabIndex = 65
        Me.BsTitleLabel.Text = "Level Detection"
        '
        'BsLevelDetectionInfoTitle
        '
        Me.BsLevelDetectionInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsLevelDetectionInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsLevelDetectionInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsLevelDetectionInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsLevelDetectionInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsLevelDetectionInfoTitle.Name = "BsLevelDetectionInfoTitle"
        Me.BsLevelDetectionInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsLevelDetectionInfoTitle.TabIndex = 22
        Me.BsLevelDetectionInfoTitle.Text = "Information"
        Me.BsLevelDetectionInfoTitle.Title = True
        '
        'BsLevelDetectionInfoPanel
        '
        Me.BsLevelDetectionInfoPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsLevelDetectionInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsLevelDetectionInfoPanel.Controls.Add(Me.BsInfoXPSViewer)
        Me.BsLevelDetectionInfoPanel.Controls.Add(Me.BsInfoExpandButton)
        Me.BsLevelDetectionInfoPanel.Controls.Add(Me.BsLevelDetectionInfoTitle)
        Me.BsLevelDetectionInfoPanel.Location = New System.Drawing.Point(4, 25)
        Me.BsLevelDetectionInfoPanel.Name = "BsLevelDetectionInfoPanel"
        Me.BsLevelDetectionInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsLevelDetectionInfoPanel.TabIndex = 61
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
        Me.BsInfoXPSViewer.HorizontalPageMargin = 10
        Me.BsInfoXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoXPSViewer.IsScrollable = False
        Me.BsInfoXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoXPSViewer.MenuBarVisible = False
        Me.BsInfoXPSViewer.Name = "BsInfoXPSViewer"
        Me.BsInfoXPSViewer.PopupMenuEnabled = True
        Me.BsInfoXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoXPSViewer.PrintButtonVisible = True
        Me.BsInfoXPSViewer.SearchBarVisible = False
        Me.BsInfoXPSViewer.Size = New System.Drawing.Size(230, 510)
        Me.BsInfoXPSViewer.TabIndex = 37
        Me.BsInfoXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoXPSViewer.VerticalPageMargin = 10
        Me.BsInfoXPSViewer.Visible = False
        Me.BsInfoXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoXPSViewer.WholePageButtonVisible = True
        '
        'BsInfoExpandButton
        '
        Me.BsInfoExpandButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsInfoExpandButton.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsInfoExpandButton.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BsInfoExpandButton.Location = New System.Drawing.Point(210, 0)
        Me.BsInfoExpandButton.Name = "BsInfoExpandButton"
        Me.BsInfoExpandButton.Size = New System.Drawing.Size(20, 20)
        Me.BsInfoExpandButton.TabIndex = 33
        Me.BsInfoExpandButton.Visible = False
        '
        'BsFrequencyLayoutPanel
        '
        Me.BsFrequencyLayoutPanel.ColumnCount = 4
        Me.BsFrequencyLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190.0!))
        Me.BsFrequencyLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190.0!))
        Me.BsFrequencyLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190.0!))
        Me.BsFrequencyLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80.0!))
        Me.BsFrequencyLayoutPanel.Controls.Add(Me.BsFreqReagent2ValueLabel, 2, 1)
        Me.BsFrequencyLayoutPanel.Controls.Add(Me.BsFreqReagent1ValueLabel, 1, 1)
        Me.BsFrequencyLayoutPanel.Controls.Add(Me.BsFreqSampleValueLabel, 0, 1)
        Me.BsFrequencyLayoutPanel.Controls.Add(Me.BsFreqReagent2Label, 2, 0)
        Me.BsFrequencyLayoutPanel.Controls.Add(Me.BsFreqButtonPanel, 3, 1)
        Me.BsFrequencyLayoutPanel.Controls.Add(Me.BsFreqReagent1Label, 1, 0)
        Me.BsFrequencyLayoutPanel.Controls.Add(Me.BsFreqSampleLabel, 0, 0)
        Me.BsFrequencyLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsFrequencyLayoutPanel.Location = New System.Drawing.Point(3, 17)
        Me.BsFrequencyLayoutPanel.Name = "BsFrequencyLayoutPanel"
        Me.BsFrequencyLayoutPanel.RowCount = 2
        Me.BsFrequencyLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.BsFrequencyLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.BsFrequencyLayoutPanel.Size = New System.Drawing.Size(688, 94)
        Me.BsFrequencyLayoutPanel.TabIndex = 56
        '
        'BsFreqReagent2ValueLabel
        '
        Me.BsFreqReagent2ValueLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsFreqReagent2ValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsFreqReagent2ValueLabel.Font = New System.Drawing.Font("Digiface", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFreqReagent2ValueLabel.ForeColor = System.Drawing.Color.LimeGreen
        Me.BsFreqReagent2ValueLabel.Location = New System.Drawing.Point(420, 35)
        Me.BsFreqReagent2ValueLabel.Margin = New System.Windows.Forms.Padding(40, 5, 40, 40)
        Me.BsFreqReagent2ValueLabel.Name = "BsFreqReagent2ValueLabel"
        Me.BsFreqReagent2ValueLabel.Size = New System.Drawing.Size(110, 34)
        Me.BsFreqReagent2ValueLabel.TabIndex = 76
        Me.BsFreqReagent2ValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BsFreqReagent1ValueLabel
        '
        Me.BsFreqReagent1ValueLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsFreqReagent1ValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsFreqReagent1ValueLabel.Font = New System.Drawing.Font("Digiface", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFreqReagent1ValueLabel.ForeColor = System.Drawing.Color.LimeGreen
        Me.BsFreqReagent1ValueLabel.Location = New System.Drawing.Point(230, 35)
        Me.BsFreqReagent1ValueLabel.Margin = New System.Windows.Forms.Padding(40, 5, 40, 40)
        Me.BsFreqReagent1ValueLabel.Name = "BsFreqReagent1ValueLabel"
        Me.BsFreqReagent1ValueLabel.Size = New System.Drawing.Size(110, 34)
        Me.BsFreqReagent1ValueLabel.TabIndex = 76
        Me.BsFreqReagent1ValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BsFreqSampleValueLabel
        '
        Me.BsFreqSampleValueLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsFreqSampleValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsFreqSampleValueLabel.Font = New System.Drawing.Font("Digiface", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFreqSampleValueLabel.ForeColor = System.Drawing.Color.LimeGreen
        Me.BsFreqSampleValueLabel.Location = New System.Drawing.Point(40, 35)
        Me.BsFreqSampleValueLabel.Margin = New System.Windows.Forms.Padding(40, 5, 40, 40)
        Me.BsFreqSampleValueLabel.Name = "BsFreqSampleValueLabel"
        Me.BsFreqSampleValueLabel.Size = New System.Drawing.Size(110, 34)
        Me.BsFreqSampleValueLabel.TabIndex = 75
        Me.BsFreqSampleValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BsFreqReagent2Label
        '
        Me.BsFreqReagent2Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsFreqReagent2Label.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFreqReagent2Label.Location = New System.Drawing.Point(383, 3)
        Me.BsFreqReagent2Label.Margin = New System.Windows.Forms.Padding(3)
        Me.BsFreqReagent2Label.Name = "BsFreqReagent2Label"
        Me.BsFreqReagent2Label.Size = New System.Drawing.Size(184, 24)
        Me.BsFreqReagent2Label.TabIndex = 62
        Me.BsFreqReagent2Label.Text = "Reagent 2"
        Me.BsFreqReagent2Label.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'BsFreqButtonPanel
        '
        Me.BsFreqButtonPanel.Controls.Add(Me.BsFrequencyButton)
        Me.BsFreqButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsFreqButtonPanel.Location = New System.Drawing.Point(573, 33)
        Me.BsFreqButtonPanel.Name = "BsFreqButtonPanel"
        Me.BsFreqButtonPanel.Size = New System.Drawing.Size(112, 76)
        Me.BsFreqButtonPanel.TabIndex = 63
        '
        'BsFrequencyButton
        '
        Me.BsFrequencyButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsFrequencyButton.Enabled = False
        Me.BsFrequencyButton.Location = New System.Drawing.Point(22, 7)
        Me.BsFrequencyButton.Name = "BsFrequencyButton"
        Me.BsFrequencyButton.Size = New System.Drawing.Size(32, 32)
        Me.BsFrequencyButton.TabIndex = 55
        Me.BsFrequencyButton.UseVisualStyleBackColor = True
        '
        'BsFreqReagent1Label
        '
        Me.BsFreqReagent1Label.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsFreqReagent1Label.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFreqReagent1Label.Location = New System.Drawing.Point(193, 3)
        Me.BsFreqReagent1Label.Margin = New System.Windows.Forms.Padding(3)
        Me.BsFreqReagent1Label.Name = "BsFreqReagent1Label"
        Me.BsFreqReagent1Label.Size = New System.Drawing.Size(184, 24)
        Me.BsFreqReagent1Label.TabIndex = 61
        Me.BsFreqReagent1Label.Text = "Reagent 1"
        Me.BsFreqReagent1Label.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'BsFreqSampleLabel
        '
        Me.BsFreqSampleLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsFreqSampleLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFreqSampleLabel.Location = New System.Drawing.Point(3, 3)
        Me.BsFreqSampleLabel.Margin = New System.Windows.Forms.Padding(3)
        Me.BsFreqSampleLabel.Name = "BsFreqSampleLabel"
        Me.BsFreqSampleLabel.Size = New System.Drawing.Size(184, 24)
        Me.BsFreqSampleLabel.TabIndex = 60
        Me.BsFreqSampleLabel.Text = "Sample"
        Me.BsFreqSampleLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'BsDetectionLayoutPanel
        '
        Me.BsDetectionLayoutPanel.ColumnCount = 4
        Me.BsDetectionLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190.0!))
        Me.BsDetectionLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190.0!))
        Me.BsDetectionLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190.0!))
        Me.BsDetectionLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80.0!))
        Me.BsDetectionLayoutPanel.Controls.Add(Me.BsDetectedLabel, 2, 0)
        Me.BsDetectionLayoutPanel.Controls.Add(Me.Panel1, 3, 1)
        Me.BsDetectionLayoutPanel.Controls.Add(Me.BsDetectionPosLabel, 1, 0)
        Me.BsDetectionLayoutPanel.Controls.Add(Me.Panel2, 1, 1)
        Me.BsDetectionLayoutPanel.Controls.Add(Me.BsDetectionArmLabel, 0, 0)
        Me.BsDetectionLayoutPanel.Controls.Add(Me.Panel3, 2, 1)
        Me.BsDetectionLayoutPanel.Controls.Add(Me.Panel4, 0, 1)
        Me.BsDetectionLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsDetectionLayoutPanel.Location = New System.Drawing.Point(3, 17)
        Me.BsDetectionLayoutPanel.Name = "BsDetectionLayoutPanel"
        Me.BsDetectionLayoutPanel.RowCount = 2
        Me.BsDetectionLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.BsDetectionLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.BsDetectionLayoutPanel.Size = New System.Drawing.Size(688, 94)
        Me.BsDetectionLayoutPanel.TabIndex = 57
        '
        'BsDetectedLabel
        '
        Me.BsDetectedLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsDetectedLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsDetectedLabel.Location = New System.Drawing.Point(383, 3)
        Me.BsDetectedLabel.Margin = New System.Windows.Forms.Padding(3)
        Me.BsDetectedLabel.Name = "BsDetectedLabel"
        Me.BsDetectedLabel.Size = New System.Drawing.Size(184, 24)
        Me.BsDetectedLabel.TabIndex = 62
        Me.BsDetectedLabel.Text = "Detection"
        Me.BsDetectedLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.BsDetectionButton)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(573, 33)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(112, 64)
        Me.Panel1.TabIndex = 63
        '
        'BsDetectionButton
        '
        Me.BsDetectionButton.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsDetectionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsDetectionButton.Enabled = False
        Me.BsDetectionButton.Location = New System.Drawing.Point(22, 4)
        Me.BsDetectionButton.Name = "BsDetectionButton"
        Me.BsDetectionButton.Size = New System.Drawing.Size(32, 32)
        Me.BsDetectionButton.TabIndex = 54
        Me.BsDetectionButton.UseVisualStyleBackColor = True
        '
        'BsDetectionPosLabel
        '
        Me.BsDetectionPosLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsDetectionPosLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsDetectionPosLabel.Location = New System.Drawing.Point(193, 3)
        Me.BsDetectionPosLabel.Margin = New System.Windows.Forms.Padding(3)
        Me.BsDetectionPosLabel.Name = "BsDetectionPosLabel"
        Me.BsDetectionPosLabel.Size = New System.Drawing.Size(184, 24)
        Me.BsDetectionPosLabel.TabIndex = 61
        Me.BsDetectionPosLabel.Text = "Position"
        Me.BsDetectionPosLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.BsDetectionPosUpDown)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(193, 33)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(184, 64)
        Me.Panel2.TabIndex = 61
        '
        'BsDetectionPosUpDown
        '
        Me.BsDetectionPosUpDown.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsDetectionPosUpDown.ForeColor = System.Drawing.Color.Black
        Me.BsDetectionPosUpDown.Location = New System.Drawing.Point(43, 9)
        Me.BsDetectionPosUpDown.Maximum = New Decimal(New Integer() {120, 0, 0, 0})
        Me.BsDetectionPosUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.BsDetectionPosUpDown.Name = "BsDetectionPosUpDown"
        Me.BsDetectionPosUpDown.Size = New System.Drawing.Size(99, 21)
        Me.BsDetectionPosUpDown.TabIndex = 58
        Me.BsDetectionPosUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsDetectionPosUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsDetectionArmLabel
        '
        Me.BsDetectionArmLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsDetectionArmLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsDetectionArmLabel.Location = New System.Drawing.Point(3, 3)
        Me.BsDetectionArmLabel.Margin = New System.Windows.Forms.Padding(3)
        Me.BsDetectionArmLabel.Name = "BsDetectionArmLabel"
        Me.BsDetectionArmLabel.Size = New System.Drawing.Size(184, 24)
        Me.BsDetectionArmLabel.TabIndex = 60
        Me.BsDetectionArmLabel.Text = "Arm"
        Me.BsDetectionArmLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.BsDetectionMonitorLED)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(383, 33)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(184, 64)
        Me.Panel3.TabIndex = 62
        '
        'BsDetectionMonitorLED
        '
        Me.BsDetectionMonitorLED.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsDetectionMonitorLED.BackColor = System.Drawing.Color.Transparent
        Me.BsDetectionMonitorLED.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status._ON
        Me.BsDetectionMonitorLED.LightColor = CType(resources.GetObject("BsDetectionMonitorLED.LightColor"), System.Collections.Generic.List(Of System.Drawing.Color))
        Me.BsDetectionMonitorLED.Location = New System.Drawing.Point(75, 4)
        Me.BsDetectionMonitorLED.Margin = New System.Windows.Forms.Padding(0)
        Me.BsDetectionMonitorLED.MaxLimit = 100
        Me.BsDetectionMonitorLED.MinLimit = 0
        Me.BsDetectionMonitorLED.Name = "BsDetectionMonitorLED"
        Me.BsDetectionMonitorLED.Size = New System.Drawing.Size(35, 35)
        Me.BsDetectionMonitorLED.TabIndex = 75
        Me.BsDetectionMonitorLED.TitleAlignment = System.Drawing.ContentAlignment.MiddleCenter
        Me.BsDetectionMonitorLED.TitleFont = Nothing
        Me.BsDetectionMonitorLED.TitleForeColor = System.Drawing.Color.Black
        Me.BsDetectionMonitorLED.TitleHeight = 0
        Me.BsDetectionMonitorLED.TitleText = ""
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.BsDetectionArmComboBox)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(3, 33)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(184, 64)
        Me.Panel4.TabIndex = 60
        '
        'BsDetectionArmComboBox
        '
        Me.BsDetectionArmComboBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsDetectionArmComboBox.FormattingEnabled = True
        Me.BsDetectionArmComboBox.Items.AddRange(New Object() {"SAMPLE", "REAGENT 1", "REAGENT 2"})
        Me.BsDetectionArmComboBox.Location = New System.Drawing.Point(26, 9)
        Me.BsDetectionArmComboBox.Name = "BsDetectionArmComboBox"
        Me.BsDetectionArmComboBox.Size = New System.Drawing.Size(129, 21)
        Me.BsDetectionArmComboBox.TabIndex = 0
        '
        'BsTestTitleLabel
        '
        Me.BsTestTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsTestTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsTestTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTestTitleLabel.Location = New System.Drawing.Point(0, 0)
        Me.BsTestTitleLabel.Name = "BsTestTitleLabel"
        Me.BsTestTitleLabel.Size = New System.Drawing.Size(739, 20)
        Me.BsTestTitleLabel.TabIndex = 59
        Me.BsTestTitleLabel.Text = "Level Detection Test"
        Me.BsTestTitleLabel.Title = True
        '
        'BsLevelDetectionTestPanel
        '
        Me.BsLevelDetectionTestPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsLevelDetectionTestPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsLevelDetectionTestPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsLevelDetectionTestPanel.Controls.Add(Me.BsDetectionGroupBox)
        Me.BsLevelDetectionTestPanel.Controls.Add(Me.BsFrequencyReadGroupBox)
        Me.BsLevelDetectionTestPanel.Controls.Add(Me.BsTestTitleLabel)
        Me.BsLevelDetectionTestPanel.Location = New System.Drawing.Point(234, 25)
        Me.BsLevelDetectionTestPanel.Name = "BsLevelDetectionTestPanel"
        Me.BsLevelDetectionTestPanel.Size = New System.Drawing.Size(740, 532)
        Me.BsLevelDetectionTestPanel.TabIndex = 32
        '
        'BsDetectionGroupBox
        '
        Me.BsDetectionGroupBox.Controls.Add(Me.BsDetectionLayoutPanel)
        Me.BsDetectionGroupBox.Location = New System.Drawing.Point(20, 223)
        Me.BsDetectionGroupBox.Name = "BsDetectionGroupBox"
        Me.BsDetectionGroupBox.Size = New System.Drawing.Size(694, 114)
        Me.BsDetectionGroupBox.TabIndex = 74
        Me.BsDetectionGroupBox.TabStop = False
        Me.BsDetectionGroupBox.Text = "2. Detection Test"
        '
        'BsFrequencyReadGroupBox
        '
        Me.BsFrequencyReadGroupBox.Controls.Add(Me.BsFrequencyLayoutPanel)
        Me.BsFrequencyReadGroupBox.Location = New System.Drawing.Point(20, 50)
        Me.BsFrequencyReadGroupBox.Name = "BsFrequencyReadGroupBox"
        Me.BsFrequencyReadGroupBox.Size = New System.Drawing.Size(694, 114)
        Me.BsFrequencyReadGroupBox.TabIndex = 73
        Me.BsFrequencyReadGroupBox.TabStop = False
        Me.BsFrequencyReadGroupBox.Text = "1. Frequency Read (MHz)"
        '
        'ILevelDetectionTest
        '
        Me.AcceptButton = Me.BsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 593)
        Me.Controls.Add(Me.BsTitleLabel)
        Me.Controls.Add(Me.BsLevelDetectionInfoPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsLevelDetectionTestPanel)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "ILevelDetectionTest"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "StressModeTest"
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsButtonsPanel.ResumeLayout(False)
        Me.BsLevelDetectionInfoPanel.ResumeLayout(False)
        Me.BsFrequencyLayoutPanel.ResumeLayout(False)
        Me.BsFreqButtonPanel.ResumeLayout(False)
        Me.BsDetectionLayoutPanel.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        CType(Me.BsDetectionPosUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel3.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.BsLevelDetectionTestPanel.ResumeLayout(False)
        Me.BsDetectionGroupBox.ResumeLayout(False)
        Me.BsFrequencyReadGroupBox.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTitleLabel As System.Windows.Forms.Label
    Friend WithEvents BsLevelDetectionInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLevelDetectionInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsInfoExpandButton As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsInfoXPSViewer As BsXPSViewer
    Friend WithEvents BsFrequencyLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents BsFreqButtonPanel As System.Windows.Forms.Panel
    Friend WithEvents BsFrequencyButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsDetectionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTestTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLevelDetectionTestPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsFreqReagent1Label As System.Windows.Forms.Label
    Friend WithEvents BsFreqSampleLabel As System.Windows.Forms.Label
    Friend WithEvents BsFreqReagent2Label As System.Windows.Forms.Label
    Friend WithEvents BsDetectionLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents BsDetectedLabel As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents BsDetectionPosLabel As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents BsDetectionArmLabel As System.Windows.Forms.Label
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents BsDetectionGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsFrequencyReadGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsDetectionArmComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents BsDetectionPosUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsDetectionMonitorLED As Biosystems.Ax00.Controls.UserControls.BSMonitorLED
    Friend WithEvents BsFreqSampleValueLabel As System.Windows.Forms.Label
    Friend WithEvents BsFreqReagent2ValueLabel As System.Windows.Forms.Label
    Friend WithEvents BsFreqReagent1ValueLabel As System.Windows.Forms.Label
End Class
