<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ISECodeGenerator
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
        Me.BsGenerateButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsCheckButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsCodeTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsPrepareButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.BsCodesTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsProgressBar = New Biosystems.Ax00.Controls.UserControls.BSProgressBar()
        Me.BsAllCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsValidateAllCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox()
        Me.BsCodesOKTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsCodesErrorTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox()
        Me.BsLabel1 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.BsLabel2 = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.SuspendLayout()
        '
        'BsGenerateButton
        '
        Me.BsGenerateButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsGenerateButton.Enabled = False
        Me.BsGenerateButton.Location = New System.Drawing.Point(146, 23)
        Me.BsGenerateButton.Name = "BsGenerateButton"
        Me.BsGenerateButton.Size = New System.Drawing.Size(98, 46)
        Me.BsGenerateButton.TabIndex = 0
        Me.BsGenerateButton.Text = "Generate"
        Me.BsGenerateButton.UseVisualStyleBackColor = True
        '
        'BsCheckButton
        '
        Me.BsCheckButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCheckButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsCheckButton.Location = New System.Drawing.Point(566, 23)
        Me.BsCheckButton.Name = "BsCheckButton"
        Me.BsCheckButton.Size = New System.Drawing.Size(98, 46)
        Me.BsCheckButton.TabIndex = 2
        Me.BsCheckButton.Text = "Check"
        Me.BsCheckButton.UseVisualStyleBackColor = True
        '
        'BsCodeTextBox
        '
        Me.BsCodeTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCodeTextBox.BackColor = System.Drawing.Color.White
        Me.BsCodeTextBox.DecimalsValues = False
        Me.BsCodeTextBox.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsCodeTextBox.IsNumeric = False
        Me.BsCodeTextBox.Location = New System.Drawing.Point(12, 86)
        Me.BsCodeTextBox.Mandatory = False
        Me.BsCodeTextBox.Name = "BsCodeTextBox"
        Me.BsCodeTextBox.Size = New System.Drawing.Size(750, 23)
        Me.BsCodeTextBox.TabIndex = 4
        '
        'BsPrepareButton
        '
        Me.BsPrepareButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsPrepareButton.Location = New System.Drawing.Point(12, 23)
        Me.BsPrepareButton.Name = "BsPrepareButton"
        Me.BsPrepareButton.Size = New System.Drawing.Size(98, 46)
        Me.BsPrepareButton.TabIndex = 5
        Me.BsPrepareButton.Text = "Prepare"
        Me.BsPrepareButton.UseVisualStyleBackColor = True
        '
        'BsCodesTextBox
        '
        Me.BsCodesTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCodesTextBox.BackColor = System.Drawing.Color.White
        Me.BsCodesTextBox.DecimalsValues = False
        Me.BsCodesTextBox.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsCodesTextBox.IsNumeric = False
        Me.BsCodesTextBox.Location = New System.Drawing.Point(10, 124)
        Me.BsCodesTextBox.Mandatory = False
        Me.BsCodesTextBox.Multiline = True
        Me.BsCodesTextBox.Name = "BsCodesTextBox"
        Me.BsCodesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.BsCodesTextBox.Size = New System.Drawing.Size(750, 136)
        Me.BsCodesTextBox.TabIndex = 6
        '
        'BsProgressBar
        '
        Me.BsProgressBar.Location = New System.Drawing.Point(10, 266)
        Me.BsProgressBar.Name = "BsProgressBar"
        Me.BsProgressBar.Size = New System.Drawing.Size(750, 17)
        Me.BsProgressBar.TabIndex = 7
        Me.BsProgressBar.Visible = False
        '
        'BsAllCheckbox
        '
        Me.BsAllCheckbox.AutoSize = True
        Me.BsAllCheckbox.Location = New System.Drawing.Point(268, 48)
        Me.BsAllCheckbox.Name = "BsAllCheckbox"
        Me.BsAllCheckbox.Size = New System.Drawing.Size(67, 17)
        Me.BsAllCheckbox.TabIndex = 8
        Me.BsAllCheckbox.Text = "Find All"
        Me.BsAllCheckbox.UseVisualStyleBackColor = True
        '
        'BsValidateAllCheckbox
        '
        Me.BsValidateAllCheckbox.AutoSize = True
        Me.BsValidateAllCheckbox.Location = New System.Drawing.Point(682, 52)
        Me.BsValidateAllCheckbox.Name = "BsValidateAllCheckbox"
        Me.BsValidateAllCheckbox.Size = New System.Drawing.Size(80, 17)
        Me.BsValidateAllCheckbox.TabIndex = 9
        Me.BsValidateAllCheckbox.Text = "Check All"
        Me.BsValidateAllCheckbox.UseVisualStyleBackColor = True
        '
        'BsCodesOKTextBox
        '
        Me.BsCodesOKTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCodesOKTextBox.BackColor = System.Drawing.Color.White
        Me.BsCodesOKTextBox.DecimalsValues = False
        Me.BsCodesOKTextBox.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsCodesOKTextBox.IsNumeric = False
        Me.BsCodesOKTextBox.Location = New System.Drawing.Point(10, 310)
        Me.BsCodesOKTextBox.Mandatory = False
        Me.BsCodesOKTextBox.Multiline = True
        Me.BsCodesOKTextBox.Name = "BsCodesOKTextBox"
        Me.BsCodesOKTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.BsCodesOKTextBox.Size = New System.Drawing.Size(750, 90)
        Me.BsCodesOKTextBox.TabIndex = 10
        '
        'BsCodesErrorTextBox
        '
        Me.BsCodesErrorTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsCodesErrorTextBox.BackColor = System.Drawing.Color.White
        Me.BsCodesErrorTextBox.DecimalsValues = False
        Me.BsCodesErrorTextBox.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BsCodesErrorTextBox.IsNumeric = False
        Me.BsCodesErrorTextBox.Location = New System.Drawing.Point(9, 441)
        Me.BsCodesErrorTextBox.Mandatory = False
        Me.BsCodesErrorTextBox.Multiline = True
        Me.BsCodesErrorTextBox.Name = "BsCodesErrorTextBox"
        Me.BsCodesErrorTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.BsCodesErrorTextBox.Size = New System.Drawing.Size(750, 90)
        Me.BsCodesErrorTextBox.TabIndex = 11
        '
        'BsLabel1
        '
        Me.BsLabel1.AutoSize = True
        Me.BsLabel1.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel1.Location = New System.Drawing.Point(9, 294)
        Me.BsLabel1.Name = "BsLabel1"
        Me.BsLabel1.Size = New System.Drawing.Size(89, 13)
        Me.BsLabel1.TabIndex = 12
        Me.BsLabel1.Text = "Validation OK:"
        Me.BsLabel1.Title = False
        '
        'BsLabel2
        '
        Me.BsLabel2.AutoSize = True
        Me.BsLabel2.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel2.Location = New System.Drawing.Point(9, 425)
        Me.BsLabel2.Name = "BsLabel2"
        Me.BsLabel2.Size = New System.Drawing.Size(112, 13)
        Me.BsLabel2.TabIndex = 13
        Me.BsLabel2.Text = "Validation ERROR:"
        Me.BsLabel2.Title = False
        '
        'ISECodeGenerator
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(771, 543)
        Me.Controls.Add(Me.BsLabel2)
        Me.Controls.Add(Me.BsLabel1)
        Me.Controls.Add(Me.BsCodesErrorTextBox)
        Me.Controls.Add(Me.BsCodesOKTextBox)
        Me.Controls.Add(Me.BsValidateAllCheckbox)
        Me.Controls.Add(Me.BsAllCheckbox)
        Me.Controls.Add(Me.BsProgressBar)
        Me.Controls.Add(Me.BsCodesTextBox)
        Me.Controls.Add(Me.BsPrepareButton)
        Me.Controls.Add(Me.BsCodeTextBox)
        Me.Controls.Add(Me.BsCheckButton)
        Me.Controls.Add(Me.BsGenerateButton)
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "ISECodeGenerator"
        Me.Text = "ISE Code Generator"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BsGenerateButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsCheckButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsCodeTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsPrepareButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsCodesTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsProgressBar As Biosystems.Ax00.Controls.UserControls.BSProgressBar
    Friend WithEvents BsAllCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsValidateAllCheckbox As Biosystems.Ax00.Controls.UserControls.BSCheckbox
    Friend WithEvents BsCodesOKTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsCodesErrorTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsLabel1 As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsLabel2 As Biosystems.Ax00.Controls.UserControls.BSLabel
End Class
