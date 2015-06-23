Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Core.Entities.Worksession.Interfaces
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Core.Entities.Worksession

Namespace Biosystems.Ax00.Core.Interfaces

    Public Interface IAnalyzerManager

#Region "Properties"

        Property BaseLine As IBaseLineEntity
        Property ISEAnalyzer As IISEManager

        'AG 30/10/2014 BA-2064 comment new code temporally
        'Property Calculations As CalculationsDelegate

        Property classInitializationError() As Boolean
        Property ActiveWorkSession() As String
        ReadOnly Property WorkSessionStatus() As String
        Property ActiveAnalyzer() As String
        Property ActiveFwVersion() As String
        Property ReadedFwVersion() As String
        Property CommThreadsStarted() As Boolean
        Property Connected() As Boolean
        Property IsShutDownRequested() As Boolean
        Property IsUserConnectRequested() As Boolean
        Property PortName() As String
        Property Bauds() As String
        Property AnalyzerIsReady() As Boolean
        Property AnalyzerStatus() As AnalyzerManagerStatus
        Property AnalyzerCurrentAction() As AnalyzerManagerAx00Actions
        Property InstructionSent() As String
        Property InstructionReceived() As String
        Property InstructionTypeReceived() As AnalyzerManagerSwActionList
        Property InstructionTypeSent() As AppLayerEventList
        Property ISEModuleIsReady() As Boolean
        ReadOnly Property Alarms() As List(Of AlarmEnumerates.Alarms)
        ReadOnly Property ErrorCodes() As String
        Property IsServiceAlarmInformed() As Boolean
        Property IsServiceRotorMissingInformed() As Boolean
        Property IsFwUpdateInProcess() As Boolean
        Property IsConfigGeneralProcess() As Boolean
        Property AnalyzerIsFreeze() As Boolean
        Property AnalyzerFreezeMode() As String
        Property AnalyzerHasSubStatus() As Boolean
        Property SessionFlag(ByVal pFlag As AnalyzerManagerFlags) As String
        ReadOnly Property GetSensorValue(ByVal pSensorID As AnalyzerSensors) As Single
        WriteOnly Property SetSensorValue(ByVal pSensorID As AnalyzerSensors) As Single
        Property MaxWaitTime() As Integer
        ReadOnly Property ShowBaseLineInitializationFailedMessage() As Boolean
        ReadOnly Property ShowBaseLineWellRejectionParameterFailedMessage() As Boolean
        ReadOnly Property SensorValueChanged() As UIRefreshDS.SensorValueChangedDataTable
        ReadOnly Property ValidALIGHT() As Boolean
        ReadOnly Property ExistsALIGHT() As Boolean
        ReadOnly Property ValidFLIGHT() As Boolean
        Property CurrentWell() As Integer
        Property BarCodeProcessBeforeRunning() As BarcodeWorksessionActionsEnum
        ReadOnly Property GetModelValue(ByVal pAnalyzerID As String) As String
        ReadOnly Property GetUpperPartSN(ByVal pAnalyzerID As String) As String
        ReadOnly Property GetLowerPartSN(ByVal pAnalyzerID As String) As String
        Property Ringing() As Boolean
        Property IsAutoInfoActivated() As Boolean
        Property IsAlarmInfoRequested() As Boolean
        Property IsInstructionRejected() As Boolean
        Property IsRecoverFailed() As Boolean
        Property IsInstructionAborted() As Boolean
        Property LevelDetected() As Boolean
        Property EndRunInstructionSent() As Boolean
        Property AbortInstructionSent() As Boolean
        Property RecoverInstructionSent() As Boolean
        Property PauseInstructionSent() As Boolean
        ReadOnly Property ContinueAlreadySentFlag() As Boolean
        Property InfoActivated() As Integer
        Property IsStressing() As Boolean
        Property IsFwSwCompatible() As Boolean
        Property IsFwReaded() As Boolean
        ReadOnly Property wellBaseLineAutoPausesSession() As Integer
        Property IsAnalyzerIDNotLikeWS() As Boolean
        Property StartingApplication() As Boolean
        ReadOnly Property TemporalAnalyzerConnected() As String
        Property ForceAbortSession() As Boolean
        Property DifferentWSType() As String
        Property AdjustmentsRead() As Boolean
        Property TestingCollidedNeedle() As UTILCollidedNeedles
        Property SaveSNResult() As UTILSNSavedResults
        ReadOnly Property ErrorCodesDisplay() As List(Of AnalyzerManager.ErrorCodesDisplayStruct)
        Property InfoRefreshFirstTime() As Boolean
        ReadOnly Property LastExportedResults() As ExecutionsDS
        WriteOnly Property autoWSCreationWithLISMode() As Boolean
        ReadOnly Property AllowScanInRunning() As Boolean
        Property BarcodeStartInstrExpected As Boolean
        Property FWUpdateResponseData As FWUpdateResponseTO
        Property AdjustmentsFilePath As String
        Property BaseLineTypeForCalculations As BaseLineType
        Property BaseLineTypeForWellReject As BaseLineType
        Property Model As String
        Property CurrentInstructionAction As InstructionActions
        Property FlightInitFailures As Integer
        Property DynamicBaselineInitializationFailures As Integer
        Property LockISE As Boolean
        Property ShutDownisPending As Boolean
        Property StartSessionisPending As Boolean
        Property ISECMDStateIsLost As Boolean
        Property CanSendingRepetitions As Boolean
        Property NumSendingRepetitionsTimeout As Integer
        Property WaitingStartTaskTimerEnabled As Boolean
        Property SetTimeISEOffsetFirstTime As Boolean
        ReadOnly Property ConnectedPortName As String
        ReadOnly Property ConnectedBauds As String
        Property RecoveryResultsInPause As Boolean
        Property RunningLostState As Boolean
        Property NumRepetitionsStateInstruction As Integer
        Property RunningConnectionPollSnSentStatus As Boolean
        Property IseIsAlreadyStarted() As Boolean
        ReadOnly Property BufferANSPHRReceivedCount() As Integer
        Property ProcessingLastANSPHRInstructionStatus() As Boolean
        Property StartUseRequestFlag() As Boolean
        ReadOnly Property StartTaskInstructionsQueueCount() As Integer
        Property ThermoReactionsRotorWarningTimerEnabled() As Boolean
        ReadOnly Property PauseModeIsStartingState() As Boolean
        Property PauseSendingTestPreparations() As Boolean
        ReadOnly Property BaselineInitializationFailures As Integer
        Property WELLbaselineParametersFailures As Boolean
        Property FutureRequestNextWellValue As Integer
        ReadOnly Property ReceivedAlarms() As UIRefreshDS.ReceivedAlarmsDataTable
        ReadOnly Property FieldLimits As FieldLimitsDS.tfmwFieldLimitsDataTable
        ReadOnly Property SentPreparations As AnalyzerManagerDS.sentPreparationsDataTable
        ReadOnly Property NextPreparationsToSend As AnalyzerManagerDS.nextPreparationDataTable
        Property NextPreparationsAnalyzerManagerDS As AnalyzerManagerDS
        Property wellContaminatedWithWashSent As Integer
        Property CanManageRetryAlarm As Boolean
        Property StartingRunningFirstTime As Boolean
        ReadOnly Property WashingIDRequired As Boolean
        Property AnalyzerSettings As AnalyzerSettingsDS
        Property AnalyzerSwParameters As ParametersDS
        Property Bl_listener As BaseLineListener
        'Property AnalyzerSettings As AnalyzerSettingsDS.tcfgAnalyzerSettingsDataTable
        'Property AnalyzerSwParameters As ParametersDS.tfmwSwParametersDataTable
