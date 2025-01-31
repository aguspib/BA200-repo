﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Este código fue generado por una herramienta.
'     Versión de runtime:4.0.30319.18444
'
'     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
'     se vuelve a generar el código.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "Funcionalidad para autoguardar de My.Settings"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(ByVal sender As Global.System.Object, ByVal e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\FwScripts\FwScriptsData.xml")>  _
        Public Property XmlFwScripts() As String
            Get
                Return CType(Me("XmlFwScripts"),String)
            End Get
            Set
                Me("XmlFwScripts") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\FwScripts\Factory\FactoryFwScriptsData.xml")>  _
        Public Property FactoryXmlFwScripts() As String
            Get
                Return CType(Me("FactoryXmlFwScripts"),String)
            End Get
            Set
                Me("FactoryXmlFwScripts") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\FwScripts\FwScriptsDataImporting.tmp")>  _
        Public Property XmlFwScriptsImporting() As String
            Get
                Return CType(Me("XmlFwScriptsImporting"),String)
            End Get
            Set
                Me("XmlFwScriptsImporting") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\FwScripts\FwScriptsDataDecrypting.tmp")>  _
        Public Property XmlFwScriptsDecrypting() As String
            Get
                Return CType(Me("XmlFwScriptsDecrypting"),String)
            End Get
            Set
                Me("XmlFwScriptsDecrypting") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\FwScripts\TempFile.tmp")>  _
        Public Property XmlFwScriptsTempFile() As String
            Get
                Return CType(Me("XmlFwScriptsTempFile"),String)
            End Get
            Set
                Me("XmlFwScriptsTempFile") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("C:\Program Files\Biosystems\")>  _
        Public Property AppPath() As String
            Get
                Return CType(Me("AppPath"),String)
            End Get
            Set
                Me("AppPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Service Sw\")>  _
        Public Property ServicePath() As String
            Get
                Return CType(Me("ServicePath"),String)
            End Get
            Set
                Me("ServicePath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\User Sw\")>  _
        Public Property UserPath() As String
            Get
                Return CType(Me("UserPath"),String)
            End Get
            Set
                Me("UserPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Images\")>  _
        Public Property ImagesPath() As String
            Get
                Return CType(Me("ImagesPath"),String)
            End Get
            Set
                Me("ImagesPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Log\")>  _
        Public Property XmlLogFilePath() As String
            Get
                Return CType(Me("XmlLogFilePath"),String)
            End Get
            Set
                Me("XmlLogFilePath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\")>  _
        Public Property PCOSInfoFilePath() As String
            Get
                Return CType(Me("PCOSInfoFilePath"),String)
            End Get
            Set
                Me("PCOSInfoFilePath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Version.txt")>  _
        Public Property VersionFileName() As String
            Get
                Return CType(Me("VersionFileName"),String)
            End Get
            Set
                Me("VersionFileName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute(".SAT")>  _
        Public Property ZIPExtension() As String
            Get
                Return CType(Me("ZIPExtension"),String)
            End Get
            Set
                Me("ZIPExtension") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("3.08")>  _
        Public Property DataBaseVersion() As String
            Get
                Return CType(Me("DataBaseVersion"),String)
            End Get
            Set
                Me("DataBaseVersion") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property AllowDBGreaterVersion() As Boolean
            Get
                Return CType(Me("AllowDBGreaterVersion"),Boolean)
            End Get
            Set
                Me("AllowDBGreaterVersion") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property ClientSettingsProvider_ServiceUri() As String
            Get
                Return CType(Me("ClientSettingsProvider_ServiceUri"),String)
            End Get
            Set
                Me("ClientSettingsProvider_ServiceUri") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("115200")>  _
        Public Property PortSpeed() As Integer
            Get
                Return CType(Me("PortSpeed"),Integer)
            End Get
            Set
                Me("PortSpeed") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property LastPortConnected() As String
            Get
                Return CType(Me("LastPortConnected"),String)
            End Get
            Set
                Me("LastPortConnected") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("115200")>  _
        Public Property PortSpeedList() As String
            Get
                Return CType(Me("PortSpeedList"),String)
            End Get
            Set
                Me("PortSpeedList") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property RealDevelopmentMode() As Integer
            Get
                Return CType(Me("RealDevelopmentMode"),Integer)
            End Get
            Set
                Me("RealDevelopmentMode") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property WriteToSystemLog() As Boolean
            Get
                Return CType(Me("WriteToSystemLog"),Boolean)
            End Get
            Set
                Me("WriteToSystemLog") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Data\")>  _
        Public Property DataPath() As String
            Get
                Return CType(Me("DataPath"),String)
            End Get
            Set
                Me("DataPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Data Base\")>  _
        Public Property DataBasePath() As String
            Get
                Return CType(Me("DataBasePath"),String)
            End Get
            Set
                Me("DataBasePath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Adjustments\")>  _
        Public Property FwAdjustmentsPath() As String
            Get
                Return CType(Me("FwAdjustmentsPath"),String)
            End Get
            Set
                Me("FwAdjustmentsPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\XmlFiles\ISEModuleParammeters.xml")>  _
        Public Property ISEParammetersFilePath() As String
            Get
                Return CType(Me("ISEParammetersFilePath"),String)
            End Get
            Set
                Me("ISEParammetersFilePath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\XmlFiles\ISEResultsDebugMode.xml")>  _
        Public Property ISEResultDebugModeFilePath() As String
            Get
                Return CType(Me("ISEResultDebugModeFilePath"),String)
            End Get
            Set
                Me("ISEResultDebugModeFilePath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Update\bak\Ax00.bak")>  _
        Public Property InstallDBBackupFilePath() As String
            Get
                Return CType(Me("InstallDBBackupFilePath"),String)
            End Get
            Set
                Me("InstallDBBackupFilePath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Update\Task\TaskList.xml")>  _
        Public Property InstallDBTaskFilePath() As String
            Get
                Return CType(Me("InstallDBTaskFilePath"),String)
            End Get
            Set
                Me("InstallDBTaskFilePath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2")>  _
        Public Property ContaminationReagentPersistance() As Integer
            Get
                Return CType(Me("ContaminationReagentPersistance"),Integer)
            End Get
            Set
                Me("ContaminationReagentPersistance") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Adjustments\PhotometryTests.bin")>  _
        Public Property PhotometryTestsFile() As String
            Get
                Return CType(Me("PhotometryTestsFile"),String)
            End Get
            Set
                Me("PhotometryTestsFile") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Data Source=(local)\SQLEXPRESS;Initial Catalog=Ax00TEM;user=sa;Password=BIOSYSTEM"& _ 
            "S")>  _
        Public Property UpdateDatabaseConnectionString() As String
            Get
                Return CType(Me("UpdateDatabaseConnectionString"),String)
            End Get
            Set
                Me("UpdateDatabaseConnectionString") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Adjustments\BLResults.res")>  _
        Public Property PhotometryBLFileOut() As String
            Get
                Return CType(Me("PhotometryBLFileOut"),String)
            End Get
            Set
                Me("PhotometryBLFileOut") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Adjustments\RepeatabilityResults.res")>  _
        Public Property PhotometryRepeatabilityFileOut() As String
            Get
                Return CType(Me("PhotometryRepeatabilityFileOut"),String)
            End Get
            Set
                Me("PhotometryRepeatabilityFileOut") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Adjustments\StabilityResults.res")>  _
        Public Property PhotometryStabilityFileOut() As String
            Get
                Return CType(Me("PhotometryStabilityFileOut"),String)
            End Get
            Set
                Me("PhotometryStabilityFileOut") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("3")>  _
        Public Property MaxRepetitionsTimeout() As Integer
            Get
                Return CType(Me("MaxRepetitionsTimeout"),Integer)
            End Get
            Set
                Me("MaxRepetitionsTimeout") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("8000")>  _
        Public Property OpticalCenteringCurrentLed() As Integer
            Get
                Return CType(Me("OpticalCenteringCurrentLed"),Integer)
            End Get
            Set
                Me("OpticalCenteringCurrentLed") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("1000")>  _
        Public Property OpticalCenteringStepLed() As Integer
            Get
                Return CType(Me("OpticalCenteringStepLed"),Integer)
            End Get
            Set
                Me("OpticalCenteringStepLed") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("5")>  _
        Public Property OpticalCenteringWellsToRead() As Integer
            Get
                Return CType(Me("OpticalCenteringWellsToRead"),Integer)
            End Get
            Set
                Me("OpticalCenteringWellsToRead") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("80")>  _
        Public Property OpticalCenteringStepsbyWell() As Integer
            Get
                Return CType(Me("OpticalCenteringStepsbyWell"),Integer)
            End Get
            Set
                Me("OpticalCenteringStepsbyWell") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("AX00Log.xml")>  _
        Public Property XmlLogFile() As String
            Get
                Return CType(Me("XmlLogFile"),String)
            End Get
            Set
                Me("XmlLogFile") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("50")>  _
        Public Property ScalesDinamicRange() As Integer
            Get
                Return CType(Me("ScalesDinamicRange"),Integer)
            End Get
            Set
                Me("ScalesDinamicRange") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Adjustments\AnalyzerInfo.txt")>  _
        Public Property AnalyzerInfoFileOut() As String
            Get
                Return CType(Me("AnalyzerInfoFileOut"),String)
            End Get
            Set
                Me("AnalyzerInfoFileOut") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\QCResultsFiles\")>  _
        Public Property QCResultsFilesPath() As String
            Get
                Return CType(Me("QCResultsFilesPath"),String)
            End Get
            Set
                Me("QCResultsFilesPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\InfoDocs\")>  _
        Public Property ServiceInfoDocsPath() As String
            Get
                Return CType(Me("ServiceInfoDocsPath"),String)
            End Get
            Set
                Me("ServiceInfoDocsPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("MSInfo32Report.nfo")>  _
        Public Property MSInfo32ReportName() As String
            Get
                Return CType(Me("MSInfo32ReportName"),String)
            End Get
            Set
                Me("MSInfo32ReportName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Reports\Templates")>  _
        Public Property ReportPath() As String
            Get
                Return CType(Me("ReportPath"),String)
            End Get
            Set
                Me("ReportPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Data Source=(local)\SQLEXPRESS;Initial Catalog=Ax00;user=sa;Password=BIOSYSTEMS;P"& _ 
            "ersist Security Info=true;min pool size=1; max pool size=10;")>  _
        Public Property BiosystemsConnDev() As String
            Get
                Return CType(Me("BiosystemsConnDev"),String)
            End Get
            Set
                Me("BiosystemsConnDev") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property IsSystemLogFull() As Boolean
            Get
                Return CType(Me("IsSystemLogFull"),Boolean)
            End Get
            Set
                Me("IsSystemLogFull") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("3")>  _
        Public Property SNUpperPartSize() As Integer
            Get
                Return CType(Me("SNUpperPartSize"),Integer)
            End Get
            Set
                Me("SNUpperPartSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("6")>  _
        Public Property SNLowerPartSize() As Integer
            Get
                Return CType(Me("SNLowerPartSize"),Integer)
            End Get
            Set
                Me("SNLowerPartSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("834")>  _
        Public Property BA400ModelID() As String
            Get
                Return CType(Me("BA400ModelID"),String)
            End Get
            Set
                Me("BA400ModelID") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("RestorePoint End Version")>  _
        Public Property UpdateVersionRestorePointName() As String
            Get
                Return CType(Me("UpdateVersionRestorePointName"),String)
            End Get
            Set
                Me("UpdateVersionRestorePointName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("\Previous\")>  _
        Public Property PreviousFolder() As String
            Get
                Return CType(Me("PreviousFolder"),String)
            End Get
            Set
                Me("PreviousFolder") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Ax00TEM")>  _
        Public Property TemporalDBName() As String
            Get
                Return CType(Me("TemporalDBName"),String)
            End Get
            Set
                Me("TemporalDBName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Update\bak\")>  _
        Public Property BakDirectoryPath() As String
            Get
                Return CType(Me("BakDirectoryPath"),String)
            End Get
            Set
                Me("BakDirectoryPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("C:\TEMP\")>  _
        Public Property TemporalDirectory() As String
            Get
                Return CType(Me("TemporalDirectory"),String)
            End Get
            Set
                Me("TemporalDirectory") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property IsServiceAssembly() As Boolean
            Get
                Return CType(Me("IsServiceAssembly"),Boolean)
            End Get
            Set
                Me("IsServiceAssembly") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("UpdateLogError.xml")>  _
        Public Property UpdateLogFile() As String
            Get
                Return CType(Me("UpdateLogFile"),String)
            End Get
            Set
                Me("UpdateLogFile") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("RSATUpdateError")>  _
        Public Property RSATUpdateErrorName() As String
            Get
                Return CType(Me("RSATUpdateErrorName"),String)
            End Get
            Set
                Me("RSATUpdateErrorName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("SynapseEventLog")>  _
        Public Property SynapseLogFileName() As String
            Get
                Return CType(Me("SynapseLogFileName"),String)
            End Get
            Set
                Me("SynapseLogFileName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("BA200User.exe")>  _
        Public Property UserSwExeName() As String
            Get
                Return CType(Me("UserSwExeName"),String)
            End Get
            Set
                Me("UserSwExeName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("PresentationUSR")>  _
        Public Property UserProjectName() As String
            Get
                Return CType(Me("UserProjectName"),String)
            End Get
            Set
                Me("UserProjectName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("PresentationSRV")>  _
        Public Property ServiceProjectName() As String
            Get
                Return CType(Me("ServiceProjectName"),String)
            End Get
            Set
                Me("ServiceProjectName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Ax00UpdateVersion.xml")>  _
        Public Property UpdateVersionProcessLogFileName() As String
            Get
                Return CType(Me("UpdateVersionProcessLogFileName"),String)
            End Get
            Set
                Me("UpdateVersionProcessLogFileName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("1")>  _
        Public Property MaxRepetitionsRetry() As Integer
            Get
                Return CType(Me("MaxRepetitionsRetry"),Integer)
            End Get
            Set
                Me("MaxRepetitionsRetry") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Ax00TEM.bak")>  _
        Public Property DBBackUpFileName() As String
            Get
                Return CType(Me("DBBackUpFileName"),String)
            End Get
            Set
                Me("DBBackUpFileName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Ax00TEM.bak")>  _
        Public Property TempDBBakupFileName() As String
            Get
                Return CType(Me("TempDBBakupFileName"),String)
            End Get
            Set
                Me("TempDBBakupFileName") = value
            End Set
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.My.MySettings
            Get
                Return Global.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
