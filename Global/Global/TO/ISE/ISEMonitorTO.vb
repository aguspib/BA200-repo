Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.Global

    ''' <summary>
    ''' Contains all the data needed by the ISE monitor to display
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ISEMonitorTO

#Region "Public Structures"

        Public Structure ElectrodeData
            Public Id As GlobalEnumerates.ISE_Electrodes
            Public Installed As Boolean
            Public InstallationDate As DateTime
            Public IsExpired As Boolean
            Public IsOverUsed As Boolean
            Public TestCount As Integer
            Public Sub New(ByVal pId As GlobalEnumerates.ISE_Electrodes)
                Id = pId
                Installed = False
                InstallationDate = Nothing
                IsExpired = False
                IsOverUsed = False
                TestCount = -1
            End Sub
        End Structure
#End Region

#Region "Attribute"

        'common
        Private IsInitiatedOKAttr As Boolean = False
        Private HasDataAttr As Boolean = False
        Private IsLongTermDeactivationAttr As Boolean = False
        Private SafetyCalAVolumeAttr As Single = 5
        Private SafetyCalBVolumeAttr As Single = 5

        'ReagentsPack:
        Private RP_InstallationDateAttr As DateTime
        Private RP_ExpirationDateAttr As DateTime
        Private RP_IsExpiredAttr As Boolean
        Private RP_RemainingVolAAttr As Single
        Private RP_RemainingVolBAttr As Single
        Private RP_InitialVolAAttr As Single
        Private RP_InitialVolBAttr As Single
        Private RP_IsEnoughVolAAttr As Boolean
        Private RP_IsEnoughVolBAttr As Boolean
        Private RP_HasBioCodeAttr As Boolean = True
        Private RP_CleanPackInstalledAttr As Boolean

        'Electrodes:
        Private REF_DataAttr As New ElectrodeData(GlobalEnumerates.ISE_Electrodes.Ref)
        Private NA_DataAttr As New ElectrodeData(GlobalEnumerates.ISE_Electrodes.Na)
        Private K_DataAttr As New ElectrodeData(GlobalEnumerates.ISE_Electrodes.K)
        Private CL_DataAttr As New ElectrodeData(GlobalEnumerates.ISE_Electrodes.Cl)
        Private LI_DataAttr As New ElectrodeData(GlobalEnumerates.ISE_Electrodes.Li)
        Private LI_EnabledAttr As Boolean = False

        'Calibrations:
        Private CAL_ElectrodesCalibDateAttr As DateTime
        Private CAL_ElectrodesCalibResult1StringAttr As String
        Private CAL_ElectrodesCalibResult2StringAttr As String
        Private CAL_ElectrodesCalibResult1OKAttr As Boolean
        Private CAL_ElectrodesCalibResult2OKAttr As Boolean
        Private CAL_PumpsCalibDateAttr As DateTime
        Private CAL_PumpsCalibResultStringAttr As String
        Private CAL_PumpsCalibResultOKAttr As Boolean
        Private CAL_BubbleCalibDateAttr As DateTime
        Private CAL_BubbleCalibResultStringAttr As String
        Private CAL_BubbleCalibResultOKAttr As Boolean
        Private CAL_ElectrodesRecommendedAttr As Boolean
        Private CAL_PumpsRecommendedAttr As Boolean
        Private CAL_BubbleRecommendedAttr As Boolean
        Private CleanRecommendedAttr As Boolean
        Private CleanDateAttr As DateTime

        ' XB 26/05/2014 - BT #1639 - Do not lock ISE preparations during Runnning (not Pause) by Pending Calibrations
        Private CAL_ElectrodesCalibExpiredTimeAttr As Integer
        Private CAL_PumpsCalibExpiredTimeAttr As Integer

        'Tubing
        Private TUB_PumpInstallDateAttr As DateTime
        Private TUB_FluidInstallDateAttr As DateTime

#End Region

