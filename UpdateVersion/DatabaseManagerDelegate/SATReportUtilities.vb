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
        ''' <param name="pAction">Type of process from which the function has been called. Possible values:
        '''                         ** SAT_REPORT     --> Generate Report SAT from User or Service SW, and also automatic generation from Reset WS process
        '''                         ** SAT_RESTORE    --> Create a Restore Point
        '''                         ** SAT_UPDATE     --> Application Update process
        '''                         ** SAT_UPDATE_ERR --> Application Update process finished with Error</param>
        ''' <param name="pAuto">Optional parameter. TRUE when the function is called from Reset WS process to generate an automatic RSAT</param>
        ''' <param name="ExcelPath">Optional parameter. NOT USED: always informed as an empty String</param>
        ''' <param name="pAdjFilePath">Optional parameter. Path for the FW Adjustments file</param>
        ''' <param name="pFilePath">Optional parameter. Path for the Report SAT file</param>
        ''' <param name="pFileName">Optional parameter. Name for the Report SAT file</param>
        ''' <param name="pUserVersion">Optional parameter. User SW Version. Informed only when pAction is SAT_UPDATE or SAT_UPDATE_ERROR</param>
        ''' <param name="pIncludeZIP">Optional parameter. NOT USED: never informed. The default value is used (TRUE)</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if the RSAT has been created (TRUE) or there has been an error (FALSE)</returns>
        ''' <remarks>
        ''' Created by:  SG 13/10/2010
        ''' Modified by: TR 25/01/2011 - Get the name of the txt file from the Global Setting VersionFileName
        '''              SG 08/03/2011 - Get from DB all SW Parameters needed for the Create RSAT process
        '''              TR 14/03/2011 - Get from DB the value of the SW Parameter SYSTEM_BACKUP_PREFIX
        '''              DL 13/05/2011 - Get from DB the value of the SW Parameter for the RSAT Prefix depending on value of pAuto parameter:
        '''                               ** When pAuto = TRUE (RSAT automatically generated during the Reset WS), get value of SW Parameter SAT_REPORT_PREFIX_AUTO
        '''                               ** When pAuto = FALSE, get value of SW Parameter SAT_REPORT_PREFIX
        '''                            - Get the full path of the RSAT file depending on value of pAuto parameter 
        '''              AG 28/09/2011 - Added the Adjustments file to the RSAT
        '''              SG 01/02/2012 - BT #1112 ==> Check if the function has been called from the Service SW using the Service Assembly
        '''              DL 18/01/2013 - Added new optional parameter pUserVersion.
        '''              TR 19/02/2013 - If the Application Update process finishes with error (pAction = SAT_UPDATE_ERROR), the process Log is added to the RSAT
        '''              XB 28/05/2013 - BT #1139 ==> Correction: when adding Log files to the RSAT, validation must done by Directory instead of by File. 
        '''                                           PreviousLog.zip is also included into RSAT 
        '''              DL 31/05/2013 - Added new optional parameter pIncludeZIP (default value = TRUE)
        '''              TR 10/06/2013 - Added code to include the Synapse Event Log in the RSAT file
        '''              XB 14/06/2013 - Added conditions to protect directory and files creates/copies
        '''              XB 17/06/2013 - During process of Application Update: if there is a previous file with the same name, it is deleted, 
        '''                              instead of moved it into previous folder
        '''              SA 12/05/2014 - BT #1617 ==> Changed the call to function Utilities.CreateVersionFile to inform the new boolean parameter pForRSATFromServiceSW 
        '''                                           in the following way:
        '''                                            ** When CreateSATReport is called from UserSW, inform the new parameter as FALSE
        '''                                            ** When CreateSATReport is called from ServiceSW, inform the new parameter as TRUE
        '''                                           To known if the RSAT has been launched from UserSW or from ServiceSW, inform the optional parameter with value of 
        '''                                           the GlobalBase property IsServiceAssembly
        '''              SA 20/10/2014 - BA-1944 ==> Include in the RSAT the last XML File generated for the Update Version process
        ''' </remarks>
        Public Function CreateSATReport(ByVal pAction As GlobalEnumerates.SATReportActions, Optional ByVal pAuto As Boolean = False, _
                                        Optional ByVal ExcelPath As String = "", Optional ByVal pAdjFilePath As String = "", _
                                        Optional ByVal pFilePath As String = "", Optional ByVal pFileName As String = "", _
                                        Optional ByVal pUserVersion As String = "", Optional ByVal pIncludeZIP As Boolean = True) As GlobalDataTO
            Dim myUtil As New Utilities
            Dim myGlobal As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            Dim ReportFolderPath As String = String.Empty
            Dim myConnection As SqlClient.SqlConnection = Nothing

            'Local variables for Directories
            Dim SATReportDir As String = String.Empty
            Dim SATReportRestoreDir As String = String.Empty
            Dim SystemBackupDir As String = String.Empty
            Dim HistoryBackupDir As String = String.Empty

            'Local variables for Prefixes
            Dim SATReportPrefix As String = String.Empty
            Dim SATReportRestorePrefix As String = String.Empty
            Dim SystemBackupPrefix As String = String.Empty
            Dim HistoryBackupPrefix As String = String.Empty

            '******************************************************************************
            '* BLOCK 1 - Get from DB all SW Parameters needed for the Create RSAT process *
            '******************************************************************************
            Try
                myGlobal = DAOBase.GetOpenDBConnection(Nothing)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    myConnection = DirectCast(myGlobal.SetDatos, SqlClient.SqlConnection)

                    'SATReportDir
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.SAT_PATH.ToString, Nothing)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        SATReportDir = Application.StartupPath & CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'SATReportRestoreDir
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.RESTORE_POINT_DIR.ToString, Nothing)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        SATReportRestoreDir = Application.StartupPath & CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'SystemBackupDir
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.SYSTEM_BACKUP_DIR.ToString, Nothing)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        SystemBackupDir = Application.StartupPath & CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'HistoryBackupDir
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.HISTORY_BACKUP_DIR.ToString, Nothing)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        HistoryBackupDir = Application.StartupPath & CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'Get from DB the value of the SW Parameter for the RSAT Prefix depending on value of pAuto parameter
                    If (pAuto) Then
                        'SATReportPrefix
                        myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.SAT_REPORT_PREFIX_AUTO.ToString, Nothing)
                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            SATReportPrefix = CStr(myGlobal.SetDatos)

                            If (System.IO.File.Exists(SATReportDir & SATReportPrefix & GlobalBase.ZIPExtension)) Then
                                System.IO.File.Delete(SATReportDir & SATReportPrefix & GlobalBase.ZIPExtension)
                            End If
                        Else
                            Throw New Exception
                        End If
                    Else
                        'SATReportPrefix
                        myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.SAT_REPORT_PREFIX.ToString, Nothing)
                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            SATReportPrefix = CStr(myGlobal.SetDatos)
                        Else
                            Throw New Exception
                        End If
                    End If

                    'SATReportRestorePrefix
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.RESTORE_POINT_PREFIX.ToString, Nothing)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        SATReportRestorePrefix = CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'SystemBackupPrefix 
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.SYSTEM_BACKUP_PREFIX.ToString, Nothing)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        SystemBackupPrefix = CStr(myGlobal.SetDatos)
                    Else
                        Throw New Exception
                    End If

                    'HistoryBackupPrefix
                    myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.HISTORY_BACKUP_PREFIX.ToString, Nothing)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
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
            If (Not myConnection Is Nothing) Then myConnection.Close()


            '********************************************************************************************************************
            '* BLOCK 2 - Create all needed Directories, prepare all files to include in the RSAT and finally compress all files *
            '********************************************************************************************************************
            Try
                '2.1 - Create the full path for the Report SAT
                Select Case (pAction)
                    Case GlobalEnumerates.SATReportActions.SAT_REPORT
                        'Get the full path for the RSAT file depending on value of pAuto parameter
                        If (pAuto) Then
                            ReportFolderPath = SATReportDir & SATReportPrefix & pUserVersion
                        Else
                            If (Not pFilePath = String.Empty AndAlso Not pFileName = String.Empty) Then
                                ReportFolderPath = pFilePath & "\" & pFileName
                            Else
                                ReportFolderPath = SATReportDir & SATReportPrefix & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT)
                            End If
                        End If

                    Case GlobalEnumerates.SATReportActions.SAT_RESTORE
                        ReportFolderPath = SATReportRestoreDir & SATReportRestorePrefix & _
                                           DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT) & " " & Application.ProductVersion

                    Case GlobalEnumerates.SATReportActions.SYSTEM_BACKUP
                        ReportFolderPath = SystemBackupDir & SystemBackupPrefix & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT)

                    Case GlobalEnumerates.SATReportActions.HISTORY_BACKUP
                        ReportFolderPath = HistoryBackupDir & HistoryBackupPrefix & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT)

                    Case GlobalEnumerates.SATReportActions.SAT_UPDATE
                        If (GlobalBase.IsServiceAssembly) Then
                            'Function has been called from Service SW
                            Dim myPath As String
