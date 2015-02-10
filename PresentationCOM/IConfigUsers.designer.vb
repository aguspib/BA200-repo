<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiConfigUsers
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiConfigUsers))
        Me.bsUserDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsUserListLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsUserDetailsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsLevelComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsLastNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsFirstNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsPasswordConfirmTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsPasswordTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsUserIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsTestNumberUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
        Me.bsTestNumberLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLevelLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsUserNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsPasswordConfirmLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsPasswordLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLastNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsFirstNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsUserDetailLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsUsersListGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsNewButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsEditButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsDeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        CType(Me.bsUserDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsUserDetailsGroupBox.SuspendLayout()
        CType(Me.bsTestNumberUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsUsersListGroupBox.SuspendLayout()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsUserDataGridView
        '
        Me.bsUserDataGridView.AllowUserToAddRows = False
        Me.bsUserDataGridView.AllowUserToDeleteRows = False
        Me.bsUserDataGridView.AllowUserToResizeColumns = False
        Me.bsUserDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsUserDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsUserDataGridView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsUserDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsUserDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsUserDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsUserDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsUserDataGridView.ColumnHeadersHeight = 20
        Me.bsUserDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsUserDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsUserDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsUserDataGridView.EnterToTab = False
        Me.bsUserDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsUserDataGridView.Location = New System.Drawing.Point(10, 40)
        Me.bsUserDataGridView.Name = "bsUserDataGridView"
        Me.bsUserDataGridView.ReadOnly = True
        Me.bsUserDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsUserDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsUserDataGridView.RowHeadersVisible = False
        Me.bsUserDataGridView.RowHeadersWidth = 20
        Me.bsUserDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsUserDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsUserDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsUserDataGridView.Size = New System.Drawing.Size(938, 285)
        Me.bsUserDataGridView.TabIndex = 3
        Me.bsUserDataGridView.TabStop = False
        Me.bsUserDataGridView.TabToEnter = False
        '
        'bsUserListLabel
        '
        Me.bsUserListLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsUserListLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsUserListLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUserListLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsUserListLabel.Name = "bsUserListLabel"
        Me.bsUserListLabel.Size = New System.Drawing.Size(938, 20)
        Me.bsUserListLabel.TabIndex = 2
        Me.bsUserListLabel.Text = "List of Application Users"
        Me.bsUserListLabel.Title = True
        '
        'bsUserDetailsGroupBox
        '
        Me.bsUserDetailsGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsLevelComboBox)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsSaveButton)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsCancelButton)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsLastNameTextBox)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsFirstNameTextBox)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsPasswordConfirmTextBox)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsPasswordTextBox)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsUserIDTextBox)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsTestNumberUpDown)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsTestNumberLabel)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsLevelLabel)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsUserNameLabel)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsPasswordConfirmLabel)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsPasswordLabel)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsLastNameLabel)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsFirstNameLabel)
        Me.bsUserDetailsGroupBox.Controls.Add(Me.bsUserDetailLabel)
        Me.bsUserDetailsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsUserDetailsGroupBox.Location = New System.Drawing.Point(10, 393)
        Me.bsUserDetailsGroupBox.Name = "bsUserDetailsGroupBox"
        Me.bsUserDetailsGroupBox.Size = New System.Drawing.Size(958, 214)
        Me.bsUserDetailsGroupBox.TabIndex = 8
        Me.bsUserDetailsGroupBox.TabStop = False
        '
        'bsLevelComboBox
        '
        Me.bsLevelComboBox.BackColor = System.Drawing.Color.White
        Me.bsLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsLevelComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLevelComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsLevelComboBox.FormattingEnabled = True
        Me.bsLevelComboBox.Location = New System.Drawing.Point(419, 59)
        Me.bsLevelComboBox.Name = "bsLevelComboBox"
        Me.bsLevelComboBox.Size = New System.Drawing.Size(239, 21)
        Me.bsLevelComboBox.TabIndex = 11
        '
        'bsSaveButton
        '
        Me.bsSaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSaveButton.Location = New System.Drawing.Point(879, 172)
        Me.bsSaveButton.Name = "bsSaveButton"
        Me.bsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveButton.TabIndex = 17
        Me.bsSaveButton.UseVisualStyleBackColor = True
        '
        'bsCancelButton
        '
        Me.bsCancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Location = New System.Drawing.Point(916, 172)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 18
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsLastNameTextBox
        '
        Me.bsLastNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsLastNameTextBox.DecimalsValues = False
        Me.bsLastNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLastNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsLastNameTextBox.IsNumeric = False
        Me.bsLastNameTextBox.Location = New System.Drawing.Point(419, 102)
        Me.bsLastNameTextBox.Mandatory = False
        Me.bsLastNameTextBox.MaxLength = 30
        Me.bsLastNameTextBox.Name = "bsLastNameTextBox"
        Me.bsLastNameTextBox.Size = New System.Drawing.Size(389, 21)
        Me.bsLastNameTextBox.TabIndex = 14
        Me.bsLastNameTextBox.WordWrap = False
        '
        'bsFirstNameTextBox
        '
        Me.bsFirstNameTextBox.BackColor = System.Drawing.Color.White
        Me.bsFirstNameTextBox.DecimalsValues = False
        Me.bsFirstNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFirstNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsFirstNameTextBox.IsNumeric = False
        Me.bsFirstNameTextBox.Location = New System.Drawing.Point(10, 102)
        Me.bsFirstNameTextBox.Mandatory = False
        Me.bsFirstNameTextBox.MaxLength = 30
        Me.bsFirstNameTextBox.Name = "bsFirstNameTextBox"
        Me.bsFirstNameTextBox.Size = New System.Drawing.Size(359, 21)
        Me.bsFirstNameTextBox.TabIndex = 13
        Me.bsFirstNameTextBox.WordWrap = False
        '
        'bsPasswordConfirmTextBox
        '
        Me.bsPasswordConfirmTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsPasswordConfirmTextBox.DecimalsValues = False
        Me.bsPasswordConfirmTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPasswordConfirmTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsPasswordConfirmTextBox.IsNumeric = False
        Me.bsPasswordConfirmTextBox.Location = New System.Drawing.Point(419, 146)
        Me.bsPasswordConfirmTextBox.Mandatory = True
        Me.bsPasswordConfirmTextBox.MaxLength = 10
        Me.bsPasswordConfirmTextBox.Name = "bsPasswordConfirmTextBox"
        Me.bsPasswordConfirmTextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.bsPasswordConfirmTextBox.Size = New System.Drawing.Size(239, 21)
        Me.bsPasswordConfirmTextBox.TabIndex = 16
        Me.bsPasswordConfirmTextBox.WordWrap = False
        '
        'bsPasswordTextBox
        '
        Me.bsPasswordTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsPasswordTextBox.DecimalsValues = False
        Me.bsPasswordTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPasswordTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsPasswordTextBox.IsNumeric = False
        Me.bsPasswordTextBox.Location = New System.Drawing.Point(10, 146)
        Me.bsPasswordTextBox.Mandatory = True
        Me.bsPasswordTextBox.MaxLength = 10
        Me.bsPasswordTextBox.Name = "bsPasswordTextBox"
        Me.bsPasswordTextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.bsPasswordTextBox.Size = New System.Drawing.Size(239, 21)
        Me.bsPasswordTextBox.TabIndex = 15
        Me.bsPasswordTextBox.WordWrap = False
        '
        'bsUserIDTextBox
        '
        Me.bsUserIDTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsUserIDTextBox.DecimalsValues = False
        Me.bsUserIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsUserIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsUserIDTextBox.IsNumeric = False
        Me.bsUserIDTextBox.Location = New System.Drawing.Point(10, 58)
        Me.bsUserIDTextBox.Mandatory = True
        Me.bsUserIDTextBox.MaxLength = 16
        Me.bsUserIDTextBox.Name = "bsUserIDTextBox"
        Me.bsUserIDTextBox.Size = New System.Drawing.Size(239, 21)
        Me.bsUserIDTextBox.TabIndex = 10
        Me.bsUserIDTextBox.WordWrap = False
        '
        'bsTestNumberUpDown
        '
        Me.bsTestNumberUpDown.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsTestNumberUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsTestNumberUpDown.Location = New System.Drawing.Point(846, 59)
        Me.bsTestNumberUpDown.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        Me.bsTestNumberUpDown.Name = "bsTestNumberUpDown"
        Me.bsTestNumberUpDown.Size = New System.Drawing.Size(98, 21)
        Me.bsTestNumberUpDown.TabIndex = 12
        Me.bsTestNumberUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'bsTestNumberLabel
        '
        Me.bsTestNumberLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTestNumberLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestNumberLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestNumberLabel.Location = New System.Drawing.Point(846, 40)
        Me.bsTestNumberLabel.Name = "bsTestNumberLabel"
        Me.bsTestNumberLabel.Size = New System.Drawing.Size(101, 13)
        Me.bsTestNumberLabel.TabIndex = 25
        Me.bsTestNumberLabel.Text = "Tests Number:"
        Me.bsTestNumberLabel.Title = False
        '
        'bsLevelLabel
        '
        Me.bsLevelLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLevelLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLevelLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLevelLabel.Location = New System.Drawing.Point(419, 40)
        Me.bsLevelLabel.Name = "bsLevelLabel"
        Me.bsLevelLabel.Size = New System.Drawing.Size(239, 13)
        Me.bsLevelLabel.TabIndex = 24
        Me.bsLevelLabel.Text = "Level:"
        Me.bsLevelLabel.Title = False
        '
        'bsUserNameLabel
        '
        Me.bsUserNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsUserNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsUserNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUserNameLabel.Location = New System.Drawing.Point(10, 40)
        Me.bsUserNameLabel.Name = "bsUserNameLabel"
        Me.bsUserNameLabel.Size = New System.Drawing.Size(239, 13)
        Me.bsUserNameLabel.TabIndex = 23
        Me.bsUserNameLabel.Text = "User ID:"
        Me.bsUserNameLabel.Title = False
        '
        'bsPasswordConfirmLabel
        '
        Me.bsPasswordConfirmLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsPasswordConfirmLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPasswordConfirmLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPasswordConfirmLabel.Location = New System.Drawing.Point(419, 128)
        Me.bsPasswordConfirmLabel.Name = "bsPasswordConfirmLabel"
        Me.bsPasswordConfirmLabel.Size = New System.Drawing.Size(239, 13)
        Me.bsPasswordConfirmLabel.TabIndex = 22
        Me.bsPasswordConfirmLabel.Text = "Password Confirmation:"
        Me.bsPasswordConfirmLabel.Title = False
        '
        'bsPasswordLabel
        '
        Me.bsPasswordLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsPasswordLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPasswordLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPasswordLabel.Location = New System.Drawing.Point(10, 128)
        Me.bsPasswordLabel.Name = "bsPasswordLabel"
        Me.bsPasswordLabel.Size = New System.Drawing.Size(239, 13)
        Me.bsPasswordLabel.TabIndex = 21
        Me.bsPasswordLabel.Text = "Password:"
        Me.bsPasswordLabel.Title = False
        '
        'bsLastNameLabel
        '
        Me.bsLastNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLastNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLastNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLastNameLabel.Location = New System.Drawing.Point(419, 84)
        Me.bsLastNameLabel.Name = "bsLastNameLabel"
        Me.bsLastNameLabel.Size = New System.Drawing.Size(389, 13)
        Me.bsLastNameLabel.TabIndex = 20
        Me.bsLastNameLabel.Text = "Last Name:"
        Me.bsLastNameLabel.Title = False
        '
        'bsFirstNameLabel
        '
        Me.bsFirstNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsFirstNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsFirstNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsFirstNameLabel.Location = New System.Drawing.Point(10, 84)
        Me.bsFirstNameLabel.Name = "bsFirstNameLabel"
        Me.bsFirstNameLabel.Size = New System.Drawing.Size(359, 13)
        Me.bsFirstNameLabel.TabIndex = 19
        Me.bsFirstNameLabel.Text = "First Name:"
        Me.bsFirstNameLabel.Title = False
        '
        'bsUserDetailLabel
        '
        Me.bsUserDetailLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsUserDetailLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsUserDetailLabel.ForeColor = System.Drawing.Color.Black
        Me.bsUserDetailLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsUserDetailLabel.Name = "bsUserDetailLabel"
        Me.bsUserDetailLabel.Size = New System.Drawing.Size(938, 20)
        Me.bsUserDetailLabel.TabIndex = 9
        Me.bsUserDetailLabel.Text = "User's Details"
        Me.bsUserDetailLabel.Title = True
        '
        'bsExitButton
        '
        Me.bsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(926, 612)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 19
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsUsersListGroupBox
        '
        Me.bsUsersListGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bsUsersListGroupBox.Controls.Add(Me.bsNewButton)
        Me.bsUsersListGroupBox.Controls.Add(Me.bsPrintButton)
        Me.bsUsersListGroupBox.Controls.Add(Me.bsEditButton)
        Me.bsUsersListGroupBox.Controls.Add(Me.bsDeleteButton)
        Me.bsUsersListGroupBox.Controls.Add(Me.bsUserDataGridView)
        Me.bsUsersListGroupBox.Controls.Add(Me.bsUserListLabel)
        Me.bsUsersListGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsUsersListGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsUsersListGroupBox.Name = "bsUsersListGroupBox"
        Me.bsUsersListGroupBox.Size = New System.Drawing.Size(958, 378)
        Me.bsUsersListGroupBox.TabIndex = 2
        Me.bsUsersListGroupBox.TabStop = False
        '
        'bsNewButton
        '
        Me.bsNewButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsNewButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsNewButton.Location = New System.Drawing.Point(805, 336)
        Me.bsNewButton.Name = "bsNewButton"
        Me.bsNewButton.Size = New System.Drawing.Size(32, 32)
        Me.bsNewButton.TabIndex = 4
        Me.bsNewButton.UseVisualStyleBackColor = True
        '
        'bsPrintButton
        '
        Me.bsPrintButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Location = New System.Drawing.Point(916, 336)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 7
        Me.bsPrintButton.UseVisualStyleBackColor = True
        '
        'bsEditButton
        '
        Me.bsEditButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsEditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsEditButton.Location = New System.Drawing.Point(842, 336)
        Me.bsEditButton.Name = "bsEditButton"
        Me.bsEditButton.Size = New System.Drawing.Size(32, 32)
        Me.bsEditButton.TabIndex = 5
        Me.bsEditButton.UseVisualStyleBackColor = True
        '
        'bsDeleteButton
        '
        Me.bsDeleteButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsDeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsDeleteButton.Location = New System.Drawing.Point(879, 336)
        Me.bsDeleteButton.Name = "bsDeleteButton"
        Me.bsDeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.bsDeleteButton.TabIndex = 6
        Me.bsDeleteButton.UseVisualStyleBackColor = True
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'IConfigUsers
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
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsUsersListGroupBox)
        Me.Controls.Add(Me.bsUserDetailsGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IConfigUsers"
        Me.ShowInTaskbar = False
        Me.Text = ""
        CType(Me.bsUserDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsUserDetailsGroupBox.ResumeLayout(False)
        Me.bsUserDetailsGroupBox.PerformLayout()
        CType(Me.bsTestNumberUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsUsersListGroupBox.ResumeLayout(False)
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsUserDetailsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsUserDetailLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsUserListLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLastNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsFirstNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsPasswordConfirmLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsPasswordLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsTestNumberLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsLevelLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsUserNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsTestNumberUpDown As Biosystems.Ax00.Controls.Usercontrols.BSNumericUpDown
    Friend WithEvents bsPasswordConfirmTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsPasswordTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsUserIDTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsLastNameTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsFirstNameTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents bsUserDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsUsersListGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsNewButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsEditButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDeleteButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsLevelComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
End Class
