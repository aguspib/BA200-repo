Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text
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
        <Obsolete("This class is going to be converted to a Module. Do not create new instances of it! All its members are shared.")>
        Sub New()

        End Sub

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
        Public Shared Sub InsertLog(ByVal pApplicationLogTO As ApplicationLogTO)
            Dim myGlobalDataTO As New GlobalDataTO
            Try

                Dim myApplicationLogList As New List(Of ApplicationLogTO)
                myApplicationLogList.Add(pApplicationLogTO)

                myGlobalDataTO = Create(myApplicationLogList)

                'SGM 18/02/2013
                If SystemInfoManager.IsUpdateProcess Then
                    InsertUpdateVersionLog(pApplicationLogTO)
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
        Public Shared Function Create(ByVal pApplicationLogTOList As List(Of ApplicationLogTO)) As GlobalDataTO
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
        Public Shared Function DeleteAll() As GlobalDataTO
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
        Private Shared Function ManageLogFiles(ByVal pCurrentDate As String, _
                                        ByVal pLogMaxDays As Integer) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myLogPath As String = Application.StartupPath & XmlLogFilePath

                ' XBC+TR 03/10/2012 - Correction
                'If Not File.Exists(Application.StartupPath & GlobalBase.XmlLogFilePath) Then
                If Not Directory.Exists(Application.StartupPath & XmlLogFilePath) Then ' XB 28/05/2013 - Correction : condition must done by Directory instead of File
                    Directory.CreateDirectory(Application.StartupPath & XmlLogFilePath)
                End If
                ' XBC+TR 03/10/2012 - Correction

                Dim myLogFile As FileInfo
                Dim myLogDIR As New DirectoryInfo(myLogPath)
                Dim myLogFilesList As FileInfo() = myLogDIR.GetFiles("Ax00Log_*.xml")

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
                            Dim myTmpDIR As New DirectoryInfo(myTempFolder)                       'DL 04/06/2013
                            Dim myTmpFilesList As FileInfo() = myTmpDIR.GetFiles("Ax00Log_*.xml") 'DL 04/06/2013
                            Dim myTmpFile As FileInfo                                             'DL 04/06/2013
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
        Public Shared Function ExportLogToXml(ByVal pWorkSessionID As String, _
                                       ByVal pLogMaxDays As Integer) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing

            Try
                'Get the date part for the identifier of the active WorkSession
                Dim wsDatePart As String = pWorkSessionID.Substring(0, 8)
                Dim myLogPath As String = Application.StartupPath & XmlLogFilePath
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


        'End Function
#End Region

#Region "UPDATE VERSION"

        Private Shared LockObject As Object 'RH 26/03/2012
        Private Shared ReadOnly myUpdateLogPath As String = Application.StartupPath & My.Settings.PreviousFolder & UpdateLogFile


        'Public Sub InsertUpdateVersionLog(ByVal MyApplicationLogTO As ApplicationLogTO)

        'End Sub

        '''' <summary>
        '''' Insert a Log Activity into the Log file, and System Log if WriteSystemLog is true
        '''' </summary>
        '''' <param name="MyApplicationLogTO"></param>
        '''' <param name="WriteSystemLog">If True Write to System Log, False Do not Write.</param>
        '''' <remarks>
        '''' Created by SG 18/02/2013 - for Update Version Error Log
        '''' </remarks>
        Private Shared Sub InsertUpdateVersionLog(ByVal MyApplicationLogTO As ApplicationLogTO)
            'Try

            LockObject = New Object
            SyncLock LockObject         '1. Start Log SynLock

                'Initialize xml log file
                If Not File.Exists(myUpdateLogPath) Then
                    InitUpdateVersionLogFile()
                End If


                Dim MyLogXmlDocument As New XmlDocument ' create a XmlDocument object to load the Log Xml file.

                '3 - Load and write Log File
                MyLogXmlDocument.Load(myUpdateLogPath) 'load the document.



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
                MyLogXmlDocument.Save(myUpdateLogPath) ' Save the document



            End SyncLock
        End Sub

        '''' <summary>
        '''' Initialize the LogWiter control writing in the Log file Xml.
        '''' </summary>
        '''' <remarks>Created by SG 18/02/2013</remarks>
        Private Shared Sub InitUpdateVersionLogFile()

            If Not Directory.Exists(Application.StartupPath & My.Settings.PreviousFolder) Then
                Directory.CreateDirectory(Application.StartupPath & My.Settings.PreviousFolder)
            End If

            Dim myLogFile As FileStream = New FileStream(myUpdateLogPath, FileMode.OpenOrCreate)

            Dim LogWriter As XmlTextWriter = New XmlTextWriter(myLogFile, Encoding.UTF8) 'create the XML Document.

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

    End Class
End Namespace