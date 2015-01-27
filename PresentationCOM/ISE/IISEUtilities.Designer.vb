<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiISEUtilities
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
        Me.components = New System.ComponentModel.Container()
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsCancelButtonTODELETE = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsSaveButtonTODELETE = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsAdjustButtonTODELETE = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.FeaturesGroupBox = New System.Windows.Forms.GroupBox()
        Me.BsVolumeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsVolumeUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.BsPositionUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.BsPositionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsTimesUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.BsTimesLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BSActionsTreeImageList = New System.Windows.Forms.ImageList(Me.components)
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.SaveAsDialog = New System.Windows.Forms.SaveFileDialog()
        Me.bsISEInformationGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsISEInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.BsInfoXPSViewer = New BsXPSViewer()
        Me.BsISEInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsISEGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.LineShape3 = New System.Windows.Forms.Panel()
        Me.LineShape2 = New System.Windows.Forms.Panel()
        Me.LineShape1 = New System.Windows.Forms.Panel()
        Me.BsLabelResults = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsClearButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsSaveAsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsRichTextBox1 = New Biosystems.Ax00.Controls.UserControls.BSRichTextBox()
        Me.BsLegendWarningLabel = New System.Windows.Forms.Label()
        Me.BsLegendResponseLabel = New System.Windows.Forms.Label()
        Me.BsLegendCommandLabel = New System.Windows.Forms.Label()
        Me.BsStopButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsAdjustButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsActionsTreeView = New Biosystems.Ax00.Controls.UserControls.BSTreeView()
        Me.BsSubtitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsButtonsPanel.SuspendLayout()
        Me.FeaturesGroupBox.SuspendLayout()
        CType(Me.BsVolumeUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BsPositionUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BsTimesUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsISEInformationGroupBox.SuspendLayout()
        Me.BsISEInfoPanel.SuspendLayout()
        Me.BsISEGroupBox.SuspendLayout()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsMessagesPanel
        '
        Me.BsMessagesPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessagesPanel.BackColor = System.Drawing.Color.Transparent
        Me.BsMessagesPanel.Controls.Add(Me.ProgressBar1)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageImage)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageLabel)
        Me.BsMessagesPanel.Location = New System.Drawing.Point(4, 554)
        Me.BsMessagesPanel.Name = "BsMessagesPanel"
        Me.BsMessagesPanel.Size = New System.Drawing.Size(807, 35)
        Me.BsMessagesPanel.TabIndex = 13
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar1.Location = New System.Drawing.Point(496, 10)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(180, 18)
        Me.ProgressBar1.TabIndex = 5
        Me.ProgressBar1.Visible = False
        '
        'BsMessageImage
        '
        Me.BsMessageImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
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
        Me.BsButtonsPanel.BackColor = System.Drawing.Color.Transparent
        Me.BsButtonsPanel.Controls.Add(Me.BsExitButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsCancelButtonTODELETE)
        Me.BsButtonsPanel.Controls.Add(Me.BsSaveButtonTODELETE)
        Me.BsButtonsPanel.Controls.Add(Me.BsAdjustButtonTODELETE)
        Me.BsButtonsPanel.Location = New System.Drawing.Point(810, 554)
        Me.BsButtonsPanel.Name = "BsButtonsPanel"
        Me.BsButtonsPanel.Size = New System.Drawing.Size(164, 35)
        Me.BsButtonsPanel.TabIndex = 14
        '
        'BsExitButton
        '
        Me.BsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsExitButton.Location = New System.Drawing.Point(125, 1)
        Me.BsExitButton.Name = "BsExitButton"
        Me.BsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.BsExitButton.TabIndex = 3
        Me.BsExitButton.UseVisualStyleBackColor = True
        '
        'BsCancelButtonTODELETE
        '
        Me.BsCancelButtonTODELETE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCancelButtonTODELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsCancelButtonTODELETE.Enabled = False
        Me.BsCancelButtonTODELETE.Location = New System.Drawing.Point(90, 1)
        Me.BsCancelButtonTODELETE.Name = "BsCancelButtonTODELETE"
        Me.BsCancelButtonTODELETE.Size = New System.Drawing.Size(32, 32)
        Me.BsCancelButtonTODELETE.TabIndex = 2
        Me.BsCancelButtonTODELETE.UseVisualStyleBackColor = True
        Me.BsCancelButtonTODELETE.Visible = False
        '
        'BsSaveButtonTODELETE
        '
        Me.BsSaveButtonTODELETE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsSaveButtonTODELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsSaveButtonTODELETE.Enabled = False
        Me.BsSaveButtonTODELETE.Location = New System.Drawing.Point(54, 1)
        Me.BsSaveButtonTODELETE.Name = "BsSaveButtonTODELETE"
        Me.BsSaveButtonTODELETE.Size = New System.Drawing.Size(32, 32)
        Me.BsSaveButtonTODELETE.TabIndex = 1
        Me.BsSaveButtonTODELETE.UseVisualStyleBackColor = True
        Me.BsSaveButtonTODELETE.Visible = False
        '
        'BsAdjustButtonTODELETE
        '
        Me.BsAdjustButtonTODELETE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustButtonTODELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjustButtonTODELETE.Enabled = False
        Me.BsAdjustButtonTODELETE.Location = New System.Drawing.Point(18, 1)
        Me.BsAdjustButtonTODELETE.Name = "BsAdjustButtonTODELETE"
        Me.BsAdjustButtonTODELETE.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjustButtonTODELETE.TabIndex = 0
        Me.BsAdjustButtonTODELETE.UseVisualStyleBackColor = True
        Me.BsAdjustButtonTODELETE.Visible = False
        '
        'FeaturesGroupBox
        '
        Me.FeaturesGroupBox.Controls.Add(Me.BsVolumeLabel)
        Me.FeaturesGroupBox.Controls.Add(Me.BsVolumeUpDown)
        Me.FeaturesGroupBox.Controls.Add(Me.BsPositionUpDown)
        Me.FeaturesGroupBox.Controls.Add(Me.BsPositionLabel)
        Me.FeaturesGroupBox.Controls.Add(Me.BsTimesUpDown)
        Me.FeaturesGroupBox.Controls.Add(Me.BsTimesLabel)
        Me.FeaturesGroupBox.Location = New System.Drawing.Point(256, 36)
        Me.FeaturesGroupBox.Name = "FeaturesGroupBox"
        Me.FeaturesGroupBox.Size = New System.Drawing.Size(464, 66)
        Me.FeaturesGroupBox.TabIndex = 84
        Me.FeaturesGroupBox.TabStop = False
        Me.FeaturesGroupBox.Text = "Command Features"
        '
        'BsVolumeLabel
        '
        Me.BsVolumeLabel.AutoSize = True
        Me.BsVolumeLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsVolumeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsVolumeLabel.ForeColor = System.Drawing.Color.Black
        Me.BsVolumeLabel.Location = New System.Drawing.Point(355, 20)
        Me.BsVolumeLabel.Name = "BsVolumeLabel"
        Me.BsVolumeLabel.Size = New System.Drawing.Size(50, 13)
        Me.BsVolumeLabel.TabIndex = 66
        Me.BsVolumeLabel.Text = "Volume"
        Me.BsVolumeLabel.Title = False
        '
        'BsVolumeUpDown
        '
        Me.BsVolumeUpDown.Enabled = False
        Me.BsVolumeUpDown.ForeColor = System.Drawing.Color.Black
        Me.BsVolumeUpDown.Location = New System.Drawing.Point(358, 36)
        Me.BsVolumeUpDown.Maximum = New Decimal(New Integer() {120, 0, 0, 0})
        Me.BsVolumeUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.BsVolumeUpDown.Name = "BsVolumeUpDown"
        Me.BsVolumeUpDown.Size = New System.Drawing.Size(52, 21)
        Me.BsVolumeUpDown.TabIndex = 65
        Me.BsVolumeUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsVolumeUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsPositionUpDown
        '
        Me.BsPositionUpDown.Enabled = False
        Me.BsPositionUpDown.ForeColor = System.Drawing.Color.Black
        Me.BsPositionUpDown.Location = New System.Drawing.Point(189, 36)
        Me.BsPositionUpDown.Maximum = New Decimal(New Integer() {135, 0, 0, 0})
        Me.BsPositionUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.BsPositionUpDown.Name = "BsPositionUpDown"
        Me.BsPositionUpDown.Size = New System.Drawing.Size(52, 21)
        Me.BsPositionUpDown.TabIndex = 63
        Me.BsPositionUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsPositionUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsPositionLabel
        '
        Me.BsPositionLabel.AutoSize = True
        Me.BsPositionLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsPositionLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsPositionLabel.ForeColor = System.Drawing.Color.Black
        Me.BsPositionLabel.Location = New System.Drawing.Point(186, 20)
        Me.BsPositionLabel.Name = "BsPositionLabel"
        Me.BsPositionLabel.Size = New System.Drawing.Size(130, 13)
        Me.BsPositionLabel.TabIndex = 64
        Me.BsPositionLabel.Text = "Sample rotor position"
        Me.BsPositionLabel.Title = False
        '
        'BsTimesUpDown
        '
        Me.BsTimesUpDown.Enabled = False
        Me.BsTimesUpDown.ForeColor = System.Drawing.Color.Black
        Me.BsTimesUpDown.Location = New System.Drawing.Point(17, 36)
        Me.BsTimesUpDown.Maximum = New Decimal(New Integer() {120, 0, 0, 0})
        Me.BsTimesUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.BsTimesUpDown.Name = "BsTimesUpDown"
        Me.BsTimesUpDown.Size = New System.Drawing.Size(52, 21)
        Me.BsTimesUpDown.TabIndex = 59
        Me.BsTimesUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsTimesUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsTimesLabel
        '
        Me.BsTimesLabel.AutoSize = True
        Me.BsTimesLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsTimesLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsTimesLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTimesLabel.Location = New System.Drawing.Point(14, 20)
        Me.BsTimesLabel.Name = "BsTimesLabel"
        Me.BsTimesLabel.Size = New System.Drawing.Size(41, 13)
        Me.BsTimesLabel.TabIndex = 60
        Me.BsTimesLabel.Text = "Times"
        Me.BsTimesLabel.Title = False
        '
        'BSActionsTreeImageList
        '
        Me.BSActionsTreeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.BSActionsTreeImageList.ImageSize = New System.Drawing.Size(16, 16)
        Me.BSActionsTreeImageList.TransparentColor = System.Drawing.Color.Transparent
        '
        'bsISEInformationGroupBox
        '
        Me.bsISEInformationGroupBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bsISEInformationGroupBox.Controls.Add(Me.BsISEInfoPanel)
        Me.bsISEInformationGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsISEInformationGroupBox.Location = New System.Drawing.Point(4, 3)
        Me.bsISEInformationGroupBox.Name = "bsISEInformationGroupBox"
        Me.bsISEInformationGroupBox.Size = New System.Drawing.Size(243, 549)
        Me.bsISEInformationGroupBox.TabIndex = 71
        Me.bsISEInformationGroupBox.TabStop = False
        '
        'BsISEInfoPanel
        '
        Me.BsISEInfoPanel.BackColor = System.Drawing.Color.White
        Me.BsISEInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsISEInfoPanel.Controls.Add(Me.BsInfoXPSViewer)
        Me.BsISEInfoPanel.Controls.Add(Me.BsISEInfoTitle)
        Me.BsISEInfoPanel.Location = New System.Drawing.Point(5, 8)
        Me.BsISEInfoPanel.Name = "BsISEInfoPanel"
        Me.BsISEInfoPanel.Size = New System.Drawing.Size(232, 533)
        Me.BsISEInfoPanel.TabIndex = 26
        '
        'BsInfoXPSViewer
        '
        Me.BsInfoXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoXPSViewer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
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
        Me.BsInfoXPSViewer.TabIndex = 34
        Me.BsInfoXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoXPSViewer.VerticalPageMargin = 0
        Me.BsInfoXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoXPSViewer.WholePageButtonVisible = True
        '
        'BsISEInfoTitle
        '
        Me.BsISEInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsISEInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsISEInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsISEInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsISEInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsISEInfoTitle.Name = "BsISEInfoTitle"
        Me.BsISEInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsISEInfoTitle.TabIndex = 22
        Me.BsISEInfoTitle.Text = "Information"
        Me.BsISEInfoTitle.Title = True
        '
        'BsISEGroupBox
        '
        Me.BsISEGroupBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsISEGroupBox.Controls.Add(Me.LineShape3)
        Me.BsISEGroupBox.Controls.Add(Me.LineShape2)
        Me.BsISEGroupBox.Controls.Add(Me.LineShape1)
        Me.BsISEGroupBox.Controls.Add(Me.BsLabelResults)
        Me.BsISEGroupBox.Controls.Add(Me.BsClearButton)
        Me.BsISEGroupBox.Controls.Add(Me.BsSaveAsButton)
        Me.BsISEGroupBox.Controls.Add(Me.BsRichTextBox1)
        Me.BsISEGroupBox.Controls.Add(Me.BsLegendWarningLabel)
        Me.BsISEGroupBox.Controls.Add(Me.BsLegendResponseLabel)
        Me.BsISEGroupBox.Controls.Add(Me.BsLegendCommandLabel)
        Me.BsISEGroupBox.Controls.Add(Me.BsStopButton)
        Me.BsISEGroupBox.Controls.Add(Me.BsAdjustButton)
        Me.BsISEGroupBox.Controls.Add(Me.BsActionsTreeView)
        Me.BsISEGroupBox.Controls.Add(Me.FeaturesGroupBox)
        Me.BsISEGroupBox.Controls.Add(Me.BsSubtitleLabel)
        Me.BsISEGroupBox.ForeColor = System.Drawing.Color.Black
        Me.BsISEGroupBox.Location = New System.Drawing.Point(247, 3)
        Me.BsISEGroupBox.Name = "BsISEGroupBox"
        Me.BsISEGroupBox.Size = New System.Drawing.Size(725, 549)
        Me.BsISEGroupBox.TabIndex = 84
        Me.BsISEGroupBox.TabStop = False
        '
        'LineShape3
        '
        Me.LineShape3.BackColor = System.Drawing.Color.OrangeRed
        Me.LineShape3.Location = New System.Drawing.Point(495, 530)
        Me.LineShape3.Name = "LineShape3"
        Me.LineShape3.Size = New System.Drawing.Size(26, 2)
        Me.LineShape3.TabIndex = 125
        '
        'LineShape2
        '
        Me.LineShape2.BackColor = System.Drawing.Color.DarkOliveGreen
        Me.LineShape2.Location = New System.Drawing.Point(385, 530)
        Me.LineShape2.Name = "LineShape2"
        Me.LineShape2.Size = New System.Drawing.Size(26, 2)
        Me.LineShape2.TabIndex = 124
        '
        'LineShape1
        '
        Me.LineShape1.BackColor = System.Drawing.Color.SteelBlue
        Me.LineShape1.Location = New System.Drawing.Point(268, 530)
        Me.LineShape1.Name = "LineShape1"
        Me.LineShape1.Size = New System.Drawing.Size(26, 2)
        Me.LineShape1.TabIndex = 123
        '
        'BsLabelResults
        '
        Me.BsLabelResults.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsLabelResults.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsLabelResults.ForeColor = System.Drawing.Color.Black
        Me.BsLabelResults.Location = New System.Drawing.Point(256, 114)
        Me.BsLabelResults.Name = "BsLabelResults"
        Me.BsLabelResults.Size = New System.Drawing.Size(463, 20)
        Me.BsLabelResults.TabIndex = 122
        Me.BsLabelResults.Text = "Results"
        Me.BsLabelResults.Title = True
        '
        'BsClearButton
        '
        Me.BsClearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsClearButton.Location = New System.Drawing.Point(653, 514)
        Me.BsClearButton.Name = "BsClearButton"
        Me.BsClearButton.Size = New System.Drawing.Size(32, 32)
        Me.BsClearButton.TabIndex = 120
        Me.BsClearButton.UseVisualStyleBackColor = True
        '
        'BsSaveAsButton
        '
        Me.BsSaveAsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsSaveAsButton.Location = New System.Drawing.Point(689, 514)
        Me.BsSaveAsButton.Name = "BsSaveAsButton"
        Me.BsSaveAsButton.Size = New System.Drawing.Size(32, 32)
        Me.BsSaveAsButton.TabIndex = 121
        Me.BsSaveAsButton.UseVisualStyleBackColor = True
        '
        'BsRichTextBox1
        '
        Me.BsRichTextBox1.BackColor = System.Drawing.SystemColors.Window
        Me.BsRichTextBox1.Location = New System.Drawing.Point(256, 137)
        Me.BsRichTextBox1.Name = "BsRichTextBox1"
        Me.BsRichTextBox1.ReadOnly = True
        Me.BsRichTextBox1.Size = New System.Drawing.Size(463, 369)
        Me.BsRichTextBox1.TabIndex = 114
        Me.BsRichTextBox1.Text = ""
        '
        'BsLegendWarningLabel
        '
        Me.BsLegendWarningLabel.AutoSize = True
        Me.BsLegendWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsLegendWarningLabel.ForeColor = System.Drawing.Color.OrangeRed
        Me.BsLegendWarningLabel.Location = New System.Drawing.Point(527, 524)
        Me.BsLegendWarningLabel.Name = "BsLegendWarningLabel"
        Me.BsLegendWarningLabel.Size = New System.Drawing.Size(60, 13)
        Me.BsLegendWarningLabel.TabIndex = 113
        Me.BsLegendWarningLabel.Text = "Warnings"
        '
        'BsLegendResponseLabel
        '
        Me.BsLegendResponseLabel.AutoSize = True
        Me.BsLegendResponseLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsLegendResponseLabel.ForeColor = System.Drawing.Color.DarkOliveGreen
        Me.BsLegendResponseLabel.Location = New System.Drawing.Point(417, 524)
        Me.BsLegendResponseLabel.Name = "BsLegendResponseLabel"
        Me.BsLegendResponseLabel.Size = New System.Drawing.Size(68, 13)
        Me.BsLegendResponseLabel.TabIndex = 112
        Me.BsLegendResponseLabel.Text = "Responses"
        '
        'BsLegendCommandLabel
        '
        Me.BsLegendCommandLabel.AutoSize = True
        Me.BsLegendCommandLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsLegendCommandLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.BsLegendCommandLabel.Location = New System.Drawing.Point(299, 524)
        Me.BsLegendCommandLabel.Name = "BsLegendCommandLabel"
        Me.BsLegendCommandLabel.Size = New System.Drawing.Size(72, 13)
        Me.BsLegendCommandLabel.TabIndex = 111
        Me.BsLegendCommandLabel.Text = "Commands"
        '
        'BsStopButton
        '
        Me.BsStopButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsStopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsStopButton.Location = New System.Drawing.Point(218, 514)
        Me.BsStopButton.Name = "BsStopButton"
        Me.BsStopButton.Size = New System.Drawing.Size(32, 32)
        Me.BsStopButton.TabIndex = 108
        Me.BsStopButton.UseVisualStyleBackColor = True
        Me.BsStopButton.Visible = False
        '
        'BsAdjustButton
        '
        Me.BsAdjustButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjustButton.Enabled = False
        Me.BsAdjustButton.Location = New System.Drawing.Point(218, 513)
        Me.BsAdjustButton.Name = "BsAdjustButton"
        Me.BsAdjustButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjustButton.TabIndex = 106
        Me.BsAdjustButton.UseVisualStyleBackColor = True
        '
        'BsActionsTreeView
        '
        Me.BsActionsTreeView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BsActionsTreeView.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsActionsTreeView.HideSelection = False
        Me.BsActionsTreeView.ImageIndex = 0
        Me.BsActionsTreeView.ImageList = Me.BSActionsTreeImageList
        Me.BsActionsTreeView.Indent = 16
        Me.BsActionsTreeView.ItemHeight = 24
        Me.BsActionsTreeView.Location = New System.Drawing.Point(5, 39)
        Me.BsActionsTreeView.Name = "BsActionsTreeView"
        Me.BsActionsTreeView.SelectedImageIndex = 0
        Me.BsActionsTreeView.Size = New System.Drawing.Size(244, 468)
        Me.BsActionsTreeView.TabIndex = 107
        '
        'BsSubtitleLabel
        '
        Me.BsSubtitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsSubtitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsSubtitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsSubtitleLabel.Location = New System.Drawing.Point(5, 15)
        Me.BsSubtitleLabel.Name = "BsSubtitleLabel"
        Me.BsSubtitleLabel.Size = New System.Drawing.Size(714, 20)
        Me.BsSubtitleLabel.TabIndex = 25
        Me.BsSubtitleLabel.Text = "ISE Module Utilities"
        Me.BsSubtitleLabel.Title = True
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'IISEUtilities
        '
        Me.AcceptButton = Me.BsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(978, 593)
        Me.Controls.Add(Me.BsISEGroupBox)
        Me.Controls.Add(Me.bsISEInformationGroupBox)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IISEUtilities"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsButtonsPanel.ResumeLayout(False)
        Me.FeaturesGroupBox.ResumeLayout(False)
        Me.FeaturesGroupBox.PerformLayout()
        CType(Me.BsVolumeUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BsPositionUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BsTimesUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsISEInformationGroupBox.ResumeLayout(False)
        Me.BsISEInfoPanel.ResumeLayout(False)
        Me.BsISEGroupBox.ResumeLayout(False)
        Me.BsISEGroupBox.PerformLayout()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsCancelButtonTODELETE As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsSaveButtonTODELETE As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjustButtonTODELETE As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BSActionsTreeImageList As System.Windows.Forms.ImageList
    Friend WithEvents FeaturesGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsPositionUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsPositionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTimesUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsTimesLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsVolumeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsVolumeUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents SaveAsDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents bsISEInformationGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsISEGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsSubtitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents BsStopButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjustButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsActionsTreeView As Biosystems.Ax00.Controls.UserControls.BSTreeView
    Friend WithEvents BsLabelResults As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsClearButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsSaveAsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsRichTextBox1 As Biosystems.Ax00.Controls.UserControls.BSRichTextBox
    Friend WithEvents BsLegendWarningLabel As System.Windows.Forms.Label
    Friend WithEvents BsLegendResponseLabel As System.Windows.Forms.Label
    Friend WithEvents BsLegendCommandLabel As System.Windows.Forms.Label
    Friend WithEvents BsISEInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsInfoXPSViewer As BsXPSViewer
    Friend WithEvents BsISEInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LineShape3 As System.Windows.Forms.Panel
    Friend WithEvents LineShape2 As System.Windows.Forms.Panel
    Friend WithEvents LineShape1 As System.Windows.Forms.Panel
End Class
