Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports System.Configuration
Imports Biosystems.Ax00.BL.Framework
Imports System.Windows.Forms
Imports Microsoft.SqlServer.Management.Smo
Imports Biosystems.Ax00.Types
Imports System.IO
Imports System.Security
Imports System.Xml

Namespace Biosystems.Ax00.BL.UpdateVersion

    ''' <summary>
    ''' This Class manages the Business Logic related with the database update.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataBaseUpdateManagerDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Valiatate the installe database And application database version in case the application db version is greather than the
        ''' installed db, start the update process incharge to change database sctuctures and data.
        ''' </summary>
        ''' <param name="pServerName">Server name where database is installed</param>
        ''' <param name="pDataBaseName">Database name</param>
        ''' <param name="DBLogin">User login</param>
        ''' <param name="DBPassword">User password</param>
        ''' <returns>
        ''' GlobalDataTo with the result of the update procces if there was any db update return TRUE otherwise False.
        ''' incase there is and error on the update process will return has error and the error type.
        ''' </returns>
        ''' <remarks>
        ''' CREATED BY: TR 
        ''' Modified by XB 13/05/2013  - Add the new parameter pusSwVersion to evaluate required updates 
        '''                              when call ConfigureDataBaseAfterUpdateVersion method.
        ''' </remarks>
        Public Function UpdateDatabase(ByVal pServerName As String, ByVal pDataBaseName As String, _
                                             ByVal DBLogin As String, ByVal DBPassword As String, _
                                              Optional pLoadingRSAT As Boolean = False) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim result As Boolean = False
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity("On UpdateDatabase method ", "DataBaseUpdateManager.UpdateDatabase", EventLogEntryType.Information, False)

            Try
                'TR 16/01/2013 v1.0.1 - New implementation for DataBaseVersionEqual return a globaldataTO.
                Dim usSwVersion As String = ""
                Dim srvSwVersion As String = ""
                myGlobalDataTO = DataBaseVersionEqual(usSwVersion, srvSwVersion)

                'Validate the database version (if ">" method returns HasError = True)
                If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos.ToString() = "<" Then

                    'DL 18/01/2013  Create an automatic restore point (subfolders RestorePoints and Previous only for User Sw)
                    Dim mySATUtil As New SATReportUtilities
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = mySATUtil.CreateSATReport(GlobalEnumerates.SATReportActions.SAT_UPDATE, False, "", "", "", "", usSwVersion)
                    'DL 18/01/2013
                    If Not myGlobal.HasError Then
                        Dim myDatabaseAdmin As New DataBaseManagerDelegate()
                        'TR 16/01/2013 v1.0.1 -Search new database backup file to restore on temporal db
                        Dim myBackUpFile As String = AppDomain.CurrentDomain.BaseDirectory & GlobalBase.BakDirectoryPath & GlobalBase.TempDBBakupFileName

                        If IO.File.Exists(myBackUpFile) Then
                            'Create a temporal directory
                            myBackUpFile = CopyBackupToTempDirectory(myBackUpFile) 'copy the file to the new directory and assign the value.
                            'restore temporal database (Ax00_TEM)
                            If myDatabaseAdmin.RestoreDatabase(pServerName, GlobalBase.TemporalDBName, DBLogin, DBPassword, myBackUpFile) Then

                                myLogAcciones.CreateLogActivity("Temporal Database restore Success ", "DataBaseUpdateManager.UpdateDatabase", _
                                                                                                            EventLogEntryType.Information, False)
                                Dim myServer As Server = New Server(pServerName)

                                'RH 17/05/2011
                                myServer.ConnectionContext.LoginSecure = False
                                myServer.ConnectionContext.Login = DBLogin
                                myServer.ConnectionContext.Password = DBPassword
                                myServer.ConnectionContext.BeginTransaction()

                                'SGM 18/02/2013
                                'Set the Update process is in progress
                                SystemInfoManager.IsUpdateProcess = True

                                'Create 'Previous' folder if not exists
                                If Not Directory.Exists(Application.StartupPath & GlobalBase.PreviousFolder) Then
                                    Directory.CreateDirectory(Application.StartupPath & GlobalBase.PreviousFolder)
                                End If
                                'end SGM 18/02/2013
                                'TR 16/01/2013 -New implementation Update the tables structures and data.
                                myGlobalDataTO = UpdateDatabaseSructureAndData(pDataBaseName, myServer)

                                'TR 18/02/2013 -Add the data update into the same transaction.
                                'AG 16/01/2013 v1.0.1
                                If Not myGlobalDataTO.HasError Then
                                    myGlobalDataTO = ConfigureDataBaseAfterUpdateVersion(myServer, usSwVersion, pLoadingRSAT) 'XB 13/05/2013 -pass usSwVersion parameter to function
                                    'Inform the database update version process failed
                                    If myGlobalDataTO.HasError Then myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString
                                Else
                                    'Warn the user (return myGlobalDataTo with HasError = True and the same ErrorCode)
                                End If
                                'AG 16/01/2013 v1.0.1
                                'TR 18/02/2013 -END

                                'Validate if there was error updating the structures and data.
                                If myGlobalDataTO.HasError Then
                                    myServer.ConnectionContext.RollBackTransaction()
                                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString()
                                    myGlobalDataTO.SetDatos = False
                                    Dim myLogFile As String = String.Empty
                                    'Validate if exist log file 

                                    'Search for update log file on previous folder and add it to RSAT
                                    If System.IO.File.Exists(Application.StartupPath & GlobalBase.PreviousFolder & GlobalBase.UpdateLogFile) Then
                                        myLogFile = Application.StartupPath & GlobalBase.PreviousFolder & GlobalBase.UpdateLogFile
                                    End If

                                    'CREATE A RSAT and copy the Xml file used for log in updateprocess.
                                    'myGlobal = mySATUtil.CreateSATReport(GlobalEnumerates.SATReportActions.SAT_UPDATE, False, "", "", "", "", usSwVersion)
                                    myGlobal = mySATUtil.CreateSATReport(GlobalEnumerates.SATReportActions.SAT_UPDATE_ERR, False, "", _
                                                                         myLogFile, Application.StartupPath & GlobalBase.PreviousFolder, _
                                                                         "RSATUpdateError", usSwVersion)

                                    'Remove the UpdateLog File because is included on the RSAT
                                    If File.Exists(myLogFile) Then
                                        File.Delete(myLogFile)
                                    End If
                                Else
                                    myServer.ConnectionContext.CommitTransaction()
                                    myGlobalDataTO.SetDatos = True
                                    'If the process was successful Delete the log file - SGM 18/02/2013
                                    If System.IO.File.Exists(Application.StartupPath & GlobalBase.PreviousFolder & GlobalBase.UpdateLogFile) Then
                                        System.IO.File.Delete(Application.StartupPath & GlobalBase.PreviousFolder & GlobalBase.UpdateLogFile)
                                    End If
                                    'TR 08/07/2013 -After Update process then execute the SHRINK Command outside the transaction.
                                    myGlobalDataTO = ShrinkDatabase(pDataBaseName, myServer)
                                End If
                                'TR 16/01/2013 -End
                            End If
                            'End If
                        End If
                    Else
                        'Restore point creation error.
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString()
                        myGlobalDataTO.SetDatos = False
                        myGlobalDataTO.HasError = True
                    End If
                    'DL 18/01/2013
                Else
                    myGlobalDataTO.SetDatos = False
                    'TODO: AG Validate if there was an error or if the versions are equals.
                    If myGlobalDataTO.HasError OrElse myGlobalDataTO.SetDatos.ToString() = ">" Then
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_VERSION.ToString()
                    End If
                    'myLogAcciones.CreateLogActivity("database version Equals", "DataBaseUpdateManager.UpdateDatabase", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " Innner: " + ex.InnerException.Message
                myLogAcciones.CreateLogActivity(ex.Message, "DataBaseUpdateManager.UpdateDatabase", EventLogEntryType.Error, False)
            End Try

            'Set the update process as finished - SGM 18/02/2013
            SystemInfoManager.IsUpdateProcess = False

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update the tables structures and some tables information.
        ''' </summary>
        ''' <param name="pDataBaseName"></param>
        ''' <param name="pServer"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY : TR 16/01/2012  v1.0.1-New implementation
        ''' </remarks>
        Public Function UpdateDatabaseSructureAndData(ByVal pDataBaseName As String, ByRef pServer As Server) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myLogAcciones As New ApplicationLogManager()
            Try
                'Get installed Database Version
                Dim InstalledDataBaseVersion As Single = 0
                Dim PackageID As String = String.Empty

                Dim myVersionsDelegate As New VersionsDelegate
                myGlobalDataTO = myVersionsDelegate.GetVersionsData(Nothing)
                If Not myGlobalDataTO.HasError Then
                    Dim myVersionsDS As New VersionsDS
                    myVersionsDS = DirectCast(myGlobalDataTO.SetDatos, VersionsDS)

                    If myVersionsDS.tfmwVersions.Count > 0 Then
                        'Set the value to local varible.
                        InstalledDataBaseVersion = CSng(myVersionsDS.tfmwVersions(0).DBSoftware.Replace(".", SystemInfoManager.OSDecimalSeparator))
                        PackageID = myVersionsDS.tfmwVersions(0).PackageID
                    End If
                End If
                'Validate if could get the installed database version.
                If Not InstalledDataBaseVersion = 0 Then
                    Dim myTaskListDS As New TaskListDS
                    myGlobalDataTO = DecryptFile(AppDomain.CurrentDomain.BaseDirectory & GlobalBase.InstallDBBTaskFilePath)
                    If Not myGlobalDataTO.HasError Then
                        myTaskListDS.ReadXml(New XmlNodeReader(DirectCast(myGlobalDataTO.SetDatos, XmlDocument)))
                    Else

                    End If
                    'Validate  if there're information loaded on tables to continue
                    If myTaskListDS.CommonTask.Count > 0 OrElse myTaskListDS.AppVersion.Count > 0 Then
                        Dim myDataBaseMangDelegate As New DataBaseManagerDelegate()
                        Dim ScriptExecOK As Boolean = True 'Use to indicate if the sctructure script execute Ok.
                        'Execute the commonTask 
                        For Each CommonTaskRow As TaskListDS.CommonTaskRow In myTaskListDS.CommonTask.Rows
                            'Update the Structure if exist.
                            If Not CommonTaskRow.StructureScript = String.Empty Then
                                If Not myDataBaseMangDelegate.RunDatabaseScript(pServer, pDataBaseName, CommonTaskRow.StructureScript) Then
                                    ScriptExecOK = False ' set false to execution script and exit for
                                    Exit For
                                End If
                            End If
                            'Update Data if exist.
                            If Not CommonTaskRow.UpdateScript = String.Empty Then
                                If Not myDataBaseMangDelegate.RunDatabaseScript(pServer, pDataBaseName, CommonTaskRow.UpdateScript) Then
                                    ScriptExecOK = False ' set false to execution script and exit for
                                    Exit For
                                End If
                            End If
                        Next

                        'Validate if there was no error on previous script execution to continue or rollback transaction.
                        If ScriptExecOK Then
                            Dim TaskVersion As Single = 0 ' declar the task version variable to compare.
                            Dim StructureUpdate As Boolean = False ' use to control the database structure update
                            Dim DataUpdate As Boolean = False ' use to control the data update.

                            'Then execute the appversion update scrips
                            For Each AppVersionkRow As TaskListDS.AppVersionRow In myTaskListDS.AppVersion.Rows
                                StructureUpdate = False
                                DataUpdate = False
                                TaskVersion = AppVersionkRow.Version ' assign the value from the task list.

                                If TaskVersion > InstalledDataBaseVersion Then ' validate is the database version is oldest 

                                    If Not AppVersionkRow.StructureScript = String.Empty Then ' validate if there is any structure update script.
                                        If myDataBaseMangDelegate.RunDatabaseScript(pServer, pDataBaseName, AppVersionkRow.StructureScript) Then
                                            StructureUpdate = True
                                        Else

                                        End If
                                    Else
                                        StructureUpdate = True
                                    End If
                                    'update data if there any
                                    If Not AppVersionkRow.UpdateScript = String.Empty Then 'validate if there is any data update script.
                                        If myDataBaseMangDelegate.RunDatabaseScript(pServer, pDataBaseName, AppVersionkRow.UpdateScript) Then
                                            myLogAcciones.CreateLogActivity("Data Updated", "DataBaseAdmin.UpdataDatabaseSructureAndData", EventLogEntryType.Information, False)
                                            DataUpdate = True
                                        Else
                                            myLogAcciones.CreateLogActivity("DataUpdate error.", "DataBaseAdmin.UpdateDatabaseSructureAndData", EventLogEntryType.Information, False)
                                        End If
                                    Else
                                        DataUpdate = True
                                    End If

                                    'if everything ok the send de true value as function result.
                                    If StructureUpdate And DataUpdate Then
                                        ' update the new Database version. pass the server connection contex because we are inside a transaction that can affect the table.
                                        myGlobalDataTO = myVersionsDelegate.SaveDBSoftwareVersion(pServer.ConnectionContext.SqlConnectionObject(), _
                                                                                                  PackageID, TaskVersion)
                                        If Not myGlobalDataTO.HasError Then
                                            InstalledDataBaseVersion = TaskVersion
                                        Else
                                            'Send error Invalid databaser update
                                            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString()
                                        End If
                                    Else
                                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString()
                                    End If
                                ElseIf TaskVersion = InstalledDataBaseVersion Then
                                    'result = True
                                End If
                            Next
                        Else ' there was a problem on the previos scrips executions 
                            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString()
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
                myLogAcciones.CreateLogActivity(ex.Message, "DataBaseAdmin.UpdateDatabaseSructureAndData", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a TEMP directory if do not exist, and copy the database backup
        ''' into the TEMP directory.
        ''' >Move the backup file into the TEMP Directory, because SQL user do not have permision.
        ''' to access to the application software.
        ''' </summary>
        ''' <param name="BackupFile"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' MODIFIED BY: TR 17/01/2013 V1.0.1 -use values on global base.
        ''' </remarks>
        Public Function CopyBackupToTempDirectory(ByVal BackupFile As String) As String
            Dim TempDirectory As String = GlobalBase.TemporalDirectory  '"C:\TEMP"
            Dim NewBackupFilePath As String = ""
            Try
                If Not IO.Directory.Exists(TempDirectory) Then ' validate if the directory exist to create it.
                    IO.Directory.CreateDirectory(TempDirectory) 'create the Temp Directory
                End If
                If IO.File.Exists(TempDirectory & GlobalBase.TempDBBakupFileName) Then ' validate if the file exist to delete
                    IO.File.Delete(TempDirectory & GlobalBase.TempDBBakupFileName) 'delete the file 
                End If
                If IO.File.Exists(BackupFile) Then ' validate if the file exist at the application software.
                    IO.File.Copy(BackupFile, TempDirectory & GlobalBase.TempDBBakupFileName) ' copy the file into the temp directory
                    NewBackupFilePath = TempDirectory & GlobalBase.TempDBBakupFileName ' assigne the value to send as result of the operation.
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "DataBaseUpdateManager.CopyBackupToTEMPDirectory", EventLogEntryType.Error, False)
            End Try
            Return NewBackupFilePath
        End Function

        ''' <summary>
        ''' Remove the temporal database, created to move the new data.
        ''' and the backup file from the temp directory too.
        ''' </summary>
        ''' <param name="TempBackupFile">Backup file Name, with the path.</param>
        ''' <param name="ServerName">Server name</param>
        ''' <param name="TempDatabaseName">Temporal Database Name.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveBackupFileAndTempDatabase(ByVal TempBackupFile As String, ByVal ServerName As String, _
                                                        ByVal DBLogin As String, ByVal DBPassword As String, _
                                                        ByVal TempDatabaseName As String) As Boolean
            Dim result As Boolean = False
            Try
                'validate if the backupfile still exist.
                If IO.File.Exists(TempBackupFile) Then
                    IO.File.Delete(TempBackupFile) 'remove temp. backup file.
                End If
                Dim myDatabaseAdmin As New DataBaseManagerDelegate()
                result = myDatabaseAdmin.DeleteDatabase(ServerName, TempDatabaseName, DBLogin, DBPassword) 'remover Temp. Database
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " --- " & ex.InnerException.ToString(), _
                                                "DataBaseUpdateManager.RemoveBackupFileAndTempDatabase", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Shrinking data files recovers space by moving pages of data from the end of the file to unoccupied space closer to the front of the file. 
        ''' When enough free space is created at the end of the file, data pages at end of the file can deallocated and returned to the file system.
        ''' </summary>
        ''' <param name="pDataBaseName">Data Base Name</param>
        ''' <param name="pServer">Server Name</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 08/07/2013
        ''' UPDATE BY: TR 10/07/2013 -Validate if server is nothing in this case config the pServer parameter.
        ''' </remarks>
        Public Function ShrinkDatabase(ByVal pDataBaseName As String, ByRef pServer As Server) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDataBaseMangDelegate As New DataBaseManagerDelegate()
                Dim myShrinkScript As String = "USE [" & pDataBaseName & "] DBCC SHRINKDATABASE(N'" & pDataBaseName & "' )"

                If pServer Is Nothing Then
                    'Create a new server control
                    pServer = New Server(DAL.DAOBase.DBServer)
                    pServer.ConnectionContext.LoginSecure = False
                    pServer.ConnectionContext.Login = DAL.DAOBase.DBLogin
                    pServer.ConnectionContext.Password = DAL.DAOBase.DBPassword
                End If

                myGlobalDataTO.SetDatos = myDataBaseMangDelegate.RunDatabaseScript(pServer, pDataBaseName, myShrinkScript)

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " Innner: " + ex.InnerException.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "DataBaseUpdateManager.ShrinkDatabase", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Decrypt the TaskList file an load it into XmlDocument
        ''' </summary>
        ''' <param name="CryptFile">File to decrypt</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 22/01/2013 v1.0.1
        ''' </remarks>
        Private Function DecryptFile(ByVal CryptFile As String) As GlobalDataTO
            '// Se copia de: http://support.microsoft.com/kb/301070
            Dim myGlobalDataTO As New GlobalDataTO
            Dim CryptKey As String = "Ax00Bios"  '// Key de encriptacion (deben ser 8 chars pq sino falla el algoritmo que usamos)
            Dim cryptostreamDecr As Cryptography.CryptoStream = Nothing
            Dim fsread As FileStream = Nothing
            Try
                Dim DES As New Cryptography.DESCryptoServiceProvider()
                DES.Key() = Text.ASCIIEncoding.ASCII.GetBytes(CryptKey)
                DES.IV = Text.ASCIIEncoding.ASCII.GetBytes(CryptKey)

                fsread = New FileStream(CryptFile, FileMode.Open, FileAccess.Read)

                cryptostreamDecr = New Cryptography.CryptoStream(fsread, DES.CreateDecryptor(), Cryptography.CryptoStreamMode.Read)

                Dim xmlDoc As New XmlDocument()
                xmlDoc.Load(New StreamReader(cryptostreamDecr))
                'Send the xml document.
                myGlobalDataTO.SetDatos = xmlDoc

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "DatabaseUpdateManager.DecryptFile", EventLogEntryType.Error, False)
            Finally
                '// Close the files
                If Not fsread Is Nothing Then
                    fsread.Close()
                    fsread.Dispose()
                    fsread = Nothing
                End If
                If Not cryptostreamDecr Is Nothing Then
                    cryptostreamDecr = Nothing
                End If
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Compare the Installed database version with the application database version.
        ''' </summary>
        ''' <returns> "=" if equal, if lower or if greather</returns>
        ''' <remarks>
        ''' CREATE BY: 
        ''' MODIFIED BY: TR 16/01/2013 V1.0.1 -Use the table tfmwVersions to get the application version and the database version instead the ApplicationSetting data table.
        ''' AG 17/01/2013 v1.0.1 - informs also the 2 byref parameters
        ''' </remarks>
        Private Function DataBaseVersionEqual(ByRef pUsSwVersion As String, ByRef pSrvSwVersion As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'Get the Application and Database Version from installed application 
                Dim AppDataBaseVersion As Double = CDbl(GlobalBase.DataBaseVersion.Replace(".", SystemInfoManager.OSDecimalSeparator)) 'Global setting.
                Dim AppApplicationVersion As String = Application.ProductVersion ' application product version.
                ' InstalledDataBaseVersion = CDbl(myVersionsDS.tfmwVersions(0).DBSoftware.Replace(".", SystemInfoManager.OSDecimalSeparator))
                'Get the Application and Database version from installed database
                Dim myVersionsDelegate As New VersionsDelegate
                myGlobalDataTO = myVersionsDelegate.GetVersionsData(Nothing)
                If Not myGlobalDataTO.HasError Then
                    Dim myVersionsDS As New VersionsDS
                    myVersionsDS = DirectCast(myGlobalDataTO.SetDatos, VersionsDS)
                    If myVersionsDS.tfmwVersions.Count > 0 Then
                        'AG 17/01/2013 v1.0.1 - Inform the Us and Srv Sw versions
                        pUsSwVersion = myVersionsDS.tfmwVersions(0).UserSoftware
                        pSrvSwVersion = myVersionsDS.tfmwVersions(0).ServiceSoftware
                        Dim myDBVersion As Double = CDbl(myVersionsDS.tfmwVersions(0).DBSoftware.Replace(".", SystemInfoManager.OSDecimalSeparator))
                        'Validate if versions are equal
                        If myDBVersion = AppDataBaseVersion Then
                            myGlobalDataTO.SetDatos = "="
                        ElseIf myDBVersion < AppDataBaseVersion Then
                            myGlobalDataTO.SetDatos = "<"
                        ElseIf myDBVersion > AppDataBaseVersion Then
                            myGlobalDataTO.SetDatos = ">"
                            'There is an error 'cause database version installed on server is greather than the application dbVersion.
                            myGlobalDataTO.HasError = True
                            'Set the corresponding error code indicating the invalid database version
                            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_VERSION.ToString()
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "DataBaseUpdateManager.DataBaseVersionEqual", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Validate if Database Version is Greater than the Application database version.
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>NOT USED ON V1.0.1</remarks>
        Private Function DataBaseVerGreaterThanAppVer() As Boolean
            Dim result As Boolean = False
            Try
                'get the app.config Database Version
                Dim AppDataBaseVersion As Double = CType(GlobalBase.DataBaseVersion, Double) ' get application database version.
                Dim myApplicationSetting As New ApplicationSettingDelegate() 'declare the object to get the Database installed
                'get the database version.
                Dim DatabaseVersion As Double = CType(myApplicationSetting.GetApplicationSettingCurrentValueBySettingID("DatabaseVersion"), Double) ' get database version.
                If DatabaseVersion > AppDataBaseVersion Then 'validate if version is greater
                    result = True
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "DataBaseVerGreaterThanAppVer.UpdateDatabase", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function


        ''' <summary>
        ''' Execute required business after update database process
        ''' 1) Update alarms definition table
        ''' 2) Reset current WorkSession
        ''' 3) Update preloaded BioSystems test programming
        ''' </summary>
        '''  <param name="pLoadingRSAT">Indicate if loading a RSAT.</param>
        ''' <returns>GlobalDataTo</returns>
        ''' <remarks>
        ''' Created by:  AG 16/01/2013
        ''' Modified by: TR 29/01/2013 - Added new optional parameter pLoadingRSAT to validate if user is loading a RSAT, in which case
        '''                              the Reset WorSession and the Factory Tests Programming updates are not executed
        '''              TR 18/02/2013 - Added the new parameter pServer to implement the required DB Transaction
        '''              XB 13/05/2013 - Added the new parameter pusSwVersion to evaluate required updates
        '''              SA 17/10/2014 - BA-1944 ==> 
        ''' </remarks>
        Private Function ConfigureDataBaseAfterUpdateVersion(ByRef pServer As Server, ByVal pusSwVersion As String, _
                                                             Optional pLoadingRSAT As Boolean = False) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Get the current language (USR software)
                'We do not get the SRV language because it not requires update the alarms definition (name, description, solution)
                Dim myUserSettingsDelegate As New UserSettingsDelegate
                myGlobal = myUserSettingsDelegate.GetCurrentValueBySettingID(pServer.ConnectionContext.SqlConnectionObject(), _
                                                                             GlobalEnumerates.UserSettingsEnum.CURRENT_LANGUAGE.ToString)

                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    Dim langID As String = CType(myGlobal.SetDatos, String)

                    'Update alarms definition
                    Dim alarmsDlg As New AlarmsDelegate
                    myGlobal = alarmsDlg.UpdateLanguageResource(pServer.ConnectionContext.SqlConnectionObject(), langID)

                    'TR 29/01/2013 - Validate the user is not loading a RSAT
                    If (Not pLoadingRSAT) Then
                        If (Not myGlobal.HasError) Then
                            Dim analyzerModel As String = String.Empty
                            Dim analyzerID As String = String.Empty
                            Dim worksessionID As String = String.Empty

                            'Get WorkSessionID and AnalyzerID
                            GetAnalyzerAndWorksessionInfo(pServer.ConnectionContext.SqlConnectionObject(), analyzerModel, analyzerID, worksessionID)

                            'Reset any WorkSession before starting the Update Version Process
                            Dim myWS As New WorkSessionsDelegate
                            If (Not GlobalConstants.HISTWorkingMode) Then
                                myGlobal = myWS.ResetWS(pServer.ConnectionContext.SqlConnectionObject(), analyzerID, worksessionID)
                            Else
                                myGlobal = myWS.ResetWSNEW(pServer.ConnectionContext.SqlConnectionObject(), analyzerID, worksessionID, False, True, True)
                            End If

                            'Update preloaded BioSystems Tests
                            If (Not myGlobal.HasError) Then
                                Dim updateTest As New UpdatePreloadedFactoryTestDelegate

                                myGlobal = updateTest.SetFactoryTestProgrammingNEW(pServer.ConnectionContext.SqlConnectionObject(), pusSwVersion)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    Dim myUpdateVersionChangesList As UpdateVersionChangesDS = DirectCast(myGlobal.SetDatos, UpdateVersionChangesDS)

                                    Dim myDirName As String = Application.StartupPath & GlobalBase.XmlLogFilePath
                                    Dim myFileName As String = Now.ToString("yyyyMMdd HHmm") & " UPDATE VERSION CHANGES.xml"

                                    'If needed, create the final directory... 
                                    If (Not IO.Directory.Exists(myDirName)) Then IO.Directory.CreateDirectory(myDirName)

                                    'Write the XML
                                    myUpdateVersionChangesList.WriteXml(myDirName & myFileName)
                                End If
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.ConfigureDataBaseAfterUpdateVersion", EventLogEntryType.Error, False)
                myGlobal.HasError = True
                myGlobal.ErrorMessage = ex.Message
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Get the analyzer model, analyzerID and worksessionID
        ''' (copied and adapted from BioSystemUpdate.GetAnalyzerAndWorksessionInfo)
        ''' </summary>
        ''' <param name="dbConnection"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <remarks>
        ''' Created AG 16/01/2013
        ''' Modified by: TR 29/10/2013 - BT # 1370 if the Active WS is not found by the analyzer number then search againg withourt the analyzer number
        '''                             on table twksWSAnalyzers.
        ''' </remarks>
        Private Sub GetAnalyzerAndWorksessionInfo(ByVal dbConnection As SqlClient.SqlConnection, ByRef pAnalyzerModel As String, ByRef pAnalyzerID As String, ByRef pWorkSessionID As String)

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myAnalyzerDelegate As New AnalyzersDelegate
            Try
                myGlobalDataTO = myAnalyzerDelegate.GetAnalyzer(dbConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myAnalyzerData As New AnalyzersDS
                    myAnalyzerData = DirectCast(myGlobalDataTO.SetDatos, AnalyzersDS)

                    If (myAnalyzerData.tcfgAnalyzers.Rows.Count > 0) Then
                        'Inform properties AnalyzerID and AnalyzerModel
                        pAnalyzerModel = myAnalyzerData.tcfgAnalyzers(0).AnalyzerModel.ToString
                        pAnalyzerID = myAnalyzerData.tcfgAnalyzers(0).AnalyzerID.ToString
                    End If
                End If

                Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate

                myGlobalDataTO = myWSAnalyzersDelegate.GetActiveWSByAnalyzer(dbConnection, pAnalyzerID)

                Dim myWSAnalyzersDS As New WSAnalyzersDS
                myWSAnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS)
                If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
                    pWorkSessionID = myWSAnalyzersDS.twksWSAnalyzers(0).WorkSessionID.ToString()
                Else
                    'TR 29/10/2013 -Get tha WS an the analyser info BT #1370.
                    myGlobalDataTO = myWSAnalyzersDelegate.GetActiveWSByAnalyzer(dbConnection)
                    If Not myGlobalDataTO.HasError Then
                        myWSAnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS)
                        If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
                            pWorkSessionID = myWSAnalyzersDS.twksWSAnalyzers(0).WorkSessionID.ToString()
                            pAnalyzerModel = myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerModel.ToString
                            pAnalyzerID = myWSAnalyzersDS.twksWSAnalyzers(0).AnalyzerID.ToString
                        End If
                    End If
                    'TR 29/10/2013 -END.
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.GetAnalyzerAndWorksessionInfo", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

        End Sub

#End Region

    End Class
End Namespace


