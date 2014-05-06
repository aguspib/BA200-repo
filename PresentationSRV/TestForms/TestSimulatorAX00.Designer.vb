<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TestSimulatorAX00
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TestSimulatorAX00))
        Me.CloseButton = New System.Windows.Forms.Button
        Me.AnswerButton = New System.Windows.Forms.Button
        Me.BsTextWrite_Endw = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.BsReceivedTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.Button1 = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'CloseButton
        '
        Me.CloseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CloseButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CloseButton.Location = New System.Drawing.Point(566, 0)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(26, 32)
        Me.CloseButton.TabIndex = 0
        Me.CloseButton.Text = "x"
        Me.CloseButton.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'AnswerButton
        '
        Me.AnswerButton.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AnswerButton.Location = New System.Drawing.Point(114, 132)
        Me.AnswerButton.Name = "AnswerButton"
        Me.AnswerButton.Size = New System.Drawing.Size(108, 32)
        Me.AnswerButton.TabIndex = 1
        Me.AnswerButton.Text = "ANSWER"
        Me.AnswerButton.UseVisualStyleBackColor = True
        '
        'BsTextWrite_Endw
        '
        Me.BsTextWrite_Endw.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsTextWrite_Endw.BackColor = System.Drawing.Color.White
        Me.BsTextWrite_Endw.DecimalsValues = False
        Me.BsTextWrite_Endw.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsTextWrite_Endw.IsNumeric = False
        Me.BsTextWrite_Endw.Location = New System.Drawing.Point(100, 180)
        Me.BsTextWrite_Endw.Mandatory = False
        Me.BsTextWrite_Endw.Multiline = True
        Me.BsTextWrite_Endw.Name = "BsTextWrite_Endw"
        Me.BsTextWrite_Endw.Size = New System.Drawing.Size(377, 24)
        Me.BsTextWrite_Endw.TabIndex = 48
        Me.BsTextWrite_Endw.Text = "A400;FWSCRIPT_ANSWER;S:2;AC:6;T:0;C:6;W:0;R:0;E:0;"
        '
        'BsReceivedTextBox
        '
        Me.BsReceivedTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsReceivedTextBox.BackColor = System.Drawing.Color.White
        Me.BsReceivedTextBox.DecimalsValues = False
        Me.BsReceivedTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsReceivedTextBox.IsNumeric = False
        Me.BsReceivedTextBox.Location = New System.Drawing.Point(100, 239)
        Me.BsReceivedTextBox.Mandatory = False
        Me.BsReceivedTextBox.Multiline = True
        Me.BsReceivedTextBox.Name = "BsReceivedTextBox"
        Me.BsReceivedTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.BsReceivedTextBox.Size = New System.Drawing.Size(377, 151)
        Me.BsReceivedTextBox.TabIndex = 13
        '
        'Button1
        '
        Me.Button1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.Location = New System.Drawing.Point(504, 358)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(76, 32)
        Me.Button1.TabIndex = 49
        Me.Button1.Text = "TEST"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TestSimulatorAX00
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Gainsboro
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(592, 416)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.BsTextWrite_Endw)
        Me.Controls.Add(Me.BsReceivedTextBox)
        Me.Controls.Add(Me.AnswerButton)
        Me.Controls.Add(Me.CloseButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "TestSimulatorAX00"
        Me.Text = "TestSimulatorAX00"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CloseButton As System.Windows.Forms.Button
    Friend WithEvents AnswerButton As System.Windows.Forms.Button
    Friend WithEvents BsReceivedTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsTextWrite_Endw As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
