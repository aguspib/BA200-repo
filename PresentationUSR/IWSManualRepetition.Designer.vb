<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IWSManualRepetition
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IWSManualRepetition))
        Me.bsScreenToolTips = New System.Windows.Forms.ToolTip(Me.components)
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsManualRepetitionTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLanguageSelectionGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsNoRepeat = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.BsDecrease = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.BsEqual = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.BsIncrease = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.bsLanguageSelectionGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCancelButton.Location = New System.Drawing.Point(258, 172)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 4
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(221, 172)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 3
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsManualRepetitionTitleLabel
        '
        Me.bsManualRepetitionTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsManualRepetitionTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsManualRepetitionTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsManualRepetitionTitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsManualRepetitionTitleLabel.Name = "bsManualRepetitionTitleLabel"
        Me.bsManualRepetitionTitleLabel.Size = New System.Drawing.Size(260, 20)
        Me.bsManualRepetitionTitleLabel.TabIndex = 0
        Me.bsManualRepetitionTitleLabel.Text = "Select Repetition Criterion"
        Me.bsManualRepetitionTitleLabel.Title = True
        '
        'bsLanguageSelectionGroupBox
        '
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.BsNoRepeat)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.BsDecrease)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.BsEqual)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.BsIncrease)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsManualRepetitionTitleLabel)
        Me.bsLanguageSelectionGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLanguageSelectionGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsLanguageSelectionGroupBox.Name = "bsLanguageSelectionGroupBox"
        Me.bsLanguageSelectionGroupBox.Size = New System.Drawing.Size(280, 157)
        Me.bsLanguageSelectionGroupBox.TabIndex = 5
        Me.bsLanguageSelectionGroupBox.TabStop = False
        '
        'BsNoRepeat
        '
        Me.BsNoRepeat.AutoSize = True
        Me.BsNoRepeat.Location = New System.Drawing.Point(13, 116)
        Me.BsNoRepeat.Name = "BsNoRepeat"
        Me.BsNoRepeat.Size = New System.Drawing.Size(104, 17)
        Me.BsNoRepeat.TabIndex = 4
        Me.BsNoRepeat.Text = "Do not repeat"
        Me.BsNoRepeat.UseVisualStyleBackColor = True
        '
        'BsDecrease
        '
        Me.BsDecrease.AutoSize = True
        Me.BsDecrease.Location = New System.Drawing.Point(13, 93)
        Me.BsDecrease.Name = "BsDecrease"
        Me.BsDecrease.Size = New System.Drawing.Size(133, 17)
        Me.BsDecrease.TabIndex = 3
        Me.BsDecrease.Text = "Repeat Decreasing"
        Me.BsDecrease.UseVisualStyleBackColor = True
        '
        'BsEqual
        '
        Me.BsEqual.AutoSize = True
        Me.BsEqual.Checked = True
        Me.BsEqual.Location = New System.Drawing.Point(13, 47)
        Me.BsEqual.Name = "BsEqual"
        Me.BsEqual.Size = New System.Drawing.Size(100, 17)
        Me.BsEqual.TabIndex = 1
        Me.BsEqual.TabStop = True
        Me.BsEqual.Text = "Repeat equal"
        Me.BsEqual.UseVisualStyleBackColor = True
        '
        'BsIncrease
        '
        Me.BsIncrease.AutoSize = True
        Me.BsIncrease.Location = New System.Drawing.Point(13, 70)
        Me.BsIncrease.Name = "BsIncrease"
        Me.BsIncrease.Size = New System.Drawing.Size(129, 17)
        Me.BsIncrease.TabIndex = 2
        Me.BsIncrease.Text = "Repeat Increasing"
        Me.BsIncrease.UseVisualStyleBackColor = True
        '
        'IWSManualRepetition
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(300, 212)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLanguageSelectionGroupBox)
        Me.Controls.Add(Me.bsCancelButton)
        Me.Controls.Add(Me.bsExitButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IWSManualRepetition"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        Me.bsLanguageSelectionGroupBox.ResumeLayout(False)
        Me.bsLanguageSelectionGroupBox.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsScreenToolTips As System.Windows.Forms.ToolTip
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsManualRepetitionTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLanguageSelectionGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsNoRepeat As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents BsDecrease As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents BsEqual As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents BsIncrease As Biosystems.Ax00.Controls.UserControls.BSRadioButton
End Class
