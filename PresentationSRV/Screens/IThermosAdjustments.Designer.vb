<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiThermosAdjustments
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
        Dim XyDiagram1 As DevExpress.XtraCharts.XYDiagram = New DevExpress.XtraCharts.XYDiagram
        Dim ConstantLine1 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim ConstantLine2 As DevExpress.XtraCharts.ConstantLine = New DevExpress.XtraCharts.ConstantLine
        Dim Series1 As DevExpress.XtraCharts.Series = New DevExpress.XtraCharts.Series
        Dim PointSeriesLabel1 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel
        Dim PointOptions1 As DevExpress.XtraCharts.PointOptions = New DevExpress.XtraCharts.PointOptions
        Dim PointOptions2 As DevExpress.XtraCharts.PointOptions = New DevExpress.XtraCharts.PointOptions
        Dim PointSeriesView1 As DevExpress.XtraCharts.PointSeriesView = New DevExpress.XtraCharts.PointSeriesView
        Dim PointSeriesLabel2 As DevExpress.XtraCharts.PointSeriesLabel = New DevExpress.XtraCharts.PointSeriesLabel
        Dim SplineSeriesView1 As DevExpress.XtraCharts.SplineSeriesView = New DevExpress.XtraCharts.SplineSeriesView
        Dim ChartTitle1 As DevExpress.XtraCharts.ChartTitle = New DevExpress.XtraCharts.ChartTitle
        Me.BsTabPagesControl = New Biosystems.Ax00.Controls.UserControls.BSTabControl
        Me.TabWSHeater = New System.Windows.Forms.TabPage
        Me.BsWSHeaterInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoWsHeaterXPSViewer = New BsXPSViewer
        Me.BsWSHeaterInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsWSHeaterAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BSOpticWSGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab3HeaterWSLabel = New System.Windows.Forms.Label
        Me.BsButton4 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsUpDownWSButton1 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab3AdjustGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab3CorrectionTextBox = New DevExpress.XtraEditors.TextEdit
        Me.Tab3AdjustProposedLabel = New System.Windows.Forms.Label
        Me.Tab3AdjustButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab3MeasurementGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab3TextBoxTemp = New DevExpress.XtraEditors.TextEdit
        Me.Tab3TempMeasuredLabel = New System.Windows.Forms.Label
        Me.Tab3ConditioningGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab3ConditioningButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab3ConditioningDescLabel = New System.Windows.Forms.Label
        Me.BsButton6 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsWSHeaterAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.TabReactionsRotor = New System.Windows.Forms.TabPage
        Me.BsReactionsRotorInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoRotorXPSViewer = New BsXPSViewer
        Me.BsReactionsRotorInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsReactionsRotorAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Tab1RotorReactLabel = New System.Windows.Forms.Label
        Me.BsButton1 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsUpDownWSButton2 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab1AdjustGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab1CorrectionTextBox = New DevExpress.XtraEditors.TextEdit
        Me.Tab1AdjustButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab1AdjustProposedLabel = New System.Windows.Forms.Label
        Me.Tab1ConditioningGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab1ConditioningButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab1AutoRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.Tab1ManualRadioButton = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.Tab1MeasurementGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab1TextBoxTemp1 = New DevExpress.XtraEditors.TextEdit
        Me.Tab1TextBoxTemp2 = New DevExpress.XtraEditors.TextEdit
        Me.Tab1TextBoxTemp3 = New DevExpress.XtraEditors.TextEdit
        Me.Tab1TextBoxTemp4 = New DevExpress.XtraEditors.TextEdit
        Me.Tab1MeanLabel = New System.Windows.Forms.Label
        Me.TabMeanTempLabel = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.ReactionsRotorAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.TabReagentsNeedles = New System.Windows.Forms.TabPage
        Me.BsReagentsNeedlesInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsInfoNeedlesXPSViewer = New BsXPSViewer
        Me.BsReagentsNeedlesInfoTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsReagentsNeedlesAdjustPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.Tab2AuxButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab2AdjustGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab2CorrectionTextBox = New DevExpress.XtraEditors.TextEdit
        Me.Tab2AdjustProposedLabel = New System.Windows.Forms.Label
        Me.Tab2AdjustButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab2MeasurementGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab2TextBoxTemp = New DevExpress.XtraEditors.TextEdit
        Me.Tab2DispensationsCountLabel = New System.Windows.Forms.Label
        Me.Tab2TempMeasuredLabel = New System.Windows.Forms.Label
        Me.Tab2DispensationsCountTitleLabel = New System.Windows.Forms.Label
        Me.Tab2MeasureButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab2ConditioningGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab2ConditioningButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab2ConditioningDescLabel = New System.Windows.Forms.Label
        Me.BsButton9 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Tab2SelectArmGroupBox = New System.Windows.Forms.GroupBox
        Me.Tab2RadioButtonR1 = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.Tab2RadioButtonR2 = New Biosystems.Ax00.Controls.UserControls.BSRadioButton
        Me.BsReagentsNeedlesAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.TabFridge = New System.Windows.Forms.TabPage
        Me.BsFridgeAdjustmentPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsRotorPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsAnalyzerTempGroupBox = New System.Windows.Forms.GroupBox
        Me.BsRotorEditSetPointButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Label32 = New System.Windows.Forms.Label
        Me.TextBox22 = New System.Windows.Forms.TextBox
        Me.BSRotorGroupBox = New System.Windows.Forms.GroupBox
        Me.FridgeTempChart = New DevExpress.XtraCharts.ChartControl
        Me.RotorChartGroupBox = New System.Windows.Forms.GroupBox
        Me.BsButton7 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsButton8 = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.Label30 = New System.Windows.Forms.Label
        Me.Label31 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.TextBox11 = New System.Windows.Forms.TextBox
        Me.Label18 = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.TextBox12 = New System.Windows.Forms.TextBox
        Me.TextBox13 = New System.Windows.Forms.TextBox
        Me.TextBox14 = New System.Windows.Forms.TextBox
        Me.Label20 = New System.Windows.Forms.Label
        Me.Label21 = New System.Windows.Forms.Label
        Me.TextBox15 = New System.Windows.Forms.TextBox
        Me.TextBox16 = New System.Windows.Forms.TextBox
        Me.Label22 = New System.Windows.Forms.Label
        Me.Label23 = New System.Windows.Forms.Label
        Me.TextBox20 = New System.Windows.Forms.TextBox
        Me.TextBox21 = New System.Windows.Forms.TextBox
        Me.Label26 = New System.Windows.Forms.Label
        Me.BsFridgeAdjustTitle = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsFridgeInfoPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsLabel2 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsButtonsPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.BsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsSaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsSaveButtonNOUSED = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAdjustButtonNOUSED = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsMessagesPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
        Me.ProgressBar1 = New Biosystems.Ax00.Controls.UserControls.BSProgressBar
        Me.BsMessageImage = New Biosystems.Ax00.Controls.UserControls.BSPictureBox
        Me.BsMessageLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.CurrentOperationTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SimulationTimer = New Biosystems.Ax00.Controls.UserControls.BSTimer
        Me.BsTabPagesControl.SuspendLayout()
        Me.TabWSHeater.SuspendLayout()
        Me.BsWSHeaterInfoPanel.SuspendLayout()
        Me.BsWSHeaterAdjustPanel.SuspendLayout()
        Me.BSOpticWSGroupBox.SuspendLayout()
        Me.Tab3AdjustGroupBox.SuspendLayout()
        CType(Me.Tab3CorrectionTextBox.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Tab3MeasurementGroupBox.SuspendLayout()
        CType(Me.Tab3TextBoxTemp.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Tab3ConditioningGroupBox.SuspendLayout()
        Me.TabReactionsRotor.SuspendLayout()
        Me.BsReactionsRotorInfoPanel.SuspendLayout()
        Me.BsReactionsRotorAdjustPanel.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.Tab1AdjustGroupBox.SuspendLayout()
        CType(Me.Tab1CorrectionTextBox.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Tab1ConditioningGroupBox.SuspendLayout()
        Me.Tab1MeasurementGroupBox.SuspendLayout()
        CType(Me.Tab1TextBoxTemp1.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Tab1TextBoxTemp2.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Tab1TextBoxTemp3.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Tab1TextBoxTemp4.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabReagentsNeedles.SuspendLayout()
        Me.BsReagentsNeedlesInfoPanel.SuspendLayout()
        Me.BsReagentsNeedlesAdjustPanel.SuspendLayout()
        Me.Tab2AdjustGroupBox.SuspendLayout()
        CType(Me.Tab2CorrectionTextBox.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Tab2MeasurementGroupBox.SuspendLayout()
        CType(Me.Tab2TextBoxTemp.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Tab2ConditioningGroupBox.SuspendLayout()
        Me.Tab2SelectArmGroupBox.SuspendLayout()
        Me.TabFridge.SuspendLayout()
        Me.BsFridgeAdjustmentPanel.SuspendLayout()
        Me.BsRotorPanel.SuspendLayout()
        Me.BsAnalyzerTempGroupBox.SuspendLayout()
        Me.BSRotorGroupBox.SuspendLayout()
        CType(Me.FridgeTempChart, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(SplineSeriesView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RotorChartGroupBox.SuspendLayout()
        Me.BsFridgeInfoPanel.SuspendLayout()
        Me.BsButtonsPanel.SuspendLayout()
        Me.BsMessagesPanel.SuspendLayout()
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsTabPagesControl
        '
        Me.BsTabPagesControl.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTabPagesControl.Controls.Add(Me.TabWSHeater)
        Me.BsTabPagesControl.Controls.Add(Me.TabReactionsRotor)
        Me.BsTabPagesControl.Controls.Add(Me.TabReagentsNeedles)
        Me.BsTabPagesControl.Controls.Add(Me.TabFridge)
        Me.BsTabPagesControl.Location = New System.Drawing.Point(0, 0)
        Me.BsTabPagesControl.Name = "BsTabPagesControl"
        Me.BsTabPagesControl.SelectedIndex = 0
        Me.BsTabPagesControl.Size = New System.Drawing.Size(978, 558)
        Me.BsTabPagesControl.TabIndex = 15
        '
        'TabWSHeater
        '
        Me.TabWSHeater.Controls.Add(Me.BsWSHeaterInfoPanel)
        Me.TabWSHeater.Controls.Add(Me.BsWSHeaterAdjustPanel)
        Me.TabWSHeater.Location = New System.Drawing.Point(4, 22)
        Me.TabWSHeater.Name = "TabWSHeater"
        Me.TabWSHeater.Size = New System.Drawing.Size(970, 532)
        Me.TabWSHeater.TabIndex = 2
        Me.TabWSHeater.Text = "Washing Station Heater"
        Me.TabWSHeater.UseVisualStyleBackColor = True
        '
        'BsWSHeaterInfoPanel
        '
        Me.BsWSHeaterInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsWSHeaterInfoPanel.Controls.Add(Me.BsInfoWsHeaterXPSViewer)
        Me.BsWSHeaterInfoPanel.Controls.Add(Me.BsWSHeaterInfoTitle)
        Me.BsWSHeaterInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsWSHeaterInfoPanel.Name = "BsWSHeaterInfoPanel"
        Me.BsWSHeaterInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsWSHeaterInfoPanel.TabIndex = 26
        '
        'BsInfoWsHeaterXPSViewer
        '
        Me.BsInfoWsHeaterXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoWsHeaterXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoWsHeaterXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoWsHeaterXPSViewer.CopyButtonVisible = True
        Me.BsInfoWsHeaterXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoWsHeaterXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoWsHeaterXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoWsHeaterXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoWsHeaterXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoWsHeaterXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoWsHeaterXPSViewer.HorizontalPageMargin = 10
        Me.BsInfoWsHeaterXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoWsHeaterXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoWsHeaterXPSViewer.IsScrollable = False
        Me.BsInfoWsHeaterXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoWsHeaterXPSViewer.MenuBarVisible = False
        Me.BsInfoWsHeaterXPSViewer.Name = "BsInfoWsHeaterXPSViewer"
        Me.BsInfoWsHeaterXPSViewer.PopupMenuEnabled = True
        Me.BsInfoWsHeaterXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoWsHeaterXPSViewer.PrintButtonVisible = True
        Me.BsInfoWsHeaterXPSViewer.SearchBarVisible = False
        Me.BsInfoWsHeaterXPSViewer.Size = New System.Drawing.Size(230, 510)
        Me.BsInfoWsHeaterXPSViewer.TabIndex = 35
        Me.BsInfoWsHeaterXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoWsHeaterXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoWsHeaterXPSViewer.VerticalPageMargin = 10
        Me.BsInfoWsHeaterXPSViewer.Visible = False
        Me.BsInfoWsHeaterXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoWsHeaterXPSViewer.WholePageButtonVisible = True
        '
        'BsWSHeaterInfoTitle
        '
        Me.BsWSHeaterInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsWSHeaterInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsWSHeaterInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsWSHeaterInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsWSHeaterInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsWSHeaterInfoTitle.Name = "BsWSHeaterInfoTitle"
        Me.BsWSHeaterInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsWSHeaterInfoTitle.TabIndex = 25
        Me.BsWSHeaterInfoTitle.Text = "Information"
        Me.BsWSHeaterInfoTitle.Title = True
        '
        'BsWSHeaterAdjustPanel
        '
        Me.BsWSHeaterAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsWSHeaterAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsWSHeaterAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsWSHeaterAdjustPanel.Controls.Add(Me.BSOpticWSGroupBox)
        Me.BsWSHeaterAdjustPanel.Controls.Add(Me.Tab3AdjustGroupBox)
        Me.BsWSHeaterAdjustPanel.Controls.Add(Me.Tab3MeasurementGroupBox)
        Me.BsWSHeaterAdjustPanel.Controls.Add(Me.Tab3ConditioningGroupBox)
        Me.BsWSHeaterAdjustPanel.Controls.Add(Me.BsWSHeaterAdjustTitle)
        Me.BsWSHeaterAdjustPanel.Location = New System.Drawing.Point(231, 0)
        Me.BsWSHeaterAdjustPanel.Name = "BsWSHeaterAdjustPanel"
        Me.BsWSHeaterAdjustPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsWSHeaterAdjustPanel.TabIndex = 25
        '
        'BSOpticWSGroupBox
        '
        Me.BSOpticWSGroupBox.Controls.Add(Me.Tab3HeaterWSLabel)
        Me.BSOpticWSGroupBox.Controls.Add(Me.BsButton4)
        Me.BSOpticWSGroupBox.Controls.Add(Me.BsUpDownWSButton1)
        Me.BSOpticWSGroupBox.Location = New System.Drawing.Point(7, 22)
        Me.BSOpticWSGroupBox.Name = "BSOpticWSGroupBox"
        Me.BSOpticWSGroupBox.Size = New System.Drawing.Size(722, 45)
        Me.BSOpticWSGroupBox.TabIndex = 76
        Me.BSOpticWSGroupBox.TabStop = False
        '
        'Tab3HeaterWSLabel
        '
        Me.Tab3HeaterWSLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab3HeaterWSLabel.Location = New System.Drawing.Point(6, 18)
        Me.Tab3HeaterWSLabel.Name = "Tab3HeaterWSLabel"
        Me.Tab3HeaterWSLabel.Size = New System.Drawing.Size(679, 19)
        Me.Tab3HeaterWSLabel.TabIndex = 108
        Me.Tab3HeaterWSLabel.Text = "Place the Reactions Rotor"
        Me.Tab3HeaterWSLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BsButton4
        '
        Me.BsButton4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton4.Location = New System.Drawing.Point(687, 228)
        Me.BsButton4.Name = "BsButton4"
        Me.BsButton4.Size = New System.Drawing.Size(30, 30)
        Me.BsButton4.TabIndex = 106
        Me.BsButton4.UseVisualStyleBackColor = True
        '
        'BsUpDownWSButton1
        '
        Me.BsUpDownWSButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsUpDownWSButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsUpDownWSButton1.Location = New System.Drawing.Point(686, 10)
        Me.BsUpDownWSButton1.Name = "BsUpDownWSButton1"
        Me.BsUpDownWSButton1.Size = New System.Drawing.Size(32, 32)
        Me.BsUpDownWSButton1.TabIndex = 45
        Me.BsUpDownWSButton1.UseVisualStyleBackColor = True
        '
        'Tab3AdjustGroupBox
        '
        Me.Tab3AdjustGroupBox.Controls.Add(Me.Tab3CorrectionTextBox)
        Me.Tab3AdjustGroupBox.Controls.Add(Me.Tab3AdjustProposedLabel)
        Me.Tab3AdjustGroupBox.Controls.Add(Me.Tab3AdjustButton)
        Me.Tab3AdjustGroupBox.Location = New System.Drawing.Point(7, 231)
        Me.Tab3AdjustGroupBox.Name = "Tab3AdjustGroupBox"
        Me.Tab3AdjustGroupBox.Size = New System.Drawing.Size(723, 85)
        Me.Tab3AdjustGroupBox.TabIndex = 74
        Me.Tab3AdjustGroupBox.TabStop = False
        Me.Tab3AdjustGroupBox.Text = "3. Adjustment"
        '
        'Tab3CorrectionTextBox
        '
        Me.Tab3CorrectionTextBox.Location = New System.Drawing.Point(17, 34)
        Me.Tab3CorrectionTextBox.Name = "Tab3CorrectionTextBox"
        Me.Tab3CorrectionTextBox.Properties.Appearance.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Tab3CorrectionTextBox.Properties.Appearance.ForeColor = System.Drawing.Color.Red
        Me.Tab3CorrectionTextBox.Properties.Appearance.Options.UseFont = True
        Me.Tab3CorrectionTextBox.Properties.Appearance.Options.UseForeColor = True
        Me.Tab3CorrectionTextBox.Properties.Mask.EditMask = "n1"
        Me.Tab3CorrectionTextBox.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.Tab3CorrectionTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Tab3CorrectionTextBox.Size = New System.Drawing.Size(124, 39)
        Me.Tab3CorrectionTextBox.TabIndex = 119
        '
        'Tab3AdjustProposedLabel
        '
        Me.Tab3AdjustProposedLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab3AdjustProposedLabel.Location = New System.Drawing.Point(147, 36)
        Me.Tab3AdjustProposedLabel.Name = "Tab3AdjustProposedLabel"
        Me.Tab3AdjustProposedLabel.Size = New System.Drawing.Size(491, 36)
        Me.Tab3AdjustProposedLabel.TabIndex = 106
        Me.Tab3AdjustProposedLabel.Text = "Adjust Proposed"
        Me.Tab3AdjustProposedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Tab3AdjustButton
        '
        Me.Tab3AdjustButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tab3AdjustButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Tab3AdjustButton.Location = New System.Drawing.Point(687, 48)
        Me.Tab3AdjustButton.Name = "Tab3AdjustButton"
        Me.Tab3AdjustButton.Size = New System.Drawing.Size(32, 32)
        Me.Tab3AdjustButton.TabIndex = 40
        Me.Tab3AdjustButton.UseVisualStyleBackColor = True
        '
        'Tab3MeasurementGroupBox
        '
        Me.Tab3MeasurementGroupBox.Controls.Add(Me.Tab3TextBoxTemp)
        Me.Tab3MeasurementGroupBox.Controls.Add(Me.Tab3TempMeasuredLabel)
        Me.Tab3MeasurementGroupBox.Location = New System.Drawing.Point(7, 140)
        Me.Tab3MeasurementGroupBox.Name = "Tab3MeasurementGroupBox"
        Me.Tab3MeasurementGroupBox.Size = New System.Drawing.Size(723, 85)
        Me.Tab3MeasurementGroupBox.TabIndex = 73
        Me.Tab3MeasurementGroupBox.TabStop = False
        Me.Tab3MeasurementGroupBox.Text = "2. Temperature Measurement"
        '
        'Tab3TextBoxTemp
        '
        Me.Tab3TextBoxTemp.Location = New System.Drawing.Point(17, 29)
        Me.Tab3TextBoxTemp.Name = "Tab3TextBoxTemp"
        Me.Tab3TextBoxTemp.Properties.Appearance.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Tab3TextBoxTemp.Properties.Appearance.Options.UseFont = True
        Me.Tab3TextBoxTemp.Properties.Mask.EditMask = "n1"
        Me.Tab3TextBoxTemp.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.Tab3TextBoxTemp.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Tab3TextBoxTemp.Size = New System.Drawing.Size(124, 39)
        Me.Tab3TextBoxTemp.TabIndex = 118
        '
        'Tab3TempMeasuredLabel
        '
        Me.Tab3TempMeasuredLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab3TempMeasuredLabel.Location = New System.Drawing.Point(147, 30)
        Me.Tab3TempMeasuredLabel.Name = "Tab3TempMeasuredLabel"
        Me.Tab3TempMeasuredLabel.Size = New System.Drawing.Size(491, 35)
        Me.Tab3TempMeasuredLabel.TabIndex = 106
        Me.Tab3TempMeasuredLabel.Text = "Measured"
        Me.Tab3TempMeasuredLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Tab3ConditioningGroupBox
        '
        Me.Tab3ConditioningGroupBox.Controls.Add(Me.Tab3ConditioningButton)
        Me.Tab3ConditioningGroupBox.Controls.Add(Me.Tab3ConditioningDescLabel)
        Me.Tab3ConditioningGroupBox.Controls.Add(Me.BsButton6)
        Me.Tab3ConditioningGroupBox.Location = New System.Drawing.Point(6, 74)
        Me.Tab3ConditioningGroupBox.Name = "Tab3ConditioningGroupBox"
        Me.Tab3ConditioningGroupBox.Size = New System.Drawing.Size(723, 60)
        Me.Tab3ConditioningGroupBox.TabIndex = 72
        Me.Tab3ConditioningGroupBox.TabStop = False
        Me.Tab3ConditioningGroupBox.Text = "1. Conditioning"
        '
        'Tab3ConditioningButton
        '
        Me.Tab3ConditioningButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tab3ConditioningButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Tab3ConditioningButton.Location = New System.Drawing.Point(687, 23)
        Me.Tab3ConditioningButton.Name = "Tab3ConditioningButton"
        Me.Tab3ConditioningButton.Size = New System.Drawing.Size(32, 32)
        Me.Tab3ConditioningButton.TabIndex = 109
        Me.Tab3ConditioningButton.UseVisualStyleBackColor = True
        '
        'Tab3ConditioningDescLabel
        '
        Me.Tab3ConditioningDescLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab3ConditioningDescLabel.Location = New System.Drawing.Point(12, 20)
        Me.Tab3ConditioningDescLabel.Name = "Tab3ConditioningDescLabel"
        Me.Tab3ConditioningDescLabel.Size = New System.Drawing.Size(639, 30)
        Me.Tab3ConditioningDescLabel.TabIndex = 108
        Me.Tab3ConditioningDescLabel.Text = "Remember put the tool into correct position to  proceed to conditioning the syste" & _
            "m ..."
        Me.Tab3ConditioningDescLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BsButton6
        '
        Me.BsButton6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton6.Location = New System.Drawing.Point(688, 228)
        Me.BsButton6.Name = "BsButton6"
        Me.BsButton6.Size = New System.Drawing.Size(30, 30)
        Me.BsButton6.TabIndex = 106
        Me.BsButton6.UseVisualStyleBackColor = True
        '
        'BsWSHeaterAdjustTitle
        '
        Me.BsWSHeaterAdjustTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsWSHeaterAdjustTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsWSHeaterAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.BsWSHeaterAdjustTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsWSHeaterAdjustTitle.Name = "BsWSHeaterAdjustTitle"
        Me.BsWSHeaterAdjustTitle.Size = New System.Drawing.Size(739, 20)
        Me.BsWSHeaterAdjustTitle.TabIndex = 32
        Me.BsWSHeaterAdjustTitle.Text = "Washing Station Heater Adjustment/Test"
        Me.BsWSHeaterAdjustTitle.Title = True
        '
        'TabReactionsRotor
        '
        Me.TabReactionsRotor.Controls.Add(Me.BsReactionsRotorInfoPanel)
        Me.TabReactionsRotor.Controls.Add(Me.BsReactionsRotorAdjustPanel)
        Me.TabReactionsRotor.Location = New System.Drawing.Point(4, 22)
        Me.TabReactionsRotor.Name = "TabReactionsRotor"
        Me.TabReactionsRotor.Padding = New System.Windows.Forms.Padding(3)
        Me.TabReactionsRotor.Size = New System.Drawing.Size(970, 532)
        Me.TabReactionsRotor.TabIndex = 0
        Me.TabReactionsRotor.Text = "Reactions Rotor"
        Me.TabReactionsRotor.UseVisualStyleBackColor = True
        '
        'BsReactionsRotorInfoPanel
        '
        Me.BsReactionsRotorInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsReactionsRotorInfoPanel.Controls.Add(Me.BsInfoRotorXPSViewer)
        Me.BsReactionsRotorInfoPanel.Controls.Add(Me.BsReactionsRotorInfoTitle)
        Me.BsReactionsRotorInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsReactionsRotorInfoPanel.Name = "BsReactionsRotorInfoPanel"
        Me.BsReactionsRotorInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsReactionsRotorInfoPanel.TabIndex = 25
        '
        'BsInfoRotorXPSViewer
        '
        Me.BsInfoRotorXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoRotorXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoRotorXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoRotorXPSViewer.CopyButtonVisible = True
        Me.BsInfoRotorXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoRotorXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoRotorXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoRotorXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoRotorXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoRotorXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoRotorXPSViewer.HorizontalPageMargin = 10
        Me.BsInfoRotorXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoRotorXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoRotorXPSViewer.IsScrollable = False
        Me.BsInfoRotorXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoRotorXPSViewer.MenuBarVisible = False
        Me.BsInfoRotorXPSViewer.Name = "BsInfoRotorXPSViewer"
        Me.BsInfoRotorXPSViewer.PopupMenuEnabled = True
        Me.BsInfoRotorXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoRotorXPSViewer.PrintButtonVisible = True
        Me.BsInfoRotorXPSViewer.SearchBarVisible = False
        Me.BsInfoRotorXPSViewer.Size = New System.Drawing.Size(230, 510)
        Me.BsInfoRotorXPSViewer.TabIndex = 35
        Me.BsInfoRotorXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoRotorXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoRotorXPSViewer.VerticalPageMargin = 10
        Me.BsInfoRotorXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoRotorXPSViewer.WholePageButtonVisible = True
        '
        'BsReactionsRotorInfoTitle
        '
        Me.BsReactionsRotorInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsReactionsRotorInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsReactionsRotorInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsReactionsRotorInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsReactionsRotorInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsReactionsRotorInfoTitle.Name = "BsReactionsRotorInfoTitle"
        Me.BsReactionsRotorInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsReactionsRotorInfoTitle.TabIndex = 22
        Me.BsReactionsRotorInfoTitle.Text = "Information"
        Me.BsReactionsRotorInfoTitle.Title = True
        '
        'BsReactionsRotorAdjustPanel
        '
        Me.BsReactionsRotorAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsReactionsRotorAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsReactionsRotorAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsReactionsRotorAdjustPanel.Controls.Add(Me.GroupBox1)
        Me.BsReactionsRotorAdjustPanel.Controls.Add(Me.Tab1AdjustGroupBox)
        Me.BsReactionsRotorAdjustPanel.Controls.Add(Me.Tab1ConditioningGroupBox)
        Me.BsReactionsRotorAdjustPanel.Controls.Add(Me.Tab1MeasurementGroupBox)
        Me.BsReactionsRotorAdjustPanel.Controls.Add(Me.ReactionsRotorAdjustTitle)
        Me.BsReactionsRotorAdjustPanel.Location = New System.Drawing.Point(231, 0)
        Me.BsReactionsRotorAdjustPanel.Name = "BsReactionsRotorAdjustPanel"
        Me.BsReactionsRotorAdjustPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsReactionsRotorAdjustPanel.TabIndex = 21
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Tab1RotorReactLabel)
        Me.GroupBox1.Controls.Add(Me.BsButton1)
        Me.GroupBox1.Controls.Add(Me.BsUpDownWSButton2)
        Me.GroupBox1.Location = New System.Drawing.Point(7, 22)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(722, 45)
        Me.GroupBox1.TabIndex = 77
        Me.GroupBox1.TabStop = False
        '
        'Tab1RotorReactLabel
        '
        Me.Tab1RotorReactLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab1RotorReactLabel.Location = New System.Drawing.Point(6, 18)
        Me.Tab1RotorReactLabel.Name = "Tab1RotorReactLabel"
        Me.Tab1RotorReactLabel.Size = New System.Drawing.Size(679, 19)
        Me.Tab1RotorReactLabel.TabIndex = 108
        Me.Tab1RotorReactLabel.Text = "Place the Reactions Rotor"
        Me.Tab1RotorReactLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BsButton1
        '
        Me.BsButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton1.Location = New System.Drawing.Point(687, 228)
        Me.BsButton1.Name = "BsButton1"
        Me.BsButton1.Size = New System.Drawing.Size(30, 30)
        Me.BsButton1.TabIndex = 106
        Me.BsButton1.UseVisualStyleBackColor = True
        '
        'BsUpDownWSButton2
        '
        Me.BsUpDownWSButton2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsUpDownWSButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsUpDownWSButton2.Location = New System.Drawing.Point(686, 10)
        Me.BsUpDownWSButton2.Name = "BsUpDownWSButton2"
        Me.BsUpDownWSButton2.Size = New System.Drawing.Size(32, 32)
        Me.BsUpDownWSButton2.TabIndex = 45
        Me.BsUpDownWSButton2.UseVisualStyleBackColor = True
        '
        'Tab1AdjustGroupBox
        '
        Me.Tab1AdjustGroupBox.Controls.Add(Me.Tab1CorrectionTextBox)
        Me.Tab1AdjustGroupBox.Controls.Add(Me.Tab1AdjustButton)
        Me.Tab1AdjustGroupBox.Controls.Add(Me.Tab1AdjustProposedLabel)
        Me.Tab1AdjustGroupBox.Location = New System.Drawing.Point(7, 231)
        Me.Tab1AdjustGroupBox.Name = "Tab1AdjustGroupBox"
        Me.Tab1AdjustGroupBox.Size = New System.Drawing.Size(723, 85)
        Me.Tab1AdjustGroupBox.TabIndex = 69
        Me.Tab1AdjustGroupBox.TabStop = False
        Me.Tab1AdjustGroupBox.Text = "3. Adjustment"
        '
        'Tab1CorrectionTextBox
        '
        Me.Tab1CorrectionTextBox.Location = New System.Drawing.Point(17, 34)
        Me.Tab1CorrectionTextBox.Name = "Tab1CorrectionTextBox"
        Me.Tab1CorrectionTextBox.Properties.Appearance.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Tab1CorrectionTextBox.Properties.Appearance.ForeColor = System.Drawing.Color.Red
        Me.Tab1CorrectionTextBox.Properties.Appearance.Options.UseFont = True
        Me.Tab1CorrectionTextBox.Properties.Appearance.Options.UseForeColor = True
        Me.Tab1CorrectionTextBox.Properties.Mask.EditMask = "n1"
        Me.Tab1CorrectionTextBox.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.Tab1CorrectionTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Tab1CorrectionTextBox.Size = New System.Drawing.Size(124, 39)
        Me.Tab1CorrectionTextBox.TabIndex = 124
        '
        'Tab1AdjustButton
        '
        Me.Tab1AdjustButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tab1AdjustButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Tab1AdjustButton.Location = New System.Drawing.Point(686, 48)
        Me.Tab1AdjustButton.Name = "Tab1AdjustButton"
        Me.Tab1AdjustButton.Size = New System.Drawing.Size(32, 32)
        Me.Tab1AdjustButton.TabIndex = 14
        Me.Tab1AdjustButton.UseVisualStyleBackColor = True
        '
        'Tab1AdjustProposedLabel
        '
        Me.Tab1AdjustProposedLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab1AdjustProposedLabel.Location = New System.Drawing.Point(147, 42)
        Me.Tab1AdjustProposedLabel.Name = "Tab1AdjustProposedLabel"
        Me.Tab1AdjustProposedLabel.Size = New System.Drawing.Size(358, 25)
        Me.Tab1AdjustProposedLabel.TabIndex = 57
        Me.Tab1AdjustProposedLabel.Text = "Rotor Thermo Adjust Proposed"
        Me.Tab1AdjustProposedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Tab1ConditioningGroupBox
        '
        Me.Tab1ConditioningGroupBox.Controls.Add(Me.Tab1ConditioningButton)
        Me.Tab1ConditioningGroupBox.Controls.Add(Me.Tab1AutoRadioButton)
        Me.Tab1ConditioningGroupBox.Controls.Add(Me.Tab1ManualRadioButton)
        Me.Tab1ConditioningGroupBox.Location = New System.Drawing.Point(6, 74)
        Me.Tab1ConditioningGroupBox.Name = "Tab1ConditioningGroupBox"
        Me.Tab1ConditioningGroupBox.Size = New System.Drawing.Size(723, 60)
        Me.Tab1ConditioningGroupBox.TabIndex = 67
        Me.Tab1ConditioningGroupBox.TabStop = False
        Me.Tab1ConditioningGroupBox.Text = "1. Conditioning"
        '
        'Tab1ConditioningButton
        '
        Me.Tab1ConditioningButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tab1ConditioningButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Tab1ConditioningButton.Location = New System.Drawing.Point(687, 23)
        Me.Tab1ConditioningButton.Name = "Tab1ConditioningButton"
        Me.Tab1ConditioningButton.Size = New System.Drawing.Size(32, 32)
        Me.Tab1ConditioningButton.TabIndex = 3
        Me.Tab1ConditioningButton.UseVisualStyleBackColor = True
        '
        'Tab1AutoRadioButton
        '
        Me.Tab1AutoRadioButton.AutoSize = True
        Me.Tab1AutoRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab1AutoRadioButton.Location = New System.Drawing.Point(24, 27)
        Me.Tab1AutoRadioButton.Name = "Tab1AutoRadioButton"
        Me.Tab1AutoRadioButton.Size = New System.Drawing.Size(118, 17)
        Me.Tab1AutoRadioButton.TabIndex = 1
        Me.Tab1AutoRadioButton.TabStop = True
        Me.Tab1AutoRadioButton.Text = "Automatic Filling"
        Me.Tab1AutoRadioButton.UseVisualStyleBackColor = True
        '
        'Tab1ManualRadioButton
        '
        Me.Tab1ManualRadioButton.AutoSize = True
        Me.Tab1ManualRadioButton.Checked = True
        Me.Tab1ManualRadioButton.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab1ManualRadioButton.ForeColor = System.Drawing.Color.Black
        Me.Tab1ManualRadioButton.Location = New System.Drawing.Point(262, 27)
        Me.Tab1ManualRadioButton.Name = "Tab1ManualRadioButton"
        Me.Tab1ManualRadioButton.Size = New System.Drawing.Size(101, 17)
        Me.Tab1ManualRadioButton.TabIndex = 2
        Me.Tab1ManualRadioButton.TabStop = True
        Me.Tab1ManualRadioButton.Text = "Manual Filling"
        Me.Tab1ManualRadioButton.UseVisualStyleBackColor = True
        '
        'Tab1MeasurementGroupBox
        '
        Me.Tab1MeasurementGroupBox.Controls.Add(Me.Tab1TextBoxTemp1)
        Me.Tab1MeasurementGroupBox.Controls.Add(Me.Tab1TextBoxTemp2)
        Me.Tab1MeasurementGroupBox.Controls.Add(Me.Tab1TextBoxTemp3)
        Me.Tab1MeasurementGroupBox.Controls.Add(Me.Tab1TextBoxTemp4)
        Me.Tab1MeasurementGroupBox.Controls.Add(Me.Tab1MeanLabel)
        Me.Tab1MeasurementGroupBox.Controls.Add(Me.TabMeanTempLabel)
        Me.Tab1MeasurementGroupBox.Controls.Add(Me.Label4)
        Me.Tab1MeasurementGroupBox.Controls.Add(Me.Label9)
        Me.Tab1MeasurementGroupBox.Controls.Add(Me.Label5)
        Me.Tab1MeasurementGroupBox.Controls.Add(Me.Label7)
        Me.Tab1MeasurementGroupBox.Location = New System.Drawing.Point(7, 140)
        Me.Tab1MeasurementGroupBox.Name = "Tab1MeasurementGroupBox"
        Me.Tab1MeasurementGroupBox.Size = New System.Drawing.Size(723, 85)
        Me.Tab1MeasurementGroupBox.TabIndex = 68
        Me.Tab1MeasurementGroupBox.TabStop = False
        Me.Tab1MeasurementGroupBox.Text = "2. Temperature Measurement"
        '
        'Tab1TextBoxTemp1
        '
        Me.Tab1TextBoxTemp1.Location = New System.Drawing.Point(17, 34)
        Me.Tab1TextBoxTemp1.Name = "Tab1TextBoxTemp1"
        Me.Tab1TextBoxTemp1.Properties.Appearance.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Tab1TextBoxTemp1.Properties.Appearance.Options.UseFont = True
        Me.Tab1TextBoxTemp1.Properties.Mask.EditMask = "n1"
        Me.Tab1TextBoxTemp1.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.Tab1TextBoxTemp1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Tab1TextBoxTemp1.Size = New System.Drawing.Size(80, 39)
        Me.Tab1TextBoxTemp1.TabIndex = 127
        '
        'Tab1TextBoxTemp2
        '
        Me.Tab1TextBoxTemp2.Location = New System.Drawing.Point(117, 34)
        Me.Tab1TextBoxTemp2.Name = "Tab1TextBoxTemp2"
        Me.Tab1TextBoxTemp2.Properties.Appearance.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Tab1TextBoxTemp2.Properties.Appearance.Options.UseFont = True
        Me.Tab1TextBoxTemp2.Properties.Mask.EditMask = "n1"
        Me.Tab1TextBoxTemp2.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.Tab1TextBoxTemp2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Tab1TextBoxTemp2.Size = New System.Drawing.Size(80, 39)
        Me.Tab1TextBoxTemp2.TabIndex = 126
        '
        'Tab1TextBoxTemp3
        '
        Me.Tab1TextBoxTemp3.Location = New System.Drawing.Point(217, 34)
        Me.Tab1TextBoxTemp3.Name = "Tab1TextBoxTemp3"
        Me.Tab1TextBoxTemp3.Properties.Appearance.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Tab1TextBoxTemp3.Properties.Appearance.Options.UseFont = True
        Me.Tab1TextBoxTemp3.Properties.Mask.EditMask = "n1"
        Me.Tab1TextBoxTemp3.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.Tab1TextBoxTemp3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Tab1TextBoxTemp3.Size = New System.Drawing.Size(80, 39)
        Me.Tab1TextBoxTemp3.TabIndex = 125
        '
        'Tab1TextBoxTemp4
        '
        Me.Tab1TextBoxTemp4.Location = New System.Drawing.Point(317, 34)
        Me.Tab1TextBoxTemp4.Name = "Tab1TextBoxTemp4"
        Me.Tab1TextBoxTemp4.Properties.Appearance.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Tab1TextBoxTemp4.Properties.Appearance.Options.UseFont = True
        Me.Tab1TextBoxTemp4.Properties.Mask.EditMask = "n1"
        Me.Tab1TextBoxTemp4.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.Tab1TextBoxTemp4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Tab1TextBoxTemp4.Size = New System.Drawing.Size(80, 39)
        Me.Tab1TextBoxTemp4.TabIndex = 124
        '
        'Tab1MeanLabel
        '
        Me.Tab1MeanLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab1MeanLabel.Location = New System.Drawing.Point(609, 43)
        Me.Tab1MeanLabel.Name = "Tab1MeanLabel"
        Me.Tab1MeanLabel.Size = New System.Drawing.Size(103, 25)
        Me.Tab1MeanLabel.TabIndex = 105
        Me.Tab1MeanLabel.Text = "Mean"
        Me.Tab1MeanLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TabMeanTempLabel
        '
        Me.TabMeanTempLabel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.TabMeanTempLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TabMeanTempLabel.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.TabMeanTempLabel.ForeColor = System.Drawing.Color.SteelBlue
        Me.TabMeanTempLabel.Location = New System.Drawing.Point(479, 34)
        Me.TabMeanTempLabel.Name = "TabMeanTempLabel"
        Me.TabMeanTempLabel.Size = New System.Drawing.Size(124, 39)
        Me.TabMeanTempLabel.TabIndex = 104
        Me.TabMeanTempLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(251, 15)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(18, 20)
        Me.Label4.TabIndex = 103
        Me.Label4.Text = "3"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label9
        '
        Me.Label9.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(151, 15)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(18, 20)
        Me.Label9.TabIndex = 97
        Me.Label9.Text = "2"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(53, 15)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(18, 20)
        Me.Label5.TabIndex = 88
        Me.Label5.Text = "1"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(350, 15)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(18, 20)
        Me.Label7.TabIndex = 102
        Me.Label7.Text = "4"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ReactionsRotorAdjustTitle
        '
        Me.ReactionsRotorAdjustTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.ReactionsRotorAdjustTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.ReactionsRotorAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.ReactionsRotorAdjustTitle.Location = New System.Drawing.Point(0, 0)
        Me.ReactionsRotorAdjustTitle.Name = "ReactionsRotorAdjustTitle"
        Me.ReactionsRotorAdjustTitle.Size = New System.Drawing.Size(738, 20)
        Me.ReactionsRotorAdjustTitle.TabIndex = 31
        Me.ReactionsRotorAdjustTitle.Text = "Reactions Rotor Adjustment/Test"
        Me.ReactionsRotorAdjustTitle.Title = True
        '
        'TabReagentsNeedles
        '
        Me.TabReagentsNeedles.Controls.Add(Me.BsReagentsNeedlesInfoPanel)
        Me.TabReagentsNeedles.Controls.Add(Me.BsReagentsNeedlesAdjustPanel)
        Me.TabReagentsNeedles.Location = New System.Drawing.Point(4, 22)
        Me.TabReagentsNeedles.Name = "TabReagentsNeedles"
        Me.TabReagentsNeedles.Padding = New System.Windows.Forms.Padding(3)
        Me.TabReagentsNeedles.Size = New System.Drawing.Size(970, 532)
        Me.TabReagentsNeedles.TabIndex = 1
        Me.TabReagentsNeedles.Text = "Reagents Needles"
        Me.TabReagentsNeedles.UseVisualStyleBackColor = True
        '
        'BsReagentsNeedlesInfoPanel
        '
        Me.BsReagentsNeedlesInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsReagentsNeedlesInfoPanel.Controls.Add(Me.BsInfoNeedlesXPSViewer)
        Me.BsReagentsNeedlesInfoPanel.Controls.Add(Me.BsReagentsNeedlesInfoTitle)
        Me.BsReagentsNeedlesInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsReagentsNeedlesInfoPanel.Name = "BsReagentsNeedlesInfoPanel"
        Me.BsReagentsNeedlesInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsReagentsNeedlesInfoPanel.TabIndex = 23
        '
        'BsInfoNeedlesXPSViewer
        '
        Me.BsInfoNeedlesXPSViewer.ActualZoomButtonCaption = "Actual Zoom"
        Me.BsInfoNeedlesXPSViewer.ActualZoomButtonVisible = True
        Me.BsInfoNeedlesXPSViewer.CopyButtonCaption = "Copy"
        Me.BsInfoNeedlesXPSViewer.CopyButtonVisible = True
        Me.BsInfoNeedlesXPSViewer.DecreaseZoomButtonCaption = "Zoom Out"
        Me.BsInfoNeedlesXPSViewer.DecreaseZoomButtonVisible = True
        Me.BsInfoNeedlesXPSViewer.FitToHeightButtonCaption = "Fit To Height"
        Me.BsInfoNeedlesXPSViewer.FitToHeightButtonVisible = True
        Me.BsInfoNeedlesXPSViewer.FitToWidthButtonCaption = "Fit To Width"
        Me.BsInfoNeedlesXPSViewer.FitToWidthButtonVisible = True
        Me.BsInfoNeedlesXPSViewer.HorizontalPageMargin = 10
        Me.BsInfoNeedlesXPSViewer.IncreaseZoomButtonCaption = "Zoom In"
        Me.BsInfoNeedlesXPSViewer.IncreaseZoomButtonVisible = True
        Me.BsInfoNeedlesXPSViewer.IsScrollable = False
        Me.BsInfoNeedlesXPSViewer.Location = New System.Drawing.Point(0, 22)
        Me.BsInfoNeedlesXPSViewer.MenuBarVisible = False
        Me.BsInfoNeedlesXPSViewer.Name = "BsInfoNeedlesXPSViewer"
        Me.BsInfoNeedlesXPSViewer.PopupMenuEnabled = True
        Me.BsInfoNeedlesXPSViewer.PrintButtonCaption = "Print"
        Me.BsInfoNeedlesXPSViewer.PrintButtonVisible = True
        Me.BsInfoNeedlesXPSViewer.SearchBarVisible = False
        Me.BsInfoNeedlesXPSViewer.Size = New System.Drawing.Size(230, 510)
        Me.BsInfoNeedlesXPSViewer.TabIndex = 35
        Me.BsInfoNeedlesXPSViewer.TwoPagesButtonCaption = "Two Pages"
        Me.BsInfoNeedlesXPSViewer.TwoPagesButtonVisible = True
        Me.BsInfoNeedlesXPSViewer.VerticalPageMargin = 10
        Me.BsInfoNeedlesXPSViewer.WholePageButtonCaption = "Whole Page"
        Me.BsInfoNeedlesXPSViewer.WholePageButtonVisible = True
        '
        'BsReagentsNeedlesInfoTitle
        '
        Me.BsReagentsNeedlesInfoTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsReagentsNeedlesInfoTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsReagentsNeedlesInfoTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsReagentsNeedlesInfoTitle.ForeColor = System.Drawing.Color.Black
        Me.BsReagentsNeedlesInfoTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsReagentsNeedlesInfoTitle.Name = "BsReagentsNeedlesInfoTitle"
        Me.BsReagentsNeedlesInfoTitle.Size = New System.Drawing.Size(231, 20)
        Me.BsReagentsNeedlesInfoTitle.TabIndex = 29
        Me.BsReagentsNeedlesInfoTitle.Text = "Information"
        Me.BsReagentsNeedlesInfoTitle.Title = True
        '
        'BsReagentsNeedlesAdjustPanel
        '
        Me.BsReagentsNeedlesAdjustPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsReagentsNeedlesAdjustPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsReagentsNeedlesAdjustPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsReagentsNeedlesAdjustPanel.Controls.Add(Me.Tab2AuxButton)
        Me.BsReagentsNeedlesAdjustPanel.Controls.Add(Me.Tab2AdjustGroupBox)
        Me.BsReagentsNeedlesAdjustPanel.Controls.Add(Me.Tab2MeasurementGroupBox)
        Me.BsReagentsNeedlesAdjustPanel.Controls.Add(Me.Tab2ConditioningGroupBox)
        Me.BsReagentsNeedlesAdjustPanel.Controls.Add(Me.Tab2SelectArmGroupBox)
        Me.BsReagentsNeedlesAdjustPanel.Controls.Add(Me.BsReagentsNeedlesAdjustTitle)
        Me.BsReagentsNeedlesAdjustPanel.Location = New System.Drawing.Point(231, 0)
        Me.BsReagentsNeedlesAdjustPanel.Name = "BsReagentsNeedlesAdjustPanel"
        Me.BsReagentsNeedlesAdjustPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsReagentsNeedlesAdjustPanel.TabIndex = 22
        '
        'Tab2AuxButton
        '
        Me.Tab2AuxButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tab2AuxButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Tab2AuxButton.Enabled = False
        Me.Tab2AuxButton.Location = New System.Drawing.Point(692, 493)
        Me.Tab2AuxButton.Name = "Tab2AuxButton"
        Me.Tab2AuxButton.Size = New System.Drawing.Size(32, 32)
        Me.Tab2AuxButton.TabIndex = 71
        Me.Tab2AuxButton.UseVisualStyleBackColor = True
        Me.Tab2AuxButton.Visible = False
        '
        'Tab2AdjustGroupBox
        '
        Me.Tab2AdjustGroupBox.Controls.Add(Me.Tab2CorrectionTextBox)
        Me.Tab2AdjustGroupBox.Controls.Add(Me.Tab2AdjustProposedLabel)
        Me.Tab2AdjustGroupBox.Controls.Add(Me.Tab2AdjustButton)
        Me.Tab2AdjustGroupBox.Location = New System.Drawing.Point(7, 311)
        Me.Tab2AdjustGroupBox.Name = "Tab2AdjustGroupBox"
        Me.Tab2AdjustGroupBox.Size = New System.Drawing.Size(723, 85)
        Me.Tab2AdjustGroupBox.TabIndex = 70
        Me.Tab2AdjustGroupBox.TabStop = False
        Me.Tab2AdjustGroupBox.Text = "4. Adjustment"
        '
        'Tab2CorrectionTextBox
        '
        Me.Tab2CorrectionTextBox.Location = New System.Drawing.Point(17, 34)
        Me.Tab2CorrectionTextBox.Name = "Tab2CorrectionTextBox"
        Me.Tab2CorrectionTextBox.Properties.Appearance.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Tab2CorrectionTextBox.Properties.Appearance.ForeColor = System.Drawing.Color.Red
        Me.Tab2CorrectionTextBox.Properties.Appearance.Options.UseFont = True
        Me.Tab2CorrectionTextBox.Properties.Appearance.Options.UseForeColor = True
        Me.Tab2CorrectionTextBox.Properties.Mask.EditMask = "n1"
        Me.Tab2CorrectionTextBox.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.Tab2CorrectionTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Tab2CorrectionTextBox.Size = New System.Drawing.Size(80, 39)
        Me.Tab2CorrectionTextBox.TabIndex = 128
        '
        'Tab2AdjustProposedLabel
        '
        Me.Tab2AdjustProposedLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab2AdjustProposedLabel.Location = New System.Drawing.Point(111, 37)
        Me.Tab2AdjustProposedLabel.Name = "Tab2AdjustProposedLabel"
        Me.Tab2AdjustProposedLabel.Size = New System.Drawing.Size(519, 32)
        Me.Tab2AdjustProposedLabel.TabIndex = 106
        Me.Tab2AdjustProposedLabel.Text = "Adjust Proposed"
        Me.Tab2AdjustProposedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Tab2AdjustButton
        '
        Me.Tab2AdjustButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tab2AdjustButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Tab2AdjustButton.Location = New System.Drawing.Point(687, 46)
        Me.Tab2AdjustButton.Name = "Tab2AdjustButton"
        Me.Tab2AdjustButton.Size = New System.Drawing.Size(32, 32)
        Me.Tab2AdjustButton.TabIndex = 40
        Me.Tab2AdjustButton.UseVisualStyleBackColor = True
        '
        'Tab2MeasurementGroupBox
        '
        Me.Tab2MeasurementGroupBox.Controls.Add(Me.Tab2TextBoxTemp)
        Me.Tab2MeasurementGroupBox.Controls.Add(Me.Tab2DispensationsCountLabel)
        Me.Tab2MeasurementGroupBox.Controls.Add(Me.Tab2TempMeasuredLabel)
        Me.Tab2MeasurementGroupBox.Controls.Add(Me.Tab2DispensationsCountTitleLabel)
        Me.Tab2MeasurementGroupBox.Controls.Add(Me.Tab2MeasureButton)
        Me.Tab2MeasurementGroupBox.Location = New System.Drawing.Point(7, 198)
        Me.Tab2MeasurementGroupBox.Name = "Tab2MeasurementGroupBox"
        Me.Tab2MeasurementGroupBox.Size = New System.Drawing.Size(723, 107)
        Me.Tab2MeasurementGroupBox.TabIndex = 69
        Me.Tab2MeasurementGroupBox.TabStop = False
        Me.Tab2MeasurementGroupBox.Text = "3. Temperature Measurement"
        '
        'Tab2TextBoxTemp
        '
        Me.Tab2TextBoxTemp.Location = New System.Drawing.Point(17, 40)
        Me.Tab2TextBoxTemp.Name = "Tab2TextBoxTemp"
        Me.Tab2TextBoxTemp.Properties.Appearance.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Tab2TextBoxTemp.Properties.Appearance.Options.UseFont = True
        Me.Tab2TextBoxTemp.Properties.Mask.EditMask = "n1"
        Me.Tab2TextBoxTemp.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.Tab2TextBoxTemp.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Tab2TextBoxTemp.Size = New System.Drawing.Size(80, 39)
        Me.Tab2TextBoxTemp.TabIndex = 128
        '
        'Tab2DispensationsCountLabel
        '
        Me.Tab2DispensationsCountLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tab2DispensationsCountLabel.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab2DispensationsCountLabel.Location = New System.Drawing.Point(682, 12)
        Me.Tab2DispensationsCountLabel.Name = "Tab2DispensationsCountLabel"
        Me.Tab2DispensationsCountLabel.Size = New System.Drawing.Size(31, 22)
        Me.Tab2DispensationsCountLabel.TabIndex = 113
        Me.Tab2DispensationsCountLabel.Text = "0"
        Me.Tab2DispensationsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Tab2DispensationsCountLabel.Visible = False
        '
        'Tab2TempMeasuredLabel
        '
        Me.Tab2TempMeasuredLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab2TempMeasuredLabel.Location = New System.Drawing.Point(111, 32)
        Me.Tab2TempMeasuredLabel.Name = "Tab2TempMeasuredLabel"
        Me.Tab2TempMeasuredLabel.Size = New System.Drawing.Size(519, 55)
        Me.Tab2TempMeasuredLabel.TabIndex = 106
        Me.Tab2TempMeasuredLabel.Text = "Measured"
        Me.Tab2TempMeasuredLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Tab2DispensationsCountTitleLabel
        '
        Me.Tab2DispensationsCountTitleLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tab2DispensationsCountTitleLabel.Location = New System.Drawing.Point(646, 13)
        Me.Tab2DispensationsCountTitleLabel.Name = "Tab2DispensationsCountTitleLabel"
        Me.Tab2DispensationsCountTitleLabel.Size = New System.Drawing.Size(40, 22)
        Me.Tab2DispensationsCountTitleLabel.TabIndex = 112
        Me.Tab2DispensationsCountTitleLabel.Text = "Disp:"
        Me.Tab2DispensationsCountTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Tab2DispensationsCountTitleLabel.Visible = False
        '
        'Tab2MeasureButton
        '
        Me.Tab2MeasureButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tab2MeasureButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Tab2MeasureButton.Location = New System.Drawing.Point(688, 69)
        Me.Tab2MeasureButton.Name = "Tab2MeasureButton"
        Me.Tab2MeasureButton.Size = New System.Drawing.Size(32, 32)
        Me.Tab2MeasureButton.TabIndex = 40
        Me.Tab2MeasureButton.UseVisualStyleBackColor = True
        '
        'Tab2ConditioningGroupBox
        '
        Me.Tab2ConditioningGroupBox.Controls.Add(Me.Tab2ConditioningButton)
        Me.Tab2ConditioningGroupBox.Controls.Add(Me.Tab2ConditioningDescLabel)
        Me.Tab2ConditioningGroupBox.Controls.Add(Me.BsButton9)
        Me.Tab2ConditioningGroupBox.Location = New System.Drawing.Point(7, 89)
        Me.Tab2ConditioningGroupBox.Name = "Tab2ConditioningGroupBox"
        Me.Tab2ConditioningGroupBox.Size = New System.Drawing.Size(723, 102)
        Me.Tab2ConditioningGroupBox.TabIndex = 68
        Me.Tab2ConditioningGroupBox.TabStop = False
        Me.Tab2ConditioningGroupBox.Text = "2. Conditioning"
        '
        'Tab2ConditioningButton
        '
        Me.Tab2ConditioningButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Tab2ConditioningButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Tab2ConditioningButton.Location = New System.Drawing.Point(687, 64)
        Me.Tab2ConditioningButton.Name = "Tab2ConditioningButton"
        Me.Tab2ConditioningButton.Size = New System.Drawing.Size(32, 32)
        Me.Tab2ConditioningButton.TabIndex = 109
        Me.Tab2ConditioningButton.UseVisualStyleBackColor = True
        '
        'Tab2ConditioningDescLabel
        '
        Me.Tab2ConditioningDescLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab2ConditioningDescLabel.Location = New System.Drawing.Point(12, 20)
        Me.Tab2ConditioningDescLabel.Name = "Tab2ConditioningDescLabel"
        Me.Tab2ConditioningDescLabel.Size = New System.Drawing.Size(618, 70)
        Me.Tab2ConditioningDescLabel.TabIndex = 108
        Me.Tab2ConditioningDescLabel.Text = "Remember put the tool into correct position to  proceed to conditioning the syste" & _
            "m ..."
        Me.Tab2ConditioningDescLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BsButton9
        '
        Me.BsButton9.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton9.Location = New System.Drawing.Point(688, 228)
        Me.BsButton9.Name = "BsButton9"
        Me.BsButton9.Size = New System.Drawing.Size(30, 30)
        Me.BsButton9.TabIndex = 106
        Me.BsButton9.UseVisualStyleBackColor = True
        '
        'Tab2SelectArmGroupBox
        '
        Me.Tab2SelectArmGroupBox.Controls.Add(Me.Tab2RadioButtonR1)
        Me.Tab2SelectArmGroupBox.Controls.Add(Me.Tab2RadioButtonR2)
        Me.Tab2SelectArmGroupBox.Location = New System.Drawing.Point(6, 23)
        Me.Tab2SelectArmGroupBox.Name = "Tab2SelectArmGroupBox"
        Me.Tab2SelectArmGroupBox.Size = New System.Drawing.Size(723, 60)
        Me.Tab2SelectArmGroupBox.TabIndex = 67
        Me.Tab2SelectArmGroupBox.TabStop = False
        Me.Tab2SelectArmGroupBox.Text = "1. Select Arm's Needle"
        '
        'Tab2RadioButtonR1
        '
        Me.Tab2RadioButtonR1.AutoSize = True
        Me.Tab2RadioButtonR1.Checked = True
        Me.Tab2RadioButtonR1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab2RadioButtonR1.Location = New System.Drawing.Point(24, 27)
        Me.Tab2RadioButtonR1.Name = "Tab2RadioButtonR1"
        Me.Tab2RadioButtonR1.Size = New System.Drawing.Size(117, 17)
        Me.Tab2RadioButtonR1.TabIndex = 79
        Me.Tab2RadioButtonR1.TabStop = True
        Me.Tab2RadioButtonR1.Text = "Reagents Arm 1"
        Me.Tab2RadioButtonR1.UseVisualStyleBackColor = True
        '
        'Tab2RadioButtonR2
        '
        Me.Tab2RadioButtonR2.AutoSize = True
        Me.Tab2RadioButtonR2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Tab2RadioButtonR2.ForeColor = System.Drawing.Color.Black
        Me.Tab2RadioButtonR2.Location = New System.Drawing.Point(199, 27)
        Me.Tab2RadioButtonR2.Name = "Tab2RadioButtonR2"
        Me.Tab2RadioButtonR2.Size = New System.Drawing.Size(117, 17)
        Me.Tab2RadioButtonR2.TabIndex = 78
        Me.Tab2RadioButtonR2.Text = "Reagents Arm 2"
        Me.Tab2RadioButtonR2.UseVisualStyleBackColor = True
        '
        'BsReagentsNeedlesAdjustTitle
        '
        Me.BsReagentsNeedlesAdjustTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsReagentsNeedlesAdjustTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsReagentsNeedlesAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.BsReagentsNeedlesAdjustTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsReagentsNeedlesAdjustTitle.Name = "BsReagentsNeedlesAdjustTitle"
        Me.BsReagentsNeedlesAdjustTitle.Size = New System.Drawing.Size(739, 20)
        Me.BsReagentsNeedlesAdjustTitle.TabIndex = 31
        Me.BsReagentsNeedlesAdjustTitle.Text = "Reagents Arms Needles Adjustment/Test"
        Me.BsReagentsNeedlesAdjustTitle.Title = True
        '
        'TabFridge
        '
        Me.TabFridge.Controls.Add(Me.BsFridgeAdjustmentPanel)
        Me.TabFridge.Controls.Add(Me.BsFridgeInfoPanel)
        Me.TabFridge.Location = New System.Drawing.Point(4, 22)
        Me.TabFridge.Name = "TabFridge"
        Me.TabFridge.Padding = New System.Windows.Forms.Padding(3)
        Me.TabFridge.Size = New System.Drawing.Size(970, 532)
        Me.TabFridge.TabIndex = 3
        Me.TabFridge.Text = "Fridge"
        Me.TabFridge.UseVisualStyleBackColor = True
        '
        'BsFridgeAdjustmentPanel
        '
        Me.BsFridgeAdjustmentPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsFridgeAdjustmentPanel.BackColor = System.Drawing.Color.Gainsboro
        Me.BsFridgeAdjustmentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsFridgeAdjustmentPanel.Controls.Add(Me.BsRotorPanel)
        Me.BsFridgeAdjustmentPanel.Controls.Add(Me.BsFridgeAdjustTitle)
        Me.BsFridgeAdjustmentPanel.Location = New System.Drawing.Point(231, 0)
        Me.BsFridgeAdjustmentPanel.Name = "BsFridgeAdjustmentPanel"
        Me.BsFridgeAdjustmentPanel.Size = New System.Drawing.Size(739, 532)
        Me.BsFridgeAdjustmentPanel.TabIndex = 28
        '
        'BsRotorPanel
        '
        Me.BsRotorPanel.Controls.Add(Me.BsAnalyzerTempGroupBox)
        Me.BsRotorPanel.Controls.Add(Me.BSRotorGroupBox)
        Me.BsRotorPanel.Controls.Add(Me.RotorChartGroupBox)
        Me.BsRotorPanel.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.BsRotorPanel.Location = New System.Drawing.Point(0, 67)
        Me.BsRotorPanel.Name = "BsRotorPanel"
        Me.BsRotorPanel.Size = New System.Drawing.Size(737, 463)
        Me.BsRotorPanel.TabIndex = 71
        '
        'BsAnalyzerTempGroupBox
        '
        Me.BsAnalyzerTempGroupBox.Controls.Add(Me.BsRotorEditSetPointButton)
        Me.BsAnalyzerTempGroupBox.Controls.Add(Me.Label32)
        Me.BsAnalyzerTempGroupBox.Controls.Add(Me.TextBox22)
        Me.BsAnalyzerTempGroupBox.Location = New System.Drawing.Point(275, 273)
        Me.BsAnalyzerTempGroupBox.Name = "BsAnalyzerTempGroupBox"
        Me.BsAnalyzerTempGroupBox.Size = New System.Drawing.Size(461, 108)
        Me.BsAnalyzerTempGroupBox.TabIndex = 69
        Me.BsAnalyzerTempGroupBox.TabStop = False
        Me.BsAnalyzerTempGroupBox.Text = "Adjustment"
        '
        'BsRotorEditSetPointButton
        '
        Me.BsRotorEditSetPointButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsRotorEditSetPointButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsRotorEditSetPointButton.Location = New System.Drawing.Point(425, 71)
        Me.BsRotorEditSetPointButton.Name = "BsRotorEditSetPointButton"
        Me.BsRotorEditSetPointButton.Size = New System.Drawing.Size(32, 32)
        Me.BsRotorEditSetPointButton.TabIndex = 40
        Me.BsRotorEditSetPointButton.UseVisualStyleBackColor = True
        '
        'Label32
        '
        Me.Label32.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label32.Location = New System.Drawing.Point(106, 55)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(239, 25)
        Me.Label32.TabIndex = 57
        Me.Label32.Text = "Rotor Thermo Adjust Proposed"
        Me.Label32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TextBox22
        '
        Me.TextBox22.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox22.BackColor = System.Drawing.Color.White
        Me.TextBox22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox22.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.TextBox22.ForeColor = System.Drawing.Color.Red
        Me.TextBox22.Location = New System.Drawing.Point(12, 46)
        Me.TextBox22.Name = "TextBox22"
        Me.TextBox22.ReadOnly = True
        Me.TextBox22.ShortcutsEnabled = False
        Me.TextBox22.Size = New System.Drawing.Size(88, 40)
        Me.TextBox22.TabIndex = 56
        Me.TextBox22.TabStop = False
        Me.TextBox22.Text = "-0.6"
        Me.TextBox22.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'BSRotorGroupBox
        '
        Me.BSRotorGroupBox.Controls.Add(Me.FridgeTempChart)
        Me.BSRotorGroupBox.Location = New System.Drawing.Point(5, 3)
        Me.BSRotorGroupBox.Name = "BSRotorGroupBox"
        Me.BSRotorGroupBox.Size = New System.Drawing.Size(263, 378)
        Me.BSRotorGroupBox.TabIndex = 67
        Me.BSRotorGroupBox.TabStop = False
        Me.BSRotorGroupBox.Text = "Rotor Temperature"
        '
        'FridgeTempChart
        '
        Me.FridgeTempChart.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FridgeTempChart.AppearanceName = "Dark"
        Me.FridgeTempChart.BackColor = System.Drawing.Color.Gainsboro
        Me.FridgeTempChart.BorderOptions.Color = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.FridgeTempChart.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False
        XyDiagram1.AxisX.Color = System.Drawing.Color.Black
        XyDiagram1.AxisX.GridLines.Color = System.Drawing.Color.Black
        XyDiagram1.AxisX.NumericScaleOptions.AutoGrid = False
        XyDiagram1.AxisX.InterlacedColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        XyDiagram1.AxisX.Label.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        XyDiagram1.AxisX.Label.TextColor = System.Drawing.Color.Black
        XyDiagram1.AxisX.WholeRange.Auto = False
        XyDiagram1.AxisX.WholeRange.MaxValueSerializable = "6"
        XyDiagram1.AxisX.WholeRange.MinValueSerializable = "0"
        XyDiagram1.AxisX.WholeRange.AutoSideMargins = True
        XyDiagram1.AxisX.VisualRange.Auto = False
        XyDiagram1.AxisX.VisualRange.MaxValueSerializable = "6"
        XyDiagram1.AxisX.VisualRange.MinValueSerializable = "0"
        XyDiagram1.AxisX.VisualRange.AutoSideMargins = True
        XyDiagram1.AxisX.Title.Alignment = System.Drawing.StringAlignment.Far
        XyDiagram1.AxisX.Title.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        XyDiagram1.AxisX.Title.Text = "Measurements"
        XyDiagram1.AxisX.Title.TextColor = System.Drawing.Color.Black
        XyDiagram1.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.True
        XyDiagram1.AxisX.VisibleInPanesSerializable = "-1"
        XyDiagram1.AxisY.Color = System.Drawing.Color.Black
        ConstantLine1.AxisValueSerializable = "5.9"
        ConstantLine1.Color = System.Drawing.Color.SteelBlue
        ConstantLine1.LegendText = "Setpoint Temperature"
        ConstantLine1.LineStyle.Thickness = 3
        ConstantLine1.Name = ""
        ConstantLine1.ShowBehind = True
        ConstantLine2.AxisValueSerializable = "0"
        ConstantLine2.Color = System.Drawing.Color.Red
        ConstantLine2.LegendText = "Average Temperature"
        ConstantLine2.Name = ""
        ConstantLine2.Title.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        XyDiagram1.AxisY.ConstantLines.AddRange(New DevExpress.XtraCharts.ConstantLine() {ConstantLine1, ConstantLine2})
        XyDiagram1.AxisY.NumericScaleOptions.AutoGrid = False
        XyDiagram1.AxisY.InterlacedColor = System.Drawing.Color.Black
        XyDiagram1.AxisY.Label.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        XyDiagram1.AxisY.Label.TextColor = System.Drawing.Color.Black
        XyDiagram1.AxisY.VisualRange.Auto = False
        XyDiagram1.AxisY.VisualRange.MaxValueSerializable = "10"
        XyDiagram1.AxisY.VisualRange.MinValueSerializable = "0"
        XyDiagram1.AxisY.WholeRange.Auto = False
        XyDiagram1.AxisY.WholeRange.MaxValueSerializable = "11"
        XyDiagram1.AxisY.WholeRange.MinValueSerializable = "0"
        XyDiagram1.AxisY.WholeRange.AutoSideMargins = True
        XyDiagram1.AxisY.VisualRange.AutoSideMargins = True
        XyDiagram1.AxisY.ScaleBreakOptions.Color = System.Drawing.Color.Black
        XyDiagram1.AxisY.Title.Alignment = System.Drawing.StringAlignment.Far
        XyDiagram1.AxisY.Title.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        XyDiagram1.AxisY.Title.Text = "Temperature (℃)"
        XyDiagram1.AxisY.Title.TextColor = System.Drawing.Color.Black
        XyDiagram1.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True
        XyDiagram1.AxisY.VisibleInPanesSerializable = "-1"
        XyDiagram1.Margins.Bottom = 0
        XyDiagram1.Margins.Left = 0
        XyDiagram1.Margins.Right = 0
        XyDiagram1.Margins.Top = 0
        XyDiagram1.PaneDistance = 0
        Me.FridgeTempChart.Diagram = XyDiagram1
        Me.FridgeTempChart.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Right
        Me.FridgeTempChart.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.BottomOutside
        Me.FridgeTempChart.Legend.Antialiasing = True
        Me.FridgeTempChart.Legend.BackColor = System.Drawing.Color.DimGray
        Me.FridgeTempChart.Legend.BackImage.Stretch = True
        Me.FridgeTempChart.Legend.Border.Visibility = DevExpress.Utils.DefaultBoolean.False
        Me.FridgeTempChart.Legend.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FridgeTempChart.Legend.TextColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.FridgeTempChart.Location = New System.Drawing.Point(6, 20)
        Me.FridgeTempChart.Name = "FridgeTempChart"
        Me.FridgeTempChart.Padding.Bottom = 0
        Me.FridgeTempChart.Padding.Left = 0
        Me.FridgeTempChart.Padding.Right = 0
        Me.FridgeTempChart.Padding.Top = 0
        Me.FridgeTempChart.PaletteName = "Nature Colors"
        Series1.ArgumentScaleType = DevExpress.XtraCharts.ScaleType.Numerical
        PointSeriesLabel1.LineVisibility = DevExpress.Utils.DefaultBoolean.True
        Series1.Label = PointSeriesLabel1
        PointOptions1.ArgumentNumericOptions.Format = DevExpress.XtraCharts.NumericFormat.FixedPoint
        PointOptions1.ArgumentNumericOptions.Precision = 1
        PointOptions1.ValueNumericOptions.Format = DevExpress.XtraCharts.NumericFormat.FixedPoint
        PointOptions1.ValueNumericOptions.Precision = 1
        Series1.LegendTextPattern = "{V:n2}"
        Series1.LegendText = "Probe Temperatures"
        Series1.Name = "ProbeTemps"
        PointOptions2.ArgumentNumericOptions.Format = DevExpress.XtraCharts.NumericFormat.FixedPoint
        PointOptions2.ArgumentNumericOptions.Precision = 1
        PointOptions2.ValueNumericOptions.Format = DevExpress.XtraCharts.NumericFormat.FixedPoint
        PointOptions2.ValueNumericOptions.Precision = 1
        Series1.SynchronizePointOptions = False
        PointSeriesView1.PointMarkerOptions.FillStyle.FillMode = DevExpress.XtraCharts.FillMode.Gradient
        PointSeriesView1.PointMarkerOptions.Size = 10
        Series1.View = PointSeriesView1
        Me.FridgeTempChart.SeriesSerializable = New DevExpress.XtraCharts.Series() {Series1}
        PointSeriesLabel2.LineVisibility = DevExpress.Utils.DefaultBoolean.True
        Me.FridgeTempChart.SeriesTemplate.Label = PointSeriesLabel2
        SplineSeriesView1.LineMarkerOptions.Size = 8
        Me.FridgeTempChart.SeriesTemplate.View = SplineSeriesView1
        Me.FridgeTempChart.Size = New System.Drawing.Size(251, 348)
        Me.FridgeTempChart.SmallChartText.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FridgeTempChart.SmallChartText.Text = ""
        Me.FridgeTempChart.SmallChartText.TextColor = System.Drawing.Color.Black
        Me.FridgeTempChart.TabIndex = 3
        ChartTitle1.Alignment = System.Drawing.StringAlignment.Near
        ChartTitle1.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        ChartTitle1.Text = "Fridge Temperature:"
        ChartTitle1.TextColor = System.Drawing.Color.Black
        ChartTitle1.Visibility = DevExpress.Utils.DefaultBoolean.False
        ChartTitle1.WordWrap = True
        Me.FridgeTempChart.Titles.AddRange(New DevExpress.XtraCharts.ChartTitle() {ChartTitle1})
        '
        'RotorChartGroupBox
        '
        Me.RotorChartGroupBox.Controls.Add(Me.BsButton7)
        Me.RotorChartGroupBox.Controls.Add(Me.BsButton8)
        Me.RotorChartGroupBox.Controls.Add(Me.Label30)
        Me.RotorChartGroupBox.Controls.Add(Me.Label31)
        Me.RotorChartGroupBox.Controls.Add(Me.Label15)
        Me.RotorChartGroupBox.Controls.Add(Me.TextBox11)
        Me.RotorChartGroupBox.Controls.Add(Me.Label18)
        Me.RotorChartGroupBox.Controls.Add(Me.Label19)
        Me.RotorChartGroupBox.Controls.Add(Me.TextBox12)
        Me.RotorChartGroupBox.Controls.Add(Me.TextBox13)
        Me.RotorChartGroupBox.Controls.Add(Me.TextBox14)
        Me.RotorChartGroupBox.Controls.Add(Me.Label20)
        Me.RotorChartGroupBox.Controls.Add(Me.Label21)
        Me.RotorChartGroupBox.Controls.Add(Me.TextBox15)
        Me.RotorChartGroupBox.Controls.Add(Me.TextBox16)
        Me.RotorChartGroupBox.Controls.Add(Me.Label22)
        Me.RotorChartGroupBox.Controls.Add(Me.Label23)
        Me.RotorChartGroupBox.Controls.Add(Me.TextBox20)
        Me.RotorChartGroupBox.Controls.Add(Me.TextBox21)
        Me.RotorChartGroupBox.Controls.Add(Me.Label26)
        Me.RotorChartGroupBox.Location = New System.Drawing.Point(275, 3)
        Me.RotorChartGroupBox.Name = "RotorChartGroupBox"
        Me.RotorChartGroupBox.Size = New System.Drawing.Size(460, 264)
        Me.RotorChartGroupBox.TabIndex = 68
        Me.RotorChartGroupBox.TabStop = False
        Me.RotorChartGroupBox.Text = "Temperature Measurement"
        '
        'BsButton7
        '
        Me.BsButton7.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsButton7.Location = New System.Drawing.Point(387, 228)
        Me.BsButton7.Name = "BsButton7"
        Me.BsButton7.Size = New System.Drawing.Size(32, 32)
        Me.BsButton7.TabIndex = 107
        Me.BsButton7.UseVisualStyleBackColor = True
        '
        'BsButton8
        '
        Me.BsButton8.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButton8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsButton8.Location = New System.Drawing.Point(423, 228)
        Me.BsButton8.Name = "BsButton8"
        Me.BsButton8.Size = New System.Drawing.Size(32, 32)
        Me.BsButton8.TabIndex = 106
        Me.BsButton8.UseVisualStyleBackColor = True
        '
        'Label30
        '
        Me.Label30.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label30.Location = New System.Drawing.Point(106, 228)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(88, 25)
        Me.Label30.TabIndex = 105
        Me.Label30.Text = "Mean"
        Me.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label31
        '
        Me.Label31.BackColor = System.Drawing.Color.WhiteSmoke
        Me.Label31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label31.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.Label31.ForeColor = System.Drawing.Color.SteelBlue
        Me.Label31.Location = New System.Drawing.Point(12, 220)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(88, 36)
        Me.Label31.TabIndex = 104
        Me.Label31.Text = "37.6"
        Me.Label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label15
        '
        Me.Label15.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(154, 35)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(18, 25)
        Me.Label15.TabIndex = 103
        Me.Label15.Text = "3"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TextBox11
        '
        Me.TextBox11.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox11.BackColor = System.Drawing.Color.White
        Me.TextBox11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox11.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.TextBox11.ForeColor = System.Drawing.Color.Black
        Me.TextBox11.Location = New System.Drawing.Point(11, 63)
        Me.TextBox11.Name = "TextBox11"
        Me.TextBox11.ReadOnly = True
        Me.TextBox11.ShortcutsEnabled = False
        Me.TextBox11.Size = New System.Drawing.Size(55, 40)
        Me.TextBox11.TabIndex = 89
        Me.TextBox11.TabStop = False
        Me.TextBox11.Text = "0"
        Me.TextBox11.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label18
        '
        Me.Label18.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label18.Location = New System.Drawing.Point(48, 35)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(18, 25)
        Me.Label18.TabIndex = 88
        Me.Label18.Text = "1"
        Me.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label19
        '
        Me.Label19.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(210, 35)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(18, 25)
        Me.Label19.TabIndex = 102
        Me.Label19.Text = "4"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TextBox12
        '
        Me.TextBox12.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox12.BackColor = System.Drawing.Color.White
        Me.TextBox12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox12.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.TextBox12.ForeColor = System.Drawing.Color.Black
        Me.TextBox12.Location = New System.Drawing.Point(335, 63)
        Me.TextBox12.Name = "TextBox12"
        Me.TextBox12.ReadOnly = True
        Me.TextBox12.ShortcutsEnabled = False
        Me.TextBox12.Size = New System.Drawing.Size(55, 40)
        Me.TextBox12.TabIndex = 95
        Me.TextBox12.TabStop = False
        Me.TextBox12.Text = "0"
        Me.TextBox12.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBox13
        '
        Me.TextBox13.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox13.BackColor = System.Drawing.Color.White
        Me.TextBox13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox13.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.TextBox13.ForeColor = System.Drawing.Color.Black
        Me.TextBox13.Location = New System.Drawing.Point(65, 63)
        Me.TextBox13.Name = "TextBox13"
        Me.TextBox13.ReadOnly = True
        Me.TextBox13.ShortcutsEnabled = False
        Me.TextBox13.Size = New System.Drawing.Size(55, 40)
        Me.TextBox13.TabIndex = 90
        Me.TextBox13.TabStop = False
        Me.TextBox13.Text = "0"
        Me.TextBox13.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBox14
        '
        Me.TextBox14.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox14.BackColor = System.Drawing.Color.White
        Me.TextBox14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox14.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.TextBox14.ForeColor = System.Drawing.Color.Black
        Me.TextBox14.Location = New System.Drawing.Point(389, 63)
        Me.TextBox14.Name = "TextBox14"
        Me.TextBox14.ReadOnly = True
        Me.TextBox14.ShortcutsEnabled = False
        Me.TextBox14.Size = New System.Drawing.Size(55, 40)
        Me.TextBox14.TabIndex = 96
        Me.TextBox14.TabStop = False
        Me.TextBox14.Text = "0"
        Me.TextBox14.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label20
        '
        Me.Label20.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(264, 35)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(18, 25)
        Me.Label20.TabIndex = 101
        Me.Label20.Text = "5"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label21
        '
        Me.Label21.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.Location = New System.Drawing.Point(102, 35)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(18, 25)
        Me.Label21.TabIndex = 97
        Me.Label21.Text = "2"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TextBox15
        '
        Me.TextBox15.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox15.BackColor = System.Drawing.Color.White
        Me.TextBox15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox15.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.TextBox15.ForeColor = System.Drawing.Color.Black
        Me.TextBox15.Location = New System.Drawing.Point(119, 63)
        Me.TextBox15.Name = "TextBox15"
        Me.TextBox15.ReadOnly = True
        Me.TextBox15.ShortcutsEnabled = False
        Me.TextBox15.Size = New System.Drawing.Size(55, 40)
        Me.TextBox15.TabIndex = 91
        Me.TextBox15.TabStop = False
        Me.TextBox15.Text = "0"
        Me.TextBox15.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBox16
        '
        Me.TextBox16.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox16.BackColor = System.Drawing.Color.White
        Me.TextBox16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox16.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.TextBox16.ForeColor = System.Drawing.Color.Black
        Me.TextBox16.Location = New System.Drawing.Point(281, 63)
        Me.TextBox16.Name = "TextBox16"
        Me.TextBox16.ReadOnly = True
        Me.TextBox16.ShortcutsEnabled = False
        Me.TextBox16.Size = New System.Drawing.Size(55, 40)
        Me.TextBox16.TabIndex = 94
        Me.TextBox16.TabStop = False
        Me.TextBox16.Text = "0"
        Me.TextBox16.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label22
        '
        Me.Label22.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(372, 35)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(18, 25)
        Me.Label22.TabIndex = 100
        Me.Label22.Text = "7"
        Me.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label23
        '
        Me.Label23.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(318, 35)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(18, 25)
        Me.Label23.TabIndex = 98
        Me.Label23.Text = "6"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TextBox20
        '
        Me.TextBox20.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox20.BackColor = System.Drawing.Color.White
        Me.TextBox20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox20.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.TextBox20.ForeColor = System.Drawing.Color.Black
        Me.TextBox20.Location = New System.Drawing.Point(173, 63)
        Me.TextBox20.Name = "TextBox20"
        Me.TextBox20.ReadOnly = True
        Me.TextBox20.ShortcutsEnabled = False
        Me.TextBox20.Size = New System.Drawing.Size(55, 40)
        Me.TextBox20.TabIndex = 92
        Me.TextBox20.TabStop = False
        Me.TextBox20.Text = "0"
        Me.TextBox20.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBox21
        '
        Me.TextBox21.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox21.BackColor = System.Drawing.Color.White
        Me.TextBox21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox21.Font = New System.Drawing.Font("Digiface", 20.25!)
        Me.TextBox21.ForeColor = System.Drawing.Color.Black
        Me.TextBox21.Location = New System.Drawing.Point(227, 63)
        Me.TextBox21.Name = "TextBox21"
        Me.TextBox21.ReadOnly = True
        Me.TextBox21.ShortcutsEnabled = False
        Me.TextBox21.Size = New System.Drawing.Size(55, 40)
        Me.TextBox21.TabIndex = 93
        Me.TextBox21.TabStop = False
        Me.TextBox21.Text = "0"
        Me.TextBox21.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label26
        '
        Me.Label26.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label26.Location = New System.Drawing.Point(426, 35)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(18, 25)
        Me.Label26.TabIndex = 99
        Me.Label26.Text = "8"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BsFridgeAdjustTitle
        '
        Me.BsFridgeAdjustTitle.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsFridgeAdjustTitle.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsFridgeAdjustTitle.ForeColor = System.Drawing.Color.Black
        Me.BsFridgeAdjustTitle.Location = New System.Drawing.Point(0, 0)
        Me.BsFridgeAdjustTitle.Name = "BsFridgeAdjustTitle"
        Me.BsFridgeAdjustTitle.Size = New System.Drawing.Size(739, 20)
        Me.BsFridgeAdjustTitle.TabIndex = 33
        Me.BsFridgeAdjustTitle.Text = "Fridge Temperature Adjustment/Test"
        Me.BsFridgeAdjustTitle.Title = True
        '
        'BsFridgeInfoPanel
        '
        Me.BsFridgeInfoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsFridgeInfoPanel.Controls.Add(Me.BsLabel2)
        Me.BsFridgeInfoPanel.Location = New System.Drawing.Point(0, 0)
        Me.BsFridgeInfoPanel.Name = "BsFridgeInfoPanel"
        Me.BsFridgeInfoPanel.Size = New System.Drawing.Size(232, 532)
        Me.BsFridgeInfoPanel.TabIndex = 27
        '
        'BsLabel2
        '
        Me.BsLabel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsLabel2.BackColor = System.Drawing.Color.LightSteelBlue
        Me.BsLabel2.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.BsLabel2.ForeColor = System.Drawing.Color.Black
        Me.BsLabel2.Location = New System.Drawing.Point(0, 0)
        Me.BsLabel2.Name = "BsLabel2"
        Me.BsLabel2.Size = New System.Drawing.Size(231, 20)
        Me.BsLabel2.TabIndex = 25
        Me.BsLabel2.Text = "Information"
        Me.BsLabel2.Title = True
        '
        'BsButtonsPanel
        '
        Me.BsButtonsPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsButtonsPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsButtonsPanel.Controls.Add(Me.BsExitButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsSaveButton)
        Me.BsButtonsPanel.Controls.Add(Me.BsSaveButtonNOUSED)
        Me.BsButtonsPanel.Controls.Add(Me.BsAdjustButtonNOUSED)
        Me.BsButtonsPanel.Location = New System.Drawing.Point(810, 557)
        Me.BsButtonsPanel.Name = "BsButtonsPanel"
        Me.BsButtonsPanel.Size = New System.Drawing.Size(168, 35)
        Me.BsButtonsPanel.TabIndex = 16
        '
        'BsExitButton
        '
        Me.BsExitButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsExitButton.Enabled = False
        Me.BsExitButton.Location = New System.Drawing.Point(134, 1)
        Me.BsExitButton.Name = "BsExitButton"
        Me.BsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.BsExitButton.TabIndex = 17
        Me.BsExitButton.UseVisualStyleBackColor = True
        '
        'BsSaveButton
        '
        Me.BsSaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsSaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsSaveButton.Enabled = False
        Me.BsSaveButton.Location = New System.Drawing.Point(98, 1)
        Me.BsSaveButton.Name = "BsSaveButton"
        Me.BsSaveButton.Size = New System.Drawing.Size(32, 32)
        Me.BsSaveButton.TabIndex = 16
        Me.BsSaveButton.UseVisualStyleBackColor = True
        '
        'BsSaveButtonNOUSED
        '
        Me.BsSaveButtonNOUSED.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsSaveButtonNOUSED.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsSaveButtonNOUSED.Enabled = False
        Me.BsSaveButtonNOUSED.Location = New System.Drawing.Point(60, 1)
        Me.BsSaveButtonNOUSED.Name = "BsSaveButtonNOUSED"
        Me.BsSaveButtonNOUSED.Size = New System.Drawing.Size(32, 32)
        Me.BsSaveButtonNOUSED.TabIndex = 1
        Me.BsSaveButtonNOUSED.UseVisualStyleBackColor = True
        '
        'BsAdjustButtonNOUSED
        '
        Me.BsAdjustButtonNOUSED.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAdjustButtonNOUSED.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.BsAdjustButtonNOUSED.Enabled = False
        Me.BsAdjustButtonNOUSED.Location = New System.Drawing.Point(22, 1)
        Me.BsAdjustButtonNOUSED.Name = "BsAdjustButtonNOUSED"
        Me.BsAdjustButtonNOUSED.Size = New System.Drawing.Size(32, 32)
        Me.BsAdjustButtonNOUSED.TabIndex = 0
        Me.BsAdjustButtonNOUSED.UseVisualStyleBackColor = True
        '
        'BsMessagesPanel
        '
        Me.BsMessagesPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsMessagesPanel.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BsMessagesPanel.Controls.Add(Me.ProgressBar1)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageImage)
        Me.BsMessagesPanel.Controls.Add(Me.BsMessageLabel)
        Me.BsMessagesPanel.Location = New System.Drawing.Point(0, 557)
        Me.BsMessagesPanel.Name = "BsMessagesPanel"
        Me.BsMessagesPanel.Size = New System.Drawing.Size(811, 35)
        Me.BsMessagesPanel.TabIndex = 17
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(490, 8)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(180, 18)
        Me.ProgressBar1.TabIndex = 8
        Me.ProgressBar1.Visible = False
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
        'CurrentOperationTimer
        '
        '
        'SimulationTimer
        '
        Me.SimulationTimer.Interval = 5000
        '
        'IThermosAdjustments
        '
        Me.AcceptButton = Me.BsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(978, 593)
        Me.Controls.Add(Me.BsMessagesPanel)
        Me.Controls.Add(Me.BsButtonsPanel)
        Me.Controls.Add(Me.BsTabPagesControl)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "UiThermosAdjustments"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = ""
        Me.BsTabPagesControl.ResumeLayout(False)
        Me.TabWSHeater.ResumeLayout(False)
        Me.BsWSHeaterInfoPanel.ResumeLayout(False)
        Me.BsWSHeaterAdjustPanel.ResumeLayout(False)
        Me.BSOpticWSGroupBox.ResumeLayout(False)
        Me.Tab3AdjustGroupBox.ResumeLayout(False)
        CType(Me.Tab3CorrectionTextBox.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Tab3MeasurementGroupBox.ResumeLayout(False)
        CType(Me.Tab3TextBoxTemp.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Tab3ConditioningGroupBox.ResumeLayout(False)
        Me.TabReactionsRotor.ResumeLayout(False)
        Me.BsReactionsRotorInfoPanel.ResumeLayout(False)
        Me.BsReactionsRotorAdjustPanel.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.Tab1AdjustGroupBox.ResumeLayout(False)
        CType(Me.Tab1CorrectionTextBox.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Tab1ConditioningGroupBox.ResumeLayout(False)
        Me.Tab1ConditioningGroupBox.PerformLayout()
        Me.Tab1MeasurementGroupBox.ResumeLayout(False)
        CType(Me.Tab1TextBoxTemp1.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Tab1TextBoxTemp2.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Tab1TextBoxTemp3.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Tab1TextBoxTemp4.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabReagentsNeedles.ResumeLayout(False)
        Me.BsReagentsNeedlesInfoPanel.ResumeLayout(False)
        Me.BsReagentsNeedlesAdjustPanel.ResumeLayout(False)
        Me.Tab2AdjustGroupBox.ResumeLayout(False)
        CType(Me.Tab2CorrectionTextBox.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Tab2MeasurementGroupBox.ResumeLayout(False)
        CType(Me.Tab2TextBoxTemp.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Tab2ConditioningGroupBox.ResumeLayout(False)
        Me.Tab2SelectArmGroupBox.ResumeLayout(False)
        Me.Tab2SelectArmGroupBox.PerformLayout()
        Me.TabFridge.ResumeLayout(False)
        Me.BsFridgeAdjustmentPanel.ResumeLayout(False)
        Me.BsRotorPanel.ResumeLayout(False)
        Me.BsAnalyzerTempGroupBox.ResumeLayout(False)
        Me.BsAnalyzerTempGroupBox.PerformLayout()
        Me.BSRotorGroupBox.ResumeLayout(False)
        CType(XyDiagram1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Series1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(PointSeriesLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(SplineSeriesView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.FridgeTempChart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RotorChartGroupBox.ResumeLayout(False)
        Me.RotorChartGroupBox.PerformLayout()
        Me.BsFridgeInfoPanel.ResumeLayout(False)
        Me.BsButtonsPanel.ResumeLayout(False)
        Me.BsMessagesPanel.ResumeLayout(False)
        CType(Me.BsMessageImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsTabPagesControl As Biosystems.Ax00.Controls.UserControls.BSTabControl
    Friend WithEvents TabReactionsRotor As System.Windows.Forms.TabPage
    Friend WithEvents BsReactionsRotorInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsReactionsRotorInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsReactionsRotorAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents ReactionsRotorAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents TabReagentsNeedles As System.Windows.Forms.TabPage
    Friend WithEvents BsReagentsNeedlesInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsReagentsNeedlesInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsReagentsNeedlesAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsReagentsNeedlesAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents TabWSHeater As System.Windows.Forms.TabPage
    Friend WithEvents BsWSHeaterInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsWSHeaterInfoLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsWSHeaterInfoTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsWSHeaterAdjustPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsWSHeaterAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents TabFridge As System.Windows.Forms.TabPage
    Friend WithEvents BsFridgeAdjustmentPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsFridgeAdjustTitle As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsFridgeInfoPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsLabel2 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsButtonsPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsSaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsSaveButtonNOUSED As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAdjustButtonNOUSED As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsMessagesPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsMessageImage As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents BsMessageLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents Tab1ConditioningButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Tab1AutoRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents Tab1ManualRadioButton As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents Tab2RadioButtonR1 As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents Tab2RadioButtonR2 As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents BsRotorPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
    Friend WithEvents BsAnalyzerTempGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsRotorEditSetPointButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BSRotorGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents RotorChartGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents FridgeTempChart As DevExpress.XtraCharts.ChartControl
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents TextBox22 As System.Windows.Forms.TextBox
    Friend WithEvents BsButton7 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsButton8 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents TextBox11 As System.Windows.Forms.TextBox
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents TextBox12 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox13 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox14 As System.Windows.Forms.TextBox
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents TextBox15 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox16 As System.Windows.Forms.TextBox
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents TextBox20 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox21 As System.Windows.Forms.TextBox
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents Tab1AdjustGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Tab1AdjustButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Tab1AdjustProposedLabel As System.Windows.Forms.Label
    Friend WithEvents Tab1ConditioningGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Tab1MeasurementGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Tab1MeanLabel As System.Windows.Forms.Label
    Friend WithEvents TabMeanTempLabel As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Tab2MeasurementGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Tab2MeasureButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Tab2SelectArmGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Tab2ConditioningGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents BsButton9 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Tab2TempMeasuredLabel As System.Windows.Forms.Label
    Friend WithEvents Tab2ConditioningDescLabel As System.Windows.Forms.Label
    Friend WithEvents Tab2ConditioningButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Tab2AdjustGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Tab2AdjustProposedLabel As System.Windows.Forms.Label
    Friend WithEvents Tab2AdjustButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents CurrentOperationTimer As System.Windows.Forms.Timer
    Friend WithEvents Tab3AdjustGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Tab3AdjustProposedLabel As System.Windows.Forms.Label
    Friend WithEvents Tab3AdjustButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Tab3MeasurementGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Tab3TempMeasuredLabel As System.Windows.Forms.Label
    Friend WithEvents Tab3ConditioningGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Tab3ConditioningButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Tab3ConditioningDescLabel As System.Windows.Forms.Label
    Friend WithEvents BsButton6 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ProgressBar1 As Biosystems.Ax00.Controls.UserControls.BSProgressBar
    Friend WithEvents Tab2DispensationsCountTitleLabel As System.Windows.Forms.Label
    Friend WithEvents Tab2DispensationsCountLabel As System.Windows.Forms.Label
    Friend WithEvents SimulationTimer As Biosystems.Ax00.Controls.UserControls.BSTimer
    Friend WithEvents BsInfoWsHeaterXPSViewer As BsXPSViewer
    Friend WithEvents BsInfoRotorXPSViewer As BsXPSViewer
    Friend WithEvents BsInfoNeedlesXPSViewer As BsXPSViewer
    Friend WithEvents Tab2AuxButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Tab3TextBoxTemp As DevExpress.XtraEditors.TextEdit
    Friend WithEvents Tab3CorrectionTextBox As DevExpress.XtraEditors.TextEdit
    Friend WithEvents Tab1CorrectionTextBox As DevExpress.XtraEditors.TextEdit
    Friend WithEvents Tab1TextBoxTemp1 As DevExpress.XtraEditors.TextEdit
    Friend WithEvents Tab1TextBoxTemp2 As DevExpress.XtraEditors.TextEdit
    Friend WithEvents Tab1TextBoxTemp3 As DevExpress.XtraEditors.TextEdit
    Friend WithEvents Tab1TextBoxTemp4 As DevExpress.XtraEditors.TextEdit
    Friend WithEvents Tab2CorrectionTextBox As DevExpress.XtraEditors.TextEdit
    Friend WithEvents Tab2TextBoxTemp As DevExpress.XtraEditors.TextEdit
    Friend WithEvents BSOpticWSGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Tab3HeaterWSLabel As System.Windows.Forms.Label
    Friend WithEvents BsButton4 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsUpDownWSButton1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Tab1RotorReactLabel As System.Windows.Forms.Label
    Friend WithEvents BsButton1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsUpDownWSButton2 As Biosystems.Ax00.Controls.UserControls.BSButton
End Class
