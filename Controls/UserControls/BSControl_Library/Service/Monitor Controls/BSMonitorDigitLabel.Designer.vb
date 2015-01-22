Namespace Biosystems.Ax00.Controls.UserControls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BSMonitorDigitLabel
        Inherits BSMonitorControlBase


        'UserControl overrides dispose to clean up the component list.
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
            Me.InstrumentPanel = New System.Windows.Forms.Panel
            Me.LabelPanel = New System.Windows.Forms.Panel
            Me.UnitsLabel = New System.Windows.Forms.Label
            Me.DigitLabel = New System.Windows.Forms.Label
            Me.InstrumentPanel.SuspendLayout()
            Me.LabelPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            'InstrumentPanel
            '
            Me.InstrumentPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.InstrumentPanel.Controls.Add(Me.LabelPanel)
            Me.InstrumentPanel.Dock = System.Windows.Forms.DockStyle.Top
            Me.InstrumentPanel.Location = New System.Drawing.Point(0, 0)
            Me.InstrumentPanel.Name = "InstrumentPanel"
            Me.InstrumentPanel.Size = New System.Drawing.Size(302, 30)
            Me.InstrumentPanel.TabIndex = 4
            '
            'LabelPanel
            '
            Me.LabelPanel.Controls.Add(Me.UnitsLabel)
            Me.LabelPanel.Controls.Add(Me.DigitLabel)
            Me.LabelPanel.Dock = System.Windows.Forms.DockStyle.Fill
            Me.LabelPanel.Location = New System.Drawing.Point(0, 0)
            Me.LabelPanel.Margin = New System.Windows.Forms.Padding(0)
            Me.LabelPanel.Name = "LabelPanel"
            Me.LabelPanel.Size = New System.Drawing.Size(302, 30)
            Me.LabelPanel.TabIndex = 1
            '
            'UnitsLabel
            '
            Me.UnitsLabel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.UnitsLabel.AutoSize = True
            Me.UnitsLabel.ForeColor = System.Drawing.Color.LimeGreen
            Me.UnitsLabel.Location = New System.Drawing.Point(180, 14)
            Me.UnitsLabel.Name = "UnitsLabel"
            Me.UnitsLabel.Size = New System.Drawing.Size(29, 13)
            Me.UnitsLabel.TabIndex = 1
            Me.UnitsLabel.Text = "units"
            Me.UnitsLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft
            Me.UnitsLabel.Visible = False
            '
            'DigitLabel
            '
            Me.DigitLabel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.DigitLabel.AutoSize = True
            Me.DigitLabel.Font = New System.Drawing.Font("Digiface", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.DigitLabel.ForeColor = System.Drawing.Color.LimeGreen
            Me.DigitLabel.Location = New System.Drawing.Point(123, 0)
            Me.DigitLabel.Margin = New System.Windows.Forms.Padding(0)
            Me.DigitLabel.Name = "DigitLabel"
            Me.DigitLabel.Size = New System.Drawing.Size(56, 29)
            Me.DigitLabel.TabIndex = 0
            Me.DigitLabel.Text = "99.9"
            Me.DigitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'BSMonitorDigitLabel
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.Transparent
            Me.Controls.Add(Me.InstrumentPanel)
            Me.Name = "BSMonitorDigitLabel"
            Me.Size = New System.Drawing.Size(302, 52)
            Me.Controls.SetChildIndex(Me.InstrumentPanel, 0)
            Me.InstrumentPanel.ResumeLayout(False)
            Me.LabelPanel.ResumeLayout(False)
            Me.LabelPanel.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents InstrumentPanel As System.Windows.Forms.Panel
        Friend WithEvents DigitLabel As System.Windows.Forms.Label
        Friend WithEvents LabelPanel As System.Windows.Forms.Panel
        Friend WithEvents UnitsLabel As System.Windows.Forms.Label

        Protected Friend Overrides Sub RefreshControl()
            UpdateContentsSize(InstrumentPanel)
        End Sub
    End Class

End Namespace