<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiConfigLIS
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiConfigLIS))
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsLISConfigGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsLISConfigurationLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLISConfigTabControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl()
        Me.SessionSettingsTab = New System.Windows.Forms.TabPage()
        Me.BsUploadGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsAutomaticCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsFreqComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsFrequencyLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsUploadResultsOnResetCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsUploadUnsolicitedQCResultsCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsUploadUnsolicitedPatientResultsCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsDownloadGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsMaxTimeToWaitLISOrdersLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.BsAutoQueryStartCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsWSReflexTestsComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.BsWSReflexTestsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsWSRerunsComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.BsWSRerunsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsAcceptOnRunningCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsAcceptUnsolicitedOrdersCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsHostQueryCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.CommunicationsSettingsTab = New System.Windows.Forms.TabPage()
        Me.BsLISCommsEnabledCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsInternalGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsTimeoutQueueLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsMaxTimeToRespondNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.BsMaxTransmissionMsgsNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.BsMaxTransmissionMsgsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsMaxReceptionMsgsNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.BsMaxReceptionMsgsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLISCommsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.HostNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsDataTransmissionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsDataTransmissionComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.TCPPort2NumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.TCPPort2Label = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.HostNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.TCPPortNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.TCPPortLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.ProtocolSettingsTab = New System.Windows.Forms.TabPage()
        Me.bsLISProtocolGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsInstrumentIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsInstrumentProviderTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsHostProviderTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsHostIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsTransmissionCodePageTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsDelimitersGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsSubComponentSeparatorTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsSpecialSeparatorTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsRepeatSeparatorTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsFieldSeparatorTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsComponentSeparatorTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsSubcomponentLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsSpecialLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsRepeatLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsComponentLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsFieldLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsmaxTimeWaitACKNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.BsmaxTimeWaitACKLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsInstrumentProviderLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsInstrumentIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsHostProviderLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsHostIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsmaxTimeWaitingForResponseNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.BsmaxTimeWaitingForResponseLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsHostQueryPackageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsIHECompliantCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsHostQueryPackageNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.BsTransmissionCodePageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsProtocolNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsProtocolNameComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bwPreload = New System.ComponentModel.BackgroundWorker()
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsLISConfigGroupBox.SuspendLayout()
        Me.bsLISConfigTabControl.SuspendLayout()
        Me.SessionSettingsTab.SuspendLayout()
        Me.BsUploadGroupBox.SuspendLayout()
        Me.BsDownloadGroupBox.SuspendLayout()
        CType(Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CommunicationsSettingsTab.SuspendLayout()
        Me.bsInternalGroupBox.SuspendLayout()
        CType(Me.BsMaxTimeToRespondNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BsMaxTransmissionMsgsNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BsMaxReceptionMsgsNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsLISCommsGroupBox.SuspendLayout()
        CType(Me.TCPPort2NumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TCPPortNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ProtocolSettingsTab.SuspendLayout()
        Me.bsLISProtocolGroupBox.SuspendLayout()
        Me.BsDelimitersGroupBox.SuspendLayout()
        CType(Me.BsmaxTimeWaitACKNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BsmaxTimeWaitingForResponseNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BsHostQueryPackageNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsCancelButton
        '
        Me.bsCancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Location = New System.Drawing.Point(706, 567)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 21
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsErrorProvider1
        '
        Me.bsErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsErrorProvider1.ContainerControl = Me
        '
        'bsLISConfigGroupBox
        '
        Me.bsLISConfigGroupBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsLISConfigGroupBox.Controls.Add(Me.bsLISConfigurationLabel)
        Me.bsLISConfigGroupBox.Controls.Add(Me.bsLISConfigTabControl)
        Me.bsLISConfigGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLISConfigGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsLISConfigGroupBox.Name = "bsLISConfigGroupBox"
        Me.bsLISConfigGroupBox.Size = New System.Drawing.Size(728, 547)
        Me.bsLISConfigGroupBox.TabIndex = 171
        Me.bsLISConfigGroupBox.TabStop = False
        '
        'bsLISConfigurationLabel
        '
        Me.bsLISConfigurationLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsLISConfigurationLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsLISConfigurationLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsLISConfigurationLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLISConfigurationLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsLISConfigurationLabel.Name = "bsLISConfigurationLabel"
        Me.bsLISConfigurationLabel.Size = New System.Drawing.Size(705, 20)
        Me.bsLISConfigurationLabel.TabIndex = 20
        Me.bsLISConfigurationLabel.Text = "LIS Configuration"
        Me.bsLISConfigurationLabel.Title = True
        '
        'bsLISConfigTabControl
        '
        Me.bsLISConfigTabControl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsLISConfigTabControl.Controls.Add(Me.SessionSettingsTab)
        Me.bsLISConfigTabControl.Controls.Add(Me.CommunicationsSettingsTab)
        Me.bsLISConfigTabControl.Controls.Add(Me.ProtocolSettingsTab)
        Me.bsLISConfigTabControl.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLISConfigTabControl.Location = New System.Drawing.Point(10, 45)
        Me.bsLISConfigTabControl.Name = "bsLISConfigTabControl"
        Me.bsLISConfigTabControl.SelectedIndex = 0
        Me.bsLISConfigTabControl.Size = New System.Drawing.Size(705, 492)
        Me.bsLISConfigTabControl.TabIndex = 0
        '
        'SessionSettingsTab
        '
        Me.SessionSettingsTab.BackColor = System.Drawing.Color.Gainsboro
        Me.SessionSettingsTab.Controls.Add(Me.BsUploadGroupBox)
        Me.SessionSettingsTab.Controls.Add(Me.BsDownloadGroupBox)
        Me.SessionSettingsTab.Location = New System.Drawing.Point(4, 22)
        Me.SessionSettingsTab.Name = "SessionSettingsTab"
        Me.SessionSettingsTab.Padding = New System.Windows.Forms.Padding(3)
        Me.SessionSettingsTab.Size = New System.Drawing.Size(697, 466)
        Me.SessionSettingsTab.TabIndex = 0
        Me.SessionSettingsTab.Text = "WorkSession"
        '
        'BsUploadGroupBox
        '
        Me.BsUploadGroupBox.Controls.Add(Me.BsAutomaticCheckbox)
        Me.BsUploadGroupBox.Controls.Add(Me.bsFreqComboBox)
        Me.BsUploadGroupBox.Controls.Add(Me.bsFrequencyLabel)
        Me.BsUploadGroupBox.Controls.Add(Me.BsUploadResultsOnResetCheckbox)
        Me.BsUploadGroupBox.Controls.Add(Me.BsUploadUnsolicitedQCResultsCheckbox)
        Me.BsUploadGroupBox.Controls.Add(Me.BsUploadUnsolicitedPatientResultsCheckbox)
        Me.BsUploadGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsUploadGroupBox.ForeColor = System.Drawing.Color.Black
        Me.BsUploadGroupBox.Location = New System.Drawing.Point(6, 190)
        Me.BsUploadGroupBox.Name = "BsUploadGroupBox"
        Me.BsUploadGroupBox.Size = New System.Drawing.Size(685, 270)
        Me.BsUploadGroupBox.TabIndex = 13
        Me.BsUploadGroupBox.TabStop = False
        Me.BsUploadGroupBox.Text = "*Select Upload Results scenarios"
        '
        'BsAutomaticCheckbox
        '
        Me.BsAutomaticCheckbox.AutoSize = True
        Me.BsAutomaticCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAutomaticCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAutomaticCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsAutomaticCheckbox.Location = New System.Drawing.Point(20, 116)
        Me.BsAutomaticCheckbox.Name = "BsAutomaticCheckbox"
        Me.BsAutomaticCheckbox.Size = New System.Drawing.Size(178, 17)
        Me.BsAutomaticCheckbox.TabIndex = 8
        Me.BsAutomaticCheckbox.Text = "*Automatic Results Upload"
        Me.BsAutomaticCheckbox.UseVisualStyleBackColor = False
        '
        'bsFreqComboBox
        '
        Me.bsFreqComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsFreqComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsFreqComboBox.FormattingEnabled = True
        Me.bsFreqComboBox.Location = New System.Drawing.Point(41, 161)
        Me.bsFreqComboBox.Name = "bsFreqComboBox"
        Me.bsFreqComboBox.Size = New System.Drawing.Size(296, 21)
        Me.bsFreqComboBox.TabIndex = 9
        '
        'bsFrequencyLabel
        '
        Me.bsFrequencyLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsFrequencyLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFrequencyLabel.ForeColor = System.Drawing.Color.Black
        Me.bsFrequencyLabel.Location = New System.Drawing.Point(41, 143)
        Me.bsFrequencyLabel.Name = "bsFrequencyLabel"
        Me.bsFrequencyLabel.Size = New System.Drawing.Size(220, 13)
        Me.bsFrequencyLabel.TabIndex = 16
        Me.bsFrequencyLabel.Text = "*Frequency:"
        Me.bsFrequencyLabel.Title = False
        '
        'BsUploadResultsOnResetCheckbox
        '
        Me.BsUploadResultsOnResetCheckbox.AutoSize = True
        Me.BsUploadResultsOnResetCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsUploadResultsOnResetCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsUploadResultsOnResetCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsUploadResultsOnResetCheckbox.Location = New System.Drawing.Point(20, 86)
        Me.BsUploadResultsOnResetCheckbox.Name = "BsUploadResultsOnResetCheckbox"
        Me.BsUploadResultsOnResetCheckbox.Size = New System.Drawing.Size(264, 17)
        Me.BsUploadResultsOnResetCheckbox.TabIndex = 7
        Me.BsUploadResultsOnResetCheckbox.Text = "*Upload results during Reset worksession"
        Me.BsUploadResultsOnResetCheckbox.UseVisualStyleBackColor = False
        '
        'BsUploadUnsolicitedQCResultsCheckbox
        '
        Me.BsUploadUnsolicitedQCResultsCheckbox.AutoSize = True
        Me.BsUploadUnsolicitedQCResultsCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsUploadUnsolicitedQCResultsCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsUploadUnsolicitedQCResultsCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsUploadUnsolicitedQCResultsCheckbox.Location = New System.Drawing.Point(20, 58)
        Me.BsUploadUnsolicitedQCResultsCheckbox.Name = "BsUploadUnsolicitedQCResultsCheckbox"
        Me.BsUploadUnsolicitedQCResultsCheckbox.Size = New System.Drawing.Size(424, 17)
        Me.BsUploadUnsolicitedQCResultsCheckbox.TabIndex = 6
        Me.BsUploadUnsolicitedQCResultsCheckbox.Text = "*Upload QC results of orders requested form Analyzer (not from LIS)"
        Me.BsUploadUnsolicitedQCResultsCheckbox.UseVisualStyleBackColor = False
        '
        'BsUploadUnsolicitedPatientResultsCheckbox
        '
        Me.BsUploadUnsolicitedPatientResultsCheckbox.AutoSize = True
        Me.BsUploadUnsolicitedPatientResultsCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsUploadUnsolicitedPatientResultsCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsUploadUnsolicitedPatientResultsCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsUploadUnsolicitedPatientResultsCheckbox.Location = New System.Drawing.Point(20, 30)
        Me.BsUploadUnsolicitedPatientResultsCheckbox.Name = "BsUploadUnsolicitedPatientResultsCheckbox"
        Me.BsUploadUnsolicitedPatientResultsCheckbox.Size = New System.Drawing.Size(448, 17)
        Me.BsUploadUnsolicitedPatientResultsCheckbox.TabIndex = 5
        Me.BsUploadUnsolicitedPatientResultsCheckbox.Text = "*Upload Patient Results of orders requested from Analyzer (not from LIS)"
        Me.BsUploadUnsolicitedPatientResultsCheckbox.UseVisualStyleBackColor = False
        '
        'BsDownloadGroupBox
        '
        Me.BsDownloadGroupBox.Controls.Add(Me.BsMaxTimeToWaitLISOrdersLabel)
        Me.BsDownloadGroupBox.Controls.Add(Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown)
        Me.BsDownloadGroupBox.Controls.Add(Me.BsAutoQueryStartCheckbox)
        Me.BsDownloadGroupBox.Controls.Add(Me.BsWSReflexTestsComboBox)
        Me.BsDownloadGroupBox.Controls.Add(Me.BsWSReflexTestsLabel)
        Me.BsDownloadGroupBox.Controls.Add(Me.BsWSRerunsComboBox)
        Me.BsDownloadGroupBox.Controls.Add(Me.BsWSRerunsLabel)
        Me.BsDownloadGroupBox.Controls.Add(Me.BsAcceptOnRunningCheckbox)
        Me.BsDownloadGroupBox.Controls.Add(Me.BsAcceptUnsolicitedOrdersCheckbox)
        Me.BsDownloadGroupBox.Controls.Add(Me.BsHostQueryCheckbox)
        Me.BsDownloadGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsDownloadGroupBox.ForeColor = System.Drawing.Color.Black
        Me.BsDownloadGroupBox.Location = New System.Drawing.Point(6, 15)
        Me.BsDownloadGroupBox.Name = "BsDownloadGroupBox"
        Me.BsDownloadGroupBox.Size = New System.Drawing.Size(685, 169)
        Me.BsDownloadGroupBox.TabIndex = 12
        Me.BsDownloadGroupBox.TabStop = False
        Me.BsDownloadGroupBox.Text = "*Select Download Orders scenarios"
        '
        'BsMaxTimeToWaitLISOrdersLabel
        '
        Me.BsMaxTimeToWaitLISOrdersLabel.AutoSize = True
        Me.BsMaxTimeToWaitLISOrdersLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsMaxTimeToWaitLISOrdersLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsMaxTimeToWaitLISOrdersLabel.ForeColor = System.Drawing.Color.Black
        Me.BsMaxTimeToWaitLISOrdersLabel.Location = New System.Drawing.Point(21, 131)
        Me.BsMaxTimeToWaitLISOrdersLabel.Name = "BsMaxTimeToWaitLISOrdersLabel"
        Me.BsMaxTimeToWaitLISOrdersLabel.Size = New System.Drawing.Size(246, 13)
        Me.BsMaxTimeToWaitLISOrdersLabel.TabIndex = 60
        Me.BsMaxTimeToWaitLISOrdersLabel.Text = "*Maximum Time waiting for LIS Order[s]:"
        Me.BsMaxTimeToWaitLISOrdersLabel.Title = False
        '
        'BsBsMaxTimeToWaitLISOrdersNumericUpDown
        '
        Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown.Location = New System.Drawing.Point(348, 129)
        Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown.Maximum = New Decimal(New Integer() {60, 0, 0, 0})
        Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown.Name = "BsBsMaxTimeToWaitLISOrdersNumericUpDown"
        Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown.Size = New System.Drawing.Size(71, 21)
        Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown.TabIndex = 59
        Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown.Value = New Decimal(New Integer() {20, 0, 0, 0})
        '
        'BsAutoQueryStartCheckbox
        '
        Me.BsAutoQueryStartCheckbox.AutoSize = True
        Me.BsAutoQueryStartCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAutoQueryStartCheckbox.Checked = True
        Me.BsAutoQueryStartCheckbox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.BsAutoQueryStartCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAutoQueryStartCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsAutoQueryStartCheckbox.Location = New System.Drawing.Point(20, 100)
        Me.BsAutoQueryStartCheckbox.Name = "BsAutoQueryStartCheckbox"
        Me.BsAutoQueryStartCheckbox.Size = New System.Drawing.Size(402, 17)
        Me.BsAutoQueryStartCheckbox.TabIndex = 25
        Me.BsAutoQueryStartCheckbox.Text = "*Automatic Query to LIS (before starting/continuing worksession)"
        Me.BsAutoQueryStartCheckbox.UseVisualStyleBackColor = False
        '
        'BsWSReflexTestsComboBox
        '
        Me.BsWSReflexTestsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.BsWSReflexTestsComboBox.ForeColor = System.Drawing.Color.Black
        Me.BsWSReflexTestsComboBox.FormattingEnabled = True
        Me.BsWSReflexTestsComboBox.Location = New System.Drawing.Point(605, 143)
        Me.BsWSReflexTestsComboBox.Name = "BsWSReflexTestsComboBox"
        Me.BsWSReflexTestsComboBox.Size = New System.Drawing.Size(50, 21)
        Me.BsWSReflexTestsComboBox.TabIndex = 23
        Me.BsWSReflexTestsComboBox.Visible = False
        '
        'BsWSReflexTestsLabel
        '
        Me.BsWSReflexTestsLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsWSReflexTestsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsWSReflexTestsLabel.ForeColor = System.Drawing.Color.Black
        Me.BsWSReflexTestsLabel.Location = New System.Drawing.Point(546, 153)
        Me.BsWSReflexTestsLabel.Name = "BsWSReflexTestsLabel"
        Me.BsWSReflexTestsLabel.Size = New System.Drawing.Size(53, 13)
        Me.BsWSReflexTestsLabel.TabIndex = 24
        Me.BsWSReflexTestsLabel.Text = "*Select Reflex working mode:"
        Me.BsWSReflexTestsLabel.Title = False
        Me.BsWSReflexTestsLabel.Visible = False
        '
        'BsWSRerunsComboBox
        '
        Me.BsWSRerunsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.BsWSRerunsComboBox.ForeColor = System.Drawing.Color.Black
        Me.BsWSRerunsComboBox.FormattingEnabled = True
        Me.BsWSRerunsComboBox.Location = New System.Drawing.Point(348, 46)
        Me.BsWSRerunsComboBox.Name = "BsWSRerunsComboBox"
        Me.BsWSRerunsComboBox.Size = New System.Drawing.Size(267, 21)
        Me.BsWSRerunsComboBox.TabIndex = 2
        '
        'BsWSRerunsLabel
        '
        Me.BsWSRerunsLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsWSRerunsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsWSRerunsLabel.ForeColor = System.Drawing.Color.Black
        Me.BsWSRerunsLabel.Location = New System.Drawing.Point(345, 30)
        Me.BsWSRerunsLabel.Name = "BsWSRerunsLabel"
        Me.BsWSRerunsLabel.Size = New System.Drawing.Size(270, 13)
        Me.BsWSRerunsLabel.TabIndex = 22
        Me.BsWSRerunsLabel.Text = "*Select Rerun working mode:"
        Me.BsWSRerunsLabel.Title = False
        '
        'BsAcceptOnRunningCheckbox
        '
        Me.BsAcceptOnRunningCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAcceptOnRunningCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAcceptOnRunningCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsAcceptOnRunningCheckbox.Location = New System.Drawing.Point(519, 100)
        Me.BsAcceptOnRunningCheckbox.Name = "BsAcceptOnRunningCheckbox"
        Me.BsAcceptOnRunningCheckbox.Size = New System.Drawing.Size(72, 37)
        Me.BsAcceptOnRunningCheckbox.TabIndex = 4
        Me.BsAcceptOnRunningCheckbox.Text = "*Downloading Orders in Running mode"
        Me.BsAcceptOnRunningCheckbox.UseVisualStyleBackColor = False
        Me.BsAcceptOnRunningCheckbox.Visible = False
        '
        'BsAcceptUnsolicitedOrdersCheckbox
        '
        Me.BsAcceptUnsolicitedOrdersCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsAcceptUnsolicitedOrdersCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsAcceptUnsolicitedOrdersCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsAcceptUnsolicitedOrdersCheckbox.Location = New System.Drawing.Point(597, 100)
        Me.BsAcceptUnsolicitedOrdersCheckbox.Name = "BsAcceptUnsolicitedOrdersCheckbox"
        Me.BsAcceptUnsolicitedOrdersCheckbox.Size = New System.Drawing.Size(58, 37)
        Me.BsAcceptUnsolicitedOrdersCheckbox.TabIndex = 3
        Me.BsAcceptUnsolicitedOrdersCheckbox.Text = "*Unsolicited Orders (without Query from analyzer)"
        Me.BsAcceptUnsolicitedOrdersCheckbox.UseVisualStyleBackColor = False
        Me.BsAcceptUnsolicitedOrdersCheckbox.Visible = False
        '
        'BsHostQueryCheckbox
        '
        Me.BsHostQueryCheckbox.AutoSize = True
        Me.BsHostQueryCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsHostQueryCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsHostQueryCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsHostQueryCheckbox.Location = New System.Drawing.Point(20, 30)
        Me.BsHostQueryCheckbox.Name = "BsHostQueryCheckbox"
        Me.BsHostQueryCheckbox.Size = New System.Drawing.Size(97, 17)
        Me.BsHostQueryCheckbox.TabIndex = 1
        Me.BsHostQueryCheckbox.Text = "*Host Query"
        Me.BsHostQueryCheckbox.UseVisualStyleBackColor = False
        '
        'CommunicationsSettingsTab
        '
        Me.CommunicationsSettingsTab.BackColor = System.Drawing.Color.Gainsboro
        Me.CommunicationsSettingsTab.Controls.Add(Me.BsLISCommsEnabledCheckbox)
        Me.CommunicationsSettingsTab.Controls.Add(Me.bsInternalGroupBox)
        Me.CommunicationsSettingsTab.Controls.Add(Me.bsLISCommsGroupBox)
        Me.CommunicationsSettingsTab.Location = New System.Drawing.Point(4, 22)
        Me.CommunicationsSettingsTab.Name = "CommunicationsSettingsTab"
        Me.CommunicationsSettingsTab.Size = New System.Drawing.Size(697, 466)
        Me.CommunicationsSettingsTab.TabIndex = 3
        Me.CommunicationsSettingsTab.Text = "Communications"
        '
        'BsLISCommsEnabledCheckbox
        '
        Me.BsLISCommsEnabledCheckbox.AutoSize = True
        Me.BsLISCommsEnabledCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsLISCommsEnabledCheckbox.ForeColor = System.Drawing.Color.Black
        Me.BsLISCommsEnabledCheckbox.Location = New System.Drawing.Point(16, 21)
        Me.BsLISCommsEnabledCheckbox.Name = "BsLISCommsEnabledCheckbox"
        Me.BsLISCommsEnabledCheckbox.Size = New System.Drawing.Size(191, 17)
        Me.BsLISCommsEnabledCheckbox.TabIndex = 1
        Me.BsLISCommsEnabledCheckbox.Text = "*LIS communication enabled"
        Me.BsLISCommsEnabledCheckbox.UseVisualStyleBackColor = True
        '
        'bsInternalGroupBox
        '
        Me.bsInternalGroupBox.Controls.Add(Me.BsTimeoutQueueLabel)
        Me.bsInternalGroupBox.Controls.Add(Me.BsMaxTimeToRespondNumericUpDown)
        Me.bsInternalGroupBox.Controls.Add(Me.BsMaxTransmissionMsgsNumericUpDown)
        Me.bsInternalGroupBox.Controls.Add(Me.BsMaxTransmissionMsgsLabel)
        Me.bsInternalGroupBox.Controls.Add(Me.BsMaxReceptionMsgsNumericUpDown)
        Me.bsInternalGroupBox.Controls.Add(Me.BsMaxReceptionMsgsLabel)
        Me.bsInternalGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsInternalGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsInternalGroupBox.Location = New System.Drawing.Point(16, 289)
        Me.bsInternalGroupBox.Name = "bsInternalGroupBox"
        Me.bsInternalGroupBox.Size = New System.Drawing.Size(661, 147)
        Me.bsInternalGroupBox.TabIndex = 56
        Me.bsInternalGroupBox.TabStop = False
        Me.bsInternalGroupBox.Text = "  *Internal LIS settings"
        '
        'BsTimeoutQueueLabel
        '
        Me.BsTimeoutQueueLabel.AutoSize = True
        Me.BsTimeoutQueueLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsTimeoutQueueLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsTimeoutQueueLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTimeoutQueueLabel.Location = New System.Drawing.Point(352, 30)
        Me.BsTimeoutQueueLabel.Name = "BsTimeoutQueueLabel"
        Me.BsTimeoutQueueLabel.Size = New System.Drawing.Size(227, 13)
        Me.BsTimeoutQueueLabel.TabIndex = 71
        Me.BsTimeoutQueueLabel.Text = "*Maximum time to respond by BA400:"
        Me.BsTimeoutQueueLabel.Title = False
        '
        'BsMaxTimeToRespondNumericUpDown
        '
        Me.BsMaxTimeToRespondNumericUpDown.Location = New System.Drawing.Point(355, 47)
        Me.BsMaxTimeToRespondNumericUpDown.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.BsMaxTimeToRespondNumericUpDown.Name = "BsMaxTimeToRespondNumericUpDown"
        Me.BsMaxTimeToRespondNumericUpDown.Size = New System.Drawing.Size(71, 21)
        Me.BsMaxTimeToRespondNumericUpDown.TabIndex = 8
        Me.BsMaxTimeToRespondNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsMaxTimeToRespondNumericUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsMaxTransmissionMsgsNumericUpDown
        '
        Me.BsMaxTransmissionMsgsNumericUpDown.Location = New System.Drawing.Point(23, 97)
        Me.BsMaxTransmissionMsgsNumericUpDown.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.BsMaxTransmissionMsgsNumericUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.BsMaxTransmissionMsgsNumericUpDown.Name = "BsMaxTransmissionMsgsNumericUpDown"
        Me.BsMaxTransmissionMsgsNumericUpDown.Size = New System.Drawing.Size(71, 21)
        Me.BsMaxTransmissionMsgsNumericUpDown.TabIndex = 7
        Me.BsMaxTransmissionMsgsNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsMaxTransmissionMsgsNumericUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsMaxTransmissionMsgsLabel
        '
        Me.BsMaxTransmissionMsgsLabel.AutoSize = True
        Me.BsMaxTransmissionMsgsLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsMaxTransmissionMsgsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsMaxTransmissionMsgsLabel.ForeColor = System.Drawing.Color.Black
        Me.BsMaxTransmissionMsgsLabel.Location = New System.Drawing.Point(23, 79)
        Me.BsMaxTransmissionMsgsLabel.Name = "BsMaxTransmissionMsgsLabel"
        Me.BsMaxTransmissionMsgsLabel.Size = New System.Drawing.Size(223, 13)
        Me.BsMaxTransmissionMsgsLabel.TabIndex = 69
        Me.BsMaxTransmissionMsgsLabel.Text = "*Internal storage size (transmission):"
        Me.BsMaxTransmissionMsgsLabel.Title = False
        '
        'BsMaxReceptionMsgsNumericUpDown
        '
        Me.BsMaxReceptionMsgsNumericUpDown.Location = New System.Drawing.Point(23, 46)
        Me.BsMaxReceptionMsgsNumericUpDown.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.BsMaxReceptionMsgsNumericUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.BsMaxReceptionMsgsNumericUpDown.Name = "BsMaxReceptionMsgsNumericUpDown"
        Me.BsMaxReceptionMsgsNumericUpDown.Size = New System.Drawing.Size(71, 21)
        Me.BsMaxReceptionMsgsNumericUpDown.TabIndex = 6
        Me.BsMaxReceptionMsgsNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsMaxReceptionMsgsNumericUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsMaxReceptionMsgsLabel
        '
        Me.BsMaxReceptionMsgsLabel.AutoSize = True
        Me.BsMaxReceptionMsgsLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsMaxReceptionMsgsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsMaxReceptionMsgsLabel.ForeColor = System.Drawing.Color.Black
        Me.BsMaxReceptionMsgsLabel.Location = New System.Drawing.Point(23, 30)
        Me.BsMaxReceptionMsgsLabel.Name = "BsMaxReceptionMsgsLabel"
        Me.BsMaxReceptionMsgsLabel.Size = New System.Drawing.Size(204, 13)
        Me.BsMaxReceptionMsgsLabel.TabIndex = 67
        Me.BsMaxReceptionMsgsLabel.Text = "*Internal storage size (reception):"
        Me.BsMaxReceptionMsgsLabel.Title = False
        '
        'bsLISCommsGroupBox
        '
        Me.bsLISCommsGroupBox.Controls.Add(Me.HostNameTextBox)
        Me.bsLISCommsGroupBox.Controls.Add(Me.BsDataTransmissionLabel)
        Me.bsLISCommsGroupBox.Controls.Add(Me.BsDataTransmissionComboBox)
        Me.bsLISCommsGroupBox.Controls.Add(Me.TCPPort2NumericUpDown)
        Me.bsLISCommsGroupBox.Controls.Add(Me.TCPPort2Label)
        Me.bsLISCommsGroupBox.Controls.Add(Me.HostNameLabel)
        Me.bsLISCommsGroupBox.Controls.Add(Me.TCPPortNumericUpDown)
        Me.bsLISCommsGroupBox.Controls.Add(Me.TCPPortLabel)
        Me.bsLISCommsGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLISCommsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLISCommsGroupBox.Location = New System.Drawing.Point(16, 47)
        Me.bsLISCommsGroupBox.Name = "bsLISCommsGroupBox"
        Me.bsLISCommsGroupBox.Size = New System.Drawing.Size(661, 236)
        Me.bsLISCommsGroupBox.TabIndex = 11
        Me.bsLISCommsGroupBox.TabStop = False
        Me.bsLISCommsGroupBox.Text = "  *Communications LIS settings"
        '
        'HostNameTextBox
        '
        Me.HostNameTextBox.BackColor = System.Drawing.Color.Khaki
        Me.HostNameTextBox.DecimalsValues = False
        Me.HostNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.HostNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.HostNameTextBox.IsNumeric = False
        Me.HostNameTextBox.Location = New System.Drawing.Point(23, 104)
        Me.HostNameTextBox.Mandatory = True
        Me.HostNameTextBox.MaxLength = 25
        Me.HostNameTextBox.Name = "HostNameTextBox"
        Me.HostNameTextBox.Size = New System.Drawing.Size(231, 21)
        Me.HostNameTextBox.TabIndex = 3
        Me.HostNameTextBox.WordWrap = False
        '
        'BsDataTransmissionLabel
        '
        Me.BsDataTransmissionLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsDataTransmissionLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsDataTransmissionLabel.ForeColor = System.Drawing.Color.Black
        Me.BsDataTransmissionLabel.Location = New System.Drawing.Point(23, 34)
        Me.BsDataTransmissionLabel.Name = "BsDataTransmissionLabel"
        Me.BsDataTransmissionLabel.Size = New System.Drawing.Size(231, 13)
        Me.BsDataTransmissionLabel.TabIndex = 58
        Me.BsDataTransmissionLabel.Text = "*Data Transmission type:"
        Me.BsDataTransmissionLabel.Title = False
        '
        'BsDataTransmissionComboBox
        '
        Me.BsDataTransmissionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.BsDataTransmissionComboBox.ForeColor = System.Drawing.Color.Black
        Me.BsDataTransmissionComboBox.FormattingEnabled = True
        Me.BsDataTransmissionComboBox.Location = New System.Drawing.Point(23, 52)
        Me.BsDataTransmissionComboBox.Name = "BsDataTransmissionComboBox"
        Me.BsDataTransmissionComboBox.Size = New System.Drawing.Size(231, 21)
        Me.BsDataTransmissionComboBox.TabIndex = 2
        '
        'TCPPort2NumericUpDown
        '
        Me.TCPPort2NumericUpDown.Location = New System.Drawing.Point(23, 199)
        Me.TCPPort2NumericUpDown.Maximum = New Decimal(New Integer() {5000, 0, 0, 0})
        Me.TCPPort2NumericUpDown.Minimum = New Decimal(New Integer() {1024, 0, 0, 0})
        Me.TCPPort2NumericUpDown.Name = "TCPPort2NumericUpDown"
        Me.TCPPort2NumericUpDown.Size = New System.Drawing.Size(71, 21)
        Me.TCPPort2NumericUpDown.TabIndex = 5
        Me.TCPPort2NumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TCPPort2NumericUpDown.Value = New Decimal(New Integer() {1024, 0, 0, 0})
        '
        'TCPPort2Label
        '
        Me.TCPPort2Label.AutoSize = True
        Me.TCPPort2Label.BackColor = System.Drawing.Color.Transparent
        Me.TCPPort2Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.TCPPort2Label.ForeColor = System.Drawing.Color.Black
        Me.TCPPort2Label.Location = New System.Drawing.Point(23, 183)
        Me.TCPPort2Label.Name = "TCPPort2Label"
        Me.TCPPort2Label.Size = New System.Drawing.Size(85, 13)
        Me.TCPPort2Label.TabIndex = 56
        Me.TCPPort2Label.Text = "*Port Server:"
        Me.TCPPort2Label.Title = False
        '
        'HostNameLabel
        '
        Me.HostNameLabel.AutoSize = True
        Me.HostNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.HostNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.HostNameLabel.ForeColor = System.Drawing.Color.Black
        Me.HostNameLabel.Location = New System.Drawing.Point(23, 86)
        Me.HostNameLabel.Name = "HostNameLabel"
        Me.HostNameLabel.Size = New System.Drawing.Size(80, 13)
        Me.HostNameLabel.TabIndex = 50
        Me.HostNameLabel.Text = "*Host name:"
        Me.HostNameLabel.Title = False
        '
        'TCPPortNumericUpDown
        '
        Me.TCPPortNumericUpDown.Location = New System.Drawing.Point(23, 148)
        Me.TCPPortNumericUpDown.Maximum = New Decimal(New Integer() {5000, 0, 0, 0})
        Me.TCPPortNumericUpDown.Minimum = New Decimal(New Integer() {1024, 0, 0, 0})
        Me.TCPPortNumericUpDown.Name = "TCPPortNumericUpDown"
        Me.TCPPortNumericUpDown.Size = New System.Drawing.Size(71, 21)
        Me.TCPPortNumericUpDown.TabIndex = 4
        Me.TCPPortNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TCPPortNumericUpDown.Value = New Decimal(New Integer() {1024, 0, 0, 0})
        '
        'TCPPortLabel
        '
        Me.TCPPortLabel.AutoSize = True
        Me.TCPPortLabel.BackColor = System.Drawing.Color.Transparent
        Me.TCPPortLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.TCPPortLabel.ForeColor = System.Drawing.Color.Black
        Me.TCPPortLabel.Location = New System.Drawing.Point(26, 132)
        Me.TCPPortLabel.Name = "TCPPortLabel"
        Me.TCPPortLabel.Size = New System.Drawing.Size(42, 13)
        Me.TCPPortLabel.TabIndex = 52
        Me.TCPPortLabel.Text = "*Port:"
        Me.TCPPortLabel.Title = False
        '
        'ProtocolSettingsTab
        '
        Me.ProtocolSettingsTab.BackColor = System.Drawing.Color.Gainsboro
        Me.ProtocolSettingsTab.Controls.Add(Me.bsLISProtocolGroupBox)
        Me.ProtocolSettingsTab.ForeColor = System.Drawing.Color.Black
        Me.ProtocolSettingsTab.Location = New System.Drawing.Point(4, 22)
        Me.ProtocolSettingsTab.Name = "ProtocolSettingsTab"
        Me.ProtocolSettingsTab.Padding = New System.Windows.Forms.Padding(3)
        Me.ProtocolSettingsTab.Size = New System.Drawing.Size(697, 466)
        Me.ProtocolSettingsTab.TabIndex = 1
        Me.ProtocolSettingsTab.Text = "Protocol"
        '
        'bsLISProtocolGroupBox
        '
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsInstrumentIDTextBox)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsInstrumentProviderTextBox)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsHostProviderTextBox)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsHostIDTextBox)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsTransmissionCodePageTextBox)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsDelimitersGroupBox)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsmaxTimeWaitACKNumericUpDown)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsmaxTimeWaitACKLabel)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsInstrumentProviderLabel)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsInstrumentIDLabel)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsHostProviderLabel)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsHostIDLabel)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsmaxTimeWaitingForResponseNumericUpDown)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsmaxTimeWaitingForResponseLabel)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsHostQueryPackageLabel)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.bsIHECompliantCheckbox)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsHostQueryPackageNumericUpDown)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.BsTransmissionCodePageLabel)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.bsProtocolNameLabel)
        Me.bsLISProtocolGroupBox.Controls.Add(Me.bsProtocolNameComboBox)
        Me.bsLISProtocolGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLISProtocolGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsLISProtocolGroupBox.Location = New System.Drawing.Point(6, 20)
        Me.bsLISProtocolGroupBox.Name = "bsLISProtocolGroupBox"
        Me.bsLISProtocolGroupBox.Size = New System.Drawing.Size(685, 440)
        Me.bsLISProtocolGroupBox.TabIndex = 75
        Me.bsLISProtocolGroupBox.TabStop = False
        Me.bsLISProtocolGroupBox.Text = "  *Protocol settings"
        '
        'BsInstrumentIDTextBox
        '
        Me.BsInstrumentIDTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsInstrumentIDTextBox.DecimalsValues = False
        Me.BsInstrumentIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsInstrumentIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsInstrumentIDTextBox.IsNumeric = False
        Me.BsInstrumentIDTextBox.Location = New System.Drawing.Point(23, 246)
        Me.BsInstrumentIDTextBox.Mandatory = True
        Me.BsInstrumentIDTextBox.MaxLength = 20
        Me.BsInstrumentIDTextBox.Name = "BsInstrumentIDTextBox"
        Me.BsInstrumentIDTextBox.Size = New System.Drawing.Size(231, 21)
        Me.BsInstrumentIDTextBox.TabIndex = 5
        Me.BsInstrumentIDTextBox.WordWrap = False
        '
        'BsInstrumentProviderTextBox
        '
        Me.BsInstrumentProviderTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsInstrumentProviderTextBox.DecimalsValues = False
        Me.BsInstrumentProviderTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsInstrumentProviderTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsInstrumentProviderTextBox.IsNumeric = False
        Me.BsInstrumentProviderTextBox.Location = New System.Drawing.Point(23, 295)
        Me.BsInstrumentProviderTextBox.Mandatory = True
        Me.BsInstrumentProviderTextBox.MaxLength = 20
        Me.BsInstrumentProviderTextBox.Name = "BsInstrumentProviderTextBox"
        Me.BsInstrumentProviderTextBox.ReadOnly = True
        Me.BsInstrumentProviderTextBox.Size = New System.Drawing.Size(231, 21)
        Me.BsInstrumentProviderTextBox.TabIndex = 6
        Me.BsInstrumentProviderTextBox.WordWrap = False
        '
        'BsHostProviderTextBox
        '
        Me.BsHostProviderTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsHostProviderTextBox.DecimalsValues = False
        Me.BsHostProviderTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsHostProviderTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsHostProviderTextBox.IsNumeric = False
        Me.BsHostProviderTextBox.Location = New System.Drawing.Point(23, 195)
        Me.BsHostProviderTextBox.Mandatory = True
        Me.BsHostProviderTextBox.MaxLength = 20
        Me.BsHostProviderTextBox.Name = "BsHostProviderTextBox"
        Me.BsHostProviderTextBox.Size = New System.Drawing.Size(231, 21)
        Me.BsHostProviderTextBox.TabIndex = 4
        Me.BsHostProviderTextBox.WordWrap = False
        '
        'BsHostIDTextBox
        '
        Me.BsHostIDTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsHostIDTextBox.DecimalsValues = False
        Me.BsHostIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsHostIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsHostIDTextBox.IsNumeric = False
        Me.BsHostIDTextBox.Location = New System.Drawing.Point(23, 147)
        Me.BsHostIDTextBox.Mandatory = True
        Me.BsHostIDTextBox.MaxLength = 20
        Me.BsHostIDTextBox.Name = "BsHostIDTextBox"
        Me.BsHostIDTextBox.Size = New System.Drawing.Size(231, 21)
        Me.BsHostIDTextBox.TabIndex = 3
        Me.BsHostIDTextBox.WordWrap = False
        '
        'BsTransmissionCodePageTextBox
        '
        Me.BsTransmissionCodePageTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsTransmissionCodePageTextBox.DecimalsValues = False
        Me.BsTransmissionCodePageTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsTransmissionCodePageTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsTransmissionCodePageTextBox.IsNumeric = False
        Me.BsTransmissionCodePageTextBox.Location = New System.Drawing.Point(20, 88)
        Me.BsTransmissionCodePageTextBox.Mandatory = True
        Me.BsTransmissionCodePageTextBox.MaxLength = 25
        Me.BsTransmissionCodePageTextBox.Name = "BsTransmissionCodePageTextBox"
        Me.BsTransmissionCodePageTextBox.Size = New System.Drawing.Size(231, 21)
        Me.BsTransmissionCodePageTextBox.TabIndex = 2
        Me.BsTransmissionCodePageTextBox.WordWrap = False
        '
        'BsDelimitersGroupBox
        '
        Me.BsDelimitersGroupBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsDelimitersGroupBox.Controls.Add(Me.BsSubComponentSeparatorTextBox)
        Me.BsDelimitersGroupBox.Controls.Add(Me.BsSpecialSeparatorTextBox)
        Me.BsDelimitersGroupBox.Controls.Add(Me.BsRepeatSeparatorTextBox)
        Me.BsDelimitersGroupBox.Controls.Add(Me.BsFieldSeparatorTextBox)
        Me.BsDelimitersGroupBox.Controls.Add(Me.BsComponentSeparatorTextBox)
        Me.BsDelimitersGroupBox.Controls.Add(Me.BsSubcomponentLabel)
        Me.BsDelimitersGroupBox.Controls.Add(Me.BsSpecialLabel)
        Me.BsDelimitersGroupBox.Controls.Add(Me.BsRepeatLabel)
        Me.BsDelimitersGroupBox.Controls.Add(Me.BsComponentLabel)
        Me.BsDelimitersGroupBox.Controls.Add(Me.BsFieldLabel)
        Me.BsDelimitersGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsDelimitersGroupBox.ForeColor = System.Drawing.Color.Black
        Me.BsDelimitersGroupBox.Location = New System.Drawing.Point(375, 195)
        Me.BsDelimitersGroupBox.Name = "BsDelimitersGroupBox"
        Me.BsDelimitersGroupBox.Size = New System.Drawing.Size(304, 239)
        Me.BsDelimitersGroupBox.TabIndex = 89
        Me.BsDelimitersGroupBox.TabStop = False
        Me.BsDelimitersGroupBox.Text = "  *Delimiters"
        '
        'BsSubComponentSeparatorTextBox
        '
        Me.BsSubComponentSeparatorTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BsSubComponentSeparatorTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsSubComponentSeparatorTextBox.DecimalsValues = False
        Me.BsSubComponentSeparatorTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsSubComponentSeparatorTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsSubComponentSeparatorTextBox.IsNumeric = False
        Me.BsSubComponentSeparatorTextBox.Location = New System.Drawing.Point(18, 212)
        Me.BsSubComponentSeparatorTextBox.Mandatory = True
        Me.BsSubComponentSeparatorTextBox.MaxLength = 10
        Me.BsSubComponentSeparatorTextBox.Name = "BsSubComponentSeparatorTextBox"
        Me.BsSubComponentSeparatorTextBox.Size = New System.Drawing.Size(51, 21)
        Me.BsSubComponentSeparatorTextBox.TabIndex = 15
        Me.BsSubComponentSeparatorTextBox.WordWrap = False
        '
        'BsSpecialSeparatorTextBox
        '
        Me.BsSpecialSeparatorTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BsSpecialSeparatorTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsSpecialSeparatorTextBox.DecimalsValues = False
        Me.BsSpecialSeparatorTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsSpecialSeparatorTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsSpecialSeparatorTextBox.IsNumeric = False
        Me.BsSpecialSeparatorTextBox.Location = New System.Drawing.Point(18, 169)
        Me.BsSpecialSeparatorTextBox.Mandatory = True
        Me.BsSpecialSeparatorTextBox.MaxLength = 10
        Me.BsSpecialSeparatorTextBox.Name = "BsSpecialSeparatorTextBox"
        Me.BsSpecialSeparatorTextBox.Size = New System.Drawing.Size(51, 21)
        Me.BsSpecialSeparatorTextBox.TabIndex = 14
        Me.BsSpecialSeparatorTextBox.WordWrap = False
        '
        'BsRepeatSeparatorTextBox
        '
        Me.BsRepeatSeparatorTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BsRepeatSeparatorTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsRepeatSeparatorTextBox.DecimalsValues = False
        Me.BsRepeatSeparatorTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsRepeatSeparatorTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsRepeatSeparatorTextBox.IsNumeric = False
        Me.BsRepeatSeparatorTextBox.Location = New System.Drawing.Point(18, 126)
        Me.BsRepeatSeparatorTextBox.Mandatory = True
        Me.BsRepeatSeparatorTextBox.MaxLength = 10
        Me.BsRepeatSeparatorTextBox.Name = "BsRepeatSeparatorTextBox"
        Me.BsRepeatSeparatorTextBox.Size = New System.Drawing.Size(51, 21)
        Me.BsRepeatSeparatorTextBox.TabIndex = 13
        Me.BsRepeatSeparatorTextBox.WordWrap = False
        '
        'BsFieldSeparatorTextBox
        '
        Me.BsFieldSeparatorTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BsFieldSeparatorTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsFieldSeparatorTextBox.DecimalsValues = False
        Me.BsFieldSeparatorTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsFieldSeparatorTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsFieldSeparatorTextBox.IsNumeric = False
        Me.BsFieldSeparatorTextBox.Location = New System.Drawing.Point(18, 39)
        Me.BsFieldSeparatorTextBox.Mandatory = True
        Me.BsFieldSeparatorTextBox.MaxLength = 10
        Me.BsFieldSeparatorTextBox.Name = "BsFieldSeparatorTextBox"
        Me.BsFieldSeparatorTextBox.Size = New System.Drawing.Size(51, 21)
        Me.BsFieldSeparatorTextBox.TabIndex = 11
        Me.BsFieldSeparatorTextBox.WordWrap = False
        '
        'BsComponentSeparatorTextBox
        '
        Me.BsComponentSeparatorTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BsComponentSeparatorTextBox.BackColor = System.Drawing.Color.Khaki
        Me.BsComponentSeparatorTextBox.DecimalsValues = False
        Me.BsComponentSeparatorTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsComponentSeparatorTextBox.ForeColor = System.Drawing.Color.Black
        Me.BsComponentSeparatorTextBox.IsNumeric = False
        Me.BsComponentSeparatorTextBox.Location = New System.Drawing.Point(18, 82)
        Me.BsComponentSeparatorTextBox.Mandatory = True
        Me.BsComponentSeparatorTextBox.MaxLength = 10
        Me.BsComponentSeparatorTextBox.Name = "BsComponentSeparatorTextBox"
        Me.BsComponentSeparatorTextBox.Size = New System.Drawing.Size(51, 21)
        Me.BsComponentSeparatorTextBox.TabIndex = 12
        Me.BsComponentSeparatorTextBox.WordWrap = False
        '
        'BsSubcomponentLabel
        '
        Me.BsSubcomponentLabel.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BsSubcomponentLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsSubcomponentLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsSubcomponentLabel.ForeColor = System.Drawing.Color.Black
        Me.BsSubcomponentLabel.Location = New System.Drawing.Point(15, 196)
        Me.BsSubcomponentLabel.Name = "BsSubcomponentLabel"
        Me.BsSubcomponentLabel.Size = New System.Drawing.Size(280, 15)
        Me.BsSubcomponentLabel.TabIndex = 31
        Me.BsSubcomponentLabel.Text = "*Subcomponent delimiter:"
        Me.BsSubcomponentLabel.Title = False
        '
        'BsSpecialLabel
        '
        Me.BsSpecialLabel.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BsSpecialLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsSpecialLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsSpecialLabel.ForeColor = System.Drawing.Color.Black
        Me.BsSpecialLabel.Location = New System.Drawing.Point(15, 151)
        Me.BsSpecialLabel.Name = "BsSpecialLabel"
        Me.BsSpecialLabel.Size = New System.Drawing.Size(280, 15)
        Me.BsSpecialLabel.TabIndex = 29
        Me.BsSpecialLabel.Text = "*Special delimiter:"
        Me.BsSpecialLabel.Title = False
        '
        'BsRepeatLabel
        '
        Me.BsRepeatLabel.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BsRepeatLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsRepeatLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsRepeatLabel.ForeColor = System.Drawing.Color.Black
        Me.BsRepeatLabel.Location = New System.Drawing.Point(15, 108)
        Me.BsRepeatLabel.Name = "BsRepeatLabel"
        Me.BsRepeatLabel.Size = New System.Drawing.Size(280, 16)
        Me.BsRepeatLabel.TabIndex = 27
        Me.BsRepeatLabel.Text = "*Repeat delimiter:"
        Me.BsRepeatLabel.Title = False
        '
        'BsComponentLabel
        '
        Me.BsComponentLabel.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BsComponentLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsComponentLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsComponentLabel.ForeColor = System.Drawing.Color.Black
        Me.BsComponentLabel.Location = New System.Drawing.Point(15, 63)
        Me.BsComponentLabel.Name = "BsComponentLabel"
        Me.BsComponentLabel.Size = New System.Drawing.Size(280, 17)
        Me.BsComponentLabel.TabIndex = 25
        Me.BsComponentLabel.Text = "*Component delimiter:"
        Me.BsComponentLabel.Title = False
        '
        'BsFieldLabel
        '
        Me.BsFieldLabel.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BsFieldLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsFieldLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsFieldLabel.ForeColor = System.Drawing.Color.Black
        Me.BsFieldLabel.Location = New System.Drawing.Point(15, 21)
        Me.BsFieldLabel.Name = "BsFieldLabel"
        Me.BsFieldLabel.Size = New System.Drawing.Size(264, 15)
        Me.BsFieldLabel.TabIndex = 23
        Me.BsFieldLabel.Text = "*Field delimiter:"
        Me.BsFieldLabel.Title = False
        '
        'BsmaxTimeWaitACKNumericUpDown
        '
        Me.BsmaxTimeWaitACKNumericUpDown.Location = New System.Drawing.Point(621, 153)
        Me.BsmaxTimeWaitACKNumericUpDown.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.BsmaxTimeWaitACKNumericUpDown.Name = "BsmaxTimeWaitACKNumericUpDown"
        Me.BsmaxTimeWaitACKNumericUpDown.Size = New System.Drawing.Size(53, 21)
        Me.BsmaxTimeWaitACKNumericUpDown.TabIndex = 10
        Me.BsmaxTimeWaitACKNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsmaxTimeWaitACKNumericUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsmaxTimeWaitACKLabel
        '
        Me.BsmaxTimeWaitACKLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsmaxTimeWaitACKLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsmaxTimeWaitACKLabel.ForeColor = System.Drawing.Color.Black
        Me.BsmaxTimeWaitACKLabel.Location = New System.Drawing.Point(285, 155)
        Me.BsmaxTimeWaitACKLabel.Name = "BsmaxTimeWaitACKLabel"
        Me.BsmaxTimeWaitACKLabel.Size = New System.Drawing.Size(330, 22)
        Me.BsmaxTimeWaitACKLabel.TabIndex = 88
        Me.BsmaxTimeWaitACKLabel.Text = "*Max Time for ACK from LIS:"
        Me.BsmaxTimeWaitACKLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.BsmaxTimeWaitACKLabel.Title = False
        '
        'BsInstrumentProviderLabel
        '
        Me.BsInstrumentProviderLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsInstrumentProviderLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsInstrumentProviderLabel.ForeColor = System.Drawing.Color.Black
        Me.BsInstrumentProviderLabel.Location = New System.Drawing.Point(23, 277)
        Me.BsInstrumentProviderLabel.Name = "BsInstrumentProviderLabel"
        Me.BsInstrumentProviderLabel.Size = New System.Drawing.Size(228, 13)
        Me.BsInstrumentProviderLabel.TabIndex = 86
        Me.BsInstrumentProviderLabel.Text = "*Instrument provider:"
        Me.BsInstrumentProviderLabel.Title = False
        '
        'BsInstrumentIDLabel
        '
        Me.BsInstrumentIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsInstrumentIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsInstrumentIDLabel.ForeColor = System.Drawing.Color.Black
        Me.BsInstrumentIDLabel.Location = New System.Drawing.Point(23, 228)
        Me.BsInstrumentIDLabel.Name = "BsInstrumentIDLabel"
        Me.BsInstrumentIDLabel.Size = New System.Drawing.Size(228, 13)
        Me.BsInstrumentIDLabel.TabIndex = 84
        Me.BsInstrumentIDLabel.Text = "*Instrument ID:"
        Me.BsInstrumentIDLabel.Title = False
        '
        'BsHostProviderLabel
        '
        Me.BsHostProviderLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsHostProviderLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsHostProviderLabel.ForeColor = System.Drawing.Color.Black
        Me.BsHostProviderLabel.Location = New System.Drawing.Point(23, 177)
        Me.BsHostProviderLabel.Name = "BsHostProviderLabel"
        Me.BsHostProviderLabel.Size = New System.Drawing.Size(231, 13)
        Me.BsHostProviderLabel.TabIndex = 82
        Me.BsHostProviderLabel.Text = "*Host provider:"
        Me.BsHostProviderLabel.Title = False
        '
        'BsHostIDLabel
        '
        Me.BsHostIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsHostIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsHostIDLabel.ForeColor = System.Drawing.Color.Black
        Me.BsHostIDLabel.Location = New System.Drawing.Point(23, 129)
        Me.BsHostIDLabel.Name = "BsHostIDLabel"
        Me.BsHostIDLabel.Size = New System.Drawing.Size(231, 13)
        Me.BsHostIDLabel.TabIndex = 80
        Me.BsHostIDLabel.Text = "*Host ID:"
        Me.BsHostIDLabel.Title = False
        '
        'BsmaxTimeWaitingForResponseNumericUpDown
        '
        Me.BsmaxTimeWaitingForResponseNumericUpDown.Location = New System.Drawing.Point(621, 114)
        Me.BsmaxTimeWaitingForResponseNumericUpDown.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.BsmaxTimeWaitingForResponseNumericUpDown.Name = "BsmaxTimeWaitingForResponseNumericUpDown"
        Me.BsmaxTimeWaitingForResponseNumericUpDown.Size = New System.Drawing.Size(53, 21)
        Me.BsmaxTimeWaitingForResponseNumericUpDown.TabIndex = 9
        Me.BsmaxTimeWaitingForResponseNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsmaxTimeWaitingForResponseNumericUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsmaxTimeWaitingForResponseLabel
        '
        Me.BsmaxTimeWaitingForResponseLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsmaxTimeWaitingForResponseLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsmaxTimeWaitingForResponseLabel.ForeColor = System.Drawing.Color.Black
        Me.BsmaxTimeWaitingForResponseLabel.Location = New System.Drawing.Point(263, 116)
        Me.BsmaxTimeWaitingForResponseLabel.Name = "BsmaxTimeWaitingForResponseLabel"
        Me.BsmaxTimeWaitingForResponseLabel.Size = New System.Drawing.Size(352, 19)
        Me.BsmaxTimeWaitingForResponseLabel.TabIndex = 78
        Me.BsmaxTimeWaitingForResponseLabel.Text = "*Max Time for Response from LIS:"
        Me.BsmaxTimeWaitingForResponseLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.BsmaxTimeWaitingForResponseLabel.Title = False
        '
        'BsHostQueryPackageLabel
        '
        Me.BsHostQueryPackageLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsHostQueryPackageLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsHostQueryPackageLabel.ForeColor = System.Drawing.Color.Black
        Me.BsHostQueryPackageLabel.Location = New System.Drawing.Point(299, 80)
        Me.BsHostQueryPackageLabel.Name = "BsHostQueryPackageLabel"
        Me.BsHostQueryPackageLabel.Size = New System.Drawing.Size(316, 19)
        Me.BsHostQueryPackageLabel.TabIndex = 58
        Me.BsHostQueryPackageLabel.Text = "*Host Query Package size:"
        Me.BsHostQueryPackageLabel.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.BsHostQueryPackageLabel.Title = False
        '
        'bsIHECompliantCheckbox
        '
        Me.bsIHECompliantCheckbox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.bsIHECompliantCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsIHECompliantCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsIHECompliantCheckbox.Location = New System.Drawing.Point(325, 38)
        Me.bsIHECompliantCheckbox.Name = "bsIHECompliantCheckbox"
        Me.bsIHECompliantCheckbox.Size = New System.Drawing.Size(346, 17)
        Me.bsIHECompliantCheckbox.TabIndex = 7
        Me.bsIHECompliantCheckbox.Text = "*IHE compliant"
        Me.bsIHECompliantCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.bsIHECompliantCheckbox.UseVisualStyleBackColor = True
        '
        'BsHostQueryPackageNumericUpDown
        '
        Me.BsHostQueryPackageNumericUpDown.Location = New System.Drawing.Point(621, 78)
        Me.BsHostQueryPackageNumericUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.BsHostQueryPackageNumericUpDown.Name = "BsHostQueryPackageNumericUpDown"
        Me.BsHostQueryPackageNumericUpDown.Size = New System.Drawing.Size(53, 21)
        Me.BsHostQueryPackageNumericUpDown.TabIndex = 8
        Me.BsHostQueryPackageNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsHostQueryPackageNumericUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsTransmissionCodePageLabel
        '
        Me.BsTransmissionCodePageLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsTransmissionCodePageLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsTransmissionCodePageLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTransmissionCodePageLabel.Location = New System.Drawing.Point(20, 72)
        Me.BsTransmissionCodePageLabel.Name = "BsTransmissionCodePageLabel"
        Me.BsTransmissionCodePageLabel.Size = New System.Drawing.Size(231, 13)
        Me.BsTransmissionCodePageLabel.TabIndex = 23
        Me.BsTransmissionCodePageLabel.Text = "*Transmission Code Page:"
        Me.BsTransmissionCodePageLabel.Title = False
        '
        'bsProtocolNameLabel
        '
        Me.bsProtocolNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsProtocolNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsProtocolNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsProtocolNameLabel.Location = New System.Drawing.Point(20, 20)
        Me.bsProtocolNameLabel.Name = "bsProtocolNameLabel"
        Me.bsProtocolNameLabel.Size = New System.Drawing.Size(163, 13)
        Me.bsProtocolNameLabel.TabIndex = 21
        Me.bsProtocolNameLabel.Text = "*Protocol name:"
        Me.bsProtocolNameLabel.Title = False
        '
        'bsProtocolNameComboBox
        '
        Me.bsProtocolNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsProtocolNameComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsProtocolNameComboBox.FormattingEnabled = True
        Me.bsProtocolNameComboBox.Location = New System.Drawing.Point(20, 38)
        Me.bsProtocolNameComboBox.Name = "bsProtocolNameComboBox"
        Me.bsProtocolNameComboBox.Size = New System.Drawing.Size(231, 21)
        Me.bsProtocolNameComboBox.TabIndex = 1
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.Location = New System.Drawing.Point(669, 567)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 20
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'IConfigLIS
        '
        Me.AcceptButton = Me.bsCancelButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(750, 611)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsAcceptButton)
        Me.Controls.Add(Me.bsLISConfigGroupBox)
        Me.Controls.Add(Me.bsCancelButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiConfigLIS"
        Me.ShowInTaskbar = False
        Me.Text = ""
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsLISConfigGroupBox.ResumeLayout(False)
        Me.bsLISConfigTabControl.ResumeLayout(False)
        Me.SessionSettingsTab.ResumeLayout(False)
        Me.BsUploadGroupBox.ResumeLayout(False)
        Me.BsUploadGroupBox.PerformLayout()
        Me.BsDownloadGroupBox.ResumeLayout(False)
        Me.BsDownloadGroupBox.PerformLayout()
        CType(Me.BsBsMaxTimeToWaitLISOrdersNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CommunicationsSettingsTab.ResumeLayout(False)
        Me.CommunicationsSettingsTab.PerformLayout()
        Me.bsInternalGroupBox.ResumeLayout(False)
        Me.bsInternalGroupBox.PerformLayout()
        CType(Me.BsMaxTimeToRespondNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BsMaxTransmissionMsgsNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BsMaxReceptionMsgsNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsLISCommsGroupBox.ResumeLayout(False)
        Me.bsLISCommsGroupBox.PerformLayout()
        CType(Me.TCPPort2NumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TCPPortNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ProtocolSettingsTab.ResumeLayout(False)
        Me.bsLISProtocolGroupBox.ResumeLayout(False)
        Me.bsLISProtocolGroupBox.PerformLayout()
        Me.BsDelimitersGroupBox.ResumeLayout(False)
        Me.BsDelimitersGroupBox.PerformLayout()
        CType(Me.BsmaxTimeWaitACKNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BsmaxTimeWaitingForResponseNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BsHostQueryPackageNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsLISConfigGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsLISConfigurationLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLISConfigTabControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents SessionSettingsTab As System.Windows.Forms.TabPage
    Friend WithEvents CommunicationsSettingsTab As System.Windows.Forms.TabPage
    Friend WithEvents bsLISCommsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents ProtocolSettingsTab As System.Windows.Forms.TabPage
    Friend WithEvents BsDownloadGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsAcceptOnRunningCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsAcceptUnsolicitedOrdersCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsHostQueryCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsUploadGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsUploadResultsOnResetCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsUploadUnsolicitedQCResultsCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsUploadUnsolicitedPatientResultsCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsFreqComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsFrequencyLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents HostNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents TCPPortLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents TCPPortNumericUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsWSReflexTestsComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsWSReflexTestsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsWSRerunsComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsWSRerunsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsInternalGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsMaxReceptionMsgsNumericUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsMaxReceptionMsgsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsMaxTransmissionMsgsNumericUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsMaxTransmissionMsgsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsAutomaticCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsLISCommsEnabledCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsLISProtocolGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsHostQueryPackageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsHostQueryPackageNumericUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsTransmissionCodePageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsProtocolNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsProtocolNameComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsTimeoutQueueLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsMaxTimeToRespondNumericUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsInstrumentProviderLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsInstrumentIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsHostProviderLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsHostIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsmaxTimeWaitingForResponseNumericUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsmaxTimeWaitingForResponseLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsIHECompliantCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsmaxTimeWaitACKNumericUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsmaxTimeWaitACKLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsDelimitersGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsSubcomponentLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsSpecialLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsRepeatLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsComponentLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsFieldLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents TCPPort2NumericUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents TCPPort2Label As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsDataTransmissionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsDataTransmissionComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bwPreload As System.ComponentModel.BackgroundWorker
    Friend WithEvents HostNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsInstrumentProviderTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsHostProviderTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsHostIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsTransmissionCodePageTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsSubComponentSeparatorTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsSpecialSeparatorTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsRepeatSeparatorTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsFieldSeparatorTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsComponentSeparatorTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsInstrumentIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsMaxTimeToWaitLISOrdersLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsBsMaxTimeToWaitLISOrdersNumericUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents BsAutoQueryStartCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
End Class
