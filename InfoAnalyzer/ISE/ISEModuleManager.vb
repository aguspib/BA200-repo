'Option Explicit On
'Option Strict On

'Imports Biosystems.Ax00.Types
'Imports Biosystems.Ax00.Global


'Namespace Biosystems.Ax00.Global

'    ''' <summary>
'    ''' This class is the type of the object that contains all the data related 
'    ''' to the current status of the ISE Module
'    ''' </summary>
'    ''' <remarks>Created by SGM 08/02/2012</remarks>
'    Public Class ISEModuleManager

'#Region "Constructor"
'        Public Sub New()

'        End Sub

'#End Region


'#Region "Attributes"

'        Private ISEInformationAttr As ISEInformationDS

'        Private LastISEResultAttr As ISEResultTO

'        'Common
'        Private IsISESwitchedONAttr As Boolean = False
'        Private IsISECommsOkAttr As Boolean = False
'        Private IsISEModuleReadyAttr As Boolean = False
'        Private IsLongTermDeactivationAttr As Boolean = False

'        'Reagents Pack
'        Private AreReagentsInstalledAttr As Boolean = False
'        Private AreReagentsReadyAttr As Boolean = False
'        Private HasBiosystemsCodeAttr As Boolean = False
'        Private IsCleanPackInstalledAttr As Boolean = False
'        Private HaveReagentsInstallationDateAttr As Boolean = False
'        Private AreReagentsExpiredAttr As Boolean = False
'        Private IsEnoughCalibratorAAttr As Boolean = False
'        Private IsEnoughCalibratorBAttr As Boolean = False

'        'Electrodes
'        Private IsElectrodesReadyAttr As Boolean = False
'        Private IsLiEnabledByUserAttr As Boolean = True
'        Private HasRefInstallDateAttr As Boolean = False
'        Private HasLiInstallDateAttr As Boolean = False
'        Private HasNaInstallDateAttr As Boolean = False
'        Private HasKInstallDateAttr As Boolean = False
'        Private HasClInstallDateAttr As Boolean = False
'        Private IsRefInstalledAttr As Boolean = False
'        Private IsLiInstalledAttr As Boolean = False
'        Private IsNaInstalledAttr As Boolean = False
'        Private IsKInstalledAttr As Boolean = False
'        Private IsClInstalledAttr As Boolean = False
'        Private IsRefExpiredAttr As Boolean = False
'        Private IsLiExpiredAttr As Boolean = False
'        Private IsNaExpiredAttr As Boolean = False
'        Private IsKExpiredAttr As Boolean = False
'        Private IsClExpiredAttr As Boolean = False
'        Private IsRefOverusedAttr As Boolean = False
'        Private IsLiOverusedAttr As Boolean = False
'        Private IsNaOverusedAttr As Boolean = False
'        Private IsKOverusedAttr As Boolean = False
'        Private IsClOverusedAttr As Boolean = False

'        'recommended operations
'        Private IsCalibrationNeededAttr As Boolean = False
'        Private IsPumpCalibrationNeededAttr As Boolean = False
'        Private IsCleanNeededAttr As Boolean = False
'        Private IsReplaceLithiumNeededAttr As Boolean = False
'        Private IsReplaceSodiumNeededAttr As Boolean = False
'        Private IsReplacePotassiumNeededAttr As Boolean = False
'        Private IsReplaceClorineNeededAttr As Boolean = False
'        Private IsReplaceReferenceNeededAttr As Boolean = False
'        Private IsReplacePumpTubingNeededAttr As Boolean = False
'        Private IsReplaceFluidTubingNeededAttr As Boolean = False
'        Private IsReplaceReagentsPackNeededAttr As Boolean = False
'        Private IsCleaningNeededAttr As Boolean = False

'        'last calibrations and clean
'        Private LastElectrodesCalibrationResultAttr As String = ""
'        Private LastElectrodesCalibrationDateAttr As DateTime = Nothing
'        Private LastPumpsCalibrationResultAttr As String = ""
'        Private LastPumpsCalibrationDateAttr As DateTime = Nothing
'        Private LastBubbleCalibrationResultAttr As String = ""
'        Private LastBubbleCalibrationDateAttr As DateTime = Nothing
'        Private LastCleanDateAttr As DateTime = Nothing

'#End Region

'#Region "Private Enumerates"

'        Private Enum ExpirationItems
'            None
'            ReferenceElectrode
'            LithiumElectrode
'            SodiumElectrode
'            PotassiumElectrode
'            ClorineElectrode
'            CalibratorA
'            CalibratorB
'            PumpTubing
'            FluidicTubing
'        End Enum

'        Private Enum ValidationItems
'            None
'            LithiumElectrode
'            SodiumElectrode
'            PotassiumElectrode
'            ClorineElectrode
'            PumpCalibrationDiffAB
'            BubbleDetCalibrationDiffAL
'        End Enum
'#End Region

