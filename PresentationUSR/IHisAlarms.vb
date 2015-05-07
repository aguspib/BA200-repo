Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.BL
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Globalization
Imports Biosystems.Ax00.Global.GlobalEnumerates

Public Class UiHisAlarms

#Region "Structures"
    Public Structure SearchFilter
        Public dateFrom As Date
        Public dateTo As Date
        Public alarmType As String
        Public analyzerId As String
    End Structure
#End Region

#Region "Declarations"
    Private myCultureInfo As CultureInfo
    Private myWSAlarmHistory As HisWSAnalyzerAlarmsDelegate
    Private myWSAlarmsDelegate As WSAnalyzerAlarmsDelegate

    'Language
    Private currentLanguage As String

    'Dictionaries for Code + Multilanguage Description
    Private mImageDict As Dictionary(Of String, Image)
    Private mTextDict As Dictionary(Of String, String)
    Private mAlarmTypes As Dictionary(Of String, String)
    Private SampleClassDict As New Dictionary(Of String, String)
    Private BlankModeDict As New Dictionary(Of String, String)
    Private SolutionCodeDict As New Dictionary(Of String, String)

    'List of Analyzers with Historical Alarms information
    Private mAnalyzers As List(Of String)

    'Images for the alarms
    Private NoImage As Byte() = Nothing
    Private ERRORImage As Byte() = Nothing
    Private WARNINGImage As Byte() = Nothing
    Private SOLVEDImage As Byte() = Nothing
    Private SOLVEDWarningImage As Byte() = Nothing
    Private SOLVEDErrorImage As Byte() = Nothing
#End Region

#Region "Attributes"
    Private AnalyzerIDAttribute As String = String.Empty
#End Region

#Region "Properties"
    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

#Region " Properties for Text definitions "
    ''' <summary>
    ''' Used for compatibility with code in UiMonitor
    ''' Text: "LBL_WSPrep_SampleClass"
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblSampleClass() As String
        Get
            Return GetText("LBL_WSPrep_SampleClass")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in UiMonitor
    ''' Text: "LBL_Name"
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblName() As String
        Get
            Return GetText("LBL_Name")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in UiMonitor
    ''' Text: "LBL_Test_Singular"
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblTest() As String
        Get
            Return GetText("LBL_Test_Singular")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in UiMonitor
    ''' Text: "LBL_CurveReplicate_Rep"
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblReplicateNumber() As String
        Get
            Return GetText("LBL_CurveReplicate_Rep")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in UiMonitor
    ''' Text: "LBL_SRV_POSITION"
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblRotorPosition() As String
        Get
            Return GetText("LBL_SRV_POSITION")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in UiMonitor
    ''' Text: "PMD_TUBE_CONTENTS_REAGENT"
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblReagent() As String
        Get
            Return GetText("PMD_TUBE_CONTENTS_REAGENT")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in UiMonitor
    ''' Text: "LBL_Solution"
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblSolution() As String
        Get
            Return GetText("LBL_Solution")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in UiMonitor
    ''' Text: "LBL_RotorPos_WashingSol"
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblWashingSolution() As String
        Get
            Return GetText("LBL_RotorPos_WashingSol")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in UiMonitor
    ''' Text: "PMD_TUBE_CONTENTS_SPEC_SOL"
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblSpecialSolution() As String
        Get
            Return GetText("PMD_TUBE_CONTENTS_SPEC_SOL")
        End Get
    End Property
#End Region

#Region " Properties for Format definitions "
    ''' <summary>
    ''' Defined for compatibility with code in UiMonitor
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property S_NO_VOLUME_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}, {4}: {5}", lblSampleClass, "{0}", lblName, "{1}", lblRotorPosition, "{2}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in UiMonitor
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property S_NO_VOLUME_BLANK_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}, {4}: {5}", lblSampleClass, "{0}", lblSolution, "{1}", lblRotorPosition, "{2}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in UiMonitor
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property PREP_LOCKED_Format() As String
        Get
            Return String.Format("- {0}, {1}, {2}: {3}", "{0}", "{1}", lblTest, "{2}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in UiMonitor
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property PREP_LOCKED_BLANK_Format() As String
        Get
            Return String.Format("- {0}, {1}: {2}, {3}: {4}", "{0}", lblSolution, "{1}", lblTest, "{2}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in UiMonitor
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property PREP_WITH_CLOT_Format() As String
        Get
            Return String.Format("- {0}, {1}, {2}: {3}, {4}: {5}", "{0}", "{1}", lblTest, "{2}", lblReplicateNumber, "{3}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in UiMonitor
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property PREP_WITH_CLOT_BLANK_Format() As String
        Get
            Return String.Format("- {0}, {1}: {2}, {3}: {4}, {5}: {6}", "{0}", lblSolution, "{1}", lblTest, "{2}", lblReplicateNumber, "{3}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in UiMonitor
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property R_NO_VOLUME_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}", lblReagent, "{0}", lblRotorPosition, "{1}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in UiMonitor
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property WASH_SOL_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}", lblWashingSolution, "{0}", lblRotorPosition, "{1}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in UiMonitor
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property SPEC_SOL_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}", lblSpecialSolution, "{0}", lblRotorPosition, "{1}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in UiMonitor
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 01/04/2015 - BA-2384 ==> Management of Alarm BOTTLE_LOCKED_WARN was missing in Historic Module
    ''' </remarks>
    Protected ReadOnly Property BOTTLE_LOCKED_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}", lblReagent, "{0}", lblRotorPosition, "{1}")
        End Get
    End Property
#End Region
#End Region

