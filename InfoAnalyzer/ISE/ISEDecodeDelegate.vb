'Option Strict On
'Option Explicit On

'Imports Biosystems.Ax00.Global
'Imports System.Xml

'Namespace Biosystems.Ax00.InfoAnalyzer
'    ''' <summary>
'    ''' Auxiliar library for result delegate.
'    ''' Handle the convertion and treatment for the ISE recived results.
'    ''' </summary>
'    ''' <remarks>
'    ''' CREATE BY: TR 
'    ''' </remarks>
'    Public Class ISEDecodeDelegate

'#Region "Declarations"

'        Private ReadOnly ISEErrorAffectedIonesHT As New Hashtable()
'        Private ReadOnly ISECancelErrorsHT As New Hashtable()

'#End Region

'#Region "Constructor"
'        Public Sub New()
'            MyClass.LoadISEErrorsDataHT()
'        End Sub
'#End Region



'#Region "common"

'        Private Sub LoadISEErrorsDataHT()
'            Try
'                Dim myISEParamXmlDS As New DataSet

'                Dim myISEParamXml As New XmlDocument
'                Dim myXmlPath As String = Windows.Forms.Application.StartupPath.ToString() & GlobalBase.ISEParammetersFilePath
'                myISEParamXml.Load(myXmlPath)
'                myISEParamXmlDS.ReadXml(myXmlPath)

'                For Each iseRow As DataRow In myISEParamXmlDS.Tables("AffectedElementTable").Rows
'                    ISEErrorAffectedIonesHT.Add(iseRow("id").ToString(), iseRow("AffecteElements").ToString())
'                Next
'                For Each iseRow As DataRow In myISEParamXmlDS.Tables("ISEModuleErrors").Rows
'                    ISECancelErrorsHT.Add(iseRow("id").ToString(), iseRow("AffecteElements").ToString())
'                Next


'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEDecodeDelegate.LoadISEErrorsDataHT", EventLogEntryType.Error, False)
'            End Try

'        End Sub

'        ''' <summary>
'        ''' Esta linea peta 'A400;ANSISE;P:7;R:ERC COM T000000;
'        ''' </summary>
'        ''' <param name="pDataStr"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 15/01/2012</remarks>
'        Public Function GetResultErrors(ByVal pDataStr As String) As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO
'            Try
'                Dim myResultErrors As New List(Of ISEErrorTO)

'                Dim myResultError As ISEErrorTO
'                Dim myStr As String = pDataStr.Trim
'                If myStr.Length > 0 Then
'                    myResultError = New ISEErrorTO


'                    Dim Position As Integer = 1
'                    For Each posValue As Char In myStr
'                        'Start from position 1 because position 0 has the IseModule Error or independent errors
'                        If Position > 1 And posValue <> "0" Then

'                            myResultError = New ISEErrorTO

'                            myResultError.DigitNumber = Position
'                            myResultError.DigitValue = posValue
'                            myResultError.Message = "ISE_REMARK" & Position

'                            If MyClass.ISEErrorAffectedIonesHT.ContainsKey(CStr(posValue)) Then
'                                myResultError.Affected = MyClass.ISEErrorAffectedIonesHT(CStr(posValue)).ToString()
'                            End If

'                            myResultErrors.Add(myResultError)
'                        End If
'                        Position += 1
'                    Next

'                    'End If


'                End If

'                myGlobal.SetDatos = myResultErrors

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = "SYSTEM_ERROR"
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEDecodeDelegate.GetResultErrors", EventLogEntryType.Error, False)
'            End Try
'            Return myGlobal
'        End Function

'        Public Function GetCancelError(ByVal pDataStr As String) As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO
'            Try
'                Dim myResultErrors As New List(Of ISEErrorTO)

'                Dim myResultError As ISEErrorTO
'                Dim myStr As String = pDataStr.Trim
'                If myStr.Length > 0 Then
'                    myResultError = New ISEErrorTO

'                    Dim myErrorCycle As ISEErrorTO.ErrorCycles

'                    Select Case myStr.Substring(0, 3).ToUpper
'                        Case "CAL" : myErrorCycle = ISEErrorTO.ErrorCycles.Calibration
'                        Case "SER" : myErrorCycle = ISEErrorTO.ErrorCycles.Sample
'                        Case "URN" : myErrorCycle = ISEErrorTO.ErrorCycles.Urine
'                        Case "CLE" : myErrorCycle = ISEErrorTO.ErrorCycles.Clean
'                        Case "PMC" : myErrorCycle = ISEErrorTO.ErrorCycles.PumpCalibration
'                        Case "BBC" : myErrorCycle = ISEErrorTO.ErrorCycles.BubbleDetCalibration
'                        Case "SIP" : myErrorCycle = ISEErrorTO.ErrorCycles.SipCycle
'                        Case "PGA" : myErrorCycle = ISEErrorTO.ErrorCycles.PurgeA
'                        Case "PGB" : myErrorCycle = ISEErrorTO.ErrorCycles.PurgeB
'                        Case "DAL" : myErrorCycle = ISEErrorTO.ErrorCycles.DallasWrite
'                        Case "MAT" : myErrorCycle = ISEErrorTO.ErrorCycles.Maintenance
'                        Case "COM" : myErrorCycle = ISEErrorTO.ErrorCycles.Communication

'                    End Select

'                    myStr = myStr.Substring(4).Trim

'                    Dim PosValue As String = myStr.Substring(0, 1)
'                    myResultError = New ISEErrorTO
'                    myResultError.IsCancelError = True
'                    myResultError.DigitNumber = 1
'                    myResultError.DigitValue = PosValue
'                    myResultError.Message = "ISE_REMARK" & PosValue 'Set the position value

'                    If MyClass.ISECancelErrorsHT.ContainsKey(PosValue) Then
'                        myResultError.Affected = MyClass.ISECancelErrorsHT(PosValue).ToString()
'                    End If

'                    myResultError.ErrorCycle = myErrorCycle

'                    myResultErrors.Add(myResultError)


'                End If


'                myGlobal.SetDatos = myResultErrors

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = "SYSTEM_ERROR"
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEDecodeDelegate.GetCancelError", EventLogEntryType.Error, False)
'            End Try
'            Return myGlobal
'        End Function

'#End Region

'#Region "ISE TEST Result Decoding"

'        ''' <summary>
'        ''' Method incharge to convert the recived ISE result into a ISE Result TO.
'        ''' </summary>
'        ''' <param name="pISEResult">Recived ISE Result</param>
'        ''' <param name="pDebugModeOn">ISE Module is in debug mode.</param>
'        ''' <returns>Return an ISE Result TO.</returns>
'        ''' <remarks>
'        ''' CREATE BY: TR 14/01/2011
'        ''' </remarks>
'        Public Function ConvertISETESTResultToISEResultTO(ByVal pISEResult As String, _
'                                                      ByVal pDebugModeOn As Boolean) As GlobalDataTO