#Region "Constructor"
        Public Sub New()
            HasDataAttr = False
        End Sub

        Public Sub New(ByVal pSafetyCalAVol As Single, _
                       ByVal pSafetyCalBVol As Single, _
                       ByVal pRP_InstallationDate As DateTime, _
                        ByVal pRP_ExpirationDate As DateTime, _
                        ByVal pRP_IsExpired As Boolean, _
                        ByVal pRP_RemainingVolA As Single, _
                        ByVal pRP_RemainingVolB As Single, _
                        ByVal pRP_InitialVolA As Single, _
                        ByVal pRP_InitialVolB As Single, _
                        ByVal pRP_IsEnoughVolA As Boolean, _
                        ByVal pRP_IsEnoughVolB As Boolean, _
                        ByVal pRP_HasBioCode As Boolean, _
                        ByVal pREF_Data As ElectrodeData, _
                        ByVal pLI_Data As ElectrodeData, _
                        ByVal pNA_Data As ElectrodeData, _
                        ByVal pK_Data As ElectrodeData, _
                        ByVal pCL_Data As ElectrodeData, _
                        ByVal pLI_Enabled As Boolean, _
                        ByVal pCAL_ElectrodesCalibDate As DateTime, _
                        ByVal pCAL_ElectrodesCalibResult1String As String, _
                        ByVal pCAL_ElectrodesCalibResult2String As String, _
                        ByVal pCAL_ElectrodesCalibResult1OK As Boolean, _
                        ByVal pCAL_ElectrodesCalibResult2OK As Boolean, _
                        ByVal pCAL_ElectrodesRecommended As Boolean, _
                        ByVal pCAL_PumpsCalibDate As DateTime, _
                        ByVal pCAL_PumpsCalibResultString As String, _
                        ByVal pCAL_PumpsCalibResultOK As Boolean, _
                        ByVal pCAL_PumpsRecommended As Boolean, _
                        ByVal pCAL_BubbleCalibDate As DateTime, _
                        ByVal pCAL_BubbleCalibResultString As String, _
                        ByVal pCAL_BubbleCalibResultOK As Boolean, _
                        ByVal pCAL_BubbleRecommended As Boolean, _
                        ByVal pCleanDate As DateTime, _
                        ByVal pCleanRecommended As Boolean, _
                        ByVal pInitiatedOK As Boolean, _
                        ByVal pIsLongTermDeactivation As Boolean, _
                        ByVal pCAL_ElectrodesCalibExpiredTime As Integer, _
                        ByVal pCAL_PumpsCalibExpiredTime As Integer)


            'ByVal pLI_Enabled As Boolean, _

            SafetyCalAVolumeAttr = pSafetyCalAVol
            SafetyCalBVolumeAttr = pSafetyCalBVol

            'ReagentsPack:
            RP_InstallationDateAttr = pRP_InstallationDate
            RP_ExpirationDateAttr = pRP_ExpirationDate
            RP_IsExpiredAttr = pRP_IsExpired
            RP_RemainingVolAAttr = pRP_RemainingVolA
            RP_RemainingVolBAttr = pRP_RemainingVolB
            RP_InitialVolAAttr = pRP_InitialVolA
            RP_InitialVolBAttr = pRP_InitialVolB
            RP_IsEnoughVolAAttr = pRP_IsEnoughVolA
            RP_IsEnoughVolBAttr = pRP_IsEnoughVolB
            RP_HasBioCodeAttr = pRP_HasBioCode

            'Electrodes:
            REF_DataAttr = pREF_Data
            LI_DataAttr = pLI_Data
            NA_DataAttr = pNA_Data
            K_DataAttr = pK_Data
            CL_DataAttr = pCL_Data

            LI_EnabledAttr = pLI_Enabled

            'Calibrations:
            CAL_ElectrodesCalibDateAttr = pCAL_ElectrodesCalibDate
            CAL_ElectrodesCalibResult1StringAttr = pCAL_ElectrodesCalibResult1String
            CAL_ElectrodesCalibResult2StringAttr = pCAL_ElectrodesCalibResult2String
            CAL_ElectrodesCalibResult1OKAttr = pCAL_ElectrodesCalibResult1OK
            CAL_ElectrodesCalibResult2OKAttr = pCAL_ElectrodesCalibResult2OK
            CAL_ElectrodesRecommendedAttr = pCAL_ElectrodesRecommended

            CAL_PumpsCalibDateAttr = pCAL_PumpsCalibDate
            CAL_PumpsCalibResultStringAttr = pCAL_PumpsCalibResultString
            CAL_PumpsCalibResultOKAttr = pCAL_PumpsCalibResultOK
            CAL_PumpsRecommendedAttr = pCAL_PumpsRecommended

            CAL_BubbleCalibDateAttr = pCAL_BubbleCalibDate
            CAL_BubbleCalibResultStringAttr = pCAL_BubbleCalibResultString
            CAL_BubbleCalibResultOKAttr = pCAL_BubbleCalibResultOK
            CAL_BubbleRecommendedAttr = pCAL_BubbleRecommended

            CleanDateAttr = pCleanDate
            CleanRecommendedAttr = pCleanRecommended

            IsInitiatedOKAttr = pInitiatedOK

            IsLongTermDeactivationAttr = pIsLongTermDeactivation

            HasDataAttr = True


            ' XB 26/05/2014 - BT #1639 - Do not lock ISE preparations during Runnning (not Pause) by Pending Calibrations
            CAL_ElectrodesCalibExpiredTimeAttr = pCAL_ElectrodesCalibExpiredTime
            CAL_PumpsCalibExpiredTimeAttr = pCAL_PumpsCalibExpiredTime

        End Sub

