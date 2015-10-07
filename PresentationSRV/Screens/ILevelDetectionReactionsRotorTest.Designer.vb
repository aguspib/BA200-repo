<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ILevelDetectionReactionsRotorTest
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
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsTitleLabel = New System.Windows.Forms.Label()
        Me.BsLevelDetectionInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsLevelDetectionInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.BsInfoXPSViewer = New BsXPSViewer()
        Me.BsInfoExpandButton = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.lblLdmin = New System.Windows.Forms.Label()
        Me.lblLdminValue = New System.Windows.Forms.Label()
        Me.btnStartTest = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsTestTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsLevelDetectionTestPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.gpbLevelDetection = New System.Windows.Forms.GroupBox()
        Me.btnShowLevel = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.lblLdmem = New System.Windows.Forms.Label()
        Me.lblLdmemValue = New System.Windows.Forms.Label()
        Me.btnCancel = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsButtonsPanel.SuspendLayout()
        Me.BsLevelDetectionInfoPanel.SuspendLayout()
        Me.BsLevelDetectionTestPanel.SuspendLayout()
        Me.gpbLevelDetection.SuspendLayout()
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
        Me.BsInfoXPSViewer.TabIndex = 37
        Me.BsInfoXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoXPSViewer.VerticalPageMargin = 0
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
        'lblLdmin
        '
        Me.lblLdmin.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLdmin.Location = New System.Drawing.Point(35, 46)
        Me.lblLdmin.Margin = New System.Windows.Forms.Padding(3)
        Me.lblLdmin.Name = "lblLdmin"
        Me.lblLdmin.Size = New System.Drawing.Size(68, 24)
        Me.lblLdmin.TabIndex = 61
        Me.lblLdmin.Text = "LDMIN"
        Me.lblLdmin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblLdminValue
        '
        Me.lblLdminValue.BackColor = System.Drawing.Color.WhiteSmoke
        Me.lblLdminValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblLdminValue.Font = New System.Drawing.Font("Digiface", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLdminValue.ForeColor = System.Drawing.Color.LimeGreen
        Me.lblLdminValue.Location = New System.Drawing.Point(121, 42)
        Me.lblLdminValue.Margin = New System.Windows.Forms.Padding(40, 5, 40, 40)
        Me.lblLdminValue.Name = "lblLdminValue"
        Me.lblLdminValue.Size = New System.Drawing.Size(110, 34)
        Me.lblLdminValue.TabIndex = 76
        Me.lblLdminValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnStartTest
        '
        Me.btnStartTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.btnStartTest.Enabled = False
        Me.btnStartTest.Location = New System.Drawing.Point(559, 109)
        Me.btnStartTest.Name = "btnStartTest"
        Me.btnStartTest.Size = New System.Drawing.Size(32, 32)
        Me.btnStartTest.TabIndex = 55
        Me.btnStartTest.UseVisualStyleBackColor = True
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
        Me.BsLevelDetectionTestPanel.Controls.Add(Me.gpbLevelDetection)
        Me.BsLevelDetectionTestPanel.Controls.Add(Me.BsTestTitleLabel)
        Me.BsLevelDetectionTestPanel.Location = New System.Drawing.Point(234, 25)
        Me.BsLevelDetectionTestPanel.Name = "BsLevelDetectionTestPanel"
        Me.BsLevelDetectionTestPanel.Size = New System.Drawing.Size(740, 532)
        Me.BsLevelDetectionTestPanel.TabIndex = 32
        '
        'gpbLevelDetection
        '
        Me.gpbLevelDetection.Controls.Add(Me.btnCancel)
        Me.gpbLevelDetection.Controls.Add(Me.btnShowLevel)
        Me.gpbLevelDetection.Controls.Add(Me.lblLdmem)
        Me.gpbLevelDetection.Controls.Add(Me.btnStartTest)
        Me.gpbLevelDetection.Controls.Add(Me.lblLdmemValue)
        Me.gpbLevelDetection.Controls.Add(Me.lblLdmin)
        Me.gpbLevelDetection.Controls.Add(Me.lblLdminValue)
        Me.gpbLevelDetection.Location = New System.Drawing.Point(16, 23)
        Me.gpbLevelDetection.Name = "gpbLevelDetection"
        Me.gpbLevelDetection.Size = New System.Drawing.Size(705, 161)
        Me.gpbLevelDetection.TabIndex = 73
        Me.gpbLevelDetection.TabStop = False
        '
        'btnShowLevel
        '
        Me.btnShowLevel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.btnShowLevel.Enabled = False
        Me.btnShowLevel.Location = New System.Drawing.Point(606, 109)
        Me.btnShowLevel.Name = "btnShowLevel"
        Me.btnShowLevel.Size = New System.Drawing.Size(32, 32)
        Me.btnShowLevel.TabIndex = 79
        Me.btnShowLevel.UseVisualStyleBackColor = True
        '
        'lblLdmem
        '
        Me.lblLdmem.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLdmem.Location = New System.Drawing.Point(35, 93)
        Me.lblLdmem.Margin = New System.Windows.Forms.Padding(3)
        Me.lblLdmem.Name = "lblLdmem"
        Me.lblLdmem.Size = New System.Drawing.Size(68, 24)
        Me.lblLdmem.TabIndex = 77
        Me.lblLdmem.Text = "LDMEM"
        Me.lblLdmem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblLdmemValue
        '
        Me.lblLdmemValue.BackColor = System.Drawing.Color.WhiteSmoke
        Me.lblLdmemValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblLdmemValue.Font = New System.Drawing.Font("Digiface", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLdmemValue.ForeColor = System.Drawing.Color.LimeGreen
        Me.lblLdmemValue.Location = New System.Drawing.Point(121, 89)
        Me.lblLdmemValue.Margin = New System.Windows.Forms.Padding(40, 5, 40, 40)
        Me.lblLdmemValue.Name = "lblLdmemValue"
        Me.lblLdmemValue.Size = New System.Drawing.Size(110, 34)
        Me.lblLdmemValue.TabIndex = 78
        Me.lblLdmemValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnCancel
        '
        Me.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.btnCancel.Enabled = False
        Me.btnCancel.Location = New System.Drawing.Point(654, 109)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(32, 32)
        Me.btnCancel.TabIndex = 80
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'ILevelDetectionReactionsRotorTest
        '
        Me.AcceptButton = Me.BsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = true
        Me.Appearance.Options.UseFont = true
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 593)
        Me.Controls.Add(Me.BsTitleLabel)
        Me.Controls.Add(Me.BsLevelDetectionInfoPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsLevelDetectionTestPanel)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = false
        Me.Name = "ILevelDetectionReactionsRotorTest"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "StressModeTest"
        Me.BsMessagesPanel.ResumeLayout(false)
        CType(Me.BsMessageImage,System.ComponentModel.ISupportInitialize).EndInit
        Me.BsButtonsPanel.ResumeLayout(false)
        Me.BsLevelDetectionInfoPanel.ResumeLayout(false)
        Me.BsLevelDetectionTestPanel.ResumeLayout(false)
        Me.gpbLevelDetection.ResumeLayout(false)
        Me.ResumeLayout(false)
        Me.PerformLayout

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
    Friend WithEvents btnStartTest As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTestTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLevelDetectionTestPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents lblLdmin As System.Windows.Forms.Label
    Friend WithEvents gpbLevelDetection As System.Windows.Forms.GroupBox
    Friend WithEvents lblLdminValue As System.Windows.Forms.Label
    Friend WithEvents btnShowLevel As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents lblLdmem As System.Windows.Forms.Label
    Friend WithEvents lblLdmemValue As System.Windows.Forms.Label
    Friend WithEvents btnCancel As Biosystems.Ax00.Controls.UserControls.BSButton
End Class
