<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiWSTestSelectionWarning
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                ReleaseElements()
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiWSTestSelectionWarning))
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsNotPosWarningGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.MemoEdit1 = New DevExpress.XtraEditors.MemoEdit
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsNotPosWarningGroupBox.SuspendLayout()
        CType(Me.MemoEdit1.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsExitButton.ForeColor = System.Drawing.Color.Black
        Me.bsExitButton.Location = New System.Drawing.Point(455, 455)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 50
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsNotPosWarningGroupBox
        '
        Me.bsNotPosWarningGroupBox.Controls.Add(Me.MemoEdit1)
        Me.bsNotPosWarningGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsNotPosWarningGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsNotPosWarningGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsNotPosWarningGroupBox.Name = "bsNotPosWarningGroupBox"
        Me.bsNotPosWarningGroupBox.Size = New System.Drawing.Size(477, 439)
        Me.bsNotPosWarningGroupBox.TabIndex = 53
        Me.bsNotPosWarningGroupBox.TabStop = False
        '
        'MemoEdit1
        '
        Me.MemoEdit1.Location = New System.Drawing.Point(10, 43)
        Me.MemoEdit1.Name = "MemoEdit1"
        Me.MemoEdit1.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
        Me.MemoEdit1.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MemoEdit1.Properties.Appearance.Options.UseBackColor = True
        Me.MemoEdit1.Properties.Appearance.Options.UseFont = True
        Me.MemoEdit1.Properties.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MemoEdit1.Properties.LookAndFeel.UseWindowsXPTheme = True
        Me.MemoEdit1.Properties.ReadOnly = True
        Me.MemoEdit1.Size = New System.Drawing.Size(456, 388)
        Me.MemoEdit1.TabIndex = 48
        '
        'bsTitleLabel
        '
        Me.bsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsTitleLabel.Name = "bsTitleLabel"
        Me.bsTitleLabel.Size = New System.Drawing.Size(456, 20)
        Me.bsTitleLabel.TabIndex = 46
        Me.bsTitleLabel.Text = "*Positioning Warnings"
        Me.bsTitleLabel.Title = True
        '
        'IWSTestSelectionWarning
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.CancelButton = Me.bsExitButton
        Me.ClientSize = New System.Drawing.Size(497, 494)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsNotPosWarningGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiWSTestSelectionWarning"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsNotPosWarningGroupBox.ResumeLayout(False)
        CType(Me.MemoEdit1.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsNotPosWarningGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents MemoEdit1 As DevExpress.XtraEditors.MemoEdit
End Class
