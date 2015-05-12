Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global


Namespace Biosystems.Ax00.BL.UpdateVersion

    Public Class DataBaseInstallerManagerDelegate

        ''' <summary>
        ''' Installs the application database, restoring a database backup file.
        ''' </summary>
        ''' <returns>true if restore is OK, or false if Fail</returns>
        ''' <remarks>
        ''' Modified by: RH    18/11/2010 - Assume SQL Server and Browser services are running. This speeds up the process.
        '''                               - Assume Database does not exist. This speeds up the process.
        '''              TR    24/01/2011 - Get the backup file form the AppConfig.
        '''              TR    22/01/2013 -Implement the use of globaldata TO   
        ''' </remarks>
        Public Function InstallApplicationDataBase(ByVal pServerName As String, ByVal pDataBaseName As String, _
                                                   ByVal DBLogin As String, ByVal DBPassword As String) As GlobalDataTO
            Dim myGlobalDataTO = New GlobalDataTO
            Dim MyDabaseManagerDelegate As New DataBaseManagerDelegate()
            'Dim myLogAcciones As New ApplicationLogManager()

            Try
                'TR 25/01/2011 -Replace by corresponding value on global base.
                Dim BackUpfile As String = AppDomain.CurrentDomain.BaseDirectory & GlobalBase.InstallDBBackupFilePath
                'TR 25/01/2011 -END
                'TR 21/01/2013 v1.0.1 -New implementation using the global data TO
                myGlobalDataTO = CopyBackupToTempDirectory(BackUpfile)
                If Not myGlobalDataTO.HasError Then
                    BackUpfile = myGlobalDataTO.SetDatos.ToString()
                    MyDabaseManagerDelegate.RestoreDatabase(pServerName, pDataBaseName, DBLogin, DBPassword, BackUpfile)
                    myGlobalDataTO.SetDatos = True
                Else
                    myGlobalDataTO.SetDatos = False
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorMessage = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
                myGlobalDataTO.SetDatos = False
                GlobalBase.CreateLogActivity(ex.Message, "DataBaseInstallerManagerDelegate.InstallApplicationDataBase", EventLogEntryType.Error, False)
            Finally
                'TR 21/01/2013 v1.0.1 -Remove Ax00.bak file from temporal folder.
                RemoveDBBackupFile(GlobalBase.TemporalDirectory & GlobalBase.DBBakupFileName)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Create a temporal directory and move the backup database file into.
        ''' </summary>
        ''' <param name="BackupFile"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: 
        ''' Modified BY:TR 21/01/2013 v1.0.1 -Implement the Global Base values. 
        ''' </remarks>
        Public Function CopyBackupToTempDirectory(ByVal BackupFile As String) As GlobalDataTO
            Dim TempDirectory As String = GlobalBase.TemporalDirectory  '"C:\TEMP"
            Dim myGlobalDataTO = New GlobalDataTO
            Try
                Dim NewBackupFilePath As String = ""
                If Not IO.Directory.Exists(TempDirectory) Then ' validate if the directory exist to create it.
                    IO.Directory.CreateDirectory(TempDirectory) 'create the Temp Directory
                End If

                If IO.File.Exists(TempDirectory & GlobalBase.DBBakupFileName) Then ' validate if the file exist to delete it
                    IO.File.Delete(TempDirectory & GlobalBase.DBBakupFileName) 'delete the file                 
                End If

                If IO.File.Exists(BackupFile) Then ' validate if the file exist at the application software.
                    IO.File.Copy(BackupFile, TempDirectory & GlobalBase.DBBakupFileName) ' copy the file into the temp directory
                    NewBackupFilePath = TempDirectory & GlobalBase.DBBakupFileName ' assigne the value to send as result of the operation.
                End If

                'Set the new backup file path to setDatos on globaldata to
                myGlobalDataTO.SetDatos = NewBackupFilePath

            Catch ex As Exception
                'Set the error value to the global Data TO.
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
                ''Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "DataBaseUpdateManager.CopyBackupToTEMPDirectory", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Remove the Database Backup files from temporal foldel
        ''' Ax00.bak
        ''' </summary>
        ''' <param name="pDBBackupFile">Backup file Name, with the path.</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 21/01/2012 V1.0.1</remarks>
        Public Function RemoveDBBackupFile(ByVal pDBBackupFile As String) As GlobalDataTO
            Dim myGlobalDataTO = New GlobalDataTO
            Try
                If IO.File.Exists(pDBBackupFile) Then
                    IO.File.Delete(pDBBackupFile) 'remove Ax00.bak backup file. from temporal foldel.
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message & " --- " & ex.InnerException.ToString(), _
                                                "DataBaseUpdateManager.RemoveDBBackupFile", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


    End Class

End Namespace
