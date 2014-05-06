Namespace Biosystems.Ax00.Controls.UserControls


    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BSMonitorControlBase
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
            Me.TitleLabel = New System.Windows.Forms.Label
            Me.SuspendLayout()
            '
            'TitleLabel
            '
            Me.TitleLabel.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.TitleLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TitleLabel.ForeColor = System.Drawing.Color.Black
            Me.TitleLabel.Location = New System.Drawing.Point(0, 173)
            Me.TitleLabel.Name = "TitleLabel"
            Me.TitleLabel.Size = New System.Drawing.Size(193, 20)
            Me.TitleLabel.TabIndex = 3
            Me.TitleLabel.Text = "Control Title"
            Me.TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'BSMonitorControlBase
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.Gainsboro
            Me.Controls.Add(Me.TitleLabel)
            Me.Name = "BSMonitorControlBase"
            Me.Size = New System.Drawing.Size(193, 193)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents TitleLabel As System.Windows.Forms.Label

    End Class

End Namespace