'#Region "Properties"

'#Region "Included Analyzer Information"

'        Public Property ISEInformation() As ISEInformationDS
'            Get
'                Return ISEInformationAttr
'            End Get
'            Set(ByVal value As ISEInformationDS)
'                ISEInformationAttr = value
'            End Set
'        End Property

'#End Region

'#Region "Monitored"

'        Public Property LastISEResult() As ISEResultTO
'            Get
'                Return LastISEResultAttr
'            End Get
'            Set(ByVal value As ISEResultTO)
'                LastISEResultAttr = value
'            End Set
'        End Property

'        ''' <summary>
'        ''' Indicates if the ISE Module is powered on in the Analyzer
'        ''' Is updated with the ANSINF instruction
'        ''' </summary>
'        ''' <value></value>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Property IsISESwitchedON() As Boolean
'            Get
'                Return IsISESwitchedONAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsISESwitchedONAttr = value
'            End Set
'        End Property


'        ''' <summary>
'        ''' Indicates that the communications between the Analyzer 
'        ''' and the ISE Module work properly.
'        ''' is updated after an ISE command has received a response
'        ''' </summary>
'        ''' <value></value>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Property IsISECommsOk() As Boolean
'            Get
'                Return IsISECommsOkAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsISECommsOkAttr = value
'            End Set
'        End Property


'#End Region

'#Region "Readiness"

'#Region "Common"

'        Public Property IsISEModuleReady() As Boolean
'            Get
'                Return (AreReagentsReady And IsElectrodesReady And Not IsLongTermDeactivation)
'            End Get
'            Set(ByVal value As Boolean)
'                IsISEModuleReadyAttr = value 'READONLY?
'            End Set
'        End Property

'        'This property is updated after user set/reset the Long Term Deactivation
'        Public Property IsLongTermDeactivation() As Boolean
'            Get
'                Return IsLongTermDeactivationAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsLongTermDeactivationAttr = value
'            End Set
'        End Property

'#End Region

'#Region "Reagents Pack"

'        'This property is updated after validating that Reagents pack are installed and not expired
'        Public Property AreReagentsInstalled() As Boolean
'            Get
'                Return ((HasBiosystemsCode And Not IsCleanPackInstalled) _
'                        And HaveReagentsInstallationDate)
'            End Get
'            Set(ByVal value As Boolean)
'                AreReagentsInstalledAttr = value 'READONLY?
'            End Set
'        End Property


'        'This property is updated after validating that Reagents pack are installed and not expired
'        Public Property AreReagentsReady() As Boolean
'            Get
'                Return (AreReagentsInstalled _
'                        And Not AreReagentsExpired _
'                        And IsEnoughCalibratorA _
'                        And IsEnoughCalibratorB)
'            End Get
'            Set(ByVal value As Boolean)
'                AreReagentsReadyAttr = value 'READONLY?
'            End Set
'        End Property

'        'This property is updated after requesting Dallas chip data
'        Public Property HasBiosystemsCode() As Boolean
'            Get
'                Return HasBiosystemsCodeAttr
'            End Get
'            Set(ByVal value As Boolean)
'                HasBiosystemsCodeAttr = value
'            End Set
'        End Property

'        'This property is updated after setting Long Term Deactivation
'        Public Property IsCleanPackInstalled() As Boolean
'            Get
'                Return IsCleanPackInstalledAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsCleanPackInstalledAttr = value
'            End Set
'        End Property

'        'This property is updated after requesting Dallas chip data
'        Public Property HaveReagentsInstallationDate() As Boolean
'            Get
'                Return HaveReagentsInstallationDateAttr
'            End Get
'            Set(ByVal value As Boolean)
'                HaveReagentsInstallationDateAttr = value
'            End Set
'        End Property

'        Public Property AreReagentsExpired() As Boolean
'            Get
'                Return AreReagentsExpiredAttr
'            End Get
'            Set(ByVal value As Boolean)
'                AreReagentsExpiredAttr = value
'            End Set
'        End Property

'        Public Property IsEnoughCalibratorA() As Boolean
'            Get
'                Return IsEnoughCalibratorAAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsEnoughCalibratorAAttr = value
'            End Set
'        End Property

'        Public Property IsEnoughCalibratorB() As Boolean
'            Get
'                Return IsEnoughCalibratorBAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsEnoughCalibratorBAttr = value
'            End Set
'        End Property

'#End Region


'#Region "Electrodes"