'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                If pDebugModeOn Then
'                    myGlobalDataTO = DecodeComplexISETESTResult(pISEResult)
'                Else
'                    myGlobalDataTO = DecodeSimpleISETESTResult(pISEResult)
'                End If
'            Catch ex As Exception
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
'                myGlobalDataTO.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.ConvertISEResultToISEResultTO", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobalDataTO
'        End Function




'        '''' <summary>
'        '''' Decode Simple ISE result and set to ISEResultTO. DEBUG OFF
'        '''' </summary>
'        '''' <param name="pISEResult">Recived ISE Result</param>
'        '''' <returns>Returns an ISE Result TO</returns>
'        '''' <remarks>
'        '''' CREATE BY: TR 03/01/2010
'        '''' Modified by: TR 22/11/2011 - The recived string change, now no <> are recived.
'        '''' </remarks>
'        'Private Function DecodeSimpleISETESTResult_0(ByVal pISEResult As String) As GlobalDataTO
'        '    Dim myGlobalDataTO As New GlobalDataTO

'        '    Try
'        '        'Get the decimal separator.
'        '        Dim myDecimalSeparator As String = ""
'        '        Dim mySystemInfoManager As New SystemInfoManager
'        '        myDecimalSeparator = mySystemInfoManager.FillSystemInfoSessionTO().OSDecimalSeparator

'        '        Dim myISEResultTO As New ISEResultTO

'        '        'Get the ISE TYPE (Sample Type or CAL)
'        '        Dim myType As String = pISEResult.Substring(0, 3).ToString()


'        '        'AG 02/12/2011
'        '        Dim startChar As Integer = pISEResult.Length - 7 - 2
'        '        Dim endChar As Integer = startChar - 1
'        '        'AG 02/12/2011


'        '        'SGM 11/01/2012
'        '        Dim myCl As Single = 0
'        '        Dim myK As Single = 0
'        '        Dim myNa As Single = 0
'        '        Dim myLi As Single = 0

'        '        Select Case myType.ToUpper.Trim
'        '            Case "SER"
'        '                myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.SER
'        '                'SER Li xx.xx Na xxx.x K xx.xx Cl xxx.x eeeeeeec
'        '                'AG 02/12/2011 - Example ser response "SER Li 0.00 Na 141.0 K -0.01 Cl 3.1 000000D\" it is different from the write on the manual

'        '                ''Get the lithium value
'        '                'myISEResultTO.Li = CSng(pISEResult.Substring(7, 5).ToString().Replace(".", myDecimalSeparator))

'        '                ''Get the Sodium value
'        '                'myISEResultTO.Na = CSng(pISEResult.Substring(16, 5).ToString().Replace(".", myDecimalSeparator))

'        '                ''Get the potassium value
'        '                'myISEResultTO.K = CSng(pISEResult.Substring(24, 5).ToString().Replace(".", myDecimalSeparator))

'        '                ''Get the Chlorine value 
'        '                'myISEResultTO.Cl = CSng(pISEResult.Substring(33, 5).ToString().Replace(".", myDecimalSeparator))

'        '                ''Get the Remarks value 
'        '                'myISEResultTO.Remarks = pISEResult.Substring(39, 7).ToString()

'        '                'Get the Remarks value 
'        '                myISEResultTO.ResultErrorsString = pISEResult.Substring(startChar, 7)
'        '                endChar = startChar - 1

'        '                'Get the Chlorine value 
'        '                myCl = GetISEIoneValue("Cl", pISEResult, startChar, endChar)
'        '                'myISEResultTO.Cl = GetISEIoneValue("Cl", pISEResult, startChar, endChar)

'        '                'Get the potassium value
'        '                myK = GetISEIoneValue("K", pISEResult, startChar, endChar)
'        '                'myISEResultTO.K = GetISEIoneValue("K", pISEResult, startChar, endChar)

'        '                'Get the Sodium value
'        '                myNa = GetISEIoneValue("Na", pISEResult, startChar, endChar)
'        '                'myISEResultTO.Na = GetISEIoneValue("Na", pISEResult, startChar, endChar)

'        '                'Get the lithium value
'        '                myLi = GetISEIoneValue("Li", pISEResult, startChar, endChar)
'        '                'myISEResultTO.Li = GetISEIoneValue("Li", pISEResult, startChar, endChar)

'        '                myISEResultTO.ConcentrationValues = New ISEResultTO.LiNaKCl(myLi, myNa, myK, myCl)

'        '            Case "URN"
'        '                myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.URN
'        '                'URN Na xxxxx K xxxxx Cl xxxxx eeeeeeec urine structure
'        '                'AG 02/12/2011 - Example urn response "URN Na 674 K - 1 Cl 30 0804006B" it is different from the write on the manual
'        '                'NOTE the '-' is separated from the number "- 1"

'        '                ''Get the Sodium value
'        '                'myISEResultTO.Na = CSng(pISEResult.Substring(8, 5).ToString().Replace(".", myDecimalSeparator))

'        '                ''Get the potassium value
'        '                'myISEResultTO.K = CSng(pISEResult.Substring(15, 5).ToString().Replace(".", myDecimalSeparator))

'        '                ''Get the Chlorine value 
'        '                'myISEResultTO.Cl = CSng(pISEResult.Substring(24, 5).ToString().Replace(".", myDecimalSeparator))

'        '                ''Get the Remarks value 
'        '                'myISEResultTO.Remarks = pISEResult.Substring(30, 7).ToString()

'        '                'Get the Remarks value 
'        '                myISEResultTO.ResultErrorsString = pISEResult.Substring(startChar, 7)
'        '                endChar = startChar - 1

'        '                'Get the Chlorine value 
'        '                myCl = GetISEIoneValue("Cl", pISEResult, startChar, endChar)
'        '                'myISEResultTO.Cl = GetISEIoneValue("Cl", pISEResult, startChar, endChar)

'        '                'Get the potassium value
'        '                myK = GetISEIoneValue("K", pISEResult, startChar, endChar)
'        '                'myISEResultTO.K = GetISEIoneValue("K", pISEResult, startChar, endChar)

'        '                'Get the Sodium value
'        '                myNa = GetISEIoneValue("Na", pISEResult, startChar, endChar)
'        '                'myISEResultTO.Na = GetISEIoneValue("Na", pISEResult, startChar, endChar)
'        '                'AG 02/12/2011

'        '                myISEResultTO.ConcentrationValues = New ISEResultTO.LiNaKCl(myLi, myNa, myK, myCl)

'        '            Case "ERC"
'        '                myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ERC
'        '                'Example "ERC URN F000000"
'        '                'TODO
'        '                myISEResultTO.IsCancelError = True
'        '                myISEResultTO.ResultErrorsString = pISEResult.Substring(4, 11)


'        '                ''Get the Chlorine value 
'        '                'myISEResultTO.Cl = 0

'        '                ''Get the potassium value
'        '                'myISEResultTO.K = 0

'        '                ''Get the Sodium value
'        '                'myISEResultTO.Na = 0


'        '        End Select


'        '        myGlobalDataTO.SetDatos = myISEResultTO

'        '    Catch ex As Exception
'        '        myGlobalDataTO.HasError = True
'        '        myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
'        '        myGlobalDataTO.ErrorMessage = ex.Message

'        '        Dim myLogAcciones As New ApplicationLogManager()
'        '        myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.DecodeSimpleISETESTResult", EventLogEntryType.Error, False)
'        '    End Try

'        '    Return myGlobalDataTO
'        'End Function

'        ''' <summary>
'        ''' Decode Simple ISE result and set to ISEResultTO. DEBUG OFF
'        ''' </summary>
'        ''' <param name="pISEResult">Recived ISE Result</param>
'        ''' <returns>Returns an ISE Result TO</returns>
'        ''' <remarks>
'        ''' CREATE BY: TR 03/01/2010
'        ''' Modified by: TR 22/11/2011 - The recived string change, now no <> are recived.
'        ''' Modified by: SG 08/02/2012 - The received string does contain "<>". Prevent unexpected Debug mode
'        ''' </remarks>
'        Private Function DecodeSimpleISETESTResult(ByVal pISEResult As String) As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO

