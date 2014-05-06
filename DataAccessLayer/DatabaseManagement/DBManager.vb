Option Explicit On
Option Strict On

Imports Microsoft.Win32
Imports System.Data.SqlClient
Imports System.ServiceProcess
Imports Biosystems.Ax00.Global
Imports Microsoft.SqlServer.Management.Smo
Imports Microsoft.SqlServer.Management.Smo.Wmi
Imports Microsoft.SqlServer.Management.Sdk
Imports Microsoft.SqlServer.Management.Common

Namespace Biosystems.Ax00.DAL

    ''' <summary>
    ''' Class incharge to Restore and BackUp Database.
    ''' </summary>
    ''' <remarks>
    ''' This is an interesting web page http://www.sqldbatips.com/showarticle.asp?ID=40, related 
    ''' to all the database management through SMO.
    ''' </remarks>
    Public Class DBManager

        ''' <summary>
        ''' Create a Backup file into SQL instance Backup Directory containing the
        '''  Database name and the creation datetime. 
        ''' </summary>
        ''' <param name="ServerName">Server name to connect</param>
        ''' <param name="DataBaseName">Database name to open</param>
        ''' <returns>True if ok OR False if Fail.</returns>
        ''' <remarks>
        ''' Modified by: RH 10/11/2010
        ''' Based on http://msdn.microsoft.com/en-us/library/ms162133(v=SQL.105).aspx
        '''              TR 03/05/2011 -Add the optional parammeter pInstalationProcess
        ''' </remarks>
        Public Shared Function BackUpDataBase(ByVal ServerName As String, ByVal DataBaseName As String, _
                                              ByVal DBLogin As String, ByVal DBPassword As String, _
                                       Optional ByVal pInstalationProcess As Boolean = False) As Boolean
            Dim result As Boolean = False ' keep the operation result.
            Dim myLogAcciones As New ApplicationLogManager()
            Try
                Dim MyServer As Server = New Server(ServerName) ' instance of SQL server

                'RH 17/05/2011
                MyServer.ConnectionContext.LoginSecure = False
                MyServer.ConnectionContext.Login = DBLogin
                MyServer.ConnectionContext.Password = DBPassword

                'Reference the DataBaseName database.
                Dim db As Database
                db = MyServer.Databases(DataBaseName)
                Dim MyBackUpDirectory As String = ""
                'Validate if i's intallation process to set the previous installation folder
                If pInstalationProcess Then
                    If Not IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory & "\PreviousInstallation") Then
                        'Create the directory if do not exist.
                        IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory & "\PreviousInstallation")
                    End If
                    MyBackUpDirectory = AppDomain.CurrentDomain.BaseDirectory & "PreviousInstallation\" & DataBaseName & ".bak"
                Else
                    MyBackUpDirectory = MyServer.BackupDirectory & "\" & DataBaseName & Now.ToString("ddMMyyyyHHmm") & _
                                                                       "v" & MyServer.Information.Version.Major & ".bak"
                End If

                Dim BackupFileName As String = MyBackUpDirectory

                'Dim BackupFileName As String = MyServer.BackupDirectory & "\" & DataBaseName & Now.ToString("ddMMyyyyHHmm") & _
                '        "v" & MyServer.Information.Version.Major & ".bak"
                'TR 03/05/2011 -Change the directory to the TEMP instead sql backupdir

                'Define a Backup object variable. 
                Dim myBackUp As New Backup()
                'Specify the type of backup, the description, the name, and the database to be backed up.
                myBackUp.Action = BackupActionType.Database
                myBackUp.BackupSetDescription = String.Format("Full backup of {0}/{1}", ServerName, DataBaseName)
                myBackUp.BackupSetName = DataBaseName & " Backup"
                myBackUp.Database = DataBaseName

                'Declare a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.
                Dim bdi As BackupDeviceItem
                bdi = New BackupDeviceItem(BackupFileName, DeviceType.File)

                'Add the device to the Backup object.
                myBackUp.Devices.Add(bdi)

                'Set the Incremental property to False to specify that this is a full database backup.
                myBackUp.Incremental = False

                'Specify that the log must be truncated after the backup is complete.
                myBackUp.LogTruncation = BackupTruncateLogType.Truncate

                'Run SqlBackup to perform the full database backup on the instance of SQL Server.
                myBackUp.SqlBackup(MyServer)

                'Remove the backup device from the Backup object.
                myBackUp.Devices.Remove(bdi)

                result = True ' if ok then set true to the result.

                myLogAcciones.CreateLogActivity("Create " & BackupFileName & " BackUp Database OK", "BackUpDataBase", EventLogEntryType.Information, False)

            Catch ex As Exception
                Dim Message As String
                If ex.InnerException IsNot Nothing AndAlso ex.InnerException.InnerException IsNot Nothing Then
                    Message = ex.InnerException.InnerException.Message
                Else
                    Message = ex.Message
                End If

                myLogAcciones.CreateLogActivity(Message, "BackUpDataBase", EventLogEntryType.Error, False)

                'Throw ex  'Commented line RH 10/11/2010
                'Do prefer using an empty throw when catching and re-throwing an exception.
                'This is the best way to preserve the exception call stack.
                'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
                'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
                Throw

            End Try

            Return result
        End Function

        ''' <summary>
        ''' Create a Backup file into SQL instance Backup Directory containing the
        '''  Database name and the creation datetime. 
        ''' </summary>
        ''' <param name="ServerName">Server name to connect</param>
        ''' <param name="DataBaseName">Database name to open</param>
        ''' <param name="DirectoryTo">Directory where to move the generated back up file</param>
        ''' <returns>True if ok OR False if Fail.</returns>
        ''' <remarks>
        ''' Modified by: RH 10/11/2010
        ''' Based on http://msdn.microsoft.com/en-us/library/ms162133(v=SQL.105).aspx
        ''' </remarks>
        Public Shared Function BackUpDataBaseAndMoveBkFile(ByVal ServerName As String, ByVal DataBaseName As String, _
                                                           ByVal DBLogin As String, ByVal DBPassword As String, _
                                                           ByVal DirectoryTo As String) As Boolean
            Dim result As Boolean = False ' keep the operation result.
            Dim myLogAcciones As New ApplicationLogManager()

            Try
                Dim MyServer As Server = New Server(ServerName) ' instance of SQL server

                'RH 17/05/2011
                MyServer.ConnectionContext.LoginSecure = False
                MyServer.ConnectionContext.Login = DBLogin
                MyServer.ConnectionContext.Password = DBPassword

                Dim BackupDirectory As String = MyServer.BackupDirectory & "\"
                Dim BackupFileName As String = DataBaseName & Now.ToString("ddMMyyyyHHmm") & _
                        "v" & MyServer.Information.Version.Major & ".bak"

                'Reference the DataBaseName database.
                Dim db As Database
                db = MyServer.Databases(DataBaseName)

                'Define a Backup object variable. 
                Dim myBackUp As Backup = New Backup()

                'Specify the type of backup, the description, the name, and the database to be backed up.
                myBackUp.Action = BackupActionType.Database
                myBackUp.BackupSetDescription = String.Format("Full backup of {0}/{1}", ServerName, DataBaseName)
                myBackUp.BackupSetName = DataBaseName & " Backup"
                myBackUp.Database = DataBaseName

                'Declare a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.
                Dim bdi As BackupDeviceItem
                bdi = New BackupDeviceItem(BackupDirectory & BackupFileName, DeviceType.File)

                'Add the device to the Backup object.
                myBackUp.Devices.Add(bdi)

                'Set the Incremental property to False to specify that this is a full database backup.
                myBackUp.Incremental = False

                'Specify that the log must be truncated after the backup is complete.
                myBackUp.LogTruncation = BackupTruncateLogType.Truncate

                'Run SqlBackup to perform the full database backup on the instance of SQL Server.
                myBackUp.SqlBackup(MyServer)

                'Remove the backup device from the Backup object.
                myBackUp.Devices.Remove(bdi)

                If Not DirectoryTo.EndsWith("\") Then
                    DirectoryTo = DirectoryTo & "\"
                End If

                If Not IO.Directory.Exists(DirectoryTo) Then ' validate if the directory exist to create it.
                    IO.Directory.CreateDirectory(DirectoryTo) 'create the destination Directory
                End If

                If IO.File.Exists(DirectoryTo & BackupFileName) Then ' validate if the file exist to delete
                    IO.File.Delete(DirectoryTo & BackupFileName) 'delete the file 
                End If

                IO.File.Move(BackupDirectory & BackupFileName, DirectoryTo & BackupFileName) ' move the backup file into the destination directory

                result = True ' if ok then set true to the result.

                myLogAcciones.CreateLogActivity("Create " & BackupFileName & " BackUp Database OK", "BackUpDataBase", EventLogEntryType.Information, False)

            Catch ex As Exception
                Dim Message As String
                If ex.InnerException IsNot Nothing AndAlso ex.InnerException.InnerException IsNot Nothing Then
                    Message = ex.InnerException.InnerException.Message
                Else
                    Message = ex.Message
                End If

                myLogAcciones.CreateLogActivity(Message, "BackUpDataBaseAndMoveBkFile", EventLogEntryType.Error, False)

                'Throw ex  'Commented line RH 10/11/2010
                'Do prefer using an empty throw when catching and re-throwing an exception.
                'This is the best way to preserve the exception call stack.
                'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
                'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
                Throw

            End Try

            Return result
        End Function

        ''' <summary>
        ''' Restore a database from an specific backup file.
        ''' </summary>
        ''' <param name="ServerName">Server name to connect</param>
        ''' <param name="DataBaseName">DataBase Name to restore</param>
        ''' <param name="BackUpFileName">contains the file name and the file path.</param>
        ''' <returns>True if OK (OR) False if Fail</returns>
        ''' <remarks>
        ''' Modified by: RH 10/11/2010
        '''              RH 18/05/2011
        ''' </remarks>
        Public Shared Function RestoreDataBase(ByVal ServerName As String, ByVal DataBaseName As String, _
                                               ByVal DBLogin As String, ByVal DBPassword As String, _
                                               ByVal BackUpFileName As String) As Boolean

            Dim result As Boolean = False ' keep the operation result.
            Dim myLogAcciones As New ApplicationLogManager()

            Try
                Dim MyServer As Server = New Server(ServerName) 'SQL server instance

                'RH 17/05/2011
                MyServer.ConnectionContext.LoginSecure = False
                MyServer.ConnectionContext.Login = DBLogin
                MyServer.ConnectionContext.Password = DBPassword
                MyServer.ConnectionContext.DatabaseName = "master"

                If Not DataBaseExist(ServerName, DataBaseName, DBLogin, DBPassword) Then
                    'http://msdn.microsoft.com/en-us/library/ms162577.aspx
                    Dim myTempDataBase As New Database(MyServer, DataBaseName)
                    myTempDataBase.Create()
                End If

                'TR 08/11/2011 - THIS CODE IS AN Implementation Is commented wating to be accepted.
                'Validate if DB name is Ax00 to set the Folder where the DB is gonna be restore. 
                '(inside the application path on Data directory)
                'Dim DataPathFile As String = ""
                'If DataBaseName = "Ax00" Then
                '    DataPathFile = AppDomain.CurrentDomain.BaseDirectory & "\Data\" & MyServer.Databases(DataBaseName).FileGroups(0).Files(0).Name & ".mdf"
                'Else
                '    DataPathFile = MyServer.Databases(DataBaseName).FileGroups(0).Files(0).FileName
                'End If
                'TR 08/11/2011 -END

                'Dim DataDirectory As String = MyServer.Databases(DataBaseName).PrimaryFilePath
                Dim DataPathFile As String = MyServer.Databases(DataBaseName).FileGroups(0).Files(0).FileName
                Dim LogPathFile As String = DataPathFile.Replace(".mdf", "_log.ldf")
                Dim LogicalFile As String = MyServer.Databases(DataBaseName).FileGroups(0).Files(0).Name
                Dim MyRestore As Restore = New Restore()

                'Declare a BackupDeviceItem by supplying the backup device file name in the constructor, and the type of device is a file.
                Dim bdi As BackupDeviceItem
                bdi = New BackupDeviceItem(BackUpFileName, DeviceType.File)

                'Add the device to the Restore object.
                MyRestore.Devices.Add(bdi)

                MyRestore.Action = RestoreActionType.Database
                MyRestore.Database = DataBaseName ' database to restore
                MyRestore.NoRecovery = False
                MyRestore.ReplaceDatabase = True

                'Move the file in case the backup was made with a diferent operating system's "Programs files" directory.
                MyRestore.RelocateFiles.Add(New RelocateFile(LogicalFile, DataPathFile))
                MyRestore.RelocateFiles.Add(New RelocateFile(LogicalFile & "_log", LogPathFile))

                If SetDataBaseSingleUser(ServerName, DataBaseName, DBLogin, DBPassword) Then

                    'Now there is no active connection to the DB
                    MyRestore.SqlRestore(MyServer) 'Start restore. Set one connection to the DB.

                    'Remove the backup device from the Backup object.
                    MyRestore.Devices.Remove(bdi)

                    'Close connection
                    MyServer.ConnectionContext.Disconnect()

                    result = True

                    myLogAcciones.CreateLogActivity("Restore " & BackUpFileName & " Database OK", "RestoreDataBase", EventLogEntryType.Information, False)
                Else
                    Throw New Exception("Unable to set DB to Single User.")
                End If

            Catch ex As Exception
                Dim Message As String
                If ex.InnerException IsNot Nothing AndAlso ex.InnerException.InnerException IsNot Nothing Then
                    Message = ex.InnerException.InnerException.Message
                Else
                    Message = ex.Message
                End If

                myLogAcciones.CreateLogActivity(Message, "RestoreDataBase", EventLogEntryType.Error, False)

                'Throw ex  'Commented line RH 10/11/2010
                'Do prefer using an empty throw when catching and re-throwing an exception.
                'This is the best way to preserve the exception call stack.
                'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
                'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
                Throw

            Finally
                SetDataBaseMultiUser(ServerName, DataBaseName, DBLogin, DBPassword) 'SG 18/10/10

            End Try

            Return result
        End Function

        ''' <summary>
        ''' Executes scripts in a selected server.
        ''' </summary>
        ''' <param name="pServer">Server Name.</param>
        ''' <param name="SQLScripts">SQL Script to run</param>
        ''' <returns>True if succes, False if Fail.</returns>
        ''' <remarks></remarks>
        Public Shared Function ExecuteScripts(ByVal pServer As Server, ByVal DataBaseName As String, _
                                              ByVal SQLScripts As String, _
                                              Optional ByVal pTransactionCommand As String = "") As Boolean
            Dim result As Boolean = False

            Try
                'Dim MyServer As Server = New Server(ServerName)
                Dim MyServer As Server = pServer

                'Set the master database to allow modification into database.
                Dim db As Database = MyServer.Databases(DataBaseName)

                db.ExecuteNonQuery(SQLScripts)
                'MyServer.Refresh()

                result = True

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.ExecuteScripts", EventLogEntryType.Error, False)

            End Try

            Return result
        End Function

        ''' <summary>
        ''' Validate if a database exist on the server.
        ''' </summary>
        ''' <param name="ServerName">Server Name to connect</param>
        ''' <param name="DataBaseName">Database name to seach for.</param>
        ''' <returns>True if database Exist, False if database do not exist</returns>
        ''' <remarks>
        ''' Modified by: XB 01/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
        ''' </remarks>
        Public Shared Function DataBaseExist(ByVal ServerName As String, ByVal DataBaseName As String, _
                                             ByVal DBLogin As String, ByVal DBPassword As String) As Boolean
            Dim result As Boolean = False

            Try
                Dim MyServer As Server = New Server(ServerName) 'SQL server instance

                'RH 17/05/2011
                MyServer.ConnectionContext.LoginSecure = False
                MyServer.ConnectionContext.Login = DBLogin
                MyServer.ConnectionContext.Password = DBPassword

                For Each myDataBase As Database In MyServer.Databases 'Go through all installed Databases.
                    'If myDataBase.Name.ToUpper().Trim() = DataBaseName.ToUpper().Trim() Then ' validate databases name.
                    If myDataBase.Name.ToUpperBS().Trim() = DataBaseName.ToUpperBS().Trim() Then ' validate databases name.
                        result = True 'when found then set the result to true and break the for loop.
                        Exit For
                    End If
                Next

                MyServer.ConnectionContext.Disconnect()

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.DataBaseExist", EventLogEntryType.Error, False)

            End Try

            Return result
        End Function

        ''' <summary>
        ''' Detects if the server engine is Express 2005
        ''' </summary>
        ''' <param name="ServerName">Server Name</param>
        ''' <returns>
        ''' True if ServerName is Express 2005, False otherwise
        ''' </returns>
        ''' <remarks>
        ''' Created by: TR and RH some time ago
        ''' </remarks>
        Public Shared Function IsSQLServer2005(ByVal ServerName As String, _
                                               ByVal DBLogin As String, ByVal DBPassword As String) As Boolean
            Dim result As Boolean = False
            Try
                Dim MyServer As Server = New Server(ServerName)

                'RH 17/05/2011
                MyServer.ConnectionContext.LoginSecure = False
                MyServer.ConnectionContext.Login = DBLogin
                MyServer.ConnectionContext.Password = DBPassword

                Dim Product As String = MyServer.Information.Product
                Dim Edition As String = MyServer.Information.Edition
                Dim Version As Integer = MyServer.Information.Version.Major

                result = (Product = "Microsoft SQL Server") AndAlso (Edition = "Express Edition") AndAlso (Version = 9)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.IsSQLServer2005", EventLogEntryType.Error, False)

            End Try

            Return result
        End Function

        ''' <summary>
        ''' Delete a database from an specific server.
        ''' </summary>
        ''' <param name="pServerName">Server Name</param>
        ''' <param name="DataBaseName">Database name to Delete</param>
        ''' <returns>True if Delete Succes, False if Delete fail.</returns>
        ''' <remarks></remarks>
        Public Shared Function DropDatabaseFromServer(ByVal pServerName As String, ByVal DataBaseName As String, _
                                                      ByVal DBLogin As String, ByVal DBPassword As String) As Boolean
            Dim result As Boolean = False

            Try
                If DataBaseExist(pServerName, DataBaseName, DBLogin, DBPassword) Then ' validate if the database exist.
                    Dim MyServer As Server = New Server(pServerName) 'SQL server instance.

                    'RH 17/05/2011
                    MyServer.ConnectionContext.LoginSecure = False
                    MyServer.ConnectionContext.Login = DBLogin
                    MyServer.ConnectionContext.Password = DBPassword

                    'TR 10/01/2013
                    'Close all open connection to the DB. Put DB offline.
                    Dim myTSQL As String = String.Format("ALTER DATABASE {0} SET OFFLINE WITH ROLLBACK IMMEDIATE", DataBaseName)
                    result = ExecuteScripts(MyServer, "master", myTSQL)

                    'Put DB online
                    myTSQL = String.Format("ALTER DATABASE {0} SET ONLINE", DataBaseName)
                    result = ExecuteScripts(MyServer, "master", myTSQL)
                    'TR 10/01/2013
                    'SQL command to drop the database, first close the connection and then drop the database.
                    Dim DropDatabase As String = "DROP DATABASE " & DataBaseName
                    result = RunDatabaseScript(MyServer, "master", DropDatabase)
                    'Dim MyDataBase As New Database(MyServer, DataBaseName) 'Database instance.
                    ''MyDataBase.dr
                    'MyDataBase.Drop() ' Eliminate the database.
                    'result = True

                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.DropDatabaseFromServer", EventLogEntryType.Error, False)

            End Try

            Return result
        End Function

        ''' <summary>
        ''' Updates the database structure and data.
        ''' </summary>
        ''' <param name="pServer">Server Name </param>
        ''' <param name="SQLScript">Script with structure modification.</param>
        ''' <returns>True if success or False if fail</returns>
        ''' <remarks></remarks>
        Public Shared Function RunDatabaseScript(ByVal pServer As Server, ByVal DataBaseName As String, _
                                             ByVal SQLScript As String) As Boolean
            Dim result As Boolean = False
            Dim myLogAcciones As New ApplicationLogManager()

            Try
                If Not String.IsNullOrEmpty(SQLScript) Then ' validate if not empty
                    result = ExecuteScripts(pServer, DataBaseName, SQLScript) ' execute the script and validate the result(True/False)
                    myLogAcciones.CreateLogActivity("Script success", "DataBaseManager.RunDatabaseScript", EventLogEntryType.Information, False)
                Else
                    myLogAcciones.CreateLogActivity("Empty Script. Failed to run it", "DataBaseManager.RunDatabaseScript", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.RunDatabaseScript", EventLogEntryType.Error, False)

            End Try

            Return result
        End Function

        ''' <summary>
        ''' Start SQL Services (SqlServer, SqlBrowser and SqlWriter) in case they are not activated
        ''' </summary>
        ''' <returns>Boolean value indicating if the three SQL Services are or have been started</returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: RH 16/11/2010
        '''              XB 01/02/2013 - BT #1112 ==> Upper conversions must use Invariant Culture Info
        '''              SA 10/02/2014 - BT #1495 ==> Check also the status of SQLWriter service and Start it in case it is Stopped.
        '''                                           When services SQLServer and SQLBrowser are Paused, the action to apply is Resume(); action Start
        '''                                           works only when the service state is Stopped. Changed the way to wait for the start of all SQL Services 
        '''                                           (previous time was short when one of the services being started was SQL Server)
        ''' </remarks>
        Public Shared Function StartSQLService(ByVal pServerName As String) As Boolean
            Dim result As Boolean = False
            Dim myLogAcciones As New ApplicationLogManager()

            Try
                'Declare and create an instance of the ManagedComputer object that represents the WMI Provider services.
                Dim mc As New ManagedComputer()

                If (pServerName.Contains("\")) Then pServerName = pServerName.Substring(pServerName.LastIndexOf("\") + 1, pServerName.Length - pServerName.LastIndexOf("\") - 1)
                Dim serviceName As String = "MSSQL$" & pServerName.ToUpperBS()

                'Reference the Microsoft SQL Server service.
                Dim sqlService As Service
                sqlService = mc.Services(serviceName)

                'Reference the Microsoft SQL Server service.
                Dim sqlBrowser As Service
                sqlBrowser = mc.Services("SQLBrowser")

                'Reference the Microsoft SQL Writer service
                Dim sc As ServiceController = New ServiceController("SqlWriter")

                'Check if the three services are RUNNING
                result = (sqlService.ServiceState = ServiceState.Running) AndAlso (sqlBrowser.ServiceState = ServiceState.Running) AndAlso _
                         (sc.Status = ServiceControllerStatus.Running)
                If (result) Then Return True 'Nothing to do. The services are running.

                '*****************************************************************'
                '** Here there is at least one service stopped, so start it up! **'
                '*****************************************************************'
                'Start the SQL Engine Service in case it is Paused or Stopped, and force it to Start automatically
                sqlService.StartMode = Wmi.ServiceStartMode.Auto
                sqlService.Alter() 'Update the service properties

                If (sqlService.ServiceState = ServiceState.Stopped) Then
                    sqlService.Start()
                ElseIf (sqlService.ServiceState = ServiceState.Paused) Then
                    sqlService.Resume()
                End If

                'Start the SQLBrowser Service in case it is Paused or Stopped, and force it to Start automatically
                sqlBrowser.StartMode = Wmi.ServiceStartMode.Auto
                sqlBrowser.Alter() 'Update the service properties

                If (sqlBrowser.ServiceState = ServiceState.Stopped) Then
                    sqlBrowser.Start()
                ElseIf (sqlBrowser.ServiceState = ServiceState.Paused) Then
                    sqlBrowser.Resume()
                End If

                'Start the SQLWriter Service in case it is Stopped (Note -> Pause action is not available for this service)
                If (sc.Status = ServiceControllerStatus.Stopped) Then sc.Start()

                'Wait for the servers. They are starting...
                Dim i As Integer = 0
                Do Until (sqlService.ServiceState = ServiceState.Running AndAlso sqlBrowser.ServiceState = ServiceState.Running AndAlso _
                          sc.Status = ServiceControllerStatus.Running) OrElse (i = 1000)
                    sqlService.Refresh() 'Refresh SQL Server service to update service status 
                    sqlBrowser.Refresh() 'Refresh SQL Browser service to update service status
                    sc.Refresh()         'Refresh SQL Writer service to update service status

                    i += 1
                Loop

                'Function return TRUE only when all Services are RUNNING...
                result = (sqlService.ServiceState = ServiceState.Running) AndAlso (sqlBrowser.ServiceState = ServiceState.Running) AndAlso _
                         (sc.Status = ServiceControllerStatus.Running)
            Catch ex As Exception
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.StartSQLService", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Stop SQL Services: SqlServer, SqlBrowser and SqlWriter
        ''' </summary>
        ''' <returns>Boolean value indicating if the three SQL Services have been stopped</returns>
        ''' <remarks>
        ''' Created by: SA 06/02/2014 - BT #1495
        ''' </remarks>
        Public Shared Function StopSQLService(ByVal pServerName As String) As Boolean
            Dim result As Boolean = False
            Dim myLogAcciones As New ApplicationLogManager()

            Try
                'Declare and create an instance of the ManagedComputer object that represents the WMI Provider services.
                Dim mc As New ManagedComputer()

                If (pServerName.Contains("\")) Then pServerName = pServerName.Substring(pServerName.LastIndexOf("\") + 1, pServerName.Length - pServerName.LastIndexOf("\") - 1)
                Dim serviceName As String = "MSSQL$" & pServerName.ToUpperBS()

                'Stop the Microsoft SQL Server service in case it is Running
                Dim sqlService As Service = mc.Services(serviceName)

                Dim i As Integer = 0
                If (sqlService.ServiceState = ServiceState.Running) Then
                    sqlService.Stop()
                    Do Until ((sqlService.ServiceState = ServiceState.Stopped) OrElse (i = 100))
                        sqlService.Refresh()
                        i += 1
                    Loop
                End If

                'Stop the Microsoft SQL Browser service in case it is Running
                Dim sqlBrowser As Service = mc.Services("SQLBrowser")

                i = 0
                If (sqlBrowser.ServiceState = ServiceState.Running) Then
                    sqlBrowser.Stop()
                    Do Until ((sqlBrowser.ServiceState = ServiceState.Stopped) OrElse (i = 100))
                        sqlBrowser.Refresh()
                        i += 1
                    Loop
                End If

                'Reference the Microsoft SQL Writer service
                Dim sc As ServiceController = New ServiceController("SqlWriter")

                i = 0
                If (sc.Status = ServiceControllerStatus.Running) Then
                    sc.Stop()
                    Do Until ((sc.Status = ServiceControllerStatus.Stopped) OrElse (i = 100))
                        sc.Refresh()
                        i += 1
                    Loop
                End If

                result = (sqlService.ServiceState = ServiceState.Stopped AndAlso sqlBrowser.ServiceState = ServiceState.Stopped AndAlso _
                          sc.Status = ServiceControllerStatus.Stopped)
            Catch ex As Exception
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.StopSQLService", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Checks if the SQL Server service is running
        ''' </summary>
        ''' <param name="ServerName">Server Name </param>
        ''' <returns>
        ''' True if ServerName local service is running, False otherwise
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 11/11/2010
        ''' Modified by: XB 01/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
        ''' </remarks>
        Public Shared Function IsSQLServiceRunning(ByVal ServerName As String) As Boolean
            Dim result As Boolean = False
            Dim myLogAcciones As New ApplicationLogManager()

            Try
                'Declare and create an instance of the ManagedComputer 
                'object that represents the WMI Provider services.
                Dim mc As New ManagedComputer()

                If ServerName.Contains("\") Then
                    ServerName = ServerName.Substring(ServerName.LastIndexOf("\") + 1, ServerName.Length - ServerName.LastIndexOf("\") - 1)
                End If

                'Dim ServiceName As String = "MSSQL$" & ServerName.ToUpper()
                Dim ServiceName As String = "MSSQL$" & ServerName.ToUpperBS()

                'Reference the Microsoft SQL Server service.
                Dim SqlService As Service
                SqlService = mc.Services(ServiceName)

                result = (SqlService.ServiceState = ServiceState.Running)

            Catch ex As Exception
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "IsSQLServiceRunning.StartSQLService", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' This will return two result sets, the first one containing the database name, size, and unallocated space
        ''' and the second containing a breakdown of the database's size into how much size is reserved and how much 
        ''' of that is taken up by data, how much by indexes, and how much remains unused.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDataBaseSizeInformation() As DataSet
            Dim result As New DataSet()

            Try
                'RH 18/05/2011 - Introduce the Using statement
                Using MySQLConnection As New SqlClient.SqlConnection(DAOBase.GetConnectionString())
                    'command to execute the system database StoreProcesure.
                    Dim MySQLCommand As SqlClient.SqlCommand = MySQLConnection.CreateCommand()
                    MySQLCommand.CommandType = CommandType.StoredProcedure
                    MySQLCommand.CommandText = "sp_spaceused"

                    'DataAdapter to fill the dataset with the results.
                    Dim MySQLDataAdapter As New SqlClient.SqlDataAdapter(MySQLCommand)

                    'fill the dataset with the information 
                    MySQLDataAdapter.Fill(result)
                End Using
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.StartSQLService", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Sets the database as SINGLE USER
        ''' </summary>
        ''' <param name="ServerName"></param>
        ''' <param name="DataBaseName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  SG 18/10/10
        ''' Modified by: RH 18/05/2011
        ''' </remarks>
        Public Shared Function SetDataBaseSingleUser(ByVal ServerName As String, ByVal DataBaseName As String, _
                                                     ByVal DBLogin As String, ByVal DBPassword As String) As Boolean
            Dim result As Boolean = False
            Dim MyServer As Server = Nothing

            Try
                MyServer = New Server(ServerName) 'SQL server instance.

                MyServer.ConnectionContext.LoginSecure = False
                MyServer.ConnectionContext.Login = DBLogin
                MyServer.ConnectionContext.Password = DBPassword
                MyServer.ConnectionContext.DatabaseName = "master"

                'Close all open connection to the DB. Put DB offline.
                Dim myTSQL As String = String.Format("ALTER DATABASE {0} SET OFFLINE WITH ROLLBACK IMMEDIATE", DataBaseName)
                result = ExecuteScripts(MyServer, DataBaseName, myTSQL)

                'Put DB online
                myTSQL = String.Format("ALTER DATABASE {0} SET ONLINE", DataBaseName)
                result = ExecuteScripts(MyServer, "master", myTSQL)

                'Set Single User
                myTSQL = String.Format( _
                        "ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", DataBaseName)

                result = ExecuteScripts(MyServer, DataBaseName, myTSQL)

                Dim db As Database = MyServer.Databases(DataBaseName)
                result = (db.UserAccess = DatabaseUserAccess.Single)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.SetDataBaseSingleUser", EventLogEntryType.Error, False)

            Finally
                If Not MyServer Is Nothing Then
                    MyServer.ConnectionContext.Disconnect()
                End If

            End Try

            Return result
        End Function

        ''' <summary>
        ''' Sets the database as MULTI USER
        ''' </summary>
        ''' <param name="ServerName"></param>
        ''' <param name="DataBaseName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  SG 18/10/10
        ''' Modified by: RH 18/05/2011
        ''' </remarks>
        Public Shared Function SetDataBaseMultiUser(ByVal ServerName As String, ByVal DataBaseName As String, _
                                                    ByVal DBLogin As String, ByVal DBPassword As String) As Boolean
            Dim result As Boolean = False
            Dim MyServer As Server = Nothing

            Try
                MyServer = New Server(ServerName) 'SQL server instance.

                MyServer.ConnectionContext.LoginSecure = False
                MyServer.ConnectionContext.Login = DBLogin
                MyServer.ConnectionContext.Password = DBPassword
                MyServer.ConnectionContext.DatabaseName = "master"

                'Set Multi User
                Dim myTSQL As String = String.Format( _
                        "ALTER DATABASE {0} SET MULTI_USER WITH ROLLBACK IMMEDIATE", DataBaseName)

                result = ExecuteScripts(MyServer, DataBaseName, myTSQL)

                Dim db As Database = MyServer.Databases(DataBaseName)
                result = (db.UserAccess = DatabaseUserAccess.Multiple)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.SetDataBaseMultiUser", EventLogEntryType.Error, False)

            Finally
                If Not MyServer Is Nothing Then
                    MyServer.ConnectionContext.Disconnect()
                End If
            End Try

            Return result
        End Function

#Region "NEW FUNCTIONS FOR BACKUP/RESTORE DATABASE"
        ''' <summary>
        ''' Validate if a DataBase exists on the server. New implementation to avoid using of SMO objects (due to problems with Framework 4.5)
        ''' </summary>
        ''' <param name="pDataBaseName">DataBase Name</param>
        ''' <returns>True if DataBase exists, False if DataBase does not exist</returns>
        ''' <remarks>
        ''' Created by: SA/XB 08/01/2013 
        ''' </remarks>
        Public Shared Function DataBaseExistsNEW(ByVal pDataBaseName As String) As Boolean
            Dim result As Boolean = False
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                'Open a Connection to master DB
                Dim connectionString As String = DAOBase.GetConnectionString.Replace(pDataBaseName, "master")
                dbConnection.ConnectionString = connectionString
                dbConnection.Open()

                Dim cmdText As String = " SELECT COUNT(*) AS DBExists FROM sys.sysdatabases " & vbCrLf & _
                                        " WHERE  UPPER(name) ='" & pDataBaseName.Trim.ToUpper & "'"

                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                    Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                    If (dbDataReader.HasRows) Then
                        dbDataReader.Read()

                        If (dbDataReader.IsDBNull(0)) Then
                            result = False
                        Else
                            result = Convert.ToBoolean(IIf(Convert.ToInt32(dbDataReader.Item("DBExists")) = 0, False, True))
                        End If
                    End If
                    dbDataReader.Close()
                End Using
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.DataBaseExistsNEW", EventLogEntryType.Error, False)
            Finally
                'Close the DB Connection
                If (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Create the Application DataBase
        ''' </summary>
        ''' <param name="pDataBaseName">DataBase Name</param>
        ''' <remarks>
        ''' Created by: SA/XB 08/01/2013 
        ''' </remarks>
        Public Shared Function CreateDataBase(ByVal pDataBaseName As String) As Boolean
            Dim result As Boolean = False
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                'Open a Connection to master DB
                Dim connectionString As String = DAOBase.GetConnectionString.Replace(pDataBaseName, "master")
                dbConnection.ConnectionString = connectionString
                dbConnection.Open()

                Dim myTSQL As String = " CREATE DATABASE [" & pDataBaseName & "] " & vbCrLf
                Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                    dbCmd.ExecuteNonQuery()
                    result = True
                End Using
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.CreateDataBase", EventLogEntryType.Error, False)
            Finally
                'Close the DB Connection
                If (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Get the installed version of SQL Server
        ''' </summary>
        ''' <param name="pDataBaseName">DataBase name</param>
        ''' <returns>A String value containing the installed version of SQL Server</returns>
        ''' <remarks>
        ''' Created by: SA 09/01/2013
        ''' </remarks>
        Public Shared Function GetSQLServerVersion(ByVal pDataBaseName As String) As String
            Dim result As String = String.Empty
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                'Open a Connection to master DB
                Dim connectionString As String = DAOBase.GetConnectionString().Replace(pDataBaseName, "master")
                dbConnection.ConnectionString = connectionString
                dbConnection.Open()

                'Use the TSQL function SERVERPROPERTY to get the installed version of SQL Server
                Dim myTSQL As String = " SELECT SERVERPROPERTY('ProductVersion') AS Version "
                Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                    Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                    If (dbDataReader.HasRows) Then
                        dbDataReader.Read()

                        If (Not dbDataReader.IsDBNull(0)) Then
                            result = dbDataReader.Item("Version").ToString
                        End If
                    End If
                    dbDataReader.Close()
                End Using
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.GetSQLServerVersion", EventLogEntryType.Error, False)
            Finally
                'Close the DB Connection
                If (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Sets the database as SINGLE USER. New implementation to avoid using of SMO objects (due to problems with Framework 4.5)
        ''' </summary>
        ''' <param name="pDataBaseName">DataBase Name</param>
        ''' <returns>True if access mode to DB has been changed to Single User mode; otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by: SA/XB 08/01/2013 
        ''' </remarks>
        Public Shared Function SetDataBaseSingleUserNEW(ByVal pDataBaseName As String) As Boolean
            Dim result As Boolean = False
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                'Open a Connection to master DB
                Dim connectionString As String = DAOBase.GetConnectionString().Replace(pDataBaseName, "master")
                dbConnection.ConnectionString = connectionString
                dbConnection.Open()

                'Close all open connections to the DB and put it offline
                Dim myTSQL As String = " ALTER DATABASE [" & pDataBaseName & "] SET OFFLINE WITH ROLLBACK IMMEDIATE "
                Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                    dbCmd.ExecuteNonQuery()
                End Using

                'Put DB online
                myTSQL = " ALTER DATABASE [" & pDataBaseName & "] SET ONLINE "
                Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                    dbCmd.ExecuteNonQuery()
                End Using

                'Set DB to Single User
                myTSQL = " ALTER DATABASE [" & pDataBaseName & "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE "
                Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                    dbCmd.ExecuteNonQuery()
                End Using

                'Finally, check the user access mode for the DB has been changed to Single User Mode
                myTSQL = " SELECT user_access_desc FROM sys.databases " & vbCrLf & _
                         " WHERE  UPPER(name) ='" & pDataBaseName.Trim.ToUpper & "'" & vbCrLf

                Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                    Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                    If (dbDataReader.HasRows) Then
                        dbDataReader.Read()

                        If (dbDataReader.IsDBNull(0)) Then
                            result = False
                        Else
                            result = Convert.ToBoolean(IIf(dbDataReader.Item("user_access_desc").ToString = "SINGLE_USER", True, False))
                        End If
                    End If
                    dbDataReader.Close()
                End Using
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.SetDataBaseSingleUserNEW", EventLogEntryType.Error, False)
            Finally
                'Close the DB Connection
                If (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Sets the database as MULTI USER. New implementation to avoid using of SMO objects (due to problems with Framework 4.5)
        ''' </summary>
        ''' <param name="pDataBaseName">DataBase Name</param>
        ''' <returns>True if access mode to DB has been changed to Multi User mode; otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by: SA/XB 08/01/2013 
        ''' </remarks>
        Public Shared Function SetDataBaseMultiUserNEW(ByVal pDataBaseName As String) As Boolean
            Dim result As Boolean = False
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                'Open a Connection to the application DB
                Dim connectionString As String = DAOBase.GetConnectionString()
                dbConnection.ConnectionString = connectionString
                dbConnection.Open()

                'Set DB to Single User
                Dim myTSQL As String = " ALTER DATABASE [" & pDataBaseName & "] SET MULTI_USER WITH ROLLBACK IMMEDIATE "
                Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                    dbCmd.ExecuteNonQuery()
                End Using

                'Finally, check the user access mode for the DB has been changed to Multi User Mode
                myTSQL = " SELECT user_access_desc FROM sys.databases " & vbCrLf & _
                         " WHERE  UPPER(name) ='" & pDataBaseName.Trim.ToUpper & "'" & vbCrLf

                Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                    Dim dbDataReader As SqlClient.SqlDataReader = dbCmd.ExecuteReader()
                    If (dbDataReader.HasRows) Then
                        dbDataReader.Read()

                        If (dbDataReader.IsDBNull(0)) Then
                            result = False
                        Else
                            result = Convert.ToBoolean(IIf(dbDataReader.Item("user_access_desc").ToString = "MULTI_USER", True, False))
                        End If
                    End If
                    dbDataReader.Close()
                End Using
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message & " ----- " & ex.InnerException.ToString(), "DataBaseManager.SetDataBaseMultiUserNEW", EventLogEntryType.Error, False)
            Finally
                'Close the DB Connection
                If (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Restore a DataBase from an specific BackUp file. New implementation to avoid use of SMO objects
        ''' </summary>
        ''' <param name="pDataBaseName">DataBase Name to restore</param>
        ''' <param name="pBackUpFileName">Full path and name of the DataBase backup</param>
        ''' <returns>True if DataBase has been restored; otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by: SA/XB 08/01/2013 
        ''' </remarks>
        Public Shared Function RestoreDataBaseNEW(ByVal pDataBaseName As String, ByVal pBackUpFileName As String) As Boolean
            Dim result As Boolean = False
            Dim myLogAcciones As New ApplicationLogManager()

            Try
                'Verify if the Application DataBase exists
                If (Not DataBaseExistsNEW(pDataBaseName)) Then
                    'If Application DataBase does not exist, then create it
                    CreateDataBase(pDataBaseName)
                End If

                If (SetDataBaseSingleUserNEW(pDataBaseName)) Then
                    'Open a Connection to master DB
                    Dim connectionString As String = DAOBase.GetConnectionString().Replace(pDataBaseName, "master")
                    Dim dbConnection As New SqlClient.SqlConnection
                    dbConnection.ConnectionString = connectionString
                    dbConnection.Open()

                    'SQL Sentence for Restore DataBase
                    Dim myTSQL As String = " RESTORE DATABASE [" & pDataBaseName.Trim & "] " & vbCrLf & _
                                           " FROM DISK = '" & pBackUpFileName.Trim & "' " & vbCrLf & _
                                           " WITH RECOVERY, REPLACE; "

                    Using dbCmd As New SqlClient.SqlCommand(myTSQL, dbConnection)
                        dbCmd.ExecuteNonQuery()
                    End Using
                    dbConnection.Close()

                    result = True
                    myLogAcciones.CreateLogActivity("Restore " & pBackUpFileName & " Database OK", "RestoreDataBaseNEW", EventLogEntryType.Information, False)
                Else
                    Throw New Exception("Unable to set DB to Single User.")
                End If
            Catch ex As Exception
                Dim Message As String
                If ex.InnerException IsNot Nothing AndAlso ex.InnerException.InnerException IsNot Nothing Then
                    Message = ex.InnerException.InnerException.Message
                Else
                    Message = ex.Message
                End If
                myLogAcciones.CreateLogActivity(Message, "RestoreDataBaseNEW", EventLogEntryType.Error, False)
                Throw
            Finally
                'Put DB in Multi User Mode
                SetDataBaseMultiUserNEW(pDataBaseName)
            End Try
            Return result
        End Function

#End Region

    End Class

End Namespace
