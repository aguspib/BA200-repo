Imports Biosystems.Ax00.Controls.UserControls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IWSSampleRequest
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            CreateLogActivity("Initial - Dispose", Me.Name & ".Dispose", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            ReleaseElements()
        Finally
            MyBase.Dispose(disposing)
            isClosingFlag = False
            CreateLogActivity("Final - Dispose", Me.Name & ".Dispose", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle13 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle14 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle15 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IWSSampleRequest))
        Me.bsLoadWSButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSaveWSButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsOpenRotorButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSearchTestsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsPatientSearchButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsSampleClassesTabControl = New DevExpress.XtraTab.XtraTabControl()
        Me.PatientsTab = New DevExpress.XtraTab.XtraTabPage()
        Me.bsPatientOrdersDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsAllPatientsCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsDelPatientsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.OtherSamplesTab = New DevExpress.XtraTab.XtraTabPage()
        Me.bsControlOrdersDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsBlkCalibDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.bsAllCtrlsCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsAllBlkCalCheckBox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsDelControlsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsDelCalibratorsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsOrderDetailsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsPrepareWSLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsNumOrdersNumericUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.bsSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsNumOrdersLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsStatCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsSampleClassComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsSampleTypeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsSampleClassLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsPatientIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsPatientIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsLIMSImportButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsLIMSErrorsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsOffSystemResultsButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsScanningButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsBarcodeWarningButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        CType(Me.bsSampleClassesTabControl, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsSampleClassesTabControl.SuspendLayout()
        Me.PatientsTab.SuspendLayout()
        CType(Me.bsPatientOrdersDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.OtherSamplesTab.SuspendLayout()
        CType(Me.bsControlOrdersDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsBlkCalibDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsOrderDetailsGroupBox.SuspendLayout()
        CType(Me.bsNumOrdersNumericUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsLoadWSButton
        '
        Me.bsLoadWSButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsLoadWSButton.Enabled = False
        Me.bsLoadWSButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLoadWSButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsLoadWSButton.Location = New System.Drawing.Point(10, 615)
        Me.bsLoadWSButton.Name = "bsLoadWSButton"
        Me.bsLoadWSButton.Size = New System.Drawing.Size(32, 32)
        Me.bsLoadWSButton.TabIndex = 18
        Me.bsLoadWSButton.UseVisualStyleBackColor = True
        '
        'bsSaveWSButton
        '
        Me.bsSaveWSButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsSaveWSButton.Enabled = False
        Me.bsSaveWSButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSaveWSButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsSaveWSButton.Location = New System.Drawing.Point(44, 615)
        Me.bsSaveWSButton.Name = "bsSaveWSButton"
        Me.bsSaveWSButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveWSButton.TabIndex = 19
        Me.bsSaveWSButton.UseVisualStyleBackColor = True
        '
        'bsOpenRotorButton
        '
        Me.bsOpenRotorButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsOpenRotorButton.Enabled = False
        Me.bsOpenRotorButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsOpenRotorButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsOpenRotorButton.Location = New System.Drawing.Point(898, 615)
        Me.bsOpenRotorButton.Name = "bsOpenRotorButton"
        Me.bsOpenRotorButton.Size = New System.Drawing.Size(32, 32)
        Me.bsOpenRotorButton.TabIndex = 22
        Me.bsOpenRotorButton.UseVisualStyleBackColor = True
        '
        'bsSearchTestsButton
        '
        Me.bsSearchTestsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSearchTestsButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSearchTestsButton.ForeColor = System.Drawing.Color.SteelBlue
        Me.bsSearchTestsButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsSearchTestsButton.Location = New System.Drawing.Point(858, 56)
        Me.bsSearchTestsButton.Name = "bsSearchTestsButton"
        Me.bsSearchTestsButton.Size = New System.Drawing.Size(90, 21)
        Me.bsSearchTestsButton.TabIndex = 6
        Me.bsSearchTestsButton.Text = "Tests"
        Me.bsSearchTestsButton.UseMnemonic = False
        Me.bsSearchTestsButton.UseVisualStyleBackColor = True
        '
        'bsPatientSearchButton
        '
        Me.bsPatientSearchButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPatientSearchButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsPatientSearchButton.ForeColor = System.Drawing.Color.Indigo
        Me.bsPatientSearchButton.Location = New System.Drawing.Point(441, 57)
        Me.bsPatientSearchButton.Name = "bsPatientSearchButton"
        Me.bsPatientSearchButton.Size = New System.Drawing.Size(35, 21)
        Me.bsPatientSearchButton.TabIndex = 3
        Me.bsPatientSearchButton.Text = "..."
        Me.bsPatientSearchButton.UseVisualStyleBackColor = True
        '
        'bsSampleClassesTabControl
        '
        Me.bsSampleClassesTabControl.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.bsSampleClassesTabControl.Appearance.ForeColor = System.Drawing.Color.Black
        Me.bsSampleClassesTabControl.Appearance.Options.UseBackColor = True
        Me.bsSampleClassesTabControl.Appearance.Options.UseForeColor = True
        Me.bsSampleClassesTabControl.AppearancePage.Header.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSampleClassesTabControl.AppearancePage.Header.ForeColor = System.Drawing.Color.Black
        Me.bsSampleClassesTabControl.AppearancePage.Header.Options.UseFont = True
        Me.bsSampleClassesTabControl.AppearancePage.Header.Options.UseForeColor = True
        Me.bsSampleClassesTabControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.bsSampleClassesTabControl.Location = New System.Drawing.Point(10, 106)
        Me.bsSampleClassesTabControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003
        Me.bsSampleClassesTabControl.LookAndFeel.UseDefaultLookAndFeel = False
        Me.bsSampleClassesTabControl.LookAndFeel.UseWindowsXPTheme = True
        Me.bsSampleClassesTabControl.Name = "bsSampleClassesTabControl"
        Me.bsSampleClassesTabControl.SelectedTabPage = Me.PatientsTab
        Me.bsSampleClassesTabControl.Size = New System.Drawing.Size(958, 503)
        Me.bsSampleClassesTabControl.TabIndex = 7
        Me.bsSampleClassesTabControl.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.PatientsTab, Me.OtherSamplesTab})
        '
        'PatientsTab
        '
        Me.PatientsTab.Appearance.PageClient.BackColor = System.Drawing.Color.Gainsboro
        Me.PatientsTab.Appearance.PageClient.Options.UseBackColor = True
        Me.PatientsTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.PatientsTab.Controls.Add(Me.bsPatientOrdersDataGridView)
        Me.PatientsTab.Controls.Add(Me.bsAllPatientsCheckBox)
        Me.PatientsTab.Controls.Add(Me.bsDelPatientsButton)
        Me.PatientsTab.Name = "PatientsTab"
        Me.PatientsTab.Size = New System.Drawing.Size(950, 474)
        Me.PatientsTab.Text = "*Patient Samples"
        '
        'bsPatientOrdersDataGridView
        '
        Me.bsPatientOrdersDataGridView.AllowUserToAddRows = False
        Me.bsPatientOrdersDataGridView.AllowUserToDeleteRows = False
        Me.bsPatientOrdersDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White
        Me.bsPatientOrdersDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsPatientOrdersDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsPatientOrdersDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsPatientOrdersDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable
        Me.bsPatientOrdersDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientOrdersDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsPatientOrdersDataGridView.ColumnHeadersHeight = 20
        Me.bsPatientOrdersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsPatientOrdersDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsPatientOrdersDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsPatientOrdersDataGridView.EnterToTab = False
        Me.bsPatientOrdersDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsPatientOrdersDataGridView.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.bsPatientOrdersDataGridView.Location = New System.Drawing.Point(10, 30)
        Me.bsPatientOrdersDataGridView.Name = "bsPatientOrdersDataGridView"
        Me.bsPatientOrdersDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsPatientOrdersDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsPatientOrdersDataGridView.RowHeadersVisible = False
        Me.bsPatientOrdersDataGridView.RowHeadersWidth = 20
        Me.bsPatientOrdersDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        Me.bsPatientOrdersDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsPatientOrdersDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsPatientOrdersDataGridView.Size = New System.Drawing.Size(896, 437)
        Me.bsPatientOrdersDataGridView.TabIndex = 9
        Me.bsPatientOrdersDataGridView.TabToEnter = False
        '
        'bsAllPatientsCheckBox
        '
        Me.bsAllPatientsCheckBox.AutoSize = True
        Me.bsAllPatientsCheckBox.ForeColor = System.Drawing.Color.Black
        Me.bsAllPatientsCheckBox.Location = New System.Drawing.Point(10, 10)
        Me.bsAllPatientsCheckBox.Name = "bsAllPatientsCheckBox"
        Me.bsAllPatientsCheckBox.Size = New System.Drawing.Size(71, 17)
        Me.bsAllPatientsCheckBox.TabIndex = 10
        Me.bsAllPatientsCheckBox.Text = "Patients"
        Me.bsAllPatientsCheckBox.UseVisualStyleBackColor = True
        '
        'bsDelPatientsButton
        '
        Me.bsDelPatientsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsDelPatientsButton.Enabled = False
        Me.bsDelPatientsButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDelPatientsButton.ForeColor = System.Drawing.Color.Black
        Me.bsDelPatientsButton.Location = New System.Drawing.Point(912, 29)
        Me.bsDelPatientsButton.Name = "bsDelPatientsButton"
        Me.bsDelPatientsButton.Size = New System.Drawing.Size(32, 32)
        Me.bsDelPatientsButton.TabIndex = 10
        Me.bsDelPatientsButton.UseVisualStyleBackColor = True
        '
        'OtherSamplesTab
        '
        Me.OtherSamplesTab.Appearance.Header.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OtherSamplesTab.Appearance.Header.ForeColor = System.Drawing.Color.Black
        Me.OtherSamplesTab.Appearance.Header.Options.UseFont = True
        Me.OtherSamplesTab.Appearance.Header.Options.UseForeColor = True
        Me.OtherSamplesTab.Appearance.PageClient.BackColor = System.Drawing.Color.Gainsboro
        Me.OtherSamplesTab.Appearance.PageClient.Options.UseBackColor = True
        Me.OtherSamplesTab.Controls.Add(Me.bsControlOrdersDataGridView)
        Me.OtherSamplesTab.Controls.Add(Me.bsBlkCalibDataGridView)
        Me.OtherSamplesTab.Controls.Add(Me.bsAllCtrlsCheckBox)
        Me.OtherSamplesTab.Controls.Add(Me.bsAllBlkCalCheckBox)
        Me.OtherSamplesTab.Controls.Add(Me.bsDelControlsButton)
        Me.OtherSamplesTab.Controls.Add(Me.bsDelCalibratorsButton)
        Me.OtherSamplesTab.Name = "OtherSamplesTab"
        Me.OtherSamplesTab.Size = New System.Drawing.Size(950, 474)
        Me.OtherSamplesTab.Text = "*Blanks, Calibrators And Controls"
        '
        'bsControlOrdersDataGridView
        '
        Me.bsControlOrdersDataGridView.AllowUserToAddRows = False
        Me.bsControlOrdersDataGridView.AllowUserToDeleteRows = False
        Me.bsControlOrdersDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle6.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White
        Me.bsControlOrdersDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle6
        Me.bsControlOrdersDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsControlOrdersDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsControlOrdersDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsControlOrdersDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.bsControlOrdersDataGridView.ColumnHeadersHeight = 20
        Me.bsControlOrdersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsControlOrdersDataGridView.DefaultCellStyle = DataGridViewCellStyle8
        Me.bsControlOrdersDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsControlOrdersDataGridView.EnterToTab = False
        Me.bsControlOrdersDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsControlOrdersDataGridView.Location = New System.Drawing.Point(10, 275)
        Me.bsControlOrdersDataGridView.Name = "bsControlOrdersDataGridView"
        Me.bsControlOrdersDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle9.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsControlOrdersDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.bsControlOrdersDataGridView.RowHeadersVisible = False
        Me.bsControlOrdersDataGridView.RowHeadersWidth = 20
        Me.bsControlOrdersDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle10.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White
        Me.bsControlOrdersDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle10
        Me.bsControlOrdersDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsControlOrdersDataGridView.Size = New System.Drawing.Size(896, 192)
        Me.bsControlOrdersDataGridView.TabIndex = 15
        Me.bsControlOrdersDataGridView.TabToEnter = False
        '
        'bsBlkCalibDataGridView
        '
        Me.bsBlkCalibDataGridView.AllowUserToAddRows = False
        Me.bsBlkCalibDataGridView.AllowUserToDeleteRows = False
        Me.bsBlkCalibDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle11.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle11.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.White
        Me.bsBlkCalibDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle11
        Me.bsBlkCalibDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.bsBlkCalibDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsBlkCalibDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsBlkCalibDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle12.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle12.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle12.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsBlkCalibDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle12
        Me.bsBlkCalibDataGridView.ColumnHeadersHeight = 20
        Me.bsBlkCalibDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle13.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle13.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle13.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsBlkCalibDataGridView.DefaultCellStyle = DataGridViewCellStyle13
        Me.bsBlkCalibDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsBlkCalibDataGridView.EnterToTab = False
        Me.bsBlkCalibDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsBlkCalibDataGridView.Location = New System.Drawing.Point(10, 30)
        Me.bsBlkCalibDataGridView.Name = "bsBlkCalibDataGridView"
        Me.bsBlkCalibDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle14.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle14.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle14.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle14.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle14.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsBlkCalibDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle14
        Me.bsBlkCalibDataGridView.RowHeadersVisible = False
        Me.bsBlkCalibDataGridView.RowHeadersWidth = 20
        Me.bsBlkCalibDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle15.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle15.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle15.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.White
        Me.bsBlkCalibDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle15
        Me.bsBlkCalibDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsBlkCalibDataGridView.Size = New System.Drawing.Size(896, 215)
        Me.bsBlkCalibDataGridView.TabIndex = 12
        Me.bsBlkCalibDataGridView.TabToEnter = False
        '
        'bsAllCtrlsCheckBox
        '
        Me.bsAllCtrlsCheckBox.AutoSize = True
        Me.bsAllCtrlsCheckBox.ForeColor = System.Drawing.Color.Black
        Me.bsAllCtrlsCheckBox.Location = New System.Drawing.Point(10, 255)
        Me.bsAllCtrlsCheckBox.Name = "bsAllCtrlsCheckBox"
        Me.bsAllCtrlsCheckBox.Size = New System.Drawing.Size(74, 17)
        Me.bsAllCtrlsCheckBox.TabIndex = 14
        Me.bsAllCtrlsCheckBox.Text = "Controls"
        Me.bsAllCtrlsCheckBox.UseVisualStyleBackColor = True
        '
        'bsAllBlkCalCheckBox
        '
        Me.bsAllBlkCalCheckBox.AutoSize = True
        Me.bsAllBlkCalCheckBox.ForeColor = System.Drawing.Color.Black
        Me.bsAllBlkCalCheckBox.Location = New System.Drawing.Point(10, 10)
        Me.bsAllBlkCalCheckBox.Name = "bsAllBlkCalCheckBox"
        Me.bsAllBlkCalCheckBox.Size = New System.Drawing.Size(140, 17)
        Me.bsAllBlkCalCheckBox.TabIndex = 11
        Me.bsAllBlkCalCheckBox.Text = "Blanks / Calibrators"
        Me.bsAllBlkCalCheckBox.UseVisualStyleBackColor = True
        '
        'bsDelControlsButton
        '
        Me.bsDelControlsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsDelControlsButton.Enabled = False
        Me.bsDelControlsButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDelControlsButton.ForeColor = System.Drawing.Color.Black
        Me.bsDelControlsButton.Location = New System.Drawing.Point(912, 274)
        Me.bsDelControlsButton.Name = "bsDelControlsButton"
        Me.bsDelControlsButton.Size = New System.Drawing.Size(32, 32)
        Me.bsDelControlsButton.TabIndex = 16
        Me.bsDelControlsButton.UseVisualStyleBackColor = True
        '
        'bsDelCalibratorsButton
        '
        Me.bsDelCalibratorsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsDelCalibratorsButton.Enabled = False
        Me.bsDelCalibratorsButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsDelCalibratorsButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsDelCalibratorsButton.Location = New System.Drawing.Point(912, 29)
        Me.bsDelCalibratorsButton.Name = "bsDelCalibratorsButton"
        Me.bsDelCalibratorsButton.Size = New System.Drawing.Size(32, 32)
        Me.bsDelCalibratorsButton.TabIndex = 13
        Me.bsDelCalibratorsButton.UseVisualStyleBackColor = True
        '
        'bsOrderDetailsGroupBox
        '
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsPrepareWSLabel)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsSearchTestsButton)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsNumOrdersNumericUpDown)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsSampleTypeLabel)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsNumOrdersLabel)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsStatCheckbox)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsSampleClassComboBox)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsSampleTypeComboBox)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsSampleClassLabel)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsPatientSearchButton)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsPatientIDTextBox)
        Me.bsOrderDetailsGroupBox.Controls.Add(Me.bsPatientIDLabel)
        Me.bsOrderDetailsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsOrderDetailsGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsOrderDetailsGroupBox.Name = "bsOrderDetailsGroupBox"
        Me.bsOrderDetailsGroupBox.Size = New System.Drawing.Size(958, 86)
        Me.bsOrderDetailsGroupBox.TabIndex = 0
        Me.bsOrderDetailsGroupBox.TabStop = False
        '
        'bsPrepareWSLabel
        '
        Me.bsPrepareWSLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsPrepareWSLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsPrepareWSLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPrepareWSLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsPrepareWSLabel.Name = "bsPrepareWSLabel"
        Me.bsPrepareWSLabel.Size = New System.Drawing.Size(938, 19)
        Me.bsPrepareWSLabel.TabIndex = 0
        Me.bsPrepareWSLabel.Text = "Work Session Preparation"
        Me.bsPrepareWSLabel.Title = True
        '
        'bsNumOrdersNumericUpDown
        '
        Me.bsNumOrdersNumericUpDown.BackColor = System.Drawing.Color.White
        Me.bsNumOrdersNumericUpDown.ForeColor = System.Drawing.Color.Black
        Me.bsNumOrdersNumericUpDown.Location = New System.Drawing.Point(533, 57)
        Me.bsNumOrdersNumericUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.bsNumOrdersNumericUpDown.Name = "bsNumOrdersNumericUpDown"
        Me.bsNumOrdersNumericUpDown.Size = New System.Drawing.Size(54, 21)
        Me.bsNumOrdersNumericUpDown.TabIndex = 4
        Me.bsNumOrdersNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.bsNumOrdersNumericUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'bsSampleTypeLabel
        '
        Me.bsSampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeLabel.Location = New System.Drawing.Point(642, 39)
        Me.bsSampleTypeLabel.Name = "bsSampleTypeLabel"
        Me.bsSampleTypeLabel.Size = New System.Drawing.Size(210, 13)
        Me.bsSampleTypeLabel.TabIndex = 0
        Me.bsSampleTypeLabel.Text = "Sample Type:"
        Me.bsSampleTypeLabel.Title = False
        '
        'bsNumOrdersLabel
        '
        Me.bsNumOrdersLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNumOrdersLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNumOrdersLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNumOrdersLabel.Location = New System.Drawing.Point(533, 39)
        Me.bsNumOrdersLabel.Name = "bsNumOrdersLabel"
        Me.bsNumOrdersLabel.Size = New System.Drawing.Size(57, 13)
        Me.bsNumOrdersLabel.TabIndex = 0
        Me.bsNumOrdersLabel.Text = "Number:"
        Me.bsNumOrdersLabel.Title = False
        '
        'bsStatCheckbox
        '
        Me.bsStatCheckbox.AutoSize = True
        Me.bsStatCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsStatCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsStatCheckbox.Location = New System.Drawing.Point(164, 59)
        Me.bsStatCheckbox.Name = "bsStatCheckbox"
        Me.bsStatCheckbox.Size = New System.Drawing.Size(49, 17)
        Me.bsStatCheckbox.TabIndex = 1
        Me.bsStatCheckbox.Text = "Stat"
        Me.bsStatCheckbox.UseVisualStyleBackColor = True
        '
        'bsSampleClassComboBox
        '
        Me.bsSampleClassComboBox.BackColor = System.Drawing.Color.White
        Me.bsSampleClassComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsSampleClassComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSampleClassComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleClassComboBox.FormattingEnabled = True
        Me.bsSampleClassComboBox.Location = New System.Drawing.Point(10, 57)
        Me.bsSampleClassComboBox.Name = "bsSampleClassComboBox"
        Me.bsSampleClassComboBox.Size = New System.Drawing.Size(148, 21)
        Me.bsSampleClassComboBox.TabIndex = 0
        '
        'bsSampleTypeComboBox
        '
        Me.bsSampleTypeComboBox.BackColor = System.Drawing.Color.White
        Me.bsSampleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsSampleTypeComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsSampleTypeComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeComboBox.FormattingEnabled = True
        Me.bsSampleTypeComboBox.Location = New System.Drawing.Point(642, 57)
        Me.bsSampleTypeComboBox.Name = "bsSampleTypeComboBox"
        Me.bsSampleTypeComboBox.Size = New System.Drawing.Size(210, 21)
        Me.bsSampleTypeComboBox.TabIndex = 5
        '
        'bsSampleClassLabel
        '
        Me.bsSampleClassLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleClassLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleClassLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleClassLabel.Location = New System.Drawing.Point(10, 39)
        Me.bsSampleClassLabel.Name = "bsSampleClassLabel"
        Me.bsSampleClassLabel.Size = New System.Drawing.Size(148, 13)
        Me.bsSampleClassLabel.TabIndex = 0
        Me.bsSampleClassLabel.Text = "Sample Class:"
        Me.bsSampleClassLabel.Title = False
        '
        'bsPatientIDTextBox
        '
        Me.bsPatientIDTextBox.BackColor = System.Drawing.Color.White
        Me.bsPatientIDTextBox.DecimalsValues = False
        Me.bsPatientIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsPatientIDTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsPatientIDTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.bsPatientIDTextBox.IsNumeric = False
        Me.bsPatientIDTextBox.Location = New System.Drawing.Point(247, 57)
        Me.bsPatientIDTextBox.Mandatory = False
        Me.bsPatientIDTextBox.MaxLength = 30
        Me.bsPatientIDTextBox.Name = "bsPatientIDTextBox"
        Me.bsPatientIDTextBox.Size = New System.Drawing.Size(188, 21)
        Me.bsPatientIDTextBox.TabIndex = 2
        Me.bsPatientIDTextBox.WordWrap = False
        '
        'bsPatientIDLabel
        '
        Me.bsPatientIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsPatientIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsPatientIDLabel.ForeColor = System.Drawing.Color.Black
        Me.bsPatientIDLabel.Location = New System.Drawing.Point(247, 39)
        Me.bsPatientIDLabel.Name = "bsPatientIDLabel"
        Me.bsPatientIDLabel.Size = New System.Drawing.Size(188, 13)
        Me.bsPatientIDLabel.TabIndex = 0
        Me.bsPatientIDLabel.Text = "Patient / Sample:"
        Me.bsPatientIDLabel.Title = False
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bsAcceptButton.Font = New System.Drawing.Font("Verdana", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsAcceptButton.ForeColor = System.Drawing.Color.Black
        Me.bsAcceptButton.Location = New System.Drawing.Point(932, 615)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 21
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'bsLIMSImportButton
        '
        Me.bsLIMSImportButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsLIMSImportButton.Enabled = False
        Me.bsLIMSImportButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLIMSImportButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsLIMSImportButton.Location = New System.Drawing.Point(280, 615)
        Me.bsLIMSImportButton.Name = "bsLIMSImportButton"
        Me.bsLIMSImportButton.Size = New System.Drawing.Size(32, 32)
        Me.bsLIMSImportButton.TabIndex = 24
        Me.bsLIMSImportButton.UseVisualStyleBackColor = True
        '
        'bsLIMSErrorsButton
        '
        Me.bsLIMSErrorsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsLIMSErrorsButton.Enabled = False
        Me.bsLIMSErrorsButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsLIMSErrorsButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsLIMSErrorsButton.Location = New System.Drawing.Point(246, 615)
        Me.bsLIMSErrorsButton.Name = "bsLIMSErrorsButton"
        Me.bsLIMSErrorsButton.Size = New System.Drawing.Size(32, 32)
        Me.bsLIMSErrorsButton.TabIndex = 25
        Me.bsLIMSErrorsButton.UseVisualStyleBackColor = True
        '
        'bsOffSystemResultsButton
        '
        Me.bsOffSystemResultsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsOffSystemResultsButton.Enabled = False
        Me.bsOffSystemResultsButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsOffSystemResultsButton.ForeColor = System.Drawing.Color.DarkGreen
        Me.bsOffSystemResultsButton.Location = New System.Drawing.Point(100, 615)
        Me.bsOffSystemResultsButton.Name = "bsOffSystemResultsButton"
        Me.bsOffSystemResultsButton.Size = New System.Drawing.Size(32, 32)
        Me.bsOffSystemResultsButton.TabIndex = 20
        Me.bsOffSystemResultsButton.UseVisualStyleBackColor = True
        '
        'bsScanningButton
        '
        Me.bsScanningButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsScanningButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsScanningButton.Location = New System.Drawing.Point(156, 615)
        Me.bsScanningButton.Name = "bsScanningButton"
        Me.bsScanningButton.Size = New System.Drawing.Size(32, 32)
        Me.bsScanningButton.TabIndex = 23
        Me.bsScanningButton.UseVisualStyleBackColor = True
        '
        'bsBarcodeWarningButton
        '
        Me.bsBarcodeWarningButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsBarcodeWarningButton.Location = New System.Drawing.Point(190, 615)
        Me.bsBarcodeWarningButton.Name = "bsBarcodeWarningButton"
        Me.bsBarcodeWarningButton.Size = New System.Drawing.Size(32, 32)
        Me.bsBarcodeWarningButton.TabIndex = 26
        Me.bsBarcodeWarningButton.UseVisualStyleBackColor = True
        '
        'bsErrorProvider1
        '
        Me.bsErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsErrorProvider1.ContainerControl = Me
        '
        'IWSSampleRequest
        '
        Me.AcceptButton = Me.bsAcceptButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.Controls.Add(Me.bsBarcodeWarningButton)
        Me.Controls.Add(Me.bsScanningButton)
        Me.Controls.Add(Me.bsOffSystemResultsButton)
        Me.Controls.Add(Me.bsLIMSErrorsButton)
        Me.Controls.Add(Me.bsLIMSImportButton)
        Me.Controls.Add(Me.bsAcceptButton)
        Me.Controls.Add(Me.bsLoadWSButton)
        Me.Controls.Add(Me.bsSaveWSButton)
        Me.Controls.Add(Me.bsSampleClassesTabControl)
        Me.Controls.Add(Me.bsOpenRotorButton)
        Me.Controls.Add(Me.bsOrderDetailsGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IWSSampleRequest"
        Me.ShowInTaskbar = False
        Me.Text = "WS_Preparation"
        CType(Me.bsSampleClassesTabControl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsSampleClassesTabControl.ResumeLayout(False)
        Me.PatientsTab.ResumeLayout(False)
        Me.PatientsTab.PerformLayout()
        CType(Me.bsPatientOrdersDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.OtherSamplesTab.ResumeLayout(False)
        Me.OtherSamplesTab.PerformLayout()
        CType(Me.bsControlOrdersDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsBlkCalibDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsOrderDetailsGroupBox.ResumeLayout(False)
        Me.bsOrderDetailsGroupBox.PerformLayout()
        CType(Me.bsNumOrdersNumericUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsOrderDetailsGroupBox As BSGroupBox
    Friend WithEvents bsNumOrdersNumericUpDown As BSNumericUpDown
    Friend WithEvents bsNumOrdersLabel As BSLabel
    Friend WithEvents bsStatCheckbox As BSCheckbox
    Friend WithEvents bsSampleClassComboBox As BSComboBox
    Friend WithEvents bsSampleClassLabel As BSLabel
    Friend WithEvents bsPatientSearchButton As BSButton
    Friend WithEvents bsPatientIDTextBox As BSTextBox
    Friend WithEvents bsPatientIDLabel As BSLabel
    Friend WithEvents bsSampleTypeLabel As BSLabel
    Friend WithEvents bsSampleTypeComboBox As BSComboBox
    Friend WithEvents bsSearchTestsButton As BSButton
    Friend WithEvents bsOpenRotorButton As BSButton
    Friend WithEvents bsSampleClassesTabControl As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents PatientsTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents OtherSamplesTab As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents bsSaveWSButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLoadWSButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsPrepareWSLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDelPatientsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDelControlsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDelCalibratorsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLIMSImportButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLIMSErrorsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsAllPatientsCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsAllBlkCalCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsAllCtrlsCheckBox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsBlkCalibDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsControlOrdersDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsPatientOrdersDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsOffSystemResultsButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScanningButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsBarcodeWarningButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
End Class
