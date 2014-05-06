Namespace Biosystems.Ax00.Controls.UserControls



    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BSFormula
        Inherits System.Windows.Forms.UserControl

        'UserControl reemplaza a Dispose para limpiar la lista de componentes.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Requerido por el Diseñador de Windows Forms
        Private components As System.ComponentModel.IContainer

        'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
        'Se puede modificar usando el Diseñador de Windows Forms.  
        'No lo modifique con el editor de código.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BSFormula))
            Me.OneButton = New System.Windows.Forms.Button
            Me.ThreeButton = New System.Windows.Forms.Button
            Me.SevenButton = New System.Windows.Forms.Button
            Me.FourButton = New System.Windows.Forms.Button
            Me.EightButton = New System.Windows.Forms.Button
            Me.NineButton = New System.Windows.Forms.Button
            Me.ZeroButton = New System.Windows.Forms.Button
            Me.FiveButton = New System.Windows.Forms.Button
            Me.SixButton = New System.Windows.Forms.Button
            Me.TwoButton = New System.Windows.Forms.Button
            Me.DotButton = New System.Windows.Forms.Button
            Me.NumbersPanel = New System.Windows.Forms.Panel
            Me.MultButton = New System.Windows.Forms.Button
            Me.ClearButton = New System.Windows.Forms.Button
            Me.CloseParenthesisButton = New System.Windows.Forms.Button
            Me.BackButton = New System.Windows.Forms.Button
            Me.OpenParenthesisButton = New System.Windows.Forms.Button
            Me.DivisionButton = New System.Windows.Forms.Button
            Me.AddButton = New System.Windows.Forms.Button
            Me.MinusButton = New System.Windows.Forms.Button
            Me.FormulaTextBox = New System.Windows.Forms.TextBox
            Me.FormulaLabel = New System.Windows.Forms.Label
            Me.Panel1 = New System.Windows.Forms.Panel
            Me.bsCalculatedLabel = New System.Windows.Forms.Label
            Me.bsStandardLabel = New System.Windows.Forms.Label
            Me.bsCalculatedTestListView = New Biosystems.Ax00.Controls.UserControls.BSListView
            Me.bsStandardTestListView = New Biosystems.Ax00.Controls.UserControls.BSListView
            Me.SampleTypeListComboBox = New System.Windows.Forms.ComboBox
            Me.SampleTypeLabel = New System.Windows.Forms.Label
            Me.AddSelectTestButton = New System.Windows.Forms.Button
            Me.FormulaStatusImageList = New System.Windows.Forms.ImageList(Me.components)
            Me.FormulaStatusImage = New System.Windows.Forms.PictureBox
            Me.bsAuxForErrorLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.bsFormulaToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
            Me.bsFormulaErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
            Me.NumbersPanel.SuspendLayout()
            Me.Panel1.SuspendLayout()
            CType(Me.FormulaStatusImage, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.bsFormulaErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'OneButton
            '
            Me.OneButton.BackColor = System.Drawing.Color.Gainsboro
            Me.OneButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.OneButton.Image = CType(resources.GetObject("OneButton.Image"), System.Drawing.Image)
            Me.OneButton.Location = New System.Drawing.Point(4, 118)
            Me.OneButton.Name = "OneButton"
            Me.OneButton.Size = New System.Drawing.Size(45, 45)
            Me.OneButton.TabIndex = 7
            Me.OneButton.UseVisualStyleBackColor = False
            '
            'ThreeButton
            '
            Me.ThreeButton.BackColor = System.Drawing.Color.Gainsboro
            Me.ThreeButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ThreeButton.Image = CType(resources.GetObject("ThreeButton.Image"), System.Drawing.Image)
            Me.ThreeButton.Location = New System.Drawing.Point(94, 118)
            Me.ThreeButton.Name = "ThreeButton"
            Me.ThreeButton.Size = New System.Drawing.Size(45, 45)
            Me.ThreeButton.TabIndex = 9
            Me.ThreeButton.UseVisualStyleBackColor = False
            '
            'SevenButton
            '
            Me.SevenButton.BackColor = System.Drawing.Color.Gainsboro
            Me.SevenButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.SevenButton.Image = CType(resources.GetObject("SevenButton.Image"), System.Drawing.Image)
            Me.SevenButton.Location = New System.Drawing.Point(4, 4)
            Me.SevenButton.Name = "SevenButton"
            Me.SevenButton.Size = New System.Drawing.Size(45, 45)
            Me.SevenButton.TabIndex = 13
            Me.SevenButton.UseVisualStyleBackColor = False
            '
            'FourButton
            '
            Me.FourButton.BackColor = System.Drawing.Color.Gainsboro
            Me.FourButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FourButton.Image = CType(resources.GetObject("FourButton.Image"), System.Drawing.Image)
            Me.FourButton.Location = New System.Drawing.Point(4, 61)
            Me.FourButton.Name = "FourButton"
            Me.FourButton.Size = New System.Drawing.Size(45, 45)
            Me.FourButton.TabIndex = 10
            Me.FourButton.UseVisualStyleBackColor = False
            '
            'EightButton
            '
            Me.EightButton.BackColor = System.Drawing.Color.Gainsboro
            Me.EightButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.EightButton.Image = CType(resources.GetObject("EightButton.Image"), System.Drawing.Image)
            Me.EightButton.Location = New System.Drawing.Point(49, 4)
            Me.EightButton.Name = "EightButton"
            Me.EightButton.Size = New System.Drawing.Size(45, 45)
            Me.EightButton.TabIndex = 14
            Me.EightButton.UseVisualStyleBackColor = False
            '
            'NineButton
            '
            Me.NineButton.BackColor = System.Drawing.Color.Gainsboro
            Me.NineButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.NineButton.Image = CType(resources.GetObject("NineButton.Image"), System.Drawing.Image)
            Me.NineButton.Location = New System.Drawing.Point(94, 4)
            Me.NineButton.Name = "NineButton"
            Me.NineButton.Size = New System.Drawing.Size(45, 45)
            Me.NineButton.TabIndex = 15
            Me.NineButton.UseVisualStyleBackColor = False
            '
            'ZeroButton
            '
            Me.ZeroButton.BackColor = System.Drawing.Color.Gainsboro
            Me.ZeroButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ZeroButton.Image = CType(resources.GetObject("ZeroButton.Image"), System.Drawing.Image)
            Me.ZeroButton.Location = New System.Drawing.Point(4, 175)
            Me.ZeroButton.Name = "ZeroButton"
            Me.ZeroButton.Size = New System.Drawing.Size(90, 45)
            Me.ZeroButton.TabIndex = 6
            Me.ZeroButton.UseVisualStyleBackColor = False
            '
            'FiveButton
            '
            Me.FiveButton.BackColor = System.Drawing.Color.Gainsboro
            Me.FiveButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FiveButton.Image = CType(resources.GetObject("FiveButton.Image"), System.Drawing.Image)
            Me.FiveButton.Location = New System.Drawing.Point(49, 61)
            Me.FiveButton.Name = "FiveButton"
            Me.FiveButton.Size = New System.Drawing.Size(45, 45)
            Me.FiveButton.TabIndex = 11
            Me.FiveButton.UseVisualStyleBackColor = False
            '
            'SixButton
            '
            Me.SixButton.BackColor = System.Drawing.Color.Gainsboro
            Me.SixButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.SixButton.Image = CType(resources.GetObject("SixButton.Image"), System.Drawing.Image)
            Me.SixButton.Location = New System.Drawing.Point(94, 61)
            Me.SixButton.Name = "SixButton"
            Me.SixButton.Size = New System.Drawing.Size(45, 45)
            Me.SixButton.TabIndex = 12
            Me.SixButton.UseVisualStyleBackColor = False
            '
            'TwoButton
            '
            Me.TwoButton.BackColor = System.Drawing.Color.Gainsboro
            Me.TwoButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TwoButton.Image = CType(resources.GetObject("TwoButton.Image"), System.Drawing.Image)
            Me.TwoButton.Location = New System.Drawing.Point(49, 118)
            Me.TwoButton.Name = "TwoButton"
            Me.TwoButton.Size = New System.Drawing.Size(45, 45)
            Me.TwoButton.TabIndex = 8
            Me.TwoButton.UseVisualStyleBackColor = False
            '
            'DotButton
            '
            Me.DotButton.BackColor = System.Drawing.Color.Gainsboro
            Me.DotButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.DotButton.Image = CType(resources.GetObject("DotButton.Image"), System.Drawing.Image)
            Me.DotButton.Location = New System.Drawing.Point(94, 175)
            Me.DotButton.Name = "DotButton"
            Me.DotButton.Size = New System.Drawing.Size(45, 45)
            Me.DotButton.TabIndex = 16
            Me.DotButton.UseVisualStyleBackColor = False
            '
            'NumbersPanel
            '
            Me.NumbersPanel.BackColor = System.Drawing.Color.Transparent
            Me.NumbersPanel.Controls.Add(Me.MultButton)
            Me.NumbersPanel.Controls.Add(Me.ClearButton)
            Me.NumbersPanel.Controls.Add(Me.CloseParenthesisButton)
            Me.NumbersPanel.Controls.Add(Me.BackButton)
            Me.NumbersPanel.Controls.Add(Me.OpenParenthesisButton)
            Me.NumbersPanel.Controls.Add(Me.DivisionButton)
            Me.NumbersPanel.Controls.Add(Me.AddButton)
            Me.NumbersPanel.Controls.Add(Me.MinusButton)
            Me.NumbersPanel.Controls.Add(Me.SixButton)
            Me.NumbersPanel.Controls.Add(Me.OneButton)
            Me.NumbersPanel.Controls.Add(Me.ThreeButton)
            Me.NumbersPanel.Controls.Add(Me.SevenButton)
            Me.NumbersPanel.Controls.Add(Me.FourButton)
            Me.NumbersPanel.Controls.Add(Me.EightButton)
            Me.NumbersPanel.Controls.Add(Me.NineButton)
            Me.NumbersPanel.Controls.Add(Me.ZeroButton)
            Me.NumbersPanel.Controls.Add(Me.FiveButton)
            Me.NumbersPanel.Controls.Add(Me.DotButton)
            Me.NumbersPanel.Controls.Add(Me.TwoButton)
            Me.NumbersPanel.Location = New System.Drawing.Point(435, 137)
            Me.NumbersPanel.Name = "NumbersPanel"
            Me.NumbersPanel.Size = New System.Drawing.Size(243, 224)
            Me.NumbersPanel.TabIndex = 5
            '
            'MultButton
            '
            Me.MultButton.BackColor = System.Drawing.Color.Gainsboro
            Me.MultButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.MultButton.Image = CType(resources.GetObject("MultButton.Image"), System.Drawing.Image)
            Me.MultButton.Location = New System.Drawing.Point(194, 61)
            Me.MultButton.Name = "MultButton"
            Me.MultButton.Size = New System.Drawing.Size(45, 45)
            Me.MultButton.TabIndex = 20
            Me.MultButton.UseVisualStyleBackColor = False
            '
            'ClearButton
            '
            Me.ClearButton.BackColor = System.Drawing.Color.Gainsboro
            Me.ClearButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ClearButton.Image = CType(resources.GetObject("ClearButton.Image"), System.Drawing.Image)
            Me.ClearButton.Location = New System.Drawing.Point(194, 175)
            Me.ClearButton.Name = "ClearButton"
            Me.ClearButton.Size = New System.Drawing.Size(45, 45)
            Me.ClearButton.TabIndex = 24
            Me.bsFormulaToolTips.SetToolTip(Me.ClearButton, "Clear")
            Me.ClearButton.UseVisualStyleBackColor = False
            '
            'CloseParenthesisButton
            '
            Me.CloseParenthesisButton.BackColor = System.Drawing.Color.Gainsboro
            Me.CloseParenthesisButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CloseParenthesisButton.Image = CType(resources.GetObject("CloseParenthesisButton.Image"), System.Drawing.Image)
            Me.CloseParenthesisButton.Location = New System.Drawing.Point(194, 4)
            Me.CloseParenthesisButton.Name = "CloseParenthesisButton"
            Me.CloseParenthesisButton.Size = New System.Drawing.Size(45, 45)
            Me.CloseParenthesisButton.TabIndex = 18
            Me.CloseParenthesisButton.UseVisualStyleBackColor = False
            '
            'BackButton
            '
            Me.BackButton.BackColor = System.Drawing.Color.Gainsboro
            Me.BackButton.Font = New System.Drawing.Font("Symbol", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
            Me.BackButton.Image = CType(resources.GetObject("BackButton.Image"), System.Drawing.Image)
            Me.BackButton.Location = New System.Drawing.Point(149, 175)
            Me.BackButton.Name = "BackButton"
            Me.BackButton.Size = New System.Drawing.Size(45, 45)
            Me.BackButton.TabIndex = 23
            Me.BackButton.TextAlign = System.Drawing.ContentAlignment.TopCenter
            Me.bsFormulaToolTips.SetToolTip(Me.BackButton, "Delete")
            Me.BackButton.UseVisualStyleBackColor = False
            '
            'OpenParenthesisButton
            '
            Me.OpenParenthesisButton.BackColor = System.Drawing.Color.Gainsboro
            Me.OpenParenthesisButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.OpenParenthesisButton.Image = CType(resources.GetObject("OpenParenthesisButton.Image"), System.Drawing.Image)
            Me.OpenParenthesisButton.Location = New System.Drawing.Point(149, 4)
            Me.OpenParenthesisButton.Name = "OpenParenthesisButton"
            Me.OpenParenthesisButton.Size = New System.Drawing.Size(45, 45)
            Me.OpenParenthesisButton.TabIndex = 17
            Me.OpenParenthesisButton.UseVisualStyleBackColor = False
            '
            'DivisionButton
            '
            Me.DivisionButton.BackColor = System.Drawing.Color.Gainsboro
            Me.DivisionButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.DivisionButton.Image = CType(resources.GetObject("DivisionButton.Image"), System.Drawing.Image)
            Me.DivisionButton.Location = New System.Drawing.Point(149, 61)
            Me.DivisionButton.Name = "DivisionButton"
            Me.DivisionButton.Size = New System.Drawing.Size(45, 45)
            Me.DivisionButton.TabIndex = 19
            Me.DivisionButton.UseVisualStyleBackColor = False
            '
            'AddButton
            '
            Me.AddButton.BackColor = System.Drawing.Color.Gainsboro
            Me.AddButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.AddButton.Image = CType(resources.GetObject("AddButton.Image"), System.Drawing.Image)
            Me.AddButton.Location = New System.Drawing.Point(194, 118)
            Me.AddButton.Name = "AddButton"
            Me.AddButton.Size = New System.Drawing.Size(45, 45)
            Me.AddButton.TabIndex = 22
            Me.AddButton.UseVisualStyleBackColor = False
            '
            'MinusButton
            '
            Me.MinusButton.BackColor = System.Drawing.Color.Gainsboro
            Me.MinusButton.Font = New System.Drawing.Font("Verdana", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.MinusButton.Image = CType(resources.GetObject("MinusButton.Image"), System.Drawing.Image)
            Me.MinusButton.Location = New System.Drawing.Point(149, 118)
            Me.MinusButton.Name = "MinusButton"
            Me.MinusButton.Size = New System.Drawing.Size(45, 45)
            Me.MinusButton.TabIndex = 21
            Me.MinusButton.UseVisualStyleBackColor = False
            '
            'FormulaTextBox
            '
            Me.FormulaTextBox.BackColor = System.Drawing.Color.LightGray
            Me.FormulaTextBox.Font = New System.Drawing.Font("Verdana", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormulaTextBox.ForeColor = System.Drawing.Color.Black
            Me.FormulaTextBox.Location = New System.Drawing.Point(4, 22)
            Me.FormulaTextBox.MaxLength = 255
            Me.FormulaTextBox.Multiline = True
            Me.FormulaTextBox.Name = "FormulaTextBox"
            Me.FormulaTextBox.ReadOnly = True
            Me.FormulaTextBox.Size = New System.Drawing.Size(625, 109)
            Me.FormulaTextBox.TabIndex = 0
            '
            'FormulaLabel
            '
            Me.FormulaLabel.AutoSize = True
            Me.FormulaLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormulaLabel.Location = New System.Drawing.Point(4, 4)
            Me.FormulaLabel.Name = "FormulaLabel"
            Me.FormulaLabel.Size = New System.Drawing.Size(58, 13)
            Me.FormulaLabel.TabIndex = 22
            Me.FormulaLabel.Text = "Formula:"
            '
            'Panel1
            '
            Me.Panel1.BackColor = System.Drawing.Color.Transparent
            Me.Panel1.Controls.Add(Me.bsCalculatedLabel)
            Me.Panel1.Controls.Add(Me.bsStandardLabel)
            Me.Panel1.Controls.Add(Me.bsCalculatedTestListView)
            Me.Panel1.Controls.Add(Me.bsStandardTestListView)
            Me.Panel1.Controls.Add(Me.SampleTypeListComboBox)
            Me.Panel1.Controls.Add(Me.SampleTypeLabel)
            Me.Panel1.Controls.Add(Me.AddSelectTestButton)
            Me.Panel1.Location = New System.Drawing.Point(3, 137)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(428, 224)
            Me.Panel1.TabIndex = 1
            '
            'bsCalculatedLabel
            '
            Me.bsCalculatedLabel.AutoSize = True
            Me.bsCalculatedLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.bsCalculatedLabel.Location = New System.Drawing.Point(217, 48)
            Me.bsCalculatedLabel.Name = "bsCalculatedLabel"
            Me.bsCalculatedLabel.Size = New System.Drawing.Size(67, 13)
            Me.bsCalculatedLabel.TabIndex = 34
            Me.bsCalculatedLabel.Text = "Calculated"
            '
            'bsStandardLabel
            '
            Me.bsStandardLabel.AutoSize = True
            Me.bsStandardLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.bsStandardLabel.Location = New System.Drawing.Point(4, 48)
            Me.bsStandardLabel.Name = "bsStandardLabel"
            Me.bsStandardLabel.Size = New System.Drawing.Size(59, 13)
            Me.bsStandardLabel.TabIndex = 33
            Me.bsStandardLabel.Text = "Standard"
            '
            'bsCalculatedTestListView
            '
            Me.bsCalculatedTestListView.BackColor = System.Drawing.Color.White
            Me.bsCalculatedTestListView.ForeColor = System.Drawing.Color.Black
            Me.bsCalculatedTestListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
            Me.bsCalculatedTestListView.Location = New System.Drawing.Point(217, 66)
            Me.bsCalculatedTestListView.Name = "bsCalculatedTestListView"
            Me.bsCalculatedTestListView.Size = New System.Drawing.Size(208, 154)
            Me.bsCalculatedTestListView.TabIndex = 4
            Me.bsCalculatedTestListView.UseCompatibleStateImageBehavior = False
            Me.bsCalculatedTestListView.View = System.Windows.Forms.View.Details
            '
            'bsStandardTestListView
            '
            Me.bsStandardTestListView.BackColor = System.Drawing.Color.White
            Me.bsStandardTestListView.ForeColor = System.Drawing.Color.Black
            Me.bsStandardTestListView.Location = New System.Drawing.Point(4, 66)
            Me.bsStandardTestListView.Name = "bsStandardTestListView"
            Me.bsStandardTestListView.Size = New System.Drawing.Size(208, 154)
            Me.bsStandardTestListView.TabIndex = 3
            Me.bsStandardTestListView.UseCompatibleStateImageBehavior = False
            '
            'SampleTypeListComboBox
            '
            Me.SampleTypeListComboBox.BackColor = System.Drawing.Color.White
            Me.SampleTypeListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.SampleTypeListComboBox.ForeColor = System.Drawing.Color.Black
            Me.SampleTypeListComboBox.FormattingEnabled = True
            Me.SampleTypeListComboBox.IntegralHeight = False
            Me.SampleTypeListComboBox.Location = New System.Drawing.Point(4, 22)
            Me.SampleTypeListComboBox.Name = "SampleTypeListComboBox"
            Me.SampleTypeListComboBox.Size = New System.Drawing.Size(208, 21)
            Me.SampleTypeListComboBox.TabIndex = 2
            '
            'SampleTypeLabel
            '
            Me.SampleTypeLabel.AutoSize = True
            Me.SampleTypeLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.SampleTypeLabel.Location = New System.Drawing.Point(4, 4)
            Me.SampleTypeLabel.Name = "SampleTypeLabel"
            Me.SampleTypeLabel.Size = New System.Drawing.Size(87, 13)
            Me.SampleTypeLabel.TabIndex = 28
            Me.SampleTypeLabel.Text = "Sample Type:"
            '
            'AddSelectTestButton
            '
            Me.AddSelectTestButton.BackColor = System.Drawing.Color.Gainsboro
            Me.AddSelectTestButton.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.AddSelectTestButton.Location = New System.Drawing.Point(373, 12)
            Me.AddSelectTestButton.Name = "AddSelectTestButton"
            Me.AddSelectTestButton.Size = New System.Drawing.Size(36, 31)
            Me.AddSelectTestButton.TabIndex = 27
            Me.AddSelectTestButton.UseVisualStyleBackColor = False
            Me.AddSelectTestButton.Visible = False
            '
            'FormulaStatusImageList
            '
            Me.FormulaStatusImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
            Me.FormulaStatusImageList.ImageSize = New System.Drawing.Size(40, 40)
            Me.FormulaStatusImageList.TransparentColor = System.Drawing.Color.Transparent
            '
            'FormulaStatusImage
            '
            Me.FormulaStatusImage.BackColor = System.Drawing.Color.Transparent
            Me.FormulaStatusImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
            Me.FormulaStatusImage.Location = New System.Drawing.Point(640, 107)
            Me.FormulaStatusImage.Name = "FormulaStatusImage"
            Me.FormulaStatusImage.Size = New System.Drawing.Size(24, 24)
            Me.FormulaStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
            Me.FormulaStatusImage.TabIndex = 24
            Me.FormulaStatusImage.TabStop = False
            '
            'bsAuxForErrorLabel
            '
            Me.bsAuxForErrorLabel.AutoSize = True
            Me.bsAuxForErrorLabel.BackColor = System.Drawing.Color.LightSteelBlue
            Me.bsAuxForErrorLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
            Me.bsAuxForErrorLabel.ForeColor = System.Drawing.Color.Black
            Me.bsAuxForErrorLabel.Location = New System.Drawing.Point(590, 25)
            Me.bsAuxForErrorLabel.Name = "bsAuxForErrorLabel"
            Me.bsAuxForErrorLabel.Size = New System.Drawing.Size(13, 17)
            Me.bsAuxForErrorLabel.TabIndex = 25
            Me.bsAuxForErrorLabel.Text = " "
            Me.bsAuxForErrorLabel.Title = True
            '
            'bsFormulaErrorProvider
            '
            Me.bsFormulaErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
            Me.bsFormulaErrorProvider.ContainerControl = Me
            '
            'BSFormula
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
            Me.AutoScroll = True
            Me.AutoSize = True
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.BackColor = System.Drawing.Color.Gainsboro
            Me.Controls.Add(Me.FormulaStatusImage)
            Me.Controls.Add(Me.Panel1)
            Me.Controls.Add(Me.FormulaLabel)
            Me.Controls.Add(Me.FormulaTextBox)
            Me.Controls.Add(Me.NumbersPanel)
            Me.Controls.Add(Me.bsAuxForErrorLabel)
            Me.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ForeColor = System.Drawing.Color.Black
            Me.Name = "BSFormula"
            Me.Size = New System.Drawing.Size(681, 364)
            Me.NumbersPanel.ResumeLayout(False)
            Me.Panel1.ResumeLayout(False)
            Me.Panel1.PerformLayout()
            CType(Me.FormulaStatusImage, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.bsFormulaErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents OneButton As System.Windows.Forms.Button
        Friend WithEvents ThreeButton As System.Windows.Forms.Button
        Friend WithEvents SevenButton As System.Windows.Forms.Button
        Friend WithEvents FourButton As System.Windows.Forms.Button
        Friend WithEvents EightButton As System.Windows.Forms.Button
        Friend WithEvents NineButton As System.Windows.Forms.Button
        Friend WithEvents ZeroButton As System.Windows.Forms.Button
        Friend WithEvents FiveButton As System.Windows.Forms.Button
        Friend WithEvents SixButton As System.Windows.Forms.Button
        Friend WithEvents TwoButton As System.Windows.Forms.Button
        Friend WithEvents DotButton As System.Windows.Forms.Button
        Friend WithEvents NumbersPanel As System.Windows.Forms.Panel
        Friend WithEvents FormulaTextBox As System.Windows.Forms.TextBox
        Friend WithEvents FormulaLabel As System.Windows.Forms.Label
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents SampleTypeLabel As System.Windows.Forms.Label
        Friend WithEvents AddSelectTestButton As System.Windows.Forms.Button
        Friend WithEvents FormulaStatusImage As System.Windows.Forms.PictureBox
        Friend WithEvents FormulaStatusImageList As System.Windows.Forms.ImageList
        Friend WithEvents SampleTypeListComboBox As System.Windows.Forms.ComboBox
        Friend WithEvents MultButton As System.Windows.Forms.Button
        Friend WithEvents ClearButton As System.Windows.Forms.Button
        Friend WithEvents CloseParenthesisButton As System.Windows.Forms.Button
        Friend WithEvents BackButton As System.Windows.Forms.Button
        Friend WithEvents OpenParenthesisButton As System.Windows.Forms.Button
        Friend WithEvents DivisionButton As System.Windows.Forms.Button
        Friend WithEvents AddButton As System.Windows.Forms.Button
        Friend WithEvents MinusButton As System.Windows.Forms.Button
        Friend WithEvents bsStandardTestListView As Biosystems.Ax00.Controls.UserControls.BSListView
        Friend WithEvents bsCalculatedTestListView As Biosystems.Ax00.Controls.UserControls.BSListView
        Friend WithEvents bsStandardLabel As System.Windows.Forms.Label
        Friend WithEvents bsCalculatedLabel As System.Windows.Forms.Label
        Friend WithEvents bsFormulaToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
        Friend WithEvents bsFormulaErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Friend WithEvents bsAuxForErrorLabel As Biosystems.Ax00.Controls.UserControls.BSLabel

    End Class
End Namespace