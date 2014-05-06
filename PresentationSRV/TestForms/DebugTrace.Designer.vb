<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DebugTrace
    Inherits System.Windows.Forms.Form

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
        Me.ConsoleLabel = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'ConsoleLabel
        '
        Me.ConsoleLabel.BackColor = System.Drawing.Color.Black
        Me.ConsoleLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ConsoleLabel.ForeColor = System.Drawing.Color.White
        Me.ConsoleLabel.Location = New System.Drawing.Point(0, 0)
        Me.ConsoleLabel.Name = "ConsoleLabel"
        Me.ConsoleLabel.Size = New System.Drawing.Size(319, 548)
        Me.ConsoleLabel.TabIndex = 0
        Me.ConsoleLabel.Text = "DEBUG CONSOLE"
        '
        'DebugConsole
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(319, 548)
        Me.Controls.Add(Me.ConsoleLabel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Name = "DebugConsole"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Tag = "DebugConsole"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ConsoleLabel As System.Windows.Forms.Label
End Class
