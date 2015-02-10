<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiWarningAfectedElements
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiWarningAfectedElements))
        Me.AfectedElementGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.MessageDetailGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsWarningPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.LBL_ElementsDeleteMessage = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.LBL_AfectedElementsWarning = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.AffectedElementsGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.ExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.ButtonCancel = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.AfectedElementGroupBox.SuspendLayout()
        Me.MessageDetailGroupBox.SuspendLayout()
        CType(Me.bsWarningPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AffectedElementsGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'AfectedElementGroupBox
        '
        Me.AfectedElementGroupBox.Controls.Add(Me.MessageDetailGroupBox)
        Me.AfectedElementGroupBox.Controls.Add(Me.LBL_AfectedElementsWarning)
        Me.AfectedElementGroupBox.Controls.Add(Me.AffectedElementsGridView)
        Me.AfectedElementGroupBox.ForeColor = System.Drawing.Color.Black
        Me.AfectedElementGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.AfectedElementGroupBox.Name = "AfectedElementGroupBox"
        Me.AfectedElementGroupBox.Size = New System.Drawing.Size(468, 434)
        Me.AfectedElementGroupBox.TabIndex = 0
        Me.AfectedElementGroupBox.TabStop = False
        '
        'MessageDetailGroupBox
        '
        Me.MessageDetailGroupBox.Controls.Add(Me.bsWarningPictureBox)
        Me.MessageDetailGroupBox.Controls.Add(Me.LBL_ElementsDeleteMessage)
        Me.MessageDetailGroupBox.ForeColor = System.Drawing.Color.Black
        Me.MessageDetailGroupBox.Location = New System.Drawing.Point(10, 45)
        Me.MessageDetailGroupBox.Name = "MessageDetailGroupBox"
        Me.MessageDetailGroupBox.Size = New System.Drawing.Size(448, 65)
        Me.MessageDetailGroupBox.TabIndex = 5
        Me.MessageDetailGroupBox.TabStop = False
        '
        'bsWarningPictureBox
        '
        Me.bsWarningPictureBox.Location = New System.Drawing.Point(10, 23)
        Me.bsWarningPictureBox.Name = "bsWarningPictureBox"
        Me.bsWarningPictureBox.PositionNumber = 0
        Me.bsWarningPictureBox.Size = New System.Drawing.Size(26, 26)
        Me.bsWarningPictureBox.TabIndex = 1
        Me.bsWarningPictureBox.TabStop = False
        '
        'LBL_ElementsDeleteMessage
        '
        Me.LBL_ElementsDeleteMessage.BackColor = System.Drawing.Color.Transparent
        Me.LBL_ElementsDeleteMessage.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.LBL_ElementsDeleteMessage.ForeColor = System.Drawing.Color.Black
        Me.LBL_ElementsDeleteMessage.Location = New System.Drawing.Point(50, 15)
        Me.LBL_ElementsDeleteMessage.Name = "LBL_ElementsDeleteMessage"
        Me.LBL_ElementsDeleteMessage.Size = New System.Drawing.Size(387, 42)
        Me.LBL_ElementsDeleteMessage.TabIndex = 0
        Me.LBL_ElementsDeleteMessage.Text = "* All elements in the list will be also modified or deleted. Please confirm or ca" & _
            "ncel the requested action."
        Me.LBL_ElementsDeleteMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.LBL_ElementsDeleteMessage.Title = False
        '
        'LBL_AfectedElementsWarning
        '
        Me.LBL_AfectedElementsWarning.BackColor = System.Drawing.Color.LightSteelBlue
        Me.LBL_AfectedElementsWarning.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.LBL_AfectedElementsWarning.ForeColor = System.Drawing.Color.Black
        Me.LBL_AfectedElementsWarning.Location = New System.Drawing.Point(10, 15)
        Me.LBL_AfectedElementsWarning.Name = "LBL_AfectedElementsWarning"
        Me.LBL_AfectedElementsWarning.Size = New System.Drawing.Size(448, 20)
        Me.LBL_AfectedElementsWarning.TabIndex = 4
        Me.LBL_AfectedElementsWarning.Text = "Affected Elements Warning"
        Me.LBL_AfectedElementsWarning.Title = True
        '
        'AffectedElementsGridView
        '
        Me.AffectedElementsGridView.AllowUserToAddRows = False
        Me.AffectedElementsGridView.AllowUserToDeleteRows = False
        Me.AffectedElementsGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black
        Me.AffectedElementsGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.AffectedElementsGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.AffectedElementsGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.AffectedElementsGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AffectedElementsGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.AffectedElementsGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.AffectedElementsGridView.ColumnHeadersHeight = 20
        Me.AffectedElementsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.AffectedElementsGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.AffectedElementsGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.AffectedElementsGridView.EnterToTab = False
        Me.AffectedElementsGridView.GridColor = System.Drawing.Color.Silver
        Me.AffectedElementsGridView.Location = New System.Drawing.Point(10, 130)
        Me.AffectedElementsGridView.MultiSelect = False
        Me.AffectedElementsGridView.Name = "AffectedElementsGridView"
        Me.AffectedElementsGridView.ReadOnly = True
        Me.AffectedElementsGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.AffectedElementsGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.AffectedElementsGridView.RowHeadersVisible = False
        Me.AffectedElementsGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.AffectedElementsGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.AffectedElementsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.AffectedElementsGridView.Size = New System.Drawing.Size(448, 290)
        Me.AffectedElementsGridView.TabIndex = 2
        Me.AffectedElementsGridView.TabToEnter = False
        '
        'ExitButton
        '
        Me.ExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExitButton.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.ExitButton.Location = New System.Drawing.Point(408, 456)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(32, 32)
        Me.ExitButton.TabIndex = 1
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'CancelButton
        '
        Me.ButtonCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Location = New System.Drawing.Point(446, 456)
        Me.ButtonCancel.Name = "CancelButton"
        Me.ButtonCancel.Size = New System.Drawing.Size(32, 32)
        Me.ButtonCancel.TabIndex = 2
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'IWarningAfectedElements
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(492, 500)
        Me.ControlBox = False
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.ExitButton)
        Me.Controls.Add(Me.AfectedElementGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiWarningAfectedElements"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        Me.AfectedElementGroupBox.ResumeLayout(False)
        Me.MessageDetailGroupBox.ResumeLayout(False)
        CType(Me.bsWarningPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AffectedElementsGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents AfectedElementGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents AffectedElementsGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents ExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ButtonCancel As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents MessageDetailGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsWarningPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents LBL_ElementsDeleteMessage As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LBL_AfectedElementsWarning As Biosystems.Ax00.Controls.UserControls.BSLabel

End Class
