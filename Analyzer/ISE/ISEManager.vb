Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports Biosystems.Ax00.InfoAnalyzer
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports Biosystems.Ax00.Core.Interfaces

Namespace Biosystems.Ax00.Core.Entities

    Public Class ISEManager
        Implements IISEManager

#Region "Constructor"

        Public Sub New(ByRef pAnalyzer As IAnalyzerManager, ByVal pAnalyzerID As String, ByVal pAnalyzerModel As String, Optional ByVal pDisconnectedMode As Boolean = False)
            Try
                MyClass.IsAnalyzerDisconnectedAttr = pDisconnectedMode
                MyClass.myAnalyzer = pAnalyzer
                MyClass.AnalyzerIDAttr = pAnalyzerID
                MyClass.AnalyzerModelAttr = pAnalyzerModel

                Dim myGlobal As New GlobalDataTO
                myGlobal = MyClass.RefreshAllDatabaseInformation

                If myGlobal.HasError Then
                    Throw New Exception(myGlobal.ErrorMessage)
                End If

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region

#Region "Destructor"

        'Protected disposed As Boolean = False

        'Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        '    If Not Me.disposed Then
        '        If disposing Then
        '            ' Insert code to free managed resources.
        '        End If
        '        ' Insert code to free unmanaged resources.
        '    End If
        '    Me.disposed = True
        'End Sub

        'Public Sub Dispose() Implements IDisposable.Dispose
        '    Dispose(True)
        '    GC.SuppressFinalize(Me)
        'End Sub

        'Protected Overrides Sub Finalize()
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub
#End Region

#Region "Constants"
        'SGM 06/06/2012
        Private Const ISEInfoDateFormat As String = "dd/MM/yyyy HH:mm:ss"
#End Region

#Region "Enumerates"

        'available ISE procedures consisting of one or more ISE commands
        Public Enum ISEProcedures
            None
            Test
            SingleCommand
            SingleReadCommand
            ActivateModule
            ActivateReagentsPack
            ActivateElectrodes
            GeneralCheckings
            CheckReagentsPack
            CalibrateElectrodes
            CalibratePumps
            CalibrateBubbles ' XBC 25/07/2012 
            Clean
            MaintenanceExit
            CheckCleanPackInstalled
            WriteConsumption
            PrimeAndCalibration
            PrimeX2AndCalibration

        End Enum

        'possible results for all ISE procedures
        Public Enum ISEProcedureResult
            None
            OK
            NOK
            CancelError
            Exception
        End Enum

        Public Enum MaintenanceOperations
            None
            ElectrodesCalibration
            PumpsCalibration
            BubbleCalibration
            Clean
        End Enum

        'ISE related alarms
        Public Enum Alarms
            ReagentsPack_Invalid = 0
            ReagentsPack_Depleted = 1
            ReagentsPack_Expired = 2
            Electrodes_Wrong = 3
            Electrodes_Cons_Expired = 4
            Electrodes_Date_Expired = 5
            CleanPack_Installed = 6
            CleanPack_Wrong = 7
            LongTermDeactivated = 8
            ReagentsPack_DateInstall = 9
            Switch_Off = 10
            Timeout = 11        ' XB 04/11/2014 - BA-1872
        End Enum

#End Region

#Region "Declarations"
        Private myAnalyzer As IAnalyzerManager 'Instance of Analyzer Manager class that creates ISE manager 'REFACTORING
        Private myISEInfoDelegate As New ISEDelegate 'Delegate for accessing tinfoISE table
        Private myISECalibHistory As New ISECalibHistoryDelegate 'Delegate for calibration history 'JB 30/07/2012
        Private myISEInfoDS As ISEInformationDS 'Data obtained from tinfoISE table
        Private myISESwParametersDS As ParametersDS 'Data obtained from SwParameters
        Private myISELimitsDS As FieldLimitsDS 'Data obtained from SwLimits
        ' XBC 26/03/2012
        Private myAlarms() As Boolean 'array of current ISE alarms

        ' XBC 02/07/2012
        Private SIPcycles As Single
        Private SIPIntervalConsumption As Single     ' interval of time that a SIP cycle is consumed (in minutes)
        Private Interval1forPurgeACompletedbyFW As Single
        Private Interval2forPurgeACompletedbyFW As Single
        Private ForceConsumptionsInitialize As Boolean = False
        Private ISE_EXECUTION_TIME_SER As Single = 0
        Private ISE_EXECUTION_TIME_URI As Single = 0


#End Region

#Region "Public Events"

        Public Event ISEMonitorDataChanged() Implements IISEManager.ISEMonitorDataChanged 'occurs when some change happens in Monitor Data
        Public Event ISEProcedureFinished() Implements IISEManager.ISEProcedureFinished 'occurs when some ISE procedure has finished
        Public Event ISEReadyChanged() Implements IISEManager.ISEReadyChanged 'occurs when IsISEModuleReady property has changed
        Public Event ISESwitchedOnChanged() Implements IISEManager.ISESwitchedOnChanged 'occurs when IsISESwitchOn property has changed
        Public Event ISEConnectionFinished(ByVal pOK As Boolean) Implements IISEManager.ISEConnectionFinished 'occurs when Initialization has finished
        Public Event ISEMaintenanceRequired(ByVal pOperation As MaintenanceOperations) Implements IISEManager.ISEMaintenanceRequired 'occurs when maintenance (calibrations and cleaning) operations are required
#End Region

#Region "Attributes"

        'Analyzer
        Private IsAnalyzerDisconnectedAttr As Boolean = False 'flag for informing that the Application is in disconnected mode
        Private IsAnalyzerWarmUpAttr As Boolean = False
        Private AnalyzerIDAttr As String = ""
        Private AnalyzerModelAttr As String = ""
        ' XBC 20/07/2012
        Private WorkSessionIDAttr As String = ""

        'Procedures & results management
        Private CurrentProcedureAttr As ISEProcedures = ISEProcedures.None
        Private CurrentCommandTOAttr As ISECommandTO = Nothing
        Private LastISEResultAttr As ISEResultTO = Nothing
        Private LastProcedureResultAttr As ISEProcedureResult = ISEProcedureResult.None
        ' XBC 27/08/2012 - Correction : Save consumptions after Current Procedure is finished
        Private CurrentProcedureIsFinishedAttr As Boolean

        'Private ISETestsCountForCleanAttr As Integer = 0

        'Monitor
        Private MonitorDataTOAttr As ISEMonitorTO = Nothing

        'Status
        Private IsISEModuleInstalledAttr As Boolean = False
        Private IsISESwitchONAttr As Boolean = False
        Private IsISEInitializationDoneAttr As Boolean = False
        Private IsISEInitiatedOKAttr As Boolean = False
        Private IsISEOnceInitiatedOKAttr As Boolean = False
        Private IsISECommsOkAttr As Boolean = False
        Private IsISEModuleReadyAttr As Boolean = False
        Private IsLongTermDeactivationAttr As Boolean = False
        Private isISEStatusUnknownAttr As Boolean = True 'AG 02/04/2012 True: When object instanced but still no information received
        '                                                               False: Ise not installed
        '                                                               Ise installed and the 1st ANSINF instruction received and the field 'IS' treated


        'Reagents Pack
        Private ISEDallasSNAttr As ISEDallasSNTO 'SGM 05/06/2012
        Private ISEDallasPage00Attr As ISEDallasPage00TO
        Private ISEDallasPage01Attr As ISEDallasPage01TO
        Private ReagentsPackInstallationDateAttr As DateTime
        Private IsReagentsPackInstalledAttr As Boolean = False
        Private ReagentsPackExpirationDateAttr As DateTime
        Private BiosystemsCodeAttr As String
        Private ReagentsPackInitialVolCalAAttr As Single
        Private ReagentsPackInitialVolCalBAttr As Single
        Private IsReagentsPackReadyAttr As Boolean = False
        Private IsCleanPackInstalledAttr As Boolean = False
        Private IsEnoughCalibratorAAttr As Boolean = False
        Private IsEnoughCalibratorBAttr As Boolean = False
        Private IsCalAUpdateRequiredAttr As Boolean = False
        Private IsCalBUpdateRequiredAttr As Boolean = False

        'Electrodes
        Private IsElectrodesReadyAttr As Boolean = False
        Private IsLiEnabledByUserAttr As Boolean = True

        Private IsLiMountedAttr As Boolean = False
        Private IsNaMountedAttr As Boolean = False
        Private IsKMountedAttr As Boolean = False
        Private IsClMountedAttr As Boolean = False

        Private IsRefExpiredAttr As Boolean = False
        Private IsLiExpiredAttr As Boolean = False
        Private IsNaExpiredAttr As Boolean = False
        Private IsKExpiredAttr As Boolean = False
        Private IsClExpiredAttr As Boolean = False

        Private IsRefOverUsedAttr As Boolean = False
        Private IsLiOverUsedAttr As Boolean = False
        Private IsNaOverUsedAttr As Boolean = False
        Private IsKOverUsedAttr As Boolean = False
        Private IsClOverUsedAttr As Boolean = False

        Private RefInstallDateAttr As DateTime = Nothing
        Private LiInstallDateAttr As DateTime = Nothing
        Private NaInstallDateAttr As DateTime = Nothing
        Private KInstallDateAttr As DateTime = Nothing
        Private ClInstallDateAttr As DateTime = Nothing

        Private RefTestCountAttr As Integer = -1
        Private LiTestCountAttr As Integer = -1
        Private NaTestCountAttr As Integer = -1
        Private KTestCountAttr As Integer = -1
        Private ClTestCountAttr As Integer = -1

        Private PumpTubingInstallDateAttr As DateTime = Nothing
        Private FluidicTubingInstallDateAttr As DateTime = Nothing
        Private PumpTubingExpireDateAttr As DateTime = Nothing
        Private FluidicTubingExpireDateAttr As DateTime = Nothing

        'recommended Procedures
        Private IsCalibrationNeededAttr As Boolean = False
        Private IsPumpCalibrationNeededAttr As Boolean = False
        Private IsBubbleCalibrationNeededAttr As Boolean = False
        Private IsCleanNeededAttr As Boolean = False


        'last calibrations and clean
        Private LastElectrodesCalibrationResult1Attr As String = ""
        Private LastElectrodesCalibrationResult2Attr As String = ""
        Private LastElectrodesCalibrationDateAttr As DateTime = Nothing
        Private LastElectrodesCalibrationErrorAttr As String = "" 'JB 02/08/2012
        Private LastPumpsCalibrationResultAttr As String = ""
        Private LastPumpsCalibrationDateAttr As DateTime = Nothing
        Private LastPumpsCalibrationErrorAttr As String = "" 'JB 02/08/2012
        Private LastBubbleCalibrationResultAttr As String = ""
        Private LastBubbleCalibrationDateAttr As DateTime = Nothing
        Private LastBubbleCalibrationErrorAttr As String = "" 'JB 02/08/2012
        Private LastCleanDateAttr As DateTime = Nothing
        Private LastCleanErrorAttr As String = "" 'JB 02/08/2012
        Private TestsCountSinceLastCleanAttr As Integer = -1


        'Consumptions
        Private ConsumptionCalAbySerumAttr As Single
        Private ConsumptionCalBbySerumAttr As Single
        Private ConsumptionCalAbyUrine1Attr As Single
        Private ConsumptionCalBbyUrine1Attr As Single
        Private ConsumptionCalAbyUrine2Attr As Single
        Private ConsumptionCalBbyUrine2Attr As Single
        Private ConsumptionCalAbyElectrodesCalAttr As Single
        Private ConsumptionCalBbyElectrodesCalAttr As Single
        Private ConsumptionCalAbyPumpsCalAttr As Single
        Private ConsumptionCalBbyPumpsCalAttr As Single
        Private ConsumptionCalAbyBubblesCalAttr As Single
        Private ConsumptionCalBbyBubblesCalAttr As Single
        Private ConsumptionCalAbyCleanCycleAttr As Single
        Private ConsumptionCalBbyCleanCycleAttr As Single
        Private ConsumptionCalAbyPurgeAAttr As Single
        Private ConsumptionCalBbyPurgeAAttr As Single
        Private ConsumptionCalAbyPurgeBAttr As Single
        Private ConsumptionCalBbyPurgeBAttr As Single
        Private ConsumptionCalAbyPrimeAAttr As Single
        Private ConsumptionCalBbyPrimeAAttr As Single
        Private ConsumptionCalAbyPrimeBAttr As Single
        Private ConsumptionCalBbyPrimeBAttr As Single
        Private ConsumptionCalAbySippingAttr As Single
        Private ConsumptionCalBbySippingAttr As Single

        Private Const CteVolumeToSaveDallasData As Single = 1
        Private MinConsumptionVolToSaveDallasData_CalA As Single
        Private MinConsumptionVolToSaveDallasData_CalB As Single
        Private CountConsumptionToSaveDallasData_CalA As Single
        Private CountConsumptionToSaveDallasData_CalB As Single

        ' XBC 17/07/2012
        Private PurgeAbyFirmwareAttr As Integer
        Private PurgeBbyFirmwareAttr As Integer
        Private WorkSessionOverallTimeAttr As Single
        Private WorkSessionIsRunningAttr As Boolean
        Private WorkSessionTestsByTypeAttr As String

        ' XBC 02/07/2012 - NO V1
        Private IsReagentsPackSerialNumberMatchAttr As Boolean

        'SGM 30/07/2012 - NO V1
        Private IsInUtilitiesAttr As Boolean = False

        'SGM 01/08/2012
        Private ISEWSCancelErrorCounterAttr As Integer

        ' XB 04/11/2014 - BA-1872
        Private IsTimeoutAttr As Boolean
        Private FirmwareErrDetectedAttr As Boolean
#End Region

#Region "Properties"

#Region "Generic"
        Public Property WorkSessionID() As String Implements IISEManager.WorkSessionID
            Get
                Return MyClass.WorkSessionIDAttr
            End Get
            Set(ByVal value As String)
                MyClass.WorkSessionIDAttr = value
            End Set
        End Property

        Public Property IsInUtilities() As Boolean Implements IISEManager.IsInUtilities
            Get
                Return IsInUtilitiesAttr
            End Get
            Set(ByVal value As Boolean)
                If IsInUtilitiesAttr <> value Then
                    IsInUtilitiesAttr = value
                End If
            End Set
        End Property

        'Counts the times that an ERC (A,B,S,F) error occured while WS
        Public Property ISEWSCancelErrorCounter() As Integer Implements IISEManager.ISEWSCancelErrorCounter
            Get
                Return ISEWSCancelErrorCounterAttr
            End Get
            Set(ByVal value As Integer)
                ISEWSCancelErrorCounterAttr = value

            End Set
        End Property
#End Region

#Region "Module Status"

        ''' <summary>
        ''' Determines if Biosystems validation algorithm is applied
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 28/06/2012</remarks>
        Public ReadOnly Property IsBiosystemsValidationMode() As Boolean Implements IISEManager.IsBiosystemsValidationMode
            Get
                Dim IsBioMode As Boolean = False
                Dim myGlobal As New GlobalDataTO
                myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_SECURITY_MODE, False)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsBioMode = CBool(myGlobal.SetDatos)
                End If
                Return IsBioMode
            End Get
        End Property
        ''' <summary>
        ''' Informs if the ISE Module functionality is available after checking the corresponding firmware adjustment
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public Property IsISEModuleInstalled() As Boolean Implements IISEManager.IsISEModuleInstalled
            Get
                'Dim myISEAdj As String = MyClass.myAnalyzerManager.ReadAdjustValue(Ax00Adjustsments.ISEINS)
                'MyClass.IsISEModuleInstalledAttr = (IsNumeric(myISEAdj) AndAlso CInt(myISEAdj) > 0)
                Return IsISEModuleInstalledAttr
            End Get
            Set(ByVal value As Boolean)
                If Not value Then 'AG 22/06/2012 - if not installed the status is known (OFF)
                    isISEStatusUnknownAttr = False
                End If

                If IsISEModuleInstalledAttr <> value Then
                    IsISEModuleInstalledAttr = value

                    If Not value Then 'if not installed the status is known. If installed the status will be know with the next Ansinf received
                        isISEStatusUnknownAttr = False
                    Else
                        MyClass.RefreshMonitorDataTO() 'SGM 03/05/2012
                    End If

                    RaiseEvent ISEMonitorDataChanged() 'SGM 18/06/2012

                End If
            End Set
        End Property

        ''' <summary>
        ''' Informs if the ISE Module's switch is On
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 12/03/2012
        ''' Modified by XB 30/09/2014 - Deactivate old timeout management - Remove too restrictive limitations because timeouts - BA-1872
        '''             XB 12/12/2014 - When ISE is Off set IsISEInitiatedOKAttr attribute to FALSE because the correct inicialization is required again - BA-2178
        ''' </remarks>
        Public Property IsISESwitchON() As Boolean Implements IISEManager.IsISESwitchON
            Get
                If GlobalConstants.REAL_DEVELOPMENT_MODE > 0 Then
                    Return True
                Else
                    Return IsISESwitchONAttr
                End If
            End Get
            Set(ByVal value As Boolean)

                ' XBC 03/10/2012 - Correction : When ISE is switch off set Ise as initialized
                If Not value Then
                    MyClass.IsISEInitializationDoneAttr = True
                    ' XB 12/12/2014 - BA-2178
                    MyClass.IsISEInitiatedOKAttr = False
                    MyClass.IsISEOnceInitiatedOKAttr = False
                    ' XB 12/12/2014 - BA-2178
                End If
                ' XBC 03/10/2012

                'MyClass.IsInitiatedAttr = True

                isISEStatusUnknownAttr = False 'the status is already known

                If IsISESwitchONAttr <> value Then
                    IsISESwitchONAttr = value

                    If Not value Then
                        ' XB 30/09/2014 - BA-1872
                        'MyClass.IsISECommsOkAttr = False       
                        'MyClass.IsISEInitiatedOKAttr = False

                        ''MyClass.IsISEInitializationDoneAttr = False

                        ''stop timeout timer
                        'MyClass.StopInstructionStartedTimer()  
                        ' XB 30/09/2014 - BA-1872

                        If MyClass.CurrentProcedure <> ISEProcedures.None Then
                            MyClass.AbortCurrentProcedureDueToException()
                        End If

                    Else

                        'check if there are communications with ISE
                        'MyClass.DoGeneralCheckings()

                    End If

                    MyClass.UpdateISEModuleReady()
                    MyClass.RefreshMonitorDataTO()

                    RaiseEvent ISESwitchedOnChanged()
                End If



            End Set
        End Property

        ''' <summary>
        ''' Informs if the communications to the ISE Module are Ok
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public Property IsISECommsOk() As Boolean Implements IISEManager.IsISECommsOk
            Get
                If GlobalConstants.REAL_DEVELOPMENT_MODE > 0 Then
                    Return True
                Else
                    Return IsISECommsOkAttr
                End If
            End Get
            Set(ByVal value As Boolean)
                If IsISECommsOkAttr <> value Then
                    IsISECommsOkAttr = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Returns if the Application is working in disconnected mode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM  12/03/2012
        ''' Modified by XBC 17/01/2013 - Delete 'Readonly' property to be available from AnalyzerManager.ProcessUSBCableDisconnection
        '''                              ISE Object to reset it after a disconnection communications (Bugs tracking #1109)
        ''' </remarks>
        Public Property IsAnalyzerDisconnected() As Boolean Implements IISEManager.IsAnalyzerDisconnected
            Get
                Return IsAnalyzerDisconnectedAttr
            End Get
            Set(value As Boolean)
                IsAnalyzerDisconnectedAttr = value
            End Set
        End Property

        ''' <summary>
        ''' Returns if the Analyzer is still in the Warm Up (Terminate button is not still visible)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public Property IsAnalyzerWarmUp() As Boolean Implements IISEManager.IsAnalyzerWarmUp
            Get
                Return IsAnalyzerWarmUpAttr
            End Get
            Set(ByVal value As Boolean)
                If IsAnalyzerWarmUpAttr <> value Then
                    IsAnalyzerWarmUpAttr = value
                End If
            End Set
        End Property

        Public Property IsCommErrorDetected() As Boolean = False Implements IISEManager.IsCommErrorDetected
        ' XB 04/11/2014 - BA-1872
        Public Property IsTimeOut As Boolean Implements IISEManager.IsTimeOut
            Get
                Return IsTimeoutAttr
            End Get
            Set(value As Boolean)
                If IsTimeoutAttr <> value Then
                    IsTimeoutAttr = value
                End If
            End Set
        End Property

        ' XB 19/11/2014 - BA-1872
        Public ReadOnly Property FirmwareErrDetected As Boolean Implements IISEManager.FirmwareErrDetected
            Get
                Return FirmwareErrDetectedAttr
            End Get
        End Property
#End Region

#Region "Initial Checkings"


        ''' <summary>
        '''The connection to ISE Module is being performed 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public ReadOnly Property IsISEInitiating() As Boolean Implements IISEManager.IsISEInitiating
            Get
                'AG 02/04/2012 - add "OrElse isISEStatusUnknownAttr"
                Return Not MyClass.IsAnalyzerDisconnected And _
                        ((MyClass.CurrentProcedure = ISEProcedures.GeneralCheckings) _
                        And (MyClass.LastProcedureResult = ISEProcedureResult.None)) _
                        And Not IsISEInitializationDone OrElse isISEStatusUnknownAttr
            End Get
        End Property


        ''' <summary>
        '''The connection to ISE Module has been performed (OK or not) 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public ReadOnly Property IsISEInitializationDone() As Boolean Implements IISEManager.IsISEInitializationDone
            Get
                Return IsISEInitializationDoneAttr
            End Get
        End Property


        ''' <summary>
        '''The connection to ISE Module has been performed successfully 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public ReadOnly Property IsISEInitiatedOK() As Boolean Implements IISEManager.IsISEInitiatedOK
            Get
                If GlobalConstants.REAL_DEVELOPMENT_MODE > 0 Then
                    Return True
                Else
                    Return IsISEInitiatedOKAttr
                End If
            End Get
        End Property


        ''' <summary>
        '''The initialization process has finished and the connection process can continue (Wash, Alight...) 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public ReadOnly Property ConnectionTasksCanContinue() As Boolean Implements IISEManager.ConnectionTasksCanContinue
            Get
                ' XBC 03/10/2012 - Correction : some case didn't actives the menu
                'Return ((IsISEModuleInstalled And (IsISEInitializationDone Or Not IsISESwitchON)) Or IsLongTermDeactivation Or Not IsISEModuleInstalled)
                Dim returnValue As Boolean
                returnValue = False

                If Not returnValue AndAlso Not IsISEModuleInstalled Then
                    returnValue = True
                End If

                If Not returnValue AndAlso IsLongTermDeactivation Then
                    returnValue = True
                End If

                If Not returnValue Then
                    returnValue = IsISEInitializationDone
                End If

                'If IsISEInitializationDone AndAlso Not IsISESwitchON Then
                '    returnValue = True
                'End If

                Return returnValue
                ' XBC 03/10/2012
            End Get
        End Property


        ''' <summary>
        '''The connection to ISE Module has been performed successfully once 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public ReadOnly Property IsISEOnceInitiatedOK() As Boolean Implements IISEManager.IsISEOnceInitiatedOK
            Get
                Return IsISEOnceInitiatedOKAttr
            End Get
        End Property


        ''' <summary>
        ''' ISE module must Set as Not initalized from Comms layer
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Created by XBC 24/05/2012</remarks>
        Public WriteOnly Property IsISEInitiatedDone() As Boolean Implements IISEManager.IsISEInitiatedDone
            Set(ByVal value As Boolean)
                MyClass.IsISEInitiatedOKAttr = value
                MyClass.IsISEInitializationDoneAttr = value
            End Set
        End Property

        Public Property IsPendingToInitializeAfterActivation() As Boolean = False Implements IISEManager.IsPendingToInitializeAfterActivation

#End Region

#Region "READY"

#Region "Common"
        ''' <summary>
        ''' Returns if the ISE Module is ready for working
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public ReadOnly Property IsISEModuleReady() As Boolean Implements IISEManager.IsISEModuleReady
            Get
                Dim myGlobal As GlobalDataTO = MyClass.UpdateISEModuleReady
                If Not myGlobal.HasError Then
                    Return IsISEModuleReadyAttr
                Else
                    Return False
                End If
            End Get
        End Property

        ''' <summary>
        ''' This property is updated after user set/reset the Long Term Deactivation 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public Property IsLongTermDeactivation() As Boolean Implements IISEManager.IsLongTermDeactivation
            Get
                Dim myGlobal As New GlobalDataTO
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LONG_TERM_DEACTIVATED)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsLongTermDeactivationAttr = (CInt(myGlobal.SetDatos) > 0)
                Else
                    IsLongTermDeactivationAttr = Nothing
                End If
                Return IsLongTermDeactivationAttr
            End Get
            Set(ByVal value As Boolean)
                Dim myGlobal As New GlobalDataTO
                If value Then
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LONG_TERM_DEACTIVATED, "1")
                    'MyClass.IsISEOnceInitiatedOKAttr = False
                    MyClass.IsISEInitiatedOKAttr = False
                Else
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LONG_TERM_DEACTIVATED, "0")
                    MyClass.IsPendingToInitializeAfterActivation = True
                End If
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                    If Not myGlobal.HasError Then
                        If IsLongTermDeactivationAttr <> value Then
                            IsLongTermDeactivationAttr = value
                        End If
                    End If
                End If
            End Set
        End Property

#End Region

#Region "Reagents"


        ''' <summary>
        '''This property is updated after validating that Reagents pack are installed and not expired 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public ReadOnly Property IsReagentsPackReady() As Boolean Implements IISEManager.IsReagentsPackReady
            Get
                Dim myReturn As Boolean = False

                Dim myGlobal As GlobalDataTO = MyClass.CheckReagentsPackReady

                If Not myGlobal.HasError Then
                    myReturn = CBool(myGlobal.SetDatos)

                    'If myReturn Then
                    '    myGlobal = MyClass.CheckReagentsPackVolumeEnough
                    '    If Not myGlobal.HasError Then
                    '        myReturn = CBool(myGlobal.SetDatos)
                    '    Else
                    '        myReturn = False
                    '    End If
                    'End If
                Else
                    myReturn = False
                End If

                Return myReturn

            End Get
        End Property

        ' XBC 02/07/2012 - NO V1
        Public ReadOnly Property IsReagentsPackSerialNumberMatch() As Boolean Implements IISEManager.IsReagentsPackSerialNumberMatch
            Get
                Return MyClass.IsReagentsPackSerialNumberMatchAttr
            End Get
        End Property

#End Region

#Region "Electrodes"


        ''' <summary>
        '''This property is updated after validating that Electrodes are installed 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public Property IsElectrodesReady() As Boolean Implements IISEManager.IsElectrodesReady
            Get
                Dim myReturn As Boolean = False
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodesReady()
                If Not myGlobal.HasError Then
                    myReturn = CBool(myGlobal.SetDatos)
                Else
                    myReturn = False
                End If

                Return myReturn

            End Get
            Set(ByVal value As Boolean)
                IsElectrodesReadyAttr = value
            End Set
        End Property


#End Region

#End Region

#Region "Monitor Data"

        ''' <summary>
        ''' Returns the current value of the data object used for filling the ISE Monitor tab
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public ReadOnly Property MonitorDataTO() As ISEMonitorTO Implements IISEManager.MonitorDataTO
            Get
                Return MonitorDataTOAttr
            End Get
        End Property

#End Region

#Region "ProcedureS MANAGEMENT"

        ''' <summary>
        '''These properties are used for managing the Procedures related to the ISE like checkings, installations, etc 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public Property CurrentProcedure() As ISEProcedures Implements IISEManager.CurrentProcedure
            Get
                Return CurrentProcedureAttr
            End Get
            Set(ByVal value As ISEProcedures)
                If CurrentProcedureAttr <> value Then
                    'MyClass.LastProcedureResultAttr = ISEProcedureResult.None
                    If value = ISEProcedures.GeneralCheckings Then
                        MyClass.IsISEInitializationDoneAttr = False
                        MyClass.IsISEInitiatedOKAttr = False
                    End If
                    CurrentProcedureAttr = value

                    ' XBC 27/08/2012 - Correction : Save consumptions after Current Procedure is finished
                    CurrentProcedureIsFinishedAttr = False
                End If
            End Set
        End Property

        ' XBC 27/08/2012 - Correction : Save consumptions after Current Procedure is finished
        Public ReadOnly Property CurrentProcedureIsFinished() As Boolean Implements IISEManager.CurrentProcedureIsFinished
            Get
                Return CurrentProcedureIsFinishedAttr
            End Get
        End Property

        ''' <summary>
        ''' Informs about the current ISE command sent to the Module
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public Property CurrentCommandTO() As ISECommandTO Implements IISEManager.CurrentCommandTO
            Get
                Return CurrentCommandTOAttr
            End Get
            Set(ByVal value As ISECommandTO)
                If CurrentCommandTOAttr IsNot value Then
                    CurrentCommandTOAttr = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Informs about the last ISE Result received from the Module
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public Property LastISEResult() As ISEResultTO Implements IISEManager.LastISEResult
            Get
                Return LastISEResultAttr
            End Get
            Set(ByVal value As ISEResultTO)

                If LastISEResultAttr IsNot value Then
                    Dim myResult As ISEResultTO = CType(value, ISEResultTO)
                    If myResult.ISEResultType <> ISEResultTO.ISEResultTypes.None Then
                        LastISEResultAttr = value
                        'MyClass.StopInstructionStartedTimer()
                        MyClass.ManageLastISEResult()
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' Informs about the result of the last procedure performed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Public ReadOnly Property LastProcedureResult() As ISEProcedureResult Implements IISEManager.LastProcedureResult
            Get
                Return LastProcedureResultAttr
            End Get
        End Property




#End Region

#Region "Consumptions"

        ''' <summary>
        ''' Returns if it is required to write calibrator A consumption data to the Module
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by XB 12/03/2012</remarks>
        Public ReadOnly Property IsCalAUpdateRequired() As Boolean Implements IISEManager.IsCalAUpdateRequired
            Get
                Return IsCalAUpdateRequiredAttr
            End Get
        End Property

        ''' <summary>
        ''' Returns if it is required to write calibrator B consumption data to the Module
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Created by XB 12/03/2012</remarks>
        Public ReadOnly Property IsCalBUpdateRequired() As Boolean Implements IISEManager.IsCalBUpdateRequired
            Get
                Return IsCalBUpdateRequiredAttr
            End Get
        End Property

        ' XBC 17/07/2012
        Public Property PurgeAbyFirmware() As Integer Implements IISEManager.PurgeAbyFirmware
            Get
                Return MyClass.PurgeAbyFirmwareAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.PurgeAbyFirmwareAttr = value
            End Set
        End Property

        ' XBC 17/07/2012
        Public Property PurgeBbyFirmware() As Integer Implements IISEManager.PurgeBbyFirmware
            Get
                Return MyClass.PurgeBbyFirmwareAttr
            End Get
            Set(ByVal value As Integer)
                MyClass.PurgeBbyFirmwareAttr = value
            End Set
        End Property

        ' XBC 17/07/2012
        Public Property WorkSessionOverallTime() As Single Implements IISEManager.WorkSessionOverallTime
            Get
                Return MyClass.WorkSessionOverallTimeAttr
            End Get
            Set(ByVal value As Single)
                MyClass.WorkSessionOverallTimeAttr = value
            End Set
        End Property

        ' XBC 17/07/2012
        Public Property WorkSessionIsRunning() As Boolean Implements IISEManager.WorkSessionIsRunning
            Get
                Return MyClass.WorkSessionIsRunningAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.WorkSessionIsRunningAttr = value
            End Set
        End Property

        ' XBC 23/07/2012
        Public Property WorkSessionTestsByType() As String Implements IISEManager.WorkSessionTestsByType
            Get
                Return MyClass.WorkSessionTestsByTypeAttr
            End Get
            Set(ByVal value As String)
                MyClass.WorkSessionTestsByTypeAttr = value
            End Set
        End Property

#End Region


#Region "Maintenance Needed"

        'This property is updated after validating that a Electrodes' Calibration is needed
        Public Property IsCalibrationNeeded() As Boolean Implements IISEManager.IsCalibrationNeeded
            Get
                Return IsCalibrationNeededAttr
            End Get
            Set(ByVal value As Boolean)
                If IsCalibrationNeededAttr <> value Then
                    IsCalibrationNeededAttr = value
                    If value Then
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.CALB_PENDING, "1")
                        Debug.Print("CALB PENDING = TRUE")
                        RaiseEvent ISEMaintenanceRequired(MaintenanceOperations.ElectrodesCalibration)
                    Else
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.CALB_PENDING, "0")
                        Debug.Print("CALB PENDING = FALSE")
                    End If
                    UpdateISEInformationTable() 'JB 03/09/2012 - Save to Database
                End If
            End Set
        End Property

        'This property is updated after validating that a Pumps' Calibration is needed
        Public Property IsPumpCalibrationNeeded() As Boolean Implements IISEManager.IsPumpCalibrationNeeded
            Get
                Return IsPumpCalibrationNeededAttr
            End Get
            Set(ByVal value As Boolean)
                If IsPumpCalibrationNeededAttr <> value Then
                    IsPumpCalibrationNeededAttr = value
                    If value Then
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.PUMP_CAL_PENDING, "1")
                        Debug.Print("PUMP CAL PENDING = TRUE")
                        RaiseEvent ISEMaintenanceRequired(MaintenanceOperations.PumpsCalibration)
                    Else
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.PUMP_CAL_PENDING, "0")
                        Debug.Print("PUMP CAL PENDING = FALSE")
                    End If
                    UpdateISEInformationTable() 'JB 03/09/2012 - Save to Database
                End If

            End Set
        End Property

        'SGM 25/07/2012
        'This property is updated after validating that a Bubble detector Calibration is needed
        Public Property IsBubbleCalibrationNeeded() As Boolean Implements IISEManager.IsBubbleCalibrationNeeded
            Get
                Return IsBubbleCalibrationNeededAttr
            End Get
            Set(ByVal value As Boolean)
                If IsBubbleCalibrationNeededAttr <> value Then
                    IsBubbleCalibrationNeededAttr = value
                    If value Then
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.BUBBLE_CAL_PENDING, "1")
                        Debug.Print("BUBBLE PENDING = TRUE")
                        RaiseEvent ISEMaintenanceRequired(MaintenanceOperations.BubbleCalibration)
                    Else
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.BUBBLE_CAL_PENDING, "0")
                        Debug.Print("BUBBLE PENDING = FALSE")
                    End If
                    UpdateISEInformationTable() 'JB 03/09/2012 - Save to Database
                End If
            End Set
        End Property

        Public Property IsCleanNeeded() As Boolean Implements IISEManager.IsCleanNeeded
            Get
                Return IsCleanNeededAttr
            End Get
            Set(ByVal value As Boolean)
                If IsCleanNeededAttr <> value Then
                    IsCleanNeededAttr = value
                    If value Then
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.CLEAN_PENDING, "1")
                        Debug.Print("CLEAN PENDING = TRUE")
                        RaiseEvent ISEMaintenanceRequired(MaintenanceOperations.Clean)
                    Else
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.CLEAN_PENDING, "0")
                        Debug.Print("CLEAN PENDING = FALSE")
                    End If
                    UpdateISEInformationTable() 'JB 03/09/2012 - Save to Database
                End If

            End Set
        End Property



#End Region

#Region "Maintenance Recommended" 'IS IT USED?????


        Private ReadOnly Property IsReplaceLiRecommended() As Boolean
            Get
                Return (IsLiEnabledByUser And IsLiInstalled And (IsLiExpired Or IsLiOverUsed))
            End Get
        End Property

        Public ReadOnly Property IsReplaceNaRecommended() As Boolean Implements IISEManager.IsReplaceNaRecommended
            Get
                Return (IsNaInstalled And (IsNaExpired Or IsNaOverUsed))
            End Get
        End Property

        Public ReadOnly Property IsReplaceKRecommended() As Boolean Implements IISEManager.IsReplaceKRecommended
            Get
                Return (IsKInstalled And (IsKExpired Or IsKOverUsed))
            End Get
        End Property

        Public ReadOnly Property IsReplaceClRecommended() As Boolean Implements IISEManager.IsReplaceClRecommended
            Get
                Return (IsClInstalled And (IsClExpired Or IsClOverUsed))
            End Get
        End Property


        Public ReadOnly Property IsReplaceRefRecommended() As Boolean Implements IISEManager.IsReplaceRefRecommended
            Get
                Return (IsRefInstalled And (IsRefExpired Or IsRefOverUsed))
            End Get
        End Property


        Public ReadOnly Property IsReplaceFluidicTubingRecommended() As Boolean Implements IISEManager.IsReplaceFluidicTubingRecommended
            Get
                Return ((FluidicTubingExpireDate <> Nothing) AndAlso FluidicTubingExpireDate > DateTime.Now)
            End Get
        End Property


        Public ReadOnly Property IsReplacePumpTubingRecommended() As Boolean Implements IISEManager.IsReplacePumpTubingRecommended
            Get
                Return ((PumpTubingExpireDate <> Nothing) AndAlso PumpTubingExpireDate > DateTime.Now)
            End Get
        End Property

#End Region

