﻿Option Strict On
Option Explicit On
Option Infer On

Imports System.IO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.PresentationCOM
Imports Biosystems.Ax00.Global.TO
'Imports LIS.Biosystems.Ax00.LISCommunications   ' XB 07/05/2013
Imports Biosystems.Ax00.App
Imports Biosystems.Ax00.Framework.CrossCutting
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Threading.Tasks


Public Class UiSATReportLoad
    Inherits Biosystems.Ax00.PresentationCOM.BSBaseForm




#Region "Properties"

    Public Property MainMDI As IMainMDI = New EmptyMainMDI

    Public Property RestorePointMode() As Boolean
        Get
            Return RestorePointModeField
        End Get
        Set(ByVal value As Boolean)
            RestorePointModeField = value
            Me.bsAllowRestoreCheckbox.Visible = Not value
            Me.bsSelectedTextBox.Visible = Not value
            Me.bsBrowseButton.Visible = Not value
            Me.bsSelectlabel.Visible = Not value

            'Me.BsOKButton.Visible = value
            Me.bsSATDirListBox.Visible = value
            Me.bsRestoreLabel.Visible = value

            'If Not value Then
            '    'Me.Size = New Size(442, 183)
            'Else
            '    'Me.Size = New Size(387, 210)
            'End If
        End Set
    End Property

#End Region

#Region "Fields"

    Private RestorePointModeField As Boolean = True

#End Region

#Region "Declarations"

    Private CreatingRestorePoint As Boolean = False
    Private RestoringDB As Boolean = False
    Private UpdatingDB As Boolean = False

    Private SATReportCreated As Boolean = False
    Private DBRestored As Boolean = False
    Private DBUpdated As Boolean = False

    Private ErrorMessage As String

    Private RestorePointPath As String
    Private RestoreDBFilePath As String

    Private CurrentLanguage As String
    'Private mdiAnalyzerCopy As AnalyzerManager '#REFACTORING

#End Region

