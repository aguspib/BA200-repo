<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BsGIFViewer
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
        Me.BackPanel = New System.Windows.Forms.Panel
        Me.myPicBox = New System.Windows.Forms.PictureBox
        Me.BackPanel.SuspendLayout()
        CType(Me.myPicBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BackPanel
        '
        Me.BackPanel.AutoScroll = True
        Me.BackPanel.Controls.Add(Me.myPicBox)
        Me.BackPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BackPanel.Location = New System.Drawing.Point(0, 0)
        Me.BackPanel.Name = "BackPanel"
        Me.BackPanel.Size = New System.Drawing.Size(150, 150)
        Me.BackPanel.TabIndex = 0
        '
        'myPicBox
        '
        Me.myPicBox.Location = New System.Drawing.Point(0, 0)
        Me.myPicBox.Name = "myPicBox"
        Me.myPicBox.Size = New System.Drawing.Size(82, 79)
        Me.myPicBox.TabIndex = 0
        Me.myPicBox.TabStop = False
        '
        'BsGIFViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.BackPanel)
        Me.Name = "BsGIFViewer"
        Me.BackPanel.ResumeLayout(False)
        CType(Me.myPicBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BackPanel As System.Windows.Forms.Panel
    Friend WithEvents myPicBox As System.Windows.Forms.PictureBox

End Class
