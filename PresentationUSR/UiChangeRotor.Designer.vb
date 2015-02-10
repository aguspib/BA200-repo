<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiChangeRotor
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiChangeRotor))
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsChangeRotortButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLoadSaveGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsPoint4Label = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsPoint3Label = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsEmptyAndFinalizeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsRepeatReadLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsChangeRotorReadButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsChangeRotorFinalizeButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsPoint2Label = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsChangeRotorLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsPoint1Label = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsNewAdjLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsContinueButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.dxProgressBar = New DevExpress.XtraEditors.ProgressBarControl()
        Me.bsStatusImage = New System.Windows.Forms.PictureBox()
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.BsBorderedPanel1 = New bsBorderedPanel()
        Me.bsLoadSaveGroupBox.SuspendLayout()
        CType(Me.dxProgressBar.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsStatusImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsCancelButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCancelButton.Location = New System.Drawing.Point(358, 226)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 3
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsChangeRotortButton
        '
        Me.bsChangeRotortButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.bsChangeRotortButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsChangeRotortButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsChangeRotortButton.Location = New System.Drawing.Point(348, 53)
        Me.bsChangeRotortButton.Name = "bsChangeRotortButton"
        Me.bsChangeRotortButton.Size = New System.Drawing.Size(32, 32)
        Me.bsChangeRotortButton.TabIndex = 2
        Me.bsChangeRotortButton.UseVisualStyleBackColor = True
        '
        'bsTitleLabel
        '
        Me.bsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsTitleLabel.Name = "bsTitleLabel"
        Me.bsTitleLabel.Size = New System.Drawing.Size(381, 20)
        Me.bsTitleLabel.TabIndex = 8
        Me.bsTitleLabel.Text = "Change Rotor"
        Me.bsTitleLabel.Title = True
        '
        'bsLoadSaveGroupBox
        '
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsPoint4Label)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsPoint3Label)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsEmptyAndFinalizeLabel)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsRepeatReadLabel)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsChangeRotorReadButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsChangeRotorFinalizeButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsPoint2Label)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsChangeRotorLabel)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsPoint1Label)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsNewAdjLabel)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsChangeRotortButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsContinueButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsLoadSaveGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLoadSaveGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsLoadSaveGroupBox.Name = "bsLoadSaveGroupBox"
        Me.bsLoadSaveGroupBox.Size = New System.Drawing.Size(397, 210)
        Me.bsLoadSaveGroupBox.TabIndex = 14
        Me.bsLoadSaveGroupBox.TabStop = False
        '
        'bsPoint4Label
        '
        Me.bsPoint4Label.BackColor = System.Drawing.Color.Transparent
        Me.bsPoint4Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPoint4Label.ForeColor = System.Drawing.Color.Black
        Me.bsPoint4Label.Location = New System.Drawing.Point(13, 171)
        Me.bsPoint4Label.Name = "bsPoint4Label"
        Me.bsPoint4Label.Size = New System.Drawing.Size(19, 28)
        Me.bsPoint4Label.TabIndex = 59
        Me.bsPoint4Label.Text = "4-"
        Me.bsPoint4Label.Title = False
        '
        'bsPoint3Label
        '
        Me.bsPoint3Label.BackColor = System.Drawing.Color.Transparent
        Me.bsPoint3Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPoint3Label.ForeColor = System.Drawing.Color.Black
        Me.bsPoint3Label.Location = New System.Drawing.Point(13, 133)
        Me.bsPoint3Label.Name = "bsPoint3Label"
        Me.bsPoint3Label.Size = New System.Drawing.Size(19, 28)
        Me.bsPoint3Label.TabIndex = 58
        Me.bsPoint3Label.Text = "3-"
        Me.bsPoint3Label.Title = False
        '
        'bsEmptyAndFinalizeLabel
        '
        Me.bsEmptyAndFinalizeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsEmptyAndFinalizeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsEmptyAndFinalizeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsEmptyAndFinalizeLabel.Location = New System.Drawing.Point(30, 171)
        Me.bsEmptyAndFinalizeLabel.Name = "bsEmptyAndFinalizeLabel"
        Me.bsEmptyAndFinalizeLabel.Size = New System.Drawing.Size(279, 28)
        Me.bsEmptyAndFinalizeLabel.TabIndex = 57
        Me.bsEmptyAndFinalizeLabel.Text = "Empty and finalize"
        Me.bsEmptyAndFinalizeLabel.Title = False
        '
        'bsRepeatReadLabel
        '
        Me.bsRepeatReadLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRepeatReadLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRepeatReadLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRepeatReadLabel.Location = New System.Drawing.Point(30, 133)
        Me.bsRepeatReadLabel.Name = "bsRepeatReadLabel"
        Me.bsRepeatReadLabel.Size = New System.Drawing.Size(279, 28)
        Me.bsRepeatReadLabel.TabIndex = 56
        Me.bsRepeatReadLabel.Text = "Read again"
        Me.bsRepeatReadLabel.Title = False
        '
        'bsChangeRotorReadButton
        '
        Me.bsChangeRotorReadButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.bsChangeRotorReadButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsChangeRotorReadButton.Enabled = False
        Me.bsChangeRotorReadButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsChangeRotorReadButton.Location = New System.Drawing.Point(348, 129)
        Me.bsChangeRotorReadButton.Name = "bsChangeRotorReadButton"
        Me.bsChangeRotorReadButton.Size = New System.Drawing.Size(32, 32)
        Me.bsChangeRotorReadButton.TabIndex = 55
        Me.bsChangeRotorReadButton.UseVisualStyleBackColor = True
        '
        'bsChangeRotorFinalizeButton
        '
        Me.bsChangeRotorFinalizeButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.bsChangeRotorFinalizeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsChangeRotorFinalizeButton.Enabled = False
        Me.bsChangeRotorFinalizeButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsChangeRotorFinalizeButton.Location = New System.Drawing.Point(348, 167)
        Me.bsChangeRotorFinalizeButton.Name = "bsChangeRotorFinalizeButton"
        Me.bsChangeRotorFinalizeButton.Size = New System.Drawing.Size(32, 32)
        Me.bsChangeRotorFinalizeButton.TabIndex = 54
        Me.bsChangeRotorFinalizeButton.UseVisualStyleBackColor = True
        '
        'bsPoint2Label
        '
        Me.bsPoint2Label.BackColor = System.Drawing.Color.Transparent
        Me.bsPoint2Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPoint2Label.ForeColor = System.Drawing.Color.Black
        Me.bsPoint2Label.Location = New System.Drawing.Point(13, 95)
        Me.bsPoint2Label.Name = "bsPoint2Label"
        Me.bsPoint2Label.Size = New System.Drawing.Size(19, 28)
        Me.bsPoint2Label.TabIndex = 53
        Me.bsPoint2Label.Text = "2-"
        Me.bsPoint2Label.Title = False
        '
        'bsChangeRotorLabel
        '
        Me.bsChangeRotorLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsChangeRotorLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsChangeRotorLabel.ForeColor = System.Drawing.Color.Black
        Me.bsChangeRotorLabel.Location = New System.Drawing.Point(30, 57)
        Me.bsChangeRotorLabel.Name = "bsChangeRotorLabel"
        Me.bsChangeRotorLabel.Size = New System.Drawing.Size(283, 28)
        Me.bsChangeRotorLabel.TabIndex = 49
        Me.bsChangeRotorLabel.Text = "Press button and remove current rotor "
        Me.bsChangeRotorLabel.Title = False
        '
        'bsPoint1Label
        '
        Me.bsPoint1Label.BackColor = System.Drawing.Color.Transparent
        Me.bsPoint1Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPoint1Label.ForeColor = System.Drawing.Color.Black
        Me.bsPoint1Label.Location = New System.Drawing.Point(13, 57)
        Me.bsPoint1Label.Name = "bsPoint1Label"
        Me.bsPoint1Label.Size = New System.Drawing.Size(19, 28)
        Me.bsPoint1Label.TabIndex = 52
        Me.bsPoint1Label.Text = "1-"
        Me.bsPoint1Label.Title = False
        '
        'bsNewAdjLabel
        '
        Me.bsNewAdjLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNewAdjLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNewAdjLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNewAdjLabel.Location = New System.Drawing.Point(30, 95)
        Me.bsNewAdjLabel.Name = "bsNewAdjLabel"
        Me.bsNewAdjLabel.Size = New System.Drawing.Size(279, 28)
        Me.bsNewAdjLabel.TabIndex = 50
        Me.bsNewAdjLabel.Text = "Put new rotor and press button to perform Light Adjustment"
        Me.bsNewAdjLabel.Title = False
        '
        'bsContinueButton
        '
        Me.bsContinueButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.bsContinueButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsContinueButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsContinueButton.Location = New System.Drawing.Point(348, 91)
        Me.bsContinueButton.Name = "bsContinueButton"
        Me.bsContinueButton.Size = New System.Drawing.Size(32, 32)
        Me.bsContinueButton.TabIndex = 4
        Me.bsContinueButton.UseVisualStyleBackColor = True
        '
        'dxProgressBar
        '
        Me.dxProgressBar.Location = New System.Drawing.Point(118, 233)
        Me.dxProgressBar.Name = "dxProgressBar"
        Me.dxProgressBar.Properties.LookAndFeel.SkinName = "Money Twins"
        Me.dxProgressBar.Properties.LookAndFeel.UseDefaultLookAndFeel = False
        Me.dxProgressBar.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid
        Me.dxProgressBar.ShowToolTips = False
        Me.dxProgressBar.Size = New System.Drawing.Size(180, 18)
        Me.dxProgressBar.TabIndex = 51
        Me.dxProgressBar.UseWaitCursor = True
        Me.dxProgressBar.Visible = False
        '
        'bsStatusImage
        '
        Me.bsStatusImage.BackColor = System.Drawing.Color.Transparent
        Me.bsStatusImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsStatusImage.Location = New System.Drawing.Point(304, 230)
        Me.bsStatusImage.Name = "bsStatusImage"
        Me.bsStatusImage.Size = New System.Drawing.Size(24, 24)
        Me.bsStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsStatusImage.TabIndex = 25
        Me.bsStatusImage.TabStop = False
        Me.bsStatusImage.Visible = False
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'BsBorderedPanel1
        '
        Me.BsBorderedPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsBorderedPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsBorderedPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsBorderedPanel1.Name = "BsBorderedPanel1"
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(416, 274)
        Me.BsBorderedPanel1.TabIndex = 33
        '
        'IChangeRotor
        '
        Me.AcceptButton = Me.bsCancelButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.CancelButton = Me.bsCancelButton
        Me.ClientSize = New System.Drawing.Size(416, 274)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLoadSaveGroupBox)
        Me.Controls.Add(Me.bsCancelButton)
        Me.Controls.Add(Me.dxProgressBar)
        Me.Controls.Add(Me.bsStatusImage)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiChangeRotor"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsLoadSaveGroupBox.ResumeLayout(False)
        CType(Me.dxProgressBar.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsStatusImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsChangeRotortButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLoadSaveGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsContinueButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsChangeRotorLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsNewAdjLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsStatusImage As System.Windows.Forms.PictureBox
    Friend WithEvents dxProgressBar As DevExpress.XtraEditors.ProgressBarControl
    Friend WithEvents bsPoint1Label As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsPoint2Label As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
    Friend WithEvents bsChangeRotorReadButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsChangeRotorFinalizeButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsPoint4Label As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsPoint3Label As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsEmptyAndFinalizeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRepeatReadLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
End Class
