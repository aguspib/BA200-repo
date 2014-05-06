<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AppConfiguration
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.DataBaseAccess = New System.Windows.Forms.TabPage
        Me.CloseButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.ConnectionStringTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.IntegratedSecurityCheckbox = New Biosystems.Ax00.Controls.UserControls.BSCheckbox
        Me.ChangeAppConfigButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsLabel4 = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.DBUserIDLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.DataBaseNameLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.SqlServerLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.PasswordTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.UserIDTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.DataBaseNameTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.DataSourceTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.InstalledApplicationsTAB = New System.Windows.Forms.TabPage
        Me.GetApplicationButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.IntalledApplicationGridView = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.TabControl1.SuspendLayout()
        Me.DataBaseAccess.SuspendLayout()
        Me.InstalledApplicationsTAB.SuspendLayout()
        CType(Me.IntalledApplicationGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.DataBaseAccess)
        Me.TabControl1.Controls.Add(Me.InstalledApplicationsTAB)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(478, 414)
        Me.TabControl1.TabIndex = 0
        '
        'DataBaseAccess
        '
        Me.DataBaseAccess.BackColor = System.Drawing.Color.Gainsboro
        Me.DataBaseAccess.Controls.Add(Me.CloseButton)
        Me.DataBaseAccess.Controls.Add(Me.ConnectionStringTextBox)
        Me.DataBaseAccess.Controls.Add(Me.IntegratedSecurityCheckbox)
        Me.DataBaseAccess.Controls.Add(Me.ChangeAppConfigButton)
        Me.DataBaseAccess.Controls.Add(Me.BsLabel4)
        Me.DataBaseAccess.Controls.Add(Me.DBUserIDLabel)
        Me.DataBaseAccess.Controls.Add(Me.DataBaseNameLabel)
        Me.DataBaseAccess.Controls.Add(Me.SqlServerLabel)
        Me.DataBaseAccess.Controls.Add(Me.PasswordTextBox)
        Me.DataBaseAccess.Controls.Add(Me.UserIDTextBox)
        Me.DataBaseAccess.Controls.Add(Me.DataBaseNameTextBox)
        Me.DataBaseAccess.Controls.Add(Me.DataSourceTextBox)
        Me.DataBaseAccess.Location = New System.Drawing.Point(4, 22)
        Me.DataBaseAccess.Name = "DataBaseAccess"
        Me.DataBaseAccess.Padding = New System.Windows.Forms.Padding(3)
        Me.DataBaseAccess.Size = New System.Drawing.Size(470, 388)
        Me.DataBaseAccess.TabIndex = 0
        Me.DataBaseAccess.Text = "Data Base Access"
        '
        'CloseButton
        '
        Me.CloseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.CloseButton.Location = New System.Drawing.Point(395, 362)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(64, 23)
        Me.CloseButton.TabIndex = 11
        Me.CloseButton.Text = "Close"
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'ConnectionStringTextBox
        '
        Me.ConnectionStringTextBox.BackColor = System.Drawing.Color.White
        Me.ConnectionStringTextBox.DecimalsValues = False
        Me.ConnectionStringTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.ConnectionStringTextBox.IsNumeric = False
        Me.ConnectionStringTextBox.Location = New System.Drawing.Point(7, 226)
        Me.ConnectionStringTextBox.Mandatory = False
        Me.ConnectionStringTextBox.Multiline = True
        Me.ConnectionStringTextBox.Name = "ConnectionStringTextBox"
        Me.ConnectionStringTextBox.ReadOnly = True
        Me.ConnectionStringTextBox.Size = New System.Drawing.Size(453, 62)
        Me.ConnectionStringTextBox.TabIndex = 10
        '
        'IntegratedSecurityCheckbox
        '
        Me.IntegratedSecurityCheckbox.AutoSize = True
        Me.IntegratedSecurityCheckbox.Location = New System.Drawing.Point(167, 203)
        Me.IntegratedSecurityCheckbox.Name = "IntegratedSecurityCheckbox"
        Me.IntegratedSecurityCheckbox.Size = New System.Drawing.Size(115, 17)
        Me.IntegratedSecurityCheckbox.TabIndex = 9
        Me.IntegratedSecurityCheckbox.Text = "Integrated Security"
        Me.IntegratedSecurityCheckbox.UseVisualStyleBackColor = True
        '
        'ChangeAppConfigButton
        '
        Me.ChangeAppConfigButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ChangeAppConfigButton.Location = New System.Drawing.Point(153, 309)
        Me.ChangeAppConfigButton.Name = "ChangeAppConfigButton"
        Me.ChangeAppConfigButton.Size = New System.Drawing.Size(111, 23)
        Me.ChangeAppConfigButton.TabIndex = 8
        Me.ChangeAppConfigButton.Text = "Change App.Config"
        Me.ChangeAppConfigButton.UseVisualStyleBackColor = True
        '
        'BsLabel4
        '
        Me.BsLabel4.AutoSize = True
        Me.BsLabel4.BackColor = System.Drawing.Color.Transparent
        Me.BsLabel4.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsLabel4.ForeColor = System.Drawing.Color.Black
        Me.BsLabel4.Location = New System.Drawing.Point(6, 169)
        Me.BsLabel4.Name = "BsLabel4"
        Me.BsLabel4.Size = New System.Drawing.Size(154, 13)
        Me.BsLabel4.TabIndex = 7
        Me.BsLabel4.Text = "Data Base User Password"
        Me.BsLabel4.Title = False
        '
        'DBUserIDLabel
        '
        Me.DBUserIDLabel.AutoSize = True
        Me.DBUserIDLabel.BackColor = System.Drawing.Color.Transparent
        Me.DBUserIDLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.DBUserIDLabel.ForeColor = System.Drawing.Color.Black
        Me.DBUserIDLabel.Location = New System.Drawing.Point(42, 124)
        Me.DBUserIDLabel.Name = "DBUserIDLabel"
        Me.DBUserIDLabel.Size = New System.Drawing.Size(112, 13)
        Me.DBUserIDLabel.TabIndex = 6
        Me.DBUserIDLabel.Text = "Data Base User Id"
        Me.DBUserIDLabel.Title = False
        '
        'DataBaseNameLabel
        '
        Me.DataBaseNameLabel.AutoSize = True
        Me.DataBaseNameLabel.BackColor = System.Drawing.Color.Transparent
        Me.DataBaseNameLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.DataBaseNameLabel.ForeColor = System.Drawing.Color.Black
        Me.DataBaseNameLabel.Location = New System.Drawing.Point(50, 79)
        Me.DataBaseNameLabel.Name = "DataBaseNameLabel"
        Me.DataBaseNameLabel.Size = New System.Drawing.Size(103, 13)
        Me.DataBaseNameLabel.TabIndex = 5
        Me.DataBaseNameLabel.Text = "Data Base Name"
        Me.DataBaseNameLabel.Title = False
        '
        'SqlServerLabel
        '
        Me.SqlServerLabel.AutoSize = True
        Me.SqlServerLabel.BackColor = System.Drawing.Color.Transparent
        Me.SqlServerLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.SqlServerLabel.ForeColor = System.Drawing.Color.Black
        Me.SqlServerLabel.Location = New System.Drawing.Point(48, 34)
        Me.SqlServerLabel.Name = "SqlServerLabel"
        Me.SqlServerLabel.Size = New System.Drawing.Size(105, 13)
        Me.SqlServerLabel.TabIndex = 4
        Me.SqlServerLabel.Text = "Sql Server Name"
        Me.SqlServerLabel.Title = False
        '
        'PasswordTextBox
        '
        Me.PasswordTextBox.BackColor = System.Drawing.Color.White
        Me.PasswordTextBox.DecimalsValues = False
        Me.PasswordTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.PasswordTextBox.IsNumeric = False
        Me.PasswordTextBox.Location = New System.Drawing.Point(164, 161)
        Me.PasswordTextBox.Mandatory = False
        Me.PasswordTextBox.Name = "PasswordTextBox"
        Me.PasswordTextBox.Size = New System.Drawing.Size(222, 21)
        Me.PasswordTextBox.TabIndex = 3
        '
        'UserIDTextBox
        '
        Me.UserIDTextBox.BackColor = System.Drawing.Color.White
        Me.UserIDTextBox.DecimalsValues = False
        Me.UserIDTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.UserIDTextBox.IsNumeric = False
        Me.UserIDTextBox.Location = New System.Drawing.Point(164, 116)
        Me.UserIDTextBox.Mandatory = False
        Me.UserIDTextBox.Name = "UserIDTextBox"
        Me.UserIDTextBox.Size = New System.Drawing.Size(222, 21)
        Me.UserIDTextBox.TabIndex = 2
        '
        'DataBaseNameTextBox
        '
        Me.DataBaseNameTextBox.BackColor = System.Drawing.Color.White
        Me.DataBaseNameTextBox.DecimalsValues = False
        Me.DataBaseNameTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.DataBaseNameTextBox.IsNumeric = False
        Me.DataBaseNameTextBox.Location = New System.Drawing.Point(164, 71)
        Me.DataBaseNameTextBox.Mandatory = False
        Me.DataBaseNameTextBox.Name = "DataBaseNameTextBox"
        Me.DataBaseNameTextBox.Size = New System.Drawing.Size(222, 21)
        Me.DataBaseNameTextBox.TabIndex = 1
        '
        'DataSourceTextBox
        '
        Me.DataSourceTextBox.BackColor = System.Drawing.Color.White
        Me.DataSourceTextBox.DecimalsValues = False
        Me.DataSourceTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.DataSourceTextBox.IsNumeric = False
        Me.DataSourceTextBox.Location = New System.Drawing.Point(164, 26)
        Me.DataSourceTextBox.Mandatory = False
        Me.DataSourceTextBox.Name = "DataSourceTextBox"
        Me.DataSourceTextBox.Size = New System.Drawing.Size(222, 21)
        Me.DataSourceTextBox.TabIndex = 0
        '
        'InstalledApplicationsTAB
        '
        Me.InstalledApplicationsTAB.Controls.Add(Me.GetApplicationButton)
        Me.InstalledApplicationsTAB.Controls.Add(Me.IntalledApplicationGridView)
        Me.InstalledApplicationsTAB.Location = New System.Drawing.Point(4, 22)
        Me.InstalledApplicationsTAB.Name = "InstalledApplicationsTAB"
        Me.InstalledApplicationsTAB.Padding = New System.Windows.Forms.Padding(3)
        Me.InstalledApplicationsTAB.Size = New System.Drawing.Size(470, 388)
        Me.InstalledApplicationsTAB.TabIndex = 1
        Me.InstalledApplicationsTAB.Text = "Installed Applications"
        Me.InstalledApplicationsTAB.UseVisualStyleBackColor = True
        '
        'GetApplicationButton
        '
        Me.GetApplicationButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.GetApplicationButton.Location = New System.Drawing.Point(8, 42)
        Me.GetApplicationButton.Name = "GetApplicationButton"
        Me.GetApplicationButton.Size = New System.Drawing.Size(97, 23)
        Me.GetApplicationButton.TabIndex = 1
        Me.GetApplicationButton.Text = "Get Applications"
        Me.GetApplicationButton.UseVisualStyleBackColor = True
        '
        'IntalledApplicationGridView
        '
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSlateGray
        Me.IntalledApplicationGridView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.IntalledApplicationGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.IntalledApplicationGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.IntalledApplicationGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.IntalledApplicationGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSlateGray
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.IntalledApplicationGridView.DefaultCellStyle = DataGridViewCellStyle3
        Me.IntalledApplicationGridView.EnterToTab = False
        Me.IntalledApplicationGridView.GridColor = System.Drawing.Color.Silver
        Me.IntalledApplicationGridView.Location = New System.Drawing.Point(8, 71)
        Me.IntalledApplicationGridView.Name = "IntalledApplicationGridView"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGray
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.IntalledApplicationGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.IntalledApplicationGridView.Size = New System.Drawing.Size(456, 309)
        Me.IntalledApplicationGridView.TabIndex = 0
        Me.IntalledApplicationGridView.TabToEnter = False
        '
        'AppConfiguration
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.Gainsboro
        Me.ClientSize = New System.Drawing.Size(478, 414)
        Me.Controls.Add(Me.TabControl1)
        Me.Name = "AppConfiguration"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Application Configuration"
        Me.TabControl1.ResumeLayout(False)
        Me.DataBaseAccess.ResumeLayout(False)
        Me.DataBaseAccess.PerformLayout()
        Me.InstalledApplicationsTAB.ResumeLayout(False)
        CType(Me.IntalledApplicationGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents DataBaseAccess As System.Windows.Forms.TabPage
    Friend WithEvents DataSourceTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents InstalledApplicationsTAB As System.Windows.Forms.TabPage
    Friend WithEvents PasswordTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents UserIDTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents DataBaseNameTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents ChangeAppConfigButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents BsLabel4 As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents DBUserIDLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents DataBaseNameLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents SqlServerLabel As Biosystems.Ax00.Controls.Usercontrols.BSLabel
    Friend WithEvents IntegratedSecurityCheckbox As Biosystems.Ax00.Controls.Usercontrols.BSCheckbox
    Friend WithEvents ConnectionStringTextBox As Biosystems.Ax00.Controls.Usercontrols.BSTextBox
    Friend WithEvents CloseButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents GetApplicationButton As Biosystems.Ax00.Controls.Usercontrols.BSButton
    Friend WithEvents IntalledApplicationGridView As Biosystems.Ax00.Controls.Usercontrols.BSDataGridView
End Class
