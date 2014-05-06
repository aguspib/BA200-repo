Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports System.Globalization
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports Biosystems.Ax00.PresentationCOM

Imports LIS.Biosystems.Ax00.LISCommunications

Public Class IHisResults

#Region "Events definitions"

#End Region

#Region " Structures "
    Public Structure SearchFilter
        Public analyzerId As String
        Public dateFrom As Date
        Public dateTo As Date
        Public samplePatientID As String
        Public sampleClasses As List(Of String)
        Public statFlag As TriState
        Public sampleTypes As List(Of String)
        Public testTypes As List(Of String)
        Public testStartName As String
    End Structure
#End Region

#Region "Attributes"
    Private AnalyzerIDAttribute As String = String.Empty
    Private WorkSessionIDAttribute As String = String.Empty
#End Region

#Region "Declarations"
    Private myCultureInfo As CultureInfo

    'Language
    Private currentLanguage As String

    'Multi language resources
    Private mImageDict As Dictionary(Of String, Image)        ' The Screen images
    Private mTextDict As Dictionary(Of String, String)        ' The Multilanguage texts
    Private mImageGridDict As Dictionary(Of String, Byte())   ' The images inside the grid

    Private mStatFlags As Dictionary(Of TriState, String)     ' The Stat Flags: {StatFlag, Multilanguage Text}
    Private mSampleTypes As Dictionary(Of String, String)     ' The Sample Types: {SampleType, Multilanguage Text}
    Private mTestTypes As Dictionary(Of String, String)       ' The Test Types: {TestType, Multilanguage Text}

    Private mAnalyzers As List(Of String)

    Private alarmsDefiniton As New AlarmsDS 'AG 22/10/2012

    Private myHisWSResultsDelegate As HisWSResultsDelegate

    Private DeletingHistOrderTests As Boolean = False 'SGM 02/07/2013

#End Region

#Region "Properties"
    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

    Public WriteOnly Property WorkSessionID() As String
        Set(ByVal value As String)
            WorkSessionIDAttribute = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New()
        MyBase.New()
        InitializeComponent()

        mImageDict = New Dictionary(Of String, Image)()
        mTextDict = New Dictionary(Of String, String)()
        mImageGridDict = New Dictionary(Of String, Byte())()

        mStatFlags = New Dictionary(Of TriState, String)
        mSampleTypes = New Dictionary(Of String, String)
        mTestTypes = New Dictionary(Of String, String)

        mAnalyzers = New List(Of String)

        myHisWSResultsDelegate = New HisWSResultsDelegate

        myCultureInfo = My.Computer.Info.InstalledUICulture

        'Get the current Language from the current Application Session
        Dim currentLanguageGlobal As New GlobalBase
        currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString()
    End Sub
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Refresh screen after reception of LIS notification (delivered, undelivered,unresponded)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 16/04/2013
    ''' </remarks>
    Public Overrides Sub RefreshScreen(ByVal pRefreshEventType As List(Of GlobalEnumerates.UI_RefreshEvents), _
                                       ByVal pRefreshDS As Biosystems.Ax00.Types.UIRefreshDS)
        Try
            If (isClosingFlag) Then Return
            FindHistoricalResults()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", ".RefreshScreen " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".RefreshScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Private Methods"

#Region "Multilanguage Support"
    ''' <summary>
    ''' Gets the image by its key. Only reads from file once for key
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012 
    ''' </remarks>
    Private Function GetImage(ByVal pKey As String) As Image
        If (Not mImageDict.ContainsKey(pKey)) Then SetImageToDictionary(pKey)
        If (Not mImageDict.ContainsKey(pKey)) Then Return New Bitmap(16, 16)
        Return mImageDict.Item(pKey)
    End Function

    Private Sub SetImageToDictionary(ByVal pKey As String)
        Dim auxIconName As String = ""
        Dim iconPath As String = MyBase.IconsPath

        auxIconName = GetIconName(pKey)
        If (Not String.IsNullOrEmpty(auxIconName)) Then
            If mImageDict.ContainsKey(pKey) Then
                mImageDict.Item(pKey) = Image.FromFile(iconPath & auxIconName)
            Else
                mImageDict.Add(pKey, Image.FromFile(iconPath & auxIconName))
            End If
        End If
    End Sub

    ''' <summary>
    ''' Gets the multilanguage text by its key. Only reads from DataBase once for key
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012 
    ''' </remarks>
    Private Function GetText(ByVal pKey As String) As String
        If (Not mTextDict.ContainsKey(pKey)) Then SetTextToDictionary(pKey)
        If (Not mTextDict.ContainsKey(pKey)) Then Return ""
        Return mTextDict.Item(pKey)
    End Function

    Private Sub SetTextToDictionary(ByVal pKey As String)
        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
        Dim text As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, pKey, currentLanguage)

        If (String.IsNullOrEmpty(text)) Then text = "*" & pKey
        If (mTextDict.ContainsKey(pKey)) Then
            mTextDict.Item(pKey) = text
        Else
            mTextDict.Add(pKey, text)
        End If
    End Sub

    ''' <summary>
    ''' Gets the Grid image by its key. Only reads gets the image once for key
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 19/10/2012 
    ''' </remarks>
    Private Function GetImageGrid(ByVal pKey As String) As Byte()
        If (Not mImageGridDict.ContainsKey(pKey)) Then SetImageGridToDictionary(pKey)
        If Not mImageGridDict.ContainsKey(pKey) Then Return Nothing
        Return mImageGridDict.Item(pKey)
    End Function

    Private Sub SetImageGridToDictionary(ByVal pKey As String)
        Dim preloadedDataConfig As New PreloadedMasterDataDelegate

        If (mImageGridDict.ContainsKey(pKey)) Then
            mImageGridDict.Item(pKey) = preloadedDataConfig.GetIconImage(pKey)
        Else
            mImageGridDict.Add(pKey, preloadedDataConfig.GetIconImage(pKey))
        End If
    End Sub
#End Region

