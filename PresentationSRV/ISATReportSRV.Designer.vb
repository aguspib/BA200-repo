<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ISATReportSRV
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
        Me.components = New System.ComponentModel.Container
        Me.BsTitleCreation = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsSATReportTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsSaveSATRepButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsSATDirListBox = New Biosystems.Ax00.Controls.UserControls.BSCheckedListBox
        Me.bsDeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsSelectAllDirCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.resetSaveButtonTimer = New System.Windows.Forms.Timer(Me.components)
        Me.ExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.FolderPathLbl = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.FileNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.FileNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.FolderPathTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.FolderButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.RepSATFolderLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsGroupBox1 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.BsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.BsBorderedPanel1 = New bsBorderedPanel
        Me.BsGroupBox1.SuspendLayout()
        CType(Me.BsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsTitleCreation
        '
        Me.BsTitleCreation.BackColor = System.Drawing.Color.Transparent
        Me.BsTitleCreation.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsTitleCreation.ForeColor = System.Drawing.Color.Black
        Me.BsTitleCreation.Location = New System.Drawing.Point(6, 41)
        Me.BsTitleCreation.Name = "BsTitleCreation"
        Me.BsTitleCreation.Size = New System.Drawing.Size(399, 31)
        Me.BsTitleCreation.TabIndex = 7
        Me.BsTitleCreation.Text = "* You can create files with data for technical service analysis"
        Me.BsTitleCreation.Title = False
        '
        'bsSATReportTitle
        '
        Me.bsSATReportTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSATReportTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsSATReportTitle.ForeColor = System.Drawing.Color.Black
        Me.bsSATReportTitle.Location = New System.Drawing.Point(6, 15)
        Me.bsSATReportTitle.Name = "bsSATReportTitle"
        Me.bsSATReportTitle.Size = New System.Drawing.Size(399, 20)
        Me.bsSATReportTitle.TabIndex = 6
        Me.bsSATReportTitle.Text = "*ReportSAT files creation"
        Me.bsSATReportTitle.Title = True
        '
        'bsSaveSATRepButton
        '
        Me.bsSaveSATRepButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSaveSATRepButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSaveSATRepButton.Location = New System.Drawing.Point(373, 125)
        Me.bsSaveSATRepButton.Name = "bsSaveSATRepButton"
        Me.bsSaveSATRepButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveSATRepButton.TabIndex = 2
        Me.bsSaveSATRepButton.UseVisualStyleBackColor = True
        '
        'bsSATDirListBox
        '
        Me.bsSATDirListBox.CheckOnClick = True
        Me.bsSATDirListBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSATDirListBox.FormattingEnabled = True
        Me.bsSATDirListBox.Location = New System.Drawing.Point(6, 184)
        Me.bsSATDirListBox.Name = "bsSATDirListBox"
        Me.bsSATDirListBox.Size = New System.Drawing.Size(355, 116)
        Me.bsSATDirListBox.TabIndex = 8
        '
        'bsDeleteButton
        '
        Me.bsDeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsDeleteButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDeleteButton.Location = New System.Drawing.Point(329, 304)
        Me.bsDeleteButton.Name = "bsDeleteButton"
        Me.bsDeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.bsDeleteButton.TabIndex = 7
        Me.bsDeleteButton.UseVisualStyleBackColor = True
        '
        'bsSelectAllDirCheckbox
        '
        Me.bsSelectAllDirCheckbox.AutoSize = True
        Me.bsSelectAllDirCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSelectAllDirCheckbox.Location = New System.Drawing.Point(6, 306)
        Me.bsSelectAllDirCheckbox.Name = "bsSelectAllDirCheckbox"
        Me.bsSelectAllDirCheckbox.Size = New System.Drawing.Size(140, 17)
        Me.bsSelectAllDirCheckbox.TabIndex = 6
        Me.bsSelectAllDirCheckbox.Text = "*Select All SAT files"
        Me.bsSelectAllDirCheckbox.UseVisualStyleBackColor = True
        '
        'resetSaveButtonTimer
        '
        '
        'ExitButton
        '
        Me.ExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ExitButton.Location = New System.Drawing.Point(391, 352)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(32, 32)
        Me.ExitButton.TabIndex = 0
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'FolderPathLbl
        '
        Me.FolderPathLbl.AutoSize = True
        Me.FolderPathLbl.BackColor = System.Drawing.Color.Transparent
        Me.FolderPathLbl.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.FolderPathLbl.ForeColor = System.Drawing.Color.Black
        Me.FolderPathLbl.Location = New System.Drawing.Point(6, 72)
        Me.FolderPathLbl.Name = "FolderPathLbl"
        Me.FolderPathLbl.Size = New System.Drawing.Size(49, 13)
        Me.FolderPathLbl.TabIndex = 25
        Me.FolderPathLbl.Text = "*Folder"
        Me.FolderPathLbl.Title = False
        '
        'FileNameLabel
        '
        Me.FileNameLabel.AutoSize = True
        Me.FileNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.FileNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.FileNameLabel.ForeColor = System.Drawing.Color.Black
        Me.FileNameLabel.Location = New System.Drawing.Point(6, 120)
        Me.FileNameLabel.Name = "FileNameLabel"
        Me.FileNameLabel.Size = New System.Drawing.Size(70, 13)
        Me.FileNameLabel.TabIndex = 26
        Me.FileNameLabel.Text = "*File Name"
        Me.FileNameLabel.Title = False
        '
        'FileNameTextBox
        '
        Me.FileNameTextBox.BackColor = System.Drawing.Color.Khaki
        Me.FileNameTextBox.DecimalsValues = False
        Me.FileNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.FileNameTextBox.IsNumeric = False
        Me.FileNameTextBox.Location = New System.Drawing.Point(6, 136)
        Me.FileNameTextBox.Mandatory = True
        Me.FileNameTextBox.MaxLength = 40
        Me.FileNameTextBox.Name = "FileNameTextBox"
        Me.FileNameTextBox.Size = New System.Drawing.Size(355, 21)
        Me.FileNameTextBox.TabIndex = 0
        '
        'FolderPathTextBox
        '
        Me.FolderPathTextBox.BackColor = System.Drawing.Color.White
        Me.FolderPathTextBox.DecimalsValues = False
        Me.FolderPathTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.FolderPathTextBox.ForeColor = System.Drawing.Color.Black
        Me.FolderPathTextBox.IsNumeric = False
        Me.FolderPathTextBox.Location = New System.Drawing.Point(6, 88)
        Me.FolderPathTextBox.Mandatory = False
        Me.FolderPathTextBox.Name = "FolderPathTextBox"
        Me.FolderPathTextBox.ReadOnly = True
        Me.FolderPathTextBox.Size = New System.Drawing.Size(355, 21)
        Me.FolderPathTextBox.TabIndex = 28
        '
        'FolderButton
        '
        Me.FolderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.FolderButton.Location = New System.Drawing.Point(373, 77)
        Me.FolderButton.Name = "FolderButton"
        Me.FolderButton.Size = New System.Drawing.Size(32, 32)
        Me.FolderButton.TabIndex = 1
        Me.FolderButton.UseVisualStyleBackColor = True
        '
        'RepSATFolderLabel
        '
        Me.RepSATFolderLabel.AutoSize = True
        Me.RepSATFolderLabel.BackColor = System.Drawing.Color.Transparent
        Me.RepSATFolderLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.RepSATFolderLabel.ForeColor = System.Drawing.Color.Black
        Me.RepSATFolderLabel.Location = New System.Drawing.Point(6, 168)
        Me.RepSATFolderLabel.Name = "RepSATFolderLabel"
        Me.RepSATFolderLabel.Size = New System.Drawing.Size(180, 13)
        Me.RepSATFolderLabel.TabIndex = 30
        Me.RepSATFolderLabel.Text = "*Report SAT in Current Folder"
        Me.RepSATFolderLabel.Title = False
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.Controls.Add(Me.bsSATDirListBox)
        Me.BsGroupBox1.Controls.Add(Me.RepSATFolderLabel)
        Me.BsGroupBox1.Controls.Add(Me.bsSATReportTitle)
        Me.BsGroupBox1.Controls.Add(Me.bsDeleteButton)
        Me.BsGroupBox1.Controls.Add(Me.bsSaveSATRepButton)
        Me.BsGroupBox1.Controls.Add(Me.BsTitleCreation)
        Me.BsGroupBox1.Controls.Add(Me.bsSelectAllDirCheckbox)
        Me.BsGroupBox1.Controls.Add(Me.FolderPathLbl)
        Me.BsGroupBox1.Controls.Add(Me.FolderButton)
        Me.BsGroupBox1.Controls.Add(Me.FileNameLabel)
        Me.BsGroupBox1.Controls.Add(Me.FolderPathTextBox)
        Me.BsGroupBox1.Controls.Add(Me.FileNameTextBox)
        Me.BsGroupBox1.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox1.Location = New System.Drawing.Point(12, 4)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New System.Drawing.Size(411, 342)
        Me.BsGroupBox1.TabIndex = 31
        Me.BsGroupBox1.TabStop = False
        '
        'BsErrorProvider1
        '
        Me.BsErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.BsErrorProvider1.ContainerControl = Me
        '
        'BsBorderedPanel1
        '
        Me.BsBorderedPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsBorderedPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsBorderedPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsBorderedPanel1.Name = "BsBorderedPanel1"
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(435, 395)
        Me.BsBorderedPanel1.TabIndex = 32
        '
        'ISATReport
        '
        Me.AcceptButton = Me.ExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(435, 395)
        Me.ControlBox = False
        Me.Controls.Add(Me.BsGroupBox1)
        Me.Controls.Add(Me.ExitButton)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ISATReport"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        Me.BsGroupBox1.ResumeLayout(False)
        Me.BsGroupBox1.PerformLayout()
        CType(Me.BsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsSaveSATRepButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDeleteButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSelectAllDirCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsSATDirListBox As Biosystems.Ax00.Controls.UserControls.BSCheckedListBox
    Friend WithEvents resetSaveButtonTimer As System.Windows.Forms.Timer
    Friend WithEvents ExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents BsTitleCreation As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSATReportTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents FolderPathLbl As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents FileNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents FileNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents FolderPathTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents FolderButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents RepSATFolderLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsGroupBox1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
End Class
