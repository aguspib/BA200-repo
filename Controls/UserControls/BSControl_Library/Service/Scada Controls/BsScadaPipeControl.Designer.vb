

Namespace Biosystems.Ax00.Controls.UserControls


    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BsScadaPipeControl
        Inherits BsScadaControl

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
            Me.BsEnd2 = New System.Windows.Forms.Panel
            Me.BsEnd1 = New System.Windows.Forms.Panel
            Me.BsOuter2 = New System.Windows.Forms.Panel
            Me.BsOuter1 = New System.Windows.Forms.Panel
            Me.SuspendLayout()
            '
            'BsEnd2
            '
            Me.BsEnd2.BackColor = System.Drawing.Color.DimGray
            Me.BsEnd2.Location = New System.Drawing.Point(99, 0)
            Me.BsEnd2.Name = "BsEnd2"
            Me.BsEnd2.Size = New System.Drawing.Size(1, 5)
            Me.BsEnd2.TabIndex = 3
            Me.BsEnd2.Visible = False
            '
            'BsEnd1
            '
            Me.BsEnd1.BackColor = System.Drawing.Color.DimGray
            Me.BsEnd1.Location = New System.Drawing.Point(0, 0)
            Me.BsEnd1.Name = "BsEnd1"
            Me.BsEnd1.Size = New System.Drawing.Size(1, 5)
            Me.BsEnd1.TabIndex = 2
            Me.BsEnd1.Visible = False
            '
            'BsOuter2
            '
            Me.BsOuter2.BackColor = System.Drawing.Color.DimGray
            Me.BsOuter2.Location = New System.Drawing.Point(0, 4)
            Me.BsOuter2.Name = "BsOuter2"
            Me.BsOuter2.Size = New System.Drawing.Size(100, 1)
            Me.BsOuter2.TabIndex = 1
            '
            'BsOuter1
            '
            Me.BsOuter1.BackColor = System.Drawing.Color.DimGray
            Me.BsOuter1.Location = New System.Drawing.Point(0, 0)
            Me.BsOuter1.Name = "BsOuter1"
            Me.BsOuter1.Size = New System.Drawing.Size(100, 1)
            Me.BsOuter1.TabIndex = 0
            '
            'BsPipeControl
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.WhiteSmoke
            Me.Controls.Add(Me.BsEnd2)
            Me.Controls.Add(Me.BsEnd1)
            Me.Controls.Add(Me.BsOuter2)
            Me.Controls.Add(Me.BsOuter1)
            Me.Margin = New System.Windows.Forms.Padding(0)
            Me.Name = "BsPipeControl"
            Me.Size = New System.Drawing.Size(100, 5)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents BsOuter1 As System.Windows.Forms.Panel
        Friend WithEvents BsOuter2 As System.Windows.Forms.Panel
        Friend WithEvents BsEnd1 As System.Windows.Forms.Panel
        Friend WithEvents BsEnd2 As System.Windows.Forms.Panel

    End Class
End Namespace