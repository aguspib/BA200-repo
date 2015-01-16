Option Explicit On
Option Strict On

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.PresentationCOM.XRManager

Public Class IResultsSummaryTable

#Region "Declarations"

    Private TestNames As New List(Of OrderTestTO)
    Private Samples As New List(Of Sample)
    Private Const TestNameFormat As String = "{0} ({1})"
    Private LanguageID As String

#End Region

#Region "Fields"

    Private AverageResultsDSField As ResultsDS = Nothing
    Private ExecutionsResultsDSField As ExecutionsDS = Nothing
    Private WorkSessionIDField As String = String.Empty
    Private AnalyzerIDField As String = String.Empty

#End Region

#Region "Properties"

    Public Property AverageResultsDS() As ResultsDS
        Get
            Return AverageResultsDSField
        End Get
        Set(ByVal value As ResultsDS)
            AverageResultsDSField = value
        End Set
    End Property

    Public Property ExecutionsResultsDS() As ExecutionsDS
        Get
            Return ExecutionsResultsDSField
        End Get
        Set(ByVal value As ExecutionsDS)
            ExecutionsResultsDSField = value
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

    Public Property ActiveAnalyzer() As String
        Get
            Return AnalyzerIDField
        End Get
        Set(ByVal value As String)
            AnalyzerIDField = value
        End Set
    End Property

#End Region

