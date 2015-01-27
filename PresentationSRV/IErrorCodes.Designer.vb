<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiErrorCodes
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
        Me.bsLanguageSelectionGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.BsRichTextBox1 = New Biosystems.Ax00.Controls.UserControls.BSRichTextBox
        Me.BsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsClearButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsLanguageSelectionGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsLanguageSelectionGroupBox
        '
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.BsRichTextBox1)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.BsTitleLabel)
        Me.bsLanguageSelectionGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.bsLanguageSelectionGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLanguageSelectionGroupBox.Location = New System.Drawing.Point(12, 12)
        Me.bsLanguageSelectionGroupBox.Name = "bsLanguageSelectionGroupBox"
        Me.bsLanguageSelectionGroupBox.Size = New System.Drawing.Size(511, 460)
        Me.bsLanguageSelectionGroupBox.TabIndex = 65
        Me.bsLanguageSelectionGroupBox.TabStop = False
        '
        'BsRichTextBox1
        '
        Me.BsRichTextBox1.BackColor = System.Drawing.SystemColors.Window
        Me.BsRichTextBox1.Location = New System.Drawing.Point(10, 49)
        Me.BsRichTextBox1.Name = "BsRichTextBox1"
        Me.BsRichTextBox1.ReadOnly = True
        Me.BsRichTextBox1.Size = New System.Drawing.Size(487, 405)
        Me.BsRichTextBox1.TabIndex = 115
        Me.BsRichTextBox1.Text = ""
        '
        'BsTitleLabel
        '
        Me.BsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTitleLabel.Location = New System.Drawing.Point(6, 17)
        Me.BsTitleLabel.Name = "BsTitleLabel"
        Me.BsTitleLabel.Size = New System.Drawing.Size(491, 20)
        Me.BsTitleLabel.TabIndex = 60
        Me.BsTitleLabel.Text = "Error Codes"
        Me.BsTitleLabel.Title = True
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Location = New System.Drawing.Point(491, 478)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 63
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsClearButton
        '
        Me.bsClearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsClearButton.Location = New System.Drawing.Point(453, 478)
        Me.bsClearButton.Name = "bsClearButton"
        Me.bsClearButton.Size = New System.Drawing.Size(32, 32)
        Me.bsClearButton.TabIndex = 64
        Me.bsClearButton.UseVisualStyleBackColor = True
        '
        'IErrorCodes
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(533, 519)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLanguageSelectionGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsClearButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IErrorCodes"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation
        Me.Text = " "
        Me.bsLanguageSelectionGroupBox.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsLanguageSelectionGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsClearButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsRichTextBox1 As Biosystems.Ax00.Controls.UserControls.BSRichTextBox
End Class