'        'This property is updated after validating that Electrodes are installed
'        Public Property IsElectrodesReady() As Boolean
'            Get
'                Dim myReturn As Boolean = False
'                If IsLiEnabledByUser Then
'                    myReturn = ((HasRefInstallDate AndAlso IsRefInstalled) _
'                        And (HasLiInstallDate AndAlso IsLiInstalled) _
'                        And (HasNaInstallDate AndAlso IsNaInstalled) _
'                        And (HasKInstallDate AndAlso IsKInstalled) _
'                        And (HasClInstallDate AndAlso IsClInstalled))
'                Else
'                    myReturn = ((HasRefInstallDate AndAlso IsRefInstalled) _
'                        And (HasNaInstallDate AndAlso IsNaInstalled) _
'                        And (HasKInstallDate AndAlso IsKInstalled) _
'                        And (HasClInstallDate AndAlso IsClInstalled))
'                End If

'                Return myReturn

'            End Get
'            Set(ByVal value As Boolean)
'                IsElectrodesReadyAttr = value 'READONLY?
'            End Set
'        End Property

'        'defined by the user that the Lithium is usable
'        Public Property IsLiEnabledByUser() As Boolean
'            Get
'                Return IsLiEnabledByUserAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsLiEnabledByUserAttr = value
'            End Set
'        End Property


'        Public Property HasRefInstallDate() As Boolean
'            Get
'                Return HasRefInstallDateAttr
'            End Get
'            Set(ByVal value As Boolean)
'                HasRefInstallDateAttr = value
'            End Set
'        End Property

'        Public Property HasLiInstallDate() As Boolean
'            Get
'                Return HasLiInstallDateAttr
'            End Get
'            Set(ByVal value As Boolean)
'                HasLiInstallDateAttr = value
'            End Set
'        End Property

'        Public Property HasNaInstallDate() As Boolean
'            Get
'                Return HasNaInstallDateAttr
'            End Get
'            Set(ByVal value As Boolean)
'                HasNaInstallDateAttr = value
'            End Set
'        End Property

'        Public Property HasKInstallDate() As Boolean
'            Get
'                Return HasKInstallDateAttr
'            End Get
'            Set(ByVal value As Boolean)
'                HasKInstallDateAttr = value
'            End Set
'        End Property

'        Public Property HasClInstallDate() As Boolean
'            Get
'                Return HasClInstallDateAttr
'            End Get
'            Set(ByVal value As Boolean)
'                HasClInstallDateAttr = value
'            End Set
'        End Property

'        Public Property IsRefInstalled() As Boolean
'            Get
'                Return IsRefInstalledAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsRefInstalledAttr = value
'            End Set
'        End Property

'        Public Property IsLiInstalled() As Boolean
'            Get
'                Return IsLiInstalledAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsLiInstalledAttr = value
'            End Set
'        End Property

'        Public Property IsNaInstalled() As Boolean
'            Get
'                Return IsNaInstalledAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsNaInstalledAttr = value
'            End Set
'        End Property

'        Public Property IsKInstalled() As Boolean
'            Get
'                Return IsKInstalledAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsKInstalledAttr = value
'            End Set
'        End Property

'        Public Property IsClInstalled() As Boolean
'            Get
'                Return IsClInstalledAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsClInstalledAttr = value
'            End Set
'        End Property

'        Public Property IsRefExpired() As Boolean
'            Get
'                Return IsRefExpiredAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsRefExpiredAttr = value
'            End Set
'        End Property

'        Public Property IsLiExpired() As Boolean
'            Get
'                Return IsLiExpiredAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsLiExpiredAttr = value
'            End Set
'        End Property

'        Public Property IsNaExpired() As Boolean
'            Get
'                Return IsNaExpiredAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsNaExpiredAttr = value
'            End Set
'        End Property

'        Public Property IsKExpired() As Boolean
'            Get
'                Return IsKExpiredAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsKExpiredAttr = value
'            End Set
'        End Property

'        Public Property IsClExpired() As Boolean
'            Get
'                Return IsClExpiredAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsClExpiredAttr = value
'            End Set
'        End Property

'        Public Property IsRefOverused() As Boolean
'            Get
'                Return IsRefOverusedAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsRefOverusedAttr = value
'            End Set
'        End Property

'        Public Property IsLiOverused() As Boolean
'            Get
'                Return IsLiOverusedAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsLiOverusedAttr = value
'            End Set
'        End Property

'        Public Property IsNaOverused() As Boolean
'            Get
'                Return IsNaOverusedAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsNaOverusedAttr = value
'            End Set
'        End Property

'        Public Property IsKOverused() As Boolean
'            Get
'                Return IsKOverusedAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsKOverusedAttr = value
'            End Set
'        End Property

'        Public Property IsClOverused() As Boolean
'            Get
'                Return IsClOverusedAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsClOverusedAttr = value
'            End Set
'        End Property
'#End Region


'#End Region

'#Region "Needed Operations"

'        'This property is updated after validating that a Electrodes' Calibration is needed
'        Public Property IsCalibrationNeeded() As Boolean
'            Get
'                Return IsCalibrationNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsCalibrationNeededAttr = value
'            End Set
'        End Property

