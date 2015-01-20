Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Controls.UserControls
'AG 26/07/2010


Partial Class IResults

    Dim CollapseColumnControls As New bsDataGridViewCollapseColumn

#Region "ControlsDataGridView Methods"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' '<param name="pLanguageID">parameter has the current language session </param>
    ''' Modified by: PG 14/10/2010 - Add the LanguageID parameter
    ''' Modified by: RH 18/10/2010 - Remove the LanguageID parameter. Now it is a class property.
    ''' </remarks>
    Private Sub InitializeControlsGrid()
        'Private Sub InitializeControlsGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsControlsDataGridView.Columns.Clear()

            'Dim CollapseColumn As New bsDataGridViewCollapseColumn
            With CollapseColumnControls
                .Name = CollapseColName
                AddHandler .HeaderClickEventHandler, AddressOf GenericDataGridView_CellMouseClick
            End With
            bsControlsDataGridView.Columns.Add(CollapseColumnControls)

            'dl 23/09/2011
            'Dim OkColumn As New DataGridViewCheckBoxColumn
            Dim OkColumn As New DataGridViewImageColumn
            With OkColumn
                .Name = "Ok"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_OK", LanguageID)
                .Width = 35
            End With
            bsControlsDataGridView.Columns.Add(OkColumn)
            'DL 23/09/2011

            'DL 30/09/2011
            'bsControlsDataGridView.Columns.Add("ExportStatus", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LIS", LanguageID))
            'bsControlsDataGridView.Columns("ExportStatus").Width = 35

            'HISSent column
            Dim HISSentColumn As New DataGridViewImageColumn
            With HISSentColumn
                .Name = "ExportStatus"
                .HeaderText = MyClass.LISNameForColumnHeaders 'String.Empty" SGM 12/04/2012
                .Width = 33
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With
            bsControlsDataGridView.Columns.Add(HISSentColumn)
            'DL 30/09/2011
            'JC 12/11/2012
            Dim NewRepColumn As New DataGridViewImageColumn
            With NewRepColumn
                .Name = "NewRep"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_New", LanguageID)
                .Width = 50
            End With
            bsControlsDataGridView.Columns.Add(NewRepColumn)

            Dim NameColumn As New DataGridViewTextBoxSpanColumn
            With NameColumn
                .Name = "ControlName" '"TestName" AG 15/08/2010
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", LanguageID)
                .Width = 120
            End With
            bsControlsDataGridView.Columns.Add(NameColumn)

            Dim LotColumn As New DataGridViewTextBoxSpanColumn
            With LotColumn
                .Name = "Lot"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", LanguageID)
                .Width = 60
            End With
            bsControlsDataGridView.Columns.Add(LotColumn)

            Dim RerunColumn As New DataGridViewTextBoxSpanColumn
            With RerunColumn
                .Name = "Rerun"
                .HeaderText = "" 'myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rerun", LanguageID)
                .Visible = False
                .Width = 50
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End With
            bsControlsDataGridView.Columns.Add(RerunColumn)

            'bsControlsDataGridView.Columns.Add("SeeRems", "")
            'bsControlsDataGridView.Columns("SeeRems").Width = 33
            'bsControlsDataGridView.Columns("SeeRems").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            bsControlsDataGridView.Columns.Add("SampleType", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", LanguageID))
            bsControlsDataGridView.Columns("SampleType").Width = 45

            Dim GraphColumn As New DataGridViewImageColumn
            With GraphColumn
                .Name = "Graph"
                .HeaderText = "" 'myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Graph", LanguageID)
                .Width = 30
            End With
            bsControlsDataGridView.Columns.Add(GraphColumn)

            ' JC 09/11/2012
            bsControlsDataGridView.Columns.Add("No", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", LanguageID))
            ' bsControlsDataGridView.Columns("No").Width = 33
            bsControlsDataGridView.Columns("No").Width = 33
            bsControlsDataGridView.Columns("No").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight


            bsControlsDataGridView.Columns.Add("ABSValue", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", LanguageID))
            bsControlsDataGridView.Columns("ABSValue").Width = 55
            bsControlsDataGridView.Columns("ABSValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            bsControlsDataGridView.Columns.Add("Concentration", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", LanguageID))
            bsControlsDataGridView.Columns("Concentration").Width = 65
            bsControlsDataGridView.Columns("Concentration").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            'END AG 15/08/2010

            ' JC 09/11/2012
            Dim UnitColumn As New DataGridViewTextBoxSpanColumn
            With UnitColumn
                .Name = "Unit"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", LanguageID)
                .Width = 55
            End With
            bsControlsDataGridView.Columns.Add(UnitColumn)

            Dim LimitsColumn As New DataGridViewTextBoxSpanColumn
            With LimitsColumn
                .Name = "Limits"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Concentration_Limits", LanguageID)
                .Width = 130
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
            End With
            bsControlsDataGridView.Columns.Add(LimitsColumn)

            'PG 14/10/2010
            'ControlsDataGridView.Columns.Add("Remarks", "Remarks")
            bsControlsDataGridView.Columns.Add("Remarks", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", LanguageID))
            'PG 14/10/2010
            bsControlsDataGridView.Columns("Remarks").Width = 300
            bsControlsDataGridView.Columns("Remarks").DefaultCellStyle.WrapMode = DataGridViewTriState.True

            'PG 14/10/2010
            'ControlsDataGridView.Columns.Add("ResultDate", "Date")
            bsControlsDataGridView.Columns.Add("ResultDate", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", LanguageID))
            'PG 14/10/2010
            bsControlsDataGridView.Columns("ResultDate").Width = 140 'AG 15/08/2010 (120)
            bsControlsDataGridView.Columns("ResultDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'DL 23/09/2011
            'Dim RepColumn As New DataGridViewImageColumn
            'With RepColumn
            '    .Name = "Rep"
            '    .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Replicate_Short", LanguageID)
            '    .Width = 35
            'End With
            'bsControlsDataGridView.Columns.Add(RepColumn)
            bsControlsDataGridView.Columns.Add("Rep", myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITTLE_RepetitionMode", LanguageID))
            bsControlsDataGridView.Columns("Rep").Width = 200
            bsControlsDataGridView.Columns("Rep").DefaultCellStyle.WrapMode = DataGridViewTriState.True
            'DL 23/09/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeControlsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Defines the ControlsDataGridView column names to be sorted
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 10/09/2010
    ''' </remarks>
    Private Sub DefineControlsSortedColumns()
        Try
            'Insert here the column names to be sorted
            ControlSortType("ControlName") = SortType.ASC
            ControlSortType("SampleType") = SortType.ASC
            ControlSortType("ResultDate") = SortType.ASC

            'For drawing the SortGlyph
            For Each Col As DataGridViewColumn In bsControlsDataGridView.Columns
                If ControlSortType.ContainsKey(Col.Name) Then
                    Col.SortMode = DataGridViewColumnSortMode.Programmatic
                Else
                    Col.SortMode = DataGridViewColumnSortMode.NotSortable
                End If
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " DefineControlsSortedColumns ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fills the ControlsDataGridView with test name associated data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 21/07/2010
    ''' Modified by: SA 12/11/2010 - Show Date and Time using the format defined in the OS Culture Info
    '''              SA 26/04/2011 - Changed the implementation to process all Controls for the Test, not just the first one
    ''' </remarks>
    Private Sub UpdateControlsDataGrid()
        If isClosingFlag Then Exit Sub ' XB 24/02/2014 - #1496 No refresh if screen is closing

        If ControlTestName = TestsListViewText Then Return
        Dim dgv As BSDataGridView = bsControlsDataGridView

        Try
            dgv.Enabled = False
            Dim refreshModeFlag As Boolean = False 'AG 21/06/2012
            If Not copyRefreshDS Is Nothing Then refreshModeFlag = True 'AG 21/06/2012
            Dim TestName As String = TestsListViewText
            Dim Remark As String = Nothing

            ControlTestName = TestName

            RemoveOldSortGlyph(dgv)

            Dim orderTestIDs As List(Of Integer)
            orderTestIDs = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                            Where row.TestName = TestName AndAlso row.SampleClass = "CTRL" _
                            Select row.OrderTestID Distinct).ToList()

            'Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
            '            (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
            '             Where row.TestName = TestName AndAlso row.SampleClass = "CTRL" _
            '             Select row).ToList()

            'If TestList.Count = 0 Then
            If (orderTestIDs.Count = 0) Then
                'Set the rows count for the Collapse Column
                CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = 0

                For j As Integer = 0 To dgv.Rows.Count - 1
                    dgv.Rows(j).Visible = False
                Next

                Return
            End If

            Dim maxRows As Integer = 0

            For Each orderTestID As Integer In orderTestIDs
                'RH 23/04/2012
                Dim RerunList As List(Of Integer) = _
                        (From row In AverageResultsDS.vwksResults _
                         Where row.OrderTestID = orderTestID _
                         Select row.RerunNumber Distinct).ToList()

                Dim MaxRerunNumber As Integer = RerunList.Count
                'END RH 23/04/2012

                'Get all results for the currently processed OrderTestID 
                Dim AverageList As List(Of ResultsDS.vwksResultsRow) = _
                        (From row In AverageResultsDS.vwksResults _
                         Where row.OrderTestID = orderTestID _
                         Select row).ToList()

                For Each resultRow As ResultsDS.vwksResultsRow In AverageList
                    If (maxRows >= dgv.Rows.Count) Then
                        dgv.Rows.Add()
                    Else
                        dgv.Rows(maxRows).Visible = True
                        dgv.Rows(maxRows).DefaultCellStyle.Font = RegularFont 'AG 25/11/2010

                        'RH 21/10/2011
                        MergeCells(dgv, "ControlName", maxRows, 1)
                        MergeCells(dgv, "Lot", maxRows, 1)
                        'MergeCells(dgv, "Rerun", maxRows, 1)
                        MergeCells(dgv, "Unit", maxRows, 1)
                        MergeCells(dgv, "Limits", maxRows, 1)
                    End If

                    dgv.Rows(maxRows).DefaultCellStyle.BackColor = AverageBkColor
                    dgv.Rows(maxRows).DefaultCellStyle.SelectionBackColor = AverageBkColor
                    dgv.Rows(maxRows).DefaultCellStyle.ForeColor = AverageForeColor
                    dgv.Rows(maxRows).DefaultCellStyle.SelectionForeColor = AverageForeColor

                    CType(dgv(CollapseColName, maxRows), bsDataGridViewCollapseCell).IsSubHeader = True

                    If (resultRow.AcceptedResultFlag) Then
                        dgv("Ok", maxRows).Value = OKImage
                    Else
                        dgv("Ok", maxRows).Value = UnCheckImage 'NoImage
                    End If

                    'dgv("Ok", maxRows).Value = resultRow.AcceptedResultFlag

                    Dim sentRepetitionCriteria As String = ""
                    If (Me.AllowManualRepetitions(resultRow, MaxRerunNumber, "CONTROL", sentRepetitionCriteria)) Then
                        SetRepPostDilutionImage(dgv, "NewRep", maxRows, resultRow.PostDilutionType, True)
                    Else
                        SetRepPostDilutionImage(dgv, "NewRep", maxRows, sentRepetitionCriteria, False)
                    End If

                    'dgv("ControlName", maxRows).Value = resultRow.ControlName
                    'RH 14/11/2011
                    dgv("ControlName", maxRows).Value = IIf(resultRow.RerunNumber = 1, resultRow.ControlName, String.Format("{0} ({1})", resultRow.ControlName, resultRow.RerunNumber))

                    dgv("Lot", maxRows).Value = resultRow.ControlLotNumber
                    dgv("Rerun", maxRows).Value = resultRow.RerunNumber

                    'If (Not resultRow.IsExportStatusNull) Then
                    '    dgv("ExportStatus", maxRows).Value = IIf(resultRow.ExportStatus = "SENT", GlobalConstants.EXPORTED_RESULT, "")
                    'Else
                    '    dgv("ExportStatus", maxRows).Value = ""
                    'End If

                    If Not resultRow.IsExportStatusNull Then
                        'dgv("ExportStatus", MaxRows).Value = IIf(resultRow.ExportStatus = "SENT", GlobalConstants.EXPORTED_RESULT, "")
                        If resultRow.ExportStatus = "SENT" Then
                            dgv("ExportStatus", maxRows).Value = LISHeadImage
                            dgv("ExportStatus", maxRows).ToolTipText = labelHISNOTSent
                        Else
                            dgv("ExportStatus", maxRows).Value = NoImage
                            dgv("ExportStatus", maxRows).ToolTipText = labelHISNOTSent
                        End If
                    Else
                        'dgv("ExportStatus", MaxRows).Value = ""
                        dgv("ExportStatus", maxRows).Value = NoImage
                        dgv("ExportStatus", maxRows).ToolTipText = labelHISNOTSent
                    End If

                    dgv("SampleType", maxRows).Value = resultRow.SampleType
                    dgv("No", maxRows).Value = Nothing

                    If Not resultRow.IsABSValueNull Then
                        dgv("ABSValue", maxRows).Value = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                    Else
                        dgv("ABSValue", maxRows).Value = String.Empty
                    End If

                    dgv("Unit", maxRows).Value = resultRow.MeasureUnit

                    If (Not resultRow.IsCONC_ValueNull) Then
                        Dim hasConcentrationError As Boolean = False

                        If (Not resultRow.IsCONC_ErrorNull) Then
                            hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error)
                        End If

                        If (Not hasConcentrationError) Then
                            dgv("Concentration", maxRows).Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                        Else
                            dgv("Concentration", maxRows).Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                        End If
                    Else
                        dgv("Concentration", maxRows).Value = Nothing
                    End If

                    If (Not resultRow.IsMinConcentrationNull AndAlso Not resultRow.IsMaxConcentrationNull) Then
                        dgv("Limits", maxRows).Value = String.Format("{0} - {1}", resultRow.MinConcentration, resultRow.MaxConcentration)
                    End If

                    If (Not resultRow.IsABS_ErrorNull) Then
                        If (Not String.IsNullOrEmpty(resultRow.ABS_Error)) Then
                            dgv("ABSValue", maxRows).Value = GlobalConstants.ABSORBANCE_ERROR
                            dgv("Concentration", maxRows).Value = GlobalConstants.CONC_DUE_ABS_ERROR
                        End If
                    End If

                    Remark = GetResultAlarmDescription(resultRow.OrderTestID, resultRow.RerunNumber, resultRow.MultiPointNumber)
                    'dgv("SeeRems", maxRows).Style.Font = SeeRemsFont
                    'If (Not String.IsNullOrEmpty(Remark)) Then
                    '    dgv("SeeRems", maxRows).Value = "*"
                    'Else
                    '    dgv("SeeRems", maxRows).Value = Nothing
                    'End If
                    'dgv("SeeRems", maxRows).ToolTipText = Remark
                    dgv("Remarks", maxRows).Value = Remark

                    'If Not Remark.ToString.Trim Is String.Empty Then dgv("No", maxRows).Value = "*" 'DL 22/09/2011
                    If Remark <> String.Empty Then dgv("No", maxRows).Value = "*" 'RH 21/10/2011
                    'RH 15/11/2011
                    dgv("No", maxRows).ToolTipText = Remark

                    dgv("Graph", maxRows).Value = AVG_ABS_GRAPHImage
                    dgv("Graph", maxRows).ToolTipText = labelOpenAbsorbanceCurve

                    dgv("ResultDate", maxRows).Value = resultRow.ResultDateTime.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                       resultRow.ResultDateTime.ToString(SystemInfoManager.OSLongTimeFormat)

                    dgv.Rows(maxRows).Tag = resultRow

                    SetSubHeaderColors(dgv, maxRows)

                    Dim myRerunNumber As Integer = resultRow.RerunNumber
                    Dim Striked As Boolean = False
                    Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                                                                       Where row.OrderTestID = orderTestID _
                                                                                     AndAlso row.RerunNumber = myRerunNumber _
                                                                                      Select row).ToList()

                    If (TestList.Count > 0) Then
                        'dl23/09/2011
                        'SetPostDilutionImage(dgv, "Rep", maxRows, TestList(0).PostDilutionType)
                        SetPostDilutionText(dgv, "Rep", maxRows, TestList(0).PostDilutionType, TestList(0).RerunNumber)

                        dgv.Rows.Add(TestList.Count)
                        For j As Integer = 0 To TestList.Count - 1
                            Dim k As Integer = j + maxRows + 1

                            If (k >= dgv.Rows.Count) Then
                                dgv.Rows.Add()
                                dgv.Rows(k).Visible = Not resultRow.Collapsed
                            Else
                                dgv.Rows(k).Visible = Not resultRow.Collapsed

                                'RH 21/10/2011
                                MergeCells(dgv, "ControlName", k, 1)
                                MergeCells(dgv, "Lot", k, 1)
                                'MergeCells(dgv, "Rerun", k, 1)
                                MergeCells(dgv, "Unit", k, 1)
                                MergeCells(dgv, "Limits", k, 1)
                            End If

                            If (TestList(j).InUse) Then
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

                            dgv("Rep", k).Value = "" ' NoImage dl 23/09/2011
                            dgv("Ok", k).Value = NoImage
                            dgv("ExportStatus", k).Value = NoImage
                            dgv("NewRep", k).Value = NoImage

                            dgv("Graph", k).Value = ABS_GRAPHImage
                            dgv("Graph", k).ToolTipText = labelOpenAbsorbanceCurve

                            dgv("SampleType", k).Value = TestList(j).SampleType

                            'If (Not String.IsNullOrEmpty(Remark)) Then
                            '    dgv("No", k).Value = "* " & TestList(j).ReplicateNumber
                            'Else
                            '    dgv("No", k).Value = TestList(j).ReplicateNumber
                            'End If

                            If Not TestList(j).IsABS_ValueNull Then
                                dgv("ABSValue", k).Value = TestList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                            Else
                                dgv("ABSValue", k).Value = String.Empty
                            End If

                            Dim hasConcentrationError As Boolean = False
                            If (Not TestList(j).IsCONC_ErrorNull) Then
                                hasConcentrationError = Not String.IsNullOrEmpty(TestList(j).CONC_Error)
                            End If
                            If (Not hasConcentrationError) Then
                                dgv("Concentration", k).Value = TestList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                            Else
                                dgv("Concentration", k).Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                            End If

                            If (Not TestList(j).IsABS_ErrorNull) Then
                                If (Not String.IsNullOrEmpty(TestList(j).ABS_Error)) Then
                                    dgv("ABSValue", k).Value = GlobalConstants.ABSORBANCE_ERROR
                                    dgv("Concentration", k).Value = GlobalConstants.CONC_DUE_ABS_ERROR
                                End If
                            End If

                            Remark = GetExecutionAlarmDescription(TestList(j).ExecutionID)
                            'dgv("SeeRems", k).Style.Font = SeeRemsFont
                            'If (Not String.IsNullOrEmpty(Remark)) Then
                            '    dgv("SeeRems", k).Value = "*"
                            'Else
                            '    dgv("SeeRems", k).Value = String.Empty
                            'End If
                            'dgv("SeeRems", k).ToolTipText = Remark
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
                            dgv.Rows(k).Tag = TestList(j)

                            dgv("ControlName", k).Value = dgv("ControlName", maxRows).Value
                            dgv("Lot", k).Value = dgv("Lot", maxRows).Value
                            dgv("Rerun", k).Value = dgv("Rerun", maxRows).Value
                            dgv("Unit", k).Value = dgv("Unit", maxRows).Value
                            dgv("Limits", k).Value = dgv("Limits", maxRows).Value
                        Next

                        If (Not Striked) Then
                            MergeCells(dgv, "ControlName", maxRows + 1, TestList.Count)
                            MergeCells(dgv, "Lot", maxRows + 1, TestList.Count)
                            'MergeCells(dgv, "Rerun", maxRows + 1, TestList.Count)
                            MergeCells(dgv, "Unit", maxRows + 1, TestList.Count)
                            MergeCells(dgv, "Limits", maxRows + 1, TestList.Count)
                        End If

                        If (Not IsColCollapsed.ContainsKey(TestsListViewText & "_CTRL")) Then
                            CType(dgv(CollapseColName, maxRows), bsDataGridViewCollapseCell).IsCollapsed = resultRow.Collapsed
                        End If
                    Else
                        'dgv("Rep", maxRows).Value = EQUAL_REP
                        SetPostDilutionText(dgv, "Rep", maxRows, "EQUAL_REP", CInt(dgv("Rerun", maxRows).Value))
                    End If

                    maxRows += TestList.Count + 1
                Next

                'Set the rows count for the Collapse Column
                CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = maxRows

                If (IsColCollapsed.ContainsKey(TestsListViewText & "_CTRL")) Then
                    If (IsColCollapsed(TestsListViewText & "_CTRL")) Then
                        CollapseAll(dgv)
                    Else
                        ExpandAll(dgv)
                    End If
                End If

                If (maxRows < dgv.Rows.Count) Then
                    For j As Integer = maxRows To dgv.Rows.Count - 1
                        dgv.Rows(j).Visible = False
                    Next
                End If
            Next

            'Dim ElapsedTime As Double = Now.Subtract(StartTime).TotalMilliseconds
            'MessageBox.Show("UpdateControlsDataGrid Elapsed Time: " & ElapsedTime.ToStringWithDecimals(0))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateControlsDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Application.DoEvents()
            dgv.Enabled = True

        End Try
    End Sub

#End Region

End Class