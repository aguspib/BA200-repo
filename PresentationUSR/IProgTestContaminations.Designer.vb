<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiProgTestContaminations
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
        Me.bsTestListGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsContaminatorsListLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsTestContaminatorsListView = New Biosystems.Ax00.Controls.UserControls.BSListView
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsEditButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsContaminatedDetailsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsR1GroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsR2GroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsR2ContaminatedDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsContaminesAllR2Checkbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bsR1ContaminatedDataGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsContaminesAllR1Checkbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bsCuvettesGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsWashingSolR2ComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsWashingSolR2Label = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsWashingSolR1ComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.bsWashingSolR1Label = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsContaminatesCuvettesCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.bsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsContaminationsLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsSummaryByTestButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsDeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsTestListGroupBox.SuspendLayout()
        Me.bsContaminatedDetailsGroupBox.SuspendLayout()
        Me.bsR1GroupBox.SuspendLayout()
        Me.bsR2GroupBox.SuspendLayout()
        CType(Me.bsR2ContaminatedDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsR1ContaminatedDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsCuvettesGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsTestListGroupBox
        '
        Me.bsTestListGroupBox.Controls.Add(Me.bsContaminatorsListLabel)
        Me.bsTestListGroupBox.Controls.Add(Me.bsTestContaminatorsListView)
        Me.bsTestListGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsTestListGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsTestListGroupBox.Name = "bsTestListGroupBox"
        Me.bsTestListGroupBox.Size = New System.Drawing.Size(234, 598)
        Me.bsTestListGroupBox.TabIndex = 42
        Me.bsTestListGroupBox.TabStop = False
        '
        'bsContaminatorsListLabel
        '
        Me.bsContaminatorsListLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsContaminatorsListLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsContaminatorsListLabel.ForeColor = System.Drawing.Color.Black
        Me.bsContaminatorsListLabel.Location = New System.Drawing.Point(5, 15)
        Me.bsContaminatorsListLabel.Name = "bsContaminatorsListLabel"
        Me.bsContaminatorsListLabel.Size = New System.Drawing.Size(224, 20)
        Me.bsContaminatorsListLabel.TabIndex = 17
        Me.bsContaminatorsListLabel.Text = "Contaminators"
        Me.bsContaminatorsListLabel.Title = True
        '
        'bsTestContaminatorsListView
        '
        Me.bsTestContaminatorsListView.AllowColumnReorder = True
        Me.bsTestContaminatorsListView.AutoArrange = False
        Me.bsTestContaminatorsListView.BackColor = System.Drawing.Color.White
        Me.bsTestContaminatorsListView.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsTestContaminatorsListView.ForeColor = System.Drawing.Color.Black
        Me.bsTestContaminatorsListView.FullRowSelect = True
        Me.bsTestContaminatorsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.bsTestContaminatorsListView.HideSelection = False
        Me.bsTestContaminatorsListView.Location = New System.Drawing.Point(5, 40)
        Me.bsTestContaminatorsListView.Name = "bsTestContaminatorsListView"
        Me.bsTestContaminatorsListView.Size = New System.Drawing.Size(224, 550)
        Me.bsTestContaminatorsListView.TabIndex = 1
        Me.bsTestContaminatorsListView.UseCompatibleStateImageBehavior = False
        Me.bsTestContaminatorsListView.View = System.Windows.Forms.View.Details
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Location = New System.Drawing.Point(212, 613)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 4
        Me.bsPrintButton.UseVisualStyleBackColor = True
        '
        'bsEditButton
        '
        Me.bsEditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsEditButton.Location = New System.Drawing.Point(138, 613)
        Me.bsEditButton.Name = "bsEditButton"
        Me.bsEditButton.Size = New System.Drawing.Size(32, 32)
        Me.bsEditButton.TabIndex = 2
        Me.bsEditButton.UseVisualStyleBackColor = True
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(923, 613)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 6
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsContaminatedDetailsGroupBox
        '
        Me.bsContaminatedDetailsGroupBox.Controls.Add(Me.bsR1GroupBox)
        Me.bsContaminatedDetailsGroupBox.Controls.Add(Me.bsCuvettesGroupBox)
        Me.bsContaminatedDetailsGroupBox.Controls.Add(Me.bsSaveButton)
        Me.bsContaminatedDetailsGroupBox.Controls.Add(Me.bsCancelButton)
        Me.bsContaminatedDetailsGroupBox.Controls.Add(Me.bsContaminationsLabel)
        Me.bsContaminatedDetailsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsContaminatedDetailsGroupBox.Location = New System.Drawing.Point(249, 10)
        Me.bsContaminatedDetailsGroupBox.Name = "bsContaminatedDetailsGroupBox"
        Me.bsContaminatedDetailsGroupBox.Size = New System.Drawing.Size(719, 598)
        Me.bsContaminatedDetailsGroupBox.TabIndex = 7
        Me.bsContaminatedDetailsGroupBox.TabStop = False
        '
        'bsR1GroupBox
        '
        Me.bsR1GroupBox.Controls.Add(Me.bsR2GroupBox)
        Me.bsR1GroupBox.Controls.Add(Me.bsR1ContaminatedDataGridView)
        Me.bsR1GroupBox.Controls.Add(Me.bsContaminesAllR1Checkbox)
        Me.bsR1GroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsR1GroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsR1GroupBox.Location = New System.Drawing.Point(10, 40)
        Me.bsR1GroupBox.Name = "bsR1GroupBox"
        Me.bsR1GroupBox.Size = New System.Drawing.Size(436, 545)
        Me.bsR1GroupBox.TabIndex = 2
        Me.bsR1GroupBox.TabStop = False
        Me.bsR1GroupBox.Text = "R1"
        '
        'bsR2GroupBox
        '
        Me.bsR2GroupBox.Controls.Add(Me.bsR2ContaminatedDataGridView)
        Me.bsR2GroupBox.Controls.Add(Me.bsContaminesAllR2Checkbox)
        Me.bsR2GroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsR2GroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsR2GroupBox.Location = New System.Drawing.Point(3, 285)
        Me.bsR2GroupBox.Name = "bsR2GroupBox"
        Me.bsR2GroupBox.Size = New System.Drawing.Size(434, 136)
        Me.bsR2GroupBox.TabIndex = 3
        Me.bsR2GroupBox.TabStop = False
        Me.bsR2GroupBox.Text = "R2"
        Me.bsR2GroupBox.Visible = False
        '
        'bsR2ContaminatedDataGridView
        '
        Me.bsR2ContaminatedDataGridView.AllowUserToAddRows = False
        Me.bsR2ContaminatedDataGridView.AllowUserToDeleteRows = False
        Me.bsR2ContaminatedDataGridView.AllowUserToResizeColumns = False
        Me.bsR2ContaminatedDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsR2ContaminatedDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsR2ContaminatedDataGridView.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsR2ContaminatedDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsR2ContaminatedDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsR2ContaminatedDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsR2ContaminatedDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsR2ContaminatedDataGridView.ColumnHeadersHeight = 20
        Me.bsR2ContaminatedDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsR2ContaminatedDataGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsR2ContaminatedDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsR2ContaminatedDataGridView.EnterToTab = False
        Me.bsR2ContaminatedDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsR2ContaminatedDataGridView.Location = New System.Drawing.Point(20, 43)
        Me.bsR2ContaminatedDataGridView.MultiSelect = False
        Me.bsR2ContaminatedDataGridView.Name = "bsR2ContaminatedDataGridView"
        Me.bsR2ContaminatedDataGridView.ReadOnly = True
        Me.bsR2ContaminatedDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsR2ContaminatedDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsR2ContaminatedDataGridView.RowHeadersVisible = False
        Me.bsR2ContaminatedDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsR2ContaminatedDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsR2ContaminatedDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsR2ContaminatedDataGridView.Size = New System.Drawing.Size(392, 70)
        Me.bsR2ContaminatedDataGridView.TabIndex = 2
        Me.bsR2ContaminatedDataGridView.TabToEnter = False
        '
        'bsContaminesAllR2Checkbox
        '
        Me.bsContaminesAllR2Checkbox.AutoSize = True
        Me.bsContaminesAllR2Checkbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsContaminesAllR2Checkbox.ForeColor = System.Drawing.Color.Black
        Me.bsContaminesAllR2Checkbox.Location = New System.Drawing.Point(20, 20)
        Me.bsContaminesAllR2Checkbox.Name = "bsContaminesAllR2Checkbox"
        Me.bsContaminesAllR2Checkbox.Size = New System.Drawing.Size(170, 17)
        Me.bsContaminesAllR2Checkbox.TabIndex = 1
        Me.bsContaminesAllR2Checkbox.Text = "Contaminates everything"
        Me.bsContaminesAllR2Checkbox.UseVisualStyleBackColor = True
        '
        'bsR1ContaminatedDataGridView
        '
        Me.bsR1ContaminatedDataGridView.AllowUserToAddRows = False
        Me.bsR1ContaminatedDataGridView.AllowUserToDeleteRows = False
        Me.bsR1ContaminatedDataGridView.AllowUserToResizeColumns = False
        Me.bsR1ContaminatedDataGridView.AllowUserToResizeRows = False
        DataGridViewCellStyle6.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsR1ContaminatedDataGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle6
        Me.bsR1ContaminatedDataGridView.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bsR1ContaminatedDataGridView.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsR1ContaminatedDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsR1ContaminatedDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsR1ContaminatedDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.bsR1ContaminatedDataGridView.ColumnHeadersHeight = 20
        Me.bsR1ContaminatedDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsR1ContaminatedDataGridView.DefaultCellStyle = DataGridViewCellStyle8
        Me.bsR1ContaminatedDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsR1ContaminatedDataGridView.EnterToTab = False
        Me.bsR1ContaminatedDataGridView.GridColor = System.Drawing.Color.Silver
        Me.bsR1ContaminatedDataGridView.Location = New System.Drawing.Point(10, 50)
        Me.bsR1ContaminatedDataGridView.MultiSelect = False
        Me.bsR1ContaminatedDataGridView.Name = "bsR1ContaminatedDataGridView"
        Me.bsR1ContaminatedDataGridView.ReadOnly = True
        Me.bsR1ContaminatedDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.bsR1ContaminatedDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.bsR1ContaminatedDataGridView.RowHeadersVisible = False
        Me.bsR1ContaminatedDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle10.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsR1ContaminatedDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle10
        Me.bsR1ContaminatedDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsR1ContaminatedDataGridView.Size = New System.Drawing.Size(415, 480)
        Me.bsR1ContaminatedDataGridView.TabIndex = 2
        Me.bsR1ContaminatedDataGridView.TabToEnter = False
        '
        'bsContaminesAllR1Checkbox
        '
        Me.bsContaminesAllR1Checkbox.AutoSize = True
        Me.bsContaminesAllR1Checkbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsContaminesAllR1Checkbox.ForeColor = System.Drawing.Color.Black
        Me.bsContaminesAllR1Checkbox.Location = New System.Drawing.Point(10, 20)
        Me.bsContaminesAllR1Checkbox.Name = "bsContaminesAllR1Checkbox"
        Me.bsContaminesAllR1Checkbox.Size = New System.Drawing.Size(170, 17)
        Me.bsContaminesAllR1Checkbox.TabIndex = 1
        Me.bsContaminesAllR1Checkbox.Text = "Contaminates everything"
        Me.bsContaminesAllR1Checkbox.UseVisualStyleBackColor = True
        '
        'bsCuvettesGroupBox
        '
        Me.bsCuvettesGroupBox.Controls.Add(Me.bsWashingSolR2ComboBox)
        Me.bsCuvettesGroupBox.Controls.Add(Me.bsWashingSolR2Label)
        Me.bsCuvettesGroupBox.Controls.Add(Me.bsWashingSolR1ComboBox)
        Me.bsCuvettesGroupBox.Controls.Add(Me.bsWashingSolR1Label)
        Me.bsCuvettesGroupBox.Controls.Add(Me.bsContaminatesCuvettesCheckbox)
        Me.bsCuvettesGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsCuvettesGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsCuvettesGroupBox.Location = New System.Drawing.Point(453, 40)
        Me.bsCuvettesGroupBox.Name = "bsCuvettesGroupBox"
        Me.bsCuvettesGroupBox.Size = New System.Drawing.Size(256, 143)
        Me.bsCuvettesGroupBox.TabIndex = 4
        Me.bsCuvettesGroupBox.TabStop = False
        Me.bsCuvettesGroupBox.Text = "Cuvettes"
        '
        'bsWashingSolR2ComboBox
        '
        Me.bsWashingSolR2ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsWashingSolR2ComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsWashingSolR2ComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsWashingSolR2ComboBox.FormattingEnabled = True
        Me.bsWashingSolR2ComboBox.Location = New System.Drawing.Point(10, 110)
        Me.bsWashingSolR2ComboBox.Name = "bsWashingSolR2ComboBox"
        Me.bsWashingSolR2ComboBox.Size = New System.Drawing.Size(233, 21)
        Me.bsWashingSolR2ComboBox.TabIndex = 3
        '
        'bsWashingSolR2Label
        '
        Me.bsWashingSolR2Label.BackColor = System.Drawing.Color.Transparent
        Me.bsWashingSolR2Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsWashingSolR2Label.ForeColor = System.Drawing.Color.Black
        Me.bsWashingSolR2Label.Location = New System.Drawing.Point(10, 95)
        Me.bsWashingSolR2Label.Name = "bsWashingSolR2Label"
        Me.bsWashingSolR2Label.Size = New System.Drawing.Size(140, 13)
        Me.bsWashingSolR2Label.TabIndex = 21
        Me.bsWashingSolR2Label.Text = "R2:"
        Me.bsWashingSolR2Label.Title = False
        '
        'bsWashingSolR1ComboBox
        '
        Me.bsWashingSolR1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.bsWashingSolR1ComboBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsWashingSolR1ComboBox.ForeColor = System.Drawing.Color.Black
        Me.bsWashingSolR1ComboBox.FormattingEnabled = True
        Me.bsWashingSolR1ComboBox.Location = New System.Drawing.Point(10, 60)
        Me.bsWashingSolR1ComboBox.Name = "bsWashingSolR1ComboBox"
        Me.bsWashingSolR1ComboBox.Size = New System.Drawing.Size(233, 21)
        Me.bsWashingSolR1ComboBox.TabIndex = 2
        '
        'bsWashingSolR1Label
        '
        Me.bsWashingSolR1Label.BackColor = System.Drawing.Color.Transparent
        Me.bsWashingSolR1Label.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsWashingSolR1Label.ForeColor = System.Drawing.Color.Black
        Me.bsWashingSolR1Label.Location = New System.Drawing.Point(10, 45)
        Me.bsWashingSolR1Label.Name = "bsWashingSolR1Label"
        Me.bsWashingSolR1Label.Size = New System.Drawing.Size(140, 13)
        Me.bsWashingSolR1Label.TabIndex = 19
        Me.bsWashingSolR1Label.Text = "R1:"
        Me.bsWashingSolR1Label.Title = False
        '
        'bsContaminatesCuvettesCheckbox
        '
        Me.bsContaminatesCuvettesCheckbox.AutoSize = True
        Me.bsContaminatesCuvettesCheckbox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsContaminatesCuvettesCheckbox.ForeColor = System.Drawing.Color.Black
        Me.bsContaminatesCuvettesCheckbox.Location = New System.Drawing.Point(10, 20)
        Me.bsContaminatesCuvettesCheckbox.Name = "bsContaminatesCuvettesCheckbox"
        Me.bsContaminatesCuvettesCheckbox.Size = New System.Drawing.Size(160, 17)
        Me.bsContaminatesCuvettesCheckbox.TabIndex = 1
        Me.bsContaminatesCuvettesCheckbox.Text = "Contaminates Cuvettes"
        Me.bsContaminatesCuvettesCheckbox.UseVisualStyleBackColor = True
        '
        'bsSaveButton
        '
        Me.bsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSaveButton.Location = New System.Drawing.Point(637, 556)
        Me.bsSaveButton.Name = "bsSaveButton"
        Me.bsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSaveButton.TabIndex = 5
        Me.bsSaveButton.UseVisualStyleBackColor = True
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Location = New System.Drawing.Point(674, 556)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 6
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsContaminationsLabel
        '
        Me.bsContaminationsLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsContaminationsLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsContaminationsLabel.ForeColor = System.Drawing.Color.Black
        Me.bsContaminationsLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsContaminationsLabel.Name = "bsContaminationsLabel"
        Me.bsContaminationsLabel.Size = New System.Drawing.Size(699, 20)
        Me.bsContaminationsLabel.TabIndex = 14
        Me.bsContaminationsLabel.Text = "Contaminations"
        Me.bsContaminationsLabel.Title = True
        '
        'bsSummaryByTestButton
        '
        Me.bsSummaryByTestButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsSummaryByTestButton.Location = New System.Drawing.Point(10, 613)
        Me.bsSummaryByTestButton.Name = "bsSummaryByTestButton"
        Me.bsSummaryByTestButton.Size = New System.Drawing.Size(32, 32)
        Me.bsSummaryByTestButton.TabIndex = 5
        Me.bsSummaryByTestButton.UseVisualStyleBackColor = True
        '
        'bsDeleteButton
        '
        Me.bsDeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsDeleteButton.Location = New System.Drawing.Point(175, 613)
        Me.bsDeleteButton.Name = "bsDeleteButton"
        Me.bsDeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.bsDeleteButton.TabIndex = 3
        Me.bsDeleteButton.UseVisualStyleBackColor = True
        '
        'IProgTestContaminations
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
        Me.Controls.Add(Me.bsContaminatedDetailsGroupBox)
        Me.Controls.Add(Me.bsDeleteButton)
        Me.Controls.Add(Me.bsSummaryByTestButton)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsTestListGroupBox)
        Me.Controls.Add(Me.bsPrintButton)
        Me.Controls.Add(Me.bsEditButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "IProgTestContaminations"
        Me.ShowInTaskbar = False
        Me.Text = ""
        Me.bsTestListGroupBox.ResumeLayout(False)
        Me.bsContaminatedDetailsGroupBox.ResumeLayout(False)
        Me.bsR1GroupBox.ResumeLayout(False)
        Me.bsR1GroupBox.PerformLayout()
        Me.bsR2GroupBox.ResumeLayout(False)
        Me.bsR2GroupBox.PerformLayout()
        CType(Me.bsR2ContaminatedDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsR1ContaminatedDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsCuvettesGroupBox.ResumeLayout(False)
        Me.bsCuvettesGroupBox.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsTestListGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsContaminatorsListLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTestContaminatorsListView As Biosystems.Ax00.Controls.UserControls.BSListView
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsEditButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsContaminatedDetailsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsContaminesAllR1Checkbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsR1GroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsContaminationsLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsR2GroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsContaminesAllR2Checkbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsCuvettesGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsContaminatesCuvettesCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsWashingSolR2ComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsWashingSolR2Label As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsWashingSolR1ComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsWashingSolR1Label As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsR2ContaminatedDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsR1ContaminatedDataGridView As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsSummaryByTestButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsDeleteButton As Biosystems.Ax00.Controls.UserControls.BSButton

End Class
