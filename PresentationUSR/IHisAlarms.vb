Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.BL
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Globalization

Public Class IHisAlarms

#Region "Events definitions"

#End Region

#Region " Structures "
    Public Structure SearchFilter
        Public dateFrom As Date
        Public dateTo As Date
        Public alarmType As String
        Public analyzerId As String
    End Structure
#End Region

#Region "Attributes"
    Private AnalyzerIDAttribute As String = String.Empty
#End Region

#Region "Declarations"
    Private myCultureInfo As CultureInfo
    Private myWSAlarmHistory As HisWSAnalyzerAlarmsDelegate
    Private myWSAlarmsDelegate As WSAnalyzerAlarmsDelegate

    ' Language
    Private currentLanguage As String

    'Multi language resources
    Private mImageDict As Dictionary(Of String, Image)
    Private mTextDict As Dictionary(Of String, String)

    Private mAlarmTypes As Dictionary(Of String, String) ' The alarm Typles: {AlarmType, Multilanguage Text}
    Private mAnalyzers As List(Of String)

    'Images for the alarms
    Private NoImage As Byte() = Nothing
    Private ERRORImage As Byte() = Nothing
    Private WARNINGImage As Byte() = Nothing
    Private SOLVEDimage As Byte() = Nothing
    Private SOLVEDWarningImage As Byte() = Nothing
    Private SOLVEDErrorImage As Byte() = Nothing


    Private SampleClassDict As New Dictionary(Of String, String)
    Private BlankModeDict As New Dictionary(Of String, String)
    Private SolutionCodeDict As New Dictionary(Of String, String)
#End Region


#Region "Properties"
    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property

#Region "Text definitions"
    ''' <summary>
    ''' Used for compatibility with code in IMonitor
    ''' Text: "LBL_WSPrep_SampleClass"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblSampleClass() As String
        Get
            Return GetText("LBL_WSPrep_SampleClass")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in IMonitor
    ''' Text: "LBL_Name"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblName() As String
        Get
            Return GetText("LBL_Name")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in IMonitor
    ''' Text: "LBL_Test_Singular"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblTest() As String
        Get
            Return GetText("LBL_Test_Singular")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in IMonitor
    ''' Text: "LBL_CurveReplicate_Rep"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblReplicateNumber() As String
        Get
            Return GetText("LBL_CurveReplicate_Rep")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in IMonitor
    ''' Text: "LBL_SRV_POSITION"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblRotorPosition() As String
        Get
            Return GetText("LBL_SRV_POSITION")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in IMonitor
    ''' Text: "PMD_TUBE_CONTENTS_REAGENT"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblReagent() As String
        Get
            Return GetText("PMD_TUBE_CONTENTS_REAGENT")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in IMonitor
    ''' Text: "LBL_Solution"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblSolution() As String
        Get
            Return GetText("LBL_Solution")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in IMonitor
    ''' Text: "LBL_RotorPos_WashingSol"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblWashingSolution() As String
        Get
            Return GetText("LBL_RotorPos_WashingSol")
        End Get
    End Property

    ''' <summary>
    ''' Used for compatibility with code in IMonitor
    ''' Text: "PMD_TUBE_CONTENTS_SPEC_SOL"
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property lblSpecialSolution() As String
        Get
            Return GetText("PMD_TUBE_CONTENTS_SPEC_SOL")
        End Get
    End Property
#End Region

#Region "Format definitions"
    ''' <summary>
    ''' Defined for compatibility with code in IMonitor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property S_NO_VOLUME_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}, {4}: {5}", _
                                 lblSampleClass, "{0}", _
                                 lblName, "{1}", _
                                 lblRotorPosition, "{2}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in IMonitor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property S_NO_VOLUME_BLANK_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}, {4}: {5}", _
                                 lblSampleClass, "{0}", _
                                 lblSolution, "{1}", _
                                 lblRotorPosition, "{2}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in IMonitor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property PREP_LOCKED_Format() As String
        Get
            Return String.Format("- {0}, {1}, {2}: {3}", _
                                 "{0}", "{1}", _
                                 lblTest, "{2}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in IMonitor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property PREP_LOCKED_BLANK_Format() As String
        Get
            Return String.Format("- {0}, {1}: {2}, {3}: {4}", _
                                 "{0}", lblSolution, _
                                 "{1}", lblTest, "{2}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in IMonitor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property PREP_WITH_CLOT_Format() As String
        Get
            Return String.Format("- {0}, {1}, {2}: {3}, {4}: {5}", _
                                 "{0}", "{1}", _
                                 lblTest, "{2}", _
                                 lblReplicateNumber, "{3}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in IMonitor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property PREP_WITH_CLOT_BLANK_Format() As String
        Get
            Return String.Format("- {0}, {1}: {2}, {3}: {4}, {5}: {6}", _
                                 "{0}", lblSolution, _
                                 "{1}", lblTest, _
                                 "{2}", lblReplicateNumber, _
                                 "{3}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in IMonitor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property R_NO_VOLUME_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}", _
                                 lblReagent, "{0}", _
                                 lblRotorPosition, "{1}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in IMonitor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property WASH_SOL_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}", _
                                 lblWashingSolution, "{0}", _
                                 lblRotorPosition, "{1}")
        End Get
    End Property

    ''' <summary>
    ''' Defined for compatibility with code in IMonitor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Protected ReadOnly Property SPEC_SOL_Format() As String
        Get
            Return String.Format("- {0}: {1}, {2}: {3}", _
                                 lblSpecialSolution, "{0}", _
                                 lblRotorPosition, "{1}")
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
    ''' Gets the multilanguage text by its key
    ''' Only reads from DataBase once for key
    ''' </summary>
    ''' <param name="pKey"></param>
    ''' <returns></returns>
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
        Dim text As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, pKey, currentLanguage)
        If String.IsNullOrEmpty(text) Then text = "*" & pKey
        If mTextDict.ContainsKey(pKey) Then
            mTextDict.Item(pKey) = text
        Else
            mTextDict.Add(pKey, text)
        End If
    End Sub
