<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiConfigLISMapping
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiConfigLISMapping))
        Me.CloseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsLISMappingGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.LISMappingDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsLISMappingLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.SaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.ButtonCancel = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.EditButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.FilterComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.BsLISMappingTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.bsLISMappingGroupBox.SuspendLayout()
        CType(Me.LISMappingDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CloseButton
        '
        Me.CloseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CloseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.CloseButton.Location = New System.Drawing.Point(923, 614)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(32, 32)
        Me.CloseButton.TabIndex = 169
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'bsLISMappingGroupBox
        '
        Me.bsLISMappingGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bsLISMappingGroupBox.Controls.Add(Me.LISMappingDataGridView)
        Me.bsLISMappingGroupBox.Controls.Add(Me.bsLISMappingLabel)
        Me.bsLISMappingGroupBox.Controls.Add(Me.SaveButton)
        Me.bsLISMappingGroupBox.Controls.Add(Me.ButtonCancel)
        Me.bsLISMappingGroupBox.Controls.Add(Me.EditButton)
        Me.bsLISMappingGroupBox.Controls.Add(Me.FilterComboBox)
        Me.bsLISMappingGroupBox.Controls.Add(Me.BsLISMappingTypeLabel)
        Me.bsLISMappingGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLISMappingGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsLISMappingGroupBox.Name = "bsLISMappingGroupBox"
        Me.bsLISMappingGroupBox.Size = New System.Drawing.Size(956, 594)
        Me.bsLISMappingGroupBox.TabIndex = 173
        Me.bsLISMappingGroupBox.TabStop = False
        '
        'LISMappingDataGridView
        '
        Me.LISMappingDataGridView.AllowUserToAddRows = False
        Me.LISMappingDataGridView.AllowUserToDeleteRows = False
        Me.LISMappingDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LISMappingDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.LISMappingDataGridView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.LISMappingDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.LISMappingDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LISMappingDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LISMappingDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.LISMappingDataGridView.ColumnHeadersHeight = 20
        Me.LISMappingDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LISMappingDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.LISMappingDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.LISMappingDataGridView.EnterToTab = True
        Me.LISMappingDataGridView.GridColor = System.Drawing.Color.Silver
        Me.LISMappingDataGridView.Location = New System.Drawing.Point(12, 77)
        Me.LISMappingDataGridView.Name = "LISMappingDataGridView"
        Me.LISMappingDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LISMappingDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.LISMappingDataGridView.RowHeadersVisible = False
        Me.LISMappingDataGridView.RowHeadersWidth = 20
        Me.LISMappingDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.LISMappingDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.LISMappingDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.LISMappingDataGridView.Size = New System.Drawing.Size(930, 465)
        Me.LISMappingDataGridView.TabIndex = 178
        Me.LISMappingDataGridView.TabToEnter = False
        '
        'bsLISMappingLabel
        '
        Me.bsLISMappingLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsLISMappingLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsLISMappingLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLISMappingLabel.Location = New System.Drawing.Point(9, 17)
        Me.bsLISMappingLabel.Name = "bsLISMappingLabel"
        Me.bsLISMappingLabel.Size = New System.Drawing.Size(933, 20)
        Me.bsLISMappingLabel.TabIndex = 1
        Me.bsLISMappingLabel.Text = "*LIS Mapping"
        Me.bsLISMappingLabel.Title = True
        '
        'SaveButton
        '
        Me.SaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.SaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.SaveButton.Enabled = False
        Me.SaveButton.Location = New System.Drawing.Point(872, 552)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(32, 32)
        Me.SaveButton.TabIndex = 175
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'CancelButton
        '
        Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ButtonCancel.Enabled = False
        Me.ButtonCancel.Location = New System.Drawing.Point(910, 552)
        Me.ButtonCancel.Name = "CancelButton"
        Me.ButtonCancel.Size = New System.Drawing.Size(32, 32)
        Me.ButtonCancel.TabIndex = 174
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'EditButton
        '
        Me.EditButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.EditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.EditButton.Enabled = False
        Me.EditButton.Location = New System.Drawing.Point(834, 552)
        Me.EditButton.Name = "EditButton"
        Me.EditButton.Size = New System.Drawing.Size(32, 32)
        Me.EditButton.TabIndex = 173
        Me.EditButton.UseVisualStyleBackColor = True
        '
        'FilterComboBox
        '
        Me.FilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.FilterComboBox.ForeColor = System.Drawing.Color.Black
        Me.FilterComboBox.FormattingEnabled = True
        Me.FilterComboBox.Location = New System.Drawing.Point(192, 50)
        Me.FilterComboBox.Name = "FilterComboBox"
        Me.FilterComboBox.Size = New System.Drawing.Size(214, 21)
        Me.FilterComboBox.TabIndex = 77
        '
        'BsLISMappingTypeLabel
        '
        Me.BsLISMappingTypeLabel.AutoSize = True
        Me.BsLISMappingTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsLISMappingTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLISMappingTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.BsLISMappingTypeLabel.Location = New System.Drawing.Point(9, 53)
        Me.BsLISMappingTypeLabel.Name = "BsLISMappingTypeLabel"
        Me.BsLISMappingTypeLabel.Size = New System.Drawing.Size(98, 13)
        Me.BsLISMappingTypeLabel.TabIndex = 75
        Me.BsLISMappingTypeLabel.Text = "*Mapping Type:"
        Me.BsLISMappingTypeLabel.Title = False
        '
        'bsErrorProvider1
        '
        Me.bsErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsErrorProvider1.ContainerControl = Me
        '
        'IConfigLISMapping
        '
        Me.AcceptButton = Me.CloseButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLISMappingGroupBox)
        Me.Controls.Add(Me.CloseButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiConfigLISMapping"
        Me.ShowInTaskbar = False
        Me.Text = ""
        Me.bsLISMappingGroupBox.ResumeLayout(False)
        Me.bsLISMappingGroupBox.PerformLayout()
        CType(Me.LISMappingDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CloseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLISMappingGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents LISMappingDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsLISMappingLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents SaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ButtonCancel As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents EditButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents FilterComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsLISMappingTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
End Class
