Imports System.ComponentModel
Imports Biosystems.Ax00.Controls.UserControls
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class TestForm
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
        Me.BsButton7 = New BSButton()
        Me.BsButton5 = New BSButton()
        Me.BsButton4 = New BSButton()
        Me.BsButton3 = New BSButton()
        Me.BsButton2 = New BSButton()
        Me.BsButton8 = New BSButton()
        Me.SuspendLayout()
        '
        'BsButton7
        '
        Me.BsButton7.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton7.Location = New Point(12, 12)
        Me.BsButton7.Name = "BsButton7"
        Me.BsButton7.Size = New Size(73, 39)
        Me.BsButton7.TabIndex = 6
        Me.BsButton7.Text = "PDFTest"
        Me.BsButton7.UseVisualStyleBackColor = True
        '
        'BsButton5
        '
        Me.BsButton5.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton5.Location = New Point(255, 95)
        Me.BsButton5.Name = "BsButton5"
        Me.BsButton5.Size = New Size(74, 32)
        Me.BsButton5.TabIndex = 4
        Me.BsButton5.Text = "XaviTest"
        Me.BsButton5.UseVisualStyleBackColor = True
        '
        'BsButton4
        '
        Me.BsButton4.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton4.Location = New Point(348, 188)
        Me.BsButton4.Name = "BsButton4"
        Me.BsButton4.Size = New Size(73, 42)
        Me.BsButton4.TabIndex = 3
        Me.BsButton4.Text = "Edit XML Scripts"
        Me.BsButton4.UseVisualStyleBackColor = True
        '
        'BsButton3
        '
        Me.BsButton3.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton3.Location = New Point(348, 143)
        Me.BsButton3.Name = "BsButton3"
        Me.BsButton3.Size = New Size(73, 39)
        Me.BsButton3.TabIndex = 2
        Me.BsButton3.Text = "SergioTest"
        Me.BsButton3.UseVisualStyleBackColor = True
        '
        'BsButton2
        '
        Me.BsButton2.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton2.Location = New Point(335, 95)
        Me.BsButton2.Name = "BsButton2"
        Me.BsButton2.Size = New Size(87, 32)
        Me.BsButton2.TabIndex = 1
        Me.BsButton2.Text = "Test Comm's"
        Me.BsButton2.UseVisualStyleBackColor = True
        '
        'BsButton8
        '
        Me.BsButton8.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton8.Location = New Point(33, 215)
        Me.BsButton8.Name = "BsButton8"
        Me.BsButton8.Size = New Size(73, 39)
        Me.BsButton8.TabIndex = 7
        Me.BsButton8.Text = "FWUTIL test"
        Me.BsButton8.UseVisualStyleBackColor = True
        '
        'TestForm
        '
        Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(489, 266)
        Me.Controls.Add(Me.BsButton8)
        Me.Controls.Add(Me.BsButton7)
        Me.Controls.Add(Me.BsButton5)
        Me.Controls.Add(Me.BsButton4)
        Me.Controls.Add(Me.BsButton3)
        Me.Controls.Add(Me.BsButton2)
        Me.Name = "TestForm"
        Me.Text = "TestForm"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsButton2 As BSButton
    Friend WithEvents BsButton3 As BSButton
    Friend WithEvents BsButton4 As BSButton
    Friend WithEvents BsButton5 As BSButton
    Friend WithEvents BsButton7 As BSButton
    Friend WithEvents BsButton8 As BSButton
End Class
