Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.Core.Entities

Namespace Biosystems.Ax00.Core.Interfaces

    Public Interface IISEAnalyzerEntity

#Region "Properties"

        Event ISEMonitorDataChanged()
        Event ISEProcedureFinished()
        Event ISEReadyChanged()
        Event ISESwitchedOnChanged()
        Event ISEConnectionFinished(ByVal pOK As Boolean)
        Event ISEMaintenanceRequired(ByVal pOperation As ISEAnalyzerEntity.MaintenanceOperations)
        Property WorkSessionID() As String
        Property IsInUtilities() As Boolean
        Property ISEWSCancelErrorCounter() As Integer
        ReadOnly Property IsBiosystemsValidationMode() As Boolean
        Property IsISEModuleInstalled() As Boolean
        Property IsISESwitchON() As Boolean
        Property IsISECommsOk() As Boolean
        Property IsAnalyzerDisconnected() As Boolean
        Property IsAnalyzerWarmUp() As Boolean
        Property IsCommErrorDetected() As Boolean
        ReadOnly Property IsISEInitiating() As Boolean
        ReadOnly Property IsISEInitializationDone() As Boolean
        ReadOnly Property IsISEInitiatedOK() As Boolean
        Property IsPendingToInitializeAfterActivation() As Boolean
        ReadOnly Property ConnectionTasksCanContinue() As Boolean
        ReadOnly Property IsISEOnceInitiatedOK() As Boolean
        WriteOnly Property IsISEInitiatedDone() As Boolean
        ReadOnly Property IsISEModuleReady() As Boolean
        Property IsLongTermDeactivation() As Boolean
        ReadOnly Property IsReagentsPackReady() As Boolean
        ReadOnly Property IsReagentsPackSerialNumberMatch() As Boolean
        Property IsElectrodesReady() As Boolean
        ReadOnly Property MonitorDataTO() As ISEMonitorTO
        Property CurrentProcedure() As ISEAnalyzerEntity.ISEProcedures
        ReadOnly Property CurrentProcedureIsFinished() As Boolean
        Property CurrentCommandTO() As ISECommandTO
        Property LastISEResult() As ISEResultTO
        ReadOnly Property LastProcedureResult() As ISEAnalyzerEntity.ISEProcedureResult
        ReadOnly Property IsCalAUpdateRequired() As Boolean
        ReadOnly Property IsCalBUpdateRequired() As Boolean
        Property PurgeAbyFirmware() As Integer
        Property PurgeBbyFirmware() As Integer
        Property WorkSessionOverallTime() As Single
        Property WorkSessionIsRunning() As Boolean
        Property WorkSessionTestsByType() As String
        Property IsCalibrationNeeded() As Boolean
        Property IsPumpCalibrationNeeded() As Boolean
        Property IsBubbleCalibrationNeeded() As Boolean
        Property IsCleanNeeded() As Boolean
        ReadOnly Property IsReplaceNaRecommended() As Boolean
        ReadOnly Property IsReplaceKRecommended() As Boolean
        ReadOnly Property IsReplaceClRecommended() As Boolean
        ReadOnly Property IsReplaceRefRecommended() As Boolean
        ReadOnly Property IsReplaceFluidicTubingRecommended() As Boolean
        ReadOnly Property IsReplacePumpTubingRecommended() As Boolean
        Property LastElectrodesCalibrationResult1() As String
        Property LastElectrodesCalibrationResult2() As String
        Property LastElectrodesCalibrationError() As String
        Property LastPumpsCalibrationResult() As String
        Property LastPumpsCalibrationError() As String
        Property LastElectrodesCalibrationDate() As DateTime
        Property LastPumpsCalibrationDate() As DateTime
        Property LastBubbleCalibrationResult() As String
        Property LastBubbleCalibrationError() As String
        Property LastBubbleCalibrationDate() As DateTime
        Property LastCleanDate() As DateTime
        Property LastCleanError() As String
        Property ReagentsPackInstallationDate() As DateTime
        Property IsCleanPackInstalled() As Boolean
        Property LiInstallDate() As DateTime
        Property NaInstallDate() As DateTime
        Property KInstallDate() As DateTime
        Property ClInstallDate() As DateTime
        Property RefInstallDate() As DateTime
        ReadOnly Property HasLiInstallDate() As Boolean
        ReadOnly Property HasNaInstallDate() As Boolean
        ReadOnly Property HasKInstallDate() As Boolean
        ReadOnly Property HasClInstallDate() As Boolean
        ReadOnly Property HasRefInstallDate() As Boolean
        Property IsLiEnabledByUser() As Boolean
        Property PumpTubingInstallDate() As DateTime
        Property FluidicTubingInstallDate() As DateTime
        ReadOnly Property HasPumpTubingInstallDate() As Boolean
        ReadOnly Property HasFluidicTubingInstallDate() As Boolean
        ReadOnly Property IsWaitingForInstructionStart() As Boolean