#End Region

#Region " Initializations "
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
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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
            MyBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            MyBase.ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
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
    ''' Load all the Analyzers
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' Modified by: IR 04/10/2012
    ''' </remarks>
    Private Sub GetAnalyzerList()
        Dim myAnalyzerDelegate As New AnalyzersDelegate
        Dim myGlobalDataTO As New GlobalDataTO
        'Dim myAnalyzerData As AnalyzersDS
        Dim myAnalyzerData As List(Of String)
        Try
            'Begin IR 04/10/2012 Let the user select more than one analyzer if available. We must read data from thisWSAnalyzerAlarmsDAO
            'myGlobalDataTO = myAnalyzerDelegate.GetAllAnalyzers(Nothing)
            myGlobalDataTO = myAnalyzerDelegate.GetDistinctAnalyzers(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                'myAnalyzerData = DirectCast(myGlobalDataTO.SetDatos, AnalyzersDS)
                myAnalyzerData = DirectCast(myGlobalDataTO.SetDatos, List(Of String))
                'mAnalyzers = (From a In myAnalyzerData.tcfgAnalyzers _
                '              Where Not a.Generic _
                '              Order By a.Active Descending _
                '              Select a.AnalyzerID).ToList

                Dim o As String
                For Each o In myAnalyzerData
                    mAnalyzers.Add(o.ToString)
                Next

                'mAnalyzers = myGlobalDataTO.SetDatos
            End If
            'End IR 04/10/2012

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".GetAnalyzerList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".GetAnalyzerList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    '''  Fills the DropDownLists
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Sub FillDropDownLists()
        GetAlarmTypes()
        GetAnalyzerList()

        bsTypeComboBox.DataSource = mAlarmTypes.Values.ToList
        bsAnalyzerIDComboBox.DataSource = mAnalyzers

        bsAnalyzerIDComboBox.Enabled = mAnalyzers.Count > 1
        bsAnalyzerIDComboBox.Visible = bsAnalyzerIDComboBox.Enabled 'JB 16/10/2012 Not visible if only one analyzer
        bsAnalyzerIDLabel.Visible = bsAnalyzerIDComboBox.Visible 'JB 16/10/2012 Not visible if only one analyzer
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
            Dim auxIconName As String = ""

            'ERROR Image
            auxIconName = GetIconName("WARNINGSMALL")
            If Not String.IsNullOrEmpty(auxIconName) Then ERRORImage = preloadedDataConfig.GetIconImage("WARNINGSMALL")

            'WARNING Image
            auxIconName = GetIconName("STUS_WITHERRS")
            If Not String.IsNullOrEmpty(auxIconName) Then WARNINGImage = preloadedDataConfig.GetIconImage("STUS_WITHERRS")

            'SOLVED ALARM Image
            SOLVEDimage = NoImage
            auxIconName = GetIconName("SOLVEDSMALL")
            If Not String.IsNullOrEmpty(auxIconName) Then SOLVEDimage = preloadedDataConfig.GetIconImage("SOLVEDSMALL")

            'WARNING Disable Image 
            auxIconName = GetIconName("WRNGSMLDIS")
            If Not String.IsNullOrEmpty(auxIconName) Then SOLVEDErrorImage = preloadedDataConfig.GetIconImage("WRNGSMLDIS")

            'ALERT Disable Image 
            auxIconName = GetIconName("ALRTSMLDIS")
            If Not String.IsNullOrEmpty(auxIconName) Then SOLVEDWarningImage = preloadedDataConfig.GetIconImage("ALRTSMLDIS")

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetPreloadedImages ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetPreloadedImages ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
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
            With xtraHistoryGrid.EmbeddedNavigator.Buttons
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
                .Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() _
                            {TypeColumn, DateColumn, TimeColumn, _
                             NameColumn, DescriptionColumn, SolutionColumn, AnalyzerColumn})

                .OptionsView.AllowCellMerge = False
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
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeAlarmsHistoryGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeAlarmsHistoryGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)
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

            If bsAnalyzerIDComboBox.Items.Count > 0 Then
                bsAnalyzerIDComboBox.SelectedIndex = 0
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeFilterSearch ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeFilterSearch", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    '''  Gets all label texts for Alarms
    ''' </summary>
    ''' <remarks>
    ''' Created by:  JB 28/09/2012 (Addapted from IMonitorAlarmsTab.vb -> GetAlarmsTabLabels() )
    ''' </remarks>
    Private Sub InitializeAlarmTexts()
        Try
            SampleClassDict("BLANK") = GetText("PMD_SAMPLE_CLASSES_BLANK")
            SampleClassDict("CALIB") = GetText("PMD_SAMPLE_CLASSES_CALIB")
            SampleClassDict("CTRL") = GetText("PMD_SAMPLE_CLASSES_CTRL")
            SampleClassDict("PATIENT") = GetText("PMD_SAMPLE_CLASSES_PATIENT")

            BlankModeDict("SALINESOL") = GetText("PMD_SPECIAL_SOLUTIONS_SALINESOL")
            BlankModeDict("DISTW") = GetText("PMD_SPECIAL_SOLUTIONS_DISTW")
            BlankModeDict("REAGENT") = GetText("PMD_BLANK_MODES_REAGENT") 'AG 29/01/2014 - #1479 3b) Blank only with reagent

            'SGM 06/09/2012 - Automation of filling Solution Codes dictionary
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myPreloadedMasterDataDelegate As New PreloadedMasterDataDelegate

            'Dilution Solution Codes
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.DIL_SOLUTIONS)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myDilutionCodesMasterDataDS As PreloadedMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                For Each S As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myDilutionCodesMasterDataDS.tfmwPreloadedMasterData
                    If Not SolutionCodeDict.ContainsKey(S.ItemID.ToUpper) Then
                        SolutionCodeDict(S.ItemID.ToUpper) = S.FixedItemDesc
                    End If
                Next
            End If

            'Special Solution Codes
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.SPECIAL_SOLUTIONS)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim mySpecialCodesMasterDataDS As PreloadedMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                For Each S As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In mySpecialCodesMasterDataDS.tfmwPreloadedMasterData
                    If Not SolutionCodeDict.ContainsKey(S.ItemID.ToUpper) Then
                        SolutionCodeDict(S.ItemID.ToUpper) = S.FixedItemDesc
                    End If
                Next
            End If

            'Washing Solution Codes
            myGlobalDataTO = myPreloadedMasterDataDelegate.GetList(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.WASHING_SOLUTIONS)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myWashingCodesMasterDataDS As PreloadedMasterDataDS = CType(myGlobalDataTO.SetDatos, PreloadedMasterDataDS)
                For Each S As PreloadedMasterDataDS.tfmwPreloadedMasterDataRow In myWashingCodesMasterDataDS.tfmwPreloadedMasterData
                    If Not SolutionCodeDict.ContainsKey(S.ItemID.ToUpper) Then
                        SolutionCodeDict(S.ItemID.ToUpper) = S.FixedItemDesc
                    End If
                Next
            End If
            'end SGM 06/09/2012


        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeAlarmTexts ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeAlarmTexts", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", MsgParent)

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
            Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage.Trim.ToString()


            'SGM 31/05/2013 - Get Level of the connected User
            Dim MyGlobalBase As New GlobalBase
            CurrentUserLevel = MyGlobalBase.GetSessionInfo().UserLevel
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
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".InitializeScreen ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' created by SG 31/05/2013
    ''' </remarks>
    Private Sub ScreenStatusByUserLevel()
        Try
            Select Case CurrentUserLevel    '.ToUpper()
                Case "SUPERVISOR"
                    Exit Select

                Case "OPERATOR"
                    bsHistoryDelete.Enabled = False
                    Exit Select
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ScreenStatusByUserLevel ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ScreenStatusByUserLevel ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region " Search "
    ''' <summary>
    ''' Returns the selected search filter
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSearchFilter() As SearchFilter
        Dim filter As New SearchFilter
        With filter
            .analyzerId = bsAnalyzerIDComboBox.SelectedItem.ToString

            If bsDateFromDateTimePick.Checked Then
                .dateFrom = bsDateFromDateTimePick.Value
            Else
                .dateFrom = Nothing
            End If
            If bsDateToDateTimePick.Checked Then
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
        Dim myGlobalDataTO As New GlobalDataTO
        Dim myHisWSAnalyzerAlarmsDS As HisWSAnalyzerAlarmsDS

        xtraHistoryGrid.DataSource = Nothing
        Try
            If bsAnalyzerIDComboBox.Items.Count > 0 Then

                Dim filter As SearchFilter = GetSearchFilter()

                myGlobalDataTO = myWSAlarmHistory.GetAlarmsMonitor(Nothing, filter.analyzerId, filter.dateFrom, filter.dateTo, filter.alarmType)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myHisWSAnalyzerAlarmsDS = DirectCast(myGlobalDataTO.SetDatos, HisWSAnalyzerAlarmsDS)

                    PrepareAndSetDataToGrid(myHisWSAnalyzerAlarmsDS.vwksAlarmsMonitor)
                End If
            End If
            UpdateFormBehavior()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".FindHistoricalResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".FindHistoricalResults ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Decodes the image of the alarm type
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <remarks>
    ''' Created by: JB 28/09/2012 (addapted from IMonitorAlarmsTab.vb -> UpdateAlarmsTab() )
    ''' </remarks>
    Private Sub DecodeAlarmTypeImage(ByRef pRow As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow)
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            Select Case pRow.AlarmType
                Case "ERROR"
                    If pRow.AlarmStatus Then
                        pRow.AlarmTypeImage = ERRORImage
                    Else
                        pRow.AlarmTypeImage = SOLVEDErrorImage
                    End If
                Case "WARNING"
                    If pRow.AlarmStatus Then
                        pRow.AlarmTypeImage = WARNINGImage
                    Else
                        pRow.AlarmTypeImage = SOLVEDWarningImage
                    End If
                Case Else
                    pRow.AlarmTypeImage = NoImage
            End Select

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DecodeAlarmTypeImage ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DecodeAlarmTypeImage ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Decodes the alarm description
    ''' </summary>
    ''' <param name="pRow"></param>
    ''' <remarks>
    ''' Created by:  JB 28/09/2012 (addapted from IMonitorAlarmsTab.vb -> UpdateAlarmsTab() )
    ''' Modified by: JV 27/01/2014 #1463
    ''' </remarks>
    Private Sub DecodeAlarmDescription(ByRef pRow As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow)
        Dim myGlobalDataTO As New GlobalDataTO
        Dim additionalInfoDS As WSAnalyzerAlarmsDS
        Dim additionalInfo As String

        Try
            'RH 30/01/2012 Decode Additional Info
            If Not String.IsNullOrEmpty(pRow.AdditionalInfo) Then
                'SGM 30/07/2012 ISE aditional Info
                If pRow.AlarmID = Alarms.ISE_CALIB_ERROR.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_A.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_B.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_C.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_D.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_S.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_F.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_M.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_N.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_R.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_W.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_P.ToString Or _
                   pRow.AlarmID = Alarms.ISE_ERROR_T.ToString Then
                    'SGM 27/07/2012
                    Dim additionalISEInfoDS As WSAnalyzerAlarmsDS

                    myGlobalDataTO = myWSAlarmsDelegate.DecodeISEAdditionalInfo(pRow.AlarmID, pRow.AdditionalInfo, pRow.AnalyzerID)
                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        additionalISEInfoDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzerAlarmsDS)

                        For Each adISERow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In additionalISEInfoDS.twksWSAnalyzerAlarms.Rows
                            If adISERow.AdditionalInfo.Trim.Length > 0 Then
                                additionalInfo = adISERow.AlarmID & " " & adISERow.AdditionalInfo
                                pRow.Description &= Environment.NewLine & additionalInfo
                            Else
                                pRow.Description = adISERow.AlarmID
                            End If
                        Next

                    End If
                ElseIf pRow.AlarmID = Alarms.WS_PAUSE_MODE_WARN.ToString Then 'JV 27/01/2014 #1463
                    pRow.Description &= pRow.AdditionalInfo
                Else
                    myGlobalDataTO = myWSAlarmsDelegate.DecodeAdditionalInfo(pRow.AlarmID, pRow.AdditionalInfo)
                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        Dim adRow As WSAnalyzerAlarmsDS.AdditionalInfoPrepLockedRow

                        additionalInfoDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzerAlarmsDS)

                        adRow = additionalInfoDS.AdditionalInfoPrepLocked(0)

                        If String.Equals(adRow.SampleClass, "CALIB") Then
                            If String.Equals(adRow.NumberOfCalibrators, "1") Then
                                Name = adRow.Name
                            Else
                                Name = String.Format("{0}-{1}", adRow.Name, adRow.MultiItemNumber)
                            End If
                        Else
                            Name = adRow.Name
                        End If

                        additionalInfo = String.Empty

                        Select Case pRow.AlarmID
                            Case GlobalEnumerates.Alarms.S_NO_VOLUME_WARN.ToString()
                                If String.Equals(adRow.SampleClass, "BLANK") Then
                                    additionalInfo = String.Format(S_NO_VOLUME_BLANK_Format, _
                                                                   SampleClassDict(adRow.SampleClass), _
                                                                   BlankModeDict(Name), _
                                                                   adRow.RotorPosition)
                                Else
                                    additionalInfo = String.Format(S_NO_VOLUME_Format, _
                                                                   SampleClassDict(adRow.SampleClass), _
                                                                   Name, _
                                                                   adRow.RotorPosition)
                                End If

                            Case GlobalEnumerates.Alarms.PREP_LOCKED_WARN.ToString()
                                If String.Equals(adRow.SampleClass, "BLANK") Then
                                    additionalInfo = String.Format(PREP_LOCKED_BLANK_Format, _
                                                                   SampleClassDict(adRow.SampleClass), _
                                                                   BlankModeDict(Name), _
                                                                   adRow.TestName) ', adRow.ReplicateNumber) 'AG 18/06/2012 - do not shown the replicate number
                                Else
                                    additionalInfo = String.Format(PREP_LOCKED_Format, _
                                                                   SampleClassDict(adRow.SampleClass), _
                                                                   Name, _
                                                                   adRow.TestName) ', adRow.ReplicateNumber)'AG 18/06/2012 - do not shown the replicate number
                                End If

                                'AG 25/07/2012
                            Case GlobalEnumerates.Alarms.CLOT_DETECTION_ERR.ToString(), GlobalEnumerates.Alarms.CLOT_DETECTION_WARN.ToString()
                                If String.Equals(adRow.SampleClass, "BLANK") Then
                                    additionalInfo = String.Format(PREP_WITH_CLOT_BLANK_Format, _
                                                                   SampleClassDict(adRow.SampleClass), _
                                                                   BlankModeDict(Name), _
                                                                   adRow.TestName, adRow.ReplicateNumber)
                                Else
                                    additionalInfo = String.Format(PREP_WITH_CLOT_Format, _
                                                                   SampleClassDict(adRow.SampleClass), _
                                                                   Name, _
                                                                   adRow.TestName, adRow.ReplicateNumber)
                                End If
                                'AG 25/07/2012

                            Case GlobalEnumerates.Alarms.R1_NO_VOLUME_WARN.ToString(), _
                                 GlobalEnumerates.Alarms.R2_NO_VOLUME_WARN.ToString()
                                'RH 18/05/2012
                                Select Case adRow.TubeContent
                                    Case "SPEC_SOL"
                                        additionalInfo = String.Format(SPEC_SOL_Format, _
                                                                       SolutionCodeDict(adRow.SolutionCode), _
                                                                       adRow.RotorPosition)
                                    Case "WASH_SOL"
                                        additionalInfo = String.Format(WASH_SOL_Format, _
                                                                       SolutionCodeDict(adRow.SolutionCode), _
                                                                       adRow.RotorPosition)
                                    Case Else
                                        additionalInfo = String.Format(R_NO_VOLUME_Format, _
                                                                       adRow.TestName, _
                                                                       adRow.RotorPosition)
                                End Select
                                'RH 18/05/2012 - END
                        End Select

                        pRow.Description &= Environment.NewLine & additionalInfo
                    End If
                End If
            End If
            'END RH 30/01/2012
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DecodeAlarmDescription ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DecodeAlarmDescription ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Prepare the DataSet with friendly data (as Monitor tab) and sets to Grid
    ''' </summary>
    ''' <param name="pAlarmsDataTable"></param>
    ''' <remarks>
    ''' Created by: JB 28/09/2012
    ''' </remarks>
    Private Sub PrepareAndSetDataToGrid(ByVal pAlarmsDataTable As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorDataTable)
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            For Each row As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow In pAlarmsDataTable
                DecodeAlarmTypeImage(row)
                DecodeAlarmDescription(row)
            Next

            xtraHistoryGrid.DataSource = pAlarmsDataTable

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
    ''' Created by: JB 01/10/2012
    ''' </remarks>
    Private Sub UpdateFormBehavior()
        Try
            bsHistoryDelete.Enabled = xtraHistoryGridView.SelectedRowsCount > 0
            bsSearchGroup.Enabled = bsAnalyzerIDComboBox.Items.Count > 0
            ScreenStatusByUserLevel()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".UpdateFormBehavior ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".UpdateFormBehavior ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Delete all selected rows in the grid
    ''' </summary>
    ''' <remarks>
    ''' Created by: JB 02/10/2012
    ''' Modified by: JV 27/01/2014 #1463
    ''' </remarks>
    Private Sub DeleteSelectedRowsFromGrid(ByVal pGrid As DevExpress.XtraGrid.Views.Grid.GridView)
        Dim myGlobalDataTO As New GlobalDataTO

        Try
            If pGrid.SelectedRowsCount = 0 Then Exit Sub

            'Show delete confirmation message 
            If (ShowMessage(Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.DELETE_CONFIRMATION.ToString) <> Windows.Forms.DialogResult.Yes) Then Exit Sub

            Dim myHisWSAnalyzerAlarmDS As New HisWSAnalyzerAlarmsDS
            Dim vAlarmMonitorRow As HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow
            'For each row, creates a new "HistoryAlarm"
            For i As Integer = 0 To pGrid.SelectedRowsCount() - 1
                If (pGrid.GetSelectedRows()(i) >= 0) Then
                    vAlarmMonitorRow = DirectCast(pGrid.GetDataRow(pGrid.GetSelectedRows()(i)), HisWSAnalyzerAlarmsDS.vwksAlarmsMonitorRow)
                    With vAlarmMonitorRow
                        If .IsOKDateTimeNull Then .OKDateTime = Nothing

                        'Creates the new row HisWSAnalyzerAlarmsDS
                        myHisWSAnalyzerAlarmDS.thisWSAnalyzerAlarms.AddthisWSAnalyzerAlarmsRow(.AlarmID, _
                                                                                               .AnalyzerID, _
                                                                                               .AlarmDateTime, _
                                                                                               .AlarmItem, _
                                                                                               .AlarmType, _
                                                                                               .WorkSessionID, _
                                                                                               .AdditionalInfo, _
                                                                                               .AlarmStatus, _
                                                                                               .OKDateTime,
                                                                                               Nothing) 'JV 27/01/2014 #1463
                    End With
                End If
            Next
            myHisWSAnalyzerAlarmDS.thisWSAnalyzerAlarms.AcceptChanges()

            myGlobalDataTO = myWSAlarmHistory.Delete(Nothing, myHisWSAnalyzerAlarmDS)

            FindHistoricalResults()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".DeleteSelectedRowsFromGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".DeleteSelectedRowsFromGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region
#End Region

#Region "Events"
#Region " Screen Events "
    Private Sub IHisAlarms_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'If (Not bsRejectionNumeric.Focused AndAlso Not bsNumberOfSeriesNumeric.Focused) Then
                '    bsExitButton.PerformClick()

                'ElseIf (bsRejectionNumeric.Focused AndAlso bsRejectionNumeric.Text = String.Empty) Then
                '    bsRejectionNumeric.Text = bsRejectionNumeric.Value.ToString()
                '    bsResultErrorProv.Clear()

                'ElseIf (bsNumberOfSeriesNumeric.Focused AndAlso bsNumberOfSeriesNumeric.Text = String.Empty) Then
                '    bsNumberOfSeriesNumeric.Text = bsNumberOfSeriesNumeric.Value.ToString()
                '    bsResultErrorProv.Clear()
                'Else
                bsExitButton.PerformClick()
                'End If
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
            'If (bsTestSampleListView.Items.Count > 0) Then
            '    If (QCTestSampleIDAttribute > 0) Then
            '        'Select the Test/SampleType on the ListView
            '        SelectQCTestSampleID(QCTestSampleIDAttribute)
            '    Else
            '        'Select the first Test/SampleType on the ListView
            '        bsTestSampleListView.Items(0).Selected = True
            '    End If

            '    'TR 20/04/2012 - Validate the user level to activate/desactivate functionalities
            '    ScreenStatusByUserLevel()
            'End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".IHisAlarms_Shown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".IHisAlarms_Shown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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
                IAx00MainMDI.OpenMonitorForm(Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".ExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".ExitButton_Click ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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