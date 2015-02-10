

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiConfigLanguage
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiConfigLanguage))
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsLanguagesLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLangComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsLangConfigTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLanguageSelectionGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.BsBorderedPanel1 = New bsBorderedPanel
        Me.bsLanguageSelectionGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCancelButton.Location = New System.Drawing.Point(269, 118)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 2
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAcceptButton.Location = New System.Drawing.Point(232, 118)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 1
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'bsLanguagesLabel
        '
        Me.bsLanguagesLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLanguagesLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLanguagesLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLanguagesLabel.Location = New System.Drawing.Point(10, 41)
        Me.bsLanguagesLabel.Name = "bsLanguagesLabel"
        Me.bsLanguagesLabel.Size = New System.Drawing.Size(271, 13)
        Me.bsLanguagesLabel.TabIndex = 1
        Me.bsLanguagesLabel.Text = "Select the application language:"
        Me.bsLanguagesLabel.Title = False
        '
        'bsLangComboBox
        '
        Me.bsLangComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsLangComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLangComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsLangComboBox.FormattingEnabled = True
        Me.bsLangComboBox.Location = New System.Drawing.Point(10, 59)
        Me.bsLangComboBox.Name = "bsLangComboBox"
        Me.bsLangComboBox.Size = New System.Drawing.Size(271, 21)
        Me.bsLangComboBox.TabIndex = 0
        '
        'bsLangConfigTitleLabel
        '
        Me.bsLangConfigTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsLangConfigTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsLangConfigTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLangConfigTitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsLangConfigTitleLabel.Name = "bsLangConfigTitleLabel"
        Me.bsLangConfigTitleLabel.Size = New System.Drawing.Size(271, 20)
        Me.bsLangConfigTitleLabel.TabIndex = 0
        Me.bsLangConfigTitleLabel.Text = "Language Configuration"
        Me.bsLangConfigTitleLabel.Title = True
        '
        'bsLanguageSelectionGroupBox
        '
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsLangConfigTitleLabel)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsLangComboBox)
        Me.bsLanguageSelectionGroupBox.Controls.Add(Me.bsLanguagesLabel)
        Me.bsLanguageSelectionGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLanguageSelectionGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsLanguageSelectionGroupBox.Name = "bsLanguageSelectionGroupBox"
        Me.bsLanguageSelectionGroupBox.Size = New System.Drawing.Size(291, 95)
        Me.bsLanguageSelectionGroupBox.TabIndex = 5
        Me.bsLanguageSelectionGroupBox.TabStop = False
        '
        'BsBorderedPanel1
        '
        Me.BsBorderedPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsBorderedPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsBorderedPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsBorderedPanel1.Name = "BsBorderedPanel1"
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(312, 162)
        Me.BsBorderedPanel1.TabIndex = 6
        '
        'IConfigLanguage
        '
        Me.AcceptButton = Me.bsCancelButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(312, 162)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLanguageSelectionGroupBox)
        Me.Controls.Add(Me.bsCancelButton)
        Me.Controls.Add(Me.bsAcceptButton)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IConfigLanguage"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        Me.bsLanguageSelectionGroupBox.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLanguagesLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLangComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsLangConfigTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLanguageSelectionGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
End Class
