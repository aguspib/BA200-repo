Namespace Biosystems.Ax00.Controls.UserControls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BsDoubleListOld
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
            Me.BsButtonPanel = New Biosystems.Ax00.Controls.UserControls.BSPanel
            Me.BsMoveLeftToRightSelectedItemButton = New Biosystems.Ax00.Controls.UserControls.BSButton
            Me.BsMoveLeftToRightAllButton = New Biosystems.Ax00.Controls.UserControls.BSButton
            Me.BsMoveRightToLeftSelectedItemButton = New Biosystems.Ax00.Controls.UserControls.BSButton
            Me.BsMoveRightToLeftAllButton = New Biosystems.Ax00.Controls.UserControls.BSButton
            Me.BsRightListView = New Biosystems.Ax00.Controls.UserControls.BSListView
            Me.BsLeftListView = New Biosystems.Ax00.Controls.UserControls.BSListView
            Me.BsRightLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.BsLeftLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
            Me.BsButtonPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            'BsButtonPanel
            '
            Me.BsButtonPanel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsButtonPanel.Controls.Add(Me.BsMoveLeftToRightSelectedItemButton)
            Me.BsButtonPanel.Controls.Add(Me.BsMoveLeftToRightAllButton)
            Me.BsButtonPanel.Controls.Add(Me.BsMoveRightToLeftSelectedItemButton)
            Me.BsButtonPanel.Controls.Add(Me.BsMoveRightToLeftAllButton)
            Me.BsButtonPanel.Location = New System.Drawing.Point(202, 78)
            Me.BsButtonPanel.Name = "BsButtonPanel"
            Me.BsButtonPanel.Size = New System.Drawing.Size(45, 187)
            Me.BsButtonPanel.TabIndex = 10
            '
            'BsMoveLeftToRightSelectedItemButton
            '
            Me.BsMoveLeftToRightSelectedItemButton.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsMoveLeftToRightSelectedItemButton.BackColor = System.Drawing.Color.Gainsboro
            Me.BsMoveLeftToRightSelectedItemButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BsMoveLeftToRightSelectedItemButton.Location = New System.Drawing.Point(7, 59)
            Me.BsMoveLeftToRightSelectedItemButton.Name = "BsMoveLeftToRightSelectedItemButton"
            Me.BsMoveLeftToRightSelectedItemButton.Size = New System.Drawing.Size(31, 31)
            Me.BsMoveLeftToRightSelectedItemButton.TabIndex = 5
            Me.BsMoveLeftToRightSelectedItemButton.Text = ">"
            Me.BsMoveLeftToRightSelectedItemButton.UseVisualStyleBackColor = False
            '
            'BsMoveLeftToRightAllButton
            '
            Me.BsMoveLeftToRightAllButton.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsMoveLeftToRightAllButton.BackColor = System.Drawing.Color.Gainsboro
            Me.BsMoveLeftToRightAllButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BsMoveLeftToRightAllButton.Location = New System.Drawing.Point(7, 19)
            Me.BsMoveLeftToRightAllButton.Name = "BsMoveLeftToRightAllButton"
            Me.BsMoveLeftToRightAllButton.Size = New System.Drawing.Size(31, 31)
            Me.BsMoveLeftToRightAllButton.TabIndex = 4
            Me.BsMoveLeftToRightAllButton.Text = ">>"
            Me.BsMoveLeftToRightAllButton.UseVisualStyleBackColor = False
            '
            'BsMoveRightToLeftSelectedItemButton
            '
            Me.BsMoveRightToLeftSelectedItemButton.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsMoveRightToLeftSelectedItemButton.BackColor = System.Drawing.Color.Gainsboro
            Me.BsMoveRightToLeftSelectedItemButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BsMoveRightToLeftSelectedItemButton.Location = New System.Drawing.Point(7, 99)
            Me.BsMoveRightToLeftSelectedItemButton.Name = "BsMoveRightToLeftSelectedItemButton"
            Me.BsMoveRightToLeftSelectedItemButton.Size = New System.Drawing.Size(31, 31)
            Me.BsMoveRightToLeftSelectedItemButton.TabIndex = 6
            Me.BsMoveRightToLeftSelectedItemButton.Text = "<"
            Me.BsMoveRightToLeftSelectedItemButton.UseVisualStyleBackColor = False
            '
            'BsMoveRightToLeftAllButton
            '
            Me.BsMoveRightToLeftAllButton.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsMoveRightToLeftAllButton.BackColor = System.Drawing.Color.Gainsboro
            Me.BsMoveRightToLeftAllButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BsMoveRightToLeftAllButton.Location = New System.Drawing.Point(7, 139)
            Me.BsMoveRightToLeftAllButton.Name = "BsMoveRightToLeftAllButton"
            Me.BsMoveRightToLeftAllButton.Size = New System.Drawing.Size(31, 31)
            Me.BsMoveRightToLeftAllButton.TabIndex = 7
            Me.BsMoveRightToLeftAllButton.Text = "<<"
            Me.BsMoveRightToLeftAllButton.UseVisualStyleBackColor = False
            '
            'BsRightListView
            '
            Me.BsRightListView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsRightListView.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BsRightListView.ForeColor = System.Drawing.Color.Black
            Me.BsRightListView.FullRowSelect = True
            Me.BsRightListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
            Me.BsRightListView.LabelWrap = False
            Me.BsRightListView.Location = New System.Drawing.Point(253, 22)
            Me.BsRightListView.MultiSelect = False
            Me.BsRightListView.Name = "BsRightListView"
            Me.BsRightListView.Size = New System.Drawing.Size(191, 291)
            Me.BsRightListView.TabIndex = 9
            Me.BsRightListView.TileSize = New System.Drawing.Size(160, 16)
            Me.BsRightListView.UseCompatibleStateImageBehavior = False
            Me.BsRightListView.View = System.Windows.Forms.View.Details
            '
            'BsLeftListView
            '
            Me.BsLeftListView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsLeftListView.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.BsLeftListView.ForeColor = System.Drawing.Color.Black
            Me.BsLeftListView.FullRowSelect = True
            Me.BsLeftListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
            Me.BsLeftListView.HideSelection = False
            Me.BsLeftListView.LabelWrap = False
            Me.BsLeftListView.Location = New System.Drawing.Point(5, 22)
            Me.BsLeftListView.MultiSelect = False
            Me.BsLeftListView.Name = "BsLeftListView"
            Me.BsLeftListView.Size = New System.Drawing.Size(191, 291)
            Me.BsLeftListView.TabIndex = 8
            Me.BsLeftListView.TileSize = New System.Drawing.Size(160, 16)
            Me.BsLeftListView.UseCompatibleStateImageBehavior = False
            Me.BsLeftListView.View = System.Windows.Forms.View.Details
            '
            'BsRightLabel
            '
            Me.BsRightLabel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsRightLabel.BackColor = System.Drawing.SystemColors.Control
            Me.BsRightLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.BsRightLabel.Location = New System.Drawing.Point(254, 6)
            Me.BsRightLabel.Margin = New System.Windows.Forms.Padding(3)
            Me.BsRightLabel.Name = "BsRightLabel"
            Me.BsRightLabel.Size = New System.Drawing.Size(175, 12)
            Me.BsRightLabel.TabIndex = 1
            Me.BsRightLabel.Title = False
            '
            'BsLeftLabel
            '
            Me.BsLeftLabel.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.BsLeftLabel.BackColor = System.Drawing.SystemColors.Control
            Me.BsLeftLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.BsLeftLabel.Location = New System.Drawing.Point(5, 7)
            Me.BsLeftLabel.Margin = New System.Windows.Forms.Padding(3)
            Me.BsLeftLabel.Name = "BsLeftLabel"
            Me.BsLeftLabel.Size = New System.Drawing.Size(175, 12)
            Me.BsLeftLabel.TabIndex = 0
            Me.BsLeftLabel.Title = False
            '
            'BsDoubleList
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.BsButtonPanel)
            Me.Controls.Add(Me.BsRightListView)
            Me.Controls.Add(Me.BsLeftListView)
            Me.Controls.Add(Me.BsRightLabel)
            Me.Controls.Add(Me.BsLeftLabel)
            Me.MaximumSize = New System.Drawing.Size(453, 332)
            Me.MinimumSize = New System.Drawing.Size(440, 317)
            Me.Name = "BsDoubleList"
            Me.Size = New System.Drawing.Size(450, 321)
            Me.BsButtonPanel.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents BsButtonPanel As Biosystems.Ax00.Controls.UserControls.BSPanel
        Friend WithEvents BsMoveLeftToRightSelectedItemButton As Biosystems.Ax00.Controls.UserControls.BSButton
        Friend WithEvents BsMoveLeftToRightAllButton As Biosystems.Ax00.Controls.UserControls.BSButton
        Friend WithEvents BsMoveRightToLeftSelectedItemButton As Biosystems.Ax00.Controls.UserControls.BSButton
        Friend WithEvents BsMoveRightToLeftAllButton As Biosystems.Ax00.Controls.UserControls.BSButton
        Friend WithEvents BsLeftLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents BsRightLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
        Friend WithEvents BsLeftListView As Biosystems.Ax00.Controls.UserControls.BSListView
        Friend WithEvents BsRightListView As Biosystems.Ax00.Controls.UserControls.BSListView

    End Class
End Namespace