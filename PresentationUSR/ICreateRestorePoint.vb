Option Strict On
Option Explicit On

Imports System.IO
'Imports System.Configuration
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.BL.UpdateVersion
Imports Biosystems.Ax00.CommunicationsSwFw

Public Class ICreateRestorePoint
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm

#Region "Properties"

    

#End Region

#Region "AttributesFields"


#End Region

#Region "Declarations"

    Private CreatingRestorePoint As Boolean = False
    Private RestorePointCreated As Boolean = False

    Private ErrorMessage As String

    Private RestorePointPath As String

    Private CurrentLanguage As String
    Private mdiAnalyzerCopy As AnalyzerManager

#End Region

#Region "Methods"

    ''' <summary>
    ''' Method incharge to load the button image
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 30/11/2012
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'OK Button
            auxIconName = GetIconName("CREATE_REP_SAT")
            If (auxIconName <> "") Then
                bsCreateButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

            'CANCEL 
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsCancelButton.Image = Image.FromFile(iconPath & auxIconName)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the screen texts in the current application Language
    ''' </summary>
    ''' <param name="pLanguageID">Current Language Identifier</param>
    ''' <remarks>
    ''' Created by: SG 30/11/2012
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            bsTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSAT_Restore", pLanguageID)
            bsFileNameTitleLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FILE_NAME", pLanguageID)

            'For ToolTips...
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CancelSelection", pLanguageID))
            bsScreenToolTips.SetToolTip(bsCreateButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSAT_Restore", pLanguageID))

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' CREATED BY: ¿?
    ''' MODIFIED BY: TR -Add the application version at the end of the file name, 
    '''                  this is to allow filtering by application version when restore.
    ''' </remarks>
    Private Sub GetRestorePointName()

        Try
            Dim myGlobal As New GlobalDataTO
            Dim myRestoreDataPath As String
            Dim myParams As New SwParametersDelegate
            myGlobal = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.RESTORE_POINT_PREFIX.ToString, Nothing)
            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                myRestoreDataPath = CStr(myGlobal.SetDatos)
            Else
                myRestoreDataPath = "RestorePoint"
            End If
            'TR 14/06/2013 -Add the application version ad the end of the name
            Me.bsRestorepointNameTextBox.Text = myRestoreDataPath & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT) & " " & Application.ProductVersion

        Catch ex As Exception

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetRestorePointName", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetRestorePointName", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

    End Sub


    Private Function CreateRestorePoint() As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities
        Dim tempFolder As String = ""
        Dim mySATUtil As New SATReportUtilities

        Try

            'the user wants to be able to restore his current data in the future
            Dim SATThread As New Threading.Thread(AddressOf CreateRestorePointStart)
            Me.CreatingRestorePoint = True
            ScreenWorkingProcess = True 'AG 08/11/2012 - inform this flag because the MDI requires it
            Application.DoEvents() 'RH 16/11/2010

            SATThread.Start()

            While Me.CreatingRestorePoint 'RH 16/11/2010
                IAx00MainMDI.InitializeMarqueeProgreesBar()
                Application.DoEvents()
                Threading.Thread.Sleep(100)
            End While
            IAx00MainMDI.StopMarqueeProgressBar()

            SATThread.Join()

            If Not RestorePointCreated Then
                Application.DoEvents()
                Dim res As DialogResult = ShowMessage(Me.Name & ".CreateRestorePoint", GlobalEnumerates.Messages.SAT_SAVE_RESTORE_POINT_ERROR.ToString)
                If res = DialogResult.No Then
                    'RH 07/02/2012 Return from the Function.
                    'Avoid the execution of the next code lines.
                    'Log the error
                    CreateLogActivity(GlobalEnumerates.Messages.SAT_SAVE_RESTORE_POINT_ERROR.ToString(), _
                                      Me.Name & ".CreateRestorePoint", EventLogEntryType.Error, _
                                      GetApplicationInfoSession().ActivateSystemLog)

                    myGlobal.HasError = False
                    myGlobal.SetDatos = Nothing
                    'myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                    'myGlobal.ErrorMessage = GlobalEnumerates.Messages.SAT_SAVE_RESTORE_POINT_ERROR.ToString()
                    Return myGlobal
                    'END RH 07/02/2012
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            myGlobal.ErrorMessage = ex.Message

            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".CreateRestorePoint", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".CreateRestorePoint", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        Finally

            If Directory.Exists(tempFolder) Then DeleteDirectory(tempFolder) 'RH 31/05/2011

            If ErrorMessage <> String.Empty Then
                ShowMessage(Me.Name & ".CreateRestorePoint", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ErrorMessage)
            End If

        End Try

        Return myGlobal
    End Function

    Public Shared Sub DeleteDirectory(ByVal target_dir As String)
        Dim files As String() = Directory.GetFiles(target_dir)
        Dim dirs As String() = Directory.GetDirectories(target_dir)

        For Each FileName As String In files
            File.SetAttributes(FileName, FileAttributes.Normal)
            File.Delete(FileName)
        Next

        For Each DirName As String In dirs
            DeleteDirectory(DirName)
        Next

        Directory.Delete(target_dir, False)
    End Sub





    ''' <summary>
    ''' Creates a Restore Point of the current status and data in order not to lose it
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 08/10/2010
    ''' Modified by: RH 12/11/2010
    ''' Modified by AG 25/10/2011 - before create a restore point disable ANSINF, once finished enable it
    ''' </remarks>
    Private Sub CreateRestorePointStart()
        'Cursor.Current = Cursors.WaitCursor
        Dim myGlobal As GlobalDataTO

        Try

            Dim mySATUtil As New SATReportUtilities
            myGlobal = mySATUtil.CreateSATReport(GlobalEnumerates.SATReportActions.SAT_RESTORE, False, "", MyClass.mdiAnalyzerCopy.AdjustmentsFilePath)
            If Not myGlobal.HasError AndAlso Not myGlobal Is Nothing Then
                RestorePointCreated = CBool(myGlobal.SetDatos)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CreateRestorePoint ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ErrorMessage = ex.Message

        Finally
            CreatingRestorePoint = False
            ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it
        End Try
    End Sub



    ''' <summary>
    ''' Check if the OK button can be enabled or not depending the instrument conection and status
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>AG 25/10/2011</remarks>
    Private Function AllowStartProcess() As Boolean
        Dim returnValue As Boolean = True
        Try

            'Ax00 allow load RSAT / Restore Point when: No connection or (sleeping + no working) or (standby + no working)
            'NOTE in Ax5 the condition was more secure (no connection or sleeping)

            If Not mdiAnalyzerCopy Is Nothing Then
                If Not mdiAnalyzerCopy.Connected Then 'AG 06/02/2012 - add Connected to the acitvation rule
                    returnValue = True

                    'DL 02/07/2012. Begin
                Else
                    returnValue = False
                    'ElseIf mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING _
                    'OrElse (mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY AndAlso Not mdiAnalyzerCopy.AnalyzerIsReady) _
                    'OrElse (mdiAnalyzerCopy.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING AndAlso Not mdiAnalyzerCopy.AnalyzerIsReady) Then
                    'returnValue = False
                    'DL 02/07/2012. End 

                    ' XBC 04/07/2012
                    IAx00MainMDI.ErrorStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                    IAx00MainMDI.ErrorStatusLabel.Text = GetMessageText(GlobalEnumerates.Messages.LOADRSAT_NOTALLOWED.ToString)
                    ' XBC 04/07/2012
                End If
            End If

        Catch ex As Exception
            returnValue = False
        End Try
        Return returnValue
    End Function


#End Region

#Region "Events"




    ''' <summary>
    ''' When the screen ESC key is pressed, the screen is closed
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SGM 30/11/2012
    ''' </remarks>
    Private Sub ICreateRestorePoint_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'RH 04/07/2011 Escape key should do exactly the same operations as bsCancelButton_Click()
                bsCancelButton.PerformClick()

            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadSATReportData_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadSATReportData_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load SAT data
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 30/11/2012
    ''' </remarks>
    Private Sub ICreateRestorePoint_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try

            'DL 28/07/2011
            Dim myLocation As Point = Me.Parent.Location
            Dim mySize As Size = Me.Parent.Size

            Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)
            'END DL 28/07/2011

            If Not AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager") Is Nothing Then
                mdiAnalyzerCopy = CType(AppDomain.CurrentDomain.GetData("GlobalAnalyzerManager"), AnalyzerManager) 'AG 13/07/2011 - Use the same AnalyzerManager as the MDI
            End If

            PrepareButtons()

            'Get the current Language from the current Application Session
            Dim currentLanguageGlobal As New GlobalBase
            CurrentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels
            GetScreenLabels(CurrentLanguage)

            Me.bsRestorepointNameTextBox.BackColor = Color.White
            MyClass.GetRestorePointName()

            CreatingRestorePoint = False

            RestorePointCreated = False

            ErrorMessage = ""

            ''AG 25/10/2011 - Disable ACCEPT process button  on RUNNING or STANDBY/SLEEPING but analyzer working
            ''NOTE in Ax5 the condition was more secure (no connection + (sleeping + no work))
            'If Not AllowStartProcess() Then
            '    bsCreateRestorePointGroupBox.Enabled = False
            '    bsCreateButton.Enabled = False
            'End If
            ''AG 25/10/2011

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadSATReportData_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadSATReportData_Load ", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
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


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 08/10/2010
    ''' </remarks>
    Private Sub bsCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCancelButton.Click
        Try
            'TR 11/04/2012 -Disable form on close to avoid any button press.
            Me.Enabled = False

            'RH 09/03/2012
            If Not Me.Tag Is Nothing Then
                'A PerformClick() method was executed
                Me.Close()
            Else
                'Normal button click
                'Open the WS Monitor form and close this one
                IAx00MainMDI.OpenMonitorForm(Me)
            End If

        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>Created by SGM 30/11/2012</remarks>
    Private Sub bsCreateButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsCreateButton.Click

        Try
            Dim myGlobal As New GlobalDataTO
            Me.bsCreateButton.Enabled = False
            Me.bsCancelButton.Enabled = False
            IAx00MainMDI.EnableButtonAndMenus(False)
            myGlobal = MyClass.CreateRestorePoint()
            If Not myGlobal.HasError Then
                ShowMessage(Me.Name & bsTitle.Text, GlobalEnumerates.Messages.CREATE_RESTOREPOINT_SUCCESS.ToString) 'PENDING MESSAGE
                Me.bsCancelButton_Click(Me, Nothing) 'exit
                Exit Sub
            Else
                ShowMessage(Me.Name & bsTitle.Text, GlobalEnumerates.Messages.CREATE_RESTOREPOINT_ERROR.ToString) 'PENDING MESSAGE
                Me.bsCreateButton.Enabled = True
                Me.bsCancelButton.Enabled = True
            End If
        Catch ex As Exception
            CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCreateButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCreateButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        Finally
            IAx00MainMDI.EnableButtonAndMenus(True)
        End Try
    End Sub

#End Region

   
End Class