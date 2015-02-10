
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiProgOffSystemTest
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiProgOffSystemTest))
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsNewButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsEditButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsDeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsOffSystemTestListLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsOffSystemTestListView = New Biosystems.Ax00.Controls.UserControls.BSListView()
        Me.bsTestDetailsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsDefaultValueUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.bsDefaultValueLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDefaultValueComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsResultTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsResultTypeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleTypeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsOffSystemTabControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl()
        Me.RefRangesTabPage = New System.Windows.Forms.TabPage()
        Me.bsRefRangesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel()
        Me.bsTestRefRanges = New BSReferenceRanges()
        Me.bsFullNameTextbox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsFullNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsDecimalsUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.bsDecimalsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsUnitComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsUnitLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsShortNameTextbox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsOffSystemTestDefLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.bsCalcTestListGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsCalTestDefLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsLabel2 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsLabel3 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsCustomOrderButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsTestDetailsGroupBox.SuspendLayout()
        CType(Me.bsDefaultValueUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsOffSystemTabControl.SuspendLayout()
        Me.RefRangesTabPage.SuspendLayout()
        Me.bsRefRangesPanel.SuspendLayout()
        CType(Me.bsDecimalsUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.bsPrintButton.Location = New System.Drawing.Point(249, 613)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 18
        Me.bsPrintButton.UseVisualStyleBackColor = True
        Me.bsPrintButton.Visible = False
        '
        'bsOffSystemTestListLabel
        '
        Me.bsOffSystemTestListLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsOffSystemTestListLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsOffSystemTestListLabel.ForeColor = System.Drawing.Color.Black
        Me.bsOffSystemTestListLabel.Location = New System.Drawing.Point(5, 15)
        Me.bsOffSystemTestListLabel.Name = "bsOffSystemTestListLabel"
        Me.bsOffSystemTestListLabel.Size = New System.Drawing.Size(224, 20)
        Me.bsOffSystemTestListLabel.TabIndex = 17
        Me.bsOffSystemTestListLabel.Text = "Off-System Tests"
        Me.bsOffSystemTestListLabel.Title = True
        '
        'bsOffSystemTestListView
        '
        Me.bsOffSystemTestListView.AllowColumnReorder = True
        Me.bsOffSystemTestListView.AutoArrange = False
        Me.bsOffSystemTestListView.BackColor = System.Drawing.Color.White
        Me.bsOffSystemTestListView.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsOffSystemTestListView.ForeColor = System.Drawing.Color.Black
        Me.bsOffSystemTestListView.FullRowSelect = True
        Me.bsOffSystemTestListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.bsOffSystemTestListView.HideSelection = False
        Me.bsOffSystemTestListView.Location = New System.Drawing.Point(5, 40)
        Me.bsOffSystemTestListView.Name = "bsOffSystemTestListView"
        Me.bsOffSystemTestListView.Size = New System.Drawing.Size(224, 550)
        Me.bsOffSystemTestListView.TabIndex = 1
        Me.bsOffSystemTestListView.UseCompatibleStateImageBehavior = False
        Me.bsOffSystemTestListView.View = System.Windows.Forms.View.Details
        '
        'bsTestDetailsGroupBox
        '
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsDefaultValueUpDown)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsDefaultValueLabel)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsDefaultValueComboBox)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsResultTypeLabel)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsResultTypeComboBox)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsSampleTypeLabel)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsSampleTypeComboBox)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsSaveButton)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsCancelButton)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsOffSystemTabControl)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsFullNameTextbox)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsFullNameLabel)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsDecimalsUpDown)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsDecimalsLabel)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsUnitComboBox)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsUnitLabel)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsShortNameTextbox)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsNameLabel)
        Me.bsTestDetailsGroupBox.Controls.Add(Me.bsOffSystemTestDefLabel)
        Me.bsTestDetailsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestDetailsGroupBox.Location = New System.Drawing.Point(249, 10)
        Me.bsTestDetailsGroupBox.Name = "bsTestDetailsGroupBox"
        Me.bsTestDetailsGroupBox.Size = New System.Drawing.Size(719, 598)
        Me.bsTestDetailsGroupBox.TabIndex = 14
        Me.bsTestDetailsGroupBox.TabStop = False
        '
        'bsDefaultValueUpDown
        '
        Me.bsDefaultValueUpDown.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDefaultValueUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsDefaultValueUpDown.Location = New System.Drawing.Point(394, 146)
        Me.bsDefaultValueUpDown.Name = "bsDefaultValueUpDown"
        Me.bsDefaultValueUpDown.Size = New System.Drawing.Size(72, 21)
        Me.bsDefaultValueUpDown.TabIndex = 10
        Me.bsDefaultValueUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.bsDefaultValueUpDown.Visible = False
        '
        'bsDefaultValueLabel
        '
        Me.bsDefaultValueLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDefaultValueLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDefaultValueLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDefaultValueLabel.Location = New System.Drawing.Point(394, 128)
        Me.bsDefaultValueLabel.Name = "bsDefaultValueLabel"
        Me.bsDefaultValueLabel.Size = New System.Drawing.Size(175, 13)
        Me.bsDefaultValueLabel.TabIndex = 47
        Me.bsDefaultValueLabel.Text = "Default value:"
        Me.bsDefaultValueLabel.Title = False
        Me.bsDefaultValueLabel.Visible = False
        '
        'bsDefaultValueComboBox
        '
        Me.bsDefaultValueComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsDefaultValueComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDefaultValueComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsDefaultValueComboBox.FormattingEnabled = True
        Me.bsDefaultValueComboBox.Location = New System.Drawing.Point(394, 146)
        Me.bsDefaultValueComboBox.Name = "bsDefaultValueComboBox"
        Me.bsDefaultValueComboBox.Size = New System.Drawing.Size(203, 21)
        Me.bsDefaultValueComboBox.TabIndex = 11
        Me.bsDefaultValueComboBox.Visible = False
        '
        'bsResultTypeLabel
        '
        Me.bsResultTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsResultTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsResultTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsResultTypeLabel.Location = New System.Drawing.Point(394, 86)
        Me.bsResultTypeLabel.Name = "bsResultTypeLabel"
        Me.bsResultTypeLabel.Size = New System.Drawing.Size(175, 13)
        Me.bsResultTypeLabel.TabIndex = 45
        Me.bsResultTypeLabel.Text = "Result Type:"
        Me.bsResultTypeLabel.Title = False
        '
        'bsResultTypeComboBox
        '
        Me.bsResultTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsResultTypeComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsResultTypeComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsResultTypeComboBox.FormattingEnabled = True
        Me.bsResultTypeComboBox.Location = New System.Drawing.Point(394, 104)
        Me.bsResultTypeComboBox.Name = "bsResultTypeComboBox"
        Me.bsResultTypeComboBox.Size = New System.Drawing.Size(203, 21)
        Me.bsResultTypeComboBox.TabIndex = 7
        '
        'bsSampleTypeLabel
        '
        Me.bsSampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeLabel.Location = New System.Drawing.Point(10, 84)
        Me.bsSampleTypeLabel.Name = "bsSampleTypeLabel"
        Me.bsSampleTypeLabel.Size = New System.Drawing.Size(175, 13)
        Me.bsSampleTypeLabel.TabIndex = 43
        Me.bsSampleTypeLabel.Text = "Sample Type:"
        Me.bsSampleTypeLabel.Title = False
        '
        'bsSampleTypeComboBox
        '
        Me.bsSampleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsSampleTypeComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSampleTypeComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeComboBox.FormattingEnabled = True
        Me.bsSampleTypeComboBox.Location = New System.Drawing.Point(10, 102)
        Me.bsSampleTypeComboBox.Name = "bsSampleTypeComboBox"
        Me.bsSampleTypeComboBox.Size = New System.Drawing.Size(203, 21)
        Me.bsSampleTypeComboBox.TabIndex = 6
        '
        'bsSaveButton
        '
        Me.bsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSaveButton.Location = New System.Drawing.Point(637, 556)
        Me.bsSaveButton.Name = "bsSaveButton"
        Me.bsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveButton.TabIndex = 41
        Me.bsSaveButton.UseVisualStyleBackColor = True
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Location = New System.Drawing.Point(674, 556)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 42
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsOffSystemTabControl
        '
        Me.bsOffSystemTabControl.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsOffSystemTabControl.Controls.Add(Me.RefRangesTabPage)
        Me.bsOffSystemTabControl.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsOffSystemTabControl.Location = New System.Drawing.Point(10, 190)
        Me.bsOffSystemTabControl.Name = "bsOffSystemTabControl"
        Me.bsOffSystemTabControl.SelectedIndex = 0
        Me.bsOffSystemTabControl.Size = New System.Drawing.Size(699, 358)
        Me.bsOffSystemTabControl.TabIndex = 12
        '
        'RefRangesTabPage
        '
        Me.RefRangesTabPage.Controls.Add(Me.bsRefRangesPanel)
        Me.RefRangesTabPage.Location = New System.Drawing.Point(4, 22)
        Me.RefRangesTabPage.Name = "RefRangesTabPage"
        Me.RefRangesTabPage.Size = New System.Drawing.Size(691, 332)
        Me.RefRangesTabPage.TabIndex = 1
        Me.RefRangesTabPage.Text = "Details"
        Me.RefRangesTabPage.UseVisualStyleBackColor = True
        '
        'bsRefRangesPanel
        '
        Me.bsRefRangesPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.bsRefRangesPanel.Controls.Add(Me.bsTestRefRanges)
        Me.bsRefRangesPanel.ForeColor = System.Drawing.Color.Black
        Me.bsRefRangesPanel.Location = New System.Drawing.Point(0, 2)
        Me.bsRefRangesPanel.Name = "bsRefRangesPanel"
        Me.bsRefRangesPanel.Size = New System.Drawing.Size(691, 330)
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
        Me.bsTestRefRanges.Location = New System.Drawing.Point(20, 15)
        Me.bsTestRefRanges.Name = "bsTestRefRanges"
        Me.bsTestRefRanges.Size = New System.Drawing.Size(645, 252)
        Me.bsTestRefRanges.TabIndex = 0
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
        'bsDecimalsUpDown
        '
        Me.bsDecimalsUpDown.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDecimalsUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsDecimalsUpDown.Location = New System.Drawing.Point(283, 146)
        Me.bsDecimalsUpDown.Name = "bsDecimalsUpDown"
        Me.bsDecimalsUpDown.Size = New System.Drawing.Size(72, 21)
        Me.bsDecimalsUpDown.TabIndex = 9
        Me.bsDecimalsUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsDecimalsLabel
        '
        Me.bsDecimalsLabel.AutoSize = True
        Me.bsDecimalsLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDecimalsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDecimalsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDecimalsLabel.Location = New System.Drawing.Point(283, 128)
        Me.bsDecimalsLabel.Name = "bsDecimalsLabel"
        Me.bsDecimalsLabel.Size = New System.Drawing.Size(64, 13)
        Me.bsDecimalsLabel.TabIndex = 19
        Me.bsDecimalsLabel.Text = "Decimals:"
        Me.bsDecimalsLabel.Title = False
        '
        'bsUnitComboBox
        '
        Me.bsUnitComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsUnitComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsUnitComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsUnitComboBox.FormattingEnabled = True
        Me.bsUnitComboBox.Location = New System.Drawing.Point(10, 146)
        Me.bsUnitComboBox.Name = "bsUnitComboBox"
        Me.bsUnitComboBox.Size = New System.Drawing.Size(203, 21)
        Me.bsUnitComboBox.TabIndex = 8
        '
        'bsUnitLabel
        '
        Me.bsUnitLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsUnitLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsUnitLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUnitLabel.Location = New System.Drawing.Point(10, 128)
        Me.bsUnitLabel.Name = "bsUnitLabel"
        Me.bsUnitLabel.Size = New System.Drawing.Size(175, 13)
        Me.bsUnitLabel.TabIndex = 17
        Me.bsUnitLabel.Text = "Unit:"
        Me.bsUnitLabel.Title = False
        '
        'bsShortNameTextbox
        '
        Me.bsShortNameTextbox.BackColor = System.Drawing.Color.Khaki
        Me.bsShortNameTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.bsShortNameTextbox.DecimalsValues = False
        Me.bsShortNameTextbox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsShortNameTextbox.ForeColor = System.Drawing.Color.Black
        Me.bsShortNameTextbox.IsNumeric = False
        Me.bsShortNameTextbox.Location = New System.Drawing.Point(394, 58)
        Me.bsShortNameTextbox.Mandatory = True
        Me.bsShortNameTextbox.MaxLength = 8
        Me.bsShortNameTextbox.Name = "bsShortNameTextbox"
        Me.bsShortNameTextbox.Size = New System.Drawing.Size(203, 21)
        Me.bsShortNameTextbox.TabIndex = 3
        Me.bsShortNameTextbox.WordWrap = False
        '
        'bsNameLabel
        '
        Me.bsNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNameLabel.Location = New System.Drawing.Point(394, 40)
        Me.bsNameLabel.Name = "bsNameLabel"
        Me.bsNameLabel.Size = New System.Drawing.Size(175, 13)
        Me.bsNameLabel.TabIndex = 15
        Me.bsNameLabel.Text = "Short Name:"
        Me.bsNameLabel.Title = False
        '
        'bsOffSystemTestDefLabel
        '
        Me.bsOffSystemTestDefLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsOffSystemTestDefLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsOffSystemTestDefLabel.ForeColor = System.Drawing.Color.Black
        Me.bsOffSystemTestDefLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsOffSystemTestDefLabel.Name = "bsOffSystemTestDefLabel"
        Me.bsOffSystemTestDefLabel.Size = New System.Drawing.Size(699, 20)
        Me.bsOffSystemTestDefLabel.TabIndex = 14
        Me.bsOffSystemTestDefLabel.Text = "Off-System Test Parameters Programming"
        Me.bsOffSystemTestDefLabel.Title = True
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'bsCalcTestListGroupBox
        '
        Me.bsCalcTestListGroupBox.Controls.Add(Me.bsOffSystemTestListLabel)
        Me.bsCalcTestListGroupBox.Controls.Add(Me.bsOffSystemTestListView)
        Me.bsCalcTestListGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsCalcTestListGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsCalcTestListGroupBox.Name = "bsCalcTestListGroupBox"
        Me.bsCalcTestListGroupBox.Size = New System.Drawing.Size(234, 598)
        Me.bsCalcTestListGroupBox.TabIndex = 39
        Me.bsCalcTestListGroupBox.TabStop = False
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
        Me.bsCalTestDefLabel.Text = "Off-System Test Parameters Programming"
        Me.bsCalTestDefLabel.Title = True
        '
        'BsLabel1
        '
        Me.BsLabel1.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel1.ForeColor = System.Drawing.Color.Black
        Me.BsLabel1.Location = New System.Drawing.Point(10, 84)
        Me.BsLabel1.Name = "BsLabel1"
        Me.BsLabel1.Size = New System.Drawing.Size(175, 13)
        Me.BsLabel1.TabIndex = 43
        Me.BsLabel1.Text = "Sample Type:"
        Me.BsLabel1.Title = False
        '
        'BsLabel2
        '
        Me.BsLabel2.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel2.ForeColor = System.Drawing.Color.Black
        Me.BsLabel2.Location = New System.Drawing.Point(10, 128)
        Me.BsLabel2.Name = "BsLabel2"
        Me.BsLabel2.Size = New System.Drawing.Size(175, 13)
        Me.BsLabel2.TabIndex = 45
        Me.BsLabel2.Text = "Result Type:"
        Me.BsLabel2.Title = False
        '
        'BsLabel3
        '
        Me.BsLabel3.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel3.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel3.ForeColor = System.Drawing.Color.Black
        Me.BsLabel3.Location = New System.Drawing.Point(394, 128)
        Me.BsLabel3.Name = "BsLabel3"
        Me.BsLabel3.Size = New System.Drawing.Size(175, 13)
        Me.BsLabel3.TabIndex = 47
        Me.BsLabel3.Text = "Default values:"
        Me.BsLabel3.Title = False
        Me.BsLabel3.Visible = False
        '
        'BsCustomOrderButton
        '
        Me.BsCustomOrderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsCustomOrderButton.Location = New System.Drawing.Point(212, 613)
        Me.BsCustomOrderButton.Name = "BsCustomOrderButton"
        Me.BsCustomOrderButton.Size = New System.Drawing.Size(32, 32)
        Me.BsCustomOrderButton.TabIndex = 17
        Me.BsCustomOrderButton.UseVisualStyleBackColor = True
        '
        'IProgOffSystemTest
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(970, 685)
        Me.Controls.Add(Me.BsCustomOrderButton)
        Me.Controls.Add(Me.bsCalcTestListGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsNewButton)
        Me.Controls.Add(Me.bsPrintButton)
        Me.Controls.Add(Me.bsEditButton)
        Me.Controls.Add(Me.bsDeleteButton)
        Me.Controls.Add(Me.bsTestDetailsGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiProgOffSystemTest"
        Me.ShowInTaskbar = False
        Me.Text = ""
        Me.bsTestDetailsGroupBox.ResumeLayout(False)
        Me.bsTestDetailsGroupBox.PerformLayout()
        CType(Me.bsDefaultValueUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsOffSystemTabControl.ResumeLayout(False)
        Me.RefRangesTabPage.ResumeLayout(False)
        Me.bsRefRangesPanel.ResumeLayout(False)
        CType(Me.bsDecimalsUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsCalcTestListGroupBox.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsNewButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsEditButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDeleteButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTestDetailsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsFullNameTextbox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsFullNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsDecimalsUpDown As Biosystems.Ax00.Controls.Usercontrols.BSNumericUpDown
    Friend WithEvents bsDecimalsLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsUnitComboBox As Biosystems.Ax00.Controls.Usercontrols.BSComboBox
    Friend WithEvents bsUnitLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsShortNameTextbox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsOffSystemTestDefLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsSampleTypeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsOffSystemTestListView As Biosystems.Ax00.Controls.UserControls.BSListView
    Friend WithEvents bsOffSystemTestListLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsCalcTestListGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsOffSystemTabControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents RefRangesTabPage As System.Windows.Forms.TabPage
    Friend WithEvents bsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDefaultValueLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDefaultValueComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsResultTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsResultTypeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsDefaultValueUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsCalTestDefLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel2 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel3 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRefRangesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsTestRefRanges As BSReferenceRanges
    Friend WithEvents BsCustomOrderButton As Biosystems.Ax00.Controls.UserControls.BSButton
End Class
