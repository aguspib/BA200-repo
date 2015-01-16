Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.CommunicationsSwFw

Namespace Biosystems.Ax00.FwScriptsManagement

    Public Class FwScriptsEditionDelegate
        Inherits BaseFwScriptDelegate

#Region "Declarations"
        Private ActiveFwScriptsData As FwScriptsDataTO
#End Region

#Region "Constructors"
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsData As FwScriptsDataTO, Optional ByVal pFwScriptsDelegate As SendFwScriptsDelegate = Nothing)
            MyBase.New(pAnalyzerID) 'SGM 20/01/2012
            ActiveFwScriptsData = pFwScriptsData.Clone
            myFwScriptDelegate = pFwScriptsDelegate
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        'Public Sub New(ByVal myAnalyzer As AnalyzerManager)
        '    Dim myGlobal As New GlobalDataTO
        '    ActiveScriptsData = New ScriptsDataTO 'COGER DE ANALYZER MANAGER
        '    myGlobal = myAnalyzer.ReadScriptData()
        '    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
        '        ActiveScriptsData = CType(myGlobal.SetDatos, ScriptsDataTO)
        '    End If
        'End Sub

        ' XBC 02/11/2011 - error multiples threads 
        Public Sub New(ByVal pAnalyzerID As String, ByVal pFwScriptsDelegate As SendFwScriptsDelegate)
            MyBase.New(pAnalyzerID) 'SGM 20/01/2012
            myFwScriptDelegate = pFwScriptsDelegate
            ActiveFwScriptsData = pFwScriptsDelegate.ActiveFwScriptsDO.Clone
        End Sub
        ' XBC 02/11/2011 - error multiples threads 

#End Region

