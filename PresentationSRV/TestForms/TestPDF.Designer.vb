<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TestPDF
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
        Me.Button1 = New System.Windows.Forms.Button
        Me.BsRichTextBox1 = New Biosystems.Ax00.Controls.UserControls.BSRichTextBox
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(7, 4)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(106, 42)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'BsRichTextBox1
        '
        Me.BsRichTextBox1.Location = New System.Drawing.Point(571, 80)
        Me.BsRichTextBox1.Name = "BsRichTextBox1"
        Me.BsRichTextBox1.Size = New System.Drawing.Size(320, 458)
        Me.BsRichTextBox1.TabIndex = 2
        Me.BsRichTextBox1.Text = ""
        '
        'TestPDF
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(903, 597)
        Me.Controls.Add(Me.BsRichTextBox1)
        Me.Controls.Add(Me.Button1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "TestPDF"
        Me.Padding = New System.Windows.Forms.Padding(9)
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "TestPDF"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents BsRichTextBox1 As Biosystems.Ax00.Controls.UserControls.BSRichTextBox


End Class
