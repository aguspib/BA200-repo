﻿Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO

Namespace Biosystems.Ax00.CommunicationsSwFw

    Public Class LAX00Interpreter

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 13/04/2010
        ''' </remarks>
        Public Function Write(ByVal myParametersList As List(Of ParametersTO)) As GlobalDataTO
            Dim myResult As String = ""
            Dim myGlobalDataTO As New GlobalDataTO()
            Try
                'Dim myInstruction As New Instructions()
                If myParametersList.Count > 0 Then
                    For Each myParameterTO As ParametersTO In myParametersList
                        myResult &= myParameterTO.ParameterID
                        If myParameterTO.ParameterValues <> "" Then
                            myResult &= ":"
                            myResult &= myParameterTO.ParameterValues
                        End If
                        myResult &= ";"
                    Next
                End If
                myGlobalDataTO.SetDatos = myResult

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "LAX00Interpreter.Write", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInsturction"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 13/04/2010
        ''' AG 11/10/2011
        ''' </remarks>
        Public Function Read(ByVal pInsturction As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO()
            Try
                Dim myInstruction As New Instructions()
                myInstruction.Clear()
                If pInsturction <> "" Then
                    For Each myParameter As String In pInsturction.Split(CChar(";"))
                        If myParameter.Trim <> "" Then  'AG 21/04/2010 - Last ; is the end instruction (isn't a parameter, dont add)
                            If myParameter.Contains(":") Then

                                'AG 11/10/2011 - some data can contain also the ":" character so we have to count the occurences
                                'myInstruction.Add(myParameter.Split(CChar(":"))(0), myParameter.Split(CChar(":"))(1))
                                Dim tempStr As String() = Split(myParameter, ":")
                                Dim occurenceNumber As Integer = tempStr.Count - 1
                                If occurenceNumber > 1 Then
                                    Dim myParameterValue As String = ""
                                    Dim addText As String = ""
                                    For i As Integer = 1 To tempStr.Length - 1
                                        If i > 1 Then addText = ":"
                                        myParameterValue &= addText & tempStr(i)
                                    Next
                                    myInstruction.Add(myParameter.Split(CChar(":"))(0), myParameterValue)
                                Else
                                    myInstruction.Add(myParameter.Split(CChar(":"))(0), myParameter.Split(CChar(":"))(1))
                                End If
                                'AG 11/10/2011

                            Else
                                myInstruction.Add(myParameter)
                            End If
                        End If

                    Next myParameter
                End If
                myGlobalDataTO.SetDatos = myInstruction.ParameterList
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "LAX00Interpreter.Read", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Apply additional business into lax00 interpretation due exists data using the reserved lax00 characters ';'
        ''' </summary>
        ''' <param name="pInstruction">Instruction received from BAx00</param>
        ''' <param name="pInstructionName">Instruction name</param>
        ''' <param name="pBarcodeFullTotal">The sample barcode full length configurated</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 11/10/2011
        ''' </remarks>
        Public Function ReadWithAdditionalBusiness(ByVal pInstruction As String, ByVal pInstructionName As GlobalEnumerates.AppLayerInstrucionReception, _
                                                   ByVal pBarcodeFullTotal As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO()
            Try
                Dim myInstruction As New Instructions()
                Dim temporal As String = pInstruction

                myInstruction.Clear()
                If pInstruction <> "" Then

                    Select Case pInstructionName
                        Case GlobalEnumerates.AppLayerInstrucionReception.ANSCBR
                            'A400;ANSCBR;R:<value>;ST:<value>;N:<value>;P:<value>;D:<value>;V:<string value>;
                            '
                            'P, D and V parameters are loop N times. The only parameter value that can contain ';' inside data is the 'V' parameter, only when barcode sample is read

                            Dim maxLoopItera As Integer = 1
                            'Decode until parameter N
                            For Each myParameter As String In pInstruction.Split(CChar(";"))
                                If myParameter.Trim <> "" Then  'AG 21/04/2010 - Last ; is the end instruction (isn't a parameter, dont add)
                                    If myParameter.Contains(":") Then
                                        myInstruction.Add(myParameter.Split(CChar(":"))(0), myParameter.Split(CChar(":"))(1))
                                    Else
                                        myInstruction.Add(myParameter)
                                    End If
                                End If
                                temporal = temporal.Remove(0, myParameter.Length + 1) 'update temporal variable with the instruction pending to decode

                                If myParameter.Contains("N:") Then
                                    maxLoopItera = CInt(myParameter.Split(CChar(":"))(1))
                                    If maxLoopItera = 0 Then maxLoopItera = 1 'Protection case
                                    Exit For
                                End If

                            Next myParameter

                            'Decode the loop parameters (P, D and V)
                            Dim loopingParametersNumber As Integer = 3 'Three loop parameters: P, D and V
                            Dim diagnosticValue As Integer = 0
                            For i As Integer = 1 To maxLoopItera * loopingParametersNumber

                                'Use the standard decodification but the case V: and diagnostic is read OK, in this case use the special decodification
                                If temporal.StartsWith("P:") OrElse temporal.StartsWith("D:") OrElse (temporal.StartsWith("V:") AndAlso diagnosticValue <> 1) Then

                                    For Each myParameter As String In temporal.Split(CChar(";"))
                                        If myParameter.Trim <> "" Then  'AG 21/04/2010 - Last ; is the end instruction (isn't a parameter, dont add)
                                            If myParameter.Contains(":") Then
                                                myInstruction.Add(myParameter.Split(CChar(":"))(0), myParameter.Split(CChar(":"))(1))
                                            Else
                                                myInstruction.Add(myParameter)
                                            End If
                                        End If
                                        temporal = temporal.Remove(0, myParameter.Length + 1) 'update temporal variable with the instruction pending to decode

                                        If myParameter.StartsWith("D:") Then
                                            diagnosticValue = CInt(myParameter.Split(CChar(":"))(1))
                                        Else
                                            diagnosticValue = 0
                                        End If

                                        Exit For 'Get only the first parameter and parameter values
                                    Next myParameter

                                Else 'Case 'V' parameter and read OK is decoded using the pBarcodeFullTotal value
                                    Dim myValue As String = temporal.Substring(2, pBarcodeFullTotal)
                                    myInstruction.Add("V", myValue)
                                    myValue = "V:" & myValue
                                    temporal = temporal.Remove(0, myValue.Length + 1) 'update temporal variable with the instruction pending to decode
                                End If
                            Next

                        Case Else

                    End Select

                End If
                myGlobalDataTO.SetDatos = myInstruction.ParameterList

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "LAX00Interpreter.ReadWithAdditionalBusiness", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function


