<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiLotChangeAuxScreen
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiLotChangeAuxScreen))
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsLotNumberTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLoadSaveGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsSaveDataCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bsPreviousLotDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsSavedLotRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.bsNewLotRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.bsLoadSaveGroupBox.SuspendLayout()
        CType(Me.bsPreviousLotDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsLotNumberTextBox
        '
        Me.bsLotNumberTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsLotNumberTextBox.DecimalsValues = False
        Me.bsLotNumberTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLotNumberTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsLotNumberTextBox.IsNumeric = False
        Me.bsLotNumberTextBox.Location = New System.Drawing.Point(32, 78)
        Me.bsLotNumberTextBox.Mandatory = True
        Me.bsLotNumberTextBox.MaxLength = 16
        Me.bsLotNumberTextBox.Name = "bsLotNumberTextBox"
        Me.bsLotNumberTextBox.Size = New System.Drawing.Size(196, 21)
        Me.bsLotNumberTextBox.TabIndex = 1
        Me.bsLotNumberTextBox.WordWrap = False
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsCancelButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCancelButton.Location = New System.Drawing.Point(399, 259)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 3
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAcceptButton.Location = New System.Drawing.Point(362, 259)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 2
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'bsTitleLabel
        '
        Me.bsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsTitleLabel.Name = "bsTitleLabel"
        Me.bsTitleLabel.Size = New System.Drawing.Size(399, 20)
        Me.bsTitleLabel.TabIndex = 8
        Me.bsTitleLabel.Text = "*Lot change"
        Me.bsTitleLabel.Title = True
        '
        'bsLoadSaveGroupBox
        '
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsSaveDataCheckBox)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsPreviousLotDataGridView)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsSavedLotRadioButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsLotNumberTextBox)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsNewLotRadioButton)
        Me.bsLoadSaveGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsLoadSaveGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLoadSaveGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsLoadSaveGroupBox.Name = "bsLoadSaveGroupBox"
        Me.bsLoadSaveGroupBox.Size = New System.Drawing.Size(421, 243)
        Me.bsLoadSaveGroupBox.TabIndex = 14
        Me.bsLoadSaveGroupBox.TabStop = False
        '
        'bsSaveDataCheckBox
        '
        Me.bsSaveDataCheckBox.AutoSize = True
        Me.bsSaveDataCheckBox.Checked = True
        Me.bsSaveDataCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.bsSaveDataCheckBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSaveDataCheckBox.Location = New System.Drawing.Point(10, 216)
        Me.bsSaveDataCheckBox.Name = "bsSaveDataCheckBox"
        Me.bsSaveDataCheckBox.Size = New System.Drawing.Size(208, 17)
        Me.bsSaveDataCheckBox.TabIndex = 15
        Me.bsSaveDataCheckBox.Text = "*Save data of the current LOT: "
        Me.bsSaveDataCheckBox.UseVisualStyleBackColor = True
        '
        'bsPreviousLotDataGridView
        '
        Me.bsPreviousLotDataGridView.AllowUserToAddRows = False
        Me.bsPreviousLotDataGridView.AllowUserToDeleteRows = False
        Me.bsPreviousLotDataGridView.AllowUserToResizeColumns = False
        Me.bsPreviousLotDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPreviousLotDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsPreviousLotDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsPreviousLotDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsPreviousLotDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPreviousLotDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsPreviousLotDataGridView.ColumnHeadersHeight = 20
        Me.bsPreviousLotDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPreviousLotDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsPreviousLotDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsPreviousLotDataGridView.EnterToTab = False
        Me.bsPreviousLotDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsPreviousLotDataGridView.Location = New System.Drawing.Point(32, 137)
        Me.bsPreviousLotDataGridView.MultiSelect = False
        Me.bsPreviousLotDataGridView.Name = "bsPreviousLotDataGridView"
        Me.bsPreviousLotDataGridView.ReadOnly = True
        Me.bsPreviousLotDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPreviousLotDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsPreviousLotDataGridView.RowHeadersVisible = False
        Me.bsPreviousLotDataGridView.RowHeadersWidth = 20
        Me.bsPreviousLotDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPreviousLotDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsPreviousLotDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsPreviousLotDataGridView.ShowRowErrors = False
        Me.bsPreviousLotDataGridView.Size = New System.Drawing.Size(377, 59)
        Me.bsPreviousLotDataGridView.TabIndex = 55
        Me.bsPreviousLotDataGridView.TabToEnter = False
        '
        'bsSavedLotRadioButton
        '
        Me.bsSavedLotRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSavedLotRadioButton.Location = New System.Drawing.Point(10, 119)
        Me.bsSavedLotRadioButton.Name = "bsSavedLotRadioButton"
        Me.bsSavedLotRadioButton.Size = New System.Drawing.Size(123, 17)
        Me.bsSavedLotRadioButton.TabIndex = 16
        Me.bsSavedLotRadioButton.Text = "*Saved Lot"
        Me.bsSavedLotRadioButton.UseVisualStyleBackColor = True
        '
        'bsNewLotRadioButton
        '
        Me.bsNewLotRadioButton.Checked = True
        Me.bsNewLotRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsNewLotRadioButton.Location = New System.Drawing.Point(10, 60)
        Me.bsNewLotRadioButton.Name = "bsNewLotRadioButton"
        Me.bsNewLotRadioButton.Size = New System.Drawing.Size(123, 17)
        Me.bsNewLotRadioButton.TabIndex = 15
        Me.bsNewLotRadioButton.TabStop = True
        Me.bsNewLotRadioButton.Text = "*New Lot"
        Me.bsNewLotRadioButton.UseVisualStyleBackColor = True
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'ILotChangeAuxScreen
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.CancelButton = Me.bsCancelButton
        Me.ClientSize = New System.Drawing.Size(443, 303)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLoadSaveGroupBox)
        Me.Controls.Add(Me.bsCancelButton)
        Me.Controls.Add(Me.bsAcceptButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ILotChangeAuxScreen"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsLoadSaveGroupBox.ResumeLayout(False)
        Me.bsLoadSaveGroupBox.PerformLayout()
        CType(Me.bsPreviousLotDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsLotNumberTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLoadSaveGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsNewLotRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsSavedLotRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsPreviousLotDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsSaveDataCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
End Class
