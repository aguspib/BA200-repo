Option Strict On
Option Explicit On
Option Infer On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Public Class IQCAddManualResultsAux

#Region "Declarations"
    Private myNewLocation As Point              'To avoid the screen movement
    Private isScreenLoading As Boolean          'To avoid execution of some events when the screen is loading
    Private LocalChange As Boolean = False      'To verify if there are changes pending to save when buttons SEARCH and/or CANCEL are clicked
    Private QCResultsToAddDS As New QCResultsDS 'Used as DataSource of the DataGridView allowing the adding of new QC Results for the selected Serie 
#End Region

#Region "Attributes"
    Private LanguageIDAttribute As String
    Private MaxRunNumberAttribute As Integer
    Private LocalDecimalAllowAttribute As Integer
    Private AllQCResultsDSAttribute As New QCResultsDS
    Private LocalQCResultDSAttribute As New QCResultsDS
    Private MinDateAttribute As New DateTime
    Private MaxDateAttribute As New DateTime
#End Region

#Region "Properties"
    'Current Application Language
    Public WriteOnly Property LanguageID() As String
        Set(ByVal value As String)
            LanguageIDAttribute = value
        End Set
    End Property

    'Maximum number of Serie that can be selected in the corresponding Numeric Up/Down
    'It is the number of the NEXT Serie of Results to add (maximum RunNumber for the 
    'TestType/TestID/SampleType plus one)
    Public WriteOnly Property MaxRunNumber() As Integer
        Set(ByVal value As Integer)
            MaxRunNumberAttribute = value
        End Set
    End Property

    'Number of decimals allowed for QC Results of the TestType/TestID/SampleType
    Public WriteOnly Property DecimalAllowed() As Integer
        Set(ByVal value As Integer)
            LocalDecimalAllowAttribute = value
        End Set
    End Property

    'Typed Dataset QCResultsDS containing all non cumulated QC Results for the TestType/TestID/SampleType
    '(for all its linked Controls/Lots)
    Public WriteOnly Property AllQCResultsDS() As QCResultsDS
        Set(ByVal value As QCResultsDS)
            AllQCResultsDSAttribute = value
        End Set
    End Property

    'Typed Dataset QCResultsDS containing a row for each Control/Lot linked to the TestType/TestID/SampleType
    'It is used to load the local QCResultsToAddDS when a different Number of Serie is selected
    Public WriteOnly Property NewQCResultDS() As QCResultsDS
        Set(ByVal value As QCResultsDS)
            LocalQCResultDSAttribute = value
        End Set
    End Property

    'Return the minimum date of the group of QC Results added
    Public ReadOnly Property MinDate() As DateTime
        Get
            Return MinDateAttribute
        End Get
    End Property

    'Return the maximum date of the group of QC Results added
    Public ReadOnly Property MaxDate() As DateTime
        Get
            Return MaxDateAttribute
        End Get
    End Property
#End Region

