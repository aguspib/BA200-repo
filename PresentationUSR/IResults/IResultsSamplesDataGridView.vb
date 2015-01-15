Option Explicit On
'Option Strict On

Imports Biosystems.Ax00.BL
'Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
'Imports Biosystems.Ax00.DAL
'Imports Biosystems.Ax00.Controls.UserControls
'Imports Biosystems.Ax00.Calculations 'AG 26/07/2010
'Imports Biosystems.Ax00.CommunicationsSwFw

Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.Utils


Partial Class IResults

#Region "SamplesXtraGrid Methods"
    ''' <summary>
    ''' Creates and initializes the SamplesXtraGrid
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 24/10/2011
    ''' Modified by: SGM 17/06/2013 - new hidden column for inform SpecimenId in the group
    ''' </remarks>
    Private Sub InitializeSamplesXtraGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            SamplesXtraGridView.Columns.Clear()

            Dim GroupColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            'Dim CollapseColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim OkColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim HISSentColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim NewRepColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim ClassColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim NameColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim SampleTypeColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            'Dim RerunColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim GraphColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim NoColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim ABSValueColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim ConcentrationColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim UnitColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim RefRangesColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim RemarksColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim ResultDateColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim RepColumn As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim SortingCol As New DevExpress.XtraGrid.Columns.GridColumn()
            Dim SpecimenIdCol As New DevExpress.XtraGrid.Columns.GridColumn() 'SGM 17/06/2013 - new hidden column for inform SpecimenId in the group

            ', RerunColumn, CollapseColumn

            SamplesXtraGridView.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() _
                        {GroupColumn, OkColumn, SortingCol, _
                         HISSentColumn, NewRepColumn, ClassColumn, _
                         NameColumn, SampleTypeColumn, GraphColumn, _
                         NoColumn, ABSValueColumn, ConcentrationColumn, UnitColumn, _
                         RefRangesColumn, RemarksColumn, ResultDateColumn, RepColumn})

            SamplesXtraGridView.Images = XtraGridIconList
            SamplesXtraGridView.OptionsView.AllowCellMerge = True
            SamplesXtraGridView.OptionsView.GroupDrawMode = GroupDrawMode.Default
            SamplesXtraGridView.OptionsView.ShowGroupedColumns = True
            SamplesXtraGridView.OptionsView.ColumnAutoWidth = False
            SamplesXtraGridView.OptionsView.RowAutoHeight = True
            SamplesXtraGridView.OptionsView.ShowIndicator = False

            SamplesXtraGridView.ColumnPanelRowHeight = 30

            SamplesXtraGridView.Appearance.GroupRow.BackColor = Color.WhiteSmoke 'AverageBkColor
            SamplesXtraGridView.Appearance.GroupRow.ForeColor = Color.Black 'AverageForeColor
            SamplesXtraGridView.Appearance.FocusedCell.BackColor = Color.Transparent
            SamplesXtraGridView.Appearance.GroupButton.BackColor = Color.Transparent

            SamplesXtraGridView.OptionsHint.ShowColumnHeaderHints = False

            SamplesXtraGridView.OptionsBehavior.Editable = False
            SamplesXtraGridView.OptionsBehavior.ReadOnly = True
            SamplesXtraGridView.OptionsCustomization.AllowFilter = False
            SamplesXtraGridView.OptionsCustomization.AllowSort = False

            SamplesXtraGridView.OptionsMenu.EnableColumnMenu = False

            SamplesXtraGridView.GroupCount = 1

            'GroupColumn. Rows will be grouped by this column/field
            GroupColumn.FieldName = "Group"
            GroupColumn.Name = "Group"
            GroupColumn.Visible = False
            'GroupColumn.Width = 40
            'GroupColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.True
            'GroupColumn.GroupInterval = ColumnGroupInterval.DisplayText
            GroupColumn.GroupIndex = 0
            GroupColumn.SortMode = ColumnSortMode.DisplayText
            'GroupColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            'GroupColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            'OkColumn
            OkColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_OK", LanguageID)
            OkColumn.FieldName = "Ok"
            OkColumn.Name = "Ok"
            OkColumn.Visible = True
            'OkColumn.Width = 35
            OkColumn.Width = 40
            OkColumn.OptionsColumn.AllowSize = True
            OkColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            OkColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
            OkColumn.ImageIndex = 1
            OkColumn.ImageAlignment = StringAlignment.Near
            'OkColumn.Fixed = FixedStyle.Left

            'SortingCol. Rows will be sorted by this column/field
            SortingCol.FieldName = "_SortingCol"
            SortingCol.Name = "_SortingCol"
            SortingCol.Visible = False
            SortingCol.SortMode = ColumnSortMode.DisplayText
            SortingCol.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            XtraCollapseColName = OkColumn.Name

            'HISSent column
            HISSentColumn.Caption = MyClass.LISNameForColumnHeaders ' String.Empty SGM 12/04/2013
            HISSentColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            HISSentColumn.FieldName = "ExportStatus"
            HISSentColumn.Name = "ExportStatus"
            HISSentColumn.Visible = True
            HISSentColumn.Width = 33
            HISSentColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
            'HISSentColumn.ImageIndex = 0
            'HISSentColumn.ImageAlignment = StringAlignment.Center
            HISSentColumn.OptionsColumn.AllowSize = True
            'HISSentColumn.OptionsColumn.ShowCaption = False 'SGM 12/04/2013
            HISSentColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            HISSentColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            'NewRepColumn
            NewRepColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_New", LanguageID)
            NewRepColumn.FieldName = "NewRep"
            NewRepColumn.Name = "NewRep"
            NewRepColumn.Visible = True
            NewRepColumn.Width = 45
            'NewRepColumn.ImageAlignment = StringAlignment.Center
            NewRepColumn.OptionsColumn.AllowSize = True
            NewRepColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            NewRepColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            'ClassColumn
            ClassColumn.Caption = String.Empty
            ClassColumn.FieldName = "StatFlag"
            ClassColumn.Name = "Class"
            ClassColumn.Visible = True
            ClassColumn.Width = 30
            ClassColumn.OptionsColumn.AllowSize = True
            ClassColumn.OptionsColumn.ShowCaption = False
            ClassColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            ClassColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            ''NameColumn
            'NameColumn.Caption = labelPatient
            'NameColumn.FieldName = "PatientName"
            'NameColumn.Name = "PatientName"
            'NameColumn.Visible = True
            'NameColumn.Width = 140
            'NameColumn.OptionsColumn.AllowSize = True
            'NameColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True
            'NameColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            'NameColumn
            NameColumn.Caption = labelPatient
            NameColumn.FieldName = "PatientID"
            NameColumn.Name = "PatientID"
            NameColumn.Visible = True
            NameColumn.Width = 140
            NameColumn.OptionsColumn.AllowSize = True
            NameColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True
            NameColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            'SampleTypeColumn
            SampleTypeColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", LanguageID)
            SampleTypeColumn.FieldName = "SampleType"
            SampleTypeColumn.Name = "SampleType"
            SampleTypeColumn.Visible = True
            SampleTypeColumn.Width = 45
            SampleTypeColumn.OptionsColumn.AllowSize = True
            SampleTypeColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            SampleTypeColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            ''RerunColumn
            'RerunColumn.Caption = String.Empty
            'RerunColumn.FieldName = "RerunNumber"
            'RerunColumn.Name = "Rerun"
            'RerunColumn.Visible = True
            'RerunColumn.Width = 50
            'RerunColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            'RerunColumn.OptionsColumn.AllowSize = True
            'RerunColumn.OptionsColumn.ShowCaption = False
            'RerunColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True
            'RerunColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            'GraphColumn
            GraphColumn.Caption = String.Empty
            GraphColumn.FieldName = "Graph"
            GraphColumn.Name = "Graph"
            GraphColumn.Visible = True
            GraphColumn.Width = 30
            GraphColumn.OptionsColumn.AllowSize = True
            GraphColumn.OptionsColumn.ShowCaption = False
            GraphColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            GraphColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False

            'NoColumn
            NoColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", LanguageID)
            NoColumn.FieldName = "ReplicateNumber"
            NoColumn.Name = "No"
            NoColumn.Visible = True
            NoColumn.Width = 33
            NoColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far
            NoColumn.OptionsColumn.AllowSize = True
            NoColumn.OptionsColumn.AllowMerge = DefaultBoolean.False
            NoColumn.OptionsColumn.AllowSort = DefaultBoolean.False

            'ABSValueColumn
            ABSValueColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", LanguageID)
            ABSValueColumn.FieldName = "ABSValue"
            ABSValueColumn.Name = "ABSValue"
            ABSValueColumn.Visible = True
            ABSValueColumn.Width = 55
            ABSValueColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far
            ABSValueColumn.OptionsColumn.AllowSize = True
            ABSValueColumn.OptionsColumn.AllowMerge = DefaultBoolean.False
            ABSValueColumn.OptionsColumn.AllowSort = DefaultBoolean.False

            'ConcentrationColumn
            ConcentrationColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", LanguageID)
            ConcentrationColumn.FieldName = "Concentration"
            ConcentrationColumn.Name = "Concentration"
            ConcentrationColumn.Visible = True
            ConcentrationColumn.Width = 65
            ConcentrationColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far
            ConcentrationColumn.OptionsColumn.AllowSize = True
            ConcentrationColumn.OptionsColumn.AllowMerge = DefaultBoolean.False
            ConcentrationColumn.OptionsColumn.AllowSort = DefaultBoolean.False

            'UnitColumn
            UnitColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", LanguageID)
            UnitColumn.FieldName = "Unit"
            UnitColumn.Name = "Unit"
            UnitColumn.Visible = True
            UnitColumn.Width = 55
            UnitColumn.OptionsColumn.AllowSize = True
            UnitColumn.OptionsColumn.AllowMerge = DefaultBoolean.True
            UnitColumn.OptionsColumn.AllowSort = DefaultBoolean.False

            'RefRangesColumn
            RefRangesColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Short", LanguageID)
            RefRangesColumn.FieldName = "ReferenceRanges"
            RefRangesColumn.Name = "ReferenceRanges"
            RefRangesColumn.Visible = True
            RefRangesColumn.Width = 100
            RefRangesColumn.OptionsColumn.AllowSize = True
            RefRangesColumn.OptionsColumn.AllowMerge = DefaultBoolean.True
            RefRangesColumn.OptionsColumn.AllowSort = DefaultBoolean.False

            'RemarksColumn
            RemarksColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", LanguageID)
            RemarksColumn.FieldName = "Remarks"
            RemarksColumn.Name = "Remarks"
            RemarksColumn.Visible = True
            RemarksColumn.Width = 300
            'RemarksColumn.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
            RemarksColumn.OptionsColumn.AllowSize = True
            RemarksColumn.OptionsColumn.AllowMerge = DefaultBoolean.False
            RemarksColumn.OptionsColumn.AllowSort = DefaultBoolean.False

            'ResultDateColumn
            ResultDateColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", LanguageID)
            ResultDateColumn.FieldName = "ResultDate"
            ResultDateColumn.Name = "ResultDate"
            ResultDateColumn.Visible = True
            ResultDateColumn.Width = 130
            ResultDateColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center
            ResultDateColumn.OptionsColumn.AllowSize = True
            ResultDateColumn.OptionsColumn.AllowMerge = DefaultBoolean.False
            ResultDateColumn.OptionsColumn.AllowSort = DefaultBoolean.False

            'RepColumn
            RepColumn.Caption = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITTLE_RepetitionMode", LanguageID)
            RepColumn.FieldName = "Rep"
            RepColumn.Name = "Rep"
            RepColumn.Visible = True
            RepColumn.Width = 200
            'RepColumn.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
            RepColumn.OptionsColumn.AllowSize = True
            RepColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            RepColumn.OptionsColumn.AllowSort = DefaultBoolean.False

            'SpecimenIdCol 
            'SGM 17/06/2013 - new hidden column for inform SpecimenId in the group
            SpecimenIdCol.Caption = ""
            SpecimenIdCol.FieldName = "SpecimenIdList"
            SpecimenIdCol.Name = "SpecimenIdList"
            SpecimenIdCol.Visible = False
            SpecimenIdCol.Width = 0
            SpecimenIdCol.OptionsColumn.AllowSize = False
            SpecimenIdCol.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False
            SpecimenIdCol.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False
            'end SGM 17/06/2013

            'SamplesXtraGridView

            'For showing an image on cells
            Dim pictureEdit As DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit = _
                    TryCast(SamplesXtraGrid.RepositoryItems.Add("PictureEdit"),  _
                    DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit)

            pictureEdit.SizeMode = PictureSizeMode.Clip
            pictureEdit.NullText = " "

            OkColumn.ColumnEdit = pictureEdit
            GraphColumn.ColumnEdit = pictureEdit
            ClassColumn.ColumnEdit = pictureEdit
            NewRepColumn.ColumnEdit = pictureEdit
            HISSentColumn.ColumnEdit = pictureEdit


            'For wrapping remmark text
            Dim largeTextEdit As DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit = _
                    TryCast(SamplesXtraGrid.RepositoryItems.Add("MemoEdit"),  _
                    DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit)

            'largeTextEdit.AutoHeight = True
            'largeTextEdit.WordWrap = True

            RemarksColumn.ColumnEdit = largeTextEdit

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeSamplesXtraGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Defines the SamplesDataGridView column names to be sorted
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 24/10/2011
    ''' </remarks>
    Private Sub DefineXtraSamplesSortedColumns()
        Try
            'Insert here the column names to be sorted
            SampleSortType("PatientID") = SortType.ASC
            SampleSortType("SampleType") = SortType.ASC
            SampleSortType("ResultDate") = SortType.ASC

            'SamplesXtraGridView.CanSortColumn(SamplesXtraGridView.Columns("PatientName"))
            'SamplesXtraGridView.CanSortColumn(SamplesXtraGridView.Columns("SampleType"))
            'SamplesXtraGridView.CanSortColumn(SamplesXtraGridView.Columns("ResultDate"))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " DefineSamplesSortedColumns ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Creates a new Average List for a given Test
    ''' </summary>
    ''' <param name="TestName"></param>
    ''' <param name="CalcTestInserted"></param>
    ''' <param name="ISEOFFSTestInserted"></param>
    ''' <param name="PatientInserted"></param>
    ''' <remarks>
    ''' Created by: RH 24/10/2011
    ''' </remarks>
    Private Sub CreateAverageList(ByVal TestName As String, ByRef CalcTestInserted As Dictionary(Of String, Boolean), _
                                  ByRef ISEOFFSTestInserted As Dictionary(Of String, Boolean), _
                                  ByRef PatientInserted As Dictionary(Of String, Boolean))
        Try
            SamplesAverageList = New List(Of ResultsDS.vwksResultsRow)

            CalcTestInserted = New Dictionary(Of String, Boolean)
            ISEOFFSTestInserted = New Dictionary(Of String, Boolean)
            PatientInserted = New Dictionary(Of String, Boolean)

            For Each row As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                If String.Equals(row.TestName, TestName) AndAlso Not String.Equals(row.PatientID, String.Empty) _
                   AndAlso (Not row.IsCONC_ValueNull OrElse Not row.IsManualResultTextNull) Then
                    If String.Equals(row.TestType, "STD") Then
                        Dim myOrderTestID As Integer = row.OrderTestID
                        Dim maxTheoreticalConc As Single = _
                            (From resultRow In AverageResultsDS.vwksResults _
                             Where resultRow.OrderTestID = myOrderTestID _
                             Select resultRow.TheoricalConcentration).Max

                        If row.TheoricalConcentration = maxTheoreticalConc Then SamplesAverageList.Add(row)
                        'AG 01/12/2010
                    ElseIf String.Equals(row.TestType, "ISE") OrElse String.Equals(row.TestType, "OFFS") Then
                        'AG 14/01/2011 - original code fails when several reruns ... <AG 22/12/2010>
                        ''If Not ISEOFFSTestInserted.ContainsKey(row.TestName) Then
                        'If ISEOFFSTestInserted.ContainsKey(row.TestName) And PatientInserted.ContainsKey(row.PatientName) Then
                        'Dim pat_RerunID As String = ""
                        'pat_RerunID = row.PatientName & "<RerunNumber: " & row.RerunNumber & ">"
                        'Dim pat_RerunID As String = String.Format("{0}<RerunNumber: {1}>", row.PatientID, row.RerunNumber)

                        'RH 13/12/2011
                        Dim pat_RerunID As String = String.Format("{0}<RerunNumber: {1}><SampleType: {2}>", _
                                                                  row.PatientID, row.RerunNumber, row.SampleType)

                        If ISEOFFSTestInserted.ContainsKey(row.TestName) AndAlso PatientInserted.ContainsKey(pat_RerunID) Then
                            'Nothing: Test-Patient already added
                        Else
                            ISEOFFSTestInserted(row.TestName) = True
                            'PatientInserted(row.PatientName) = True 'AG 22/12/2010
                            PatientInserted(pat_RerunID) = True 'AG 14/01/2011
                            SamplesAverageList.Add(row)
                        End If
                        'End AG 01/12/2010

                    Else 'It is CALC
                        'AG 22/12/2010
                        'If Not CalcTestInserted.ContainsKey(row.TestName) Then
                        If CalcTestInserted.ContainsKey(row.TestName) AndAlso PatientInserted.ContainsKey(row.PatientID) Then
                            ''Nothing: Test-Patient already added
                        Else
                            CalcTestInserted(row.TestName) = True
                            PatientInserted(row.PatientID) = True 'AG 22/12/2010
                            SamplesAverageList.Add(row)
                        End If
                    End If
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CreateAverageList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub


    ''' <summary>
    ''' Prepare and fill the results view (Tests -> Patients)
    ''' </summary>
    ''' <remarks>
    ''' Created RH - v1.0.0
    ''' Modified AG + DL 14/06/2013 (show barcode when informed in v2.0.0)
    ''' Modified AG 28/06/2013 - fix issue #1199 (show barcode if informed also for calculated and offsystem tests)
    ''' </remarks>
    Private Sub UpdateSamplesXtraGrid()
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Dim StartTime As DateTime = Now
        Dim myLogAcciones As New ApplicationLogManager()
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Try
            If isClosingFlag Then Exit Sub ' XB 24/02/2014 - #1496 No refresh if screen is closing

            If String.Equals(SampleTestName, TestsListViewText) Then Return
            'Dim StartTime As DateTime = Now

            Dim TestName As String = TestsListViewText

            SampleTestName = TestName

            Dim dgv As DevExpress.XtraGrid.GridControl = SamplesXtraGrid
            Dim refreshModeFlag As Boolean = False 'AG 21/06/2012
            If Not copyRefreshDS Is Nothing Then refreshModeFlag = True 'AG 21/06/2012

            Dim Remark As String = Nothing
            Dim CalcTestInserted As Dictionary(Of String, Boolean) = Nothing
            Dim ISEOFFSTestInserted As Dictionary(Of String, Boolean) = Nothing
            Dim PatientInserted As Dictionary(Of String, Boolean) = Nothing

            CreateAverageList(TestName, CalcTestInserted, ISEOFFSTestInserted, PatientInserted)

            If SamplesAverageList.Count = 0 Then
                SamplesXtraGrid.DataSource = Nothing
                Return
            End If

            Dim IsAverageDone As New Dictionary(Of String, Boolean)

            Dim MaxRows As Integer = 0
            Dim TagRowIndex As Integer = 0
            Dim CurrentRow As ResultsDS.XtraSamplesRow
            Dim AverageRow As ResultsDS.XtraSamplesRow
            Dim Filter As String = String.Empty
            Dim sentRepetitionCriteria As String = String.Empty

            Dim GroupCode As Integer = 0

            dgv.DataSource = Nothing

            tblXtraSamples = New ResultsDS.XtraSamplesDataTable()

            HeadersCount = 0
            'TR 07/07/2012
            Dim myLetter As String = String.Empty
            'TR 10/07/2012 -Declaration outside the for.
            Dim OrderTestID As Integer = 0
            Dim myRerunNumber As Integer = 0
            Dim RerunList As New List(Of Integer)
            Dim hasConcentrationError As Boolean = False
            'TR 02/08/2012 -Declare outside the for
            Dim MaxRerunNumber As Integer = RerunList.Count
            Dim SampleList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
            'TR 02/08/2012 -END
            Dim myResultsDlg As New ResultsDelegate 'AG 28/06/2013
            Dim auxGlobal As New GlobalDataTO 'AG 28/06/2013

            For Each resultRow As ResultsDS.vwksResultsRow In SamplesAverageList
                Filter = String.Format("{0}{1}", resultRow.OrderTestID, resultRow.RerunNumber)

                If Not IsAverageDone.ContainsKey(Filter) Then

                    IsAverageDone(Filter) = True

                    'RH 23/04/2012
                    RerunList = (From row In AverageResultsDS.vwksResults _
                                 Where row.OrderTestID = resultRow.OrderTestID _
                                 Select row.RerunNumber Distinct).ToList()

                    'Dim MaxRerunNumber As Integer = RerunList.Count
                    MaxRerunNumber = RerunList.Count

                    'END RH 23/04/2012

                    GroupCode += 1

                    'TR 10/07/2012 -Set value and Declare Outside for.
                    OrderTestID = resultRow.OrderTestID

                    'Dim OrderTestID As Integer = resultRow.OrderTestID

                    CurrentRow = tblXtraSamples.NewXtraSamplesRow()

                    AverageRow = CurrentRow

                    CurrentRow.Group = GroupCode.ToString()

                    'IMPORTANT! TagRowIndex here points to a row inside AverageList
                    'That way, each DataGridView row can access the whole info behind.
                    CurrentRow.TagRowIndex = TagRowIndex

                    CurrentRow.IsSubHeader = True
                    HeadersCount += 1

                    'CurrentRow.Ok = resultRow.AcceptedResultFlag
                    If resultRow.AcceptedResultFlag Then
                        CurrentRow.Ok = OKImage
                    Else
                        CurrentRow.Ok = UnCheckImage
                    End If

                    sentRepetitionCriteria = String.Empty 'RH 25/05/2012
                    If AllowManualRepetitions(resultRow, MaxRerunNumber, "PATIENTS", sentRepetitionCriteria) Then
                        SetRepPostDilutionImage(CurrentRow, resultRow.PostDilutionType, True)
                    Else
                        SetRepPostDilutionImage(CurrentRow, sentRepetitionCriteria, False)
                    End If

                    CurrentRow.SampleType = resultRow.SampleType
                    CurrentRow.RerunNumber = resultRow.RerunNumber

                    If Not resultRow.IsExportStatusNull Then
                        If String.Equals(resultRow.ExportStatus, "SENT") Then
                            CurrentRow.ExportStatus = LISSubHeaderImage
                            'Else
                            '    CurrentRow.ExportStatus = NoImage
                        End If
                        'Else
                        '    CurrentRow.ExportStatus = NoImage
                    End If

                    If Not resultRow.IsABSValueNull Then
                        CurrentRow.ABSValue = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                    Else
                        CurrentRow.ABSValue = String.Empty
                    End If

                    If Not resultRow.IsCONC_ValueNull Then
                        'AG 02/08/2010
                        'dgv("Concentration", i).Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)

                        hasConcentrationError = False 'TR 10/07/2012 -Set the Value and Declare Outside.
                        'Dim hasConcentrationError As Boolean = False

                        If Not resultRow.IsCONC_ErrorNull Then 'RH 14/09/2010
                            hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error)
                        End If

                        If Not hasConcentrationError Then
                            CurrentRow.Concentration = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                        Else
                            CurrentRow.Concentration = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                        End If
                        'dgv("Concentration", MaxRows).Style.Alignment = DataGridViewContentAlignment.MiddleRight
                        'END AG 02/08/2010
                    Else
                        If (Not resultRow.IsManualResultTextNull) Then
                            CurrentRow.Concentration = resultRow.ManualResultText
                            'dgv("Concentration", MaxRows).Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                        Else
                            CurrentRow.Concentration = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                            'dgv("Concentration", MaxRows).Style.Alignment = DataGridViewContentAlignment.MiddleRight
                        End If
                    End If

                    CurrentRow.Unit = resultRow.MeasureUnit

                    'dgv("ReferenceRanges", i).Value = GlobalConstants.TO_DO  'AG 02/08/2010
                    If Not String.IsNullOrEmpty(resultRow.NormalLowerLimit) AndAlso _
                       Not String.IsNullOrEmpty(resultRow.NormalUpperLimit) Then
                        CurrentRow.ReferenceRanges = String.Format("{0} - {1}", _
                                                     resultRow.NormalLowerLimit, resultRow.NormalUpperLimit)
                    End If

                    'AG 15/09/2010 - Special case when Absorbance has error
                    If Not resultRow.IsABS_ErrorNull Then
                        If Not String.IsNullOrEmpty(resultRow.ABS_Error) Then
                            CurrentRow.ABSValue = GlobalConstants.ABSORBANCE_ERROR
                            CurrentRow.Concentration = GlobalConstants.CONC_DUE_ABS_ERROR
                        End If
                    End If
                    'END AG 15/09/2010

                    Remark = GetResultAlarmDescription(resultRow.OrderTestID, resultRow.RerunNumber, _
                                                                            resultRow.MultiPointNumber)
                    CurrentRow.Remarks = Remark

                    'If Remark <> String.Empty Then CurrentRow.ReplicateNumber = "*"

                    'TR 07/06/2012 Set the leter to concentration
                    If Not String.Equals(Remark, String.Empty) Then
                        'Validate if there is a range alarm to replace the asterisk by the corresponding letter.
                        myLetter = ""
                        myLetter = GetRangeAlarmsLetter(resultRow.OrderTestID, resultRow.RerunNumber, _
                                                                            resultRow.MultiPointNumber)
                        If String.Equals(myLetter, String.Empty) Then
                            CurrentRow.ReplicateNumber = "*"
                        Else
                            CurrentRow.ReplicateNumber = myLetter
                        End If
                    End If
                    'TR 07/06/2012 -END.

                    If String.Equals(resultRow.TestType, "STD") Then
                        CurrentRow.Graph = AVG_ABS_GRAPHImage
                        'Else
                        '    CurrentRow.Graph = NoImage
                    End If

                    CurrentRow.ResultDate = resultRow.ResultDateTime.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                 resultRow.ResultDateTime.ToString(SystemInfoManager.OSLongTimeFormat)

                    CurrentRow.PatientName = resultRow.PatientName
                    CurrentRow.PatientID = IIf(resultRow.RerunNumber = 1, resultRow.PatientID, String.Format("{0} ({1})", resultRow.PatientID, resultRow.RerunNumber))
                    CurrentRow.TestType = resultRow.TestType 'RH 23/03/2012

                    'AG + DL 14/06/2013 - show specimen id in the Group 
                    myRerunNumber = resultRow.RerunNumber
                    SampleList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                  Where row.OrderTestID = OrderTestID _
                                  AndAlso row.RerunNumber = myRerunNumber _
                                  Select row).ToList()
                    If SampleList.Count > 0 AndAlso Not SampleList(0).IsSpecimenIDListNull AndAlso Not SampleList(0).SpecimenIDList = String.Empty Then
                        'CurrentRow.PatientID &= " (" & SampleList(0).SpecimenIDList & ")"
                        CurrentRow.SpecimenIDList = SampleList(0).SpecimenIDList 'SGM 17/06/2013 - inform SpecimenIDList
                    End If
                    'AG + DL 14/06/2013

                    'AG 28/06/2013 - Patch for the test types without executions
                    If (CurrentRow.TestType = "CALC" OrElse CurrentRow.TestType = "OFFS") AndAlso CurrentRow.IsSpecimenIDListNull Then
                        auxGlobal = myResultsDlg.FillSpecimenIDList(Nothing, WorkSessionIDField, CurrentRow)
                    End If

                    tblXtraSamples.AddXtraSamplesRow(CurrentRow)

                    'AG + DL 14/06/2013 - move upper
                    ''TR 10/07/2012 -Set value and declare Outside.
                    'myRerunNumber = resultRow.RerunNumber
                    ''Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010

                    ''Dim SampleList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                    'SampleList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                    '              Where row.OrderTestID = OrderTestID _
                    '              AndAlso row.RerunNumber = myRerunNumber _
                    '              Select row).ToList()
                    'AG + DL 14/06/2013

                    If SampleList.Count > 0 Then

                        SetPostDilutionText(CurrentRow, SampleList(0).PostDilutionType, SampleList(0).RerunNumber)

                        For j As Integer = 0 To SampleList.Count - 1
                            'Dim k As Integer = j + MaxRows + 1

                            CurrentRow = tblXtraSamples.NewXtraSamplesRow()
                            CurrentRow.Group = GroupCode.ToString()

                            'IMPORTANT! TagRowIndex here points to a row inside ExecutionsResultsDS.vwksWSExecutionsResults.
                            'That way, each DataGridView row can access the whole info behind.
                            CurrentRow.TagRowIndex = SampleList(j).RowIndex 'RH 10/04/2012  'SampleList(j).ExecutionID - 1

                            CurrentRow.IsSubHeader = False

                            CurrentRow.Rep = String.Empty
                            'CurrentRow.Ok = NoImage
                            'CurrentRow.NewRep = NoImage
                            CurrentRow.PatientName = SampleList(j).PatientName
                            'CurrentRow.PatientName = IIf(resultRow.RerunNumber = 1, resultRow.PatientName, String.Format("{0} ({1})", resultRow.PatientName, resultRow.RerunNumber))
                            CurrentRow.PatientID = SampleList(j).PatientID
                            CurrentRow.SampleType = SampleList(j).SampleType
                            CurrentRow.InUse = SampleList(j).InUse

                            If SampleList(j).StatFlag Then
                                CurrentRow.StatFlag = PATIENT_STATImage
                            Else
                                CurrentRow.StatFlag = PATIENT_ROUTINEImage
                            End If

                            CurrentRow.RerunNumber = SampleList(j).RerunNumber

                            'AG 22/12/2010 - ISE results show also replicates
                            If String.Equals(SampleList(j).ExecutionType, "PREP_STD") Then
                                CurrentRow.Graph = ABS_GRAPHImage
                                'dgv("Graph", k).ToolTipText = labelOpenAbsorbanceCurve
                                CurrentRow.ABSValue = SampleList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                'Else
                                '    CurrentRow.Graph = NoImage
                                '    'dgv("Graph", k).ToolTipText = String.Empty
                                '    CurrentRow.ABSValue = Nothing
                            End If
                            'AG 22/12/2010

                            'AG 02/08/2010
                            'dgv("Concentration", k).Value = SampleList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)

                            hasConcentrationError = False 'TR 10/07/2012 Set value declare outside
                            'Dim hasConcentrationError As Boolean = False

                            If Not SampleList(j).IsCONC_ErrorNull Then 'RH 14/09/2010
                                hasConcentrationError = Not String.IsNullOrEmpty(SampleList(j).CONC_Error)
                            End If

                            If Not hasConcentrationError Then
                                If (Not SampleList(j).IsCONC_ValueNull) Then
                                    CurrentRow.Concentration = SampleList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                    'dgv("Concentration", k).Style.Alignment = DataGridViewContentAlignment.MiddleRight
                                    'ElseIf (Not SampleList(j).IsManualResultTextNull) Then
                                ElseIf Not resultRow.IsManualResultTextNull Then
                                    'dgv("Concentration", k).Value = SampleList(j).ManualResultText
                                    'RH 31/01/2011
                                    'Take the Manual Result text from the average result
                                    CurrentRow.Concentration = resultRow.ManualResultText
                                    'dgv("Concentration", k).Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                                Else
                                    CurrentRow.Concentration = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                    'dgv("Concentration", k).Style.Alignment = DataGridViewContentAlignment.MiddleRight
                                End If
                            Else
                                CurrentRow.Concentration = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                'dgv("Concentration", k).Style.Alignment = DataGridViewContentAlignment.MiddleRight
                            End If
                            'END AG 02/08/2010

                            'AG 15/09/2010 - Special case when Absorbance has error
                            If Not SampleList(j).IsABS_ErrorNull Then
                                If Not String.IsNullOrEmpty(SampleList(j).ABS_Error) Then
                                    CurrentRow.ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                    CurrentRow.Concentration = GlobalConstants.CONC_DUE_ABS_ERROR
                                End If
                            End If
                            'END AG 15/09/2010

                            'RH 01/02/2011 Get alarms for OffSystem tests and the others. OffSystem test do not have valid ExecutionID
                            If Not String.Equals(resultRow.TestType, "OFFS") Then
                                Remark = GetExecutionAlarmDescription(SampleList(j).ExecutionID)
                            Else
                                Remark = GetResultAlarmDescription(SampleList(j).OrderTestID, 1, 1)
                            End If

                            CurrentRow.Remarks = Remark

                            'RH 03/08/2011
                            If Not String.IsNullOrEmpty(Remark) Then
                                'CurrentRow.ReplicateNumber = "* " & SampleList(j).ReplicateNumber

                                'TR 07/06/2012 -Set the letter to the replicates
                                myLetter = ""
                                'Validate if there is a range alarm to replace the asterisk by the corresponding letter.
                                myLetter = GetReplicatesRangeAlarmsLetter(SampleList(j).ExecutionID)
                                If String.Equals(myLetter, String.Empty) Then
                                    CurrentRow.ReplicateNumber = "* " & SampleList(j).ReplicateNumber
                                Else
                                    CurrentRow.ReplicateNumber = SampleList(j).ReplicateNumber & " " & myLetter
                                End If
                                'TR(07/06 /2012)

                            Else
                                CurrentRow.ReplicateNumber = SampleList(j).ReplicateNumber
                            End If
                            'dgv("No", k).ToolTipText = Remark
                            'RH 03/08/2011

                            CurrentRow.ResultDate = SampleList(j).ResultDate.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                         SampleList(j).ResultDate.ToString(SystemInfoManager.OSLongTimeFormat)

                            CurrentRow.RerunNumber = AverageRow.RerunNumber
                            CurrentRow.ExportStatus = AverageRow.ExportStatus
                            CurrentRow.Unit = AverageRow.Unit '& Modifier
                            CurrentRow.ReferenceRanges = AverageRow.ReferenceRanges '& Modifier

                            CurrentRow.TestType = resultRow.TestType 'RH 23/03/2012

                            CurrentRow.SpecimenIDList = AverageRow.SpecimenIDList 'SGM 17/06/2013

                            tblXtraSamples.AddXtraSamplesRow(CurrentRow)
                        Next

                        'dgv("PatientName", MaxRows).Value = resultRow.PatientName
                        'dgv("Class", MaxRows).Value = dgv("Class", MaxRows + 1).Value
                        AverageRow.StatFlag = CurrentRow.StatFlag

                        'If Not IsColCollapsed.ContainsKey(TestsListViewText & "_PATIENT") Then
                        AverageRow.Collapsed = resultRow.Collapsed
                        'End If

                    Else
                        'CType(dgv(CollapseColName, MaxRows), bsDataGridViewCollapseCell).IsEnabled = False
                        SetPostDilutionText(CurrentRow, "EQUAL_REP", CurrentRow.RerunNumber)

                        'AG 02/12/2010 - Show the proper icon
                        If resultRow.StatFlag Then
                            CurrentRow.StatFlag = PATIENT_STATImage
                        Else
                            CurrentRow.StatFlag = PATIENT_ROUTINEImage
                        End If
                        'END AG 02/12/2010
                    End If

                    MaxRows += SampleList.Count + 1
                End If

                TagRowIndex += 1
            Next

            UpdateSubHeaders = True
            HeaderIndex = 0

            dgv.DataSource = tblXtraSamples

            'RH 15/12/2011 Sort Average by PatientName, so the related info will be displayed together
            SortXtraGridView(SamplesXtraGridView, "PatientName", "_SortingCol", SortType.ASC)

            'Dim ElapsedTime As Double = Now.Subtract(StartTime).TotalMilliseconds
            'MessageBox.Show("UpdateXtraSamplesDataGrid Elapsed Time: " & ElapsedTime.ToStringWithDecimals(0))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " UpdateSamplesGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        myLogAcciones.CreateLogActivity("IResults UpdateSamplesXtraGrid (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0) & _
                                        " OPEN TAB: " & bsTestDetailsTabControl.SelectedTab.Name, _
                                        "IResults.UpdateSamplesXtraGrid", EventLogEntryType.Information, False)
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
    End Sub

#End Region

#Region "XtraGridView Events"

    ''' <summary>
    ''' Modifies the RowStyle of the currently painted XtraGridView Row
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: RH 09/11/2011
    ''' </remarks>
    Private Sub SamplesXtraGridView_RowStyle(ByVal sender As System.Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs) Handles SamplesXtraGridView.RowStyle
        Try
            Dim View As GridView = sender

            If (e.RowHandle >= 0) Then
                'This is how to get the DataRow behind the GridViewRow
                Dim CurrentRow As ResultsDS.XtraSamplesRow = CType(View.GetDataRow(e.RowHandle), ResultsDS.XtraSamplesRow) 'tblXtraSamples(e.RowHandle)

                If CurrentRow.IsSubHeader Then
                    'Set SubHeader color
                    e.Appearance.BackColor = AverageBkColor
                    e.Appearance.ForeColor = AverageForeColor
                Else 'Replicate
                    'Dim ExecutionsResultsRow As ExecutionsDS.vwksWSExecutionsResultsRow
                    'ExecutionsResultsRow = ExecutionsResultsDS.vwksWSExecutionsResults(CurrentRow.TagRowIndex)

                    'Set Not In Use color and striked font
                    If Not CurrentRow.InUse Then
                        e.Appearance.Font = XtraStrikeFont
                        e.Appearance.BackColor = StrikeBkColor
                        e.Appearance.ForeColor = StrikeForeColor
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SamplesXtraGridView_RowStyle ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Tells a XtraGridView if two cells should be merged or not
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 09/11/2011
    ''' </remarks>
    Private Sub SamplesXtraGridView_CellMerge(ByVal sender As System.Object, ByVal e As DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs) Handles SamplesXtraGridView.CellMerge
        Try
            Dim View As GridView = sender

            Dim CurrentRow1 As ResultsDS.XtraSamplesRow = CType(View.GetDataRow(e.RowHandle1), ResultsDS.XtraSamplesRow) 'tblXtraSamples(e.RowHandle1)
            Dim CurrentRow2 As ResultsDS.XtraSamplesRow = CType(View.GetDataRow(e.RowHandle2), ResultsDS.XtraSamplesRow) 'tblXtraSamples(e.RowHandle2)

            If CurrentRow1.IsSubHeader OrElse Not CurrentRow1.InUse OrElse Not CurrentRow2.InUse Then
                e.Merge = False
                e.Handled = True
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SamplesXtraGridView_CellMerge ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Keeps track of CollapseAll or ExpandAll events. Writes the Group Text.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: RH 09/11/2011
    ''' Modified by: SGM 17/06/2013 - add SpecimenID to group headers
    ''' AG 29/08/2013 - group show first the specimen ID if exists and then the patient ID - Rerun
    ''' </remarks>
    Private Sub SamplesXtraGridView_CustomDrawGroupRow(ByVal sender As Object, ByVal e As DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs) Handles SamplesXtraGridView.CustomDrawGroupRow
        Try
            Dim info As GridGroupRowInfo = TryCast(e.Info, GridGroupRowInfo)

            If info Is Nothing Then Return

            Dim View As GridView = sender

            Dim ChildHandle As Integer = View.GetChildRowHandle(e.RowHandle, 0)

            Dim CurrentRow As ResultsDS.XtraSamplesRow = CType(View.GetDataRow(ChildHandle), ResultsDS.XtraSamplesRow)

            If CurrentRow.IsSubHeader Then 'It should for shure!!!
                Dim resultRow As ResultsDS.vwksResultsRow
                resultRow = SamplesAverageList(CurrentRow.TagRowIndex)

                If UpdateSubHeaders Then
                    If resultRow.Collapsed Then
                        SamplesXtraGridView.CollapseGroupRow(e.RowHandle)
                    Else
                        SamplesXtraGridView.ExpandGroupRow(e.RowHandle)
                    End If
                Else
                    If e.RowHandle + 1 = -CurrentRow.TagRowIndex Then
                        resultRow.Collapsed = Not resultRow.Collapsed
                        If Not Me.UpdateCollapse(False, resultRow.Collapsed, Nothing, resultRow) Then
                            resultRow.Collapsed = Not resultRow.Collapsed
                            info.GroupExpanded = Not info.GroupExpanded
                        Else
                            If e.RowHandle + 1 = -ExperimentalSampleIndex Then
                                ExperimentalSampleIndex = -1
                            End If
                        End If
                    End If
                End If

                Dim tmps As System.Text.StringBuilder = New System.Text.StringBuilder()

                'SGM 17/06/2013
                'tmps.Append(CurrentRow.PatientID.PadRight(39))
                Dim myPatientSpecimen As String = CurrentRow.PatientID
                If Not String.IsNullOrEmpty(CurrentRow.SpecimenIDList) Then
                    'AG 29/08/2013 - Show first the specimen ID if informed
                    'myPatientSpecimen &= " (" & CurrentRow.SpecimenIDList & ")"
                    myPatientSpecimen = CurrentRow.SpecimenIDList
                    myPatientSpecimen &= " (" & CurrentRow.PatientID & ")" 'Between brackets add the patient Identifier
                    'AG 29/08/2013

                    If (myPatientSpecimen.Length + 3) > (39) Then
                        myPatientSpecimen = myPatientSpecimen.Substring(0, 39 - 3 - 2) + ".."
                    End If
                End If

                tmps.Append(myPatientSpecimen.PadRight(39))
                'end SGM 17/06/2013


                tmps.Append(CurrentRow.SampleType.PadRight(13))
                tmps.Append(CurrentRow.ReplicateNumber.PadRight(10))
                tmps.AppendFormat(" {0} {1}", CurrentRow.Concentration.PadLeft(8), CurrentRow.Unit)

                info.GroupText = tmps.ToString()


                HeaderIndex += 1

                If HeaderIndex >= HeadersCount Then
                    HeaderIndex = HeadersCount
                    UpdateSubHeaders = False
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SamplesXtraGridView_CustomDrawGroupRow ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Processes the mouse click event over columns headers
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: RH 09/11/2011
    ''' </remarks>
    Private Sub SamplesXtraGridView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SamplesXtraGridView.Click
        Try
            Dim EventArgs As DevExpress.Utils.DXMouseEventArgs = DirectCast(e, DevExpress.Utils.DXMouseEventArgs)

            Dim hi As GridHitInfo = SamplesXtraGridView.CalcHitInfo(EventArgs.Location)

            If hi.InColumn AndAlso (hi.RowHandle < 0) Then
                'Collapse column
                If (hi.Column.Name = "Ok") AndAlso (EventArgs.Location.X < 18) Then
                    XtraSamplesCollapsed = Not XtraSamplesCollapsed

                    If XtraSamplesCollapsed Then
                        SamplesXtraGridView.CollapseAllGroups()
                    Else
                        SamplesXtraGridView.ExpandAllGroups()
                    End If
                Else
                    'Sort column
                    If SampleSortType.ContainsKey(hi.Column.Name) Then
                        If SampleSortType(hi.Column.Name) = SortType.ASC Then
                            SampleSortType(hi.Column.Name) = SortType.DESC
                        Else
                            SampleSortType(hi.Column.Name) = SortType.ASC
                        End If

                        SortXtraGridView(SamplesXtraGridView, hi.Column.Name, "_SortingCol", SampleSortType(hi.Column.Name))
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SamplesXtraGridView_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Processes the mouse click event over the cells
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: RH 09/11/2011
    ''' </remarks>
    Private Sub SamplesXtraGridView_RowCellClick(ByVal sender As System.Object, ByVal e As DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs) Handles SamplesXtraGridView.RowCellClick
        Try
            Cursor = Cursors.WaitCursor

            Dim resultRow As ResultsDS.vwksResultsRow
            Dim key As String = String.Empty
            Dim executionRow As ExecutionsDS.vwksWSExecutionsResultsRow 'SG 30/08/2010
            'Dim ParentControl As System.Windows.Forms.Control

            If sender Is Nothing Then Return

            Dim dgv As GridView = sender

            'This is how to get the DataRow behind the GridViewRow
            Dim CurrentRow As ResultsDS.XtraSamplesRow = CType(dgv.GetDataRow(e.RowHandle), ResultsDS.XtraSamplesRow) 'tblXtraSamples(e.RowHandle)

            'RH 23/03/2012 Do nothing when TestType is OFFS
            'CurrentRow.TagRowIndex will have an invalid value
            If String.Equals(CurrentRow.TestType, "OFFS") Then Return

            Dim myGridSampleClass As String = "PATIENT"

            'key = TestsListViewText & "_PATIENT"
            'ParentControl = XtraSamplesTabPage

            Dim changeInUseValue As Boolean = False 'AG 08/08/2010

            Select Case e.Column.Name
                Case "NewRep"
                    If CurrentRow.IsSubHeader Then
                        'If Not dgv("NewRep", e.RowIndex).Value Is NoImage Then 'AG 14/08/2010 - Only the last rerun can select the new repetition criterion
                        'If Not CurrentRow.NewRep Is NoImage OrElse CurrentRow.NewRep Is INC_SENT_REPImage _
                        '        OrElse CurrentRow.NewRep Is RED_SENT_REPImage OrElse CurrentRow.NewRep Is EQ_SENT_REPImage Then
                        'If Not CurrentRow.NewRep Is Nothing Then
                        'RH 25/05/2012 Update conditions
                        If (Not CurrentRow.NewRep Is Nothing) AndAlso Not (CurrentRow.NewRep Is NoImage OrElse CurrentRow.NewRep Is INC_SENT_REPImage _
                            OrElse CurrentRow.NewRep Is RED_SENT_REPImage OrElse CurrentRow.NewRep Is EQ_SENT_REPImage) Then
                            'AG 28/07/2010
                            'Select the new repetition criterion
                            resultRow = SamplesAverageList(CurrentRow.TagRowIndex)
                            Dim manualCriteria As GlobalEnumerates.PostDilutionTypes
                            manualCriteria = Me.ShowManualRepetitions(resultRow.OrderTestID, resultRow.TestType, myGridSampleClass)
                            'RH: 27/08/2010
                            If Not manualCriteria = PostDilutionTypes.UNDEFINED Then
                                resultRow.PostDilutionType = manualCriteria.ToString()
                                SetRepPostDilutionImage(CurrentRow, resultRow.PostDilutionType, True)
                            End If
                            'END AG 28/07/2010
                        End If 'END AG 14/08/2010
                    Else
                        'AG 22/06/2012 - TO CONFIRM
                        'If mdiAnalyzerCopy.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then changeInUseValue = True 'AG 22/06/2012 - allow change in use replicate value only out of Running
                        changeInUseValue = True
                    End If

                Case "Graph"
                    'SG 30/08/2010
                    If Not CurrentRow.IsSubHeader Then '(1)
                        'If Not CurrentRow.Graph Is NoImage Then '(1.2) - AG 22/12/2010 - If no graph image then do not execute following code
                        executionRow = ExecutionsResultsDS.vwksWSExecutionsResults(CurrentRow.TagRowIndex)

                        If String.Equals(executionRow.ExecutionType, "PREP_STD") Then '(1.3) - AG 22/12/2010 - ISE results dont have graphical Abs(t)

                            ShowResultsChart(executionRow.ExecutionID, _
                                             executionRow.OrderTestID, _
                                             executionRow.RerunNumber, _
                                             executionRow.MultiItemNumber, _
                                             executionRow.ReplicateNumber)
                        End If '(1.3)
                        'End If '(1.2)

                    Else '(1)
                        'If Not CurrentRow.Graph Is NoImage Then '(2.1)
                        resultRow = SamplesAverageList(CurrentRow.TagRowIndex)
                        If String.Equals(resultRow.TestType, "STD") Then '(2.2)
                            ' DL 11/02/2011
                            ShowResultsChart(-1, resultRow.OrderTestID, resultRow.RerunNumber, resultRow.MultiPointNumber, -1)
                        End If '(2.2)

                        'End If '(2.1)
                    End If '(1)
                    'END SG 30/08/2010

                Case "Ok"
                    If Not CurrentRow.IsSubHeader Then
                        'AG 04/08/2010 - If click over the header ... change accepted result
                        'AG 22/06/2012 - TO CONFIRM
                        'If mdiAnalyzerCopy.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then changeInUseValue = True 'AG 22/06/2012 - allow change in use replicate value only out of Running
                        changeInUseValue = True

                    Else
                        resultRow = SamplesAverageList(CurrentRow.TagRowIndex)
                        If Not resultRow.AcceptedResultFlag Then
                            'AG 22/06/2012 - TO CONFIRM
                            'If mdiAnalyzerCopy.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then ChangeAcceptedResult(resultRow, myGridSampleClass) 'AG 22/06/2012 -	Do not allow change accepted results in RUNNING
                            'ChangeAcceptedResult(resultRow, myGridSampleClass)
                            ChangeAcceptedResultNEW(resultRow)
                        Else
                            'If myGridSampleClass = "PATIENT" OrElse myGridSampleClass = "CTRL" Then
                            RejectResult(resultRow, myGridSampleClass) 'AG 17/02/2011 - Reject results only for patients
                            'End If

                        End If
                        'END AG 04/08/2010
                    End If

                Case Else
                    If Not CurrentRow.IsSubHeader Then
                        'AG 22/06/2012 - TO CONFIRM
                        'If mdiAnalyzerCopy.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then changeInUseValue = True 'AG 22/06/2012 - allow change in use replicate value only out of Running
                        changeInUseValue = True
                    End If
            End Select

            If changeInUseValue Then ChangeInUseFlagReplicate(CurrentRow)

        Catch ex As Exception
            Cursor = Cursors.Default
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".SamplesXtraGridView_RowCellClick ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Processes the mouse leave event over XtraGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 09/11/2011
    ''' </remarks>
    Private Sub SamplesXtraGridView_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SamplesXtraGridView.MouseLeave
        Try
            Cursor = Cursors.Default

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SamplesXtraGridView_MouseLeave ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Processes the mouse over event over XtraGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 09/11/2011
    ''' Modified by: AG 29/08/2013 - Group shows first the SpecimenID (if it is informed) and then the PatientID - RerunNumber
    '''              SA 16/06/2014 - BT #1667 ==> Changes in the construction of ToolTips with Patient data:
    '''                                           ** If First Character of PatientName is "-" and Last Character of PatientName is "-", remove both
    ''' </remarks>
    Private Sub ToolTipController1_GetActiveObjectInfo(ByVal sender As System.Object, ByVal e As ToolTipControllerGetActiveObjectInfoEventArgs) Handles ToolTipController1.GetActiveObjectInfo
        Try
            If Not e.SelectedControl Is SamplesXtraGrid Then Return

            'Get the view at the current mouse position
            Dim view As GridView = SamplesXtraGrid.GetViewAt(e.ControlMousePosition)
            If view Is Nothing Then Return

            'Get the view's element information that resides at the current position
            Dim hi As GridHitInfo = view.CalcHitInfo(e.ControlMousePosition)


            If hi.InRowCell Then
                'Dim o As Object = hi.HitTest.ToString() + hi.RowHandle.ToString()
                'Dim text As String = "Row " + hi.RowHandle.ToString()
                'info = New ToolTipControlInfo(o, text)

                'This is how to get the DataRow behind the GridViewRow
                Dim CurrentRow As ResultsDS.XtraSamplesRow = CType(view.GetDataRow(hi.RowHandle), ResultsDS.XtraSamplesRow) 'tblXtraSamples(hi.RowHandle)

                Select Case hi.Column.Name
                    Case "NewRep"
                        'If CurrentRow.IsSubHeader AndAlso Not CurrentRow.NewRep Is Nothing Then
                        'RH 25/05/2012 Update conditions
                        If (Not CurrentRow.NewRep Is Nothing) AndAlso Not (CurrentRow.NewRep Is NoImage OrElse CurrentRow.NewRep Is INC_SENT_REPImage _
                            OrElse CurrentRow.NewRep Is RED_SENT_REPImage OrElse CurrentRow.NewRep Is EQ_SENT_REPImage) Then
                            Cursor = Cursors.Hand
                        Else
                            Cursor = Cursors.Default
                        End If

                    Case "Ok"
                        If CurrentRow.IsSubHeader Then
                            Cursor = Cursors.Hand
                        Else
                            Cursor = Cursors.Default
                        End If

                    Case "Graph"
                        'If Not CurrentRow.Graph Is NoImage Then
                        If Not CurrentRow.Graph Is Nothing Then
                            Cursor = Cursors.Hand
                            e.Info = New ToolTipControlInfo(labelOpenAbsorbanceCurve, labelOpenAbsorbanceCurve)
                        Else
                            Cursor = Cursors.Default
                        End If

                    Case "No"
                        e.Info = New ToolTipControlInfo(CurrentRow.Remarks, CurrentRow.Remarks)

                    Case "PatientID"
                        Dim myTooltipText As String = CurrentRow.PatientID.Trim

                        'BT #1667 - Get value of Patient First and Last Name (if both of them have a hyphen as value, ignore these 
                        '           fields and shown only the PatientID
                        Dim hyphenIndex As Integer = -1
                        Dim myPatientName As String = CurrentRow.PatientName.Trim

                        If (CurrentRow.PatientName.Trim.StartsWith("-") AndAlso CurrentRow.PatientName.Trim.EndsWith("-")) Then
                            hyphenIndex = CurrentRow.PatientName.Trim.IndexOf("-")
                            myPatientName = CurrentRow.PatientName.Trim.Remove(hyphenIndex, 1)

                            hyphenIndex = myPatientName.Trim.LastIndexOf("-")
                            myPatientName = myPatientName.Trim.Remove(hyphenIndex, 1)
                        End If

                        If (Not String.IsNullOrEmpty(CurrentRow.SpecimenIDList)) Then
                            'When informed, show first the SpecimenID and then the PatientID between brackets
                            myTooltipText = CurrentRow.SpecimenIDList
                            myTooltipText &= " (" & CurrentRow.PatientID & ")"
                        End If

                        'BT #1667 - Do not add the hyphen also when the PatientName is not informed
                        If (CurrentRow.PatientID.Trim <> myPatientName.Trim AndAlso myPatientName.Trim <> String.Empty) Then
                            myTooltipText &= " - " & myPatientName
                        End If
                        e.Info = New ToolTipControlInfo(CurrentRow.PatientName, myTooltipText)

                    Case Else
                        Cursor = Cursors.Default
                End Select

                If CurrentRow.IsSubHeader Then
                    Dim resultRow As ResultsDS.vwksResultsRow
                    resultRow = SamplesAverageList(CurrentRow.TagRowIndex)

                    Select Case hi.Column.Name
                        Case "ExportStatus"
                            'ExportStatus ToolTip
                            If Not resultRow.IsExportStatusNull Then
                                If resultRow.ExportStatus = "SENT" Then
                                    e.Info = New ToolTipControlInfo(labelHISSent, labelHISSent)
                                Else
                                    e.Info = New ToolTipControlInfo(labelHISNOTSent, labelHISNOTSent)
                                End If
                            Else
                                e.Info = New ToolTipControlInfo(labelHISNOTSent, labelHISNOTSent)
                            End If

                    End Select
                End If

            ElseIf (Not hi.InRow) AndAlso (Not hi.Column Is Nothing) AndAlso (SampleSortType.ContainsKey(hi.Column.Name)) Then
                Cursor = Cursors.Hand
            Else
                Cursor = Cursors.Default

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ToolTipController1_GetActiveObjectInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Paints the column headers, including the "Collapse" column image (+, -)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Created by: RH 09/11/2011
    ''' </remarks>
    Private Sub SamplesXtraGridView_CustomDrawColumnHeader(ByVal sender As Object, ByVal e As ColumnHeaderCustomDrawEventArgs) Handles SamplesXtraGridView.CustomDrawColumnHeader
        Try
            If Not SampleSortType.ContainsKey(e.Column.Name) Then
                'How to disable the column headers hottracking
                'http://www.devexpress.com/Support/Center/e/E427.aspx
                e.Info.State = DevExpress.Utils.Drawing.ObjectState.Normal
            End If

            If e.Column Is Nothing OrElse e.Column.Name <> XtraCollapseColName Then Return

            'Draw the column header
            e.Painter.DrawObject(e.Info)

            'paint Caption
            'e.Appearance.DrawString(e.Cache, e.Column.GetTextCaption(), e.Bounds)

            If Not e.Bounds.IsEmpty Then
                'Perform custom painting
                DrawPlusOrMinusImage(e.Graphics, e.Bounds)
            End If

            e.Handled = True

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SamplesXtraGridView_CustomDrawColumnHeader ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Paints the "Collapse" column image (+, -) plus the righ hand border
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 09/11/2011
    ''' </remarks>
    Private Sub DrawPlusOrMinusImage(ByVal g As Graphics, ByVal r As Rectangle)
        Dim i As Integer = 0

        'DL 27/02/2012. AndAlso CollapseIconList.Images.Count > 1
        If Not XtraSamplesCollapsed AndAlso CollapseIconList.Images.Count > 1 Then
            i = 1
        End If

        'r.X + (r.Width - XtraGridIconList.ImageSize.Width) \ 2 
        g.DrawImageUnscaled(CollapseIconList.Images(i), _
                            r.X + 5, _
                            r.Y + (r.Height - CollapseIconList.ImageSize.Height) \ 2)

        'Draw right hand border
        g.DrawImageUnscaled(XtraVerticalBar, r.X + 15, r.Y)
    End Sub

    ''' <summary>
    ''' Sorts a XtraGridView on a given column and sort type (ascending or descending)
    ''' Uses a temporal column for storing fake values.
    ''' </summary>
    ''' <param name="dgv"></param>
    ''' <param name="ColName"></param>
    ''' <param name="Asc"></param>
    ''' <remarks>
    ''' Created by: RH - 10/11/2011
    ''' Modified by: RH 14/12/2011 Take the Group column into account, because it is relevant for sorting
    ''' </remarks>
    Private Sub SortXtraGridView(ByRef dgv As GridView, ByVal ColName As String, ByVal SortingCol As String, Optional ByVal Asc As SortType = SortType.ASC)
        Try
            If dgv Is Nothing OrElse dgv.RowCount = 0 Then Return

            Dim direction As DevExpress.Data.ColumnSortOrder
            Dim theDataView As DataView = CType(dgv.DataSource, DataView)
            Dim theTable As ResultsDS.XtraSamplesDataTable = CType(theDataView.Table, ResultsDS.XtraSamplesDataTable)

            Dim NewValue As Object = Nothing
            Dim GroupCode As Object = Nothing

            dgv.BeginSort()

            'Fill the fake column with proper data to be sorted
            If Asc = SortType.DESC Then
                direction = DevExpress.Data.ColumnSortOrder.Descending

                For i As Integer = 0 To theTable.Rows.Count - 1
                    If theTable(i).IsSubHeader Then
                        NewValue = theTable(i).Item(ColName)
                        GroupCode = NewValue.ToString() + (theTable.Rows.Count - i).ToString("0000")
                    End If

                    theTable(i).Item(SortingCol) = NewValue.ToString() + (theTable.Rows.Count - i).ToString("0000")
                    theTable(i).Item(0) = GroupCode
                Next
            Else
                direction = DevExpress.Data.ColumnSortOrder.Ascending

                For i As Integer = 0 To theTable.Rows.Count - 1
                    If theTable(i).IsSubHeader Then
                        NewValue = theTable(i).Item(ColName)
                        GroupCode = NewValue.ToString() + i.ToString("0000")
                    End If

                    theTable(i).Item(SortingCol) = NewValue.ToString() + i.ToString("0000")
                    theTable(i).Item(0) = GroupCode
                Next
            End If

            'Column(0) should be the Group column 
            dgv.SortInfo.ClearAndAddRange(New GridColumnSortInfo() _
                                          { _
                                            New GridColumnSortInfo(dgv.Columns(0), direction), _
                                            New GridColumnSortInfo(dgv.Columns(SortingCol), direction) _
                                          }, 1)

            dgv.Columns(0).Visible = False

            dgv.EndSort()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SortXtraGridView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

    Private Function ImageText() As Char
        Throw New NotImplementedException
    End Function

End Class