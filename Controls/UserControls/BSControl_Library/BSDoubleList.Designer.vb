
Namespace Biosystems.Ax00.Controls.UserControls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BSDoubleList
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
            Me.SelectableElementsTitleLabel = New System.Windows.Forms.Label
            Me.SelectableElementsListView = New System.Windows.Forms.ListView
            Me.Description = New System.Windows.Forms.ColumnHeader
            Me.Code = New System.Windows.Forms.ColumnHeader
            Me.SelectedElementsTitleLabel = New System.Windows.Forms.Label
            Me.SelectedElementsListView = New System.Windows.Forms.ListView
            Me.Descriptions = New System.Windows.Forms.ColumnHeader
            Me.Codes = New System.Windows.Forms.ColumnHeader
            Me.UnselectAllSelectedElementsButton = New System.Windows.Forms.Button
            Me.UnselectChosenSelectedElementButton = New System.Windows.Forms.Button
            Me.SelectChosenButton = New System.Windows.Forms.Button
            Me.SelectALLSelectableElementsButton = New System.Windows.Forms.Button
            Me.SuspendLayout()
            '
            'SelectableElementsTitleLabel
            '
            Me.SelectableElementsTitleLabel.AutoSize = True
            Me.SelectableElementsTitleLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.SelectableElementsTitleLabel.Location = New System.Drawing.Point(3, 9)
            Me.SelectableElementsTitleLabel.Name = "SelectableElementsTitleLabel"
            Me.SelectableElementsTitleLabel.Size = New System.Drawing.Size(142, 13)
            Me.SelectableElementsTitleLabel.TabIndex = 2
            Me.SelectableElementsTitleLabel.Text = "SelectableElementsTitle"
            '
            'SelectableElementsListView
            '
            Me.SelectableElementsListView.Activation = System.Windows.Forms.ItemActivation.OneClick
            Me.SelectableElementsListView.AllowDrop = True
            Me.SelectableElementsListView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.SelectableElementsListView.BackColor = System.Drawing.Color.White
            Me.SelectableElementsListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Description, Me.Code})
            Me.SelectableElementsListView.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.SelectableElementsListView.ForeColor = System.Drawing.Color.Black
            Me.SelectableElementsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
            Me.SelectableElementsListView.Location = New System.Drawing.Point(6, 25)
            Me.SelectableElementsListView.Name = "SelectableElementsListView"
            Me.SelectableElementsListView.ShowGroups = False
            Me.SelectableElementsListView.Size = New System.Drawing.Size(186, 360)
            Me.SelectableElementsListView.TabIndex = 0
            Me.SelectableElementsListView.UseCompatibleStateImageBehavior = False
            Me.SelectableElementsListView.View = System.Windows.Forms.View.Details
            '
            'Description
            '
            Me.Description.Width = 150
            '
            'Code
            '
            Me.Code.Text = ""
            Me.Code.Width = 0
            '
            'SelectedElementsTitleLabel
            '
            Me.SelectedElementsTitleLabel.AutoSize = True
            Me.SelectedElementsTitleLabel.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.SelectedElementsTitleLabel.Location = New System.Drawing.Point(258, 9)
            Me.SelectedElementsTitleLabel.Name = "SelectedElementsTitleLabel"
            Me.SelectedElementsTitleLabel.Size = New System.Drawing.Size(132, 13)
            Me.SelectedElementsTitleLabel.TabIndex = 1
            Me.SelectedElementsTitleLabel.Text = "SelectedElementsTitle"
            '
            'SelectedElementsListView
            '
            Me.SelectedElementsListView.Activation = System.Windows.Forms.ItemActivation.OneClick
            Me.SelectedElementsListView.AllowDrop = True
            Me.SelectedElementsListView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.SelectedElementsListView.BackColor = System.Drawing.Color.White
            Me.SelectedElementsListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Descriptions, Me.Codes})
            Me.SelectedElementsListView.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.SelectedElementsListView.ForeColor = System.Drawing.Color.Black
            Me.SelectedElementsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
            Me.SelectedElementsListView.Location = New System.Drawing.Point(261, 25)
            Me.SelectedElementsListView.Name = "SelectedElementsListView"
            Me.SelectedElementsListView.Size = New System.Drawing.Size(186, 360)
            Me.SelectedElementsListView.TabIndex = 5
            Me.SelectedElementsListView.UseCompatibleStateImageBehavior = False
            Me.SelectedElementsListView.View = System.Windows.Forms.View.Details
            '
            'Descriptions
            '
            Me.Descriptions.Width = 180
            '
            'Codes
            '
            Me.Codes.Text = ""
            Me.Codes.Width = 0
            '
            'UnselectAllSelectedElementsButton
            '
            Me.UnselectAllSelectedElementsButton.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.UnselectAllSelectedElementsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.UnselectAllSelectedElementsButton.BackColor = System.Drawing.Color.Gainsboro
            Me.UnselectAllSelectedElementsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.UnselectAllSelectedElementsButton.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.UnselectAllSelectedElementsButton.ForeColor = System.Drawing.Color.Black
            Me.UnselectAllSelectedElementsButton.Location = New System.Drawing.Point(211, 223)
            Me.UnselectAllSelectedElementsButton.Name = "UnselectAllSelectedElementsButton"
            Me.UnselectAllSelectedElementsButton.Size = New System.Drawing.Size(32, 32)
            Me.UnselectAllSelectedElementsButton.TabIndex = 4
            Me.UnselectAllSelectedElementsButton.UseVisualStyleBackColor = True
            '
            'UnselectChosenSelectedElementButton
            '
            Me.UnselectChosenSelectedElementButton.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.UnselectChosenSelectedElementButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.UnselectChosenSelectedElementButton.BackColor = System.Drawing.Color.Gainsboro
            Me.UnselectChosenSelectedElementButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.UnselectChosenSelectedElementButton.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.UnselectChosenSelectedElementButton.ForeColor = System.Drawing.Color.Black
            Me.UnselectChosenSelectedElementButton.Location = New System.Drawing.Point(211, 149)
            Me.UnselectChosenSelectedElementButton.Name = "UnselectChosenSelectedElementButton"
            Me.UnselectChosenSelectedElementButton.Size = New System.Drawing.Size(32, 32)
            Me.UnselectChosenSelectedElementButton.TabIndex = 2
            Me.UnselectChosenSelectedElementButton.UseVisualStyleBackColor = True
            '
            'SelectChosenButton
            '
            Me.SelectChosenButton.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.SelectChosenButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.SelectChosenButton.BackColor = System.Drawing.Color.Gainsboro
            Me.SelectChosenButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.SelectChosenButton.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.SelectChosenButton.ForeColor = System.Drawing.Color.Black
            Me.SelectChosenButton.Location = New System.Drawing.Point(211, 112)
            Me.SelectChosenButton.Name = "SelectChosenButton"
            Me.SelectChosenButton.Size = New System.Drawing.Size(32, 32)
            Me.SelectChosenButton.TabIndex = 1
            Me.SelectChosenButton.UseVisualStyleBackColor = True
            '
            'SelectALLSelectableElementsButton
            '
            Me.SelectALLSelectableElementsButton.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.SelectALLSelectableElementsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.SelectALLSelectableElementsButton.BackColor = System.Drawing.Color.Gainsboro
            Me.SelectALLSelectableElementsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            Me.SelectALLSelectableElementsButton.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro
            Me.SelectALLSelectableElementsButton.Font = New System.Drawing.Font("Verdana", 8.25!)
            Me.SelectALLSelectableElementsButton.ForeColor = System.Drawing.Color.Black
            Me.SelectALLSelectableElementsButton.Location = New System.Drawing.Point(211, 186)
            Me.SelectALLSelectableElementsButton.Name = "SelectALLSelectableElementsButton"
            Me.SelectALLSelectableElementsButton.Size = New System.Drawing.Size(32, 32)
            Me.SelectALLSelectableElementsButton.TabIndex = 3
            Me.SelectALLSelectableElementsButton.UseVisualStyleBackColor = True
            '
            'BSDoubleList
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
            Me.BackColor = System.Drawing.Color.Gainsboro
            Me.Controls.Add(Me.SelectedElementsTitleLabel)
            Me.Controls.Add(Me.UnselectAllSelectedElementsButton)
            Me.Controls.Add(Me.SelectedElementsListView)
            Me.Controls.Add(Me.SelectableElementsTitleLabel)
            Me.Controls.Add(Me.UnselectChosenSelectedElementButton)
            Me.Controls.Add(Me.SelectChosenButton)
            Me.Controls.Add(Me.SelectableElementsListView)
            Me.Controls.Add(Me.SelectALLSelectableElementsButton)
            Me.Name = "BSDoubleList"
            Me.Size = New System.Drawing.Size(450, 391)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents SelectableElementsTitleLabel As System.Windows.Forms.Label
        Friend WithEvents SelectableElementsListView As System.Windows.Forms.ListView
        Friend WithEvents SelectedElementsListView As System.Windows.Forms.ListView
        Friend WithEvents SelectedElementsTitleLabel As System.Windows.Forms.Label
        Friend WithEvents Description As System.Windows.Forms.ColumnHeader
        Friend WithEvents Code As System.Windows.Forms.ColumnHeader
        Friend WithEvents Descriptions As System.Windows.Forms.ColumnHeader
        Friend WithEvents Codes As System.Windows.Forms.ColumnHeader
        Friend WithEvents UnselectAllSelectedElementsButton As System.Windows.Forms.Button
        Friend WithEvents UnselectChosenSelectedElementButton As System.Windows.Forms.Button
        Friend WithEvents SelectChosenButton As System.Windows.Forms.Button
        Friend WithEvents SelectALLSelectableElementsButton As System.Windows.Forms.Button

    End Class
End Namespace