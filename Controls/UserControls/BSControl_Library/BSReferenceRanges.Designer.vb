<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BSReferenceRanges
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.BsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsRefBorderMaxLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsRefBorderMinLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDetailedCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bsGenericCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bsDetailedSubTypesComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsRefDetailDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsRefDetailDeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsRefNormalUpperLimitUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.bsRefBorderLineLowerLimitUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.bsRefBorderLineUpperLimitUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.bsRefNormalLowerLimitUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.bsRefNormalMaxLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsRefNormalMinLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsBorderLineUnitLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsBorderLineLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsGenericNormalUnitLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsRefGenericNormalLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsControlErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.bsControlToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.BsPanel.SuspendLayout()
        CType(Me.bsRefDetailDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsRefNormalUpperLimitUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsRefBorderLineLowerLimitUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsRefBorderLineUpperLimitUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsRefNormalLowerLimitUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsControlErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsPanel
        '
        Me.BsPanel.Controls.Add(Me.bsRefBorderMaxLabel)
        Me.BsPanel.Controls.Add(Me.bsRefBorderMinLabel)
        Me.BsPanel.Controls.Add(Me.bsDetailedCheckBox)
        Me.BsPanel.Controls.Add(Me.bsGenericCheckBox)
        Me.BsPanel.Controls.Add(Me.bsDetailedSubTypesComboBox)
        Me.BsPanel.Controls.Add(Me.bsRefDetailDataGridView)
        Me.BsPanel.Controls.Add(Me.bsRefDetailDeleteButton)
        Me.BsPanel.Controls.Add(Me.bsRefNormalUpperLimitUpDown)
        Me.BsPanel.Controls.Add(Me.bsRefBorderLineLowerLimitUpDown)
        Me.BsPanel.Controls.Add(Me.bsRefBorderLineUpperLimitUpDown)
        Me.BsPanel.Controls.Add(Me.bsRefNormalLowerLimitUpDown)
        Me.BsPanel.Controls.Add(Me.bsRefNormalMaxLabel)
        Me.BsPanel.Controls.Add(Me.bsRefNormalMinLabel)
        Me.BsPanel.Controls.Add(Me.bsBorderLineUnitLabel)
        Me.BsPanel.Controls.Add(Me.bsBorderLineLabel)
        Me.BsPanel.Controls.Add(Me.bsGenericNormalUnitLabel)
        Me.BsPanel.Controls.Add(Me.bsRefGenericNormalLabel)
        Me.BsPanel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsPanel.Name = "BsPanel"
        Me.BsPanel.Size = New System.Drawing.Size(648, 225)
        Me.BsPanel.TabIndex = 0
        '
        'bsRefBorderMaxLabel
        '
        Me.bsRefBorderMaxLabel.AutoSize = True
        Me.bsRefBorderMaxLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRefBorderMaxLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRefBorderMaxLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRefBorderMaxLabel.Location = New System.Drawing.Point(512, 22)
        Me.bsRefBorderMaxLabel.Name = "bsRefBorderMaxLabel"
        Me.bsRefBorderMaxLabel.Size = New System.Drawing.Size(62, 13)
        Me.bsRefBorderMaxLabel.TabIndex = 225
        Me.bsRefBorderMaxLabel.Text = "Maximum"
        Me.bsRefBorderMaxLabel.Title = False
        '
        'bsRefBorderMinLabel
        '
        Me.bsRefBorderMinLabel.AutoSize = True
        Me.bsRefBorderMinLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRefBorderMinLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRefBorderMinLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRefBorderMinLabel.Location = New System.Drawing.Point(415, 22)
        Me.bsRefBorderMinLabel.Name = "bsRefBorderMinLabel"
        Me.bsRefBorderMinLabel.Size = New System.Drawing.Size(58, 13)
        Me.bsRefBorderMinLabel.TabIndex = 224
        Me.bsRefBorderMinLabel.Text = "Minimum"
        Me.bsRefBorderMinLabel.Title = False
        '
        'bsDetailedCheckBox
        '
        Me.bsDetailedCheckBox.AutoSize = True
        Me.bsDetailedCheckBox.Location = New System.Drawing.Point(5, 69)
        Me.bsDetailedCheckBox.Name = "bsDetailedCheckBox"
        Me.bsDetailedCheckBox.Size = New System.Drawing.Size(73, 17)
        Me.bsDetailedCheckBox.TabIndex = 223
        Me.bsDetailedCheckBox.Text = "Detailed"
        Me.bsDetailedCheckBox.UseVisualStyleBackColor = True
        '
        'bsGenericCheckBox
        '
        Me.bsGenericCheckBox.AutoSize = True
        Me.bsGenericCheckBox.Location = New System.Drawing.Point(5, 5)
        Me.bsGenericCheckBox.Name = "bsGenericCheckBox"
        Me.bsGenericCheckBox.Size = New System.Drawing.Size(70, 17)
        Me.bsGenericCheckBox.TabIndex = 222
        Me.bsGenericCheckBox.Text = "Generic"
        Me.bsGenericCheckBox.UseVisualStyleBackColor = True
        '
        'bsDetailedSubTypesComboBox
        '
        Me.bsDetailedSubTypesComboBox.BackColor = System.Drawing.Color.White
        Me.bsDetailedSubTypesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsDetailedSubTypesComboBox.Enabled = False
        Me.bsDetailedSubTypesComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsDetailedSubTypesComboBox.FormattingEnabled = True
        Me.bsDetailedSubTypesComboBox.Location = New System.Drawing.Point(25, 89)
        Me.bsDetailedSubTypesComboBox.Name = "bsDetailedSubTypesComboBox"
        Me.bsDetailedSubTypesComboBox.Size = New System.Drawing.Size(159, 21)
        Me.bsDetailedSubTypesComboBox.TabIndex = 221
        '
        'bsRefDetailDataGridView
        '
        Me.bsRefDetailDataGridView.AllowUserToAddRows = False
        Me.bsRefDetailDataGridView.AllowUserToDeleteRows = False
        Me.bsRefDetailDataGridView.AllowUserToResizeColumns = False
        Me.bsRefDetailDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsRefDetailDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsRefDetailDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsRefDetailDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsRefDetailDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsRefDetailDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsRefDetailDataGridView.ColumnHeadersHeight = 20
        Me.bsRefDetailDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsRefDetailDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsRefDetailDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsRefDetailDataGridView.EnterToTab = True
        Me.bsRefDetailDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsRefDetailDataGridView.Location = New System.Drawing.Point(25, 113)
        Me.bsRefDetailDataGridView.Name = "bsRefDetailDataGridView"
        Me.bsRefDetailDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsRefDetailDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsRefDetailDataGridView.RowHeadersWidth = 20
        Me.bsRefDetailDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsRefDetailDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsRefDetailDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsRefDetailDataGridView.Size = New System.Drawing.Size(573, 98)
        Me.bsRefDetailDataGridView.TabIndex = 6
        Me.bsRefDetailDataGridView.TabToEnter = False
        '
        'bsRefDetailDeleteButton
        '
        Me.bsRefDetailDeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsRefDetailDeleteButton.Enabled = False
        Me.bsRefDetailDeleteButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsRefDetailDeleteButton.Location = New System.Drawing.Point(604, 113)
        Me.bsRefDetailDeleteButton.Name = "bsRefDetailDeleteButton"
        Me.bsRefDetailDeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.bsRefDetailDeleteButton.TabIndex = 8
        Me.bsRefDetailDeleteButton.UseVisualStyleBackColor = True
        '
        'bsRefNormalUpperLimitUpDown
        '
        Me.bsRefNormalUpperLimitUpDown.BackColor = System.Drawing.Color.White
        Me.bsRefNormalUpperLimitUpDown.Enabled = False
        Me.bsRefNormalUpperLimitUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsRefNormalUpperLimitUpDown.Location = New System.Drawing.Point(202, 39)
        Me.bsRefNormalUpperLimitUpDown.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.bsRefNormalUpperLimitUpDown.Name = "bsRefNormalUpperLimitUpDown"
        Me.bsRefNormalUpperLimitUpDown.Size = New System.Drawing.Size(70, 21)
        Me.bsRefNormalUpperLimitUpDown.TabIndex = 2
        Me.bsRefNormalUpperLimitUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsRefBorderLineLowerLimitUpDown
        '
        Me.bsRefBorderLineLowerLimitUpDown.BackColor = System.Drawing.Color.White
        Me.bsRefBorderLineLowerLimitUpDown.Enabled = False
        Me.bsRefBorderLineLowerLimitUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsRefBorderLineLowerLimitUpDown.Location = New System.Drawing.Point(418, 39)
        Me.bsRefBorderLineLowerLimitUpDown.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.bsRefBorderLineLowerLimitUpDown.Name = "bsRefBorderLineLowerLimitUpDown"
        Me.bsRefBorderLineLowerLimitUpDown.Size = New System.Drawing.Size(70, 21)
        Me.bsRefBorderLineLowerLimitUpDown.TabIndex = 3
        Me.bsRefBorderLineLowerLimitUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsRefBorderLineUpperLimitUpDown
        '
        Me.bsRefBorderLineUpperLimitUpDown.BackColor = System.Drawing.Color.White
        Me.bsRefBorderLineUpperLimitUpDown.Enabled = False
        Me.bsRefBorderLineUpperLimitUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsRefBorderLineUpperLimitUpDown.Location = New System.Drawing.Point(515, 39)
        Me.bsRefBorderLineUpperLimitUpDown.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.bsRefBorderLineUpperLimitUpDown.Name = "bsRefBorderLineUpperLimitUpDown"
        Me.bsRefBorderLineUpperLimitUpDown.Size = New System.Drawing.Size(70, 21)
        Me.bsRefBorderLineUpperLimitUpDown.TabIndex = 4
        Me.bsRefBorderLineUpperLimitUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsRefNormalLowerLimitUpDown
        '
        Me.bsRefNormalLowerLimitUpDown.BackColor = System.Drawing.Color.White
        Me.bsRefNormalLowerLimitUpDown.Enabled = False
        Me.bsRefNormalLowerLimitUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsRefNormalLowerLimitUpDown.Location = New System.Drawing.Point(106, 39)
        Me.bsRefNormalLowerLimitUpDown.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.bsRefNormalLowerLimitUpDown.Name = "bsRefNormalLowerLimitUpDown"
        Me.bsRefNormalLowerLimitUpDown.Size = New System.Drawing.Size(74, 21)
        Me.bsRefNormalLowerLimitUpDown.TabIndex = 1
        Me.bsRefNormalLowerLimitUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsRefNormalMaxLabel
        '
        Me.bsRefNormalMaxLabel.AutoSize = True
        Me.bsRefNormalMaxLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRefNormalMaxLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRefNormalMaxLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRefNormalMaxLabel.Location = New System.Drawing.Point(199, 22)
        Me.bsRefNormalMaxLabel.Name = "bsRefNormalMaxLabel"
        Me.bsRefNormalMaxLabel.Size = New System.Drawing.Size(62, 13)
        Me.bsRefNormalMaxLabel.TabIndex = 219
        Me.bsRefNormalMaxLabel.Text = "Maximum"
        Me.bsRefNormalMaxLabel.Title = False
        '
        'bsRefNormalMinLabel
        '
        Me.bsRefNormalMinLabel.AutoSize = True
        Me.bsRefNormalMinLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRefNormalMinLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRefNormalMinLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRefNormalMinLabel.Location = New System.Drawing.Point(103, 22)
        Me.bsRefNormalMinLabel.Name = "bsRefNormalMinLabel"
        Me.bsRefNormalMinLabel.Size = New System.Drawing.Size(58, 13)
        Me.bsRefNormalMinLabel.TabIndex = 217
        Me.bsRefNormalMinLabel.Text = "Minimum"
        Me.bsRefNormalMinLabel.Title = False
        '
        'bsBorderLineUnitLabel
        '
        Me.bsBorderLineUnitLabel.AutoSize = True
        Me.bsBorderLineUnitLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsBorderLineUnitLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsBorderLineUnitLabel.ForeColor = System.Drawing.Color.Black
        Me.bsBorderLineUnitLabel.Location = New System.Drawing.Point(586, 41)
        Me.bsBorderLineUnitLabel.Name = "bsBorderLineUnitLabel"
        Me.bsBorderLineUnitLabel.Size = New System.Drawing.Size(51, 13)
        Me.bsBorderLineUnitLabel.TabIndex = 215
        Me.bsBorderLineUnitLabel.Text = "mg/24h"
        Me.bsBorderLineUnitLabel.Title = False
        '
        'bsBorderLineLabel
        '
        Me.bsBorderLineLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsBorderLineLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsBorderLineLabel.ForeColor = System.Drawing.Color.Black
        Me.bsBorderLineLabel.Location = New System.Drawing.Point(336, 41)
        Me.bsBorderLineLabel.Name = "bsBorderLineLabel"
        Me.bsBorderLineLabel.Size = New System.Drawing.Size(81, 13)
        Me.bsBorderLineLabel.TabIndex = 214
        Me.bsBorderLineLabel.Text = "Panic:"
        Me.bsBorderLineLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.bsBorderLineLabel.Title = False
        '
        'bsGenericNormalUnitLabel
        '
        Me.bsGenericNormalUnitLabel.AutoSize = True
        Me.bsGenericNormalUnitLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsGenericNormalUnitLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsGenericNormalUnitLabel.ForeColor = System.Drawing.Color.Black
        Me.bsGenericNormalUnitLabel.Location = New System.Drawing.Point(273, 41)
        Me.bsGenericNormalUnitLabel.Name = "bsGenericNormalUnitLabel"
        Me.bsGenericNormalUnitLabel.Size = New System.Drawing.Size(51, 13)
        Me.bsGenericNormalUnitLabel.TabIndex = 213
        Me.bsGenericNormalUnitLabel.Text = "mg/24h"
        Me.bsGenericNormalUnitLabel.Title = False
        '
        'bsRefGenericNormalLabel
        '
        Me.bsRefGenericNormalLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRefGenericNormalLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRefGenericNormalLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRefGenericNormalLabel.Location = New System.Drawing.Point(6, 41)
        Me.bsRefGenericNormalLabel.Name = "bsRefGenericNormalLabel"
        Me.bsRefGenericNormalLabel.Size = New System.Drawing.Size(99, 13)
        Me.bsRefGenericNormalLabel.TabIndex = 212
        Me.bsRefGenericNormalLabel.Text = "Normality:"
        Me.bsRefGenericNormalLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.bsRefGenericNormalLabel.Title = False
        '
        'bsControlErrorProvider
        '
        Me.bsControlErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsControlErrorProvider.ContainerControl = Me
        '
        'BSReferenceRanges
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.Gainsboro
        Me.Controls.Add(Me.BsPanel)
        Me.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ForeColor = System.Drawing.Color.Black
        Me.Name = "BSReferenceRanges"
        Me.Size = New System.Drawing.Size(648, 228)
        Me.BsPanel.ResumeLayout(False)
        Me.BsPanel.PerformLayout()
        CType(Me.bsRefDetailDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsRefNormalUpperLimitUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsRefBorderLineLowerLimitUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsRefBorderLineUpperLimitUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsRefNormalLowerLimitUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsControlErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsControlErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsControlToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents BsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsRefBorderMaxLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRefBorderMinLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDetailedCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsGenericCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsDetailedSubTypesComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsRefDetailDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsRefDetailDeleteButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsRefNormalUpperLimitUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsRefBorderLineLowerLimitUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsRefBorderLineUpperLimitUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsRefNormalLowerLimitUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsRefNormalMaxLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRefNormalMinLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsBorderLineUnitLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsBorderLineLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsGenericNormalUnitLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRefGenericNormalLabel As Biosystems.Ax00.Controls.UserControls.BSLabel

End Class
