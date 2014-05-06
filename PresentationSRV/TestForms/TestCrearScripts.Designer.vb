<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TestCrearScripts
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
        Me.BsNuevoAnalizadorButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsBorrarButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsVersionLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsVersionTextBox = New Biosystems.Ax00.Controls.UserControls.BSTextBox
        Me.BsExportarButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsImportarButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BSScriptsDGV = New Biosystems.Ax00.Controls.UserControls.BSDataGridView
        Me.colAnalyzer = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.colScreen = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.colAction = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.colScript = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.colDescription = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.BsScreenErrorProvider = New Biosystems.Ax00.Controls.UserControls.BSErrorProvider
        Me.BsNuevoButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.BsAnalyzerLabel = New Biosystems.Ax00.Controls.UserControls.BSLabel
        Me.BsAnalyzerComboBox = New Biosystems.Ax00.Controls.UserControls.BSComboBox
        Me.EncriptadoCheckBox = New System.Windows.Forms.CheckBox
        Me.BsButton1 = New Biosystems.Ax00.Controls.UserControls.BSButton
        CType(Me.BSScriptsDGV, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BsScreenErrorProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsNuevoAnalizadorButton
        '
        Me.BsNuevoAnalizadorButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BsNuevoAnalizadorButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsNuevoAnalizadorButton.Location = New System.Drawing.Point(477, 470)
        Me.BsNuevoAnalizadorButton.Name = "BsNuevoAnalizadorButton"
        Me.BsNuevoAnalizadorButton.Size = New System.Drawing.Size(101, 32)
        Me.BsNuevoAnalizadorButton.TabIndex = 15
        Me.BsNuevoAnalizadorButton.Text = "Nuevo Analizador"
        Me.BsNuevoAnalizadorButton.UseVisualStyleBackColor = True
        '
        'BsBorrarButton
        '
        Me.BsBorrarButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BsBorrarButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsBorrarButton.Location = New System.Drawing.Point(671, 470)
        Me.BsBorrarButton.Name = "BsBorrarButton"
        Me.BsBorrarButton.Size = New System.Drawing.Size(81, 32)
        Me.BsBorrarButton.TabIndex = 13
        Me.BsBorrarButton.Text = "Borrar Script"
        Me.BsBorrarButton.UseVisualStyleBackColor = True
        '
        'BsVersionLabel
        '
        Me.BsVersionLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsVersionLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsVersionLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsVersionLabel.ForeColor = System.Drawing.Color.Black
        Me.BsVersionLabel.Location = New System.Drawing.Point(748, 473)
        Me.BsVersionLabel.Name = "BsVersionLabel"
        Me.BsVersionLabel.Size = New System.Drawing.Size(89, 26)
        Me.BsVersionLabel.TabIndex = 12
        Me.BsVersionLabel.Text = "Version:"
        Me.BsVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsVersionLabel.Title = False
        '
        'BsVersionTextBox
        '
        Me.BsVersionTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsVersionTextBox.BackColor = System.Drawing.Color.White
        Me.BsVersionTextBox.DecimalsValues = False
        Me.BsVersionTextBox.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsVersionTextBox.IsNumeric = False
        Me.BsVersionTextBox.Location = New System.Drawing.Point(843, 477)
        Me.BsVersionTextBox.Mandatory = False
        Me.BsVersionTextBox.Name = "BsVersionTextBox"
        Me.BsVersionTextBox.Size = New System.Drawing.Size(82, 21)
        Me.BsVersionTextBox.TabIndex = 11
        Me.BsVersionTextBox.Text = "1.0.0.0"
        Me.BsVersionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'BsExportarButton
        '
        Me.BsExportarButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BsExportarButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsExportarButton.Location = New System.Drawing.Point(289, 470)
        Me.BsExportarButton.Name = "BsExportarButton"
        Me.BsExportarButton.Size = New System.Drawing.Size(59, 32)
        Me.BsExportarButton.TabIndex = 10
        Me.BsExportarButton.Text = "Exportar"
        Me.BsExportarButton.UseVisualStyleBackColor = True
        '
        'BsImportarButton
        '
        Me.BsImportarButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BsImportarButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsImportarButton.Location = New System.Drawing.Point(227, 470)
        Me.BsImportarButton.Name = "BsImportarButton"
        Me.BsImportarButton.Size = New System.Drawing.Size(56, 32)
        Me.BsImportarButton.TabIndex = 9
        Me.BsImportarButton.Text = "Importar"
        Me.BsImportarButton.UseVisualStyleBackColor = True
        '
        'BSScriptsDGV
        '
        Me.BSScriptsDGV.AllowUserToAddRows = False
        Me.BSScriptsDGV.AllowUserToDeleteRows = False
        Me.BSScriptsDGV.AllowUserToResizeRows = False
        Me.BSScriptsDGV.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BSScriptsDGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.BSScriptsDGV.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.BSScriptsDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.BSScriptsDGV.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colAnalyzer, Me.colScreen, Me.colAction, Me.colScript, Me.colDescription})
        Me.BSScriptsDGV.EnterToTab = False
        Me.BSScriptsDGV.Location = New System.Drawing.Point(16, 18)
        Me.BSScriptsDGV.MultiSelect = False
        Me.BSScriptsDGV.Name = "BSScriptsDGV"
        Me.BSScriptsDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.BSScriptsDGV.Size = New System.Drawing.Size(916, 415)
        Me.BSScriptsDGV.TabIndex = 8
        Me.BSScriptsDGV.TabToEnter = False
        '
        'colAnalyzer
        '
        Me.colAnalyzer.HeaderText = "AnalyzerID"
        Me.colAnalyzer.Name = "colAnalyzer"
        Me.colAnalyzer.ReadOnly = True
        '
        'colScreen
        '
        Me.colScreen.HeaderText = "ScreenID"
        Me.colScreen.Name = "colScreen"
        '
        'colAction
        '
        Me.colAction.HeaderText = "ActionID"
        Me.colAction.Name = "colAction"
        '
        'colScript
        '
        Me.colScript.HeaderText = "ScriptID"
        Me.colScript.Name = "colScript"
        '
        'colDescription
        '
        Me.colDescription.HeaderText = "Description"
        Me.colDescription.Name = "colDescription"
        '
        'BsScreenErrorProvider
        '
        Me.BsScreenErrorProvider.ContainerControl = Me
        '
        'BsNuevoButton
        '
        Me.BsNuevoButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BsNuevoButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsNuevoButton.Location = New System.Drawing.Point(584, 470)
        Me.BsNuevoButton.Name = "BsNuevoButton"
        Me.BsNuevoButton.Size = New System.Drawing.Size(81, 32)
        Me.BsNuevoButton.TabIndex = 16
        Me.BsNuevoButton.Text = "Nuevo Script"
        Me.BsNuevoButton.UseVisualStyleBackColor = True
        '
        'BsAnalyzerLabel
        '
        Me.BsAnalyzerLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BsAnalyzerLabel.BackColor = System.Drawing.Color.Transparent
        Me.BsAnalyzerLabel.Font = New System.Drawing.Font("Verdana", 8.25!)
        Me.BsAnalyzerLabel.ForeColor = System.Drawing.Color.Black
        Me.BsAnalyzerLabel.Location = New System.Drawing.Point(13, 473)
        Me.BsAnalyzerLabel.Name = "BsAnalyzerLabel"
        Me.BsAnalyzerLabel.Size = New System.Drawing.Size(65, 26)
        Me.BsAnalyzerLabel.TabIndex = 17
        Me.BsAnalyzerLabel.Text = "Analyzer:"
        Me.BsAnalyzerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.BsAnalyzerLabel.Title = False
        '
        'BsAnalyzerComboBox
        '
        Me.BsAnalyzerComboBox.FormattingEnabled = True
        Me.BsAnalyzerComboBox.Location = New System.Drawing.Point(83, 477)
        Me.BsAnalyzerComboBox.Name = "BsAnalyzerComboBox"
        Me.BsAnalyzerComboBox.Size = New System.Drawing.Size(100, 21)
        Me.BsAnalyzerComboBox.TabIndex = 18
        '
        'EncriptadoCheckBox
        '
        Me.EncriptadoCheckBox.AutoSize = True
        Me.EncriptadoCheckBox.Location = New System.Drawing.Point(227, 446)
        Me.EncriptadoCheckBox.Name = "EncriptadoCheckBox"
        Me.EncriptadoCheckBox.Size = New System.Drawing.Size(86, 17)
        Me.EncriptadoCheckBox.TabIndex = 19
        Me.EncriptadoCheckBox.Text = "Encriptado"
        Me.EncriptadoCheckBox.UseVisualStyleBackColor = True
        '
        'BsButton1
        '
        Me.BsButton1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BsButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.BsButton1.Location = New System.Drawing.Point(370, 470)
        Me.BsButton1.Name = "BsButton1"
        Me.BsButton1.Size = New System.Drawing.Size(101, 32)
        Me.BsButton1.TabIndex = 20
        Me.BsButton1.Text = "Nuevo XML"
        Me.BsButton1.UseVisualStyleBackColor = True
        '
        'TestCrearScripts
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.ClientSize = New System.Drawing.Size(948, 520)
        Me.Controls.Add(Me.BsButton1)
        Me.Controls.Add(Me.EncriptadoCheckBox)
        Me.Controls.Add(Me.BsAnalyzerComboBox)
        Me.Controls.Add(Me.BsAnalyzerLabel)
        Me.Controls.Add(Me.BsNuevoButton)
        Me.Controls.Add(Me.BsNuevoAnalizadorButton)
        Me.Controls.Add(Me.BsBorrarButton)
        Me.Controls.Add(Me.BsVersionLabel)
        Me.Controls.Add(Me.BsVersionTextBox)
        Me.Controls.Add(Me.BsExportarButton)
        Me.Controls.Add(Me.BsImportarButton)
        Me.Controls.Add(Me.BSScriptsDGV)
        Me.LookAndFeel.SkinName = "Black"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "TestCrearScripts"
        Me.Text = "Crear Scripts"
        CType(Me.BSScriptsDGV, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BsScreenErrorProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BsNuevoAnalizadorButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsBorrarButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsVersionLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents BsVersionTextBox As Biosystems.Ax00.Controls.UserControls.BSTextBox
    Friend WithEvents BsExportarButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsImportarButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BSScriptsDGV As Biosystems.Ax00.Controls.UserControls.BSDataGridView
    Friend WithEvents BsScreenErrorProvider As Biosystems.Ax00.Controls.UserControls.BSErrorProvider
    Friend WithEvents BsNuevoButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents BsAnalyzerComboBox As Biosystems.Ax00.Controls.UserControls.BSComboBox
    Friend WithEvents BsAnalyzerLabel As Biosystems.Ax00.Controls.UserControls.BSLabel
    Friend WithEvents EncriptadoCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents colAnalyzer As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colScreen As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colAction As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colScript As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colDescription As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents BsButton1 As Biosystems.Ax00.Controls.UserControls.BSButton

End Class
