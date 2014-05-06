<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ApplicationLogView
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
        Me.ExitButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.ApplicationLogGridView = New DevExpress.XtraGrid.GridControl
        Me.GridView1 = New DevExpress.XtraGrid.Views.Grid.GridView
        Me.LogDateTime = New DevExpress.XtraGrid.Columns.GridColumn
        Me.Message = New DevExpress.XtraGrid.Columns.GridColumn
        Me.Modules = New DevExpress.XtraGrid.Columns.GridColumn
        Me.LogType = New DevExpress.XtraGrid.Columns.GridColumn
        Me.SamplesXtraGridView = New DevExpress.XtraGrid.Views.Grid.GridView
        Me.GridColumn2 = New DevExpress.XtraGrid.Columns.GridColumn
        Me.OpenFileDialogButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        Me.GenerateXmlButton = New Biosystems.Ax00.Controls.UserControls.BSButton
        CType(Me.ApplicationLogGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SamplesXtraGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ExitButton
        '
        Me.ExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ExitButton.Location = New System.Drawing.Point(918, 572)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(32, 32)
        Me.ExitButton.TabIndex = 0
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'ApplicationLogGridView
        '
        Me.ApplicationLogGridView.Location = New System.Drawing.Point(3, 3)
        Me.ApplicationLogGridView.LookAndFeel.UseWindowsXPTheme = True
        Me.ApplicationLogGridView.MainView = Me.GridView1
        Me.ApplicationLogGridView.Name = "ApplicationLogGridView"
        Me.ApplicationLogGridView.Size = New System.Drawing.Size(958, 492)
        Me.ApplicationLogGridView.TabIndex = 2
        Me.ApplicationLogGridView.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridView1, Me.SamplesXtraGridView})
        '
        'GridView1
        '
        Me.GridView1.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.GridView1.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.GridView1.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.GridView1.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.GridView1.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.GridView1.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.GridView1.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.GridView1.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.GridView1.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.GridView1.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.GridView1.Appearance.Empty.Options.UseBackColor = True
        Me.GridView1.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.GridView1.Appearance.EvenRow.Options.UseBackColor = True
        Me.GridView1.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.GridView1.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.GridView1.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.GridView1.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.GridView1.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.GridView1.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.GridView1.Appearance.FilterPanel.Options.UseBackColor = True
        Me.GridView1.Appearance.FilterPanel.Options.UseForeColor = True
        Me.GridView1.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent
        Me.GridView1.Appearance.FocusedCell.Options.UseBackColor = True
        Me.GridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.Black
        Me.GridView1.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.GridView1.Appearance.FocusedRow.Options.UseBackColor = True
        Me.GridView1.Appearance.FocusedRow.Options.UseForeColor = True
        Me.GridView1.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.FooterPanel.Options.UseBackColor = True
        Me.GridView1.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.GridView1.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.GridView1.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.GridView1.Appearance.GroupButton.Options.UseBackColor = True
        Me.GridView1.Appearance.GroupButton.Options.UseBorderColor = True
        Me.GridView1.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.GridView1.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.GridView1.Appearance.GroupFooter.Options.UseBackColor = True
        Me.GridView1.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.GridView1.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.GridView1.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.GridView1.Appearance.GroupPanel.Options.UseBackColor = True
        Me.GridView1.Appearance.GroupPanel.Options.UseForeColor = True
        Me.GridView1.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.GridView1.Appearance.GroupRow.Font = New System.Drawing.Font("Lucida Console", 9.0!)
        Me.GridView1.Appearance.GroupRow.Options.UseBackColor = True
        Me.GridView1.Appearance.GroupRow.Options.UseFont = True
        Me.GridView1.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.GridView1.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.GridView1.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.GridView1.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.GridView1.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.GridView1.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.GridView1.Appearance.HorzLine.Options.UseBackColor = True
        Me.GridView1.Appearance.OddRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.GridView1.Appearance.OddRow.Options.UseBackColor = True
        Me.GridView1.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.GridView1.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.GridView1.Appearance.Preview.Options.UseBackColor = True
        Me.GridView1.Appearance.Preview.Options.UseForeColor = True
        Me.GridView1.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.GridView1.Appearance.Row.Options.UseBackColor = True
        Me.GridView1.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.GridView1.Appearance.RowSeparator.Options.UseBackColor = True
        Me.GridView1.Appearance.SelectedRow.BackColor = System.Drawing.Color.DimGray
        Me.GridView1.Appearance.SelectedRow.Options.UseBackColor = True
        Me.GridView1.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.GridView1.Appearance.VertLine.Options.UseBackColor = True
        Me.GridView1.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {Me.LogDateTime, Me.Message, Me.Modules, Me.LogType})
        Me.GridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.GridView1.GridControl = Me.ApplicationLogGridView
        Me.GridView1.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.GridView1.Name = "GridView1"
        Me.GridView1.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.GridView1.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.GridView1.OptionsView.EnableAppearanceEvenRow = True
        Me.GridView1.OptionsView.EnableAppearanceOddRow = True
        Me.GridView1.SortInfo.AddRange(New DevExpress.XtraGrid.Columns.GridColumnSortInfo() {New DevExpress.XtraGrid.Columns.GridColumnSortInfo(Me.LogDateTime, DevExpress.Data.ColumnSortOrder.Ascending)})
        '
        'LogDateTime
        '
        Me.LogDateTime.Caption = "Date Time"
        Me.LogDateTime.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss:fff"
        Me.LogDateTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        Me.LogDateTime.FieldName = "LogDateTime"
        Me.LogDateTime.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText
        Me.LogDateTime.Name = "LogDateTime"
        Me.LogDateTime.OptionsColumn.AllowEdit = False
        Me.LogDateTime.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value
        Me.LogDateTime.UnboundType = DevExpress.Data.UnboundColumnType.DateTime
        Me.LogDateTime.Visible = True
        Me.LogDateTime.VisibleIndex = 0
        '
        'Message
        '
        Me.Message.Caption = "Message"
        Me.Message.FieldName = "Message"
        Me.Message.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText
        Me.Message.Name = "Message"
        Me.Message.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.[True]
        Me.Message.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.[True]
        Me.Message.Visible = True
        Me.Message.VisibleIndex = 1
        '
        'Modules
        '
        Me.Modules.Caption = "Module"
        Me.Modules.FieldName = "Module"
        Me.Modules.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText
        Me.Modules.Name = "Modules"
        Me.Modules.Visible = True
        Me.Modules.VisibleIndex = 2
        '
        'LogType
        '
        Me.LogType.Caption = "Type"
        Me.LogType.FieldName = "LogType"
        Me.LogType.Name = "LogType"
        Me.LogType.Visible = True
        Me.LogType.VisibleIndex = 3
        '
        'SamplesXtraGridView
        '
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.BorderColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.ForeColor = System.Drawing.Color.DimGray
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.ColumnFilterButton.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.BorderColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.ForeColor = System.Drawing.Color.Gainsboro
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.ColumnFilterButtonActive.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.Empty.BackColor = System.Drawing.Color.LightGray
        Me.SamplesXtraGridView.Appearance.Empty.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal
        Me.SamplesXtraGridView.Appearance.Empty.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.EvenRow.BackColor = System.Drawing.Color.White
        Me.SamplesXtraGridView.Appearance.EvenRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.Gray
        Me.SamplesXtraGridView.Appearance.FilterCloseButton.BorderColor = System.Drawing.Color.Gray
        Me.SamplesXtraGridView.Appearance.FilterCloseButton.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FilterCloseButton.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.FilterPanel.BackColor = System.Drawing.Color.Gray
        Me.SamplesXtraGridView.Appearance.FilterPanel.ForeColor = System.Drawing.Color.Black
        Me.SamplesXtraGridView.Appearance.FilterPanel.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FilterPanel.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent
        Me.SamplesXtraGridView.Appearance.FocusedCell.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FocusedRow.BackColor = System.Drawing.Color.Black
        Me.SamplesXtraGridView.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White
        Me.SamplesXtraGridView.Appearance.FocusedRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FocusedRow.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.FooterPanel.BackColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.FooterPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.FooterPanel.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.FooterPanel.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.GroupButton.BackColor = System.Drawing.Color.Silver
        Me.SamplesXtraGridView.Appearance.GroupButton.BorderColor = System.Drawing.Color.Silver
        Me.SamplesXtraGridView.Appearance.GroupButton.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.GroupButton.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.GroupFooter.BackColor = System.Drawing.Color.Silver
        Me.SamplesXtraGridView.Appearance.GroupFooter.BorderColor = System.Drawing.Color.Silver
        Me.SamplesXtraGridView.Appearance.GroupFooter.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.GroupFooter.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.GroupPanel.BackColor = System.Drawing.Color.DimGray
        Me.SamplesXtraGridView.Appearance.GroupPanel.ForeColor = System.Drawing.Color.White
        Me.SamplesXtraGridView.Appearance.GroupPanel.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.GroupPanel.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.GroupRow.BackColor = System.Drawing.Color.Transparent
        Me.SamplesXtraGridView.Appearance.GroupRow.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SamplesXtraGridView.Appearance.GroupRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.GroupRow.Options.UseFont = True
        Me.SamplesXtraGridView.Appearance.HeaderPanel.BackColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.DarkGray
        Me.SamplesXtraGridView.Appearance.HeaderPanel.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.HeaderPanel.Options.UseBorderColor = True
        Me.SamplesXtraGridView.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.LightSlateGray
        Me.SamplesXtraGridView.Appearance.HideSelectionRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.HorzLine.BackColor = System.Drawing.Color.LightGray
        Me.SamplesXtraGridView.Appearance.HorzLine.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.OddRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.SamplesXtraGridView.Appearance.OddRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.Preview.BackColor = System.Drawing.Color.Gainsboro
        Me.SamplesXtraGridView.Appearance.Preview.ForeColor = System.Drawing.Color.DimGray
        Me.SamplesXtraGridView.Appearance.Preview.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.Preview.Options.UseForeColor = True
        Me.SamplesXtraGridView.Appearance.Row.BackColor = System.Drawing.Color.White
        Me.SamplesXtraGridView.Appearance.Row.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.RowSeparator.BackColor = System.Drawing.Color.DimGray
        Me.SamplesXtraGridView.Appearance.RowSeparator.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.SelectedRow.BackColor = System.Drawing.Color.DimGray
        Me.SamplesXtraGridView.Appearance.SelectedRow.Options.UseBackColor = True
        Me.SamplesXtraGridView.Appearance.VertLine.BackColor = System.Drawing.Color.LightGray
        Me.SamplesXtraGridView.Appearance.VertLine.Options.UseBackColor = True
        Me.SamplesXtraGridView.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {Me.GridColumn2})
        Me.SamplesXtraGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.SamplesXtraGridView.GridControl = Me.ApplicationLogGridView
        Me.SamplesXtraGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always
        Me.SamplesXtraGridView.Name = "SamplesXtraGridView"
        Me.SamplesXtraGridView.OptionsCustomization.AllowFilter = False
        Me.SamplesXtraGridView.OptionsCustomization.AllowSort = False
        Me.SamplesXtraGridView.OptionsFind.AllowFindPanel = False
        Me.SamplesXtraGridView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.SamplesXtraGridView.OptionsSelection.EnableAppearanceFocusedRow = False
        Me.SamplesXtraGridView.OptionsView.EnableAppearanceEvenRow = True
        Me.SamplesXtraGridView.OptionsView.EnableAppearanceOddRow = True
        Me.SamplesXtraGridView.OptionsView.ShowGroupPanel = False
        Me.SamplesXtraGridView.PaintStyleName = "WindowsXP"
        '
        'GridColumn2
        '
        Me.GridColumn2.Caption = "GridColumn2"
        Me.GridColumn2.Name = "GridColumn2"
        Me.GridColumn2.Visible = True
        Me.GridColumn2.VisibleIndex = 0
        '
        'OpenFileDialogButton
        '
        Me.OpenFileDialogButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.OpenFileDialogButton.Location = New System.Drawing.Point(918, 511)
        Me.OpenFileDialogButton.Name = "OpenFileDialogButton"
        Me.OpenFileDialogButton.Size = New System.Drawing.Size(32, 32)
        Me.OpenFileDialogButton.TabIndex = 3
        Me.OpenFileDialogButton.UseVisualStyleBackColor = True
        '
        'GenerateXmlButton
        '
        Me.GenerateXmlButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.GenerateXmlButton.Location = New System.Drawing.Point(880, 511)
        Me.GenerateXmlButton.Name = "GenerateXmlButton"
        Me.GenerateXmlButton.Size = New System.Drawing.Size(32, 32)
        Me.GenerateXmlButton.TabIndex = 4
        Me.GenerateXmlButton.UseVisualStyleBackColor = True
        '
        'ApplicationLogView
        '
        Me.Appearance.BackColor = System.Drawing.Color.Gainsboro
        Me.Appearance.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal
        Me.Appearance.Options.UseBackColor = True
        Me.Appearance.Options.UseFont = True
        Me.ClientSize = New System.Drawing.Size(962, 616)
        Me.Controls.Add(Me.GenerateXmlButton)
        Me.Controls.Add(Me.OpenFileDialogButton)
        Me.Controls.Add(Me.ApplicationLogGridView)
        Me.Controls.Add(Me.ExitButton)
        Me.LookAndFeel.SkinName = "Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ApplicationLogView"
        Me.Text = "Application Log Viewer"
        CType(Me.ApplicationLogGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SamplesXtraGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ExitButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents ApplicationLogGridView As DevExpress.XtraGrid.GridControl
    Friend WithEvents SamplesXtraGridView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents OpenFileDialogButton As Biosystems.Ax00.Controls.UserControls.BSButton
    Friend WithEvents GridView1 As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents LogDateTime As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents GridColumn2 As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Message As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents Modules As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents LogType As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents GenerateXmlButton As Biosystems.Ax00.Controls.UserControls.BSButton

End Class
