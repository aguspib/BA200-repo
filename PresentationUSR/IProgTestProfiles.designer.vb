<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiProgTestProfiles
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiProgTestProfiles))
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsDeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsEditButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsNewButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsProfileNameGrpbox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsTestProfileDefinitionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleTypesComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.bsSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTestSelGrpbox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsTestsTypesComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.bsTestTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsTestsSelectionDoubleList = New Biosystems.Ax00.Controls.UserControls.BSDoubleList()
        Me.bsTestsSelectionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTestProfilesListView = New Biosystems.Ax00.Controls.UserControls.BSListView()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsTestProfileLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.bsTestProfileListGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.BsCustomOrderButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsProfileNameGrpbox.SuspendLayout()
        Me.bsTestSelGrpbox.SuspendLayout()
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsTestProfileListGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.ForeColor = System.Drawing.Color.Black
        Me.bsPrintButton.Location = New System.Drawing.Point(212, 613)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 5
        Me.bsPrintButton.UseVisualStyleBackColor = True
        '
        'bsDeleteButton
        '
        Me.bsDeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsDeleteButton.ForeColor = System.Drawing.Color.Black
        Me.bsDeleteButton.Location = New System.Drawing.Point(138, 613)
        Me.bsDeleteButton.Name = "bsDeleteButton"
        Me.bsDeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.bsDeleteButton.TabIndex = 3
        Me.bsDeleteButton.UseVisualStyleBackColor = True
        '
        'bsEditButton
        '
        Me.bsEditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsEditButton.ForeColor = System.Drawing.Color.Black
        Me.bsEditButton.Location = New System.Drawing.Point(101, 613)
        Me.bsEditButton.Name = "bsEditButton"
        Me.bsEditButton.Size = New System.Drawing.Size(32, 32)
        Me.bsEditButton.TabIndex = 2
        Me.bsEditButton.UseVisualStyleBackColor = True
        '
        'bsNewButton
        '
        Me.bsNewButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsNewButton.ForeColor = System.Drawing.Color.Black
        Me.bsNewButton.Location = New System.Drawing.Point(64, 613)
        Me.bsNewButton.Name = "bsNewButton"
        Me.bsNewButton.Size = New System.Drawing.Size(32, 32)
        Me.bsNewButton.TabIndex = 1
        Me.bsNewButton.UseVisualStyleBackColor = True
        '
        'bsProfileNameGrpbox
        '
        Me.bsProfileNameGrpbox.Controls.Add(Me.bsTestProfileDefinitionLabel)
        Me.bsProfileNameGrpbox.Controls.Add(Me.bsSampleTypesComboBox)
        Me.bsProfileNameGrpbox.Controls.Add(Me.bsNameTextBox)
        Me.bsProfileNameGrpbox.Controls.Add(Me.bsSampleTypeLabel)
        Me.bsProfileNameGrpbox.Controls.Add(Me.bsNameLabel)
        Me.bsProfileNameGrpbox.ForeColor = System.Drawing.Color.Black
        Me.bsProfileNameGrpbox.Location = New System.Drawing.Point(249, 10)
        Me.bsProfileNameGrpbox.Name = "bsProfileNameGrpbox"
        Me.bsProfileNameGrpbox.Size = New System.Drawing.Size(719, 94)
        Me.bsProfileNameGrpbox.TabIndex = 5
        Me.bsProfileNameGrpbox.TabStop = False
        '
        'bsTestProfileDefinitionLabel
        '
        Me.bsTestProfileDefinitionLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTestProfileDefinitionLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTestProfileDefinitionLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestProfileDefinitionLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsTestProfileDefinitionLabel.Name = "bsTestProfileDefinitionLabel"
        Me.bsTestProfileDefinitionLabel.Size = New System.Drawing.Size(699, 20)
        Me.bsTestProfileDefinitionLabel.TabIndex = 17
        Me.bsTestProfileDefinitionLabel.Text = "Test Profile Definition"
        Me.bsTestProfileDefinitionLabel.Title = True
        '
        'bsSampleTypesComboBox
        '
        Me.bsSampleTypesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsSampleTypesComboBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypesComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypesComboBox.FormattingEnabled = True
        Me.bsSampleTypesComboBox.Location = New System.Drawing.Point(425, 58)
        Me.bsSampleTypesComboBox.Name = "bsSampleTypesComboBox"
        Me.bsSampleTypesComboBox.Size = New System.Drawing.Size(284, 21)
        Me.bsSampleTypesComboBox.TabIndex = 1
        '
        'bsNameTextBox
        '
        Me.bsNameTextBox.BackColor = System.Drawing.Color.Khaki
        Me.bsNameTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.bsNameTextBox.DecimalsValues = False
        Me.bsNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNameTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsNameTextBox.IsNumeric = False
        Me.bsNameTextBox.Location = New System.Drawing.Point(10, 58)
        Me.bsNameTextBox.Mandatory = True
        Me.bsNameTextBox.MaxLength = 16
        Me.bsNameTextBox.Name = "bsNameTextBox"
        Me.bsNameTextBox.Size = New System.Drawing.Size(284, 21)
        Me.bsNameTextBox.TabIndex = 0
        Me.bsNameTextBox.WordWrap = False
        '
        'bsSampleTypeLabel
        '
        Me.bsSampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeLabel.Location = New System.Drawing.Point(425, 40)
        Me.bsSampleTypeLabel.Margin = New System.Windows.Forms.Padding(3, 2, 3, 0)
        Me.bsSampleTypeLabel.Name = "bsSampleTypeLabel"
        Me.bsSampleTypeLabel.Size = New System.Drawing.Size(284, 13)
        Me.bsSampleTypeLabel.TabIndex = 1
        Me.bsSampleTypeLabel.Text = "Sample Type:"
        Me.bsSampleTypeLabel.Title = False
        '
        'bsNameLabel
        '
        Me.bsNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsNameLabel.ForeColor = System.Drawing.Color.Black
        Me.bsNameLabel.Location = New System.Drawing.Point(10, 40)
        Me.bsNameLabel.Margin = New System.Windows.Forms.Padding(3, 2, 3, 0)
        Me.bsNameLabel.Name = "bsNameLabel"
        Me.bsNameLabel.Size = New System.Drawing.Size(284, 13)
        Me.bsNameLabel.TabIndex = 0
        Me.bsNameLabel.Text = "Name:"
        Me.bsNameLabel.Title = False
        '
        'bsTestSelGrpbox
        '
        Me.bsTestSelGrpbox.Controls.Add(Me.bsTestsTypesComboBox)
        Me.bsTestSelGrpbox.Controls.Add(Me.bsTestTypeLabel)
        Me.bsTestSelGrpbox.Controls.Add(Me.bsSaveButton)
        Me.bsTestSelGrpbox.Controls.Add(Me.bsCancelButton)
        Me.bsTestSelGrpbox.Controls.Add(Me.bsTestsSelectionDoubleList)
        Me.bsTestSelGrpbox.Controls.Add(Me.bsTestsSelectionLabel)
        Me.bsTestSelGrpbox.ForeColor = System.Drawing.Color.Black
        Me.bsTestSelGrpbox.Location = New System.Drawing.Point(249, 109)
        Me.bsTestSelGrpbox.Name = "bsTestSelGrpbox"
        Me.bsTestSelGrpbox.Size = New System.Drawing.Size(719, 500)
        Me.bsTestSelGrpbox.TabIndex = 6
        Me.bsTestSelGrpbox.TabStop = False
        '
        'bsTestsTypesComboBox
        '
        Me.bsTestsTypesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsTestsTypesComboBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestsTypesComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestsTypesComboBox.FormattingEnabled = True
        Me.bsTestsTypesComboBox.Location = New System.Drawing.Point(10, 58)
        Me.bsTestsTypesComboBox.Name = "bsTestsTypesComboBox"
        Me.bsTestsTypesComboBox.Size = New System.Drawing.Size(162, 21)
        Me.bsTestsTypesComboBox.TabIndex = 0
        '
        'bsTestTypeLabel
        '
        Me.bsTestTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTestTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestTypeLabel.Location = New System.Drawing.Point(10, 40)
        Me.bsTestTypeLabel.Margin = New System.Windows.Forms.Padding(3, 2, 3, 0)
        Me.bsTestTypeLabel.Name = "bsTestTypeLabel"
        Me.bsTestTypeLabel.Size = New System.Drawing.Size(138, 13)
        Me.bsTestTypeLabel.TabIndex = 40
        Me.bsTestTypeLabel.Text = "Test Type:"
        Me.bsTestTypeLabel.Title = False
        '
        'bsSaveButton
        '
        Me.bsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSaveButton.ForeColor = System.Drawing.Color.Black
        Me.bsSaveButton.Location = New System.Drawing.Point(640, 459)
        Me.bsSaveButton.Name = "bsSaveButton"
        Me.bsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveButton.TabIndex = 2
        Me.bsSaveButton.UseVisualStyleBackColor = True
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.ForeColor = System.Drawing.Color.Black
        Me.bsCancelButton.Location = New System.Drawing.Point(677, 459)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 3
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsTestsSelectionDoubleList
        '
        Me.bsTestsSelectionDoubleList.BackColor = System.Drawing.Color.Gainsboro
        Me.bsTestsSelectionDoubleList.FactoryValueMessage = ""
        Me.bsTestsSelectionDoubleList.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsTestsSelectionDoubleList.Location = New System.Drawing.Point(149, 84)
        Me.bsTestsSelectionDoubleList.MultiSelection = False
        Me.bsTestsSelectionDoubleList.Name = "bsTestsSelectionDoubleList"
        Me.bsTestsSelectionDoubleList.SelectableElementsTitle = "Selectable Tests:"
        Me.bsTestsSelectionDoubleList.SelectAllToolTip = Nothing
        Me.bsTestsSelectionDoubleList.SelectedElementsTitle = "Selected Tests:"
        Me.bsTestsSelectionDoubleList.SelectSomeToolTip = Nothing
        Me.bsTestsSelectionDoubleList.Size = New System.Drawing.Size(450, 369)
        Me.bsTestsSelectionDoubleList.Sorted = False
        Me.bsTestsSelectionDoubleList.TabIndex = 1
        Me.bsTestsSelectionDoubleList.UnselectAllToolTip = Nothing
        Me.bsTestsSelectionDoubleList.UnselectSomeToolTip = Nothing
        '
        'bsTestsSelectionLabel
        '
        Me.bsTestsSelectionLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTestsSelectionLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTestsSelectionLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestsSelectionLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsTestsSelectionLabel.Name = "bsTestsSelectionLabel"
        Me.bsTestsSelectionLabel.Size = New System.Drawing.Size(699, 20)
        Me.bsTestsSelectionLabel.TabIndex = 18
        Me.bsTestsSelectionLabel.Text = "Tests Selection"
        Me.bsTestsSelectionLabel.Title = True
        '
        'bsTestProfilesListView
        '
        Me.bsTestProfilesListView.AutoArrange = False
        Me.bsTestProfilesListView.BackColor = System.Drawing.Color.White
        Me.bsTestProfilesListView.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsTestProfilesListView.ForeColor = System.Drawing.Color.Black
        Me.bsTestProfilesListView.FullRowSelect = True
        Me.bsTestProfilesListView.HideSelection = False
        Me.bsTestProfilesListView.Location = New System.Drawing.Point(5, 40)
        Me.bsTestProfilesListView.Name = "bsTestProfilesListView"
        Me.bsTestProfilesListView.Size = New System.Drawing.Size(224, 550)
        Me.bsTestProfilesListView.TabIndex = 0
        Me.bsTestProfilesListView.UseCompatibleStateImageBehavior = False
        Me.bsTestProfilesListView.View = System.Windows.Forms.View.Details
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.ForeColor = System.Drawing.Color.Black
        Me.bsExitButton.Location = New System.Drawing.Point(926, 613)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 7
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsTestProfileLabel
        '
        Me.bsTestProfileLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTestProfileLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTestProfileLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestProfileLabel.Location = New System.Drawing.Point(5, 15)
        Me.bsTestProfileLabel.Name = "bsTestProfileLabel"
        Me.bsTestProfileLabel.Size = New System.Drawing.Size(224, 20)
        Me.bsTestProfileLabel.TabIndex = 38
        Me.bsTestProfileLabel.Text = "Test Profile"
        Me.bsTestProfileLabel.Title = True
        '
        'bsScreenErrorProvider
        '
        Me.bsScreenErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsScreenErrorProvider.ContainerControl = Me
        '
        'bsTestProfileListGroupBox
        '
        Me.bsTestProfileListGroupBox.Controls.Add(Me.bsTestProfileLabel)
        Me.bsTestProfileListGroupBox.Controls.Add(Me.bsTestProfilesListView)
        Me.bsTestProfileListGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestProfileListGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsTestProfileListGroupBox.Name = "bsTestProfileListGroupBox"
        Me.bsTestProfileListGroupBox.Size = New System.Drawing.Size(234, 598)
        Me.bsTestProfileListGroupBox.TabIndex = 0
        Me.bsTestProfileListGroupBox.TabStop = False
        '
        'BsCustomOrderButton
        '
        Me.BsCustomOrderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsCustomOrderButton.ForeColor = System.Drawing.Color.Black
        Me.BsCustomOrderButton.Location = New System.Drawing.Point(175, 613)
        Me.BsCustomOrderButton.Name = "BsCustomOrderButton"
        Me.BsCustomOrderButton.Size = New System.Drawing.Size(32, 32)
        Me.BsCustomOrderButton.TabIndex = 4
        Me.BsCustomOrderButton.UseVisualStyleBackColor = True
        '
        'IProgTestProfiles
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 654)
        Me.Controls.Add(Me.BsCustomOrderButton)
        Me.Controls.Add(Me.bsTestProfileListGroupBox)
        Me.Controls.Add(Me.bsNewButton)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsEditButton)
        Me.Controls.Add(Me.bsPrintButton)
        Me.Controls.Add(Me.bsDeleteButton)
        Me.Controls.Add(Me.bsProfileNameGrpbox)
        Me.Controls.Add(Me.bsTestSelGrpbox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiProgTestProfiles"
        Me.ShowInTaskbar = False
        Me.Text = ""
        Me.bsProfileNameGrpbox.ResumeLayout(False)
        Me.bsProfileNameGrpbox.PerformLayout()
        Me.bsTestSelGrpbox.ResumeLayout(False)
        CType(Me.bsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsTestProfileListGroupBox.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsTestProfilesListView As Biosystems.Ax00.Controls.UserControls.BSListView
    Friend WithEvents bsProfileNameGrpbox As Biosystems.Ax00.Controls.Usercontrols.BSGroupBox
    Friend WithEvents bsTestProfileDefinitionLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsSampleTypesComboBox As Biosystems.Ax00.Controls.Usercontrols.BSComboBox
    Friend WithEvents bsNameTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextbox
    Friend WithEvents bsSampleTypeLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents bsTestSelGrpbox As Biosystems.Ax00.Controls.Usercontrols.BSGroupBox
    Friend WithEvents bsTestsSelectionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsNewButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDeleteButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsEditButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsTestProfileLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTestsSelectionDoubleList As Biosystems.Ax00.Controls.UserControls.BSDoubleList
    Friend WithEvents bsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsTestsTypesComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsTestTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTestProfileListGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsCustomOrderButton As Biosystems.Ax00.Controls.UserControls.BSButton

End Class
