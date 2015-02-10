<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiProgPatientData
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
        Dim DataGridViewCellStyle16 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle17 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle18 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle19 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle20 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.bsSearchButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsDeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsClearButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsPatientSearchGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsResetButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsSearchButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsCloseSearchButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsPatientSearchLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsSearchCriteriaGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.BsDateBirthRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.BsAgeRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.bsPatientIDTextbox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsPatientIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLastNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsGenderComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsLastNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsAgeGroupbox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsAgeUnitsComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsSearchAgeUnitLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsAgeToNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.bsAgeFromNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.bsAgeToLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsAgeFromLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsGenderLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDateOfBirthGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsDOBToLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDOBFromLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsToDateTimePicker = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
        Me.bsFromDateTimePicker = New Biosystems.Ax00.Controls.UserControls.BSDateTimePicker
        Me.bsFirstNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsFirstNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsPatientGridGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsPatientsListLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsGridButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsOpenSearchButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsNewButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsEditButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsPatientListDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsSearchCriteriaPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsGridPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsDetailsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsPatientDetailsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsDateOfBirthMaskedTextBox = New DevExpress.XtraEditors.DateEdit
        Me.bsDetailsAgeByTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsGenderDetailsComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsCommentsTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsCommentsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsPerformedByTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsPerformedbyLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDetailsAgeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDetailsLastNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDetailsPatientIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsDetailsDoBLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDetailsGenderLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDetailsPatientIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDetailsLastNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsDetailsFirstNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsDetailsFirstNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsDetailAreaButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.bsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsPatientDetailsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsSelectPatientButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.bsPatientSearchGroupBox.SuspendLayout()
        Me.bsSearchButtonsPanel.SuspendLayout()
        Me.bsSearchCriteriaGroupBox.SuspendLayout()
        Me.bsAgeGroupbox.SuspendLayout()
        CType(Me.bsAgeToNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsAgeFromNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsDateOfBirthGroupBox.SuspendLayout()
        Me.bsPatientGridGroupBox.SuspendLayout()
        Me.bsGridButtonsPanel.SuspendLayout()
        CType(Me.bsPatientListDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsSearchCriteriaPanel.SuspendLayout()
        Me.bsGridPanel.SuspendLayout()
        Me.bsDetailsPanel.SuspendLayout()
        Me.bsPatientDetailsGroupBox.SuspendLayout()
        CType(Me.bsDateOfBirthMaskedTextBox.Properties.VistaTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsDateOfBirthMaskedTextBox.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsDetailAreaButtonsPanel.SuspendLayout()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsSearchButton
        '
        Me.bsSearchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSearchButton.ForeColor = System.Drawing.Color.Black
        Me.bsSearchButton.Location = New System.Drawing.Point(0, 0)
        Me.bsSearchButton.Name = "bsSearchButton"
        Me.bsSearchButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSearchButton.TabIndex = 11
        Me.bsSearchButton.UseVisualStyleBackColor = True
        '
        'bsDeleteButton
        '
        Me.bsDeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsDeleteButton.Location = New System.Drawing.Point(165, 0)
        Me.bsDeleteButton.Name = "bsDeleteButton"
        Me.bsDeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.bsDeleteButton.TabIndex = 12
        Me.bsDeleteButton.UseVisualStyleBackColor = True
        '
        'bsClearButton
        '
        Me.bsClearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsClearButton.ForeColor = System.Drawing.Color.Black
        Me.bsClearButton.Location = New System.Drawing.Point(0, 0)
        Me.bsClearButton.Name = "bsClearButton"
        Me.bsClearButton.Size = New System.Drawing.Size(32, 32)
        Me.bsClearButton.TabIndex = 15
        Me.bsClearButton.UseVisualStyleBackColor = True
        '
        'bsPatientSearchGroupBox
        '
        Me.bsPatientSearchGroupBox.Controls.Add(Me.bsResetButton)
        Me.bsPatientSearchGroupBox.Controls.Add(Me.bsSearchButtonsPanel)
        Me.bsPatientSearchGroupBox.Controls.Add(Me.bsPatientSearchLabel)
        Me.bsPatientSearchGroupBox.Controls.Add(Me.bsSearchCriteriaGroupBox)
        Me.bsPatientSearchGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsPatientSearchGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsPatientSearchGroupBox.Margin = New System.Windows.Forms.Padding(4, 3, 3, 3)
        Me.bsPatientSearchGroupBox.Name = "bsPatientSearchGroupBox"
        Me.bsPatientSearchGroupBox.Size = New System.Drawing.Size(955, 295)
        Me.bsPatientSearchGroupBox.TabIndex = 1
        Me.bsPatientSearchGroupBox.TabStop = False
        '
        'bsResetButton
        '
        Me.bsResetButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsResetButton.Enabled = False
        Me.bsResetButton.ForeColor = System.Drawing.Color.Black
        Me.bsResetButton.Location = New System.Drawing.Point(10, 254)
        Me.bsResetButton.Name = "bsResetButton"
        Me.bsResetButton.Size = New System.Drawing.Size(32, 32)
        Me.bsResetButton.TabIndex = 13
        Me.bsResetButton.UseVisualStyleBackColor = True
        '
        'bsSearchButtonsPanel
        '
        Me.bsSearchButtonsPanel.Controls.Add(Me.bsSearchButton)
        Me.bsSearchButtonsPanel.Controls.Add(Me.bsCloseSearchButton)
        Me.bsSearchButtonsPanel.Location = New System.Drawing.Point(876, 254)
        Me.bsSearchButtonsPanel.Name = "bsSearchButtonsPanel"
        Me.bsSearchButtonsPanel.Size = New System.Drawing.Size(69, 32)
        Me.bsSearchButtonsPanel.TabIndex = 48
        '
        'bsCloseSearchButton
        '
        Me.bsCloseSearchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsCloseSearchButton.Location = New System.Drawing.Point(37, 0)
        Me.bsCloseSearchButton.Name = "bsCloseSearchButton"
        Me.bsCloseSearchButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCloseSearchButton.TabIndex = 12
        Me.bsCloseSearchButton.UseVisualStyleBackColor = True
        '
        'bsPatientSearchLabel
        '
        Me.bsPatientSearchLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsPatientSearchLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsPatientSearchLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPatientSearchLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsPatientSearchLabel.Name = "bsPatientSearchLabel"
        Me.bsPatientSearchLabel.Size = New System.Drawing.Size(935, 20)
        Me.bsPatientSearchLabel.TabIndex = 2
        Me.bsPatientSearchLabel.Text = "Patients Search Criteria"
        Me.bsPatientSearchLabel.Title = True
        '
        'bsSearchCriteriaGroupBox
        '
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.BsDateBirthRadioButton)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.BsAgeRadioButton)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.bsPatientIDTextbox)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.bsPatientIDLabel)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.bsLastNameTextBox)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.bsGenderComboBox)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.bsLastNameLabel)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.bsAgeGroupbox)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.bsGenderLabel)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.bsDateOfBirthGroupBox)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.bsFirstNameTextBox)
        Me.bsSearchCriteriaGroupBox.Controls.Add(Me.bsFirstNameLabel)
        Me.bsSearchCriteriaGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsSearchCriteriaGroupBox.Location = New System.Drawing.Point(10, 40)
        Me.bsSearchCriteriaGroupBox.Name = "bsSearchCriteriaGroupBox"
        Me.bsSearchCriteriaGroupBox.Size = New System.Drawing.Size(935, 204)
        Me.bsSearchCriteriaGroupBox.TabIndex = 35
        Me.bsSearchCriteriaGroupBox.TabStop = False
        '
        'BsDateBirthRadioButton
        '
        Me.BsDateBirthRadioButton.AutoSize = True
        Me.BsDateBirthRadioButton.Location = New System.Drawing.Point(55, 113)
        Me.BsDateBirthRadioButton.Name = "BsDateBirthRadioButton"
        Me.BsDateBirthRadioButton.Size = New System.Drawing.Size(100, 17)
        Me.BsDateBirthRadioButton.TabIndex = 4
        Me.BsDateBirthRadioButton.TabStop = True
        Me.BsDateBirthRadioButton.Text = "Date Of Birth"
        Me.BsDateBirthRadioButton.UseVisualStyleBackColor = True
        '
        'BsAgeRadioButton
        '
        Me.BsAgeRadioButton.AutoSize = True
        Me.BsAgeRadioButton.Location = New System.Drawing.Point(532, 113)
        Me.BsAgeRadioButton.Name = "BsAgeRadioButton"
        Me.BsAgeRadioButton.Size = New System.Drawing.Size(47, 17)
        Me.BsAgeRadioButton.TabIndex = 7
        Me.BsAgeRadioButton.TabStop = True
        Me.BsAgeRadioButton.Text = "Age"
        Me.BsAgeRadioButton.UseVisualStyleBackColor = True
        '
        'bsPatientIDTextbox
        '
        Me.bsPatientIDTextbox.BackColor = System.Drawing.Color.White
        Me.bsPatientIDTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.bsPatientIDTextbox.DecimalsValues = False
        Me.bsPatientIDTextbox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPatientIDTextbox.ForeColor = System.Drawing.Color.Black
        Me.bsPatientIDTextbox.IsNumeric = False
        Me.bsPatientIDTextbox.Location = New System.Drawing.Point(55, 33)
        Me.bsPatientIDTextbox.Mandatory = False
        Me.bsPatientIDTextbox.MaxLength = 16
        Me.bsPatientIDTextbox.Name = "bsPatientIDTextbox"
        Me.bsPatientIDTextbox.Size = New System.Drawing.Size(204, 21)
        Me.bsPatientIDTextbox.TabIndex = 0
        Me.bsPatientIDTextbox.WordWrap = False
        '
        'bsPatientIDLabel
        '
        Me.bsPatientIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsPatientIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPatientIDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPatientIDLabel.Location = New System.Drawing.Point(55, 15)
        Me.bsPatientIDLabel.Name = "bsPatientIDLabel"
        Me.bsPatientIDLabel.Size = New System.Drawing.Size(204, 13)
        Me.bsPatientIDLabel.TabIndex = 3
        Me.bsPatientIDLabel.Text = "Patient ID:"
        Me.bsPatientIDLabel.Title = False
        '
        'bsLastNameTextBox
        '
        Me.bsLastNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsLastNameTextBox.DecimalsValues = False
        Me.bsLastNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLastNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsLastNameTextBox.IsNumeric = False
        Me.bsLastNameTextBox.Location = New System.Drawing.Point(532, 77)
        Me.bsLastNameTextBox.Mandatory = False
        Me.bsLastNameTextBox.MaxLength = 30
        Me.bsLastNameTextBox.Name = "bsLastNameTextBox"
        Me.bsLastNameTextBox.Size = New System.Drawing.Size(348, 21)
        Me.bsLastNameTextBox.TabIndex = 3
        Me.bsLastNameTextBox.WordWrap = False
        '
        'bsGenderComboBox
        '
        Me.bsGenderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsGenderComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsGenderComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsGenderComboBox.FormattingEnabled = True
        Me.bsGenderComboBox.Location = New System.Drawing.Point(532, 33)
        Me.bsGenderComboBox.Name = "bsGenderComboBox"
        Me.bsGenderComboBox.Size = New System.Drawing.Size(160, 21)
        Me.bsGenderComboBox.TabIndex = 1
        '
        'bsLastNameLabel
        '
        Me.bsLastNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLastNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLastNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLastNameLabel.Location = New System.Drawing.Point(532, 59)
        Me.bsLastNameLabel.Name = "bsLastNameLabel"
        Me.bsLastNameLabel.Size = New System.Drawing.Size(350, 13)
        Me.bsLastNameLabel.TabIndex = 10
        Me.bsLastNameLabel.Text = "Last Name:"
        Me.bsLastNameLabel.Title = False
        '
        'bsAgeGroupbox
        '
        Me.bsAgeGroupbox.Controls.Add(Me.bsAgeUnitsComboBox)
        Me.bsAgeGroupbox.Controls.Add(Me.bsSearchAgeUnitLabel)
        Me.bsAgeGroupbox.Controls.Add(Me.bsAgeToNumericUpDown)
        Me.bsAgeGroupbox.Controls.Add(Me.bsAgeFromNumericUpDown)
        Me.bsAgeGroupbox.Controls.Add(Me.bsAgeToLabel)
        Me.bsAgeGroupbox.Controls.Add(Me.bsAgeFromLabel)
        Me.bsAgeGroupbox.ForeColor = System.Drawing.Color.Black
        Me.bsAgeGroupbox.Location = New System.Drawing.Point(532, 125)
        Me.bsAgeGroupbox.Name = "bsAgeGroupbox"
        Me.bsAgeGroupbox.Size = New System.Drawing.Size(348, 68)
        Me.bsAgeGroupbox.TabIndex = 13
        Me.bsAgeGroupbox.TabStop = False
        '
        'bsAgeUnitsComboBox
        '
        Me.bsAgeUnitsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsAgeUnitsComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAgeUnitsComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsAgeUnitsComboBox.FormattingEnabled = True
        Me.bsAgeUnitsComboBox.Location = New System.Drawing.Point(20, 35)
        Me.bsAgeUnitsComboBox.Name = "bsAgeUnitsComboBox"
        Me.bsAgeUnitsComboBox.Size = New System.Drawing.Size(98, 21)
        Me.bsAgeUnitsComboBox.TabIndex = 8
        '
        'bsSearchAgeUnitLabel
        '
        Me.bsSearchAgeUnitLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSearchAgeUnitLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSearchAgeUnitLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSearchAgeUnitLabel.Location = New System.Drawing.Point(20, 15)
        Me.bsSearchAgeUnitLabel.Name = "bsSearchAgeUnitLabel"
        Me.bsSearchAgeUnitLabel.Size = New System.Drawing.Size(98, 13)
        Me.bsSearchAgeUnitLabel.TabIndex = 21
        Me.bsSearchAgeUnitLabel.Text = "Unit:"
        Me.bsSearchAgeUnitLabel.Title = False
        '
        'bsAgeToNumericUpDown
        '
        Me.bsAgeToNumericUpDown.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAgeToNumericUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsAgeToNumericUpDown.Location = New System.Drawing.Point(263, 35)
        Me.bsAgeToNumericUpDown.Name = "bsAgeToNumericUpDown"
        Me.bsAgeToNumericUpDown.Size = New System.Drawing.Size(65, 21)
        Me.bsAgeToNumericUpDown.TabIndex = 10
        Me.bsAgeToNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsAgeFromNumericUpDown
        '
        Me.bsAgeFromNumericUpDown.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAgeFromNumericUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsAgeFromNumericUpDown.Location = New System.Drawing.Point(158, 35)
        Me.bsAgeFromNumericUpDown.Name = "bsAgeFromNumericUpDown"
        Me.bsAgeFromNumericUpDown.Size = New System.Drawing.Size(65, 21)
        Me.bsAgeFromNumericUpDown.TabIndex = 9
        Me.bsAgeFromNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsAgeToLabel
        '
        Me.bsAgeToLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsAgeToLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsAgeToLabel.ForeColor = System.Drawing.Color.Black
        Me.bsAgeToLabel.Location = New System.Drawing.Point(263, 15)
        Me.bsAgeToLabel.Name = "bsAgeToLabel"
        Me.bsAgeToLabel.Size = New System.Drawing.Size(65, 13)
        Me.bsAgeToLabel.TabIndex = 19
        Me.bsAgeToLabel.Text = "To:"
        Me.bsAgeToLabel.Title = False
        '
        'bsAgeFromLabel
        '
        Me.bsAgeFromLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsAgeFromLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsAgeFromLabel.ForeColor = System.Drawing.Color.Black
        Me.bsAgeFromLabel.Location = New System.Drawing.Point(158, 15)
        Me.bsAgeFromLabel.Name = "bsAgeFromLabel"
        Me.bsAgeFromLabel.Size = New System.Drawing.Size(65, 13)
        Me.bsAgeFromLabel.TabIndex = 17
        Me.bsAgeFromLabel.Text = "From:"
        Me.bsAgeFromLabel.Title = False
        '
        'bsGenderLabel
        '
        Me.bsGenderLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsGenderLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsGenderLabel.ForeColor = System.Drawing.Color.Black
        Me.bsGenderLabel.Location = New System.Drawing.Point(532, 15)
        Me.bsGenderLabel.Name = "bsGenderLabel"
        Me.bsGenderLabel.Size = New System.Drawing.Size(160, 15)
        Me.bsGenderLabel.TabIndex = 5
        Me.bsGenderLabel.Text = "Gender:"
        Me.bsGenderLabel.Title = False
        '
        'bsDateOfBirthGroupBox
        '
        Me.bsDateOfBirthGroupBox.Controls.Add(Me.bsDOBToLabel)
        Me.bsDateOfBirthGroupBox.Controls.Add(Me.bsDOBFromLabel)
        Me.bsDateOfBirthGroupBox.Controls.Add(Me.bsToDateTimePicker)
        Me.bsDateOfBirthGroupBox.Controls.Add(Me.bsFromDateTimePicker)
        Me.bsDateOfBirthGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsDateOfBirthGroupBox.Location = New System.Drawing.Point(55, 125)
        Me.bsDateOfBirthGroupBox.Name = "bsDateOfBirthGroupBox"
        Me.bsDateOfBirthGroupBox.Size = New System.Drawing.Size(348, 68)
        Me.bsDateOfBirthGroupBox.TabIndex = 22
        Me.bsDateOfBirthGroupBox.TabStop = False
        '
        'bsDOBToLabel
        '
        Me.bsDOBToLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDOBToLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDOBToLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDOBToLabel.Location = New System.Drawing.Point(199, 15)
        Me.bsDOBToLabel.Name = "bsDOBToLabel"
        Me.bsDOBToLabel.Size = New System.Drawing.Size(132, 13)
        Me.bsDOBToLabel.TabIndex = 25
        Me.bsDOBToLabel.Text = "To:"
        Me.bsDOBToLabel.Title = False
        '
        'bsDOBFromLabel
        '
        Me.bsDOBFromLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDOBFromLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDOBFromLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDOBFromLabel.Location = New System.Drawing.Point(20, 15)
        Me.bsDOBFromLabel.Name = "bsDOBFromLabel"
        Me.bsDOBFromLabel.Size = New System.Drawing.Size(131, 13)
        Me.bsDOBFromLabel.TabIndex = 23
        Me.bsDOBFromLabel.Text = "From:"
        Me.bsDOBFromLabel.Title = False
        '
        'bsToDateTimePicker
        '
        Me.bsToDateTimePicker.CalendarFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsToDateTimePicker.CalendarForeColor = System.Drawing.Color.Black
        Me.bsToDateTimePicker.CalendarMonthBackground = System.Drawing.Color.White
        Me.bsToDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.bsToDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.bsToDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.bsToDateTimePicker.Checked = False
        Me.bsToDateTimePicker.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsToDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.bsToDateTimePicker.Location = New System.Drawing.Point(199, 35)
        Me.bsToDateTimePicker.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsToDateTimePicker.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsToDateTimePicker.Name = "bsToDateTimePicker"
        Me.bsToDateTimePicker.ShowCheckBox = True
        Me.bsToDateTimePicker.Size = New System.Drawing.Size(129, 21)
        Me.bsToDateTimePicker.TabIndex = 6
        '
        'bsFromDateTimePicker
        '
        Me.bsFromDateTimePicker.CalendarFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsFromDateTimePicker.CalendarForeColor = System.Drawing.Color.Black
        Me.bsFromDateTimePicker.CalendarMonthBackground = System.Drawing.Color.White
        Me.bsFromDateTimePicker.CalendarTitleBackColor = System.Drawing.Color.LightSlateGray
        Me.bsFromDateTimePicker.CalendarTitleForeColor = System.Drawing.Color.Black
        Me.bsFromDateTimePicker.CalendarTrailingForeColor = System.Drawing.Color.Silver
        Me.bsFromDateTimePicker.Checked = False
        Me.bsFromDateTimePicker.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsFromDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.bsFromDateTimePicker.Location = New System.Drawing.Point(20, 35)
        Me.bsFromDateTimePicker.MaxDate = New Date(3000, 12, 31, 0, 0, 0, 0)
        Me.bsFromDateTimePicker.MinDate = New Date(1900, 1, 1, 0, 0, 0, 0)
        Me.bsFromDateTimePicker.Name = "bsFromDateTimePicker"
        Me.bsFromDateTimePicker.ShowCheckBox = True
        Me.bsFromDateTimePicker.Size = New System.Drawing.Size(129, 21)
        Me.bsFromDateTimePicker.TabIndex = 5
        '
        'bsFirstNameTextBox
        '
        Me.bsFirstNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsFirstNameTextBox.DecimalsValues = False
        Me.bsFirstNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFirstNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsFirstNameTextBox.IsNumeric = False
        Me.bsFirstNameTextBox.Location = New System.Drawing.Point(55, 77)
        Me.bsFirstNameTextBox.Mandatory = False
        Me.bsFirstNameTextBox.MaxLength = 30
        Me.bsFirstNameTextBox.Name = "bsFirstNameTextBox"
        Me.bsFirstNameTextBox.Size = New System.Drawing.Size(348, 21)
        Me.bsFirstNameTextBox.TabIndex = 2
        Me.bsFirstNameTextBox.WordWrap = False
        '
        'bsFirstNameLabel
        '
        Me.bsFirstNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsFirstNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFirstNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsFirstNameLabel.Location = New System.Drawing.Point(55, 59)
        Me.bsFirstNameLabel.Name = "bsFirstNameLabel"
        Me.bsFirstNameLabel.Size = New System.Drawing.Size(348, 13)
        Me.bsFirstNameLabel.TabIndex = 8
        Me.bsFirstNameLabel.Text = "First Name:"
        Me.bsFirstNameLabel.Title = False
        '
        'bsPatientGridGroupBox
        '
        Me.bsPatientGridGroupBox.Controls.Add(Me.bsPatientsListLabel)
        Me.bsPatientGridGroupBox.Controls.Add(Me.bsGridButtonsPanel)
        Me.bsPatientGridGroupBox.Controls.Add(Me.bsPatientListDataGridView)
        Me.bsPatientGridGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsPatientGridGroupBox.Location = New System.Drawing.Point(12, 10)
        Me.bsPatientGridGroupBox.Margin = New System.Windows.Forms.Padding(4, 3, 3, 3)
        Me.bsPatientGridGroupBox.Name = "bsPatientGridGroupBox"
        Me.bsPatientGridGroupBox.Size = New System.Drawing.Size(955, 343)
        Me.bsPatientGridGroupBox.TabIndex = 44
        Me.bsPatientGridGroupBox.TabStop = False
        '
        'bsPatientsListLabel
        '
        Me.bsPatientsListLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsPatientsListLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsPatientsListLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPatientsListLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsPatientsListLabel.Name = "bsPatientsListLabel"
        Me.bsPatientsListLabel.Size = New System.Drawing.Size(935, 20)
        Me.bsPatientsListLabel.TabIndex = 13
        Me.bsPatientsListLabel.Text = "Patients List"
        Me.bsPatientsListLabel.Title = True
        '
        'bsGridButtonsPanel
        '
        Me.bsGridButtonsPanel.Controls.Add(Me.bsOpenSearchButton)
        Me.bsGridButtonsPanel.Controls.Add(Me.bsNewButton)
        Me.bsGridButtonsPanel.Controls.Add(Me.bsEditButton)
        Me.bsGridButtonsPanel.Controls.Add(Me.bsDeleteButton)
        Me.bsGridButtonsPanel.Controls.Add(Me.bsClearButton)
        Me.bsGridButtonsPanel.Controls.Add(Me.bsPrintButton)
        Me.bsGridButtonsPanel.Location = New System.Drawing.Point(711, 305)
        Me.bsGridButtonsPanel.Name = "bsGridButtonsPanel"
        Me.bsGridButtonsPanel.Size = New System.Drawing.Size(235, 32)
        Me.bsGridButtonsPanel.TabIndex = 42
        '
        'bsOpenSearchButton
        '
        Me.bsOpenSearchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsOpenSearchButton.Location = New System.Drawing.Point(37, 0)
        Me.bsOpenSearchButton.Name = "bsOpenSearchButton"
        Me.bsOpenSearchButton.Size = New System.Drawing.Size(32, 32)
        Me.bsOpenSearchButton.TabIndex = 14
        Me.bsOpenSearchButton.UseVisualStyleBackColor = True
        '
        'bsNewButton
        '
        Me.bsNewButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsNewButton.Location = New System.Drawing.Point(91, 0)
        Me.bsNewButton.Name = "bsNewButton"
        Me.bsNewButton.Size = New System.Drawing.Size(32, 32)
        Me.bsNewButton.TabIndex = 10
        Me.bsNewButton.UseVisualStyleBackColor = True
        '
        'bsEditButton
        '
        Me.bsEditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsEditButton.Location = New System.Drawing.Point(128, 0)
        Me.bsEditButton.Name = "bsEditButton"
        Me.bsEditButton.Size = New System.Drawing.Size(32, 32)
        Me.bsEditButton.TabIndex = 11
        Me.bsEditButton.UseVisualStyleBackColor = True
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Location = New System.Drawing.Point(202, 0)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 13
        Me.bsPrintButton.UseVisualStyleBackColor = True
        '
        'bsPatientListDataGridView
        '
        Me.bsPatientListDataGridView.AllowUserToAddRows = False
        Me.bsPatientListDataGridView.AllowUserToDeleteRows = False
        Me.bsPatientListDataGridView.AllowUserToResizeColumns = False
        Me.bsPatientListDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle16.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle16.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle16.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientListDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle16
        Me.bsPatientListDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsPatientListDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsPatientListDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle17.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle17.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle17.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientListDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle17
        Me.bsPatientListDataGridView.ColumnHeadersHeight = 20
        Me.bsPatientListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle18.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle18.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle18.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle18.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle18.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientListDataGridView.DefaultCellStyle = DataGridViewCellStyle18
        Me.bsPatientListDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsPatientListDataGridView.EnterToTab = False
        Me.bsPatientListDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsPatientListDataGridView.Location = New System.Drawing.Point(10, 40)
        Me.bsPatientListDataGridView.Name = "bsPatientListDataGridView"
        Me.bsPatientListDataGridView.ReadOnly = True
        Me.bsPatientListDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle19.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle19.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle19.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle19.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle19.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientListDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle19
        Me.bsPatientListDataGridView.RowHeadersVisible = False
        Me.bsPatientListDataGridView.RowHeadersWidth = 20
        Me.bsPatientListDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle20.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle20.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle20.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle20.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle20.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientListDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle20
        Me.bsPatientListDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsPatientListDataGridView.Size = New System.Drawing.Size(935, 255)
        Me.bsPatientListDataGridView.TabIndex = 9
        Me.bsPatientListDataGridView.TabToEnter = False
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.ForeColor = System.Drawing.Color.Green
        Me.bsExitButton.Location = New System.Drawing.Point(923, 614)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 41
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsSearchCriteriaPanel
        '
        Me.bsSearchCriteriaPanel.Controls.Add(Me.bsPatientSearchGroupBox)
        Me.bsSearchCriteriaPanel.Location = New System.Drawing.Point(0, 0)
        Me.bsSearchCriteriaPanel.Name = "bsSearchCriteriaPanel"
        Me.bsSearchCriteriaPanel.Padding = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.bsSearchCriteriaPanel.Size = New System.Drawing.Size(978, 315)
        Me.bsSearchCriteriaPanel.TabIndex = 38
        '
        'bsGridPanel
        '
        Me.bsGridPanel.Controls.Add(Me.bsPatientGridGroupBox)
        Me.bsGridPanel.Location = New System.Drawing.Point(0, 0)
        Me.bsGridPanel.Name = "bsGridPanel"
        Me.bsGridPanel.Padding = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.bsGridPanel.Size = New System.Drawing.Size(978, 361)
        Me.bsGridPanel.TabIndex = 45
        '
        'bsDetailsPanel
        '
        Me.bsDetailsPanel.Controls.Add(Me.bsPatientDetailsGroupBox)
        Me.bsDetailsPanel.Location = New System.Drawing.Point(0, 359)
        Me.bsDetailsPanel.Name = "bsDetailsPanel"
        Me.bsDetailsPanel.Padding = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.bsDetailsPanel.Size = New System.Drawing.Size(965, 254)
        Me.bsDetailsPanel.TabIndex = 46
        '
        'bsPatientDetailsGroupBox
        '
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDateOfBirthMaskedTextBox)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailsAgeByTextBox)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsGenderDetailsComboBox)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsCommentsTextBox)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsCommentsLabel)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsPerformedByTextBox)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsPerformedbyLabel)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailsAgeLabel)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailsLastNameLabel)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailsPatientIDTextBox)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailsDoBLabel)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailsGenderLabel)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailsPatientIDLabel)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailsLastNameTextBox)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailsFirstNameTextBox)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailsFirstNameLabel)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsDetailAreaButtonsPanel)
        Me.bsPatientDetailsGroupBox.Controls.Add(Me.bsPatientDetailsLabel)
        Me.bsPatientDetailsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsPatientDetailsGroupBox.Location = New System.Drawing.Point(10, 7)
        Me.bsPatientDetailsGroupBox.Margin = New System.Windows.Forms.Padding(2)
        Me.bsPatientDetailsGroupBox.Name = "bsPatientDetailsGroupBox"
        Me.bsPatientDetailsGroupBox.Padding = New System.Windows.Forms.Padding(2)
        Me.bsPatientDetailsGroupBox.Size = New System.Drawing.Size(955, 239)
        Me.bsPatientDetailsGroupBox.TabIndex = 44
        Me.bsPatientDetailsGroupBox.TabStop = False
        '
        'bsDateOfBirthMaskedTextBox
        '
        Me.bsDateOfBirthMaskedTextBox.EditValue = Nothing
        Me.bsDateOfBirthMaskedTextBox.Location = New System.Drawing.Point(226, 98)
        Me.bsDateOfBirthMaskedTextBox.Name = "bsDateOfBirthMaskedTextBox"
        Me.bsDateOfBirthMaskedTextBox.Properties.Appearance.BackColor = System.Drawing.Color.White
        Me.bsDateOfBirthMaskedTextBox.Properties.Appearance.ForeColor = System.Drawing.Color.Black
        Me.bsDateOfBirthMaskedTextBox.Properties.Appearance.Options.UseBackColor = True
        Me.bsDateOfBirthMaskedTextBox.Properties.Appearance.Options.UseForeColor = True
        Me.bsDateOfBirthMaskedTextBox.Properties.AppearanceDisabled.BackColor = System.Drawing.Color.WhiteSmoke
        Me.bsDateOfBirthMaskedTextBox.Properties.AppearanceDisabled.ForeColor = System.Drawing.Color.DarkGray
        Me.bsDateOfBirthMaskedTextBox.Properties.AppearanceDisabled.Options.UseBackColor = True
        Me.bsDateOfBirthMaskedTextBox.Properties.AppearanceDisabled.Options.UseForeColor = True
        Me.bsDateOfBirthMaskedTextBox.Properties.LookAndFeel.UseDefaultLookAndFeel = False
        Me.bsDateOfBirthMaskedTextBox.Properties.VistaTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton})
        Me.bsDateOfBirthMaskedTextBox.Size = New System.Drawing.Size(84, 20)
        Me.bsDateOfBirthMaskedTextBox.TabIndex = 54
        '
        'bsDetailsAgeByTextBox
        '
        Me.bsDetailsAgeByTextBox.BackColor = System.Drawing.Color.White
        Me.bsDetailsAgeByTextBox.DecimalsValues = False
        Me.bsDetailsAgeByTextBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDetailsAgeByTextBox.ForeColor = System.Drawing.Color.DarkGray
        Me.bsDetailsAgeByTextBox.IsNumeric = False
        Me.bsDetailsAgeByTextBox.Location = New System.Drawing.Point(400, 97)
        Me.bsDetailsAgeByTextBox.Mandatory = False
        Me.bsDetailsAgeByTextBox.Name = "bsDetailsAgeByTextBox"
        Me.bsDetailsAgeByTextBox.ReadOnly = True
        Me.bsDetailsAgeByTextBox.Size = New System.Drawing.Size(174, 21)
        Me.bsDetailsAgeByTextBox.TabIndex = 55
        Me.bsDetailsAgeByTextBox.TabStop = False
        Me.bsDetailsAgeByTextBox.WordWrap = False
        '
        'bsGenderDetailsComboBox
        '
        Me.bsGenderDetailsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsGenderDetailsComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsGenderDetailsComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsGenderDetailsComboBox.FormattingEnabled = True
        Me.bsGenderDetailsComboBox.Location = New System.Drawing.Point(10, 97)
        Me.bsGenderDetailsComboBox.Name = "bsGenderDetailsComboBox"
        Me.bsGenderDetailsComboBox.Size = New System.Drawing.Size(170, 21)
        Me.bsGenderDetailsComboBox.TabIndex = 53
        '
        'bsCommentsTextBox
        '
        Me.bsCommentsTextBox.AcceptsReturn = True
        Me.bsCommentsTextBox.BackColor = System.Drawing.Color.White
        Me.bsCommentsTextBox.DecimalsValues = False
        Me.bsCommentsTextBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCommentsTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsCommentsTextBox.IsNumeric = False
        Me.bsCommentsTextBox.Location = New System.Drawing.Point(10, 136)
        Me.bsCommentsTextBox.Mandatory = False
        Me.bsCommentsTextBox.MaxLength = 255
        Me.bsCommentsTextBox.Multiline = True
        Me.bsCommentsTextBox.Name = "bsCommentsTextBox"
        Me.bsCommentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.bsCommentsTextBox.Size = New System.Drawing.Size(935, 54)
        Me.bsCommentsTextBox.TabIndex = 57
        '
        'bsCommentsLabel
        '
        Me.bsCommentsLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsCommentsLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCommentsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsCommentsLabel.Location = New System.Drawing.Point(10, 120)
        Me.bsCommentsLabel.Name = "bsCommentsLabel"
        Me.bsCommentsLabel.Size = New System.Drawing.Size(300, 13)
        Me.bsCommentsLabel.TabIndex = 64
        Me.bsCommentsLabel.Text = "Comments:"
        Me.bsCommentsLabel.Title = False
        '
        'bsPerformedByTextBox
        '
        Me.bsPerformedByTextBox.BackColor = System.Drawing.Color.White
        Me.bsPerformedByTextBox.DecimalsValues = False
        Me.bsPerformedByTextBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsPerformedByTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsPerformedByTextBox.IsNumeric = False
        Me.bsPerformedByTextBox.Location = New System.Drawing.Point(621, 97)
        Me.bsPerformedByTextBox.Mandatory = False
        Me.bsPerformedByTextBox.MaxLength = 30
        Me.bsPerformedByTextBox.Name = "bsPerformedByTextBox"
        Me.bsPerformedByTextBox.Size = New System.Drawing.Size(324, 21)
        Me.bsPerformedByTextBox.TabIndex = 56
        Me.bsPerformedByTextBox.WordWrap = False
        '
        'bsPerformedbyLabel
        '
        Me.bsPerformedbyLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsPerformedbyLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPerformedbyLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPerformedbyLabel.Location = New System.Drawing.Point(621, 81)
        Me.bsPerformedbyLabel.Name = "bsPerformedbyLabel"
        Me.bsPerformedbyLabel.Size = New System.Drawing.Size(328, 13)
        Me.bsPerformedbyLabel.TabIndex = 63
        Me.bsPerformedbyLabel.Text = "Performed by:"
        Me.bsPerformedbyLabel.Title = False
        '
        'bsDetailsAgeLabel
        '
        Me.bsDetailsAgeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDetailsAgeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDetailsAgeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDetailsAgeLabel.Location = New System.Drawing.Point(400, 81)
        Me.bsDetailsAgeLabel.Name = "bsDetailsAgeLabel"
        Me.bsDetailsAgeLabel.Size = New System.Drawing.Size(174, 13)
        Me.bsDetailsAgeLabel.TabIndex = 62
        Me.bsDetailsAgeLabel.Text = "Age:"
        Me.bsDetailsAgeLabel.Title = False
        '
        'bsDetailsLastNameLabel
        '
        Me.bsDetailsLastNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDetailsLastNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDetailsLastNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDetailsLastNameLabel.Location = New System.Drawing.Point(621, 42)
        Me.bsDetailsLastNameLabel.Name = "bsDetailsLastNameLabel"
        Me.bsDetailsLastNameLabel.Size = New System.Drawing.Size(328, 13)
        Me.bsDetailsLastNameLabel.TabIndex = 60
        Me.bsDetailsLastNameLabel.Text = "Last Name:"
        Me.bsDetailsLastNameLabel.Title = False
        '
        'bsDetailsPatientIDTextBox
        '
        Me.bsDetailsPatientIDTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsDetailsPatientIDTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.bsDetailsPatientIDTextBox.DecimalsValues = False
        Me.bsDetailsPatientIDTextBox.Enabled = False
        Me.bsDetailsPatientIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDetailsPatientIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsDetailsPatientIDTextBox.IsNumeric = False
        Me.bsDetailsPatientIDTextBox.Location = New System.Drawing.Point(10, 58)
        Me.bsDetailsPatientIDTextBox.Mandatory = True
        Me.bsDetailsPatientIDTextBox.MaxLength = 30
        Me.bsDetailsPatientIDTextBox.Name = "bsDetailsPatientIDTextBox"
        Me.bsDetailsPatientIDTextBox.Size = New System.Drawing.Size(170, 21)
        Me.bsDetailsPatientIDTextBox.TabIndex = 50
        Me.bsDetailsPatientIDTextBox.WordWrap = False
        '
        'bsDetailsDoBLabel
        '
        Me.bsDetailsDoBLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDetailsDoBLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDetailsDoBLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDetailsDoBLabel.Location = New System.Drawing.Point(226, 81)
        Me.bsDetailsDoBLabel.Name = "bsDetailsDoBLabel"
        Me.bsDetailsDoBLabel.Size = New System.Drawing.Size(110, 13)
        Me.bsDetailsDoBLabel.TabIndex = 61
        Me.bsDetailsDoBLabel.Text = "Date Of Birth:"
        Me.bsDetailsDoBLabel.Title = False
        '
        'bsDetailsGenderLabel
        '
        Me.bsDetailsGenderLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDetailsGenderLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDetailsGenderLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDetailsGenderLabel.Location = New System.Drawing.Point(10, 81)
        Me.bsDetailsGenderLabel.Name = "bsDetailsGenderLabel"
        Me.bsDetailsGenderLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsDetailsGenderLabel.TabIndex = 58
        Me.bsDetailsGenderLabel.Text = "Gender:"
        Me.bsDetailsGenderLabel.Title = False
        '
        'bsDetailsPatientIDLabel
        '
        Me.bsDetailsPatientIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDetailsPatientIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDetailsPatientIDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDetailsPatientIDLabel.Location = New System.Drawing.Point(10, 42)
        Me.bsDetailsPatientIDLabel.Name = "bsDetailsPatientIDLabel"
        Me.bsDetailsPatientIDLabel.Size = New System.Drawing.Size(170, 13)
        Me.bsDetailsPatientIDLabel.TabIndex = 54
        Me.bsDetailsPatientIDLabel.Text = "Patient ID:"
        Me.bsDetailsPatientIDLabel.Title = False
        '
        'bsDetailsLastNameTextBox
        '
        Me.bsDetailsLastNameTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsDetailsLastNameTextBox.DecimalsValues = False
        Me.bsDetailsLastNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDetailsLastNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsDetailsLastNameTextBox.IsNumeric = False
        Me.bsDetailsLastNameTextBox.Location = New System.Drawing.Point(621, 58)
        Me.bsDetailsLastNameTextBox.Mandatory = True
        Me.bsDetailsLastNameTextBox.MaxLength = 30
        Me.bsDetailsLastNameTextBox.Name = "bsDetailsLastNameTextBox"
        Me.bsDetailsLastNameTextBox.Size = New System.Drawing.Size(324, 21)
        Me.bsDetailsLastNameTextBox.TabIndex = 52
        Me.bsDetailsLastNameTextBox.WordWrap = False
        '
        'bsDetailsFirstNameTextBox
        '
        Me.bsDetailsFirstNameTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsDetailsFirstNameTextBox.DecimalsValues = False
        Me.bsDetailsFirstNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDetailsFirstNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsDetailsFirstNameTextBox.IsNumeric = False
        Me.bsDetailsFirstNameTextBox.Location = New System.Drawing.Point(226, 58)
        Me.bsDetailsFirstNameTextBox.Mandatory = True
        Me.bsDetailsFirstNameTextBox.MaxLength = 30
        Me.bsDetailsFirstNameTextBox.Name = "bsDetailsFirstNameTextBox"
        Me.bsDetailsFirstNameTextBox.Size = New System.Drawing.Size(348, 21)
        Me.bsDetailsFirstNameTextBox.TabIndex = 51
        Me.bsDetailsFirstNameTextBox.WordWrap = False
        '
        'bsDetailsFirstNameLabel
        '
        Me.bsDetailsFirstNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDetailsFirstNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDetailsFirstNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDetailsFirstNameLabel.Location = New System.Drawing.Point(226, 42)
        Me.bsDetailsFirstNameLabel.Name = "bsDetailsFirstNameLabel"
        Me.bsDetailsFirstNameLabel.Size = New System.Drawing.Size(348, 13)
        Me.bsDetailsFirstNameLabel.TabIndex = 59
        Me.bsDetailsFirstNameLabel.Text = "First Name:"
        Me.bsDetailsFirstNameLabel.Title = False
        '
        'bsDetailAreaButtonsPanel
        '
        Me.bsDetailAreaButtonsPanel.Controls.Add(Me.bsSaveButton)
        Me.bsDetailAreaButtonsPanel.Controls.Add(Me.bsCancelButton)
        Me.bsDetailAreaButtonsPanel.Location = New System.Drawing.Point(876, 200)
        Me.bsDetailAreaButtonsPanel.Name = "bsDetailAreaButtonsPanel"
        Me.bsDetailAreaButtonsPanel.Size = New System.Drawing.Size(69, 32)
        Me.bsDetailAreaButtonsPanel.TabIndex = 49
        '
        'bsSaveButton
        '
        Me.bsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSaveButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSaveButton.Location = New System.Drawing.Point(0, 0)
        Me.bsSaveButton.Name = "bsSaveButton"
        Me.bsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveButton.TabIndex = 7
        Me.bsSaveButton.UseVisualStyleBackColor = True
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Location = New System.Drawing.Point(37, 0)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 8
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsPatientDetailsLabel
        '
        Me.bsPatientDetailsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsPatientDetailsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsPatientDetailsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPatientDetailsLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsPatientDetailsLabel.Name = "bsPatientDetailsLabel"
        Me.bsPatientDetailsLabel.Size = New System.Drawing.Size(935, 20)
        Me.bsPatientDetailsLabel.TabIndex = 13
        Me.bsPatientDetailsLabel.Text = "Patients Details"
        Me.bsPatientDetailsLabel.Title = True
        '
        'bsSelectPatientButton
        '
        Me.bsSelectPatientButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSelectPatientButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSelectPatientButton.Location = New System.Drawing.Point(885, 614)
        Me.bsSelectPatientButton.Name = "bsSelectPatientButton"
        Me.bsSelectPatientButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSelectPatientButton.TabIndex = 25
        Me.bsSelectPatientButton.UseVisualStyleBackColor = True
        Me.bsSelectPatientButton.Visible = False
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'IProgPatientData
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsSelectPatientButton)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsGridPanel)
        Me.Controls.Add(Me.bsDetailsPanel)
        Me.Controls.Add(Me.bsSearchCriteriaPanel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiProgPatientData"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsPatientSearchGroupBox.ResumeLayout(False)
        Me.bsSearchButtonsPanel.ResumeLayout(False)
        Me.bsSearchCriteriaGroupBox.ResumeLayout(False)
        Me.bsSearchCriteriaGroupBox.PerformLayout()
        Me.bsAgeGroupbox.ResumeLayout(False)
        CType(Me.bsAgeToNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsAgeFromNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsDateOfBirthGroupBox.ResumeLayout(False)
        Me.bsPatientGridGroupBox.ResumeLayout(False)
        Me.bsGridButtonsPanel.ResumeLayout(False)
        CType(Me.bsPatientListDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsSearchCriteriaPanel.ResumeLayout(False)
        Me.bsGridPanel.ResumeLayout(False)
        Me.bsDetailsPanel.ResumeLayout(False)
        Me.bsPatientDetailsGroupBox.ResumeLayout(False)
        Me.bsPatientDetailsGroupBox.PerformLayout()
        CType(Me.bsDateOfBirthMaskedTextBox.Properties.VistaTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsDateOfBirthMaskedTextBox.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsDetailAreaButtonsPanel.ResumeLayout(False)
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsEditButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsNewButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsPatientGridGroupBox As Biosystems.Ax00.Controls.Usercontrols.BSGroupBox
    Friend WithEvents bsPatientSearchGroupBox As Biosystems.Ax00.Controls.Usercontrols.BSGroupBox
    Friend WithEvents bsPatientSearchLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsPatientIDLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsPatientIDTextbox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsFirstNameTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsFirstNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsGenderComboBox As Biosystems.Ax00.Controls.Usercontrols.BSComboBox
    Friend WithEvents bsGenderLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsLastNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsLastNameTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsAgeGroupbox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsDateOfBirthGroupBox As Biosystems.Ax00.Controls.Usercontrols.BSGroupBox
    Friend WithEvents bsAgeToLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAgeFromLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsDOBToLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsDOBFromLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsToDateTimePicker As Biosystems.Ax00.Controls.Usercontrols.BSDateTimePicker
    Friend WithEvents bsFromDateTimePicker As Biosystems.Ax00.Controls.Usercontrols.BSDateTimePicker
    Friend WithEvents bsAgeToNumericUpDown As Biosystems.Ax00.Controls.Usercontrols.BSNumericUpDown
    Friend WithEvents bsAgeFromNumericUpDown As Biosystems.Ax00.Controls.Usercontrols.BSNumericUpDown
    Friend WithEvents bsClearButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDeleteButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsSearchButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsPatientListDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsGridButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsPatientsListLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSearchCriteriaPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsGridPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsOpenSearchButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCloseSearchButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDetailsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsPatientDetailsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsPatientDetailsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSearchCriteriaGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsSearchButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsSelectPatientButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsAgeUnitsComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsSearchAgeUnitLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDetailAreaButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents BsAgeRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents BsDateBirthRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsResetButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDetailsAgeByTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsGenderDetailsComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsCommentsTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsCommentsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsPerformedByTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsPerformedbyLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDetailsAgeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDetailsLastNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDetailsPatientIDTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsDetailsDoBLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDetailsGenderLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDetailsPatientIDLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDetailsLastNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsDetailsFirstNameTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsDetailsFirstNameLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsDateOfBirthMaskedTextBox As DevExpress.XtraEditors.DateEdit
End Class
