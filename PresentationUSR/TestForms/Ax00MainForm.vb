Option Explicit On
Option Strict On
Option Infer On
Imports System.Windows.Forms
Imports Biosystems.Ax00.App

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.Core.Services


Public Class Ax00MainForm
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Private WithEvents MDIAnalyzerManager As AnalyzerManager

    Dim LocalAnalizerDS As New AnalyzersDS

#Region "PERMANENT BUTTONS - DO NOT DELETE THEM!!!"
    Private Sub bsTestUpdateVersionProcessButton_Click(sender As Object, e As EventArgs) Handles bsTestUpdateVersionProcessButton.Click
        UpdateProcessValidation.ShowDialog()
        Me.Close()
    End Sub

    Private Sub bsXmlEncryptButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsXmlEncryptButton.Click
        XmlEncrypt.ShowDialog()
        Me.Close()
    End Sub

    Private Sub bsLogFileViewButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsLogFileViewButton.Click
        Dim myApplicationLogView As New ApplicationLogView

        myApplicationLogView.WorkSessionID = UiAx00MainMDI.ActiveWorkSession
        myApplicationLogView.ShowDialog()
    End Sub

    Private Sub bsAGTestingButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsAGTestingButton.Click
        Dim myForm As New bsReception
        myForm.ShowDialog()

        '// AG tests button - calculations
        'Dim Calc As New AG_Tests
        'Calc.Show()

        ''//AG Tests buttons - test programming
        'Dim myTestProgrammingForm As New TestProgramingParammetersTR

        'If LocalAnalizerDS.tcfgAnalyzers.Rows.Count > 0 Then
        '    myTestProgrammingForm.AnalyzerModel = LocalAnalizerDS.tcfgAnalyzers(0).AnalyzerModel
        '    myTestProgrammingForm.AnalyzerID = LocalAnalizerDS.tcfgAnalyzers(0).AnalyzerID
        '    myTestProgrammingForm.WorkSessionID = ""
        'End If

        'myTestProgrammingForm.Show()   
    End Sub
#End Region

