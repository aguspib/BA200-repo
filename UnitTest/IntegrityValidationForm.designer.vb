<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IntegrityValidationForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IntegrityValidationForm))
        Me.ExecuteTestButton = New System.Windows.Forms.Button()
        Me.ResultTextBox = New System.Windows.Forms.RichTextBox()
        Me.TitleLabel = New System.Windows.Forms.Label()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ShowErrorChkBox = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'ExecuteTestButton
        '
        Me.ExecuteTestButton.Image = CType(resources.GetObject("ExecuteTestButton.Image"), System.Drawing.Image)
        Me.ExecuteTestButton.Location = New System.Drawing.Point(490, 401)
        Me.ExecuteTestButton.Name = "ExecuteTestButton"
        Me.ExecuteTestButton.Size = New System.Drawing.Size(32, 32)
        Me.ExecuteTestButton.TabIndex = 0
        Me.ExecuteTestButton.UseVisualStyleBackColor = True
        '
        'ResultTextBox
        '
        Me.ResultTextBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ResultTextBox.Location = New System.Drawing.Point(12, 35)
        Me.ResultTextBox.Name = "ResultTextBox"
        Me.ResultTextBox.ReadOnly = True
        Me.ResultTextBox.Size = New System.Drawing.Size(548, 334)
        Me.ResultTextBox.TabIndex = 1
        Me.ResultTextBox.Text = ""
        '
        'TitleLabel
        '
        Me.TitleLabel.AutoSize = True
        Me.TitleLabel.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TitleLabel.Location = New System.Drawing.Point(13, 14)
        Me.TitleLabel.Name = "TitleLabel"
        Me.TitleLabel.Size = New System.Drawing.Size(228, 18)
        Me.TitleLabel.TabIndex = 2
        Me.TitleLabel.Text = "Integrity validation results"
        '
        'CancelButton
        '
        Me.ButtonCancel.Image = CType(resources.GetObject("CancelButton.Image"), System.Drawing.Image)
        Me.ButtonCancel.Location = New System.Drawing.Point(528, 401)
        Me.ButtonCancel.Name = "CancelButton"
        Me.ButtonCancel.Size = New System.Drawing.Size(32, 32)
        Me.ButtonCancel.TabIndex = 3
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(12, 372)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(548, 23)
        Me.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ProgressBar1.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Arial Narrow", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(13, 435)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(547, 43)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "ATTENTION: Before starting the validation process, remember to restore the Ax00_T" & _
    "EM DataBase on your local Server and set the connection  to Ax00 Database."
        '
        'ShowErrorChkBox
        '
        Me.ShowErrorChkBox.AutoSize = True
        Me.ShowErrorChkBox.Location = New System.Drawing.Point(12, 402)
        Me.ShowErrorChkBox.Name = "ShowErrorChkBox"
        Me.ShowErrorChkBox.Size = New System.Drawing.Size(107, 17)
        Me.ShowErrorChkBox.TabIndex = 6
        Me.ShowErrorChkBox.Text = "Show Only Errors"
        Me.ShowErrorChkBox.UseVisualStyleBackColor = True
        '
        'IntegrityValidationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(572, 488)
        Me.Controls.Add(Me.ShowErrorChkBox)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.ButtonCancel)
        Me.Controls.Add(Me.TitleLabel)
        Me.Controls.Add(Me.ResultTextBox)
        Me.Controls.Add(Me.ExecuteTestButton)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "IntegrityValidationForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Integrity Validation "
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ExecuteTestButton As System.Windows.Forms.Button
    Friend WithEvents ResultTextBox As System.Windows.Forms.RichTextBox
    Friend WithEvents TitleLabel As System.Windows.Forms.Label
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ShowErrorChkBox As System.Windows.Forms.CheckBox
End Class