'            Try
'                'Get the decimal separator.
'                Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

'                Dim myISEResultTO As New ISEResultTO
'                Dim isDebugResult As Boolean = False
'                Dim myType As String = ""

'                'SGM 08/02/2012 prevent unexpected Debug Mode
'                isDebugResult = (pISEResult.Contains("<SMV ") Or pISEResult.Contains("<UMV "))
'                If isDebugResult Then
'                    'PDT FORZAR DEBUG OFF?
'                    If pISEResult.Contains("<SER ") Then
'                        pISEResult = pISEResult.Substring(pISEResult.IndexOf("<SER "))
'                    ElseIf pISEResult.Contains("<URN ") Then
'                        pISEResult = pISEResult.Substring(pISEResult.IndexOf("<URN "))
'                    Else
'                        'Sipping (not probable)
'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.SIP
'                    End If
'                End If


'                ''AG 02/12/2011
'                'Dim startChar As Integer
'                'Dim endChar As Integer
'                ''AG 02/12/2011

'                'type of response
'                myType = pISEResult.Substring(1, 3).ToString()


'                'end SGM 08/02/2012


'                'SGM 11/01/2012

'                Dim myConcentrationValues As ISEResultTO.LiNaKCl

'                Select Case myType.ToUpper.Trim
'                    Case "SER"
'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.SER
'                        '<SER Li xx.xx Na xxx.x K xx.xx Cl xxx.x eeeeeeec>
'                        'AG 02/12/2011 - Example ser response "<SER Li 0.00 Na 141.0 K -0.01 Cl 3.1 000000D\>" it is different from the write on the manual

'                        'Get the Remarks value 
'                        myISEResultTO.ResultErrorsString = pISEResult.Substring(pISEResult.Length - 9, 7)

'                        'Get the concentration values
'                        myGlobal = MyClass.GetLiNaKClValues(pISEResult)
'                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                            myConcentrationValues = CType(myGlobal.SetDatos, ISEResultTO.LiNaKCl)
'                        End If

'                        myISEResultTO.ConcentrationValues = myConcentrationValues



'                    Case "URN"
'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.URN
'                        '<URN Na xxxxx K xxxxx Cl xxxxx eeeeeeec> urine structure
'                        'AG 02/12/2011 - Example urn response "<URN Na 674 K - 1 Cl 30 0804006B>" it is different from the write on the manual
'                        'NOTE the '-' is separated from the number "- 1"

'                        'Get the Remarks value 
'                        myISEResultTO.ResultErrorsString = pISEResult.Substring(pISEResult.Length - 9, 7)

'                        'Get the concentration values
'                        myGlobal = MyClass.GetLiNaKClValues(pISEResult)
'                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                            myConcentrationValues = CType(myGlobal.SetDatos, ISEResultTO.LiNaKCl)
'                        End If

'                        myISEResultTO.ConcentrationValues = myConcentrationValues



'                    Case "ERC"
'                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ERC
'                        'Example "ERC URN F000000"
'                        'TODO
'                        myISEResultTO.IsCancelError = True
'                        myISEResultTO.ResultErrorsString = pISEResult.Substring(4, 11)


'                End Select


'                myGlobal.SetDatos = myISEResultTO

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = "SYSTEM_ERROR"
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.DecodeSimpleISETESTResult", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobal
'        End Function


'        ''' <summary>
'        ''' Decode the ISE result when the ise module has the Debug mode on
'        ''' </summary>
'        ''' <param name="pISEResult">Recived ISE Results</param>
'        ''' <returns></returns>
'        ''' <remarks>
'        ''' CREATE BY: TR 14/01/2011 ISE Result lines format. SER/URI
'        ''' </remarks>
'        Private Function DecodeComplexISETESTResult(ByVal pISEResult As String) As GlobalDataTO
'            Dim myGlobalDataTO As New GlobalDataTO
'            Try
'                Dim MyResults As String()
'                Dim myISEResultTO As New ISEResultTO
'                'Do and split by the character > because i can get all the importan info.
'                MyResults = pISEResult.Split(CChar(">"))
'                'ISE Result Line Format:
'                'SER -<SMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x><AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x><SER Li xx.xx Na xxx.x K xx.xx Cl xxx.x eeeeeeec>
'                'URINE- <UMV Na xxx.x K xxx.x Cl xxx.x><AMV Na xxx.x K xxx.x Cl xxx.x><URN Na xxxxx K xxxxx Cl xxxxx eeeeeeec> 
'                For Each iseLine As String In MyResults
'                    If iseLine.Length > 0 Then
'                        If iseLine.Substring(1, 3).ToString() = "SER" OrElse iseLine.Substring(1, 3).ToString() = "URN" Then
'                            'Call the Decode Simple function as soons as we haver the part of the recive result.
'                            myGlobalDataTO = DecodeSimpleISETESTResult(iseLine)
'                            If Not myGlobalDataTO.HasError Then
'                                'Get the ISETO to set the correct recive line.
'                                myISEResultTO = DirectCast(myGlobalDataTO.SetDatos, ISEResultTO)
'                                myISEResultTO.ReceivedResults = pISEResult
'                            Else
'                                Exit For
'                            End If
'                        End If
'                    End If
'                Next
'            Catch ex As Exception
'                myGlobalDataTO.HasError = True
'                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
'                myGlobalDataTO.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.DecodeComplexISETESTResult", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobalDataTO
'        End Function