#Region "Events"

    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010
    ''' </remarks>
    Private Sub SummaryResultForm_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                bsExitButton.PerformClick()
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SummaryResultForm_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SummaryResultForm_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub SummaryResultForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            InitializeScreen()

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Table Resume LOAD (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResultsSummaryTable.SummaryResultForm_Load", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ResultForm_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ResultForm_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click

        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            XRManager.ShowSummaryResultsReport(ActiveAnalyzer, ActiveWorkSession, bsVerticalRadioButton.Checked)

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("Patients Final Report: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResults.PrintReportButton_Click", EventLogEntryType.Information, False)
            StartTime = Now
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrintReportButton ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try

    End Sub

    Private Sub bsExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsExitButton.Click
        Try
            Me.Close()
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsExitButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsExitButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Initializes all the controls
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 23/09/2010
    ''' Modified by: PG 13/10/2010 - Get the current Language
    '''              RH 22/10/2010 - Remove the currentLanguage local variable/parameter.
    '''                              Initialize the LanguageID new class property.
    ''' </remarks>
    Private Sub InitializeScreen()
        Try
            'DL 28/07/2011
            Dim myLocation As Point = Me.Owner.Location
            Dim mySize As Size = Me.Owner.Size

            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2))
            'END DL 28/07/2011

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            LanguageID = GlobalBase.GetSessionInfo().ApplicationLanguage

            GetScreenLabels()
            PrepareButtons()
            InitializePatientListDataGrid()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " InitializeScreen ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Not allow move form and mantain the center location in center parent
    ''' </summary>
    ''' <remarks>
    ''' Created by: DL 27/07/2011
    ''' </remarks>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_WINDOWPOSCHANGING Then
            Dim pos As WINDOWPOS = DirectCast(Runtime.InteropServices.Marshal.PtrToStructure(m.LParam, _
                                                                                             GetType(WINDOWPOS)),  _
                                                                                             WINDOWPOS)
            Dim myLocation As Point = Me.Owner.Location
            Dim mySize As Size = Me.Owner.Size

            pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
            pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2)
            Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
        End If

        MyBase.WndProc(m)

    End Sub

    ''' <summary>
    ''' Loads the images for all graphical buttons
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 23/09/2010
    ''' Modified by: DL 03/11/2010 - Load Icon in Image Property instead of in BackgroundImage Property
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'PRINT Button
            auxIconName = GetIconName("PRINT")
            If (auxIconName <> "") Then
                bsPrintButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CANCEL Button
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsExitButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        End Try
    End Sub

    ''' <summary>
    ''' Creates the PatientListDataGridView structure and content
    ''' </summary>
    ''' <remarks>
    ''' Created by:  RH 23/09/2010
    ''' Modified by: PG 13/10/2010 - Add the LanguageID parameter
    '''              RH 22/10/2010 - Remove the LanguageID parameter. Now it is a class property.
    '''              RH 28/01/2011 - Take Off System Tests into account
    ''' </remarks>
    Private Sub InitializePatientListDataGrid()
        Try
            If AverageResultsDS Is Nothing OrElse ExecutionsResultsDS Is Nothing Then Return

            Dim TestsList As List(Of ResultsDS.vwksResultsRow)
            Dim hasConcentrationError As Boolean
            Dim concentration As String = String.Empty
            Dim columnName As String


            'Get Tests and Patients Names
            TestNames = XRManager.GetSummaryResultsReportHeaderColumns(AverageResultsDS)
            Samples = XRManager.GetSummaryResultsReportPatientList(AverageResultsDS)

            bsPatientListDataGridView.Columns.Clear()
            bsPatientListDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader

            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Patient Id Column
            bsPatientListDataGridView.Columns.Add("PatientId", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", LanguageID))
            bsPatientListDataGridView.Columns("PatientId").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            bsPatientListDataGridView.Columns("PatientId").Frozen = True
            'Patient Barcode Column
            bsPatientListDataGridView.Columns.Add("Barcode", myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BARCODE", LanguageID))
            bsPatientListDataGridView.Columns("Barcode").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            bsPatientListDataGridView.Columns("Barcode").Frozen = True
            'Patient FirstName Column
            bsPatientListDataGridView.Columns.Add("FirstName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirstName", LanguageID))
            bsPatientListDataGridView.Columns("FirstName").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            bsPatientListDataGridView.Columns("FirstName").Frozen = True
            'Patient LastName Column
            bsPatientListDataGridView.Columns.Add("LastName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LastName", LanguageID))
            bsPatientListDataGridView.Columns("LastName").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            bsPatientListDataGridView.Columns("LastName").Frozen = True

            'Test Name Columns
            For i As Integer = 0 To TestNames.Count - 1
                columnName = String.Format("{0} ({1})", TestNames.ElementAt(i).TestName, TestNames.ElementAt(i).SampleType.Name)
                bsPatientListDataGridView.Columns.Add(columnName, columnName)
                bsPatientListDataGridView.Columns(columnName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            Next

            'Fill the Grid with data
            Dim sample As Sample
            For i As Integer = 0 To Samples.Count - 1
                sample = Samples.ElementAt(i)
                bsPatientListDataGridView.Rows.Add()
                bsPatientListDataGridView("PatientId", i).Value = sample.patientId
                bsPatientListDataGridView("Barcode", i).Value = sample.barcode
                bsPatientListDataGridView("FirstName", i).Value = sample.patient.firstName
                bsPatientListDataGridView("LastName", i).Value = sample.patient.lastName

                For j = 0 To TestNames.Count - 1
                    Dim auxJ = j
                    TestsList = (From row In AverageResultsDS.vwksResults _
                                     Where String.Compare(row.PatientID, sample.patientId, False) = 0 _
                                     AndAlso row.SampleType = sample.sampleType _
                                     AndAlso row.TestID = TestNames.ElementAt(auxJ).TestId _
                                     AndAlso row.SampleType = TestNames.ElementAt(auxJ).SampleType.Name _
                                     AndAlso row.TestType = TestNames.ElementAt(auxJ).TestType _
                                     AndAlso row.AcceptedResultFlag _
                                     Select row).ToList()

                    If TestsList.Count > 0 Then

                        If Not TestsList.First.IsCONC_ValueNull Then
                            hasConcentrationError = False

                            If Not TestsList.First.IsCONC_ErrorNull Then
                                hasConcentrationError = Not String.IsNullOrEmpty(TestsList.First.CONC_Error)
                            End If

                            If Not hasConcentrationError Then
                                concentration = TestsList.First.CONC_Value.ToStringWithDecimals(TestsList.First.DecimalsAllowed)
                                concentration = String.Format("{0} {1}", concentration, TestsList.First.MeasureUnit)
                            Else
                                concentration = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                            End If
                        ElseIf Not TestsList.First.IsManualResultTextNull Then 'Off System Test
                            concentration = TestsList.First.ManualResultText
                            concentration = String.Format("{0} {1}", concentration, TestsList.First.MeasureUnit)
                        Else
                            concentration = "-"
                        End If
                    Else
                        concentration = "-"
                    End If

                    columnName = String.Format("{0} ({1})", TestNames.ElementAt(auxJ).TestName, TestNames.ElementAt(auxJ).SampleType.Name)
                    bsPatientListDataGridView(columnName, i).Value = concentration
                Next
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".InitializePatientListDataGrid ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".InitializePatientListDataGrid ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary> 
    ''' <remarks>
    ''' Created by:  PG 13/10/2010 - Add the LanguageID parameter
    ''' Modified by: RH 22/10/2010 - Remove the LanguageID parameter. Now it is a class property.
    ''' </remarks>
    Private Sub GetScreenLabels()
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For CheckBox, RadioButtons...
            bsPatientsListLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_RESULTS_SUMMARY", LanguageID)
            bsHorizontalRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Summary_Horizontal", LanguageID)
            bsVerticalRadioButton.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Summary_Vertical", LanguageID)

            'For Tooltips...
            bsScreenToolTips.SetToolTip(bsPrintButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Print", LanguageID))
            bsScreenToolTips.SetToolTip(bsExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", LanguageID))

            'Me.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_OpenSummary", LanguageID)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

#End Region

End Class