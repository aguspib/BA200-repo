Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports System.Windows.Forms
Imports System.IO
Imports System.Xml.Serialization
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.CommunicationsSwFw

    Public Class FwScripts

#Region "Attributes"
        Private FwScriptsDataAttribute As FwScriptsDataTO
        Private FwScriptsDataErrorCodeAttribute As String = ""
#End Region

#Region "Properties"
        Public Property FwScriptsData() As FwScriptsDataTO
            Get
                Return FwScriptsDataAttribute
            End Get

            Set(ByVal value As FwScriptsDataTO)
                FwScriptsDataAttribute = value
            End Set
        End Property

        Public ReadOnly Property FwScriptsDataErrorCode() As String
            Get
                Return FwScriptsDataErrorCodeAttribute
            End Get
        End Property
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor overloaded to use just for short instances
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()

        End Sub

        ''' <summary>
        ''' Constructor to use just at the first initialization of the application 
        ''' This instance of the object is used along the life of the execution
        ''' </summary>
        ''' <param name="pStartUp"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal pStartUp As Boolean)
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myGlobalbase As New GlobalBase
                Dim XMLFactoryFwScriptFileNamePath As String = Application.StartupPath & myGlobalbase.FactoryXmlFwScripts
                Dim XMLFwScriptFileNamePath As String = Application.StartupPath & GlobalBase.XmlFwScripts


                If Not File.Exists(XMLFwScriptFileNamePath) Then
                    If File.Exists(XMLFactoryFwScriptFileNamePath) Then
                        File.Copy(XMLFactoryFwScriptFileNamePath, XMLFwScriptFileNamePath)
                    Else
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_DATA_MISSING.ToString
                        FwScriptsDataErrorCodeAttribute = myGlobalDataTO.ErrorCode
                        Exit Try
                    End If

                Else
                    'SGM 19/12/2011
                    'Factory Scripts Data update automation
                    'it is updated in case of the Factory scripts are newer than current scripts
                    Dim CurrentFileChangedDatetime As DateTime = New FileInfo(XMLFwScriptFileNamePath).LastWriteTime
                    Dim FactoryFileCreatedDatetime As DateTime = New FileInfo(XMLFactoryFwScriptFileNamePath).LastWriteTime

                    If FactoryFileCreatedDatetime > CurrentFileChangedDatetime Then
                        '#If DEBUG Then
                        Dim res As DialogResult = DialogResult.OK
                        'res = MessageBox.Show("New Factory Scripts detected! (" & FactoryFileCreatedDatetime.ToString() & ")" & vbCrLf & _
                        '                      "Do you want to replace the current Active Scripts " & "(" & CurrentFileChangedDatetime.ToString & ")" & _
                        '                      " with the new Factory Scripts? (A backup of a previous Scripts will be created).", _
                        '                      "SCRIPTS", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)
                        If res = DialogResult.OK Then
                            '#End If
                            File.Copy(XMLFwScriptFileNamePath, XMLFwScriptFileNamePath.Replace(".xml", "_" & Now.ToString("yyyy-MM-dd_HH-mm-ss") & ".xml")) 'backup
                            File.Delete(XMLFwScriptFileNamePath)
                            File.Copy(XMLFactoryFwScriptFileNamePath, XMLFwScriptFileNamePath)
                        End If
                    End If
                    'end SGM 19/12/2011
                End If

                myGlobalDataTO = MyClass.GetFwScriptData()

                If Not myGlobalDataTO.HasError And Not myGlobalDataTO Is Nothing Then
                    MyClass.FwScriptsData = CType(myGlobalDataTO.SetDatos, FwScriptsDataTO)
                End If

                If myGlobalDataTO.ErrorCode IsNot Nothing Then
                    FwScriptsDataErrorCodeAttribute = myGlobalDataTO.ErrorCode
                End If


            Catch ex As Exception
                myGlobalDataTO.HasError = True

                GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()

                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.New", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region " Public Methods"
        ''' <summary>
        ''' Restore Scripts with the contents of Factory
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SG 06/11/10</remarks>
        Public Function GetFactoryFwScriptData() As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim OriginalFwScriptsData As New FwScriptsDataTO
                Dim myGlobalbase As New GlobalBase
                Dim XMLFactoryFwScriptFileNamePath As String = Application.StartupPath & myGlobalbase.FactoryXmlFwScripts

                resultData = MyClass.ImportFwScriptsDataFromXML(OriginalFwScriptsData.GetType, XMLFactoryFwScriptFileNamePath, True)

                If Not resultData.HasError And Not resultData Is Nothing Then
                    Dim myFwScriptsData As New FwScriptsDataTO
                    myFwScriptsData = CType(resultData.SetDatos, FwScriptsDataTO)
                    resultData = MyClass.CheckFwScriptData(myFwScriptsData)
                    If Not resultData.HasError And Not resultData Is Nothing Then
                        resultData.SetDatos = myFwScriptsData
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.GetFactoryFwScriptData", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function



        ''' <summary>
        ''' Get the list of full Scripts from XML phisical file
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SG 06/11/10</remarks>
        Public Function GetFwScriptData(Optional ByVal pPath As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim myGlobalbase As New GlobalBase
            Try
                Dim OriginalFwScriptsData As New FwScriptsDataTO
                Dim XMLFwScriptFileNamePath As String

                If pPath.Length = 0 Then
                    XMLFwScriptFileNamePath = Application.StartupPath & GlobalBase.XmlFwScripts
                Else
                    XMLFwScriptFileNamePath = pPath

                    'make a backup of the current Scripts data file
                    File.Copy(Application.StartupPath & GlobalBase.XmlFwScripts, Application.StartupPath & myGlobalbase.XmlFwScriptsWhileDecrypting, True)

                    System.Threading.Thread.Sleep(1)
                    'import the file
                    File.Copy(pPath, Application.StartupPath & GlobalBase.XmlFwScripts, True)
                End If

                resultData = ImportFwScriptsDataFromXML(OriginalFwScriptsData.GetType, XMLFwScriptFileNamePath, True)

                If Not resultData.HasError And Not resultData Is Nothing Then

                    Dim myFwScriptsData As FwScriptsDataTO
                    myFwScriptsData = CType(resultData.SetDatos, FwScriptsDataTO)

                    resultData = CheckFwScriptData(myFwScriptsData)
                    If Not resultData.HasError And Not resultData Is Nothing Then
                        resultData.SetDatos = myFwScriptsData
                    End If

                End If

                If pPath.Length > 0 Then
                    If resultData.HasError Then
                        'restore the backup
                        File.Copy(Application.StartupPath & myGlobalbase.XmlFwScriptsWhileDecrypting, Application.StartupPath & GlobalBase.XmlFwScripts, True)
                    End If

                    'File.Delete(Application.StartupPath & myGlobalbase.XmlScriptsWhileImporting)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.GetFwScriptsData", EventLogEntryType.Error, False)
            Finally
                If File.Exists(Application.StartupPath & myGlobalbase.XmlFwScriptsWhileDecrypting) Then
                    File.Delete(Application.StartupPath & myGlobalbase.XmlFwScriptsWhileDecrypting)
                End If
            End Try
            Return resultData
        End Function




        ''' <summary>
        ''' Update Scripts with user’s changes
        ''' </summary>
        ''' <param name="pNewFwScriptsData">object data scripts of the user</param>
        ''' <returns></returns>
        ''' <remarks>SG 06/11/10</remarks>
        Public Function SetFwScriptData(ByVal pNewFwScriptsData As FwScriptsDataTO) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim myGlobalbase As New GlobalBase
            Dim XMLFwScriptFileNamePath As String = Application.StartupPath & GlobalBase.XmlFwScripts
            Dim XMLCopyFwScriptFileNamePath As String = Application.StartupPath & "\temp.xml"
            Dim CopyOfFwScriptsData As New FwScriptsDataTO

            Try
                'first make a safety copy
                CopyOfFwScriptsData = MyClass.FwScriptsData.Clone

                'second update the Application's ScriptsData
                MyClass.FwScriptsData = pNewFwScriptsData.Clone

                'third update the Scripts Data XML File
                'make a safety copy of the file
                If File.Exists(XMLFwScriptFileNamePath) Then
                    File.Copy(XMLFwScriptFileNamePath, XMLCopyFwScriptFileNamePath)
                    File.Delete(XMLFwScriptFileNamePath)
                End If

                'tries to export to the XML file
                resultData = Me.ExportFwScriptsDataToXML(XMLFwScriptFileNamePath, True)


            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'in case of error restore the original data
                Me.FwScriptsData = CopyOfFwScriptsData.Clone

                'and restore the XML File
                If File.Exists(XMLFwScriptFileNamePath) Then
                    File.Delete(XMLFwScriptFileNamePath)
                End If

                File.Copy(XMLCopyFwScriptFileNamePath, XMLFwScriptFileNamePath)

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.SetFwScriptsData", EventLogEntryType.Error, False)

            Finally
                'delete the temp safety copy file
                If File.Exists(XMLCopyFwScriptFileNamePath) Then
                    File.Delete(XMLCopyFwScriptFileNamePath)
                End If
            End Try
            Return resultData
        End Function

        

        ''' <summary>
        ''' Generate a script instruction with the corresponding values
        ''' </summary>
        ''' <param name="pFwScriptID">Action/script identifier to sent</param>
        ''' <param name="pParams">In case to need dynamic parameters. If hasn't the value must be set to Nothing</param>
        ''' <returns></returns>
        ''' <remarks>SG 06/11/10</remarks>
        Public Function GenerateFwScriptInstruction(ByVal pFwScriptID As String, Optional ByVal pParams As List(Of String) = Nothing) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myInstructions As List(Of InstructionTO)
            Dim myInstructionstoSend As New List(Of InstructionTO)
            Try
                'Get the instruction parameter list corresponding to the script.
                myGlobalDataTO = GetInstructions(pFwScriptID)

                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    myInstructions = CType(myGlobalDataTO.SetDatos, List(Of InstructionTO))
                Else
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_VALIDATION_ERROR.ToString
                    Exit Try
                End If

                ' Check params that Script expects
                Dim params As List(Of String) = Nothing
                Dim paramsTmp As List(Of String) = Nothing
                'Dim valors As New List(Of String)
                For Each myInstruction As InstructionTO In myInstructions
                    'If Not myInstruction.EnableEdition Then
                    myGlobalDataTO = myInstruction.getFwScriptParams()
                    If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                        paramsTmp = CType(myGlobalDataTO.SetDatos, List(Of String))
                    Else
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_VALIDATION_ERROR.ToString
                        Exit Try
                    End If

                    For i As Integer = 0 To paramsTmp.Count - 1
                        If params Is Nothing Then
                            params = New List(Of String)
                        End If
                        params.Add(paramsTmp(i))
                    Next
                    'End If
                Next

                If pParams Is Nothing And params Is Nothing Then
                    myInstructionstoSend = myInstructions
                Else
                    ' Validate that params sent and params expected are success
                    If ValidateParams(pParams, params) Then
                        ' Cloning myInstructions to send with values of the parameters without afectations into Scripts XML original
                        For i As Integer = 0 To myInstructions.Count - 1
                            Dim myInstruction As New InstructionTO
                            myInstruction = CType(myInstructions(i).Clone, InstructionTO)
                            myInstructionstoSend.Add(myInstruction)
                        Next
                        For i As Integer = 0 To pParams.Count - 1
                            For Each myInstruction As InstructionTO In myInstructionstoSend
                                If myInstruction.Params.Contains("#" + (i + 1).ToString + "@") Then
                                    If pParams(i).ToString.Length > 0 Then
                                        myInstruction.Params = myInstruction.Params.Replace("#" + (i + 1).ToString + "@", pParams(i).ToString)
                                    Else
                                        myGlobalDataTO.HasError = True
                                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_VALIDATION_ERROR.ToString
                                        Exit Try
                                    End If
                                End If
                            Next
                        Next
                    Else
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_VALIDATION_ERROR.ToString
                        Exit Try
                    End If
                End If

                myGlobalDataTO.SetDatos = myInstructionstoSend

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.GenerateFwScriptInstruction", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Generate a script instruction with the corresponding values
        ''' </summary>
        ''' <param name="pInstructions">List of Instructions to sent</param>
        ''' <param name="pParams">In case to need dynamic parameters. If hasn't the value must be set to Nothing</param>
        ''' <returns></returns>
        ''' <remarks>XBC 20/09/2011</remarks>
        Public Function GenerateFwScriptInstruction(ByVal pInstructions As List(Of InstructionTO), Optional ByVal pParams As List(Of String) = Nothing) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myInstructionstoSend As New List(Of InstructionTO)
            Try
                ' Check params that Script expects
                Dim params As List(Of String) = Nothing
                Dim paramsTmp As List(Of String) = Nothing
                'Dim valors As New List(Of String)
                For Each myInstruction As InstructionTO In pInstructions
                    'If Not myInstruction.EnableEdition Then
                    myGlobalDataTO = myInstruction.getFwScriptParams()
                    If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                        paramsTmp = CType(myGlobalDataTO.SetDatos, List(Of String))
                    Else
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_VALIDATION_ERROR.ToString
                        Exit Try
                    End If

                    For i As Integer = 0 To paramsTmp.Count - 1
                        If params Is Nothing Then
                            params = New List(Of String)
                        End If
                        params.Add(paramsTmp(i))
                    Next
                    'End If
                Next

                If pParams Is Nothing And params Is Nothing Then
                    myInstructionstoSend = pInstructions
                Else
                    ' Validate that params sent and params expected are success
                    If ValidateParams(pParams, params) Then
                        ' Cloning pInstructions to send with values of the parameters without afectations into Scripts XML original
                        For i As Integer = 0 To pInstructions.Count - 1
                            Dim myInstruction As New InstructionTO
                            myInstruction = CType(pInstructions(i).Clone, InstructionTO)
                            myInstructionstoSend.Add(myInstruction)
                        Next
                        For i As Integer = 0 To pParams.Count - 1
                            For Each myInstruction As InstructionTO In myInstructionstoSend
                                If myInstruction.Params.Contains("#" + (i + 1).ToString + "@") Then
                                    If pParams(i).ToString.Length > 0 Then
                                        myInstruction.Params = myInstruction.Params.Replace("#" + (i + 1).ToString + "@", pParams(i).ToString)
                                    Else
                                        myGlobalDataTO.HasError = True
                                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_VALIDATION_ERROR.ToString
                                        Exit Try
                                    End If
                                End If
                            Next
                        Next
                    Else
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_VALIDATION_ERROR.ToString
                        Exit Try
                    End If
                End If

                myGlobalDataTO.SetDatos = myInstructionstoSend

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.GenerateFwScriptInstruction 2", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Validation Methods"
        ''' <summary>
        ''' Checks the syntax of a instruction
        ''' </summary>
        ''' <param name="pInstruction">Instruction to be validated</param>
        ''' <returns>True if OK False if NOK</returns>
        ''' <remarks>
        ''' Created by SG 27/09/2010
        ''' Modified by XBC 25/01/2011
        ''' </remarks>
        Public Function CheckInstruction(ByVal pInstruction As String) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Dim myStr As String = pInstruction
            Dim hasError As Boolean = False
            Try
                'It must end with ';'
                hasError = Not myStr.Trim.EndsWith(";")
                If Not hasError Then
                    'It must have two separators - three fields ':'
                    Dim pars As String() = myStr.Trim.Split(CChar(":"))
                    'hasError = Not (pars.Length = 3)
                    hasError = Not (pars.Length >= 2)

                    'each one must not have any ';'
                    If pars.Length = 3 Then
                        ' with parameters
                        For p As Integer = 0 To pars.Length - 2
                            If pars(p).Contains(";") Then
                                hasError = True
                                Exit For
                            End If
                            ' Modified by XBC 25/01/2011 - Why ?
                            'If pars(p).Contains(".") Then
                            '    hasError = True
                            '    Exit For
                            'End If
                        Next

                       
                    Else
                        ' without parameters
                        If pars(0).Contains(";") Then
                            hasError = True
                        End If
                    End If

                    If Not hasError Then
                        If pars.Count > 0 Then
                            'It must have temp mark and it must be numerical
                            Dim temp As String = pars(0)
                            Try
                                Dim num As Integer = CInt(temp)
                            Catch ex As Exception
                                hasError = True
                            End Try
                        Else
                            hasError = True
                        End If
                    End If
                End If

                If hasError Then
                    myglobal.HasError = True
                    myglobal.SetDatos = False
                Else
                    myglobal.SetDatos = True
                End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CheckSyntaxDelegate.CheckInstruction", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Check that the timing of each instructions is ascent
        ''' </summary>
        ''' <param name="pInstructions"></param>
        ''' <returns>boolean</returns>
        ''' <remarks>CREATED BY SG 27/09/2010</remarks>
        Public Function CheckTextFwScript(ByVal pInstructions As String) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            'Dim SequenceOK As Boolean = True
            Try
                Dim myLAX00 As New LAX00Interpreter
                myglobal = myLAX00.ReadFwScript(pInstructions.Trim)

                If (Not myglobal.HasError) And (Not myglobal.SetDatos Is Nothing) Then
                    myglobal.SetDatos = True
                End If
            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.CheckTextFwScriptSequence", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Check that the timing of each instructions is ascent
        ''' </summary>
        ''' <param name="pInstructions"></param>
        ''' <returns>boolean</returns>
        ''' <remarks>CREATED BY SG 30/09/2010</remarks>
        Public Function CheckFwScript(ByVal pInstructions As List(Of InstructionTO)) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Try
                myglobal.SetDatos = True

                Dim myLAX00 As New LAX00Interpreter
                myglobal = myLAX00.WriteFwScript(pInstructions)
                If (Not myglobal.HasError) And (Not myglobal.SetDatos Is Nothing) Then
                    Dim myInstructions As String = CStr(myglobal.SetDatos)
                    myInstructions = myInstructions.Replace(";", ";" & vbCrLf)
                    myglobal = Me.CheckTextFwScript(myInstructions)
                End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.CheckFwScriptSequence", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Get for the list of microinstructions that belongs to a specified ScriptID
        ''' </summary>
        ''' <param name="pFwScriptID">Action/Script identifier</param>
        ''' <returns></returns>
        ''' <remarks>SG 06/11/10</remarks>
        Private Function GetInstructions(ByVal pFwScriptID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If pFwScriptID <> "" Then
                    Dim myFwScript As New List(Of FwScriptTO)

                    myFwScript = (From s In MyClass.FwScriptsDataAttribute.FwScripts _
                                               Where s.ActionID = pFwScriptID _
                                               Select s).ToList()

                    ' Select to obtain instructions list that belong to ScriptID
                    If myFwScript.Count > 0 Then
                        resultData.SetDatos = (From a In myFwScript(0).Instructions Select a).ToList()
                    Else
                        resultData.HasError = True
                    End If
                    myFwScript = Nothing 'AG 02/08/2012 - free memory

                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.GetInstructions", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Deseralize full object to a phisical XML file
        ''' </summary>
        ''' <param name="pFilePath"></param>
        ''' <returns></returns>
        ''' <remarks>SG 06/11/10
        ''' Modified by SG 30/11/10 Encrypting</remarks>
        Public Function ExportFwScriptsDataToXML(ByVal pFilePath As String, ByVal pEncrypt As Boolean, Optional ByVal pFwScriptsData As FwScriptsDataTO = Nothing) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            'SG 30/11/10
            Dim myGlobalbase As New GlobalBase
            Dim myTempFile As String = Application.StartupPath & myGlobalbase.XmlFwScriptsTempFile
            Try
                'delete if previously exists
                If File.Exists(myTempFile) Then
                    File.Delete(myTempFile)
                End If

                'END SG 30/11/10

                Dim myFwScriptsData As FwScriptsDataTO
                If pFwScriptsData IsNot Nothing Then
                    myFwScriptsData = pFwScriptsData.Clone
                Else
                    myFwScriptsData = MyClass.FwScriptsDataAttribute.Clone
                End If
                Dim FS As FileStream

                If pEncrypt Then
                    FS = File.OpenWrite(myTempFile) 'SG 30/11/10
                Else
                    FS = File.OpenWrite(pFilePath) 'SG 30/11/10
                End If

                Dim serializer As New XmlSerializer(myFwScriptsData.GetType)
                serializer.Serialize(FS, myFwScriptsData)
                FS.Close()
                FS.Dispose()

                'SG 30/11/10
                If pEncrypt Then
                    'encrypt the file
                    If File.Exists(myTempFile) Then

                        Dim myUtil As New Utilities
                        resultData = myUtil.EncryptFile(myTempFile, pFilePath)

                    Else
                        Throw New Exception(GlobalEnumerates.Messages.SYSTEM_ERROR.ToString)
                    End If
                End If

                'END SG 30/11/10
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.ExportFwScriptsDataToXML", EventLogEntryType.Error, False)
            Finally
                'delete the temp file
                If File.Exists(myTempFile) Then
                    File.Delete(myTempFile)
                End If
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Seralize full object to a phisical XML file
        ''' </summary>
        ''' <param name="pFwScriptsDataType"></param>
        ''' <param name="pFilePath"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SG 06/11/10
        ''' Modified by PG 26/11/2010. Add the function Decrypt
        ''' Modified by SG 30/11/10
        ''' </remarks>
        Private Function ImportFwScriptsDataFromXML(ByVal pFwScriptsDataType As Type, ByVal pFilePath As String, ByVal pDecrypt As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim myUtil As New Utilities 'PG 26/11/2010 
            ' 
            Dim FS As FileStream = Nothing
            Dim myTempFilePath As String = ""
            Try
                'PG 26/11/2010. Add the function Decrypt 
                Dim serializer As New XmlSerializer(pFwScriptsDataType)

                If pDecrypt Then
                    Dim myGlobalBase As New GlobalBase
                    myTempFilePath = Application.StartupPath & myGlobalBase.XmlFwScriptsTempFile

                    If File.Exists(myTempFilePath) Then
                        File.Delete(myTempFilePath)
                    End If

                    resultData = myUtil.DecryptFile(pFilePath, myTempFilePath)

                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        If File.Exists(myTempFilePath) Then
                            FS = File.OpenRead(myTempFilePath)
                            resultData.SetDatos = CType(serializer.Deserialize(FS), FwScriptsDataTO)
                            FS.Close()
                            FS.Dispose()

                            File.Delete(myTempFilePath)
                        Else
                            resultData.HasError = True
                            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        End If
                    End If
                Else
                    'without decrypting
                    FS = File.OpenRead(pFilePath)
                    resultData.SetDatos = CType(serializer.Deserialize(FS), FwScriptsDataTO)
                    FS.Close()
                    FS.Dispose()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.ImportFwScriptsDataFromXML", EventLogEntryType.Error, False)
            Finally
                If Not FS Is Nothing Then
                    FS.Close()
                    FS.Dispose()
                End If
                If File.Exists(myTempFilePath) Then
                    File.Delete(myTempFilePath)
                End If
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Check dynamic parameters to send with the parameters expected by definition
        ''' </summary>
        ''' <param name="pParamsSent">parameters to send</param>
        ''' <param name="pParamsExpected">parameters expected</param>
        ''' <returns></returns>
        ''' <remarks>SG 06/11/10</remarks>
        Private Function ValidateParams(ByVal pParamsSent As List(Of String), ByVal pParamsExpected As List(Of String)) As Boolean
            Dim returnValue As Boolean
            Try
                If pParamsSent Is Nothing And pParamsExpected Is Nothing Then
                    returnValue = True
                Else
                    If Not pParamsSent Is Nothing And pParamsExpected Is Nothing Then
                        returnValue = False
                    ElseIf pParamsSent Is Nothing And Not pParamsExpected Is Nothing Then
                        returnValue = False
                    ElseIf Not pParamsSent Is Nothing And Not pParamsExpected Is Nothing Then
                        If pParamsSent.Count = pParamsExpected.Count Then
                            returnValue = True
                        Else
                            returnValue = False
                        End If
                    End If
                End If

            Catch ex As Exception
                returnValue = False
            End Try
            ValidateParams = returnValue
        End Function

        ''' <summary>
        ''' check the syntas of all the script data
        ''' </summary>
        ''' <param name="pFwScriptsData"></param>
        ''' <returns></returns>
        ''' <remarks>SG 23/11/10</remarks>
        Private Function CheckFwScriptData(ByVal pFwScriptsData As FwScriptsDataTO, Optional ByVal pSkipVersion As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                'check contents
                If pFwScriptsData.Analyzers.Count = 0 Or _
                   pFwScriptsData.Screens.Count = 0 Or _
                   pFwScriptsData.FwScripts.Count = 0 Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_DATA_ERROR.ToString
                    Exit Try
                End If

                'check version and compare it with the application's one
                'Dim myAppVersion() As String
                Dim myScriptsVersion() As String
                Dim isVersionOK As Boolean = True
                'Dim myUtil As New Utilities

                ' XBC 08/05/2012
                Dim myVersionsDelegate As New VersionsDelegate
                Dim myVersionsDS As New VersionsDS
                Dim myFwScriptsDBVersion() As String = Nothing

                resultData = myVersionsDelegate.GetVersionsByPackage(Nothing, "1") ' By now Package is a temporal TODO
                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    myVersionsDS = CType(resultData.SetDatos, VersionsDS)
                    If myVersionsDS.tfmwVersions.Rows.Count = 1 Then
                        myFwScriptsDBVersion = myVersionsDS.tfmwVersions(0).FirmwareScripts.Split(CChar("."))
                    End If

                    myScriptsVersion = pFwScriptsData.Version.Split(CChar("."))
                    If myFwScriptsDBVersion.Length > 0 AndAlso myScriptsVersion.Length > 0 Then
                        For v As Integer = 0 To myFwScriptsDBVersion.Length - 1 Step 1
                            If myFwScriptsDBVersion(v) <> myScriptsVersion(v) Then
                                isVersionOK = False
                                Exit For
                            End If
                        Next
                    End If
                End If

                'resultData = myUtil.GetSoftwareVersion
                'If Not pSkipVersion Then
                '    If Not resultData.HasError And Not resultData Is Nothing Then
                '        myAppVersion = CType(resultData.SetDatos, String).Split(CChar("."))
                '        myScriptsVersion = pFwScriptsData.Version.Split(CChar("."))
                '        If myAppVersion.Length > 0 AndAlso myScriptsVersion.Length > 0 Then
                '            For v As Integer = 0 To myAppVersion.Length - 1 Step 1
                '                If myAppVersion(v) <> myScriptsVersion(v) Then
                '                    isVersionOK = False
                '                    Exit For
                '                End If
                '            Next
                '        End If
                '    Else
                '        isVersionOK = False
                '    End If
                'End If
                ' XBC 08/05/2012

                If Not isVersionOK Then

                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_VERSION_ERROR.ToString

                Else

                    'PDT
                    'check that all the Application's screens are included in the Scripts data

                    'check syntax
                    For Each S As FwScriptTO In pFwScriptsData.FwScripts
                        resultData = CheckFwScript(S.Instructions)
                        If resultData.HasError Or resultData Is Nothing Then
                            resultData.ErrorMessage = "Error in Script " & S.ActionID & vbCrLf & resultData.ErrorMessage
                            Exit For
                        End If
                    Next

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.CheckFwScriptData", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region


#Region "For Creation"

        ''' <summary>
        ''' Get the Scripts Data for creation management
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SG 25/11/10</remarks>
        Public Function ReadFwScriptDataForCreating(ByVal pPath As String, ByVal pDecrypt As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim OriginalFwScriptsData As New FwScriptsDataTO
                'Dim myGlobalbase As New GlobalBase
                Dim XMLFwScriptFileNamePath As String

                XMLFwScriptFileNamePath = pPath

                resultData = ImportFwScriptsDataFromXML(OriginalFwScriptsData.GetType, XMLFwScriptFileNamePath, pDecrypt)

                If Not resultData.HasError And Not resultData Is Nothing Then

                    Dim myFwScriptsData As FwScriptsDataTO
                    myFwScriptsData = CType(resultData.SetDatos, FwScriptsDataTO)

                    resultData = CheckFwScriptData(myFwScriptsData, True)
                    If Not resultData.HasError And Not resultData Is Nothing Then
                        resultData.SetDatos = myFwScriptsData
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.ReadFwScriptDataForCreating", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Writes the Script Data to an external XML file
        ''' </summary>
        ''' <param name="pFwScriptsData">object data scripts of the user</param>
        ''' <param name="pPath">object data scripts of the user</param>
        ''' <returns></returns>
        ''' <remarks>SG 25/11/10</remarks>
        Public Function WriteFwScriptDataForCreating(ByVal pFwScriptsData As FwScriptsDataTO, ByVal pPath As String, ByVal pEncrypt As Boolean) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            'Dim myGlobalbase As New GlobalBase

            Try
                If pFwScriptsData IsNot Nothing Then
                    resultData = Me.ExportFwScriptsDataToXML(pPath, pEncrypt, pFwScriptsData)
                Else
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_DATA_MISSING.ToString
                End If


            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "FwScripts.WriteFwScriptDataForCreating", EventLogEntryType.Error, False)

            End Try
            Return resultData
        End Function

#End Region

    End Class

End Namespace