#Region "Methods"

    ''' <summary>
    ''' Method incharge to load the button image
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 08/10/2010
    ''' Modified by: DL 03/11/2010 - Load Icon in Image Property instead of in BackgroundImage
    ''' </remarks>
    Private Sub PrepareButtons()
        Try
            Dim auxIconName As String = ""
            Dim iconPath As String = MyBase.IconsPath

            'OK Button
            auxIconName = GetIconName("ACCEPT1")
            If (auxIconName <> "") Then
                bsOKButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'CANCEL AcceptButton
            auxIconName = GetIconName("CANCEL")
            If (auxIconName <> "") Then
                bsCancelButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If

            'TR 11/01/2011 -Path Button.
            auxIconName = GetIconName("OPEN")
            If (auxIconName <> "") Then
                bsBrowseButton.Image = ImageUtilities.ImageFromFile(iconPath & auxIconName)
            End If


        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".PrepareButtons", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".PrepareButtons", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load the screen texts in the current application Language
    ''' </summary>
    ''' <param name="pLanguageID">Current Language Identifier</param>
    ''' <remarks>
    ''' Created by: SG 08/10/2010
    ''' </remarks>
    Private Sub GetScreenLabels(ByVal pLanguageID As String)
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'For Labels, CheckBox, RadioButtons...
            If (Not RestorePointModeField) Then
                bsTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSAT_Title", pLanguageID)
                bsSelectlabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FILE_PATH", pLanguageID) + ":"

                bsAllowRestoreCheckbox.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSAT_Restore", pLanguageID)
            Else
                bsTitle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RestorePrevious_Title", pLanguageID)
                bsRestoreLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSAT_SelectRestore", pLanguageID) + ":"
            End If

            'For ToolTips...
            bsScreenToolTips.SetToolTip(bsCancelButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_CancelSelection", pLanguageID))
            bsScreenToolTips.SetToolTip(bsOKButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_AcceptSelection", pLanguageID))
            'TR 11/01/2011 -Path Button.
            bsScreenToolTips.SetToolTip(bsBrowseButton, myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LoadSAT_Select", pLanguageID))

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".GetScreenLabels", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".GetScreenLabels", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Decides about the behavior of the SAT Report loading process depending on the version comparison between the
    ''' SAT Report's version and Application's version
    ''' </summary>
    ''' <param name="pVersionComparison">Result of the version comparison</param>
    ''' <param name="pUnzippedSATFolder">Path of the Unzipped Report SAT Files</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by: SG 08/10/2010
    ''' Modified by: SA 16/05/2014 - BT #1631 ==> When pVersionComparison indicates that SAT Report Version is higher than Application Version,
    '''                                           return REPORT_SAT_VERSION_IS_HIGHER as ErrorCode instead of as ErrorMessage
    '''              IT 08/05/2015 - BA-2471
    ''' </remarks>
    Private Function ManageVersionComparison(ByVal pVersionComparison As GlobalEnumerates.SATReportVersionComparison, ByVal pUnzippedSATFolder As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO
        'Dim myUtil As New Utilities.
        Dim tempFolder As String = ""
        'Dim mySATUtil As New SATReportUtilities

        Try
            'Dim myLangDelegate As New MultilanguageResourcesDelegate
            SATReportCreated = False
            DBRestored = False
            DBUpdated = False
            ErrorMessage = ""

            Select Case pVersionComparison
                'REPORT VERSION > APPLICATION VERSION
                Case GlobalEnumerates.SATReportVersionComparison.UpperThanAPP
                    'show warning message and end process
                    'ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.REPORT_SAT_VERSION_IS_HIGHER.ToString)
                    myGlobal.HasError = True
                    myGlobal.SetDatos = Nothing

                    'BT #1631 - Return REPORT_SAT_VERSION_IS_HIGHER as ErrorCode instead of as ErrorMessage
                    myGlobal.ErrorCode = GlobalEnumerates.Messages.REPORT_SAT_VERSION_IS_HIGHER.ToString()

                    'myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                    'myGlobal.ErrorMessage = GlobalEnumerates.Messages.REPORT_SAT_VERSION_IS_HIGHER.ToString()
                    Return myGlobal

                    'REPORT VERSION <= APPLICATION VERSION
                Case GlobalEnumerates.SATReportVersionComparison.EqualsAPP, GlobalEnumerates.SATReportVersionComparison.LowerThanAPP

                    'its necessary that the SQL Server is installed as a Local Service and is running
                    'Dim SQLServerInstalledAsLocal As Boolean

                    'SQLServerInstalledAsLocal = (PCInfoReader.CheckSQLServerServiceLogon.ToUpper.Trim.StartsWith("LOCAL"))
                    'SQLServerInstalledAsLocal = PCInfoReader.IsSQLServerLocal(DAOBase.DBServer) 'RH 11/11/2010

                    'If Not SQLServerInstalledAsLocal Then
                    '    ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.SAT_SQLSERVER_NOT_LOCAL.ToString)
                    '    Exit Select
                    'End If

                    If Not Me.RestorePointMode AndAlso Me.bsAllowRestoreCheckbox.Checked Then
                        'the user wants to be able to restore his current data in the future
                        Dim SATThread As New Threading.Thread(AddressOf CreateRestorePoint)
                        Me.CreatingRestorePoint = True
                        ScreenWorkingProcess = True 'AG 08/11/2012 - inform this flag because the MDI requires it
                        Application.DoEvents() 'RH 16/11/2010
                        SATThread.Start()

                        MarqueeProgressBarShowingPregoress(Me.CreatingRestorePoint)

                        SATThread.Join()



                        If Not SATReportCreated Then
                            Application.DoEvents()
                            Dim res As DialogResult = ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.SAT_SAVE_RESTORE_POINT_ERROR.ToString)
                            If res = DialogResult.No Then
                                'RH 07/02/2012 Return from the Function.
                                'Avoid the execution of the next code lines.
                                'Log the error
                                GlobalBase.CreateLogActivity(GlobalEnumerates.Messages.SAT_SAVE_RESTORE_POINT_ERROR.ToString(), _
                                                  Me.Name & ".ManageVersionComparison", EventLogEntryType.Error, _
                                                  GetApplicationInfoSession().ActivateSystemLog)

                                myGlobal.HasError = False
                                myGlobal.SetDatos = Nothing
                                'myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                                'myGlobal.ErrorMessage = GlobalEnumerates.Messages.SAT_SAVE_RESTORE_POINT_ERROR.ToString()
                                Return myGlobal
                                'END RH 07/02/2012
                            End If
                        End If
                    End If

                    If Not myGlobal.HasError AndAlso Not myGlobal Is Nothing Then
                        'search for the .bak file
                        Dim myFiles As String() = Directory.GetFiles(pUnzippedSATFolder, "*.bak")
                        If myFiles.Length > 0 Then
                            Me.RestoreDBFilePath = myFiles(0)
                            Dim RestoreThread As New Threading.Thread(AddressOf RestoreDataBase)
                            Me.RestoringDB = True
                            ScreenWorkingProcess = True 'AG 08/11/2012 - inform this flag because the MDI requires it
                            Application.DoEvents() 'RH 16/11/2010

                            RestoreThread.Start()
                            MarqueeProgressBarShowingPregoress(Me.RestoringDB)
                            'MainMDI.InitializeMarqueeProgreesBar()
                            'While Me.RestoringDB 'RH 16/11/2010
                            '    Application.DoEvents()
                            '    Dim waiter = Task.Delay(100)
                            '    waiter.Wait()
                            'End While
                            'MainMDI.StopMarqueeProgressBar()

                            RestoreThread.Join()

                            If Not DBRestored Then
                                ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.SAT_DB_RESTORE_ERROR.ToString)
                                'The DataBase was not restored successfully
                            Else
                                'REPORT VERSION < APPLICATION VERSION
                                If pVersionComparison = GlobalEnumerates.SATReportVersionComparison.LowerThanAPP Then

                                    Dim UpdateThread As New Threading.Thread(AddressOf UpdateDataBase)
                                    Me.UpdatingDB = True
                                    ScreenWorkingProcess = True 'AG 08/11/2012 - inform this flag because the MDI requires it

                                    UpdateThread.Start()
                                    MarqueeProgressBarShowingPregoress(Me.UpdatingDB)
                                    'While Me.UpdatingDB 'RH 16/11/2010
                                    '    MainMDI.InitializeMarqueeProgreesBar()
                                    '    Application.DoEvents()
                                    '    Threading.Thread.Sleep(100)
                                    'End While
                                    'MainMDI.StopMarqueeProgressBar()

                                    UpdateThread.Join()

                                    'If Not DBUpdated Then
                                    '    'ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.SAT_DB_UPDATE_ERROR.ToString) 'TR 29/01/2013 commented
                                    'End If
                                End If

                                Application.DoEvents()

                                If Me.RestorePointMode Then
                                    ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.SAT_LOAD_RESTORE_POINT_OK.ToString)
                                ElseIf DBUpdated Then
                                    ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.SAT_LOAD_REPORT_OK.ToString)
                                End If

                                'AG 11/06/2014 #1661 - After load a RSAT (or restore point) remove all report templates not preloaded those designers do not exists on local computer
                                Dim templateList As New ReportTemplatesDelegate
                                myGlobal = templateList.DeleteNonExistingReportTemplates(Nothing)
                                'AG 11/06/2014 #1661

                                MainMDI.InitializeAnalyzerAndWorkSession(False) 'AG 24/11/2010
                            End If
                        Else
                            If Me.RestorePointMode Then
                                ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.SAT_LOAD_RESTORE_POINT_ERROR.ToString)
                            Else
                                ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.SAT_LOAD_REPORT_ERROR.ToString)
                            End If
                        End If

                        'Else
                    End If
            End Select

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
            myGlobal.ErrorMessage = ex.Message

            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".ManageVersionComparison", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        Finally

            If Directory.Exists(tempFolder) Then DeleteDirectory(tempFolder) 'RH 31/05/2011

            If ErrorMessage <> String.Empty Then
                ShowMessage(Me.Name & ".ManageVersionComparison", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString(), ErrorMessage)
            End If

        End Try

        Return myGlobal
    End Function

    Private Sub MarqueeProgressBarShowingPregoress(ByRef FinishFlag As Boolean)

        MainMDI.InitializeMarqueeProgreesBar()
        While FinishFlag
            Application.DoEvents()
            Dim waiter = Task.Delay(100)
            waiter.Wait()
        End While
        MainMDI.StopMarqueeProgressBar()

    End Sub

    'Function 

    'RH 31/05/2011
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
    ''' Restores the Database with the data contained in a SAT Report or a Restore Point.
    ''' </summary>
    ''' <remarks>
    ''' Created by:  SG 08/10/2010
    ''' Modified by: RH 12/11/2010
    '''              TR 29/01/2013 -Call the updatedatabase process.
    '''              IT 08/05/2015 - BA-2471
    ''' </remarks>
    Private Sub RestoreDataBase()
        Try
            'Restore
            'Me.DBRestored = myDBManager.RestoreDatabaseNEW(DAOBase.CurrentDB, Me.RestoreDBFilePath)new mode with System.Data library (discarded)
            Me.DBRestored = DataBaseManagerDelegate.RestoreDatabase(DAOBase.DBServer, DAOBase.CurrentDB, DAOBase.DBLogin, DAOBase.DBPassword, Me.RestoreDBFilePath) 'BA-2471: IT 08/05/2015

            'TR 29/01/2013 -Call the update process
            UpdateDataBase()
            Me.DBRestored = Me.DBUpdated

            'RH 08/02/2012 Not needed. Here all connections are closed.
            'myDBManager.CloseAllOpenConnection() 'TR 23/12/2011 -Make sure close any open connection from application.

        Catch ex As Exception
            Dim myErrorMessage As String '= ex.Message + " ((" + ex.HResult.ToString + "))" & " - " & ex.InnerException.InnerException.Message
            If ex.InnerException IsNot Nothing AndAlso ex.InnerException.InnerException IsNot Nothing Then
                myErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))" & " - " & ex.InnerException.InnerException.Message
            Else
                myErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"
            End If
            GlobalBase.CreateLogActivity(myErrorMessage, Me.Name & " RestoreDataBase ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ErrorMessage = myErrorMessage

        Finally
            RestoringDB = False
            ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it
            'Cursor.Current = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Updates the Database to the version of the data contained in a SAT Report or a Restore Point.
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 08/10/2010
    ''' Modified by: RH 12/11/2010
    '''              TR 29/01/2013 -Implement the update database update process.
    '''              IT 08/05/2015 - BA-2471
    ''' </remarks>
    Private Sub UpdateDataBase()
        Try
            'UPDATE DATABASE 
            'Cursor.Current = Cursors.WaitCursor
            'Dim myDatabaseMngUpdater As New DataBaseUpdateManagerDelegate
            'TR 04/05/2011 -Commented
            'TODO:Me.DBUpdated = myDBUpdater.UpdateDatabaseSructureAndData(Me.DataServerName, Me.DataBaseName)
            'TR 21/01/2012 -Set true until we implement the update data stucture
            'Me.DBUpdated = True

            'TR 29/01/2013 -Implementation Update process.
            Dim myGlobalDataTO As New GlobalDataTO
            Dim debugLogger As DebugLogger = New DebugLogger()

            If RestorePointMode Then
                debugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
                debugLogger.AddLog(" UPDATE VERSION - RESTORE POINT PROCESS (INI)", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            Else
                debugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
                debugLogger.AddLog(" UPDATE VERSION - SATREPORT PROCESS (INI)", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            End If

            myGlobalDataTO = UpdaterController.Instance.InstallUpdateProcess(DAOBase.DBServer, DAOBase.CurrentDB, DAOBase.DBLogin, DAOBase.DBPassword, True) 'BA-2471: IT 08/05/2015

            If RestorePointMode Then
                debugLogger.AddLog(" UPDATE VERSION - RESTORE POINT PROCESS (END)", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
                debugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            Else
                debugLogger.AddLog(" UPDATE VERSION - SATREPORT PROCESS (END)", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
                debugLogger.AddLog(" --------------------------------------------", GlobalBase.UpdateVersionDatabaseProcessLogFileName)
            End If


            If myGlobalDataTO.HasError Then
                Me.DBUpdated = False
                Dim myMessage As String = "Updating process has been cancelled because some errors have been found. No changes were made on database." & _
                                          Environment.NewLine & _
                                          "You can continue working but is recommended to load the proper restore point."

                MessageBox.Show(myMessage, "BA400 Software", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                Me.DBUpdated = True
            End If
            'TR 29/01/2013 -END

            If Me.DBUpdated Then
                'delete version file
                'Dim myFolder As String = ConfigurationManager.AppSettings("PCOSInfoFilePath").ToString()

                'TR 25/01/2011 -Replace by corresponding value on global base.
                Dim myFolder As String = GlobalBase.PCOSInfoFilePath

                'Dim myVersionFileName As String = ConfigurationManager.AppSettings("VersionFileName").ToString()
                'TR 25/01/2011 -Replace by corresponding value on global base.
                Dim myVersionFileName As String = GlobalBase.VersionFileName

                If File.Exists(myFolder & myVersionFileName) Then
                    File.Delete(myFolder & myVersionFileName)
                End If
            End If

        Catch ex As Exception
            Dim myErrorMessage As String = ex.Message + " ((" + ex.HResult.ToString + "))" & " - " & ex.InnerException.InnerException.Message
            GlobalBase.CreateLogActivity(myErrorMessage, Me.Name & " UpdateDataBase ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ErrorMessage = myErrorMessage

        Finally
            UpdatingDB = False
            ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it
            'Cursor.Current = Cursors.Default

        End Try
    End Sub


    ''' <summary>
    ''' Creates a Restore Point of the current status and data in order not to lose it
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 08/10/2010
    ''' Modified by: RH 12/11/2010
    '''              AG 25/10/2011 - before create a restore point disable ANSINF, once finished enable it
    '''              IT 08/05/2015 - BA-2471
    ''' </remarks>
    Private Sub CreateRestorePoint()
        'Cursor.Current = Cursors.WaitCursor
        Dim myGlobal As GlobalDataTO

        Try

            'Dim mySATUtil As New SATReportUtilities
            Dim Model As String = AnalyzerController.Instance.Analyzer.Model

            myGlobal = SATReportUtilities.CreateSATReport(GlobalEnumerates.SATReportActions.SAT_RESTORE, False, "", AnalyzerController.Instance.Analyzer.AdjustmentsFilePath, "", "", "", True, Model) 'BA-2471: IT 08/05/2015
            If Not myGlobal.HasError AndAlso Not myGlobal Is Nothing Then
                SATReportCreated = CBool(myGlobal.SetDatos)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " CreateRestorePoint ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ErrorMessage = ex.Message

        Finally
            CreatingRestorePoint = False
            ScreenWorkingProcess = False 'AG 08/11/2012 - inform this flag because the MDI requires it
        End Try
    End Sub

    ''' <summary>
    ''' Loads a SAT Report or a Restore Point from the selected path
    ''' </summary>
    ''' <param name="pFilePath"></param>
    ''' <returns></returns>
    ''' <remarks>Created by: SG 08/10/2010
    ''' Modified by: IT 23/10/2014 - REFACTORING (BA-2016)
    '''              IT 08/05/2015 - BA-2471
    ''' </remarks>
    Private Function LoadSATReport(ByVal pFilePath As String) As GlobalDataTO
        Dim myGlobal As New GlobalDataTO

        Dim SATTempFolderPath As String = ""
        Try
            'obtain the APP version
            Dim myAppVersion As String
            myGlobal = Utilities.GetSoftwareVersion()
            If Not myGlobal.HasError And Not myGlobal Is Nothing Then

                myAppVersion = CStr(myGlobal.SetDatos)

                'Extract Data and get extracted folder path
                myGlobal = SATReportUtilities.ExtractZipDataInTempFolder(pFilePath)
                If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                    SATTempFolderPath = myGlobal.SetDatos.ToString
                End If

                If Not myGlobal.HasError And Not myGlobal Is Nothing Then

                    'obtain the SAT Version and Model
                    Dim mySATVersion As String
                    Dim mySATInfo As String()
                    Dim mySATModel As String = String.Empty
                    Dim mySWModel As String = String.Empty
                    myGlobal = SATReportUtilities.GetSATReportVersionAndModel(SATTempFolderPath)

                    If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                        mySATInfo = CStr(myGlobal.SetDatos).Split(System.Convert.ToChar(vbCr))
                        If mySATInfo.Count > 0 Then
                            mySATVersion = mySATInfo(0).Trim
                            'If AnalyzerController.Instance.Analyzer is not already created, we find Model Sw on Database 
                            If AnalyzerController.Instance Is Nothing OrElse AnalyzerController.Instance.Analyzer Is Nothing OrElse String.IsNullOrEmpty(AnalyzerController.Instance.Analyzer.Model) Then
                                mySWModel = Biosystems.Ax00.BL.UpdateVersion.DataBaseUpdateManagerDelegate.GetAnalyzerModel()
                            Else
                                mySWModel = AnalyzerController.Instance.Analyzer.Model
                            End If

                            If mySATInfo.Count > 1 Then
                                mySATModel = mySATInfo(1).Trim

                                Select Case mySWModel
                                    Case AnalyzerModelEnum.A400.ToString
                                        'Check If SAT Model is A200
                                        If mySATModel = AnalyzerModelEnum.A200.ToString Then
                                            'Incompatible Model, Not continue, diferent Models
                                            myGlobal.HasError = True
                                            myGlobal.ErrorCode = GlobalEnumerates.Messages.SAT_LOAD_MODEL_ERROR.ToString
                                            myGlobal.ErrorMessage = mySWModel

                                            GlobalBase.CreateLogActivity(myGlobal.ErrorMessage, Me.Name & " LoadSATReport ", EventLogEntryType.Warning, GetApplicationInfoSession().ActivateSystemLog)

                                            Exit Try
                                        End If
                                    Case AnalyzerModelEnum.A200.ToString
                                        If mySATModel <> mySWModel Then
                                            'Incompatible Model, Not continue, diferent Models
                                            myGlobal.HasError = True
                                            myGlobal.ErrorCode = GlobalEnumerates.Messages.SAT_LOAD_MODEL_ERROR.ToString
                                            myGlobal.ErrorMessage = mySWModel

                                            GlobalBase.CreateLogActivity(myGlobal.ErrorMessage, Me.Name & " LoadSATReport ", EventLogEntryType.Warning, GetApplicationInfoSession().ActivateSystemLog)

                                            Exit Try
                                        End If
                                    Case Else
                                        'If we don't know SW Model (unlikely because always we have a database to find it: normal DB or backup restored DB on software init) we bypass the SAT Model Check
                                End Select
                            Else
                                If mySWModel <> AnalyzerModelEnum.A400.ToString Then
                                    'Incompatible Model, Not continue, diferent Models
                                    myGlobal.HasError = True
                                    myGlobal.ErrorCode = GlobalEnumerates.Messages.SAT_LOAD_MODEL_ERROR.ToString
                                    myGlobal.ErrorMessage = mySWModel
                                    GlobalBase.CreateLogActivity(myGlobal.ErrorMessage, Me.Name & " LoadSATReport ", EventLogEntryType.Warning, GetApplicationInfoSession().ActivateSystemLog)

                                    Exit Try
                                End If
                            End If

                            Dim myComparisonResult As New GlobalEnumerates.SATReportVersionComparison
                            myGlobal = SATReportUtilities.CompareSATandAPPversions(myAppVersion, mySATVersion) 'BA-2471: IT 08/05/2015
                            If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                                myComparisonResult = CType(myGlobal.SetDatos, GlobalEnumerates.SATReportVersionComparison)
                                Me.RestorePointPath = pFilePath

                                'AG 25/10/2011 - Stop ANSINF
                                If (AnalyzerController.IsAnalyzerInstantiated) Then
                                    If AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then
                                        myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STP) 'Stop ANSINF
                                    End If
                                End If
                                'AG 25/10/2011

                                If Not myGlobal.HasError Then
                                    myGlobal = Me.ManageVersionComparison(myComparisonResult, SATTempFolderPath)
                                End If

                                'AG 25/10/2011 - Start ANSINF
                                If Not myGlobal.HasError AndAlso (AnalyzerController.IsAnalyzerInstantiated) Then
                                    If AnalyzerController.Instance.Analyzer.Connected AndAlso AnalyzerController.Instance.Analyzer.AnalyzerStatus <> GlobalEnumerates.AnalyzerManagerStatus.SLEEPING Then
                                        myGlobal = AnalyzerController.Instance.Analyzer.ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.INFO, True, Nothing, GlobalEnumerates.Ax00InfoInstructionModes.STR) 'Start ANSINF
                                    End If

                                End If
                                'AG 25/10/2011

                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & " LoadSATReport ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ErrorMessage = ex.Message
        Finally
            Try
                If Not String.IsNullOrEmpty(SATTempFolderPath) Then
                    If Directory.Exists(SATTempFolderPath) Then
                        System.IO.Directory.Delete(SATTempFolderPath, True)
                    End If
                End If
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)
            End Try
        End Try

        Return myGlobal
    End Function

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
            '#REFACTORING
            If (AnalyzerController.IsAnalyzerInstantiated) Then
                If Not AnalyzerController.Instance.Analyzer.Connected Then 'AG 06/02/2012 - add Connected to the acitvation rule
                    returnValue = True

                    'DL 02/07/2012. Begin
                Else
                    returnValue = False
                    'ElseIf AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.RUNNING _
                    'OrElse (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.STANDBY AndAlso Not AnalyzerController.Instance.Analyzer.AnalyzerIsReady) _
                    'OrElse (AnalyzerController.Instance.Analyzer.AnalyzerStatus = GlobalEnumerates.AnalyzerManagerStatus.SLEEPING AndAlso Not AnalyzerController.Instance.Analyzer.AnalyzerIsReady) Then
                    'returnValue = False
                    'DL 02/07/2012. End 

                    ' XBC 04/07/2012
                    MainMDI.ErrorStatusLabelDisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                    MainMDI.ErrorStatusLabelText = GetMessageText(GlobalEnumerates.Messages.LOADRSAT_NOTALLOWED.ToString)
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
    ''' Created by:  DL 09/11/2010
    ''' </remarks>
    Private Sub LoadSATReportData_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Try
            If (e.KeyCode = Keys.Escape) Then
                'RH 04/07/2011 Escape key should do exactly the same operations as bsCancelButton_Click()
                bsCancelButton.PerformClick()

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadSATReportData_KeyDown ", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".LoadSATReportData_KeyDown", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' Load SAT data
    ''' </summary>
    ''' <remarks>
    ''' Created by  : SG 08/10/2010
    ''' Modified by : XB 07/05/2013 - Instantiate Wrapper LIS into mdiESWrapperCopy 
    '''               XB 17/06/2013 - Also v1.0 restore points must be displayed
    '''               IT 23/10/2014 - REFACTORING (BA-2016)
    ''' </remarks>
    Private Sub LoadSATReportData_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim myLocation As Point
        Dim mySize As Size
        Try
            If Not Me.Parent Is Nothing Then
                'DL 28/07/2011
                myLocation = Me.Parent.Location
                mySize = Me.Parent.Size

                Me.Location = New Point(myLocation.X + CInt((mySize.Width - Me.Width) / 2), myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60)
                'END DL 28/07/2011
            End If



            '  XB 07/05/2013

            PrepareButtons()

            'Get the current Language from the current Application Session
            'Dim currentLanguageGlobal As New GlobalBase
            CurrentLanguage = GlobalBase.GetSessionInfo().ApplicationLanguage.Trim.ToString

            'Load the multilanguage texts for all Screen Labels
            GetScreenLabels(CurrentLanguage)

            CreatingRestorePoint = False
            RestoringDB = False
            UpdatingDB = False

            SATReportCreated = False
            DBRestored = False
            DBUpdated = False
            ErrorMessage = ""

            If Me.RestorePointMode Then
                'Dim myRestoreDataPath As String = Application.StartupPath & "\" & ConfigurationManager.AppSettings("RestorePointDir").ToString()
                'TR 25/01/2011 -Replace by corresponding value on global base.
                Dim myRestoreDataPath As String '= Application.StartupPath & "\" & GlobalBase.RestorePointDir

                'SGM 08/03/11 Get from SWParameters table
                Dim myGlobalDataTO As New GlobalDataTO
                Dim myParams As New SwParametersDelegate
                myGlobalDataTO = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.RESTORE_POINT_DIR.ToString, Nothing)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    myRestoreDataPath = CStr(myGlobalDataTO.SetDatos)
                Else
                    myRestoreDataPath = "\RestorePoints\"
                End If

                'AG 19/04/2011
                If myRestoreDataPath.StartsWith("\") AndAlso Not myRestoreDataPath.StartsWith("\\") Then
                    myRestoreDataPath = Application.StartupPath & myRestoreDataPath
                Else
                    myRestoreDataPath = Application.StartupPath & "\" & myRestoreDataPath
                End If
                'AG 19/04/2011

                'TR 25/01/2011 -Replace by corresponding value on global base.
                Dim myZipExtension As String = GlobalBase.ZIPExtension

                'TR 14/06/2013 -Get installed application current version to filter RP Equal or lower than the application installed.
                Dim myInstalledAppVersion As Integer = CInt(Application.ProductVersion.Replace(CChar("."), ""))

                If Not Directory.Exists(myRestoreDataPath) Then Directory.CreateDirectory(myRestoreDataPath)

                Dim DirList As String()

                If AnalyzerController.Instance.Analyzer Is Nothing OrElse String.IsNullOrEmpty(AnalyzerController.Instance.Analyzer.Model) Then
                    DirList = Directory.GetFiles(myRestoreDataPath, "*" & myZipExtension).Union(Directory.GetFiles(myRestoreDataPath, "*.*")).ToArray()
                Else
                    DirList = Directory.GetFiles(myRestoreDataPath, "*" & myZipExtension).Union(Directory.GetFiles(myRestoreDataPath, "*." & AnalyzerController.Instance.Analyzer.Model)).ToArray()
                End If


                Dim TemporalFileVersion As String = String.Empty
                For i As Integer = 0 To DirList.Count - 1
                    'Validate if belong to the current version
                    Dim myRestorePoint As String = DirList(i).Substring(DirList(i).LastIndexOf("\") + 1)

                    If myRestorePoint.Contains(Application.ProductVersion) Then
                        'Insert
                        bsSATDirListBox.Items.Add(myRestorePoint)

                    ElseIf myRestorePoint.Contains(GlobalBase.UpdateVersionRestorePointName) Then 'Validate the endvesion files
                        TemporalFileVersion = myRestorePoint.Replace(GlobalBase.UpdateVersionRestorePointName, "")
                        'TemporalFileVersion = TemporalFileVersion.Replace(".SAT", "").Trim()
                        TemporalFileVersion = Path.GetFileNameWithoutExtension(TemporalFileVersion)
                        TemporalFileVersion = TemporalFileVersion.Replace(CChar("."), "")
                        If IsNumeric(TemporalFileVersion) Then
                            If CInt(TemporalFileVersion) <= myInstalledAppVersion Then
                                bsSATDirListBox.Items.Add(myRestorePoint)
                            End If
                        End If
                        ' XB 17/06/2013 - Also v1.0 restore points are displayed
                    ElseIf myRestorePoint.Length = 33 Then
                        'Insert
                        'bsSATDirListBox.Items.Add(myRestorePoint.Replace(myZipExtension, String.Empty))
                        bsSATDirListBox.Items.Add(myRestorePoint)
                        ' XB 17/06/2013 
                    End If

                    TemporalFileVersion = ""
                Next
            End If

            'AG 25/10/2011 - Disable ACCEPT process button  on RUNNING or STANDBY/SLEEPING but analyzer working
            'NOTE in Ax5 the condition was more secure (no connection + (sleeping + no work))
            If Not AllowStartProcess() Then
                bsLoadRestoreSATGroupBox.Enabled = False
                bsOKButton.Enabled = False
            End If
            'AG 25/10/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadSATReportData_Load", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
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
            Dim myLocation As Point
            Dim mySize As Size

            If Not Me.Parent Is Nothing Then
                myLocation = Me.Parent.Location
                mySize = Me.Parent.Size
                pos.x = myLocation.X + CInt((mySize.Width - Me.Width) / 2)
                pos.y = myLocation.Y + CInt((mySize.Height - Me.Height) / 2) - 60
                Runtime.InteropServices.Marshal.StructureToPtr(pos, m.LParam, True)
            End If

        End If

        MyBase.WndProc(m)

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 08/10/2010
    ''' Modified by: RH 12/11/2010 Get zipExtension from configuration file
    '''              PG 11/01/2011 Eliminate Dialog's Title 
    ''' </remarks>
    Private Sub BrowseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsBrowseButton.Click
        Try
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Dim zipExtension As String = ConfigurationManager.AppSettings("ZIPExtension").ToString()

            'TR 25/01/2011 -Replace by corresponding value on global base.
            Dim zipExtension As String = GlobalBase.ZIPExtension
            Dim myOpenDlg As New OpenFileDialog

            With myOpenDlg
                '.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.Desktop
                .InitialDirectory = GetSATReportDirectory()
                'PG 11/01/2011
                '.Filter = "SAT Report files|*" & zipExtension & "|All files|*.*"
                '.Filter = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SATReport_File", CurrentLanguage) & "|*" & zipExtension & "|" & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SATReport_FilesAll", CurrentLanguage) & "|*.*"
                'Take Model extensio or All files
                .Filter = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SATReport_File", CurrentLanguage) & " (*." & AnalyzerController.Instance.Analyzer.Model & ")|*." & AnalyzerController.Instance.Analyzer.Model _
                          & "|" & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AllFiles", CurrentLanguage) & " (*.*)|*.*"
                .FilterIndex = 0
                .DefaultExt = "." & AnalyzerController.Instance.Analyzer.Model
                .CheckFileExists = True
                .CheckPathExists = True
                .Multiselect = False
                '.Title = "Load SAT Report file"
                .Title = " "
            End With

            Dim myResponse As DialogResult = myOpenDlg.ShowDialog()

            If myResponse <> Windows.Forms.DialogResult.Cancel Then
                Me.bsSelectedTextBox.Text = myOpenDlg.FileName
                'AG 25/10/2011 - Disable ACCEPT process button  on RUNNING or STANDBY/SLEEPING but analyzer working
                'NOTE in Ax5 the condition was more secure (no connection + (sleeping + no work))
                'Me.bsOKButton.Enabled = True
                If AllowStartProcess() Then
                    bsOKButton.Enabled = True
                End If
                'AG 25/10/2011

            Else
                Me.bsSelectedTextBox.Text = ""
                Me.bsOKButton.Enabled = False
            End If

            bsScreenToolTips.SetToolTip(bsSelectedTextBox, bsSelectedTextBox.Text)

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".BrowseButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".BrowseButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' Created by: SG 08/10/2010
    ''' Modified by: XB 07/05/2013 - Release LIS channels in order to recreate LIS channel according with the settings configured on ReportSAT just loaded
    '''              TR 24/05/2013 - Validate the LIS trace level. If the value saved on the DB is the same the machine has configure on the registry.
    '''              XB 17/06/2013 - No restore points are deleted at v2.0. In the future we design a screen to clean them
    '''              SA 15/05/2014 - BT #1617 ==> When Property RestorePointMode = TRUE, the full path of the Restore Point file has an error (a double slash).  
    '''                                           When function LoadSATReport returns an error, it is not shown because function ShowMessage is called with 
    '''                                           parameters in wrong order; besides, the function does not return the correct ErrorCode. Changes: removed the 
    '''                                           double slash + replace the ErrorCode for SAT_LOAD_RESTORE_POINT_ERROR, which is the specific for this screen +
    '''                                           call function ShowMessage with parameters in the correct order
    ''' </remarks>
    Private Sub bsOKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsOKButton.Click
        ' XBC 04/07/2012 - Indicate Load Rsat START on Application LOG.
        Dim StartTime As DateTime = Now
        Try

            Cursor = Cursors.WaitCursor
            bsOKButton.Enabled = False
            bsCancelButton.Enabled = False
            bsBrowseButton.Enabled = False
            'IAx00MainMDI.EnableMenusBar(False)
            MainMDI.EnableButtonAndMenus(False) 'TR 04/10/2011 -Implement new method.
            'Dim myZipExtension As String = ConfigurationManager.AppSettings("ZIPExtension").ToString

            'TR 25/01/2011 -Replace by corresponding value on global base.
            Dim myZipExtension As String = GlobalBase.ZIPExtension

            MainMDI.SetActionButtonsEnableProperty(False) 'AG 12/07/2011 - Disable all vertical action buttons bar

            Dim myGlobal As GlobalDataTO

            If (Not Me.RestorePointMode) Then
                myGlobal = LoadSATReport(Me.bsSelectedTextBox.Text.Trim)
                If myGlobal.HasError Then
                    Cursor = Cursors.Default
                    'BT #1631 - Replace the ErrorCode returned for the ErrorCode specific for Restore Error; 
                    '           call ShowMessage with parameters in the correct order
                    'ShowMessage(myGlobal.ErrorCode, myGlobal.ErrorMessage)
                    ShowMessage(Me.Name, myGlobal.ErrorCode, myGlobal.ErrorMessage)
                Else
                    'DL 31/05/2013
                    Dim myLogMaxDays As Integer = 30

                    myGlobal = SwParametersDelegate.ReadNumValueByParameterName(Nothing, GlobalEnumerates.SwParameters.MAX_DAYS_IN_PREVIOUSLOG.ToString(), Nothing)
                    If Not myGlobal.HasError Then myLogMaxDays = CInt(myGlobal.SetDatos)
                    'DL 31/05/2013

                    'If not error found the export log file and clean application log table.
                    'TR 31/08/2012 -Export the log information saved on DB to Xml file.
                    'Dim myLogAcciones As New ApplicationLogManager()
                    myGlobal = ApplicationLogManager.ExportLogToXml(AnalyzerController.Instance.Analyzer.ActiveWorkSession, myLogMaxDays) '#REFACTORING
                    'If expor to xml OK then delete all records on Application log Table
                    If (Not myGlobal.HasError) Then
                        myGlobal = ApplicationLogManager.DeleteAll()
                    Else
                        'DL 31/05/2013
                        'The Reset process will continue even if errors in ExportLogToXML
                        myGlobal.HasError = False
                        'DL 31/05/2013
                    End If
                    'TR 31/082012 -END.

                End If

            Else
                'AG 16/11/2010
                'myGlobal = LoadSATReport(Application.StartupPath & "\" & ConfigurationManager.AppSettings("RestorePointDir").ToString() & Me.bsSATDirListBox.SelectedItem.ToString & myZipExtension)

                'SGM 08/03/11 Get from SWParameters table
                Dim myRestoreDataPath As String
                Dim myParams As New SwParametersDelegate
                myGlobal = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.RESTORE_POINT_DIR.ToString, Nothing)
                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                    myRestoreDataPath = CStr(myGlobal.SetDatos)
                Else
                    myRestoreDataPath = "\RestorePoints\"
                End If

                'BT #1631 - The path is wrong. The "\" added before myRestoreDataPath produces a double slash due to the value in myRestoreDataPath contains a slash on the left
                Dim myRestoringFile As String = Application.StartupPath & myRestoreDataPath & Me.bsSATDirListBox.SelectedItem.ToString ' & myZipExtension
                'Dim myRestoringFile As String = Application.StartupPath & "\" & myRestoreDataPath & Me.bsSATDirListBox.SelectedItem.ToString & myZipExtension

                myGlobal = LoadSATReport(myRestoringFile)
                'END AG 16/11/2010

                If (myGlobal.HasError) Then
                    Cursor = Cursors.Default

                    'BT #1631 - Replace the ErrorCode returned for the ErrorCode specific for Restore Error; 
                    '           call ShowMessage with parameters in the correct order
                    If (myGlobal.ErrorCode = "MISSING_DATA") Then myGlobal.ErrorCode = "SAT_LOAD_RESTORE_POINT_ERROR"
                    ShowMessage(Me.Name, myGlobal.ErrorCode, myGlobal.ErrorMessage)
                    'ShowMessage(myGlobal.ErrorCode, myGlobal.ErrorMessage)

                Else ' If process finished successfully then delete the restore point!
                    ' XB - 17/06/2013 - No restore points are deleted at v2.0. In the future we design a screen to clean them
                    'If File.Exists(myRestoringFile) Then File.Delete(myRestoringFile) 'AG 16/11/2010
                    ' XB - 17/06/2013
                End If
            End If

            If Not myGlobal.HasError Then
                'TR 08/02/2012 -After loading the RSAT make sure all information are loaded with the RSAT Values.
                myGlobal = LoadRSATConfig()

                If Not myGlobal.HasError Then
                    'TR 24/05/2013 
                    Dim DBLISTraceValue As String = String.Empty
                    Dim RegistryLISTraceValue As String = String.Empty
                    'Get the database information 
                    Dim myUserSettingsDelegate As New UserSettingsDelegate
                    myGlobal = myUserSettingsDelegate.GetCurrentValueBySettingID(Nothing, UserSettingsEnum.LIS_TRACE_LEVEL.ToString())
                    If Not myGlobal.HasError Then
                        DBLISTraceValue = myGlobal.SetDatos.ToString()
                    End If

                    'Get the registry information 
                    'Dim Utilities As New Utilities
                    myGlobal = Utilities.GetLISTraceLevel()
                    If Not myGlobal.HasError Then
                        RegistryLISTraceValue = myGlobal.SetDatos.ToString()
                    End If

                    'Compare the values
                    If Not (RegistryLISTraceValue = DBLISTraceValue) Then
                        'Save the registry value on the DB.
                        myGlobal = myUserSettingsDelegate.Update(Nothing, UserSettingsEnum.LIS_TRACE_LEVEL.ToString(), RegistryLISTraceValue)
                        'Inform on the application log the change and the previous value.
                        GlobalBase.CreateLogActivity("RSAT LIS Trace value change From -->" & DBLISTraceValue & " To --> " & RegistryLISTraceValue, "LOAD RSAT", EventLogEntryType.Information, False)
                    End If
                End If
                'TR 24/05/2013 -END.

                ' XB 07/05/2013
                Cursor = Cursors.WaitCursor

                MainMDI.ReleaseLIS()

                Cursor = Cursors.Default
                ' XB 07/05/2013
            End If

            If Not myGlobal.HasError Then
                'RH 09/01/2012 Load Reports Default Portrait Template
                If Not XRManager.LoadDefaultPortraitTemplate() Then
                    'No Reports template has been loaded.
                    'Take the proper action!
                End If

                'RH 09/01/2012 Load Reports Default Landscape Template
                If Not XRManager.LoadDefaultLandscapeTemplate() Then
                    'No Reports template has been loaded.
                    'Take the proper action!
                End If

                'RH 07/02/2012 Insert a REPORTSAT_LOADED_WARN alarm in DB
                Dim myWSAnalyzerAlarmDS As New WSAnalyzerAlarmsDS
                Dim myActiveAnalyzerModel As String = String.Empty
                Dim myActiveWorkSession As String = String.Empty

                If AnalyzerController.Instance Is Nothing OrElse AnalyzerController.Instance.Analyzer Is Nothing OrElse String.IsNullOrEmpty(AnalyzerController.Instance.Analyzer.Model) Then
                    myActiveAnalyzerModel = Biosystems.Ax00.BL.UpdateVersion.DataBaseUpdateManagerDelegate.GetAnalyzerModel()
                Else
                    myActiveAnalyzerModel = AnalyzerController.Instance.Analyzer.Model
                    myActiveWorkSession = AnalyzerController.Instance.Analyzer.ActiveWorkSession
                End If

                myWSAnalyzerAlarmDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow( _
                        Alarms.REPORTSATLOADED_WARN.ToString(), myActiveAnalyzerModel, _
                        DateTime.Now, 1, myActiveWorkSession, Nothing, True, Nothing) 'AG 24/07/2012 - This alarm is created with status TRUE not false as before 'False, DateTime.Now)

                Dim myWSAlarmDelegate As New WSAnalyzerAlarmsDelegate
                myGlobal = myWSAlarmDelegate.Create(Nothing, myWSAnalyzerAlarmDS) 'AG 24/07/2012 - keep using Create, do not use Save, it is not necessary
                'END RH 07/02/2012
            End If

            'SGM 03/05/2012
            '#REFACTORING
            If AnalyzerController.Instance IsNot Nothing AndAlso AnalyzerController.Instance.Analyzer IsNot Nothing AndAlso AnalyzerController.Instance.Analyzer.ISEAnalyzer IsNot Nothing Then
                AnalyzerController.Instance.Analyzer.ISEAnalyzer.RefreshAllDatabaseInformation()
            End If

            'RH 07/02/2012
            'Open the WS Monitor form and close this one
            MainMDI.OpenMonitorForm(Me)

            '' XBC 26/06/2012
            'IAx00MainMDI.Connect()

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsOKButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            Cursor = Cursors.Default
            ShowMessage(Me.Name & ".bsOKButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)

        Finally
            Cursor = Cursors.Default
            MainMDI.SetActionButtonsEnableProperty(True) 'AG 12/07/2011 - Enable vertical action buttons bar
            'IAx00MainMDI.EnableMenusBar(True) 'TR 02/08/2011
            MainMDI.EnableButtonAndMenus(True) 'TR 04/10/2011 -Implement new method.

            ' XBC 04/07/2012 - Indicate Load Rsat END on Application LOG.
            GlobalBase.CreateLogActivity("LOAD RSAT END - Start Time: " & StartTime.ToLongTimeString, Name & ".bsOKButton_Click", _
                                       EventLogEntryType.Information, GetApplicationInfoSession().ActivateSystemLog)
            'MainMDI.CloseForm(Me)
        End Try
    End Sub


    ''' <summary>
    '''Reload the application session manager with the information loaded on the Report Sat.
    ''' Set the Language loaded, initialize the application Info session. 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>CREATED BY: TR 08/02/2012</remarks>
    Private Function LoadRSATConfig() As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            'Dim myUserSettingsDS As New UserSettingDS
            Dim myUserSettings As New UserSettingsDelegate
            'Get the language from loaded reportsat
            myGlobalDataTO = myUserSettings.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE.ToString)

            If Not myGlobalDataTO.HasError Then
                Dim newLanguageID As String = myGlobalDataTO.SetDatos.ToString()
                'Dim currentSession As New GlobalBase
                Dim myApplicationInfoSessionTO As New ApplicationInfoSessionTO
                myApplicationInfoSessionTO = GlobalBase.GetSessionInfo()
                'Validate if language is diferent than the current use languae.
                If Not myApplicationInfoSessionTO.ApplicationLanguage = newLanguageID Then
                    'Initialize session information.
                    Dim resetStatus As Boolean = False
                    Dim myApplicationSessionManager As New ApplicationSessionManager

                    If (myApplicationSessionManager.SessionExist) Then
                        resetStatus = ResetApplicationInfoSession()
                        If (resetStatus) Then
                            InitializeApplicationInfoSession(myApplicationInfoSessionTO.UserName.Trim(), myApplicationInfoSessionTO.UserLevel.Trim, newLanguageID)
                        Else
                            ShowMessage(Me.Name & ".SetApplicationSessionInfo", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, , Me)
                        End If

                    Else
                        InitializeApplicationInfoSession(myApplicationInfoSessionTO.UserName.Trim(), myApplicationInfoSessionTO.UserLevel.Trim, newLanguageID)
                    End If

                    'RH 22/02/2012 Update this first
                    MultilanguageResourcesDelegate.SetCurrentLanguage(newLanguageID)

                    'Set the new language to the MDI form to reload menus in new language.
                    MainMDI.CurrentLanguage = newLanguageID
                End If

            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".LoadRSATConfig", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            Cursor = Cursors.Default
            ShowMessage(Me.Name & ".LoadRSATConfig", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try

        Return myGlobalDataTO

    End Function

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
                MainMDI.OpenMonitorForm(Me)
            End If

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsCancelButton_Click", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsCancelButton_Click", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>Created by: SG 08/10/2010</remarks>
    Private Sub bsSATDirListBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bsSATDirListBox.SelectedIndexChanged
        Try
            'AG 25/10/2011 - Disable ACCEPT process button  on RUNNING or STANDBY/SLEEPING but analyzer working
            'NOTE in Ax5 the condition was more secure (no connection + (sleeping + no work))
            'Me.bsOKButton.Enabled = True
            If AllowStartProcess() Then
                bsOKButton.Enabled = True
            End If
            'AG 25/10/2011

        Catch ex As Exception
            GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", Me.Name & ".bsSATDirListBox_SelectedIndexChanged", EventLogEntryType.Error, GetApplicationInfoSession().ActivateSystemLog)
            ShowMessage(Me.Name & ".bsSATDirListBox_SelectedIndexChanged", GlobalEnumerates.Messages.SYSTEM_ERROR.ToString, ex.Message + " ((" + ex.HResult.ToString + "))", Me)
        End Try
    End Sub
#End Region

End Class
