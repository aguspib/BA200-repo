Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports Biosystems.Ax00.Controls.UserControls
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class FormSergio
    Inherits BSAdjustmentBaseForm

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
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Me.ISEResultsTextBox = New TextBox()
        Me.Label1 = New Label()
        Me.BsButton10 = New BSButton()
        Me.BsButton9 = New BSButton()
        Me.BsButton8 = New BSButton()
        Me.BsButton5 = New BSButton()
        Me.BsButton4 = New BSButton()
        Me.BsButton3 = New BSButton()
        Me.BsButton2 = New BSButton()
        Me.BsButton1 = New BSButton()
        Me.BsExceptionsButton = New BSButton()
        Me.BsCloseButton = New BSButton()
        Me.BsButton6 = New BSButton()
        Me.BsButton7 = New BSButton()
        Me.BsButton11 = New BSButton()
        Me.BsButton12 = New BSButton()
        Me.BsButton13 = New BSButton()
        Me.BsButton14 = New BSButton()
        Me.BsDataGridView1 = New BSDataGridView()
        Me.Column1 = New DataGridViewButtonColumn()
        Me.Column2 = New DataGridViewTextBoxColumn()
        Me.Column3 = New DataGridViewTextBoxColumn()
        Me.Column4 = New DataGridViewButtonColumn()
        Me.BsBindingSource1 = New BSBindingSource()
        Me.BsButton15 = New BSButton()
        Me.BsButton16 = New BSButton()
        Me.BsButton17 = New BSButton()
        Me.BsButton18 = New BSButton()
        Me.BsButton19 = New BSButton()
        Me.BsToUpper = New BSButton()
        Me.BsToUpperTextBox = New BSTextBox()
        Me.BsCompareTextBox = New BSTextBox()
        Me.BsCompareButton = New BSButton()
        CType(Me.BsDataGridView1, ISupportInitialize).BeginInit()
        CType(Me.BsBindingSource1, ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ISEResultsTextBox
        '
        Me.ISEResultsTextBox.Font = New Font("Courier New", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.ISEResultsTextBox.Location = New Point(218, 360)
        Me.ISEResultsTextBox.Multiline = True
        Me.ISEResultsTextBox.Name = "ISEResultsTextBox"
        Me.ISEResultsTextBox.ScrollBars = ScrollBars.Both
        Me.ISEResultsTextBox.Size = New Size(490, 218)
        Me.ISEResultsTextBox.TabIndex = 46
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New Font("Verdana", 8.25!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New Point(888, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New Size(69, 13)
        Me.Label1.TabIndex = 38
        Me.Label1.Text = "TASK BAR"
        '
        'BsButton10
        '
        Me.BsButton10.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton10.Location = New Point(51, 411)
        Me.BsButton10.Name = "BsButton10"
        Me.BsButton10.Size = New Size(120, 45)
        Me.BsButton10.TabIndex = 45
        Me.BsButton10.Text = "ISECMD response"
        Me.BsButton10.UseVisualStyleBackColor = True
        '
        'BsButton9
        '
        Me.BsButton9.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton9.Location = New Point(51, 360)
        Me.BsButton9.Name = "BsButton9"
        Me.BsButton9.Size = New Size(120, 45)
        Me.BsButton9.TabIndex = 44
        Me.BsButton9.Text = "ISETEST response"
        Me.BsButton9.UseVisualStyleBackColor = True
        '
        'BsButton8
        '
        Me.BsButton8.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton8.Location = New Point(51, 264)
        Me.BsButton8.Name = "BsButton8"
        Me.BsButton8.Size = New Size(120, 45)
        Me.BsButton8.TabIndex = 43
        Me.BsButton8.Text = "BsButton8"
        Me.BsButton8.UseVisualStyleBackColor = True
        '
        'BsButton5
        '
        Me.BsButton5.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton5.Location = New Point(891, 109)
        Me.BsButton5.Name = "BsButton5"
        Me.BsButton5.Size = New Size(92, 45)
        Me.BsButton5.TabIndex = 36
        Me.BsButton5.Text = "Always Visible"
        Me.BsButton5.TextAlign = ContentAlignment.MiddleLeft
        Me.BsButton5.UseVisualStyleBackColor = True
        '
        'BsButton4
        '
        Me.BsButton4.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton4.Location = New Point(891, 58)
        Me.BsButton4.Name = "BsButton4"
        Me.BsButton4.Size = New Size(92, 45)
        Me.BsButton4.TabIndex = 35
        Me.BsButton4.Text = "Auto Hide"
        Me.BsButton4.TextAlign = ContentAlignment.MiddleLeft
        Me.BsButton4.UseVisualStyleBackColor = True
        '
        'BsButton3
        '
        Me.BsButton3.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton3.Location = New Point(429, 32)
        Me.BsButton3.Name = "BsButton3"
        Me.BsButton3.Size = New Size(120, 45)
        Me.BsButton3.TabIndex = 34
        Me.BsButton3.Text = "BsButton3"
        Me.BsButton3.UseVisualStyleBackColor = True
        '
        'BsButton2
        '
        Me.BsButton2.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton2.Location = New Point(51, 194)
        Me.BsButton2.Name = "BsButton2"
        Me.BsButton2.Size = New Size(120, 45)
        Me.BsButton2.TabIndex = 33
        Me.BsButton2.Text = "BsButton2"
        Me.BsButton2.UseVisualStyleBackColor = True
        '
        'BsButton1
        '
        Me.BsButton1.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton1.Location = New Point(51, 122)
        Me.BsButton1.Name = "BsButton1"
        Me.BsButton1.Size = New Size(120, 45)
        Me.BsButton1.TabIndex = 32
        Me.BsButton1.Text = "BsButton1"
        Me.BsButton1.UseVisualStyleBackColor = True
        '
        'BsExceptionsButton
        '
        Me.BsExceptionsButton.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsExceptionsButton.Location = New Point(36, 32)
        Me.BsExceptionsButton.Name = "BsExceptionsButton"
        Me.BsExceptionsButton.Size = New Size(236, 46)
        Me.BsExceptionsButton.TabIndex = 31
        Me.BsExceptionsButton.Text = "Manage Exceptions"
        Me.BsExceptionsButton.UseVisualStyleBackColor = True
        '
        'BsCloseButton
        '
        Me.BsCloseButton.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsCloseButton.Location = New Point(990, 0)
        Me.BsCloseButton.Name = "BsCloseButton"
        Me.BsCloseButton.Size = New Size(19, 22)
        Me.BsCloseButton.TabIndex = 30
        Me.BsCloseButton.Text = "X"
        Me.BsCloseButton.UseVisualStyleBackColor = True
        '
        'BsButton6
        '
        Me.BsButton6.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton6.Location = New Point(269, 122)
        Me.BsButton6.Name = "BsButton6"
        Me.BsButton6.Size = New Size(120, 45)
        Me.BsButton6.TabIndex = 47
        Me.BsButton6.Text = "Read ISE Settings"
        Me.BsButton6.UseVisualStyleBackColor = True
        '
        'BsButton7
        '
        Me.BsButton7.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton7.Location = New Point(269, 194)
        Me.BsButton7.Name = "BsButton7"
        Me.BsButton7.Size = New Size(120, 45)
        Me.BsButton7.TabIndex = 48
        Me.BsButton7.Text = "Save ISE Settings"
        Me.BsButton7.UseVisualStyleBackColor = True
        '
        'BsButton11
        '
        Me.BsButton11.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton11.Location = New Point(417, 122)
        Me.BsButton11.Name = "BsButton11"
        Me.BsButton11.Size = New Size(120, 45)
        Me.BsButton11.TabIndex = 49
        Me.BsButton11.Text = "View ISE Calib History"
        Me.BsButton11.UseVisualStyleBackColor = True
        '
        'BsButton12
        '
        Me.BsButton12.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton12.Location = New Point(417, 194)
        Me.BsButton12.Name = "BsButton12"
        Me.BsButton12.Size = New Size(120, 45)
        Me.BsButton12.TabIndex = 50
        Me.BsButton12.Text = "Add to ISE Calib History"
        Me.BsButton12.UseVisualStyleBackColor = True
        '
        'BsButton13
        '
        Me.BsButton13.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton13.Location = New Point(51, 476)
        Me.BsButton13.Name = "BsButton13"
        Me.BsButton13.Size = New Size(120, 45)
        Me.BsButton13.TabIndex = 51
        Me.BsButton13.Text = "ISE Monitor 1"
        Me.BsButton13.UseVisualStyleBackColor = True
        '
        'BsButton14
        '
        Me.BsButton14.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton14.ImageAlign = ContentAlignment.MiddleLeft
        Me.BsButton14.Location = New Point(635, 32)
        Me.BsButton14.Name = "BsButton14"
        Me.BsButton14.Size = New Size(120, 25)
        Me.BsButton14.TabIndex = 174
        Me.BsButton14.Text = "Imagen"
        Me.BsButton14.TextAlign = ContentAlignment.MiddleRight
        Me.BsButton14.UseVisualStyleBackColor = True
        '
        'BsDataGridView1
        '
        DataGridViewCellStyle1.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = Color.Black
        DataGridViewCellStyle1.SelectionBackColor = Color.LightSlateGray
        Me.BsDataGridView1.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.BsDataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = Color.DarkGray
        DataGridViewCellStyle2.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = Color.Black
        DataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = DataGridViewTriState.[False]
        Me.BsDataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.BsDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.BsDataGridView1.Columns.AddRange(New DataGridViewColumn() {Me.Column1, Me.Column2, Me.Column3, Me.Column4})
        DataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = SystemColors.Window
        DataGridViewCellStyle3.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = Color.Black
        DataGridViewCellStyle3.SelectionBackColor = Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = Color.White
        DataGridViewCellStyle3.WrapMode = DataGridViewTriState.[False]
        Me.BsDataGridView1.DefaultCellStyle = DataGridViewCellStyle3
        Me.BsDataGridView1.EnterToTab = False
        Me.BsDataGridView1.GridColor = Color.Silver
        Me.BsDataGridView1.Location = New Point(690, 186)
        Me.BsDataGridView1.Name = "BsDataGridView1"
        DataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = Color.DarkGray
        DataGridViewCellStyle4.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = DataGridViewTriState.[False]
        Me.BsDataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.BsDataGridView1.Size = New Size(266, 153)
        Me.BsDataGridView1.TabIndex = 175
        Me.BsDataGridView1.TabToEnter = False
        '
        'Column1
        '
        Me.Column1.HeaderText = "Column1"
        Me.Column1.Name = "Column1"
        '
        'Column2
        '
        Me.Column2.HeaderText = "Column2"
        Me.Column2.Name = "Column2"
        '
        'Column3
        '
        Me.Column3.HeaderText = "Column3"
        Me.Column3.Name = "Column3"
        '
        'Column4
        '
        Me.Column4.HeaderText = "Column4"
        Me.Column4.Name = "Column4"
        '
        'BsButton15
        '
        Me.BsButton15.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton15.Location = New Point(747, 360)
        Me.BsButton15.Name = "BsButton15"
        Me.BsButton15.Size = New Size(120, 45)
        Me.BsButton15.TabIndex = 178
        Me.BsButton15.Text = "BsButton15"
        Me.BsButton15.UseVisualStyleBackColor = True
        '
        'BsButton16
        '
        Me.BsButton16.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton16.CausesValidation = False
        Me.BsButton16.Location = New Point(878, 360)
        Me.BsButton16.Name = "BsButton16"
        Me.BsButton16.Size = New Size(120, 45)
        Me.BsButton16.TabIndex = 179
        Me.BsButton16.Text = "Reset Communications"
        Me.BsButton16.UseVisualStyleBackColor = True
        '
        'BsButton17
        '
        Me.BsButton17.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton17.Location = New Point(747, 447)
        Me.BsButton17.Name = "BsButton17"
        Me.BsButton17.Size = New Size(120, 45)
        Me.BsButton17.TabIndex = 180
        Me.BsButton17.Text = "Send UTIL"
        Me.BsButton17.UseVisualStyleBackColor = True
        '
        'BsButton18
        '
        Me.BsButton18.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton18.Location = New Point(878, 447)
        Me.BsButton18.Name = "BsButton18"
        Me.BsButton18.Size = New Size(120, 45)
        Me.BsButton18.TabIndex = 181
        Me.BsButton18.Text = "ANSUTIL"
        Me.BsButton18.UseVisualStyleBackColor = True
        '
        'BsButton19
        '
        Me.BsButton19.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton19.Location = New Point(747, 516)
        Me.BsButton19.Name = "BsButton19"
        Me.BsButton19.Size = New Size(120, 45)
        Me.BsButton19.TabIndex = 182
        Me.BsButton19.Text = "Alarms"
        Me.BsButton19.UseVisualStyleBackColor = True
        '
        'BsToUpper
        '
        Me.BsToUpper.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsToUpper.Location = New Point(472, 277)
        Me.BsToUpper.Name = "BsToUpper"
        Me.BsToUpper.Size = New Size(120, 21)
        Me.BsToUpper.TabIndex = 183
        Me.BsToUpper.Text = "To Upper"
        Me.BsToUpper.UseVisualStyleBackColor = True
        '
        'BsToUpperTextBox
        '
        Me.BsToUpperTextBox.BackColor = Color.White
        Me.BsToUpperTextBox.DecimalsValues = False
        Me.BsToUpperTextBox.Font = New Font("Verdana", 8.25!)
        Me.BsToUpperTextBox.IsNumeric = False
        Me.BsToUpperTextBox.Location = New Point(269, 277)
        Me.BsToUpperTextBox.Mandatory = False
        Me.BsToUpperTextBox.Name = "BsToUpperTextBox"
        Me.BsToUpperTextBox.Size = New Size(186, 21)
        Me.BsToUpperTextBox.TabIndex = 184
        '
        'BsCompareTextBox
        '
        Me.BsCompareTextBox.BackColor = Color.White
        Me.BsCompareTextBox.DecimalsValues = False
        Me.BsCompareTextBox.Font = New Font("Verdana", 8.25!)
        Me.BsCompareTextBox.IsNumeric = False
        Me.BsCompareTextBox.Location = New Point(269, 318)
        Me.BsCompareTextBox.Mandatory = False
        Me.BsCompareTextBox.Name = "BsCompareTextBox"
        Me.BsCompareTextBox.Size = New Size(186, 21)
        Me.BsCompareTextBox.TabIndex = 185
        '
        'BsCompareButton
        '
        Me.BsCompareButton.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsCompareButton.Location = New Point(472, 317)
        Me.BsCompareButton.Name = "BsCompareButton"
        Me.BsCompareButton.Size = New Size(120, 21)
        Me.BsCompareButton.TabIndex = 186
        Me.BsCompareButton.Text = "Compare"
        Me.BsCompareButton.UseVisualStyleBackColor = True
        '
        'FormSergio
        '
        Me.Appearance.BackColor = Color.Gainsboro
        Me.Appearance.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(1010, 598)
        Me.Controls.Add(Me.BsCompareButton)
        Me.Controls.Add(Me.BsCompareTextBox)
        Me.Controls.Add(Me.BsToUpperTextBox)
        Me.Controls.Add(Me.BsToUpper)
        Me.Controls.Add(Me.BsButton19)
        Me.Controls.Add(Me.BsButton18)
        Me.Controls.Add(Me.BsButton17)
        Me.Controls.Add(Me.BsButton16)
        Me.Controls.Add(Me.BsButton15)
        Me.Controls.Add(Me.BsDataGridView1)
        Me.Controls.Add(Me.BsButton14)
        Me.Controls.Add(Me.BsButton13)
        Me.Controls.Add(Me.BsButton12)
        Me.Controls.Add(Me.BsButton11)
        Me.Controls.Add(Me.BsButton7)
        Me.Controls.Add(Me.BsButton6)
        Me.Controls.Add(Me.ISEResultsTextBox)
        Me.Controls.Add(Me.BsButton10)
        Me.Controls.Add(Me.BsButton9)
        Me.Controls.Add(Me.BsButton8)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BsButton5)
        Me.Controls.Add(Me.BsButton4)
        Me.Controls.Add(Me.BsButton3)
        Me.Controls.Add(Me.BsButton2)
        Me.Controls.Add(Me.BsButton1)
        Me.Controls.Add(Me.BsExceptionsButton)
        Me.Controls.Add(Me.BsCloseButton)
        Me.Cursor = Cursors.Default
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "FormSergio"
        Me.Text = "FormSergio"
        CType(Me.BsDataGridView1, ISupportInitialize).EndInit()
        CType(Me.BsBindingSource1, ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents IdDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents AdjustmentGroupDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents DescriptionDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents AdjustmentIdDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents AxisIdDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents GroupFwDataGridViewTextBoxColumn As DataGridViewTextBoxColumn
    Friend WithEvents BsCloseButton As BSButton
    Friend WithEvents BsExceptionsButton As BSButton
    Friend WithEvents BsButton1 As BSButton
    Friend WithEvents BsButton2 As BSButton
    Friend WithEvents BsButton3 As BSButton
    Friend WithEvents BsButton4 As BSButton
    Friend WithEvents BsButton5 As BSButton
    Friend WithEvents Label1 As Label
    Friend WithEvents BsButton8 As BSButton
    Friend WithEvents BsButton9 As BSButton
    Friend WithEvents BsButton10 As BSButton
    Friend WithEvents ISEResultsTextBox As TextBox
    Friend WithEvents BsButton6 As BSButton
    Friend WithEvents BsButton7 As BSButton
    Friend WithEvents BsButton11 As BSButton
    Friend WithEvents BsButton12 As BSButton
    Friend WithEvents BsButton13 As BSButton
    Friend WithEvents BsButton14 As BSButton
    Friend WithEvents BsDataGridView1 As BSDataGridView
    Friend WithEvents Column1 As DataGridViewButtonColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents Column4 As DataGridViewButtonColumn
    Friend WithEvents BsBindingSource1 As BSBindingSource
    Friend WithEvents BsButton15 As BSButton
    Friend WithEvents BsButton16 As BSButton
    Friend WithEvents BsButton17 As BSButton
    Friend WithEvents BsButton18 As BSButton
    Friend WithEvents BsButton19 As BSButton
    Friend WithEvents BsToUpper As BSButton
    Friend WithEvents BsToUpperTextBox As BSTextBox
    Friend WithEvents BsCompareTextBox As BSTextBox
    Friend WithEvents BsCompareButton As BSButton

End Class