#Region "Constructor"
    Public Sub New()
        MyBase.New()
        InitializeComponent()

        myCultureInfo = My.Computer.Info.InstalledUICulture
        myWSAlarmHistory = New HisWSAnalyzerAlarmsDelegate
        myWSAlarmsDelegate = New WSAnalyzerAlarmsDelegate
    End Sub
#End Region

#Region "Private Methods to load local Dictionaries and Lists"
    ''' <summary>
    ''' Gets the image by its key. Only reads from file once for key
    ''' </summary>
    ''' <remarks>
    ''' Created by: IR 28/09/2012 
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
                mImageDict.Item(pKey) = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            Else
                mImageDict.Add(pKey, ImageUtilities.ImageFromFile(iconPath & auxIconName))
            End If
        End If
    End Sub

    ''' <summary>
    ''' Gets the multilanguage text by its key. Only reads from DataBase once for key
    ''' </summary>
    ''' <remarks>
    ''' Created by: IR 28/09/2012 
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
        Dim myText = myMultiLangResourcesDelegate.GetResourceText(Nothing, pKey, currentLanguage)

        If (String.IsNullOrEmpty(myText)) Then myText = "*" & pKey
        If mTextDict.ContainsKey(pKey) Then
            mTextDict.Item(pKey) = myText
        Else
            mTextDict.Add(pKey, myText)
        End If
    End Sub

    ''' <summary>
    ''' Load all the Alarm Types availables (with its multilanguage text)
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Sub GetAlarmTypes()
        mAlarmTypes.Clear()
        mAlarmTypes.Add("", GetText("LBL_SRV_All"))
        mAlarmTypes.Add("ERROR", GetText("LBL_ERROR"))
        mAlarmTypes.Add("WARNING", GetText("LBL_WARNING"))
    End Sub

    ''' <summary>
    ''' Read the list of Analyzers for which there are Historic Alarms in DB and load the global Analyzers list
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 28/09/2012
    ''' Modified by: IR 04/10/2012
    '''              SA 01/04/2015 - BA-2384 ==> Call function GetDistinctAnalyzers in HisAnalyzerWorkSessionsDelegate instead of the function 
    '''                                          with the same name in AnalyzerDelegate class (which read Analyzers from table thisWSAnalyzerAlarms).
    '''                                          If the connected Analyzer is not in the list, it is added to the Analyzer ComboBox
    ''' </remarks>
    Private Sub GetAnalyzerList()
        Try
            Dim myGlobalData As New GlobalDataTO
            Dim myHisAnalyzerWSDelegate As New HisAnalyzerWorkSessionsDelegate

            'Get the list of Analyzers for which there are Historic Alarms in DB 
            myGlobalData = myHisAnalyzerWSDelegate.GetDistinctAnalyzers(Nothing)
            If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                Dim myAnalyzers = DirectCast(myGlobalData.SetDatos, AnalyzersDS)

                Dim addConnectedAnalyzer = True
                If (myAnalyzers.tcfgAnalyzers.Rows.Count > 0) Then
                    'Add all of them to the the Analyzers list
                    For Each row As AnalyzersDS.tcfgAnalyzersRow In myAnalyzers.tcfgAnalyzers
                        mAnalyzers.Add(row.AnalyzerID)
                    Next

                    'If the connected Analyzer is not in the returned list, it has to be added to it 
                    addConnectedAnalyzer = ((myAnalyzers.tcfgAnalyzers.ToList.Where(Function(a) a.AnalyzerID = AnalyzerIDAttribute)).Count = 0)
                End If

                'Add the connected Analyzer to the list although there are not Historic Alarms for it yet
                If (addConnectedAnalyzer) Then mAnalyzers.Add(AnalyzerIDAttribute)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAnalyzerList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        'Dim myAnalyzerDelegate As New AnalyzersDelegate
        'Dim myGlobalDataTO As New GlobalDataTO
        'Dim myAnalyzerData As List(Of String)
        ''Let the user select more than one analyzer if available. We must read data from thisWSAnalyzerAlarmsDAO
        'myGlobalDataTO = myAnalyzerDelegate.GetDistinctAnalyzers(Nothing)
        'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '    myAnalyzerData = DirectCast(myGlobalDataTO.SetDatos, List(Of String))
        '    'mAnalyzers = (From a In myAnalyzerData.tcfgAnalyzers _
        '    '              Where Not a.Generic _
        '    '              Order By a.Active Descending _
        '    '              Select a.AnalyzerID).ToList

        '    Dim o As String
        '    For Each o In myAnalyzerData
        '        mAnalyzers.Add(o.ToString)
        '    Next

        '    'mAnalyzers = myGlobalDataTO.SetDatos
        'End If
        ''End IR 04/10/2012
    End Sub

    ''' <summary>
    ''' Gets all the preloaded images
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Sub GetPreloadedImages()
        Try
            Dim preloadedDataConfig As New PreloadedMasterDataDelegate
            Dim auxIconName As String = String.Empty

            'ERROR Image
            auxIconName = GetIconName("WARNINGSMALL")
            If Not String.IsNullOrEmpty(auxIconName) Then ERRORImage = preloadedDataConfig.GetIconImage("WARNINGSMALL")

            'WARNING Image
            auxIconName = GetIconName("STUS_WITHERRS")
            If Not String.IsNullOrEmpty(auxIconName) Then WARNINGImage = preloadedDataConfig.GetIconImage("STUS_WITHERRS")

            'SOLVED ALARM Image
            SOLVEDImage = NoImage
            auxIconName = GetIconName("SOLVEDSMALL")
            If Not String.IsNullOrEmpty(auxIconName) Then SOLVEDImage = preloadedDataConfig.GetIconImage("SOLVEDSMALL")

            'WARNING Disable Image 
            auxIconName = GetIconName("WRNGSMLDIS")
            If Not String.IsNullOrEmpty(auxIconName) Then SOLVEDErrorImage = preloadedDataConfig.GetIconImage("WRNGSMLDIS")

            'ALERT Disable Image 
            auxIconName = GetIconName("ALRTSMLDIS")
            If Not String.IsNullOrEmpty(auxIconName) Then SOLVEDWarningImage = preloadedDataConfig.GetIconImage("ALRTSMLDIS")

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetPreloadedImages ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    '''  Gets all label texts for Alarms
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 28/09/2012 - Adapted from UiMonitorAlarmsTab.vb -> GetAlarmsTabLabels()
    ''' Modified by: SA 01/04/2015 - BA-2384 ==> Code to fill the global Dictionary of Additional Solution Codes moved to a new function 
    '''                                          FillSolutionCodesDictionary
    ''' </remarks>
    Private Sub InitializeAlarmTexts()
        Try
            'Fill the Dictionary of Sample Classes Codes
            SampleClassDict("BLANK") = GetText("PMD_SAMPLE_CLASSES_BLANK")
            SampleClassDict("CALIB") = GetText("PMD_SAMPLE_CLASSES_CALIB")
            SampleClassDict("CTRL") = GetText("PMD_SAMPLE_CLASSES_CTRL")
            SampleClassDict("PATIENT") = GetText("PMD_SAMPLE_CLASSES_PATIENT")

            'Fill the Dictionary of Special Solutions used for Blanks
            BlankModeDict("SALINESOL") = GetText("PMD_SPECIAL_SOLUTIONS_SALINESOL")
            BlankModeDict("DISTW") = GetText("PMD_SPECIAL_SOLUTIONS_DISTW")
            BlankModeDict("REAGENT") = GetText("PMD_BLANK_MODES_REAGENT")

            'BA-2384: Fill the Dictionary of Additional Solution Codes with all Dilution, Special and Washing Solutions
            FillSolutionCodesDictionary(GlobalEnumerates.PreloadedMasterDataEnum.DIL_SOLUTIONS)
            FillSolutionCodesDictionary(GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS)
            FillSolutionCodesDictionary(GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS)
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".InitializeAlarmTexts", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Fill the Dictionary of Additional Solution Codes with the type of Additional Solution informed; Dilution, Special or Washing Solutions
    ''' </summary>
    ''' <param name="pAdditionalSolutionType">Unique identifier of the Sub Table of the Preloaded Master Data from which the data will be obtained</param>
    ''' <remarks>
    ''' Created by:  SA 01/04/2015 - Code extracted from function InitializeAlarmTexts
    ''' </remarks>
    Private Sub FillSolutionCodesDictionary(ByVal pAdditionalSolutionType As PreloadedMasterDataEnum)
        Try
            Dim myGlobalData As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            myGlobalData = myPreloadedMasterDataDelegate.GetList(Nothing, pAdditionalSolutionType)
            If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                Dim myMasterDataDS As PreloadedMasterDataDS = DirectCast(myGlobalData.SetDatos, PreloadedMasterDataDS)

                For Each solutionCode As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myMasterDataDS.tfmwPreloadedMasterData
                    If (Not SolutionCodeDict.ContainsKey(solutionCode.ItemID)) Then
                        SolutionCodeDict(solutionCode.ItemID) = solutionCode.FixedItemDesc
                    End If
                Next
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".FillSolutionCodesDictionary", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub
#End Region

