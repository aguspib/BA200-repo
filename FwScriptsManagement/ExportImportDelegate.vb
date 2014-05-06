'Created by: AG 10/05/2010

Option Strict On
Option Explicit On


Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.CommunicationsSwFw
Imports System.IO
Imports System.Configuration
Imports System.Text



Namespace Biosystems.Ax00.FwScriptsManagement

    Partial Public Class ExportImportDelegate

        Private ActiveFwScriptsData As FwScriptsDataTO

        Public Sub New(ByVal pFwScriptsData As FwScriptsDataTO)
            ActiveFwScriptsData = pFwScriptsData
        End Sub

#Region "Declarations"

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Export an Instructions (string) to a external text file
        ''' </summary>
        ''' <param name="pInstructions">Instructions string</param>
        ''' <param name="pExportPath">Path of the destination file</param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 27/09/2010</remarks>
        Public Function ExportTextInstructions(ByVal pInstructions As String, ByVal pExportPath As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim TextFileWriter As StreamWriter

            Try


                Dim myInstructions As String = CStr(myGlobal.SetDatos)

                TextFileWriter = New StreamWriter(pExportPath)

                'split the text into lines
                Dim myInstructionsLines As String() = pInstructions.Split(CChar(";"))
                For s As Integer = 0 To myInstructionsLines.Length - 1 Step 1
                    Dim myLine As String = myInstructionsLines(s).Trim & ";"
                    If myLine.Trim <> ";" Then
                        TextFileWriter.WriteLine(myLine)
                    End If
                Next

                TextFileWriter.Close()

                'if all OK returns the exporting instructions
                myGlobal.SetDatos = pInstructions


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString 'PG 15/10/2010 "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ExportImportDelegate.ExportTextInstructions", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function


        ''' <summary>
        ''' Export an InstructionsDS (related to specified Script) to a external text file
        ''' </summary>
        ''' <param name="pInstructions">Instructions dataset</param>
        ''' <param name="pExportPath">Path of the destination file</param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 27/09/2010</remarks>
        Public Function ExportInstructions(ByVal pInstructions As List(Of InstructionTO), ByVal pExportPath As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim TextFileWriter As StreamWriter

            Try

                'convert to string
                Dim myLAX00 As New LAX00Interpreter

                myGlobal = myLAX00.WriteFwScript(pInstructions)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then

                    Dim myInstructions As String = CStr(myGlobal.SetDatos)

                    TextFileWriter = New StreamWriter(pExportPath)

                    'split the text into lines
                    Dim myInstructionsLines As String() = myInstructions.Split(CChar(";"))
                    For s As Integer = 0 To myInstructionsLines.Length - 1 Step 1
                        Dim myLine As String = myInstructionsLines(s).Trim & ";"
                        If myLine.Trim <> ";" Then
                            TextFileWriter.WriteLine(myLine)
                        End If
                    Next

                    TextFileWriter.Close()

                    'if all OK returns the exporting instructions
                    myGlobal.SetDatos = pInstructions

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString 'PG 15/10/2010 "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ExportImportDelegate.ExportInstructions", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Export an InstructionsDS (related to specified Script) to a external text file
        ''' </summary>
        ''' <param name="pFwScriptID">Identifier of the Script</param>
        ''' <param name="pExportPath">Path of the destination file</param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 27/09/2010</remarks>
        Public Function ExportFwScript(ByVal pFwScriptID As Integer, ByVal pExportPath As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                Dim myFwScriptsDelegate As New FwScriptsEditionDelegate("", Me.ActiveFwScriptsData)
                myGlobal = myFwScriptsDelegate.GetInstructionsByFwScriptID(pFwScriptID)
                If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then

                    'get the instructions list
                    Dim myFwScriptInstructions As List(Of InstructionTO)
                    myFwScriptInstructions = CType(myGlobal.SetDatos, List(Of InstructionTO))

                    myGlobal = Me.ExportInstructions(myFwScriptInstructions, pExportPath)

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString 'PG 15/11/2010 "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ExportImportDelegate.ExportFwScript", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Imports an InstructionsDS (related to specified Script) from a external text file
        ''' </summary>
        ''' <param name="pImportPath">Path of the origin file</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  SG 27/09/2010
        ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
        ''' </remarks>
        Public Function ImportFwScript(ByVal pImportPath As String, ByVal pTargetFwScriptsData As FwScriptsDataTO) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim TextFileReader As StreamReader
            Try
                If File.Exists(pImportPath) Then
                    TextFileReader = New StreamReader(pImportPath)

                    Dim myInstructions As String = TextFileReader.ReadToEnd

                    'CHECK SYNTAXIS
                    Dim myCommDelegate As New FwScriptsEditionDelegate("", pTargetFwScriptsData)
                    myGlobal = myCommDelegate.CheckTextFwScript(myInstructions)
                    If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                        'CHECK SEQUENCE
                        myGlobal = myCommDelegate.CheckTextFwScript(myInstructions)

                        If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then
                            If CBool(myGlobal.SetDatos) Then
                                'create the Instructions List
                                Dim myInstructionsList As New List(Of InstructionTO)

                                Dim myTextLines() As String = myInstructions.Split(CChar(";"))
                                Dim myCounter As Integer = 1
                                For c As Integer = 0 To myTextLines.Length - 1
                                    Dim myLine As String = myTextLines(c).Trim
                                    If myLine.Length > 0 Then
                                        Dim pars As String() = myLine.Split(CChar(":"))
                                        If pars.Count > 0 Then
                                            Dim myNewInstruction As New InstructionTO
                                            With myNewInstruction
                                                .InstructionID = myCounter
                                                .Sequence = myCounter
                                                .Timer = CInt(pars(0).Trim)
                                                .Code = pars(1).Trim.ToUpperBS    ' ToUpper
                                                If pars.Length > 2 Then
                                                    .Params = pars(2).Trim
                                                Else
                                                    .Params = ""
                                                End If
                                            End With
                                            myInstructionsList.Add(myNewInstruction)
                                            myCounter = myCounter + 1
                                        End If
                                    End If
                                Next

                                myGlobal.SetDatos = myInstructionsList
                            End If
                        End If
                    End If
                Else
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = GlobalEnumerates.Messages.FWSCRIPT_FILE_MISSING.ToString  'PG 15/11/2010 "FILE_MISSING"
                    'Exit Try 'TR 17/10/2011 -Commented Exit Try to allow the finally executions and close the TextFileReader.
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString 'PG 15/11/2010 "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ExportImportDelegate.ImportFwScript", EventLogEntryType.Error, False)

            Finally
                If TextFileReader IsNot Nothing Then
                    TextFileReader.Close()
                    TextFileReader.Dispose()
                End If

            End Try

            Return myGlobal

        End Function

#End Region

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class

End Namespace