'        ''' <summary>
'        ''' Returns the ise result by Ione
'        ''' NOTE: This method does not implements try catch (the method who calls it implements)
'        ''' </summary>
'        ''' <param name="pIoneName"></param>
'        ''' <param name="pISEResult"></param>
'        ''' <param name="pStartChar"></param>
'        ''' <param name="pEndChar"></param>
'        ''' <returns></returns>
'        ''' <remarks>AG 02/12/2011</remarks>
'        Private Function GetISEIoneValue(ByVal pIoneName As String, ByVal pISEResult As String, ByRef pStartChar As Integer, ByRef pEndChar As Integer) As Single
'            Dim fieldSize As Integer = 1
'            Dim signChar As Integer = 1
'            Dim returnValue As Single = -1

'            'Get the decimal separator.
'            Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

'            pStartChar = InStr(pISEResult, pIoneName, CompareMethod.Text)
'            If pStartChar <> 0 Then
'                pStartChar = pStartChar + 1
'                fieldSize = pEndChar - pStartChar

'                Dim newResult As String = pISEResult
'                'Remove '-' due sometimes there is a space between sign and value and the the IsNumeric, Csng methods do not work properly
'                signChar = InStr(pISEResult.Substring(pStartChar, fieldSize), "-", CompareMethod.Text)
'                If signChar > 0 Then
'                    newResult = pISEResult.Substring(pStartChar, fieldSize).ToString().Replace("-", " ")
'                    signChar = -1
'                Else
'                    newResult = pISEResult.Substring(pStartChar, fieldSize).ToString()
'                    signChar = 1
'                End If

'                If IsNumeric(Trim(newResult.ToString().Replace(".", myDecimalSeparator))) Then
'                    returnValue = signChar * CSng(newResult.ToString().Replace(".", myDecimalSeparator))
'                End If
'                pEndChar = pStartChar - 2
'            End If

'            Return returnValue

'        End Function

'        ''' <summary>
'        ''' Gets a structured data with values for each electrode
'        ''' </summary>
'        ''' <param name="pISEResult"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 09/02/2012</remarks>
'        Private Function GetLiNaKClValues(ByVal pISEResult As String) As GlobalDataTO

'            Dim myGlobal As New GlobalDataTO

'            Try
'                Dim mySign As Integer = 1
'                Dim myLiNaKClValues As New ISEResultTO.LiNaKCl(-1, -1, -1, -1)

'                If pISEResult.Length > 0 Then

'                    Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

'                    Dim myIoneValuesStr As String

'                    myIoneValuesStr = pISEResult.Substring(1, pISEResult.Length - 2).Trim()


'                    'it does not bring result errors
'                    If Not myIoneValuesStr.Contains(GlobalEnumerates.ISE_Electrodes.Cl.ToString & " ") Then
'                        myIoneValuesStr = pISEResult.Substring(1, pISEResult.Length - 2).Trim()
'                    End If

'                    Dim myLiPos As Integer = myIoneValuesStr.IndexOf(GlobalEnumerates.ISE_Electrodes.Li.ToString & " ")
'                    Dim myNaPos As Integer = myIoneValuesStr.IndexOf(GlobalEnumerates.ISE_Electrodes.Na.ToString & " ")
'                    Dim myKPos As Integer = myIoneValuesStr.IndexOf(GlobalEnumerates.ISE_Electrodes.K.ToString & " ")
'                    Dim myClPos As Integer = myIoneValuesStr.IndexOf(GlobalEnumerates.ISE_Electrodes.Cl.ToString & " ")
'                    Dim myErrPos As Integer = myIoneValuesStr.LastIndexOf(" ")

'                    'Lithium can be unused (urine tests)
'                    If myLiPos >= 0 Then
'                        Dim myLiStr As String = myIoneValuesStr.Substring(myLiPos, myNaPos - myLiPos).Trim
'                        Dim myLiValueStr As String = myLiStr.Substring(GlobalEnumerates.ISE_Electrodes.Li.ToString.Length + 1).Trim

'                        With myLiNaKClValues
'                            If myLiValueStr.Contains("-") Then
'                                myLiValueStr = myLiValueStr.Replace("-", "").Trim : mySign = -1
'                            Else
'                                mySign = 1
'                            End If
'                            If IsNumeric(Trim(myLiValueStr.Replace(".", myDecimalSeparator))) Then
'                                .Li = mySign * CSng(myLiValueStr.Replace(".", myDecimalSeparator))
'                            End If
'                        End With
'                    End If

'                    'Sodium
'                    If myNaPos >= 0 Then
'                        Dim myNaStr As String = myIoneValuesStr.Substring(myNaPos, myKPos - myNaPos).Trim
'                        Dim myNaValueStr As String = myNaStr.Substring(GlobalEnumerates.ISE_Electrodes.Na.ToString.Length + 1).Trim

'                        With myLiNaKClValues
'                            If myNaValueStr.Contains("-") Then
'                                myNaValueStr = myNaValueStr.Replace("-", "").Trim : mySign = -1
'                            Else
'                                mySign = 1
'                            End If
'                            If IsNumeric(Trim(myNaValueStr.Replace(".", myDecimalSeparator))) Then
'                                .Na = mySign * CSng(myNaValueStr.Replace(".", myDecimalSeparator))
'                            End If
'                        End With
'                    End If

'                    'Potassium 
'                    If myKPos >= 0 Then
'                        Dim myKStr As String = myIoneValuesStr.Substring(myKPos, myClPos - myKPos).Trim
'                        Dim myKValueStr As String = myKStr.Substring(GlobalEnumerates.ISE_Electrodes.K.ToString.Length + 1).Trim

'                        With myLiNaKClValues
'                            If myKValueStr.Contains("-") Then
'                                myKValueStr = myKValueStr.Replace("-", "").Trim : mySign = -1
'                            Else
'                                mySign = 1
'                            End If
'                            If IsNumeric(Trim(myKValueStr.Replace(".", myDecimalSeparator))) Then
'                                .K = mySign * CSng(myKValueStr.Replace(".", myDecimalSeparator))
'                            End If
'                        End With
'                    End If

'                    'Chlorine
'                    If myClPos >= 0 Then
'                        Dim myClStr As String = myIoneValuesStr.Substring(myClPos).Trim
'                        If myErrPos > myClPos + 3 Then
'                            myClStr = myIoneValuesStr.Substring(myClPos, myErrPos - myClPos).Trim
'                        End If
'                        Dim myClValueStr As String = myClStr.Substring(GlobalEnumerates.ISE_Electrodes.Cl.ToString.Length + 1).Trim


