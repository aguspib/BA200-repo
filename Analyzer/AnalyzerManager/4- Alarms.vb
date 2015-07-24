Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Calculations
Imports System.Timers
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports System.Globalization
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Partial Public Class AnalyzerManager

#Region "Main Manage Alarms treatment"
        ''' <summary>
        ''' Indicate if the Alarm Sound is enable on the Analyzer configuration.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>CREATE BY: TR 27/10/2011</remarks>
        Public Function IsAlarmSoundDisable(ByVal pdbConnection As SqlConnection) As GlobalDataTO Implements IAnalyzerManager.IsAlarmSoundDisable

            Dim myGlobalDataTo As New GlobalDataTO
            Dim dbConnection As New SqlConnection
            Try
                myGlobalDataTo = GetOpenDBTransaction(pdbConnection)
                If (Not myGlobalDataTo.HasError And Not myGlobalDataTo.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTo.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        'Search on tcfgAnalyzerSettings where setting id = ALARM_DISABLED
                        Dim myAnalyzerSettingDelegate As New AnalyzerSettingsDelegate
                        myGlobalDataTo = myAnalyzerSettingDelegate.GetAnalyzerSetting(AnalyzerIDAttribute, AnalyzerSettingsEnum.ALARM_DISABLED).GetCompatibleGlobalDataTO

                        If Not myGlobalDataTo.HasError Then
                            Dim myAnalyzerSettingsDS As AnalyzerSettingsDS = DirectCast(myGlobalDataTo.SetDatos, AnalyzerSettingsDS)
                            If (myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count = 1) Then
                                myGlobalDataTo.SetDatos = CType(myAnalyzerSettingsDS.tcfgAnalyzerSettings.First.CurrentValue, Boolean)
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTo.HasError = True
                myGlobalDataTo.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobalDataTo.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.IsAlarmSoundEnable", EventLogEntryType.Error, False)

            Finally
                If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobalDataTo

        End Function


        ''' <summary>
        ''' If needed perform the proper business depending the alarm code and status
        ''' </summary>
        ''' <param name="pdbConnection"></param>
        ''' <param name="pAlarmIDList">List of alarms to treat</param>
        ''' <param name="pAlarmStatusList" >For each alarm in pAlarmCode indicates his status: TRUE alarm exists, FALSE alarm solved</param>
        ''' <param name="pErrorCodeList">List of Error Firmware Codes to treat</param>
        ''' <param name="pAnswerErrorReception" >Comming from ANSERR instruction received</param>
        ''' <returns>Global Data To indicating when an error has occurred </returns>
        ''' <remarks>
        ''' Created by XBC 23/05/2011
        ''' Modified by XBC 16/10/2012 - Add pErrorCodeList functionality to manage Firmware Alarms into Service Software
        ''' </remarks>
        Public Function ManageAlarms_SRV(ByVal pdbConnection As SqlConnection, _
                                          ByVal pAlarmIDList As List(Of Alarms), _
                                          ByVal pAlarmStatusList As List(Of Boolean), _
                                          Optional ByVal pErrorCodeList As List(Of String) = Nothing, _
                                          Optional ByVal pAnswerErrorReception As Boolean = False) As GlobalDataTO _
                                      Implements IAnalyzerManager.ManageAlarms_SRV
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try

                Dim myManageAlarmType As ManagementAlarmTypes = ManagementAlarmTypes.OMMIT_ERROR

                If Not pErrorCodeList Is Nothing Then
                    If pErrorCodeList.Count > 0 Then

                        SolveErrorCodesToDisplay(pErrorCodeList)

                        Dim myAlarmsDelegate As New AlarmsDelegate
                        ' Get Management priotity of each error
                        Dim myManageAlarmTypeTemp As ManagementAlarmTypes
                        Dim addErrCode As Boolean
                        Dim informAlarm As Boolean = False
                        Dim alarmId As String = ""

                        For i As Integer = 0 To pErrorCodeList.Count - 1

                            'Generate UI_Refresh event ALARMS_RECEIVED
                            If i > pAlarmIDList.Count - 1 Then
                                ' Error - Firmare has sent and identifier that not exist into masterdata database
                                GlobalBase.CreateLogActivity("Firmware Alarm Code received not specified on Masterdata [" & pErrorCodeList(i) & "]", "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Error, False)
                                Exit Try
                            End If

                            alarmId = pAlarmIDList(i).ToString

                            myGlobal = myAlarmsDelegate.ReadManagementAlarm(Nothing, pErrorCodeList(i))
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                Dim myErrorCodesDs As AlarmErrorCodesDS
                                myErrorCodesDs = CType(myGlobal.SetDatos, AlarmErrorCodesDS)
                                If myErrorCodesDs.tfmwAlarmErrorCodes.Count > 0 Then

                                    AddErrorCodesToDisplay(myErrorCodesDs)

                                    Select Case myErrorCodesDs.tfmwAlarmErrorCodes(0).ManagementID
                                        Case "1_UPDATE_FW"
                                            myManageAlarmTypeTemp = ManagementAlarmTypes.UPDATE_FW
                                            addErrCode = True
                                            informAlarm = True

                                        Case "2_FATAL_ERROR"
                                            myManageAlarmTypeTemp = ManagementAlarmTypes.FATAL_ERROR
                                            addErrCode = True
                                            informAlarm = True

                                        Case "3_RECOVER_ERROR"
                                            ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.STP) 'SGM 19/11/2012
                                            If (alarmId = AlarmEnumerates.Alarms.INST_ABORTED_ERR.ToString Or alarmId = AlarmEnumerates.Alarms.RECOVER_ERR.ToString) _
                                               AndAlso Not pAnswerErrorReception Then
                                                myManageAlarmTypeTemp = ManagementAlarmTypes.REQUEST_INFO
                                                addErrCode = False
                                                informAlarm = False
                                                IsInstructionAborted = (alarmId = AlarmEnumerates.Alarms.INST_ABORTED_ERR.ToString)
                                                GlobalBase.CreateLogActivity("Alarm error codes received [" & pErrorCodeList(i) & "] - Priority Management : " & myManageAlarmTypeTemp.ToString, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Information, False)
                                            Else
                                                myManageAlarmTypeTemp = ManagementAlarmTypes.RECOVER_ERROR
                                                addErrCode = True
                                                informAlarm = True
                                            End If

                                        Case "4_SIMPLE_ERROR"
                                            myManageAlarmTypeTemp = ManagementAlarmTypes.SIMPLE_ERROR
                                            addErrCode = True
                                            If alarmId = AlarmEnumerates.Alarms.REACT_MISSING_ERR.ToString Then 'SGM 07/11/2012 - reactions rotor
                                                If Not IsServiceRotorMissingInformed Then
                                                    informAlarm = True
                                                End If
                                            Else
                                                informAlarm = informAlarm Or (IsInstructionRejected Or IsRecoverFailed) 'SGM 29/10/2012 - manage only in case of origin of Error 20 - INST_REJECTED_WARN
                                            End If


                                        Case "5_REQUEST_INFO"
                                            'SGM 29/10/2012 - reset flag that indicates E:20 received (Instruction Rejected)
                                            IsInstructionRejected = (alarmId = AlarmEnumerates.Alarms.INST_REJECTED_ERR.ToString)

                                            'SGM 07/11/2012 - reset flag that indicates E:22 received (Recover failed)
                                            IsRecoverFailed = (alarmId = AlarmEnumerates.Alarms.RECOVER_ERR.ToString)

                                            myManageAlarmTypeTemp = ManagementAlarmTypes.REQUEST_INFO
                                            addErrCode = False
                                            informAlarm = False
                                            GlobalBase.CreateLogActivity("Alarm error codes received [" & pErrorCodeList(i) & "] - Priority Management : " & myManageAlarmTypeTemp.ToString, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Information, False)

                                        Case "6_OMMIT_ERROR"
                                            myManageAlarmTypeTemp = ManagementAlarmTypes.OMMIT_ERROR
                                            addErrCode = False
                                            GlobalBase.CreateLogActivity("Alarm error codes received [" & pErrorCodeList(i) & "] - Priority Management : " & myManageAlarmTypeTemp.ToString, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Information, False)

                                        Case Else
                                            myManageAlarmTypeTemp = ManagementAlarmTypes.NONE
                                            addErrCode = False

                                    End Select

                                    If addErrCode Then
                                        ' Add New Error Codes received
                                        If Not myErrorCodesAttribute.Contains(pErrorCodeList(i)) Then
                                            myErrorCodesAttribute.Add(pErrorCodeList(i))
                                        End If
                                    End If

                                    ' Request Info if is need
                                    If myManageAlarmTypeTemp = ManagementAlarmTypes.REQUEST_INFO Then
                                        ' INSTRUCTIONS: ask for detailed errors
                                        myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.INFO, True, Nothing, Ax00InfoInstructionModes.ALR)
                                    End If

                                    ' Choose maximum priority
                                    If myManageAlarmTypeTemp <> 0 Then
                                        If myManageAlarmType > myManageAlarmTypeTemp Then
                                            myManageAlarmType = myManageAlarmTypeTemp
                                        End If
                                    End If

                                End If
                            End If
                        Next

                        If Not myGlobal.HasError And informAlarm Then
                            ' Update Alarm sensors to inform to Presentation layer what kind of management is need to display
                            PrepareUIRefreshEvent(Nothing, UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmId, True)
                            UpdateSensorValuesAttribute(AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE, myManageAlarmType, True)
                            IsServiceAlarmInformedAttr = True
                        End If

                    End If

                Else
                    ' Check if there exists any Deposits Alarm - by default these alarms belongs to 4_SIMPLE_ERROR alarm management type
                    If Not IsServiceAlarmInformedAttr Then
                        For Each alarmItem As Alarms In pAlarmIDList
                            Select Case alarmItem
                                Case AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR, AlarmEnumerates.Alarms.WASTE_DEPOSIT_ERR

                                    Dim myErrorCodesDS As New AlarmErrorCodesDS
                                    Dim myErrorCodesRow As AlarmErrorCodesDS.tfmwAlarmErrorCodesRow
                                    myErrorCodesRow = myErrorCodesDS.tfmwAlarmErrorCodes.NewtfmwAlarmErrorCodesRow
                                    myErrorCodesRow.AlarmID = alarmItem.ToString
                                    myErrorCodesRow.ErrorCode = -1
                                    myErrorCodesRow.ManagementID = ManagementAlarmTypes.SIMPLE_ERROR.ToString
                                    If alarmItem = AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR Then
                                        myErrorCodesRow.ResourceID = "ALM_WATER_DEPOSIT_ERR"
                                    Else
                                        myErrorCodesRow.ResourceID = "ALM_WASTE_DEPOSIT_ERR"
                                    End If
                                    myErrorCodesDS.tfmwAlarmErrorCodes.Rows.Add(myErrorCodesRow)
                                    AddErrorCodesToDisplay(myErrorCodesDS)

                                    ' Update Alarm sensors to inform to Presentation layer what kind of management is need to display
                                    PrepareUIRefreshEvent(Nothing, UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, alarmItem.ToString, True)
                                    UpdateSensorValuesAttribute(AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE, ManagementAlarmTypes.SIMPLE_ERROR, True)
                                    GlobalBase.CreateLogActivity("Deposits Alarm received [" & alarmItem.ToString & "] - Priority Management : " & ManagementAlarmTypes.SIMPLE_ERROR.ToString, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Information, False)
                                    IsServiceAlarmInformedAttr = True
                            End Select
                        Next
                    End If
                End If

                If myErrorCodesAttribute.Count > 0 Then
                    GlobalBase.CreateLogActivity("Alarm error codes received [" & ErrorCodes & "] - Priority Management : " & myManageAlarmType.ToString, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ManageAlarms_SRV", EventLogEntryType.Error, False)

            Finally
                If (pdbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return myGlobal
        End Function

#End Region

#Region "Individual alarm treatment Methods"

        ''' <summary>
        ''' Prepare the alarm list and alarm status list
        ''' Also apply the exception rules
        ''' 
        ''' NEW SPEC Eva 10/02/2012 - During start instrument only Sw only generates the alarms that could affect (pause) the process 
        '''                         - WASH_CONTAINER (ERROR or WARN) (wash solution bottle)
        '''                         - HIGH_CONTAMIN (ERROR or WARN) (high contamination bottle)
        '''                         - WATER_DEPOSIT (ERROR or SYSTEM_ERROR) (water deposit)
        '''                         - WASTE_DEPOSIT (ERROR or SYSTEM_ERROR) (waste deposit)
        ''' </summary>
        ''' <param name="pAlarmCode">Alarm identifier</param>
        ''' <param name="pAlarmStatus">TRUE: Alarm exists (active), FALSE: Alarm no exists</param>
        ''' <param name="pAlarmList">Final Alarm list to be processed</param>
        ''' <param name="pAlarmStatusList">Final Status for alarm list to be processed</param>
        ''' <param name="pAddInfo">Additional info used for volume missing or clot warning generated by ANSBxx</param>
        ''' <param name="pAdditionalInfoList">Additonal info list used for volume missing or clot warning generated by ANSBxx</param>
        ''' <param name="pAddAlwaysFlag" >TRUE: Add alarm always, do not take care if is Wup or Shown or ... are in process (used for code error alarms)</param>
        ''' <remarks>
        ''' Created by:  AG 18/03/2011
        ''' Modified by: AG 13/04/2012 - Added optional parameter pAddAlwaysFlag
        '''              AG 25/07/2012 - Added optional parameters pAddInfo and pAdditionalInfoList to be used for the volume missing and clot alarms
        '''              XB 03/11/2014 - Add new ISE Timeout Alarm - BA-1872
        '''              XB 06/11/2014 - Add new COMMS Timeour Alarm - BA-1872
        '''              AG 04/12/2014 BA-2236 add new optional parameters ErrorCode and pErrorCodesList
        ''' </remarks>
        Public Sub PrepareLocalAlarmList(ByVal pAlarmCode As Alarms, ByVal pAlarmStatus As Boolean, _
                                          ByRef pAlarmList As List(Of Alarms), ByRef pAlarmStatusList As List(Of Boolean), _
                                          Optional ByVal pAddInfo As String = "", _
                                          Optional ByRef pAdditionalInfoList As List(Of String) = Nothing, _
                                          Optional ByVal pAddAlwaysFlag As Boolean = False) Implements IAnalyzerManager.PrepareLocalAlarmList
            'NOTE: Do not use Try/Catch: the caller method implements it

            If (pAlarmStatus) Then
                'ALARM EXISTS
                '''''''''''''
                Dim addFlag As Boolean = True 'By default add all new alarms

                Select Case pAlarmCode
                    Case AlarmEnumerates.Alarms.REACT_TEMP_WARN 'Exception Nr.1: if exists Reactions rotor thermo error/nok do not add reaction rotor thermo warning!!
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.REACT_TEMP_ERR) Then addFlag = False

                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.REACT_TEMP_SYS_ERR) Then addFlag = False
                    Case AlarmEnumerates.Alarms.REACT_TEMP_ERR
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.REACT_TEMP_SYS_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.FRIDGE_TEMP_WARN 'Exception Nr.2: if exists Fridge thermo error/nok do not add fridge thermo warning!!
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.FRIDGE_TEMP_ERR) Then addFlag = False
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.FRIDGE_TEMP_SYS_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.FRIDGE_TEMP_ERR
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.FRIDGE_TEMP_SYS_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.HIGH_CONTAMIN_WARN 'Exception Nr.3: if exists High contamination deposit error do not add high contamination deposit warning!!
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.HIGH_CONTAMIN_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.WASH_CONTAINER_WARN 'Exception Nr.4: if exists Wash solution deposit error do not add Wash solution deposit warning!!
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASH_CONTAINER_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.R1_TEMP_WARN 'Exception Nr.5: if exists R1 thermo system error do not add R1 thermo warning!!
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.R1_TEMP_SYSTEM_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.R2_TEMP_WARN 'Exception Nr.6: if exists R2 thermo system error do not add R2 thermo warning!!
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.R2_TEMP_SYSTEM_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.WS_TEMP_WARN 'Exception Nr.7: if exists Washing station system error do not add washing station thermo warning!!
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WS_TEMP_SYSTEM_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR 'Exception Nr.8: if exists Water deposit system error do not add water deposit error (calculated by Sw)!!
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WATER_SYSTEM_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.WASTE_DEPOSIT_ERR 'Exception Nr.8: if exists Waste deposit system error do not add waste deposit error (calculated by Sw)!!
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASTE_SYSTEM_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.ISE_OFF_ERR 'Exception Nr.9: if exists ISE system error do not add ise status off !!
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.ISE_OFF_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.MAIN_COVER_WARN 'Exception Nr.10: if exists main cover error do not add main cover warn!! 'AG 15/03/2012
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.MAIN_COVER_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.FRIDGE_COVER_WARN 'Exception Nr.11: if exists fridge cover error do not add fridge cover warn!! 'AG 15/03/2012
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.FRIDGE_COVER_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.S_COVER_WARN 'Exception Nr.12: if exists samples cover error do not add samples cover warn!! 'AG 15/03/2012
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.S_COVER_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.REACT_COVER_WARN 'Exception Nr.13: if exists reactions cover error do not add reactions cover warn!! 'AG 15/03/2012
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.REACT_COVER_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.BASELINE_WELL_WARN 'Exception Nr.14: if exists base line error do not add BASE LINE WELL warn (baseline warn)!! 'AG 26/04/2012
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.BASELINE_INIT_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.BASELINE_INIT_ERR 'AG 07/09/2012 - In running if the Base line init error appears remove the METHACRYL_ROTOR_WARN alarm if exists
                        If AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING AndAlso myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.BASELINE_WELL_WARN) Then
                            myAlarmListAttribute.Remove(AlarmEnumerates.Alarms.BASELINE_WELL_WARN)
                        End If

                    Case AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR 'if exists ISE timeout or  ise status off do not add it again !!   ' XB 03/11/2014 - BA-1872
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.ISE_OFF_ERR) Or myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.COMMS_TIMEOUT_ERR 'if exists COMMS timeout do not add it again !!   ' XB 06/11/2014 - BA-1872
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.COMMS_TIMEOUT_ERR) Then addFlag = False

                    Case AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR) Or (CanManageRetryAlarm AndAlso NumSendingRepetitionsTimeout < 2) Then
                            addFlag = False                        
                        End If
                End Select
                'AG 10/02/2012 - While start instrument is inprocess only generate the alarms that affect the process
                Dim warmUpInProcess = String.Equals(mySessionFlags(AnalyzerManagerFlags.WUPprocess.ToString), "INPROCESS") OrElse _
                                      String.Equals(mySessionFlags(AnalyzerManagerFlags.WUPprocess.ToString), "PAUSED")

                'AG 13/04/2012 - when warm up is in process check if the alarm must be generated or not
                If warmUpInProcess AndAlso Not pAddAlwaysFlag Then
                    'If warmUpInProcess Then
                    addFlag = False 'By default no alarms generation

                    'Only those alarms affecting the start instrument process 
                    If Not IgnoreAlarmWhileWarmUp(pAlarmCode) Then
                        addFlag = True
                    End If
                End If

                'AG 18/05/2012 - While shutdown instrument is inprocess only generate the alarms that affect the process
                Dim sDownInProcess = String.Equals(mySessionFlags(AnalyzerManagerFlags.SDOWNprocess.ToString), "INPROCESS")

                If sDownInProcess AndAlso Not pAddAlwaysFlag Then
                    addFlag = False 'By default no alarms generation
                    If Not IgnoreAlarmWhileShutDown(pAlarmCode) Then
                        addFlag = True
                    End If
                End If
                'AG 18/05/2012

                'AG 28/02/2012 - rare case: sw shows message no comms but communications exists and sw continues receiving instructions (for example ANSINF)
                'When no comms Sw clears all alarms 
                'When Sw receives instructions (for instance ANSINF) the alarms will we generated and insert into database again and again every 2 seconds
                If addFlag AndAlso myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.COMMS_ERR) Then
                    'Software business alarms

                    ' XBC 23/03/2012
                    'If pAlarmCode <> AlarmEnumerates.Alarms.BASELINE_INIT_ERR AndAlso pAlarmCode <> AlarmEnumerates.Alarms.METHACRYL_ROTOR_WARN _
                    'AndAlso pAlarmCode <> AlarmEnumerates.Alarms.REPORTSATLOADED_WARN Then
                    If Not IgnoreAlarmWhileCommError(pAlarmCode) Then
                        addFlag = True
                    End If

                End If
                'AG 28/02/2012

                'Get the alarms defined with OKType = False (never are marked as solved)
                Dim alarmsWithOkTypeFalse As List(Of String) = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms _
                                                               Where a.OKType = False Select a.AlarmID).ToList

                If (addFlag) Then
                    'New alarm exists ... add to the list with TRUE status but only if the AlarmID doesn't exist in it
                    'BA-2384: call new function AddLocalActiveAlarmToList to add the new Alarm to the Alarms List
                    If (Not myAlarmListAttribute.Contains(pAlarmCode)) Then
                        AddLocalActiveAlarmToList(pAlarmCode, pAlarmList, pAlarmStatusList, pAddInfo, pAdditionalInfoList)
                    Else
                        'BA-2384: If the Alarm already exists but one or more FW Error Codes have been received for it, check if field AdditionalInfo needs to be
                        '         updated (by adding new FW Error Codes) and in this case, execute the UPDATE
                        If (Not String.IsNullOrEmpty(pAddInfo) AndAlso IsNumeric(pAddInfo.Replace(",", ""))) Then
                            Dim myGlobal = UpdateAdditionalInfoField(Nothing, pAlarmCode.ToString, pAddInfo)
                        End If

                        If (alarmsWithOkTypeFalse.Contains(pAlarmCode.ToString)) Then
                            'Exception: BASELINE_WELL_WARN (Change Reactions Rotor recommended). It can appear several times in the same WS but the alarm is generated just once
                            If (pAlarmCode <> AlarmEnumerates.Alarms.BASELINE_WELL_WARN) Then
                                AddLocalActiveAlarmToList(pAlarmCode, pAlarmList, pAlarmStatusList, pAddInfo, pAdditionalInfoList)
                            End If

                            'Base Line Init Error can appear several times in Running. For these Alarm repetitions, a flag defines when stop WS
                        ElseIf (pAlarmCode = AlarmEnumerates.Alarms.BASELINE_INIT_ERR) Then
                            If (wellBaseLineAutoPausesSession <> 0) Then
                                AddLocalActiveAlarmToList(pAlarmCode, pAlarmList, pAlarmStatusList, pAddInfo, pAdditionalInfoList)
                            End If
                        End If
                    End If
                End If

                'AG 15/03/2012 - Special code for AUTOrecove FREEZE alarms - if error code appears add it, but if the warning level already exists then remove it
                Select Case pAlarmCode
                    Case AlarmEnumerates.Alarms.MAIN_COVER_ERR
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.MAIN_COVER_WARN) Then
                            pAlarmList.Add(AlarmEnumerates.Alarms.MAIN_COVER_WARN) 'If cover error is added remove the warning level
                            pAlarmStatusList.Add(False)
                        End If
                    Case AlarmEnumerates.Alarms.FRIDGE_COVER_ERR
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.FRIDGE_COVER_WARN) Then
                            pAlarmList.Add(AlarmEnumerates.Alarms.FRIDGE_COVER_WARN) 'If cover error is added remove the warning level
                            pAlarmStatusList.Add(False)
                        End If
                    Case AlarmEnumerates.Alarms.REACT_COVER_ERR
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.REACT_COVER_WARN) Then
                            pAlarmList.Add(AlarmEnumerates.Alarms.REACT_COVER_WARN) 'If cover error is added remove the warning level
                            pAlarmStatusList.Add(False)
                        End If
                    Case AlarmEnumerates.Alarms.S_COVER_ERR
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.S_COVER_WARN) Then
                            pAlarmList.Add(AlarmEnumerates.Alarms.S_COVER_WARN) 'If cover error is added remove the warning level
                            pAlarmStatusList.Add(False)
                        End If
                        'Case AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR
                        '    If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR) Then
                        '        pAlarmList.Add(AlarmEnumerates.Alarms.GLF_BOARD_FBLD_ERR)
                        '        pAlarmStatusList.Add(False)
                        '    End If
                End Select
            Else
                'ALARM SOLVED
                '''''''''''''

                'Alarm solved ... add to list with FALSE status only if alarm exists
                If myAlarmListAttribute.Contains(pAlarmCode) Then
                    pAlarmList.Add(pAlarmCode)
                    pAlarmStatusList.Add(False)
                End If


                Dim OtherAlarmsSolved As Boolean = False
                Dim solvedErrAlarmID As New List(Of Alarms)

                'AG 05/01/2011
                'Special code Nr.1: if Reactions rotor thermo warning is solved also mark the error alarm as solved too!!
                If pAlarmCode = AlarmEnumerates.Alarms.REACT_TEMP_WARN Then
                    solvedErrAlarmID.Add(AlarmEnumerates.Alarms.REACT_TEMP_ERR)
                    OtherAlarmsSolved = True

                    'Special code Nr.2: if Firdge thermo warning is solved also mark the error/nok alarm as solved too!!
                ElseIf pAlarmCode = AlarmEnumerates.Alarms.FRIDGE_TEMP_WARN Then
                    solvedErrAlarmID.Add(AlarmEnumerates.Alarms.FRIDGE_TEMP_ERR)
                    OtherAlarmsSolved = True

                    'AG 13/03/2012
                    'Special code Nr.6: if R1 collision err (freeze) is solved also mark the r1 collision warn as solved too!!
                ElseIf pAlarmCode = AlarmEnumerates.Alarms.R1_COLLISION_ERR Then
                    solvedErrAlarmID.Add(AlarmEnumerates.Alarms.R1_COLLISION_WARN)
                    OtherAlarmsSolved = True

                    'Special code Nr.7: if R2 collision err (freeze) is solved also mark the r2 collision warn as solved too!!
                ElseIf pAlarmCode = AlarmEnumerates.Alarms.R2_COLLISION_ERR Then
                    solvedErrAlarmID.Add(AlarmEnumerates.Alarms.R2_COLLISION_WARN)
                    OtherAlarmsSolved = True

                    'Special code Nr.8: if samples collision err (freeze) is solved also mark the samples collision warn as solved too!!
                ElseIf pAlarmCode = AlarmEnumerates.Alarms.S_COLLISION_ERR Then
                    solvedErrAlarmID.Add(AlarmEnumerates.Alarms.S_COLLISION_WARN)
                    OtherAlarmsSolved = True
                    'AG 13/03/2012
                    'JV 08/01/2014 BT #1118
                ElseIf pAlarmCode = AlarmEnumerates.Alarms.ISE_OFF_ERR Then
                    solvedErrAlarmID.Add(AlarmEnumerates.Alarms.ISE_OFF_ERR)
                    OtherAlarmsSolved = True
                    'JV 08/01/2014 BT #1118
                ElseIf pAlarmCode = AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR Then
                    solvedErrAlarmID.Add(AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR)
                    OtherAlarmsSolved = True
                ElseIf pAlarmCode = AlarmEnumerates.Alarms.COMMS_TIMEOUT_ERR Then
                    solvedErrAlarmID.Add(AlarmEnumerates.Alarms.COMMS_TIMEOUT_ERR)
                    OtherAlarmsSolved = True
                End If

                'AG 28/03/2012 - Special code Nr.9: for auto recover freeze alarms (when cover warn solved also mark the cover error as solved too!!)
                If Not analyzerFREEZEFlagAttribute OrElse analyzerFREEZEModeAttribute = "AUTO" Then
                    If pAlarmCode = AlarmEnumerates.Alarms.MAIN_COVER_WARN Then
                        solvedErrAlarmID.Add(AlarmEnumerates.Alarms.MAIN_COVER_ERR)
                        OtherAlarmsSolved = True

                    ElseIf pAlarmCode = AlarmEnumerates.Alarms.FRIDGE_COVER_WARN Then
                        solvedErrAlarmID.Add(AlarmEnumerates.Alarms.FRIDGE_COVER_ERR)
                        OtherAlarmsSolved = True

                    ElseIf pAlarmCode = AlarmEnumerates.Alarms.S_COVER_WARN Then
                        solvedErrAlarmID.Add(AlarmEnumerates.Alarms.S_COVER_ERR)
                        OtherAlarmsSolved = True

                    ElseIf pAlarmCode = AlarmEnumerates.Alarms.REACT_COVER_WARN Then
                        solvedErrAlarmID.Add(AlarmEnumerates.Alarms.REACT_COVER_ERR)
                        OtherAlarmsSolved = True
                    End If
                End If
                'AG 28/03/2012

                If OtherAlarmsSolved Then
                    For Each item As Alarms In solvedErrAlarmID
                        If myAlarmListAttribute.Contains(item) Then
                            pAlarmList.Add(item)
                            pAlarmStatusList.Add(False)
                            'JV 08/01/2014 BT #1118
                        ElseIf item = AlarmEnumerates.Alarms.ISE_OFF_ERR Then
                            pAlarmList.Add(item)
                            pAlarmStatusList.Add(False)
                            'JV 08/01/2014 BT #1118
                        ElseIf item = AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR Then
                            pAlarmList.Add(item)
                            pAlarmStatusList.Add(False)
                        ElseIf item = AlarmEnumerates.Alarms.COMMS_TIMEOUT_ERR Then
                            pAlarmList.Add(item)
                            pAlarmStatusList.Add(False)
                        End If
                    Next

                End If

            End If
        End Sub

        ''' <summary>
        ''' Prepare the Error Code list
        ''' </summary>
        ''' <param name="pErrorCodeList">Fw Error identifiers</param>
        ''' <param name="pErrorCodeFinalList">Final Fw Error Code list to be processed</param>
        ''' <remarks>
        ''' Created XBC 16/10/2012
        ''' </remarks>
        Public Sub PrepareLocalAlarmList_SRV(ByVal pErrorCodeList As List(Of Integer), ByRef pErrorCodeFinalList As List(Of String)) Implements IAnalyzerManager.PrepareLocalAlarmList_SRV

            'NOTE: Do not use Try Catch do the caller method implements it

            ' Initialize Alarms list to empty content value 
            UpdateSensorValuesAttribute(AnalyzerSensors.SRV_MANAGEMENT_ALARM_TYPE, 0, True)

            For i As Integer = 0 To pErrorCodeList.Count - 1
                ' Add new received Fw error codes
                If Not pErrorCodeFinalList.Contains(pErrorCodeList(i).ToString) Then
                    pErrorCodeFinalList.Add(pErrorCodeList(i).ToString)
                End If
            Next

        End Sub

        ''' <summary>
        ''' Add to a local Alarms list the informed Alarm. This list will be used for Alarms treatment
        ''' </summary>
        ''' <param name="pAlarmCode">Alarm Identifier</param>
        ''' <param name="pAlarmList">List to return all Alarms</param>
        ''' <param name="pAlarmStatusList">List containing the Status of all Alarms</param>
        ''' <param name="pAddInfo">Additional information to save for the Alarm. Optional parameter.</param>
        ''' <param name="pAdditionalInfoList">List of Strings to store the additional information for the Alarm</param>
        ''' <remarks>
        ''' Created by:  SA 31/03/2015 - BA-2384 (Created in BA200 for implementation of BA-2236)
        ''' </remarks>
        Private Sub AddLocalActiveAlarmToList(ByVal pAlarmCode As Alarms, ByRef pAlarmList As List(Of Alarms), ByRef pAlarmStatusList As List(Of Boolean), _
                                              Optional ByVal pAddInfo As String = "", Optional ByRef pAdditionalInfoList As List(Of String) = Nothing)
            Try
                pAlarmList.Add(pAlarmCode)
                pAlarmStatusList.Add(True)

                'Additional Information is used to save information for Alarms related with Volume Missing, Clot Warnings, Blocked Preparations
                'and also the FW Error Code when an instruction ANSERR is received
                If (Not pAdditionalInfoList Is Nothing AndAlso pAddInfo <> String.Empty) Then pAdditionalInfoList.Add(pAddInfo)
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)
            End Try
        End Sub

        ''' <summary>
        ''' For Alarms that already exist for the Analyzer Work Session and having one or more FW Error Codes as additional information, if the Alarm is received again, this 
        ''' function verifies if the FW Error Codes received are already registered as additional information and for those not registered, add them to the additional information
        ''' field and update the Alarm in table twksWSAnalyzerAlarms
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pAlarmID">Alarm Identifier</param>
        ''' <param name="pAdditionalInfo">Additional information to save for the Alarm</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 13/04/2015 - BA-2384
        ''' </remarks>
        Private Function UpdateAdditionalInfoField(ByVal pDbConnection As SqlConnection, ByVal pAlarmId As String, ByVal pAdditionalInfo As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing

            Try
                myGlobal = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobal.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim updateRowFlag = False
                        Dim alarmsDelg As New WSAnalyzerAlarmsDelegate

                        myGlobal = alarmsDelg.GetByAlarmID(dbConnection, pAlarmID, Nothing, Nothing, AnalyzerIDAttribute, String.Empty)
                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                            Dim temporalDs = DirectCast(myGlobal.SetDatos, WSAnalyzerAlarmsDS)

                            If (temporalDS.twksWSAnalyzerAlarms.Rows.Count > 0) Then
                                If (Not String.IsNullOrEmpty(temporalDS.twksWSAnalyzerAlarms.First.AdditionalInfo)) Then
                                    Dim existingFwCodes = temporalDs.twksWSAnalyzerAlarms.First.AdditionalInfo.Split(","c)

                                    Dim codeExist = False
                                    Dim receivedFwCodes = pAdditionalInfo.Split(","c)
                                    For Each receivedCode As String In receivedFWCodes
                                        For Each existingCode As String In existingFWCodes
                                            codeExist = (receivedCode = existingCode)
                                            If (codeExist) Then Exit For
                                        Next

                                        If (Not codeExist) Then
                                            temporalDS.twksWSAnalyzerAlarms.First.BeginEdit()
                                            temporalDS.twksWSAnalyzerAlarms.First.AdditionalInfo &= "," & receivedCode
                                            temporalDS.twksWSAnalyzerAlarms.First.EndEdit()
                                            updateRowFlag = True
                                        End If
                                    Next
                                Else
                                    temporalDS.twksWSAnalyzerAlarms.First.BeginEdit()
                                    temporalDS.twksWSAnalyzerAlarms.First.AdditionalInfo = pAdditionalInfo
                                    temporalDS.twksWSAnalyzerAlarms.First.EndEdit()
                                    updateRowFlag = True
                                End If

                                If (updateRowFlag) Then
                                    myGlobal = alarmsDelg.Update(dbConnection, temporalDS)
                                End If
                            End If
                        End If

                        If (Not myGlobal.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobal = New GlobalDataTO()
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex)
            Finally
                If (pDbConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Verify if there are Temperature Warning Alarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pThermoFridge">Fridge Temperature</param>
        ''' <param name="pThermoReactions">Reactions Rotor Temperature</param>
        ''' <param name="pThermoWS">Washing Station Temperature</param>
        ''' <param name="pThermoR1">R1 Arm Temperature</param>
        ''' <param name="pThermoR2">R2 Arm Temperature</param>
        ''' <param name="pAlarmList"></param>
        ''' <param name="pAlarmStatusList"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 18/03/2011
        ''' Modified by: AG 30/03/2011 - Used myClassFieldLimitsDS class variable. Dont use delegate and read again
        '''              SA 25/05/2012 - Implementation changed due to a especification change by RP (17/05/2012):
        '''                                 ** Reactions Rotor Temp has to be between SetPointTemp-0.5 and SetPointTemp+0.5
        '''                                 ** Fridge Temp has to be between SetPointTemp-3 and SetPointTemp+7
        '''                                 ** Washing Station Temp has to be between SetPointTemp-5 and SetPointTemp+5
        '''                                 ** R1 Arm Temp/R2 Arm Temp have to be between SetPointTemp-15 and SetPointTemp+5
        '''                               Due to this, target temperature and offset calculations have been removed from the code
        '''              SA 29/05/2012 - Changes of 25/05/2012 have been commented and the previous function implementation has been
        '''                              recovered due to it is not clear how to implement those changes in Service Software 
        '''              SA 01/06/2012 - Changes of 25/05/2012 have been implemented again
        '''              XB 29/01/2013 - Change IsNumeric function by Double.TryParse method for Temperature values (Bugs tracking #1122)
        '''              XB 30/01/2013 - Add Trace log information about malfunctions on temperature values (Bugs tracking #1122)
        ''' </remarks>
        Private Function CheckTemperaturesAlarms(ByVal pDBConnection As SqlConnection, ByVal pThermoFridge As Single, _
                                                 ByVal pThermoReactions As Single, ByVal pThermoWS As Single, ByVal pThermoR1 As Single, _
                                                 ByVal pThermoR2 As Single, ByRef pAlarmList As List(Of Alarms), _
                                                 ByRef pAlarmStatusList As List(Of Boolean)) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            'Dim Utilities As New Utilities

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim alarmStatus As Boolean = False
                        Dim alarmId As Alarms = AlarmEnumerates.Alarms.NONE

                        Dim adjustValue As String = String.Empty
                        Dim tempSetPoint As Single = 0 'Temperature "consigna"

                        'THERMO REACTIONS ROTOR
                        'Read parameter THERMO_REACTIONS_LIMIT in tfmwFieldLimits to get Min/Max values for it
                        Dim limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                    Where a.LimitID = FieldLimitsEnum.THERMO_REACTIONS_LIMIT.ToString _
                                   Select a).ToList

                        If (limitList.Count > 0) Then
                            alarmId = AlarmEnumerates.Alarms.REACT_TEMP_WARN
                            alarmStatus = False

                            'Get the SetPoint Temperature
                            adjustValue = ReadAdjustValue(Ax00Adjustsments.GTGF)
                            'If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                            If (adjustValue <> String.Empty AndAlso _
                                Double.TryParse(adjustValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double)) Then
                                tempSetPoint = FormatToSingle(adjustValue)
                            Else
                                'Dim myLogAcciones2 As New ApplicationLogManager()
                                'GlobalBase.CreateLogActivity(Ax00Adjustsments.GTGF.ToString() & "Adjust Temperature value not valid [" & adjustValue & "]", "AnalyzerManager.CheckTemperaturesAlarms ", EventLogEntryType.Warning, False)
                                GlobalBase.CreateLogActivity(Ax00Adjustsments.GTGF.ToString() & "Adjust Temperature value not valid.", EventLogEntryType.Warning)
                            End If

                            'Verify the Reactions Rotor Temperature is inside the allowed limits: 
                            '       SetPointTemp(+MinLimit <= ReactionRotorsTemp <= SetPointTemp + MaxLimit)
                            'If it is outside the allowed limits, then raise alarm REACT_TEMP_WARN
                            If (pThermoReactions < (tempSetPoint + limitList(0).MinValue)) Or _
                               (pThermoReactions > (tempSetPoint + limitList(0).MaxValue)) Then
                                alarmStatus = True

                                'Alarm will be generated only if Reactions Cover detection is enabled
                                adjustValue = ReadAdjustValue(Ax00Adjustsments.PHCOV) '0=Disabled / 1=Enabled
                                Dim reactionsCoverDetectionEnabled As Boolean = True
                                If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                                    reactionsCoverDetectionEnabled = CType(adjustValue, Boolean)
                                    If (Not reactionsCoverDetectionEnabled) Then alarmStatus = False
                                End If
                            Else
                                alarmStatus = False
                            End If
                            PrepareLocalAlarmList(alarmId, alarmStatus, pAlarmList, pAlarmStatusList)
                        End If

                        'THERMO FRIDGE (Reagents Rotor)
                        'Read parameter THERMO_FRIDGE_LIMIT in tfmwFieldLimits to get Min/Max values for it
                        limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                    Where a.LimitID = FieldLimitsEnum.THERMO_FRIDGE_LIMIT.ToString _
                                   Select a).ToList

                        If (limitList.Count > 0) Then
                            alarmId = AlarmEnumerates.Alarms.FRIDGE_TEMP_WARN
                            alarmStatus = False

                            'Get the SetPoint Temperature
                            adjustValue = ReadAdjustValue(Ax00Adjustsments.GTN1)
                            'If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                            If (adjustValue <> String.Empty AndAlso _
                                Double.TryParse(adjustValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double)) Then
                                tempSetPoint = FormatToSingle(adjustValue)
                            Else
                                'Dim myLogAcciones2 As New ApplicationLogManager()
                                'GlobalBase.CreateLogActivity(Ax00Adjustsments.GTN1.ToString() & "Adjust Temperature value not valid [" & adjustValue & "]", "AnalyzerManager.CheckTemperaturesAlarms ", EventLogEntryType.Warning, False)
                                GlobalBase.CreateLogActivity(Ax00Adjustsments.GTN1.ToString() & "Adjust Temperature value not valid.", EventLogEntryType.Warning)
                            End If

                            'Verify the Fridge Temperature is inside the allowed limits: 
                            '       SetPointTemp(+MinLimit <= FridgeTemp <= SetPointTemp + MaxLimit)
                            'If it is outside the allowed limits, then raise alarm FRIDGE_TEMP_WARN
                            If (pThermoFridge < (tempSetPoint + limitList(0).MinValue)) Or _
                               (pThermoFridge > (tempSetPoint + limitList(0).MaxValue)) Then
                                alarmStatus = True

                                'Alarm will be generated only if Fridge Cover detection is enabled
                                adjustValue = ReadAdjustValue(Ax00Adjustsments.RCOV) '0=Disabled / 1=Enabled
                                Dim fridgeCoverDetectionEnabled As Boolean = True
                                If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                                    fridgeCoverDetectionEnabled = CType(adjustValue, Boolean)
                                    If (Not fridgeCoverDetectionEnabled) Then alarmStatus = False
                                End If
                            Else
                                alarmStatus = False
                            End If
                            PrepareLocalAlarmList(alarmId, alarmStatus, pAlarmList, pAlarmStatusList)
                        End If

                        'THERMO WASHING STATION
                        'Read parameter THERMO_WASHSTATION_LIMIT in tfmwFieldLimits to get Min/Max values for it
                        limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                    Where a.LimitID = FieldLimitsEnum.THERMO_WASHSTATION_LIMIT.ToString _
                                   Select a).ToList

                        If (limitList.Count > 0) Then
                            alarmId = AlarmEnumerates.Alarms.WS_TEMP_WARN
                            alarmStatus = False

                            'Get the SetPoint Temperature
                            adjustValue = ReadAdjustValue(Ax00Adjustsments.GTH1)
                            'If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                            If (adjustValue <> String.Empty AndAlso _
                                Double.TryParse(adjustValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double)) Then
                                tempSetPoint = FormatToSingle(adjustValue)
                            Else
                                'Dim myLogAcciones2 As New ApplicationLogManager()
                                'GlobalBase.CreateLogActivity(Ax00Adjustsments.GTH1.ToString() & "Adjust Temperature value not valid [" & adjustValue & "]", "AnalyzerManager.CheckTemperaturesAlarms ", EventLogEntryType.Warning, False)
                                GlobalBase.CreateLogActivity(Ax00Adjustsments.GTH1.ToString() & "Adjust Temperature value not valid.", EventLogEntryType.Warning)
                            End If

                            'Verify the Washing Station Temperature is inside the allowed limits: 
                            '       SetPointTemp(+MinLimit <= WashingStationTemp <= SetPointTemp + MaxLimit)
                            'If it is outside the allowed limits, then raise alarm WS_TEMP_WARN
                            If (pThermoWS < (tempSetPoint + limitList(0).MinValue)) Or _
                               (pThermoWS > (tempSetPoint + limitList(0).MaxValue)) Then
                                alarmStatus = True
                            Else
                                alarmStatus = False
                            End If
                            PrepareLocalAlarmList(alarmId, alarmStatus, pAlarmList, pAlarmStatusList)
                        End If

                        'AG 05/01/2012 - Do not treat R1 or R2 temperature in RUNNING mode
                        If (AnalyzerStatusAttribute <> AnalyzerManagerStatus.RUNNING) Then
                            'THERMO R1 ARM / R2 ARM
                            'This alarms can be triggered by Sw if out of limits or by Fw error code Exxx instead R1 or R2 temperatures XX,X

                            'Read parameter THERMO_ARMS_LIMIT in tfmwFieldLimits to get Min/Max values for it
                            limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                        Where a.LimitID = FieldLimitsEnum.THERMO_ARMS_LIMIT.ToString _
                                       Select a).ToList

                            If (limitList.Count > 0) Then
                                'THERMO R1 ARM
                                alarmStatus = False

                                'Get the SetPoint Temperarure
                                adjustValue = ReadAdjustValue(Ax00Adjustsments.GTR1)
                                'If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                                If (adjustValue <> String.Empty AndAlso _
                                    Double.TryParse(adjustValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double)) Then
                                    tempSetPoint = FormatToSingle(adjustValue)
                                Else
                                    'Dim myLogAcciones2 As New ApplicationLogManager()
                                    'GlobalBase.CreateLogActivity(Ax00Adjustsments.GTR1.ToString() & "Adjust Temperature value not valid [" & adjustValue & "]", "AnalyzerManager.CheckTemperaturesAlarms ", EventLogEntryType.Warning, False)
                                    GlobalBase.CreateLogActivity(Ax00Adjustsments.GTR1.ToString() & "Adjust Temperature value not valid.", EventLogEntryType.Warning)
                                End If

                                'Verify the R1 Arm Temperature is inside the allowed limits: 
                                '       SetPointTemp(+MinLimit <= R1ArmTemp <= SetPointTemp + MaxLimit)
                                'If it is outside the allowed limits, then activate the Warning Timer for R1 Arm
                                If (pThermoR1 < (tempSetPoint + limitList(0).MinValue)) Or _
                                   (pThermoR1 > (tempSetPoint + limitList(0).MaxValue)) Then
                                    alarmStatus = True
                                    If (Not thermoR1ArmWarningTimer.Enabled) Then 'AG 05/01/2012 - Activate waiting temporal time period
                                        thermoR1ArmWarningTimer.Enabled = True
                                    End If
                                Else
                                    alarmStatus = False
                                    If (thermoR1ArmWarningTimer.Enabled) Then 'AG 05/01/2012 Deactivate waiting temporal time period
                                        thermoR1ArmWarningTimer.Enabled = False
                                    End If
                                End If

                                If (Not alarmStatus) Then
                                    alarmId = AlarmEnumerates.Alarms.R1_TEMP_WARN
                                    PrepareLocalAlarmList(alarmId, alarmStatus, pAlarmList, pAlarmStatusList)
                                End If

                                'THERMO R2 ARM
                                alarmStatus = False

                                'Get the SetPoint Temperarure
                                adjustValue = ReadAdjustValue(Ax00Adjustsments.GTR2)
                                'If (adjustValue <> String.Empty AndAlso IsNumeric(adjustValue)) Then
                                If (adjustValue <> String.Empty AndAlso _
                                    Double.TryParse(adjustValue, NumberStyles.Any, CultureInfo.InvariantCulture, New Double)) Then
                                    tempSetPoint = FormatToSingle(adjustValue)
                                Else
                                    'Dim myLogAcciones2 As New ApplicationLogManager()
                                    'GlobalBase.CreateLogActivity(Ax00Adjustsments.GTR2.ToString() & "Adjust Temperature value not valid [" & adjustValue & "]", "AnalyzerManager.CheckTemperaturesAlarms ", EventLogEntryType.Warning, False)
                                    GlobalBase.CreateLogActivity(Ax00Adjustsments.GTR2.ToString() & "Adjust Temperature value not valid.", EventLogEntryType.Warning)
                                End If

                                'Verify the R2 Arm Temperature is inside the allowed limits: 
                                '       SetPointTemp(+MinLimit <= R2ArmTemp <= SetPointTemp + MaxLimit)
                                'If it is outside the allowed limits, then activate the Warning Timer for R2 Arm
                                If (pThermoR2 < (tempSetPoint + limitList(0).MinValue)) Or _
                                   (pThermoR2 > (tempSetPoint + limitList(0).MaxValue)) Then
                                    alarmStatus = True
                                    If (Not thermoR2ArmWarningTimer.Enabled) Then 'AG 05/01/2012 - Activate waiting temporal time period
                                        thermoR2ArmWarningTimer.Enabled = True
                                    End If
                                Else
                                    alarmStatus = False
                                    If (thermoR2ArmWarningTimer.Enabled) Then 'AG 05/01/2012 Deactivate waiting temporal time period
                                        thermoR2ArmWarningTimer.Enabled = False
                                    End If
                                End If

                                If (Not alarmStatus) Then
                                    alarmId = AlarmEnumerates.Alarms.R2_TEMP_WARN
                                    PrepareLocalAlarmList(alarmId, alarmStatus, pAlarmList, pAlarmStatusList)
                                End If
                            End If
                        Else
                            'AG 05/01/2012 Deactivate waiting temporal time period
                            If (thermoR1ArmWarningTimer.Enabled) Then thermoR1ArmWarningTimer.Enabled = False
                            If (thermoR2ArmWarningTimer.Enabled) Then thermoR2ArmWarningTimer.Enabled = False
                        End If

                        'AG 01/04/2011 - Inform new numerical values for Ax00 sensors to be monitorized
                        UpdateSensorValuesAttribute(AnalyzerSensors.TEMPERATURE_REACTIONS, pThermoReactions, False)
                        UpdateSensorValuesAttribute(AnalyzerSensors.TEMPERATURE_FRIDGE, pThermoFridge, False)
                        UpdateSensorValuesAttribute(AnalyzerSensors.TEMPERATURE_WASHINGSTATION, pThermoWS, False)

                        'AG 05/01/2012 - Do not treat R1 or R2 temperature in RUNNING mode
                        If (AnalyzerStatusAttribute <> AnalyzerManagerStatus.RUNNING) Then
                            UpdateSensorValuesAttribute(AnalyzerSensors.TEMPERATURE_R1, pThermoR1, False)
                            UpdateSensorValuesAttribute(AnalyzerSensors.TEMPERATURE_R2, pThermoR2, False)
                        End If

                        limitList = Nothing 'AG 02/08/2012 - free memory
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.CheckTemperaturesAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Check if exists or not High contamination or Washing solution container alarms
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHighContaminationContainer"></param>
        ''' <param name="pWashSolutionContainer"></param>
        ''' <param name="pAlarmList" ></param>
        ''' <param name="pAlarmStatusList" ></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' AG 24/03/2011 creation
        ''' AG 30/03/2011 - used myClassFieldLimitsDS class variable. Dont use delegate and read again
        ''' </remarks>
        Private Function CheckContainerAlarms(ByVal pDBConnection As SqlConnection, ByVal pHighContaminationContainer As Single, ByVal pWashSolutionContainer As Single, _
                                              ByRef pAlarmList As List(Of Alarms), ByRef pAlarmStatusList As List(Of Boolean)) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                            Dim myPercentage As Single = 0
                            Dim newSensorNumericalValueFlag As Boolean = False 'AG 01/04/2011 - Inform new numerical values for Ax00 sensors to be monitorized

                            Dim alarmId As Alarms = AlarmEnumerates.Alarms.NONE
                            Dim alarmStatus As Boolean = False
                            Dim limitList As New List(Of FieldLimitsDS.tfmwFieldLimitsRow)
                            'Dim Utilities As New Utilities

                            'High contamination container
                            'Read the parameter HIGH_CONTAMIN_DEPOSIT_LIMIT in tfmwFieldLimits
                            limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                         Where a.LimitID = FieldLimitsEnum.HIGH_CONTAMIN_DEPOSIT_LIMIT.ToString _
                                         Select a).ToList

                            If limitList.Count > 0 Then
                                myPercentage = 0 'By default no alarm 
                                'Calculate the percentage using pHighContaminationContainer) get value into variable myPercentage
                                resultData = GetBottlePercentage(CInt(pHighContaminationContainer), False)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myPercentage = CSng(CType(resultData.SetDatos, Double))
                                    myPercentage = FormatToSingle(ToStringWithFormat(myPercentage, 0)) 'AG 21/12/2011 format 0 decimals BugsTrackings 258 (Elena) ('AG 04/10/2011 - format mypercentage with 1 decimal)

                                    'AG 22/02/2012 Only inform new numerical value when the alarm is not locked to prevent level oscillations
                                    ''AG 01/04/2011 - Inform new numerical values for Ax00 sensors to be monitorized
                                    'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE, myPercentage, True)
                                End If

                                'Vol > Critical limit (error)
                                alarmId = AlarmEnumerates.Alarms.HIGH_CONTAMIN_ERR
                                alarmStatus = False
                                If myPercentage >= limitList(0).MaxValue Then
                                    'Alarm Warning
                                    alarmStatus = True
                                    UpdateSensorValuesAttribute(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE, myPercentage, True) 'AG 22/02/2012 - Inform new numerical values for Ax00 sensors to be monitorized
                                Else
                                    alarmStatus = False

                                    'AG 21/02/2012 - To prevent the ERROR or WARNING alarm level oscillations when near the limit (electronics tolerances),
                                    'when the ERROR level alarm exists it is remove only after user has pressed the "Change Bottle Confirmation" button
                                    If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.HIGH_CONTAMIN_ERR) Then
                                        If mySessionFlags(AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS" Then
                                            alarmStatus = True
                                        End If
                                    End If

                                    If Not alarmStatus Then
                                        UpdateSensorValuesAttribute(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE, myPercentage, True) 'AG 22/02/2012 - Inform new numerical values for Ax00 sensors to be monitorized
                                    End If
                                    'AG 21/02/2012

                                End If
                                PrepareLocalAlarmList(alarmId, alarmStatus, pAlarmList, pAlarmStatusList)

                                'If not exist high contamination error ... check if warning
                                If Not alarmStatus Then
                                    'Vol > Warning limit & Vol < Critical limit (warning)
                                    alarmId = AlarmEnumerates.Alarms.HIGH_CONTAMIN_WARN
                                    alarmStatus = False
                                    If myPercentage >= limitList(0).MinValue And myPercentage < limitList(0).MaxValue Then
                                        'Alarm Warning
                                        alarmStatus = True
                                    Else
                                        alarmStatus = False

                                        'AG 21/02/2012 - To prevent the WARNING or OK alarm level oscillations when near the limit (electronics tolerances),
                                        'when the WARNING level alarm exists it is remove only after user has pressed the "Change Bottle Confirmation" button or after generate the ERROR alarm level
                                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.HIGH_CONTAMIN_WARN) Then
                                            If mySessionFlags(AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS" Then
                                                alarmStatus = True
                                            End If
                                        End If
                                        'AG 21/02/2012
                                    End If

                                Else 'If exists error warning can not be active
                                    alarmId = AlarmEnumerates.Alarms.HIGH_CONTAMIN_WARN
                                    alarmStatus = False
                                End If
                                PrepareLocalAlarmList(alarmId, alarmStatus, pAlarmList, pAlarmStatusList)

                            End If


                            'Wash solution container
                            'Read the parameter WASH_SOLUTION_DEPOSIT_LIMIT in tfmwFieldLimits
                            limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                        Where a.LimitID = FieldLimitsEnum.WASH_SOLUTION_DEPOSIT_LIMIT.ToString _
                                        Select a).ToList

                            If limitList.Count > 0 Then
                                myPercentage = 100 'By Default no alarm 
                                'Calculate the percentage using pWashSolutionContainer) get value into variable myPercentage
                                resultData = GetBottlePercentage(CInt(pWashSolutionContainer), True)
                                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                                    myPercentage = CSng(CType(resultData.SetDatos, Double))
                                    myPercentage = FormatToSingle(ToStringWithFormat(myPercentage, 0)) 'AG 21/12/2011 format 0 decimals BugsTrackings 258 (Elena)  'AG 04/10/2011 - format mypercentage with 1 decimal

                                    'AG 22/02/2012 Only inform new numerical value when the alarm is not locked to prevent level oscillations
                                    ''AG 01/04/2011 - Inform new numerical values for Ax00 sensors to be monitorized
                                    'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.BOTTLE_WASHSOLUTION, myPercentage, True)
                                End If

                                'Vol < Critical limit (error)
                                alarmId = AlarmEnumerates.Alarms.WASH_CONTAINER_ERR
                                alarmStatus = False
                                If myPercentage <= limitList(0).MinValue Then
                                    'Alarm Warning
                                    alarmStatus = True
                                    UpdateSensorValuesAttribute(AnalyzerSensors.BOTTLE_WASHSOLUTION, myPercentage, True) 'AG 22/02/2012 - Inform new numerical values for Ax00 sensors to be monitorized

                                Else
                                    alarmStatus = False

                                    'AG 21/02/2012 - To prevent the ERROR or WARNING alarm level oscillations when near the limit (electronics tolerances),
                                    'when the ERROR level alarm exists it is remove only after user has pressed the "Change Bottle Confirmation" button
                                    If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASH_CONTAINER_ERR) Then
                                        If mySessionFlags(AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS" Then
                                            alarmStatus = True
                                        End If
                                    End If

                                    If Not alarmStatus Then
                                        UpdateSensorValuesAttribute(AnalyzerSensors.BOTTLE_WASHSOLUTION, myPercentage, True) 'AG 22/02/2012 - Inform new numerical values for Ax00 sensors to be monitorized
                                    End If
                                    'AG 21/02/2012

                                End If
                                PrepareLocalAlarmList(alarmId, alarmStatus, pAlarmList, pAlarmStatusList)

                                'If not exist wash solution container error ... check if warning
                                If Not alarmStatus Then
                                    'Vol < Warning limit & Vol > Critical limit (warning)
                                    alarmId = AlarmEnumerates.Alarms.WASH_CONTAINER_WARN
                                    alarmStatus = False
                                    If myPercentage > limitList(0).MinValue And myPercentage <= limitList(0).MaxValue Then
                                        'Alarm Warning
                                        alarmStatus = True
                                    Else
                                        alarmStatus = False

                                        'AG 21/02/2012 - To prevent the WARNING or OK alarm level oscillations when near the limit (electronics tolerances),
                                        'when the WARNING level alarm exists it is remove only after user has pressed the "Change Bottle Confirmation" button or after generate the ERROR alarm level
                                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASH_CONTAINER_WARN) Then
                                            If mySessionFlags(AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS" Then
                                                alarmStatus = True
                                            End If
                                        End If
                                        'AG 21/02/2012

                                    End If

                                Else 'If exists error warning can not be active
                                    alarmId = AlarmEnumerates.Alarms.WASH_CONTAINER_WARN
                                    alarmStatus = False
                                End If
                                PrepareLocalAlarmList(alarmId, alarmStatus, pAlarmList, pAlarmStatusList)

                            End If
                            limitList = Nothing 'AG 02/08/2012 - free memory

                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.CheckContainerAlarms", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' SW has received and ANSINF instruction and now check for alarms
        ''' User Sw has his own threatment different form service
        ''' </summary>
        ''' <param name="pSensors "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 14/04/2011
        ''' Modified by: XB 27/05/2014 - BT #1630 ==> Fix bug Abort+Reset after Tanks Alarms solved (in stanby requires user clicks button Change Bottle confirm, not automatically fixed as in Running)
        '''              IT 03/12/2014 - BA-2075
        ''' </remarks>
        Private Function UserSwANSINFTreatment(ByVal pSensors As Dictionary(Of AnalyzerSensors, Single)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try

                Dim myAlarmList As New List(Of Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)
                Dim alarmID As Alarms = AlarmEnumerates.Alarms.NONE
                Dim alarmStatus As Boolean = False
                'Dim myLogAcciones As New ApplicationLogManager() 'AG 29/05/2014 - #1630 add traces informing when deposit sensors activate/deactivate timers

                'Get General cover (parameter index 3)
                Dim myIntValue As Integer = 0
                If pSensors.ContainsKey(AnalyzerSensors.COVER_GENERAL) Then
                    myIntValue = CInt(pSensors(AnalyzerSensors.COVER_GENERAL))
                    alarmID = AlarmEnumerates.Alarms.MAIN_COVER_WARN

                    If myIntValue = 1 Then 'Open
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.COVER_GENERAL, pSensors(AnalyzerSensors.COVER_GENERAL), False)
                End If


                'Get Photometrics (Reaccions) cover (parameter index 4)
                If pSensors.ContainsKey(AnalyzerSensors.COVER_REACTIONS) Then
                    myIntValue = CInt(pSensors(AnalyzerSensors.COVER_REACTIONS))
                    alarmID = AlarmEnumerates.Alarms.REACT_COVER_WARN

                    If myIntValue = 1 Then 'Open
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.COVER_REACTIONS, pSensors(AnalyzerSensors.COVER_REACTIONS), False)
                End If


                'Get Reagents (Fridge) cover (parameter index 5)
                If pSensors.ContainsKey(AnalyzerSensors.COVER_FRIDGE) Then
                    myIntValue = CInt(pSensors(AnalyzerSensors.COVER_FRIDGE))
                    alarmID = AlarmEnumerates.Alarms.FRIDGE_COVER_WARN

                    If myIntValue = 1 Then 'Open
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.COVER_FRIDGE, pSensors(AnalyzerSensors.COVER_FRIDGE), False)
                End If


                'Get Samples cover (parameter index 6)
                If pSensors.ContainsKey(AnalyzerSensors.COVER_SAMPLES) Then
                    myIntValue = CInt(pSensors(AnalyzerSensors.COVER_SAMPLES))
                    alarmID = AlarmEnumerates.Alarms.S_COVER_WARN

                    If myIntValue = 1 Then 'Open
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.COVER_SAMPLES, pSensors(AnalyzerSensors.COVER_SAMPLES), False)
                End If


                'Get System liquid sensor (parameter index 7)
                If pSensors.ContainsKey(AnalyzerSensors.WATER_DEPOSIT) Then
                    'AG 01/12/2011 - Sw do not generates the alarm WATER_DEPOSIT_ERR now, it activates a timer it is who generates the alarm if the time expires
                    alarmID = AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR
                    myIntValue = CInt(pSensors(AnalyzerSensors.WATER_DEPOSIT))

                    If myIntValue = 0 Or myIntValue = 2 Then 'Empty or impossible status
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False

                        'AG 29/05/2014 - BT #1630 - Alarm fixed automatically in running but in standby requires user action (click ChangeBottleButton)
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR) AndAlso _
                           (AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY AndAlso mySessionFlags(AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS") Then
                            alarmStatus = True ' Keep alarm until user clicks confirmation
                        End If
                        'AG 29/05/2014 - BT #1630
                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.WATER_DEPOSIT, pSensors(AnalyzerSensors.WATER_DEPOSIT), False)

                    'AG 29/05/2014 - #1630 - Do not activate timer is alarm is already active!!
                    'If alarmStatus AndAlso Not waterDepositTimer.Enabled Then
                    If alarmStatus AndAlso Not waterDepositTimer.Enabled AndAlso Not myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR) Then
                        GlobalBase.CreateLogActivity("Water deposit empty!! Enable timer waterDepositTimer. Sensor value: " & myIntValue, "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Information, False)
                        waterDepositTimer.Enabled = True 'Activate timer (but NOT ACTIVATE ALARM!!!)

                    ElseIf Not alarmStatus AndAlso waterDepositTimer.Enabled Then
                        GlobalBase.CreateLogActivity("Water deposit OK!! Disable timer waterDepositTimer. Sensor value: " & myIntValue, "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Information, False)
                        waterDepositTimer.Enabled = False  'Deactivate timer
                    End If
                    'AG 01/12/2011

                    If Not alarmStatus Then 'AG 09/01/2012 Remove alarm (if exists)
                        PrepareLocalAlarmList(alarmID, False, myAlarmList, myAlarmStatusList)
                    End If

                End If


                'Get Waste sensor (parameter index 8)
                If pSensors.ContainsKey(AnalyzerSensors.WASTE_DEPOSIT) Then
                    'AG 01/12/2011 - Sw do not generates the alarm WASTE_DEPOSIT_ERR now, it activates a timer it is who generates the alarm if the time expires
                    alarmID = AlarmEnumerates.Alarms.WASTE_DEPOSIT_ERR

                    myIntValue = CInt(pSensors(AnalyzerSensors.WASTE_DEPOSIT))

                    If myIntValue = 3 Or myIntValue = 2 Then 'Full or impossible status
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False

                        'AG 29/05/2014 - BT #1630 - Alarm fixed automatically in running but in standby requires user action (click ChangeBottleButton)
                        If myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASTE_DEPOSIT_ERR) AndAlso _
                           (AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY AndAlso mySessionFlags(AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) <> "INPROCESS") Then
                            alarmStatus = True ' Keep alarm until user clicks confirmation
                        End If
                        'AG 29/05/2014 - BT #1630

                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.WASTE_DEPOSIT, pSensors(AnalyzerSensors.WASTE_DEPOSIT), False)

                    'AG 29/05/2014 - #1630 - Do not activate timer is alarm is already active!!
                    'If alarmStatus AndAlso Not wasteDepositTimer.Enabled Then
                    If alarmStatus AndAlso Not wasteDepositTimer.Enabled AndAlso Not myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.WASTE_DEPOSIT_ERR) Then
                        GlobalBase.CreateLogActivity("Waste deposit full!! Enable timer wasteDepositTimer. Sensor value: " & myIntValue, "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Information, False)
                        wasteDepositTimer.Enabled = True 'Activate timer (but NOT ACTIVATE ALARM!!!)

                    ElseIf Not alarmStatus AndAlso wasteDepositTimer.Enabled Then
                        GlobalBase.CreateLogActivity("Waste deposit OK!! Disable timer wasteDepositTimer. Sensor value: " & myIntValue, "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Information, False)
                        wasteDepositTimer.Enabled = False  'Deactivate timer
                    End If
                    'AG 01/12/2011

                    If Not alarmStatus Then 'AG 09/01/2012 Remove alarm (if exists)
                        PrepareLocalAlarmList(alarmID, False, myAlarmList, myAlarmStatusList)
                    End If


                End If


                'Get Weight sensors (wash solution & high contamination waste) (parameter index 9, 10)
                If pSensors.ContainsKey(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE) And _
                    pSensors.ContainsKey(AnalyzerSensors.BOTTLE_WASHSOLUTION) Then

                    'Evaluate if exists or not alarm
                    CheckContainerAlarms(Nothing, pSensors(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE), _
                                         pSensors(AnalyzerSensors.BOTTLE_WASHSOLUTION), myAlarmList, myAlarmStatusList)

                End If


                'Get Fridge status (parameter index 12)
                If pSensors.ContainsKey(AnalyzerSensors.FRIDGE_STATUS) Then
                    myIntValue = CInt(pSensors(AnalyzerSensors.FRIDGE_STATUS))
                    alarmID = AlarmEnumerates.Alarms.FRIDGE_STATUS_WARN

                    If myIntValue = 0 Then 'OFF
                        alarmStatus = True
                    Else 'ON
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.FRIDGE_STATUS, pSensors(AnalyzerSensors.FRIDGE_STATUS), False)
                End If


                'Get Temperature values (reactions, fridge, wash station heater, R1, R2) (parameter index 11, 13, 14, 15, 16)
                If pSensors.ContainsKey(AnalyzerSensors.TEMPERATURE_FRIDGE) And _
                    pSensors.ContainsKey(AnalyzerSensors.TEMPERATURE_REACTIONS) And _
                    pSensors.ContainsKey(AnalyzerSensors.TEMPERATURE_WASHINGSTATION) And _
                    pSensors.ContainsKey(AnalyzerSensors.TEMPERATURE_R1) And _
                    pSensors.ContainsKey(AnalyzerSensors.TEMPERATURE_R2) Then

                    'Evaluate if exists or not alarm
                    CheckTemperaturesAlarms(Nothing, pSensors(AnalyzerSensors.TEMPERATURE_FRIDGE), _
                                            pSensors(AnalyzerSensors.TEMPERATURE_REACTIONS), _
                                            pSensors(AnalyzerSensors.TEMPERATURE_WASHINGSTATION), _
                                            pSensors(AnalyzerSensors.TEMPERATURE_R1), _
                                            pSensors(AnalyzerSensors.TEMPERATURE_R2), myAlarmList, myAlarmStatusList)

                End If


                'Get ISE status (parameter index 17)
                '(this alarm is only generated when Instrument has ISE module installed - adjust ISEINS = 1)
                If pSensors.ContainsKey(AnalyzerSensors.ISE_STATUS) Then
                    myIntValue = CInt(pSensors(AnalyzerSensors.ISE_STATUS))
                    alarmID = AlarmEnumerates.Alarms.ISE_OFF_ERR

                    If myIntValue = 0 Then 'OFF
                        'AG 23/11/2011 - this alarm is only generated when Instrument has ISE module installed - adjust ISEINS = 1
                        'alarmStatus = True
                        alarmStatus = False
                        Dim ISEInstalledValue As String = ""
                        ISEInstalledValue = ReadAdjustValue(Ax00Adjustsments.ISEINS)
                        If Not String.Equals(ISEInstalledValue, String.Empty) AndAlso IsNumeric(ISEInstalledValue) Then
                            If CType(ISEInstalledValue, Boolean) Then
                                alarmStatus = True
                            End If
                        End If
                        'AG 23/11/2011

                    Else 'ON
                        alarmStatus = False
                    End If
                    PrepareLocalAlarmList(alarmID, alarmStatus, myAlarmList, myAlarmStatusList)

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.ISE_STATUS, pSensors(AnalyzerSensors.ISE_STATUS), False)
                End If

                ''''''''
                'Finally call manage all alarms detected (new or solved)
                Dim instrAddedToLogFlag As Boolean = False 'Indicates if the instruction has been added to log file
                If myAlarmList.Count > 0 Then
                    'NOTE: Here do not place different code for service or user because this method is called only for the user Software
                    Dim currentAlarms = New AnalyzerAlarms(Me)
                    myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)

                    'AG 18/06/2012 - The ANSINF instruction is saved to log only when new alarms are generated or solved
                    '                Previous code only saved it when new alarms were generated
                    'Dim newNoProbeTemperatureAlarms As List(Of Boolean)
                    'newNoProbeTemperatureAlarms = (From a As Boolean In myAlarmStatusList Where a = True Select a).ToList
                    'If newNoProbeTemperatureAlarms.Count > 0 Then

                    If ((myAlarmList.Count = 1 AndAlso Not myAlarmList.Contains(AlarmEnumerates.Alarms.ISE_OFF_ERR)) OrElse myAlarmList.Count > 1) Then 'AG + XB 08/04/2014 - #1118 (do not save always ANSINF in log due to ISE_OFF_ERR)
                        ''Dim myLogAcciones As New ApplicationLogManager() 'AG 29/05/2014 - #1630 declare at the beginning
                        GlobalBase.CreateLogActivity("Received Instruction [" & AppLayer.InstructionReceived & "]", "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Information, False)
                        instrAddedToLogFlag = True
                    End If
                    'End If
                    'AG 04/10/2011
                End If

                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS
                If Not analyzerFREEZEFlagAttribute Then 'AG 14/03/2012 -Evaluate if process can continue only if analyzer is not freeze

                    'AG 16/03/2012 - Check if process can continue only when (ise intalled and isemanager initiated) or ise not installed 
                    '((Ise Instaled and (initiated or not swtiched on)) or not ise instaled)
                    If ISEAnalyzer IsNot Nothing AndAlso ISEAnalyzer.ConnectionTasksCanContinue Then

                        If Not CheckIfProcessCanContinue() Then
                            UpdateSensorValuesAttribute(AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS, 1, True)
                        Else
                            UpdateSensorValuesAttribute(AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS, 0, False)
                        End If



                        'AG 23/03/2012 - While ise is initiating save all the ansinf received (commented)
                    Else
                        'AG 24/05/2012 - If exists inform about bottle alarms 
                        'Case: Start instrument without reactions rotor and without bottles
                        '1) Sw ask for execute the change rotor utility
                        '2) When the washing station is risen the Sw starts initializating ISE but not inform about bottle alarms  if next code does not exists
                        If String.Equals(mySessionFlags(AnalyzerManagerFlags.NEWROTORprocess.ToString), "INPROCESS") Then
                            If ExistBottleAlarms() Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.NEWROTORprocess, "PAUSED")
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.NewRotor, "CANCELED")
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.BaseLine, "CANCELED")

                                UpdateSensorValuesAttribute(AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS, 1, True)
                            Else
                                UpdateSensorValuesAttribute(AnalyzerSensors.ABORTED_DUE_BOTTLE_ALARMS, 0, True)
                            End If
                        End If
                    End If

                    'AG 19/04/2012 - Abort (with out wash) is allowed while Partial Freeze
                ElseIf AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY AndAlso String.Equals(analyzerFREEZEModeAttribute, "PARTIAL") Then
                    If String.Equals(mySessionFlags(AnalyzerManagerFlags.ABORTprocess.ToString), "INPROCESS") Then
                        UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ABORTprocess, "CLOSED")
                        UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.Washing, "END")

                        'AG 04/09/2012 - When ABORT session finishes. Auto ABORT due to re-connection in running with incosistent data ... execute the full connection process
                        'Else execute previous code: Ask for status
                        'myGlobal = ManageAnalyzer(GlobalEnumerates.AnalyzerManagerSwActionList.STATE, True)
                        If ForceAbortSessionAttr Then
                            ProcessConnection(Nothing, True)
                        Else
                            myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.STATE, True)
                        End If
                    End If
                End If

                'Update analyzer session flags into DataBase
                If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                    If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.UserSwANSINFTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received and ANSINF instruction
        ''' Service Sw has his own threatment different form User SW
        ''' </summary>
        ''' <param name="pSensors "></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 15/04/2011
        ''' Modified by XBC 18/10/2012 - functionality to WATER_DEPOSIT and WASTE_DEPOSIT alarms using timers
        ''' </remarks>
        Private Function ServiceSwAnsInfTreatment(ByVal pSensors As Dictionary(Of AnalyzerSensors, Single)) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try
                Dim myCalc As New CalculationsDelegate()

                Dim isLogActivity As Boolean = False 'SGM 11/10/2011

                Dim alarmStatus As Boolean = False ' XBC 18/10/2012

                'Get General cover (parameter index 3)
                Dim myIntValue As Integer = 0
                If pSensors.ContainsKey(AnalyzerSensors.COVER_GENERAL) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(AnalyzerSensors.COVER_GENERAL, pSensors(AnalyzerSensors.COVER_GENERAL), True)
                End If


                'Get Photometrics (Reaccions) cover (parameter index 4)
                If pSensors.ContainsKey(AnalyzerSensors.COVER_REACTIONS) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(AnalyzerSensors.COVER_REACTIONS, pSensors(AnalyzerSensors.COVER_REACTIONS), True)
                End If


                'Get Reagents (Fridge) cover (parameter index 5)
                If pSensors.ContainsKey(AnalyzerSensors.COVER_FRIDGE) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(AnalyzerSensors.COVER_FRIDGE, pSensors(AnalyzerSensors.COVER_FRIDGE), True)
                End If


                'Get Samples cover (parameter index 6)
                If pSensors.ContainsKey(AnalyzerSensors.COVER_SAMPLES) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(AnalyzerSensors.COVER_SAMPLES, pSensors(AnalyzerSensors.COVER_SAMPLES), True)
                End If


                'Get System liquid sensor (parameter index 7)
                If pSensors.ContainsKey(AnalyzerSensors.WATER_DEPOSIT) Then
                    ' XBC 18/10/2012 - Sw do not generates the alarm WATER_DEPOSIT_ERR now, it activates a timer it is who generates the alarm if the time expires
                    ''Update the class attribute SensorValuesAttribute
                    'isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT, pSensors(GlobalEnumerates.AnalyzerSensors.WATER_DEPOSIT), True)

                    myIntValue = CInt(pSensors(AnalyzerSensors.WATER_DEPOSIT))

                    If myIntValue = 0 Or myIntValue = 2 Then 'Empty or impossible status
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.WATER_DEPOSIT, pSensors(AnalyzerSensors.WATER_DEPOSIT), False)

                    If alarmStatus AndAlso Not waterDepositTimer.Enabled Then
                        waterDepositTimer.Enabled = True 'Activate timer (but NOT ACTIVATE ALARM!!!)
                    ElseIf Not alarmStatus AndAlso waterDepositTimer.Enabled Then
                        waterDepositTimer.Enabled = False  'Deactivate timer
                    End If
                    ' XBC 18/10/2012

                End If


                'Get Waste sensor (parameter index 8)
                If pSensors.ContainsKey(AnalyzerSensors.WASTE_DEPOSIT) Then
                    ' XBC 18/10/2012 - Sw do not generates the alarm WASTE_DEPOSIT_ERR now, it activates a timer it is who generates the alarm if the time expires
                    ''Update the class attribute SensorValuesAttribute
                    'isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT, pSensors(GlobalEnumerates.AnalyzerSensors.WASTE_DEPOSIT), True)

                    myIntValue = CInt(pSensors(AnalyzerSensors.WASTE_DEPOSIT))

                    If myIntValue = 3 Or myIntValue = 2 Then 'Full or impossible status
                        alarmStatus = True
                    Else 'Closed
                        alarmStatus = False
                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.WASTE_DEPOSIT, pSensors(AnalyzerSensors.WASTE_DEPOSIT), False)

                    If alarmStatus AndAlso Not wasteDepositTimer.Enabled Then
                        wasteDepositTimer.Enabled = True 'Activate timer (but NOT ACTIVATE ALARM!!!)
                    ElseIf Not alarmStatus AndAlso wasteDepositTimer.Enabled Then
                        wasteDepositTimer.Enabled = False  'Deactivate timer
                    End If
                    ' XBC 18/10/2012

                End If

                'take adjustment values for Scales and Temperature managing
                Dim myAdjustmentsDS As New SRVAdjustmentsDS
                Dim myAdjustmentDelegate As FwAdjustmentsDelegate



                myGlobal = MyClass.ReadFwAdjustmentsDS()
                If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                    myAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)

                    'instantiate the delegate
                    myAdjustmentDelegate = New FwAdjustmentsDelegate(myAdjustmentsDS)
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If


                'SGM 11/10/2011 log only in case of variation >=5%

                'Get wash solution sensor (parameter index 9)
                If pSensors.ContainsKey(AnalyzerSensors.BOTTLE_WASHSOLUTION) Then

                    Dim myEmptyLevel As Single
                    Dim myFullLevel As Single
                    Dim myLastLevel As Single
                    Dim myNewLevel As Single
                    Dim myLastPercent As Single
                    Dim myNewPercent As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.WASHING_SOLUTION.ToString, _
                                                                                      AXIS.EMPTY.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myEmptyLevel = FormatToSingle(myValue)
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.WASHING_SOLUTION.ToString, _
                                                                                      AXIS.FULL.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myFullLevel = FormatToSingle(myValue)
                    End If

                    If myGlobal.SetDatos Is Nothing OrElse myGlobal.HasError Then
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    If SensorValuesAttribute.ContainsKey(AnalyzerSensors.BOTTLE_WASHSOLUTION) Then

                        myLastLevel = SensorValuesAttribute(AnalyzerSensors.BOTTLE_WASHSOLUTION)
                        myNewLevel = pSensors(AnalyzerSensors.BOTTLE_WASHSOLUTION)

                        myGlobal = CalculatePercent(myLastLevel, myEmptyLevel, myFullLevel)
                        If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                            myLastPercent = CType(myGlobal.SetDatos, Single)
                        End If

                        myGlobal = CalculatePercent(myNewLevel, myEmptyLevel, myFullLevel)
                        If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                            myNewPercent = CType(myGlobal.SetDatos, Single)
                        End If

                        If myGlobal.SetDatos Is Nothing OrElse myGlobal.HasError Then
                            myGlobal.HasError = True
                            Exit Try
                        End If

                        If Math.Abs(myNewPercent - myLastPercent) >= 5 Then
                            isLogActivity = True
                        Else
                            isLogActivity = False
                        End If

                        'Else

                        '    isLogActivity = True

                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.BOTTLE_WASHSOLUTION, pSensors(AnalyzerSensors.BOTTLE_WASHSOLUTION), True)



                End If

                'Get high contamination sensor (parameter index 10)
                If pSensors.ContainsKey(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE) Then
                    Dim myEmptyLevel As Single
                    Dim myFullLevel As Single
                    Dim myLastLevel As Single
                    Dim myNewLevel As Single
                    Dim myLastPercent As Single
                    Dim myNewPercent As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION.ToString, _
                                                                                      AXIS.EMPTY.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myEmptyLevel = FormatToSingle(myValue)
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.HIGH_CONTAMINATION.ToString, _
                                                                                      AXIS.FULL.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myFullLevel = FormatToSingle(myValue)
                    End If

                    If myGlobal.SetDatos Is Nothing OrElse myGlobal.HasError Then
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    If SensorValuesAttribute.ContainsKey(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE) Then

                        myLastLevel = SensorValuesAttribute(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE)
                        myNewLevel = pSensors(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE)

                        myGlobal = CalculatePercent(myLastLevel, myEmptyLevel, myFullLevel)
                        If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                            myLastPercent = CType(myGlobal.SetDatos, Single)
                        End If

                        myGlobal = CalculatePercent(myNewLevel, myEmptyLevel, myFullLevel)
                        If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                            myNewPercent = CType(myGlobal.SetDatos, Single)
                        End If

                        If myGlobal.SetDatos Is Nothing OrElse myGlobal.HasError Then
                            myGlobal.HasError = True
                            Exit Try
                        End If

                        If Math.Abs(myNewPercent - myLastPercent) >= 5 Then
                            isLogActivity = True
                        Else
                            isLogActivity = False
                        End If

                        'Else

                        '    isLogActivity = True

                    End If

                    'Update the class attribute SensorValuesAttribute
                    UpdateSensorValuesAttribute(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE, pSensors(AnalyzerSensors.BOTTLE_HIGHCONTAMINATION_WASTE), True)


                End If

                'END SGM 11/10/2011


                ' XBC 22/06/2011 - Thermos temperatures must be add a correction to monitorize them
                'obtain Master Data Adjustments DS


                'Get fridge temperature sensor (parameter index 11)
                If pSensors.ContainsKey(AnalyzerSensors.TEMPERATURE_FRIDGE) Then

                    Dim mySetpointTemp As Single
                    Dim myTargetTemp As Single
                    Dim myMeasuredTemp As Single
                    Dim sensorValueToMonitor As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.THERMOS_FRIDGE.ToString, _
                                                                                      AXIS.SETPOINT.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        mySetpointTemp = FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.THERMOS_FRIDGE.ToString, _
                                                                                      AXIS.TARGET.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myTargetTemp = FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myMeasuredTemp = pSensors(AnalyzerSensors.TEMPERATURE_FRIDGE)

                    myGlobal = myCalc.CalculateTemperatureToMonitor(mySetpointTemp, myTargetTemp, myMeasuredTemp)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'sensorValueToMonitor = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        sensorValueToMonitor = FormatToSingle(myValue)

                        'Update the class attribute SensorValuesAttribute
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_FRIDGE), True)
                        UpdateSensorValuesAttribute(AnalyzerSensors.TEMPERATURE_FRIDGE, sensorValueToMonitor, True)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                End If

                'Get Reactions Rotor temperature sensor (parameter index 13)
                If pSensors.ContainsKey(AnalyzerSensors.TEMPERATURE_REACTIONS) Then

                    Dim mySetpointTemp As Single
                    Dim myTargetTemp As Single
                    Dim myMeasuredTemp As Single
                    Dim sensorValueToMonitor As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY.ToString, _
                                                                                      AXIS.SETPOINT.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        mySetpointTemp = FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.THERMOS_PHOTOMETRY.ToString, _
                                                                                      AXIS.TARGET.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myTargetTemp = FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myMeasuredTemp = pSensors(AnalyzerSensors.TEMPERATURE_REACTIONS)

                    myGlobal = myCalc.CalculateTemperatureToMonitor(mySetpointTemp, myTargetTemp, myMeasuredTemp)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'sensorValueToMonitor = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        sensorValueToMonitor = FormatToSingle(myValue)

                        'Update the class attribute SensorValuesAttribute
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_REACTIONS), True)
                        UpdateSensorValuesAttribute(AnalyzerSensors.TEMPERATURE_REACTIONS, sensorValueToMonitor, True)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If
                End If

                'Get WS temperature sensor (parameter index 14)
                If pSensors.ContainsKey(AnalyzerSensors.TEMPERATURE_WASHINGSTATION) Then

                    Dim mySetpointTemp As Single
                    Dim myTargetTemp As Single
                    Dim myMeasuredTemp As Single
                    Dim sensorValueToMonitor As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.THERMOS_WS_HEATER.ToString, _
                                                                                      AXIS.SETPOINT.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        mySetpointTemp = FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.THERMOS_WS_HEATER.ToString, _
                                                                                      AXIS.TARGET.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myTargetTemp = FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myMeasuredTemp = pSensors(AnalyzerSensors.TEMPERATURE_WASHINGSTATION)

                    myGlobal = myCalc.CalculateTemperatureToMonitor(mySetpointTemp, myTargetTemp, myMeasuredTemp)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'sensorValueToMonitor = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        sensorValueToMonitor = FormatToSingle(myValue)

                        'Update the class attribute SensorValuesAttribute
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_WASHINGSTATION), True)
                        UpdateSensorValuesAttribute(AnalyzerSensors.TEMPERATURE_WASHINGSTATION, sensorValueToMonitor, True)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If
                End If

                'Get R1 temperature sensor (parameter index 15)
                If pSensors.ContainsKey(AnalyzerSensors.TEMPERATURE_R1) Then

                    Dim mySetpointTemp As Single
                    Dim myTargetTemp As Single
                    Dim myMeasuredTemp As Single
                    Dim sensorValueToMonitor As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.THERMOS_REAGENT1.ToString, _
                                                                                      AXIS.SETPOINT.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        mySetpointTemp = FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.THERMOS_REAGENT1.ToString, _
                                                                                      AXIS.TARGET.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myTargetTemp = FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myMeasuredTemp = pSensors(AnalyzerSensors.TEMPERATURE_R1)

                    myGlobal = myCalc.CalculateTemperatureToMonitor(mySetpointTemp, myTargetTemp, myMeasuredTemp)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'sensorValueToMonitor = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        sensorValueToMonitor = FormatToSingle(myValue)

                        'Update the class attribute SensorValuesAttribute
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R1), True)
                        UpdateSensorValuesAttribute(AnalyzerSensors.TEMPERATURE_R1, sensorValueToMonitor, True)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If
                End If

                'Get R2 temperature sensor (parameter index 16)
                If pSensors.ContainsKey(AnalyzerSensors.TEMPERATURE_R2) Then

                    Dim mySetpointTemp As Single
                    Dim myTargetTemp As Single
                    Dim myMeasuredTemp As Single
                    Dim sensorValueToMonitor As Single
                    Dim myValue As String

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.THERMOS_REAGENT2.ToString, _
                                                                                      AXIS.SETPOINT.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'mySetpointTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        mySetpointTemp = FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myGlobal = myAdjustmentDelegate.ReadAdjustmentValueByGroupAndAxis(ADJUSTMENT_GROUPS.THERMOS_REAGENT2.ToString, _
                                                                                      AXIS.TARGET.ToString)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'myTargetTemp = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        myTargetTemp = FormatToSingle(myValue)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If

                    myMeasuredTemp = pSensors(AnalyzerSensors.TEMPERATURE_R2)

                    myGlobal = myCalc.CalculateTemperatureToMonitor(mySetpointTemp, myTargetTemp, myMeasuredTemp)
                    If myGlobal.SetDatos IsNot Nothing AndAlso Not myGlobal.HasError Then
                        'sensorValueToMonitor = CType(myGlobal.SetDatos, Single)
                        myValue = CType(myGlobal.SetDatos, String)
                        sensorValueToMonitor = FormatToSingle(myValue)

                        'Update the class attribute SensorValuesAttribute
                        'UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2, pSensors(GlobalEnumerates.AnalyzerSensors.TEMPERATURE_R2), True)
                        UpdateSensorValuesAttribute(AnalyzerSensors.TEMPERATURE_R2, sensorValueToMonitor, True)
                    Else
                        myGlobal.HasError = True
                        Exit Try
                    End If
                End If


                'Get Fridge status (parameter index 12)
                If pSensors.ContainsKey(AnalyzerSensors.FRIDGE_STATUS) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(AnalyzerSensors.FRIDGE_STATUS, pSensors(AnalyzerSensors.FRIDGE_STATUS), False)
                End If


                'Get ISE status (parameter index 17)
                If pSensors.ContainsKey(AnalyzerSensors.ISE_STATUS) Then
                    'Update the class attribute SensorValuesAttribute
                    isLogActivity = isLogActivity Or UpdateSensorValuesAttribute(AnalyzerSensors.ISE_STATUS, pSensors(AnalyzerSensors.ISE_STATUS), False)
                End If

                ' XBC 17/05/2011 - delete redundant Events replaced using AnalyzerManager.ReceptionEvent
                'RaiseEvent SensorValuesChangedEvent(myUI_RefreshDS.SensorValueChanged)


                If isLogActivity Then
                    GlobalBase.CreateLogActivity("Received Instruction [" & AppLayer.InstructionReceived & "]", "ApplicationLayer.ActivateProtocol (case RECEIVE)", EventLogEntryType.Information, False)
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwAnsInfTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received FW data
        ''' Service Sw has his own threatment different form User SW
        ''' </summary>
        ''' <param name="pElements "></param>
        ''' <returns></returns>
        ''' <remarks>XBC 02/06/2011 - creation
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function ServiceFWInfoTreatment(ByVal pElements As Dictionary(Of FW_INFO, String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                'If Not MyClass.IsDisplayingServiceData Then Exit Try 'only while not displaying data

                'Generate UI_Refresh Firmware values dataset
                Dim myNewFirmwareValuesRow As UIRefreshDS.FirmwareValueChangedRow
                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                SyncLock myUI_RefreshDS.FirmwareValueChanged
                    myNewFirmwareValuesRow = myUI_RefreshDS.FirmwareValueChanged.NewFirmwareValueChangedRow
                    With myNewFirmwareValuesRow
                        .BeginEdit()
                        .ElementID = pElements(FW_INFO.ID)
                        .BoardSerialNumber = pElements(FW_INFO.SMC)

                        .RepositoryVersion = pElements(FW_INFO.RV)
                        .RepositoryCRCResult = pElements(FW_INFO.CRC)
                        .RepositoryCRCValue = pElements(FW_INFO.CRCV)
                        .RepositoryCRCSize = pElements(FW_INFO.CRCS)

                        .BoardFirmwareVersion = pElements(FW_INFO.FWV)
                        .BoardFirmwareCRCResult = pElements(FW_INFO.FWCRC)
                        .BoardFirmwareCRCValue = pElements(FW_INFO.FWCRCV)
                        .BoardFirmwareCRCSize = pElements(FW_INFO.FWCRCS)

                        .BoardHardwareVersion = pElements(FW_INFO.HWV)

                        If .ElementID = POLL_IDs.CPU.ToString Then
                            .AnalyzerSerialNumber = pElements(FW_INFO.ASN)
                        End If
                        .EndEdit()
                    End With
                    myUI_RefreshDS.FirmwareValueChanged.AddFirmwareValueChangedRow(myNewFirmwareValuesRow)
                    myUI_RefreshDS.FirmwareValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                End SyncLock
                'myUI_RefreshDS.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceFWInfoTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received ARMS data
        ''' Service Sw has his own threatment different form User SW
        ''' </summary>
        ''' <param name="pElements "></param>
        ''' <returns></returns>
        ''' <remarks>XBC 31/05/2011 - creation
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function ServiceSwArmsInfoTreatment(ByVal pElements As Dictionary(Of ARMS_ELEMENTS, String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                'If Not MyClass.IsDisplayingServiceData Then Exit Try 'only while not displaying data

                'Generate UI_Refresh Arms values dataset
                Dim myNewArmsValuesRow As UIRefreshDS.ArmValueChangedRow
                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                SyncLock myUI_RefreshDS.ArmValueChanged
                    myNewArmsValuesRow = myUI_RefreshDS.ArmValueChanged.NewArmValueChangedRow
                    With myNewArmsValuesRow
                        .BeginEdit()
                        .ArmID = pElements(ARMS_ELEMENTS.ID)
                        .BoardTemp = CType(pElements(ARMS_ELEMENTS.TMP), Single)
                        .MotorHorizontal = pElements(ARMS_ELEMENTS.MH)
                        .MotorHorizontalHome = pElements(ARMS_ELEMENTS.MHH)
                        .MotorHorizontalPosition = CType(pElements(ARMS_ELEMENTS.MHA), Single)
                        .MotorVertical = pElements(ARMS_ELEMENTS.MV)
                        .MotorVerticalHome = pElements(ARMS_ELEMENTS.MVH)
                        .MotorVerticalPosition = CType(pElements(ARMS_ELEMENTS.MVA), Single)
                        .EndEdit()
                    End With
                    myUI_RefreshDS.ArmValueChanged.AddArmValueChangedRow(myNewArmsValuesRow)

                    'myUI_RefreshDS.AcceptChanges()
                    myUI_RefreshDS.ArmValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 - AcceptChanges in datatable layer instead of dataset layer
                End SyncLock

                'Generate UI_Refresh event ARMSVALUE_CHANGED
                If Not myUI_RefreshEvent.Contains(UI_RefreshEvents.ARMSVALUE_CHANGED) Then myUI_RefreshEvent.Add(UI_RefreshEvents.ARMSVALUE_CHANGED)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwArmsInfoTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received PROBES data
        ''' Service Sw has his own threatment different form User SW
        ''' </summary>
        ''' <param name="pElements "></param>
        ''' <returns></returns>
        ''' <remarks>XBC 01/06/2011 - creation 
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function ServiceSwProbesInfoTreatment(ByVal pElements As Dictionary(Of PROBES_ELEMENTS, String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Generate UI_Refresh Arms Probes dataset
                Dim myNewprobesValuesRow As UIRefreshDS.ProbeValueChangedRow

                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                SyncLock myUI_RefreshDS.ProbeValueChanged
                    myNewprobesValuesRow = myUI_RefreshDS.ProbeValueChanged.NewProbeValueChangedRow
                    With myNewprobesValuesRow
                        .BeginEdit()
                        .ProbeID = pElements(PROBES_ELEMENTS.ID)
                        .BoardTemp = CType(pElements(PROBES_ELEMENTS.TMP), Single)
                        .DetectionStatus = pElements(PROBES_ELEMENTS.DST)
                        .DetectionFrequency = CType(pElements(PROBES_ELEMENTS.DFQ), Single)
                        .Detection = pElements(PROBES_ELEMENTS.D)
                        .LastInternalRate = CType(pElements(PROBES_ELEMENTS.DCV), Single)
                        .ThermistorValue = CType(pElements(PROBES_ELEMENTS.PTH), Single)
                        .ThermistorDiagnostic = CType(pElements(PROBES_ELEMENTS.PTHD), Single)
                        .HeaterStatus = pElements(PROBES_ELEMENTS.PH)
                        .HeaterDiagnostic = CType(pElements(PROBES_ELEMENTS.PHD), Single)
                        .CollisionDetector = pElements(PROBES_ELEMENTS.CD)
                        .EndEdit()
                    End With
                    myUI_RefreshDS.ProbeValueChanged.AddProbeValueChangedRow(myNewprobesValuesRow)
                    myUI_RefreshDS.ProbeValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                End SyncLock

                'Generate UI_Refresh event PROBESSVALUE_CHANGED
                If Not myUI_RefreshEvent.Contains(UI_RefreshEvents.PROBESVALUE_CHANGED) Then myUI_RefreshEvent.Add(UI_RefreshEvents.PROBESVALUE_CHANGED)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwProbesInfoTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' SW has received ROTORS data
        ''' Service Sw has his own threatment different form User SW
        ''' </summary>
        ''' <param name="pElements "></param>
        ''' <returns></returns>
        ''' <remarks>XBC 01/06/2011 - creation
        ''' AG 22/05/2014 - #1637 Remove old commented code + use exclusive lock (multithread protection) + AcceptChanges in the datatable with changes, not in the whole dataset
        ''' </remarks>
        Private Function ServiceSwRotorsInfoTreatment(ByVal pElements As Dictionary(Of ROTORS_ELEMENTS, String)) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'Generate UI_Refresh Rotors values dataset
                Dim myNewRotorsValuesRow As UIRefreshDS.RotorValueChangedRow
                SyncLock myUI_RefreshDS.RotorValueChanged
                    myNewRotorsValuesRow = myUI_RefreshDS.RotorValueChanged.NewRotorValueChangedRow
                    With myNewRotorsValuesRow
                        .BeginEdit()
                        .RotorID = pElements(ROTORS_ELEMENTS.ID)
                        .BoardTemp = CType(pElements(ROTORS_ELEMENTS.TMP), Single)
                        .Motor = pElements(ROTORS_ELEMENTS.MR)
                        .MotorHome = pElements(ROTORS_ELEMENTS.MRH)
                        .MotorPosition = CType(pElements(ROTORS_ELEMENTS.MRA), Single)
                        .ThermistorFridgeValue = CType(pElements(ROTORS_ELEMENTS.FTH), Single)
                        .ThermistorFridgeDiagnostic = CType(pElements(ROTORS_ELEMENTS.FTHD), Single)
                        .PeltiersFridgeStatus = pElements(ROTORS_ELEMENTS.FH)
                        .PeltiersFridgeDiagnostic = CType(pElements(ROTORS_ELEMENTS.FHD), Single)
                        .PeltiersFan1Speed = CType(pElements(ROTORS_ELEMENTS.PF1), Single)
                        .PeltiersFan1Diagnostic = CType(pElements(ROTORS_ELEMENTS.PF1D), Single)
                        .PeltiersFan2Speed = CType(pElements(ROTORS_ELEMENTS.PF2), Single)
                        .PeltiersFan2Diagnostic = CType(pElements(ROTORS_ELEMENTS.PF2D), Single)
                        .PeltiersFan3Speed = CType(pElements(ROTORS_ELEMENTS.PF3), Single)
                        .PeltiersFan3Diagnostic = CType(pElements(ROTORS_ELEMENTS.PF3D), Single)
                        .PeltiersFan4Speed = CType(pElements(ROTORS_ELEMENTS.PF4), Single)
                        .PeltiersFan4Diagnostic = CType(pElements(ROTORS_ELEMENTS.PF4D), Single)
                        .FrameFan1Speed = CType(pElements(ROTORS_ELEMENTS.FF1), Single)
                        .FrameFan1Diagnostic = CType(pElements(ROTORS_ELEMENTS.FF1D), Single)
                        .FrameFan2Speed = CType(pElements(ROTORS_ELEMENTS.FF2), Single)
                        .FrameFan2Diagnostic = CType(pElements(ROTORS_ELEMENTS.FF2D), Single)
                        .Cover = pElements(ROTORS_ELEMENTS.RC)
                        .BarCodeStatus = CType(pElements(ROTORS_ELEMENTS.CB), Single)
                        .BarcodeError = pElements(ROTORS_ELEMENTS.CBE)
                        .EndEdit()
                    End With
                    myUI_RefreshDS.RotorValueChanged.AddRotorValueChangedRow(myNewRotorsValuesRow)
                    myUI_RefreshDS.RotorValueChanged.AcceptChanges() 'AG 22/05/2014 #1637 AcceptChanges in datatable layer instead of dataset layer
                End SyncLock

                'Generate UI_Refresh event ROTORSSVALUE_CHANGED
                If Not myUI_RefreshEvent.Contains(UI_RefreshEvents.ROTORSVALUE_CHANGED) Then myUI_RefreshEvent.Add(UI_RefreshEvents.ROTORSVALUE_CHANGED)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ServiceSwRotorsInfoTreatment", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' Update the internal attribte SensorValuesAttribute
        ''' </summary>
        ''' <param name="pSensor"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks>
        ''' Modified by: IT 19/12/2014 - BA-2143 (Accessibility Level)
        ''' </remarks>
        Public Function UpdateSensorValuesAttribute(ByVal pSensor As AnalyzerSensors, ByVal pNewValue As Single, ByVal pUIEventForChangesFlag As Boolean) As Boolean Implements IAnalyzerManager.UpdateSensorValuesAttribute
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            Try
                If GlobalBase.IsServiceAssembly Then
                    If InfoRefreshFirstTime Then
                        InfoRefreshFirstTime = False
                        If Not SensorValuesAttribute.ContainsKey(pSensor) Then
                            SensorValuesAttribute.Add(pSensor, pNewValue)

                        Else
                            SensorValuesAttribute(pSensor) = pNewValue
                        End If
                        flagChanges = True
                    Else
                        If Not SensorValuesAttribute.ContainsKey(pSensor) Then
                            SensorValuesAttribute.Add(pSensor, pNewValue)
                            flagChanges = True

                        ElseIf SensorValuesAttribute(pSensor) <> pNewValue Then
                            SensorValuesAttribute(pSensor) = pNewValue
                            flagChanges = True
                        End If
                    End If


                Else

                    If Not SensorValuesAttribute.ContainsKey(pSensor) Then
                        SensorValuesAttribute.Add(pSensor, pNewValue)
                        flagChanges = True

                    ElseIf SensorValuesAttribute(pSensor) <> pNewValue Then
                        SensorValuesAttribute(pSensor) = pNewValue
                        flagChanges = True
                    End If

                End If

                If pUIEventForChangesFlag And flagChanges Then
                    'Generate UI_Refresh event SENSORVALUE_CHANGED
                    PrepareUIRefreshEventNum2(Nothing, UI_RefreshEvents.SENSORVALUE_CHANGED, "", -1, "", "", -1, -1, "", "", pSensor, pNewValue, Nothing, -1, -1, "", "")

                    'AG 17/10/2011
                    'Special business for the CONNECTED false sensor ... insert into Alarms table (only for User Sw)
                    If pSensor = AnalyzerSensors.CONNECTED Then

                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        If Not GlobalBase.IsServiceAssembly Then
                            'Add into alarms table
                            Dim myAlarmList As New List(Of Alarms)
                            Dim myAlarmStatusList As New List(Of Boolean)
                            Const alarmId As Alarms = AlarmEnumerates.Alarms.COMMS_ERR
                            Dim alarmStatus = pNewValue <> 1
                            PrepareLocalAlarmList(alarmId, alarmStatus, myAlarmList, myAlarmStatusList)
                            If myAlarmList.Count > 0 Then
                                Dim myGlobal As New GlobalDataTO
                                Dim currentAlarms = New AnalyzerAlarms(Me)
                                myGlobal = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                            End If
                        End If

                    End If

                    Return True
                End If
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.UpdateSensorValuesAttribute", EventLogEntryType.Error, False)
            End Try

            Return False

        End Function

        ''' <summary>
        ''' Update the internal attribte CpuValuesAttribute
        ''' </summary>
        ''' <param name="pElement"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks></remarks>
        Private Sub UpdateCpuValuesAttribute(ByVal pElement As CPU_ELEMENTS, _
                                                  ByVal pNewValue As String, _
                                                  ByVal pUIEventForChangesFlag As Boolean)
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            If Not CpuValuesAttribute.ContainsKey(pElement) Then
                CpuValuesAttribute.Add(pElement, pNewValue)
                flagChanges = True

            ElseIf CpuValuesAttribute(pElement) <> pNewValue Then
                CpuValuesAttribute(pElement) = pNewValue
                flagChanges = True
            End If

            'If pUIEventForChangesFlag And flagChanges Then
            ' XBC 09/06/2011 - is interesting event will be invoked always that an ANSCPU instruction is received
            'Generate UI_Refresh event CPUVALUE_CHANGED
            PrepareUIRefreshEventCpu(Nothing, UI_RefreshEvents.CPUVALUE_CHANGED, pElement, pNewValue)
            'End If

        End Sub

        ''' <summary>
        ''' Update the internal attribte ManifoldValuesAttribute
        ''' </summary>
        ''' <param name="pElement"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks></remarks>
        Private Sub UpdateManifoldValuesAttribute(ByVal pElement As MANIFOLD_ELEMENTS, _
                                                  ByVal pNewValue As String, _
                                                  ByVal pUIEventForChangesFlag As Boolean)
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            If Not ManifoldValuesAttribute.ContainsKey(pElement) Then
                ManifoldValuesAttribute.Add(pElement, pNewValue)
                flagChanges = True

            ElseIf ManifoldValuesAttribute(pElement) <> pNewValue Then
                ManifoldValuesAttribute(pElement) = pNewValue
                flagChanges = True
            End If

            'Generate UI_Refresh event MANIFOLDVALUE_CHANGED
            PrepareUIRefreshEventManifold(Nothing, UI_RefreshEvents.MANIFOLDVALUE_CHANGED, pElement, pNewValue)

        End Sub

        ''' <summary>
        ''' Update the internal attribte FluidicsValuesAttribute
        ''' </summary>
        ''' <param name="pElement"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks></remarks>
        Private Sub UpdateFluidicsValuesAttribute(ByVal pElement As FLUIDICS_ELEMENTS, _
                                                  ByVal pNewValue As String, _
                                                  ByVal pUIEventForChangesFlag As Boolean)
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            If Not FluidicsValuesAttribute.ContainsKey(pElement) Then
                FluidicsValuesAttribute.Add(pElement, pNewValue)
                flagChanges = True

            ElseIf FluidicsValuesAttribute(pElement) <> pNewValue Then
                FluidicsValuesAttribute(pElement) = pNewValue
                flagChanges = True
            End If

            'Generate UI_Refresh event FLUIDICSVALUE_CHANGED
            PrepareUIRefreshEventFluidics(Nothing, UI_RefreshEvents.FLUIDICSVALUE_CHANGED, pElement, pNewValue)

        End Sub

        ''' <summary>
        ''' Update the internal attribte PhotometricsValuesAttribute
        ''' </summary>
        ''' <param name="pElement"></param>
        ''' <param name="pNewValue"></param>
        ''' <param name="pUIEventForChangesFlag"></param>
        ''' <remarks></remarks>
        Private Sub UpdatePhotometricsValuesAttribute(ByVal pElement As PHOTOMETRICS_ELEMENTS, _
                                                      ByVal pNewValue As String, _
                                                      ByVal pUIEventForChangesFlag As Boolean)
            'NOTE: Do not implement Try .. Catch due the methods who call it implements it

            Dim flagChanges As Boolean = False

            If Not PhotometricsValuesAttribute.ContainsKey(pElement) Then
                PhotometricsValuesAttribute.Add(pElement, pNewValue)
                flagChanges = True

            ElseIf PhotometricsValuesAttribute(pElement) <> pNewValue Then
                PhotometricsValuesAttribute(pElement) = pNewValue
                flagChanges = True
            End If

            'Generate UI_Refresh event FLUIDICSVALUE_CHANGED
            PrepareUIRefreshEventPhotometrics(Nothing, UI_RefreshEvents.PHOTOMETRICSVALUE_CHANGED, pElement, pNewValue)

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pCounts"></param>
        ''' <param name="pWashSolutionBottleFlag" ></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created SGM
        ''' Modified AG 27/04/2011 - use linq instead loop for searching the adjustment
        ''' </remarks>
        Public Function GetBottlePercentage(ByVal pCounts As Integer, ByVal pWashSolutionBottleFlag As Boolean) As GlobalDataTO Implements IAnalyzerManager.GetBottlePercentage
            Dim myGlobal As New GlobalDataTO

            Try
                'obtain adjustments
                Dim MinAdj As Double = -1
                Dim MaxAdj As Double = -1

                Dim minCode As String = ""
                Dim maxCode As String = ""

                If pWashSolutionBottleFlag Then 'Wash solution bottle adjustments
                    minCode = Ax00Adjustsments.GSMI.ToString
                    maxCode = Ax00Adjustsments.GSMA.ToString
                Else 'High contamination bottle adjustments
                    minCode = Ax00Adjustsments.GWMI.ToString
                    maxCode = Ax00Adjustsments.GWMA.ToString
                End If

                myGlobal = AppLayer.ReadFwAdjustmentsDS()
                If myGlobal.SetDatos IsNot Nothing Then
                    Dim myAdjDS As SRVAdjustmentsDS = CType(myGlobal.SetDatos, SRVAdjustmentsDS)
                    If myAdjDS IsNot Nothing Then
                        Dim myRes As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                        myRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                                 Where a.CodeFw = minCode Select a).ToList
                        If myRes.Count > 0 AndAlso IsNumeric(myRes(0).Value) Then
                            MinAdj = CDbl(myRes(0).Value)
                        End If

                        myRes = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjDS.srv_tfmwAdjustments _
                                Where a.CodeFw = maxCode Select a).ToList
                        If myRes.Count > 0 AndAlso IsNumeric(myRes(0).Value) Then
                            MaxAdj = CDbl(myRes(0).Value)
                        End If
                        myRes = Nothing 'AG 02/08/2012 - free memory

                    End If
                End If

                If MinAdj <> -1 AndAlso MaxAdj <> -1 AndAlso MinAdj <> MaxAdj Then
                    ''Dim myUtil As New Utilities.
                    myGlobal = CalculatePercent(pCounts, MinAdj, MaxAdj, True)
                Else
                    myGlobal.SetDatos = Nothing
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.GetBottlePercentage", EventLogEntryType.Error, False)
            End Try
            Return myGlobal

        End Function

        ''' <summary>
        ''' Error codes not detected yet are marked as solved
        ''' </summary>
        ''' <param name="pErrorCodeList"></param>
        ''' <remarks>Created by XBC 07/11/2012</remarks>
        Public Sub SolveErrorCodesToDisplay(ByVal pErrorCodeList As List(Of String)) Implements IAnalyzerManager.SolveErrorCodesToDisplay
            Try
                If Not myErrorCodesDisplayAttribute Is Nothing AndAlso _
                   myErrorCodesDisplayAttribute.Count > 0 AndAlso _
                   Not pErrorCodeList Is Nothing AndAlso _
                   pErrorCodeList.Count > 0 Then

                    ' Clone myErrorCodesDisplayAttribute into Aux Structure
                    Dim myErrorCodesDisplayAttributeAux As New List(Of ErrorCodesDisplayStruct)
                    For Each myErrorCodesDisplayRow As ErrorCodesDisplayStruct In myErrorCodesDisplayAttribute
                        Dim myErrorCodesDisplayAuxRow As New ErrorCodesDisplayStruct
                        myErrorCodesDisplayAuxRow.ErrorDateTime = myErrorCodesDisplayRow.ErrorDateTime
                        myErrorCodesDisplayAuxRow.ErrorCode = myErrorCodesDisplayRow.ErrorCode
                        myErrorCodesDisplayAuxRow.ResourceID = myErrorCodesDisplayRow.ResourceID
                        myErrorCodesDisplayAuxRow.Solved = myErrorCodesDisplayRow.Solved
                        myErrorCodesDisplayAttributeAux.Add(myErrorCodesDisplayAuxRow)
                    Next

                    ' Remove myErrorCodesDisplayAttributeAux
                    myErrorCodesDisplayAttribute.Clear()

                    ' Check error codes that has been solved (not exists on pErrorCodeList)
                    For Each myErrorCodesDisplayAuxRow As ErrorCodesDisplayStruct In myErrorCodesDisplayAttributeAux
                        If Not pErrorCodeList.Contains(myErrorCodesDisplayAuxRow.ErrorCode) Then
                            ' Add the error code again but with the value Solved as TRUE
                            Dim myErrorCodesDisplayRow As New ErrorCodesDisplayStruct
                            myErrorCodesDisplayRow.ErrorDateTime = myErrorCodesDisplayAuxRow.ErrorDateTime
                            myErrorCodesDisplayRow.ErrorCode = myErrorCodesDisplayAuxRow.ErrorCode
                            myErrorCodesDisplayRow.ResourceID = myErrorCodesDisplayAuxRow.ResourceID
                            If myErrorCodesDisplayAuxRow.ErrorCode <> "-1" Then
                                myErrorCodesDisplayRow.Solved = True
                            Else
                                myErrorCodesDisplayRow.Solved = False
                            End If
                            myErrorCodesDisplayAttribute.Add(myErrorCodesDisplayRow)
                        ElseIf pErrorCodeList.Contains(myErrorCodesDisplayAuxRow.ErrorCode) AndAlso _
                               myErrorCodesDisplayAuxRow.Solved Then
                            If myErrorCodesDisplayAuxRow.ErrorCode <> "-1" Then
                            End If
                            ' There are the same previous error but was solved, in this case, the historic log keep this information
                            Dim myErrorCodesDisplayRow As New ErrorCodesDisplayStruct
                            myErrorCodesDisplayRow.ErrorDateTime = myErrorCodesDisplayAuxRow.ErrorDateTime
                            myErrorCodesDisplayRow.ErrorCode = myErrorCodesDisplayAuxRow.ErrorCode
                            myErrorCodesDisplayRow.ResourceID = myErrorCodesDisplayAuxRow.ResourceID
                            If myErrorCodesDisplayAuxRow.ErrorCode <> "-1" Then
                                myErrorCodesDisplayRow.Solved = True
                            Else
                                myErrorCodesDisplayRow.Solved = False
                            End If
                            myErrorCodesDisplayAttribute.Add(myErrorCodesDisplayRow)
                        End If
                    Next

                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.SolveErrorCodesToDisplay", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Add new item to Error Codes to display List
        ''' </summary>
        ''' <param name="pErrorCodesDS"></param>
        ''' <remarks>Created by XBC 07/11/2012</remarks>
        Public Sub AddErrorCodesToDisplay(ByVal pErrorCodesDS As AlarmErrorCodesDS) Implements IAnalyzerManager.AddErrorCodesToDisplay
            Try
                If Not myErrorCodesDisplayAttribute Is Nothing AndAlso _
                   Not pErrorCodesDS Is Nothing AndAlso _
                   pErrorCodesDS.tfmwAlarmErrorCodes.Count > 0 Then

                    ' Check is Error Code exists on structure
                    Dim AddItPlease As Boolean = True
                    For Each myErrorCodesDisplayRow As ErrorCodesDisplayStruct In myErrorCodesDisplayAttribute
                        If myErrorCodesDisplayRow.ErrorCode = pErrorCodesDS.tfmwAlarmErrorCodes(0).ErrorCode.ToString Then
                            If myErrorCodesDisplayRow.Solved Then
                                ' Exist but it was previously solved and appears the same error again
                                AddItPlease = True
                                Exit For
                            Else
                                AddItPlease = False
                            End If
                        End If
                    Next
                    If AddItPlease Then
                        ' Add new error codes received (avoiding repetitions of NOT solved errors)
                        Dim newErrorCodesDisplayRow As New ErrorCodesDisplayStruct
                        newErrorCodesDisplayRow.ErrorDateTime = DateTime.Now
                        newErrorCodesDisplayRow.ErrorCode = pErrorCodesDS.tfmwAlarmErrorCodes(0).ErrorCode.ToString
                        newErrorCodesDisplayRow.ResourceID = pErrorCodesDS.tfmwAlarmErrorCodes(0).ResourceID
                        newErrorCodesDisplayRow.Solved = False
                        myErrorCodesDisplayAttribute.Add(newErrorCodesDisplayRow)
                    End If

                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.AddErrorCodesToDisplay", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' Clear Error Codes to display list
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 07/11/2012</remarks>
        Public Function RemoveErrorCodesToDisplay() As GlobalDataTO Implements IAnalyzerManager.RemoveErrorCodesToDisplay
            Dim myGlobal As New GlobalDataTO
            Try
                myErrorCodesDisplayAttribute.Clear()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.RemoveErrorCodesToDisplay", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#End Region

