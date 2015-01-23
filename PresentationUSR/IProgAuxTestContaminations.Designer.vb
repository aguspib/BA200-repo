<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiProgAuxTestContaminations
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiProgAuxTestContaminations))
        Me.bsTestListGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsContaminationsDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsContaminationsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsContaminatedDetailsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsCuvettesDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsCuvettesLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsTestListGroupBox.SuspendLayout()
        CType(Me.bsContaminationsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsContaminatedDetailsGroupBox.SuspendLayout()
        CType(Me.bsCuvettesDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsTestListGroupBox
        '
        Me.bsTestListGroupBox.Controls.Add(Me.bsContaminationsDataGridView)
        Me.bsTestListGroupBox.Controls.Add(Me.bsContaminationsLabel)
        Me.bsTestListGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestListGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsTestListGroupBox.Name = "bsTestListGroupBox"
        Me.bsTestListGroupBox.Size = New System.Drawing.Size(475, 520)
        Me.bsTestListGroupBox.TabIndex = 42
        Me.bsTestListGroupBox.TabStop = False
        '
        'bsContaminationsDataGridView
        '
        Me.bsContaminationsDataGridView.AllowUserToAddRows = False
        Me.bsContaminationsDataGridView.AllowUserToDeleteRows = False
        Me.bsContaminationsDataGridView.AllowUserToResizeColumns = False
        Me.bsContaminationsDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsContaminationsDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsContaminationsDataGridView.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsContaminationsDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsContaminationsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsContaminationsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsContaminationsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsContaminationsDataGridView.ColumnHeadersHeight = 20
        Me.bsContaminationsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsContaminationsDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsContaminationsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsContaminationsDataGridView.EnterToTab = False
        Me.bsContaminationsDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsContaminationsDataGridView.Location = New System.Drawing.Point(8, 42)
        Me.bsContaminationsDataGridView.MultiSelect = False
        Me.bsContaminationsDataGridView.Name = "bsContaminationsDataGridView"
        Me.bsContaminationsDataGridView.ReadOnly = True
        Me.bsContaminationsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsContaminationsDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsContaminationsDataGridView.RowHeadersVisible = False
        Me.bsContaminationsDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsContaminationsDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsContaminationsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsContaminationsDataGridView.Size = New System.Drawing.Size(457, 467)
        Me.bsContaminationsDataGridView.TabIndex = 164
        Me.bsContaminationsDataGridView.TabToEnter = False
        '
        'bsContaminationsLabel
        '
        Me.bsContaminationsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsContaminationsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsContaminationsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsContaminationsLabel.Location = New System.Drawing.Point(5, 15)
        Me.bsContaminationsLabel.Name = "bsContaminationsLabel"
        Me.bsContaminationsLabel.Size = New System.Drawing.Size(464, 20)
        Me.bsContaminationsLabel.TabIndex = 17
        Me.bsContaminationsLabel.Text = "Contaminations"
        Me.bsContaminationsLabel.Title = True
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(921, 536)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 11
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsContaminatedDetailsGroupBox
        '
        Me.bsContaminatedDetailsGroupBox.Controls.Add(Me.bsCuvettesDataGridView)
        Me.bsContaminatedDetailsGroupBox.Controls.Add(Me.bsCuvettesLabel)
        Me.bsContaminatedDetailsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsContaminatedDetailsGroupBox.Location = New System.Drawing.Point(490, 10)
        Me.bsContaminatedDetailsGroupBox.Name = "bsContaminatedDetailsGroupBox"
        Me.bsContaminatedDetailsGroupBox.Size = New System.Drawing.Size(475, 520)
        Me.bsContaminatedDetailsGroupBox.TabIndex = 43
        Me.bsContaminatedDetailsGroupBox.TabStop = False
        '
        'bsCuvettesDataGridView
        '
        Me.bsCuvettesDataGridView.AllowUserToAddRows = False
        Me.bsCuvettesDataGridView.AllowUserToDeleteRows = False
        Me.bsCuvettesDataGridView.AllowUserToResizeColumns = False
        Me.bsCuvettesDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCuvettesDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle6
        Me.bsCuvettesDataGridView.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsCuvettesDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsCuvettesDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsCuvettesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCuvettesDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.bsCuvettesDataGridView.ColumnHeadersHeight = 20
        Me.bsCuvettesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCuvettesDataGridView.DefaultCellStyle = DataGridViewCellStyle8
        Me.bsCuvettesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsCuvettesDataGridView.EnterToTab = False
        Me.bsCuvettesDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsCuvettesDataGridView.Location = New System.Drawing.Point(9, 42)
        Me.bsCuvettesDataGridView.MultiSelect = False
        Me.bsCuvettesDataGridView.Name = "bsCuvettesDataGridView"
        Me.bsCuvettesDataGridView.ReadOnly = True
        Me.bsCuvettesDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsCuvettesDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.bsCuvettesDataGridView.RowHeadersVisible = False
        Me.bsCuvettesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsCuvettesDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle10
        Me.bsCuvettesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsCuvettesDataGridView.Size = New System.Drawing.Size(457, 467)
        Me.bsCuvettesDataGridView.TabIndex = 163
        Me.bsCuvettesDataGridView.TabToEnter = False
        '
        'bsCuvettesLabel
        '
        Me.bsCuvettesLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsCuvettesLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsCuvettesLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCuvettesLabel.Location = New System.Drawing.Point(6, 15)
        Me.bsCuvettesLabel.Name = "bsCuvettesLabel"
        Me.bsCuvettesLabel.Size = New System.Drawing.Size(463, 20)
        Me.bsCuvettesLabel.TabIndex = 14
        Me.bsCuvettesLabel.Text = "Cuvettes"
        Me.bsCuvettesLabel.Title = True
        '
        'IProgAuxTestContaminations
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
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(975, 573)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsContaminatedDetailsGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsTestListGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IProgAuxTestContaminations"
        Me.Text = " "
        Me.bsTestListGroupBox.ResumeLayout(False)
        CType(Me.bsContaminationsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsContaminatedDetailsGroupBox.ResumeLayout(False)
        CType(Me.bsCuvettesDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsTestListGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsContaminationsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsContaminatedDetailsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsCuvettesLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsCuvettesDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsContaminationsDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView

End Class