'                        With myLiNaKClValues
'                            If myClValueStr.Contains("-") Then
'                                myClValueStr = myClValueStr.Replace("-", "").Trim : mySign = -1
'                            Else
'                                mySign = 1
'                            End If
'                            If IsNumeric(Trim(myClValueStr.Replace(".", myDecimalSeparator))) Then
'                                .Cl = mySign * CSng(myClValueStr.Replace(".", myDecimalSeparator))
'                            End If
'                        End With
'                    End If

'                End If

'                myGlobal.SetDatos = myLiNaKClValues



'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = "SYSTEM_ERROR"
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEDecodeDelegate.GetLiNaKClValues", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobal

'        End Function

'        ''' <summary>
'        ''' Gets a structured data with values for each peristaltic pump after calibration
'        ''' </summary>
'        ''' <param name="pISEResult"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 09/02/2012</remarks>
'        Private Function GetPumpCalibrationValues(ByVal pISEResult As String) As GlobalDataTO

'            Dim myGlobal As New GlobalDataTO

'            Try
'                Dim mySign As Integer = 1
'                Dim myPumpValues As New ISEResultTO.PumpCalibrationValues(-1, -1, -1)

'                If pISEResult.Length > 0 Then

'                    Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

'                    Dim myPumpValuesStr As String = pISEResult.Substring(1, pISEResult.Length - 2).Trim

'                    Dim myPumpAPos As Integer = myPumpValuesStr.IndexOf(GlobalEnumerates.ISE_Pumps.A.ToString & " ")
'                    Dim myPumpBPos As Integer = myPumpValuesStr.IndexOf(GlobalEnumerates.ISE_Pumps.B.ToString & " ")
'                    Dim myPumpWPos As Integer = myPumpValuesStr.IndexOf(GlobalEnumerates.ISE_Pumps.W.ToString & " ")


'                    If myPumpAPos >= 0 And myPumpBPos >= 0 And myPumpWPos >= 0 Then
'                        Dim myPumpAStr As String = myPumpValuesStr.Substring(myPumpAPos, myPumpBPos - myPumpAPos).Trim
'                        Dim myPumpAValueStr As String = myPumpAStr.Substring(GlobalEnumerates.ISE_Pumps.A.ToString.Length + 1).Trim
'                        Dim myPumpBStr As String = myPumpValuesStr.Substring(myPumpBPos, myPumpWPos - myPumpBPos).Trim
'                        Dim myPumpBValueStr As String = myPumpBStr.Substring(GlobalEnumerates.ISE_Pumps.B.ToString.Length + 1).Trim
'                        Dim myPumpWStr As String = myPumpValuesStr.Substring(myPumpWPos).Trim
'                        Dim myPumpWValueStr As String = myPumpWStr.Substring(GlobalEnumerates.ISE_Pumps.W.ToString.Length + 1).Trim

'                        With myPumpValues
'                            If myPumpAStr.Contains("-") Then
'                                myPumpAValueStr = myPumpAValueStr.Replace("-", "").Trim : mySign = -1
'                            Else
'                                mySign = 1
'                            End If
'                            If IsNumeric(Trim(myPumpAValueStr.Replace(".", myDecimalSeparator))) Then
'                                .PumpA = mySign * CSng(myPumpAValueStr.Replace(".", myDecimalSeparator))
'                            End If

'                            If myPumpBValueStr.Contains("-") Then
'                                myPumpBValueStr = myPumpBValueStr.Replace("-", "").Trim : mySign = -1
'                            Else
'                                mySign = 1
'                            End If
'                            If IsNumeric(Trim(myPumpBValueStr.Replace(".", myDecimalSeparator))) Then
'                                .PumpB = mySign * CSng(myPumpBValueStr.Replace(".", myDecimalSeparator))
'                            End If

'                            If myPumpWValueStr.Contains("-") Then
'                                myPumpWValueStr = myPumpWValueStr.Replace("-", "").Trim : mySign = -1
'                            Else
'                                mySign = 1
'                            End If
'                            If IsNumeric(Trim(myPumpWValueStr.Replace(".", myDecimalSeparator))) Then
'                                .PumpW = mySign * CSng(myPumpWValueStr.Replace(".", myDecimalSeparator))
'                            End If
'                        End With
'                    End If

'                End If

'                myGlobal.SetDatos = myPumpValues



'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = "SYSTEM_ERROR"
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEDecodeDelegate.GetPumpCalibrationValues", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobal

'        End Function

'        ''' <summary>
'        ''' Gets a structured data with values after calibration of bubble detector
'        ''' </summary>
'        ''' <param name="pISEResult"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 09/02/2012</remarks>
'        Private Function GetBubbleDetectorCalibrationValues(ByVal pISEResult As String) As GlobalDataTO

'            Dim myGlobal As New GlobalDataTO

'            Try
'                Dim mySign As Integer = 1
'                Dim myBubbleCalibValues As New ISEResultTO.BubbleCalibrationValues(-1, -1, -1)

'                If pISEResult.Length > 0 Then

'                    Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

'                    Dim myBubbleValuesStr As String = pISEResult.Substring(1, pISEResult.Length - 2).Trim

'                    Dim myBubbleAPos As Integer = myBubbleValuesStr.IndexOf(GlobalEnumerates.ISE_Bubble_Detector.A.ToString & " ")
'                    Dim myBubbleMPos As Integer = myBubbleValuesStr.IndexOf(GlobalEnumerates.ISE_Bubble_Detector.M.ToString & " ")
'                    Dim myBubbleLPos As Integer = myBubbleValuesStr.IndexOf(GlobalEnumerates.ISE_Bubble_Detector.L.ToString & " ")


'                    If myBubbleAPos >= 0 And myBubbleMPos >= 0 And myBubbleLPos >= 0 Then
'                        Dim myBubbleAStr As String = myBubbleValuesStr.Substring(myBubbleAPos, myBubbleMPos - myBubbleAPos).Trim
'                        Dim myBubbleAValueStr As String = myBubbleAStr.Substring(GlobalEnumerates.ISE_Bubble_Detector.A.ToString.Length + 1).Trim
'                        Dim myBubbleMStr As String = myBubbleValuesStr.Substring(myBubbleMPos, myBubbleLPos - myBubbleMPos).Trim
'                        Dim myBubbleMValueStr As String = myBubbleMStr.Substring(GlobalEnumerates.ISE_Bubble_Detector.M.ToString.Length + 1).Trim
'                        Dim myBubbleLStr As String = myBubbleValuesStr.Substring(myBubbleLPos).Trim
'                        Dim myBubbleLValueStr As String = myBubbleLStr.Substring(GlobalEnumerates.ISE_Bubble_Detector.L.ToString.Length + 1).Trim

'                        With myBubbleCalibValues
'                            If myBubbleAValueStr.Contains("-") Then
'                                myBubbleAValueStr = myBubbleAValueStr.Replace("-", "").Trim : mySign = -1
'                            Else
'                                mySign = 1
'                            End If
'                            If IsNumeric(Trim(myBubbleAValueStr.Replace(".", myDecimalSeparator))) Then
'                                .ValueA = mySign * CSng(myBubbleAValueStr.Replace(".", myDecimalSeparator))
'                            End If

