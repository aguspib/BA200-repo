Option Explicit On
Option Strict On

Imports System.Xml
Imports System.Windows.Forms

Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.Controls.UserControls
Imports LIS.Biosystems.Ax00.LISCommunications
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraEditors


Public Class Ax00MainForm
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

    'Private WithEvents MDIAnalyzerManager As AnalyzerManager

    Dim LocalAnalizerDS As New AnalyzersDS

    Private Sub BsButton1_Click(ByVal sender As System.Object, _
                                ByVal e As System.EventArgs)

        InitializeApplicationInfoSession("BIOSYSTEMS", "BIOSYSTEMS", "ENG")

        Dim myTestProfilesManagement As New IProgTestProfiles

        If Not GetFormFromList(myTestProfilesManagement.Name) Then
            myTestProfilesManagement.MdiParent = Me
            myTestProfilesManagement.Show()
        End If

    End Sub

    Private Sub BsButton2_Click(ByVal sender As System.Object, _
                                ByVal e As System.EventArgs) _
            Handles BsButton2.Click

        'Dim ActiveForm As Form = Me.ActiveMdiChild
        'ActiveForm.Close()

    End Sub

    Private Sub BsButton3_Click(ByVal sender As System.Object, _
                                ByVal e As System.EventArgs) _
            Handles BsButton3.Click

        Dim myNewContaminacion As New IProgTestContaminations
        myNewContaminacion.ShowDialog()
        'MessageBox.Show(GetApplicationInfoSession().UserName)

        'Dim myOrderTestForm As New OrderTest
        'myOrderTestForm.ShowDialog()

        'Dim resultdata As New GlobalDataTO
        'Dim myDelegate As New WSRequiredElementsDelegate
        'resultdata = myDelegate.GetNotPositionedElements(Nothing, Ax00MainMDI.ActiveWorkSession)

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, _
                              ByVal e As System.EventArgs) _
            Handles xmlEncrypt.Click

        Dim f2 As New XmlEncrypt
        f2.ShowDialog()

    End Sub

    Private Sub BsButton5_Click(ByVal sender As System.Object, _
                                ByVal e As System.EventArgs) _
            Handles BsButton5.Click

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

        Dim myForm As New bsReception

        myForm.ShowDialog()

    End Sub

    Private Sub BsButton6_Click(ByVal sender As System.Object, _
                                ByVal e As System.EventArgs) _
            Handles BsButton6.Click

        Dim David As New IProgTest
        'Dim David As New Contaminations

        David.Show()

    End Sub

    Private Sub bsOrderRequestButton_Click(ByVal sender As System.Object, _
                                           ByVal e As System.EventArgs) _
            Handles bsOrderRequestButton.Click

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
            'Write error SYSTEM_ERROR in the Application Log
            CreateLogActivity(ex.Message, Me.Name & " GetAnalyzerInfo ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)

            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message) 'AG 07/07/2010 "SYSTEM_ERROR", ex.Message)
        End Try

    End Sub

    Private Sub Panel2_Paint(ByVal sender As System.Object, _
                             ByVal e As System.Windows.Forms.PaintEventArgs) _
            Handles Panel2.Paint

        GetAnalyzerInfo()

    End Sub


    Private Sub BsButton1_Click_1(ByVal sender As System.Object, _
                                  ByVal e As System.EventArgs) _
            Handles BsButton1.Click

        Dim obj As New IAx00MainMDI
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

    Private Sub bsSusanaButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSusanaButton.Click
        Dim myPWD As String
        Dim mySecurityDEL As New Security.Security

        myPWD = mySecurityDEL.Decryption("7dfCJTp87OCx0M3xFOxlAQ==")

        'Dim myGlobalDataTO As New GlobalDataTO
        'Dim myWSExecDelegate As New ExecutionsDelegate

        'myGlobalDataTO = myWSExecDelegate.CreateWSExecutions(Nothing, "SN0000099999_Ax40", "2013121001", False, -1, String.Empty, False, Nothing)

        'Dim pLAX00Instr As String = "A400;WRUN;M:2;S:2;BP1:87;BT1:6;BRT1:1;BP2:86;BT2:6;BRT2:1;"
        'Dim pToSearch As String = ";BP2:"

        'Dim myTextPos As Integer = InStr(pLAx00Instr, pToSearch)
        'If (myTextPos > 0) Then
        '    Dim myText As String = pLAx00Instr.Substring((myTextPos - 1) + Len(pToSearch))

        '    myTextPos = InStr(myText, ";")
        '    If (myTextPos > 0) Then
        '        myText = myText.Substring(0, myTextPos - 1)

        '        Dim myPosition As Integer = Convert.ToInt32(myText)
        '    End If
        'End If

        'Dim LAX00Inst As String = "A400;WRUN;M:2;S:2;BP1:87;BT1:6;BRT1:1;BP2:86;BT2:6;BRT2:1;"
        ''Dim LAX00Inst As String = "A400;TEST;TI:1;ID:12;M1:5;TM1:1;RM1:1;R1:3;TR1:6;RR1:1;R2:0;TR2:0;RR2:0;VM1:170;VR1:2400;VR2:0;MW:2;RW:6;RN:68"

        'Dim myItems() As String = LAX00Inst.Split(CChar(";"))
        'Dim myInstType As String = LAX00Inst.Split(CChar(";"))(1)

        ''Dim myTextPos As Integer = InStr(LAX00Inst, ";M1:")
        'Dim myTextPos As Integer = InStr(LAX00Inst, ";BP2:")
        'If (myTextPos > 0) Then
        '    'Dim myText As String = LAX00Inst.Substring((myTextPos - 1) + Len(";M1:"))
        '    Dim myText As String = LAX00Inst.Substring((myTextPos - 1) + Len(";BP2:"))

        '    myTextPos = InStr(myText, ";")
        '    If (myTextPos > 0) Then
        '        myText = myText.Substring(0, myTextPos - 1)

        '        Dim myPosition As Integer = Convert.ToInt32(myText)
        '    End If
        'End If


        'Dim myGlobalDataTO As New GlobalDataTO
        'Dim myHisWSOTsDelegate As New HisWSOrderTestsDelegate

        'myGlobalDataTO = myHisWSOTsDelegate.ReadAll(Nothing, "834000134")
        'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '    Dim myHisWSOrderTestsDS As HisWSOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, HisWSOrderTestsDS)

        '    If (myHisWSOrderTestsDS.thisWSOrderTests.Rows.Count > 0) Then
        '        Dim myHisWSResultsDelegate As New HisWSResultsDelegate

        '        Debug.Print("INICIO: " & Now)
        '        myGlobalDataTO = myHisWSResultsDelegate.DeleteResults(Nothing, myHisWSOrderTestsDS)
        '        Debug.Print("FIN: " & Now)
        '    End If
        'End If
    End Sub

    'Public Function ProcessToMountTheNewSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSAnalyzerID As String) As GlobalDataTO
    '    Dim myGlobal As New GlobalDataTO
    '    Try
    '        Dim ActiveAnalyzer As String = "834000004"
    '        Dim ActiveWorkSession As String = "2012072701"
    '        Dim WorkSessionStatusAttribute As String = "PENDING"

    '        ' Save the current WS using  ActiveAnalyzer as name
    '        Dim myWSDelegate As New WorkSessionsDelegate
    '        Dim mySavedWSDelegate As New SavedWSDelegate
    '        Dim savedWSID As Integer
    '        myGlobal = myWSDelegate.GetOrderTestsForWS(pDBConnection, ActiveWorkSession)
    '        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
    '            Dim myWorkSessionResultDS As WorkSessionResultDS = DirectCast(myGlobal.SetDatos, WorkSessionResultDS)
    '            ' Save temporally the WS to recuperate it after the Reset of the current WS
    '            myGlobal = mySavedWSDelegate.Save(pDBConnection, ActiveAnalyzer, myWorkSessionResultDS, -1)
    '        End If

    '        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
    '            savedWSID = DirectCast(myGlobal.SetDatos, Integer)
    '            ' Reset the current WS
    '            Dim myWS As New WorkSessionsDelegate
    '            myGlobal = myWS.ResetWS(pDBConnection, pWSAnalyzerID, ActiveWorkSession, False)
    '        End If

    '        If Not myGlobal.HasError Then
    '            ' Delete ReactionsRotor
    '            Dim myReactionsRotorDelegate As New ReactionsRotorDelegate
    '            'myGlobal = myReactionsRotorDelegate.DeleteAll(pDBConnection, ActiveAnalyzer, ActiveWorkSession)
    '            '' Update field AnalyzerID in table Reactions Rotor WS
    '            myGlobal = myReactionsRotorDelegate.UpdateWSAnalyzerID(pDBConnection, ActiveAnalyzer, pWSAnalyzerID)
    '        End If

    '        If Not myGlobal.HasError Then
    '            ' Load new WorkSession saved with name ActiveAnalyzer
    '            Dim myOrderTests As New OrderTestsDelegate
    '            myGlobal = myOrderTests.LoadFromSavedWSToChangeAnalyzer(pDBConnection, savedWSID, ActiveAnalyzer)
    '            Dim myWSOrderTestsDS As New WSOrderTestsDS
    '            If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
    '                Dim myWorkSessionsDS As WorkSessionsDS = DirectCast(myGlobal.SetDatos, WorkSessionsDS)
    '                If (myWorkSessionsDS.twksWorkSessions.Rows.Count = 1) Then
    '                    ActiveWorkSession = myWorkSessionsDS.twksWorkSessions(0).WorkSessionID
    '                    WorkSessionStatusAttribute = myWorkSessionsDS.twksWorkSessions(0).WorkSessionStatus
    '                End If
    '            End If
    '        End If

    '        If Not myGlobal.HasError Then
    '            myGlobal = mySavedWSDelegate.ExistsSavedWS(pDBConnection, ActiveAnalyzer)
    '            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
    '                Dim mySavedWSDS As SavedWSDS = DirectCast(myGlobal.SetDatos, SavedWSDS)
    '                ' Delete the remporally Work Session created initially
    '                myGlobal = mySavedWSDelegate.Delete(pDBConnection, mySavedWSDS)
    '            End If
    '        End If

    '    Catch ex As Exception
    '        myGlobal.HasError = True
    '        myGlobal.ErrorCode = "SYSTEM_ERROR"
    '        myGlobal.ErrorMessage = ex.Message

    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessToMountTheNewSession", EventLogEntryType.Error, False)
    '    End Try
    '    Return myGlobal
    'End Function

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSusanaTest2.Click
        Dim myGlobal As New GlobalDataTO
        Dim dbConnection As SqlClient.SqlConnection

        myGlobal = DAOBase.GetOpenDBConnection(Nothing)
        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
            dbConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)
            If (Not dbConnection Is Nothing) Then
                Dim myOrdersDelegate As New OrdersDelegate
                myGlobal = myOrdersDelegate.ImportFromLIMS(Nothing, "C:\Users\Susana Angueira\Documents\Ax00 v1.1\AX00\PresentationUSR\bin\x86\Debug\Import\Import.txt", New WorkSessionResultDS, _
                                                           "834000114", "C:\Users\Susana Angueira\Documents\", "2013041601", "EMPTY", True)


                'Dim myXMLMsgDelegate As New xmlMessagesDelegate
                'myGlobal = myXMLMsgDelegate.CancelAWOSID(Nothing, "AWOSID78")


                'Dim myXMLMsgDAO As New twksXmlMessagesDAO
                'myGlobal = myXMLMsgDAO.ReadByStatus(dbConnection, "AAA")

                'If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                '    Dim myXMLMessages As List(Of XMLMessagesTO) = DirectCast(myGlobal.SetDatos, List(Of XMLMessagesTO))

                '    If (myXMLMessages.Count > 0) Then
                '        Dim myUtils As New Utilities
                '        myGlobal = myUtils.GetNewGUID()
                '        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                '            Dim myGUID As String = DirectCast(myGlobal.SetDatos, String)

                '            myGlobal = myXMLMsgDAO.Create(dbConnection, myGUID, myXMLMessages(0).XMLMessage, "PENDING")
                '        End If
                '    End If
                'End If
            End If
        End If




        'Dim myOTDelegate As New OperateCalculatedTestDelegate

        'myGlobal = myOTDelegate.ExecuteCalculatedTest(Nothing, 276, False)

        'Using MyForm As IWSIncompleteSamplesAuxScreen()
        'MyForm.wo
        'MyForm.analyzerid()
        'MyForm.SampleClass = "PATIENT"
        'MyForm.SampleType = bsSampleTypeComboBox.SelectedValue.ToString()
        'MyForm.SampleTypeName = bsSampleTypeComboBox.Text
        'MyForm.ListOfSelectedTests = mySelectedTestsDS
        'MyForm.MaxValues = myMaxOrderTestsDS


        'End Using
        'Dim f As New IWSIncompleteSamplesAuxScreen ' WSTestForm
        'f.ShowDialog()

        ' From IAx00MainMDI
        'Using MyForm As New IWSIncompleteSamplesAuxScreen()
        '    MyForm.AnalyzerID = "SN0000099999_Ax400"
        '    MyForm.WorkSessionID = "2011091201"
        '    MyForm.WorkSessionStatus = "EMPTY"
        '    MyForm.SourceScreen = GlobalEnumerates.SourceScreen.START_BUTTON
        '    MyForm.ShowDialog()

        '    'OpenRotorPositionsForm(Nothing)
        '    IAx00MainMDI.OpenRotorPositionsForm(Nothing)
        'End Using
        ''

        ' From IWSRotorPositions
        'Using MyForm As New IWSIncompleteSamplesAuxScreen()
        '    MyForm.AnalyzerID = "SN0000099999_Ax400"
        '    MyForm.WorkSessionID = "2011090501"
        '    MyForm.WorkSessionStatus = ""
        '    MyForm.SourceScreen = "ROTORPOS"
        '    MyForm.ShowDialog()
        'End Using
        ' InitializeScreen (false, false)

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

    Private Sub LogFileView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LogFileView.Click
        Dim myApplicationLogView As New ApplicationLogView

        myApplicationLogView.WorkSessionID = IAx00MainMDI.ActiveWorkSession
        myApplicationLogView.ShowDialog()
    End Sub

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

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        UpdateProcessValidation.ShowDialog()
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
                    resultData = xlsResults.ExportTestXLS("2013101701", pathname, "Test.xls", "834000134")
                    If resultData.HasError Then
                        Dim myLogAcciones As New ApplicationLogManager()
                        myLogAcciones.CreateLogActivity(resultData.ErrorMessage, "ExportCalculations.ExportResults. ExportXLS", EventLogEntryType.Error, False)

                        'DL 15/05/2013
                        'ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), resultData.ErrorMessage, Me)
                        Me.UIThread(Function() ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), resultData.ErrorMessage, Me))
                        'DL 15/05/2013
                    End If
                End If
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message, Me.Name & ".ExportResults ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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

End Class