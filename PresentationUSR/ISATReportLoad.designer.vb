<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ISATReportLoad
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ISATReportLoad))
        Me.resetSaveButtonTimer = New System.Windows.Forms.Timer()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsColorDialog1 = New Biosystems.Ax00.Controls.UserControls.BSColorDialog()
        Me.bsOKButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsLoadRestoreSATGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsSATDirListBox = New Biosystems.Ax00.Controls.UserControls.BSListBox()
        Me.bsRestoreLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsAllowRestoreCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsSelectedTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSelectlabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsBrowseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsBorderedPanel1 = New bsBorderedPanel()
        Me.bsLoadRestoreSATGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsCancelButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCancelButton.Location = New System.Drawing.Point(376, 169)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 28
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsOKButton
        '
        Me.bsOKButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsOKButton.Enabled = False
        Me.bsOKButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsOKButton.Location = New System.Drawing.Point(339, 169)
        Me.bsOKButton.Name = "bsOKButton"
        Me.bsOKButton.Size = New System.Drawing.Size(32, 32)
        Me.bsOKButton.TabIndex = 33
        Me.bsOKButton.UseVisualStyleBackColor = True
        '
        'bsLoadRestoreSATGroupBox
        '
        Me.bsLoadRestoreSATGroupBox.Controls.Add(Me.bsSATDirListBox)
        Me.bsLoadRestoreSATGroupBox.Controls.Add(Me.bsRestoreLabel)
        Me.bsLoadRestoreSATGroupBox.Controls.Add(Me.bsAllowRestoreCheckbox)
        Me.bsLoadRestoreSATGroupBox.Controls.Add(Me.bsSelectedTextBox)
        Me.bsLoadRestoreSATGroupBox.Controls.Add(Me.bsSelectlabel)
        Me.bsLoadRestoreSATGroupBox.Controls.Add(Me.bsBrowseButton)
        Me.bsLoadRestoreSATGroupBox.Controls.Add(Me.bsTitle)
        Me.bsLoadRestoreSATGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLoadRestoreSATGroupBox.Location = New System.Drawing.Point(10, 5)
        Me.bsLoadRestoreSATGroupBox.Name = "bsLoadRestoreSATGroupBox"
        Me.bsLoadRestoreSATGroupBox.Size = New System.Drawing.Size(398, 158)
        Me.bsLoadRestoreSATGroupBox.TabIndex = 36
        Me.bsLoadRestoreSATGroupBox.TabStop = False
        '
        'bsSATDirListBox
        '
        Me.bsSATDirListBox.ForeColor = System.Drawing.Color.Black
        Me.bsSATDirListBox.FormattingEnabled = True
        Me.bsSATDirListBox.Location = New System.Drawing.Point(10, 42)
        Me.bsSATDirListBox.Name = "bsSATDirListBox"
        Me.bsSATDirListBox.Size = New System.Drawing.Size(376, 108)
        Me.bsSATDirListBox.TabIndex = 41
        Me.bsSATDirListBox.Visible = False
        '
        'bsRestoreLabel
        '
        Me.bsRestoreLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRestoreLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRestoreLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRestoreLabel.Location = New System.Drawing.Point(7, 49)
        Me.bsRestoreLabel.Name = "bsRestoreLabel"
        Me.bsRestoreLabel.Size = New System.Drawing.Size(335, 13)
        Me.bsRestoreLabel.TabIndex = 40
        Me.bsRestoreLabel.Text = "Select the system point to restore:"
        Me.bsRestoreLabel.Title = False
        Me.bsRestoreLabel.Visible = False
        '
        'bsAllowRestoreCheckbox
        '
        Me.bsAllowRestoreCheckbox.AutoSize = True
        Me.bsAllowRestoreCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAllowRestoreCheckbox.Location = New System.Drawing.Point(10, 104)
        Me.bsAllowRestoreCheckbox.Name = "bsAllowRestoreCheckbox"
        Me.bsAllowRestoreCheckbox.Size = New System.Drawing.Size(234, 17)
        Me.bsAllowRestoreCheckbox.TabIndex = 39
        Me.bsAllowRestoreCheckbox.Text = "Allow me to restore my current data"
        Me.bsAllowRestoreCheckbox.UseVisualStyleBackColor = True
        '
        'bsSelectedTextBox
        '
        Me.bsSelectedTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsSelectedTextBox.DecimalsValues = False
        Me.bsSelectedTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSelectedTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsSelectedTextBox.IsNumeric = False
        Me.bsSelectedTextBox.Location = New System.Drawing.Point(10, 68)
        Me.bsSelectedTextBox.Mandatory = True
        Me.bsSelectedTextBox.MaxLength = 16
        Me.bsSelectedTextBox.Name = "bsSelectedTextBox"
        Me.bsSelectedTextBox.ReadOnly = True
        Me.bsSelectedTextBox.Size = New System.Drawing.Size(335, 21)
        Me.bsSelectedTextBox.TabIndex = 38
        Me.bsSelectedTextBox.WordWrap = False
        '
        'bsSelectlabel
        '
        Me.bsSelectlabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSelectlabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSelectlabel.ForeColor = System.Drawing.Color.Black
        Me.bsSelectlabel.Location = New System.Drawing.Point(10, 49)
        Me.bsSelectlabel.Name = "bsSelectlabel"
        Me.bsSelectlabel.Size = New System.Drawing.Size(337, 13)
        Me.bsSelectlabel.TabIndex = 37
        Me.bsSelectlabel.Text = "Select the SAT Report to load:"
        Me.bsSelectlabel.Title = False
        '
        'bsBrowseButton
        '
        Me.bsBrowseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsBrowseButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsBrowseButton.ForeColor = System.Drawing.Color.Indigo
        Me.bsBrowseButton.Location = New System.Drawing.Point(351, 57)
        Me.bsBrowseButton.Name = "bsBrowseButton"
        Me.bsBrowseButton.Size = New System.Drawing.Size(32, 32)
        Me.bsBrowseButton.TabIndex = 36
        Me.bsBrowseButton.UseVisualStyleBackColor = True
        '
        'bsTitle
        '
        Me.bsTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTitle.ForeColor = System.Drawing.Color.Black
        Me.bsTitle.Location = New System.Drawing.Point(10, 15)
        Me.bsTitle.Name = "bsTitle"
        Me.bsTitle.Size = New System.Drawing.Size(376, 20)
        Me.bsTitle.TabIndex = 7
        Me.bsTitle.Text = "Load or Restore Label"
        Me.bsTitle.Title = True
        '
        'BsBorderedPanel1
        '
        Me.BsBorderedPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsBorderedPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsBorderedPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsBorderedPanel1.Name = "BsBorderedPanel1"
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(420, 210)
        Me.BsBorderedPanel1.TabIndex = 37
        '
        'ISATReportLoad
        '
        Me.AcceptButton = Me.bsCancelButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.CancelButton = Me.bsCancelButton
        Me.ClientSize = New System.Drawing.Size(420, 210)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsLoadRestoreSATGroupBox)
        Me.Controls.Add(Me.bsOKButton)
        Me.Controls.Add(Me.bsCancelButton)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ISATReportLoad"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsLoadRestoreSATGroupBox.ResumeLayout(False)
        Me.bsLoadRestoreSATGroupBox.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents resetSaveButtonTimer As System.Windows.Forms.Timer
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsColorDialog1 As Biosystems.Ax00.Controls.UserControls.BSColorDialog
    Friend WithEvents bsOKButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLoadRestoreSATGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSATDirListBox As Biosystems.Ax00.Controls.UserControls.BSListBox
    Friend WithEvents bsRestoreLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAllowRestoreCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsSelectedTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsSelectlabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsBrowseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
End Class
