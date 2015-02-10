<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiWSImportLIMSErrors
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiWSImportLIMSErrors))
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsLIMSErrorsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsImportDateTimeTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsLIMSErrorsDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsImportDTLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLIMSErrorsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLIMSErrorsGroupBox.SuspendLayout()
        CType(Me.bsLIMSErrorsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsExitButton
        '
        Me.bsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.ForeColor = System.Drawing.Color.Black
        Me.bsExitButton.Location = New System.Drawing.Point(778, 450)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 55
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsLIMSErrorsGroupBox
        '
        Me.bsLIMSErrorsGroupBox.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsLIMSErrorsGroupBox.Controls.Add(Me.bsImportDateTimeTextBox)
        Me.bsLIMSErrorsGroupBox.Controls.Add(Me.bsLIMSErrorsDataGridView)
        Me.bsLIMSErrorsGroupBox.Controls.Add(Me.bsImportDTLabel)
        Me.bsLIMSErrorsGroupBox.Controls.Add(Me.bsLIMSErrorsLabel)
        Me.bsLIMSErrorsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLIMSErrorsGroupBox.Location = New System.Drawing.Point(8, 9)
        Me.bsLIMSErrorsGroupBox.Name = "bsLIMSErrorsGroupBox"
        Me.bsLIMSErrorsGroupBox.Size = New System.Drawing.Size(800, 437)
        Me.bsLIMSErrorsGroupBox.TabIndex = 54
        Me.bsLIMSErrorsGroupBox.TabStop = False
        '
        'bsImportDateTimeTextBox
        '
        Me.bsImportDateTimeTextBox.BackColor = System.Drawing.Color.White
        Me.bsImportDateTimeTextBox.DecimalsValues = False
        Me.bsImportDateTimeTextBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsImportDateTimeTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsImportDateTimeTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.bsImportDateTimeTextBox.IsNumeric = False
        Me.bsImportDateTimeTextBox.Location = New System.Drawing.Point(640, 40)
        Me.bsImportDateTimeTextBox.Mandatory = False
        Me.bsImportDateTimeTextBox.MaxLength = 30
        Me.bsImportDateTimeTextBox.Name = "bsImportDateTimeTextBox"
        Me.bsImportDateTimeTextBox.ReadOnly = True
        Me.bsImportDateTimeTextBox.Size = New System.Drawing.Size(150, 21)
        Me.bsImportDateTimeTextBox.TabIndex = 162
        Me.bsImportDateTimeTextBox.TabStop = False
        Me.bsImportDateTimeTextBox.Text = "14/09/2010 22:45"
        Me.bsImportDateTimeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.bsImportDateTimeTextBox.WordWrap = False
        '
        'bsLIMSErrorsDataGridView
        '
        Me.bsLIMSErrorsDataGridView.AllowUserToAddRows = False
        Me.bsLIMSErrorsDataGridView.AllowUserToDeleteRows = False
        Me.bsLIMSErrorsDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsLIMSErrorsDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsLIMSErrorsDataGridView.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsLIMSErrorsDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsLIMSErrorsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsLIMSErrorsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsLIMSErrorsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsLIMSErrorsDataGridView.ColumnHeadersHeight = 20
        Me.bsLIMSErrorsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsLIMSErrorsDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsLIMSErrorsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsLIMSErrorsDataGridView.EnterToTab = False
        Me.bsLIMSErrorsDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsLIMSErrorsDataGridView.Location = New System.Drawing.Point(10, 66)
        Me.bsLIMSErrorsDataGridView.MultiSelect = False
        Me.bsLIMSErrorsDataGridView.Name = "bsLIMSErrorsDataGridView"
        Me.bsLIMSErrorsDataGridView.ReadOnly = True
        Me.bsLIMSErrorsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsLIMSErrorsDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsLIMSErrorsDataGridView.RowHeadersVisible = False
        Me.bsLIMSErrorsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsLIMSErrorsDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsLIMSErrorsDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.bsLIMSErrorsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsLIMSErrorsDataGridView.Size = New System.Drawing.Size(780, 356)
        Me.bsLIMSErrorsDataGridView.TabIndex = 161
        Me.bsLIMSErrorsDataGridView.TabToEnter = False
        '
        'bsImportDTLabel
        '
        Me.bsImportDTLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsImportDTLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsImportDTLabel.ForeColor = System.Drawing.Color.Black
        Me.bsImportDTLabel.Location = New System.Drawing.Point(480, 42)
        Me.bsImportDTLabel.Name = "bsImportDTLabel"
        Me.bsImportDTLabel.Size = New System.Drawing.Size(163, 13)
        Me.bsImportDTLabel.TabIndex = 48
        Me.bsImportDTLabel.Text = "Import datetime:"
        Me.bsImportDTLabel.Title = False
        '
        'bsLIMSErrorsLabel
        '
        Me.bsLIMSErrorsLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsLIMSErrorsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsLIMSErrorsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsLIMSErrorsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLIMSErrorsLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsLIMSErrorsLabel.Name = "bsLIMSErrorsLabel"
        Me.bsLIMSErrorsLabel.Size = New System.Drawing.Size(780, 20)
        Me.bsLIMSErrorsLabel.TabIndex = 46
        Me.bsLIMSErrorsLabel.Text = "LIMS Import Errors"
        Me.bsLIMSErrorsLabel.Title = True
        '
        'IWSImportLIMSErrors
        '
        Me.AllowDrop = False
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(819, 490)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsLIMSErrorsGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiWSImportLIMSErrors"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        Me.bsLIMSErrorsGroupBox.ResumeLayout(False)
        Me.bsLIMSErrorsGroupBox.PerformLayout()
        CType(Me.bsLIMSErrorsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLIMSErrorsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsImportDateTimeTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsLIMSErrorsDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsImportDTLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLIMSErrorsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel

End Class