#Region "Initializations"
    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            subtitleLabel.Text = GetText("LBL_HistResultsPatient")

            dateFromLabel.Text = GetText("LBL_Date_From") & ":"
            dateToLabel.Text = GetText("LBL_Date_To") & ":"

            sampleIdLabel.Text = GetText("LBL_PatientSample") & ":"
            analyzerIDLabel.Text = GetText("LBL_SCRIPT_EDIT_Analyzer") & ":"

            statFlagLabel.Text = GetText("LBL_Stat") & ":"
            sampleTypeLabel.Text = GetText("LBL_SampleType") & ":"

            testTypeLabel.Text = GetText("LBL_Profiles_TestType") & ":"
            testNameLabel.Text = GetText("LBL_TestName") & ":"
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012
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

            'DELETE Button
            historyDeleteButton.Image = GetImage("REMOVE")
            myToolTipsControl.SetToolTip(historyDeleteButton, GetText("BTN_Delete"))

            'EXPORT Button
            exportButton.Image = GetImage("MANUAL_EXP")
            myToolTipsControl.SetToolTip(exportButton, GetText("BTN_Results_ManualExport"))

            'PRINT Buttons
            PrintButton.Image = GetImage("PRINT")
            myToolTipsControl.SetToolTip(PrintButton, GetText("BTN_Print"))

            'Compact Print button
            CompactPrintButton.Image = GetImage("COMPACTPRINT")
            myToolTipsControl.SetToolTip(CompactPrintButton, GetText("PMD_CompactReport"))

        Catch ex As Exception
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load all the Analyzers
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' Modified by: IR 04/10/2012
    ''' </remarks>
    Private Sub GetAnalyzerList()
        Try
            'IR 04/10/2012 - Let the user select more than one analyzer if available. We must read data from thisWSAnalyzerAlarmsDAO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myAnalyzerDelegate As New AnalyzersDelegate

            myGlobalDataTO = myAnalyzerDelegate.GetDistinctAnalyzers(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myAnalyzerData As List(Of String) = DirectCast(myGlobalDataTO.SetDatos, List(Of String))

                For Each o As String In myAnalyzerData
                    mAnalyzers.Add(o.ToString)
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetAnalyzerList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetAnalyzerList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load all the Sample Class availables (with its multilanguage text)
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012
    ''' </remarks>
    Private Sub GetStatFlags()
        With mStatFlags
            .Clear()
            .Add(TriState.UseDefault, GetText("LBL_SRV_All"))
            .Add(TriState.True, GetText("LBL_Stat"))     'Stat
            .Add(TriState.False, GetText("LBL_Routine")) 'Routine
        End With
    End Sub

    ''' <summary>
    ''' Load all the Sample Type availables (with its multilanguage text)
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012
    ''' </remarks>
    Private Sub GetSampleTypes()
        Dim myGlobalDataTO As New GlobalDataTO()
        Dim myMasterDataDelegate As New MasterDataDelegate()
       
        myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, "SAMPLE_TYPES")
        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
            Dim myMasteDataDS As MasterDataDS = DirectCast(myGlobalDataTO.SetDatos, MasterDataDS)

            'Sort the returned data 
            Dim qSampleType As List(Of MasterDataDS.tcfgMasterDataRow) = (From a As MasterDataDS.tcfgMasterDataRow In myMasteDataDS.tcfgMasterData _
                                                                      Order By a.Position _
                                                                        Select a).ToList()
            mSampleTypes.Clear()
            For Each SampleTypeRow As MasterDataDS.tcfgMasterDataRow In qSampleType
                mSampleTypes.Add(SampleTypeRow.ItemID.ToString(), "-" & SampleTypeRow.FixedItemDesc)
            Next
            qSampleType = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Load all the Test Type availables (with its multilanguage text)
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012
    ''' </remarks>
    Private Sub GetTestTypes()
        With mTestTypes
            .Clear()
            .Add("STD", GetText("PMD_TEST_TYPES_STD"))
            .Add("CALC", GetText("PMD_TEST_TYPES_CALC"))
            .Add("ISE", GetText("PMD_TEST_TYPES_ISE"))
            .Add("OFFS", GetText("PMD_TEST_TYPES_OFFS"))
        End With
    End Sub

    ''' <summary>
    '''  Fills the DropDownLists
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012
    ''' Modified by: TR 08/05/2013 - Take values from DB
    '''          by: XB 06/06/2013 - Add kvp.Key for the sample type description into the corresponding combo
    ''' </remarks>
    Private Sub FillDropDownLists()
        GetStatFlags()
        GetSampleTypes()
        GetTestTypes()
        GetAnalyzerList()

        statFlagComboBox.DataSource = mStatFlags.Values.ToList

        With sampleTypeChkComboBox.Properties.Items
            For Each kvp As KeyValuePair(Of String, String) In mSampleTypes
                .Add(kvp.Key, kvp.Key + kvp.Value)
            Next
        End With

        With testTypeChkComboBox.Properties.Items
            For Each kvp As KeyValuePair(Of String, String) In mTestTypes
                .Add(kvp.Key, kvp.Value)
            Next
        End With

        analyzerIDComboBox.DataSource = mAnalyzers

        sampleTypeChkComboBox.Properties.SelectAllItemCaption = GetText("LBL_SRV_All")
        testTypeChkComboBox.Properties.SelectAllItemCaption = GetText("LBL_SRV_All")

        analyzerIDComboBox.Enabled = mAnalyzers.Count > 1
        analyzerIDComboBox.Visible = analyzerIDComboBox.Enabled
        analyzerIDLabel.Visible = analyzerIDComboBox.Visible
    End Sub

    ''' <summary>
    ''' Initializes the embedded navigator in xtraHistoryGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Private Sub InitializeGridNavigator()
        Try
            With historyGrid.EmbeddedNavigator.Buttons
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

    Private Sub DrawCheckBox(ByVal g As Graphics, ByVal r As Rectangle, ByVal Checked As Boolean)
        Dim info As DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo
        Dim edit As RepositoryItemCheckEdit = TryCast(historyGrid.RepositoryItems.Add("CheckEdit"), RepositoryItemCheckEdit)
        info = TryCast(edit.CreateViewInfo(), DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)
        info.EditValue = Checked
        info.Bounds = r
        info.CalcViewInfo(g)

        Dim painter As DevExpress.XtraEditors.Drawing.CheckEditPainter
        painter = TryCast(edit.CreatePainter(), DevExpress.XtraEditors.Drawing.CheckEditPainter)

        Dim args As DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs
        args = New DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, New DevExpress.Utils.Drawing.GraphicsCache(g), r)
        painter.Draw(args)
        args.Cache.Dispose()
    End Sub

    ''' <summary>
    ''' Creates and initializes the historyGrid
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012
    ''' Modified by AG 29/10/2012 - (AG + EF MEETING) Merge only by patientID, sort grid columns: Date, Patient, Type, TestName, RemarkAlert
    '''             XB 03/10/2013 - BT #1309 ==> Added TestPosition field to preserve the sorting configured on Reports Tests Sorting screen 
    '''             AG 14/02/2014 - BT #1505 ==> User can sort also by column ExportStatus
    '''             SA 29/04/2014 - BT #1608 ==> Added a hidden column for field TestLongName (this field has to be shown as Test Name in reports
    '''                                          when it is informed)
    ''' </remarks>
    Private Sub InitializeResultHistoryGrid()
        Try
            historyGridView.Columns.Clear()

            InitializeGridNavigator()

            Dim column As DevExpress.XtraGrid.Columns.GridColumn

            With historyGridView
                .OptionsView.AllowCellMerge = True
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
                .OptionsSelection.MultiSelect = True

                .ColumnPanelRowHeight = 30
                .GroupCount = 0
                .OptionsMenu.EnableColumnMenu = False
            End With

            'For showing an image on cells
            Dim pictureEdit As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
            pictureEdit = TryCast(historyGrid.RepositoryItems.Add("PictureEdit"),  _
                                  DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit)

            pictureEdit.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip
            pictureEdit.NullText = " "

            'For wrapping text
            Dim largeTextEdit As DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit
            largeTextEdit = TryCast(historyGrid.RepositoryItems.Add("MemoEdit"),  _
                                    DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit)


            'Select Column
            column = historyGridView.Columns.Add()
            With column
                .Caption = ""
                .FieldName = "Selected"
                .Name = "Selected"
                .Visible = True
                .Width = 16
                .OptionsColumn.AllowSize = False
                .OptionsColumn.ShowCaption = False
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'Date Column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_Date")
                .FieldName = "ResultDateTime"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                .DisplayFormat.FormatString = SystemInfoManager.OSDateFormat & " " & SystemInfoManager.OSShortTimeFormat
                .Name = "ResultDateTime"
                .Visible = True
                .Width = 100
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'SamplePatient Column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_PatientSample")
                .FieldName = "PatientID"
                .Name = "PatientID"
                .Visible = True
                .Width = 95
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near 'AG 26/10/2012
            End With

            'StatFlagImage column
            column = historyGridView.Columns.Add()
            With column
                .Caption = ""
                .FieldName = "StatFlagImage"
                .FieldNameSortGroup = "StatFlag"
                .Name = "StatFlagImage"
                .Visible = True
                .Width = 16
                .OptionsColumn.AllowSize = False
                .OptionsColumn.ShowCaption = False
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False 'AG 29/10/2012 (sort only by date, patientID, type, testname and remarkalert)
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
                .ColumnEdit = pictureEdit
            End With

            'SampleType column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_Type")
                .FieldName = "SampleType"
                .Name = "SampleType"
                .Visible = True
                .Width = 35
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False 'AG 29/10/2012 - merge only by patientID
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'TestName column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_Test_Singular")
                .FieldName = "TestName"
                .Name = "TestName"
                .Visible = True
                .Width = 100
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False 'AG 29/10/2012 - merge only by patientID
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'BT #1608 - TestLongName column (hidden)
            column = historyGridView.Columns.Add()
            With column
                .FieldName = "TestLongName"
                .Name = "TestLongName"
                .Visible = False
                .Width = 0
            End With

            'RemarkAlert column
            column = historyGridView.Columns.Add()
            With column
                .Caption = ""
                .FieldName = "RemarkAlert"
                .Name = "RemarkAlert"
                .Visible = True
                .Width = 10
                .OptionsColumn.AllowSize = False
                .OptionsColumn.ShowCaption = False
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'Conc column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_CurveRes_Conc_Short")
                .FieldName = "CONCValueString"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .Name = "CONCValueString"
                .Visible = True
                .Width = 42
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            End With

            'Unit column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_Unit")
                .FieldName = "MeasureUnit"
                .Name = "MeasureUnit"
                .Visible = True
                .Width = 42
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'RefRanges column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_ReferenceRanges_Short")
                .FieldName = "RefRange"
                .Name = "RefRange"
                .Visible = True
                .Width = 75
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'Lims column
            column = historyGridView.Columns.Add()
            With column
                .Caption = ""
                .FieldName = "ExportImage"
                .Name = "ExportImage"
                .FieldNameSortGroup = "ExportStatus"
                .Visible = True
                .Width = 16
                .OptionsColumn.AllowSize = False
                .OptionsColumn.ShowCaption = False
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                'AG 14/02/2014 - #1505
                '.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False 'AG 29/10/2012 (sort only by date, patientID, type, testname and remarkalert)
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                'AG 14/02/2014 - #1505
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
                .ColumnEdit = pictureEdit
            End With

            'Graph column
            column = historyGridView.Columns.Add()
            With column
                .Caption = ""
                .FieldName = "GraphImage"
                .Name = "GraphImage"
                .Visible = False 'No V1
                .Width = 16
                .OptionsColumn.AllowSize = False
                .OptionsColumn.ShowCaption = False
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
                .ColumnEdit = pictureEdit
            End With

            'Remarks column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_Remarks")
                .FieldName = "Remarks"
                .Name = "Remarks"
                .Visible = True
                .Width = 250
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False 'AG 29/10/2012 (sort only by date, patientID, type, testname and remarkalert)
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
                .ColumnEdit = largeTextEdit
            End With

            'AdditionalInfo column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_AdditionalInformation")
                .FieldName = "AdditionalInfo"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                .DisplayFormat.FormatString = SystemInfoManager.OSLongTimeFormat
                .Name = "AdditionalInfo"
                .Visible = True
                .Width = 150
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False 'AG 29/10/2012 (sort only by date, patientID, type, testname and remarkalert)
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
                .ColumnEdit = largeTextEdit
            End With

            'AnalyzerColumn Column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_SCRIPT_EDIT_Analyzer")
                .FieldName = "AnalyzerID"
                .Name = "AnalyzerID"
                .Visible = True
                .Width = 120
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            ' XB 03/10/2013
            'TestPosition Column
            column = historyGridView.Columns.Add()
            With column
                .Caption = "TestPosition"
                .FieldName = "TestPosition"
                .Name = "TestPosition"
                .Visible = False
                .Width = 120
                .OptionsColumn.AllowSize = False
                .OptionsColumn.ShowCaption = False
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeResultHistoryGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeResultHistoryGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the filter search
    ''' </summary>
    ''' <remarks>
    ''' Created by JB 18/10/2012
    ''' </remarks>
    Private Sub InitializeFilterSearch()
        Try
            dateFromDateTimePick.Checked = True
            dateFromDateTimePick.Value = Today.AddDays(-1)

            dateToDateTimePick.Checked = True
            dateToDateTimePick.Value = Today

            statFlagComboBox.SelectedIndex = 0

            If (analyzerIDComboBox.Items.Count > 0) Then
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
    ''' Created by: JB 18/10/2012
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            GetScreenLabels()
            PrepareButtons()
            FillDropDownLists()

            'SG 31/05/2013 - Get Level of the connected User
            Dim MyGlobalBase As New GlobalBase
            CurrentUserLevel = MyGlobalBase.GetSessionInfo().UserLevel

            'AG 22/10/2012 - Get the alarms descriptions
            Dim resultData As New GlobalDataTO
            Dim alarmsDlg As New AlarmsDelegate

            resultData = alarmsDlg.ReadAll(Nothing)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                alarmsDefiniton = DirectCast(resultData.SetDatos, AlarmsDS)
            End If

            InitializeResultHistoryGrid()

            InitializeFilterSearch()

            FindHistoricalResults()

            ScreenStatusByUserLevel() 'SGM 31/05/2013

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Search"
    ''' <summary>
    ''' Returns the selected search filter
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSearchFilter() As SearchFilter
        Dim filter As New SearchFilter
        With filter
            .analyzerId = analyzerIDComboBox.SelectedItem.ToString

            If (dateFromDateTimePick.Checked) Then
                .dateFrom = dateFromDateTimePick.Value
            Else
                .dateFrom = Nothing
            End If

            If (dateToDateTimePick.Checked) Then
                .dateTo = dateToDateTimePick.Value.AddDays(1)
            Else
                .dateTo = Nothing
            End If

            .samplePatientID = sampleIdTextBox.Text

            .sampleClasses = New List(Of String)
            .sampleClasses.Add("PATIENT")

            .statFlag = (From kvp As KeyValuePair(Of TriState, String) In mStatFlags _
                        Where kvp.Value = statFlagComboBox.SelectedItem.ToString _
                       Select kvp.Key).FirstOrDefault

            .sampleTypes = New List(Of String)
            For Each elem As DevExpress.XtraEditors.Controls.CheckedListBoxItem In sampleTypeChkComboBox.Properties.Items
                If (elem.CheckState = CheckState.Checked) Then .sampleTypes.Add(elem.Value.ToString)
            Next

            .testTypes = New List(Of String)
            For Each elem As DevExpress.XtraEditors.Controls.CheckedListBoxItem In testTypeChkComboBox.Properties.Items
                If (elem.CheckState = CheckState.Checked) Then .testTypes.Add(elem.Value.ToString)
            Next

            .testStartName = testNameTextBox.Text
        End With
        Return filter
    End Function

    ''' <summary>
    ''' Find the Results by the selected filter in screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012
    ''' Modified by: AG 13/02/2014 - BT #1505
    ''' </remarks>
    Private Sub FindHistoricalResults()
        Dim myGlobalDataTO As New GlobalDataTO
        Dim myHisWSResultsDS As HisWSResultsDS

        historyGrid.DataSource = Nothing
        If analyzerIDComboBox.Items.Count = 0 Then Exit Sub
        Try
            UpdateFormBehavior(False)

            Me.Cursor = Cursors.WaitCursor
            Dim StartTime As DateTime = Now 'AG 13/02/2014 - #1505
            Dim filter As SearchFilter = GetSearchFilter()
            With filter
                'AG 14/02/2014 - #1505
                Dim valueDateTo As Date = filter.dateTo
                Dim diffDays As TimeSpan = valueDateTo.Subtract(filter.dateFrom)
                CreateLogActivity("Days number to find: " & diffDays.Days.ToString, Me.Name & ".FindHistoricalResults", EventLogEntryType.Information, False)
                'AG 14/02/2014 - #1505

                myGlobalDataTO = myHisWSResultsDelegate.GetHistoricalResultsByFilter(Nothing, .analyzerId, .dateFrom, .dateTo, .samplePatientID, .sampleClasses, _
                                                                                     .sampleTypes, .statFlag, .testTypes, .testStartName)
            End With
            CreateLogActivity("GetHistoricalResultsByFilter: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Me.Name & ".FindHistoricalResults", EventLogEntryType.Information, False) 'AG 13/02/2014 - #1505

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myHisWSResultsDS = DirectCast(myGlobalDataTO.SetDatos, HisWSResultsDS)
                PrepareAndSetDataToGrid(myHisWSResultsDS.vhisWSResults)
                'AG 13/02/2014 - #1505 NOTE: Do not add trace, it has already been added into method PrepareAndSetDataToGrid

            ElseIf myGlobalDataTO.HasError Then
                ShowMessage(Me.Name & ".FindHistoricalResults", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FindHistoricalResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FindHistoricalResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            Me.Cursor = Cursors.Default
            UpdateFormBehavior(True)
        End Try
    End Sub

    ''' <summary>
    ''' Decodes the images of the row
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <remarks>
    ''' Created by: JB 19/10/2012 
    ''' </remarks>
    Private Sub DecodeRowImages(ByRef pRow As HisWSResultsDS.vhisWSResultsRow)
        Try
            With pRow
                If .StatFlag Then
                    .StatFlagImage = GetImageGrid("STATS")
                Else
                    .StatFlagImage = GetImageGrid("ROUTINES")
                End If
                If .ExportStatus = "SENT" Then
                    .ExportImage = GetImageGrid("EXPORTHEAD")
                End If
                If .TestType = "STD" Then
                    .GraphImage = GetImageGrid("AVG_ABS_GRAPH")
                End If
            End With
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DecodeRowImages ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DecodeRowImages ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Decodes the row texts
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <remarks>
    ''' Created by: JB 19/10/2012
    ''' </remarks>
    Private Function DecodeRowTexts(ByRef pRow As HisWSResultsDS.vhisWSResultsRow) As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            With pRow
                'Reference Range
                If (Not .IsMinRefRangeNull AndAlso Not .IsMaxRefRangeNull) Then .RefRange = .MinRefRange.ToString & " - " & .MaxRefRange.ToString

                'Additional Info
                .AdditionalInfo = .FormulaText

                'CONC Value String
                .CONCValueString = String.Empty
                If (.IsCONCValueNull) Then
                    If (.TestType = "OFFS") Then
                        If (Not .IsManualResultNull) Then
                            .CONCValueString = .ManualResult.ToStringWithDecimals(.DecimalsAllowed)
                        Else
                            .CONCValueString = .ManualResultText
                        End If
                    End If
                Else
                    .CONCValueString = .CONCValue.ToStringWithDecimals(.DecimalsAllowed)
                End If

                'Remarks
                If (Not .IsAlarmListNull) Then
                    'Set the alarm description into .Remarks field
                    If (alarmsDefiniton.tfmwAlarms.Rows.Count > 0) Then
                        myGlobalDataTO = myHisWSResultsDelegate.GetAvgAlarmsForScreen(Nothing, alarmsDefiniton, pRow)

                    Else 'We can decode alarm so leave the columns empty
                        .Remarks = String.Empty
                        .RemarkAlert = String.Empty
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
    ''' Created by:  JB 19/10/2012
    ''' Modified by: AG 13/02/2014 - BT #1505
    ''' </remarks>
    Private Sub PrepareAndSetDataToGrid(ByVal pHisResultsDataTable As HisWSResultsDS.vhisWSResultsDataTable)
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            UpdateFormBehavior(False)

            Dim StartTime As DateTime = Now 'AG 13/02/2014 - #1505
            For Each row As HisWSResultsDS.vhisWSResultsRow In pHisResultsDataTable
                'Mark the row as "not selected"
                row.Selected = False

                'Get the images
                DecodeRowImages(row)

                'Get the texts
                myGlobalDataTO = DecodeRowTexts(row)
                If myGlobalDataTO.HasError Then Exit For

                'JV 03/01/2014 BT #1285 - Comment DoEvents to avoid flicking on the DataGrid area (data, images, etc.) and also on the form
                'Application.DoEvents()
            Next

            CreateLogActivity("PrepareAndSetDataToGrid : " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Me.Name & ".PrepareAndSetDataToGrid", EventLogEntryType.Information, False) 'AG 13/02/2014 - #1505
            StartTime = Now 'AG 13/02/2014 - #1505

            If (Not myGlobalDataTO.HasError) Then
                historyGrid.DataSource = pHisResultsDataTable
                CreateLogActivity("Assign HistoryGrid.DataSource = HisResults dataset : " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Me.Name & ".PrepareAndSetDataToGrid", EventLogEntryType.Information, False) 'AG 13/02/2014 - #1505
            Else
                historyGrid.DataSource = Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".PrepareAndSetDataToGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".PrepareAndSetDataToGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            UpdateFormBehavior(True)
        End Try
    End Sub
#End Region

#Region "Behavior"
    ''' <summary></summary>
    ''' <remarks>
    ''' created by SG 31/05/2013
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    historyDeleteButton.Enabled = False
                    Exit Select
            End Select
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Updates the Form Behavior: enable buttons, show actions...
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012
    ''' Modified by: DL 25/04/2013. New condition for activate export status when any result NOT EXPORTED  
    ''' </remarks>
    Private Sub UpdateFormBehavior(ByVal pStatus As Boolean)
        Try
            searchGroup.Enabled = pStatus
            exitButton.Enabled = pStatus

            Dim selectedRows As List(Of HisWSResultsDS.vhisWSResultsRow) = GetSelectedRows()

            historyDeleteButton.Enabled = pStatus AndAlso selectedRows.Count > 0

            'DL 25/04/2013
            Dim selectedNotExport As Integer = (From row In selectedRows Where row.ExportStatus = "NOTSENT").Count
            exportButton.Enabled = pStatus AndAlso selectedRows.Count > 0 AndAlso selectedNotExport > 0
            'DL 25/04/2013

            PrintButton.Enabled = pStatus AndAlso selectedRows.Count > 0
            searchGroup.Enabled = pStatus AndAlso analyzerIDComboBox.Items.Count > 0
            CompactPrintButton.Enabled = pStatus AndAlso selectedRows.Count > 0

            ScreenStatusByUserLevel() 'SGM 31/05/2013
            selectedRows = Nothing 'AG 14/02/2014 - #1505 release memory
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateFormBehavior ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpdateFormBehavior ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Export all selected rows in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 22/10/2012
    ''' Modified by: SA 17/12/2012 - If an error has happened when expoting, shown it 
    '''              DL 24/04/2013 - 
    '''              AG 13/02/2014 - BT #1505
    ''' </remarks>
    Private Sub ExportSelectedRowsFromGrid(ByVal pGrid As DevExpress.XtraGrid.Views.Grid.GridView)
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            Dim StartTime As DateTime = Now 'AG 13/02/2014 - #1505

            Dim selectedRows As List(Of HisWSResultsDS.vhisWSResultsRow) = GetSelectedRows()
            If selectedRows.Count = 0 Then Exit Sub

            'AG 14/02/2014 - #1505 Evaluate the results to export from historic are lower than the limit
            Dim maxHistResultsToExport As Integer = 100 'Default value
            Dim swParamDlg As New SwParametersDelegate
            myGlobalDataTO = swParamDlg.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_RESULTSTOEXPORT_HIST.ToString, Nothing)
            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                Dim myDS As New ParametersDS
                myDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)
                If myDS.tfmwSwParameters.Rows.Count = 0 Then
                    'Do nothing, in v300 initial database contains this parameter
                Else
                    If Not myDS.tfmwSwParameters(0).IsValueNumericNull Then
                        maxHistResultsToExport = CInt(myDS.tfmwSwParameters(0).ValueNumeric)
                    End If
                End If
            End If
            'AG 14/02/2014 - #1505

            'Show export confirmation message ??
            'If (ShowMessage(Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) <> Windows.Forms.DialogResult.Yes) Then Exit Sub

            'BEGIN DL 24/04/2013 - The results SENT or SENDING could not be uploaded again from Historical results screen 
            Dim histOrderTestIDList As List(Of Integer) = (From row In selectedRows
                                                          Where row.ExportStatus <> "SENT" AndAlso row.ExportStatus <> "SENDING" _
                                                         Select row.HistOrderTestID Distinct).ToList

            'AG 14/02/2014 - #1505 If number of results to export > limit --> show warning!! No export
            If histOrderTestIDList.Count > maxHistResultsToExport Then
                'Upload only the maxHistResults - (CANCELLED, instead of this show a message)
                'histOrderTestIDList = histOrderTestIDList.GetRange(0, maxHistResultsToExport)

                'Show message and leave method
                MessageBox.Show(Me, "Please, export to LIS in groups of " & maxHistResultsToExport & " results (maximum)", My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            'AG 14/02/2014 - #1505

            Dim myExportDelegate As New ExportDelegate
            myGlobalDataTO = myExportDelegate.ExportToLISManualFromHIST(histOrderTestIDList)
            CreateLogActivity("ExportToLISManualFromHIST: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Me.Name & ".ExportSelectedRowsFromGrid", EventLogEntryType.Information, False) 'AG 13/02/2014 - #1505
            StartTime = Now 'AG 13/02/2014 - #1505

            ' For testing LIS History Upload
            If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing Then
                'Get the results exported (ExecutionsDS) from GlobalDataToReturned
                Dim myExportedExecutionsDS As ExecutionsDS = TryCast(myGlobalDataTO.SetDatos, ExecutionsDS)
                Dim myHisWSResultsDS As HisWSResultsDS = New HisWSResultsDS

                'AG 14/02/2014 - #1505 do not create DS using selectedRows, add only those results to export!!!
                'Create a new HisWSResultsDS (data table vhisWSResults) using the list of selected rows
                'For Each HisWSResultsRow As HisWSResultsDS.vhisWSResultsRow In selectedRows
                '    myHisWSResultsDS.vhisWSResults.ImportRow(HisWSResultsRow)
                'Next
                For Each HisWSResultsRow As HisWSResultsDS.vhisWSResultsRow In (From A In selectedRows Where A.ExportStatus <> "SENT" AndAlso A.ExportStatus <> "SENDING" Select A).ToList
                    If Not HisWSResultsRow.IsHistOrderTestIDNull AndAlso histOrderTestIDList.Contains(HisWSResultsRow.HistOrderTestID) Then
                        myHisWSResultsDS.vhisWSResults.ImportRow(HisWSResultsRow)
                    End If
                Next
                myHisWSResultsDS.AcceptChanges()

                'AG 13/02/2014 - #1505
                CreateLogActivity("Prepare myHisWSResultsDS (loop): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Me.Name & ".ExportSelectedRowsFromGrid", EventLogEntryType.Information, False)
                'CreateLogActivity("Historical Results manual upload", Me.Name & ".ExportSelectedRowsFromGrid ", EventLogEntryType.Information, False) 'AG 02/01/2014 - BT #1433 (v211 patch2) 'Comment this line AG 13/02/2014
                StartTime = Now
                'AG 13/02/2014 - #1505

                ' 'Inform the new results to be updated into MDI property
                If myExportedExecutionsDS.twksWSExecutions.Rows.Count > 0 Then 'AG 21/02/2014 - #1505 call mdi threat only when needed
                    IAx00MainMDI.AddResultsIntoQueueToUpload(myExportedExecutionsDS)
                    IAx00MainMDI.InvokeUploadResultsLIS(True, Nothing, Nothing, myHisWSResultsDS)
                End If 'AG 21/02/2014 - #1505

                CreateLogActivity("Historical Results manual upload (end): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Me.Name & ".ExportSelectedRowsFromGrid", EventLogEntryType.Information, False) 'AG 13/02/2014 - #1505

            End If
            'DL 24/04/2013. END

            If (myGlobalDataTO.HasError) Then
                'If an error has happened when expoting, shown it
                ShowMessage(Me.Name & ".ExportSelectedRowsFromGrid ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
            FindHistoricalResults()

            'AG 14/02/2014 - BT #1505 - relesea memory
            selectedRows = Nothing
            histOrderTestIDList = Nothing
            'AG 14/02/2014 - BT #1505

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExportSelectedRowsFromGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExportSelectedRowsFromGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete all selected rows in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 19/10/2012
    ''' Modified by: SG 02/07/2013 - Changes to use the new function for delete Historic Patient Results; the previous one was wrong because
    '''                              it deletes only the result from table thisWSResults, but not the rest of data
    '''              AG 13/02/2014 - BT #1505
    ''' </remarks>
    Private Sub DeleteSelectedRowsFromGrid(ByVal pGrid As DevExpress.XtraGrid.Views.Grid.GridView)
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            If (ShowMessage(Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                Dim StartTime As DateTime = Now 'AG 13/02/2014 - #1505

                Dim workingThread As New Threading.Thread(AddressOf DeleteHistOrderTests)
                DeletingHistOrderTests = True
                ScreenWorkingProcess = True

                MyClass.EnableScreen(False)
                IAx00MainMDI.EnableButtonAndMenus(False)

                workingThread.Start()

                While DeletingHistOrderTests
                    IAx00MainMDI.InitializeMarqueeProgreesBar()
                    Cursor = Cursors.WaitCursor
                    Application.DoEvents()
                End While

                workingThread = Nothing
                IAx00MainMDI.StopMarqueeProgressBar()
                CreateLogActivity("DeleteSelectedRowsFromGrid: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Me.Name & ".DeleteSelectedRowsFromGrid", EventLogEntryType.Information, False) 'AG 13/02/2014 - #1505
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteSelectedRowsFromGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        MyClass.EnableScreen(True)
        IAx00MainMDI.EnableButtonAndMenus(True)
        Me.Cursor = Cursors.Default
        MyClass.UpdateFormBehavior(True)
    End Sub

    ''' <summary>
    ''' Delete all selected rows in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 02/07/2013
    ''' </remarks>
    Public Sub DeleteHistOrderTests()
        Try
            Debug.Print(DateTime.Now.ToString("H:mm:ss:fff") & " - start get selected")

            Dim hisWSOTToDeleteDS As HisWSOrderTestsDS = GetSelectedRowsNEW()
            If (hisWSOTToDeleteDS.thisWSOrderTests.Rows.Count = 0) Then Exit Try

            Dim resultData As New GlobalDataTO
            Dim hisWSOTDelegate As New HisWSOrderTestsDelegate
            
            resultData = myHisWSResultsDelegate.DeleteResults(Nothing, hisWSOTToDeleteDS)
            If (Not resultData.HasError) Then
                'Reload the grid
                Debug.Print(DateTime.Now.ToString("H:mm:ss:fff") & " - start reload grid")
                Me.UIThread(Sub() FindHistoricalResults())
            Else
                Me.UIThread(Function() ShowMessage(Me.Name & ".DeleteSelectedRowsFromGrid ", resultData.ErrorCode, resultData.ErrorMessage, Me))
            End If

            Debug.Print(DateTime.Now.ToString("H:mm:ss:fff") & " - end delete all")
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".DeleteHistOrderTests ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            Me.UIThread(Function() ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))"))
        Finally
            MyClass.DeletingHistOrderTests = False
            MyBase.ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it
        End Try
    End Sub

    ''' <summary>
    ''' Enables/Disables the screen
    ''' </summary>
    ''' <param name="pEnable"></param>
    ''' <remarks>
    ''' Created by:  SG 02/07/2013
    ''' </remarks>
    Private Sub EnableScreen(ByVal pEnable As Boolean)
        Try
            Me.searchGroup.Enabled = pEnable
            Me.historyGrid.Enabled = pEnable
            Me.historyDeleteButton.Enabled = pEnable
            Me.exportButton.Enabled = pEnable
            Me.PrintButton.Enabled = pEnable
            Me.CompactPrintButton.Enabled = pEnable
            Me.exitButton.Enabled = pEnable
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".EnableScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Open the Graph Screen with the result data
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <remarks>
    ''' TODO!!!
    ''' Created by:  JB 23/10/2012
    ''' </remarks>
    Private Sub OpenGraphScreen(ByVal pRow As HisWSResultsDS.vhisWSResultsRow)
        Try
            'TODO: Show the History Result Graph Screen 

        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".OpenGraphScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#Region " Selection "
    ''' <summary>
    ''' Get the selected rows in the grid
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by:  JB 24/10/2012
    ''' </remarks>
    Protected Overridable Function GetSelectedRows() As List(Of HisWSResultsDS.vhisWSResultsRow)
        Try
            Dim dataTable As HisWSResultsDS.vhisWSResultsDataTable = DirectCast(historyGrid.DataSource, HisWSResultsDS.vhisWSResultsDataTable)
            If dataTable IsNot Nothing Then
                Return (From row In dataTable Where row.Selected).ToList
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSelectedRows ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return New List(Of HisWSResultsDS.vhisWSResultsRow)
    End Function

    Private Function GetSelectedRowsNEW() As HisWSOrderTestsDS
        Dim hisWSOrderTestsDS As New HisWSOrderTestsDS

        Try
            Dim dataTable As HisWSResultsDS.vhisWSResultsDataTable = DirectCast(historyGrid.DataSource, HisWSResultsDS.vhisWSResultsDataTable)
            If (Not dataTable Is Nothing) Then
                Dim lstSelectedResults As List(Of HisWSResultsDS.vhisWSResultsRow) = (From row In dataTable Where row.Selected).ToList

                Dim hisWSOTRow As HisWSOrderTestsDS.thisWSOrderTestsRow
                For Each row As HisWSResultsDS.vhisWSResultsRow In lstSelectedResults
                    'hisWSOrderTestsDS.thisWSOrderTests.ImportRow(row)
                    hisWSOTRow = hisWSOrderTestsDS.thisWSOrderTests.NewthisWSOrderTestsRow()
                    hisWSOTRow.AnalyzerID = row.AnalyzerID
                    hisWSOTRow.WorkSessionID = row.WorkSessionID
                    hisWSOTRow.HistOrderTestID = row.HistOrderTestID
                    hisWSOrderTestsDS.thisWSOrderTests.AddthisWSOrderTestsRow(hisWSOTRow)
                Next
                hisWSOrderTestsDS.AcceptChanges()
                lstSelectedResults = Nothing
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSelectedRowsNEW ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return hisWSOrderTestsDS
    End Function

    ''' <summary>
    ''' Select all rows in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  24/10/2012
    ''' </remarks>
    Private Sub SelectAllRows(ByVal selection As Boolean)
        Try
            UpdateFormBehavior(False)

            Dim dataTable As HisWSResultsDS.vhisWSResultsDataTable = DirectCast(historyGrid.DataSource, HisWSResultsDS.vhisWSResultsDataTable)
            If dataTable IsNot Nothing Then
                For Each row As HisWSResultsDS.vhisWSResultsRow In dataTable
                    row.Selected = selection
                Next
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SelectAllRows ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            UpdateFormBehavior(True)
        End Try
    End Sub
#End Region
#End Region
#End Region

#Region "Events"

#Region " Screen Events "
    Private Sub IHisResults_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then exitButton.PerformClick()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IHisResults_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IHisResults_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IHisResults_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If (Me.DesignMode) Then Exit Sub

            InitializeScreen()
            ResetBorder()
            Application.DoEvents()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IHisResults_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IHisResults_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " Button Events "
    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitButton.Click
        Try
            'AG 24/02/2014 - use parameter MAX_APP_MEMORYUSAGE into performance counters (but do not show message here!!!)  ' XB 18/02/2014 BT #1499
            Dim PCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage)
            PCounters.GetAllCounters()
            PCounters = Nothing
            'XB 18/02/2014 BT #1499

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
        'XB 14/02/2014 - register Historic screen user actions into the Log - Task #1495
        Dim myLogAcciones As New ApplicationLogManager()
        myLogAcciones.CreateLogActivity("User press 'Search' Button", Name & ".bsSearchButton_Click ", EventLogEntryType.Information, False)

        FindHistoricalResults()
    End Sub

    Private Sub bsHistoryDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles historyDeleteButton.Click
        'XB 14/02/2014 - register Historic screen user actions into the Log - Task #1495
        Dim myLogAcciones As New ApplicationLogManager()
        myLogAcciones.CreateLogActivity("User press 'Delete' Button", Name & ".bsHistoryDelete_Click ", EventLogEntryType.Information, False)

        DeleteSelectedRowsFromGrid(historyGridView)
    End Sub

    Private Sub ExportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exportButton.Click
        'XB 14/02/2014 - register Historic screen user actions into the Log - Task #1495
        Dim myLogAcciones As New ApplicationLogManager()
        myLogAcciones.CreateLogActivity("User press 'Export' Button", Name & ".ExportButton_Click ", EventLogEntryType.Information, False)

        ExportSelectedRowsFromGrid(historyGridView)
    End Sub

    ''' <summary>
    ''' Prints Historic Results by Patient report
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 26/10/2012
    ''' Modified by XB 03/10/2013 - BT #1309 ==> Added TestPosition field to preserve the sorting configured on Reports Tests Sorting screen 
    ''' </remarks>
    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintButton.Click
        Try
            Dim StartTime As DateTime = Now                         '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim myLogAcciones As New ApplicationLogManager()        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim myHisWSResults As List(Of HisWSResultsDS.vhisWSResultsRow)

            myHisWSResults = GetSelectedRows()
            sampleTypeChkComboBox.Properties.TextEditStyle = TextEditStyles.DisableTextEditor
            If myHisWSResults.Count > 0 Then
                ' XB 03/10/2013
                'sort the result
                Dim myHisWSResultsSorted As List(Of HisWSResultsDS.vhisWSResultsRow) = (From a In myHisWSResults _
                                                                                        Order By a.TestPosition _
                                                                                        Select a).ToList
                myHisWSResults.Clear()
                XRManager.ShowHistoricResultsByPatientSampleReport(myHisWSResultsSorted)
                ' XB 03/10/2013

                myHisWSResultsSorted = Nothing 'AG 14/02/2014 - #1505 release memory
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("Test Results Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.PrintTestButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myHisWSResults = Nothing 'AG 14/02/2014 - #1505 release memory

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrintTestButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Modified by XB 03/10/2013 - BT #1309 ==> Added TestPosition field to preserve the sorting configured on Reports Tests Sorting screen 
    ''' </remarks>
    Private Sub CompactPrintButton_Click(sender As Object, e As EventArgs) Handles CompactPrintButton.Click
        Try
            Dim StartTime As DateTime = Now                         '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim myLogAcciones As New ApplicationLogManager()        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim myHisWSResults As List(Of HisWSResultsDS.vhisWSResultsRow)

            myHisWSResults = GetSelectedRows()
            sampleTypeChkComboBox.Properties.TextEditStyle = TextEditStyles.DisableTextEditor
            If (myHisWSResults.Count > 0) Then
                'XB 03/10/2013 - Sort the result
                Dim myHisWSResultsSorted As List(Of HisWSResultsDS.vhisWSResultsRow) = (From a In myHisWSResults _
                                                                                        Order By a.TestPosition _
                                                                                        Select a).ToList
                myHisWSResults.Clear()
                XRManager.ShowHistoricByCompactPatientsSamplesResult(myHisWSResultsSorted)
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("Test Results Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.CompactPrintButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myHisWSResults = Nothing 'AG 14/02/2014 - #1505 release memory
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CompactPrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

#Region " DateTimePicker Events "
    Private Sub bsDateFromDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dateFromDateTimePick.ValueChanged
        Try
            If dateFromDateTimePick.Checked Then
                dateToDateTimePick.MinDate = dateFromDateTimePick.Value
            Else
                dateToDateTimePick.MinDate = Today.AddYears(-100)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsDateFromDateTimePick_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDateFromDateTimePick_ValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsDateToDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dateToDateTimePick.ValueChanged
        Try
            If dateToDateTimePick.Checked Then
                dateFromDateTimePick.MaxDate = dateToDateTimePick.Value
            Else
                dateFromDateTimePick.MaxDate = Today.AddYears(100)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsDateToDateTimePick_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDateToDateTimePick_ValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " CheckComboBox Events "
    Private Sub sampleTypeChkComboBox_CustomDisplayText(ByVal sender As System.Object, ByVal e As DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs) Handles sampleTypeChkComboBox.CustomDisplayText
        Try
            Dim edit As CheckedComboBoxEdit = TryCast(sender, CheckedComboBoxEdit)
            Dim numChecks As Integer = 0
            Dim text As String = ""
            For Each item As CheckedListBoxItem In edit.Properties.Items
                If item.CheckState = CheckState.Checked Then
                    numChecks += 1
                    If Not String.IsNullOrEmpty(text) Then text &= ", "
                    text &= item.Value.ToString
                End If
            Next item
            If numChecks = 0 OrElse numChecks = edit.Properties.Items.Count Then
                text = GetText("LBL_SRV_All")
            End If
            e.DisplayText = text
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".sampleTypeChkComboBox_CustomDisplayText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".sampleTypeChkComboBox_CustomDisplayText ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub testTypeChkComboBox_CustomDisplayText(ByVal sender As System.Object, ByVal e As DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs) Handles testTypeChkComboBox.CustomDisplayText
        Try
            Dim edit As CheckedComboBoxEdit = TryCast(sender, CheckedComboBoxEdit)
            Dim numChecks As Integer = 0
            For Each item As CheckedListBoxItem In edit.Properties.Items
                If item.CheckState = CheckState.Checked Then numChecks += 1
            Next item
            If numChecks = 0 OrElse numChecks = edit.Properties.Items.Count Then
                e.DisplayText = GetText("LBL_SRV_All")
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".testTypeChkComboBox_CustomDisplayText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".testTypeChkComboBox_CustomDisplayText ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " Grid Events "
    Private Sub xtraHistoryGridView_SelectionChanged(ByVal sender As System.Object, ByVal e As DevExpress.Data.SelectionChangedEventArgs) Handles historyGridView.SelectionChanged
        UpdateFormBehavior(True)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  JB 23/10/2012 
    ''' </remarks>
    Private Sub historyGridView_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles historyGridView.MouseMove
        Try
            With historyGridView.CalcHitInfo(New Point(e.X, e.Y))
                If .InColumnPanel AndAlso .Column IsNot Nothing AndAlso .Column.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True Then
                    Me.Cursor = Cursors.Hand
                ElseIf .InRowCell AndAlso .Column IsNot Nothing AndAlso .Column.Name = "GraphImage" Then
                    Me.Cursor = Cursors.Hand
                Else
                    Me.Cursor = Cursors.Default
                End If
            End With
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".historyGridView_MouseMove ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Processes the mouse click event over the cells (history grid)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  AG 23/10/2012 (Addapted from IHisBlankCalibResults)
    ''' </remarks>
    Private Sub historyGridView_RowCellClick(ByVal sender As System.Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs) Handles historyGridView.RowCellClick
        Try
            UpdateFormBehavior(False)
            Cursor = Cursors.WaitCursor
            If sender Is Nothing Then Return
            Dim dgv As DevExpress.XtraGrid.Views.Grid.GridView = CType(sender, DevExpress.XtraGrid.Views.Grid.GridView)

            'This is how to get the DataRow behind the GridViewRow
            Dim currentRow As HisWSResultsDS.vhisWSResultsRow = CType(dgv.GetDataRow(e.RowHandle), HisWSResultsDS.vhisWSResultsRow)

            currentRow.Selected = Not currentRow.Selected

            Select Case e.Column.Name
                Case "GraphImage"
                    OpenGraphScreen(currentRow)
            End Select

        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".historyGridView_RowCellClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Cursor = Cursors.Default
            UpdateFormBehavior(True)
        End Try
    End Sub

    ''' <summary>
    ''' Processes the CustomDrawColumnHeader event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: JB 24/10/2012
    ''' </remarks>
    Private Sub historyGridView_CustomDrawColumnHeader(ByVal sender As System.Object, ByVal e As DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs) Handles historyGridView.CustomDrawColumnHeader
        Try
            If e.Column Is Nothing Then Exit Sub

            If e.Column.Name = "Selected" Then
                Dim selectedRows As List(Of HisWSResultsDS.vhisWSResultsRow) = GetSelectedRows()

                e.Info.InnerElements.Clear()
                e.Painter.DrawObject(e.Info)
                DrawCheckBox(e.Graphics, e.Bounds, selectedRows.Count > 0 AndAlso selectedRows.Count = historyGridView.DataRowCount)
                e.Handled = True
                selectedRows = Nothing 'AG 14/02/2014 - #1505 release memory
            End If

        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".historyGridView_CustomDrawColumnHeader ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Processes the Click event in the GridView
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by:  JB 24/10/2012
    ''' </remarks>
    Private Sub historyGridView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles historyGridView.Click
        Try
            With historyGridView.CalcHitInfo(historyGrid.PointToClient(MousePosition))
                If .Column IsNot Nothing AndAlso .Column.Name = "Selected" AndAlso .InColumn Then
                    Dim selectedRows As List(Of HisWSResultsDS.vhisWSResultsRow) = GetSelectedRows()

                    If historyGridView.DataRowCount > 0 Then
                        SelectAllRows(selectedRows.Count <> historyGridView.DataRowCount)
                    End If
                    selectedRows = Nothing 'AG 14/02/2014 - #1505 release memory
                End If
            End With
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".historyGridView_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

#End Region
#End Region
End Class