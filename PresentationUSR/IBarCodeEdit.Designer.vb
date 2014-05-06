<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IBarCodeEdit
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IBarCodeEdit))
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsMinMaxValueLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsEnterBarcodeGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsElementTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsEnterBarcodeGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(301, 130)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 3
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.bsAcceptButton.Enabled = False
        Me.bsAcceptButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAcceptButton.Location = New System.Drawing.Point(264, 130)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 2
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'bsMinMaxValueLabel
        '
        Me.bsMinMaxValueLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsMinMaxValueLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsMinMaxValueLabel.ForeColor = System.Drawing.Color.Black
        Me.bsMinMaxValueLabel.Location = New System.Drawing.Point(10, 60)
        Me.bsMinMaxValueLabel.Name = "bsMinMaxValueLabel"
        Me.bsMinMaxValueLabel.Size = New System.Drawing.Size(323, 13)
        Me.bsMinMaxValueLabel.TabIndex = 9
        Me.bsMinMaxValueLabel.Text = "*Flexible Barcode size between:"
        Me.bsMinMaxValueLabel.Title = False
        '
        'bsTitleLabel
        '
        Me.bsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsTitleLabel.Name = "bsTitleLabel"
        Me.bsTitleLabel.Size = New System.Drawing.Size(306, 20)
        Me.bsTitleLabel.TabIndex = 8
        Me.bsTitleLabel.Text = "Enter Bar Code"
        Me.bsTitleLabel.Title = True
        '
        'bsEnterBarcodeGroupBox
        '
        Me.bsEnterBarcodeGroupBox.Controls.Add(Me.bsElementTextBox)
        Me.bsEnterBarcodeGroupBox.Controls.Add(Me.bsMinMaxValueLabel)
        Me.bsEnterBarcodeGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsEnterBarcodeGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsEnterBarcodeGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsEnterBarcodeGroupBox.Name = "bsEnterBarcodeGroupBox"
        Me.bsEnterBarcodeGroupBox.Size = New System.Drawing.Size(326, 114)
        Me.bsEnterBarcodeGroupBox.TabIndex = 14
        Me.bsEnterBarcodeGroupBox.TabStop = False
        '
        'bsElementTextBox
        '
        Me.bsElementTextBox.BackColor = System.Drawing.Color.White
        Me.bsElementTextBox.DecimalsValues = False
        Me.bsElementTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsElementTextBox.IsNumeric = False
        Me.bsElementTextBox.Location = New System.Drawing.Point(7, 77)
        Me.bsElementTextBox.Mandatory = False
        Me.bsElementTextBox.Name = "bsElementTextBox"
        Me.bsElementTextBox.Size = New System.Drawing.Size(309, 21)
        Me.bsElementTextBox.TabIndex = 10
        '
        'IBarCodeEdit
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
        Me.ClientSize = New System.Drawing.Size(345, 167)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsEnterBarcodeGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsAcceptButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IBarCodeEdit"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsEnterBarcodeGroupBox.ResumeLayout(False)
        Me.bsEnterBarcodeGroupBox.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsMinMaxValueLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsEnterBarcodeGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsElementTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
End Class
