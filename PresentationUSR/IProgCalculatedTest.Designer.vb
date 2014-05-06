
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IProgCalculatedTest
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IProgCalculatedTest))
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsNewButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsEditButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsDeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsCalcTestListLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsCalTestListView = New Biosystems.Ax00.Controls.UserControls.BSListView()
        Me.bsCalTestDefGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.CalculatedTestTabControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl()
        Me.FormulaTabPage = New System.Windows.Forms.TabPage()
        Me.bsFormulaPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.bsCalTestFormula = New Biosystems.Ax00.Controls.UserControls.BSFormula()
        Me.RefRangesTabPage = New System.Windows.Forms.TabPage()
        Me.bsRefRangesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.bsTestRefRanges = New BSReferenceRanges()
        Me.bsPrintExpTestCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsSampleGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsSampleComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsMultipleRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.bsUniqueRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.bsFullNameTextbox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsFullNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDecimalsNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.bsDecimalsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsUnitComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsUnitLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsNameTextbox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsCalTestDefLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.bsCalcTestListGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsCalTestDefGroupBox.SuspendLayout()
        Me.CalculatedTestTabControl.SuspendLayout()
        Me.FormulaTabPage.SuspendLayout()
        Me.bsFormulaPanel.SuspendLayout()
        Me.RefRangesTabPage.SuspendLayout()
        Me.bsRefRangesPanel.SuspendLayout()
        Me.bsSampleGroupBox.SuspendLayout()
        CType(Me.bsDecimalsNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsCalcTestListGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(923, 613)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 18
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsNewButton
        '
        Me.bsNewButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsNewButton.Location = New System.Drawing.Point(101, 613)
        Me.bsNewButton.Name = "bsNewButton"
        Me.bsNewButton.Size = New System.Drawing.Size(32, 32)
        Me.bsNewButton.TabIndex = 14
        Me.bsNewButton.UseVisualStyleBackColor = True
        '
        'bsEditButton
        '
        Me.bsEditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsEditButton.Location = New System.Drawing.Point(138, 613)
        Me.bsEditButton.Name = "bsEditButton"
        Me.bsEditButton.Size = New System.Drawing.Size(32, 32)
        Me.bsEditButton.TabIndex = 15
        Me.bsEditButton.UseVisualStyleBackColor = True
        '
        'bsDeleteButton
        '
        Me.bsDeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsDeleteButton.Location = New System.Drawing.Point(175, 613)
        Me.bsDeleteButton.Name = "bsDeleteButton"
        Me.bsDeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.bsDeleteButton.TabIndex = 16
        Me.bsDeleteButton.UseVisualStyleBackColor = True
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Enabled = False
        Me.bsPrintButton.Location = New System.Drawing.Point(212, 613)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 17
        Me.bsPrintButton.UseVisualStyleBackColor = True
        Me.bsPrintButton.Visible = False
        '
        'bsCalcTestListLabel
        '
        Me.bsCalcTestListLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsCalcTestListLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsCalcTestListLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCalcTestListLabel.Location = New System.Drawing.Point(5, 15)
        Me.bsCalcTestListLabel.Name = "bsCalcTestListLabel"
        Me.bsCalcTestListLabel.Size = New System.Drawing.Size(224, 20)
        Me.bsCalcTestListLabel.TabIndex = 17
        Me.bsCalcTestListLabel.Text = "Calculated Tests"
        Me.bsCalcTestListLabel.Title = True
        '
        'bsCalTestListView
        '
        Me.bsCalTestListView.AllowColumnReorder = True
        Me.bsCalTestListView.AutoArrange = False
        Me.bsCalTestListView.BackColor = System.Drawing.Color.White
        Me.bsCalTestListView.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCalTestListView.ForeColor = System.Drawing.Color.Black
        Me.bsCalTestListView.FullRowSelect = True
        Me.bsCalTestListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.bsCalTestListView.HideSelection = False
        Me.bsCalTestListView.Location = New System.Drawing.Point(5, 40)
        Me.bsCalTestListView.Name = "bsCalTestListView"
        Me.bsCalTestListView.Size = New System.Drawing.Size(224, 550)
        Me.bsCalTestListView.TabIndex = 1
        Me.bsCalTestListView.UseCompatibleStateImageBehavior = False
        Me.bsCalTestListView.View = System.Windows.Forms.View.Details
        '
        'bsCalTestDefGroupBox
        '
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsSaveButton)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsCancelButton)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.CalculatedTestTabControl)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsPrintExpTestCheckbox)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsSampleGroupBox)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsFullNameTextbox)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsFullNameLabel)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsDecimalsNumericUpDown)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsDecimalsLabel)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsUnitComboBox)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsUnitLabel)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsNameTextbox)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsNameLabel)
        Me.bsCalTestDefGroupBox.Controls.Add(Me.bsCalTestDefLabel)
        Me.bsCalTestDefGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsCalTestDefGroupBox.Location = New System.Drawing.Point(249, 10)
        Me.bsCalTestDefGroupBox.Name = "bsCalTestDefGroupBox"
        Me.bsCalTestDefGroupBox.Size = New System.Drawing.Size(719, 598)
        Me.bsCalTestDefGroupBox.TabIndex = 14
        Me.bsCalTestDefGroupBox.TabStop = False
        '
        'bsSaveButton
        '
        Me.bsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSaveButton.Location = New System.Drawing.Point(637, 562)
        Me.bsSaveButton.Name = "bsSaveButton"
        Me.bsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveButton.TabIndex = 41
        Me.bsSaveButton.UseVisualStyleBackColor = True
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Location = New System.Drawing.Point(674, 562)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 42
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'CalculatedTestTabControl
        '
        Me.CalculatedTestTabControl.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.CalculatedTestTabControl.Controls.Add(Me.FormulaTabPage)
        Me.CalculatedTestTabControl.Controls.Add(Me.RefRangesTabPage)
        Me.CalculatedTestTabControl.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.CalculatedTestTabControl.Location = New System.Drawing.Point(10, 168)
        Me.CalculatedTestTabControl.Name = "CalculatedTestTabControl"
        Me.CalculatedTestTabControl.SelectedIndex = 0
        Me.CalculatedTestTabControl.Size = New System.Drawing.Size(699, 389)
        Me.CalculatedTestTabControl.TabIndex = 40
        '
        'FormulaTabPage
        '
        Me.FormulaTabPage.BackColor = System.Drawing.Color.Transparent
        Me.FormulaTabPage.Controls.Add(Me.bsFormulaPanel)
        Me.FormulaTabPage.ForeColor = System.Drawing.Color.Black
        Me.FormulaTabPage.Location = New System.Drawing.Point(4, 22)
        Me.FormulaTabPage.Name = "FormulaTabPage"
        Me.FormulaTabPage.Size = New System.Drawing.Size(691, 363)
        Me.FormulaTabPage.TabIndex = 0
        Me.FormulaTabPage.Text = "Formula Definition"
        Me.FormulaTabPage.UseVisualStyleBackColor = True
        '
        'bsFormulaPanel
        '
        Me.bsFormulaPanel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsFormulaPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.bsFormulaPanel.Controls.Add(Me.bsCalTestFormula)
        Me.bsFormulaPanel.Location = New System.Drawing.Point(0, 0)
        Me.bsFormulaPanel.Name = "bsFormulaPanel"
        Me.bsFormulaPanel.Size = New System.Drawing.Size(691, 363)
        Me.bsFormulaPanel.TabIndex = 1
        '
        'bsCalTestFormula
        '
        Me.bsCalTestFormula.AutoScroll = True
        Me.bsCalTestFormula.AutoSize = True
        Me.bsCalTestFormula.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.bsCalTestFormula.BackColor = System.Drawing.Color.Gainsboro
        Me.bsCalTestFormula.CalculatedTestsTitle = "Calculated Tests:"
        Me.bsCalTestFormula.CancelImage = ""
        Me.bsCalTestFormula.CheckImage = ""
        Me.bsCalTestFormula.ClearFormulaToolTip = ""
        Me.bsCalTestFormula.DecimalSeparator = Nothing
        Me.bsCalTestFormula.DelFormulaMemberToolTip = ""
        Me.bsCalTestFormula.FactoryValueMessage = ""
        Me.bsCalTestFormula.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCalTestFormula.ForeColor = System.Drawing.Color.Black
        Me.bsCalTestFormula.FormulaTitle = "Formula:"
        Me.bsCalTestFormula.Location = New System.Drawing.Point(4, 0)
        Me.bsCalTestFormula.Name = "bsCalTestFormula"
        Me.bsCalTestFormula.SampleTypeTitle = "Sample Type:"
        Me.bsCalTestFormula.SelectedSampleType = ""
        Me.bsCalTestFormula.Size = New System.Drawing.Size(681, 364)
        Me.bsCalTestFormula.StandardTestsTitle = "Standard Tests:"
        Me.bsCalTestFormula.TabIndex = 0
        Me.bsCalTestFormula.WarningImage = ""
        '
        'RefRangesTabPage
        '
        Me.RefRangesTabPage.Controls.Add(Me.bsRefRangesPanel)
        Me.RefRangesTabPage.Location = New System.Drawing.Point(4, 22)
        Me.RefRangesTabPage.Name = "RefRangesTabPage"
        Me.RefRangesTabPage.Size = New System.Drawing.Size(691, 363)
        Me.RefRangesTabPage.TabIndex = 1
        Me.RefRangesTabPage.Text = "Reference Ranges"
        Me.RefRangesTabPage.UseVisualStyleBackColor = True
        '
        'bsRefRangesPanel
        '
        Me.bsRefRangesPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.bsRefRangesPanel.Controls.Add(Me.bsTestRefRanges)
        Me.bsRefRangesPanel.ForeColor = System.Drawing.Color.Black
        Me.bsRefRangesPanel.Location = New System.Drawing.Point(0, 0)
        Me.bsRefRangesPanel.Name = "bsRefRangesPanel"
        Me.bsRefRangesPanel.Size = New System.Drawing.Size(691, 363)
        Me.bsRefRangesPanel.TabIndex = 1
        '
        'bsTestRefRanges
        '
        Me.bsTestRefRanges.ActiveRangeType = ""
        Me.bsTestRefRanges.BackColor = System.Drawing.Color.Gainsboro
        Me.bsTestRefRanges.ChangeSampleTypeDuringEdition = False
        Me.bsTestRefRanges.ChangesMade = False
        Me.bsTestRefRanges.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsTestRefRanges.ForeColor = System.Drawing.Color.Black
        Me.bsTestRefRanges.Location = New System.Drawing.Point(23, 13)
        Me.bsTestRefRanges.Name = "bsTestRefRanges"
        Me.bsTestRefRanges.Size = New System.Drawing.Size(646, 249)
        Me.bsTestRefRanges.TabIndex = 1
        '
        'bsPrintExpTestCheckbox
        '
        Me.bsPrintExpTestCheckbox.AutoSize = True
        Me.bsPrintExpTestCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsPrintExpTestCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsPrintExpTestCheckbox.Location = New System.Drawing.Point(404, 143)
        Me.bsPrintExpTestCheckbox.Name = "bsPrintExpTestCheckbox"
        Me.bsPrintExpTestCheckbox.Size = New System.Drawing.Size(159, 17)
        Me.bsPrintExpTestCheckbox.TabIndex = 10
        Me.bsPrintExpTestCheckbox.Text = "Print Experimental Test"
        Me.bsPrintExpTestCheckbox.UseVisualStyleBackColor = True
        '
        'bsSampleGroupBox
        '
        Me.bsSampleGroupBox.Controls.Add(Me.bsSampleComboBox)
        Me.bsSampleGroupBox.Controls.Add(Me.bsMultipleRadioButton)
        Me.bsSampleGroupBox.Controls.Add(Me.bsUniqueRadioButton)
        Me.bsSampleGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSampleGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleGroupBox.Location = New System.Drawing.Point(10, 88)
        Me.bsSampleGroupBox.Name = "bsSampleGroupBox"
        Me.bsSampleGroupBox.Size = New System.Drawing.Size(345, 72)
        Me.bsSampleGroupBox.TabIndex = 4
        Me.bsSampleGroupBox.TabStop = False
        Me.bsSampleGroupBox.Text = "Sample Type"
        '
        'bsSampleComboBox
        '
        Me.bsSampleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsSampleComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSampleComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleComboBox.FormattingEnabled = True
        Me.bsSampleComboBox.Location = New System.Drawing.Point(119, 20)
        Me.bsSampleComboBox.Name = "bsSampleComboBox"
        Me.bsSampleComboBox.Size = New System.Drawing.Size(203, 21)
        Me.bsSampleComboBox.TabIndex = 6
        '
        'bsMultipleRadioButton
        '
        Me.bsMultipleRadioButton.AutoSize = True
        Me.bsMultipleRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsMultipleRadioButton.ForeColor = System.Drawing.Color.Black
        Me.bsMultipleRadioButton.Location = New System.Drawing.Point(20, 40)
        Me.bsMultipleRadioButton.Name = "bsMultipleRadioButton"
        Me.bsMultipleRadioButton.Size = New System.Drawing.Size(68, 17)
        Me.bsMultipleRadioButton.TabIndex = 7
        Me.bsMultipleRadioButton.TabStop = True
        Me.bsMultipleRadioButton.Text = "Multiple"
        Me.bsMultipleRadioButton.UseVisualStyleBackColor = True
        '
        'bsUniqueRadioButton
        '
        Me.bsUniqueRadioButton.AutoSize = True
        Me.bsUniqueRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsUniqueRadioButton.ForeColor = System.Drawing.Color.Black
        Me.bsUniqueRadioButton.Location = New System.Drawing.Point(20, 20)
        Me.bsUniqueRadioButton.Name = "bsUniqueRadioButton"
        Me.bsUniqueRadioButton.Size = New System.Drawing.Size(64, 17)
        Me.bsUniqueRadioButton.TabIndex = 5
        Me.bsUniqueRadioButton.TabStop = True
        Me.bsUniqueRadioButton.Text = "Unique"
        Me.bsUniqueRadioButton.UseVisualStyleBackColor = True
        '
        'bsFullNameTextbox
        '
        Me.bsFullNameTextbox.BackColor = System.Drawing.Color.Khaki
        Me.bsFullNameTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.bsFullNameTextbox.DecimalsValues = False
        Me.bsFullNameTextbox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFullNameTextbox.ForeColor = System.Drawing.Color.Black
        Me.bsFullNameTextbox.IsNumeric = False
        Me.bsFullNameTextbox.Location = New System.Drawing.Point(10, 58)
        Me.bsFullNameTextbox.Mandatory = True
        Me.bsFullNameTextbox.MaxLength = 16
        Me.bsFullNameTextbox.Name = "bsFullNameTextbox"
        Me.bsFullNameTextbox.Size = New System.Drawing.Size(345, 21)
        Me.bsFullNameTextbox.TabIndex = 2
        Me.bsFullNameTextbox.WordWrap = False
        '
        'bsFullNameLabel
        '
        Me.bsFullNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsFullNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFullNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsFullNameLabel.Location = New System.Drawing.Point(10, 40)
        Me.bsFullNameLabel.Name = "bsFullNameLabel"
        Me.bsFullNameLabel.Size = New System.Drawing.Size(345, 13)
        Me.bsFullNameLabel.TabIndex = 21
        Me.bsFullNameLabel.Text = "Name:"
        Me.bsFullNameLabel.Title = False
        '
        'bsDecimalsNumericUpDown
        '
        Me.bsDecimalsNumericUpDown.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDecimalsNumericUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsDecimalsNumericUpDown.Location = New System.Drawing.Point(634, 106)
        Me.bsDecimalsNumericUpDown.Name = "bsDecimalsNumericUpDown"
        Me.bsDecimalsNumericUpDown.Size = New System.Drawing.Size(72, 21)
        Me.bsDecimalsNumericUpDown.TabIndex = 9
        Me.bsDecimalsNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsDecimalsLabel
        '
        Me.bsDecimalsLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDecimalsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDecimalsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDecimalsLabel.Location = New System.Drawing.Point(634, 88)
        Me.bsDecimalsLabel.Name = "bsDecimalsLabel"
        Me.bsDecimalsLabel.Size = New System.Drawing.Size(75, 13)
        Me.bsDecimalsLabel.TabIndex = 19
        Me.bsDecimalsLabel.Text = "Decimals:"
        Me.bsDecimalsLabel.Title = False
        '
        'bsUnitComboBox
        '
        Me.bsUnitComboBox.BackColor = System.Drawing.Color.White
        Me.bsUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsUnitComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsUnitComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsUnitComboBox.FormattingEnabled = True
        Me.bsUnitComboBox.Location = New System.Drawing.Point(404, 106)
        Me.bsUnitComboBox.Name = "bsUnitComboBox"
        Me.bsUnitComboBox.Size = New System.Drawing.Size(128, 21)
        Me.bsUnitComboBox.TabIndex = 8
        '
        'bsUnitLabel
        '
        Me.bsUnitLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsUnitLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsUnitLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUnitLabel.Location = New System.Drawing.Point(404, 88)
        Me.bsUnitLabel.Name = "bsUnitLabel"
        Me.bsUnitLabel.Size = New System.Drawing.Size(175, 13)
        Me.bsUnitLabel.TabIndex = 17
        Me.bsUnitLabel.Text = "Unit:"
        Me.bsUnitLabel.Title = False
        '
        'bsNameTextbox
        '
        Me.bsNameTextbox.BackColor = System.Drawing.Color.Khaki
        Me.bsNameTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.bsNameTextbox.DecimalsValues = False
        Me.bsNameTextbox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNameTextbox.ForeColor = System.Drawing.Color.Black
        Me.bsNameTextbox.IsNumeric = False
        Me.bsNameTextbox.Location = New System.Drawing.Point(404, 58)
        Me.bsNameTextbox.Mandatory = True
        Me.bsNameTextbox.MaxLength = 8
        Me.bsNameTextbox.Name = "bsNameTextbox"
        Me.bsNameTextbox.Size = New System.Drawing.Size(175, 21)
        Me.bsNameTextbox.TabIndex = 3
        Me.bsNameTextbox.WordWrap = False
        '
        'bsNameLabel
        '
        Me.bsNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNameLabel.Location = New System.Drawing.Point(404, 40)
        Me.bsNameLabel.Name = "bsNameLabel"
        Me.bsNameLabel.Size = New System.Drawing.Size(175, 13)
        Me.bsNameLabel.TabIndex = 15
        Me.bsNameLabel.Text = "Short Name:"
        Me.bsNameLabel.Title = False
        '
        'bsCalTestDefLabel
        '
        Me.bsCalTestDefLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsCalTestDefLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsCalTestDefLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCalTestDefLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsCalTestDefLabel.Name = "bsCalTestDefLabel"
        Me.bsCalTestDefLabel.Size = New System.Drawing.Size(699, 20)
        Me.bsCalTestDefLabel.TabIndex = 14
        Me.bsCalTestDefLabel.Text = "Calculated Test Definition"
        Me.bsCalTestDefLabel.Title = True
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'bsCalcTestListGroupBox
        '
        Me.bsCalcTestListGroupBox.Controls.Add(Me.bsCalcTestListLabel)
        Me.bsCalcTestListGroupBox.Controls.Add(Me.bsCalTestListView)
        Me.bsCalcTestListGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsCalcTestListGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsCalcTestListGroupBox.Name = "bsCalcTestListGroupBox"
        Me.bsCalcTestListGroupBox.Size = New System.Drawing.Size(234, 598)
        Me.bsCalcTestListGroupBox.TabIndex = 39
        Me.bsCalcTestListGroupBox.TabStop = False
        '
        'IProgCalculatedTest
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.Controls.Add(Me.bsCalcTestListGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsNewButton)
        Me.Controls.Add(Me.bsPrintButton)
        Me.Controls.Add(Me.bsEditButton)
        Me.Controls.Add(Me.bsDeleteButton)
        Me.Controls.Add(Me.bsCalTestDefGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IProgCalculatedTest"
        Me.ShowInTaskbar = False
        Me.Text = ""
        Me.bsCalTestDefGroupBox.ResumeLayout(False)
        Me.bsCalTestDefGroupBox.PerformLayout()
        Me.CalculatedTestTabControl.ResumeLayout(False)
        Me.FormulaTabPage.ResumeLayout(False)
        Me.bsFormulaPanel.ResumeLayout(False)
        Me.bsFormulaPanel.PerformLayout()
        Me.RefRangesTabPage.ResumeLayout(False)
        Me.bsRefRangesPanel.ResumeLayout(False)
        Me.bsSampleGroupBox.ResumeLayout(False)
        Me.bsSampleGroupBox.PerformLayout()
        CType(Me.bsDecimalsNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsCalcTestListGroupBox.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsNewButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsEditButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDeleteButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCalTestDefGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsSampleGroupBox As Biosystems.Ax00.Controls.Usercontrols.BSGroupBox
    Friend WithEvents bsFullNameTextbox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsFullNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsDecimalsNumericUpDown As Biosystems.Ax00.Controls.Usercontrols.BSNumericUpDown
    Friend WithEvents bsDecimalsLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsUnitComboBox As Biosystems.Ax00.Controls.Usercontrols.BSComboBox
    Friend WithEvents bsUnitLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsNameTextbox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsCalTestDefLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsPrintExpTestCheckbox As Biosystems.Ax00.Controls.Usercontrols.BSCheckbox
    Friend WithEvents bsSampleComboBox As Biosystems.Ax00.Controls.Usercontrols.BSComboBox
    Friend WithEvents bsMultipleRadioButton As Biosystems.Ax00.Controls.Usercontrols.BSRadioButton
    Friend WithEvents bsUniqueRadioButton As Biosystems.Ax00.Controls.Usercontrols.BSRadioButton
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsCalTestListView As Biosystems.Ax00.Controls.UserControls.BSListView
    Friend WithEvents bsCalcTestListLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsCalcTestListGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents CalculatedTestTabControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents FormulaTabPage As System.Windows.Forms.TabPage
    Friend WithEvents bsCalTestFormula As Biosystems.Ax00.Controls.UserControls.BSFormula
    Friend WithEvents RefRangesTabPage As System.Windows.Forms.TabPage
    Friend WithEvents bsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsRefRangesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsFormulaPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsTestRefRanges As BSReferenceRanges
End Class
