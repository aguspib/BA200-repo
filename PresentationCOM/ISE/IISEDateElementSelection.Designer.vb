<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IISEDateElementSelection
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsChangePwdGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsLiCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsLiDateTimePicker = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.BsClCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsClDateTimePicker = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.BsKCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsKDateTimePicker = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.BsNaCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsNaDateTimePicker = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.bsRefCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsRefDateTimePicker = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker()
        Me.BsLabel5 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDateSelectionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLabel2 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLabel3 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLabel4 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.bsChangePwdGroupBox.SuspendLayout()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsExitButton
        '
        Me.bsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(228, 311)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 6
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.bsAcceptButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAcceptButton.Location = New System.Drawing.Point(191, 311)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 5
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'bsChangePwdGroupBox
        '
        Me.bsChangePwdGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.bsChangePwdGroupBox.Controls.Add(Me.BsLiCheckbox)
        Me.bsChangePwdGroupBox.Controls.Add(Me.BsLiDateTimePicker)
        Me.bsChangePwdGroupBox.Controls.Add(Me.BsClCheckbox)
        Me.bsChangePwdGroupBox.Controls.Add(Me.BsClDateTimePicker)
        Me.bsChangePwdGroupBox.Controls.Add(Me.BsKCheckbox)
        Me.bsChangePwdGroupBox.Controls.Add(Me.BsKDateTimePicker)
        Me.bsChangePwdGroupBox.Controls.Add(Me.BsNaCheckbox)
        Me.bsChangePwdGroupBox.Controls.Add(Me.BsNaDateTimePicker)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsRefCheckBox)
        Me.bsChangePwdGroupBox.Controls.Add(Me.BsRefDateTimePicker)
        Me.bsChangePwdGroupBox.Controls.Add(Me.BsLabel5)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsDateSelectionLabel)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsLabel2)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsLabel3)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsLabel4)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsLabel1)
        Me.bsChangePwdGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsChangePwdGroupBox.Location = New System.Drawing.Point(10, 7)
        Me.bsChangePwdGroupBox.Name = "bsChangePwdGroupBox"
        Me.bsChangePwdGroupBox.Size = New System.Drawing.Size(250, 294)
        Me.bsChangePwdGroupBox.TabIndex = 0
        Me.bsChangePwdGroupBox.TabStop = False
        '
        'BsLiCheckbox
        '
        Me.BsLiCheckbox.AutoSize = True
        Me.BsLiCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsLiCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsLiCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsLiCheckbox.Location = New System.Drawing.Point(13, 194)
        Me.BsLiCheckbox.Name = "BsLiCheckbox"
        Me.BsLiCheckbox.Size = New System.Drawing.Size(15, 14)
        Me.BsLiCheckbox.TabIndex = 21
        Me.BsLiCheckbox.UseVisualStyleBackColor = False
        '
        'BsLiDateTimePicker
        '
        Me.BsLiDateTimePicker.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLiDateTimePicker.CalendarForeColor = System.Drawing.Color.Black
        Me.BsLiDateTimePicker.CalendarMonthBackground = System.Drawing.Color.White
        Me.BsLiDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.BsLiDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.BsLiDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.BsLiDateTimePicker.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLiDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.BsLiDateTimePicker.Location = New System.Drawing.Point(122, 189)
        Me.BsLiDateTimePicker.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.BsLiDateTimePicker.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.BsLiDateTimePicker.Name = "BsLiDateTimePicker"
        Me.BsLiDateTimePicker.Size = New System.Drawing.Size(91, 21)
        Me.BsLiDateTimePicker.TabIndex = 20
        '
        'BsClCheckbox
        '
        Me.BsClCheckbox.AutoSize = True
        Me.BsClCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsClCheckbox.Checked = True
        Me.BsClCheckbox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.BsClCheckbox.Enabled = False
        Me.BsClCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsClCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsClCheckbox.Location = New System.Drawing.Point(13, 162)
        Me.BsClCheckbox.Name = "BsClCheckbox"
        Me.BsClCheckbox.Size = New System.Drawing.Size(15, 14)
        Me.BsClCheckbox.TabIndex = 19
        Me.BsClCheckbox.UseVisualStyleBackColor = False
        Me.BsClCheckbox.Visible = False
        '
        'BsClDateTimePicker
        '
        Me.BsClDateTimePicker.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsClDateTimePicker.CalendarForeColor = System.Drawing.Color.Black
        Me.BsClDateTimePicker.CalendarMonthBackground = System.Drawing.Color.White
        Me.BsClDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.BsClDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.BsClDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.BsClDateTimePicker.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsClDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.BsClDateTimePicker.Location = New System.Drawing.Point(122, 156)
        Me.BsClDateTimePicker.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.BsClDateTimePicker.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.BsClDateTimePicker.Name = "BsClDateTimePicker"
        Me.BsClDateTimePicker.Size = New System.Drawing.Size(91, 21)
        Me.BsClDateTimePicker.TabIndex = 18
        '
        'BsKCheckbox
        '
        Me.BsKCheckbox.AutoSize = True
        Me.BsKCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsKCheckbox.Checked = True
        Me.BsKCheckbox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.BsKCheckbox.Enabled = False
        Me.BsKCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsKCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsKCheckbox.Location = New System.Drawing.Point(13, 127)
        Me.BsKCheckbox.Name = "BsKCheckbox"
        Me.BsKCheckbox.Size = New System.Drawing.Size(15, 14)
        Me.BsKCheckbox.TabIndex = 17
        Me.BsKCheckbox.UseVisualStyleBackColor = False
        Me.BsKCheckbox.Visible = False
        '
        'BsKDateTimePicker
        '
        Me.BsKDateTimePicker.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsKDateTimePicker.CalendarForeColor = System.Drawing.Color.Black
        Me.BsKDateTimePicker.CalendarMonthBackground = System.Drawing.Color.White
        Me.BsKDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.BsKDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.BsKDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.BsKDateTimePicker.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsKDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.BsKDateTimePicker.Location = New System.Drawing.Point(122, 121)
        Me.BsKDateTimePicker.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.BsKDateTimePicker.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.BsKDateTimePicker.Name = "BsKDateTimePicker"
        Me.BsKDateTimePicker.Size = New System.Drawing.Size(91, 21)
        Me.BsKDateTimePicker.TabIndex = 16
        '
        'BsNaCheckbox
        '
        Me.BsNaCheckbox.AutoSize = True
        Me.BsNaCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsNaCheckbox.Checked = True
        Me.BsNaCheckbox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.BsNaCheckbox.Enabled = False
        Me.BsNaCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsNaCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsNaCheckbox.Location = New System.Drawing.Point(13, 93)
        Me.BsNaCheckbox.Name = "BsNaCheckbox"
        Me.BsNaCheckbox.Size = New System.Drawing.Size(15, 14)
        Me.BsNaCheckbox.TabIndex = 15
        Me.BsNaCheckbox.UseVisualStyleBackColor = False
        Me.BsNaCheckbox.Visible = False
        '
        'BsNaDateTimePicker
        '
        Me.BsNaDateTimePicker.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsNaDateTimePicker.CalendarForeColor = System.Drawing.Color.Black
        Me.BsNaDateTimePicker.CalendarMonthBackground = System.Drawing.Color.White
        Me.BsNaDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.BsNaDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.BsNaDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.BsNaDateTimePicker.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsNaDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.BsNaDateTimePicker.Location = New System.Drawing.Point(122, 87)
        Me.BsNaDateTimePicker.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.BsNaDateTimePicker.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.BsNaDateTimePicker.Name = "BsNaDateTimePicker"
        Me.BsNaDateTimePicker.Size = New System.Drawing.Size(91, 21)
        Me.BsNaDateTimePicker.TabIndex = 14
        '
        'bsRefCheckBox
        '
        Me.bsRefCheckBox.AutoSize = True
        Me.bsRefCheckBox.BackColor = System.Drawing.Color.Gainsboro
        Me.bsRefCheckBox.Checked = True
        Me.bsRefCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.bsRefCheckBox.Enabled = False
        Me.bsRefCheckBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsRefCheckBox.ForeColor = System.Drawing.Color.Black
        Me.bsRefCheckBox.Location = New System.Drawing.Point(13, 60)
        Me.bsRefCheckBox.Name = "bsRefCheckBox"
        Me.bsRefCheckBox.Size = New System.Drawing.Size(15, 14)
        Me.bsRefCheckBox.TabIndex = 13
        Me.bsRefCheckBox.UseVisualStyleBackColor = False
        Me.bsRefCheckBox.Visible = False
        '
        'BsRefDateTimePicker
        '
        Me.BsRefDateTimePicker.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsRefDateTimePicker.CalendarForeColor = System.Drawing.Color.Black
        Me.BsRefDateTimePicker.CalendarMonthBackground = System.Drawing.Color.White
        Me.BsRefDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.BsRefDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.BsRefDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.BsRefDateTimePicker.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsRefDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.BsRefDateTimePicker.Location = New System.Drawing.Point(122, 54)
        Me.BsRefDateTimePicker.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.BsRefDateTimePicker.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.BsRefDateTimePicker.Name = "BsRefDateTimePicker"
        Me.BsRefDateTimePicker.Size = New System.Drawing.Size(91, 21)
        Me.BsRefDateTimePicker.TabIndex = 12
        '
        'BsLabel5
        '
        Me.BsLabel5.AutoSize = True
        Me.BsLabel5.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel5.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel5.ForeColor = System.Drawing.Color.Black
        Me.BsLabel5.Location = New System.Drawing.Point(34, 193)
        Me.BsLabel5.Name = "BsLabel5"
        Me.BsLabel5.Size = New System.Drawing.Size(25, 13)
        Me.BsLabel5.TabIndex = 11
        Me.BsLabel5.Text = "Li+"
        Me.BsLabel5.Title = False
        '
        'bsDateSelectionLabel
        '
        Me.bsDateSelectionLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsDateSelectionLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsDateSelectionLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDateSelectionLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsDateSelectionLabel.Name = "bsDateSelectionLabel"
        Me.bsDateSelectionLabel.Size = New System.Drawing.Size(230, 20)
        Me.bsDateSelectionLabel.TabIndex = 2
        Me.bsDateSelectionLabel.Text = "Date Selection"
        Me.bsDateSelectionLabel.Title = True
        '
        'bsLabel2
        '
        Me.bsLabel2.AutoSize = True
        Me.bsLabel2.BackColor = System.Drawing.Color.Transparent
        Me.bsLabel2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.bsLabel2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLabel2.ForeColor = System.Drawing.Color.Black
        Me.bsLabel2.Location = New System.Drawing.Point(31, 91)
        Me.bsLabel2.Name = "bsLabel2"
        Me.bsLabel2.Size = New System.Drawing.Size(31, 13)
        Me.bsLabel2.TabIndex = 8
        Me.bsLabel2.Text = "Na+"
        Me.bsLabel2.Title = False
        '
        'bsLabel3
        '
        Me.bsLabel3.AutoSize = True
        Me.bsLabel3.BackColor = System.Drawing.Color.Transparent
        Me.bsLabel3.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLabel3.ForeColor = System.Drawing.Color.Black
        Me.bsLabel3.Location = New System.Drawing.Point(31, 125)
        Me.bsLabel3.Name = "bsLabel3"
        Me.bsLabel3.Size = New System.Drawing.Size(24, 13)
        Me.bsLabel3.TabIndex = 9
        Me.bsLabel3.Text = "K+"
        Me.bsLabel3.Title = False
        '
        'bsLabel4
        '
        Me.bsLabel4.AutoSize = True
        Me.bsLabel4.BackColor = System.Drawing.Color.Transparent
        Me.bsLabel4.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLabel4.ForeColor = System.Drawing.Color.Black
        Me.bsLabel4.Location = New System.Drawing.Point(31, 160)
        Me.bsLabel4.Name = "bsLabel4"
        Me.bsLabel4.Size = New System.Drawing.Size(24, 13)
        Me.bsLabel4.TabIndex = 10
        Me.bsLabel4.Text = "Cl-"
        Me.bsLabel4.Title = False
        '
        'bsLabel1
        '
        Me.bsLabel1.BackColor = System.Drawing.Color.Transparent
        Me.bsLabel1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLabel1.ForeColor = System.Drawing.Color.Black
        Me.bsLabel1.Location = New System.Drawing.Point(31, 56)
        Me.bsLabel1.Name = "bsLabel1"
        Me.bsLabel1.Size = New System.Drawing.Size(54, 47)
        Me.bsLabel1.TabIndex = 3
        Me.bsLabel1.Text = "Ref."
        Me.bsLabel1.Title = False
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'IISEDateElementSelection
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
        Me.ClientSize = New System.Drawing.Size(274, 352)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsAcceptButton)
        Me.Controls.Add(Me.bsChangePwdGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IISEDateElementSelection"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsChangePwdGroupBox.ResumeLayout(False)
        Me.bsChangePwdGroupBox.PerformLayout()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsChangePwdGroupBox As Biosystems.Ax00.Controls.Usercontrols.BSGroupBox
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsDateSelectionLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsLabel1 As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsLabel4 As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsLabel3 As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsLabel2 As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents BsLabel5 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsRefDateTimePicker As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents bsRefCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsNaCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsNaDateTimePicker As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents BsLiCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsLiDateTimePicker As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents BsClCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsClDateTimePicker As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents BsKCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsKDateTimePicker As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
End Class
