<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BSCalibrationValues
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
        Me.LBL_Lot = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.LBL_CalibratorName = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.CalibCurveInfoGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.YAxisCombo = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.YaxisLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.XAxisCombo = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.XaxisLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.CurveTypeCombo = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.DecreasingRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.IncreasingRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.CalibrationValuesCurveLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.ConcentrationGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.CalibNumber = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Concentration = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Factor = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.BsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.CalibCurveInfoGroupBox.SuspendLayout()
        CType(Me.ConcentrationGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LBL_Lot
        '
        Me.LBL_Lot.AutoSize = True
        Me.LBL_Lot.BackColor = System.Drawing.Color.Transparent
        Me.LBL_Lot.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LBL_Lot.ForeColor = System.Drawing.Color.Black
        Me.LBL_Lot.Location = New System.Drawing.Point(3, 28)
        Me.LBL_Lot.Name = "LBL_Lot"
        Me.LBL_Lot.Size = New System.Drawing.Size(82, 13)
        Me.LBL_Lot.TabIndex = 53
        Me.LBL_Lot.Text = "Lot Number :"
        Me.LBL_Lot.Title = False
        '
        'LBL_CalibratorName
        '
        Me.LBL_CalibratorName.AutoSize = True
        Me.LBL_CalibratorName.BackColor = System.Drawing.Color.Transparent
        Me.LBL_CalibratorName.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LBL_CalibratorName.ForeColor = System.Drawing.Color.Black
        Me.LBL_CalibratorName.Location = New System.Drawing.Point(2, 4)
        Me.LBL_CalibratorName.Name = "LBL_CalibratorName"
        Me.LBL_CalibratorName.Size = New System.Drawing.Size(106, 13)
        Me.LBL_CalibratorName.TabIndex = 52
        Me.LBL_CalibratorName.Text = "Calibrator Name:"
        Me.LBL_CalibratorName.Title = False
        '
        'CalibCurveInfoGroupBox
        '
        Me.CalibCurveInfoGroupBox.Controls.Add(Me.YAxisCombo)
        Me.CalibCurveInfoGroupBox.Controls.Add(Me.YaxisLabel)
        Me.CalibCurveInfoGroupBox.Controls.Add(Me.XAxisCombo)
        Me.CalibCurveInfoGroupBox.Controls.Add(Me.XaxisLabel)
        Me.CalibCurveInfoGroupBox.Controls.Add(Me.CurveTypeCombo)
        Me.CalibCurveInfoGroupBox.Controls.Add(Me.DecreasingRadioButton)
        Me.CalibCurveInfoGroupBox.Controls.Add(Me.IncreasingRadioButton)
        Me.CalibCurveInfoGroupBox.Enabled = False
        Me.CalibCurveInfoGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CalibCurveInfoGroupBox.Location = New System.Drawing.Point(3, 67)
        Me.CalibCurveInfoGroupBox.Name = "CalibCurveInfoGroupBox"
        Me.CalibCurveInfoGroupBox.Size = New System.Drawing.Size(242, 151)
        Me.CalibCurveInfoGroupBox.TabIndex = 49
        Me.CalibCurveInfoGroupBox.TabStop = False
        '
        'YAxisCombo
        '
        Me.YAxisCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.YAxisCombo.FormattingEnabled = True
        Me.YAxisCombo.Location = New System.Drawing.Point(137, 112)
        Me.YAxisCombo.Name = "YAxisCombo"
        Me.YAxisCombo.Size = New System.Drawing.Size(99, 21)
        Me.YAxisCombo.TabIndex = 2
        '
        'YaxisLabel
        '
        Me.YaxisLabel.AutoSize = True
        Me.YaxisLabel.BackColor = System.Drawing.Color.Transparent
        Me.YaxisLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.YaxisLabel.ForeColor = System.Drawing.Color.Black
        Me.YaxisLabel.Location = New System.Drawing.Point(134, 95)
        Me.YaxisLabel.Name = "YaxisLabel"
        Me.YaxisLabel.Size = New System.Drawing.Size(48, 13)
        Me.YaxisLabel.TabIndex = 5
        Me.YaxisLabel.Text = "Y-Axis:"
        Me.YaxisLabel.Title = False
        '
        'XAxisCombo
        '
        Me.XAxisCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.XAxisCombo.FormattingEnabled = True
        Me.XAxisCombo.Location = New System.Drawing.Point(13, 112)
        Me.XAxisCombo.Name = "XAxisCombo"
        Me.XAxisCombo.Size = New System.Drawing.Size(99, 21)
        Me.XAxisCombo.TabIndex = 1
        '
        'XaxisLabel
        '
        Me.XaxisLabel.AutoSize = True
        Me.XaxisLabel.BackColor = System.Drawing.Color.Transparent
        Me.XaxisLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.XaxisLabel.ForeColor = System.Drawing.Color.Black
        Me.XaxisLabel.Location = New System.Drawing.Point(13, 95)
        Me.XaxisLabel.Name = "XaxisLabel"
        Me.XaxisLabel.Size = New System.Drawing.Size(49, 13)
        Me.XaxisLabel.TabIndex = 3
        Me.XaxisLabel.Text = "X-Axis:"
        Me.XaxisLabel.Title = False
        '
        'CurveTypeCombo
        '
        Me.CurveTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CurveTypeCombo.FormattingEnabled = True
        Me.CurveTypeCombo.Location = New System.Drawing.Point(13, 52)
        Me.CurveTypeCombo.Name = "CurveTypeCombo"
        Me.CurveTypeCombo.Size = New System.Drawing.Size(223, 21)
        Me.CurveTypeCombo.TabIndex = 0
        '
        'DecreasingRadioButton
        '
        Me.DecreasingRadioButton.AutoSize = True
        Me.DecreasingRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DecreasingRadioButton.Location = New System.Drawing.Point(129, 15)
        Me.DecreasingRadioButton.Name = "DecreasingRadioButton"
        Me.DecreasingRadioButton.Size = New System.Drawing.Size(89, 17)
        Me.DecreasingRadioButton.TabIndex = 1
        Me.DecreasingRadioButton.TabStop = True
        Me.DecreasingRadioButton.Text = "Decreasing"
        Me.DecreasingRadioButton.UseVisualStyleBackColor = True
        '
        'IncreasingRadioButton
        '
        Me.IncreasingRadioButton.AutoSize = True
        Me.IncreasingRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IncreasingRadioButton.Location = New System.Drawing.Point(13, 15)
        Me.IncreasingRadioButton.Name = "IncreasingRadioButton"
        Me.IncreasingRadioButton.Size = New System.Drawing.Size(85, 17)
        Me.IncreasingRadioButton.TabIndex = 0
        Me.IncreasingRadioButton.TabStop = True
        Me.IncreasingRadioButton.Text = "Increasing"
        Me.IncreasingRadioButton.UseVisualStyleBackColor = True
        '
        'CalibrationValuesCurveLabel
        '
        Me.CalibrationValuesCurveLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.CalibrationValuesCurveLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.CalibrationValuesCurveLabel.ForeColor = System.Drawing.Color.Black
        Me.CalibrationValuesCurveLabel.Location = New System.Drawing.Point(251, 0)
        Me.CalibrationValuesCurveLabel.Name = "CalibrationValuesCurveLabel"
        Me.CalibrationValuesCurveLabel.Size = New System.Drawing.Size(239, 19)
        Me.CalibrationValuesCurveLabel.TabIndex = 43
        Me.CalibrationValuesCurveLabel.Text = "Concentration Values"
        Me.CalibrationValuesCurveLabel.Title = True
        '
        'ConcentrationGridView
        '
        Me.ConcentrationGridView.AllowUserToAddRows = False
        Me.ConcentrationGridView.AllowUserToDeleteRows = False
        Me.ConcentrationGridView.AllowUserToResizeColumns = False
        Me.ConcentrationGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ConcentrationGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.ConcentrationGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.ConcentrationGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ConcentrationGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ConcentrationGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.ConcentrationGridView.ColumnHeadersHeight = 20
        Me.ConcentrationGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.ConcentrationGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.CalibNumber, Me.Concentration, Me.Factor})
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ConcentrationGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.ConcentrationGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.ConcentrationGridView.EnableHeadersVisualStyles = True
        Me.ConcentrationGridView.EnterToTab = False
        Me.ConcentrationGridView.GridColor = System.Drawing.Color.Silver
        Me.ConcentrationGridView.Location = New System.Drawing.Point(251, 20)
        Me.ConcentrationGridView.MultiSelect = False
        Me.ConcentrationGridView.Name = "ConcentrationGridView"
        Me.ConcentrationGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ConcentrationGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.ConcentrationGridView.RowHeadersVisible = False
        Me.ConcentrationGridView.RowHeadersWidth = 20
        Me.ConcentrationGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ConcentrationGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.ConcentrationGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.ConcentrationGridView.Size = New System.Drawing.Size(239, 200)
        Me.ConcentrationGridView.TabIndex = 42
        Me.ConcentrationGridView.TabToEnter = True
        '
        'CalibNumber
        '
        Me.CalibNumber.HeaderText = "N"
        Me.CalibNumber.Name = "CalibNumber"
        Me.CalibNumber.Width = 65
        '
        'Concentration
        '
        Me.Concentration.HeaderText = "Concentration"
        Me.Concentration.Name = "Concentration"
        '
        'Factor
        '
        Me.Factor.HeaderText = "Factor"
        Me.Factor.Name = "Factor"
        Me.Factor.Width = 70
        '
        'BsLabel1
        '
        Me.BsLabel1.AutoSize = True
        Me.BsLabel1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel1.Location = New System.Drawing.Point(2, 51)
        Me.BsLabel1.Name = "BsLabel1"
        Me.BsLabel1.Size = New System.Drawing.Size(100, 13)
        Me.BsLabel1.TabIndex = 54
        Me.BsLabel1.Text = "Expiration Date:"
        Me.BsLabel1.Title = False
        '
        'BSCalibrationValues
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.LightGray
        Me.Controls.Add(Me.BsLabel1)
        Me.Controls.Add(Me.LBL_Lot)
        Me.Controls.Add(Me.LBL_CalibratorName)
        Me.Controls.Add(Me.CalibCurveInfoGroupBox)
        Me.Controls.Add(Me.CalibrationValuesCurveLabel)
        Me.Controls.Add(Me.ConcentrationGridView)
        Me.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "BSCalibrationValues"
        Me.Size = New System.Drawing.Size(493, 227)
        Me.CalibCurveInfoGroupBox.ResumeLayout(False)
        Me.CalibCurveInfoGroupBox.PerformLayout()
        CType(Me.ConcentrationGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CalibrationValuesCurveLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents ConcentrationGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents CalibNumber As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Concentration As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Factor As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents LBL_Lot As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LBL_CalibratorName As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents CalibCurveInfoGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents YAxisCombo As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents YaxisLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents XAxisCombo As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents XaxisLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents CurveTypeCombo As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents DecreasingRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents IncreasingRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents BsLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel

End Class
