Imports System.ComponentModel
Imports Biosystems.Ax00.Controls.UserControls
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class TestSimulatorAX00
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
        Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(TestSimulatorAX00))
        Me.CloseButton = New Button
        Me.AnswerButton = New Button
        Me.BsTextWrite_Endw = New BSTextBox
        Me.BsReceivedTextBox = New BSTextBox
        Me.Button1 = New Button
        Me.SuspendLayout()
        '
        'CloseButton
        '
        Me.CloseButton.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
        Me.CloseButton.Font = New Font("Microsoft Sans Serif", 12.0!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
        Me.CloseButton.Location = New Point(566, 0)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New Size(26, 32)
        Me.CloseButton.TabIndex = 0
        Me.CloseButton.Text = "x"
        Me.CloseButton.TextAlign = ContentAlignment.TopCenter
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'AnswerButton
        '
        Me.AnswerButton.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
                    Or AnchorStyles.Left) _
                    Or AnchorStyles.Right), AnchorStyles)
        Me.AnswerButton.Location = New Point(114, 132)
        Me.AnswerButton.Name = "AnswerButton"
        Me.AnswerButton.Size = New Size(108, 32)
        Me.AnswerButton.TabIndex = 1
        Me.AnswerButton.Text = "ANSWER"
        Me.AnswerButton.UseVisualStyleBackColor = True
        '
        'BsTextWrite_Endw
        '
        Me.BsTextWrite_Endw.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
                    Or AnchorStyles.Left) _
                    Or AnchorStyles.Right), AnchorStyles)
        Me.BsTextWrite_Endw.BackColor = Color.White
        Me.BsTextWrite_Endw.DecimalsValues = False
        Me.BsTextWrite_Endw.Font = New Font("Verdana", 8.25!)
        Me.BsTextWrite_Endw.IsNumeric = False
        Me.BsTextWrite_Endw.Location = New Point(100, 180)
        Me.BsTextWrite_Endw.Mandatory = False
        Me.BsTextWrite_Endw.Multiline = True
        Me.BsTextWrite_Endw.Name = "BsTextWrite_Endw"
        Me.BsTextWrite_Endw.Size = New Size(377, 24)
        Me.BsTextWrite_Endw.TabIndex = 48
        Me.BsTextWrite_Endw.Text = "A400;FWSCRIPT_ANSWER;S:2;AC:6;T:0;C:6;W:0;R:0;E:0;"
        '
        'BsReceivedTextBox
        '
        Me.BsReceivedTextBox.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
                    Or AnchorStyles.Left) _
                    Or AnchorStyles.Right), AnchorStyles)
        Me.BsReceivedTextBox.BackColor = Color.White
        Me.BsReceivedTextBox.DecimalsValues = False
        Me.BsReceivedTextBox.Font = New Font("Verdana", 8.25!)
        Me.BsReceivedTextBox.IsNumeric = False
        Me.BsReceivedTextBox.Location = New Point(100, 239)
        Me.BsReceivedTextBox.Mandatory = False
        Me.BsReceivedTextBox.Multiline = True
        Me.BsReceivedTextBox.Name = "BsReceivedTextBox"
        Me.BsReceivedTextBox.ScrollBars = ScrollBars.Vertical
        Me.BsReceivedTextBox.Size = New Size(377, 151)
        Me.BsReceivedTextBox.TabIndex = 13
        '
        'Button1
        '
        Me.Button1.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
                    Or AnchorStyles.Left) _
                    Or AnchorStyles.Right), AnchorStyles)
        Me.Button1.Location = New Point(504, 358)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New Size(76, 32)
        Me.Button1.TabIndex = 49
        Me.Button1.Text = "TEST"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TestSimulatorAX00
        '
        Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = Color.Gainsboro
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), Image)
        Me.BackgroundImageLayout = ImageLayout.Stretch
        Me.ClientSize = New Size(592, 416)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.BsTextWrite_Endw)
        Me.Controls.Add(Me.BsReceivedTextBox)
        Me.Controls.Add(Me.AnswerButton)
        Me.Controls.Add(Me.CloseButton)
        Me.FormBorderStyle = FormBorderStyle.None
        Me.Name = "TestSimulatorAX00"
        Me.Text = "TestSimulatorAX00"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CloseButton As Button
    Friend WithEvents AnswerButton As Button
    Friend WithEvents BsReceivedTextBox As BSTextBox
    Friend WithEvents BsTextWrite_Endw As BSTextBox
    Friend WithEvents Button1 As Button
End Class
