
Imports Biosystems.Ax00.Controls.UserControls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ITankLevelsAdjustments
    Inherits PesentationLayer.BSAdjustmentBaseForm

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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ITankLevelsAdjustments))
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.TestProcessTimer = New System.Windows.Forms.Timer(Me.components)
        Me.BsResponse = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.BsPanel1 = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsLabel4 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsLabel5 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.TestSimulatorTimer = New System.Windows.Forms.Timer(Me.components)
        Me.BsTabPagesControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl
        Me.BsScalesTabPage = New System.Windows.Forms.TabPage
        Me.BsTanksInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoScalesXPSViewer = New BsXPSViewer
        Me.BsInfoExpandButton = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsTanksInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsTanksAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.HCWGroupBox = New System.Windows.Forms.GroupBox
        Me.HCTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel
        Me.HCEmptyNewValueLabel = New System.Windows.Forms.Label
        Me.HCEmptySavedLabel = New System.Windows.Forms.Label
        Me.HCFullAdjustButton = New System.Windows.Forms.Button
        Me.HCFullSavedLabel = New System.Windows.Forms.Label
        Me.HCEmptyAdjustButton = New System.Windows.Forms.Button
        Me.HCEmptySavedPictureBox = New System.Windows.Forms.PictureBox
        Me.HCFullNewValueLabel = New System.Windows.Forms.Label
        Me.HCFullSavedPictureBox = New System.Windows.Forms.PictureBox
        Me.HCSavedLabel = New System.Windows.Forms.Label
        Me.HCMonitorTank = New Biosystems.Ax00.Controls.UserControls.BSMonitorTankA
        Me.HCCountsLabel = New System.Windows.Forms.Label
        Me.HCCurrentLabel = New System.Windows.Forms.Label
        Me.HCCurrentPercentLabel = New System.Windows.Forms.Label
        Me.HCPercentLabel = New System.Windows.Forms.Label
        Me.WashingSolutionGroupBox = New System.Windows.Forms.GroupBox
        Me.WSMonitorTank = New Biosystems.Ax00.Controls.UserControls.BSMonitorTankA
        Me.WSCountsLabel = New System.Windows.Forms.Label
        Me.WSCurrentLabel = New System.Windows.Forms.Label
        Me.WSTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel
        Me.WSEmptyNewValueLabel = New System.Windows.Forms.Label
        Me.WSFullSavedLabel = New System.Windows.Forms.Label
        Me.WSFullNewValueLabel = New System.Windows.Forms.Label
        Me.WSSavedLabel = New System.Windows.Forms.Label
        Me.WSEmptySavedPictureBox = New System.Windows.Forms.PictureBox
        Me.WSEmptyAdjustButton = New System.Windows.Forms.Button
        Me.WSFullSavedPictureBox = New System.Windows.Forms.PictureBox
        Me.WSEmptySavedLabel = New System.Windows.Forms.Label
        Me.WSFullAdjustButton = New System.Windows.Forms.Button
        Me.WSCurrentPercentLabel = New System.Windows.Forms.Label
        Me.WSPercentLabel = New System.Windows.Forms.Label
        Me.BsTanksTestButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsTanksAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsIntermediateTabPage = New System.Windows.Forms.TabPage
        Me.BsIntermediateInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsIntermediateInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsIntermediateInfoLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsIntermediateAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar
        Me.TestProgressBar = New System.Windows.Forms.ProgressBar
        Me.IntermediateTanksTestGroupBox = New System.Windows.Forms.GroupBox
        Me.BsTestLabel = New System.Windows.Forms.Label
        Me.BsStartTestButton = New System.Windows.Forms.Button
        Me.ProcessDataGridView = New System.Windows.Forms.DataGridView
        Me.NumberColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.IconColumn = New System.Windows.Forms.DataGridViewImageColumn
        Me.StepColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.BsStopTestButton = New System.Windows.Forms.Button
        Me.DWInputGroupBox = New System.Windows.Forms.GroupBox
        Me.DWTankInputLabel = New System.Windows.Forms.Label
        Me.BsInternalTanksAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.LCGroupBox = New System.Windows.Forms.GroupBox
        Me.LCPanel = New System.Windows.Forms.Panel
        Me.LCMonitorTank = New Biosystems.Ax00.Controls.UserControls.BSMonitorTankLevels
        Me.DWGroupBox = New System.Windows.Forms.GroupBox
        Me.DWPanel = New System.Windows.Forms.Panel
        Me.DWMonitorTank = New Biosystems.Ax00.Controls.UserControls.BSMonitorTankLevels
        Me.BsIntermediateAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.BsTabPagesControl.SuspendLayout()
        Me.BsScalesTabPage.SuspendLayout()
        Me.BsTanksInfoPanel.SuspendLayout()
        Me.BsTanksAdjustPanel.SuspendLayout()
        Me.HCWGroupBox.SuspendLayout()
        Me.HCTableLayoutPanel.SuspendLayout()
        CType(Me.HCEmptySavedPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.HCFullSavedPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.WashingSolutionGroupBox.SuspendLayout()
        Me.WSTableLayoutPanel.SuspendLayout()
        CType(Me.WSEmptySavedPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.WSFullSavedPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.BsIntermediateTabPage.SuspendLayout()
        Me.BsIntermediateInfoPanel.SuspendLayout()
        Me.BsIntermediateAdjustPanel.SuspendLayout()
        Me.IntermediateTanksTestGroupBox.SuspendLayout()
        CType(Me.ProcessDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.DWInputGroupBox.SuspendLayout()
        Me.LCGroupBox.SuspendLayout()
        Me.LCPanel.SuspendLayout()
        Me.DWGroupBox.SuspendLayout()
        Me.DWPanel.SuspendLayout()
        Me.BsButtonsPanel.SuspendLayout()
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TestProcessTimer
        '
        '
        'BsResponse
        '
        Me.BsResponse.BackColor = System.Drawing.Color.White
        Me.BsResponse.DecimalsValues = False
        Me.BsResponse.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsResponse.IsNumeric = False
        Me.BsResponse.Location = New System.Drawing.Point(52, 57)
        Me.BsResponse.Mandatory = False
        Me.BsResponse.Multiline = True
        Me.BsResponse.Name = "BsResponse"
        Me.BsResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.BsResponse.Size = New System.Drawing.Size(836, 130)
        Me.BsResponse.TabIndex = 43
        '
        'BsPanel1
        '
        Me.BsPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsPanel1.Name = "BsPanel1"
        Me.BsPanel1.Size = New System.Drawing.Size(200, 100)
        Me.BsPanel1.TabIndex = 0
        '
        'BsLabel4
        '
        Me.BsLabel4.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel4.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel4.ForeColor = System.Drawing.Color.Black
        Me.BsLabel4.Location = New System.Drawing.Point(0, 0)
        Me.BsLabel4.Name = "BsLabel4"
        Me.BsLabel4.Size = New System.Drawing.Size(100, 23)
        Me.BsLabel4.TabIndex = 0
        Me.BsLabel4.Title = False
        '
        'BsLabel5
        '
        Me.BsLabel5.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel5.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel5.ForeColor = System.Drawing.Color.Black
        Me.BsLabel5.Location = New System.Drawing.Point(0, 0)
        Me.BsLabel5.Name = "BsLabel5"
        Me.BsLabel5.Size = New System.Drawing.Size(100, 23)
        Me.BsLabel5.TabIndex = 0
        Me.BsLabel5.Title = False
        '
        'TestSimulatorTimer
        '
        Me.TestSimulatorTimer.Interval = 1000
        '
        'BsTabPagesControl
        '
        Me.BsTabPagesControl.Controls.Add(Me.BsScalesTabPage)
        Me.BsTabPagesControl.Controls.Add(Me.BsIntermediateTabPage)
        Me.BsTabPagesControl.Location = New System.Drawing.Point(0, 0)
        Me.BsTabPagesControl.Name = "BsTabPagesControl"
        Me.BsTabPagesControl.SelectedIndex = 0
        Me.BsTabPagesControl.Size = New System.Drawing.Size(978, 558)
        Me.BsTabPagesControl.TabIndex = 14
        '
        'BsScalesTabPage
        '
        Me.BsScalesTabPage.Controls.Add(Me.BsTanksInfoPanel)
        Me.BsScalesTabPage.Controls.Add(Me.BsTanksAdjustPanel)
        Me.BsScalesTabPage.Location = New System.Drawing.Point(4, 22)
        Me.BsScalesTabPage.Name = "BsScalesTabPage"
        Me.BsScalesTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.BsScalesTabPage.Size = New System.Drawing.Size(970, 532)
        Me.BsScalesTabPage.TabIndex = 0
        Me.BsScalesTabPage.Text = "Scales"
        Me.BsScalesTabPage.UseVisualStyleBackColor = True
        '
        'BsTanksInfoPanel
        '
        Me.BsTanksInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsTanksInfoPanel.Controls.Add(Me.BsInfoScalesXPSViewer)
        Me.BsTanksInfoPanel.Controls.Add(Me.BsInfoExpandButton)
        Me.BsTanksInfoPanel.Controls.Add(Me.BsTanksInfoTitle)
        Me.BsTanksInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsTanksInfoPanel.Name = "BsTanksInfoPanel"
        Me.BsTanksInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsTanksInfoPanel.TabIndex = 25
        '
        'BsInfoScalesXPSViewer
        '
        Me.BsInfoScalesXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoScalesXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoScalesXPSViewer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsInfoScalesXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoScalesXPSViewer.CopyButtonVisible = True
        Me.BsInfoScalesXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoScalesXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoScalesXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoScalesXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoScalesXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoScalesXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoScalesXPSViewer.HorizontalPageMargin = 10
        Me.BsInfoScalesXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoScalesXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoScalesXPSViewer.IsScrollable = False
        Me.BsInfoScalesXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoScalesXPSViewer.MenuBarVisible = False
        Me.BsInfoScalesXPSViewer.Name = "BsInfoScalesXPSViewer"
        Me.BsInfoScalesXPSViewer.PopupMenuEnabled = True
        Me.BsInfoScalesXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoScalesXPSViewer.PrintButtonVisible = True
        Me.BsInfoScalesXPSViewer.SearchBarVisible = False
        Me.BsInfoScalesXPSViewer.Size = New System.Drawing.Size(230, 509)
        Me.BsInfoScalesXPSViewer.TabIndex = 85
        Me.BsInfoScalesXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoScalesXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoScalesXPSViewer.VerticalPageMargin = 10
        Me.BsInfoScalesXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoScalesXPSViewer.WholePageButtonVisible = True
        '
        'BsInfoExpandButton
        '
        Me.BsInfoExpandButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsInfoExpandButton.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsInfoExpandButton.Location = New System.Drawing.Point(210, 0)
        Me.BsInfoExpandButton.Name = "BsInfoExpandButton"
        Me.BsInfoExpandButton.Size = New System.Drawing.Size(20, 20)
        Me.BsInfoExpandButton.TabIndex = 33
        Me.BsInfoExpandButton.Visible = False
        '
        'BsTanksInfoTitle
        '
        Me.BsTanksInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTanksInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsTanksInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsTanksInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsTanksInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsTanksInfoTitle.Name = "BsTanksInfoTitle"
        Me.BsTanksInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsTanksInfoTitle.TabIndex = 22
        Me.BsTanksInfoTitle.Text = "Information"
        Me.BsTanksInfoTitle.Title = True
        '
        'BsTanksAdjustPanel
        '
        Me.BsTanksAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTanksAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsTanksAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsTanksAdjustPanel.Controls.Add(Me.HCWGroupBox)
        Me.BsTanksAdjustPanel.Controls.Add(Me.WashingSolutionGroupBox)
        Me.BsTanksAdjustPanel.Controls.Add(Me.BsTanksTestButton)
        Me.BsTanksAdjustPanel.Controls.Add(Me.BsTanksAdjustTitle)
        Me.BsTanksAdjustPanel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsTanksAdjustPanel.ForeColor = System.Drawing.Color.SteelBlue
        Me.BsTanksAdjustPanel.Location = New System.Drawing.Point(231, 0)
        Me.BsTanksAdjustPanel.Name = "BsTanksAdjustPanel"
        Me.BsTanksAdjustPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsTanksAdjustPanel.TabIndex = 21
        '
        'HCWGroupBox
        '
        Me.HCWGroupBox.BackColor = System.Drawing.Color.Transparent
        Me.HCWGroupBox.Controls.Add(Me.HCTableLayoutPanel)
        Me.HCWGroupBox.Controls.Add(Me.HCMonitorTank)
        Me.HCWGroupBox.Controls.Add(Me.HCCountsLabel)
        Me.HCWGroupBox.Controls.Add(Me.HCCurrentLabel)
        Me.HCWGroupBox.Controls.Add(Me.HCCurrentPercentLabel)
        Me.HCWGroupBox.Controls.Add(Me.HCPercentLabel)
        Me.HCWGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HCWGroupBox.ForeColor = System.Drawing.Color.Black
        Me.HCWGroupBox.Location = New System.Drawing.Point(373, 35)
        Me.HCWGroupBox.Name = "HCWGroupBox"
        Me.HCWGroupBox.Size = New System.Drawing.Size(337, 475)
        Me.HCWGroupBox.TabIndex = 52
        Me.HCWGroupBox.TabStop = False
        Me.HCWGroupBox.Text = "High Contamination Waste"
        '
        'HCTableLayoutPanel
        '
        Me.HCTableLayoutPanel.ColumnCount = 6
        Me.HCTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110.0!))
        Me.HCTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10.0!))
        Me.HCTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80.0!))
        Me.HCTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10.0!))
        Me.HCTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.HCTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.HCTableLayoutPanel.Controls.Add(Me.HCEmptyNewValueLabel, 5, 2)
        Me.HCTableLayoutPanel.Controls.Add(Me.HCEmptySavedLabel, 0, 2)
        Me.HCTableLayoutPanel.Controls.Add(Me.HCFullAdjustButton, 2, 1)
        Me.HCTableLayoutPanel.Controls.Add(Me.HCFullSavedLabel, 0, 1)
        Me.HCTableLayoutPanel.Controls.Add(Me.HCEmptyAdjustButton, 2, 2)
        Me.HCTableLayoutPanel.Controls.Add(Me.HCEmptySavedPictureBox, 4, 2)
        Me.HCTableLayoutPanel.Controls.Add(Me.HCFullNewValueLabel, 5, 1)
        Me.HCTableLayoutPanel.Controls.Add(Me.HCFullSavedPictureBox, 4, 1)
        Me.HCTableLayoutPanel.Controls.Add(Me.HCSavedLabel, 0, 0)
        Me.HCTableLayoutPanel.Location = New System.Drawing.Point(18, 38)
        Me.HCTableLayoutPanel.Name = "HCTableLayoutPanel"
        Me.HCTableLayoutPanel.RowCount = 3
        Me.HCTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.HCTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.0!))
        Me.HCTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.0!))
        Me.HCTableLayoutPanel.Size = New System.Drawing.Size(300, 103)
        Me.HCTableLayoutPanel.TabIndex = 81
        '
        'HCEmptyNewValueLabel
        '
        Me.HCEmptyNewValueLabel.AutoSize = True
        Me.HCEmptyNewValueLabel.BackColor = System.Drawing.Color.YellowGreen
        Me.HCEmptyNewValueLabel.Font = New System.Drawing.Font("Digiface", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HCEmptyNewValueLabel.ForeColor = System.Drawing.Color.White
        Me.HCEmptyNewValueLabel.Location = New System.Drawing.Point(243, 66)
        Me.HCEmptyNewValueLabel.Name = "HCEmptyNewValueLabel"
        Me.HCEmptyNewValueLabel.Size = New System.Drawing.Size(52, 26)
        Me.HCEmptyNewValueLabel.TabIndex = 73
        Me.HCEmptyNewValueLabel.Text = "1425"
        Me.HCEmptyNewValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.HCEmptyNewValueLabel.Visible = False
        '
        'HCEmptySavedLabel
        '
        Me.HCEmptySavedLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.HCEmptySavedLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.HCEmptySavedLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HCEmptySavedLabel.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.HCEmptySavedLabel.ForeColor = System.Drawing.Color.Black
        Me.HCEmptySavedLabel.Location = New System.Drawing.Point(3, 66)
        Me.HCEmptySavedLabel.Name = "HCEmptySavedLabel"
        Me.HCEmptySavedLabel.Size = New System.Drawing.Size(104, 37)
        Me.HCEmptySavedLabel.TabIndex = 76
        Me.HCEmptySavedLabel.Text = "2200"
        Me.HCEmptySavedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'HCFullAdjustButton
        '
        Me.HCFullAdjustButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HCFullAdjustButton.Enabled = False
        Me.HCFullAdjustButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HCFullAdjustButton.ForeColor = System.Drawing.Color.Black
        Me.HCFullAdjustButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.HCFullAdjustButton.Location = New System.Drawing.Point(120, 30)
        Me.HCFullAdjustButton.Margin = New System.Windows.Forms.Padding(0)
        Me.HCFullAdjustButton.Name = "HCFullAdjustButton"
        Me.HCFullAdjustButton.Size = New System.Drawing.Size(80, 36)
        Me.HCFullAdjustButton.TabIndex = 77
        Me.HCFullAdjustButton.Text = "Full"
        Me.HCFullAdjustButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.HCFullAdjustButton.UseVisualStyleBackColor = True
        '
        'HCFullSavedLabel
        '
        Me.HCFullSavedLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.HCFullSavedLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.HCFullSavedLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HCFullSavedLabel.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.HCFullSavedLabel.ForeColor = System.Drawing.Color.Black
        Me.HCFullSavedLabel.Location = New System.Drawing.Point(3, 30)
        Me.HCFullSavedLabel.Name = "HCFullSavedLabel"
        Me.HCFullSavedLabel.Size = New System.Drawing.Size(104, 36)
        Me.HCFullSavedLabel.TabIndex = 75
        Me.HCFullSavedLabel.Text = "9830"
        Me.HCFullSavedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'HCEmptyAdjustButton
        '
        Me.HCEmptyAdjustButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HCEmptyAdjustButton.Enabled = False
        Me.HCEmptyAdjustButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HCEmptyAdjustButton.ForeColor = System.Drawing.Color.Black
        Me.HCEmptyAdjustButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.HCEmptyAdjustButton.Location = New System.Drawing.Point(120, 66)
        Me.HCEmptyAdjustButton.Margin = New System.Windows.Forms.Padding(0)
        Me.HCEmptyAdjustButton.Name = "HCEmptyAdjustButton"
        Me.HCEmptyAdjustButton.Size = New System.Drawing.Size(80, 37)
        Me.HCEmptyAdjustButton.TabIndex = 78
        Me.HCEmptyAdjustButton.Text = "Empty"
        Me.HCEmptyAdjustButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.HCEmptyAdjustButton.UseVisualStyleBackColor = True
        '
        'HCEmptySavedPictureBox
        '
        Me.HCEmptySavedPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.HCEmptySavedPictureBox.Location = New System.Drawing.Point(213, 69)
        Me.HCEmptySavedPictureBox.Name = "HCEmptySavedPictureBox"
        Me.HCEmptySavedPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.HCEmptySavedPictureBox.TabIndex = 80
        Me.HCEmptySavedPictureBox.TabStop = False
        Me.HCEmptySavedPictureBox.Visible = False
        '
        'HCFullNewValueLabel
        '
        Me.HCFullNewValueLabel.AutoSize = True
        Me.HCFullNewValueLabel.BackColor = System.Drawing.Color.YellowGreen
        Me.HCFullNewValueLabel.Font = New System.Drawing.Font("Digiface", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HCFullNewValueLabel.ForeColor = System.Drawing.Color.White
        Me.HCFullNewValueLabel.Location = New System.Drawing.Point(243, 30)
        Me.HCFullNewValueLabel.Name = "HCFullNewValueLabel"
        Me.HCFullNewValueLabel.Size = New System.Drawing.Size(52, 26)
        Me.HCFullNewValueLabel.TabIndex = 72
        Me.HCFullNewValueLabel.Text = "3521"
        Me.HCFullNewValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.HCFullNewValueLabel.Visible = False
        '
        'HCFullSavedPictureBox
        '
        Me.HCFullSavedPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.HCFullSavedPictureBox.Location = New System.Drawing.Point(213, 33)
        Me.HCFullSavedPictureBox.Name = "HCFullSavedPictureBox"
        Me.HCFullSavedPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.HCFullSavedPictureBox.TabIndex = 79
        Me.HCFullSavedPictureBox.TabStop = False
        Me.HCFullSavedPictureBox.Visible = False
        '
        'HCSavedLabel
        '
        Me.HCSavedLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HCSavedLabel.ForeColor = System.Drawing.Color.Black
        Me.HCSavedLabel.Location = New System.Drawing.Point(3, 0)
        Me.HCSavedLabel.Name = "HCSavedLabel"
        Me.HCSavedLabel.Size = New System.Drawing.Size(104, 30)
        Me.HCSavedLabel.TabIndex = 73
        Me.HCSavedLabel.Text = "Saved (counts):"
        Me.HCSavedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'HCMonitorTank
        '
        Me.HCMonitorTank.BackColor = System.Drawing.Color.Transparent
        Me.HCMonitorTank.BackImage = Nothing
        Me.HCMonitorTank.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status.DISABLED
        Me.HCMonitorTank.LevelValue = 57.32
        Me.HCMonitorTank.Location = New System.Drawing.Point(84, 173)
        Me.HCMonitorTank.LowerLevelValue = 0
        Me.HCMonitorTank.LowerLevelVisible = False
        Me.HCMonitorTank.MaxLimit = 100
        Me.HCMonitorTank.MinLimit = 0
        Me.HCMonitorTank.Name = "HCMonitorTank"
        Me.HCMonitorTank.ScaleDivisions = 4
        Me.HCMonitorTank.ScaleStep = 50
        Me.HCMonitorTank.ScaleSubDivisions = 5
        Me.HCMonitorTank.Size = New System.Drawing.Size(189, 236)
        Me.HCMonitorTank.TabIndex = 74
        Me.HCMonitorTank.TitleAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.HCMonitorTank.TitleFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HCMonitorTank.TitleForeColor = System.Drawing.Color.Black
        Me.HCMonitorTank.TitleHeight = 0
        Me.HCMonitorTank.TitleText = "Control Title"
        Me.HCMonitorTank.UpperLevelValue = 100
        Me.HCMonitorTank.UpperLevelVisible = False
        '
        'HCCountsLabel
        '
        Me.HCCountsLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.HCCountsLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HCCountsLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.HCCountsLabel.Location = New System.Drawing.Point(156, 415)
        Me.HCCountsLabel.Name = "HCCountsLabel"
        Me.HCCountsLabel.Size = New System.Drawing.Size(60, 45)
        Me.HCCountsLabel.TabIndex = 72
        Me.HCCountsLabel.Text = "counts"
        Me.HCCountsLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        Me.HCCountsLabel.Visible = False
        '
        'HCCurrentLabel
        '
        Me.HCCurrentLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.HCCurrentLabel.Font = New System.Drawing.Font("Digiface", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HCCurrentLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.HCCurrentLabel.Location = New System.Drawing.Point(45, 421)
        Me.HCCurrentLabel.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.HCCurrentLabel.Name = "HCCurrentLabel"
        Me.HCCurrentLabel.Size = New System.Drawing.Size(112, 45)
        Me.HCCurrentLabel.TabIndex = 71
        Me.HCCurrentLabel.Text = "3024"
        Me.HCCurrentLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight
        Me.HCCurrentLabel.Visible = False
        '
        'HCCurrentPercentLabel
        '
        Me.HCCurrentPercentLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.HCCurrentPercentLabel.Font = New System.Drawing.Font("Digiface", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HCCurrentPercentLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.HCCurrentPercentLabel.Location = New System.Drawing.Point(207, 423)
        Me.HCCurrentPercentLabel.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.HCCurrentPercentLabel.Name = "HCCurrentPercentLabel"
        Me.HCCurrentPercentLabel.Size = New System.Drawing.Size(66, 41)
        Me.HCCurrentPercentLabel.TabIndex = 68
        Me.HCCurrentPercentLabel.Text = "__._"
        Me.HCCurrentPercentLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight
        Me.HCCurrentPercentLabel.Visible = False
        '
        'HCPercentLabel
        '
        Me.HCPercentLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.HCPercentLabel.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HCPercentLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.HCPercentLabel.Location = New System.Drawing.Point(243, 420)
        Me.HCPercentLabel.Name = "HCPercentLabel"
        Me.HCPercentLabel.Size = New System.Drawing.Size(51, 41)
        Me.HCPercentLabel.TabIndex = 67
        Me.HCPercentLabel.Text = "%"
        Me.HCPercentLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight
        Me.HCPercentLabel.Visible = False
        '
        'WashingSolutionGroupBox
        '
        Me.WashingSolutionGroupBox.Controls.Add(Me.WSMonitorTank)
        Me.WashingSolutionGroupBox.Controls.Add(Me.WSCountsLabel)
        Me.WashingSolutionGroupBox.Controls.Add(Me.WSCurrentLabel)
        Me.WashingSolutionGroupBox.Controls.Add(Me.WSTableLayoutPanel)
        Me.WashingSolutionGroupBox.Controls.Add(Me.WSCurrentPercentLabel)
        Me.WashingSolutionGroupBox.Controls.Add(Me.WSPercentLabel)
        Me.WashingSolutionGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WashingSolutionGroupBox.ForeColor = System.Drawing.Color.Black
        Me.WashingSolutionGroupBox.Location = New System.Drawing.Point(8, 35)
        Me.WashingSolutionGroupBox.Name = "WashingSolutionGroupBox"
        Me.WashingSolutionGroupBox.Size = New System.Drawing.Size(337, 475)
        Me.WashingSolutionGroupBox.TabIndex = 51
        Me.WashingSolutionGroupBox.TabStop = False
        Me.WashingSolutionGroupBox.Text = "Washing Solution"
        '
        'WSMonitorTank
        '
        Me.WSMonitorTank.BackColor = System.Drawing.Color.Transparent
        Me.WSMonitorTank.BackImage = Nothing
        Me.WSMonitorTank.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status.DISABLED
        Me.WSMonitorTank.LevelValue = 57.32
        Me.WSMonitorTank.Location = New System.Drawing.Point(84, 173)
        Me.WSMonitorTank.LowerLevelValue = 0
        Me.WSMonitorTank.LowerLevelVisible = False
        Me.WSMonitorTank.MaxLimit = 100
        Me.WSMonitorTank.MinLimit = 0
        Me.WSMonitorTank.Name = "WSMonitorTank"
        Me.WSMonitorTank.ScaleDivisions = 4
        Me.WSMonitorTank.ScaleStep = 50
        Me.WSMonitorTank.ScaleSubDivisions = 5
        Me.WSMonitorTank.Size = New System.Drawing.Size(189, 236)
        Me.WSMonitorTank.TabIndex = 71
        Me.WSMonitorTank.TitleAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.WSMonitorTank.TitleFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WSMonitorTank.TitleForeColor = System.Drawing.Color.Black
        Me.WSMonitorTank.TitleHeight = 0
        Me.WSMonitorTank.TitleText = "Control Title"
        Me.WSMonitorTank.UpperLevelValue = 100
        Me.WSMonitorTank.UpperLevelVisible = False
        '
        'WSCountsLabel
        '
        Me.WSCountsLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.WSCountsLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WSCountsLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.WSCountsLabel.Location = New System.Drawing.Point(156, 415)
        Me.WSCountsLabel.Name = "WSCountsLabel"
        Me.WSCountsLabel.Size = New System.Drawing.Size(60, 45)
        Me.WSCountsLabel.TabIndex = 69
        Me.WSCountsLabel.Text = "counts"
        Me.WSCountsLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        Me.WSCountsLabel.Visible = False
        '
        'WSCurrentLabel
        '
        Me.WSCurrentLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.WSCurrentLabel.Font = New System.Drawing.Font("Digiface", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WSCurrentLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.WSCurrentLabel.Location = New System.Drawing.Point(45, 421)
        Me.WSCurrentLabel.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.WSCurrentLabel.Name = "WSCurrentLabel"
        Me.WSCurrentLabel.Size = New System.Drawing.Size(112, 45)
        Me.WSCurrentLabel.TabIndex = 68
        Me.WSCurrentLabel.Text = "1750"
        Me.WSCurrentLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight
        Me.WSCurrentLabel.Visible = False
        '
        'WSTableLayoutPanel
        '
        Me.WSTableLayoutPanel.ColumnCount = 6
        Me.WSTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110.0!))
        Me.WSTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10.0!))
        Me.WSTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80.0!))
        Me.WSTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10.0!))
        Me.WSTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.WSTableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.WSTableLayoutPanel.Controls.Add(Me.WSEmptyNewValueLabel, 5, 2)
        Me.WSTableLayoutPanel.Controls.Add(Me.WSFullSavedLabel, 0, 1)
        Me.WSTableLayoutPanel.Controls.Add(Me.WSFullNewValueLabel, 5, 1)
        Me.WSTableLayoutPanel.Controls.Add(Me.WSSavedLabel, 0, 0)
        Me.WSTableLayoutPanel.Controls.Add(Me.WSEmptySavedPictureBox, 4, 2)
        Me.WSTableLayoutPanel.Controls.Add(Me.WSEmptyAdjustButton, 2, 2)
        Me.WSTableLayoutPanel.Controls.Add(Me.WSFullSavedPictureBox, 4, 1)
        Me.WSTableLayoutPanel.Controls.Add(Me.WSEmptySavedLabel, 0, 2)
        Me.WSTableLayoutPanel.Controls.Add(Me.WSFullAdjustButton, 2, 1)
        Me.WSTableLayoutPanel.Location = New System.Drawing.Point(18, 38)
        Me.WSTableLayoutPanel.Name = "WSTableLayoutPanel"
        Me.WSTableLayoutPanel.RowCount = 3
        Me.WSTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.WSTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.0!))
        Me.WSTableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.0!))
        Me.WSTableLayoutPanel.Size = New System.Drawing.Size(300, 103)
        Me.WSTableLayoutPanel.TabIndex = 67
        '
        'WSEmptyNewValueLabel
        '
        Me.WSEmptyNewValueLabel.AutoSize = True
        Me.WSEmptyNewValueLabel.BackColor = System.Drawing.Color.YellowGreen
        Me.WSEmptyNewValueLabel.Font = New System.Drawing.Font("Digiface", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WSEmptyNewValueLabel.ForeColor = System.Drawing.Color.White
        Me.WSEmptyNewValueLabel.Location = New System.Drawing.Point(243, 66)
        Me.WSEmptyNewValueLabel.Name = "WSEmptyNewValueLabel"
        Me.WSEmptyNewValueLabel.Size = New System.Drawing.Size(52, 26)
        Me.WSEmptyNewValueLabel.TabIndex = 73
        Me.WSEmptyNewValueLabel.Text = "2000"
        Me.WSEmptyNewValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.WSEmptyNewValueLabel.Visible = False
        '
        'WSFullSavedLabel
        '
        Me.WSFullSavedLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.WSFullSavedLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.WSFullSavedLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WSFullSavedLabel.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.WSFullSavedLabel.ForeColor = System.Drawing.Color.Black
        Me.WSFullSavedLabel.Location = New System.Drawing.Point(3, 30)
        Me.WSFullSavedLabel.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.WSFullSavedLabel.Name = "WSFullSavedLabel"
        Me.WSFullSavedLabel.Size = New System.Drawing.Size(107, 36)
        Me.WSFullSavedLabel.TabIndex = 42
        Me.WSFullSavedLabel.Text = "9862"
        Me.WSFullSavedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'WSFullNewValueLabel
        '
        Me.WSFullNewValueLabel.AutoSize = True
        Me.WSFullNewValueLabel.BackColor = System.Drawing.Color.YellowGreen
        Me.WSFullNewValueLabel.Font = New System.Drawing.Font("Digiface", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WSFullNewValueLabel.ForeColor = System.Drawing.Color.White
        Me.WSFullNewValueLabel.Location = New System.Drawing.Point(243, 30)
        Me.WSFullNewValueLabel.Name = "WSFullNewValueLabel"
        Me.WSFullNewValueLabel.Size = New System.Drawing.Size(52, 26)
        Me.WSFullNewValueLabel.TabIndex = 72
        Me.WSFullNewValueLabel.Text = "1500"
        Me.WSFullNewValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.WSFullNewValueLabel.Visible = False
        '
        'WSSavedLabel
        '
        Me.WSSavedLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WSSavedLabel.ForeColor = System.Drawing.Color.Black
        Me.WSSavedLabel.Location = New System.Drawing.Point(3, 0)
        Me.WSSavedLabel.Name = "WSSavedLabel"
        Me.WSSavedLabel.Size = New System.Drawing.Size(104, 30)
        Me.WSSavedLabel.TabIndex = 70
        Me.WSSavedLabel.Text = "Saved (counts):"
        Me.WSSavedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'WSEmptySavedPictureBox
        '
        Me.WSEmptySavedPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.WSEmptySavedPictureBox.Location = New System.Drawing.Point(213, 69)
        Me.WSEmptySavedPictureBox.Name = "WSEmptySavedPictureBox"
        Me.WSEmptySavedPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.WSEmptySavedPictureBox.TabIndex = 64
        Me.WSEmptySavedPictureBox.TabStop = False
        Me.WSEmptySavedPictureBox.Visible = False
        '
        'WSEmptyAdjustButton
        '
        Me.WSEmptyAdjustButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WSEmptyAdjustButton.Enabled = False
        Me.WSEmptyAdjustButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WSEmptyAdjustButton.ForeColor = System.Drawing.Color.Black
        Me.WSEmptyAdjustButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.WSEmptyAdjustButton.Location = New System.Drawing.Point(120, 66)
        Me.WSEmptyAdjustButton.Margin = New System.Windows.Forms.Padding(0)
        Me.WSEmptyAdjustButton.Name = "WSEmptyAdjustButton"
        Me.WSEmptyAdjustButton.Size = New System.Drawing.Size(80, 37)
        Me.WSEmptyAdjustButton.TabIndex = 66
        Me.WSEmptyAdjustButton.Text = "Empty"
        Me.WSEmptyAdjustButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.WSEmptyAdjustButton.UseVisualStyleBackColor = True
        '
        'WSFullSavedPictureBox
        '
        Me.WSFullSavedPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.WSFullSavedPictureBox.Location = New System.Drawing.Point(213, 33)
        Me.WSFullSavedPictureBox.Name = "WSFullSavedPictureBox"
        Me.WSFullSavedPictureBox.Size = New System.Drawing.Size(20, 20)
        Me.WSFullSavedPictureBox.TabIndex = 63
        Me.WSFullSavedPictureBox.TabStop = False
        Me.WSFullSavedPictureBox.Visible = False
        '
        'WSEmptySavedLabel
        '
        Me.WSEmptySavedLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.WSEmptySavedLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.WSEmptySavedLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WSEmptySavedLabel.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.WSEmptySavedLabel.ForeColor = System.Drawing.Color.Black
        Me.WSEmptySavedLabel.Location = New System.Drawing.Point(3, 66)
        Me.WSEmptySavedLabel.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.WSEmptySavedLabel.Name = "WSEmptySavedLabel"
        Me.WSEmptySavedLabel.Size = New System.Drawing.Size(107, 37)
        Me.WSEmptySavedLabel.TabIndex = 43
        Me.WSEmptySavedLabel.Text = "1005"
        Me.WSEmptySavedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'WSFullAdjustButton
        '
        Me.WSFullAdjustButton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WSFullAdjustButton.Enabled = False
        Me.WSFullAdjustButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WSFullAdjustButton.ForeColor = System.Drawing.Color.Black
        Me.WSFullAdjustButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.WSFullAdjustButton.Location = New System.Drawing.Point(120, 30)
        Me.WSFullAdjustButton.Margin = New System.Windows.Forms.Padding(0)
        Me.WSFullAdjustButton.Name = "WSFullAdjustButton"
        Me.WSFullAdjustButton.Size = New System.Drawing.Size(80, 36)
        Me.WSFullAdjustButton.TabIndex = 65
        Me.WSFullAdjustButton.Text = "Full"
        Me.WSFullAdjustButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.WSFullAdjustButton.UseVisualStyleBackColor = True
        '
        'WSCurrentPercentLabel
        '
        Me.WSCurrentPercentLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.WSCurrentPercentLabel.Font = New System.Drawing.Font("Digiface", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WSCurrentPercentLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.WSCurrentPercentLabel.Location = New System.Drawing.Point(207, 423)
        Me.WSCurrentPercentLabel.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.WSCurrentPercentLabel.Name = "WSCurrentPercentLabel"
        Me.WSCurrentPercentLabel.Size = New System.Drawing.Size(66, 41)
        Me.WSCurrentPercentLabel.TabIndex = 55
        Me.WSCurrentPercentLabel.Text = "__._"
        Me.WSCurrentPercentLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight
        Me.WSCurrentPercentLabel.Visible = False
        '
        'WSPercentLabel
        '
        Me.WSPercentLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.WSPercentLabel.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WSPercentLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.WSPercentLabel.Location = New System.Drawing.Point(243, 420)
        Me.WSPercentLabel.Name = "WSPercentLabel"
        Me.WSPercentLabel.Size = New System.Drawing.Size(51, 41)
        Me.WSPercentLabel.TabIndex = 54
        Me.WSPercentLabel.Text = "%"
        Me.WSPercentLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight
        Me.WSPercentLabel.Visible = False
        '
        'BsTanksTestButton
        '
        Me.BsTanksTestButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsTanksTestButton.Enabled = False
        Me.BsTanksTestButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsTanksTestButton.ForeColor = System.Drawing.Color.Black
        Me.BsTanksTestButton.Location = New System.Drawing.Point(652, 428)
        Me.BsTanksTestButton.Name = "BsTanksTestButton"
        Me.BsTanksTestButton.Size = New System.Drawing.Size(51, 32)
        Me.BsTanksTestButton.TabIndex = 35
        Me.BsTanksTestButton.Text = "TEST"
        Me.BsTanksTestButton.UseVisualStyleBackColor = True
        Me.BsTanksTestButton.Visible = False
        '
        'BsTanksAdjustTitle
        '
        Me.BsTanksAdjustTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsTanksAdjustTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsTanksAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.BsTanksAdjustTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsTanksAdjustTitle.Name = "BsTanksAdjustTitle"
        Me.BsTanksAdjustTitle.Size = New System.Drawing.Size(739, 20)
        Me.BsTanksAdjustTitle.TabIndex = 31
        Me.BsTanksAdjustTitle.Text = "Adjustment"
        Me.BsTanksAdjustTitle.Title = True
        '
        'BsIntermediateTabPage
        '
        Me.BsIntermediateTabPage.Controls.Add(Me.BsIntermediateInfoPanel)
        Me.BsIntermediateTabPage.Controls.Add(Me.BsIntermediateAdjustPanel)
        Me.BsIntermediateTabPage.Location = New System.Drawing.Point(4, 22)
        Me.BsIntermediateTabPage.Name = "BsIntermediateTabPage"
        Me.BsIntermediateTabPage.Padding = New System.Windows.Forms.Padding(3)
        Me.BsIntermediateTabPage.Size = New System.Drawing.Size(970, 532)
        Me.BsIntermediateTabPage.TabIndex = 3
        Me.BsIntermediateTabPage.Text = "Internal Tanks"
        Me.BsIntermediateTabPage.UseVisualStyleBackColor = True
        '
        'BsIntermediateInfoPanel
        '
        Me.BsIntermediateInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsIntermediateInfoPanel.Controls.Add(Me.BsIntermediateInfoTitle)
        Me.BsIntermediateInfoPanel.Controls.Add(Me.BsIntermediateInfoLabel)
        Me.BsIntermediateInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsIntermediateInfoPanel.Name = "BsIntermediateInfoPanel"
        Me.BsIntermediateInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsIntermediateInfoPanel.TabIndex = 29
        '
        'BsIntermediateInfoTitle
        '
        Me.BsIntermediateInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsIntermediateInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsIntermediateInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsIntermediateInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsIntermediateInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsIntermediateInfoTitle.Name = "BsIntermediateInfoTitle"
        Me.BsIntermediateInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsIntermediateInfoTitle.TabIndex = 22
        Me.BsIntermediateInfoTitle.Text = "Information"
        Me.BsIntermediateInfoTitle.Title = True
        '
        'BsIntermediateInfoLabel
        '
        Me.BsIntermediateInfoLabel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsIntermediateInfoLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsIntermediateInfoLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsIntermediateInfoLabel.ForeColor = System.Drawing.Color.Black
        Me.BsIntermediateInfoLabel.Location = New System.Drawing.Point(0, 21)
        Me.BsIntermediateInfoLabel.Name = "BsIntermediateInfoLabel"
        Me.BsIntermediateInfoLabel.Size = New System.Drawing.Size(232, 328)
        Me.BsIntermediateInfoLabel.TabIndex = 23
        Me.BsIntermediateInfoLabel.Text = resources.GetString("BsIntermediateInfoLabel.Text")
        Me.BsIntermediateInfoLabel.Title = False
        '
        'BsIntermediateAdjustPanel
        '
        Me.BsIntermediateAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsIntermediateAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsIntermediateAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsIntermediateAdjustPanel.Controls.Add(Me.ProgressBar1)
        Me.BsIntermediateAdjustPanel.Controls.Add(Me.TestProgressBar)
        Me.BsIntermediateAdjustPanel.Controls.Add(Me.IntermediateTanksTestGroupBox)
        Me.BsIntermediateAdjustPanel.Controls.Add(Me.DWInputGroupBox)
        Me.BsIntermediateAdjustPanel.Controls.Add(Me.BsInternalTanksAdjustTitle)
        Me.BsIntermediateAdjustPanel.Controls.Add(Me.LCGroupBox)
        Me.BsIntermediateAdjustPanel.Controls.Add(Me.DWGroupBox)
        Me.BsIntermediateAdjustPanel.Controls.Add(Me.BsIntermediateAdjustTitle)
        Me.BsIntermediateAdjustPanel.Location = New System.Drawing.Point(231, 0)
        Me.BsIntermediateAdjustPanel.Name = "BsIntermediateAdjustPanel"
        Me.BsIntermediateAdjustPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsIntermediateAdjustPanel.TabIndex = 28
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(13, 480)
        Me.ProgressBar1.Maximum = 300
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(180, 18)
        Me.ProgressBar1.Step = 1
        Me.ProgressBar1.TabIndex = 46
        Me.ProgressBar1.Visible = False
        '
        'TestProgressBar
        '
        Me.TestProgressBar.Location = New System.Drawing.Point(13, 456)
        Me.TestProgressBar.Maximum = 300
        Me.TestProgressBar.Name = "TestProgressBar"
        Me.TestProgressBar.Size = New System.Drawing.Size(180, 18)
        Me.TestProgressBar.Step = 1
        Me.TestProgressBar.TabIndex = 4
        Me.TestProgressBar.Visible = False
        '
        'IntermediateTanksTestGroupBox
        '
        Me.IntermediateTanksTestGroupBox.Controls.Add(Me.BsTestLabel)
        Me.IntermediateTanksTestGroupBox.Controls.Add(Me.BsStartTestButton)
        Me.IntermediateTanksTestGroupBox.Controls.Add(Me.ProcessDataGridView)
        Me.IntermediateTanksTestGroupBox.Controls.Add(Me.BsStopTestButton)
        Me.IntermediateTanksTestGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.IntermediateTanksTestGroupBox.ForeColor = System.Drawing.Color.Black
        Me.IntermediateTanksTestGroupBox.Location = New System.Drawing.Point(13, 151)
        Me.IntermediateTanksTestGroupBox.Name = "IntermediateTanksTestGroupBox"
        Me.IntermediateTanksTestGroupBox.Size = New System.Drawing.Size(180, 289)
        Me.IntermediateTanksTestGroupBox.TabIndex = 45
        Me.IntermediateTanksTestGroupBox.TabStop = False
        Me.IntermediateTanksTestGroupBox.Text = "Process Steps"
        '
        'BsTestLabel
        '
        Me.BsTestLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsTestLabel.ForeColor = System.Drawing.Color.Black
        Me.BsTestLabel.Location = New System.Drawing.Point(23, 26)
        Me.BsTestLabel.Name = "BsTestLabel"
        Me.BsTestLabel.Size = New System.Drawing.Size(84, 30)
        Me.BsTestLabel.TabIndex = 71
        Me.BsTestLabel.Text = "Test:"
        Me.BsTestLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BsStartTestButton
        '
        Me.BsStartTestButton.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsStartTestButton.ForeColor = System.Drawing.Color.Black
        Me.BsStartTestButton.Location = New System.Drawing.Point(144, 23)
        Me.BsStartTestButton.Name = "BsStartTestButton"
        Me.BsStartTestButton.Size = New System.Drawing.Size(32, 32)
        Me.BsStartTestButton.TabIndex = 42
        Me.BsStartTestButton.UseVisualStyleBackColor = True
        '
        'ProcessDataGridView
        '
        Me.ProcessDataGridView.AllowUserToAddRows = False
        Me.ProcessDataGridView.AllowUserToDeleteRows = False
        Me.ProcessDataGridView.AllowUserToResizeColumns = False
        Me.ProcessDataGridView.AllowUserToResizeRows = False
        Me.ProcessDataGridView.BackgroundColor = System.Drawing.Color.Gainsboro
        Me.ProcessDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.ProcessDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.ProcessDataGridView.ColumnHeadersVisible = False
        Me.ProcessDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.NumberColumn, Me.IconColumn, Me.StepColumn})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.Gainsboro
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Gainsboro
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.ProcessDataGridView.DefaultCellStyle = DataGridViewCellStyle2
        Me.ProcessDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.ProcessDataGridView.GridColor = System.Drawing.Color.Gainsboro
        Me.ProcessDataGridView.Location = New System.Drawing.Point(5, 61)
        Me.ProcessDataGridView.MultiSelect = False
        Me.ProcessDataGridView.Name = "ProcessDataGridView"
        Me.ProcessDataGridView.ReadOnly = True
        Me.ProcessDataGridView.RowHeadersVisible = False
        Me.ProcessDataGridView.RowHeadersWidth = 10
        Me.ProcessDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ProcessDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle3
        Me.ProcessDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None
        Me.ProcessDataGridView.ShowCellErrors = False
        Me.ProcessDataGridView.ShowCellToolTips = False
        Me.ProcessDataGridView.ShowEditingIcon = False
        Me.ProcessDataGridView.ShowRowErrors = False
        Me.ProcessDataGridView.Size = New System.Drawing.Size(170, 210)
        Me.ProcessDataGridView.TabIndex = 44
        '
        'NumberColumn
        '
        Me.NumberColumn.HeaderText = ""
        Me.NumberColumn.Name = "NumberColumn"
        Me.NumberColumn.ReadOnly = True
        Me.NumberColumn.Visible = False
        Me.NumberColumn.Width = 20
        '
        'IconColumn
        '
        Me.IconColumn.HeaderText = ""
        Me.IconColumn.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Stretch
        Me.IconColumn.Name = "IconColumn"
        Me.IconColumn.ReadOnly = True
        Me.IconColumn.Width = 30
        '
        'StepColumn
        '
        Me.StepColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StepColumn.DefaultCellStyle = DataGridViewCellStyle1
        Me.StepColumn.HeaderText = ""
        Me.StepColumn.Name = "StepColumn"
        Me.StepColumn.ReadOnly = True
        Me.StepColumn.Width = 5
        '
        'BsStopTestButton
        '
        Me.BsStopTestButton.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsStopTestButton.ForeColor = System.Drawing.Color.Black
        Me.BsStopTestButton.Location = New System.Drawing.Point(144, 26)
        Me.BsStopTestButton.Name = "BsStopTestButton"
        Me.BsStopTestButton.Size = New System.Drawing.Size(30, 30)
        Me.BsStopTestButton.TabIndex = 43
        Me.BsStopTestButton.UseVisualStyleBackColor = True
        Me.BsStopTestButton.Visible = False
        '
        'DWInputGroupBox
        '
        Me.DWInputGroupBox.Controls.Add(Me.DWTankInputLabel)
        Me.DWInputGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DWInputGroupBox.ForeColor = System.Drawing.Color.Black
        Me.DWInputGroupBox.Location = New System.Drawing.Point(13, 53)
        Me.DWInputGroupBox.Name = "DWInputGroupBox"
        Me.DWInputGroupBox.Size = New System.Drawing.Size(180, 80)
        Me.DWInputGroupBox.TabIndex = 41
        Me.DWInputGroupBox.TabStop = False
        Me.DWInputGroupBox.Text = "Distilled Water Input"
        '
        'DWTankInputLabel
        '
        Me.DWTankInputLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DWTankInputLabel.ForeColor = System.Drawing.Color.Black
        Me.DWTankInputLabel.Location = New System.Drawing.Point(8, 26)
        Me.DWTankInputLabel.Name = "DWTankInputLabel"
        Me.DWTankInputLabel.Size = New System.Drawing.Size(166, 35)
        Me.DWTankInputLabel.TabIndex = 0
        Me.DWTankInputLabel.Text = "From external tank"
        Me.DWTankInputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BsInternalTanksAdjustTitle
        '
        Me.BsInternalTanksAdjustTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsInternalTanksAdjustTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.BsInternalTanksAdjustTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsInternalTanksAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.BsInternalTanksAdjustTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsInternalTanksAdjustTitle.Name = "BsInternalTanksAdjustTitle"
        Me.BsInternalTanksAdjustTitle.Size = New System.Drawing.Size(737, 20)
        Me.BsInternalTanksAdjustTitle.TabIndex = 39
        Me.BsInternalTanksAdjustTitle.Text = "Test"
        Me.BsInternalTanksAdjustTitle.Title = True
        '
        'LCGroupBox
        '
        Me.LCGroupBox.Controls.Add(Me.LCPanel)
        Me.LCGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LCGroupBox.ForeColor = System.Drawing.Color.Black
        Me.LCGroupBox.Location = New System.Drawing.Point(470, 53)
        Me.LCGroupBox.Name = "LCGroupBox"
        Me.LCGroupBox.Size = New System.Drawing.Size(243, 387)
        Me.LCGroupBox.TabIndex = 37
        Me.LCGroupBox.TabStop = False
        Me.LCGroupBox.Text = "Low Contamination Waste Tank"
        '
        'LCPanel
        '
        Me.LCPanel.Controls.Add(Me.LCMonitorTank)
        Me.LCPanel.Location = New System.Drawing.Point(27, 38)
        Me.LCPanel.Name = "LCPanel"
        Me.LCPanel.Size = New System.Drawing.Size(185, 322)
        Me.LCPanel.TabIndex = 47
        '
        'LCMonitorTank
        '
        Me.LCMonitorTank.BackColor = System.Drawing.Color.Transparent
        Me.LCMonitorTank.BackImage = Nothing
        Me.LCMonitorTank.BottomLevelImage = Nothing
        Me.LCMonitorTank.BottomStopImage = Nothing
        Me.LCMonitorTank.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status.DISABLED
        Me.LCMonitorTank.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LCMonitorTank.LevelValue = 50
        Me.LCMonitorTank.Location = New System.Drawing.Point(0, 0)
        Me.LCMonitorTank.LowerLevelValue = 0
        Me.LCMonitorTank.LowerLevelVisible = False
        Me.LCMonitorTank.MaxLimit = 100
        Me.LCMonitorTank.MinLimit = 0
        Me.LCMonitorTank.Name = "LCMonitorTank"
        Me.LCMonitorTank.SharpImage = Nothing
        Me.LCMonitorTank.Size = New System.Drawing.Size(185, 322)
        Me.LCMonitorTank.TabIndex = 46
        Me.LCMonitorTank.TankLevel = Biosystems.Ax00.Controls.UserControls.BSMonitorTankLevels.TankLevels.MIDDLE
        Me.LCMonitorTank.TitleAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.LCMonitorTank.TitleFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LCMonitorTank.TitleForeColor = System.Drawing.Color.Black
        Me.LCMonitorTank.TitleHeight = 0
        Me.LCMonitorTank.TitleText = "Control Title"
        Me.LCMonitorTank.TopLevelImage = Nothing
        Me.LCMonitorTank.TopStopImage = Nothing
        Me.LCMonitorTank.UpperLevelValue = 100
        Me.LCMonitorTank.UpperLevelVisible = False
        '
        'DWGroupBox
        '
        Me.DWGroupBox.Controls.Add(Me.DWPanel)
        Me.DWGroupBox.Enabled = False
        Me.DWGroupBox.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DWGroupBox.ForeColor = System.Drawing.Color.Black
        Me.DWGroupBox.Location = New System.Drawing.Point(199, 53)
        Me.DWGroupBox.Name = "DWGroupBox"
        Me.DWGroupBox.Size = New System.Drawing.Size(243, 387)
        Me.DWGroupBox.TabIndex = 36
        Me.DWGroupBox.TabStop = False
        Me.DWGroupBox.Text = "Distilled Water Tank"
        '
        'DWPanel
        '
        Me.DWPanel.Controls.Add(Me.DWMonitorTank)
        Me.DWPanel.Location = New System.Drawing.Point(27, 38)
        Me.DWPanel.Name = "DWPanel"
        Me.DWPanel.Size = New System.Drawing.Size(185, 322)
        Me.DWPanel.TabIndex = 48
        '
        'DWMonitorTank
        '
        Me.DWMonitorTank.BackColor = System.Drawing.Color.Transparent
        Me.DWMonitorTank.BackImage = Nothing
        Me.DWMonitorTank.BottomLevelImage = Nothing
        Me.DWMonitorTank.BottomStopImage = Nothing
        Me.DWMonitorTank.CurrentStatus = Biosystems.Ax00.Controls.UserControls.BSMonitorControlBase.Status.DISABLED
        Me.DWMonitorTank.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DWMonitorTank.LevelValue = 10
        Me.DWMonitorTank.Location = New System.Drawing.Point(0, 0)
        Me.DWMonitorTank.LowerLevelValue = 0
        Me.DWMonitorTank.LowerLevelVisible = False
        Me.DWMonitorTank.MaxLimit = 100
        Me.DWMonitorTank.MinLimit = 0
        Me.DWMonitorTank.Name = "DWMonitorTank"
        Me.DWMonitorTank.SharpImage = Nothing
        Me.DWMonitorTank.Size = New System.Drawing.Size(185, 322)
        Me.DWMonitorTank.TabIndex = 46
        Me.DWMonitorTank.TankLevel = Biosystems.Ax00.Controls.UserControls.BSMonitorTankLevels.TankLevels.BOTTOM
        Me.DWMonitorTank.TitleAlignment = System.Drawing.ContentAlignment.BottomCenter
        Me.DWMonitorTank.TitleFont = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DWMonitorTank.TitleForeColor = System.Drawing.Color.Black
        Me.DWMonitorTank.TitleHeight = 0
        Me.DWMonitorTank.TitleText = "Control Title"
        Me.DWMonitorTank.TopLevelImage = Nothing
        Me.DWMonitorTank.TopStopImage = Nothing
        Me.DWMonitorTank.UpperLevelValue = 100
        Me.DWMonitorTank.UpperLevelVisible = False
        '
        'BsIntermediateAdjustTitle
        '
        Me.BsIntermediateAdjustTitle.BackColor = System.Drawing.Color.Transparent
        Me.BsIntermediateAdjustTitle.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsIntermediateAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.BsIntermediateAdjustTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsIntermediateAdjustTitle.Name = "BsIntermediateAdjustTitle"
        Me.BsIntermediateAdjustTitle.Size = New System.Drawing.Size(100, 23)
        Me.BsIntermediateAdjustTitle.TabIndex = 40
        Me.BsIntermediateAdjustTitle.Title = False
        '
        'BsButtonsPanel
        '
        Me.BsButtonsPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButtonsPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsButtonsPanel.Controls.Add(Me.BsExitButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsCancelButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsSaveButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsAdjustButton)
        Me.BsButtonsPanel.Location = New System.Drawing.Point(810, 557)
        Me.BsButtonsPanel.Name = "BsButtonsPanel"
        Me.BsButtonsPanel.Size = New System.Drawing.Size(168, 35)
        Me.BsButtonsPanel.TabIndex = 13
        '
        'BsExitButton
        '
        Me.BsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsExitButton.Enabled = False
        Me.BsExitButton.Location = New System.Drawing.Point(134, 1)
        Me.BsExitButton.Name = "BsExitButton"
        Me.BsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.BsExitButton.TabIndex = 3
        Me.BsExitButton.UseVisualStyleBackColor = True
        '
        'BsCancelButton
        '
        Me.BsCancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsCancelButton.Location = New System.Drawing.Point(98, 1)
        Me.BsCancelButton.Name = "BsCancelButton"
        Me.BsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.BsCancelButton.TabIndex = 2
        Me.BsCancelButton.UseVisualStyleBackColor = True
        '
        'BsSaveButton
        '
        Me.BsSaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsSaveButton.Location = New System.Drawing.Point(62, 1)
        Me.BsSaveButton.Name = "BsSaveButton"
        Me.BsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.BsSaveButton.TabIndex = 1
        Me.BsSaveButton.UseVisualStyleBackColor = True
        '
        'BsAdjustButton
        '
        Me.BsAdjustButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsAdjustButton.Enabled = False
        Me.BsAdjustButton.Location = New System.Drawing.Point(26, 1)
        Me.BsAdjustButton.Name = "BsAdjustButton"
        Me.BsAdjustButton.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjustButton.TabIndex = 0
        Me.BsAdjustButton.UseVisualStyleBackColor = True
        Me.BsAdjustButton.Visible = False
        '
        'BsMessagesPanel
        '
        Me.BsMessagesPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessagesPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageImage)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageLabel)
        Me.BsMessagesPanel.Location = New System.Drawing.Point(0, 557)
        Me.BsMessagesPanel.Name = "BsMessagesPanel"
        Me.BsMessagesPanel.Size = New System.Drawing.Size(811, 35)
        Me.BsMessagesPanel.TabIndex = 12
        '
        'BsMessageImage
        '
        Me.BsMessageImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsMessageImage.Location = New System.Drawing.Point(3, 1)
        Me.BsMessageImage.Name = "BsMessageImage"
        Me.BsMessageImage.PositionNumber = 0
        Me.BsMessageImage.Size = New System.Drawing.Size(32, 32)
        Me.BsMessageImage.TabIndex = 3
        Me.BsMessageImage.TabStop = False
        '
        'BsMessageLabel
        '
        Me.BsMessageLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsMessageLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsMessageLabel.ForeColor = System.Drawing.Color.Black
        Me.BsMessageLabel.Location = New System.Drawing.Point(41, 11)
        Me.BsMessageLabel.Name = "BsMessageLabel"
        Me.BsMessageLabel.Size = New System.Drawing.Size(762, 13)
        Me.BsMessageLabel.TabIndex = 1
        Me.BsMessageLabel.Title = False
        '
        'ITankLevelsAdjustments
        '
        Me.AcceptButton = Me.BsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 593)
        Me.Controls.Add(Me.BsTabPagesControl)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "ITankLevelsAdjustments"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.BsTabPagesControl.ResumeLayout(False)
        Me.BsScalesTabPage.ResumeLayout(False)
        Me.BsTanksInfoPanel.ResumeLayout(False)
        Me.BsTanksAdjustPanel.ResumeLayout(False)
        Me.HCWGroupBox.ResumeLayout(False)
        Me.HCTableLayoutPanel.ResumeLayout(False)
        Me.HCTableLayoutPanel.PerformLayout()
        CType(Me.HCEmptySavedPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.HCFullSavedPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.WashingSolutionGroupBox.ResumeLayout(False)
        Me.WSTableLayoutPanel.ResumeLayout(False)
        Me.WSTableLayoutPanel.PerformLayout()
        CType(Me.WSEmptySavedPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.WSFullSavedPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.BsIntermediateTabPage.ResumeLayout(False)
        Me.BsIntermediateInfoPanel.ResumeLayout(False)
        Me.BsIntermediateAdjustPanel.ResumeLayout(False)
        Me.IntermediateTanksTestGroupBox.ResumeLayout(False)
        CType(Me.ProcessDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.DWInputGroupBox.ResumeLayout(False)
        Me.LCGroupBox.ResumeLayout(False)
        Me.LCPanel.ResumeLayout(False)
        Me.DWGroupBox.ResumeLayout(False)
        Me.DWPanel.ResumeLayout(False)
        Me.BsButtonsPanel.ResumeLayout(False)
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsResponse As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjustButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsTabPagesControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents BsScalesTabPage As System.Windows.Forms.TabPage
    Friend WithEvents BsTanksInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTanksAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsTanksAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsTanksInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsTanksTestButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsIntermediateTabPage As System.Windows.Forms.TabPage
    Friend WithEvents BsPanel1 As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsLabel4 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel5 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsNotContaminatedInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsNotContaminatedInfoLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsIntermediateAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsIntermediateAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents WSCurrentPercentLabel As System.Windows.Forms.Label
    Friend WithEvents WSPercentLabel As System.Windows.Forms.Label
    Friend WithEvents WSEmptySavedLabel As System.Windows.Forms.Label
    Friend WithEvents WSFullSavedLabel As System.Windows.Forms.Label
    Friend WithEvents DWGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents LCGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsIntermediateInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsIntermediateInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsIntermediateInfoLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsInternalTanksAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents WashingSolutionGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents WSFullSavedPictureBox As System.Windows.Forms.PictureBox
    Friend WithEvents WSEmptySavedPictureBox As System.Windows.Forms.PictureBox
    Friend WithEvents WSFullAdjustButton As System.Windows.Forms.Button
    Friend WithEvents HCWGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents HCCurrentPercentLabel As System.Windows.Forms.Label
    Friend WithEvents HCPercentLabel As System.Windows.Forms.Label
    Friend WithEvents WSEmptyAdjustButton As System.Windows.Forms.Button
    Friend WithEvents WSTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents WSCurrentLabel As System.Windows.Forms.Label
    Friend WithEvents HCCurrentLabel As System.Windows.Forms.Label
    Friend WithEvents WSCountsLabel As System.Windows.Forms.Label
    Friend WithEvents HCCountsLabel As System.Windows.Forms.Label
    Friend WithEvents HCSavedLabel As System.Windows.Forms.Label
    Friend WithEvents WSSavedLabel As System.Windows.Forms.Label
    Friend WithEvents DWInputGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsStopTestButton As System.Windows.Forms.Button
    Friend WithEvents BsStartTestButton As System.Windows.Forms.Button
    Friend WithEvents ProcessDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents IntermediateTanksTestGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents WSMonitorTank As BSMonitorTankA
    Friend WithEvents HCMonitorTank As BSMonitorTankA
    Friend WithEvents WSEmptyNewValueLabel As System.Windows.Forms.Label
    Friend WithEvents WSFullNewValueLabel As System.Windows.Forms.Label
    Friend WithEvents HCTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents HCEmptyNewValueLabel As System.Windows.Forms.Label
    Friend WithEvents HCFullAdjustButton As System.Windows.Forms.Button
    Friend WithEvents HCFullSavedLabel As System.Windows.Forms.Label
    Friend WithEvents HCEmptyAdjustButton As System.Windows.Forms.Button
    Friend WithEvents HCEmptySavedPictureBox As System.Windows.Forms.PictureBox
    Friend WithEvents HCFullNewValueLabel As System.Windows.Forms.Label
    Friend WithEvents HCFullSavedPictureBox As System.Windows.Forms.PictureBox
    Friend WithEvents HCEmptySavedLabel As System.Windows.Forms.Label
    Friend WithEvents LCMonitorTank As BSMonitorTankLevels
    Friend WithEvents LCPanel As System.Windows.Forms.Panel
    Friend WithEvents DWPanel As System.Windows.Forms.Panel
    Friend WithEvents DWMonitorTank As BSMonitorTankLevels
    Friend WithEvents NumberColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents IconColumn As System.Windows.Forms.DataGridViewImageColumn
    Friend WithEvents StepColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TestProcessTimer As System.Windows.Forms.Timer
    Friend WithEvents DWTankInputLabel As System.Windows.Forms.Label
    Friend WithEvents TestProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents TestSimulatorTimer As System.Windows.Forms.Timer
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents BsInfoExpandButton As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsTestLabel As System.Windows.Forms.Label
    Friend WithEvents BsInfoScalesXPSViewer As BsXPSViewer
End Class