#Region "SERVICE SOFTWARE"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: SG 20/09/10
        ''' Modified by XBC 26/01/11
        ''' </remarks>
        Public Function WriteFwScript(ByVal pInstructions As List(Of InstructionTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myString As String = ""
                For Each mi As InstructionTO In pInstructions
                    If mi.Params.Length = 0 Then
                        'without parameters
                        myString &= mi.Timer.ToString.PadLeft(6, CChar("0")) & ":" & mi.Code.Trim & ";"
                    Else
                        myString &= mi.Timer.ToString.PadLeft(6, CChar("0")) & ":" & mi.Code.Trim & ":" & mi.Params & ";"
                    End If
                Next

                myGlobal.SetDatos = myString
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "LAX00Interpreter.WriteFwScript", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructions"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: SG 20/09/10
        ''' Modified by XBC 26/01/11
        ''' </remarks>
        Public Function ReadFwScript(ByVal pInstructions As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim SequenceOK As Boolean = True
            Dim SyntaxOK As Boolean = True
            Try
                Dim MyInstructions As String() = pInstructions.Split(CChar(";"))
                If MyInstructions.Length > 0 Then
                    Dim myInstructionsList As New List(Of InstructionTO)
                    Dim myTimer As Integer = -1 ' Modified by XBC 11/05/2011 - timer can be zero
                    For m As Integer = 0 To MyInstructions.Length - 1
                        If MyInstructions(m).Trim <> "" Then
                            Dim myArgs As String() = MyInstructions(m).Split(CChar(":"))
                            ' Maybe without parameters
                            If myArgs.Length >= 2 Then
                                Dim myInstruction As New InstructionTO

                                With myInstruction
                                    .Timer = CInt(myArgs(0))
                                    .Code = myArgs(1)
                                    ' Modified by XBC 25/01/2011 - Why ?
                                    'If .Code.Contains(".") Or .Code.Contains(":") Or .Code.Contains(";") Then
                                    If .Code.Contains(":") Or .Code.Contains(";") Then
                                        myGlobal.HasError = True
                                        myGlobal.ErrorCode = GlobalEnumerates.CHECK_RESULTS.SYNTAX_ERROR.ToString
                                        myGlobal.ErrorMessage = "Syntax Error in Instruction " & m.ToString
                                        Exit For
                                    End If
                                    If myArgs.Length = 3 Then
                                        .Params = myArgs(2)
                                    End If
                                End With

                                

                                If myTimer > myInstruction.Timer Then ' Modified by XBC 11/05/2011 - timer can be the same between some diferent instructions
                                    myGlobal.HasError = True
                                    myGlobal.ErrorCode = GlobalEnumerates.CHECK_RESULTS.SEQUENCE_ERROR.ToString
                                    myGlobal.ErrorMessage &= "Sequence Error in Instruction " & m.ToString & "|"
                                    SequenceOK = False
                                End If

                                myTimer = myInstruction.Timer
                                myInstructionsList.Add(myInstruction)

                            Else
                                myGlobal.HasError = True
                                myGlobal.ErrorCode = GlobalEnumerates.CHECK_RESULTS.SYNTAX_ERROR.ToString
                                myGlobal.ErrorMessage &= "Generic Syntax Error" & m.ToString & "|"
                                SyntaxOK = False
                            End If
                        End If
                    Next

                    myGlobal.SetDatos = myInstructionsList

                Else
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = GlobalEnumerates.CHECK_RESULTS.SYNTAX_ERROR.ToString
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "LAX00Interpreter.ReadFwScript", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function
#End Region
    End Class

End Namespace

