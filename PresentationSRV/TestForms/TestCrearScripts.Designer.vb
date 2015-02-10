Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.PresentationCOM
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class TestCrearScripts
    Inherits BSBaseForm

    'Form overrides dispose to clean up the component list.
    <DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.BsNuevoAnalizadorButton = New BSButton
        Me.BsBorrarButton = New BSButton
        Me.BsVersionLabel = New BSLabel
        Me.BsVersionTextBox = New BSTextBox
        Me.BsExportarButton = New BSButton
        Me.BsImportarButton = New BSButton
        Me.BSScriptsDGV = New BSDataGridView
        Me.colAnalyzer = New DataGridViewTextBoxColumn
        Me.colScreen = New DataGridViewTextBoxColumn
        Me.colAction = New DataGridViewTextBoxColumn
        Me.colScript = New DataGridViewTextBoxColumn
        Me.colDescription = New DataGridViewTextBoxColumn
        Me.BsScreenErrorProvider = New BSErrorProvider
        Me.BsNuevoButton = New BSButton
        Me.BsAnalyzerLabel = New BSLabel
        Me.BsAnalyzerComboBox = New BSComboBox
        Me.EncriptadoCheckBox = New CheckBox
        Me.BsButton1 = New BSButton
        CType(Me.BSScriptsDGV, ISupportInitialize).BeginInit()
        CType(Me.BsScreenErrorProvider, ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BsNuevoAnalizadorButton
        '
        Me.BsNuevoAnalizadorButton.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
        Me.BsNuevoAnalizadorButton.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsNuevoAnalizadorButton.Location = New Point(477, 470)
        Me.BsNuevoAnalizadorButton.Name = "BsNuevoAnalizadorButton"
        Me.BsNuevoAnalizadorButton.Size = New Size(101, 32)
        Me.BsNuevoAnalizadorButton.TabIndex = 15
        Me.BsNuevoAnalizadorButton.Text = "Nuevo Analizador"
        Me.BsNuevoAnalizadorButton.UseVisualStyleBackColor = True
        '
        'BsBorrarButton
        '
        Me.BsBorrarButton.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
        Me.BsBorrarButton.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsBorrarButton.Location = New Point(671, 470)
        Me.BsBorrarButton.Name = "BsBorrarButton"
        Me.BsBorrarButton.Size = New Size(81, 32)
        Me.BsBorrarButton.TabIndex = 13
        Me.BsBorrarButton.Text = "Borrar Script"
        Me.BsBorrarButton.UseVisualStyleBackColor = True
        '
        'BsVersionLabel
        '
        Me.BsVersionLabel.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
        Me.BsVersionLabel.BackColor = Color.Transparent
        Me.BsVersionLabel.Font = New Font("Verdana", 8.25!)
        Me.BsVersionLabel.ForeColor = Color.Black
        Me.BsVersionLabel.Location = New Point(748, 473)
        Me.BsVersionLabel.Name = "BsVersionLabel"
        Me.BsVersionLabel.Size = New Size(89, 26)
        Me.BsVersionLabel.TabIndex = 12
        Me.BsVersionLabel.Text = "Version:"
        Me.BsVersionLabel.TextAlign = ContentAlignment.MiddleRight
        Me.BsVersionLabel.Title = False
        '
        'BsVersionTextBox
        '
        Me.BsVersionTextBox.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
        Me.BsVersionTextBox.BackColor = Color.White
        Me.BsVersionTextBox.DecimalsValues = False
        Me.BsVersionTextBox.Font = New Font("Verdana", 8.25!)
        Me.BsVersionTextBox.IsNumeric = False
        Me.BsVersionTextBox.Location = New Point(843, 477)
        Me.BsVersionTextBox.Mandatory = False
        Me.BsVersionTextBox.Name = "BsVersionTextBox"
        Me.BsVersionTextBox.Size = New Size(82, 21)
        Me.BsVersionTextBox.TabIndex = 11
        Me.BsVersionTextBox.Text = "1.0.0.0"
        Me.BsVersionTextBox.TextAlign = HorizontalAlignment.Right
        '
        'BsExportarButton
        '
        Me.BsExportarButton.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
        Me.BsExportarButton.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsExportarButton.Location = New Point(289, 470)
        Me.BsExportarButton.Name = "BsExportarButton"
        Me.BsExportarButton.Size = New Size(59, 32)
        Me.BsExportarButton.TabIndex = 10
        Me.BsExportarButton.Text = "Exportar"
        Me.BsExportarButton.UseVisualStyleBackColor = True
        '
        'BsImportarButton
        '
        Me.BsImportarButton.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
        Me.BsImportarButton.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsImportarButton.Location = New Point(227, 470)
        Me.BsImportarButton.Name = "BsImportarButton"
        Me.BsImportarButton.Size = New Size(56, 32)
        Me.BsImportarButton.TabIndex = 9
        Me.BsImportarButton.Text = "Importar"
        Me.BsImportarButton.UseVisualStyleBackColor = True
        '
        'BSScriptsDGV
        '
        Me.BSScriptsDGV.AllowUserToAddRows = False
        Me.BSScriptsDGV.AllowUserToDeleteRows = False
        Me.BSScriptsDGV.AllowUserToResizeRows = False
        Me.BSScriptsDGV.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
                    Or AnchorStyles.Left) _
                    Or AnchorStyles.Right), AnchorStyles)
        Me.BSScriptsDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Me.BSScriptsDGV.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.BSScriptsDGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.BSScriptsDGV.Columns.AddRange(New DataGridViewColumn() {Me.colAnalyzer, Me.colScreen, Me.colAction, Me.colScript, Me.colDescription})
        Me.BSScriptsDGV.EnterToTab = False
        Me.BSScriptsDGV.Location = New Point(16, 18)
        Me.BSScriptsDGV.MultiSelect = False
        Me.BSScriptsDGV.Name = "BSScriptsDGV"
        Me.BSScriptsDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.BSScriptsDGV.Size = New Size(916, 415)
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
        Me.BsNuevoButton.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
        Me.BsNuevoButton.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsNuevoButton.Location = New Point(584, 470)
        Me.BsNuevoButton.Name = "BsNuevoButton"
        Me.BsNuevoButton.Size = New Size(81, 32)
        Me.BsNuevoButton.TabIndex = 16
        Me.BsNuevoButton.Text = "Nuevo Script"
        Me.BsNuevoButton.UseVisualStyleBackColor = True
        '
        'BsAnalyzerLabel
        '
        Me.BsAnalyzerLabel.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
        Me.BsAnalyzerLabel.BackColor = Color.Transparent
        Me.BsAnalyzerLabel.Font = New Font("Verdana", 8.25!)
        Me.BsAnalyzerLabel.ForeColor = Color.Black
        Me.BsAnalyzerLabel.Location = New Point(13, 473)
        Me.BsAnalyzerLabel.Name = "BsAnalyzerLabel"
        Me.BsAnalyzerLabel.Size = New Size(65, 26)
        Me.BsAnalyzerLabel.TabIndex = 17
        Me.BsAnalyzerLabel.Text = "Analyzer:"
        Me.BsAnalyzerLabel.TextAlign = ContentAlignment.MiddleRight
        Me.BsAnalyzerLabel.Title = False
        '
        'BsAnalyzerComboBox
        '
        Me.BsAnalyzerComboBox.FormattingEnabled = True
        Me.BsAnalyzerComboBox.Location = New Point(83, 477)
        Me.BsAnalyzerComboBox.Name = "BsAnalyzerComboBox"
        Me.BsAnalyzerComboBox.Size = New Size(100, 21)
        Me.BsAnalyzerComboBox.TabIndex = 18
        '
        'EncriptadoCheckBox
        '
        Me.EncriptadoCheckBox.AutoSize = True
        Me.EncriptadoCheckBox.Location = New Point(227, 446)
        Me.EncriptadoCheckBox.Name = "EncriptadoCheckBox"
        Me.EncriptadoCheckBox.Size = New Size(86, 17)
        Me.EncriptadoCheckBox.TabIndex = 19
        Me.EncriptadoCheckBox.Text = "Encriptado"
        Me.EncriptadoCheckBox.UseVisualStyleBackColor = True
        '
        'BsButton1
        '
        Me.BsButton1.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
        Me.BsButton1.BackgroundImageLayout = ImageLayout.Stretch
        Me.BsButton1.Location = New Point(370, 470)
        Me.BsButton1.Name = "BsButton1"
        Me.BsButton1.Size = New Size(101, 32)
        Me.BsButton1.TabIndex = 20
        Me.BsButton1.Text = "Nuevo XML"
        Me.BsButton1.UseVisualStyleBackColor = True
        '
        'TestCrearScripts
        '
        Me.Appearance.BackColor = Color.Gainsboro
        Me.Appearance.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.ClientSize = New Size(948, 520)
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
        CType(Me.BSScriptsDGV, ISupportInitialize).EndInit()
        CType(Me.BsScreenErrorProvider, ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BsNuevoAnalizadorButton As BSButton
    Friend WithEvents BsBorrarButton As BSButton
    Friend WithEvents BsVersionLabel As BSLabel
    Friend WithEvents BsVersionTextBox As BSTextBox
    Friend WithEvents BsExportarButton As BSButton
    Friend WithEvents BsImportarButton As BSButton
    Friend WithEvents BSScriptsDGV As BSDataGridView
    Friend WithEvents BsScreenErrorProvider As BSErrorProvider
    Friend WithEvents BsNuevoButton As BSButton
    Friend WithEvents BsAnalyzerComboBox As BSComboBox
    Friend WithEvents BsAnalyzerLabel As BSLabel
    Friend WithEvents EncriptadoCheckBox As CheckBox
    Friend WithEvents colAnalyzer As DataGridViewTextBoxColumn
    Friend WithEvents colScreen As DataGridViewTextBoxColumn
    Friend WithEvents colAction As DataGridViewTextBoxColumn
    Friend WithEvents colScript As DataGridViewTextBoxColumn
    Friend WithEvents colDescription As DataGridViewTextBoxColumn
    Friend WithEvents BsButton1 As BSButton

End Class