#Region "Properties"

        Public Property ActiveFwScriptsDO() As FwScriptsDataTO
            Get
                Return ActiveFwScriptsData
            End Get
            Set(ByVal value As FwScriptsDataTO)
                ActiveFwScriptsData = value.Clone
            End Set
        End Property
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Gets a list of Screens related to specified Analyzer
        ''' </summary>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  SG 29/10/10
        ''' Modified by XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Public Function GetScreensByAnalyzerID(ByVal pAnalyzerID As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                Dim myScreensIDs As New List(Of String)
                For Each A As AnalyzerFwScriptsTO In ActiveFwScriptsData.Analyzers
                    'If A.AnalyzerID.ToUpper.Trim = pAnalyzerID.ToUpper.Trim Then
                    If A.AnalyzerID.Trim = pAnalyzerID.Trim Then
                        myGlobal.SetDatos = A.ScreenIDs
                    End If
                Next

                If Not myGlobal.HasError And Not myGlobal Is Nothing Then
                    myScreensIDs = CType(myGlobal.SetDatos, List(Of String))
                    Dim myScreens As New List(Of ScreenTO)
                    For Each Sn1 As String In myScreensIDs
                        For Each Sn2 As ScreenTO In ActiveFwScriptsData.Screens
                            'If Sn1.ToUpper.Trim = Sn2.ScreenID.ToUpper.Trim Then
                            If Sn1.Trim = Sn2.ScreenID.Trim Then
                                myScreens.Add(Sn2)
                                Exit For
                            End If
                        Next
                    Next

                    'order alphabetically
                    myGlobal.SetDatos = (From a In myScreens Order By a.ScreenID Select a).ToList()

                    'myGlobal.SetDatos = myScreens
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.GetScreensByAnalyzerID", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Gets the Script's data from an specified Action ID 
        ''' </summary>
        ''' <param name="ActionID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  SG 29/10/10
        ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
        ''' </remarks>
        Public Function GetFwScriptByActionID(ByVal ActionID As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                For Each S As FwScriptTO In ActiveFwScriptsData.FwScripts
                    'If S.ActionID.ToUpper.Trim = ActionID.ToUpper.Trim Then
                    If S.ActionID.ToUpperBS.Trim = ActionID.ToUpperBS.Trim Then
                        Dim myCopyFwScript As New FwScriptTO() With {.FwScriptID = S.FwScriptID, .ActionID = S.ActionID, .Description = S.Description, .Created = S.Created, .Modified = S.Modified, .Author = S.Author, .SyntaxOK = S.SyntaxOK, .TestedOK = S.TestedOK, .Instructions = S.Instructions}
                        myGlobal.SetDatos = myCopyFwScript.Clone
                        Exit For
                    End If
                Next
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.GetFwScriptByActionID", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Get data of the specified Script identifier
        ''' </summary>
        ''' <param name="pFwScriptID">Script Identifier</param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>created by SG 29/10/10</remarks>
        Public Function GetFwScriptByID(ByVal pFwScriptID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                For Each S As FwScriptTO In ActiveFwScriptsData.FwScripts
                    If S.FwScriptID = pFwScriptID Then
                        resultData.SetDatos = S
                        Exit For
                    End If
                Next

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.GetFwScriptByID", EventLogEntryType.Error, False)

            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Gets all analyzers available from the Scripts Data
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAnalyzers() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                myGlobal.SetDatos = ActiveFwScriptsData.Analyzers

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.GetAnalyzers", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' Get the Instructions that belong to an specified Script
        ''' </summary>
        ''' <param name="pFwScriptID">List of Instructions</param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>created by SG 29/10/10</remarks>
        Public Function GetInstructionsByFwScriptID(ByVal pFwScriptID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                For Each S As FwScriptTO In ActiveFwScriptsData.FwScripts
                    If S.FwScriptID = pFwScriptID Then
                        resultData.SetDatos = S.Instructions
                    End If
                Next
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.GetInstructionsByFwScriptID", EventLogEntryType.Error, False)

            End Try

            Return resultData

        End Function


        ''' <summary>Update data of the specified script</summary>
        ''' <param name="pFwScript">inform that if there are an operation that means instructions added or erased</param> 
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>Created by SG 29/10/2010</remarks> 
        Public Function ModifyFwScript(ByVal pFwScript As FwScriptTO) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                'in this case Me.ActiveScriptsData must be Global Application Data!!
                resultData = MyClass.GetFwScriptByID(pFwScript.FwScriptID)
                If Not resultData.HasError And Not resultData Is Nothing Then
                    Dim myFwScript As New FwScriptTO
                    myFwScript = CType(resultData.SetDatos, FwScriptTO)
                    If myFwScript IsNot Nothing Then
                        Dim i As Integer = MyClass.ActiveFwScriptsData.FwScripts.IndexOf(myFwScript)
                        MyClass.ActiveFwScriptsData.FwScripts(i) = pFwScript
                        myFwScript = pFwScript

                        resultData.SetDatos = pFwScript

                    Else
                        resultData.SetDatos = Nothing
                    End If

                End If


            Catch ex As Exception

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.ModifyFwScript", EventLogEntryType.Error, False)


            End Try

            Return resultData

        End Function

        ''' <summary>
        ''' Creates the Script List for test scripts in edition
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 23/11/10</remarks>
        Public Function SendQueueForTESTING(ByVal pFwScriptID As String, ByVal pParams As List(Of String)) As GlobalDataTO
            Dim myResultData As New GlobalDataTO
            Try

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                'FOR TESTING
                '*************************************************************************************************
                Dim myFwScript1 As New FwScriptQueueItem

                'Script1
                With myFwScript1
                    .FwScriptID = pFwScriptID
                    .EvaluateType = EVALUATE_TYPES.NUM_VALUE
                    .EvaluateValue = 1
                    .NextOnResultOK = Nothing
                    .NextOnResultNG = Nothing
                    .NextOnTimeOut = Nothing
                    .NextOnError = Nothing

                    If Not pParams Is Nothing Then
                        .ParamList = New List(Of String)
                        .ParamList = pParams
                    Else
                        .ParamList = Nothing ' New List(Of String)
                    End If
                End With

                '*************************************************************************************************************
                ''in case of specific handling the response
                '.ResponseEventHandling  = True 'enable event handling
                'AddHandler .ResponseEvent, AddressOf OnResponseEvent 'add event handler

                'add to the queue list
                If Not myResultData.HasError Then myResultData = myFwScriptDelegate.AddToFwScriptQueue(myFwScript1, True)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                If myFwScriptDelegate.CurrentFwScriptsQueue IsNot Nothing Then
                    myFwScriptDelegate.CurrentFwScriptsQueue.Clear()
                End If

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.SendQueueForTESTING", EventLogEntryType.Error, False)
            End Try
            Return myResultData
        End Function
#End Region

#Region "Communication Linking Methods"
        ''' <summary>
        ''' overwrites the Scripts Data of the Application with a XML file
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 10/11/10</remarks>
        Public Function UpdateAppFwScriptsData(Optional ByVal pPath As String = "") As GlobalDataTO

            Dim myResultData As New GlobalDataTO

            Try
                Dim myFwScripts As New FwScripts
                myResultData = myFwScripts.GetFwScriptData(pPath)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.UpdateAppFwScriptsData", EventLogEntryType.Error, False)

            End Try

            Return myResultData

        End Function

        ''' <summary>
        ''' overwrites the Scripts Data of the Application with the Factory XML file
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 10/11/10</remarks>
        Public Function UpdateAppFwScriptsDataToFactory() As GlobalDataTO

            Dim myResultData As New GlobalDataTO

            Try
                Dim myFwScripts As New FwScripts
                myResultData = myFwScripts.GetFactoryFwScriptData()
                If Not myResultData.HasError And myResultData.SetDatos IsNot Nothing Then
                    Dim myFactoryData As FwScriptsDataTO = CType(myResultData.SetDatos, FwScriptsDataTO)
                    If myFactoryData IsNot Nothing Then
                        myResultData = MyClass.SetAppFwScriptsData(myFactoryData)
                    End If
                End If

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.UpdateAppFwScriptsDataToFactory", EventLogEntryType.Error, False)

            End Try

            Return myResultData

        End Function

        ''' <summary>
        ''' overwrites the Scripts Data of the Application
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 10/11/10</remarks>
        Public Function SetAppFwScriptsData(ByVal pNewFwScriptsData As FwScriptsDataTO) As GlobalDataTO

            Dim myResultData As New GlobalDataTO

            Try
                Dim myFwScripts As New FwScripts
                myFwScripts.FwScriptsData = MyClass.ActiveFwScriptsData.Clone

                myResultData = myFwScripts.SetFwScriptData(pNewFwScriptsData)




            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.SetAppFwScriptsData", EventLogEntryType.Error, False)

            End Try

            Return myResultData

        End Function

        ''' <summary>
        ''' exports the Scripts Data of the Application to an external file (encrypted or decrypted)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 22/10/11</remarks>
        Public Function ExportFwScriptsData(ByVal pPath As String, ByVal pDecrypted As Boolean) As GlobalDataTO

            Dim myResultData As New GlobalDataTO

            Try
                Dim myFwScripts As New FwScripts
                myFwScripts.FwScriptsData = MyClass.ActiveFwScriptsData.Clone



                myResultData = myFwScripts.ExportFwScriptsDataToXML(pPath, Not pDecrypted, myFwScripts.FwScriptsData)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.ExportFwScriptsData", EventLogEntryType.Error, False)

            End Try

            Return myResultData

        End Function

        ''' <summary>
        ''' checks that the syntax of the script is OK
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 10/11/10</remarks>
        Public Function CheckFwScriptInstruction(ByVal pInstruction As String) As GlobalDataTO

            Dim myResultData As New GlobalDataTO

            Try
                Dim myFwScripts As New FwScripts
                myResultData = myFwScripts.CheckInstruction(pInstruction)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.CheckFwScriptInstruction", EventLogEntryType.Error, False)

            End Try

            Return myResultData

        End Function

        ''' <summary>
        ''' check that the sequence of the Script is OK
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 10/11/10</remarks>
        Public Function CheckFwScript(ByVal pInstructions As List(Of InstructionTO)) As GlobalDataTO

            Dim myResultData As New GlobalDataTO

            Try
                Dim myFwScripts As New FwScripts
                myResultData = myFwScripts.CheckFwScript(pInstructions)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.CheckFwScriptSequence", EventLogEntryType.Error, False)

            End Try

            Return myResultData

        End Function

        ''' <summary>
        ''' check that the text Script is OK
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 10/11/10</remarks>
        Public Function CheckTextFwScript(ByVal pInstructions As String) As GlobalDataTO

            Dim myResultData As New GlobalDataTO

            Try
                Dim myFwScripts As New FwScripts
                myResultData = myFwScripts.CheckTextFwScript(pInstructions)

            Catch ex As Exception
                myResultData.HasError = True
                myResultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "FwScriptsEditionDelegate.CheckTextFwScript", EventLogEntryType.Error, False)

            End Try

            Return myResultData

        End Function
#End Region


#Region "Not implemented"
        '''' <summary>
        '''' Get list of Scripts in the specified Screen in the specified Screen list
        '''' </summary>
        '''' <param name="pScreenID">Script Identifier</param>
        '''' <returns>GlobalDataTO</returns>
        '''' <remarks>created by SG 29/10/10</remarks>
        'Public Function GetScriptsByScreenID(ByVal pScreens As List(Of ScreenTO), ByVal pScreenID As String) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO

        '    Try
        '        Dim myScriptIDs As New List(Of Integer)
        '        For Each S As ScreenTO In pScreens
        '            If S.ScreenID.ToUpper.Trim = pScreenID.ToUpper.Trim Then
        '                resultData.SetDatos = S.ScriptIDs
        '            End If
        '        Next

        '        If Not resultData.HasError And Not resultData Is Nothing Then
        '            myScriptIDs = CType(resultData.SetDatos, List(Of Integer))
        '            Dim myScripts As New List(Of ScriptTO)
        '            For Each St1 As Integer In myScriptIDs
        '                For Each St2 As ScriptTO In Me.ActiveScriptsData.Scripts
        '                    If St1 = St2.ScriptID Then
        '                        myScripts.Add(St2)
        '                        Exit For
        '                    End If
        '                Next
        '            Next

        '            resultData.SetDatos = myScripts
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.GetScriptsByScreenID", EventLogEntryType.Error, False)

        '    End Try

        '    Return resultData

        'End Function

        '''' <summary>
        '''' gets the screens that are using the script
        '''' </summary>
        '''' <param name="pScreens"></param>
        '''' <param name="ScriptID"></param>
        '''' <returns></returns>
        '''' <remarks>Created by SG 29/10/10</remarks>
        'Public Function GetScriptOwnerScreens(ByVal pScreens As List(Of ScreenTO), ByVal pScriptID As Integer) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try
        '        Dim myScript As ScriptTO
        '        For Each T As ScriptTO In Me.ActiveScriptsData.Scripts
        '            If T.ScriptID = pScriptID Then
        '                myScript = T
        '                Exit For
        '            End If
        '        Next

        '        If myScript IsNot Nothing Then
        '            Dim myScreens As New List(Of ScreenTO)
        '            For Each S As ScreenTO In pScreens
        '                If Me.ContainsScript(S.ScriptIDs, myScript) Then
        '                    myScreens.Add(S)
        '                End If
        '            Next

        '            myGlobal.SetDatos = myScreens
        '        End If

        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.GetScriptOwnerScreens", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobal

        'End Function

        '''' <summary>
        '''' gets the analyzers that are using the screen
        '''' </summary>
        '''' <param name="pAnalyzers"></param>
        '''' <param name="ScreenID"></param>
        '''' <returns></returns>
        '''' <remarks>Created by SG 29/10/10</remarks>
        'Public Function GetScreenOwnerAnalyzers(ByVal pAnalyzers As List(Of AnalyzerScriptsTO), ByVal pScreenID As String) As GlobalDataTO

        '    Dim myGlobal As New GlobalDataTO

        '    Try

        '        Dim myScreen As ScreenTO
        '        For Each S As ScreenTO In Me.ActiveScriptsData.Screens
        '            If S.ScreenID = pScreenID Then
        '                myScreen = S
        '                Exit For
        '            End If
        '        Next

        '        If myScreen IsNot Nothing Then
        '            Dim myAnalyzers As New List(Of AnalyzerScriptsTO)
        '            For Each A As AnalyzerScriptsTO In pAnalyzers
        '                If Me.ContainsScreen(A.ScreenIDs, myScreen) Then
        '                    myAnalyzers.Add(A)
        '                End If
        '            Next

        '            myGlobal.SetDatos = myAnalyzers
        '        End If


        '    Catch ex As Exception
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = "SYSTEM_ERROR"
        '        myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.GetScreenOwnerAnalyzers", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobal

        'End Function

        '''' <summary>
        '''' Get data of the specified Script Instruction
        '''' </summary>
        '''' <param name="pInstructionID">Script Instruction Identifier</param>
        '''' <returns>GlobalDataTO</returns>
        '''' <remarks>created by SG 29/10/10</remarks>
        'Public Function GetInstruction(ByVal pScript As ScriptTO, ByVal pInstructionID As Integer) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO

        '    Try
        '        For Each I As InstructionTO In pScript.Instructions
        '            If I.InstructionID = pInstructionID Then
        '                resultData.SetDatos = I
        '            End If
        '        Next

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = "SYSTEM_ERROR"
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.GetInstruction", EventLogEntryType.Error, False)

        '    End Try

        '    Return resultData

        'End Function

        '''' <summary>
        '''' Create a new Script
        '''' </summary>
        '''' <param name="pScript">Typed DataSet ScriptsDS containing data of the new Script</param>
        '''' <returns>GlobalDataTO</returns>
        '''' <remarks>created by SG 29/10/10</remarks>
        'Public Function AddScript(ByVal pActionID As String, _
        '                    ByVal pScript As ScriptTO, _
        '                    ByVal pInstructionsList As List(Of InstructionTO)) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO

        '    Try
        '        'it has to be added both to the Screen and to the full Script list
        '    Catch ex As Exception

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.AddScript", EventLogEntryType.Error, False)

        '    End Try

        '    Return resultData

        'End Function

        '''' <summary>
        '''' Modifies all the ScriptData (THIS LOGIC MUST BE AT ANALYZERMANAGER)
        '''' </summary>
        '''' <param name="pNewScriptData"></param>
        '''' <returns></returns>
        '''' <remarks>Created by SG 05/11/10</remarks>
        'Public Function ModifyAllScriptData(ByVal pNewScriptData As ScriptsDataTO) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO

        '    Try
        '        Me.ActiveScriptsData = pNewScriptData

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.ModifyAllScriptData", EventLogEntryType.Error, False)

        '    End Try

        '    Return resultData

        'End Function

        '''' <summary>
        '''' Delete the specified Script
        '''' </summary>
        '''' <param name="pScriptID">Script Identifier to be deleted</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>created by SG 29/10/10</remarks>
        'Public Function DeleteScript(ByVal pScriptID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        'it has to be deleted both in the Screen and in the full Script list

        '    Catch ex As Exception

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.DeleteScript", EventLogEntryType.Error, False)

        '    End Try

        '    Return resultData

        'End Function


        '''' <summary>
        '''' Create a new Script Instruction
        '''' </summary>
        '''' <param name="pInstructionList"></param>
        '''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        '''' <remarks>created by SG 29/10/10</remarks>
        'Public Function AddInstruction(ByVal pInstructionList As List(Of InstructionTO)) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        'it has to be added both to the Screen and to the full Script list

        '    Catch ex As Exception

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.AddInstruction", EventLogEntryType.Error, False)

        '    End Try

        '    Return resultData

        'End Function

        '''' <summary>
        '''' Update data of the specified Script Instruction
        '''' </summary>
        '''' <param name="pInstructionList"></param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>created by 29/10/10</remarks>
        'Public Function ModifyInstruction(ByVal pInstructionList As List(Of InstructionTO)) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        'it has to be modified both to the Screen and to the full Script list

        '    Catch ex As Exception

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.ModifyInstruction", EventLogEntryType.Error, False)


        '    End Try

        '    Return resultData

        'End Function

        '''' <summary>
        '''' Delete the specified Script Instruction
        '''' </summary>
        '''' <param name="pInstructionList"></param>
        '''' <returns>GlobalDataTO</returns>
        '''' <remarks>created by SG 29/10/10</remarks>
        'Public Function DeleteInstruction(ByVal pInstructionList As List(Of InstructionTO)) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        'it has to be deleted both from the Screen and from the full Script list

        '    Catch ex As Exception

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.DeleteInstruction", EventLogEntryType.Error, False)

        '    End Try

        '    Return resultData

        'End Function

        '''' <summary>
        '''' Delete the specified Script Instructions
        '''' </summary>
        '''' <param name="pScriptID">Script identifier</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>created by SG 29/10/10</remarks>
        'Public Function DeletebyScriptID(ByVal pScriptID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        'they have to be deleted both from the Screen and from the full Script list

        '    Catch ex As Exception


        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ScriptsEditionDelegate.DeletebyScriptID", EventLogEntryType.Error, False)


        '    End Try

        '    Return resultData

        'End Function


        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pScripts"></param>
        '''' <param name="pScript"></param>
        '''' <returns></returns>
        '''' <remarks>created by SG 29/10/10</remarks>
        'Public Function ContainsScript(ByVal pScripts As List(Of Integer), ByVal pScript As ScriptTO) As Boolean
        '    Try
        '        For Each S As Integer In pScripts
        '            If S = pScript.ScriptID Then
        '                Return True
        '            End If
        '        Next
        '    Catch ex As Exception
        '        Throw ex
        '    End Try

        '    Return False

        'End Function

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pScreens"></param>
        '''' <param name="pScreen"></param>
        '''' <returns></returns>
        '''' <remarks>created by SG 29/10/10</remarks>
        'Public Function ContainsScreen(ByVal pScreens As List(Of String), ByVal pScreen As ScreenTO) As Boolean
        '    Try
        '        For Each S As String In pScreens
        '            If S.ToUpper.Trim = pScreen.ScreenID.ToUpper.Trim Then
        '                Return True
        '            End If
        '        Next
        '    Catch ex As Exception
        '        Throw ex
        '    End Try

        '    Return False

        'End Function

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="pAnalyzers"></param>
        '''' <param name="pAnalyzer"></param>
        '''' <returns></returns>
        '''' <remarks>created by SG 29/10/10</remarks>
        'Public Function ContainsAnalyzer(ByVal pAnalyzers As List(Of AnalyzerScriptsTO), ByVal pAnalyzer As AnalyzerScriptsTO) As Boolean
        '    Try
        '        For Each S As AnalyzerScriptsTO In pAnalyzers
        '            If S.AnalyzerID.ToUpper.Trim = pAnalyzer.AnalyzerID.ToUpper.Trim Then
        '                Return True
        '            End If
        '        Next
        '    Catch ex As Exception
        '        Throw ex
        '    End Try

        '    Return False

        'End Function


#End Region

    End Class


End Namespace