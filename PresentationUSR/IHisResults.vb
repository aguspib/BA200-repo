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

#Region "Structures"
    'SA 01/08/2014
    'BT #1861 ==> Added new field specimenID. Removed field sampleClasses, it is not needed.
    Public Structure SearchFilter
        Public analyzerId As String
        Public dateFrom As Date
        Public dateTo As Date
        Public patientData As String
        Public specimenID As String
        Public statFlag As TriState
        Public sampleTypes As List(Of String)
        Public testTypes As List(Of String)
        Public testName As String
    End Structure
#End Region

#Region "Declarations"
    'Variables for multilanguage texts
    Private currentLanguage As String
    Private mTextDict As Dictionary(Of String, String)

    'Variables for screen images (graphical buttons and grid columns)
    Private mImageDict As Dictionary(Of String, Image)
    Private mImageGridDict As Dictionary(Of String, Byte())

    'Data dictionaries and lists for ComboBoxes in Filter Area
    Private mStatFlags As Dictionary(Of TriState, String)     'Stat Flags:   {StatFlag Code, Multilanguage Text}
    Private mSampleTypes As Dictionary(Of String, String)     'Sample Types: {SampleType Code, Multilanguage Text}
    Private mTestTypes As Dictionary(Of String, String)       'Test Types:   {TestType Code, Multilanguage Text}
    Private mAnalyzers As List(Of String)                     'AnalyzerIDs

    'AG 22/10/2012
    'Typed DataSet containing the list of all defined Alarms
    Private alarmsDefiniton As New AlarmsDS

    'Delegate Class to manage Historic Patient Data 
    Private myHisWSResultsDelegate As HisWSResultsDelegate

    'SG 02/07/2013
    'Global variable to control the thread for deleting Historic Patient Data
    Private DeletingHistOrderTests As Boolean = False

    'SA 01/08/2014
    'BT #1861 ==> Global DS to get/save the width of all visible grid columns
    Private gridColWidthConfigDS As GridColsConfigDS
#End Region

#Region "Constructor"
    Public Sub New()
        MyBase.New()
        InitializeComponent()

        'Initialize all global dictionaries and lists
        mImageDict = New Dictionary(Of String, Image)()
        mTextDict = New Dictionary(Of String, String)()
        mImageGridDict = New Dictionary(Of String, Byte())()
        mStatFlags = New Dictionary(Of TriState, String)
        mSampleTypes = New Dictionary(Of String, String)
        mTestTypes = New Dictionary(Of String, String)
        mAnalyzers = New List(Of String)

        'Initialize the global variable for the Delegate Class 
        myHisWSResultsDelegate = New HisWSResultsDelegate

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

