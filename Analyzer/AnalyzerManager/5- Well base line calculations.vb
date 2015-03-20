Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Calculations
Imports System.Timers
Imports System.Data
Imports System.ComponentModel 'AG 20/04/2011 - added when create instance to an BackGroundWorker
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.Core.Entities

    Partial Public Class AnalyzerManager

#Region "Background Worker events"

        ''' <summary>
        ''' Secundary thread that treats the photometric readings. Use a different treat to attend quickly the analyzer requests in Running
        ''' </summary>
        ''' <remarks>
        ''' AG 12/06/2012 - Changed completely the worker now process the biochemical and well base line readings
        '''                 old method is commented, renamed as wellBaseLineWorker_DoWork_AG12062012 and moved to '9- Temporal Testing and Old Methods.vb'
        ''' </remarks>
        Private Sub wellBaseLineWorker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
            Dim resultData As New GlobalDataTO

            Try
                Dim InstructionReceived As New List(Of InstructionParameterTO)
                InstructionReceived = CType(e.Argument, List(Of InstructionParameterTO))
                resultData = ProcessANSPHRInstruction(InstructionReceived)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.wellBaseLineWorker_DoWork", EventLogEntryType.Error, False)
            End Try

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub wellBaseLineWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
            Try

                'AG 03/07/2012 - 
                If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                    'Raise event for UI refrsh
                    'If Not resultData.HasError Then
                    'Now do not use the secondary refresh (mySecondaryUI_RefreshDS, mySecondaryUI_RefreshEvent) use always the primary (myUI_RefreshDS, myUI_RefreshEvent)

                    'If myUI_RefreshEvent.Count = 0 Then myUI_RefreshDS.Clear()
                    ClearRefreshDataSets(True, False) 'AG 22/05/2014 - #1637

                    RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)
                    'eventDataPendingToTriggerFlag = False 'AG 07/10/2011 - inform not exists information in UI_RefreshDS to be send to the event
                    'End If
                End If

                'AG 28/06/2012 - once treated remove item 0
                SyncLock lockThis
                    If bufferANSPHRReceived.Count > 0 Then
                        bufferANSPHRReceived.RemoveRange(0, 1)
                    End If
                    processingLastANSPHRInstructionFlag = False 'Inform NO readings received are in process
                End SyncLock
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.wellBaseLineWorker_RunWorkerCompleted", EventLogEntryType.Error, False)
            End Try
        End Sub
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by XB 11/10/2013 - BT #1319 ==> PAUSE mode management: when analyzer is in mode that allows scan in running and readings are received 
        '''                             with flag Pause = 1 the method ProcessWellBaseLineReadings is not called </remarks>
        Private Function ProcessANSPHRInstruction(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                Dim StartTime As DateTime = Now 'AG 05/06/2012 - time estimation
                'Dim myLogAcciones As New ApplicationLogManager()
                Dim myReadingCycleStatus As Boolean = False

                'AG 02/06/2014 #1644 - Set the semaphore to busy value (before process ANSPHR)
                If GlobalConstants.CreateWSExecutionsWithSemaphore Then
                    GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Waiting (timeout = " & GlobalConstants.SEMAPHORE_TOUT_CREATE_EXECUTIONS.ToString & ")", "AnalyzerManager.ProcessANSPHRInstruction", EventLogEntryType.Information, False)
                    GlobalSemaphores.createWSExecutionsSemaphore.WaitOne(GlobalConstants.SEMAPHORE_TOUT_CREATE_EXECUTIONS)
                    GlobalSemaphores.createWSExecutionsQueue = 1 'Only 1 thread is allowed, so set to 1 instead of increment ++1 'GlobalSemaphores.createWSExecutionsQueue += 1
                    GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Passed through, semaphore busy", "AnalyzerManager.ProcessANSPHRInstruction", EventLogEntryType.Information, False)
                End If

                '2) Call the biochemical readings treatment
                'resultData = ProcessBiochemicalReadings(Nothing, pInstructionReceived)
                resultData = ProcessBiochemicalReadingsNEW(Nothing, pInstructionReceived, myReadingCycleStatus)

                'AG 21/05/2014 activate code: TR 06/05/2014 BT#1612, #1634 -**UNCOMMENT Version 3.0.1**-
                If resultData.HasError AndAlso resultData.ErrorCode = GlobalEnumerates.Messages.READING_NOT_SAVED.ToString() Then
                    GlobalBase.CreateLogActivity("Try saving the reading again... ", "AnalyzerManager.ProcessANSPHRInstruction", EventLogEntryType.Information, False)
                    'Try to save the reading one more time 
                    resultData = ProcessBiochemicalReadingsNEW(Nothing, pInstructionReceived, myReadingCycleStatus)

                    If resultData.HasError Then
                        GlobalBase.CreateLogActivity("2º attempt saving readings FAILED!!!. " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessANSPHRInstruction", EventLogEntryType.Information, False)
                    Else
                        GlobalBase.CreateLogActivity("2º attempt saving the reading SUCCESS!. " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessANSPHRInstruction", EventLogEntryType.Information, False)
                    End If
                End If
                'TR 06/05/2014 BT#1612, #1634 -END

                'AG 02/06/2014 #1644 - Set the semaphore to free value (after process ANSPHR)
                If GlobalConstants.CreateWSExecutionsWithSemaphore Then
                    GlobalSemaphores.createWSExecutionsSemaphore.Release()
                    GlobalSemaphores.createWSExecutionsQueue = 0 'Only 1 thread is allowed, so reset to 0 instead of decrement --1 'GlobalSemaphores.createWSExecutionsQueue -= 1
                    GlobalBase.CreateLogActivity("CreateWSExecutions semaphore: Released, semaphore free", "AnalyzerManager.ProcessANSPHRInstruction", EventLogEntryType.Information, False)
                End If

                GlobalBase.CreateLogActivity("Treat Readings (biochemical): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessANSPHRInstruction", EventLogEntryType.Information, False) 'AG 28/06/2012
                StartTime = Now

                '3) Call the well base line and well rejection control
                'No do this (they are independent processes)
                'If not resultData.HasError  Then
                'resultData = ProcessWellBaseLineReadings (Nothing, InstructionReceived)
                'End If
                'TR + JV #1364 + #1368 29/10/2013
                'If Not (MyClass.AllowScanInRunning And myReadingCycleStatus) Then
                If Not (myReadingCycleStatus) Then
                    resultData = ProcessWellBaseLineReadings(Nothing, pInstructionReceived)
                End If
                'TR + JV #1364 + #1368 29/10/2013

                ''Inform NO readings received are in process (also here, protection against slow refreshs) 
                'processingLastANSPHRInstructionFlag = False

                'AG 03/07/2012 - moved to wellBaseLineWorker_RunWorkerCompleted 
                'If AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING Then
                '    If myUI_RefreshEvent.Count = 0 Then myUI_RefreshDS.Clear()
                '    RaiseEvent ReceptionEvent(InstructionReceivedAttribute, True, myUI_RefreshEvent, myUI_RefreshDS, True)
                'End If

                GlobalBase.CreateLogActivity("Treat readings (Wells Rejections): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessANSPHRInstruction", EventLogEntryType.Information, False)

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessANSPHRInstruction", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Apply the well rejection algorithm
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pInstructionReceived"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  AG 12/06/2012 - created (copied from wellBaseLineWorker_DoWork (v0.4.3)
        ''' Modified by: IT 03/11/2014 - BA-2067: Dynamic BaseLine
        ''' </remarks>
        Private Function ProcessWellBaseLineReadings(ByVal pDBConnection As SqlConnection, ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing

            Try
                'AG 29/06/2012 - Running Cycles lost - Solution!
                'resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                '    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                '    If (Not dbConnection Is Nothing) Then

                ''1) Get the argument
                'Dim InstructionReceived As New List(Of InstructionParameterTO)
                'InstructionReceived = CType(e.Argument, List(Of InstructionParameterTO))

                '2) Get instruction parameters
                'Dim Utilities As New Utilities
                Dim myInstParamTO As New InstructionParameterTO

                ' Get BLW (base line well) field (parameter index 3)
                Dim myBaseLineWell As Integer = 0
                Dim newID As Integer = 0
                resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, 3)
                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    myInstParamTO = DirectCast(resultData.SetDatos, InstructionParameterTO)
                    myBaseLineWell = CType(myInstParamTO.ParameterValue, Integer)

                    resultData = GetNextBaseLineID(dbConnection, AnalyzerIDAttribute, WorkSessionIDAttribute, myBaseLineWell, False)
                    If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                        newID = CType(resultData.SetDatos, Integer)
                    End If
                End If

                'Prepare DS for save results
                If Not resultData.HasError Then
                    Dim myOffset As Integer = 3
                    Dim myWellBaseLineDS As New BaseLinesDS

                    For myIteration As Integer = 1 To GlobalConstants.MAX_WAVELENGTHS
                        Dim newRow As BaseLinesDS.twksWSBaseLinesRow
                        newRow = myWellBaseLineDS.twksWSBaseLines.NewtwksWSBaseLinesRow
                        newRow.AnalyzerID = AnalyzerIDAttribute
                        newRow.WorkSessionID = WorkSessionIDAttribute
                        newRow.BaseLineID = newID
                        newRow.WellUsed = myBaseLineWell
                        newRow.DateTime = Now
                        newRow.Type = GlobalEnumerates.BaseLineType.STATIC.ToString() 'BA-2067
                        newRow.SetMainDarkNull() 'This field is only used in base line with adjust
                        newRow.SetRefDarkNull() 'This field is only used in base line with adjust
                        newRow.SetDACNull() 'This field is only used in base line with adjust
                        newRow.SetITNull() 'This field is only used in base line with adjust
                        newRow.SetABSvalueNull() 'This field is informed after base line by well calculations
                        newRow.SetIsMeanNull()  'This field is informed after base line by well calculations

                        'WaveLenght
                        resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, 4 + (myIteration - 1) * myOffset)
                        If Not resultData.HasError Then
                            newRow.Wavelength = CInt(CType(resultData.SetDatos, InstructionParameterTO).ParameterValue)
                        Else
                            Exit For
                        End If

                        'Main light counts
                        resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, 5 + (myIteration - 1) * myOffset)
                        If Not resultData.HasError Then
                            newRow.MainLight = CInt(CType(resultData.SetDatos, InstructionParameterTO).ParameterValue)
                        Else
                            Exit For
                        End If

                        'Ref light counts
                        resultData = Utilities.GetItemByParameterIndex(pInstructionReceived, 6 + (myIteration - 1) * myOffset)
                        If Not resultData.HasError Then
                            newRow.RefLight = CInt(CType(resultData.SetDatos, InstructionParameterTO).ParameterValue)
                        Else
                            Exit For
                        End If
                        myWellBaseLineDS.twksWSBaseLines.AddtwksWSBaseLinesRow(newRow)
                    Next
                    myWellBaseLineDS.AcceptChanges()

                    'AG 04/05/2011 - results are save in ControlWellBaseLine
                    ''Save results
                    'resultData = SaveBaseLineResults(dbConnection, myWellBaseLineDS, False)

                    If Not resultData.HasError Then
                        'Peform well base line calculations
                        'AG 11/11/2014 BA-2065 #REFACTORING (prepare for dynamic base line)
                        resultData = ControlWellBaseLine(dbConnection, False, myWellBaseLineDS)
                    End If

                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessWellBaseLineReadings", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' This business has been moved into a method from AnalyzerManager.ProcessWellBaseLineReadings
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pClassInitialization"></param>
        ''' <param name="pWellBaseLine"></param>
        ''' <returns></returns>
        ''' <remarks>'AG 11/11/2014 BA-2065 #REFACTORING (prepare for dynamic base line)</remarks>
        Private Function ControlWellBaseLine(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pClassInitialization As Boolean, ByVal pWellBaseLine As BaseLinesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = BaseLine.ControlWellBaseLine(dbConnection, False, pWellBaseLine, BaseLineType.STATIC)
                If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
                    'ControlAdjustBaseLine only generates myAlarm = Alarms.METHACRYL_ROTOR_WARN  ... now 
                    'we have to calculate his status = TRUE or FALSE
                    Dim alarmStatus As Boolean = False 'By default no alarm
                    Dim myAlarm As Alarms = GlobalEnumerates.Alarms.NONE
                    myAlarm = CType(resultData.SetDatos, Alarms)
                    If myAlarm <> GlobalEnumerates.Alarms.NONE Then alarmStatus = True
                    'If Not alarmStatus Then myAlarm = Alarms.METHACRYL_ROTOR_WARN 'AG 04/06/2012 - This method is called in Running, if the alarm METHACRYL_ROTOR_WARN already exists do not remove it

                    Dim AlarmList As New List(Of Alarms)
                    Dim AlarmStatusList As New List(Of Boolean)
                    PrepareLocalAlarmList(myAlarm, alarmStatus, AlarmList, AlarmStatusList)
                    'resultData = ManageAlarms(dbConnection, AlarmList, AlarmStatusList)
                    If AlarmList.Count > 0 Then
                        'resultData = ManageAlarms(dbConnection, AlarmList, AlarmStatusList)
                        'SGM 01/02/2012 - Check if it is Service Assembly - Bug #1112
                        'If My.Application.Info.AssemblyName.ToUpper.Contains("SERVICE") Then
                        If Not GlobalBase.IsServiceAssembly Then
                            Dim StartTime As DateTime = Now 'AG 05/06/2012 - time estimation
                            Dim currentAlarms = New CurrentAlarms(Me)
                            resultData = currentAlarms.Manage(AlarmList, AlarmStatusList)

                            GlobalBase.CreateLogActivity("Treat alarms (well rejection): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "AnalyzerManager.ProcessWellBaseLineReadings", EventLogEntryType.Information, False) 'AG 28/06/2012
                        End If
                    Else
                        ''If no alarm items in list ... inform presentation the reactions rotor is good!!
                        'resultData = PrepareUIRefreshEvent(dbConnection, GlobalEnumerates.UI_RefreshEvents.ALARMS_RECEIVED, 0, 0, _
                        '                                 GlobalEnumerates.Alarms.METHACRYL_ROTOR_WARN.ToString, False)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ControlWellBaseLine", EventLogEntryType.Error, False)

            End Try
            Return resultData
        End Function

#End Region

#Region "NEW METHODS FOR PERFORMANCE IMPROVEMENTS"
        ''' <summary>
        ''' Processing Biochemical Readings: save, calculate, add automatic repetitions, automatic export to LIMS, ...
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pInstructionReceived">List of InstructionParameterTO objects containing all data of an instruction sent by the connected Analyzer</param>
        ''' <param name="pReadingCycleStatus">Return value of field Status (ST) received in the ANSPHR Instruction: 0=Normal Reading Cycle / 1=Pause Reading Cycle</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 02/07/2012 - Based in ProcessBiochemicalReadings; changes to improve the function perfomance
        ''' Modified by: SA 26/07/2012 - When calling function to create Executions, ISEModuleIsReadyAttribute is sent as parameter to allow blocking
        '''                              all ISE Executions when the module is not ready
        '''              AG 03/06/2013 - Changes to fix bug #1170
        '''              JC 12/06/2013 - Added changes to calculate value of CtrlsSendingGroup when an automatic rerun for a Control linked to an Standar Test/SampleType
        '''                              is launched
        '''              TR 10/10/2013 - the ST (Status Photimetric Reading) and introduce to dataset.
        '''              XB 11/10/2013 - Add reference parameter pReadingCycleStatus for the PAUSE mode management - BT #1319
        '''              TR 10/10/2013 - BT #1319 ==> Allow Scanning in Running (Pause): get value of field Status (ST) in the Instruction received, return it in 
        '''                              new ByRef parameter, and inform the Pause status when saving the received Readings
        '''              AG 22/11/2013 - #1397 During 'Recovery Results' in pause mode call method SaveReadings instead of SaverReadingsNEW (it checks if exists before call 
        '''                              the insert in DAO)
        ''' AG 22/05/2014 - #1637 Use exclusive lock (multithread protection)       
        ''' </remarks>
        Private Function ProcessBiochemicalReadingsNEW(ByVal pDBConnection As SqlConnection, _
                                                       ByVal pInstructionReceived As List(Of InstructionParameterTO), _
                                                       ByRef pReadingCycleStatus As Boolean) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing
            Dim instructionSavedFlag As Boolean = False 'AG 21/05/2014 BT#1612, #1634 (update TR initial design)

            Try
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                Dim StartTime As DateTime = Now
                Dim StartTimeAux As DateTime = Now
                'Dim myLogAcciones As New ApplicationLogManager()
                Dim calculationsPerformedFlag As Boolean
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                'Declaration of Constant values
                Const myOffset As Integer = 6               'Instruction OffSet 

                'AG 27/09/2012 - For internalReadingsOffset get the value in SwParameters database 
                'Const internalReadingsOffset As Integer = 2 'Related cycle machine with test programming cycles
                Dim internalReadingsOffset As Integer = 2 'Related cycle machine with test programming cycles
                Dim paramsDelg As New SwParametersDelegate
                myGlobal = paramsDelg.ReadByParameterName(Nothing, GlobalEnumerates.SwParameters.SW_READINGSOFFSET.ToString, myAnalyzerModel)
                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                    If (CType(myGlobal.SetDatos, ParametersDS).tfmwSwParameters.Rows.Count > 0 AndAlso _
                        Not CType(myGlobal.SetDatos, ParametersDS).tfmwSwParameters(0).IsValueNumericNull) Then
                        internalReadingsOffset = CInt(CType(myGlobal.SetDatos, ParametersDS).tfmwSwParameters(0).ValueNumeric)
                    End If
                End If
                'AG 27/09/2012


                'Get the total number of Readings as: Value of limit READING1_CYCLES minus the internal Readings OffSet
                'AG 10/11/2014 BA-2082 filter by Model
                Dim totalReadingsNumber As Integer = 0
                Dim limitList As List(Of FieldLimitsDS.tfmwFieldLimitsRow) = (From a As FieldLimitsDS.tfmwFieldLimitsRow In myClassFieldLimitsDS.tfmwFieldLimits _
                                                                             Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.READING1_CYCLES.ToString AndAlso a.AnalyzerModel = myAnalyzerModel _
                                                                            Select a).ToList
                If (limitList.Count > 0) Then totalReadingsNumber = CInt(limitList.First.MaxValue) - internalReadingsOffset
                'limitList = Nothing 'AG 24/10/2013

                'AG 22/05/2014 #1637 - Use exclusive lock over myUI_RefreshDS variables
                SyncLock myUI_RefreshDS.ReceivedReadings
                    myUI_RefreshDS.ReceivedReadings.Clear() 'Clear DS used for update of presentation layer (only the proper data table)
                End SyncLock

                'Process all Readings received
                'Dim Utilities As New Utilities()

                Dim myExecutionDS As New ExecutionsDS
                Dim allExecutionsDS As New ExecutionsDS
                Dim myToUpdateExecDS As New ExecutionsDS
                Dim myExecutionDlg As New ExecutionsDelegate

                Dim myReadingDS As New twksWSReadingsDS
                Dim myReadingRow As twksWSReadingsDS.twksWSReadingsRow
                Dim myReadingsDelegate As New WSReadingsDelegate

                Dim myWellUsed As Integer = 0
                Dim myLedPosition As Integer = 0
                Dim myMainCounts As Integer = 0
                Dim myRefCounts As Integer = 0
                Dim myReadingNumber As Integer = 0
                Dim myPreparationID As Integer = 0
                Dim localBaseLineID As Integer = 0
                Dim myAdjustBaseLineID As Integer = 0
                Dim executionUpdated As Boolean
                Dim validReadingFlag As Boolean
                Dim completeReadingsFlag As Boolean

                'TR 10/10/2013 declare variable to get the ST (Status Photimetric Reading)
                Dim myPause As Boolean = False
                myGlobal = Utilities.GetItemByParameterIndex(pInstructionReceived, 47)
                If Not (myGlobal.HasError) Then
                    myPause = CBool(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)
                End If
                'TR 10/10/2013 -END.

                pReadingCycleStatus = myPause   ' XB 11/10/2013 - BT #1319

                'AG 14/03/2014 - #1524 - before start processing the biochemical readings, set 'false' sensor with no auto rerun added!!!
                UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_RERUN_ADDED, 0, False)

                For i As Integer = 1 To totalReadingsNumber
                    'AG 25/09/2012 - remember the prepID during recovery results
                    If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" Then
                        If Not recoveredPrepIDList.Contains(myPreparationID) Then recoveredPrepIDList.Add(myPreparationID)
                    Else
                        recoveredPrepIDList.Clear()
                    End If
                    'AG 25/09/2012

                    'Read the PreparationID using ParameterIndex = 48 + (i - 1) * 6
                    myGlobal = Utilities.GetItemByParameterIndex(pInstructionReceived, (49 + (i - 1) * myOffset)) '48 +1 TR 10/10/2013 
                    If (myGlobal.HasError) Then Exit For

                    myPreparationID = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)
                    If (myPreparationID <> 0) Then   'Ignore DUMMY Readings
                        'Read the ReadingNumber using ParameterIndex = 47 + (i - 1) * 6
                        myGlobal = Utilities.GetItemByParameterIndex(pInstructionReceived, (48 + (i - 1) * myOffset)) '47+1 TR 10/10/2013 
                        If (myGlobal.HasError) Then Exit For
                        myReadingNumber = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)

                        'Read the LedPosition using ParameterIndex = 50 + (i - 1) * 6
                        myGlobal = Utilities.GetItemByParameterIndex(pInstructionReceived, (51 + (i - 1) * myOffset)) '50+1 TR 10/10/2013 
                        If (myGlobal.HasError) Then Exit For
                        myLedPosition = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)

                        'Read the MainCounts using ParameterIndex = 51 + (i - 1) * 6
                        myGlobal = Utilities.GetItemByParameterIndex(pInstructionReceived, (52 + (i - 1) * myOffset)) '51+1 TR 10/10/2013 
                        If (myGlobal.HasError) Then Exit For
                        myMainCounts = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)

                        'Read the RefCounts using ParameterIndex = 52 + (i - 1) * 6
                        myGlobal = Utilities.GetItemByParameterIndex(pInstructionReceived, (53 + (i - 1) * myOffset)) '52 + 1 TR 10/10/2013 
                        If (myGlobal.HasError) Then Exit For
                        myRefCounts = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)

                        'Get the Execution related with the PreparationID but only if it status is different of Pending or Locked 
                        myGlobal = myExecutionDlg.GetExecutionByPreparationID(Nothing, myPreparationID, WorkSessionIDAttribute, AnalyzerIDAttribute, True)
                        If (myGlobal.HasError OrElse myGlobal.SetDatos Is Nothing) Then Exit For

                        myExecutionDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                        If (myExecutionDS.twksWSExecutions.Rows.Count > 0) Then
                            executionUpdated = False

                            'For the 1st Reading, inform fields WellUsed, BaseLineID and AdjustBaseLineID in the ExecutionsDS
                            'If it is not the 1st Reading, but at least one of the fields is empty, then inform them as well
                            If (myReadingNumber = 1 OrElse myExecutionDS.twksWSExecutions(0).IsWellUsedNull OrElse _
                                myExecutionDS.twksWSExecutions(0).IsBaseLineIDNull OrElse _
                                myExecutionDS.twksWSExecutions(0).IsAdjustBaseLineIDNull) Then

                                'Read the ReadingNumber using ParameterIndex = 49 + (i - 1) * 6
                                myGlobal = Utilities.GetItemByParameterIndex(pInstructionReceived, (50 + (i - 1) * myOffset)) '49 + 1 TR 10/10/2013 
                                If (myGlobal.HasError) Then Exit For
                                myWellUsed = CInt(CType(myGlobal.SetDatos, InstructionParameterTO).ParameterValue)

                                'Get the BaseLineID for the base lines without adjust (from table twksWSBLinesByWell)
                                myGlobal = GetCurrentBaseLineID(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, myWellUsed, False)
                                If (myGlobal.HasError OrElse myGlobal.SetDatos Is Nothing) Then Exit For
                                localBaseLineID = DirectCast(myGlobal.SetDatos, Integer)

                                'Get the BaseLineID for the base lines with adjust (from table twksWSBLines)
                                myGlobal = GetCurrentBaseLineID(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, myWellUsed, True) ''AG 28/05/2014 - #1644 - Make code more readable (use Nothing instead of dbConnection)
                                If (myGlobal.HasError OrElse myGlobal.SetDatos Is Nothing) Then Exit For
                                myAdjustBaseLineID = DirectCast(myGlobal.SetDatos, Integer)

                                'Update values in the ExecutionsDS
                                myExecutionDS.twksWSExecutions(0).BeginEdit()
                                myExecutionDS.twksWSExecutions(0).WellUsed = myWellUsed
                                myExecutionDS.twksWSExecutions(0).BaseLineID = localBaseLineID
                                myExecutionDS.twksWSExecutions(0).AdjustBaseLineID = myAdjustBaseLineID
                                myExecutionDS.twksWSExecutions(0).HasReadings = True
                                myExecutionDS.twksWSExecutions(0).EndEdit()

                                executionUpdated = True

                                'AG 28/05/2014 - #1644 - When 1st reading is received remove all previous readings linked with this execution

                                If myReadingNumber = 1 Then
                                    GlobalBase.CreateLogActivity("Call myReadingsDelegate.Delete ", "AnalyzerManager.ProcessBiochemicalReadingsNEW", EventLogEntryType.Information, False)
                                    myGlobal = myReadingsDelegate.Delete(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, myExecutionDS)
                                End If
                                'AG 28/05/2014

                            End If

                            'Verify if it is needed to activate ThermoWarningFlag for the Execution
                            If (Not myExecutionDS.twksWSExecutions(0).ThermoWarningFlag) Then
                                If (myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_WARN) OrElse _
                                    myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_ERR) OrElse _
                                    myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.REACT_TEMP_SYS_ERR)) Then
                                    myExecutionDS.twksWSExecutions(0).BeginEdit()
                                    myExecutionDS.twksWSExecutions(0).ThermoWarningFlag = True
                                    myExecutionDS.twksWSExecutions(0).EndEdit()

                                    executionUpdated = True
                                End If
                            End If

                            'Verify Sample Arm obstructed
                            If (myExecutionDS.twksWSExecutions(0).IsClotValueNull OrElse _
                                Not String.Equals(myExecutionDS.twksWSExecutions(0).ClotValue, GlobalEnumerates.Ax00ArmClotDetectionValues.BS.ToString)) Then
                                If (myAlarmListAttribute.Contains(GlobalEnumerates.Alarms.S_OBSTRUCTED_ERR)) Then
                                    myExecutionDS.twksWSExecutions(0).BeginEdit()
                                    myExecutionDS.twksWSExecutions(0).ClotValue = GlobalEnumerates.Ax00ArmClotDetectionValues.BS.ToString
                                    myExecutionDS.twksWSExecutions(0).EndEdit()

                                    executionUpdated = True
                                End If
                            End If

                            'Fw first Reading = 1, but for Sw is Reading at 18s... RN=3
                            myReadingNumber += internalReadingsOffset

                            'Move the Reading to the final DS that will be used to save all Readings for the Execution
                            myReadingRow = myReadingDS.twksWSReadings.NewtwksWSReadingsRow
                            myReadingRow.AnalyzerID = myExecutionDS.twksWSExecutions(0).AnalyzerID
                            myReadingRow.WorkSessionID = myExecutionDS.twksWSExecutions(0).WorkSessionID
                            myReadingRow.ExecutionID = myExecutionDS.twksWSExecutions(0).ExecutionID
                            myReadingRow.ReactionComplete = True          'Set ReactionComplete = TRUE (always when the Sample has been dispensed)
                            myReadingRow.ReadingNumber = myReadingNumber
                            myReadingRow.LedPosition = myLedPosition
                            myReadingRow.MainCounts = myMainCounts
                            myReadingRow.RefCounts = myRefCounts
                            myReadingRow.DateTime = Now
                            myReadingRow.Pause = myPause 'TR 10/10/2013 -Set the new Pause value
                            myReadingDS.twksWSReadings.Rows.Add(myReadingRow)

                            'Verify if the Reading is valid...
                            validReadingFlag = True
                            If (myMainCounts = GlobalConstants.SATURATED_READING OrElse myMainCounts = GlobalConstants.READING_ERROR OrElse _
                                myRefCounts = GlobalConstants.SATURATED_READING OrElse myRefCounts = GlobalConstants.READING_ERROR) Then
                                validReadingFlag = False
                            End If

                            'If it is not the first Reading, verify if the previous Reading is completed
                            completeReadingsFlag = True
                            If ((myReadingNumber - internalReadingsOffset) > 1) Then
                                myGlobal = myReadingsDelegate.GetPreviousReading(Nothing, myReadingRow)
                                If (myGlobal.HasError OrElse myGlobal.SetDatos Is Nothing) Then Exit For

                                completeReadingsFlag = (CType(myGlobal.SetDatos, twksWSReadingsDS).twksWSReadings.Count > 0)
                            End If

                            '1- Update ValidRedingFlag (1st reading initiate value, else evaluate it but if current value is FALSE do not change it!!!)
                            If ((myReadingNumber - internalReadingsOffset) = 1) OrElse (Not validReadingFlag AndAlso myExecutionDS.twksWSExecutions(0).ValidReadings <> validReadingFlag) Then
                                myExecutionDS.twksWSExecutions(0).BeginEdit()
                                myExecutionDS.twksWSExecutions(0).ValidReadings = validReadingFlag
                                myExecutionDS.twksWSExecutions(0).EndEdit()
                                executionUpdated = True
                            End If

                            '2- Update CompleteReadings (1st reading set to TRUE, else evaluate it but if current value is FALSE do not change it!!!)
                            If ((myReadingNumber - internalReadingsOffset) = 1) OrElse (Not completeReadingsFlag AndAlso myExecutionDS.twksWSExecutions(0).CompleteReadings <> completeReadingsFlag) Then
                                myExecutionDS.twksWSExecutions(0).BeginEdit()
                                myExecutionDS.twksWSExecutions(0).CompleteReadings = completeReadingsFlag
                                myExecutionDS.twksWSExecutions(0).EndEdit()
                                executionUpdated = True
                            End If

                            'Move the Execution to the DS containing all processed Executions
                            allExecutionsDS.twksWSExecutions.ImportRow(myExecutionDS.twksWSExecutions(0))

                            'Besides, if the Execution has been updated, move it to the DS of Executions to update
                            If (executionUpdated) Then myToUpdateExecDS.twksWSExecutions.ImportRow(myExecutionDS.twksWSExecutions(0))

                            'Finally, prepare the UI Refresh for the Execution
                            myGlobal = PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.READINGS_RECEIVED, myReadingRow.ExecutionID, myReadingRow.ReadingNumber, Nothing, False)
                            'AG 21/05/2014 BT#1612, #1634 (comment next line applies only for #1634 - If this method (used only for real time presentation refresh) returns error do not stop saving process. Skip the error!!)
                            'If (myGlobal.HasError) Then Exit For
                            If (myGlobal.HasError) Then myGlobal.HasError = False 'skip error in function PrepareUIRefreshEvent
                        End If
                    End If
                Next i

                If (Not myGlobal.HasError) Then
                    'Open a DB Transaction to UPDATE all affected Executions and INSERT the group of Readings
                    myGlobal = DAOBase.GetOpenDBTransaction(pDBConnection)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        dbConnection = DirectCast(myGlobal.SetDatos, SqlConnection)
                        If (Not dbConnection Is Nothing) Then
                            'Update all affected Executions....and if not error, then insert the group of Readings
                            If (myToUpdateExecDS.twksWSExecutions.Rows.Count > 0) Then myGlobal = myExecutionDlg.UpdateReadingsFieldsNEW(dbConnection, myToUpdateExecDS)

                            'AG 22/11/2013 - Task #1397 during recovery results in pause mode call SaveReadings instead of SaveReadingsNEW
                            'NOTE: the AllowScanInRunningAttribute is now FALSE, we have to use AppLayer.RecoveryResultsInPause  property

                            'If (Not myGlobal.HasError) Then myGlobal = myReadingsDelegate.SaveReadingsNEW(dbConnection, myReadingDS)
                            If mySessionFlags(GlobalEnumerates.AnalyzerManagerFlags.RESULTSRECOVERProcess.ToString) = "INPROCESS" AndAlso AppLayer.RecoveryResultsInPause Then
                                'Recovery results during pause mode scenario
                                If (Not myGlobal.HasError) Then myGlobal = myReadingsDelegate.SaveReadings(dbConnection, myReadingDS)
                            Else
                                GlobalBase.CreateLogActivity("Call myReadingsDelegate.SaveReadingsNEW", "AnalyzerManager.ProcessBiochemicalReadingsNEW", EventLogEntryType.Information, False)

                                'Normal scenario
                                If (Not myGlobal.HasError) Then myGlobal = myReadingsDelegate.SaveReadingsNEW(dbConnection, myReadingDS)
                            End If
                            'AG 22/11/2013

                            'Rollback or Commit the DB Transaction depending if an error was raised or not
                            If (Not myGlobal.HasError) Then
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                instructionSavedFlag = True 'AG 21/05/2014 BT#1612, #1634 (flag indicates instruction saved OK)
                            Else
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
                        End If
                    End If
                End If

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                GlobalBase.CreateLogActivity("Decode and Save Readings: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                "AnalyzerManager.ProcessBiochemicalReadingsNEW", EventLogEntryType.Information, False)
                StartTime = Now
                calculationsPerformedFlag = False
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                If (Not myGlobal.HasError) Then
                    Dim repCreatedFlag As Boolean
                    Dim isTheLastMultiItem As Boolean
                    Dim executionCLOSEDNOK As Boolean
                    Dim myTestReadingNumber As Integer
                    Dim validAndCompleteReadings As Boolean

                    Dim newWellStatusDS As New ReactionsRotorDS
                    Dim reactionsRotorDlg As New ReactionsRotorDelegate
                    Dim myWellsRow As ReactionsRotorDS.twksWSReactionsRotorRow

                    Dim myMultiItemExecDS As New ExecutionsDS
                    Dim myExecutionRow As List(Of ExecutionsDS.twksWSExecutionsRow)

                    'Clear DS used for update of presentation layer (only the proper data table)
                    'myUI_RefreshDS.ExecutionStatusChanged.Clear() 'AG 07/03/2014 - #1534 comment this line #1524 Do not clear the datatable!! It will be cleared once the event is launch to presentation!!!!

                    'Final process for all created Readings....
                    For Each readingsRow As twksWSReadingsDS.twksWSReadingsRow In myReadingDS.twksWSReadings.Rows
                        repCreatedFlag = False 'AG 03/06/2013 - reset flag for each reading to be treated

                        'Get all data of the Execution to which the Reading belong
                        myExecutionRow = (From b As ExecutionsDS.twksWSExecutionsRow In allExecutionsDS.twksWSExecutions _
                                         Where b.ExecutionID = readingsRow.ExecutionID Select b).ToList()

                        'Get the last Reading Number for the Test of the Execution
                        If (Not myExecutionRow.First.IsSecondReadingCycleNull) Then
                            'Bireagent Test, the last Reading Number is the value of field SecondReadingCycle
                            myTestReadingNumber = myExecutionRow.First.SecondReadingCycle
                        Else
                            'Monoreagent Test, the last Reading Number is the value of field FirstReadingCycle
                            myTestReadingNumber = myExecutionRow.First.FirstReadingCycle
                        End If

                        'AG 24/10/2013 Task #1347. Take into account the possible offset in R2 additonal due to paused mode
                        If (readingsRow.ReadingNumber >= myTestReadingNumber) AndAlso (myExecutionRow.First.ExecutionStatus = "INPROCESS") Then
                            Dim delayedR2Cycles As Integer = 0
                            Dim defaultR2IsAdded As Integer = 0
                            Dim maxReadingsWithR2 As Integer = 0

                            'Use linq for que the limits for R2
                            'AG 10/11/2014 BA-2082 filter by model
                            limitList = (From a In myClassFieldLimitsDS.tfmwFieldLimits _
                                        Where a.LimitID = GlobalEnumerates.FieldLimitsEnum.READING2_CYCLES.ToString AndAlso a.AnalyzerModel = myAnalyzerModel _
                                        Select a).ToList
                            If limitList.Count > 0 Then
                                defaultR2IsAdded = CInt(limitList(0).MinValue) - internalReadingsOffset
                                maxReadingsWithR2 = CInt(limitList(0).MaxValue) - CInt(limitList(0).MinValue) + 1
                            End If

                            Dim realR2IsAdded As Integer = 0
                            myGlobal = myReadingsDelegate.GetR2DelayedCycles(Nothing, AnalyzerIDAttribute, myAnalyzerModel, WorkSessionIDAttribute, _
                                                                             readingsRow.ExecutionID, defaultR2IsAdded, maxReadingsWithR2, internalReadingsOffset, _
                                                                             realR2IsAdded, myExecutionRow.First.TestID, Nothing)
                            If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                                delayedR2Cycles = DirectCast(myGlobal.SetDatos, Integer)
                            End If

                            GlobalBase.CreateLogActivity("Task #1347 - New rules before call calculations, delayed R2 cycles = " & delayedR2Cycles, "AnalyzerManager.ProcessBiochemicalReadingsNEW", EventLogEntryType.Information, False)

                            'Apply offset to the test reading number in order to add the R2 delayed cycles
                            myTestReadingNumber += delayedR2Cycles
                        End If
                        'AG 24/10/2013

                        'Continue only if this is the last Reading for the Test; if not go to process the next Reading
                        If (readingsRow.ReadingNumber = myTestReadingNumber) Then
                            GlobalBase.CreateLogActivity("Task #1347 - Rules for calculations triggers", "AnalyzerManager.ProcessBiochemicalReadingsNEW", EventLogEntryType.Information, False)

                            'For Multipoint Calibrators, verify if the point in process is the last point of the Calibration Kit
                            'For the rest of Sample Classes, it is always the last MultiItem because there is the only one
                            If String.Equals(myExecutionRow.First.SampleClass, "CALIB") Then
                                'Verify if it is a MultiPoint Calibrator by getting all Executions for the same OrderTestID, RerunNumber and ReplicateNumber
                                myGlobal = myExecutionDlg.GetExecutionMultiItemsNEW(Nothing, AnalyzerIDAttribute, WorkSessionIDAttribute, myExecutionRow.First.OrderTestID, _
                                                                                    myExecutionRow.First.RerunNumber, myExecutionRow.First.ReplicateNumber)
                                If (myGlobal.HasError OrElse myGlobal.SetDatos Is Nothing) Then Exit For

                                myMultiItemExecDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                                isTheLastMultiItem = (myExecutionRow.First.MultiItemNumber = myMultiItemExecDS.twksWSExecutions.Last.MultiItemNumber)
                            Else
                                'Add the Execution to the DS for later use
                                myMultiItemExecDS.Clear()
                                myMultiItemExecDS.twksWSExecutions.ImportRow(myExecutionRow.First)

                                isTheLastMultiItem = True
                            End If

                            If (isTheLastMultiItem) Then
                                'Verify if all the Readings for the Execution have been received 
                                validAndCompleteReadings = True
                                If (Not myExecutionRow.First.IsCompleteReadingsNull AndAlso Not myExecutionRow.First.CompleteReadings) OrElse _
                                   (Not myExecutionRow.First.IsValidReadingsNull AndAlso Not myExecutionRow.First.ValidReadings) Then
                                    validAndCompleteReadings = False
                                End If

                                'Apply calculations only if all Readings for the Execution are complete and valid; otherwise mark the Execution as Closed by NOK
                                executionCLOSEDNOK = True
                                If (validAndCompleteReadings) Then
                                    'AG 30/10/2014 BA-2064 comment new code temporally
                                    Dim myCalc As New CalculationsDelegate() '!!Declare this variable inside the loop. Otherwise, some structures keep information of previous Executions calculated
                                    myGlobal = myCalc.CalculateExecutionNEW(Nothing, readingsRow.AnalyzerID, readingsRow.WorkSessionID, readingsRow.ExecutionID, False, "")
                                    'myGlobal = Calculations.CalculateExecutionNEW(Nothing, readingsRow.AnalyzerID, readingsRow.WorkSessionID, readingsRow.ExecutionID, False, "")

                                    If (Not myGlobal.HasError) Then
                                        myGlobal = PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED, readingsRow.ExecutionID, _
                                                                         readingsRow.ReadingNumber, Nothing, False)
                                        executionCLOSEDNOK = False
                                    End If
                                    myCalc = Nothing 'AG 02/08/2012 release memory

                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                    calculationsPerformedFlag = True
                                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                End If

                                If (executionCLOSEDNOK) Then
                                    'Execution (or Executions if it is a multipoint Calibrator) is marked as ClosedNOK
                                    myGlobal = myExecutionDlg.UpdateStatusClosedNOK_NEW(Nothing, myMultiItemExecDS, True)
                                    If (Not myGlobal.HasError) Then myGlobal = PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.EXECUTION_STATUS, _
                                                                                                     readingsRow.ExecutionID, 0, Nothing, False)
                                End If

                                'Inform to UIRefresh that the Status of all Well used is finished (calculation are done)
                                If (Not myGlobal.HasError) Then
                                    newWellStatusDS.Clear()
                                    For Each executionRow As ExecutionsDS.twksWSExecutionsRow In myMultiItemExecDS.twksWSExecutions.Rows
                                        myWellsRow = newWellStatusDS.twksWSReactionsRotor.NewtwksWSReactionsRotorRow()
                                        myWellsRow.AnalyzerID = executionRow.AnalyzerID
                                        myWellsRow.WellNumber = executionRow.WellUsed
                                        myWellsRow.WellStatus = "F"
                                        myWellsRow.ExecutionID = executionRow.ExecutionID
                                        newWellStatusDS.twksWSReactionsRotor.AddtwksWSReactionsRotorRow(myWellsRow)
                                    Next
                                    newWellStatusDS.AcceptChanges()

                                    myGlobal = reactionsRotorDlg.Update(dbConnection, newWellStatusDS, True)
                                    If (Not myGlobal.HasError) Then myGlobal = PrepareUIRefreshEventNum3(Nothing, GlobalEnumerates.UI_RefreshEvents.REACTIONS_WELL_STATUS_CHANGED, _
                                                                                                         newWellStatusDS, True)
                                End If

                                If (Not myGlobal.HasError) Then
                                    'Call Repetitions and LIMS Export if last Replicate of the OrderTest has been received
                                    If (myExecutionRow.First.ReplicateNumber = myExecutionRow.First.ReplicatesTotalNum) Then
                                        Const newSamplesRotorStatus As String = ""
                                        Dim myRepetitionsDelegate As New RepetitionsDelegate
                                        Dim rotorPosDS As New WSRotorContentByPositionDS
                                        Dim rcpDelegate As New WSRotorContentByPositionDelegate
                                        Dim rcpList As New List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                                        Dim myExportedExecutionsDS As New ExecutionsDS
                                        Dim myExportDelegate As New ExportDelegate

                                        'Process Repetitions
                                        Dim iseModuleReady As Boolean = (Not ISEAnalyzer Is Nothing AndAlso ISEAnalyzer.IsISEModuleReady)
                                        repCreatedFlag = False 'AG 03/06/2013 - Reset this flag for each results to be evaluated

                                        'JCM 12/06/2013
                                        'If Sample Class is Control: Read MaxControlsSending
                                        Dim myQCResults As New WSOrderTestsDelegate
                                        Dim maxCtrlsSending As Integer = 1

                                        'If the Execution is for a CONTROL of an Standard Test, get the maximum Sending Group for the Test/SampleType in the WS
                                        If (myExecutionRow.First.SampleClass = "CTRL") Then
                                            myGlobal = myQCResults.GetMaxCtrlsSendingGroup(Nothing, readingsRow.WorkSessionID, myExecutionRow.First.TestID, myExecutionRow.First.SampleType)
                                            If (myGlobal.HasError) Then Exit For

                                            maxCtrlsSending = CType(myGlobal.SetDatos, Integer)
                                        End If

                                        'Verify is the current Analyzer Status is RUNNING
                                        Dim runningMode As Boolean = (AnalyzerStatusAttribute = GlobalEnumerates.AnalyzerManagerStatus.RUNNING)

                                        'AG 31/03/2014 - #1565 add parameter pRunningMode in the proper position in method ManageRepetitions
                                        myGlobal = myRepetitionsDelegate.ManageRepetitions(Nothing, readingsRow.AnalyzerID, readingsRow.WorkSessionID, _
                                                                                          myExecutionRow.First.OrderTestID, myExecutionRow.First.RerunNumber, _
                                                                                          repCreatedFlag, newSamplesRotorStatus, runningMode, False, String.Empty, iseModuleReady)
                                        'AG 31/03/2014 - #1565

                                        If (myGlobal.HasError) Then Exit For

                                        If (repCreatedFlag) Then
                                            rcpList = CType(myGlobal.SetDatos, List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow))

                                            'JCM 12/06/2013
                                            'If the Execution is for a CONTROL of an Standard Test:
                                            '*** Increment the MaxCtrlSendingGroup only for the first automatic rerun generated for the Test/SampleType; automatic Reruns for the
                                            '    rest of linked Controls will be included in the same Sending Group
                                            '*** Update the CtrlsSendingGroup for the OrderTestID in table twksWSOrderTests
                                            If (myExecutionRow.First.SampleClass = "CTRL") Then
                                                If (maxCtrlsSending <> (myExecutionRow.First.CtrlsSendingGroup + 1)) Then maxCtrlsSending += 1

                                                myGlobal = myQCResults.UpdateCtrlsSendingGroup(Nothing, readingsRow.WorkSessionID, myExecutionRow.First.OrderTestID, maxCtrlsSending)
                                                If (myGlobal.HasError) Then Exit For
                                            End If

                                            For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rcpList
                                                myGlobal = PrepareUIRefreshEventNum2(Nothing, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, "SAMPLES", row.CellNumber, _
                                                                                      newSamplesRotorStatus, "", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                                                If myGlobal.HasError Then Exit For
                                            Next

                                            'AG 14/03/2014 - #1524 - Inform that auto rerun added for refresh screen use
                                            UpdateSensorValuesAttribute(GlobalEnumerates.AnalyzerSensors.AUTO_RERUN_ADDED, 1, True)
                                        Else
                                            myGlobal = rcpDelegate.UpdateSamplePositionStatus(Nothing, readingsRow.ExecutionID, WorkSessionIDAttribute, _
                                                                                              AnalyzerIDAttribute, -1, "", myExecutionRow.First.MultiItemNumber)

                                            If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                                rotorPosDS = DirectCast(myGlobal.SetDatos, WSRotorContentByPositionDS)

                                                For Each row As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In rotorPosDS.twksWSRotorContentByPosition.Rows
                                                    myGlobal = PrepareUIRefreshEventNum2(Nothing, GlobalEnumerates.UI_RefreshEvents.ROTORPOSITION_CHANGED, "SAMPLES", row.CellNumber, _
                                                                                         row.Status, "", -1, -1, "", "", Nothing, Nothing, Nothing, -1, -1, "", "")
                                                    If (myGlobal.HasError) Then Exit For
                                                Next
                                            End If
                                        End If
                                        If (myGlobal.HasError) Then Exit For

                                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                        StartTimeAux = Now
                                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

                                        'Export to LIMS
                                        myGlobal = myExportDelegate.ManageLISExportation(Nothing, readingsRow.AnalyzerID, readingsRow.WorkSessionID, myExecutionRow.First.OrderTestID, False)
                                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                            myExportedExecutionsDS = DirectCast(myGlobal.SetDatos, ExecutionsDS)
                                            For Each ex_row As ExecutionsDS.twksWSExecutionsRow In myExportedExecutionsDS.twksWSExecutions.Rows
                                                myGlobal = PrepareUIRefreshEvent(Nothing, GlobalEnumerates.UI_RefreshEvents.RESULTS_CALCULATED, ex_row.ExecutionID, _
                                                                                 Nothing, Nothing, False)
                                                If (myGlobal.HasError) Then Exit For
                                            Next
                                            AddIntoLastExportedResults(myExportedExecutionsDS) 'AG 19/03/2013

                                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                            GlobalBase.CreateLogActivity("Total time Export and Refresh OT = " & myExecutionRow.First.OrderTestID & ": " & _
                                                                            Now.Subtract(StartTimeAux).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                            "AnalyzerManager.ProcessBiochemicalReadingsNEW", EventLogEntryType.Information, False)
                                            StartTimeAux = Now
                                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                        End If
                                        '//JVV 19/09/13 Auto-Report: las funcionalidades pedidas son similares a la exportación de LIMS
                                        Dim myAutoreportDelegate As New AutoReportsDelegate
                                        myGlobal = myAutoreportDelegate.ManageAutoReportCreationExecutions(Nothing, readingsRow.AnalyzerID, readingsRow.WorkSessionID, myExecutionRow.First.OrderTestID, False)
                                        If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                                            Dim oList As New List(Of String)
                                            oList = DirectCast(myGlobal.SetDatos, List(Of String))
                                            If oList.Count > 0 Then
                                                myGlobal = PrepareUIRefreshEventNum4(Nothing, GlobalEnumerates.UI_RefreshEvents.AUTO_REPORT, readingsRow.AnalyzerID, readingsRow.WorkSessionID, oList)
                                            End If

                                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                            GlobalBase.CreateLogActivity("Auto-Report OrdersID = " & myExecutionRow.First.OrderTestID & ": " & _
                                                                            Now.Subtract(StartTimeAux).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                            "AnalyzerManager.ProcessBiochemicalReadingsNEW", EventLogEntryType.Information, False)
                                            StartTimeAux = Now
                                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                                        End If
                                        '//JVV 19/09/13

                                        rcpList = Nothing 'AG 24/10/2013
                                        If (myGlobal.HasError) Then Exit For
                                    End If
                                End If
                            End If
                        End If
                    Next
                    myExecutionRow = Nothing

                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                    If (calculationsPerformedFlag) Then
                        GlobalBase.CreateLogActivity("Calculate Results: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                        "AnalyzerManager.ProcessBiochemicalReadingsNEW", EventLogEntryType.Information, False)
                    End If
                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                End If
                limitList = Nothing 'AG 24/10/2013

            Catch ex As Exception
                'If a Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobal = New GlobalDataTO()
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerManager.ProcessBiochemicalReadingsNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()

                'AG 21/05/2014 BT#1612, #1634 (change TR initial design) Indicate there was an error and instruction has not been saved -**UNCOMMENT Version 3.0.1**-
                If Not instructionSavedFlag Then
                    myGlobal.ErrorCode = GlobalEnumerates.Messages.READING_NOT_SAVED.ToString()
                    myGlobal.HasError = True
                    'Dim myLogAcciones As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity("There was an error and the ANSPHR instruction had not been saved. Set error code = READING_NOT_SAVED! ", _
                                "AnalyzerManager.ProcessBiochemicalReadingsNEW", EventLogEntryType.Information, False)
                End If
                'TR 06/05/2014 BT#1612 -END

            End Try
            Return myGlobal
        End Function
#End Region
    End Class
End Namespace
