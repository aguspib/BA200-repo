Option Strict On
Option Explicit On
Option Infer On

'Imports ICSharpCode.SharpZipLib.Zip
Imports System.IO
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL.UpdateVersion
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Controls.UserControls
Imports Biosystems.Ax00.App
Imports Biosystems.Ax00.Framework.CrossCutting


Public Class UiSATReport
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Declaration"

    Private zipExtension As String
    Private checkAllItems As Boolean = True
    Private processing As Boolean = False

    'Private mdiAnalyzerCopy As AnalyzerManager '#REFACTORING
    Private currentLanguage As String = ""

    Private SATFilePath As String = ""
    Private SATFileName As String = ""

    Private EditonMode As Boolean
    Private ChangesMade As Boolean

#End Region

#Region "Events"

    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  DL 09/11/2010
    ''' </remarks>
    Private Sub SATReportData_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'RH 04/07/2011 Escape key should do exactly the same operations as ExitButton_Click()
                ExitButton.PerformClick()
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".SATReportData_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".SATReportData_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub SATReportData_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            currentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            PrepareButtons()
            GetScreenLabels(currentLanguage)

            'TR 16/12/2011 -Get Report SAT Directory
            FolderPathTextBox.BackColor = Color.Gainsboro
            FolderPathTextBox.Text = GetSATReportDirectory()
            FileNameTextBox.Text = GetDefaultReportSATName()
            LoadFilesInSatDirectory()

            If File.Exists(FolderPathTextBox.Text & "\" & FileNameTextBox.Text & GlobalBase.ZIPExtension) Then
                bsSaveSATRepButton.Enabled = False
                resetSaveButtonTimer.Enabled = True
            Else
                bsSaveSATRepButton.Enabled = True
            End If


            'DL 28/07/2011
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)
            'END DL 28/07/2011

            EditonMode = True ' TR 14/02/2012


            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("ISATReport LOAD (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ISATReport.SATReportData_Load", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***


        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SATReportData_Load ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
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
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
            pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60
            Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
        End If

        MyBase.WndProc(m)

    End Sub

    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click
        Try

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'TR 14/02/2012
            Dim CloseForm As Boolean = False
            If ChangesMade Then
                If (ShowMessage(Name, GlobalEnumerates.Messages.DISCARD_PENDING_CHANGES.ToString) = Windows.Forms.DialogResult.Yes) Then
                    StartTime = Now
                    ChangesMade = False
                    CloseForm = True
                End If
            Else
                CloseForm = True
            End If
            If CloseForm Then
                'TR 11/04/2012 -Disable form on close to avoid any button press.
                Me.Enabled = False
                If Not Me.Tag Is Nothing Then
                    'A PerformClick() method was executed
                    Me.Close()
                Else
                    'Normal button click
                    'Open the WS Monitor form and close this one
                    UiAx00MainMDI.OpenMonitorForm(Me)
                End If
            End If
            'TR 14/02/2012 -END.

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            GlobalBase.CreateLogActivity("ISATReport Exit Button (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                            "ISATReport.ExitButton_Click", EventLogEntryType.Information, False)
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " ExitButton_Click ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary></summary>
    ''' <remarks>
    ''' Modified by: SG 03/08/2010
    '''              SA 11/02/2014 - BT #1506 ==> Call the new function for CreateReportSAT
    '''              TR 03/04/2014 - BT #1560- Set the value of variable ChangeMade = false.
    ''' </remarks>
    Private Sub bsSaveSATRepButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSaveSATRepButton.Click
        Try

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            'Dim myLogAcciones As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            'TR 22/12/2011 - Validate if file name exists on the selected folder before starting the ReportSAT creation
            If (FileNameExist(FileNameTextBox.Text)) Then
                ShowMessage("Warning", GlobalEnumerates.Messages.FILE_EXIST.ToString())
                FileNameTextBox.Focus()
            Else
                'TR 03/04/2014 - #1560
                ChangesMade = False
                'TR 03/04/2014 - #1560 -END.

                'TR 22/12/2011 - Validate if the File Path exists in case is a removal device and loose connection
                If Directory.Exists(FolderPathTextBox.Text) Then
                    bsSaveSATRepButton.Enabled = False
                    ExitButton.Enabled = False
                    FolderButton.Enabled = False
                    bsSATDirListBox.Enabled = False
                    UiAx00MainMDI.SetActionButtonsEnableProperty(False) 'AG 12/07/2011 - Disable all vertical action buttons bar

                    Application.DoEvents()
                    Dim workingThread As New Threading.Thread(AddressOf CreateReportSAT_NEW)

                    'TR 09/01/2012 - Indicate RSAT Start on Application LOG
                    GlobalBase.CreateLogActivity("RSAT START  Time: " & Now.ToLongTimeString, Name & ".bsSaveSATRepButton_Click", _
                                      EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)

                    processing = True
                    ScreenWorkingProcess = True  'AG 08/11/2012 - Inform this flag because the MDI requires it
                    workingThread.Start()
                    UiAx00MainMDI.EnableButtonAndMenus(False) 'TR 04/10/2011 -Implement new method.

                    While processing
                        UiAx00MainMDI.InitializeMarqueeProgreesBar()
                        Application.DoEvents()
                        Threading.Thread.Sleep(100)
                    End While
                    UiAx00MainMDI.StopMarqueeProgressBar()

                    'TR 22/12/2011 - Validate if file is created on the current folder.
                    If (File.Exists(FolderPathTextBox.Text & "\" & FileNameTextBox.Text & GlobalBase.ZIPExtension)) Then
                        'Load all SAT reports in current Dir
                        LoadFilesInSatDirectory()
                        resetSaveButtonTimer.Enabled = True
                        Application.DoEvents()

                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                        GlobalBase.CreateLogActivity("Before Email -> ReportSAT Generated (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                        "ISATReport.bsSaveSATRepButton_Click", EventLogEntryType.Information, False)
                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                        'DL 05/12/2012 We don't ask if we want send report by mail. BEGIN
                        'If (ShowMessage("AX00", GlobalEnumerates.Messages.SEND_REPORT_SAT.ToString()) = DialogResult.Yes) Then
                        '    Dim recipient As String = "address@biosystem.es"
                        '    Dim subject As String = MyBase.AnalyzerModel & " Data"
                        '    Dim body As String = "Lorem Ipsum..."

                        '    'SGM 08/03/11 Get from SWParameters table
                        '    Dim myGlobalDataTO As GlobalDataTO
                        '    Dim myParams As New SwParametersDelegate
                        '    'Recipient
                        '    myGlobalDataTO = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.EMAIL_RECIPIENT.ToString, Nothing)
                        '    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        '        recipient = CStr(myGlobalDataTO.SetDatos)
                        '    End If
                        '    'subject
                        '    myGlobalDataTO = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.EMAIL_SUBJECT.ToString, MyBase.AnalyzerModel)
                        '    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        '        subject = CStr(myGlobalDataTO.SetDatos)
                        '    End If
                        '    'body
                        '    myGlobalDataTO = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.EMAIL_BODY.ToString, Nothing)
                        '    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        '        body = CStr(myGlobalDataTO.SetDatos)
                        '    Else
                        '        body = "Lorem Ipsum..."
                        '    End If
                        '    'Dim attachment As String = dirName & zipExtension
                        '    Dim attachment As String = FolderPathTextBox.Text & "\" & FileNameTextBox.Text & GlobalBase.ZIPExtension
                        '    'RH 11/11/2010
                        '    'Using new SendMail(), which uses the default Windows e-mail client, such as:
                        '    'MS Outlook, Outlook Express, Windows Mail, Eudora or Thunderbird, for sending the e-mail,
                        '    'and not forces the user to install MS Outlook instead.
                        '    SendMail(recipient, subject, body, attachment)
                        'End If
                        'DL 05/12/2012 We don't ask if we want send report by mail. END
                    Else
                        'bsProgressBar.Position = 0 'DL 18/07/2011
                        Application.DoEvents()
                        ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), "Unable to create the Report SAT. There has been some errors.")
                    End If
                Else
                    'Directory Do not Exist Message.
                    ShowMessage("Warning", GlobalEnumerates.Messages.PATH_NOFOUND.ToString())
                End If
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " bsSaveSATRepButton_Click ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")

        Finally
            resetSaveButtonTimer.Enabled = True
            ExitButton.Enabled = True
            FolderButton.Enabled = True
            bsSATDirListBox.Enabled = True
            UiAx00MainMDI.SetActionButtonsEnableProperty(True)
            UiAx00MainMDI.EnableButtonAndMenus(True) 'TR 04/10/2011 - Implement new method

            'TR 09/01/2012 - Indicate Rsat END on Application LOG.'TR 09/01/2012 -Indicate Rsat END on Application LOG.
            GlobalBase.CreateLogActivity("RSAT END  Time: " & Now.ToLongTimeString, Name & ".bsSaveSATRepButton_Click", EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
        End Try
    End Sub

    Private Sub bsDeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsDeleteButton.Click
        Try
            If Directory.Exists(FolderPathTextBox.Text) Then
                If (ShowMessage(bsDeleteButton.Text, GlobalEnumerates.Messages.DELETE_REPORT_SAT.ToString()) = Windows.Forms.DialogResult.Yes) Then

                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    Dim StartTime As DateTime = Now
                    'Dim myLogAcciones As New ApplicationLogManager()
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                    Dim CheckedIndicesCount As Integer = bsSATDirListBox.CheckedIndices.Count
                    Dim reportDir As String = ""
                    For i As Integer = CheckedIndicesCount - 1 To 0 Step -1
                        reportDir = FolderPathTextBox.Text & "\" & bsSATDirListBox.Items(bsSATDirListBox.CheckedIndices(i)).ToString()
                        'If File.Exists(reportDir & zipExtension) Then File.Delete(reportDir & zipExtension)
                        If File.Exists(reportDir) Then File.Delete(reportDir)
                    Next

                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    GlobalBase.CreateLogActivity("ISATReport Delete WS Files (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                    "ISATReport.bsDeleteButton_Click", EventLogEntryType.Information, False)
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                    LoadFilesInSatDirectory()
                End If

            Else
                'Directory Do not Exist Message.
                ShowMessage("Warning", GlobalEnumerates.Messages.PATH_NOFOUND.ToString())
            End If

        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " bsDeleteButton_Click ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    Private Sub bsSelectAllDirCheckbox_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSelectAllDirCheckbox.Click
        checkAllItems = False
        For i As Integer = 0 To bsSATDirListBox.Items.Count - 1
            bsSATDirListBox.SetItemChecked(i, bsSelectAllDirCheckbox.Checked)
        Next
        bsDeleteButton.Enabled = bsSelectAllDirCheckbox.Checked
        checkAllItems = True
    End Sub

    Private Sub bsSATDirListBox_ItemCheck(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemCheckEventArgs) Handles bsSATDirListBox.ItemCheck
        If checkAllItems = False Then Return

        If e.NewValue = CheckState.Unchecked Then
            bsSelectAllDirCheckbox.Checked = False
            bsDeleteButton.Enabled = (bsSATDirListBox.CheckedIndices.Count - 1) > 0
        Else
            bsSelectAllDirCheckbox.Checked = (bsSATDirListBox.Items.Count - bsSATDirListBox.CheckedIndices.Count) = 1
            bsDeleteButton.Enabled = True
        End If
    End Sub

    Private Sub resetSaveButtonTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles resetSaveButtonTimer.Tick
        If Not File.Exists(FolderPathTextBox.Text & "\" & FileNameTextBox.Text & GlobalBase.ZIPExtension) Then
            bsSaveSATRepButton.Enabled = True
            resetSaveButtonTimer.Enabled = False
        End If
    End Sub

    Private Sub FolderButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FolderButton.Click
        SetFolderLocation()
    End Sub

    Private Sub FileNameTextBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileNameTextBox.TextChanged
        BsErrorProvider1.Clear()
        If FileNameTextBox.Text.Trim() = String.Empty Then
            BsErrorProvider1.SetError(FileNameTextBox, GetMessageText(GlobalEnumerates.Messages.REQUIRED_VALUE.ToString, currentLanguage))
            FileNameTextBox.Focus()
            bsSaveSATRepButton.Enabled = False
        Else
            bsSaveSATRepButton.Enabled = True

            If EditonMode Then
                ChangesMade = True ' TR 14/02/2012
            End If
        End If
    End Sub

    Private Sub FileNameTextBox_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles FileNameTextBox.KeyPress
        If ValidateSpecialCharacters(e.KeyChar, "[@#~$%&/()-+><_.:,;!¿?=·ªº'¡|}^{]") Then
            e.Handled = True
        End If
    End Sub

#End Region

#Region "Methods"
    ''' <summary>
    ''' Method incharge to load the buttons image.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 30/04/2010
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            auxIconName = GetIconName("OPEN")
            If auxIconName <> "" Then
                FolderButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("CREATE_REP_SAT")
            If auxIconName <> "" Then
                bsSaveSATRepButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("REMOVE")
            If auxIconName <> "" Then
                bsDeleteButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            auxIconName = GetIconName("CANCEL")
            If auxIconName <> "" Then
                ExitButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " PrepareButtons ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Get texts in the current application language for all screen controls
    ''' </summary>
    ''' <param name="pLanguageID"> The current Language of Application </param>
    ''' <remarks>
    ''' Created by: PG 14/10/10
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels.....
            bsSATReportTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_SATReportData", pLanguageID)
            BsTitleCreation.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SATReport_Creation", pLanguageID)

            bsSelectAllDirCheckbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SATReport_DelAll", pLanguageID)

            FolderPathLbl.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FILE_PATH", pLanguageID) + ":"
            FileNameLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FILE_NAME", pLanguageID)
            RepSATFolderLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RSAT_INDIRECTORY", pLanguageID)

            'For Tooltips...
            bsScreenToolTips.SetToolTip(ExitButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CloseScreen", pLanguageID))
            bsScreenToolTips.SetToolTip(bsDeleteButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SATReport_DelSelected", pLanguageID))
            bsScreenToolTips.SetToolTip(bsSaveSATRepButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_SATReport_SaveTTip", pLanguageID))
            bsScreenToolTips.SetToolTip(FolderButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_NEW_FILE_PATH", pLanguageID))
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' New function for CreateRSAT with process for encrypt/decrypt Patient names optimized
    ''' </summary>
    ''' <remarks>
    ''' Created by: SA 11/02/2014
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016) 
    ''' </remarks>
    Private Sub CreateReportSAT_NEW()
        Dim myGlobal As New GlobalDataTO

        Try
            Dim patientsList As New PatientDelegate
            Dim myPatientsToUpdate As New PatientsDS

            Dim myHistPatientsToUpdate As New HisPatientDS
            Dim histPatientsList As New HisPatientsDelegate

            'Encrypt First and Last Names of all Patients in tparPatients 
            myGlobal = patientsList.GetAllForRSAT(Nothing)
            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                myPatientsToUpdate = DirectCast(myGlobal.SetDatos, PatientsDS)

                If (myPatientsToUpdate.tparPatients.Count > 0) Then
                    myGlobal = patientsList.EncryptDataForRSAT(Nothing)
                End If
            End If

            'Encrypt First and Last Names of all Patients in thisPatients 
            myGlobal = histPatientsList.GetAllForRSAT(Nothing)
            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                myHistPatientsToUpdate = DirectCast(myGlobal.SetDatos, HisPatientDS)

                If (myPatientsToUpdate.tparPatients.Count > 0) Then
                    myGlobal = histPatientsList.EncryptDataForRSAT(Nothing)
                End If
            End If

            'XBC 02/08/2012 - If WS status is Running mark this into a flag
            If (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING) Then
                Dim pFlagsDS As New AnalyzerManagerFlagsDS

                'Add row into Dataset to Update
                Dim flagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow
                flagRow = pFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
                With flagRow
                    .BeginEdit()
                    .AnalyzerID = AnalyzerController.Instance.Analyzer.ActiveAnalyzer
                    .FlagID = GlobalEnumerates.AnalyzerManagerFlags.ReportSATonRUNNING.ToString
                    .Value = "INPROCESS"
                    .EndEdit()
                End With
                pFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(flagRow)
                pFlagsDS.AcceptChanges()

                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                myGlobal = myFlagsDelg.Update(Nothing, pFlagsDS)
            End If

            'Generate the ReportSAT (if an error has been raised in the encryption process ignore it)
            SATFileName = FileNameTextBox.Text
            SATFilePath = FolderPathTextBox.Text

            'Dim mySATUtil As New SATReportUtilities
            myGlobal = SATReportUtilities.CreateSATReport(GlobalEnumerates.SATReportActions.SAT_REPORT, False, String.Empty, AnalyzerController.Instance.Analyzer.AdjustmentsFilePath, SATFilePath, SATFileName) 'BA-2471: IT 08/05/2015

            If (Not myGlobal.HasError) Then
                'Restore original values of First and Last Names in tparPatients
                If (myPatientsToUpdate.tparPatients.Count > 0) Then
                    myGlobal = patientsList.DecryptDataAfterRSAT(myPatientsToUpdate)
                    ChangesMade = False 'TR 14/02/2012
                End If

                'Restore original values of First and Last Names in thisPatients
                If (myHistPatientsToUpdate.thisPatients.Count > 0) Then
                    myGlobal = histPatientsList.DecryptDataAfterRSAT(myHistPatientsToUpdate)
                    ChangesMade = ChangesMade OrElse False 'JC 11/06/2013
                End If
            End If

            'XBC 02/08/2012 - If WS status is Running mark this into a flag
            If (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING) Then
                Dim pFlagsDS As New AnalyzerManagerFlagsDS

                'Add row into Dataset to Update
                Dim flagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow
                flagRow = pFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
                With flagRow
                    .BeginEdit()
                    .AnalyzerID = AnalyzerController.Instance.Analyzer.ActiveAnalyzer
                    .FlagID = GlobalEnumerates.AnalyzerManagerFlags.ReportSATonRUNNING.ToString
                    .Value = "CLOSED"
                    .EndEdit()
                End With
                pFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(flagRow)
                pFlagsDS.AcceptChanges()

                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                myGlobal = myFlagsDelg.Update(Nothing, pFlagsDS)
            End If
            'XBC 02/08/2012
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))" + " ((" + ex.HResult.ToString + "))", Me.Name & " CreateReportSAT_NEW", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            Me.UIThread(Function() ShowMessage(Name & ".CreateReportSAT_NEW", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))" + " ((" + ex.HResult.ToString + "))"))
        Finally
            processing = False
            ScreenWorkingProcess = False 'AG 08/11/2012 - Inform this flag because the MDI requires it
        End Try
    End Sub

    ''' <summary>
    ''' Sends an e-mail using the default Windows e-mail client, such as:
    ''' MS Outlook, Outlook Express, Windows Mail, Eudora or Thunderbird
    ''' </summary>
    ''' <param name="pTo"></param>
    ''' <param name="pSubject"></param>
    ''' <param name="pBody"></param>
    ''' <param name="pAttachment"></param>
    ''' <remarks>
    ''' Created by:  RH 11/11/2010
    ''' Modified by: TR 08/03/2012 - Create new message on message table and multilanguage, set implementation:
    '''                              1) Unable to send email to SAT EMAIL_ERROR.
    '''                              2) SAT Email sent successfully. EMAIL_SUCCESS.
    ''' </remarks>
    Private Sub SendMail(ByVal pTo As String, ByVal pSubject As String, ByVal pBody As String, Optional ByVal pAttachment As String = "")
        Try
            Dim ma As New Mapi()
            Dim errorMessage As String = GetMessageText(GlobalEnumerates.Messages.EMAIL_ERROR.ToString(), currentLanguage) '"*Unable to send e-mail to SAT.*"

            If Not ma.Logon(IntPtr.Zero) Then
                'Write error SYSTEM_ERROR in the Application Log
                GlobalBase.CreateLogActivity(errorMessage, Me.Name & " SendMail ", EventLogEntryType.Error, _
                                                                GetApplicationInfoSession().ActivateSystemLog)
                'Show error message
                ShowMessage("Information", GlobalEnumerates.Messages.EMAIL_ERROR.ToString())
                Return
            End If

            If Not String.IsNullOrEmpty(pAttachment) Then ma.Attach(pAttachment)

            ma.AddRecip(pTo, Nothing, False)

            If Not ma.Send(pSubject, pBody) Then
                'Write error SYSTEM_ERROR in the Application Log
                GlobalBase.CreateLogActivity(errorMessage, Me.Name & " SendMail ", EventLogEntryType.Error, _
                                                                GetApplicationInfoSession().ActivateSystemLog)
                'Show error message
                ShowMessage("Information", GlobalEnumerates.Messages.EMAIL_ERROR.ToString())
                Return
            End If

            ma.Logoff()

            GlobalBase.CreateLogActivity(GetMessageText(GlobalEnumerates.Messages.EMAIL_ERROR.ToString(), currentLanguage), Me.Name & " SendMail ", _
                              EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
        Catch ex As Exception
            'Write error SYSTEM_ERROR in the Application Log
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SendMail ", EventLogEntryType.Error, _
                                                            GetApplicationInfoSession().ActivateSystemLog)
            'Show error message
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Encode text to Outlook parameters format
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 03/08/2010
    ''' </remarks>
    Private Function EncodeMail(ByVal pContent As String) As GlobalDataTO
        Dim res As String = pContent
        Dim myResult As New GlobalDataTO

        Try
            res = res.Replace(" ", "%20")
            res = res.Replace(",", "%2C")
            res = res.Replace("?", "%3F")
            res = res.Replace("¿", "%3F")
            res = res.Replace(".", "%2E")
            res = res.Replace("!", "%21")
            res = res.Replace("¡", "%A1")
            res = res.Replace(":", "%3A")
            res = res.Replace(";", "%3B")
            res = res.Replace("@", "%40")
            res = res.Replace("#", "%23")
            res = res.Replace("-", "%2D")
            res = res.Replace("+", "%2B")
            res = res.Replace("_", "%5F")

            myResult.SetDatos = res
        Catch ex As Exception
            myResult.HasError = True
            myResult.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myResult.ErrorMessage = ex.Message

            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " EncodeMail ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myResult
    End Function

    ''' <summary>
    ''' Get the ReportSAT prefix
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: TR 19/12/2011
    ''' </remarks>
    Private Function GetDefaultReportSATName() As String
        Dim myReportSatPrefix As String = ""
        Try
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myParamsDelegate As New SwParametersDelegate

            myGlobalDataTO = myParamsDelegate.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.SAT_REPORT_PREFIX.ToString, Nothing)
            If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                myReportSatPrefix = CStr(myGlobalDataTO.SetDatos) & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT)
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetSatReportPrefix ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return myReportSatPrefix
    End Function

    ''' <summary>
    ''' Get SAT files in Selected Directory
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 20/12/2011
    ''' </remarks>
    Private Sub LoadFilesInSatDirectory()
        Try
            'TR - Validate if Path exists
            If Not FolderPathTextBox.Text = String.Empty AndAlso Directory.Exists(FolderPathTextBox.Text) Then
                zipExtension = ".SAT"
                Dim DirList As New DirectoryInfo(FolderPathTextBox.Text)
                bsSATDirListBox.Items.Clear()
                For Each SATFile As FileSystemInfo In DirList.GetFileSystemInfos("*" & zipExtension).ToList()
                    bsSATDirListBox.Items.Add(SATFile.Name)
                Next

                'Enable/Disable Delete Button And Select all CheckBox
                bsDeleteButton.Enabled = (bsSATDirListBox.CheckedIndices.Count > 0)

                bsSelectAllDirCheckbox.Enabled = (bsSATDirListBox.Items.Count > 0)
                bsSelectAllDirCheckbox.Checked = bsSelectAllDirCheckbox.Checked And bsSelectAllDirCheckbox.Enabled
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " GetFilesInSatDirectory ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Manage the folder location for SAT Reports.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 21/12/2011
    ''' </remarks>
    Private Sub SetFolderLocation()
        Try
            Dim myFolderBrowserDlg As New FolderBrowserDialog
            myFolderBrowserDlg.SelectedPath = FolderPathTextBox.Text
            If myFolderBrowserDlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                If Directory.Exists(myFolderBrowserDlg.SelectedPath) Then
                    If Not FolderPathTextBox.Text = myFolderBrowserDlg.SelectedPath Then
                        FolderPathTextBox.Text = myFolderBrowserDlg.SelectedPath
                        'Save New Path 
                        SaveSATReportDirectory(FolderPathTextBox.Text)
                        'Get files in current folder if exist
                        LoadFilesInSatDirectory()
                    End If
                End If
            End If
        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SetFolderLocation ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
    End Sub

    ''' <summary>
    ''' Validate if the File name selected for Report SAT Exist on selected Folder Path.
    ''' </summary>
    ''' <remarks>
    ''' Created by: TR 22/12/2011
    ''' </remarks>
    Private Function FileNameExist(ByVal pFileName As String) As Boolean
        Dim ExistFileName As Boolean = False
        Try
            'Searh on selected directory if a file with the same name exist
            If File.Exists(FolderPathTextBox.Text & "\" & FileNameTextBox.Text & GlobalBase.ZIPExtension) Then
                ExistFileName = True
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " FileNameExist ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
        End Try
        Return ExistFileName
    End Function

#End Region

#Region "TO DELETE"
    ' ''' <summary>
    ' ''' 
    ' ''' </summary>
    ' ''' <remarks>Created by SG 13/10/10</remarks>
    'Private Sub CreateReportSAT()
    '    Try
    '        'AG 25/10/2011 - Stop ANSINF
    '        Dim myGlobal As New GlobalDataTO
    '        'TR-AG 05/01/2012 -Commented because cause functional error on RUNTIME
    '        'If Not mdiAnalyzerCopy Is Nothing Then
    '        '    If AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then
    '        '        myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP) 'Stop ANSINF
    '        '    End If
    '        'End If
    '        'AG 25/10/2011
    '        'TR-AG 05/01/2012 -END.

    '        If Not myGlobal.HasError Then
    '            ' Begin DL 15/11/2010
    '            ' Backup tparpatient
    '            Dim original_Patients As New PatientsDS
    '            Dim modify_Patients As New PatientsDS
    '            Dim resultData As GlobalDataTO
    '            Dim patientsList As New PatientDelegate

    '            Dim original_HisPatients As New HisPatientDS
    '            Dim modify_HisPatients As New HisPatientDS
    '            Dim hisPatientsList As New HisPatientsDelegate

    '            '-----------
    '            'Modify confidential Data Before Export data to ReportSat
    '            '-----------
    '            ' Patient DL 15/11/2010
    '            resultData = patientsList.GetListWithFilters(Nothing, Nothing)
    '            If Not resultData.HasError Then
    '                original_Patients = CType(resultData.SetDatos, PatientsDS)
    '                modify_Patients = CType(original_Patients.Copy(), PatientsDS)
    '                ' Modify tparPatient Confidential Data 
    '                Dim myPatientIndex As Integer = 1
    '                For Each myrow As PatientsDS.tparPatientsRow In modify_Patients.tparPatients.Rows
    '                    myrow.FirstName = String.Format("FN_{0:000000000}", myPatientIndex)
    '                    myrow.LastName = String.Format("LN_{0:000000000}", myPatientIndex)
    '                    myPatientIndex += 1
    '                    myrow.AcceptChanges()
    '                Next myrow
    '                ' Update on DB before to generate SAT Report
    '                resultData = patientsList.ModifyPatientsByID(Nothing, modify_Patients.tparPatients)
    '            End If

    '            'JC 11/06/2013
    '            ' HistPatient
    '            resultData = hisPatientsList.GetAllPatientsHistory(Nothing)
    '            If Not resultData.HasError Then
    '                original_HisPatients = CType(resultData.SetDatos, HisPatientDS)
    '                modify_HisPatients = CType(original_HisPatients.Copy(), HisPatientDS)
    '                ' Modify tparPatient Confidential Data 
    '                Dim myPatientIndex As Integer = 1
    '                For Each myrow As HisPatientDS.thisPatientsRow In modify_HisPatients.thisPatients.Rows
    '                    myrow.FirstName = String.Format("FN_{0:000000000}", myPatientIndex)
    '                    myrow.LastName = String.Format("LN_{0:000000000}", myPatientIndex)
    '                    myPatientIndex += 1
    '                    myrow.AcceptChanges()
    '                Next myrow
    '                ' Update on DB before to generate SAT Report
    '                resultData = hisPatientsList.ModifyPatientsByID(Nothing, modify_HisPatients.thisPatients)
    '            End If
    '            '---------
    '            'End Modify Confidential Data Before Export data to Report Sat
    '            '---------


    '            'TR 21/12/2011 
    '            SATFileName = FileNameTextBox.Text
    '            SATFilePath = FolderPathTextBox.Text
    '            'TR 21/12/2011 -END.

    '            ' XBC 02/08/2012 - If WS status is Running mark this into a flag
    '            If AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
    '                Dim pFlagsDS As New AnalyzerManagerFlagsDS
    '                'Add row into Dataset to Update
    '                Dim flagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow
    '                flagRow = pFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
    '                With flagRow
    '                    .BeginEdit()
    '                    .AnalyzerID = AnalyzerController.Instance.Analyzer.ActiveAnalyzer
    '                    .FlagID = GlobalEnumerates.AnalyzerManagerFlags.ReportSATonRUNNING.ToString
    '                    .Value = "INPROCESS"
    '                    .EndEdit()
    '                End With
    '                pFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(flagRow)
    '                pFlagsDS.AcceptChanges()

    '                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
    '                myGlobal = myFlagsDelg.Update(Nothing, pFlagsDS)
    '            End If
    '            ' XBC 02/08/2012 

    '            Dim mySATUtil As New SATReportUtilities
    '            'TR 02/02/2012 -Set the result value of CreateSatReport mothod to GlobalDataTO, to 
    '            '               Validate if there's any error on the process.
    '            myGlobal = mySATUtil.CreateSATReport(GlobalEnumerates.SATReportActions.SAT_REPORT, False, "", _
    '                                      MyClass.AnalyzerController.Instance.Analyzer.AdjustmentsFilePath, SATFilePath, SATFileName)

    '            '-----------
    '            'Restore confidential Data After Export data to ReportSat
    '            '-----------
    '            ' Patient
    '            If Not myGlobal.HasError Then
    '                resultData = patientsList.ModifyPatientsByID(Nothing, original_Patients.tparPatients)

    '                ChangesMade = False 'TR 14/02/2012
    '            End If

    '            ' HistPatient JC 11/06/2013
    '            If Not myGlobal.HasError Then
    '                resultData = hisPatientsList.ModifyPatientsByID(Nothing, original_HisPatients.thisPatients)

    '                ChangesMade = ChangesMade OrElse False 'JC 11/06/2013
    '            End If
    '            '---------
    '            'End Restore Confidential Data After Export data to Report Sat
    '            '---------

    '            'TR 02/02/2012 -END
    '        End If

    '        ' XBC 02/08/2012 - If WS status is Running mark this into a flag
    '        If AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
    '            Dim pFlagsDS As New AnalyzerManagerFlagsDS
    '            'Add row into Dataset to Update
    '            Dim flagRow As AnalyzerManagerFlagsDS.tcfgAnalyzerManagerFlagsRow
    '            flagRow = pFlagsDS.tcfgAnalyzerManagerFlags.NewtcfgAnalyzerManagerFlagsRow
    '            With flagRow
    '                .BeginEdit()
    '                .AnalyzerID = AnalyzerController.Instance.Analyzer.ActiveAnalyzer
    '                .FlagID = GlobalEnumerates.AnalyzerManagerFlags.ReportSATonRUNNING.ToString
    '                .Value = "CLOSED"
    '                .EndEdit()
    '            End With
    '            pFlagsDS.tcfgAnalyzerManagerFlags.AddtcfgAnalyzerManagerFlagsRow(flagRow)
    '            pFlagsDS.AcceptChanges()

    '            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
    '            myGlobal = myFlagsDelg.Update(Nothing, pFlagsDS)
    '        End If
    '        ' XBC 02/08/2012


    '        'TR-AG 05/01/2012 -Commented because cause functional error on RUNTIME
    '        ''AG 25/10/2011 - Start ANSINF
    '        'If Not myGlobal.HasError AndAlso Not mdiAnalyzerCopy Is Nothing Then
    '        '    If AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then
    '        '        myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR) 'Start ANSINF
    '        '    End If
    '        'End If
    '        'AG 25/10/2011
    '        'TR-AG 05/01/2012 -END

    '    Catch ex As Exception
    '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CreateReportSAT ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
    '        'DL 15/05/2013
    '        'ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ex.Message + " ((" + ex.HResult.ToString + "))")
    '        Me.UIThread(Function() ShowMessage("Error", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))"))
    '        'DL 15/05/2013

    '    Finally
    '        processing = False
    '        ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it

    '    End Try


    'End Sub
