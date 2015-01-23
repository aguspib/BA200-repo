Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Public Class Historycal

#Region "Declaration"

    Private LanguageID As String

    Private myGloblaData As GlobalDataTO = Nothing
    Private myWSAnalyzerAlarmsDS As WSAnalyzerAlarmsDS = Nothing
    Private myWSAlarmsDelegate As New WSAnalyzerAlarmsDelegate

#End Region

#Region "Fields"

    Private AnalyzerIDField As String = String.Empty
    Private WorkSessionIDField As String = String.Empty
    Private WorkSessionStatusField As String = String.Empty

#End Region

#Region "Properties"

    Public Property ActiveAnalyzer() As String
        Get
            Return AnalyzerIDField
        End Get
        Set(ByVal value As String)
            AnalyzerIDField = value
        End Set
    End Property
    Public Property ActiveWorkSession() As String
        Get
            Return WorkSessionIDField
        End Get
        Set(ByVal value As String)
            WorkSessionIDField = value
        End Set
    End Property

    Public Property CurrentWorkSessionStatus() As String
        Get
            Return WorkSessionIDField
        End Get
        Set(ByVal value As String)
            WorkSessionStatusField = value
        End Set
    End Property

#End Region

#Region "Public Methods"


#End Region

#Region "Private Methods"

    Private Sub InitializeAlarmsTab()
        'Put your initialization code here. It will be executed in the Monitor OnLoad event
        InitializeAlarmsGrid()
        PrepareAlarmsTabButtons()

        myGloblaData = myWSAlarmsDelegate.GetAlarmsMonitor(Nothing, AnalyzerIDField)

        If Not myGloblaData.HasError Then
            myWSAnalyzerAlarmsDS = DirectCast(myGloblaData.SetDatos, WSAnalyzerAlarmsDS)
            bsAlarmsDataGridView.DataSource = myWSAnalyzerAlarmsDS.vwksAlarmsMonitor

            FillDropDownLists()
        Else
            'Do something with the error
        End If
    End Sub

    ''' <summary>
    '''  Fills the AlarmsTab DropDownLists
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 11/04/2011
    ''' </remarks>
    Private Sub FillDropDownLists()
        If Not myWSAnalyzerAlarmsDS Is Nothing Then

            Dim Types As List(Of String) = _
                (From Alarms In myWSAnalyzerAlarmsDS.vwksAlarmsMonitor _
                Select Alarms.AlarmType Distinct).ToList()
            Types.Insert(0, "All")
            bsTypeComboBox.DataSource = Types

            'Dim Actives As List(Of String) = _
            '    (From Alarms In myWSAnalyzerAlarmsDS.vwksAlarmsMonitor _
            '    Select Alarms.AlarmStatus.ToString()).ToList()
            Dim Actives As New List(Of String)
            Actives.Insert(0, "All")
            Actives.Insert(1, "True")
            Actives.Insert(2, "False")
            bsActiveComboBox.DataSource = Actives

            Dim AnalyzerIDs As List(Of String) = _
                (From Alarms In myWSAnalyzerAlarmsDS.vwksAlarmsMonitor _
                Select Alarms.AnalyzerID Distinct).ToList()
            AnalyzerIDs.Insert(0, "All")
            bsAnalyzerIDComboBox.DataSource = AnalyzerIDs

            Dim WorkSessionIDs As List(Of String) = _
                (From Alarms In myWSAnalyzerAlarmsDS.vwksAlarmsMonitor _
                Select Alarms.WorkSessionID Distinct).ToList()
            WorkSessionIDs.Insert(0, "All")
            bsWorkSessionIDComboBox.DataSource = WorkSessionIDs
        End If
    End Sub

    ''' <summary>
    '''  Initializes the bsAlarmsDataGridView DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 11/04/2011
    ''' </remarks>
    Private Sub InitializeAlarmsGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim columnName As String

            'bsAlarmsDataGridView.Rows.Clear()
            'bsAlarmsDataGridView.Columns.Clear()

            'Type column
            columnName = "Type"
            bsAlarmsDataGridView.Columns.Add(columnName, "Type")
            bsAlarmsDataGridView.Columns(columnName).DataPropertyName = "AlarmType"
            bsAlarmsDataGridView.Columns(columnName).Width = 80

            'Date column
            columnName = "Date"
            bsAlarmsDataGridView.Columns.Add(columnName, "Date")
            bsAlarmsDataGridView.Columns(columnName).DataPropertyName = "AlarmDateTime"
            bsAlarmsDataGridView.Columns(columnName).CellTemplate.Style.Format = SystemInfoManager.OSDateFormat
            bsAlarmsDataGridView.Columns(columnName).Width = 80

            'Time column
            columnName = "Time"
            bsAlarmsDataGridView.Columns.Add(columnName, "Time")
            bsAlarmsDataGridView.Columns(columnName).DataPropertyName = "AlarmDateTime"
            bsAlarmsDataGridView.Columns(columnName).CellTemplate.Style.Format = SystemInfoManager.OSLongTimeFormat
            bsAlarmsDataGridView.Columns(columnName).Width = 65

            'Status column
            'columnName = "Status"
            'bsAlarmsDataGridView.Columns.Add(columnName, "Active")
            'bsAlarmsDataGridView.Columns(columnName).DataPropertyName = "AlarmStatus"
            'bsAlarmsDataGridView.Columns(columnName).Width = 57

            'Status column
            Dim checkBoxColumnStatus As New DataGridViewCheckBoxColumn
            columnName = "Status"
            checkBoxColumnStatus.Name = columnName
            checkBoxColumnStatus.HeaderText = "Active"
            checkBoxColumnStatus.Width = 55
            checkBoxColumnStatus.DataPropertyName = "AlarmStatus"
            checkBoxColumnStatus.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            bsAlarmsDataGridView.Columns.Add(checkBoxColumnStatus)

            'Name column
            columnName = "Name"
            bsAlarmsDataGridView.Columns.Add(columnName, "Name")
            bsAlarmsDataGridView.Columns(columnName).DataPropertyName = "Name"
            bsAlarmsDataGridView.Columns(columnName).Width = 130

            'Description column
            columnName = "Description"
            bsAlarmsDataGridView.Columns.Add(columnName, "Description")
            bsAlarmsDataGridView.Columns(columnName).DataPropertyName = "Description"
            bsAlarmsDataGridView.Columns(columnName).Width = 240

            'Solution column
            columnName = "Solution"
            bsAlarmsDataGridView.Columns.Add(columnName, "Solution")
            bsAlarmsDataGridView.Columns(columnName).DataPropertyName = "Solution"
            bsAlarmsDataGridView.Columns(columnName).Width = 290

            bsAlarmsDataGridView.ScrollBars = ScrollBars.Both
            bsAlarmsDataGridView.AutoGenerateColumns = False

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeAlarmsGrid", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeAlarmsGrid", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Loads the button's images
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 11/04/2011
    ''' </remarks>
    Private Sub PrepareAlarmsTabButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'FILTER Button
            'auxIconName = GetIconName("FILTER")
            'If (auxIconName <> "") Then
            '    bsOpenSearchButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            'End If

            'CLEAR FILTER Button
            auxIconName = GetIconName("DELFILTER")
            If (auxIconName <> "") Then
                bsClearButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                ExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareAlarmsTabButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareAlarmsTabButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub splitContainerControl1_SplitterPositionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles splitContainerControl1.SplitterPositionChanged
        bsAlarmsGroupBox.Height = splitContainerControl1.SplitterPosition
        bsAlarmsDataGridView.Height = bsAlarmsGroupBox.Height - 40
    End Sub

    Private Sub splitContainerControl1_SplitGroupPanelCollapsed(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles splitContainerControl1.SplitGroupPanelCollapsed
        If splitContainerControl1.Collapsed Then
            bsAlarmsGroupBox.Height = splitContainerControl1.Height - 5
            bsAlarmsDataGridView.Height = bsAlarmsGroupBox.Height - 40
        Else
            splitContainerControl1_SplitterPositionChanged(sender, e)
        End If
    End Sub

    Private Sub ReleaseElements()
        Try
            MonitorTabs = Nothing
            MainTab = Nothing
            StatesTab = Nothing
            SamplesTab = Nothing
            ReagentsTab = Nothing
            ReactionsTab = Nothing
            ISETab = Nothing
            AlarmsTab = Nothing
            splitContainerControl1 = Nothing
            bsAlarmsGroupBox = Nothing
            bsTitleLabel = Nothing
            bsAlarmsDataGridView = Nothing
            BsGroupBox12 = Nothing
            bsMaxDateDateTimePicker = Nothing
            bsMinDateDateTimePicker = Nothing
            bsAnalyzerIDComboBox = Nothing
            bsTypeComboBox = Nothing
            bsActiveComboBox = Nothing
            bsWorkSessionIDComboBox = Nothing
            BsLabel25 = Nothing
            BsPanel8 = Nothing
            bsClearButton = Nothing
            bsMaxDateLabel = Nothing
            bsAnalyzerIDLabel = Nothing
            bsMinDateLabel = Nothing
            bsTypeLabel = Nothing
            bsActiveLabel = Nothing
            BsLabel17 = Nothing
            bsPanel2 = Nothing
            ExitButton = Nothing
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

#End Region

#Region "Events"

    Private Sub Monitor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Get the current Language from the current Application Session
        'Dim currentLanguageGlobal As New GlobalBase
        LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage

        InitializeAlarmsTab()

        ResetBorder()
    End Sub

    Private Sub RotorsTabs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MonitorTabs.SelectedPageChanged
        
    End Sub

#End Region

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click
        Close()
    End Sub
End Class