Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Core.Entities
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.Core.Interfaces

    Public Interface IAnalyzerEntity

#Region "Properties"

        Property BaseLine As IBaseLineEntity
        Property ISEAnalyzer As IISEAnalyzerEntity

        Property classInitializationError() As Boolean '15/07/2011 AG
        Property ActiveWorkSession() As String '19/04/2010 AG
        ReadOnly Property WorkSessionStatus() As String ' XBC 14/06/2012
        Property ActiveAnalyzer() As String '19/04/2010 AG
        Property ActiveFwVersion() As String 'SGM 28/11/2011    
        Property ReadedFwVersion() As String 'SGM 28/11/2011
        Property CommThreadsStarted() As Boolean '21/04/2010 AG
        ReadOnly Property Connected() As Boolean '22/04/2010 AG
        Property IsShutDownRequested() As Boolean
        Property IsUserConnectRequested() As Boolean
        Property PortName() As String '22/04/2010 AG
        Property Bauds() As String '22/04/2010 AG
        Property AnalyzerIsReady() As Boolean '19/05/2010 AG
        Property AnalyzerStatus() As GlobalEnumerates.AnalyzerManagerStatus
        Property AnalyzerCurrentAction() As GlobalEnumerates.AnalyzerManagerAx00Actions 'AG 01/06/2010
        Property InstructionSent() As String '20/10/2010 AG
        Property InstructionReceived() As String '20/10/2010 AG
        Property InstructionTypeReceived() As GlobalEnumerates.AnalyzerManagerSwActionList '02/11/2010 AG
        ReadOnly Property InstructionTypeSent() As GlobalEnumerates.AppLayerEventList '27/03/2012 AG
        Property ISEModuleIsReady() As Boolean '18/01/2011 AG
        ReadOnly Property Alarms() As List(Of GlobalEnumerates.Alarms)
        ReadOnly Property ErrorCodes() As String
        Property IsServiceAlarmInformed() As Boolean
        Property IsServiceRotorMissingInformed() As Boolean
        Property IsFwUpdateInProcess() As Boolean
        Property IsConfigGeneralProcess() As Boolean
        ReadOnly Property AnalyzerIsFreeze() As Boolean
        ReadOnly Property AnalyzerFreezeMode() As String
        Property SessionFlag(ByVal pFlag As GlobalEnumerates.AnalyzerManagerFlags) As String '19/04/2010 AG
        ReadOnly Property GetSensorValue(ByVal pSensorID As GlobalEnumerates.AnalyzerSensors) As Single
        WriteOnly Property SetSensorValue(ByVal pSensorID As GlobalEnumerates.AnalyzerSensors) As Single
        ReadOnly Property MaxWaitTime() As Integer
        ReadOnly Property ShowBaseLineInitializationFailedMessage() As Boolean
        ReadOnly Property ShowBaseLineParameterFailedMessage() As Boolean
        ReadOnly Property SensorValueChanged() As UIRefreshDS.SensorValueChangedDataTable
        ReadOnly Property ValidALIGHT() As Boolean '20/05/2011 AG
        ReadOnly Property ExistsALIGHT() As Boolean '20/06/2012 AG
        ReadOnly Property CurrentWell() As Integer
        Property BarCodeProcessBeforeRunning() As AnalyzerEntity.BarcodeWorksessionActions 'AG 04/08/2011
        ReadOnly Property GetModelValue(ByVal pAnalyzerID As String) As String
        ReadOnly Property GetUpperPartSN(ByVal pAnalyzerID As String) As String
        ReadOnly Property GetLowerPartSN(ByVal pAnalyzerID As String) As String
        ReadOnly Property Ringing() As Boolean '26/10/2011 AG
        Property IsAutoInfoActivated() As Boolean
        Property IsAlarmInfoRequested() As Boolean
        Property IsInstructionRejected() As Boolean
        Property IsRecoverFailed() As Boolean
        Property IsInstructionAborted() As Boolean
        Property LevelDetected() As Boolean
        ReadOnly Property EndRunInstructionSent() As Boolean
        ReadOnly Property AbortInstructionSent() As Boolean
        ReadOnly Property RecoverInstructionSent() As Boolean
        ReadOnly Property PauseInstructionSent() As Boolean
        ReadOnly Property ContinueAlreadySentFlag() As Boolean
        Property InfoActivated() As Integer 'AG 14/03/2012
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
        ReadOnly Property ErrorCodesDisplay() As List(Of AnalyzerEntity.ErrorCodesDisplayStruct)
        Property InfoRefreshFirstTime() As Boolean
        ReadOnly Property LastExportedResults() As ExecutionsDS
        WriteOnly Property autoWSCreationWithLISMode() As Boolean 'AG 11/07/2013
        ReadOnly Property AllowScanInRunning() As Boolean
        Property BarcodeStartInstrExpected As Boolean

#End Region

