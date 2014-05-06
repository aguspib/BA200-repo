<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ISettings
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ISettings))
        Me.bsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.BsButton1 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.LastButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.NextButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.FilterComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.BsShowLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.CloseButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsLoadSaveGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.PrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.SettingsListView = New Biosystems.Ax00.Controls.UserControls.BSListView
        Me.ParametersDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsSettingsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.LimitsDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.SaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.CancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.EditButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.BsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsFwVersionTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsLoadSaveGroupBox.SuspendLayout()
        CType(Me.ParametersDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LimitsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsErrorProvider1
        '
        Me.bsErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsErrorProvider1.ContainerControl = Me
        '
        'BsButton1
        '
        Me.BsButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton1.Location = New System.Drawing.Point(659, 891)
        Me.BsButton1.Name = "BsButton1"
        Me.BsButton1.Size = New System.Drawing.Size(32, 32)
        Me.BsButton1.TabIndex = 171
        Me.BsButton1.UseVisualStyleBackColor = True
        '
        'LastButton
        '
        Me.LastButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.LastButton.Location = New System.Drawing.Point(697, 892)
        Me.LastButton.Name = "LastButton"
        Me.LastButton.Size = New System.Drawing.Size(32, 32)
        Me.LastButton.TabIndex = 169
        Me.LastButton.UseVisualStyleBackColor = True
        '
        'NextButton
        '
        Me.NextButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.NextButton.Location = New System.Drawing.Point(204, 892)
        Me.NextButton.Name = "NextButton"
        Me.NextButton.Size = New System.Drawing.Size(32, 32)
        Me.NextButton.TabIndex = 170
        Me.NextButton.UseVisualStyleBackColor = True
        '
        'FilterComboBox
        '
        Me.FilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.FilterComboBox.ForeColor = System.Drawing.Color.Black
        Me.FilterComboBox.FormattingEnabled = True
        Me.FilterComboBox.Location = New System.Drawing.Point(139, 50)
        Me.FilterComboBox.Name = "FilterComboBox"
        Me.FilterComboBox.Size = New System.Drawing.Size(97, 21)
        Me.FilterComboBox.TabIndex = 77
        '
        'BsShowLabel
        '
        Me.BsShowLabel.AutoSize = True
        Me.BsShowLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsShowLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsShowLabel.ForeColor = System.Drawing.Color.Black
        Me.BsShowLabel.Location = New System.Drawing.Point(9, 53)
        Me.BsShowLabel.Name = "BsShowLabel"
        Me.BsShowLabel.Size = New System.Drawing.Size(106, 13)
        Me.BsShowLabel.TabIndex = 75
        Me.BsShowLabel.Text = "*Analyzer Model:"
        Me.BsShowLabel.Title = False
        '
        'CloseButton
        '
        Me.CloseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CloseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.CloseButton.Location = New System.Drawing.Point(923, 614)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(32, 32)
        Me.CloseButton.TabIndex = 168
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'bsLoadSaveGroupBox
        '
        Me.bsLoadSaveGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.BsFwVersionTextBox)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.BsLabel1)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.PrintButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.SettingsListView)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.ParametersDataGridView)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsSettingsLabel)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.LimitsDataGridView)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.SaveButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.CancelButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.EditButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.FilterComboBox)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.BsShowLabel)
        Me.bsLoadSaveGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLoadSaveGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsLoadSaveGroupBox.Name = "bsLoadSaveGroupBox"
        Me.bsLoadSaveGroupBox.Size = New System.Drawing.Size(956, 594)
        Me.bsLoadSaveGroupBox.TabIndex = 172
        Me.bsLoadSaveGroupBox.TabStop = False
        '
        'PrintButton
        '
        Me.PrintButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PrintButton.Location = New System.Drawing.Point(244, 552)
        Me.PrintButton.Name = "PrintButton"
        Me.PrintButton.Size = New System.Drawing.Size(32, 32)
        Me.PrintButton.TabIndex = 173
        Me.PrintButton.UseVisualStyleBackColor = True
        Me.PrintButton.Visible = False
        '
        'SettingsListView
        '
        Me.SettingsListView.AllowColumnReorder = True
        Me.SettingsListView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.SettingsListView.AutoArrange = False
        Me.SettingsListView.BackColor = System.Drawing.Color.White
        Me.SettingsListView.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.SettingsListView.ForeColor = System.Drawing.Color.Black
        Me.SettingsListView.FullRowSelect = True
        Me.SettingsListView.HideSelection = False
        Me.SettingsListView.Location = New System.Drawing.Point(12, 77)
        Me.SettingsListView.Name = "SettingsListView"
        Me.SettingsListView.Size = New System.Drawing.Size(224, 465)
        Me.SettingsListView.TabIndex = 176
        Me.SettingsListView.UseCompatibleStateImageBehavior = False
        Me.SettingsListView.View = System.Windows.Forms.View.Details
        '
        'ParametersDataGridView
        '
        Me.ParametersDataGridView.AllowUserToAddRows = False
        Me.ParametersDataGridView.AllowUserToDeleteRows = False
        Me.ParametersDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ParametersDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.ParametersDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ParametersDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.ParametersDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ParametersDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ParametersDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.ParametersDataGridView.ColumnHeadersHeight = 20
        Me.ParametersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ParametersDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.ParametersDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.ParametersDataGridView.EnterToTab = True
        Me.ParametersDataGridView.GridColor = System.Drawing.Color.Silver
        Me.ParametersDataGridView.Location = New System.Drawing.Point(245, 77)
        Me.ParametersDataGridView.Name = "ParametersDataGridView"
        Me.ParametersDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ParametersDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.ParametersDataGridView.RowHeadersVisible = False
        Me.ParametersDataGridView.RowHeadersWidth = 20
        Me.ParametersDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ParametersDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.ParametersDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.ParametersDataGridView.Size = New System.Drawing.Size(697, 465)
        Me.ParametersDataGridView.TabIndex = 178
        Me.ParametersDataGridView.TabToEnter = False
        '
        'bsSettingsLabel
        '
        Me.bsSettingsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSettingsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsSettingsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSettingsLabel.Location = New System.Drawing.Point(9, 17)
        Me.bsSettingsLabel.Name = "bsSettingsLabel"
        Me.bsSettingsLabel.Size = New System.Drawing.Size(933, 20)
        Me.bsSettingsLabel.TabIndex = 1
        Me.bsSettingsLabel.Text = "*Settings"
        Me.bsSettingsLabel.Title = True
        '
        'LimitsDataGridView
        '
        Me.LimitsDataGridView.AllowUserToAddRows = False
        Me.LimitsDataGridView.AllowUserToDeleteRows = False
        Me.LimitsDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle6.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LimitsDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle6
        Me.LimitsDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.LimitsDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.LimitsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LimitsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle7.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LimitsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.LimitsDataGridView.ColumnHeadersHeight = 20
        Me.LimitsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LimitsDataGridView.DefaultCellStyle = DataGridViewCellStyle8
        Me.LimitsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.LimitsDataGridView.EnterToTab = True
        Me.LimitsDataGridView.GridColor = System.Drawing.Color.Silver
        Me.LimitsDataGridView.Location = New System.Drawing.Point(245, 77)
        Me.LimitsDataGridView.Name = "LimitsDataGridView"
        Me.LimitsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle9.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LimitsDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.LimitsDataGridView.RowHeadersVisible = False
        Me.LimitsDataGridView.RowHeadersWidth = 20
        Me.LimitsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle10.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LimitsDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle10
        Me.LimitsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.LimitsDataGridView.Size = New System.Drawing.Size(697, 465)
        Me.LimitsDataGridView.TabIndex = 177
        Me.LimitsDataGridView.TabToEnter = False
        '
        'SaveButton
        '
        Me.SaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.SaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.SaveButton.Location = New System.Drawing.Point(872, 552)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(32, 32)
        Me.SaveButton.TabIndex = 175
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'CancelButton
        '
        Me.CancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.CancelButton.Location = New System.Drawing.Point(910, 552)
        Me.CancelButton.Name = "CancelButton"
        Me.CancelButton.Size = New System.Drawing.Size(32, 32)
        Me.CancelButton.TabIndex = 174
        Me.CancelButton.UseVisualStyleBackColor = True
        '
        'EditButton
        '
        Me.EditButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.EditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.EditButton.Location = New System.Drawing.Point(204, 552)
        Me.EditButton.Name = "EditButton"
        Me.EditButton.Size = New System.Drawing.Size(32, 32)
        Me.EditButton.TabIndex = 173
        Me.EditButton.UseVisualStyleBackColor = True
        '
        'BsLabel1
        '
        Me.BsLabel1.AutoSize = True
        Me.BsLabel1.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel1.ForeColor = System.Drawing.Color.Black
        Me.BsLabel1.Location = New System.Drawing.Point(724, 53)
        Me.BsLabel1.Name = "BsLabel1"
        Me.BsLabel1.Size = New System.Drawing.Size(112, 13)
        Me.BsLabel1.TabIndex = 179
        Me.BsLabel1.Text = "Firmware Version:"
        Me.BsLabel1.Title = False
        '
        'BsFwVersionTextBox
        '
        Me.BsFwVersionTextBox.DecimalsValues = False
        Me.BsFwVersionTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsFwVersionTextBox.IsNumeric = False
        Me.BsFwVersionTextBox.Location = New System.Drawing.Point(842, 50)
        Me.BsFwVersionTextBox.Mandatory = False
        Me.BsFwVersionTextBox.Name = "BsFwVersionTextBox"
        Me.BsFwVersionTextBox.Size = New System.Drawing.Size(100, 21)
        Me.BsFwVersionTextBox.TabIndex = 180
        Me.BsFwVersionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ISettings
        '
        Me.AcceptButton = Me.CloseButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLoadSaveGroupBox)
        Me.Controls.Add(Me.BsButton1)
        Me.Controls.Add(Me.LastButton)
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.NextButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ISettings"
        Me.ShowInTaskbar = False
        Me.Text = " "
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsLoadSaveGroupBox.ResumeLayout(False)
        Me.bsLoadSaveGroupBox.PerformLayout()
        CType(Me.ParametersDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LimitsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents BsShowLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents CloseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents LastButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents NextButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents FilterComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsButton1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLoadSaveGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents SaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents CancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents EditButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSettingsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents SettingsListView As Biosystems.Ax00.Controls.UserControls.BSListView
    Friend WithEvents LimitsDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents ParametersDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents PrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsFwVersionTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
End Class
