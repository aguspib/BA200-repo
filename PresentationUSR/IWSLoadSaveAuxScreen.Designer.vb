<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiWSLoadSaveAuxScreen
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiWSLoadSaveAuxScreen))
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsElementTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsElementsComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsNameSelectionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLoadSaveGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsLoadSaveGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsElementTextBox
        '
        Me.bsElementTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsElementTextBox.DecimalsValues = False
        Me.bsElementTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsElementTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsElementTextBox.IsNumeric = False
        Me.bsElementTextBox.Location = New System.Drawing.Point(10, 59)
        Me.bsElementTextBox.Mandatory = True
        Me.bsElementTextBox.MaxLength = 30
        Me.bsElementTextBox.Name = "bsElementTextBox"
        Me.bsElementTextBox.Size = New System.Drawing.Size(270, 21)
        Me.bsElementTextBox.TabIndex = 1
        Me.bsElementTextBox.WordWrap = False
        '
        'bsElementsComboBox
        '
        Me.bsElementsComboBox.Cursor = System.Windows.Forms.Cursors.Default
        Me.bsElementsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsElementsComboBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsElementsComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsElementsComboBox.FormattingEnabled = True
        Me.bsElementsComboBox.Location = New System.Drawing.Point(10, 59)
        Me.bsElementsComboBox.Name = "bsElementsComboBox"
        Me.bsElementsComboBox.Size = New System.Drawing.Size(270, 21)
        Me.bsElementsComboBox.TabIndex = 0
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(268, 103)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 3
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAcceptButton.Location = New System.Drawing.Point(231, 103)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 2
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'bsNameSelectionLabel
        '
        Me.bsNameSelectionLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNameSelectionLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNameSelectionLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNameSelectionLabel.Location = New System.Drawing.Point(10, 41)
        Me.bsNameSelectionLabel.Name = "bsNameSelectionLabel"
        Me.bsNameSelectionLabel.Size = New System.Drawing.Size(270, 13)
        Me.bsNameSelectionLabel.TabIndex = 9
        Me.bsNameSelectionLabel.Text = "Virtual Rotor Name:"
        Me.bsNameSelectionLabel.Title = False
        '
        'bsTitleLabel
        '
        Me.bsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsTitleLabel.Name = "bsTitleLabel"
        Me.bsTitleLabel.Size = New System.Drawing.Size(270, 20)
        Me.bsTitleLabel.TabIndex = 8
        Me.bsTitleLabel.Text = "Virtual Rotor Selection"
        Me.bsTitleLabel.Title = True
        '
        'bsLoadSaveGroupBox
        '
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsElementTextBox)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsElementsComboBox)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsNameSelectionLabel)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsLoadSaveGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLoadSaveGroupBox.Location = New System.Drawing.Point(10, 3)
        Me.bsLoadSaveGroupBox.Name = "bsLoadSaveGroupBox"
        Me.bsLoadSaveGroupBox.Size = New System.Drawing.Size(290, 93)
        Me.bsLoadSaveGroupBox.TabIndex = 14
        Me.bsLoadSaveGroupBox.TabStop = False
        '
        'IWSLoadSaveAuxScreen
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.CancelButton = Me.bsExitButton
        Me.ClientSize = New System.Drawing.Size(311, 145)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLoadSaveGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsAcceptButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IWSLoadSaveAuxScreen"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsLoadSaveGroupBox.ResumeLayout(False)
        Me.bsLoadSaveGroupBox.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsElementTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsElementsComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsNameSelectionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLoadSaveGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
End Class
