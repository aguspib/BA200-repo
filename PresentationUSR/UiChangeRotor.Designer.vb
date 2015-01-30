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
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsChangeRotortButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLoadSaveGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.BsLabel2 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsChangeRotorLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.dxProgressBar = New DevExpress.XtraEditors.ProgressBarControl
        Me.bsStatusImage = New System.Windows.Forms.PictureBox
        Me.bsNewAdjLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsContinueButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.BsBorderedPanel1 = New bsBorderedPanel
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
        Me.bsCancelButton.Location = New System.Drawing.Point(358, 189)
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
        Me.bsLoadSaveGroupBox.Controls.Add(Me.BsLabel2)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsChangeRotorLabel)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.BsLabel1)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.dxProgressBar)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsStatusImage)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsNewAdjLabel)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsChangeRotortButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsContinueButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsLoadSaveGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLoadSaveGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsLoadSaveGroupBox.Name = "bsLoadSaveGroupBox"
        Me.bsLoadSaveGroupBox.Size = New System.Drawing.Size(397, 170)
        Me.bsLoadSaveGroupBox.TabIndex = 14
        Me.bsLoadSaveGroupBox.TabStop = False
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
        'dxProgressBar
        '
        Me.dxProgressBar.Location = New System.Drawing.Point(89, 141)
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
        'bsContinueButton
        '
        Me.bsContinueButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.bsContinueButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsContinueButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsContinueButton.Location = New System.Drawing.Point(348, 94)
        Me.bsContinueButton.Name = "bsContinueButton"
        Me.bsContinueButton.Size = New System.Drawing.Size(32, 32)
        Me.bsContinueButton.TabIndex = 4
        Me.bsContinueButton.UseVisualStyleBackColor = True
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
        Me.ClientSize = New System.Drawing.Size(416, 229)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLoadSaveGroupBox)
        Me.Controls.Add(Me.bsCancelButton)
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
    Friend WithEvents BsLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel2 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
End Class
