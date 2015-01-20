Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global


Public Class QCSimulator

#Region "Declarations"
    Private ChangesMade As Boolean = False
    Private TestsListDS As New TestControlsDS
    Private TextBoxesCtrl1Array As New ArrayList()
    Private TextBoxesCtrl2Array As New ArrayList()
    Private TextBoxesCtrl3Array As New ArrayList()

    Private lowerLimitCtrl1 As Single
    Private lowerLimitCtrl2 As Single
    Private lowerLimitCtrl3 As Single
    Private upperLimitCtrl1 As Single
    Private upperLimitCtrl2 As Single
    Private upperLimitCtrl3 As Single

#End Region

#Region "Attributes"
    Private AnalyzerIDAttribute As String
#End Region

#Region "Properties"
    Public WriteOnly Property AnalyzerID() As String
        Set(ByVal value As String)
            AnalyzerIDAttribute = value
        End Set
    End Property
#End Region

#Region "Methods"

    Private Sub CreateResultsArrays()
        Try
            TextBoxesCtrl1Array.Add(bsCtrl1Run1TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run2TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run3TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run4TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run5TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run6TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run7TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run8TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run9TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run10TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run11TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run12TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run13TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run14TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run15TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run16TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run17TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run18TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run19TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run20TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run21TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run22TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run23TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run24TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run25TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run26TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run27TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run28TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run29TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run30TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run31TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run32TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run33TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run34TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run35TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run36TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run37TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run38TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run39TextBox)
            TextBoxesCtrl1Array.Add(bsCtrl1Run40TextBox)
           
            TextBoxesCtrl2Array.Add(bsCtrl2Run1TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run2TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run3TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run4TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run5TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run6TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run7TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run8TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run9TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run10TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run11TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run12TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run13TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run14TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run15TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run16TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run17TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run18TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run19TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run20TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run21TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run22TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run23TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run24TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run25TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run26TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run27TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run28TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run29TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run30TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run31TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run32TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run33TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run34TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run35TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run36TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run37TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run38TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run39TextBox)
            TextBoxesCtrl2Array.Add(bsCtrl2Run40TextBox)

            TextBoxesCtrl3Array.Add(bsCtrl3Run1TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run2TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run3TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run4TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run5TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run6TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run7TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run8TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run9TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run10TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run11TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run12TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run13TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run14TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run15TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run16TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run17TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run18TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run19TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run20TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run21TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run22TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run23TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run24TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run25TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run26TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run27TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run28TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run29TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run30TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run31TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run32TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run33TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run34TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run35TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run36TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run37TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run38TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run39TextBox)
            TextBoxesCtrl3Array.Add(bsCtrl3Run40TextBox)
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".CreateResultsArrays", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub GetControlsInfo(ByVal pTestType As String, ByVal pTestID As Integer, ByVal pSampleType As String)
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestControlsDelegate As New TestControlsDelegate

            myGlobalDataTO = myTestControlsDelegate.GetAdditionalInformationNEW(Nothing, pTestType, pTestID, pSampleType)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myControlsDS As TestControlsDS = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)

                bsCtrl1Panel.Visible = False
                bsCtrl2Panel.Visible = False
                bsCtrl3Panel.Visible = False

                Dim myControlNum As Integer = 0
                For Each myTestControlRow As TestControlsDS.tparTestControlsRow In myControlsDS.tparTestControls.Rows
                    Select Case myControlNum
                        Case 0
                            bsCtrl1Panel.Visible = True
                            bsCtrl1IDLabel.Text = myTestControlRow.ControlID.ToString
                            bsCtrl1LotNumLabel.Text = myTestControlRow.LotNumber
                            bsCtrl1InfoLabel.Text = myTestControlRow.ControlName & "/" & myTestControlRow.LotNumber
                            bsCtrl1ValuesLabel.Text = "(" & myTestControlRow.MinConcentration.ToString & _
                                                      "-" & myTestControlRow.MaxConcentration.ToString & ")"

                            lowerLimitCtrl1 = myTestControlRow.MinConcentration
                            upperLimitCtrl1 = myTestControlRow.MaxConcentration
                        Case 1
                            bsCtrl2Panel.Visible = True
                            bsCtrl2IDLabel.Text = myTestControlRow.ControlID.ToString
                            bsCtrl2LotNumLabel.Text = myTestControlRow.LotNumber
                            bsCtrl2InfoLabel.Text = myTestControlRow.ControlName & "/" & myTestControlRow.LotNumber
                            bsCtrl2ValuesLabel.Text = "(" & myTestControlRow.MinConcentration.ToString & _
                                                      "-" & myTestControlRow.MaxConcentration.ToString & ")"

                            lowerLimitCtrl2 = myTestControlRow.MinConcentration
                            upperLimitCtrl2 = myTestControlRow.MaxConcentration
                        Case 2
                            bsCtrl3Panel.Visible = True
                            bsCtrl3IDLabel.Text = myTestControlRow.ControlID.ToString
                            bsCtrl3LotNumLabel.Text = myTestControlRow.LotNumber
                            bsCtrl3InfoLabel.Text = myTestControlRow.ControlName & "/" & myTestControlRow.LotNumber
                            bsCtrl3ValuesLabel.Text = "(" & myTestControlRow.MinConcentration.ToString & _
                                                      "-" & myTestControlRow.MaxConcentration.ToString & ")"

                            lowerLimitCtrl3 = myTestControlRow.MinConcentration
                            upperLimitCtrl3 = myTestControlRow.MaxConcentration
                    End Select
                    myControlNum += 1
                Next
                SetStatusForResultArea(False)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".GetControlsInfo", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub InitializeDatesRanges()
        Try
            bsDateFromDateTimePick.MinDate = DateAdd(DateInterval.Year, -5, Now)
            bsDateFromDateTimePick.MaxDate = DateAdd(DateInterval.Year, 5, Now)

            bsDateToDateTimePick.MinDate = DateAdd(DateInterval.Year, -5, Now)
            bsDateToDateTimePick.MaxDate = DateAdd(DateInterval.Year, 5, Now)

        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".InitializeDatesRanges", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub InitializeTestsListGrid()
        Try
            Dim columnName As String

            bsTestsListDataGridView.Rows.Clear()
            bsTestsListDataGridView.Columns.Clear()

            bsTestsListDataGridView.AllowUserToAddRows = False
            bsTestsListDataGridView.AllowUserToDeleteRows = False
            bsTestsListDataGridView.AutoGenerateColumns = False
            bsTestsListDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically

            'TestType Icon (Standard Preloaded, Standard User Defined or ISE)
            Dim ImageCol As New DataGridViewImageColumn
            ImageCol.Name = "TestTypeIcon"
            ImageCol.DataPropertyName = "TestTypeIcon"
            ImageCol.HeaderText = ""
            bsTestsListDataGridView.Columns.Add(ImageCol)
            bsTestsListDataGridView.Columns("TestTypeIcon").Width = 30
            bsTestsListDataGridView.Columns("TestTypeIcon").Visible = True
            bsTestsListDataGridView.Columns("TestTypeIcon").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestsListDataGridView.Columns("TestTypeIcon").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Test Name
            Dim TestNameTypeCol As New DataGridViewTextBoxColumn
            TestNameTypeCol.Name = "TestName"
            TestNameTypeCol.DataPropertyName = "TestName"
            TestNameTypeCol.HeaderText = "Test Name"
            TestNameTypeCol.ReadOnly = True
            bsTestsListDataGridView.Columns.Add(TestNameTypeCol)
            bsTestsListDataGridView.Columns("TestName").Width = 145
            bsTestsListDataGridView.Columns("TestName").SortMode = DataGridViewColumnSortMode.Automatic
            bsTestsListDataGridView.Columns("TestName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft

            'Sample Type Code
            Dim SampleTypeCol As New DataGridViewTextBoxColumn
            SampleTypeCol.Name = "SampleType"
            SampleTypeCol.DataPropertyName = "SampleType"
            SampleTypeCol.HeaderText = "Type"
            SampleTypeCol.ReadOnly = True
            bsTestsListDataGridView.Columns.Add(SampleTypeCol)
            bsTestsListDataGridView.Columns("SampleType").Width = 45
            bsTestsListDataGridView.Columns("SampleType").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestsListDataGridView.Columns("SampleType").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'Test Measure Unit
            Dim Unit As New DataGridViewTextBoxColumn
            Unit.Name = "MeasureUnit"
            Unit.DataPropertyName = "MeasureUnit"
            Unit.HeaderText = "Unit"
            Unit.ReadOnly = True
            bsTestsListDataGridView.Columns.Add(Unit)
            bsTestsListDataGridView.Columns("MeasureUnit").Width = 55
            bsTestsListDataGridView.Columns("MeasureUnit").SortMode = DataGridViewColumnSortMode.NotSortable
            bsTestsListDataGridView.Columns("MeasureUnit").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter

            'TestType column
            columnName = "TestType"
            bsTestsListDataGridView.Columns.Add(columnName, "")
            bsTestsListDataGridView.Columns(columnName).Width = 0
            bsTestsListDataGridView.Columns(columnName).Visible = False
            bsTestsListDataGridView.Columns(columnName).DataPropertyName = columnName
            bsTestsListDataGridView.Columns(columnName).ReadOnly = True

            'TestID column
            columnName = "TestID"
            bsTestsListDataGridView.Columns.Add(columnName, "")
            bsTestsListDataGridView.Columns(columnName).Width = 0
            bsTestsListDataGridView.Columns(columnName).Visible = False
            bsTestsListDataGridView.Columns(columnName).DataPropertyName = columnName
            bsTestsListDataGridView.Columns(columnName).ReadOnly = True

            'Decimals allowed for the Test
            columnName = "DecimalsAllowed"
            bsTestsListDataGridView.Columns.Add(columnName, "")
            bsTestsListDataGridView.Columns(columnName).Width = 0
            bsTestsListDataGridView.Columns(columnName).Visible = False
            bsTestsListDataGridView.Columns(columnName).DataPropertyName = columnName
            bsTestsListDataGridView.Columns(columnName).ReadOnly = True

            bsTestsListDataGridView.Columns("TestTypeIcon").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestsListDataGridView.Columns("TestName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsTestsListDataGridView.Columns("SampleType").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsTestsListDataGridView.Columns("MeasureUnit").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            bsTestsListDataGridView.ScrollBars = ScrollBars.Vertical
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".InitializeTestsListGrid", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            auxIconName = GetIconName("ADD")
            If (auxIconName <> "") Then bsAddResultsButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("SAVE")
            If (auxIconName <> "") Then bsAcceptButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("UNDO")
            If (auxIconName <> "") Then bsCancelButton.Image = Image.FromFile(iconPath & auxIconName)

            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".PrepareButtons", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SaveResults()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myResultsDelegate As New ResultsDelegate

            Dim myQCResultsDS As New QCResultsDS
            Dim qcResultsRow As QCResultsDS.tqcResultsRow
            Dim myQCResultsDelegate As New QCResultsDelegate

            Dim numControls As Integer = 1
            If (bsCtrl2Panel.Visible) Then numControls += 1
            If (bsCtrl3Panel.Visible) Then numControls += 1

            Dim allResultsOK As Boolean = True
            Dim firstDateTime As DateTime = bsDateFromDateTimePick.Value
            myGlobalDataTO = myQCResultsDelegate.GetFirstDateTimeForResultsCreationNEW(Nothing, AnalyzerIDAttribute, _
                                                                                       bsTestsListDataGridView.SelectedRows(0).Cells("TestType").Value.ToString, _
                                                                                       Convert.ToInt32(bsTestsListDataGridView.SelectedRows(0).Cells("TestID").Value), _
                                                                                       bsTestsListDataGridView.SelectedRows(0).Cells("SampleType").Value.ToString)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim maxDateTime As DateTime
                If (Not myGlobalDataTO.SetDatos Is DBNull.Value) Then
                    maxDateTime = Convert.ToDateTime(myGlobalDataTO.SetDatos)
                    If (maxDateTime > firstDateTime) Then firstDateTime = DateAdd(DateInterval.Day, 1, Convert.ToDateTime(myGlobalDataTO.SetDatos))
                End If
            End If

            Dim myDateTime As Date
            Dim myTextBox1 As New TextBox
            Dim myTextBox2 As New TextBox
            Dim myTextBox3 As New TextBox

            Dim myCtrlSendingGroup As Integer = 0
            For i As Integer = 1 To Convert.ToInt32(bsNumResultsNumericUpDown.Value)
                If (myCtrlSendingGroup = 0) Then
                    myGlobalDataTO = myQCResultsDelegate.GetMaxCtrlsSendingGroup(Nothing, AnalyzerIDAttribute)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myCtrlSendingGroup = CInt(myGlobalDataTO.SetDatos) + 1
                    End If
                Else
                    myCtrlSendingGroup += 1
                End If

                myDateTime = DateAdd(DateInterval.Day, i, firstDateTime)

                myTextBox1 = CType(TextBoxesCtrl1Array(i - 1), TextBox)
                If (myTextBox1.Text <> String.Empty) Then
                    qcResultsRow = myQCResultsDS.tqcResults.NewtqcResultsRow
                    qcResultsRow.AnalyzerID = AnalyzerIDAttribute
                    qcResultsRow.TestType = bsTestsListDataGridView.SelectedRows(0).Cells("TestType").Value.ToString
                    qcResultsRow.TestID = Convert.ToInt32(bsTestsListDataGridView.SelectedRows(0).Cells("TestID").Value)
                    qcResultsRow.SampleType = bsTestsListDataGridView.SelectedRows(0).Cells("SampleType").Value.ToString
                    qcResultsRow.ControlID = Convert.ToInt32(bsCtrl1IDLabel.Text)
                    qcResultsRow.LotNumber = bsCtrl1LotNumLabel.Text
                    qcResultsRow.RerunNumber = 1
                    qcResultsRow.ResultValue = Convert.ToSingle(myTextBox1.Text)
                    qcResultsRow.ResultDateTime = myDateTime
                    qcResultsRow.SetResultCommentNull()
                    qcResultsRow.ManualResultFlag = False
                    qcResultsRow.SetManualResultValueNull()
                    qcResultsRow.CtrlsSendingGroup = myCtrlSendingGroup
                    myQCResultsDS.tqcResults.AddtqcResultsRow(qcResultsRow)
                Else
                    If (numControls = 1) Then allResultsOK = False
                End If

                If (bsCtrl2Panel.Visible) Then
                    myTextBox2 = CType(TextBoxesCtrl2Array(i - 1), TextBox)
                    If (myTextBox2.Text <> String.Empty) Then
                        qcResultsRow = myQCResultsDS.tqcResults.NewtqcResultsRow
                        qcResultsRow.AnalyzerID = AnalyzerIDAttribute
                        qcResultsRow.TestType = bsTestsListDataGridView.SelectedRows(0).Cells("TestType").Value.ToString
                        qcResultsRow.TestID = Convert.ToInt32(bsTestsListDataGridView.SelectedRows(0).Cells("TestID").Value)
                        qcResultsRow.SampleType = bsTestsListDataGridView.SelectedRows(0).Cells("SampleType").Value.ToString
                        qcResultsRow.ControlID = Convert.ToInt32(bsCtrl2IDLabel.Text)
                        qcResultsRow.LotNumber = bsCtrl2LotNumLabel.Text
                        qcResultsRow.RerunNumber = 1
                        qcResultsRow.ResultValue = Convert.ToSingle(myTextBox2.Text)
                        qcResultsRow.ResultDateTime = myDateTime
                        qcResultsRow.SetResultCommentNull()
                        qcResultsRow.ManualResultFlag = False
                        qcResultsRow.SetManualResultValueNull()
                        qcResultsRow.CtrlsSendingGroup = myCtrlSendingGroup
                        myQCResultsDS.tqcResults.AddtqcResultsRow(qcResultsRow)
                    Else
                        If (numControls = 2 AndAlso myTextBox1.Text = String.Empty AndAlso myTextBox2.Text = String.Empty) Then allResultsOK = False
                    End If
                End If

                If (bsCtrl3Panel.Visible) Then
                    myTextBox3 = CType(TextBoxesCtrl3Array(i - 1), TextBox)
                    If (myTextBox3.Text <> String.Empty) Then
                        qcResultsRow = myQCResultsDS.tqcResults.NewtqcResultsRow
                        qcResultsRow.AnalyzerID = AnalyzerIDAttribute
                        qcResultsRow.TestType = bsTestsListDataGridView.SelectedRows(0).Cells("TestType").Value.ToString
                        qcResultsRow.TestID = Convert.ToInt32(bsTestsListDataGridView.SelectedRows(0).Cells("TestID").Value)
                        qcResultsRow.SampleType = bsTestsListDataGridView.SelectedRows(0).Cells("SampleType").Value.ToString
                        qcResultsRow.ControlID = Convert.ToInt32(bsCtrl3IDLabel.Text)
                        qcResultsRow.LotNumber = bsCtrl3LotNumLabel.Text
                        qcResultsRow.RerunNumber = 1
                        qcResultsRow.ResultValue = Convert.ToSingle(myTextBox3.Text)
                        qcResultsRow.ResultDateTime = myDateTime
                        qcResultsRow.SetResultCommentNull()
                        qcResultsRow.ManualResultFlag = False
                        qcResultsRow.SetManualResultValueNull()
                        qcResultsRow.CtrlsSendingGroup = myCtrlSendingGroup
                        myQCResultsDS.tqcResults.AddtqcResultsRow(qcResultsRow)
                    Else
                        If (numControls = 3 AndAlso myTextBox1.Text = String.Empty AndAlso _
                            myTextBox2.Text = String.Empty AndAlso myTextBox3.Text = String.Empty) Then allResultsOK = False
                    End If
                End If
            Next i

            If (allResultsOK) Then
                myGlobalDataTO = myResultsDelegate.ExportControlResultsNEW(Nothing, "", "", myQCResultsDS)
                If (Not myGlobalDataTO.HasError) Then
                    SetStatusForResultArea(True)
                Else
                    MessageBox.Show(myGlobalDataTO.ErrorMessage, Me.Name & ".SaveResults", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            Else
                MessageBox.Show("For each Serie, it is mandatory a result for at least one of the linked Controls", _
                                Me.Name & ".SaveResults", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".SaveResults", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ScreenLoad()
        Try
            PrepareButtons()
            InitializeDatesRanges()
            InitializeTestsListGrid()

            CreateResultsArrays()
            SetStatusForResultArea(True)

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myTestControlsDelegate As New TestControlsDelegate

            myGlobalDataTO = myTestControlsDelegate.GetAllNEW(Nothing)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                TestsListDS = DirectCast(myGlobalDataTO.SetDatos, TestControlsDS)

                If (TestsListDS.tparTestControls.Rows.Count > 0) Then
                    bsTestsListDataGridView.DataSource = TestsListDS.tparTestControls
                    bsTestsListDataGridView.Rows(0).Selected = False

                    bsTestsListDataGridView.Enabled = True
                    bsAddResultsButton.Enabled = False
                Else
                    bsTestsListDataGridView.Enabled = False
                    bsAddResultsButton.Enabled = False
                End If
            Else
                MessageBox.Show(myGlobalDataTO.ErrorMessage, Me.Name & ".ScreenLoad", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".ScreenLoad", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SetStatusForResultArea(ByVal pDisableArea As Boolean)
        Try
            Dim decimalsAllowed As Boolean = False
            If (bsTestsListDataGridView.SelectedRows.Count = 1) Then
                decimalsAllowed = (Convert.ToInt32(bsTestsListDataGridView.SelectedRows(0).Cells("DecimalsAllowed").Value) > 0)
            End If

            If (pDisableArea) Then
                bsNumResultsNumericUpDown.Enabled = False
                bsNumResultsNumericUpDown.BackColor = SystemColors.MenuBar

                bsDateFromDateTimePick.Enabled = False
                bsDateFromDateTimePick.BackColor = SystemColors.MenuBar

                bsDateToDateTimePick.Enabled = False
                bsDateToDateTimePick.BackColor = SystemColors.MenuBar

                bsGenerationModeGroupBox.Enabled = False
            Else
                bsNumResultsNumericUpDown.Enabled = True
                bsNumResultsNumericUpDown.BackColor = Color.White

                bsDateFromDateTimePick.Enabled = True
                bsDateFromDateTimePick.BackColor = Color.White

                'Date To is always disabled
                bsDateToDateTimePick.Enabled = False
                bsDateToDateTimePick.BackColor = SystemColors.MenuBar

                bsGenerationModeGroupBox.Enabled = True
                bsManualRadioButton.Checked = True
            End If

            'FIRST CONTROL
            Dim myTextBox As New TextBox
            For Each obj As Object In TextBoxesCtrl1Array
                myTextBox = CType(obj, TextBox)

                myTextBox.MaxLength = 11
                myTextBox.TextAlign = HorizontalAlignment.Right
                If (Convert.ToInt32(myTextBox.Tag) <= bsNumResultsNumericUpDown.Value) AndAlso (Not pDisableArea) Then
                    myTextBox.Enabled = True
                    myTextBox.BackColor = Color.White
                Else
                    myTextBox.Enabled = False
                    myTextBox.Text = ""
                    myTextBox.BackColor = SystemColors.MenuBar
                End If
            Next
            bsCtrl1Run1TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run2TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run3TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run4TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run5TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run6TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run7TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run8TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run9TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run10TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run11TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run12TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run13TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run14TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run15TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run16TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run17TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run18TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run19TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run20TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run21TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run22TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run23TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run24TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run25TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run26TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run27TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run28TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run29TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run30TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run31TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run32TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run33TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run34TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run35TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run36TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run37TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run38TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run39TextBox.DecimalsValues = decimalsAllowed
            bsCtrl1Run40TextBox.DecimalsValues = decimalsAllowed

            If (pDisableArea) Then
                bsCtrl1IDLabel.Text = String.Empty
                bsCtrl1InfoLabel.Text = String.Empty
                bsCtrl1ValuesLabel.Text = String.Empty
            End If

            'SECOND CONTROL
            If (bsCtrl2Panel.Visible) Then
                For Each obj As Object In TextBoxesCtrl2Array
                    myTextBox = CType(obj, TextBox)

                    myTextBox.MaxLength = 11
                    myTextBox.TextAlign = HorizontalAlignment.Right
                    If (Convert.ToInt32(myTextBox.Tag) <= bsNumResultsNumericUpDown.Value) AndAlso (Not pDisableArea) Then
                        myTextBox.Enabled = True
                        myTextBox.BackColor = Color.White
                    Else
                        myTextBox.Enabled = False
                        myTextBox.Text = ""
                        myTextBox.BackColor = SystemColors.MenuBar
                    End If
                Next
                bsCtrl2Run1TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run2TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run3TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run4TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run5TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run6TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run7TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run8TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run9TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run10TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run11TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run12TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run13TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run14TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run15TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run16TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run17TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run18TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run19TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run20TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run21TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run22TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run23TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run24TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run25TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run26TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run27TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run28TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run29TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run30TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run31TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run32TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run33TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run34TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run35TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run36TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run37TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run38TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run39TextBox.DecimalsValues = decimalsAllowed
                bsCtrl2Run40TextBox.DecimalsValues = decimalsAllowed

                If (pDisableArea) Then
                    bsCtrl2IDLabel.Text = String.Empty
                    bsCtrl2InfoLabel.Text = String.Empty
                    bsCtrl2ValuesLabel.Text = String.Empty
                End If
            End If

            'THIRD CONTROL
            If (bsCtrl3Panel.Visible) Then
                For Each obj As Object In TextBoxesCtrl3Array
                    myTextBox = CType(obj, TextBox)

                    myTextBox.MaxLength = 11
                    myTextBox.TextAlign = HorizontalAlignment.Right
                    If (Convert.ToInt32(myTextBox.Tag) <= bsNumResultsNumericUpDown.Value) AndAlso (Not pDisableArea) Then
                        myTextBox.Enabled = True
                        myTextBox.BackColor = Color.White
                    Else
                        myTextBox.Enabled = False
                        myTextBox.Text = ""
                        myTextBox.BackColor = SystemColors.MenuBar
                    End If
                Next
                bsCtrl3Run1TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run2TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run3TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run4TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run5TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run6TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run7TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run8TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run9TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run10TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run11TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run12TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run13TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run14TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run15TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run16TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run17TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run18TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run19TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run20TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run21TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run22TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run23TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run24TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run25TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run26TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run27TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run28TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run29TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run30TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run31TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run32TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run33TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run34TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run35TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run36TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run37TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run38TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run39TextBox.DecimalsValues = decimalsAllowed
                bsCtrl3Run40TextBox.DecimalsValues = decimalsAllowed

                If (pDisableArea) Then
                    bsCtrl3IDLabel.Text = String.Empty
                    bsCtrl3InfoLabel.Text = String.Empty
                    bsCtrl3ValuesLabel.Text = String.Empty
                End If
            End If

            bsAcceptButton.Enabled = (Not pDisableArea)
            bsCancelButton.Enabled = (Not pDisableArea)

            bsTestsListDataGridView.Enabled = pDisableArea
            bsAddResultsButton.Enabled = pDisableArea
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".SetStatusForResultArea", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Events"
    Private Sub QCSimulator_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                If (Not bsNumResultsNumericUpDown.Focused) Then
                    bsExitButton.PerformClick()
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message, Name & ".QCSimulator_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".QCSimulator_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message, Me)
        End Try
    End Sub

    Private Sub QCSimulator_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            ScreenLoad()
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".QCSimulator_Load", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bsTestsListDataGridView_CellMouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsTestsListDataGridView.CellMouseClick
        Try
            If (bsTestsListDataGridView.Enabled) Then
                bsAddResultsButton.Enabled = True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".bsTestsListDataGridView_CellMouseClick", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bsTestsListDataGridView_CellMouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles bsTestsListDataGridView.CellMouseDoubleClick
        Try
            If (bsTestsListDataGridView.Enabled) Then
                If (bsTestsListDataGridView.SelectedRows.Count = 1) Then
                    ChangesMade = False
                    GetControlsInfo(bsTestsListDataGridView.SelectedRows(0).Cells("TestType").Value.ToString(), _
                                    CInt(bsTestsListDataGridView.SelectedRows(0).Cells("TestID").Value), _
                                    bsTestsListDataGridView.SelectedRows(0).Cells("SampleType").Value.ToString())
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".bsTestsListDataGridView_CellMouseDoubleClick", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bsAddResultsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAddResultsButton.Click
        Try
            If (bsTestsListDataGridView.SelectedRows.Count = 1) Then
                ChangesMade = False
                GetControlsInfo(bsTestsListDataGridView.SelectedRows(0).Cells("TestType").Value.ToString(), _
                                CInt(bsTestsListDataGridView.SelectedRows(0).Cells("TestID").Value), _
                                bsTestsListDataGridView.SelectedRows(0).Cells("SampleType").Value.ToString())
                bsManualRadioButton.Checked = True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".bsAddResultsButton_Click", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bsResultTextBox_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles bsCtrl1Run1TextBox.KeyPress, bsCtrl1Run2TextBox.KeyPress, bsCtrl1Run3TextBox.KeyPress, bsCtrl1Run4TextBox.KeyPress, bsCtrl1Run5TextBox.KeyPress, _
                                                                                                                            bsCtrl1Run6TextBox.KeyPress, bsCtrl1Run7TextBox.KeyPress, bsCtrl1Run8TextBox.KeyPress, bsCtrl1Run9TextBox.KeyPress, bsCtrl1Run10TextBox.KeyPress, _
                                                                                                                            bsCtrl1Run11TextBox.KeyPress, bsCtrl1Run12TextBox.KeyPress, bsCtrl1Run13TextBox.KeyPress, bsCtrl1Run14TextBox.KeyPress, bsCtrl1Run15TextBox.KeyPress, _
                                                                                                                            bsCtrl1Run16TextBox.KeyPress, bsCtrl1Run17TextBox.KeyPress, bsCtrl1Run18TextBox.KeyPress, bsCtrl1Run19TextBox.KeyPress, bsCtrl1Run20TextBox.KeyPress, _
                                                                                                                            bsCtrl1Run21TextBox.KeyPress, bsCtrl1Run22TextBox.KeyPress, bsCtrl1Run23TextBox.KeyPress, bsCtrl1Run24TextBox.KeyPress, bsCtrl1Run25TextBox.KeyPress, _
                                                                                                                            bsCtrl1Run26TextBox.KeyPress, bsCtrl1Run27TextBox.KeyPress, bsCtrl1Run28TextBox.KeyPress, bsCtrl1Run29TextBox.KeyPress, bsCtrl1Run30TextBox.KeyPress, _
                                                                                                                            bsCtrl1Run31TextBox.KeyPress, bsCtrl1Run32TextBox.KeyPress, bsCtrl1Run33TextBox.KeyPress, bsCtrl1Run34TextBox.KeyPress, bsCtrl1Run35TextBox.KeyPress, _
                                                                                                                            bsCtrl1Run36TextBox.KeyPress, bsCtrl1Run37TextBox.KeyPress, bsCtrl1Run38TextBox.KeyPress, bsCtrl1Run39TextBox.KeyPress, bsCtrl1Run40TextBox.KeyPress, _
                                                                                                                            bsCtrl2Run1TextBox.KeyPress, bsCtrl2Run2TextBox.KeyPress, bsCtrl2Run3TextBox.KeyPress, bsCtrl2Run4TextBox.KeyPress, bsCtrl2Run5TextBox.KeyPress, _
                                                                                                                            bsCtrl2Run6TextBox.KeyPress, bsCtrl2Run7TextBox.KeyPress, bsCtrl2Run8TextBox.KeyPress, bsCtrl2Run9TextBox.KeyPress, bsCtrl2Run10TextBox.KeyPress, _
                                                                                                                            bsCtrl2Run11TextBox.KeyPress, bsCtrl2Run12TextBox.KeyPress, bsCtrl2Run13TextBox.KeyPress, bsCtrl2Run14TextBox.KeyPress, bsCtrl2Run15TextBox.KeyPress, _
                                                                                                                            bsCtrl2Run16TextBox.KeyPress, bsCtrl2Run17TextBox.KeyPress, bsCtrl2Run18TextBox.KeyPress, bsCtrl2Run19TextBox.KeyPress, bsCtrl2Run20TextBox.KeyPress, _
                                                                                                                            bsCtrl2Run21TextBox.KeyPress, bsCtrl2Run22TextBox.KeyPress, bsCtrl2Run23TextBox.KeyPress, bsCtrl2Run24TextBox.KeyPress, bsCtrl2Run25TextBox.KeyPress, _
                                                                                                                            bsCtrl2Run26TextBox.KeyPress, bsCtrl2Run27TextBox.KeyPress, bsCtrl2Run28TextBox.KeyPress, bsCtrl2Run29TextBox.KeyPress, bsCtrl2Run30TextBox.KeyPress, _
                                                                                                                            bsCtrl2Run31TextBox.KeyPress, bsCtrl2Run32TextBox.KeyPress, bsCtrl2Run33TextBox.KeyPress, bsCtrl2Run34TextBox.KeyPress, bsCtrl2Run35TextBox.KeyPress, _
                                                                                                                            bsCtrl2Run36TextBox.KeyPress, bsCtrl2Run37TextBox.KeyPress, bsCtrl2Run38TextBox.KeyPress, bsCtrl2Run39TextBox.KeyPress, bsCtrl2Run40TextBox.KeyPress, _
                                                                                                                            bsCtrl3Run1TextBox.KeyPress, bsCtrl3Run2TextBox.KeyPress, bsCtrl3Run3TextBox.KeyPress, bsCtrl3Run4TextBox.KeyPress, bsCtrl3Run5TextBox.KeyPress, _
                                                                                                                            bsCtrl3Run6TextBox.KeyPress, bsCtrl3Run7TextBox.KeyPress, bsCtrl3Run8TextBox.KeyPress, bsCtrl3Run9TextBox.KeyPress, bsCtrl3Run10TextBox.KeyPress, _
                                                                                                                            bsCtrl3Run11TextBox.KeyPress, bsCtrl3Run12TextBox.KeyPress, bsCtrl3Run13TextBox.KeyPress, bsCtrl3Run14TextBox.KeyPress, bsCtrl3Run15TextBox.KeyPress, _
                                                                                                                            bsCtrl3Run16TextBox.KeyPress, bsCtrl3Run17TextBox.KeyPress, bsCtrl3Run18TextBox.KeyPress, bsCtrl3Run19TextBox.KeyPress, bsCtrl3Run20TextBox.KeyPress, _
                                                                                                                            bsCtrl3Run21TextBox.KeyPress, bsCtrl3Run22TextBox.KeyPress, bsCtrl3Run23TextBox.KeyPress, bsCtrl3Run24TextBox.KeyPress, bsCtrl3Run25TextBox.KeyPress, _
                                                                                                                            bsCtrl3Run26TextBox.KeyPress, bsCtrl3Run27TextBox.KeyPress, bsCtrl3Run28TextBox.KeyPress, bsCtrl3Run29TextBox.KeyPress, bsCtrl3Run30TextBox.KeyPress, _
                                                                                                                            bsCtrl3Run31TextBox.KeyPress, bsCtrl3Run32TextBox.KeyPress, bsCtrl3Run33TextBox.KeyPress, bsCtrl3Run34TextBox.KeyPress, bsCtrl3Run35TextBox.KeyPress, _
                                                                                                                            bsCtrl3Run36TextBox.KeyPress, bsCtrl3Run37TextBox.KeyPress, bsCtrl3Run38TextBox.KeyPress, bsCtrl3Run39TextBox.KeyPress, bsCtrl3Run40TextBox.KeyPress

        Try
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
            If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",") OrElse e.KeyChar = CChar("") OrElse e.KeyChar = ChrW(Keys.Back)) Then
                If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                    e.KeyChar = CChar(myDecimalSeparator)
                End If

                If (CType(sender, TextBox).Text.Contains(".") Or CType(sender, TextBox).Text.Contains(",")) Then
                    e.Handled = True
                Else
                    e.Handled = False
                    ChangesMade = True
                End If
            Else
                If (Not IsNumeric(e.KeyChar)) Then
                    e.Handled = True
                Else
                    ChangesMade = True
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".bsResultTextBox_KeyPress", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bsDateFromDateTimePick_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsDateFromDateTimePick.ValueChanged
        bsDateToDateTimePick.Value = DateAdd(DateInterval.Day, bsNumResultsNumericUpDown.Value, bsDateFromDateTimePick.Value)
    End Sub

    Private Sub bsNumResultsNumericUpDown_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsNumResultsNumericUpDown.ValueChanged
        Try
            SetStatusForResultArea(False)
            bsDateToDateTimePick.Value = DateAdd(DateInterval.Day, bsNumResultsNumericUpDown.Value, bsDateFromDateTimePick.Value)
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".bsNumResultsNumericUpDown_ValueChanged", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            SaveResults()
            ChangesMade = False
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".bsAcceptButton_Click", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            If (Not ChangesMade OrElse _
                ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                SetStatusForResultArea(True)
                bsTestsListDataGridView.Enabled = True
                bsAddResultsButton.Enabled = (bsTestsListDataGridView.SelectedRows.Count = 1)
                ChangesMade = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".bsCancelButton_Click", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            If (Not ChangesMade OrElse ShowMessage(Me.Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                Me.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".bsExitButton_Click", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bsManualRadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles bsManualRadioButton.CheckedChanged
        Try
            If (bsManualRadioButton.Checked) Then
                Dim myTextBox As New TextBox
                For Each obj As Object In TextBoxesCtrl1Array
                    myTextBox = CType(obj, TextBox)

                    If (Convert.ToInt32(myTextBox.Tag) <= bsNumResultsNumericUpDown.Value) Then
                        myTextBox.Enabled = True
                        myTextBox.BackColor = Color.White
                    End If
                Next

                If (bsCtrl2Panel.Visible) Then
                    For Each obj As Object In TextBoxesCtrl2Array
                        myTextBox = CType(obj, TextBox)

                        If (Convert.ToInt32(myTextBox.Tag) <= bsNumResultsNumericUpDown.Value) Then
                            myTextBox.Enabled = True
                            myTextBox.BackColor = Color.White
                        End If
                    Next
                End If

                If (bsCtrl3Panel.Visible) Then
                    For Each obj As Object In TextBoxesCtrl3Array
                        myTextBox = CType(obj, TextBox)

                        If (Convert.ToInt32(myTextBox.Tag) <= bsNumResultsNumericUpDown.Value) Then
                            myTextBox.Enabled = True
                            myTextBox.BackColor = Color.White
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".bsManualRadioButton_CheckedChanged", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub bsRandomRadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles bsRandomRadioButton.CheckedChanged
        Try
            If (bsRandomRadioButton.Checked) Then
                'Get the number of decimals allowed for the selected Test
                Dim numOfTestdecimals As Integer = Convert.ToInt32(bsTestsListDataGridView.SelectedRows(0).Cells("DecimalsAllowed").Value)

                'Get random values for the first Control
                Dim myTextBox As New TextBox
                For Each obj As Object In TextBoxesCtrl1Array
                    myTextBox = CType(obj, TextBox)

                    myTextBox.MaxLength = 11
                    myTextBox.TextAlign = HorizontalAlignment.Right
                    If (Convert.ToInt32(myTextBox.Tag) <= bsNumResultsNumericUpDown.Value) Then
                        Randomize()
                        myTextBox.Text = ((upperLimitCtrl1 - lowerLimitCtrl1 + 1) * Rnd() + lowerLimitCtrl1).ToString("F" & numOfTestdecimals.ToString())

                        myTextBox.Enabled = False
                        myTextBox.BackColor = SystemColors.MenuBar
                    End If
                Next

                'Get random values for the second Control
                If (bsCtrl2Panel.Visible) Then
                    For Each obj As Object In TextBoxesCtrl2Array
                        myTextBox = CType(obj, TextBox)

                        myTextBox.MaxLength = 11
                        myTextBox.TextAlign = HorizontalAlignment.Right
                        If (Convert.ToInt32(myTextBox.Tag) <= bsNumResultsNumericUpDown.Value) Then
                            Randomize()
                            myTextBox.Text = ((upperLimitCtrl2 - lowerLimitCtrl2 + 1) * Rnd() + lowerLimitCtrl2).ToString("F" & numOfTestdecimals.ToString())

                            myTextBox.Enabled = False
                            myTextBox.BackColor = SystemColors.MenuBar
                        End If
                    Next
                End If

                'Get random values for the third Control
                If (bsCtrl3Panel.Visible) Then
                    For Each obj As Object In TextBoxesCtrl3Array
                        myTextBox = CType(obj, TextBox)

                        myTextBox.MaxLength = 11
                        myTextBox.TextAlign = HorizontalAlignment.Right
                        If (Convert.ToInt32(myTextBox.Tag) <= bsNumResultsNumericUpDown.Value) Then
                            Randomize()
                            myTextBox.Text = ((upperLimitCtrl3 - lowerLimitCtrl3 + 1) * Rnd() + lowerLimitCtrl3).ToString("F" & numOfTestdecimals.ToString())

                            myTextBox.Enabled = False
                            myTextBox.BackColor = SystemColors.MenuBar
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Name & ".bsRandomRadioButton_CheckedChanged", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region
End Class