#Region "Events definition & methods"

        Event ReceptionEvent(ByVal pInstructionReceived As String, ByVal pTreated As Boolean, _
                                           ByVal pUIRefresh_Event As List(Of GlobalEnumerates.UI_RefreshEvents), ByVal pUI_RefreshDS As UIRefreshDS, ByVal pMainThread As Boolean)
        Event SendEvent(ByVal pInstructionSent As String)
        Event WatchDogEvent(ByVal pEnable As Boolean)

#End Region

#Region "Public Methods"

        Function ManageAnalyzer(ByVal pAction As GlobalEnumerates.AnalyzerManagerSwActionList, ByVal pSendingEvent As Boolean, _
                               Optional ByVal pInstructionReceived As List(Of InstructionParameterTO) = Nothing, _
                               Optional ByVal pSwAdditionalParameters As Object = Nothing, _
                               Optional ByVal pFwScriptID As String = "", _
                               Optional ByVal pParams As List(Of String) = Nothing) As GlobalDataTO
        Sub InitializeAnalyzerFlags(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pPreviousAnalyzerID As String = "")
        Function SetSessionFlags(ByVal pFlagCode As GlobalEnumerates.AnalyzerManagerFlags, ByVal pNewValue As String) As GlobalDataTO
        Function ClearStartTaskQueueToSend() As GlobalDataTO
        Function SimulateInstructionReception(ByVal pInstructionReceived As String, Optional ByVal pReturnData As Object = Nothing) As GlobalDataTO
        Function SimulateSendNext(ByVal pNextWell As Integer) As GlobalDataTO
        Function SimulateAlarmsManagement(ByVal pAlarmList As List(Of GlobalEnumerates.Alarms), ByVal pAlarmStatusList As List(Of Boolean)) As GlobalDataTO
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
        Function MarkWashWellContaminationRunningAccepted(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Function ProcessRecivedISEResult(ByVal pInstructionReceived As List(Of InstructionParameterTO)) As GlobalDataTO
        Function RefreshISEAlarms() As GlobalDataTO
        Function ActivateAnalyzerISEAlarms(ByVal pISEResult As ISEResultTO) As GlobalDataTO
        Function BlockISEPreparationByElectrode(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISEResult As ISEResultTO, ByVal pWorkSessionId As String, ByVal pAnalyzerID As String) As GlobalDataTO
        Sub InitBaseLineCalculations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pStartingApplication As Boolean)
        Function Start(ByVal pPooling As Boolean) As Boolean
        Function SynchronizeComm() As Boolean
        Function Terminate() As Boolean
        Function StopComm() As Boolean
        Function StartComm() As Boolean
        Function ReadRegistredPorts() As GlobalDataTO
        Sub ResetWorkSession()
        Sub ReadyToClearUIRefreshDS(ByVal pMainThread As Boolean)
        Sub RemoveItemFromQueue(ByVal pAction As GlobalEnumerates.AnalyzerManagerSwActionList)
        Function ClearQueueToSend() As GlobalDataTO
        Function QueueContains(ByVal pInstruction As GlobalEnumerates.AnalyzerManagerSwActionList) As Boolean
        Function ReadFwScriptData() As GlobalDataTO
        Function LoadAppFwScriptsData() As GlobalDataTO
        Function LoadFwAdjustmentsMasterData(Optional ByVal pSimulationMode As Boolean = False) As GlobalDataTO
        Function ReadFwAdjustmentsDS() As GlobalDataTO
        Function UpdateFwAdjustmentsDS(ByVal pAdjustmentsDS As SRVAdjustmentsDS) As GlobalDataTO
        Function ReadPhotometryData() As GlobalDataTO
        Function SetPhotometryData(ByVal pPhotometryData As PhotometryDataTO) As GlobalDataTO
        Function ReadStressModeData() As GlobalDataTO
        Function SetStressModeData(ByVal pStressModeData As StressDataTO) As GlobalDataTO
        Function ReadOpticCenterData() As GlobalDataTO
        Function ProcessUSBCableDisconnection(Optional ByVal pConnectionCompleted As Boolean = False) As GlobalDataTO
        Function StopAnalyzerRinging(Optional ByVal pForceEndSound As Boolean = False) As GlobalDataTO
        Function StartAnalyzerRinging(Optional ByVal pForceSound As Boolean = False) As GlobalDataTO
        Function StopAnalyzerInfo() As GlobalDataTO
        Function ReadAdjustValue(ByVal pAdjust As GlobalEnumerates.Ax00Adjustsments) As String
        Function ManageBarCodeRequestBeforeRUNNING(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBarcodeProcessCurrentStep As AnalyzerEntity.BarcodeWorksessionActions) As GlobalDataTO
        Function ExistBottleAlarms() As Boolean
        Function ProcessUpdateWSByAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Function ProcessToMountTheNewSession(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWSAnalyzerID As String) As GlobalDataTO
        Function InsertConnectedAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Function ReadAdjustments(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Sub ClearLastExportedResults()
        Function ExistSomeAlarmThatRequiresStopWS() As Boolean

#End Region

    End Interface

End Namespace


