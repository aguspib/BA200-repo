Imports DevExpress.Utils

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiResultsAbsCurve
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
        Me.components = New System.ComponentModel.Container()
        Dim SideBySideBarSeriesLabel1 As DevExpress.XtraCharts.SideBySideBarSeriesLabel = New DevExpress.XtraCharts.SideBySideBarSeriesLabel()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiResultsAbsCurve))
        Me.bsErrorProvider1 = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider()
        Me.bsGraphToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.bsWell = New DevExpress.XtraEditors.TextEdit()
        Me.BsWellLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsReplicate = New DevExpress.XtraEditors.TextEdit()
        Me.BsReplicateLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsMultiItemLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsRerunLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsComodinLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.FilterComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.BsShowLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTestLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsTimer1 = New Biosystems.Ax00.Controls.UserControls.BSTimer()
        Me.ToolTipController1 = New DevExpress.Utils.ToolTipController(Me.components)
        Me.bsCloseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsLastButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsNextButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsPreviousButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsFirstButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsPrintButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsExpandButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsGroupBox1 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsRerunText = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsMultiItemText = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsSampleTypeText = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLotText = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTestText = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsComodinText = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsLotLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsClassPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsGraphResultLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsGroupBox2 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.ReplicateUpDown = New Biosystems.Ax00.Controls.UserControls.BSNumericUpDown()
        Me.AllReplicatesRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.ReplicateRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.bsGraphTypeLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsGroupBox3 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.ReplicatesGridControl = New DevExpress.XtraGrid.GridControl()
        Me.ReplicatesGridView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.ResultChartControl = New DevExpress.XtraCharts.ChartControl()
        Me.bsCollapseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsButton3 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsButton2 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.TextBox5 = New System.Windows.Forms.TextBox()
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsWell.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsReplicate.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsGroupBox1.SuspendLayout()
        CType(Me.bsClassPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsGroupBox2.SuspendLayout()
        CType(Me.ReplicateUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsGroupBox3.SuspendLayout()
        CType(Me.ReplicatesGridControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ReplicatesGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ResultChartControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(SideBySideBarSeriesLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsErrorProvider1
        '
        Me.bsErrorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.bsErrorProvider1.ContainerControl = Me
        '
        'bsWell
        '
        Me.bsWell.Enabled = False
        Me.bsWell.Location = New System.Drawing.Point(822, 671)
        Me.bsWell.Name = "bsWell"
        Me.bsWell.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.bsWell.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsWell.Properties.Appearance.Options.UseBackColor = True
        Me.bsWell.Properties.Appearance.Options.UseFont = True
        Me.bsWell.Properties.Appearance.Options.UseTextOptions = True
        Me.bsWell.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
        Me.bsWell.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.bsWell.Size = New System.Drawing.Size(35, 18)
        Me.bsWell.TabIndex = 46
        Me.bsWell.Visible = False
        '
        'BsWellLabel
        '
        Me.BsWellLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsWellLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsWellLabel.ForeColor = System.Drawing.Color.Black
        Me.BsWellLabel.Location = New System.Drawing.Point(737, 673)
        Me.BsWellLabel.Name = "BsWellLabel"
        Me.BsWellLabel.Size = New System.Drawing.Size(81, 13)
        Me.BsWellLabel.TabIndex = 44
        Me.BsWellLabel.Text = "Well:"
        Me.BsWellLabel.Title = False
        Me.BsWellLabel.Visible = False
        '
        'bsReplicate
        '
        Me.bsReplicate.Enabled = False
        Me.bsReplicate.Location = New System.Drawing.Point(822, 653)
        Me.bsReplicate.Name = "bsReplicate"
        Me.bsReplicate.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.bsReplicate.Properties.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsReplicate.Properties.Appearance.Options.UseBackColor = True
        Me.bsReplicate.Properties.Appearance.Options.UseFont = True
        Me.bsReplicate.Properties.Appearance.Options.UseTextOptions = True
        Me.bsReplicate.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
        Me.bsReplicate.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.bsReplicate.Size = New System.Drawing.Size(35, 18)
        Me.bsReplicate.TabIndex = 39
        Me.bsReplicate.Visible = False
        '
        'BsReplicateLabel
        '
        Me.BsReplicateLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsReplicateLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsReplicateLabel.ForeColor = System.Drawing.Color.Black
        Me.BsReplicateLabel.Location = New System.Drawing.Point(737, 655)
        Me.BsReplicateLabel.Name = "BsReplicateLabel"
        Me.BsReplicateLabel.Size = New System.Drawing.Size(90, 13)
        Me.BsReplicateLabel.TabIndex = 34
        Me.BsReplicateLabel.Text = "Replicate No:"
        Me.BsReplicateLabel.Title = False
        Me.BsReplicateLabel.Visible = False
        '
        'bsMultiItemLabel
        '
        Me.bsMultiItemLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsMultiItemLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsMultiItemLabel.ForeColor = System.Drawing.Color.Black
        Me.bsMultiItemLabel.Location = New System.Drawing.Point(616, 43)
        Me.bsMultiItemLabel.Name = "bsMultiItemLabel"
        Me.bsMultiItemLabel.Size = New System.Drawing.Size(71, 13)
        Me.bsMultiItemLabel.TabIndex = 45
        Me.bsMultiItemLabel.Text = "*Num.:"
        Me.bsMultiItemLabel.Title = False
        '
        'bsRerunLabel
        '
        Me.bsRerunLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsRerunLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRerunLabel.ForeColor = System.Drawing.Color.Black
        Me.bsRerunLabel.Location = New System.Drawing.Point(616, 83)
        Me.bsRerunLabel.Name = "bsRerunLabel"
        Me.bsRerunLabel.Size = New System.Drawing.Size(74, 13)
        Me.bsRerunLabel.TabIndex = 32
        Me.bsRerunLabel.Text = "*Repetition:"
        Me.bsRerunLabel.Title = False
        '
        'bsComodinLabel
        '
        Me.bsComodinLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsComodinLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsComodinLabel.ForeColor = System.Drawing.Color.Black
        Me.bsComodinLabel.Location = New System.Drawing.Point(70, 43)
        Me.bsComodinLabel.Name = "bsComodinLabel"
        Me.bsComodinLabel.Size = New System.Drawing.Size(235, 13)
        Me.bsComodinLabel.TabIndex = 41
        Me.bsComodinLabel.Text = "Patient ID/Sample ID:"
        Me.bsComodinLabel.Title = False
        '
        'FilterComboBox
        '
        Me.FilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.FilterComboBox.ForeColor = System.Drawing.Color.Black
        Me.FilterComboBox.FormattingEnabled = True
        Me.FilterComboBox.Location = New System.Drawing.Point(161, 66)
        Me.FilterComboBox.Name = "FilterComboBox"
        Me.FilterComboBox.Size = New System.Drawing.Size(106, 21)
        Me.FilterComboBox.TabIndex = 77
        '
        'BsShowLabel
        '
        Me.BsShowLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsShowLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsShowLabel.ForeColor = System.Drawing.Color.Black
        Me.BsShowLabel.Location = New System.Drawing.Point(158, 45)
        Me.BsShowLabel.Name = "BsShowLabel"
        Me.BsShowLabel.Size = New System.Drawing.Size(64, 13)
        Me.BsShowLabel.TabIndex = 75
        Me.BsShowLabel.Text = "Graph:"
        Me.BsShowLabel.Title = False
        '
        'bsSampleTypeLabel
        '
        Me.bsSampleTypeLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeLabel.Location = New System.Drawing.Point(377, 83)
        Me.bsSampleTypeLabel.Name = "bsSampleTypeLabel"
        Me.bsSampleTypeLabel.Size = New System.Drawing.Size(186, 13)
        Me.bsSampleTypeLabel.TabIndex = 40
        Me.bsSampleTypeLabel.Text = "Sample Type:"
        Me.bsSampleTypeLabel.Title = False
        '
        'bsTestLabel
        '
        Me.bsTestLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsTestLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTestLabel.Location = New System.Drawing.Point(70, 83)
        Me.bsTestLabel.Name = "bsTestLabel"
        Me.bsTestLabel.Size = New System.Drawing.Size(241, 13)
        Me.bsTestLabel.TabIndex = 35
        Me.bsTestLabel.Text = "*Test:"
        Me.bsTestLabel.Title = False
        '
        'bsSampleLabel
        '
        Me.bsSampleLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsSampleLabel.Location = New System.Drawing.Point(12, 43)
        Me.bsSampleLabel.Name = "bsSampleLabel"
        Me.bsSampleLabel.Size = New System.Drawing.Size(49, 13)
        Me.bsSampleLabel.TabIndex = 33
        Me.bsSampleLabel.Text = "Class:"
        Me.bsSampleLabel.Title = False
        '
        'bsCloseButton
        '
        Me.bsCloseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCloseButton.Location = New System.Drawing.Point(966, 650)
        Me.bsCloseButton.Name = "bsCloseButton"
        Me.bsCloseButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCloseButton.TabIndex = 168
        Me.bsCloseButton.UseVisualStyleBackColor = True
        '
        'bsLastButton
        '
        Me.bsLastButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsLastButton.Location = New System.Drawing.Point(643, 650)
        Me.bsLastButton.Name = "bsLastButton"
        Me.bsLastButton.Size = New System.Drawing.Size(32, 32)
        Me.bsLastButton.TabIndex = 169
        Me.bsLastButton.UseVisualStyleBackColor = True
        '
        'bsNextButton
        '
        Me.bsNextButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsNextButton.Location = New System.Drawing.Point(606, 650)
        Me.bsNextButton.Name = "bsNextButton"
        Me.bsNextButton.Size = New System.Drawing.Size(32, 32)
        Me.bsNextButton.TabIndex = 170
        Me.bsNextButton.UseVisualStyleBackColor = True
        '
        'bsPreviousButton
        '
        Me.bsPreviousButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPreviousButton.Location = New System.Drawing.Point(569, 650)
        Me.bsPreviousButton.Name = "bsPreviousButton"
        Me.bsPreviousButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPreviousButton.TabIndex = 171
        Me.bsPreviousButton.UseVisualStyleBackColor = True
        '
        'bsFirstButton
        '
        Me.bsFirstButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsFirstButton.Location = New System.Drawing.Point(532, 650)
        Me.bsFirstButton.Name = "bsFirstButton"
        Me.bsFirstButton.Size = New System.Drawing.Size(32, 32)
        Me.bsFirstButton.TabIndex = 172
        Me.bsFirstButton.UseVisualStyleBackColor = True
        '
        'bsPrintButton
        '
        Me.bsPrintButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsPrintButton.Location = New System.Drawing.Point(495, 650)
        Me.bsPrintButton.Name = "bsPrintButton"
        Me.bsPrintButton.Size = New System.Drawing.Size(32, 32)
        Me.bsPrintButton.TabIndex = 174
        Me.bsPrintButton.UseVisualStyleBackColor = True
        Me.bsPrintButton.Visible = False
        '
        'bsExpandButton
        '
        Me.bsExpandButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExpandButton.Location = New System.Drawing.Point(680, 650)
        Me.bsExpandButton.Name = "bsExpandButton"
        Me.bsExpandButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExpandButton.TabIndex = 185
        Me.bsExpandButton.UseVisualStyleBackColor = True
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.Controls.Add(Me.bsRerunText)
        Me.BsGroupBox1.Controls.Add(Me.bsMultiItemText)
        Me.BsGroupBox1.Controls.Add(Me.bsSampleTypeText)
        Me.BsGroupBox1.Controls.Add(Me.bsLotText)
        Me.BsGroupBox1.Controls.Add(Me.bsTestText)
        Me.BsGroupBox1.Controls.Add(Me.bsComodinText)
        Me.BsGroupBox1.Controls.Add(Me.bsLotLabel)
        Me.BsGroupBox1.Controls.Add(Me.bsMultiItemLabel)
        Me.BsGroupBox1.Controls.Add(Me.bsClassPictureBox)
        Me.BsGroupBox1.Controls.Add(Me.bsGraphResultLabel)
        Me.BsGroupBox1.Controls.Add(Me.bsSampleLabel)
        Me.BsGroupBox1.Controls.Add(Me.bsTestLabel)
        Me.BsGroupBox1.Controls.Add(Me.bsRerunLabel)
        Me.BsGroupBox1.Controls.Add(Me.bsSampleTypeLabel)
        Me.BsGroupBox1.Controls.Add(Me.bsComodinLabel)
        Me.BsGroupBox1.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox1.Location = New System.Drawing.Point(10, 10)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New System.Drawing.Size(701, 127)
        Me.BsGroupBox1.TabIndex = 186
        Me.BsGroupBox1.TabStop = False
        '
        'bsRerunText
        '
        Me.bsRerunText.BackColor = System.Drawing.Color.Transparent
        Me.bsRerunText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsRerunText.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsRerunText.ForeColor = System.Drawing.Color.Black
        Me.bsRerunText.Location = New System.Drawing.Point(619, 99)
        Me.bsRerunText.Name = "bsRerunText"
        Me.bsRerunText.Size = New System.Drawing.Size(71, 20)
        Me.bsRerunText.TabIndex = 54
        Me.bsRerunText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsRerunText.Title = False
        '
        'bsMultiItemText
        '
        Me.bsMultiItemText.BackColor = System.Drawing.Color.Transparent
        Me.bsMultiItemText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsMultiItemText.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsMultiItemText.ForeColor = System.Drawing.Color.Black
        Me.bsMultiItemText.Location = New System.Drawing.Point(619, 59)
        Me.bsMultiItemText.Name = "bsMultiItemText"
        Me.bsMultiItemText.Size = New System.Drawing.Size(71, 20)
        Me.bsMultiItemText.TabIndex = 53
        Me.bsMultiItemText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsMultiItemText.Title = False
        '
        'bsSampleTypeText
        '
        Me.bsSampleTypeText.BackColor = System.Drawing.Color.Transparent
        Me.bsSampleTypeText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsSampleTypeText.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsSampleTypeText.ForeColor = System.Drawing.Color.Black
        Me.bsSampleTypeText.Location = New System.Drawing.Point(380, 99)
        Me.bsSampleTypeText.Name = "bsSampleTypeText"
        Me.bsSampleTypeText.Size = New System.Drawing.Size(229, 20)
        Me.bsSampleTypeText.TabIndex = 52
        Me.bsSampleTypeText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsSampleTypeText.Title = False
        '
        'bsLotText
        '
        Me.bsLotText.BackColor = System.Drawing.Color.Transparent
        Me.bsLotText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsLotText.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLotText.ForeColor = System.Drawing.Color.Black
        Me.bsLotText.Location = New System.Drawing.Point(380, 59)
        Me.bsLotText.Name = "bsLotText"
        Me.bsLotText.Size = New System.Drawing.Size(229, 20)
        Me.bsLotText.TabIndex = 51
        Me.bsLotText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsLotText.Title = False
        '
        'bsTestText
        '
        Me.bsTestText.BackColor = System.Drawing.Color.Transparent
        Me.bsTestText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsTestText.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsTestText.ForeColor = System.Drawing.Color.Black
        Me.bsTestText.Location = New System.Drawing.Point(70, 99)
        Me.bsTestText.Name = "bsTestText"
        Me.bsTestText.Size = New System.Drawing.Size(296, 20)
        Me.bsTestText.TabIndex = 50
        Me.bsTestText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsTestText.Title = False
        '
        'bsComodinText
        '
        Me.bsComodinText.BackColor = System.Drawing.Color.Transparent
        Me.bsComodinText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.bsComodinText.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsComodinText.ForeColor = System.Drawing.Color.Black
        Me.bsComodinText.Location = New System.Drawing.Point(70, 59)
        Me.bsComodinText.Name = "bsComodinText"
        Me.bsComodinText.Size = New System.Drawing.Size(296, 20)
        Me.bsComodinText.TabIndex = 49
        Me.bsComodinText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsComodinText.Title = False
        '
        'bsLotLabel
        '
        Me.bsLotLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsLotLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsLotLabel.ForeColor = System.Drawing.Color.Black
        Me.bsLotLabel.Location = New System.Drawing.Point(377, 43)
        Me.bsLotLabel.Name = "bsLotLabel"
        Me.bsLotLabel.Size = New System.Drawing.Size(186, 13)
        Me.bsLotLabel.TabIndex = 48
        Me.bsLotLabel.Text = "*Lot:"
        Me.bsLotLabel.Title = False
        '
        'bsClassPictureBox
        '
        Me.bsClassPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.bsClassPictureBox.InitialImage = Nothing
        Me.bsClassPictureBox.Location = New System.Drawing.Point(22, 59)
        Me.bsClassPictureBox.Name = "bsClassPictureBox"
        Me.bsClassPictureBox.PositionNumber = 0
        Me.bsClassPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.bsClassPictureBox.TabIndex = 43
        Me.bsClassPictureBox.TabStop = False
        '
        'bsGraphResultLabel
        '
        Me.bsGraphResultLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsGraphResultLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsGraphResultLabel.ForeColor = System.Drawing.Color.Black
        Me.bsGraphResultLabel.Location = New System.Drawing.Point(5, 15)
        Me.bsGraphResultLabel.Name = "bsGraphResultLabel"
        Me.bsGraphResultLabel.Size = New System.Drawing.Size(690, 20)
        Me.bsGraphResultLabel.TabIndex = 24
        Me.bsGraphResultLabel.Text = "Graphical Results Absorbance / time"
        Me.bsGraphResultLabel.Title = True
        '
        'BsGroupBox2
        '
        Me.BsGroupBox2.Controls.Add(Me.ReplicateUpDown)
        Me.BsGroupBox2.Controls.Add(Me.BsShowLabel)
        Me.BsGroupBox2.Controls.Add(Me.FilterComboBox)
        Me.BsGroupBox2.Controls.Add(Me.AllReplicatesRadioButton)
        Me.BsGroupBox2.Controls.Add(Me.ReplicateRadioButton)
        Me.BsGroupBox2.Controls.Add(Me.bsGraphTypeLabel)
        Me.BsGroupBox2.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox2.Location = New System.Drawing.Point(717, 12)
        Me.BsGroupBox2.Name = "BsGroupBox2"
        Me.BsGroupBox2.Size = New System.Drawing.Size(281, 125)
        Me.BsGroupBox2.TabIndex = 188
        Me.BsGroupBox2.TabStop = False
        '
        'ReplicateUpDown
        '
        Me.ReplicateUpDown.ForeColor = System.Drawing.Color.Black
        Me.ReplicateUpDown.Location = New System.Drawing.Point(36, 66)
        Me.ReplicateUpDown.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.ReplicateUpDown.Name = "ReplicateUpDown"
        Me.ReplicateUpDown.Size = New System.Drawing.Size(50, 21)
        Me.ReplicateUpDown.TabIndex = 78
        Me.ReplicateUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ReplicateUpDown.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'AllReplicatesRadioButton
        '
        Me.AllReplicatesRadioButton.Location = New System.Drawing.Point(14, 97)
        Me.AllReplicatesRadioButton.Name = "AllReplicatesRadioButton"
        Me.AllReplicatesRadioButton.Size = New System.Drawing.Size(173, 17)
        Me.AllReplicatesRadioButton.TabIndex = 26
        Me.AllReplicatesRadioButton.Text = "*All Replicates"
        Me.AllReplicatesRadioButton.UseVisualStyleBackColor = True
        '
        'ReplicateRadioButton
        '
        Me.ReplicateRadioButton.Location = New System.Drawing.Point(14, 45)
        Me.ReplicateRadioButton.Name = "ReplicateRadioButton"
        Me.ReplicateRadioButton.Size = New System.Drawing.Size(123, 17)
        Me.ReplicateRadioButton.TabIndex = 25
        Me.ReplicateRadioButton.Text = "Replicate:"
        Me.ReplicateRadioButton.UseVisualStyleBackColor = True
        '
        'bsGraphTypeLabel
        '
        Me.bsGraphTypeLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsGraphTypeLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsGraphTypeLabel.ForeColor = System.Drawing.Color.Black
        Me.bsGraphTypeLabel.Location = New System.Drawing.Point(5, 15)
        Me.bsGraphTypeLabel.Name = "bsGraphTypeLabel"
        Me.bsGraphTypeLabel.Size = New System.Drawing.Size(270, 20)
        Me.bsGraphTypeLabel.TabIndex = 24
        Me.bsGraphTypeLabel.Text = "Graph Type"
        Me.bsGraphTypeLabel.Title = True
        '
        'BsGroupBox3
        '
        Me.BsGroupBox3.Controls.Add(Me.ReplicatesGridControl)
        Me.BsGroupBox3.Controls.Add(Me.ResultChartControl)
        Me.BsGroupBox3.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox3.Location = New System.Drawing.Point(10, 138)
        Me.BsGroupBox3.Name = "BsGroupBox3"
        Me.BsGroupBox3.Size = New System.Drawing.Size(988, 506)
        Me.BsGroupBox3.TabIndex = 189
        Me.BsGroupBox3.TabStop = False
        '
        'ReplicatesGridControl
        '
        Me.ReplicatesGridControl.Location = New System.Drawing.Point(707, 15)
        Me.ReplicatesGridControl.LookAndFeel.UseWindowsXPTheme = True
        Me.ReplicatesGridControl.MainView = Me.ReplicatesGridView
        Me.ReplicatesGridControl.Name = "ReplicatesGridControl"
        Me.ReplicatesGridControl.Size = New System.Drawing.Size(275, 483)
        Me.ReplicatesGridControl.TabIndex = 192
        Me.ReplicatesGridControl.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.ReplicatesGridView})
        '
        'ReplicatesGridView
        '
        Me.ReplicatesGridView.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.ReplicatesGridView.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.ReplicatesGridView.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.ReplicatesGridView.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.ReplicatesGridView.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.ReplicatesGridView.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.ReplicatesGridView.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.ReplicatesGridView.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.ReplicatesGridView.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.ReplicatesGridView.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.ReplicatesGridView.Appearance.Empty.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.ReplicatesGridView.Appearance.EvenRow.ForeColor = System.Drawing.Color.Black
        Me.ReplicatesGridView.Appearance.EvenRow.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.EvenRow.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.ReplicatesGridView.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.ReplicatesGridView.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.ReplicatesGridView.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.ReplicatesGridView.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.ReplicatesGridView.Appearance.FilterPanel.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.FilterPanel.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.FocusedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.ReplicatesGridView.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.ReplicatesGridView.Appearance.FocusedRow.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.FocusedRow.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.ReplicatesGridView.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.ReplicatesGridView.Appearance.FooterPanel.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.ReplicatesGridView.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.ReplicatesGridView.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.ReplicatesGridView.Appearance.GroupButton.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.GroupButton.Options.UseBorderColor = True
        Me.ReplicatesGridView.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.ReplicatesGridView.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.ReplicatesGridView.Appearance.GroupFooter.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.ReplicatesGridView.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.ReplicatesGridView.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.ReplicatesGridView.Appearance.GroupPanel.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.GroupPanel.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.GroupRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ReplicatesGridView.Appearance.GroupRow.BorderColor = System.Drawing.Color.Gainsboro
        Me.ReplicatesGridView.Appearance.GroupRow.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.ReplicatesGridView.Appearance.GroupRow.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.GroupRow.Options.UseBorderColor = True
        Me.ReplicatesGridView.Appearance.GroupRow.Options.UseFont = True
        Me.ReplicatesGridView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.ReplicatesGridView.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.ReplicatesGridView.Appearance.HeaderPanel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ReplicatesGridView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black
        Me.ReplicatesGridView.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.ReplicatesGridView.Appearance.HeaderPanel.Options.UseFont = True
        Me.ReplicatesGridView.Appearance.HeaderPanel.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.ReplicatesGridView.Appearance.HorzLine.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.OddRow.BackColor = System.Drawing.Color.White
        Me.ReplicatesGridView.Appearance.OddRow.ForeColor = System.Drawing.Color.Black
        Me.ReplicatesGridView.Appearance.OddRow.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.OddRow.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.ReplicatesGridView.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.ReplicatesGridView.Appearance.Preview.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.Preview.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.ReplicatesGridView.Appearance.Row.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ReplicatesGridView.Appearance.Row.ForeColor = System.Drawing.Color.Black
        Me.ReplicatesGridView.Appearance.Row.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.Row.Options.UseFont = True
        Me.ReplicatesGridView.Appearance.Row.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.ReplicatesGridView.Appearance.RowSeparator.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.SelectedRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.ReplicatesGridView.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White
        Me.ReplicatesGridView.Appearance.SelectedRow.Options.UseBackColor = True
        Me.ReplicatesGridView.Appearance.SelectedRow.Options.UseForeColor = True
        Me.ReplicatesGridView.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.ReplicatesGridView.Appearance.VertLine.Options.UseBackColor = True
        Me.ReplicatesGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.ReplicatesGridView.GridControl = Me.ReplicatesGridControl
        Me.ReplicatesGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never
        Me.ReplicatesGridView.Name = "ReplicatesGridView"
        Me.ReplicatesGridView.OptionsCustomization.AllowColumnMoving = False
        Me.ReplicatesGridView.OptionsCustomization.AllowFilter = False
        Me.ReplicatesGridView.OptionsCustomization.AllowQuickHideColumns = False
        Me.ReplicatesGridView.OptionsFilter.AllowFilterEditor = False
        Me.ReplicatesGridView.OptionsFind.AllowFindPanel = False
        Me.ReplicatesGridView.OptionsMenu.EnableColumnMenu = False
        Me.ReplicatesGridView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.ReplicatesGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never
        Me.ReplicatesGridView.OptionsView.ShowGroupPanel = False
        Me.ReplicatesGridView.PaintStyleName = "WindowsXP"
        '
        'ResultChartControl
        '
        Me.ResultChartControl.AppearanceName = "Light"
        Me.ResultChartControl.FillStyle.FillMode = DevExpress.XtraCharts.FillMode.Empty
        Me.ResultChartControl.Location = New System.Drawing.Point(8, 15)
        Me.ResultChartControl.Name = "ResultChartControl"
        Me.ResultChartControl.PaletteName = "Nature Colors"
        Me.ResultChartControl.SeriesSerializable = New DevExpress.XtraCharts.Series(-1) {}
        SideBySideBarSeriesLabel1.LineVisibility = DefaultBoolean.True
        Me.ResultChartControl.SeriesTemplate.Label = SideBySideBarSeriesLabel1
        Me.ResultChartControl.Size = New System.Drawing.Size(693, 483)
        Me.ResultChartControl.TabIndex = 191
        '
        'bsCollapseButton
        '
        Me.bsCollapseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCollapseButton.Location = New System.Drawing.Point(929, 650)
        Me.bsCollapseButton.Name = "bsCollapseButton"
        Me.bsCollapseButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCollapseButton.TabIndex = 190
        Me.bsCollapseButton.UseVisualStyleBackColor = True
        Me.bsCollapseButton.Visible = False
        '
        'BsButton3
        '
        Me.BsButton3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton3.Location = New System.Drawing.Point(366, 650)
        Me.BsButton3.Name = "BsButton3"
        Me.BsButton3.Size = New System.Drawing.Size(66, 42)
        Me.BsButton3.TabIndex = 193
        Me.BsButton3.Text = "clear"
        Me.BsButton3.UseVisualStyleBackColor = True
        Me.BsButton3.Visible = False
        '
        'BsButton2
        '
        Me.BsButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton2.Location = New System.Drawing.Point(300, 650)
        Me.BsButton2.Name = "BsButton2"
        Me.BsButton2.Size = New System.Drawing.Size(66, 42)
        Me.BsButton2.TabIndex = 192
        Me.BsButton2.Text = "process"
        Me.BsButton2.UseVisualStyleBackColor = True
        Me.BsButton2.Visible = False
        '
        'TextBox5
        '
        Me.TextBox5.Font = New System.Drawing.Font("Courier New", 6.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox5.Location = New System.Drawing.Point(11, 650)
        Me.TextBox5.Multiline = True
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(285, 40)
        Me.TextBox5.TabIndex = 191
        Me.TextBox5.Visible = False
        '
        'IResultsAbsCurve
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(1010, 694)
        Me.ControlBox = False
        Me.Controls.Add(Me.BsButton3)
        Me.Controls.Add(Me.BsButton2)
        Me.Controls.Add(Me.TextBox5)
        Me.Controls.Add(Me.bsWell)
        Me.Controls.Add(Me.BsWellLabel)
        Me.Controls.Add(Me.bsCollapseButton)
        Me.Controls.Add(Me.bsReplicate)
        Me.Controls.Add(Me.BsReplicateLabel)
        Me.Controls.Add(Me.BsGroupBox3)
        Me.Controls.Add(Me.BsGroupBox2)
        Me.Controls.Add(Me.BsGroupBox1)
        Me.Controls.Add(Me.bsExpandButton)
        Me.Controls.Add(Me.bsPrintButton)
        Me.Controls.Add(Me.bsFirstButton)
        Me.Controls.Add(Me.bsPreviousButton)
        Me.Controls.Add(Me.bsNextButton)
        Me.Controls.Add(Me.bsLastButton)
        Me.Controls.Add(Me.bsCloseButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiResultsAbsCurve"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        CType(Me.bsErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsWell.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsReplicate.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsGroupBox1.ResumeLayout(False)
        CType(Me.bsClassPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsGroupBox2.ResumeLayout(False)
        CType(Me.ReplicateUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsGroupBox3.ResumeLayout(False)
        CType(Me.ReplicatesGridControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ReplicatesGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(SideBySideBarSeriesLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ResultChartControl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents bsErrorProvider1 As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents bsGraphToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents bsWell As DevExpress.XtraEditors.TextEdit
    Friend WithEvents bsMultiItemLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsWellLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsReplicate As DevExpress.XtraEditors.TextEdit
    Friend WithEvents BsReplicateLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRerunLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsComodinLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSampleTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTestLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSampleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTimer1 As Biosystems.Ax00.Controls.UserControls.BSTimer
    Friend WithEvents ToolTipController1 As DevExpress.Utils.ToolTipController
    Friend WithEvents BsShowLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsCloseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsLastButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsNextButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsPreviousButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsFirstButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsPrintButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents FilterComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents bsExpandButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsGroupBox1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsGraphResultLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsGroupBox2 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsGraphTypeLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsGroupBox3 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents AllReplicatesRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents ReplicateRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents ReplicateUpDown As Biosystems.Ax00.Controls.UserControls.BSNumericUpDown
    Friend WithEvents bsClassPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsLotLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsCollapseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ResultChartControl As DevExpress.XtraCharts.ChartControl
    Friend WithEvents BsButton3 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsButton2 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents TextBox5 As System.Windows.Forms.TextBox
    Friend WithEvents bsComodinText As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsLotText As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsTestText As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsRerunText As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsMultiItemText As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsSampleTypeText As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents ReplicatesGridControl As DevExpress.XtraGrid.GridControl
    Friend WithEvents ReplicatesGridView As DevExpress.XtraGrid.Views.Grid.GridView
End Class
