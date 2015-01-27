<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiWSNotPosWarning
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiWSNotPosWarning))
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsAdviceGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsWarningPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsAdviceLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsNotPosWarningGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsNotPositionedDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsNotPosWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsNotPosWarningTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsPositioningButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsAdviceGroupBox.SuspendLayout()
        CType(Me.bsWarningPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsNotPosWarningGroupBox.SuspendLayout()
        CType(Me.bsNotPositionedDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsExitButton.ForeColor = System.Drawing.Color.Black
        Me.bsExitButton.Location = New System.Drawing.Point(378, 454)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 50
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsAdviceGroupBox
        '
        Me.bsAdviceGroupBox.Controls.Add(Me.bsWarningPictureBox)
        Me.bsAdviceGroupBox.Controls.Add(Me.bsAdviceLabel)
        Me.bsAdviceGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsAdviceGroupBox.Location = New System.Drawing.Point(10, 491)
        Me.bsAdviceGroupBox.Name = "bsAdviceGroupBox"
        Me.bsAdviceGroupBox.Size = New System.Drawing.Size(400, 50)
        Me.bsAdviceGroupBox.TabIndex = 52
        Me.bsAdviceGroupBox.TabStop = False
        '
        'bsWarningPictureBox
        '
        Me.bsWarningPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsWarningPictureBox.Location = New System.Drawing.Point(15, 18)
        Me.bsWarningPictureBox.Name = "bsWarningPictureBox"
        Me.bsWarningPictureBox.PositionNumber = 0
        Me.bsWarningPictureBox.Size = New System.Drawing.Size(16, 16)
        Me.bsWarningPictureBox.TabIndex = 50
        Me.bsWarningPictureBox.TabStop = False
        '
        'bsAdviceLabel
        '
        Me.bsAdviceLabel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsAdviceLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsAdviceLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsAdviceLabel.ForeColor = System.Drawing.Color.Black
        Me.bsAdviceLabel.Location = New System.Drawing.Point(42, 19)
        Me.bsAdviceLabel.Name = "bsAdviceLabel"
        Me.bsAdviceLabel.Size = New System.Drawing.Size(347, 27)
        Me.bsAdviceLabel.TabIndex = 49
        Me.bsAdviceLabel.Text = "*Please check Rotors before running Work Session"
        Me.bsAdviceLabel.Title = False
        '
        'bsNotPosWarningGroupBox
        '
        Me.bsNotPosWarningGroupBox.Controls.Add(Me.bsNotPositionedDataGridView)
        Me.bsNotPosWarningGroupBox.Controls.Add(Me.bsNotPosWarningLabel)
        Me.bsNotPosWarningGroupBox.Controls.Add(Me.bsNotPosWarningTitleLabel)
        Me.bsNotPosWarningGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsNotPosWarningGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsNotPosWarningGroupBox.Name = "bsNotPosWarningGroupBox"
        Me.bsNotPosWarningGroupBox.Size = New System.Drawing.Size(400, 439)
        Me.bsNotPosWarningGroupBox.TabIndex = 53
        Me.bsNotPosWarningGroupBox.TabStop = False
        '
        'bsNotPositionedDataGridView
        '
        Me.bsNotPositionedDataGridView.AllowUserToAddRows = False
        Me.bsNotPositionedDataGridView.AllowUserToDeleteRows = False
        Me.bsNotPositionedDataGridView.AllowUserToResizeColumns = False
        Me.bsNotPositionedDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsNotPositionedDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsNotPositionedDataGridView.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsNotPositionedDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsNotPositionedDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsNotPositionedDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsNotPositionedDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsNotPositionedDataGridView.ColumnHeadersHeight = 20
        Me.bsNotPositionedDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsNotPositionedDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsNotPositionedDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsNotPositionedDataGridView.EnterToTab = False
        Me.bsNotPositionedDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsNotPositionedDataGridView.Location = New System.Drawing.Point(10, 78)
        Me.bsNotPositionedDataGridView.MultiSelect = False
        Me.bsNotPositionedDataGridView.Name = "bsNotPositionedDataGridView"
        Me.bsNotPositionedDataGridView.ReadOnly = True
        Me.bsNotPositionedDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!)
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsNotPositionedDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsNotPositionedDataGridView.RowHeadersVisible = False
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsNotPositionedDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsNotPositionedDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsNotPositionedDataGridView.Size = New System.Drawing.Size(380, 346)
        Me.bsNotPositionedDataGridView.TabIndex = 161
        Me.bsNotPositionedDataGridView.TabToEnter = False
        '
        'bsNotPosWarningLabel
        '
        Me.bsNotPosWarningLabel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsNotPosWarningLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNotPosWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNotPosWarningLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNotPosWarningLabel.Location = New System.Drawing.Point(10, 60)
        Me.bsNotPosWarningLabel.Name = "bsNotPosWarningLabel"
        Me.bsNotPosWarningLabel.Size = New System.Drawing.Size(377, 13)
        Me.bsNotPosWarningLabel.TabIndex = 48
        Me.bsNotPosWarningLabel.Text = "*Not positioned Elements:"
        Me.bsNotPosWarningLabel.Title = False
        '
        'bsNotPosWarningTitleLabel
        '
        Me.bsNotPosWarningTitleLabel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsNotPosWarningTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsNotPosWarningTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsNotPosWarningTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNotPosWarningTitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsNotPosWarningTitleLabel.Name = "bsNotPosWarningTitleLabel"
        Me.bsNotPosWarningTitleLabel.Size = New System.Drawing.Size(380, 20)
        Me.bsNotPosWarningTitleLabel.TabIndex = 46
        Me.bsNotPosWarningTitleLabel.Text = "*Positioning Warnings"
        Me.bsNotPosWarningTitleLabel.Title = True
        '
        'bsPositioningButton
        '
        Me.bsPositioningButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPositioningButton.ForeColor = System.Drawing.Color.Black
        Me.bsPositioningButton.Location = New System.Drawing.Point(341, 454)
        Me.bsPositioningButton.Name = "bsPositioningButton"
        Me.bsPositioningButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPositioningButton.TabIndex = 54
        Me.bsPositioningButton.UseVisualStyleBackColor = True
        '
        'IWSNotPosWarning
        '
        Me.AcceptButton = Me.bsPositioningButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.CancelButton = Me.bsExitButton
        Me.ClientSize = New System.Drawing.Size(424, 558)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsPositioningButton)
        Me.Controls.Add(Me.bsNotPosWarningGroupBox)
        Me.Controls.Add(Me.bsAdviceGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IWSNotPosWarning"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsAdviceGroupBox.ResumeLayout(False)
        CType(Me.bsWarningPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsNotPosWarningGroupBox.ResumeLayout(False)
        CType(Me.bsNotPositionedDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAdviceGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsAdviceLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsWarningPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsNotPosWarningGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsNotPositionedDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsNotPosWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsNotPosWarningTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsPositioningButton As Biosystems.Ax00.Controls.UserControls.BSButton
End Class