#End Region

#Region "Events definition & methods"

        Event ReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                           ByVal pUIRefresh_Event As List(Of UI_RefreshEvents), ByVal pUI_RefreshDS As UIRefreshDS, ByVal pMainThread As Boolean)
        Event SendEvent(ByVal pInstructionSent As String)
        Event WatchDogEvent(ByVal pEnable As Boolean)

        Event ReceptionFwScriptEvent(ByVal pDataReceived As String, _
                                                   ByVal pResponseValue As String, _
                                                   ByVal pTreated As Boolean)

        Event ReceivedStatusInformationEventHandler() 'BA-2143
        Event ProcessFlagEventHandler(ByVal pFlagCode As AnalyzerManagerFlags) 'BA-2143


        'EVENT WRAPPERS
        Sub ConnectionDoneReceptionEvent()
        Sub ActionToSendEvent(Instruction As String)
        Sub RunWellBaseLineWorker()

#End Region

#Region "Public Methods"

        Function ManageAnalyzer(ByVal pAction As AnalyzerManagerSwActionList, ByVal pSendingEvent As Boolean, _
                               Optional ByVal pInstructionReceived As List(Of InstructionParameterTO) = Nothing, _
                               Optional ByVal pSwAdditionalParameters As Object = Nothing, _
                               Optional ByVal pFwScriptId As String = "", _
                               Optional ByVal pParams As List(Of String) = Nothing) As GlobalDataTO
        Sub InitializeAnalyzerFlags(ByVal pDBConnection As SqlConnection, Optional ByVal pPreviousAnalyzerID As String = "")
        Function SetSessionFlags(ByVal pFlagCode As AnalyzerManagerFlags, ByVal pNewValue As String) As GlobalDataTO
        Function ClearStartTaskQueueToSend() As GlobalDataTO
        Function SimulateInstructionReception(ByVal pInstructionReceived As String, Optional ByVal pReturnData As Object = Nothing) As GlobalDataTO
        Function SimulateSendNext(ByVal pNextWell As Integer) As GlobalDataTO
        Function SimulateAlarmsManagement(ByVal pAlarmList As List(Of Alarms), ByVal pAlarmStatusList As List(Of Boolean)) As GlobalDataTO
        Function SimulateRequestAdjustmentsFromAnalyzer(ByVal pPath As String) As GlobalDataTO
        Function ProcessArmCollisionDuringRecoverBuss(ByVal pPrepWithKO As PrepWithProblemTO) As GlobalDataTO
        Function ProcessClotDetectionDuringRecoverBuss(ByVal pPrepWithKO As PrepWithProblemTO) As GlobalDataTO
        Function ProcessLevelDetectionDuringRecoverBuss(ByVal pPrepWithKO As PrepWithProblemTO) As GlobalDataTO
        Function ProcessANSUTILReceived(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
        Function CalculateFwFileCRC32(ByVal pFileStringData As String) As GlobalDataTO
        Function CalculateFwFileCRC32(ByVal pFileBytes As Byte()) As GlobalDataTO
        Function ValidateFwSwCompatibility(ByVal pFWVersion As String, ByVal pSWVersion As String) As GlobalDataTO
        Function GetFwVersionNeeded(ByVal pSWVersion As String) As GlobalDataTO
        Function GetBottlePercentage(ByVal pCounts As Integer, ByVal pWashSolutionBottleFlag As Boolean) As GlobalDataTO
        Sub SolveErrorCodesToDisplay(ByVal pErrorCodeList As List(Of String))
        Sub AddErrorCodesToDisplay(ByVal pErrorCodesDS As AlarmErrorCodesDS)
        Function RemoveErrorCodesToDisplay() As GlobalDataTO
        Function ManageSendAndSearchNext(ByVal pNextWell As Integer) As GlobalDataTO
        Function SendNextPreparation(ByVal pNextWell As Integer, ByRef pActionSentFlag As Boolean, ByRef pEndRunSentFlag As Boolean, ByRef pSystemErrorFlag As Boolean) As GlobalDataTO
        Function MarkWashWellContaminationRunningAccepted(ByVal pDBConnection As SqlConnection) As GlobalDataTO
        Function ProcessRecivedISEResult(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
        Function RefreshISEAlarms() As GlobalDataTO
        Function ActivateAnalyzerISEAlarms(ByVal pISEResult As ISEResultTO) As GlobalDataTO
        Function BlockISEPreparationByElectrode(ByVal pDBConnection As SqlConnection, ByVal pISEResult As ISEResultTO, ByVal pWorkSessionId As String, ByVal pAnalyzerID As String) As GlobalDataTO
        Sub InitBaseLineCalculations(ByVal pDBConnection As SqlConnection, ByVal pStartingApplication As Boolean)
        Function Start(ByVal pPooling As Boolean) As Boolean
        Function SynchronizeComm() As Boolean
        Function Terminate() As Boolean
        Function StopComm() As Boolean
        Function StartComm() As Boolean
        Function ReadRegistredPorts() As GlobalDataTO
        Sub ResetWorkSession()
        Sub ReadyToClearUIRefreshDS(ByVal pMainThread As Boolean)
        Sub RemoveItemFromQueue(ByVal pAction As AnalyzerManagerSwActionList)
        Function ClearQueueToSend() As GlobalDataTO
        Function QueueContains(ByVal pInstruction As AnalyzerManagerSwActionList) As Boolean
        Function ReadFwScriptData() As GlobalDataTO
        Function LoadAppFwScriptsData() As GlobalDataTO
        Function LoadFwAdjustmentsMasterData(Optional ByVal pSimulationMode As Boolean = False) As GlobalDataTO
        Function ReadFwAdjustmentsDS() As GlobalDataTO
        Function UpdateFwAdjustmentsDS(ByVal pAdjustmentsDs As SRVAdjustmentsDS) As GlobalDataTO
        Function ReadPhotometryData() As GlobalDataTO
        Function SetPhotometryData(ByVal pPhotometryData As PhotometryDataTO) As GlobalDataTO
        Function ReadStressModeData() As GlobalDataTO
        Function SetStressModeData(ByVal pStressModeData As StressDataTO) As GlobalDataTO
        Function ReadOpticCenterData() As GlobalDataTO
        Function ProcessUSBCableDisconnection(Optional ByVal pConnectionCompleted As Boolean = False) As GlobalDataTO
        Function StopAnalyzerRinging(Optional ByVal pForceEndSound As Boolean = False) As GlobalDataTO
        Function StartAnalyzerRinging(Optional ByVal pForceSound As Boolean = False) As GlobalDataTO
        Function StopAnalyzerInfo() As GlobalDataTO
        Function ReadAdjustValue(ByVal pAdjust As Ax00Adjustsments) As String
        Function ManageBarCodeRequestBeforeRUNNING(ByVal pDbConnection As SqlConnection, ByVal pBarcodeProcessCurrentStep As BarcodeWorksessionActionsEnum) As GlobalDataTO
        Function ExistBottleAlarms() As Boolean
        Function ProcessUpdateWSByAnalyzerID(ByVal pDbConnection As SqlConnection) As GlobalDataTO
        Function ProcessToMountTheNewSession(ByVal pDbConnection As SqlConnection, ByVal pWsAnalyzerId As String) As GlobalDataTO
        Function InsertConnectedAnalyzer(ByVal pDBConnection As SqlConnection) As GlobalDataTO
        Function ReadAdjustments(ByVal pDbConnection As SqlConnection) As GlobalDataTO
        Sub ClearLastExportedResults()
        Function ExistSomeAlarmThatRequiresStopWS() As Boolean
        Function ProcessDynamicBaseLine(ByVal pDBConnection As SqlConnection, ByVal pWorkSessionID As String, ByVal pInitialWell As Integer) As GlobalDataTO
        Sub UpdateSessionFlags(ByRef pFlagsDS As AnalyzerManagerFlagsDS, ByVal pFlagCode As AnalyzerManagerFlags, ByVal pNewValue As String)
        Sub ResetBaseLineFailuresCounters()
        Sub SetAnalyzerNotReady()
        Function UpdateSensorValuesAttribute(ByVal pSensor As AnalyzerSensors, ByVal pNewValue As Single, ByVal pUIEventForChangesFlag As Boolean) As Boolean
        Function ProcessFlightReadAction() As Boolean
        Function RemoveErrorCodeAlarms(ByVal pDbConnection As SqlConnection, ByVal pAction As AnalyzerManagerAx00Actions) As GlobalDataTO
        Function ExecuteSpecialBusinessOnAnalyzerStatusChanges(ByVal pDbConnection As SqlConnection, ByVal pNewStatusValue As AnalyzerManagerStatus) As GlobalDataTO
        Sub PrepareLocalAlarmList(ByVal pAlarmCode As Alarms, ByVal pAlarmStatus As Boolean, _
                                          ByRef pAlarmList As List(Of Alarms), ByRef pAlarmStatusList As List(Of Boolean), _
                                          Optional ByVal pAddInfo As String = "", _
                                          Optional ByRef pAdditionalInfoList As List(Of String) = Nothing, _
                                          Optional ByVal pAddAlwaysFlag As Boolean = False)

        Sub InitializeTimerStartTaskControl(ByVal pInterval As Integer, Optional ByVal pNotUseOffset As Boolean = False)
        Sub InitializeTimerSTATEControl(ByVal pInterval As Integer)
        Function ManageStandByStatus(ByVal pAx00ActionCode As AnalyzerManagerAx00Actions, ByVal pNextWell As Integer) As GlobalDataTO
        Function ManageRunningStatus(ByVal pAx00ActionCode As AnalyzerManagerAx00Actions, ByVal pNextWell As Integer) As GlobalDataTO
        Sub InitializeTimerControl(ByVal pInterval As Integer)
        Function ManageInterruptedProcess(ByVal pDBConnection As SqlConnection) As GlobalDataTO
        Function SendStartTaskinQueue() As GlobalDataTO
        Sub AlarmListRemoveItem(itemToRemove As Alarms)
        Sub AlarmListClear()
        Function AlarmListAddtem(itemToAdd As Alarms) As Boolean
        Sub MyErrorCodesClear()
        Function IgnoreErrorCodes(ByVal pLastInstructionTypeSent As AppLayerEventList, ByVal pInstructionSent As String, ByVal pErrorValue As Integer) As Boolean
        Function TranslateErrorCodeToAlarmID(ByVal pDbConnection As SqlConnection, ByRef pErrorCodeList As List(Of Integer)) As List(Of Alarms)
        Sub PrepareLocalAlarmList_SRV(ByVal pErrorCodeList As List(Of Integer), ByRef pErrorCodeFinalList As List(Of String))
        Function QueueAdds(ByVal pInstruction As AnalyzerManagerSwActionList, ByVal pParamsQueue As Object) As Boolean
        Sub SetAllowScanInRunningValue(ByVal pValue As Boolean)
        Function SearchNextPreparation(ByVal pDBConnection As SqlConnection, ByVal pNextWell As Integer, Optional ByVal pLookForISEExecutionsFlag As Boolean = True) As GlobalDataTO
        Sub FillNextPreparationToSend(ByRef myGlobal As GlobalDataTO)
        Function PrepareUIRefreshEvent(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                               ByVal pExecutionID As Integer, ByVal pReadingNumber As Integer, _
                                               ByVal pAlarmID As String, ByVal pAlarmStatus As Boolean) As GlobalDataTO
        Sub ClearReceivedAlarmsFromRefreshDs()
        Function IsAlarmSoundDisable(ByVal pdbConnection As SqlConnection) As GlobalDataTO
        Function ManageAlarms_SRV(ByVal pdbConnection As SqlConnection, _
                                          ByVal pAlarmIDList As List(Of Alarms), _
                                          ByVal pAlarmStatusList As List(Of Boolean), _
                                          Optional ByVal pErrorCodeList As List(Of String) = Nothing, _
                                          Optional ByVal pAnswerErrorReception As Boolean = False) As GlobalDataTO
        Function PrepareUIRefreshEventNum3(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                               ByVal pReactionsRotorWellDS As ReactionsRotorDS, ByVal pMainThreadIsUsedFlag As Boolean) As GlobalDataTO

        Function PrepareUIRefreshEventNum2(ByVal pDBConnection As SqlConnection, ByVal pUI_EventType As UI_RefreshEvents, _
                                               ByVal pRotorType As String, ByVal pCellNumber As Integer, ByVal pStatus As String, ByVal pElementStatus As String, _
                                               ByVal pRealVolume As Single, ByVal pTestsLeft As Integer, ByVal pBarCodeInfo As String, ByVal pBarCodeStatus As String, _
                                               ByVal pSensorId As AnalyzerSensors, ByVal pSensorValue As Single, _
                                               ByVal pScannedPosition As Boolean, ByVal pElementID As Integer, ByVal pMultiTubeNumber As Integer, _
                                               ByVal pTubeType As String, ByVal pTubeContent As String) As GlobalDataTO

        Function ActivateProtocolWrapper(ByVal pEvent As AppLayerEventList, Optional ByVal pSwEntry As Object = Nothing, _
                                          Optional ByVal pFwEntry As String = "", _
                                          Optional ByVal pFwScriptID As String = "", _
                                          Optional ByVal pServiceParams As List(Of String) = Nothing) As GlobalDataTO

        Function SimpleTranslateErrorCodeToAlarmId(ByVal pDbConnection As SqlConnection, ByVal errorCode As Integer) As Alarms

        'Function ContaminationsSpecification() As IAnalyzerContaminationsSpecification


        Sub ResetFLIGHT()



#End Region

    End Interface

End Namespace