#Region "Aborted processes due alarms"

        ''' <summary>
        ''' Some processes can be aborted due analyzer alarms:
        ''' - During Wup process do not sent WASH or ALIGHT if bottle/deposit alarms exists
        ''' - During Sdowh process do not sent WASH or ALIGHT if bottle/deposit alarms exists
        ''' - During ChangeRotor / Alight util process do not sent ALIGHT if bottle/deposit alarms exists
        ''' - During Abort do not sent WASH if bottle/deposit alarms exists
        ''' 
        ''' </summary>
        ''' <remarks>AG 29/09/2011
        ''' Modified by: IT 01/12/2014 - BA-2075
        ''' </remarks>
        Private Function CheckIfProcessCanContinue() As Boolean
            Dim readyToSentInstruction As Boolean = True ' True next instruction can be sent / False can not be sent due a bottle alarms
            Try
                Dim bottleErrAlarm As Boolean = False
                Dim reactRotorMissingAlarm As Boolean = myAlarmListAttribute.Contains(AlarmEnumerates.Alarms.REACT_MISSING_ERR) 'AG 12/03/2012
                Dim myGlobal As New GlobalDataTO
                Dim myAnalyzerFlagsDS As New AnalyzerManagerFlagsDS

                'AG 20/02/2012 - The following condition is added into a public method
                If ExistBottleAlarms() Then
                    bottleErrAlarm = True
                End If

                'AG 22/02/2012 - Change bottle confirmation process
                If mySessionFlags(AnalyzerManagerFlags.CHANGE_BOTTLES_Process.ToString) = "INPROCESS" Then
                    Dim askForStateFlag As Boolean = True
                    UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.CHANGE_BOTTLES_Process, "CLOSED")

                    If bottleErrAlarm OrElse reactRotorMissingAlarm Then
                        readyToSentInstruction = False

                    Else
                        'If change bottles confirmation success and there was an ABORT ws wash pending ... send it automatically
                        If mySessionFlags(AnalyzerManagerFlags.ABORTprocess.ToString) = "PAUSED" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ABORTprocess, "INPROCESS")
                            UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.Washing, "")
                            askForStateFlag = False 'In this case is not necessary to ask for status, we sent the Wash instruction

                            'If change bottles confirmation success and there was an RECOVER ws wash pending ... send it automatically
                        ElseIf mySessionFlags(AnalyzerManagerFlags.RECOVERprocess.ToString) = "PAUSED" Then
                            UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.RECOVERprocess, "INPROCESS")
                            UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.Washing, "")
                            askForStateFlag = False 'In this case is not necessary to ask for status, we sent the Wash instruction

                        Else 'for other processes activate action button but not send next instruction automatically

                        End If
                    End If

                    'Finally Sw also ask for status
                    'Send a STATE instruction
                    If askForStateFlag Then
                        myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.STATE, True)
                    End If

                End If

                If readyToSentInstruction Then

                    If bottleErrAlarm OrElse reactRotorMissingAlarm Then
                        readyToSentInstruction = False
                    End If

                    'Some action button process required Wash instruction is bottle level OK before Sw sends it
                    If mySessionFlags(AnalyzerManagerFlags.Washing.ToString) = "" Then

                        'AG 20/02/2012 - abort process
                        'ElseIf mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.ABORTprocess.ToString) = "INPROCESS" Then
                        If mySessionFlags(AnalyzerManagerFlags.ABORTprocess.ToString) = "INPROCESS" Then
                            If AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                                If bottleErrAlarm OrElse reactRotorMissingAlarm Then
                                    readyToSentInstruction = False
                                    UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ABORTprocess, "PAUSED")
                                    UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.Washing, "CANCELED")

                                    'Else 'AG 28/02/2012 - when ABORT send the WASH instrucion
                                Else
                                    UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.ABORTprocess, "INPROCESS") 'AG 05/03/2012
                                    UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.Washing, "INI")

                                    'Send a WASH instruction (Conditioning complete)
                                    myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.WASH, True)
                                End If
                            End If
                            'AG 20/02/2012

                            'AG 08/03/2012 - recover process
                        ElseIf mySessionFlags(AnalyzerManagerFlags.RECOVERprocess.ToString) = "INPROCESS" Then

                            If bottleErrAlarm OrElse reactRotorMissingAlarm Then
                                readyToSentInstruction = False
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.RECOVERprocess, "PAUSED")
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.Washing, "CANCELED")

                                'Else 'AG 28/02/2012 - when RECOVER finishes send the WASH instrucion only in standby
                            ElseIf AnalyzerStatusAttribute = AnalyzerManagerStatus.STANDBY Then
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.RECOVERprocess, "INPROCESS") 'AG 05/03/2012
                                UpdateSessionFlags(myAnalyzerFlagsDS, AnalyzerManagerFlags.Washing, "INI")

                                'Send a WASH instruction (Conditioning complete)
                                myGlobal = ManageAnalyzer(AnalyzerManagerSwActionList.WASH, True)
                            End If

                        End If
                    End If
                End If

                'Update analyzer session flags into DataBase
                If Not myGlobal.HasError AndAlso ConnectedAttribute Then
                    If myAnalyzerFlagsDS.tcfgAnalyzerManagerFlags.Rows.Count > 0 Then
                        Dim myFlagsDelg As New AnalyzerManagerFlagsDelegate
                        myGlobal = myFlagsDelg.Update(Nothing, myAnalyzerFlagsDS)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.CheckIfProcessCanContinue", EventLogEntryType.Error, False)
            End Try
            Return readyToSentInstruction
        End Function

#End Region

#Region "Freeze alarms methods"

        ''' <summary>
        ''' Transalte the Fw error code to an internal Sw alarm ID enumerate
        ''' Depending the error code the internal flag AnalyzerIsFREEZEMode is activated
        ''' </summary>
        ''' <param name="pDbConnection"></param>
        ''' <param name="pErrorCodeList"></param>
        ''' <remarks>
        ''' AG 25/03/2011 - creation ... but not implemented due the error codes are not still defined
        ''' AG 02/03/2012 - redesigned with final Fw information
        ''' AG 02/02/2015 BA-2222 Protection against:
        '''                       - Not valid error codes (error codes with not alarm related)
        '''                       - Error codes related with multiple alarm (1 error code generates - N alarms, with N > 1. For example 302)
        ''' 
        '''               When method finishes, the list of alarms returned must be equivalent to the pErrorCodeList
        '''               So: define byRef the parameter pErrorCodeList (error codes that not generate alarm will be removed from list), error codes that generates N alarms will be added N-1 times to list           
        ''' </remarks>
        Public Function TranslateErrorCodeToAlarmID(ByVal pDbConnection As SqlConnection, ByRef pErrorCodeList As List(Of Integer)) As List(Of Alarms) _
            Implements IAnalyzerManager.TranslateErrorCodeToAlarmID

            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing
            Dim toReturnAlarmList As New List(Of Alarms)

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim stringAlarmList As New List(Of String)
                        Dim myLinq As List(Of AlarmsDS.tfmwAlarmsRow)
                        Dim fwStartsFreezeFlag As Boolean = False

                        'use a local list in loop because the list in parameter can be modified
                        Dim alarmItemCounter As Integer = 0 'Count the alarms generated by 1 error code!!
                        Dim myLocalErrorCodeList As New List(Of Integer)
                        myLocalErrorCodeList.AddRange(pErrorCodeList)

                        'For each ErrorCode in pErrorCodeList search the related AlarmID and evaluate if Freeze
                        For Each row As Integer In myLocalErrorCodeList
                            myLinq = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms _
                                      Where Not a.IsErrorCodeNull AndAlso a.ErrorCode = row Select a).ToList

                            If myLinq.Count > 0 Then
                                alarmItemCounter = 0
                                For Each item As AlarmsDS.tfmwAlarmsRow In myLinq
                                    'Get the related AlarmID
                                    'This line causes system error, vb .net can not convert a String (item.AlarmID) into a internal enumerate (AlarmEnumerates.Alarms)
                                    'Solution: Create a list of alarm string and the call a method who implements the select case to asign to proper AlarmEnumerates.Alarms ID

                                    If Not stringAlarmList.Contains(item.AlarmID) Then
                                        stringAlarmList.Add(item.AlarmID)

                                        'AG 02/02/2015 BA-2222 - When 1 error code generates several alarms we must add new item in error code list
                                        alarmItemCounter += 1
                                        If alarmItemCounter > 1 Then
                                            pErrorCodeList.Insert(pErrorCodeList.IndexOf(row), row)
                                        End If

                                        ' XBC 16/10/2012 - Alarms treatment for Service
                                        If Not GlobalBase.IsServiceAssembly Then
                                            'Evaluate if Freeze and FreezeMode
                                            If Not item.IsFreezeNull Then
                                                If Not analyzerFREEZEFlagAttribute Then
                                                    analyzerFREEZEFlagAttribute = True
                                                    analyzerFREEZEModeAttribute = item.Freeze '"Auto" or "Partial" or "Total" or "Reset"
                                                    fwStartsFreezeFlag = True 'AG 28/03/2012

                                                    'AG 07/03/2012 - RPM spec the cover errors are treated as "Total" freeze in running
                                                    '                in other status are a special freeze mode autosolved "auto"
                                                    If item.Freeze = "AUTO" And AnalyzerStatusAttribute = AnalyzerManagerStatus.RUNNING Then
                                                        analyzerFREEZEModeAttribute = "TOTAL"
                                                    End If
                                                Else
                                                    'Inform when user try to connect after freeze on connection
                                                    If SensorValuesAttribute(AnalyzerSensors.FREEZE) <> 1 Then
                                                        UpdateSensorValuesAttribute(AnalyzerSensors.FREEZE, 1, True) 'AG 06/03/2012 - Inform presentation analyzer is FREEZE
                                                    End If
                                                End If

                                                'Freeze priority: lower "AUTO", middle "PARTIAL", high "TOTAL", bestial "RESET"
                                                If item.Freeze = "PARTIAL" AndAlso analyzerFREEZEModeAttribute = "AUTO" Then
                                                    analyzerFREEZEModeAttribute = item.Freeze 'new freeze mode is Partial

                                                ElseIf item.Freeze = "TOTAL" AndAlso (analyzerFREEZEModeAttribute <> "TOTAL" AndAlso analyzerFREEZEModeAttribute <> "RESET") Then
                                                    analyzerFREEZEModeAttribute = item.Freeze 'new freeze mode is Total

                                                ElseIf item.Freeze = "RESET" AndAlso (analyzerFREEZEModeAttribute <> "RESET") Then
                                                    analyzerFREEZEModeAttribute = item.Freeze 'new freeze mode is Reset

                                                End If

                                            End If
                                        End If
                                    End If
                                Next
                            Else
                                pErrorCodeList.Remove(row)
                            End If
                        Next
                        'Translate the stringAlarmList into toReturnAlarmList
                        If stringAlarmList.Count > 0 Then
                            toReturnAlarmList = ConvertToAlarmIDEnumerate(stringAlarmList)

                            ' XBC 16/10/2012 - Alarms treatment for Service
                            If Not GlobalBase.IsServiceAssembly Then
                                'AG 28/03/2012 - The only AUTO freeze code error in standby is INSTRUCTION REJECTED ERROR (not covers)
                                'FREEZE is when Fw informs instruction rejected error because some enabled cover is opened
                                'not when informs only enabled cover opened
                                If fwStartsFreezeFlag AndAlso analyzerFREEZEModeAttribute = "AUTO" Then
                                    'Remove false Freeze
                                    If Not toReturnAlarmList.Contains(AlarmEnumerates.Alarms.INST_REJECTED_ERR) Then
                                        analyzerFREEZEFlagAttribute = False
                                        analyzerFREEZEModeAttribute = ""
                                    End If
                                End If

                                'Finally inform presentation layer the analyzer is FREEZE
                                If fwStartsFreezeFlag AndAlso analyzerFREEZEFlagAttribute Then
                                    UpdateSensorValuesAttribute(AnalyzerSensors.FREEZE, 1, True)
                                End If
                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                toReturnAlarmList.Clear() 'Clear the alarm list to return

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.TranslateErrorCodeToAlarmID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (toReturnAlarmList)
        End Function

        Public Function SimpleTranslateErrorCodeToAlarmId(pDbConnection As SqlConnection, errorCode As Integer) As Alarms Implements IAnalyzerManager.SimpleTranslateErrorCodeToAlarmId
            Try
                If alarmsDefintionTableDS Is Nothing OrElse alarmsDefintionTableDS.tfmwAlarms Is Nothing OrElse Not alarmsDefintionTableDS.tfmwAlarms.Any Then
                    Throw New Exception("Alarms dataset was not ready at " & Environment.StackTrace.ToString)
                Else
                    Dim c = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms Where a.ErrorCode = errorCode Select a.AlarmID)
                    If c.Any Then
                        Return DirectCast([Enum].Parse(GetType(Alarms), c.FirstOrDefault()), Alarms)
                    Else
                        CreateLogActivity(String.Format("Data integrity error. Alarm with code {0} was returned by firmware, but it does not exist in the database.", errorCode), Reflection.MethodInfo.GetCurrentMethod.ReflectedType.FullName & " . " & Reflection.MethodInfo.GetCurrentMethod.Name, EventLogEntryType.Warning, False)
                        Debug.WriteLine(String.Format("ATTENTION!! DATA INTEGRITY ERROR: Error was found while processing a satus alarm. Alarm with code {0} was not found in the database. ", errorCode))
                        Return AlarmEnumerates.Alarms.NONE
                    End If
                End If

            Catch
                Throw
            End Try
        End Function


        ''' <summary>
        ''' Transforms a string list of Alarm Codes to a list of Alarm Enumerates
        ''' </summary>
        ''' <param name="pAlarmStringCodes">String List of Alarm Codes</param>
        ''' <returns>List of Alarm Enumerates</returns>
        ''' <remarks>
        ''' Created by:  AG 05/03/2012
        ''' Modified by: SA 22/10/2013 - BT #1355 - Added Case for new Alarm WS_PAUSE_MODE_WARN
        '''              XB 20/01/2015 - Add new ISE Alarms - BA-1873
        ''' </remarks>
        Private Function ConvertToAlarmIDEnumerate(ByVal pAlarmStringCodes As List(Of String)) As List(Of Alarms)
            Dim alarmIdEnumList As New List(Of Alarms)
            For Each row As String In pAlarmStringCodes
                alarmIdEnumList.Add(DirectCast([Enum].Parse(GetType(Alarms), row), Alarms))
            Next
            Return alarmIdEnumList
        End Function

        ''' <summary>
        ''' All alarms involving error code must be removed after receive a status instruction with E:0
        ''' All alarms involving FREEZE must be removed after recover instruction finishes
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>AG 06/03/2012
        ''' AG 27/03/2012</remarks>
        Public Function RemoveErrorCodeAlarms(ByVal pDBConnection As SqlConnection, ByVal pAction As AnalyzerManagerAx00Actions) As GlobalDataTO Implements IAnalyzerManager.RemoveErrorCodeAlarms
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim stringAlarmList As New List(Of String)
                        Dim myLinq As List(Of AlarmsDS.tfmwAlarmsRow)
                        Dim okAlarmList As New List(Of Alarms)
                        Dim okAlarmStatusList As New List(Of Boolean)

                        'NOT FREEZE ALARMS!!!
                        'AG 27/03/2012 - all error code not freeze alarms 
                        If pAction <> AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_END Then
                            'Using linq get only those that are not freeze
                            myLinq = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms _
                                      Where a.IsFreezeNull Select a).ToList
                            For Each row As AlarmsDS.tfmwAlarmsRow In myLinq
                                If Not stringAlarmList.Contains(row.AlarmID) Then stringAlarmList.Add(row.AlarmID)
                            Next

                            'Convert alarm string into internal alarm enumerate
                            Dim notFreezeErrorCodeAlarmList As New List(Of Alarms)
                            If stringAlarmList.Count > 0 Then
                                notFreezeErrorCodeAlarmList = ConvertToAlarmIDEnumerate(stringAlarmList)
                            End If

                            'All not freeze error code alarms in myAlarmListAttribute must be removed
                            For Each alarmID As Alarms In notFreezeErrorCodeAlarmList
                                PrepareLocalAlarmList(alarmID, False, okAlarmList, okAlarmStatusList)
                            Next
                        End If
                        'AG 27/03/2012
                        'NOT FREEZE ALARMS!!!

                        'FREEZE ALARMS!!!
                        'Using linq get only those that are freeze
                        myLinq = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefintionTableDS.tfmwAlarms _
                                  Where Not a.IsFreezeNull Select a).ToList
                        For Each row As AlarmsDS.tfmwAlarmsRow In myLinq
                            If Not stringAlarmList.Contains(row.AlarmID) Then stringAlarmList.Add(row.AlarmID)
                        Next

                        'Convert alarm string into internal alarm enumerate
                        Dim freezeAlarmList As New List(Of Alarms)
                        If stringAlarmList.Count > 0 Then
                            freezeAlarmList = ConvertToAlarmIDEnumerate(stringAlarmList)
                        End If

                        'All freeze alarms in myAlarmListAttribute must be removed
                        For Each alarmID As Alarms In freezeAlarmList
                            PrepareLocalAlarmList(alarmID, False, okAlarmList, okAlarmStatusList)
                        Next

                        'Update internal flags
                        analyzerFREEZEFlagAttribute = False
                        analyzerFREEZEModeAttribute = ""
                        AnalyzerIsReadyAttribute = True 'Analyzer is ready
                        'FREEZE ALARMS!!!

                        'TREATMENT!!!
                        If okAlarmList.Count > 0 Then
                            '3- Finally call manage all alarms detected (new or fixed)
                            'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                            'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                            If Not GlobalBase.IsServiceAssembly Then
                                Dim currentAlarms = New AnalyzerAlarms(Me)
                                resultData = currentAlarms.Manage(okAlarmList, okAlarmStatusList)
                            End If
                        End If

                        If pAction <> AnalyzerManagerAx00Actions.RECOVER_INSTRUMENT_START Then
                            UpdateSensorValuesAttribute(AnalyzerSensors.FREEZE, 0, True) 'AG 06/03/2012
                        End If

                        myLinq = Nothing 'AG 02/08/2012 - free memory
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.RemoveErrorCodeAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Ignore alarms cases"
        ''' <summary>
        ''' Inform if the error code must be consider or not
        ''' When Fw send status instruction with error codes they must be treated or not depending
        ''' the last instruction sent by Software
        ''' </summary>
        ''' <param name="pLastInstructionTypeSent"></param>
        ''' <returns>Booelan</returns>
        ''' <remarks>
        ''' Created by  AG 27/03/2012
        ''' Modified by XB 04/05/2012 - add pErrorValue and funtionality related to depurate errors ignores
        '''             TR 19/09/2012 -Add the STATE alarms on validation and POLLSN ( belong to Xavi Badia).
        '''             AG 25/09/2012 - Add the POLLRD instruction
        '''             XB 30/09/2014 - Deactivate old timeout management - Remove too restrictive limitations because timeouts - BA-1872
        ''' </remarks>
        Public Function IgnoreErrorCodes(ByVal pLastInstructionTypeSent As AppLayerEventList, ByVal pInstructionSent As String, ByVal pErrorValue As Integer) As Boolean _
            Implements IAnalyzerManager.IgnoreErrorCodes
            Dim ignoreFlag As Boolean = False 'Default value FALSE
            Try
                ' XBC 18/10/2012 - Alarms treatment for Service
                If GlobalBase.IsServiceAssembly Then
                    ' Service Software no ignore errors
                    If IsServiceAlarmInformed Then
                        ignoreFlag = True
                    ElseIf IsFwUpdateInProcess Then
                        'in case that FW update is currently being performed, ignore alarms.
                        ignoreFlag = True
                    ElseIf IsConfigGeneralProcess Then
                        'in case user is processing general configurations, ignore corresponding alarms.
                        Dim myAlarms As New List(Of Alarms)
                        Dim myErrorCode As New List(Of Integer)
                        myErrorCode.Add(pErrorValue)
                        myAlarms = TranslateErrorCodeToAlarmID(Nothing, myErrorCode)
                        For Each alarmID As Alarms In myAlarms
                            If alarmID = AlarmEnumerates.Alarms.MAIN_COVER_ERR Or _
                               alarmID = AlarmEnumerates.Alarms.MAIN_COVER_WARN Or _
                               alarmID = AlarmEnumerates.Alarms.FRIDGE_COVER_ERR Or _
                               alarmID = AlarmEnumerates.Alarms.FRIDGE_COVER_WARN Or _
                               alarmID = AlarmEnumerates.Alarms.S_COVER_ERR Or _
                               alarmID = AlarmEnumerates.Alarms.S_COVER_WARN Or _
                               alarmID = AlarmEnumerates.Alarms.REACT_COVER_ERR Or _
                               alarmID = AlarmEnumerates.Alarms.REACT_COVER_WARN Or _
                               alarmID = AlarmEnumerates.Alarms.CLOT_SYSTEM_ERR Then
                                ignoreFlag = True
                            End If
                        Next
                    End If

                    If ignoreFlag Then Exit Try

                    ' These instructions are no ignored by Service Sw
                    Select Case pLastInstructionTypeSent
                        Case AppLayerEventList.LOADADJ, AppLayerEventList.ISE_CMD, AppLayerEventList.CONNECT
                            Exit Try
                    End Select
                End If

                Select Case pLastInstructionTypeSent
                    Case AppLayerEventList.CONNECT, AppLayerEventList.CONFIG, AppLayerEventList.READADJ, AppLayerEventList.INFO, AppLayerEventList.SOUND, _
                        AppLayerEventList.POLLFW, AppLayerEventList.POLLHW, AppLayerEventList.LOADADJ, AppLayerEventList.STATE, AppLayerEventList.POLLSN, AppLayerEventList.POLLRD
                        'AG-XB 17/09/2012 - Ignore STATEs in connection process.
                        If pLastInstructionTypeSent = AppLayerEventList.STATE Then
                            If mySessionFlags(AnalyzerManagerFlags.CONNECTprocess.ToString) = "INPROCESS" Then
                                ignoreFlag = True
                            End If
                        Else
                            'Always ignore error code
                            ignoreFlag = True
                        End If

                    Case AppLayerEventList.BARCODE_REQUEST
                        'Ignore error code only when barcode request in configuration mode
                        If Not pInstructionSent Is Nothing Then
                            If InStr(pInstructionSent, "A:1;") > 0 Then
                                ignoreFlag = True
                            End If
                        End If


                    Case AppLayerEventList.ISE_CMD
                        'Ignore error code only when ISE cmd request in low level control mode
                        If Not pInstructionSent Is Nothing Then
                            'The following code requires new Fw not available in 30/03/2012
                            If InStr(pInstructionSent, "M:1;") > 0 Or _
                               InStr(pInstructionSent, "M:4;") > 0 Or _
                               InStr(pInstructionSent, "M:5;") > 0 Then

                                ignoreFlag = True

                                Dim myAlarms As New List(Of Alarms)
                                Dim myErrorCode As New List(Of Integer)
                                myErrorCode.Add(pErrorValue)
                                myAlarms = TranslateErrorCodeToAlarmID(Nothing, myErrorCode)
                                For Each alarmID As Alarms In myAlarms
                                    If alarmID = AlarmEnumerates.Alarms.ISE_FAILED_ERR Or _
                                       alarmID = AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR Then
                                        ignoreFlag = False
                                    End If
                                Next

                            End If
                            If Not ignoreFlag Then

                                If pErrorValue = 61 Then
                                    If ISEAnalyzer IsNot Nothing Then ISEAnalyzer.AbortCurrentProcedureDueToException(True)
                                End If
                                If pErrorValue = 20 Or pErrorValue = 21 Then
                                    If ISEAnalyzer IsNot Nothing Then ISEAnalyzer.AbortCurrentProcedureDueToException()
                                End If
                            End If
                        End If
                End Select


            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.IgnoreErrorCodes", EventLogEntryType.Error, False)
            End Try
            Return ignoreFlag
        End Function

        ''' <summary>
        ''' Most of alarms must be ignored while the start instrument process is in process
        ''' but some of them must be taken into account
        ''' </summary>
        ''' <param name="pAlarmID"></param>
        ''' <returns>Boolean</returns>
        ''' <remarks>
        ''' Created by  AG 27/03/2012
        ''' Modified by XB 31/10/2014 - add ISE_TIMEOUT_ERR alarm - BA-1872
        ''' </remarks>
        Private Function IgnoreAlarmWhileWarmUp(ByVal pAlarmID As Alarms) As Boolean
            Dim ignoreFlag As Boolean = True 'Default value TRUE
            Try
                'AG 22/05/2012 - Note the ise alarms will be asked for when the Terminate button is activated
                If pAlarmID = AlarmEnumerates.Alarms.WASH_CONTAINER_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.WASH_CONTAINER_WARN OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.HIGH_CONTAMIN_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.HIGH_CONTAMIN_WARN OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.WATER_SYSTEM_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.WASTE_DEPOSIT_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.WASTE_SYSTEM_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.BASELINE_INIT_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.REACT_MISSING_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.FRIDGE_COVER_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.S_COVER_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.MAIN_COVER_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.REACT_COVER_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_CP_INSTALL_WARN OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_CP_WRONG_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_LONG_DEACT_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_ELEC_CONS_WARN OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_ELEC_DATE_WARN OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_RP_INVALID_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_RP_EXPIRED_WARN OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_RP_NO_INST_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_ELEC_WRONG_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_OFF_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_A OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_B OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_C OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_D OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_F OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_M OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_N OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_P OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_R OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_S OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_T OrElse pAlarmID = AlarmEnumerates.Alarms.ISE_ERROR_W OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.ISE_TIMEOUT_ERR Then
                    ignoreFlag = False
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.IgnoreAlarmWhileWarmUp", EventLogEntryType.Error, False)
            End Try
            Return ignoreFlag
        End Function

        ''' <summary>
        ''' Most of alarms must be ignored while the communications is not established
        ''' but some of them must be taken into account
        ''' </summary>
        ''' <param name="pAlarmID"></param>
        ''' <returns>Boolean</returns>
        ''' <remarks>AG 27/03/2012</remarks>
        Private Function IgnoreAlarmWhileCommError(ByVal pAlarmID As Alarms) As Boolean
            Dim ignoreFlag As Boolean = True 'Default value TRUE
            Try
                If pAlarmID = AlarmEnumerates.Alarms.REPORTSATLOADED_WARN Then
                    ignoreFlag = False
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.IgnoreAlarmWhileCommError", EventLogEntryType.Error, False)
            End Try
            Return ignoreFlag
        End Function

        ''' <summary>
        ''' Most of alarms must be ignored while the shut down instrument process is in process
        ''' but some of them must be taken into account
        ''' </summary>
        ''' <param name="pAlarmID"></param>
        ''' <returns>Boolean</returns>
        ''' <remarks>AG 18/05/2012</remarks>
        Private Function IgnoreAlarmWhileShutDown(ByVal pAlarmID As Alarms) As Boolean
            Dim ignoreFlag As Boolean = True 'Default value TRUE
            Try
                If pAlarmID = AlarmEnumerates.Alarms.WASH_CONTAINER_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.WASH_CONTAINER_WARN OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.HIGH_CONTAMIN_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.HIGH_CONTAMIN_WARN OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.WATER_SYSTEM_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.WASTE_DEPOSIT_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.WASTE_SYSTEM_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.REACT_MISSING_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.FRIDGE_COVER_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.S_COVER_ERR OrElse _
                  pAlarmID = AlarmEnumerates.Alarms.MAIN_COVER_ERR OrElse pAlarmID = AlarmEnumerates.Alarms.REACT_COVER_ERR Then
                    ignoreFlag = False
                End If

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.IgnoreAlarmWhileShutDown", EventLogEntryType.Error, False)
            End Try
            Return ignoreFlag
        End Function

#End Region

#Region "Alarm timers methods"

        ''' <summary>
        ''' If the reactions rotor thermo warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Created AG 05/01/2012
        ''' </remarks>
        Private Sub ThermoReactionsRotorError_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                thermoReactionsRotorWarningTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                'Change warning to error
                PrepareLocalAlarmList(AlarmEnumerates.Alarms.REACT_TEMP_WARN, False, myAlarmList, myAlarmStatusList)
                PrepareLocalAlarmList(AlarmEnumerates.Alarms.REACT_TEMP_ERR, True, myAlarmList, myAlarmStatusList)
                If myAlarmList.Count > 0 Then
                    If Not GlobalBase.IsServiceAssembly Then
                        Dim currentAlarms = New AnalyzerAlarms(Me)
                        resultData = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ThermoReactionsRotorError_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' If the fridge thermo warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Created AG 05/01/2011
        ''' </remarks>
        Private Sub ThermoFridgeError_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                thermoFridgeWarningTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                'Change warning for error
                PrepareLocalAlarmList(AlarmEnumerates.Alarms.FRIDGE_TEMP_WARN, False, myAlarmList, myAlarmStatusList)
                PrepareLocalAlarmList(AlarmEnumerates.Alarms.FRIDGE_TEMP_ERR, True, myAlarmList, myAlarmStatusList)
                If myAlarmList.Count > 0 Then
                    If Not GlobalBase.IsServiceAssembly Then
                        Dim currentAlarms = New AnalyzerAlarms(Me)
                        resultData = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ThermoFridgeError_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' If the waste deposit warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>AG 01/12/2011</remarks>
        Private Sub WasteDepositError_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            'Fw implements the timer!!
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                wasteDepositTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                'Prepare internal alarm structures and variables
                PrepareLocalAlarmList(AlarmEnumerates.Alarms.WASTE_DEPOSIT_ERR, True, myAlarmList, myAlarmStatusList)

                'Manage the new alarm
                If myAlarmList.Count > 0 Then
                    'AG 29/05/2014 - #1630
                    GlobalBase.CreateLogActivity("Waste deposit full too much time!! Generate alarm WASTE_DEPOSIT_ERR", "AnalyzerManager.WaterDepositError_Timer", EventLogEntryType.Information, False)

                    If GlobalBase.IsServiceAssembly Then
                        resultData = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                    Else
                        Dim currentAlarms = New AnalyzerAlarms(Me)
                        resultData = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.WasteDepositError_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' If the water deposit warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>AG 01/12/2011</remarks>
        Private Sub WaterDepositError_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            'Fw implements the timer!!
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                waterDepositTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                'Prepare internal alarm structures and variables
                PrepareLocalAlarmList(AlarmEnumerates.Alarms.WATER_DEPOSIT_ERR, True, myAlarmList, myAlarmStatusList)

                'Manage the new alarm
                If myAlarmList.Count > 0 Then
                    GlobalBase.CreateLogActivity("Water deposit empty too much time!! Generate alarm WATER_DEPOSIT_ERR", "AnalyzerManager.WaterDepositError_Timer", EventLogEntryType.Information, False)

                    'resultData = ManageAlarms(Nothing, myAlarmList, myAlarmStatusList)
                    'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                    'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                    If GlobalBase.IsServiceAssembly Then
                        resultData = ManageAlarms_SRV(Nothing, myAlarmList, myAlarmStatusList)
                    Else
                        Dim currentAlarms = New AnalyzerAlarms(Me)
                        resultData = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.WaterDepositError_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' If the R1 arm thermo warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Created AG 05/01/2011
        ''' </remarks>
        Private Sub thermoR1ArmWarningTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                thermoR1ArmWarningTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                PrepareLocalAlarmList(AlarmEnumerates.Alarms.R1_TEMP_WARN, True, myAlarmList, myAlarmStatusList)
                If myAlarmList.Count > 0 Then
                    If Not GlobalBase.IsServiceAssembly Then
                        Dim currentAlarms = New AnalyzerAlarms(Me)
                        resultData = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.thermoR1ArmWarningTimer_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>
        ''' If the R2 arm thermo warning persists too much time change warning to error
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Created AG 05/01/2011
        ''' </remarks>
        Private Sub thermoR2ArmWarningTimer_Timer(ByVal source As Object, ByVal e As ElapsedEventArgs)
            Dim resultData As New GlobalDataTO

            Try
                'Deactivate waiting time control
                thermoR2ArmWarningTimer.Enabled = False

                'Inform the alarm 
                Dim myAlarmList As New List(Of Alarms)
                Dim myAlarmStatusList As New List(Of Boolean)

                PrepareLocalAlarmList(AlarmEnumerates.Alarms.R2_TEMP_WARN, True, myAlarmList, myAlarmStatusList)
                If myAlarmList.Count > 0 Then
                    If Not GlobalBase.IsServiceAssembly Then
                        Dim currentAlarms = New AnalyzerAlarms(Me)
                        resultData = currentAlarms.Manage(myAlarmList, myAlarmStatusList)
                    End If
                End If

            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.thermoR2ArmWarningTimer_Timer", EventLogEntryType.Error, False)
            End Try
        End Sub

#End Region
    End Class

End Namespace




