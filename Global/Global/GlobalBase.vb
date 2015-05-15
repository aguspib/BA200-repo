Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global.TO
Imports System.Reflection
Imports System.IO
Imports System.Text


Namespace Biosystems.Ax00.Global

    ''' <summary>
    ''' Base class for all the global classes, all global clases must inherits from
    ''' GlobalBase Clase, to add all the general funtionality create here.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class GlobalBase

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
        Public Shared ReadOnly Property FactoryXmlFwScripts() As String
            Get
                Return My.Settings.FactoryXmlFwScripts
            End Get
        End Property

        'SG 09/11/10 - SERVICE SOFTWARE
        Public Shared ReadOnly Property XmlFwScriptsWhileDecrypting() As String
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
        Public Shared ReadOnly Property XmlFwScriptsTempFile() As String
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
        Public Shared ReadOnly Property ServiceInfoDocsPath() As String
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

        Public Shared ReadOnly Property ServicePath() As String
            Get
#If DEBUG Then
                Return AppPath
#Else
                Return AppPath & My.Settings.ServicePath
#End If
            End Get
        End Property

        'SA 15/05/2014 - BT #1617 ==> New property to return the full path for the exe file of UserSW. A new Global Setting with the name
        '                             of the exe file has been also created (named as UserSwExeName)
        Public Shared ReadOnly Property UserSwExeFullPath() As String
            Get
                Dim myFullPath As String = Path.GetFullPath(My.Settings.UserSwExeName)

#If DEBUG Then
                'DEBUG MODE
                If (myFullPath.Contains(My.Settings.ServiceProjectName)) Then
                    Return myFullPath.Replace(My.Settings.ServiceProjectName, My.Settings.UserProjectName)
                Else
                    Return myFullPath
                End If
#Else
                'RELEASE MODE
                If (myFullPath.Contains(My.Settings.ServiceProjectName)) Then
                    Return myFullPath.Replace(My.Settings.ServiceProjectName, My.Settings.UserProjectName)

                ElseIf (myFullPath.Contains(My.Settings.ServicePath)) Then
                    Return myFullPath.Replace(My.Settings.ServicePath, My.Settings.UserPath)

                Else
                    Return myFullPath
                End If
#End If
            End Get
        End Property

        Shared ReadOnly Property ImagesPath() As String
            Get
                Return My.Settings.ImagesPath
            End Get
        End Property

        ' XBC 03/12/2010 - Common folders (User & Service)
        Public Shared ReadOnly Property DataBaseConnection() As String
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
        Public Shared ReadOnly Property ScalesDinamicRange() As Integer
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

        Protected Friend Shared Property IsSystemLogFull() As Boolean
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


        'Public Property SynapseNumberOfDays() As Integer
        '    Get
        '        Return SynapseNumberOfDaysAttribute
        '    End Get
        '    Set(value As Integer)
        '        SynapseNumberOfDaysAttribute = value
        '    End Set
        'End Property

        'SA 20/10/2014
        'BA-1944: Read new setting for the name of the XML file containing all changes made in CUSTOMER DB for the Update Version Process
        Shared ReadOnly Property UpdateVersionProcessLogFileName() As String
            Get
                Return My.Settings.UpdateVersionProcessLogFileName
            End Get
        End Property

        Shared ReadOnly Property WriteToSystemLog() As Boolean
            Get
                Return My.Settings.WriteToSystemLog
            End Get
        End Property

        ' XBC 05/05/2011
        Shared ReadOnly Property MaxRepetitionsRetry() As Integer
            Get
                Return My.Settings.MaxRepetitionsRetry
            End Get
        End Property

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
        Public Shared Sub CreateLogActivity(ByVal Message As String, ByVal LogModule As String, ByVal LogType As EventLogEntryType, ByVal InformSystem As Boolean)

            Dim MyApplicationLogTO As New ApplicationLogTO
            'Dim MyApplicationLogManager As New ApplicationLogManager

            Try
                'Fill the ApplicationLogTO with the information to insert in the Log'
                MyApplicationLogTO.LogDate = DateTime.Now
                MyApplicationLogTO.LogMessage = Message
                MyApplicationLogTO.LogModule = LogModule
                MyApplicationLogTO.LogType = LogType

                ApplicationLogManager.InsertLog(MyApplicationLogTO)

            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>
        ''' This function is used to create log activity for a runtime exception
        ''' </summary>
        ''' <param name="ex">A execption to be notified into the application log</param>
        ''' <remarks>Use this function to add exception to the application log when possible.</remarks>
        Public Shared Sub CreateLogActivity(ex As Exception)

            ' ReSharper disable once InconsistentNaming
            Const MAXIMUM_INNER_EXCEPTION_LEVELS = 10

            Try
                If ex IsNot Nothing AndAlso ex.TargetSite IsNot Nothing Then
                    Dim exceptionIterator = ex, exceptionDesc As New StringBuilder, count = 0

                    '1.- We iterate the exception and all innerException data (up to 10 levels) to build a detailed explanation of the unhanled exception:
                    While exceptionIterator IsNot Nothing And count < MAXIMUM_INNER_EXCEPTION_LEVELS
                        exceptionDesc.Append(exceptionIterator.Message & " ((" & exceptionIterator.HResult & "))")
                        exceptionIterator = exceptionIterator.InnerException
                        If exceptionIterator IsNot Nothing Then
                            exceptionIterator = exceptionIterator.InnerException
                            If exceptionIterator IsNot Nothing Then
                                exceptionDesc.Append("Additional inner exception found: ")
                            End If
                        End If
                        count += 1    'Cross reference prevention
                    End While

                    '2.- We append the call stack information, with delimiters (to make it a bit easier to read)
                    exceptionDesc.Append(vbCr & vbCr & "CallStack:" & vbCr)
                    exceptionDesc.Append(ex.StackTrace)
                    exceptionDesc.Append("--------------")
                    exceptionDesc.Append(Environment.StackTrace)
                    exceptionDesc.Append(vbCr & "end of CallStack" & vbCr & vbCr)

                    '2.- We inspect the exception origin by inspecing the target of main exception 
                    Dim sourceOfException = ex.Source &
                        ex.TargetSite.DeclaringType.ToString & "." &
                        ex.TargetSite.Name

                    Dim exceptionCompleteMessage = exceptionDesc.ToString

                    GlobalBase.CreateLogActivity(exceptionCompleteMessage, sourceOfException, EventLogEntryType.Error, False)
#If config = "Debug" Then
                    Debug.WriteLine("EXCEPTION LOGGED: " & exceptionCompleteMessage)
#End If

                Else
                    GlobalBase.CreateLogActivity(
                        "An Unhandled Exception has been occurred, but exception data is missing ",
                        Environment.StackTrace, EventLogEntryType.Error, False)
                End If

#If config = "Debug" Then
            Catch ex2 As Exception
                MsgBox("The application has been unable to report an exception. This means the log system is failing too at some point." & vbCr & vbCr & "Happy debugging!", MsgBoxStyle.Critical, "Something went wrong")
#Else
                Catch
#End If

            End Try
        End Sub

        ''' <summary>
        ''' Get the information from  the load session of current domain and 
        ''' returna a ApplicationInfoSession.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSessionInfo() As ApplicationInfoSessionTO
            'Declare application session Object 
            Dim MyApplicationInfoSession As New ApplicationInfoSessionTO
            Try
                If Not AppDomain.CurrentDomain.GetData("ApplicationInfoSession") Is Nothing Then
                    MyApplicationInfoSession = CType(AppDomain.CurrentDomain.GetData("ApplicationInfoSession"), ApplicationInfoSessionTO)
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "ApplicationSessionManager.GetSessionInfo", EventLogEntryType.Error, False)
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
        Public Shared Sub ShowExceptionDetails(ByVal pException As Exception)
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
            GlobalBase.CreateLogActivity(errMsg & " " & errDoc & " " & errMethod & ": " & errLine, _
                              errDoc & "." & errMethod & ": " & errLine, _
                              EventLogEntryType.Error, False)
            'Catch ex As Exception
            'Throw ex
            'End Try
        End Sub
#End Region

    End Class

End Namespace