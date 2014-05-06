Namespace Biosystems.Ax00.Controls.UserControls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    <Drawing.ToolboxBitmap(GetType(System.Windows.Forms.Panel))> _
    Partial Class BSISEMonitorPanel
        Inherits System.Windows.Forms.UserControl

        'Control overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Control Designer
        Private components As System.ComponentModel.IContainer

        ' NOTE: The following procedure is required by the Component Designer
        ' It can be modified using the Component Designer.  Do not modify it
        ' using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.ReagentsPackGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
            Me.VolumeGB = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
            Me.RPCalBWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.RPCalAWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.RPInitialCalBTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.RPInitialCalATextEdit = New DevExpress.XtraEditors.TextEdit
            Me.RPRemainingCalBTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.RPCalBIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.RPRemainingCalATextEdit = New DevExpress.XtraEditors.TextEdit
            Me.RPCalAIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.RPInitialVolumeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.RPRemainingLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.RPCalibratorBLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.RPCalibratorALabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.RPExpiredWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.RPNotInstalledWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.RPInstalledIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.RPExpiredIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.RPExpireDateTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.RPInstallDateTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.RPExpireDateLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.RPInstallDateLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.RPTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ElectrodesGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
            Me.ELLiWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELClWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELKWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELNaWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELKTestTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.ELLiTestTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.ELRefTestTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.ELLithiumLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELClTestTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.ELNaTestTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.ELLiInstallTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.ELRefInstallTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.ELClInstallTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.ELKInstallTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.ELNaInstallTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.ELRefWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELRefIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.ELLiIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.ELClIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.ELKIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.ELNaIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.ELTestCompletedLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELInstallDateLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELSodiumLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELPotassiumLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELChlorineLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELReferenceLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ELTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CalibrationsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
            Me.CALBubbleWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CALBubbleResultTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.CALBubbleDateTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.CALBubbleIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.CALBubbleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CALCleanWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CALPumpsWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CALElectrodesWarningLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CALElectrodesIcon1 = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.CALElectrodesResult2TextEdit = New DevExpress.XtraEditors.TextEdit
            Me.CALPumpsResultTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.CALElectrodesResult1TextEdit = New DevExpress.XtraEditors.TextEdit
            Me.CALCleanDateTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.CALPumpsDateTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.CALElectrodesDateTextEdit = New DevExpress.XtraEditors.TextEdit
            Me.CALCleanIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.CALCleanLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CALPumpsIcon = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
            Me.CALResultLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CALLastDateLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CALPumpsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CALElectrodesLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.CALTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.ReagentsPackGroupBox.SuspendLayout()
            Me.VolumeGB.SuspendLayout()
            CType(Me.RPInitialCalBTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.RPInitialCalATextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.RPRemainingCalBTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.RPCalBIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.RPRemainingCalATextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.RPCalAIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.RPInstalledIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.RPExpiredIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.RPExpireDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.RPInstallDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.ElectrodesGroupBox.SuspendLayout()
            CType(Me.ELKTestTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELLiTestTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELRefTestTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELClTestTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELNaTestTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELLiInstallTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELRefInstallTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELClInstallTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELKInstallTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELNaInstallTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELRefIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELLiIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELClIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELKIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.ELNaIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.CalibrationsGroupBox.SuspendLayout()
            CType(Me.CALBubbleResultTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALBubbleDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALBubbleIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALElectrodesIcon1, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALElectrodesResult2TextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALPumpsResultTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALElectrodesResult1TextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALCleanDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALPumpsDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALElectrodesDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALCleanIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.CALPumpsIcon, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'ReagentsPackGroupBox
            '
            Me.ReagentsPackGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ReagentsPackGroupBox.BackColor = System.Drawing.Color.Transparent
            Me.ReagentsPackGroupBox.Controls.Add(Me.VolumeGB)
            Me.ReagentsPackGroupBox.Controls.Add(Me.RPExpiredWarningLabel)
            Me.ReagentsPackGroupBox.Controls.Add(Me.RPNotInstalledWarningLabel)
            Me.ReagentsPackGroupBox.Controls.Add(Me.RPInstalledIcon)
            Me.ReagentsPackGroupBox.Controls.Add(Me.RPExpiredIcon)
            Me.ReagentsPackGroupBox.Controls.Add(Me.RPExpireDateTextEdit)
            Me.ReagentsPackGroupBox.Controls.Add(Me.RPInstallDateTextEdit)
            Me.ReagentsPackGroupBox.Controls.Add(Me.RPExpireDateLabel)
            Me.ReagentsPackGroupBox.Controls.Add(Me.RPInstallDateLabel)
            Me.ReagentsPackGroupBox.Controls.Add(Me.RPTitleLabel)
            Me.ReagentsPackGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ReagentsPackGroupBox.ForeColor = System.Drawing.Color.Black
            Me.ReagentsPackGroupBox.Location = New System.Drawing.Point(5, -4)
            Me.ReagentsPackGroupBox.Name = "ReagentsPackGroupBox"
            Me.ReagentsPackGroupBox.Size = New System.Drawing.Size(754, 200)
            Me.ReagentsPackGroupBox.TabIndex = 49
            Me.ReagentsPackGroupBox.TabStop = False
            '
            'VolumeGB
            '
            Me.VolumeGB.Controls.Add(Me.RPCalBWarningLabel)
            Me.VolumeGB.Controls.Add(Me.RPCalAWarningLabel)
            Me.VolumeGB.Controls.Add(Me.RPInitialCalBTextEdit)
            Me.VolumeGB.Controls.Add(Me.RPInitialCalATextEdit)
            Me.VolumeGB.Controls.Add(Me.RPRemainingCalBTextEdit)
            Me.VolumeGB.Controls.Add(Me.RPCalBIcon)
            Me.VolumeGB.Controls.Add(Me.RPRemainingCalATextEdit)
            Me.VolumeGB.Controls.Add(Me.RPCalAIcon)
            Me.VolumeGB.Controls.Add(Me.RPInitialVolumeLabel)
            Me.VolumeGB.Controls.Add(Me.RPRemainingLabel)
            Me.VolumeGB.Controls.Add(Me.RPCalibratorBLabel)
            Me.VolumeGB.Controls.Add(Me.RPCalibratorALabel)
            Me.VolumeGB.ForeColor = System.Drawing.Color.Black
            Me.VolumeGB.Location = New System.Drawing.Point(8, 102)
            Me.VolumeGB.Name = "VolumeGB"
            Me.VolumeGB.Size = New System.Drawing.Size(738, 92)
            Me.VolumeGB.TabIndex = 110
            Me.VolumeGB.TabStop = False
            Me.VolumeGB.Text = "Volume"
            '
            'RPCalBWarningLabel
            '
            Me.RPCalBWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.RPCalBWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.RPCalBWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.RPCalBWarningLabel.Location = New System.Drawing.Point(323, 60)
            Me.RPCalBWarningLabel.Name = "RPCalBWarningLabel"
            Me.RPCalBWarningLabel.Size = New System.Drawing.Size(405, 20)
            Me.RPCalBWarningLabel.TabIndex = 118
            Me.RPCalBWarningLabel.Text = "----"
            Me.RPCalBWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.RPCalBWarningLabel.Title = False
            '
            'RPCalAWarningLabel
            '
            Me.RPCalAWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.RPCalAWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.RPCalAWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.RPCalAWarningLabel.Location = New System.Drawing.Point(323, 34)
            Me.RPCalAWarningLabel.Name = "RPCalAWarningLabel"
            Me.RPCalAWarningLabel.Size = New System.Drawing.Size(405, 20)
            Me.RPCalAWarningLabel.TabIndex = 117
            Me.RPCalAWarningLabel.Text = "----"
            Me.RPCalAWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.RPCalAWarningLabel.Title = False
            '
            'RPInitialCalBTextEdit
            '
            Me.RPInitialCalBTextEdit.CausesValidation = False
            Me.RPInitialCalBTextEdit.EditValue = "190"
            Me.RPInitialCalBTextEdit.Enabled = False
            Me.RPInitialCalBTextEdit.Location = New System.Drawing.Point(198, 60)
            Me.RPInitialCalBTextEdit.Name = "RPInitialCalBTextEdit"
            Me.RPInitialCalBTextEdit.Properties.AllowFocused = False
            Me.RPInitialCalBTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.RPInitialCalBTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.RPInitialCalBTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.RPInitialCalBTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.RPInitialCalBTextEdit.Properties.Appearance.Options.UseFont = True
            Me.RPInitialCalBTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.RPInitialCalBTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.RPInitialCalBTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Me.RPInitialCalBTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.RPInitialCalBTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.RPInitialCalBTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.RPInitialCalBTextEdit.Properties.ReadOnly = True
            Me.RPInitialCalBTextEdit.ShowToolTips = False
            Me.RPInitialCalBTextEdit.Size = New System.Drawing.Size(87, 22)
            Me.RPInitialCalBTextEdit.TabIndex = 116
            '
            'RPInitialCalATextEdit
            '
            Me.RPInitialCalATextEdit.CausesValidation = False
            Me.RPInitialCalATextEdit.EditValue = "520"
            Me.RPInitialCalATextEdit.Enabled = False
            Me.RPInitialCalATextEdit.Location = New System.Drawing.Point(198, 34)
            Me.RPInitialCalATextEdit.Name = "RPInitialCalATextEdit"
            Me.RPInitialCalATextEdit.Properties.AllowFocused = False
            Me.RPInitialCalATextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.RPInitialCalATextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.RPInitialCalATextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.RPInitialCalATextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.RPInitialCalATextEdit.Properties.Appearance.Options.UseFont = True
            Me.RPInitialCalATextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.RPInitialCalATextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.RPInitialCalATextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Me.RPInitialCalATextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.RPInitialCalATextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.RPInitialCalATextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.RPInitialCalATextEdit.Properties.ReadOnly = True
            Me.RPInitialCalATextEdit.ShowToolTips = False
            Me.RPInitialCalATextEdit.Size = New System.Drawing.Size(87, 22)
            Me.RPInitialCalATextEdit.TabIndex = 115
            '
            'RPRemainingCalBTextEdit
            '
            Me.RPRemainingCalBTextEdit.CausesValidation = False
            Me.RPRemainingCalBTextEdit.EditValue = "% 15"
            Me.RPRemainingCalBTextEdit.Enabled = False
            Me.RPRemainingCalBTextEdit.Location = New System.Drawing.Point(107, 60)
            Me.RPRemainingCalBTextEdit.Name = "RPRemainingCalBTextEdit"
            Me.RPRemainingCalBTextEdit.Properties.AllowFocused = False
            Me.RPRemainingCalBTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.RPRemainingCalBTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.RPRemainingCalBTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.RPRemainingCalBTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.RPRemainingCalBTextEdit.Properties.Appearance.Options.UseFont = True
            Me.RPRemainingCalBTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.RPRemainingCalBTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.RPRemainingCalBTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Me.RPRemainingCalBTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.RPRemainingCalBTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.RPRemainingCalBTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.RPRemainingCalBTextEdit.Properties.ReadOnly = True
            Me.RPRemainingCalBTextEdit.ShowToolTips = False
            Me.RPRemainingCalBTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.RPRemainingCalBTextEdit.TabIndex = 114
            '
            'RPCalBIcon
            '
            Me.RPCalBIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.RPCalBIcon.BackColor = System.Drawing.Color.Transparent
            Me.RPCalBIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.RPCalBIcon.Location = New System.Drawing.Point(302, 60)
            Me.RPCalBIcon.Name = "RPCalBIcon"
            Me.RPCalBIcon.PositionNumber = 0
            Me.RPCalBIcon.Size = New System.Drawing.Size(21, 20)
            Me.RPCalBIcon.TabIndex = 112
            Me.RPCalBIcon.TabStop = False
            Me.RPCalBIcon.Visible = False
            '
            'RPRemainingCalATextEdit
            '
            Me.RPRemainingCalATextEdit.CausesValidation = False
            Me.RPRemainingCalATextEdit.EditValue = "% 23"
            Me.RPRemainingCalATextEdit.Enabled = False
            Me.RPRemainingCalATextEdit.Location = New System.Drawing.Point(107, 34)
            Me.RPRemainingCalATextEdit.Name = "RPRemainingCalATextEdit"
            Me.RPRemainingCalATextEdit.Properties.AllowFocused = False
            Me.RPRemainingCalATextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.RPRemainingCalATextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.RPRemainingCalATextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.RPRemainingCalATextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.RPRemainingCalATextEdit.Properties.Appearance.Options.UseFont = True
            Me.RPRemainingCalATextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.RPRemainingCalATextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.RPRemainingCalATextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Me.RPRemainingCalATextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.RPRemainingCalATextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.RPRemainingCalATextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.RPRemainingCalATextEdit.Properties.ReadOnly = True
            Me.RPRemainingCalATextEdit.ShowToolTips = False
            Me.RPRemainingCalATextEdit.Size = New System.Drawing.Size(85, 22)
            Me.RPRemainingCalATextEdit.TabIndex = 113
            '
            'RPCalAIcon
            '
            Me.RPCalAIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.RPCalAIcon.BackColor = System.Drawing.Color.Transparent
            Me.RPCalAIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.RPCalAIcon.Location = New System.Drawing.Point(302, 34)
            Me.RPCalAIcon.Name = "RPCalAIcon"
            Me.RPCalAIcon.PositionNumber = 0
            Me.RPCalAIcon.Size = New System.Drawing.Size(21, 20)
            Me.RPCalAIcon.TabIndex = 111
            Me.RPCalAIcon.TabStop = False
            Me.RPCalAIcon.Visible = False
            '
            'RPInitialVolumeLabel
            '
            Me.RPInitialVolumeLabel.BackColor = System.Drawing.Color.Transparent
            Me.RPInitialVolumeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.RPInitialVolumeLabel.ForeColor = System.Drawing.Color.Black
            Me.RPInitialVolumeLabel.Location = New System.Drawing.Point(198, 13)
            Me.RPInitialVolumeLabel.Name = "RPInitialVolumeLabel"
            Me.RPInitialVolumeLabel.Size = New System.Drawing.Size(103, 20)
            Me.RPInitialVolumeLabel.TabIndex = 110
            Me.RPInitialVolumeLabel.Text = "Initial "
            Me.RPInitialVolumeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.RPInitialVolumeLabel.Title = False
            '
            'RPRemainingLabel
            '
            Me.RPRemainingLabel.BackColor = System.Drawing.Color.Transparent
            Me.RPRemainingLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.RPRemainingLabel.ForeColor = System.Drawing.Color.Black
            Me.RPRemainingLabel.Location = New System.Drawing.Point(92, 13)
            Me.RPRemainingLabel.Name = "RPRemainingLabel"
            Me.RPRemainingLabel.Size = New System.Drawing.Size(103, 20)
            Me.RPRemainingLabel.TabIndex = 109
            Me.RPRemainingLabel.Text = "Remaining"
            Me.RPRemainingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.RPRemainingLabel.Title = False
            '
            'RPCalibratorBLabel
            '
            Me.RPCalibratorBLabel.BackColor = System.Drawing.Color.Transparent
            Me.RPCalibratorBLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.RPCalibratorBLabel.ForeColor = System.Drawing.Color.Black
            Me.RPCalibratorBLabel.Location = New System.Drawing.Point(5, 60)
            Me.RPCalibratorBLabel.Name = "RPCalibratorBLabel"
            Me.RPCalibratorBLabel.Size = New System.Drawing.Size(96, 20)
            Me.RPCalibratorBLabel.TabIndex = 108
            Me.RPCalibratorBLabel.Text = "Calibrator B:"
            Me.RPCalibratorBLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.RPCalibratorBLabel.Title = False
            '
            'RPCalibratorALabel
            '
            Me.RPCalibratorALabel.BackColor = System.Drawing.Color.Transparent
            Me.RPCalibratorALabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.RPCalibratorALabel.ForeColor = System.Drawing.Color.Black
            Me.RPCalibratorALabel.Location = New System.Drawing.Point(6, 34)
            Me.RPCalibratorALabel.Name = "RPCalibratorALabel"
            Me.RPCalibratorALabel.Size = New System.Drawing.Size(96, 20)
            Me.RPCalibratorALabel.TabIndex = 107
            Me.RPCalibratorALabel.Text = "Calibrator A:"
            Me.RPCalibratorALabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.RPCalibratorALabel.Title = False
            '
            'RPExpiredWarningLabel
            '
            Me.RPExpiredWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.RPExpiredWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.RPExpiredWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.RPExpiredWarningLabel.Location = New System.Drawing.Point(334, 74)
            Me.RPExpiredWarningLabel.Name = "RPExpiredWarningLabel"
            Me.RPExpiredWarningLabel.Size = New System.Drawing.Size(412, 22)
            Me.RPExpiredWarningLabel.TabIndex = 109
            Me.RPExpiredWarningLabel.Text = "----"
            Me.RPExpiredWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.RPExpiredWarningLabel.Title = False
            '
            'RPNotInstalledWarningLabel
            '
            Me.RPNotInstalledWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.RPNotInstalledWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.RPNotInstalledWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.RPNotInstalledWarningLabel.Location = New System.Drawing.Point(337, 46)
            Me.RPNotInstalledWarningLabel.Name = "RPNotInstalledWarningLabel"
            Me.RPNotInstalledWarningLabel.Size = New System.Drawing.Size(400, 20)
            Me.RPNotInstalledWarningLabel.TabIndex = 108
            Me.RPNotInstalledWarningLabel.Text = "----"
            Me.RPNotInstalledWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.RPNotInstalledWarningLabel.Title = False
            '
            'RPInstalledIcon
            '
            Me.RPInstalledIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.RPInstalledIcon.BackColor = System.Drawing.Color.Transparent
            Me.RPInstalledIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.RPInstalledIcon.Location = New System.Drawing.Point(310, 49)
            Me.RPInstalledIcon.Name = "RPInstalledIcon"
            Me.RPInstalledIcon.PositionNumber = 0
            Me.RPInstalledIcon.Size = New System.Drawing.Size(21, 20)
            Me.RPInstalledIcon.TabIndex = 107
            Me.RPInstalledIcon.TabStop = False
            Me.RPInstalledIcon.Visible = False
            '
            'RPExpiredIcon
            '
            Me.RPExpiredIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.RPExpiredIcon.BackColor = System.Drawing.Color.Transparent
            Me.RPExpiredIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.RPExpiredIcon.Location = New System.Drawing.Point(310, 77)
            Me.RPExpiredIcon.Name = "RPExpiredIcon"
            Me.RPExpiredIcon.PositionNumber = 0
            Me.RPExpiredIcon.Size = New System.Drawing.Size(21, 20)
            Me.RPExpiredIcon.TabIndex = 104
            Me.RPExpiredIcon.TabStop = False
            Me.RPExpiredIcon.Visible = False
            '
            'RPExpireDateTextEdit
            '
            Me.RPExpireDateTextEdit.CausesValidation = False
            Me.RPExpireDateTextEdit.EditValue = "2012/02/10"
            Me.RPExpireDateTextEdit.Enabled = False
            Me.RPExpireDateTextEdit.Location = New System.Drawing.Point(206, 74)
            Me.RPExpireDateTextEdit.Name = "RPExpireDateTextEdit"
            Me.RPExpireDateTextEdit.Properties.AllowFocused = False
            Me.RPExpireDateTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.RPExpireDateTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.RPExpireDateTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.RPExpireDateTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.RPExpireDateTextEdit.Properties.Appearance.Options.UseFont = True
            Me.RPExpireDateTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.RPExpireDateTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.RPExpireDateTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.RPExpireDateTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.RPExpireDateTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.RPExpireDateTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.RPExpireDateTextEdit.Properties.ReadOnly = True
            Me.RPExpireDateTextEdit.ShowToolTips = False
            Me.RPExpireDateTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.RPExpireDateTextEdit.TabIndex = 103
            '
            'RPInstallDateTextEdit
            '
            Me.RPInstallDateTextEdit.CausesValidation = False
            Me.RPInstallDateTextEdit.EditValue = "2011/10/23"
            Me.RPInstallDateTextEdit.Enabled = False
            Me.RPInstallDateTextEdit.Location = New System.Drawing.Point(206, 47)
            Me.RPInstallDateTextEdit.Name = "RPInstallDateTextEdit"
            Me.RPInstallDateTextEdit.Properties.AllowFocused = False
            Me.RPInstallDateTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.RPInstallDateTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.RPInstallDateTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.RPInstallDateTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.RPInstallDateTextEdit.Properties.Appearance.Options.UseFont = True
            Me.RPInstallDateTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.RPInstallDateTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.RPInstallDateTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.RPInstallDateTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.RPInstallDateTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.RPInstallDateTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.RPInstallDateTextEdit.Properties.ReadOnly = True
            Me.RPInstallDateTextEdit.ShowToolTips = False
            Me.RPInstallDateTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.RPInstallDateTextEdit.TabIndex = 102
            '
            'RPExpireDateLabel
            '
            Me.RPExpireDateLabel.BackColor = System.Drawing.Color.Transparent
            Me.RPExpireDateLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.RPExpireDateLabel.ForeColor = System.Drawing.Color.Black
            Me.RPExpireDateLabel.Location = New System.Drawing.Point(0, 74)
            Me.RPExpireDateLabel.Name = "RPExpireDateLabel"
            Me.RPExpireDateLabel.Size = New System.Drawing.Size(205, 20)
            Me.RPExpireDateLabel.TabIndex = 95
            Me.RPExpireDateLabel.Text = "Expiration:"
            Me.RPExpireDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.RPExpireDateLabel.Title = False
            '
            'RPInstallDateLabel
            '
            Me.RPInstallDateLabel.BackColor = System.Drawing.Color.Transparent
            Me.RPInstallDateLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.RPInstallDateLabel.ForeColor = System.Drawing.Color.Black
            Me.RPInstallDateLabel.Location = New System.Drawing.Point(0, 46)
            Me.RPInstallDateLabel.Name = "RPInstallDateLabel"
            Me.RPInstallDateLabel.Size = New System.Drawing.Size(205, 20)
            Me.RPInstallDateLabel.TabIndex = 94
            Me.RPInstallDateLabel.Text = "Installation:"
            Me.RPInstallDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.RPInstallDateLabel.Title = False
            '
            'RPTitleLabel
            '
            Me.RPTitleLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.RPTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
            Me.RPTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
            Me.RPTitleLabel.ForeColor = System.Drawing.Color.Black
            Me.RPTitleLabel.Location = New System.Drawing.Point(5, 12)
            Me.RPTitleLabel.Name = "RPTitleLabel"
            Me.RPTitleLabel.Size = New System.Drawing.Size(743, 19)
            Me.RPTitleLabel.TabIndex = 25
            Me.RPTitleLabel.Text = "Reagents Pack"
            Me.RPTitleLabel.Title = True
            '
            'ElectrodesGroupBox
            '
            Me.ElectrodesGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ElectrodesGroupBox.BackColor = System.Drawing.Color.Transparent
            Me.ElectrodesGroupBox.Controls.Add(Me.ELLiWarningLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELClWarningLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELKWarningLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELNaWarningLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELKTestTextEdit)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELLiTestTextEdit)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELRefTestTextEdit)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELLithiumLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELClTestTextEdit)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELNaTestTextEdit)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELLiInstallTextEdit)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELRefInstallTextEdit)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELClInstallTextEdit)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELKInstallTextEdit)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELNaInstallTextEdit)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELRefWarningLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELRefIcon)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELLiIcon)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELClIcon)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELKIcon)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELNaIcon)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELTestCompletedLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELInstallDateLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELSodiumLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELPotassiumLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELChlorineLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELReferenceLabel)
            Me.ElectrodesGroupBox.Controls.Add(Me.ELTitleLabel)
            Me.ElectrodesGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ElectrodesGroupBox.ForeColor = System.Drawing.Color.Black
            Me.ElectrodesGroupBox.Location = New System.Drawing.Point(5, 192)
            Me.ElectrodesGroupBox.Name = "ElectrodesGroupBox"
            Me.ElectrodesGroupBox.Size = New System.Drawing.Size(754, 204)
            Me.ElectrodesGroupBox.TabIndex = 50
            Me.ElectrodesGroupBox.TabStop = False
            '
            'ELLiWarningLabel
            '
            Me.ELLiWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELLiWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELLiWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.ELLiWarningLabel.Location = New System.Drawing.Point(331, 175)
            Me.ELLiWarningLabel.Name = "ELLiWarningLabel"
            Me.ELLiWarningLabel.Size = New System.Drawing.Size(402, 20)
            Me.ELLiWarningLabel.TabIndex = 121
            Me.ELLiWarningLabel.Text = "----"
            Me.ELLiWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.ELLiWarningLabel.Title = False
            '
            'ELClWarningLabel
            '
            Me.ELClWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELClWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELClWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.ELClWarningLabel.Location = New System.Drawing.Point(332, 149)
            Me.ELClWarningLabel.Name = "ELClWarningLabel"
            Me.ELClWarningLabel.Size = New System.Drawing.Size(405, 20)
            Me.ELClWarningLabel.TabIndex = 120
            Me.ELClWarningLabel.Text = "----"
            Me.ELClWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.ELClWarningLabel.Title = False
            '
            'ELKWarningLabel
            '
            Me.ELKWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELKWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELKWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.ELKWarningLabel.Location = New System.Drawing.Point(332, 119)
            Me.ELKWarningLabel.Name = "ELKWarningLabel"
            Me.ELKWarningLabel.Size = New System.Drawing.Size(405, 20)
            Me.ELKWarningLabel.TabIndex = 119
            Me.ELKWarningLabel.Text = "----"
            Me.ELKWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.ELKWarningLabel.Title = False
            '
            'ELNaWarningLabel
            '
            Me.ELNaWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELNaWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELNaWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.ELNaWarningLabel.Location = New System.Drawing.Point(332, 91)
            Me.ELNaWarningLabel.Name = "ELNaWarningLabel"
            Me.ELNaWarningLabel.Size = New System.Drawing.Size(405, 20)
            Me.ELNaWarningLabel.TabIndex = 118
            Me.ELNaWarningLabel.Text = "----"
            Me.ELNaWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.ELNaWarningLabel.Title = False
            '
            'ELKTestTextEdit
            '
            Me.ELKTestTextEdit.CausesValidation = False
            Me.ELKTestTextEdit.EditValue = "9587"
            Me.ELKTestTextEdit.Enabled = False
            Me.ELKTestTextEdit.Location = New System.Drawing.Point(206, 119)
            Me.ELKTestTextEdit.Name = "ELKTestTextEdit"
            Me.ELKTestTextEdit.Properties.AllowFocused = False
            Me.ELKTestTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.ELKTestTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ELKTestTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.ELKTestTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.ELKTestTextEdit.Properties.Appearance.Options.UseFont = True
            Me.ELKTestTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.ELKTestTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.ELKTestTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Me.ELKTestTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.ELKTestTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.ELKTestTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.ELKTestTextEdit.Properties.ReadOnly = True
            Me.ELKTestTextEdit.ShowToolTips = False
            Me.ELKTestTextEdit.Size = New System.Drawing.Size(87, 22)
            Me.ELKTestTextEdit.TabIndex = 117
            '
            'ELLiTestTextEdit
            '
            Me.ELLiTestTextEdit.CausesValidation = False
            Me.ELLiTestTextEdit.EditValue = ""
            Me.ELLiTestTextEdit.Enabled = False
            Me.ELLiTestTextEdit.Location = New System.Drawing.Point(206, 175)
            Me.ELLiTestTextEdit.Name = "ELLiTestTextEdit"
            Me.ELLiTestTextEdit.Properties.AllowFocused = False
            Me.ELLiTestTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.ELLiTestTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ELLiTestTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.ELLiTestTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.ELLiTestTextEdit.Properties.Appearance.Options.UseFont = True
            Me.ELLiTestTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.ELLiTestTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.ELLiTestTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Me.ELLiTestTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.ELLiTestTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.ELLiTestTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.ELLiTestTextEdit.Properties.ReadOnly = True
            Me.ELLiTestTextEdit.ShowToolTips = False
            Me.ELLiTestTextEdit.Size = New System.Drawing.Size(87, 22)
            Me.ELLiTestTextEdit.TabIndex = 116
            '
            'ELRefTestTextEdit
            '
            Me.ELRefTestTextEdit.CausesValidation = False
            Me.ELRefTestTextEdit.EditValue = "4568"
            Me.ELRefTestTextEdit.Enabled = False
            Me.ELRefTestTextEdit.Location = New System.Drawing.Point(206, 63)
            Me.ELRefTestTextEdit.Name = "ELRefTestTextEdit"
            Me.ELRefTestTextEdit.Properties.AllowFocused = False
            Me.ELRefTestTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.ELRefTestTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ELRefTestTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.ELRefTestTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.ELRefTestTextEdit.Properties.Appearance.Options.UseFont = True
            Me.ELRefTestTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.ELRefTestTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.ELRefTestTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Me.ELRefTestTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.ELRefTestTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.ELRefTestTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.ELRefTestTextEdit.Properties.ReadOnly = True
            Me.ELRefTestTextEdit.ShowToolTips = False
            Me.ELRefTestTextEdit.Size = New System.Drawing.Size(87, 22)
            Me.ELRefTestTextEdit.TabIndex = 115
            '
            'ELLithiumLabel
            '
            Me.ELLithiumLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELLithiumLabel.Enabled = False
            Me.ELLithiumLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELLithiumLabel.ForeColor = System.Drawing.Color.Black
            Me.ELLithiumLabel.Location = New System.Drawing.Point(11, 177)
            Me.ELLithiumLabel.Name = "ELLithiumLabel"
            Me.ELLithiumLabel.Size = New System.Drawing.Size(98, 20)
            Me.ELLithiumLabel.TabIndex = 44
            Me.ELLithiumLabel.Text = "Li+"
            Me.ELLithiumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ELLithiumLabel.Title = False
            '
            'ELClTestTextEdit
            '
            Me.ELClTestTextEdit.CausesValidation = False
            Me.ELClTestTextEdit.EditValue = "4568"
            Me.ELClTestTextEdit.Enabled = False
            Me.ELClTestTextEdit.Location = New System.Drawing.Point(206, 147)
            Me.ELClTestTextEdit.Name = "ELClTestTextEdit"
            Me.ELClTestTextEdit.Properties.AllowFocused = False
            Me.ELClTestTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.ELClTestTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ELClTestTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.ELClTestTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.ELClTestTextEdit.Properties.Appearance.Options.UseFont = True
            Me.ELClTestTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.ELClTestTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.ELClTestTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Me.ELClTestTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.ELClTestTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.ELClTestTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.ELClTestTextEdit.Properties.ReadOnly = True
            Me.ELClTestTextEdit.ShowToolTips = False
            Me.ELClTestTextEdit.Size = New System.Drawing.Size(87, 22)
            Me.ELClTestTextEdit.TabIndex = 114
            '
            'ELNaTestTextEdit
            '
            Me.ELNaTestTextEdit.CausesValidation = False
            Me.ELNaTestTextEdit.EditValue = "4568"
            Me.ELNaTestTextEdit.Enabled = False
            Me.ELNaTestTextEdit.Location = New System.Drawing.Point(206, 91)
            Me.ELNaTestTextEdit.Name = "ELNaTestTextEdit"
            Me.ELNaTestTextEdit.Properties.AllowFocused = False
            Me.ELNaTestTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.ELNaTestTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ELNaTestTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.ELNaTestTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.ELNaTestTextEdit.Properties.Appearance.Options.UseFont = True
            Me.ELNaTestTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.ELNaTestTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.ELNaTestTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Me.ELNaTestTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.ELNaTestTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.ELNaTestTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.ELNaTestTextEdit.Properties.ReadOnly = True
            Me.ELNaTestTextEdit.ShowToolTips = False
            Me.ELNaTestTextEdit.Size = New System.Drawing.Size(87, 22)
            Me.ELNaTestTextEdit.TabIndex = 113
            '
            'ELLiInstallTextEdit
            '
            Me.ELLiInstallTextEdit.CausesValidation = False
            Me.ELLiInstallTextEdit.EditValue = ""
            Me.ELLiInstallTextEdit.Enabled = False
            Me.ELLiInstallTextEdit.Location = New System.Drawing.Point(115, 175)
            Me.ELLiInstallTextEdit.Name = "ELLiInstallTextEdit"
            Me.ELLiInstallTextEdit.Properties.AllowFocused = False
            Me.ELLiInstallTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.ELLiInstallTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ELLiInstallTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.ELLiInstallTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.ELLiInstallTextEdit.Properties.Appearance.Options.UseFont = True
            Me.ELLiInstallTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.ELLiInstallTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.ELLiInstallTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.ELLiInstallTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.ELLiInstallTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.ELLiInstallTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.ELLiInstallTextEdit.Properties.ReadOnly = True
            Me.ELLiInstallTextEdit.ShowToolTips = False
            Me.ELLiInstallTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.ELLiInstallTextEdit.TabIndex = 104
            '
            'ELRefInstallTextEdit
            '
            Me.ELRefInstallTextEdit.CausesValidation = False
            Me.ELRefInstallTextEdit.EditValue = "2012/02/05"
            Me.ELRefInstallTextEdit.Enabled = False
            Me.ELRefInstallTextEdit.Location = New System.Drawing.Point(115, 63)
            Me.ELRefInstallTextEdit.Name = "ELRefInstallTextEdit"
            Me.ELRefInstallTextEdit.Properties.AllowFocused = False
            Me.ELRefInstallTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.ELRefInstallTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ELRefInstallTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.ELRefInstallTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.ELRefInstallTextEdit.Properties.Appearance.Options.UseFont = True
            Me.ELRefInstallTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.ELRefInstallTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.ELRefInstallTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.ELRefInstallTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.ELRefInstallTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.ELRefInstallTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.ELRefInstallTextEdit.Properties.ReadOnly = True
            Me.ELRefInstallTextEdit.ShowToolTips = False
            Me.ELRefInstallTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.ELRefInstallTextEdit.TabIndex = 103
            '
            'ELClInstallTextEdit
            '
            Me.ELClInstallTextEdit.CausesValidation = False
            Me.ELClInstallTextEdit.EditValue = "2012/02/05"
            Me.ELClInstallTextEdit.Enabled = False
            Me.ELClInstallTextEdit.Location = New System.Drawing.Point(115, 147)
            Me.ELClInstallTextEdit.Name = "ELClInstallTextEdit"
            Me.ELClInstallTextEdit.Properties.AllowFocused = False
            Me.ELClInstallTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.ELClInstallTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ELClInstallTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.ELClInstallTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.ELClInstallTextEdit.Properties.Appearance.Options.UseFont = True
            Me.ELClInstallTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.ELClInstallTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.ELClInstallTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.ELClInstallTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.ELClInstallTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.ELClInstallTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.ELClInstallTextEdit.Properties.ReadOnly = True
            Me.ELClInstallTextEdit.ShowToolTips = False
            Me.ELClInstallTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.ELClInstallTextEdit.TabIndex = 102
            '
            'ELKInstallTextEdit
            '
            Me.ELKInstallTextEdit.CausesValidation = False
            Me.ELKInstallTextEdit.EditValue = "2012/02/05"
            Me.ELKInstallTextEdit.Enabled = False
            Me.ELKInstallTextEdit.Location = New System.Drawing.Point(115, 119)
            Me.ELKInstallTextEdit.Name = "ELKInstallTextEdit"
            Me.ELKInstallTextEdit.Properties.AllowFocused = False
            Me.ELKInstallTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.ELKInstallTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ELKInstallTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.ELKInstallTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.ELKInstallTextEdit.Properties.Appearance.Options.UseFont = True
            Me.ELKInstallTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.ELKInstallTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.ELKInstallTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.ELKInstallTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.ELKInstallTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.ELKInstallTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.ELKInstallTextEdit.Properties.ReadOnly = True
            Me.ELKInstallTextEdit.ShowToolTips = False
            Me.ELKInstallTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.ELKInstallTextEdit.TabIndex = 101
            '
            'ELNaInstallTextEdit
            '
            Me.ELNaInstallTextEdit.CausesValidation = False
            Me.ELNaInstallTextEdit.EditValue = "2012/02/05"
            Me.ELNaInstallTextEdit.Enabled = False
            Me.ELNaInstallTextEdit.Location = New System.Drawing.Point(115, 91)
            Me.ELNaInstallTextEdit.Name = "ELNaInstallTextEdit"
            Me.ELNaInstallTextEdit.Properties.AllowFocused = False
            Me.ELNaInstallTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.ELNaInstallTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ELNaInstallTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.ELNaInstallTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.ELNaInstallTextEdit.Properties.Appearance.Options.UseFont = True
            Me.ELNaInstallTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.ELNaInstallTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.ELNaInstallTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.ELNaInstallTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.ELNaInstallTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.ELNaInstallTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.ELNaInstallTextEdit.Properties.ReadOnly = True
            Me.ELNaInstallTextEdit.ShowToolTips = False
            Me.ELNaInstallTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.ELNaInstallTextEdit.TabIndex = 100
            '
            'ELRefWarningLabel
            '
            Me.ELRefWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELRefWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELRefWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.ELRefWarningLabel.Location = New System.Drawing.Point(332, 63)
            Me.ELRefWarningLabel.Name = "ELRefWarningLabel"
            Me.ELRefWarningLabel.Size = New System.Drawing.Size(405, 20)
            Me.ELRefWarningLabel.TabIndex = 67
            Me.ELRefWarningLabel.Text = "----"
            Me.ELRefWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.ELRefWarningLabel.Title = False
            '
            'ELRefIcon
            '
            Me.ELRefIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ELRefIcon.BackColor = System.Drawing.Color.Transparent
            Me.ELRefIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.ELRefIcon.Location = New System.Drawing.Point(310, 63)
            Me.ELRefIcon.Name = "ELRefIcon"
            Me.ELRefIcon.PositionNumber = 0
            Me.ELRefIcon.Size = New System.Drawing.Size(21, 20)
            Me.ELRefIcon.TabIndex = 64
            Me.ELRefIcon.TabStop = False
            Me.ELRefIcon.Visible = False
            '
            'ELLiIcon
            '
            Me.ELLiIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ELLiIcon.BackColor = System.Drawing.Color.Transparent
            Me.ELLiIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.ELLiIcon.Location = New System.Drawing.Point(310, 175)
            Me.ELLiIcon.Name = "ELLiIcon"
            Me.ELLiIcon.PositionNumber = 0
            Me.ELLiIcon.Size = New System.Drawing.Size(21, 20)
            Me.ELLiIcon.TabIndex = 62
            Me.ELLiIcon.TabStop = False
            Me.ELLiIcon.Visible = False
            '
            'ELClIcon
            '
            Me.ELClIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ELClIcon.BackColor = System.Drawing.Color.Transparent
            Me.ELClIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.ELClIcon.Location = New System.Drawing.Point(310, 149)
            Me.ELClIcon.Name = "ELClIcon"
            Me.ELClIcon.PositionNumber = 0
            Me.ELClIcon.Size = New System.Drawing.Size(21, 20)
            Me.ELClIcon.TabIndex = 60
            Me.ELClIcon.TabStop = False
            Me.ELClIcon.Visible = False
            '
            'ELKIcon
            '
            Me.ELKIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ELKIcon.BackColor = System.Drawing.Color.Transparent
            Me.ELKIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.ELKIcon.Location = New System.Drawing.Point(310, 119)
            Me.ELKIcon.Name = "ELKIcon"
            Me.ELKIcon.PositionNumber = 0
            Me.ELKIcon.Size = New System.Drawing.Size(21, 20)
            Me.ELKIcon.TabIndex = 58
            Me.ELKIcon.TabStop = False
            Me.ELKIcon.Visible = False
            '
            'ELNaIcon
            '
            Me.ELNaIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ELNaIcon.BackColor = System.Drawing.Color.Transparent
            Me.ELNaIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.ELNaIcon.Location = New System.Drawing.Point(310, 91)
            Me.ELNaIcon.Name = "ELNaIcon"
            Me.ELNaIcon.PositionNumber = 0
            Me.ELNaIcon.Size = New System.Drawing.Size(21, 20)
            Me.ELNaIcon.TabIndex = 52
            Me.ELNaIcon.TabStop = False
            Me.ELNaIcon.UseWaitCursor = True
            Me.ELNaIcon.Visible = False
            '
            'ELTestCompletedLabel
            '
            Me.ELTestCompletedLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELTestCompletedLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELTestCompletedLabel.ForeColor = System.Drawing.Color.Black
            Me.ELTestCompletedLabel.Location = New System.Drawing.Point(206, 40)
            Me.ELTestCompletedLabel.Name = "ELTestCompletedLabel"
            Me.ELTestCompletedLabel.Size = New System.Drawing.Size(103, 20)
            Me.ELTestCompletedLabel.TabIndex = 51
            Me.ELTestCompletedLabel.Text = "Tests completed"
            Me.ELTestCompletedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.ELTestCompletedLabel.Title = False
            '
            'ELInstallDateLabel
            '
            Me.ELInstallDateLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELInstallDateLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELInstallDateLabel.ForeColor = System.Drawing.Color.Black
            Me.ELInstallDateLabel.Location = New System.Drawing.Point(100, 40)
            Me.ELInstallDateLabel.Name = "ELInstallDateLabel"
            Me.ELInstallDateLabel.Size = New System.Drawing.Size(103, 20)
            Me.ELInstallDateLabel.TabIndex = 50
            Me.ELInstallDateLabel.Text = "Installation Date"
            Me.ELInstallDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.ELInstallDateLabel.Title = False
            '
            'ELSodiumLabel
            '
            Me.ELSodiumLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELSodiumLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELSodiumLabel.ForeColor = System.Drawing.Color.Black
            Me.ELSodiumLabel.Location = New System.Drawing.Point(11, 93)
            Me.ELSodiumLabel.Name = "ELSodiumLabel"
            Me.ELSodiumLabel.Size = New System.Drawing.Size(98, 20)
            Me.ELSodiumLabel.TabIndex = 48
            Me.ELSodiumLabel.Text = "Na+"
            Me.ELSodiumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ELSodiumLabel.Title = False
            '
            'ELPotassiumLabel
            '
            Me.ELPotassiumLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELPotassiumLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELPotassiumLabel.ForeColor = System.Drawing.Color.Black
            Me.ELPotassiumLabel.Location = New System.Drawing.Point(11, 121)
            Me.ELPotassiumLabel.Name = "ELPotassiumLabel"
            Me.ELPotassiumLabel.Size = New System.Drawing.Size(98, 20)
            Me.ELPotassiumLabel.TabIndex = 47
            Me.ELPotassiumLabel.Text = "K+"
            Me.ELPotassiumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ELPotassiumLabel.Title = False
            '
            'ELChlorineLabel
            '
            Me.ELChlorineLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELChlorineLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELChlorineLabel.ForeColor = System.Drawing.Color.Black
            Me.ELChlorineLabel.Location = New System.Drawing.Point(11, 149)
            Me.ELChlorineLabel.Name = "ELChlorineLabel"
            Me.ELChlorineLabel.Size = New System.Drawing.Size(98, 20)
            Me.ELChlorineLabel.TabIndex = 46
            Me.ELChlorineLabel.Text = "Cl-"
            Me.ELChlorineLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ELChlorineLabel.Title = False
            '
            'ELReferenceLabel
            '
            Me.ELReferenceLabel.BackColor = System.Drawing.Color.Transparent
            Me.ELReferenceLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.ELReferenceLabel.ForeColor = System.Drawing.Color.Black
            Me.ELReferenceLabel.Location = New System.Drawing.Point(11, 63)
            Me.ELReferenceLabel.Name = "ELReferenceLabel"
            Me.ELReferenceLabel.Size = New System.Drawing.Size(98, 20)
            Me.ELReferenceLabel.TabIndex = 45
            Me.ELReferenceLabel.Text = "Ref."
            Me.ELReferenceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ELReferenceLabel.Title = False
            '
            'ELTitleLabel
            '
            Me.ELTitleLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.ELTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
            Me.ELTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
            Me.ELTitleLabel.ForeColor = System.Drawing.Color.Black
            Me.ELTitleLabel.Location = New System.Drawing.Point(5, 12)
            Me.ELTitleLabel.Name = "ELTitleLabel"
            Me.ELTitleLabel.Size = New System.Drawing.Size(743, 19)
            Me.ELTitleLabel.TabIndex = 25
            Me.ELTitleLabel.Text = "Electrodes"
            Me.ELTitleLabel.Title = True
            '
            'CalibrationsGroupBox
            '
            Me.CalibrationsGroupBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.CalibrationsGroupBox.BackColor = System.Drawing.Color.Transparent
            Me.CalibrationsGroupBox.Controls.Add(Me.CALBubbleWarningLabel)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALBubbleResultTextEdit)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALBubbleDateTextEdit)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALBubbleIcon)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALBubbleLabel)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALCleanWarningLabel)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALPumpsWarningLabel)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALElectrodesWarningLabel)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALElectrodesIcon1)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALElectrodesResult2TextEdit)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALPumpsResultTextEdit)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALElectrodesResult1TextEdit)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALCleanDateTextEdit)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALPumpsDateTextEdit)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALElectrodesDateTextEdit)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALCleanIcon)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALCleanLabel)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALPumpsIcon)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALResultLabel)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALLastDateLabel)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALPumpsLabel)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALElectrodesLabel)
            Me.CalibrationsGroupBox.Controls.Add(Me.CALTitleLabel)
            Me.CalibrationsGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CalibrationsGroupBox.ForeColor = System.Drawing.Color.Black
            Me.CalibrationsGroupBox.Location = New System.Drawing.Point(5, 393)
            Me.CalibrationsGroupBox.Name = "CalibrationsGroupBox"
            Me.CalibrationsGroupBox.Size = New System.Drawing.Size(754, 222)
            Me.CalibrationsGroupBox.TabIndex = 51
            Me.CalibrationsGroupBox.TabStop = False
            '
            'CALBubbleWarningLabel
            '
            Me.CALBubbleWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.CALBubbleWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.CALBubbleWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.CALBubbleWarningLabel.Location = New System.Drawing.Point(556, 164)
            Me.CALBubbleWarningLabel.Name = "CALBubbleWarningLabel"
            Me.CALBubbleWarningLabel.Size = New System.Drawing.Size(177, 43)
            Me.CALBubbleWarningLabel.TabIndex = 123
            Me.CALBubbleWarningLabel.Text = "----"
            Me.CALBubbleWarningLabel.Title = False
            '
            'CALBubbleResultTextEdit
            '
            Me.CALBubbleResultTextEdit.CausesValidation = False
            Me.CALBubbleResultTextEdit.EditValue = "<PMC A 2350 B 2358 W 2436>"
            Me.CALBubbleResultTextEdit.Enabled = False
            Me.CALBubbleResultTextEdit.Location = New System.Drawing.Point(206, 162)
            Me.CALBubbleResultTextEdit.Name = "CALBubbleResultTextEdit"
            Me.CALBubbleResultTextEdit.Properties.AllowFocused = False
            Me.CALBubbleResultTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.CALBubbleResultTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CALBubbleResultTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.CALBubbleResultTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.CALBubbleResultTextEdit.Properties.Appearance.Options.UseFont = True
            Me.CALBubbleResultTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.CALBubbleResultTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.CALBubbleResultTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.CALBubbleResultTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.CALBubbleResultTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.CALBubbleResultTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.CALBubbleResultTextEdit.Properties.ReadOnly = True
            Me.CALBubbleResultTextEdit.ShowToolTips = False
            Me.CALBubbleResultTextEdit.Size = New System.Drawing.Size(311, 22)
            Me.CALBubbleResultTextEdit.TabIndex = 122
            '
            'CALBubbleDateTextEdit
            '
            Me.CALBubbleDateTextEdit.CausesValidation = False
            Me.CALBubbleDateTextEdit.EditValue = "2012/02/05"
            Me.CALBubbleDateTextEdit.Enabled = False
            Me.CALBubbleDateTextEdit.Location = New System.Drawing.Point(115, 162)
            Me.CALBubbleDateTextEdit.Name = "CALBubbleDateTextEdit"
            Me.CALBubbleDateTextEdit.Properties.AllowFocused = False
            Me.CALBubbleDateTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.CALBubbleDateTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CALBubbleDateTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.CALBubbleDateTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.CALBubbleDateTextEdit.Properties.Appearance.Options.UseFont = True
            Me.CALBubbleDateTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.CALBubbleDateTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.CALBubbleDateTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.CALBubbleDateTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.CALBubbleDateTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.CALBubbleDateTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.CALBubbleDateTextEdit.Properties.ReadOnly = True
            Me.CALBubbleDateTextEdit.ShowToolTips = False
            Me.CALBubbleDateTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.CALBubbleDateTextEdit.TabIndex = 121
            '
            'CALBubbleIcon
            '
            Me.CALBubbleIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.CALBubbleIcon.BackColor = System.Drawing.Color.Transparent
            Me.CALBubbleIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.CALBubbleIcon.Location = New System.Drawing.Point(533, 164)
            Me.CALBubbleIcon.Name = "CALBubbleIcon"
            Me.CALBubbleIcon.PositionNumber = 0
            Me.CALBubbleIcon.Size = New System.Drawing.Size(21, 20)
            Me.CALBubbleIcon.TabIndex = 120
            Me.CALBubbleIcon.TabStop = False
            Me.CALBubbleIcon.Visible = False
            '
            'CALBubbleLabel
            '
            Me.CALBubbleLabel.BackColor = System.Drawing.Color.Transparent
            Me.CALBubbleLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.CALBubbleLabel.ForeColor = System.Drawing.Color.Black
            Me.CALBubbleLabel.Location = New System.Drawing.Point(8, 162)
            Me.CALBubbleLabel.Name = "CALBubbleLabel"
            Me.CALBubbleLabel.Size = New System.Drawing.Size(101, 20)
            Me.CALBubbleLabel.TabIndex = 119
            Me.CALBubbleLabel.Text = "Bubbles:"
            Me.CALBubbleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.CALBubbleLabel.Title = False
            '
            'CALCleanWarningLabel
            '
            Me.CALCleanWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.CALCleanWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.CALCleanWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.CALCleanWarningLabel.Location = New System.Drawing.Point(242, 193)
            Me.CALCleanWarningLabel.Name = "CALCleanWarningLabel"
            Me.CALCleanWarningLabel.Size = New System.Drawing.Size(301, 21)
            Me.CALCleanWarningLabel.TabIndex = 118
            Me.CALCleanWarningLabel.Text = "----"
            Me.CALCleanWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            Me.CALCleanWarningLabel.Title = False
            '
            'CALPumpsWarningLabel
            '
            Me.CALPumpsWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.CALPumpsWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.CALPumpsWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.CALPumpsWarningLabel.Location = New System.Drawing.Point(556, 117)
            Me.CALPumpsWarningLabel.Name = "CALPumpsWarningLabel"
            Me.CALPumpsWarningLabel.Size = New System.Drawing.Size(177, 43)
            Me.CALPumpsWarningLabel.TabIndex = 117
            Me.CALPumpsWarningLabel.Text = "----"
            Me.CALPumpsWarningLabel.Title = False
            '
            'CALElectrodesWarningLabel
            '
            Me.CALElectrodesWarningLabel.BackColor = System.Drawing.Color.Transparent
            Me.CALElectrodesWarningLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.CALElectrodesWarningLabel.ForeColor = System.Drawing.Color.Black
            Me.CALElectrodesWarningLabel.Location = New System.Drawing.Point(555, 63)
            Me.CALElectrodesWarningLabel.Name = "CALElectrodesWarningLabel"
            Me.CALElectrodesWarningLabel.Size = New System.Drawing.Size(177, 43)
            Me.CALElectrodesWarningLabel.TabIndex = 114
            Me.CALElectrodesWarningLabel.Text = "----"
            Me.CALElectrodesWarningLabel.Title = False
            '
            'CALElectrodesIcon1
            '
            Me.CALElectrodesIcon1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.CALElectrodesIcon1.BackColor = System.Drawing.Color.Transparent
            Me.CALElectrodesIcon1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.CALElectrodesIcon1.Location = New System.Drawing.Point(533, 61)
            Me.CALElectrodesIcon1.Name = "CALElectrodesIcon1"
            Me.CALElectrodesIcon1.PositionNumber = 0
            Me.CALElectrodesIcon1.Size = New System.Drawing.Size(21, 20)
            Me.CALElectrodesIcon1.TabIndex = 113
            Me.CALElectrodesIcon1.TabStop = False
            Me.CALElectrodesIcon1.Visible = False
            '
            'CALElectrodesResult2TextEdit
            '
            Me.CALElectrodesResult2TextEdit.CausesValidation = False
            Me.CALElectrodesResult2TextEdit.EditValue = "<CAL Li 128.9 Na 36.98 K 99.35 Cl 16.87>"
            Me.CALElectrodesResult2TextEdit.Enabled = False
            Me.CALElectrodesResult2TextEdit.Location = New System.Drawing.Point(206, 80)
            Me.CALElectrodesResult2TextEdit.Name = "CALElectrodesResult2TextEdit"
            Me.CALElectrodesResult2TextEdit.Properties.AllowFocused = False
            Me.CALElectrodesResult2TextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.CALElectrodesResult2TextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CALElectrodesResult2TextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.CALElectrodesResult2TextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.CALElectrodesResult2TextEdit.Properties.Appearance.Options.UseFont = True
            Me.CALElectrodesResult2TextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.CALElectrodesResult2TextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.CALElectrodesResult2TextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.CALElectrodesResult2TextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.CALElectrodesResult2TextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.CALElectrodesResult2TextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.CALElectrodesResult2TextEdit.Properties.ReadOnly = True
            Me.CALElectrodesResult2TextEdit.ShowToolTips = False
            Me.CALElectrodesResult2TextEdit.Size = New System.Drawing.Size(311, 22)
            Me.CALElectrodesResult2TextEdit.TabIndex = 112
            '
            'CALPumpsResultTextEdit
            '
            Me.CALPumpsResultTextEdit.CausesValidation = False
            Me.CALPumpsResultTextEdit.EditValue = "<PMC A 2350 B 2358 W 2436>"
            Me.CALPumpsResultTextEdit.Enabled = False
            Me.CALPumpsResultTextEdit.Location = New System.Drawing.Point(206, 113)
            Me.CALPumpsResultTextEdit.Name = "CALPumpsResultTextEdit"
            Me.CALPumpsResultTextEdit.Properties.AllowFocused = False
            Me.CALPumpsResultTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.CALPumpsResultTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CALPumpsResultTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.CALPumpsResultTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.CALPumpsResultTextEdit.Properties.Appearance.Options.UseFont = True
            Me.CALPumpsResultTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.CALPumpsResultTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.CALPumpsResultTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.CALPumpsResultTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.CALPumpsResultTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.CALPumpsResultTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.CALPumpsResultTextEdit.Properties.ReadOnly = True
            Me.CALPumpsResultTextEdit.ShowToolTips = False
            Me.CALPumpsResultTextEdit.Size = New System.Drawing.Size(311, 22)
            Me.CALPumpsResultTextEdit.TabIndex = 111
            '
            'CALElectrodesResult1TextEdit
            '
            Me.CALElectrodesResult1TextEdit.CausesValidation = False
            Me.CALElectrodesResult1TextEdit.EditValue = "<CAL Li 128.9 Na 36.98 K 99.35 Cl 16.87>"
            Me.CALElectrodesResult1TextEdit.Enabled = False
            Me.CALElectrodesResult1TextEdit.Location = New System.Drawing.Point(206, 59)
            Me.CALElectrodesResult1TextEdit.Name = "CALElectrodesResult1TextEdit"
            Me.CALElectrodesResult1TextEdit.Properties.AllowFocused = False
            Me.CALElectrodesResult1TextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.CALElectrodesResult1TextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CALElectrodesResult1TextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.CALElectrodesResult1TextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.CALElectrodesResult1TextEdit.Properties.Appearance.Options.UseFont = True
            Me.CALElectrodesResult1TextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.CALElectrodesResult1TextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.CALElectrodesResult1TextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.CALElectrodesResult1TextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.CALElectrodesResult1TextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.CALElectrodesResult1TextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.CALElectrodesResult1TextEdit.Properties.ReadOnly = True
            Me.CALElectrodesResult1TextEdit.ShowToolTips = False
            Me.CALElectrodesResult1TextEdit.Size = New System.Drawing.Size(311, 22)
            Me.CALElectrodesResult1TextEdit.TabIndex = 110
            '
            'CALCleanDateTextEdit
            '
            Me.CALCleanDateTextEdit.CausesValidation = False
            Me.CALCleanDateTextEdit.EditValue = "2012/02/05"
            Me.CALCleanDateTextEdit.Enabled = False
            Me.CALCleanDateTextEdit.Location = New System.Drawing.Point(115, 193)
            Me.CALCleanDateTextEdit.Name = "CALCleanDateTextEdit"
            Me.CALCleanDateTextEdit.Properties.AllowFocused = False
            Me.CALCleanDateTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.CALCleanDateTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CALCleanDateTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.CALCleanDateTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.CALCleanDateTextEdit.Properties.Appearance.Options.UseFont = True
            Me.CALCleanDateTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.CALCleanDateTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.CALCleanDateTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.CALCleanDateTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.CALCleanDateTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.CALCleanDateTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.CALCleanDateTextEdit.Properties.ReadOnly = True
            Me.CALCleanDateTextEdit.ShowToolTips = False
            Me.CALCleanDateTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.CALCleanDateTextEdit.TabIndex = 103
            '
            'CALPumpsDateTextEdit
            '
            Me.CALPumpsDateTextEdit.CausesValidation = False
            Me.CALPumpsDateTextEdit.EditValue = "2012/02/05"
            Me.CALPumpsDateTextEdit.Enabled = False
            Me.CALPumpsDateTextEdit.Location = New System.Drawing.Point(115, 113)
            Me.CALPumpsDateTextEdit.Name = "CALPumpsDateTextEdit"
            Me.CALPumpsDateTextEdit.Properties.AllowFocused = False
            Me.CALPumpsDateTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.CALPumpsDateTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CALPumpsDateTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.CALPumpsDateTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.CALPumpsDateTextEdit.Properties.Appearance.Options.UseFont = True
            Me.CALPumpsDateTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.CALPumpsDateTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.CALPumpsDateTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.CALPumpsDateTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.CALPumpsDateTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.CALPumpsDateTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.CALPumpsDateTextEdit.Properties.ReadOnly = True
            Me.CALPumpsDateTextEdit.ShowToolTips = False
            Me.CALPumpsDateTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.CALPumpsDateTextEdit.TabIndex = 102
            '
            'CALElectrodesDateTextEdit
            '
            Me.CALElectrodesDateTextEdit.CausesValidation = False
            Me.CALElectrodesDateTextEdit.EditValue = "2012/02/05"
            Me.CALElectrodesDateTextEdit.Enabled = False
            Me.CALElectrodesDateTextEdit.Location = New System.Drawing.Point(115, 59)
            Me.CALElectrodesDateTextEdit.Name = "CALElectrodesDateTextEdit"
            Me.CALElectrodesDateTextEdit.Properties.AllowFocused = False
            Me.CALElectrodesDateTextEdit.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke
            Me.CALElectrodesDateTextEdit.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CALElectrodesDateTextEdit.Properties.Appearance.ForeColor = System.Drawing.Color.Black
            Me.CALElectrodesDateTextEdit.Properties.Appearance.Options.UseBackColor = True
            Me.CALElectrodesDateTextEdit.Properties.Appearance.Options.UseFont = True
            Me.CALElectrodesDateTextEdit.Properties.Appearance.Options.UseForeColor = True
            Me.CALElectrodesDateTextEdit.Properties.Appearance.Options.UseTextOptions = True
            Me.CALElectrodesDateTextEdit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            Me.CALElectrodesDateTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat
            Me.CALElectrodesDateTextEdit.Properties.DisplayFormat.FormatString = "{0.0}"
            Me.CALElectrodesDateTextEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
            Me.CALElectrodesDateTextEdit.Properties.ReadOnly = True
            Me.CALElectrodesDateTextEdit.ShowToolTips = False
            Me.CALElectrodesDateTextEdit.Size = New System.Drawing.Size(85, 22)
            Me.CALElectrodesDateTextEdit.TabIndex = 101
            '
            'CALCleanIcon
            '
            Me.CALCleanIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.CALCleanIcon.BackColor = System.Drawing.Color.Transparent
            Me.CALCleanIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.CALCleanIcon.Location = New System.Drawing.Point(220, 195)
            Me.CALCleanIcon.Name = "CALCleanIcon"
            Me.CALCleanIcon.PositionNumber = 0
            Me.CALCleanIcon.Size = New System.Drawing.Size(21, 20)
            Me.CALCleanIcon.TabIndex = 80
            Me.CALCleanIcon.TabStop = False
            Me.CALCleanIcon.Visible = False
            '
            'CALCleanLabel
            '
            Me.CALCleanLabel.BackColor = System.Drawing.Color.Transparent
            Me.CALCleanLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.CALCleanLabel.ForeColor = System.Drawing.Color.Black
            Me.CALCleanLabel.Location = New System.Drawing.Point(8, 193)
            Me.CALCleanLabel.Name = "CALCleanLabel"
            Me.CALCleanLabel.Size = New System.Drawing.Size(101, 20)
            Me.CALCleanLabel.TabIndex = 77
            Me.CALCleanLabel.Text = "Clean:"
            Me.CALCleanLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.CALCleanLabel.Title = False
            '
            'CALPumpsIcon
            '
            Me.CALPumpsIcon.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.CALPumpsIcon.BackColor = System.Drawing.Color.Transparent
            Me.CALPumpsIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.CALPumpsIcon.Location = New System.Drawing.Point(533, 115)
            Me.CALPumpsIcon.Name = "CALPumpsIcon"
            Me.CALPumpsIcon.PositionNumber = 0
            Me.CALPumpsIcon.Size = New System.Drawing.Size(21, 20)
            Me.CALPumpsIcon.TabIndex = 75
            Me.CALPumpsIcon.TabStop = False
            Me.CALPumpsIcon.Visible = False
            '
            'CALResultLabel
            '
            Me.CALResultLabel.BackColor = System.Drawing.Color.Transparent
            Me.CALResultLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.CALResultLabel.ForeColor = System.Drawing.Color.Black
            Me.CALResultLabel.Location = New System.Drawing.Point(207, 38)
            Me.CALResultLabel.Name = "CALResultLabel"
            Me.CALResultLabel.Size = New System.Drawing.Size(314, 20)
            Me.CALResultLabel.TabIndex = 71
            Me.CALResultLabel.Text = "Results"
            Me.CALResultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.CALResultLabel.Title = False
            '
            'CALLastDateLabel
            '
            Me.CALLastDateLabel.BackColor = System.Drawing.Color.Transparent
            Me.CALLastDateLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.CALLastDateLabel.ForeColor = System.Drawing.Color.Black
            Me.CALLastDateLabel.Location = New System.Drawing.Point(103, 38)
            Me.CALLastDateLabel.Name = "CALLastDateLabel"
            Me.CALLastDateLabel.Size = New System.Drawing.Size(97, 20)
            Me.CALLastDateLabel.TabIndex = 70
            Me.CALLastDateLabel.Text = "Last Date"
            Me.CALLastDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            Me.CALLastDateLabel.Title = False
            '
            'CALPumpsLabel
            '
            Me.CALPumpsLabel.BackColor = System.Drawing.Color.Transparent
            Me.CALPumpsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.CALPumpsLabel.ForeColor = System.Drawing.Color.Black
            Me.CALPumpsLabel.Location = New System.Drawing.Point(8, 113)
            Me.CALPumpsLabel.Name = "CALPumpsLabel"
            Me.CALPumpsLabel.Size = New System.Drawing.Size(101, 20)
            Me.CALPumpsLabel.TabIndex = 68
            Me.CALPumpsLabel.Text = "Pumps:"
            Me.CALPumpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.CALPumpsLabel.Title = False
            '
            'CALElectrodesLabel
            '
            Me.CALElectrodesLabel.BackColor = System.Drawing.Color.Transparent
            Me.CALElectrodesLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.CALElectrodesLabel.ForeColor = System.Drawing.Color.Black
            Me.CALElectrodesLabel.Location = New System.Drawing.Point(8, 59)
            Me.CALElectrodesLabel.Name = "CALElectrodesLabel"
            Me.CALElectrodesLabel.Size = New System.Drawing.Size(101, 20)
            Me.CALElectrodesLabel.TabIndex = 67
            Me.CALElectrodesLabel.Text = "Electrodes:"
            Me.CALElectrodesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.CALElectrodesLabel.Title = False
            '
            'CALTitleLabel
            '
            Me.CALTitleLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.CALTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
            Me.CALTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
            Me.CALTitleLabel.ForeColor = System.Drawing.Color.Black
            Me.CALTitleLabel.Location = New System.Drawing.Point(5, 12)
            Me.CALTitleLabel.Name = "CALTitleLabel"
            Me.CALTitleLabel.Size = New System.Drawing.Size(743, 19)
            Me.CALTitleLabel.TabIndex = 25
            Me.CALTitleLabel.Text = "Last Calibrations and Cleanings"
            Me.CALTitleLabel.Title = True
            '
            'BSISEMonitorPanel
            '
            Me.Controls.Add(Me.ReagentsPackGroupBox)
            Me.Controls.Add(Me.ElectrodesGroupBox)
            Me.Controls.Add(Me.CalibrationsGroupBox)
            Me.MaximumSize = New System.Drawing.Size(978, 623)
            Me.MinimumSize = New System.Drawing.Size(762, 593)
            Me.Name = "BSISEMonitorPanel"
            Me.Size = New System.Drawing.Size(762, 618)
            Me.ReagentsPackGroupBox.ResumeLayout(False)
            Me.VolumeGB.ResumeLayout(False)
            CType(Me.RPInitialCalBTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.RPInitialCalATextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.RPRemainingCalBTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.RPCalBIcon, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.RPRemainingCalATextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.RPCalAIcon, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.RPInstalledIcon, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.RPExpiredIcon, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.RPExpireDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.RPInstallDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ElectrodesGroupBox.ResumeLayout(False)
            CType(Me.ELKTestTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELLiTestTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELRefTestTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELClTestTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELNaTestTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELLiInstallTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELRefInstallTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELClInstallTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELKInstallTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELNaInstallTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELRefIcon, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELLiIcon, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELClIcon, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELKIcon, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.ELNaIcon, System.ComponentModel.ISupportInitialize).EndInit()
            Me.CalibrationsGroupBox.ResumeLayout(False)
            CType(Me.CALBubbleResultTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALBubbleDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALBubbleIcon, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALElectrodesIcon1, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALElectrodesResult2TextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALPumpsResultTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALElectrodesResult1TextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALCleanDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALPumpsDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALElectrodesDateTextEdit.Properties, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALCleanIcon, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.CALPumpsIcon, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents CalibrationsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Friend WithEvents CALPumpsResultTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents CALElectrodesResult1TextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents CALCleanDateTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents CALPumpsDateTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents CALElectrodesDateTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents CALCleanIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents CALCleanLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALPumpsIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents CALResultLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALLastDateLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALPumpsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALElectrodesLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ElectrodesGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Friend WithEvents ELKTestTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents ELLiTestTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents ELRefTestTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents ELLithiumLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELClTestTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents ELNaTestTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents ELLiInstallTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents ELRefInstallTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents ELClInstallTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents ELKInstallTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents ELNaInstallTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents ELRefWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELRefIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents ELLiIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents ELClIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents ELKIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents ELNaIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents ELTestCompletedLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELInstallDateLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELSodiumLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELPotassiumLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELChlorineLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELReferenceLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ReagentsPackGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Friend WithEvents RPExpiredIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents RPExpireDateTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents RPInstallDateTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents RPExpireDateLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents RPInstallDateLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents RPTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALElectrodesResult2TextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents CALElectrodesWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALElectrodesIcon1 As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents ELLiWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELClWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELKWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents ELNaWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALCleanWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALPumpsWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents RPInstalledIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents RPExpiredWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents RPNotInstalledWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents VolumeGB As Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Friend WithEvents RPCalBWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents RPCalAWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents RPInitialCalBTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents RPInitialCalATextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents RPRemainingCalBTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents RPCalBIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents RPRemainingCalATextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents RPCalAIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents RPInitialVolumeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents RPRemainingLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents RPCalibratorBLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents RPCalibratorALabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALBubbleResultTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents CALBubbleDateTextEdit As DevExpress.XtraEditors.TextEdit
        Friend WithEvents CALBubbleIcon As Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Friend WithEvents CALBubbleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents CALBubbleWarningLabel As Biosystems.Ax00.Controls.UserControls.BSLabel

    End Class
End Namespace