'        'This property is updated after validating that a Pumps' Calibration is needed
'        Public Property IsPumpCalibrationNeeded() As Boolean
'            Get
'                Return IsPumpCalibrationNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsPumpCalibrationNeededAttr = value
'            End Set
'        End Property

'        Public Property IsCleanNeeded() As Boolean
'            Get
'                Return IsCleanNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsCleanNeededAttr = value
'            End Set
'        End Property

'        'This property is updated after checking that the corresponding item has expired
'        Public Property IsReplaceLithiumNeeded() As Boolean
'            Get
'                Return IsReplaceLithiumNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsReplaceLithiumNeededAttr = value
'            End Set
'        End Property


'        'This property is updated after checking that the corresponding item has expired
'        Public Property IsReplaceSodiumNeeded() As Boolean
'            Get
'                Return IsReplaceSodiumNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsReplaceSodiumNeededAttr = value
'            End Set
'        End Property


'        'This property is updated after checking that the corresponding item has expired
'        Public Property IsReplacePotassiumNeeded() As Boolean
'            Get
'                Return IsReplacePotassiumNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsReplacePotassiumNeededAttr = value
'            End Set
'        End Property


'        'This property is updated after checking that the corresponding item has expired
'        Public Property IsReplaceClorineNeeded() As Boolean
'            Get
'                Return IsReplaceClorineNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsReplaceClorineNeededAttr = value
'            End Set
'        End Property


'        Public Property IsReplaceReferenceNeeded() As Boolean
'            Get
'                Return IsReplaceReferenceNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsReplaceReferenceNeededAttr = value
'            End Set
'        End Property


'        'This property is updated after checking that the corresponding item has expired
'        Public Property IsReplacePumpTubingNeeded() As Boolean
'            Get
'                Return IsReplacePumpTubingNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsReplacePumpTubingNeededAttr = value
'            End Set
'        End Property


'        'This property is updated after checking that the corresponding item has expired
'        Public Property IsReplaceFluidTubingNeeded() As Boolean
'            Get
'                Return IsReplaceFluidTubingNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsReplaceFluidTubingNeededAttr = value
'            End Set
'        End Property



'        'This property is updated after checking that the Reagents Pack has expired
'        Public Property IsReplaceReagentsPackNeeded() As Boolean
'            Get
'                Return IsReplaceReagentsPackNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsReplaceReagentsPackNeededAttr = value
'            End Set
'        End Property


'        'This property is updated after checking that a Cleaning Cycle is needed
'        Public Property IsCleaningNeeded() As Boolean
'            Get
'                Return IsCleaningNeededAttr
'            End Get
'            Set(ByVal value As Boolean)
'                IsCleaningNeededAttr = value
'            End Set
'        End Property

'#End Region


'#Region "Last Calibratrions and Cleans"

'        Public Property LastElectrodesCalibrationResult() As String
'            Get
'                Return LastElectrodesCalibrationResultAttr
'            End Get
'            Set(ByVal value As String)
'                LastElectrodesCalibrationResultAttr = value
'            End Set
'        End Property

'        Public Property LastPumpsCalibrationResult() As String
'            Get
'                Return LastPumpsCalibrationResultAttr
'            End Get
'            Set(ByVal value As String)
'                LastPumpsCalibrationResultAttr = value
'            End Set
'        End Property

'        Public Property LastBubbleCalibrationResult() As String
'            Get
'                Return LastBubbleCalibrationResultAttr
'            End Get
'            Set(ByVal value As String)
'                LastBubbleCalibrationResultAttr = value
'            End Set
'        End Property

'        Public Property LastElectrodesCalibrationDate() As DateTime
'            Get
'                Return LastElectrodesCalibrationDateAttr
'            End Get
'            Set(ByVal value As DateTime)
'                LastElectrodesCalibrationDateAttr = value
'            End Set
'        End Property

'        Public Property LastPumpsCalibrationDate() As DateTime
'            Get
'                Return LastPumpsCalibrationDateAttr
'            End Get
'            Set(ByVal value As DateTime)
'                LastPumpsCalibrationDateAttr = value
'            End Set
'        End Property

'        Public Property LastBubbleCalibrationDate() As DateTime
'            Get
'                Return LastBubbleCalibrationDateAttr
'            End Get
'            Set(ByVal value As DateTime)
'                LastBubbleCalibrationDateAttr = value
'            End Set
'        End Property

'        Public Property LastCleanDate() As DateTime
'            Get
'                Return LastCleanDateAttr
'            End Get
'            Set(ByVal value As DateTime)
'                LastCleanDateAttr = value
'            End Set
'        End Property

'#End Region
'#End Region

'#Region "Methods"




'#End Region

'    End Class
'End Namespace