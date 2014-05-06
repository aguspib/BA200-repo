Option Explicit On
'Option Strict On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Calculations 'AG 26/07/2010
Imports Biosystems.Ax00.CommunicationsSwFw

Imports System.Text
Imports System.ComponentModel
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrintingLinks
Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraGrid.Repository
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.Utils


Partial Class IResults

#Region "BlanksDataGridView Methods"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' '<param name="pLanguageID">parameter has the current language session </param>
    ''' Modified by: PG 14/10/2010 - Add the LanguageID parameter
    ''' Modified by: RH 18/10/2010 - Remove the LanguageID parameter. Now it is a class property.
    ''' AG 03/03/2011 - view most important results without scrolling (change column witdh, ...)
    ''' </remarks>
    Private Sub InitializeBlanksGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsBlanksDataGridView.Columns.Clear()

            Dim CollapseColumn As New bsDataGridViewCollapseColumn
            With CollapseColumn
                .Name = CollapseColName
                AddHandler .HeaderClickEventHandler, AddressOf GenericDataGridView_CellMouseClick
            End With
            bsBlanksDataGridView.Columns.Add(CollapseColumn)

            'dl 23/09/2011
            Dim OkColumn As New DataGridViewImageColumn
            'Dim OkColumn As New DataGridViewCheckBoxColumn
            With OkColumn
                .Name = "Ok"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_OK", LanguageID)
                .Width = 35
            End With
            bsBlanksDataGridView.Columns.Add(OkColumn)
            'dl 23/09/2011
            'JC 12/11/2012
            Dim NewRepColumn As New DataGridViewImageColumn
            With NewRepColumn
                .Name = "NewRep"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_New", LanguageID)
                .Width = 50
            End With
            bsBlanksDataGridView.Columns.Add(NewRepColumn)

            Dim NameColumn As New DataGridViewTextBoxSpanColumn
            With NameColumn
                .Name = "TestName"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", LanguageID)
                .Width = 120
            End With
            bsBlanksDataGridView.Columns.Add(NameColumn)

            Dim RerunColumn As New DataGridViewTextBoxSpanColumn
            With RerunColumn
                .Name = "Rerun"
                '.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rerun", LanguageID)
                .Visible = False
                .Width = 50
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End With
            bsBlanksDataGridView.Columns.Add(RerunColumn)

            ' DL 27/06/2011
            'bsBlanksDataGridView.Columns.Add("SeeRems", "")
            'bsBlanksDataGridView.Columns("SeeRems").Width = 33
            'bsBlanksDataGridView.Columns("SeeRems").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            ' end DL 27/06/2011

            Dim GraphColumn As New DataGridViewImageColumn
            With GraphColumn
                .Name = "Graph"
                .HeaderText = "" 'myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Graph", LanguageID)
                .Width = 30
            End With
            bsBlanksDataGridView.Columns.Add(GraphColumn)

            'PG 14/10/2010
            'BlanksDataGridView.Columns.Add("No", "No.")
            bsBlanksDataGridView.Columns.Add("No", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", LanguageID))
            'PG 14/10/2010
            'JC 09/11/2012
            'bsControlsDataGridView.Columns("No").Width = 33
            bsBlanksDataGridView.Columns("No").Width = 33
            bsBlanksDataGridView.Columns("No").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            'PG 14/10/2010
            'BlanksDataGridView.Columns.Add("ABSValue", "Absorbance")
            bsBlanksDataGridView.Columns.Add("ABSValue", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", LanguageID))
            'PG 14/10/2010
            bsBlanksDataGridView.Columns("ABSValue").Width = 55
            bsBlanksDataGridView.Columns("ABSValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            bsBlanksDataGridView.Columns.Add("WorkReagent", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Results_WorkReagent", LanguageID))
            bsBlanksDataGridView.Columns("WorkReagent").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsBlanksDataGridView.Columns("WorkReagent").Width = 80
            bsBlanksDataGridView.Columns("WorkReagent").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            bsBlanksDataGridView.Columns.Add("KineticBlankLimit", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_KineticBlankLimit", LanguageID))
            bsBlanksDataGridView.Columns("KineticBlankLimit").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsBlanksDataGridView.Columns("KineticBlankLimit").Width = 80
            bsBlanksDataGridView.Columns("KineticBlankLimit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            bsBlanksDataGridView.Columns.Add("ABSInitial", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AbsInitial", LanguageID))
            bsBlanksDataGridView.Columns("ABSInitial").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsBlanksDataGridView.Columns("ABSInitial").Width = 80
            bsBlanksDataGridView.Columns("ABSInitial").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight


            ' JC 09/11/2012
            bsBlanksDataGridView.Columns.Add("ABSMainFilter", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AbsMainFilter", LanguageID))
            bsBlanksDataGridView.Columns("ABSMainFilter").HeaderCell.Style.WrapMode = DataGridViewTriState.True
            bsBlanksDataGridView.Columns("ABSMainFilter").Width = 80
            bsBlanksDataGridView.Columns("ABSMainFilter").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            Dim BlankAbsLimitColumn As New DataGridViewTextBoxSpanColumn
            With BlankAbsLimitColumn
                .Name = "BlankAbsLimit"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AbsLimit_Short", LanguageID)
                .Width = 80
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End With
            bsBlanksDataGridView.Columns.Add(BlankAbsLimitColumn)
            bsBlanksDataGridView.Columns("BlankAbsLimit").HeaderCell.Style.WrapMode = DataGridViewTriState.True

            'PG 14/10/2010
            'BlanksDataGridView.Columns.Add("Remarks", "Remarks")
            bsBlanksDataGridView.Columns.Add("Remarks", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", LanguageID))
            'PG 14/10/2010
            bsBlanksDataGridView.Columns("Remarks").Width = 300
            bsBlanksDataGridView.Columns("Remarks").DefaultCellStyle.WrapMode = DataGridViewTriState.True

            'PG 14/10/2010
            'BlanksDataGridView.Columns.Add("ResultDate", "Date")
            bsBlanksDataGridView.Columns.Add("ResultDate", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", LanguageID))
            'PG 14/10/2010
            bsBlanksDataGridView.Columns("ResultDate").Width = 140 'AG 15/08/2010 (120)
            bsBlanksDataGridView.Columns("ResultDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'DL 23/09/2011
            'Dim RepColumn As New DataGridViewImageColumn
            'With RepColumn
            '    .Name = "Rep"
            '    .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Replicate_Short", LanguageID)
            '    .Width = 35
            'End With
            'bsBlanksDataGridView.Columns.Add(RepColumn)

            bsBlanksDataGridView.Columns.Add("Rep", myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITTLE_RepetitionMode", LanguageID))
            bsBlanksDataGridView.Columns("Rep").Width = 200
            bsBlanksDataGridView.Columns("Rep").DefaultCellStyle.WrapMode = DataGridViewTriState.True
            'DL 23/09/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeBlanksGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Defines the BlanksDataGridView column names to be sorted
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 10/09/2010
    ''' Modified by: RH 25/11/2011
    ''' </remarks>
    Private Sub DefineBlanksSortedColumns()
        Try
            'Insert here the column names to be sorted
            BlankSortType("ResultDate") = SortType.ASC

            'For drawing the SortGlyph
            For Each Col As DataGridViewColumn In bsBlanksDataGridView.Columns
                If BlankSortType.ContainsKey(Col.Name) Then
                    Col.SortMode = DataGridViewColumnSortMode.Programmatic
                Else
                    Col.SortMode = DataGridViewColumnSortMode.NotSortable
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " DefineBlanksSortedColumns ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fills the BlanksDataGridView with test name associated data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 14/07/2010
    ''' Modified by: SA 12/11/2010 - Show Date and Time using the format defined in the OS Culture Info
    ''' </remarks>
    Private Sub UpdateBlanksDataGrid()
        If isClosingFlag Then Exit Sub ' XB 24/02/2014 - #1496 No refresh if screen is closing

        If BlankTestName = TestsListViewText Then Return

        'Dim StartTime As DateTime = Now

        Dim dgv As BSDataGridView = bsBlanksDataGridView

        Try
            Dim refreshModeFlag As Boolean = False 'AG 21/06/2012
            If Not copyRefreshDS Is Nothing Then refreshModeFlag = True 'AG 21/06/2012

            Dim TestName As String = TestsListViewText
            Dim IsRerunDone As New Dictionary(Of Integer, Boolean)
            Dim Remark As String = Nothing

            dgv.Enabled = False
            BlankTestName = TestName
            RemoveOldSortGlyph(dgv)

            Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                        (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                         Where row.TestName = TestName AndAlso row.SampleClass = "BLANK" _
                         Select row).ToList()

            If TestList.Count = 0 Then
                'Set the rows count for the Collapse Column
                CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = 0

                For j As Integer = 0 To dgv.Rows.Count - 1
                    dgv.Rows(j).Visible = False
                Next
                'dgv.Visible = True
                Return
            End If

            'AG 08/08/2010 - Error for fix: When the Test calibrates with multi item number the result appears repeated as many
            'times as multi item number in calibrator due the view has one record for each calibrator Theoretical concentration

            'Dim AverageList As List(Of ResultsDS.vwksResultsRow) = _
            '                (From row In AverageResultsDS.vwksResults _
            '                 Where row.OrderTestID = TestList(0).OrderTestID _
            '                 Select row).ToList()

            Dim maxTheoreticalConc As Single = _
                (From row In AverageResultsDS.vwksResults _
                 Where row.OrderTestID = TestList(0).OrderTestID _
                 Select row.TheoricalConcentration).Max

            Dim AverageList As List(Of ResultsDS.vwksResultsRow) = _
                (From row In AverageResultsDS.vwksResults _
                 Where row.OrderTestID = TestList(0).OrderTestID _
                 AndAlso row.TheoricalConcentration = maxTheoreticalConc _
                 Select row).ToList()
            'END AG 08/08/2010

            dgv.Columns("WorkReagent").Visible = False
            dgv.Columns("KineticBlankLimit").Visible = False
            dgv.Columns("ABSInitial").Visible = False
            dgv.Columns("ABSMainFilter").Visible = False

            Dim MaxRows As Integer = 0
            Dim sentRepetitionCriteria As String = String.Empty

            For Each resultRow As ResultsDS.vwksResultsRow In AverageList
                If Not IsRerunDone.ContainsKey(resultRow.RerunNumber) Then
                    IsRerunDone(resultRow.RerunNumber) = True

                    'AG 21/06/2012 - use AverageList instead of AverageResultsDS.vwksResults 'RH 23/04/2012 
                    Dim RerunList As List(Of Integer) = _
                            (From row In AverageList _
                             Where row.OrderTestID = resultRow.OrderTestID _
                             Select row.RerunNumber Distinct).ToList()

                    Dim MaxRerunNumber As Integer = RerunList.Count
                    'END RH 23/04/2012

                    If MaxRows >= dgv.Rows.Count Then
                        dgv.Rows.Add()
                    Else
                        dgv.Rows(MaxRows).Visible = True
                        dgv.Rows(MaxRows).DefaultCellStyle.Font = RegularFont 'AG 25/11/2010

                        'RH 21/10/2011
                        MergeCells(dgv, "TestName", MaxRows, 1)
                        'MergeCells(dgv, "Rerun", MaxRows, 1)
                        MergeCells(dgv, "BlankAbsLimit", MaxRows, 1)
                    End If

                    'SetSubHeaderColors(dgv, i)
                    dgv.Rows(MaxRows).DefaultCellStyle.BackColor = AverageBkColor
                    dgv.Rows(MaxRows).DefaultCellStyle.SelectionBackColor = AverageBkColor
                    dgv.Rows(MaxRows).DefaultCellStyle.ForeColor = AverageForeColor
                    dgv.Rows(MaxRows).DefaultCellStyle.SelectionForeColor = AverageForeColor

                    CType(dgv(CollapseColName, MaxRows), bsDataGridViewCollapseCell).IsSubHeader = True

                    'dl 23/09/2011
                    'dgv("Ok", MaxRows).Value = resultRow.AcceptedResultFlag
                    If resultRow.AcceptedResultFlag Then
                        dgv("Ok", MaxRows).Value = OKImage
                    Else
                        dgv("Ok", MaxRows).Value = UnCheckImage ' NoImage 
                    End If
                    'dl

                    'AG 15/08/2010
                    'If resultRow.Equals(AverageList(AverageList.Count - 1)) Then
                    If AllowManualRepetitions(resultRow, MaxRerunNumber, "BLANK", sentRepetitionCriteria) Then
                        SetRepPostDilutionImage(dgv, "NewRep", MaxRows, resultRow.PostDilutionType, True)
                    Else
                        'dgv("NewRep", MaxRows).Value = NoImage
                        SetRepPostDilutionImage(dgv, "NewRep", MaxRows, sentRepetitionCriteria, False)
                    End If

                    'dgv("TestName", MaxRows).Value = TestName
                    'dgv("TestName", MaxRows).Value = IIf(resultRow.RerunNumber = 1, TestName, TestName & " (" & resultRow.RerunNumber.ToString & ")")
                    'RH 20/10/2011 String.Format() is a method optimized for Strings concatenations
                    dgv("TestName", MaxRows).Value = IIf(resultRow.RerunNumber = 1, TestName, String.Format("{0} ({1})", TestName, resultRow.RerunNumber))
                    dgv("Rerun", MaxRows).Value = resultRow.RerunNumber

                    'dgv("No", i).Value = Nothing

                    If Not resultRow.IsAbs_WorkReagentNull Then
                        dgv("WorkReagent", MaxRows).Value = resultRow.Abs_WorkReagent.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                        dgv.Columns("WorkReagent").Visible = True
                    Else
                        dgv("WorkReagent", MaxRows).Value = Nothing
                        dgv.Columns("WorkReagent").Visible = False
                    End If

                    'AG 02/08/2010 - Initial Abs is has NO NULL value for kinetics or fixed time tests
                    'In these cases show also the KineticBlankLimit
                    'If Not resultRow.IsKineticBlankLimitNull Then
                    '    dgv("KineticBlankLimit", i).Value = resultRow.KineticBlankLimit.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                    '    dgv.Columns("KineticBlankLimit").Visible = True
                    'End If

                    If Not resultRow.IsABS_InitialNull Then
                        dgv("ABSInitial", MaxRows).Value = resultRow.ABS_Initial.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                        dgv.Columns("ABSInitial").Visible = True
                    Else
                        dgv("ABSInitial", MaxRows).Value = Nothing
                        dgv.Columns("ABSInitial").Visible = False
                    End If

                    If Not resultRow.IsKineticBlankLimitNull Then
                        dgv("KineticBlankLimit", MaxRows).Value = resultRow.KineticBlankLimit.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                    Else
                        dgv("KineticBlankLimit", MaxRows).Value = Nothing
                    End If

                    'RH 18/10/2010
                    dgv.Columns("KineticBlankLimit").Visible = dgv.Columns("ABSInitial").Visible
                    'If dgv.Columns("ABSInitial").Visible = True Then
                    '    dgv.Columns("KineticBlankLimit").Visible = True
                    'End If
                    'END AG 02/08/2010

                    If Not resultRow.IsABS_MainFilterNull Then
                        dgv("ABSMainFilter", MaxRows).Value = resultRow.ABS_MainFilter.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                        dgv.Columns("ABSMainFilter").Visible = True
                    Else
                        dgv("ABSMainFilter", MaxRows).Value = Nothing
                        dgv.Columns("ABSMainFilter").Visible = False
                    End If

                    If Not resultRow.IsBlankAbsorbanceLimitNull Then
                        dgv("BlankAbsLimit", MaxRows).Value = resultRow.BlankAbsorbanceLimit.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                    Else
                        dgv("BlankAbsLimit", MaxRows).Value = Nothing
                    End If

                    'AG 15/09/2010 - Special case when Absorbance has error
                    If Not resultRow.IsABS_ErrorNull Then
                        If Not String.IsNullOrEmpty(resultRow.ABS_Error) Then
                            dgv("ABSValue", MaxRows).Value = GlobalConstants.ABSORBANCE_ERROR
                        Else
                            dgv("ABSValue", MaxRows).Value = Nothing
                            'Also change value for WorkReagent, AbsInitial,AbsMainFilter? By now no!
                        End If
                    Else
                        'RH 18/10/2010
                        'There is no error, so update dgv("ABSValue", i).Value
                        If Not resultRow.IsABSValueNull Then
                            dgv("ABSValue", MaxRows).Value = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                        Else
                            dgv("ABSValue", MaxRows).Value = Nothing
                        End If
                    End If
                    'END AG 15/09/2010

                    Remark = GetResultAlarmDescription(resultRow.OrderTestID, resultRow.RerunNumber, resultRow.MultiPointNumber)
                    ' dl 27/06/2011
                    'dgv("SeeRems", MaxRows).Style.Font = SeeRemsFont
                    'If Not String.IsNullOrEmpty(Remark) Then
                    '    dgv("SeeRems", MaxRows).Value = "*"
                    'Else
                    '    dgv("SeeRems", MaxRows).Value = Nothing
                    'End If
                    'dgv("SeeRems", MaxRows).ToolTipText = Remark
                    ' en dl 27/06/2011

                    dgv("Remarks", MaxRows).Value = Remark
                    'If Not Remark.ToString.Trim Is String.Empty Then dgv("No", MaxRows).Value = "*" 'DL 22/09/2011
                    If Remark <> String.Empty Then dgv("No", MaxRows).Value = "*" 'RH 21/10/2011
                    'RH 15/11/2011
                    dgv("No", MaxRows).ToolTipText = Remark

                    dgv("Graph", MaxRows).Value = AVG_ABS_GRAPHImage
                    dgv("Graph", MaxRows).ToolTipText = labelOpenAbsorbanceCurve
                    dgv("ResultDate", MaxRows).Value = resultRow.ResultDateTime.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                 resultRow.ResultDateTime.ToString(SystemInfoManager.OSLongTimeFormat)

                    'dgv("ResultDate", i).Value = resultRow.ResultDateTime.ToShortDateString() & _
                    '                                            " " & resultRow.ResultDateTime.ToShortTimeString()
                    dgv.Rows(MaxRows).Tag = resultRow

                    Dim OrderTestID As Integer = resultRow.OrderTestID
                    Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010
                    Dim Striked As Boolean = False

                    'AG 04/08/2010 - add rerunnumber condition
                    TestList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                Where row.OrderTestID = OrderTestID _
                                AndAlso row.RerunNumber = myRerunNumber _
                                Select row).ToList()

                    If TestList.Count > 0 Then
                        'dl 23/09/2011
                        'SetPostDilutionImage(dgv, "Rep", MaxRows, TestList(0).PostDilutionType)
                        SetPostDilutionText(dgv, "Rep", MaxRows, TestList(0).PostDilutionType, TestList(0).RerunNumber)

                        'dgv.Rows.Add(TestList.Count)

                        For j As Integer = 0 To TestList.Count - 1
                            Dim k As Integer = j + MaxRows + 1

                            If k >= dgv.Rows.Count Then
                                dgv.Rows.Add()
                                dgv.Rows(k).Visible = Not resultRow.Collapsed 'RH 13/12/2010
                            Else
                                'dgv.Rows(k).Visible = True
                                dgv.Rows(k).Visible = Not resultRow.Collapsed 'RH 13/12/2010

                                'RH 21/10/2011
                                MergeCells(dgv, "TestName", k, 1)
                                'MergeCells(dgv, "Rerun", k, 1)
                                MergeCells(dgv, "BlankAbsLimit", k, 1)
                            End If

                            'SetStrike(dgv, k, TestList(j).InUse)
                            If TestList(j).InUse Then
                                dgv.Rows(k).DefaultCellStyle.Font = RegularFont
                                dgv.Rows(k).DefaultCellStyle.BackColor = RegularBkColor
                                dgv.Rows(k).DefaultCellStyle.SelectionBackColor = RegularBkColor
                                dgv.Rows(k).DefaultCellStyle.ForeColor = RegularForeColor
                                dgv.Rows(k).DefaultCellStyle.SelectionForeColor = RegularForeColor
                            Else
                                dgv.Rows(k).DefaultCellStyle.Font = StrikeFont
                                dgv.Rows(k).DefaultCellStyle.BackColor = StrikeBkColor
                                dgv.Rows(k).DefaultCellStyle.SelectionBackColor = StrikeBkColor
                                dgv.Rows(k).DefaultCellStyle.ForeColor = StrikeForeColor
                                dgv.Rows(k).DefaultCellStyle.SelectionForeColor = StrikeForeColor
                                Striked = True
                            End If

                            CType(dgv(CollapseColName, k), bsDataGridViewCollapseCell).IsSubHeader = False

                            dgv("Rep", k).Value = Nothing
                            dgv("Ok", k).Value = NoImage
                            dgv("NewRep", k).Value = NoImage
                            dgv("Rerun", k).Value = TestList(j).RerunNumber

                            dgv("Graph", k).Value = ABS_GRAPHImage
                            dgv("Graph", k).ToolTipText = labelOpenAbsorbanceCurve

                            ' DL 27/06/2011
                            'dgv("No", k).Value = TestList(j).ReplicateNumber
                            'If Not String.IsNullOrEmpty(Remark) Then
                            '    dgv("No", k).Value = "* " & TestList(j).ReplicateNumber
                            'Else
                            '    dgv("No", k).Value = TestList(j).ReplicateNumber
                            'End If
                            ' End DL 27/06/2011

                            If Not TestList(j).IsAbs_WorkReagentNull Then
                                dgv("WorkReagent", k).Value = TestList(j).Abs_WorkReagent.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                            Else
                                dgv("WorkReagent", k).Value = Nothing
                            End If

                            If Not TestList(j).IsKineticBlankLimitNull Then
                                dgv("KineticBlankLimit", k).Value = TestList(j).KineticBlankLimit.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                            Else
                                dgv("KineticBlankLimit", k).Value = Nothing
                            End If

                            If Not TestList(j).IsABS_InitialNull Then
                                dgv("ABSInitial", k).Value = TestList(j).ABS_Initial.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                            Else
                                dgv("ABSInitial", k).Value = Nothing
                            End If

                            If Not TestList(j).IsABS_MainFilterNull Then
                                dgv("ABSMainFilter", k).Value = TestList(j).ABS_MainFilter.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                            Else
                                dgv("ABSMainFilter", k).Value = Nothing
                            End If

                            'AG 15/09/2010 - Special case when Absorbance has error
                            If Not TestList(j).IsABS_ErrorNull Then
                                If Not String.IsNullOrEmpty(TestList(j).ABS_Error) Then
                                    dgv("ABSValue", k).Value = GlobalConstants.ABSORBANCE_ERROR
                                Else
                                    dgv("ABSValue", k).Value = Nothing
                                    'Also change value for WorkReagent, AbsInitial,AbsMainFilter? By now no!
                                End If
                            Else
                                'RH 18/10/2010
                                'There is no error, so update dgv("ABSValue", k).Value
                                If Not TestList(j).IsABS_ValueNull Then
                                    dgv("ABSValue", k).Value = TestList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                Else
                                    dgv("ABSValue", k).Value = Nothing
                                End If
                            End If
                            'END AG 15/09/2010

                            Remark = GetExecutionAlarmDescription(TestList(j).ExecutionID)
                            ' dl 27/06/2011
                            'dgv("SeeRems", k).Style.Font = SeeRemsFont
                            'If Not String.IsNullOrEmpty(Remark) Then
                            '    dgv("SeeRems", k).Value = "*"
                            'Else
                            '    dgv("SeeRems", k).Value = String.Empty
                            'End If
                            'dgv("SeeRems", k).ToolTipText = Remark
                            ' End dl 27/06/2011
                            dgv("Remarks", k).Value = Remark

                            'RH 03/08/2011
                            If Not String.IsNullOrEmpty(Remark) Then
                                dgv("No", k).Value = "* " & TestList(j).ReplicateNumber
                            Else
                                dgv("No", k).Value = TestList(j).ReplicateNumber
                            End If
                            dgv("No", k).ToolTipText = Remark
                            'RH 03/08/2011

                            dgv("ResultDate", k).Value = TestList(j).ResultDate.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                         TestList(j).ResultDate.ToString(SystemInfoManager.OSLongTimeFormat)

                            'dgv("ResultDate", k).Value = TestList(j).ResultDate.ToShortDateString() & _
                            '                                        " " & TestList(j).ResultDate.ToShortTimeString()

                            dgv.Rows(k).Tag = TestList(j)

                            dgv("TestName", k).Value = dgv("TestName", MaxRows).Value
                            dgv("Rerun", k).Value = dgv("Rerun", MaxRows).Value
                            dgv("BlankAbsLimit", k).Value = dgv("BlankAbsLimit", MaxRows).Value
                        Next

                        If Not Striked Then
                            MergeCells(dgv, "TestName", MaxRows + 1, TestList.Count)
                            'MergeCells(dgv, "Rerun", MaxRows + 1, TestList.Count)
                            MergeCells(dgv, "BlankAbsLimit", MaxRows + 1, TestList.Count)
                        End If

                        If Not IsColCollapsed.ContainsKey(TestsListViewText & "_BLANK") Then
                            CType(dgv(CollapseColName, MaxRows), bsDataGridViewCollapseCell).IsCollapsed = resultRow.Collapsed
                        End If

                    Else
                        'DL 23/09/2011
                        'dgv("Rep", MaxRows).Value = EQUAL_REP
                        SetPostDilutionText(dgv, "Rep", MaxRows, "EQUAL_REP", CInt(dgv("Rerun", MaxRows).Value))
                    End If

                    MaxRows += TestList.Count + 1

                End If
            Next

            'Set the rows count for the Collapse Column
            CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = MaxRows

            If IsColCollapsed.ContainsKey(TestsListViewText & "_BLANK") Then
                If IsColCollapsed(TestsListViewText & "_BLANK") Then
                    CollapseAll(dgv)
                Else
                    ExpandAll(dgv)
                End If
            End If

            If MaxRows < dgv.Rows.Count Then
                For j As Integer = MaxRows To dgv.Rows.Count - 1
                    dgv.Rows(j).Visible = False
                Next
            End If

            'Dim ElapsedTime As Double = Now.Subtract(StartTime).TotalMilliseconds
            'MessageBox.Show("UpdateBlanksDataGrid Elapsed Time: " & ElapsedTime.ToStringWithDecimals(0))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateBlanksDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            dgv.Enabled = True
            Application.DoEvents()

        End Try
    End Sub

#End Region

End Class