#Region "OTHER BUTTONS"
    Private Sub BsButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        InitializeApplicationInfoSession("BIOSYSTEMS", "BIOSYSTEMS", "ENG")

        Dim myTestProfilesManagement As New UiProgTestProfiles
        If Not GetFormFromList(myTestProfilesManagement.Name) Then
            myTestProfilesManagement.MdiParent = Me
            myTestProfilesManagement.Show()
        End If
    End Sub

    Private Sub BsButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton2.Click
        'Dim ActiveForm As Form = Me.ActiveMdiChild
        'ActiveForm.Close()
    End Sub

    Private Sub BsButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton3.Click
        Dim myNewContaminacion As New UiProgTestContaminations
        myNewContaminacion.ShowDialog()
        'MessageBox.Show(GetApplicationInfoSession().UserName)

        'Dim myOrderTestForm As New OrderTest
        'myOrderTestForm.ShowDialog()

        'Dim resultdata As New GlobalDataTO
        'Dim myDelegate As New WSRequiredElementsDelegate
        'resultdata = myDelegate.GetNotPositionedElements(Nothing, Ax00MainMDI.ActiveWorkSession)
    End Sub

    Private Sub BsButton6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton6.Click
        Dim David As New UiProgTest
        David.Show()
    End Sub

    Private Sub bsOrderRequestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsOrderRequestButton.Click
        Dim myf As New Ax00MainMenuForm
        myf.ShowDialog()
    End Sub

    Private Sub GetAnalyzerInfo()
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myAnalyzerDelegate As New AnalyzersDelegate

            ' XBC 07/06/2012
            'myGlobalDataTO = myAnalyzerDelegate.GetAnalyzer(Nothing)
            myGlobalDataTO = myAnalyzerDelegate.CheckAnalyzer(Nothing)
            ' XBC 07/06/2012

            If Not myGlobalDataTO.HasError Then
                LocalAnalizerDS = CType(myGlobalDataTO.SetDatos, AnalyzersDS)
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & " GetAnalyzerInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message) 'AG 07/07/2010 "SYSTEM_ERROR", ex.Message)
        End Try
    End Sub

    Private Sub Panel2_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel2.Paint
        GetAnalyzerInfo()
    End Sub

    Private Sub BsButton1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BsButton1.Click
        Dim obj As New UiAx00MainMDI
        obj.ShowDialog()
    End Sub

    'Private Sub Button3_Click(ByVal sender As System.Object, _
    '                          ByVal e As System.EventArgs) _
    '        Handles Button3.Click

    '    'A place for testing basic classes

    '    'TestWSPreparationsDelegate.RunTest()
    '    'TestExecutionsDelegate.RunTest()
    '    'TestOperateCalculatedTestDelegate.RunTest()

    '    'tparTestDAO_TestClass.RunTest()

    '    'Dim testForm As New ResultForm
    '    'testForm.ShowDialog()

    '    'Dim WSStatesTest As New WSStates

    '    'WSStatesTest.ActiveAnalyzer = Ax00MainMDI.ActiveAnalyzer
    '    'WSStatesTest.ActiveWorkSession = Ax00MainMDI.ActiveWorkSession
    '    'WSStatesTest.CurrentWorkSessionStatus = "PENDING"
    '    'WSStatesTest.ShowDialog()

    '    'Dim MP1 As New IMonitor
    '    'MP1.ShowDialog()

    '    'XRManager.ShowUsersReport()
    '    'XRManager.ShowTestProfilesReport()

    '    'TestHisCalculatedTestsDelegate.RunTest()
    '    TestHisAnalyzerWorkSessionsDelegate.RunTest()
    'End Sub

    Private Sub butTestSergio_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butTestSergio.Click
        Dim f As New LIS_Test(Me.AnalyzerModel)
        f.Text = "LIS Test"
        f.Show()
    End Sub

    Private Sub bsReadLISFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) 'Handles bsReadLISFile.Click
        'Dim SessionFile As New OpenFileDialog()
        'If SessionFile.ShowDialog() = Windows.Forms.DialogResult.OK Then
        '    Dim myOrdersDelegate As New OrdersDelegate()
        '    Dim myLimsSession As New List(Of LimsWorkSessionTO)
        '    myLimsSession = myOrdersDelegate.ReadLimsWSFile(SessionFile.FileName)

        '    If myLimsSession.Count > 0 Then
        '        myOrdersDelegate.ImportsOrders(myLimsSession)
        '    End If
        'End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim f As New Form_XBC
        f.ShowDialog()
    End Sub

    ' deleted 'RH Tests 2' GUI button
    'Private Sub Button5_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
    '    If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
    '        Dim resultData As GlobalDataTO

    '        Dim myAnalyzerManager As AnalyzerManager = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager)
    '        If myAnalyzerManager.CommThreadsStarted Then
    '            resultData = myAnalyzerManager.SimulateSendNext(98)

    '            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
    '                Dim FoundPreparation As AnalyzerManagerDS
    '                FoundPreparation = CType(resultData.SetDatos, AnalyzerManagerDS)

    '                If FoundPreparation.nextPreparation.Rows.Count > 0 Then
    '                    Dim myRow As AnalyzerManagerDS.nextPreparationRow = FoundPreparation.nextPreparation(0)

    '                    MessageBox.Show("Execution found: " & myRow.ExecutionID)
    '                End If
    '            End If
    '        End If
    '    End If
    'End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        ISE_Test.Show()
        Me.Close()
    End Sub

    'Private Sub btnIR_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnIR.Click
    '    FormIR.Show()
    '    Me.Close()
    'End Sub

    Private Sub BsGenerateISECodesButton_Click(sender As Object, e As EventArgs) Handles BsGenerateISECodesButton.Click
        ISECodeGenerator.Show()
        Me.Close()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim myInstalledForm As New InstallerForm
        myInstalledForm.ShowDialog()
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        'Dim m As New XtraMessageBox()

        'If you wish stop after finish the in course preparations press STOP
        'If you wish stop immediately (losing the in course preparations) press ABORT
        'If you wish close this message and continue press CANCEL
        Dim message As String = String.Format("If you wish to stop after the in course preparations are complete press {0}." & vbNewLine & _
             "If you wish to stop Immediately press {1}." & vbNewLine & "If you wish to close this message press {2}", "STOP", "ABORT", "CANCEL")
        Dim message2 As String = String.Format("Do you want to stop (cancel)? or continue (retry)?")
        Dim title As String = "SESSION STOP"
        Dim dialogres As DialogResult = BSCustomMessageBox.Show(Me, message2, title, MessageBoxButtons.RetryCancel, MessageBoxIcon.Question, "Continue", "Stop", "")

        Select Case dialogres
            Case DialogResult.Retry
                MessageBox.Show("you clicked Continue")
            Case System.Windows.Forms.DialogResult.Retry

            Case System.Windows.Forms.DialogResult.Cancel, DialogResult.Ignore
                MessageBox.Show("you clicked Stop")
        End Select
    End Sub

    Private Sub BsButton4_Click(sender As Object, e As EventArgs) Handles BsButton4.Click
        ExportResults()
    End Sub

    Public Sub ExportResults()
        Try
            Dim resultData As GlobalDataTO
            Dim swParams As New SwParametersDelegate
            resultData = swParams.ReadByAnalyzerModel(Nothing, AnalyzerModel)

            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                Dim myDS As ParametersDS
                myDS = CType(resultData.SetDatos, ParametersDS)

                Dim myList As List(Of ParametersDS.tfmwSwParametersRow)
                myList = (From a As ParametersDS.tfmwSwParametersRow In myDS.tfmwSwParameters _
                          Where String.Compare(a.ParameterName, GlobalEnumerates.SwParameters.XLS_RESULTS_DOC.ToString(), False) = 0 _
                          Select a).ToList()

                Dim filename As String = ""
                If myList.Count > 0 Then filename = myList(0).ValueText

                myList = (From a As ParametersDS.tfmwSwParametersRow In myDS.tfmwSwParameters _
                          Where String.Compare(a.ParameterName, GlobalEnumerates.SwParameters.XLS_PATH.ToString(), False) = 0 _
                          Select a).ToList()

                Dim pathname As String = ""
                If myList.Count > 0 Then pathname = myList(0).ValueText

                If pathname.StartsWith("\") AndAlso Not pathname.StartsWith("\\") Then
                    pathname = Application.StartupPath & pathname & DateTime.Now.ToString("dd-MM-yyyy HH-mm") & "\"
                End If

                If String.Compare(filename, "", False) <> 0 AndAlso String.Compare(pathname, "", False) <> 0 Then
                    Dim xlsResults As New ResultsExcelTest
                    'resultData = xlsResults.ExportXLS(WorkSessionIDField, pathname, filename, AnalyzerIDField)
                    resultData = xlsResults.ExportTestXLS("2013101701", pathname, "Test.xls", "834000134", GlobalEnumerates.BaseLineType.STATIC.ToString)
                    If resultData.HasError Then
                        'Dim myLogAcciones As New ApplicationLogManager()
                        GlobalBase.CreateLogActivity(resultData.ErrorMessage, "ExportCalculations.ExportResults. ExportXLS", EventLogEntryType.Error, False)

                        'DL 15/05/2013
                        'ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), resultData.ErrorMessage, Me)
                        Me.UIThread(Function() ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), resultData.ErrorMessage, Me))
                        'DL 15/05/2013
                    End If
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message, Me.Name & ".ExportResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            'DL 15/05/2013
            'ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message)
            Me.UIThread(Function() ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message))
            'DL 15/05/2013

        Finally
            ' CreatingXlsResults = False
            ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it
        End Try
    End Sub

    'JV 29/10/2013
    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        Dim messageBoxType As MessageBoxButtons = MessageBoxButtons.AbortRetryIgnore
        Dim userAnswer As DialogResult = DialogResult.No

        userAnswer = BSCustomMessageBox.Show(Me, "Default button Left", My.Application.Info.Title, messageBoxType, MessageBoxIcon.Warning, BSCustomMessageBox.BSMessageBoxDefaultButton.LeftButton, "Stop", "Abort", "Cancel")

        userAnswer = BSCustomMessageBox.Show(Me, "Default button Middle", My.Application.Info.Title, messageBoxType, MessageBoxIcon.Warning, BSCustomMessageBox.BSMessageBoxDefaultButton.MiddleButton, "Stop", "Abort", "Cancel")

        userAnswer = BSCustomMessageBox.Show(Me, "Default button Right", My.Application.Info.Title, messageBoxType, MessageBoxIcon.Warning, BSCustomMessageBox.BSMessageBoxDefaultButton.RightButton, "Stop", "Abort", "Cancel")
    End Sub
    'JV 29/10/2013

    Private Sub SimpleButton2_Click(sender As Object, e As EventArgs) Handles SimpleButton2.Click
        Dim resultData As New GlobalDataTO
        Dim myExDlg As New ExecutionsDelegate
        'resultData = myExDlg.ExistCriticalPauseTests(Nothing, "SN0000099999_Ax400", "A400", "2013112501")

        resultData = myExDlg.ExistCriticalPauseTests(Nothing, "834000160", "A400", "2013111901")

        'Button to Show
        Dim messageBoxType As MessageBoxButtons = MessageBoxButtons.YesNo
        Dim userAnswer As DialogResult = DialogResult.No
        'Multilanguage for Wait / Pause!!
        userAnswer = BSCustomMessageBox.Show(Me, "En este momento el analizador está realizando preparaciones que se pueden ver afectadas por la pausa, se recomienda esperar a que termine la dispensación", _
                                             My.Application.Info.Title, messageBoxType, MessageBoxIcon.Warning, BSCustomMessageBox.BSMessageBoxDefaultButton.LeftButton, "", "Wait", "Pause")
        MessageBox.Show("Resp.: " & userAnswer.ToString())
    End Sub
#End Region

    Private Sub MITestButtonClick(sender As Object, e As EventArgs) Handles MITestProcess.Click

    End Sub
End Class
