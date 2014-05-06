<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InstallerForm
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
        Me.bwPrecarga = New System.ComponentModel.BackgroundWorker()
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Varios = New System.Windows.Forms.Button()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.SaveButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.CancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.EditButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.Button7 = New System.Windows.Forms.Button()
        Me.BsButton1 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.Button8 = New System.Windows.Forms.Button()
        Me.BsButton2 = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.Button9 = New System.Windows.Forms.Button()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RichTextBox1
        '
        Me.RichTextBox1.Location = New System.Drawing.Point(0, 125)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.Size = New System.Drawing.Size(450, 488)
        Me.RichTextBox1.TabIndex = 0
        Me.RichTextBox1.Text = ""
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(12, 96)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(142, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "DecodeXMLNotification"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Varios
        '
        Me.Varios.Location = New System.Drawing.Point(456, 96)
        Me.Varios.Name = "Varios"
        Me.Varios.Size = New System.Drawing.Size(78, 23)
        Me.Varios.TabIndex = 2
        Me.Varios.Text = "Get LIS Values"
        Me.Varios.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(456, 125)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(562, 150)
        Me.DataGridView1.TabIndex = 3
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(570, 96)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "Save LIS"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'SaveButton
        '
        Me.SaveButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.SaveButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.SaveButton.Location = New System.Drawing.Point(977, 569)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(32, 32)
        Me.SaveButton.TabIndex = 178
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'CancelButton
        '
        Me.CancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.CancelButton.Location = New System.Drawing.Point(1015, 569)
        Me.CancelButton.Name = "CancelButton"
        Me.CancelButton.Size = New System.Drawing.Size(32, 32)
        Me.CancelButton.TabIndex = 177
        Me.CancelButton.UseVisualStyleBackColor = True
        '
        'EditButton
        '
        Me.EditButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.EditButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.EditButton.Location = New System.Drawing.Point(939, 569)
        Me.EditButton.Name = "EditButton"
        Me.EditButton.Size = New System.Drawing.Size(32, 32)
        Me.EditButton.TabIndex = 176
        Me.EditButton.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(457, 282)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 23)
        Me.Button3.TabIndex = 179
        Me.Button3.Text = "Load DS"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(570, 282)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(75, 35)
        Me.Button4.TabIndex = 180
        Me.Button4.Text = "Change LIS Status"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(651, 282)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(75, 35)
        Me.Button5.TabIndex = 181
        Me.Button5.Text = "Cancel AWOSID"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(733, 282)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(166, 35)
        Me.Button6.TabIndex = 182
        Me.Button6.Text = "ProcessLISPatientOTs"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'Button7
        '
        Me.Button7.Location = New System.Drawing.Point(457, 348)
        Me.Button7.Name = "Button7"
        Me.Button7.Size = New System.Drawing.Size(105, 39)
        Me.Button7.TabIndex = 183
        Me.Button7.Text = "Save Synapse ActivityLog"
        Me.Button7.UseVisualStyleBackColor = True
        '
        'BsButton1
        '
        Me.BsButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton1.Location = New System.Drawing.Point(457, 413)
        Me.BsButton1.Name = "BsButton1"
        Me.BsButton1.Size = New System.Drawing.Size(105, 32)
        Me.BsButton1.TabIndex = 184
        Me.BsButton1.Text = "Read Registry"
        Me.BsButton1.UseVisualStyleBackColor = True
        '
        'Button8
        '
        Me.Button8.Location = New System.Drawing.Point(570, 359)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(75, 23)
        Me.Button8.TabIndex = 185
        Me.Button8.Text = "Button8"
        Me.Button8.UseVisualStyleBackColor = True
        '
        'BsButton2
        '
        Me.BsButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton2.Location = New System.Drawing.Point(685, 359)
        Me.BsButton2.Name = "BsButton2"
        Me.BsButton2.Size = New System.Drawing.Size(180, 32)
        Me.BsButton2.TabIndex = 186
        Me.BsButton2.Text = "BsButton2"
        Me.BsButton2.UseVisualStyleBackColor = True
        '
        'Button9
        '
        Me.Button9.Location = New System.Drawing.Point(459, 462)
        Me.Button9.Name = "Button9"
        Me.Button9.Size = New System.Drawing.Size(75, 23)
        Me.Button9.TabIndex = 187
        Me.Button9.Text = "Button9"
        Me.Button9.UseVisualStyleBackColor = True
        '
        'InstallerForm
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1059, 613)
        Me.Controls.Add(Me.Button9)
        Me.Controls.Add(Me.BsButton2)
        Me.Controls.Add(Me.Button8)
        Me.Controls.Add(Me.BsButton1)
        Me.Controls.Add(Me.Button7)
        Me.Controls.Add(Me.Button6)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.SaveButton)
        Me.Controls.Add(Me.CancelButton)
        Me.Controls.Add(Me.EditButton)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.Varios)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.RichTextBox1)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "InstallerForm"
        Me.Text = "Installer Test Form"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bwPrecarga As System.ComponentModel.BackgroundWorker
    Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Varios As System.Windows.Forms.Button
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents SaveButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents CancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents EditButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Friend WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents BsButton1 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Button8 As System.Windows.Forms.Button
    Friend WithEvents BsButton2 As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents Button9 As System.Windows.Forms.Button
End Class
