<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IResultsRecover
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IResultsRecover))
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLoadSaveGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.dxProgressBar = New DevExpress.XtraEditors.MarqueeProgressBarControl
        Me.bsRecoverLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.BsBorderedPanel1 = New bsBorderedPanel
        Me.bsLoadSaveGroupBox.SuspendLayout()
        CType(Me.dxProgressBar.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
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
        Me.bsTitleLabel.Text = "*Recovering Results"
        Me.bsTitleLabel.Title = True
        '
        'bsLoadSaveGroupBox
        '
        Me.bsLoadSaveGroupBox.Controls.Add(Me.dxProgressBar)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsRecoverLabel)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsLoadSaveGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLoadSaveGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsLoadSaveGroupBox.Name = "bsLoadSaveGroupBox"
        Me.bsLoadSaveGroupBox.Size = New System.Drawing.Size(397, 131)
        Me.bsLoadSaveGroupBox.TabIndex = 14
        Me.bsLoadSaveGroupBox.TabStop = False
        '
        'dxProgressBar
        '
        Me.dxProgressBar.EditValue = "Loading, please wait..."
        Me.dxProgressBar.Location = New System.Drawing.Point(108, 88)
        Me.dxProgressBar.Name = "dxProgressBar"
        Me.dxProgressBar.Properties.LookAndFeel.SkinName = "Money Twins"
        Me.dxProgressBar.Properties.LookAndFeel.UseDefaultLookAndFeel = False
        Me.dxProgressBar.ShowToolTips = False
        Me.dxProgressBar.Size = New System.Drawing.Size(170, 18)
        Me.dxProgressBar.TabIndex = 53
        Me.dxProgressBar.UseWaitCursor = True
        '
        'bsRecoverLabel
        '
        Me.bsRecoverLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRecoverLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRecoverLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRecoverLabel.Location = New System.Drawing.Point(10, 57)
        Me.bsRecoverLabel.Name = "bsRecoverLabel"
        Me.bsRecoverLabel.Size = New System.Drawing.Size(283, 28)
        Me.bsRecoverLabel.TabIndex = 49
        Me.bsRecoverLabel.Text = "*Recovering lost results. Please wait"
        Me.bsRecoverLabel.Title = False
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
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(416, 160)
        Me.BsBorderedPanel1.TabIndex = 33
        '
        'IResultsRecover
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(416, 160)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLoadSaveGroupBox)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IResultsRecover"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsLoadSaveGroupBox.ResumeLayout(False)
        CType(Me.dxProgressBar.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLoadSaveGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsRecoverLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
    Friend WithEvents dxProgressBar As DevExpress.XtraEditors.MarqueeProgressBarControl
End Class
