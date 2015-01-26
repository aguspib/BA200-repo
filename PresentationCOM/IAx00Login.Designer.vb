<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiAx00Login
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiAx00Login))
        Me.bsLoginTimer = New Biosystems.Ax00.Controls.UserControls.BSTimer
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bwPreload = New Biosystems.Ax00.Controls.UserControls.BSBackgroundWorker
        Me.Ax00MDBackGround = New Biosystems.Ax00.Controls.UserControls.BSBackgroundWorker
        Me.BsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsLoginButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsChangePwdButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsPasswordTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsUserIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsPasswordLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsUserIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsSoftwareLabel = New DevExpress.XtraEditors.LabelControl
        Me.TestSortingLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.TestSortingGB = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.LoginErrorLabel = New System.Windows.Forms.Label
        Me.bsVersionLabel = New DevExpress.XtraEditors.LabelControl
        Me.LogoBAServicePicture = New DevExpress.XtraEditors.PictureEdit
        Me.LogoBAUserPicture = New DevExpress.XtraEditors.PictureEdit
        Me.BsBorderedPanel1 = New bsBorderedPanel
        CType(Me.BsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TestSortingGB.SuspendLayout()
        CType(Me.LogoBAServicePicture.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LogoBAUserPicture.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsLoginTimer
        '
        Me.bsLoginTimer.Interval = 10000
        '
        'bwPreload
        '
        '
        'Ax00MDBackGround
        '
        '
        'BsErrorProvider1
        '
        Me.BsErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.BsErrorProvider1.ContainerControl = Me
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Location = New System.Drawing.Point(463, 266)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 13
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsLoginButton
        '
        Me.bsLoginButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsLoginButton.Location = New System.Drawing.Point(425, 266)
        Me.bsLoginButton.Name = "bsLoginButton"
        Me.bsLoginButton.Size = New System.Drawing.Size(32, 32)
        Me.bsLoginButton.TabIndex = 12
        Me.bsLoginButton.UseVisualStyleBackColor = True
        '
        'bsChangePwdButton
        '
        Me.bsChangePwdButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsChangePwdButton.Enabled = False
        Me.bsChangePwdButton.Location = New System.Drawing.Point(369, 192)
        Me.bsChangePwdButton.Name = "bsChangePwdButton"
        Me.bsChangePwdButton.Size = New System.Drawing.Size(32, 32)
        Me.bsChangePwdButton.TabIndex = 8
        Me.bsChangePwdButton.UseVisualStyleBackColor = True
        '
        'bsPasswordTextBox
        '
        Me.bsPasswordTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsPasswordTextBox.DecimalsValues = False
        Me.bsPasswordTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPasswordTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsPasswordTextBox.IsNumeric = False
        Me.bsPasswordTextBox.Location = New System.Drawing.Point(126, 199)
        Me.bsPasswordTextBox.Mandatory = True
        Me.bsPasswordTextBox.MaxLength = 10
        Me.bsPasswordTextBox.Name = "bsPasswordTextBox"
        Me.bsPasswordTextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.bsPasswordTextBox.Size = New System.Drawing.Size(231, 21)
        Me.bsPasswordTextBox.TabIndex = 5
        Me.bsPasswordTextBox.WordWrap = False
        '
        'bsUserIDTextBox
        '
        Me.bsUserIDTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsUserIDTextBox.DecimalsValues = False
        Me.bsUserIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsUserIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsUserIDTextBox.IsNumeric = False
        Me.bsUserIDTextBox.Location = New System.Drawing.Point(126, 151)
        Me.bsUserIDTextBox.Mandatory = True
        Me.bsUserIDTextBox.MaxLength = 10
        Me.bsUserIDTextBox.Name = "bsUserIDTextBox"
        Me.bsUserIDTextBox.Size = New System.Drawing.Size(231, 21)
        Me.bsUserIDTextBox.TabIndex = 3
        Me.bsUserIDTextBox.WordWrap = False
        '
        'bsPasswordLabel
        '
        Me.bsPasswordLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsPasswordLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPasswordLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPasswordLabel.Location = New System.Drawing.Point(124, 183)
        Me.bsPasswordLabel.Name = "bsPasswordLabel"
        Me.bsPasswordLabel.Size = New System.Drawing.Size(228, 13)
        Me.bsPasswordLabel.TabIndex = 4
        Me.bsPasswordLabel.Text = "Password:"
        Me.bsPasswordLabel.Title = False
        '
        'bsUserIDLabel
        '
        Me.bsUserIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsUserIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsUserIDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUserIDLabel.Location = New System.Drawing.Point(124, 135)
        Me.bsUserIDLabel.Name = "bsUserIDLabel"
        Me.bsUserIDLabel.Size = New System.Drawing.Size(228, 13)
        Me.bsUserIDLabel.TabIndex = 2
        Me.bsUserIDLabel.Text = "User ID:"
        Me.bsUserIDLabel.Title = False
        '
        'bsSoftwareLabel
        '
        Me.bsSoftwareLabel.Appearance.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsSoftwareLabel.Appearance.BackColor2 = System.Drawing.Color.Transparent
        Me.bsSoftwareLabel.Appearance.BorderColor = System.Drawing.Color.Transparent
        Me.bsSoftwareLabel.Appearance.Font = New System.Drawing.Font("Verdana", 14.0!, System.Drawing.FontStyle.Bold)
        Me.bsSoftwareLabel.Location = New System.Drawing.Point(126, 48)
        Me.bsSoftwareLabel.LookAndFeel.SkinName = "iMaginary"
        Me.bsSoftwareLabel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003
        Me.bsSoftwareLabel.Name = "bsSoftwareLabel"
        Me.bsSoftwareLabel.Size = New System.Drawing.Size(231, 23)
        Me.bsSoftwareLabel.TabIndex = 17
        Me.bsSoftwareLabel.Text = "BA400 User Software"
        '
        'TestSortingLabel
        '
        Me.TestSortingLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.TestSortingLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.TestSortingLabel.ForeColor = System.Drawing.Color.Black
        Me.TestSortingLabel.Location = New System.Drawing.Point(12, 16)
        Me.TestSortingLabel.Name = "TestSortingLabel"
        Me.TestSortingLabel.Size = New System.Drawing.Size(457, 97)
        Me.TestSortingLabel.TabIndex = 1
        Me.TestSortingLabel.Text = " "
        Me.TestSortingLabel.Title = True
        '
        'TestSortingGB
        '
        Me.TestSortingGB.Controls.Add(Me.LoginErrorLabel)
        Me.TestSortingGB.Controls.Add(Me.bsVersionLabel)
        Me.TestSortingGB.Controls.Add(Me.LogoBAServicePicture)
        Me.TestSortingGB.Controls.Add(Me.bsSoftwareLabel)
        Me.TestSortingGB.Controls.Add(Me.LogoBAUserPicture)
        Me.TestSortingGB.Controls.Add(Me.TestSortingLabel)
        Me.TestSortingGB.Controls.Add(Me.bsPasswordLabel)
        Me.TestSortingGB.Controls.Add(Me.bsUserIDLabel)
        Me.TestSortingGB.Controls.Add(Me.bsChangePwdButton)
        Me.TestSortingGB.Controls.Add(Me.bsUserIDTextBox)
        Me.TestSortingGB.Controls.Add(Me.bsPasswordTextBox)
        Me.TestSortingGB.ForeColor = System.Drawing.Color.Black
        Me.TestSortingGB.Location = New System.Drawing.Point(12, 5)
        Me.TestSortingGB.Name = "TestSortingGB"
        Me.TestSortingGB.Size = New System.Drawing.Size(482, 249)
        Me.TestSortingGB.TabIndex = 19
        Me.TestSortingGB.TabStop = False
        '
        'LoginErrorLabel
        '
        Me.LoginErrorLabel.AutoSize = True
        Me.LoginErrorLabel.BackColor = System.Drawing.Color.Transparent
        Me.LoginErrorLabel.Location = New System.Drawing.Point(371, 167)
        Me.LoginErrorLabel.Name = "LoginErrorLabel"
        Me.LoginErrorLabel.Size = New System.Drawing.Size(31, 13)
        Me.LoginErrorLabel.TabIndex = 21
        Me.LoginErrorLabel.Text = "      "
        '
        'bsVersionLabel
        '
        Me.bsVersionLabel.Appearance.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsVersionLabel.Appearance.BackColor2 = System.Drawing.Color.Transparent
        Me.bsVersionLabel.Appearance.BorderColor = System.Drawing.Color.Transparent
        Me.bsVersionLabel.Appearance.Font = New System.Drawing.Font("Verdana", 9.0!)
        Me.bsVersionLabel.Location = New System.Drawing.Point(126, 72)
        Me.bsVersionLabel.LookAndFeel.SkinName = "iMaginary"
        Me.bsVersionLabel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003
        Me.bsVersionLabel.Name = "bsVersionLabel"
        Me.bsVersionLabel.Size = New System.Drawing.Size(77, 14)
        Me.bsVersionLabel.TabIndex = 20
        Me.bsVersionLabel.Text = "Beta 0.3.3.0"
        '
        'LogoBAServicePicture
        '
        Me.LogoBAServicePicture.EditValue = CType(resources.GetObject("LogoBAServicePicture.EditValue"), Object)
        Me.LogoBAServicePicture.Location = New System.Drawing.Point(22, 22)
        Me.LogoBAServicePicture.Name = "LogoBAServicePicture"
        Me.LogoBAServicePicture.Properties.AllowFocused = False
        Me.LogoBAServicePicture.Properties.Appearance.BackColor = System.Drawing.Color.LightSteelBlue
        Me.LogoBAServicePicture.Properties.Appearance.Options.UseBackColor = True
        Me.LogoBAServicePicture.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.LogoBAServicePicture.Properties.ShowMenu = False
        Me.LogoBAServicePicture.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch
        Me.LogoBAServicePicture.Size = New System.Drawing.Size(85, 85)
        Me.LogoBAServicePicture.TabIndex = 19
        Me.LogoBAServicePicture.Visible = False
        '
        'LogoBAUserPicture
        '
        Me.LogoBAUserPicture.EditValue = CType(resources.GetObject("LogoBAUserPicture.EditValue"), Object)
        Me.LogoBAUserPicture.Location = New System.Drawing.Point(22, 22)
        Me.LogoBAUserPicture.Name = "LogoBAUserPicture"
        Me.LogoBAUserPicture.Properties.AllowFocused = False
        Me.LogoBAUserPicture.Properties.Appearance.BackColor = System.Drawing.Color.LightSteelBlue
        Me.LogoBAUserPicture.Properties.Appearance.Options.UseBackColor = True
        Me.LogoBAUserPicture.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.LogoBAUserPicture.Properties.ShowMenu = False
        Me.LogoBAUserPicture.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch
        Me.LogoBAUserPicture.Size = New System.Drawing.Size(85, 85)
        Me.LogoBAUserPicture.TabIndex = 18
        '
        'BsBorderedPanel1
        '
        Me.BsBorderedPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsBorderedPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsBorderedPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsBorderedPanel1.Name = "BsBorderedPanel1"
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(508, 310)
        Me.BsBorderedPanel1.TabIndex = 20
        '
        'IAx00Login
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(508, 310)
        Me.ControlBox = False
        Me.Controls.Add(Me.TestSortingGB)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsLoginButton)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IAx00Login"
        Me.Text = ""
        CType(Me.BsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TestSortingGB.ResumeLayout(False)
        Me.TestSortingGB.PerformLayout()
        CType(Me.LogoBAServicePicture.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LogoBAUserPicture.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsLoginTimer As Biosystems.Ax00.Controls.UserControls.BSTimer
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bwPreload As Biosystems.Ax00.Controls.UserControls.BSBackgroundWorker
    Friend WithEvents Ax00MDBackGround As Biosystems.Ax00.Controls.UserControls.BSBackgroundWorker
    Friend WithEvents BsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLoginButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsChangePwdButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsPasswordTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsUserIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsPasswordLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsUserIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSoftwareLabel As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TestSortingGB As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents TestSortingLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents LogoBAUserPicture As DevExpress.XtraEditors.PictureEdit
    Friend WithEvents LogoBAServicePicture As DevExpress.XtraEditors.PictureEdit
    Friend WithEvents bsVersionLabel As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LoginErrorLabel As System.Windows.Forms.Label
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
End Class