#Region "Last Calibrations and Cleans"

        Public Property LastElectrodesCalibrationResult1() As String Implements IISEManager.LastElectrodesCalibrationResult1
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_CALB_RESULT1)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastElectrodesCalibrationResult1Attr = CType(myGlobal.SetDatos, String)
                    Return LastElectrodesCalibrationResult1Attr
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                If LastElectrodesCalibrationResult1Attr <> value Then
                    LastElectrodesCalibrationResult1Attr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_CALB_RESULT1, value)
                End If
            End Set
        End Property
        Public Property LastElectrodesCalibrationResult2() As String Implements IISEManager.LastElectrodesCalibrationResult2
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_CALB_RESULT2)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastElectrodesCalibrationResult2Attr = CType(myGlobal.SetDatos, String)
                    Return LastElectrodesCalibrationResult2Attr
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                If LastElectrodesCalibrationResult2Attr <> value Then
                    LastElectrodesCalibrationResult2Attr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_CALB_RESULT2, value)
                End If
            End Set
        End Property
        'JB 02/08/2012
        Public Property LastElectrodesCalibrationError() As String Implements IISEManager.LastElectrodesCalibrationError
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_CALB_ERROR)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastElectrodesCalibrationErrorAttr = CType(myGlobal.SetDatos, String)
                    Return LastElectrodesCalibrationErrorAttr
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                If LastElectrodesCalibrationErrorAttr <> value Then
                    LastElectrodesCalibrationErrorAttr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_CALB_ERROR, value)
                End If
            End Set
        End Property

        Public Property LastPumpsCalibrationResult() As String Implements IISEManager.LastPumpsCalibrationResult
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_PUMPCAL_RESULT)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastPumpsCalibrationResultAttr = CType(myGlobal.SetDatos, String)
                    Return LastPumpsCalibrationResultAttr
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                If LastPumpsCalibrationResultAttr <> value Then
                    LastPumpsCalibrationResultAttr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_PUMPCAL_RESULT, value)
                End If
            End Set
        End Property
        'JB 02/08/2012
        Public Property LastPumpsCalibrationError() As String Implements IISEManager.LastPumpsCalibrationError
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_PUMPCAL_ERROR)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastPumpsCalibrationErrorAttr = CType(myGlobal.SetDatos, String)
                    Return LastPumpsCalibrationErrorAttr
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                If LastPumpsCalibrationErrorAttr <> value Then
                    LastPumpsCalibrationErrorAttr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_PUMPCAL_ERROR, value)
                End If
            End Set
        End Property


        Public Property LastElectrodesCalibrationDate() As DateTime Implements IISEManager.LastElectrodesCalibrationDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_CALB_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastElectrodesCalibrationDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    LastElectrodesCalibrationDateAttr = Nothing
                End If
                Return LastElectrodesCalibrationDateAttr
            End Get
            Set(ByVal value As DateTime)
                If LastElectrodesCalibrationDateAttr <> value Then
                    LastElectrodesCalibrationDateAttr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_CALB_DATE, value.ToString, True)
                End If
            End Set
        End Property

        Public Property LastPumpsCalibrationDate() As DateTime Implements IISEManager.LastPumpsCalibrationDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_PUMP_CAL_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastPumpsCalibrationDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    LastPumpsCalibrationDateAttr = Nothing
                End If
                Return LastPumpsCalibrationDateAttr
            End Get
            Set(ByVal value As DateTime)
                If LastPumpsCalibrationDateAttr <> value Then
                    LastPumpsCalibrationDateAttr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_PUMP_CAL_DATE, value.ToString, True)
                End If
            End Set
        End Property
        'SGM 25/07/2012
        Public Property LastBubbleCalibrationResult() As String Implements IISEManager.LastBubbleCalibrationResult
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_BUBBLECAL_RESULT)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastBubbleCalibrationResultAttr = CType(myGlobal.SetDatos, String)
                    Return LastBubbleCalibrationResultAttr
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                If LastBubbleCalibrationResultAttr <> value Then
                    LastBubbleCalibrationResultAttr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_BUBBLECAL_RESULT, value)
                End If
            End Set
        End Property
        'JB 02/08/2012
        Public Property LastBubbleCalibrationError() As String Implements IISEManager.LastBubbleCalibrationError
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_BUBBLECAL_ERROR)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastBubbleCalibrationErrorAttr = CType(myGlobal.SetDatos, String)
                    Return LastBubbleCalibrationErrorAttr
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                If LastBubbleCalibrationErrorAttr <> value Then
                    LastBubbleCalibrationErrorAttr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_BUBBLECAL_ERROR, value)
                End If
            End Set
        End Property

        'SGM 25/07/2012
        Public Property LastBubbleCalibrationDate() As DateTime Implements IISEManager.LastBubbleCalibrationDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_BUBBLE_CAL_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastBubbleCalibrationDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    LastBubbleCalibrationDateAttr = Nothing
                End If
                Return LastBubbleCalibrationDateAttr
            End Get
            Set(ByVal value As DateTime)
                If LastBubbleCalibrationDateAttr <> value Then
                    LastBubbleCalibrationDateAttr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_BUBBLE_CAL_DATE, value.ToString, True)
                End If
            End Set
        End Property

        Public Property LastCleanDate() As DateTime Implements IISEManager.LastCleanDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_CLEAN_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastCleanDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    LastCleanDateAttr = Nothing
                End If
                Return LastCleanDateAttr
            End Get
            Set(ByVal value As DateTime)
                If LastCleanDateAttr <> value Then
                    LastCleanDateAttr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_CLEAN_DATE, value.ToString, True)
                End If
            End Set
        End Property
        'JB 02/08/2012
        Public Property LastCleanError() As String Implements IISEManager.LastCleanError
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_CLEAN_ERROR)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LastCleanErrorAttr = CType(myGlobal.SetDatos, String)
                    Return LastCleanErrorAttr
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As String)
                If LastCleanErrorAttr <> value Then
                    LastCleanErrorAttr = value
                    Dim myGlobal As New GlobalDataTO 'update info DS
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_CLEAN_ERROR, value)
                End If
            End Set
        End Property


#End Region





#Region "Private properties"





#Region "Reagents"

        'This property is updated after validating that Reagents pack are installed and not expired
        Private ReadOnly Property IsReagentsPackInstalled() As Boolean
            Get

                Return (HasBiosystemsCode And (MyClass.ReagentsPackInstallationDate <> Nothing))

            End Get
        End Property

        'This property is updated after requesting Dallas chip data
        Private ReadOnly Property HasBiosystemsCode() As Boolean
            Get
                If MyClass.IsBiosystemsValidationMode Then
                    'NEW PACKS
                    Return CBool(MyClass.ValidateBiosystemsPack().SetDatos)
                Else
                    'OLD PACKS
                    Return CBool(MyClass.ValidateBiosystemsCode().SetDatos)
                End If
            End Get
        End Property

        Private Property ReagentsPackExpirationDate() As DateTime
            Get
                If MyClass.ISEDallasPage00Attr IsNot Nothing Then
                    ReagentsPackExpirationDateAttr = MyClass.ISEDallasPage00Attr.ExpirationDate
                Else
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.REAGENTS_EXPIRE_DATE, True)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        ReagentsPackExpirationDateAttr = CDate(myGlobal.SetDatos)
                    Else
                        ReagentsPackExpirationDateAttr = Nothing
                    End If
                End If
                'Return Nothing 'For Test Document
                'Return Now.AddDays(-1) 'For Test Document
                Return ReagentsPackExpirationDateAttr
            End Get
            Set(ByVal value As DateTime)
                If ReagentsPackExpirationDateAttr <> value Then
                    ReagentsPackExpirationDateAttr = value
                    Dim myGlobal As GlobalDataTO = MyClass.UpdateISEInfoSetting(ISEModuleSettings.REAGENTS_EXPIRE_DATE, value.ToString, True)
                    If Not myGlobal.HasError Then
                        myGlobal = MyClass.UpdateISEInformationTable
                    End If
                End If
            End Set

        End Property

        Public Property ReagentsPackInstallationDate() As DateTime Implements IISEManager.ReagentsPackInstallationDate
            Get
                If MyClass.ISEDallasPage01Attr IsNot Nothing Then
                    ReagentsPackInstallationDateAttr = MyClass.ISEDallasPage01Attr.InstallationDate
                Else
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.REAGENTS_INSTALL_DATE, True)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        ReagentsPackInstallationDateAttr = CDate(myGlobal.SetDatos)
                    Else
                        ReagentsPackInstallationDateAttr = Nothing
                    End If
                End If
                'Return Nothing 'For Test Document
                Return (ReagentsPackInstallationDateAttr)
            End Get
            Set(ByVal value As DateTime)
                If ReagentsPackInstallationDateAttr <> value Then
                    ReagentsPackInstallationDateAttr = value
                    Dim myGlobal As GlobalDataTO = MyClass.UpdateISEInfoSetting(ISEModuleSettings.REAGENTS_INSTALL_DATE, value.ToString, True)
                    If Not myGlobal.HasError Then
                        myGlobal = MyClass.UpdateISEInformationTable
                    End If
                End If
            End Set

        End Property


        Private Property BiosystemsCode() As String
            Get
                If MyClass.ISEDallasPage00Attr IsNot Nothing Then
                    BiosystemsCodeAttr = MyClass.ISEDallasPage00Attr.DistributorCode
                Else
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.DISTRIBUTOR_CODE)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        BiosystemsCodeAttr = CStr(myGlobal.SetDatos)
                    Else
                        BiosystemsCodeAttr = Nothing
                    End If
                End If
                Return BiosystemsCodeAttr
            End Get
            Set(ByVal value As String)
                If BiosystemsCodeAttr <> value Then
                    BiosystemsCodeAttr = value
                    Dim myGlobal As GlobalDataTO = MyClass.UpdateISEInfoSetting(ISEModuleSettings.DISTRIBUTOR_CODE, value.ToString)
                    If Not myGlobal.HasError Then
                        myGlobal = MyClass.UpdateISEInformationTable
                    End If
                End If
            End Set
        End Property

        Private Property ReagentsPackInitialVolCalA() As Single
            Get
                If MyClass.ISEDallasPage00Attr IsNot Nothing Then
                    ReagentsPackInitialVolCalAAttr = MyClass.ISEDallasPage00Attr.InitialCalibAVolume
                Else
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.INITIAL_VOL_CAL_A)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        ReagentsPackInitialVolCalAAttr = CSng(myGlobal.SetDatos)
                    Else
                        ReagentsPackInitialVolCalAAttr = Nothing
                    End If
                End If
                Return ReagentsPackInitialVolCalAAttr
            End Get
            Set(ByVal value As Single)
                If ReagentsPackInitialVolCalAAttr <> value Then
                    ReagentsPackInitialVolCalAAttr = value
                    Dim myGlobal As GlobalDataTO = MyClass.UpdateISEInfoSetting(ISEModuleSettings.INITIAL_VOL_CAL_A, value.ToString)
                    If Not myGlobal.HasError Then
                        myGlobal = MyClass.UpdateISEInformationTable
                    End If
                End If
            End Set
        End Property


        Private Property ReagentsPackInitialVolCalB() As Single
            Get
                If MyClass.ISEDallasPage00Attr IsNot Nothing Then
                    ReagentsPackInitialVolCalBAttr = MyClass.ISEDallasPage00Attr.InitialCalibBVolume
                Else
                    Dim myGlobal As New GlobalDataTO
                    myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.INITIAL_VOL_CAL_B)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        ReagentsPackInitialVolCalBAttr = CSng(myGlobal.SetDatos)
                    Else
                        ReagentsPackInitialVolCalBAttr = Nothing
                    End If
                End If
                Return ReagentsPackInitialVolCalBAttr
            End Get
            Set(ByVal value As Single)
                If ReagentsPackInitialVolCalBAttr <> value Then
                    ReagentsPackInitialVolCalBAttr = value
                    Dim myGlobal As GlobalDataTO = MyClass.UpdateISEInfoSetting(ISEModuleSettings.INITIAL_VOL_CAL_B, value.ToString)
                    If Not myGlobal.HasError Then
                        myGlobal = MyClass.UpdateISEInformationTable
                    End If
                End If
            End Set
        End Property

        Private ReadOnly Property ReagentsPackRemainingVolCalA() As Single
            Get
                'Return 5 'For Test Document
                Dim myglobal As New GlobalDataTO
                'Dim Utilities As New Utilities
                myglobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.AVAILABLE_VOL_CAL_A)
                If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                    Return Utilities.FormatToSingle(CStr(myglobal.SetDatos))
                Else
                    Return -1
                End If
            End Get
        End Property

        Private ReadOnly Property ReagentsPackRemainingVolCalB() As Single
            Get
                Dim myglobal As New GlobalDataTO
                'Dim Utilities As New Utilities
                myglobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.AVAILABLE_VOL_CAL_B)
                If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                    Return Utilities.FormatToSingle(CStr(myglobal.SetDatos))
                Else
                    Return -1
                End If
            End Get
        End Property

        Public Property IsCleanPackInstalled() As Boolean Implements IISEManager.IsCleanPackInstalled
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.CLEANING_PACK_INSTALLED)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsCleanPackInstalledAttr = (CInt(myGlobal.SetDatos) > 0)
                Else
                    IsCleanPackInstalledAttr = False
                End If
                Return IsCleanPackInstalledAttr
            End Get
            Set(ByVal value As Boolean)
                IsCleanPackInstalledAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                If IsCleanPackInstalledAttr Then
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.CLEANING_PACK_INSTALLED, "1")
                Else
                    myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.CLEANING_PACK_INSTALLED, "0")
                End If
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property

        Private ReadOnly Property IsEnoughCalibratorA() As Boolean
            Get
                Return CBool(MyClass.ValidateReagentsPackVolumes("A", MyClass.ReagentsPackRemainingVolCalA).SetDatos)
            End Get
        End Property

        Private ReadOnly Property IsEnoughCalibratorB() As Boolean
            Get
                Return CBool(MyClass.ValidateReagentsPackVolumes("B", MyClass.ReagentsPackRemainingVolCalB).SetDatos)
            End Get
        End Property

        'SGM 05/06/2012 - for Biosystems validation
        Private Property ISEDallasSN() As ISEDallasSNTO
            Get
                Return ISEDallasSNAttr
            End Get
            Set(ByVal value As ISEDallasSNTO)
                ISEDallasSNAttr = value
            End Set
        End Property

        Private Property ISEDallasPage00() As ISEDallasPage00TO
            Get
                Return ISEDallasPage00Attr
            End Get
            Set(ByVal value As ISEDallasPage00TO)
                If ISEDallasPage00Attr IsNot value Then
                    ISEDallasPage00Attr = value
                    If ISEDallasPage00Attr IsNot Nothing Then
                        MyClass.BiosystemsCode = ISEDallasPage00Attr.DistributorCode
                        MyClass.ReagentsPackExpirationDate = ISEDallasPage00Attr.ExpirationDate
                        MyClass.ReagentsPackInitialVolCalA = ISEDallasPage00Attr.InitialCalibAVolume
                        MyClass.ReagentsPackInitialVolCalB = ISEDallasPage00Attr.InitialCalibBVolume
                    End If
                End If
            End Set
        End Property
        Private Property ISEDallasPage01() As ISEDallasPage01TO
            Get
                Return ISEDallasPage01Attr
            End Get
            Set(ByVal value As ISEDallasPage01TO)
                If ISEDallasPage01Attr IsNot value Then

                    ISEDallasPage01Attr = value

                    'Dim Utilities As New Utilities
                    Dim myGlobal As New GlobalDataTO

                    'if the current consumption value in the Dallas > saved in DB then update the db, 
                    'else, update with the new consumption value
                    Dim myCalibAVolumeSaved As Single = -1
                    Dim myCalibBVolumeSaved As Single = -1
                    Dim myCalibAVolumeToSave As Single = -1
                    Dim myCalibBVolumeToSave As Single = -1

                    If ISEDallasPage00 IsNot Nothing Then 'SGM 13/06/2012
                        myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.AVAILABLE_VOL_CAL_A)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            myCalibAVolumeSaved = Utilities.FormatToSingle(CStr(myGlobal.SetDatos))


                            Dim myDallasCalibAVolume As Single

                            myDallasCalibAVolume = (ISEDallasPage00.InitialCalibAVolume - MyClass.GetValueFromPercent(ISEDallasPage00.InitialCalibAVolume, value.ConsumptionCalA))

                            If (myDallasCalibAVolume < myCalibAVolumeSaved) Or _
                               myCalibAVolumeSaved = 0 Or _
                               MyClass.ForceConsumptionsInitialize Then

                                myCalibAVolumeToSave = myDallasCalibAVolume
                                MyClass.CountConsumptionToSaveDallasData_CalA = 0
                            Else
                                myCalibAVolumeToSave = myCalibAVolumeSaved
                                MyClass.CountConsumptionToSaveDallasData_CalA = (myDallasCalibAVolume - myCalibAVolumeSaved)
                            End If


                        End If
                        myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.AVAILABLE_VOL_CAL_B)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            myCalibBVolumeSaved = Utilities.FormatToSingle(CStr(myGlobal.SetDatos))

                            Dim myDallasCalibBVolume As Single

                            myDallasCalibBVolume = ISEDallasPage00.InitialCalibBVolume - MyClass.GetValueFromPercent(ISEDallasPage00.InitialCalibBVolume, value.ConsumptionCalB)

                            If (myDallasCalibBVolume < myCalibBVolumeSaved) Or _
                               myCalibBVolumeSaved = 0 Or _
                               MyClass.ForceConsumptionsInitialize Then

                                myCalibBVolumeToSave = myDallasCalibBVolume
                                MyClass.CountConsumptionToSaveDallasData_CalB = 0
                            Else
                                myCalibBVolumeToSave = myCalibBVolumeSaved
                                MyClass.CountConsumptionToSaveDallasData_CalB = (myDallasCalibBVolume - myCalibBVolumeSaved)
                            End If
                        End If

                        ' XBC 18/07/2012
                        MyClass.ForceConsumptionsInitialize = False
                    End If


                    MyClass.ReagentsPackInstallationDate = value.InstallationDate

                    'If value.InstallationDate <> Nothing Then
                    '    MyClass.UpdateISEInfoSetting(ISEModuleSettings.REAGENTS_INSTALL_DATE, value.InstallationDate.ToString)
                    'End If
                    If myCalibAVolumeToSave >= 0 Then
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.AVAILABLE_VOL_CAL_A, (myCalibAVolumeToSave).ToString)
                    End If
                    If myCalibBVolumeToSave >= 0 Then
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.AVAILABLE_VOL_CAL_B, (myCalibBVolumeToSave).ToString)
                    End If

                End If
            End Set
        End Property

#End Region

