'AG 19/10/2012 - copied and adapted from IHisResults

Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.FwScriptsManagement
Imports Biosystems.Ax00.BL
Imports System.Windows.Forms
Imports System.Drawing
Imports Biosystems.Ax00.CommunicationsSwFw
Imports System.IO
Imports Biosystems.Ax00.Controls.UserControls
Imports System.Globalization
Imports Biosystems.Ax00.InfoAnalyzer
Imports Biosystems.Ax00.PresentationCOM

Public Class IHisBlankCalibResults

#Region "Events definitions"

#End Region

#Region "Structures"
    Public Structure SearchFilter
        Public analyzerId As String
        Public dateFrom As Date
        Public dateTo As Date
        Public testNameContains As String
    End Structure
#End Region

#Region "Attributes"
    Private AnalyzerIDAttribute As String = String.Empty
#End Region

#Region "Declarations"
    Private myCultureInfo As CultureInfo

    ' Language
    Private currentLanguage As String

    'Multi language resources
    Private mImageDict As Dictionary(Of String, Image)
    Private mTextDict As Dictionary(Of String, String)
    Private mSampleTypes As Dictionary(Of String, String)   ' The Sample Types: {SampleType, Multilanguage Text}
    Private mImageGridDict As Dictionary(Of String, Byte())   ' The images inside the grid

    'SA 01/09/2014
    'BA-1910 ==> Changed from list of Strings to a typed DataSet AnalyzersDS to allow manage properties DisplayMember and ValueMember in the ComboBox
    Private mAnalyzers As New AnalyzersDS

    Private alarmsDefiniton As New AlarmsDS 'AG 22/10/2012
    Dim myHisWSResultsDelegate As New HisWSResultsDelegate 'AG 22/10/2012

#End Region

#Region "Properties"
    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        MyBase.New()
        InitializeComponent()

        myCultureInfo = My.Computer.Info.InstalledUICulture
    End Sub
#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"

#Region " Multilanguage Support "
    ''' <summary>
    ''' Gets the image by its key
    ''' Only reads from file once for key
    ''' </summary>
    ''' <param name="pKey"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 18/10/2012 (adapted to screen by AG 19/10/2012)
    ''' </remarks>
    Private Function GetImage(ByVal pKey As String) As Image
        If Not mImageDict.ContainsKey(pKey) Then
            SetImageToDictionary(pKey)
        End If
        If Not mImageDict.ContainsKey(pKey) Then
            Return New Bitmap(16, 16)
        End If

        Return mImageDict.Item(pKey)
    End Function


    Private Sub SetImageToDictionary(ByVal pKey As String)
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath

        auxIconName = GetIconName(pKey)
        If Not String.IsNullOrEmpty(auxIconName) Then
            If mImageDict.ContainsKey(pKey) Then
                mImageDict.Item(pKey) = Image.FromFile(iconPath & auxIconName)
            Else
                mImageDict.Add(pKey, Image.FromFile(iconPath & auxIconName))
            End If
        End If

    End Sub

    ''' <summary>
    ''' Gets the multilanguage text by its key
    ''' Only reads from DataBase once for key
    ''' </summary>
    ''' <param name="pKey"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 18/10/2012
    ''' </remarks>
    Private Function GetText(ByVal pKey As String) As String
        If Not mTextDict.ContainsKey(pKey) Then
            SetTextToDictionary(pKey)
        End If
        If Not mTextDict.ContainsKey(pKey) Then
            Return ""
        End If
        Return mTextDict.Item(pKey)
    End Function


    Private Sub SetTextToDictionary(ByVal pKey As String)
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
        Dim text As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, pKey, currentLanguage)
        If String.IsNullOrEmpty(text) Then text = "*" & pKey
        If mTextDict.ContainsKey(pKey) Then
            mTextDict.Item(pKey) = text
        Else
            mTextDict.Add(pKey, text)
        End If
    End Sub

    ''' <summary>
    ''' Gets the Grid image by its key
    ''' Only reads gets the image once for key
    ''' </summary>
    ''' <param name="pKey"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 19/10/2012 
    ''' </remarks>
    Private Function GetImageGrid(ByVal pKey As String) As Byte()
        If Not mImageGridDict.ContainsKey(pKey) Then
            SetImageGridToDictionary(pKey)
        End If
        If Not mImageGridDict.ContainsKey(pKey) Then
            Return Nothing
        End If

        Return mImageGridDict.Item(pKey)
    End Function
    Private Sub SetImageGridToDictionary(ByVal pKey As String)
        Dim preloadedDataConfig As New PreloadedMasterDataDelegate

        If mImageGridDict.ContainsKey(pKey) Then
            mImageGridDict.Item(pKey) = preloadedDataConfig.GetIconImage(pKey)
        Else
            mImageGridDict.Add(pKey, preloadedDataConfig.GetIconImage(pKey))
        End If
    End Sub
#End Region