#Region "Private Methods to initialize the screen"
    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            bsSubtitleLabel.Text = GetText("LBL_HistAlarms")

            bsDateFromLabel.Text = GetText("LBL_Date_From") & ":"
            bsDateToLabel.Text = GetText("LBL_Date_To") & ":"
            bsTypeLabel.Text = GetText("LBL_Type") & ":"
            bsAnalyzerIDLabel.Text = GetText("LBL_SCRIPT_EDIT_Analyzer") & ":"

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the icons and tooltips used for painting the buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Sub PrepareButtons()
        Dim myToolTipsControl As New ToolTip
        Try
            'EXIT Button
            bsExitButton.Image = GetImage("CANCEL")
            myToolTipsControl.SetToolTip(bsExitButton, GetText("BTN_CloseScreen"))

            'SEARCH Button
            bsSearchButton.Image = GetImage("FIND")
            myToolTipsControl.SetToolTip(bsSearchButton, GetText("BTN_Search"))

            'DELETE Button
            bsHistoryDelete.Image = GetImage("REMOVE")
            myToolTipsControl.SetToolTip(bsHistoryDelete, GetText("BTN_Delete"))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    '''  Fills the DropDownLists
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 28/09/2012
    ''' Modified by: JB 16/10/2012 - ComboBox of Analyzers is not visible when there is only one Analyzer in the list 
    ''' </remarks>
    Private Sub FillDropDownLists()
        GetAlarmTypes()
        GetAnalyzerList()

        bsTypeComboBox.DataSource = mAlarmTypes.Values.ToList
        bsAnalyzerIDComboBox.DataSource = mAnalyzers

        bsAnalyzerIDComboBox.Enabled = mAnalyzers.Count > 1
        bsAnalyzerIDComboBox.Visible = bsAnalyzerIDComboBox.Enabled
        bsAnalyzerIDLabel.Visible = bsAnalyzerIDComboBox.Visible
    End Sub

    ''' <summary>
    ''' Initializes the embedded navigator in xtraHistoryGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Private Sub InitializeGridNavigator()
        Try
            With xtraHistoryGrid.EmbeddedNavigator.Buttons
                .First.Hint = GetText("BTN_GoToFirst")
                .PrevPage.Hint = GetText("BTN_GoToPrevious")
                .NextPage.Hint = GetText("BTN_GoToNext")
                .Last.Hint = GetText("BTN_GoToLast")
            End With
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".InitializeGridNavigator ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Creates and initializes the xtraHistoryGrid
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Sub InitializeAlarmsHistoryGrid()
        Try
            xtraHistoryGridView.Columns.Clear()

            InitializeGridNavigator()

            Dim TypeColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim DateColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim TimeColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim NameColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim DescriptionColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim SolutionColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim AnalyzerColumn As New DevExpress.XtraGrid.Columns.GridColumn()


            With xtraHistoryGridView
                .Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {TypeColumn, DateColumn, TimeColumn, NameColumn, _
                                                                                DescriptionColumn, SolutionColumn, AnalyzerColumn})

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
                .OptionsSelection.EnableAppearanceFocusedRow = True
                .OptionsSelection.MultiSelect = True
                .ColumnPanelRowHeight = 30
                .GroupCount = 0
                .OptionsMenu.EnableColumnMenu = False
            End With

            'TypeColumn
            With TypeColumn
                .Caption = GetText("LBL_Type")
                .FieldName = "AlarmTypeImage"
                .Name = "Type"
                .Visible = True
                .Width = 38
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
            End With

            'Date Column
            With DateColumn
                .Caption = GetText("LBL_Date")
                .FieldName = "AlarmDateTime"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                .DisplayFormat.FormatString = SystemInfoManager.OSDateFormat
                .Name = "Date"
                .Visible = True
                .Width = 75
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'Time column
            With TimeColumn
                .Caption = GetText("LBL_Time")
                .FieldName = "AlarmDateTime"
                .DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                .DisplayFormat.FormatString = SystemInfoManager.OSLongTimeFormat
                .Name = "Time"
                .Visible = True
                .Width = 75
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
                .AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            End With

            'Name Column
            With NameColumn
                .Caption = GetText("LBL_Name")
                .FieldName = "Name"
                .Name = "Name"
                .Visible = True
                .Width = 140
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
            End With

            'Description Column
            With DescriptionColumn
                .Caption = GetText("LBL_Description")
                .FieldName = "Description"
                .Name = "Description"
                .Visible = True
                .Width = 370
                .OptionsColumn.AllowSize = True
                .OptionsColumn.ShowCaption = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
            End With

            'Solution Column
            With SolutionColumn
                .Caption = GetText("LBL_Solution")
                .FieldName = "Solution"
                .Name = "Solution"
                .Visible = True
                .Width = 370
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True
            End With

            'AnalyzerColumn Column
            With AnalyzerColumn
                .Caption = GetText("LBL_SCRIPT_EDIT_Analyzer")
                .FieldName = "AnalyzerID"
                .Name = "AnalyzerID"
                .Visible = True
                .Width = 140
                .OptionsColumn.AllowSize = True
                .OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
                .OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
            End With

            'For showing an image on cells
            Dim pictureEdit As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit
            pictureEdit = TryCast(xtraHistoryGrid.RepositoryItems.Add("PictureEdit"),  _
                                  DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit)

            pictureEdit.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip
            pictureEdit.NullText = " "

            TypeColumn.ColumnEdit = pictureEdit

            'For wrapping text
            Dim largeTextEdit As DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit
            largeTextEdit = TryCast(xtraHistoryGrid.RepositoryItems.Add("MemoEdit"),  _
                                    DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit)

            NameColumn.ColumnEdit = largeTextEdit
            DescriptionColumn.ColumnEdit = largeTextEdit
            SolutionColumn.ColumnEdit = largeTextEdit

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".InitializeAlarmsHistoryGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
    End Sub

    ''' <summary>
    ''' Initializes the filter search
    ''' </summary>
    ''' <remarks>
    ''' Created by JB 28/09/2012
    ''' </remarks>
    Private Sub InitializeFilterSearch()
        Try
            bsDateFromDateTimePick.Checked = True
            bsDateFromDateTimePick.Value = Today.AddMonths(-1)

            bsDateToDateTimePick.Checked = True
            bsDateToDateTimePick.Value = Today

            bsTypeComboBox.SelectedIndex = 0

            If (bsAnalyzerIDComboBox.Items.Count > 0) Then
                bsAnalyzerIDComboBox.SelectedIndex = 0
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".InitializeFilterSearch", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Initialize all screen controls
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            mImageDict = New Dictionary(Of String, Image)()
            mTextDict = New Dictionary(Of String, String)()

            mAlarmTypes = New Dictionary(Of String, String)
            mAnalyzers = New List(Of String)

            'Get the current Language from the current Application Session
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString()

            'SG 31/05/2013 - Get Level of the connected User
            CurrentUserLevel = GlobalBase.GetSessionInfo().UserLevel
            ScreenStatusByUserLevel()

            GetScreenLabels()
            InitializeAlarmTexts()
            PrepareButtons()
            FillDropDownLists()
            GetPreloadedImages()
            InitializeAlarmsHistoryGrid()

            InitializeFilterSearch()
            FindHistoricalResults()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Created by: SG 31/05/2013
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    bsHistoryDelete.Enabled = False
                    Exit Select
            End Select
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Private Methods for searching Historic Alarms and load the grid"
    ''' <summary>
    ''' Returns the selected search filter
    ''' </summary>
    ''' <returns>Structure SearchFilter filled with all values selected in Search Area in the screen</returns>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Function GetSearchFilter() As SearchFilter
        Dim filter As New SearchFilter
        With filter
            .analyzerId = bsAnalyzerIDComboBox.SelectedItem.ToString

            If (bsDateFromDateTimePick.Checked) Then
                .dateFrom = bsDateFromDateTimePick.Value
            Else
                .dateFrom = Nothing
            End If
            If (bsDateToDateTimePick.Checked) Then
                .dateTo = bsDateToDateTimePick.Value.AddDays(1)
            Else
                .dateTo = Nothing
            End If

            .alarmType = (From kvp As KeyValuePair(Of String, String) In mAlarmTypes _
                         Where kvp.Value = bsTypeComboBox.SelectedItem.ToString _
                        Select kvp.Key).FirstOrDefault
        End With
        Return filter
    End Function

    ''' <summary>
    ''' Find the Results by the selected filter in screen
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Sub FindHistoricalResults()
        xtraHistoryGrid.DataSource = Nothing

        Try
            If (bsAnalyzerIDComboBox.Items.Count > 0) Then
                Dim filter As SearchFilter = GetSearchFilter()

                Dim myGlobalData As New GlobalDataTO
                myGlobalData = myWSAlarmHistory.GetAlarmsMonitor(Nothing, filter.analyzerId, filter.dateFrom, filter.dateTo, filter.alarmType)

                If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                    Dim myHisWSAnalyzerAlarmsDS As HisWSAnalyzerAlarmsDS = DirectCast(myGlobalData.SetDatos, HisWSAnalyzerAlarmsDS)

                    PrepareAndSetDataToGrid(myHisWSAnalyzerAlarmsDS.vwksAlarmsMonitor)
                End If
            End If
            UpdateFormBehavior()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Name & ".FindHistoricalResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Updates the Form Behavior: enable buttons, show actions...
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Private Sub UpdateFormBehavior()
        Try
            bsHistoryDelete.Enabled = xtraHistoryGridView.SelectedRowsCount > 0
            bsSearchGroup.Enabled = bsAnalyzerIDComboBox.Items.Count > 0
            ScreenStatusByUserLevel()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".UpdateFormBehavior ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Private Methods to Decode Image and Description of all Alarms loaded"
    ''' <summary>
    ''' Decodes the alarm description
    ''' </summary>
    ''' <param name="pRow">Row of a typed DataSet HisWSAnalyzerAlarmsDS containing data of the Alarm that has to be shown</param>
    ''' <remarks>
    ''' Created by:  JB 28/09/2012 (addapted from IMonitorAlarmsTab.vb -> UpdateAlarmsTab() )
    ''' Modified by: JV 27/01/2014 - BA-1463 ==> For Alarm WS_PAUSE_MODE_WARN is not needed to decode field AdditionalInfo; the content of the
    '''                                          field is added to the Alarm Description
    '''              SA 01/04/2015 - BA-2384 ==> Based on implementation of BA-2236 for BA-200. When field AdditionalInfo is informed and contains 
    '''                                          a numeric value, it means that value is the FW Error Code for the Alarm and it is added to the Alarm
    '''                                          Description between brackets. Changes in the code to call new functions GetDescriptionForISEAlarms 
    '''                                          and GetDescriptionForOtherAlarms.
    ''' </remarks>
    Private Sub DecodeAlarmDescription(ByRef pRow As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow)
        Try
            If (Not String.IsNullOrEmpty(pRow.AdditionalInfo)) Then
                Dim myGlobalData As New GlobalDataTO

                If (pRow.AlarmID = Alarms.ISE_CALIB_ERROR.ToString OrElse pRow.AlarmID = Alarms.ISE_ERROR_A.ToString OrElse _
                    pRow.AlarmID = Alarms.ISE_ERROR_B.ToString OrElse pRow.AlarmID = Alarms.ISE_ERROR_C.ToString OrElse _
                    pRow.AlarmID = Alarms.ISE_ERROR_D.ToString OrElse pRow.AlarmID = Alarms.ISE_ERROR_S.ToString OrElse _
                    pRow.AlarmID = Alarms.ISE_ERROR_F.ToString OrElse pRow.AlarmID = Alarms.ISE_ERROR_M.ToString OrElse _
                    pRow.AlarmID = Alarms.ISE_ERROR_N.ToString OrElse pRow.AlarmID = Alarms.ISE_ERROR_R.ToString OrElse _
                    pRow.AlarmID = Alarms.ISE_ERROR_W.ToString OrElse pRow.AlarmID = Alarms.ISE_ERROR_P.ToString OrElse _
                    pRow.AlarmID = Alarms.ISE_ERROR_T.ToString) Then
                    'Decode field AdditionalInfo for ISE Alarms
                    myGlobalData = GetDescriptionForISEAlarms(pRow)

                ElseIf (pRow.AlarmID = Alarms.WS_PAUSE_MODE_WARN.ToString) Then
                    'BA-1463: For this Alarm field AdditionalInfo does not need to be decoded
                    pRow.Description &= pRow.AdditionalInfo

                ElseIf (IsNumeric(pRow.AdditionalInfo.Replace(",", ""))) Then
                    'BA-2384: When field AdditionalInfo is NUMERIC, it contains the FW Error Code for the Alarm (field can contain
                    '         also a list of FW Error Codes divided by commas, when the Alarm is linked to several Error Codes and
                    '         the Analyzer has sent all of them). The value is shown between brackets following the Alarm Description
                    pRow.Description &= " - [" & pRow.AdditionalInfo.ToString & "]"
                Else
                    'Decode field Additional Info for other types of Alarms
                    myGlobalData = GetDescriptionForOtherAlarms(pRow)
                End If

                If (myGlobalData.HasError) Then
                    'If an error has happened, show it
                    ShowMessage(Me.Name & ".DecodeAlarmDescription", myGlobalData.ErrorCode, myGlobalData.ErrorMessage, MsgParent)
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".DecodeAlarmDescription ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' According the type of Alarm, show the Error Icon or the Warning Icon. Additionally, if the Alarm Status is True, it means
    ''' the Alarm has been solved, and in this case the Icon has a different BackColor 
    ''' </summary>
    ''' <param name="pRow">Row of a typed DataSet HisWSAnalyzerAlarmsDS containing data of the Alarm that has to be shown</param>
    ''' <remarks>
    ''' Created by: JB 28/09/2012 (adapted from UiMonitorAlarmsTab.vb -> UpdateAlarmsTab())
    ''' </remarks>
    Private Sub DecodeAlarmTypeImage(ByRef pRow As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow)
        Try
            Select Case pRow.AlarmType
                Case "ERROR"
                    If (pRow.AlarmStatus) Then
                        pRow.AlarmTypeImage = ERRORImage
                    Else
                        pRow.AlarmTypeImage = SOLVEDErrorImage
                    End If
                Case "WARNING"
                    If (pRow.AlarmStatus) Then
                        pRow.AlarmTypeImage = WARNINGImage
                    Else
                        pRow.AlarmTypeImage = SOLVEDWarningImage
                    End If
                Case Else
                    pRow.AlarmTypeImage = NoImage
            End Select

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".DecodeAlarmTypeImage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the formatted additional information to show for alarm PREP_LOCKED_WARN 
    ''' </summary>
    ''' <param name="pAddInfoPreparationRow">Row of typed DataSet WSAnalyzerAlarmsDS (sub-table AdditionalInfoPrepLocked) containing all data needed 
    '''                                      to build the Additional Information field for this type of Alarm</param>
    ''' <param name="pElementName">Name of the affected Element</param>
    ''' <returns>String formatted with all additional information that has to be shown</returns>
    ''' <remarks>
    ''' Created by: SA 01/04/2015 - BA-2384 ==> Adapted from function with the same name in UiMonitorAlarmsTab.vb
    ''' </remarks>
    Private Function GetAdditionalInfoForAlarmPREP_LOCKED_WARN(ByVal pAddInfoPreparationRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow, ByVal pElementName As String) As String
        Dim additionalInfo As String = String.Empty

        Try
            If (pAddInfoPreparationRow.SampleClass = "BLANK") Then
                additionalInfo = String.Format(PREP_LOCKED_BLANK_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), BlankModeDict(pElementName), _
                                               pAddInfoPreparationRow.TestName)
            Else
                additionalInfo = String.Format(PREP_LOCKED_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), pElementName, pAddInfoPreparationRow.TestName)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAdditionalInfoForAlarmPREP_LOCKED_WARN", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return additionalInfo
    End Function

    ''' <summary>
    ''' Get the formatted additional information to show for alarm S_NO_VOLUME_WARN (depleted Sample Tube)
    ''' </summary>
    ''' <param name="pAddInfoPreparationRow">Row of typed DataSet WSAnalyzerAlarmsDS (sub-table AdditionalInfoPrepLocked) containing all data needed 
    '''                                      to build the Additional Information field for this type of Alarm</param>
    ''' <param name="pElementName">Name of the affected Element</param>
    ''' <returns>String formatted with all additional information that has to be shown</returns>
    ''' <remarks>
    ''' Created by: SA 01/04/2015 - BA-2384 ==> Adapted from function with the same name in UiMonitorAlarmsTab.vb
    ''' </remarks>
    Private Function GetAdditionalInfoForAlarmS_NO_VOLUME_WARN(ByVal pAddInfoPreparationRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow, ByVal pElementName As String) As String
        Dim additionalInfo As String = String.Empty

        Try
            If (pAddInfoPreparationRow.SampleClass = "BLANK") Then
                additionalInfo = String.Format(S_NO_VOLUME_BLANK_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), BlankModeDict(pElementName), _
                                               pAddInfoPreparationRow.RotorPosition)
            Else
                additionalInfo = String.Format(S_NO_VOLUME_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), pElementName, pAddInfoPreparationRow.RotorPosition)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAdditionalInfoForAlarmS_NO_VOLUME_WARN", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return additionalInfo
    End Function

    ''' <summary>
    ''' Get the formatted additional information to show for both Clot Detection Alarms: CLOT_DETECTION_ERR and CLOT_DETECTION_WARN
    ''' </summary>
    ''' <param name="pAddInfoPreparationRow">Row of typed DataSet WSAnalyzerAlarmsDS (sub-table AdditionalInfoPrepLocked) containing all data needed 
    '''                                      to build the Additional Information field for this type of Alarm</param>
    ''' <param name="pElementName">Name of the affected Element</param>
    ''' <returns>String formatted with all additional information that has to be shown</returns>
    ''' <remarks>
    ''' Created by: SA 01/04/2015 - BA-2384 ==> Adapted from function with the same name in UiMonitorAlarmsTab.vb
    ''' </remarks>
    Private Function GetAdditionalInfoForAlarmsCLOT_DETECTION(ByVal pAddInfoPreparationRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow, ByVal pElementName As String) As String
        Dim additionalInfo As String = String.Empty

        Try
            If (pAddInfoPreparationRow.SampleClass = "BLANK") Then
                additionalInfo = String.Format(PREP_WITH_CLOT_BLANK_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), BlankModeDict(pElementName), _
                                               pAddInfoPreparationRow.TestName, pAddInfoPreparationRow.ReplicateNumber)
            Else
                additionalInfo = String.Format(PREP_WITH_CLOT_Format, SampleClassDict(pAddInfoPreparationRow.SampleClass), pElementName, pAddInfoPreparationRow.TestName, _
                                               pAddInfoPreparationRow.ReplicateNumber)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAdditionalInfoForAlarmsCLOT_DETECTION", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return additionalInfo
    End Function

    ''' <summary>
    ''' Get the formatted additional information to show for both Depleted Reagent Alarms: R1_NO_VOLUME_WARN and R2_NO_VOLUME_WARN
    ''' </summary>
    ''' <param name="pAddInfoPreparationRow">Row of typed DataSet WSAnalyzerAlarmsDS (sub-table AdditionalInfoPrepLocked) containing all data needed 
    '''                                      to build the Additional Information field for this type of Alarm</param>
    ''' <param name="pElementName">Name of the affected Element</param>
    ''' <returns>String formatted with all additional information that has to be shown</returns>
    ''' <remarks>
    ''' Created by: SA 01/04/2015 - BA-2384 ==> Adapted from function with the same name in UiMonitorAlarmsTab.vb
    ''' </remarks>
    Private Function GetAdditionalInfoForAlarmsREAGENT_NO_VOLUME(ByVal pAddInfoPreparationRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow, ByVal pElementName As String) As String
        Dim additionalInfo As String = String.Empty

        Try
            Select Case (pAddInfoPreparationRow.TubeContent)
                Case "SPEC_SOL"
                    additionalInfo = String.Format(SPEC_SOL_Format, SolutionCodeDict(pAddInfoPreparationRow.SolutionCode), pAddInfoPreparationRow.RotorPosition)

                Case "WASH_SOL"
                    additionalInfo = String.Format(WASH_SOL_Format, SolutionCodeDict(pAddInfoPreparationRow.SolutionCode), pAddInfoPreparationRow.RotorPosition)

                Case Else
                    additionalInfo = String.Format(R_NO_VOLUME_Format, pAddInfoPreparationRow.TestName, pAddInfoPreparationRow.RotorPosition)
            End Select
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetAdditionalInfoForAlarmsREAGENT_NO_VOLUME", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return additionalInfo
    End Function

    ''' <summary>
    ''' For ISE Alarms, decode field AdditionalInfo and inform it in Description field
    ''' </summary>
    ''' <param name="pAlarmMonitorRow">Row of a typed DataSet HisWSAnalyzerAlarmsDS containing data of the Alarm that has to be shown</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by: SA 31/03/2015 - BA-2384 ==> Adapted from function with the same name in UiMonitorAlarmsTab.vb
    ''' </remarks>
    Private Function GetDescriptionForISEAlarms(ByVal pAlarmMonitorRow As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow) As GlobalDataTO
        Dim myGloblaData As New GlobalDataTO

        Try
            Dim myAnalyzerAlarmsDelegate As New WSAnalyzerAlarmsDelegate
            myGloblaData = myAnalyzerAlarmsDelegate.DecodeISEAdditionalInfo(pAlarmMonitorRow.AlarmID, pAlarmMonitorRow.AdditionalInfo, pAlarmMonitorRow.AnalyzerID)

            If (Not myGloblaData.HasError AndAlso Not myGloblaData.SetDatos Is Nothing) Then
                Dim myAdditionalInfoDS = DirectCast(myGloblaData.SetDatos, WSAnalyzerAlarmsDS)

                For Each row As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In myAdditionalInfoDS.twksWSAnalyzerAlarms.Rows
                    If (row.AdditionalInfo.Trim.Length > 0) Then
                        pAlarmMonitorRow.Description &= Environment.NewLine & row.AlarmID & " " & row.AdditionalInfo
                    Else
                        pAlarmMonitorRow.Description = row.AlarmID
                    End If
                Next
                pAlarmMonitorRow.AcceptChanges()
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetDescriptionForISEAlarms", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return myGloblaData
    End Function

    ''' <summary>
    ''' Decode content of field Additional Info for all types of Alarms that have this field informed
    ''' </summary>
    ''' <param name="pAlarmMonitorRow">Row of a typed DataSet HisWSAnalyzerAlarmsDS containing data of the Alarm that has to be shown</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by: SA 31/03/2015 - BA-2384 ==> Adapted from function with the same name in UiMonitorAlarmsTab.vb
    ''' </remarks>
    Private Function GetDescriptionForOtherAlarms(ByVal pAlarmMonitorRow As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow) As GlobalDataTO
        Dim myGlobalData As New GlobalDataTO
        Try
            Dim myWSAlarmsDelegate As New WSAnalyzerAlarmsDelegate
            myGlobalData = myWSAlarmsDelegate.DecodeAdditionalInfo(pAlarmMonitorRow.AlarmID, pAlarmMonitorRow.AdditionalInfo)

            If (Not myGlobalData.HasError AndAlso Not myGlobalData.SetDatos Is Nothing) Then
                Dim additionalInfoDS As WSAnalyzerAlarmsDS = DirectCast(myGlobalData.SetDatos, WSAnalyzerAlarmsDS)

                If (additionalInfoDS.AdditionalInfoPrepLocked.Rows.Count > 0) Then
                    Dim adRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow = additionalInfoDS.AdditionalInfoPrepLocked(0)

                    Dim elementName As String = adRow.Name
                    If (adRow.SampleClass = "CALIB" AndAlso adRow.NumberOfCalibrators <> "1") Then
                        elementName = String.Format("{0}-{1}", adRow.Name, adRow.MultiItemNumber)
                    End If

                    'Get the formatted Additional Information to show according the type of Alarm
                    Dim additionalInfo As String = String.Empty
                    Select Case (pAlarmMonitorRow.AlarmID)
                        Case Alarms.S_NO_VOLUME_WARN.ToString()
                            additionalInfo = GetAdditionalInfoForAlarmS_NO_VOLUME_WARN(adRow, elementName)

                        Case Alarms.PREP_LOCKED_WARN.ToString()
                            additionalInfo = GetAdditionalInfoForAlarmPREP_LOCKED_WARN(adRow, elementName)

                        Case Alarms.CLOT_DETECTION_ERR.ToString(), Alarms.CLOT_DETECTION_WARN.ToString()
                            additionalInfo = GetAdditionalInfoForAlarmsCLOT_DETECTION(adRow, elementName)

                        Case Alarms.R1_NO_VOLUME_WARN.ToString(), Alarms.R2_NO_VOLUME_WARN.ToString()
                            additionalInfo = GetAdditionalInfoForAlarmsREAGENT_NO_VOLUME(adRow, elementName)

                        Case Alarms.BOTTLE_LOCKED_WARN.ToString()
                            additionalInfo = String.Format(BOTTLE_LOCKED_Format, adRow.TestName, adRow.RotorPosition)
                    End Select

                    'Finally, add the formatted Additional Information in a new line of Description field
                    pAlarmMonitorRow.Description &= Environment.NewLine & additionalInfo
                    pAlarmMonitorRow.AcceptChanges()
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".GetDescriptionForOtherAlarms", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
        End Try
        Return myGlobalData
    End Function

    ''' <summary>
    ''' Prepare the DataSet with friendly data (as Monitor tab) and sets to Grid
    ''' </summary>
    ''' <param name="pAlarmsDataTable"></param>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Sub PrepareAndSetDataToGrid(ByVal pAlarmsDataTable As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorDataTable)
        Try
            For Each row As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow In pAlarmsDataTable
                DecodeAlarmTypeImage(row)
                DecodeAlarmDescription(row)
            Next
            xtraHistoryGrid.DataSource = pAlarmsDataTable

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".PrepareAndSetDataToGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Private Methods for deleting selected Historic Alarms"
    ''' <summary>
    ''' Delete all selected rows in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 02/10/2012
    ''' Modified by: JV 27/01/2014 -BA-1463
    ''' </remarks>
    Private Sub DeleteSelectedRowsFromGrid(ByVal pGrid As DevExpress.XtraGrid.Views.Grid.GridView)
        Try
            If (pGrid.SelectedRowsCount = 0) Then Exit Sub

            'Show delete confirmation message 
            If (ShowMessage(Me.Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) <> Windows.Forms.DialogResult.Yes) Then Exit Sub

            Dim myHisWSAnalyzerAlarmDS As New HisWSAnalyzerAlarmsDS
            Dim vAlarmMonitorRow As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow

            'For each row, creates a new "HistoryAlarm"
            For i As Integer = 0 To pGrid.SelectedRowsCount() - 1
                If (pGrid.GetSelectedRows()(i) >= 0) Then
                    vAlarmMonitorRow = DirectCast(pGrid.GetDataRow(pGrid.GetSelectedRows()(i)), HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow)
                    With vAlarmMonitorRow
                        If (.IsOKDateTimeNull) Then .OKDateTime = Nothing

                        'Creates the new row HisWSAnalyzerAlarmsDS
                        myHisWSAnalyzerAlarmDS.thisWSAnalyzerAlarms.AddthisWSAnalyzerAlarmsRow(.AlarmID, .AnalyzerID, .AlarmDateTime, .AlarmItem, _
                                                                                               .AlarmType, .WorkSessionID, .AdditionalInfo, _
                                                                                               .AlarmStatus, .OKDateTime, Nothing)
                    End With
                End If
            Next
            myHisWSAnalyzerAlarmDS.thisWSAnalyzerAlarms.AcceptChanges()

            'Delete all selected Alarms from DB
            Dim myGlobalData As New GlobalDataTO
            myGlobalData = myWSAlarmHistory.Delete(Nothing, myHisWSAnalyzerAlarmDS)

            If (myGlobalData.HasError) Then
                'If an error has happened, show it
                ShowMessage(Me.Name & ".DeleteSelectedRowsFromGrid", myGlobalData.ErrorCode, myGlobalData.ErrorMessage, MsgParent)
            End If

            'Reload the grid
            FindHistoricalResults()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region "Events"