#End Region

#Region "Old SendMail that only works with Outlook.exe"

    '''' <summary>
    '''' Encode the email contents and open the current installed outlook
    '''' </summary>
    '''' <param name="pTo"></param>
    '''' <param name="pSubject"></param>
    '''' <param name="pBody"></param>
    '''' <param name="pAttachment"></param>
    '''' <remarks>Created by SG 03/08/2010</remarks>
    'Private Sub SendMail(ByVal pTo As String, ByVal pSubject As String, ByVal pBody As String, Optional ByVal pAttachment As String = "")
    '    Try
    '        Dim myGlobal As New GlobalDataTO

    '        myGlobal = EncodeMail(pSubject)

    '        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
    '            pSubject = CStr(myGlobal.SetDatos)

    '            myGlobal = EncodeMail(pBody)

    '            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then

    '                pBody = CStr(myGlobal.SetDatos)


    '                Dim strMessage As String = "mailto:" & pTo & "?subject=" & pSubject & "&body=" & pBody ' & "&attachment=""" & pAttachment & """"

    '                If pAttachment <> "" Then
    '                    pAttachment = """" & pAttachment & """"
    '                    System.Diagnostics.Process.Start("Outlook.exe", "/c ipm.note /m " & strMessage & "/a " & pAttachment)
    '                Else
    '                    System.Diagnostics.Process.Start("Outlook.exe", "/c ipm.note /m " & strMessage)
    '                End If
    '            Else
    '                Throw New Exception(myGlobal.ErrorMessage)
    '            End If

    '        Else
    '            Throw New Exception(myGlobal.ErrorMessage)
    '        End If


    '    Catch ex As Exception
    '        'Write error SYSTEM_ERROR in the Application Log
    '        GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " SendMail ", EventLogEntryType.Error, _
    '                                                        GetApplicationInfoSession().ActivateSystemLog)
    '        'Show error message
    '        ShowMessage("Error", "SYSTEM_ERROR", ex.Message + " ((" + ex.HResult.ToString + "))")
    '    End Try
    'End Sub

#End Region

#Region "Constructor"

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        EditonMode = False 'TR 14/02/2012
        ChangesMade = False 'TR 14/02/2012
    End Sub

#End Region

End Class