#Region "Methods"
    ''' <summary>
    ''' Verify the characters entered in cell ResultValue are allowed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 10/06/2011
    ''' </remarks>
    Private Sub CheckCell(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Try
            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

            'Verify if the key pressed if one of the special characters allowed
            If (e.KeyChar = myDecimalSeparator OrElse e.KeyChar = CChar("") _
                                               OrElse e.KeyChar = CChar(".") _
                                               OrElse e.KeyChar = CChar(",") _
                                               OrElse e.KeyChar = ChrW(Keys.Back)) Then

                If (e.KeyChar = CChar(".") OrElse e.KeyChar = CChar(",")) Then
                    e.KeyChar = CChar(myDecimalSeparator)
                End If

                'If there was already a decimal character, then the pressed key is ignored
                e.Handled = (CType(sender, TextBox).Text.Contains(",") OrElse CType(sender, TextBox).Text.Contains("."))
            Else
                'If the key pressed is not a number nor one of the special allowed characters, it is ignored
                If (Not IsNumeric(e.KeyChar)) Then e.Handled = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CheckCell ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CheckCell ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the text of the screen labels in the current application language
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 07/06/2011
    ''' Modified by: SA 08/06/2012 - Get multilanguage text for new label for Run number
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            bsAddManualResultLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ADD_MANUAL_RESULTS", LanguageIDAttribute)
            bsNumSerieLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Serie", LanguageIDAttribute) & ":"
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Screen initialization
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 07/06/2011
    ''' Modified by: SA 25/04/2012 - Open the screen centered regarding the Main MDI form   
    '''              SA 08/06/2012 - Initialize Numeric Up/Down containing the available number of Series and 
    '''                              call new function PrepareDataGridForSelectedSerie to load and configure the grid
    '''                              for the next Serie to add. Do not set here the DataSource property of the DataGridView 
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            PrepareButtons()
            GetScreenLabels()

            'Initialize the Numeric UpDown that allow select the Run Number
            bsNumSerieNumericUpDown.Minimum = 1
            bsNumSerieNumericUpDown.Maximum = MaxRunNumberAttribute
            bsNumSerieNumericUpDown.Increment = 1
            bsNumSerieNumericUpDown.DecimalPlaces = 0

            'Initialize and load the DataGridView
            PrepareNewResultsGridView()

            'The MaxRunNumber is proposed as the next Serie to add...
            bsNumSerieNumericUpDown.Text = bsNumSerieNumericUpDown.Maximum.ToString
            PrepareDataGridForSelectedSerie(Convert.ToInt32(bsNumSerieNumericUpDown.Value))

            'Initialize Max/Min Date properties
            MinDateAttribute = DateTime.MinValue
            MaxDateAttribute = DateTime.MaxValue
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializeScreen", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializeScreen", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Get the icon and tooltip text (in the current application language) of all screen buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/04/2010
    ''' Modified by: SA 08/06/2012 - Get icon and tooltip of Search button
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'SAVE Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsAcceptButton.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsAcceptButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Save&Close", LanguageIDAttribute))
            End If

            'EXIT Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
                myToolTipsControl.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Cancel", LanguageIDAttribute))
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & " PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' For the selected RunNumber, verify if there are QC Results for all the linked Controls and load and configure 
    ''' the grid in the following way: 
    ''' ** In rows for Controls having a QC Result for the Serie, cells Date, Time and Value are read-only
    ''' ** In rows for Controls without a QC Result for the Serie, cells Time and Value are editable. Cell Date is 
    '''    editable only when the selected Serie is a new one (there are not QC Results for none of the linked Controls), and 
    '''    in this case, it is initialized with the current date; otherwise, the cell shows the same date as the Controls with
    '''    a QC Result for the Serie and the cell is disabled
    ''' </summary>
    ''' <param name="pRunNumber">Number of the Serie selected for adding manual QC Results</param>
    ''' <remarks>
    ''' Created by:  SA 08/06/2012
    ''' </remarks>
    Private Sub PrepareDataGridForSelectedSerie(ByVal pRunNumber As Integer)
        Try
            Dim allNew As Boolean = True
            Dim lstQCResults As List(Of QCResultsDS.tqcResultsRow)
            'TR 21/06/2012 -Clear the rows on dataset before initializing
            QCResultsToAddDS.tqcResults.Clear()
            'Initialize the DS that will be used as DataSource of the DataGridView
            For Each row As QCResultsDS.tqcResultsRow In LocalQCResultDSAttribute.tqcResults
                QCResultsToAddDS.tqcResults.ImportRow(row)
            Next

            For Each row As QCResultsDS.tqcResultsRow In QCResultsToAddDS.tqcResults
                'Verify if there are Results for the Control/Lot and the selected Serie
                lstQCResults = (From a As QCResultsDS.tqcResultsRow In AllQCResultsDSAttribute.tqcResults _
                               Where a.QCControlLotID = row.QCControlLotID _
                             AndAlso a.RunNumber = pRunNumber _
                              Select a).ToList

                row.BeginEdit()
                row.RunNumber = pRunNumber
                row.ValidationStatus = "NEW"
                row.SetManualResultValueNull()

                If (lstQCResults.Count > 0) Then
                    row.ResultDateTime = lstQCResults.First.ResultDateTime.Date
                    row.ResultTime = lstQCResults.First.ResultDateTime
                    row.ManualResultValue = lstQCResults.First.VisibleResultValue
                    row.ValidationStatus = lstQCResults.First.ValidationStatus

                    allNew = False
                End If
                row.EndEdit()
            Next row

            'Get the more recent DateTime between all previous QC Results in the selected Serie
            If (Not allNew) Then
                Dim myDate As DateTime = (From b In QCResultsToAddDS.tqcResults _
                         Where b.ValidationStatus <> "NEW" _
                        Select b.ResultTime).Max

                lstQCResults = (From c As QCResultsDS.tqcResultsRow In QCResultsToAddDS.tqcResults _
                               Where c.ValidationStatus = "NEW" _
                              Select c).ToList

                For Each row As QCResultsDS.tqcResultsRow In lstQCResults
                    row.BeginEdit()
                    row.ResultDateTime = myDate.Date
                    row.ResultTime = myDate
                    row.EndEdit()
                Next
            End If

            bsNewResultGridView.DataSource = QCResultsToAddDS.tqcResults
            For Each row As DataGridViewRow In bsNewResultGridView.Rows
                row.Cells("ControlName").ReadOnly = True
                row.Cells("ControlName").Style.BackColor = SystemColors.MenuBar

                row.Cells("LotNumber").ReadOnly = True
                row.Cells("LotNumber").Style.BackColor = SystemColors.MenuBar

                If (Not row.Cells("ResultValue").Value Is DBNull.Value) Then
                    row.Cells("Date").ReadOnly = True
                    row.Cells("Date").Style.BackColor = SystemColors.MenuBar

                    row.Cells("TimeCol").ReadOnly = True
                    row.Cells("TimeCol").Style.BackColor = SystemColors.MenuBar

                    row.Cells("ResultValue").ReadOnly = True
                    row.Cells("ResultValue").Style.BackColor = SystemColors.MenuBar
                Else
                    If (allNew) Then
                        row.Cells("Date").ReadOnly = False
                        row.Cells("Date").Style.BackColor = Color.White
                    Else
                        row.Cells("Date").ReadOnly = True
                        row.Cells("Date").Style.BackColor = SystemColors.MenuBar
                    End If

                    row.Cells("TimeCol").ReadOnly = False
                    row.Cells("TimeCol").Style.BackColor = Color.White

                    row.Cells("ResultValue").ReadOnly = False
                    row.Cells("ResultValue").Style.BackColor = Color.White
                End If
            Next
            bsNewResultGridView.ClearSelection()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareDataGridForSelectedSerie", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareDataGridForSelectedSerie", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Initialize the DataGridView
    ''' </summary>
    ''' <remarks>
    ''' Created by:  TR 30/04/2010
    ''' </remarks>
    Private Sub PrepareNewResultsGridView()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsNewResultGridView.Columns.Clear()

            bsNewResultGridView.AutoSize = False
            bsNewResultGridView.MultiSelect = False
            bsNewResultGridView.AllowUserToAddRows = False
            bsNewResultGridView.AutoGenerateColumns = False
            bsNewResultGridView.EditMode = DataGridViewEditMode.EditOnEnter

            'Control Name Column
            bsNewResultGridView.Columns.Add("ControlName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Control_Name", LanguageIDAttribute))
            bsNewResultGridView.Columns("ControlName").Width = 130
            bsNewResultGridView.Columns("ControlName").DataPropertyName = "ControlName"
            bsNewResultGridView.Columns("ControlName").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsNewResultGridView.Columns("ControlName").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsNewResultGridView.Columns("ControlName").ReadOnly = True

            'Lot Number Column
            bsNewResultGridView.Columns.Add("LotNumber", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LotNumber", LanguageIDAttribute))
            bsNewResultGridView.Columns("LotNumber").Width = 120
            bsNewResultGridView.Columns("LotNumber").DataPropertyName = "LotNumber"
            bsNewResultGridView.Columns("LotNumber").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsNewResultGridView.Columns("LotNumber").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            bsNewResultGridView.Columns("LotNumber").ReadOnly = True

            'Result Date Column
            Dim myDateCombo As New Biosystems.Ax00.Controls.UserControls.CalendarColumn
            myDateCombo.Name = "Date"

            bsNewResultGridView.Columns.Add(myDateCombo)
            bsNewResultGridView.Columns("Date").Width = 110
            bsNewResultGridView.Columns("Date").SortMode = DataGridViewColumnSortMode.NotSortable
            bsNewResultGridView.Columns("Date").DataPropertyName = "ResultDateTime"
            bsNewResultGridView.Columns("Date").HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", LanguageIDAttribute)
            bsNewResultGridView.Columns("Date").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsNewResultGridView.Columns("Date").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsNewResultGridView.Columns("Date").ReadOnly = False

            'Result Time Column
            Dim myTimeCol As New Biosystems.Ax00.Controls.UserControls.CalendarTimeColumn
            myTimeCol.Name = "TimeCol"

            bsNewResultGridView.Columns.Add(myTimeCol)
            bsNewResultGridView.Columns("TimeCol").Width = 100
            bsNewResultGridView.Columns("TimeCol").DataPropertyName = "ResultTime"
            bsNewResultGridView.Columns("TimeCol").SortMode = DataGridViewColumnSortMode.NotSortable
            bsNewResultGridView.Columns("TimeCol").HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Hour", LanguageIDAttribute)
            bsNewResultGridView.Columns("TimeCol").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsNewResultGridView.Columns("TimeCol").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            bsNewResultGridView.Columns("TimeCol").ReadOnly = False

            'Result Value Column
            Dim ResultValue As New DataGridViewTextBoxColumn
            ResultValue.Name = "ResultValue"
            ResultValue.HeaderText = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Result", LanguageIDAttribute)
            ResultValue.MaxInputLength = 10

            bsNewResultGridView.Columns.Add(ResultValue)
            bsNewResultGridView.Columns("ResultValue").Width = 70
            bsNewResultGridView.Columns("ResultValue").DataPropertyName = "ManualResultValue"
            bsNewResultGridView.Columns("ResultValue").SortMode = DataGridViewColumnSortMode.NotSortable
            bsNewResultGridView.Columns("ResultValue").HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight
            bsNewResultGridView.Columns("ResultValue").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            bsNewResultGridView.Columns("ResultValue").ReadOnly = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareNewResultsGridView ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareNewResultsGridView ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Save the added QC Results
    ''' </summary>
    ''' <returns>True if the new QC Results were saved successfully; otherwise, it returns False</returns>
    ''' <remarks>
    ''' Created by:  TR 10/06/2011
    ''' Modified by: SA 08/06/2012 - QC Results to add are those having ValidationStatus=NEW and ManualResultValue informed
    ''' </remarks>
    Private Function SaveChanges() As Boolean
        Dim myResult As Boolean = False

        Try
            'For each Control/Lot in the grid, if a new QC Result has been added for it, build the DateTime field and inform the rest of 
            'fields needed to save the new QC Result
            Dim myQCResultDS As New QCResultsDS
            For Each qcResultRow As QCResultsDS.tqcResultsRow In QCResultsToAddDS.tqcResults.Rows
                If (Not qcResultRow.IsManualResultValueNull And qcResultRow.ValidationStatus = "NEW") Then
                    'Build the DateTime
                    qcResultRow.ResultDateTime = qcResultRow.ResultDateTime.AddHours(qcResultRow.ResultTime.Hour).AddMinutes(qcResultRow.ResultTime.Minute)

                    'Fill the rest of fields
                    qcResultRow.ManualResultFlag = True
                    qcResultRow.ValidationStatus = "OK"
                    qcResultRow.Excluded = False
                    qcResultRow.ClosedResult = False
                    qcResultRow.ResultValue = 0
                    qcResultRow.TS_DateTime = DateTime.Now

                    myQCResultDS.tqcResults.ImportRow(qcResultRow)
                End If
            Next

            'Select the MinDate and MaxDate of the informed QC Results and inform the properties that have to be returned
            'to the target screen of Individual QC Results by Test/Sample Type to change the dates filters when needed
            If (myQCResultDS.tqcResults.Where(Function(a) Not a.IsResultValueNull).Count > 0) Then
                MinDateAttribute = myQCResultDS.tqcResults.ToList().Select(Function(a) a.ResultDateTime).Min
                MaxDateAttribute = myQCResultDS.tqcResults.ToList().Select(Function(a) a.ResultDateTime).Max
            End If

            'Save the new QC Results
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myQCResultDelegate As New QCResultsDelegate

            'myGlobalDataTO = myQCResultDelegate.Create(Nothing, myQCResultDS)
            myGlobalDataTO = myQCResultDelegate.CreateNEW(Nothing, myQCResultDS)
            If (Not myGlobalDataTO.HasError) Then
                myResult = True
            Else
                'Error saving the new QC Results; shown it
                ShowMessage(Me.Name & ".SaveChanges ", myGlobalDataTO.ErrorCode, myGlobalDataTO.ErrorMessage, Me)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SaveChanges ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SaveChanges ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
        Return myResult
    End Function

    Private Sub ReleaseElements()

        Try
            '--- Detach variable defined using WithEvents ---
            bsAddManualResultsGroupBox = Nothing
            bsAddManualResultLabel = Nothing
            bsNewResultGridView = Nothing
            bsExitButton = Nothing
            bsAcceptButton = Nothing
            myToolTipsControl = Nothing
            bsNumSerieNumericUpDown = Nothing
            bsNumSerieLabel = Nothing
            '-----------------------------------------------
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ReleaseElements ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ReleaseElements ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub

#End Region

#Region "Events"
    '*************************
    '* EVENTS FOR THE SCREEN *
    '*************************
    Private Sub IAddManualQCResultsAux_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                bsExitButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IAddManualQCResultsAux_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IAddManualQCResultsAux_KeyDown ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub IAddManualQCResultsAux_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            isScreenLoading = True

            InitializeScreen()

            Dim mySize As Size = IAx00MainMDI.Size
            Dim myLocation As Point = IAx00MainMDI.Location

            If (Not Me.MdiParent Is Nothing) Then
                mySize = Me.Parent.Size
                myLocation = Me.Parent.Location
            End If

            myNewLocation = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            Me.Location = myNewLocation

            isScreenLoading = False
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".IAddManualQCResultsAux_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".IAddManualQCResultsAux_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        Try
            If (m.Msg = WM_WINDOWPOSCHANGING) Then
                Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                Dim mySize As Size = IAx00MainMDI.Size
                Dim myLocation As Point = IAx00MainMDI.Location
                If (Not Me.MdiParent Is Nothing) Then
                    mySize = Me.Parent.Size
                    myLocation = Me.Parent.Location
                End If

                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

            MyBase.WndProc(m)
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "WndProc " & Me.Name, EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & "WndProc", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*********************************
    '* EVENTS FOR THE DATA GRID VIEW *
    '*********************************
    Private Sub bsNewResultGridView_CellEndEdit(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsNewResultGridView.CellEndEdit
        Try
            If (Not bsNewResultGridView.Rows(e.RowIndex).Cells("ResultValue").Value Is DBNull.Value) Then
                LocalChange = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNewResultGridView_CellEndEdit ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNewResultGridView_CellEndEdit ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsNewResultGridView_CellFormatting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellFormattingEventArgs) Handles bsNewResultGridView.CellFormatting
        Try
            If (bsNewResultGridView.Columns(e.ColumnIndex).Name = "ResultValue" AndAlso Not e.Value Is DBNull.Value) Then
                e.Value = DirectCast(e.Value, Single).ToString("F" & LocalDecimalAllowAttribute.ToString())
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNewResultGridView_CellFormatting ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNewResultGridView_CellFormatting ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsNewResultGridView_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles bsNewResultGridView.CellValueChanged
        Try
            If (e.RowIndex < 0) Then Return
            If (bsNewResultGridView.Columns(e.ColumnIndex).Name = "Date" AndAlso Not bsNewResultGridView.Rows(e.RowIndex).Cells("Date").Value Is DBNull.Value) Then
                For Each row As QCResultsDS.tqcResultsRow In QCResultsToAddDS.tqcResults
                    If (row.ValidationStatus = "NEW") Then
                        row.ResultDateTime = CDate(bsNewResultGridView.Rows(e.RowIndex).Cells("Date").Value)
                    End If
                Next
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNewResultGridView_CellValueChanged ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNewResultGridView_CellValueChanged ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsNewResultGridView_CurrentCellDirtyStateChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNewResultGridView.CurrentCellDirtyStateChanged
        Try
            If (TypeOf bsNewResultGridView.CurrentCell Is DataGridViewComboBoxCell) Then
                bsNewResultGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsNewResultGridView_CurrentCellDirtyStateChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsNewResultGridView_CurrentCellDirtyStateChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsNewResultGridView_EditingControlShowing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles bsNewResultGridView.EditingControlShowing
        Try
            If (e.Control.GetType().Name = "DataGridViewTextBoxEditingControl") Then
                DirectCast(e.Control, DataGridViewTextBoxEditingControl).ShortcutsEnabled = False
            End If

            If (Me.bsNewResultGridView.CurrentRow.Index >= 0 AndAlso bsNewResultGridView.Columns(bsNewResultGridView.CurrentCell.ColumnIndex).Name = "ResultValue") Then
                AddHandler e.Control.KeyPress, AddressOf CheckCell
            Else
                RemoveHandler e.Control.KeyPress, AddressOf CheckCell
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsNewResultGridView_EditingControlShowing", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsNewResultGridView_EditingControlShowing", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '****************
    '* OTHER EVENTS *
    '****************
    Private Sub bsNumSerieNumericUpDown_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsNumSerieNumericUpDown.LostFocus
        Try
            If (bsNumSerieNumericUpDown.Text.Length = 0) Then bsNumSerieNumericUpDown.Text = bsNumSerieNumericUpDown.Minimum.ToString
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNumSerieNumericUpDown_LostFocus", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNumSerieNumericUpDown_LostFocus", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub bsNumSerieNumericUpDown_TextChange(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsNumSerieNumericUpDown.TextChanged
        Try
            If (isScreenLoading) Then Return

            Dim myNewValue As String = bsNumSerieNumericUpDown.Text
            Dim myMinValue As Single = bsNumSerieNumericUpDown.Minimum
            Dim myMaxValue As Single = bsNumSerieNumericUpDown.Maximum

            If (myNewValue.Length > 0 AndAlso myNewValue.Length > myMaxValue.ToString.Length) Then
                If (CSng(myNewValue) > CSng(myMaxValue)) Then
                    bsNumSerieNumericUpDown.Text = myMaxValue.ToString
                ElseIf (CSng(myNewValue) < myMinValue) Then
                    If (IsNumeric(Microsoft.VisualBasic.Left(myNewValue, 1))) Then
                        bsNumSerieNumericUpDown.Text = myMinValue.ToString
                    End If
                End If
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsNumSerieNumericUpDown_TextChange", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsNumSerieNumericUpDown_TextChange", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub bsNumSerieNumericUpDown_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles bsNumSerieNumericUpDown.ValueChanged
        Try
            If (isScreenLoading) Then Return


            If (LocalChange) Then
                If bsNumSerieNumericUpDown.Text <> bsNumSerieNumericUpDown.Value.ToString() Then
                    If (ShowMessage(Name & ".bsNumSerieNumericUpDown_ValueChanged", GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                        PrepareDataGridForSelectedSerie(Convert.ToInt32(bsNumSerieNumericUpDown.Value))
                        LocalChange = False
                    Else
                        'Set the previous value.
                        bsNumSerieNumericUpDown.Value = CDec(bsNumSerieNumericUpDown.Text)
                    End If
                End If

            Else
                QCResultsToAddDS = New QCResultsDS
                PrepareDataGridForSelectedSerie(Convert.ToInt32(bsNumSerieNumericUpDown.Value))
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsNumSerieNumericUpDown_ValueChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsNumSerieNumericUpDown_ValueChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    '*****************************
    '* EVENTS FOR SCREEN BUTTONS *
    '*****************************
    Private Sub bsAcceptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAcceptButton.Click
        Try
            If (LocalChange) Then
                If (SaveChanges()) Then Me.Close()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsAcceptButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsAcceptButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            If (LocalChange) Then
                If (ShowMessage(Name & ".ExitButton_Click", GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString, , Me) = Windows.Forms.DialogResult.Yes) Then
                    Me.Close()
                End If
            Else
                Me.Close()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Name & ".bsExitButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

    
   
End Class
