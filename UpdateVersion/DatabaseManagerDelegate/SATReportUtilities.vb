Option Explicit On
Option Strict On

'Imports System.Configuration
Imports System.Windows.Forms
Imports System.IO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL

Namespace Biosystems.Ax00.BL.UpdateVersion

    Public Class SATReportUtilities

        ''' <summary>
        ''' Creates a backup of all the database and also the system info file, version and log file
        ''' </summary>
        ''' <param name="pAction"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  SG 13/10/10
        ''' Modified by DL 18/01/2013 Add pUserVersion parameter.
        '''             XB 28/05/2013 Correction : Condition must done by Directory instead of File (bugstranking: # 1139)
        '''                                          PreviousLog.zip is also included into ReportSAT (bugstranking: # 1139)
        '''             DL 31/05/2013 Add option pIncludeZIP (default value TRUE - ag 03/06/2013)
        '''             XB 14/06/2013 Add conditions to protect directory and files creates/copies
        '''             XB 17/06/2013 During process Update : If there are a previous file with the same name, it is deleted, instead of moved into previous folder
        ''' </remarks>
        Public Function CreateSATReport(ByVal pAction As GlobalEnumerates.SATReportActions, _
                                        Optional ByVal pAuto As Boolean = False, _
                                        Optional ByVal ExcelPath As String = "", _
                                        Optional ByVal pAdjFilePath As String = "", _
                                        Optional ByVal pFilePath As String = "", _
                                        Optional ByVal pFileName As String = "", _
                                        Optional ByVal pUserVersion As String = "", _
                                        Optional ByVal pIncludeZIP As Boolean = True) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myUtil As New Utilities
            'dirs
            Dim SATReportDir As String = ""
            Dim SATReportRestoreDir As String = ""
            Dim SystemBackupDir As String = ""
            Dim HistoryBackupDir As String = ""

            'prefixes
            Dim SATReportPrefix As String = ""
            Dim SATReportRestorePrefix As String = ""
            Dim SystemBackupPrefix As String = ""
            Dim HistoryBackupPrefix As String = ""
            Dim ReportFolderPath As String = ""
            Dim myParams As New SwParametersDelegate

            Dim myConnection As SqlClient.SqlConnection = Nothing 'SGM 08/03/11

            Try
                'SGM 08/03/11 Get from DataBase SWParameters
                myGlobal = DAOBase.GetOpenDBConnection(Nothing)
                If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                    myConnection = CType(myGlobal.SetDatos, SqlClient.SqlConnection)

                    'SATReportDir
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.SAT_PATH.ToString, Nothing)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        SATReportDir = Application.StartupPath & CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'SATReportRestoreDir
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.RESTORE_POINT_DIR.ToString, Nothing)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        SATReportRestoreDir = Application.StartupPath & CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'SystemBackupDir
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.SYSTEM_BACKUP_DIR.ToString, Nothing)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        SystemBackupDir = Application.StartupPath & CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'HistoryBackupDir
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.HISTORY_BACKUP_DIR.ToString, Nothing)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        HistoryBackupDir = Application.StartupPath & CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    ' DL 13/05/2011
                    'SATReportPrefix
                    If pAuto Then
                        myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.SAT_REPORT_PREFIX_AUTO.ToString, Nothing)

                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                            SATReportPrefix = CStr(myGlobal.SetDatos)

                            If System.IO.File.Exists(SATReportDir & SATReportPrefix & GlobalBase.ZIPExtension) Then
                                System.IO.File.Delete(SATReportDir & SATReportPrefix & GlobalBase.ZIPExtension)
                            End If
                        Else
                            Throw New Exception
                        End If

                    Else
                        myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.SAT_REPORT_PREFIX.ToString, Nothing)
                        If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                            SATReportPrefix = CStr(myGlobal.SetDatos)
                        Else
                            Throw New Exception
                        End If
                    End If
                    ' End DL 13/05/2011

                    'SATReportRestorePrefix
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.RESTORE_POINT_PREFIX.ToString, Nothing)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        SATReportRestorePrefix = CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'TR 14/03/2011 -SEt the SYSTEM_BACKUP_PREFIX
                    'SystemBackupPrefix 
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.SYSTEM_BACKUP_PREFIX.ToString, Nothing)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        SystemBackupPrefix = CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'HistoryBackupPrefix
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.HISTORY_BACKUP_PREFIX.ToString, Nothing)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        HistoryBackupPrefix = CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                End If

            Catch ex As Exception
                SATReportDir = Application.StartupPath & "\SATReports\"
                SATReportRestoreDir = Application.StartupPath & "\RestorePoints\"
                SystemBackupDir = Application.StartupPath & "\SystemBackups\"
                HistoryBackupDir = Application.StartupPath & "\HistoryBackups\"
                SATReportPrefix = "SATReport"
                SATReportRestorePrefix = "RestorePoint"
                SystemBackupPrefix = "SystemBackup"
                HistoryBackupPrefix = "HistoryBackup"
            End Try

            If myConnection IsNot Nothing Then myConnection.Close()

            Try
                Dim myLogAcciones As New ApplicationLogManager()
                Select Case pAction
                    Case GlobalEnumerates.SATReportActions.SAT_UPDATE_ERR
                        If Not pFilePath = "" AndAlso Not pFileName = "" Then
                            ReportFolderPath = pFilePath & "\" & pFileName
                        Else
                            ReportFolderPath = SATReportDir & SATReportPrefix & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT)
                        End If

                    Case GlobalEnumerates.SATReportActions.SAT_REPORT 'RH 23/12/2010 Introduce GlobalConstants.SAT_DATETIME_FORMAT
                        ' DL 13/05/2011
                        If pAuto Then
                            ReportFolderPath = SATReportDir & SATReportPrefix & pUserVersion
                        Else
                            If Not pFilePath = "" AndAlso Not pFileName = "" Then
                                ReportFolderPath = pFilePath & "\" & pFileName
                            Else
                                ReportFolderPath = SATReportDir & SATReportPrefix & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT)
                            End If
                            'ReportFolderPath = SATReportDir & SATReportPrefix & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT)

                        End If
                        ' END DL 13/05/2011

                    Case GlobalEnumerates.SATReportActions.SAT_RESTORE
                        ReportFolderPath = SATReportRestoreDir & SATReportRestorePrefix & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT) & _
                                                                                                                            " " & Application.ProductVersion

                    Case GlobalEnumerates.SATReportActions.SAT_UPDATE  'DL 18/01/2013. Begin SAT_UPDATE

                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If GlobalBase.IsServiceAssembly Then
                            Dim myPath As String

#If DEBUG Then
                            myPath = Application.StartupPath()

                            Dim splitSQL As String() = Split(myPath, "\")
                            Dim firstItem As Integer = -1


                            If splitSQL.[Select](Function(item, index) New With {
                                    Key .ItemName = item,
                                    Key .Position = index}
                                ).Where(Function(i) i.ItemName = "PresentationSRV").Count > 0 Then

                                firstItem = splitSQL.[Select](Function(item, index) New With {
                                        Key .ItemName = item,
                                        Key .Position = index
                                        }).Where(Function(i) i.ItemName = "PresentationSRV").First().Position
                            End If

                            If firstItem > -1 Then
                                splitSQL(firstItem) = "PresentationUSR"
                                myPath = String.Join("\", splitSQL)

                                'Dim myParams As New SwParametersDelegate

                                myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.RESTORE_POINT_DIR.ToString, Nothing)
                                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                    Dim myRestoreParam As String = CStr(myGlobal.SetDatos)
                                    ReportFolderPath = myPath & myRestoreParam & SATReportRestorePrefix & " End Version " & pUserVersion

                                    Dim myFileName As String = ReportFolderPath & ".SAT"
                                    If File.Exists(myFileName) Then
                                        ' XB - 17/06/2013 - If there are a previous file with the same name, it is deleted instead of moved into previous folder
                                        File.Delete(myFileName)
                                        'Dim myPreviousPath As String = myPath & myRestoreParam & "Previous"
                                        'If Not System.IO.Directory.Exists(myPreviousPath) Then
                                        '    Directory.CreateDirectory(myPreviousPath)
                                        'End If
                                        'ReportFolderPath = myPreviousPath & "\" & SATReportRestorePrefix & " End Version " & pUserVersion
                                        ' XB - 17/06/2013 
                                    End If

                                End If
                            Else
                                'TO DO
                            End If
#Else
                            Dim ficPruebas As String
                            myPath = Application.StartupPath()
                            ficPruebas = Path.Combine(myPath, "..\User Sw")

                            'Dim myParams As New SwParametersDelegate
                            myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.RESTORE_POINT_DIR.ToString, Nothing)
                            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                                Dim myRestoreParam As String = CStr(myGlobal.SetDatos)
                                ReportFolderPath = ficPruebas & myRestoreParam & SATReportRestorePrefix & " End Version " & pUserVersion  

                                Dim myFileName As String = ReportFolderPath & ".SAT"
                                If File.Exists(myFileName) Then
                                    ' XB - 17/06/2013 - If there are a previous file with the same name, it is deleted instead of moved into previous folder
                                    File.Delete(myFileName)
                                    'Dim myPreviousPath As String = ficPruebas & myRestoreParam & "Previous"
                                    'If Not System.IO.Directory.Exists(myPreviousPath) Then
                                    '     Directory.CreateDirectory(myPreviousPath)
                                    'End If
                                    'ReportFolderPath = myPreviousPath & "\" & SATReportRestorePrefix & " End Version " & pUserVersion
                                    ' XB - 17/06/2013 
                                End If
                            End If

#End If
                        Else
                            ReportFolderPath = SATReportRestoreDir & SATReportRestorePrefix & " End Version " & pUserVersion

                            Dim myFileName As String = ReportFolderPath & ".SAT"
                            If File.Exists(myFileName) Then
                                ' XB - 17/06/2013 - If there are a previous file with the same name, it is deleted instead of moved into previous folder
                                File.Delete(myFileName)
                                'Dim myPreviousPath As String = SATReportRestoreDir & "Previous"
                                'If Not System.IO.Directory.Exists(myPreviousPath) Then
                                '    Directory.CreateDirectory(myPreviousPath)
                                'End If
                                'ReportFolderPath = myPreviousPath & "\" & SATReportRestorePrefix & " End Version " & pUserVersion
                                ' XB - 17/06/2013 
                            End If

                        End If
                        'DL 18/01/2013. End SAT_UPDATE

                    Case GlobalEnumerates.SATReportActions.SYSTEM_BACKUP
                        ReportFolderPath = SystemBackupDir & SystemBackupPrefix & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT)
                    Case GlobalEnumerates.SATReportActions.HISTORY_BACKUP
                        ReportFolderPath = HistoryBackupDir & HistoryBackupPrefix & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT)
                End Select

                Dim myDatabaseAdmin As New DataBaseManagerDelegate()

                If pAction = GlobalEnumerates.SATReportActions.SAT_REPORT Then
                    '**************************************************************************************
                    'if it is a report made for sending to Biosystems SAT the patients' data must be hidden
                    '**************************************************************************************
                End If

                If Not myDatabaseAdmin.BackUpDataBaseAndMoveBkFile(DAOBase.DBServer, DAOBase.CurrentDB, _
                                                                   DAOBase.DBLogin, DAOBase.DBPassword, ReportFolderPath) Then
                    'ERROR
                    'Write error SYSTEM_ERROR in the Application Log
                    Dim Message As String = "Unable to create a backup copy of the data base."

                    myLogAcciones.CreateLogActivity(Message, "Utilities.CreateSATReport", EventLogEntryType.Error, False)

                    Dim myDataBaseManager As New DataBaseManagerDelegate
                    Dim IsSQLServer2005 As Boolean = myDataBaseManager.IsSQLServer2005(DAOBase.DBServer, DAOBase.DBLogin, DAOBase.DBPassword)

                    myLogAcciones.CreateLogActivity("IsSQLServer2005 = " & IsSQLServer2005.ToString(), "Utilities.CreateSATReport", EventLogEntryType.Error, False)

                End If

                If pAction = GlobalEnumerates.SATReportActions.HISTORY_BACKUP Then
                    '*******************************************************
                    'the historic data must be cleared after saving backup
                    '*******************************************************
                ElseIf pAction = GlobalEnumerates.SATReportActions.SAT_REPORT Then
                    '*******************************************************
                    'the patient data must be restored after saving backup
                    '*******************************************************
                End If

                'SG 07/10/2010 Add the PC and OS Info File
                'Dim SATReportOSFileNamePath As String = Application.StartupPath & ConfigurationManager.AppSettings("PCOSInfoFilePath") & "\AX00PCOSInfo.txt"

                'TR 25/01/2011 -Replace by corresponding value on global base.
                Dim SATReportOSFileNamePath As String = Application.StartupPath & GlobalBase.PCOSInfoFilePath & "\AX00PCOSInfo.txt"

                Dim PCOSFileName As String = ReportFolderPath & "\AX00PCOSInfo.txt"
                If File.Exists(SATReportOSFileNamePath) Then 'only if exits SGM 25/11/2011
                    If Not Directory.Exists(ReportFolderPath) Then  'only if not exists XBC 14/06/2013
                        Directory.CreateDirectory(ReportFolderPath)
                    End If
                    If Not File.Exists(PCOSFileName) Then   'only if not exists XBC 14/06/2013
                        File.Copy(SATReportOSFileNamePath, PCOSFileName)
                    End If
                End If
                'END SG 07/10/10

                If pUserVersion <> "1.0.0" Then
                    'TR 10/06/2013 -New implementation for saving synapseEvent Log does not need the number of days.
                    myGlobal = myUtil.SaveSynapseEventLog(GlobalBase.SynapseLogFileName, ReportFolderPath)
                End If

                'If File.Exists(Application.StartupPath & GlobalBase.XmlLogFilePath) Then ' XBC+TR 03/10/2012 - Correction
                If Directory.Exists(Application.StartupPath & GlobalBase.XmlLogFilePath) Then ' XB 28/05/2013 - Correction : condition must done by Directory instead of File
                    Dim SATLogFolder As String = Application.StartupPath & GlobalBase.XmlLogFilePath
                    Dim di As New IO.DirectoryInfo(SATLogFolder)
                    Dim diar1 As IO.FileInfo() = di.GetFiles("Ax00Log_*.xml")
                    Dim dra As IO.FileInfo
                    Dim logFileName As String

                    For Each dra In diar1
                        logFileName = ReportFolderPath & "\" & dra.ToString
                        If File.Exists(SATLogFolder & dra.ToString) Then
                            Directory.CreateDirectory(ReportFolderPath)
                            File.Copy(SATLogFolder & dra.ToString, logFileName)
                        End If
                    Next dra

                    'DL 31/05/2013
                    If pIncludeZIP Then
                        ' XB 28/05/2013 - PreviousLog.zip is also included into ReportSAT
                        Dim diar2 As IO.FileInfo() = di.GetFiles("*.zip")
                        For Each dra In diar2
                            logFileName = ReportFolderPath & "\" & dra.ToString
                            If File.Exists(SATLogFolder & dra.ToString) Then
                                If Not Directory.Exists(ReportFolderPath) Then
                                    Directory.CreateDirectory(ReportFolderPath)
                                End If
                                If Not File.Exists(logFileName) Then    'only if not exists XBC 14/06/2013
                                    File.Copy(SATLogFolder & dra.ToString, logFileName)
                                End If
                            End If
                        Next
                    End If
                    ' XB 28/05/2013 

                End If
                'DL 26/01/2012. End

                'AG 28/09/2011 - Adjustments file
                Dim FwAdjustmentsFileNamePath As String = Application.StartupPath & GlobalBase.FwAdjustmentsPath & "FwAdjustments.txt"

                'SGM 25/11/2011 - Get Adjustment File path
                FwAdjustmentsFileNamePath = pAdjFilePath

                Dim FwAdjustmentsFileName As String = ReportFolderPath & "\FWAdjustments.txt"
                Directory.CreateDirectory(ReportFolderPath)
                If File.Exists(FwAdjustmentsFileNamePath) Then 'only if exits SGM 25/11/2011
                    File.Copy(FwAdjustmentsFileNamePath, FwAdjustmentsFileName)
                End If
                'AG 28/09/2011

                'DL 14/06/2011
                If ExcelPath <> "" Then myUtil.CopyFiles(ExcelPath, ReportFolderPath & "\", "*.xls")

                'TR 19/02/2013 
                If pAction = GlobalEnumerates.SATReportActions.SAT_UPDATE_ERR Then
                    'validate if file exist.
                    If File.Exists(Application.StartupPath & GlobalBase.PreviousFolder & GlobalBase.UpdateLogFile) Then
                        'Then add the Update error log file 
                        myUtil.CopyFiles(Application.StartupPath & GlobalBase.PreviousFolder, _
                                         ReportFolderPath & "\", GlobalBase.UpdateLogFile)
                    End If
                End If
                'TR 19/02/2013 END

                ' XBC 25/07/2011 - Add sw service files into SatReport
                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                If GlobalBase.IsServiceAssembly Then

                    Dim FwScriptsFileNamePath As String = Application.StartupPath & GlobalBase.XmlFwScripts
                    Dim FwScriptsFileName As String = ReportFolderPath & "\FwScriptsData.xml"
                    If File.Exists(FwScriptsFileNamePath) Then 'only if exits SGM 25/11/2011
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(FwScriptsFileNamePath, FwScriptsFileName)
                    End If

                    'AG 28/09/2011 - this file is common for the Service and User software
                    'Dim FwAdjustmentsFileNamePath As String = Application.StartupPath & GlobalBase.FwAdjustmentsFile
                    'Dim FwAdjustmentsFileName As String = ReportFolderPath & "\FWAdjustments.txt"
                    'Directory.CreateDirectory(ReportFolderPath)
                    'File.Copy(FwAdjustmentsFileNamePath, FwAdjustmentsFileName)
                    'AG 28/09/2011

                    Dim AnalyzerInfoFileNamePath As String = Application.StartupPath & GlobalBase.AnalyzerInfoFileOut
                    Dim AnalyzerInfoFileName As String = ReportFolderPath & "\AnalyzerInfo.txt"
                    If File.Exists(AnalyzerInfoFileNamePath) Then 'only if exits SGM 25/11/2011
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(AnalyzerInfoFileNamePath, AnalyzerInfoFileName)
                    End If

                    Dim PhotometryTestsFileNamePath As String = Application.StartupPath & GlobalBase.PhotometryTestsFile
                    Dim PhotometryTestsFileName As String = ReportFolderPath & "\PhotometryTests.bin"
                    If File.Exists(PhotometryTestsFileNamePath) Then 'only if exits
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(PhotometryTestsFileNamePath, PhotometryTestsFileName)
                    End If

                    Dim PhotometryBLFileNamePath As String = Application.StartupPath & GlobalBase.PhotometryBLFileOut
                    Dim PhotometryBLFileName As String = ReportFolderPath & "\BLResults.res"
                    If File.Exists(PhotometryBLFileNamePath) Then 'only if exits
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(PhotometryBLFileNamePath, PhotometryBLFileName)
                    End If

                    Dim PhotometryRepeatabilityFileNamePath As String = Application.StartupPath & GlobalBase.PhotometryRepeatabilityFileOut
                    Dim PhotometryRepeatabilityFileName As String = ReportFolderPath & "\RepeatabilityResults.res"
                    If File.Exists(PhotometryRepeatabilityFileNamePath) Then 'only if exits SGM 25/11/2011
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(PhotometryRepeatabilityFileNamePath, PhotometryRepeatabilityFileName)
                    End If

                    Dim PhotometryStabilityFileNamePath As String = Application.StartupPath & GlobalBase.PhotometryStabilityFileOut
                    Dim PhotometryStabilityFileName As String = ReportFolderPath & "\StabilityResults.res"
                    If File.Exists(PhotometryStabilityFileNamePath) Then 'only if exits SGM 25/11/2011
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(PhotometryStabilityFileNamePath, PhotometryStabilityFileName)
                    End If

                End If
                ' XBC 25/07/2011


                'SG 08/10/2010 Version txt file
                'Dim myVersionFileName As String = ConfigurationManager.AppSettings("VersionFileName").ToString()

                'TR 25/01/2011 -Replace by corresponding value on global base.
                Dim myVersionFileName As String = GlobalBase.VersionFileName

                'myGlobal = myUtil.CreateVersionFile(ReportFolderPath & "\" & myVersionFileName)
                myGlobal = myUtil.CreateVersionFile(ReportFolderPath & "\" & myVersionFileName, pAction, pUserVersion)
                'END SG 08/10/10

                'Compress and delete the temporal folder
                myGlobal = myUtil.CompressToZip(ReportFolderPath, ReportFolderPath & GlobalBase.ZIPExtension)
                If Not myGlobal Is Nothing AndAlso Not myGlobal.HasError Then
                    If Directory.Exists(ReportFolderPath) Then
                        Directory.Delete(ReportFolderPath, True)
                    End If
                    myGlobal.SetDatos = True
                End If

            Catch ex As Exception
                If Directory.Exists(ReportFolderPath) Then Directory.Delete(ReportFolderPath, True)

                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SATReportUtilities.CreateSATReport", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDirectoryPath"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 25/11/2011</remarks>
        Public Function GetLastWrittenFile(ByVal pDirectoryPath As String, ByVal pExtension As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Try
                If Directory.Exists(pDirectoryPath) Then

                    Dim myLastWrittenFilePath As String = ""

                    Dim myFiles() As String = System.IO.Directory.GetFiles(pDirectoryPath)
                    Dim myFileCreationDatetimes As New List(Of Date)
                    For F As Integer = 0 To myFiles.Length - 1
                        Dim myFileInfo As FileInfo = New FileInfo(myFiles(F))
                        If myFileInfo.Extension.ToLower = pExtension.ToLower Then
                            myFileCreationDatetimes.Add(myFileInfo.LastWriteTime)
                        End If
                    Next

                    Dim myNewestDate As Date = (From a As Date In myFileCreationDatetimes Order By a Descending Select a).First

                    For F As Integer = 0 To myFiles.Length - 1
                        If New FileInfo(myFiles(F)).LastWriteTime = myNewestDate Then
                            myLastWrittenFilePath = myFiles(F)
                            Exit For
                        End If
                    Next

                    myGlobal.SetDatos = myLastWrittenFilePath
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SATReportUtilities.GetLastWrittenFile", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Gets the SAT Report's version
        ''' </summary>
        ''' <param name="pSATReportFilePath"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 13/10/10</remarks>
        Public Function GetSATReportVersion(ByVal pSATReportFilePath As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myUtil As New Utilities
            Try
                'extract temporaly
                Dim tempFolder As String = Directory.GetParent(pSATReportFilePath).FullName & "\temp"
                myGlobal = myUtil.ExtractFromZip(pSATReportFilePath, tempFolder)
                Dim VersionData As String = String.Empty

                'RH 12/11/2010 Introduce the Using statement
                If Not myGlobal.HasError AndAlso Not myGlobal Is Nothing Then
                    'open the text file
                    Using TextFileReader As New StreamReader(String.Format("{0}\{1}", tempFolder, GlobalBase.VersionFileName)) 'RH 02/02/2011
                        'get the version
                        VersionData = TextFileReader.ReadToEnd.TrimEnd()
                        'Close the file to avoid error. (RH: The using statement takes care of this, but it is OK)
                        TextFileReader.Close()
                    End Using

                    'delete temp folder
                    If Directory.Exists(tempFolder) Then
                        Directory.Delete(tempFolder, True)
                    End If

                    'make the special reading operations if needed

                    myGlobal.SetDatos = VersionData
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SATReportUtilities.GetSATReportVersion", EventLogEntryType.Error, False)

            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Compares the Application's version to the SAT Report's version
        ''' </summary>
        ''' <param name="pAppVersion"></param>
        ''' <param name="pSATReportVersion"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 13/10/10</remarks>
        Public Function CompareSATandAPPversions(ByVal pAppVersion As String, ByVal pSATReportVersion As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myUtil As New Utilities

            Try
                Dim myAppVersion As String() = pAppVersion.Split(CChar("."))
                Dim mySATVersion As String() = pSATReportVersion.Split(CChar("."))

                If myAppVersion.Length > 0 And mySATVersion.Length > 0 And myAppVersion.Length = mySATVersion.Length Then
                    For v As Integer = 0 To myAppVersion.Length - 1
                        Dim app As Integer = CInt(myAppVersion(v))
                        Dim sat As Integer = CInt(mySATVersion(v))
                        If sat <> app Then
                            If sat > app Then
                                myGlobal.SetDatos = GlobalEnumerates.SATReportVersionComparison.UpperThanAPP
                            ElseIf sat < app Then
                                myGlobal.SetDatos = GlobalEnumerates.SATReportVersionComparison.LowerThanAPP
                            End If
                            Exit For
                        End If

                        If myGlobal.SetDatos IsNot Nothing Then
                            Exit For
                        End If
                    Next

                    If myGlobal.SetDatos Is Nothing Then
                        myGlobal.SetDatos = GlobalEnumerates.SATReportVersionComparison.EqualsAPP
                    End If
                Else
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = "MISSING_DATA"
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SATReportUtilities.CompareSATandAPPversions", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Creates a new temporal folder
        ''' </summary>
        ''' <returns>
        ''' A new existing temporal folder in path format "C:\{GUID value}"
        ''' </returns>
        ''' <remarks>
        ''' Created by RH 12/11/2010
        ''' Remove the folder after using it.
        ''' </remarks>
        Public Function GetTempFolder() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Const DirFormat As String = "C:\{0}"

            Try
                Dim NewFolder As String = String.Format(DirFormat, System.Guid.NewGuid)
                While Directory.Exists(NewFolder) 'Is there an existing folder named like this one?
                    NewFolder = String.Format(DirFormat, System.Guid.NewGuid)
                End While
                Directory.CreateDirectory(NewFolder)
                myGlobal.SetDatos = NewFolder

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "SATReportUtilities.CompareSATandAPPversions", EventLogEntryType.Error, False)

            End Try

            Return myGlobal

        End Function

    End Class

End Namespace
