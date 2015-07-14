

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiConfigGeneral
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiConfigGeneral))
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsAnalyzerConfigGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsAnalyzerConfigurationLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsAnalyzersConfigTabControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl()
        Me.SessionSettingsTab = New System.Windows.Forms.TabPage()
        Me.BsAutomaticReportGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsAutoReportFreqLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsAutoReportTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsAutomaticReportFrequency = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsAutomaticReportType = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsSampleTubeTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.SampleTubeTypeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsResetSamplesRotorCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsBarCodeStartCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsAutResultPrintCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsAutomaticRerunCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.AnalyzerTab = New System.Windows.Forms.TabPage()
        Me.bsDisabledElementsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsClotDetectionCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsReagentsRotorCvrCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsSamplesRotorCvrCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsReactionRotorCvrCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsGralAnalyzerCvrCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsAlarmSoundDisabledCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsWaterEntranceGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsLabWaterCircuitRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.bsExtWaterTankRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.CommunicationSettingsTab = New System.Windows.Forms.TabPage()
        Me.BsCommsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsAutomaticRadioButtonC = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.bsPortLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsManualRadioButtonC = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.bsSpeedLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsPortComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsSpeedComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.BsTestCommunicationsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.LIMSTab = New System.Windows.Forms.TabPage()
        Me.bsAutomaticSessionCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsOnlineExportGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsFreqComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsFrequencyLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsManualRadioButtonS = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.bsAutomaticRadioButtonS = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.bsOnLineExportCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsBorderedPanel1 = New bsBorderedPanel()
        Me.bsAnalyzerConfigGroupBox.SuspendLayout()
        Me.bsAnalyzersConfigTabControl.SuspendLayout()
        Me.SessionSettingsTab.SuspendLayout()
        Me.BsAutomaticReportGroupBox.SuspendLayout()
        Me.AnalyzerTab.SuspendLayout()
        Me.bsDisabledElementsGroupBox.SuspendLayout()
        Me.bsWaterEntranceGroupBox.SuspendLayout()
        Me.CommunicationSettingsTab.SuspendLayout()
        Me.BsCommsGroupBox.SuspendLayout()
        Me.LIMSTab.SuspendLayout()
        Me.bsOnlineExportGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsCancelButton
        '
        Me.bsCancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Location = New System.Drawing.Point(706, 407)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 13
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsAnalyzerConfigGroupBox
        '
        Me.bsAnalyzerConfigGroupBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsAnalyzerConfigGroupBox.Controls.Add(Me.bsAnalyzerConfigurationLabel)
        Me.bsAnalyzerConfigGroupBox.Controls.Add(Me.bsAnalyzersConfigTabControl)
        Me.bsAnalyzerConfigGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsAnalyzerConfigGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsAnalyzerConfigGroupBox.Name = "bsAnalyzerConfigGroupBox"
        Me.bsAnalyzerConfigGroupBox.Size = New System.Drawing.Size(728, 387)
        Me.bsAnalyzerConfigGroupBox.TabIndex = 21
        Me.bsAnalyzerConfigGroupBox.TabStop = False
        '
        'bsAnalyzerConfigurationLabel
        '
        Me.bsAnalyzerConfigurationLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsAnalyzerConfigurationLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsAnalyzerConfigurationLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsAnalyzerConfigurationLabel.ForeColor = System.Drawing.Color.Black
        Me.bsAnalyzerConfigurationLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsAnalyzerConfigurationLabel.Name = "bsAnalyzerConfigurationLabel"
        Me.bsAnalyzerConfigurationLabel.Size = New System.Drawing.Size(705, 20)
        Me.bsAnalyzerConfigurationLabel.TabIndex = 20
        Me.bsAnalyzerConfigurationLabel.Text = "Analyzer Configuration"
        Me.bsAnalyzerConfigurationLabel.Title = True
        '
        'bsAnalyzersConfigTabControl
        '
        Me.bsAnalyzersConfigTabControl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsAnalyzersConfigTabControl.Controls.Add(Me.SessionSettingsTab)
        Me.bsAnalyzersConfigTabControl.Controls.Add(Me.AnalyzerTab)
        Me.bsAnalyzersConfigTabControl.Controls.Add(Me.CommunicationSettingsTab)
        Me.bsAnalyzersConfigTabControl.Controls.Add(Me.LIMSTab)
        Me.bsAnalyzersConfigTabControl.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAnalyzersConfigTabControl.Location = New System.Drawing.Point(10, 45)
        Me.bsAnalyzersConfigTabControl.Name = "bsAnalyzersConfigTabControl"
        Me.bsAnalyzersConfigTabControl.SelectedIndex = 0
        Me.bsAnalyzersConfigTabControl.Size = New System.Drawing.Size(705, 332)
        Me.bsAnalyzersConfigTabControl.TabIndex = 0
        '
        'SessionSettingsTab
        '
        Me.SessionSettingsTab.BackColor = System.Drawing.Color.Gainsboro
        Me.SessionSettingsTab.Controls.Add(Me.BsAutomaticReportGroupBox)
        Me.SessionSettingsTab.Controls.Add(Me.bsSampleTubeTypeLabel)
        Me.SessionSettingsTab.Controls.Add(Me.SampleTubeTypeComboBox)
        Me.SessionSettingsTab.Controls.Add(Me.bsResetSamplesRotorCheckBox)
        Me.SessionSettingsTab.Controls.Add(Me.bsBarCodeStartCheckBox)
        Me.SessionSettingsTab.Controls.Add(Me.bsAutResultPrintCheckBox)
        Me.SessionSettingsTab.Controls.Add(Me.bsAutomaticRerunCheckBox)
        Me.SessionSettingsTab.Location = New System.Drawing.Point(4, 22)
        Me.SessionSettingsTab.Name = "SessionSettingsTab"
        Me.SessionSettingsTab.Padding = New System.Windows.Forms.Padding(3)
        Me.SessionSettingsTab.Size = New System.Drawing.Size(697, 306)
        Me.SessionSettingsTab.TabIndex = 0
        Me.SessionSettingsTab.Text = "WorkSession"
        '
        'BsAutomaticReportGroupBox
        '
        Me.BsAutomaticReportGroupBox.Controls.Add(Me.bsAutoReportFreqLabel)
        Me.BsAutomaticReportGroupBox.Controls.Add(Me.bsAutoReportTypeLabel)
        Me.BsAutomaticReportGroupBox.Controls.Add(Me.bsAutomaticReportFrequency)
        Me.BsAutomaticReportGroupBox.Controls.Add(Me.bsAutomaticReportType)
        Me.BsAutomaticReportGroupBox.ForeColor = System.Drawing.Color.Black
        Me.BsAutomaticReportGroupBox.Location = New System.Drawing.Point(37, 203)
        Me.BsAutomaticReportGroupBox.Name = "BsAutomaticReportGroupBox"
        Me.BsAutomaticReportGroupBox.Size = New System.Drawing.Size(563, 78)
        Me.BsAutomaticReportGroupBox.TabIndex = 19
        Me.BsAutomaticReportGroupBox.TabStop = False
        '
        'bsAutoReportFreqLabel
        '
        Me.bsAutoReportFreqLabel.AutoSize = True
        Me.bsAutoReportFreqLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsAutoReportFreqLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsAutoReportFreqLabel.ForeColor = System.Drawing.Color.Black
        Me.bsAutoReportFreqLabel.Location = New System.Drawing.Point(15, 47)
        Me.bsAutoReportFreqLabel.Name = "bsAutoReportFreqLabel"
        Me.bsAutoReportFreqLabel.Size = New System.Drawing.Size(145, 13)
        Me.bsAutoReportFreqLabel.TabIndex = 3
        Me.bsAutoReportFreqLabel.Text = "*Auto Report Frequency"
        Me.bsAutoReportFreqLabel.Title = False
        '
        'bsAutoReportTypeLabel
        '
        Me.bsAutoReportTypeLabel.AutoSize = True
        Me.bsAutoReportTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsAutoReportTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsAutoReportTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsAutoReportTypeLabel.Location = New System.Drawing.Point(15, 23)
        Me.bsAutoReportTypeLabel.Name = "bsAutoReportTypeLabel"
        Me.bsAutoReportTypeLabel.Size = New System.Drawing.Size(111, 13)
        Me.bsAutoReportTypeLabel.TabIndex = 2
        Me.bsAutoReportTypeLabel.Text = "*Auto Report type"
        Me.bsAutoReportTypeLabel.Title = False
        '
        'bsAutomaticReportFrequency
        '
        Me.bsAutomaticReportFrequency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsAutomaticReportFrequency.Enabled = False
        Me.bsAutomaticReportFrequency.FormattingEnabled = True
        Me.bsAutomaticReportFrequency.Location = New System.Drawing.Point(189, 47)
        Me.bsAutomaticReportFrequency.Name = "bsAutomaticReportFrequency"
        Me.bsAutomaticReportFrequency.Size = New System.Drawing.Size(359, 21)
        Me.bsAutomaticReportFrequency.TabIndex = 1
        '
        'bsAutomaticReportType
        '
        Me.bsAutomaticReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsAutomaticReportType.Enabled = False
        Me.bsAutomaticReportType.FormattingEnabled = True
        Me.bsAutomaticReportType.Location = New System.Drawing.Point(189, 20)
        Me.bsAutomaticReportType.Name = "bsAutomaticReportType"
        Me.bsAutomaticReportType.Size = New System.Drawing.Size(359, 21)
        Me.bsAutomaticReportType.TabIndex = 0
        '
        'bsSampleTubeTypeLabel
        '
        Me.bsSampleTubeTypeLabel.AutoSize = True
        Me.bsSampleTubeTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleTubeTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTubeTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTubeTypeLabel.Location = New System.Drawing.Point(17, 27)
        Me.bsSampleTubeTypeLabel.Name = "bsSampleTubeTypeLabel"
        Me.bsSampleTubeTypeLabel.Size = New System.Drawing.Size(166, 13)
        Me.bsSampleTubeTypeLabel.TabIndex = 18
        Me.bsSampleTubeTypeLabel.Text = "*Default Sample Tube Type"
        Me.bsSampleTubeTypeLabel.Title = False
        '
        'SampleTubeTypeComboBox
        '
        Me.SampleTubeTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.SampleTubeTypeComboBox.ForeColor = System.Drawing.Color.Black
        Me.SampleTubeTypeComboBox.FormattingEnabled = True
        Me.SampleTubeTypeComboBox.Location = New System.Drawing.Point(20, 45)
        Me.SampleTubeTypeComboBox.Name = "SampleTubeTypeComboBox"
        Me.SampleTubeTypeComboBox.Size = New System.Drawing.Size(182, 21)
        Me.SampleTubeTypeComboBox.TabIndex = 10
        '
        'bsResetSamplesRotorCheckBox
        '
        Me.bsResetSamplesRotorCheckBox.AutoSize = True
        Me.bsResetSamplesRotorCheckBox.BackColor = System.Drawing.Color.Gainsboro
        Me.bsResetSamplesRotorCheckBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsResetSamplesRotorCheckBox.ForeColor = System.Drawing.Color.Black
        Me.bsResetSamplesRotorCheckBox.Location = New System.Drawing.Point(20, 128)
        Me.bsResetSamplesRotorCheckBox.Name = "bsResetSamplesRotorCheckBox"
        Me.bsResetSamplesRotorCheckBox.Size = New System.Drawing.Size(415, 17)
        Me.bsResetSamplesRotorCheckBox.TabIndex = 11
        Me.bsResetSamplesRotorCheckBox.Text = "Reset worksession download only patient tubes from Samples Rotor"
        Me.bsResetSamplesRotorCheckBox.UseVisualStyleBackColor = False
        '
        'bsBarCodeStartCheckBox
        '
        Me.bsBarCodeStartCheckBox.AutoSize = True
        Me.bsBarCodeStartCheckBox.BackColor = System.Drawing.Color.Gainsboro
        Me.bsBarCodeStartCheckBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsBarCodeStartCheckBox.ForeColor = System.Drawing.Color.Black
        Me.bsBarCodeStartCheckBox.Location = New System.Drawing.Point(20, 98)
        Me.bsBarCodeStartCheckBox.Name = "bsBarCodeStartCheckBox"
        Me.bsBarCodeStartCheckBox.Size = New System.Drawing.Size(299, 17)
        Me.bsBarCodeStartCheckBox.TabIndex = 10
        Me.bsBarCodeStartCheckBox.Text = "Checking BarCode before starting WorkSession"
        Me.bsBarCodeStartCheckBox.UseVisualStyleBackColor = False
        '
        'bsAutResultPrintCheckBox
        '
        Me.bsAutResultPrintCheckBox.AutoSize = True
        Me.bsAutResultPrintCheckBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAutResultPrintCheckBox.ForeColor = System.Drawing.Color.Black
        Me.bsAutResultPrintCheckBox.Location = New System.Drawing.Point(20, 188)
        Me.bsAutResultPrintCheckBox.Name = "bsAutResultPrintCheckBox"
        Me.bsAutResultPrintCheckBox.Size = New System.Drawing.Size(233, 17)
        Me.bsAutResultPrintCheckBox.TabIndex = 6
        Me.bsAutResultPrintCheckBox.Text = "Automatic Printing of Patient Results"
        Me.bsAutResultPrintCheckBox.UseVisualStyleBackColor = True
        '
        'bsAutomaticRerunCheckBox
        '
        Me.bsAutomaticRerunCheckBox.AutoSize = True
        Me.bsAutomaticRerunCheckBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAutomaticRerunCheckBox.ForeColor = System.Drawing.Color.Black
        Me.bsAutomaticRerunCheckBox.Location = New System.Drawing.Point(20, 158)
        Me.bsAutomaticRerunCheckBox.Name = "bsAutomaticRerunCheckBox"
        Me.bsAutomaticRerunCheckBox.Size = New System.Drawing.Size(230, 17)
        Me.bsAutomaticRerunCheckBox.TabIndex = 5
        Me.bsAutomaticRerunCheckBox.Text = "Automatic Processing of Repetitions"
        Me.bsAutomaticRerunCheckBox.UseVisualStyleBackColor = True
        '
        'AnalyzerTab
        '
        Me.AnalyzerTab.BackColor = System.Drawing.Color.Gainsboro
        Me.AnalyzerTab.Controls.Add(Me.bsDisabledElementsGroupBox)
        Me.AnalyzerTab.Controls.Add(Me.bsAlarmSoundDisabledCheckbox)
        Me.AnalyzerTab.Controls.Add(Me.bsWaterEntranceGroupBox)
        Me.AnalyzerTab.Location = New System.Drawing.Point(4, 22)
        Me.AnalyzerTab.Name = "AnalyzerTab"
        Me.AnalyzerTab.Size = New System.Drawing.Size(697, 306)
        Me.AnalyzerTab.TabIndex = 3
        Me.AnalyzerTab.Text = "Analyzer"
        '
        'bsDisabledElementsGroupBox
        '
        Me.bsDisabledElementsGroupBox.Controls.Add(Me.bsClotDetectionCheckbox)
        Me.bsDisabledElementsGroupBox.Controls.Add(Me.bsReagentsRotorCvrCheckbox)
        Me.bsDisabledElementsGroupBox.Controls.Add(Me.bsSamplesRotorCvrCheckbox)
        Me.bsDisabledElementsGroupBox.Controls.Add(Me.bsReactionRotorCvrCheckbox)
        Me.bsDisabledElementsGroupBox.Controls.Add(Me.bsGralAnalyzerCvrCheckbox)
        Me.bsDisabledElementsGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDisabledElementsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsDisabledElementsGroupBox.Location = New System.Drawing.Point(20, 121)
        Me.bsDisabledElementsGroupBox.Name = "bsDisabledElementsGroupBox"
        Me.bsDisabledElementsGroupBox.Size = New System.Drawing.Size(276, 167)
        Me.bsDisabledElementsGroupBox.TabIndex = 11
        Me.bsDisabledElementsGroupBox.TabStop = False
        Me.bsDisabledElementsGroupBox.Text = "  *Disabled elements"
        '
        'bsClotDetectionCheckbox
        '
        Me.bsClotDetectionCheckbox.AutoSize = True
        Me.bsClotDetectionCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.bsClotDetectionCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsClotDetectionCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsClotDetectionCheckbox.Location = New System.Drawing.Point(20, 58)
        Me.bsClotDetectionCheckbox.Name = "bsClotDetectionCheckbox"
        Me.bsClotDetectionCheckbox.Size = New System.Drawing.Size(112, 17)
        Me.bsClotDetectionCheckbox.TabIndex = 15
        Me.bsClotDetectionCheckbox.Text = "*Clot detection"
        Me.bsClotDetectionCheckbox.UseVisualStyleBackColor = False
        '
        'bsReagentsRotorCvrCheckbox
        '
        Me.bsReagentsRotorCvrCheckbox.AutoSize = True
        Me.bsReagentsRotorCvrCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.bsReagentsRotorCvrCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReagentsRotorCvrCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsReagentsRotorCvrCheckbox.Location = New System.Drawing.Point(20, 114)
        Me.bsReagentsRotorCvrCheckbox.Name = "bsReagentsRotorCvrCheckbox"
        Me.bsReagentsRotorCvrCheckbox.Size = New System.Drawing.Size(160, 17)
        Me.bsReagentsRotorCvrCheckbox.TabIndex = 14
        Me.bsReagentsRotorCvrCheckbox.Text = "*Reagents Rotor Cover"
        Me.bsReagentsRotorCvrCheckbox.UseMnemonic = False
        Me.bsReagentsRotorCvrCheckbox.UseVisualStyleBackColor = False
        '
        'bsSamplesRotorCvrCheckbox
        '
        Me.bsSamplesRotorCvrCheckbox.AutoSize = True
        Me.bsSamplesRotorCvrCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.bsSamplesRotorCvrCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSamplesRotorCvrCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsSamplesRotorCvrCheckbox.Location = New System.Drawing.Point(20, 142)
        Me.bsSamplesRotorCvrCheckbox.Name = "bsSamplesRotorCvrCheckbox"
        Me.bsSamplesRotorCvrCheckbox.Size = New System.Drawing.Size(156, 17)
        Me.bsSamplesRotorCvrCheckbox.TabIndex = 13
        Me.bsSamplesRotorCvrCheckbox.Text = "*Samples Rotor Cover"
        Me.bsSamplesRotorCvrCheckbox.UseVisualStyleBackColor = False
        '
        'bsReactionRotorCvrCheckbox
        '
        Me.bsReactionRotorCvrCheckbox.AutoSize = True
        Me.bsReactionRotorCvrCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.bsReactionRotorCvrCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReactionRotorCvrCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsReactionRotorCvrCheckbox.Location = New System.Drawing.Point(20, 86)
        Me.bsReactionRotorCvrCheckbox.Name = "bsReactionRotorCvrCheckbox"
        Me.bsReactionRotorCvrCheckbox.Size = New System.Drawing.Size(156, 17)
        Me.bsReactionRotorCvrCheckbox.TabIndex = 12
        Me.bsReactionRotorCvrCheckbox.Text = "*Reaction Rotor Cover"
        Me.bsReactionRotorCvrCheckbox.UseVisualStyleBackColor = False
        '
        'bsGralAnalyzerCvrCheckbox
        '
        Me.bsGralAnalyzerCvrCheckbox.AutoSize = True
        Me.bsGralAnalyzerCvrCheckbox.BackColor = System.Drawing.Color.Gainsboro
        Me.bsGralAnalyzerCvrCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsGralAnalyzerCvrCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsGralAnalyzerCvrCheckbox.Location = New System.Drawing.Point(20, 30)
        Me.bsGralAnalyzerCvrCheckbox.Name = "bsGralAnalyzerCvrCheckbox"
        Me.bsGralAnalyzerCvrCheckbox.Size = New System.Drawing.Size(171, 17)
        Me.bsGralAnalyzerCvrCheckbox.TabIndex = 11
        Me.bsGralAnalyzerCvrCheckbox.Text = "*General Analyzer Cover"
        Me.bsGralAnalyzerCvrCheckbox.UseVisualStyleBackColor = False
        '
        'bsAlarmSoundDisabledCheckbox
        '
        Me.bsAlarmSoundDisabledCheckbox.AutoSize = True
        Me.bsAlarmSoundDisabledCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAlarmSoundDisabledCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsAlarmSoundDisabledCheckbox.Location = New System.Drawing.Point(313, 32)
        Me.bsAlarmSoundDisabledCheckbox.Name = "bsAlarmSoundDisabledCheckbox"
        Me.bsAlarmSoundDisabledCheckbox.Size = New System.Drawing.Size(160, 17)
        Me.bsAlarmSoundDisabledCheckbox.TabIndex = 10
        Me.bsAlarmSoundDisabledCheckbox.Text = "*Alarm Sound Disabled"
        Me.bsAlarmSoundDisabledCheckbox.UseVisualStyleBackColor = True
        '
        'bsWaterEntranceGroupBox
        '
        Me.bsWaterEntranceGroupBox.Controls.Add(Me.bsLabWaterCircuitRadioButton)
        Me.bsWaterEntranceGroupBox.Controls.Add(Me.bsExtWaterTankRadioButton)
        Me.bsWaterEntranceGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsWaterEntranceGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsWaterEntranceGroupBox.Location = New System.Drawing.Point(20, 20)
        Me.bsWaterEntranceGroupBox.Name = "bsWaterEntranceGroupBox"
        Me.bsWaterEntranceGroupBox.Size = New System.Drawing.Size(276, 86)
        Me.bsWaterEntranceGroupBox.TabIndex = 9
        Me.bsWaterEntranceGroupBox.TabStop = False
        Me.bsWaterEntranceGroupBox.Text = "*Water entrance Selection"
        '
        'bsLabWaterCircuitRadioButton
        '
        Me.bsLabWaterCircuitRadioButton.AutoSize = True
        Me.bsLabWaterCircuitRadioButton.Checked = True
        Me.bsLabWaterCircuitRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLabWaterCircuitRadioButton.Location = New System.Drawing.Point(20, 54)
        Me.bsLabWaterCircuitRadioButton.Name = "bsLabWaterCircuitRadioButton"
        Me.bsLabWaterCircuitRadioButton.Size = New System.Drawing.Size(122, 17)
        Me.bsLabWaterCircuitRadioButton.TabIndex = 11
        Me.bsLabWaterCircuitRadioButton.TabStop = True
        Me.bsLabWaterCircuitRadioButton.Text = "*Main water inlet"
        Me.bsLabWaterCircuitRadioButton.UseVisualStyleBackColor = True
        '
        'bsExtWaterTankRadioButton
        '
        Me.bsExtWaterTankRadioButton.AutoSize = True
        Me.bsExtWaterTankRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExtWaterTankRadioButton.Location = New System.Drawing.Point(20, 27)
        Me.bsExtWaterTankRadioButton.Name = "bsExtWaterTankRadioButton"
        Me.bsExtWaterTankRadioButton.Size = New System.Drawing.Size(123, 17)
        Me.bsExtWaterTankRadioButton.TabIndex = 9
        Me.bsExtWaterTankRadioButton.TabStop = True
        Me.bsExtWaterTankRadioButton.Text = "*Water tank inlet"
        Me.bsExtWaterTankRadioButton.UseVisualStyleBackColor = True
        '
        'CommunicationSettingsTab
        '
        Me.CommunicationSettingsTab.BackColor = System.Drawing.Color.Gainsboro
        Me.CommunicationSettingsTab.Controls.Add(Me.BsCommsGroupBox)
        Me.CommunicationSettingsTab.Controls.Add(Me.BsTestCommunicationsButton)
        Me.CommunicationSettingsTab.ForeColor = System.Drawing.Color.Black
        Me.CommunicationSettingsTab.Location = New System.Drawing.Point(4, 22)
        Me.CommunicationSettingsTab.Name = "CommunicationSettingsTab"
        Me.CommunicationSettingsTab.Padding = New System.Windows.Forms.Padding(3)
        Me.CommunicationSettingsTab.Size = New System.Drawing.Size(697, 306)
        Me.CommunicationSettingsTab.TabIndex = 1
        Me.CommunicationSettingsTab.Text = "Communication"
        '
        'BsCommsGroupBox
        '
        Me.BsCommsGroupBox.Controls.Add(Me.bsAutomaticRadioButtonC)
        Me.BsCommsGroupBox.Controls.Add(Me.bsPortLabel)
        Me.BsCommsGroupBox.Controls.Add(Me.bsManualRadioButtonC)
        Me.BsCommsGroupBox.Controls.Add(Me.bsSpeedLabel)
        Me.BsCommsGroupBox.Controls.Add(Me.bsPortComboBox)
        Me.BsCommsGroupBox.Controls.Add(Me.bsSpeedComboBox)
        Me.BsCommsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.BsCommsGroupBox.Location = New System.Drawing.Point(6, 6)
        Me.BsCommsGroupBox.Name = "BsCommsGroupBox"
        Me.BsCommsGroupBox.Size = New System.Drawing.Size(418, 294)
        Me.BsCommsGroupBox.TabIndex = 20
        Me.BsCommsGroupBox.TabStop = False
        '
        'bsAutomaticRadioButtonC
        '
        Me.bsAutomaticRadioButtonC.AutoSize = True
        Me.bsAutomaticRadioButtonC.Checked = True
        Me.bsAutomaticRadioButtonC.ForeColor = System.Drawing.Color.Black
        Me.bsAutomaticRadioButtonC.Location = New System.Drawing.Point(18, 29)
        Me.bsAutomaticRadioButtonC.Name = "bsAutomaticRadioButtonC"
        Me.bsAutomaticRadioButtonC.Size = New System.Drawing.Size(93, 17)
        Me.bsAutomaticRadioButtonC.TabIndex = 1
        Me.bsAutomaticRadioButtonC.TabStop = True
        Me.bsAutomaticRadioButtonC.Text = "*Automatic "
        Me.bsAutomaticRadioButtonC.UseVisualStyleBackColor = True
        '
        'bsPortLabel
        '
        Me.bsPortLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsPortLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPortLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPortLabel.Location = New System.Drawing.Point(35, 84)
        Me.bsPortLabel.Name = "bsPortLabel"
        Me.bsPortLabel.Size = New System.Drawing.Size(163, 13)
        Me.bsPortLabel.TabIndex = 17
        Me.bsPortLabel.Text = "*Port"
        Me.bsPortLabel.Title = False
        '
        'bsManualRadioButtonC
        '
        Me.bsManualRadioButtonC.AutoSize = True
        Me.bsManualRadioButtonC.ForeColor = System.Drawing.Color.Black
        Me.bsManualRadioButtonC.Location = New System.Drawing.Point(18, 59)
        Me.bsManualRadioButtonC.Name = "bsManualRadioButtonC"
        Me.bsManualRadioButtonC.Size = New System.Drawing.Size(72, 17)
        Me.bsManualRadioButtonC.TabIndex = 2
        Me.bsManualRadioButtonC.Text = "*Manual"
        Me.bsManualRadioButtonC.UseVisualStyleBackColor = True
        '
        'bsSpeedLabel
        '
        Me.bsSpeedLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSpeedLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSpeedLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSpeedLabel.Location = New System.Drawing.Point(218, 84)
        Me.bsSpeedLabel.Name = "bsSpeedLabel"
        Me.bsSpeedLabel.Size = New System.Drawing.Size(163, 13)
        Me.bsSpeedLabel.TabIndex = 18
        Me.bsSpeedLabel.Text = "*Speed"
        Me.bsSpeedLabel.Title = False
        '
        'bsPortComboBox
        '
        Me.bsPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsPortComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsPortComboBox.FormattingEnabled = True
        Me.bsPortComboBox.Location = New System.Drawing.Point(35, 102)
        Me.bsPortComboBox.Name = "bsPortComboBox"
        Me.bsPortComboBox.Size = New System.Drawing.Size(163, 21)
        Me.bsPortComboBox.TabIndex = 3
        '
        'bsSpeedComboBox
        '
        Me.bsSpeedComboBox.BackColor = System.Drawing.Color.Gainsboro
        Me.bsSpeedComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple
        Me.bsSpeedComboBox.Enabled = False
        Me.bsSpeedComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsSpeedComboBox.FormattingEnabled = True
        Me.bsSpeedComboBox.Location = New System.Drawing.Point(218, 102)
        Me.bsSpeedComboBox.Name = "bsSpeedComboBox"
        Me.bsSpeedComboBox.Size = New System.Drawing.Size(163, 21)
        Me.bsSpeedComboBox.TabIndex = 4
        '
        'BsTestCommunicationsButton
        '
        Me.BsTestCommunicationsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsTestCommunicationsButton.Location = New System.Drawing.Point(430, 20)
        Me.BsTestCommunicationsButton.Name = "BsTestCommunicationsButton"
        Me.BsTestCommunicationsButton.Size = New System.Drawing.Size(32, 32)
        Me.BsTestCommunicationsButton.TabIndex = 19
        Me.BsTestCommunicationsButton.UseVisualStyleBackColor = True
        Me.BsTestCommunicationsButton.Visible = False
        '
        'LIMSTab
        '
        Me.LIMSTab.BackColor = System.Drawing.Color.Gainsboro
        Me.LIMSTab.Controls.Add(Me.bsAutomaticSessionCheckBox)
        Me.LIMSTab.Controls.Add(Me.bsOnlineExportGroupBox)
        Me.LIMSTab.Controls.Add(Me.bsOnLineExportCheckBox)
        Me.LIMSTab.Location = New System.Drawing.Point(4, 22)
        Me.LIMSTab.Name = "LIMSTab"
        Me.LIMSTab.Size = New System.Drawing.Size(697, 306)
        Me.LIMSTab.TabIndex = 2
        Me.LIMSTab.Text = "LIMS"
        '
        'bsAutomaticSessionCheckBox
        '
        Me.bsAutomaticSessionCheckBox.AutoSize = True
        Me.bsAutomaticSessionCheckBox.BackColor = System.Drawing.Color.Gainsboro
        Me.bsAutomaticSessionCheckBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAutomaticSessionCheckBox.ForeColor = System.Drawing.Color.Black
        Me.bsAutomaticSessionCheckBox.Location = New System.Drawing.Point(20, 20)
        Me.bsAutomaticSessionCheckBox.Name = "bsAutomaticSessionCheckBox"
        Me.bsAutomaticSessionCheckBox.Size = New System.Drawing.Size(194, 17)
        Me.bsAutomaticSessionCheckBox.TabIndex = 12
        Me.bsAutomaticSessionCheckBox.Text = "*Export Session during Reset"
        Me.bsAutomaticSessionCheckBox.UseVisualStyleBackColor = False
        '
        'bsOnlineExportGroupBox
        '
        Me.bsOnlineExportGroupBox.Controls.Add(Me.bsFreqComboBox)
        Me.bsOnlineExportGroupBox.Controls.Add(Me.bsFrequencyLabel)
        Me.bsOnlineExportGroupBox.Controls.Add(Me.bsManualRadioButtonS)
        Me.bsOnlineExportGroupBox.Controls.Add(Me.bsAutomaticRadioButtonS)
        Me.bsOnlineExportGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsOnlineExportGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsOnlineExportGroupBox.Location = New System.Drawing.Point(37, 57)
        Me.bsOnlineExportGroupBox.Name = "bsOnlineExportGroupBox"
        Me.bsOnlineExportGroupBox.Size = New System.Drawing.Size(363, 132)
        Me.bsOnlineExportGroupBox.TabIndex = 9
        Me.bsOnlineExportGroupBox.TabStop = False
        '
        'bsFreqComboBox
        '
        Me.bsFreqComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsFreqComboBox.Enabled = False
        Me.bsFreqComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsFreqComboBox.FormattingEnabled = True
        Me.bsFreqComboBox.Location = New System.Drawing.Point(40, 63)
        Me.bsFreqComboBox.Name = "bsFreqComboBox"
        Me.bsFreqComboBox.Size = New System.Drawing.Size(296, 21)
        Me.bsFreqComboBox.TabIndex = 10
        '
        'bsFrequencyLabel
        '
        Me.bsFrequencyLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsFrequencyLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFrequencyLabel.ForeColor = System.Drawing.Color.Black
        Me.bsFrequencyLabel.Location = New System.Drawing.Point(40, 45)
        Me.bsFrequencyLabel.Name = "bsFrequencyLabel"
        Me.bsFrequencyLabel.Size = New System.Drawing.Size(220, 13)
        Me.bsFrequencyLabel.TabIndex = 11
        Me.bsFrequencyLabel.Text = "*Frequency"
        Me.bsFrequencyLabel.Title = False
        '
        'bsManualRadioButtonS
        '
        Me.bsManualRadioButtonS.AutoSize = True
        Me.bsManualRadioButtonS.Checked = True
        Me.bsManualRadioButtonS.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsManualRadioButtonS.Location = New System.Drawing.Point(20, 103)
        Me.bsManualRadioButtonS.Name = "bsManualRadioButtonS"
        Me.bsManualRadioButtonS.Size = New System.Drawing.Size(72, 17)
        Me.bsManualRadioButtonS.TabIndex = 11
        Me.bsManualRadioButtonS.TabStop = True
        Me.bsManualRadioButtonS.Text = "*Manual"
        Me.bsManualRadioButtonS.UseVisualStyleBackColor = True
        '
        'bsAutomaticRadioButtonS
        '
        Me.bsAutomaticRadioButtonS.AutoSize = True
        Me.bsAutomaticRadioButtonS.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAutomaticRadioButtonS.Location = New System.Drawing.Point(20, 20)
        Me.bsAutomaticRadioButtonS.Name = "bsAutomaticRadioButtonS"
        Me.bsAutomaticRadioButtonS.Size = New System.Drawing.Size(89, 17)
        Me.bsAutomaticRadioButtonS.TabIndex = 9
        Me.bsAutomaticRadioButtonS.TabStop = True
        Me.bsAutomaticRadioButtonS.Text = "*Automatic"
        Me.bsAutomaticRadioButtonS.UseVisualStyleBackColor = True
        '
        'bsOnLineExportCheckBox
        '
        Me.bsOnLineExportCheckBox.AutoSize = True
        Me.bsOnLineExportCheckBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsOnLineExportCheckBox.ForeColor = System.Drawing.Color.Black
        Me.bsOnLineExportCheckBox.Location = New System.Drawing.Point(20, 50)
        Me.bsOnLineExportCheckBox.Name = "bsOnLineExportCheckBox"
        Me.bsOnLineExportCheckBox.Size = New System.Drawing.Size(117, 17)
        Me.bsOnLineExportCheckBox.TabIndex = 8
        Me.bsOnLineExportCheckBox.Text = "*On Line Export"
        Me.bsOnLineExportCheckBox.UseVisualStyleBackColor = True
        Me.bsOnLineExportCheckBox.Visible = False
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.Location = New System.Drawing.Point(669, 407)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 12
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'BsBorderedPanel1
        '
        Me.BsBorderedPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsBorderedPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsBorderedPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsBorderedPanel1.Name = "BsBorderedPanel1"
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(750, 451)
        Me.BsBorderedPanel1.TabIndex = 22
        '
        'UiConfigGeneral
        '
        Me.AcceptButton = Me.bsCancelButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(750, 451)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsAnalyzerConfigGroupBox)
        Me.Controls.Add(Me.bsCancelButton)
        Me.Controls.Add(Me.bsAcceptButton)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiConfigGeneral"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        Me.bsAnalyzerConfigGroupBox.ResumeLayout(False)
        Me.bsAnalyzersConfigTabControl.ResumeLayout(False)
        Me.SessionSettingsTab.ResumeLayout(False)
        Me.SessionSettingsTab.PerformLayout()
        Me.BsAutomaticReportGroupBox.ResumeLayout(False)
        Me.BsAutomaticReportGroupBox.PerformLayout()
        Me.AnalyzerTab.ResumeLayout(False)
        Me.AnalyzerTab.PerformLayout()
        Me.bsDisabledElementsGroupBox.ResumeLayout(False)
        Me.bsDisabledElementsGroupBox.PerformLayout()
        Me.bsWaterEntranceGroupBox.ResumeLayout(False)
        Me.bsWaterEntranceGroupBox.PerformLayout()
        Me.CommunicationSettingsTab.ResumeLayout(False)
        Me.BsCommsGroupBox.ResumeLayout(False)
        Me.BsCommsGroupBox.PerformLayout()
        Me.LIMSTab.ResumeLayout(False)
        Me.LIMSTab.PerformLayout()
        Me.bsOnlineExportGroupBox.ResumeLayout(False)
        Me.bsOnlineExportGroupBox.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SessionSettingsTab As System.Windows.Forms.TabPage
    Friend WithEvents bsAutResultPrintCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsAutomaticRerunCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsAnalyzersConfigTabControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsAnalyzerConfigurationLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents CommunicationSettingsTab As System.Windows.Forms.TabPage
    Friend WithEvents bsManualRadioButtonC As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsAutomaticRadioButtonC As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsSpeedComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsPortComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsSpeedLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsPortLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAnalyzerConfigGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents SampleTubeTypeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsBarCodeStartCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsResetSamplesRotorCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents LIMSTab As System.Windows.Forms.TabPage
    Friend WithEvents AnalyzerTab As System.Windows.Forms.TabPage
    Friend WithEvents bsAlarmSoundDisabledCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsWaterEntranceGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsLabWaterCircuitRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsExtWaterTankRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsOnlineExportGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsFreqComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsFrequencyLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsManualRadioButtonS As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsAutomaticRadioButtonS As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsAutomaticSessionCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsDisabledElementsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsClotDetectionCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsReagentsRotorCvrCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsSamplesRotorCvrCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsReactionRotorCvrCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsGralAnalyzerCvrCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsSampleTubeTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTestCommunicationsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsCommsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
    Friend WithEvents bsOnLineExportCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsAutomaticReportGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsAutomaticReportFrequency As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsAutomaticReportType As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsAutoReportFreqLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAutoReportTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
End Class
