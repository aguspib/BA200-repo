<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ICycleCountScreen
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
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.BsCycleCountInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoXPSViewer = New BsXPSViewer
        Me.BsInfoExpandButton = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsCycleCountInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsCycleAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsFontLabel = New System.Windows.Forms.Label
        Me.BsSelectAllCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.BsWriteButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsCyclesDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.BsCycleCountTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsStopTestButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTestButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTestButtonTODELETE = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTitleLabel = New System.Windows.Forms.Label
        Me.BsWaitProgressBar = New System.Windows.Forms.ProgressBar
        Me.BsCycleCountInfoPanel.SuspendLayout()
        Me.BsCycleAdjustPanel.SuspendLayout()
        CType(Me.bsCyclesDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsButtonsPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'BsCycleCountInfoPanel
        '
        Me.BsCycleCountInfoPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsCycleCountInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsCycleCountInfoPanel.Controls.Add(Me.BsInfoXPSViewer)
        Me.BsCycleCountInfoPanel.Controls.Add(Me.BsInfoExpandButton)
        Me.BsCycleCountInfoPanel.Controls.Add(Me.BsCycleCountInfoTitle)
        Me.BsCycleCountInfoPanel.Location = New System.Drawing.Point(4, 25)
        Me.BsCycleCountInfoPanel.Name = "BsCycleCountInfoPanel"
        Me.BsCycleCountInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsCycleCountInfoPanel.TabIndex = 62
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
        Me.BsInfoXPSViewer.Location = New System.Drawing.Point(0, 21)
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
        'BsCycleCountInfoTitle
        '
        Me.BsCycleCountInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCycleCountInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsCycleCountInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsCycleCountInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsCycleCountInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsCycleCountInfoTitle.Name = "BsCycleCountInfoTitle"
        Me.BsCycleCountInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsCycleCountInfoTitle.TabIndex = 22
        Me.BsCycleCountInfoTitle.Text = "Information"
        Me.BsCycleCountInfoTitle.Title = True
        '
        'BsCycleAdjustPanel
        '
        Me.BsCycleAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCycleAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsCycleAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsCycleAdjustPanel.Controls.Add(Me.BsFontLabel)
        Me.BsCycleAdjustPanel.Controls.Add(Me.BsSelectAllCheckbox)
        Me.BsCycleAdjustPanel.Controls.Add(Me.BsWriteButton)
        Me.BsCycleAdjustPanel.Controls.Add(Me.bsCyclesDataGridView)
        Me.BsCycleAdjustPanel.Controls.Add(Me.BsCycleCountTitleLabel)
        Me.BsCycleAdjustPanel.Location = New System.Drawing.Point(234, 25)
        Me.BsCycleAdjustPanel.Name = "BsCycleAdjustPanel"
        Me.BsCycleAdjustPanel.Size = New System.Drawing.Size(740, 532)
        Me.BsCycleAdjustPanel.TabIndex = 63
        '
        'BsFontLabel
        '
        Me.BsFontLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFontLabel.Location = New System.Drawing.Point(15, 506)
        Me.BsFontLabel.Name = "BsFontLabel"
        Me.BsFontLabel.Size = New System.Drawing.Size(75, 21)
        Me.BsFontLabel.TabIndex = 63
        Me.BsFontLabel.Visible = False
        '
        'BsSelectAllCheckbox
        '
        Me.BsSelectAllCheckbox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsSelectAllCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsSelectAllCheckbox.Location = New System.Drawing.Point(538, 503)
        Me.BsSelectAllCheckbox.Name = "BsSelectAllCheckbox"
        Me.BsSelectAllCheckbox.Size = New System.Drawing.Size(130, 20)
        Me.BsSelectAllCheckbox.TabIndex = 62
        Me.BsSelectAllCheckbox.Text = "Select All"
        Me.BsSelectAllCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsSelectAllCheckbox.UseVisualStyleBackColor = True
        '
        'BsWriteButton
        '
        Me.BsWriteButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsWriteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsWriteButton.Enabled = False
        Me.BsWriteButton.Location = New System.Drawing.Point(688, 33)
        Me.BsWriteButton.Name = "BsWriteButton"
        Me.BsWriteButton.Size = New System.Drawing.Size(32, 32)
        Me.BsWriteButton.TabIndex = 61
        Me.BsWriteButton.UseVisualStyleBackColor = True
        '
        'bsCyclesDataGridView
        '
        Me.bsCyclesDataGridView.AllowUserToAddRows = False
        Me.bsCyclesDataGridView.AllowUserToDeleteRows = False
        Me.bsCyclesDataGridView.AllowUserToResizeColumns = False
        Me.bsCyclesDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle6.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCyclesDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle6
        Me.bsCyclesDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsCyclesDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsCyclesDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsCyclesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle7.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCyclesDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.bsCyclesDataGridView.ColumnHeadersHeight = 20
        Me.bsCyclesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCyclesDataGridView.DefaultCellStyle = DataGridViewCellStyle8
        Me.bsCyclesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsCyclesDataGridView.EnterToTab = False
        Me.bsCyclesDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsCyclesDataGridView.Location = New System.Drawing.Point(18, 33)
        Me.bsCyclesDataGridView.Name = "bsCyclesDataGridView"
        Me.bsCyclesDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle9.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCyclesDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.bsCyclesDataGridView.RowHeadersVisible = False
        Me.bsCyclesDataGridView.RowHeadersWidth = 20
        Me.bsCyclesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle10.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCyclesDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle10
        Me.bsCyclesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsCyclesDataGridView.Size = New System.Drawing.Size(651, 464)
        Me.bsCyclesDataGridView.TabIndex = 60
        Me.bsCyclesDataGridView.TabStop = False
        Me.bsCyclesDataGridView.TabToEnter = False
        '
        'BsCycleCountTitleLabel
        '
        Me.BsCycleCountTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsCycleCountTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsCycleCountTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsCycleCountTitleLabel.Location = New System.Drawing.Point(0, 0)
        Me.BsCycleCountTitleLabel.Name = "BsCycleCountTitleLabel"
        Me.BsCycleCountTitleLabel.Size = New System.Drawing.Size(739, 20)
        Me.BsCycleCountTitleLabel.TabIndex = 59
        Me.BsCycleCountTitleLabel.Text = "Cycle Count"
        Me.BsCycleCountTitleLabel.Title = True
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
        Me.BsMessagesPanel.TabIndex = 64
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
        Me.BsButtonsPanel.Controls.Add(Me.BsStopTestButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsTestButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsTestButtonTODELETE)
        Me.BsButtonsPanel.Location = New System.Drawing.Point(810, 557)
        Me.BsButtonsPanel.Name = "BsButtonsPanel"
        Me.BsButtonsPanel.Size = New System.Drawing.Size(168, 35)
        Me.BsButtonsPanel.TabIndex = 65
        '
        'BsExitButton
        '
        Me.BsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsExitButton.Enabled = False
        Me.BsExitButton.Location = New System.Drawing.Point(134, 1)
        Me.BsExitButton.Name = "BsExitButton"
        Me.BsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.BsExitButton.TabIndex = 3
        Me.BsExitButton.UseVisualStyleBackColor = True
        '
        'BsStopTestButton
        '
        Me.BsStopTestButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsStopTestButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsStopTestButton.Enabled = False
        Me.BsStopTestButton.Location = New System.Drawing.Point(98, 1)
        Me.BsStopTestButton.Name = "BsStopTestButton"
        Me.BsStopTestButton.Size = New System.Drawing.Size(32, 32)
        Me.BsStopTestButton.TabIndex = 2
        Me.BsStopTestButton.UseVisualStyleBackColor = True
        Me.BsStopTestButton.Visible = False
        '
        'BsTestButton
        '
        Me.BsTestButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTestButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsTestButton.Enabled = False
        Me.BsTestButton.Location = New System.Drawing.Point(62, 1)
        Me.BsTestButton.Name = "BsTestButton"
        Me.BsTestButton.Size = New System.Drawing.Size(32, 32)
        Me.BsTestButton.TabIndex = 1
        Me.BsTestButton.UseVisualStyleBackColor = True
        Me.BsTestButton.Visible = False
        '
        'BsTestButtonTODELETE
        '
        Me.BsTestButtonTODELETE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTestButtonTODELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsTestButtonTODELETE.Enabled = False
        Me.BsTestButtonTODELETE.Location = New System.Drawing.Point(25, 1)
        Me.BsTestButtonTODELETE.Name = "BsTestButtonTODELETE"
        Me.BsTestButtonTODELETE.Size = New System.Drawing.Size(32, 32)
        Me.BsTestButtonTODELETE.TabIndex = 0
        Me.BsTestButtonTODELETE.UseVisualStyleBackColor = True
        Me.BsTestButtonTODELETE.Visible = False
        '
        'BsTitleLabel
        '
        Me.BsTitleLabel.AutoSize = True
        Me.BsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsTitleLabel.Location = New System.Drawing.Point(2, 2)
        Me.BsTitleLabel.Name = "BsTitleLabel"
        Me.BsTitleLabel.Size = New System.Drawing.Size(92, 17)
        Me.BsTitleLabel.TabIndex = 66
        Me.BsTitleLabel.Text = "Cycle Count"
        '
        'BsWaitProgressBar
        '
        Me.BsWaitProgressBar.Location = New System.Drawing.Point(494, 9)
        Me.BsWaitProgressBar.Name = "BsWaitProgressBar"
        Me.BsWaitProgressBar.Size = New System.Drawing.Size(180, 18)
        Me.BsWaitProgressBar.TabIndex = 8
        Me.BsWaitProgressBar.Visible = False
        '
        'ICycleCountScreen
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
        Me.Controls.Add(Me.BsCycleCountInfoPanel)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.Controls.Add(Me.BsCycleAdjustPanel)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "ICycleCountScreen"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "DemoMode"
        Me.BsCycleCountInfoPanel.ResumeLayout(False)
        Me.BsCycleAdjustPanel.ResumeLayout(False)
        CType(Me.bsCyclesDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsButtonsPanel.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BsCycleCountInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsCycleCountInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsCycleAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsCycleCountTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsStopTestButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTestButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTestButtonTODELETE As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTitleLabel As System.Windows.Forms.Label
    Friend WithEvents bsCyclesDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents BsSelectAllCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsWriteButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsFontLabel As System.Windows.Forms.Label
    Friend WithEvents BsInfoExpandButton As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsInfoXPSViewer As BsXPSViewer
    Friend WithEvents BsWaitProgressBar As System.Windows.Forms.ProgressBar
End Class
