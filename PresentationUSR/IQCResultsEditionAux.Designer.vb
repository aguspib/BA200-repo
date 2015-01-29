<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiQCResultsEditionAux
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiQCResultsEditionAux))
        Me.bsResultEditionGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsExcludeResultCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bsRemarksLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsRemarkTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsMeasureUnitLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsResultValueTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsResultLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsRunNumberTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsControlNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsControlLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLotNumberLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLotNumberTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsTestSampleTypeTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsTestSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsResultEditionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.myToolTipsControl = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.myErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.bsResultEditionGroupBox.SuspendLayout()
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsResultEditionGroupBox
        '
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsExcludeResultCheckBox)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsRemarksLabel)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsRemarkTextBox)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsMeasureUnitLabel)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsResultValueTextBox)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsResultLabel)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsRunNumberTextBox)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsControlNameTextBox)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsControlLabel)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsLotNumberLabel)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsLotNumberTextBox)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsTestSampleTypeTextBox)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsTestSampleTypeLabel)
        Me.bsResultEditionGroupBox.Controls.Add(Me.bsResultEditionLabel)
        Me.bsResultEditionGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsResultEditionGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsResultEditionGroupBox.Name = "bsResultEditionGroupBox"
        Me.bsResultEditionGroupBox.Size = New System.Drawing.Size(515, 265)
        Me.bsResultEditionGroupBox.TabIndex = 0
        Me.bsResultEditionGroupBox.TabStop = False
        '
        'bsExcludeResultCheckBox
        '
        Me.bsExcludeResultCheckBox.Location = New System.Drawing.Point(10, 236)
        Me.bsExcludeResultCheckBox.Name = "bsExcludeResultCheckBox"
        Me.bsExcludeResultCheckBox.Size = New System.Drawing.Size(495, 17)
        Me.bsExcludeResultCheckBox.TabIndex = 6
        Me.bsExcludeResultCheckBox.Text = "*Exclude Result from statistics"
        Me.bsExcludeResultCheckBox.UseVisualStyleBackColor = True
        '
        'bsRemarksLabel
        '
        Me.bsRemarksLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRemarksLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRemarksLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRemarksLabel.Location = New System.Drawing.Point(10, 148)
        Me.bsRemarksLabel.Name = "bsRemarksLabel"
        Me.bsRemarksLabel.Size = New System.Drawing.Size(495, 13)
        Me.bsRemarksLabel.TabIndex = 12
        Me.bsRemarksLabel.Text = "*Comments:"
        Me.bsRemarksLabel.Title = False
        '
        'bsRemarkTextBox
        '
        Me.bsRemarkTextBox.BackColor = System.Drawing.Color.White
        Me.bsRemarkTextBox.DecimalsValues = False
        Me.bsRemarkTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRemarkTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsRemarkTextBox.IsNumeric = False
        Me.bsRemarkTextBox.Location = New System.Drawing.Point(10, 166)
        Me.bsRemarkTextBox.Mandatory = False
        Me.bsRemarkTextBox.MaxLength = 255
        Me.bsRemarkTextBox.Multiline = True
        Me.bsRemarkTextBox.Name = "bsRemarkTextBox"
        Me.bsRemarkTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.bsRemarkTextBox.Size = New System.Drawing.Size(495, 65)
        Me.bsRemarkTextBox.TabIndex = 5
        '
        'bsMeasureUnitLabel
        '
        Me.bsMeasureUnitLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsMeasureUnitLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsMeasureUnitLabel.ForeColor = System.Drawing.Color.Black
        Me.bsMeasureUnitLabel.Location = New System.Drawing.Point(441, 122)
        Me.bsMeasureUnitLabel.Name = "bsMeasureUnitLabel"
        Me.bsMeasureUnitLabel.Size = New System.Drawing.Size(63, 21)
        Me.bsMeasureUnitLabel.TabIndex = 10
        Me.bsMeasureUnitLabel.Text = "*U/L"
        Me.bsMeasureUnitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsMeasureUnitLabel.Title = False
        '
        'bsResultValueTextBox
        '
        Me.bsResultValueTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsResultValueTextBox.DecimalsValues = True
        Me.bsResultValueTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsResultValueTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsResultValueTextBox.IsNumeric = True
        Me.bsResultValueTextBox.Location = New System.Drawing.Point(335, 122)
        Me.bsResultValueTextBox.Mandatory = True
        Me.bsResultValueTextBox.Name = "bsResultValueTextBox"
        Me.bsResultValueTextBox.ShortcutsEnabled = False
        Me.bsResultValueTextBox.Size = New System.Drawing.Size(100, 21)
        Me.bsResultValueTextBox.TabIndex = 4
        Me.bsResultValueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.bsResultValueTextBox.WordWrap = False
        '
        'bsResultLabel
        '
        Me.bsResultLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsResultLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsResultLabel.ForeColor = System.Drawing.Color.Black
        Me.bsResultLabel.Location = New System.Drawing.Point(285, 104)
        Me.bsResultLabel.Name = "bsResultLabel"
        Me.bsResultLabel.Size = New System.Drawing.Size(220, 13)
        Me.bsResultLabel.TabIndex = 8
        Me.bsResultLabel.Text = "*Result:"
        Me.bsResultLabel.Title = False
        '
        'bsRunNumberTextBox
        '
        Me.bsRunNumberTextBox.BackColor = System.Drawing.Color.White
        Me.bsRunNumberTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsRunNumberTextBox.DecimalsValues = False
        Me.bsRunNumberTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRunNumberTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsRunNumberTextBox.IsNumeric = False
        Me.bsRunNumberTextBox.Location = New System.Drawing.Point(285, 122)
        Me.bsRunNumberTextBox.Mandatory = False
        Me.bsRunNumberTextBox.Name = "bsRunNumberTextBox"
        Me.bsRunNumberTextBox.ReadOnly = True
        Me.bsRunNumberTextBox.Size = New System.Drawing.Size(45, 21)
        Me.bsRunNumberTextBox.TabIndex = 3
        Me.bsRunNumberTextBox.TabStop = False
        Me.bsRunNumberTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'bsControlNameTextBox
        '
        Me.bsControlNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsControlNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsControlNameTextBox.DecimalsValues = False
        Me.bsControlNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsControlNameTextBox.IsNumeric = False
        Me.bsControlNameTextBox.Location = New System.Drawing.Point(285, 78)
        Me.bsControlNameTextBox.Mandatory = False
        Me.bsControlNameTextBox.Name = "bsControlNameTextBox"
        Me.bsControlNameTextBox.ReadOnly = True
        Me.bsControlNameTextBox.Size = New System.Drawing.Size(220, 21)
        Me.bsControlNameTextBox.TabIndex = 1
        Me.bsControlNameTextBox.TabStop = False
        '
        'bsControlLabel
        '
        Me.bsControlLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsControlLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsControlLabel.ForeColor = System.Drawing.Color.Black
        Me.bsControlLabel.Location = New System.Drawing.Point(284, 60)
        Me.bsControlLabel.Name = "bsControlLabel"
        Me.bsControlLabel.Size = New System.Drawing.Size(220, 13)
        Me.bsControlLabel.TabIndex = 5
        Me.bsControlLabel.Text = "*Control Name:"
        Me.bsControlLabel.Title = False
        '
        'bsLotNumberLabel
        '
        Me.bsLotNumberLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLotNumberLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLotNumberLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLotNumberLabel.Location = New System.Drawing.Point(10, 104)
        Me.bsLotNumberLabel.Name = "bsLotNumberLabel"
        Me.bsLotNumberLabel.Size = New System.Drawing.Size(220, 13)
        Me.bsLotNumberLabel.TabIndex = 4
        Me.bsLotNumberLabel.Text = "*Lot Number:"
        Me.bsLotNumberLabel.Title = False
        '
        'bsLotNumberTextBox
        '
        Me.bsLotNumberTextBox.BackColor = System.Drawing.Color.White
        Me.bsLotNumberTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsLotNumberTextBox.DecimalsValues = False
        Me.bsLotNumberTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLotNumberTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsLotNumberTextBox.IsNumeric = False
        Me.bsLotNumberTextBox.Location = New System.Drawing.Point(10, 122)
        Me.bsLotNumberTextBox.Mandatory = False
        Me.bsLotNumberTextBox.Name = "bsLotNumberTextBox"
        Me.bsLotNumberTextBox.ReadOnly = True
        Me.bsLotNumberTextBox.Size = New System.Drawing.Size(220, 21)
        Me.bsLotNumberTextBox.TabIndex = 2
        Me.bsLotNumberTextBox.TabStop = False
        '
        'bsTestSampleTypeTextBox
        '
        Me.bsTestSampleTypeTextBox.BackColor = System.Drawing.Color.White
        Me.bsTestSampleTypeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsTestSampleTypeTextBox.DecimalsValues = False
        Me.bsTestSampleTypeTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestSampleTypeTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestSampleTypeTextBox.IsNumeric = False
        Me.bsTestSampleTypeTextBox.Location = New System.Drawing.Point(10, 78)
        Me.bsTestSampleTypeTextBox.Mandatory = False
        Me.bsTestSampleTypeTextBox.Name = "bsTestSampleTypeTextBox"
        Me.bsTestSampleTypeTextBox.ReadOnly = True
        Me.bsTestSampleTypeTextBox.Size = New System.Drawing.Size(220, 21)
        Me.bsTestSampleTypeTextBox.TabIndex = 0
        Me.bsTestSampleTypeTextBox.TabStop = False
        '
        'bsTestSampleTypeLabel
        '
        Me.bsTestSampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTestSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestSampleTypeLabel.Location = New System.Drawing.Point(10, 60)
        Me.bsTestSampleTypeLabel.Name = "bsTestSampleTypeLabel"
        Me.bsTestSampleTypeLabel.Size = New System.Drawing.Size(220, 13)
        Me.bsTestSampleTypeLabel.TabIndex = 1
        Me.bsTestSampleTypeLabel.Text = "*Test/Sample Type:"
        Me.bsTestSampleTypeLabel.Title = False
        '
        'bsResultEditionLabel
        '
        Me.bsResultEditionLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsResultEditionLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsResultEditionLabel.ForeColor = System.Drawing.Color.Black
        Me.bsResultEditionLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsResultEditionLabel.Name = "bsResultEditionLabel"
        Me.bsResultEditionLabel.Size = New System.Drawing.Size(495, 20)
        Me.bsResultEditionLabel.TabIndex = 0
        Me.bsResultEditionLabel.Text = "*Result Edition"
        Me.bsResultEditionLabel.Title = True
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Location = New System.Drawing.Point(495, 280)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 8
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.Location = New System.Drawing.Point(458, 280)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 7
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'myErrorProvider
        '
        Me.myErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.myErrorProvider.ContainerControl = Me
        '
        'IQCResultsEditionAux
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.ClientSize = New System.Drawing.Size(537, 319)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsAcceptButton)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsResultEditionGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiQCResultsEditionAux"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        Me.bsResultEditionGroupBox.ResumeLayout(False)
        Me.bsResultEditionGroupBox.PerformLayout()
        CType(Me.myErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsResultEditionGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLotNumberLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLotNumberTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsTestSampleTypeTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsTestSampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsResultEditionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsMeasureUnitLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsResultValueTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsResultLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRunNumberTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsControlNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsControlLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRemarkTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsExcludeResultCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsRemarksLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents myToolTipsControl As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents myErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider

End Class