'                            If myBubbleMValueStr.Contains("-") Then
'                                myBubbleMValueStr = myBubbleMValueStr.Replace("-", "").Trim : mySign = -1
'                            Else
'                                mySign = 1
'                            End If
'                            If IsNumeric(Trim(myBubbleMValueStr.Replace(".", myDecimalSeparator))) Then
'                                .ValueM = mySign * CSng(myBubbleMValueStr.Replace(".", myDecimalSeparator))
'                            End If

'                            If myBubbleLValueStr.Contains("-") Then
'                                myBubbleLValueStr = myBubbleLValueStr.Replace("-", "").Trim : mySign = -1
'                            Else
'                                mySign = 1
'                            End If
'                            If IsNumeric(Trim(myBubbleLValueStr.Replace(".", myDecimalSeparator))) Then
'                                .ValueL = mySign * CSng(myBubbleLValueStr.Replace(".", myDecimalSeparator))
'                            End If
'                        End With
'                    End If

'                End If

'                myGlobal.SetDatos = myBubbleCalibValues



'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = "SYSTEM_ERROR"
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEDecodeDelegate.GetBubbleDetectorCalibrationValues", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobal

'        End Function

'#End Region

'#Region "ISE CMD result Decoding"

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="pISEResultStr"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 11/01/2012</remarks>
'        Public Function FillISEResultValues(ByRef pISEResultTO As ISEResultTO, ByVal pResultItem As ISEResultTO.ISEResultItemTypes, ByVal pISEValuesStr As String) As GlobalDataTO
'            Dim myGlobal As New GlobalDataTO

'            Try
'                'Get the decimal separator.
'                Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

'                pISEValuesStr = pISEValuesStr.Trim

'                Dim myLiNaKClValues As ISEResultTO.LiNaKCl
'                Dim myPumpCalibrationValues As ISEResultTO.PumpCalibrationValues
'                Dim myBubbleCalibrationValues As ISEResultTO.BubbleCalibrationValues

'                Dim myDallasSN As ISEDallasSNTO
'                Dim myDallas00 As ISEDallasPage00TO
'                Dim myDallas01 As ISEDallasPage01TO
'                Dim myResultErrors As New List(Of ISEErrorTO)

'                If pResultItem = ISEResultTO.ISEResultItemTypes.CancelError Then
'                    Dim myErrorStr As String = pISEValuesStr.Trim.Substring(5, pISEValuesStr.Length - 2 - 5)
'                    myGlobal = MyClass.GetCancelError(myErrorStr)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        myResultErrors = CType(myGlobal.SetDatos, List(Of ISEErrorTO))
'                        pISEResultTO.ResultErrorsString = myErrorStr
'                    End If


'                ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.PumpsCalibration Then
'                    myGlobal = MyClass.GetPumpCalibrationValues(pISEValuesStr)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        myPumpCalibrationValues = CType(myGlobal.SetDatos, ISEResultTO.PumpCalibrationValues)
'                    End If


'                ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.BubbleCalibration Then
'                    myGlobal = MyClass.GetBubbleDetectorCalibrationValues(pISEValuesStr)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        myBubbleCalibrationValues = CType(myGlobal.SetDatos, ISEResultTO.BubbleCalibrationValues)
'                    End If


'                ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.SerumConcentration Or _
'                    pResultItem = ISEResultTO.ISEResultItemTypes.UrineConcentration Or _
'                    pResultItem = ISEResultTO.ISEResultItemTypes.Calibration1 Or _
'                    pResultItem = ISEResultTO.ISEResultItemTypes.Calibration2 Then

'                    myGlobal = MyClass.GetLiNaKClValues(pISEValuesStr)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        myLiNaKClValues = CType(myGlobal.SetDatos, ISEResultTO.LiNaKCl)
'                    End If


'                    Dim myErrorIndex As Integer = pISEValuesStr.IndexOf("Cl") + 9
'                    If pISEValuesStr.Length > 45 Then
'                        Dim myErrorStr As String = pISEValuesStr.Substring(myErrorIndex, 7)
'                        myGlobal = MyClass.GetResultErrors(myErrorStr)
'                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                            myResultErrors = CType(myGlobal.SetDatos, List(Of ISEErrorTO))
'                            pISEResultTO.ResultErrorsString = myErrorStr
'                        End If
'                    End If

'                    ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.CalAMilivolts Or _
'                       pResultItem = ISEResultTO.ISEResultItemTypes.CalBMilivolts Or _
'                       pResultItem = ISEResultTO.ISEResultItemTypes.SerumMilivolts Or _
'                       pResultItem = ISEResultTO.ISEResultItemTypes.UrineMilivolts Then

'                    myGlobal = MyClass.GetLiNaKClValues(pISEValuesStr)
'                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                            myLiNaKClValues = CType(myGlobal.SetDatos, ISEResultTO.LiNaKCl)
'                        End If


'                    ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.Dallas_SN Then

'                        'serial number
'                        myGlobal = MyClass.GetDallasSNValues(pISEValuesStr)
'                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                            myDallasSN = CType(myGlobal.SetDatos, ISEDallasSNTO)
'                        End If

'                    ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.Dallas_Page0 Then

'                        'page 00
'                        myGlobal = MyClass.GetDallasPage00Values(pISEValuesStr)
'                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                            myDallas00 = CType(myGlobal.SetDatos, ISEDallasPage00TO)
'                        End If

'                    ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.Dallas_Page1 Then

'                        'page 01
'                        myGlobal = MyClass.GetDallasPage01Values(pISEValuesStr)
'                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                            myDallas01 = CType(myGlobal.SetDatos, ISEDallasPage01TO)
'                        End If

'                    End If

'                    Select Case pResultItem
'                        Case ISEResultTO.ISEResultItemTypes.SerumConcentration, ISEResultTO.ISEResultItemTypes.UrineConcentration
'                            pISEResultTO.ConcentrationValues = myLiNaKClValues

'                        Case ISEResultTO.ISEResultItemTypes.SerumMilivolts
'                            pISEResultTO.SerumMilivolts = myLiNaKClValues

'                        Case ISEResultTO.ISEResultItemTypes.UrineMilivolts
'                            pISEResultTO.UrineMilivolts = myLiNaKClValues

'                        Case ISEResultTO.ISEResultItemTypes.CalAMilivolts
'                            pISEResultTO.CalibratorAMilivolts = myLiNaKClValues

'                        Case ISEResultTO.ISEResultItemTypes.CalBMilivolts
'                            pISEResultTO.CalibratorBMilivolts = myLiNaKClValues

'                        Case ISEResultTO.ISEResultItemTypes.Calibration1
'                            pISEResultTO.CalibrationResults1 = myLiNaKClValues

