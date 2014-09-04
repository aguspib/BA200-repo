<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ISortingTestsAux
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ISortingTestsAux))
        Me.TestSortingGB = New Biosystems.Ax00.Controls.UserControls.BSGroupBox()
        Me.bsTestListGrid = New Biosystems.Ax00.Controls.UserControls.BSDataGridView()
        Me.DefaultSortingButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.LastPosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.DownPosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.UpPosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.FirstPosButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.TestListView = New Biosystems.Ax00.Controls.UserControls.BSListView()
        Me.TestSortingLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel()
        Me.bsAcceptButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.CloseButton = New Biosystems.Ax00.Controls.UserControls.BSButton()
        Me.ScreenToolTips = New Biosystems.Ax00.Controls.UserControls.BSToolTip()
        Me.BsBorderedPanel1 = New bsBorderedPanel()
        Me.TestSortingGB.SuspendLayout()
        CType(Me.bsTestListGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TestSortingGB
        '
        Me.TestSortingGB.Controls.Add(Me.bsTestListGrid)
        Me.TestSortingGB.Controls.Add(Me.DefaultSortingButton)
        Me.TestSortingGB.Controls.Add(Me.LastPosButton)
        Me.TestSortingGB.Controls.Add(Me.DownPosButton)
        Me.TestSortingGB.Controls.Add(Me.UpPosButton)
        Me.TestSortingGB.Controls.Add(Me.FirstPosButton)
        Me.TestSortingGB.Controls.Add(Me.TestListView)
        Me.TestSortingGB.Controls.Add(Me.TestSortingLabel)
        Me.TestSortingGB.ForeColor = System.Drawing.Color.Black
        Me.TestSortingGB.Location = New System.Drawing.Point(12, 5)
        Me.TestSortingGB.Name = "TestSortingGB"
        Me.TestSortingGB.Size = New System.Drawing.Size(291, 497)
        Me.TestSortingGB.TabIndex = 1
        Me.TestSortingGB.TabStop = False
        '
        'bsTestListGrid
        '
        Me.bsTestListGrid.AllowUserToAddRows = False
        Me.bsTestListGrid.AllowUserToDeleteRows = False
        Me.bsTestListGrid.AllowUserToResizeColumns = False
        Me.bsTestListGrid.AllowUserToResizeRows = False
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DodgerBlue
        Me.bsTestListGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.bsTestListGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.bsTestListGrid.BackgroundColor = System.Drawing.Color.White
        Me.bsTestListGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.bsTestListGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None
        Me.bsTestListGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.bsTestListGrid.ColumnHeadersHeight = 20
        Me.bsTestListGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.DodgerBlue
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListGrid.DefaultCellStyle = DataGridViewCellStyle3
        Me.bsTestListGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.bsTestListGrid.EnterToTab = False
        Me.bsTestListGrid.GridColor = System.Drawing.Color.Silver
        Me.bsTestListGrid.Location = New System.Drawing.Point(9, 250)
        Me.bsTestListGrid.MultiSelect = False
        Me.bsTestListGrid.Name = "bsTestListGrid"
        Me.bsTestListGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.bsTestListGrid.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.bsTestListGrid.RowHeadersVisible = False
        Me.bsTestListGrid.RowHeadersWidth = 20
        Me.bsTestListGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.DodgerBlue
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.bsTestListGrid.RowsDefaultCellStyle = DataGridViewCellStyle5
        Me.bsTestListGrid.RowTemplate.Height = 30
        Me.bsTestListGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.bsTestListGrid.Size = New System.Drawing.Size(237, 238)
        Me.bsTestListGrid.TabIndex = 41
        Me.bsTestListGrid.TabToEnter = False
        '
        'DefaultSortingButton
        '
        Me.DefaultSortingButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.DefaultSortingButton.Location = New System.Drawing.Point(252, 454)
        Me.DefaultSortingButton.Name = "DefaultSortingButton"
        Me.DefaultSortingButton.Size = New System.Drawing.Size(32, 32)
        Me.DefaultSortingButton.TabIndex = 8
        Me.DefaultSortingButton.UseVisualStyleBackColor = True
        '
        'LastPosButton
        '
        Me.LastPosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.LastPosButton.Location = New System.Drawing.Point(252, 295)
        Me.LastPosButton.Name = "LastPosButton"
        Me.LastPosButton.Size = New System.Drawing.Size(32, 32)
        Me.LastPosButton.TabIndex = 6
        Me.LastPosButton.UseVisualStyleBackColor = True
        '
        'DownPosButton
        '
        Me.DownPosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.DownPosButton.Location = New System.Drawing.Point(252, 256)
        Me.DownPosButton.Name = "DownPosButton"
        Me.DownPosButton.Size = New System.Drawing.Size(32, 32)
        Me.DownPosButton.TabIndex = 5
        Me.DownPosButton.UseVisualStyleBackColor = True
        '
        'UpPosButton
        '
        Me.UpPosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.UpPosButton.Location = New System.Drawing.Point(252, 217)
        Me.UpPosButton.Name = "UpPosButton"
        Me.UpPosButton.Size = New System.Drawing.Size(32, 32)
        Me.UpPosButton.TabIndex = 4
        Me.UpPosButton.UseVisualStyleBackColor = True
        '
        'FirstPosButton
        '
        Me.FirstPosButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.FirstPosButton.Location = New System.Drawing.Point(252, 178)
        Me.FirstPosButton.Name = "FirstPosButton"
        Me.FirstPosButton.Size = New System.Drawing.Size(32, 32)
        Me.FirstPosButton.TabIndex = 3
        Me.FirstPosButton.UseVisualStyleBackColor = True
        '
        'TestListView
        '
        Me.TestListView.BackColor = System.Drawing.Color.White
        Me.TestListView.ForeColor = System.Drawing.Color.Black
        Me.TestListView.FullRowSelect = True
        Me.TestListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.TestListView.HideSelection = False
        Me.TestListView.Location = New System.Drawing.Point(9, 43)
        Me.TestListView.Name = "TestListView"
        Me.TestListView.Size = New System.Drawing.Size(237, 200)
        Me.TestListView.TabIndex = 2
        Me.TestListView.UseCompatibleStateImageBehavior = False
        Me.TestListView.View = System.Windows.Forms.View.Details
        '
        'TestSortingLabel
        '
        Me.TestSortingLabel.BackColor = System.Drawing.Color.LightSteelBlue
        Me.TestSortingLabel.Font = New System.Drawing.Font("Verdana", 10.0!)
        Me.TestSortingLabel.ForeColor = System.Drawing.Color.Black
        Me.TestSortingLabel.Location = New System.Drawing.Point(9, 16)
        Me.TestSortingLabel.Name = "TestSortingLabel"
        Me.TestSortingLabel.Size = New System.Drawing.Size(275, 20)
        Me.TestSortingLabel.TabIndex = 1
        Me.TestSortingLabel.Text = "* Test Sorting for Reports"
        Me.TestSortingLabel.Title = True
        '
        'bsAcceptButton
        '
        Me.bsAcceptButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.bsAcceptButton.Location = New System.Drawing.Point(232, 508)
        Me.bsAcceptButton.Name = "bsAcceptButton"
        Me.bsAcceptButton.Size = New System.Drawing.Size(32, 32)
        Me.bsAcceptButton.TabIndex = 2
        Me.bsAcceptButton.UseVisualStyleBackColor = True
        '
        'CloseButton
        '
        Me.CloseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.CloseButton.Location = New System.Drawing.Point(271, 508)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(32, 32)
        Me.CloseButton.TabIndex = 3
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'BsBorderedPanel1
        '
        Me.BsBorderedPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BsBorderedPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BsBorderedPanel1.Location = New System.Drawing.Point(0, 0)
        Me.BsBorderedPanel1.Name = "BsBorderedPanel1"
        Me.BsBorderedPanel1.Size = New System.Drawing.Size(317, 546)
        Me.BsBorderedPanel1.TabIndex = 4
        '
        'ISortingTestsAux
        '
        Me.AcceptButton = Me.CloseButton
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.ClientSize = New System.Drawing.Size(317, 546)
        Me.ControlBox = False
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.bsAcceptButton)
        Me.Controls.Add(Me.TestSortingGB)
        Me.Controls.Add(Me.BsBorderedPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "ISortingTestsAux"
        Me.ShowInTaskbar = False
        Me.Text = " "
        Me.TestSortingGB.ResumeLayout(False)
        CType(Me.bsTestListGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TestSortingGB As Biosystems.Ax00.Controls.UserControls.BSGroupBox
    Friend WithEvents LastPosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents DownPosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents UpPosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents FirstPosButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents TestListView As Biosystems.Ax00.Controls.UserControls.BSListView
    Friend WithEvents TestSortingLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents bsAcceptButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents CloseButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ScreenToolTips As Biosystems.Ax00.Controls.UserControls.BSToolTip
    Friend WithEvents DefaultSortingButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsBorderedPanel1 As bsBorderedPanel
    Friend WithEvents bsTestListGrid As Biosystems.Ax00.Controls.UserControls.BSDataGridView

End Class
