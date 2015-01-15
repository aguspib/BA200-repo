Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraEditors.Repository

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports System.Globalization


Public Class IHistoricalReports
    Inherits PesentationLayer.BSAdjustmentBaseForm

#Region "Declarations"
    'Screen's business delegate
    Private WithEvents myScreenDelegate As New HistoricalReportsDelegate()

    ' Language
    Private currentLanguage As String

    Private ChangedValue As Boolean

    Private myCultureInfo As CultureInfo
#End Region

#Region "Constructor"
    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DefineScreenLayout()
        MyClass.myCultureInfo = My.Computer.Info.InstalledUICulture
    End Sub
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Creates and initializes the GridControl1
    ''' </summary>
    ''' <remarks>Created by: RH and XBC 25/04/2012</remarks>
    Private Sub InitializeGridControl1()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            GridView1.Columns.Clear()

            Dim GridColumn1 As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim Task As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim Action As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim Data As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim Comments As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim DateTime As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim Recommendations As New DevExpress.XtraGrid.Columns.GridColumn()

            GridView1.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() _
                        {GridColumn1, DateTime, Task, Action, Data, Comments, Recommendations})


            Dim RepositoryItemCheckEdit1 As RepositoryItemCheckEdit = New RepositoryItemCheckEdit
            Dim RepositoryItemTextEdit1 As RepositoryItemTextEdit = New RepositoryItemTextEdit
            Dim RepositoryItemRichTextEdit2 As RepositoryItemRichTextEdit = New RepositoryItemRichTextEdit


            Me.GridControl1.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {RepositoryItemCheckEdit1, RepositoryItemTextEdit1, RepositoryItemRichTextEdit2})

            Me.GridControl1.Dock = System.Windows.Forms.DockStyle.Fill

            Me.GridView1.Appearance.ColumnFilterButton.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.ColumnFilterButton.Options.UseFont = True
            Me.GridView1.Appearance.ColumnFilterButtonActive.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.ColumnFilterButtonActive.Options.UseFont = True
            Me.GridView1.Appearance.CustomizationFormHint.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.CustomizationFormHint.Options.UseFont = True
            Me.GridView1.Appearance.DetailTip.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.DetailTip.Options.UseFont = True
            Me.GridView1.Appearance.Empty.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.Empty.Options.UseFont = True
            Me.GridView1.Appearance.EvenRow.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.EvenRow.Options.UseFont = True
            Me.GridView1.Appearance.FilterCloseButton.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.FilterCloseButton.Options.UseFont = True
            Me.GridView1.Appearance.FilterPanel.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.FilterPanel.Options.UseFont = True
            Me.GridView1.Appearance.FixedLine.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.FixedLine.Options.UseFont = True
            Me.GridView1.Appearance.FocusedCell.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.FocusedCell.Options.UseFont = True
            Me.GridView1.Appearance.FocusedRow.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.FocusedRow.Options.UseFont = True
            Me.GridView1.Appearance.FooterPanel.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.FooterPanel.Options.UseFont = True
            Me.GridView1.Appearance.GroupButton.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.GroupButton.Options.UseFont = True
            Me.GridView1.Appearance.GroupFooter.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.GroupFooter.Options.UseFont = True
            Me.GridView1.Appearance.GroupPanel.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.GroupPanel.Options.UseFont = True
            Me.GridView1.Appearance.GroupRow.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.GroupRow.Options.UseFont = True
            Me.GridView1.Appearance.HeaderPanel.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.HeaderPanel.Options.UseFont = True
            Me.GridView1.Appearance.HideSelectionRow.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.HideSelectionRow.Options.UseFont = True
            Me.GridView1.Appearance.HorzLine.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.HorzLine.Options.UseFont = True
            Me.GridView1.Appearance.OddRow.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.OddRow.Options.UseFont = True
            Me.GridView1.Appearance.Preview.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.Preview.Options.UseFont = True
            Me.GridView1.Appearance.Row.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.Row.Options.UseFont = True
            Me.GridView1.Appearance.RowSeparator.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.RowSeparator.Options.UseFont = True
            Me.GridView1.Appearance.SelectedRow.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.SelectedRow.Options.UseFont = True
            Me.GridView1.Appearance.TopNewRow.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.TopNewRow.Options.UseFont = True
            Me.GridView1.Appearance.VertLine.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.VertLine.Options.UseFont = True
            Me.GridView1.Appearance.ViewCaption.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.GridView1.Appearance.ViewCaption.Options.UseFont = True


            Me.GridView1.Appearance.GroupRow.Options.UseTextOptions = True
            Me.GridView1.Appearance.GroupRow.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Hide


            Me.GridView1.OptionsCustomization.AllowColumnMoving = False
            Me.GridView1.OptionsCustomization.AllowColumnResizing = False
            Me.GridView1.OptionsCustomization.AllowFilter = False
            Me.GridView1.OptionsCustomization.AllowGroup = False
            Me.GridView1.OptionsSelection.MultiSelect = True
            Me.GridView1.OptionsView.ShowGroupPanel = False
            Me.GridView1.RowHeight = 120

            GridView1.OptionsView.AllowCellMerge = False
            GridView1.OptionsView.GroupDrawMode = GroupDrawMode.Default
            GridView1.OptionsView.ShowGroupedColumns = False
            GridView1.OptionsView.ColumnAutoWidth = False
            GridView1.OptionsView.RowAutoHeight = False
            GridView1.OptionsView.ShowIndicator = True

            GridView1.ColumnPanelRowHeight = 30

            GridView1.Appearance.GroupRow.BackColor = Color.WhiteSmoke 'AverageBkColor
            GridView1.Appearance.GroupRow.ForeColor = Color.Black 'AverageForeColor
            GridView1.Appearance.FocusedCell.BackColor = Color.Transparent
            GridView1.Appearance.GroupButton.BackColor = Color.Transparent

            GridView1.OptionsHint.ShowColumnHeaderHints = False

            GridView1.OptionsMenu.EnableColumnMenu = False 'RH 22/05/2012

            'GridView1.OptionsBehavior.Editable = False
            'GridView1.OptionsBehavior.ReadOnly = True
            GridView1.OptionsCustomization.AllowSort = False

            GridView1.GroupCount = 1
            '
            'GridColumn1
            '
            'GridColumn1.AppearanceCell.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'GridColumn1.AppearanceCell.Options.UseFont = True
            'GridColumn1.AppearanceHeader.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'GridColumn1.AppearanceHeader.Options.UseFont = True
            GridColumn1.Caption = "ResultServiceID"
            GridColumn1.FieldName = "ResultServiceID"
            GridColumn1.Name = "GridColumn1"
            'GridColumn1.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
            GridColumn1.OptionsColumn.ReadOnly = True
            '
            'Task
            '
            'Task.AppearanceCell.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'Task.AppearanceCell.Options.UseFont = True
            Task.AppearanceCell.Options.UseTextOptions = True
            Task.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Task.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top
            Task.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
            'Task.AppearanceHeader.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'Task.AppearanceHeader.Options.UseFont = True
            Task.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Task", currentLanguage)
            Task.ColumnEdit = RepositoryItemRichTextEdit2
            Task.FieldName = "TaskDesc"
            Task.Name = "Task"
            Task.OptionsColumn.AllowEdit = False
            'Task.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
            Task.OptionsColumn.ReadOnly = True
            Task.OptionsFilter.AllowFilter = False
            Task.Visible = True
            Task.VisibleIndex = 1
            Task.Width = 60
            '
            'RepositoryItemRichTextEdit2
            '
            RepositoryItemRichTextEdit2.Name = "RepositoryItemRichTextEdit2"

            '
            'Action
            '
            'Action.AppearanceCell.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'Action.AppearanceCell.Options.UseFont = True
            Action.AppearanceCell.Options.UseTextOptions = True
            Action.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            Action.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top
            Action.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
            'Action.AppearanceHeader.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'Action.AppearanceHeader.Options.UseFont = True
            Action.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Action", currentLanguage)
            Action.ColumnEdit = RepositoryItemRichTextEdit2
            Action.FieldName = "ActionDesc"
            Action.Name = "Action"
            Action.OptionsColumn.AllowEdit = False
            'Action.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
            Action.OptionsColumn.ReadOnly = True
            Action.OptionsFilter.AllowFilter = False
            Action.Visible = True
            Action.VisibleIndex = 2
            '
            'Data
            '
            'Data.AppearanceCell.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'Data.AppearanceHeader.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Data.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", currentLanguage)
            Data.ColumnEdit = RepositoryItemRichTextEdit1
            Data.FieldName = "Data"
            Data.Name = "Data"
            'Data.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
            DateTime.OptionsColumn.AllowEdit = True
            Data.OptionsColumn.ReadOnly = True
            Data.OptionsFilter.AllowFilter = False
            Data.Visible = True
            Data.VisibleIndex = 3
            Data.Width = 500
            '
            'Comments
            '
            'Comments.AppearanceCell.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'Comments.AppearanceCell.Options.UseFont = True
            Comments.AppearanceCell.Options.UseTextOptions = True
            Comments.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top
            'Comments.AppearanceHeader.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'Comments.AppearanceHeader.Options.UseFont = True
            Comments.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", currentLanguage)
            Comments.ColumnEdit = RepositoryItemTextEdit1
            Comments.FieldName = "Comments"
            Comments.Name = "Comments"
            'Comments.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
            Comments.OptionsFilter.AllowFilter = False
            Comments.Visible = True
            Comments.VisibleIndex = 4
            Comments.Width = 102
            '
            'RepositoryItemTextEdit1
            '
            'RepositoryItemTextEdit1.Appearance.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'RepositoryItemTextEdit1.Appearance.Options.UseFont = True
            'RepositoryItemTextEdit1.AppearanceDisabled.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'RepositoryItemTextEdit1.AppearanceDisabled.Options.UseFont = True
            'RepositoryItemTextEdit1.AppearanceFocused.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'RepositoryItemTextEdit1.AppearanceFocused.Options.UseFont = True
            'RepositoryItemTextEdit1.AppearanceReadOnly.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'RepositoryItemTextEdit1.AppearanceReadOnly.Options.UseFont = True
            RepositoryItemTextEdit1.AutoHeight = False
            RepositoryItemTextEdit1.Name = "RepositoryItemTextEdit1"
            '
            'DateTime
            '
            'DateTime.AppearanceCell.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'DateTime.AppearanceCell.Options.UseFont = True
            DateTime.AppearanceCell.Options.UseTextOptions = True
            DateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            DateTime.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top
            DateTime.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
            'DateTime.AppearanceHeader.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'DateTime.AppearanceHeader.Options.UseFont = True
            DateTime.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", currentLanguage)
            DateTime.ColumnEdit = RepositoryItemRichTextEdit2
            DateTime.DisplayFormat.FormatString = MyClass.myCultureInfo.DateTimeFormat.FullDateTimePattern  ' "dd/MM/yyyy HH:mm:ss"
            DateTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
            DateTime.FieldName = "TS_DateTime"
            DateTime.Name = "DateTime"
            DateTime.OptionsColumn.AllowEdit = False
            'DateTime.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
            DateTime.OptionsColumn.ReadOnly = True
            DateTime.OptionsFilter.AllowFilter = False
            DateTime.Visible = True
            DateTime.VisibleIndex = 0
            DateTime.Width = 80
            '
            'Recommendations
            '
            'Recommendations.AppearanceCell.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'Recommendations.AppearanceCell.Options.UseFont = True
            Recommendations.AppearanceCell.Options.UseTextOptions = True
            Recommendations.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            'Recommendations.AppearanceHeader.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            'Recommendations.AppearanceHeader.Options.UseFont = True
            Recommendations.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Recommendations", currentLanguage)
            Recommendations.ColumnEdit = RepositoryItemRichTextEdit1
            Recommendations.FieldName = "Recommendations"
            Recommendations.Name = "Recommendations"
            'Recommendations.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.[False]
            Recommendations.OptionsColumn.ReadOnly = True
            Recommendations.OptionsFilter.AllowFilter = False
            Recommendations.Visible = True
            Recommendations.VisibleIndex = 5
            Recommendations.Width = 114

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & " InitializeSamplesXtraGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>Created by: XBC 19/07/2011</remarks>
    Private Sub PrepareButtons()
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath
        'Dim myGlobal As New GlobalDataTO
        'Dim myutil As New Utilities
        Try

            'DL 20/04/2012. Substitute icons and optimize
            auxIconName = GetIconName("FIND")
            If (auxIconName <> "") Then
                SearchButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'SAVE Button
            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then
                SaveButton.Image = Image.FromFile(iconPath & auxIconName)
            End If


            'CANCEL Button
            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then
                CancelButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'DELETE Button
            auxIconName = GetIconName("REMOVE")
            If (auxIconName <> "") Then
                DeleteButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                PrintButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                ExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If
            'DL 20/04/2012


            ''SEARCH Button
            ''DL 20/04/2012
            ''auxIconName = GetIconName("SEARCH")
            'auxIconName = GetIconName("FIND")
            ''DL 20/04/2012
            'Dim myAuxImage As Image
            'If File.Exists(iconPath & auxIconName) Then
            '    Dim myImage As Image
            '    myImage = Image.FromFile(iconPath & auxIconName)

            '    myGlobal = myutil.ResizeImage(myImage, New Size(24, 24))
            '    If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
            '        myAuxImage = CType(myGlobal.SetDatos, Bitmap)
            '    Else
            '        myAuxImage = CType(myImage, Bitmap)
            '    End If

            '    SearchButton.Image = myAuxImage
            '    'SearchButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''SAVE Button
            'auxIconName = GetIconName("SAVE")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    SaveButton.Image = Image.FromFile(iconPath & auxIconName)
            '    'SaveButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''CANCEL Button
            'auxIconName = GetIconName("UNDO") 'CANCEL
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    CancelButton.Image = Image.FromFile(iconPath & auxIconName)
            '    'CancelButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''DELETE Button
            'auxIconName = GetIconName("REMOVE")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    DeleteButton.Image = Image.FromFile(iconPath & auxIconName)
            '    'DeleteButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''PRINT Button
            'auxIconName = GetIconName("PRINT")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    PrintButton.Image = Image.FromFile(iconPath & auxIconName)
            '    'PrintButton.BackgroundImageLayout = ImageLayout.Center
            'End If

            ''EXIT Button
            'auxIconName = GetIconName("CANCEL")
            'If System.IO.File.Exists(iconPath & auxIconName) Then
            '    ExitButton.Image = Image.FromFile(iconPath & auxIconName)
            '    'ExitButton.BackgroundImageLayout = ImageLayout.Stretch
            'End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 29/07/2011
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            Me.BsTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_TITLE_HISTORICS", currentLanguage)
            Me.SearchGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_SEARCH_CONFIG", currentLanguage) & ":"
            Me.AnalyzerLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_Analyzer", currentLanguage) & ":"
            Me.DateFromLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_From", currentLanguage) & ":"
            Me.DateToLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_To", currentLanguage) & ":"
            Me.TaskLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Task", currentLanguage) & ":"
            Me.ActionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Action", currentLanguage) & ":"
            Me.ActivityGroupBox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Activity", currentLanguage)

            ' Tooltips
            GetScreenTooltip()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: XBC 29/07/11
    ''' </remarks>
    Private Sub GetScreenTooltip()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            ' For Tooltips...
            MyBase.bsScreenToolTips.SetToolTip(SaveButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save", currentLanguage))
            MyBase.bsScreenToolTips.SetToolTip(CancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", currentLanguage))
            MyBase.bsScreenToolTips.SetToolTip(DeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Delete", currentLanguage))
            MyBase.bsScreenToolTips.SetToolTip(PrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", currentLanguage))
            MyBase.bsScreenToolTips.SetToolTip(ExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", currentLanguage))

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".GetScreenTooltip ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenTooltip ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub DisableAll()
        Me.SearchButton.Enabled = False
        Me.SaveButton.Enabled = False
        Me.CancelButton.Enabled = False
        Me.DeleteButton.Enabled = False
        Me.PrintButton.Enabled = False
        Me.AnalyzerCombo.Enabled = False
        Me.DateFromDateTimePick.Enabled = False
        Me.DateToDateTimePick.Enabled = False
        Me.TasksCombo.Enabled = False
        Me.ActionsCombo.Enabled = False

        Me.ExitButton.Enabled = True    ' just exit screen option is available
    End Sub

    Private Sub Initializations()
        Try
            Me.SearchButton.Enabled = True
            Me.SaveButton.Enabled = False
            Me.CancelButton.Enabled = False
            Me.DeleteButton.Enabled = False
            Me.PrintButton.Enabled = False
            Me.ExitButton.Enabled = True
            Me.ChangedValue = False

            FillAnalyzerTypesCombo()
            FillTaskTypesCombo()
            FillActionTypesCombo(GlobalEnumerates.PreloadedMasterDataEnum.SRV_ACT_ADJ_ALL)

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Initializations ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Initializations ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the Analyzers ComboBox. with the Analyzers identifiers 
    ''' of the activities already registered.
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 03/08/2011
    ''' </remarks>
    Private Sub FillAnalyzerTypesCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myResultsDS As New SRVResultsServiceDS
            Dim myHistoricalReportsDelegate As New HistoricalReportsDelegate
            'Get the data of the Analyzers which have already registered activities
            myGlobalDataTO = myHistoricalReportsDelegate.GetAnalyzerResultsService(Nothing)

            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                myResultsDS = CType(myGlobalDataTO.SetDatos, SRVResultsServiceDS)

                Dim qAnalyzerIDs As New List(Of SRVResultsServiceDS.srv_thrsResultsServiceRow)

                qAnalyzerIDs = (From a In myResultsDS.srv_thrsResultsService _
                                Order By a.AnalyzerID _
                                Select a).ToList()

                If qAnalyzerIDs.Count > 0 Then
                    Me.AnalyzerCombo.DisplayMember = "AnalyzerID"
                    Me.AnalyzerCombo.ValueMember = "AnalyzerID"
                    Me.AnalyzerCombo.DataSource = qAnalyzerIDs
                    Me.AnalyzerCombo.SelectedIndex = 0
                Else
                    DisableAll()
                End If
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".FillAnalyzerTypesCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillAnalyzerTypesCombo ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the Task Types ComboBox. with the tasks 
    ''' defined on the preloaded master data.
    ''' SubTableID = SRV_TASK_TYPES
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 29/07/2011
    ''' </remarks>
    Private Sub FillTaskTypesCombo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            'Get the data corresponding to the Task types
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, _
                                    GlobalEnumerates.PreloadedMasterDataEnum.SRV_TASK_TYPES)

            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                Dim qTaskTypes As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)

                qTaskTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                              Order By a.Position _
                              Select a).ToList()

                If qTaskTypes.Count > 0 Then
                    Me.TasksCombo.DisplayMember = "FixedItemDesc"
                    Me.TasksCombo.ValueMember = "ItemID"
                    Me.TasksCombo.DataSource = qTaskTypes
                    Me.TasksCombo.SelectedIndex = 0
                End If
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".FillTaskTypesCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillTaskTypesCombo ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the Actions Types ComboBox. with the actions 
    ''' defined on the preloaded master data.
    ''' SubTableID = SRV_ACT_ADJ_TYPES
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 29/07/2011
    ''' </remarks>
    Private Sub FillActionTypesCombo(ByVal pType As GlobalEnumerates.PreloadedMasterDataEnum)
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myPreMasterDataDS As New PreloadedMasterDataDS
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate
            'Get the data corresponding to the Action types
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, pType)

            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                myPreMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                Dim qActionTypes As New List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)

                qActionTypes = (From a In myPreMasterDataDS.tfmwPreloadedMasterData _
                              Order By a.Position _
                              Select a).ToList()

                If qActionTypes.Count > 0 Then
                    Me.ActionsCombo.DisplayMember = "FixedItemDesc"
                    Me.ActionsCombo.ValueMember = "ItemID"
                    Me.ActionsCombo.DataSource = qActionTypes
                    Me.ActionsCombo.SelectedIndex = 0
                End If
            Else
                ShowMessage("Error", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".FillActionTypesCombo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillActionTypesCombo ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub FormatDataGrid()
        Try
            '' Grouping ...
            ''Dim view As GridView = GridControl1.FocusedView
            ''Me.GridView1.SortInfo.ClearAndAddRange(New GridColumnSortInfo() { _
            ''   New GridColumnSortInfo(view.Columns("TaskDesc"), DevExpress.Data.ColumnSortOrder.Ascending), _
            ''   New GridColumnSortInfo(view.Columns("ActionDesc"), DevExpress.Data.ColumnSortOrder.Descending)}, 2)

            'Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            'Me.GridView1.Columns.ColumnByFieldName("TaskDesc").Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Task", currentLanguage)
            'Me.GridView1.Columns.ColumnByFieldName("ActionDesc").Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Action", currentLanguage)
            'Me.GridView1.Columns.ColumnByFieldName("Data").Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_INFO_TITLE", currentLanguage)
            'Me.GridView1.Columns.ColumnByFieldName("Comments").Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", currentLanguage)
            ''Me.GridView1.Columns.ColumnByFieldName("AnalyzerID").Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SCRIPT_EDIT_Analyzer", currentLanguage)
            ''Me.GridView1.Columns.ColumnByFieldName("TS_User").Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_User", currentLanguage)
            'Me.GridView1.Columns.ColumnByFieldName("TS_DateTime").Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", currentLanguage)
            'Me.GridView1.Columns.ColumnByFieldName("Recommendations").Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_Recommendations", currentLanguage)

            ExpandAllRows()

            UnSelectAllRows()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".FormatDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FormatDataGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        End Try
    End Sub

    Private Sub ExpandAllRows()
        Me.GridView1.BeginUpdate()
        Try
            Dim dataRowCount As Integer = Me.GridView1.DataRowCount
            For i As Integer = 0 To dataRowCount - 1
                Me.GridView1.SetMasterRowExpanded(i, True)
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".ExpandAllRows ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExpandAllRows ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        Finally
            Me.GridView1.EndUpdate()
        End Try
    End Sub

    Private Sub UnSelectAllRows()
        Me.GridView1.BeginUpdate()
        Try
            For i As Integer = 0 To Me.GridView1.RowCount - 1
                Me.GridView1.UnselectRow(i)
            Next
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".UnSelectAllRows ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UnSelectAllRows ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        Finally
            Me.GridView1.EndUpdate()
        End Try
    End Sub

    ''' <summary>
    ''' Assigns the specific screen's elements to the common screen layout structure
    ''' </summary>
    ''' <remarks>XBC 04/08/2011</remarks>
    Private Sub DefineScreenLayout()
        Try
            With MyBase.myScreenLayout
                .MessagesPanel.Container = Me.BsMessagesPanel
                .MessagesPanel.Icon = Me.BsMessageImage
                .MessagesPanel.Label = Me.BsMessageLabel
            End With
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".DefineScreenLayout ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DefineScreenLayout ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub EnablingOptions()
        Try
            If GridView1.SelectedRowsCount > 0 Then
                Me.DeleteButton.Enabled = True
            Else
                Me.DeleteButton.Enabled = False
            End If

            If GridView1.RowCount > 0 Then
                Me.PrintButton.Enabled = True
            Else
                Me.SaveButton.Enabled = False
                Me.CancelButton.Enabled = False
                Me.PrintButton.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".EnablingOptions ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".EnablingOptions ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Function Validation() As Boolean
        Dim returnValue As Boolean = True
        Try
            If Me.AnalyzerCombo.SelectedValue Is Nothing Then
                returnValue = False
                Exit Try
            End If
            If Me.DateToDateTimePick.Value < Me.DateFromDateTimePick.Value Then
                returnValue = False
                Me.BsErrorProvider1.SetError(Me.DateToDateTimePick, GetMessageText(GlobalEnumerates.Messages.INVALIDDATE.ToString, currentLanguage))
                Exit Try
            End If

        Catch ex As Exception
            returnValue = False
            CreateLogActivity(ex.Message, Me.Name & ".Validation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Validation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
        Return returnValue
    End Function

    Private Sub ExitScreen()
        Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.No
        Try
            If Me.ChangedValue Then
                dialogResultToReturn = MyBase.ShowMessage(GetMessageText(Messages.SAVE_PENDING.ToString), Messages.SAVE_PENDING.ToString)

                If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                    MyClass.SaveButton.PerformClick()
                Else
                    MyClass.CancelButton.PerformClick()
                End If
            End If

            Me.Close()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ExitScreen ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Must Inherited"

    Public Overrides Sub PrepareErrorMode(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrepareErrorMode ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareErrorMode ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' It manages the Stop of the operation(s) that are currently being performed. 
    ''' </summary>
    ''' <param name="pAlarmType"></param>
    ''' <remarks>Created by SGM 19/10/2012</remarks>
    Public Overrides Sub StopCurrentOperation(Optional ByVal pAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.NONE)
        Try
            'TODO
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".StopCurrentOperation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".StopCurrentOperation ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region

