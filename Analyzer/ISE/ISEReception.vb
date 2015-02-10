Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports System.Xml
Imports System.Data
Imports System.Data.SqlClient
Imports System.Windows.Forms
'Imports System.Configuration
Imports Biosystems.Ax00.Calculations
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Public Class ISEReception

#Region "Declarations"

        Private ReadOnly ISEResultErrorsHT As New Hashtable()
        Private ReadOnly ISECancelErrorsHT As New Hashtable()
        Private ReadOnly ISEResultErrorDescriptionsHT As New Hashtable()

        Private ReadOnly myAffectedElementsHT As New Hashtable()
        Private ReadOnly myISEModuleErrorHT As New Hashtable()
        Private ReadOnly myResultErrorDescHT As New Hashtable() 'SGM 20/07/2012

        Private myAnalyzer As IAnalyzerManager 'SGM 23/07/2012 '#REFACTORING
        'Private myISEManager As IISEAnalyzerEntity 'SGM 23/07/2012 '#REFACTORING

#End Region

#Region "Constructor"

        Public Sub New(ByVal pAnalyzer As IAnalyzerManager) '#REFACTORING
            MyClass.LoadISEErrorsDataHT()
            MyClass.FillAffectedElementHT()
            MyClass.FillISECancelErrorDescHT()

            MyClass.myAnalyzer = pAnalyzer
            'MyClass.myISEManager = MyClass.myAnalyzer.ISEAnalyzer '#REFACTORING
        End Sub
#End Region

#Region "Properties"

#End Region

#Region "common"

        ''' <summary>
        ''' LOads all the master data related to possible result and cancel errors (from XML files)
        ''' </summary>
        ''' <remarks>Created by SGM 29/07/2012</remarks>
        Private Sub LoadISEErrorsDataHT()
            Try
                Dim myISEParamXmlDS As New DataSet

                Dim myISEParamXml As New XmlDocument
                Dim myXmlPath As String = Application.StartupPath.ToString() & GlobalBase.ISEParammetersFilePath
                myISEParamXml.Load(myXmlPath)
                myISEParamXmlDS.ReadXml(myXmlPath)

                For Each iseRow As DataRow In myISEParamXmlDS.Tables("ResultErrorDescTable").Rows
                    ISEResultErrorDescriptionsHT.Add(iseRow("digit").ToString(), iseRow("ResultErrorDesc").ToString())
                Next
                For Each iseRow As DataRow In myISEParamXmlDS.Tables("AffectedElementTable").Rows
                    ISEResultErrorsHT.Add(iseRow("id").ToString(), iseRow("AffectedElements").ToString())
                Next
                For Each iseRow As DataRow In myISEParamXmlDS.Tables("CancelErrorDescTable").Rows
                    ISECancelErrorsHT.Add(iseRow("code").ToString(), iseRow("CancelErrorDesc").ToString())
                Next


            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.LoadISEErrorsDataHT", EventLogEntryType.Error, False)
            End Try

        End Sub

        ''' <summary>
        ''' </summary>
        ''' <param name="pISEResult"></param>
        ''' <param name="pForcedLiEnabledValue">The value to force in LiEnabled checks in the validation (Optional)
        ''' Value TriState.UseDefault: The LiEnabled value is the current LiEnabled value (saved in DB)
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY:  SGM 15/01/2012
        ''' MODIFIED BY: JBL 20/09/2012 - Added parameter pForcedLiEnabledValue set Public Function
        ''' </remarks>
        Public Function GetResultErrors(ByVal pISEResult As ISEResultTO, Optional ByVal pForcedLiEnabledValue As TriState = TriState.UseDefault) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Try

                Dim myResultErrors As New List(Of ISEErrorTO)

                Dim myResultError As ISEErrorTO

                'get error string
                'SGM 23/07/2012
                If pISEResult.ErrorsString.Length = 0 Then
                    Dim myErrorIndex As Integer = pISEResult.ReceivedResults.IndexOf("Cl") + 9
                    If pISEResult.ReceivedResults.Length > 45 Then
                        pISEResult.ErrorsString = pISEResult.ReceivedResults.Substring(myErrorIndex, 7)
                    End If
                End If
                'end SGM 23/07/2012

                Dim myStr As String = pISEResult.ErrorsString.Trim
                If myStr.Length > 0 Then
                    myResultError = New ISEErrorTO

                    Dim Position As Integer = 1

                    For Each posValue As Char In myStr
                        'Start from position 1 because position 0 has the IseModule Error or independent errors
                        If Position > 1 And posValue <> "0" Then

                            myResultError = New ISEErrorTO

                            myResultError.DigitNumber = Position
                            myResultError.DigitValue = posValue

                            If MyClass.ISEResultErrorsHT.ContainsKey(CStr(posValue)) Then
                                myResultError.Affected = MyClass.ISEResultErrorsHT(CStr(posValue)).ToString()

                                'SGM 23/07/2012
                                'In case of Li+ not installed, apply the one position lower
                                'JB 20/09/2012 -  Added parameter pForcedLiEnabledValue (to historical validations)
                                'If MyClass.myISEManager IsNot Nothing AndAlso Not MyClass.myISEManager.IsLiEnabledByUser Then
                                If pForcedLiEnabledValue = TriState.False OrElse _
                                  (pForcedLiEnabledValue = TriState.UseDefault AndAlso MyClass.myAnalyzer.ISEAnalyzer IsNot Nothing AndAlso Not MyClass.myAnalyzer.ISEAnalyzer.IsLiEnabledByUser) Then

                                    If myResultError.Affected.Contains("Li") Then
                                        If String.Equals(CStr(posValue), "1") Then
                                            myResultError.Affected = ""
                                        Else
                                            Select Case CStr(posValue)
                                                Case "1" : posValue = CChar("0")
                                                Case "3" : posValue = CChar("2")
                                                Case "5" : posValue = CChar("4")
                                                Case "7" : posValue = CChar("6")
                                                Case "9" : posValue = CChar("8")
                                                Case "B" : posValue = CChar("A")
                                                Case "D" : posValue = CChar("C")
                                                Case "F" : posValue = CChar("E")
                                            End Select
                                            myResultError.Affected = MyClass.ISEResultErrorsHT(CStr(posValue)).ToString()
                                        End If
                                    End If
                                End If
                                'end SGM 23/07/2012
                            End If

                            If myResultError.Affected.Length > 0 Then
                                'If Position = 3 Or Position = 5 Then

                                '    If pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.SER Then
                                '        myResultError.Message = "ISE_REMARK" & Position & "_SER"
                                '    ElseIf pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.URN Then
                                '        myResultError.Message = "ISE_REMARK" & Position & "_URI"
                                '    ElseIf pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL Then
                                '        myResultError.Message = "ISE_REMARK" & Position & "_SER"
                                '    End If
                                'Else
                                '    myResultError.Message = "ISE_REMARK" & Position
                                'End If


                                'SGM 20/07/2012
                                If MyClass.ISEResultErrorDescriptionsHT.ContainsKey(CStr(Position)) Then
                                    myResultError.ResultErrorCode = CType(Position, ISEErrorTO.ISEResultErrorCodes)
                                    Dim isUrine As Boolean = (pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.URN)
                                    Dim isCalibration As Boolean = (pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL)
                                    myGlobal = MyClass.SetISEErrorDescription(myResultError, isUrine, isCalibration)
                                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                        myResultError.ErrorDesc = CStr(myGlobal.SetDatos)
                                    End If
                                End If
                                'end SGM 20/07/2012

                                myResultErrors.Add(myResultError)

                            End If

                        End If
                        Position += 1
                    Next

                    'End If


                End If

                'SGM 23/07/2012
                'Add to pre-existing errors (for calibration results)
                If pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL Then
                    Dim myPreExistingErrors As List(Of ISEErrorTO) = pISEResult.Errors
                    For Each E As ISEErrorTO In myResultErrors
                        Dim alreadyExists As Boolean = False
                        For Each P As ISEErrorTO In myPreExistingErrors
                            If E.ResultErrorCode = P.ResultErrorCode Then
                                alreadyExists = True
                                Exit For
                            End If
                        Next
                        If Not alreadyExists Then
                            myPreExistingErrors.Add(E)
                        End If
                    Next
                    myResultErrors = myPreExistingErrors
                End If
                'end SGM 23/07/2012

                myGlobal.SetDatos = myResultErrors

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetResultErrors", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Gets independent error info (ERC)
        ''' </summary>
        ''' <param name="pISEResult"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  SG 25/07/2012
        ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
        ''' </remarks>
        Public Function GetCancelError(ByVal pISEResult As ISEResultTO) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myResultErrors As New List(Of ISEErrorTO)

                Dim myResultError As ISEErrorTO
                If pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL Then 'special case for CALB ERC error
                    If pISEResult.ReceivedResults.IndexOf("<ERC") > 40 Then
                        pISEResult.ErrorsString = pISEResult.ReceivedResults.Trim.Substring(pISEResult.ReceivedResults.Length - 13, 11)
                    Else
                        pISEResult.ErrorsString = pISEResult.ReceivedResults.Trim.Substring(5, pISEResult.ReceivedResults.Length - 2 - 5)
                    End If
                ElseIf pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.PMC Then 'special case for PMCL ERC error
                    If pISEResult.ReceivedResults.IndexOf("<ERC") > 24 Then
                        pISEResult.ErrorsString = pISEResult.ReceivedResults.Trim.Substring(pISEResult.ReceivedResults.Length - 13, 11)
                    Else
                        pISEResult.ErrorsString = pISEResult.ReceivedResults.Trim.Substring(5, pISEResult.ReceivedResults.Length - 2 - 5)
                    End If
                Else
                    pISEResult.ErrorsString = pISEResult.ReceivedResults.Trim.Substring(5, pISEResult.ReceivedResults.Length - 2 - 5)
                End If
                Dim myStr As String = pISEResult.ErrorsString.Trim
                If myStr.Length > 0 Then
                    myResultError = New ISEErrorTO

                    Dim myErrorCycle As ISEErrorTO.ErrorCycles

                    Select Case myStr.Substring(0, 3).ToUpperBS   ' ToUpper
                        Case "CAL" : myErrorCycle = ISEErrorTO.ErrorCycles.Calibration
                        Case "SER" : myErrorCycle = ISEErrorTO.ErrorCycles.Sample
                        Case "URN" : myErrorCycle = ISEErrorTO.ErrorCycles.Urine
                        Case "CLE" : myErrorCycle = ISEErrorTO.ErrorCycles.Clean
                        Case "PMC" : myErrorCycle = ISEErrorTO.ErrorCycles.PumpCalibration
                        Case "BBC" : myErrorCycle = ISEErrorTO.ErrorCycles.BubbleDetCalibration
                        Case "SIP" : myErrorCycle = ISEErrorTO.ErrorCycles.SipCycle
                        Case "PGA" : myErrorCycle = ISEErrorTO.ErrorCycles.PurgeA
                        Case "PGB" : myErrorCycle = ISEErrorTO.ErrorCycles.PurgeB
                        Case "DAL" : myErrorCycle = ISEErrorTO.ErrorCycles.DallasReadWrite
                        Case "MAT" : myErrorCycle = ISEErrorTO.ErrorCycles.Maintenance
                        Case "COM" : myErrorCycle = ISEErrorTO.ErrorCycles.Communication

                    End Select

                    myStr = myStr.Substring(4).Trim

                    Dim PosValue As String = myStr.Substring(0, 1)
                    myResultError = New ISEErrorTO
                    myResultError.IsCancelError = True
                    myResultError.DigitNumber = 1
                    myResultError.DigitValue = PosValue
                    'myResultError.Message = "ISE_REMARK" & PosValue 'Set the position value

                    If MyClass.ISECancelErrorsHT.ContainsKey(PosValue) Then
                        For E As Integer = 1 To 12 Step 1
                            Dim myErr As ISEErrorTO.ISECancelErrorCodes = CType(E, ISEErrorTO.ISECancelErrorCodes)
                            If String.Equals(myErr.ToString, PosValue.Trim) Then
                                myResultError.CancelErrorCode = myErr
                                myGlobal = MyClass.SetISEErrorDescription(myResultError)
                                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                    myResultError.ErrorDesc = CStr(myGlobal.SetDatos)
                                End If
                                Exit For
                            End If
                        Next
                        myResultError.Affected = MyClass.ISECancelErrorsHT(PosValue).ToString()
                    End If

                    myResultError.ErrorCycle = myErrorCycle
                    myResultErrors.Add(myResultError)

                End If

                myGlobal.SetDatos = myResultErrors

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetCancelError", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#End Region