#Region "Private Methods to load local Dictionaries and Lists"
    ''' <summary>
    ''' Gets the image by its key. Reads from file only once for each key
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012 
    ''' </remarks>
    Private Function GetImage(ByVal pKey As String) As Image
        If (Not mImageDict.ContainsKey(pKey)) Then SetImageToDictionary(pKey)
        If (Not mImageDict.ContainsKey(pKey)) Then Return New Bitmap(16, 16)
        Return mImageDict.Item(pKey)
    End Function

    ''' <summary>
    ''' Get the specified Icon image and load it in the images dictionary 
    ''' </summary>
    ''' <param name="pKey">Name of the Icon image to load</param>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012 
    ''' </remarks>
    Private Sub SetImageToDictionary(ByVal pKey As String)
        Dim auxIconName As String = String.Empty
        Dim iconPath As String = MyBase.IconsPath

        auxIconName = GetIconName(pKey)
        If (Not String.IsNullOrEmpty(auxIconName)) Then
            If (mImageDict.ContainsKey(pKey)) Then
                mImageDict.Item(pKey) = Image.FromFile(iconPath & auxIconName)
            Else
                mImageDict.Add(pKey, Image.FromFile(iconPath & auxIconName))
            End If
        End If
    End Sub

    ''' <summary>
    ''' Gets the multilanguage text by its key. Reads from DataBase only once for key
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012 
    ''' </remarks>
    Private Function GetText(ByVal pKey As String) As String
        If (Not mTextDict.ContainsKey(pKey)) Then SetTextToDictionary(pKey)
        If (Not mTextDict.ContainsKey(pKey)) Then Return String.Empty
        Return mTextDict.Item(pKey)
    End Function

    ''' <summary>
    ''' Get the multilanguage text for the specified Resource and load it in the texts dictionary 
    ''' </summary>
    ''' <param name="pKey">Name of the Resource to load</param>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012 
    ''' </remarks>
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
    ''' Gets the grid image by its key. Reads from file only once for each key
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 19/10/2012 
    ''' </remarks>
    Private Function GetImageGrid(ByVal pKey As String) As Byte()
        If (Not mImageGridDict.ContainsKey(pKey)) Then SetImageGridToDictionary(pKey)
        If (Not mImageGridDict.ContainsKey(pKey)) Then Return Nothing
        Return mImageGridDict.Item(pKey)
    End Function

    ''' <summary>
    ''' Get the specified grid Icon image and load it in the grid images dictionary 
    ''' </summary>
    ''' <param name="pKey">Name of the grid Icon image to load</param>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012 
    ''' </remarks>
    Private Sub SetImageGridToDictionary(ByVal pKey As String)
        Dim preloadedDataConfig As New PreloadedMasterDataDelegate

        If (mImageGridDict.ContainsKey(pKey)) Then
            mImageGridDict.Item(pKey) = preloadedDataConfig.GetIconImage(pKey)
        Else
            mImageGridDict.Add(pKey, preloadedDataConfig.GetIconImage(pKey))
        End If
    End Sub

    ''' <summary>
    ''' Read the list of Analyzers for which there are Historic Patient Results in DB and load the global Analyzers list
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 28/09/2012
    ''' Modified by: IR 04/10/2012 - Let the user select more than one analyzer if available. We must read data from thisWSAnalyzerAlarmsDAO
    ''' </remarks>
    Private Sub GetAnalyzerList()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myAnalyzerDelegate As New AnalyzersDelegate

            myGlobalDataTO = myAnalyzerDelegate.GetDistinctAnalyzers(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myAnalyzerData As List(Of String) = DirectCast(myGlobalDataTO.SetDatos, List(Of String))

                For Each o As String In myAnalyzerData
                    mAnalyzers.Add(o.ToString)
                Next
            End If

            myGlobalDataTO = Nothing
            myAnalyzerDelegate = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetAnalyzerList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetAnalyzerList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load all the Sample Class availables (with its multilanguage text)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012
    ''' Modified by: SA 25/08/2014 - Added Try/Catch block
    ''' </remarks>
    Private Sub GetStatFlags()
        Try
            With mStatFlags
                .Clear()
                .Add(TriState.UseDefault, GetText("LBL_SRV_All"))  'All
                .Add(TriState.True, GetText("LBL_Stat"))           'Stat
                .Add(TriState.False, GetText("LBL_Routine"))       'Routine
            End With
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetStatFlags ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetStatFlags ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load all the Sample Type availables (with its multilanguage text)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012
    ''' Modified by: SA 25/08/2014 - Added Try/Catch block
    ''' </remarks>
    Private Sub GetSampleTypes()
        Try
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
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSampleTypes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetSampleTypes ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load all the Test Type availables (with its multilanguage text)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012
    ''' Modified by: SA 01/08/2014 - Load the ComboBox of Test Types from DB. Added Try/Catch block
    ''' </remarks>
    Private Sub GetTestTypes()
        Try
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim myMasterDataDelegate As New PreloadedMasterDataDelegate()

            myGlobalDataTO = myMasterDataDelegate.GetList(Nothing, PreloadedMasterDataEnum.TEST_TYPES)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myMasteDataDS As PreloadedMasterDataDS = DirectCast(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)

                'Sort the returned data 
                Dim qTestTypes As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow) = (From a As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myMasteDataDS.tfmwPreloadedMasterData _
                                                                                           Order By a.Position Select a).ToList()
                mTestTypes.Clear()
                For Each testTypeRow As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In qTestTypes
                    mTestTypes.Add(testTypeRow.ItemID.ToString(), "-" & testTypeRow.FixedItemDesc)
                Next
                qTestTypes = Nothing
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetTestTypes ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetTestTypes ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Private Methods to initialize the screen"
    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012
    ''' Modified by: SA 01/08/2014 - Get the multilanguage label for filter field SpecimenID (Barcode)
    '''                            - Changed the multilanguage label used for field PatientData
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            subtitleLabel.Text = GetText("LBL_HistResultsPatient")

            dateFromLabel.Text = GetText("LBL_Date_From") & ":"
            dateToLabel.Text = GetText("LBL_Date_To") & ":"

            bsPatientDataLabel.Text = GetText("LBL_Patient") & ":"
            bsSpecimenIDLabel.Text = GetText("MENU_BARCODE") & ":"
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
        Try
            Dim myToolTipsControl As New ToolTip

            'EXIT Button
            exitButton.Image = GetImage("CANCEL")
            myToolTipsControl.SetToolTip(exitButton, GetText("BTN_CloseScreen"))

            'SEARCH Button
            searchButton.Image = GetImage("FIND")
            myToolTipsControl.SetToolTip(searchButton, GetText("BTN_Search"))

            'DELETE Button
            historyDeleteButton.Image = GetImage("REMOVE")
            myToolTipsControl.SetToolTip(historyDeleteButton, GetText("BTN_Delete"))

            'EXPORT TO LIS Button
            exportButton.Image = GetImage("MANUAL_EXP")
            myToolTipsControl.SetToolTip(exportButton, GetText("BTN_Results_ManualExport"))

            'PRINT Button
            PrintButton.Image = GetImage("PRINT")
            myToolTipsControl.SetToolTip(PrintButton, GetText("BTN_Print"))

            'COMPACT PRINT Button
            CompactPrintButton.Image = GetImage("COMPACTPRINT")
            myToolTipsControl.SetToolTip(CompactPrintButton, GetText("PMD_CompactReport"))

            myToolTipsControl = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    '''  Fills the DropDownLists
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012
    ''' Modified by: TR 08/05/2013 - Take values from DB
    '''              XB 06/06/2013 - Added kvp.Key for the Sample Type description into the corresponding combo
    '''              SA 01/08/2014 - Added Try/Catch block
    ''' </remarks>
    Private Sub FillDropDownLists()
        Try
            GetStatFlags()
            GetSampleTypes()
            GetTestTypes()
            GetAnalyzerList()

            bsStatFlagComboBox.DataSource = mStatFlags.Values.ToList

            With sampleTypesChkComboBox.Properties.Items
                For Each kvp As KeyValuePair(Of String, String) In mSampleTypes
                    .Add(kvp.Key, kvp.Key + kvp.Value)
                Next
            End With

            With testTypesChkComboBox.Properties.Items
                For Each kvp As KeyValuePair(Of String, String) In mTestTypes
                    .Add(kvp.Key, kvp.Key + kvp.Value)
                Next
            End With

            bsAnalyzersComboBox.DataSource = mAnalyzers

            sampleTypesChkComboBox.Properties.SelectAllItemCaption = GetText("LBL_SRV_All")
            testTypesChkComboBox.Properties.SelectAllItemCaption = GetText("LBL_SRV_All")

            bsAnalyzersComboBox.Enabled = mAnalyzers.Count > 1
            bsAnalyzersComboBox.Visible = bsAnalyzersComboBox.Enabled
            analyzerIDLabel.Visible = bsAnalyzersComboBox.Visible
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FillDropDownLists ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FillDropDownLists ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
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

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by:  JB
    ''' Modified by: SA 25/08/2014 - Added Try/Catch block
    ''' </remarks>
    Private Sub DrawCheckBox(ByVal g As Graphics, ByVal r As Rectangle, ByVal Checked As Boolean)
        Try
            Dim edit As RepositoryItemCheckEdit = TryCast(historyGrid.RepositoryItems.Add("CheckEdit"), RepositoryItemCheckEdit)

            Dim info As DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo
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
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DrawCheckBox ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DrawCheckBox ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Creates and initializes the historyGrid
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 18/10/2012
    ''' Modified by AG 29/10/2012 - (AG + EF MEETING) Merge only by patientID, sort grid columns: Date, Patient, Type, TestName, RemarkAlert
    '''             XB 03/10/2013 - BT #1309 ==> Added TestPosition field to preserve the sorting configured on Reports Tests Sorting screen 
    '''             AG 14/02/2014 - BT #1505 ==> User can sort grid data also by column ExportStatus
    '''             SA 29/04/2014 - BT #1608 ==> Added a hidden column for field TestLongName (this field has to be shown as Test Name in reports
    '''                                          when it is informed)
    '''             SA 01/08/2014 - BT #1861 ==> Added new visible grid columns: SpecimenID (Barcode), Patient Last Name and Patient First Name.  
    '''                                          Added new hidden grid column> HistPatientID.
    '''                                          Call new function to get the saved width of all visible grid columns and assign the value.
    ''' </remarks>
    Private Sub InitializeResultHistoryGrid()
        Try
            '/**********************************************************************************************************************************/
            '/*                                          READ BEFORE CHANGE THIS FUNCTION!!                                                    */
            '/*                                                                                                                                */
            '/* Each time a Column is ADDED/DELETED from this Grid, table tcfgGridColsConfiguration for ScreenID = HIS001 have to be updated   */
            '/* to ADD/DELETE the row for the Column. When  property Name of a Column is changed, it is also needed to update ColumnName field */
            '/* in that table                                                                                                                  */
            '/**********************************************************************************************************************************/

            historyGridView.Columns.Clear()
            InitializeGridNavigator()

            Dim column As DevExpress.XtraGrid.Columns.GridColumn
            With historyGridView
                .OptionsView.AllowCellMerge = False
                .OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Default
                .OptionsView.ShowGroupedColumns = False
                .OptionsView.ColumnAutoWidth = False
                .OptionsView.RowAutoHeight = True
                .OptionsView.ShowIndicator = False

                .Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center

                .OptionsHint.ShowColumnHeaderHints = False
                .OptionsBehavior.Editable = False
                .OptionsBehavior.ReadOnly = True
                .OptionsCustomization.AllowFilter = False
                .OptionsCustomization.AllowSort = True
                .OptionsSelection.EnableAppearanceFocusedRow = False
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

            'SELECT ROW Column
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

            'RESULT DATETIME Column
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

            'SPECIMEN ID (Barcode) Column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("MENU_BARCODE")
                .FieldName = "SpecimenID"
                .Name = "SpecimenID"
                .Visible = True
                .Width = 95
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'PATIENT ID/SAMPLE ID Column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_PatientID")
                .FieldName = "PatientID"
                .Name = "PatientID"
                .Visible = True
                .Width = 95
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'PATIENT FAMILY NAME Column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_LastName")
                .FieldName = "LastName"
                .Name = "LastName"
                .Visible = True
                .Width = 95
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'PATIENT GIVEN NAME Column
            column = historyGridView.Columns.Add()
            With column
                .Caption = GetText("LBL_FirstName")
                .FieldName = "FirstName"
                .Name = "FirstName"
                .Visible = True
                .Width = 95
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
            End With

            'STAT FLAG ICON Column
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

            'SAMPLE TYPE Column
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

            'TEST NAME Column
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

            'BT #1608 - TEST LONG NAME Column (hidden, used to print reports)
            column = historyGridView.Columns.Add()
            With column
                .FieldName = "TestLongName"
                .Name = "TestLongName"
                .Visible = False
                .Width = 0
            End With

            'ALERT Column
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

            'RESULT CONCENTRATION Column
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

            'MEASURE UNIT Column
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

            'REFERENCE RANGES Column
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
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'EXPORTED TO LIS column
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
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
                .ColumnEdit = pictureEdit
            End With

            ''ABSORBANCE GRAPH column
            'column = historyGridView.Columns.Add()
            'With column
            '    .Caption = ""
            '    .FieldName = "GraphImage"
            '    .Name = "GraphImage"
            '    .Visible = False 'No V1
            '    .Width = 16
            '    .OptionsColumn.AllowSize = False
            '    .OptionsColumn.ShowCaption = False
            '    .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            '    .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
            '    .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            '    .ColumnEdit = pictureEdit
            'End With

            'REMARKS Column
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

            'ADDITIONAL INFORMATION Column
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

            'ANALYZER ID Column
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

            'BT #1309 - TEST POSITION Column (hidden, used when print reports)
            column = historyGridView.Columns.Add()
            With column
                .Caption = String.Empty
                .FieldName = "TestPosition"
                .Name = "TestPosition"
                .Visible = False
                .Width = 0
            End With

            'BT #1861 - HISTORIC PATIENT IDENTIFIER Column (hidden)
            column = historyGridView.Columns.Add()
            With column
                .Caption = String.Empty
                .FieldName = "HistPatientID"
                .Name = "HistPatientID"
                .Visible = False
                .Width = 0
            End With

            'BT #1861 - BACKCOLOR GROUP Column (hidden, used to set the Row BackColor in grid event RowStyle)
            column = historyGridView.Columns.Add()
            With column
                .Caption = String.Empty
                .FieldName = "BackColorGroup"
                .Name = "BackColorGroup"
                .Visible = False
                .Width = 0
            End With

            'BT #1861 - Get the saved width of all visible grid columns and assign the value
            Dim resultData As New GlobalDataTO
            Dim myColWidthConfigDelegate As New GridColsConfigurationDelegate

            resultData = myColWidthConfigDelegate.Read(Nothing, "HIS001", "historyGridView")
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                gridColWidthConfigDS = DirectCast(resultData.SetDatos, GridColsConfigDS)

                For Each row As GridColsConfigDS.tcfgGridColsConfigurationRow In gridColWidthConfigDS.tcfgGridColsConfiguration
                    historyGridView.Columns(row.ColumnName).Width = row.SavedWidth
                Next
            End If
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
            bsDateFromDateTimePick.Checked = True
            bsDateFromDateTimePick.Value = Today.AddDays(-1)

            dateToDateTimePick.Checked = True
            dateToDateTimePick.Value = Today

            bsStatFlagComboBox.SelectedIndex = 0

            If (bsAnalyzersComboBox.Items.Count > 0) Then
                bsAnalyzersComboBox.SelectedIndex = 0
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
    ''' Created by:  JB 18/10/2012
    ''' Modified by: AG 22/10/2012 - Added code to get the Alarms descriptions
    '''              SG 31/05/2013 - Added code to set the screen status according Level of the current User
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            GetScreenLabels()
            PrepareButtons()
            FillDropDownLists()

            'Get the alarms descriptions
            Dim resultData As New GlobalDataTO
            Dim alarmsDlg As New AlarmsDelegate

            resultData = alarmsDlg.ReadAll(Nothing)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                alarmsDefiniton = DirectCast(resultData.SetDatos, AlarmsDS)
            End If

            'Initialize the grid
            InitializeResultHistoryGrid()

            'Set the default search criteria to controls in Filter area
            InitializeFilterSearch()

            'Get data to shown in the grid according the default search criteria
            FindHistoricalResults()

            'Get Level of the connected User and set the screen status according the Level of the current User
            Dim myGlobalBase As New GlobalBase
            CurrentUserLevel = myGlobalBase.GetSessionInfo().UserLevel
            ScreenStatusByUserLevel()

            'Set declared variables to Nothing 
            myGlobalBase = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Private Methods for searching Historic Patient Results and load the grid"
    ''' <summary>
    ''' Fill a SearchFilter structure with filters selected in Search Area
    ''' </summary>
    ''' <returns>SearchFilter structure filled with filters selected in Search Area</returns>
    ''' <remarks>
    ''' Created by:  JB
    ''' Modified by: SA 01/08/2014 - BT #1861 ==> Inform SpecimenID in the structure with value of the corresponding filter.
    '''                                           Added Try/Catch block
    ''' </remarks>
    Private Function GetSearchFilter() As SearchFilter
        Dim filter As New SearchFilter
        With filter
            .analyzerId = bsAnalyzersComboBox.SelectedItem.ToString

            If (bsDateFromDateTimePick.Checked) Then
                .dateFrom = bsDateFromDateTimePick.Value
            Else
                .dateFrom = Nothing
            End If

            If (dateToDateTimePick.Checked) Then
                .dateTo = dateToDateTimePick.Value.AddDays(1)
            Else
                .dateTo = Nothing
            End If

            .patientData = bsPatientDataTextBox.Text
            .specimenID = bsSpecimenIDTextBox.Text

            .statFlag = (From kvp As KeyValuePair(Of TriState, String) In mStatFlags _
                        Where kvp.Value = bsStatFlagComboBox.SelectedItem.ToString _
                       Select kvp.Key).FirstOrDefault

            .sampleTypes = New List(Of String)
            For Each elem As DevExpress.XtraEditors.Controls.CheckedListBoxItem In sampleTypesChkComboBox.Properties.Items
                If (elem.CheckState = CheckState.Checked) Then .sampleTypes.Add(elem.Value.ToString)
            Next

            .testTypes = New List(Of String)
            For Each elem As DevExpress.XtraEditors.Controls.CheckedListBoxItem In testTypesChkComboBox.Properties.Items
                If (elem.CheckState = CheckState.Checked) Then .testTypes.Add(elem.Value.ToString)
            Next

            .testName = bsTestNameTextBox.Text
        End With
        Return filter
    End Function

    ''' <summary>
    ''' Find the Results by the selected filter in screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 18/10/2012
    ''' Modified by: AG 13/02/2014 - BT #1505 ==> Added traces in the LOG 
    '''              SA 01/08/2014 - BT #1861 ==> Changed the call to function GetHistoricalResultsByFilter due to its parameters have been changed
    ''' </remarks>
    Private Sub FindHistoricalResults()
        Dim myGlobalDataTO As New GlobalDataTO
        Dim myHisWSResultsDS As HisWSResultsDS

        historyGrid.DataSource = Nothing
        If (bsAnalyzersComboBox.Items.Count = 0) Then Exit Sub

        Try
            UpdateFormBehavior(False)

            Me.Cursor = Cursors.WaitCursor
            Dim StartTime As DateTime = Now 'AG 13/02/2014 - #1505

            'Get all informed filters
            Dim filter As SearchFilter = GetSearchFilter()
            With filter
                'AG 14/02/2014 - #1505
                Dim valueDateTo As Date = filter.dateTo
                Dim diffDays As TimeSpan = valueDateTo.Subtract(filter.dateFrom)
                CreateLogActivity("Days number to find: " & diffDays.Days.ToString, Me.Name & ".FindHistoricalResults", EventLogEntryType.Information, False)
                'AG 14/02/2014 - #1505

                myGlobalDataTO = myHisWSResultsDelegate.GetHistoricalResultsByFilter(Nothing, .analyzerId, .dateFrom, .dateTo, .patientData, .sampleTypes, _
                                                                                     .statFlag, .testTypes, .testName, String.Empty, .specimenID)
            End With

            CreateLogActivity("GetHistoricalResultsByFilter: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Me.Name & ".FindHistoricalResults", _
                              EventLogEntryType.Information, False) 'AG 13/02/2014 - #1505

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                myHisWSResultsDS = DirectCast(myGlobalDataTO.SetDatos, HisWSResultsDS)

                PrepareAndSetDataToGrid(myHisWSResultsDS.vhisWSResults)
                'AG 13/02/2014 - #1505 NOTE: Do not add trace, it has already been added into method PrepareAndSetDataToGrid
            ElseIf (myGlobalDataTO.HasError) Then
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
    ''' <remarks>
    ''' Created by:  JB 19/10/2012 
    ''' Modified by: SA 01/08/2014 - Do not load the Icon for the ABS/Time Graph: the column is not visible
    ''' </remarks>
    Private Sub DecodeRowImages(ByRef pRow As HisWSResultsDS.vhisWSResultsRow)
        Try
            With pRow
                If (.StatFlag) Then
                    .StatFlagImage = GetImageGrid("STATS")
                Else
                    .StatFlagImage = GetImageGrid("ROUTINES")
                End If
                If (.ExportStatus = "SENT") Then
                    .ExportImage = GetImageGrid("EXPORTHEAD")
                End If

                'If (.TestType = "STD") Then
                '    .GraphImage = GetImageGrid("AVG_ABS_GRAPH")
                'End If
            End With
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DecodeRowImages ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DecodeRowImages ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Decodes the row texts
    ''' </summary>
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
    ''' Set value of field BackColorGroup for the Result row beign processed: if the Result is for the same HistPatientID/PatientID/SpecimenID than the 
    ''' previous Result row, the same BackColor is assigned; otherwise, the alternate BackColor value is assigned
    ''' </summary>
    ''' <param name="pRow">Result row in process</param>
    ''' <param name="pPreviousBackColorGroup">BackColorGroup assigned to the previous Result Row in the DataSet</param>
    ''' <param name="pPreviousSpecimenID">SpecimenID in the previous Result Row in the DataSet</param>
    ''' <param name="pPreviousPatientID">PatientID in the previous Result Row in the DataSet</param>
    ''' <param name="pPreviousHistPatientID">HistPatientID in the previous Result Row in the DataSet. It is an optional parameter due to if the
    '''                                      previous Result Row corresponds to an unknown Patient, HistPatientID is NULL in the DataSet</param>
    ''' <remarks>
    ''' Created by: SA 25/08/2014 - BT #1861
    ''' </remarks>
    Private Sub SetRowBackColorGroup(ByRef pRow As HisWSResultsDS.vhisWSResultsRow, ByVal pPreviousBackColorGroup As Integer, ByVal pPreviousSpecimenID As String, _
                                     ByVal pPreviousPatientID As String, Optional ByVal pPreviousHistPatientID As Integer = 0)
        Try
            Dim changeRowBC As Boolean = True

            If (pRow.SpecimenID = pPreviousSpecimenID AndAlso pRow.PatientID = pPreviousPatientID) Then
                If (pRow.IsHistPatientIDNull AndAlso pPreviousHistPatientID = 0) OrElse _
                   (Not pRow.IsHistPatientIDNull AndAlso pPreviousHistPatientID <> 0 AndAlso pRow.HistPatientID = pPreviousHistPatientID) Then
                    pRow.BackColorGroup = pPreviousBackColorGroup
                    changeRowBC = False
                End If
            End If

            If (changeRowBC) Then
                If (pPreviousBackColorGroup = 1) Then
                    pRow.BackColorGroup = 2
                Else
                    pRow.BackColorGroup = 1
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SetRowBackColorGroup ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SetRowBackColorGroup ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the DataSet with friendly data and sets to Grid
    ''' </summary>
    ''' <param name="pHisResultsDataTable">Typed DataSet HisWSResultsDS containing all data to load in the grid</param>
    ''' <remarks>
    ''' Created by:  JB 19/10/2012
    ''' Modified by: JV 03/01/2014 - BT #1285 ==> Removed Application.DoEvents from the For/Next to avoid flicking on the DataGrid area 
    '''                                           (data, images, etc.) and also on the form
    '''              AG 13/02/2014 - BT #1505 ==> Added traces in the LOG 
    '''              SA 25/08/2014 - BT #1861 ==> For each processed row, set value of field BackColorGroup: all results for the same PatientID/SpecimenID
    '''                                           will have the same BackColor, and there will be an alternate color for the different pairs of 
    '''                                           PatientID/SpecimenID loaded
    ''' </remarks>
    Private Sub PrepareAndSetDataToGrid(ByVal pHisResultsDataTable As HisWSResultsDS.vhisWSResultsDataTable)
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            UpdateFormBehavior(False)
            Dim StartTime As DateTime = Now 'AG 13/02/2014 - #1505

            Dim i As Integer = 0
            Dim myPrevBackColor As Integer = 1
            Dim myPrevPatientID As String = String.Empty
            Dim myPrevHistPatientID As Integer = 0
            Dim myPrevSpecimenID As String = String.Empty

            For Each row As HisWSResultsDS.vhisWSResultsRow In pHisResultsDataTable
                'Mark the row as "not selected"
                row.Selected = False

                'Get the images
                DecodeRowImages(row)

                'Get the texts
                myGlobalDataTO = DecodeRowTexts(row)
                If (myGlobalDataTO.HasError) Then Exit For

                'BT #1861 - Set the group BackColor for each grid row 
                If (i = 0) Then
                    SetRowBackColorGroup(row, 1, row.SpecimenID, row.PatientID, 0)
                Else
                    SetRowBackColorGroup(row, myPrevBackColor, myPrevSpecimenID, myPrevPatientID, myPrevHistPatientID)
                End If

                i += 1
                myPrevBackColor = row.BackColorGroup
                myPrevPatientID = row.PatientID
                myPrevSpecimenID = row.SpecimenID
                If (Not row.IsHistPatientIDNull) Then myPrevHistPatientID = row.HistPatientID Else myPrevHistPatientID = 0
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