#End Region

#Region "Properties"

        Public ReadOnly Property HasData() As Boolean
            Get
                Return HasDataAttr
            End Get
        End Property

        'The initialization has been performed successfully
        Public Property IsInitiatedOK() As Boolean
            Get
                Return MyClass.IsInitiatedOKAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.IsInitiatedOKAttr = value
            End Set
        End Property

        Public Property IsLongTermDeactivation() As Boolean
            Get
                Return MyClass.IsLongTermDeactivationAttr
            End Get
            Set(ByVal value As Boolean)
                MyClass.IsLongTermDeactivationAttr = value
            End Set
        End Property

        Public ReadOnly Property SafetyCalAVolume() As Single
            Get
                Return SafetyCalAVolumeAttr
            End Get
        End Property

        Public ReadOnly Property SafetyCalBVolume() As Single
            Get
                Return SafetyCalBVolumeAttr
            End Get
        End Property

#Region "Reagents Pack"

        Public ReadOnly Property RP_InstallationDate() As DateTime
            Get
                Return RP_InstallationDateAttr
            End Get
        End Property
        Public ReadOnly Property RP_HasBioCode() As Boolean
            Get
                Return RP_HasBioCodeAttr
            End Get
        End Property
        Public ReadOnly Property RP_CleanPackInstalled() As Boolean
            Get
                Return RP_CleanPackInstalledAttr
            End Get
        End Property
        Public ReadOnly Property RP_ExpirationDate() As DateTime
            Get
                Return RP_ExpirationDateAttr
            End Get
        End Property
        Public ReadOnly Property RP_IsExpired() As Boolean
            Get
                Return RP_IsExpiredAttr
            End Get
        End Property
        Public ReadOnly Property RP_RemainingVolA() As Single
            Get
                Return RP_RemainingVolAAttr
            End Get
        End Property
        Public ReadOnly Property RP_RemainingVolB() As Single
            Get
                Return RP_RemainingVolBAttr
            End Get
        End Property
        Public ReadOnly Property RP_InitialVolA() As Single
            Get
                Return RP_InitialVolAAttr
            End Get
        End Property
        Public ReadOnly Property RP_InitialVolB() As Single
            Get
                Return RP_InitialVolBAttr
            End Get
        End Property
        Public ReadOnly Property RP_IsEnoughVolA() As Boolean
            Get
                Return RP_IsEnoughVolAAttr
            End Get
        End Property

        Public ReadOnly Property RP_IsEnoughVolB() As Boolean
            Get
                Return RP_IsEnoughVolBAttr
            End Get
        End Property

        Public ReadOnly Property TUB_PumpInstallDate() As DateTime
            Get
                Return TUB_PumpInstallDateAttr
            End Get
        End Property

        Public ReadOnly Property TUB_FluidInstallDate() As DateTime
            Get
                Return TUB_FluidInstallDateAttr
            End Get
        End Property
