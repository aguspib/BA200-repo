Imports DevExpress.Utils

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IISEResultsHistoryGraph
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
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
        Me.components = New System.ComponentModel.Container()
        Dim SideBySideBarSeriesLabel1 As DevExpress.XtraCharts.SideBySideBarSeriesLabel = New DevExpress.XtraCharts.SideBySideBarSeriesLabel()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IISEResultsHistoryGraph))
        Me.bsGraphicalResultsGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsElectrodesTableLayout = New Biosystems.Ax00.Controls.UserControls.BSTableLayoutPanel()
        Me.bsElectrodeNaCheck = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsElectrodeKCheck = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsElectrodeClCheck = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsElectrodeLiCheck = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.bsISEResultChartControl = New DevExpress.XtraCharts.ChartControl()
        Me.bsLegend = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsLegendTableLayout = New Biosystems.Ax00.Controls.UserControls.BSTableLayoutPanel()
        Me.bsElectrodeLiLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsElectrodeNaImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsElectrodeLiImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsElectrodeNaLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsElectrodeClLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsElectrodeKImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsElectrodeClImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.bsElectrodeKLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsToolTipController = New DevExpress.Utils.ToolTipController(Me.components)
        Me.bsGraphicalResultsGroupBox.SuspendLayout()
        Me.bsElectrodesTableLayout.SuspendLayout()
        CType(Me.bsISEResultChartControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(SideBySideBarSeriesLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.bsLegend.SuspendLayout()
        Me.bsLegendTableLayout.SuspendLayout()
        CType(Me.bsElectrodeNaImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsElectrodeLiImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsElectrodeKImage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.bsElectrodeClImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bsGraphicalResultsGroupBox
        '
        Me.bsGraphicalResultsGroupBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsGraphicalResultsGroupBox.Controls.Add(Me.bsElectrodesTableLayout)
        Me.bsGraphicalResultsGroupBox.Controls.Add(Me.bsISEResultChartControl)
        Me.bsGraphicalResultsGroupBox.Controls.Add(Me.bsLegend)
        Me.bsGraphicalResultsGroupBox.Controls.Add(Me.bsTitleLabel)
        Me.bsGraphicalResultsGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsGraphicalResultsGroupBox.Location = New System.Drawing.Point(2, 3)
        Me.bsGraphicalResultsGroupBox.Name = "bsGraphicalResultsGroupBox"
        Me.bsGraphicalResultsGroupBox.Size = New System.Drawing.Size(693, 484)
        Me.bsGraphicalResultsGroupBox.TabIndex = 1
        Me.bsGraphicalResultsGroupBox.TabStop = False
        '
        'bsElectrodesTableLayout
        '
        Me.bsElectrodesTableLayout.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.bsElectrodesTableLayout.ColumnCount = 4
        Me.bsElectrodesTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.bsElectrodesTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.bsElectrodesTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.bsElectrodesTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.bsElectrodesTableLayout.Controls.Add(Me.bsElectrodeNaCheck, 0, 0)
        Me.bsElectrodesTableLayout.Controls.Add(Me.bsElectrodeKCheck, 1, 0)
        Me.bsElectrodesTableLayout.Controls.Add(Me.bsElectrodeClCheck, 2, 0)
        Me.bsElectrodesTableLayout.Controls.Add(Me.bsElectrodeLiCheck, 3, 0)
        Me.bsElectrodesTableLayout.Location = New System.Drawing.Point(148, 49)
        Me.bsElectrodesTableLayout.Name = "bsElectrodesTableLayout"
        Me.bsElectrodesTableLayout.RowCount = 1
        Me.bsElectrodesTableLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.bsElectrodesTableLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.bsElectrodesTableLayout.Size = New System.Drawing.Size(397, 25)
        Me.bsElectrodesTableLayout.TabIndex = 6
        '
        'bsElectrodeNaCheck
        '
        Me.bsElectrodeNaCheck.AutoSize = True
        Me.bsElectrodeNaCheck.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsElectrodeNaCheck.Location = New System.Drawing.Point(3, 3)
        Me.bsElectrodeNaCheck.Name = "bsElectrodeNaCheck"
        Me.bsElectrodeNaCheck.Size = New System.Drawing.Size(93, 19)
        Me.bsElectrodeNaCheck.TabIndex = 0
        Me.bsElectrodeNaCheck.Text = "*Na+"
        Me.bsElectrodeNaCheck.UseVisualStyleBackColor = True
        '
        'bsElectrodeKCheck
        '
        Me.bsElectrodeKCheck.AutoSize = True
        Me.bsElectrodeKCheck.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsElectrodeKCheck.Location = New System.Drawing.Point(102, 3)
        Me.bsElectrodeKCheck.Name = "bsElectrodeKCheck"
        Me.bsElectrodeKCheck.Size = New System.Drawing.Size(93, 19)
        Me.bsElectrodeKCheck.TabIndex = 1
        Me.bsElectrodeKCheck.Text = "*K+"
        Me.bsElectrodeKCheck.UseVisualStyleBackColor = True
        '
        'bsElectrodeClCheck
        '
        Me.bsElectrodeClCheck.AutoSize = True
        Me.bsElectrodeClCheck.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsElectrodeClCheck.Location = New System.Drawing.Point(201, 3)
        Me.bsElectrodeClCheck.Name = "bsElectrodeClCheck"
        Me.bsElectrodeClCheck.Size = New System.Drawing.Size(93, 19)
        Me.bsElectrodeClCheck.TabIndex = 2
        Me.bsElectrodeClCheck.Text = "*Cl-"
        Me.bsElectrodeClCheck.UseVisualStyleBackColor = True
        '
        'bsElectrodeLiCheck
        '
        Me.bsElectrodeLiCheck.AutoSize = True
        Me.bsElectrodeLiCheck.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsElectrodeLiCheck.Location = New System.Drawing.Point(300, 3)
        Me.bsElectrodeLiCheck.Name = "bsElectrodeLiCheck"
        Me.bsElectrodeLiCheck.Size = New System.Drawing.Size(94, 19)
        Me.bsElectrodeLiCheck.TabIndex = 3
        Me.bsElectrodeLiCheck.Text = "*Li+"
        Me.bsElectrodeLiCheck.UseVisualStyleBackColor = True
        '
        'bsISEResultChartControl
        '
        Me.bsISEResultChartControl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsISEResultChartControl.AppearanceName = "Light"
        Me.bsISEResultChartControl.Location = New System.Drawing.Point(13, 90)
        Me.bsISEResultChartControl.Name = "bsISEResultChartControl"
        Me.bsISEResultChartControl.PaletteName = "Nature Colors"
        Me.bsISEResultChartControl.SeriesSerializable = New DevExpress.XtraCharts.Series(-1) {}
        SideBySideBarSeriesLabel1.LineVisibility = DefaultBoolean.True
        Me.bsISEResultChartControl.SeriesTemplate.Label = SideBySideBarSeriesLabel1
        Me.bsISEResultChartControl.Size = New System.Drawing.Size(669, 331)
        Me.bsISEResultChartControl.TabIndex = 5
        '
        'bsLegend
        '
        Me.bsLegend.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.bsLegend.Controls.Add(Me.bsLegendTableLayout)
        Me.bsLegend.ForeColor = System.Drawing.Color.Black
        Me.bsLegend.Location = New System.Drawing.Point(13, 427)
        Me.bsLegend.Name = "bsLegend"
        Me.bsLegend.Size = New System.Drawing.Size(669, 51)
        Me.bsLegend.TabIndex = 4
        Me.bsLegend.TabStop = False
        Me.bsLegend.Text = "*Legend"
        '
        'bsLegendTableLayout
        '
        Me.bsLegendTableLayout.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsLegendTableLayout.ColumnCount = 11
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.bsLegendTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.bsLegendTableLayout.Controls.Add(Me.bsElectrodeLiLabel, 10, 0)
        Me.bsLegendTableLayout.Controls.Add(Me.bsElectrodeNaImage, 0, 0)
        Me.bsLegendTableLayout.Controls.Add(Me.bsElectrodeLiImage, 9, 0)
        Me.bsLegendTableLayout.Controls.Add(Me.bsElectrodeNaLabel, 1, 0)
        Me.bsLegendTableLayout.Controls.Add(Me.bsElectrodeClLabel, 7, 0)
        Me.bsLegendTableLayout.Controls.Add(Me.bsElectrodeKImage, 3, 0)
        Me.bsLegendTableLayout.Controls.Add(Me.bsElectrodeClImage, 6, 0)
        Me.bsLegendTableLayout.Controls.Add(Me.bsElectrodeKLabel, 4, 0)
        Me.bsLegendTableLayout.Location = New System.Drawing.Point(44, 20)
        Me.bsLegendTableLayout.Name = "bsLegendTableLayout"
        Me.bsLegendTableLayout.RowCount = 1
        Me.bsLegendTableLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.bsLegendTableLayout.Size = New System.Drawing.Size(582, 23)
        Me.bsLegendTableLayout.TabIndex = 8
        '
        'bsElectrodeLiLabel
        '
        Me.bsElectrodeLiLabel.AutoSize = True
        Me.bsElectrodeLiLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsElectrodeLiLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsElectrodeLiLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsElectrodeLiLabel.ForeColor = System.Drawing.Color.Black
        Me.bsElectrodeLiLabel.Location = New System.Drawing.Point(475, 0)
        Me.bsElectrodeLiLabel.Name = "bsElectrodeLiLabel"
        Me.bsElectrodeLiLabel.Size = New System.Drawing.Size(104, 23)
        Me.bsElectrodeLiLabel.TabIndex = 7
        Me.bsElectrodeLiLabel.Text = "*Li+"
        Me.bsElectrodeLiLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsElectrodeLiLabel.Title = False
        '
        'bsElectrodeNaImage
        '
        Me.bsElectrodeNaImage.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.bsElectrodeNaImage.Image = CType(resources.GetObject("bsElectrodeNaImage.Image"), System.Drawing.Image)
        Me.bsElectrodeNaImage.Location = New System.Drawing.Point(3, 3)
        Me.bsElectrodeNaImage.Name = "bsElectrodeNaImage"
        Me.bsElectrodeNaImage.PositionNumber = 0
        Me.bsElectrodeNaImage.Size = New System.Drawing.Size(16, 16)
        Me.bsElectrodeNaImage.TabIndex = 0
        Me.bsElectrodeNaImage.TabStop = False
        '
        'bsElectrodeLiImage
        '
        Me.bsElectrodeLiImage.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.bsElectrodeLiImage.Image = CType(resources.GetObject("bsElectrodeLiImage.Image"), System.Drawing.Image)
        Me.bsElectrodeLiImage.Location = New System.Drawing.Point(453, 3)
        Me.bsElectrodeLiImage.Name = "bsElectrodeLiImage"
        Me.bsElectrodeLiImage.PositionNumber = 0
        Me.bsElectrodeLiImage.Size = New System.Drawing.Size(16, 16)
        Me.bsElectrodeLiImage.TabIndex = 6
        Me.bsElectrodeLiImage.TabStop = False
        '
        'bsElectrodeNaLabel
        '
        Me.bsElectrodeNaLabel.AutoSize = True
        Me.bsElectrodeNaLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsElectrodeNaLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsElectrodeNaLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsElectrodeNaLabel.ForeColor = System.Drawing.Color.Black
        Me.bsElectrodeNaLabel.Location = New System.Drawing.Point(25, 0)
        Me.bsElectrodeNaLabel.Name = "bsElectrodeNaLabel"
        Me.bsElectrodeNaLabel.Size = New System.Drawing.Size(102, 23)
        Me.bsElectrodeNaLabel.TabIndex = 1
        Me.bsElectrodeNaLabel.Text = "*Na+"
        Me.bsElectrodeNaLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsElectrodeNaLabel.Title = False
        '
        'bsElectrodeClLabel
        '
        Me.bsElectrodeClLabel.AutoSize = True
        Me.bsElectrodeClLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsElectrodeClLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsElectrodeClLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsElectrodeClLabel.ForeColor = System.Drawing.Color.Black
        Me.bsElectrodeClLabel.Location = New System.Drawing.Point(325, 0)
        Me.bsElectrodeClLabel.Name = "bsElectrodeClLabel"
        Me.bsElectrodeClLabel.Size = New System.Drawing.Size(102, 23)
        Me.bsElectrodeClLabel.TabIndex = 5
        Me.bsElectrodeClLabel.Text = "*Cl-"
        Me.bsElectrodeClLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsElectrodeClLabel.Title = False
        '
        'bsElectrodeKImage
        '
        Me.bsElectrodeKImage.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.bsElectrodeKImage.Image = CType(resources.GetObject("bsElectrodeKImage.Image"), System.Drawing.Image)
        Me.bsElectrodeKImage.Location = New System.Drawing.Point(153, 3)
        Me.bsElectrodeKImage.Name = "bsElectrodeKImage"
        Me.bsElectrodeKImage.PositionNumber = 0
        Me.bsElectrodeKImage.Size = New System.Drawing.Size(16, 16)
        Me.bsElectrodeKImage.TabIndex = 2
        Me.bsElectrodeKImage.TabStop = False
        '
        'bsElectrodeClImage
        '
        Me.bsElectrodeClImage.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.bsElectrodeClImage.Image = CType(resources.GetObject("bsElectrodeClImage.Image"), System.Drawing.Image)
        Me.bsElectrodeClImage.Location = New System.Drawing.Point(303, 3)
        Me.bsElectrodeClImage.Name = "bsElectrodeClImage"
        Me.bsElectrodeClImage.PositionNumber = 0
        Me.bsElectrodeClImage.Size = New System.Drawing.Size(16, 16)
        Me.bsElectrodeClImage.TabIndex = 4
        Me.bsElectrodeClImage.TabStop = False
        '
        'bsElectrodeKLabel
        '
        Me.bsElectrodeKLabel.AutoSize = True
        Me.bsElectrodeKLabel.BackColor = System.Drawing.Color.Transparent
        Me.bsElectrodeKLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.bsElectrodeKLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.bsElectrodeKLabel.ForeColor = System.Drawing.Color.Black
        Me.bsElectrodeKLabel.Location = New System.Drawing.Point(175, 0)
        Me.bsElectrodeKLabel.Name = "bsElectrodeKLabel"
        Me.bsElectrodeKLabel.Size = New System.Drawing.Size(102, 23)
        Me.bsElectrodeKLabel.TabIndex = 3
        Me.bsElectrodeKLabel.Text = "*K+"
        Me.bsElectrodeKLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.bsElectrodeKLabel.Title = False
        '
        'bsTitleLabel
        '
        Me.bsTitleLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsTitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsTitleLabel.Name = "bsTitleLabel"
        Me.bsTitleLabel.Size = New System.Drawing.Size(672, 20)
        Me.bsTitleLabel.TabIndex = 0
        Me.bsTitleLabel.Text = "*ISE Graphic trend"
        Me.bsTitleLabel.Title = True
        '
        'bsExitButton
        '
        Me.bsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.Location = New System.Drawing.Point(663, 493)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 0
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsToolTipController
        '
        Me.bsToolTipController.ToolTipLocation = DevExpress.Utils.ToolTipLocation.Fixed
        '
        'IISEResultsHistoryGraph
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(707, 530)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsGraphicalResultsGroupBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.Name = "IISEResultsHistoryGraph"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = " "
        Me.bsGraphicalResultsGroupBox.ResumeLayout(False)
        Me.bsElectrodesTableLayout.ResumeLayout(False)
        Me.bsElectrodesTableLayout.PerformLayout()
        CType(SideBySideBarSeriesLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsISEResultChartControl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.bsLegend.ResumeLayout(False)
        Me.bsLegendTableLayout.ResumeLayout(False)
        Me.bsLegendTableLayout.PerformLayout()
        CType(Me.bsElectrodeNaImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsElectrodeLiImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsElectrodeKImage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.bsElectrodeClImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsGraphicalResultsGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsElectrodeLiCheck As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsElectrodeClCheck As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsElectrodeKCheck As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsElectrodeNaCheck As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents bsLegend As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsElectrodeLiLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsElectrodeLiImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsElectrodeClLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsElectrodeClImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsElectrodeKLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsElectrodeKImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsElectrodeNaLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsElectrodeNaImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents bsToolTipController As DevExpress.Utils.ToolTipController
    Friend WithEvents bsISEResultChartControl As DevExpress.XtraCharts.ChartControl
    Friend WithEvents bsLegendTableLayout As Biosystems.Ax00.Controls.UserControls.BSTableLayoutPanel
    Friend WithEvents bsElectrodesTableLayout As Biosystems.Ax00.Controls.UserControls.BSTableLayoutPanel
End Class
