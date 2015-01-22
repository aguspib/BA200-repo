Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Controls.UserControls
'AG 26/07/2010

'Imports DevExpress.XtraReports.UI
'Imports DevExpress.XtraPrinting
'Imports DevExpress.XtraPrintingLinks
'Imports DevExpress.XtraEditors
'Imports DevExpress.XtraGrid
'Imports DevExpress.XtraGrid.Columns
'Imports DevExpress.XtraGrid.Views.Base
'Imports DevExpress.XtraGrid.Views.Grid
'Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
'Imports DevExpress.XtraGrid.Repository
'Imports DevExpress.XtraEditors.Controls
'Imports DevExpress.Utils


Partial Class IResults

    Dim CollapseColumnExperimentals As New bsDataGridViewCollapseColumn

#Region "ExperimentalsDataGridView Methods"
    ''' <summary>
    ''' Initialize the DataGridView for Experimental Results
    ''' </summary>
    ''' <remarks>
    ''' Modified by: AG 02/08/2010 - Added ReferenceRanges column
    '''              PG 14/10/2010 - Added the LanguageID parameter
    '''              RH 18/10/2010 - Remove the LanguageID parameter. Now it is a class property.
    '''              AG 03/03/2011 - view most important results without scrolling (change column witdh, ...)
    ''' </remarks> 
    Private Sub InitializeExperimentalsGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsExperimentalsDataGridView.Columns.Clear()

            'Dim CollapseColumn As New bsDataGridViewCollapseColumn
            With CollapseColumnExperimentals
                .Name = CollapseColName
                AddHandler .HeaderClickEventHandler, AddressOf GenericDataGridView_CellMouseClick
            End With
            bsExperimentalsDataGridView.Columns.Add(CollapseColumnExperimentals)


            'Dim OkColumn As New DataGridViewCheckBoxColumn
            Dim OkColumn As New DataGridViewImageColumn
            With OkColumn
                .Name = "Ok"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_OK", LanguageID)
                .Width = 35
            End With
            bsExperimentalsDataGridView.Columns.Add(OkColumn)

            'DL 30/09/2011
            'bsExperimentalsDataGridView.Columns.Add("ExportStatus", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LIS", LanguageID))
            'bsExperimentalsDataGridView.Columns("ExportStatus").Width = 35
            'bsExperimentalsDataGridView.Columns("ExportStatus").HeaderCell.Style.WrapMode = DataGridViewTriState.True

            'HISSent column
            Dim HISSentColumn As New DataGridViewImageColumn
            With HISSentColumn
                .Name = "ExportStatus"
                .HeaderText = MyClass.LISNameForColumnHeaders 'String.Empty" SGM 12/04/2013
                .Width = 33
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With
            bsExperimentalsDataGridView.Columns.Add(HISSentColumn)
            'DL 30/09/2011
            'JC 12/11/2012

            Dim NewRepColumn As New DataGridViewImageColumn
            With NewRepColumn
                .Name = "NewRep"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_New", LanguageID)
                .Width = 50
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            End With
            bsExperimentalsDataGridView.Columns.Add(NewRepColumn)

            Dim NameColumn As New DataGridViewTextBoxSpanColumn
            With NameColumn
                .Name = "TestName"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", LanguageID)
                .Width = 120
            End With
            bsExperimentalsDataGridView.Columns.Add(NameColumn)

            'ExperimentalsDataGridView.Columns.Add("TestName", "Test")
            'ExperimentalsDataGridView.Columns("TestName").Width = 100

            Dim RerunColumn As New DataGridViewTextBoxSpanColumn
            With RerunColumn
                .Name = "Rerun"
                '.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rerun", LanguageID)
                .Width = 50
                .Visible = False ' AG 03/03/2011
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End With
            bsExperimentalsDataGridView.Columns.Add(RerunColumn)

            'bsExperimentalsDataGridView.Columns.Add("SeeRems", String.Empty)
            'bsExperimentalsDataGridView.Columns("SeeRems").Width = 33
            'bsExperimentalsDataGridView.Columns("SeeRems").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'JC 12/11/2012
            Dim SampleTypeColumn As New DataGridViewTextBoxSpanColumn
            With SampleTypeColumn
                .Name = "SampleType"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", LanguageID)
                .Width = 45
            End With
            bsExperimentalsDataGridView.Columns.Add(SampleTypeColumn)

            Dim GraphColumn As New DataGridViewImageColumn
            With GraphColumn
                .Name = "Graph"
                .HeaderText = "" 'AG 03/03/2011 myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Graph", LanguageID)
                .Width = 30
            End With
            bsExperimentalsDataGridView.Columns.Add(GraphColumn)

            'ExperimentalsDataGridView.Columns.Add("SampleType", "Type")
            'ExperimentalsDataGridView.Columns("SampleType").Width = 65

            'JC 12/11/2012
            bsExperimentalsDataGridView.Columns.Add("No", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", LanguageID))
            bsExperimentalsDataGridView.Columns("No").Width = 33
            bsExperimentalsDataGridView.Columns("No").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            bsExperimentalsDataGridView.Columns.Add("ABSValue", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", LanguageID))
            bsExperimentalsDataGridView.Columns("ABSValue").Width = 55
            bsExperimentalsDataGridView.Columns("ABSValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            bsExperimentalsDataGridView.Columns.Add("Concentration", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", LanguageID))
            bsExperimentalsDataGridView.Columns("Concentration").Width = 65
            bsExperimentalsDataGridView.Columns("Concentration").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            Dim UnitColumn As New DataGridViewTextBoxSpanColumn
            With UnitColumn
                .Name = "Unit"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", LanguageID)
                .Width = 55
            End With
            bsExperimentalsDataGridView.Columns.Add(UnitColumn)

            'ExperimentalsDataGridView.Columns.Add("Unit", "Unit")
            'ExperimentalsDataGridView.Columns("Unit").Width = 45

            'JC 12/11/2012
            Dim RefRangesColumn As New DataGridViewTextBoxSpanColumn
            With RefRangesColumn
                .Name = "ReferenceRanges"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Short", LanguageID)
                .Width = 100
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End With
            bsExperimentalsDataGridView.Columns.Add(RefRangesColumn)

            'ExperimentalsDataGridView.Columns.Add("ReferenceRanges", "Ref. Ranges")
            'ExperimentalsDataGridView.Columns("ReferenceRanges").Width = 95
            'ExperimentalsDataGridView.Columns("ReferenceRanges").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            bsExperimentalsDataGridView.Columns.Add("Remarks", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", LanguageID))
            bsExperimentalsDataGridView.Columns("Remarks").Width = 300
            bsExperimentalsDataGridView.Columns("Remarks").DefaultCellStyle.WrapMode = DataGridViewTriState.True

            bsExperimentalsDataGridView.Columns.Add("ResultDate", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", LanguageID))
            bsExperimentalsDataGridView.Columns("ResultDate").Width = 140 'AG 15/08/2010 (120)
            bsExperimentalsDataGridView.Columns("ResultDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'DL 23/09/2011
            'Dim RepColumn As New DataGridViewImageColumn
            'With RepColumn
            '    .Name = "Rep"
            '    .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Replicate_Short", LanguageID)
            '    .Width = 35
            'End With
            'bsExperimentalsDataGridView.Columns.Add(RepColumn)
            bsExperimentalsDataGridView.Columns.Add("Rep", myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITTLE_RepetitionMode", LanguageID))
            bsExperimentalsDataGridView.Columns("Rep").Width = 200
            bsExperimentalsDataGridView.Columns("Rep").DefaultCellStyle.WrapMode = DataGridViewTriState.True
            'DL 23/09/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeExperimentalsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Defines the ExperimentalsDataGridView column names to be sorted
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 10/09/2010
    ''' </remarks>
    Private Sub DefineExperimentalsSortedColumns()
        Try
            'Insert here the column names to be sorted
            ExperimentalSortType("TestName") = SortType.ASC
            ExperimentalSortType("SampleType") = SortType.ASC
            ExperimentalSortType("ResultDate") = SortType.ASC

            'For drawing the SortGlyph
            For Each Col As DataGridViewColumn In bsExperimentalsDataGridView.Columns
                If ExperimentalSortType.ContainsKey(Col.Name) Then
                    Col.SortMode = DataGridViewColumnSortMode.Programmatic
                Else
                    Col.SortMode = DataGridViewColumnSortMode.NotSortable
                End If
            Next Col

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " DefineExperimentalsSortedColumns ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Fills the ExperimentalsDataGridView with Sample name associated data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 21/07/2010
    ''' Modified by: RH 06/10/2010 - Algorithm redesign for speeding up the execution
    '''                              Now the the method runs 8 - 10 times faster
    '''              SA 21/01/2011 - For OFF-SYSTEM Tests with qualitative results, shown value of field ManualResultText
    '''                              in the grid column for the result (CONC Value)
    '''              AG 22/09/2014 - BA-1940 show specimen in the grid (test name column ... specimen (TEST NAME) )
    '''                              Same format as in the results by test (patients)
    '''              XB 28/11/2014 - Sort the CALC tests behind ISE anf OFFS too - BA-1867
    '''              XB 16/01/2015 - Change on displaying CONC errors values derived from ISE error - BA-1064
    ''' </remarks>
    Private Sub UpdateExperimentalsDataGrid()
        Dim dgv As BSDataGridView = bsExperimentalsDataGridView

        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        Dim StartTime As DateTime = Now
        'Dim myLogAcciones As New ApplicationLogManager()
        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Try
            If isClosingFlag Then Exit Sub ' XB 24/02/2014 - #1496 No refresh if screen is closing

            If bsSamplesListDataGridView.SelectedRows.Count = 0 Then Return 'There is no selected sample
            If ExperimentalSampleIndex = SamplesListViewIndex Then Return

            Cursor = Cursors.WaitCursor

            Dim refreshModeFlag As Boolean = False 'AG 21/06/2012
            If Not copyRefreshDS Is Nothing Then refreshModeFlag = True 'AG 21/06/2012

            Dim SampleName As String = SamplesListViewText
            dgv.Enabled = False
            ExperimentalSampleIndex = SamplesListViewIndex
            RemoveOldSortGlyph(dgv)

            Dim Remark As String = Nothing
            Dim MaxRows As Integer = 0
            Dim OrderTestId As Integer = -1
            Dim IsAverageDone As New Dictionary(Of String, Boolean)

            Dim AverageList As New List(Of ResultsDS.vwksResultsRow)

            Dim myOrderTestID As Integer
            Dim maxTheoreticalConc As Single
            Dim Filter As String = String.Empty
            Dim sentRepetitionCriteria As String = String.Empty

            'AG 25/06/2012 - implement a loop for test type

            ' XB 28/11/2014 - BA-1867
            ' Dim TestType() As String = {"STD", "CALC", "ISE", "OFFS"}
            Dim TestType() As String = {"STD", "ISE", "OFFS", "CALC"}
            ' XB 28/11/2014 - BA-1867

            For Each itemType As String In TestType
                'AG 21/06/2012 - improve speed, search current patient results using LINQ and then apply loop only the linq results
                Dim currentPatientAvgResultsList As List(Of ResultsDS.vwksResultsRow)
                currentPatientAvgResultsList = (From a As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults _
                                                Where String.Equals(a.PatientID, SampleName) _
                                                AndAlso String.Equals(a.TestType, itemType) _
                                                Select a).ToList

                'For Each row As ResultsDS.vwksResultsRow In AverageResultsDS.vwksResults.Rows
                For Each row As ResultsDS.vwksResultsRow In currentPatientAvgResultsList
                    If String.Equals(row.PatientID, SampleName) Then
                        If String.Equals(row.TestType, "STD") Then
                            myOrderTestID = row.OrderTestID
                            'AG 21/06/2012 - use currentPatientAvgResultsList instead of AverageResultsDS.vwksResults
                            maxTheoreticalConc = (From resultRow In currentPatientAvgResultsList _
                                                  Where resultRow.OrderTestID = myOrderTestID _
                                                  Select resultRow.TheoricalConcentration).Max

                            If row.TheoricalConcentration = maxTheoreticalConc Then AverageList.Add(row)

                            'AG 01/12/2010
                        ElseIf String.Equals(row.TestType, "ISE") OrElse String.Equals(row.TestType, "OFFS") Then
                            AverageList.Add(row)
                            'End AG 01/12/2010

                        Else 'It is CALC
                            'If Not CalcTestInserted.ContainsKey(row.TestName) Then
                            'CalcTestInserted(row.TestName) = True
                            AverageList.Add(row)
                        End If
                    End If
                Next row
                'AG 21/06/2012
            Next 'AG 25/06/2012


            'TR 09/07/2012 -Defined Cell style to be used on FOR 
            Dim myCellStyle As New DataGridViewCellStyle
            myCellStyle.Font = RegularFont
            myCellStyle.BackColor = RegularBkColor
            myCellStyle.SelectionBackColor = RegularBkColor
            myCellStyle.ForeColor = RegularForeColor
            myCellStyle.SelectionForeColor = RegularForeColor

            Dim myStrikeCellStyle As New DataGridViewCellStyle
            myStrikeCellStyle.Font = StrikeFont
            myStrikeCellStyle.BackColor = StrikeBkColor
            myStrikeCellStyle.SelectionBackColor = StrikeBkColor
            myStrikeCellStyle.ForeColor = StrikeForeColor
            myStrikeCellStyle.SelectionForeColor = StrikeForeColor

            Dim myAverageCellStyle As New DataGridViewCellStyle
            myAverageCellStyle.BackColor = AverageBkColor
            myAverageCellStyle.SelectionBackColor = AverageBkColor
            myAverageCellStyle.ForeColor = AverageForeColor
            myAverageCellStyle.SelectionForeColor = AverageForeColor
            'TR 09/07/2012 -END

            'TR 07/06/2012
            Dim myLetter As String = ""

            For Each resultRow As ResultsDS.vwksResultsRow In AverageList

                Filter = String.Format("{0}{1}", resultRow.OrderTestID, resultRow.RerunNumber)

                If Not IsAverageDone.ContainsKey(Filter) Then
                    IsAverageDone(Filter) = True

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
                        'Reset merged cells
                        MergeCells(dgv, "TestName", MaxRows, 1)
                        'MergeCells(dgv, "Rerun", MaxRows, 1)
                        MergeCells(dgv, "SampleType", MaxRows, 1)
                        MergeCells(dgv, "Unit", MaxRows, 1)
                        MergeCells(dgv, "ReferenceRanges", MaxRows, 1)
                    End If

                    'SetSubHeaderColors(dgv, i)
                    dgv.Rows(MaxRows).DefaultCellStyle = myAverageCellStyle 'TR 09/07/2012 -Set defined CellStyle
                    'dgv.Rows(MaxRows).DefaultCellStyle.BackColor = AverageBkColor
                    'dgv.Rows(MaxRows).DefaultCellStyle.SelectionBackColor = AverageBkColor
                    'dgv.Rows(MaxRows).DefaultCellStyle.ForeColor = AverageForeColor
                    'dgv.Rows(MaxRows).DefaultCellStyle.SelectionForeColor = AverageForeColor

                    CType(dgv(CollapseColName, MaxRows), bsDataGridViewCollapseCell).IsSubHeader = True

                    'DL 23/09/2011
                    If resultRow.AcceptedResultFlag Then
                        dgv("Ok", MaxRows).Value = OKImage
                    Else
                        dgv("Ok", MaxRows).Value = UnCheckImage ' NoImage 
                    End If

                    'dgv("Ok", i).Value = resultRow.AcceptedResultFlag
                    'DL 23/09/2011

                    sentRepetitionCriteria = String.Empty 'RH 25/05/2012
                    If AllowManualRepetitions(resultRow, MaxRerunNumber, "PATIENTS", sentRepetitionCriteria) Then
                        SetRepPostDilutionImage(dgv, "NewRep", MaxRows, resultRow.PostDilutionType, True)
                    Else
                        'dgv("NewRep", i).Value = NoImage
                        SetRepPostDilutionImage(dgv, "NewRep", MaxRows, sentRepetitionCriteria, False)
                    End If

                    'RH 20/10/2011 String.Format() is a method optimized for Strings concatenations
                    dgv("TestName", MaxRows).Value = IIf(resultRow.RerunNumber = 1, resultRow.TestName, String.Format("{0} ({1})", resultRow.TestName, resultRow.RerunNumber))
                    dgv("Rerun", MaxRows).Value = resultRow.RerunNumber

                    'AG 22/09/2014 - BA-1940 show specimen in the grid (sample type column)
                    dgv("TestName", MaxRows).ToolTipText = String.Empty
                    If Not resultRow.IsSpecimenIDListNull Then
                        dgv("TestName", MaxRows).ToolTipText = resultRow.SpecimenIDList & " (" & dgv("TestName", MaxRows).Value.ToString & ")"
                    End If
                    'AG 22/09/2014


                    'DL 30/09/2011
                    If Not resultRow.IsExportStatusNull Then

                        If String.Equals(resultRow.ExportStatus, "SENT") Then
                            dgv("ExportStatus", MaxRows).Value = LISHeadImage
                            dgv("ExportStatus", MaxRows).ToolTipText = labelHISSent
                        Else
                            dgv("ExportStatus", MaxRows).Value = NoImage
                            dgv("ExportStatus", MaxRows).ToolTipText = labelHISNOTSent
                        End If
                        'dgv("ExportStatus", i).Value = IIf(resultRow.ExportStatus = "SENT", GlobalConstants.EXPORTED_RESULT, "")

                    Else
                        'dgv("ExportStatus", i).Value = ""
                        dgv("ExportStatus", MaxRows).Value = NoImage
                        dgv("ExportStatus", MaxRows).ToolTipText = labelHISNOTSent
                    End If
                    'DL 30/09/2011

                    dgv("SampleType", MaxRows).Value = resultRow.SampleType
                    dgv("No", MaxRows).Value = Nothing

                    If Not resultRow.IsABSValueNull Then
                        dgv("ABSValue", MaxRows).Value = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                    Else
                        dgv("ABSValue", MaxRows).Value = Nothing
                    End If

                    If Not resultRow.IsCONC_ValueNull Then
                        Dim hasConcentrationError As Boolean = False

                        If Not resultRow.IsCONC_ErrorNull Then
                            hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error)
                        End If

                        If Not hasConcentrationError Then
                            dgv("Concentration", MaxRows).Value = resultRow.CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                        Else
                            dgv("Concentration", MaxRows).Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                        End If
                        dgv("Concentration", MaxRows).Style.Alignment = DataGridViewContentAlignment.MiddleRight
                    Else
                        If Not resultRow.IsManualResultTextNull Then
                            dgv("Concentration", MaxRows).Value = resultRow.ManualResultText
                            dgv("Concentration", MaxRows).Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                        Else
                            dgv("Concentration", MaxRows).Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                            dgv("Concentration", MaxRows).Style.Alignment = DataGridViewContentAlignment.MiddleRight
                        End If
                    End If

                    dgv("Unit", MaxRows).Value = resultRow.MeasureUnit

                    If Not String.IsNullOrEmpty(resultRow.NormalLowerLimit) AndAlso _
                           Not String.IsNullOrEmpty(resultRow.NormalUpperLimit) Then
                        dgv("ReferenceRanges", MaxRows).Value = String.Format("{0} - {1}", resultRow.NormalLowerLimit, resultRow.NormalUpperLimit)
                    Else
                        dgv("ReferenceRanges", MaxRows).Value = Nothing
                    End If

                    'AG 15/09/2010 - Special case when Absorbance has error
                    If Not resultRow.IsABS_ErrorNull Then
                        If Not String.IsNullOrEmpty(resultRow.ABS_Error) Then
                            dgv("ABSValue", MaxRows).Value = GlobalConstants.ABSORBANCE_ERROR
                            dgv("Concentration", MaxRows).Value = GlobalConstants.CONC_DUE_ABS_ERROR
                        End If
                    End If
                    'END AG 15/09/2010

                    ' XB 16/01/2015 - BA-1064 
                    If Not resultRow.IsCONC_ErrorNull Then
                        If Not String.IsNullOrEmpty(resultRow.CONC_Error) Then
                            If resultRow.TestType = "ISE" Then
                                dgv("Concentration", MaxRows).Value = GlobalConstants.CONC_ISE_ERROR
                            End If
                        End If
                    End If
                    ' XB 16/01/2015 - BA-1064 

                    Remark = GetResultAlarmDescription(resultRow.OrderTestID, resultRow.RerunNumber, resultRow.MultiPointNumber)
                    ' dl 27/06/2011
                    'dgv("SeeRems", i).Style.Font = SeeRemsFont
                    'If Not String.IsNullOrEmpty(Remark) Then
                    '    dgv("SeeRems", i).Value = "*"
                    'Else
                    '    dgv("SeeRems", i).Value = Nothing
                    'End If
                    'dgv("SeeRems", i).ToolTipText = Remark
                    ' end dl 27/06/2011
                    dgv("Remarks", MaxRows).Value = Remark
                    'If Not Remark.ToString.Trim Is String.Empty Then dgv("No", i).Value = "*" 'DL 22/09/2011

                    'If Remark <> String.Empty Then dgv("No", MaxRows).Value = "*" 'RH 21/10/2011
                    'TR 07/06/2012
                    If Not String.Equals(Remark, String.Empty) Then
                        'Validate if there is a range alarm to replace the asterisk by the corresponding letter.
                        myLetter = ""
                        myLetter = GetRangeAlarmsLetter(resultRow.OrderTestID, resultRow.RerunNumber, resultRow.MultiPointNumber)
                        If String.Equals(myLetter, String.Empty) Then
                            dgv("No", MaxRows).Value = "*"
                        Else
                            dgv("No", MaxRows).Value = myLetter
                        End If

                    End If
                    'TR 07/06/2012 -END.

                    'RH 15/11/2011
                    dgv("No", MaxRows).ToolTipText = Remark

                    'AG 22/12/2010 - Graph image only in STD test average results
                    'dgv("Graph", i).Value = AVG_ABS_GRAPH
                    If String.Equals(resultRow.TestType, "STD") Then
                        dgv("Graph", MaxRows).Value = AVG_ABS_GRAPHImage
                        dgv("Graph", MaxRows).ToolTipText = labelOpenAbsorbanceCurve 'myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ABSORBANCE_CURVE", LanguageID) '"Open Absorbance Curve"
                    Else
                        dgv("Graph", MaxRows).Value = NoImage
                        dgv("Graph", MaxRows).ToolTipText = String.Empty
                    End If
                    'AG 22/12/2010

                    dgv("ResultDate", MaxRows).Value = resultRow.ResultDateTime.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                 resultRow.ResultDateTime.ToString(SystemInfoManager.OSLongTimeFormat)

                    dgv.Rows(MaxRows).Tag = resultRow

                    OrderTestId = resultRow.OrderTestID
                    Dim myRerunNumber As Integer = resultRow.RerunNumber 'AG 04/08/2010
                    Dim Striked As Boolean = False

                    'AG 22/12/2010 - Show replicates also in ISE (remove condition ExecutionType = 'PREP_STD')
                    'AG 02/12/2010 - add condition ExecutionType = 'PREP_STD' ('AG 04/08/2010 - add rerunnumber condition)
                    'Dim SampleList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                    '               (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                    '                Where row.OrderTestID = OrderTestId _
                    '                AndAlso row.RerunNumber = myRerunNumber _
                    '                AndAlso row.ExecutionType = "PREP_STD" _
                    '                Select row).ToList()
                    Dim SampleList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                        (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                        Where row.OrderTestID = OrderTestId _
                        AndAlso row.RerunNumber = myRerunNumber _
                        Select row).ToList()


                    If SampleList.Count > 0 Then

                        CType(dgv(CollapseColName, MaxRows), bsDataGridViewCollapseCell).IsEnabled = True
                        'SetPostDilutionImage(dgv, "Rep", i, SampleList(0).PostDilutionType)
                        SetPostDilutionText(dgv, "Rep", MaxRows, SampleList(0).PostDilutionType, SampleList(0).RerunNumber)
                        'dgv.Rows.Add(SampleList.Count)

                        For j As Integer = 0 To SampleList.Count - 1
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
                                MergeCells(dgv, "SampleType", k, 1)
                                MergeCells(dgv, "Unit", k, 1)
                                MergeCells(dgv, "ReferenceRanges", k, 1)
                            End If

                            'SetStrike(dgv, k, SampleList(j).InUse)
                            If SampleList(j).InUse Then
                                dgv.Rows(k).DefaultCellStyle = myCellStyle 'TR 09/07/2012 -Set defined CellStyle
                                'dgv.Rows(k).DefaultCellStyle.Font = RegularFont
                                'dgv.Rows(k).DefaultCellStyle.BackColor = RegularBkColor
                                'dgv.Rows(k).DefaultCellStyle.SelectionBackColor = RegularBkColor
                                'dgv.Rows(k).DefaultCellStyle.ForeColor = RegularForeColor
                                'dgv.Rows(k).DefaultCellStyle.SelectionForeColor = RegularForeColor
                            Else
                                dgv.Rows(k).DefaultCellStyle = myStrikeCellStyle 'TR 09/07/2012 -Set defined CellStyle
                                'dgv.Rows(k).DefaultCellStyle.Font = StrikeFont
                                'dgv.Rows(k).DefaultCellStyle.BackColor = StrikeBkColor
                                'dgv.Rows(k).DefaultCellStyle.SelectionBackColor = StrikeBkColor
                                'dgv.Rows(k).DefaultCellStyle.ForeColor = StrikeForeColor
                                'dgv.Rows(k).DefaultCellStyle.SelectionForeColor = StrikeForeColor
                                Striked = True
                            End If

                            CType(dgv(CollapseColName, k), bsDataGridViewCollapseCell).IsSubHeader = False
                            CType(dgv(CollapseColName, MaxRows), bsDataGridViewCollapseCell).IsEnabled = True

                            dgv("Rep", k).Value = String.Empty ' NoImage
                            dgv("Ok", k).Value = NoImage
                            'dgv("Ok", k).Value = False
                            dgv("NewRep", k).Value = NoImage

                            ' dl 27/06/2011
                            'dgv("No", k).Value = SampleList(j).ReplicateNumber
                            'If Not String.IsNullOrEmpty(Remark) Then
                            '    dgv("No", k).Value = "* " & SampleList(j).ReplicateNumber
                            'Else
                            '    dgv("No", k).Value = SampleList(j).ReplicateNumber
                            'End If
                            ' end dl 27/06/2011

                            'AG 22/12/2010 - ISE results show also replicates
                            'dgv("Graph", k).Value = ABS_GRAPH
                            'dgv("ABSValue", k).Value = SampleList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                            If String.Equals(SampleList(j).ExecutionType, "PREP_STD") Then
                                dgv("Graph", k).Value = ABS_GRAPHImage
                                dgv("Graph", k).ToolTipText = labelOpenAbsorbanceCurve

                                If Not SampleList(j).IsABS_ValueNull Then
                                    dgv("ABSValue", k).Value = SampleList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                                End If

                            Else
                                dgv("Graph", k).Value = NoImage
                                dgv("Graph", k).ToolTipText = String.Empty
                                dgv("ABSValue", k).Value = String.Empty
                            End If
                            'AG 22/12/2010

                            'AG 02/08/2010
                            Dim hasConcentrationError As Boolean = False

                            If Not SampleList(j).IsCONC_ErrorNull Then 'RH 14/09/2010
                                hasConcentrationError = Not String.IsNullOrEmpty(SampleList(j).CONC_Error)
                            End If

                            If Not hasConcentrationError Then
                                If (Not SampleList(j).IsCONC_ValueNull) Then
                                    dgv("Concentration", k).Value = SampleList(j).CONC_Value.ToStringWithDecimals(resultRow.DecimalsAllowed)
                                    dgv("Concentration", k).Style.Alignment = DataGridViewContentAlignment.MiddleRight
                                    'ElseIf (Not SampleList(j).IsManualResultTextNull) Then
                                ElseIf Not resultRow.IsManualResultTextNull Then
                                    'dgv("Concentration", k).Value = SampleList(j).ManualResultText
                                    'RH 31/01/2011
                                    'Take the Manual Result text from the average result
                                    dgv("Concentration", k).Value = resultRow.ManualResultText
                                    dgv("Concentration", k).Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                                Else
                                    dgv("Concentration", k).Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                    dgv("Concentration", k).Style.Alignment = DataGridViewContentAlignment.MiddleRight
                                End If
                            Else
                                dgv("Concentration", k).Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                dgv("Concentration", k).Style.Alignment = DataGridViewContentAlignment.MiddleRight
                            End If
                            'END AG 02/08/2010

                            'AG 15/09/2010 - Special case when Absorbance has error
                            If Not SampleList(j).IsABS_ErrorNull Then
                                If Not String.IsNullOrEmpty(SampleList(j).ABS_Error) Then
                                    dgv("ABSValue", k).Value = GlobalConstants.ABSORBANCE_ERROR
                                    dgv("Concentration", k).Value = GlobalConstants.CONC_DUE_ABS_ERROR
                                End If
                            End If
                            'END AG 15/09/2010

                            ' XB 16/01/2015 - BA-1064 
                            If Not resultRow.IsCONC_ErrorNull Then
                                If Not String.IsNullOrEmpty(resultRow.CONC_Error) Then
                                    If resultRow.TestType = "ISE" Then
                                        dgv("Concentration", k).Value = GlobalConstants.CONC_ISE_ERROR
                                    End If
                                End If
                            End If
                            ' XB 16/01/2015 - BA-1064 

                            'RH 01/02/2011 Get alarms for OffSystem tests and the others. OffSystem test do not have valid ExecutionID
                            If Not String.Equals(resultRow.TestType, "OFFS") Then
                                Remark = GetExecutionAlarmDescription(SampleList(j).ExecutionID)
                            Else
                                Remark = GetResultAlarmDescription(SampleList(j).OrderTestID, 1, 1)
                            End If

                            ' dl 27/06/2011
                            'dgv("SeeRems", k).Style.Font = SeeRemsFont
                            'If Not String.IsNullOrEmpty(Remark) Then
                            '    dgv("SeeRems", k).Value = "*"
                            'Else
                            '    dgv("SeeRems", k).Value = String.Empty
                            'End If
                            'dgv("SeeRems", k).ToolTipText = Remark
                            ' end dl 27/06/2011

                            dgv("Remarks", k).Value = Remark

                            'RH 03/08/2011
                            If Not String.IsNullOrEmpty(Remark) Then
                                'dgv("No", k).Value = "* " & SampleList(j).ReplicateNumber

                                'TR 07/06/2012 -Set the letter to the replicates
                                myLetter = ""
                                'Validate if there is a range alarm to replace the asterisk by the corresponding letter.
                                myLetter = GetReplicatesRangeAlarmsLetter(SampleList(j).ExecutionID)
                                If String.Equals(myLetter, String.Empty) Then
                                    dgv("No", k).Value = myLetter & " " & SampleList(j).ReplicateNumber
                                Else
                                    dgv("No", k).Value = SampleList(j).ReplicateNumber & " " & myLetter
                                End If
                                'TR 07/06/2012 
                            Else
                                dgv("No", k).Value = SampleList(j).ReplicateNumber
                            End If
                            dgv("No", k).ToolTipText = Remark
                            'RH 03/08/2011

                            dgv("ResultDate", k).Value = SampleList(j).ResultDate.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                         SampleList(j).ResultDate.ToString(SystemInfoManager.OSLongTimeFormat)

                            dgv.Rows(k).Tag = SampleList(j)

                            dgv("TestName", k).Value = dgv("TestName", MaxRows).Value
                            dgv("TestName", k).ToolTipText = dgv("TestName", MaxRows).ToolTipText 'AG 22/09/2014 - BA-1940 show specimen in the grid (testname column)
                            dgv("Rerun", k).Value = dgv("Rerun", MaxRows).Value
                            dgv("SampleType", k).Value = dgv("SampleType", MaxRows).Value
                            dgv("Unit", k).Value = dgv("Unit", MaxRows).Value
                            dgv("ReferenceRanges", k).Value = dgv("ReferenceRanges", MaxRows).Value
                            'dgv("ExportStatus", k).Value = Nothing
                            dgv("ExportStatus", k).Value = NoImage
                        Next

                        If Not Striked Then
                            MergeCells(dgv, "TestName", MaxRows + 1, SampleList.Count)
                            'MergeCells(dgv, "Rerun", MaxRows + 1, SampleList.Count)
                            MergeCells(dgv, "SampleType", MaxRows + 1, SampleList.Count)
                            MergeCells(dgv, "Unit", MaxRows + 1, SampleList.Count)
                            MergeCells(dgv, "ReferenceRanges", MaxRows + 1, SampleList.Count)
                        End If

                        If Not IsColCollapsed.ContainsKey(SamplesListViewText & "_SAMPLE") Then
                            CType(dgv(CollapseColName, MaxRows), bsDataGridViewCollapseCell).IsCollapsed = resultRow.Collapsed
                        End If

                    Else
                        CType(dgv(CollapseColName, MaxRows), bsDataGridViewCollapseCell).IsEnabled = False
                        'DL 23/09/2011
                        'dgv("Rep", i).Value = EQUAL_REP
                        SetPostDilutionText(dgv, "Rep", MaxRows, "EQUAL_REP", CInt(dgv("Rerun", MaxRows).Value))
                        'DL 23/09/2011
                    End If

                    MaxRows += SampleList.Count + 1
                End If
            Next

            If MaxRows < dgv.Rows.Count Then
                For j As Integer = MaxRows To dgv.Rows.Count - 1
                    dgv.Rows(j).Visible = False
                Next
            End If

            'Set the rows count for the Collapse Column
            CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = MaxRows

            If IsColCollapsed.ContainsKey(SamplesListViewText & "_SAMPLE") Then
                If IsColCollapsed(SamplesListViewText & "_SAMPLE") Then
                    CollapseAll(dgv)
                Else
                    ExpandAll(dgv)
                End If
            End If

            'Dim ElapsedTime As Double = Now.Subtract(StartTime).TotalMilliseconds
            'MessageBox.Show("UpdateExperimentalsDataGrid Elapsed Time: " & ElapsedTime.ToStringWithDecimals(0))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " UpdateExperimentalsDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            Application.DoEvents()
            dgv.Enabled = True
            Cursor = Cursors.Default
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("IResults UpdateExperimentalDataGrid (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0) & _
                                            " OPEN TAB: " & bsTestDetailsTabControl.SelectedTab.Name, _
                                            "IResults.UpdateExperimentalDataGrid", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        End Try
    End Sub

#End Region

End Class