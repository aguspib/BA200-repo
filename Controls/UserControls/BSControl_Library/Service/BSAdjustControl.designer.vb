Namespace Biosystems.Ax00.Controls.UserControls


    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BSAdjustControl
        Inherits System.Windows.Forms.UserControl

        'UserControl1 overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BSAdjustControl))
            Me.BSAdjustButtonsPanel = New System.Windows.Forms.Panel
            Me.BSStepButton = New System.Windows.Forms.Button
            Me.BSEnterButton = New System.Windows.Forms.Button
            Me.BSIncreaseButton = New System.Windows.Forms.Button
            Me.BSDecreaseButton = New System.Windows.Forms.Button
            Me.BSHomeButton = New System.Windows.Forms.Button
            Me.BSDisplayValuesPanel = New System.Windows.Forms.Panel
            Me.BSDisplayPanel = New System.Windows.Forms.Panel
            Me.BSUnitsLabel = New System.Windows.Forms.Label
            Me.BSDisplayTextBox = New System.Windows.Forms.TextBox
            Me.BSInfoPanel = New System.Windows.Forms.Panel
            Me.BsRangePanel = New System.Windows.Forms.Panel
            Me.BSRangeValuesTitle = New System.Windows.Forms.Label
            Me.BSRangeValueLabel = New System.Windows.Forms.Label
            Me.BSLastValuePanel = New System.Windows.Forms.Panel
            Me.BSLastValueTitle = New System.Windows.Forms.Label
            Me.BSLastValueLabel = New System.Windows.Forms.Label
            Me.BSToolTip = New System.Windows.Forms.ToolTip(Me.components)
            Me.BsButtonsImageList = New System.Windows.Forms.ImageList(Me.components)
            Me.BSAdjustButtonsPanel.SuspendLayout()
            Me.BSDisplayValuesPanel.SuspendLayout()
            Me.BSDisplayPanel.SuspendLayout()
            Me.BSInfoPanel.SuspendLayout()
            Me.BsRangePanel.SuspendLayout()
            Me.BSLastValuePanel.SuspendLayout()
            Me.SuspendLayout()
            '
            'BSAdjustButtonsPanel
            '
            Me.BSAdjustButtonsPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSAdjustButtonsPanel.BackColor = System.Drawing.Color.DarkGray
            Me.BSAdjustButtonsPanel.Controls.Add(Me.BSStepButton)
            Me.BSAdjustButtonsPanel.Controls.Add(Me.BSEnterButton)
            Me.BSAdjustButtonsPanel.Controls.Add(Me.BSIncreaseButton)
            Me.BSAdjustButtonsPanel.Controls.Add(Me.BSDecreaseButton)
            Me.BSAdjustButtonsPanel.Controls.Add(Me.BSHomeButton)
            Me.BSAdjustButtonsPanel.Location = New System.Drawing.Point(3, 90)
            Me.BSAdjustButtonsPanel.Name = "BSAdjustButtonsPanel"
            Me.BSAdjustButtonsPanel.Size = New System.Drawing.Size(147, 34)
            Me.BSAdjustButtonsPanel.TabIndex = 1
            '
            'BSStepButton
            '
            Me.BSStepButton.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSStepButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.BSStepButton.Font = New System.Drawing.Font("Verdana", 7.0!, System.Drawing.FontStyle.Bold)
            Me.BSStepButton.ForeColor = System.Drawing.SystemColors.Desktop
            Me.BSStepButton.Location = New System.Drawing.Point(28, 1)
            Me.BSStepButton.Name = "BSStepButton"
            Me.BSStepButton.Size = New System.Drawing.Size(36, 31)
            Me.BSStepButton.TabIndex = 5
            Me.BSStepButton.Text = "100"
            Me.BSStepButton.UseVisualStyleBackColor = True
            '
            'BSEnterButton
            '
            Me.BSEnterButton.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSEnterButton.BackgroundImage = CType(resources.GetObject("BSEnterButton.BackgroundImage"), System.Drawing.Image)
            Me.BSEnterButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.BSEnterButton.DialogResult = System.Windows.Forms.DialogResult.Ignore
            Me.BSEnterButton.Enabled = False
            Me.BSEnterButton.Location = New System.Drawing.Point(116, 1)
            Me.BSEnterButton.Name = "BSEnterButton"
            Me.BSEnterButton.Size = New System.Drawing.Size(29, 31)
            Me.BSEnterButton.TabIndex = 3
            Me.BSEnterButton.UseVisualStyleBackColor = True
            '
            'BSIncreaseButton
            '
            Me.BSIncreaseButton.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSIncreaseButton.BackgroundImage = CType(resources.GetObject("BSIncreaseButton.BackgroundImage"), System.Drawing.Image)
            Me.BSIncreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.BSIncreaseButton.Location = New System.Drawing.Point(89, 1)
            Me.BSIncreaseButton.Name = "BSIncreaseButton"
            Me.BSIncreaseButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes
            Me.BSIncreaseButton.Size = New System.Drawing.Size(29, 31)
            Me.BSIncreaseButton.TabIndex = 2
            Me.BSIncreaseButton.UseVisualStyleBackColor = True
            '
            'BSDecreaseButton
            '
            Me.BSDecreaseButton.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSDecreaseButton.AutoSize = True
            Me.BSDecreaseButton.BackgroundImage = CType(resources.GetObject("BSDecreaseButton.BackgroundImage"), System.Drawing.Image)
            Me.BSDecreaseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.BSDecreaseButton.Location = New System.Drawing.Point(62, 1)
            Me.BSDecreaseButton.Name = "BSDecreaseButton"
            Me.BSDecreaseButton.Size = New System.Drawing.Size(29, 31)
            Me.BSDecreaseButton.TabIndex = 1
            Me.BSDecreaseButton.UseVisualStyleBackColor = True
            '
            'BSHomeButton
            '
            Me.BSHomeButton.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSHomeButton.AutoSize = True
            Me.BSHomeButton.BackgroundImage = CType(resources.GetObject("BSHomeButton.BackgroundImage"), System.Drawing.Image)
            Me.BSHomeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
            Me.BSHomeButton.Location = New System.Drawing.Point(1, 1)
            Me.BSHomeButton.Name = "BSHomeButton"
            Me.BSHomeButton.Size = New System.Drawing.Size(29, 31)
            Me.BSHomeButton.TabIndex = 0
            Me.BSHomeButton.UseVisualStyleBackColor = True
            '
            'BSDisplayValuesPanel
            '
            Me.BSDisplayValuesPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSDisplayValuesPanel.BackColor = System.Drawing.Color.LightSteelBlue
            Me.BSDisplayValuesPanel.Controls.Add(Me.BSDisplayPanel)
            Me.BSDisplayValuesPanel.Controls.Add(Me.BSInfoPanel)
            Me.BSDisplayValuesPanel.Location = New System.Drawing.Point(3, 3)
            Me.BSDisplayValuesPanel.Name = "BSDisplayValuesPanel"
            Me.BSDisplayValuesPanel.Size = New System.Drawing.Size(147, 85)
            Me.BSDisplayValuesPanel.TabIndex = 0
            '
            'BSDisplayPanel
            '
            Me.BSDisplayPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSDisplayPanel.BackColor = System.Drawing.Color.Black
            Me.BSDisplayPanel.Controls.Add(Me.BSUnitsLabel)
            Me.BSDisplayPanel.Controls.Add(Me.BSDisplayTextBox)
            Me.BSDisplayPanel.Location = New System.Drawing.Point(3, 43)
            Me.BSDisplayPanel.Name = "BSDisplayPanel"
            Me.BSDisplayPanel.Size = New System.Drawing.Size(140, 39)
            Me.BSDisplayPanel.TabIndex = 4
            '
            'BSUnitsLabel
            '
            Me.BSUnitsLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSUnitsLabel.BackColor = System.Drawing.Color.Transparent
            Me.BSUnitsLabel.ForeColor = System.Drawing.Color.LightGreen
            Me.BSUnitsLabel.Location = New System.Drawing.Point(105, 23)
            Me.BSUnitsLabel.Name = "BSUnitsLabel"
            Me.BSUnitsLabel.Size = New System.Drawing.Size(37, 13)
            Me.BSUnitsLabel.TabIndex = 1
            Me.BSUnitsLabel.Text = "units"
            Me.BSUnitsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'BSDisplayTextBox
            '
            Me.BSDisplayTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSDisplayTextBox.BackColor = System.Drawing.Color.Black
            Me.BSDisplayTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.BSDisplayTextBox.Font = New System.Drawing.Font("Digiface", 26.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BSDisplayTextBox.ForeColor = System.Drawing.Color.LightGreen
            Me.BSDisplayTextBox.Location = New System.Drawing.Point(0, 0)
            Me.BSDisplayTextBox.Name = "BSDisplayTextBox"
            Me.BSDisplayTextBox.ReadOnly = True
            Me.BSDisplayTextBox.ShortcutsEnabled = False
            Me.BSDisplayTextBox.Size = New System.Drawing.Size(105, 43)
            Me.BSDisplayTextBox.TabIndex = 6
            Me.BSDisplayTextBox.TabStop = False
            Me.BSDisplayTextBox.Text = "0.000"
            Me.BSDisplayTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'BSInfoPanel
            '
            Me.BSInfoPanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSInfoPanel.BackColor = System.Drawing.Color.LightSteelBlue
            Me.BSInfoPanel.Controls.Add(Me.BsRangePanel)
            Me.BSInfoPanel.Controls.Add(Me.BSLastValuePanel)
            Me.BSInfoPanel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BSInfoPanel.Location = New System.Drawing.Point(3, 3)
            Me.BSInfoPanel.Name = "BSInfoPanel"
            Me.BSInfoPanel.Size = New System.Drawing.Size(140, 40)
            Me.BSInfoPanel.TabIndex = 3
            '
            'BsRangePanel
            '
            Me.BsRangePanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsRangePanel.BackColor = System.Drawing.Color.LightSteelBlue
            Me.BsRangePanel.Controls.Add(Me.BSRangeValuesTitle)
            Me.BsRangePanel.Controls.Add(Me.BSRangeValueLabel)
            Me.BsRangePanel.ForeColor = System.Drawing.Color.Black
            Me.BsRangePanel.Location = New System.Drawing.Point(0, 0)
            Me.BsRangePanel.Name = "BsRangePanel"
            Me.BsRangePanel.Size = New System.Drawing.Size(140, 18)
            Me.BsRangePanel.TabIndex = 8
            '
            'BSRangeValuesTitle
            '
            Me.BSRangeValuesTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.BSRangeValuesTitle.AutoSize = True
            Me.BSRangeValuesTitle.BackColor = System.Drawing.Color.LightSteelBlue
            Me.BSRangeValuesTitle.Font = New System.Drawing.Font("Verdana", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BSRangeValuesTitle.ForeColor = System.Drawing.Color.Black
            Me.BSRangeValuesTitle.Location = New System.Drawing.Point(0, 0)
            Me.BSRangeValuesTitle.Name = "BSRangeValuesTitle"
            Me.BSRangeValuesTitle.Size = New System.Drawing.Size(40, 12)
            Me.BSRangeValuesTitle.TabIndex = 6
            Me.BSRangeValuesTitle.Text = "Range:"
            Me.BSRangeValuesTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'BSRangeValueLabel
            '
            Me.BSRangeValueLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSRangeValueLabel.BackColor = System.Drawing.Color.LightSteelBlue
            Me.BSRangeValueLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BSRangeValueLabel.ForeColor = System.Drawing.Color.Black
            Me.BSRangeValueLabel.Location = New System.Drawing.Point(26, -2)
            Me.BSRangeValueLabel.Name = "BSRangeValueLabel"
            Me.BSRangeValueLabel.Size = New System.Drawing.Size(115, 19)
            Me.BSRangeValueLabel.TabIndex = 5
            Me.BSRangeValueLabel.Text = "---- / ----"
            Me.BSRangeValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'BSLastValuePanel
            '
            Me.BSLastValuePanel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSLastValuePanel.BackColor = System.Drawing.Color.LightSteelBlue
            Me.BSLastValuePanel.Controls.Add(Me.BSLastValueTitle)
            Me.BSLastValuePanel.Controls.Add(Me.BSLastValueLabel)
            Me.BSLastValuePanel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BSLastValuePanel.Location = New System.Drawing.Point(0, 20)
            Me.BSLastValuePanel.Name = "BSLastValuePanel"
            Me.BSLastValuePanel.Size = New System.Drawing.Size(140, 20)
            Me.BSLastValuePanel.TabIndex = 7
            '
            'BSLastValueTitle
            '
            Me.BSLastValueTitle.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.BSLastValueTitle.BackColor = System.Drawing.Color.LightSteelBlue
            Me.BSLastValueTitle.Font = New System.Drawing.Font("Verdana", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BSLastValueTitle.ForeColor = System.Drawing.Color.Black
            Me.BSLastValueTitle.Location = New System.Drawing.Point(0, 0)
            Me.BSLastValueTitle.Name = "BSLastValueTitle"
            Me.BSLastValueTitle.Size = New System.Drawing.Size(70, 14)
            Me.BSLastValueTitle.TabIndex = 2
            Me.BSLastValueTitle.Text = "Last Value:"
            Me.BSLastValueTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'BSLastValueLabel
            '
            Me.BSLastValueLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BSLastValueLabel.BackColor = System.Drawing.Color.LightSteelBlue
            Me.BSLastValueLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BSLastValueLabel.ForeColor = System.Drawing.Color.Black
            Me.BSLastValueLabel.Location = New System.Drawing.Point(63, 4)
            Me.BSLastValueLabel.Name = "BSLastValueLabel"
            Me.BSLastValueLabel.Size = New System.Drawing.Size(74, 13)
            Me.BSLastValueLabel.TabIndex = 3
            Me.BSLastValueLabel.Text = "----"
            Me.BSLastValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'BsButtonsImageList
            '
            Me.BsButtonsImageList.ImageStream = CType(resources.GetObject("BsButtonsImageList.ImageStream"), System.Windows.Forms.ImageListStreamer)
            Me.BsButtonsImageList.TransparentColor = System.Drawing.Color.Transparent
            Me.BsButtonsImageList.Images.SetKeyName(0, "IMG_HOME")
            Me.BsButtonsImageList.Images.SetKeyName(1, "IMG_HOME_DIS")
            Me.BsButtonsImageList.Images.SetKeyName(2, "IMG_LEFT")
            Me.BsButtonsImageList.Images.SetKeyName(3, "IMG_LEFT_DIS")
            Me.BsButtonsImageList.Images.SetKeyName(4, "IMG_RIGHT")
            Me.BsButtonsImageList.Images.SetKeyName(5, "IMG_RIGHT_DIS")
            Me.BsButtonsImageList.Images.SetKeyName(6, "IMG_DOWN")
            Me.BsButtonsImageList.Images.SetKeyName(7, "IMG_DOWN_DIS")
            Me.BsButtonsImageList.Images.SetKeyName(8, "IMG_UP")
            Me.BsButtonsImageList.Images.SetKeyName(9, "IMG_UP_DIS")
            Me.BsButtonsImageList.Images.SetKeyName(10, "IMG_ENTER")
            Me.BsButtonsImageList.Images.SetKeyName(11, "IMG_ENTER_DIS")
            '
            'BSAdjustControl
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.Gainsboro
            Me.Controls.Add(Me.BSAdjustButtonsPanel)
            Me.Controls.Add(Me.BSDisplayValuesPanel)
            Me.Enabled = False
            Me.MaximumSize = New System.Drawing.Size(153, 127)
            Me.MinimumSize = New System.Drawing.Size(153, 87)
            Me.Name = "BSAdjustControl"
            Me.Size = New System.Drawing.Size(153, 127)
            Me.BSAdjustButtonsPanel.ResumeLayout(False)
            Me.BSAdjustButtonsPanel.PerformLayout()
            Me.BSDisplayValuesPanel.ResumeLayout(False)
            Me.BSDisplayPanel.ResumeLayout(False)
            Me.BSDisplayPanel.PerformLayout()
            Me.BSInfoPanel.ResumeLayout(False)
            Me.BsRangePanel.ResumeLayout(False)
            Me.BsRangePanel.PerformLayout()
            Me.BSLastValuePanel.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents BSAdjustButtonsPanel As System.Windows.Forms.Panel
        Friend WithEvents BSEnterButton As System.Windows.Forms.Button
        Friend WithEvents BSIncreaseButton As System.Windows.Forms.Button
        Friend WithEvents BSDecreaseButton As System.Windows.Forms.Button
        Friend WithEvents BSHomeButton As System.Windows.Forms.Button
        Friend WithEvents BSDisplayValuesPanel As System.Windows.Forms.Panel
        Friend WithEvents BSUnitsLabel As System.Windows.Forms.Label
        Friend WithEvents BSInfoPanel As System.Windows.Forms.Panel
        Friend WithEvents BSLastValueTitle As System.Windows.Forms.Label
        Friend WithEvents BSRangeValueLabel As System.Windows.Forms.Label
        Friend WithEvents BSLastValueLabel As System.Windows.Forms.Label
        Friend WithEvents BSDisplayPanel As System.Windows.Forms.Panel
        Friend WithEvents BSRangeValuesTitle As System.Windows.Forms.Label
        Friend WithEvents BSToolTip As System.Windows.Forms.ToolTip
        Friend WithEvents BSDisplayTextBox As System.Windows.Forms.TextBox
        Friend WithEvents BSLastValuePanel As System.Windows.Forms.Panel
        Friend WithEvents BsRangePanel As System.Windows.Forms.Panel
        Friend WithEvents BSStepButton As System.Windows.Forms.Button
        Friend WithEvents BsButtonsImageList As System.Windows.Forms.ImageList

    End Class
End Namespace