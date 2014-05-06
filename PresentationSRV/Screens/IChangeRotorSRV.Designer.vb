<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IChangeRotorSRV
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IChangeRotorSRV))
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsWSUpDownButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsChangeRotorGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.BsLabel2 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsChangeRotorLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsStatusImage = New System.Windows.Forms.PictureBox
        Me.bsNewAdjLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsNewRotorButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.BsBorderedPanel1 = New bsBorderedPanel
        Me.BsProgressBar = New Biosystems.Ax00.Controls.UserControls.BSProgressBar
        Me.BsNewRotorProcessTimer = New System.Windows.Forms.Timer(Me.components)
        Me.bsChangeRotorGroupBox.SuspendLayout()
        CType(Me.bsStatusImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(358, 189)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 3
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsWSUpDownButton
        '
        Me.bsWSUpDownButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.bsWSUpDownButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsWSUpDownButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsWSUpDownButton.Location = New System.Drawing.Point(348, 53)
        Me.bsWSUpDownButton.Name = "bsWSUpDownButton"
        Me.bsWSUpDownButton.Size = New System.Drawing.Size(32, 32)
        Me.bsWSUpDownButton.TabIndex = 2
        Me.bsWSUpDownButton.UseVisualStyleBackColor = True
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
        'bsChangeRotorGroupBox
        '
        Me.bsChangeRotorGroupBox.Controls.Add(Me.BsProgressBar)
        Me.bsChangeRotorGroupBox.Controls.Add(Me.BsLabel2)
        Me.bsChangeRotorGroupBox.Controls.Add(Me.bsChangeRotorLabel)
        Me.bsChangeRotorGroupBox.Controls.Add(Me.BsLabel1)
        Me.bsChangeRotorGroupBox.Controls.Add(Me.bsStatusImage)
        Me.bsChangeRotorGroupBox.Controls.Add(Me.bsNewAdjLabel)
        Me.bsChangeRotorGroupBox.Controls.Add(Me.bsWSUpDownButton)
        Me.bsChangeRotorGroupBox.Controls.Add(Me.bsNewRotorButton)
        Me.bsChangeRotorGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsChangeRotorGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsChangeRotorGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsChangeRotorGroupBox.Name = "bsChangeRotorGroupBox"
        Me.bsChangeRotorGroupBox.Size = New System.Drawing.Size(397, 170)
        Me.bsChangeRotorGroupBox.TabIndex = 14
        Me.bsChangeRotorGroupBox.TabStop = False
        '
        'BsLabel2
        '
        Me.BsLabel2.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel2.ForeColor = System.Drawing.Color.Black
        Me.BsLabel2.Location = New System.Drawing.Point(13, 97)
        Me.BsLabel2.Name = "BsLabel2"
        Me.BsLabel2.Size = New System.Drawing.Size(19, 28)
        Me.BsLabel2.TabIndex = 53
        Me.BsLabel2.Text = "2-"
        Me.BsLabel2.Title = False
        '
        'bsChangeRotorLabel
        '
        Me.bsChangeRotorLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsChangeRotorLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsChangeRotorLabel.ForeColor = System.Drawing.Color.Black
        Me.bsChangeRotorLabel.Location = New System.Drawing.Point(30, 58)
        Me.bsChangeRotorLabel.Name = "bsChangeRotorLabel"
        Me.bsChangeRotorLabel.Size = New System.Drawing.Size(283, 28)
        Me.bsChangeRotorLabel.TabIndex = 49
        Me.bsChangeRotorLabel.Text = "Press button and remove current rotor "
        Me.bsChangeRotorLabel.Title = False
        '
        'BsLabel1
        '
        Me.BsLabel1.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel1.ForeColor = System.Drawing.Color.Black
        Me.BsLabel1.Location = New System.Drawing.Point(13, 58)
        Me.BsLabel1.Name = "BsLabel1"
        Me.BsLabel1.Size = New System.Drawing.Size(19, 28)
        Me.BsLabel1.TabIndex = 52
        Me.BsLabel1.Text = "1-"
        Me.BsLabel1.Title = False
        '
        'bsStatusImage
        '
        Me.bsStatusImage.BackColor = System.Drawing.Color.Transparent
        Me.bsStatusImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsStatusImage.Location = New System.Drawing.Point(276, 138)
        Me.bsStatusImage.Name = "bsStatusImage"
        Me.bsStatusImage.Size = New System.Drawing.Size(24, 24)
        Me.bsStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsStatusImage.TabIndex = 25
        Me.bsStatusImage.TabStop = False
        Me.bsStatusImage.Visible = False
        '
        'bsNewAdjLabel
        '
        Me.bsNewAdjLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNewAdjLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNewAdjLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNewAdjLabel.Location = New System.Drawing.Point(30, 97)
        Me.bsNewAdjLabel.Name = "bsNewAdjLabel"
        Me.bsNewAdjLabel.Size = New System.Drawing.Size(279, 28)
        Me.bsNewAdjLabel.TabIndex = 50
        Me.bsNewAdjLabel.Text = "Put new rotor and press button to perform Light Adjustment"
        Me.bsNewAdjLabel.Title = False
        '
        'bsNewRotorButton
        '
        Me.bsNewRotorButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.bsNewRotorButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsNewRotorButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsNewRotorButton.Location = New System.Drawing.Point(348, 94)
        Me.bsNewRotorButton.Name = "bsNewRotorButton"
        Me.bsNewRotorButton.Size = New System.Drawing.Size(32, 32)
        Me.bsNewRotorButton.TabIndex = 4
        Me.bsNewRotorButton.UseVisualStyleBackColor = True
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
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(416, 229)
        Me.BsBorderedPanel1.TabIndex = 33
        '
        'BsProgressBar
        '
        Me.BsProgressBar.Location = New System.Drawing.Point(72, 143)
        Me.BsProgressBar.Name = "BsProgressBar"
        Me.BsProgressBar.Size = New System.Drawing.Size(189, 16)
        Me.BsProgressBar.TabIndex = 34
        Me.BsProgressBar.Visible = False
        '
        'BsNewRotorProcessTimer
        '
        Me.BsNewRotorProcessTimer.Interval = 1000
        '
        'IChangeRotorSRV
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.CancelButton = Me.bsExitButton
        Me.ClientSize = New System.Drawing.Size(416, 229)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsChangeRotorGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IChangeRotorSRV"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsChangeRotorGroupBox.ResumeLayout(False)
        CType(Me.bsStatusImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsWSUpDownButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsChangeRotorGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsNewRotorButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsChangeRotorLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsNewAdjLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel2 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
    Friend WithEvents bsStatusImage As System.Windows.Forms.PictureBox
    Friend WithEvents BsProgressBar As Biosystems.Ax00.Controls.UserControls.BSProgressBar
    Friend WithEvents BsNewRotorProcessTimer As System.Windows.Forms.Timer
End Class
