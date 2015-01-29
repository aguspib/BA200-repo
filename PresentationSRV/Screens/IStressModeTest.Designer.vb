<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiStressModeTest
    Inherits PesentationLayer.BSAdjustmentBaseForm

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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.RequestStatusStressTimer = New System.Windows.Forms.Timer(Me.components)
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTestButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTestButton_TODELETE = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustButtonTODELETE = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsStressAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsCyclesUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.BsConfigLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsAbortTestButtonTODELETE = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTestButtonTODELETE = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsResultsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.lbTitle = New System.Windows.Forms.Label
        Me.TypeTestLabel = New System.Windows.Forms.Label
        Me.ErrorsNumLabel = New System.Windows.Forms.Label
        Me.TimeCompletedLabel = New System.Windows.Forms.Label
        Me.TimeTotalLabel = New System.Windows.Forms.Label
        Me.TimeStartLabel = New System.Windows.Forms.Label
        Me.ResetsNumLabel = New System.Windows.Forms.Label
        Me.CyclesCompletedLabel = New System.Windows.Forms.Label
        Me.CyclesLabel = New System.Windows.Forms.Label
        Me.BsErrorsDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.BsResetsTextBox = New System.Windows.Forms.TextBox
        Me.BsNumErrLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsErrDescLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsNumResetsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsResetsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsProgramCyclesLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsCompleteCyclesLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsTimeProgLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsTimeCompleteLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsTimeStartLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsStressTypeResultLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsStressTypeGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.BsFluidRadioBtn = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.BsSyringesRadioBtn = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.BsPhotometryRadioBtn = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.BsRotorsRadioBtn = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.BsFluidsComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.BsSyringesComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.BsPhotometryComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.BsRotorsComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.BsArmsComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.BsArmsRadBtn = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.bsPartialRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.bsCompleteRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.bsNumCyclesLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsResultsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsTitleLabel = New System.Windows.Forms.Label
        Me.BsStressInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsStressInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoXPSViewer = New BsXPSViewer
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsButtonsPanel.SuspendLayout()
        Me.BsStressAdjustPanel.SuspendLayout()
        CType(Me.BsCyclesUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsResultsGroupBox.SuspendLayout()
        CType(Me.BsErrorsDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsStressTypeGroupBox.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.BsStressInfoPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'RequestStatusStressTimer
        '
        '
        'BsMessagesPanel
        '
        Me.BsMessagesPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessagesPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsMessagesPanel.Controls.Add(Me.ProgressBar1)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageImage)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageLabel)
        Me.BsMessagesPanel.Location = New System.Drawing.Point(0, 557)
        Me.BsMessagesPanel.Name = "BsMessagesPanel"
        Me.BsMessagesPanel.Size = New System.Drawing.Size(811, 35)
        Me.BsMessagesPanel.TabIndex = 35
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(440, 8)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(180, 18)
        Me.ProgressBar1.TabIndex = 4
        Me.ProgressBar1.Visible = False
        '
        'BsMessageImage
        '
        Me.BsMessageImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsMessageImage.Location = New System.Drawing.Point(3, 1)
        Me.BsMessageImage.Name = "BsMessageImage"
        Me.BsMessageImage.PositionNumber = 0
        Me.BsMessageImage.Size = New System.Drawing.Size(32, 32)
        Me.BsMessageImage.TabIndex = 3
        Me.BsMessageImage.TabStop = False
        '
        'BsMessageLabel
        '
        Me.BsMessageLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsMessageLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsMessageLabel.ForeColor = System.Drawing.Color.Black
        Me.BsMessageLabel.Location = New System.Drawing.Point(41, 11)
        Me.BsMessageLabel.Name = "BsMessageLabel"
        Me.BsMessageLabel.Size = New System.Drawing.Size(762, 13)
        Me.BsMessageLabel.TabIndex = 1
        Me.BsMessageLabel.Title = False
        '
        'BsButtonsPanel
        '
        Me.BsButtonsPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButtonsPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsButtonsPanel.Controls.Add(Me.BsExitButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsTestButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsTestButton_TODELETE)
        Me.BsButtonsPanel.Controls.Add(Me.BsAdjustButtonTODELETE)
        Me.BsButtonsPanel.Location = New System.Drawing.Point(810, 557)
        Me.BsButtonsPanel.Name = "BsButtonsPanel"
        Me.BsButtonsPanel.Size = New System.Drawing.Size(168, 35)
        Me.BsButtonsPanel.TabIndex = 34
        '
        'BsExitButton
        '
        Me.BsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsExitButton.Enabled = False
        Me.BsExitButton.Location = New System.Drawing.Point(134, 1)
        Me.BsExitButton.Name = "BsExitButton"
        Me.BsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.BsExitButton.TabIndex = 3
        Me.BsExitButton.UseVisualStyleBackColor = True
        '
        'BsTestButton
        '
        Me.BsTestButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTestButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsTestButton.Enabled = False
        Me.BsTestButton.Location = New System.Drawing.Point(98, 1)
        Me.BsTestButton.Name = "BsTestButton"
        Me.BsTestButton.Size = New System.Drawing.Size(32, 32)
        Me.BsTestButton.TabIndex = 2
        Me.BsTestButton.UseVisualStyleBackColor = True
        '
        'BsTestButton_TODELETE
        '
        Me.BsTestButton_TODELETE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTestButton_TODELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsTestButton_TODELETE.Enabled = False
        Me.BsTestButton_TODELETE.Location = New System.Drawing.Point(62, 1)
        Me.BsTestButton_TODELETE.Name = "BsTestButton_TODELETE"
        Me.BsTestButton_TODELETE.Size = New System.Drawing.Size(32, 32)
        Me.BsTestButton_TODELETE.TabIndex = 1
        Me.BsTestButton_TODELETE.UseVisualStyleBackColor = True
        Me.BsTestButton_TODELETE.Visible = False
        '
        'BsAdjustButtonTODELETE
        '
        Me.BsAdjustButtonTODELETE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustButtonTODELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsAdjustButtonTODELETE.Enabled = False
        Me.BsAdjustButtonTODELETE.Location = New System.Drawing.Point(25, 1)
        Me.BsAdjustButtonTODELETE.Name = "BsAdjustButtonTODELETE"
        Me.BsAdjustButtonTODELETE.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjustButtonTODELETE.TabIndex = 0
        Me.BsAdjustButtonTODELETE.UseVisualStyleBackColor = True
        Me.BsAdjustButtonTODELETE.Visible = False
        '
        'BsStressAdjustPanel
        '
        Me.BsStressAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsStressAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsStressAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsStressAdjustPanel.Controls.Add(Me.BsCyclesUpDown)
        Me.BsStressAdjustPanel.Controls.Add(Me.BsConfigLabel)
        Me.BsStressAdjustPanel.Controls.Add(Me.BsAbortTestButtonTODELETE)
        Me.BsStressAdjustPanel.Controls.Add(Me.BsTestButtonTODELETE)
        Me.BsStressAdjustPanel.Controls.Add(Me.BsResultsGroupBox)
        Me.BsStressAdjustPanel.Controls.Add(Me.BsStressTypeGroupBox)
        Me.BsStressAdjustPanel.Controls.Add(Me.bsNumCyclesLabel)
        Me.BsStressAdjustPanel.Controls.Add(Me.BsResultsLabel)
        Me.BsStressAdjustPanel.Location = New System.Drawing.Point(234, 25)
        Me.BsStressAdjustPanel.Name = "BsStressAdjustPanel"
        Me.BsStressAdjustPanel.Size = New System.Drawing.Size(741, 532)
        Me.BsStressAdjustPanel.TabIndex = 32
        '
        'BsCyclesUpDown
        '
        Me.BsCyclesUpDown.ForeColor = System.Drawing.Color.Black
        Me.BsCyclesUpDown.Location = New System.Drawing.Point(10, 50)
        Me.BsCyclesUpDown.Maximum = New Decimal(New Integer() {30000, 0, 0, 0})
        Me.BsCyclesUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.BsCyclesUpDown.Name = "BsCyclesUpDown"
        Me.BsCyclesUpDown.Size = New System.Drawing.Size(66, 21)
        Me.BsCyclesUpDown.TabIndex = 64
        Me.BsCyclesUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.BsCyclesUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'BsConfigLabel
        '
        Me.BsConfigLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsConfigLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsConfigLabel.ForeColor = System.Drawing.Color.Black
        Me.BsConfigLabel.Location = New System.Drawing.Point(0, 0)
        Me.BsConfigLabel.Name = "BsConfigLabel"
        Me.BsConfigLabel.Size = New System.Drawing.Size(739, 20)
        Me.BsConfigLabel.TabIndex = 59
        Me.BsConfigLabel.Text = "Configurig Test"
        Me.BsConfigLabel.Title = True
        '
        'BsAbortTestButtonTODELETE
        '
        Me.BsAbortTestButtonTODELETE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAbortTestButtonTODELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsAbortTestButtonTODELETE.Enabled = False
        Me.BsAbortTestButtonTODELETE.Location = New System.Drawing.Point(701, 43)
        Me.BsAbortTestButtonTODELETE.Name = "BsAbortTestButtonTODELETE"
        Me.BsAbortTestButtonTODELETE.Size = New System.Drawing.Size(32, 32)
        Me.BsAbortTestButtonTODELETE.TabIndex = 54
        Me.BsAbortTestButtonTODELETE.UseVisualStyleBackColor = True
        Me.BsAbortTestButtonTODELETE.Visible = False
        '
        'BsTestButtonTODELETE
        '
        Me.BsTestButtonTODELETE.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTestButtonTODELETE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsTestButtonTODELETE.Enabled = False
        Me.BsTestButtonTODELETE.Location = New System.Drawing.Point(665, 43)
        Me.BsTestButtonTODELETE.Name = "BsTestButtonTODELETE"
        Me.BsTestButtonTODELETE.Size = New System.Drawing.Size(32, 32)
        Me.BsTestButtonTODELETE.TabIndex = 55
        Me.BsTestButtonTODELETE.UseVisualStyleBackColor = True
        Me.BsTestButtonTODELETE.Visible = False
        '
        'BsResultsGroupBox
        '
        Me.BsResultsGroupBox.Controls.Add(Me.Label1)
        Me.BsResultsGroupBox.Controls.Add(Me.lbTitle)
        Me.BsResultsGroupBox.Controls.Add(Me.TypeTestLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.ErrorsNumLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.TimeCompletedLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.TimeTotalLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.TimeStartLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.ResetsNumLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.CyclesCompletedLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.CyclesLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.BsErrorsDataGridView)
        Me.BsResultsGroupBox.Controls.Add(Me.BsResetsTextBox)
        Me.BsResultsGroupBox.Controls.Add(Me.BsNumErrLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.BsErrDescLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.BsNumResetsLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.BsResetsLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.BsProgramCyclesLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.BsCompleteCyclesLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.BsTimeProgLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.BsTimeCompleteLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.BsTimeStartLabel)
        Me.BsResultsGroupBox.Controls.Add(Me.BsStressTypeResultLabel)
        Me.BsResultsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.BsResultsGroupBox.Location = New System.Drawing.Point(9, 183)
        Me.BsResultsGroupBox.Name = "BsResultsGroupBox"
        Me.BsResultsGroupBox.Size = New System.Drawing.Size(722, 153)
        Me.BsResultsGroupBox.TabIndex = 49
        Me.BsResultsGroupBox.TabStop = False
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.LightGray
        Me.Label1.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.Label1.ForeColor = System.Drawing.Color.Red
        Me.Label1.Location = New System.Drawing.Point(452, 205)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(220, 19)
        Me.Label1.TabIndex = 83
        Me.Label1.Text = "PROVISIONAL (pdt Fw)"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.Label1.Visible = False
        '
        'lbTitle
        '
        Me.lbTitle.BackColor = System.Drawing.Color.Transparent
        Me.lbTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.lbTitle.ForeColor = System.Drawing.Color.Red
        Me.lbTitle.Location = New System.Drawing.Point(72, 205)
        Me.lbTitle.Name = "lbTitle"
        Me.lbTitle.Size = New System.Drawing.Size(220, 19)
        Me.lbTitle.TabIndex = 82
        Me.lbTitle.Text = "PROVISIONAL (pdt Fw)"
        Me.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lbTitle.Visible = False
        '
        'TypeTestLabel
        '
        Me.TypeTestLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TypeTestLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TypeTestLabel.Font = New System.Drawing.Font("Verdana", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TypeTestLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.TypeTestLabel.Location = New System.Drawing.Point(244, 16)
        Me.TypeTestLabel.Name = "TypeTestLabel"
        Me.TypeTestLabel.Size = New System.Drawing.Size(110, 30)
        Me.TypeTestLabel.TabIndex = 79
        Me.TypeTestLabel.Text = "Complete Test"
        Me.TypeTestLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ErrorsNumLabel
        '
        Me.ErrorsNumLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ErrorsNumLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ErrorsNumLabel.Enabled = False
        Me.ErrorsNumLabel.Font = New System.Drawing.Font("Digiface", 18.0!)
        Me.ErrorsNumLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.ErrorsNumLabel.Location = New System.Drawing.Point(606, 112)
        Me.ErrorsNumLabel.Name = "ErrorsNumLabel"
        Me.ErrorsNumLabel.Size = New System.Drawing.Size(110, 30)
        Me.ErrorsNumLabel.TabIndex = 78
        Me.ErrorsNumLabel.Text = "3"
        Me.ErrorsNumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TimeCompletedLabel
        '
        Me.TimeCompletedLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TimeCompletedLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TimeCompletedLabel.Font = New System.Drawing.Font("Digiface", 18.0!)
        Me.TimeCompletedLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.TimeCompletedLabel.Location = New System.Drawing.Point(606, 80)
        Me.TimeCompletedLabel.Name = "TimeCompletedLabel"
        Me.TimeCompletedLabel.Size = New System.Drawing.Size(110, 30)
        Me.TimeCompletedLabel.TabIndex = 77
        Me.TimeCompletedLabel.Text = "00:03:00"
        Me.TimeCompletedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TimeTotalLabel
        '
        Me.TimeTotalLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TimeTotalLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TimeTotalLabel.Font = New System.Drawing.Font("Digiface", 18.0!)
        Me.TimeTotalLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.TimeTotalLabel.Location = New System.Drawing.Point(606, 48)
        Me.TimeTotalLabel.Name = "TimeTotalLabel"
        Me.TimeTotalLabel.Size = New System.Drawing.Size(110, 30)
        Me.TimeTotalLabel.TabIndex = 76
        Me.TimeTotalLabel.Text = "00:06:00"
        Me.TimeTotalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TimeStartLabel
        '
        Me.TimeStartLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TimeStartLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TimeStartLabel.Font = New System.Drawing.Font("Digiface", 18.0!)
        Me.TimeStartLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.TimeStartLabel.Location = New System.Drawing.Point(606, 16)
        Me.TimeStartLabel.Name = "TimeStartLabel"
        Me.TimeStartLabel.Size = New System.Drawing.Size(110, 30)
        Me.TimeStartLabel.TabIndex = 75
        Me.TimeStartLabel.Text = "11:02:20"
        Me.TimeStartLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ResetsNumLabel
        '
        Me.ResetsNumLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ResetsNumLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ResetsNumLabel.Enabled = False
        Me.ResetsNumLabel.Font = New System.Drawing.Font("Digiface", 18.0!)
        Me.ResetsNumLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.ResetsNumLabel.Location = New System.Drawing.Point(244, 112)
        Me.ResetsNumLabel.Name = "ResetsNumLabel"
        Me.ResetsNumLabel.Size = New System.Drawing.Size(110, 30)
        Me.ResetsNumLabel.TabIndex = 74
        Me.ResetsNumLabel.Text = "3"
        Me.ResetsNumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CyclesCompletedLabel
        '
        Me.CyclesCompletedLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.CyclesCompletedLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CyclesCompletedLabel.Font = New System.Drawing.Font("Digiface", 18.0!)
        Me.CyclesCompletedLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.CyclesCompletedLabel.Location = New System.Drawing.Point(244, 80)
        Me.CyclesCompletedLabel.Name = "CyclesCompletedLabel"
        Me.CyclesCompletedLabel.Size = New System.Drawing.Size(110, 30)
        Me.CyclesCompletedLabel.TabIndex = 73
        Me.CyclesCompletedLabel.Text = "20.000"
        Me.CyclesCompletedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'CyclesLabel
        '
        Me.CyclesLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.CyclesLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CyclesLabel.Font = New System.Drawing.Font("Digiface", 18.0!)
        Me.CyclesLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.CyclesLabel.Location = New System.Drawing.Point(244, 48)
        Me.CyclesLabel.Name = "CyclesLabel"
        Me.CyclesLabel.Size = New System.Drawing.Size(110, 30)
        Me.CyclesLabel.TabIndex = 72
        Me.CyclesLabel.Text = "30.000"
        Me.CyclesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'BsErrorsDataGridView
        '
        Me.BsErrorsDataGridView.AllowUserToAddRows = False
        Me.BsErrorsDataGridView.AllowUserToDeleteRows = False
        Me.BsErrorsDataGridView.AllowUserToResizeColumns = False
        Me.BsErrorsDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.BsErrorsDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.BsErrorsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.BsErrorsDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders
        Me.BsErrorsDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.BsErrorsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.BsErrorsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BsErrorsDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.BsErrorsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.BsErrorsDataGridView.ColumnHeadersVisible = False
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BsErrorsDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.BsErrorsDataGridView.Enabled = False
        Me.BsErrorsDataGridView.EnterToTab = False
        Me.BsErrorsDataGridView.GridColor = System.Drawing.Color.Silver
        Me.BsErrorsDataGridView.Location = New System.Drawing.Point(383, 169)
        Me.BsErrorsDataGridView.MultiSelect = False
        Me.BsErrorsDataGridView.Name = "BsErrorsDataGridView"
        Me.BsErrorsDataGridView.ReadOnly = True
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.BsErrorsDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.BsErrorsDataGridView.RowHeadersVisible = False
        Me.BsErrorsDataGridView.Size = New System.Drawing.Size(333, 91)
        Me.BsErrorsDataGridView.TabIndex = 62
        Me.BsErrorsDataGridView.TabToEnter = False
        '
        'BsResetsTextBox
        '
        Me.BsResetsTextBox.BackColor = System.Drawing.Color.Gainsboro
        Me.BsResetsTextBox.Enabled = False
        Me.BsResetsTextBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsResetsTextBox.Location = New System.Drawing.Point(10, 169)
        Me.BsResetsTextBox.Multiline = True
        Me.BsResetsTextBox.Name = "BsResetsTextBox"
        Me.BsResetsTextBox.ReadOnly = True
        Me.BsResetsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.BsResetsTextBox.Size = New System.Drawing.Size(344, 91)
        Me.BsResetsTextBox.TabIndex = 61
        '
        'BsNumErrLabel
        '
        Me.BsNumErrLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsNumErrLabel.Enabled = False
        Me.BsNumErrLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsNumErrLabel.ForeColor = System.Drawing.Color.Black
        Me.BsNumErrLabel.Location = New System.Drawing.Point(467, 114)
        Me.BsNumErrLabel.Name = "BsNumErrLabel"
        Me.BsNumErrLabel.Size = New System.Drawing.Size(133, 30)
        Me.BsNumErrLabel.TabIndex = 60
        Me.BsNumErrLabel.Text = "Number of Errors:"
        Me.BsNumErrLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsNumErrLabel.Title = False
        '
        'BsErrDescLabel
        '
        Me.BsErrDescLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsErrDescLabel.Enabled = False
        Me.BsErrDescLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsErrDescLabel.ForeColor = System.Drawing.Color.Black
        Me.BsErrDescLabel.Location = New System.Drawing.Point(380, 152)
        Me.BsErrDescLabel.Name = "BsErrDescLabel"
        Me.BsErrDescLabel.Size = New System.Drawing.Size(133, 19)
        Me.BsErrDescLabel.TabIndex = 59
        Me.BsErrDescLabel.Text = "Error Description"
        Me.BsErrDescLabel.Title = False
        '
        'BsNumResetsLabel
        '
        Me.BsNumResetsLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsNumResetsLabel.Enabled = False
        Me.BsNumResetsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsNumResetsLabel.ForeColor = System.Drawing.Color.Black
        Me.BsNumResetsLabel.Location = New System.Drawing.Point(105, 114)
        Me.BsNumResetsLabel.Name = "BsNumResetsLabel"
        Me.BsNumResetsLabel.Size = New System.Drawing.Size(133, 30)
        Me.BsNumResetsLabel.TabIndex = 58
        Me.BsNumResetsLabel.Text = "Number of Resets:"
        Me.BsNumResetsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsNumResetsLabel.Title = False
        '
        'BsResetsLabel
        '
        Me.BsResetsLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsResetsLabel.Enabled = False
        Me.BsResetsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsResetsLabel.ForeColor = System.Drawing.Color.Black
        Me.BsResetsLabel.Location = New System.Drawing.Point(6, 152)
        Me.BsResetsLabel.Name = "BsResetsLabel"
        Me.BsResetsLabel.Size = New System.Drawing.Size(133, 19)
        Me.BsResetsLabel.TabIndex = 57
        Me.BsResetsLabel.Text = "Cycles with Resets"
        Me.BsResetsLabel.Title = False
        Me.BsResetsLabel.Visible = False
        '
        'BsProgramCyclesLabel
        '
        Me.BsProgramCyclesLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsProgramCyclesLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsProgramCyclesLabel.ForeColor = System.Drawing.Color.Black
        Me.BsProgramCyclesLabel.Location = New System.Drawing.Point(6, 50)
        Me.BsProgramCyclesLabel.Name = "BsProgramCyclesLabel"
        Me.BsProgramCyclesLabel.Size = New System.Drawing.Size(232, 30)
        Me.BsProgramCyclesLabel.TabIndex = 50
        Me.BsProgramCyclesLabel.Text = "Number of stress cycles programmed:"
        Me.BsProgramCyclesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsProgramCyclesLabel.Title = False
        '
        'BsCompleteCyclesLabel
        '
        Me.BsCompleteCyclesLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsCompleteCyclesLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsCompleteCyclesLabel.ForeColor = System.Drawing.Color.Black
        Me.BsCompleteCyclesLabel.Location = New System.Drawing.Point(6, 82)
        Me.BsCompleteCyclesLabel.Name = "BsCompleteCyclesLabel"
        Me.BsCompleteCyclesLabel.Size = New System.Drawing.Size(232, 30)
        Me.BsCompleteCyclesLabel.TabIndex = 49
        Me.BsCompleteCyclesLabel.Text = "Cycles number completed:"
        Me.BsCompleteCyclesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsCompleteCyclesLabel.Title = False
        '
        'BsTimeProgLabel
        '
        Me.BsTimeProgLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsTimeProgLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsTimeProgLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTimeProgLabel.Location = New System.Drawing.Point(398, 50)
        Me.BsTimeProgLabel.Name = "BsTimeProgLabel"
        Me.BsTimeProgLabel.Size = New System.Drawing.Size(202, 30)
        Me.BsTimeProgLabel.TabIndex = 48
        Me.BsTimeProgLabel.Text = "Stress total time programmed:"
        Me.BsTimeProgLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsTimeProgLabel.Title = False
        '
        'BsTimeCompleteLabel
        '
        Me.BsTimeCompleteLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsTimeCompleteLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsTimeCompleteLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTimeCompleteLabel.Location = New System.Drawing.Point(371, 80)
        Me.BsTimeCompleteLabel.Name = "BsTimeCompleteLabel"
        Me.BsTimeCompleteLabel.Size = New System.Drawing.Size(229, 30)
        Me.BsTimeCompleteLabel.TabIndex = 47
        Me.BsTimeCompleteLabel.Text = "Stress time completed:"
        Me.BsTimeCompleteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsTimeCompleteLabel.Title = False
        '
        'BsTimeStartLabel
        '
        Me.BsTimeStartLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsTimeStartLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsTimeStartLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTimeStartLabel.Location = New System.Drawing.Point(467, 18)
        Me.BsTimeStartLabel.Name = "BsTimeStartLabel"
        Me.BsTimeStartLabel.Size = New System.Drawing.Size(133, 30)
        Me.BsTimeStartLabel.TabIndex = 46
        Me.BsTimeStartLabel.Text = "Time start:"
        Me.BsTimeStartLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsTimeStartLabel.Title = False
        '
        'BsStressTypeResultLabel
        '
        Me.BsStressTypeResultLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsStressTypeResultLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsStressTypeResultLabel.ForeColor = System.Drawing.Color.Black
        Me.BsStressTypeResultLabel.Location = New System.Drawing.Point(139, 23)
        Me.BsStressTypeResultLabel.Name = "BsStressTypeResultLabel"
        Me.BsStressTypeResultLabel.Size = New System.Drawing.Size(99, 30)
        Me.BsStressTypeResultLabel.TabIndex = 45
        Me.BsStressTypeResultLabel.Text = "Stress type:"
        Me.BsStressTypeResultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsStressTypeResultLabel.Title = False
        '
        'BsStressTypeGroupBox
        '
        Me.BsStressTypeGroupBox.Controls.Add(Me.Label2)
        Me.BsStressTypeGroupBox.Controls.Add(Me.Panel1)
        Me.BsStressTypeGroupBox.Controls.Add(Me.bsPartialRadioButton)
        Me.BsStressTypeGroupBox.Controls.Add(Me.bsCompleteRadioButton)
        Me.BsStressTypeGroupBox.ForeColor = System.Drawing.Color.Black
        Me.BsStressTypeGroupBox.Location = New System.Drawing.Point(10, 76)
        Me.BsStressTypeGroupBox.Name = "BsStressTypeGroupBox"
        Me.BsStressTypeGroupBox.Size = New System.Drawing.Size(722, 43)
        Me.BsStressTypeGroupBox.TabIndex = 48
        Me.BsStressTypeGroupBox.TabStop = False
        Me.BsStressTypeGroupBox.Text = "Select stress type:"
        '
        'Label2
        '
        Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.Label2.ForeColor = System.Drawing.Color.Red
        Me.Label2.Location = New System.Drawing.Point(293, -31)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(220, 19)
        Me.Label2.TabIndex = 83
        Me.Label2.Text = "PROVISIONAL (pdt Fw)"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.Label2.Visible = False
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Gainsboro
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.BsFluidRadioBtn)
        Me.Panel1.Controls.Add(Me.BsSyringesRadioBtn)
        Me.Panel1.Controls.Add(Me.BsPhotometryRadioBtn)
        Me.Panel1.Controls.Add(Me.BsRotorsRadioBtn)
        Me.Panel1.Controls.Add(Me.BsFluidsComboBox)
        Me.Panel1.Controls.Add(Me.BsSyringesComboBox)
        Me.Panel1.Controls.Add(Me.BsPhotometryComboBox)
        Me.Panel1.Controls.Add(Me.BsRotorsComboBox)
        Me.Panel1.Controls.Add(Me.BsArmsComboBox)
        Me.Panel1.Controls.Add(Me.BsArmsRadBtn)
        Me.Panel1.ForeColor = System.Drawing.Color.Black
        Me.Panel1.Location = New System.Drawing.Point(6, 49)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(710, 81)
        Me.Panel1.TabIndex = 78
        Me.Panel1.Visible = False
        '
        'BsFluidRadioBtn
        '
        Me.BsFluidRadioBtn.AutoSize = True
        Me.BsFluidRadioBtn.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFluidRadioBtn.Location = New System.Drawing.Point(581, 7)
        Me.BsFluidRadioBtn.Name = "BsFluidRadioBtn"
        Me.BsFluidRadioBtn.Size = New System.Drawing.Size(51, 17)
        Me.BsFluidRadioBtn.TabIndex = 99
        Me.BsFluidRadioBtn.Text = "Fluid"
        Me.BsFluidRadioBtn.UseVisualStyleBackColor = True
        '
        'BsSyringesRadioBtn
        '
        Me.BsSyringesRadioBtn.AutoSize = True
        Me.BsSyringesRadioBtn.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsSyringesRadioBtn.Location = New System.Drawing.Point(448, 6)
        Me.BsSyringesRadioBtn.Name = "BsSyringesRadioBtn"
        Me.BsSyringesRadioBtn.Size = New System.Drawing.Size(75, 17)
        Me.BsSyringesRadioBtn.TabIndex = 98
        Me.BsSyringesRadioBtn.Text = "Syringes"
        Me.BsSyringesRadioBtn.UseVisualStyleBackColor = True
        '
        'BsPhotometryRadioBtn
        '
        Me.BsPhotometryRadioBtn.AutoSize = True
        Me.BsPhotometryRadioBtn.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsPhotometryRadioBtn.Location = New System.Drawing.Point(313, 7)
        Me.BsPhotometryRadioBtn.Name = "BsPhotometryRadioBtn"
        Me.BsPhotometryRadioBtn.Size = New System.Drawing.Size(91, 17)
        Me.BsPhotometryRadioBtn.TabIndex = 97
        Me.BsPhotometryRadioBtn.Text = "Photometry"
        Me.BsPhotometryRadioBtn.UseVisualStyleBackColor = True
        '
        'BsRotorsRadioBtn
        '
        Me.BsRotorsRadioBtn.AutoSize = True
        Me.BsRotorsRadioBtn.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsRotorsRadioBtn.Location = New System.Drawing.Point(179, 6)
        Me.BsRotorsRadioBtn.Name = "BsRotorsRadioBtn"
        Me.BsRotorsRadioBtn.Size = New System.Drawing.Size(62, 17)
        Me.BsRotorsRadioBtn.TabIndex = 96
        Me.BsRotorsRadioBtn.Text = "Rotors"
        Me.BsRotorsRadioBtn.UseVisualStyleBackColor = True
        '
        'BsFluidsComboBox
        '
        Me.BsFluidsComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsFluidsComboBox.FormattingEnabled = True
        Me.BsFluidsComboBox.Location = New System.Drawing.Point(581, 30)
        Me.BsFluidsComboBox.Name = "BsFluidsComboBox"
        Me.BsFluidsComboBox.Size = New System.Drawing.Size(121, 21)
        Me.BsFluidsComboBox.TabIndex = 95
        '
        'BsSyringesComboBox
        '
        Me.BsSyringesComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsSyringesComboBox.FormattingEnabled = True
        Me.BsSyringesComboBox.Location = New System.Drawing.Point(448, 30)
        Me.BsSyringesComboBox.Name = "BsSyringesComboBox"
        Me.BsSyringesComboBox.Size = New System.Drawing.Size(121, 21)
        Me.BsSyringesComboBox.TabIndex = 94
        '
        'BsPhotometryComboBox
        '
        Me.BsPhotometryComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsPhotometryComboBox.FormattingEnabled = True
        Me.BsPhotometryComboBox.Location = New System.Drawing.Point(313, 30)
        Me.BsPhotometryComboBox.Name = "BsPhotometryComboBox"
        Me.BsPhotometryComboBox.Size = New System.Drawing.Size(121, 21)
        Me.BsPhotometryComboBox.TabIndex = 93
        '
        'BsRotorsComboBox
        '
        Me.BsRotorsComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsRotorsComboBox.FormattingEnabled = True
        Me.BsRotorsComboBox.Location = New System.Drawing.Point(179, 30)
        Me.BsRotorsComboBox.Name = "BsRotorsComboBox"
        Me.BsRotorsComboBox.Size = New System.Drawing.Size(121, 21)
        Me.BsRotorsComboBox.TabIndex = 92
        '
        'BsArmsComboBox
        '
        Me.BsArmsComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsArmsComboBox.FormattingEnabled = True
        Me.BsArmsComboBox.Location = New System.Drawing.Point(8, 30)
        Me.BsArmsComboBox.Name = "BsArmsComboBox"
        Me.BsArmsComboBox.Size = New System.Drawing.Size(159, 21)
        Me.BsArmsComboBox.TabIndex = 91
        '
        'BsArmsRadBtn
        '
        Me.BsArmsRadBtn.AutoSize = True
        Me.BsArmsRadBtn.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsArmsRadBtn.Location = New System.Drawing.Point(8, 7)
        Me.BsArmsRadBtn.Name = "BsArmsRadBtn"
        Me.BsArmsRadBtn.Size = New System.Drawing.Size(55, 17)
        Me.BsArmsRadBtn.TabIndex = 84
        Me.BsArmsRadBtn.Text = "Arms"
        Me.BsArmsRadBtn.UseVisualStyleBackColor = True
        '
        'bsPartialRadioButton
        '
        Me.bsPartialRadioButton.AutoSize = True
        Me.bsPartialRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsPartialRadioButton.Location = New System.Drawing.Point(288, 20)
        Me.bsPartialRadioButton.Name = "bsPartialRadioButton"
        Me.bsPartialRadioButton.Size = New System.Drawing.Size(99, 17)
        Me.bsPartialRadioButton.TabIndex = 77
        Me.bsPartialRadioButton.Text = "Partial stress"
        Me.bsPartialRadioButton.UseVisualStyleBackColor = True
        Me.bsPartialRadioButton.Visible = False
        '
        'bsCompleteRadioButton
        '
        Me.bsCompleteRadioButton.AutoSize = True
        Me.bsCompleteRadioButton.Checked = True
        Me.bsCompleteRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCompleteRadioButton.ForeColor = System.Drawing.Color.Black
        Me.bsCompleteRadioButton.Location = New System.Drawing.Point(120, 20)
        Me.bsCompleteRadioButton.Name = "bsCompleteRadioButton"
        Me.bsCompleteRadioButton.Size = New System.Drawing.Size(118, 17)
        Me.bsCompleteRadioButton.TabIndex = 76
        Me.bsCompleteRadioButton.TabStop = True
        Me.bsCompleteRadioButton.Text = "Complete stress"
        Me.bsCompleteRadioButton.UseVisualStyleBackColor = True
        '
        'bsNumCyclesLabel
        '
        Me.bsNumCyclesLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsNumCyclesLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNumCyclesLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNumCyclesLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNumCyclesLabel.Location = New System.Drawing.Point(7, 31)
        Me.bsNumCyclesLabel.Name = "bsNumCyclesLabel"
        Me.bsNumCyclesLabel.Size = New System.Drawing.Size(133, 19)
        Me.bsNumCyclesLabel.TabIndex = 44
        Me.bsNumCyclesLabel.Text = "Num Stress Cycles:"
        Me.bsNumCyclesLabel.Title = False
        '
        'BsResultsLabel
        '
        Me.BsResultsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsResultsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsResultsLabel.ForeColor = System.Drawing.Color.Black
        Me.BsResultsLabel.Location = New System.Drawing.Point(2, 152)
        Me.BsResultsLabel.Name = "BsResultsLabel"
        Me.BsResultsLabel.Size = New System.Drawing.Size(739, 20)
        Me.BsResultsLabel.TabIndex = 33
        Me.BsResultsLabel.Text = "Results Test"
        Me.BsResultsLabel.Title = True
        '
        'BsTitleLabel
        '
        Me.BsTitleLabel.AutoSize = True
        Me.BsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsTitleLabel.Location = New System.Drawing.Point(2, 2)
        Me.BsTitleLabel.Name = "BsTitleLabel"
        Me.BsTitleLabel.Size = New System.Drawing.Size(144, 17)
        Me.BsTitleLabel.TabIndex = 65
        Me.BsTitleLabel.Text = "Global Stress Mode"
        '
        'BsStressInfoTitle
        '
        Me.BsStressInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsStressInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsStressInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsStressInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsStressInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsStressInfoTitle.Name = "BsStressInfoTitle"
        Me.BsStressInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsStressInfoTitle.TabIndex = 22
        Me.BsStressInfoTitle.Text = "Information"
        Me.BsStressInfoTitle.Title = True
        '
        'BsStressInfoPanel
        '
        Me.BsStressInfoPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsStressInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsStressInfoPanel.Controls.Add(Me.BsInfoXPSViewer)
        Me.BsStressInfoPanel.Controls.Add(Me.BsStressInfoTitle)
        Me.BsStressInfoPanel.Location = New System.Drawing.Point(4, 25)
        Me.BsStressInfoPanel.Name = "BsStressInfoPanel"
        Me.BsStressInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsStressInfoPanel.TabIndex = 61
        '
        'BsInfoXPSViewer
        '
        Me.BsInfoXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoXPSViewer.CopyButtonVisible = True
        Me.BsInfoXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoXPSViewer.HorizontalPageMargin = 0
        Me.BsInfoXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoXPSViewer.IsLoaded = False
        Me.BsInfoXPSViewer.IsScrollable = False
        Me.BsInfoXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoXPSViewer.MenuBarVisible = False
        Me.BsInfoXPSViewer.Name = "BsInfoXPSViewer"
        Me.BsInfoXPSViewer.PopupMenuEnabled = True
        Me.BsInfoXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoXPSViewer.PrintButtonVisible = True
        Me.BsInfoXPSViewer.SearchBarVisible = False
        Me.BsInfoXPSViewer.Size = New System.Drawing.Size(230, 510)
        Me.BsInfoXPSViewer.TabIndex = 37
        Me.BsInfoXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoXPSViewer.VerticalPageMargin = 0
        Me.BsInfoXPSViewer.Visible = False
        Me.BsInfoXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoXPSViewer.WholePageButtonVisible = True
        '
        'IStressModeTest
        '
        Me.AcceptButton = Me.BsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 593)
        Me.Controls.Add(Me.BsTitleLabel)
        Me.Controls.Add(Me.BsStressInfoPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsStressAdjustPanel)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "UiStressModeTest"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "StressModeTest"
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsButtonsPanel.ResumeLayout(False)
        Me.BsStressAdjustPanel.ResumeLayout(False)
        CType(Me.BsCyclesUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsResultsGroupBox.ResumeLayout(False)
        Me.BsResultsGroupBox.PerformLayout()
        CType(Me.BsErrorsDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsStressTypeGroupBox.ResumeLayout(False)
        Me.BsStressTypeGroupBox.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.BsStressInfoPanel.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BsStressAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsConfigLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsAbortTestButtonTODELETE As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTestButtonTODELETE As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsResultsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsStressTypeGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsNumCyclesLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsResultsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTestButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsTestButton_TODELETE As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjustButtonTODELETE As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsCyclesUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsPartialRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsCompleteRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents BsArmsRadBtn As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents BsProgramCyclesLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsCompleteCyclesLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTimeProgLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTimeCompleteLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTimeStartLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsStressTypeResultLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents RequestStatusStressTimer As System.Windows.Forms.Timer
    Friend WithEvents BsNumErrLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsErrDescLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsNumResetsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsResetsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsResetsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents BsErrorsDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents BsFluidsComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsSyringesComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsPhotometryComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsRotorsComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsArmsComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsFluidRadioBtn As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents BsSyringesRadioBtn As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents BsPhotometryRadioBtn As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents BsRotorsRadioBtn As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents CyclesLabel As System.Windows.Forms.Label
    Friend WithEvents CyclesCompletedLabel As System.Windows.Forms.Label
    Friend WithEvents ResetsNumLabel As System.Windows.Forms.Label
    Friend WithEvents TimeStartLabel As System.Windows.Forms.Label
    Friend WithEvents ErrorsNumLabel As System.Windows.Forms.Label
    Friend WithEvents TimeCompletedLabel As System.Windows.Forms.Label
    Friend WithEvents TimeTotalLabel As System.Windows.Forms.Label
    Friend WithEvents TypeTestLabel As System.Windows.Forms.Label
    Friend WithEvents BsTitleLabel As System.Windows.Forms.Label
    Friend WithEvents BsStressInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsStressInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsInfoXPSViewer As BsXPSViewer
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lbTitle As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
