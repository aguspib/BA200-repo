Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Controls.UserControls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IInstrumentUpdateUtil
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
        Me.BsResponse = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.BsPanel1 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsLabel4 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsLabel5 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.PrintDocument1 = New System.Drawing.Printing.PrintDocument
        Me.PrintDialog1 = New System.Windows.Forms.PrintDialog
        Me.OpenAdjFileDialog = New System.Windows.Forms.OpenFileDialog
        Me.SaveAdjFileDialog = New System.Windows.Forms.SaveFileDialog
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTabPagesControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl
        Me.BsAdjustmentsTab = New System.Windows.Forms.TabPage
        Me.BsAdjInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoAdjXPSViewer = New BsXPSViewer
        Me.BsAdjInfoTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsAdjustmentsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsAdjNewButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjUndoButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjEditButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustmentsTextPanel = New System.Windows.Forms.Panel
        Me.BSAdjHeaderRTextBox = New Biosystems.Ax00.Controls.UserControls.BSRichTextBox
        Me.BsAdjustmentsRTextBox = New Biosystems.Ax00.Controls.UserControls.BSRichTextBox
        Me.BsAdjSaveAsButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjOpenFileButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjRestoreFactoryButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjRestoreButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjBackupButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustmentsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsFirmwareTab = New System.Windows.Forms.TabPage
        Me.BsFwInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoFwXPSViewer = New BsXPSViewer
        Me.BsFirmwareInfoTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsFirmwarePanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsUpdateFirmwareGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.BsFwOpenFileCaptionLabel = New System.Windows.Forms.Label
        Me.BsFileNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.BsFwUpdateCaptionLabel = New System.Windows.Forms.Label
        Me.BsFwNewVersionLabel = New System.Windows.Forms.Label
        Me.BsFwUpdateButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsFwOpenFileButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsFwNewVersionCaptionLabel = New System.Windows.Forms.Label
        Me.BsFwOpenFilePathLabel = New System.Windows.Forms.Label
        Me.BsFwCurrentVersionLabel = New System.Windows.Forms.Label
        Me.BsFwCurrentVersionCaptionLabel = New System.Windows.Forms.Label
        Me.BsFwTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsIntermediateAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsWaitProgressBar = New System.Windows.Forms.ProgressBar
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.OpenFwFileDialog = New System.Windows.Forms.OpenFileDialog
        Me.BsTabPagesControl.SuspendLayout()
        Me.BsAdjustmentsTab.SuspendLayout()
        Me.BsAdjInfoPanel.SuspendLayout()
        Me.BsAdjustmentsPanel.SuspendLayout()
        Me.BsAdjustmentsTextPanel.SuspendLayout()
        Me.BsFirmwareTab.SuspendLayout()
        Me.BsFwInfoPanel.SuspendLayout()
        Me.BsFirmwarePanel.SuspendLayout()
        Me.BsUpdateFirmwareGroupBox.SuspendLayout()
        Me.BsButtonsPanel.SuspendLayout()
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsResponse
        '
        Me.BsResponse.BackColor = System.Drawing.Color.White
        Me.BsResponse.DecimalsValues = False
        Me.BsResponse.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsResponse.IsNumeric = False
        Me.BsResponse.Location = New System.Drawing.Point(52, 57)
        Me.BsResponse.Mandatory = False
        Me.BsResponse.Multiline = True
        Me.BsResponse.Name = "BsResponse"
        Me.BsResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.BsResponse.Size = New System.Drawing.Size(836, 130)
        Me.BsResponse.TabIndex = 43
        '
        'BsPanel1
        '
        Me.BsPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsPanel1.Name = "BsPanel1"
        Me.BsPanel1.Size = New System.Drawing.Size(200, 100)
        Me.BsPanel1.TabIndex = 0
        '
        'BsLabel4
        '
        Me.BsLabel4.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel4.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel4.ForeColor = System.Drawing.Color.Black
        Me.BsLabel4.Location = New System.Drawing.Point(0, 0)
        Me.BsLabel4.Name = "BsLabel4"
        Me.BsLabel4.Size = New System.Drawing.Size(100, 23)
        Me.BsLabel4.TabIndex = 0
        Me.BsLabel4.Title = False
        '
        'BsLabel5
        '
        Me.BsLabel5.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel5.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel5.ForeColor = System.Drawing.Color.Black
        Me.BsLabel5.Location = New System.Drawing.Point(0, 0)
        Me.BsLabel5.Name = "BsLabel5"
        Me.BsLabel5.Size = New System.Drawing.Size(100, 23)
        Me.BsLabel5.TabIndex = 0
        Me.BsLabel5.Title = False
        '
        'PrintDocument1
        '
        '
        'PrintDialog1
        '
        Me.PrintDialog1.UseEXDialog = True
        '
        'OpenAdjFileDialog
        '
        Me.OpenAdjFileDialog.FileName = "OpenFileDialog1"
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
        'BsTabPagesControl
        '
        Me.BsTabPagesControl.Controls.Add(Me.BsAdjustmentsTab)
        Me.BsTabPagesControl.Controls.Add(Me.BsFirmwareTab)
        Me.BsTabPagesControl.Location = New System.Drawing.Point(0, 0)
        Me.BsTabPagesControl.Name = "BsTabPagesControl"
        Me.BsTabPagesControl.SelectedIndex = 0
        Me.BsTabPagesControl.Size = New System.Drawing.Size(978, 558)
        Me.BsTabPagesControl.TabIndex = 14
        '
        'BsAdjustmentsTab
        '
        Me.BsAdjustmentsTab.Controls.Add(Me.BsAdjInfoPanel)
        Me.BsAdjustmentsTab.Controls.Add(Me.BsAdjustmentsPanel)
        Me.BsAdjustmentsTab.Location = New System.Drawing.Point(4, 22)
        Me.BsAdjustmentsTab.Name = "BsAdjustmentsTab"
        Me.BsAdjustmentsTab.Padding = New System.Windows.Forms.Padding(3)
        Me.BsAdjustmentsTab.Size = New System.Drawing.Size(970, 532)
        Me.BsAdjustmentsTab.TabIndex = 0
        Me.BsAdjustmentsTab.Text = "Backup / Restore Adjustments"
        Me.BsAdjustmentsTab.UseVisualStyleBackColor = True
        '
        'BsAdjInfoPanel
        '
        Me.BsAdjInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsAdjInfoPanel.Controls.Add(Me.BsInfoAdjXPSViewer)
        Me.BsAdjInfoPanel.Controls.Add(Me.BsAdjInfoTitleLabel)
        Me.BsAdjInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsAdjInfoPanel.Name = "BsAdjInfoPanel"
        Me.BsAdjInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsAdjInfoPanel.TabIndex = 25
        '
        'BsInfoAdjXPSViewer
        '
        Me.BsInfoAdjXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoAdjXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoAdjXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoAdjXPSViewer.CopyButtonVisible = True
        Me.BsInfoAdjXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoAdjXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoAdjXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoAdjXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoAdjXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoAdjXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoAdjXPSViewer.HorizontalPageMargin = 0
        Me.BsInfoAdjXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoAdjXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoAdjXPSViewer.IsLoaded = False
        Me.BsInfoAdjXPSViewer.IsScrollable = False
        Me.BsInfoAdjXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoAdjXPSViewer.MenuBarVisible = False
        Me.BsInfoAdjXPSViewer.Name = "BsInfoAdjXPSViewer"
        Me.BsInfoAdjXPSViewer.PopupMenuEnabled = True
        Me.BsInfoAdjXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoAdjXPSViewer.PrintButtonVisible = True
        Me.BsInfoAdjXPSViewer.SearchBarVisible = False
        Me.BsInfoAdjXPSViewer.Size = New System.Drawing.Size(230, 510)
        Me.BsInfoAdjXPSViewer.TabIndex = 35
        Me.BsInfoAdjXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoAdjXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoAdjXPSViewer.VerticalPageMargin = 0
        Me.BsInfoAdjXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoAdjXPSViewer.WholePageButtonVisible = True
        '
        'BsAdjInfoTitleLabel
        '
        Me.BsAdjInfoTitleLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjInfoTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsAdjInfoTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsAdjInfoTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsAdjInfoTitleLabel.Location = New System.Drawing.Point(0, 0)
        Me.BsAdjInfoTitleLabel.Name = "BsAdjInfoTitleLabel"
        Me.BsAdjInfoTitleLabel.Size = New System.Drawing.Size(231, 20)
        Me.BsAdjInfoTitleLabel.TabIndex = 22
        Me.BsAdjInfoTitleLabel.Text = "Information"
        Me.BsAdjInfoTitleLabel.Title = True
        '
        'BsAdjustmentsPanel
        '
        Me.BsAdjustmentsPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustmentsPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAdjustmentsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjNewButton)
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjUndoButton)
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjEditButton)
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjustmentsTextPanel)
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjSaveAsButton)
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjOpenFileButton)
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjRestoreFactoryButton)
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjPrintButton)
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjRestoreButton)
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjBackupButton)
        Me.BsAdjustmentsPanel.Controls.Add(Me.BsAdjustmentsTitleLabel)
        Me.BsAdjustmentsPanel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAdjustmentsPanel.ForeColor = System.Drawing.Color.SteelBlue
        Me.BsAdjustmentsPanel.Location = New System.Drawing.Point(231, 0)
        Me.BsAdjustmentsPanel.Name = "BsAdjustmentsPanel"
        Me.BsAdjustmentsPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsAdjustmentsPanel.TabIndex = 21
        '
        'BsAdjNewButton
        '
        Me.BsAdjNewButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjNewButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjNewButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BsAdjNewButton.Location = New System.Drawing.Point(679, 412)
        Me.BsAdjNewButton.Name = "BsAdjNewButton"
        Me.BsAdjNewButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjNewButton.TabIndex = 79
        Me.BsAdjNewButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsAdjNewButton.UseVisualStyleBackColor = True
        '
        'BsAdjUndoButton
        '
        Me.BsAdjUndoButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BsAdjUndoButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjUndoButton.Enabled = False
        Me.BsAdjUndoButton.ForeColor = System.Drawing.Color.Black
        Me.BsAdjUndoButton.Location = New System.Drawing.Point(681, 483)
        Me.BsAdjUndoButton.Name = "BsAdjUndoButton"
        Me.BsAdjUndoButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjUndoButton.TabIndex = 78
        Me.BsAdjUndoButton.UseVisualStyleBackColor = True
        '
        'BsAdjEditButton
        '
        Me.BsAdjEditButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BsAdjEditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjEditButton.Enabled = False
        Me.BsAdjEditButton.ForeColor = System.Drawing.Color.Black
        Me.BsAdjEditButton.Location = New System.Drawing.Point(681, 447)
        Me.BsAdjEditButton.Name = "BsAdjEditButton"
        Me.BsAdjEditButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjEditButton.TabIndex = 77
        Me.BsAdjEditButton.UseVisualStyleBackColor = True
        '
        'BsAdjustmentsTextPanel
        '
        Me.BsAdjustmentsTextPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.BsAdjustmentsTextPanel.Controls.Add(Me.BSAdjHeaderRTextBox)
        Me.BsAdjustmentsTextPanel.Controls.Add(Me.BsAdjustmentsRTextBox)
        Me.BsAdjustmentsTextPanel.Enabled = False
        Me.BsAdjustmentsTextPanel.Location = New System.Drawing.Point(16, 32)
        Me.BsAdjustmentsTextPanel.Name = "BsAdjustmentsTextPanel"
        Me.BsAdjustmentsTextPanel.Size = New System.Drawing.Size(659, 484)
        Me.BsAdjustmentsTextPanel.TabIndex = 76
        '
        'BSAdjHeaderRTextBox
        '
        Me.BSAdjHeaderRTextBox.BackColor = System.Drawing.Color.LightYellow
        Me.BSAdjHeaderRTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.BSAdjHeaderRTextBox.Location = New System.Drawing.Point(0, 0)
        Me.BSAdjHeaderRTextBox.Margin = New System.Windows.Forms.Padding(0)
        Me.BSAdjHeaderRTextBox.Name = "BSAdjHeaderRTextBox"
        Me.BSAdjHeaderRTextBox.ReadOnly = True
        Me.BSAdjHeaderRTextBox.Size = New System.Drawing.Size(655, 60)
        Me.BSAdjHeaderRTextBox.TabIndex = 71
        Me.BSAdjHeaderRTextBox.Text = ""
        '
        'BsAdjustmentsRTextBox
        '
        Me.BsAdjustmentsRTextBox.BackColor = System.Drawing.Color.LightYellow
        Me.BsAdjustmentsRTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.BsAdjustmentsRTextBox.Location = New System.Drawing.Point(0, 60)
        Me.BsAdjustmentsRTextBox.Margin = New System.Windows.Forms.Padding(0)
        Me.BsAdjustmentsRTextBox.Name = "BsAdjustmentsRTextBox"
        Me.BsAdjustmentsRTextBox.Size = New System.Drawing.Size(655, 420)
        Me.BsAdjustmentsRTextBox.TabIndex = 70
        Me.BsAdjustmentsRTextBox.Text = ""
        '
        'BsAdjSaveAsButton
        '
        Me.BsAdjSaveAsButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjSaveAsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjSaveAsButton.Enabled = False
        Me.BsAdjSaveAsButton.Location = New System.Drawing.Point(679, 69)
        Me.BsAdjSaveAsButton.Name = "BsAdjSaveAsButton"
        Me.BsAdjSaveAsButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjSaveAsButton.TabIndex = 75
        Me.BsAdjSaveAsButton.UseVisualStyleBackColor = True
        '
        'BsAdjOpenFileButton
        '
        Me.BsAdjOpenFileButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjOpenFileButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjOpenFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BsAdjOpenFileButton.Location = New System.Drawing.Point(679, 33)
        Me.BsAdjOpenFileButton.Name = "BsAdjOpenFileButton"
        Me.BsAdjOpenFileButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjOpenFileButton.TabIndex = 74
        Me.BsAdjOpenFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsAdjOpenFileButton.UseVisualStyleBackColor = True
        '
        'BsAdjRestoreFactoryButton
        '
        Me.BsAdjRestoreFactoryButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjRestoreFactoryButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjRestoreFactoryButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BsAdjRestoreFactoryButton.Location = New System.Drawing.Point(679, 194)
        Me.BsAdjRestoreFactoryButton.Name = "BsAdjRestoreFactoryButton"
        Me.BsAdjRestoreFactoryButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjRestoreFactoryButton.TabIndex = 71
        Me.BsAdjRestoreFactoryButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsAdjRestoreFactoryButton.UseVisualStyleBackColor = True
        Me.BsAdjRestoreFactoryButton.Visible = False
        '
        'BsAdjPrintButton
        '
        Me.BsAdjPrintButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjPrintButton.Enabled = False
        Me.BsAdjPrintButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BsAdjPrintButton.Location = New System.Drawing.Point(679, 246)
        Me.BsAdjPrintButton.Name = "BsAdjPrintButton"
        Me.BsAdjPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjPrintButton.TabIndex = 69
        Me.BsAdjPrintButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsAdjPrintButton.UseVisualStyleBackColor = True
        '
        'BsAdjRestoreButton
        '
        Me.BsAdjRestoreButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjRestoreButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjRestoreButton.Enabled = False
        Me.BsAdjRestoreButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BsAdjRestoreButton.Location = New System.Drawing.Point(679, 158)
        Me.BsAdjRestoreButton.Name = "BsAdjRestoreButton"
        Me.BsAdjRestoreButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjRestoreButton.TabIndex = 68
        Me.BsAdjRestoreButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsAdjRestoreButton.UseVisualStyleBackColor = True
        '
        'BsAdjBackupButton
        '
        Me.BsAdjBackupButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjBackupButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjBackupButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BsAdjBackupButton.Location = New System.Drawing.Point(679, 122)
        Me.BsAdjBackupButton.Name = "BsAdjBackupButton"
        Me.BsAdjBackupButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjBackupButton.TabIndex = 67
        Me.BsAdjBackupButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsAdjBackupButton.UseVisualStyleBackColor = True
        '
        'BsAdjustmentsTitleLabel
        '
        Me.BsAdjustmentsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsAdjustmentsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsAdjustmentsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsAdjustmentsTitleLabel.Location = New System.Drawing.Point(0, 0)
        Me.BsAdjustmentsTitleLabel.Name = "BsAdjustmentsTitleLabel"
        Me.BsAdjustmentsTitleLabel.Size = New System.Drawing.Size(739, 20)
        Me.BsAdjustmentsTitleLabel.TabIndex = 31
        Me.BsAdjustmentsTitleLabel.Text = "Backup / Restore Adjustments"
        Me.BsAdjustmentsTitleLabel.Title = True
        '
        'BsFirmwareTab
        '
        Me.BsFirmwareTab.Controls.Add(Me.BsFwInfoPanel)
        Me.BsFirmwareTab.Controls.Add(Me.BsFirmwarePanel)
        Me.BsFirmwareTab.Location = New System.Drawing.Point(4, 22)
        Me.BsFirmwareTab.Name = "BsFirmwareTab"
        Me.BsFirmwareTab.Padding = New System.Windows.Forms.Padding(3)
        Me.BsFirmwareTab.Size = New System.Drawing.Size(970, 532)
        Me.BsFirmwareTab.TabIndex = 3
        Me.BsFirmwareTab.Text = "Update Firmware"
        Me.BsFirmwareTab.UseVisualStyleBackColor = True
        '
        'BsFwInfoPanel
        '
        Me.BsFwInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsFwInfoPanel.Controls.Add(Me.BsInfoFwXPSViewer)
        Me.BsFwInfoPanel.Controls.Add(Me.BsFirmwareInfoTitleLabel)
        Me.BsFwInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsFwInfoPanel.Name = "BsFwInfoPanel"
        Me.BsFwInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsFwInfoPanel.TabIndex = 29
        '
        'BsInfoFwXPSViewer
        '
        Me.BsInfoFwXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoFwXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoFwXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoFwXPSViewer.CopyButtonVisible = True
        Me.BsInfoFwXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoFwXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoFwXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoFwXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoFwXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoFwXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoFwXPSViewer.HorizontalPageMargin = 0
        Me.BsInfoFwXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoFwXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoFwXPSViewer.IsLoaded = False
        Me.BsInfoFwXPSViewer.IsScrollable = False
        Me.BsInfoFwXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoFwXPSViewer.MenuBarVisible = False
        Me.BsInfoFwXPSViewer.Name = "BsInfoFwXPSViewer"
        Me.BsInfoFwXPSViewer.PopupMenuEnabled = True
        Me.BsInfoFwXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoFwXPSViewer.PrintButtonVisible = True
        Me.BsInfoFwXPSViewer.SearchBarVisible = False
        Me.BsInfoFwXPSViewer.Size = New System.Drawing.Size(230, 510)
        Me.BsInfoFwXPSViewer.TabIndex = 35
        Me.BsInfoFwXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoFwXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoFwXPSViewer.VerticalPageMargin = 0
        Me.BsInfoFwXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoFwXPSViewer.WholePageButtonVisible = True
        '
        'BsFirmwareInfoTitleLabel
        '
        Me.BsFirmwareInfoTitleLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsFirmwareInfoTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsFirmwareInfoTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsFirmwareInfoTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsFirmwareInfoTitleLabel.Location = New System.Drawing.Point(0, 1)
        Me.BsFirmwareInfoTitleLabel.Name = "BsFirmwareInfoTitleLabel"
        Me.BsFirmwareInfoTitleLabel.Size = New System.Drawing.Size(230, 20)
        Me.BsFirmwareInfoTitleLabel.TabIndex = 22
        Me.BsFirmwareInfoTitleLabel.Text = "Information"
        Me.BsFirmwareInfoTitleLabel.Title = True
        '
        'BsFirmwarePanel
        '
        Me.BsFirmwarePanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsFirmwarePanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsFirmwarePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsFirmwarePanel.Controls.Add(Me.BsUpdateFirmwareGroupBox)
        Me.BsFirmwarePanel.Controls.Add(Me.BsFwOpenFilePathLabel)
        Me.BsFirmwarePanel.Controls.Add(Me.BsFwCurrentVersionLabel)
        Me.BsFirmwarePanel.Controls.Add(Me.BsFwCurrentVersionCaptionLabel)
        Me.BsFirmwarePanel.Controls.Add(Me.BsFwTitleLabel)
        Me.BsFirmwarePanel.Controls.Add(Me.BsIntermediateAdjustTitle)
        Me.BsFirmwarePanel.ImeMode = System.Windows.Forms.ImeMode.[On]
        Me.BsFirmwarePanel.Location = New System.Drawing.Point(231, 0)
        Me.BsFirmwarePanel.Name = "BsFirmwarePanel"
        Me.BsFirmwarePanel.Size = New System.Drawing.Size(739, 532)
        Me.BsFirmwarePanel.TabIndex = 28
        '
        'BsUpdateFirmwareGroupBox
        '
        Me.BsUpdateFirmwareGroupBox.Controls.Add(Me.BsFwOpenFileCaptionLabel)
        Me.BsUpdateFirmwareGroupBox.Controls.Add(Me.BsFileNameTextBox)
        Me.BsUpdateFirmwareGroupBox.Controls.Add(Me.BsFwUpdateCaptionLabel)
        Me.BsUpdateFirmwareGroupBox.Controls.Add(Me.BsFwNewVersionLabel)
        Me.BsUpdateFirmwareGroupBox.Controls.Add(Me.BsFwUpdateButton)
        Me.BsUpdateFirmwareGroupBox.Controls.Add(Me.BsFwOpenFileButton)
        Me.BsUpdateFirmwareGroupBox.Controls.Add(Me.BsFwNewVersionCaptionLabel)
        Me.BsUpdateFirmwareGroupBox.ForeColor = System.Drawing.Color.Black
        Me.BsUpdateFirmwareGroupBox.Location = New System.Drawing.Point(35, 138)
        Me.BsUpdateFirmwareGroupBox.Name = "BsUpdateFirmwareGroupBox"
        Me.BsUpdateFirmwareGroupBox.Size = New System.Drawing.Size(669, 258)
        Me.BsUpdateFirmwareGroupBox.TabIndex = 79
        Me.BsUpdateFirmwareGroupBox.TabStop = False
        '
        'BsFwOpenFileCaptionLabel
        '
        Me.BsFwOpenFileCaptionLabel.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFwOpenFileCaptionLabel.Location = New System.Drawing.Point(20, 31)
        Me.BsFwOpenFileCaptionLabel.Name = "BsFwOpenFileCaptionLabel"
        Me.BsFwOpenFileCaptionLabel.Size = New System.Drawing.Size(180, 20)
        Me.BsFwOpenFileCaptionLabel.TabIndex = 72
        Me.BsFwOpenFileCaptionLabel.Text = "Select Firmware file:"
        '
        'BsFileNameTextBox
        '
        Me.BsFileNameTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsFileNameTextBox.DecimalsValues = False
        Me.BsFileNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsFileNameTextBox.IsNumeric = False
        Me.BsFileNameTextBox.Location = New System.Drawing.Point(23, 54)
        Me.BsFileNameTextBox.Mandatory = True
        Me.BsFileNameTextBox.MaxLength = 40
        Me.BsFileNameTextBox.Name = "BsFileNameTextBox"
        Me.BsFileNameTextBox.ReadOnly = True
        Me.BsFileNameTextBox.Size = New System.Drawing.Size(355, 21)
        Me.BsFileNameTextBox.TabIndex = 78
        '
        'BsFwUpdateCaptionLabel
        '
        Me.BsFwUpdateCaptionLabel.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFwUpdateCaptionLabel.Location = New System.Drawing.Point(20, 203)
        Me.BsFwUpdateCaptionLabel.Name = "BsFwUpdateCaptionLabel"
        Me.BsFwUpdateCaptionLabel.Size = New System.Drawing.Size(378, 20)
        Me.BsFwUpdateCaptionLabel.TabIndex = 74
        Me.BsFwUpdateCaptionLabel.Text = "Update Firmware:"
        '
        'BsFwNewVersionLabel
        '
        Me.BsFwNewVersionLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsFwNewVersionLabel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsFwNewVersionLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsFwNewVersionLabel.Font = New System.Drawing.Font("Digiface", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFwNewVersionLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.BsFwNewVersionLabel.Location = New System.Drawing.Point(531, 116)
        Me.BsFwNewVersionLabel.Name = "BsFwNewVersionLabel"
        Me.BsFwNewVersionLabel.Size = New System.Drawing.Size(120, 30)
        Me.BsFwNewVersionLabel.TabIndex = 76
        Me.BsFwNewVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BsFwUpdateButton
        '
        Me.BsFwUpdateButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsFwUpdateButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsFwUpdateButton.Enabled = False
        Me.BsFwUpdateButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BsFwUpdateButton.Location = New System.Drawing.Point(619, 195)
        Me.BsFwUpdateButton.Name = "BsFwUpdateButton"
        Me.BsFwUpdateButton.Size = New System.Drawing.Size(32, 32)
        Me.BsFwUpdateButton.TabIndex = 69
        Me.BsFwUpdateButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsFwUpdateButton.UseVisualStyleBackColor = True
        '
        'BsFwOpenFileButton
        '
        Me.BsFwOpenFileButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsFwOpenFileButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsFwOpenFileButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.BsFwOpenFileButton.Location = New System.Drawing.Point(619, 43)
        Me.BsFwOpenFileButton.Name = "BsFwOpenFileButton"
        Me.BsFwOpenFileButton.Size = New System.Drawing.Size(32, 32)
        Me.BsFwOpenFileButton.TabIndex = 73
        Me.BsFwOpenFileButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsFwOpenFileButton.UseVisualStyleBackColor = True
        '
        'BsFwNewVersionCaptionLabel
        '
        Me.BsFwNewVersionCaptionLabel.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFwNewVersionCaptionLabel.Location = New System.Drawing.Point(20, 121)
        Me.BsFwNewVersionCaptionLabel.Name = "BsFwNewVersionCaptionLabel"
        Me.BsFwNewVersionCaptionLabel.Size = New System.Drawing.Size(287, 20)
        Me.BsFwNewVersionCaptionLabel.TabIndex = 75
        Me.BsFwNewVersionCaptionLabel.Text = "Firmware file version:"
        '
        'BsFwOpenFilePathLabel
        '
        Me.BsFwOpenFilePathLabel.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFwOpenFilePathLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.BsFwOpenFilePathLabel.Location = New System.Drawing.Point(115, 234)
        Me.BsFwOpenFilePathLabel.Name = "BsFwOpenFilePathLabel"
        Me.BsFwOpenFilePathLabel.Size = New System.Drawing.Size(460, 20)
        Me.BsFwOpenFilePathLabel.TabIndex = 77
        Me.BsFwOpenFilePathLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'BsFwCurrentVersionLabel
        '
        Me.BsFwCurrentVersionLabel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsFwCurrentVersionLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsFwCurrentVersionLabel.Font = New System.Drawing.Font("Digiface", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFwCurrentVersionLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.BsFwCurrentVersionLabel.Location = New System.Drawing.Point(566, 54)
        Me.BsFwCurrentVersionLabel.Name = "BsFwCurrentVersionLabel"
        Me.BsFwCurrentVersionLabel.Size = New System.Drawing.Size(120, 30)
        Me.BsFwCurrentVersionLabel.TabIndex = 71
        Me.BsFwCurrentVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BsFwCurrentVersionCaptionLabel
        '
        Me.BsFwCurrentVersionCaptionLabel.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFwCurrentVersionCaptionLabel.Location = New System.Drawing.Point(41, 64)
        Me.BsFwCurrentVersionCaptionLabel.Name = "BsFwCurrentVersionCaptionLabel"
        Me.BsFwCurrentVersionCaptionLabel.Size = New System.Drawing.Size(301, 20)
        Me.BsFwCurrentVersionCaptionLabel.TabIndex = 70
        Me.BsFwCurrentVersionCaptionLabel.Text = "Analyzer's Firmware current version:"
        '
        'BsFwTitleLabel
        '
        Me.BsFwTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsFwTitleLabel.Dock = System.Windows.Forms.DockStyle.Top
        Me.BsFwTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsFwTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsFwTitleLabel.Location = New System.Drawing.Point(0, 0)
        Me.BsFwTitleLabel.Name = "BsFwTitleLabel"
        Me.BsFwTitleLabel.Size = New System.Drawing.Size(737, 20)
        Me.BsFwTitleLabel.TabIndex = 39
        Me.BsFwTitleLabel.Text = "Update Firmware"
        Me.BsFwTitleLabel.Title = True
        '
        'BsIntermediateAdjustTitle
        '
        Me.BsIntermediateAdjustTitle.BackColor = System.Drawing.Color.Transparent
        Me.BsIntermediateAdjustTitle.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsIntermediateAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.BsIntermediateAdjustTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsIntermediateAdjustTitle.Name = "BsIntermediateAdjustTitle"
        Me.BsIntermediateAdjustTitle.Size = New System.Drawing.Size(100, 23)
        Me.BsIntermediateAdjustTitle.TabIndex = 40
        Me.BsIntermediateAdjustTitle.Title = False
        '
        'BsButtonsPanel
        '
        Me.BsButtonsPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButtonsPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsButtonsPanel.Controls.Add(Me.BsExitButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsCancelButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsSaveButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsAdjustButton)
        Me.BsButtonsPanel.Location = New System.Drawing.Point(810, 557)
        Me.BsButtonsPanel.Name = "BsButtonsPanel"
        Me.BsButtonsPanel.Size = New System.Drawing.Size(168, 35)
        Me.BsButtonsPanel.TabIndex = 13
        '
        'BsCancelButton
        '
        Me.BsCancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsCancelButton.Location = New System.Drawing.Point(98, 1)
        Me.BsCancelButton.Name = "BsCancelButton"
        Me.BsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.BsCancelButton.TabIndex = 2
        Me.BsCancelButton.UseVisualStyleBackColor = True
        Me.BsCancelButton.Visible = False
        '
        'BsSaveButton
        '
        Me.BsSaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsSaveButton.Location = New System.Drawing.Point(62, 1)
        Me.BsSaveButton.Name = "BsSaveButton"
        Me.BsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.BsSaveButton.TabIndex = 1
        Me.BsSaveButton.UseVisualStyleBackColor = True
        Me.BsSaveButton.Visible = False
        '
        'BsAdjustButton
        '
        Me.BsAdjustButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjustButton.Enabled = False
        Me.BsAdjustButton.Location = New System.Drawing.Point(25, 1)
        Me.BsAdjustButton.Name = "BsAdjustButton"
        Me.BsAdjustButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjustButton.TabIndex = 0
        Me.BsAdjustButton.UseVisualStyleBackColor = True
        Me.BsAdjustButton.Visible = False
        '
        'BsMessagesPanel
        '
        Me.BsMessagesPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessagesPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsMessagesPanel.Controls.Add(Me.BsWaitProgressBar)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageImage)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageLabel)
        Me.BsMessagesPanel.Location = New System.Drawing.Point(0, 557)
        Me.BsMessagesPanel.Name = "BsMessagesPanel"
        Me.BsMessagesPanel.Size = New System.Drawing.Size(811, 35)
        Me.BsMessagesPanel.TabIndex = 12
        '
        'BsWaitProgressBar
        '
        Me.BsWaitProgressBar.Location = New System.Drawing.Point(494, 9)
        Me.BsWaitProgressBar.Name = "BsWaitProgressBar"
        Me.BsWaitProgressBar.Size = New System.Drawing.Size(180, 18)
        Me.BsWaitProgressBar.TabIndex = 7
        Me.BsWaitProgressBar.Visible = False
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
        'IInstrumentUpdateUtil
        '
        Me.AcceptButton = Me.BsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 593)
        Me.Controls.Add(Me.BsTabPagesControl)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "IInstrumentUpdateUtil"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.BsTabPagesControl.ResumeLayout(False)
        Me.BsAdjustmentsTab.ResumeLayout(False)
        Me.BsAdjInfoPanel.ResumeLayout(False)
        Me.BsAdjustmentsPanel.ResumeLayout(False)
        Me.BsAdjustmentsTextPanel.ResumeLayout(False)
        Me.BsFirmwareTab.ResumeLayout(False)
        Me.BsFwInfoPanel.ResumeLayout(False)
        Me.BsFirmwarePanel.ResumeLayout(False)
        Me.BsUpdateFirmwareGroupBox.ResumeLayout(False)
        Me.BsUpdateFirmwareGroupBox.PerformLayout()
        Me.BsButtonsPanel.ResumeLayout(False)
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsResponse As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjustButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsTabPagesControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents BsAdjustmentsTab As System.Windows.Forms.TabPage
    Friend WithEvents BsAdjInfoTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsAdjustmentsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsAdjustmentsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsAdjInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsFirmwareTab As System.Windows.Forms.TabPage
    Friend WithEvents BsPanel1 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsLabel4 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel5 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsNotContaminatedInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsNotContaminatedInfoLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsFirmwarePanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsIntermediateAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsFwInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsFirmwareInfoTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsFwTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsAdjustmentsRTextBox As Biosystems.Ax00.Controls.UserControls.BSRichTextBox
    Friend WithEvents BsAdjPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjRestoreButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjBackupButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents PrintDocument1 As System.Drawing.Printing.PrintDocument
    Friend WithEvents PrintDialog1 As System.Windows.Forms.PrintDialog
    Friend WithEvents OpenAdjFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SaveAdjFileDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents BsAdjRestoreFactoryButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsFwUpdateButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents OpenFwFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents BsFwCurrentVersionLabel As System.Windows.Forms.Label
    Friend WithEvents BsFwCurrentVersionCaptionLabel As System.Windows.Forms.Label
    Friend WithEvents BsFwNewVersionLabel As System.Windows.Forms.Label
    Friend WithEvents BsFwNewVersionCaptionLabel As System.Windows.Forms.Label
    Friend WithEvents BsFwUpdateCaptionLabel As System.Windows.Forms.Label
    Friend WithEvents BsFwOpenFileButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsFwOpenFileCaptionLabel As System.Windows.Forms.Label
    Friend WithEvents BsFwOpenFilePathLabel As System.Windows.Forms.Label
    Friend WithEvents BsInfoAdjXPSViewer As BsXPSViewer
    Friend WithEvents BsInfoFwXPSViewer As BsXPSViewer
    Friend WithEvents BsAdjSaveAsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjOpenFileButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjustmentsTextPanel As System.Windows.Forms.Panel
    Friend WithEvents BsAdjEditButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjUndoButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjNewButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsUpdateFirmwareGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsFileNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsWaitProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents BSAdjHeaderRTextBox As Biosystems.Ax00.Controls.UserControls.BSRichTextBox
End Class
