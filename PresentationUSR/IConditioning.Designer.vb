<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IConditioning
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IConditioning))
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsConditioningButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLoadSaveGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsStatusImage = New System.Windows.Forms.PictureBox
        Me.bsConditioningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.dxProgressBar = New DevExpress.XtraEditors.ProgressBarControl
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.BsBorderedPanel1 = New bsBorderedPanel
        Me.bsLoadSaveGroupBox.SuspendLayout()
        CType(Me.bsStatusImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dxProgressBar.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsCancelButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCancelButton.Location = New System.Drawing.Point(364, 149)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 3
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsConditioningButton
        '
        Me.bsConditioningButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.bsConditioningButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsConditioningButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsConditioningButton.Location = New System.Drawing.Point(354, 53)
        Me.bsConditioningButton.Name = "bsConditioningButton"
        Me.bsConditioningButton.Size = New System.Drawing.Size(32, 32)
        Me.bsConditioningButton.TabIndex = 2
        Me.bsConditioningButton.UseVisualStyleBackColor = True
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
        Me.bsTitleLabel.Text = "*Conditioning Utility"
        Me.bsTitleLabel.Title = True
        '
        'bsLoadSaveGroupBox
        '
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsStatusImage)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsConditioningLabel)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.dxProgressBar)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsConditioningButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsLoadSaveGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLoadSaveGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsLoadSaveGroupBox.Name = "bsLoadSaveGroupBox"
        Me.bsLoadSaveGroupBox.Size = New System.Drawing.Size(397, 131)
        Me.bsLoadSaveGroupBox.TabIndex = 14
        Me.bsLoadSaveGroupBox.TabStop = False
        '
        'bsStatusImage
        '
        Me.bsStatusImage.BackColor = System.Drawing.Color.Transparent
        Me.bsStatusImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsStatusImage.Location = New System.Drawing.Point(286, 91)
        Me.bsStatusImage.Name = "bsStatusImage"
        Me.bsStatusImage.Size = New System.Drawing.Size(24, 24)
        Me.bsStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.bsStatusImage.TabIndex = 52
        Me.bsStatusImage.TabStop = False
        Me.bsStatusImage.Visible = False
        '
        'bsConditioningLabel
        '
        Me.bsConditioningLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsConditioningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsConditioningLabel.ForeColor = System.Drawing.Color.Black
        Me.bsConditioningLabel.Location = New System.Drawing.Point(10, 57)
        Me.bsConditioningLabel.Name = "bsConditioningLabel"
        Me.bsConditioningLabel.Size = New System.Drawing.Size(283, 28)
        Me.bsConditioningLabel.TabIndex = 49
        Me.bsConditioningLabel.Text = "*Perform conditioning of the analyzer"
        Me.bsConditioningLabel.Title = False
        '
        'dxProgressBar
        '
        Me.dxProgressBar.Location = New System.Drawing.Point(99, 95)
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
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(416, 190)
        Me.BsBorderedPanel1.TabIndex = 33
        '
        'IConditioning
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
        Me.ClientSize = New System.Drawing.Size(416, 190)
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
        Me.Name = "IConditioning"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsLoadSaveGroupBox.ResumeLayout(False)
        CType(Me.bsStatusImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dxProgressBar.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsConditioningButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLoadSaveGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsConditioningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents dxProgressBar As DevExpress.XtraEditors.ProgressBarControl
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
    Friend WithEvents bsStatusImage As System.Windows.Forms.PictureBox
End Class
