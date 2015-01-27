<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiDemoMode
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiDemoMode))
        Me.BsDemoModeInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoXPSViewer = New BsXPSViewer
        Me.BsDemoModeInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsDemoAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.picAjax = New System.Windows.Forms.PictureBox
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.BsSubtitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTestButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTestButton_TODELETE = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTestButtonTODELETE = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTitleLabel = New System.Windows.Forms.Label
        Me.RequestStatusStressTimer = New System.Windows.Forms.Timer(Me.components)
        Me.BsDemoModeInfoPanel.SuspendLayout()
        Me.BsDemoAdjustPanel.SuspendLayout()
        CType(Me.picAjax, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsButtonsPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'BsDemoModeInfoPanel
        '
        Me.BsDemoModeInfoPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsDemoModeInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsDemoModeInfoPanel.Controls.Add(Me.BsInfoXPSViewer)
        Me.BsDemoModeInfoPanel.Controls.Add(Me.BsDemoModeInfoTitle)
        Me.BsDemoModeInfoPanel.Location = New System.Drawing.Point(4, 25)
        Me.BsDemoModeInfoPanel.Name = "BsDemoModeInfoPanel"
        Me.BsDemoModeInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsDemoModeInfoPanel.TabIndex = 62
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
        'BsDemoModeInfoTitle
        '
        Me.BsDemoModeInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsDemoModeInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsDemoModeInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsDemoModeInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsDemoModeInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsDemoModeInfoTitle.Name = "BsDemoModeInfoTitle"
        Me.BsDemoModeInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsDemoModeInfoTitle.TabIndex = 22
        Me.BsDemoModeInfoTitle.Text = "Information"
        Me.BsDemoModeInfoTitle.Title = True
        '
        'BsDemoAdjustPanel
        '
        Me.BsDemoAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsDemoAdjustPanel.BackColor = System.Drawing.Color.White
        Me.BsDemoAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsDemoAdjustPanel.Controls.Add(Me.picAjax)
        Me.BsDemoAdjustPanel.Controls.Add(Me.PictureBox1)
        Me.BsDemoAdjustPanel.Controls.Add(Me.BsSubtitleLabel)
        Me.BsDemoAdjustPanel.Location = New System.Drawing.Point(234, 25)
        Me.BsDemoAdjustPanel.Name = "BsDemoAdjustPanel"
        Me.BsDemoAdjustPanel.Size = New System.Drawing.Size(742, 532)
        Me.BsDemoAdjustPanel.TabIndex = 63
        '
        'picAjax
        '
        Me.picAjax.BackColor = System.Drawing.Color.Transparent
        Me.picAjax.Image = CType(resources.GetObject("picAjax.Image"), System.Drawing.Image)
        Me.picAjax.Location = New System.Drawing.Point(37, 276)
        Me.picAjax.Name = "picAjax"
        Me.picAjax.Size = New System.Drawing.Size(80, 80)
        Me.picAjax.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picAjax.TabIndex = 60
        Me.picAjax.TabStop = False
        Me.picAjax.Visible = False
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Transparent
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.InitialImage = Nothing
        Me.PictureBox1.Location = New System.Drawing.Point(61, 22)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(644, 533)
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
        Me.BsSubtitleLabel.Text = "BioSystems A400 Analyzer"
        Me.BsSubtitleLabel.Title = True
        '
        'BsMessagesPanel
        '
        Me.BsMessagesPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessagesPanel.BackColor = System.Drawing.Color.WhiteSmoke
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
        Me.BsButtonsPanel.Controls.Add(Me.BsTestButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsTestButton_TODELETE)
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
        'BsTestButton
        '
        Me.BsTestButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTestButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsTestButton.Enabled = False
        Me.BsTestButton.Location = New System.Drawing.Point(98, 1)
        Me.BsTestButton.Name = "BsTestButton"
        Me.BsTestButton.Size = New System.Drawing.Size(32, 32)
        Me.BsTestButton.TabIndex = 2
        Me.BsTestButton.UseVisualStyleBackColor = True
        '
        'BsTestButton_TODELETE
        '
        Me.BsTestButton_TODELETE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTestButton_TODELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsTestButton_TODELETE.Enabled = False
        Me.BsTestButton_TODELETE.Location = New System.Drawing.Point(62, 1)
        Me.BsTestButton_TODELETE.Name = "BsTestButton_TODELETE"
        Me.BsTestButton_TODELETE.Size = New System.Drawing.Size(32, 32)
        Me.BsTestButton_TODELETE.TabIndex = 1
        Me.BsTestButton_TODELETE.UseVisualStyleBackColor = True
        Me.BsTestButton_TODELETE.Visible = False
        '
        'BsTestButtonTODELETE
        '
        Me.BsTestButtonTODELETE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTestButtonTODELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsTestButtonTODELETE.Enabled = False
        Me.BsTestButtonTODELETE.Location = New System.Drawing.Point(26, 1)
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
        Me.BsTitleLabel.Size = New System.Drawing.Size(91, 17)
        Me.BsTitleLabel.TabIndex = 66
        Me.BsTitleLabel.Text = "Demo Mode"
        '
        'RequestStatusStressTimer
        '
        '
        'IDemoMode
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
        Me.Controls.Add(Me.BsDemoModeInfoPanel)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.Controls.Add(Me.BsDemoAdjustPanel)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "IDemoMode"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "DemoMode"
        Me.BsDemoModeInfoPanel.ResumeLayout(False)
        Me.BsDemoAdjustPanel.ResumeLayout(False)
        CType(Me.picAjax, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsButtonsPanel.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BsDemoModeInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsDemoModeInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsDemoAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsSubtitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTestButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTestButton_TODELETE As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTestButtonTODELETE As Biosystems.Ax00.Controls.UserControls.BSButton
    Private WithEvents picAjax As System.Windows.Forms.PictureBox
    Private WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents BsTitleLabel As System.Windows.Forms.Label
    Friend WithEvents BsInfoXPSViewer As BsXPSViewer
    Friend WithEvents RequestStatusStressTimer As System.Windows.Forms.Timer
End Class