'                        Case ISEResultTO.ISEResultItemTypes.Calibration2
'                            pISEResultTO.CalibrationResults2 = myLiNaKClValues

'                        Case ISEResultTO.ISEResultItemTypes.PumpsCalibration
'                            pISEResultTO.PumpsCalibrationValues = myPumpCalibrationValues

'                        Case ISEResultTO.ISEResultItemTypes.BubbleCalibration
'                            pISEResultTO.BubbleDetCalibrationValues = myBubbleCalibrationValues

'                        Case ISEResultTO.ISEResultItemTypes.Dallas_SN
'                            If myDallasSN IsNot Nothing Then pISEResultTO.DallasSNData = myDallasSN

'                        Case ISEResultTO.ISEResultItemTypes.Dallas_Page0
'                            If myDallas00 IsNot Nothing Then pISEResultTO.DallasPage00Data = myDallas00

'                        Case ISEResultTO.ISEResultItemTypes.Dallas_Page1
'                            If myDallas01 IsNot Nothing Then pISEResultTO.DallasPage01Data = myDallas01

'                    End Select

'                    If myResultErrors IsNot Nothing Then pISEResultTO.ResultErrors = myResultErrors

'                    myGlobal.SetDatos = pISEResultTO

'            Catch ex As Exception
'                myGlobal.HasError = True
'                myGlobal.ErrorCode = "SYSTEM_ERROR"
'                myGlobal.ErrorMessage = ex.Message

'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.FillISEResultValues", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobal
'        End Function

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="pIoneName"></param>
'        ''' <param name="pISEResultStr"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 11/01/2012</remarks>
'        Private Function GetIoneValue(ByVal pIoneName As String, ByVal pDataStr As String) As Single

'            Dim fieldSize As Integer = 1
'            Dim returnValue As Single = -1
'            Dim startIndex As Integer = -1
'            Dim myIoneStr As String

'            Try
'                'Get the decimal separator.
'                Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

'                'get the Ione's string
'                startIndex = pDataStr.IndexOf(pIoneName.Trim) + pIoneName.Trim.Length + 1
'                myIoneStr = pDataStr.Trim.Substring(startIndex, 5)

'                If myIoneStr.Trim.Length > 0 Then

'                    returnValue = CSng(myIoneStr.ToString().Replace(".", myDecimalSeparator))

'                End If
'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.GetIoneValue", EventLogEntryType.Error, False)
'            End Try

'            Return returnValue

'        End Function

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="pPumpName"></param>
'        ''' <param name="pDataStr"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 16/01/2012</remarks>
'        Private Function GetPumpValue(ByVal pPumpName As String, ByVal pDataStr As String) As Single

'            Dim fieldSize As Integer = 1
'            Dim signChar As Integer = 1
'            Dim returnValue As Single = -1
'            Dim startIndex As Integer = -1
'            Dim myPumpStr As String

'            Try
'                'Get the decimal separator.
'                Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

'                'get the Pump's string
'                startIndex = pDataStr.IndexOf(pPumpName.Trim) + pPumpName.Trim.Length + 1
'                myPumpStr = pDataStr.Trim.Substring(startIndex, 4)

'                If myPumpStr.Trim.Length > 0 Then
'                    If myPumpStr.Trim.Contains("-") Then
'                        signChar = -1
'                        'Remove '-' due sometimes there is a space between sign and value and the the IsNumeric, Csng methods do not work properly
'                        myPumpStr.Replace("-", "")
'                    End If

'                    If IsNumeric(Trim(myPumpStr.ToString().Replace(".", myDecimalSeparator))) Then
'                        returnValue = signChar * CSng(myPumpStr.ToString().Replace(".", myDecimalSeparator))
'                    End If
'                End If
'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.GetPumpValue", EventLogEntryType.Error, False)
'            End Try

'            Return returnValue

'        End Function

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="pBubbleItem"></param>
'        ''' <param name="pDataStr"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 16/01/2012</remarks>
'        Private Function GetBubbleValue(ByVal pBubbleItem As String, ByVal pDataStr As String) As Single

'            Dim fieldSize As Integer = 1
'            Dim returnValue As Single = -1
'            Dim startIndex As Integer = -1
'            Dim myBubbleStr As String

'            Try
'                'get the Bubble detector's string
'                startIndex = pDataStr.IndexOf(pBubbleItem.Trim) + pBubbleItem.Trim.Length + 1
'                myBubbleStr = pDataStr.Trim.Substring(startIndex, 3)

'                If myBubbleStr.Trim.Length > 0 Then
'                    If IsNumeric(Trim(myBubbleStr.ToString())) Then
'                        returnValue = CSng(myBubbleStr.ToString())
'                    End If
'                End If
'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.GetBubbleValue", EventLogEntryType.Error, False)
'            End Try

'            Return returnValue

'        End Function

'        'PDT ver si hace falta conversión a numérico
'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="pDataStr"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 16/01/2012</remarks>
'        Private Function GetDallasSNValues(ByVal pDataStr As String) As GlobalDataTO

'            Dim myGlobal As New GlobalDataTO
'            Dim myUtil As New Utilities

'            Try
'                Dim myDallasSNData As New ISEDallasSNTO

'                Dim myFamilyCodeHex As String = pDataStr.Trim.Substring(5, 2)
'                Dim mySerialNumberHex As String = pDataStr.Trim.Substring(7, 12)
'                Dim myCRCHex As String = pDataStr.Trim.Substring(19, 2)

'                With myDallasSNData
'                    .SNDataString = pDataStr.Trim

'                    'Family Code
'                    myGlobal = myUtil.ConvertHexToString(myFamilyCodeHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .FamilyCode = CStr(myGlobal.SetDatos)
'                    End If

'                    'Serial Number
'                    myGlobal = myUtil.ConvertHexToString(mySerialNumberHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .SerialNumber = CStr(myGlobal.SetDatos)
'                    End If

'                    'CRC Tester
'                    myGlobal = myUtil.ConvertHexToString(myCRCHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .CRC = CStr(myGlobal.SetDatos)
'                    End If
'                End With

'                myGlobal.SetDatos = myDallasSNData

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.GetDallasSNValues", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobal

'        End Function

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="pDataStr"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 16/01/2012</remarks>
'        Private Function GetDallasPage00Values(ByVal pDataStr As String) As GlobalDataTO

'            Dim myGlobal As New GlobalDataTO
'            Dim myUtil As New Utilities

'            Try
'                Dim myDallas00Data As New ISEDallasPage00TO

