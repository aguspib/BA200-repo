<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ICreateRestorePoint
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ICreateRestorePoint))
        Me.resetSaveButtonTimer = New System.Windows.Forms.Timer(Me.components)
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsColorDialog1 = New Biosystems.Ax00.Controls.UserControls.BSColorDialog
        Me.bsCreateRestorePointGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsFileNameTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsRestorepointNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsCreateButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsBorderedPanel1 = New bsBorderedPanel
        Me.bsCreateRestorePointGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsCancelButton
        '
        Me.bsCancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsCancelButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCancelButton.Location = New System.Drawing.Point(426, 137)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 28
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsCreateRestorePointGroupBox
        '
        Me.bsCreateRestorePointGroupBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsCreateRestorePointGroupBox.Controls.Add(Me.bsFileNameTitleLabel)
        Me.bsCreateRestorePointGroupBox.Controls.Add(Me.bsRestorepointNameTextBox)
        Me.bsCreateRestorePointGroupBox.Controls.Add(Me.bsCreateButton)
        Me.bsCreateRestorePointGroupBox.Controls.Add(Me.bsTitle)
        Me.bsCreateRestorePointGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsCreateRestorePointGroupBox.Location = New System.Drawing.Point(10, 5)
        Me.bsCreateRestorePointGroupBox.Name = "bsCreateRestorePointGroupBox"
        Me.bsCreateRestorePointGroupBox.Size = New System.Drawing.Size(448, 122)
        Me.bsCreateRestorePointGroupBox.TabIndex = 36
        Me.bsCreateRestorePointGroupBox.TabStop = False
        '
        'bsFileNameTitleLabel
        '
        Me.bsFileNameTitleLabel.AutoSize = True
        Me.bsFileNameTitleLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsFileNameTitleLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFileNameTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsFileNameTitleLabel.Location = New System.Drawing.Point(10, 52)
        Me.bsFileNameTitleLabel.Name = "bsFileNameTitleLabel"
        Me.bsFileNameTitleLabel.Size = New System.Drawing.Size(74, 13)
        Me.bsFileNameTitleLabel.TabIndex = 40
        Me.bsFileNameTitleLabel.Text = "*File name:"
        Me.bsFileNameTitleLabel.Title = False
        '
        'bsRestorepointNameTextBox
        '
        Me.bsRestorepointNameTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsRestorepointNameTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsRestorepointNameTextBox.DecimalsValues = False
        Me.bsRestorepointNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRestorepointNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsRestorepointNameTextBox.IsNumeric = False
        Me.bsRestorepointNameTextBox.Location = New System.Drawing.Point(10, 68)
        Me.bsRestorepointNameTextBox.Mandatory = True
        Me.bsRestorepointNameTextBox.MaxLength = 16
        Me.bsRestorepointNameTextBox.Name = "bsRestorepointNameTextBox"
        Me.bsRestorepointNameTextBox.ReadOnly = True
        Me.bsRestorepointNameTextBox.Size = New System.Drawing.Size(388, 21)
        Me.bsRestorepointNameTextBox.TabIndex = 38
        Me.bsRestorepointNameTextBox.WordWrap = False
        '
        'bsCreateButton
        '
        Me.bsCreateButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsCreateButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCreateButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCreateButton.ForeColor = System.Drawing.Color.Indigo
        Me.bsCreateButton.Location = New System.Drawing.Point(401, 57)
        Me.bsCreateButton.Name = "bsCreateButton"
        Me.bsCreateButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCreateButton.TabIndex = 36
        Me.bsCreateButton.UseVisualStyleBackColor = True
        '
        'bsTitle
        '
        Me.bsTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTitle.ForeColor = System.Drawing.Color.Black
        Me.bsTitle.Location = New System.Drawing.Point(10, 15)
        Me.bsTitle.Name = "bsTitle"
        Me.bsTitle.Size = New System.Drawing.Size(426, 20)
        Me.bsTitle.TabIndex = 7
        Me.bsTitle.Text = "*Create Restore Point with the current data"
        Me.bsTitle.Title = True
        '
        'BsBorderedPanel1
        '
        Me.BsBorderedPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsBorderedPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsBorderedPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsBorderedPanel1.Name = "BsBorderedPanel1"
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(470, 180)
        Me.BsBorderedPanel1.TabIndex = 37
        '
        'ICreateRestorePoint
        '
        Me.AcceptButton = Me.bsCancelButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.CancelButton = Me.bsCancelButton
        Me.ClientSize = New System.Drawing.Size(470, 180)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsCreateRestorePointGroupBox)
        Me.Controls.Add(Me.bsCancelButton)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ICreateRestorePoint"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsCreateRestorePointGroupBox.ResumeLayout(False)
        Me.bsCreateRestorePointGroupBox.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents resetSaveButtonTimer As System.Windows.Forms.Timer
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsColorDialog1 As Biosystems.Ax00.Controls.UserControls.BSColorDialog
    Friend WithEvents bsCreateRestorePointGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRestorepointNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsCreateButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
    Friend WithEvents bsFileNameTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
End Class
