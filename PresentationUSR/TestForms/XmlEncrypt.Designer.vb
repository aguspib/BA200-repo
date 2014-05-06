<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class XmlEncrypt
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
        Me.components = New System.ComponentModel.Container()
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox()
        Me.EncriptButton = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.RichTextBox2 = New System.Windows.Forms.RichTextBox()
        Me.EncrypFileButton = New System.Windows.Forms.Button()
        Me.EncriptedFileName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ErrorProvider1 = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.Button1 = New System.Windows.Forms.Button()
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RichTextBox1
        '
        Me.RichTextBox1.Location = New System.Drawing.Point(12, 84)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.ReadOnly = True
        Me.RichTextBox1.Size = New System.Drawing.Size(532, 559)
        Me.RichTextBox1.TabIndex = 0
        Me.RichTextBox1.Text = ""
        '
        'EncriptButton
        '
        Me.EncriptButton.Location = New System.Drawing.Point(12, 26)
        Me.EncriptButton.Name = "EncriptButton"
        Me.EncriptButton.Size = New System.Drawing.Size(111, 52)
        Me.EncriptButton.TabIndex = 1
        Me.EncriptButton.Text = "Open file"
        Me.EncriptButton.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'RichTextBox2
        '
        Me.RichTextBox2.Location = New System.Drawing.Point(564, 84)
        Me.RichTextBox2.Name = "RichTextBox2"
        Me.RichTextBox2.ReadOnly = True
        Me.RichTextBox2.Size = New System.Drawing.Size(532, 559)
        Me.RichTextBox2.TabIndex = 2
        Me.RichTextBox2.Text = ""
        '
        'EncrypFileButton
        '
        Me.EncrypFileButton.Location = New System.Drawing.Point(985, 27)
        Me.EncrypFileButton.Name = "EncrypFileButton"
        Me.EncrypFileButton.Size = New System.Drawing.Size(111, 51)
        Me.EncrypFileButton.TabIndex = 3
        Me.EncrypFileButton.Text = "Encrypt File"
        Me.EncrypFileButton.UseVisualStyleBackColor = True
        '
        'EncriptedFileName
        '
        Me.EncriptedFileName.Location = New System.Drawing.Point(564, 57)
        Me.EncriptedFileName.Name = "EncriptedFileName"
        Me.EncriptedFileName.Size = New System.Drawing.Size(199, 21)
        Me.EncriptedFileName.TabIndex = 4
        Me.EncriptedFileName.Text = "TaskList.xml"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(564, 38)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(109, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "OutPut  File Name"
        '
        'ErrorProvider1
        '
        Me.ErrorProvider1.ContainerControl = Me
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(985, 650)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(111, 39)
        Me.Button1.TabIndex = 6
        Me.Button1.Text = "Clear"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'XmlEncrypt
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(1118, 722)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.EncriptedFileName)
        Me.Controls.Add(Me.EncrypFileButton)
        Me.Controls.Add(Me.RichTextBox2)
        Me.Controls.Add(Me.EncriptButton)
        Me.Controls.Add(Me.RichTextBox1)
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "XmlEncrypt"
        Me.Text = "Encrypt File "
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
    Friend WithEvents EncriptButton As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents RichTextBox2 As System.Windows.Forms.RichTextBox
    Friend WithEvents EncrypFileButton As System.Windows.Forms.Button
    Friend WithEvents EncriptedFileName As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ErrorProvider1 As System.Windows.Forms.ErrorProvider
    Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
