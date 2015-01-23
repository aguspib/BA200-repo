<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiQCAddManualResultsAux
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                ReleaseElements()
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiQCAddManualResultsAux))
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsAddManualResultsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsNumSerieNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.bsNumSerieLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsAddManualResultLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsNewResultGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.myToolTipsControl = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsAddManualResultsGroupBox.SuspendLayout()
        CType(Me.bsNumSerieNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsNewResultGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.Location = New System.Drawing.Point(489, 248)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 3
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Location = New System.Drawing.Point(526, 248)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 4
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsAddManualResultsGroupBox
        '
        Me.bsAddManualResultsGroupBox.Controls.Add(Me.bsNumSerieNumericUpDown)
        Me.bsAddManualResultsGroupBox.Controls.Add(Me.bsNumSerieLabel)
        Me.bsAddManualResultsGroupBox.Controls.Add(Me.bsAddManualResultLabel)
        Me.bsAddManualResultsGroupBox.Controls.Add(Me.bsNewResultGridView)
        Me.bsAddManualResultsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsAddManualResultsGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsAddManualResultsGroupBox.Name = "bsAddManualResultsGroupBox"
        Me.bsAddManualResultsGroupBox.Size = New System.Drawing.Size(560, 232)
        Me.bsAddManualResultsGroupBox.TabIndex = 0
        Me.bsAddManualResultsGroupBox.TabStop = False
        '
        'bsNumSerieNumericUpDown
        '
        Me.bsNumSerieNumericUpDown.BackColor = System.Drawing.Color.White
        Me.bsNumSerieNumericUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsNumSerieNumericUpDown.Location = New System.Drawing.Point(469, 66)
        Me.bsNumSerieNumericUpDown.Name = "bsNumSerieNumericUpDown"
        Me.bsNumSerieNumericUpDown.Size = New System.Drawing.Size(79, 21)
        Me.bsNumSerieNumericUpDown.TabIndex = 0
        Me.bsNumSerieNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsNumSerieLabel
        '
        Me.bsNumSerieLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNumSerieLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNumSerieLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNumSerieLabel.Location = New System.Drawing.Point(469, 48)
        Me.bsNumSerieLabel.Name = "bsNumSerieLabel"
        Me.bsNumSerieLabel.Size = New System.Drawing.Size(91, 13)
        Me.bsNumSerieLabel.TabIndex = 12
        Me.bsNumSerieLabel.Text = "*Num Serie:"
        Me.bsNumSerieLabel.Title = False
        '
        'bsAddManualResultLabel
        '
        Me.bsAddManualResultLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsAddManualResultLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsAddManualResultLabel.ForeColor = System.Drawing.Color.Black
        Me.bsAddManualResultLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsAddManualResultLabel.Name = "bsAddManualResultLabel"
        Me.bsAddManualResultLabel.Size = New System.Drawing.Size(540, 20)
        Me.bsAddManualResultLabel.TabIndex = 1
        Me.bsAddManualResultLabel.Text = "*Add Manual Results"
        Me.bsAddManualResultLabel.Title = True
        '
        'bsNewResultGridView
        '
        Me.bsNewResultGridView.AllowUserToDeleteRows = False
        Me.bsNewResultGridView.AllowUserToResizeColumns = False
        Me.bsNewResultGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.bsNewResultGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsNewResultGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsNewResultGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsNewResultGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsNewResultGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsNewResultGridView.ColumnHeadersHeight = 20
        Me.bsNewResultGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsNewResultGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsNewResultGridView.EnterToTab = False
        Me.bsNewResultGridView.GridColor = System.Drawing.Color.Silver
        Me.bsNewResultGridView.Location = New System.Drawing.Point(10, 122)
        Me.bsNewResultGridView.MultiSelect = False
        Me.bsNewResultGridView.Name = "bsNewResultGridView"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsNewResultGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsNewResultGridView.RowHeadersVisible = False
        Me.bsNewResultGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        Me.bsNewResultGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsNewResultGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsNewResultGridView.ShowRowErrors = False
        Me.bsNewResultGridView.Size = New System.Drawing.Size(540, 100)
        Me.bsNewResultGridView.TabIndex = 2
        Me.bsNewResultGridView.TabToEnter = True
        '
        'IQCAddManualResultsAux
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.ClientSize = New System.Drawing.Size(584, 286)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsAcceptButton)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsAddManualResultsGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IQCAddManualResultsAux"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        Me.bsAddManualResultsGroupBox.ResumeLayout(False)
        CType(Me.bsNumSerieNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsNewResultGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsAddManualResultsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsAddManualResultLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsNewResultGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents myToolTipsControl As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsNumSerieNumericUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsNumSerieLabel As Biosystems.Ax00.Controls.UserControls.BSLabel

End Class