#Region "Common"

        ''' <summary>
        ''' Returns the value respect a Total value corresponding with a percent
        ''' </summary>
        ''' <param name="pTotal"></param>
        ''' <param name="pPercent"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 31/05/2012</remarks>
        Private Function GetValueFromPercent(ByVal pTotal As Single, ByVal pPercent As Single) As Integer
            Dim myReturnValue As Integer = 0
            Try
                If pTotal > 0 Then
                    myReturnValue = CInt((pPercent * pTotal) / 100)
                End If
            Catch ex As Exception
                Throw ex
            End Try
            Return myReturnValue
        End Function

#End Region

#Region "Electrodes"

        Public Property LiInstallDate() As DateTime Implements IISEManager.LiInstallDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LI_INSTALL_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LiInstallDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    Return Nothing
                End If
                Return LiInstallDateAttr
            End Get
            Set(ByVal value As DateTime)
                LiInstallDateAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LI_INSTALL_DATE, LiInstallDateAttr.ToString, True)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property

        Public Property NaInstallDate() As DateTime Implements IISEManager.NaInstallDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.NA_INSTALL_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    NaInstallDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    Return Nothing
                End If
                Return NaInstallDateAttr
            End Get
            Set(ByVal value As DateTime)
                NaInstallDateAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.NA_INSTALL_DATE, NaInstallDateAttr.ToString, True)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property

        Public Property KInstallDate() As DateTime Implements IISEManager.KInstallDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.K_INSTALL_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    KInstallDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    Return Nothing
                End If
                Return KInstallDateAttr
            End Get
            Set(ByVal value As DateTime)
                KInstallDateAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.K_INSTALL_DATE, KInstallDateAttr.ToString, True)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property

        Public Property ClInstallDate() As DateTime Implements IISEManager.ClInstallDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.CL_INSTALL_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    ClInstallDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    Return Nothing
                End If
                Return ClInstallDateAttr
            End Get
            Set(ByVal value As DateTime)
                ClInstallDateAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.CL_INSTALL_DATE, ClInstallDateAttr.ToString, True)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property

        Public Property RefInstallDate() As DateTime Implements IISEManager.RefInstallDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.REF_INSTALL_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    RefInstallDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    Return Nothing
                End If
                Return RefInstallDateAttr
            End Get
            Set(ByVal value As DateTime)
                RefInstallDateAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.REF_INSTALL_DATE, RefInstallDateAttr.ToString, True)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property

        Public ReadOnly Property HasLiInstallDate() As Boolean Implements IISEManager.HasLiInstallDate
            Get
                Return (MyClass.LiInstallDate <> Nothing)
            End Get
        End Property

        Public ReadOnly Property HasNaInstallDate() As Boolean Implements IISEManager.HasNaInstallDate
            Get
                Return (MyClass.NaInstallDate <> Nothing)
            End Get
        End Property

        Public ReadOnly Property HasKInstallDate() As Boolean Implements IISEManager.HasKInstallDate
            Get
                Return (MyClass.KInstallDate <> Nothing)
            End Get
        End Property

        Public ReadOnly Property HasClInstallDate() As Boolean Implements IISEManager.HasClInstallDate
            Get
                Return (MyClass.ClInstallDate <> Nothing)
            End Get
        End Property

        Public ReadOnly Property HasRefInstallDate() As Boolean Implements IISEManager.HasRefInstallDate
            Get
                Return (MyClass.RefInstallDate <> Nothing)
            End Get
        End Property

        Private ReadOnly Property IsRefMounted() As Boolean
            Get
                If MyClass.IsLiEnabledByUser Then
                    Return (MyClass.IsLiMounted Or MyClass.IsNaMounted Or MyClass.IsKMounted Or MyClass.IsClMounted)
                Else
                    Return (MyClass.IsNaMounted Or MyClass.IsKMounted Or MyClass.IsClMounted)
                End If
            End Get
        End Property

        Private Property IsLiMounted() As Boolean
            Get
                Return IsLiMountedAttr
            End Get
            Set(ByVal value As Boolean)
                IsLiMountedAttr = value
            End Set
        End Property

        Private Property IsNaMounted() As Boolean
            Get
                Return IsNaMountedAttr
            End Get
            Set(ByVal value As Boolean)
                IsNaMountedAttr = value
            End Set
        End Property

        Private Property IsKMounted() As Boolean
            Get
                'Return False 'For Test Document
                Return IsKMountedAttr
            End Get
            Set(ByVal value As Boolean)
                IsKMountedAttr = value
            End Set
        End Property

        Private Property IsClMounted() As Boolean
            Get
                Return IsClMountedAttr
            End Get
            Set(ByVal value As Boolean)
                IsClMountedAttr = value
            End Set
        End Property


        Private ReadOnly Property IsRefInstalled() As Boolean
            Get
                If MyClass.IsISEInitiatedOK And Not MyClass.IsLongTermDeactivation Then
                    Return (MyClass.HasRefInstallDate And MyClass.IsRefMounted)
                Else
                    Return MyClass.HasRefInstallDate
                End If
            End Get
        End Property

        'defined by the user that the Lithium is usable
        Public Property IsLiEnabledByUser() As Boolean Implements IISEManager.IsLiEnabledByUser
            Get
                Dim myGlobal As New GlobalDataTO
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LI_ENABLED)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsLiEnabledByUserAttr = (CInt(myGlobal.SetDatos) > 0)
                Else
                    IsLiEnabledByUserAttr = False
                End If
                Return IsLiEnabledByUserAttr
            End Get
            Set(ByVal value As Boolean)
                IsLiEnabledByUserAttr = value
                Dim isEnabled As String = CStr(IIf(IsLiEnabledByUserAttr, "1", "0"))
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LI_ENABLED, isEnabled)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If

            End Set
        End Property

        Private ReadOnly Property IsLiInstalled() As Boolean
            Get
                If MyClass.IsISEInitiatedOK And Not MyClass.IsLongTermDeactivation Then
                    Return (MyClass.HasLiInstallDate And MyClass.IsLiMounted)
                Else
                    Return MyClass.HasLiInstallDate
                End If
            End Get
        End Property

        Private ReadOnly Property IsNaInstalled() As Boolean
            Get
                If MyClass.IsISEInitiatedOK And Not MyClass.IsLongTermDeactivation Then
                    Return (MyClass.HasNaInstallDate And MyClass.IsNaMounted)
                Else
                    Return MyClass.HasNaInstallDate
                End If
            End Get
        End Property

        Private ReadOnly Property IsKInstalled() As Boolean
            Get
                If MyClass.IsISEInitiatedOK And Not MyClass.IsLongTermDeactivation Then
                    Return (MyClass.HasKInstallDate And MyClass.IsKMounted)
                Else
                    Return MyClass.HasKInstallDate
                End If
            End Get
        End Property

        Private ReadOnly Property IsClInstalled() As Boolean
            Get
                If MyClass.IsISEInitiatedOK And Not MyClass.IsLongTermDeactivation Then
                    Return (MyClass.HasClInstallDate And MyClass.IsClMounted)
                Else
                    Return MyClass.HasClInstallDate
                End If
            End Get
        End Property

        Private ReadOnly Property IsRefExpired() As Boolean
            Get
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodeExpired(ISE_Electrodes.Ref)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsRefExpiredAttr = CBool(myGlobal.SetDatos)
                Else
                    IsRefExpiredAttr = False
                End If
                Return IsRefExpiredAttr
            End Get
        End Property

        Private ReadOnly Property IsLiExpired() As Boolean
            Get
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodeExpired(ISE_Electrodes.Li)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsLiExpiredAttr = CBool(myGlobal.SetDatos)
                Else
                    IsLiExpiredAttr = False
                End If
                Return IsLiExpiredAttr
            End Get
        End Property

        Private ReadOnly Property IsNaExpired() As Boolean
            Get
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodeExpired(ISE_Electrodes.Na)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsNaExpiredAttr = CBool(myGlobal.SetDatos)
                Else
                    IsNaExpiredAttr = False
                End If
                Return IsNaExpiredAttr
            End Get
        End Property

        Private ReadOnly Property IsKExpired() As Boolean
            Get
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodeExpired(ISE_Electrodes.K)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsKExpiredAttr = CBool(myGlobal.SetDatos)
                Else
                    IsKExpiredAttr = False
                End If
                Return IsKExpiredAttr
            End Get
        End Property

        Private ReadOnly Property IsClExpired() As Boolean
            Get
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodeExpired(ISE_Electrodes.Cl)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsClExpiredAttr = CBool(myGlobal.SetDatos)
                Else
                    IsClExpiredAttr = False
                End If
                Return IsClExpiredAttr
            End Get
        End Property

        Private ReadOnly Property IsRefOverUsed() As Boolean
            Get
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodeOverUsed(ISE_Electrodes.Ref)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsRefOverUsedAttr = CBool(myGlobal.SetDatos)
                Else
                    IsRefOverUsedAttr = False
                End If
                Return IsRefOverUsedAttr
            End Get
        End Property

        Private ReadOnly Property IsLiOverUsed() As Boolean
            Get
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodeOverUsed(ISE_Electrodes.Li)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsLiOverUsedAttr = CBool(myGlobal.SetDatos)
                Else
                    IsLiOverUsedAttr = False
                End If
                Return IsLiOverUsedAttr
            End Get
        End Property

        Private ReadOnly Property IsNaOverUsed() As Boolean
            Get
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodeOverUsed(ISE_Electrodes.Na)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsNaOverUsedAttr = CBool(myGlobal.SetDatos)
                Else
                    IsNaOverUsedAttr = False
                End If
                Return IsNaOverUsedAttr
            End Get
        End Property

        Private ReadOnly Property IsKOverUsed() As Boolean
            Get
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodeOverUsed(ISE_Electrodes.K)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsKOverUsedAttr = CBool(myGlobal.SetDatos)
                Else
                    IsKOverUsedAttr = False
                End If
                Return IsKOverUsedAttr
            End Get
        End Property

        Private ReadOnly Property IsClOverUsed() As Boolean
            Get
                Dim myGlobal As GlobalDataTO = MyClass.CheckElectrodeOverUsed(ISE_Electrodes.Cl)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsClOverUsedAttr = CBool(myGlobal.SetDatos)
                Else
                    IsClOverUsedAttr = False
                End If
                Return IsClOverUsedAttr
            End Get
        End Property

        Private Property RefTestCount() As Integer
            Get
                Dim myGlobal As GlobalDataTO = MyClass.GetElectrodeTestCount(ISE_Electrodes.Ref)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    RefTestCountAttr = CInt(myGlobal.SetDatos)
                Else
                    RefTestCountAttr = -1
                End If
                Return RefTestCountAttr
            End Get
            Set(ByVal value As Integer)
                RefTestCountAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.REF_CONSUMPTION, RefTestCountAttr.ToString)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property

        Private Property LiTestCount() As Integer
            Get
                Dim myGlobal As GlobalDataTO = MyClass.GetElectrodeTestCount(ISE_Electrodes.Li)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    LiTestCountAttr = CInt(myGlobal.SetDatos)
                Else
                    LiTestCountAttr = -1
                End If
                Return LiTestCountAttr
            End Get
            Set(ByVal value As Integer)
                LiTestCountAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.LI_CONSUMPTION, LiTestCountAttr.ToString)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property

        Private Property NaTestCount() As Integer
            Get
                Dim myGlobal As GlobalDataTO = MyClass.GetElectrodeTestCount(ISE_Electrodes.Na)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    NaTestCountAttr = CInt(myGlobal.SetDatos)
                Else
                    NaTestCountAttr = -1
                End If
                Return NaTestCountAttr
            End Get
            Set(ByVal value As Integer)
                NaTestCountAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.NA_CONSUMPTION, NaTestCountAttr.ToString)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property

        Private Property KTestCount() As Integer
            Get
                Dim myGlobal As GlobalDataTO = MyClass.GetElectrodeTestCount(ISE_Electrodes.K)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    KTestCountAttr = CInt(myGlobal.SetDatos)
                Else
                    KTestCountAttr = -1
                End If
                Return KTestCountAttr
            End Get
            Set(ByVal value As Integer)
                KTestCountAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.K_CONSUMPTION, KTestCountAttr.ToString)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property

        Private Property ClTestCount() As Integer
            Get
                Dim myGlobal As GlobalDataTO = MyClass.GetElectrodeTestCount(ISE_Electrodes.Cl)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    ClTestCountAttr = CInt(myGlobal.SetDatos)
                Else
                    ClTestCountAttr = -1
                End If
                Return ClTestCountAttr
            End Get
            Set(ByVal value As Integer)
                ClTestCountAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.CL_CONSUMPTION, ClTestCountAttr.ToString)
                If Not myGlobal.HasError Then
                    myGlobal = MyClass.UpdateISEInformationTable
                End If
            End Set
        End Property
#End Region

#Region "Tubings"



        Public Property PumpTubingInstallDate() As DateTime Implements IISEManager.PumpTubingInstallDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.PUMP_TUBING_INSTALL_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    PumpTubingInstallDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    ' XBC 29/06/2012
                    'Return Nothing
                    Return DateTime.Now
                    ' XBC 29/06/2012
                End If
                Return PumpTubingInstallDateAttr
            End Get
            Set(ByVal value As DateTime)
                PumpTubingInstallDateAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.PUMP_TUBING_INSTALL_DATE, PumpTubingInstallDateAttr.ToString, True)
                If Not myGlobal.HasError Then
                    Dim myParameter As SwParameters = SwParameters.ISE_EXPIRATION_TIME_PUMP_TUBING
                    myGlobal = MyClass.GetISEParameterValue(myParameter)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        Dim myExpirationTerm As Integer = CInt(myGlobal.SetDatos)
                        MyClass.PumpTubingExpireDate = PumpTubingInstallDateAttr.AddMonths(myExpirationTerm)
                        myGlobal = MyClass.UpdateISEInformationTable
                    End If
                End If
            End Set
        End Property

        Public Property FluidicTubingInstallDate() As DateTime Implements IISEManager.FluidicTubingInstallDate
            Get
                Dim myGlobal As New GlobalDataTO 'get from info DS
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.FLUID_TUBING_INSTALL_DATE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    FluidicTubingInstallDateAttr = CType(myGlobal.SetDatos, DateTime)
                Else
                    ' XBC 29/06/2012
                    'Return Nothing
                    Return DateTime.Now
                    ' XBC 29/06/2012
                End If
                Return FluidicTubingInstallDateAttr
            End Get
            Set(ByVal value As DateTime)
                FluidicTubingInstallDateAttr = value
                Dim myGlobal As New GlobalDataTO 'update info DS
                myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.FLUID_TUBING_INSTALL_DATE, FluidicTubingInstallDateAttr.ToString, True)
                If Not myGlobal.HasError Then
                    Dim myParameter As SwParameters = SwParameters.ISE_EXPIRATION_TIME_FLUID_TUBING
                    myGlobal = MyClass.GetISEParameterValue(myParameter)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        Dim myExpirationTerm As Integer = CInt(myGlobal.SetDatos)
                        MyClass.FluidicTubingExpireDate = FluidicTubingInstallDateAttr.AddMonths(myExpirationTerm)
                        myGlobal = MyClass.UpdateISEInformationTable
                    End If
                End If
            End Set
        End Property


        Public ReadOnly Property HasPumpTubingInstallDate() As Boolean Implements IISEManager.HasPumpTubingInstallDate
            Get
                Return (MyClass.PumpTubingInstallDate <> Nothing)
            End Get
        End Property


        Public ReadOnly Property HasFluidicTubingInstallDate() As Boolean Implements IISEManager.HasFluidicTubingInstallDate
            Get
                Return (MyClass.FluidicTubingInstallDate <> Nothing)
            End Get
        End Property


        Private Property PumpTubingExpireDate() As DateTime
            Get
                Return PumpTubingExpireDateAttr
            End Get
            Set(ByVal value As DateTime)
                PumpTubingExpireDateAttr = value
            End Set
        End Property

        Private Property FluidicTubingExpireDate() As DateTime
            Get
                Return FluidicTubingExpireDateAttr
            End Get
            Set(ByVal value As DateTime)
                FluidicTubingExpireDateAttr = value
            End Set
        End Property

#End Region

#Region "Other"

        'This property is updated every test completed
        Private Property TestsCountSinceLastClean() As Integer
            Get
                'Dim myglobal As New GlobalDataTO
                ''Dim myUtil As New Utilities.
                Dim myglobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.SAMPLES_SINCE_LAST_CLEAN)
                If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                    TestsCountSinceLastCleanAttr = CInt(myglobal.SetDatos)
                Else
                    TestsCountSinceLastCleanAttr = 0
                End If
                Return TestsCountSinceLastCleanAttr
            End Get
            Set(ByVal value As Integer)
                TestsCountSinceLastCleanAttr = value
                If TestsCountSinceLastCleanAttr >= 0 Then
                    Dim myGlobal As GlobalDataTO = MyClass.UpdateISEInfoSetting(ISEModuleSettings.SAMPLES_SINCE_LAST_CLEAN, value.ToString)
                    If Not myGlobal.HasError Then
                        ' XBC 28/06/2012
                        ' Check if ISE cycle clean is required
                        myGlobal = MyClass.CheckCleanIsNeeded
                        ' XBC 28/06/2012 
                        If Not myGlobal.HasError Then
                            myGlobal = MyClass.UpdateISEInformationTable
                        End If
                    End If
                End If
            End Set
        End Property

#End Region

#End Region


#End Region

#Region "Private Methods"

#Region "Common"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by XB 21/01/2013 - When a valid Reagent Pack is read, Page0 and Page1 must be refreshed with news values, and not set 
        '''                              to Nothing. Additionally, IsCleanPackInstalled change to FALSE (Bugs tracking #1108)
        '''             XB 30/09/2014 - Deactivate old timeout management - Remove too restrictive limitations because timeouts - BA-1872
        ''' </remarks>
        Private Function ManageLastISEResult() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If LastISEResult.ISEResultType <> ISEResultTO.ISEResultTypes.None Then

                    ' XB 30/09/2014 - BA-1872
                    'If MyClass.IsWaitingForInstructionStart Then

                    '    'START TIMEOUT 

                    '    'there are communications errors
                    '    MyClass.IsISECommsOk = False

                    '    'stop timeout timer
                    '    MyClass.StopInstructionStartedTimer()

                    '    'abort procedure because of communucations error, timeout
                    '    myGlobal = MyClass.AbortCurrentProcedureDueToException

                    '    'force ISE START Timeout error
                    '    RaiseEvent ISEProcedureFinished()
                    '    'MyClass.myAnalyzerManager.ManageAnalyzer(AnalyzerManagerSwActionList.ISE_RESULT_RECEIVED, False)

                    '    Exit Try

                    'Else
                    ' XB 30/09/2014 - BA-1872

                    If MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.ComError Then 'SGM 11/05/2012

                        'RESULT TIMEOUT
                        'MyClass.IsISECommsOk = False 'there are communications  ' XB 30/09/2014 - BA-1872
                        'error because of communucations error, timeout
                        myGlobal = MyClass.AbortCurrentProcedureDueToException
                        Exit Try

                    ElseIf MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.ERC Then

                        'ISE ERROR
                        MyClass.IsISECommsOk = True 'there are communications

                        'SGM 01/08/2012 - Set ISE as not initialized
                        If MyClass.myAnalyzer.AnalyzerStatus <> AnalyzerManagerStatus.RUNNING Then
                            If MyClass.LastISEResult.Errors(0).CancelErrorCode = ISEErrorTO.ISECancelErrorCodes.R Or _
                                MyClass.LastISEResult.Errors(0).CancelErrorCode = ISEErrorTO.ISECancelErrorCodes.N Then
                                'MyClass.IsISECommsOkAttr = False  ' XB 30/09/2014 - BA-1872
                                MyClass.IsISEInitiatedOKAttr = False
                                MyClass.IsCommErrorDetected = True
                            End If
                        Else
                            If MyClass.LastISEResult.Errors(0).CancelErrorCode = ISEErrorTO.ISECancelErrorCodes.N Then
                                'MyClass.IsISECommsOkAttr = False  ' XB 30/09/2014 - BA-1872
                                MyClass.IsISEInitiatedOKAttr = False
                                MyClass.IsCommErrorDetected = True
                            End If
                        End If
                        'end SGM 01/08/2012

                        myGlobal = MyClass.ManageISEProcedureFinished(ISEProcedureResult.CancelError)



                    ElseIf MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.SwError Then 'SGM 11/05/2012

                        'SW ERROR
                        MyClass.IsISECommsOk = True 'there are communications
                        'error because of software exception
                        myGlobal = MyClass.AbortCurrentProcedureDueToException
                        Exit Try


                    Else

                        'update consumptions
                        If MyClass.IsISEInitiatedOK Then
                            If Not myGlobal.HasError Then myGlobal = MyClass.UpdateConsumptions()

                            ' XBC 17/07/2012 
                            If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER OrElse _
                               MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN OrElse _
                               MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.CALB OrElse _
                               MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.BUBBLE_CAL OrElse _
                               MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.CLEAN OrElse _
                               MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.PURGEA OrElse _
                               MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.PURGEB OrElse _
                               MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.PRIME_CALA OrElse _
                               MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.PRIME_CALB OrElse _
                               MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.DSPA OrElse _
                               MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.DSPB Then
                                ' If there are perform any instruction which use liquides

                                If Not MyClass.WorkSessionIsRunningAttr Then
                                    ' Update date for the Last ISE operation
                                    MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_OPERATION_DATE, DateTime.Now.ToString, True)
                                Else
                                    ''Dim myLogAcciones As New ApplicationLogManager()    ' TO COMMENT !!!
                                    'GlobalBase.CreateLogActivity("Update Consumptions - Update Last Date WS ISE Operation [ " & DateTime.Now.ToString & "]", "ISEManager.Manage", EventLogEntryType.Information, False)   ' TO COMMENT !!!
                                    ' Update date for the ISE test executed while running
                                    MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_OPERATION_WS_DATE, DateTime.Now.ToString, True)
                                    ' Update date for the Last registered SIP cycles
                                    MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_OPERATION_DATE, DateTime.Now.ToString, True)
                                End If

                            End If
                            ' XBC 17/07/2012

                            If Not myGlobal.HasError Then myGlobal = MyClass.CheckElectrodesCalibrationIsNeeded()
                            If Not myGlobal.HasError Then myGlobal = MyClass.CheckPumpsCalibrationIsNeeded()
                            If Not myGlobal.HasError Then myGlobal = MyClass.CheckCleanIsNeeded()

                            'If Not myGlobal.HasError Then myGlobal = MyClass.ValidatePreparatiosAllowed()

                            If myGlobal.HasError Then
                                myGlobal = MyClass.AbortCurrentProcedureDueToException
                                Exit Try
                            End If

                        End If

                        'get data 
                        Select Case MyClass.LastISEResult.ISEResultType
                            Case ISEResultTO.ISEResultTypes.OK
                                'TODO

                            Case ISEResultTO.ISEResultTypes.SER
                                'TODO

                            Case ISEResultTO.ISEResultTypes.URN
                                'TODO

                            Case ISEResultTO.ISEResultTypes.CAL
                                'TODO

                            Case ISEResultTO.ISEResultTypes.PMC
                                'TODO

                            Case ISEResultTO.ISEResultTypes.BBC
                                'TODO

                                ' XBC 21/01/2013
                                'Case ISEResultTO.ISEResultTypes.DDT00
                                '    If Not MyClass.IsCleanPackInstalled Then
                                '        MyClass.ISEDallasSN = MyClass.LastISEResult.DallasSNData 'SGM 05/06/2012
                                '        MyClass.ISEDallasPage00 = MyClass.LastISEResult.DallasPage00Data
                                '    End If

                                'Case ISEResultTO.ISEResultTypes.DDT01
                                '    If Not MyClass.IsCleanPackInstalled Then
                                '        MyClass.ISEDallasPage01 = MyClass.LastISEResult.DallasPage01Data
                                '    End If
                            Case ISEResultTO.ISEResultTypes.DDT00
                                If Not MyClass.LastISEResult.DallasSNData Is Nothing And _
                                   Not MyClass.LastISEResult.DallasPage00Data Is Nothing Then
                                    MyClass.IsCleanPackInstalled = False
                                    MyClass.ISEDallasSN = MyClass.LastISEResult.DallasSNData 'SGM 05/06/2012
                                    MyClass.ISEDallasPage00 = MyClass.LastISEResult.DallasPage00Data
                                End If

                            Case ISEResultTO.ISEResultTypes.DDT01
                                If Not MyClass.LastISEResult.DallasPage01Data Is Nothing Then
                                    MyClass.ISEDallasPage01 = MyClass.LastISEResult.DallasPage01Data
                                End If
                                ' XBC 21/01/2013

                            Case ISEResultTO.ISEResultTypes.AMV, ISEResultTO.ISEResultTypes.BMV
                                'TODO

                            Case ISEResultTO.ISEResultTypes.ISV
                                'TODO

                        End Select

                        'in case that an ISE result has been received without requesting a Procedure we create a new one
                        If MyClass.CurrentCommandTO Is Nothing Then
                            Select Case MyClass.LastISEResult.ISEResultType
                                Case ISEResultTO.ISEResultTypes.ERC : MyClass.CurrentProcedure = ISEProcedures.SingleCommand
                                Case ISEResultTO.ISEResultTypes.SER : MyClass.CurrentProcedure = ISEProcedures.Test
                                Case ISEResultTO.ISEResultTypes.URN : MyClass.CurrentProcedure = ISEProcedures.Test
                                Case ISEResultTO.ISEResultTypes.CAL : MyClass.CurrentProcedure = ISEProcedures.CalibrateElectrodes
                                Case ISEResultTO.ISEResultTypes.PMC : MyClass.CurrentProcedure = ISEProcedures.CalibratePumps
                                Case ISEResultTO.ISEResultTypes.OK : MyClass.CurrentProcedure = ISEProcedures.SingleCommand

                            End Select



                        End If



                        'check if the current Procedure is finished and release an event if so
                        myGlobal = MyClass.ManageISEProcedureFinished()

                        'update Database data
                        If Not myGlobal.HasError Then
                            myGlobal = MyClass.UpdateISEInformationTable()
                        End If

                        'update ISE ready
                        If Not myGlobal.HasError Then
                            If MyClass.IsISEInitiatedOK Then
                                myGlobal = MyClass.UpdateISEModuleReady()
                            End If
                        End If

                        'refresh monitor data
                        If Not myGlobal.HasError Then
                            If MyClass.IsISEInitiatedOK Then
                                myGlobal = MyClass.RefreshMonitorDataTO
                            End If
                        End If


                    End If

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                MyClass.AbortCurrentProcedureDueToException()

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ManageLastISEResult", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  SGM 08/03/2012
        ''' Modified by SGM 16/01/2013 - Bug #1108
        '''             XB  30/04/2014 - Add protection on CALB operation if FW answers ISE! by error instead of CAL Results or ERC - Task #1614
        '''             XB  20/05/2014 - Add more protections against not expected answers from Firmware - Task #1614
        '''             XB  20/05/2014 - Fix Bug #1629
        '''             XB  12/06/2014 - Fix Bug caused by Task #1614 - ActivateReagentsPack
        '''             XB  19/11/2014 - Emplace the kind of errors derived of BA-1614 inside the management of timeouts and automatic instructions repetitions - BA-1872
        ''' </remarks>
        Private Function ManageISEProcedureFinished(Optional ByVal pForcedResult As ISEProcedureResult = ISEProcedureResult.None) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim isFinished As Boolean = False
                Dim ToValidateCalB As Boolean = False
                Dim ToValidatePumpCal As Boolean = False
                Dim ToValidateBubbleCal As Boolean = False

                FirmwareErrDetectedAttr = False     ' XB 19/11/2014 - BA-1872

                'catch last command of the Procedure
                Select Case MyClass.CurrentProcedure
                    Case ISEProcedures.Test
                        Select Case MyClass.LastISEResult.ISEResultType
                            Case ISEResultTO.ISEResultTypes.SER : isFinished = True : MyClass.TestsCountSinceLastClean += 1
                            Case ISEResultTO.ISEResultTypes.URN : isFinished = True : MyClass.TestsCountSinceLastClean += 1
                        End Select

                    Case ISEProcedures.SingleCommand
                        isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.OK)

                        If MyClass.CurrentCommandTO.ISECommandID = ISECommands.CLEAN Then
                            MyClass.LastCleanDate = DateTime.Now    ' XBC 04/09/2012 - Correction : Always save date (error or not)
                            MyClass.LastCleanError = ""             ' JBL 07/09/2012 - Correction : Always reset the last error
                            If isFinished Then
                                MyClass.IsCleanNeeded = False
                                MyClass.TestsCountSinceLastClean = 0
                                MyClass.IsCalibrationNeeded = True  ' XBC 26/06/2012 - Self-Maintenace

                                'JB 30/07/2012 - Insert Cleaning to ISE Historic
                                If myISECalibHistory IsNot Nothing Then
                                    'JB 20/09/2012 - Added LiEnabled to history
                                    myISECalibHistory.AddCleaning(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, LastCleanDateAttr, LastCleanErrorAttr)
                                End If
                                'JB 30/07/2012
                            ElseIf LastISEResult.IsCancelError Then
                                LastCleanError = LastISEResult.ReceivedResults
                                MyClass.IsCleanNeeded = True    ' JBL 04/09/2012 - Error: Clean neeeded
                                If myISECalibHistory IsNot Nothing Then
                                    'JB 20/09/2012 - Added LiEnabled to history
                                    myISECalibHistory.AddCleaning(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, LastCleanDateAttr, LastCleanErrorAttr)
                                End If
                            End If
                        End If


                    Case ISEProcedures.SingleReadCommand
                        Select Case MyClass.CurrentCommandTO.ISECommandID
                            Case ISECommands.CALB
                                isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL)
                                MyClass.LastElectrodesCalibrationDate = DateTime.Now    ' XBC 04/09/2012 - Correction : Always save date (error or not)
                                MyClass.LastElectrodesCalibrationResult1 = ""           ' JBL 05/09/2012 - Correction : Always reset last results
                                MyClass.LastElectrodesCalibrationResult2 = ""           ' JBL 05/09/2012 - Correction : Always reset last results
                                MyClass.LastElectrodesCalibrationError = ""             ' JBL 07/09/2012 - Correction : Always reset the last error
                                If isFinished Then
                                    ToValidateCalB = True
                                ElseIf pForcedResult = ISEProcedureResult.CancelError Then
                                    'JB 03/09/2012 - Set CALB Error and Insert to ISE Historic
                                    LastElectrodesCalibrationError = LastISEResult.ReceivedResults
                                    If myISECalibHistory IsNot Nothing Then
                                        'JB 20/09/2012 - Added LiEnabled to history
                                        myISECalibHistory.AddElectrodeCalibration(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, _
                                                                                  LastElectrodesCalibrationDateAttr, _
                                                                                  LastElectrodesCalibrationResult1Attr, LastElectrodesCalibrationResult2Attr, _
                                                                                  LastElectrodesCalibrationErrorAttr)
                                    End If

                                    ' XB 30/04/2014 - Task #1629
                                    MyClass.IsCalibrationNeeded = True

                                    ' XB 30/04/2014 - Task #1614
                                ElseIf LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                                    ' This is an error. ISE must answer a CAL results or an ERC, but no this instruction: <ISE!>

                                    ' XB 19/11/2014 - BA-1872
                                    FirmwareErrDetectedAttr = True

                                    'LastISEResult.IsCancelError = True
                                    'pForcedResult = ISEProcedureResult.Exception
                                    'MyClass.IsCalibrationNeeded = True
                                    ' XB 30/04/2014 - Task #1614
                                    ' XB 19/11/2014 - BA-1872
                                End If

                            Case ISECommands.LAST_SLOPES
                                isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL)

                            Case ISECommands.PUMP_CAL, ISECommands.SHOW_PUMP_CAL
                                isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.PMC)

                                If MyClass.CurrentCommandTO.ISECommandID = ISECommands.PUMP_CAL Then
                                    MyClass.LastPumpsCalibrationDate = DateTime.Now     ' XBC 04/09/2012 - Correction : Always save date (error or not)
                                    MyClass.LastPumpsCalibrationResult = ""             ' JBL 05/09/2012 - Correction : Always reset last result (error or not)
                                    MyClass.LastPumpsCalibrationError = ""              ' JBL 07/09/2012 - Correction : Always reset the last error
                                End If

                                If isFinished Then
                                    ToValidatePumpCal = True
                                ElseIf pForcedResult = ISEProcedureResult.CancelError Then
                                    'JB 03/09/2012 - Set Pump Error and Insert to ISE Historic
                                    LastPumpsCalibrationError = LastISEResult.ReceivedResults
                                    If myISECalibHistory IsNot Nothing Then
                                        'JB 20/09/2012 - Added LiEnabled to history
                                        myISECalibHistory.AddPumpCalibration(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, _
                                                                             LastPumpsCalibrationDateAttr, _
                                                                             LastPumpsCalibrationResultAttr, LastPumpsCalibrationErrorAttr)
                                    End If

                                    ' XB 30/04/2014 - Task #1629
                                    MyClass.IsPumpCalibrationNeeded = True

                                    ' XB 20/05/2014 - Task #1614
                                ElseIf LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                                    ' This is an error. ISE must answer results or an ERC, but no this instruction: <ISE!>

                                    ' XB 19/11/2014 - BA-1872
                                    FirmwareErrDetectedAttr = True

                                    'LastISEResult.IsCancelError = True
                                    'pForcedResult = ISEProcedureResult.Exception
                                    'MyClass.IsPumpCalibrationNeeded = True
                                    '' XB 20/05/2014 - Task #1614
                                    ' XB 19/11/2014 - BA-1872
                                End If
                            Case ISECommands.SHOW_PUMP_CAL
                                isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.PMC)

                            Case ISECommands.BUBBLE_CAL, ISECommands.SHOW_BUBBLE_CAL
                                isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BBC)

                                If MyClass.CurrentCommandTO.ISECommandID = ISECommands.BUBBLE_CAL Then
                                    MyClass.LastBubbleCalibrationDate = DateTime.Now    ' XBC 04/09/2012 - Correction : Always save date (error or not)
                                    MyClass.LastBubbleCalibrationResult = ""            ' JBL 05/09/2012 - Correction : Always reset last result (error or not)
                                    MyClass.LastBubbleCalibrationError = ""             ' JBL 07/09/2012 - Correction : Always reset the last error
                                End If

                                If isFinished Then
                                    ToValidateBubbleCal = True
                                ElseIf pForcedResult = ISEProcedureResult.CancelError Then
                                    'JB 27/08/2012 - Set Bubble Error and Insert to ISE Historic
                                    LastBubbleCalibrationError = LastISEResult.ReceivedResults
                                    If myISECalibHistory IsNot Nothing Then
                                        'JB 20/09/2012 - Added LiEnabled to history
                                        myISECalibHistory.AddBubbleCalibration(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, _
                                                                               LastBubbleCalibrationDateAttr, _
                                                                               LastBubbleCalibrationResultAttr, LastBubbleCalibrationErrorAttr)
                                    End If

                                    ' XB 30/04/2014 - Task #1629
                                    MyClass.IsBubbleCalibrationNeeded = True

                                    ' XB 20/05/2014 - Task #1614
                                ElseIf LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                                    ' This is an error. ISE must answer results or an ERC, but no this instruction: <ISE!>

                                    ' XB 19/11/2014 - BA-1872
                                    FirmwareErrDetectedAttr = True

                                    'LastISEResult.IsCancelError = True
                                    'pForcedResult = ISEProcedureResult.Exception
                                    ' XB 20/05/2014 - Task #1614
                                    ' XB 19/11/2014 - BA-1872
                                End If

                            Case ISECommands.READ_mV
                                isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.AMV) Or (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BMV)

                                ' XB 20/05/2014 - Task #1614
                                If Not isFinished Then
                                    If LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                                        ' This is an error. ISE must answer AMV or BMV or an ERC, but no this instruction: <ISE!>

                                        ' XB 19/11/2014 - BA-1872
                                        FirmwareErrDetectedAttr = True

                                        'LastISEResult.IsCancelError = True
                                        'pForcedResult = ISEProcedureResult.Exception
                                        ' XB 19/11/2014 - BA-1872

                                    End If
                                End If
                                ' XB 20/05/2014 - Task #1614

                            Case ISECommands.READ_PAGE_0_DALLAS
                                isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.DDT00)

                                ' XB 20/05/2014 - Task #1614
                                If Not isFinished Then
                                    If LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                                        ' This is an error. ISE must answer DDT00 but no this instruction: <ISE!>
                                        ' XB 19/11/2014 - BA-1872
                                        FirmwareErrDetectedAttr = True

                                        'LastISEResult.IsCancelError = True
                                        'pForcedResult = ISEProcedureResult.Exception
                                        ' XB 19/11/2014 - BA-1872

                                    End If
                                End If
                                ' XB 20/05/2014 - Task #1614

                            Case ISECommands.READ_PAGE_1_DALLAS
                                isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.DDT01)

                                ' XB 20/05/2014 - Task #1614
                                If Not isFinished Then
                                    If LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                                        ' This is an error. ISE must answer DDT01 but no this instruction: <ISE!>
                                        ' XB 19/11/2014 - BA-1872
                                        FirmwareErrDetectedAttr = True

                                        'LastISEResult.IsCancelError = True
                                        'pForcedResult = ISEProcedureResult.Exception
                                        ' XB 19/11/2014 - BA-1872
                                    End If
                                End If
                                ' XB 20/05/2014 - Task #1614

                            Case ISECommands.VERSION_CHECKSUM
                                isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.ISV)
                        End Select

                    Case ISEProcedures.GeneralCheckings
                        isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.READ_mV) And ((MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.AMV) Or (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BMV)))
                        'If Not MyClass.IsLongTermDeactivation Then
                        '    isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.READ_mV) And ((MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.AMV) Or (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BMV)))
                        'Else
                        '    isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.POLL) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.OK))
                        'End If

                        If isFinished Then

                            If Not MyClass.IsLongTermDeactivation Then
                                If Not MyClass.IsCleanPackInstalled Then
                                    MyClass.CheckReagentsPackReady()
                                    MyClass.CheckReagentsPackVolumeEnough()
                                    MyClass.InitializeReagentsConsumptions()
                                End If

                                MyClass.CheckElectrodesMounted()
                                MyClass.CheckElectrodesReady()

                                MyClass.IsISEInitializationDoneAttr = True
                                MyClass.IsISEInitiatedOKAttr = True
                                MyClass.IsISEOnceInitiatedOKAttr = True
                                MyClass.IsPendingToInitializeAfterActivation = False
                                RaiseEvent ISEConnectionFinished(True)

                                ' XBC 19/03/2012
                                If Not myGlobal.HasError Then myGlobal = MyClass.UpdateConsumptions()

                                If Not myGlobal.HasError Then myGlobal = MyClass.CheckElectrodesCalibrationIsNeeded()
                                If Not myGlobal.HasError Then myGlobal = MyClass.CheckPumpsCalibrationIsNeeded()
                                If Not myGlobal.HasError Then myGlobal = MyClass.CheckBubbleCalibrationIsNeeded
                                If Not myGlobal.HasError Then myGlobal = MyClass.CheckCleanIsNeeded()

                                'If Not myGlobal.HasError Then myGlobal = MyClass.ValidatePreparatiosAllowed()
                                ' XBC 19/03/2012

                                '' XBC 02/07/2012 - NO V1
                                'If Not myGlobal.HasError Then myGlobal = MyClass.checkReagentsSerialNumber()

                            Else

                                MyClass.IsISEInitializationDoneAttr = True
                                MyClass.IsISEOnceInitiatedOKAttr = True
                                MyClass.IsISEInitiatedOKAttr = True
                                MyClass.IsPendingToInitializeAfterActivation = False
                                RaiseEvent ISEConnectionFinished(True)

                            End If

                            MyClass.RefreshMonitorDataTO() 'SGM 06/06/2012

                        ElseIf pForcedResult = ISEProcedureResult.CancelError Or pForcedResult = ISEProcedureResult.Exception Then
                            MyClass.IsISEInitializationDoneAttr = True
                            MyClass.IsISEInitiatedOKAttr = False
                            RaiseEvent ISEConnectionFinished(False)

                            ' XB 20/05/2014 - Task #1614
                        ElseIf MyClass.CurrentCommandTO.ISECommandID = ISECommands.READ_mV AndAlso LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                            ' This is an error. ISE must answer a AMV or BMV or an ERC, but no this instruction: <ISE!>
                            ' XB 19/11/2014 - BA-1872
                            FirmwareErrDetectedAttr = True

                            'LastISEResult.IsCancelError = True
                            'pForcedResult = ISEProcedureResult.Exception
                            'MyClass.IsISEInitializationDoneAttr = True
                            'MyClass.IsISEInitiatedOKAttr = False
                            'RaiseEvent ISEConnectionFinished(False)
                            ' XB 20/05/2014 - Task #1614
                            ' XB 19/11/2014 - BA-1872
                        End If

                    Case ISEProcedures.ActivateModule
                        isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.READ_mV) And ((MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.AMV) Or (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BMV)))
                        If isFinished Then

                            MyClass.CheckReagentsPackReady()
                            MyClass.CheckReagentsPackVolumeEnough()

                            MyClass.CheckElectrodesMounted()
                            MyClass.CheckElectrodesReady()

                            MyClass.IsCleanPackInstalled = False

                            MyClass.IsISEInitializationDoneAttr = True

                        End If

                    Case ISEProcedures.ActivateReagentsPack
                        'SG 16/01/2012 - Bug #1108
                        isFinished = ((MyClass.ReagentsPackInstallationDate <> Nothing) And (MyClass.CurrentCommandTO.ISECommandID = ISECommands.READ_PAGE_1_DALLAS) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.DDT01))
                        If isFinished Then
                            MyClass.CheckReagentsPackReady()
                            MyClass.CheckReagentsPackVolumeEnough()
                        End If
                        'end SG 16/01/2012

                        'Commented SG 16/01/2012 - Bug #1108
                        'isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.READ_PAGE_1_DALLAS) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.DDT01))
                        'If isFinished Then
                        '    MyClass.CheckReagentsPackReady()
                        '    MyClass.CheckReagentsPackVolumeEnough()
                        'End If

                    Case ISEProcedures.CheckReagentsPack
                        isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.READ_PAGE_1_DALLAS) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.DDT01))
                        If isFinished Then
                            MyClass.CheckReagentsPackReady()
                            MyClass.CheckReagentsPackVolumeEnough()
                        End If

                    Case ISEProcedures.ActivateElectrodes
                        isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.READ_mV) And ((MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.AMV) Or (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BMV)))
                        If isFinished Then
                            'SGM 21/09/2012 - force wrong result in case of not mounted or installed
                            Dim resultOk As Boolean = False
                            myGlobal = MyClass.CheckElectrodesMounted()
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                resultOk = CBool(myGlobal.SetDatos)
                                myGlobal = MyClass.CheckElectrodesReady()
                                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                    resultOk = CBool(myGlobal.SetDatos)
                                End If
                            End If
                            If Not resultOk Then pForcedResult = ISEProcedureResult.NOK
                            'end SGM 21/09/2012
                        End If

                        ' XB 20/05/2014 - Task #1614
                        If Not isFinished Then
                            If LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                                ' This is an error. ISE must answer a AMV or BMV or an ERC, but no this instruction: <ISE!>
                                ' XB 19/11/2014 - BA-1872
                                FirmwareErrDetectedAttr = True

                                'LastISEResult.IsCancelError = True
                                'pForcedResult = ISEProcedureResult.Exception
                                ' XB 19/11/2014 - BA-1872
                            End If
                        End If
                        ' XB 20/05/2014 - Task #1614

                    Case ISEProcedures.CalibrateElectrodes
                        isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL)
                        MyClass.LastElectrodesCalibrationDate = DateTime.Now    ' XBC 04/09/2012 - Correction : Always save date (error or not)
                        MyClass.LastElectrodesCalibrationResult1 = ""           ' JBL 05/09/2012 - Correction : Always reset last result (error or not)
                        MyClass.LastElectrodesCalibrationResult2 = ""           ' JBL 05/09/2012 - Correction : Always reset last result (error or not)
                        MyClass.LastElectrodesCalibrationError = ""             ' JBL 07/09/2012 - Correction : Always reset last error
                        If isFinished Then
                            ToValidateCalB = True
                        ElseIf pForcedResult = ISEProcedureResult.CancelError Then
                            'JB 28/08/2012 - Set the Error and insert to ISE Historic
                            If LastISEResult.ReceivedResults.Contains("<ERC") Then
                                Dim pos As Integer = LastISEResult.ReceivedResults.IndexOf("<ERC")
                                LastElectrodesCalibrationError = LastISEResult.ReceivedResults.Substring(pos)
                                If myISECalibHistory IsNot Nothing Then
                                    'JB 20/09/2012 - Added LiEnabled to history
                                    myISECalibHistory.AddElectrodeCalibration(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, _
                                                                              LastElectrodesCalibrationDateAttr, _
                                                                              LastElectrodesCalibrationResult1Attr, LastElectrodesCalibrationResult2Attr, _
                                                                              LastElectrodesCalibrationErrorAttr)
                                End If
                            End If
                            'JB 28/08/2012 - End

                            ' XB 30/04/2014 - Task #1629
                            MyClass.IsCalibrationNeeded = True

                            ' XB 30/04/2014 - Task #1614
                        ElseIf LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                            ' This is an error. ISE must answer a CAL results or an ERC, but no this instruction: <ISE!>
                            ' XB 19/11/2014 - BA-1872
                            FirmwareErrDetectedAttr = True

                            'LastISEResult.IsCancelError = True
                            'pForcedResult = ISEProcedureResult.Exception
                            'MyClass.IsCalibrationNeeded = True
                            ' XB 30/04/2014 - Task #1614
                            ' XB 19/11/2014 - BA-1872
                        End If

                    Case ISEProcedures.CalibratePumps
                        isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.PMC)
                        MyClass.LastPumpsCalibrationDate = DateTime.Now     ' XBC 04/09/2012 - Correction : Always save date (error or not)
                        MyClass.LastPumpsCalibrationResult = ""             ' JBL 05/09/2012 - Correction : Always reset last result (error or not)
                        MyClass.LastPumpsCalibrationError = ""              ' JBL 07/09/2012 - Correction : Always reset last error
                        If isFinished Then
                            ToValidatePumpCal = True
                        ElseIf pForcedResult = ISEProcedureResult.CancelError Then
                            'JB 27/08/2012 - Set Pump Error and Insert to ISE Historic
                            LastPumpsCalibrationError = LastISEResult.ReceivedResults
                            If myISECalibHistory IsNot Nothing Then
                                'JB 20/09/2012 - Added LiEnabled to history
                                myISECalibHistory.AddPumpCalibration(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, _
                                                                     LastPumpsCalibrationDateAttr, _
                                                                     LastPumpsCalibrationResultAttr, LastPumpsCalibrationErrorAttr)
                            End If

                            ' XB 30/04/2014 - Task #1629
                            MyClass.IsPumpCalibrationNeeded = True

                            ' XB 20/05/2014 - Task #1614
                        ElseIf LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                            ' This is an error. ISE must answer results or an ERC, but no this instruction: <ISE!>
                            ' XB 19/11/2014 - BA-1872
                            FirmwareErrDetectedAttr = True

                            'LastISEResult.IsCancelError = True
                            'pForcedResult = ISEProcedureResult.Exception
                            'MyClass.IsPumpCalibrationNeeded = True
                            ' XB 20/05/2014 - Task #1614
                            ' XB 19/11/2014 - BA-1872
                        End If

                        ' XBC 27/09/2012
                    Case ISEProcedures.CalibrateBubbles
                        isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BBC)
                        MyClass.LastBubbleCalibrationDate = DateTime.Now
                        MyClass.LastBubbleCalibrationResult = ""
                        MyClass.LastBubbleCalibrationError = ""
                        If isFinished Then
                            ToValidateBubbleCal = True
                        ElseIf pForcedResult = ISEProcedureResult.CancelError Then
                            LastBubbleCalibrationError = LastISEResult.ReceivedResults
                            If myISECalibHistory IsNot Nothing Then
                                myISECalibHistory.AddBubbleCalibration(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, _
                                                                       LastBubbleCalibrationDateAttr, _
                                                                       LastBubbleCalibrationResultAttr, LastBubbleCalibrationErrorAttr)

                            End If

                            ' XB 30/04/2014 - Task #1629
                            MyClass.IsBubbleCalibrationNeeded = True

                            ' XB 20/05/2014 - Task #1614
                        ElseIf LastISEResult.ReceivedResults.Contains("<ISE!>") Then
                            ' This is an error. ISE must answer results or an ERC, but no this instruction: <ISE!>
                            ' XB 19/11/2014 - BA-1872
                            FirmwareErrDetectedAttr = True

                            'LastISEResult.IsCancelError = True
                            'pForcedResult = ISEProcedureResult.Exception
                            ' XB 19/11/2014 - BA-1872
                            ' XB 20/05/2014 - Task #1614
                        End If

                    Case ISEProcedures.Clean
                        'isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.CALB) And ((MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL) Or (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.BMV))) ''PDT to optimization: After Clean -> calB
                        isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.CLEAN) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.OK))
                        MyClass.LastCleanDate = DateTime.Now                ' XBC 04/09/2012 - Correction : Always save date (error or not)
                        MyClass.LastCleanError = ""                         ' JBL 07/09/2012 - Correction : Always reset last error
                        If isFinished Then
                            MyClass.IsCleanNeeded = False
                            'MyClass.LastCleanDate = DateTime.Now    ' XBC 04/09/2012 - Correction : Always save date (error or not)
                            MyClass.TestsCountSinceLastClean = 0
                            'ToValidateCalB = True
                            MyClass.IsCalibrationNeeded = True  ' XBC 26/06/2012 - Self-Maintenace
                        End If

                        'JB 02/08/2012 - Set the Error
                        If LastISEResult.IsCancelError Then
                            LastCleanError = LastISEResult.ReceivedResults
                        End If
                        'JB 02/08/2012 - End

                        'JB 30/07/2012 - Insert Cleaning to ISE Historic
                        If myISECalibHistory IsNot Nothing Then
                            'JB 20/09/2012 - Added LiEnabled to history
                            myISECalibHistory.AddCleaning(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, LastCleanDateAttr, LastCleanErrorAttr)
                        End If
                        'JB 30/07/2012

                    Case ISEProcedures.MaintenanceExit
                        isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.PURGEB) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.OK))

                    Case ISEProcedures.CheckCleanPackInstalled
                        isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.PURGEB) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.OK))
                        If isFinished Then
                            MyClass.IsCleanPackInstalled = True
                        End If

                        ' XBC 04/04/2012
                        'Case ISEProcedures.WriteCalAConsumption
                        '    isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.WRITE_DALLAS) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.OK))
                        '    If isFinished Then
                        '        MyClass.IsCalAUpdateRequiredAttr = False
                        '        ' update counter
                        '        MyClass.CountConsumptionToSaveDallasData_CalA -= MyClass.MinConsumptionVolToSaveDallasData_CalA
                        '    End If

                        'Case ISEProcedures.WriteCalBConsumption
                        '    isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.WRITE_DALLAS) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.OK))
                        '    If isFinished Then
                        '        MyClass.IsCalBUpdateRequiredAttr = False
                        '        ' update counter
                        '        MyClass.CountConsumptionToSaveDallasData_CalB -= MyClass.MinConsumptionVolToSaveDallasData_CalB
                        '    End If

                    Case ISEProcedures.WriteConsumption
                        If MyClass.IsCalBUpdateRequiredAttr Then
                            isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.WRITE_CALB_CONSUMPTION) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.OK))
                            If isFinished Then
                                If MyClass.IsCalBUpdateRequiredAttr Then
                                    MyClass.IsCalBUpdateRequiredAttr = False
                                    ' update counter Cal B
                                    MyClass.CountConsumptionToSaveDallasData_CalB -= MyClass.MinConsumptionVolToSaveDallasData_CalB
                                End If

                                If MyClass.IsCalAUpdateRequiredAttr Then
                                    MyClass.IsCalAUpdateRequiredAttr = False
                                    ' update counter Cal A
                                    MyClass.CountConsumptionToSaveDallasData_CalA -= MyClass.MinConsumptionVolToSaveDallasData_CalA
                                End If
                            End If
                        Else
                            isFinished = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.WRITE_CALA_CONSUMPTION) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.OK))
                            If isFinished Then
                                If MyClass.IsCalAUpdateRequiredAttr Then
                                    MyClass.IsCalAUpdateRequiredAttr = False
                                    ' update counter Cal A
                                    MyClass.CountConsumptionToSaveDallasData_CalA -= MyClass.MinConsumptionVolToSaveDallasData_CalA
                                End If
                            End If
                        End If

                    Case ISEProcedures.PrimeAndCalibration 'SGM 11/06/2012
                        isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL)
                        If isFinished Then
                            ToValidateCalB = True
                        End If

                    Case ISEProcedures.PrimeX2AndCalibration 'SGM 11/06/2012
                        isFinished = (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.CAL)
                        If isFinished Then
                            ToValidateCalB = True
                        End If

                    Case Else
                        'Debug.Print("Caso NONE !")
                        ' XBC 04/04/2012

                End Select

                'Validation of calibrations
                If ToValidateCalB Then
                    Dim myCal1OK As Boolean = False
                    Dim myCal2OK As Boolean = False
                    MyClass.LastElectrodesCalibrationDate = DateTime.Now    ' JBL 07/09/2012 - Correction : Always save date (error or not)
                    MyClass.LastElectrodesCalibrationResult1 = ""           ' JBL 30/07/2012 Initialization (for history)
                    MyClass.LastElectrodesCalibrationResult2 = ""           ' JBL 30/07/2012 Initialization (for history)
                    MyClass.LastElectrodesCalibrationError = ""             ' JBL 02/08/2012 Initialization
                    'JB 02/08/2012 - Set the Error
                    If LastISEResult.IsCancelError Then
                        Dim pos As Integer = LastISEResult.ReceivedResults.IndexOf("<ERC")
                        LastElectrodesCalibrationError = LastISEResult.ReceivedResults.Substring(pos)
                    End If
                    'JB 02/08/2012 - End
                    Dim myResults() As String = MyClass.LastISEResult.ReceivedResults.Split(CChar("<"))
                    If myResults.Count >= 3 Then
                        MyClass.LastElectrodesCalibrationResult1 = myResults(1).Trim.Replace(">", "")
                        MyClass.LastElectrodesCalibrationResult2 = myResults(2).Trim.Replace(">", "")
                        myGlobal = MyClass.ValidateElectrodesCalibration(myResults(1))
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            myCal1OK = CBool(myGlobal.SetDatos)
                            myGlobal = MyClass.ValidateElectrodesCalibration(myResults(2))
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                myCal2OK = CBool(myGlobal.SetDatos)
                            End If
                        End If
                    End If

                    'JB 30/07/2012 - Insert Electrode calibration to ISE Historic
                    If myISECalibHistory IsNot Nothing Then
                        'JB 20/09/2012 - Added LiEnabled to history
                        myISECalibHistory.AddElectrodeCalibration(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, _
                                                                  LastElectrodesCalibrationDateAttr, _
                                                                  LastElectrodesCalibrationResult1Attr, LastElectrodesCalibrationResult2Attr, _
                                                                  LastElectrodesCalibrationErrorAttr)
                    End If
                    'JB 30/07/2012 - End

                    'JB 03/09/2012 - IsCalibrationNeeded also when IsCancelError 
                    If LastISEResult.IsCancelError Then
                        pForcedResult = ISEProcedureResult.CancelError
                        MyClass.IsCalibrationNeeded = True
                    ElseIf LastISEResult.Errors.Count > 0 Then 'SGM 09/11/2012
                        pForcedResult = ISEProcedureResult.NOK
                        MyClass.IsCalibrationNeeded = True
                    ElseIf (Not myCal1OK Or Not myCal2OK) Then
                        pForcedResult = ISEProcedureResult.NOK
                        MyClass.IsCalibrationNeeded = True
                    Else
                        MyClass.IsCalibrationNeeded = False
                    End If
                    'MyClass.IsCalibrationNeeded = Not myCal1OK Or Not myCal2OK
                    'JB 03/09/2012 - End

                End If

                If ToValidatePumpCal Then
                    Dim myCalOK As Boolean = False
                    MyClass.LastPumpsCalibrationResult = MyClass.LastISEResult.ReceivedResults
                    MyClass.LastPumpsCalibrationDate = DateTime.Now
                    MyClass.LastPumpsCalibrationError = ""                  ' JBL 07/09/2012 - Correction : Always reset last error

                    myGlobal = MyClass.ValidatePumpsCalibration(MyClass.LastISEResult.ReceivedResults)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myCalOK = CBool(myGlobal.SetDatos)
                        'MyClass.IsPumpCalibrationNeeded = Not myCalOK 'JB 03/09/2012: At the end of this block
                    End If


                    'JB 28/08/2012 - Set the Error (only ERC string), and update the Result without ERC string
                    'LastPumpsCalibrationError = ""
                    If LastISEResult.IsCancelError Then
                        Dim pos As Integer = LastISEResult.ReceivedResults.IndexOf("<ERC")
                        LastPumpsCalibrationError = LastISEResult.ReceivedResults.Substring(pos)
                        LastPumpsCalibrationResult = LastPumpsCalibrationResult.Replace(LastPumpsCalibrationError, "")
                    End If
                    'JB 28/08/2012 - End

                    'JB 30/07/2012 - Insert Pump calibration to ISE Historic
                    If myISECalibHistory IsNot Nothing Then
                        'JB 20/09/2012 - Added LiEnabled to history
                        myISECalibHistory.AddPumpCalibration(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, _
                                                             LastPumpsCalibrationDateAttr, _
                                                             LastPumpsCalibrationResultAttr, LastPumpsCalibrationErrorAttr)
                    End If
                    'JB 30/07/2012 - End

                    'JB 03/09/2012 - IsPumpCalibrationNeeded also when IsCancelError 
                    If LastISEResult.IsCancelError Then
                        pForcedResult = ISEProcedureResult.CancelError
                        MyClass.IsPumpCalibrationNeeded = True
                    ElseIf (Not myCalOK) Then
                        pForcedResult = ISEProcedureResult.NOK
                        MyClass.IsPumpCalibrationNeeded = True
                    Else
                        MyClass.IsPumpCalibrationNeeded = False
                    End If
                    'If Not myCalOK Then pForcedResult = ISEProcedureResult.NOK
                    'JB 03/09/2012 - End
                End If

                ' XBC 26/07/2012
                If ToValidateBubbleCal Then
                    Dim myCalOK As Boolean = False
                    MyClass.LastBubbleCalibrationResult = MyClass.LastISEResult.ReceivedResults
                    MyClass.LastBubbleCalibrationDate = DateTime.Now
                    MyClass.LastBubbleCalibrationError = ""

                    myGlobal = MyClass.ValidateBubbleCalibration(MyClass.LastISEResult.ReceivedResults)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myCalOK = CBool(myGlobal.SetDatos)
                        MyClass.IsBubbleCalibrationNeeded = Not myCalOK
                    End If


                    'JB 02/08/2012 - Set the Error
                    'LastBubbleCalibrationError = ""
                    If LastISEResult.IsCancelError Then
                        LastBubbleCalibrationError = LastISEResult.ReceivedResults
                    End If
                    'JB 02/08/2012 - End

                    'JB 30/07/2012 - Insert Bubble calibration to ISE Historic
                    If myISECalibHistory IsNot Nothing Then
                        'JB 20/09/2012 - Added LiEnabled to history
                        myISECalibHistory.AddBubbleCalibration(Nothing, AnalyzerIDAttr, IsLiEnabledByUser, _
                                                               LastBubbleCalibrationDateAttr, _
                                                               LastBubbleCalibrationResultAttr, LastBubbleCalibrationErrorAttr)
                    End If
                    'JB 30/07/2012

                    If Not myCalOK Then pForcedResult = ISEProcedureResult.NOK
                End If



                If MyClass.CurrentProcedure <> ISEProcedures.None Then
                    If isFinished Or (pForcedResult <> ISEProcedureResult.None) Then

                        If pForcedResult <> ISEProcedureResult.None Then
                            MyClass.LastProcedureResultAttr = pForcedResult
                        Else
                            MyClass.LastProcedureResultAttr = ISEProcedureResult.OK
                        End If

                        RaiseEvent ISEProcedureFinished()

                        'MyClass.LastProcedureResultAttr = ISEProcedureResult.None

                        ' XBC 10/04/2012
                        ''SGM 23/03/2012 Only write consumptions to Dallas while Standby
                        'If MyClass.myAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                        '    If MyClass.IsCalAUpdateRequired Then 'update cal A consumption
                        '        MyClass.CurrentProcedure = ISEProcedures.WriteConsumption
                        '        ' Save it into Dallas Xip
                        '        myGlobal = SaveConsumptionCalToDallasData("A")
                        '        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        '            MyClass.CurrentCommandTO = CType(myGlobal.SetDatos, ISECommandTO)

                        '            MyClass.SendISECommand()
                        '        End If
                        '    ElseIf MyClass.IsCalBUpdateRequired Then 'update calB consumption
                        '        MyClass.CurrentProcedure = ISEProcedures.WriteConsumption
                        '        ' Save it into Dallas Xip
                        '        myGlobal = SaveConsumptionCalToDallasData("B")
                        '        If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
                        '            MyClass.CurrentCommandTO = CType(myGlobal.SetDatos, ISECommandTO)

                        '            MyClass.SendISECommand()
                        '        End If

                        '    End If
                        'End If
                        ' XBC 10/04/2012

                    End If

                    MyClass.UpdateISEModuleReady()

                End If

                ' XBC 27/08/2012 - Correction : Save consumptions after Current Procedure is finished
                CurrentProcedureIsFinishedAttr = isFinished

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                MyClass.AbortCurrentProcedureDueToException()

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ManageISEProcedureFinished", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Cancel the current Procedure
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by XB 30/09/2014 - Add parameter isTimeoutException - Remove too restrictive limitations because timeouts - BA-1872
        ''' </remarks>
        Public Function AbortCurrentProcedureDueToException(Optional ByVal isTimeoutException As Boolean = False) As GlobalDataTO Implements IISEManager.AbortCurrentProcedureDueToException
            Dim myGlobal As New GlobalDataTO
            Try
                If Not isTimeoutException AndAlso MyClass.CurrentProcedure = ISEProcedures.GeneralCheckings Then
                    MyClass.IsISEInitializationDoneAttr = True
                    MyClass.IsISEInitiatedOKAttr = False
                    RaiseEvent ISEConnectionFinished(False)
                End If

                If MyClass.CurrentProcedure <> ISEProcedures.None Then

                    If Not isTimeoutException Then
                        MyClass.LastProcedureResultAttr = ISEProcedureResult.Exception

                        RaiseEvent ISEProcedureFinished()
                    End If

                    'MyClass.LastProcedureResultAttr = ISEProcedureResult.None
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.AbortCurrentProcedureDueToException", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Gets all the necessary data from database
        ''' Only invoked in constructor and after loading a Report SAT
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by XB 30/09/2014 - Deactivate old timeout management - Remove too restrictive limitations because timeouts - BA-1872
        ''' </remarks>
        Public Function RefreshAllDatabaseInformation() As GlobalDataTO Implements IISEManager.RefreshAllDatabaseInformation
            Dim myGlobal As New GlobalDataTO
            Try
                myGlobal = MyClass.LoadISEParameters()
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    myGlobal = MyClass.LoadISELimits()

                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myGlobal = MyClass.ReadISEInformationTable()
                        ' XBC 22/03/2012
                        'MyClass.MonitorDataTOAttr = New ISEMonitorTO
                        ' XBC 22/03/2012
                    End If

                    ' XB 30/09/2014 - BA-1872
                    ''SGM 02/07/2012
                    'myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_CMD_TIMEOUT)
                    'If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    '    MyClass.InstructionStartedTimeout = CInt(myGlobal.SetDatos)
                    'End If
                    ''SGM 02/07/2012
                    ' XB 30/09/2014 - BA-1872

                    ' XBC 18/07/2012
                    'get parameters
                    myGlobal = MyClass.GetConsumptionParameters()


                    ' Get fixed parameter values from database
                    myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_SIP_INTERVAL, False)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        SIPIntervalConsumption = CType(myGlobal.SetDatos, Single) * 60 ' convert to seconds
                    End If
                    ' Get fixed interval 1 (xx time no ISE activity) value for Firmware
                    myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_PUGAbyFW_TIME1, False)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        Interval1forPurgeACompletedbyFW = CType(myGlobal.SetDatos, Single) * 60 ' convert to seconds
                    End If
                    ' Get fixed interval 2 (each ISE test since xx time no ISE activity) value for Firmware
                    myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_PUGAbyFW_TIME2, False)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        Interval2forPurgeACompletedbyFW = CType(myGlobal.SetDatos, Single) * 60 ' convert to seconds
                    End If

                    Dim cycleMachineTime As Single = 0
                    Dim myParametersDS As New ParametersDS
                    Dim mySWParametersDelegate As New SwParametersDelegate

                    'Get time needed for each Analyzer Cycle
                    myGlobal = mySWParametersDelegate.GetParameterByAnalyzer(Nothing, MyClass.AnalyzerIDAttr, SwParameters.CYCLE_MACHINE.ToString, True)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        myParametersDS = DirectCast(myGlobal.SetDatos, ParametersDS)
                        If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then cycleMachineTime = Convert.ToSingle(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                    End If

                    myGlobal = mySWParametersDelegate.GetParameterByAnalyzer(Nothing, MyClass.AnalyzerIDAttr, SwParameters.ISE_EXECUTION_TIME_SER.ToString, True)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        myParametersDS = DirectCast(myGlobal.SetDatos, ParametersDS)
                        If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                            ISE_EXECUTION_TIME_SER = Convert.ToInt32(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                        End If
                        'Calculate the time
                        ISE_EXECUTION_TIME_SER = (ISE_EXECUTION_TIME_SER * cycleMachineTime) ' seconds
                    End If

                    myGlobal = mySWParametersDelegate.GetParameterByAnalyzer(Nothing, MyClass.AnalyzerIDAttr, SwParameters.ISE_EXECUTION_TIME_URI.ToString, True)
                    If (Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing) Then
                        myParametersDS = DirectCast(myGlobal.SetDatos, ParametersDS)
                        If (myParametersDS.tfmwSwParameters.Rows.Count > 0) Then
                            ISE_EXECUTION_TIME_URI = Convert.ToInt32(myParametersDS.tfmwSwParameters.First.ValueNumeric)
                        End If
                        'Calculate the time
                        ISE_EXECUTION_TIME_URI = (ISE_EXECUTION_TIME_URI * cycleMachineTime) ' seconds
                    End If
                    ' XBC 18/07/2012

                End If

                ' XBC 26/03/2012
                ReDim myAlarms(11)
                myAlarms(Alarms.ReagentsPack_Invalid) = False
                myAlarms(Alarms.ReagentsPack_Depleted) = False
                myAlarms(Alarms.ReagentsPack_Expired) = False
                myAlarms(Alarms.Electrodes_Wrong) = False
                myAlarms(Alarms.Electrodes_Cons_Expired) = False
                myAlarms(Alarms.Electrodes_Date_Expired) = False
                myAlarms(Alarms.CleanPack_Installed) = False
                myAlarms(Alarms.CleanPack_Wrong) = False
                myAlarms(Alarms.LongTermDeactivated) = False
                myAlarms(Alarms.ReagentsPack_DateInstall) = False
                myAlarms(Alarms.Switch_Off) = False 'SGM 18/06/2012
                myAlarms(Alarms.Timeout) = False ' XB 04/11/2014 - BA-1872
                ' XBC 26/03/2012



            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.RefreshAllDatabaseInformation", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Refreshes the current Monitor Data
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by XB 23/05/2014 - BT #1639 - Display Calibrations Expired Data times
        ''' </remarks>
        Public Function RefreshMonitorDataTO() As GlobalDataTO Implements IISEManager.RefreshMonitorDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If Not MyClass.IsISEModuleInstalled Or MyClass.IsLongTermDeactivation Then
                    MyClass.MonitorDataTOAttr = Nothing
                    Return myGlobal 'not to show anything in case of not installed
                End If


                Dim isError As Boolean = False

                Dim mySafetyVolA As Single
                myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_MIN_SAFETY_VOLUME_CAL_A)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    mySafetyVolA = CSng(myGlobal.SetDatos)
                End If

                Dim mySafetyVolB As Single
                myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_MIN_SAFETY_VOLUME_CAL_B)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    mySafetyVolB = CSng(myGlobal.SetDatos)
                End If


                'ReagentsPack:
                Dim myRPInstallDate As DateTime = MyClass.ReagentsPackInstallationDate
                Dim myRPExpireDate As DateTime = MyClass.ReagentsPackExpirationDate
                Dim myRPIsExpired As Boolean = (MyClass.ReagentsPackExpirationDate < Now) And Not (MyClass.ReagentsPackExpirationDate = Nothing)
                Dim myRPRemVolA As Single = MyClass.ReagentsPackRemainingVolCalA
                Dim myRPRemVolB As Single = MyClass.ReagentsPackRemainingVolCalB
                Dim myRPInitVolA As Single = MyClass.ReagentsPackInitialVolCalA
                Dim myRPInitVolB As Single = MyClass.ReagentsPackInitialVolCalB
                Dim myRPEnoughA As Boolean = MyClass.IsEnoughCalibratorA
                Dim myRPEnoughB As Boolean = MyClass.IsEnoughCalibratorB


                Dim myRPHasBioCode As Boolean

                'SGM 02/10/2012 - DL 10/10/2012 B&T 826
                If MyClass.IsISEInitiatedOK Then
                    myRPHasBioCode = (MyClass.IsCleanPackInstalled OrElse MyClass.HasBiosystemsCode)
                Else
                    myRPHasBioCode = True
                End If
                'SGM 02/10/2012 - DL 10/10/2012 B&T 826

                'Electrodes:
                Dim myRefData As ISEMonitorTO.ElectrodeData = New ISEMonitorTO.ElectrodeData(ISE_Electrodes.Ref)
                Dim myLiData As ISEMonitorTO.ElectrodeData = New ISEMonitorTO.ElectrodeData(ISE_Electrodes.Li)
                Dim myNaData As ISEMonitorTO.ElectrodeData = New ISEMonitorTO.ElectrodeData(ISE_Electrodes.Na)
                Dim myKData As ISEMonitorTO.ElectrodeData = New ISEMonitorTO.ElectrodeData(ISE_Electrodes.K)
                Dim myClData As ISEMonitorTO.ElectrodeData = New ISEMonitorTO.ElectrodeData(ISE_Electrodes.Cl)
                Dim myLithiumEnabled As Boolean = MyClass.IsLiEnabledByUser

                With myRefData
                    If MyClass.IsISEInitiatedOK Then
                        .Installed = MyClass.IsRefInstalled
                    Else
                        .Installed = MyClass.HasRefInstallDate
                    End If
                    .InstallationDate = MyClass.RefInstallDate
                    .IsExpired = MyClass.IsRefExpired
                    .IsOverUsed = MyClass.IsRefOverUsed
                    .TestCount = MyClass.RefTestCount
                End With

                With myLiData
                    If MyClass.IsISEInitiatedOK Then
                        .Installed = MyClass.IsLiInstalled And MyClass.IsLiEnabledByUser
                    Else
                        .Installed = MyClass.HasLiInstallDate And MyClass.IsLiEnabledByUser
                    End If
                    .InstallationDate = MyClass.LiInstallDate
                    .IsExpired = MyClass.IsLiExpired
                    .IsOverUsed = MyClass.IsLiOverUsed
                    .TestCount = MyClass.LiTestCount
                End With

                With myNaData
                    If MyClass.IsISEInitiatedOK Then
                        .Installed = MyClass.IsNaInstalled
                    Else
                        .Installed = MyClass.HasNaInstallDate
                    End If
                    .InstallationDate = MyClass.NaInstallDate
                    .IsExpired = MyClass.IsNaExpired
                    .IsOverUsed = MyClass.IsNaOverUsed
                    .TestCount = MyClass.NaTestCount
                End With

                With myKData
                    If MyClass.IsISEInitiatedOK Then
                        .Installed = MyClass.IsKInstalled
                    Else
                        .Installed = MyClass.HasKInstallDate
                    End If
                    .InstallationDate = MyClass.KInstallDate
                    .IsExpired = MyClass.IsKExpired
                    .IsOverUsed = MyClass.IsKOverUsed
                    .TestCount = MyClass.KTestCount
                End With

                With myClData
                    If MyClass.IsISEInitiatedOK Then
                        .Installed = MyClass.IsClInstalled
                    Else
                        .Installed = MyClass.HasClInstallDate
                    End If
                    .InstallationDate = MyClass.ClInstallDate
                    .IsExpired = MyClass.IsClExpired
                    .IsOverUsed = MyClass.IsClOverUsed
                    .TestCount = MyClass.ClTestCount
                End With


                'Calibrations:

                Dim myCalElectroDate As DateTime = MyClass.LastElectrodesCalibrationDate
                Dim myCalElectroString1 As String = MyClass.LastElectrodesCalibrationResult1
                Dim myCalElectroString2 As String = MyClass.LastElectrodesCalibrationResult2
                Dim myCalElectroRecomm As Boolean = MyClass.IsCalibrationNeeded
                Dim myCalElectroResult1OK As Boolean = False
                Dim myCalElectroResult2OK As Boolean = False
                If MyClass.LastElectrodesCalibrationDate <> Nothing AndAlso _
                   MyClass.LastElectrodesCalibrationResult1 <> Nothing AndAlso _
                   MyClass.LastElectrodesCalibrationResult2 <> Nothing Then
                    myGlobal = MyClass.ValidateElectrodesCalibration(MyClass.LastElectrodesCalibrationResult1)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        myCalElectroResult1OK = CBool(myGlobal.SetDatos)
                    Else
                        isError = True
                    End If
                    myGlobal = MyClass.ValidateElectrodesCalibration(MyClass.LastElectrodesCalibrationResult2)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        myCalElectroResult2OK = CBool(myGlobal.SetDatos)
                    Else
                        isError = True
                    End If
                End If

                ' XB 26/05/2014 - BT #1639
                Dim myCalElectrodesExpiredTime As Integer = 4
                myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_CALB_HOURS_NEEDED)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    myCalElectrodesExpiredTime = CInt(myGlobal.SetDatos)
                End If

                Dim myCalPumpsdate As DateTime = MyClass.LastPumpsCalibrationDate
                Dim myCalPumpsString As String = MyClass.LastPumpsCalibrationResult
                Dim myCalPumpsRecomm As Boolean = MyClass.IsPumpCalibrationNeeded
                Dim myCalPumpsResultOK As Boolean
                If MyClass.LastPumpsCalibrationDate <> Nothing AndAlso _
                   MyClass.LastPumpsCalibrationResult <> Nothing Then
                    myGlobal = MyClass.ValidatePumpsCalibration(MyClass.LastPumpsCalibrationResult)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        myCalPumpsResultOK = CBool(myGlobal.SetDatos)
                    Else
                        isError = True
                    End If
                End If

                ' XB 26/05/2014 - BT #1639
                Dim myCalPumpsExpiredTime As Integer = 24
                myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_PUMPCAL_HOURS_NEEDED)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    myCalPumpsExpiredTime = CInt(myGlobal.SetDatos)
                End If

                'SGM 25/07/2012 Bubble detector
                Dim myCalBubbledate As DateTime = MyClass.LastBubbleCalibrationDate
                Dim myCalBubbleString As String = MyClass.LastBubbleCalibrationResult
                Dim myCalBubbleRecomm As Boolean = MyClass.IsBubbleCalibrationNeeded
                Dim myCalBubbleResultOK As Boolean
                If MyClass.LastBubbleCalibrationDate <> Nothing AndAlso _
                   MyClass.LastBubbleCalibrationResult <> Nothing Then
                    myGlobal = MyClass.ValidateBubbleCalibration(MyClass.LastBubbleCalibrationResult)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        myCalBubbleResultOK = CBool(myGlobal.SetDatos)
                    Else
                        isError = True
                    End If
                End If
                'end SGM 25/07/2012

                Dim myCleanDate As DateTime = MyClass.LastCleanDate
                Dim myCleanRecomm As Boolean = MyClass.IsCleanNeeded

                If Not isError Then

                    Dim myMonitorData As New ISEMonitorTO(mySafetyVolA, mySafetyVolB, myRPInstallDate, myRPExpireDate, myRPIsExpired, _
                                                          myRPRemVolA, myRPRemVolB, myRPInitVolA, myRPInitVolB, myRPEnoughA, myRPEnoughB, myRPHasBioCode, _
                                                          myRefData, myLiData, myNaData, myKData, myClData, myLithiumEnabled, _
                                                          myCalElectroDate, myCalElectroString1, myCalElectroString2, myCalElectroResult1OK, myCalElectroResult2OK, myCalElectroRecomm, _
                                                          myCalPumpsdate, myCalPumpsString, myCalPumpsResultOK, myCalPumpsRecomm, _
                                                          myCalBubbledate, myCalBubbleString, myCalBubbleResultOK, myCalBubbleRecomm, _
                                                          myCleanDate, myCleanRecomm, IsISEInitiatedOK, IsLongTermDeactivation, myCalElectrodesExpiredTime, myCalPumpsExpiredTime)

                    MyClass.MonitorDataTOAttr = myMonitorData

                    RaiseEvent ISEMonitorDataChanged()

                    myGlobal.SetDatos = myMonitorData

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.RefreshMonitorDataTO", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Gets the current alarms for showing in ISE Utilities
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 26/10/2012
        ''' Modified by XB 31/10/2014 - Add ISE timeout alarm - BA-1872
        ''' </remarks>
        Public Function GetISEAlarmsForUtilities(ByRef pPendingCalibrations As List(Of MaintenanceOperations)) As GlobalDataTO Implements IISEManager.GetISEAlarmsForUtilities
            Dim myGlobal As New GlobalDataTO
            Try
                If Not MyClass.IsISEModuleInstalled Then
                    Return myGlobal 'not to show anything in case of not installed
                End If

                Dim myISEUtilAlarms As New List(Of GlobalEnumerates.Alarms)

                If MyClass.myAlarms(Alarms.LongTermDeactivated) Or MyClass.IsLongTermDeactivation Then
                    myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_LONG_DEACT_ERR)
                    If Not MyClass.IsISEInitiatedOK Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR)
                Else
                    If Not MyClass.IsISESwitchON Then
                        myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_OFF_ERR)
                    Else
                        If MyClass.myAlarms(Alarms.CleanPack_Installed) Then
                            If MyClass.myAlarms(Alarms.CleanPack_Installed) Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_CP_INSTALL_WARN)
                            If MyClass.myAlarms(Alarms.CleanPack_Wrong) Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_CP_WRONG_ERR)
                        Else
                            If Not MyClass.IsISEInitiatedOK Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR)
                            If MyClass.myAlarms(Alarms.ReagentsPack_Invalid) Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_RP_INVALID_ERR)
                            If MyClass.myAlarms(Alarms.ReagentsPack_DateInstall) Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_RP_NO_INST_ERR)
                            If MyClass.myAlarms(Alarms.ReagentsPack_Expired) Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_RP_EXPIRED_WARN)
                            If MyClass.myAlarms(Alarms.ReagentsPack_Depleted) Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_RP_DEPLETED_ERR)

                            If MyClass.myAlarms(Alarms.Electrodes_Wrong) Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_ELEC_WRONG_ERR)
                            If MyClass.myAlarms(Alarms.Electrodes_Cons_Expired) Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_ELEC_CONS_WARN)
                            If MyClass.myAlarms(Alarms.Electrodes_Date_Expired) Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_ELEC_DATE_WARN)

                            ' XB 04/11/2014 - BA-1872
                            If MyClass.myAlarms(Alarms.Timeout) Then myISEUtilAlarms.Add(GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR)

                            pPendingCalibrations = New List(Of MaintenanceOperations)
                            If MyClass.IsCalibrationNeeded Then pPendingCalibrations.Add(MaintenanceOperations.ElectrodesCalibration)
                            If MyClass.IsPumpCalibrationNeeded Then pPendingCalibrations.Add(MaintenanceOperations.PumpsCalibration)
                            If MyClass.IsBubbleCalibrationNeeded Then pPendingCalibrations.Add(MaintenanceOperations.BubbleCalibration)

                        End If

                    End If
                End If

                myGlobal.SetDatos = myISEUtilAlarms

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetISEAlarmsForUtilities", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

#End Region

#Region "Read/Write from/to Database"

        ''' <summary>
        ''' Loads all ISE Parameters
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 16/02/2012</remarks>
        Private Function LoadISEParameters() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            'Dim myAllParametersDS As New ParametersDS
            ''Dim myGlobalbase As New GlobalBase
            Try
                If MyClass.AnalyzerModelAttr.Length > 0 Then
                    myGlobal = myParams.GetAllISEList(Nothing)
                    If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                        MyClass.myISESwParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    End If
                Else
                    myGlobal.SetDatos = Nothing
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.LoadISEParameters", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function



        ''' <summary>
        ''' Loads all ISE related limits
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/03/2012</remarks>
        Private Function LoadISELimits() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myAllFieldLimitsDS As FieldLimitsDS
                Dim myFieldLimitsDelegate As New FieldLimitsDelegate
                myGlobal = myFieldLimitsDelegate.GetAllList(Nothing)
                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                    myAllFieldLimitsDS = DirectCast(myGlobal.SetDatos, FieldLimitsDS)
                    If myAllFieldLimitsDS IsNot Nothing Then
                        Dim myLimitList As New List(Of FieldLimitsDS.tfmwFieldLimitsRow)

                        myLimitList = (From a In myAllFieldLimitsDS.tfmwFieldLimits _
                                     Where a.LimitID.Trim.StartsWith("ISE_") _
                                     Select a).ToList
                        If myLimitList.Count > 0 Then
                            MyClass.myISELimitsDS = New FieldLimitsDS
                            For Each L As FieldLimitsDS.tfmwFieldLimitsRow In myLimitList
                                Dim myRow As FieldLimitsDS.tfmwFieldLimitsRow = MyClass.myISELimitsDS.tfmwFieldLimits.NewtfmwFieldLimitsRow
                                With myRow
                                    .BeginEdit()
                                    .LimitID = L.LimitID
                                    .LimitDescription = L.LimitDescription
                                    .MinValue = L.MinValue
                                    .MaxValue = L.MaxValue
                                    .StepValue = L.StepValue
                                    .DefaultValue = L.DefaultValue
                                    .DecimalsAllowed = L.DecimalsAllowed
                                    .AnalyzerModel = L.AnalyzerModel
                                    .EndEdit()
                                End With
                                MyClass.myISELimitsDS.tfmwFieldLimits.Rows.Add(myRow)
                                myGlobal.AffectedRecords += 1
                            Next
                        End If
                        myLimitList = Nothing 'AG 02/08/2012 release memory
                    End If
                End If

                MyClass.myISELimitsDS.AcceptChanges()

                myGlobal.SetDatos = MyClass.myISELimitsDS

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.LoadISELimits", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Gets the value of the informed parameter from the parameters dataset
        ''' </summary>
        ''' <param name="pISEParameterName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  SG 16/02/2012
        ''' Modified by XB 04/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
        ''' </remarks>
        Private Function GetISEParameterValue(ByVal pISEParameterName As SwParameters, Optional ByVal pIsTextValue As Boolean = False) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If MyClass.myISESwParametersDS IsNot Nothing Then

                    Dim myParamsRows As New List(Of ParametersDS.tfmwSwParametersRow)
                    myParamsRows = (From a As ParametersDS.tfmwSwParametersRow _
                                        In MyClass.myISESwParametersDS.tfmwSwParameters _
                                        Where a.ParameterName.Trim.ToUpperBS = pISEParameterName.ToString.ToUpperBS _
                                        Select a).ToList()
                    'Where a.ParameterName.Trim.ToUpper = pISEParameterName.ToString.ToUpper _

                    If myParamsRows.Count > 0 Then
                        If Not pIsTextValue Then
                            myGlobal.SetDatos = myParamsRows.First.ValueNumeric
                        Else
                            Dim myText As String = myParamsRows.First.ValueText
                            If myText.Length > 0 Then
                                myGlobal.SetDatos = myText
                            Else
                                myGlobal.SetDatos = Nothing
                            End If
                        End If
                    Else
                        myGlobal.SetDatos = Nothing
                    End If
                    myParamsRows = Nothing 'AG 02/08/2012 release memory
                Else
                    myGlobal.SetDatos = Nothing
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetISEParameterValue", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function



        ''' <summary>
        ''' Gets the values of the informed limit from the limits dataset
        ''' </summary>
        ''' <param name="pLimitEnum"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/03/2012</remarks>
        Private Function GetISELimitValues(ByVal pLimitEnum As FieldLimitsEnum) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                Dim myLimitList As New List(Of FieldLimitsDS.tfmwFieldLimitsRow)
                myLimitList = (From a In MyClass.myISELimitsDS.tfmwFieldLimits _
                             Where a.LimitID = pLimitEnum.ToString _
                             Select a).ToList

                If myLimitList.Count > 0 Then
                    myGlobal.SetDatos = myLimitList.First
                End If
                myLimitList = Nothing 'AG 02/08/2012 release memory

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetISELimitValues", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Reads the data from the tinfoISE datatable in the Database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/03/2012</remarks>
        Private Function ReadISEInformationTable() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If MyClass.myISEInfoDelegate IsNot Nothing Then
                    myGlobal = MyClass.myISEInfoDelegate.ReadAllInfo(Nothing, MyClass.AnalyzerIDAttr)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        MyClass.myISEInfoDS = CType(myGlobal.SetDatos, ISEInformationDS)

                        MyClass.RefreshMonitorDataTO()
                    End If
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ReadISEInformationTable", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Updates the data to the tinfoISE datatable in the Database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/03/2012</remarks>
        Private Function UpdateISEInformationTable() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If MyClass.myISEInfoDS IsNot Nothing Then
                    If MyClass.myISEInfoDelegate IsNot Nothing Then
                        myGlobal = MyClass.myISEInfoDelegate.UpdateISEInfo(Nothing, MyClass.AnalyzerIDAttr, MyClass.myISEInfoDS)
                    End If
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateISEInformationTable", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Updates the Info ISE dataset
        ''' </summary>
        ''' <param name="pISESettingID"></param>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 06/03/2012
        ''' Modified by XBC 31/05/2012
        ''' Add parameter pIsDatetime. Datetime Format must be controled due ISE info may be loaded into systems with different culture info
        ''' Modified by SGM 29/01/2013 - DateTime to Invariant Format - Bug #1121
        ''' </remarks>
        Public Function UpdateISEInfoSetting(ByVal pISESettingID As ISEModuleSettings, ByVal pValue As Object, Optional ByVal pIsDatetime As Boolean = False) As GlobalDataTO Implements IISEManager.UpdateISEInfoSetting
            Dim myGlobal As New GlobalDataTO
            Try
                If MyClass.AnalyzerIDAttr.Length > 0 AndAlso MyClass.myISEInfoDS IsNot Nothing Then

                    Dim myInfoRows As New List(Of ISEInformationDS.tinfoISERow)
                    myInfoRows = (From a As ISEInformationDS.tinfoISERow _
                                        In MyClass.myISEInfoDS.tinfoISE _
                                        Where a.AnalyzerID = MyClass.AnalyzerIDAttr _
                                        And a.ISESettingID = pISESettingID.ToString _
                                        Select a).ToList()

                    If myInfoRows.Count > 0 Then
                        Dim myRow As ISEInformationDS.tinfoISERow = myInfoRows.First
                        myRow.BeginEdit()
                        If pIsDatetime Then
                            'SGM 29/01/2013 - DateTime to Invariant Format
                            myRow.Value = CDate(pValue).ToString(CultureInfo.InvariantCulture)
                            'myRow.Value = CDate(pValue).ToString(MyClass.ISEInfoDateFormat)
                        Else
                            myRow.Value = CStr(pValue)
                            'myRow.Value = pValue
                        End If
                        myRow.EndEdit()
                        MyClass.myISEInfoDS.tinfoISE.AcceptChanges()
                    Else
                        myGlobal.SetDatos = Nothing
                    End If
                    myInfoRows = Nothing 'AG 02/08/2012 release memory
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateISEInfoSetting", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Gets the value from ISE Info dataset
        ''' </summary>
        ''' <param name="pISESettingID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 16/02/2012
        ''' Modified by XBC 31/05/2012
        ''' Add parameter pIsDatetime. Datetime Format must be controled due ISE info may be loaded into systems with different culture info
        ''' Modified by SGM 29/01/2013 - DateTime to Invariant Format - Bug #1121
        ''' </remarks>
        Private Function GetISEInfoSettingValue(ByVal pISESettingID As ISEModuleSettings, Optional ByVal pIsDatetime As Boolean = False) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If MyClass.AnalyzerIDAttr.Length > 0 AndAlso MyClass.myISEInfoDS IsNot Nothing Then

                    Dim myInfoRows As New List(Of ISEInformationDS.tinfoISERow)
                    myInfoRows = (From a As ISEInformationDS.tinfoISERow _
                                        In MyClass.myISEInfoDS.tinfoISE _
                                        Where a.AnalyzerID = MyClass.AnalyzerIDAttr _
                                        And a.ISESettingID = pISESettingID.ToString _
                                        Select a).ToList()

                    If myInfoRows.Count > 0 Then
                        Dim myValue As String = myInfoRows.First.Value
                        If myValue.Length > 0 Then

                            If pIsDatetime Then
                                'SGM 29/01/2013 - DateTime to Invariant Format
                                myGlobal.SetDatos = DateTime.Parse(myValue, CultureInfo.InvariantCulture)
                                'myGlobal.SetDatos = DateTime.ParseExact(myValue, MyClass.ISEInfoDateFormat, DateTimeFormatInfo.InvariantInfo)
                            Else
                                myGlobal.SetDatos = myValue
                            End If

                        Else
                            myGlobal.SetDatos = Nothing
                        End If
                    Else
                        myGlobal.SetDatos = Nothing
                    End If
                    myInfoRows = Nothing 'AG 02/08/2012 release memory
                Else
                    myGlobal.SetDatos = Nothing
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetISEInfoSettingValue", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Checks if the informed electrode's test count
        ''' </summary>
        ''' <param name="pElectrode"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/03/2012</remarks>
        Private Function GetElectrodeTestCount(ByVal pElectrode As ISE_Electrodes) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myConsumption As Integer = -1
                Dim myISESetting As ISEModuleSettings = ISEModuleSettings.NONE

                Select Case pElectrode
                    Case ISE_Electrodes.Ref : myISESetting = ISEModuleSettings.REF_CONSUMPTION
                    Case ISE_Electrodes.Li : myISESetting = ISEModuleSettings.LI_CONSUMPTION
                    Case ISE_Electrodes.Na : myISESetting = ISEModuleSettings.NA_CONSUMPTION
                    Case ISE_Electrodes.K : myISESetting = ISEModuleSettings.K_CONSUMPTION
                    Case ISE_Electrodes.Cl : myISESetting = ISEModuleSettings.CL_CONSUMPTION
                End Select

                myGlobal = MyClass.GetISEInfoSettingValue(myISESetting)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    myConsumption = CInt(myGlobal.SetDatos)
                End If

                myGlobal.SetDatos = myConsumption

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetElectrodeTestCount", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function
#End Region

#Region "Checkings"

        ''' <summary>
        ''' Checks if the informed electrode is expired
        ''' </summary>
        ''' <param name="pElectrode"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/03/2012</remarks>
        Private Function CheckElectrodeExpired(ByVal pElectrode As ISE_Electrodes) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IsExpired As Boolean = False
                Dim HasInstallDate As Boolean = False
                Dim myInstallDate As DateTime = Nothing
                Dim myParameter As SwParameters

                Select Case pElectrode
                    Case ISE_Electrodes.Ref : HasInstallDate = MyClass.HasRefInstallDate : myParameter = SwParameters.ISE_EXPIRATION_TIME_REF : myInstallDate = MyClass.RefInstallDate
                    Case ISE_Electrodes.Li : HasInstallDate = MyClass.HasLiInstallDate : myParameter = SwParameters.ISE_EXPIRATION_TIME_LI : myInstallDate = MyClass.LiInstallDate
                    Case ISE_Electrodes.Na : HasInstallDate = MyClass.HasNaInstallDate : myParameter = SwParameters.ISE_EXPIRATION_TIME_NA : myInstallDate = MyClass.NaInstallDate
                    Case ISE_Electrodes.K : HasInstallDate = MyClass.HasKInstallDate : myParameter = SwParameters.ISE_EXPIRATION_TIME_K : myInstallDate = MyClass.KInstallDate
                    Case ISE_Electrodes.Cl : HasInstallDate = MyClass.HasClInstallDate : myParameter = SwParameters.ISE_EXPIRATION_TIME_CL : myInstallDate = MyClass.ClInstallDate
                End Select

                If HasInstallDate Then
                    myGlobal = MyClass.GetISEParameterValue(myParameter)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        Dim myExpirationTerm As Integer = CInt(myGlobal.SetDatos)
                        Dim myExpirationDate As DateTime = myInstallDate.AddMonths(myExpirationTerm)
                        IsExpired = (myExpirationDate < DateTime.Now)
                    End If
                Else
                    IsExpired = False
                End If

                myGlobal.SetDatos = IsExpired

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckElectrodeExpired", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Checks if the informed electrode is overused
        ''' </summary>
        ''' <param name="pElectrode"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/03/2012</remarks>
        Private Function CheckElectrodeOverUsed(ByVal pElectrode As ISE_Electrodes) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IsOverUsed As Boolean = False
                Dim myConsumption As Integer = -1
                Dim myParameter As SwParameters

                Select Case pElectrode
                    Case ISE_Electrodes.Ref : myParameter = SwParameters.ISE_MAX_CONSUME_REF : myConsumption = MyClass.RefTestCount
                    Case ISE_Electrodes.Li : myParameter = SwParameters.ISE_MAX_CONSUME_LI : myConsumption = MyClass.LiTestCount
                    Case ISE_Electrodes.Na : myParameter = SwParameters.ISE_MAX_CONSUME_NA : myConsumption = MyClass.NaTestCount
                    Case ISE_Electrodes.K : myParameter = SwParameters.ISE_MAX_CONSUME_K : myConsumption = MyClass.KTestCount
                    Case ISE_Electrodes.Cl : myParameter = SwParameters.ISE_MAX_CONSUME_CL : myConsumption = MyClass.ClTestCount
                End Select


                myGlobal = MyClass.GetISEParameterValue(myParameter)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim myMaximumConsumption As Single = CSng(myGlobal.SetDatos)
                    IsOverUsed = (myConsumption > myMaximumConsumption)
                End If

                myGlobal.SetDatos = IsOverUsed

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckElectrodeOverUsed", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function



        ''' <summary>
        ''' Validates the Electrodes calibration result
        ''' </summary>
        ''' <param name="pResultStr"></param>
        ''' <param name="pForcedLiEnabledValue">The value to force in LiEnabled checks in the validation (Optional)
        ''' Value TriState.UseDefault: The LiEnabled value is the current LiEnabled value (saved in DB)
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY:  SGM 06/03/2012
        ''' MODIFIED BY: JBL 20/09/2012 - Added parameter pForcedLiEnabledValue set Public Function
        ''' </remarks>
        Public Function ValidateElectrodesCalibration(ByVal pResultStr As String, Optional ByVal pForcedLiEnabledValue As TriState = TriState.UseDefault) As GlobalDataTO Implements IISEManager.ValidateElectrodesCalibration
            Dim myGlobal As New GlobalDataTO
            Try
                Dim isError As Boolean = False
                Dim isLiOK As Boolean = False
                Dim isNaOK As Boolean = False
                Dim isKOK As Boolean = False
                Dim isClOK As Boolean = False
                Dim myCalibrationOK As Boolean = False

                Dim myReceptionDecoder As New ISEReception(MyClass.myAnalyzer)
                myGlobal = myReceptionDecoder.GetLiNaKClValues(pResultStr)
                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                    Dim myValues As ISEResultTO.LiNaKCl = CType(myGlobal.SetDatos, ISEResultTO.LiNaKCl)

                    Dim myLimits As FieldLimitsDS.tfmwFieldLimitsRow

                    ' JB 20/09/2019 - Added parameter pForcedLiEnabledValue (to historical validations)
                    If (pForcedLiEnabledValue = TriState.UseDefault And MyClass.IsLiEnabledByUser) Or (pForcedLiEnabledValue = TriState.True) Then
                        'If MyClass.IsLiEnabledByUser Then
                        'Lithium
                        myGlobal = MyClass.GetISELimitValues(FieldLimitsEnum.ISE_CALIB_ACCEPTABLE_LI)
                        If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                            myLimits = CType(myGlobal.SetDatos, FieldLimitsDS.tfmwFieldLimitsRow)
                            isLiOK = (myValues.Li > myLimits.MinValue And myValues.Li < myLimits.MaxValue)
                        Else
                            isError = True
                        End If
                    End If

                    'Sodium
                    myGlobal = MyClass.GetISELimitValues(FieldLimitsEnum.ISE_CALIB_ACCEPTABLE_NA)
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        myLimits = CType(myGlobal.SetDatos, FieldLimitsDS.tfmwFieldLimitsRow)
                        isNaOK = (myValues.Na > myLimits.MinValue And myValues.Na < myLimits.MaxValue)
                    Else
                        isError = True
                    End If

                    'Potassium
                    myGlobal = MyClass.GetISELimitValues(FieldLimitsEnum.ISE_CALIB_ACCEPTABLE_K)
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        myLimits = CType(myGlobal.SetDatos, FieldLimitsDS.tfmwFieldLimitsRow)
                        isKOK = (myValues.K > myLimits.MinValue And myValues.K < myLimits.MaxValue)
                    Else
                        isError = True
                    End If

                    'Chlorine
                    myGlobal = MyClass.GetISELimitValues(FieldLimitsEnum.ISE_CALIB_ACCEPTABLE_CL)
                    If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                        myLimits = CType(myGlobal.SetDatos, FieldLimitsDS.tfmwFieldLimitsRow)
                        isClOK = (myValues.Cl > myLimits.MinValue And myValues.Cl < myLimits.MaxValue)
                    Else
                        isError = True
                    End If

                    If Not isError Then
                        ' XBC 25/07/2012 - Locks by Electrode
                        Dim myExecutionsDelegate As New ExecutionsDelegate

                        myCalibrationOK = isNaOK And isKOK And isClOK
                        ' JB 20/09/2019 - Added parameter pForcedLiEnabledValue (to historical validations)
                        If (pForcedLiEnabledValue = TriState.UseDefault And MyClass.IsLiEnabledByUser) Or (pForcedLiEnabledValue = TriState.True) Then
                            'If MyClass.IsLiEnabledByUser Then
                            myCalibrationOK = myCalibrationOK And isLiOK
                            ' XBC 25/07/2012
                            If Not isLiOK Then
                                myGlobal = myExecutionsDelegate.UpdateStatusByISETestType(Nothing, WorkSessionIDAttr, AnalyzerIDAttr, "Li", "PENDING", "LOCKED")
                            End If
                        End If

                        ' XBC 25/07/2012
                        If Not isNaOK Then
                            myGlobal = myExecutionsDelegate.UpdateStatusByISETestType(Nothing, WorkSessionIDAttr, AnalyzerIDAttr, "Na", "PENDING", "LOCKED")
                        End If
                        If Not isKOK Then
                            myGlobal = myExecutionsDelegate.UpdateStatusByISETestType(Nothing, WorkSessionIDAttr, AnalyzerIDAttr, "K", "PENDING", "LOCKED")
                        End If
                        If Not isClOK Then
                            myGlobal = myExecutionsDelegate.UpdateStatusByISETestType(Nothing, WorkSessionIDAttr, AnalyzerIDAttr, "Cl", "PENDING", "LOCKED")
                        End If

                    End If

                End If

                myGlobal.SetDatos = myCalibrationOK

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ValidateElectrodesCalibration", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Validates the Pumps calibration result.
        ''' The difference between A and B must be between defined limits
        ''' </summary>
        ''' <param name="pResultStr"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 06/03/2012</remarks>
        Private Function ValidatePumpsCalibration(ByVal pResultStr As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myCalibrationOK As Boolean = False
                Dim myReceptionDecoder As New ISEReception(MyClass.myAnalyzer)
                myGlobal = myReceptionDecoder.GetPumpCalibrationValues(pResultStr)
                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                    Dim myValues As ISEResultTO.PumpCalibrationValues = CType(myGlobal.SetDatos, ISEResultTO.PumpCalibrationValues)

                    Dim myLimitList As New List(Of FieldLimitsDS.tfmwFieldLimitsRow)

                    myLimitList = (From a In MyClass.myISELimitsDS.tfmwFieldLimits _
                                 Where a.LimitID = FieldLimitsEnum.ISE_PUMPS_CALIBR_OK.ToString _
                                 Select a).ToList

                    If myLimitList.Count > 0 Then
                        myCalibrationOK = (myValues.PumpA >= myLimitList.First.MinValue And myValues.PumpA <= myLimitList.First.MaxValue) And (myValues.PumpB >= myLimitList.First.MinValue And myValues.PumpB <= myLimitList.First.MaxValue)
                    End If
                    myLimitList = Nothing 'AG 02/08/2012 release memory
                End If

                myGlobal.SetDatos = myCalibrationOK

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ValidatePumpsCalibration", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Validates the Bubble Detector calibration result.
        ''' The difference between A and L must be higher than ISE_MIN_BUBBLE_CALIBR_OK (90)
        ''' </summary>
        ''' <param name="pResultStr"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 25/07/2012</remarks>
        Private Function ValidateBubbleCalibration(ByVal pResultStr As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myCalibrationOK As Boolean = False
                Dim myReceptionDecoder As New ISEReception(MyClass.myAnalyzer)
                myGlobal = myReceptionDecoder.GetBubbleDetectorCalibrationValues(pResultStr)
                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                    Dim myValues As ISEResultTO.BubbleCalibrationValues = CType(myGlobal.SetDatos, ISEResultTO.BubbleCalibrationValues)

                    Dim myMinOK As Double
                    myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_MIN_BUBBLE_CALIBR_OK, False)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myMinOK = CType(myGlobal.SetDatos, Double)
                        myCalibrationOK = (Math.Abs(myValues.ValueA - myValues.ValueL) >= myMinOK)
                    End If

                End If

                myGlobal.SetDatos = myCalibrationOK

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ValidateBubbleCalibration", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Validates the informed value between informed limits
        ''' </summary>
        ''' <param name="pLimitsID"></param>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 16/02/2012</remarks>
        Private Function ValidateDataLimits(ByVal pLimitsID As FieldLimitsEnum, ByVal pValue As Single) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myResult As Boolean = False
                Dim myValidateMin As Single
                Dim myValidateMax As Single
                Dim myFieldLimitsDS As New FieldLimitsDS
                Dim myFieldLimitsDelegate As New FieldLimitsDelegate()
                'Load the specified limits
                myGlobal = myFieldLimitsDelegate.GetList(Nothing, pLimitsID)
                If (Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing) Then
                    myFieldLimitsDS = CType(myGlobal.SetDatos, FieldLimitsDS)
                    If myFieldLimitsDS.tfmwFieldLimits.Rows.Count > 0 Then
                        myValidateMin = CType(myFieldLimitsDS.tfmwFieldLimits(0).MinValue, Single)
                        myValidateMax = CType(myFieldLimitsDS.tfmwFieldLimits(0).MaxValue, Single)
                        myResult = (pValue >= myValidateMin And pValue <= myValidateMax)
                    End If
                End If

                myGlobal.SetDatos = myResult

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetValidationLimits", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function





        ''' <summary>
        ''' Validates the milivolts values after sending a RDMV
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 19/02/2012</remarks>
        Private Function CheckElectrodesMounted() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myCheckValues As ISEResultTO.LiNaKCl

                If MyClass.LastISEResult.CalibratorAMilivolts.HasData Then
                    myCheckValues = MyClass.LastISEResult.CalibratorAMilivolts
                ElseIf MyClass.LastISEResult.CalibratorBMilivolts.HasData Then
                    myCheckValues = MyClass.LastISEResult.CalibratorBMilivolts
                End If

                For E As Integer = 0 To 5 Step 1
                    Dim myMaxValue As Single
                    Dim myParameter As SwParameters
                    Dim myElectrode As ISE_Electrodes = CType(E, ISE_Electrodes)
                    Select Case myElectrode
                        Case ISE_Electrodes.Li : myParameter = SwParameters.ISE_OUT_OF_RANGE_LI
                        Case ISE_Electrodes.Na : myParameter = SwParameters.ISE_OUT_OF_RANGE_NA
                        Case ISE_Electrodes.K : myParameter = SwParameters.ISE_OUT_OF_RANGE_K
                        Case ISE_Electrodes.Cl : myParameter = SwParameters.ISE_OUT_OF_RANGE_CL
                    End Select

                    If CInt(myParameter) > 0 Then
                        myGlobal = MyClass.GetISEParameterValue(myParameter)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                            myMaxValue = CSng(myGlobal.SetDatos)

                            Select Case myElectrode
                                Case ISE_Electrodes.Li
                                    'If MyClass.IsLiEnabledByUser Then
                                    MyClass.IsLiMounted = (myCheckValues.Li <= myMaxValue)
                                    'End If

                                Case ISE_Electrodes.Na
                                    MyClass.IsNaMounted = (myCheckValues.Na <= myMaxValue)

                                Case ISE_Electrodes.K
                                    MyClass.IsKMounted = (myCheckValues.K <= myMaxValue)

                                Case ISE_Electrodes.Cl
                                    MyClass.IsClMounted = (myCheckValues.Cl <= myMaxValue)

                            End Select

                        End If
                    End If
                Next


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckElectrodesMounted", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Check if the electrodes are ready for working. They must have Instalation date and must be mounted
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 12/03/2012</remarks>
        Private Function CheckElectrodesReady() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim AreReady As Boolean = False

                ' XBC 16/03/2012
                'If IsLiEnabledByUser Then
                '    AreReady = ((MyClass.IsRefInstalled And Not MyClass.IsRefExpired And Not MyClass.IsRefOverUsed) _
                '        And (MyClass.IsLiInstalled And Not MyClass.IsLiExpired And Not MyClass.IsLiOverUsed) _
                '        And (MyClass.IsNaInstalled And Not MyClass.IsNaExpired And Not MyClass.IsNaOverUsed) _
                '        And (MyClass.IsKInstalled And Not MyClass.IsKExpired And Not MyClass.IsKOverUsed) _
                '        And (MyClass.IsClInstalled And Not MyClass.IsClExpired And Not MyClass.IsClOverUsed))
                'Else
                '    AreReady = ((MyClass.IsRefInstalled And Not MyClass.IsRefExpired And Not MyClass.IsRefOverUsed) _
                '        And (MyClass.IsNaInstalled And Not MyClass.IsNaExpired And Not MyClass.IsNaOverUsed) _
                '        And (MyClass.IsKInstalled And Not MyClass.IsKExpired And Not MyClass.IsKOverUsed) _
                '        And (MyClass.IsClInstalled And Not MyClass.IsClExpired And Not MyClass.IsClOverUsed))
                'End If


                If IsLiEnabledByUser Then
                    AreReady = ((MyClass.IsRefInstalled) _
                        And (MyClass.IsLiInstalled) _
                        And (MyClass.IsNaInstalled) _
                        And (MyClass.IsKInstalled) _
                        And (MyClass.IsClInstalled))
                Else
                    AreReady = ((MyClass.IsRefInstalled) _
                        And (MyClass.IsNaInstalled) _
                        And (MyClass.IsKInstalled) _
                        And (MyClass.IsClInstalled))
                End If
                ' XBC 16/03/2012


                myGlobal.SetDatos = AreReady

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckElectrodesReady", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        Private Function UpdateISEModuleReady() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                Dim isReady As Boolean = (IsISESwitchON And _
                                           IsISEInitiatedOK And _
                                           IsISECommsOk And Not _
                                           IsLongTermDeactivation And _
                                           IsReagentsPackReady And _
                                           IsElectrodesReady And _
                                           Not IsCleanPackInstalled)
                'And (FluidicTubingInstallDate <> Nothing) And _
                '(PumpTubingInstallDate <> Nothing))

                If Not MyClass.IsInUtilities Then
                    isReady = isReady And (ISEWSCancelErrorCounterAttr < 3) 'SGM 01/08/2012 - If error A,B,S or F are repeated up to 3 then ISE is not ready and ISE preparations are blocked
                End If

                If IsISEModuleReadyAttr <> isReady Then
                    IsISEModuleReadyAttr = isReady
                    RaiseEvent ISEReadyChanged()
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateISEModuleReady", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        Private Function CheckReagentsPackReady() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IsReady As Boolean = False

                If IsCleanPackInstalled Then
                    IsReady = False
                Else
                    IsReady = IsReagentsPackInstalled
                End If


                'And IsEnoughCalibratorA _
                'And IsEnoughCalibratorB)
                'And Not AreReagentsExpired _

                myGlobal.SetDatos = IsReady

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckReagentsPackReady", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        Private Function CheckReagentsPackVolumeEnough() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IsReady As Boolean = False

                IsReady = (IsEnoughCalibratorA _
                        And IsEnoughCalibratorB)

                myGlobal.SetDatos = IsReady

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckReagentsPackVolumeEnough", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Validate Reagents Pack Serial number with the last registered
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/07/2012 - NO V1</remarks>
        Private Function CheckReagentsSerialNumber() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IsMatch As Boolean = False
                Dim myLastReagentsSN As String

                myGlobal = GetISEInfoSettingValue(ISEModuleSettings.REAGENTS_SERIAL_NUM)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    myLastReagentsSN = CType(myGlobal.SetDatos, String)

                    If ISEDallasSNAttr.SerialNumber.Length = 0 Then
                        IsReagentsPackSerialNumberMatchAttr = (ISEDallasSNAttr.SerialNumber = myLastReagentsSN)
                    Else
                        If ISEDallasSNAttr.SerialNumber.Length > 0 Then
                            myGlobal = MyClass.UpdateISEInfoSetting(ISEModuleSettings.REAGENTS_SERIAL_NUM, ISEDallasSNAttr.SerialNumber)
                        End If
                    End If

                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckReagentsSerialNumber", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Checks if any ISE calibration is needed that could lock ISE preparations
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 07/09/2012</remarks>
        Public Function CheckAnyCalibrationIsNeeded(ByRef pAffectedElectrodes As List(Of String)) As GlobalDataTO Implements IISEManager.CheckAnyCalibrationIsNeeded
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IsNeeded As Boolean = False

                myGlobal = MyClass.CheckBubbleCalibrationIsNeeded
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    IsNeeded = IsNeeded Or CBool(myGlobal.SetDatos)
                    myGlobal = MyClass.CheckPumpsCalibrationIsNeeded
                    If Not IsNeeded AndAlso Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        IsNeeded = IsNeeded Or CBool(myGlobal.SetDatos)
                        myGlobal = MyClass.CheckElectrodesCalibrationIsNeeded
                        If Not IsNeeded AndAlso Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            IsNeeded = IsNeeded Or CBool(myGlobal.SetDatos)
                            If IsNeeded Then 'check if it is result error
                                'affected electrodes will be locked in current ws
                                'pAffectedElectrodes = New List(Of String)
                                Dim myISEReception As New ISEReception(MyClass.myAnalyzer)

                                Dim myResultTO_1 As New ISEResultTO
                                myISEReception.FillISEResultValues(myResultTO_1, ISEResultTO.ISEResultItemTypes.Calibration1, MyClass.LastElectrodesCalibrationResult1)
                                If Not myResultTO_1.IsCancelError Then
                                    For Each E As ISEErrorTO In myResultTO_1.Errors
                                        Dim Aff As String() = E.Affected.Split(CChar(","))
                                        For a As Integer = 0 To Aff.Length - 1
                                            If Aff(a).Trim.Length > 0 Then
                                                Dim myElectrode As String = Aff(a) '.Substring(Aff(a).Length - 1)
                                                If pAffectedElectrodes Is Nothing Then pAffectedElectrodes = New List(Of String)
                                                If Not pAffectedElectrodes.Contains(myElectrode) Then pAffectedElectrodes.Add(myElectrode)
                                            End If
                                        Next
                                    Next
                                End If

                                Dim myResultTO_2 As New ISEResultTO
                                myISEReception.FillISEResultValues(myResultTO_2, ISEResultTO.ISEResultItemTypes.Calibration2, MyClass.LastElectrodesCalibrationResult2)
                                If Not myResultTO_2.IsCancelError Then
                                    For Each E As ISEErrorTO In myResultTO_2.Errors
                                        Dim Aff As String() = E.Affected.Split(CChar(","))
                                        For a As Integer = 0 To Aff.Length - 1
                                            If Aff(a).Trim.Length > 0 Then
                                                Dim myElectrode As String = Aff(a) '.Substring(Aff(a).Length - 1)
                                                If pAffectedElectrodes Is Nothing Then pAffectedElectrodes = New List(Of String)
                                                If Not pAffectedElectrodes.Contains(myElectrode) Then pAffectedElectrodes.Add(myElectrode)
                                            End If
                                        Next
                                    Next
                                End If
                            End If
                        End If
                    End If
                End If

                myGlobal.SetDatos = IsNeeded

                'in case of IsNeeded = False the pAffectedElectrodes must be evaluated in the result is returned

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckAnyCalibrationIsNeeded", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Checks if a new Electrodes calibration is needed
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 12/03/2012
        ''' Modified by XB 23/05/2014 - BT #1639 - Do not lock ISE preparations during Runnning (not Pause) by Pending Calibrations
        ''' </remarks>
        Public Function CheckElectrodesCalibrationIsNeeded() As GlobalDataTO Implements IISEManager.CheckElectrodesCalibrationIsNeeded
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IsNeeded As Boolean = False

                'If MyClass.IsISEModuleReady Then
                myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_CALB_HOURS_NEEDED)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim myNeededTime As DateTime
                    Dim myHours As Integer = CInt(myGlobal.SetDatos)
                    If MyClass.LastElectrodesCalibrationDate <> Nothing Then
                        myNeededTime = MyClass.LastElectrodesCalibrationDate.AddHours(myHours)
                    Else
                        myNeededTime = DateTime.Now
                    End If

                    IsNeeded = (DateTime.Now >= myNeededTime)

                End If

                ' XBC 28/06/2012
                If Not myGlobal.HasError AndAlso Not IsNeeded Then
                    myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.CALB_PENDING)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        IsNeeded = (CInt(myGlobal.SetDatos) > 0)
                    End If
                End If
                ' XBC 28/06/2012

                'End If

                MyClass.IsCalibrationNeeded = IsNeeded

                If IsNeeded Then RaiseEvent ISEMonitorDataChanged() ' XB 23/05/2014 - BT #1639

                myGlobal.SetDatos = IsNeeded

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckElectrodesCalibrationIsNeeded", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Checks if a new Bubble Detector calibration is needed
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 25/07/2012</remarks>
        Public Function CheckBubbleCalibrationIsNeeded() As GlobalDataTO Implements IISEManager.CheckBubbleCalibrationIsNeeded
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IsNeeded As Boolean = False

                If MyClass.LastPumpsCalibrationDate = Nothing Then
                    IsNeeded = True
                End If

                If Not myGlobal.HasError AndAlso Not IsNeeded Then
                    myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.BUBBLE_CAL_PENDING)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        IsNeeded = (CInt(myGlobal.SetDatos) > 0)
                    End If
                End If

                MyClass.IsBubbleCalibrationNeeded = IsNeeded

                myGlobal.SetDatos = IsNeeded

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckBubbleCalibrationIsNeeded", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Checks if a new Pumps calibration is needed
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 12/03/2012
        ''' Modified by XB 07/04/2014 - Refresh MonitorTO when change - task #1485
        ''' </remarks>
        Public Function CheckPumpsCalibrationIsNeeded() As GlobalDataTO Implements IISEManager.CheckPumpsCalibrationIsNeeded
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IsNeeded As Boolean = False

                'every day change
                If MyClass.LastPumpsCalibrationDate <> Nothing Then
                    ' XBC 27/06/2012
                    'IsNeeded = (DateTime.Now.Day <> MyClass.LastPumpsCalibrationDate.Day)
                    IsNeeded = (MyClass.LastPumpsCalibrationDate < DateAdd(DateInterval.Day, -1, DateTime.Now))
                    ' XBC 27/06/2012
                Else
                    IsNeeded = True
                End If

                ' XBC 28/06/2012
                If Not myGlobal.HasError AndAlso Not IsNeeded Then
                    myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.PUMP_CAL_PENDING)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        IsNeeded = (CInt(myGlobal.SetDatos) > 0)
                    End If
                End If
                ' XBC 28/06/2012

                ' XB 07/04/2014 - task #1485
                If IsNeeded Then MyClass.RefreshMonitorDataTO()

                MyClass.IsPumpCalibrationNeeded = IsNeeded

                myGlobal.SetDatos = IsNeeded

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckPumpsCalibrationIsNeeded", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Returns if it is requested to perform a cleaning Procedure
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 16/02/2012</remarks>
        Public Function CheckCleanIsNeeded() As GlobalDataTO Implements IISEManager.CheckCleanIsNeeded
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IsNeeded As Boolean = False
                Dim myMaximumTestsWithoutClean As Integer = -1

                ''If MyClass.IsISEModuleReady Then

                ' XBC 27/06/2012 - Self-maintenance 
                Dim myLastCleanDate As DateTime


                'get Lst Clean flag
                myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.LAST_CLEAN_DATE, True)
                If Not myGlobal.HasError Then
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        myLastCleanDate = CDate(myGlobal.SetDatos)
                    End If

                    If myLastCleanDate <> Nothing Then
                        If myLastCleanDate < DateAdd(DateInterval.Day, -1, DateTime.Now) Then
                            ' Clean is needed just in the case that ISE has been used
                            If MyClass.TestsCountSinceLastClean > 0 Then
                                IsNeeded = True
                            End If
                        Else
                            'get Maximum Test for requiring Sample
                            myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_CLEAN_REQUIRED_SAMPLES)
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                myMaximumTestsWithoutClean = CInt(myGlobal.SetDatos)
                                If MyClass.TestsCountSinceLastClean >= myMaximumTestsWithoutClean Then
                                    IsNeeded = True
                                End If
                            End If
                        End If
                    Else
                        '    IsNeeded = True    ' discarded XBC 10/07/2012
                        If MyClass.TestsCountSinceLastClean > 0 Then
                            IsNeeded = True
                        End If
                    End If

                End If
                'End If

                ''get Maximum Test for requiring Sample
                'myGlobal = MyClass.GetISEParameterValue(GlobalEnumerates.SwParameters.ISE_CLEAN_REQUIRED_SAMPLES)
                'If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                '    myMaximumTestsWithoutClean = CInt(myGlobal.SetDatos)
                '    If MyClass.TestsCountSinceLastClean >= myMaximumTestsWithoutClean Then
                '        IsNeeded = True
                '    End If
                'End If
                ' XBC 27/06/2012

                ' XBC 28/06/2012
                If Not myGlobal.HasError AndAlso Not IsNeeded Then
                    myGlobal = MyClass.GetISEInfoSettingValue(ISEModuleSettings.CLEAN_PENDING)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        IsNeeded = (CInt(myGlobal.SetDatos) > 0)
                    End If
                End If
                ' XBC 28/06/2012

                MyClass.IsCleanNeeded = IsNeeded

                myGlobal.SetDatos = IsNeeded

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckCleanIsNeeded", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Performs the Validation process for a Reagents pack provided by Biosystems
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 05/06/2012</remarks>
        Private Function ValidateBiosystemsPack() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            ''Dim myUtil As New Utilities.
            'Dim myLogAcciones As New ApplicationLogManager()
            Try

                '**********BIOSYSTEMS ALGORITHM SPECIFICATION*********************

                'Serial ID[8] = {0x23,0x00,0x00,0x06,0xF6,0x56,0xAE,0x09} (ID7,ID6,ID5,ID4,ID3,ID2,ID1,ID0)
                'X = =x06F656AE ([ID4][ID3][ID2][ID1]) 4 Bytes

                'X = X * X

                'MSB = 0xB830 (16 bits of Odd bits of X) -> [31][29][27][25]....[1]
                'LSB = 0xD9EA (16 bits of Even bits of X) -> [30][28][26][24]....[0]

                'Y = 0x[MSB][LSB] = 0xB830D9EA

                'Security Key Validation:
                'Y Byte 3 = 0xB8 = address 0
                'Y Byte 2 = 0x30 = address 25
                'Y Byte 1 = 0xD9 = address 26
                'Y Byte 0 = 0xEA = address 1

                '**********************************************

                Dim result As Boolean = False

                'Firs check Biosystems Code
                Dim myDistributorCode As String = MyClass.BiosystemsCode
                myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_BIOSYSTEMS_CODE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim myBioCode As String = CStr(myGlobal.SetDatos)
                    If myDistributorCode = myBioCode Then

                        If MyClass.ISEDallasSN IsNot Nothing And _
                            MyClass.ISEDallasPage00 IsNot Nothing And _
                            MyClass.ISEDallasPage01 IsNot Nothing Then

                            If Not MyClass.ISEDallasSN.ValidationError And _
                                Not MyClass.ISEDallasPage00.ValidationError And _
                                Not MyClass.ISEDallasPage01.ValidationError Then

                                'SGM 22/01/2013 - separate the algorithm
                                myGlobal = BiosystemsValidationAlgorithm(MyClass.ISEDallasSN, MyClass.ISEDallasPage00)
                                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                    result = CBool(myGlobal.SetDatos)
                                End If

                            End If
                        Else
                            Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + "ValidateBiosystemsPack: ISE dallas null")
                        End If
                    End If
                End If

                myGlobal.SetDatos = result

            Catch ex As Exception
                myGlobal.SetDatos = False
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                ''Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ValidateBiosystemsPack", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        Private Function ValidateBiosystemsCode() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                'OLD PACKS
                Dim myDistributorCode As String = MyClass.BiosystemsCode
                myGlobal = MyClass.GetISEParameterValue(SwParameters.ISE_BIOSYSTEMS_CODE, True)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim myBioCode As String = CStr(myGlobal.SetDatos)
                    Dim res As Boolean = (myDistributorCode = myBioCode)
                    myGlobal.SetDatos = res
                End If

            Catch ex As Exception
                myGlobal.SetDatos = False
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ValidateBiosystemsCode", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pISEDallasSN"></param>
        ''' <param name="pDallas00"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 22/01/2012</remarks>
        Public Shared Function BiosystemsValidationAlgorithm(ByVal pISEDallasSN As ISEDallasSNTO, ByVal pDallas00 As ISEDallasPage00TO) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            'Dim myLogAcciones As New ApplicationLogManager()
            'Dim Utilities As New Utilities

            Try
                Dim result As Boolean = False

                'Obtain SerialID 64 bits integer
                'Serial ID[8] = (ID7,ID6,ID5,ID4,ID3,ID2,ID1,ID0)
                Dim mySerialID_0 As String = pISEDallasSN.SerialNumber.Substring(0, 2)
                Dim mySerialID_1 As String = pISEDallasSN.SerialNumber.Substring(2, 2)
                Dim mySerialID_2 As String = pISEDallasSN.SerialNumber.Substring(4, 2)
                Dim mySerialID_3 As String = pISEDallasSN.SerialNumber.Substring(6, 2)
                Dim mySerialID_4 As String = pISEDallasSN.SerialNumber.Substring(8, 2)
                Dim mySerialID_5 As String = pISEDallasSN.SerialNumber.Substring(10, 2)
                Dim mySerialID_6 As String = pISEDallasSN.SerialNumber.Substring(12, 2)
                Dim mySerialID_7 As String = pISEDallasSN.SerialNumber.Substring(14, 2)

                Dim mySerialID As String = mySerialID_7 & mySerialID_6 & mySerialID_5 & mySerialID_4 & _
                            mySerialID_3 & mySerialID_2 & mySerialID_1 & mySerialID_0

                'X = ([ID4][ID3][ID2][ID1]) 4 Bytes
                Dim strX As String = mySerialID.Substring(6, 8)
                myGlobal = Utilities.ConvertHexToUInt32(strX)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim X As UInt32 = Convert.ToUInt32(myGlobal.SetDatos)

                    'SGM 31/08/2012 - This does not work!!!
                    'Dim X2old As UInt64 = Convert.ToUInt64(Math.Pow(X, 2))

                    'new way for power to 2 SGM 31/08/2012
                    Dim X2 As UInt64
                    myGlobal = Utilities.PowUint64To2(X)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        X2 = Convert.ToUInt64(myGlobal.SetDatos)
                        'X2 = Convert.ToUInt64((Convert.ToUInt64(X)) * (Convert.ToUInt64(X)))
                        Dim MMask As UInt64 = 4294901760 ' &HFFFF0000
                        Dim LMask As UInt64 = 65535 '&HFFFF
                        Dim High As UInt32 = CType(((X2 And MMask) >> 16), UInt32)
                        Dim Low As UInt32 = CType((X2 And LMask), UInt32)

                        Dim strHigh As String = ""
                        myGlobal = Utilities.ConvertUint32ToHex(High)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            strHigh = CStr(myGlobal.SetDatos)

                            'SGM 14/12/2012, V1.0.0 - prevent that result's length is minor than 4 bytes. add as many "0" up to 4
                            If strHigh.Length < 4 Then
                                For c As Integer = 1 To 4 - strHigh.Length Step 1
                                    strHigh = "0" & strHigh
                                Next
                            ElseIf strHigh.Length > 4 Then
                                GlobalBase.CreateLogActivity("ISE reagents Pack validation algorithm error", "ISEManager.BiosystemsValidationAlgorithm", EventLogEntryType.Error, False)
                            End If
                            'end SGM 14/12/2012

                        End If

                        Dim strLow As String = ""
                        myGlobal = Utilities.ConvertUint32ToHex(Low)
                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                            strLow = CStr(myGlobal.SetDatos)

                            'SGM 14/12/2012, V1.0.0 - prevent that result's length is minor than 4 bytes. add as many "0" up to 4
                            If strLow.Length < 4 Then
                                For c As Integer = 1 To 4 - strLow.Length Step 1
                                    strLow = "0" & strLow
                                Next
                            ElseIf strLow.Length > 4 Then
                                GlobalBase.CreateLogActivity("ISE reagents Pack validation algorithm error", "ISEManager.BiosystemsValidationAlgorithm", EventLogEntryType.Error, False)
                            End If
                            'end SGM 14/12/2012

                        End If


                        'MSB =(16 bits of Odd bits of X) -> [31][29][27][25]....[1]
                        'LSB =(16 bits of Even bits of X) -> [30][28][26][24]....[0]
                        If strHigh.Length = 4 And strLow.Length = 4 Then

                            Dim strHighLow As String = strHigh & strLow
                            Dim difTo8 As Integer = 8 - strHighLow.Length
                            For c As Integer = 1 To difTo8 Step 1
                                strHighLow = "0" & strHighLow
                            Next
                            myGlobal = Utilities.ConvertHexToBinaryString(strHighLow)
                            If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                Dim myBinary As String = CStr(myGlobal.SetDatos)
                                If myBinary.Length = 32 Then
                                    Dim strbMSB As String = ""
                                    Dim strbLSB As String = ""
                                    For B As Integer = 1 To myBinary.Length - 1 Step 2
                                        strbMSB &= myBinary.Substring(B - 1, 1)
                                        strbLSB &= myBinary.Substring(B, 1)
                                    Next

                                    'Y = 0x[MSB][LSB]
                                    Dim strMSBLSB As String = strbMSB & strbLSB
                                    myGlobal = Utilities.ConvertBinaryStringToDecimal(strMSBLSB)
                                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                                        Dim intY As UInt64 = Convert.ToUInt64(myGlobal.SetDatos)
                                        myGlobal = Utilities.ConvertDecimalToHex(Convert.ToInt64(intY))
                                        If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then

                                            'Security Key Validation:
                                            'these bytes must match the readed Security code from Dallas Page 00
                                            Dim myResultantCode As String = CStr(myGlobal.SetDatos)
                                            Dim dif8 As Integer = 8 - myResultantCode.Length
                                            'bug fixed: dif8 used
                                            For c As Integer = 1 To dif8 Step 1
                                                myResultantCode = "0" & myResultantCode
                                            Next

                                            If myResultantCode.Length <> 8 Then
                                                GlobalBase.CreateLogActivity("ISE reagents Pack validation algorithm error", "ISEManager.BiosystemsValidationAlgorithm", EventLogEntryType.Error, False)
                                            Else
                                                result = (myResultantCode = pDallas00.SecurityCode)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                myGlobal.SetDatos = result

            Catch ex As Exception
                myGlobal.SetDatos = False
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                ''Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.BiosystemsValidationAlgorithm", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function



        Private Function ValidateReagentsPackVolumes(ByVal pCal As String, ByVal pVol As Single) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                'If MyClass.ISEDallasPage00 IsNot Nothing Then
                Dim myPar As SwParameters
                Dim myPercent As Double
                Select Case pCal
                    Case "A"
                        myPar = SwParameters.ISE_MIN_SAFETY_VOLUME_CAL_A
                        If MyClass.ReagentsPackInitialVolCalA > 0 Then
                            myPercent = ((pVol * 100) / MyClass.ReagentsPackInitialVolCalA)
                        Else
                            myPercent = 100
                        End If

                    Case "B"
                        myPar = SwParameters.ISE_MIN_SAFETY_VOLUME_CAL_B
                        If MyClass.ReagentsPackInitialVolCalB > 0 Then
                            myPercent = ((pVol * 100) / MyClass.ReagentsPackInitialVolCalB)
                        Else
                            myPercent = 100
                        End If

                    Case Else : Exit Try
                End Select
                myGlobal = MyClass.GetISEParameterValue(myPar)
                If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                    Dim myMin As Integer = CInt(myGlobal.SetDatos)
                    myGlobal.SetDatos = (myPercent >= myMin)
                End If
                'End If

            Catch ex As Exception
                myGlobal.SetDatos = False
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ValidateReagentsPackVolumes", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        '''' <summary>
        '''' Check if ISE test are available for execute
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by XBC 16/03/2012</remarks>
        'Private Function ValidatePreparatiosAllowed() As GlobalDataTO
        '    Dim returnValue As Boolean = True
        '    Dim myglobal As New GlobalDataTO
        '    Try
        '        If Not IsISECommsOkAttr Then
        '            returnValue = False
        '        End If

        '        'If Not IsISEModuleReadyAttr Then
        '        '    returnValue = False
        '        'End If

        '        If IsCleanPackInstalledAttr Then
        '            returnValue = False
        '        End If

        '        If IsLongTermDeactivationAttr Then
        '            returnValue = False
        '        End If

        '        myglobal = CheckElectrodesReady()
        '        If myglobal.HasError Or myglobal.SetDatos Is Nothing Then
        '            returnValue = False
        '        Else
        '            If Not CType(myglobal.SetDatos, Boolean) Then
        '                returnValue = False
        '            End If
        '        End If

        '        myglobal = CheckReagentsPackReady()
        '        If myglobal.HasError Or myglobal.SetDatos Is Nothing Then
        '            returnValue = False
        '        Else
        '            If Not CType(myglobal.SetDatos, Boolean) Then
        '                returnValue = False
        '            End If
        '        End If

        '        If Not returnValue Then
        '            MyClass.IsReadyForISEPreparationsAttr = returnValue
        '        End If

        '    Catch ex As Exception
        '        myglobal.SetDatos = False
        '        myglobal.HasError = True
        '        myglobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myglobal.ErrorMessage = ex.Message

        '        returnValue = False

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ValidatePreparatiosAllowed", EventLogEntryType.Error, False)
        '    End Try
        '    Return myglobal
        'End Function

#End Region

#Region "Consumptions"

        ''' <summary>
        ''' Update all kind of ISE consumptions
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Private Function UpdateConsumptions() As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Try
                ' XBC 17/07/2012
                MyClass.SIPcycles = 0
                If Not WorkSessionIsRunningAttr Then
                    ' Check SIPPING consumption out of Work Session running
                    MyClass.SIPcycles = MyClass.EstimatedSIPCycles()
                Else
                    ' XBC 17/07/2012
                    ''Dim myLogAcciones As New ApplicationLogManager()    ' TO COMMENT !!!
                    'GlobalBase.CreateLogActivity("Update Consumptions - Work Session Running ! ", "ISEManager.UpdateConsumptions", EventLogEntryType.Information, False)   ' TO COMMENT !!!

                    ' Into Work session running, Sips are avoid
                    Dim myLastOperationDate As DateTime
                    ' Get last date ISE test completed
                    myglobal = GetISEInfoSettingValue(ISEModuleSettings.LAST_OPERATION_WS_DATE, True)
                    If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                        myLastOperationDate = CDate(myglobal.SetDatos)
                    End If

                    If myLastOperationDate <> Nothing Then
                        'GlobalBase.CreateLogActivity("Update Consumptions - Last Date " & myLastOperationDate.ToString, "ISEManager.UpdateConsumptions", EventLogEntryType.Information, False)   ' TO COMMENT !!!

                        Dim TimeOperation As Single
                        If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Then
                            TimeOperation = ISE_EXECUTION_TIME_SER
                            'GlobalBase.CreateLogActivity("Update Consumptions - SERUM [" & ISE_EXECUTION_TIME_SER.ToString & "]", "ISEManager.UpdateConsumptions", EventLogEntryType.Information, False)   ' TO COMMENT !!!
                        ElseIf MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then
                            TimeOperation = ISE_EXECUTION_TIME_URI
                            'GlobalBase.CreateLogActivity("Update Consumptions - URINE [" & ISE_EXECUTION_TIME_URI.ToString & "]", "ISEManager.UpdateConsumptions", EventLogEntryType.Information, False)   ' TO COMMENT !!!
                        End If

                        ' variable times depending on interval Firmware time duration
                        If Interval2forPurgeACompletedbyFW > 0 AndAlso myLastOperationDate < DateAdd(DateInterval.Second, -(Interval2forPurgeACompletedbyFW + TimeOperation), DateTime.Now) Then
                            ' If last ISE operation completed > ISE_PUGAbyFW_TIME2 the corresponding flag is increased
                            MyClass.PurgeAbyFirmwareAttr += 1
                            'GlobalBase.CreateLogActivity("Update Consumptions - PurgeA [" & MyClass.PurgeAbyFirmwareAttr.ToString & "]", "ISEManager.UpdateConsumptions", EventLogEntryType.Information, False)   ' TO COMMENT !!!
                        End If
                    End If
                End If
                ' XBC 17/07/2012

                If Not myglobal.HasError And Not MyClass.IsLongTermDeactivation And Not MyClass.IsCleanPackInstalled Then
                    myglobal = UpdateReagentsCalAConsumption()
                End If

                If Not myglobal.HasError And Not MyClass.IsLongTermDeactivation And Not MyClass.IsCleanPackInstalled Then
                    myglobal = UpdateReagentsCalBConsumption()
                End If

                If Not myglobal.HasError Then
                    myglobal = UpdateElectrodeRefConsumption()
                End If

                If Not myglobal.HasError Then
                    If MyClass.IsLiEnabledByUser And MyClass.IsLiInstalled Then
                        myglobal = UpdateElectrodeLiConsumption()
                    End If
                End If

                If Not myglobal.HasError Then
                    myglobal = UpdateElectrodeNaConsumption()
                End If

                If Not myglobal.HasError Then
                    myglobal = UpdateElectrodeKConsumption()
                End If

                If Not myglobal.HasError Then
                    myglobal = UpdateElectrodeClConsumption()
                End If

                ' XBC 17/07/2012
                If MyClass.SIPcycles > 0 Then
                    ' Update date for the Last registered SIP cycles
                    MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_OPERATION_DATE, DateTime.Now.ToString, True)
                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateConsumptions", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Get the Consumption Parameters
        ''' </summary>
        ''' <returns>GlobalDataTO containing a Typed Dataset AnalyzerLedPositionsDS with the list of WaveLength items</returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Public Function GetConsumptionParameters() As GlobalDataTO Implements IISEManager.GetConsumptionParameters
            Dim myGlobal As New GlobalDataTO
            Dim myParams As New SwParametersDelegate
            Dim myParametersDS As New ParametersDS
            Try
                ' Consumption of calibrator A by Serum Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_SERUM_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbySerumAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by Serum Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_SERUM_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbySerumAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by Urine (1st phase) Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_URINE1_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbyUrine1Attr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by Urine (1st phase) Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_URINE1_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbyUrine1Attr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by Urine (2nd phase) Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_URINE2_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbyUrine2Attr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by Urine (2nd phase) Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_URINE2_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbyUrine2Attr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by Electrodes calibration Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_CALB_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbyElectrodesCalAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by Electrodes calibration Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_CALB_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbyElectrodesCalAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by Pumps calibration Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_PMCL_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbyPumpsCalAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by Pumps calibration Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_PMCL_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbyPumpsCalAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by Bubbles detector calibration Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_BBCL_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbyBubblesCalAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by Bubbles detector calibration Test
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_BBCL_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbyBubblesCalAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by Clean cycle Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_CLEN_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbyCleanCycleAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by Clean cycle Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_CLEN_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbyCleanCycleAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by PurgeA Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_PUGA_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbyPurgeAAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by PurgeA Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_PUGA_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbyPurgeAAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by PurgeB Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_PUGB_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbyPurgeBAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by PurgeB Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_PUGB_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbyPurgeBAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by PrimeA Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_PRMA_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbyPrimeAAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by PrimeA Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_PRMA_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbyPrimeAAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by PrimeB Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_PRMB_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbyPrimeBAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by PrimeB Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_PRMB_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbyPrimeBAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator A by Sipping Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_SIPPING_A.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalAbySippingAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If
                ' Consumption of calibrator B by Sipping Procedure
                myGlobal = myParams.ReadByParameterName(Nothing, SwParameters.ISE_CONSUMPTION_SIPPING_B.ToString, Nothing)
                If Not myGlobal.HasError And Not myGlobal.SetDatos Is Nothing Then
                    myParametersDS = CType(myGlobal.SetDatos, ParametersDS)
                    MyClass.ConsumptionCalBbySippingAttr = myParametersDS.tfmwSwParameters.Item(0).ValueNumeric
                Else
                    myGlobal.HasError = True
                    Exit Try
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetConsumptionParameters", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Update Consumption of Calibrator A
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Private Function UpdateReagentsCalAConsumption() As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Try
                Dim myCurrentValue As Single
                Dim myAdditionValue As Single

                ' Get current value 
                myglobal = GetISEInfoSettingValue(ISEModuleSettings.AVAILABLE_VOL_CAL_A)
                If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                    myCurrentValue = CType(myglobal.SetDatos, Single)

                    If myCurrentValue > 0 Then      ' XB 03/12/2013 - Improvment - Do not save Consumptions when this value is negative - task #1414
                        If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.OK Or _
                           MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.CAL Or _
                           MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.PMC Or _
                           MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.BBC Or _
                           MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Or _
                           MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then

                            If MyClass.LastISEResultAttr IsNot Nothing Then

                                If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Then
                                    ' ISE Serum Test
                                    myAdditionValue = MyClass.ConsumptionCalAbySerumAttr
                                ElseIf MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then
                                    ' ISE Urine Test (phase 1)
                                    myAdditionValue = MyClass.ConsumptionCalAbyUrine1Attr
                                    ' ISE Urine Test (phase 2)
                                    myAdditionValue += MyClass.ConsumptionCalAbyUrine2Attr
                                Else
                                    ' ISE Commands

                                    Select Case MyClass.CurrentCommandTOAttr.ISECommandID

                                        Case ISECommands.CALB
                                            myAdditionValue = MyClass.ConsumptionCalAbyElectrodesCalAttr

                                        Case ISECommands.PUMP_CAL
                                            myAdditionValue = MyClass.ConsumptionCalAbyPumpsCalAttr

                                        Case ISECommands.BUBBLE_CAL
                                            myAdditionValue = MyClass.ConsumptionCalAbyBubblesCalAttr

                                        Case ISECommands.CLEAN
                                            myAdditionValue = MyClass.ConsumptionCalAbyCleanCycleAttr

                                        Case ISECommands.PURGEA
                                            myAdditionValue = MyClass.ConsumptionCalAbyPurgeAAttr

                                        Case ISECommands.PURGEB
                                            myAdditionValue = MyClass.ConsumptionCalAbyPurgeBAttr

                                        Case ISECommands.PRIME_CALA
                                            myAdditionValue = MyClass.ConsumptionCalAbyPrimeAAttr

                                        Case ISECommands.PRIME_CALB
                                            myAdditionValue = MyClass.ConsumptionCalAbyPrimeBAttr

                                        Case ISECommands.DSPA
                                            If Not MyClass.CurrentCommandTO Is Nothing Then
                                                If MyClass.CurrentCommandTO.ISECommandID = ISECommands.DSPA Then
                                                    If IsNumeric(MyClass.CurrentCommandTO.P1) Then
                                                        myAdditionValue = (CSng(MyClass.CurrentCommandTO.P1) / 1000) ' mL
                                                    End If
                                                End If
                                            End If

                                        Case ISECommands.DSPB
                                            ' No consume Cal A

                                    End Select

                                End If

                            End If
                        End If

                        ' XBC 02/07/2012
                        ' Add SIPPING consumption
                        If MyClass.SIPcycles > 0 Then
                            myAdditionValue += (MyClass.SIPcycles * ConsumptionCalAbySippingAttr)
                        End If

                        ' Update its value
                        myCurrentValue -= myAdditionValue
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.AVAILABLE_VOL_CAL_A, myCurrentValue.ToString)

                        MyClass.CountConsumptionToSaveDallasData_CalA += myAdditionValue

                        Debug.Print("CONSUME CAL A : " & MyClass.CountConsumptionToSaveDallasData_CalA.ToString & " (-" & myAdditionValue.ToString & ")")

                        If MyClass.CountConsumptionToSaveDallasData_CalA >= MyClass.MinConsumptionVolToSaveDallasData_CalA Then
                            Debug.Print("SAVE Consume ISE PENDING (A) !")
                            MyClass.IsCalAUpdateRequiredAttr = True
                        End If

                    End If

                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateReagentsCalAConsumption", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Update Consumption of Calibrator B
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Private Function UpdateReagentsCalBConsumption() As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Try
                Dim myCurrentValue As Single
                Dim myAdditionValue As Single

                ' Get current value 
                myglobal = GetISEInfoSettingValue(ISEModuleSettings.AVAILABLE_VOL_CAL_B)
                If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                    myCurrentValue = CType(myglobal.SetDatos, Single)

                    If myCurrentValue > 0 Then      ' XB 03/12/2013 - Improvment - Do not save Consumptions when this value is negative - task #1414
                        If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.OK Or _
                           MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.CAL Or _
                           MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.PMC Or _
                           MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.BBC Or _
                           MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Or _
                           MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then

                            If MyClass.LastISEResultAttr IsNot Nothing Then

                                If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Then
                                    ' ISE Serum Test
                                    myAdditionValue = MyClass.ConsumptionCalBbySerumAttr
                                ElseIf MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then
                                    ' ISE Urine Test (phase 1)
                                    myAdditionValue = MyClass.ConsumptionCalBbyUrine1Attr
                                    ' ISE Urine Test (phase 2)
                                    myAdditionValue += MyClass.ConsumptionCalBbyUrine2Attr
                                Else
                                    ' ISE Commands

                                    Select Case MyClass.CurrentCommandTOAttr.ISECommandID

                                        Case ISECommands.CALB
                                            myAdditionValue = MyClass.ConsumptionCalBbyElectrodesCalAttr

                                        Case ISECommands.PUMP_CAL
                                            myAdditionValue = MyClass.ConsumptionCalBbyPumpsCalAttr

                                        Case ISECommands.BUBBLE_CAL
                                            myAdditionValue = MyClass.ConsumptionCalBbyBubblesCalAttr

                                        Case ISECommands.CLEAN
                                            myAdditionValue = MyClass.ConsumptionCalBbyCleanCycleAttr

                                        Case ISECommands.PURGEA
                                            myAdditionValue = MyClass.ConsumptionCalBbyPurgeAAttr

                                        Case ISECommands.PURGEB
                                            myAdditionValue = MyClass.ConsumptionCalBbyPurgeBAttr

                                        Case ISECommands.PRIME_CALA
                                            myAdditionValue = MyClass.ConsumptionCalBbyPrimeAAttr

                                        Case ISECommands.PRIME_CALB
                                            myAdditionValue = MyClass.ConsumptionCalBbyPrimeBAttr

                                        Case ISECommands.DSPA
                                            ' No consume Cal B

                                        Case ISECommands.DSPB
                                            If Not MyClass.CurrentCommandTO Is Nothing Then
                                                If MyClass.CurrentCommandTO.ISECommandID = ISECommands.DSPB Then
                                                    If IsNumeric(MyClass.CurrentCommandTO.P1) Then
                                                        myAdditionValue = (CSng(MyClass.CurrentCommandTO.P1) / 1000) ' mL
                                                    End If
                                                End If
                                            End If


                                    End Select

                                End If
                            End If
                        End If

                        ' XBC 02/07/2012
                        ' Add SIPPING consumption
                        If MyClass.SIPcycles > 0 Then
                            myAdditionValue += (MyClass.SIPcycles * ConsumptionCalBbySippingAttr)
                        End If

                        ' Update its value
                        myCurrentValue -= myAdditionValue
                        MyClass.UpdateISEInfoSetting(ISEModuleSettings.AVAILABLE_VOL_CAL_B, myCurrentValue.ToString)


                        MyClass.CountConsumptionToSaveDallasData_CalB += myAdditionValue

                        Debug.Print("CONSUME CAL B : " & MyClass.CountConsumptionToSaveDallasData_CalB.ToString & " (-" & myAdditionValue.ToString & ")")


                        If MyClass.CountConsumptionToSaveDallasData_CalB >= MyClass.MinConsumptionVolToSaveDallasData_CalB Then
                            Debug.Print("SAVE Consume ISE PENDING (B) !")
                            MyClass.IsCalBUpdateRequiredAttr = True
                        End If

                    End If

                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateReagentsCalBConsumption", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Update Consumption of Reference Electrode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Private Function UpdateElectrodeRefConsumption() As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Try
                Dim myCurrentValue As Single

                If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Or _
                   MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then

                    If MyClass.LastISEResultAttr IsNot Nothing Then

                        ' Get current value 
                        myglobal = GetISEInfoSettingValue(ISEModuleSettings.REF_CONSUMPTION)
                        If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                            myCurrentValue = CType(myglobal.SetDatos, Single)

                            If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Then
                                ' ISE Serum Test
                                myCurrentValue += 1
                            ElseIf MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then
                                ' ISE Urine Test
                                myCurrentValue += 1
                            End If

                            ' Update its value
                            MyClass.UpdateISEInfoSetting(ISEModuleSettings.REF_CONSUMPTION, myCurrentValue.ToString)
                        End If
                    End If
                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateElectrodeRefConsumption", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Update Consumption of Lithium Electrode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Private Function UpdateElectrodeLiConsumption() As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Try
                Dim myCurrentValue As Single

                If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Or _
                   MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then

                    If MyClass.LastISEResultAttr IsNot Nothing Then

                        ' Get current value 
                        myglobal = GetISEInfoSettingValue(ISEModuleSettings.LI_CONSUMPTION)
                        If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                            myCurrentValue = CType(myglobal.SetDatos, Single)

                            If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Then
                                ' ISE Serum Test
                                myCurrentValue += 1
                            ElseIf MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then
                                ' ISE Urine Test
                                myCurrentValue += 1
                            End If

                            ' Update its value
                            MyClass.UpdateISEInfoSetting(ISEModuleSettings.LI_CONSUMPTION, myCurrentValue.ToString)
                        End If
                    End If
                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateElectrodeLiConsumption", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Update Consumption of Sodium Electrode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Private Function UpdateElectrodeNaConsumption() As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Try
                Dim myCurrentValue As Single

                If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Or _
                   MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then

                    If MyClass.LastISEResultAttr IsNot Nothing Then

                        ' Get current value 
                        myglobal = GetISEInfoSettingValue(ISEModuleSettings.NA_CONSUMPTION)
                        If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                            myCurrentValue = CType(myglobal.SetDatos, Single)

                            If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Then
                                ' ISE Serum Test
                                myCurrentValue += 1
                            ElseIf MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then
                                ' ISE Urine Test
                                myCurrentValue += 1
                            End If

                            ' Update its value
                            MyClass.UpdateISEInfoSetting(ISEModuleSettings.NA_CONSUMPTION, myCurrentValue.ToString)
                        End If
                    End If
                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateElectrodeNaConsumption", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Update Consumption of Potassium Electrode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Private Function UpdateElectrodeKConsumption() As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Try
                Dim myCurrentValue As Single

                If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Or _
                   MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then

                    If MyClass.LastISEResultAttr IsNot Nothing Then

                        ' Get current value 
                        myglobal = GetISEInfoSettingValue(ISEModuleSettings.K_CONSUMPTION)
                        If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                            myCurrentValue = CType(myglobal.SetDatos, Single)

                            If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Then
                                ' ISE Serum Test
                                myCurrentValue += 1
                            ElseIf MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then
                                ' ISE Urine Test
                                myCurrentValue += 1
                            End If

                            ' Update its value
                            MyClass.UpdateISEInfoSetting(ISEModuleSettings.K_CONSUMPTION, myCurrentValue.ToString)
                        End If
                    End If
                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateElectrodeKConsumption", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Update Consumption of Clorine Electrode
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Private Function UpdateElectrodeClConsumption() As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Try
                Dim myCurrentValue As Single

                If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Or _
                   MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then

                    If MyClass.LastISEResultAttr IsNot Nothing Then

                        ' Get current value 
                        myglobal = GetISEInfoSettingValue(ISEModuleSettings.CL_CONSUMPTION)
                        If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                            myCurrentValue = CType(myglobal.SetDatos, Single)

                            If MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.SER Then
                                ' ISE Serum Test
                                myCurrentValue += 1
                            ElseIf MyClass.LastISEResultAttr.ISEResultType = ISEResultTO.ISEResultTypes.URN Then
                                ' ISE Urine Test
                                myCurrentValue += 1
                            End If

                            ' Update its value
                            MyClass.UpdateISEInfoSetting(ISEModuleSettings.CL_CONSUMPTION, myCurrentValue.ToString)
                        End If
                    End If
                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateElectrodeClConsumption", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Initilize consumptions calculation parameters
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Private Function InitializeReagentsConsumptions() As GlobalDataTO
            Dim myglobal As New GlobalDataTO
            Try
                If MyClass.ISEDallasPage00Attr IsNot Nothing Then           'JBL 12/09/2012 - Correction: verify null reference
                    Dim myInitialCalibAVolume As Single
                    Dim myInitialCalibBVolume As Single

                    myInitialCalibAVolume = MyClass.ISEDallasPage00Attr.InitialCalibAVolume
                    myInitialCalibBVolume = MyClass.ISEDallasPage00Attr.InitialCalibBVolume

                    MyClass.MinConsumptionVolToSaveDallasData_CalA = myInitialCalibAVolume * CteVolumeToSaveDallasData / 100
                    MyClass.MinConsumptionVolToSaveDallasData_CalB = myInitialCalibBVolume * CteVolumeToSaveDallasData / 100

                    ' XBC 31/05/2012
                    'MyClass.CountConsumptionToSaveDallasData_CalA = 0
                    'MyClass.CountConsumptionToSaveDallasData_CalB = 0
                    ' XBC 31/05/2012
                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.InitializeReagentsConsumptions", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Save consumption of calibrator A consume into Dallas Xip
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Public Function SaveConsumptionCalToDallasData(ByVal pCalibrator As String) As GlobalDataTO Implements IISEManager.SaveConsumptionCalToDallasData
            Dim myglobal As New GlobalDataTO
            Dim myDataIseCmdTo As New ISECommandTO
            'Dim myUtility As New Utilities()
            Try
                Dim myPositionToSave As String
                Dim myPosition As String
                Dim myInfo As String

                myglobal = MyClass.GetNextPositionToSaveConsumptionCal(pCalibrator)
                If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                    myPositionToSave = CType(myglobal.SetDatos, String)

                    myPosition = myPositionToSave.Substring(0, 2)
                    myInfo = myPositionToSave.Substring(2, 3)

                    ' XBC 15/03/2012
                    ' Sw envia la informacion al Fw en decimal (porque así lo espera Fw) y es Fw quien lo convierte a Hexadecimal
                    'If IsNumeric(myPosition) Then
                    '    myglobal = Utilities.ConvertDecimalToHex(CLng(myPosition))
                    '    If Not myglobal.HasError AndAlso Not myglobal.SetDatos Is Nothing Then
                    '        myPosition = CType(myglobal.SetDatos, String)
                    '    Else
                    '        Exit Try
                    '    End If
                    'Else
                    '    Exit Try
                    'End If

                    'If IsNumeric(myInfo) Then
                    '    myglobal = Utilities.ConvertDecimalToHex(CLng(myInfo))
                    '    If Not myglobal.HasError AndAlso Not myglobal.SetDatos Is Nothing Then
                    '        myInfo = CType(myglobal.SetDatos, String)
                    '    Else
                    '        Exit Try
                    '    End If
                    'Else
                    '    Exit Try
                    'End If
                    ' XBC 15/03/2012

                    myglobal = MyClass.PrepareDataToSend_WRITE_DALLAS(myPosition, myInfo)
                    If Not myglobal.HasError AndAlso Not myglobal.SetDatos Is Nothing Then
                        myDataIseCmdTo = CType(myglobal.SetDatos, ISECommandTO)
                    Else
                        Exit Try
                    End If

                    myglobal.SetDatos = myDataIseCmdTo

                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.SaveConsumptionCalAToDallasData", EventLogEntryType.Error, False)
            End Try
            Return myglobal
        End Function

        ''' <summary>
        ''' Get the next position allowed to write into Dallas Xip by consumption of calibrator A or B
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 08/03/2012</remarks>
        Private Function GetNextPositionToSaveConsumptionCal(ByVal pCalibrator As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim pHexString As String
                Dim returnValue As String = ""

                If pCalibrator = "A" Then
                    pHexString = MyClass.ISEDallasPage01Attr.ConsumptionCalAbitData.ToString
                Else
                    pHexString = MyClass.ISEDallasPage01Attr.ConsumptionCalBbitData.ToString
                End If

                'each byte (2 char) represents 8%, one per bit
                Dim myBytes As New List(Of String)
                For c As Integer = 0 To pHexString.Length - 1 Step 2
                    myBytes.Add(pHexString.Substring(c, 2)) 'it must be 13 length
                Next

                Dim i As Integer
                If pCalibrator = "A" Then
                    i = DallasXipPage1.FirstPositionConsumptionCalA
                Else
                    i = DallasXipPage1.FirstPositionConsumptionCalB
                End If
                For Each B As String In myBytes

                    ' XBC 15/03/2012
                    ' Sw envia la informacion al Fw en decimal (porque así lo espera Fw) y es Fw quien lo convierte a Hexadecimal
                    Select Case UCase(B)
                        Case "FF"
                            returnValue = i.ToString("00") + "247" ' "F7"   
                            Exit For
                        Case "F7"
                            returnValue = i.ToString("00") + "243" ' "F3"   
                            Exit For
                        Case "F3"
                            returnValue = i.ToString("00") + "241" ' "F1"   
                            Exit For
                        Case "F1"
                            returnValue = i.ToString("00") + "240" ' "F0"   
                            Exit For
                        Case "F0"
                            returnValue = i.ToString("00") + "112" ' "70"   
                            Exit For
                        Case "70"
                            returnValue = i.ToString("00") + "048" ' "30"   
                            Exit For
                        Case "30"
                            returnValue = i.ToString("00") + "016" ' "10"   
                            Exit For
                        Case "10"
                            returnValue = i.ToString("00") + "000" ' "00"   
                            Exit For
                        Case Else
                            ' continue For with next position
                    End Select

                    i += 1

                    ' XBC 03/08/2012 - Protection when overflow the Last position to write
                    If pCalibrator = "A" Then
                        If i > DallasXipPage1.LastPositionConsumptionCalA Then
                            myGlobal.HasError = True
                            Exit Try
                        End If
                    Else
                        If i > DallasXipPage1.LastPositionConsumptionCalB Then
                            myGlobal.HasError = True
                            Exit Try
                        End If
                    End If

                Next

                myGlobal.SetDatos = returnValue

            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetNextPositionToSaveConsumptionCalA", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Calculate an estimation of SIP cycles completed
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/07/2012</remarks>
        Private Function EstimatedSIPCycles() As Single
            Dim myglobal As New GlobalDataTO
            Dim returnValue As Single = 0
            Try
                Dim myLastOperationDate As DateTime
                ' Get date of the last operation
                myglobal = GetISEInfoSettingValue(ISEModuleSettings.LAST_OPERATION_DATE, True)
                If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing AndAlso SIPIntervalConsumption > 0 Then
                    myLastOperationDate = CDate(myglobal.SetDatos)

                    Dim myspan As TimeSpan
                    myspan = DateTime.Now.Subtract(myLastOperationDate)
                    Dim mySeconds As Long
                    mySeconds = myspan.Seconds + _
                                myspan.Minutes * 60 + _
                                myspan.Hours * 60 * 60 + _
                                myspan.Days * 24 * 60 * 60

                    returnValue = Int(mySeconds / SIPIntervalConsumption)
                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.EstimatedSIPCycles", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function


        ''' <summary>
        ''' Calculate an estimation of consumptions spent by Firmware
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 17/07/2012</remarks>
        Public Function EstimatedFWConsumptionWS() As Long Implements IISEManager.EstimatedFWConsumptionWS
            Dim myglobal As New GlobalDataTO
            Dim returnValue As Long = 0
            Try
                If Not MyClass.WorkSessionIsRunningAttr Then
                    ' No Sessino has fininshed
                    Exit Function
                Else
                    MyClass.WorkSessionIsRunningAttr = False
                End If

                Dim myCurrentValue As Single
                Dim myAdditionValue As Single

                '
                ' FIRST CALCULATE FIXED CONSUMPTIONS 
                '

                ' fixed operations executed by Firmaware when WorkSession starts (with or without ISE tests !)
                MyClass.PurgeAbyFirmwareAttr += 1
                MyClass.PurgeBbyFirmwareAttr += 1



                '
                ' SECOND CALCULATE VARIABLE CONSUMPTIONS
                '

                ' Firmware throw a Purge A every 25 minutes with no activity ISE

                If WorkSessionTestsByTypeAttr = "ISE" Then
                    ' WS just with ISE preparations
                    ' None additional Purge A is accounted. Just the Purges associated to ISE preparations will be accounted
                ElseIf WorkSessionTestsByTypeAttr = "STD" Then
                    ' WS Just with STD preparations

                    ' variable times depending on work session time duration
                    If Interval1forPurgeACompletedbyFW > 0 AndAlso MyClass.WorkSessionOverallTimeAttr > Interval1forPurgeACompletedbyFW Then
                        MyClass.PurgeAbyFirmwareAttr += CInt(Math.Truncate(MyClass.WorkSessionOverallTimeAttr / Interval1forPurgeACompletedbyFW))
                    End If
                Else
                    ' WS with ISE and STD preparations
                    ' Estimation consumptions is difficult to estimate ! PENDING !!! - Canceled by now
                End If


                '
                ' PURGE A CONSUMPTION
                '

                ' Get current value for Calibrator A
                myglobal = GetISEInfoSettingValue(ISEModuleSettings.AVAILABLE_VOL_CAL_A)
                If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                    myCurrentValue = CType(myglobal.SetDatos, Single)

                    myAdditionValue = MyClass.ConsumptionCalAbyPurgeAAttr * MyClass.PurgeAbyFirmwareAttr
                    ' initialize it
                    MyClass.PurgeAbyFirmwareAttr = 0
                End If
                ' Update its value for Calibrator A
                myCurrentValue -= myAdditionValue
                MyClass.UpdateISEInfoSetting(ISEModuleSettings.AVAILABLE_VOL_CAL_A, myCurrentValue.ToString)

                MyClass.CountConsumptionToSaveDallasData_CalA += myAdditionValue

                '
                ' PURGE B CONSUMPTION
                '

                ' Get current value for Calibrator B
                myglobal = GetISEInfoSettingValue(ISEModuleSettings.AVAILABLE_VOL_CAL_B)
                If Not myglobal.HasError AndAlso myglobal.SetDatos IsNot Nothing Then
                    myCurrentValue = CType(myglobal.SetDatos, Single)

                    myAdditionValue = MyClass.ConsumptionCalBbyPurgeBAttr * MyClass.PurgeBbyFirmwareAttr
                    ' initialize it
                    MyClass.PurgeAbyFirmwareAttr = 0
                End If
                ' Update its value for Calibrator B
                myCurrentValue -= myAdditionValue
                MyClass.UpdateISEInfoSetting(ISEModuleSettings.AVAILABLE_VOL_CAL_B, myCurrentValue.ToString)

                MyClass.CountConsumptionToSaveDallasData_CalB += myAdditionValue


                '
                ' FINALLY CHECK IF IS NEED WRITE INTO MODULE
                '

                ' Check if is need to write into Reagents Pack
                If MyClass.CountConsumptionToSaveDallasData_CalA >= MyClass.MinConsumptionVolToSaveDallasData_CalA Then
                    MyClass.IsCalAUpdateRequiredAttr = True
                End If
                If MyClass.CountConsumptionToSaveDallasData_CalB >= MyClass.MinConsumptionVolToSaveDallasData_CalB Then
                    MyClass.IsCalBUpdateRequiredAttr = True
                End If

            Catch ex As Exception
                myglobal.SetDatos = False
                myglobal.HasError = True
                myglobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myglobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.EstimatedFWConsumptionWS", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function

#End Region

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 08/02/2012
        ''' Modified by XB 23/10/2013 - ISECMDs instructions are allowed to send while Analyzer is in Running during Pause mode - BT #1343
        ''' </remarks>
        Public Function SendISECommand() As GlobalDataTO Implements IISEManager.SendISECommand
            Dim myGlobal As New GlobalDataTO
            Try
                If MyClass.CurrentCommandTO IsNot Nothing Then
                    MyClass.CurrentCommandTO.SentDatetime = DateTime.Now

                    If MyClass.CurrentCommandTO.ISECommandID = ISECommands.WRITE_CALA_CONSUMPTION Or _
                        MyClass.CurrentCommandTO.ISECommandID = ISECommands.WRITE_CALB_CONSUMPTION Then
                        ManageISEProcedureFinished(ISEProcedureResult.OK)
                    Else
                        ' XB 23/10/2013
                        ' If MyClass.myAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
                        If MyClass.myAnalyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Or _
                           (MyClass.myAnalyzer.AnalyzerStatus = AnalyzerManagerStatus.RUNNING And MyClass.myAnalyzer.AllowScanInRunning) Then
                            ' XB 23/10/2013

                            myGlobal = myAnalyzer.ManageAnalyzer(AnalyzerManagerSwActionList.ISE_CMD, True, Nothing, MyClass.CurrentCommandTO)
                            If Not myGlobal.HasError Then

                                ' XBC 05/09/2012 - Start Timeout must emplaced inside ManageAnalyzer
                                ' because is the last point when the instruction is sent 
                                ' There was a malfunction when any instruction get in the queue due 
                                ' send it when Analyzer was Ready
                                'myGlobal = MyClass.StartInstructionStartedTimer() 'SGM 7/6/2012
                                ' XBC 05/09/2012 

                            End If
                        Else
                            ManageISEProcedureFinished(ISEProcedureResult.NOK)
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.SendISECommand", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Moves the R2 arm away from park position
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 05/07/2012</remarks>
        Public Function MoveR2ArmAway() As GlobalDataTO Implements IISEManager.MoveR2ArmAway
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.SingleCommand
                MyClass.PrepareDataToSend_R2_TO_WASH()
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.MoveR2ArmAway", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Moves the R2 arm back to park position
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 05/07/2012</remarks>
        Public Function MoveR2ArmBack() As GlobalDataTO Implements IISEManager.MoveR2ArmBack
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.SingleCommand
                MyClass.PrepareDataToSend_R2_TO_PARK()
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.MoveR2ArmBack", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        Public Function SetElectrodesInstallDates(ByVal pRef As DateTime, ByVal pNa As DateTime, _
                                                  ByVal pK As DateTime, ByVal pCl As DateTime, _
                                                  ByVal pLi As DateTime, Optional ByVal pSimulationMode As Boolean = False) As GlobalDataTO Implements IISEManager.SetElectrodesInstallDates

            Dim resultData As New GlobalDataTO
            Try
                If pSimulationMode Then ' XBC 29/06/2012
                    MyClass.IsNaMountedAttr = True
                    MyClass.IsKMountedAttr = True
                    MyClass.IsClMountedAttr = True
                    MyClass.IsLiMountedAttr = True
                End If

                If MyClass.IsRefMounted Then MyClass.RefInstallDate = pRef : MyClass.RefTestCount = 0
                If MyClass.IsNaMounted Then MyClass.NaInstallDate = pNa : MyClass.NaTestCount = 0
                If MyClass.IsKMounted Then MyClass.KInstallDate = pK : MyClass.KTestCount = 0
                If MyClass.IsClMounted Then MyClass.ClInstallDate = pCl : MyClass.ClTestCount = 0
                If MyClass.IsLiEnabledByUser And MyClass.IsLiMounted Then MyClass.LiInstallDate = pLi : MyClass.LiTestCount = 0

                resultData = MyClass.RefreshMonitorDataTO

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.SetElectrodesInstallDates", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        Public Function SetReagentsPackInstallDate(ByVal pdate As DateTime, Optional ByVal pResetConsumptions As Boolean = False) As GlobalDataTO Implements IISEManager.SetReagentsPackInstallDate

            Dim resultData As New GlobalDataTO
            Try

                MyClass.ReagentsPackInstallationDate = pdate

                If MyClass.ISEDallasPage00 IsNot Nothing AndAlso pResetConsumptions Then
                    'after installation get the initial volumes

                    ' XBC+SG 26/09/2012 - Correction : calculate with same unit (milimeters)
                    'If Not resultData.HasError Then MyClass.UpdateISEInfoSetting(ISEModuleSettings.AVAILABLE_VOL_CAL_A, (MyClass.ISEDallasPage00.InitialCalibAVolume - MyClass.ISEDallasPage01.ConsumptionCalA).ToString)
                    'If Not resultData.HasError Then MyClass.UpdateISEInfoSetting(ISEModuleSettings.AVAILABLE_VOL_CAL_B, (MyClass.ISEDallasPage00.InitialCalibBVolume - MyClass.ISEDallasPage01.ConsumptionCalB).ToString)
                    If Not resultData.HasError Then MyClass.UpdateISEInfoSetting(ISEModuleSettings.AVAILABLE_VOL_CAL_A, (MyClass.ISEDallasPage00.InitialCalibAVolume - MyClass.GetValueFromPercent(ISEDallasPage00.InitialCalibAVolume, MyClass.ISEDallasPage01.ConsumptionCalA)).ToString)
                    If Not resultData.HasError Then MyClass.UpdateISEInfoSetting(ISEModuleSettings.AVAILABLE_VOL_CAL_B, (MyClass.ISEDallasPage00.InitialCalibBVolume - MyClass.GetValueFromPercent(ISEDallasPage00.InitialCalibBVolume, MyClass.ISEDallasPage01.ConsumptionCalB)).ToString)
                    ' XBC+SG 26/09/2012

                    If Not resultData.HasError Then MyClass.UpdateISEInformationTable()
                End If

                If Not resultData.HasError Then resultData = MyClass.RefreshMonitorDataTO

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.SetReagentsPackInstallDate", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update Installation Date Pump Tubing
        ''' </summary>
        ''' <param name="pDate"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/03/2012</remarks>
        Public Function SetPumpTubingInstallDate(ByVal pDate As DateTime) As GlobalDataTO Implements IISEManager.SetPumpTubingInstallDate
            Dim resultData As New GlobalDataTO
            Try
                MyClass.PumpTubingInstallDate = pDate
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.SetPumpTubingInstallDate", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Update Installation Date Fluidic Tubing
        ''' </summary>
        ''' <param name="pDate"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/03/2012</remarks>
        Public Function SetFluidicTubingInstallDate(ByVal pDate As DateTime) As GlobalDataTO Implements IISEManager.SetFluidicTubingInstallDate
            Dim resultData As New GlobalDataTO
            Try
                MyClass.FluidicTubingInstallDate = pDate
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.SetFluidicTubingInstallDate", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Installs ISE Module
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/03/2012</remarks>
        Public Function InstallISEModule() As GlobalDataTO Implements IISEManager.InstallISEModule
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.ActivateModule
                MyClass.PrepareDataToSend_READ_PAGE_DALLAS(0)
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.InstallISEModule", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function



        '-	Deactivate ISE Module (Long Term Deactivation)
        ''' <summary>
        ''' Activate ISE Module
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/03/2012</remarks>
        Public Function DeactivateISEModule(ByVal pDeactivationMode As Boolean) As GlobalDataTO Implements IISEManager.DeactivateISEModule
            Dim resultData As New GlobalDataTO
            Try
                MyClass.IsLongTermDeactivation = pDeactivationMode

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DeactivateISEModule", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Activate Reagent Pack
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 07/03/2012
        ''' Modified by SGM 16/01/2013 - Bug #1108
        ''' </remarks>
        Public Function ActivateReagentsPack() As GlobalDataTO Implements IISEManager.ActivateReagentsPack
            Dim resultData As New GlobalDataTO
            Try
                ' XBC 18/07/2012 - Initialize database consumptions counters
                ' Initialize Consumptions Cal A and Cal B with the volumes corresponding with the volumes read from Reagents Pack
                MyClass.ForceConsumptionsInitialize = True
                ' Initialize Dates for SIPs estimation consumptions
                MyClass.UpdateISEInfoSetting(ISEModuleSettings.LAST_OPERATION_DATE, DateTime.Now.ToString, True)
                ' XBC 18/07/2012


                'MyClass.ElectrodesActivationRequested = True

                MyClass.CurrentProcedure = ISEProcedures.ActivateReagentsPack

                'SGM 16/01/2013 Start reading data from RP: Bug #1108
                Debug.Print("Activant Reagents Pack ...")
                MyClass.PrepareDataToSend_READ_PAGE_DALLAS(0)
                resultData = MyClass.SendISECommand()
                If Not resultData.HasError Then
                    Debug.Print("Reading DALLAS PAGE 00 ...")
                End If
                'end SGM 16/01/2013

                '' XBC 04/05/2012
                'If MyClass.ReagentsPackInstallationDate <> Nothing Then
                '    MyClass.PrepareDataToSend_READ_PAGE_DALLAS(1)
                '    Debug.Print("Activant Reagents Pack ...")

                '    resultData = MyClass.SendISECommand()
                'Else
                '    MyClass.PrepareDataToSend_WRITE_INSTALL_DAY(Day(Now))
                '    Debug.Print("Guardant DIA Reagents Pack ..." & Day(Now).ToString)

                '    resultData = MyClass.SendISECommand()
                '    If Not resultData.HasError Then
                '        MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.WRITE_DAY_INSTALL
                '        Debug.Print("Canvi d'estat a WRITE_DAY_INSTALL ...")
                '    End If
                'End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ActivateReagentsPack", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set Electrodes Installation dates
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/03/2012</remarks>
        Public Function ActivateElectrodes() As GlobalDataTO Implements IISEManager.ActivateElectrodes
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.ActivateElectrodes
                MyClass.PrepareDataToSend_READ_mV()
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ActivateElectrodes", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Enables ISE preparations
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/03/2012</remarks>
        Public Function ActivateISEPreparations() As GlobalDataTO Implements IISEManager.ActivateISEPreparations
            Dim resultData As New GlobalDataTO
            Try

                'MyClass.IsReadyForISEPreparations = True
                resultData.SetDatos = True

                ' TODO : llamar a la funcion de Worksesion para desbloquear preparaciones de ISE

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.ActivateISEpreparations", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Performs a PrimeB + PrimeA + CalB
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 11/06/2012</remarks>
        Public Function DoPrimeAndCalibration() As GlobalDataTO Implements IISEManager.DoPrimeAndCalibration
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.PrimeAndCalibration
                MyClass.PrepareDataToSend_PRIME_CALB()
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DoPrimeAndCalibration", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Performs a PrimeB + PrimeA + PrimeB + PrimeA + CalB
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 11/06/2012</remarks>
        Public Function DoPrimeX2AndCalibration() As GlobalDataTO Implements IISEManager.DoPrimeX2AndCalibration
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.PrimeX2AndCalibration
                MyClass.PrepareDataToSend_PRIME_CALB()
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DoPrimeX2AndCalibration", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Check Reagents pack and Electrodes
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/03/2012</remarks>
        Public Function DoGeneralCheckings(Optional ByVal pIsRPAlreadyRead As Boolean = False) As GlobalDataTO Implements IISEManager.DoGeneralCheckings
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.GeneralCheckings
                If Not pIsRPAlreadyRead Then
                    MyClass.PrepareDataToSend_POLL()
                Else
                    MyClass.PrepareDataToSend_READ_mV()
                End If

                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DoGeneralCheckings", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Perform Cleaning operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SGM 07/03/2012
        ''' </remarks>
        Public Function DoCleaning(ByVal pTubePos As Integer) As GlobalDataTO Implements IISEManager.DoCleaning
            Dim resultData As New GlobalDataTO
            Try
                MyClass.CurrentProcedure = ISEProcedures.Clean
                MyClass.PrepareDataToSend_CLEAN(pTubePos, MyClass.AnalyzerIDAttr)
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DoCleaning", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' do utilities exit procedure
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/03/2012</remarks>
        Public Function DoMaintenanceExit() As GlobalDataTO Implements IISEManager.DoMaintenanceExit
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.MaintenanceExit
                MyClass.PrepareDataToSend_PURGEA()
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DoMaintenanceExit", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' do Purge (A/B)
        ''' </summary>
        ''' <param name="pType">"A" or "B"</param>
        ''' <returns></returns>
        ''' <remarks>Created by XB 28/04/2014 - Improve Initial Purges sends before StartWS - Task #1587</remarks>
        Public Function DoPurge(ByVal pType As String) As GlobalDataTO Implements IISEManager.DoPurge
            Dim resultData As New GlobalDataTO
            Try
                MyClass.CurrentProcedure = ISEProcedures.SingleCommand
                Select Case pType
                    Case "A"
                        MyClass.PrepareDataToSend_PURGEA()
                    Case "B"
                        MyClass.PrepareDataToSend_PURGEB()
                End Select
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DoPurge" & pType, EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        '''' <summary>
        '''' Writes Consumption
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SGM 07/03/2012</remarks>
        'Public Function DoWriteConsumption() As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        Return myGlobal 'PENDING!!!!!!!!!!!
        '        If MyClass.myAnalyzerManager.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then
        '            If MyClass.IsCalAUpdateRequired Then 'update cal A consumption
        '                MyClass.CurrentProcedure = ISEProcedures.WriteCalAConsumption
        '                ' Save it into Dallas Xip
        '                myGlobal = SaveConsumptionCalToDallasData("A")
        '                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
        '                    MyClass.CurrentCommandTO = CType(myGlobal.SetDatos, ISECommandTO)

        '                    MyClass.SendISECommand()
        '                End If
        '            End If
        '            If MyClass.IsCalBUpdateRequired Then 'update calB consumption
        '                MyClass.CurrentProcedure = ISEProcedures.WriteCalBConsumption
        '                ' Save it into Dallas Xip
        '                myGlobal = SaveConsumptionCalToDallasData("B")
        '                If Not myGlobal.HasError AndAlso Not myGlobal.SetDatos Is Nothing Then
        '                    MyClass.CurrentCommandTO = CType(myGlobal.SetDatos, ISECommandTO)

        '                    MyClass.SendISECommand()
        '                End If

        '            End If
        '        End If


        '    Catch ex As Exception
        '        myGlobal = New GlobalDataTO()
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString()
        '        myGlobal.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DoWriteConsumption", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function

        ''' <summary>
        ''' Clean Pack Installed
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/03/2012</remarks>
        Public Function CheckCleanPackInstalled() As GlobalDataTO Implements IISEManager.CheckCleanPackInstalled
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.CheckCleanPackInstalled
                MyClass.PrepareDataToSend_PURGEA()
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckCleanPackInstalled", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function



        ''' <summary>
        ''' Check ISE module Alarms
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 26/03/2012
        ''' Modified by XB 04/11/2014 - Add ISE Timeout Alarm - BA-1872
        ''' </remarks>
        Public Function CheckAlarms(ByVal pConnectedAttribute As Boolean, ByRef pAlarmList As List(Of GlobalEnumerates.Alarms), ByRef pAlarmStatusList As List(Of Boolean)) As GlobalDataTO Implements IISEManager.CheckAlarms
            Dim resultData As New GlobalDataTO
            Try
                Dim treat As Boolean = True

                If Not pConnectedAttribute Then
                    treat = True
                Else
                    If Not MyClass.IsISEModuleInstalled Then
                        treat = False
                    ElseIf MyClass.myAnalyzer.AnalyzerStatus = AnalyzerManagerStatus.SLEEPING Then
                        treat = False
                    ElseIf MyClass.IsISEInitiating Then
                        treat = False
                    ElseIf MyClass.IsAnalyzerWarmUp Then 'SGM 17/05/2012
                        treat = False
                    End If
                End If



                Dim alarmID As GlobalEnumerates.Alarms = GlobalEnumerates.Alarms.NONE
                Dim alarmStatus As Boolean = False

                If treat Then

                    ' Ise module is deativated by longterm
                    If MyClass.IsLongTermDeactivation Then
                        alarmID = GlobalEnumerates.Alarms.ISE_LONG_DEACT_ERR
                        alarmStatus = True
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                        myAlarms(Alarms.LongTermDeactivated) = True

                        'SGM 14/06/2012
                        'deactivate all ISE Alarms
                        alarmStatus = False

                        'ReagentsPack_Invalid
                        myAlarms(Alarms.ReagentsPack_Invalid) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_RP_INVALID_ERR
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                        'ReagentsPack_Depleted
                        myAlarms(Alarms.ReagentsPack_Depleted) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_RP_DEPLETED_ERR
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                        'ReagentsPack_Expired
                        myAlarms(Alarms.ReagentsPack_Expired) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_RP_EXPIRED_WARN
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                        'Electrodes_Wrong
                        myAlarms(Alarms.Electrodes_Wrong) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_ELEC_WRONG_ERR
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                        'Electrodes_Cons_Expired
                        myAlarms(Alarms.Electrodes_Cons_Expired) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_ELEC_CONS_WARN
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                        'Electrodes_Date_Expired
                        myAlarms(Alarms.Electrodes_Date_Expired) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_ELEC_DATE_WARN
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                        'CleanPack_Installed
                        myAlarms(Alarms.CleanPack_Installed) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_CP_INSTALL_WARN
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                        'CleanPack_Wrong
                        myAlarms(Alarms.CleanPack_Wrong) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_CP_WRONG_ERR
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                        'ReagentsPack_DateInstall
                        myAlarms(Alarms.ReagentsPack_DateInstall) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_RP_NO_INST_ERR
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                        'switch off
                        myAlarms(Alarms.Switch_Off) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_OFF_ERR
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)
                        'SGM 14/06/2012

                        'Timeout    XB 04/11/2014 - BA-1872
                        myAlarms(Alarms.Timeout) = False
                        alarmID = GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR
                        pAlarmList.Add(alarmID)
                        pAlarmStatusList.Add(alarmStatus)

                    Else
                        If myAlarms(Alarms.LongTermDeactivated) Then
                            myAlarms(Alarms.LongTermDeactivated) = False
                            ' solved
                            alarmID = GlobalEnumerates.Alarms.ISE_LONG_DEACT_ERR
                            alarmStatus = False
                            pAlarmList.Add(alarmID)
                            pAlarmStatusList.Add(alarmStatus)
                        End If

                        If Not pConnectedAttribute Then
                            MyClass.CheckElectrodesCalibrationIsNeeded()
                            MyClass.CheckPumpsCalibrationIsNeeded()
                            MyClass.CheckCleanIsNeeded()
                        End If



                        ''SGM 18/06/2012
                        'If Not MyClass.IsISESwitchON Then

                        '    alarmID = GlobalEnumerates.Alarms.ISE_CONNECT_PDT_ERR
                        '    alarmStatus = False
                        '    pAlarmList.Add(alarmID)
                        '    pAlarmStatusList.Add(alarmStatus)

                        '    alarmID = GlobalEnumerates.Alarms.ISE_OFF_ERR
                        '    alarmStatus = True
                        '    pAlarmList.Add(alarmID)
                        '    pAlarmStatusList.Add(alarmStatus)

                        '    myAlarms(Alarms.Switch_Off) = True

                        'End If

                        If MyClass.IsISESwitchON Then
                            If Not MyClass.IsCleanPackInstalled Then

                                ' Reagents Pack Install Date
                                If MyClass.ReagentsPackInstallationDate = Nothing Then

                                    If MyClass.IsISEInitiatedOK Then

                                        ' Reagents Pack valid
                                        If Not MyClass.HasBiosystemsCode Then
                                            alarmID = GlobalEnumerates.Alarms.ISE_RP_INVALID_ERR
                                            alarmStatus = True
                                            pAlarmList.Add(alarmID)
                                            pAlarmStatusList.Add(alarmStatus)

                                            myAlarms(Alarms.ReagentsPack_Invalid) = True

                                        Else
                                            alarmID = GlobalEnumerates.Alarms.ISE_RP_NO_INST_ERR
                                            alarmStatus = True
                                            pAlarmList.Add(alarmID)
                                            pAlarmStatusList.Add(alarmStatus)

                                            myAlarms(Alarms.ReagentsPack_DateInstall) = True

                                        End If

                                    End If

                                Else
                                    If myAlarms(Alarms.ReagentsPack_DateInstall) Then
                                        myAlarms(Alarms.ReagentsPack_DateInstall) = False
                                        ' solved
                                        alarmID = GlobalEnumerates.Alarms.ISE_RP_NO_INST_ERR
                                        alarmStatus = False
                                        pAlarmList.Add(alarmID)
                                        pAlarmStatusList.Add(alarmStatus)
                                    End If


                                    If Not MyClass.IsReagentsPackReady Then

                                        ''SGM 31/08/2012 - not to show alarm in case of data not loaded yet
                                        'If MyClass.ISEDallasSN IsNot Nothing And _
                                        '   MyClass.ISEDallasPage00 IsNot Nothing And _
                                        '   MyClass.ISEDallasPage01 IsNot Nothing Then

                                        'SGM 16/01/2013 Bug #1108
                                        If MyClass.IsISEInitiatedOK And Not MyClass.HasBiosystemsCode Then
                                            If MyClass.ISEDallasSN IsNot Nothing And _
                                                MyClass.ISEDallasPage00 IsNot Nothing And _
                                                MyClass.ISEDallasPage01 IsNot Nothing Then

                                                alarmID = GlobalEnumerates.Alarms.ISE_RP_INVALID_ERR
                                                alarmStatus = True
                                                pAlarmList.Add(alarmID)
                                                pAlarmStatusList.Add(alarmStatus)

                                                myAlarms(Alarms.ReagentsPack_Invalid) = True
                                            End If

                                        ElseIf MyClass.IsISEInitiatedOK And MyClass.ReagentsPackInstallationDate = Nothing Then
                                            alarmID = GlobalEnumerates.Alarms.ISE_RP_NO_INST_ERR
                                            alarmStatus = True
                                            pAlarmList.Add(alarmID)
                                            pAlarmStatusList.Add(alarmStatus)

                                            myAlarms(Alarms.ReagentsPack_DateInstall) = True

                                        ElseIf MyClass.IsCleanPackInstalled Then
                                            alarmID = GlobalEnumerates.Alarms.ISE_CP_INSTALL_WARN
                                            alarmStatus = True
                                            pAlarmList.Add(alarmID)
                                            pAlarmStatusList.Add(alarmStatus)

                                            myAlarms(Alarms.CleanPack_Installed) = True
                                        End If
                                        'End SGM 16/01/2013 Bug #1108

                                    Else
                                        'reset alarm RP invalid
                                        If myAlarms(Alarms.ReagentsPack_Invalid) Then
                                            myAlarms(Alarms.ReagentsPack_Invalid) = False
                                            ' solved
                                            alarmID = GlobalEnumerates.Alarms.ISE_RP_INVALID_ERR
                                            alarmStatus = False
                                            pAlarmList.Add(alarmID)
                                            pAlarmStatusList.Add(alarmStatus)
                                        End If
                                        'reset CP Installed
                                        If myAlarms(Alarms.CleanPack_Installed) Then
                                            myAlarms(Alarms.CleanPack_Installed) = False
                                            ' solved
                                            alarmID = GlobalEnumerates.Alarms.ISE_CP_INSTALL_WARN
                                            alarmStatus = False
                                            pAlarmList.Add(alarmID)
                                            pAlarmStatusList.Add(alarmStatus)
                                        End If
                                    End If

                                    If MyClass.MonitorDataTO IsNot Nothing Then
                                        ' Ise Reagents Pack depleted
                                        If Not MyClass.MonitorDataTO.RP_IsEnoughVolA Or _
                                           Not MyClass.MonitorDataTO.RP_IsEnoughVolB Then

                                            alarmID = GlobalEnumerates.Alarms.ISE_RP_DEPLETED_ERR
                                            alarmStatus = True
                                            pAlarmList.Add(alarmID)
                                            pAlarmStatusList.Add(alarmStatus)

                                            myAlarms(Alarms.ReagentsPack_Depleted) = True
                                        Else
                                            If myAlarms(Alarms.ReagentsPack_Depleted) Then
                                                myAlarms(Alarms.ReagentsPack_Depleted) = False
                                                ' solved
                                                alarmID = GlobalEnumerates.Alarms.ISE_RP_DEPLETED_ERR
                                                alarmStatus = False
                                                pAlarmList.Add(alarmID)
                                                pAlarmStatusList.Add(alarmStatus)
                                            End If
                                        End If

                                        ' Ise Reagents Pack Expired by date
                                        If MyClass.MonitorDataTO.RP_IsExpired Then

                                            alarmID = GlobalEnumerates.Alarms.ISE_RP_EXPIRED_WARN
                                            alarmStatus = True
                                            pAlarmList.Add(alarmID)
                                            pAlarmStatusList.Add(alarmStatus)

                                            myAlarms(Alarms.ReagentsPack_Expired) = True
                                        Else
                                            If myAlarms(Alarms.ReagentsPack_Expired) Then
                                                myAlarms(Alarms.ReagentsPack_Expired) = False
                                                ' solved
                                                alarmID = GlobalEnumerates.Alarms.ISE_RP_EXPIRED_WARN
                                                alarmStatus = False
                                                pAlarmList.Add(alarmID)
                                                pAlarmStatusList.Add(alarmStatus)
                                            End If
                                        End If
                                    End If
                                End If

                            Else
                                If myAlarms(Alarms.ReagentsPack_Invalid) Then
                                    myAlarms(Alarms.ReagentsPack_Invalid) = False
                                    ' solved
                                    alarmID = GlobalEnumerates.Alarms.ISE_RP_INVALID_ERR
                                    alarmStatus = False
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)
                                End If
                                If myAlarms(Alarms.ReagentsPack_Depleted) Then
                                    myAlarms(Alarms.ReagentsPack_Depleted) = False
                                    ' solved
                                    alarmID = GlobalEnumerates.Alarms.ISE_RP_DEPLETED_ERR
                                    alarmStatus = False
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)
                                End If
                                If myAlarms(Alarms.ReagentsPack_Expired) Then
                                    myAlarms(Alarms.ReagentsPack_Expired) = False
                                    ' solved
                                    alarmID = GlobalEnumerates.Alarms.ISE_RP_EXPIRED_WARN
                                    alarmStatus = False
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)
                                End If
                                If myAlarms(Alarms.ReagentsPack_DateInstall) Then
                                    myAlarms(Alarms.ReagentsPack_DateInstall) = False
                                    ' solved
                                    alarmID = GlobalEnumerates.Alarms.ISE_RP_NO_INST_ERR
                                    alarmStatus = False
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)
                                End If
                            End If


                            ' Ise Electrodes is wrong installed
                            If Not MyClass.IsElectrodesReady Then

                                alarmID = GlobalEnumerates.Alarms.ISE_ELEC_WRONG_ERR
                                alarmStatus = True
                                pAlarmList.Add(alarmID)
                                pAlarmStatusList.Add(alarmStatus)

                                myAlarms(Alarms.Electrodes_Wrong) = True
                            Else
                                If myAlarms(Alarms.Electrodes_Wrong) Then
                                    myAlarms(Alarms.Electrodes_Wrong) = False
                                    ' solved
                                    alarmID = GlobalEnumerates.Alarms.ISE_ELEC_WRONG_ERR
                                    alarmStatus = False
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)
                                End If
                            End If

                            'End If

                            ' Ise Clean Pack installed
                            If MyClass.IsCleanPackInstalled Then

                                alarmID = GlobalEnumerates.Alarms.ISE_CP_INSTALL_WARN
                                alarmStatus = Not MyClass.IsLongTermDeactivation 'True
                                pAlarmList.Add(alarmID)
                                pAlarmStatusList.Add(alarmStatus)

                                myAlarms(Alarms.CleanPack_Installed) = True

                                If myAlarms(Alarms.CleanPack_Wrong) Then
                                    myAlarms(Alarms.CleanPack_Wrong) = False
                                    ' solved
                                    alarmID = GlobalEnumerates.Alarms.ISE_CP_WRONG_ERR
                                    alarmStatus = False
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)
                                End If
                            Else
                                If myAlarms(Alarms.CleanPack_Installed) Then
                                    myAlarms(Alarms.CleanPack_Installed) = False
                                    ' solved
                                    alarmID = GlobalEnumerates.Alarms.ISE_CP_INSTALL_WARN
                                    alarmStatus = False
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)
                                End If
                            End If

                            ' Ise Clean Pack wrong installed           
                            If MyClass.CurrentProcedure = ISEProcedures.CheckCleanPackInstalled Then
                                Dim isFinished As Boolean = ((MyClass.CurrentCommandTO.ISECommandID = ISECommands.PURGEB) And (MyClass.LastISEResult.ISEResultType = ISEResultTO.ISEResultTypes.OK))
                                If isFinished And Not MyClass.IsCleanPackInstalled Then

                                    alarmID = GlobalEnumerates.Alarms.ISE_CP_WRONG_ERR
                                    alarmStatus = True
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)

                                    myAlarms(Alarms.CleanPack_Wrong) = True

                                    'Else
                                    '    If myAlarms(Alarms.CleanPack_Wrong) Then
                                    '        myAlarms(Alarms.CleanPack_Wrong) = False
                                    '        ' solved
                                    '        alarmID = GlobalEnumerates.Alarms.ISE_CP_WRONG
                                    '        alarmStatus = False
                                    '        pAlarmList.Add(alarmID)
                                    '        pAlarmStatusList.Add(alarmStatus)
                                    '    End If
                                End If

                            End If

                            ' XB 04/11/2014 - BA-1872
                            ' Ise Timeout   
                            If MyClass.IsTimeOut Then

                                alarmID = GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR
                                alarmStatus = True
                                pAlarmList.Add(alarmID)
                                pAlarmStatusList.Add(alarmStatus)

                                myAlarms(Alarms.Timeout) = True
                            Else
                                If myAlarms(Alarms.Timeout) Then
                                    myAlarms(Alarms.Timeout) = False
                                    ' solved
                                    alarmID = GlobalEnumerates.Alarms.ISE_TIMEOUT_ERR
                                    alarmStatus = False
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)
                                End If
                            End If

                        End If

                        ' Ise Electrodes Expired by consumption
                        If MyClass.MonitorDataTO IsNot Nothing Then
                            If MyClass.MonitorDataTO.REF_Data.IsOverUsed Or _
                               MyClass.MonitorDataTO.NA_Data.IsOverUsed Or _
                               MyClass.MonitorDataTO.K_Data.IsOverUsed Or _
                               MyClass.MonitorDataTO.CL_Data.IsOverUsed Or _
                               (MyClass.MonitorDataTO.LI_Enabled And MyClass.MonitorDataTO.LI_Data.IsOverUsed) Then

                                alarmID = GlobalEnumerates.Alarms.ISE_ELEC_CONS_WARN
                                alarmStatus = True
                                pAlarmList.Add(alarmID)
                                pAlarmStatusList.Add(alarmStatus)

                                myAlarms(Alarms.Electrodes_Cons_Expired) = True
                            Else
                                If myAlarms(Alarms.Electrodes_Cons_Expired) Then
                                    myAlarms(Alarms.Electrodes_Cons_Expired) = False
                                    ' solved
                                    alarmID = GlobalEnumerates.Alarms.ISE_ELEC_CONS_WARN
                                    alarmStatus = False
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)
                                End If
                            End If

                            ' Ise Electrodes Expired by date
                            If MyClass.MonitorDataTO.REF_Data.IsExpired Or _
                               MyClass.MonitorDataTO.NA_Data.IsExpired Or _
                               MyClass.MonitorDataTO.K_Data.IsExpired Or _
                               MyClass.MonitorDataTO.CL_Data.IsExpired Or _
                               (MyClass.MonitorDataTO.LI_Enabled And MyClass.MonitorDataTO.LI_Data.IsExpired) Then

                                alarmID = GlobalEnumerates.Alarms.ISE_ELEC_DATE_WARN
                                alarmStatus = True
                                pAlarmList.Add(alarmID)
                                pAlarmStatusList.Add(alarmStatus)

                                myAlarms(Alarms.Electrodes_Date_Expired) = True
                            Else
                                If myAlarms(Alarms.Electrodes_Date_Expired) Then
                                    myAlarms(Alarms.Electrodes_Date_Expired) = False
                                    ' solved
                                    alarmID = GlobalEnumerates.Alarms.ISE_ELEC_DATE_WARN
                                    alarmStatus = False
                                    pAlarmList.Add(alarmID)
                                    pAlarmStatusList.Add(alarmStatus)
                                End If
                            End If
                        End If
                        '' Ise module is Activated  
                        'If MyClass.IsISEModuleInstalled Then           SERVICE !!!!

                        '    alarmID = GlobalEnumerates.Alarms.ISE_ACTIVATED
                        '    alarmStatus = True

                        '     pAlarmList.Add(alarmID)
                        '     pAlarmStatusList.Add(alarmStatus)
                        'End If

                    End If

                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.CheckAlarms", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Save calibrator Consumptions
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by XBC 04/04/2012
        ''' Modified by AG + XBC 28/09/2012 - Returns GlobalDataTo (SetDatos as boolean, TRUE any instruction sent, FALSE no instructin sent)
        '''                   XB 07/01/2015 - Do not save consumptions when Start Session or Shutdown operations are processing - BA-1178
        ''' </remarks>
        Public Function SaveConsumptions() As GlobalDataTO Implements IISEManager.SaveConsumptions
            Dim resultData As New GlobalDataTO
            Try
                Dim instructionSentFlag As Boolean = False 'AG + XBC 28/09/2012

                ' XBC 30/07/2012 - Correction when ISE has no correctly initiated
                ' And Reagents Pack Ready (Has Distributor Code Ok and Installed)
                If MyClass.IsISEInitiatedOKAttr And MyClass.IsReagentsPackReady Then

                    '  XB 07/01/2015 - BA-1178
                    If Not myAnalyzer.ShutDownisPending And Not myAnalyzer.StartSessionisPending Then

                        If MyClass.myAnalyzer.AnalyzerStatus = AnalyzerManagerStatus.STANDBY Then

                            If MyClass.IsCalAUpdateRequired Then 'update cal A consumption

                                MyClass.CurrentProcedure = ISEProcedures.WriteConsumption

                                ' Save it into Dallas Xip
                                resultData = SaveConsumptionCalToDallasData("A")
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    MyClass.CurrentCommandTO = CType(resultData.SetDatos, ISECommandTO)
                                    Debug.Print("Guardant Consum Cal A ...")

                                    resultData = MyClass.SendISECommand()
                                    If Not resultData.HasError Then
                                        instructionSentFlag = True  'AG + XBC 28/09/2012
                                        MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.WRITE_CALA_CONSUMPTION
                                        Debug.Print("Canvi d'estat a WRITE_CALA_CONSUMPTION ...")
                                    Else
                                        Debug.Print("Cal A ... No entra !")
                                    End If

                                End If

                            ElseIf MyClass.IsCalBUpdateRequired Then 'update calB consumption

                                MyClass.CurrentProcedure = ISEProcedures.WriteConsumption

                                ' Save it into Dallas Xip
                                resultData = SaveConsumptionCalToDallasData("B")
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    MyClass.CurrentCommandTO = CType(resultData.SetDatos, ISECommandTO)
                                    Debug.Print("Guardant Consum Cal B ...")

                                    resultData = MyClass.SendISECommand()
                                    If Not resultData.HasError Then
                                        instructionSentFlag = True  'AG + XBC 28/09/2012
                                        MyClass.CurrentCommandTOAttr.ISECommandID = ISECommands.WRITE_CALB_CONSUMPTION
                                        Debug.Print("Canvi d'estat a WRITE_CALB_CONSUMPTION ...")
                                    Else
                                        Debug.Print("Cal B ... No entra !")
                                    End If
                                End If

                            End If
                        End If

                    End If
                End If
                resultData.SetDatos = instructionSentFlag 'AG + XBC 28/09/2012

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.SaveConsumptions", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Perform Electrodes Calibration operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 26/06/2012</remarks>
        Public Function DoElectrodesCalibration() As GlobalDataTO Implements IISEManager.DoElectrodesCalibration
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.CalibrateElectrodes
                MyClass.PrepareDataToSend_CALB()
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DoElectrodesCalibration", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Perform Pumps Calibration operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 26/06/2012</remarks>
        Public Function DoPumpsCalibration() As GlobalDataTO Implements IISEManager.DoPumpsCalibration
            Dim resultData As New GlobalDataTO
            Try

                MyClass.CurrentProcedure = ISEProcedures.CalibratePumps
                MyClass.PrepareDataToSend_PUMP_CAL(1, AnalyzerIDAttr)
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DoPumpsCalibration", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Perform Bubbles Calibration operation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 25/07/2012</remarks>
        Public Function DoBubblesCalibration() As GlobalDataTO Implements IISEManager.DoBubblesCalibration
            Dim resultData As New GlobalDataTO
            Try
                MyClass.CurrentProcedure = ISEProcedures.CalibrateBubbles
                MyClass.PrepareDataToSend_BUBBLE_CAL()
                MyClass.SendISECommand()

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.DoBubblesCalibration", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        Public Function GetISEErrorDescription(ByVal pISEError As ISEErrorTO, Optional ByVal pIsUrine As Boolean = False, Optional ByVal pIsCalibration As Boolean = False) As String Implements IISEManager.GetISEErrorDescription
            Dim myDesc As String = ""
            Try
                Dim myDescID As String = ""

                If pISEError.IsCancelError Then
                    Dim myAlarmID As GlobalEnumerates.Alarms
                    Select Case pISEError.CancelErrorCode
                        Case ISEErrorTO.ISECancelErrorCodes.A : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_A
                        Case ISEErrorTO.ISECancelErrorCodes.B : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_B
                        Case ISEErrorTO.ISECancelErrorCodes.C : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_C
                        Case ISEErrorTO.ISECancelErrorCodes.D : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_D
                        Case ISEErrorTO.ISECancelErrorCodes.F : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_F
                        Case ISEErrorTO.ISECancelErrorCodes.M : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_M
                        Case ISEErrorTO.ISECancelErrorCodes.N : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_N
                        Case ISEErrorTO.ISECancelErrorCodes.P : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_P
                        Case ISEErrorTO.ISECancelErrorCodes.R : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_R
                        Case ISEErrorTO.ISECancelErrorCodes.S : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_S
                        Case ISEErrorTO.ISECancelErrorCodes.T : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_T
                        Case ISEErrorTO.ISECancelErrorCodes.W : myAlarmID = GlobalEnumerates.Alarms.ISE_ERROR_W

                    End Select

                    myDescID = myAlarmID.ToString

                Else

                    Dim myRemarkID As GlobalEnumerates.Alarms
                    Select Case pISEError.ResultErrorCode
                        Case ISEErrorTO.ISEResultErrorCodes.mvOut_CalBSample
                            myRemarkID = GlobalEnumerates.Alarms.ISE_mVOutB

                        Case ISEErrorTO.ISEResultErrorCodes.mvOut_CalASample_CalBUrine
                            If Not pIsUrine Then
                                myRemarkID = GlobalEnumerates.Alarms.ISE_mVOutA_SER
                            Else
                                myRemarkID = GlobalEnumerates.Alarms.ISE_mVOutB_URI
                            End If

                        Case ISEErrorTO.ISEResultErrorCodes.mvNoise_CalBSample
                            myRemarkID = GlobalEnumerates.Alarms.ISE_mVNoiseB

                        Case ISEErrorTO.ISEResultErrorCodes.mvNoise_CalBSample_CalBUrine
                            If Not pIsUrine Then
                                myRemarkID = GlobalEnumerates.Alarms.ISE_mVNoiseA_SER
                            Else
                                myRemarkID = GlobalEnumerates.Alarms.ISE_mVNoiseB_URI
                            End If

                        Case ISEErrorTO.ISEResultErrorCodes.Drift_CalASample
                            If Not pIsCalibration Then
                                myRemarkID = GlobalEnumerates.Alarms.ISE_Drift_SER
                            Else
                                myRemarkID = GlobalEnumerates.Alarms.ISE_Drift_CAL
                            End If

                        Case ISEErrorTO.ISEResultErrorCodes.OutOfSlope_MachineRanges
                            myRemarkID = GlobalEnumerates.Alarms.ISE_OutSlope

                    End Select

                    myDescID = myRemarkID.ToString

                End If

                If myDescID.Length > 0 Then
                    Dim myGlobal As GlobalDataTO
                    Dim myAlarmsDelegate As New AlarmsDelegate
                    myGlobal = myAlarmsDelegate.Read(Nothing, myDescID.ToString)
                    If Not myGlobal.HasError AndAlso myGlobal.SetDatos IsNot Nothing Then
                        Dim myAlarmsDS As AlarmsDS
                        myAlarmsDS = CType(myGlobal.SetDatos, AlarmsDS)
                        If myAlarmsDS.tfmwAlarms.Count > 0 Then
                            myDesc = myAlarmsDS.tfmwAlarms(0).Name
                        End If
                    End If
                End If

            Catch ex As Exception
                myDesc = ""
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetISEErrorDescription", EventLogEntryType.Error, False)
            End Try
            Return myDesc
        End Function

        ''' <summary>
        ''' Update Analyzer identifaction
        ''' </summary>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pAnalyzerModel"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XB 03/12/2013 - protection : Ensure that connected Analyzer is fine registered on ISE table DB - task #1410</remarks>
        Public Function UpdateAnalyzerInformation(ByVal pAnalyzerID As String, ByVal pAnalyzerModel As String) As GlobalDataTO Implements IISEManager.UpdateAnalyzerInformation
            Dim resultData As New GlobalDataTO
            Try
                AnalyzerIDAttr = pAnalyzerID
                AnalyzerModelAttr = pAnalyzerModel

                resultData = MyClass.RefreshAllDatabaseInformation

                'Dim myLogAccionesAux As New ApplicationLogManager()
                GlobalBase.CreateLogActivity("(Analyzer Change) Update Analyzer ID on ISE Manager [ " & pAnalyzerID & " ] ", "ISEManager.UpdateAnalyzerInformation", EventLogEntryType.Information, False)

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.UpdateAnalyzerInformation", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Initialize() Implements IISEManager.Initialize
            Try
                MyClass.IsAnalyzerDisconnectedAttr = False

                Dim myGlobal As New GlobalDataTO
                myGlobal = MyClass.RefreshAllDatabaseInformation

                If myGlobal.HasError Then
                    Throw New Exception(myGlobal.ErrorMessage)
                End If

                myISEInfoDelegate = New ISEDelegate
                myISECalibHistory = New ISECalibHistoryDelegate
                myISEInfoDS = Nothing
                myISESwParametersDS = Nothing
                myISELimitsDS = Nothing
                myAlarms = Nothing
                SIPcycles = Nothing
                SIPIntervalConsumption = Nothing
                Interval1forPurgeACompletedbyFW = Nothing
                Interval2forPurgeACompletedbyFW = Nothing
                ForceConsumptionsInitialize = False
                ISE_EXECUTION_TIME_SER = 0
                ISE_EXECUTION_TIME_URI = 0

                'Analyzer
                IsAnalyzerDisconnectedAttr = False
                IsAnalyzerWarmUpAttr = False
                AnalyzerIDAttr = ""
                AnalyzerModelAttr = ""
                WorkSessionIDAttr = ""

                'Procedures & results management
                CurrentProcedureAttr = ISEProcedures.None
                CurrentCommandTOAttr = Nothing
                LastISEResultAttr = Nothing
                LastProcedureResultAttr = ISEProcedureResult.None
                CurrentProcedureIsFinishedAttr = False

                'Monitor
                MonitorDataTOAttr = Nothing

                'Status
                IsISEModuleInstalledAttr = False
                IsISESwitchONAttr = False
                IsISEInitializationDoneAttr = False
                IsISEInitiatedOKAttr = False
                IsISEOnceInitiatedOKAttr = False
                IsISECommsOkAttr = False
                IsISEModuleReadyAttr = False
                IsLongTermDeactivationAttr = False
                isISEStatusUnknownAttr = True

                'Reagents Pack
                ISEDallasSNAttr = Nothing
                ISEDallasPage00Attr = Nothing
                ISEDallasPage01Attr = Nothing
                ReagentsPackInstallationDateAttr = Nothing
                IsReagentsPackInstalledAttr = False
                ReagentsPackExpirationDateAttr = Nothing
                BiosystemsCodeAttr = ""
                ReagentsPackInitialVolCalAAttr = 0
                ReagentsPackInitialVolCalBAttr = 0
                IsReagentsPackReadyAttr = False
                IsCleanPackInstalledAttr = False
                IsEnoughCalibratorAAttr = False
                IsEnoughCalibratorBAttr = False
                IsCalAUpdateRequiredAttr = False
                IsCalBUpdateRequiredAttr = False

                'Electrodes
                IsElectrodesReadyAttr = False
                IsLiEnabledByUserAttr = True

                IsLiMountedAttr = False
                IsNaMountedAttr = False
                IsKMountedAttr = False
                IsClMountedAttr = False

                IsRefExpiredAttr = False
                IsLiExpiredAttr = False
                IsNaExpiredAttr = False
                IsKExpiredAttr = False
                IsClExpiredAttr = False

                IsRefOverUsedAttr = False
                IsLiOverUsedAttr = False
                IsNaOverUsedAttr = False
                IsKOverUsedAttr = False
                IsClOverUsedAttr = False

                RefInstallDateAttr = Nothing
                LiInstallDateAttr = Nothing
                NaInstallDateAttr = Nothing
                KInstallDateAttr = Nothing
                ClInstallDateAttr = Nothing

                RefTestCountAttr = -1
                LiTestCountAttr = -1
                NaTestCountAttr = -1
                KTestCountAttr = -1
                ClTestCountAttr = -1

                PumpTubingInstallDateAttr = Nothing
                FluidicTubingInstallDateAttr = Nothing
                PumpTubingExpireDateAttr = Nothing
                FluidicTubingExpireDateAttr = Nothing

                'recommended Procedures
                IsCalibrationNeededAttr = False
                IsPumpCalibrationNeededAttr = False
                IsBubbleCalibrationNeededAttr = False
                IsCleanNeededAttr = False

                'last calibrations and clean
                LastElectrodesCalibrationResult1Attr = ""
                LastElectrodesCalibrationResult2Attr = ""
                LastElectrodesCalibrationDateAttr = Nothing
                LastElectrodesCalibrationErrorAttr = ""
                LastPumpsCalibrationResultAttr = ""
                LastPumpsCalibrationDateAttr = Nothing
                LastPumpsCalibrationErrorAttr = ""
                LastBubbleCalibrationResultAttr = ""
                LastBubbleCalibrationDateAttr = Nothing
                LastBubbleCalibrationErrorAttr = ""
                LastCleanDateAttr = Nothing
                LastCleanErrorAttr = ""
                TestsCountSinceLastCleanAttr = -1

                'Consumptions
                ConsumptionCalAbySerumAttr = 0
                ConsumptionCalBbySerumAttr = 0
                ConsumptionCalAbyUrine1Attr = 0
                ConsumptionCalBbyUrine1Attr = 0
                ConsumptionCalAbyUrine2Attr = 0
                ConsumptionCalBbyUrine2Attr = 0
                ConsumptionCalAbyElectrodesCalAttr = 0
                ConsumptionCalBbyElectrodesCalAttr = 0
                ConsumptionCalAbyPumpsCalAttr = 0
                ConsumptionCalBbyPumpsCalAttr = 0
                ConsumptionCalAbyBubblesCalAttr = 0
                ConsumptionCalBbyBubblesCalAttr = 0
                ConsumptionCalAbyCleanCycleAttr = 0
                ConsumptionCalBbyCleanCycleAttr = 0
                ConsumptionCalAbyPurgeAAttr = 0
                ConsumptionCalBbyPurgeAAttr = 0
                ConsumptionCalAbyPurgeBAttr = 0
                ConsumptionCalBbyPurgeBAttr = 0
                ConsumptionCalAbyPrimeAAttr = 0
                ConsumptionCalBbyPrimeAAttr = 0
                ConsumptionCalAbyPrimeBAttr = 0
                ConsumptionCalBbyPrimeBAttr = 0
                ConsumptionCalAbySippingAttr = 0
                ConsumptionCalBbySippingAttr = 0

                MinConsumptionVolToSaveDallasData_CalA = 0
                MinConsumptionVolToSaveDallasData_CalB = 0
                CountConsumptionToSaveDallasData_CalA = 0
                CountConsumptionToSaveDallasData_CalB = 0

                PurgeAbyFirmwareAttr = 0
                PurgeBbyFirmwareAttr = 0
                WorkSessionOverallTimeAttr = 0
                WorkSessionIsRunningAttr = False
                WorkSessionTestsByTypeAttr = ""

                IsReagentsPackSerialNumberMatchAttr = False
                IsInUtilitiesAttr = False
                ISEWSCancelErrorCounterAttr = 0
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

     ''' <summary>
        ''' Returns an formated string displaying the results with the corresponding ISE test names
        ''' </summary>
        ''' <param name="pAffected"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: XB 08/09/2014 - Use ISE Test Name field instead of fixed texts - BA-1902
        ''' </remarks>
        Public Shared Function FormatAffected(ByVal pAffected As String) As String
            Dim res As String = ""
            Try
                'Cl-, K+, Na+, Li+
                Dim ISETestList As New ISETestsDelegate

                Dim Li As String = ISETestList.GetName(Nothing, ISE_Tests.Li)
                Dim Na As String = ISETestList.GetName(Nothing, ISE_Tests.Na)
                Dim K As String = ISETestList.GetName(Nothing, ISE_Tests.K)
                Dim Cl As String = ISETestList.GetName(Nothing, ISE_Tests.Cl)

                If pAffected.Contains("Cl") Then res &= Cl & ", "
                If pAffected.Contains("K") Then res &= K & ", "
                If pAffected.Contains("Na") Then res &= Na & ", "
                If pAffected.Contains("Li") Then res &= Li & ", "

                If res.EndsWith(", ") Then res = res.Substring(0, res.Length - 2)

            Catch ex As Exception
                res = ""
            End Try
            Return res
        End Function

#End Region

#Region "ISE Sendings"

#Region "Instruction Start Timeout Management"
        'this functionality is implemented in order to manage timeouts 
        'when no Status Start notification is received after sending an ISE Command

        ' XB 30/09/2014 - Deactivate old timeout management - Remove too restrictive limitations because timeouts - BA-1872
        'Private InstructionStartedTimeout As Integer = 2000 '1000 miliseconds

        'Private InstructionStartedTimerCallBack As System.Threading.TimerCallback
        'Private WithEvents InstructionStartedTimer As System.Threading.Timer

        ' ''' <summary>
        ' ''' flag for knowing if it is waiting for the 1st instruction start after sending ISECMD
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks>Created by SGM 20/06/2012</remarks>
        'Public ReadOnly Property IsWaitingForInstructionStart() As Boolean
        '    Get
        '        Return (MyClass.InstructionStartedTimer IsNot Nothing)
        '    End Get
        'End Property

        ' ''' <summary>
        ' ''' Start Timer for controlling timeout until receiving Status Start instruction after sending ISECMD
        ' ''' </summary>
        ' ''' <param name="pTime"></param>
        ' ''' <returns></returns>
        ' ''' <remarks>
        ' ''' Created by SGM 20/06/2012
        ' ''' Modified by XBC 04/09/2012 - change to Public to allow calls from ManageAnalyzer
        ' ''' </remarks>
        'Public Function StartInstructionStartedTimer(Optional ByVal pTime As Integer = 0) As GlobalDataTO
        '    Dim myGlobal As New GlobalDataTO
        '    Try
        '        Dim myTime As Integer

        '        If pTime > 0 Then
        '            myTime = pTime
        '        Else
        '            myTime = MyClass.InstructionStartedTimeout
        '        End If

        '        MyClass.StopInstructionStartedTimer()

        '        MyClass.InstructionStartedTimerCallBack = New System.Threading.TimerCallback(AddressOf OnInstructionStartedTimerTick)

        '        MyClass.InstructionStartedTimer = New System.Threading.Timer(MyClass.InstructionStartedTimerCallBack, New Object, myTime, 0)

        '        Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - ISE Timer activated")

        '    Catch ex As Exception
        '        myGlobal = New GlobalDataTO()
        '        myGlobal.HasError = True
        '        myGlobal.ErrorCode = Messages.SYSTEM_ERROR.ToString()
        '        myGlobal.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ISEManager.StartInstructionStartedTimer", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobal
        'End Function

        ' ''' <summary>
        ' ''' Stop Timer for controlling timeout until receiving Status Start instruction after sending ISECMD
        ' ''' </summary>
        ' ''' <remarks>Created by SGM 20/06/2012</remarks>
        'Public Sub StopInstructionStartedTimer()
        '    Try
        '        If MyClass.InstructionStartedTimer IsNot Nothing Then
        '            MyClass.InstructionStartedTimer.Dispose()
        '            MyClass.InstructionStartedTimer = Nothing
        '            MyClass.InstructionStartedTimerCallBack = Nothing
        '            Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - ISE Timer killed")
        '        Else
        '            Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - ISE Timer es nothing")
        '        End If

        '    Catch ex As Exception
        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ISEManager.StopInstructionStartedTimer", EventLogEntryType.Error, False)
        '    End Try
        'End Sub

        ' ''' <summary>
        ' ''' Timer for controlling timeout until receiving Status Start instruction after sending ISECMD
        ' ''' </summary>
        ' ''' <remarks>Created by SGM 20/06/2012</remarks>
        'Private Sub OnInstructionStartedTimerTick(ByVal stateInfo As Object)
        '    Try

        '        If MyClass.InstructionStartedTimer IsNot Nothing AndAlso MyClass.InstructionStartedTimerCallBack IsNot Nothing Then
        '            'MyClass.StopInstructionStartedTimer()
        '            Debug.Print(DateTime.Now.ToString("HH:mm:ss:fff") + " - ISE Timeout!!!")

        '            Dim myISEResultWithComErrors As ISEResultTO = New ISEResultTO
        '            myISEResultWithComErrors.ISEResultType = ISEResultTO.ISEResultTypes.ComError
        '            MyClass.LastISEResult = myISEResultWithComErrors

        '            'Dim myLogAcciones As New ApplicationLogManager()
        '            GlobalBase.CreateLogActivity("ISE timeout after waiting for Status instruction from Analyzer", "ISEManager.OnInstructionStartedTimerTick", EventLogEntryType.Error, False)

        '            MyClass.StopInstructionStartedTimer()
        '        End If

        '    Catch ex As Exception
        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ISEManager.OnInstructionStartedTimerTick", EventLogEntryType.Error, False)
        '    End Try
        'End Sub
        ' XB 30/09/2014 - BA-1872

#End Region

#Region "Public Methods"




        ''' <summary>
        ''' Depending the pCommand prepare data to send a instruction ISECMD
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pISEMode "></param>
        ''' <param name="pCmd" ></param>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by AG 11/01/2012
        ''' Updated by XBC 12/01/2012 - Add ISEModes Low_Level_Control functionalities
        ''' </remarks>
        Public Function PrepareDataToSend(ByVal pDBConnection As SqlConnection, ByVal pApplicationName As String, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                          ByVal pISEMode As ISEModes, ByVal pCmd As ISECommands, _
                                          Optional ByVal pParameter1 As String = "0", _
                                          Optional ByVal pParameter2 As String = "0", _
                                          Optional ByVal pParameter3 As String = "0", _
                                          Optional ByVal pTubePosition As Integer = 1) As GlobalDataTO Implements IISEManager.PrepareDataToSend

            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlConnection = Nothing

            Try

                'SGM 16/07/2012 Protection in case of null enter
                If pISEMode = ISEModes.None Or pCmd = ISECommands.NONE Then
                    resultData.HasError = True
                    resultData.ErrorCode = Messages.MASTER_DATA_MISSING.ToString
                    Exit Try
                End If

                'MyClass.CurrentCommandTO = Nothing

                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISECommand As New ISECommandTO

                        Select Case pISEMode
                            Case ISEModes.Cleaning_Cycle
                                With myISECommand
                                    .ISEMode = pISEMode
                                    .ISECommandID = pCmd
                                    .P1 = "0"
                                    .P2 = "0"
                                    .P3 = "0"
                                    .SampleRotorType = 1

                                    ' XBC 10/04/2012 - Clean Cycle is the same in both applications (User & Service)
                                    'If pApplicationName.ToUpper.Contains("SERVICE") Then
                                    ''Service Software

                                    .SampleTubePos = pTubePosition
                                    .SampleTubeType = "T15" ' by now is by default

                                    'Search for the volume
                                    If Not resultData.HasError Then
                                        .SampleVolume = -1
                                        resultData = GetRequiredTubeVolume(dbConnection, pAnalyzerID)
                                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                            .SampleVolume = CType(resultData.SetDatos, Integer)
                                        End If

                                        If .SampleVolume = -1 Then
                                            resultData.HasError = True
                                            resultData.ErrorCode = Messages.MASTER_DATA_MISSING.ToString
                                            Exit Try
                                        End If
                                    Else
                                        resultData.ErrorCode = Messages.MASTER_DATA_MISSING.ToString
                                        Exit Try
                                    End If

                                    'Else
                                    '    'User Software (SampleTubePos, SampleTubeType, SampleVolume)

                                    '    Dim myCellNumber As Integer = 1
                                    '    Dim myTubeType As String = "PED"
                                    '    Dim foundFlag As Boolean = False

                                    '    'Search for ISE wash tube required for CLEAN
                                    '    resultData = GetRequiredTubePositon(dbConnection, pAnalyzerID, pWorkSessionID, "TUBE_WASH_SOL", "WASHSOL3", foundFlag, myCellNumber, myTubeType)

                                    '    If Not resultData.HasError Then
                                    '        If foundFlag Then
                                    '            .SampleTubePos = myCellNumber
                                    '            .SampleTubeType = myTubeType
                                    '        Else
                                    '            resultData.HasError = True
                                    '            resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                                    '        End If
                                    '    End If

                                    '    'Search for the volume
                                    '    If Not resultData.HasError Then
                                    '        .SampleVolume = -1
                                    '        resultData = GetRequiredTubeVolume(dbConnection, pAnalyzerID)
                                    '        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    '            .SampleVolume = CType(resultData.SetDatos, Integer)
                                    '        End If

                                    '        If .SampleVolume = -1 Then
                                    '            resultData.HasError = True
                                    '            resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                                    '        End If
                                    '    End If
                                    'End If
                                    ' XBC 10/04/2012
                                End With


                            Case ISEModes.Pump_Calibration
                                With myISECommand
                                    .ISEMode = pISEMode
                                    .ISECommandID = pCmd
                                    .P1 = "0"
                                    .P2 = "0"
                                    .P3 = "0"
                                    .SampleRotorType = 1

                                    ' XBC 10/04/2012 - Clean Cycle is the same in both applications (User & Service)
                                    'If pApplicationName.ToUpper.Contains("SERVICE") Then
                                    'Service Software

                                    .SampleTubePos = pTubePosition
                                    .SampleTubeType = "T15" ' by now is by default

                                    'Search for the volume
                                    If Not resultData.HasError Then
                                        .SampleVolume = -1
                                        resultData = GetRequiredTubeVolume(dbConnection, pAnalyzerID)
                                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                            .SampleVolume = CType(resultData.SetDatos, Integer)
                                        End If

                                        If .SampleVolume = -1 Then
                                            resultData.HasError = True
                                            resultData.ErrorCode = Messages.MASTER_DATA_MISSING.ToString
                                            Exit Try
                                        End If
                                    Else
                                        resultData.ErrorCode = Messages.MASTER_DATA_MISSING.ToString
                                        Exit Try
                                    End If

                                    'Else
                                    ''User Software (SampleTubePos, SampleTubeType, SampleVolume)

                                    'Dim myCellNumber As Integer = 1
                                    'Dim myTubeType As String = "PED"
                                    'Dim foundFlag As Boolean = False

                                    ''Search for ISE wash tube required for PUMP CALIB
                                    'resultData = GetRequiredTubePositon(dbConnection, pAnalyzerID, pWorkSessionID, "TUBE_WASH_SOL", "WASHSOL3", foundFlag, myCellNumber, myTubeType)

                                    'If Not resultData.HasError Then
                                    '    If foundFlag Then
                                    '        .SampleTubePos = myCellNumber
                                    '        .SampleTubeType = myTubeType
                                    '    Else
                                    '        resultData.HasError = True
                                    '        resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                                    '    End If
                                    'End If

                                    ''Search for the volume
                                    'If Not resultData.HasError Then
                                    '    .SampleVolume = -1
                                    '    resultData = GetRequiredTubeVolume(dbConnection, pAnalyzerID)
                                    '    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    '        .SampleVolume = CType(resultData.SetDatos, Integer)
                                    '    End If

                                    '    If .SampleVolume = -1 Then
                                    '        resultData.HasError = True
                                    '        resultData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                                    '    End If
                                    'End If
                                    'End If
                                    ' XBC 10/04/2012
                                End With


                            Case ISEModes.Low_Level_Control
                                With myISECommand
                                    .ISEMode = pISEMode
                                    .ISECommandID = pCmd
                                    .P1 = pParameter1
                                    .P2 = pParameter2
                                    .P3 = pParameter3
                                    .SampleRotorType = 0
                                    .SampleTubePos = 0
                                    .SampleTubeType = ""
                                    .SampleVolume = 0
                                End With

                            Case ISEModes.Move_R2_To_WashStation, ISEModes.Move_R2_To_Parking
                                With myISECommand
                                    .ISEMode = pISEMode
                                    .ISECommandID = ISECommands.POLL
                                    .P1 = "0"
                                    .P2 = "0"
                                    .P3 = "0"
                                    .SampleRotorType = 0
                                    .SampleTubePos = 0
                                    .SampleTubeType = ""
                                    .SampleVolume = 0
                                End With

                            Case Else
                                resultData.HasError = True
                                resultData.ErrorCode = Messages.MASTER_DATA_MISSING.ToString
                                Exit Try
                                'With myISECommand
                                '    .ISEMode = pISEMode
                                '    .ISECommandID = ISECommands.NONE
                                '    .P1 = "0"
                                '    .P2 = "0"
                                '    .P3 = "0"
                                '    .SampleRotorType = 0
                                '    .SampleTubePos = 0
                                '    .SampleTubeType = ""
                                '    .SampleVolume = 0
                                'End With

                        End Select

                        MyClass.CurrentCommandTO = myISECommand
                        resultData.SetDatos = myISECommand
                    End If
                End If

            Catch ex As Exception
                MyClass.CurrentCommandTO = Nothing
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE POLL
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_POLL() As GlobalDataTO Implements IISEManager.PrepareDataToSend_POLL
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.POLL)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_POLL", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE CALB
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_CALB() As GlobalDataTO Implements IISEManager.PrepareDataToSend_CALB
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.CALB)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_CALB", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE CLEAN
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_CLEAN(ByVal pTubePosition As Integer, ByVal pAnalyzerID As String) As GlobalDataTO Implements IISEManager.PrepareDataToSend_CLEAN
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, My.Application.Info.AssemblyName, pAnalyzerID, "", ISEModes.Cleaning_Cycle, ISECommands.CLEAN, "0", "0", "0", pTubePosition)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_CLEAN", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' prepare data to send an instruction ISE PUMP CAL
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_PUMP_CAL(ByVal pTubePosition As Integer, ByVal pAnalyzerID As String) As GlobalDataTO Implements IISEManager.PrepareDataToSend_PUMP_CAL
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, My.Application.Info.AssemblyName, pAnalyzerID, "", ISEModes.Pump_Calibration, ISECommands.PUMP_CAL, "0", "0", "0", pTubePosition)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_PUMP_CAL", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE START
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_START() As GlobalDataTO Implements IISEManager.PrepareDataToSend_START
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.START)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_START", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE PURGEA
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_PURGEA() As GlobalDataTO Implements IISEManager.PrepareDataToSend_PURGEA
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.PURGEA)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_PURGEA", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE PURGEB
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_PURGEB() As GlobalDataTO Implements IISEManager.PrepareDataToSend_PURGEB
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.PURGEB)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_PURGEB", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE BUBBLE_CAL
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_BUBBLE_CAL() As GlobalDataTO Implements IISEManager.PrepareDataToSend_BUBBLE_CAL
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.BUBBLE_CAL)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_BUBBLE_CAL", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE SHOW BUBBLE CAL
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_SHOW_BUBBLE_CAL() As GlobalDataTO Implements IISEManager.PrepareDataToSend_SHOW_BUBBLE_CAL
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.SHOW_BUBBLE_CAL)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_SHOW_BUBBLE_CAL", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE SHOW PUMP CAL
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_SHOW_PUMP_CAL() As GlobalDataTO Implements IISEManager.PrepareDataToSend_SHOW_PUMP_CAL
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.SHOW_PUMP_CAL)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_SHOW_PUMP_CAL", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE DSPA
        ''' </summary>
        ''' <param name="pVolume">volume (XXX µL) of Calibrant A into the sample entry port</param>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_DSPA(ByVal pVolume As String) As GlobalDataTO Implements IISEManager.PrepareDataToSend_DSPA
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.DSPA, pVolume)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_DSPA", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE DSPB
        ''' </summary>
        ''' <param name="pVolume">volume (XXX µL) of Calibrant B into the sample entry port</param>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_DSPB(ByVal pVolume As String) As GlobalDataTO Implements IISEManager.PrepareDataToSend_DSPB
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.DSPB, pVolume)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_DSPB", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE VERSION CHECKSUM
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_VERSION_CHECKSUM() As GlobalDataTO Implements IISEManager.PrepareDataToSend_VERSION_CHECKSUM
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.VERSION_CHECKSUM)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_VERSION_CHECKSUM", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE READ mV
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_READ_mV() As GlobalDataTO Implements IISEManager.PrepareDataToSend_READ_mV
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.READ_mV)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_READ_mV", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE LAST SLOPES
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_LAST_SLOPES() As GlobalDataTO Implements IISEManager.PrepareDataToSend_LAST_SLOPES
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.LAST_SLOPES)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_LAST_SLOPES", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE DEBUG mV
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_DEBUG_mV(ByVal pDebugMode As ISECommands) As GlobalDataTO Implements IISEManager.PrepareDataToSend_DEBUG_mV
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, pDebugMode)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_DEBUG_mV", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE MAINTENANCE
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_MAINTENANCE() As GlobalDataTO Implements IISEManager.PrepareDataToSend_MAINTENANCE
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.MAINTENANCE)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_MAINTENANCE", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE PRIME CALA
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_PRIME_CALA() As GlobalDataTO Implements IISEManager.PrepareDataToSend_PRIME_CALA
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.PRIME_CALA)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_PRIME_CALA", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE PRIME CALB
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_PRIME_CALB() As GlobalDataTO Implements IISEManager.PrepareDataToSend_PRIME_CALB
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.PRIME_CALB)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_PRIME_CALB", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE READ PAGE DALLAS
        ''' </summary>
        ''' <param name="pPage">page to read of Dallas chip</param>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_READ_PAGE_DALLAS(ByVal pPage As Integer) As GlobalDataTO Implements IISEManager.PrepareDataToSend_READ_PAGE_DALLAS
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                Select Case pPage
                    Case 0
                        resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.READ_PAGE_0_DALLAS)
                    Case 1
                        resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Low_Level_Control, ISECommands.READ_PAGE_1_DALLAS)
                End Select
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_READ_PAGE_DALLAS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE WRITE DALLAS
        ''' Data contents allowed to save only in Page 1
        ''' </summary>
        ''' <param name="pPosition">position to write into Dallas chip</param>
        ''' <param name="pInfo">information to write into Dallas chip</param>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_WRITE_DALLAS(ByVal pPosition As String, _
                                                       ByVal pInfo As String) As GlobalDataTO Implements IISEManager.PrepareDataToSend_WRITE_DALLAS
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", _
                                                       ISEModes.Low_Level_Control, _
                                                       ISECommands.WRITE_DALLAS, _
                                                       "01", _
                                                       pPosition, _
                                                       pInfo)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_WRITE_DALLAS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ' XBC 15/03/2012 - ANULADO POR AHORA
        '''' <summary>
        '''' prepare data to send an instruction ISE WRITE REAGENTPACK WRONG
        '''' </summary>
        '''' <param name="pReason">reason to write as a reason to discard the use of Reagent pack</param>
        '''' <returns>GlobalDataTo (ISECommandTo)</returns>
        '''' <remarks>
        '''' Created by XBC 12/01/2012
        '''' </remarks>
        'Public Function PrepareDataToSend_WRITE_REAGENTPACK_WRONG(ByVal pReason As WrongReasons) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim myDataIseCmdTo As New ISECommandTO
        '    Dim myUtility As New Utilities()
        '    Try
        '        Dim myReason As String
        '        Dim myPosition As String

        '        resultData = Utilities.ConvertDecimalToHex(CLng(DallasXipPage1.NoGoodByte))
        '        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '            myPosition = CType(resultData.SetDatos, String)
        '        Else
        '            Exit Try
        '        End If
        '        If IsNumeric(pReason) Then
        '            resultData = Utilities.ConvertDecimalToHex(CLng(pReason))
        '            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '                myReason = CType(resultData.SetDatos, String)
        '            Else
        '                Exit Try
        '            End If
        '        Else
        '            Exit Try
        '        End If
        '        resultData = MyClass.PrepareDataToSend_WRITE_DALLAS(myPosition, myReason)
        '        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
        '            myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
        '        Else
        '            Exit Try
        '        End If
        '        resultData.SetDatos = myDataIseCmdTo
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_WRITE_REAGENTPACK_WRONG", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE WRITE INSTALL DAY
        ''' </summary>
        ''' <param name="pDay">Installation Day to write</param>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_WRITE_INSTALL_DAY(ByVal pDay As Integer) As GlobalDataTO Implements IISEManager.PrepareDataToSend_WRITE_INSTALL_DAY
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            'Dim myUtility As New Utilities()
            Try
                Dim myDay As String
                Dim myPosition As Integer

                ' XBC 15/03/2012
                ' Sw envia la informacion al Fw en decimal (porque así lo espera Fw) y es Fw quien lo convierte a Hexadecimal
                'resultData = Utilities.ConvertDecimalToHex(CLng(DallasXipPage1.InstallDay))
                'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                '    myPosition = CType(resultData.SetDatos, String)
                'Else
                '    Exit Try
                'End If
                'If IsNumeric(pDay) Then
                '    resultData = Utilities.ConvertDecimalToHex(CLng(pDay))
                '    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                '        myDay = CType(resultData.SetDatos, String)
                '    Else
                '        Exit Try
                '    End If
                'Else
                '    Exit Try
                'End If

                myPosition = DallasXipPage1.InstallDay
                myDay = pDay.ToString
                ' XBC 15/03/2012

                resultData = MyClass.PrepareDataToSend_WRITE_DALLAS(myPosition.ToString, myDay)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                Else
                    Exit Try
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_WRITE_INSTALL_DAY", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE WRITE INSTALL MONTH
        ''' </summary>
        ''' <param name="pMonth">Installation Month to write</param>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_WRITE_INSTALL_MONTH(ByVal pMonth As Integer) As GlobalDataTO Implements IISEManager.PrepareDataToSend_WRITE_INSTALL_MONTH
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            'Dim myUtility As New Utilities()
            Try
                Dim myMonth As String
                Dim myPosition As Integer

                ' XBC 15/03/2012
                ' Sw envia la informacion al Fw en decimal (porque así lo espera Fw) y es Fw quien lo convierte a Hexadecimal
                'resultData = Utilities.ConvertDecimalToHex(CLng(DallasXipPage1.InstallMonth))
                'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                '    myPosition = CType(resultData.SetDatos, String)
                'Else
                '    Exit Try
                'End If
                'If IsNumeric(pMonth) Then
                '    resultData = Utilities.ConvertDecimalToHex(CLng(pMonth))
                '    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                '        myMonth = CType(resultData.SetDatos, String)
                '    Else
                '        Exit Try
                '    End If
                'Else
                '    Exit Try
                'End If

                myPosition = DallasXipPage1.InstallMonth
                myMonth = pMonth.ToString
                ' XBC 15/03/2012

                resultData = MyClass.PrepareDataToSend_WRITE_DALLAS(myPosition.ToString, myMonth)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                Else
                    Exit Try
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_WRITE_INSTALL_MONTH", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare data to send an instruction ISE WRITE INSTALL YEAR
        ''' </summary>
        ''' <param name="pYear">Installation Year to write</param>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by XBC 12/01/2012
        ''' </remarks>
        Public Function PrepareDataToSend_WRITE_INSTALL_YEAR(ByVal pYear As Integer) As GlobalDataTO Implements IISEManager.PrepareDataToSend_WRITE_INSTALL_YEAR
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            'Dim myUtility As New Utilities()
            Try
                Dim myYear As String
                Dim myPosition As Integer

                ' XBC 15/03/2012
                ' Sw envia la informacion al Fw en decimal (porque así lo espera Fw) y es Fw quien lo convierte a Hexadecimal
                'resultData = Utilities.ConvertDecimalToHex(CLng(DallasXipPage1.InstallYear))
                'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                '    myPosition = CType(resultData.SetDatos, String)
                'Else
                '    Exit Try
                'End If
                'If IsNumeric(pYear) Then
                '    resultData = Utilities.ConvertDecimalToHex(CLng(pYear) - 2000)
                '    If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                '        myYear = CType(resultData.SetDatos, String)
                '    Else
                '        Exit Try
                '    End If
                'Else
                '    Exit Try
                'End If

                myPosition = DallasXipPage1.InstallYear
                myYear = (pYear - 2000).ToString
                ' XBC 15/03/2012

                resultData = MyClass.PrepareDataToSend_WRITE_DALLAS(myPosition.ToString, myYear)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                Else
                    Exit Try
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_WRITE_INSTALL_YEAR", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare the instruction to move the R2 arm away from parking position
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by SGM 03/07/2012
        ''' </remarks>
        Public Function PrepareDataToSend_R2_TO_WASH() As GlobalDataTO Implements IISEManager.PrepareDataToSend_R2_TO_WASH
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Move_R2_To_WashStation, ISECommands.POLL)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_R2_To_WASH", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' prepare the instruction to move the R2 arm back to parking position
        ''' </summary>
        ''' <returns>GlobalDataTo (ISECommandTo)</returns>
        ''' <remarks>
        ''' Created by SGM 03/07/2012
        ''' </remarks>
        Public Function PrepareDataToSend_R2_TO_PARK() As GlobalDataTO Implements IISEManager.PrepareDataToSend_R2_TO_PARK
            Dim resultData As GlobalDataTO = Nothing
            Dim myDataIseCmdTo As New ISECommandTO
            Try
                resultData = MyClass.PrepareDataToSend(Nothing, "", "", "", ISEModes.Move_R2_To_Parking, ISECommands.POLL)
                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    myDataIseCmdTo = CType(resultData.SetDatos, ISECommandTO)
                End If
                resultData.SetDatos = myDataIseCmdTo
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.PrepareDataToSend_R2_TO_PARK", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Some ISE UTILITIES: Clean and Pump Calib requires the WASH ISE SOLUTION TUBE into samples rotor
        ''' This method search it in current samples rotor. If not exists use the default positions
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pTubeContent"></param>
        ''' <param name="pSolutionCode"></param> 
        ''' <param name="pFoundFlag">ByRef</param>
        ''' <param name="pCellNumber">ByRef</param>
        ''' <param name="pTubeType">ByRef</param>
        ''' <returns>Globaldata to with error or not</returns>
        ''' <remarks>AG 11/01/2012</remarks>
        Private Function GetRequiredTubePositon(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                     ByVal pTubeContent As String, ByVal pSolutionCode As String, _
                                     ByRef pFoundFlag As Boolean, ByRef pCellNumber As Integer, ByRef pTubeType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim rcpDel As New WSRotorContentByPositionDelegate
                        Dim myRcpDS As New WSRotorContentByPositionDS

                        'Read current rotor content by position
                        resultData = rcpDel.GetRotorContentPositions(dbConnection, pWorkSessionID, pAnalyzerID)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            myRcpDS = CType(resultData.SetDatos, WSRotorContentByPositionDS)
                        End If

                        'Search ISE Wash tube in current sample rotor positions
                        Dim myCandidateCells As List(Of WSRotorContentByPositionDS.twksWSRotorContentByPositionRow)
                        myCandidateCells = (From a In myRcpDS.twksWSRotorContentByPosition.AsEnumerable _
                                          Where a.RotorType = "SAMPLES" AndAlso a.TubeContent = pTubeContent _
                                          Select a).ToList()

                        If myCandidateCells.Count > 0 Then
                            Dim currentCellDS As New WSRotorContentByPositionDS
                            Dim currentCellDSRow As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow

                            For Each item As WSRotorContentByPositionDS.twksWSRotorContentByPositionRow In myCandidateCells
                                'Fill the Row and insert it in the DataSet to read details
                                currentCellDS.Clear()
                                currentCellDSRow = currentCellDS.twksWSRotorContentByPosition.NewtwksWSRotorContentByPositionRow()
                                currentCellDSRow.AnalyzerID = pAnalyzerID
                                currentCellDSRow.RotorType = "SAMPLES"
                                currentCellDSRow.RingNumber = item.RingNumber
                                currentCellDSRow.CellNumber = item.CellNumber
                                currentCellDSRow.WorkSessionID = pWorkSessionID
                                currentCellDS.twksWSRotorContentByPosition.Rows.Add(currentCellDSRow)
                                currentCellDS.twksWSRotorContentByPosition.AcceptChanges()

                                resultData = rcpDel.GetPositionInfo(dbConnection, currentCellDS)
                                If (Not resultData.HasError) Then
                                    'Get the returned information
                                    Dim myCellPosInfoDS As CellPositionInformationDS
                                    myCellPosInfoDS = CType(resultData.SetDatos, CellPositionInformationDS)

                                    'Search solutionCode = 'WASHSOL3'
                                    For Each info As CellPositionInformationDS.PositionInformationRow In myCellPosInfoDS.PositionInformation.Rows
                                        If (Not info.IsSolutionCodeNull AndAlso info.SolutionCode = pSolutionCode) Then
                                            If (Not item.IsStatusNull AndAlso item.Status <> "DEPLETED") OrElse item.IsStatusNull Then
                                                pFoundFlag = True
                                                pCellNumber = item.CellNumber
                                                pTubeType = item.TubeType
                                            End If
                                        End If

                                        If pFoundFlag Then Exit For
                                    Next
                                End If

                                If pFoundFlag Then Exit For
                            Next
                        End If

                        'If not found then use the default fixed positions
                        If Not pFoundFlag Then
                            Dim paramsDlg As New SwParametersDelegate
                            Dim paramsDS As New ParametersDS

                            resultData = paramsDlg.GetParameterByAnalyzer(dbConnection, pAnalyzerID, SwParameters.ISE_UTIL_WASHSOL_POSITION.ToString, True)
                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                paramsDS = CType(resultData.SetDatos, ParametersDS)
                                If paramsDS.tfmwSwParameters.Rows.Count > 0 Then
                                    pCellNumber = CInt(paramsDS.tfmwSwParameters(0).ValueNumeric)
                                    pTubeType = "T15"
                                    pFoundFlag = True
                                End If
                            End If
                        End If

                        myCandidateCells = Nothing 'AG 02/08/2012 release memory
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetRequiredTubePositon", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Gets the required volume for ISE CLEAN and ISE PUMP CALIB and convert into steps
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns>GlobalDataTo (integer)</returns>
        ''' <remarks>AG 11/01/2012</remarks>
        Private Function GetRequiredTubeVolume(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim paramsDlg As New SwParametersDelegate
                        Dim paramsDS As New ParametersDS

                        resultData = paramsDlg.GetParameterByAnalyzer(dbConnection, pAnalyzerID, SwParameters.ISE_UTIL_VOLUME.ToString, True)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            paramsDS = CType(resultData.SetDatos, ParametersDS)
                            If paramsDS.tfmwSwParameters.Rows.Count > 0 Then
                                Dim volume As Single = paramsDS.tfmwSwParameters(0).ValueNumeric

                                resultData = paramsDlg.GetParameterByAnalyzer(dbConnection, pAnalyzerID, SwParameters.SAMPLE_STEPS_UL.ToString, True)  'AJG Changed from False
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    paramsDS = CType(resultData.SetDatos, ParametersDS)
                                    If paramsDS.tfmwSwParameters.Rows.Count > 0 Then
                                        Dim sample_steps_uL As Single = paramsDS.tfmwSwParameters(0).ValueNumeric

                                        resultData.SetDatos = CInt(volume * sample_steps_uL)
                                    End If
                                End If

                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ISEManager.GetRequiredTubeVolume", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function
#End Region


#End Region

    End Class

End Namespace

