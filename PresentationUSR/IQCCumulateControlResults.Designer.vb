<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IQCCumulateControlResults
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
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IQCCumulateControlResults))
        Me.bsControlListGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsControlsListLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsControlsListView = New Biosystems.Ax00.Controls.UserControls.BSListView
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsControlGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsTestListToCumGrid = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsDataToCumLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsLastCumValuesLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsCumulateButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsCurrentLotNumberTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.bsTestListGrid = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.bsLotNumberLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsResultsByTestSampleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsEditButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsControlListGroupBox.SuspendLayout()
        Me.bsControlGroupBox.SuspendLayout()
        CType(Me.bsTestListToCumGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsTestListGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsControlListGroupBox
        '
        Me.bsControlListGroupBox.Controls.Add(Me.bsControlsListLabel)
        Me.bsControlListGroupBox.Controls.Add(Me.bsControlsListView)
        Me.bsControlListGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsControlListGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsControlListGroupBox.Name = "bsControlListGroupBox"
        Me.bsControlListGroupBox.Size = New System.Drawing.Size(234, 598)
        Me.bsControlListGroupBox.TabIndex = 42
        Me.bsControlListGroupBox.TabStop = False
        '
        'bsControlsListLabel
        '
        Me.bsControlsListLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsControlsListLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsControlsListLabel.ForeColor = System.Drawing.Color.Black
        Me.bsControlsListLabel.Location = New System.Drawing.Point(5, 15)
        Me.bsControlsListLabel.Name = "bsControlsListLabel"
        Me.bsControlsListLabel.Size = New System.Drawing.Size(224, 20)
        Me.bsControlsListLabel.TabIndex = 0
        Me.bsControlsListLabel.Text = "*Controls List"
        Me.bsControlsListLabel.Title = True
        '
        'bsControlsListView
        '
        Me.bsControlsListView.AllowColumnReorder = True
        Me.bsControlsListView.AutoArrange = False
        Me.bsControlsListView.BackColor = System.Drawing.Color.White
        Me.bsControlsListView.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsControlsListView.ForeColor = System.Drawing.Color.Black
        Me.bsControlsListView.FullRowSelect = True
        Me.bsControlsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.bsControlsListView.HideSelection = False
        Me.bsControlsListView.Location = New System.Drawing.Point(5, 40)
        Me.bsControlsListView.Name = "bsControlsListView"
        Me.bsControlsListView.Size = New System.Drawing.Size(224, 550)
        Me.bsControlsListView.TabIndex = 0
        Me.bsControlsListView.UseCompatibleStateImageBehavior = False
        Me.bsControlsListView.View = System.Windows.Forms.View.Details
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsExitButton.Location = New System.Drawing.Point(923, 613)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 3
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsControlGroupBox
        '
        Me.bsControlGroupBox.Controls.Add(Me.bsTestListToCumGrid)
        Me.bsControlGroupBox.Controls.Add(Me.bsDataToCumLabel)
        Me.bsControlGroupBox.Controls.Add(Me.bsLastCumValuesLabel)
        Me.bsControlGroupBox.Controls.Add(Me.bsCumulateButton)
        Me.bsControlGroupBox.Controls.Add(Me.bsCancelButton)
        Me.bsControlGroupBox.Controls.Add(Me.bsCurrentLotNumberTextBox)
        Me.bsControlGroupBox.Controls.Add(Me.bsTestListGrid)
        Me.bsControlGroupBox.Controls.Add(Me.bsLotNumberLabel)
        Me.bsControlGroupBox.Controls.Add(Me.bsResultsByTestSampleLabel)
        Me.bsControlGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsControlGroupBox.Location = New System.Drawing.Point(249, 10)
        Me.bsControlGroupBox.Name = "bsControlGroupBox"
        Me.bsControlGroupBox.Size = New System.Drawing.Size(719, 598)
        Me.bsControlGroupBox.TabIndex = 40
        Me.bsControlGroupBox.TabStop = False
        '
        'bsTestListToCumGrid
        '
        Me.bsTestListToCumGrid.AllowUserToAddRows = False
        Me.bsTestListToCumGrid.AllowUserToDeleteRows = False
        Me.bsTestListToCumGrid.AllowUserToResizeColumns = False
        Me.bsTestListToCumGrid.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListToCumGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsTestListToCumGrid.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsTestListToCumGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsTestListToCumGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListToCumGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsTestListToCumGrid.ColumnHeadersHeight = 20
        Me.bsTestListToCumGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListToCumGrid.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsTestListToCumGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.bsTestListToCumGrid.EnterToTab = True
        Me.bsTestListToCumGrid.GridColor = System.Drawing.Color.Silver
        Me.bsTestListToCumGrid.Location = New System.Drawing.Point(10, 370)
        Me.bsTestListToCumGrid.MultiSelect = False
        Me.bsTestListToCumGrid.Name = "bsTestListToCumGrid"
        Me.bsTestListToCumGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListToCumGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsTestListToCumGrid.RowHeadersVisible = False
        Me.bsTestListToCumGrid.RowHeadersWidth = 20
        Me.bsTestListToCumGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListToCumGrid.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsTestListToCumGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsTestListToCumGrid.ShowCellErrors = False
        Me.bsTestListToCumGrid.Size = New System.Drawing.Size(696, 185)
        Me.bsTestListToCumGrid.TabIndex = 59
        Me.bsTestListToCumGrid.TabToEnter = False
        '
        'bsDataToCumLabel
        '
        Me.bsDataToCumLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsDataToCumLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsDataToCumLabel.ForeColor = System.Drawing.Color.Black
        Me.bsDataToCumLabel.Location = New System.Drawing.Point(10, 352)
        Me.bsDataToCumLabel.Name = "bsDataToCumLabel"
        Me.bsDataToCumLabel.Size = New System.Drawing.Size(325, 13)
        Me.bsDataToCumLabel.TabIndex = 58
        Me.bsDataToCumLabel.Text = "*Data to Accumulate:"
        Me.bsDataToCumLabel.Title = False
        '
        'bsLastCumValuesLabel
        '
        Me.bsLastCumValuesLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLastCumValuesLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLastCumValuesLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLastCumValuesLabel.Location = New System.Drawing.Point(10, 124)
        Me.bsLastCumValuesLabel.Name = "bsLastCumValuesLabel"
        Me.bsLastCumValuesLabel.Size = New System.Drawing.Size(325, 13)
        Me.bsLastCumValuesLabel.TabIndex = 57
        Me.bsLastCumValuesLabel.Text = "*Last Accumulated Values:"
        Me.bsLastCumValuesLabel.Title = False
        '
        'bsCumulateButton
        '
        Me.bsCumulateButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCumulateButton.Location = New System.Drawing.Point(637, 560)
        Me.bsCumulateButton.Name = "bsCumulateButton"
        Me.bsCumulateButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCumulateButton.TabIndex = 45
        Me.bsCumulateButton.UseVisualStyleBackColor = True
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Location = New System.Drawing.Point(674, 560)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 46
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'bsCurrentLotNumberTextBox
        '
        Me.bsCurrentLotNumberTextBox.BackColor = System.Drawing.Color.White
        Me.bsCurrentLotNumberTextBox.DecimalsValues = False
        Me.bsCurrentLotNumberTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsCurrentLotNumberTextBox.ForeColor = System.Drawing.Color.Black
        Me.bsCurrentLotNumberTextBox.IsNumeric = False
        Me.bsCurrentLotNumberTextBox.Location = New System.Drawing.Point(10, 78)
        Me.bsCurrentLotNumberTextBox.Mandatory = False
        Me.bsCurrentLotNumberTextBox.MaxLength = 16
        Me.bsCurrentLotNumberTextBox.Name = "bsCurrentLotNumberTextBox"
        Me.bsCurrentLotNumberTextBox.ReadOnly = True
        Me.bsCurrentLotNumberTextBox.Size = New System.Drawing.Size(220, 21)
        Me.bsCurrentLotNumberTextBox.TabIndex = 56
        Me.bsCurrentLotNumberTextBox.TabStop = False
        Me.bsCurrentLotNumberTextBox.WordWrap = False
        '
        'bsTestListGrid
        '
        Me.bsTestListGrid.AllowUserToAddRows = False
        Me.bsTestListGrid.AllowUserToDeleteRows = False
        Me.bsTestListGrid.AllowUserToResizeColumns = False
        Me.bsTestListGrid.AllowUserToResizeRows = False
        DataGridViewCellStyle6.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle6.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle6
        Me.bsTestListGrid.BackgroundColor = System.Drawing.Color.LightGray
        Me.bsTestListGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsTestListGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle7.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle7
        Me.bsTestListGrid.ColumnHeadersHeight = 20
        Me.bsTestListGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle8.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle8.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle8.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListGrid.DefaultCellStyle = DataGridViewCellStyle8
        Me.bsTestListGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.bsTestListGrid.EnterToTab = True
        Me.bsTestListGrid.GridColor = System.Drawing.Color.Silver
        Me.bsTestListGrid.Location = New System.Drawing.Point(10, 142)
        Me.bsTestListGrid.MultiSelect = False
        Me.bsTestListGrid.Name = "bsTestListGrid"
        Me.bsTestListGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle9.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle9
        Me.bsTestListGrid.RowHeadersVisible = False
        Me.bsTestListGrid.RowHeadersWidth = 20
        Me.bsTestListGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle10.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle10.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListGrid.RowsDefaultCellStyle = DataGridViewCellStyle10
        Me.bsTestListGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsTestListGrid.ShowCellErrors = False
        Me.bsTestListGrid.Size = New System.Drawing.Size(696, 185)
        Me.bsTestListGrid.TabIndex = 1
        Me.bsTestListGrid.TabToEnter = False
        '
        'bsLotNumberLabel
        '
        Me.bsLotNumberLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLotNumberLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLotNumberLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLotNumberLabel.Location = New System.Drawing.Point(10, 60)
        Me.bsLotNumberLabel.Name = "bsLotNumberLabel"
        Me.bsLotNumberLabel.Size = New System.Drawing.Size(325, 13)
        Me.bsLotNumberLabel.TabIndex = 0
        Me.bsLotNumberLabel.Text = "*Lot Number:"
        Me.bsLotNumberLabel.Title = False
        '
        'bsResultsByTestSampleLabel
        '
        Me.bsResultsByTestSampleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsResultsByTestSampleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsResultsByTestSampleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsResultsByTestSampleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsResultsByTestSampleLabel.Name = "bsResultsByTestSampleLabel"
        Me.bsResultsByTestSampleLabel.Size = New System.Drawing.Size(699, 20)
        Me.bsResultsByTestSampleLabel.TabIndex = 0
        Me.bsResultsByTestSampleLabel.Text = "*Results by Test/Sample Type"
        Me.bsResultsByTestSampleLabel.Title = True
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Location = New System.Drawing.Point(212, 613)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 44
        Me.bsPrintButton.UseVisualStyleBackColor = True
        '
        'bsEditButton
        '
        Me.bsEditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsEditButton.Location = New System.Drawing.Point(175, 613)
        Me.bsEditButton.Name = "bsEditButton"
        Me.bsEditButton.Size = New System.Drawing.Size(32, 32)
        Me.bsEditButton.TabIndex = 43
        Me.bsEditButton.UseVisualStyleBackColor = True
        '
        'IQCCumulateControlResults
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
        Me.Controls.Add(Me.bsPrintButton)
        Me.Controls.Add(Me.bsEditButton)
        Me.Controls.Add(Me.bsControlListGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsControlGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IQCCumulateControlResults"
        Me.ShowInTaskbar = False
        Me.bsControlListGroupBox.ResumeLayout(False)
        Me.bsControlGroupBox.ResumeLayout(False)
        Me.bsControlGroupBox.PerformLayout()
        CType(Me.bsTestListToCumGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsTestListGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsControlListGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsControlsListLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsControlsListView As Biosystems.Ax00.Controls.UserControls.BSListView
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsControlGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsLotNumberLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsResultsByTestSampleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTestListGrid As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsCurrentLotNumberTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsEditButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCumulateButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsTestListToCumGrid As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents bsDataToCumLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLastCumValuesLabel As Biosystems.Ax00.Controls.UserControls.BSLabel

End Class
