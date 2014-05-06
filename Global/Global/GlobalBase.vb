Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global.TO
Imports System.Reflection
Imports System.IO
Imports System.Configuration


Namespace Biosystems.Ax00.Global

    ''' <summary>
    ''' Base class for all the global classes, all global clases must inherits from
    ''' GlobalBase Clase, to add all the general funtionality create here.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GlobalBase

        Private SynapseNumberOfDaysAttribute As Integer

#Region "Properties"

        'SGM 01/02/2013 - Bug #1112
        'property for checking Assembly's Identity
        Public Shared Property IsServiceAssembly As Boolean
            Get
                Return My.Settings.IsServiceAssembly
            End Get
            Set(value As Boolean)
                My.Settings.IsServiceAssembly = value
            End Set
        End Property

        ' DL 23/11/2011 - Report Path
        Shared ReadOnly Property ReportPath() As String
            Get
                Return My.Settings.ReportPath
            End Get
        End Property

        ' XBC 08/11/2010 - SERVICE SOFTWARE
        Shared ReadOnly Property XmlFwScripts() As String
            Get
                Return My.Settings.XmlFwScripts
            End Get
        End Property

        'SG 09/11/10 - SERVICE SOFTWARE
        Public ReadOnly Property FactoryXmlFwScripts() As String
            Get
                Return My.Settings.FactoryXmlFwScripts
            End Get
        End Property

        'SG 09/11/10 - SERVICE SOFTWARE
        Public ReadOnly Property XmlFwScriptsWhileDecrypting() As String
            Get
                Return My.Settings.XmlFwScriptsDecrypting
            End Get
        End Property

        'SG 09/11/10 - SERVICE SOFTWARE
        'Public ReadOnly Property XmlScriptsWhileImporting() As String
        '    Get
        '        Return My.Settings.XmlScriptsImporting
        '    End Get
        'End Property

        'PG 25/11/10 - SERVICE SOFTWARE
        Public ReadOnly Property XmlFwScriptsTempFile() As String
            Get
                Return My.Settings.XmlFwScriptsTempFile
            End Get
        End Property

        'SG 18/01/11 - SERVICE SOFTWARE
        Shared ReadOnly Property FwAdjustmentsPath() As String
            Get
                Return My.Settings.FwAdjustmentsPath
            End Get
        End Property

        'SG 18/01/11 - SERVICE SOFTWARE
        Shared ReadOnly Property PhotometryTestsFile() As String
            Get
                Return My.Settings.PhotometryTestsFile
            End Get
        End Property

        ' XBC 03/12/2010 - Common folders (User & Service)
        Shared Property AppPath() As String
            Get
                Return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                '#If DEBUG Then
                '                Return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                '#Else
                '                Return My.Settings.AppPath & ModelAnalyzer
                '#End If
            End Get
            Set(ByVal value As String)
                My.Settings.AppPath = value
            End Set
        End Property

        'SG 28/10/10 - SERVICE SOFTWARE
        Public ReadOnly Property ServiceInfoDocsPath() As String
            Get
                Return My.Settings.ServiceInfoDocsPath
            End Get
        End Property

        'SGM 08/03/11 NOT USED
        'Public Property ModelAnalyzer() As String
        '    Get
        '        Return My.Settings.ModelAnalyzer
        '    End Get
        '    Set(ByVal value As String)
        '        My.Settings.ModelAnalyzer = value
        '    End Set
        'End Property

        Public ReadOnly Property ServicePath() As String
            Get
#If DEBUG Then
                Return AppPath
#Else
                Return AppPath & My.Settings.ServicePath
#End If
            End Get
        End Property


        Shared ReadOnly Property ImagesPath() As String
            Get
                Return My.Settings.ImagesPath
            End Get
        End Property

        ' XBC 03/12/2010 - Common folders (User & Service)
        Public ReadOnly Property DataBaseConnection() As String
            Get
                Return My.Settings.BiosystemsConnDev
            End Get
        End Property

        'DLM 13/05/2011 - LOG FILE
        Shared ReadOnly Property XmlLogFile() As String
            Get
                Return My.Settings.XmlLogFile
            End Get
        End Property


        Shared ReadOnly Property XmlLogFilePath() As String
            Get
                Return My.Settings.XmlLogFilePath
            End Get
        End Property


        Shared ReadOnly Property ISEParammetersFilePath() As String
            Get
                Return My.Settings.ISEParammetersFilePath
            End Get
        End Property

        Shared ReadOnly Property ISEResultDebugModeFilePath() As String
            Get
                Return My.Settings.ISEResultDebugModeFilePath
            End Get
        End Property


        Shared ReadOnly Property PCOSInfoFilePath() As String
            Get
                Return My.Settings.PCOSInfoFilePath
            End Get
        End Property

        Shared ReadOnly Property MSInfo32ReportName() As String
            Get
                Return My.Settings.MSInfo32ReportName
            End Get
        End Property

        Shared ReadOnly Property InstallDBBackupFilePath() As String
            Get
                Return My.Settings.InstallDBBackupFilePath
            End Get
        End Property

        Shared ReadOnly Property InstallDBBTaskFilePath() As String
            Get
                Return My.Settings.InstallDBTaskFilePath
            End Get
        End Property

        Shared ReadOnly Property VersionFileName() As String
            Get
                Return My.Settings.VersionFileName
            End Get
        End Property



        Shared ReadOnly Property DataBaseVersion() As String
            Get
                Return My.Settings.DataBaseVersion
            End Get
        End Property

        Shared ReadOnly Property AllowDBGreaterVersion() As Boolean
            Get
                Return My.Settings.AllowDBGreaterVersion
            End Get
        End Property

        Shared ReadOnly Property ClientSettingsProvider_ServiceUri() As String
            Get
                Return My.Settings.ClientSettingsProvider_ServiceUri
            End Get
        End Property


        Shared ReadOnly Property PortSpeed() As Integer
            Get
                Return My.Settings.PortSpeed
            End Get
        End Property

        Shared ReadOnly Property LastPortConnected() As String
            Get
                Return My.Settings.LastPortConnected
            End Get
        End Property

        Shared ReadOnly Property PortSpeedList() As String
            Get
                Return My.Settings.PortSpeedList
            End Get
        End Property

        ''' <summary>
        ''' Instance to allow developer work in several modes (service)
        ''' </summary>
        ''' <value></value>
        ''' <returns>
        ''' 0 real mode
        ''' 1 simulate mode (with no communications)
        ''' 2 developer mode (simulating communications through test forms)
        ''' </returns>
        ''' <remarks></remarks>
        Shared ReadOnly Property RealDevelopmentMode() As Integer
            Get
                'For testings modify the RealDevelopmentMode settings
                'return 1
                'return 2
                Return My.Settings.RealDevelopmentMode
            End Get
        End Property

        Shared ReadOnly Property ZIPExtension() As String
            Get
                Return My.Settings.ZIPExtension
            End Get
        End Property

        Shared ReadOnly Property BioSystemsDBConn() As String
            Get
                Return My.Settings.BiosystemsConnDev
            End Get
        End Property

        ' XBC 05/05/2011
        Shared ReadOnly Property MaxRepetitionsTimeout() As Integer
            Get
                Return My.Settings.MaxRepetitionsTimeout
            End Get
        End Property

        ' XBC 1/05/2011
        Shared ReadOnly Property OpticalCenteringCurrentLed() As Integer
            Get
                Return My.Settings.OpticalCenteringCurrentLed
            End Get
        End Property

        Shared ReadOnly Property OpticalCenteringStepLed() As Integer
            Get
                Return My.Settings.OpticalCenteringStepLed
            End Get
        End Property

        Shared ReadOnly Property OpticalCenteringWellsToRead() As Integer
            Get
                Return My.Settings.OpticalCenteringWellsToRead
            End Get
        End Property

        Shared ReadOnly Property OpticalCenteringStepsbyWell() As Integer
            Get
                Return My.Settings.OpticalCenteringStepsbyWell
            End Get
        End Property

        Shared ReadOnly Property ContaminationReagentPersistance() As Integer
            Get
                Return My.Settings.ContaminationReagentPersistance
            End Get
        End Property

        ''' <summary>
        ''' Connection string use on the update application data process (special cases
        ''' </summary>
        ''' <value></value>
        ''' <returns>
        ''' Temporal Database Connection string.
        ''' </returns>
        ''' <remarks>
        ''' CREATE BY: TR 23/03/2011
        ''' </remarks>
        Shared ReadOnly Property UpdateDatabaseConnectionString() As String
            Get
                Return My.Settings.UpdateDatabaseConnectionString
            End Get
        End Property


        'XBC 02/05/11 - SERVICE SOFTWARE
        Shared ReadOnly Property PhotometryBLFileOut() As String
            Get
                Return My.Settings.PhotometryBLFileOut
            End Get
        End Property

        'XBC 02/05/11 - SERVICE SOFTWARE
        Shared ReadOnly Property PhotometryRepeatabilityFileOut() As String
            Get
                Return My.Settings.PhotometryRepeatabilityFileOut
            End Get
        End Property

        'XBC 02/05/11 - SERVICE SOFTWARE
        Shared ReadOnly Property PhotometryStabilityFileOut() As String
            Get
                Return My.Settings.PhotometryStabilityFileOut
            End Get
        End Property

        'XBC 18/05/11 - SERVICE SOFTWARE
        Public ReadOnly Property ScalesDinamicRange() As Integer
            Get
                Return My.Settings.ScalesDinamicRange
            End Get
        End Property

        Shared ReadOnly Property AnalyzerInfoFileOut() As String
            Get
                Return My.Settings.AnalyzerInfoFileOut
            End Get
        End Property

        Shared ReadOnly Property QCResultsFilesPath() As String
            Get
                Return My.Settings.QCResultsFilesPath
            End Get
        End Property

        ' XBC 05/06/2012 - Not used
        'Shared ReadOnly Property FirmwareVersion() As String
        '    Get
        '        Return My.Settings.FirmwareVersion
        '    End Get
        'End Property
        ' XBC 05/06/2012

        Protected Friend Property IsSystemLogFull() As Boolean
            Get
                Return My.Settings.IsSystemLogFull
            End Get
            Set(ByVal value As Boolean)
                My.Settings.IsSystemLogFull = value
            End Set
        End Property

        ' XBC 07/06/2012
        Shared ReadOnly Property SNUpperPartSize() As Integer
            Get
                Return My.Settings.SNUpperPartSize
            End Get
        End Property

        ' XBC 07/06/2012
        Shared ReadOnly Property SNLowerPartSize() As Integer
            Get
                Return My.Settings.SNLowerPartSize
            End Get
        End Property

        ' XBC 07/06/2012
        Shared ReadOnly Property BA400ModelID() As String
            Get
                Return My.Settings.BA400ModelID
            End Get
        End Property

        'TR 16/01/2013 v1.0.1
        Shared ReadOnly Property UpdateVersionRestorePointName() As String
            Get
                Return My.Settings.UpdateVersionRestorePointName
            End Get
        End Property

        Shared ReadOnly Property PreviousFolder As String
            Get
                Return My.Settings.PreviousFolder
            End Get
        End Property
        'TR 16/01/2013 v1.0.1 END


        ''' <summary>
        ''' Used to indicate the name of Temporal Database used on the application data update
        ''' </summary>
        ''' <value></value>
        ''' <returns>indicate the name of Temporal Database.</returns>
        ''' <remarks>TR 17/01/2013 v1.0.1</remarks>
        Shared ReadOnly Property TemporalDBName() As String
            Get
                Return My.Settings.TemporalDBName
            End Get
        End Property

        ''' <summary>
        ''' Indicate the path where the backup database are used  on the application installation/update.
        ''' </summary>
        ''' <value></value>
        ''' <returns>the path where the backup database are.</returns>
        ''' <remarks>
        ''' CREATED BY: TR 17/01/2013 v1.0.1
        ''' </remarks>
        Shared ReadOnly Property BakDirectoryPath() As String
            Get
                Return My.Settings.BakDirectoryPath
            End Get
        End Property

        ''' <summary>
        ''' Indicate the path for the temporal directory 
        ''' use to copy database backup
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 17/01/2013 v1.0.1
        ''' </remarks>
        Shared ReadOnly Property TemporalDirectory() As String
            Get
                Return My.Settings.TemporalDirectory
            End Get
        End Property

        Shared ReadOnly Property TempDBBakupFileName() As String
            Get
                Return My.Settings.TempDBBakupFileName
            End Get
        End Property
        'TR 17/01/2013 v1.0.1 -END

        'TR  21/01/2013 v1.0.1
        ''' <summary>
        ''' Database Backup file name (Ax00.bak)
        ''' File use to restore Ax00 Database
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 21/01/2013 v1.0.1
        ''' </remarks>
        Shared ReadOnly Property DBBakupFileName() As String
            Get
                Return My.Settings.DBBackUpFileName
            End Get
        End Property
        'TR  21/01/2013 v1.0.1 -END.

        'SGM 18/02/2013
        Shared ReadOnly Property UpdateLogFile() As String
            Get
                Return My.Settings.UpdateLogFile
            End Get
        End Property


        ''' <summary>
        ''' Used to indicate the RSAT Update error name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>TR 19/02/2013</remarks>
        Shared ReadOnly Property RSATUpdateErrorName As String
            Get
                Return My.Settings.RSATUpdateErrorName
            End Get
        End Property

        ''' <summary>
        ''' Indicate the Synapse event log file name.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 18/04/2013 </remarks>
        Shared ReadOnly Property SynapseLogFileName() As String
            Get
                Return My.Settings.SynapseLogFileName
            End Get
        End Property


        Public Property SynapseNumberOfDays() As Integer
            Get
                Return SynapseNumberOfDaysAttribute
            End Get
            Set(value As Integer)
                SynapseNumberOfDaysAttribute = value
            End Set
        End Property



#Region "NOT USED - Get from Database"

        'Shared ReadOnly Property InvalidPortsList() As String
        '    Get
        '        Return My.Settings.InvalidPortsList
        '    End Get
        'End Property

        '''' <summary>
        '''' There are 3 ISE mode SimpleMode,DebugMode1,DebugMode2
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>CREATED BY: TR</remarks>
        'Shared ReadOnly Property ISEMode() As String
        '    Get
        '        Return My.Settings.ISEMode
        '    End Get
        'End Property

        'Shared ReadOnly Property VolumePredilution() As Single
        '    Get
        '        Return My.Settings.VolumePredilution
        '    End Get
        'End Property

        'Shared ReadOnly Property LIMSExportFilePath() As String
        '    Get
        '        Return My.Settings.LIMSExportFilePath
        '    End Get
        'End Property

        'Shared ReadOnly Property LIMSImportMemoPath() As String
        '    Get
        '        Return My.Settings.LIMSImportMemoPath
        '    End Get
        'End Property

        'Shared ReadOnly Property LIMSImportFilePath() As String






        Shared ReadOnly Property WriteToSystemLog() As Boolean
            Get
                Return My.Settings.WriteToSystemLog
            End Get
        End Property

        'Shared ReadOnly Property SATReportDir() As String
        '    Get
        '        Return My.Settings.SATReportDir
        '    End Get
        'End Property

        'Shared ReadOnly Property SATReportPrefix() As String
        '    Get
        '        Return My.Settings.SATReportPrefix
        '    End Get
        'End Property

        'Shared ReadOnly Property RestorePointDir() As String
        '    Get
        '        Return My.Settings.RestorePointDir
        '    End Get
        'End Property

        'Shared ReadOnly Property RestorePointPrefix() As String
        '    Get
        '        Return My.Settings.RestorePointPrefix
        '    End Get
        'End Property

        'Shared ReadOnly Property SystemBackUpDir() As String
        '    Get
        '        Return My.Settings.SystemBackupDir
        '    End Get
        'End Property


        'Shared ReadOnly Property SystemBackUpPrefix() As String
        '    Get
        '        Return My.Settings.SystemBackupPrefix
        '    End Get
        'End Property

        'Shared ReadOnly Property HistoryBackUpDir() As String
        '    Get
        '        Return My.Settings.HistoryBackupDir
        '    End Get
        'End Property

        'Shared ReadOnly Property HistoryBackUpPrefix() As String
        '    Get
        '        Return My.Settings.HistoryBackupPrefix
        '    End Get
        'End Property


        'Shared ReadOnly Property USBRegistryKey() As String
        '    Get
        '        Return My.Settings.USBRegistryKey
        '    End Get
        'End Property


        'Shared ReadOnly Property EmailRecipient() As String
        '    Get
        '        Return My.Settings.EmailRecipient
        '    End Get
        'End Property

        'Shared ReadOnly Property EmailSubject() As String
        '    Get
        '        Return My.Settings.EmailSubject
        '    End Get
        'End Property

        'Shared ReadOnly Property EmailBody() As String
        '    Get
        '        Return My.Settings.EmailBody
        '    End Get
        'End Property
#End Region

#End Region

#Region "Public methods"
        ''' <summary>
        ''' Method that create a Log entry into Application Log and optionally, into Systems Log
        ''' </summary>
        ''' <param name="Message">Message to save</param>
        ''' <param name="LogModule">Method where the incident happens</param>
        ''' <param name="LogType">Log type</param>
        ''' <param name="InformSystem">When TRUE, it indicates the error has to be writing also in the System Log</param>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: SG 29/08/2011 - Added exceptions management
        ''' Modified by: SG 18/02/2013 - Added Database Update Version log
        ''' </remarks>
        Public Sub CreateLogActivity(ByVal Message As String, ByVal LogModule As String, ByVal LogType As EventLogEntryType, ByVal InformSystem As Boolean)

            Dim MyApplicationLogTO As New ApplicationLogTO
            Dim MyApplicationLogManager As New ApplicationLogManager

            Try
                'Fill the ApplicationLogTO with the information to insert in the Log'
                MyApplicationLogTO.LogDate = DateTime.Now
                MyApplicationLogTO.LogMessage = Message
                MyApplicationLogTO.LogModule = LogModule
                MyApplicationLogTO.LogType = LogType


                'Insert information in the Log
                MyApplicationLogManager.InsertLog(MyApplicationLogTO)


                'If Not GlobalConstants.AnalyzerIsRunningFlag Then
                '    'MyApplicationLogManager.InsertLog(MyApplicationLogTO, InformSystem)
                '    MyApplicationLogManager.InsertLog(MyApplicationLogTO)
                'Else
                '    'MyApplicationLogManager.InsertInfoInLOG(MyApplicationLogTO)
                '    MyApplicationLogManager.InsertLog(MyApplicationLogTO)
                'End If


            Catch ex As Exception
                'consists of trying first to solve an eventual error because of missing End Of File
                'If (TypeOf ex Is Xml.XmlException) Then
                '    Dim myGlobal As GlobalDataTO

                '    myGlobal = MyApplicationLogManager.BackupWrongLogXmlFile()
                '    If (Not myGlobal.HasError) Then
                '        Dim MyExceptionAppLogTO As New ApplicationLogTO

                '        MyExceptionAppLogTO.LogDate = DateTime.Now
                '        MyExceptionAppLogTO.LogMessage = "The previous Xml Log file was wrong. A new one has been just created and the old one was saved and zipped "
                '        MyExceptionAppLogTO.LogModule = "ApplicationLogManager"
                '        MyExceptionAppLogTO.LogType = System.Diagnostics.EventLogEntryType.Error

                '        'Insert the Exception in the LOG and also the initial error
                '        Dim MyTempLogManager As New ApplicationLogManager
                '        'MyTempLogManager.InsertLog(MyExceptionAppLogTO, True)
                '        MyTempLogManager.InsertLog(MyExceptionAppLogTO)
                '        MyTempLogManager.InsertLog(MyApplicationLogTO, InformSystem)
                '    Else
                '        'If the backup of the XML file failed, create a new one and write the exception and the initial error in the Log
                '        Dim MyExceptionAppLogTO As New ApplicationLogTO

                '        MyExceptionAppLogTO.LogDate = DateTime.Now
                '        MyExceptionAppLogTO.LogMessage = "The previous Xml Log file was wrong and it was no possible to recover it. A new one has been just created"
                '        MyExceptionAppLogTO.LogModule = "ApplicationLogManager"
                '        MyExceptionAppLogTO.LogType = System.Diagnostics.EventLogEntryType.Error

                '        Dim MyTempLogManager As New ApplicationLogManager(True)
                '        MyTempLogManager.InsertLog(MyExceptionAppLogTO, True)
                '        MyTempLogManager.InsertLog(MyApplicationLogTO, InformSystem)
                '    End If
                'Else
                ''Throw ex
                'Throw 'RH 02/12/2011 Remove ex 'AG 11/06/2012 - comment this line, when Access Denied error can cause not treat some instructions
                'End If
            End Try
        End Sub

        ''' <summary>
        ''' Get the information from  the load session of current domain and 
        ''' returna a ApplicationInfoSession.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSessionInfo() As ApplicationInfoSessionTO
            'Declare application session Object 
            Dim MyApplicationInfoSession As New ApplicationInfoSessionTO
            Try
                If Not AppDomain.CurrentDomain.GetData("ApplicationInfoSession") Is Nothing Then
                    MyApplicationInfoSession = CType(AppDomain.CurrentDomain.GetData("ApplicationInfoSession"), ApplicationInfoSessionTO)
                End If

            Catch ex As Exception
                CreateLogActivity(ex.Message, "ApplicationSessionManager.GetSessionInfo", EventLogEntryType.Error, False)
            End Try

            Return MyApplicationInfoSession

        End Function

        ''' <summary>
        ''' Get information from an Exception object and write the error information in the Application Log
        ''' </summary>
        ''' <param name="pException">NET Exception Object</param>
        ''' <remarks>
        ''' Created to replace current calls to CreateLogActivity
        ''' PENDING: in some cases the TargetSite.Name is not the method name. Not used yet 
        ''' </remarks>
        Public Sub ShowExceptionDetails(ByVal pException As Exception)
            'Try
            Dim errDoc As String = ""
            Dim errLine As String = ""
            Dim errMsg As String = pException.Message
            Dim errMethod As String = pException.TargetSite.Name

            If (pException.StackTrace.LastIndexOf(":") > -1) Then
                errLine = pException.StackTrace.Substring(pException.StackTrace.LastIndexOf(":") + 1)
                If (pException.StackTrace.LastIndexOf("\") > -1) Then
                    errDoc = pException.StackTrace.Substring(pException.StackTrace.LastIndexOf("\") + 1, _
                                                             pException.StackTrace.LastIndexOf(":") - pException.StackTrace.LastIndexOf("\") - 1)
                End If
            End If

            'Write error in the Application Log
            CreateLogActivity(errMsg & " " & errDoc & " " & errMethod & ": " & errLine, _
                              errDoc & "." & errMethod & ": " & errLine, _
                              EventLogEntryType.Error, False)
            'Catch ex As Exception
            'Throw ex
            'End Try
        End Sub
#End Region

    End Class

End Namespace