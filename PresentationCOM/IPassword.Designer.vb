<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IPassword
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IPassword))
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsChangePwdGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsUserIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsPwdConfirmTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsNewPasswordTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsCurPasswordTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsChangePwdDefLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsCurPasswordLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsNewPasswordLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsPwdConfirmLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsUserNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.bsChangePwdGroupBox.SuspendLayout()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(228, 261)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 6
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.DialogResult = System.Windows.Forms.DialogResult.Yes
        Me.bsAcceptButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAcceptButton.Location = New System.Drawing.Point(191, 261)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 5
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'bsChangePwdGroupBox
        '
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsUserIDTextBox)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsPwdConfirmTextBox)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsNewPasswordTextBox)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsCurPasswordTextBox)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsChangePwdDefLabel)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsCurPasswordLabel)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsNewPasswordLabel)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsPwdConfirmLabel)
        Me.bsChangePwdGroupBox.Controls.Add(Me.bsUserNameLabel)
        Me.bsChangePwdGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsChangePwdGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsChangePwdGroupBox.Name = "bsChangePwdGroupBox"
        Me.bsChangePwdGroupBox.Size = New System.Drawing.Size(250, 246)
        Me.bsChangePwdGroupBox.TabIndex = 0
        Me.bsChangePwdGroupBox.TabStop = False
        '
        'bsUserIDTextBox
        '
        Me.bsUserIDTextBox.BackColor = System.Drawing.Color.White
        Me.bsUserIDTextBox.DecimalsValues = False
        Me.bsUserIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsUserIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsUserIDTextBox.IsNumeric = False
        Me.bsUserIDTextBox.Location = New System.Drawing.Point(10, 78)
        Me.bsUserIDTextBox.Mandatory = False
        Me.bsUserIDTextBox.MaxLength = 10
        Me.bsUserIDTextBox.Name = "bsUserIDTextBox"
        Me.bsUserIDTextBox.ReadOnly = True
        Me.bsUserIDTextBox.Size = New System.Drawing.Size(230, 21)
        Me.bsUserIDTextBox.TabIndex = 1
        Me.bsUserIDTextBox.TabStop = False
        Me.bsUserIDTextBox.WordWrap = False
        '
        'bsPwdConfirmTextBox
        '
        Me.bsPwdConfirmTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsPwdConfirmTextBox.DecimalsValues = False
        Me.bsPwdConfirmTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPwdConfirmTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsPwdConfirmTextBox.IsNumeric = False
        Me.bsPwdConfirmTextBox.Location = New System.Drawing.Point(10, 210)
        Me.bsPwdConfirmTextBox.Mandatory = True
        Me.bsPwdConfirmTextBox.MaxLength = 10
        Me.bsPwdConfirmTextBox.Name = "bsPwdConfirmTextBox"
        Me.bsPwdConfirmTextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.bsPwdConfirmTextBox.Size = New System.Drawing.Size(215, 21)
        Me.bsPwdConfirmTextBox.TabIndex = 4
        Me.bsPwdConfirmTextBox.WordWrap = False
        '
        'bsNewPasswordTextBox
        '
        Me.bsNewPasswordTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsNewPasswordTextBox.DecimalsValues = False
        Me.bsNewPasswordTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNewPasswordTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsNewPasswordTextBox.IsNumeric = False
        Me.bsNewPasswordTextBox.Location = New System.Drawing.Point(10, 166)
        Me.bsNewPasswordTextBox.Mandatory = True
        Me.bsNewPasswordTextBox.MaxLength = 10
        Me.bsNewPasswordTextBox.Name = "bsNewPasswordTextBox"
        Me.bsNewPasswordTextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.bsNewPasswordTextBox.Size = New System.Drawing.Size(215, 21)
        Me.bsNewPasswordTextBox.TabIndex = 3
        Me.bsNewPasswordTextBox.WordWrap = False
        '
        'bsCurPasswordTextBox
        '
        Me.bsCurPasswordTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsCurPasswordTextBox.DecimalsValues = False
        Me.bsCurPasswordTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCurPasswordTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsCurPasswordTextBox.IsNumeric = False
        Me.bsCurPasswordTextBox.Location = New System.Drawing.Point(10, 122)
        Me.bsCurPasswordTextBox.Mandatory = True
        Me.bsCurPasswordTextBox.MaxLength = 10
        Me.bsCurPasswordTextBox.Name = "bsCurPasswordTextBox"
        Me.bsCurPasswordTextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.bsCurPasswordTextBox.Size = New System.Drawing.Size(215, 21)
        Me.bsCurPasswordTextBox.TabIndex = 2
        Me.bsCurPasswordTextBox.WordWrap = False
        '
        'bsChangePwdDefLabel
        '
        Me.bsChangePwdDefLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsChangePwdDefLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsChangePwdDefLabel.ForeColor = System.Drawing.Color.Black
        Me.bsChangePwdDefLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsChangePwdDefLabel.Name = "bsChangePwdDefLabel"
        Me.bsChangePwdDefLabel.Size = New System.Drawing.Size(230, 20)
        Me.bsChangePwdDefLabel.TabIndex = 2
        Me.bsChangePwdDefLabel.Text = "Change Password"
        Me.bsChangePwdDefLabel.Title = True
        '
        'bsCurPasswordLabel
        '
        Me.bsCurPasswordLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsCurPasswordLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.bsCurPasswordLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCurPasswordLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCurPasswordLabel.Location = New System.Drawing.Point(10, 104)
        Me.bsCurPasswordLabel.Name = "bsCurPasswordLabel"
        Me.bsCurPasswordLabel.Size = New System.Drawing.Size(218, 13)
        Me.bsCurPasswordLabel.TabIndex = 8
        Me.bsCurPasswordLabel.Text = "Current Password:"
        Me.bsCurPasswordLabel.Title = False
        '
        'bsNewPasswordLabel
        '
        Me.bsNewPasswordLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNewPasswordLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNewPasswordLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNewPasswordLabel.Location = New System.Drawing.Point(10, 148)
        Me.bsNewPasswordLabel.Name = "bsNewPasswordLabel"
        Me.bsNewPasswordLabel.Size = New System.Drawing.Size(218, 13)
        Me.bsNewPasswordLabel.TabIndex = 9
        Me.bsNewPasswordLabel.Text = "New Password:"
        Me.bsNewPasswordLabel.Title = False
        '
        'bsPwdConfirmLabel
        '
        Me.bsPwdConfirmLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsPwdConfirmLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPwdConfirmLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPwdConfirmLabel.Location = New System.Drawing.Point(10, 192)
        Me.bsPwdConfirmLabel.Name = "bsPwdConfirmLabel"
        Me.bsPwdConfirmLabel.Size = New System.Drawing.Size(218, 13)
        Me.bsPwdConfirmLabel.TabIndex = 10
        Me.bsPwdConfirmLabel.Text = "Confirm Password:"
        Me.bsPwdConfirmLabel.Title = False
        '
        'bsUserNameLabel
        '
        Me.bsUserNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsUserNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsUserNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUserNameLabel.Location = New System.Drawing.Point(10, 60)
        Me.bsUserNameLabel.Name = "bsUserNameLabel"
        Me.bsUserNameLabel.Size = New System.Drawing.Size(218, 13)
        Me.bsUserNameLabel.TabIndex = 3
        Me.bsUserNameLabel.Text = "User ID:"
        Me.bsUserNameLabel.Title = False
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'IPassword
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(274, 305)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsAcceptButton)
        Me.Controls.Add(Me.bsChangePwdGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IPassword"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsChangePwdGroupBox.ResumeLayout(False)
        Me.bsChangePwdGroupBox.PerformLayout()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsChangePwdGroupBox As Biosystems.Ax00.Controls.Usercontrols.BSGroupBox
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsChangePwdDefLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsUserNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsPwdConfirmTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsNewPasswordTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsCurPasswordTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsPwdConfirmLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsNewPasswordLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsCurPasswordLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsUserIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
End Class