#Region " Initializations "
    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012 (adapted to screen by AG 19/10/2012)
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            subtitleLabel.Text = GetText("LBL_HistResults")

            dateFromLabel.Text = GetText("LBL_Date_From") & ":"
            dateToLabel.Text = GetText("LBL_Date_To") & ":"
            analyzerIDLabel.Text = GetText("LBL_SCRIPT_EDIT_Analyzer") & ":"
            testNameLabel.Text = GetText("LBL_TestName") & ":"

            BsBlanksGridLabel1.Text = GetText("LBL_Blanks")
            BsCalibsGridLabel1.Text = GetText("LBL_Calibrators")

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012 (adapted to screen by AG 19/10/2012)
    ''' </remarks>
    Private Sub PrepareButtons()
        Dim myToolTipsControl As New ToolTip
        Try
            'EXIT Button
            exitButton.Image = GetImage("CANCEL")
            myToolTipsControl.SetToolTip(exitButton, GetText("BTN_CloseScreen"))

            'SEARCH Button
            searchButton.Image = GetImage("FIND")
            myToolTipsControl.SetToolTip(searchButton, GetText("BTN_Search"))

            'PRINT Button
            printButton.Image = GetImage("PRINT")
            myToolTipsControl.SetToolTip(printButton, GetText("BTN_Print"))

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load all the Analyzers
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 28/09/2012
    ''' Modified by: IR 04/10/2012 (adapted to screen AG 19/10/2012)
    '''              SA 01/09/2014 - BA-1910 ==> Call function GetDistinctAnalyzers in HisAnalyzerWorkSessionsDelegate instead of the function 
    '''                                          with the same name in AnalyzerDelegate class (which read Analyzers from table thisWSAnalyzerAlarms).
    '''                                          If the connected Analyzer is not in the list, it is added to the Analyzer ComboBox
    ''' </remarks>
    Private Sub GetAnalyzerList()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myHisAnalyzerWSDelegate As New HisAnalyzerWorkSessionsDelegate

            myGlobalDataTO = myHisAnalyzerWSDelegate.GetDistinctAnalyzers(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                mAnalyzers = DirectCast(myGlobalDataTO.SetDatos, AnalyzersDS)

                'Search if the connected Analyzer is in the returned list; add it to list in case it has not Historic Results yet
                If ((mAnalyzers.tcfgAnalyzers.ToList.Where(Function(a) a.AnalyzerID = AnalyzerIDAttribute)).Count = 0) Then
                    Dim myRow As AnalyzersDS.tcfgAnalyzersRow = mAnalyzers.tcfgAnalyzers.NewtcfgAnalyzersRow()
                    myRow.AnalyzerID = AnalyzerIDAttribute
                    mAnalyzers.tcfgAnalyzers.AddtcfgAnalyzersRow(myRow)
                    mAnalyzers.AcceptChanges()
                End If
            End If

            myGlobalDataTO = Nothing
            myHisAnalyzerWSDelegate = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetAnalyzerList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetAnalyzerList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    '''  Fills the DropDownLists
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012
    ''' Modified by: SA 01/08/2014 - Added Try/Catch block
    '''              SA 01/09/2014 - BA-1910 ==> Set field AnalyzerID as Display and Value Member for the Analyzers ComboBox
    ''' </remarks>
    Private Sub FillDropDownLists()
        Try
            'Get the list of Analyzers and load the ComboBox
            GetAnalyzerList()
            analyzerIDComboBox.DataSource = mAnalyzers.tcfgAnalyzers.DefaultView

            'BA-1910 ==> Set field AnalyzerID as Display and Value Member for the Analyzers ComboBox
            analyzerIDComboBox.DisplayMember = "AnalyzerID"
            analyzerIDComboBox.ValueMember = "AnalyzerID"

            'The label and the ComboBox are visible and enabled only when there are several Analyzers loaded in the ComboBox
            analyzerIDComboBox.Enabled = (mAnalyzers.tcfgAnalyzers.Rows.Count > 1)
            analyzerIDComboBox.Visible = analyzerIDComboBox.Enabled
            analyzerIDLabel.Visible = analyzerIDComboBox.Visible
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillDropDownLists ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillDropDownLists ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the embedded navigator in xtraHistoryGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012 (adapted to screen by AG 19/10/2012)
    ''' </remarks>
    Private Sub InitializeGridNavigator()
        Try
            With xtraBlanksGrid.EmbeddedNavigator.Buttons
                .First.Hint = GetText("BTN_GoToFirst")
                .PrevPage.Hint = GetText("BTN_GoToPrevious")
                .NextPage.Hint = GetText("BTN_GoToNext")
                .Last.Hint = GetText("BTN_GoToLast")
            End With

            With xtraCalibratorsGrid.EmbeddedNavigator.Buttons
                .First.Hint = GetText("BTN_GoToFirst")
                .PrevPage.Hint = GetText("BTN_GoToPrevious")
                .NextPage.Hint = GetText("BTN_GoToNext")
                .Last.Hint = GetText("BTN_GoToLast")
            End With

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeGridNavigator ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeGridNavigator ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Creates and initializes the historyGrid
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012 (adapted to screen by AG 19/10/2012)
    ''' </remarks>
    Private Sub InitializeResultHistoryGrids()
        Try
            'Grids initializations
            BlankGridView.Columns.Clear()
            CalibratorGridView.Columns.Clear()

            InitializeGridNavigator()

            InitializeBlankResultsGrid()
            InitializeCalibResultsGrid()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeResultHistoryGrids ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeResultHistoryGrids", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize blanks grid
    ''' </summary>
    ''' <remarks>AG 22/10/2012
    ''' AG 29/10/2012 (AG + EF meeting): Merge by TestName. Sort columns Date, Test, Remark Alert
    ''' </remarks>
    Private Sub InitializeBlankResultsGrid()
        Try
            'Prepare Blank Grid
            Dim DateColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim TestNameColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim RemarksAlertColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim AbsColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim AbsReagentColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim KineticBlankLimitColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim AbsInitialColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim AbsMainColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim BlankAbsLimitColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim GraphColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim RemarksColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim AnalyzerIDColumn As New DevExpress.XtraGrid.Columns.GridColumn()


            With BlankGridView
                .Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() _
                                  {DateColumn, TestNameColumn, RemarksAlertColumn, AbsColumn, _
                                   AbsReagentColumn, KineticBlankLimitColumn, AbsInitialColumn, AbsMainColumn, _
                                   BlankAbsLimitColumn, GraphColumn, RemarksColumn, AnalyzerIDColumn})

                .OptionsView.AllowCellMerge = True 'AG 29/10/2012
                .OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Default
                .OptionsView.ShowGroupedColumns = False
                .OptionsView.ColumnAutoWidth = False
                .OptionsView.RowAutoHeight = True
                .OptionsView.ShowIndicator = False

                .Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center
                '.Appearance.FocusedRow.ForeColor = Color.White
                '.Appearance.FocusedRow.BackColor = Color.LightSlateGray

                .OptionsHint.ShowColumnHeaderHints = False

                .OptionsBehavior.Editable = False
                .OptionsBehavior.ReadOnly = True
                .OptionsCustomization.AllowFilter = False
                .OptionsCustomization.AllowSort = True

                .OptionsSelection.EnableAppearanceFocusedRow = True
                .OptionsSelection.MultiSelect = False

                .GroupCount = 0
                .OptionsMenu.EnableColumnMenu = False
                .ColumnPanelRowHeight = 30
                .ColumnPanelRowHeight += CInt(.ColumnPanelRowHeight / 4) 'add additional height for Wrap header columns
            End With

            'Date Column
            With DateColumn
                .Caption = GetText("LBL_Date")
                .FieldName = "ResultDateTime"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                .DisplayFormat.FormatString = SystemInfoManager.OSDateFormat & " " & SystemInfoManager.OSShortTimeFormat
                .Name = "Date"
                .Visible = True
                .Width = 100
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'TestName column
            With TestNameColumn
                .Caption = GetText("LBL_Test_Singular")
                .FieldName = "TestName"
                .Name = "TestName"
                .Visible = True
                .Width = 100
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True 'AG 29/10/2012
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'RemarksAColumn column
            With RemarksAlertColumn
                .Caption = ""
                .FieldName = "RemarkAlert"
                .Name = "RemarkAlert"
                .Visible = True
                .Width = 10
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = False
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True 'AG 29/10/2012
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'Absorbance
            With AbsColumn
                .Caption = GetText("LBL_Absorbance_Short")
                .FieldName = "ABSValue"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .DisplayFormat.FormatString = GlobalConstants.ABSORBANCE_FORMAT.ToString
                .Name = "ABSValue"
                .Visible = True
                .Width = 75
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            End With

            'Abs WorkReagent
            With AbsReagentColumn
                .Caption = GetText("LBL_Results_WorkReagent")
                .FieldName = "ABSWorkReagent"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .DisplayFormat.FormatString = GlobalConstants.ABSORBANCE_FORMAT.ToString
                .Name = "ABSWorkReagent"
                .Visible = True
                .Width = 75
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far

                .AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap

            End With

            'Kinetic Blank Limit
            'JC 13/11/2012
            With KineticBlankLimitColumn
                .Caption = GetText("LBL_KineticBlankLimit")
                .FieldName = "KineticBlankLimit"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .DisplayFormat.FormatString = GlobalConstants.ABSORBANCE_FORMAT.ToString
                .Name = "KineticBlankLimit"
                .Visible = True
                .Width = 85
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far

                .AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
            End With

            'Abs Initial
            With AbsInitialColumn
                .Caption = GetText("LBL_AbsInitial")
                .FieldName = "ABSInitial"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .DisplayFormat.FormatString = GlobalConstants.ABSORBANCE_FORMAT.ToString
                .Name = "ABSInitial"
                .Visible = True
                .Width = 75
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far

                .AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
            End With

            'Abs Main Filter
            With AbsMainColumn
                .Caption = GetText("LBL_AbsMainFilter")
                .FieldName = "ABSMainFilter"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .DisplayFormat.FormatString = GlobalConstants.ABSORBANCE_FORMAT.ToString
                .Name = "ABSMainFilter"
                .Visible = True
                .Width = 75
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far

                .AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
            End With

            'Blank Absorbance Limit
            'JC 13/11/2012
            With BlankAbsLimitColumn
                .Caption = GetText("LBL_AbsLimit_Short")
                .FieldName = "BlankAbsorbanceLimit"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .DisplayFormat.FormatString = GlobalConstants.ABSORBANCE_FORMAT.ToString
                .Name = "BlankAbsorbanceLimit"
                .Visible = True
                .Width = 75
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far

                .AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
            End With

            'Graph column
            With GraphColumn
                .Caption = ""
                .FieldName = "GraphImage"
                .Name = "GraphImage"
                .Visible = True
                .Width = 16
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = False
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center

                'AG 23/10/2012 - Temporal v1 - Do not show the graph icon but in the calibrator curve results
                .Visible = False
            End With

            'Remarks column
            With RemarksColumn
                .Caption = GetText("LBL_Remarks")
                .FieldName = "Remarks"
                .Name = "Remarks"
                .Visible = True
                .Width = 250
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'AnalyzerColumn Column
            With AnalyzerIDColumn
                .Caption = GetText("LBL_SCRIPT_EDIT_Analyzer")
                .FieldName = "AnalyzerID"
                .Name = "AnalyzerID"
                .Visible = True
                .Width = 120
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False 'AG 29/10/2012
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'Additional Info
            'Not in v1

            'For showing an image on cells
            Dim pictureEdit As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
            pictureEdit = TryCast(xtraBlanksGrid.RepositoryItems.Add("PictureEdit"),  _
                                  DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit)

            pictureEdit.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip
            pictureEdit.NullText = " "

            GraphColumn.ColumnEdit = pictureEdit

            'For wrapping text
            Dim largeTextEdit As DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit
            largeTextEdit = TryCast(xtraBlanksGrid.RepositoryItems.Add("MemoEdit"),  _
                                    DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit)

            RemarksColumn.ColumnEdit = largeTextEdit
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeBlankResultsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeBlankResultsGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub


    ''' <summary>
    ''' Initialize calibrator grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 22/10/2012
    ''' Modified by: AG 29/10/2012 - Merge by TestName. Sort columns Date, Test, Type, CalibName, Remark Alert, Test, Type, Calib Name, Lot... (AG + EF meeting)
    '''              XB 25/09/2014 - BA-1863 ==> Added TestVersion column to allow filter by it when select the Calibrator chart result from Historics
    '''              SA 09/12/2014 - BA-1011 ==> Added new hidden column ManualResultFlag to allow change the font of ABSValue column to StrikeOut when 
    '''                                          ManualResultFlag is True (which mean the CalibratorFactor shown was entered manually)
    ''' </remarks>
    Private Sub InitializeCalibResultsGrid()
        Try
            'Prepare calibrator Grid
            Dim DateColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim CalibratorNameColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim LotNumberColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim NumberOfCalibratorsColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim TestNameColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim SampleTypeColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim RemarksAlertColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim AbsColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim TheoricalConcColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim UnitColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim CalibratorFactorColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim CalibratorLimitsColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim GraphColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim RemarksColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim AnalyzerIDColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim TestVersionColumn As New DevExpress.XtraGrid.Columns.GridColumn()       'BA-1863 - XB 25/09/2014
            Dim ManualResultFlagColumn As New DevExpress.XtraGrid.Columns.GridColumn()  'BA-1011

            With CalibratorGridView
                .Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() _
                                  {DateColumn, CalibratorNameColumn, LotNumberColumn, NumberOfCalibratorsColumn, _
                                   TestNameColumn, SampleTypeColumn, RemarksAlertColumn, AbsColumn, _
                                   TheoricalConcColumn, UnitColumn, CalibratorFactorColumn, CalibratorLimitsColumn, _
                                   GraphColumn, RemarksColumn, AnalyzerIDColumn, TestVersionColumn, ManualResultFlagColumn})

                .OptionsView.AllowCellMerge = True 'AG 29/10/2012
                .OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Default
                .OptionsView.ShowGroupedColumns = False
                .OptionsView.ColumnAutoWidth = False
                .OptionsView.RowAutoHeight = True
                .OptionsView.ShowIndicator = False

                .Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center
                '.Appearance.FocusedRow.ForeColor = Color.White
                '.Appearance.FocusedRow.BackColor = Color.LightSlateGray

                .OptionsHint.ShowColumnHeaderHints = False

                .OptionsBehavior.Editable = False
                .OptionsBehavior.ReadOnly = True
                .OptionsCustomization.AllowFilter = False
                .OptionsCustomization.AllowSort = True

                .OptionsSelection.EnableAppearanceFocusedRow = True
                .OptionsSelection.MultiSelect = False

                .GroupCount = 0
                .OptionsMenu.EnableColumnMenu = False
                .ColumnPanelRowHeight = 30
                .ColumnPanelRowHeight += CInt(.ColumnPanelRowHeight / 4) 'Add additional height for Wrap header columns
            End With

            'Date Column
            With DateColumn
                .Caption = GetText("LBL_Date")
                .FieldName = "ResultDateTime"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                .DisplayFormat.FormatString = SystemInfoManager.OSDateFormat & " " & SystemInfoManager.OSShortTimeFormat
                .Name = "Date"
                .Visible = True
                .Width = 100
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'TestName column
            With TestNameColumn
                .Caption = GetText("LBL_Test_Singular")
                .FieldName = "TestName"
                .Name = "TestName"
                .Visible = True
                .Width = 100
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True 'AG 29/10/2012
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'Sample Type column
            'JC 13/11/2012
            With SampleTypeColumn
                .Caption = GetText("LBL_Type")
                .FieldName = "SampleType"
                .Name = "SampleType"
                .Visible = True
                .Width = 60
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'Calibrator Name column
            'JC 13/11/2012
            With CalibratorNameColumn
                .Caption = GetText("LBL_CalibratorName")
                .FieldName = "CalibratorName"
                .Name = "CalibratorName"
                .Visible = True
                .Width = 100
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'Calibrator Lot Number column
            With LotNumberColumn
                .Caption = GetText("LBL_Lot")
                .FieldName = "LotNumber"
                .Name = "LotNumber"
                .Visible = True
                .Width = 50
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'Number of calibrators column
            With NumberOfCalibratorsColumn
                .Caption = GetText("LBL_Number_Short")
                .FieldName = "NumberOfCalibrators"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .Name = "NumberOfCalibrators"
                .Visible = True
                .Width = 45
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            End With

            'RemarksAColumn column
            With RemarksAlertColumn
                .Caption = ""
                .FieldName = "RemarkAlert"
                .Name = "RemarkAlert"
                .Visible = True
                .Width = 10
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = False
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True 'AG 29/10/2012
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'Absorbance
            With AbsColumn
                .Caption = GetText("LBL_Absorbance_Short")
                .FieldName = "ABSValue"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .DisplayFormat.FormatString = GlobalConstants.ABSORBANCE_FORMAT.ToString
                .Name = "ABSValue"
                .Visible = True
                .Width = 75
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            End With

            'Theorical Conc Column
            With TheoricalConcColumn
                .Caption = GetText("LBL_TheoricalConc_Short")
                .FieldName = "TheoreticalConcentration"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                '.DisplayFormat.FormatString = GlobalConstants.ABSORBANCE_FORMAT.ToString 'No!! value alreay saved with the proper decimals allowed
                .Name = "TheoreticalConcentration"
                .Visible = True
                .Width = 50
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far

                .AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
            End With

            'Unit Column

            With UnitColumn
                .Caption = GetText("LBL_Unit")
                .FieldName = "MeasureUnit"
                .Name = "MeasureUnit"
                .Visible = True
                .Width = 60
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'Calibrator Factor Column
            With CalibratorFactorColumn
                .Caption = GetText("LBL_CalibFactor")
                .FieldName = "CalibratorFactor"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .DisplayFormat.FormatString = GlobalConstants.CALIBRATOR_FACTOR_FORMAT.ToString
                .Name = "CalibratorFactor"
                .Visible = True
                .Width = 75
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            End With

            'Calibrator Limits Column
            With CalibratorLimitsColumn
                .Caption = GetText("LBL_FactorLimits")
                .FieldName = "FactorLimits"
                .Name = "FactorLimits"
                .Visible = True
                .Width = 120
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            End With

            'Graph column
            With GraphColumn
                .Caption = ""
                .FieldName = "GraphImage"
                .Name = "GraphImage"
                .Visible = True
                .Width = 16
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = False
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'Remarks column
            With RemarksColumn
                .Caption = GetText("LBL_Remarks")
                .FieldName = "Remarks"
                .Name = "Remarks"
                .Visible = True
                .Width = 250
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'AnalyzerColumn Column
            With AnalyzerIDColumn
                .Caption = GetText("LBL_SCRIPT_EDIT_Analyzer")
                .FieldName = "AnalyzerID"
                .Name = "AnalyzerID"
                .Visible = True
                .Width = 120
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False 'AG 29/10/2012
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'XB 25/09/2014 - BA-1863
            'Test Version column
            With TestVersionColumn
                .Caption = String.Empty
                .FieldName = "TestVersionNumber"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .Name = "TestVersionNumber"
                .Visible = False
                .Width = 0
            End With
            'XB 25/09/2014 - BA-1863

            'BA-1011: ManualResultFlag (hidden column)  
            With ManualResultFlagColumn
                .Caption = String.Empty
                .FieldName = "ManualResultFlag"
                .Name = "ManualResultFlag"
                .Visible = False
                .Width = 0
            End With

            'Additional Info
            'Not in v1

            'For showing an image on cells
            Dim pictureEdit As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
            pictureEdit = TryCast(xtraCalibratorsGrid.RepositoryItems.Add("PictureEdit"),  _
                                  DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit)

            pictureEdit.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip
            pictureEdit.NullText = " "

            GraphColumn.ColumnEdit = pictureEdit


            'For wrapping text
            Dim largeTextEdit As DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit
            largeTextEdit = TryCast(xtraCalibratorsGrid.RepositoryItems.Add("MemoEdit"),  _
                                    DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit)

            RemarksColumn.ColumnEdit = largeTextEdit

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeCalibResultsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeCalibResultsGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub


    ''' <summary>
    ''' Initializes the filter search
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012 (adapted to screen by AG 19/10/2012)
    ''' Modified by: SA 01/09/2014 - BA-1910 ==> When the ComboBox of Analyzers contains more than one element, select by default
    '''                                          the currently connected one  
    ''' </remarks>
    Private Sub InitializeFilterSearch()
        Try
            dateFromDateTimePick.Checked = True
            dateFromDateTimePick.Value = Today.AddDays(-1)

            dateToDateTimePick.Checked = True
            dateToDateTimePick.Value = Today

            If (analyzerIDComboBox.Items.Count > 0) Then
                'Get the ID of the Analyzer currently connected, and select it in the ComboBox
                analyzerIDComboBox.SelectedValue = AnalyzerIDAttribute
            Else
                'Select the unique element in the list
                analyzerIDComboBox.SelectedIndex = 0
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeFilterSearch ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeFilterSearch", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Initialize all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012 (adapted to screen by AG 19/10/2012)
    ''' Modified by: SA 01/09/2014 - BA-1910 ==> Changed from list of Strings to a typed DataSet AnalyzersDS to allow manage properties 
    '''                                          DisplayMember and ValueMember in the ComboBox
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            mImageDict = New Dictionary(Of String, Image)()
            mTextDict = New Dictionary(Of String, String)()
            mSampleTypes = New Dictionary(Of String, String)
            mImageGridDict = New Dictionary(Of String, Byte())()

            'BA-1910 - Changed from list of Strings to a typed DataSet AnalyzersDS 
            mAnalyzers = New AnalyzersDS

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString()

            GetScreenLabels()
            PrepareButtons()
            FillDropDownLists()

            'AG 22/10/2012 - Get the alarms descriptions
            Dim alarmsDlg As New AlarmsDelegate
            Dim resultData As New GlobalDataTO
            resultData = alarmsDlg.ReadAll(Nothing)
            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                alarmsDefiniton = CType(resultData.SetDatos, AlarmsDS)
            End If

            InitializeResultHistoryGrids()

            InitializeFilterSearch()
            FindHistoricalResults()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " Search "
    ''' <summary>
    ''' Fill a SearchFilter structure with filters selected in Search Area
    ''' </summary>
    ''' <returns>SearchFilter structure filled with filters selected in Search Area</returns>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012 (adapted to screen by AG 19/10/2012)
    ''' Modified by: SA 01/09/2014 - BA-1910 ==> To get the selected Analyzer, used SelectedValue instead of SelectedItem, due to the ComboBox is loaded
    '''                                          in a different way
    ''' </remarks>
    Private Function GetSearchFilter() As SearchFilter
        Dim filter As New SearchFilter
        With filter
            .analyzerId = analyzerIDComboBox.SelectedValue.ToString

            If dateFromDateTimePick.Checked Then
                .dateFrom = dateFromDateTimePick.Value
            Else
                .dateFrom = Nothing
            End If
            If dateToDateTimePick.Checked Then
                .dateTo = dateToDateTimePick.Value.AddDays(1)
            Else
                .dateTo = Nothing
            End If

            .testNameContains = testNameTextBox.Text
        End With

        Return filter
    End Function

    ''' <summary>
    ''' Find the Results by the selected filter in screen
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012 (adapted to screen by AG 19/10/2012)
    ''' </remarks>
    Private Sub FindHistoricalResults()
        Dim myGlobalDataTO As New GlobalDataTO

        xtraBlanksGrid.DataSource = Nothing
        xtraCalibratorsGrid.DataSource = Nothing
        Try
            UpdateFormBehavior(False) 'Disable controls
            If analyzerIDComboBox.Items.Count > 0 Then
                Dim filter As SearchFilter = GetSearchFilter()
                With filter
                    myGlobalDataTO = myHisWSResultsDelegate.GetHistoricalBlankCalibResultsByFilter(Nothing, .analyzerId, .dateFrom, .dateTo, .testNameContains)
                End With
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myHisWSResultsDS As HisWSResultsDS
                    myHisWSResultsDS = DirectCast(myGlobalDataTO.SetDatos, HisWSResultsDS)

                    PrepareAndSetDataToGrid(myHisWSResultsDS.vhisWSResults)
                ElseIf myGlobalDataTO.HasError Then
                    ShowMessage(Me.Name & ".FindHistoricalResults", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
                End If
            End If
            UpdateFormBehavior(True) 'Enable controls
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FindHistoricalResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FindHistoricalResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Format the numeric values to the proper decimals allowed
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <remarks>
    ''' Created by: AG 22/10/2012
    ''' </remarks>
    Private Function FormatRowNumeric(ByRef pRow As HisWSResultsDS.vhisWSResultsRow) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            With pRow
                'Nothing, for blanks and calibrators the decimals allowed can be set on the column grid definition
                'methods: InitializeBlankResultsGrid and InitializeCalibResultsGrid

            End With
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FormatRowNumeric ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FormatRowNumeric ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobalDataTO
    End Function


    ''' <summary>
    ''' Decodes the images of the row (graph icon)
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <remarks>
    ''' Created by: JB 19/10/2012 (adapted to screen by AG 22/10/2012)
    ''' </remarks>
    Private Sub DecodeRowImages(ByRef pRow As HisWSResultsDS.vhisWSResultsRow)
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            With pRow
                'Final version
                '.GraphImage = GetImageGrid("AVG_ABS_GRAPH")

                'AG 23/10/2012 - Temporal v1 - Do not show the graph icon but in the calibrator curve results
                'AG 24/10/2012 - The new condition take into account the case of the special test HbTotal calibration
                'If Not .IsNumberOfCalibratorsNull AndAlso .NumberOfCalibrators > 1 Then
                If Not .IsNumberOfCalibratorsNull AndAlso .NumberOfCalibrators > 1 AndAlso .IsCalibPointUsedNull Then
                    'Calib multi point using all points (NOT a special test calibration using only one point)
                    .GraphImage = GetImageGrid("AVG_ABS_GRAPH")
                End If

            End With
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DecodeRowImages ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DecodeRowImages ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Decodes the row texts (factor limits, remark alert, remarks)
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <remarks>
    ''' Created by: JB 19/10/2012 (adapted to screen by AG 22/10/2012)
    ''' </remarks>
    Private Function DecodeRowTexts(ByRef pRow As HisWSResultsDS.vhisWSResultsRow) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            With pRow
                'Number of calibrators (only informed when > 1)
                Dim isMultiPointCalFlag As Boolean = False
                If Not .IsNumberOfCalibratorsNull Then
                    'AG 24/10/2012 - The new condition take into account the case of the special test HbTotal calibration (2on if condition)
                    'If .NumberOfCalibrators = 1 Then
                    If .NumberOfCalibrators = 1 OrElse (.NumberOfCalibrators > 1 AndAlso Not .IsCalibPointUsedNull) Then
                        'no of calibrators = 1 orelse special test using a multi calibrator but only 1 point (HbTotal)
                        .SetNumberOfCalibratorsNull()

                        'Theoretical conc format is the decimals allowed
                        If Not .IsTheoreticalConcentrationNull AndAlso Not .IsDecimalsAllowedNull Then
                            .TheoreticalConcentration = CSng(.TheoreticalConcentration.ToStringWithDecimals(.DecimalsAllowed))
                        End If

                        'Factor Limits (calibrators)
                        If Not .IsFactorLowerLimitNull AndAlso Not .IsFactorUpperLimitNull Then
                            .FactorLimits = .FactorLowerLimit.ToString & " - " & .FactorUpperLimit.ToString
                        End If

                    Else
                        'For multi point calibrator there are some fields that has to be empty
                        'RemarkAlert, Remarks, Absorbance, Theor. Conc, Unit, Calibrator Factor, Calibrator Limits
                        isMultiPointCalFlag = True
                        .SetRemarkAlertNull()
                        .SetRemarksNull()
                        .SetABSValueNull()
                        .SetTheoreticalConcentrationNull()
                        .SetMeasureUnitNull()
                        .SetCalibratorFactorNull()
                        .SetFactorLimitsNull()
                    End If

                End If

                'Remarks 
                If Not .IsAlarmListNull AndAlso Not isMultiPointCalFlag Then
                    'Set the alarm description into .Remarks field
                    If alarmsDefiniton.tfmwAlarms.Rows.Count > 0 Then
                        myGlobalDataTO = myHisWSResultsDelegate.GetAvgAlarmsForScreen(Nothing, alarmsDefiniton, pRow)
                    Else 'We can decode alarm so leave the columns empty
                        .Remarks = ""
                        .RemarkAlert = ""
                    End If

                End If

            End With
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DecodeRowTexts ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DecodeRowTexts ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Prepare the DataSet with friendly data and sets to Grid
    ''' </summary>
    ''' <param name="pHisResultsDataTable"></param>
    ''' <remarks>
    ''' Created by: JB 19/10/2012 (adapted to screen by AG 19/10/2012)
    ''' </remarks>
    Private Sub PrepareAndSetDataToGrid(ByVal pHisResultsDataTable As HisWSResultsDS.vhisWSResultsDataTable)
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            Dim blankResultsDS As New HisWSResultsDS
            Dim calibResultsDS As New HisWSResultsDS

            Dim myRes As List(Of HisWSResultsDS.vhisWSResultsRow)
            'BLANKS RESULT GRID
            myRes = (From a As HisWSResultsDS.vhisWSResultsRow In pHisResultsDataTable _
                     Where a.SampleClass = "BLANK" Select a).ToList

            'Get and adapt the blank results
            For Each row As HisWSResultsDS.vhisWSResultsRow In myRes
                blankResultsDS.vhisWSResults.ImportRow(row)
            Next
            blankResultsDS.AcceptChanges()

            For Each row As HisWSResultsDS.vhisWSResultsRow In blankResultsDS.vhisWSResults
                FormatRowNumeric(row)
                DecodeRowImages(row)
                myGlobalDataTO = DecodeRowTexts(row)
                If myGlobalDataTO.HasError Then
                    Exit For
                End If
            Next

            If Not myGlobalDataTO.HasError Then
                blankResultsDS.AcceptChanges()
                xtraBlanksGrid.DataSource = blankResultsDS.vhisWSResults
            Else
                xtraBlanksGrid.DataSource = Nothing
                ShowMessage(Me.Name & ".PrepareAndSetDataToGrid", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If


            'CALIBRATORS RESULT GRID
            myRes = (From a As HisWSResultsDS.vhisWSResultsRow In pHisResultsDataTable _
                     Where a.SampleClass = "CALIB" Select a).ToList

            'Get and adapt the calibrator results
            'For multi point calibrator show only 1 row
            Dim histOT As Integer = 0
            For Each row As HisWSResultsDS.vhisWSResultsRow In myRes
                If Not row.IsHistOrderTestIDNull AndAlso histOT <> row.HistOrderTestID Then
                    histOT = row.HistOrderTestID
                    calibResultsDS.vhisWSResults.ImportRow(row)
                End If
            Next
            calibResultsDS.AcceptChanges()

            For Each row As HisWSResultsDS.vhisWSResultsRow In calibResultsDS.vhisWSResults
                FormatRowNumeric(row)
                DecodeRowImages(row)
                myGlobalDataTO = DecodeRowTexts(row)
                If myGlobalDataTO.HasError Then
                    Exit For
                End If
            Next

            If Not myGlobalDataTO.HasError Then
                calibResultsDS.AcceptChanges()
                xtraCalibratorsGrid.DataSource = calibResultsDS.vhisWSResults
            Else
                xtraCalibratorsGrid.DataSource = Nothing
                ShowMessage(Me.Name & ".PrepareAndSetDataToGrid", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareAndSetDataToGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareAndSetDataToGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Behavior"
    ''' <summary>
    ''' Updates the Form Behavior: enable buttons, show actions...
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012 (adapted to screen by AG 19/10/2012)
    ''' </remarks>
    Private Sub UpdateFormBehavior(ByVal pStatus As Boolean)
        Try
            searchGroup.Enabled = pStatus
            exitButton.Enabled = pStatus

            'historyDeleteButton.Enabled = historyGridView.SelectedRowsCount > 0
            'exportButton.Enabled = historyGridView.SelectedRowsCount > 0
            'searchGroup.Enabled = analyzerIDComboBox.Items.Count > 0
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateFormBehavior ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpdateFormBehavior ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


#End Region

#Region "Open Auxiliary Screens"

    ''' <summary>
    ''' Opens the auxliary screen (IResultsCalibCurve)
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <remarks>
    ''' Created by:  AG 23/10/2012
    ''' Modified by: XB 30/07/2014 - Remove call to function HisWSExecutionsDelegate.GetExecutionResultsForCalibCurve. Instead, add required rows to the local ExecutionsDS - BA-1863
    ''' </remarks>
    Private Sub OpenCalibCurveScreen(ByVal pRow As HisWSResultsDS.vhisWSResultsRow)
        Try
            If pRow.IsAnalyzerIDNull OrElse pRow.IsWorkSessionIDNull OrElse pRow.IsHistOrderTestIDNull _
             OrElse pRow.IsTestNameNull OrElse pRow.IsSampleTypeNull OrElse pRow.IsSampleClassNull _
             OrElse pRow.IsCalibratorNameNull OrElse pRow.IsLotNumberNull Then
                'Some required column is missing. The aux screen can not be opened
                'Do nothing

            Else 'All required columns are informed. The aux screen can be opened 

                Dim analyzerID As String = pRow.AnalyzerID
                Dim worksessionID As String = pRow.WorkSessionID
                Dim histOrderTestID As Integer = pRow.HistOrderTestID
                Dim testName As String = pRow.TestName
                Dim sampleType As String = pRow.SampleType
                Dim sampleClass As String = pRow.SampleClass
                Dim lotNumber As String = pRow.LotNumber
                Dim calibratorName As String = pRow.CalibratorName

                Dim resultData As New GlobalDataTO
                'Get Avg Results
                Dim dlgate As New HisWSResultsDelegate
                Dim avgResults As New ResultsDS
                resultData = dlgate.GetAvgResultsForCalibCurve(Nothing, histOrderTestID, analyzerID, worksessionID)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    avgResults = CType(resultData.SetDatos, ResultsDS)
                Else
                    ShowMessage(Me.Name & ".OpenCalibCurveScreen", resultData.ErrorCode, resultData.ErrorMessage, Me)
                End If

                'Get Replic Results
                Dim exeResults As New ExecutionsDS
                If Not resultData.HasError Then

                    ' XB 30/07/2014 - BA-1863
                    'Dim dlgate2 As New HisWSExecutionsDelegate
                    'resultData = dlgate2.GetExecutionResultsForCalibCurve(Nothing, histOrderTestID, analyzerID, worksessionID)

                    'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    '    exeResults = CType(resultData.SetDatos, ExecutionsDS)
                    'Else
                    '    ShowMessage(Me.Name & ".OpenCalibCurveScreen", resultData.ErrorCode, resultData.ErrorMessage, Me)
                    'End If

                    Dim myExecutionRow As ExecutionsDS.vwksWSExecutionsResultsRow
                    myExecutionRow = exeResults.vwksWSExecutionsResults.NewvwksWSExecutionsResultsRow()
                    myExecutionRow.OrderTestID = avgResults.vwksResults.First.OrderTestID
                    myExecutionRow.AnalyzerID = avgResults.vwksResults.First.AnalyzerID
                    myExecutionRow.WorkSessionID = avgResults.vwksResults.First.WorkSessionID
                    myExecutionRow.SampleClass = avgResults.vwksResults.First.SampleClass
                    myExecutionRow.MultiItemNumber = avgResults.vwksResults.First.MultiPointNumber
                    myExecutionRow.RerunNumber = 1
                    myExecutionRow.ReplicateNumber = 1
                    myExecutionRow.ExecutionType = "CALIB"
                    myExecutionRow.ABS_Value = avgResults.vwksResults.First.ABSValue
                    If (Not avgResults.vwksResults.First.IsABS_InitialNull) Then myExecutionRow.ABS_Initial = avgResults.vwksResults.First.ABS_Initial
                    If (Not avgResults.vwksResults.First.IsABS_MainFilterNull) Then myExecutionRow.ABS_MainFilter = avgResults.vwksResults.First.ABS_MainFilter
                    If (Not avgResults.vwksResults.First.IsAbs_WorkReagentNull) Then myExecutionRow.Abs_WorkReagent = avgResults.vwksResults.First.Abs_WorkReagent
                    myExecutionRow.CONC_Value = avgResults.vwksResults.First.TheoricalConcentration
                    myExecutionRow.ResultDate = avgResults.vwksResults.First.ResultDateTime
                    myExecutionRow.SampleType = avgResults.vwksResults.First.SampleType
                    myExecutionRow.TestName = avgResults.vwksResults.First.TestName
                    If (Not avgResults.vwksResults.First.IsKineticBlankLimitNull) Then myExecutionRow.KineticBlankLimit = avgResults.vwksResults.First.KineticBlankLimit
                    myExecutionRow.TestType = "STD"
                    myExecutionRow.AlarmList = avgResults.vwksResults.First.AlarmList
                    myExecutionRow.InUse = True
                    exeResults.vwksWSExecutionsResults.AddvwksWSExecutionsResultsRow(myExecutionRow)
                    ' XB 30/07/2014 - BA-1863
                End If

                'Open the calib curve screen
                'Inform the properties
                If Not resultData.HasError Then
                    Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                     (From row In exeResults.vwksWSExecutionsResults _
                      Where row.TestName = testName AndAlso row.SampleClass = sampleClass AndAlso row.SampleType = sampleType _
                      Select row).ToList()
                    If TestList.Count > 0 Then
                        'Get the analyzer mode
                        Dim myAnalyzers As New AnalyzersDelegate
                        Dim myModel As String = "A400" 'Default value
                        resultData = myAnalyzers.GetAnalyzerModel(Nothing, pRow.AnalyzerID)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim analyzerCfgDS As New AnalyzersDS
                            analyzerCfgDS = CType(resultData.SetDatos, AnalyzersDS)
                            If analyzerCfgDS.tcfgAnalyzers.Rows.Count > 0 AndAlso Not analyzerCfgDS.tcfgAnalyzers.First.IsAnalyzerModelNull Then
                                myModel = analyzerCfgDS.tcfgAnalyzers.First.AnalyzerModel
                            End If
                        End If

                        'Open the aux screen
                        Using myCurveForm As New IResultsCalibCurve
                            With myCurveForm
                                .ActiveAnalyzer = analyzerID
                                .ActiveWorkSession = worksessionID
                                .AnalyzerModel = myModel
                                .AverageResults = avgResults
                                .ExecutionResults = exeResults
                                .SelectedTestName = TestList(0).TestName
                                .SelectedSampleType = TestList(0).SampleType
                                .SelectedFullTestName = TestList(0).TestName
                                .SelectedOrderTestID = TestList(0).OrderTestID  ' XB 30/07/2014 - BA-1863 
                                .SelectedTestVersionNumber = pRow.TestVersionNumber  ' XB 25/09/2014 - BA-1863

                                'Add the analyzsis mode
                                Dim analysisMode As String = String.Empty
                                'JC 12/11/2012
                                'If avgResults.vwksResults.Rows.Count > 0 AndAlso Not avgResults.vwksResults(0).IsAnalysisModeNull Then
                                '    .SelectedFullTestName &= " (" & avgResults.vwksResults(0).AnalysisMode & ")"
                                'End If

                                .SelectedLot = lotNumber
                                .SelectedCalibrator = calibratorName

                                .AcceptedRerunNumber = (From row In avgResults.vwksResults _
                                                        Where row.OrderTestID = TestList(0).OrderTestID _
                                                        AndAlso row.AcceptedResultFlag = True _
                                                        Select row.RerunNumber).First
                                .HistoricalMode = True
                            End With
                            myCurveForm.ShowDialog()
                        End Using
                    End If
                End If

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OpenCalibCurveScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenCalibCurveScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub


    ''' <summary>
    ''' Opens the auxliary screen (IResultsAbsCurve)
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <remarks>AG 23/10/2012</remarks>
    Private Sub OpenAbsorbanceTimeCurveScreen(ByVal pRow As HisWSResultsDS.vhisWSResultsRow)
        Try
            'TODO
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OpenAbsorbanceTimeCurveScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".OpenAbsorbanceTimeCurveScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#End Region

#Region "Events"

#Region " Screen Events "
    Private Sub IHisAlarms_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                exitButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IHisAlarms_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IHisAlarms_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IHisAlarms_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If Me.DesignMode Then Exit Sub

            InitializeScreen()

            ResetBorder()
            Application.DoEvents()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IHisAlarms_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IHisAlarms_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IHisAlarms_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        Try

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IHisAlarms_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IHisAlarms_Shown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " Button Events "
    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitButton.Click
        Try
            If (Not Tag Is Nothing) Then
                'A PerformClick() method was executed
                Close()
            Else
                'Normal button click - Open the WS Monitor form and close this one
                IAx00MainMDI.OpenMonitorForm(Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsSearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles searchButton.Click
        FindHistoricalResults()
    End Sub

    Private Sub PrintBlanksAndCalibratorsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles printButton.Click
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Dim filter As SearchFilter = GetSearchFilter()
            With filter
                XRManager.ShowBlanksAndCalibratorsReport(.analyzerId, .dateFrom, .dateTo, .testNameContains) 'IT 06/10/2014 - BT #1883
            End With

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("Blanks and Calibrator Test Results Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IHisBlankCalibResults.PrintBlanksAndCalibratorsButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrintBlanksAndCalibratorsButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

#Region " DateTimePicker Events "
    Private Sub bsDateFromDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dateFromDateTimePick.ValueChanged
        If dateFromDateTimePick.Checked Then
            dateToDateTimePick.MinDate = dateFromDateTimePick.Value
        Else
            dateToDateTimePick.MinDate = Today.AddYears(-100)
        End If
    End Sub

    Private Sub bsDateToDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dateToDateTimePick.ValueChanged
        If dateToDateTimePick.Checked Then
            dateFromDateTimePick.MaxDate = dateToDateTimePick.Value
        Else
            dateFromDateTimePick.MaxDate = Today.AddYears(100)
        End If
    End Sub
#End Region

#Region " Grid Events "
    ''' <summary>
    ''' When in the grid row value of field ManualResultFlag is TRUE, all row cells excepting TestName, SampleType, CalibratorFactor and GraphImage) 
    ''' are  shown striked-out, which means that value shown in cell CalibratorFactor was informed manually by the final User
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 09/12/2014 - BA-1011
    ''' </remarks>
    Private Sub CalibratorGridView_RowCellStyle(sender As Object, e As DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs) Handles CalibratorGridView.RowCellStyle
        Try
            Dim View As DevExpress.XtraGrid.Views.Grid.GridView = TryCast(sender, DevExpress.XtraGrid.Views.Grid.GridView)

            If (Not IsDBNull(View.GetRowCellValue(e.RowHandle, View.Columns("ManualResultFlag")))) Then
                Dim manualResultFlag As Boolean = Convert.ToBoolean(View.GetRowCellValue(e.RowHandle, View.Columns("ManualResultFlag")))
                If (manualResultFlag) Then
                    If (e.Column.FieldName <> "TestName" AndAlso e.Column.FieldName <> "SampleType" AndAlso _
                        e.Column.FieldName <> "CalibratorFactor" AndAlso e.Column.FieldName <> "GraphImage") Then
                        Dim myFontName As String = xtraCalibratorsGrid.Font.Name
                        Dim myFontSize As Single = xtraCalibratorsGrid.Font.Size

                        e.Appearance.Font = New Font(myFontName, myFontSize, FontStyle.Strikeout)
                    End If
                End If
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message, Name & ".CalibratorGridView_RowCellStyle ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CalibratorGridView_RowCellStyle", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Processes the mouse click event over the cells (calibrator grid)
    ''' </summary>
    ''' <remarks>
    ''' Created by: AG 23/10/2012
    ''' </remarks>
    Private Sub CalibratorGridView_RowCellClick(ByVal sender As System.Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs) Handles CalibratorGridView.RowCellClick
        Try
            Cursor = Cursors.WaitCursor
            If sender Is Nothing Then Return
            Dim dgv As DevExpress.XtraGrid.Views.Grid.GridView = CType(sender, DevExpress.XtraGrid.Views.Grid.GridView)

            'This is how to get the DataRow behind the GridViewRow
            Dim CurrentRow As HisWSResultsDS.vhisWSResultsRow = CType(dgv.GetDataRow(e.RowHandle), HisWSResultsDS.vhisWSResultsRow)


            Select Case e.Column.Name
                Case "GraphImage"
                    'AG 24/10/2012 - The new condition take into account the case of the special test HbTotal calibration
                    'If Not CurrentRow.IsNumberOfCalibratorsNull AndAlso CurrentRow.NumberOfCalibrators > 1 Then
                    If Not CurrentRow.IsNumberOfCalibratorsNull AndAlso CurrentRow.NumberOfCalibrators > 1 AndAlso CurrentRow.IsCalibPointUsedNull Then
                        'Calib multi point using all points (NOT a special test calibration using only one point)

                        'Open the IResultCalibCurve auxiliary screen
                        OpenCalibCurveScreen(CurrentRow)
                    Else
                        'Open the IResultAbsCurve auxiliary screen
                        OpenAbsorbanceTimeCurveScreen(CurrentRow)
                    End If
            End Select


        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CalibratorGridView_RowCellClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: AG 23/10/2012
    ''' </remarks>
    Private Sub BlankGridView_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BlankGridView.MouseMove, CalibratorGridView.MouseMove
        Try
            If sender Is Nothing Then Return
            Dim dgv As DevExpress.XtraGrid.Views.Grid.GridView = CType(sender, DevExpress.XtraGrid.Views.Grid.GridView) 'dgv could be BlankGridView or CalibratorGridView

            With dgv.CalcHitInfo(New Point(e.X, e.Y))
                If .InColumnPanel Then 'Header
                    If .Column IsNot Nothing AndAlso .Column.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True Then
                        Me.Cursor = Cursors.Hand
                    Else
                        Me.Cursor = Cursors.Default
                    End If

                ElseIf .InRowCell Then  'Data Rows
                    Dim CurrentRow As HisWSResultsDS.vhisWSResultsRow = CType(dgv.GetDataRow(.RowHandle), HisWSResultsDS.vhisWSResultsRow)

                    If .Column IsNot Nothing AndAlso .Column.Name = "GraphImage" AndAlso Not CurrentRow.GraphImage Is Nothing Then
                        'Column with functionality
                        Me.Cursor = Cursors.Hand
                    Else
                        Me.Cursor = Cursors.Default

                        'It does not work, I have to use a tool tip controler - Not in v1
                        'If .Column.Name = "RemarkAlert" AndAlso Not CurrentRow.RemarkAlert Is Nothing Then
                        '    .Column.ToolTip = CurrentRow.Remarks
                        'End If
                    End If

                End If
            End With

        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".BlankGridView_MouseMove ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub
#End Region

#End Region

End Class