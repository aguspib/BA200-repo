Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports System.IO
Imports System.Xml
Imports Microsoft.SqlServer.Management.Smo
Imports System.Windows.Forms
Imports Biosystems.Ax00.Core.Entities.UpdateVersion
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Framework.CrossCutting
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.BL.UpdateVersion

    ''' <summary>
    ''' This Class manages the Business Logic related with the database update.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataBaseUpdateManagerDelegate

#Region "   INSTALL AND UPDATE PROCESS"

        ''' <summary>
        ''' Method incharge to start the database Update and instalation process.
        ''' </summary>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: AG 16/01/2013 v1.0.1 - Evaluate if successfully or not and continue business (see update version process design)
        '''              TR 29/01/2013 v1.0.1 - Add the new parameter pLoadingRSAT to indicate if user is loading a RSAT
        '''              SA 16/05/2014 - BT #1632 ==> Before start the update process, execute the temporary script used to change the structure 
        '''                                           of ApplicationLog table (tfmwApplicationLog) 
        '''              IT 08/05/2015 - BA-2471
        '''              MR 09/06/2015 - BA-2566 ==> We change the flow of the function adding the functionality of rename the Database depends on which 
        '''                                          Intance Analyzer start the Application.
        '''              IT 11/06/2015 - BA-2600
        '''              IT 15/07/2015 - BA-2693
        ''' </remarks>
        Public Function InstallUpdateProcess(ByVal pServerName As String, ByVal pDataBaseName As String, ByVal DBLogin As String, _
                                             ByVal DBPassword As String, Optional pLoadingRSAT As Boolean = False, Optional pModel As String = "") As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim initialTimeUpdate As New DateTime 'TR Variable used to validate the time 

            Try

                'GlobalBase.CreateLogActivity("InstallUpdateProcess" & ".Updateprocess -Validating if Data Base exists ", "Installation validation", EventLogEntryType.Information, False)
                initialTimeUpdate = Now 'Set the start time 
                Debug.Print("INICIO-->" & initialTimeUpdate.TimeOfDay.ToString()) 'Print the time
                'GlobalBase.CreateLogActivity("InstallUpdateProcess" & ".Updateprocess - Database found", "Installation validation", EventLogEntryType.Information, False)

                If Not ValidateDatabaseName(pServerName, pDataBaseName, DBLogin, DBPassword) Then 'IT 15/07/2015 - BA-2693
                    myGlobalDataTO = InstallProcessApplication(pServerName, pDataBaseName, DBLogin, DBPassword)
                End If

                If (Not myGlobalDataTO.HasError) Then
                    If String.IsNullOrEmpty(pModel) Then pModel = GetAnalyzerModel()
                    myGlobalDataTO = UpdateProcessApplication(pServerName, pDataBaseName, DBLogin, DBPassword, pLoadingRSAT, pModel)
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorMessage = ex.Message
            Finally
                'TR 21/01/2013 v1.0.1 Delete db temp bak file  and remove from server.
                RemoveBackupFileAndTempDatabase(GlobalBase.TemporalDirectory & GlobalBase.TempDBBakupFileName, pServerName, DBLogin, DBPassword, GlobalBase.TemporalDBName)
                Debug.Print("FIN-->" & Now.TimeOfDay.ToString()) 'print the finish time
                Debug.Print("TOTAL-->" & (Now - initialTimeUpdate).ToString()) 'get the diference.
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Function to call to internal functions to follow with the InstallAplication Process doing a restore of the database.
        ''' Function used in two scenarios: 
        ''' 1 - First installation 
        ''' 2 - Restore .bak file we distribute with the software.
        ''' </summary>
        ''' <param name="pServerName">Name of the Server where CUSTOMER DB is installed</param>
        ''' <param name="pDataBaseName">CUSTOMER DB Name by Instance </param>
        ''' <param name="DBLogin">DB Login</param>
        ''' <param name="DBPassword">DB Password</param>
        ''' <returns>GlobalDataTO with the result of the Install procces: if the Installation was successfully executed, the function
        '''          returns TRUE; otherwise it returns FALSE. If the Installation is executed but an error is raised during execution, the 
        '''          Error is informed in fields ErrorCode and ErrorDescription in the GlobalDataTO, and HasError is set to TRUE</returns>
        ''' <remarks> Created by:  MR 09/06/2015 ==> BA-2566 - Peace of code was placed in InstallUpdateProcess and is needed it more than once. Then I created this function.</remarks>
        Private Function InstallProcessApplication(ByVal pServerName As String, ByVal pDataBaseName As String, ByVal DBLogin As String, _
                                             ByVal DBPassword As String) As GlobalDataTO


            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'GlobalBase.CreateLogActivity("Before installing the DB", "Update process", EventLogEntryType.Information, False)
                'Call the Install database delegate.
                Dim myDBInstallerDelegate As New DataBaseInstallerManagerDelegate()
                'TR 22/01/2013 v1.0.1 -Change to recive a globalTO instead of boolean.
                myGlobalDataTO = myDBInstallerDelegate.InstallApplicationDataBase(pServerName, pDataBaseName, DBLogin, DBPassword)
                'RH 01/03/2012 Wait a moment, while the recently created DB becomes ready to work
                If Not myGlobalDataTO.HasError Then
                    System.Threading.Thread.Sleep(5000)
                End If
                myGlobalDataTO.HasError = False 'AG 16/01/2013 - clear the error flag. Validation will be done in method IAx00Login.CheckDataBaseAvailability
                ' in case this function do not return error

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Function who follow with the update process.
        ''' </summary>
        ''' <param name="pServerName">Name of the Server where CUSTOMER DB is installed</param>
        ''' <param name="pDataBaseName">CUSTOMER DB Name by Instance</param>
        ''' <param name="DBLogin">DB Login</param>
        ''' <param name="DBPassword">DB Password</param>
        ''' <param name="pLoadingRSAT">Optional parameter to say if will load a SAT report.</param>
        ''' <returns>GlobalDataTO with the result of the Update procces: if the Update was successfully executed, the function
        '''          returns TRUE; otherwise it returns FALSE. If the Update is executed but an error is raised during execution, the 
        '''          Error is informed in fields ErrorCode and ErrorDescription in the GlobalDataTO, and HasError is set to TRUE</returns>
        ''' <remarks> Created by:  MR 09/06/2015 ==> BA-2566 - Peace of code was placed in InstallUpdateProcessand we need it more than once. Then I created this function. </remarks>
        Private Function UpdateProcessApplication(ByVal pServerName As String, ByVal pDataBaseName As String, ByVal DBLogin As String, _
                                        ByVal DBPassword As String, Optional pLoadingRSAT As Boolean = False, Optional pModel As String = "") As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim initialTimeUpdate As New DateTime 'TR Variable used to validate the time 

            Try
                'BT #1632 - Before start the update process, execute temporary scripts used to change the structure of tables that have to 
                '           be already updated when the UpdateVersion process starts (f.i. ApplicationLog Table --> tfmwApplicationLog)
                ExecuteScriptsBeforeUpdate()

                'Call the Update Database delegate.
                myGlobalDataTO = UpdateDatabase(pServerName, pDataBaseName, DBLogin, DBPassword, pLoadingRSAT, pModel)

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Function that returns the name of the Active Analyzer Model on the Database
        ''' </summary>
        ''' <returns> String with the active Analyzer Model name on the Database</returns>
        ''' <remarks>Created by:  AC 11/06/2015 ==> BA-2594 </remarks>
        Public Shared Function GetAnalyzerModel() As String
            Dim Model As String = ""
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                Dim connection = DAOBase.GetOpenDBConnection(Nothing)
                If (Not connection.HasError AndAlso Not connection.SetDatos Is Nothing) Then
                    dbConnection = CType(connection.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim tcfAnalyzerDAO As New DAL.DAO.tcfgAnalyzersDAO
                        Dim returnData As GlobalDataTO = Nothing
                        returnData = tcfAnalyzerDAO.ReadByAnalyzerActive(dbConnection)
                        If Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing Then
                            Dim AnalyzerObj As AnalyzersDS = CType(returnData.SetDatos, AnalyzersDS)
                            'The app only can have one active analyzer.
                            If (AnalyzerObj.tcfgAnalyzers.Rows.Count > 0 AndAlso AnalyzerObj.tcfgAnalyzers.Rows.Count = 1) Then
                                Dim AnalyzerRow As AnalyzersDS.tcfgAnalyzersRow = AnalyzerObj.tcfgAnalyzers(0)
                                Return AnalyzerRow.AnalyzerModel.ToUpper.Trim()
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)
                Throw
            Finally
                If (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return Model
        End Function

        ''' <summary>
        ''' Function that checks from which analyzer belong the current Database 'Ax00'
        ''' </summary>
        ''' <returns> Boolean attribute saying if the AnalyzerModel active in the database is the same of who started the instance of the application.</returns>
        ''' <remarks>Created by:  MR 09/06/2015 ==> BA-2566 </remarks>
        Private Function IsSameAnalyzer(ByVal pServerName As String, ByVal pDataBaseName As String, ByVal DBLogin As String, _
                                        ByVal DBPassword As String) As Boolean
            Dim result As Boolean = False
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim dataToReturn As GlobalDataTO = Nothing

            Try
                Dim LocalServer As New Server(pServerName)

                LocalServer.ConnectionContext.LoginSecure = False
                LocalServer.ConnectionContext.Login = DBLogin
                LocalServer.ConnectionContext.Password = DBPassword
                LocalServer.ConnectionContext.DatabaseName = pDataBaseName

                dbConnection = LocalServer.ConnectionContext.SqlConnectionObject()

                If (Not dbConnection Is Nothing) Then
                    Dim tcfAnalyzerDAO As New DAL.DAO.tcfgAnalyzersDAO
                    Dim returnData As GlobalDataTO = Nothing
                    returnData = tcfAnalyzerDAO.ReadByAnalyzerActive(dbConnection)
                    If Not returnData.HasError AndAlso Not returnData.SetDatos Is Nothing Then
                        Dim AnalyzerObj As AnalyzersDS = CType(returnData.SetDatos, AnalyzersDS)
                        'The app only can have one active analyzer.
                        If (AnalyzerObj.tcfgAnalyzers.Rows.Count > 0 AndAlso AnalyzerObj.tcfgAnalyzers.Rows.Count = 1) Then
                            Dim AnalyzerRow As AnalyzersDS.tcfgAnalyzersRow = AnalyzerObj.tcfgAnalyzers(0)
                            If AnalyzerRow.AnalyzerModel.ToUpper.Trim().Equals(GlobalBase.DatabaseName.ToUpper.Trim()) Then
                                result = True
                            End If
                        End If
                    End If
                End If

            Catch
                Throw
            Finally
                If (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return result
        End Function

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Compare versions of the installed CUSTOMER DB and the FACTORY DB and determine the next step based on the result:  
        ''' ** If version of FACTORY DB is greather than version of CUSTOMER DB, the Update Version process is executed to update DB 
        '''    structures and data
        ''' ** If version of FACTORY DB is equal to version of CUSTOMER DB, nothing to do, Update Version process is not executed
        ''' ** If version of FACTORY DB is less than version of CUSTOMER DB, the Update Version process is not executed
        ''' </summary>
        ''' <param name="pServerName">Name of the Server where CUSTOMER DB is installed</param>
        ''' <param name="pDataBaseName">CUSTOMER DB Name (Ax00)</param>
        ''' <param name="pDBLogin">DB Login</param>
        ''' <param name="pDBPassword">DB Password</param>
        ''' <returns>GlobalDataTO with the result of the Update Version procces: if the Update Version was successfully executed, the function
        '''          returns TRUE; otherwise it returns FALSE. If the Update Version is executed but an error is raised during execution, the 
        '''          Error is informed in fields ErrorCode and ErrorDescription in the GlobalDataTO, and HasError is set to TRUE
        ''' </returns>
        ''' <remarks>
        ''' Created by:  TR 
        ''' Modified by: XB 13/05/2013 - Inform parameter usSwVersion when calling function ConfigureDataBaseAfterUpdateVersion (to evaluate 
        '''                              required updates)
        '''              SA 20/10/2014 - BA-1944 ==> Inform parameter with the Application Version beign installed when calling function  
        '''                                          ConfigureDataBaseAfterUpdateVersion (to inform version updated in the name of the XML containing  
        '''                                          all changes made for the Update Version process)
        '''              IT 08/05/2015 - BA-2471
        ''' </remarks>
        Public Function UpdateDatabase(ByVal pServerName As String, ByVal pDataBaseName As String, ByVal pDBLogin As String, ByVal pDBPassword As String, _
                                       Optional pLoadingRSAT As Boolean = False, Optional pModel As String = "") As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim update As Boolean = False
            'Dim myDBDatabaseManager As New DataBaseManagerDelegate 'BA-2471: IT 08/05/2015

            GlobalBase.CreateLogActivity("On UpdateDatabase method ", "DataBaseUpdateManager.UpdateDatabase", EventLogEntryType.Information, False)

            Try
                'Compare the DB Version in CUSTOMER and FACTORY Databases
                Dim usSwVersion As String = String.Empty
                Dim srvSwVersion As String = String.Empty

                'BA-2471: IT 08/05/2015 (INI)
                Try
                    update = IsUpdateDatabaseNeeded()
                Catch ex As Exception
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = ex.Message
                    myGlobalDataTO.SetDatos = False
                End Try
                'BA-2471: IT 08/05/2015 (END)

                If (update) Then 'BA-2471: IT 08/05/2015
                    'If (False) Then 'BA-2471: IT 08/05/2015

                    'DL 18/01/2013 - Create an automatic restore point (subfolders RestorePoints and Previous only for User Sw)
                    Dim myGlobal As New GlobalDataTO
                    'Dim mySATUtil As New SATReportUtilities

                    LoadDataBaseVersions(usSwVersion, srvSwVersion) 'BA-2471: IT 08/05/2015


                    myGlobal = SATReportUtilities.CreateSATReport(GlobalEnumerates.SATReportActions.SAT_UPDATE, False, "", "", "", "", usSwVersion, True, pModel) 'BA-2471: IT 08/05/2015
                    If (Not myGlobal.HasError) Then
                        'Dim myDatabaseAdmin As New DataBaseManagerDelegate()

                        'TR 16/01/2013 v1.0.1 - Search new database backup file to restore on temporal db
                        Dim myBackUpFile As String = AppDomain.CurrentDomain.BaseDirectory & GlobalBase.BakDirectoryPath & GlobalBase.TempDBBakupFileName
                        If (IO.File.Exists(myBackUpFile)) Then
                            'Copy the file to the a new temporary directory and assign the value
                            myBackUpFile = CopyBackupToTempDirectory(myBackUpFile)

                            'Restore the temporary DB (Ax00TEM)
                            If (DataBaseManagerDelegate.RestoreDatabase(pServerName, GlobalBase.TemporalDBName, pDBLogin, pDBPassword, myBackUpFile)) Then 'BA-2471: IT 08/05/2015
                                GlobalBase.CreateLogActivity("Temporal Database restore Success ", "DataBaseUpdateManager.UpdateDatabase", _
                                                                EventLogEntryType.Information, False)

                                Dim myServer As Server = New Server(pServerName)
                                myServer.ConnectionContext.LoginSecure = False
                                myServer.ConnectionContext.Login = pDBLogin
                                myServer.ConnectionContext.Password = pDBPassword
                                myServer.ConnectionContext.DatabaseName = pDataBaseName 'BA-2471
                                myServer.ConnectionContext.BeginTransaction()

                                'SGM 18/02/2013
                                'Set the Update process is in progress
                                SystemInfoManager.IsUpdateProcess = True

                                'Create 'Previous' folder if not exists
                                'TODO: Do not use ProgramFiles folder for this stuff, as it violates UAC policy.
                                If (Not Directory.Exists(Application.StartupPath & GlobalBase.PreviousFolder)) Then
                                    Directory.CreateDirectory(Application.StartupPath & GlobalBase.PreviousFolder)
                                End If

                                'TR 16/01/2013 - Update the DB Structures (tables and views) 
                                myGlobalDataTO = UpdateDatabaseStructureAndData(pDataBaseName, myServer)
                                myServer.ConnectionContext.CommitTransaction()
                                myServer.ConnectionContext.BeginTransaction()

                                'TR 18/02/2013 - Update the DB Preloaded Data 
                                If (Not myGlobalDataTO.HasError) Then
                                    myGlobalDataTO = ConfigureDataBaseAfterUpdateVersion(myServer, usSwVersion, Application.ProductVersion, pLoadingRSAT)

                                    'If an error has been raised during execution of Update Version Process, inform the ErrorCode in the GlobalDataTO to return
                                    If (myGlobalDataTO.HasError) Then myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString
                                End If

                                'Validate if there was an Error during the Update Version Process
                                If (myGlobalDataTO.HasError) Then
                                    'ERROR CASE
                                    myServer.ConnectionContext.RollBackTransaction()

                                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString()
                                    myGlobalDataTO.SetDatos = False

                                    'Search for update log file on previous folder and add it to RSAT
                                    Dim myLogFile As String = String.Empty
                                    If (System.IO.File.Exists(Application.StartupPath & GlobalBase.PreviousFolder & GlobalBase.UpdateLogFile)) Then
                                        myLogFile = Application.StartupPath & GlobalBase.PreviousFolder & GlobalBase.UpdateLogFile
                                    End If

                                    'CREATE a RSAT and copy the XML file containing the LOG of the Update Process 
                                    myGlobal = SATReportUtilities.CreateSATReport(GlobalEnumerates.SATReportActions.SAT_UPDATE_ERR, False, String.Empty, _
                                                                         myLogFile, Application.StartupPath & GlobalBase.PreviousFolder, _
                                                                         "RSATUpdateError", usSwVersion, True, pModel) 'BA-2471: IT 08/05/2015

                                    'Remove the UpdateLog File because it is included on the RSAT
                                    If (File.Exists(myLogFile)) Then File.Delete(myLogFile)
                                Else
                                    'SUCCESS CASE
                                    myServer.ConnectionContext.CommitTransaction()

                                    myGlobalDataTO.SetDatos = True

                                    'If the process was successful delete the log file - SGM 18/02/2013
                                    If (System.IO.File.Exists(Application.StartupPath & GlobalBase.PreviousFolder & GlobalBase.UpdateLogFile)) Then
                                        System.IO.File.Delete(Application.StartupPath & GlobalBase.PreviousFolder & GlobalBase.UpdateLogFile)
                                    End If

                                    'TR 08/07/2013 - After Update Process execute the SHRINK Command (outside the DB Transaction)
                                    myGlobalDataTO = DataBaseManagerDelegate.ShrinkDatabase(pDataBaseName, myServer) 'BA-2471: IT 08/05/2015
                                End If
                            End If
                        End If
                    Else
                        'Restore point creation error
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString()
                        myGlobalDataTO.SetDatos = False
                        myGlobalDataTO.HasError = True
                    End If

                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " Innner: " + ex.InnerException.Message
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseUpdateManager.UpdateDatabase", EventLogEntryType.Error, False)
            End Try

            'Set the update process as finished - SGM 18/02/2013
            SystemInfoManager.IsUpdateProcess = False

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
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseUpdateManager.CopyBackupToTEMPDirectory", EventLogEntryType.Error, False)
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
        ''' <remarks>
        ''' Modified by: IT 08/05/2015 - BA-2471
        ''' </remarks>
        Public Function RemoveBackupFileAndTempDatabase(ByVal TempBackupFile As String, ByVal ServerName As String, _
                                                        ByVal DBLogin As String, ByVal DBPassword As String, _
                                                        ByVal TempDatabaseName As String) As Boolean
            Dim result As Boolean = False
            Try
                'validate if the backupfile still exist.
                If IO.File.Exists(TempBackupFile) Then
                    IO.File.Delete(TempBackupFile) 'remove temp. backup file.
                End If
                'Dim myDatabaseAdmin As New DataBaseManagerDelegate()
                result = DataBaseManagerDelegate.DeleteDatabase(ServerName, TempDatabaseName, DBLogin, DBPassword) 'BA-2471: IT 08/05/2015 
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message & " --- " & ex.InnerException.ToString(), _
                                                "DataBaseUpdateManager.RemoveBackupFileAndTempDatabase", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function


#End Region

#Region "Private Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDataBaseName"></param>
        ''' <param name="pServer"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function UpdateDatabaseStructureAndData(ByVal pDataBaseName As String, ByRef pServer As Server) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Get installed Database Version
                Dim packageId As String = String.Empty
                Dim installedDatabaseVersion As String = String.Empty
                Dim commonRevisionNumber As Integer = 0
                Dim dataRevisionNumber As Integer = 0

                Dim myVersionsDelegate As New VersionsDelegate
                myGlobalDataTO = myVersionsDelegate.GetVersionsData(Nothing)

                If Not myGlobalDataTO.HasError Then
                    Dim myVersionsDs As New VersionsDS
                    myVersionsDs = DirectCast(myGlobalDataTO.SetDatos, VersionsDS)

                    If myVersionsDs.tfmwVersions.Count > 0 Then
                        'Set the value to local varible.
                        'InstalledDataBaseVersion = CSng(myVersionsDS.tfmwVersions(0).DBSoftware.Replace(".", SystemInfoManager.OSDecimalSeparator))
                        packageId = myVersionsDs.tfmwVersions(0).PackageID
                        installedDatabaseVersion = Utilities.FormatToCorrectVersion(myVersionsDs.tfmwVersions(0).DBSoftware) 'myVersionsDs.tfmwVersions(0).DBSoftware.Replace(".", SystemInfoManager.OSDecimalSeparator)
                        commonRevisionNumber = myVersionsDs.tfmwVersions(0).DBCommonRevisionNumber
                        dataRevisionNumber = myVersionsDs.tfmwVersions(0).DBDataRevisionNumber
                    End If
                End If

                'Validate if could get the installed database version.
                If (Utilities.IsValidVersionFormat(installedDatabaseVersion)) Then

                    Dim encryptedFile As String = AppDomain.CurrentDomain.BaseDirectory & GlobalBase.InstallDBBTaskFilePath
                    Dim databaseUpdatesManager = LoadTaskListFile(encryptedFile)
                    myGlobalDataTO = ExecuteDatabaseUpdate(pDataBaseName, pServer, databaseUpdatesManager, packageId, installedDatabaseVersion, commonRevisionNumber, dataRevisionNumber)

                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseAdmin.UpdateDatabaseSructureAndData", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function LoadTaskListFile(encryptedfileName As String) As DatabaseUpdatesManager

            Dim dbManager As DatabaseUpdatesManager = Nothing

            Try
                Dim decryptedFileContent As String = Utilities.DescryptFile(encryptedfileName)

                If (Not String.IsNullOrEmpty(decryptedFileContent)) Then
                    Dim reader As XmlTextReader = New XmlTextReader(New StringReader(decryptedFileContent))
                    dbManager = DatabaseUpdatesManager.Deserialize(reader)
                    reader.Close()
                End If
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseUpdateManagerDelegate.LoadTaskListFile", EventLogEntryType.Error, False)
                Throw
            Finally

            End Try

            Return dbManager

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDataBaseName"></param>
        ''' <param name="pServer"></param>
        ''' <param name="databaseUpdatesManager"></param>
        ''' <param name="packageId"></param>
        ''' <param name="fromVersion"></param>
        ''' <param name="fromCommonRevisionNumber"></param>
        ''' <param name="fromDataRevisionNumber"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by: IT 13/07/2015 - BA-2693
        ''' </remarks>
        Private Function ExecuteDatabaseUpdate(ByVal pDataBaseName As String, ByRef pServer As Server, databaseUpdatesManager As DatabaseUpdatesManager, packageId As String, fromVersion As String, fromCommonRevisionNumber As Integer, fromDataRevisionNumber As Integer) As GlobalDataTO

            Dim myGlobalDataTo As New GlobalDataTO
            Dim results As New ExecutionResults

            Dim appDataBaseVersion As String = Utilities.FormatToCorrectVersion(GlobalBase.DataBaseVersion)

            'IT 13/07/2015 - BA-2693 (INI)
            If (fromVersion < appDataBaseVersion) Then
                Dim initialized = InitializeDatabase(pDataBaseName, pServer)
                If (Not initialized) Then
                    myGlobalDataTo.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString()
                    myGlobalDataTo.HasError = True
                End If
            End If
            'IT 13/07/2015 - BA-2693 (END)

            Dim updates = databaseUpdatesManager.GenerateUpdatePack(fromVersion, fromCommonRevisionNumber, fromDataRevisionNumber, appDataBaseVersion)
            If updates.Releases.Count > 0 Then
                results = updates.RunScripts(pDataBaseName, pServer, packageId)

                If (Not results.Success) Then
                    myGlobalDataTo.ErrorCode = GlobalEnumerates.Messages.INVALID_DATABASE_UPDATE.ToString()
                    myGlobalDataTo.HasError = True
                End If
            End If

            Return myGlobalDataTo

        End Function

        ''' <summary>
        ''' Once the DB Structures have been updated, the Update Version process to update preloaded data is executed. Steps:
        ''' 1) Update the Alarms definition table
        ''' 2) Reset the current WorkSession
        ''' 3) Update preloaded BioSystems Tests programming (all types)
        ''' </summary>
        ''' <param name="pServer">Name of the Server where CUSTOMER DB is installed</param>
        ''' <param name="pCustomerSwVersion">DB Version in CUSTOMER DB</param>
        ''' <param name="pFactorySwVersion">DB Version in FACTORY DB</param>
        ''' <param name="pLoadingRSAT">When TRUE, it indicates the function is executed while loading a RSAT and in this case, only step 1
        '''                            (update the Alarms definition table) is executed. When FALSE, it indicates the function is executed 
        '''                            for the Update Version process and all steps are executed
        '''                            </param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 16/01/2013
        ''' Modified by: TR 29/01/2013 - Added new optional parameter pLoadingRSAT to validate if user is loading a RSAT, in which case
        '''                              the Reset WorSession and the Factory Tests Programming updates are not executed
        '''              TR 18/02/2013 - Added the new parameter pServer to implement the required DB Transaction
        '''              XB 13/05/2013 - Added parameter pCustomerSwVersion to inform the DB Version in the CUSTOMER DB
        '''              SA 17/10/2014 - BA-1944 ==> Added parameter pFactorySwVersion to inform the DB Version in the FACTORY DB. Call the  
        '''                                          function that executes the new Update Version process (SetFactoryTestProgrammingNEW) and, if
        '''                                          the process finishes successfully, write in Application Log directory the XML file containing all
        '''                                          changes made for the Update Version process in the CUSTOMER DB  
        ''' </remarks>
        Private Function ConfigureDataBaseAfterUpdateVersion(ByRef pServer As Server, ByVal pCustomerSwVersion As String, ByVal pFactorySwVersion As String, _
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

                    'Update Alarms definition
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
                                'myGlobal = myWS.ResetWS(Nothing, analyzerID, worksessionID)
                                myGlobal = myWS.ResetWS(pServer.ConnectionContext.SqlConnectionObject(), analyzerID, worksessionID, analyzerModel) 'AG 17/11/2014 BA-2065 inform analyzerModel
                            Else
                                'myGlobal = myWS.ResetWSNEW(Nothing, analyzerID, worksessionID)
                                myGlobal = myWS.ResetWSNEW(pServer.ConnectionContext.SqlConnectionObject(), analyzerID, worksessionID, analyzerModel, False, True, True) 'AG 17/11/2014 BA-2065 inform analyzerModel
                            End If

                            'Update preloaded BioSystems Tests
                            If (Not myGlobal.HasError) Then
                                Dim updateTest As New UpdatePreloadedFactoryTestDelegate

                                myGlobal = updateTest.SetFactoryTestProgrammingNEW(pServer.ConnectionContext.SqlConnectionObject(), pCustomerSwVersion, pFactorySwVersion)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    Dim myUpdateVersionChangesList As UpdateVersionChangesDS = DirectCast(myGlobal.SetDatos, UpdateVersionChangesDS)

                                    Dim myDirName As String = Application.StartupPath & GlobalBase.XmlLogFilePath
                                    Dim myFileName As String = GlobalBase.UpdateVersionProcessLogFileName

                                    'If needed, create the final directory... 
                                    If (Not IO.Directory.Exists(myDirName)) Then IO.Directory.CreateDirectory(myDirName)

                                    'If there is a previous version of the UpdateVersion file, delete it 
                                    If (IO.File.Exists(myDirName & myFileName)) Then IO.File.Delete(myDirName & myFileName)

                                    'Finally, write the XML
                                    myUpdateVersionChangesList.WriteXml(myDirName & myFileName)
                                End If
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.ConfigureDataBaseAfterUpdateVersion", EventLogEntryType.Error, False)
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
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.GetAnalyzerAndWorksessionInfo", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: IT 08/05/2015 - BA-2471
        ''' </remarks>
        Private Function IsUpdateDatabaseNeeded() As Boolean

            Dim encryptedFile As String = AppDomain.CurrentDomain.BaseDirectory & GlobalBase.InstallDBBTaskFilePath
            Dim databaseUpdatesManager = LoadTaskListFile(encryptedFile)

            Dim myGlobalDataTO As New GlobalDataTO
            Dim update As Boolean = False

            'Get Application Version and Database Version from CUSTOMER DB
            Dim myVersionsDelegate As New VersionsDelegate
            myGlobalDataTO = myVersionsDelegate.GetVersionsData(Nothing)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myVersionsDs As VersionsDS = DirectCast(myGlobalDataTO.SetDatos, VersionsDS)

                If (myVersionsDs.tfmwVersions.Count > 0) Then

                    'Dim myDBVersion As Double = CDbl(myVersionsDS.tfmwVersions(0).DBSoftware.Replace(".", SystemInfoManager.OSDecimalSeparator)) 'BA-2471
                    Dim myDbVersion As String = Utilities.FormatToCorrectVersion(myVersionsDs.tfmwVersions(0).DBSoftware)
                    Dim appDataBaseVersion As String = Utilities.FormatToCorrectVersion(GlobalBase.DataBaseVersion)

                    Dim myDbCommonRevisonNumber As Integer = myVersionsDs.tfmwVersions(0).DBCommonRevisionNumber
                    Dim myDbDataRevisionNumber As Integer = myVersionsDs.tfmwVersions(0).DBDataRevisionNumber

                    Dim dbCommonRevisionNumber As Integer = 0
                    Dim dbDataRevisionNumber As Integer = 0

                    If (Utilities.IsValidVersionFormat(myDbVersion) And Utilities.IsValidVersionFormat(appDataBaseVersion)) Then
                        'Compare the DB Versions
                        If (myDbVersion = appDataBaseVersion) Then
                            Dim release = databaseUpdatesManager.Releases.FirstOrDefault(Function(r) r.Version = myDbVersion)

                            If (release IsNot Nothing) Then
                                Dim dbCommonRevision = release.CommonRevisions.LastOrDefault()
                                Dim dbDataRevision = release.DataRevisions.LastOrDefault()

                                If (dbCommonRevision IsNot Nothing) Then dbCommonRevisionNumber = dbCommonRevision.RevisionNumber
                                If (dbDataRevision IsNot Nothing) Then dbDataRevisionNumber = dbDataRevision.RevisionNumber

                                If ((dbCommonRevisionNumber > myDbCommonRevisonNumber) Or (dbDataRevisionNumber > myDbDataRevisionNumber)) Then
                                    update = True
                                ElseIf ((dbCommonRevisionNumber < myDbCommonRevisonNumber) Or (dbDataRevisionNumber < myDbDataRevisionNumber)) Then
                                    'This case is an error because CUSTOMER DB Version is greather than FACTORY DB Version
                                    Throw New Exception(GlobalEnumerates.Messages.INVALID_DATABASE_VERSION.ToString())
                                End If
                            End If
                        ElseIf (myDbVersion < appDataBaseVersion) Then
                            update = True
                        ElseIf (myDbVersion > appDataBaseVersion) Then
                            'This case is an error because CUSTOMER DB Version is greather than FACTORY DB Version
                            Throw New Exception(GlobalEnumerates.Messages.INVALID_DATABASE_VERSION.ToString())
                        End If
                    End If
                End If
            End If

            Return update

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="usSwVersion"></param>
        ''' <param name="srvSwVersion"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function LoadDataBaseVersions(ByRef usSwVersion As String, ByRef srvSwVersion As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Get Application Version and Database Version from CUSTOMER DB
                Dim myVersionsDelegate As New VersionsDelegate
                myGlobalDataTO = myVersionsDelegate.GetVersionsData(Nothing)

                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myVersionsDS As VersionsDS = DirectCast(myGlobalDataTO.SetDatos, VersionsDS)

                    If (myVersionsDS.tfmwVersions.Count > 0) Then
                        'Inform the USER and SERVICE SW Versions to return
                        usSwVersion = myVersionsDS.tfmwVersions(0).UserSoftware
                        srvSwVersion = myVersionsDS.tfmwVersions(0).ServiceSoftware

                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseUpdateManager.LoadDataBaseVersions", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        Private Function InitializeDatabase(ByVal dataBaseName As String, ByRef server As Server) As Boolean
            Try
                Const script As String = " EXECUTE [dbo].[InitializeAx00Database] "

                If server Is Nothing Then
                    'Create a new server control
                    server = New Server(DAOBase.DBServer)
                    server.ConnectionContext.LoginSecure = False
                    server.ConnectionContext.Login = DAOBase.DBLogin
                    server.ConnectionContext.Password = DAOBase.DBPassword
                End If

                Dim connBuilder As New System.Data.SqlClient.SqlConnectionStringBuilder(GlobalBase.UpdateDatabaseConnectionString)
                Dim tempDataBaseName = connBuilder.InitialCatalog

                Return DataBaseManagerDelegate.RunDatabaseScript(server, tempDataBaseName, script)

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)
                Throw
            End Try
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pServerName"></param>
        ''' <param name="pDataBaseName"></param>
        ''' <param name="DBLogin"></param>
        ''' <param name="DBPassword"></param>
        ''' <returns>Return if exists the required database for current analyzer model</returns>
        ''' <remarks>
        ''' Modified by: IT 11/06/2015 - BA-2600
        '''              IT 15/07/2015 - BA-2693
        ''' </remarks>
        Private Function ValidateDatabaseName(ByVal pServerName As String, ByVal pDataBaseName As String, ByVal DBLogin As String, ByVal DBPassword As String) As Boolean

            'if A200 or A400 exist
            If Not DataBaseManagerDelegate.DataBaseExist(pServerName, pDataBaseName, DBLogin, DBPassword) Then 'BA-2471: IT 08/05/2015
                'Ax00 Exist
                If DataBaseManagerDelegate.DataBaseExist(pServerName, GlobalBase.CommonDatabaseName, DBLogin, DBPassword) Then 'BA-2471: IT 08/05/2015
                    'The Ax00 DB belong to the same analyzer A200 or A400 active=true
                    If IsSameAnalyzer(pServerName, GlobalBase.CommonDatabaseName, DBLogin, DBPassword) Then
                        'if we start the same instance we rename the database, to a intance name and we follow with the update process.
                        DataBaseManagerDelegate.RenameDBByModel(pServerName, GlobalBase.CommonDatabaseName, pDataBaseName, DBLogin, DBPassword)
                        Return True
                    End If
                End If
                Return False
            Else
                Return True
            End If

        End Function

#Region "TEMPORARY FUNCTIONS TO UPDATE STRUCTURE FOR v3.0.1"
        ''' <summary>
        ''' Execute scripts that cannot be included in the UpdateVersion process (tables that have to be already updated when the UpdateVersion process starts):
        ''' ** Script to add new column ThreadID to table tfmwApplicationLog
        ''' </summary>
        ''' <returns>Boolean value indicating if the scripts execution finished successfully (True) or with error (False)</returns>
        ''' <remarks>
        ''' Created by: SA 16/05/2014 - BT #1632 ==> This change cannot be included in the normal Update Version process, due to it changes the structure of 
        '''                                          Application Log table, and several functions write in the Log before the process update the table
        ''' </remarks>
        Public Function ExecuteScriptsBeforeUpdate() As Boolean
            Dim sqlExecResult As Boolean = True
            Try
                Dim myDBManager As New DBManager
                sqlExecResult = myDBManager.AddThreadIDColToApplicationLogTable()

            Catch ex As Exception
                sqlExecResult = False
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManagerDelegate.ExecuteScriptsBeforeUpdate", EventLogEntryType.Error, False)
            End Try
            Return sqlExecResult
        End Function
#End Region

#End Region

    End Class
End Namespace


