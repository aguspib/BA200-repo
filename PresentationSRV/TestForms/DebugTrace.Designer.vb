Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class DebugTrace
    Inherits Form

    'Form overrides dispose to clean up the component list.
    <DebuggerNonUserCode()> _
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
    Private components As IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ConsoleLabel = New Label
        Me.SuspendLayout()
        '
        'ConsoleLabel
        '
        Me.ConsoleLabel.BackColor = Color.Black
        Me.ConsoleLabel.Dock = DockStyle.Fill
        Me.ConsoleLabel.ForeColor = Color.White
        Me.ConsoleLabel.Location = New Point(0, 0)
        Me.ConsoleLabel.Name = "ConsoleLabel"
        Me.ConsoleLabel.Size = New Size(319, 548)
        Me.ConsoleLabel.TabIndex = 0
        Me.ConsoleLabel.Text = "DEBUG CONSOLE"
        '
        'DebugConsole
        '
        Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(319, 548)
        Me.Controls.Add(Me.ConsoleLabel)
        Me.FormBorderStyle = FormBorderStyle.SizableToolWindow
        Me.Name = "DebugConsole"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Tag = "DebugConsole"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ConsoleLabel As Label
End Class
