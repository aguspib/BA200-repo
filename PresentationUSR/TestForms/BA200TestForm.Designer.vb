<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BA200TestForm
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
        Me.searchGroup = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.btnPerformBL = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.btnEmptyRotor = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.subtitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.btnFillRotor = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.searchGroup.SuspendLayout()
        Me.SuspendLayout()
        '
        'searchGroup
        '
        Me.searchGroup.Controls.Add(Me.btnPerformBL)
        Me.searchGroup.Controls.Add(Me.btnEmptyRotor)
        Me.searchGroup.Controls.Add(Me.subtitleLabel)
        Me.searchGroup.Controls.Add(Me.btnFillRotor)
        Me.searchGroup.ForeColor = System.Drawing.Color.Black
        Me.searchGroup.Location = New System.Drawing.Point(12, 12)
        Me.searchGroup.Name = "searchGroup"
        Me.searchGroup.Size = New System.Drawing.Size(288, 178)
        Me.searchGroup.TabIndex = 27
        Me.searchGroup.TabStop = False
        '
        'btnPerformBL
        '
        Me.btnPerformBL.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnPerformBL.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPerformBL.Location = New System.Drawing.Point(6, 88)
        Me.btnPerformBL.Margin = New System.Windows.Forms.Padding(3, 3, 3, 15)
        Me.btnPerformBL.Name = "btnPerformBL"
        Me.btnPerformBL.Size = New System.Drawing.Size(276, 32)
        Me.btnPerformBL.TabIndex = 28
        Me.btnPerformBL.Text = "Perform BaseLine (FLIGHT)"
        Me.btnPerformBL.UseVisualStyleBackColor = True
        '
        'btnEmptyRotor
        '
        Me.btnEmptyRotor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnEmptyRotor.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnEmptyRotor.Location = New System.Drawing.Point(6, 138)
        Me.btnEmptyRotor.Name = "btnEmptyRotor"
        Me.btnEmptyRotor.Size = New System.Drawing.Size(276, 32)
        Me.btnEmptyRotor.TabIndex = 27
        Me.btnEmptyRotor.Text = "Empty Rotor (FLIGHT)"
        Me.btnEmptyRotor.UseVisualStyleBackColor = True
        '
        'subtitleLabel
        '
        Me.subtitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.subtitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.subtitleLabel.ForeColor = System.Drawing.Color.Black
        Me.subtitleLabel.Location = New System.Drawing.Point(6, 15)
        Me.subtitleLabel.Name = "subtitleLabel"
        Me.subtitleLabel.Size = New System.Drawing.Size(276, 20)
        Me.subtitleLabel.TabIndex = 26
        Me.subtitleLabel.Text = "*FLIGHT"
        Me.subtitleLabel.Title = True
        '
        'btnFillRotor
        '
        Me.btnFillRotor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnFillRotor.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFillRotor.Location = New System.Drawing.Point(6, 38)
        Me.btnFillRotor.Margin = New System.Windows.Forms.Padding(3, 3, 3, 15)
        Me.btnFillRotor.Name = "btnFillRotor"
        Me.btnFillRotor.Size = New System.Drawing.Size(276, 32)
        Me.btnFillRotor.TabIndex = 4
        Me.btnFillRotor.Text = "Fill Rotor (FLIGHT)"
        Me.btnFillRotor.UseVisualStyleBackColor = True
        '
        'BA200TestForm
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(312, 207)
        Me.Controls.Add(Me.searchGroup)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "BA200TestForm"
        Me.Text = "BA200 Test Form"
        Me.searchGroup.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents searchGroup As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents subtitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents btnFillRotor As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents btnPerformBL As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents btnEmptyRotor As Biosystems.Ax00.Controls.UserControls.BSButton
End Class
