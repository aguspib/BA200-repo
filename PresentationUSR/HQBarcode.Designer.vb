<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HQBarcode
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HQBarcode))
        Me.bsOrderDetailsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSamplesDetailsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSearchTestsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsStatCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsSampleTypeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BarcodeTypesGB = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.EditButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsHQSellectAllCheckBx = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsLIMSImportButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsIncompleteSamplesDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsSamplesListLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.bsOrderDetailsGroupBox.SuspendLayout()
        Me.BarcodeTypesGB.SuspendLayout()
        CType(Me.bsIncompleteSamplesDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsOrderDetailsGroupBox
        '
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsSaveButton)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsSamplesDetailsLabel)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsCancelButton)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsSearchTestsButton)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsSampleTypeLabel)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsStatCheckbox)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsSampleTypeComboBox)
        Me.bsOrderDetailsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsOrderDetailsGroupBox.Location = New System.Drawing.Point(10, 505)
        Me.bsOrderDetailsGroupBox.Name = "bsOrderDetailsGroupBox"
        Me.bsOrderDetailsGroupBox.Size = New System.Drawing.Size(772, 102)
        Me.bsOrderDetailsGroupBox.TabIndex = 14
        Me.bsOrderDetailsGroupBox.TabStop = False
        '
        'bsSaveButton
        '
        Me.bsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSaveButton.Location = New System.Drawing.Point(692, 44)
        Me.bsSaveButton.Name = "bsSaveButton"
        Me.bsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveButton.TabIndex = 5
        Me.bsSaveButton.UseVisualStyleBackColor = True
        '
        'bsSamplesDetailsLabel
        '
        Me.bsSamplesDetailsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSamplesDetailsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsSamplesDetailsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesDetailsLabel.Location = New System.Drawing.Point(10, 13)
        Me.bsSamplesDetailsLabel.Name = "bsSamplesDetailsLabel"
        Me.bsSamplesDetailsLabel.Size = New System.Drawing.Size(752, 19)
        Me.bsSamplesDetailsLabel.TabIndex = 0
        Me.bsSamplesDetailsLabel.Text = "*Enter Samples Details"
        Me.bsSamplesDetailsLabel.Title = True
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Location = New System.Drawing.Point(730, 44)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 6
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsSearchTestsButton
        '
        Me.bsSearchTestsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSearchTestsButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSearchTestsButton.ForeColor = System.Drawing.Color.SteelBlue
        Me.bsSearchTestsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsSearchTestsButton.Location = New System.Drawing.Point(364, 55)
        Me.bsSearchTestsButton.Name = "bsSearchTestsButton"
        Me.bsSearchTestsButton.Size = New System.Drawing.Size(85, 21)
        Me.bsSearchTestsButton.TabIndex = 3
        Me.bsSearchTestsButton.Text = "*Tests"
        Me.bsSearchTestsButton.UseMnemonic = False
        Me.bsSearchTestsButton.UseVisualStyleBackColor = True
        '
        'bsSampleTypeLabel
        '
        Me.bsSampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeLabel.Location = New System.Drawing.Point(10, 38)
        Me.bsSampleTypeLabel.Name = "bsSampleTypeLabel"
        Me.bsSampleTypeLabel.Size = New System.Drawing.Size(210, 13)
        Me.bsSampleTypeLabel.TabIndex = 0
        Me.bsSampleTypeLabel.Text = "*Sample Type:"
        Me.bsSampleTypeLabel.Title = False
        '
        'bsStatCheckbox
        '
        Me.bsStatCheckbox.AutoSize = True
        Me.bsStatCheckbox.BackColor = System.Drawing.Color.Transparent
        Me.bsStatCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsStatCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsStatCheckbox.Location = New System.Drawing.Point(287, 59)
        Me.bsStatCheckbox.Name = "bsStatCheckbox"
        Me.bsStatCheckbox.Size = New System.Drawing.Size(56, 17)
        Me.bsStatCheckbox.TabIndex = 4
        Me.bsStatCheckbox.Text = "*Stat"
        Me.bsStatCheckbox.UseVisualStyleBackColor = False
        '
        'bsSampleTypeComboBox
        '
        Me.bsSampleTypeComboBox.BackColor = System.Drawing.Color.White
        Me.bsSampleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsSampleTypeComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSampleTypeComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeComboBox.FormattingEnabled = True
        Me.bsSampleTypeComboBox.Location = New System.Drawing.Point(10, 55)
        Me.bsSampleTypeComboBox.Name = "bsSampleTypeComboBox"
        Me.bsSampleTypeComboBox.Size = New System.Drawing.Size(254, 21)
        Me.bsSampleTypeComboBox.TabIndex = 2
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.bsExitButton.Location = New System.Drawing.Point(750, 614)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 7
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'BarcodeTypesGB
        '
        Me.BarcodeTypesGB.Controls.Add(Me.EditButton)
        Me.BarcodeTypesGB.Controls.Add(Me.bsHQSellectAllCheckBx)
        Me.BarcodeTypesGB.Controls.Add(Me.bsLIMSImportButton)
        Me.BarcodeTypesGB.Controls.Add(Me.bsIncompleteSamplesDataGridView)
        Me.BarcodeTypesGB.Controls.Add(Me.bsSamplesListLabel)
        Me.BarcodeTypesGB.ForeColor = System.Drawing.Color.Black
        Me.BarcodeTypesGB.Location = New System.Drawing.Point(10, -2)
        Me.BarcodeTypesGB.Name = "BarcodeTypesGB"
        Me.BarcodeTypesGB.Size = New System.Drawing.Size(772, 507)
        Me.BarcodeTypesGB.TabIndex = 176
        Me.BarcodeTypesGB.TabStop = False
        '
        'EditButton
        '
        Me.EditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.EditButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.EditButton.Location = New System.Drawing.Point(730, 112)
        Me.EditButton.Name = "EditButton"
        Me.EditButton.Size = New System.Drawing.Size(32, 32)
        Me.EditButton.TabIndex = 176
        Me.EditButton.UseVisualStyleBackColor = True
        '
        'bsHQSellectAllCheckBx
        '
        Me.bsHQSellectAllCheckBx.AutoSize = True
        Me.bsHQSellectAllCheckBx.BackColor = System.Drawing.Color.Transparent
        Me.bsHQSellectAllCheckBx.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsHQSellectAllCheckBx.ForeColor = System.Drawing.Color.Black
        Me.bsHQSellectAllCheckBx.Location = New System.Drawing.Point(26, 46)
        Me.bsHQSellectAllCheckBx.Name = "bsHQSellectAllCheckBx"
        Me.bsHQSellectAllCheckBx.Size = New System.Drawing.Size(89, 17)
        Me.bsHQSellectAllCheckBx.TabIndex = 175
        Me.bsHQSellectAllCheckBx.Text = "*Sellect All"
        Me.bsHQSellectAllCheckBx.UseVisualStyleBackColor = False
        '
        'bsLIMSImportButton
        '
        Me.bsLIMSImportButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsLIMSImportButton.Location = New System.Drawing.Point(730, 74)
        Me.bsLIMSImportButton.Name = "bsLIMSImportButton"
        Me.bsLIMSImportButton.Size = New System.Drawing.Size(32, 32)
        Me.bsLIMSImportButton.TabIndex = 1
        Me.bsLIMSImportButton.UseVisualStyleBackColor = True
        '
        'bsIncompleteSamplesDataGridView
        '
        Me.bsIncompleteSamplesDataGridView.AllowUserToAddRows = False
        Me.bsIncompleteSamplesDataGridView.AllowUserToDeleteRows = False
        Me.bsIncompleteSamplesDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White
        Me.bsIncompleteSamplesDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsIncompleteSamplesDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsIncompleteSamplesDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsIncompleteSamplesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsIncompleteSamplesDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsIncompleteSamplesDataGridView.ColumnHeadersHeight = 20
        Me.bsIncompleteSamplesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsIncompleteSamplesDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsIncompleteSamplesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsIncompleteSamplesDataGridView.EnterToTab = False
        Me.bsIncompleteSamplesDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsIncompleteSamplesDataGridView.Location = New System.Drawing.Point(13, 74)
        Me.bsIncompleteSamplesDataGridView.Name = "bsIncompleteSamplesDataGridView"
        Me.bsIncompleteSamplesDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsIncompleteSamplesDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsIncompleteSamplesDataGridView.RowHeadersVisible = False
        Me.bsIncompleteSamplesDataGridView.RowHeadersWidth = 20
        Me.bsIncompleteSamplesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        Me.bsIncompleteSamplesDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsIncompleteSamplesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsIncompleteSamplesDataGridView.Size = New System.Drawing.Size(710, 420)
        Me.bsIncompleteSamplesDataGridView.TabIndex = 0
        Me.bsIncompleteSamplesDataGridView.TabToEnter = False
        '
        'bsSamplesListLabel
        '
        Me.bsSamplesListLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSamplesListLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsSamplesListLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesListLabel.Location = New System.Drawing.Point(10, 13)
        Me.bsSamplesListLabel.Name = "bsSamplesListLabel"
        Me.bsSamplesListLabel.Size = New System.Drawing.Size(752, 19)
        Me.bsSamplesListLabel.TabIndex = 174
        Me.bsSamplesListLabel.Text = "*Incomplete Samples List"
        Me.bsSamplesListLabel.Title = True
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'HQBarcode
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(789, 652)
        Me.ControlBox = False
        Me.Controls.Add(Me.BarcodeTypesGB)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsOrderDetailsGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "HQBarcode"
        Me.ShowInTaskbar = False
        Me.Text = "  "
        Me.bsOrderDetailsGroupBox.ResumeLayout(False)
        Me.bsOrderDetailsGroupBox.PerformLayout()
        Me.BarcodeTypesGB.ResumeLayout(False)
        Me.BarcodeTypesGB.PerformLayout()
        CType(Me.bsIncompleteSamplesDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsOrderDetailsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsSamplesDetailsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSearchTestsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsStatCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsSampleTypeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BarcodeTypesGB As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsSamplesListLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLIMSImportButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsIncompleteSamplesDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsHQSellectAllCheckBx As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents EditButton As Biosystems.Ax00.Controls.UserControls.BSButton
    'Friend WithEvents BsPictureBox1 As BSPictureBox
End Class
