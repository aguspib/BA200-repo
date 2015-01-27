Option Explicit On
Option Strict On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Controls.UserControls
'AG 26/07/2010


Partial Class UiResults

    Dim CollapseColumnCalibrators As New bsDataGridViewCollapseColumn

#Region "CalibratorsDataGridView Methods"
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' '<param name="pLanguageID">parameter has the current language session </param>
    ''' Modified by: PG 14/10/2010 - Add the LanguageID parameter
    ''' Modified by: RH 18/10/2010 - Remove the LanguageID parameter. Now it is a class property.
    ''' AG 03/03/2011 - view most important results without scrolling (change column witdh, ...)
    ''' Modified by: JC 09/11/2012 - Modify column width.
    ''' </remarks>
    Private Sub InitializeCalibratorsGrid()
        'Private Sub InitializeCalibratorsGrid()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsCalibratorsDataGridView.Columns.Clear()
            bsCalibratorsDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically
            bsCalibratorsDataGridView.EditingPanel.BorderStyle = BorderStyle.Fixed3D
            bsCalibratorsDataGridView.EditingPanel.Font = RegularFont
            bsCalibratorsDataGridView.EditingPanel.ForeColor = Color.Black
            bsCalibratorsDataGridView.EditingPanel.BackColor = Color.White
            'CalibratorsDataGridView.EditingControl.Padding = New Padding(3)
            'CalibratorsDataGridView.EditingControl.PreProcessMessage()

            'Dim CollapseColumn As New bsDataGridViewCollapseColumn
            With CollapseColumnCalibrators
                .Name = CollapseColName
                AddHandler .HeaderClickEventHandler, AddressOf GenericDataGridView_CellMouseClick
            End With
            bsCalibratorsDataGridView.Columns.Add(CollapseColumnCalibrators)

            'dl 23/09/2011
            'Dim OkColumn As New DataGridViewCheckBoxColumn
            Dim OkColumn As New DataGridViewImageColumn
            With OkColumn
                .Name = "Ok"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_OK", LanguageID)
                .Width = 35
            End With
            bsCalibratorsDataGridView.Columns.Add(OkColumn)
            'DL 23/09/2011
            'JC 12/11/2012
            Dim NewRepColumn As New DataGridViewImageColumn
            With NewRepColumn
                .Name = "NewRep"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_New", LanguageID)
                .Width = 50
            End With
            bsCalibratorsDataGridView.Columns.Add(NewRepColumn)

            Dim NameColumn As New DataGridViewTextBoxSpanColumn
            With NameColumn
                .Name = "TestName"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", LanguageID)
                .Width = 120
            End With
            bsCalibratorsDataGridView.Columns.Add(NameColumn)

            Dim LotColumn As New DataGridViewTextBoxSpanColumn
            With LotColumn
                .Name = "Lot"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", LanguageID)
                .Width = 60
            End With
            bsCalibratorsDataGridView.Columns.Add(LotColumn)


            Dim RerunColumn As New DataGridViewTextBoxSpanColumn
            With RerunColumn
                .Name = "Rerun"
                .HeaderText = "" 'myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Rerun", LanguageID)
                .Visible = False
                .Width = 50
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End With
            bsCalibratorsDataGridView.Columns.Add(RerunColumn)

            'bsCalibratorsDataGridView.Columns.Add("SeeRems", "")
            'bsCalibratorsDataGridView.Columns("SeeRems").Width = 33
            'bsCalibratorsDataGridView.Columns("SeeRems").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            ' JC 09/11/2012
            bsCalibratorsDataGridView.Columns.Add("SampleType", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", LanguageID))
            bsCalibratorsDataGridView.Columns("SampleType").Width = 45

            'AG 18/07/2012 - this column will be used for graph and for curve 
            Dim GraphColumn As New DataGridViewImageColumn
            With GraphColumn
                .Name = "Graph"
                .HeaderText = ""
                .Width = 30
            End With
            bsCalibratorsDataGridView.Columns.Add(GraphColumn)

            'AG 18/07/2012 - this solution does not work when several sample types for 1 test
            '' new dl 23/03/2011
            'Dim GraphColumnCurve As New DataGridViewImageColumn
            'With GraphColumnCurve
            '    .Name = "Curve"
            '    .HeaderText = ""
            '    .Width = 30
            'End With
            'bsCalibratorsDataGridView.Columns.Add(GraphColumnCurve)
            '' end dl 23/03/2011

            ' JC 09/11/2012
            bsCalibratorsDataGridView.Columns.Add("No", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", LanguageID))
            ' bsCalibratorsDataGridView.Columns("No").Width = 33
            bsCalibratorsDataGridView.Columns("No").Width = 33
            bsCalibratorsDataGridView.Columns("No").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            bsCalibratorsDataGridView.Columns.Add("ABSValue", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", LanguageID))
            bsCalibratorsDataGridView.Columns("ABSValue").Width = 55
            bsCalibratorsDataGridView.Columns("ABSValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

            Dim ConcentrationColumn As New DataGridViewTextBoxSpanColumn
            With ConcentrationColumn
                .Name = "Concentration"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TheoricalConc_Short", LanguageID)
                .Width = 55
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
            End With
            bsCalibratorsDataGridView.Columns.Add(ConcentrationColumn)

            ' JC 09/11/2012
            Dim UnitColumn As New DataGridViewTextBoxSpanColumn
            With UnitColumn
                .Name = "Unit"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", LanguageID)
                ' .Width = 55
                .Width = 55
            End With
            bsCalibratorsDataGridView.Columns.Add(UnitColumn)

            ' JC 09/11/2012
            Dim FactorColumn As New DataGridViewTextBoxSpanColumn
            With FactorColumn
                .Name = "Factor"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibFactor", LanguageID)
                .Width = 70
                ' .Width = 90
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End With
            bsCalibratorsDataGridView.Columns.Add(FactorColumn)

            Dim LimitsColumn As New DataGridViewTextBoxSpanColumn
            With LimitsColumn
                .Name = "Limits"
                .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FactorLimits", LanguageID)
                .Width = 110
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .HeaderCell.Style.WrapMode = DataGridViewTriState.True
            End With
            bsCalibratorsDataGridView.Columns.Add(LimitsColumn)

            bsCalibratorsDataGridView.Columns.Add("Remarks", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", LanguageID))
            bsCalibratorsDataGridView.Columns("Remarks").Width = 300
            bsCalibratorsDataGridView.Columns("Remarks").DefaultCellStyle.WrapMode = DataGridViewTriState.True

            bsCalibratorsDataGridView.Columns.Add("ResultDate", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", LanguageID))
            bsCalibratorsDataGridView.Columns("ResultDate").Width = 140 'AG 15/08/2010 (120)
            bsCalibratorsDataGridView.Columns("ResultDate").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            'DL 23/09/2011
            'Dim RepColumn As New DataGridViewImageColumn
            'With RepColumn
            '    .Name = "Rep"
            '    .HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Replicate_Short", LanguageID)
            '    .Width = 35
            'End With
            'bsCalibratorsDataGridView.Columns.Add(RepColumn)
            bsCalibratorsDataGridView.Columns.Add("Rep", myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITTLE_RepetitionMode", LanguageID))
            bsCalibratorsDataGridView.Columns("Rep").Width = 200
            bsCalibratorsDataGridView.Columns("Rep").DefaultCellStyle.WrapMode = DataGridViewTriState.True
            'DL 23/09/2011

            ' dl 28/07/2011
            bsCalibratorsDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            bsCalibratorsDataGridView.RowTemplate.Height = 22

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeCalibratorsGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Defines the CalibratorsDataGridView column names to be sorted
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH - 10/09/2010
    ''' </remarks>
    Private Sub DefineCalibratorsSortedColumns()
        Try
            'Insert here the column names to be sorted
            CalibratorSortType("TestName") = SortType.ASC
            CalibratorSortType("SampleType") = SortType.ASC
            CalibratorSortType("ResultDate") = SortType.ASC

            'For drawing the SortGlyph
            For Each Col As DataGridViewColumn In bsCalibratorsDataGridView.Columns
                If CalibratorSortType.ContainsKey(Col.Name) Then
                    Col.SortMode = DataGridViewColumnSortMode.Programmatic
                Else
                    Col.SortMode = DataGridViewColumnSortMode.NotSortable
                End If
            Next

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " DefineCalibratorsSortedColumns ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Fills the CalibratorsDataGridView with test name associated data
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 21/07/2010
    ''' Modified by: SA 12/11/2010 - Show Date and Time using the format defined in the OS Culture Info
    ''' AG 18/07/2012 - adapt for case calibrator for 1 test with several sample types results 
    ''' </remarks>
    Private Sub UpdateCalibratorsDataGrid()
        If isClosingFlag Then Exit Sub ' XB 24/02/2014 - #1496 No refresh if screen is closing

        'Dim StartTime As DateTime = Now

        Dim dgv As BSDataGridView = bsCalibratorsDataGridView
        Dim calibratorMaxItemNumber As Integer = 1

        Try
            Dim TestName As String = TestsListViewText
            Dim Remark As String = Nothing

            'AG 18/07/2012 -  Don't repaint the grid
            If String.Equals(CalibratorTestName, TestName) Then Return

            dgv.Enabled = False
            Dim refreshModeFlag As Boolean = False 'AG 21/06/2012
            If Not copyRefreshDS Is Nothing Then refreshModeFlag = True 'AG 21/06/2012

            RemoveOldSortGlyph(dgv)

            Dim TestList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow) = _
                        (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                         Where String.Equals(row.TestName, TestName) _
                         AndAlso String.Equals(row.SampleClass, "CALIB") _
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

            Dim sampleTypeList As List(Of String) = (From row In TestList Select row.SampleType Distinct).ToList 'AG 18/07/2012 - different sample types
            'AG 18/07/2012 - move definition here!! (TR 10/07/2012 define inside loop
            Dim MaxRows As Integer = 0
            Dim myAverageCellStyle As New DataGridViewCellStyle
            myAverageCellStyle.BackColor = AverageBkColor
            myAverageCellStyle.SelectionBackColor = AverageBkColor
            myAverageCellStyle.ForeColor = AverageForeColor
            myAverageCellStyle.SelectionForeColor = AverageForeColor

            Dim myRegularCellStyle As New DataGridViewCellStyle
            myRegularCellStyle.Font = RegularFont
            myRegularCellStyle.BackColor = RegularBkColor
            myRegularCellStyle.SelectionBackColor = RegularBkColor
            myRegularCellStyle.ForeColor = RegularForeColor
            myRegularCellStyle.SelectionForeColor = RegularForeColor

            Dim myStrikeCellStyle As New DataGridViewCellStyle
            myStrikeCellStyle.Font = StrikeFont
            myStrikeCellStyle.BackColor = StrikeBkColor
            myStrikeCellStyle.SelectionBackColor = StrikeBkColor
            myStrikeCellStyle.ForeColor = StrikeForeColor
            myStrikeCellStyle.SelectionForeColor = StrikeForeColor
            'AG 18/07/2012

            For Each myType As String In sampleTypeList
                'Dim AverageList As List(Of ResultsDS.vwksResultsRow) = _
                '                (From row In AverageResultsDS.vwksResults _
                '                 Where String.Equals(row.TestName, TestName) _
                '                 AndAlso String.Equals(row.OrderID, TestList(0).OrderID) _
                '                 Select row).ToList()

                Dim AverageList As List(Of ResultsDS.vwksResultsRow) = _
                                (From row In AverageResultsDS.vwksResults _
                                Where String.Equals(row.TestName, TestName) _
                                AndAlso String.Equals(row.SampleType, myType) _
                                AndAlso String.Equals(row.SampleClass, "CALIB") _
                                Select row).ToList()

                calibratorMaxItemNumber = (From row In AverageList _
                                           Select row.MultiPointNumber).Max

                If calibratorMaxItemNumber = 1 Then 'Show the calibrator 1 item grid

                    EnableCollapseColumn(dgv) 'RH 23/04/2012

                    'AG 18/07/2012 comment line - ' Don't repaint the grid
                    'If String.Equals(CalibratorTestName, TestName) Then Return

                    CalibratorTestName = TestName

                    'AG 18/07/2012 - comment lines
                    'Dim MaxRows As Integer = 0 
                    'TR 10/07/2012 -Create a average cellstyle
                    'Dim myAverageCellStyle As New DataGridViewCellStyle
                    'myAverageCellStyle.BackColor = AverageBkColor
                    'myAverageCellStyle.SelectionBackColor = AverageBkColor
                    'myAverageCellStyle.ForeColor = AverageForeColor
                    'myAverageCellStyle.SelectionForeColor = AverageForeColor

                    'Dim myRegularCellStyle As New DataGridViewCellStyle
                    'myRegularCellStyle.Font = RegularFont
                    'myRegularCellStyle.BackColor = RegularBkColor
                    'myRegularCellStyle.SelectionBackColor = RegularBkColor
                    'myRegularCellStyle.ForeColor = RegularForeColor
                    'myRegularCellStyle.SelectionForeColor = RegularForeColor

                    'Dim myStrikeCellStyle As New DataGridViewCellStyle
                    'myStrikeCellStyle.Font = StrikeFont
                    'myStrikeCellStyle.BackColor = StrikeBkColor
                    'myStrikeCellStyle.SelectionBackColor = StrikeBkColor
                    'myStrikeCellStyle.ForeColor = StrikeForeColor
                    'myStrikeCellStyle.SelectionForeColor = StrikeForeColor
                    'TR 10/07/2012 -END.
                    'AG 18/07/2012 - comment lines

                    For Each resultRow As ResultsDS.vwksResultsRow In AverageList
                        'AG 21/06/2012 - use AverageList instead of AverageResultsDS.vwksResults 'RH 23/04/2012
                        Dim RerunList As List(Of Integer) = _
                                (From row In AverageList _
                                 Where row.OrderTestID = resultRow.OrderTestID _
                                 Select row.RerunNumber Distinct).ToList()

                        Dim MaxRerunNumber As Integer = RerunList.Count
                        'END RH 23/04/2012

                        'dgv.Rows.Add()
                        If MaxRows >= dgv.Rows.Count Then
                            dgv.Rows.Add()
                        Else
                            dgv.Rows(MaxRows).Visible = True
                            dgv.Rows(MaxRows).DefaultCellStyle.Font = RegularFont 'AG 25/11/2010

                            'RH 21/10/2011
                            MergeCells(dgv, "TestName", MaxRows, 1)
                            MergeCells(dgv, "Lot", MaxRows, 1)
                            'MergeCells(dgv, "Rerun", MaxRows, 1)
                            MergeCells(dgv, "Concentration", MaxRows, 1)
                            MergeCells(dgv, "Unit", MaxRows, 1)
                            'MergeCells(dgv, "Factor", i + 1, TestList.Count)
                            MergeCells(dgv, "Limits", MaxRows, 1)
                        End If

                        dgv.Rows(MaxRows).DefaultCellStyle = myAverageCellStyle 'TR 10/07/2012 Set the created cellstyle.

                        'SetSubHeaderColors(dgv, i)
                        'dgv.Rows(MaxRows).DefaultCellStyle.BackColor = AverageBkColor
                        'dgv.Rows(MaxRows).DefaultCellStyle.SelectionBackColor = AverageBkColor
                        'dgv.Rows(MaxRows).DefaultCellStyle.ForeColor = AverageForeColor
                        'dgv.Rows(MaxRows).DefaultCellStyle.SelectionForeColor = AverageForeColor

                        'RH 05/11/2010, AG 10/11/2010
                        'Check if Factor value is calculated or user defined, and put the flag value here
                        'If Not resultRow.ManualFactorFlag Then
                        '    dgv.Rows(i).DefaultCellStyle.Font = RegularFont
                        'Else
                        '    dgv.Rows(i).DefaultCellStyle.Font = StrikeFont
                        'End If

                        CType(dgv(CollapseColName, MaxRows), bsDataGridViewCollapseCell).IsSubHeader = True

                        'DL 23/09/2011
                        If resultRow.AcceptedResultFlag Then
                            dgv("Ok", MaxRows).Value = OKImage
                        Else
                            dgv("Ok", MaxRows).Value = UnCheckImage 'NoImage 
                        End If
                        'dgv("Ok", MaxRows).Value = resultRow.AcceptedResultFlag
                        'DL 23/09/2011

                        'AG 15/08/2010
                        'If resultRow.Equals(AverageList(AverageList.Count - 1)) Then
                        Dim sentRepetitionCriteria As String = ""
                        If Me.AllowManualRepetitions(resultRow, MaxRerunNumber, "CALIBRATOR", sentRepetitionCriteria) Then
                            SetRepPostDilutionImage(dgv, "NewRep", MaxRows, resultRow.PostDilutionType, True)
                        Else
                            'dgv("NewRep", MaxRows).Value = NoImage
                            SetRepPostDilutionImage(dgv, "NewRep", MaxRows, sentRepetitionCriteria, False)
                        End If

                        'dgv("TestName", MaxRows).Value = TestName
                        'dgv("TestName", MaxRows).Value = IIf(resultRow.RerunNumber = 1, TestName, TestName & " (" & resultRow.RerunNumber.ToString & ")")
                        'RH 20/10/2011 String.Format() is a method optimized for Strings concatenations
                        dgv("TestName", MaxRows).Value = IIf(resultRow.RerunNumber = 1, TestName, String.Format("{0} ({1})", TestName, resultRow.RerunNumber))

                        dgv("Lot", MaxRows).Value = resultRow.CalibratorLotNumber
                        dgv("Rerun", MaxRows).Value = resultRow.RerunNumber
                        dgv("SampleType", MaxRows).Value = resultRow.SampleType
                        dgv("No", MaxRows).Value = Nothing
                        dgv("ABSValue", MaxRows).Value = resultRow.ABSValue.ToString(GlobalConstants.ABSORBANCE_FORMAT)
                        dgv("Concentration", MaxRows).Value = resultRow.TheoricalConcentration.ToStringWithDecimals(resultRow.DecimalsAllowed)
                        dgv("Unit", MaxRows).Value = resultRow.MeasureUnit

                        'AG 10/11/2010
                        If Not resultRow.ManualResultFlag Then
                            dgv.Rows(MaxRows).DefaultCellStyle.Font = RegularFont
                        Else
                            dgv.Rows(MaxRows).DefaultCellStyle.Font = StrikeFont
                            dgv.Rows(MaxRows).Cells("Factor").Style.Font = RegularFont
                        End If
                        'END AG 10/11/2010

                        If Not resultRow.IsCalibratorFactorNull Then
                            If Not resultRow.ManualResultFlag Then
                                If String.Equals(resultRow.CalibrationError.Trim, String.Empty) Then
                                    'AG 22/10/2012
                                    'dgv("Factor", MaxRows).Value = resultRow.CalibratorFactor.ToString("#####0.####")
                                    dgv("Factor", MaxRows).Value = resultRow.CalibratorFactor.ToString(GlobalConstants.CALIBRATOR_FACTOR_FORMAT.ToString)
                                Else
                                    dgv("Factor", MaxRows).Value = GlobalConstants.FACTOR_NOT_CALCULATED
                                End If
                            Else
                                'AG 22/10/2012
                                'TR 10/12/2010 -Set the format value #####.####
                                'dgv("Factor", MaxRows).Value = resultRow.ManualResult.ToString("#####0.####")
                                dgv("Factor", MaxRows).Value = resultRow.ManualResult.ToString(GlobalConstants.CALIBRATOR_FACTOR_FORMAT.ToString)
                            End If

                        Else
                            dgv("Factor", MaxRows).Value = Nothing
                        End If
                        'END AG 02/08/2010

                        If Not resultRow.IsFactorLowerLimitNull AndAlso Not resultRow.IsFactorUpperLimitNull Then
                            dgv("Limits", MaxRows).Value = String.Format("{0} - {1}", resultRow.FactorLowerLimit, resultRow.FactorUpperLimit)
                        Else
                            dgv("Limits", MaxRows).Value = Nothing 'AG 24/10/2012
                        End If

                        'AG 15/09/2010 - Special case when Absorbance has error
                        If Not resultRow.IsABS_ErrorNull Then
                            If Not String.IsNullOrEmpty(resultRow.ABS_Error) Then
                                dgv("ABSValue", MaxRows).Value = GlobalConstants.ABSORBANCE_ERROR
                                'Also change value for Factor? By now no!
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
                        ' end dl 27/06/2011
                        'dgv("SeeRems", MaxRows).ToolTipText = Remark
                        dgv("Remarks", MaxRows).Value = Remark

                        'If Not Remark.ToString.Trim Is String.Empty Then dgv("No", MaxRows).Value = "*" 'DL 22/09/2011
                        If Not String.Equals(Remark, String.Empty) Then dgv("No", MaxRows).Value = "*" 'RH 21/10/2011
                        'RH 15/11/2011
                        dgv("No", MaxRows).ToolTipText = Remark
                        dgv("Graph", MaxRows).Value = AVG_ABS_GRAPHImage
                        dgv("Graph", MaxRows).ToolTipText = labelOpenAbsorbanceCurve

                        'AG 18/07/2012 - code wrong, do not works when several sample type for 1 test (use only 1 column) ' dl 23/03/2011
                        ''DL 23/03/2011 - Begin
                        'dgv("Curve", MaxRows).Value = NoImage
                        'dgv.Columns("Graph").Visible = True
                        'dgv.Columns("Curve").Visible = False
                        'DL 23/03/2011 - End
                        dgv.Columns("Graph").Visible = True
                        dgv("Graph", MaxRows).Tag = "" 'AG 18/07/2012 - New solution "CURVE" open calibration curve / else open absorbance (time) graphical
                        'AG 18/07/2012

                        dgv("ResultDate", MaxRows).Value = resultRow.ResultDateTime.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                     resultRow.ResultDateTime.ToString(SystemInfoManager.OSLongTimeFormat)

                        'AG 10/11/2010
                        'If Not resultRow.ManualFactorFlag Then
                        '    dgv.Rows(i).DefaultCellStyle.Font = RegularFont
                        'Else
                        '    dgv.Rows(i).DefaultCellStyle.Font = StrikeFont
                        '    dgv.Columns("Factor").DefaultCellStyle.Font = RegularFont
                        'End If
                        'END AG 10/11/2010

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
                            SetPostDilutionText(dgv, "Rep", MaxRows, AverageList(0).PostDilutionType, AverageList(0).RerunNumber)
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
                                    MergeCells(dgv, "Lot", k, 1)
                                    'MergeCells(dgv, "Rerun", k, 1)
                                    MergeCells(dgv, "Concentration", k, 1)
                                    MergeCells(dgv, "Unit", k, 1)
                                    'MergeCells(dgv, "Factor", i + 1, TestList.Count)
                                    MergeCells(dgv, "Limits", k, 1)
                                End If

                                'SetStrike(dgv, k, TestList(j).InUse)
                                If TestList(j).InUse Then
                                    dgv.Rows(k).DefaultCellStyle = myRegularCellStyle 'TR 10/07/2012 -Set the created cellstyle.
                                    'dgv.Rows(k).DefaultCellStyle.Font = RegularFont
                                    'dgv.Rows(k).DefaultCellStyle.BackColor = RegularBkColor
                                    'dgv.Rows(k).DefaultCellStyle.SelectionBackColor = RegularBkColor
                                    'dgv.Rows(k).DefaultCellStyle.ForeColor = RegularForeColor
                                    'dgv.Rows(k).DefaultCellStyle.SelectionForeColor = RegularForeColor
                                Else
                                    dgv.Rows(k).DefaultCellStyle = myStrikeCellStyle 'TR 10/07/2012 -Set the created cellstyle.
                                    'dgv.Rows(k).DefaultCellStyle.Font = StrikeFont
                                    'dgv.Rows(k).DefaultCellStyle.BackColor = StrikeBkColor
                                    'dgv.Rows(k).DefaultCellStyle.SelectionBackColor = StrikeBkColor
                                    'dgv.Rows(k).DefaultCellStyle.ForeColor = StrikeForeColor
                                    'dgv.Rows(k).DefaultCellStyle.SelectionForeColor = StrikeForeColor
                                    Striked = True
                                End If

                                CType(dgv(CollapseColName, k), bsDataGridViewCollapseCell).IsSubHeader = False

                                dgv("Rep", k).Value = "" 'NoImage dl 23/09/2011
                                dgv("Ok", k).Value = NoImage
                                dgv("NewRep", k).Value = NoImage

                                dgv("Graph", k).Value = ABS_GRAPHImage
                                dgv("Graph", k).ToolTipText = labelOpenAbsorbanceCurve

                                'dgv("Curve", k).Value = NoImage
                                'DL 23/03/2011 - Begin
                                dgv.Columns("Graph").Visible = True
                                'dgv.Columns("Curve").Visible = False
                                'DL 23/03/2011 - End
                                dgv("Graph", k).Tag = "" 'AG 18/07/2012 - New solution "CURVE" open calibration curve / else open absorbance (time) graphical

                                'dgv("Curve", k).ToolTipText = CurveToolTip
                                dgv("SampleType", k).Value = TestList(j).SampleType
                                dgv("ABSValue", k).Value = TestList(j).ABS_Value.ToString(GlobalConstants.ABSORBANCE_FORMAT)

                                'AG 15/09/2010 - Special case when Absorbance has error
                                If Not TestList(j).IsABS_ErrorNull Then
                                    If Not String.IsNullOrEmpty(TestList(j).ABS_Error) Then
                                        dgv("ABSValue", k).Value = GlobalConstants.ABSORBANCE_ERROR
                                    End If
                                End If
                                'END AG 15/09/2010

                                Remark = GetExecutionAlarmDescription(TestList(j).ExecutionID)

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

                                dgv("TestName", k).Value = dgv("TestName", MaxRows).Value
                                dgv("Lot", k).Value = dgv("Lot", MaxRows).Value
                                dgv("Rerun", k).Value = dgv("Rerun", MaxRows).Value
                                dgv("Concentration", k).Value = dgv("Concentration", MaxRows).Value
                                dgv("Unit", k).Value = dgv("Unit", MaxRows).Value
                                'dgv("Factor", i + 1).Value = dgv("Factor", i).Value
                                dgv("Limits", k).Value = dgv("Limits", MaxRows).Value
                            Next j

                            If Not Striked Then
                                MergeCells(dgv, "TestName", MaxRows + 1, TestList.Count)
                                MergeCells(dgv, "Lot", MaxRows + 1, TestList.Count)
                                'MergeCells(dgv, "Rerun", MaxRows + 1, TestList.Count)
                                MergeCells(dgv, "Concentration", MaxRows + 1, TestList.Count)
                                MergeCells(dgv, "Unit", MaxRows + 1, TestList.Count)
                                'MergeCells(dgv, "Factor", i + 1, TestList.Count)
                                MergeCells(dgv, "Limits", MaxRows + 1, TestList.Count)
                            End If

                            If Not IsColCollapsed.ContainsKey(TestsListViewText & "_CALIB") Then
                                CType(dgv(CollapseColName, MaxRows), bsDataGridViewCollapseCell).IsCollapsed = resultRow.Collapsed
                            End If

                        Else
                            'DL 23/09/2011
                            'dgv("Rep", MaxRows).Value = EQUAL_REP
                            SetPostDilutionText(dgv, "Rep", MaxRows, "EQUAL_REP", CInt(dgv("Rerun", MaxRows).Value))
                        End If

                        MaxRows += TestList.Count + 1

                    Next resultRow

                    'Set the rows count for the Collapse Column
                    CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = MaxRows

                    If IsColCollapsed.ContainsKey(TestsListViewText & "_CALIB") Then
                        If IsColCollapsed(TestsListViewText & "_CALIB") Then
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
                Else
                    'Set the rows count for the Collapse Column
                    CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = 0

                    UpdateCalibratorsMuliItemDataGrid(AverageList(0).OrderTestID, MaxRows) 'Show the calibrator multi point grid

                    'END AG 31/08/2010

                    DisableCollapseColumn(dgv) 'RH 23/04/2012
                End If
                'END AG 02/08/2010

            Next


            'Dim ElapsedTime As Double = Now.Subtract(StartTime).TotalMilliseconds
            'MessageBox.Show("UpdateCalibratorsDataGrid Elapsed Time: " & ElapsedTime.ToStringWithDecimals(0))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & " UpdateCalibratorsDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            dgv.Enabled = True
            Application.DoEvents()
        End Try
    End Sub

    ''' <summary>
    ''' Fills the CalibratorsDataGridView with test name associated data when the calibrator is a multi item element
    ''' </summary>
    ''' <param name="pOrderTestID"></param>
    ''' <param name="pMaxRows"></param>
    ''' <remarks>
    ''' Created by:  AG 03/08/2010 (based on UpdateCalibratorsDataGrid)
    ''' Modified by: RH 08/10/2010
    '''              SA 12/11/2010 - Show Date and Time using the format defined in the OS Culture Info
    '''              RH 23/04/2012 - Introduce RerunNumber into the algorithm
    '''              AG 18/07/2012 - Adapt for case calibrator for 1 test with several sample type
    ''' </remarks>
    Private Sub UpdateCalibratorsMuliItemDataGrid(ByVal pOrderTestID As Integer, ByRef pMaxRows As Integer)
        Try
            If isClosingFlag Then Exit Sub ' XB 24/02/2014 - #1496 No refresh if screen is closing

            Dim TestName As String = TestsListViewText

            ' AG 18/07/2012 - Don't repaint the grid
            'If CalibratorTestName = TestName Then Return

            CalibratorTestName = TestName

            Dim dgv As BSDataGridView = bsCalibratorsDataGridView
            Dim Remark As String = Nothing

            Dim TheoreticalConcList As List(Of Single) = _
                    (From row In AverageResultsDS.vwksResults _
                     Where row.OrderTestID = pOrderTestID _
                     Select row.TheoricalConcentration Distinct _
                     Order By TheoricalConcentration Descending).ToList()

            If TheoreticalConcList.Count = 0 Then Return

            'AG 18/07/2012
            'Dim i As Integer = 0
            Dim i As Integer = pMaxRows

            Dim calibAvgResults As List(Of Integer) = _
                    (From row In AverageResultsDS.vwksResults _
                     Where row.OrderTestID = pOrderTestID _
                     Select row.RerunNumber Distinct).ToList()

            Dim MaxRerunNumber As Integer = calibAvgResults.Count

            For RerunNumber As Integer = 1 To MaxRerunNumber
                Dim auxRerunNumber = RerunNumber
                Dim IsAverageDone As New Dictionary(Of Integer, Boolean)

                Dim itempoint As Integer = TheoreticalConcList.Count + 1

                For Each myTheoreticalConc As Single In TheoreticalConcList
                    itempoint -= 1

                    Dim AverageList As List(Of ResultsDS.vwksResultsRow) = _
                                    (From row In AverageResultsDS.vwksResults _
                                     Where row.OrderTestID = pOrderTestID _
                                     AndAlso row.TheoricalConcentration = myTheoreticalConc _
                                     AndAlso row.MultiPointNumber = itempoint _
                                     AndAlso row.RerunNumber = auxRerunNumber _
                                     Select row).ToList()

                    'END AG 08/08/2010


                    For Each resultRow As ResultsDS.vwksResultsRow In AverageList
                        If Not IsAverageDone.ContainsKey(resultRow.MultiPointNumber) Then
                            IsAverageDone(resultRow.MultiPointNumber) = True
                            dgv.Rows.Add()
                            If i >= dgv.Rows.Count Then
                                dgv.Rows.Add()
                            Else
                                dgv.Rows(i).Visible = True
                                dgv.Rows(i).DefaultCellStyle.Font = RegularFont 'AG 25/11/2010

                                'RH 21/10/2011
                                MergeCells(dgv, "TestName", i, 1)
                                MergeCells(dgv, "Lot", i, 1)
                                'MergeCells(dgv, "Rerun", i, 1)
                                MergeCells(dgv, "Concentration", i, 1)
                                MergeCells(dgv, "Unit", i, 1)
                                'MergeCells(dgv, "Factor", i + 1, TestList.Count)
                                MergeCells(dgv, "Limits", i, 1)
                            End If

                            'SetSubHeaderColors(dgv, i)
                            dgv.Rows(i).DefaultCellStyle.BackColor = AverageBkColor
                            dgv.Rows(i).DefaultCellStyle.SelectionBackColor = AverageBkColor
                            dgv.Rows(i).DefaultCellStyle.ForeColor = AverageForeColor
                            dgv.Rows(i).DefaultCellStyle.SelectionForeColor = AverageForeColor

                            'RH 05/11/2010
                            dgv.Rows(i).DefaultCellStyle.Font = RegularFont

                            CType(dgv(CollapseColName, i), bsDataGridViewCollapseCell).IsSubHeader = True

                            'RH 12/01/2012
                            'dgv("Rep", i).Value = EQUAL_REPImage
                            SetPostDilutionText(dgv, "Rep", i, "EQUAL_REP", CInt(dgv("Rerun", i).Value))

                            If resultRow.AcceptedResultFlag Then
                                dgv("Ok", i).Value = OKImage
                            Else
                                dgv("Ok", i).Value = UnCheckImage 'NoImage
                            End If

                            'AG 15/08/2010
                            'If resultRow.Equals(AverageList(AverageList.Count - 1)) Then
                            Dim sentRepetitionCriteria As String = ""
                            If Me.AllowManualRepetitions(resultRow, MaxRerunNumber, "CALIBRATOR", sentRepetitionCriteria) Then
                                SetRepPostDilutionImage(dgv, "NewRep", i, resultRow.PostDilutionType, True)
                            Else
                                'dgv("NewRep", i).Value = NoImage
                                SetRepPostDilutionImage(dgv, "NewRep", i, sentRepetitionCriteria, False)
                            End If

                            'dgv("TestName", i).Value = resultRow.TestName
                            'dgv("TestName", i).Value = IIf(resultRow.RerunNumber = 1, resultRow.TestName, resultRow.TestName & " (" & resultRow.RerunNumber.ToString & ")")
                            'RH 20/10/2011 String.Format() is a method optimized for Strings concatenations
                            dgv("TestName", i).Value = IIf(resultRow.RerunNumber = 1, resultRow.TestName, String.Format("{0} ({1})", resultRow.TestName, resultRow.RerunNumber))

                            dgv("Lot", i).Value = resultRow.CalibratorLotNumber
                            dgv("Rerun", i).Value = resultRow.RerunNumber

                            dgv("Graph", i).Value = NoImage 'AVG_ABS_GRAPH
                            dgv("Graph", i).ToolTipText = String.Empty

                            'AG 18/07/2012 - code wrong, do not works when several sample type for 1 test (use only 1 column) ' dl 23/03/2011
                            'dgv("Curve", i).Value = CURVE_GRAPHImage
                            'dgv("Curve", i).ToolTipText = CurveToolTip
                            'dgv.Columns("Graph").Visible = False
                            'dgv.Columns("Curve").Visible = True
                            dgv("Graph", i).Value = CURVE_GRAPHImage
                            dgv("Graph", i).ToolTipText = CurveToolTip
                            dgv("Graph", i).Tag = "CURVE" 'AG 18/07/2012 - New solution "CURVE" open calibration curve / else open absorbance (time) graphical
                            dgv.Columns("Graph").Visible = True
                            'AG 18/07/2012 

                            dgv("SampleType", i).Value = resultRow.SampleType
                            'dgv("No", i).Value = Nothing

                            'If Not String.IsNullOrEmpty(Remark) Then
                            '    dgv("No", i).Value = "*"
                            'Else
                            '    dgv("No", i).Value = Nothing
                            'End If

                            dgv("ABSValue", i).Value = Nothing
                            dgv("Concentration", i).Value = Nothing
                            dgv("Unit", i).Value = Nothing 'resultRow.MeasureUnit
                            dgv("Factor", i).Value = Nothing

                            If Not resultRow.IsFactorLowerLimitNull AndAlso Not resultRow.IsFactorUpperLimitNull Then
                                dgv("Limits", i).Value = String.Format("{0} - {1}", resultRow.FactorLowerLimit, resultRow.FactorUpperLimit)
                            End If

                            Remark = GetResultAlarmDescription(resultRow.OrderTestID, resultRow.RerunNumber, resultRow.MultiPointNumber)
                            'dgv("SeeRems", i).Style.Font = SeeRemsFont
                            'If Not String.IsNullOrEmpty(Remark) Then
                            '    dgv("SeeRems", i).Value = "*"
                            'Else
                            '    dgv("SeeRems", i).Value = Nothing
                            'End If
                            'dgv("SeeRems", i).ToolTipText = Remark
                            dgv("Remarks", i).Value = Remark

                            'RH 03/08/2011
                            If Not String.IsNullOrEmpty(Remark) Then
                                dgv("No", i).Value = "*"
                            Else
                                dgv("No", i).Value = Nothing
                            End If
                            dgv("No", i).ToolTipText = Remark
                            'RH 03/08/2011

                            dgv("ResultDate", i).Value = resultRow.ResultDateTime.ToString(SystemInfoManager.OSDateFormat) & " " & _
                                                         resultRow.ResultDateTime.ToString(SystemInfoManager.OSLongTimeFormat)

                            'dgv("ResultDate", i).Value = resultRow.ResultDateTime.ToShortDateString() & _
                            '                                            " " & resultRow.ResultDateTime.ToShortTimeString()

                            dgv.Rows(i).Tag = resultRow

                            i += 1
                        End If
                    Next
                    Exit For ' For the multicalibrator only show the different reruns average
                Next ' AG 08/08/2010
            Next

            'Set the rows count for the Collapse Column
            CType(dgv.Columns(CollapseColName), bsDataGridViewCollapseColumn).RowsCount = i

            If i < dgv.Rows.Count Then
                For j As Integer = i To dgv.Rows.Count - 1
                    dgv.Rows(j).Visible = False
                Next
            End If

            pMaxRows = i 'AG 18/07/2012

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " UpdateCalibratorsMuliItemDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

#End Region

End Class