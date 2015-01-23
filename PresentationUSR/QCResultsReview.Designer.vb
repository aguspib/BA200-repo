<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiQCResultsReview
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                ReleaseElements()
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiQCResultsReview))
        Me.bsTestSampleListView = New Biosystems.Ax00.Controls.UserControls.BSListView
        Me.bsTestSampleGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsTestSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsCalculationCriteriaGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsRulesGroupbox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bs41SCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bs10XmCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bsR4SCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bs22SCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bs13SCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bs12SCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bsSDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsRejectionNumeric = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.bsRejectionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsNumberOfSeriesNumeric = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.bsSearchButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsCalculationModeCombo = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsCalculationModeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsMultirulesApplication2Combo = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsMultirulesApplication1Combo = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsMultirulesApplicationLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDateToDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
        Me.bsDateToLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDateFromDateTimePick = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
        Me.bsDateFromLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsCalculationCriteriaLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsResultsByCtrlGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsControlLotResultsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsGraphsButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsAddButtom = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsEditButtom = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsDeleteButtom = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsIndividualResultDetLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsResultsDetailsGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsControlLotLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsResultControlLotGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsResultErrorProv = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsCumulateButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsTestSampleGroupBox.SuspendLayout()
        Me.bsCalculationCriteriaGroupBox.SuspendLayout()
        Me.bsRulesGroupbox.SuspendLayout()
        CType(Me.bsRejectionNumeric, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsNumberOfSeriesNumeric, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsResultsByCtrlGroupBox.SuspendLayout()
        CType(Me.bsResultsDetailsGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsResultControlLotGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsResultErrorProv, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsTestSampleListView
        '
        Me.bsTestSampleListView.AllowColumnReorder = True
        Me.bsTestSampleListView.AutoArrange = False
        Me.bsTestSampleListView.BackColor = System.Drawing.Color.White
        Me.bsTestSampleListView.ForeColor = System.Drawing.Color.Black
        Me.bsTestSampleListView.FullRowSelect = True
        Me.bsTestSampleListView.HideSelection = False
        Me.bsTestSampleListView.Location = New System.Drawing.Point(5, 40)
        Me.bsTestSampleListView.MultiSelect = False
        Me.bsTestSampleListView.Name = "bsTestSampleListView"
        Me.bsTestSampleListView.Size = New System.Drawing.Size(224, 550)
        Me.bsTestSampleListView.TabIndex = 0
        Me.bsTestSampleListView.UseCompatibleStateImageBehavior = False
        Me.bsTestSampleListView.View = System.Windows.Forms.View.Details
        '
        'bsTestSampleGroupBox
        '
        Me.bsTestSampleGroupBox.Controls.Add(Me.bsTestSampleTypeLabel)
        Me.bsTestSampleGroupBox.Controls.Add(Me.bsTestSampleListView)
        Me.bsTestSampleGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestSampleGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsTestSampleGroupBox.Name = "bsTestSampleGroupBox"
        Me.bsTestSampleGroupBox.Size = New System.Drawing.Size(234, 598)
        Me.bsTestSampleGroupBox.TabIndex = 1
        Me.bsTestSampleGroupBox.TabStop = False
        '
        'bsTestSampleTypeLabel
        '
        Me.bsTestSampleTypeLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTestSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTestSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestSampleTypeLabel.Location = New System.Drawing.Point(5, 15)
        Me.bsTestSampleTypeLabel.Name = "bsTestSampleTypeLabel"
        Me.bsTestSampleTypeLabel.Size = New System.Drawing.Size(224, 20)
        Me.bsTestSampleTypeLabel.TabIndex = 1
        Me.bsTestSampleTypeLabel.Text = "*Tests/Sample Type"
        Me.bsTestSampleTypeLabel.Title = True
        '
        'bsCalculationCriteriaGroupBox
        '
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsRulesGroupbox)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsSDLabel)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsRejectionNumeric)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsRejectionLabel)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsNumberOfSeriesNumeric)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsSearchButton)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsCalculationModeCombo)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsCalculationModeLabel)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsMultirulesApplication2Combo)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsMultirulesApplication1Combo)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsMultirulesApplicationLabel)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsDateToDateTimePick)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsDateToLabel)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsDateFromDateTimePick)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsDateFromLabel)
        Me.bsCalculationCriteriaGroupBox.Controls.Add(Me.bsCalculationCriteriaLabel)
        Me.bsCalculationCriteriaGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsCalculationCriteriaGroupBox.Location = New System.Drawing.Point(249, 10)
        Me.bsCalculationCriteriaGroupBox.Name = "bsCalculationCriteriaGroupBox"
        Me.bsCalculationCriteriaGroupBox.Size = New System.Drawing.Size(719, 131)
        Me.bsCalculationCriteriaGroupBox.TabIndex = 2
        Me.bsCalculationCriteriaGroupBox.TabStop = False
        '
        'bsRulesGroupbox
        '
        Me.bsRulesGroupbox.Controls.Add(Me.bs41SCheckBox)
        Me.bsRulesGroupbox.Controls.Add(Me.bs10XmCheckBox)
        Me.bsRulesGroupbox.Controls.Add(Me.bsR4SCheckBox)
        Me.bsRulesGroupbox.Controls.Add(Me.bs22SCheckBox)
        Me.bsRulesGroupbox.Controls.Add(Me.bs13SCheckBox)
        Me.bsRulesGroupbox.Controls.Add(Me.bs12SCheckBox)
        Me.bsRulesGroupbox.ForeColor = System.Drawing.Color.Black
        Me.bsRulesGroupbox.Location = New System.Drawing.Point(298, 80)
        Me.bsRulesGroupbox.Name = "bsRulesGroupbox"
        Me.bsRulesGroupbox.Size = New System.Drawing.Size(368, 43)
        Me.bsRulesGroupbox.TabIndex = 7
        Me.bsRulesGroupbox.TabStop = False
        '
        'bs41SCheckBox
        '
        Me.bs41SCheckBox.AutoSize = True
        Me.bs41SCheckBox.Location = New System.Drawing.Point(248, 17)
        Me.bs41SCheckBox.Name = "bs41SCheckBox"
        Me.bs41SCheckBox.Size = New System.Drawing.Size(51, 17)
        Me.bs41SCheckBox.TabIndex = 4
        Me.bs41SCheckBox.Text = "4-1s"
        Me.bs41SCheckBox.UseVisualStyleBackColor = True
        '
        'bs10XmCheckBox
        '
        Me.bs10XmCheckBox.AutoSize = True
        Me.bs10XmCheckBox.Location = New System.Drawing.Point(304, 17)
        Me.bs10XmCheckBox.Name = "bs10XmCheckBox"
        Me.bs10XmCheckBox.Size = New System.Drawing.Size(59, 17)
        Me.bs10XmCheckBox.TabIndex = 5
        Me.bs10XmCheckBox.Text = "10Xm"
        Me.bs10XmCheckBox.UseVisualStyleBackColor = True
        '
        'bsR4SCheckBox
        '
        Me.bsR4SCheckBox.AutoSize = True
        Me.bsR4SCheckBox.Location = New System.Drawing.Point(184, 17)
        Me.bsR4SCheckBox.Name = "bsR4SCheckBox"
        Me.bsR4SCheckBox.Size = New System.Drawing.Size(52, 17)
        Me.bsR4SCheckBox.TabIndex = 3
        Me.bsR4SCheckBox.Text = "R-4s"
        Me.bsR4SCheckBox.UseVisualStyleBackColor = True
        '
        'bs22SCheckBox
        '
        Me.bs22SCheckBox.AutoSize = True
        Me.bs22SCheckBox.Location = New System.Drawing.Point(127, 17)
        Me.bs22SCheckBox.Name = "bs22SCheckBox"
        Me.bs22SCheckBox.Size = New System.Drawing.Size(51, 17)
        Me.bs22SCheckBox.TabIndex = 2
        Me.bs22SCheckBox.Text = "2-2s"
        Me.bs22SCheckBox.UseVisualStyleBackColor = True
        '
        'bs13SCheckBox
        '
        Me.bs13SCheckBox.AutoSize = True
        Me.bs13SCheckBox.Location = New System.Drawing.Point(71, 17)
        Me.bs13SCheckBox.Name = "bs13SCheckBox"
        Me.bs13SCheckBox.Size = New System.Drawing.Size(51, 17)
        Me.bs13SCheckBox.TabIndex = 1
        Me.bs13SCheckBox.Text = "1-3s"
        Me.bs13SCheckBox.UseVisualStyleBackColor = True
        '
        'bs12SCheckBox
        '
        Me.bs12SCheckBox.AutoSize = True
        Me.bs12SCheckBox.Checked = True
        Me.bs12SCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.bs12SCheckBox.Enabled = False
        Me.bs12SCheckBox.Location = New System.Drawing.Point(15, 17)
        Me.bs12SCheckBox.Name = "bs12SCheckBox"
        Me.bs12SCheckBox.Size = New System.Drawing.Size(51, 17)
        Me.bs12SCheckBox.TabIndex = 0
        Me.bs12SCheckBox.Text = "1-2s"
        Me.bs12SCheckBox.UseVisualStyleBackColor = True
        '
        'bsSDLabel
        '
        Me.bsSDLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bsSDLabel.AutoSize = True
        Me.bsSDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSDLabel.Location = New System.Drawing.Point(245, 105)
        Me.bsSDLabel.Name = "bsSDLabel"
        Me.bsSDLabel.Size = New System.Drawing.Size(24, 13)
        Me.bsSDLabel.TabIndex = 14
        Me.bsSDLabel.Text = "SD"
        Me.bsSDLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsSDLabel.Title = False
        '
        'bsRejectionNumeric
        '
        Me.bsRejectionNumeric.BackColor = System.Drawing.Color.White
        Me.bsRejectionNumeric.ForeColor = System.Drawing.Color.Black
        Me.bsRejectionNumeric.Location = New System.Drawing.Point(197, 102)
        Me.bsRejectionNumeric.Name = "bsRejectionNumeric"
        Me.bsRejectionNumeric.Size = New System.Drawing.Size(42, 21)
        Me.bsRejectionNumeric.TabIndex = 6
        Me.bsRejectionNumeric.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsRejectionLabel
        '
        Me.bsRejectionLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRejectionLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRejectionLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRejectionLabel.Location = New System.Drawing.Point(196, 84)
        Me.bsRejectionLabel.Name = "bsRejectionLabel"
        Me.bsRejectionLabel.Size = New System.Drawing.Size(91, 13)
        Me.bsRejectionLabel.TabIndex = 12
        Me.bsRejectionLabel.Text = "*Rejection:"
        Me.bsRejectionLabel.Title = False
        '
        'bsNumberOfSeriesNumeric
        '
        Me.bsNumberOfSeriesNumeric.BackColor = System.Drawing.Color.White
        Me.bsNumberOfSeriesNumeric.ForeColor = System.Drawing.Color.Black
        Me.bsNumberOfSeriesNumeric.Location = New System.Drawing.Point(124, 101)
        Me.bsNumberOfSeriesNumeric.Name = "bsNumberOfSeriesNumeric"
        Me.bsNumberOfSeriesNumeric.Size = New System.Drawing.Size(42, 21)
        Me.bsNumberOfSeriesNumeric.TabIndex = 5
        Me.bsNumberOfSeriesNumeric.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsSearchButton
        '
        Me.bsSearchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSearchButton.Location = New System.Drawing.Point(677, 44)
        Me.bsSearchButton.Name = "bsSearchButton"
        Me.bsSearchButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSearchButton.TabIndex = 8
        Me.bsSearchButton.UseVisualStyleBackColor = True
        '
        'bsCalculationModeCombo
        '
        Me.bsCalculationModeCombo.BackColor = System.Drawing.Color.White
        Me.bsCalculationModeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsCalculationModeCombo.ForeColor = System.Drawing.Color.Black
        Me.bsCalculationModeCombo.FormattingEnabled = True
        Me.bsCalculationModeCombo.Location = New System.Drawing.Point(10, 101)
        Me.bsCalculationModeCombo.Name = "bsCalculationModeCombo"
        Me.bsCalculationModeCombo.Size = New System.Drawing.Size(108, 21)
        Me.bsCalculationModeCombo.TabIndex = 4
        '
        'bsCalculationModeLabel
        '
        Me.bsCalculationModeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsCalculationModeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCalculationModeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCalculationModeLabel.Location = New System.Drawing.Point(9, 84)
        Me.bsCalculationModeLabel.Name = "bsCalculationModeLabel"
        Me.bsCalculationModeLabel.Size = New System.Drawing.Size(182, 13)
        Me.bsCalculationModeLabel.TabIndex = 8
        Me.bsCalculationModeLabel.Text = "* Mode:"
        Me.bsCalculationModeLabel.Title = False
        '
        'bsMultirulesApplication2Combo
        '
        Me.bsMultirulesApplication2Combo.BackColor = System.Drawing.Color.White
        Me.bsMultirulesApplication2Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsMultirulesApplication2Combo.ForeColor = System.Drawing.Color.Black
        Me.bsMultirulesApplication2Combo.FormattingEnabled = True
        Me.bsMultirulesApplication2Combo.Location = New System.Drawing.Point(485, 56)
        Me.bsMultirulesApplication2Combo.Name = "bsMultirulesApplication2Combo"
        Me.bsMultirulesApplication2Combo.Size = New System.Drawing.Size(181, 21)
        Me.bsMultirulesApplication2Combo.TabIndex = 3
        '
        'bsMultirulesApplication1Combo
        '
        Me.bsMultirulesApplication1Combo.BackColor = System.Drawing.Color.White
        Me.bsMultirulesApplication1Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsMultirulesApplication1Combo.ForeColor = System.Drawing.Color.Black
        Me.bsMultirulesApplication1Combo.FormattingEnabled = True
        Me.bsMultirulesApplication1Combo.Location = New System.Drawing.Point(298, 56)
        Me.bsMultirulesApplication1Combo.Name = "bsMultirulesApplication1Combo"
        Me.bsMultirulesApplication1Combo.Size = New System.Drawing.Size(181, 21)
        Me.bsMultirulesApplication1Combo.TabIndex = 2
        '
        'bsMultirulesApplicationLabel
        '
        Me.bsMultirulesApplicationLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsMultirulesApplicationLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsMultirulesApplicationLabel.ForeColor = System.Drawing.Color.Black
        Me.bsMultirulesApplicationLabel.Location = New System.Drawing.Point(300, 40)
        Me.bsMultirulesApplicationLabel.Name = "bsMultirulesApplicationLabel"
        Me.bsMultirulesApplicationLabel.Size = New System.Drawing.Size(362, 13)
        Me.bsMultirulesApplicationLabel.TabIndex = 5
        Me.bsMultirulesApplicationLabel.Text = "*Rules application:"
        Me.bsMultirulesApplicationLabel.Title = False
        '
        'bsDateToDateTimePick
        '
        Me.bsDateToDateTimePick.CalendarFont = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateToDateTimePick.CalendarForeColor = System.Drawing.Color.Black
        Me.bsDateToDateTimePick.CalendarMonthBackground = System.Drawing.Color.White
        Me.bsDateToDateTimePick.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.bsDateToDateTimePick.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.bsDateToDateTimePick.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.bsDateToDateTimePick.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateToDateTimePick.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.bsDateToDateTimePick.Location = New System.Drawing.Point(148, 56)
        Me.bsDateToDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsDateToDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsDateToDateTimePick.Name = "bsDateToDateTimePick"
        Me.bsDateToDateTimePick.Size = New System.Drawing.Size(108, 21)
        Me.bsDateToDateTimePick.TabIndex = 1
        '
        'bsDateToLabel
        '
        Me.bsDateToLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDateToLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateToLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDateToLabel.Location = New System.Drawing.Point(148, 40)
        Me.bsDateToLabel.Name = "bsDateToLabel"
        Me.bsDateToLabel.Size = New System.Drawing.Size(108, 13)
        Me.bsDateToLabel.TabIndex = 3
        Me.bsDateToLabel.Text = "* To:"
        Me.bsDateToLabel.Title = False
        '
        'bsDateFromDateTimePick
        '
        Me.bsDateFromDateTimePick.CalendarFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDateFromDateTimePick.CalendarForeColor = System.Drawing.Color.Black
        Me.bsDateFromDateTimePick.CalendarMonthBackground = System.Drawing.Color.White
        Me.bsDateFromDateTimePick.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.bsDateFromDateTimePick.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.bsDateFromDateTimePick.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.bsDateFromDateTimePick.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDateFromDateTimePick.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.bsDateFromDateTimePick.Location = New System.Drawing.Point(13, 56)
        Me.bsDateFromDateTimePick.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsDateFromDateTimePick.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsDateFromDateTimePick.Name = "bsDateFromDateTimePick"
        Me.bsDateFromDateTimePick.Size = New System.Drawing.Size(108, 21)
        Me.bsDateFromDateTimePick.TabIndex = 0
        '
        'bsDateFromLabel
        '
        Me.bsDateFromLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDateFromLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDateFromLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDateFromLabel.Location = New System.Drawing.Point(9, 40)
        Me.bsDateFromLabel.Name = "bsDateFromLabel"
        Me.bsDateFromLabel.Size = New System.Drawing.Size(108, 13)
        Me.bsDateFromLabel.TabIndex = 1
        Me.bsDateFromLabel.Text = "*From:"
        Me.bsDateFromLabel.Title = False
        '
        'bsCalculationCriteriaLabel
        '
        Me.bsCalculationCriteriaLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsCalculationCriteriaLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsCalculationCriteriaLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCalculationCriteriaLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsCalculationCriteriaLabel.Name = "bsCalculationCriteriaLabel"
        Me.bsCalculationCriteriaLabel.Size = New System.Drawing.Size(699, 20)
        Me.bsCalculationCriteriaLabel.TabIndex = 0
        Me.bsCalculationCriteriaLabel.Text = "*Calculation Criteria"
        Me.bsCalculationCriteriaLabel.Title = True
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Location = New System.Drawing.Point(926, 613)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 3
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsResultsByCtrlGroupBox
        '
        Me.bsResultsByCtrlGroupBox.Controls.Add(Me.bsControlLotResultsLabel)
        Me.bsResultsByCtrlGroupBox.Controls.Add(Me.bsGraphsButton)
        Me.bsResultsByCtrlGroupBox.Controls.Add(Me.bsAddButtom)
        Me.bsResultsByCtrlGroupBox.Controls.Add(Me.bsEditButtom)
        Me.bsResultsByCtrlGroupBox.Controls.Add(Me.bsDeleteButtom)
        Me.bsResultsByCtrlGroupBox.Controls.Add(Me.bsIndividualResultDetLabel)
        Me.bsResultsByCtrlGroupBox.Controls.Add(Me.bsResultsDetailsGridView)
        Me.bsResultsByCtrlGroupBox.Controls.Add(Me.bsControlLotLabel)
        Me.bsResultsByCtrlGroupBox.Controls.Add(Me.bsResultControlLotGridView)
        Me.bsResultsByCtrlGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsResultsByCtrlGroupBox.Location = New System.Drawing.Point(249, 142)
        Me.bsResultsByCtrlGroupBox.Name = "bsResultsByCtrlGroupBox"
        Me.bsResultsByCtrlGroupBox.Size = New System.Drawing.Size(719, 466)
        Me.bsResultsByCtrlGroupBox.TabIndex = 0
        Me.bsResultsByCtrlGroupBox.TabStop = False
        '
        'bsControlLotResultsLabel
        '
        Me.bsControlLotResultsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsControlLotResultsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsControlLotResultsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsControlLotResultsLabel.Location = New System.Drawing.Point(512, 15)
        Me.bsControlLotResultsLabel.Name = "bsControlLotResultsLabel"
        Me.bsControlLotResultsLabel.Size = New System.Drawing.Size(197, 20)
        Me.bsControlLotResultsLabel.TabIndex = 6
        Me.bsControlLotResultsLabel.Text = "*Results"
        Me.bsControlLotResultsLabel.Title = True
        '
        'bsGraphsButton
        '
        Me.bsGraphsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsGraphsButton.Location = New System.Drawing.Point(10, 428)
        Me.bsGraphsButton.Name = "bsGraphsButton"
        Me.bsGraphsButton.Size = New System.Drawing.Size(32, 32)
        Me.bsGraphsButton.TabIndex = 2
        Me.bsGraphsButton.UseVisualStyleBackColor = True
        '
        'bsAddButtom
        '
        Me.bsAddButtom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAddButtom.Location = New System.Drawing.Point(603, 428)
        Me.bsAddButtom.Name = "bsAddButtom"
        Me.bsAddButtom.Size = New System.Drawing.Size(32, 32)
        Me.bsAddButtom.TabIndex = 3
        Me.bsAddButtom.UseVisualStyleBackColor = True
        '
        'bsEditButtom
        '
        Me.bsEditButtom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsEditButtom.Location = New System.Drawing.Point(640, 428)
        Me.bsEditButtom.Name = "bsEditButtom"
        Me.bsEditButtom.Size = New System.Drawing.Size(32, 32)
        Me.bsEditButtom.TabIndex = 4
        Me.bsEditButtom.UseVisualStyleBackColor = True
        '
        'bsDeleteButtom
        '
        Me.bsDeleteButtom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsDeleteButtom.Location = New System.Drawing.Point(677, 428)
        Me.bsDeleteButtom.Name = "bsDeleteButtom"
        Me.bsDeleteButtom.Size = New System.Drawing.Size(32, 32)
        Me.bsDeleteButtom.TabIndex = 5
        Me.bsDeleteButtom.UseVisualStyleBackColor = True
        '
        'bsIndividualResultDetLabel
        '
        Me.bsIndividualResultDetLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsIndividualResultDetLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsIndividualResultDetLabel.ForeColor = System.Drawing.Color.Black
        Me.bsIndividualResultDetLabel.Location = New System.Drawing.Point(10, 155)
        Me.bsIndividualResultDetLabel.Name = "bsIndividualResultDetLabel"
        Me.bsIndividualResultDetLabel.Size = New System.Drawing.Size(699, 20)
        Me.bsIndividualResultDetLabel.TabIndex = 3
        Me.bsIndividualResultDetLabel.Text = "*Individual Results Details"
        Me.bsIndividualResultDetLabel.Title = True
        '
        'bsResultsDetailsGridView
        '
        Me.bsResultsDetailsGridView.AllowUserToAddRows = False
        Me.bsResultsDetailsGridView.AllowUserToDeleteRows = False
        Me.bsResultsDetailsGridView.AllowUserToResizeColumns = False
        Me.bsResultsDetailsGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.bsResultsDetailsGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsResultsDetailsGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsResultsDetailsGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsResultsDetailsGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultsDetailsGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsResultsDetailsGridView.ColumnHeadersHeight = 20
        Me.bsResultsDetailsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultsDetailsGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsResultsDetailsGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.bsResultsDetailsGridView.EnterToTab = False
        Me.bsResultsDetailsGridView.GridColor = System.Drawing.Color.Silver
        Me.bsResultsDetailsGridView.Location = New System.Drawing.Point(10, 180)
        Me.bsResultsDetailsGridView.Name = "bsResultsDetailsGridView"
        Me.bsResultsDetailsGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultsDetailsGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsResultsDetailsGridView.RowHeadersVisible = False
        Me.bsResultsDetailsGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        Me.bsResultsDetailsGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsResultsDetailsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsResultsDetailsGridView.ShowCellErrors = False
        Me.bsResultsDetailsGridView.Size = New System.Drawing.Size(699, 243)
        Me.bsResultsDetailsGridView.TabIndex = 1
        Me.bsResultsDetailsGridView.TabToEnter = False
        '
        'bsControlLotLabel
        '
        Me.bsControlLotLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsControlLotLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsControlLotLabel.ForeColor = System.Drawing.Color.Black
        Me.bsControlLotLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsControlLotLabel.Name = "bsControlLotLabel"
        Me.bsControlLotLabel.Size = New System.Drawing.Size(496, 20)
        Me.bsControlLotLabel.TabIndex = 1
        Me.bsControlLotLabel.Text = "*Controls/Lots"
        Me.bsControlLotLabel.Title = True
        '
        'bsResultControlLotGridView
        '
        Me.bsResultControlLotGridView.AllowUserToAddRows = False
        Me.bsResultControlLotGridView.AllowUserToDeleteRows = False
        Me.bsResultControlLotGridView.AllowUserToResizeColumns = False
        Me.bsResultControlLotGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.bsResultControlLotGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle6
        Me.bsResultControlLotGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsResultControlLotGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsResultControlLotGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle7.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultControlLotGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.bsResultControlLotGridView.ColumnHeadersHeight = 20
        Me.bsResultControlLotGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultControlLotGridView.DefaultCellStyle = DataGridViewCellStyle8
        Me.bsResultControlLotGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.bsResultControlLotGridView.EnterToTab = False
        Me.bsResultControlLotGridView.GridColor = System.Drawing.Color.Silver
        Me.bsResultControlLotGridView.Location = New System.Drawing.Point(10, 40)
        Me.bsResultControlLotGridView.Name = "bsResultControlLotGridView"
        Me.bsResultControlLotGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle9.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsResultControlLotGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.bsResultControlLotGridView.RowHeadersVisible = False
        Me.bsResultControlLotGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle10.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White
        Me.bsResultControlLotGridView.RowsDefaultCellStyle = DataGridViewCellStyle10
        Me.bsResultControlLotGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsResultControlLotGridView.ShowCellErrors = False
        Me.bsResultControlLotGridView.ShowRowErrors = False
        Me.bsResultControlLotGridView.Size = New System.Drawing.Size(699, 110)
        Me.bsResultControlLotGridView.TabIndex = 0
        Me.bsResultControlLotGridView.TabToEnter = False
        '
        'bsResultErrorProv
        '
        Me.bsResultErrorProv.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsResultErrorProv.ContainerControl = Me
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Location = New System.Drawing.Point(212, 613)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 7
        Me.bsPrintButton.UseVisualStyleBackColor = True
        '
        'bsCumulateButton
        '
        Me.bsCumulateButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCumulateButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCumulateButton.Location = New System.Drawing.Point(175, 613)
        Me.bsCumulateButton.Name = "bsCumulateButton"
        Me.bsCumulateButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCumulateButton.TabIndex = 6
        Me.bsCumulateButton.UseVisualStyleBackColor = True
        '
        'IQCResultsReview
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsCumulateButton)
        Me.Controls.Add(Me.bsPrintButton)
        Me.Controls.Add(Me.bsResultsByCtrlGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsCalculationCriteriaGroupBox)
        Me.Controls.Add(Me.bsTestSampleGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IQCResultsReview"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = ""
        Me.bsTestSampleGroupBox.ResumeLayout(False)
        Me.bsCalculationCriteriaGroupBox.ResumeLayout(False)
        Me.bsCalculationCriteriaGroupBox.PerformLayout()
        Me.bsRulesGroupbox.ResumeLayout(False)
        Me.bsRulesGroupbox.PerformLayout()
        CType(Me.bsRejectionNumeric, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsNumberOfSeriesNumeric, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsResultsByCtrlGroupBox.ResumeLayout(False)
        CType(Me.bsResultsDetailsGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsResultControlLotGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsResultErrorProv, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsTestSampleListView As Biosystems.Ax00.Controls.UserControls.BSListView
    Friend WithEvents bsTestSampleGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTestSampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsCalculationCriteriaGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsDateFromDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents bsDateFromLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsCalculationCriteriaLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsMultirulesApplication2Combo As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsMultirulesApplication1Combo As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsMultirulesApplicationLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDateToDateTimePick As Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
    Friend WithEvents bsDateToLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsNumberOfSeriesNumeric As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsSearchButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCalculationModeCombo As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsCalculationModeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRejectionNumeric As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsRejectionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRulesGroupbox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bs22SCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bs13SCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bs12SCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsSDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bs10XmCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsR4SCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bs41SCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsResultsByCtrlGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsControlLotLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsResultControlLotGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsResultsDetailsGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsIndividualResultDetLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsResultErrorProv As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsDeleteButtom As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAddButtom As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsEditButtom As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCumulateButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsGraphsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsControlLotResultsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel

End Class