#Region "Private Methods for deleting selected Historic Patient Results"
    ''' <summary>
    ''' Delete all selected rows in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 19/10/2012
    ''' Modified by: SG 02/07/2013 - Changes to use the new function for delete Historic Patient Results; the previous one was wrong because
    '''                              it deletes only the result from table thisWSResults, but not the rest of data
    '''              AG 13/02/2014 - BT #1505 ==> Added traces into the LOG 
    ''' </remarks>
    Private Sub DeleteSelectedRowsFromGrid(ByVal pGrid As DevExpress.XtraGrid.Views.Grid.GridView)
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            If (ShowMessage(Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) = Windows.Forms.DialogResult.Yes) Then
                Dim StartTime As DateTime = Now

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
                CreateLogActivity("DeleteSelectedRowsFromGrid: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), Name & ".DeleteSelectedRowsFromGrid", EventLogEntryType.Information, False) 'AG 13/02/2014 - #1505
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteSelectedRowsFromGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        EnableScreen(True)
        IAx00MainMDI.EnableButtonAndMenus(True)
        Cursor = Cursors.Default
        UpdateFormBehavior(True)
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
    ''' Get the list of selected Historic Patient Results and load them in a HisWSOrderTestsDS. Used for DELETE process
    ''' </summary>
    ''' <returns>Typed DataSet HisWSOrderTestsDS containing all Historic Patient Results selected for delete</returns>
    ''' <remarks>
    ''' Created by: SA 
    ''' </remarks>
    Private Function GetSelectedRowsNEW() As HisWSOrderTestsDS
        Dim hisWSOrderTestsDS As New HisWSOrderTestsDS

        Try
            Dim dataTable As HisWSResultsDS.vhisWSResultsDataTable = DirectCast(historyGrid.DataSource, HisWSResultsDS.vhisWSResultsDataTable)
            If (Not dataTable Is Nothing) Then
                Dim lstSelectedResults As List(Of HisWSResultsDS.vhisWSResultsRow) = (From row In dataTable Where row.Selected).ToList

                Dim hisWSOTRow As HisWSOrderTestsDS.thisWSOrderTestsRow
                For Each row As HisWSResultsDS.vhisWSResultsRow In lstSelectedResults
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
            ShowMessage(Name & ".GetSelectedRowsNEW ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return hisWSOrderTestsDS
    End Function
#End Region

#Region "Private Methods for exporting to LIS selected Historic Patient Results"
    ''' <summary>
    ''' Export all selected rows in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 22/10/2012
    ''' Modified by: SA 17/12/2012 - If an error has happened when expoting, shown it 
    '''              DL 24/04/2013 - 
    '''              AG 13/02/2014 - BT #1505 ==> Added code to evaluate if the number of Historic Patient Results selected to Export to LIS  
    '''                                           is lower than the allowed limit. Added traces in the Application Log. Set to Nothing all declared lists 
    '''                                           to release memory
    '''              AG 24/07/2014 - BT #1886 (RQ00086 v3.1.0) ==> Allow re-sent patient results from history
    ''' </remarks>
    Private Sub ExportSelectedRowsFromGrid(ByVal pGrid As DevExpress.XtraGrid.Views.Grid.GridView)
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            Dim StartTime As DateTime = Now

            Dim selectedRows As List(Of HisWSResultsDS.vhisWSResultsRow) = GetSelectedRows()
            If (selectedRows.Count = 0) Then Exit Sub

            'BT #1505 - Evaluate if the number of Historic Patient Results selected to Export to LIS is lower than the allowed limit
            Dim maxHistResultsToExport As Integer = 100 'Default value..

            Dim swParamDlg As New SwParametersDelegate
            myGlobalDataTO = swParamDlg.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_RESULTSTOEXPORT_HIST.ToString, Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myDS As ParametersDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)

                If (myDS.tfmwSwParameters.Rows.Count = 0) Then
                    'Do nothing, in v300 initial database contains this parameter
                Else
                    If (Not myDS.tfmwSwParameters(0).IsValueNumericNull) Then maxHistResultsToExport = CInt(myDS.tfmwSwParameters(0).ValueNumeric)
                End If
            End If

            'Show export confirmation message ??
            'If (ShowMessage(Name & ".ExportSelectedRowsFromGrid ", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) <> Windows.Forms.DialogResult.Yes) Then Exit Sub

            'AG 24/07/2014 - #1886 - RQ00086
            ''BEGIN DL 24/04/2013 - The results SENT or SENDING could not be uploaded again from Historical results screen 
            'Dim histOrderTestIDList As List(Of Integer) = (From row In selectedRows
            '                                              Where row.ExportStatus <> "SENT" AndAlso row.ExportStatus <> "SENDING" _
            '                                             Select row.HistOrderTestID Distinct).ToList

            'Convert dataset to LIST of historic order test integers
            Dim histOrderTestIDList As List(Of Integer) = (From row In selectedRows Select row.HistOrderTestID Distinct).ToList
            'AG 24/07/2014

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

            'For testing LIS History Upload
            If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing Then
                'Get the results exported (ExecutionsDS) from GlobalDataToReturned
                Dim myExportedExecutionsDS As ExecutionsDS = TryCast(myGlobalDataTO.SetDatos, ExecutionsDS)
                Dim myHisWSResultsDS As HisWSResultsDS = New HisWSResultsDS

                'AG 24/07/2014 - #1886 - RQ00086 v310 Export all selected results again (not only those not SENT)
                'AG 14/02/2014 - #1505 do not create DS using selectedRows, add only those results to export!!!
                'Create a new HisWSResultsDS (data table vhisWSResults) using the list of selected rows
                For Each HisWSResultsRow As HisWSResultsDS.vhisWSResultsRow In selectedRows
                    myHisWSResultsDS.vhisWSResults.ImportRow(HisWSResultsRow)
                Next
                'For Each HisWSResultsRow As HisWSResultsDS.vhisWSResultsRow In (From A In selectedRows Where A.ExportStatus <> "SENT" AndAlso A.ExportStatus <> "SENDING" Select A).ToList
                '    If Not HisWSResultsRow.IsHistOrderTestIDNull AndAlso histOrderTestIDList.Contains(HisWSResultsRow.HistOrderTestID) Then
                '        myHisWSResultsDS.vhisWSResults.ImportRow(HisWSResultsRow)
                '    End If
                'Next
                'AG 24/07/2014
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
#End Region

#Region "Private Methods for screen behavior"
    ''' <summary>
    ''' Apply rights by User Level
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 31/05/2013
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
    ''' Created by:  JB 18/10/2012
    ''' Modified by: DL 25/04/2013 - New condition for activate export status when any result NOT EXPORTED
    '''              SG 31/05/2013 - Added code to set the screen status according Level of the current User
    '''              AG 14/02/2014 - BT #1505 ==> Set to Nothing all declared Lists to release memory
    '''              AG 24/07/2014 - BT #1886 (RQ00086 v3.1.0) ==> Allow re-sent patient results from history
    ''' </remarks>
    Private Sub UpdateFormBehavior(ByVal pStatus As Boolean)
        Try
            searchGroup.Enabled = pStatus
            exitButton.Enabled = pStatus

            Dim selectedRows As List(Of HisWSResultsDS.vhisWSResultsRow) = GetSelectedRows()
            historyDeleteButton.Enabled = (pStatus AndAlso selectedRows.Count > 0)

            'AG 24/07/2014 - #1886 - RQ00086
            'Dim selectedNotExport As Integer = (From row In selectedRows Where row.ExportStatus = "NOTSENT").Count
            'exportButton.Enabled = pStatus AndAlso selectedRows.Count > 0 AndAlso selectedNotExport > 0
            exportButton.Enabled = pStatus AndAlso selectedRows.Count > 0
            'AG 24/07/2014

            PrintButton.Enabled = pStatus AndAlso selectedRows.Count > 0
            searchGroup.Enabled = pStatus AndAlso bsAnalyzersComboBox.Items.Count > 0
            CompactPrintButton.Enabled = pStatus AndAlso selectedRows.Count > 0

            ScreenStatusByUserLevel()
            selectedRows = Nothing '
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateFormBehavior ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpdateFormBehavior ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Enables/Disables screen buttons
    ''' </summary>
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
    ''' TODO!!!
    ''' Open the Graph Screen with the result data
    ''' </summary>
    ''' <remarks>
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

    ''' <summary>
    ''' Save the current width of all visible grid columns in table tcfgGridColsConfiguration
    ''' </summary>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by: SA 25/08/2014 - BT #1861
    ''' </remarks>
    Private Function SaveGridColumnsWidth() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            'Load the current width of all visible column grids in a typed DataSet GridColsConfigDS
            Dim myGridColumnsDS As New GridColsConfigDS
            Dim myGridColumnsRow As GridColsConfigDS.tcfgGridColsConfigurationRow

            For Each column As DevExpress.XtraGrid.Columns.GridColumn In historyGridView.Columns
                If (column.Visible) Then
                    myGridColumnsRow = myGridColumnsDS.tcfgGridColsConfiguration.NewtcfgGridColsConfigurationRow()
                    myGridColumnsRow.ScreenID = "HIS001"
                    myGridColumnsRow.GridName = "historyGridView"
                    myGridColumnsRow.ColumnName = column.Name
                    myGridColumnsRow.SavedWidth = column.VisibleWidth
                    myGridColumnsDS.tcfgGridColsConfiguration.AddtcfgGridColsConfigurationRow(myGridColumnsRow)
                End If
            Next
            myGridColumnsDS.AcceptChanges()

            'Save the width of all visible columns 
            Dim myGridColsConfigDelegate As New GridColsConfigurationDelegate
            myGlobalDataTO = myGridColsConfigDelegate.Update(Nothing, myGridColumnsDS)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SaveGridColumnsWidth ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SaveGridColumnsWidth ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Code executed when the Click Event of Exit Button is raised
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SA 25/08/2014 - BT #1861 ==> Code moved from the button click event. Added call to function SaveGridColumnsWidth to save the 
    '''                                           current width of all visible grid columns to be used the next time the screen is loaded 
    ''' </remarks>
    Private Sub ExitScreen()
        Try
            'Save the current width of all visible grid columns to be used the next time the screen is loaded 
            Dim myGlobalDataTO As GlobalDataTO = SaveGridColumnsWidth()
            If (myGlobalDataTO.HasError) Then
                ShowMessage(Name & ".ExitScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString())
            End If

            'AG 24/02/2014 - Insert parameter MAX_APP_MEMORYUSAGE into Performance Counters 
            'XB 18/02/2014 - BT #1499 ==> Do not shown message here
            Dim pCounters As New AXPerformanceCounters(applicationMaxMemoryUsage, SQLMaxMemoryUsage)
            pCounters.GetAllCounters()
            pCounters = Nothing

            If (Not Tag Is Nothing) Then
                'A PerformClick() method was executed
                Close()
            Else
                'Normal button click - Open the WS Monitor form and close this one
                IAx00MainMDI.OpenMonitorForm(Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get the selected rows in the grid. Used for processes of Export to LIS and Print, and also for grid events
    ''' </summary>
    ''' <returns>List of rows of a typed DataSet HisWSResultsDS containing all Historic Patient Results selected for Export to LIS or to Print</returns>
    ''' <remarks>
    ''' Created by:  JB 24/10/2012
    ''' </remarks>
    Protected Overridable Function GetSelectedRows() As List(Of HisWSResultsDS.vhisWSResultsRow)
        Try
            Dim dataTable As HisWSResultsDS.vhisWSResultsDataTable = DirectCast(historyGrid.DataSource, HisWSResultsDS.vhisWSResultsDataTable)
            If (dataTable IsNot Nothing) Then Return (From row In dataTable Where row.Selected).ToList
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetSelectedRows ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetSelectedRows ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return New List(Of HisWSResultsDS.vhisWSResultsRow)
    End Function

    ''' <summary>
    ''' Select all rows in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 24/10/2012
    ''' </remarks>
    Private Sub SelectAllRows(ByVal selection As Boolean)
        Try
            UpdateFormBehavior(False)

            Dim dataTable As HisWSResultsDS.vhisWSResultsDataTable = DirectCast(historyGrid.DataSource, HisWSResultsDS.vhisWSResultsDataTable)
            If (dataTable IsNot Nothing) Then
                For Each row As HisWSResultsDS.vhisWSResultsRow In dataTable
                    row.Selected = selection
                Next
            End If
        Catch ex As Exception
            Cursor = Cursors.Default

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SelectAllRows ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".SelectAllRows ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            UpdateFormBehavior(True)
        End Try
    End Sub
#End Region

#Region "Screen Events"
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

#Region "Button Events"
    ''' <summary>
    ''' Exit Button click event: save the current width of visible grid columns and close the screen
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB
    ''' Modified by: SA 25/08/2014 - BT #1861 ==> Code moved to new function ExitScreen
    ''' </remarks>
    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitButton.Click
        Try
            ExitScreen()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Search Results click event: search all Historic Patient Results that fullfil the specified search criteria
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB
    ''' Modified by: XB 14/02/2014 - BT #1495 ==> Register Historic screen user actions into the Application Log
    '''              SA 25/08/2014 - BT #1861 ==> Added Try/Catch block
    ''' </remarks>
    Private Sub bsSearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles searchButton.Click
        Try
            'BT #1495 - Register SEARCH action into the Application Log
            CreateLogActivity("User press 'Search' Button", Name & ".bsSearchButton_Click ", EventLogEntryType.Information, False)

            'Get the Historic Patient Results that fullfil the specified criteria
            FindHistoricalResults()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsSearchButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsSearchButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete Results click event: execute the process to delete all selected Historic Patient Results and related data 
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB
    ''' Modified by: XB 14/02/2014 - BT #1495 ==> Register Historic screen user actions into the Application Log
    '''              SA 25/08/2014 - BT #1861 ==> Added Try/Catch block
    ''' </remarks>
    Private Sub bsHistoryDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles historyDeleteButton.Click
        Try
            'BT #1495 - Register DELETE action into the Application Log
            CreateLogActivity("User press 'Delete' Button", Name & ".bsHistoryDelete_Click ", EventLogEntryType.Information, False)

            'Execute the process of deleting selected all Historic Patient Results
            DeleteSelectedRowsFromGrid(historyGridView)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsHistoryDelete_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsHistoryDelete_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Export to LIS click event: execute the process to export to LIS all selected Historic Patient Results
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB
    ''' Modified by: XB 14/02/2014 - BT #1495 ==> Register Historic screen user actions into the Application Log
    '''              SA 25/08/2014 - BT #1861 ==> Added Try/Catch block
    ''' </remarks>
    Private Sub ExportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exportButton.Click
        Try
            'BT #1495 - Register EXPORT TO LIS action into the Application Log
            CreateLogActivity("User press 'Export' Button", Name & ".ExportButton_Click ", EventLogEntryType.Information, False)

            'Execute the process of exporting to LIS all selected Historic Patient Results
            ExportSelectedRowsFromGrid(historyGridView)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExportButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExportButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prints Historic Results by Patient report
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 26/10/2012
    ''' Modified by XB 03/10/2013 - BT #1309 ==> Added TestPosition field to preserve the sorting configured on Reports Tests Sorting screen 
    '''             AG 14/02/2014 - BT #1505 ==> Set to Nothing all declared Lists to release memory
    ''' </remarks>
    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintButton.Click
        Try
            Dim StartTime As DateTime = Now                         '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim myLogAcciones As New ApplicationLogManager()        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim myHisWSResults As List(Of HisWSResultsDS.vhisWSResultsRow)

            myHisWSResults = GetSelectedRows()
            sampleTypesChkComboBox.Properties.TextEditStyle = TextEditStyles.DisableTextEditor

            If (myHisWSResults.Count > 0) Then
                'BT #1309 - Sort data to print by TestPosition field
                Dim myHisWSResultsSorted As List(Of HisWSResultsDS.vhisWSResultsRow) = (From a As HisWSResultsDS.vhisWSResultsRow In myHisWSResults _
                                                                                    Order By a.TestPosition _
                                                                                      Select a).ToList

                myHisWSResults.Clear()
                XRManager.ShowHistoricResultsByPatientSampleReport(myHisWSResultsSorted)
                myHisWSResultsSorted = Nothing
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("Historic Patient Results Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IHisResults.bsPrintButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            myHisWSResults = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsPrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsPrintButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Prints Historic Patient Results Compact report
    ''' </summary>
    ''' <remarks>
    ''' Created by: 
    ''' Modified by: XB 03/10/2013 - BT #1309 ==> Added TestPosition field to preserve the sorting configured on Reports Tests Sorting screen 
    '''              AG 14/02/2014 - BT #1505 ==> Set to Nothing all declared Lists to release memory 
    ''' </remarks>
    Private Sub CompactPrintButton_Click(sender As Object, e As EventArgs) Handles CompactPrintButton.Click
        Try
            Dim StartTime As DateTime = Now                         '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim myLogAcciones As New ApplicationLogManager()        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim myHisWSResults As List(Of HisWSResultsDS.vhisWSResultsRow)

            myHisWSResults = GetSelectedRows()
            sampleTypesChkComboBox.Properties.TextEditStyle = TextEditStyles.DisableTextEditor

            If (myHisWSResults.Count > 0) Then
                'BT #1309 - Sort data to print by TestPosition field
                Dim myHisWSResultsSorted As List(Of HisWSResultsDS.vhisWSResultsRow) = (From a As HisWSResultsDS.vhisWSResultsRow In myHisWSResults _
                                                                                    Order By a.TestPosition _
                                                                                      Select a).ToList
                myHisWSResults.Clear()
                XRManager.ShowHistoricByCompactPatientsSamplesResult(myHisWSResultsSorted)
                myHisWSResultsSorted = Nothing
            End If

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            myLogAcciones.CreateLogActivity("Historic Patient Results Compact Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IHisResults.CompactPrintButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            myHisWSResults = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".CompactPrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".CompactPrintButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region

#Region "DateTimePicker Events"
    Private Sub bsDateFromDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDateFromDateTimePick.ValueChanged
        Try
            If (bsDateFromDateTimePick.Checked) Then
                dateToDateTimePick.MinDate = bsDateFromDateTimePick.Value
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
            If (dateToDateTimePick.Checked) Then
                bsDateFromDateTimePick.MaxDate = dateToDateTimePick.Value
            Else
                bsDateFromDateTimePick.MaxDate = Today.AddYears(100)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsDateToDateTimePick_ValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsDateToDateTimePick_ValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "CheckComboBox Events"
    Private Sub sampleTypeChkComboBox_CustomDisplayText(ByVal sender As System.Object, ByVal e As DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs) Handles sampleTypesChkComboBox.CustomDisplayText
        Try
            Dim numChecks As Integer = 0
            Dim text As String = String.Empty

            Dim edit As CheckedComboBoxEdit = TryCast(sender, CheckedComboBoxEdit)
            For Each item As CheckedListBoxItem In edit.Properties.Items
                If (item.CheckState = CheckState.Checked) Then
                    numChecks += 1

                    If (Not String.IsNullOrEmpty(text)) Then text &= ", "
                    text &= item.Value.ToString
                End If
            Next item

            If (numChecks = 0 OrElse numChecks = edit.Properties.Items.Count) Then text = GetText("LBL_SRV_All")
            e.DisplayText = text
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".sampleTypeChkComboBox_CustomDisplayText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".sampleTypeChkComboBox_CustomDisplayText ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub testTypeChkComboBox_CustomDisplayText(ByVal sender As System.Object, ByVal e As DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs) Handles testTypesChkComboBox.CustomDisplayText
        Try
            Dim numChecks As Integer = 0

            Dim edit As CheckedComboBoxEdit = TryCast(sender, CheckedComboBoxEdit)
            For Each item As CheckedListBoxItem In edit.Properties.Items
                If (item.CheckState = CheckState.Checked) Then numChecks += 1
            Next item

            If (numChecks = 0 OrElse numChecks = edit.Properties.Items.Count) Then e.DisplayText = GetText("LBL_SRV_All")
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".testTypeChkComboBox_CustomDisplayText ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".testTypeChkComboBox_CustomDisplayText ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Grid Events"

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by:  JB 23/10/2012 
    ''' </remarks>
    Private Sub xtraHistoryGridView_SelectionChanged(ByVal sender As System.Object, ByVal e As DevExpress.Data.SelectionChangedEventArgs) Handles historyGridView.SelectionChanged
        Try
            UpdateFormBehavior(True)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".xtraHistoryGridView_SelectionChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".xtraHistoryGridView_SelectionChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by:  JB 23/10/2012 
    ''' </remarks>
    Private Sub historyGridView_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles historyGridView.MouseMove
        Try
            With historyGridView.CalcHitInfo(New Point(e.X, e.Y))
                If (.InColumnPanel AndAlso .Column IsNot Nothing AndAlso .Column.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True) Then
                    Me.Cursor = Cursors.Hand
                ElseIf (.InRowCell AndAlso .Column IsNot Nothing AndAlso .Column.Name = "GraphImage") Then
                    Me.Cursor = Cursors.Hand
                Else
                    Me.Cursor = Cursors.Default
                End If
            End With
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".historyGridView_MouseMove ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".historyGridView_MouseMove", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Processes the mouse click event over the cells (history grid)
    ''' </summary>
    ''' <remarks>
    ''' Created by:  AG 23/10/2012
    ''' </remarks>
    Private Sub historyGridView_RowCellClick(ByVal sender As System.Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs) Handles historyGridView.RowCellClick
        Try
            UpdateFormBehavior(False)

            Cursor = Cursors.WaitCursor
            If (sender Is Nothing) Then Return
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
            ShowMessage(Name & ".historyGridView_RowCellClick ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Cursor = Cursors.Default
            UpdateFormBehavior(True)
        End Try
    End Sub

    ''' <summary>
    ''' Processes the CustomDrawColumnHeader event
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 24/10/2012
    ''' Modified by: AG 14/02/2014 - BT #1505 ==> Set declared lists to Nothing to release memory 
    ''' </remarks>
    Private Sub historyGridView_CustomDrawColumnHeader(ByVal sender As System.Object, ByVal e As DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs) Handles historyGridView.CustomDrawColumnHeader
        Try
            If (e.Column Is Nothing) Then Exit Sub
            If (e.Column.Name = "Selected") Then
                Dim selectedRows As List(Of HisWSResultsDS.vhisWSResultsRow) = GetSelectedRows()

                e.Info.InnerElements.Clear()
                e.Painter.DrawObject(e.Info)
                DrawCheckBox(e.Graphics, e.Bounds, selectedRows.Count > 0 AndAlso selectedRows.Count = historyGridView.DataRowCount)
                e.Handled = True

                selectedRows = Nothing
            End If
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".historyGridView_CustomDrawColumnHeader ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".historyGridView_CustomDrawColumnHeader ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Processes the Click event in the GridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 24/10/2012
    ''' Modified by: AG 14/02/2014 - BT #1505 ==> Set declared lists to Nothing to release memory 
    ''' </remarks>
    Private Sub historyGridView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles historyGridView.Click
        Try
            With historyGridView.CalcHitInfo(historyGrid.PointToClient(MousePosition))
                If (.Column IsNot Nothing AndAlso .Column.Name = "Selected" AndAlso .InColumn) Then
                    Dim selectedRows As List(Of HisWSResultsDS.vhisWSResultsRow) = GetSelectedRows()

                    If (historyGridView.DataRowCount > 0) Then
                        SelectAllRows(selectedRows.Count <> historyGridView.DataRowCount)
                    End If

                    selectedRows = Nothing
                End If
            End With
        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".historyGridView_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".historyGridView_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Set the BackColor of each grid Row according value of field BackColorGroup in the DS used as DataSource for the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 25/08/2014 - BT #1861
    ''' </remarks>
    Private Sub historyGridView_RowStyle(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs) Handles historyGridView.RowStyle
        Try
            Dim myGridView As DevExpress.XtraGrid.Views.Grid.GridView = DirectCast(sender, DevExpress.XtraGrid.Views.Grid.GridView)

            If (e.RowHandle >= 0) Then
                If (Convert.ToInt32(myGridView.GetRowCellDisplayText(e.RowHandle, myGridView.Columns("BackColorGroup"))) = 1) Then
                    e.Appearance.BackColor = Color.AntiqueWhite
                    e.Appearance.BackColor2 = Color.AntiqueWhite
                Else
                    e.Appearance.BackColor = Color.White
                    e.Appearance.BackColor2 = Color.White
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".historyGridView_RowStyle ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".historyGridView_RowStyle ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' When the grid is sorted for a Column different of PatientID and/or SpecimenID, the Back Color of colored rows is changed to White; otherwise
    ''' function SetRowBackColorGroup is called to set for each different HistPatientID/PatientID/SpecimenID the proper BackColorGroup
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 27/08/2014 - BT #1861
    ''' </remarks>
    Private Sub historyGridView_MouseDown(sender As Object, e As MouseEventArgs) Handles historyGridView.MouseDown
        Try
            Dim dataTable As HisWSResultsDS.vhisWSResultsDataTable = DirectCast(historyGrid.DataSource, HisWSResultsDS.vhisWSResultsDataTable)
            If (Not dataTable Is Nothing) Then
                With historyGridView.CalcHitInfo(New Point(e.X, e.Y))
                    If (.InColumnPanel AndAlso Not .Column Is Nothing) Then
                        Dim colName As String = .Column.Name
                        If (colName = "ResultDateTime") OrElse (colName = "LastName") OrElse (colName = "FirstName") OrElse (colName = "SampleType") OrElse _
                           (colName = "TestName") OrElse (colName = "RemarkAlert") OrElse (colName = "ExportImage") Then

                            Dim lstColoredRows As List(Of HisWSResultsDS.vhisWSResultsRow) = (From row In dataTable Where row.BackColorGroup = 1).ToList
                            For Each coloredRow As HisWSResultsDS.vhisWSResultsRow In lstColoredRows
                                coloredRow.BackColorGroup = 2 'Set BackColor to White
                            Next

                        ElseIf (colName = "SpecimenID") OrElse (colName = "PatientID") Then
                            Dim i As Integer = 0
                            Dim myPrevBackColor As Integer = 1
                            Dim myPrevPatientID As String = String.Empty
                            Dim myPrevHistPatientID As Integer = 0
                            Dim myPrevSpecimenID As String = String.Empty

                            For Each row As HisWSResultsDS.vhisWSResultsRow In dataTable
                                'Set the group BackColor for each grid row 
                                If (i = 0) Then
                                    SetRowBackColorGroup(row, 1, row.SpecimenID, row.PatientID, 0)
                                Else
                                    SetRowBackColorGroup(row, myPrevBackColor, myPrevSpecimenID, myPrevPatientID, myPrevHistPatientID)
                                End If

                                i += 1
                                myPrevBackColor = row.BackColorGroup
                                myPrevPatientID = row.PatientID
                                myPrevSpecimenID = row.SpecimenID
                                If (Not row.IsHistPatientIDNull) Then myPrevHistPatientID = row.HistPatientID Else myPrevHistPatientID = 0
                            Next
                        End If
                    End If
                End With
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".historyGridView_MouseDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".historyGridView_MouseDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub
#End Region
End Class