#End Region

#Region "Public Methods"

        Function AbortCurrentProcedureDueToException() As GlobalDataTO
        Function RefreshAllDatabaseInformation() As GlobalDataTO
        Function RefreshMonitorDataTO() As GlobalDataTO
        Function GetISEAlarmsForUtilities(ByRef pPendingCalibrations As List(Of ISEAnalyzerEntity.MaintenanceOperations)) As GlobalDataTO
        Function UpdateISEInfoSetting(ByVal pISESettingID As GlobalEnumerates.ISEModuleSettings, ByVal pValue As Object, Optional ByVal pIsDatetime As Boolean = False) As GlobalDataTO
        Function ValidateElectrodesCalibration(ByVal pResultStr As String, Optional ByVal pForcedLiEnabledValue As TriState = TriState.UseDefault) As GlobalDataTO
        Function CheckAnyCalibrationIsNeeded(ByRef pAffectedElectrodes As List(Of String)) As GlobalDataTO
        Function CheckElectrodesCalibrationIsNeeded() As GlobalDataTO
        Function CheckBubbleCalibrationIsNeeded() As GlobalDataTO
        Function CheckPumpsCalibrationIsNeeded() As GlobalDataTO
        Function CheckCleanIsNeeded() As GlobalDataTO
        Function GetConsumptionParameters() As GlobalDataTO
        Function SaveConsumptionCalToDallasData(ByVal pCalibrator As String) As GlobalDataTO
        Function EstimatedFWConsumptionWS() As Long
        Function SendISECommand() As GlobalDataTO
        Function MoveR2ArmAway() As GlobalDataTO
        Function MoveR2ArmBack() As GlobalDataTO
        Function SetElectrodesInstallDates(ByVal pRef As DateTime, ByVal pNa As DateTime, ByVal pK As DateTime, ByVal pCl As DateTime, ByVal pLi As DateTime, Optional ByVal pSimulationMode As Boolean = False) As GlobalDataTO
        Function SetReagentsPackInstallDate(ByVal pdate As DateTime, Optional ByVal pResetConsumptions As Boolean = False) As GlobalDataTO
        Function SetPumpTubingInstallDate(ByVal pDate As DateTime) As GlobalDataTO
        Function SetFluidicTubingInstallDate(ByVal pDate As DateTime) As GlobalDataTO
        Function InstallISEModule() As GlobalDataTO
        Function DeactivateISEModule(ByVal pDeactivationMode As Boolean) As GlobalDataTO
        Function ActivateReagentsPack() As GlobalDataTO
        Function ActivateElectrodes() As GlobalDataTO
        Function ActivateISEPreparations() As GlobalDataTO
        Function DoPrimeAndCalibration() As GlobalDataTO
        Function DoPrimeX2AndCalibration() As GlobalDataTO
        Function DoGeneralCheckings(Optional ByVal pIsRPAlreadyRead As Boolean = False) As GlobalDataTO
        Function DoCleaning(ByVal pTubePos As Integer) As GlobalDataTO
        Function DoMaintenanceExit() As GlobalDataTO
        Function DoPurge(ByVal pType As String) As GlobalDataTO
        Function CheckCleanPackInstalled() As GlobalDataTO
        Function CheckAlarms(ByVal pConnectedAttribute As Boolean, ByRef pAlarmList As List(Of GlobalEnumerates.Alarms), ByRef pAlarmStatusList As List(Of Boolean)) As GlobalDataTO
        Function SaveConsumptions() As GlobalDataTO
        Function DoElectrodesCalibration() As GlobalDataTO
        Function DoPumpsCalibration() As GlobalDataTO
        Function DoBubblesCalibration() As GlobalDataTO
        Function GetISEErrorDescription(ByVal pISEError As ISEErrorTO, Optional ByVal pIsUrine As Boolean = False, Optional ByVal pIsCalibration As Boolean = False) As String
        Function UpdateAnalyzerInformation(ByVal pAnalyzerID As String, ByVal pAnalyzerModel As String) As GlobalDataTO
        Function StartInstructionStartedTimer(Optional ByVal pTime As Integer = 0) As GlobalDataTO
        Sub StopInstructionStartedTimer()
        Function PrepareDataToSend(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pApplicationName As String, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pISEMode As ISEModes, ByVal pCmd As ISECommands, Optional ByVal pParameter1 As String = "0", Optional ByVal pParameter2 As String = "0", Optional ByVal pParameter3 As String = "0", Optional ByVal pTubePosition As Integer = 1) As GlobalDataTO
        Function PrepareDataToSend_POLL() As GlobalDataTO
        Function PrepareDataToSend_CALB() As GlobalDataTO
        Function PrepareDataToSend_CLEAN(ByVal pTubePosition As Integer, ByVal pAnalyzerID As String) As GlobalDataTO
        Function PrepareDataToSend_PUMP_CAL(ByVal pTubePosition As Integer, ByVal pAnalyzerID As String) As GlobalDataTO
        Function PrepareDataToSend_START() As GlobalDataTO
        Function PrepareDataToSend_PURGEA() As GlobalDataTO
        Function PrepareDataToSend_PURGEB() As GlobalDataTO
        Function PrepareDataToSend_BUBBLE_CAL() As GlobalDataTO
        Function PrepareDataToSend_SHOW_BUBBLE_CAL() As GlobalDataTO
        Function PrepareDataToSend_SHOW_PUMP_CAL() As GlobalDataTO
        Function PrepareDataToSend_DSPA(ByVal pVolume As String) As GlobalDataTO
        Function PrepareDataToSend_DSPB(ByVal pVolume As String) As GlobalDataTO
        Function PrepareDataToSend_VERSION_CHECKSUM() As GlobalDataTO
        Function PrepareDataToSend_READ_mV() As GlobalDataTO
        Function PrepareDataToSend_LAST_SLOPES() As GlobalDataTO
        Function PrepareDataToSend_DEBUG_mV(ByVal pDebugMode As ISECommands) As GlobalDataTO
        Function PrepareDataToSend_MAINTENANCE() As GlobalDataTO
        Function PrepareDataToSend_PRIME_CALA() As GlobalDataTO
        Function PrepareDataToSend_PRIME_CALB() As GlobalDataTO
        Function PrepareDataToSend_READ_PAGE_DALLAS(ByVal pPage As Integer) As GlobalDataTO
        Function PrepareDataToSend_WRITE_DALLAS(ByVal pPosition As String, ByVal pInfo As String) As GlobalDataTO
        Function PrepareDataToSend_WRITE_INSTALL_DAY(ByVal pDay As Integer) As GlobalDataTO
        Function PrepareDataToSend_WRITE_INSTALL_MONTH(ByVal pMonth As Integer) As GlobalDataTO
        Function PrepareDataToSend_WRITE_INSTALL_YEAR(ByVal pYear As Integer) As GlobalDataTO
        Function PrepareDataToSend_R2_TO_WASH() As GlobalDataTO
        Function PrepareDataToSend_R2_TO_PARK() As GlobalDataTO
        Sub Initialize()

#End Region

    End Interface

End Namespace