#Region "Events"

    Private Sub IHistoricalReports_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myGlobal As New GlobalDataTO
        Dim myGlobalbase As New GlobalBase
        Try
            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString
            myScreenDelegate.currentLanguage = Me.currentLanguage

            'Get the current user level
            MyBase.GetUserNumericalLevel()

            'Get the current Language from the current Application Session
            currentLanguage = myGlobalbase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            InitializeGridControl1()

            GetScreenLabels()

            Initializations()

            Me.PrepareButtons()

            'Me.SetGridFont(GridView1, New Font("Consolas", 8))

            ResetBorderSRV()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".Load ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click
        Me.ExitScreen()
    End Sub

    Private Sub TasksCombo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TasksCombo.SelectedIndexChanged
        Try
            If Me.ActionsCombo IsNot Nothing Then
                If Me.ActionsCombo.Items.Count > 0 Then

                    Select Case Me.TasksCombo.SelectedValue.ToString
                        Case "ALL"
                            FillActionTypesCombo(GlobalEnumerates.PreloadedMasterDataEnum.SRV_ACT_ADJ_ALL)
                        Case "ADJUST"
                            FillActionTypesCombo(GlobalEnumerates.PreloadedMasterDataEnum.SRV_ACT_ADJ_TYPES)
                        Case "TEST"
                            FillActionTypesCombo(GlobalEnumerates.PreloadedMasterDataEnum.SRV_ACT_TEST_TYPES)
                        Case "UTIL"
                            FillActionTypesCombo(GlobalEnumerates.PreloadedMasterDataEnum.SRV_ACT_UTIL_TYPES)
                    End Select


                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".TasksCombo_SelectedIndexChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".TasksCombo_SelectedIndexChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub SearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchButton.Click
        Try
            Me.BsErrorProvider1.Clear()

            If Me.Validation Then
                Dim resultdata As New GlobalDataTO
                Dim TaskSelected As String = ""
                Dim ActionSelected As String = ""

                If Me.TasksCombo.SelectedValue.ToString = "ALL" Then
                    TaskSelected = ""
                Else
                    TaskSelected = Me.TasksCombo.SelectedValue.ToString
                End If
                If Me.ActionsCombo.SelectedValue.ToString = "ALL" Then
                    ActionSelected = ""
                Else
                    ActionSelected = Me.ActionsCombo.SelectedValue.ToString
                End If

                Me.Cursor = Cursors.WaitCursor
                resultdata = myScreenDelegate.GetAllResultsService(Nothing, CType(Me.AnalyzerCombo.SelectedValue, String), _
                                                                   TaskSelected, _
                                                                   ActionSelected, _
                                                                   Me.DateFromDateTimePick.Value, _
                                                                   Me.DateToDateTimePick.Value)

                If (Not resultdata.HasError And Not resultdata.SetDatos Is Nothing) Then
                    Dim myResultsDS As SRVResultsServiceDecodedDS
                    myResultsDS = DirectCast(resultdata.SetDatos, SRVResultsServiceDecodedDS)
                    myResultsDS.AcceptChanges()
                    GridControl1.DataSource = myResultsDS.srv_thrsResultsService
                    'For Each c As GridColumn In GridView1.Columns
                    '    c.AppearanceCell.Font = New Font(New FontFamily("Courier New"), 8.25, FontStyle.Regular, GraphicsUnit.Point)
                    'Next

                    Me.EnablingOptions()
                End If

                MyBase.DisplayMessage("")

                FormatDataGrid()

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".SearchButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SearchButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub SaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        Try
            Dim myResultsServiceDS As New SRVResultsServiceDS
            Dim myRowToUpdate As SRVResultsServiceDS.srv_thrsResultsServiceRow

            For i As Integer = 0 To GridView1.RowCount - 1
                If CType(GridView1.GetRowCellValue(i, "isModified"), Boolean) Then
                    myRowToUpdate = myResultsServiceDS.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                    myRowToUpdate.ResultServiceID = CType(GridView1.GetRowCellValue(i, "ResultServiceID"), Integer)
                    myRowToUpdate.Comments = CType(GridView1.GetRowCellValue(i, "Comments"), String)
                    myResultsServiceDS.srv_thrsResultsService.Rows.Add(myRowToUpdate)
                End If
            Next

            If myResultsServiceDS IsNot Nothing AndAlso myResultsServiceDS.srv_thrsResultsService.Rows.Count > 0 Then
                Dim resultdata As New GlobalDataTO

                resultdata = myScreenDelegate.UpdateComments(Nothing, myResultsServiceDS)
                If Not resultdata.HasError Then
                    MyBase.DisplayMessage(Messages.SRV_SAVE_OK.ToString)
                Else
                    MyBase.DisplayMessage(Messages.SRV_ERR_SAVE.ToString)
                End If

                Me.ChangedValue = False
                Me.SaveButton.Enabled = False
                Me.CancelButton.Enabled = False
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".SaveButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub DeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteButton.Click
        Try
            If GridView1.SelectedRowsCount > 0 Then

                Dim dialogResultToReturn As DialogResult = Windows.Forms.DialogResult.Yes
                dialogResultToReturn = ShowMessage(GetMessageText(GlobalEnumerates.Messages.SHOW_MESSAGE_TITLE_TEXT_SRV.ToString), Messages.SRV_DELETE_HISTORIC.ToString)
                If dialogResultToReturn = Windows.Forms.DialogResult.Yes Then
                    Dim myResultsServiceDS As New SRVResultsServiceDS
                    Dim myRowToUpdate As SRVResultsServiceDS.srv_thrsResultsServiceRow

                    For i As Integer = 0 To GridView1.RowCount - 1
                        If GridView1.IsRowSelected(i) Then
                            myRowToUpdate = myResultsServiceDS.srv_thrsResultsService.Newsrv_thrsResultsServiceRow
                            myRowToUpdate.ResultServiceID = CType(GridView1.GetRowCellValue(i, "ResultServiceID"), Integer)
                            myResultsServiceDS.srv_thrsResultsService.Rows.Add(myRowToUpdate)
                        End If
                    Next

                    If myResultsServiceDS IsNot Nothing AndAlso myResultsServiceDS.srv_thrsResultsService.Rows.Count > 0 Then
                        Dim resultdata As New GlobalDataTO

                        resultdata = myScreenDelegate.Delete(Nothing, myResultsServiceDS)
                        If Not resultdata.HasError Then
                            Me.SearchButton.PerformClick()
                        Else
                            MyBase.DisplayMessage(Messages.SYSTEM_ERROR.ToString)
                        End If

                    End If

                    Me.EnablingOptions()
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".DeleteButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".DeleteButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub GridControl1_EditorKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles GridControl1.EditorKeyDown
        Try
            If GridView1.FocusedColumn.Name Is "Comments" Then
                Dim oRow As DataRow
                oRow = GridView1.GetDataRow(GridView1.FocusedRowHandle)
                oRow.Item("isModified") = True

                Me.ChangedValue = True
                Me.SaveButton.Enabled = True
                Me.CancelButton.Enabled = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".GridControl1_EditorKeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GridControl1_EditorKeyDown ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub GridView1_SelectionChanged(ByVal sender As Object, ByVal e As DevExpress.Data.SelectionChangedEventArgs) Handles GridView1.SelectionChanged
        Try
            Me.EnablingOptions()

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".GridView1_SelectionChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GridView1_SelectionChanged ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelButton.Click
        Try
            Me.SearchButton.PerformClick()

            Me.ChangedValue = False
            Me.SaveButton.Enabled = False
            Me.CancelButton.Enabled = False

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".CancelButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CancelButton_Click ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    ''' <summary>
    ''' When the  ESC key is pressed, the screen is closed 
    ''' </summary>
    ''' <remarks>
    ''' Created by XBC 07/09/2011
    ''' </remarks>
    Private Sub ConfigUsers_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                Me.ExitButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub PrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintButton.Click
        Try
            If Not Me.GridControl1.IsPrintingAvailable Then
                CreateLogActivity("The 'DevExpress.XtraPrinting' library is not found", Me.Name & ".PrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            Else
                Me.GridControl1.ShowPrintPreview()
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".PrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

#End Region


#Region "TO DELETE"
    '#Region "Attibutes"
    '    'Private FreeCellIconNameAttr As String
    '    'Private AdjustmentTaskIconNameAttr As String = ""
    '    'Private TestTaskIconNameAttr As String = ""
    '    'Private UtilTaskIconNameAttr As String = ""
    '    'Private MonitorTaskIconNameAttr As String = ""
    '#End Region

    '#Region "Properties"

    '    'Public ReadOnly Property ShowTaskIcon(ByVal pType As Integer) As Bitmap
    '    '    Get
    '    '        Dim path As String = ""

    '    '        Select Case pType
    '    '            Case 0
    '    '                path = Me.FreeCellIconNameAttr
    '    '            Case 1
    '    '                path = Me.AdjustmentTaskIconNameAttr
    '    '            Case 2
    '    '                path = Me.TestTaskIconNameAttr
    '    '            Case 3
    '    '                path = Me.UtilTaskIconNameAttr
    '    '            Case 4
    '    '                path = Me.MonitorTaskIconNameAttr
    '    '        End Select

    '    '        Dim bm_source As New Bitmap(Image.FromFile(path))
    '    '        Dim scale_factor As Integer = 20
    '    '        ' Make a bitmap for the result.
    '    '        Dim bm_dest As New Bitmap(CInt(scale_factor), CInt(scale_factor))
    '    '        ' Make a Graphics object for the result Bitmap.
    '    '        Dim gr_dest As Graphics = Graphics.FromImage(bm_dest)
    '    '        ' Copy the source image into the destination bitmap.
    '    '        gr_dest.DrawImage(bm_source, 0, 0, bm_dest.Width, bm_dest.Height)

    '    '        Return bm_dest
    '    '    End Get
    '    'End Property

    '#End Region

    ' Private Sub PrepareButtons()
    '' Define Form icons
    'auxIconName = GetIconName("FREECELL")
    'If auxIconName <> "" Then
    '    Me.FreeCellIconNameAttr = iconPath & auxIconName
    'End If
    'auxIconName = GetIconName("VOLUME")
    'If auxIconName <> "" Then
    '    Me.AdjustmentTaskIconNameAttr = iconPath & auxIconName
    'End If
    'auxIconName = GetIconName("USERTEST")
    'If auxIconName <> "" Then
    '    Me.TestTaskIconNameAttr = iconPath & auxIconName
    'End If
    'auxIconName = GetIconName("UTILITIES")
    'If auxIconName <> "" Then
    '    Me.UtilTaskIconNameAttr = iconPath & auxIconName
    'End If
    'auxIconName = GetIconName("MONITORTOOL")
    'If auxIconName <> "" Then
    '    Me.MonitorTaskIconNameAttr = iconPath & auxIconName
    'End If
    ' End Sub

    'Private Sub SetGridFont(ByVal view As GridView, ByVal font As Font)
    '    Dim ap As AppearanceObject
    '    For Each ap In view.Appearance
    '        ap.Font = font
    '    Next
    'End Sub

    'Private Sub GridView1_ShownEditor(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GridView1.ShownEditor
    '    Try
    '        If GridView1.FocusedColumn.Name Is "Comments" Then

    '            For i As Integer = 0 To GridView1.RowCount - 1
    '                If GridView1.IsRowSelected(i) Then
    '                    Dim oRow As DataRow = GridView1.GetDataRow(i)
    '                    oRow.Item("isModified") = True
    '                End If
    '            Next

    '            Me.SaveButton.Enabled = True
    '            Me.CancelButton.Enabled = True
    '        End If

    '    Catch ex As Exception
    '        CreateLogActivity(ex.Message, Me.Name & ".GridView1_ShownEditor ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        ShowMessage(Me.Name & ".GridView1_ShownEditor ", Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
    '    End Try
    'End Sub

#End Region

End Class