#End Region

#Region "Electrodes"

        'Electrodes:
        Public ReadOnly Property REF_Data() As ElectrodeData
            Get
                Return REF_DataAttr
            End Get
        End Property
        Public ReadOnly Property LI_Data() As ElectrodeData
            Get
                Return LI_DataAttr
            End Get
        End Property
        Public ReadOnly Property NA_Data() As ElectrodeData
            Get
                Return NA_DataAttr
            End Get
        End Property
        Public ReadOnly Property K_Data() As ElectrodeData
            Get
                Return K_DataAttr
            End Get
        End Property
        Public ReadOnly Property CL_Data() As ElectrodeData
            Get
                Return CL_DataAttr
            End Get
        End Property

        Public ReadOnly Property LI_Enabled() As Boolean
            Get
                Return LI_EnabledAttr
            End Get
        End Property
#End Region

#Region "Calibrations"

        Public ReadOnly Property CAL_ElectrodesCalibDate() As DateTime
            Get
                Return CAL_ElectrodesCalibDateAttr
            End Get
        End Property
        Public ReadOnly Property CAL_ElectrodesCalibResult1String() As String
            Get
                Return CAL_ElectrodesCalibResult1StringAttr
            End Get
        End Property
        Public ReadOnly Property CAL_ElectrodesCalibResult2String() As String
            Get
                Return CAL_ElectrodesCalibResult2StringAttr
            End Get
        End Property
        Public ReadOnly Property CAL_ElectrodesCalibResult1OK() As Boolean
            Get
                Return CAL_ElectrodesCalibResult1OKAttr
            End Get
        End Property
        Public ReadOnly Property CAL_ElectrodesCalibResult2OK() As Boolean
            Get
                Return CAL_ElectrodesCalibResult2OKAttr
            End Get
        End Property
        Public ReadOnly Property CAL_PumpsCalibDate() As DateTime
            Get
                Return CAL_PumpsCalibDateAttr
            End Get
        End Property
        Public ReadOnly Property CAL_PumpsCalibResultString() As String
            Get
                Return CAL_PumpsCalibResultStringAttr
            End Get
        End Property
        Public ReadOnly Property CAL_PumpsCalibResultOK() As Boolean
            Get
                Return CAL_PumpsCalibResultOKAttr
            End Get
        End Property

        Public ReadOnly Property CAL_BubbleCalibDate() As DateTime
            Get
                Return CAL_BubbleCalibDateAttr
            End Get
        End Property
        Public ReadOnly Property CAL_BubbleCalibResultString() As String
            Get
                Return CAL_BubbleCalibResultStringAttr
            End Get
        End Property
        Public ReadOnly Property CAL_BubbleCalibResultOK() As Boolean
            Get
                Return CAL_BubbleCalibResultOKAttr
            End Get
        End Property

        Public ReadOnly Property CAL_ElectrodesRecommended() As Boolean
            Get
                Return CAL_ElectrodesRecommendedAttr
            End Get
        End Property

        Public ReadOnly Property CAL_PumpsRecommended() As Boolean
            Get
                Return CAL_PumpsRecommendedAttr
            End Get
        End Property

        Public ReadOnly Property CAL_BubbleRecommended() As Boolean
            Get
                Return CAL_BubbleRecommendedAttr
            End Get
        End Property

        Public ReadOnly Property CleanDate() As DateTime
            Get
                Return CleanDateAttr
            End Get
        End Property

        Public ReadOnly Property CleanRecommended() As Boolean
            Get
                Return CleanRecommendedAttr
            End Get
        End Property


        ' XB 26/05/2014 - BT #1639 - Do not lock ISE preparations during Runnning (not Pause) by Pending Calibrations
        Public ReadOnly Property CAL_ElectrodesCalibExpiredTime() As Integer
            Get
                Return CAL_ElectrodesCalibExpiredTimeAttr
            End Get
        End Property

        Public ReadOnly Property CAL_PumpsCalibExpiredTime() As Integer
            Get
                Return CAL_PumpsCalibExpiredTimeAttr
            End Get
        End Property
#End Region


#End Region


    End Class
End Namespace