#Region "ISE TEST Results"
        ''' <summary>
        ''' Initializes structures, get the execution, set the results values and finaly save results into database
        ''' NEW VERSION OF FUNCTION ProcessISETESTResults
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pPreparationID">Preparation ID</param>
        ''' <param name="pISEResult">Received ISE Result</param>
        ''' <param name="pISEMode">Indicate the ISE module operating mode: SimpleMode / DebugMode1 / DebugMode2
        ''' </param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS</returns>
        ''' <remarks>
        ''' Created by: SA 06/06/2014 - BT #1660 ==> Based on ProcessISETESTResults but when following issues fixed:
        '''                                          ** Validation of result inside/outside of the Normality Range has to be executed depending on the SampleClass:
        '''                                             - For CONTROLS, the result is validated using the theorical range defined for Control and ISETest/SampleType
        '''                                             - For PATIENTS, the result is validated using the Reference Range defined for the ISETest/SampleType (if there is 
        '''                                               one than exists and applies)
        '''                                          ** Changed the way of getting Alarms for the Average Result, due to the previous process has several errors
        '''                                          ** For CONTROLS it is not needed to update field AcceptedResultFlag = FALSE for the previous Rerun results
        ''' Modified by: XB 28/11/2014 - Recalculates calculated tests for ISE Tests - BA-1867
        '''              SA 01/12/2014 - Call to deprecated function UpdateStatusClosedNOK has been changed for the new version of that function (UpdateStatusClosedNOK_NEW)
        '''                              The old function was called by error and it has to be deleted
        '''              XB 16/01/2015 - Returns errors on Concentration when ISE modules returns an error - BA-1064
        ''' </remarks>
        Public Function ProcessISETESTResultsNEW(ByVal pDBConnection As SqlConnection, ByVal pPreparationID As Integer, ByRef pISEResult As ISEResultTO, _
                                                 ByVal pISEMode As String, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                Dim myReturnValue As New ExecutionsDS

                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myDebugModeOn As Boolean
                        If (pISEMode = "SimpleMode") Then
                            myDebugModeOn = False
                        ElseIf (pISEMode = "DebugMode1") OrElse (pISEMode = "DebugMode2") Then
                            myDebugModeOn = True
                        End If

                        'Decode the received ISE Result
                        Dim myISECycle As ISECycles = ISECycles.NONE
                        Dim myISEResultStr As String = pISEResult.ReceivedResults

                        myGlobalDataTO = ConvertISETESTResultToISEResultTO(myISEResultStr, myDebugModeOn)
                        If (Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing) Then
                            'Set the result to my ISE result TO 
                            pISEResult = DirectCast(myGlobalDataTO.SetDatos, ISEResultTO)
                            pISEResult.ReceivedResults = myISEResultStr

                            If (pISEResult.IsCancelError) Then
                                myGlobalDataTO = GetCancelError(pISEResult)
                            Else
                                myGlobalDataTO = GetResultErrors(pISEResult)
                            End If

                            If (Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing) Then
                                Dim myResultErrors As List(Of ISEErrorTO) = CType(myGlobalDataTO.SetDatos, List(Of ISEErrorTO))
                                pISEResult.Errors = myResultErrors

                                If (pISEResult.IsCancelError) Then
                                    Select Case myResultErrors(0).CancelErrorCode
                                        Case ISEErrorTO.ISECancelErrorCodes.A, ISEErrorTO.ISECancelErrorCodes.B, ISEErrorTO.ISECancelErrorCodes.S, ISEErrorTO.ISECancelErrorCodes.F
                                            MyClass.myAnalyzer.ISEAnalyzer.ISEWSCancelErrorCounter += 1
                                        Case ISEErrorTO.ISECancelErrorCodes.N
                                            MyClass.myAnalyzer.ISEAnalyzer.ISEWSCancelErrorCounter = 3
                                    End Select
                                Else
                                    MyClass.myAnalyzer.ISEAnalyzer.ISEWSCancelErrorCounter = 0
                                End If
                            End If

                            'Local variables to manage Replicate Results (Executions) and their Alarms
                            Dim diffExecutionIDList As List(Of Integer)
                            Dim myExecutionsAlarmsDS As New WSExecutionAlarmsDS
                            Dim myExecutionAlarmsRow As WSExecutionAlarmsDS.twksWSExecutionAlarmsRow
                            Dim myExecutionAlarmsDelegate As New WSExecutionAlarmsDelegate

                            'Local variables to manage Average Results and their Alarms
                            Dim myResultDS As New ResultsDS
                            Dim myTemResultDS As New ResultsDS
                            Dim myResultsRow As ResultsDS.twksResultsRow
                            Dim myResultDelegate As New ResultsDelegate
                            Dim qAlarmResult As New List(Of ISEErrorTO)
                            Dim myResultAlarmsDS As New ResultAlarmsDS
                            Dim myResultAlarmRow As ResultAlarmsDS.twksResultAlarmsRow
                            Dim myResultAlarmsDelegate As New ResultAlarmsDelegate

                            'Other local variables
                            Dim myAlarmID As Alarms
                            Dim myAverage As Single = 0
                            Dim myControlID As Integer = -1
                            Dim alarmExists As Boolean = False
                            'Dim currentSession As New GlobalBase
                            Dim mySampleClass As String = String.Empty

                            'Get all ISE Executions for the informed PreparationID
                            Dim myCalc As New RecalculateResultsDelegate
                            Dim myExecutionDelegate As New ExecutionsDelegate
                            Dim myCalculationISEDelegate As New CalculationISEDelegate

                            ' XB 28/11/2014 - BA-1867
                            Dim myCalcTestsDelegate As New OperateCalculatedTestDelegate

                            myGlobalDataTO = myExecutionDelegate.GetExecutionByPreparationID(dbConnection, pPreparationID, pWorkSessionID, pAnalyzerID, True)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myExecutionDS As ExecutionsDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)

                                For Each execRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions.Rows
                                    'Save the SampleClass (PATIENT or CTRL) in a local variable (just once, all Executions have the same SampleClass)
                                    If (mySampleClass = String.Empty) Then mySampleClass = execRow.SampleClass

                                    If (execRow.ExecutionStatus = "CLOSED") Then
                                        'Remove the row from the ExecutionsDS
                                        execRow.Delete()
                                    Else
                                        execRow.ExecutionStatus = "CLOSED"
                                        execRow.ResultDate = DateTime.Now
                                        execRow.InUse = True

                                        'Set the Concentration value depenting on the ISE_ResultID
                                        Select Case (execRow.ISE_ResultID)
                                            Case "Na"
                                                myGlobalDataTO = myCalculationISEDelegate.CalculatePreloadedConcentrationCorrection(execRow.SampleType, "Na", pISEResult.ConcentrationValues.Na)
                                                If (myGlobalDataTO.HasError) Then Exit For

                                                execRow.CONC_Value = DirectCast(myGlobalDataTO.SetDatos, Single)
                                                Exit Select

                                            Case "K"
                                                myGlobalDataTO = myCalculationISEDelegate.CalculatePreloadedConcentrationCorrection(execRow.SampleType, "K", pISEResult.ConcentrationValues.K)
                                                If (myGlobalDataTO.HasError) Then Exit For

                                                execRow.CONC_Value = DirectCast(myGlobalDataTO.SetDatos, Single)
                                                Exit Select

                                            Case "Cl"
                                                myGlobalDataTO = myCalculationISEDelegate.CalculatePreloadedConcentrationCorrection(execRow.SampleType, "Cl", pISEResult.ConcentrationValues.Cl)
                                                If (myGlobalDataTO.HasError) Then Exit For

                                                execRow.CONC_Value = DirectCast(myGlobalDataTO.SetDatos, Single)
                                                Exit Select

                                            Case "Li"
                                                myGlobalDataTO = myCalculationISEDelegate.CalculatePreloadedConcentrationCorrection(execRow.SampleType, "Li", pISEResult.ConcentrationValues.Li)
                                                If (myGlobalDataTO.HasError) Then Exit For

                                                execRow.CONC_Value = DirectCast(myGlobalDataTO.SetDatos, Single)
                                                Exit Select

                                            Case Else
                                                Exit Select
                                        End Select

                                        'AG 15/09/2014 - BA-1918 apply the user defined slope (if any)
                                        If Not myGlobalDataTO.HasError Then
                                            myGlobalDataTO = myCalculationISEDelegate.CalculateUserDefinedConcentrationCorrection(execRow.SampleType, execRow.TestID, execRow.CONC_Value)
                                            If Not myGlobalDataTO.HasError Then
                                                execRow.CONC_Value = DirectCast(myGlobalDataTO.SetDatos, Single)
                                            End If
                                        End If
                                        'AG 15/09/2014 - BA-1918

                                        ' XB 16/01/2015 - BA-1064
                                        If (pISEResult.IsCancelError) Then
                                            execRow.CONC_Value = -1
                                            execRow.CONC_Error = AbsorbanceErrors.INCORRECT_DATA.ToString
                                        End If
                                        ' XB 16/01/2015 - BA-1064

                                        'Get all received ISE Alarms and move them to a typed DataSet WSExecutionAlarmsDS
                                        qAlarmResult = (From a In pISEResult.Errors _
                                                       Where ((a.Affected.Contains(execRow.ISE_ResultID)) AndAlso (a.ResultErrorCode <> ISEErrorTO.ISEResultErrorCodes.None)) _
                                                      OrElse ((a.DigitNumber = 1) AndAlso (a.CancelErrorCode <> ISEErrorTO.ISECancelErrorCodes.None)) _
                                                      Select a).ToList()

                                        For Each ISEError As ISEErrorTO In qAlarmResult
                                            myExecutionAlarmsRow = myExecutionsAlarmsDS.twksWSExecutionAlarms.NewtwksWSExecutionAlarmsRow
                                            myExecutionAlarmsRow.ExecutionID = execRow.ExecutionID
                                            myExecutionAlarmsRow.AlarmDateTime = DateTime.Now

                                            If (ISEError.IsCancelError) Then
                                                Select Case (pISEResult.Errors(0).CancelErrorCode)
                                                    Case ISEErrorTO.ISECancelErrorCodes.A : myAlarmID = Alarms.ISE_ERROR_A
                                                    Case ISEErrorTO.ISECancelErrorCodes.B : myAlarmID = Alarms.ISE_ERROR_B
                                                    Case ISEErrorTO.ISECancelErrorCodes.C : myAlarmID = Alarms.ISE_ERROR_C
                                                    Case ISEErrorTO.ISECancelErrorCodes.D : myAlarmID = Alarms.ISE_ERROR_D
                                                    Case ISEErrorTO.ISECancelErrorCodes.F : myAlarmID = Alarms.ISE_ERROR_F
                                                    Case ISEErrorTO.ISECancelErrorCodes.M : myAlarmID = Alarms.ISE_ERROR_M
                                                    Case ISEErrorTO.ISECancelErrorCodes.N : myAlarmID = Alarms.ISE_ERROR_N
                                                    Case ISEErrorTO.ISECancelErrorCodes.P : myAlarmID = Alarms.ISE_ERROR_P
                                                    Case ISEErrorTO.ISECancelErrorCodes.R : myAlarmID = Alarms.ISE_ERROR_R
                                                    Case ISEErrorTO.ISECancelErrorCodes.S : myAlarmID = Alarms.ISE_ERROR_S
                                                    Case ISEErrorTO.ISECancelErrorCodes.T : myAlarmID = Alarms.ISE_ERROR_T
                                                    Case ISEErrorTO.ISECancelErrorCodes.W : myAlarmID = Alarms.ISE_ERROR_W
                                                End Select
                                                myExecutionAlarmsRow.AlarmID = myAlarmID.ToString
                                            Else
                                                myExecutionAlarmsRow.AlarmID = ISEError.ErrorDesc
                                            End If

                                            myExecutionsAlarmsDS.twksWSExecutionAlarms.AddtwksWSExecutionAlarmsRow(myExecutionAlarmsRow)
                                        Next
                                        myExecutionsAlarmsDS.AcceptChanges()

                                        'Load data of the received Result in a row of typed DataSet ResultsDS
                                        myResultsRow = myResultDS.twksResults.NewtwksResultsRow()
                                        myResultsRow.OrderTestID = execRow.OrderTestID
                                        myResultsRow.RerunNumber = execRow.RerunNumber
                                        myResultsRow.MultiPointNumber = execRow.MultiItemNumber
                                        myResultsRow.AnalyzerID = pAnalyzerID
                                        myResultsRow.WorkSessionID = pWorkSessionID
                                        myResultsRow.ValidationStatus = "OK"
                                        myResultsRow.AcceptedResultFlag = True
                                        myResultsRow.ExportStatus = "NOTSENT"
                                        myResultsRow.Printed = False
                                        myResultsRow.CONC_Value = execRow.CONC_Value
                                        myResultsRow.CONC_Error = execRow.CONC_Error  ' XB 16/01/2015 - BA-1064
                                        myResultsRow.TestID = execRow.TestID
                                        myResultsRow.SampleType = execRow.SampleType
                                        myResultsRow.SampleClass = mySampleClass
                                        If (mySampleClass = "CTRL") Then myResultsRow.ControlID = execRow.ControlID
                                        myResultsRow.ResultDateTime = execRow.ResultDate
                                        myResultsRow.TS_User = GlobalBase.GetSessionInfo.UserName
                                        myResultsRow.TS_DateTime = DateTime.Now
                                        myResultsRow.ExecutionID = execRow.ExecutionID
                                        myResultDS.twksResults.AddtwksResultsRow(myResultsRow)
                                        myResultDS.AcceptChanges()

                                        'Validate if the result for the Replicate is inside the defined Normality Range 
                                        myControlID = -1
                                        If (Not execRow.IsControlIDNull) Then myControlID = execRow.ControlID
                                        myGlobalDataTO = myCalc.ValidateISERefRanges(dbConnection, execRow.SampleClass, execRow.OrderTestID, execRow.TestID, execRow.SampleType, execRow.CONC_Value, myControlID)

                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            Dim validationResult As String = myGlobalDataTO.SetDatos.ToString

                                            If (validationResult <> String.Empty) Then
                                                'Load the Alarm found for the Replicate in the Executions Alarms DataSet (myExecutionsAlarmsDS)
                                                myExecutionAlarmsRow = myExecutionsAlarmsDS.twksWSExecutionAlarms.NewtwksWSExecutionAlarmsRow
                                                myExecutionAlarmsRow.ExecutionID = execRow.ExecutionID
                                                myExecutionAlarmsRow.AlarmDateTime = DateTime.Now
                                                myExecutionAlarmsRow.AlarmID = validationResult
                                                myExecutionsAlarmsDS.twksWSExecutionAlarms.AddtwksWSExecutionAlarmsRow(myExecutionAlarmsRow)
                                                myExecutionsAlarmsDS.AcceptChanges()
                                            End If
                                        End If
                                    End If
                                Next
                                myExecutionDS.AcceptChanges()

                                'Save the Executions 
                                myGlobalDataTO = myExecutionDelegate.SaveExecutionsResults(dbConnection, myExecutionDS)

                                'Save the Execution Alarms
                                If (Not myGlobalDataTO.HasError) Then
                                    diffExecutionIDList = (From a In myExecutionsAlarmsDS.twksWSExecutionAlarms _
                                                         Select a.ExecutionID Distinct).ToList()

                                    For Each execID As Integer In diffExecutionIDList
                                        'Remove all Alarms currently saved for this Execution
                                        myGlobalDataTO = myExecutionAlarmsDelegate.DeleteAll(dbConnection, execID)
                                        If (myGlobalDataTO.HasError) Then Exit For
                                    Next

                                    If (Not myGlobalDataTO.HasError) Then
                                        'Add all the new Alarms (for all Executions linked to the Preparation)
                                        myGlobalDataTO = myExecutionAlarmsDelegate.Add(dbConnection, myExecutionsAlarmsDS)
                                    End If
                                End If

                                'Re-calculate and save the Average Result 
                                If (Not myGlobalDataTO.HasError) Then
                                    myAverage = 0
                                    For Each resultRow As ResultsDS.twksResultsRow In myResultDS.twksResults.Rows
                                        myGlobalDataTO = myCalc.GetAverageConcentrationValue(dbConnection, resultRow.OrderTestID, resultRow.RerunNumber)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            'Get the calculated AVERAGE and update field CONC_VALUE for the Result
                                            myAverage = CType(myGlobalDataTO.SetDatos, Single)
                                            resultRow.CONC_Value = myAverage
                                        Else
                                            Exit For
                                        End If

                                        'Copy the Result to a temporary ResultsDS
                                        myTemResultDS.twksResults.Clear()
                                        myTemResultDS.twksResults.ImportRow(resultRow)

                                        'Save the Result 
                                        myGlobalDataTO = myResultDelegate.SaveResults(dbConnection, myTemResultDS)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                        'If it is a PATIENT Result, set AcceptedResultFlag = False for results of the previous Rerun Numbers
                                        '(for CONTROLS, several accepted Results are allowed, but not for PATIENT SAMPLES) 
                                        If (mySampleClass = "PATIENT") Then
                                            myGlobalDataTO = myResultDelegate.ResetAcceptedResultFlag(dbConnection, resultRow.OrderTestID, resultRow.RerunNumber)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                        End If

                                        'Validate if the Average Result is inside the Normality Range define for the ISETest/Sample Type
                                        myControlID = -1
                                        If (Not resultRow.IsControlIDNull) Then myControlID = resultRow.ControlID
                                        myGlobalDataTO = myCalc.ValidateISERefRanges(dbConnection, resultRow.SampleClass, resultRow.OrderTestID, resultRow.TestID, resultRow.SampleType, myAverage, myControlID)

                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            Dim validationResult As String = myGlobalDataTO.SetDatos.ToString

                                            If (validationResult <> String.Empty) Then
                                                'Load the Alarm found for the average Result in a row of a typed DataSet ResultAlarmsDS
                                                myResultAlarmRow = myResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
                                                myResultAlarmRow.OrderTestID = resultRow.OrderTestID
                                                myResultAlarmRow.RerunNumber = resultRow.RerunNumber
                                                myResultAlarmRow.MultiPointNumber = 1
                                                myResultAlarmRow.AlarmID = validationResult
                                                myResultAlarmRow.AlarmDateTime = Now
                                                myResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(myResultAlarmRow)
                                                myResultAlarmsDS.AcceptChanges()
                                            End If
                                        Else
                                            Exit For
                                        End If

                                        'Get the list of different Alarms for all active Executions for the OrderTestID/RerunNumber (excepting the Reference Ranges
                                        'Alarms: CONC_REMARK7 and CONC_REMARK8)
                                        myGlobalDataTO = myExecutionAlarmsDelegate.ReadAlarmsForAverageResult(dbConnection, pAnalyzerID, pWorkSessionID, resultRow.OrderTestID, _
                                                                                                              resultRow.RerunNumber)

                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            Dim myAlarmsDS As AlarmsDS = DirectCast(myGlobalDataTO.SetDatos, AlarmsDS)

                                            'Load all Alarms in the typed DataSet ResultAlarmsDS for the Average Result
                                            For Each execAlarm As AlarmsDS.tfmwAlarmsRow In myAlarmsDS.tfmwAlarms.Rows
                                                myResultAlarmRow = myResultAlarmsDS.twksResultAlarms.NewtwksResultAlarmsRow
                                                myResultAlarmRow.OrderTestID = resultRow.OrderTestID
                                                myResultAlarmRow.RerunNumber = resultRow.RerunNumber
                                                myResultAlarmRow.MultiPointNumber = 1
                                                myResultAlarmRow.AlarmID = execAlarm.AlarmID
                                                myResultAlarmRow.AlarmDateTime = Now
                                                myResultAlarmsDS.twksResultAlarms.AddtwksResultAlarmsRow(myResultAlarmRow)
                                                myResultAlarmsDS.AcceptChanges()
                                            Next
                                        End If

                                        'Delete all Alarms saved previously for the Average Result
                                        myGlobalDataTO = myResultAlarmsDelegate.DeleteAll(dbConnection, resultRow.OrderTestID, resultRow.RerunNumber, resultRow.MultiPointNumber)
                                        If (myGlobalDataTO.HasError) Then Exit For

                                    Next
                                End If

                                'Save the new Alarms for all calculated Average Result
                                If (Not myGlobalDataTO.HasError) Then
                                    myGlobalDataTO = myResultAlarmsDelegate.Add(dbConnection, myResultAlarmsDS)
                                End If

                                'Update the OrderTestStatus when needed
                                Dim myOrderID As String = String.Empty
                                Dim myOrderTestDelegate As New OrderTestsDelegate

                                If (Not myGlobalDataTO.HasError) Then
                                    For Each execRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions.Rows
                                        'Save the OrderID in a local variable (just once, all Executions have the same OrderID)
                                        If (myOrderID = String.Empty) Then myOrderID = execRow.OrderID

                                        'If it is the last Replicate Number then update the status of the OrderTest to CLOSED
                                        If (execRow.ReplicatesTotalNum = execRow.ReplicateNumber) Then
                                            myGlobalDataTO = myOrderTestDelegate.UpdateStatusByOrderTestID(dbConnection, execRow.OrderTestID, "CLOSED")
                                            If (myGlobalDataTO.HasError) Then Exit For


                                            ' XB 28/11/2014 - BA-1867
                                            ' recalculate calculated tests
                                            myCalcTestsDelegate.AnalyzerID = pAnalyzerID
                                            myCalcTestsDelegate.WorkSessionID = pWorkSessionID
                                            myGlobalDataTO = myCalcTestsDelegate.ExecuteCalculatedTest(dbConnection, execRow.OrderTestID, True)
                                            If (myGlobalDataTO.HasError) Then Exit For
                                            ' XB 28/11/2014 - BA-1867
                                        End If
                                    Next
                                End If

                                'If the DEBUG Mode is ON, then save the Results on the IseDebugModes Result
                                If (myDebugModeOn AndAlso Not myGlobalDataTO.HasError) Then
                                    Dim myPatientID As String = String.Empty

                                    If (mySampleClass = "PATIENT") Then
                                        'Get the Patient Identifier (PatientID or SampleID)
                                        Dim myOrdersDelegate As New OrdersDelegate

                                        myGlobalDataTO = myOrdersDelegate.ReadOrders(dbConnection, myOrderID)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            Dim myOrdersDS As OrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)

                                            If (myOrdersDS.twksOrders.Rows.Count > 0) Then
                                                If (Not myOrdersDS.twksOrders.First.IsPatientIDNull) Then
                                                    myPatientID = myOrdersDS.twksOrders.First.PatientID

                                                ElseIf (Not myOrdersDS.twksOrders.First.IsSampleIDNull) Then
                                                    myPatientID = myOrdersDS.twksOrders.First.SampleID
                                                End If
                                            End If
                                        End If
                                    End If

                                    If (Not myGlobalDataTO.HasError) Then
                                        'Validate the ISE Type to set the ISE Cycle
                                        Select Case pISEResult.ISEResultType
                                            Case ISEResultTO.ISEResultTypes.SER
                                                myISECycle = ISECycles.SAMPLE
                                                Exit Select
                                            Case ISEResultTO.ISEResultTypes.URN
                                                myISECycle = ISECycles.URINE1
                                                Exit Select
                                            Case ISEResultTO.ISEResultTypes.CAL
                                                myISECycle = ISECycles.CALIBRATION
                                                Exit Select
                                            Case Else
                                                Exit Select
                                        End Select

                                        pISEResult.WorkSessionID = pWorkSessionID
                                        pISEResult.PatientID = myPatientID
                                    End If
                                End If

                                'Release memory
                                qAlarmResult = Nothing
                                diffExecutionIDList = Nothing

                                'The ExecutionsDS will be the function return value
                                myReturnValue = myExecutionDS
                            End If

                            ' XB 28/11/2014 - BA-1867
                            myCalcTestsDelegate = Nothing

                        ElseIf (myGlobalDataTO.HasError) Then
                            'Get all ISE Executions for the informed PreparationID
                            Dim myExecutionDelegate As New ExecutionsDelegate
                            myGlobalDataTO = myExecutionDelegate.GetExecutionByPreparationID(dbConnection, pPreparationID, pWorkSessionID, pAnalyzerID, True)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myExecutionDS As ExecutionsDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)
                                'SA 01/12/2014 - Replaced the call to the old function UpdateStatusClosedNOK for its new version. The call to the previous version 
                                '                remains in the code by error (it was forgotten to change it when this new version of ProcessISETESTResults was implemented)  
                                myGlobalDataTO = myExecutionDelegate.UpdateStatusClosedNOK_NEW(dbConnection, myExecutionDS, False)
                                'For Each execRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions.Rows
                                '    'Mark the Execution as CLOSEDNOK
                                '    myGlobalDataTO = myExecutionDelegate.UpdateStatusClosedNOK(dbConnection, pAnalyzerID, pWorkSessionID, execRow.ExecutionID, execRow.OrderTestID, _
                                '                                                               execRow.ReplicatesTotalNum, False)
                                '    If (myGlobalDataTO.HasError) Then Exit For
                                'Next

                                'The ExecutionsDS will be the function return value
                                myReturnValue = myExecutionDS
                            End If
                        End If


                        If (Not myGlobalDataTO.HasError) Then
                            'Function returns the ExecutionsDS obtained
                            myGlobalDataTO.SetDatos = myReturnValue

                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEResultsDelegate.ProcessISEResultsNEW", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "ISE CMD Results"

        ''' <summary>
        ''' Overload for processing answer after ISECMD instruction is sent
        ''' </summary>
        ''' <param name="pISEResult"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pBiosystemsValidation">Determines if Biosystems validation algorithm is applied for validating the Reagents Pack</param>
        ''' <returns></returns>
        ''' <remarks>created by SGM 10/01/2012</remarks>
        Public Function ProcessISECMDResults(ByVal pISEResult As ISEResultTO, ByVal pAnalyzerID As String, ByVal pBiosystemsValidation As Boolean) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myISEResultTO As ISEResultTO = Nothing
                Dim myISEResultItemType As ISEResultTO.ISEResultItemTypes = ISEResultTO.ISEResultItemTypes.None

                Dim myResultStr As String = pISEResult.ReceivedResults.Trim

                If myResultStr.Length > 0 Then
                    'Decode the recived ISE Result.
                    'Dim myISEDecodeDelegate As New ISEDecodeDelegate

                    'Dim IsChecksumOK As Boolean = MyClass.CheckChecksum(myResultStr)


                    'initialize result object
                    myISEResultTO = New ISEResultTO
                    myISEResultTO.ReceivedResults = myResultStr

                    Dim myBlockStr As String

                    'first check if not an error
                    If Not myResultStr.Trim.EndsWith(">") Then
                        'If the received data is not complete or wrong infor in Log
                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ERC
                        myISEResultTO.IsCancelError = True
                        myISEResultItemType = ISEResultTO.ISEResultItemTypes.CancelError
                        Dim myError As New ISEErrorTO
                        myError.ErrorCycle = ISEErrorTO.ErrorCycles.Communication
                        myISEResultTO.Errors.Add(myError)

                        'SGM 04/09/2012
                        'Dim myLogAcciones As New ApplicationLogManager()
                        GlobalBase.CreateLogActivity("ISE - Wrong or uncomplete data received", "ISEResultsDelegate.ProcessISEResults", EventLogEntryType.Error, False)

                    ElseIf myResultStr.StartsWith("<ERC ") Then
                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ERC
                        myISEResultTO.IsCancelError = True
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.ERC)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CancelError
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)

                        End If

                    ElseIf myResultStr.Contains("<ISE!") Then
                        'simple response
                        myISEResultItemType = ISEResultTO.ISEResultItemTypes.Acknoledge
                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.OK

                    ElseIf myResultStr.Contains("<CAL ") Then
                        'CALIBRATION VALUES
                        '********************************************************************************************************************

                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.CAL

                        'one of the results can be Cancel error
                        If myResultStr.Contains("<ERC ") Then
                            'myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ERC
                            myISEResultTO.IsCancelError = True
                            myBlockStr = ""
                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.ERC)
                            If myBlockStr.Length > 0 Then
                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.CancelError
                                myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)

                            End If
                        End If

                        'Calibrator B milivolts
                        '<BMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalBMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        'Calibrator A milivolts
                        '<AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.AMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalAMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        'Calibration Slopes (milivolts/decade)
                        '<CAL Li xx.xx Na xx.xx K xx.xx Cl xx.xx>
                        'Must be in the next limits:
                        'Li+ 47-64 mV/dec
                        'Na+ 52-64 mV/dec
                        'K+ 52-64 mV/dec
                        'Cl- 40-55 mV/dec
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.CAL)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.Calibration1
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        Dim myCal1Index As Integer = myResultStr.IndexOf("<" & ISEResultTO.ISEResultBlockTypes.CAL.ToString)
                        Dim myCal2Index As Integer = myResultStr.LastIndexOf("<" & ISEResultTO.ISEResultBlockTypes.CAL.ToString)
                        If (myCal1Index >= 0 And myCal2Index >= 0) AndAlso (myCal2Index > myCal1Index) Then
                            Dim myCal2Str As String = myResultStr.Substring(myCal2Index)
                            myBlockStr = ""
                            myBlockStr = MyClass.ExtractResultBlock(myCal2Str, ISEResultTO.ISEResultBlockTypes.CAL)
                            If myBlockStr.Length > 0 Then
                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.Calibration2
                                myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                            End If
                        End If

                    ElseIf myResultStr.Contains("<PMC ") Then
                        'PUMPS CALIBRATION
                        '**************************************************************************************************************
                        '<PMC A xxxx B xxxx W xxxx> 

                        'Values A & B must be between 1500-3000
                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.PMC


                        'one of the results can be Cancel error
                        If myResultStr.Contains("<ERC ") Then
                            'myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ERC
                            myISEResultTO.IsCancelError = True
                            myBlockStr = ""
                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.ERC)
                            If myBlockStr.Length > 0 Then
                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.CancelError
                                myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)


                            End If
                        End If

                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.PMC)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.PumpsCalibration
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If


                    ElseIf myResultStr.Contains("<BBC ") Then
                        'BUBBLE DETECTOR CALIBRATION
                        '**************************************************************************************************************
                        '<BBC A xxxx B xxxx W xxxx> 
                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.BBC

                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BBC)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.BubbleCalibration
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                    ElseIf myResultStr.Contains("<SER ") Then
                        'SERUM
                        '*****************************************************************************************************************
                        'Serum milivolts
                        '<SMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.SMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.SerumMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        'Calibrator A milivolts
                        '<AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.AMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalAMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        'Calibrator B milivolts
                        '<BMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalBMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        'Sample Concentrations
                        '<SER Li xx.xx Na xxx.x K xx.xx Cl xxx.x eeeeeeec>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.SER)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.SerumConcentration
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If



                    ElseIf myResultStr.Contains("<URN ") Then
                        'URINE
                        '*****************************************************************************************************************

                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.URN


                        'Urine milivolts
                        '<UMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.UMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.UrineMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        ''Calibrator B milivolts
                        ''<BMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalBMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        ''Calibrator A milivolts
                        ''<AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.AMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalAMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        ''urine Concentrations
                        ''<URN Li xx.xx Na xxx.x K xx.xx Cl xxx.x eeeeeeec>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.URN)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.UrineConcentration
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If



                    ElseIf myResultStr.Contains("<ISV ") Then
                        'CHEKSUM*********************************************************************************************************
                        '<ISV cccc>
                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ISV

                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.ISV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.Checksum
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If
                        'PDT se pasa a caracteres? o se deja en hexadecimal?


                    ElseIf myResultStr.Contains("<AMV ") Then 'this case must be at the after SER and URN!!
                        'READ MILIVOLTS or SIPPING (Debug 2)***************************************************************************************

                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.AMV

                        'Calibrator A milivolts
                        '<AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.AMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalAMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        'Calibrator B milivolts
                        '<BMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalBMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                    ElseIf myResultStr.Contains("<BMV ") Then 'this case must be at the after SER and URN!!
                        'READ MILIVOLTS or SIPPING (Debug 2)***************************************************************************************

                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.AMV

                        'Calibrator B milivolts
                        '<BMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.BMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalBMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If

                        'Calibrator A milivolts
                        '<AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x>
                        myBlockStr = ""
                        myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.AMV)
                        If myBlockStr.Length > 0 Then
                            myISEResultItemType = ISEResultTO.ISEResultItemTypes.CalAMilivolts
                            myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                        End If


                    ElseIf myResultStr.Contains("<DSN ") Or myResultStr.Contains("<DDT ") Then
                        'DALLAS CARD*********************************************************************************************
                        If myResultStr.Contains("<DDT 00 ") Then

                            myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.DDT00

                            myBlockStr = ""
                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.DSN)
                            If myBlockStr.Length > 0 Then
                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.Dallas_SN
                                myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                            End If

                            myBlockStr = ""
                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.DDT_00)
                            If myBlockStr.Length > 0 Then
                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.Dallas_Page0
                                myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr, pBiosystemsValidation)
                            End If

                        ElseIf myResultStr.Contains("DDT 01 ") Then

                            myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.DDT01

                            myBlockStr = ""
                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.DSN)
                            If myBlockStr.Length > 0 Then
                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.Dallas_SN
                                myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr)
                            End If

                            myBlockStr = ""
                            myBlockStr = MyClass.ExtractResultBlock(myResultStr, ISEResultTO.ISEResultBlockTypes.DDT_01)
                            If myBlockStr.Length > 0 Then
                                myISEResultItemType = ISEResultTO.ISEResultItemTypes.Dallas_Page1
                                myGlobal = MyClass.FillISEResultValues(myISEResultTO, myISEResultItemType, myBlockStr, pBiosystemsValidation)
                            End If

                        End If

                    End If
                End If


                If myISEResultTO IsNot Nothing Then
                    'Save results into Database?

                    '??????
                    ''Call the save debug result data to save into XML file.
                    'myGlobalDataTO = SaveDebugModeResultData(myWorkSessionID, myPatientID, myISEResultTO.ReceivedResults, myISECycle)
                    If Not myGlobal.HasError Then
                        myGlobal.SetDatos = myISEResultTO
                    End If
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEResultsDelegate.ProcessISEResults", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function



#End Region

#Region "Common"

        ''' <summary>
        ''' Extracts the relevant data from ISE result string
        ''' </summary>
        ''' <param name="pISEResultStr"></param>
        ''' <param name="pType"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 20/01/2012</remarks>
        Private Function ExtractResultBlock(ByVal pISEResultStr As String, ByVal pType As ISEResultTO.ISEResultBlockTypes) As String
            Dim myBlockString As String = ""
            Try
                If pISEResultStr.Length > 0 AndAlso pType <> ISEResultTO.ISEResultTypes.None Then
                    Dim myType As String = pType.ToString
                    If myType = ISEResultTO.ISEResultBlockTypes.DDT_00.ToString Then myType = "DDT 00"
                    If myType = ISEResultTO.ISEResultBlockTypes.DDT_01.ToString Then myType = "DDT 01"
                    Dim myIndex As Integer = pISEResultStr.IndexOf("<" & myType)
                    If myIndex >= 0 Then
                        myBlockString = pISEResultStr.Substring(myIndex)
                        myBlockString = myBlockString.Substring(0, myBlockString.IndexOf(">") + 1)
                    End If
                End If
            Catch ex As Exception
                Throw ex
            End Try
            Return myBlockString
        End Function



        '''' <summary>
        '''' Save the ISE result and the patient or Sample id into 
        '''' the ISEResultDebugMode.Xml
        '''' </summary>
        '''' <param name="pWorkSession">WorkSession ID.</param>
        '''' <param name="pPatientID">Patient ID</param>
        '''' <param name="pISEResult">Recived ISE Result</param>
        '''' <returns></returns>
        '''' <remarks>CREATE BY: TR 14/01/2011</remarks>
        'Public Function SaveDebugModeResultData(ByVal pWorkSession As String, ByVal pPatientID As String, _
        '                                         ByVal pISEResult As String, ByVal pISECycle As GlobalEnumerates.ISECycles) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        Dim myISEResultsDebugModeDS As New ISEDebugResultsDS
        '        'Validate if the Xml file exits

        '        If IO.File.Exists(Windows.Forms.Application.StartupPath.ToString() & _
        '                                          GlobalBase.ISEResultDebugModeFilePath) Then

        '            'Load the file into the Type DataSet.
        '            myISEResultsDebugModeDS.ReadXml(Windows.Forms.Application.StartupPath.ToString() & _
        '                                            GlobalBase.ISEResultDebugModeFilePath, XmlReadMode.InferSchema)
        '        End If

        '        Dim myISEResultRow As ISEDebugResultsDS.DebugResultsRow
        '        myISEResultRow = myISEResultsDebugModeDS.DebugResults.NewDebugResultsRow
        '        'Set the values to the new row
        '        myISEResultRow.WorkSessionID = pWorkSession
        '        myISEResultRow.ISEDateTime = DateTime.Now
        '        myISEResultRow.ISECycle = pISECycle.ToString()
        '        myISEResultRow.PatientID = pPatientID.ToString()
        '        myISEResultRow.ISEResult = pISEResult

        '        myISEResultsDebugModeDS.DebugResults.AddDebugResultsRow(myISEResultRow)

        '        'Save the Xml file into the default file path.
        '        myISEResultsDebugModeDS.WriteXml(Windows.Forms.Application.StartupPath.ToString() & GlobalBase.ISEResultDebugModeFilePath)

        '        myGlobalDataTO.SetDatos = myISEResultsDebugModeDS

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ISEResultsDelegate.SaveDebugModeResultData", EventLogEntryType.Error, False)
        '    End Try

        '    Return myGlobalDataTO
        'End Function



        ''' <summary>
        ''' Load the ISE Parameter xml File into a DataSet on table AffectedElementTable
        ''' </summary>
        ''' <param name="pXmlPath"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 11/01/2010
        ''' </remarks>
        Private Function LoadISEModuleParammeters(ByVal pXmlPath As String) As DataSet
            Dim myResultDataSet As New DataSet
            Try
                Dim myISEParamXml As New XmlDocument
                'myISEParamXml.Load(Windows.Forms.Application.StartupPath.ToString() & ConfigurationManager.AppSettings("ISEParammetersFilePath").ToString())
                'TR 25/01/2011 -Replace by corresponding value on global base.
                myISEParamXml.Load(Application.StartupPath.ToString() & GlobalBase.ISEParammetersFilePath)
                myResultDataSet.ReadXml(pXmlPath)

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEResultsDelegate.LoadISEModuleParammeters", EventLogEntryType.Error, False)
            End Try
            Return myResultDataSet
        End Function

        ''' <summary>
        ''' Fill the hashTable use to store the result error descriptions
        ''' </summary>
        ''' <remarks>Cretaed by SGM 20/07/2012</remarks>
        Private Sub FillISEResultErrorDescHT()
            Try
                Dim myISEParamXmlDS As New DataSet

                myISEParamXmlDS = MyClass.LoadISEModuleParammeters(Application.StartupPath.ToString() & _
                                                                                GlobalBase.ISEParammetersFilePath)
                For Each iseRow As DataRow In myISEParamXmlDS.Tables("ResultErrorDescTable").Rows
                    myResultErrorDescHT.Add(iseRow("digit").ToString(), iseRow("ResultErrorDesc").ToString())
                Next

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.FillISEResultErrorDescHT", EventLogEntryType.Error, False)
            End Try

        End Sub


        ''' <summary>
        ''' Fill the hashTable use to store the affected elements.
        ''' </summary>
        ''' <remarks>
        ''' CREATE BY: TR 06/1/2010
        ''' </remarks>
        Private Sub FillAffectedElementHT()
            Try
                Dim myISEParamXmlDS As New DataSet

                'TR 25/01/2011
                myISEParamXmlDS = LoadISEModuleParammeters(Application.StartupPath.ToString() & _
                                                                                GlobalBase.ISEParammetersFilePath)
                'TR 25/01/2011
                For Each iseRow As DataRow In myISEParamXmlDS.Tables("AffectedElementTable").Rows
                    myAffectedElementsHT.Add(iseRow("id").ToString(), iseRow("AffectedElements").ToString())
                Next

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.FillAffectedElementHT", EventLogEntryType.Error, False)
            End Try

        End Sub

        ''' <summary>
        ''' Fill the Hastable to store ISE module Errors.
        ''' </summary>
        ''' <remarks>CREATED BY: TR 12/12/2011</remarks>
        Private Sub FillISECancelErrorDescHT()
            Try
                Dim myISEParamXmlDS As New DataSet
                myISEParamXmlDS = LoadISEModuleParammeters(Application.StartupPath.ToString() & _
                                                                                GlobalBase.ISEParammetersFilePath)
                For Each iseRow As DataRow In myISEParamXmlDS.Tables("CancelErrorDescTable").Rows
                    myISEModuleErrorHT.Add(iseRow("code").ToString(), iseRow("CancelErrorDesc").ToString())
                Next

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.FillISECancelErrorDescHT", EventLogEntryType.Error, False)
            End Try

        End Sub