#If DEBUG Then
                            myPath = Application.StartupPath()

                            Dim firstItem As Integer = -1
                            Dim splitSQL As String() = Split(myPath, "\")

                            If (splitSQL.[Select](Function(item, index) New With {Key .ItemName = item, Key .Position = index}).Where(Function(i) i.ItemName = "PresentationSRV").Count > 0) Then
                                firstItem = splitSQL.[Select](Function(item, index) New With {Key .ItemName = item, Key .Position = index}).Where(Function(i) i.ItemName = "PresentationSRV").First().Position
                            End If

                            If (firstItem > -1) Then
                                splitSQL(firstItem) = "PresentationUSR"
                                myPath = String.Join("\", splitSQL)

                                myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.RESTORE_POINT_DIR.ToString, Nothing)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    Dim myRestoreParam As String = CStr(myGlobal.SetDatos)
                                    ReportFolderPath = myPath & myRestoreParam & SATReportRestorePrefix & " End Version " & pUserVersion

                                    Dim myFileName As String = ReportFolderPath & ".SAT"
                                    If (File.Exists(myFileName)) Then
                                        'If there are a previous file with the same name, it is deleted instead of moved into previous folder
                                        File.Delete(myFileName)
                                    End If
                                End If
                            Else
                                'TO DO
                            End If
#Else
                            Dim ficPruebas As String
                            myPath = Application.StartupPath()
                            ficPruebas = Path.Combine(myPath, "..\User Sw")

                            myGlobal = myParams.ReadTextValueByParameterName(myConnection, GlobalEnumerates.SwParameters.RESTORE_POINT_DIR.ToString, Nothing)
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                Dim myRestoreParam As String = CStr(myGlobal.SetDatos)
                                ReportFolderPath = ficPruebas & myRestoreParam & SATReportRestorePrefix & " End Version " & pUserVersion  

                                Dim myFileName As String = ReportFolderPath & ".SAT"
                                If (File.Exists(myFileName)) Then
                                    'If there are a previous file with the same name, it is deleted instead of moved into previous folder
                                    File.Delete(myFileName)
                                End If
                            End If
#End If
                        Else
                            'Function has been called from User SW
                            ReportFolderPath = SATReportRestoreDir & SATReportRestorePrefix & " End Version " & pUserVersion

                            Dim myFileName As String = ReportFolderPath & ".SAT"
                            If File.Exists(myFileName) Then
                                'If there are a previous file with the same name, it is deleted instead of moved into previous folder
                                File.Delete(myFileName)
                            End If
                        End If

                    Case GlobalEnumerates.SATReportActions.SAT_UPDATE_ERR
                        If (Not pFilePath = String.Empty AndAlso Not pFileName = String.Empty) Then
                            ReportFolderPath = pFilePath & "\" & pFileName
                        Else
                            ReportFolderPath = SATReportDir & SATReportPrefix & DateTime.Now().ToString(GlobalConstants.SAT_DATETIME_FORMAT)
                        End If
                End Select

                '2.2 - Add the DataBase backup
                Dim myDatabaseAdmin As New DataBaseManagerDelegate()
                If (Not myDatabaseAdmin.BackUpDataBaseAndMoveBkFile(DAOBase.DBServer, DAOBase.CurrentDB, DAOBase.DBLogin, DAOBase.DBPassword, ReportFolderPath)) Then
                    'Error creating the DB Backup
                    Dim myLogAcciones As New ApplicationLogManager()
                    Dim Message As String = "Unable to create a backup copy of the data base"
                    myLogAcciones.CreateLogActivity(Message, "Utilities.CreateSATReport", EventLogEntryType.Error, False)

                    Dim myDataBaseManager As New DataBaseManagerDelegate
                    Dim IsSQLServer2005 As Boolean = myDataBaseManager.IsSQLServer2005(DAOBase.DBServer, DAOBase.DBLogin, DAOBase.DBPassword)
                    myLogAcciones.CreateLogActivity("IsSQLServer2005 = " & IsSQLServer2005.ToString(), "Utilities.CreateSATReport", EventLogEntryType.Error, False)
                End If

                '2.3 - Add the file with PC and OS Info 
                Dim SATReportOSFileNamePath As String = Application.StartupPath & GlobalBase.PCOSInfoFilePath & "\AX00PCOSInfo.txt"
                Dim PCOSFileName As String = ReportFolderPath & "\AX00PCOSInfo.txt"

                If (File.Exists(SATReportOSFileNamePath)) Then
                    'Create the Directory only if it does not exist; copy the file only if it does not exist
                    If (Not Directory.Exists(ReportFolderPath)) Then Directory.CreateDirectory(ReportFolderPath)
                    If (Not File.Exists(PCOSFileName)) Then File.Copy(SATReportOSFileNamePath, PCOSFileName)
                End If

                '2.4 - Add the Synapse Event Log
                If (pUserVersion <> "1.0.0") Then myGlobal = myUtil.SaveSynapseEventLog(GlobalBase.SynapseLogFileName, ReportFolderPath)

                '2.5 - Add the Application Log files 
                If (Directory.Exists(Application.StartupPath & GlobalBase.XmlLogFilePath)) Then ' XB 28/05/2013 - Correction : condition must done by Directory instead of File
                    Dim SATLogFolder As String = Application.StartupPath & GlobalBase.XmlLogFilePath
                    Dim di As New IO.DirectoryInfo(SATLogFolder)
                    Dim diar1 As IO.FileInfo() = di.GetFiles("Ax00Log_*.xml")
                    Dim dra As IO.FileInfo
                    Dim logFileName As String

                    For Each dra In diar1
                        logFileName = ReportFolderPath & "\" & dra.ToString

                        If (File.Exists(SATLogFolder & dra.ToString)) Then
                            Directory.CreateDirectory(ReportFolderPath)
                            File.Copy(SATLogFolder & dra.ToString, logFileName)
                        End If
                    Next dra

                    'Add the PreviousLog zip File depending on value of parameter pIncludeZIP
                    If (pIncludeZIP) Then
                        Dim diar2 As IO.FileInfo() = di.GetFiles("*.zip")
                        For Each dra In diar2
                            logFileName = ReportFolderPath & "\" & dra.ToString
                            If (File.Exists(SATLogFolder & dra.ToString)) Then
                                If (Not Directory.Exists(ReportFolderPath)) Then Directory.CreateDirectory(ReportFolderPath)
                                If (Not File.Exists(logFileName)) Then File.Copy(SATLogFolder & dra.ToString, logFileName)
                            End If
                        Next
                    End If

                    'BA-1944: Add the XML of the last Update Version process executed
                    Dim diar3 As IO.FileInfo() = di.GetFiles(GlobalBase.UpdateVersionProcessLogFileName)
                    For Each dra In diar3
                        logFileName = ReportFolderPath & "\" & dra.ToString
                        If (File.Exists(SATLogFolder & dra.ToString)) Then
                            If (Not Directory.Exists(ReportFolderPath)) Then Directory.CreateDirectory(ReportFolderPath)
                            If (Not File.Exists(logFileName)) Then File.Copy(SATLogFolder & dra.ToString, logFileName)
                        End If
                    Next
                End If

                '2.6 - Add the FW Adjustments file
                Dim FwAdjustmentsFileNamePath As String = pAdjFilePath
                Dim FwAdjustmentsFileName As String = ReportFolderPath & "\FWAdjustments.txt"

                Directory.CreateDirectory(ReportFolderPath)
                If (File.Exists(FwAdjustmentsFileNamePath)) Then File.Copy(FwAdjustmentsFileNamePath, FwAdjustmentsFileName)

                'NOT USED, ExcelPath is never informed
                'If (ExcelPath <> String.Empty) Then myUtil.CopyFiles(ExcelPath, ReportFolderPath & "\", "*.xls")

                '2.7 - Add the Log file of the Application Update process if it finished with Error
                If (pAction = GlobalEnumerates.SATReportActions.SAT_UPDATE_ERR) Then
                    If (File.Exists(Application.StartupPath & GlobalBase.PreviousFolder & GlobalBase.UpdateLogFile)) Then
                        myUtil.CopyFiles(Application.StartupPath & GlobalBase.PreviousFolder, ReportFolderPath & "\", GlobalBase.UpdateLogFile)
                    End If
                End If

                '2.8 - Add Service Files if function has been called from Service SW
                If (GlobalBase.IsServiceAssembly) Then
                    Dim FwScriptsFileNamePath As String = Application.StartupPath & GlobalBase.XmlFwScripts
                    Dim FwScriptsFileName As String = ReportFolderPath & "\FwScriptsData.xml"
                    If (File.Exists(FwScriptsFileNamePath)) Then
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(FwScriptsFileNamePath, FwScriptsFileName)
                    End If

                    Dim AnalyzerInfoFileNamePath As String = Application.StartupPath & GlobalBase.AnalyzerInfoFileOut
                    Dim AnalyzerInfoFileName As String = ReportFolderPath & "\AnalyzerInfo.txt"
                    If (File.Exists(AnalyzerInfoFileNamePath)) Then
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(AnalyzerInfoFileNamePath, AnalyzerInfoFileName)
                    End If

                    Dim PhotometryTestsFileNamePath As String = Application.StartupPath & GlobalBase.PhotometryTestsFile
                    Dim PhotometryTestsFileName As String = ReportFolderPath & "\PhotometryTests.bin"
                    If (File.Exists(PhotometryTestsFileNamePath)) Then
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(PhotometryTestsFileNamePath, PhotometryTestsFileName)
                    End If

                    Dim PhotometryBLFileNamePath As String = Application.StartupPath & GlobalBase.PhotometryBLFileOut
                    Dim PhotometryBLFileName As String = ReportFolderPath & "\BLResults.res"
                    If (File.Exists(PhotometryBLFileNamePath)) Then
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(PhotometryBLFileNamePath, PhotometryBLFileName)
                    End If

                    Dim PhotometryRepeatabilityFileNamePath As String = Application.StartupPath & GlobalBase.PhotometryRepeatabilityFileOut
                    Dim PhotometryRepeatabilityFileName As String = ReportFolderPath & "\RepeatabilityResults.res"
                    If (File.Exists(PhotometryRepeatabilityFileNamePath)) Then
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(PhotometryRepeatabilityFileNamePath, PhotometryRepeatabilityFileName)
                    End If

                    Dim PhotometryStabilityFileNamePath As String = Application.StartupPath & GlobalBase.PhotometryStabilityFileOut
                    Dim PhotometryStabilityFileName As String = ReportFolderPath & "\StabilityResults.res"
                    If (File.Exists(PhotometryStabilityFileNamePath)) Then
                        Directory.CreateDirectory(ReportFolderPath)
                        File.Copy(PhotometryStabilityFileNamePath, PhotometryStabilityFileName)
                    End If
                End If

                '2.9 - Add the plain text with the Application Version
                Dim myVersionFileName As String = GlobalBase.VersionFileName
                myGlobal = myUtil.CreateVersionFile(ReportFolderPath & "\" & myVersionFileName, pAction, pUserVersion, GlobalBase.IsServiceAssembly)

                '2.10 - Compress and delete the temporal folder
                myGlobal = myUtil.CompressToZip(ReportFolderPath, ReportFolderPath & GlobalBase.ZIPExtension)
                If (Not myGlobal Is Nothing AndAlso Not myGlobal.HasError) Then
                    If (Directory.Exists(ReportFolderPath)) Then Directory.Delete(ReportFolderPath, True)
                    myGlobal.SetDatos = True
                End If

            Catch ex As Exception
                If (Directory.Exists(ReportFolderPath)) Then Directory.Delete(ReportFolderPath, True)

                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
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
        ''' <param name="pAppVersion">String containing the ApplicationVersion returned for function GetSoftwareVersion composed of fields
        '''                           Major.Minor.Build. Revision field is never included</param>
        ''' <param name="pSATReportVersion">String containing the ApplicationVersion in file Version.txt. This string will contain the Revision
        '''                                 field (Major.Minor.Build.Revision) but only when it has a value greater than zero</param>
        ''' <returns>GlobalDataTO containing an String value indicating the result of the ApplicationVersion comparison: 
        '''           ** SAT and APP Version are equal OR 
        '''           ** SAT Version is greater than APP Version OR
        '''           ** SAT Version is less than APP Version
        '''          If the comparison cannot be done, a MISSING_DATA Error is returned
        ''' </returns>
        ''' <remarks>
        ''' Created by:  SG 13/10/2010
        ''' Modified by: SA 15/05/2014 - BT #1617 ==> When SAT Version contains the Revision field, ignore it when comparing the SAT Version 
        '''                                           with the current ApplicationVersion
        ''' </remarks>
        Public Function CompareSATandAPPversions(ByVal pAppVersion As String, ByVal pSATReportVersion As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                Dim myAppVersion As String() = pAppVersion.Split(CChar("."))
                Dim mySATVersion As String() = pSATReportVersion.Split(CChar("."))

                If (myAppVersion.Length > 0 AndAlso mySATVersion.Length > 0) Then
                    If (myAppVersion.Length = mySATVersion.Length OrElse myAppVersion.Length = (mySATVersion.Length - 1)) Then
                        Dim app As Integer = 0
                        Dim sat As Integer = 0

                        For v As Integer = 0 To myAppVersion.Length - 1
                            app = CInt(myAppVersion(v))
                            sat = CInt(mySATVersion(v))

                            If (sat <> app) Then
                                If (sat > app) Then
                                    myGlobal.SetDatos = GlobalEnumerates.SATReportVersionComparison.UpperThanAPP
                                ElseIf (sat < app) Then
                                    myGlobal.SetDatos = GlobalEnumerates.SATReportVersionComparison.LowerThanAPP
                                End If
                                Exit For
                            End If
                        Next

                        If (myGlobal.SetDatos Is Nothing) Then myGlobal.SetDatos = GlobalEnumerates.SATReportVersionComparison.EqualsAPP
                    Else
                        myGlobal.HasError = True
                        myGlobal.ErrorCode = "MISSING_DATA"
                    End If
                Else
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = "MISSING_DATA"
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
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
