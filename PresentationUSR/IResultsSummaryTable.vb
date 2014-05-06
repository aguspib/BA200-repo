Option Explicit On
Option Strict On

'Imports System.Configuration
'Imports System.Text
'Imports System.ComponentModel
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
'Imports Biosystems.Ax00.DAL
'Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
'Imports Biosystems.Ax00.Controls.UserControls
'Imports Biosystems.Ax00.PresentationCOM

Public Class IResultsSummaryTable

#Region "Declarations"

    Private TestNames As New List(Of String)
    Private PatientNames As New List(Of String)
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
            myLogAcciones.CreateLogActivity("Table Resume LOAD (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "IResultsSummaryTable.SummaryResultForm_Load", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ResultForm_Load ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ResultForm_Load", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    Private Sub bsPrintButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsPrintButton.Click
        Try
            'Dim Printer As DGVPrinter = New DGVPrinter
            'Printer.Title = Me.Text
            'Printer.SubTitle = Today.ToShortDateString()
            'Printer.SubTitleFormatFlags = StringFormatFlags.LineLimit Or StringFormatFlags.NoClip
            'Printer.PageNumbers = True
            'Printer.PageNumberInHeader = False
            'Printer.PorportionalColumns = True
            'Printer.HeaderCellAlignment = StringAlignment.Near
            'Printer.Footer = "*Biosystems AX00 Automatic Analyser*"
            'Printer.FooterSpacing = 15

            'Printer.TitleSpacing = 10
            'Printer.SubTitleSpacing = 40
            'Printer.ShowTotalPageNumber = True

            'Printer.printDocument.DocumentName = Printer.Title
            'Printer.printDocument.PrinterSettings.DefaultPageSettings.Landscape = bsHorizontalRadioButton.Checked

            'Dim SaveFont As Font = bsPatientListDataGridView.ColumnHeadersDefaultCellStyle.Font
            'Dim FontSize As Single = 7.0! * (10.0! / (bsPatientListDataGridView.Columns.Count - 1))

            'Dim colWidth As Integer

            'If bsPatientListDataGridView.Columns.Count < 11 Then
            '    FontSize = 8.25!
            '    colWidth = 150
            'Else
            '    FontSize = 7.0! * (10.0! / (bsPatientListDataGridView.Columns.Count - 1))
            '    If FontSize < 5.0! Then FontSize = 5.0!
            '    colWidth = 130
            'End If
            'Printer.ColumnWidths.Add(bsPatientListDataGridView.Columns(0).Name, colWidth)

            'Dim NewFont As New Font("Verdana", FontSize)

            'bsPatientListDataGridView.Visible = False
            'bsPatientListDataGridView.ColumnHeadersDefaultCellStyle.Font = NewFont
            'bsPatientListDataGridView.RowsDefaultCellStyle.Font = NewFont
            'bsPatientListDataGridView.AlternatingRowsDefaultCellStyle.Font = NewFont

            'Dim CountToDiv As Integer
            'If bsPatientListDataGridView.Columns.Count < 10 Then
            '    CountToDiv = bsPatientListDataGridView.Columns.Count + 2
            'Else
            '    CountToDiv = bsPatientListDataGridView.Columns.Count + 4
            'End If

            'colWidth = CInt((bsPatientListDataGridView.Width - colWidth + 30) / (CountToDiv))

            'If bsHorizontalRadioButton.Checked Then
            '    colWidth = CInt(colWidth * 1.4)
            'End If

            'For i As Integer = 1 To bsPatientListDataGridView.Columns.Count - 1
            '    Printer.ColumnWidths.Add(bsPatientListDataGridView.Columns(i).Name, colWidth)
            'Next

            'Printer.PrintDataGridView(bsPatientListDataGridView)

            'bsPatientListDataGridView.ColumnHeadersDefaultCellStyle.Font = SaveFont
            'bsPatientListDataGridView.RowsDefaultCellStyle.Font = SaveFont
            'bsPatientListDataGridView.AlternatingRowsDefaultCellStyle.Font = SaveFont

            'bsPatientListDataGridView.Visible = True

            'If String.IsNullOrEmpty(ActiveAnalyzer) Then
            '    Throw New Exception("Invalid ActiveAnalyzer value")
            'End If

            'If String.IsNullOrEmpty(ActiveWorkSession) Then
            '    Throw New Exception("Invalid ActiveWorkSession value")
            'End If

            'XRManager.ShowSummaryResultsReport(ActiveAnalyzer, ActiveWorkSession, bsVerticalRadioButton.Checked)

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsPrintButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsPrintButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

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
            LanguageID = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

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
    ''' Fills the TestNames List with all the test names
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 23/09/2010
    ''' Modified by: RH 28/01/2011 ISE and OFFS TestType
    ''' </remarks>
    Private Sub FillTestsList()
        Try
            If AverageResultsDS Is Nothing Then Return

            TestNames.Clear()

            Dim TestsList As List(Of ResultsDS.vwksResultsRow)
            Dim TestType() As String = {"STD", "CALC", "ISE", "OFFS"}
            Dim TestNameAndSampleClass As String

            For i As Integer = 0 To TestType.Length - 1
                TestsList = _
                    (From row In AverageResultsDS.vwksResults _
                         Where row.TestType = TestType(i) _
                         Select row).ToList()

                For j As Integer = 0 To TestsList.Count - 1
                    TestNameAndSampleClass = String.Format(TestNameFormat, TestsList(j).TestName, TestsList(j).SampleType)
                    If Not TestNames.Contains(TestNameAndSampleClass) Then
                        TestNames.Add(TestNameAndSampleClass)
                    End If
                Next
            Next

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillTestsList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillTestsList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        End Try
    End Sub

    ''' <summary>
    ''' Fills the PatientNames List with all the patient names
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 23/09/2010
    ''' </remarks>
    Private Sub FillPatientsList()
        Try
            If ExecutionsResultsDS Is Nothing Then Return
            PatientNames = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                           Where row.SampleClass = "PATIENT" _
                          Select row.PatientName Distinct).ToList()

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".FillPatientsList ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".FillPatientsList ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

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

            Dim SamplesList As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
            Dim TestsList As List(Of ResultsDS.vwksResultsRow)
            Dim hasConcentrationError As Boolean

            'Get Tests and Patients Names
            FillTestsList()
            FillPatientsList()

            bsPatientListDataGridView.Columns.Clear()
            bsPatientListDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader

            'Patient Name Column
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            bsPatientListDataGridView.Columns.Add("PatientName", myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Summary_PatientName", LanguageID))
            bsPatientListDataGridView.Columns("PatientName").AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            bsPatientListDataGridView.Columns("PatientName").Frozen = True

            'Test Name Columns
            For i As Integer = 0 To TestNames.Count - 1
                bsPatientListDataGridView.Columns.Add(TestNames(i), TestNames(i))
                bsPatientListDataGridView.Columns(TestNames(i)).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            Next

            'Fill the Grid with data
            For i As Integer = 0 To PatientNames.Count - 1
                bsPatientListDataGridView.Rows.Add()
                bsPatientListDataGridView("PatientName", i).Value = PatientNames(i)

                For j As Integer = 0 To TestNames.Count - 1
                    SamplesList = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                  Where row.PatientName = PatientNames(i) _
                                AndAlso String.Format(TestNameFormat, row.TestName, row.SampleType) = TestNames(j) _
                                 Select row).ToList()

                    If SamplesList.Count > 0 Then
                        TestsList = (From row In AverageResultsDS.vwksResults _
                                    Where row.OrderTestID = SamplesList.First.OrderTestID _
                                  AndAlso row.AcceptedResultFlag _
                                   Select row).ToList()

                        If TestsList.Count > 0 Then
                            If Not TestsList.First.IsCONC_ValueNull Then
                                hasConcentrationError = False

                                If Not TestsList.First.IsCONC_ErrorNull Then
                                    hasConcentrationError = Not String.IsNullOrEmpty(TestsList.First.CONC_Error)
                                End If

                                If Not hasConcentrationError Then
                                    bsPatientListDataGridView(TestNames(j), i).Value = TestsList.First.CONC_Value.ToStringWithDecimals(TestsList.First.DecimalsAllowed)
                                Else
                                    bsPatientListDataGridView(TestNames(j), i).Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                    bsPatientListDataGridView(TestNames(j), i).Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                                End If
                            ElseIf Not TestsList.First.IsManualResultTextNull Then 'Off System Test
                                bsPatientListDataGridView(TestNames(j), i).Value = TestsList.First.ManualResultText
                                bsPatientListDataGridView(TestNames(j), i).Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                            Else
                                bsPatientListDataGridView(TestNames(j), i).Value = Nothing
                            End If

                            'Fill Calculated Test Data
                            'Remember: vwksResults Calculated Test relates with Standard Tests in 
                            'vwksWSExecutionsResults through the field ControlNum As OrderTestID
                            ' dl 18/04/2011
                            'TestsList = (From row In AverageResultsDS.vwksResults _
                            '             Where row.TestType = "CALC" _
                            '             AndAlso row.AcceptedResultFlag = True _
                            '             Select row).ToList()
                            TestsList = (From row In AverageResultsDS.vwksResults _
                                         Where row.TestType = "CALC" _
                                         AndAlso row.STDOrderTestID = SamplesList.First.OrderTestID.ToString() _
                                         AndAlso row.AcceptedResultFlag = True _
                                         Select row).ToList()
                            ' end dl

                            'Is this Standard Test a part of a Calculated Test?
                            If TestsList.Count > 0 Then
                                For K As Integer = 0 To TestsList.Count - 1
                                    Dim ColumnName As String = String.Format(TestNameFormat, TestsList(K).TestName, TestsList(K).SampleType)
                                    If Not TestsList(K).IsCONC_ValueNull Then
                                        bsPatientListDataGridView(ColumnName, i).Value = TestsList(K).CONC_Value.ToStringWithDecimals(TestsList(K).DecimalsAllowed)

                                        hasConcentrationError = False

                                        If Not TestsList(K).IsCONC_ErrorNull Then
                                            hasConcentrationError = Not String.IsNullOrEmpty(TestsList(K).CONC_Error)
                                        End If

                                        If Not hasConcentrationError Then
                                            bsPatientListDataGridView(ColumnName, i).Value = TestsList(K).CONC_Value.ToStringWithDecimals(TestsList(K).DecimalsAllowed)
                                        Else
                                            bsPatientListDataGridView(ColumnName, i).Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                            bsPatientListDataGridView(ColumnName, i).Style.Alignment = DataGridViewContentAlignment.MiddleLeft
                                        End If
                                    Else
                                        bsPatientListDataGridView(ColumnName, i).Value = Nothing
                                    End If
                                Next
                            End If
                        End If
                    Else
                        'TestNames(j) do not apply to PatientNames(i)
                        If bsPatientListDataGridView(TestNames(j), i).Value Is Nothing Then
                            bsPatientListDataGridView(TestNames(j), i).Value = "-"
                            bsPatientListDataGridView(TestNames(j), i).Style.Alignment = DataGridViewContentAlignment.MiddleCenter
                        End If
                    End If
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