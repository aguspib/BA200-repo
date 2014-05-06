<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IResultsSummaryTable
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IResultsSummaryTable))
        Me.bsPatientGridGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsPatientsListLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsPatientListDataGridView = New System.Windows.Forms.DataGridView
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsHorizontalRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.bsVerticalRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.bsDetailAreaButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsPatientGridGroupBox.SuspendLayout()
        CType(Me.bsPatientListDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsDetailAreaButtonsPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsPatientGridGroupBox
        '
        Me.bsPatientGridGroupBox.Controls.Add(Me.bsPatientsListLabel)
        Me.bsPatientGridGroupBox.Controls.Add(Me.bsPatientListDataGridView)
        Me.bsPatientGridGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsPatientGridGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsPatientGridGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsPatientGridGroupBox.Margin = New System.Windows.Forms.Padding(4, 3, 3, 3)
        Me.bsPatientGridGroupBox.Name = "bsPatientGridGroupBox"
        Me.bsPatientGridGroupBox.Size = New System.Drawing.Size(950, 589)
        Me.bsPatientGridGroupBox.TabIndex = 45
        Me.bsPatientGridGroupBox.TabStop = False
        '
        'bsPatientsListLabel
        '
        Me.bsPatientsListLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsPatientsListLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsPatientsListLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPatientsListLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsPatientsListLabel.Name = "bsPatientsListLabel"
        Me.bsPatientsListLabel.Size = New System.Drawing.Size(930, 20)
        Me.bsPatientsListLabel.TabIndex = 13
        Me.bsPatientsListLabel.Text = "Patients List"
        Me.bsPatientsListLabel.Title = True
        '
        'bsPatientListDataGridView
        '
        Me.bsPatientListDataGridView.AllowUserToAddRows = False
        Me.bsPatientListDataGridView.AllowUserToDeleteRows = False
        Me.bsPatientListDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientListDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsPatientListDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.bsPatientListDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsPatientListDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsPatientListDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientListDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsPatientListDataGridView.ColumnHeadersHeight = 20
        Me.bsPatientListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.bsPatientListDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsPatientListDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsPatientListDataGridView.Location = New System.Drawing.Point(10, 40)
        Me.bsPatientListDataGridView.Name = "bsPatientListDataGridView"
        Me.bsPatientListDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientListDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.bsPatientListDataGridView.RowHeadersVisible = False
        Me.bsPatientListDataGridView.RowHeadersWidth = 20
        Me.bsPatientListDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientListDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle4
        Me.bsPatientListDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsPatientListDataGridView.Size = New System.Drawing.Size(930, 539)
        Me.bsPatientListDataGridView.TabIndex = 0
        '
        'bsHorizontalRadioButton
        '
        Me.bsHorizontalRadioButton.AutoSize = True
        Me.bsHorizontalRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsHorizontalRadioButton.Location = New System.Drawing.Point(188, 613)
        Me.bsHorizontalRadioButton.Name = "bsHorizontalRadioButton"
        Me.bsHorizontalRadioButton.Size = New System.Drawing.Size(124, 17)
        Me.bsHorizontalRadioButton.TabIndex = 2
        Me.bsHorizontalRadioButton.Text = "Horizontal Report"
        Me.bsHorizontalRadioButton.UseVisualStyleBackColor = True
        Me.bsHorizontalRadioButton.Visible = False
        '
        'bsVerticalRadioButton
        '
        Me.bsVerticalRadioButton.AutoSize = True
        Me.bsVerticalRadioButton.Checked = True
        Me.bsVerticalRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsVerticalRadioButton.ForeColor = System.Drawing.Color.Black
        Me.bsVerticalRadioButton.Location = New System.Drawing.Point(10, 613)
        Me.bsVerticalRadioButton.Name = "bsVerticalRadioButton"
        Me.bsVerticalRadioButton.Size = New System.Drawing.Size(110, 17)
        Me.bsVerticalRadioButton.TabIndex = 1
        Me.bsVerticalRadioButton.TabStop = True
        Me.bsVerticalRadioButton.Text = "Vertical Report"
        Me.bsVerticalRadioButton.UseVisualStyleBackColor = True
        Me.bsVerticalRadioButton.Visible = False
        '
        'bsDetailAreaButtonsPanel
        '
        Me.bsDetailAreaButtonsPanel.Controls.Add(Me.bsPrintButton)
        Me.bsDetailAreaButtonsPanel.Controls.Add(Me.bsExitButton)
        Me.bsDetailAreaButtonsPanel.Location = New System.Drawing.Point(891, 605)
        Me.bsDetailAreaButtonsPanel.Name = "bsDetailAreaButtonsPanel"
        Me.bsDetailAreaButtonsPanel.Size = New System.Drawing.Size(69, 32)
        Me.bsDetailAreaButtonsPanel.TabIndex = 53
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsPrintButton.ForeColor = System.Drawing.Color.Black
        Me.bsPrintButton.Location = New System.Drawing.Point(0, 0)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 3
        Me.bsPrintButton.UseVisualStyleBackColor = True
        Me.bsPrintButton.Visible = False
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.ForeColor = System.Drawing.Color.Black
        Me.bsExitButton.Location = New System.Drawing.Point(37, 0)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 4
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'IResultsSummaryTable
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(972, 644)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsHorizontalRadioButton)
        Me.Controls.Add(Me.bsVerticalRadioButton)
        Me.Controls.Add(Me.bsDetailAreaButtonsPanel)
        Me.Controls.Add(Me.bsPatientGridGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IResultsSummaryTable"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsPatientGridGroupBox.ResumeLayout(False)
        CType(Me.bsPatientListDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsDetailAreaButtonsPanel.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents bsPatientGridGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsPatientsListLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsPatientListDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsHorizontalRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsVerticalRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsDetailAreaButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
End Class
