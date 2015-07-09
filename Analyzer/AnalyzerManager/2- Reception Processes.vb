Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports System.Data.SqlClient
Imports System.Globalization
Imports Biosystems.Ax00.CommunicationsSwFw
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Partial Public Class AnalyzerManager
        Implements IAnalyzerManager

#Region "Private Reception Methods"

        ''' <summary>
        ''' Readings reception process (save readings, update executions tables and trigger calculations when needed)
        ''' 
        ''' Method scheme (AG 18/02/2011)
        ''' Open Transaction (Connection1)
        ''' --- Loop
        ''' --- ... Decode readings and build DS
        ''' --- End Loop
        ''' --- Save readings
        ''' Commint transaction (Connection1)
        ''' Open transaction (Connection1)
        ''' --- Use the same DS for loop
        ''' --- ... Call calculations, auto repetitions, online export, ... when needed
        ''' --- End Loop
        ''' Commint transaction (Connection1)
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns>GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Defined by:  AG - 26/04/2010
        ''' Created by: GDS - 27/04/2010
        ''' Modified by AG 12/05/2010 - Call the repetitions and autoexport when the ordertestid is closed
        ''' Modified by AG 08/06/2010 - Validate execution readings before call calculations methods (not SATURATED READINGS neiter READING_ERROR values exits)
        ''' AG 26/11/2010 - prepare the UI_Refresh event
        ''' AG 18/02/2011 - implement scheme described in summary
        ''' AG 22/03/2011 - Update executions.ThermoWarningFlag when exists the thermo reactions rotor alarm 
        '''                 Do not save readings when execution status is PENDING
        ''' Modified by AG 20/04/2011 - changes in instruction definition Excel Instruction Rev23)
        ''' Modified by AG 04/10/2011 - export generates event RESULTS_CALCULATED + move code for Auto repetitions, auto export to be used also when
        '''                             last replicate in ordertest is finished with CLOSEDNOK
        ''' Modified AG 12/06/202 - complete redesing ... the photometric readings are treated in a different thread in order the Sw can attend quickly to the Analzyer requests in Running
        '''                         The old method is commented, renamed as ProcessReadingsReceived_AG12062012 and moved into '9- Temporal Testing and Old Methods.vb'
        ''' </remarks>
        Private Function ProcessReadingsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                Dim startTime As DateTime = Now 'AG 11/06/2012 - time estimation

                SyncLock LockThis
                    bufferANSPHRReceived.Add(pInstructionReceived)

                    GlobalBase.CreateLogActivity("Treat ANSPHR (Add to buffer) received. Buffer items: " & bufferANSPHRReceived.Count.ToString & ". Time: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessReadingsReceived", EventLogEntryType.Information, False)
                End SyncLock

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessReadingsReceived", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATE BY:AG
        ''' Modified by: TR -Add the functionally to process the base line reception.
        '''              01/03/2011 - ANSAL is rename as ANSBLD (ANSBL and ANSDL are removed) - Tested PENDING
        '''              AG 28/10/2014 - BA-2057
        '''              IT 01/12/2014 - BA-2075
        '''              IT 19/12/2014 - BA-2143
        ''' </remarks>
        Private Function ProcessBaseLineReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim dbConnection As New SqlConnection
            Dim myGlobalDataTO As New GlobalDataTO


            Try
                'InitializeTimerControl(WAITING_TIME_OFF) 'AG 13/02/2012 - the waiting timer is disabled on every reception process ('AG 18/07/2011)

                myGlobalDataTO = GetOpenDBTransaction(Nothing)
                If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Business

                        'AG 28/02/2012 - move this code when the received results generates baseline init err alarm 
                        ''If exits base line alarm delete it before send a new ALIGHT instruction
                        'If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.BASELINE_INIT_ERR) Then
                        '    myAlarmListAttribute.Remove(AlarmEnumerates.Alarms.BASELINE_INIT_ERR)
                        'End If

                        Dim query As New List(Of InstructionParameterTO)
                        Dim myOffset As Integer = -1
                        Dim nextBaseLineID As Integer = 0
                        Dim myInstructionType As String = ""
                        Dim myWell As Integer = 0
                        Dim myTotalResults As Integer = 0
                        Dim baseLineWithAdjust As Boolean = False

                        'AG 03/01/2011 - Get the instruction type value. to set the offset.
                        myGlobalDataTO = GetItemByParameterIndex(pInstructionReceived, 2)

                        If Not myGlobalDataTO.HasError Then
                            myInstructionType = DirectCast(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue
                            If myInstructionType = AppLayerInstrucionReception.ANSBLD.ToString Then baseLineWithAdjust = True
                        Else
                            Exit Try
                        End If

                        'Get the well used from pInstructionReceived and initialize myWell variable
                        query = (From a In pInstructionReceived Where a.ParameterIndex = 3 Select a).ToList()
                        If query.Count > 0 Then
                            myWell = CType(query.First().ParameterValue, Integer)
                            CurrentWellAttribute = myWell 'AG 30/06/2011 - When Sw receives a ALIGHT updates this value for repeat alight in the next well is results are rejected
                            '                                                                          (this code is required due the well in status instruction has another value due means another thing and _
                            '                                                                           else the Alight repetition can be perform in the same well another time)
                        End If
                        '1st Get the next baselineID
                        myGlobalDataTO = GetNextBaseLineID(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myWell, baseLineWithAdjust)
                        If Not myGlobalDataTO.HasError Or Not myGlobalDataTO.SetDatos Is Nothing Then
                            nextBaseLineID = DirectCast(myGlobalDataTO.SetDatos, Integer)
                        Else
                            Exit Try
                        End If

                        'filter to get the results only from the instruction recived.
                        query = (From a In pInstructionReceived Where a.ParameterIndex > 3 Select a).ToList()

                        'Set the offset value depending on the instruction type
                        myOffset = 7
                        Dim myBaseLineDS As New BaseLinesDS

                        If myOffset > 0 And query.Count > 0 Then

                            myTotalResults = CInt(query.Count / myOffset)
                            Dim myIteration As Integer
                            Dim myBaseLineRow As BaseLinesDS.twksWSBaseLinesRow

                            For i As Integer = 1 To myTotalResults Step 1
                                myIteration = i
                                myBaseLineRow = myBaseLineDS.twksWSBaseLines.NewtwksWSBaseLinesRow
                                myBaseLineRow.BeginEdit()
                                myBaseLineRow.BaseLineID = nextBaseLineID
                                myBaseLineRow.WellUsed = myWell
                                myBaseLineRow.WorkSessionID = Me.WorkSessionIDAttribute
                                myBaseLineRow.AnalyzerID = Me.AnalyzerIDAttribute
                                myBaseLineRow.Type = BaseLineType.STATIC.ToString 'AG 28/10/2014 BA-2057
                                myBaseLineRow.DateTime = DateTime.Now

                                'Get the Wavelenght
                                myGlobalDataTO = GetItemByParameterIndex(pInstructionReceived, 4 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    myBaseLineRow.Wavelength = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                Else
                                    Exit For
                                End If

                                'MainLine
                                myGlobalDataTO = GetItemByParameterIndex(pInstructionReceived, 5 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.MainLight = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'RefLight
                                myGlobalDataTO = GetItemByParameterIndex(pInstructionReceived, 6 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.RefLight = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'MainDark
                                myGlobalDataTO = GetItemByParameterIndex(pInstructionReceived, 7 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.MainDark = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'RefDark
                                myGlobalDataTO = GetItemByParameterIndex(pInstructionReceived, 8 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.RefDark = CInt(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue)
                                    End If
                                Else
                                    Exit For
                                End If

                                'IT
                                myGlobalDataTO = GetItemByParameterIndex(pInstructionReceived, 9 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.IT = CType(CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue, Single)
                                    End If

                                Else
                                    Exit For
                                End If
                                'DAC
                                myGlobalDataTO = GetItemByParameterIndex(pInstructionReceived, 10 + (myIteration - 1) * myOffset)
                                If Not myGlobalDataTO.HasError Then
                                    If CType(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue <> "" Then
                                        myBaseLineRow.DAC = CType(DirectCast(myGlobalDataTO.SetDatos, InstructionParameterTO).ParameterValue, Single)
                                    End If
                                Else
                                    Exit For
                                End If

                                myBaseLineRow.SetABSvalueNull() 'This field is use in base line by well
                                myBaseLineRow.SetIsMeanNull() 'This field is use in base line by well

                                myBaseLineRow.EndEdit()
                                myBaseLineDS.twksWSBaseLines.AddtwksWSBaseLinesRow(myBaseLineRow)
                            Next
                            myBaseLineDS.AcceptChanges()

                        End If


                        If (Not myGlobalDataTO.HasError) Then
                            'Save baseline results into database
                            myGlobalDataTO = Me.SaveBaseLineResults(dbConnection, myBaseLineDS, baseLineWithAdjust, BaseLineType.STATIC.ToString)

                            If Not myGlobalDataTO.HasError Then
                                'Perform ANSBLD results business
                                validALIGHTAttribute = False
                                myGlobalDataTO = BaseLine.ControlAdjustBaseLine(dbConnection, myBaseLineDS)
                                validALIGHTAttribute = BaseLine.validALight
                                existsALIGHTAttribute = BaseLine.existsAlightResults 'AG 20/06/2012

                                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                                    'ControlAdjustBaseLine only generates myAlarm = AlarmEnumerates.Alarms.BASELINE_INIT_ERR  ... now 
                                    'we have to calculate his status = TRUE or FALSE
                                    Dim alarmStatus As Boolean = False 'By default no alarm                                    
                                    Dim myAlarm = CType(myGlobalDataTO.SetDatos, Alarms)
                                    If myAlarm <> AlarmEnumerates.Alarms.NONE Then

                                        baselineInitializationFailuresAttribute += 1
                                        If baselineInitializationFailuresAttribute >= ALIGHT_INIT_FAILURES Then
                                            alarmStatus = True
                                            If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.BASELINE_INIT_ERR) Then
                                                myAlarmListAttribute.Remove(AlarmEnumerates.Alarms.BASELINE_INIT_ERR)
                                            End If
                                        Else
                                            'No alarm!! Alarm will be generated when the ALIGHT fails the maximum times
                                            myAlarm = AlarmEnumerates.Alarms.NONE
                                            alarmStatus = True
                                            myGlobalDataTO = SendAutomaticALIGHTRerun(dbConnection)
                                        End If

                                    Else ' Valid alight
                                        myAlarm = AlarmEnumerates.Alarms.BASELINE_INIT_ERR
                                        alarmStatus = False
                                        ResetBaseLineFailuresCounters() 'AG 27/11/2014 BA-2066
                                    End If

                                    Dim AlarmList As New List(Of Alarms)
                                    Dim AlarmStatusList As New List(Of Boolean)

                                    If myAlarm <> AlarmEnumerates.Alarms.NONE Then
                                        PrepareLocalAlarmList(myAlarm, alarmStatus, AlarmList, AlarmStatusList) 'Baseline_init_err (true or false)
                                        'AG 23/05/2012 - Baseline err excludes methacrylate rotor warn
                                        If alarmStatus AndAlso myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.BASELINE_WELL_WARN) Then
                                            myAlarmListAttribute.Remove(AlarmEnumerates.Alarms.BASELINE_WELL_WARN)
                                        End If

                                        If AlarmList.Count > 0 Then
                                            If Not GlobalBase.IsServiceAssembly Then
                                                Dim currentAlarms = New AnalyzerAlarms(Me)
                                                myGlobalDataTO = currentAlarms.Manage(AlarmList, AlarmStatusList)
                                            End If
                                        End If
                                    End If

                                    'IT 26/11/2014 - BA-2075 INI
                                    'If no alarm or all ALIGHT has been rejected ... inform presentation depending the current Sw process
                                    If Not myGlobalDataTO.HasError Then
                                        'AG 28/02/2012
                                        If validALIGHTAttribute Or baselineInitializationFailuresAttribute >= ALIGHT_INIT_FAILURES Then

                                            'Inform flag for alight is finished
                                            Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                                            If (validALIGHTAttribute) Then
                                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.BaseLine, "END")
                                            Else
                                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.BaseLine, "CANCELED")
                                            End If

                                            If (mySessionFlags(AnalyzerManagerFlags.WUPprocess.ToString) = "INPROCESS") Then
                                                RaiseEvent ProcessFlagEventHandler(AnalyzerManagerFlags.BaseLine) 'BA-2288

                                            ElseIf mySessionFlags(AnalyzerManagerFlags.BASELINEprocess.ToString) = "INPROCESS" Then
                                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.BASELINEprocess, "CLOSED")

                                            ElseIf mySessionFlags(AnalyzerManagerFlags.NEWROTORprocess.ToString) = "INPROCESS" Then
                                                RaiseEvent ProcessFlagEventHandler(AnalyzerManagerFlags.BaseLine) 'BA-2143

                                            End If

                                            'Update analyzer session flags into DataBase
                                            If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                                myGlobalDataTO = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                                            End If

                                        End If
                                    End If
                                End If
                            End If
                        End If
                        query = Nothing 'AG 02/08/2012 - free memory
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessBaseLineReceived", EventLogEntryType.Error, False)
            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (Not dbConnection Is Nothing) Then
                If (Not myGlobalDataTO.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    RollbackTransaction(dbConnection)
                End If
                dbConnection.Close()
            End If

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Software has received a new Hw alarms details instruction
        ''' Detect the alarms status new or fixed and call to the alamrs manager system 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns> GlobalDataTo indicating if an error has occurred or not</returns>
        ''' <remarks>
        ''' Created by:  AG 14/03/2011
        ''' Modified by: SA 19/12/2014 - Replaced sentences DirectCast(CInt(myInstParamTO.ParameterValue), Integer) by CInt(myInstParamTO.ParameterValue)
        '''                              due to the first one is redundant and produces building warnings
        '''              SA 31/03/2015 - BA-2384 ==> Based in implementation of BA-2236 for BA-200. Added changes to allow inform the FW Error Code as 
        '''                                          Additional Information for each Alarm. Removed use of dbConnection variable and the code to Commit/Rollback 
        '''                                          a DB Transaction due to no DB Connection is opened by this function. Removed old commented code. Removed all 
        '''                                          Exit Try sentences. Added Comments. 
        '''              IT 16/04/2015 - BA-2441 Code review related with BA-2384
        ''' </remarks>
        Private Function ProcessHwAlarmDetailsReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing
            Dim myGlobal As New GlobalDataTO

            Try
                Debug.Print("ANSERR received !")
                Dim myAlarmsReceivedList As New List(Of Alarms)
                Dim myAlarmsStatusList As New List(Of Boolean)
                Dim myAlarmsAdditionalInfoList As New List(Of String) 'AG 09/12/2014 BA-2236
                Dim IsAStateError = False

                '1- Read the different instruction fields (implement in the same way as we do for instance at ProcessStatusReceived)
                '   and fill the internal alarm & alarm status lists using the method PrepareLocalAlarmList 
                'Dim Utilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO

                ' Get error number field
                Dim errorNumber As Integer = 0
                myGlobal = GetItemByParameterIndex(pInstructionReceived, 3)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                Else
                    Exit Try
                End If

                If IsNumeric(myInstParamTO.ParameterValue) Then
                    errorNumber = CInt(myInstParamTO.ParameterValue)
                End If

                Dim errorCode As Integer = 0
                Dim myErrorCodes As New List(Of Integer)


                For i As Integer = 1 To errorNumber
                    myGlobal = GetItemByParameterIndex(pInstructionReceived, 3 + i)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
                    Else
                        Exit Try
                    End If

                    If IsNumeric(myInstParamTO.ParameterValue) Then
                        errorCode = CInt(myInstParamTO.ParameterValue)

                        'SGM 16/10/2012 - Ommit these alarms in order to avoid endless loop
                        'E:20 (INST_REJECTED_ERR)
                        'E:21 (INST_ABORTED_ERR)
                        'E:99 Multiple

                        ' XBC 29/10/2012 - Service Sw no ommit err 21 on ANSERR
                        If GlobalBase.IsServiceAssembly Then
                            ' Service Sw
                            If errorCode <> 20 And errorCode <> 99 Then
                                myErrorCodes.Add(errorCode)
                            End If
                        Else
                            If errorCode = 551 Or errorCode = 552 Then
                                IsAStateError = True
                                Debug.WriteLine("ProcessHwAlarmDetailsReceived.IsAStateError TRUE")
                            End If
                            ' User Sw
                            If errorCode <> 20 And errorCode <> 21 And errorCode <> 99 Then
                                myErrorCodes.Add(errorCode)
                            End If
                        End If
                    End If
                Next

                'New translation method
                Dim myAlarms = TranslateErrorCodeToAlarmID(Nothing, myErrorCodes)

                If GlobalBase.IsServiceAssembly Then
                    If Not myAlarms.Contains(AlarmEnumerates.Alarms.REACT_MISSING_ERR) Then
                        IsServiceRotorMissingInformed = False
                    End If
                End If

                Dim currentAlarms = New AnalyzerAlarms(Me)
                Dim index As Integer = 0

                'BA-2384: Changed the way of calling function PrepareLocalAlarmList to allow inform the FW Error Code as Additional Information for each Alarm 
                Dim myFwErrorCode = String.Empty

                'Disable state 551 or 552
                If Not IsAStateError AndAlso StatusParameters.IsActive Then
                    StatusParameters.IsActive = False
                    currentAlarms.RemoveAlarmState(StatusParameters.State.ToString())
                    StatusParameters.State = StatusParameters.RotorStates.None
                    'Debug.WriteLine("ProcessHwAlarmDetailsReceived.IsAStateError FALSE")
                End If

                For Each alarmId As Alarms In myAlarms

                    Dim errorCodeId = ""
                    If index <= myErrorCodes.Count - 1 Then
                        errorCodeId = myErrorCodes(index).ToString
                    End If

                    Select Case errorCodeId

                        Case "551", "552"

                            StatusParameters.IsActive = True
                            StatusParameters.RotorStates.TryParse(alarmId.ToString(), StatusParameters.State)
                            currentAlarms.AddNewAlarmState(alarmId.ToString())
                            StatusParameters.LastSaved = DateTime.Now

                        Case "560"

                            CanSendingRepetitions() = True
                            NumSendingRepetitionsTimeout() += 1

                            If NumSendingRepetitionsTimeout() > GlobalBase.MaxRepetitionsRetry Then
                                GlobalBase.CreateLogActivity("FLIGHT Error: GLF_BOARD_FBLD_ERR", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                                CanSendingRepetitions() = False

                                ' Activates Alarm begin
                                CanManageRetryAlarm = True
                            Else
                                GlobalBase.CreateLogActivity("Repeat Start Task Instruction [" & NumSendingRepetitionsTimeout().ToString & "]", "AnalyzerManager.ProcessStatusReceived", EventLogEntryType.Error, False)
                                myGlobal = SendStartTaskinQueue()

                            End If

                            myFwErrorCode = GetFwErrorCodes(alarmId, myErrorCodes) 'BA-2441
                            PrepareLocalAlarmList(alarmId, True, myAlarmsReceivedList, myAlarmsStatusList, myFwErrorCode, myAlarmsAdditionalInfoList, True) 'BA-2384

                        Case Else

                            myFwErrorCode = GetFwErrorCodes(alarmId, myErrorCodes) 'BA-2441
                            PrepareLocalAlarmList(alarmId, True, myAlarmsReceivedList, myAlarmsStatusList, myFwErrorCode, myAlarmsAdditionalInfoList, True) 'BA-2384

                    End Select

                    index += 1
                Next

                ' XBC 17/10/2012 - Alarms treatment for Service
                Dim myFwCodeErrorReceivedList As New List(Of String)
                If GlobalBase.IsServiceAssembly Then
                    ' Initialize Error Codes List
                    myErrorCodesAttribute.Clear()
                    ' Prepare error codes List received from Analyzer
                    PrepareLocalAlarmList_SRV(myErrorCodes, myFwCodeErrorReceivedList)
                End If

                If Not myGlobal.HasError Then
                    If myAlarmsReceivedList.Count > 0 Then
                        '3- Finally call manage all alarms detected (new or fixed)
                        If GlobalBase.IsServiceAssembly Then
                            myGlobal = ManageAlarms_SRV(dbConnection, myAlarmsReceivedList, myAlarmsStatusList, myFwCodeErrorReceivedList, True)
                        Else
                            'BA-2384: Inform optional parameter pAdditionalInfoList = myAlarmsAdditionalInfoList
                            myGlobal = currentAlarms.Manage(myAlarmsReceivedList, myAlarmsStatusList, myAlarmsAdditionalInfoList)
                        End If
                    End If

                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessHwAlarmDetailsReceived", EventLogEntryType.Error, False)
            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (Not dbConnection Is Nothing) Then
                If (Not myGlobal.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    RollbackTransaction(dbConnection)
                End If
                dbConnection.Close()
            End If

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSINF instruction for real time monitoring
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 07/04/2011
        ''' Modified by: XB 29/01/2013 - Changed IsNumeric function by Double.TryParse method for Temperature values (Bugs tracking #1122)
        '''              XB 30/01/2013 - Added Trace log information about malfunctions on temperature values (Bugs tracking #1122)
        '''              SA 19/12/2014 - Replaced sentences DirectCast(CInt(myInstParamTO.ParameterValue), Integer) by CInt(myInstParamTO.ParameterValue)
        '''                              due to the first one is redundant and produces building warnings
        '''              IT 19/12/2014 - BA-2143
        ''' </remarks>
        Private Function ProcessInformationStatusReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO

            ' XBC 24/05/2012 - Declare this flag Private for the class
            'Static Dim ISEAlreadyStarted As Boolean 'provisional flag for starting ISE just the first time SGM 28/03/2012

            Dim myGlobal As New GlobalDataTO

            Try
                'Dim Utilities As New Utilities
                Dim myInstParamTo As New InstructionParameterTO
                Dim mySensors As New Dictionary(Of AnalyzerSensors, Single) 'Local structure
                Dim startTime As DateTime = Now 'AG 11/06/2012 - time estimation

                ' Set Waiting Timer Current Instruction OFF
                If GlobalBase.IsServiceAssembly Then ClearQueueToSend()

                'Obtain all parameters from the instruction received
                If Not GetParametersFromInstructionReceived(pInstructionReceived, myGlobal, myInstParamTo, mySensors) Then
                    Return myGlobal
                End If

                'ResetFbldAlarms()

                Dim myIntValue As Integer

                Dim iseCmdSent As Boolean = False  'AG 06/02/2015 BA-2246

                ' XBC 03/05/2012 - Just for SERVICE, in Stressing mode no check ISE
                If myApplicationName.ToUpper.Contains("SERVICE") AndAlso IsStressingAttribute Then
                    ' Nothing to do
                Else
                    If IsNumeric(myInstParamTo.ParameterValue) Then
                        myIntValue = CInt(myInstParamTo.ParameterValue)
                        mySensors.Add(AnalyzerSensors.ISE_STATUS, CSng(myIntValue))

                        ''SGM 17/02/2012 ISE Module is switched On
                        If ISEAnalyzer IsNot Nothing AndAlso ISEAnalyzer.IsISEModuleInstalled Then
                            If Not ISEAnalyzer.IsISEInitiatedOK OrElse Not ISEAnalyzer.IsISEInitializationDone Then
                                If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then

                                    If myIntValue > 0 Then

                                        ISEAnalyzer.IsISESwitchON = True

                                        If ISEAnalyzer.IsPendingToInitializeAfterActivation Then
                                            ISEAlreadyStarted = True
                                        End If

                                        If Not ISEAlreadyStarted Then
                                            'this condition must be replaced by the 
                                            'condition that allows the ISE module to start connecting
                                            If ISEAnalyzer.IsISESwitchON And Not ISEAnalyzer.IsLongTermDeactivation Then
                                                If Not ISEAnalyzer.IsISEInitiating Then
                                                    ISEAnalyzer.DoGeneralCheckings()
                                                    ISEAlreadyStarted = True
                                                    iseCmdSent = True 'AG 06/02/2015 BA-2246
                                                End If
                                            Else
                                                ISEAlreadyStarted = True
                                            End If

                                            If ISEAnalyzer.IsLongTermDeactivation Then
                                                RefreshISEAlarms()
                                            End If

                                        ElseIf Not ISEAnalyzer.IsISEInitiating And Not ISEAnalyzer.IsLongTermDeactivation Then

                                            'The ISE Module is switched On but its required that user connects from Utilities (Pending to implement)
                                            Dim myAlarmListTmp As New List(Of Alarms)
                                            Dim myAlarmStatusListTmp As New List(Of Boolean)
                                            Dim myAlarmList As New List(Of Alarms)
                                            Dim myAlarmStatusList As New List(Of Boolean)

                                            If Not myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.ISE_CONNECT_PDT_ERR) Then
                                                myAlarmList.Add(AlarmEnumerates.Alarms.ISE_CONNECT_PDT_ERR)
                                                myAlarmStatusList.Add(True)
                                            End If
                                            If Not myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.ISE_OFF_ERR) Then
                                                myAlarmList.Add(AlarmEnumerates.Alarms.ISE_OFF_ERR)
                                                myAlarmStatusList.Add(False)
                                            End If

                                            For i As Integer = 0 To myAlarmListTmp.Count - 1
                                                PrepareLocalAlarmList(myAlarmListTmp(i), myAlarmStatusListTmp(i), myAlarmList, myAlarmStatusList)
                                            Next

                                            If myAlarmList.Count > 0 Then
                                                Dim currentAlarms = New AnalyzerAlarms(Me)
                                                myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)

                                                If Not GlobalBase.IsServiceAssembly Then
                                                    Dim currentAlarmsRetry = New AnalyzerAlarms(Me)
                                                    myGlobal = currentAlarmsRetry.Manage(myAlarmList, myAlarmStatusList)
                                                End If
                                            End If


                                        End If
                                    Else
                                        ISEAnalyzer.IsISESwitchON = False
                                        If Not ISEAlreadyStarted Then
                                            RefreshISEAlarms()
                                        End If

                                        'SGM 09/11/2012 - ISE initiation is finished
                                        If Not ISEAlreadyStarted Then
                                            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                            If GlobalBase.IsServiceAssembly Then
                                                UpdateSensorValuesAttribute(AnalyzerSensors.ISE_CONNECTION_FINISHED, 1, True)
                                            End If
                                        End If
                                        'SGM 09/11/2012

                                        ISEAlreadyStarted = True


                                    End If
                                End If
                            Else
                                If myIntValue = 0 Then
                                    ISEAnalyzer.IsISESwitchON = False
                                End If
                            End If
                        End If

                    End If
                End If

                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                If Not iseCmdSent Then 'AG 06/02/2015 BA-2246
                    If GlobalBase.IsServiceAssembly Then
                        ServiceSwAnsInfTreatment(mySensors)
                    Else
                        UserSwANSINFTreatment(mySensors)
                    End If

                    'AG 09/02/2015 BA-2246 - raise the event only when ANSINF has been treated and the alarms have been processed!!!
                    RaiseEvent ReceivedStatusInformationEventHandler() 'BA-2143
                End If

                If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                    GlobalBase.CreateLogActivity("Treat ANSINF received: " & Now.Subtract(startTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessInformationStatusReceived", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessInformationStatusReceived", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        Private Sub ResetFbldAlarms()
            '560
            CanSendingRepetitions() = False
            NumSendingRepetitionsTimeout() = 0
            CanManageRetryAlarm = False

            '551 and 552
            StatusParameters.IsActive = False
            StatusParameters.LastSaved = Nothing
            StatusParameters.State = StatusParameters.RotorStates.None
        End Sub

        Private Function GetParametersFromInstructionReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef myGlobal As GlobalDataTO, ByRef myInstParamTo As InstructionParameterTO, _
                                                              ByVal mySensors As Dictionary(Of AnalyzerSensors, Single)) As Boolean

            'Get General cover (parameter index 3)
            Dim myIntValue As Integer = 0
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 3)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTo.ParameterValue) Then
                myIntValue = CInt(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.COVER_GENERAL, CSng(myIntValue))
            End If


            'Get Photometrics (Reaccions) cover (parameter index 4)
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 4)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTo.ParameterValue) Then
                myIntValue = CInt(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.COVER_REACTIONS, CSng(myIntValue))
            End If


            'Get Reagents (Fridge) cover (parameter index 5)
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 5)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTo.ParameterValue) Then
                myIntValue = CInt(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.COVER_FRIDGE, CSng(myIntValue))
            End If


            'Get Samples cover (parameter index 6)
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 6)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTo.ParameterValue) Then
                myIntValue = CInt(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.COVER_SAMPLES, CSng(myIntValue))
            End If


            'Get System liquid sensor (parameter index 7)
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 7)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTo.ParameterValue) Then
                myIntValue = CInt(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.WATER_DEPOSIT, CSng(myIntValue))
            End If


            'Get Waste sensor (parameter index 8)
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 8)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTo.ParameterValue) Then
                myIntValue = CInt(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.WASTE_DEPOSIT, CSng(myIntValue))
            End If


            'Get Weight sensors (wash solution & high contamination waste) (parameter index 9, 10)
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 9)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTo.ParameterValue) Then
                myIntValue = CInt(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.BOTTLE_WASHSOLUTION, CSng(myIntValue))
            End If

            myGlobal = GetItemByParameterIndex(pInstructionReceived, 10)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTo.ParameterValue) Then
                myIntValue = CInt(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE, CSng(myIntValue))
            End If


            'Get Temperature Reactions
            Dim mySingleValue As Single = 0
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 11)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            'If IsNumeric(myInstParamTO.ParameterValue) Then    
            If Double.TryParse(myInstParamTo.ParameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                'mySingleValue = CSng(myInstParamTO.ParameterValue)
                mySingleValue = FormatToSingle(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.TEMPERATURE_REACTIONS, mySingleValue)
            Else
                'Dim myLogAcciones2 As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("Input Temperature value not valid [" & myInstParamTo.ParameterValue & "]", "AnalyzerManager.ProcessInformationStatusReceived ", EventLogEntryType.Error, False)
            End If

            'Get Temperature fridge
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 13)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            'If IsNumeric(myInstParamTO.ParameterValue) Then
            If Double.TryParse(myInstParamTo.ParameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                'mySingleValue = CSng(myInstParamTO.ParameterValue)
                mySingleValue = FormatToSingle(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.TEMPERATURE_FRIDGE, mySingleValue)
            Else
                'Dim myLogAcciones2 As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("Input Temperature value not valid [" & myInstParamTo.ParameterValue & "]", "AnalyzerManager.ProcessInformationStatusReceived ", EventLogEntryType.Error, False)
            End If

            'Get Temperature Washing station heater
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 14)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            'If IsNumeric(myInstParamTO.ParameterValue) Then
            If Double.TryParse(myInstParamTo.ParameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                'mySingleValue = CSng(myInstParamTO.ParameterValue)
                mySingleValue = FormatToSingle(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.TEMPERATURE_WASHINGSTATION, mySingleValue)
            Else
                'Dim myLogAcciones2 As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("Input Temperature value not valid [" & myInstParamTo.ParameterValue & "]", "AnalyzerManager.ProcessInformationStatusReceived ", EventLogEntryType.Error, False)
            End If

            'Get Temperature R1 probe
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 15)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            'If IsNumeric(myInstParamTO.ParameterValue) Then
            If Double.TryParse(myInstParamTo.ParameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                'mySingleValue = CSng(myInstParamTO.ParameterValue)
                mySingleValue = FormatToSingle(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.TEMPERATURE_R1, mySingleValue)
            Else
                'Dim myLogAcciones2 As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("Input Temperature value not valid [" & myInstParamTo.ParameterValue & "]", "AnalyzerManager.ProcessInformationStatusReceived ", EventLogEntryType.Error, False)
            End If

            'Get Temperature R2 probe
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 16)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            'If IsNumeric(myInstParamTO.ParameterValue) Then
            If Double.TryParse(myInstParamTo.ParameterValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                'mySingleValue = CSng(myInstParamTO.ParameterValue)
                mySingleValue = FormatToSingle(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.TEMPERATURE_R2, mySingleValue)
            Else
                'Dim myLogAcciones2 As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("Input Temperature value not valid [" & myInstParamTo.ParameterValue & "]", "AnalyzerManager.ProcessInformationStatusReceived ", EventLogEntryType.Error, False)
            End If

            'Get Fridge status (parameter index 12)
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 12)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            If IsNumeric(myInstParamTo.ParameterValue) Then
                myIntValue = CInt(myInstParamTo.ParameterValue)
                mySensors.Add(AnalyzerSensors.FRIDGE_STATUS, CSng(myIntValue))
            End If

            'Get ISE status (parameter index 17)
            myGlobal = GetItemByParameterIndex(pInstructionReceived, 17)
            If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                myInstParamTo = DirectCast(myGlobal.SetDatos, InstructionParameterTO)
            Else
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' SW has received and ANSCBR instruction 
        ''' </summary>
        ''' <param name="pInstructionReceived">List of objects InstructionParameterTO containing all values in the ANSCBR Instruction
        '''                                    received from analyzer</param>
        ''' <param name="pRotorType">By Reference (output) parameter. It is received empty and returned with the Rotor Type read by the Barcode Reader</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 22/06/2011 
        ''' Modified by: SA 11/06/2013 - When load the WSRotorContentByPositionDS that has to be passed as parameter when calling function ManageBarcodeInstruction
        '''                              in BarcodeWSDelegate, inform field WSStatus with value of WorkSessionStatusAttribute (needed to update the WS Status from 
        '''                              OPEN to PENDING when the WS Required Elements are created after scanning the Samples Rotor -> new functionality added for
        '''                              LIS with ES)
        '''              SA 18/12/2014 - BA-1999 ==> In the call to function PrepareUIRefreshEventNum2, if RotorType is REAGENTS, inform value of parameters for 
        '''                                          RealValue and RemainingTestsNumber when these fields are informed for the position in the DS returned by 
        '''                                          function ManageBarcodeInstruction. Additionally, when function GetItemByParameterIndex returns an Integer value, 
        '''                                          the way of get the returned value has been changed: instead of use DirectCast(CInt(value), Integer) - This has 
        '''                                          not sense!, use CType(value, Integer). Replaced all String.Compare by comparisons using = 
        ''' </remarks>
        Private Function ProcessCodeBarInstructionReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO), ByRef pRotorType As String) As GlobalDataTO
            Dim dbConnection As New SqlConnection
            Dim myGlobal As New GlobalDataTO

            Try
                myGlobal = GetOpenDBTransaction(Nothing)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobal.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'AG 11/10/2011 - Check if the pInstructionReceived parameters are correct or not
                        Dim auxStr() As String = InstructionReceivedAttribute.Split(CChar(";"))
                        Dim parameterSeparatorsNumber As Integer = auxStr.Length - 1

                        If (pInstructionReceived.Count <> parameterSeparatorsNumber) Then
                            'There is some ';' character inside data. The instruction InstructionReceivedAttribute requires a new decode with additional business

                            'Get the samples barcode full total size configured
                            Dim sampleBarcodeFullTotal As Integer
                            Dim settingsDlg As New UserSettingsDelegate

                            myGlobal = settingsDlg.GetCurrentValueBySettingID(dbConnection, UserSettingsEnum.BARCODE_FULL_TOTAL.ToString())
                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                sampleBarcodeFullTotal = CType(myGlobal.SetDatos, Integer)

                                Dim myLax00 As New LAX00Interpreter
                                myGlobal = myLax00.ReadWithAdditionalBusiness(InstructionReceivedAttribute, AppLayerInstrucionReception.ANSCBR, _
                                                                              sampleBarcodeFullTotal)
                                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                    Dim myParameterTOList As List(Of ParametersTO) = CType(myGlobal.SetDatos, List(Of ParametersTO))

                                    Dim myInstruction As New Instructions
                                    myGlobal = myInstruction.GenerateReception(myParameterTOList)
                                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                        pInstructionReceived = DirectCast(myGlobal.SetDatos, List(Of InstructionParameterTO))
                                    End If
                                End If
                            End If
                        End If
                        'AG 11/10/2011

                        Dim errorFlag As Boolean = True
                        'Dim Utilities As New Utilities
                        Dim myInstParamTO As New InstructionParameterTO

                        'Get Reader selector (parameter index 3)
                        Dim rotorSelected As Integer = 0
                        myGlobal = GetItemByParameterIndex(pInstructionReceived, 3)
                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

                            If (IsNumeric(myInstParamTO.ParameterValue)) Then
                                rotorSelected = CType(myInstParamTO.ParameterValue, Integer)
                                errorFlag = False
                            End If
                        End If

                        If (errorFlag) Then
                            myGlobal.HasError = True
                            Exit Try
                        End If


                        'Get Status (parameter index 4)
                        Dim barCodeStatus As Integer = 0
                        myGlobal = GetItemByParameterIndex(pInstructionReceived, 4)
                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

                            If (IsNumeric(myInstParamTO.ParameterValue)) Then
                                barCodeStatus = CType(myInstParamTO.ParameterValue, Integer)
                                errorFlag = False
                            End If
                        End If

                        If (errorFlag) Then
                            myGlobal.HasError = True
                            Exit Try
                        End If


                        'Get Number of Reads (parameter index 5)
                        Dim readsNumber As Integer = 0
                        myGlobal = GetItemByParameterIndex(pInstructionReceived, 5)
                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

                            If (IsNumeric(myInstParamTO.ParameterValue)) Then
                                readsNumber = CType(myInstParamTO.ParameterValue, Integer)
                                errorFlag = False
                            End If
                        End If

                        If (errorFlag) Then
                            myGlobal.HasError = True
                            Exit Try
                        End If

                        Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                        Select Case barCodeStatus
                            Case Ax00AnsCodeBarStatus.CONFIG_DONE
                                'Configuration done OK
                                ' XBC 13/02/2012 - CODEBR Configuration instruction
                                'If myApplicationName.ToUpper.Contains("SERVICE") Then
                                'Service Sw
                                'TODO business
                                'Else
                                'User Sw
                                'If connection process in process then sent the general config instruction
                                'If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" Then

                                'Read the general configuration and sent the CONFIG instruction
                                'The analyzer answers with a Status instruction action = config done
                                If (rotorSelected = Ax00CodeBarReader.SAMPLES) Then
                                    'When Codebar SAMPLES CONFIG_DONE is received then Codebar REAGENTS CONFIG instruction is sent
                                    Dim BarCodeDS As New AnalyzerManagerDS
                                    Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow

                                    rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
                                    With rowBarCode
                                        .RotorType = "REAGENTS"
                                        .Action = Ax00CodeBarAction.CONFIG
                                        .Position = 0
                                    End With
                                    BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
                                    BarCodeDS.AcceptChanges()

                                    myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS)

                                ElseIf (rotorSelected = Ax00CodeBarReader.REAGENTS) Then
                                    'SG 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                    If (GlobalBase.IsServiceAssembly) Then
                                        'Service SW
                                        myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.CONFIG, True)
                                    Else
                                        'User Sw
                                        'When Codebar REAGENTS CONFIG_DONE is received then General CONFIG instruction is sent
                                        If (mySessionFlags(AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS") Then
                                            myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.CONFIG, True)

                                        ElseIf (mySessionFlags(AnalyzerManagerFlags.WUPprocess.ToString) = "INPROCESS") Then
                                            'UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.WUPprocess, "CLOSED")
                                            'myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.CONFIG, True)
                                            'ValidateWarmUpProcess(myAnalyzerFlagsDS, WarmUpProcessFlag.ConfigureBarCode) 'BA-2075 
                                            RaiseEvent ProcessFlagEventHandler(AnalyzerManagerFlags.Barcode) 'BA-2288
                                        End If
                                    End If
                                End If

                                'SG 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                If (GlobalBase.IsServiceAssembly) Then
                                    'Service Sw - Nothing by now
                                Else
                                    'User Sw
                                    If (Not myGlobal.HasError AndAlso ConnectedAttribute) Then
                                        'AG + TR 03/02/2014 - BT #1490 (if change barcode config in sleep the action button becomes disable)
                                        'SA + AG 11/12/2012 - Not integrated in v1.0.0 
                                        'If (rotorSelected = GlobalEnumerates.Ax00CodeBarReader.SAMPLES) Then 'After configurate SAMPLES barcode the SW configures automatically the REAGENTS barcode 
                                        'SetAnalyzerNotReady()
                                        'End If
                                        'SA + AG 11/12/2012 

                                        'Leave Analyzer ready only in sleeping mode and after received reagents rotor
                                        If Not (AnalyzerStatusAttribute = AnalyzerManagerStatus.SLEEPING AndAlso rotorSelected = Ax00CodeBarReader.REAGENTS) Then
                                            SetAnalyzerNotReady()
                                        End If
                                        'AG + TR 03/02/2014
                                    End If
                                End If

                                'XBC 13/02/2012 - CODEBR Configuration instruction
                            Case Ax00AnsCodeBarStatus.CODEBAR_ERROR
                                'Code bar reader error
                                'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                If (GlobalBase.IsServiceAssembly) Then
                                    'Service Sw
                                    'TODO business
                                    'Else
                                    'User Sw
                                    'TODO business
                                End If


                            Case Ax00AnsCodeBarStatus.FULL_ROTOR_DONE, _
                                 Ax00AnsCodeBarStatus.SINGLE_POS_DONE, _
                                 Ax00AnsCodeBarStatus.TEST_MODE_ANSWER, _
                                 Ax00AnsCodeBarStatus.TEST_MODE_ENDED
                                'XBC 28/03/2012 - Add Answers TEST_MODE_ANSWER, TEST_MODE_ENDED
                                'Code bar readings results ... save them into twksRotorContentByPosition table
                                Dim barCodeDS As New WSRotorContentByPositionDS
                                Dim row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                                Const offset As Integer = 3
                                For itera As Integer = 0 To readsNumber - 1
                                    'Get Number of Reads (parameter index 6)
                                    Dim readPosition As Integer = 0
                                    myGlobal = GetItemByParameterIndex(pInstructionReceived, 6 + itera * offset)
                                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                        myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

                                        If (IsNumeric(myInstParamTO.ParameterValue)) Then
                                            readPosition = CType(myInstParamTO.ParameterValue, Integer)
                                            errorFlag = False
                                        End If
                                    End If

                                    If (errorFlag) Then
                                        myGlobal.HasError = True
                                        Exit Try
                                    End If

                                    'Get diagnostics (parameter index 7)
                                    Dim diagnostics As Integer = 0
                                    myGlobal = GetItemByParameterIndex(pInstructionReceived, 7 + itera * offset)
                                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                        myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

                                        If (IsNumeric(myInstParamTO.ParameterValue)) Then
                                            diagnostics = CType(myInstParamTO.ParameterValue, Integer)
                                            errorFlag = False
                                        End If
                                    End If

                                    If (errorFlag) Then
                                        myGlobal.HasError = True
                                        Exit Try
                                    End If

                                    'Get value (parameter index 8)
                                    Dim value As String = String.Empty
                                    myGlobal = GetItemByParameterIndex(pInstructionReceived, 8 + itera * offset)
                                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                        myInstParamTO = DirectCast(myGlobal.SetDatos, InstructionParameterTO)

                                        value = myInstParamTO.ParameterValue
                                        errorFlag = False
                                    End If

                                    If (errorFlag) Then
                                        myGlobal.HasError = True
                                        Exit Try
                                    End If

                                    'Add into dataset
                                    row = barCodeDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow
                                    row.AnalyzerID = AnalyzerIDAttribute
                                    row.WorkSessionID = WorkSessionIDAttribute
                                    row.WSStatus = WorkSessionStatusAttribute   'SA 11/06/2013: Inform status of the active WorkSession
                                    row.CellNumber = readPosition

                                    If (rotorSelected = Ax00CodeBarReader.REAGENTS) Then
                                        row.RotorType = "REAGENTS"
                                    Else
                                        row.RotorType = "SAMPLES"
                                    End If
                                    pRotorType = row.RotorType    'Inform the byRef parameter!!

                                    row.SetBarCodeInfoNull()
                                    row.SetBarcodeStatusNull()
                                    row.SetScannedPositionNull()

                                    Select Case diagnostics
                                        Case Ax00AnsCodeBarDiagnosis.OK
                                            row.BarcodeStatus = "OK"
                                            row.BarCodeInfo = value
                                            row.ScannedPosition = True

                                        Case Ax00AnsCodeBarDiagnosis.NO_READ
                                            row.SetBarCodeInfoNull()
                                            row.BarcodeStatus = "EMPTY"
                                            row.SetScannedPositionNull()

                                        Case Ax00AnsCodeBarDiagnosis.BAD_CODE
                                            row.BarcodeStatus = "ERROR"
                                            row.ScannedPosition = True
                                            row.BarCodeInfo = value 'Save the barcode read value
                                    End Select
                                    barCodeDS.twksWSRotorContentByPosition.AddtwksWSRotorContentByPositionRow(row)
                                Next
                                barCodeDS.AcceptChanges()

                                Dim myRotorType As String = "SAMPLES"
                                Dim myStatus As String = String.Empty
                                Dim myTubeType As String = String.Empty
                                Dim myBarcodeInfo As String = String.Empty
                                Dim myTubeContent As String = String.Empty
                                Dim myElementStatus As String = String.Empty
                                Dim myBarcodeStatus As String = String.Empty

                                Dim myElementId As Integer = -1
                                Dim myTestsLeft As Integer = -1
                                Dim myRealVolume As Single = -1
                                Dim myCellNumber As Integer = -1    'XBC 16/12/2011
                                Dim myMultiTubeNumber As Integer = -1
                                Dim myScannedPosition As Boolean = Nothing

                                'SG 01/02/2012 - Check if it is Service Assembly - Bug #1112
                                If (GlobalBase.IsServiceAssembly) Then
                                    'Service SW management

                                    'XBC 16/12/2011
                                    For Each row In barCodeDS.twksWSRotorContentByPosition.Rows
                                        If (row.IsRotorTypeNull) Then myRotorType = String.Empty Else myRotorType = row.RotorType
                                        If (row.IsCellNumberNull) Then myCellNumber = -1 Else myCellNumber = row.CellNumber
                                        If (row.IsStatusNull) Then myStatus = String.Empty Else myStatus = row.Status
                                        If (row.IsElementStatusNull) Then myElementStatus = String.Empty Else myElementStatus = row.ElementStatus
                                        If (row.IsBarCodeInfoNull) Then myBarcodeInfo = String.Empty Else myBarcodeInfo = row.BarCodeInfo
                                        If (row.IsBarcodeStatusNull) Then myBarcodeStatus = String.Empty Else myBarcodeStatus = row.BarcodeStatus
                                        If (row.IsScannedPositionNull) Then myScannedPosition = Nothing Else myScannedPosition = row.ScannedPosition
                                        If (row.IsElementIDNull) Then myElementId = -1 Else myElementId = row.ElementID
                                        If (row.IsMultiTubeNumberNull) Then myMultiTubeNumber = -1 Else myMultiTubeNumber = row.MultiTubeNumber
                                        If (row.IsTubeTypeNull) Then myTubeType = String.Empty Else myTubeType = row.TubeType
                                        If (row.IsTubeContentNull) Then myTubeContent = String.Empty Else myTubeContent = row.TubeContent

                                        'Inform all fields but RealVolume and TestLeft (SERVICE SW requires only RotorType, CellNumber and BarcodeInfo,
                                        'but the rest of fields are informed because the function PrepareUIRefreshEventNum2 needs them)
                                        myGlobal = PrepareUIRefreshEventNum2(dbConnection, UI_RefreshEvents.BARCODE_POSITION_READ, _
                                                                             myRotorType, myCellNumber, myStatus, myElementStatus, -1, -1, _
                                                                             myBarcodeInfo, myBarcodeStatus, Nothing, Nothing, myScannedPosition, myElementId, _
                                                                             myMultiTubeNumber, myTubeType, myTubeContent)
                                    Next
                                    'XBC 16/12/2011
                                Else
                                    'User SW management
                                    'AG temporally uncommented 'Initial instruction testings
                                    'Update twksWSRotorContentByPosition barcode & barcode status
                                    'If barCodeDS.twksWSRotorContentByPosition.Rows.Count > 0 Then
                                    '    Dim rotorDelegate As New WSRotorContentByPositionDelegate
                                    '    myGlobal = rotorDelegate.UpdateBarCodeFields(dbConnection, barCodeDS, False)
                                    'End If
                                    'AG temporally uncommented

                                    'AG temporally commented when uncoment previous code
                                    Dim bcWSDelegate As New BarcodeWSDelegate
                                    If (rotorSelected = Ax00CodeBarReader.REAGENTS) Then myRotorType = "REAGENTS"

                                    'Generate the UIRefresh DataSET (only for the positions read)
                                    myGlobal = bcWSDelegate.ManageBarcodeInstruction(dbConnection, barCodeDS, myRotorType)
                                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                        Dim currentRotorTypeContentDS As WSRotorContentByPositionDS = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS) 'The current whole rotor (by rotortype)

                                        'The UIRefresh event has only the rotor positions read by barcode
                                        Dim linqRes As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                        For Each row In barCodeDS.twksWSRotorContentByPosition.Rows
                                            linqRes = (From a As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In currentRotorTypeContentDS.twksWSRotorContentByPosition _
                                                      Where a.RotorType = row.RotorType AndAlso a.CellNumber = row.CellNumber _
                                                     Select a).ToList

                                            If (linqRes.Count > 0) Then
                                                If (linqRes(0).IsStatusNull) Then myStatus = String.Empty Else myStatus = linqRes(0).Status
                                                If (linqRes(0).IsElementStatusNull) Then myElementStatus = String.Empty Else myElementStatus = linqRes(0).ElementStatus
                                                If (linqRes(0).IsBarCodeInfoNull) Then myBarcodeInfo = String.Empty Else myBarcodeInfo = linqRes(0).BarCodeInfo
                                                If (linqRes(0).IsBarcodeStatusNull) Then myBarcodeStatus = String.Empty Else myBarcodeStatus = linqRes(0).BarcodeStatus
                                                If (linqRes(0).IsScannedPositionNull) Then myScannedPosition = Nothing Else myScannedPosition = linqRes(0).ScannedPosition
                                                If (linqRes(0).IsElementIDNull) Then myElementId = -1 Else myElementId = linqRes(0).ElementID
                                                If (linqRes(0).IsMultiTubeNumberNull) Then myMultiTubeNumber = -1 Else myMultiTubeNumber = linqRes(0).MultiTubeNumber
                                                If (linqRes(0).IsTubeTypeNull) Then myTubeType = String.Empty Else myTubeType = linqRes(0).TubeType
                                                If (linqRes(0).IsTubeContentNull) Then myTubeContent = String.Empty Else myTubeContent = linqRes(0).TubeContent

                                                'BA-1999: For Positions in Reagents Rotor, keep values of RealVolume and RemainingTestsNumber when they are informed
                                                myTestsLeft = -1
                                                myRealVolume = -1
                                                If (myRotorType = "REAGENTS") Then
                                                    If (Not linqRes(0).IsRealVolumeNull) Then myRealVolume = linqRes(0).RealVolume
                                                    If (Not linqRes(0).IsRemainingTestsNumberNull) Then myTestsLeft = linqRes(0).RemainingTestsNumber
                                                End If

                                                'Inform all fields, including RealVolume and TestLeft when they are informed for the Rotor Position
                                                myGlobal = PrepareUIRefreshEventNum2(dbConnection, UI_RefreshEvents.BARCODE_POSITION_READ, _
                                                                                     linqRes(0).RotorType, linqRes(0).CellNumber, myStatus, myElementStatus, _
                                                                                     myRealVolume, myTestsLeft, myBarcodeInfo, myBarcodeStatus, Nothing, Nothing, myScannedPosition, _
                                                                                     myElementId, myMultiTubeNumber, myTubeType, myTubeContent)
                                            End If
                                            If (myGlobal.HasError) Then Exit For
                                        Next
                                        linqRes = Nothing
                                    End If
                                    'AG temporally commented when uncoment previous code
                                End If
                        End Select

                        'XB + AG 05/03/2012 - Update analyzer session flags into DataBase
                        If (myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0) Then
                            Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                            myGlobal = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessCodeBarInstructionReceived", EventLogEntryType.Error, False)
            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (Not dbConnection Is Nothing) Then
                If (Not myGlobal.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    RollbackTransaction(dbConnection)
                End If
                dbConnection.Close()
            End If
            Return myGlobal
        End Function

        ''' <summary>
        ''' The last sent preparation has been accepted by Analyzer
        ''' Insert the sent preparation into the twksWSPreparations table and also update 
        ''' the twksWSExecutions table (fields PreparationID and ExecutionStatus)
        '''         ''' </summary>
        ''' <param name="pDBConnection">Opened DB Connection</param>
        ''' <returns> GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 16/04/2010
        ''' Modified by: AG 17/05/2010 - Changes + using of connection template
        '''              AG 26/11/2010 - Prepare the UI_Refresh events
        '''              SA 02/07/2012 - Implementation changed to improve performance
        '''              SA 10/07/2012 - Inform SampleClass = "" when calling function GetAffectedISEExecutions in ExecutionsDelegate
        '''              AG 18/11/2013 - Increment the number of tests using the positions for sample and reagent2
        ''' </remarks>
        Private Function MarkPreparationAccepted(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                Dim myExecutionsDS As New ExecutionsDS
                'Dim affectedExecutions As New ExecutionsDS
                Dim exeDelegate As New ExecutionsDelegate

                'If the Preparation to sent is an ISE one, get all affected Executions (all ISE Tests requested for the same 
                'Patient/Control and Sample Type 
                If (AppLayer.LastInstructionTypeSent = AppLayerEventList.SEND_ISE_PREPARATION) Then
                    myGlobal = exeDelegate.GetAffectedISEExecutions(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, AppLayer.LastExecutionIDSent, "", True)

                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        myExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                    End If
                End If

                If (Not myGlobal.HasError) Then
                    'Begin TRANSACTION to create the new PreparationID and update all affected Executions
                    myGlobal = GetOpenDBTransaction(pDBConnection)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(myGlobal.SetDatos, SqlConnection)
                        If (Not dbConnection Is Nothing) Then
                            Dim myNextPreparationID = InsertNextPreparation(dbConnection)

                            'Dim myPreparationDS As New WSPreparationsDS
                            'Dim myPrepDelegate As New WSPreparationsDelegate
                            'Dim prepRow As WSPreparationsDS.twksWSPreparationsRow

                            ''Generate the next PreparationID and insert the new Preparation
                            'myGlobal = myPrepDelegate.GeneratePreparationID(dbConnection)
                            'If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            '    myNextPreparationID = CType(myGlobal.SetDatos, Integer)

                            '    'Prepare data to insert the new Preparation
                            '    prepRow = myPreparationDS.twksWSPreparations.NewtwksWSPreparationsRow
                            '    prepRow.AnalyzerID = AnalyzerIDAttribute
                            '    prepRow.WorkSessionID = WorkSessionIDAttribute
                            '    prepRow.PreparationID = myNextPreparationID
                            '    prepRow.LAX00Data = AppLayer.LastPreparationInstructionSent
                            '    prepRow.PreparationStatus = "INPROCESS"

                            '    'AG 20/09/2012 v052 - inform the well where the reactions is dispensed on (when PTEST apply the correct offset factor)
                            '    prepRow.WellUsed = CurrentWellAttribute
                            '    If InStr(prepRow.LAX00Data, "PTEST") > 0 Then
                            '        Dim reactRotorDlg As New ReactionsRotorDelegate
                            '        prepRow.WellUsed = reactRotorDlg.GetRealWellNumber(CurrentWellAttribute + WELL_OFFSET_FOR_PREDILUTION, MAX_REACTROTOR_WELLS)
                            '    End If

                            '    myPreparationDS.twksWSPreparations.AddtwksWSPreparationsRow(prepRow)

                            '    'Insert the new Preparation
                            '    myGlobal = myPrepDelegate.AddWSPreparation(dbConnection, myPreparationDS)
                            'End If

                            If (Not myGlobal.HasError) Then
                                If (AppLayer.LastInstructionTypeSent = AppLayerEventList.SEND_ISE_PREPARATION) Then
                                    'Update fields ExecutionStatus and PreparationID for all Executions returned when called function GetAffectedISEExecutions
                                    For Each affectedExecution As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                        affectedExecution.BeginEdit()
                                        affectedExecution.ExecutionStatus = "INPROCESS"
                                        affectedExecution.PreparationID = myNextPreparationID
                                        affectedExecution.EndEdit()
                                    Next
                                Else
                                    'Prepare the DS needed to update fields ExecutionStatus and PreparationID for the informed Execution
                                    Dim exeRow As ExecutionsDS.twksWSExecutionsRow
                                    exeRow = myExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow
                                    exeRow.ExecutionID = AppLayer.LastExecutionIDSent
                                    exeRow.ExecutionStatus = "INPROCESS"
                                    exeRow.PreparationID = myNextPreparationID
                                    myExecutionsDS.twksWSExecutions.AddtwksWSExecutionsRow(exeRow)
                                End If
                            End If

                            'AG 22/05/2014 #1634 - NOTE, NO CHANGES! Here we should set executions flags (ValidReadings = TRUE and CompleteReadings = TRUE) but
                            'method UpdateStatus is called in more places, so we decide to initiate these flags when 1st reading is received (method ProcessBiochemicalReadingsNEW)
                            'Flags ThermoWarningFlag and ClotValue will be reevaluate when readings are received too

                            'Update fields ExecutionStatus and PreparationID for all Executions in DS ExecutionsDS and Commit or Rollback the opened DB TRANSACTION
                            myGlobal = exeDelegate.UpdateStatus(dbConnection, myExecutionsDS)

                            'AG 28/05/2014 - #1644 - Do not delete readings here!! They will be removed when the new preparation receives his 1st reading
                            ''AG 02/10/2012 - When the execution is updated to INPROCESS remove all his readings (only for STD_TESTS)
                            'If Not myGlobal.HasError Then
                            '    Dim readingsDlg As New WSReadingsDelegate
                            '    myGlobal = readingsDlg.Delete(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myExecutionsDS)
                            'End If

                            If (Not myGlobal.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then CommitTransaction(dbConnection)
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If

                If (Not myGlobal.HasError) Then
                    'Notify to the application interfase the Status of some Executions has changed
                    For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                        myGlobal = PrepareUIRefreshEvent(Nothing, UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
                        If myGlobal.HasError Then Exit For
                    Next
                End If

                'Update status of all tubes of the Patient Sample/Control positioned in SAMPLES Rotor: PENDING, INPROCESS, FINISHED
                If (Not myGlobal.HasError) Then
                    Dim rcpDelegate As New WSRotorContentByPositionDelegate
                    myGlobal = rcpDelegate.UpdateSamplePositionStatus(Nothing, AppLayer.LastExecutionIDSent, WorkSessionIDAttribute, AnalyzerIDAttribute)

                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        Dim rotorPosDS As WSRotorContentByPositionDS = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS)

                        For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rotorPosDS.twksWSRotorContentByPosition.Rows
                            myGlobal = PrepareUIRefreshEventNum2(Nothing, UI_RefreshEvents.ROTORPOSITION_CHANGED, "SAMPLES", row.CellNumber, _
                                                                 row.Status, "", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                            If (myGlobal.HasError) Then Exit For
                        Next
                    End If
                End If

                'AG 18/11/2013 - (#1385) Increment the number of tests that will be used positions samples / r2
                If Not myGlobal.HasError Then
                    Dim inProcessDlg As New WSRotorPositionsInProcessDelegate
                    myGlobal = inProcessDlg.IncrementInProcessTestsNumber(Nothing, AnalyzerIDAttribute, AppLayer.LastPreparationInstructionSent)
                End If

            Catch ex As Exception
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then RollbackTransaction(dbConnection)

                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.MarkPreparationAccepted", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobal

        End Function


        ''' <summary>
        ''' After send a wash well (cuvette) due a well contaminated inform into twksWSReactionsRotor the washedFlag = True
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 07/06/2011
        ''' AG 18/11/2013 - Increment the number of tests using the positions for arm reagent2 washings
        ''' </remarks>
        Public Function MarkWashWellContaminationRunningAccepted(ByVal pDBConnection As SqlConnection) As GlobalDataTO Implements IAnalyzerManager.MarkWashWellContaminationRunningAccepted
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                resultData = GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myReactionsDS As New ReactionsRotorDS
                        Dim newRow As ReactionsRotorDS.twksWSReactionsRotorRow
                        newRow = myReactionsDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow
                        With newRow
                            .AnalyzerID = AnalyzerIDAttribute
                            .WellNumber = wellContaminatedWithWashSentAttr
                            .SetRotorTurnNull()
                            .SetWellContentNull()
                            .SetWellStatusNull()
                            .SetExecutionIDNull()
                            .SetRejectedFlagNull()
                            .SetCurrentTurnFlagNull()
                            .WashedFlag = True
                            .SetWashRequiredFlagNull()
                        End With
                        myReactionsDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(newRow)
                        myReactionsDS.AcceptChanges()

                        Dim reactionsDelegate As New ReactionsRotorDelegate
                        resultData = reactionsDelegate.Update(dbConnection, myReactionsDS, True)

                        'AG 18/11/2013 - (#1385) Increment the number of tests that will be used positions for arm R2 washing
                        If Not resultData.HasError Then
                            Debug.Print("Step 3 - MarkWashWellContaminationRunningAccepted -> LastPreparationInstructionSent = " & AppLayer.LastPreparationInstructionSent)

                            Dim inProcessDlg As New WSRotorPositionsInProcessDelegate
                            resultData = inProcessDlg.IncrementInProcessTestsNumber(Nothing, AnalyzerIDAttribute, AppLayer.LastPreparationInstructionSent)
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.MarkWashWellContaminationRunningAccepted", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add the last exported results into the object attribute
        ''' </summary>
        ''' <param name="pExecutionsDS"></param>
        ''' <remarks>AG 19/03/2013</remarks>
        Private Sub AddIntoLastExportedResults(ByVal pExecutionsDS As ExecutionsDS)
            Try
                'Move row from parameter to lastExportedResultsDSAttribute
                SyncLock lockThis
                    For Each row As ExecutionsDS.twksWSExecutionsRow In pExecutionsDS.twksWSExecutions.Rows
                        lastExportedResultsDSAttribute.twksWSExecutions.ImportRow(row)
                    Next
                    lastExportedResultsDSAttribute.twksWSExecutions.AcceptChanges()
                End SyncLock

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.AddIntoLastExportedResults", EventLogEntryType.Error, False)

            End Try
        End Sub

        ''' <summary>
        ''' Process reception of ANSFBLD instruction
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>AG 31/10/2014 BA-2062
        '''             AG 11/12/2014 BA-2170 update the flag DynamicBL_Read to "MIDDLE" when sw receives ANSFBLD instruction
        '''             AG 15/01/2015 cancel the new value MIDDLE, it is not required</remarks>
        Private Function ProcessANSFBLDReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                ''AG 11/12/2014 BA-2170 inform that some ANSFBLD instruction has been received
                'Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                'UpdateSessionFlags(myAnalyzerFlagsDS, GlobalEnumerates.AnalyzerManagerFlags.DynamicBL_Read, "MIDDLE")
                'If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                '    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                '    resultData = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                'End If

                If Not resultData.HasError Then
                    'AG 11/12/2014 BA-2170

                    'Decode and get the results of the instruction
                    Dim myInstruction As New Instructions
                    resultData = myInstruction.DecodeANSFBLDReceived(pInstructionReceived)

                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                        Dim myResults As New DynamicBaseLineTO
                        myResults = DirectCast(resultData.SetDatos, DynamicBaseLineTO)
                        If myResults.WellUsed.Count = myResults.MainLight.Count AndAlso myResults.WellUsed.Count = myResults.RefLight.Count Then

                            'Prepare BaseLinesDS to save
                            Dim myBaseLineDS As New BaseLinesDS
                            Dim nextBaseLineID As Integer = 0
                            Dim baseLineRow As BaseLinesDS.twksWSBaseLinesRow

                            If Not resultData.HasError Then
                                For index As Integer = 0 To myResults.WellUsed.Count - 1
                                    'Get the baseLineID
                                    resultData = GetNextBaseLineID(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, myResults.WellUsed(index), True, BaseLineType.DYNAMIC.ToString, myResults.Wavelength)
                                    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                        nextBaseLineID = DirectCast(resultData.SetDatos, Integer)
                                    Else
                                        Exit For
                                    End If

                                    baseLineRow = myBaseLineDS.twksWSBaseLines.NewtwksWSBaseLinesRow
                                    baseLineRow.BeginEdit()
                                    baseLineRow.AnalyzerID = AnalyzerIDAttribute
                                    baseLineRow.BaseLineID = nextBaseLineID
                                    baseLineRow.WorkSessionID = WorkSessionIDAttribute
                                    baseLineRow.Wavelength = myResults.Wavelength
                                    baseLineRow.WellUsed = myResults.WellUsed(index)
                                    baseLineRow.MainLight = myResults.MainLight(index)
                                    baseLineRow.RefLight = myResults.RefLight(index)
                                    baseLineRow.MainDark = myResults.MainDark
                                    baseLineRow.RefDark = myResults.RefDark
                                    baseLineRow.IT = myResults.IntegrationTime
                                    baseLineRow.DAC = myResults.DAC
                                    baseLineRow.DateTime = DateTime.Now
                                    baseLineRow.Type = BaseLineType.DYNAMIC.ToString 'AG 28/10/2014 BA-2062
                                    baseLineRow.EndEdit()
                                    myBaseLineDS.twksWSBaseLines.AddtwksWSBaseLinesRow(baseLineRow)
                                Next
                                myBaseLineDS.AcceptChanges()

                                'Save baseline results into database
                                resultData = Me.SaveBaseLineResults(Nothing, myBaseLineDS, True, BaseLineType.DYNAMIC.ToString)
                            End If

                        Else
                            'Error, invalid structure for the instruction
                        End If

                    End If
                End If 'AG 11/12/2014 BA-2170

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessANSFBLDReceived", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Method used to process the data that are read from the FLIGHT instruction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: IT 26/11/2014 - BA-2075 Modified the Warm up Process to add the FLIGHT process
        ''' AG 27/11/2014 BA-2066
        ''' AG 28/11/2014 BA-2066 reorganize code in order to refresh monitor when valid results and when much times invalid!!!
        ''' IT 19/12/2014 - BA-2143 (Accessibility Level)
        ''' AC BA-2437
        ''' </remarks>
        Public Function ProcessFlightReadAction() As Boolean Implements IAnalyzerManager.ProcessFlightReadAction

            Dim validResults As Boolean = True
            Dim myGlobal As New GlobalDataTO

            Dim AlarmList As New List(Of Alarms)
            Dim AlarmStatusList As New List(Of Boolean)
            Dim alarmStatus As Boolean = False 'By default no alarm
            Dim myAlarm As Alarms = AlarmEnumerates.Alarms.NONE

            '1. Validate results (BA-2081)
            myGlobal = BaseLine.ValidateDynamicBaseLinesResults(Nothing, AnalyzerIDAttribute)
            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                validResults = DirectCast(myGlobal.SetDatos, Boolean)
            ElseIf myGlobal.HasError Then
                validResults = False
            End If


            If validResults Then
                ResetBaseLineFailuresCounters() 'AG 27/11/2014 BA-2066
            Else
                dynamicbaselineInitializationFailuresAttribute += 1 'AG 27/11/2014 BA-2066 - Increment tentatives number
            End If

            '3.1 Prepare data for the 1st reactions rotor turn in worksession when valid results OR when max tentatives KO (BA-2066)
            If validResults OrElse dynamicbaselineInitializationFailuresAttribute >= FLIGHT_INIT_FAILURES Then
                myGlobal = ProcessDynamicBaseLine(Nothing, WorkSessionIDAttribute, 1)
                'Error or  max tentatives -- generate alarm
                If myGlobal.HasError OrElse dynamicbaselineInitializationFailuresAttribute >= FLIGHT_INIT_FAILURES Then
                    myAlarm = AlarmEnumerates.Alarms.BASELINE_INIT_ERR
                    alarmStatus = True
                ElseIf Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myAlarm = CType(myGlobal.SetDatos, Alarms)
                    If myAlarm <> AlarmEnumerates.Alarms.NONE Then alarmStatus = True
                End If

            End If

            'If still not active, generate base line alarm (error)
            If myAlarm <> AlarmEnumerates.Alarms.NONE Then
                PrepareLocalAlarmList(myAlarm, alarmStatus, AlarmList, AlarmStatusList)
                If AlarmList.Count > 0 Then
                    If GlobalBase.IsServiceAssembly Then
                        'Alarms treatment for Service
                    Else
                        Dim StartTime As DateTime = Now 'AG 05/06/2012 - time estimation
                        Dim currentAlarms = New AnalyzerAlarms(Me)
                        myGlobal = currentAlarms.Manage(AlarmList, AlarmStatusList)
                        GlobalBase.CreateLogActivity("Alarm generated during dynamic base line convertion to well rejection): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessFlightReadAction", EventLogEntryType.Information, False) 'AG 28/06/2012
                    End If
                End If
            End If

            validFLIGHTAttribute = validResults
            Return validResults

        End Function

        ''' <summary>
        ''' Method used to finalize the Warm up process.
        ''' </summary>
        ''' <remarks>
        ''' Created by: IT 26/11/2014 - BA-2075 Modified the Warm up Process to add the FLIGHT process
        ''' </remarks>
        <Obsolete("Use WarmUpService instead")>
        Private Function FinalizeWarmUpProcess() As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
            Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

            Dim AlarmList As New List(Of Alarms)
            Dim AlarmStatusList As New List(Of Boolean)
            Dim myAlarm As Alarms

            Dim wupManeuversFinishFlag As Boolean = False 'AG 23/05/2012

            'WUPCOMPLETEFLAG
            myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
            With myAnalyzerSettingsRow
                .AnalyzerID = AnalyzerIDAttribute
                .SettingID = AnalyzerSettingsEnum.WUPCOMPLETEFLAG.ToString()
                .CurrentValue = "1"
            End With
            myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

            myGlobalDataTO = AnalyzerSettingsDelegate.Save(Nothing, AnalyzerIDAttribute, myAnalyzerSettingsDS, Nothing)

            wupManeuversFinishFlag = True

            ' XBC 13/02/2012 - CODEBR Configuration instruction
            'myGlobalDataTO = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.CONFIG, True) 'AG 24/11/2011 - If Wup process FINISH sent again the config instruction (maybe user has changed something)

            Dim BarCodeDS As New AnalyzerManagerDS
            Dim rowBarCode As AnalyzerManagerDS.barCodeRequestsRow
            rowBarCode = BarCodeDS.barCodeRequests.NewbarCodeRequestsRow
            With rowBarCode
                .RotorType = "SAMPLES"
                .Action = Ax00CodeBarAction.CONFIG
                .Position = 0
            End With
            BarCodeDS.barCodeRequests.AddbarCodeRequestsRow(rowBarCode)
            BarCodeDS.AcceptChanges()
            myGlobalDataTO = ManageAnalyzer(AnalyzerManagerSwActionList.BARCODE_REQUEST, True, Nothing, BarCodeDS)
            ' XBC 13/02/2012 - CODEBR Configuration instruction

            'AG 23/05/2012
            If wupManeuversFinishFlag Then
                'Inform the presentation layer to activate the STOP wup button
                UpdateSensorValuesAttribute(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED, 1, True)
                ISEAnalyzer.IsAnalyzerWarmUp = False 'AG 22/05/2012 - ISE alarms ready to be shown

                'AG 23/05/2012 - Evaluate if Sw has to recommend to change reactions rotor (remember all alarms are remove when Start Instruments starts)
                Dim analyzerReactRotor As New AnalyzerReactionsRotorDelegate
                myGlobalDataTO = analyzerReactRotor.ChangeReactionsRotorRecommended(Nothing, AnalyzerIDAttribute, myAnalyzerModel)
                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                    If CType(myGlobalDataTO.SetDatos, String) <> "" Then
                        myAlarm = AlarmEnumerates.Alarms.BASELINE_WELL_WARN
                        'Update internal alarm list if exists alarm but not saved it into database!!!
                        'But generate refresh
                        If Not myAlarmListAttribute.Contains(myAlarm) Then
                            myAlarmListAttribute.Add(myAlarm)
                            myGlobalDataTO = PrepareUIRefreshEvent(Nothing, UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, myAlarm.ToString, True)
                        End If
                    End If
                End If
            End If
            'AG 23/05/2012

            'AG 16/05/2012 - If warm up maneuvers are finished check for the ise alarms 
            If SensorValuesAttribute.ContainsKey(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) AndAlso SensorValuesAttribute(AnalyzerSensors.WARMUP_MANEUVERS_FINISHED) = 1 Then

                Dim tempISEAlarmList As New List(Of Alarms)
                Dim tempISEAlarmStatusList As New List(Of Boolean)
                myGlobalDataTO = ISEAnalyzer.CheckAlarms(Connected, tempISEAlarmList, tempISEAlarmStatusList)

                AlarmList.Clear()
                AlarmStatusList.Clear()
                For i As Integer = 0 To tempISEAlarmList.Count - 1
                    PrepareLocalAlarmList(tempISEAlarmList(i), tempISEAlarmStatusList(i), AlarmList, AlarmStatusList)
                Next

                If AlarmList.Count > 0 Then
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        ' XBC 16/10/2012 - Alarms treatment for Service
                        ' Not Apply
                        'myGlobalDataTO = ManageAlarms_SRV(dbConnection, AlarmList, AlarmStatusList)
                    Else
                        Dim currentAlarms = New AnalyzerAlarms(Me)
                        myGlobalDataTO = currentAlarms.Manage(AlarmList, AlarmStatusList)
                    End If
                End If
            End If
            'AG 16/05/2012

            Return myGlobalDataTO

        End Function


        ''' <summary>
        ''' Reset the baseline failed tentatives (ALIGHT and FLIGHT) when valid results are received
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' Created by:  AG 27/11/2014 - BA-2066
        ''' Modified by: IT 19/12/2014 - BA-2143 (Accessibility Level)
        ''' </remarks>
        Public Sub ResetBaseLineFailuresCounters() Implements IAnalyzerManager.ResetBaseLineFailuresCounters
            baselineInitializationFailuresAttribute = 0 'Reset ALIGHT failures counter
            dynamicbaselineInitializationFailuresAttribute = 0 'Reset FLIGHT failures counter
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="alarmId"></param>
        ''' <param name="errorCodesList"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetFwErrorCodes(ByVal alarmId As Alarms, ByVal errorCodesList As List(Of Integer)) As String

            Dim myFwErrorCode = String.Empty
            Dim myFwErrorCodeList As List(Of AlarmsDS.tfmwAlarmsRow)

            'Search in the global DataSet alarmsDefinitionTableDS, the FW Error Code for the Alarm; the FW Error Code has to exists in myErrorCodes List
            'to avoid errors due to an AlarmID can be linked to several FW Error Codes
            myFwErrorCodeList = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms _
                                Where a.AlarmID = alarmId.ToString() AndAlso errorCodesList.Contains(a.ErrorCode) _
                                Select a).ToList()

            'If an Alarm is linked to several FW Error Codes and more than one of them is in the ANSERR Instruction, then they are 
            'saved in AdditionalInfo field divided by commas
            For Each fwCode As AlarmsDS.tfmwAlarmsRow In myFwErrorCodeList
                If (myFwErrorCode <> String.Empty) Then myFwErrorCode &= ","
                myFwErrorCode &= fwCode.ErrorCode.ToString()
            Next

            Return myFwErrorCode

        End Function

#End Region

#Region "UIRefresh DataSet Methods"

        ''' <summary>
        ''' Prepare the data for the UI refreh due a instruction reception
        ''' (Signature n#1)
        ''' NOTE: This method has several signatures
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"> The UI_RefreshType will determine the screens to refresh</param>
        ''' <param name="pExecutionID">Used when new execution status changes or new calculation is performed</param>
        ''' <param name="pReadingNumber">Used when new readings are received</param>
        ''' <param name="pAlarmID">Used when alarms occurs or solved</param>
        ''' <param name="pAlarmStatus" >Used when alarms occurs (TRUE) or is solved (FALSE)</param>
        ''' <returns>globalDataTo indicating if there has been error or not</returns>
        ''' <remarks>25/11/2010 AG: EXECUTION_STATUS case
        ''' AG 09/02/2011 - add NEW READINGS RECEIVED case 
        ''' AG 14/03/2011 - add NEW_ALARMS_RECEIVED case 
        ''' AG 22/05/2014 - #1637 Reorder code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Public Function PrepareUIRefreshEvent(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                               ByVal pExecutionID As Integer, ByVal pReadingNumber As Integer, _
                                               ByVal pAlarmID As String, ByVal pAlarmStatus As Boolean) As GlobalDataTO Implements IAnalyzerManager.PrepareUIRefreshEvent
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing

            Try
                myglobal = GetOpenDBConnection(pDBConnection)
                If (Not myglobal.HasError And Not myglobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myglobal.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        eventDataPendingToTriggerFlag = True 'AG 07/10/2011 - exists information in UI_RefreshDS pending to be send to the event

                        'AG 22/05/2014 - #1637 reorder code to use easily SyncLock
                        'Next code was inside Select Case
                        Dim local_DS As New UIRefreshDS
                        If pUI_EventType = UI_RefreshEvents.EXECUTION_STATUS OrElse pUI_EventType = UI_RefreshEvents.RESULTS_CALCULATED Then
                            Dim exeDelegate As New ExecutionsDelegate
                            myglobal = exeDelegate.GetExecutionStatusChangeInfo(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, pExecutionID)
                            If Not myglobal.HasError And Not myglobal.SetDatos Is Nothing Then
                                local_DS = CType(myglobal.SetDatos, UIRefreshDS)
                            End If
                        End If
                        'AG 22/05/2014

                        If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)
                        Select Case pUI_EventType
                            'Case (execution sent), (execution calculated)
                            Case UI_RefreshEvents.EXECUTION_STATUS, UI_RefreshEvents.RESULTS_CALCULATED
                                If Not myglobal.HasError And Not myglobal.SetDatos Is Nothing Then
                                    'Import new rows into the general class DataSet
                                    If local_DS.ExecutionStatusChanged.Rows.Count > 0 Then
                                        'AG 14/03/2014 - #1533
                                        'For Each row As UIRefreshDS.ExecutionStatusChangedRow In local_DS.ExecutionStatusChanged.Rows
                                        '    myUI_RefreshDS.ExecutionStatusChanged.ImportRow(row)
                                        'Next
                                        SyncLock myUI_RefreshDS.ExecutionStatusChanged 'AG 22/05/2014 - #1637 Use exclusive lock over myUI_RefreshDS variables
                                            myUI_RefreshDS.ExecutionStatusChanged.Merge(local_DS.ExecutionStatusChanged)
                                            'AG 14/03/2014 - #1533
                                            myUI_RefreshDS.ExecutionStatusChanged.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                                        End SyncLock
                                    End If
                                End If

                                'Case (new result instruction has been received)
                            Case UI_RefreshEvents.READINGS_RECEIVED
                                Dim myNewReadRow As UIRefreshDS.ReceivedReadingsRow
                                SyncLock myUI_RefreshDS.ReceivedReadings 'AG 22/05/2014 - #1637 Use exclusive lock over myUI_RefreshDS variables
                                    myNewReadRow = myUI_RefreshDS.ReceivedReadings.NewReceivedReadingsRow
                                    With myNewReadRow
                                        .BeginEdit()
                                        .WorkSessionID = WorkSessionIDAttribute
                                        .AnalyzerID = AnalyzerIDAttribute
                                        .ExecutionID = pExecutionID
                                        .ReadingNumber = pReadingNumber
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.ReceivedReadings.AddReceivedReadingsRow(myNewReadRow)
                                    myUI_RefreshDS.ReceivedReadings.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                                End SyncLock

                                'Case (new alarms instruction has been received)
                            Case UI_RefreshEvents.ALARMS_RECEIVED
                                Dim myNewAlarmRow As UIRefreshDS.ReceivedAlarmsRow
                                SyncLock myUI_RefreshDS.ReceivedAlarms 'AG 22/05/2014 - #1637 Use exclusive lock over myUI_RefreshDS variables
                                    myNewAlarmRow = myUI_RefreshDS.ReceivedAlarms.NewReceivedAlarmsRow
                                    With myNewAlarmRow
                                        .BeginEdit()
                                        .WorkSessionID = WorkSessionIDAttribute
                                        .AnalyzerID = AnalyzerIDAttribute
                                        .AlarmID = pAlarmID
                                        .AlarmStatus = pAlarmStatus                                        
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.ReceivedAlarms.AddReceivedAlarmsRow(myNewAlarmRow)
                                    myUI_RefreshDS.ReceivedAlarms.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                                End SyncLock
                        End Select                            
                    End If
                End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEvent", EventLogEntryType.Error, False)

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            Return myglobal
        End Function

        ''' <summary>
        ''' Prepare the data for the UI refreh due a instruction reception
        ''' (Signature n#2)
        ''' NOTE: This method has several signatures
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"> The UI_RefreshType will determine the screens to refresh</param>
        ''' <param name="pRotorType" ></param>
        ''' <param name="pCellNumber" ></param>
        ''' <param name="pStatus" ></param>
        ''' <param name="pElementStatus" ></param>
        ''' <param name="pRealVolume" ></param>
        ''' <param name="pTestsLeft" ></param>
        ''' <param name="pBarCodeInfo" ></param>
        ''' <param name="pBarCodeStatus" ></param>
        ''' <param name="pSensorID" ></param>
        ''' <param name="pSensorValue" ></param>
        ''' <param name="pScannedPosition" ></param>
        ''' <param name="pElementID" ></param>
        ''' <param name="pMultiTubeNumber" ></param>
        ''' <param name="pTubeType" ></param>
        ''' <param name="pTubeContent" ></param>
        ''' <returns>globalDataTo indicating if there has been error or not</returns>
        ''' <remarks>25/11/2010 AG 
        ''' AG 30/03/2011 - add case ROTORPOSITION_CHANGED (use new method signature)
        ''' add case SENSORVALUE_CHANGED 
        ''' AG 22/09/2011 - add case BARCODE_POSITION_READ
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Public Function PrepareUIRefreshEventNum2(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                               ByVal pRotorType As String, ByVal pCellNumber As Integer, ByVal pStatus As String, ByVal pElementStatus As String, _
                                               ByVal pRealVolume As Single, ByVal pTestsLeft As Integer, ByVal pBarCodeInfo As String, ByVal pBarCodeStatus As String, _
                                               ByVal pSensorId As AnalyzerSensors, ByVal pSensorValue As Single, _
                                               ByVal pScannedPosition As Boolean, ByVal pElementID As Integer, ByVal pMultiTubeNumber As Integer, _
                                               ByVal pTubeType As String, ByVal pTubeContent As String) As GlobalDataTO Implements IAnalyzerManager.PrepareUIRefreshEventNum2
            Dim myglobal As New GlobalDataTO
            'Dim dbConnection As SqlConnection = Nothing

            Try
                eventDataPendingToTriggerFlag = True 'AG 07/10/2011 - exists information in UI_RefreshDS pending to be send to the event
                If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)

                Select Case pUI_EventType
                    'Case (some rotor positions status, volume, ... has changed)
                    Case UI_RefreshEvents.ROTORPOSITION_CHANGED, UI_RefreshEvents.BARCODE_POSITION_READ
                        Dim myNewRotorPositionRow As UIRefreshDS.RotorPositionsChangedRow
                        'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                        SyncLock myUI_RefreshDS.RotorPositionsChanged
                            myNewRotorPositionRow = myUI_RefreshDS.RotorPositionsChanged.NewRotorPositionsChangedRow
                            With myNewRotorPositionRow
                                .BeginEdit()
                                .WorkSessionID = WorkSessionIDAttribute
                                .AnalyzerID = AnalyzerIDAttribute
                                .RotorType = pRotorType
                                .CellNumber = pCellNumber
                                If pStatus <> "" Then .Status = pStatus Else .SetStatusNull()
                                If String.Compare(pElementStatus, "", False) <> 0 Then .ElementStatus = pElementStatus Else .SetElementStatusNull()
                                If pRealVolume <> -1 Then .RealVolume = pRealVolume Else .SetRealVolumeNull()
                                If pTestsLeft <> -1 Then .RemainingTestsNumber = pTestsLeft Else .SetRemainingTestsNumberNull()
                                If pBarCodeInfo <> "" Then .BarCodeInfo = pBarCodeInfo Else .SetBarCodeInfoNull()
                                If pBarCodeStatus <> "" Then .BarcodeStatus = pBarCodeStatus Else .SetBarcodeStatusNull()
                                If pScannedPosition = Nothing Then .SetScannedPositionNull() Else .ScannedPosition = pScannedPosition
                                If pElementID <> -1 Then .ElementID = pElementID Else .SetElementIDNull()
                                If pMultiTubeNumber <> -1 Then .MultiTubeNumber = pMultiTubeNumber Else .SetMultiTubeNumberNull()
                                If pTubeType <> "" Then .TubeType = pTubeType Else .SetTubeTypeNull()
                                If pTubeContent <> "" Then .TubeContent = pTubeContent Else .SetTubeContentNull()
                                .EndEdit()
                            End With
                            myUI_RefreshDS.RotorPositionsChanged.AddRotorPositionsChangedRow(myNewRotorPositionRow)
                            myUI_RefreshDS.RotorPositionsChanged.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                        End SyncLock

                    Case UI_RefreshEvents.SENSORVALUE_CHANGED
                        'This case prepare DS 
                        'NOTE it's not necessary due the values are available using method AnalyzerManager.GetSensorValue

                        If pSensorId <> Nothing Then
                            Dim myNewSensorChangeRow As UIRefreshDS.SensorValueChangedRow

                            'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables

                            Dim lnqRes As IEnumerable(Of UIRefreshDS.SensorValueChangedRow)
                            SyncLock myUI_RefreshDS.SensorValueChanged  'Este SyncLock no está bien tratado. Objeto no global no readonly!!
                                lnqRes = (From a As UIRefreshDS.SensorValueChangedRow In myUI_RefreshDS.SensorValueChanged
                                          Where String.Compare(a.SensorID, pSensorId.ToString, False) = 0
                                          Select a)

                                If lnqRes.Count > 0 Then
                                    lnqRes(0).BeginEdit()
                                    lnqRes(0).Value = pSensorValue
                                    lnqRes(0).EndEdit()
                                    lnqRes(0).AcceptChanges()

                                Else
                                    ' insert new value
                                    myNewSensorChangeRow = myUI_RefreshDS.SensorValueChanged.NewSensorValueChangedRow
                                    With myNewSensorChangeRow
                                        .BeginEdit()
                                        .SensorID = pSensorId.ToString
                                        .Value = pSensorValue
                                        .EndEdit()
                                    End With
                                    myUI_RefreshDS.SensorValueChanged.AddSensorValueChangedRow(myNewSensorChangeRow)
                                    myUI_RefreshDS.SensorValueChanged.AcceptChanges()
                                End If
                            End SyncLock
                        End If

                    Case Else

                End Select
                'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex) '.Message, "AnalyzerManager.PrepareUIRefreshEventNum2", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Prepare the data for the UI refreh due a instruction reception
        ''' (Signature n#3)
        ''' NOTE: This method has several signatures
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"> The UI_RefreshType will determine the screens to refresh</param>
        ''' <param name="pReactionsRotorWellDS" ></param>
        ''' <param name="pMainThreadIsUsedFlag">TRUE indicates instruction treatment is using the main thread (arm status instructions, change status business is executed) 
        '''                                     FALSE indicates instruction treatment is using a secondary thread (ANSPHR used for well rejection)</param>
        ''' <returns>globalDataTo indicating if there has been error or not</returns>
        ''' <remarks>
        ''' AG 03/06/2011 - add case REACTIONS_WELL_STATUS_CHANGED (use new method signature)
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Public Function PrepareUIRefreshEventNum3(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                               ByVal pReactionsRotorWellDS As ReactionsRotorDS, ByVal pMainThreadIsUsedFlag As Boolean) As GlobalDataTO Implements IAnalyzerManager.PrepareUIRefreshEventNum3
            Dim myglobal As New GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing

            Try
                'AG 03/07/2012 - Running Cycles lost - Solution!
                If Not pReactionsRotorWellDS Is Nothing Then

                    'AG 22/05/2014 - #1637 Clear code. Comment dead code
                    'If pMainThreadIsUsedFlag Then
                    '    eventDataPendingToTriggerFlag = True 'exists information in UI_RefreshDS pending to be send to the event
                    '    If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)
                    'Else
                    '    secondaryEventDataPendingToTriggerFlag = True 'exists information in UI_RefreshDS pending to be send to the event
                    '    If Not mySecondaryUI_RefreshEvent.Contains(pUI_EventType) Then mySecondaryUI_RefreshEvent.Add(pUI_EventType)
                    'End If
                    eventDataPendingToTriggerFlag = True 'exists information in UI_RefreshDS pending to be send to the event
                    If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)
                    'AG 22/05/2014 - #1637

                    'Case (some reactions rotor well ... has changed)
                    Dim myNewRow As UIRefreshDS.ReactionWellStatusChangedRow
                    For Each row As ReactionsRotorDS.twksWSReactionsRotorRow In pReactionsRotorWellDS.twksWSReactionsRotor.Rows

                        'AG 22/05/2014 - #1637 Clear code. Comment dead code
                        'If pMainThreadIsUsedFlag Then
                        '    myNewRow = myUI_RefreshDS.ReactionWellStatusChanged.NewReactionWellStatusChangedRow
                        'Else
                        '    myNewRow = mySecondaryUI_RefreshDS.ReactionWellStatusChanged.NewReactionWellStatusChangedRow
                        'End If
                        'AG 22/05/2014 - #1637

                        'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                        SyncLock myUI_RefreshDS.ReactionWellStatusChanged
                            myNewRow = myUI_RefreshDS.ReactionWellStatusChanged.NewReactionWellStatusChangedRow

                            With myNewRow
                                .BeginEdit()
                                If Not row.IsAnalyzerIDNull Then .AnalyzerID = row.AnalyzerID
                                If Not row.IsWellNumberNull Then .WellNumber = row.WellNumber
                                If Not row.IsRotorTurnNull Then .RotorTurn = row.RotorTurn
                                If Not row.IsWellContentNull Then .WellContent = row.WellContent
                                If Not row.IsWellStatusNull Then .WellStatus = row.WellStatus
                                If Not row.IsExecutionIDNull Then .ExecutionID = row.ExecutionID
                                If Not row.IsRejectedFlagNull Then .RejectedFlag = row.RejectedFlag
                                .EndEdit()
                            End With

                            'AG 22/05/2014 - #1637 Clear code. Comment dead code
                            'If pMainThreadIsUsedFlag Then
                            '    myUI_RefreshDS.ReactionWellStatusChanged.AddReactionWellStatusChangedRow(myNewRow)
                            'Else
                            '    mySecondaryUI_RefreshDS.ReactionWellStatusChanged.AddReactionWellStatusChangedRow(myNewRow)
                            'End If
                            myUI_RefreshDS.ReactionWellStatusChanged.AddReactionWellStatusChangedRow(myNewRow)
                            'AG 22/05/2014 - #1637

                            myUI_RefreshDS.ReactionWellStatusChanged.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                        End SyncLock

                    Next

                    'AG 22/05/2014 - #1637 Clear code. Comment dead code
                    'If pMainThreadIsUsedFlag Then
                    '    myUI_RefreshDS.AcceptChanges()
                    'Else
                    '    mySecondaryUI_RefreshDS.AcceptChanges()
                    'End If
                    'AG 22/05/2014 - #1637 Clear code. Comment dead code

                    'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

                End If

                'AG 03/07/2012 - Running Cycles lost - Solution!
                'End If
                'End If

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEventNum3", EventLogEntryType.Error, False)
                'Finally

            End Try

            'We have used Exit Try so we have to sure the connection becomes properly closed here
            'If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()'AG 03/07/2012 - Running Cycles lost - Solution!
            Return myglobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUI_EventType"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWS"></param>
        ''' <param name="pOrderList"></param>
        ''' <returns></returns>
        ''' <remarks> Created ??
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function PrepareUIRefreshEventNum4(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                               ByVal pAnalyzerID As String, ByVal pWS As String, ByVal pOrderList As List(Of String)) As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            'Dim dbConnection As SqlConnection = Nothing

            Try
                'If (Not dbConnection Is Nothing) Then
                eventDataPendingToTriggerFlag = True 'exists information in UI_RefreshDS pending to be send to the event
                If Not myUI_RefreshEvent.Contains(pUI_EventType) Then myUI_RefreshEvent.Add(pUI_EventType)

                Dim myNewRow As UIRefreshDS.AutoReportRow

                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                SyncLock myUI_RefreshDS.AutoReport
                    For Each order As String In pOrderList
                        myNewRow = myUI_RefreshDS.AutoReport.NewAutoReportRow()
                        With myNewRow
                            .BeginEdit()
                            .AnalyzerID = pAnalyzerID
                            .OrderID = order
                            '.OrderTestID = cal??
                            .WorkSessionID = pWS
                            '.RerunNumber = cal??
                            .EndEdit()
                        End With
                        myUI_RefreshDS.AutoReport.AddAutoReportRow(myNewRow)
                        myUI_RefreshDS.AutoReport.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                    Next
                End SyncLock
                'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

            Catch ex As Exception
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.PrepareUIRefreshEventNum4", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Clear the user interface refresh information but with functionality for lock multithreads
        ''' </summary>
        ''' <param name="pMultiThreadLock">Use SyncLock or not</param>
        ''' <param name="pForceClear">Clear always or only if no refresh to perform</param>
        ''' <remarks>AG 22/05/2014 - #1637</remarks>
        Private Sub ClearRefreshDataSets(ByVal pMultiThreadLock As Boolean, ByVal pForceClear As Boolean)
            Try
                If pMultiThreadLock Then
                    SyncLock LockThis
                        If pForceClear Then 'Clear always
                            myUI_RefreshEvent.Clear()
                            myUI_RefreshDS.Clear()
                        ElseIf myUI_RefreshEvent.Count = 0 Then 'Clear only if no refresh to perform
                            myUI_RefreshDS.Clear()
                        End If
                    End SyncLock
                Else
                    If pForceClear Then
                        myUI_RefreshEvent.Clear()
                        myUI_RefreshDS.Clear()
                    ElseIf myUI_RefreshEvent.Count = 0 Then
                        myUI_RefreshDS.Clear()
                    End If
                End If
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ClearRefreshDataSets", EventLogEntryType.Error, False)
            End Try
        End Sub

#Region "NEW ALARM TYPE"

        ''' <summary>
        ''' Method to refresh UI for new alarm Type.
        ''' </summary>
        ''' <param name="_uirefresevent"></param>
        ''' <remarks></remarks>
        Public Sub Createandthroweventuirefresh(Optional _uirefresevent As UI_RefreshEvents = UI_RefreshEvents.NONE) Implements IAnalyzerManager.CreateAndThrowEventUiRefresh
            Dim InstructionReceivedAttribute = ""

            If myUI_RefreshEvent.Count = 0 Then
                ClearRefreshDataSets(True, False)
            Else
                If _uirefresevent <> UI_RefreshEvents.NONE Then
                    If Not myUI_RefreshEvent.Contains(_uirefresevent) Then
                        myUI_RefreshEvent.Add(_uirefresevent)
                    End If

                    If _myIsBlExpired Then
                        PrepareUINewAlarmType(AlarmEnumerates.Alarms.BASELINE_EXPIRED)
                    End If
                End If
            End If

            RaiseEvent ReceptionEvent(InstructionReceivedAttribute, False, myUI_RefreshEvent, myUI_RefreshDS, True)
            RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)
        End Sub

        ''' <summary>
        ''' Function that allow us add new type alarm to DS which one will show every time we refresh the screen. 
        ''' </summary>
        ''' <param name="_alarm"></param>
        ''' <remarks></remarks>
        Public Sub PrepareUINewAlarmType(ByVal _alarm As AlarmEnumerates.Alarms) Implements IAnalyzerManager.PrepareUINewAlarmType
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                resultData = GetOpenDBConnection(Nothing)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim alarmIsCreated = (From a In myUI_RefreshDS.ReceivedAlarms Where a.AlarmID = _alarm.ToString And a.AlarmStatus = True).ToList().Count()
                        If alarmIsCreated = 0 Then
                            resultData = PrepareUIRefreshEvent(dbConnection, UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, _alarm.ToString, True)
                        End If
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)
            End Try
        End Sub

#End Region

#End Region

#Region "ISE (Send and Receive Results)"

        ''' <summary>
        ''' Method incharge to process all Received ISE Results.
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  TR 03/01/2010
        ''' Modified by: SA 12/06/2014 - BT #1660 ==> Replaced call to function ProcessISETESTResults in ISEReception class, for its new 
        '''                                           version (ProcessISETESTResultsNEW) 
        '''               XB 30/09/2014 - Implement Start Task Timeout for ISE commands - Remove too restrictive limitations because timeouts - BA-1872
        '''               XB 19/11/2014 - Emplace the kind of errors derived of BA-1614 inside the management of timeouts and automatic instructions repetitions - BA-1872
        '''               XB 21/01/2015 - Refresh Alarms about ISE calibrations and clean - BA-1873
        ''' </remarks>
        Public Function ProcessRecivedISEResult(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO Implements IAnalyzerManager.ProcessRecivedISEResult
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing

            Try
                'AG 03/07/2012 - Running Cycles lost - Solution!

                Dim myPreparationID As Integer = -1
                Dim myISEResultStr As String = ""
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                myPreparationID = CInt((From a In pInstructionReceived Where a.ParameterIndex = 3 Select a.ParameterValue).First)
                myISEResultStr = (From a In pInstructionReceived Where a.ParameterIndex = 4 Select a.ParameterValue).First()

                Dim myISEResult As New ISEResultTO
                myISEResult.ReceivedResults = myISEResultStr

                Dim myISEMode As String = "SimpleMode"
                Dim myParams As New SwParametersDelegate
                myGlobalDataTO = myParams.ReadTextValueByParameterName(dbConnection, SwParameters.ISE_MODE.ToString, Nothing)
                If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
                    myISEMode = CStr(myGlobalDataTO.SetDatos)
                Else
                    myISEMode = "SimpleMode"
                End If

                Dim myISEResultsDelegate As New ISEReception(Me)

                If myPreparationID = 0 Then

                    ISEAnalyzer.LastISEResult = New ISEResultTO

                    'ISECMD answer
                    Dim myCurrentProcedure As ISEManager.ISEProcedures = ISEAnalyzer.CurrentProcedure
                    myGlobalDataTO = myISEResultsDelegate.ProcessISECMDResults(myISEResult, AnalyzerIDAttribute, ISEAnalyzer.IsBiosystemsValidationMode)
                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                        ISEAnalyzer.LastISEResult = CType(myGlobalDataTO.SetDatos, ISEResultTO)
                        'SGM 17/02/2012 ISE Communications Ok
                        If ISEAnalyzer IsNot Nothing Then
                            If ISEAnalyzer.IsCommErrorDetected Then
                                'ISEAnalyzer.IsISECommsOk = False       ' XB 30/09/2014 - BA-1872
                                ISEAnalyzer.IsCommErrorDetected = False
                            Else
                                ISEAnalyzer.IsISECommsOk = True
                            End If
                        End If
                        'end SGM 17/02/2012
                    Else
                        'SGM 11/05/2012
                        Dim myISEResultWithErrors As ISEResultTO = New ISEResultTO
                        myISEResultWithErrors.ISEResultType = ISEResultTO.ISEResultTypes.SwError
                        ISEAnalyzer.LastISEResult = myISEResultWithErrors
                    End If

                    'SGM 27/07/2012 - Activate alarmas related to ERC errors (or CALB results error)
                    If Not ISEAnalyzer.IsInUtilities Then
                        If ISEAnalyzer.LastISEResult IsNot Nothing Then
                            If ISEAnalyzer.LastISEResult.IsCancelError Then
                                Select Case ISEAnalyzer.LastISEResult.Errors(0).ErrorCycle
                                    Case ISEErrorTO.ErrorCycles.Calibration, ISEErrorTO.ErrorCycles.PumpCalibration, ISEErrorTO.ErrorCycles.BubbleDetCalibration
                                        Me.ActivateAnalyzerISEAlarms(ISEAnalyzer.LastISEResult)
                                End Select
                            ElseIf ISEAnalyzer.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL Then
                                If ISEAnalyzer.LastISEResult.Errors.Count > 0 Then
                                    For Each E As ISEErrorTO In ISEAnalyzer.LastISEResult.Errors
                                        If myCurrentProcedure = ISEManager.ISEProcedures.CalibrateElectrodes Then
                                            BlockISEPreparationByElectrode(dbConnection, ISEAnalyzer.LastISEResult, WorkSessionIDAttribute, AnalyzerIDAttribute)
                                        End If
                                    Next
                                End If
                            End If

                        End If


                    End If

                    'AG 12/01/2012 - Prepare UI Refresh
                    UpdateSensorValuesAttribute(AnalyzerSensors.ISECMD_ANSWER_RECEIVED, 1, True)

                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        'SERVICE SOFTWARE 01/07/2011
                        'TODO

                    Else
                        'USER SOFTWARE
                        If String.Equals(mySessionFlags(AnalyzerManagerFlags.ISEClean.ToString), "INI") Then
                            If Not ISEAnalyzer.LastISEResult.IsCancelError Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISEClean, "END")
                            Else
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISEClean, "CANCELED")
                            End If

                        ElseIf String.Equals(mySessionFlags(AnalyzerManagerFlags.ISEPumpCalib.ToString), "INI") Then
                            If Not ISEAnalyzer.LastISEResult.IsCancelError Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISEPumpCalib, "END")
                            Else
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISEPumpCalib, "CANCELED")
                            End If

                        ElseIf String.Equals(mySessionFlags(AnalyzerManagerFlags.ISECalibAB.ToString), "INI") Then
                            If Not ISEAnalyzer.LastISEResult.IsCancelError Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISECalibAB, "END")
                            Else
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISECalibAB, "CANCELED")
                            End If

                            'The ISE consumption flag is updated in OnISEProcedureFinished event handler

                        End If

                        If ISEAnalyzer.LastISEResult.IsCancelError Then
                            'Mark ise auto conditioning process as finished
                            If String.Equals(mySessionFlags(AnalyzerManagerFlags.ISEConditioningProcess.ToString), "INPROCESS") Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISEConditioningProcess, "CLOSED")
                            End If

                            'Cases
                            '1) Performing ISE utilities: Inform about the error
                            '2) Performing ISE auto conditioning process: Abort it
                            UpdateSensorValuesAttribute(AnalyzerSensors.ISE_WARNINGS, 1, True)
                            UpdateSensorValuesAttribute(AnalyzerSensors.BEFORE_ENTER_RUNNING, 1, True) 'Process finished due ISE conditioning warnings

                        Else
                            'Cases
                            '1) Performing ISE utilities: Nothing
                            '2) Performing ISE auto conditioning process: Call PerformAutoIseconditioning
                            If String.Equals(mySessionFlags(AnalyzerManagerFlags.ISEConditioningProcess.ToString), "INPROCESS") Then
                                If String.Equals(mySessionFlags(AnalyzerManagerFlags.ISECalibAB.ToString), "END") Then
                                    UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISEConditioningProcess, "CLOSED")
                                End If
                                myGlobalDataTO = PerformAutoIseConditioning(dbConnection)
                            End If
                        End If

                    End If

                ElseIf myPreparationID > 0 Then
                    'ISETEST answer
                    ISEAnalyzer.LastISEResult = New ISEResultTO

                    'BT #1660 - Call the new version of function ProcessISETESTResults
                    myGlobalDataTO = myISEResultsDelegate.ProcessISETESTResultsNEW(dbConnection, myPreparationID, myISEResult, myISEMode, WorkSessionIDAttribute, AnalyzerIDAttribute)

                    'UI refresh dataset (new results AND new rotor position status)
                    If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then

                        ISEAnalyzer.LastISEResult = myISEResult 'SGM 12/01/2012
                        'SGM 17/02/2012 ISE Communications Ok
                        If ISEAnalyzer IsNot Nothing Then
                            ISEAnalyzer.IsISECommsOk = True
                        End If
                        'end SGM 17/02/2012

                        Dim myExecDS As New ExecutionsDS
                        myExecDS = CType(myGlobalDataTO.SetDatos, ExecutionsDS)
                        For Each ex_row As ExecutionsDS.twksWSExecutionsRow In myExecDS.twksWSExecutions.Rows
                            myGlobalDataTO = PrepareUIRefreshEvent(dbConnection, UI_RefreshEvents.RESULTS_CALCULATED, ex_row.ExecutionID, 0, Nothing, False)

                            'AG 02/12/2011 - UIRefresh sample rotor position status
                            'Get the ordertestStatus for the Execution
                            Dim myOTDelegate As New OrderTestsDelegate
                            Dim myOTDS As New OrderTestsDS
                            myGlobalDataTO = myOTDelegate.GetOrderTest(dbConnection, ex_row.OrderTestID)
                            myOTDS = CType(myGlobalDataTO.SetDatos, OrderTestsDS)

                            'When orderTestStatus is closed call method UpdateSamplePositionStatus in RotorContentByPositionDelegate class
                            If String.Equals(myOTDS.twksOrderTests(0).OrderTestStatus, "CLOSED") Then
                                Dim rcp_del As New WSRotorContentByPositionDelegate
                                myGlobalDataTO = rcp_del.UpdateSamplePositionStatus(dbConnection, ex_row.ExecutionID, WorkSessionIDAttribute, AnalyzerIDAttribute)
                                If Not myGlobalDataTO.HasError Then
                                    Dim rotorPosDS As New WSRotorContentByPositionDS
                                    rotorPosDS = CType(myGlobalDataTO.SetDatos, WSRotorContentByPositionDS)
                                    For Each rcp_row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rotorPosDS.twksWSRotorContentByPosition.Rows
                                        myGlobalDataTO = PrepareUIRefreshEventNum2(dbConnection, UI_RefreshEvents.ROTORPOSITION_CHANGED, "SAMPLES", rcp_row.CellNumber, _
                                                                             rcp_row.Status, "", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                                        'AG 09/03/2012 - do not inform about ElementStatus (old code: '... rcp_row.Status, "POS", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")'

                                        If myGlobalDataTO.HasError Then Exit For
                                    Next
                                End If

                                'AG 21/05/2012 - Call the online exportation method
                                If Not myGlobalDataTO.HasError Then
                                    Dim myExport As New ExportDelegate
                                    myGlobalDataTO = myExport.ManageLISExportation(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, ex_row.OrderTestID, False)
                                    If myGlobalDataTO.HasError Then Exit For
                                    AddIntoLastExportedResults(CType(myGlobalDataTO.SetDatos, ExecutionsDS)) 'AG 19/03/2013

                                    ''UI refresh DataSet informing the exported OrderTests - NOT Required, the UI refresh dataset RESULTS_CALCULATED case has been already updated at the begining of  this loop
                                    'If Not myGlobalDataTO.SetDatos Is Nothing Then
                                    '    For Each ex_row As ExecutionsDS.twksWSExecutionsRow In myExecDS.twksWSExecutions.Rows
                                    '        myGlobalDataTO = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED, ex_row.ExecutionID, Nothing, Nothing, False)
                                    '        If myGlobalDataTO.HasError Then Exit For
                                    '    Next
                                    'End If
                                End If
                                'AG 21/05/2012

                            End If
                            'AG 02/12/2011
                        Next

                        'SGM 24/07/2012
                        'Block ISE Preparations in case of Cancel Error (ERC) more than 2 times
                        If ISEAnalyzer.ISEWSCancelErrorCounter >= 3 Then

                            Dim exec_delg As New ExecutionsDelegate

                            '1st: Get all ISE executions pending (required to inform the UI with the locked executions
                            myGlobalDataTO = exec_delg.GetExecutionsByStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "PENDING", False, "PREP_ISE")
                            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then

                                Dim myExecutionsDS As New ExecutionsDS
                                myExecutionsDS = CType(myGlobalDataTO.SetDatos, ExecutionsDS)

                                If myExecutionsDS.twksWSExecutions.Count > 0 Then
                                    '2on: Lock all pending ISE preparation executions
                                    myGlobalDataTO = exec_delg.UpdateStatusByExecutionTypeAndStatus(dbConnection, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "PENDING", "LOCKED")

                                    '3rd: Prepare data for generate user interface refresh event
                                    If Not myGlobalDataTO.HasError Then
                                        For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                            myGlobalDataTO = PrepareUIRefreshEvent(dbConnection, UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
                                            If myGlobalDataTO.HasError Then Exit For
                                        Next
                                    End If
                                End If
                            End If
                            myGlobalDataTO = exec_delg.GetExecutionsByStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "INPROCESS", False, "PREP_ISE")
                            If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then

                                Dim myExecutionsDS = CType(myGlobalDataTO.SetDatos, ExecutionsDS)

                                If myExecutionsDS.twksWSExecutions.Count > 0 Then
                                    '2on: Lock all pending ISE preparation executions
                                    myGlobalDataTO = exec_delg.UpdateStatusByExecutionTypeAndStatus(dbConnection, WorkSessionIDAttribute, AnalyzerIDAttribute, "PREP_ISE", "INPROCESS", "LOCKED")

                                    '3rd: Prepare data for generate user interface refresh event
                                    If Not myGlobalDataTO.HasError Then
                                        For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                            myGlobalDataTO = PrepareUIRefreshEvent(dbConnection, UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
                                            If myGlobalDataTO.HasError Then Exit For
                                        Next
                                    End If
                                End If
                            End If
                        End If
                        'end SGM 24/07/2012

                    End If
                End If

                'Update analyzer session flags into DataBase
                If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                    Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                    myGlobalDataTO = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                End If

                ' XBC 21/03/2012   
                ' Check Alarms
                Dim myAlarmListTmp As New List(Of Alarms)
                Dim myAlarmStatusListTmp As New List(Of Boolean)
                Dim myAlarmList As New List(Of Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                myGlobalDataTO = ISEAnalyzer.CheckAlarms(Connected, myAlarmListTmp, myAlarmStatusListTmp)
                If Not myGlobalDataTO.HasError Then

                    ' Deactivates Alarm begin - BA-1872
                    Dim alarmID = AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR
                    Dim alarmStatus = False
                    ISEAnalyzer.IsTimeOut = False

                    ' XB 21/01/2015 - BA-1873
                    If ISEAnalyzer.IsISEModuleInstalled And ISEAnalyzer.IsISEModuleReady Then
                        ' Check if ISE Electrodes calibration is required
                        Dim ElectrodesCalibrationRequired As Boolean = False
                        myGlobalDataTO = ISEAnalyzer.CheckElectrodesCalibrationIsNeeded()
                        If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                            ElectrodesCalibrationRequired = CType(myGlobalDataTO.SetDatos, Boolean)
                        End If

                        myAlarmList.Add(AlarmEnumerates.Alarms.ISE_CALB_PDT_WARN)
                        If ElectrodesCalibrationRequired Then
                            myAlarmStatusList.Add(True)
                        Else
                            myAlarmStatusList.Add(False)
                        End If

                        ' Check if ISE Pumps calibration is required
                        Dim PumpsCalibrationRequired As Boolean = False
                        myGlobalDataTO = ISEAnalyzer.CheckPumpsCalibrationIsNeeded
                        If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                            PumpsCalibrationRequired = CType(myGlobalDataTO.SetDatos, Boolean)
                        End If

                        myAlarmList.Add(AlarmEnumerates.Alarms.ISE_PUMP_PDT_WARN)
                        If PumpsCalibrationRequired Then
                            myAlarmStatusList.Add(True)
                        Else
                            myAlarmStatusList.Add(False)
                        End If

                        ' Check if ISE Clean is required
                        Dim CleanRequired As Boolean = False
                        myGlobalDataTO = ISEAnalyzer.CheckCleanIsNeeded
                        If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then
                            CleanRequired = CType(myGlobalDataTO.SetDatos, Boolean)
                        End If

                        myAlarmList.Add(AlarmEnumerates.Alarms.ISE_CLEAN_PDT_WARN)
                        If CleanRequired Then
                            myAlarmStatusList.Add(True)
                        Else
                            myAlarmStatusList.Add(False)
                        End If
                    End If
                    ' XB 21/01/2015 - BA-1873

                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)
                    ' Deactivates Alarm end - BA-1872

                    For i As Integer = 0 To myAlarmListTmp.Count - 1
                        PrepareLocalAlarmList(myAlarmListTmp(i), myAlarmStatusListTmp(i), myAlarmList, myAlarmStatusList)
                    Next

                    'Finally call manage all alarms detected (new or solved)
                    If myAlarmList.Count > 0 Then
                        Dim currentAlarms = New AnalyzerAlarms(Me)
                        myGlobalDataTO = currentAlarms.Manage(myAlarmList, myAlarmStatusList)

                        If Not GlobalBase.IsServiceAssembly Then
                            Dim currentAlarmsRetry = New AnalyzerAlarms(Me)
                            myGlobalDataTO = currentAlarmsRetry.Manage(myAlarmList, myAlarmStatusList)
                        End If

                    End If

                    If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR) Then myAlarmListAttribute.Remove(AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR)
                End If

                'End If'AG 03/07/2012 - Running Cycles lost - Solution!


                ' XB 19/11/2014 - BA-1872
                If ISEAnalyzer.FirmwareErrDetected Then
                    Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - Set TimerStartTaskControl to [" & WAITING_TIME_FAST.ToString & "] seconds")
                    InitializeTimerStartTaskControl(WAITING_TIME_FAST, True)
                Else
                    ISECMDLost = False
                    sendingRepetitions = False
                    InitializeTimerStartTaskControl(WAITING_TIME_OFF)
                    ClearStartTaskQueueToSend()
                End If
                ' XB 19/11/2014 - BA-1872

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessRecivedISEResult", EventLogEntryType.Error, False)
            End Try

            'AG 29/11/2011 - This method does not follow the standard and not receive a pDbConnection as parameter so we have to close the connection properly here
            If (Not dbConnection Is Nothing) Then
                If (Not myGlobalDataTO.HasError) Then
                    'When the Database Connection was opened locally, then the Commit is executed
                    CommitTransaction(dbConnection)
                Else
                    'When the Database Connection was opened locally, then the Rollback is executed
                    RollbackTransaction(dbConnection)
                End If
                dbConnection.Close()
            End If
            'AG 29/11/2011

            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Performe the auto ise conditioning before enter in running
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  AG 19/0/2012
        ''' Modified by XB+AG 16/10/2013 - Leave pause running mode - BT #1333
        ''' </remarks>
        Private Function PerformAutoIseConditioning(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                        If mySessionFlags(AnalyzerManagerFlags.ISEConditioningProcess.ToString) = "INPROCESS" Then
                            Dim myScreenIseCmdTo As New ISECommandTO
                            'Dim myIseManager As New ISEManager

                            '   Required: Clean + Pump Calib + Calib A + B
                            If mySessionFlags(AnalyzerManagerFlags.ISEClean.ToString) = "" Then 'Start process sending Ise clean
                                'Send ISECMD clean
                                resultData = ISEAnalyzer.PrepareDataToSend(dbConnection, myApplicationName.ToUpper, AnalyzerIDAttribute, WorkSessionIDAttribute, ISEModes.Cleaning_Cycle, ISECommands.CLEAN)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    myScreenIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                                End If

                                If Not resultData.HasError Then
                                    resultData = ISEAnalyzer.SendISECommand 'SGM 08/03/2012
                                    'resultData = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.ISE_CMD, True, Nothing, myScreenIseCmdTo)

                                    'If not comms errors update flags
                                    If Not resultData.HasError AndAlso ConnectedAttribute Then
                                        UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISEClean, "INI")
                                        SetAnalyzerNotReady()
                                    End If
                                End If

                            ElseIf mySessionFlags(AnalyzerManagerFlags.ISEClean.ToString) = "END" AndAlso mySessionFlags(AnalyzerManagerFlags.ISEPumpCalib.ToString) <> "END" Then 'When ISEClean finish send pump calib
                                'Send ISECMD pump calib
                                resultData = ISEAnalyzer.PrepareDataToSend(dbConnection, myApplicationName.ToUpper, AnalyzerIDAttribute, WorkSessionIDAttribute, ISEModes.Pump_Calibration, ISECommands.PUMP_CAL)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    myScreenIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                                End If

                                If Not resultData.HasError Then
                                    resultData = ISEAnalyzer.SendISECommand 'SGM 08/03/2012

                                    If Not resultData.HasError AndAlso ConnectedAttribute Then
                                        UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISEPumpCalib, "INI")
                                        SetAnalyzerNotReady()
                                    End If
                                End If

                            ElseIf mySessionFlags(AnalyzerManagerFlags.ISEClean.ToString) = "END" AndAlso String.Compare(mySessionFlags(AnalyzerManagerFlags.ISEPumpCalib.ToString), "END", False) = 0 Then
                                'Send ISECMD calib AB
                                resultData = ISEAnalyzer.PrepareDataToSend(dbConnection, myApplicationName.ToUpper, AnalyzerIDAttribute, WorkSessionIDAttribute, ISEModes.Low_Level_Control, ISECommands.CALB)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    myScreenIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                                End If

                                If Not resultData.HasError Then
                                    resultData = ISEAnalyzer.SendISECommand 'SGM 08/03/2012

                                    If Not resultData.HasError AndAlso ConnectedAttribute Then
                                        UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ISECalibAB, "INI")
                                        SetAnalyzerNotReady()
                                    End If
                                End If

                            End If


                        ElseIf mySessionFlags(AnalyzerManagerFlags.ISEConditioningProcess.ToString) = "CLOSED" Then
                            '   Not required: Running

                            If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.RUNNINGprocess, "INPROCESS") 'AG 31/08/2012
                                resultData = ManageAnalyzer(AnalyzerManagerSwActionList.RUNNING, True) 'Send go to RUNNING
                            ElseIf AllowScanInRunning Then
                                'AG 12/11/2013 - Call manage analyer instead of applayer
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.StartRunning, "INI") 'AG 19/11/2012 - task #1396-e
                                resultData = ManageAnalyzer(AnalyzerManagerSwActionList.START, True, Nothing)
                            End If

                            If Not resultData.HasError AndAlso ConnectedAttribute Then
                                endRunAlreadySentFlagAttribute = False
                                abortAlreadySentFlagAttribute = False
                                PauseAlreadySentFlagAttribute = False ' XB 15/10/2013 - BT #1318
                                UpdateSensorValuesAttribute(AnalyzerSensors.BEFORE_ENTER_RUNNING, 1, True) 'Process finished (running instruction has been sent)

                                'Set WorkSession Status to INPROCESS
                                Dim myWSAnalyzerDelegate As New WSAnalyzersDelegate
                                resultData = myWSAnalyzerDelegate.UpdateWSStatus(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "INPROCESS")
                            End If

                        End If

                        If Not resultData.HasError AndAlso ConnectedAttribute Then
                            'Update analyzer session flags into DataBase
                            If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                                Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                                resultData = myFlagsDelg.Update(dbConnection, myAnalyzerFlagsDS)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.PerformAutoIseConditioning", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Processes the ISE procedures
        ''' </summary>
        ''' <remarks>
        ''' Created by  SGM 07/03/2012
        ''' Modified by SGM 16/01/2013 - Bug #1108
        '''              XB 20/05/2014 - Add protections #1614
        ''' </remarks>
        Private Function ProcessISEManagerProcedures() As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try

                'The ISE Module must be factory installed and switched on
                If ISEAnalyzer.IsISEModuleInstalled And ISEAnalyzer.IsISESwitchON Then

                    Select Case ISEAnalyzer.CurrentProcedure

                        Case ISEManager.ISEProcedures.Test
                            If ISEAnalyzer.CurrentCommandTO IsNot Nothing Then      ' #1614
                                If ISEAnalyzer.CurrentCommandTO.TestType = ISECycles.URINE1 Then
                                    ISEAnalyzer.CurrentCommandTO = New ISECommandTO(ISECycles.URINE2)
                                End If
                            End If

                        Case ISEManager.ISEProcedures.SingleReadCommand
                            'If ISEAnalyzer.CurrentCommandTO.ISECommandID = ISECommands.READ_PAGE_0_DALLAS Then
                            '    myGlobal = ISEAnalyzer.PrepareDataToSend_READ_PAGE_DALLAS(1)
                            'End If

                        Case ISEManager.ISEProcedures.GeneralCheckings
                            If ISEAnalyzer.CurrentCommandTO IsNot Nothing Then
                                Select Case ISEAnalyzer.CurrentCommandTO.ISECommandID

                                    '1- Read Page 00 from Reagents Pack
                                    Case ISECommands.POLL
                                        myGlobal = ISEAnalyzer.PrepareDataToSend_READ_PAGE_DALLAS(0)

                                        '2- Read Page 01 from Reagents Pack
                                    Case ISECommands.READ_PAGE_0_DALLAS
                                        myGlobal = ISEAnalyzer.PrepareDataToSend_READ_PAGE_DALLAS(1)

                                    Case ISECommands.READ_PAGE_1_DALLAS
                                        'If myISEManager.IsElectrodesReady Then
                                        myGlobal = ISEAnalyzer.PrepareDataToSend_READ_mV
                                        'End If
                                End Select
                            End If

                        Case ISEManager.ISEProcedures.ActivateReagentsPack
                            If ISEAnalyzer.CurrentCommandTO IsNot Nothing Then
                                Select Case ISEAnalyzer.CurrentCommandTO.ISECommandID

                                    'SGM 16/01/2013 Follow the next sequence
                                    '1-Read Dallas Page 00 (biosystems code)
                                    Case ISECommands.READ_PAGE_0_DALLAS
                                        'reset Clean Pack flag
                                        ISEAnalyzer.IsCleanPackInstalled = False
                                        '2-Read Dallas Page 01 (install date)
                                        myGlobal = ISEAnalyzer.PrepareDataToSend_READ_PAGE_DALLAS(1)
                                        myGlobal = ISEAnalyzer.SendISECommand()
                                        Debug.Print("Reading Dallas Page 01 ...")

                                    Case ISECommands.READ_PAGE_1_DALLAS
                                        '3-If not installed:
                                        If ISEAnalyzer.ReagentsPackInstallationDate = Nothing Then
                                            ' 3.1-Write Day
                                            myGlobal = ISEAnalyzer.PrepareDataToSend_WRITE_INSTALL_DAY(Now.Day)
                                            myGlobal = ISEAnalyzer.SendISECommand()
                                            ISEAnalyzer.CurrentCommandTO.ISECommandID = ISECommands.WRITE_DAY_INSTALL
                                            Debug.Print("Writing Install Day: " & Now.Day.ToString)
                                        End If

                                    Case ISECommands.WRITE_DAY_INSTALL
                                        ' 3.2-Write Month
                                        myGlobal = ISEAnalyzer.PrepareDataToSend_WRITE_INSTALL_MONTH(Now.Month)
                                        myGlobal = ISEAnalyzer.SendISECommand()
                                        ISEAnalyzer.CurrentCommandTO.ISECommandID = ISECommands.WRITE_MONTH_INSTALL
                                        Debug.Print("Writing Install Month: " & Now.Month.ToString)

                                    Case ISECommands.WRITE_MONTH_INSTALL
                                        ' 3.3-Write Year
                                        myGlobal = ISEAnalyzer.PrepareDataToSend_WRITE_INSTALL_YEAR(Now.Year)
                                        myGlobal = ISEAnalyzer.SendISECommand()
                                        ISEAnalyzer.CurrentCommandTO.ISECommandID = ISECommands.WRITE_YEAR_INSTALL
                                        Debug.Print("Writing Install Year: " & Now.Year.ToString)

                                    Case ISECommands.WRITE_YEAR_INSTALL
                                        '4-Read Dallas Page 01 (install date, confirmation)
                                        myGlobal = ISEAnalyzer.PrepareDataToSend_READ_PAGE_DALLAS(1)
                                        myGlobal = ISEAnalyzer.SendISECommand()
                                        Debug.Print("Reading Dallas Page 01 ... confirmation")

                                End Select
                            End If

                        Case ISEManager.ISEProcedures.CheckReagentsPack
                            If ISEAnalyzer.CurrentCommandTO IsNot Nothing Then
                                Select Case ISEAnalyzer.CurrentCommandTO.ISECommandID

                                    '2- Read Page 01 from Reagents Pack
                                    Case ISECommands.READ_PAGE_0_DALLAS
                                        myGlobal = ISEAnalyzer.PrepareDataToSend_READ_PAGE_DALLAS(1)

                                End Select
                            End If

                        Case ISEManager.ISEProcedures.CheckCleanPackInstalled
                            Select Case ISEAnalyzer.CurrentCommandTO.ISECommandID
                                Case ISECommands.PURGEA
                                    myGlobal = ISEAnalyzer.PrepareDataToSend_PURGEB

                            End Select

                        Case ISEManager.ISEProcedures.Clean 'PDT to optimization
                            'Select Case ISEAnalyzer.CurrentCommandTO.ISECommandID
                            '    Case ISECommands.CLEAN
                            '        myGlobal = ISEAnalyzer.PrepareDataToSend_CALB

                            'End Select

                        Case ISEManager.ISEProcedures.MaintenanceExit
                            Select Case ISEAnalyzer.CurrentCommandTO.ISECommandID
                                Case ISECommands.PURGEA
                                    myGlobal = ISEAnalyzer.PrepareDataToSend_PURGEB

                            End Select


                            ' XBC 04/04/2012
                        Case ISEManager.ISEProcedures.WriteConsumption
                            Select Case ISEAnalyzer.CurrentCommandTO.ISECommandID
                                Case ISECommands.WRITE_CALA_CONSUMPTION
                                    If ISEAnalyzer.IsCalBUpdateRequired Then 'update calB consumption

                                        ' Save it into Dallas Xip
                                        myGlobal = ISEAnalyzer.SaveConsumptionCalToDallasData("B")
                                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                            ISEAnalyzer.CurrentCommandTO = CType(myGlobal.SetDatos, ISECommandTO)
                                            Debug.Print("Guardant Consum Cal B ..." & Day(Now).ToString)

                                            myGlobal = ISEAnalyzer.SendISECommand()
                                            ISEAnalyzer.CurrentCommandTO.ISECommandID = ISECommands.WRITE_CALB_CONSUMPTION
                                            Debug.Print("Canvi d'estat a WRITE_CALB_CONSUMPTION ...")
                                        End If
                                    End If

                            End Select

                        Case ISEManager.ISEProcedures.PrimeAndCalibration
                            Select Case ISEAnalyzer.CurrentCommandTO.ISECommandID
                                Case ISECommands.PRIME_CALB
                                    myGlobal = ISEAnalyzer.PrepareDataToSend_PRIME_CALA

                                Case ISECommands.PRIME_CALA
                                    myGlobal = ISEAnalyzer.PrepareDataToSend_CALB
                            End Select

                        Case ISEManager.ISEProcedures.PrimeX2AndCalibration 'SGM 11/06/2012
                            Static PrimeCount As Integer = 0
                            Select Case ISEAnalyzer.CurrentCommandTO.ISECommandID
                                Case ISECommands.PRIME_CALB
                                    myGlobal = ISEAnalyzer.PrepareDataToSend_PRIME_CALA

                                Case ISECommands.PRIME_CALA
                                    PrimeCount += 1
                                    If PrimeCount < 2 Then
                                        myGlobal = ISEAnalyzer.PrepareDataToSend_PRIME_CALB
                                    Else
                                        PrimeCount = 0
                                        myGlobal = ISEAnalyzer.PrepareDataToSend_CALB
                                    End If


                            End Select


                        Case Else
                            Debug.Print("Caso NONE !")

                    End Select

                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then

                        If ISEAnalyzer.CurrentProcedure <> ISEManager.ISEProcedures.ActivateReagentsPack And _
                           ISEAnalyzer.CurrentProcedure <> ISEManager.ISEProcedures.WriteConsumption Then
                            myGlobal = ISEAnalyzer.SendISECommand()
                        End If

                    End If

                    'Special case for cheking before installing ISE Module
                ElseIf ISEAnalyzer.CurrentProcedure = ISEManager.ISEProcedures.ActivateModule Then
                    If ISEAnalyzer.CurrentCommandTO IsNot Nothing Then
                        Select Case ISEAnalyzer.CurrentCommandTO.ISECommandID

                            '1- Read Page 01 from Reagents Pack
                            Case ISECommands.READ_PAGE_0_DALLAS
                                myGlobal = ISEAnalyzer.PrepareDataToSend_READ_PAGE_DALLAS(1)

                            Case ISECommands.READ_PAGE_1_DALLAS
                                myGlobal = ISEAnalyzer.PrepareDataToSend_READ_mV
                        End Select
                    End If

                    If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        myGlobal = ISEAnalyzer.SendISECommand()
                    End If
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ProcessISEManagerProcedures", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Refreshes ISE alarms
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 07/06/2012
        ''' Modified by XB 21/01/2015 - Refresh Alarms about ISE calibrations and clean - BA-1873
        ''' </remarks>
        Public Function RefreshISEAlarms() As GlobalDataTO Implements IAnalyzerManager.RefreshISEAlarms
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myAlarmListTmp As New List(Of Alarms)
                Dim myAlarmStatusListTmp As New List(Of Boolean)
                Dim myAlarmList As New List(Of Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                myGlobal = ISEAnalyzer.CheckAlarms(Connected, myAlarmListTmp, myAlarmStatusListTmp)
                If Not myGlobal.HasError Then

                    ' XB 21/01/2015 - BA-1873
                    If ISEAnalyzer.IsISEModuleInstalled And ISEAnalyzer.IsISEModuleReady Then
                        ' Check if ISE Electrodes calibration is required
                        Dim ElectrodesCalibrationRequired As Boolean = False
                        myGlobal = ISEAnalyzer.CheckElectrodesCalibrationIsNeeded()
                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                            ElectrodesCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                        End If

                        myAlarmList.Add(AlarmEnumerates.Alarms.ISE_CALB_PDT_WARN)
                        If ElectrodesCalibrationRequired Then
                            myAlarmStatusList.Add(True)
                        Else
                            myAlarmStatusList.Add(False)
                        End If

                        ' Check if ISE Pumps calibration is required
                        Dim PumpsCalibrationRequired As Boolean = False
                        myGlobal = ISEAnalyzer.CheckPumpsCalibrationIsNeeded
                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                            PumpsCalibrationRequired = CType(myGlobal.SetDatos, Boolean)
                        End If

                        myAlarmList.Add(AlarmEnumerates.Alarms.ISE_PUMP_PDT_WARN)
                        If PumpsCalibrationRequired Then
                            myAlarmStatusList.Add(True)
                        Else
                            myAlarmStatusList.Add(False)
                        End If

                        ' Check if ISE Clean is required
                        Dim CleanRequired As Boolean = False
                        myGlobal = ISEAnalyzer.CheckCleanIsNeeded
                        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                            CleanRequired = CType(myGlobal.SetDatos, Boolean)
                        End If

                        myAlarmList.Add(AlarmEnumerates.Alarms.ISE_CLEAN_PDT_WARN)
                        If CleanRequired Then
                            myAlarmStatusList.Add(True)
                        Else
                            myAlarmStatusList.Add(False)
                        End If
                    End If

                    For i As Integer = 0 To myAlarmListTmp.Count - 1
                        PrepareLocalAlarmList(myAlarmListTmp(i), myAlarmStatusListTmp(i), myAlarmList, myAlarmStatusList)
                    Next

                    'Finally call manage all alarms detected (new or solved)
                    If myAlarmList.Count > 0 Then
                        If Not GlobalBase.IsServiceAssembly Then
                            Dim currentAlarms = New AnalyzerAlarms(Me)
                            myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                        End If

                    End If
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.RefreshISEAlarms", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Activates Analyzer Alarms in case of errors in calibrations
        ''' </summary>
        ''' <param name="pISEResult"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 25/07/2012
        ''' Modified by XB 08/09/2014 - Use ISE new FormatAffected method in ISEManager layer instead of the old method in ISEErrorTO - BA-1902
        ''' </remarks>
        Public Function ActivateAnalyzerISEAlarms(ByVal pISEResult As ISEResultTO) As GlobalDataTO Implements IAnalyzerManager.ActivateAnalyzerISEAlarms

            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing
            Try

                If pISEResult.Errors.Count > 0 Then

                    'Prepare internal variables for alarms management
                    Dim myAlarmList As New List(Of Alarms)
                    Dim myAlarmStatusList As New List(Of Boolean)
                    Dim myAlarmDescriptions As New List(Of String)


                    Dim myAlarmOrigin As String = "" 'origin

                    Dim myAlarmID As Alarms 'Identifier

                    If pISEResult.IsCancelError Then

                        Dim myAdditionalInfo As String = ""

                        Select Case pISEResult.Errors(0).ErrorCycle
                            Case ISEErrorTO.ErrorCycles.Calibration : myAlarmOrigin = "CALB"
                            Case ISEErrorTO.ErrorCycles.PumpCalibration : myAlarmOrigin = "PMCL"
                            Case ISEErrorTO.ErrorCycles.BubbleDetCalibration : myAlarmOrigin = "BBCL"
                            Case ISEErrorTO.ErrorCycles.Clean : myAlarmOrigin = "CALB"
                            Case ISEErrorTO.ErrorCycles.Communication : myAlarmOrigin = "COMM"
                            Case ISEErrorTO.ErrorCycles.DallasReadWrite : myAlarmOrigin = "DALLAS RW"
                            Case ISEErrorTO.ErrorCycles.Maintenance : myAlarmOrigin = "MANT"
                            Case ISEErrorTO.ErrorCycles.PurgeA : myAlarmOrigin = "PUGA"
                            Case ISEErrorTO.ErrorCycles.PurgeB : myAlarmOrigin = "PUGB"
                            Case ISEErrorTO.ErrorCycles.Sample : myAlarmOrigin = "SER"
                            Case ISEErrorTO.ErrorCycles.Urine : myAlarmOrigin = "URN"
                            Case ISEErrorTO.ErrorCycles.SipCycle : myAlarmOrigin = "SIP"
                        End Select
                        Select Case pISEResult.Errors(0).CancelErrorCode
                            Case ISEErrorTO.ISECancelErrorCodes.A : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_A
                            Case ISEErrorTO.ISECancelErrorCodes.B : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_B
                            Case ISEErrorTO.ISECancelErrorCodes.C : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_C
                            Case ISEErrorTO.ISECancelErrorCodes.D : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_D
                            Case ISEErrorTO.ISECancelErrorCodes.F : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_F
                            Case ISEErrorTO.ISECancelErrorCodes.M : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_M
                            Case ISEErrorTO.ISECancelErrorCodes.N : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_N
                            Case ISEErrorTO.ISECancelErrorCodes.P : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_P
                            Case ISEErrorTO.ISECancelErrorCodes.R : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_R
                            Case ISEErrorTO.ISECancelErrorCodes.S : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_S
                            Case ISEErrorTO.ISECancelErrorCodes.T : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_T
                            Case ISEErrorTO.ISECancelErrorCodes.W : myAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_W

                        End Select

                        'myAlarmOrigin = "ISE Error: " & myAlarmOrigi
                        myAdditionalInfo &= GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & myAlarmOrigin & ":E" & CInt(pISEResult.Errors(0).CancelErrorCode).ToString & ":"

                        If myAlarmID <> AlarmEnumerates.Alarms.NONE Then
                            myAlarmDescriptions.Add(myAdditionalInfo)
                            myAlarmList.Add(myAlarmID)
                        End If

                        'special case for calibration result errors
                    ElseIf pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL Or _
                        pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.PMC Or _
                        pISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BBC Then

                        Dim myAdditionalInfo As String = ""

                        For Each E As ISEErrorTO In pISEResult.Errors
                            If Not E.IsCancelError Then
                                'SGM 26/07/2012 - special case for CALB, PMCL, BBCL results no ok
                                Select Case pISEResult.ISEResultType
                                    Case ISEResultTO.ISEResultTypes.CAL : myAlarmOrigin = "CALB Error - "
                                    Case ISEResultTO.ISEResultTypes.PMC : myAlarmOrigin = "PMCL Error - "
                                    Case ISEResultTO.ISEResultTypes.BBC : myAlarmOrigin = "BBCL Error - "
                                End Select

                                Dim myResultAlarm As Integer = -1
                                If E.ResultErrorCode <> ISEErrorTO.ISEResultErrorCodes.None Then
                                    myResultAlarm = CInt(E.ResultErrorCode)

                                    Dim myAffected As String = ISEManager.FormatAffected(E.Affected)

                                    myAdditionalInfo &= GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR & myAlarmOrigin & ":R" & myResultAlarm.ToString & ": (" + myAffected + ")"
                                End If

                            End If

                        Next

                        myAlarmDescriptions.Add(myAdditionalInfo)
                        myAlarmID = AlarmEnumerates.Alarms.ISE_CALIB_ERROR
                        myAlarmList.Add(myAlarmID)

                        'myAlarmStatusList.Add(True)
                    End If

                    For Each A As Alarms In myAlarmList

                        Dim myAnalyzerAlarmsDS As New WSAnalyzerAlarmsDS
                        Dim alarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow
                        alarmRow = myAnalyzerAlarmsDS.twksWSAnalyzerAlarms.NewtwksWSAnalyzerAlarmsRow

                        Dim myTempAlarmList As New List(Of Alarms)
                        PrepareLocalAlarmList(myAlarmID, True, myTempAlarmList, myAlarmStatusList, myAlarmOrigin, myAlarmDescriptions, False)

                        If myAlarmList.Count > 0 Then
                            If Not GlobalBase.IsServiceAssembly Then
                                Dim currentAlarms = New AnalyzerAlarms(Me)
                                myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                            End If
                        End If

                    Next

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.ActivateAnalyzerISEAlarms", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Blocks Individual ISE Electrode after receiving results erros
        ''' </summary>
        ''' <param name="pISEResult"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 02/08/2012
        ''' Modified by XB 20/05/2014 - Fix BT #1629
        ''' </remarks>
        Public Function BlockISEPreparationByElectrode(ByVal pDBConnection As SqlConnection, ByVal pISEResult As ISEResultTO, ByVal pWorkSessionId As String, ByVal pAnalyzerID As String) As GlobalDataTO Implements IAnalyzerManager.BlockISEPreparationByElectrode
            Dim myGlobalDataTO As New GlobalDataTO
            Try

                Dim isLiAffected As Boolean = False
                Dim isNaAffected As Boolean = False
                Dim isKAffected As Boolean = False
                Dim isClAffected As Boolean = False
                For Each E As ISEErrorTO In ISEAnalyzer.LastISEResult.Errors
                    isLiAffected = (isLiAffected Or (E.Affected.Contains("Li"))) And ISEAnalyzer.IsLiEnabledByUser
                    isNaAffected = isNaAffected Or (E.Affected.Contains("Na"))
                    isKAffected = isKAffected Or (E.Affected.Contains("K"))
                    isClAffected = isClAffected Or (E.Affected.Contains("Cl"))
                Next

                Dim myExecutionsDelegate As New ExecutionsDelegate
                myGlobalDataTO = myExecutionsDelegate.GetExecutionsByStatus(pDBConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, "PENDING", False, "PREP_ISE")
                If Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing Then

                    Dim myExecutionsDS As New ExecutionsDS
                    myExecutionsDS = CType(myGlobalDataTO.SetDatos, ExecutionsDS)

                    If myExecutionsDS.twksWSExecutions.Count > 0 Then

                        If isLiAffected Then
                            myGlobalDataTO = myExecutionsDelegate.UpdateStatusByISETestType(pDBConnection, pWorkSessionId, AnalyzerIDAttribute, "Li", "PENDING", "LOCKED")
                        End If
                        If isNaAffected Then
                            myGlobalDataTO = myExecutionsDelegate.UpdateStatusByISETestType(pDBConnection, pWorkSessionId, AnalyzerIDAttribute, "Na", "PENDING", "LOCKED")
                        End If
                        If isKAffected Then
                            myGlobalDataTO = myExecutionsDelegate.UpdateStatusByISETestType(pDBConnection, pWorkSessionId, AnalyzerIDAttribute, "K", "PENDING", "LOCKED")
                        End If
                        If isClAffected Then
                            myGlobalDataTO = myExecutionsDelegate.UpdateStatusByISETestType(pDBConnection, pWorkSessionId, AnalyzerIDAttribute, "Cl", "PENDING", "LOCKED")
                        End If

                        If Not myGlobalDataTO.HasError Then
                            For Each row As ExecutionsDS.twksWSExecutionsRow In myExecutionsDS.twksWSExecutions.Rows
                                myGlobalDataTO = PrepareUIRefreshEvent(pDBConnection, UI_RefreshEvents.EXECUTION_STATUS, row.ExecutionID, 0, Nothing, False)
                                If myGlobalDataTO.HasError Then Exit For
                            Next
                        End If

                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                GlobalBase.CreateLogActivity(ex.Message, "ISEReception.BlockISEPreparationByElectrode", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO

        End Function



#End Region

#Region "Auxiliary methods others"

        ''' <summary>
        ''' Method for code common used
        ''' </summary>
        ''' <remarks>
        ''' Created by:  AG 14/03/2011
        ''' Modified by: IT 19/12/2014 - BA-2143 (Accessibility Level)
        ''' </remarks>
        Public Sub SetAnalyzerNotReady() Implements IAnalyzerManager.SetAnalyzerNotReady
            If AnalyzerIsReadyAttribute Then
                AnalyzerIsReadyAttribute = False
            End If
        End Sub


        Private Function ReadBarCodeRotorSettingEnabled(ByVal pDBConnection As SqlConnection, ByVal pSetting As AnalyzerSettingsEnum) As Boolean
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing
            Dim returnedFlag As Boolean = False
            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        'If reagents barcode enabled send read FULL barcode reagents rotor
                        Dim anSettingsDelg As New AnalyzerSettingsDelegate
                        resultData = anSettingsDelg.GetAnalyzerSetting(AnalyzerIDAttribute, pSetting).GetCompatibleGlobalDataTO
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then '(1)
                            Dim myAnSettingsDS As New AnalyzerSettingsDS
                            myAnSettingsDS = DirectCast(resultData.SetDatos, AnalyzerSettingsDS)

                            If (myAnSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then '(1)
                                returnedFlag = CType(myAnSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                            End If

                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ReadBarCodeRotorSettingEnabled", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try
            Return returnedFlag
        End Function


#End Region

#Region "OLD version methods"

        '''' <summary>
        '''' Search next STD preparation, taking care about the last executions sent and search next avoiding contaminations
        '''' using the executions matching the OrderID, SampleType, StatFlag and SampleClass in entry parameters
        '''' 
        '''' This method is up to date due the sort algoritm by look for the next preparation is different that the used in CreateWSExecutions
        '''' </summary>
        '''' <param name="pFound" ></param>
        '''' <param name="pSTDExecutionList" ></param>
        '''' <param name="pOrderID " ></param>
        '''' <param name="pSampleType " ></param>
        '''' <param name="pStatFlag" ></param>
        '''' <param name="pSampleClass" ></param>
        '''' <returns>AnalyzerManager.searchNext inside a GlobalDataTo</returns>
        '''' <remarks>AG 25/01/2011 - Created</remarks>
        'Private Function GetNextExecutionUntil28112011(ByRef pFound As Boolean, ByVal pSTDExecutionList As AnalyzerManagerDS, _
        '                                 ByVal pOrderID As String, ByVal pSampleType As String, _
        '                                 ByVal pStatFlag As Boolean, ByVal pSampleClass As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try

        '        'Get the list of pending executions depending the entry parameters case
        '        'Apply different LINQ (where clause)
        '        Dim toSendList As New List(Of AnalyzerManagerDS.searchNextRow)
        '        If pOrderID <> "" And pSampleType <> "" Then '(1)
        '            toSendList = (From a As AnalyzerManagerDS.searchNextRow In pSTDExecutionList.searchNext _
        '                            Where a.OrderID = pOrderID And a.SampleType = pSampleType And a.StatFlag = pStatFlag _
        '                            Select a).ToList
        '        ElseIf pOrderID <> "" And pSampleType <> "" And pSampleClass <> "" Then '(1)
        '            toSendList = (From a As AnalyzerManagerDS.searchNextRow In pSTDExecutionList.searchNext _
        '                            Where a.OrderID = pOrderID And a.SampleType = pSampleType And a.StatFlag = pStatFlag And a.SampleClass = pSampleClass _
        '                            Select a).ToList

        '        ElseIf pSampleClass <> "" Then '(1)
        '            toSendList = (From a As AnalyzerManagerDS.searchNextRow In pSTDExecutionList.searchNext _
        '                            Where a.StatFlag = pStatFlag And a.SampleClass = pSampleClass _
        '                            Select a).ToList

        '        Else '(1)
        '            toSendList = (From a As AnalyzerManagerDS.searchNextRow In pSTDExecutionList.searchNext _
        '                            Where a.StatFlag = pStatFlag _
        '                            Select a).ToList

        '        End If '(1)


        '        Dim nextExecutionFound As Boolean = False
        '        Dim rowExecutionFound As AnalyzerManagerDS.searchNextRow
        '        Dim jIndex As Integer = 0
        '        Dim iIndex As Integer = 0

        '        If toSendList.Count > 0 Then '(2)
        '            Dim tmpExeID As Integer = -1
        '            For j As Integer = 0 To toSendList.Count - 1 'For (1)
        '                jIndex = j
        '                tmpExeID = toSendList(j).ExecutionID

        '                'Is reagent has contaminators ... analyze last sent preparations
        '                If Not toSendList(j).IsContaminationIDNull And Not toSendList(j).IsReagentContaminatorIDNull Then '(3)
        '                    'Get all contaminations defined for the reagent used in the execution tmpExeID
        '                    Dim list2 As New List(Of AnalyzerManagerDS.searchNextRow)
        '                    list2 = (From a As AnalyzerManagerDS.searchNextRow In toSendList _
        '                                  Where a.ExecutionID = tmpExeID Select a).ToList

        '                    Dim contaminationFound As Boolean = False
        '                    Dim i As Integer = 0
        '                    If list2.Count > 0 Then '(4)
        '                        For i = 0 To list2.Count - 1 'For (2)
        '                            iIndex = i

        '                            Dim myReagentContaminator As Integer = -1
        '                            Dim myWashSolution As String = ""
        '                            If Not list2(i).IsReagentContaminatorIDNull Then myReagentContaminator = list2(i).ReagentContaminatorID
        '                            If Not list2(i).IsWashingSolution1Null Then myWashSolution = list2(i).WashingSolution1

        '                            'Using Linq determine if the reagentcontaminator is one of the last preparations sent
        '                            Dim list3 As New List(Of AnalyzerManagerDS.sentPreparationsRow)
        '                            list3 = (From a As AnalyzerManagerDS.sentPreparationsRow In mySentPreparationsDS.sentPreparations _
        '                                      Where Not a.IsReagentIDNull Select a).ToList

        '                            If list3.Count = 0 Then
        '                                Exit For '(2) 'If no reagent sent then no contamination exists
        '                            Else
        '                                list3 = (From a As AnalyzerManagerDS.sentPreparationsRow In list3 _
        '                                          Where a.ReagentID = myReagentContaminator Select a).ToList
        '                            End If

        '                            If list3.Count > 0 Then '(5)
        '                                'Using the last PREP_STD executions sent and look for contaminations (linq is not possible due we have to determine
        '                                'if the proper wash has been sent after
        '                                Dim rowPointer As Integer = -1
        '                                For Each row As AnalyzerManagerDS.sentPreparationsRow In mySentPreparationsDS.sentPreparations.Rows 'For (3)
        '                                    rowPointer += 1
        '                                    If Not row.IsExecutionTypeNull Then '(6)
        '                                        If row.ExecutionType = "PREP_STD" And row.ReagentID = myReagentContaminator Then '(7)
        '                                            contaminationFound = True

        '                                            'If exits contamination ... look if the proper wash has been already performed
        '                                            For k As Integer = rowPointer + 1 To mySentPreparationsDS.sentPreparations.Rows.Count - 1 'For (4)
        '                                                Dim pastWashFlag As Boolean = False
        '                                                Dim pastWashSolution As String = ""
        '                                                If Not mySentPreparationsDS.sentPreparations(k).IsReagentWashFlagNull Then pastWashFlag = mySentPreparationsDS.sentPreparations(k).ReagentWashFlag
        '                                                If Not mySentPreparationsDS.sentPreparations(k).IsWashSolution1Null Then pastWashSolution = mySentPreparationsDS.sentPreparations(k).WashSolution1

        '                                                If pastWashFlag And pastWashSolution = myWashSolution Then
        '                                                    contaminationFound = False 'The contamination has been already solved!
        '                                                    Exit For 'Exit For (4)
        '                                                End If
        '                                            Next 'For (4)
        '                                        End If '(7)
        '                                    End If '(6)

        '                                    If contaminationFound Then
        '                                        'If a contamination is found finish loop
        '                                        Exit For 'Exit For (3)
        '                                    End If
        '                                Next 'For (3)

        '                                If Not contaminationFound Then Exit For 'Exit For (2) - myWashSolution has the proper WashSolution to use

        '                            End If '(5)
        '                        Next 'For (2)

        '                        If Not contaminationFound Then
        '                            nextExecutionFound = True
        '                            rowExecutionFound = toSendList(jIndex)
        '                            Exit For 'Exit For (1)
        '                        Else
        '                            nextExecutionFound = False
        '                            rowExecutionFound = list2(0) '(iIndex) If contamination found sent the first item in list2. Usign iIndex the last was sent
        '                        End If
        '                    End If '(4) If list2.Count > 0 Then


        '                    'Is reagent has no contaminators ... sent it
        '                Else '(3) If Not toSendList(j).IsContaminationIDNull And Not toSendList(j).IsReagentContaminatorIDNull Then
        '                    nextExecutionFound = True
        '                    rowExecutionFound = toSendList(jIndex)
        '                    Exit For 'Exit For (1)
        '                End If '(3)

        '            Next 'For (1)

        '            Dim myReturn As New AnalyzerManagerDS
        '            Dim myRow As AnalyzerManagerDS.searchNextRow

        '            pFound = True ' Inform something is found to be sent: execution or wash
        '            If nextExecutionFound Then '(4)
        '                'Prepare output DS with the proper information (execution to be sent)
        '                myRow = myReturn.searchNext.NewsearchNextRow
        '                myRow.ExecutionID = rowExecutionFound.ExecutionID
        '                myRow.SampleClass = rowExecutionFound.SampleClass
        '                myRow.SetContaminationIDNull()
        '                myRow.SetWashingSolution1Null()
        '                myReturn.searchNext.AddsearchNextRow(myRow)

        '            Else
        '                'Contamination (wash has to be sent)
        '                myRow = myReturn.searchNext.NewsearchNextRow
        '                myRow.ExecutionID = GlobalConstants.NO_PENDING_PREPARATION_FOUND
        '                myRow.SetSampleClassNull()
        '                myRow.ContaminationID = rowExecutionFound.ContaminationID
        '                myRow.WashingSolution1 = rowExecutionFound.WashingSolution1
        '                myReturn.searchNext.AddsearchNextRow(myRow)
        '            End If '(4)
        '            resultData.SetDatos = myReturn


        '            'Else '(2) If toSendList.Count > 0 Then
        '            'No exectution matches with linq where criteria
        '        End If '(2)

        '    Catch ex As Exception
        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.GetNextExecutionUntil28112011", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Basic method for search next STD or ISE preparation execution based in OLD SearchNextPreparation
        '''' 
        '''' Search next pending preparation
        '''' Priority criteriom:
        '''' 1st Stats
        '''' 2st SampleClass (blank, calib, ctrl, patients)
        '''' 3st MultiItemNumber (ASC) (without change ordertestid)
        '''' 4st Replicate number (ASC) (without change ordertestid) 
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pExecutionType"></param>
        '''' <returns></returns>
        '''' <remarks>AG 17/01/2011 - Created</remarks>
        'Private Function GetNextExecutionOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pExecutionType As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then '(1)
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then '(2)
        '                Dim myExecution As New ExecutionsDelegate
        '                Dim myStatFlag As Boolean = True
        '                Dim foundItem As Boolean = False

        '                If AnalyzerIDAttribute = "" Or WorkSessionIDAttribute = "" Then '(3) 'AG 03/11/2010 If no exists analyzer or worksession then return NO PENDING PREPARATION
        '                    resultData.SetDatos = GlobalConstants.NO_PENDING_PREPARATION_FOUND

        '                Else
        '                    Do
        '                        If Not foundItem Then
        '                            resultData = myExecution.GetNextPreparation(dbConnection, "BLANK", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute, pExecutionType)
        '                            If Not resultData.HasError Then
        '                                foundItem = (CType(resultData.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                            Else
        '                                Exit Try
        '                            End If
        '                        End If

        '                        If Not foundItem Then
        '                            resultData = myExecution.GetNextPreparation(dbConnection, "CALIB", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute, pExecutionType)
        '                            If Not resultData.HasError Then
        '                                foundItem = (CType(resultData.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                            Else
        '                                Exit Try
        '                            End If
        '                        End If

        '                        If Not foundItem Then
        '                            resultData = myExecution.GetNextPreparation(dbConnection, "CTRL", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute, pExecutionType)
        '                            If Not resultData.HasError Then
        '                                foundItem = (CType(resultData.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                            Else
        '                                Exit Try
        '                            End If
        '                        End If

        '                        If Not foundItem Then
        '                            resultData = myExecution.GetNextPreparation(dbConnection, "PATIENT", myStatFlag, WorkSessionIDAttribute, AnalyzerIDAttribute, pExecutionType)
        '                            If Not resultData.HasError Then
        '                                foundItem = (CType(resultData.SetDatos, Integer) <> GlobalConstants.NO_PENDING_PREPARATION_FOUND)
        '                            Else
        '                                Exit Try
        '                            End If
        '                        End If

        '                        myStatFlag = Not myStatFlag
        '                    Loop Until myStatFlag Or foundItem
        '                End If '(3)

        '            End If '(2)
        '        End If '(1)

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.GetNextExecutionOLD", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

#End Region

        Private Function InsertNextPreparation(dbConnection As SqlConnection) As Integer
            Dim myPreparationDS As New WSPreparationsDS
            Dim myPrepDelegate As New WSPreparationsDelegate
            Dim prepRow As WSPreparationsDS.twksWSPreparationsRow

            'Generate the next PreparationID and insert the new Preparation
            Dim myGlobal = myPrepDelegate.GeneratePreparationID(dbConnection)
            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                Dim myNextPreparationID = CType(myGlobal.SetDatos, Integer)

                'Prepare data to insert the new Preparation
                prepRow = myPreparationDS.twksWSPreparations.NewtwksWSPreparationsRow
                prepRow.AnalyzerID = AnalyzerIDAttribute
                prepRow.WorkSessionID = WorkSessionIDAttribute
                prepRow.PreparationID = myNextPreparationID
                prepRow.LAX00Data = AppLayer.LastPreparationInstructionSent
                prepRow.PreparationStatus = "INPROCESS"

                'AG 20/09/2012 v052 - inform the well where the reactions is dispensed on (when PTEST apply the correct offset factor)
                prepRow.WellUsed = CurrentWellAttribute
                If InStr(prepRow.LAX00Data, "PTEST") > 0 Then
                    Dim reactRotorDlg As New ReactionsRotorDelegate
                    prepRow.WellUsed = reactRotorDlg.GetRealWellNumber(CurrentWellAttribute + WELL_OFFSET_FOR_PREDILUTION, MAX_REACTROTOR_WELLS)
                End If

                myPreparationDS.twksWSPreparations.AddtwksWSPreparationsRow(prepRow)

                'Insert the new Preparation
                myPrepDelegate.AddWSPreparation(dbConnection, myPreparationDS)

                Return myNextPreparationID
            End If

        End Function

    End Class

End Namespace

