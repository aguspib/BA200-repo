

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BSMonitorPanel
    Inherits System.Windows.Forms.UserControl

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
        Me.LayoutPanel = New System.Windows.Forms.TableLayoutPanel
        Me.SuspendLayout()
        '
        'LayoutPanel
        '
        Me.LayoutPanel.ColumnCount = 2
        Me.LayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.LayoutPanel.Name = "LayoutPanel"
        Me.LayoutPanel.RowCount = 2
        Me.LayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.LayoutPanel.Size = New System.Drawing.Size(893, 250)
        Me.LayoutPanel.TabIndex = 0
        '
        'BSMonitorPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.LayoutPanel)
        Me.Name = "BSMonitorPanel"
        Me.Size = New System.Drawing.Size(893, 250)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents LayoutPanel As System.Windows.Forms.TableLayoutPanel

End Class

