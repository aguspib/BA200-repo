<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UiWSDeleteAuxScreen
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub



    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UiWSDeleteAuxScreen))
        Me.bsExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsDeleteButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.bsListViewTitleLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.bsElementsListView = New Biosystems.Ax00.Controls.UserControls.BSListView
        Me.bsAuxDeleteGroupBox = New Biosystems.Ax00.Controls.UserControls.BSGroupBox
        Me.bsScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip
        Me.BsTimer1 = New Biosystems.Ax00.Controls.UserControls.BSTimer
        Me.BsBorderedPanel1 = New bsBorderedPanel
        Me.bsAuxDeleteGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'bsExitButton
        '
        Me.bsExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsExitButton.ForeColor = System.Drawing.Color.Black
        Me.bsExitButton.Location = New System.Drawing.Point(258, 420)
        Me.bsExitButton.Name = "bsExitButton"
        Me.bsExitButton.Size = New System.Drawing.Size(32, 32)
        Me.bsExitButton.TabIndex = 2
        Me.bsExitButton.UseVisualStyleBackColor = True
        '
        'bsDeleteButton
        '
        Me.bsDeleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.bsDeleteButton.ForeColor = System.Drawing.Color.Black
        Me.bsDeleteButton.Location = New System.Drawing.Point(10, 420)
        Me.bsDeleteButton.Name = "bsDeleteButton"
        Me.bsDeleteButton.Size = New System.Drawing.Size(32, 32)
        Me.bsDeleteButton.TabIndex = 1
        Me.bsDeleteButton.UseVisualStyleBackColor = True
        '
        'bsListViewTitleLabel
        '
        Me.bsListViewTitleLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.bsListViewTitleLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.bsListViewTitleLabel.ForeColor = System.Drawing.Color.Black
        Me.bsListViewTitleLabel.Location = New System.Drawing.Point(10, 15)
        Me.bsListViewTitleLabel.Name = "bsListViewTitleLabel"
        Me.bsListViewTitleLabel.Size = New System.Drawing.Size(260, 20)
        Me.bsListViewTitleLabel.TabIndex = 46
        Me.bsListViewTitleLabel.Title = True
        '
        'bsElementsListView
        '
        Me.bsElementsListView.AutoArrange = False
        Me.bsElementsListView.BackColor = System.Drawing.Color.White
        Me.bsElementsListView.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bsElementsListView.ForeColor = System.Drawing.Color.Black
        Me.bsElementsListView.FullRowSelect = True
        Me.bsElementsListView.HideSelection = False
        Me.bsElementsListView.Location = New System.Drawing.Point(10, 40)
        Me.bsElementsListView.Name = "bsElementsListView"
        Me.bsElementsListView.Size = New System.Drawing.Size(260, 350)
        Me.bsElementsListView.TabIndex = 0
        Me.bsElementsListView.UseCompatibleStateImageBehavior = False
        Me.bsElementsListView.View = System.Windows.Forms.View.Details
        '
        'bsAuxDeleteGroupBox
        '
        Me.bsAuxDeleteGroupBox.Controls.Add(Me.bsElementsListView)
        Me.bsAuxDeleteGroupBox.Controls.Add(Me.bsListViewTitleLabel)
        Me.bsAuxDeleteGroupBox.ForeColor = System.Drawing.Color.Black
        Me.bsAuxDeleteGroupBox.Location = New System.Drawing.Point(10, 10)
        Me.bsAuxDeleteGroupBox.Name = "bsAuxDeleteGroupBox"
        Me.bsAuxDeleteGroupBox.Size = New System.Drawing.Size(280, 405)
        Me.bsAuxDeleteGroupBox.TabIndex = 48
        Me.bsAuxDeleteGroupBox.TabStop = False
        '
        'BsTimer1
        '
        Me.BsTimer1.Interval = 200
        '
        'BsBorderedPanel1
        '
        Me.BsBorderedPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsBorderedPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsBorderedPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsBorderedPanel1.Name = "BsBorderedPanel1"
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(302, 459)
        Me.BsBorderedPanel1.TabIndex = 49
        '
        'IWSDeleteAuxScreen
        '
        Me.AcceptButton = Me.bsExitButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.ForeColor = System.Drawing.Color.Black
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.Appearance.Options.UseForeColor = True
        Me.ClientSize = New System.Drawing.Size(302, 459)
        Me.ControlBox = False
        Me.Controls.Add(Me.bsAuxDeleteGroupBox)
        Me.Controls.Add(Me.bsExitButton)
        Me.Controls.Add(Me.bsDeleteButton)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UiWSDeleteAuxScreen"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.bsAuxDeleteGroupBox.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents bsExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsDeleteButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents bsListViewTitleLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsElementsListView As Biosystems.Ax00.Controls.UserControls.BSListView
    Friend WithEvents bsAuxDeleteGroupBox As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents bsScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents BsTimer1 As Biosystems.Ax00.Controls.UserControls.BSTimer
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel

End Class
