<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ILISUtilities
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ILISUtilities))
        Me.BsGroupBox1 = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.ExecuteActionButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.LISUtilitiesLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.MainGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.TaceLevelPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.DelInternalQPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.DelLISOrdersPictureBox = New Biosystems.Ax00.Controls.UserControls.BSPictureBox()
        Me.TraceLevelCombo = New Biosystems.Ax00.Controls.UserControls.BSComboBox()
        Me.TracinLevelRB = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.DeleteInternalQueue = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.DeleteLISOrdersRB = New Biosystems.Ax00.Controls.UserControls.BSRadioButton()
        Me.bsCancelButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.BsGroupBox1.SuspendLayout()
        Me.MainGroupBox.SuspendLayout()
        CType(Me.TaceLevelPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DelInternalQPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DelLISOrdersPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsGroupBox1
        '
        Me.BsGroupBox1.Controls.Add(Me.ExecuteActionButton)
        Me.BsGroupBox1.Controls.Add(Me.LISUtilitiesLabel)
        Me.BsGroupBox1.Controls.Add(Me.MainGroupBox)
        Me.BsGroupBox1.ForeColor = System.Drawing.Color.Black
        Me.BsGroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.BsGroupBox1.Name = "BsGroupBox1"
        Me.BsGroupBox1.Size = New System.Drawing.Size(427, 227)
        Me.BsGroupBox1.TabIndex = 6
        Me.BsGroupBox1.TabStop = False
        '
        'ExecuteActionButton
        '
        Me.ExecuteActionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExecuteActionButton.Image = CType(resources.GetObject("ExecuteActionButton.Image"), System.Drawing.Image)
        Me.ExecuteActionButton.Location = New System.Drawing.Point(389, 51)
        Me.ExecuteActionButton.Name = "ExecuteActionButton"
        Me.ExecuteActionButton.Size = New System.Drawing.Size(32, 32)
        Me.ExecuteActionButton.TabIndex = 6
        Me.ExecuteActionButton.UseVisualStyleBackColor = True
        '
        'LISUtilitiesLabel
        '
        Me.LISUtilitiesLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.LISUtilitiesLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.LISUtilitiesLabel.ForeColor = System.Drawing.Color.Black
        Me.LISUtilitiesLabel.Location = New System.Drawing.Point(6, 20)
        Me.LISUtilitiesLabel.Name = "LISUtilitiesLabel"
        Me.LISUtilitiesLabel.Size = New System.Drawing.Size(415, 20)
        Me.LISUtilitiesLabel.TabIndex = 5
        Me.LISUtilitiesLabel.Text = "*LIS Utilities"
        Me.LISUtilitiesLabel.Title = True
        '
        'MainGroupBox
        '
        Me.MainGroupBox.Controls.Add(Me.TaceLevelPictureBox)
        Me.MainGroupBox.Controls.Add(Me.DelInternalQPictureBox)
        Me.MainGroupBox.Controls.Add(Me.DelLISOrdersPictureBox)
        Me.MainGroupBox.Controls.Add(Me.TraceLevelCombo)
        Me.MainGroupBox.Controls.Add(Me.TracinLevelRB)
        Me.MainGroupBox.Controls.Add(Me.DeleteInternalQueue)
        Me.MainGroupBox.Controls.Add(Me.DeleteLISOrdersRB)
        Me.MainGroupBox.ForeColor = System.Drawing.Color.Black
        Me.MainGroupBox.Location = New System.Drawing.Point(8, 43)
        Me.MainGroupBox.Name = "MainGroupBox"
        Me.MainGroupBox.Size = New System.Drawing.Size(377, 167)
        Me.MainGroupBox.TabIndex = 3
        Me.MainGroupBox.TabStop = False
        '
        'TaceLevelPictureBox
        '
        Me.TaceLevelPictureBox.Location = New System.Drawing.Point(12, 88)
        Me.TaceLevelPictureBox.Name = "TaceLevelPictureBox"
        Me.TaceLevelPictureBox.PositionNumber = 0
        Me.TaceLevelPictureBox.Size = New System.Drawing.Size(24, 24)
        Me.TaceLevelPictureBox.TabIndex = 11
        Me.TaceLevelPictureBox.TabStop = False
        Me.TaceLevelPictureBox.Visible = False
        '
        'DelInternalQPictureBox
        '
        Me.DelInternalQPictureBox.Location = New System.Drawing.Point(12, 54)
        Me.DelInternalQPictureBox.Name = "DelInternalQPictureBox"
        Me.DelInternalQPictureBox.PositionNumber = 0
        Me.DelInternalQPictureBox.Size = New System.Drawing.Size(24, 24)
        Me.DelInternalQPictureBox.TabIndex = 10
        Me.DelInternalQPictureBox.TabStop = False
        Me.DelInternalQPictureBox.Visible = False
        '
        'DelLISOrdersPictureBox
        '
        Me.DelLISOrdersPictureBox.Location = New System.Drawing.Point(12, 20)
        Me.DelLISOrdersPictureBox.Name = "DelLISOrdersPictureBox"
        Me.DelLISOrdersPictureBox.PositionNumber = 0
        Me.DelLISOrdersPictureBox.Size = New System.Drawing.Size(24, 24)
        Me.DelLISOrdersPictureBox.TabIndex = 9
        Me.DelLISOrdersPictureBox.TabStop = False
        Me.DelLISOrdersPictureBox.Visible = False
        '
        'TraceLevelCombo
        '
        Me.TraceLevelCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.TraceLevelCombo.ForeColor = System.Drawing.Color.Black
        Me.TraceLevelCombo.FormattingEnabled = True
        Me.TraceLevelCombo.Location = New System.Drawing.Point(69, 125)
        Me.TraceLevelCombo.Name = "TraceLevelCombo"
        Me.TraceLevelCombo.Size = New System.Drawing.Size(174, 21)
        Me.TraceLevelCombo.TabIndex = 8
        '
        'TracinLevelRB
        '
        Me.TracinLevelRB.BackColor = System.Drawing.Color.Transparent
        Me.TracinLevelRB.Location = New System.Drawing.Point(42, 84)
        Me.TracinLevelRB.Name = "TracinLevelRB"
        Me.TracinLevelRB.Size = New System.Drawing.Size(316, 32)
        Me.TracinLevelRB.TabIndex = 7
        Me.TracinLevelRB.TabStop = True
        Me.TracinLevelRB.Text = "*Selection of Tracing level for ES log"
        Me.TracinLevelRB.TextAlign = System.Drawing.ContentAlignment.TopLeft
        Me.TracinLevelRB.UseVisualStyleBackColor = False
        '
        'DeleteInternalQueue
        '
        Me.DeleteInternalQueue.AutoSize = True
        Me.DeleteInternalQueue.BackColor = System.Drawing.Color.Transparent
        Me.DeleteInternalQueue.Location = New System.Drawing.Point(42, 58)
        Me.DeleteInternalQueue.Name = "DeleteInternalQueue"
        Me.DeleteInternalQueue.Size = New System.Drawing.Size(163, 17)
        Me.DeleteInternalQueue.TabIndex = 6
        Me.DeleteInternalQueue.TabStop = True
        Me.DeleteInternalQueue.Text = "*Delete Internal queues"
        Me.DeleteInternalQueue.UseVisualStyleBackColor = False
        '
        'DeleteLISOrdersRB
        '
        Me.DeleteLISOrdersRB.AutoSize = True
        Me.DeleteLISOrdersRB.BackColor = System.Drawing.Color.Transparent
        Me.DeleteLISOrdersRB.Location = New System.Drawing.Point(42, 23)
        Me.DeleteLISOrdersRB.Name = "DeleteLISOrdersRB"
        Me.DeleteLISOrdersRB.Size = New System.Drawing.Size(133, 17)
        Me.DeleteLISOrdersRB.TabIndex = 5
        Me.DeleteLISOrdersRB.TabStop = True
        Me.DeleteLISOrdersRB.Text = "*Delete LIS orders"
        Me.DeleteLISOrdersRB.UseVisualStyleBackColor = False
        '
        'bsCancelButton
        '
        Me.bsCancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsCancelButton.Image = CType(resources.GetObject("bsCancelButton.Image"), System.Drawing.Image)
        Me.bsCancelButton.Location = New System.Drawing.Point(401, 245)
        Me.bsCancelButton.Name = "bsCancelButton"
        Me.bsCancelButton.Size = New System.Drawing.Size(32, 32)
        Me.bsCancelButton.TabIndex = 5
        Me.bsCancelButton.UseVisualStyleBackColor = True
        '
        'ILISUtilities
        '
        Me.AcceptButton = Me.bsCancelButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(451, 286)
        Me.Controls.Add(Me.BsGroupBox1)
        Me.Controls.Add(Me.bsCancelButton)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MinimizeBox = False
        Me.Name = "ILISUtilities"
        Me.Text = "*LIS Utilities"
        Me.BsGroupBox1.ResumeLayout(False)
        Me.MainGroupBox.ResumeLayout(False)
        Me.MainGroupBox.PerformLayout()
        CType(Me.TaceLevelPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DelInternalQPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DelLISOrdersPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents BsGroupBox1 As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents ExecuteActionButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents LISUtilitiesLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents MainGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents TraceLevelCombo As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents TracinLevelRB As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents DeleteInternalQueue As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents DeleteLISOrdersRB As Biosystems.Ax00.Controls.UserControls.BSRadioButton
    Friend WithEvents bsCancelButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents TaceLevelPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents DelInternalQPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
    Friend WithEvents DelLISOrdersPictureBox As Biosystems.Ax00.Controls.UserControls.BSPictureBox
End Class