'                Dim myLotNumberHex As String = pDataStr.Trim.Substring(8, 10)
'                Dim myExpirationDayHex As String = pDataStr.Trim.Substring(18, 2)
'                Dim myExpirationMonthHex As String = pDataStr.Trim.Substring(20, 2)
'                Dim myExpirationYearHex As String = pDataStr.Trim.Substring(22, 2)
'                Dim myInitialCalibAVolumeHex As String = pDataStr.Trim.Substring(44, 2)
'                Dim myInitialCalibBVolumeHex As String = pDataStr.Trim.Substring(68, 2)
'                Dim myDistributorCodeHex As String = pDataStr.Trim.Substring(46, 2)
'                Dim mySecurityCodeHex As String = pDataStr.Trim.Substring(48, 8)
'                Dim myCRCHex As String = pDataStr.Trim.Substring(70, 2)

'                With myDallas00Data
'                    .Page00DataString = pDataStr.Trim

'                    'LotNumber
'                    myGlobal = myUtil.ConvertHexToUInt32(myLotNumberHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .LotNumber = CInt(myGlobal.SetDatos).ToString
'                    End If

'                    'ExpirationDay
'                    myGlobal = myUtil.ConvertHexToUInt32(myExpirationDayHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .ExpirationDay = CInt(myGlobal.SetDatos)
'                    End If

'                    'ExpirationMonth
'                    myGlobal = myUtil.ConvertHexToUInt32(myExpirationMonthHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .ExpirationMonth = CInt(myGlobal.SetDatos)
'                    End If

'                    'ExpirationYear
'                    myGlobal = myUtil.ConvertHexToUInt32(myExpirationYearHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .ExpirationYear = 2000 + CInt(myGlobal.SetDatos)
'                    End If

'                    'InitialCalibAVolume (mililitres)
'                    myGlobal = myUtil.ConvertHexToUInt32(myInitialCalibAVolumeHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .InitialCalibAVolume = 10 * CInt(myGlobal.SetDatos)
'                    End If

'                    'InitialCalibBVolume (mililitres)
'                    myGlobal = myUtil.ConvertHexToUInt32(myInitialCalibBVolumeHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .InitialCalibBVolume = 10 * CInt(myGlobal.SetDatos)
'                    End If

'                    'DistributorCode 'PDT???
'                    .DistributorCode = myDistributorCodeHex

'                    'SecurityCode 'PDT ????
'                    .SecurityCode = mySecurityCodeHex

'                    'CRC
'                    .CRC = myCRCHex


'                End With

'                myGlobal.SetDatos = myDallas00Data

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.GetDallasPage00Values", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobal

'        End Function

'        ''' <summary>
'        ''' 
'        ''' </summary>
'        ''' <param name="pDataStr"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 16/01/2012</remarks>
'        Private Function GetDallasPage01Values(ByVal pDataStr As String) As GlobalDataTO

'            Dim myGlobal As New GlobalDataTO
'            Dim myUtil As New Utilities

'            Try
'                Dim myDallas01Data As New ISEDallasPage01TO

'                Dim myConsumptionCalAHex As String = pDataStr.Trim.Substring(8, 26)
'                Dim myInstallationDayHex As String = pDataStr.Trim.Substring(34, 2)
'                Dim myInstallationMonthHex As String = pDataStr.Trim.Substring(36, 2)
'                Dim myInstallationYearHex As String = pDataStr.Trim.Substring(38, 2)
'                Dim myConsumptionCalBHex As String = pDataStr.Trim.Substring(40, 26)
'                Dim myNoGoodByteHex As String = pDataStr.Trim.Substring(66, 2)


'                With myDallas01Data
'                    .Page01DataString = pDataStr.Trim

'                    'InstallationDay
'                    myGlobal = myUtil.ConvertHexToUInt32(myInstallationDayHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .InstallationDay = CInt(myGlobal.SetDatos)
'                    End If

'                    'InstallationMonth
'                    myGlobal = myUtil.ConvertHexToUInt32(myInstallationMonthHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .InstallationMonth = CInt(myGlobal.SetDatos)
'                    End If

'                    'InstallationYear
'                    myGlobal = myUtil.ConvertHexToUInt32(myInstallationYearHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .InstallationYear = 2000 + CInt(myGlobal.SetDatos)
'                    End If

'                    'ConsumptionCalA (%)
'                    myGlobal = MyClass.GetConsumptionVolume(myConsumptionCalAHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .ConsumptionCalA = CInt(myGlobal.SetDatos)
'                    End If

'                    'ConsumptionCalB (%)
'                    myGlobal = MyClass.GetConsumptionVolume(myConsumptionCalBHex)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        .ConsumptionCalB = CInt(myGlobal.SetDatos)
'                    End If

'                    'No goodByte
'                    .NoGoodByte = myNoGoodByteHex

'                End With

'                myGlobal.SetDatos = myDallas01Data

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.GetDallasPage01Values", EventLogEntryType.Error, False)
'            End Try

'            Return myGlobal

'        End Function

'        ''' <summary>
'        ''' Gets the consumption value from the hex string (assuming that 1 bit equals to 1%)
'        ''' </summary>
'        ''' <param name="pHexString"></param>
'        ''' <returns></returns>
'        ''' <remarks>Created by SGM 16/01/2012</remarks>
'        Private Function GetConsumptionVolume(ByVal pHexString As String) As GlobalDataTO

'            Dim myGlobal As New GlobalDataTO

'            Try
'                Dim myConsumption As Integer
'                Dim myUtil As New Utilities

'                'each byte (2 char) represents 8%, one per bit
'                Dim myBytes As New List(Of String)
'                For c As Integer = 0 To pHexString.Length - 1 Step 2
'                    myBytes.Add(pHexString.Substring(c, 2)) 'it must be 13 length
'                Next
'                For Each B As String In myBytes
'                    Dim myBinaryString As String
'                    myGlobal = myUtil.ConvertHexToBinaryString(B)
'                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
'                        myBinaryString = CStr(myGlobal.SetDatos)
'                        Dim myHighWord As String = myBinaryString.Substring(0, 4)
'                        Dim myLowWord As String = myBinaryString.Substring(4, 4)
'                        For w As Integer = 0 To myLowWord.Length - 1
'                            Dim myBit As String = myLowWord.Substring(w, 1)
'                            If myBit = "0" Then
'                                myConsumption += 1
'                            End If
'                        Next
'                        For w As Integer = 0 To myHighWord.Length - 1
'                            Dim myBit As String = myHighWord.Substring(w, 1)
'                            If myBit = "0" Then
'                                myConsumption += 1
'                            End If
'                        Next
'                    Else
'                        Exit For
'                    End If
'                Next


'                myGlobal.SetDatos = myConsumption

'            Catch ex As Exception
'                Dim myLogAcciones As New ApplicationLogManager()
'                myLogAcciones.CreateLogActivity(ex.Message, "ISEResultDecode.GetConsumptionVolume", EventLogEntryType.Error, False)
'            End Try
'            Return myGlobal
'        End Function
'#End Region




'    End Class

'End Namespace