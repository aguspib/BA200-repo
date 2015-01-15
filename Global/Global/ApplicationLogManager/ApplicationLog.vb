Option Explicit On
Option Strict On

Imports System.IO
Imports System.Xml
Imports Biosystems.Ax00.Global.TO
Imports System.Windows.Forms
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.DAL


Namespace Biosystems.Ax00.Global

    ''' <summary>
    ''' Class than handle the Application log, create and insert logs
    ''' into an XML File Document, And/Or System Log.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ApplicationLogManager
        Inherits GlobalBase

#Region "Declarations"
        Private Const TEMP_FOLDER As String = "TEMP"
        Private Const PREVIOUS_LOG_ZIP As String = "PreviousLog.zip"
        Private Const CORRUPTED_LOG_ZIP As String = "CorruptedLog.zip"
#End Region

#Region "NEW IMPLEMENTATION: LOG IS SAVED IN A DB TABLE AND MOVED TO AN XML FILE WHEN RESET THE ACTIVE WORKSESSION"
        Dim WriteSystemLog As Boolean

        ''' <summary>
        ''' Insert log information into database instead of in an XML Log File
        ''' </summary>
        ''' <param name="pApplicationLogTO">Object containing the log information to add to the Application Log table</param>
        ''' <remarks>
        ''' Created by: TR 03/07/2012
        ''' Modified by: SG 18/02/2013 - add Xml log file for database update version process
        ''' </remarks>
        Public Sub InsertLog(ByVal pApplicationLogTO As ApplicationLogTO)
            Dim myGlobalDataTO As New GlobalDataTO
            Try

                Dim myApplicationLogList As New List(Of ApplicationLogTO)
                myApplicationLogList.Add(pApplicationLogTO)

                myGlobalDataTO = Create(myApplicationLogList)

                'SGM 18/02/2013
                If SystemInfoManager.IsUpdateProcess Then
                    MyClass.InsertUpdateVersionLog(pApplicationLogTO)
                End If
                'end SGM 18/02/2013

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
        End Sub

        ''' <summary>
        ''' Insert log information into database on table tfmApplicationLog
        ''' </summary>
        ''' <param name="pApplicationLogTOList">List of objects containing the log information to add to the 
        '''                                     Application Log table</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 03/07/2012
        ''' </remarks>
        Public Function Create(ByVal pApplicationLogTOList As List(Of ApplicationLogTO)) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Try
                Dim myApplicationLogDAO As New tfmwApplicationLogDAO
                myGlobalDataTO = myApplicationLogDAO.Create(pApplicationLogTOList)
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all log information in table tfmwApplicationLog (once they have been exported to an XML file)
        ''' </summary>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 07/03/2012
        ''' </remarks>
        Public Function DeleteAll() As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Try
                Dim myApplicationLogDAO As New tfmwApplicationLogDAO
                myGlobalDataTO = myApplicationLogDAO.DeleteAll
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ' Problems with Regional Settings Date format because do not use Invariant format
        ' ''' <summary>
        ' ''' Delete all records by date
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pMonthsToDelete">months to delete</param>
        ' ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  XB 13/11/2013
        ' ''' </remarks>
        'Public Function DeleteByDate(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pMonthsToDelete As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                Dim myDAO As New tfmwApplicationLogDAO
        '                resultData = myDAO.DeleteByDate(dbConnection, pMonthsToDelete)

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "ApplicationLog.DeleteByDate", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        ''' <summary>
        ''' Function for Log Files management:
        ''' ** Get all XML Log Files that exist in the LOG Path
        ''' ** Get the date part of the last created XML Log File and compare it with the date received:
        ''' ***** If it is the same date: get the index part of the last created XML Log File and increment it by one and built and returned
        '''                               the name for the next XML Log File:  "Ax00Log_" + pCurrentDate + nextIndex + ".xml"
        ''' ***** If it is a different date: all files in the LOG Path are moved to a PreviousLog ZIP. The name for the next Log File is
        '''                                  built and returned:  "Ax00Log_" + pCurrentDate + "001" + ".xml"
        ''' </summary>
        ''' <param name="pCurrentDate">String containing the current date formatted as YYYYMMDD</param>
        ''' <returns>GlobalDataTO containing an string value with the name of the next XML Log File to create</returns>
        ''' <remarks>
        ''' Created by:  SA 03/08/2012
        ''' Modified by: XB 28/05/2013 - Correction : Condition must done by Directory instead of File (bugstranking: # 1139)
        ''' AG 08/05/2014 - #1625 add protections for not return always error when exists files with not std name format (for example "Ax00Log_999.xml")
        ''' </remarks>
        Private Function ManageLogFiles(ByVal pCurrentDate As String, _
                                        ByVal pLogMaxDays As Integer) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myLogPath As String = Application.StartupPath & GlobalBase.XmlLogFilePath

                ' XBC+TR 03/10/2012 - Correction
                'If Not File.Exists(Application.StartupPath & GlobalBase.XmlLogFilePath) Then
                If Not Directory.Exists(Application.StartupPath & GlobalBase.XmlLogFilePath) Then ' XB 28/05/2013 - Correction : condition must done by Directory instead of File
                    Directory.CreateDirectory(Application.StartupPath & GlobalBase.XmlLogFilePath)
                End If
                ' XBC+TR 03/10/2012 - Correction

                Dim myLogFile As IO.FileInfo
                Dim myLogDIR As New IO.DirectoryInfo(myLogPath)
                Dim myLogFilesList As IO.FileInfo() = myLogDIR.GetFiles("Ax00Log_*.xml")

                'Move all XML Files to a string list
                Dim lstXMLFiles As New List(Of String)
                Dim eachFileDate As String
                Dim endday As Date
                Dim currentday As Date = Date.Parse(pCurrentDate.Substring(0, 4) & "-" & pCurrentDate.Substring(4, 2) & "-" & pCurrentDate.Substring(6, 2))

                For Each myLogFile In myLogFilesList
                    lstXMLFiles.Add(myLogFile.ToString)
                Next
                'DL 31/05/2013

                'Get the XML File with the most recent date (the last created)
                Dim index As Integer = 1
                Dim lastCreatedXML As String = String.Empty
                If (lstXMLFiles.Count > 0) Then

                    lastCreatedXML = (From b As String In lstXMLFiles
                                      Select b).Max.ToString

                    'Get the date part from the name of the last created XML file
                    'AG 08/05/2014 - #1625 new code, when the xml file "Ax00Log_999.xml" is generate once this method always returns error, and Sw overwrites always "Ax00Log_999.xml"
                    'Dim fileDate As String = lastCreatedXML.Substring(8, 8) 'Old code
                    Dim fileDate As String = String.Format("{0:yyyyMMdd}", DateTime.Now) 'File date initialization
                    If lastCreatedXML.Length >= 16 Then
                        fileDate = lastCreatedXML.Substring(8, 8)
                    End If
                    'AG 08/07/2014 - #1625

                    If (fileDate <> pCurrentDate) Then
                        'Create the TEMP Folder to move all XMLLog Files to compress
                        Dim myUtils As New Utilities
                        Dim myTempFolder As String = myLogPath & TEMP_FOLDER 'DL 04/06/2013

                        'TR 05/09/2012 -Change File Exist by Directory exist to validate if the TEMP folder exist.
                        If (Directory.Exists(myTempFolder)) Then myGlobalDataTO = myUtils.RemoveFolder(myTempFolder)
                        If (Not myGlobalDataTO.HasError) Then myGlobalDataTO = myUtils.CreateFolder(myTempFolder)

                        'If the ZIP File already exists, extract in the TEMP Folder all Log Files it contains and delete the ZIP File
                        If (Not myGlobalDataTO.HasError) Then
                            If (File.Exists(myLogPath & PREVIOUS_LOG_ZIP)) Then
                                myGlobalDataTO = myUtils.ExtractFromZip(myLogPath & PREVIOUS_LOG_ZIP, myTempFolder & "\")
                                If (Not myGlobalDataTO.HasError) Then
                                    If myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.ZIP_ERROR.ToString Then
                                        myGlobalDataTO = myUtils.MoveFiles(myLogPath, myTempFolder & "\", PREVIOUS_LOG_ZIP, CORRUPTED_LOG_ZIP)
                                    Else
                                        Kill(myLogPath & PREVIOUS_LOG_ZIP)
                                    End If
                                End If
                            End If
                        End If

                        'Evaluate the xml extracted from ZIP into TEMP Folder (remove the older than pLogMaxDays)
                        If (Not myGlobalDataTO.HasError) Then
                            Dim myTmpDIR As New IO.DirectoryInfo(myTempFolder)                       'DL 04/06/2013
                            Dim myTmpFilesList As IO.FileInfo() = myTmpDIR.GetFiles("Ax00Log_*.xml") 'DL 04/06/2013
                            Dim myTmpFile As IO.FileInfo                                             'DL 04/06/2013
                            Dim myXmlFile As String                                                  'DL 04/06/2013

                            'For Each xmlFile As String In lstXMLFiles                               'DL 04/06/2013
                            Dim deleteFileFlag As Boolean = False 'AG 08/05/2014 - #1625
                            For Each myTmpFile In myTmpFilesList                                     'DL 04/06/2013
                                myXmlFile = myTmpFile.ToString

                                'AG 08/05/2014 - #1625 add protection in case file with not std name(for example Ax00Log_999.xml)
                                'Old code
                                'deleteFileFlag = True
                                'eachFileDate = myXmlFile.ToString.Substring(8, 8)
                                'endday = Date.Parse(eachFileDate.Substring(0, 4) & "-" & eachFileDate.Substring(4, 2) & "-" & eachFileDate.Substring(6, 2))

                                ''DL 01/06/2013. Add only files witch days elapsed are lower than MAX_DAYS_IN_PREVIOUSLOG
                                'If currentday.Subtract(endday).Days <= pLogMaxDays Then
                                '    'AG 04/06/2013 - Do not move nothing, there already are in Temp Folder
                                '    'myGlobalDataTO = myUtils.MoveFiles(myLogPath, myTempFolder & "\", myXmlFile)

                                '    ''Delete the XML file
                                '    'If (Not myGlobalDataTO.HasError) Then File.Delete(myLogPath & "\" & myXmlFile)
                                '    'If (myGlobalDataTO.HasError) Then Exit For

                                '    'Leave file!!! (it belongs the correct date range)
                                '    deleteFileFlag = False
                                'End If

                                'New code
                                deleteFileFlag = True
                                If myXmlFile.Length >= 16 Then
                                    eachFileDate = myXmlFile.ToString.Substring(8, 8)
                                    If IsDate("#" & eachFileDate.ToString & "#") OrElse IsNumeric(eachFileDate.ToString) Then
                                        endday = Date.Parse(eachFileDate.Substring(0, 4) & "-" & eachFileDate.Substring(4, 2) & "-" & eachFileDate.Substring(6, 2))

                                        'DL 01/06/2013. Add only files witch days elapsed are lower than MAX_DAYS_IN_PREVIOUSLOG
                                        If currentday.Subtract(endday).Days <= pLogMaxDays Then
                                            'Leave file!!! (it belongs the correct date range)
                                            deleteFileFlag = False
                                        End If
                                    End If
                                End If

                                If deleteFileFlag Then
                                    File.Delete(myTempFolder & "\" & myXmlFile)
                                End If
                                'AG 08/05/2014 - #1625
                            Next

                        End If

                        'AG 04/06/2013 - Move all xml from logFolder to Temp folder
                        For Each myLogFile In myLogFilesList
                            myGlobalDataTO = myUtils.MoveFiles(myLogPath, myTempFolder & "\", myLogFile.ToString)
                        Next
                        'AG 04/06/2013

                        'Finally, create a new ZIP File, compress all Files in the TEMP Folder (xml and CorruptedLog.zip (if exists) ), and remove the TEMP Folder
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myUtils.CompressToZip(myLogPath & TEMP_FOLDER & "\", myLogPath & PREVIOUS_LOG_ZIP)
                            If (Not myGlobalDataTO.HasError) Then myGlobalDataTO = myUtils.RemoveFolder(myLogPath & TEMP_FOLDER)
                        End If
                    Else
                        'AG 08/05/2014 - #1625 add protection against user file rename

                        'Old code
                        ''The if is to avoid ERROR with files generated with the previous implementation (when the last part of the file was HH-mm): if it is the
                        ''case, then generate the 001 file for the current date
                        'If (IsNumeric(lastCreatedXML.Substring(19, 1))) Then
                        '    'If its the same date, get the index part from the name of the last created XML and generate the next index incrementing it by one
                        '    index = Convert.ToInt32(lastCreatedXML.Substring(17, 3)) + 1
                        'Else
                        '    'Search if there are XML files created with the new implementation
                        '    For Each xmlFile As String In lstXMLFiles
                        '        If (IsNumeric(xmlFile.Substring(19, 1))) Then
                        '            If (Convert.ToInt32(xmlFile.Substring(17, 3)) > index) Then index = Convert.ToInt32(xmlFile.Substring(17, 3))
                        '        End If
                        '    Next
                        '    index += 1
                        'End If

                        'New code
                        If lastCreatedXML.Length >= 16 Then
                            'The if is to avoid ERROR with files generated with the previous implementation (when the last part of the file was HH-mm): if it is the
                            'case, then generate the 001 file for the current date
                            If (IsNumeric(lastCreatedXML.Substring(17, 3))) Then
                                'If its the same date, get the index part from the name of the last created XML and generate the next index incrementing it by one
                                index = Convert.ToInt32(lastCreatedXML.Substring(17, 3)) + 1
                            Else
                                'Search if there are XML files created with the new implementation
                                For Each xmlFile As String In lstXMLFiles
                                    If (IsNumeric(xmlFile.Substring(17, 3))) Then
                                        If (Convert.ToInt32(xmlFile.Substring(17, 3)) > index) Then index = Convert.ToInt32(xmlFile.Substring(17, 3))
                                    End If
                                Next
                                index += 1
                            End If
                        Else
                            index = 999 'AG 08/05/2014 - #1625 force index value
                        End If
                        'AG 08/05/2014 - #1625

                    End If
                End If

                'AG 08/05/2014 - #1625 protection in case index 999 is found (some error detected) the index is calculated using list count + 1
                If index = 999 Then
                    index = lstXMLFiles.Count + 1
                End If
                'AG 08/05/2014 - #1625

                'Finally, set the name of the new XML Log File to create and return it in the GlobalDataTO
                Dim nextIndex As String = index.ToString
                If (index < 10) Then
                    nextIndex = "00" & nextIndex
                ElseIf (index < 100) Then
                    nextIndex = "0" & nextIndex
                End If

                myGlobalDataTO.SetDatos = "Ax00Log_" & pCurrentDate & "_" & nextIndex & ".xml"


            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Move all data in table tfmwApplicationLog to an XML File in the application LOG Path
        ''' </summary>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 03/08/2012 - Based in ExportToLogXML but adding functionality for moving to a ZIP previous XML files
        ''' Modified by: DL 31/05/2013 - Add new parameter pLogMaxDays - Copy into PreviousLog.zin only those xml files in list pxmlList witch days elapsed are lower than MAX_DAYS_IN_PREVIOUSLOG
        ''' AG 08/05/2014 #1625 fix error in code v300 (do not use "Ax00Log_999.xml" it is not the std name instead of it use "Ax00Log_YYYYMMDD_999.xml"
        ''' </remarks>
        Public Function ExportLogToXml(ByVal pWorkSessionID As String, _
                                       ByVal pLogMaxDays As Integer) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing

            Try
                'Get the date part for the identifier of the active WorkSession
                Dim wsDatePart As String = pWorkSessionID.Substring(0, 8)
                Dim myLogPath As String = Application.StartupPath & GlobalBase.XmlLogFilePath
                Dim myFileName As String

                'Search the name of the next XML file to create, and verify if XML files from previous days have to be moved to a zip file
                myGlobalDataTO = ManageLogFiles(wsDatePart, pLogMaxDays)

                'DL 31/05/2013
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myFileName = CType(myGlobalDataTO.SetDatos, String)
                Else
                    'AG 08/05/2014 - #1625 use the std name format
                    'myFileName = "Ax00Log_999.xml"
                    myFileName = "Ax00Log_" & wsDatePart & "_999.xml"
                    If File.Exists(myLogPath & myFileName) Then Kill(myLogPath & myFileName)
                End If

                'TR 03/10/2012 -Validate if directory exit other wise create
                If Not Directory.Exists(myLogPath) Then Directory.CreateDirectory(myLogPath)

                'Get all data in table tfmwApplicationLog and write them in the XML file
                Dim myApplicationLogDS As ApplicationLogDS
                Dim myApplicationLogDAO As New tfmwApplicationLogDAO

                myGlobalDataTO = myApplicationLogDAO.ReadAll()
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myApplicationLogDS = DirectCast(myGlobalDataTO.SetDatos, ApplicationLogDS)
                    'Write the XML file
                    myApplicationLogDS.WriteXml(myLogPath & myFileName)
                End If
                'DL 31/05/2013


            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Export all the information on Aplication Log table to an XML file
        ''' </summary>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR/DL 03/07/2012 
        ''' </remarks>
        Public Function ExportLogToXmlOLD() As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Try
                Dim myLogPath As String = Application.StartupPath & GlobalBase.XmlLogFilePath
                Dim myFileName As String = "Ax00Log_" & Now.ToString("yyyyMMdd HH-mm") & ".xml"

                Dim myApplicationLogDAO As New tfmwApplicationLogDAO
                Dim myApplicationLogDS As ApplicationLogDS
                myGlobalDataTO = myApplicationLogDAO.ReadAll()
                If Not myGlobalDataTO.HasError Then
                    myApplicationLogDS = DirectCast(myGlobalDataTO.SetDatos, ApplicationLogDS)
                    myApplicationLogDS.WriteXml(myLogPath & myFileName)
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "UPDATE VERSION"

        Private Shared LockObject As Object 'RH 26/03/2012
        Private myUpdateLogPath As String = Application.StartupPath & My.Settings.PreviousFolder & GlobalBase.UpdateLogFile

        '''' <summary>
        '''' Insert a Log Activity into the Log file, and System Log if WriteSystemLog is true
        '''' </summary>
        '''' <param name="MyApplicationLogTO"></param>
        '''' <param name="WriteSystemLog">If True Write to System Log, False Do not Write.</param>
        '''' <remarks>
        '''' Created by SG 18/02/2013 - for Update Version Error Log
        '''' </remarks>
        Public Sub InsertUpdateVersionLog(ByVal MyApplicationLogTO As ApplicationLogTO)
            'Try

            LockObject = New Object
            SyncLock LockObject         '1. Start Log SynLock

                'Initialize xml log file
                If Not System.IO.File.Exists(MyClass.myUpdateLogPath) Then
                    MyClass.InitUpdateVersionLogFile()
                End If


                Dim MyLogXmlDocument As New XmlDocument ' create a XmlDocument object to load the Log Xml file.

                '3 - Load and write Log File
                MyLogXmlDocument.Load(MyClass.myUpdateLogPath) 'load the document.



                '3.2 - Write Original Log item

                'Create a XmlElements to enter the information at the Xml Log File 
                Dim MyXmlElement As XmlElement = MyLogXmlDocument.CreateElement("ApplicationEventLogDetail") 'create node
                MyXmlElement.InnerXml = "<LogDateTime></LogDateTime><Message></Message><Module></Module><LogType></LogType>" 'create structure

                MyXmlElement("LogDateTime").InnerText = MyApplicationLogTO.LogDate.ToString("yyyy/MM/dd HH:mm:ss:fff") 'AG 29/06/2012 - add milliseconds ':fff'
                MyXmlElement("Message").InnerText = MyApplicationLogTO.LogMessage
                MyXmlElement("Module").InnerText = MyApplicationLogTO.LogModule
                MyXmlElement("LogType").InnerText = MyApplicationLogTO.LogType.ToString()

                MyLogXmlDocument.DocumentElement.AppendChild(MyXmlElement) 'add the new element to the XML document

                '4 - Save Log file
                MyLogXmlDocument.Save(MyClass.myUpdateLogPath) ' Save the document



            End SyncLock
        End Sub

        '''' <summary>
        '''' Initialize the LogWiter control writing in the Log file Xml.
        '''' </summary>
        '''' <remarks>Created by SG 18/02/2013</remarks>
        Private Sub InitUpdateVersionLogFile()

            If Not Directory.Exists(Application.StartupPath & My.Settings.PreviousFolder) Then
                Directory.CreateDirectory(Application.StartupPath & My.Settings.PreviousFolder)
            End If

            Dim myLogFile As System.IO.FileStream = New FileStream(MyClass.myUpdateLogPath, FileMode.OpenOrCreate)

            Dim LogWriter As XmlTextWriter = New XmlTextWriter(myLogFile, Text.Encoding.UTF8) 'create the XML Document.

            If LogWriter IsNot Nothing Then
                With LogWriter
                    .Formatting = Formatting.Indented
                    .WriteStartDocument()
                    .WriteStartElement("ApplicationEventLog")
                    .WriteComment("BAx00 Database Update Version Log File")
                    .WriteEndElement()
                    .WriteEndDocument()
                    .Flush()
                End With
            End If

            myLogFile.Close()

        End Sub

        
#End Region


#Region "OLD IMPLEMENTATION: LOG WAS SAVED DIRECTLY IN AN XML FILE"
        'Private Shared LockObject As Object 'RH 26/03/2012
        'Private XmlPath As String ' indicate the XML path.
        'Private Const MAX_FILE_LOG_SIZE As Long = 1024 * 2048      '(20Mb) DL 26/01/2012

        'Public Sub New(Optional ByVal pReset As Boolean = False)
        '    'RH 26/03/2012
        '    If LockObject Is Nothing Then
        '        LockObject = New Object()
        '    End If

        '    'XmlPath = Application.StartupPath & ConfigurationManager.AppSettings("XmlLogFilePath")
        '    'TR 25/01/2011 -Replace by corresponding value on global base.
        '    XmlPath = Application.StartupPath & GlobalBase.XmlLogFilePath

        '    If Not Directory.Exists(XmlPath) Then
        '        Directory.CreateDirectory(XmlPath)
        '        'include the Log file into the path 
        '        XmlPath &= GlobalBase.XmlLogFile ' "AX00Log.xml"
        '        XmlPath = XmlPath.Replace(".xml", "_" & Now.ToString("yyyyMMdd") & ".xml")
        '        'Create the new Xml File, with the encoding format UTF8
        '        'LogWriter = New XmlTextWriter(XmlPath, Text.Encoding.UTF8)
        '        'LogWriter.Formatting = Formatting.Indented
        '        'Initialize the Xml File
        '        InitializeXmlFile()
        '    Else
        '        'include the Log file into the path 
        '        'XmlPath = XmlPath & GlobalBase.XmlLogFile '"AX00Log.xml"
        '        XmlPath &= FindLastDate(XmlPath) 'XmlPath.Replace(".xml", "_" & Now.ToString("yyyyMMdd") & ".xml")

        '        If pReset Then
        '            File.Delete(XmlPath)
        '            Application.DoEvents()
        '            'Create the new Xml File, with the encoding format UTF8
        '            'LogWriter = New XmlTextWriter(XmlPath, Text.Encoding.UTF8)
        '            'LogWriter.Formatting = Formatting.Indented
        '            'Initialize the Xml File
        '            InitializeXmlFile()
        '        End If
        '    End If
        'End Sub

        '''' <summary>
        '''' Insert a Log Activity into the Log file, and System Log if WriteSystemLog is true
        '''' </summary>
        '''' <param name="MyApplicationLogTO"></param>
        '''' <param name="WriteSystemLog">If True Write to System Log, False Do not Write.</param>
        '''' <remarks>
        '''' Created by DL: 25/01/2012
        '''' </remarks>
        'Public Sub InsertLog(ByVal MyApplicationLogTO As ApplicationLogTO, ByVal WriteSystemLog As Boolean)
        '    'Try
        '    SyncLock LockObject         '1. Start Log SynLock
        '        Dim myCurrentDate As String = "Ax00Log_" & Now.ToString("yyyyMMdd") & ".xml"
        '        Dim myLastDate As String = FindLastDate(Application.StartupPath & GlobalBase.XmlLogFilePath)
        '        Dim isNewLog As Boolean = False

        '        If myLastDate Is String.Empty Then
        '            'If not exist Log
        '            isNewLog = True
        '            XmlPath = Application.StartupPath & GlobalBase.XmlLogFilePath & myCurrentDate
        '        Else
        '            'If exist Log file
        '            If UCase(myLastDate.Trim.ToString.Substring(0, 16)) = UCase(myCurrentDate.Trim.ToString.Substring(0, 16)) Then
        '                'Not is a new day
        '                isNewLog = False
        '            Else
        '                'Is a new day
        '                If Not GlobalConstants.AnalyzerIsRunningFlag Then
        '                    'Not WS Running
        '                    isNewLog = True
        '                    XmlPath = Application.StartupPath & GlobalBase.XmlLogFilePath & myCurrentDate
        '                Else
        '                    'WS Running
        '                    isNewLog = False
        '                End If
        '            End If
        '        End If

        '        Dim myLogCopyPath As String = XmlPath.Replace(".xml", "Temp.xml")
        '        Dim myFileNotFoundAppLogTO As ApplicationLogTO
        '        Dim myCorruptedFileAppLogTO As ApplicationLogTO
        '        Dim LogFileBeingUsed As Boolean

        '        MaintenanceLog(False)

        '        If isNewLog AndAlso Not File.Exists(XmlPath) Then
        '            '1.0 - Log File NOT exists
        '            myFileNotFoundAppLogTO = New ApplicationLogTO
        '            myFileNotFoundAppLogTO.LogDate = DateTime.Now
        '            myFileNotFoundAppLogTO.LogModule = "ApplicationLogManager"
        '            myFileNotFoundAppLogTO.LogType = System.Diagnostics.EventLogEntryType.Error

        '            If File.Exists(myLogCopyPath) Then
        '                '1.0.0 - Copy of Log File exits: restore from copy
        '                'Restore log file from the previous copy
        '                File.Copy(myLogCopyPath, XmlPath)

        '                myFileNotFoundAppLogTO = New ApplicationLogTO
        '                myFileNotFoundAppLogTO.LogMessage = "The previous Xml Log file was not found. A new one has been just created from a previous backup"
        '            Else
        '                '1.0.1 - Copy of Log File NOT exits: create new Log empty file
        '                'Create new one
        '                InitializeXmlFile()
        '                myFileNotFoundAppLogTO.LogMessage = "The previous Xml Log file was not found. A new one has been just created"
        '            End If

        '        Else
        '            '1.1 - Log File exists
        '            Dim MyTestLogXmlDocument As New XmlDocument

        '            Try
        '                If File.Exists(myLogCopyPath) Then
        '                    'If the copy is higher, then copy from it
        '                    If GetFileSize(XmlPath) < GetFileSize(myLogCopyPath) Then
        '                        File.Delete(XmlPath)
        '                        File.Copy(myLogCopyPath, XmlPath)
        '                    End If
        '                End If

        '                '1.1.0 - check if Log file is corrupted
        '                'check if the file can be opened
        '                MyTestLogXmlDocument.Load(XmlPath)

        '                '1.1.1 - Log file is OK
        '                '1.1.2 - delete previous copy if exists
        '                If File.Exists(myLogCopyPath) Then File.Delete(myLogCopyPath) 'if all ok delete the previous copy

        '                '1.1.3 - make a new copy
        '                File.Copy(XmlPath, myLogCopyPath)     'create a new copy
        '                If IO.File.Exists(myLogCopyPath) Then IO.File.SetAttributes(myLogCopyPath, IO.FileAttributes.Hidden)

        '            Catch ex As Exception
        '                If TypeOf (ex) Is IOException Then
        '                    'the Log file is being used by another process
        '                    '1.1.4.1 - Log file being used by another process

        '                    LogFileBeingUsed = True

        '                    'write to the copy
        '                    If Not File.Exists(myLogCopyPath) Then
        '                        File.Copy(XmlPath, myLogCopyPath)
        '                        If IO.File.Exists(myLogCopyPath) Then IO.File.SetAttributes(myLogCopyPath, IO.FileAttributes.Hidden)
        '                    End If

        '                    XmlPath = myLogCopyPath

        '                Else
        '                    '1.1.4 - Log file is corrupted. Add it to Zip
        '                    'add the corrupted file to zip
        '                    MaintenanceLog(True)

        '                    '1.1.5 - Delete corrupted Log file
        '                    File.Delete(XmlPath)  'Delete the corrupted file

        '                    '1.1.6 - Build Log file is corrupted log message
        '                    'Report that the file is corrupted
        '                    myCorruptedFileAppLogTO = New ApplicationLogTO
        '                    myCorruptedFileAppLogTO.LogDate = DateTime.Now
        '                    myCorruptedFileAppLogTO.LogModule = "ApplicationLogManager"

        '                    If File.Exists(myLogCopyPath) Then
        '                        '1.1.6.0 - If copy exists, restore from copy
        '                        'Restore the data from the previous copy
        '                        File.Copy(myLogCopyPath, XmlPath)
        '                        myCorruptedFileAppLogTO.LogMessage = "The previous Xml Log file was wrong. The data has been restored from the previous copy"
        '                        myCorruptedFileAppLogTO.LogType = System.Diagnostics.EventLogEntryType.Error
        '                    Else
        '                        '1.1.6.1 - If copy not exists, make a new Log file
        '                        'Create the new Xml File, with the encoding format UTF8
        '                        myCorruptedFileAppLogTO.LogMessage = "The previous Xml Log file was wrong and it was no possible to recover it. A new one has been just created"
        '                        myCorruptedFileAppLogTO.LogType = System.Diagnostics.EventLogEntryType.Error

        '                        InitializeXmlFile()  'Initialize the Xml File

        '                        '1.1.6.2 - make a new copy
        '                        File.Copy(XmlPath, myLogCopyPath)   'Create a new copy
        '                        If IO.File.Exists(myLogCopyPath) Then IO.File.SetAttributes(myLogCopyPath, IO.FileAttributes.Hidden)
        '                    End If
        '                End If
        '            Finally
        '                MyTestLogXmlDocument = Nothing
        '            End Try

        '        End If

        '        Dim MyLogXmlDocument As New XmlDocument ' create a XmlDocument object to load the Log Xml file.

        '        If Not isNewLog AndAlso File.Exists(XmlPath) Then
        '            '2 - Check Log File size
        '            'If file size is greater size defined in database '2MB rename old log and create new log
        '            If GlobalConstants.MaxFileLogSize > -1 AndAlso GetFileSize(XmlPath) > GlobalConstants.MaxFileLogSize Then
        '                '2.1 - Log File size > 2Mb
        '                MaintenanceLog(False)

        '                'Rename file, add number at the end
        '                Dim mypath As String = Application.StartupPath & GlobalBase.XmlLogFilePath
        '                Dim newLog As String

        '                If XmlPath.Substring(Len(mypath) + 16, 1) = "." Then
        '                    newLog = XmlPath.Substring(0, Len(mypath) + 16) & "_01.xml"
        '                    Rename(XmlPath, newLog)
        '                    XmlPath = XmlPath.Substring(0, Len(mypath) + 16) & "_02.xml"

        '                ElseIf XmlPath.Substring(Len(mypath) + 16, 1) = "_" Then
        '                    newLog = XmlPath.Substring(0, _
        '                                               Len(mypath) + 16) & "_" & _
        '                                               Integer.Parse(CStr(CInt(XmlPath.Substring(Len(mypath) + 17, 2)) + 1)).ToString("00") & ".xml"
        '                    XmlPath = newLog
        '                End If

        '                InitializeXmlFile() 'Initialize the Xml File

        '                '2.1.1 - if copy exists, delete it
        '                'Create a new copy
        '                If File.Exists(myLogCopyPath) Then
        '                    File.Delete(myLogCopyPath)
        '                End If

        '                '2.1.1 - create a new copy
        '                File.Copy(XmlPath, myLogCopyPath)
        '                If IO.File.Exists(myLogCopyPath) Then IO.File.SetAttributes(myLogCopyPath, IO.FileAttributes.Hidden)
        '            End If
        '        End If

        '        '3 - Load and write Log File
        '        MyLogXmlDocument.Load(XmlPath) 'load the document.

        '        If myFileNotFoundAppLogTO IsNot Nothing Then
        '            '3.0 - Write File Not Found")

        '            Dim MyFileNotFoundXmlElement As XmlElement = MyLogXmlDocument.CreateElement("ApplicationEventLogDetail") 'create node
        '            MyFileNotFoundXmlElement.InnerXml = "<LogDateTime></LogDateTime><Message></Message><Module></Module><LogType></LogType>" 'create structure
        '            MyFileNotFoundXmlElement("LogDateTime").InnerText = myFileNotFoundAppLogTO.LogDate.ToString("yyyy/MM/dd HH:mm:ss:fff") 'AG 29/06/2012 - add milliseconds ':fff'
        '            MyFileNotFoundXmlElement("Message").InnerText = myFileNotFoundAppLogTO.LogMessage
        '            MyFileNotFoundXmlElement("Module").InnerText = myFileNotFoundAppLogTO.LogModule
        '            MyFileNotFoundXmlElement("LogType").InnerText = myFileNotFoundAppLogTO.LogType.ToString()

        '            MyLogXmlDocument.DocumentElement.AppendChild(MyFileNotFoundXmlElement) 'add the new element to the XML document
        '        End If

        '        If myCorruptedFileAppLogTO IsNot Nothing Then
        '            'Debug.Print("3.1 - Write File is corrupted")

        '            Dim MyCorruptedFileXmlElement As XmlElement = MyLogXmlDocument.CreateElement("ApplicationEventLogDetail") 'create node
        '            MyCorruptedFileXmlElement.InnerXml = "<LogDateTime></LogDateTime><Message></Message><Module></Module><LogType></LogType>" 'create structure

        '            MyCorruptedFileXmlElement("LogDateTime").InnerText = myCorruptedFileAppLogTO.LogDate.ToString("yyyy/MM/dd HH:mm:ss:fff") 'AG 29/06/2012 - add milliseconds ':fff'
        '            MyCorruptedFileXmlElement("Message").InnerText = myCorruptedFileAppLogTO.LogMessage
        '            MyCorruptedFileXmlElement("Module").InnerText = myCorruptedFileAppLogTO.LogModule
        '            MyCorruptedFileXmlElement("LogType").InnerText = myCorruptedFileAppLogTO.LogType.ToString()

        '            MyLogXmlDocument.DocumentElement.AppendChild(MyCorruptedFileXmlElement) 'add the new element to the XML document
        '        End If

        '        '3.2 - Write Original Log item

        '        'Create a XmlElemente to enter the information at the Xml Log File 
        '        Dim MyXmlElement As XmlElement = MyLogXmlDocument.CreateElement("ApplicationEventLogDetail") 'create node
        '        MyXmlElement.InnerXml = "<LogDateTime></LogDateTime><Message></Message><Module></Module><LogType></LogType>" 'create structure

        '        MyXmlElement("LogDateTime").InnerText = MyApplicationLogTO.LogDate.ToString("yyyy/MM/dd HH:mm:ss:fff") 'AG 29/06/2012 - add milliseconds ':fff'
        '        MyXmlElement("Message").InnerText = MyApplicationLogTO.LogMessage
        '        MyXmlElement("Module").InnerText = MyApplicationLogTO.LogModule
        '        MyXmlElement("LogType").InnerText = MyApplicationLogTO.LogType.ToString()

        '        MyLogXmlDocument.DocumentElement.AppendChild(MyXmlElement) 'add the new element to the XML document

        '        '4 - Save Log file
        '        MyLogXmlDocument.Save(XmlPath) ' Save the document

        '        If (WriteSystemLog OrElse myCorruptedFileAppLogTO IsNot Nothing) Then
        '            Try
        '                'Debug.Print("5 - Report to system Log in case of needed")
        '                If Not IsSystemLogFull Then
        '                    If WriteSystemLog Then
        '                        System.Diagnostics.EventLog.WriteEntry("BiosystemsLog", MyApplicationLogTO.LogMessage, MyApplicationLogTO.LogType)
        '                    End If
        '                    If myCorruptedFileAppLogTO IsNot Nothing Then
        '                        System.Diagnostics.EventLog.WriteEntry("BiosystemsLog", myCorruptedFileAppLogTO.LogMessage, myCorruptedFileAppLogTO.LogType)
        '                    End If

        '                End If

        '            Catch ex As Exception
        '                'Debug.Print("6 - Error reporting to system log")
        '                'SGM 17/11/2011
        '                If TypeOf ex Is System.ComponentModel.Win32Exception Then
        '                    Dim myEx As System.ComponentModel.Win32Exception = CType(ex, System.ComponentModel.Win32Exception)
        '                    If myEx IsNot Nothing Then

        '                        If myEx.NativeErrorCode = 1502 Then
        '                            IsSystemLogFull = True
        '                            MyLogXmlDocument.Load(XmlPath) 'load the document.

        '                            Dim mySystemLogFullAppLogTO As ApplicationLogTO = New ApplicationLogTO
        '                            mySystemLogFullAppLogTO.LogDate = DateTime.Now
        '                            mySystemLogFullAppLogTO.LogModule = "ApplicationLogManager"
        '                            mySystemLogFullAppLogTO.LogMessage = myEx.Message
        '                            mySystemLogFullAppLogTO.LogType = System.Diagnostics.EventLogEntryType.Error

        '                            Dim mySystemLogFullXmlElement As XmlElement = MyLogXmlDocument.CreateElement("ApplicationEventLogDetail") 'create node
        '                            mySystemLogFullXmlElement.InnerXml = "<LogDateTime></LogDateTime><Message></Message><Module></Module><LogType></LogType>" 'create structure
        '                            mySystemLogFullXmlElement("LogDateTime").InnerText = mySystemLogFullAppLogTO.LogDate.ToString("yyyy/MM/dd HH:mm:ss:fff") 'AG 29/06/2012 - add milliseconds ':fff'
        '                            mySystemLogFullXmlElement("Message").InnerText = mySystemLogFullAppLogTO.LogMessage
        '                            mySystemLogFullXmlElement("Module").InnerText = mySystemLogFullAppLogTO.LogModule
        '                            mySystemLogFullXmlElement("LogType").InnerText = mySystemLogFullAppLogTO.LogType.ToString()

        '                            MyLogXmlDocument.DocumentElement.AppendChild(mySystemLogFullXmlElement) 'add the new element to the XML document

        '                            MyLogXmlDocument.Save(XmlPath) ' save the document

        '                        Else
        '                            'Throw ex
        '                            Throw 'RH 17/01/2012
        '                            'Do prefer using an empty throw (just "Throw") when catching and re-throwing an exception.
        '                            'This is the best way to preserve the exception call stack.
        '                            'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
        '                            'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
        '                        End If
        '                    Else
        '                        'Throw ex
        '                        Throw 'RH 17/01/2012
        '                    End If
        '                Else
        '                    'QUITAR para ver otro tipo de excepciones
        '                    Dim myExStr As New System.Text.StringBuilder
        '                    myExStr.Append("Message: " & ex.Message & vbCrLf)
        '                    myExStr.Append("Source: " & ex.Source & vbCrLf)
        '                    myExStr.Append("StackTrace: " & ex.StackTrace & vbCrLf)
        '                    If ex.InnerException IsNot Nothing Then
        '                        myExStr.Append("Inner Exception:" & vbCrLf)
        '                        myExStr.Append("Message: " & ex.InnerException.Message & vbCrLf)
        '                        myExStr.Append("Source: " & ex.InnerException.Source & vbCrLf)
        '                        myExStr.Append("StackTrace: " & ex.InnerException.StackTrace & vbCrLf)
        '                    End If

        '                    MessageBox.Show(myExStr.ToString, "Debugging Log")
        '                    'end QUITAR
        '                End If
        '                'SGM 17/11/2011

        '            End Try

        '        End If

        '        If Not LogFileBeingUsed Then
        '            If File.Exists(myLogCopyPath) Then File.Delete(myLogCopyPath) 'SGM 07/12/2011
        '        End If

        '    End SyncLock                '7. End Log SynLock

        '    'Catch ex As Exception
        '    '    MessageBox.Show(ex.Message.ToString, "ApplicationLog.InsertLog")
        '    'End Try
        'End Sub

        '''' <summary>
        '''' 'Find last date log
        '''' </summary>
        '''' <param name="pPath"></param>
        '''' <remarks>
        '''' Created by DL: 25/01/2012
        '''' </remarks>
        'Private Function FindLastDate(ByVal pPath As String) As String
        '    'Make a reference to a directory
        '    Dim di As New IO.DirectoryInfo(pPath)
        '    Dim diar1 As IO.FileInfo() = di.GetFiles("Ax00Log_*.xml")
        '    Dim dra As IO.FileInfo
        '    Dim myLastDate As String = ""

        '    'list the names of all xml files in the specified directory
        '    Dim listXmlFiles As New List(Of String)
        '    For Each dra In diar1
        '        'ListBox1.Items.Add(dra)
        '        'DL-TR -03/02/2012 -Validate the file name do not has a temp value on the string name.
        '        If Not dra.ToString.Contains("Temp") And Not dra.ToString.Contains("Corrupted") Then
        '            listXmlFiles.Add(dra.ToString)
        '        End If

        '    Next dra

        '    If listXmlFiles.Count > 0 Then
        '        Dim query As String = (From b In listXmlFiles _
        '                               Select b).Max.ToString

        '        myLastDate = query
        '    End If
        '    Return myLastDate
        'End Function

        'Private Function GetFileSize(ByVal pFile As String) As Long
        '    Dim myFileDetail As IO.FileInfo = My.Computer.FileSystem.GetFileInfo(pFile)
        '    Dim mySize As Long = myFileDetail.Length() 'Know file size
        '    myFileDetail = Nothing

        '    Return (mySize)
        'End Function

        '''' <summary>
        '''' Initialise the LogWiter control writing in the Log file Xml.
        '''' </summary>
        '''' <remarks></remarks>
        'Private Sub InitializeXmlFile()
        '    'Try
        '    Dim LogWriter As XmlTextWriter = New XmlTextWriter(XmlPath, Text.Encoding.UTF8) 'create the XML Document.

        '    If LogWriter IsNot Nothing Then
        '        With LogWriter
        '            .Formatting = Formatting.Indented
        '            .WriteStartDocument()
        '            .WriteStartElement("ApplicationEventLog")
        '            .WriteComment("Ax00 Log File")
        '            .WriteEndElement()
        '            .WriteEndDocument()
        '            .Close()
        '        End With
        '    End If

        '    'RH 16/11/2010 Remove Try/Catch because (read below):
        '    'Catch ex As Exception
        '    '    Throw ex
        '    'End Try
        'End Sub

        '''' <summary>
        '''' Manage exception if XML Log File is wrong or corrupted
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>
        '''' Created by:  SG 29/07/2011
        '''' </remarks>
        'Public Function BackupWrongLogXmlFile() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Dim myUtil As New Utilities
        '    Dim myReadedXmlText As String = ""
        '    Dim myOKText As String = ""
        '    Dim myWrongText As String = ""
        '    Dim myFinalText As String
        '    Dim myMessage As String = ""

        '    Const myAppEventLogStartTag As String = "<ApplicationEventLog>"
        '    Const myAppEventLogDetailStartTag As String = "<ApplicationEventLogDetail>"
        '    Const myLogDateTimeStartTag As String = "<LogDateTime>"
        '    Const myMessageStartTag As String = "<Message>"
        '    Const myModuleStartTag As String = "<Module>"
        '    Const myLogTypeStartTag As String = "<LogType>"

        '    Const myAppEventLogEndTag As String = "</ApplicationEventLog>"
        '    Const myAppEventLogDetailEndTag As String = "</ApplicationEventLogDetail>"
        '    Const myLogDateTimeEndTag As String = "</LogDateTime>"
        '    Const myMessageEndTag As String = "</Message>"
        '    Const myModuleEndTag As String = "</Module>"
        '    Const myLogTypeEndTag As String = "</LogType>"

        '    Const myEndOfFileErrorMessage As String = "End Of File not found."
        '    Const myFileCannotBeReadErrorMessage As String = "The File is wrong or corrupt and cannot be read."
        '    Const myReadErrorReportMessage As String = "This Xml Log File cannot be read/written again. A new Xml Log File is created."

        '    Try
        '        myGlobal = myUtil.ReadTextFile(XmlPath, FileAccess.Read)
        '        If (Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing) Then
        '            myReadedXmlText = CStr(myGlobal.SetDatos)

        '            myOKText = myReadedXmlText

        '            Dim rest As String
        '            If (myReadedXmlText.Length > 0) Then
        '                'In case that the log file has bytes but all nulls - Incidence 05/12/2011
        '                For Each C As Char In myReadedXmlText
        '                    If (C = Nothing) Then Throw New Exception("XML_LOG_FILE_IS_EMPTY")
        '                Next

        '                'If the Tag <ApplicationEventLog> means that End file is missing
        '                rest = myReadedXmlText.Substring(myReadedXmlText.IndexOf(myAppEventLogStartTag)).Trim
        '                If (Not rest.Contains(myAppEventLogEndTag)) Then
        '                    'END OF FILE ERROR
        '                    Dim LastDetailIndex As Integer = myReadedXmlText.LastIndexOf(myAppEventLogDetailStartTag)

        '                    myOKText = myReadedXmlText.Substring(0, LastDetailIndex).Trim
        '                    myWrongText = myReadedXmlText.Substring(LastDetailIndex).Trim

        '                    myFinalText = myOKText & vbCrLf & "***********XML LOG FILE END OF FILE ERROR TAG****************"
        '                    myFinalText = myFinalText & vbCrLf & myWrongText & vbCrLf & "***************************************************"

        '                    myMessage = myEndOfFileErrorMessage & " "
        '                Else
        '                    myFinalText = myReadedXmlText
        '                    myFinalText = myFinalText & vbCrLf & "***********XML LOG FILE ACCESS ERROR HAPPENED****************"

        '                    myMessage = myFileCannotBeReadErrorMessage & " "
        '                End If
        '            Else
        '                myGlobal.HasError = True
        '                Throw New Exception("XML_LOG_FILE_IS_EMPTY")
        '            End If

        '            If (myFinalText.Length > 0) Then
        '                myFinalText = myFinalText & vbCrLf
        '                myMessage = myMessage & myReadErrorReportMessage

        '                'Report to System Log
        '                System.Diagnostics.EventLog.WriteEntry("BiosystemsLog", myMessage, System.Diagnostics.EventLogEntryType.Error)

        '                'Create Access Exception Log
        '                myFinalText &= myAppEventLogDetailStartTag
        '                myFinalText &= myLogDateTimeStartTag
        '                myFinalText &= System.DateTime.Now.ToString
        '                myFinalText &= myLogDateTimeEndTag

        '                myFinalText &= myMessageStartTag
        '                myFinalText &= myMessage
        '                myFinalText &= myMessageEndTag

        '                myFinalText &= myModuleStartTag
        '                myFinalText &= "ApplicationLogManager"
        '                myFinalText &= myModuleEndTag

        '                myFinalText &= myLogTypeStartTag
        '                myFinalText &= System.Diagnostics.EventLogEntryType.Error.ToString
        '                myFinalText &= myLogTypeEndTag

        '                myFinalText &= myAppEventLogDetailEndTag
        '                myFinalText &= myAppEventLogEndTag

        '                If (myFinalText.Length > 0) Then myGlobal = myUtil.OverWriteTextFile(XmlPath, myFinalText)
        '            End If
        '        End If

        '        'The next log will be registered in a new file
        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '    Finally
        '        'Make the BACKUP
        '        myGlobal = MaintenanceLog()
        '    End Try
        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' Maintenance of the file log activity in order to don't grow much
        '''' </summary>
        '''' <remarks>
        '''' Created by:  DL 25/01/2012
        '''' </remarks>
        'Public Function MaintenanceLog(Optional ByVal pIsCorrupted As Boolean = False) As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        Dim myLogPath As String = Application.StartupPath & GlobalBase.XmlLogFilePath

        '        'Files to compress
        '        Dim listXmlFilesToCompress As New List(Of String)

        '        If (pIsCorrupted) Then
        '            listXmlFilesToCompress.Add(XmlPath)
        '        Else
        '            'Compress only files olds (more 1 day)
        '            Dim myDate As System.DateTime = Now
        '            myDate = myDate.AddDays(-1)

        '            'Make a Reference to a Folder
        '            Dim di As New IO.DirectoryInfo(myLogPath)
        '            Dim diar1 As IO.FileInfo() = di.GetFiles("Ax00Log_*.xml")
        '            Dim dra As IO.FileInfo
        '            Dim tempDate As String

        '            For Each dra In diar1
        '                tempDate = dra.ToString.Substring(8, 8)
        '                If (tempDate < myDate.ToString("yyyyMMdd")) Then listXmlFilesToCompress.Add(dra.ToString)
        '            Next dra
        '        End If

        '        If (listXmlFilesToCompress.Count > 0) Then
        '            'Create temp folder to move files to compress
        '            Dim myUtils As New Utilities
        '            If (File.Exists(myLogPath & TEMP_FOLDER)) Then myGlobal = myUtils.RemoveFolder(myLogPath & TEMP_FOLDER)

        '            myGlobal = myUtils.CreateFolder(myLogPath & TEMP_FOLDER)
        '            If (Not myGlobal.HasError) Then
        '                If (File.Exists(myLogPath & PREVIOUS_LOG_ZIP)) Then
        '                    myGlobal = myUtils.ExtractFromZip(myLogPath & PREVIOUS_LOG_ZIP, myLogPath & TEMP_FOLDER & "\")
        '                    If (Not myGlobal.HasError) Then Kill(myLogPath & PREVIOUS_LOG_ZIP)
        '                End If

        '                For i As Integer = 0 To listXmlFilesToCompress.Count - 1
        '                    myGlobal = myUtils.MoveFiles(myLogPath, myLogPath & TEMP_FOLDER & "\", listXmlFilesToCompress(i))

        '                    If (Not myGlobal.HasError) Then
        '                        If (pIsCorrupted) Then
        '                            Rename(myLogPath & TEMP_FOLDER & "\" & listXmlFilesToCompress(0), _
        '                                   myLogPath & TEMP_FOLDER & "\" & "Ax00LoginCorrupted_" & Now.ToString("yyyyMMdd hhmmss") & ".xml")
        '                        End If
        '                    End If
        '                Next i

        '                myGlobal = myUtils.CompressToZip(myLogPath & TEMP_FOLDER & "\", myLogPath & PREVIOUS_LOG_ZIP)
        '                If (Not myGlobal.HasError) Then myGlobal = myUtils.RemoveFolder(myLogPath & TEMP_FOLDER)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '    End Try
        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' Insert a Log Activity into the Log file
        '''' </summary>
        '''' <param name="pApplicationLogTO"></param>
        '''' <remarks>
        '''' Created by:  SA 29/06/2012
        '''' </remarks>
        'Public Sub InsertInfoInLOG(ByVal pApplicationLogTO As ApplicationLogTO)
        '    'XmlPath = Application.StartupPath & GlobalBase.XmlLogFilePath & "LOG_PRUEBA.XML"

        '    SyncLock LockObject
        '        'Load the Log File and write in it
        '        Dim myLogXMLDocument As New XmlDocument
        '        myLogXMLDocument.Load(XmlPath)

        '        'Create an XmlElement to enter the information in the XML
        '        Dim MyXmlElement As XmlElement = myLogXMLDocument.CreateElement("ApplicationEventLogDetail")
        '        MyXmlElement.InnerXml = "<LogDateTime></LogDateTime><Message></Message><Module></Module><LogType></LogType>"
        '        MyXmlElement("LogDateTime").InnerText = pApplicationLogTO.LogDate.ToString("yyyy/MM/dd HH:mm:ss:fff")
        '        MyXmlElement("Message").InnerText = pApplicationLogTO.LogMessage
        '        MyXmlElement("Module").InnerText = pApplicationLogTO.LogModule
        '        MyXmlElement("LogType").InnerText = pApplicationLogTO.LogType.ToString()
        '        myLogXMLDocument.DocumentElement.AppendChild(MyXmlElement)

        '        'Save the XML File
        '        myLogXMLDocument.Save(XmlPath)
        '    End SyncLock
        'End Sub

        '''' <summary>
        '''' When the application is closed, read all XML files in the LOG Folder and:
        '''' ** Move to a ZIP File all XML Files created in previous days
        '''' ** If the size of the XML File for the day is bigger than the allowed one, then the file is renamed adding it 
        ''''    a number as suffix, and a new  one is created with the same name and the next number
        '''' </summary>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 29/06/2012
        '''' </remarks>
        'Public Function ManageLOGWhenCloseApplication() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        Dim myLogCopyPath As String = XmlPath.Replace(".xml", "Temp.xml")
        '        Dim myLogPath As String = Application.StartupPath & GlobalBase.XmlLogFilePath

        '        Dim lstXMLFiles As New List(Of String)
        '        Dim lstXMLFilesToCompress As New List(Of String)

        '        Dim myLogFile As IO.FileInfo
        '        Dim myLogDIR As New IO.DirectoryInfo(myLogPath)
        '        Dim myLogFilesList As IO.FileInfo() = myLogDIR.GetFiles("Ax00Log_*.xml")

        '        'All old Log Files will be compressed in a Zip file (old files = those with a date < than TODAY)
        '        Dim myDate As System.DateTime = Now

        '        For Each myLogFile In myLogFilesList
        '            If (myLogFile.ToString.Substring(8, 8) < myDate.ToString("yyyyMMdd")) Then
        '                'Add the Log file to the list of files to compress
        '                lstXMLFilesToCompress.Add(myLogFile.ToString)
        '            Else
        '                'The Log File for the current date exists; save the name to verify the file size
        '                lstXMLFiles.Add(myLogFile.ToString)
        '            End If
        '        Next

        '        'Process all Log Files to compress
        '        Dim myUtils As New Utilities
        '        If (lstXMLFilesToCompress.Count > 0) Then
        '            'Create the TEMP Folder to move all Log Files to compress
        '            If (File.Exists(myLogPath & TEMP_FOLDER)) Then myGlobal = myUtils.RemoveFolder(myLogPath & TEMP_FOLDER)
        '            If (Not myGlobal.HasError) Then myGlobal = myUtils.CreateFolder(myLogPath & TEMP_FOLDER)

        '            'If the ZIP File already exists, extract in the TEMP Folder all Log Files it contains and delete the ZIP File
        '            If (Not myGlobal.HasError) Then
        '                If (File.Exists(myLogPath & PREVIOUS_LOG_ZIP)) Then
        '                    myGlobal = myUtils.ExtractFromZip(myLogPath & PREVIOUS_LOG_ZIP, myLogPath & TEMP_FOLDER & "\")
        '                    If (Not myGlobal.HasError) Then Kill(myLogPath & PREVIOUS_LOG_ZIP)
        '                End If
        '            End If

        '            'Move all Log Files in the list of Log Files to compress to the TEMP Folder
        '            If (Not myGlobal.HasError) Then
        '                For i As Integer = 0 To lstXMLFilesToCompress.Count - 1
        '                    myGlobal = myUtils.MoveFiles(myLogPath, myLogPath & TEMP_FOLDER & "\", lstXMLFilesToCompress(i))
        '                Next i
        '            End If

        '            'Finally, create a new ZIP File, compress all Log Files in the TEMP Folder, and remove the TEMP Folder
        '            If (Not myGlobal.HasError) Then
        '                myGlobal = myUtils.CompressToZip(myLogPath & TEMP_FOLDER & "\", myLogPath & PREVIOUS_LOG_ZIP)
        '                If (Not myGlobal.HasError) Then myGlobal = myUtils.RemoveFolder(myLogPath & TEMP_FOLDER)
        '            End If
        '        End If

        '        'Process the rest of Log Files - Verify if the maximum allowed size has been reached
        '        If (Not myGlobal.HasError) Then
        '            If (lstXMLFiles.Count > 0) Then
        '                Dim myIndex As Integer = 0

        '                If (lstXMLFiles.Count > 1) Then
        '                    'If there is more than one XML for the current day, it means the files have a number as suffix
        '                    'and the size validation has to be done only for the one with the biggest number
        '                    Dim myFileNumber As Integer = 0

        '                    For i As Integer = 0 To lstXMLFiles.Count - 1
        '                        If (CInt(lstXMLFiles(i).Substring(Len(myLogPath) + 17, 2)) > myFileNumber) Then
        '                            myFileNumber = CInt(lstXMLFiles(i).Substring(Len(myLogPath) + 17, 2))
        '                            myIndex = i
        '                        End If
        '                    Next
        '                End If

        '                Dim xmlFile As String = lstXMLFiles(myIndex)
        '                If (GlobalConstants.MaxFileLogSize > -1 AndAlso GetFileSize(xmlFile) > GlobalConstants.MaxFileLogSize) Then
        '                    'The maximum size allowed for the Log file has been reached. The file is renamed adding to it a number as suffix,
        '                    'and a new file is created with the next number
        '                    Dim newLogName As String = String.Empty
        '                    Dim charAfterName As String = xmlFile.Substring(Len(myLogPath) + 16, 1)

        '                    If (charAfterName = ".") Then
        '                        'Rename the current Log File by adding a number as suffix
        '                        newLogName = xmlFile.Substring(0, Len(myLogPath) + 16) & "_01.xml"
        '                        Rename(xmlFile, newLogName)

        '                        'Set the name of the new Log File to create
        '                        XmlPath = xmlFile.Substring(0, Len(myLogPath) + 16) & "_02.xml"

        '                    ElseIf (charAfterName = "_") Then
        '                        'Search the next suffix with number to set the name of the new Log File to create
        '                        Dim nextSuffix As String = Integer.Parse(CStr(CInt(xmlFile.Substring(Len(myLogPath) + 17, 2)) + 1)).ToString("00") & ".xml"
        '                        newLogName = xmlFile.Substring(0, Len(myLogPath) + 16) & "_" & nextSuffix
        '                        XmlPath = newLogName
        '                    End If

        '                    'Create the new XML File
        '                    InitializeXmlFile()

        '                    'If there was a copy of the previous Log File, delete it
        '                    If (File.Exists(myLogCopyPath)) Then File.Delete(myLogCopyPath)

        '                    'Create a copy of the new Log File 
        '                    File.Copy(XmlPath, myLogCopyPath)
        '                    If (IO.File.Exists(myLogCopyPath)) Then IO.File.SetAttributes(myLogCopyPath, IO.FileAttributes.Hidden)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '    End Try
        '    Return myGlobal
        'End Function

        '''' <summary>
        '''' When the application is opened, verify if a new Log file has to be created. When it is not needed, then it is 
        '''' checked if the file can be opened and it is not being used
        '''' </summary>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  SA 29/06/2012
        '''' </remarks>
        'Public Function ManageLOGWhenOpenApplication(ByVal pWriteInSystemLOG As Boolean) As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        Dim myLogCopyPath As String = XmlPath.Replace(".xml", "Temp.xml")
        '        Dim myLogPath As String = Application.StartupPath & GlobalBase.XmlLogFilePath

        '        Dim lstXMLFiles As New List(Of String)

        '        Dim myLogFile As IO.FileInfo
        '        Dim myLogDIR As New IO.DirectoryInfo(myLogPath)
        '        Dim myLogFilesList As IO.FileInfo() = myLogDIR.GetFiles("Ax00Log_*.xml")

        '        'Move all XML Files to a string list
        '        For Each myLogFile In myLogFilesList
        '            If (Not myLogFile.ToString.Contains("Temp") AndAlso Not myLogFile.ToString.Contains("Corrupted")) Then
        '                lstXMLFiles.Add(myLogFile.ToString)
        '            End If
        '        Next

        '        'Get the XML File with the most recent date (the last created)
        '        Dim lastCreatedXML As String = String.Empty
        '        If (lstXMLFiles.Count > 0) Then
        '            lastCreatedXML = (From b As String In lstXMLFiles Select b).Max.ToString
        '        End If

        '        'Set the name of the next XML to create (if it is needed)
        '        Dim xmlToCreate As String = "Ax00Log_" & Now.ToString("yyyyMMdd") & ".xml"

        '        'Verify if it is needed and possible to create a new Log File
        '        Dim isNewLog As Boolean = False
        '        If (lastCreatedXML = String.Empty) Then
        '            'There is not a XML File in the LOG Folder, a new one has to be created
        '            isNewLog = True
        '            XmlPath = Application.StartupPath & GlobalBase.XmlLogFilePath & xmlToCreate

        '        ElseIf (lastCreatedXML.Trim.Substring(0, 16).ToUpper = xmlToCreate.Trim.Substring(0, 16).ToUpper) Then
        '            'There is an XML in the LOG Folder for the current date, it is not needed to create a new Log file
        '            isNewLog = False

        '        ElseIf (Not GlobalConstants.AnalyzerIsRunningFlag) Then
        '            'There is an XML in the LOG folder but it is not for the current date, a new one has to be created
        '            isNewLog = True
        '            XmlPath = Application.StartupPath & GlobalBase.XmlLogFilePath & xmlToCreate

        '        Else
        '            'If the Analyzer is RUNNING, the new LOG is not created, the old one is used
        '            isNewLog = False
        '        End If

        '        Dim myFileNotFoundAppLogTO As New ApplicationLogTO
        '        Dim myCorruptedFileAppLogTO As New ApplicationLogTO
        '        Dim LogFileBeingUsed As Boolean

        '        Dim copyLogExist As Boolean = File.Exists(myLogCopyPath)
        '        If (isNewLog AndAlso Not File.Exists(XmlPath)) Then
        '            'A new Log file has to be created
        '            If (Not copyLogExist) Then
        '                'If there is not a previous copy of the Log file, a new empty file is created
        '                InitializeXmlFile()

        '                myFileNotFoundAppLogTO = New ApplicationLogTO
        '                myFileNotFoundAppLogTO.LogDate = DateTime.Now
        '                myFileNotFoundAppLogTO.LogModule = "ApplicationLogManager"
        '                myFileNotFoundAppLogTO.LogType = System.Diagnostics.EventLogEntryType.Error
        '                myFileNotFoundAppLogTO.LogMessage = "The previous Xml Log file was not found. A new one has been just created"

        '            Else
        '                'If there is a previous copy of the Log file, the Log file is restored from the copy
        '                File.Copy(myLogCopyPath, XmlPath)

        '                myFileNotFoundAppLogTO = New ApplicationLogTO
        '                myFileNotFoundAppLogTO.LogMessage = "The previous Xml Log file was not found. A new one has been just created from a previous backup"
        '            End If
        '        Else
        '            'The Log file already exists in the LOG Folder
        '            Dim myTestLogXmlDocument As New XmlDocument

        '            Try
        '                'If there is a previous copy of the Log file, and its size is bigger than the size of the Log size,
        '                'the Log File is updated with the content of the previous copy
        '                If (copyLogExist) Then
        '                    If (GetFileSize(XmlPath) < GetFileSize(myLogCopyPath)) Then
        '                        File.Delete(XmlPath)
        '                        File.Copy(myLogCopyPath, XmlPath)
        '                    End If
        '                End If

        '                'Try to open the Log File to check is not in use and that it is not corrupted (if there is a problem, an exception is raised)  
        '                myTestLogXmlDocument.Load(XmlPath)

        '                'Log File is OK, delete the previous copy and create a new one
        '                If (copyLogExist) Then
        '                    File.Delete(myLogCopyPath)
        '                    copyLogExist = False
        '                End If
        '                File.Copy(XmlPath, myLogCopyPath)

        '                'If the new copy was created then set the file attributes
        '                If (IO.File.Exists(myLogCopyPath)) Then
        '                    IO.File.SetAttributes(myLogCopyPath, IO.FileAttributes.Hidden)
        '                    copyLogExist = True
        '                End If
        '            Catch ex As Exception
        '                If (TypeOf (ex) Is IOException) Then
        '                    'The Log file is being used by another process
        '                    LogFileBeingUsed = True

        '                    'Write the content of the Log File to the copy (if there is not a previous one)
        '                    If (Not copyLogExist) Then
        '                        File.Copy(XmlPath, myLogCopyPath)
        '                        If (IO.File.Exists(myLogCopyPath)) Then
        '                            IO.File.SetAttributes(myLogCopyPath, IO.FileAttributes.Hidden)
        '                            copyLogExist = True
        '                        End If
        '                    End If

        '                    'The copy file will be used as a Log
        '                    XmlPath = myLogCopyPath
        '                Else
        '                    'The existing Log file is corrupted. Add it to a Zip
        '                    Dim myUtils As New Utilities
        '                    Dim listXmlFilesToCompress As New List(Of String)
        '                    listXmlFilesToCompress.Add(XmlPath)

        '                    'Create the TEMP Folder to move all Log Files to compress
        '                    If (File.Exists(myLogPath & TEMP_FOLDER)) Then myGlobal = myUtils.RemoveFolder(myLogPath & TEMP_FOLDER)
        '                    If (Not myGlobal.HasError) Then myGlobal = myUtils.CreateFolder(myLogPath & TEMP_FOLDER)

        '                    'If the ZIP File already exists, extract in the TEMP Folder all Log Files it contains and delete the ZIP File
        '                    If (Not myGlobal.HasError) Then
        '                        If (File.Exists(myLogPath & PREVIOUS_LOG_ZIP)) Then
        '                            myGlobal = myUtils.ExtractFromZip(myLogPath & PREVIOUS_LOG_ZIP, myLogPath & TEMP_FOLDER & "\")
        '                            If (Not myGlobal.HasError) Then Kill(myLogPath & PREVIOUS_LOG_ZIP)
        '                        End If
        '                    End If

        '                    'Move the corrupted Log File to compress to the TEMP Folder
        '                    If (Not myGlobal.HasError) Then
        '                        myGlobal = myUtils.MoveFiles(myLogPath, myLogPath & TEMP_FOLDER & "\", listXmlFilesToCompress(0))
        '                    End If

        '                    'Finally, create a new ZIP File, compress all Log Files in the TEMP Folder, and remove the TEMP Folder
        '                    If (Not myGlobal.HasError) Then
        '                        myGlobal = myUtils.CompressToZip(myLogPath & TEMP_FOLDER & "\", myLogPath & PREVIOUS_LOG_ZIP)
        '                        If (Not myGlobal.HasError) Then myGlobal = myUtils.RemoveFolder(myLogPath & TEMP_FOLDER)
        '                    End If

        '                    'Delete the corrupted Log file
        '                    File.Delete(XmlPath)

        '                    'Report that the file is corrupted
        '                    myCorruptedFileAppLogTO = New ApplicationLogTO
        '                    myCorruptedFileAppLogTO.LogDate = DateTime.Now
        '                    myCorruptedFileAppLogTO.LogModule = "ApplicationLogManager"

        '                    If (Not copyLogExist) Then
        '                        myCorruptedFileAppLogTO.LogMessage = "The previous Xml Log file was wrong and it was no possible to recover it. A new one has been just created"
        '                        myCorruptedFileAppLogTO.LogType = System.Diagnostics.EventLogEntryType.Error

        '                        'Create a new empty XML file
        '                        InitializeXmlFile()

        '                        'Create a new copy 
        '                        File.Copy(XmlPath, myLogCopyPath)
        '                        If (IO.File.Exists(myLogCopyPath)) Then
        '                            IO.File.SetAttributes(myLogCopyPath, IO.FileAttributes.Hidden)
        '                            copyLogExist = True
        '                        End If
        '                    Else
        '                        myCorruptedFileAppLogTO.LogMessage = "The previous Xml Log file was wrong. The data has been restored from the previous copy"
        '                        myCorruptedFileAppLogTO.LogType = System.Diagnostics.EventLogEntryType.Error

        '                        'Restore the Log file with data from the previous copy
        '                        File.Copy(myLogCopyPath, XmlPath)
        '                    End If
        '                End If
        '            Finally
        '                myTestLogXmlDocument = Nothing
        '            End Try
        '        End If

        '        Dim myLogXMLDocument As New XmlDocument
        '        If (myFileNotFoundAppLogTO IsNot Nothing) Then
        '            'Write FILE NOT FOUND in the XML Document
        '            Dim MyFileNotFoundXmlElement As XmlElement = myLogXMLDocument.CreateElement("ApplicationEventLogDetail")
        '            MyFileNotFoundXmlElement.InnerXml = "<LogDateTime></LogDateTime><Message></Message><Module></Module><LogType></LogType>"
        '            MyFileNotFoundXmlElement("LogDateTime").InnerText = myFileNotFoundAppLogTO.LogDate.ToString("yyyy/MM/dd HH:mm:ss:fff")
        '            MyFileNotFoundXmlElement("Message").InnerText = myFileNotFoundAppLogTO.LogMessage
        '            MyFileNotFoundXmlElement("Module").InnerText = myFileNotFoundAppLogTO.LogModule
        '            MyFileNotFoundXmlElement("LogType").InnerText = myFileNotFoundAppLogTO.LogType.ToString()

        '            myLogXMLDocument.DocumentElement.AppendChild(MyFileNotFoundXmlElement)
        '        End If

        '        If (myCorruptedFileAppLogTO IsNot Nothing) Then
        '            'Write FILE IS CORRUPTED in the XML Document
        '            Dim MyCorruptedFileXmlElement As XmlElement = myLogXMLDocument.CreateElement("ApplicationEventLogDetail")
        '            MyCorruptedFileXmlElement.InnerXml = "<LogDateTime></LogDateTime><Message></Message><Module></Module><LogType></LogType>"
        '            MyCorruptedFileXmlElement("LogDateTime").InnerText = myCorruptedFileAppLogTO.LogDate.ToString("yyyy/MM/dd HH:mm:ss:fff")
        '            MyCorruptedFileXmlElement("Message").InnerText = myCorruptedFileAppLogTO.LogMessage
        '            MyCorruptedFileXmlElement("Module").InnerText = myCorruptedFileAppLogTO.LogModule
        '            MyCorruptedFileXmlElement("LogType").InnerText = myCorruptedFileAppLogTO.LogType.ToString()

        '            myLogXMLDocument.DocumentElement.AppendChild(MyCorruptedFileXmlElement)

        '            Try
        '                If (Not IsSystemLogFull) Then
        '                    'If the Log file was corrupted, then inform it in the EventLog
        '                    System.Diagnostics.EventLog.WriteEntry("BiosystemsLog", myCorruptedFileAppLogTO.LogMessage, myCorruptedFileAppLogTO.LogType)
        '                End If

        '            Catch ex As Exception
        '                If (TypeOf ex Is System.ComponentModel.Win32Exception) Then
        '                    Dim myEx As System.ComponentModel.Win32Exception = CType(ex, System.ComponentModel.Win32Exception)
        '                    If (myEx IsNot Nothing) Then
        '                        If (myEx.NativeErrorCode = 1502) Then
        '                            IsSystemLogFull = True
        '                            myLogXMLDocument.Load(XmlPath)

        '                            Dim mySystemLogFullAppLogTO As ApplicationLogTO = New ApplicationLogTO
        '                            mySystemLogFullAppLogTO.LogDate = DateTime.Now
        '                            mySystemLogFullAppLogTO.LogModule = "ApplicationLogManager"
        '                            mySystemLogFullAppLogTO.LogMessage = myEx.Message
        '                            mySystemLogFullAppLogTO.LogType = System.Diagnostics.EventLogEntryType.Error

        '                            Dim mySystemLogFullXmlElement As XmlElement = myLogXMLDocument.CreateElement("ApplicationEventLogDetail")
        '                            mySystemLogFullXmlElement.InnerXml = "<LogDateTime></LogDateTime><Message></Message><Module></Module><LogType></LogType>"
        '                            mySystemLogFullXmlElement("LogDateTime").InnerText = mySystemLogFullAppLogTO.LogDate.ToString("yyyy/MM/dd HH:mm:ss:fff")
        '                            mySystemLogFullXmlElement("Message").InnerText = mySystemLogFullAppLogTO.LogMessage
        '                            mySystemLogFullXmlElement("Module").InnerText = mySystemLogFullAppLogTO.LogModule
        '                            mySystemLogFullXmlElement("LogType").InnerText = mySystemLogFullAppLogTO.LogType.ToString()

        '                            myLogXMLDocument.DocumentElement.AppendChild(mySystemLogFullXmlElement)
        '                            myLogXMLDocument.Save(XmlPath)
        '                        Else
        '                            Throw
        '                        End If
        '                    Else
        '                        Throw
        '                    End If
        '                End If
        '            End Try
        '        End If

        '        If (Not LogFileBeingUsed) Then
        '            If (copyLogExist) Then File.Delete(myLogCopyPath)
        '        End If
        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobal.ErrorMessage = ex.Message
        '    End Try
        '    Return myGlobal
        'End Function
#End Region

    End Class
End Namespace