#End Region

#Region "ISE TEST Result Decoding"

        ''' <summary>
        ''' Method incharge to convert the recived ISE result into a ISE Result TO.
        ''' </summary>
        ''' <param name="pISEResult">Recived ISE Result</param>
        ''' <param name="pDebugModeOn">ISE Module is in debug mode.</param>
        ''' <returns>Return an ISE Result TO.</returns>
        ''' <remarks>
        ''' CREATE BY: TR 14/01/2011
        ''' </remarks>
        Public Function ConvertISETESTResultToISEResultTO(ByVal pISEResult As String, _
                                                      ByVal pDebugModeOn As Boolean) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                'AG 19/03/2012 - Protection against Fw bug
                If pISEResult.Contains("<>") Then
                    myGlobalDataTO.HasError = True
                End If
                'AG 19/03/2012

                If Not myGlobalDataTO.HasError Then
                    If pDebugModeOn Then
                        myGlobalDataTO = DecodeComplexISETESTResult(pISEResult)
                    Else
                        myGlobalDataTO = DecodeSimpleISETESTResult(pISEResult)
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.ConvertISEResultToISEResultTO", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function




        ''' <summary>
        ''' Decode Simple ISE result and set to ISEResultTO. DEBUG OFF
        ''' </summary>
        ''' <param name="pISEResult">Recived ISE Result</param>
        ''' <returns>Returns an ISE Result TO</returns>
        ''' <remarks>
        ''' CREATE BY: TR 03/01/2010
        ''' Modified by: TR 22/11/2011 - The recived string change, now no != are received.
        ''' Modified by: SG 08/02/2012 - The received string does contain !=. Prevent unexpected Debug mode
        '''              XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
        ''' </remarks>
        Private Function DecodeSimpleISETESTResult(ByVal pISEResult As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                'Get the decimal separator.
                'Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

                Dim myISEResultTO As New ISEResultTO
                Dim isDebugResult As Boolean = False
                Dim myType As String = ""

                'SGM 08/02/2012 prevent unexpected Debug Mode
                isDebugResult = (pISEResult.Contains("<SMV ") Or pISEResult.Contains("<UMV "))
                If isDebugResult Then
                    'PDT FORZAR DEBUG OFF?
                    If pISEResult.Contains("<SER ") Then
                        pISEResult = pISEResult.Substring(pISEResult.IndexOf("<SER "))
                    ElseIf pISEResult.Contains("<URN ") Then
                        pISEResult = pISEResult.Substring(pISEResult.IndexOf("<URN "))
                    Else
                        'Sipping (not probable)
                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.SIP
                    End If
                End If

                ''AG 02/12/2011
                'Dim startChar As Integer
                'Dim endChar As Integer
                ''AG 02/12/2011

                'type of response
                myType = pISEResult.Substring(1, 3).ToString()


                'end SGM 08/02/2012



                'SGM 11/01/2012

                Dim myConcentrationValues As ISEResultTO.LiNaKCl

                Select Case myType.ToUpperBS.Trim     ' ToUpper.Trim
                    Case "SER"
                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.SER
                        '<SER Li xx.xx Na xxx.x K xx.xx Cl xxx.x eeeeeeec>
                        'AG 02/12/2011 - Example ser response "<SER Li 0.00 Na 141.0 K -0.01 Cl 3.1 000000D\>" it is different from the write on the manual

                        'Get the Remarks value 
                        'SGM 04/10/2012 - obtain Error string
                        Dim myErrorIndex As Integer = pISEResult.Trim.ToLower.IndexOf("cl") + 9
                        myISEResultTO.ErrorsString = pISEResult.Substring(myErrorIndex, 7)
                        'end SGM 04/10/2012

                        'myISEResultTO.ErrorsString = pISEResult.Substring(pISEResult.Length - 9, 7)SGM 04/10/2012

                        'Get the concentration values
                        myGlobal = MyClass.GetLiNaKClValues(pISEResult)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            myConcentrationValues = CType(myGlobal.SetDatos, ISEResultTO.LiNaKCl)
                        End If

                        myISEResultTO.ConcentrationValues = myConcentrationValues



                    Case "URN"
                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.URN
                        '<URN Na xxxxx K xxxxx Cl xxxxx eeeeeeec> urine structure
                        'AG 02/12/2011 - Example urn response "<URN Na 674 K - 1 Cl 30 0804006B>" it is different from the write on the manual
                        'NOTE the '-' is separated from the number "- 1"

                        'Get the Remarks value 
                        'Get the Remarks value 
                        'SGM 04/10/2012 - obtain Error string
                        Dim myErrorIndex As Integer = pISEResult.Trim.ToLower.IndexOf("cl") + 9
                        myISEResultTO.ErrorsString = pISEResult.Substring(myErrorIndex, 7)
                        'end SGM 04/10/2012

                        'myISEResultTO.ErrorsString = pISEResult.Substring(pISEResult.Length - 9, 7)SGM 04/10/2012

                        'Get the concentration values
                        myGlobal = MyClass.GetLiNaKClValues(pISEResult)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            myConcentrationValues = CType(myGlobal.SetDatos, ISEResultTO.LiNaKCl)
                        End If

                        myISEResultTO.ConcentrationValues = myConcentrationValues



                    Case "ERC"
                        myISEResultTO.ISEResultType = ISEResultTO.ISEResultTypes.ERC
                        'Example "ERC URN F000000"
                        'TODO
                        myISEResultTO.IsCancelError = True
                        myISEResultTO.ErrorsString = pISEResult.Substring(4, 11)


                End Select


                myGlobal.SetDatos = myISEResultTO

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.DecodeSimpleISETESTResult", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function


        ''' <summary>
        ''' Decode the ISE result when the ise module has the Debug mode on
        ''' </summary>
        ''' <param name="pISEResult">Recived ISE Results</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY: TR 14/01/2011 ISE Result lines format. SER/URI
        ''' </remarks>
        Private Function DecodeComplexISETESTResult(ByVal pISEResult As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim MyResults As String()
                Dim myISEResultTO As New ISEResultTO
                'Do and split by the character > because i can get all the importan info.
                MyResults = pISEResult.Split(CChar(">"))
                'ISE Result Line Format:
                'SER -<SMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x><AMV Li xxx.x Na xxx.x K xxx.x Cl xxx.x><SER Li xx.xx Na xxx.x K xx.xx Cl xxx.x eeeeeeec>
                'URINE- <UMV Na xxx.x K xxx.x Cl xxx.x><AMV Na xxx.x K xxx.x Cl xxx.x><URN Na xxxxx K xxxxx Cl xxxxx eeeeeeec> 
                For Each iseLine As String In MyResults
                    If iseLine.Length > 0 Then
                        If String.Equals(iseLine.Substring(1, 3).ToString(), "SER") OrElse String.Equals(iseLine.Substring(1, 3).ToString(), "URN") Then
                            'Call the Decode Simple function as soons as we haver the part of the recive result.
                            myGlobalDataTO = DecodeSimpleISETESTResult(iseLine)
                            If Not myGlobalDataTO.HasError Then
                                'Get the ISETO to set the correct recive line.
                                myISEResultTO = DirectCast(myGlobalDataTO.SetDatos, ISEResultTO)
                                myISEResultTO.ReceivedResults = pISEResult
                            Else
                                Exit For
                            End If
                        End If
                    End If
                Next
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.DecodeComplexISETESTResult", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Gets a structured data with values for each electrode
        ''' </summary>
        ''' <param name="pISEResult"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 09/02/2012</remarks>
        Public Function GetLiNaKClValues(ByVal pISEResult As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                'Dim Utilities As New Utilities
                Dim mySign As Integer = 1
                Dim myLiNaKClValues As New ISEResultTO.LiNaKCl(-1, -1, -1, -1)

                'SGM 07/05/2012
                If pISEResult.Length > 0 Then
                    While pISEResult.Contains("  ")
                        pISEResult = pISEResult.Replace("  ", " ")
                    End While
                End If
                'end SGM 07/05/2012

                If pISEResult.Length > 0 Then

                    'Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

                    Dim myIoneValuesStr As String

                    myIoneValuesStr = pISEResult.Substring(1, pISEResult.Length - 2).Trim()


                    'it does not bring result errors
                    If Not myIoneValuesStr.Contains(ISE_Electrodes.Cl.ToString & " ") Then
                        myIoneValuesStr = pISEResult.Substring(1, pISEResult.Length - 2).Trim()
                    End If

                    Dim myLiPos As Integer = myIoneValuesStr.IndexOf(ISE_Electrodes.Li.ToString & " ")
                    Dim myNaPos As Integer = myIoneValuesStr.IndexOf(ISE_Electrodes.Na.ToString & " ")
                    Dim myKPos As Integer = myIoneValuesStr.IndexOf(ISE_Electrodes.K.ToString & " ")
                    Dim myClPos As Integer = myIoneValuesStr.IndexOf(ISE_Electrodes.Cl.ToString & " ")
                    Dim myErrPos As Integer = myIoneValuesStr.LastIndexOf(" ")

                    'Lithium can be unused (urine tests)
                    If myLiPos >= 0 Then
                        Dim myLiStr As String = myIoneValuesStr.Substring(myLiPos, myNaPos - myLiPos).Trim
                        Dim myLiValueStr As String = myLiStr.Substring(ISE_Electrodes.Li.ToString.Length + 1).Trim

                        With myLiNaKClValues
                            If myLiValueStr.Contains("-") Then
                                myLiValueStr = myLiValueStr.Replace("-", "").Trim : mySign = -1
                            Else
                                mySign = 1
                            End If
                            .Li = mySign * Utilities.FormatToSingle(myLiValueStr)
                        End With
                    End If

                    'Sodium
                    If myNaPos >= 0 Then
                        Dim myNaStr As String = myIoneValuesStr.Substring(myNaPos, myKPos - myNaPos).Trim
                        Dim myNaValueStr As String = myNaStr.Substring(ISE_Electrodes.Na.ToString.Length + 1).Trim

                        With myLiNaKClValues
                            If myNaValueStr.Contains("-") Then
                                myNaValueStr = myNaValueStr.Replace("-", "").Trim : mySign = -1
                            Else
                                mySign = 1
                            End If
                            .Na = mySign * Utilities.FormatToSingle(myNaValueStr)
                        End With
                    End If

                    'Potassium 
                    If myKPos >= 0 Then
                        Dim myKStr As String = myIoneValuesStr.Substring(myKPos, myClPos - myKPos).Trim
                        Dim myKValueStr As String = myKStr.Substring(ISE_Electrodes.K.ToString.Length + 1).Trim

                        With myLiNaKClValues
                            If myKValueStr.Contains("-") Then
                                myKValueStr = myKValueStr.Replace("-", "").Trim : mySign = -1
                            Else
                                mySign = 1
                            End If
                            .K = mySign * Utilities.FormatToSingle(myKValueStr)
                        End With
                    End If

                    'Chlorine
                    If myClPos >= 0 Then
                        Dim myClStr As String = myIoneValuesStr.Substring(myClPos).Trim
                        If myErrPos > myClPos + 3 Then
                            myClStr = myIoneValuesStr.Substring(myClPos, myErrPos - myClPos).Trim
                        End If
                        Dim myClValueStr As String = myClStr.Substring(ISE_Electrodes.Cl.ToString.Length + 1).Trim


                        With myLiNaKClValues
                            If myClValueStr.Contains("-") Then
                                myClValueStr = myClValueStr.Replace("-", "").Trim : mySign = -1
                            Else
                                mySign = 1
                            End If
                            .Cl = mySign * Utilities.FormatToSingle(myClValueStr)
                        End With
                    End If

                End If

                myGlobal.SetDatos = myLiNaKClValues



            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetLiNaKClValues", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Gets a structured data with values for each peristaltic pump after calibration
        ''' </summary>
        ''' <param name="pISEResult"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 09/02/2012</remarks>
        Public Function GetPumpCalibrationValues(ByVal pISEResult As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                'Dim Utilities As New Utilities
                Dim mySign As Integer = 1
                Dim myPumpValues As New ISEResultTO.PumpCalibrationValues(-1, -1, -1)

                If pISEResult.Length > 0 Then

                    'Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

                    Dim myPumpValuesStr As String = pISEResult.Substring(1, pISEResult.Length - 2).Trim

                    Dim myPumpAPos As Integer = myPumpValuesStr.IndexOf(ISE_Pumps.A.ToString & " ")
                    Dim myPumpBPos As Integer = myPumpValuesStr.IndexOf(ISE_Pumps.B.ToString & " ")
                    Dim myPumpWPos As Integer = myPumpValuesStr.IndexOf(ISE_Pumps.W.ToString & " ")


                    If myPumpAPos >= 0 And myPumpBPos >= 0 And myPumpWPos >= 0 Then
                        Dim myPumpAStr As String = myPumpValuesStr.Substring(myPumpAPos, myPumpBPos - myPumpAPos).Trim
                        Dim myPumpAValueStr As String = myPumpAStr.Substring(ISE_Pumps.A.ToString.Length + 1).Trim
                        Dim myPumpBStr As String = myPumpValuesStr.Substring(myPumpBPos, myPumpWPos - myPumpBPos).Trim
                        Dim myPumpBValueStr As String = myPumpBStr.Substring(ISE_Pumps.B.ToString.Length + 1).Trim
                        Dim myPumpWStr As String = myPumpValuesStr.Substring(myPumpWPos).Trim
                        Dim myPumpWValueStr As String = myPumpWStr.Substring(ISE_Pumps.W.ToString.Length + 1).Trim

                        With myPumpValues
                            If myPumpAStr.Contains("-") Then
                                myPumpAValueStr = myPumpAValueStr.Replace("-", "").Trim : mySign = -1
                            Else
                                mySign = 1
                            End If
                            .PumpA = mySign * Utilities.FormatToSingle(myPumpAValueStr)

                            If myPumpBValueStr.Contains("-") Then
                                myPumpBValueStr = myPumpBValueStr.Replace("-", "").Trim : mySign = -1
                            Else
                                mySign = 1
                            End If
                            .PumpB = mySign * Utilities.FormatToSingle(myPumpBValueStr)

                            If myPumpWValueStr.Contains("-") Then
                                myPumpWValueStr = myPumpWValueStr.Replace("-", "").Trim : mySign = -1
                            Else
                                mySign = 1
                            End If
                            .PumpW = mySign * Utilities.FormatToSingle(myPumpWValueStr)

                        End With
                    End If

                End If

                myGlobal.SetDatos = myPumpValues



            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetPumpCalibrationValues", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Gets a structured data with values after calibration of bubble detector
        ''' </summary>
        ''' <param name="pISEResult"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 09/02/2012</remarks>
        Public Function GetBubbleDetectorCalibrationValues(ByVal pISEResult As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                'Dim Utilities As New Utilities
                Dim mySign As Integer = 1
                Dim myBubbleCalibValues As New ISEResultTO.BubbleCalibrationValues(-1, -1, -1)

                If pISEResult.Length > 0 Then

                    'Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

                    Dim myBubbleValuesStr As String = pISEResult.Substring(1, pISEResult.Length - 2).Trim

                    Dim myBubbleAPos As Integer = myBubbleValuesStr.IndexOf(ISE_Bubble_Detector.A.ToString & " ")
                    Dim myBubbleMPos As Integer = myBubbleValuesStr.IndexOf(ISE_Bubble_Detector.M.ToString & " ")
                    Dim myBubbleLPos As Integer = myBubbleValuesStr.IndexOf(ISE_Bubble_Detector.L.ToString & " ")


                    If myBubbleAPos >= 0 And myBubbleMPos >= 0 And myBubbleLPos >= 0 Then
                        Dim myBubbleAStr As String = myBubbleValuesStr.Substring(myBubbleAPos, myBubbleMPos - myBubbleAPos).Trim
                        Dim myBubbleAValueStr As String = myBubbleAStr.Substring(ISE_Bubble_Detector.A.ToString.Length + 1).Trim
                        Dim myBubbleMStr As String = myBubbleValuesStr.Substring(myBubbleMPos, myBubbleLPos - myBubbleMPos).Trim
                        Dim myBubbleMValueStr As String = myBubbleMStr.Substring(ISE_Bubble_Detector.M.ToString.Length + 1).Trim
                        Dim myBubbleLStr As String = myBubbleValuesStr.Substring(myBubbleLPos).Trim
                        Dim myBubbleLValueStr As String = myBubbleLStr.Substring(ISE_Bubble_Detector.L.ToString.Length + 1).Trim

                        With myBubbleCalibValues
                            If myBubbleAValueStr.Contains("-") Then
                                myBubbleAValueStr = myBubbleAValueStr.Replace("-", "").Trim : mySign = -1
                            Else
                                mySign = 1
                            End If
                            .ValueA = mySign * Utilities.FormatToSingle(myBubbleAValueStr)

                            If myBubbleMValueStr.Contains("-") Then
                                myBubbleMValueStr = myBubbleMValueStr.Replace("-", "").Trim : mySign = -1
                            Else
                                mySign = 1
                            End If
                            .ValueM = mySign * Utilities.FormatToSingle(myBubbleMValueStr)

                            If myBubbleLValueStr.Contains("-") Then
                                myBubbleLValueStr = myBubbleLValueStr.Replace("-", "").Trim : mySign = -1
                            Else
                                mySign = 1
                            End If
                            .ValueL = mySign * Utilities.FormatToSingle(myBubbleLValueStr)

                        End With
                    End If

                End If

                myGlobal.SetDatos = myBubbleCalibValues



            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetBubbleDetectorCalibrationValues", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

#End Region

#Region "ISE CMD result Decoding"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pISEResultTO"></param>
        ''' <param name="pForcedLiEnabledValue">The value to force in LiEnabled checks in the validation (Optional)
        ''' Value TriState.UseDefault: The LiEnabled value is the current LiEnabled value (saved in DB)
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY:  SGM 11/01/2012
        ''' MODIFIED BY: JBL 20/09/2012 - Added parameter pForcedLiEnabledValue set Public Function
        ''' </remarks>
        Public Function FillISEResultValues(ByRef pISEResultTO As ISEResultTO, ByVal pResultItem As ISEResultTO.ISEResultItemTypes, ByVal pISEValuesStr As String, Optional ByVal pBiosystemsValidation As Boolean = False, Optional ByVal pForcedLiEnabledValue As TriState = TriState.UseDefault) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                'Get the decimal separator.
                'Dim myDecimalSeparator As String = SystemInfoManager.OSDecimalSeparator
                If pISEValuesStr Is Nothing Then pISEValuesStr = String.Empty 'SGM 18/07/2013 - prevent null value
                pISEValuesStr = pISEValuesStr.Trim

                Dim myLiNaKClValues As ISEResultTO.LiNaKCl
                Dim myPumpCalibrationValues As ISEResultTO.PumpCalibrationValues
                Dim myBubbleCalibrationValues As ISEResultTO.BubbleCalibrationValues

                Dim myDallasSN As ISEDallasSNTO = Nothing
                Dim myDallas00 As ISEDallasPage00TO = Nothing
                Dim myDallas01 As ISEDallasPage01TO = Nothing
                Dim myResultErrors As New List(Of ISEErrorTO)
                myResultErrors = pISEResultTO.Errors

                If pResultItem = ISEResultTO.ISEResultItemTypes.CancelError Then
                    pISEResultTO.ErrorsString = pISEValuesStr.Trim.Substring(5, pISEValuesStr.Length - 2 - 5)
                    myGlobal = MyClass.GetCancelError(pISEResultTO)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myResultErrors = CType(myGlobal.SetDatos, List(Of ISEErrorTO))
                        'pISEResultTO.ResultErrorsString = myErrorStr
                    End If


                ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.PumpsCalibration Then
                    myGlobal = MyClass.GetPumpCalibrationValues(pISEValuesStr)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myPumpCalibrationValues = CType(myGlobal.SetDatos, ISEResultTO.PumpCalibrationValues)
                    End If


                ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.BubbleCalibration Then
                    myGlobal = MyClass.GetBubbleDetectorCalibrationValues(pISEValuesStr)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myBubbleCalibrationValues = CType(myGlobal.SetDatos, ISEResultTO.BubbleCalibrationValues)
                    End If


                ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.SerumConcentration Or _
                    pResultItem = ISEResultTO.ISEResultItemTypes.UrineConcentration Or _
                    pResultItem = ISEResultTO.ISEResultItemTypes.Calibration1 Or _
                    pResultItem = ISEResultTO.ISEResultItemTypes.Calibration2 Then

                    myGlobal = MyClass.GetLiNaKClValues(pISEValuesStr)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myLiNaKClValues = CType(myGlobal.SetDatos, ISEResultTO.LiNaKCl)
                    End If


                    Dim myErrorIndex As Integer
                    If pISEValuesStr.Length > 45 Then
                        If pResultItem <> ISEResultTO.ISEResultItemTypes.Calibration2 Then
                            myErrorIndex = pISEValuesStr.IndexOf("Cl") + 9
                            pISEResultTO.ErrorsString = pISEValuesStr.Substring(myErrorIndex, 7)
                        Else
                            myErrorIndex = pISEValuesStr.LastIndexOf("Cl") + 9
                            pISEResultTO.ErrorsString = pISEValuesStr.Substring(myErrorIndex, 7)
                        End If
                        'JB 20/09/2012 : Added pForcedLiEnabledValue parameter
                        myGlobal = MyClass.GetResultErrors(pISEResultTO, pForcedLiEnabledValue)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            myResultErrors = CType(myGlobal.SetDatos, List(Of ISEErrorTO))
                            'pISEResultTO.ResultErrorsString = myErrorStr
                        End If
                    End If

                ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.CalAMilivolts Or _
                   pResultItem = ISEResultTO.ISEResultItemTypes.CalBMilivolts Or _
                   pResultItem = ISEResultTO.ISEResultItemTypes.SerumMilivolts Or _
                   pResultItem = ISEResultTO.ISEResultItemTypes.UrineMilivolts Then

                    myGlobal = MyClass.GetLiNaKClValues(pISEValuesStr)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myLiNaKClValues = CType(myGlobal.SetDatos, ISEResultTO.LiNaKCl)
                    End If


                ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.Dallas_SN Then

                    'serial number
                    myGlobal = GetDallasSNValues(pISEValuesStr)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myDallasSN = CType(myGlobal.SetDatos, ISEDallasSNTO)
                    End If

                ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.Dallas_Page0 Then

                    'page 00
                    If pBiosystemsValidation Then
                        myGlobal = MyClass.GetDallasPage00Values_NEW(pISEValuesStr)
                    Else
                        myGlobal = MyClass.GetDallasPage00Values(pISEValuesStr)
                    End If

                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myDallas00 = CType(myGlobal.SetDatos, ISEDallasPage00TO)
                    End If

                ElseIf pResultItem = ISEResultTO.ISEResultItemTypes.Dallas_Page1 Then

                    'page 01
                    If pBiosystemsValidation Then
                        myGlobal = MyClass.GetDallasPage01Values_NEW(pISEValuesStr)
                    Else
                        myGlobal = MyClass.GetDallasPage01Values(pISEValuesStr)
                    End If
                    myGlobal = MyClass.GetDallasPage01Values(pISEValuesStr)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myDallas01 = CType(myGlobal.SetDatos, ISEDallasPage01TO)
                    End If

                End If

                Select Case pResultItem
                    Case ISEResultTO.ISEResultItemTypes.SerumConcentration, ISEResultTO.ISEResultItemTypes.UrineConcentration
                        pISEResultTO.ConcentrationValues = myLiNaKClValues

                    Case ISEResultTO.ISEResultItemTypes.SerumMilivolts
                        pISEResultTO.SerumMilivolts = myLiNaKClValues

                    Case ISEResultTO.ISEResultItemTypes.UrineMilivolts
                        pISEResultTO.UrineMilivolts = myLiNaKClValues

                    Case ISEResultTO.ISEResultItemTypes.CalAMilivolts
                        pISEResultTO.CalibratorAMilivolts = myLiNaKClValues

                    Case ISEResultTO.ISEResultItemTypes.CalBMilivolts
                        pISEResultTO.CalibratorBMilivolts = myLiNaKClValues

                    Case ISEResultTO.ISEResultItemTypes.Calibration1
                        pISEResultTO.CalibrationResults1 = myLiNaKClValues

                    Case ISEResultTO.ISEResultItemTypes.Calibration2
                        pISEResultTO.CalibrationResults2 = myLiNaKClValues

                    Case ISEResultTO.ISEResultItemTypes.PumpsCalibration
                        pISEResultTO.PumpsCalibrationValues = myPumpCalibrationValues

                    Case ISEResultTO.ISEResultItemTypes.BubbleCalibration
                        pISEResultTO.BubbleDetCalibrationValues = myBubbleCalibrationValues

                    Case ISEResultTO.ISEResultItemTypes.Dallas_SN
                        If myDallasSN IsNot Nothing Then pISEResultTO.DallasSNData = myDallasSN

                    Case ISEResultTO.ISEResultItemTypes.Dallas_Page0
                        If myDallas00 IsNot Nothing Then pISEResultTO.DallasPage00Data = myDallas00

                    Case ISEResultTO.ISEResultItemTypes.Dallas_Page1
                        If myDallas01 IsNot Nothing Then pISEResultTO.DallasPage01Data = myDallas01

                End Select

                If myResultErrors IsNot Nothing Then pISEResultTO.Errors = myResultErrors

                myGlobal.SetDatos = pISEResultTO

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.FillISEResultValues", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function




        'PDT ver si hace falta conversión a numérico
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDataStr"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 16/01/2012</remarks>
        Private Function GetDallasSNValues(ByVal pDataStr As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            ''Dim myUtil As New Utilities.
            Dim myDallasSNData As New ISEDallasSNTO

            Try

                Dim mySerialNumber As String = pDataStr.Trim.Substring(5, 16)
                Dim myCRCHex As String = pDataStr.Trim.Substring(21, 1)

                With myDallasSNData
                    .SNDataString = pDataStr.Trim

                    'SerialID 
                    .SerialNumber = mySerialNumber

                    'CRC Tester
                    .CRC = myCRCHex

                End With




            Catch ex As Exception
                myDallasSNData.ValidationError = True

                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetDallasSNValues", EventLogEntryType.Error, False)
            End Try

            myGlobal.SetDatos = myDallasSNData
            Return myGlobal

        End Function

#Region "OLD DALLAS"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDataStr"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 16/01/2012</remarks>
        Private Function GetDallasPage00Values(ByVal pDataStr As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            'Dim Utilities As New Utilities

            ''PENDING UNTIL INSTALLING BIOSYSTEMS REAGENTS PACKS
            ''If Environment.MachineName = "AUXSOFTWARE1" Then
            'If Environment.MachineName = "LABORATORIO-PC" Then
            '    myGlobal = GetDallasPage00Values_NEW(pDataStr)
            '    Return myGlobal
            'End If

            Try
                Dim myDallas00Data As New ISEDallasPage00TO

                Dim myLotNumberHex As String = pDataStr.Trim.Substring(8, 10)
                Dim myExpirationDayHex As String = pDataStr.Trim.Substring(18, 2)
                Dim myExpirationMonthHex As String = pDataStr.Trim.Substring(20, 2)
                Dim myExpirationYearHex As String = pDataStr.Trim.Substring(22, 2)
                Dim myInitialCalibAVolumeHex As String = pDataStr.Trim.Substring(44, 2)
                Dim myInitialCalibBVolumeHex As String = pDataStr.Trim.Substring(68, 2)
                Dim myDistributorCodeHex As String = pDataStr.Trim.Substring(46, 2)
                Dim mySecurityCodeHex As String = pDataStr.Trim.Substring(48, 8)
                Dim myCRCHex As String = pDataStr.Trim.Substring(70, 2)

                With myDallas00Data
                    .Page00DataString = pDataStr.Trim

                    'LotNumber
                    .LotNumber = myLotNumberHex

                    'ExpirationDay
                    myGlobal = Utilities.ConvertHexToUInt32(myExpirationDayHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .ExpirationDay = CInt(myGlobal.SetDatos)
                    End If

                    'ExpirationMonth
                    myGlobal = Utilities.ConvertHexToUInt32(myExpirationMonthHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .ExpirationMonth = CInt(myGlobal.SetDatos)
                    End If

                    'ExpirationYear
                    myGlobal = Utilities.ConvertHexToUInt32(myExpirationYearHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .ExpirationYear = 2000 + CInt(myGlobal.SetDatos)
                    End If

                    '.ExpirationDay = CInt(myExpirationDayHex)
                    '.ExpirationMonth = CInt(myExpirationMonthHex)
                    '.ExpirationYear = 2000 + CInt(myExpirationYearHex)

                    'InitialCalibAVolume (mililitres)
                    myGlobal = Utilities.ConvertHexToUInt32(myInitialCalibAVolumeHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .InitialCalibAVolume = 10 * CInt(myGlobal.SetDatos)
                    End If

                    'InitialCalibBVolume (mililitres)
                    myGlobal = Utilities.ConvertHexToUInt32(myInitialCalibBVolumeHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .InitialCalibBVolume = 10 * CInt(myGlobal.SetDatos)
                    End If

                    'DistributorCode 
                    myGlobal = Utilities.ConvertHexToUInt32(myDistributorCodeHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .DistributorCode = CStr(myGlobal.SetDatos)
                    End If


                    'SecurityCode 'PDT ????
                    .SecurityCode = mySecurityCodeHex

                    'CRC
                    .CRC = myCRCHex


                End With

                myGlobal.SetDatos = myDallas00Data

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetDallasPage00Values", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDataStr"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 16/01/2012</remarks>
        Private Function GetDallasPage01Values(ByVal pDataStr As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            'Dim Utilities As New Utilities

            ''PENDING UNTIL INSTALLING BIOSYSTEMS REAGENTS PACKS
            ''If Environment.MachineName = "AUXSOFTWARE1" Then
            'If Environment.MachineName = "LABORATORIO-PC" Then
            '    myGlobal = GetDallasPage01Values_NEW(pDataStr)
            '    Return myGlobal
            'End If

            Try
                Dim myDallas01Data As New ISEDallasPage01TO

                Dim myConsumptionCalAHex As String = pDataStr.Trim.Substring(8, 26)
                Dim myInstallationDayHex As String = pDataStr.Trim.Substring(34, 2)
                Dim myInstallationMonthHex As String = pDataStr.Trim.Substring(36, 2)
                Dim myInstallationYearHex As String = pDataStr.Trim.Substring(38, 2)
                Dim myConsumptionCalBHex As String = pDataStr.Trim.Substring(40, 26)
                Dim myNoGoodByteHex As String = pDataStr.Trim.Substring(66, 2)


                With myDallas01Data
                    .Page01DataString = pDataStr.Trim

                    'InstallationDay
                    myGlobal = Utilities.ConvertHexToUInt32(myInstallationDayHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .InstallationDay = CInt(myGlobal.SetDatos)
                    End If

                    'InstallationMonth
                    myGlobal = Utilities.ConvertHexToUInt32(myInstallationMonthHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .InstallationMonth = CInt(myGlobal.SetDatos)
                    End If

                    'InstallationYear
                    myGlobal = Utilities.ConvertHexToUInt32(myInstallationYearHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .InstallationYear = 2000 + CInt(myGlobal.SetDatos)
                    End If

                    '.InstallationDay = CInt(myInstallationDayHex)
                    '.InstallationMonth = CInt(myInstallationMonthHex)
                    '.InstallationYear = 2000 + CInt(myInstallationYearHex)

                    'ConsumptionCalA (%)

                    .ConsumptionCalAbitData = myConsumptionCalAHex
                    myGlobal = MyClass.GetConsumptionVolume(myConsumptionCalAHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .ConsumptionCalA = CInt(myGlobal.SetDatos)
                    End If

                    'ConsumptionCalB (%)
                    .ConsumptionCalBbitData = myConsumptionCalBHex
                    myGlobal = MyClass.GetConsumptionVolume(myConsumptionCalBHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .ConsumptionCalB = CInt(myGlobal.SetDatos)
                    End If

                    'No goodByte
                    .NoGoodByte = myNoGoodByteHex

                End With

                myGlobal.SetDatos = myDallas01Data

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetDallasPage01Values", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

#End Region

        ''' <summary>
        ''' PENDING UNTIL INSTALLING BIOSYSTEMS REAGENTS PACKS
        ''' </summary>
        ''' <param name="pDataStr"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 16/01/2012
        ''' New specification SGM 05/06/2012
        '''</remarks>
        Private Function GetDallasPage00Values_NEW(ByVal pDataStr As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            'Dim Utilities As New Utilities
            Dim myDallas00Data As New ISEDallasPage00TO

            Try

                Dim isError As Boolean
                Dim mySecCodeByte3Hex As String = pDataStr.Trim.Substring(8, 2)
                Dim mySecCodeByte0Hex As String = pDataStr.Trim.Substring(10, 2)
                Dim myLotNumberHex As String = pDataStr.Trim.Substring(24, 10)
                Dim myDistributorCodeHex As String = pDataStr.Trim.Substring(46, 2)
                Dim myInitialCalibAVolumeHex As String = pDataStr.Trim.Substring(56, 2)
                Dim mySecCodeByte2Hex As String = pDataStr.Trim.Substring(58, 2)
                Dim mySecCodeByte1Hex As String = pDataStr.Trim.Substring(60, 2)
                Dim myExpirationMonthHex As String = pDataStr.Trim.Substring(62, 2)
                Dim myExpirationYearHex As String = pDataStr.Trim.Substring(64, 2)
                Dim myExpirationDayHex As String = pDataStr.Trim.Substring(66, 2)
                Dim myInitialCalibBVolumeHex As String = pDataStr.Trim.Substring(68, 2)
                Dim myCRCHex As String = pDataStr.Trim.Substring(70, 2)

                With myDallas00Data
                    .Page00DataString = pDataStr.Trim

                    'LotNumber
                    myGlobal = Utilities.ConvertHexToUInt32(myLotNumberHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .LotNumber = CInt(myGlobal.SetDatos).ToString
                    Else
                        isError = isError Or True
                    End If


                    'ExpirationDay
                    myGlobal = Utilities.ConvertHexToUInt32(myExpirationDayHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .ExpirationDay = CInt(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If

                    'ExpirationMonth
                    myGlobal = Utilities.ConvertHexToUInt32(myExpirationMonthHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .ExpirationMonth = CInt(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If

                    'ExpirationYear
                    myGlobal = Utilities.ConvertHexToUInt32(myExpirationYearHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .ExpirationYear = 2000 + CInt(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If

                    '.ExpirationDay = CInt(myExpirationDayHex)
                    '.ExpirationMonth = CInt(myExpirationMonthHex)
                    '.ExpirationYear = 2000 + CInt(myExpirationYearHex)

                    'InitialCalibAVolume (mililitres)
                    myGlobal = Utilities.ConvertHexToUInt32(myInitialCalibAVolumeHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .InitialCalibAVolume = 10 * CInt(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If

                    'InitialCalibBVolume (mililitres)
                    myGlobal = Utilities.ConvertHexToUInt32(myInitialCalibBVolumeHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .InitialCalibBVolume = 10 * CInt(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If

                    'DistributorCode 
                    myGlobal = Utilities.ConvertHexToUInt32(myDistributorCodeHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .DistributorCode = CStr(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If


                    'SecurityCode 
                    .SecurityCode = mySecCodeByte3Hex & mySecCodeByte2Hex & mySecCodeByte1Hex & mySecCodeByte0Hex

                    'CRC
                    .CRC = myCRCHex

                    .ValidationError = isError Or .ValidationError

                End With



            Catch ex As Exception
                myDallas00Data.ValidationError = True

                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetDallasPage00Values", EventLogEntryType.Error, False)
            End Try

            myGlobal.SetDatos = myDallas00Data
            Return myGlobal

        End Function

        ''' <summary>
        ''' PENDING UNTIL INSTALLING BIOSYSTEMS REAGENTS PACKS
        ''' </summary>
        ''' <param name="pDataStr"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 16/01/2012
        ''' New specification SGM 05/06/2012
        '''</remarks>
        Private Function GetDallasPage01Values_NEW(ByVal pDataStr As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            'Dim Utilities As New Utilities
            Dim myDallas01Data As New ISEDallasPage01TO

            Try

                Dim isError As Boolean
                Dim myInstallationMonthHex As String = pDataStr.Trim.Substring(8, 2)
                Dim myInstallationYearHex As String = pDataStr.Trim.Substring(10, 2)
                Dim myInstallationDayHex As String = pDataStr.Trim.Substring(12, 2)
                Dim myConsumptionCalAHex As String = pDataStr.Trim.Substring(14, 26)
                Dim myConsumptionCalBHex As String = pDataStr.Trim.Substring(40, 26)
                Dim myNoGoodByteHex As String = pDataStr.Trim.Substring(66, 2)


                With myDallas01Data
                    .Page01DataString = pDataStr.Trim

                    'InstallationDay
                    myGlobal = Utilities.ConvertHexToUInt32(myInstallationDayHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .InstallationDay = CInt(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If

                    'InstallationMonth
                    myGlobal = Utilities.ConvertHexToUInt32(myInstallationMonthHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .InstallationMonth = CInt(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If

                    'InstallationYear
                    myGlobal = Utilities.ConvertHexToUInt32(myInstallationYearHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .InstallationYear = 2000 + CInt(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If

                    '.InstallationDay = CInt(myInstallationDayHex)
                    '.InstallationMonth = CInt(myInstallationMonthHex)
                    '.InstallationYear = 2000 + CInt(myInstallationYearHex)

                    'ConsumptionCalA (%)

                    .ConsumptionCalAbitData = myConsumptionCalAHex
                    myGlobal = MyClass.GetConsumptionVolume(myConsumptionCalAHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .ConsumptionCalA = CInt(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If

                    'ConsumptionCalB (%)
                    .ConsumptionCalBbitData = myConsumptionCalBHex
                    myGlobal = MyClass.GetConsumptionVolume(myConsumptionCalBHex)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        .ConsumptionCalB = CInt(myGlobal.SetDatos)
                    Else
                        isError = isError Or True
                    End If

                    'No goodByte
                    .NoGoodByte = myNoGoodByteHex

                    .ValidationError = isError Or .ValidationError

                End With

            Catch ex As Exception
                myDallas01Data.ValidationError = True

                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetDallasPage01Values", EventLogEntryType.Error, False)
            End Try

            myGlobal.SetDatos = myDallas01Data
            Return myGlobal

        End Function


        ''' <summary>
        ''' Gets the consumption value from the hex string (assuming that 1 bit equals to 1%)
        ''' </summary>
        ''' <param name="pHexString"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 16/01/2012</remarks>
        Private Function GetConsumptionVolume(ByVal pHexString As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                Dim myConsumption As Integer
                'Dim Utilities As New Utilities

                'each byte (2 char) represents 8%, one per bit
                Dim myBytes As New List(Of String)
                For c As Integer = 0 To pHexString.Length - 1 Step 2
                    myBytes.Add(pHexString.Substring(c, 2)) 'it must be 13 length
                Next c
                For Each B As String In myBytes
                    Dim myBinaryString As String
                    myGlobal = Utilities.ConvertHexToBinaryString(B)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myBinaryString = CStr(myGlobal.SetDatos)
                        Dim myHighWord As String = myBinaryString.Substring(0, 4)
                        Dim myLowWord As String = myBinaryString.Substring(4, 4)
                        For w As Integer = 0 To myLowWord.Length - 1
                            Dim myBit As String = myLowWord.Substring(w, 1)
                            If String.Equals(myBit, "0") Then myConsumption += 1

                        Next w
                        For w As Integer = 0 To myHighWord.Length - 1
                            Dim myBit As String = myHighWord.Substring(w, 1)
                            If String.Equals(myBit, "0") Then myConsumption += 1

                        Next w
                    Else
                        Exit For
                    End If
                Next

                myGlobal.SetDatos = myConsumption

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.GetConsumptionVolume", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pISEError"></param>
        ''' <remarks>Created by SGM 25/07/2012</remarks>
        Private Function SetISEErrorDescription(ByVal pISEError As ISEErrorTO, Optional ByVal pIsUrine As Boolean = False, Optional ByVal pIsCalibration As Boolean = False) As GlobalDataTO

            'Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            Dim myGlobal As New GlobalDataTO

            Try
                Dim myAlarmID As String = ""

                If pISEError.IsCancelError Then
                    'Independent ERC error
                    Dim myAlarm As Alarms
                    Select Case pISEError.CancelErrorCode
                        Case ISEErrorTO.ISECancelErrorCodes.A : myAlarm = Alarms.ISE_ERROR_A
                        Case ISEErrorTO.ISECancelErrorCodes.B : myAlarm = Alarms.ISE_ERROR_B
                        Case ISEErrorTO.ISECancelErrorCodes.C : myAlarm = Alarms.ISE_ERROR_C
                        Case ISEErrorTO.ISECancelErrorCodes.S : myAlarm = Alarms.ISE_ERROR_S
                        Case ISEErrorTO.ISECancelErrorCodes.F : myAlarm = Alarms.ISE_ERROR_F
                        Case ISEErrorTO.ISECancelErrorCodes.R : myAlarm = Alarms.ISE_ERROR_R
                        Case ISEErrorTO.ISECancelErrorCodes.W : myAlarm = Alarms.ISE_ERROR_W
                        Case ISEErrorTO.ISECancelErrorCodes.T : myAlarm = Alarms.ISE_ERROR_T
                        Case ISEErrorTO.ISECancelErrorCodes.N : myAlarm = Alarms.ISE_ERROR_N
                        Case ISEErrorTO.ISECancelErrorCodes.D : myAlarm = Alarms.ISE_ERROR_D
                        Case ISEErrorTO.ISECancelErrorCodes.P : myAlarm = Alarms.ISE_ERROR_P
                        Case ISEErrorTO.ISECancelErrorCodes.M : myAlarm = Alarms.ISE_ERROR_M
                        Case ISEErrorTO.ISECancelErrorCodes.None : myGlobal.SetDatos = "" : Exit Try

                    End Select

                    myAlarmID = myAlarm.ToString




                Else
                    'result error
                    Dim myAlarm As Alarms
                    Select Case pISEError.ResultErrorCode
                        Case ISEErrorTO.ISEResultErrorCodes.mvOut_CalBSample
                            myAlarm = Alarms.ISE_mVOutB

                        Case ISEErrorTO.ISEResultErrorCodes.mvOut_CalASample_CalBUrine
                            If Not pIsUrine Then
                                myAlarm = Alarms.ISE_mVOutA_SER
                            Else
                                myAlarm = Alarms.ISE_mVOutB_URI
                            End If

                        Case ISEErrorTO.ISEResultErrorCodes.mvNoise_CalBSample
                            myAlarm = Alarms.ISE_mVNoiseB

                        Case ISEErrorTO.ISEResultErrorCodes.mvNoise_CalBSample_CalBUrine
                            If Not pIsUrine Then
                                myAlarm = Alarms.ISE_mVNoiseA_SER
                            Else
                                myAlarm = Alarms.ISE_mVNoiseB_URI
                            End If

                        Case ISEErrorTO.ISEResultErrorCodes.Drift_CalASample
                            If Not pIsCalibration Then
                                myAlarm = Alarms.ISE_Drift_SER
                            Else
                                myAlarm = Alarms.ISE_Drift_CAL
                            End If

                        Case ISEErrorTO.ISEResultErrorCodes.OutOfSlope_MachineRanges
                            myAlarm = Alarms.ISE_OutSlope

                        Case ISEErrorTO.ISEResultErrorCodes.None
                            myGlobal.SetDatos = "" : Exit Try

                    End Select

                    myAlarmID = myAlarm.ToString

                End If

                myGlobal.SetDatos = myAlarmID.Trim

                'If myAlarmID.Length > 0 Then
                '    myGlobal = myFmwAlarms.Read(Nothing, myAlarmID)
                '    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                '        Dim myAlarmsDS As AlarmsDS = CType(myGlobal.SetDatos, AlarmsDS)
                '        If myAlarmsDS.tfmwAlarms.Rows.Count > 0 Then
                '            myGlobal.SetDatos = myAlarmsDS.tfmwAlarms(0).Name
                '        Else
                '            myGlobal.SetDatos = ""
                '        End If
                '    End If
                'Else
                '    myGlobal.SetDatos = ""
                'End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.SetISEErrorDescription", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function





#End Region
    End Class
End Namespace