#Region " Screen Events "
    Private Sub IHisAlarms_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then bsExitButton.PerformClick()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".IHisAlarms_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IHisAlarms_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            If (Me.DesignMode) Then Exit Sub

            InitializeScreen()
            ResetBorder()
            Application.DoEvents()
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".IHisAlarms_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

#Region " Button Events "
    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            If (Not Tag Is Nothing) Then
                'A PerformClick() method was executed
                Close()
            Else
                'Normal button click - Open the WS Monitor form and close this one
                UiAx00MainMDI.OpenMonitorForm(Me)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex)
            ShowMessage(Me.Name & ".ExitButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsSearchButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSearchButton.Click
        FindHistoricalResults()
    End Sub

    Private Sub bsHistoryDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsHistoryDelete.Click
        DeleteSelectedRowsFromGrid(xtraHistoryGridView)
    End Sub
#End Region

#Region " DateTimePicker Events "
    Private Sub bsDateFromDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDateFromDateTimePick.ValueChanged
        If bsDateFromDateTimePick.Checked Then
            bsDateToDateTimePick.MinDate = bsDateFromDateTimePick.Value
        Else
            bsDateToDateTimePick.MinDate = Today.AddYears(-100)
        End If
    End Sub

    Private Sub bsDateToDateTimePick_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDateToDateTimePick.ValueChanged
        If bsDateToDateTimePick.Checked Then
            bsDateFromDateTimePick.MaxDate = bsDateToDateTimePick.Value
        Else
            bsDateFromDateTimePick.MaxDate = Today.AddYears(100)
        End If
    End Sub
#End Region

#Region " Grid Events "
    Private Sub xtraHistoryGridView_SelectionChanged(ByVal sender As System.Object, ByVal e As DevExpress.Data.SelectionChangedEventArgs) Handles xtraHistoryGridView.SelectionChanged
        UpdateFormBehavior()
    End Sub
#End Region
#End Region
End Class