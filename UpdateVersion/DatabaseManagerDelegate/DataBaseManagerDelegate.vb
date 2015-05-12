Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Microsoft.SqlServer.Management.Smo 'AG 16/01/2013 v1.0.1

Namespace Biosystems.Ax00.BL.UpdateVersion

    ''' <summary>
    ''' This Class manages the Business Logic related with the database management, 
    ''' is in charge to validate if a database exists on the server, create backups 
    ''' and restore backup, drop database, start the required SQL services.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataBaseManagerDelegate

#Region "PUBLIC METHODS"

        ''' <summary>
        ''' Restore a database from a bak file into the selected server.
        ''' </summary>
        ''' <param name="ServerName">Server Name to restore database.</param>
        ''' <param name="DataBaseName">Database Name To restore.</param>
        ''' <param name="BackUpFileName">bak File Name, with the path include. </param>
        ''' <exception cref=" ApplicationLogManager "></exception>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by: RH 10/11/2010
        ''' </remarks>
        Public Function RestoreDatabase(ByVal ServerName As String, ByVal DataBaseName As String, _
                                        ByVal DBLogin As String, ByVal DBPassword As String, ByVal BackUpFileName As String) As Boolean
            Dim result As Boolean = False
            'Dim myLogAcciones As New ApplicationLogManager()

            Try
                If Not String.IsNullOrEmpty(ServerName) AndAlso _
                   Not String.IsNullOrEmpty(DataBaseName) AndAlso _
                   Not String.IsNullOrEmpty(BackUpFileName) Then 'validate do not send empty values.
                    If System.IO.File.Exists(BackUpFileName) Then 'validate backupfile exist.
                        result = DBManager.RestoreDataBase(ServerName, DataBaseName, DBLogin, DBPassword, _
                                                           BackUpFileName)
                    Else
                        GlobalBase.CreateLogActivity(BackUpFileName & " file not found.", _
                                                        "DataBaseManagerDelegate.RestoreDatabase", EventLogEntryType.Error, False)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.RestoreDatabase", EventLogEntryType.Error, False)

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
        ''' 'Create a Database backup into the Defaul Backup directory for 
        ''' Sql Server the backup name if create from the Database name and the
        ''' datetime of creation.
        ''' </summary>
        ''' <param name="ServerName">Server Name.</param>
        ''' <param name="DataBaseName">Database name to create the backup</param>
        ''' <returns>True if Backup Succes, False if Fail.</returns>
        ''' <remarks></remarks>
        Public Function BackUpDataBase(ByVal ServerName As String, ByVal DataBaseName As String, ByVal DBLogin As String, _
                                       ByVal DBPassword As String, Optional ByVal pInstalationProcess As Boolean = False) As Boolean
            Dim result As Boolean = False
            'Dim myLogAcciones As New ApplicationLogManager()
            Try
                If Not String.IsNullOrEmpty(ServerName) AndAlso _
                   Not String.IsNullOrEmpty(DataBaseName) Then 'validate do not send empty values.
                    If DataBaseExist(ServerName, DataBaseName, DBLogin, DBPassword) Then
                        result = DBManager.BackUpDataBase(ServerName, DataBaseName, DBLogin, DBPassword, pInstalationProcess)
                    Else
                        GlobalBase.CreateLogActivity(DataBaseName & " database not found in server " & ServerName, _
                                                        "DataBaseManagerDelegate.BackUpDataBase", EventLogEntryType.Error, False)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.BackUpDataBase", EventLogEntryType.Error, False)
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
        ''' <remarks>Only work with a local instance of the data base server</remarks>
        Public Function BackUpDataBaseAndMoveBkFile(ByVal ServerName As String, ByVal DataBaseName As String, ByVal DBLogin As String, _
                                                    ByVal DBPassword As String, ByVal DirectoryTo As String) As Boolean
            Dim result As Boolean = False
            'Dim myLogAcciones As New ApplicationLogManager()

            Try
                If Not String.IsNullOrEmpty(ServerName) AndAlso _
                   Not String.IsNullOrEmpty(DataBaseName) Then 'validate do not send empty values.
                    If DataBaseExist(ServerName, DataBaseName, DBLogin, DBPassword) Then
                        result = DBManager.BackUpDataBaseAndMoveBkFile(ServerName, DataBaseName, DBLogin, DBPassword, DirectoryTo)
                    Else
                        GlobalBase.CreateLogActivity(DataBaseName & " database not found in server " & ServerName, _
                                                        "DataBaseManagerDelegate.BackUpDataBase", EventLogEntryType.Error, False)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.BackUpDataBaseAndMoveBkFile", EventLogEntryType.Error, False)
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
        Public Function IsSQLServer2005(ByVal ServerName As String, _
                                        ByVal DBLogin As String, ByVal DBPassword As String) As Boolean
            Dim result As Boolean = False

            Try
                result = DBManager.IsSQLServer2005(ServerName, DBLogin, DBPassword)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.IsSQLServer2005", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Validate if database exist in the server
        ''' </summary>
        ''' <param name="ServerName">Server Name</param>
        ''' <param name="DataBaseName">Database Name</param>
        ''' <returns>True if exist or False if not exist</returns>
        ''' <remarks></remarks>
        Public Function DataBaseExist(ByVal ServerName As String, ByVal DataBaseName As String, _
                                      ByVal DBLogin As String, ByVal DBPassword As String) As Boolean
            Dim result As Boolean = False

            Try
                'Verify if values are empty.
                If Not String.IsNullOrEmpty(ServerName) AndAlso _
                   Not String.IsNullOrEmpty(DataBaseName) Then 'validate do not send empty values.
                    result = DBManager.DataBaseExist(ServerName, DataBaseName, DBLogin, DBPassword)
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.DataBaseExist", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Delete a database from an specific server.
        ''' </summary>
        ''' <param name="ServerName">Server Name.</param>
        ''' <param name="DataBaseName">Database Name.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteDatabase(ByVal ServerName As String, ByVal DataBaseName As String, _
                                       ByVal DBLogin As String, ByVal DBPassword As String) As Boolean
            Dim result As Boolean = False

            Try
                result = DBManager.DropDatabaseFromServer(ServerName, DataBaseName, DBLogin, DBPassword) 'drop database from server

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.DeleteDatabase", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Update the database structure and data using scripts.
        ''' </summary>
        ''' <param name="pServer">Server Name.</param>
        ''' <param name="DatabaseScript">Scripts to update the database structure or data.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RunDatabaseScript(ByVal pServer As Server, ByVal DataBaseName As String, _
                                          ByVal DatabaseScript As String) As Boolean
            Dim result As Boolean = False

            Try
                result = DBManager.RunDatabaseScript(pServer, DataBaseName, DatabaseScript)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.UpdateDatabaseStructureAndData", EventLogEntryType.Error, False)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Start SQL Services in case they are Paused or Stopped: SqlServer, SqlBrowser and SqlWriter
        ''' </summary>
        ''' <returns>Boolean value indicating if the three SQL Services are or have been started</returns>
        ''' <remarks></remarks>
        Public Function StartSQLService(ByVal pServerName As String) As Boolean
            Return DBManager.StartSQLService(pServerName)
        End Function

        ''' <summary>
        ''' Stop SQL Services: SqlServer, SqlBrowser and SqlWriter.
        ''' NOTE: this function is called from FormClosing Event of screen IAx00MainMDI but only in RELEASE Mode. That is the reason for which 
        '''       the call does not appear when it is searched using Find All References option in the left button menu
        ''' </summary>
        ''' <returns>Boolean value indicating if the three SQL Services have been stopped</returns>
        ''' <remarks>
        ''' Created by: SA 06/02/2014 - BT #1495
        ''' </remarks>
        Public Function StopSQLService(ByVal pServerName As String) As Boolean
            Return DBManager.StopSQLService(pServerName)
        End Function

        ''' <summary>
        ''' Checks if the SQL Server service is running
        ''' </summary>
        ''' <param name="ServerName">Server Name </param>
        ''' <returns>
        ''' True if ServerName local service is running, False otherwise
        ''' </returns>
        ''' <remarks>
        ''' Created by: RH 11/11/2010
        ''' </remarks>
        Public Function IsSQLServiceRunning(ByVal ServerName As String) As Boolean
            Return DBManager.IsSQLServiceRunning(ServerName)
        End Function

        ''' <summary>
        ''' This will return two result sets, the first one containing the database name, 
        ''' size, and unallocated space and the second containing a breakdown of the database's 
        ''' size into how much size is reserved and how much of that is taken up by data, 
        ''' how much by indexes, and how much remains unused.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDataBaseSizeInformation() As DataSet
            Return DBManager.GetDataBaseSizeInformation()
        End Function

        ''' <summary>
        ''' This will only return the database used size 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDataBaseSize() As String
            Dim result As String = ""
            Try
                Dim myDs As DataSet
                myDs = GetDataBaseSizeInformation()
                If Not myDs Is Nothing AndAlso myDs.Tables(0).Rows.Count > 0 Then
                    result = myDs.Tables(0).Rows(0)("database_size").ToString()
                End If

            Catch ex As Exception
                result = ex.Message
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
        ''' Created by: RH 15/07/2011
        ''' </remarks>
        Public Function SetDataBaseMultiUser(ByVal ServerName As String, ByVal DataBaseName As String, _
                                             ByVal DBLogin As String, ByVal DBPassword As String) As Boolean
            Return DBManager.SetDataBaseMultiUser(ServerName, DataBaseName, DBLogin, DBPassword)
        End Function

        ''' <summary>
        ''' Method incaharge to close the connection pool
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 23/12/2011
        ''' Modified by: RH 08/02/2012 Bugs correction
        ''' </remarks>
        Public Function CloseAllOpenConnection() As Boolean
            Dim ClosedConnections As Boolean = False

            Try
                'Close any open connection 
                Dim dbConnection As SqlClient.SqlConnection = Nothing
                Dim myGlobalDataTO As GlobalDataTO = Nothing

                myGlobalDataTO = DAOBase.GetOpenDBConnection(Nothing)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    SqlClient.SqlConnection.ClearPool(dbConnection)
                    SqlClient.SqlConnection.ClearAllPools()
                    dbConnection.Close()

                    ClosedConnections = True
                End If

            Catch ex As Exception
                ClosedConnections = False
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.CloseAllOpenConnection", EventLogEntryType.Error, False)
            End Try
            Return ClosedConnections
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
                Dim myShrinkScript As String = "USE [" & pDataBaseName & "] DBCC SHRINKDATABASE(N'" & pDataBaseName & "' )"

                If pServer Is Nothing Then
                    'Create a new server control
                    pServer = New Server(DAOBase.DBServer)
                    pServer.ConnectionContext.LoginSecure = False
                    pServer.ConnectionContext.Login = DAOBase.DBLogin
                    pServer.ConnectionContext.Password = DAOBase.DBPassword
                End If

                myGlobalDataTO.SetDatos = RunDatabaseScript(pServer, pDataBaseName, myShrinkScript)

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message + " Innner: " + ex.InnerException.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseUpdateManager.ShrinkDatabase", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

#End Region

#Region "NEW PUBLIC FUNCTIONS FOR RESTORE DATABASE"
        ''' <summary>
        ''' Restore a database from a bak file into the selected server. New implementation to avoid using of SMO objects (due to problems with Framework 4.5)
        ''' </summary>
        ''' <param name="pDataBaseName">Database Name To restore.</param>
        ''' <param name="pBackUpFileName">bak File Name, with the path include. </param>
        ''' <remarks>
        ''' Created by: SA/XB 08/01/2013 
        ''' </remarks>
        Public Function RestoreDatabaseNEW(ByVal pDataBaseName As String, ByVal pBackUpFileName As String) As Boolean
            Dim result As Boolean = False
            'Dim myLogAcciones As New ApplicationLogManager()

            Try
                If (Not String.IsNullOrEmpty(pDataBaseName) AndAlso Not String.IsNullOrEmpty(pBackUpFileName)) Then
                    'Validate the Backup file exists
                    If (System.IO.File.Exists(pBackUpFileName)) Then
                        result = DBManager.RestoreDataBaseNEW(pDataBaseName, pBackUpFileName)

                        'Delete the BackUp file
                        System.IO.File.Delete(pBackUpFileName)
                    Else
                        GlobalBase.CreateLogActivity(pBackUpFileName & " file not found.", "DataBaseManagerDelegate.RestoreDatabaseNEW", EventLogEntryType.Error, False)
                    End If
                End If
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.RestoreDatabaseNEW", EventLogEntryType.Error, False)
                Throw
            End Try
            Return result
        End Function
#End Region

#Region "PRIVATE METHODS"

        ''' <summary>
        ''' Get the analyzer model, analyzerID and worksessionID
        ''' (copied and adapted from BioSystemUpdate.GetAnalyzerAndWorksessionInfo)
        ''' </summary>
        ''' <param name="dbConnection"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <remarks>Created AG 16/01/2013</remarks>
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
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseManagerDelegate.GetAnalyzerAndWorksessionInfo", EventLogEntryType.Error, False)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

        End Sub

#End Region

    End Class
End Namespace
