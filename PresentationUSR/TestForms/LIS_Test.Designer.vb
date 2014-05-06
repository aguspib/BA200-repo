<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LIS_Test
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
        Me.EncodeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.BsGroupBox1 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.WrapperCheckBox = New System.Windows.Forms.CheckBox()
        Me.BsButton2 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.DecodeComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.BsButton3 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsButton1 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.myTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsButton4 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsButton5 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsButton6 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsButton7 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsButton8 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.lblCreateFileCount = New System.Windows.Forms.Label()
        Me.lblDeleteFileCount = New System.Windows.Forms.Label()
        Me.BsGroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'EncodeComboBox
        '
        Me.EncodeComboBox.FormattingEnabled = True
        Me.EncodeComboBox.Location = New System.Drawing.Point(17, 20)
        Me.EncodeComboBox.Name = "EncodeComboBox"
        Me.EncodeComboBox.Size = New System.Drawing.Size(204, 21)
        Me.EncodeComboBox.TabIndex = 0
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsGroupBox1.Controls.Add(Me.WrapperCheckBox)
        Me.BsGroupBox1.Controls.Add(Me.BsButton2)
        Me.BsGroupBox1.Controls.Add(Me.DecodeComboBox)
        Me.BsGroupBox1.Controls.Add(Me.BsButton3)
        Me.BsGroupBox1.Controls.Add(Me.BsButton1)
        Me.BsGroupBox1.Controls.Add(Me.myTextBox)
        Me.BsGroupBox1.Controls.Add(Me.EncodeComboBox)
        Me.BsGroupBox1.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New System.Drawing.Size(875, 567)
        Me.BsGroupBox1.TabIndex = 2
        Me.BsGroupBox1.TabStop = False
        Me.BsGroupBox1.Text = "LIS Xml Translator"
        '
        'WrapperCheckBox
        '
        Me.WrapperCheckBox.AutoSize = True
        Me.WrapperCheckBox.Location = New System.Drawing.Point(20, 48)
        Me.WrapperCheckBox.Name = "WrapperCheckBox"
        Me.WrapperCheckBox.Size = New System.Drawing.Size(75, 17)
        Me.WrapperCheckBox.TabIndex = 8
        Me.WrapperCheckBox.Text = "Wrapper"
        Me.WrapperCheckBox.UseVisualStyleBackColor = True
        '
        'BsButton2
        '
        Me.BsButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton2.Location = New System.Drawing.Point(705, 13)
        Me.BsButton2.Name = "BsButton2"
        Me.BsButton2.Size = New System.Drawing.Size(152, 32)
        Me.BsButton2.TabIndex = 7
        Me.BsButton2.Text = "Cancel AWOSId"
        Me.BsButton2.UseVisualStyleBackColor = True
        '
        'DecodeComboBox
        '
        Me.DecodeComboBox.FormattingEnabled = True
        Me.DecodeComboBox.Location = New System.Drawing.Point(354, 20)
        Me.DecodeComboBox.Name = "DecodeComboBox"
        Me.DecodeComboBox.Size = New System.Drawing.Size(204, 21)
        Me.DecodeComboBox.TabIndex = 6
        '
        'BsButton3
        '
        Me.BsButton3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton3.Location = New System.Drawing.Point(564, 13)
        Me.BsButton3.Name = "BsButton3"
        Me.BsButton3.Size = New System.Drawing.Size(117, 32)
        Me.BsButton3.TabIndex = 4
        Me.BsButton3.Text = "Decode Message"
        Me.BsButton3.UseVisualStyleBackColor = True
        '
        'BsButton1
        '
        Me.BsButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton1.Location = New System.Drawing.Point(227, 13)
        Me.BsButton1.Name = "BsButton1"
        Me.BsButton1.Size = New System.Drawing.Size(111, 32)
        Me.BsButton1.TabIndex = 2
        Me.BsButton1.Text = "Create Message"
        Me.BsButton1.UseVisualStyleBackColor = True
        '
        'myTextBox
        '
        Me.myTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.myTextBox.BackColor = System.Drawing.Color.White
        Me.myTextBox.DecimalsValues = False
        Me.myTextBox.Font = New System.Drawing.Font("Courier New", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
        Me.myTextBox.IsNumeric = False
        Me.myTextBox.Location = New System.Drawing.Point(17, 71)
        Me.myTextBox.Mandatory = False
        Me.myTextBox.Multiline = True
        Me.myTextBox.Name = "myTextBox"
        Me.myTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.myTextBox.Size = New System.Drawing.Size(840, 475)
        Me.myTextBox.TabIndex = 1
        Me.myTextBox.WordWrap = False
        '
        'BsButton4
        '
        Me.BsButton4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton4.Location = New System.Drawing.Point(12, 585)
        Me.BsButton4.Name = "BsButton4"
        Me.BsButton4.Size = New System.Drawing.Size(111, 32)
        Me.BsButton4.TabIndex = 3
        Me.BsButton4.Text = "Create Message"
        Me.BsButton4.UseVisualStyleBackColor = True
        '
        'BsButton5
        '
        Me.BsButton5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton5.Location = New System.Drawing.Point(758, 585)
        Me.BsButton5.Name = "BsButton5"
        Me.BsButton5.Size = New System.Drawing.Size(111, 32)
        Me.BsButton5.TabIndex = 4
        Me.BsButton5.Text = "Rerun Test"
        Me.BsButton5.UseVisualStyleBackColor = True
        '
        'BsButton6
        '
        Me.BsButton6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton6.Location = New System.Drawing.Point(194, 585)
        Me.BsButton6.Name = "BsButton6"
        Me.BsButton6.Size = New System.Drawing.Size(111, 32)
        Me.BsButton6.TabIndex = 5
        Me.BsButton6.Text = "Log Start"
        Me.BsButton6.UseVisualStyleBackColor = True
        '
        'BsButton7
        '
        Me.BsButton7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton7.Location = New System.Drawing.Point(366, 585)
        Me.BsButton7.Name = "BsButton7"
        Me.BsButton7.Size = New System.Drawing.Size(111, 32)
        Me.BsButton7.TabIndex = 6
        Me.BsButton7.Text = "Log Stop"
        Me.BsButton7.UseVisualStyleBackColor = True
        '
        'BsButton8
        '
        Me.BsButton8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton8.Location = New System.Drawing.Point(500, 584)
        Me.BsButton8.Name = "BsButton8"
        Me.BsButton8.Size = New System.Drawing.Size(111, 32)
        Me.BsButton8.TabIndex = 7
        Me.BsButton8.Text = "File Watcher"
        Me.BsButton8.UseVisualStyleBackColor = True
        '
        'lblCreateFileCount
        '
        Me.lblCreateFileCount.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCreateFileCount.Location = New System.Drawing.Point(617, 587)
        Me.lblCreateFileCount.Name = "lblCreateFileCount"
        Me.lblCreateFileCount.Size = New System.Drawing.Size(59, 25)
        Me.lblCreateFileCount.TabIndex = 8
        Me.lblCreateFileCount.Text = "0"
        Me.lblCreateFileCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblDeleteFileCount
        '
        Me.lblDeleteFileCount.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDeleteFileCount.Location = New System.Drawing.Point(682, 587)
        Me.lblDeleteFileCount.Name = "lblDeleteFileCount"
        Me.lblDeleteFileCount.Size = New System.Drawing.Size(59, 25)
        Me.lblDeleteFileCount.TabIndex = 9
        Me.lblDeleteFileCount.Text = "0"
        Me.lblDeleteFileCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Form_Sergio
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(899, 619)
        Me.Controls.Add(Me.lblDeleteFileCount)
        Me.Controls.Add(Me.lblCreateFileCount)
        Me.Controls.Add(Me.BsButton8)
        Me.Controls.Add(Me.BsButton7)
        Me.Controls.Add(Me.BsButton6)
        Me.Controls.Add(Me.BsButton5)
        Me.Controls.Add(Me.BsButton4)
        Me.Controls.Add(Me.BsGroupBox1)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "Form_Sergio"
        Me.Text = "Form_Sergio"
        Me.BsGroupBox1.ResumeLayout(False)
        Me.BsGroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents EncodeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsGroupBox1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents BsButton1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents myTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsButton3 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents DecodeComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsButton2 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsButton4 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents WrapperCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents BsButton5 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsButton6 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsButton7 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsButton8 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents lblCreateFileCount As System.Windows.Forms.Label
    Friend WithEvents lblDeleteFileCount As System.Windows.Forms.Label
End Class
