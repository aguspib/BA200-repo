Option Explicit On
Option Strict On

Imports Microsoft.Win32
Imports System.Windows.Forms
'Imports System.Configuration
'Imports System.Globalization
Imports System.IO
Imports System.Management
Imports System.Data.Sql

'Imports Biosystems.Ax00.Global.TO
'Imports Biosystems.Ax00

'Reference:
'http://www.webtropy.com/articles/wmi-namespace.asp?wmi=root-CIMV2-Applications-MicrosoftIE 



Namespace Biosystems.Ax00.Global
    Public Class PCInfoReader
        Inherits GlobalBase

#Region "Private Enumerates" 'SERGIO


#End Region

#Region "Structures"

        'Public Structure AX00PCOSCultureInfo
        '    Public DecimalSeparator As String
        '    Public GroupSeparator As String
        '    Public DateFormat As String
        '    Public TimeFormat As String
        '    Public TimeFormatShort As String
        '    Public TimeSeparator As String
        '    Public DateSeparator As String
        '    Public Language As String
        'End Structure

        Public Structure AX00PCOSInfo
            Public Name As String
            Public Version As String
            Public ComputerName As String
            Public WindowsDirectory As String
            Public LastBootUpTime As String
            Public Caption As String
            Public CurrentTimeZone As Integer
            Public LocalDateTime As String
            Public Manufacturer As String
            Public NumberOfUsers As String
            Public Organization As String
            Public OSLanguage As Integer
            Public RegisteredUser As String
            Public SerialNumber As String
        End Structure

        Public Structure AX00PCComputerInfo
            Public Name As String
            Public Manufacturer As String
            Public Model As String
            Public SystemType As String
            Public Domain As String
            Public OSFullname As String
            Public OSPlatForm As String
            Public OSVersion As String
            Public AvailablePhysicalRAM As Long
            Public AvailableVirtualRAM As Long
            Public TotalPhysicalRAM As Long
            Public TotalVirtualRAM As Long
        End Structure

        Public Structure AX00PCDiskDriveInfo
            Public Capabilities As String
            Public Caption As String
            Public Description As String
            Public DeviceID As String
            Public Index As Integer
            Public InterfaceType As String
            Public Manufacturer As String
            Public MediaLoaded As Boolean
            Public MediaType As String
            Public Model As String
            Public Name As String
            Public Partitions As Integer
            Public PNPDeviceID As String
            Public Size As Long
            Public Status As String
            Public SystemName As String
            Public TotalCylinders As Long
            Public TotalHeads As Long
            Public TotalSectors As Long
            Public TotalTracks As Long
        End Structure

        Public Structure AX00PCServerInfo
            Public Name As String
            Public Instance As String
            Public Version As String
        End Structure

        Public Structure AX00PCUserInfo
            Public Domain As String
            Public FullName As String
            Public LocalAccount As Boolean
            Public LockOut As Boolean
            Public AccountType As String
            Public Caption As String
            Public Description As String
            Public Status As String
        End Structure

        Public Structure AX00PCSerialPortInfo
            Public DeviceID As String
            Public Description As String
            Public Name As String
            Public MaxBaudRate As String
            Public MaximumInputBufferSize As String
            Public MaximumOutputBufferSize As String
            Public OSAutoDiscovered As String
            Public PNPDeviceID As String
            Public ProviderType As String
            Public SettableBaudRate As Boolean
            Public SettableDataBits As Boolean
            Public SettableFlowControl As Boolean
            Public SettableParity As Boolean
            Public SettableParityCheck As Boolean
            Public SettableRLSD As Boolean
            Public SettableStopBits As Boolean
            Public Supports16BitMode As Boolean
            Public SupportsDTRDSR As Boolean
            Public SupportsElapsedTimeouts As Boolean
            Public SupportsIntTimeouts As Boolean
            Public SupportsParityCheck As Boolean
            Public SupportsRLSD As Boolean
            Public SupportsRTSCTS As Boolean
            Public SupportsSpecialCharacters As Boolean
            Public SupportsXOnXOff As Boolean
            Public SupportsXOnXOffSet As Boolean
        End Structure

        Public Structure AX00PCPrinterInfo
            Public Name As String
            Public Caption As String
            Public CurrentCharSet As String
            Public CurrentLanguage As String
            Public CurrentMimeType As String
            Public CurrentNaturalLanguage As String
            Public CurrentPaperType As String
            Public Description As String
            Public DeviceID As String
            Public DriverName As String
            Public Hidden As Boolean
            Public Local As Boolean
            Public Location As String
            Public Network As Boolean
            Public PortName As String
            Public Status As String
            Public SystemName As String
            Public TimeOfLastReset As String
        End Structure

        Public Structure AX00PCServiceInfo
            Public Name As String
            Public Caption As String
            Public Description As String
            Public PathName As String
            Public InstallDate As DateTime
            Public ServiceType As String
            Public Started As Boolean
            Public StartMode As String
            Public StartName As String
            Public State As String
            Public Status As String
        End Structure

#End Region

#Region "Declarations"

#End Region

#Region "Attributes"

        Private Shared IsSQLServerInstalledAttribute As Boolean
        Private Shared IsSQLServerProcessRunningAttribute As Boolean
        Private Shared IsSQLServerServiceActiveAttribute As Boolean

        Private Shared OperativeSystemInfoAttr As AX00PCOSInfo
        Private Shared ComputerInfoAttr As AX00PCComputerInfo
        'Private Shared OSCultureInfoAttr As AX00PCOSCultureInfo

        Private Shared InstalledApplicationsAttr As List(Of String)
        Private Shared InstalledServicesAttr As List(Of AX00PCServiceInfo)
        Private Shared ServersInfoAttr As List(Of AX00PCServerInfo)
        Private Shared PrintersInfoAttr As List(Of AX00PCPrinterInfo)
        Private Shared UsersInfoAttr As List(Of AX00PCUserInfo)
        Private Shared SerialPortsInfoAttr As List(Of AX00PCSerialPortInfo)
        Private Shared DrivesInfoAttr As List(Of AX00PCDiskDriveInfo)

        Private Shared ProcessorInfoAtt As String

#End Region

#Region "Properties"
        Public Property IsSQLServerInstalled() As Boolean
            Get
                Return IsSQLServerInstalledAttribute
            End Get
            Set(ByVal value As Boolean)
                IsSQLServerInstalledAttribute = value
            End Set
        End Property

        Public Property IsSQLServerProcessRunning() As Boolean
            Get
                Return IsSQLServerProcessRunningAttribute
            End Get
            Set(ByVal value As Boolean)
                IsSQLServerProcessRunningAttribute = value
            End Set
        End Property

        Public Property IsSQLServerServiceActive() As Boolean
            Get
                Return IsSQLServerProcessRunningAttribute
            End Get
            Set(ByVal value As Boolean)
                IsSQLServerProcessRunningAttribute = value
            End Set
        End Property

        Public Property ComputerInfo() As AX00PCComputerInfo
            Get
                Return ComputerInfoAttr
            End Get
            Set(ByVal value As AX00PCComputerInfo)
                ComputerInfoAttr = value
            End Set
        End Property

        Public Property OperativeSystemInfo() As AX00PCOSInfo
            Get
                Return OperativeSystemInfoAttr
            End Get
            Set(ByVal value As AX00PCOSInfo)
                OperativeSystemInfoAttr = value
            End Set
        End Property

        'Public Property OSCultureInfo() As AX00PCOSCultureInfo
        '    Get
        '        Return OSCultureInfoAttr
        '    End Get
        '    Set(ByVal value As AX00PCOSCultureInfo)
        '        OSCultureInfoAttr = value
        '    End Set
        'End Property

        Public Property ProcessorInfo() As String
            Get
                Return ProcessorInfoAtt
            End Get
            Set(ByVal value As String)
                ProcessorInfoAtt = value
            End Set
        End Property

        Public Property InstalledApplications() As List(Of String)
            Get
                Return InstalledApplicationsAttr
            End Get
            Set(ByVal value As List(Of String))
                InstalledApplicationsAttr = value
            End Set
        End Property
        Public Property InstalledServices() As List(Of AX00PCServiceInfo)
            Get
                Return InstalledServicesAttr
            End Get
            Set(ByVal value As List(Of AX00PCServiceInfo))
                InstalledServicesAttr = value
            End Set
        End Property
        Public Property ServersInfo() As List(Of AX00PCServerInfo)
            Get
                Return ServersInfoAttr
            End Get
            Set(ByVal value As List(Of AX00PCServerInfo))
                ServersInfoAttr = value
            End Set
        End Property
        Public Property PrintersInfo() As List(Of AX00PCPrinterInfo)
            Get
                Return PrintersInfoAttr
            End Get
            Set(ByVal value As List(Of AX00PCPrinterInfo))
                PrintersInfoAttr = value
            End Set
        End Property
        Public Property UsersInfo() As List(Of AX00PCUserInfo)
            Get
                Return UsersInfoAttr
            End Get
            Set(ByVal value As List(Of AX00PCUserInfo))
                UsersInfoAttr = value
            End Set
        End Property
        Public Property SerialPortsInfo() As List(Of AX00PCSerialPortInfo)
            Get
                Return SerialPortsInfoAttr
            End Get
            Set(ByVal value As List(Of AX00PCSerialPortInfo))
                SerialPortsInfoAttr = value
            End Set
        End Property
        Public Property DrivesInfo() As List(Of AX00PCDiskDriveInfo)
            Get
                Return DrivesInfoAttr
            End Get
            Set(ByVal value As List(Of AX00PCDiskDriveInfo))
                DrivesInfoAttr = value
            End Set
        End Property


#End Region

#Region "Constructor"

        Public Sub New()
            IsSQLServerInstalledAttribute = False
            IsSQLServerProcessRunningAttribute = False
            IsSQLServerServiceActiveAttribute = False
            OperativeSystemInfoAttr = New AX00PCOSInfo
            ComputerInfoAttr = New AX00PCComputerInfo
            'OSCultureInfoAttr = New AX00PCOSCultureInfo

            InstalledApplicationsAttr = New List(Of String)
            ServersInfoAttr = New List(Of AX00PCServerInfo)
            PrintersInfoAttr = New List(Of AX00PCPrinterInfo)
            UsersInfoAttr = New List(Of AX00PCUserInfo)
            SerialPortsInfoAttr = New List(Of AX00PCSerialPortInfo)
            DrivesInfoAttr = New List(Of AX00PCDiskDriveInfo)
        End Sub

#End Region

#Region "Methods"

#Region "Public"

        ''' <summary>
        ''' Get's overall information about the suystem (PC and OS) alowing to export
        ''' </summary>
        ''' <param name="pExport"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Shared Function GetSystemInfo(ByVal pExport As Boolean) As String
            Try
                'RH 11/11/2010
                'Note that now we can have SQL Server Express 2005, 2008 or 2008 R2, and with any instance name so,
                'perhaps this function needs some update

                Dim myResponse As GlobalDataTO

                myResponse = PCInfoReader.GetProcessorInfo()
                If Not myResponse.HasError AndAlso myResponse IsNot Nothing Then
                    ProcessorInfoAtt = CType(myResponse.SetDatos, String)
                End If


                'Computer
                myResponse = PCInfoReader.GetComputerInfo()
                If Not myResponse.HasError AndAlso myResponse IsNot Nothing Then
                    ComputerInfoAttr = CType(myResponse.SetDatos, AX00PCComputerInfo)
                End If

                'Operative System
                myResponse = PCInfoReader.GetOSInfo()
                If Not myResponse.HasError AndAlso myResponse IsNot Nothing Then
                    OperativeSystemInfoAttr = CType(myResponse.SetDatos, AX00PCOSInfo)
                End If

                'culture
                'myResponse = PCInfoReader.GetOSCultureInfo()
                'If Not myResponse.HasError AndAlso myResponse IsNot Nothing Then
                '    OSCultureInfoAttr = CType(myResponse.SetDatos, AX00PCOSCultureInfo)
                'End If

                'Installed Applications
                myResponse = PCInfoReader.GetInstalledApplications(OperativeSystemInfoAttr.ComputerName)
                If Not myResponse.HasError AndAlso myResponse IsNot Nothing Then
                    InstalledApplicationsAttr = CType(myResponse.SetDatos, List(Of String))
                End If

                'RH 12/11/2010 Perhaps this code needs some update. Does not work properly.
                If InstalledApplicationsAttr.Contains("Microsoft SQL Server 2005 Express Edition (SQLEXPRESS)") Then
                    IsSQLServerInstalledAttribute = True
                End If

                'Installed services
                myResponse = PCInfoReader.GetInstalledServices()
                If Not myResponse.HasError AndAlso myResponse IsNot Nothing Then
                    InstalledServicesAttr = CType(myResponse.SetDatos, List(Of AX00PCServiceInfo))
                End If

                'Users info
                myResponse = PCInfoReader.GetUsersInfo(ComputerInfoAttr.Domain)
                If Not myResponse.HasError AndAlso myResponse IsNot Nothing Then
                    UsersInfoAttr = CType(myResponse.SetDatos, List(Of AX00PCUserInfo))
                End If

                'Server List
                'RH 01/07/2011 Commented these lines because they slow down the application's start up time
                'and this info is not needed until Restore SAT is done. So this operation is carry on only when needed.
                'myResponse = GetSQLServersInfo()
                'If Not myResponse.HasError AndAlso myResponse IsNot Nothing Then
                '    ServersInfoAttr = CType(myResponse.SetDatos, List(Of AX00PCServerInfo))
                'End If

                'Installed Printers
                myResponse = PCInfoReader.GetPrintersInfo()
                If Not myResponse.HasError And myResponse IsNot Nothing Then
                    PrintersInfoAttr = CType(myResponse.SetDatos, List(Of AX00PCPrinterInfo))
                End If

                'Serial Port
                myResponse = PCInfoReader.GetSerialPortsInfo
                If Not myResponse.HasError And myResponse IsNot Nothing Then
                    SerialPortsInfoAttr = CType(myResponse.SetDatos, List(Of AX00PCSerialPortInfo))
                End If

                'Drives
                myResponse = PCInfoReader.GetDiskDrivesInfo
                If Not myResponse.HasError And myResponse IsNot Nothing Then
                    DrivesInfoAttr = CType(myResponse.SetDatos, List(Of AX00PCDiskDriveInfo))
                End If

                'SQL Server Process
                IsSQLServerProcessRunningAttribute = PCInfoReader.CheckSQLServerProcessRunning()

                'SQL Server Service
                IsSQLServerServiceActiveAttribute = PCInfoReader.CheckSQLServerServiceIsRunning(GetCurrentServer())

                'make report
                Dim myReport As String = MakeReport()

                'export report
                If pExport Then
                    myResponse = ExportFile(myReport)
                End If

                Return myReport

                'Exit Try

                ''***********************************************************************************************
                ''Not used yet but they work!:

                ''processor
                'myResponse = PCInfoReader.GetProcessorInfo()
                'If Not myResponse.HasError And myResponse IsNot Nothing Then
                '    Me.ProcessorsInfo = CType(myResponse.SetDatos, List(Of AX00PCProcessorInfo))
                'End If

                ''BaseBoard
                'myResponse = PCInfoReader.GetBaseBoardInfo()
                'If Not myResponse.HasError And myResponse IsNot Nothing Then
                '    Me.MotherBoardInfo = CType(myResponse.SetDatos, AX00PCBaseBoardInfo)
                'End If

                ''video
                'myResponse = PCInfoReader.GetGraphicsInfo
                'If Not myResponse.HasError And myResponse IsNot Nothing Then
                '    Me.VideoControllerInfo = CType(myResponse.SetDatos, AX00PCGraphicsInfo)
                'End If

                ''network connection (NOK)
                'myResponse = PCInfoReader.GetNetworkConnectionInfo
                'If Not myResponse.HasError And myResponse IsNot Nothing Then
                '    Me.NetworkConnInfo = CType(myResponse.SetDatos, AX00PCNetworkConnectionInfo)
                'End If

                ''Parallel Port
                'myResponse = PCInfoReader.GetParallelPortsInfo
                'If Not myResponse.HasError And myResponse IsNot Nothing Then
                '    Me.ParallelPortsInfo = CType(myResponse.SetDatos, List(Of AX00PCParallelPortInfo))
                'End If

                ''USB controller
                'myResponse = PCInfoReader.GetUSBPortsInfo
                'If Not myResponse.HasError And myResponse IsNot Nothing Then
                '    Me.USBControllerInfo = CType(myResponse.SetDatos, List(Of AX00PCUSBInfo))
                'End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PCInfoReader.GetSystemInfo", EventLogEntryType.Error, False)
                Return ""
            End Try

        End Function




#End Region

#Region "Private"

        ''' <summary>
        ''' Reads the current database server name from App.Config
        ''' </summary>
        ''' <returns>CurrentDBServerName</returns>
        ''' <remarks>
        ''' Created by: RH 12/11/2010
        ''' </remarks>
        Private Shared Function GetCurrentServer() As String
            Dim myConnectionString As String
            Dim CurrentServer As String = String.Empty

            Try
                'myConnectionString = ConfigurationManager.ConnectionStrings("BiosystemsConn").ConnectionString
                'TR 25/01/2011 -Replace by corresponding value on global base.
                myConnectionString = GlobalBase.BioSystemsDBConn

                'validate if connection string is empty to send an error
                If String.IsNullOrEmpty(myConnectionString) Then
                    Dim myLogAcciones As New ApplicationLogManager()
                    GlobalBase.CreateLogActivity("Error Reading the Connection String.", "PCInfoReader.GetCurrentServer()", EventLogEntryType.Error, False)
                Else
                    Using connection As New System.Data.SqlClient.SqlConnection(myConnectionString)
                        CurrentServer = connection.DataSource
                    End Using
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PCInfoReader.GetCurrentServer()", EventLogEntryType.Error, False)

            End Try

            Return CurrentServer
        End Function

        ''' <summary>
        ''' Performs the exporting operation
        ''' </summary>
        ''' <returns>The resultant report as string</returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Private Shared Function MakeReport() As String

            Dim myReport As String = "*********COMPUTER & OPERATIVE SYSTEM CONFIGURATION DATA ***********************" & vbCrLf
            myReport &= vbCrLf
            myReport &= "Date UTC:" & vbTab & Now.ToUniversalTime.ToString & vbCrLf
            myReport &= vbCrLf
            myReport &= vbCrLf

            Try
                myReport &= "PROCESSOR INFO" & vbCrLf
                myReport &= "*****************************************************************************************" & vbCrLf
                myReport &= ProcessorInfoAtt & Environment.NewLine

                'computer info
                With ComputerInfoAttr
                    myReport &= "COMPUTER:" & vbCrLf
                    myReport &= "*****************************************************************************************" & vbCrLf
                    myReport &= vbTab & "Name:" & vbTab & vbTab & vbTab & vbTab & .Name & vbCrLf
                    myReport &= vbTab & "Domain:" & vbTab & vbTab & vbTab & vbTab & .Domain & vbCrLf
                    myReport &= vbTab & "System Type:" & vbTab & vbTab & vbTab & .SystemType & vbCrLf
                    myReport &= vbTab & "Physical RAM Memory:" & vbTab & vbTab & BytesToMegabytes(.AvailablePhysicalRAM) & "MB available/" & BytesToMegabytes(.TotalPhysicalRAM) & "MB" & vbCrLf
                    myReport &= vbTab & "Virtual RAM Memory:" & vbTab & vbTab & BytesToMegabytes(.AvailableVirtualRAM) & "MB available/" & BytesToMegabytes(.TotalVirtualRAM) & "MB" & vbCrLf
                End With

                myReport &= vbCrLf

                With OperativeSystemInfoAttr
                    myReport &= "OPERATIVE SYSTEM:" & vbCrLf
                    myReport &= "*****************************************************************************************" & vbCrLf
                    myReport &= vbTab & "Name:" & vbTab & vbTab & vbTab & vbTab & .Name & vbCrLf
                    myReport &= vbTab & "Version:" & vbTab & vbTab & vbTab & .Version & vbCrLf
                    myReport &= vbTab & "TimeStamp:" & vbTab & vbTab & vbTab & ConvertMachineDateTime(.LocalDateTime) & vbCrLf
                    myReport &= vbTab & "Made by:" & vbTab & vbTab & vbTab & .Manufacturer & vbCrLf
                    myReport &= vbTab & "Serial:" & vbTab & vbTab & vbTab & vbTab & .SerialNumber & vbCrLf
                    myReport &= vbTab & "TimeZone:" & vbTab & vbTab & vbTab & .CurrentTimeZone & vbCrLf
                    myReport &= vbTab & "Language:" & vbTab & vbTab & vbTab & .OSLanguage & vbCrLf
                    myReport &= vbTab & "Organization:" & vbTab & vbTab & vbTab & .Organization & vbCrLf
                    myReport &= vbTab & "Users:" & vbTab & vbTab & vbTab & vbTab & .NumberOfUsers & vbCrLf

                End With

                myReport &= vbCrLf

                'With OSCultureInfoAttr
                '    myReport &= "CULTURE SETTINGS:" & vbCrLf
                '    myReport &= "*****************************************************************************************" & vbCrLf
                '    myReport &= vbTab & "Date Format:" & vbTab & vbTab & vbTab & .DateFormat & vbCrLf
                '    myReport &= vbTab & "Time Format:" & vbTab & vbTab & vbTab & .TimeFormat & vbCrLf
                '    myReport &= vbTab & "Date Separator:" & vbTab & vbTab & vbTab & .DateSeparator & vbCrLf
                '    myReport &= vbTab & "Decimal Separator:" & vbTab & vbTab & .DecimalSeparator & vbCrLf
                '    myReport &= vbTab & "Group Separator:" & vbTab & vbTab & .GroupSeparator & vbCrLf
                '    myReport &= vbTab & "Time Separator:" & vbTab & vbTab & vbTab & .TimeSeparator & vbCrLf
                '    myReport &= vbTab & "Language:" & vbTab & vbTab & vbTab & .Language & vbCrLf
                'End With

                myReport &= "CULTURE SETTINGS:" & vbCrLf
                myReport &= "*****************************************************************************************" & vbCrLf
                myReport &= vbTab & "Date Format:" & vbTab & vbTab & vbTab & SystemInfoManager.OSDateFormat & vbCrLf
                myReport &= vbTab & "Time Format:" & vbTab & vbTab & vbTab & SystemInfoManager.OSLongTimeFormat & vbCrLf
                myReport &= vbTab & "Date Separator:" & vbTab & vbTab & vbTab & SystemInfoManager.OSDateSeparator & vbCrLf
                myReport &= vbTab & "Decimal Separator:" & vbTab & vbTab & SystemInfoManager.OSDecimalSeparator & vbCrLf
                myReport &= vbTab & "Group Separator:" & vbTab & vbTab & SystemInfoManager.OSGroupSeparator & vbCrLf
                myReport &= vbTab & "Time Separator:" & vbTab & vbTab & vbTab & SystemInfoManager.OSTimeSeparator & vbCrLf
                myReport &= vbTab & "Language:" & vbTab & vbTab & vbTab & SystemInfoManager.OSLanguage & vbCrLf

                myReport &= vbCrLf

                myReport &= "INSTALLED APPLICATIONS:" & vbCrLf
                myReport &= "*****************************************************************************************" & vbCrLf
                For Each s As String In InstalledApplicationsAttr
                    myReport &= vbTab & s & vbCrLf
                Next

                myReport &= vbCrLf

                myReport &= "INSTALLED SERVICES:" & vbCrLf
                myReport &= "*****************************************************************************************" & vbCrLf
                For Each s As AX00PCServiceInfo In InstalledServicesAttr
                    myReport &= vbTab & ">" & s.Name & " (" & s.Caption & ")" & vbCrLf
                    myReport &= vbTab & vbTab & "Started:" & s.Started & "," & s.State & vbTab & "Mode:" & s.StartMode & vbTab & "Type:" & s.ServiceType & vbTab & "Status:" & s.Status & vbTab & "Logon:" & s.StartName & vbCrLf
                    myReport &= vbCrLf
                Next

                myReport &= vbCrLf

                myReport &= "USER ACCOUNTS:" & vbCrLf
                myReport &= "*****************************************************************************************" & vbCrLf

                For Each u As AX00PCUserInfo In UsersInfoAttr
                    If u.FullName <> String.Empty Then
                        myReport &= vbTab & ">" & u.FullName.ToString & vbCrLf
                    Else
                        myReport &= vbTab & ">" & "System" & vbCrLf
                    End If
                    myReport &= vbTab & "Caption:" & vbTab & vbTab & vbTab & u.Caption & vbCrLf
                    myReport &= vbTab & "Description:" & vbTab & vbTab & vbTab & u.Description & vbCrLf
                    myReport &= vbTab & "Domain:" & vbTab & vbTab & vbTab & vbTab & u.Domain & vbCrLf
                    myReport &= vbTab & "Local Account:" & vbTab & vbTab & vbTab & u.LocalAccount & vbCrLf
                    myReport &= vbTab & "Status:" & vbTab & vbTab & vbTab & vbTab & u.Status & vbCrLf
                    myReport &= vbTab & "Lockout:" & vbTab & vbTab & vbTab & u.LockOut & vbCrLf
                    myReport &= vbTab & "-----------------------------------------------------------------------------------" & vbCrLf
                Next

                myReport &= vbCrLf

                'DL 13/07/2011
                If Not ServersInfoAttr Is Nothing Then
                    myReport &= "SERVERS:" & vbCrLf
                    myReport &= "*****************************************************************************************" & vbCrLf
                    For Each s As AX00PCServerInfo In ServersInfoAttr
                        myReport &= vbTab & ">" & s.Name & vbTab & vbTab & vbTab & s.Instance & vbCrLf
                    Next

                    myReport &= vbCrLf
                    myReport &= vbCrLf
                End If
                'END DL 13/07/2011

                myReport &= "*****************************************************************************************" & vbCrLf
                myReport &= "REQUIREMENTS:" & vbCrLf
                myReport &= "*****************************************************************************************" & vbCrLf
                myReport &= vbTab & "SQL SERVER 2005 EXPRESS INSTALLED:" & vbTab & IsSQLServerInstalledAttribute & vbCrLf
                myReport &= vbTab & "SQL SERVER PROCESS RUNNING:" & vbTab & vbTab & IsSQLServerProcessRunningAttribute & vbCrLf
                myReport &= vbTab & "SQL SERVER SERVICE ACTIVE:" & vbTab & vbTab & IsSQLServerServiceActiveAttribute & vbCrLf

                myReport &= vbCrLf
                myReport &= vbCrLf

                myReport &= "*****************************************************************************************" & vbCrLf
                myReport &= "HARDWARE:" & vbCrLf
                myReport &= "*****************************************************************************************" & vbCrLf

                myReport &= vbCrLf

                myReport &= "HARD DISKS:" & vbCrLf
                myReport &= "*****************************************************************************************" & vbCrLf

                For Each d As AX00PCDiskDriveInfo In DrivesInfoAttr
                    myReport &= vbTab & "Name:" & vbTab & vbTab & vbTab & vbTab & d.Caption & vbCrLf
                    myReport &= vbTab & "Description:" & vbTab & vbTab & vbTab & d.Description & vbCrLf
                    myReport &= vbTab & "Model:" & vbTab & vbTab & vbTab & vbTab & d.Model & vbCrLf
                    myReport &= vbTab & "Size:" & vbTab & vbTab & vbTab & vbTab & d.Size & " Bytes = " & BytesToMegabytes(d.Size) & " MB" & vbCrLf
                    myReport &= vbTab & "Status:" & vbTab & vbTab & vbTab & vbTab & d.Status & vbCrLf
                    myReport &= vbTab & "Type:" & vbTab & vbTab & vbTab & vbTab & d.InterfaceType & vbCrLf
                    myReport &= vbTab & "Loaded:" & vbTab & vbTab & vbTab & vbTab & d.MediaLoaded & vbCrLf
                    myReport &= vbTab & "Media Type:" & vbTab & vbTab & vbTab & d.MediaType & vbCrLf
                    myReport &= vbTab & "Partitions:" & vbTab & vbTab & vbTab & d.Partitions & vbCrLf
                    myReport &= vbTab & "Sectors:" & vbTab & vbTab & vbTab & d.TotalSectors & vbCrLf
                    myReport &= vbTab & "Tracks:" & vbTab & vbTab & vbTab & vbTab & d.TotalTracks & vbCrLf
                    myReport &= vbTab & "Cylinders:" & vbTab & vbTab & vbTab & d.TotalCylinders & vbCrLf
                    myReport &= vbTab & "Heads:" & vbTab & vbTab & vbTab & vbTab & d.TotalHeads & vbCrLf
                    myReport &= vbTab & "-----------------------------------------------------------------------------------" & vbCrLf
                Next

                myReport &= vbCrLf

                myReport &= "PRINTERS:" & vbCrLf
                myReport &= "*****************************************************************************************" & vbCrLf

                If Not PrintersInfoAttr Is Nothing Then
                    For Each p As AX00PCPrinterInfo In PrintersInfoAttr
                        myReport &= vbTab & "Name:" & vbTab & vbTab & vbTab & vbTab & p.Name & vbCrLf
                        myReport &= vbTab & "Description:" & vbTab & vbTab & vbTab & p.Description & vbCrLf
                        myReport &= vbTab & "Status:" & vbTab & vbTab & vbTab & vbTab & p.Status & vbCrLf
                        myReport &= vbTab & "Is Local:" & vbTab & vbTab & vbTab & p.Local & vbCrLf
                        myReport &= vbTab & "In Network:" & vbTab & vbTab & vbTab & p.Network & vbCrLf
                        myReport &= vbTab & "Is Hidden:" & vbTab & vbTab & vbTab & p.Hidden & vbCrLf
                        myReport &= vbTab & "Driver:" & vbTab & vbTab & vbTab & vbTab & p.DriverName & vbCrLf
                        myReport &= vbTab & "Port:" & vbTab & vbTab & vbTab & vbTab & p.PortName & vbCrLf
                        myReport &= vbTab & "-----------------------------------------------------------------------------------" & vbCrLf
                    Next p
                End If

                myReport &= vbCrLf

                myReport &= "SERIAL PORTS:" & vbCrLf
                myReport &= "*****************************************************************************************" & vbCrLf

                For Each p As AX00PCSerialPortInfo In SerialPortsInfoAttr
                    myReport &= vbTab & ">" & p.Name & vbCrLf
                    myReport &= vbTab & "Description:" & vbTab & vbTab & vbTab & p.Description & vbCrLf
                    myReport &= vbTab & "DeviceID:" & vbTab & vbTab & vbTab & p.DeviceID & vbCrLf
                    myReport &= vbTab & "MaxBaudRate:" & vbTab & vbTab & vbTab & p.MaxBaudRate & vbCrLf
                    myReport &= vbTab & "MaxInputBufferSize:" & vbTab & vbTab & p.MaximumInputBufferSize & vbCrLf
                    myReport &= vbTab & "MaxOutputBufferSize:" & vbTab & vbTab & p.MaximumOutputBufferSize & vbCrLf
                    myReport &= vbTab & "Autodetected:" & vbTab & vbTab & vbTab & p.OSAutoDiscovered & vbCrLf
                    myReport &= vbTab & "Provider Type:" & vbTab & vbTab & vbTab & p.ProviderType & vbCrLf
                    'myReport &= vbTab & "SettableBaudRate:" & vbTab & vbTab & vbTab & vbTab & p.SettableBaudRate & vbCrLf
                    'myReport &= vbTab & "SettableDataBits:" & vbTab & vbTab & vbTab & vbTab & p.SettableDataBits & vbCrLf
                    'myReport &= vbTab & "SettableFlowControl:" & vbTab & vbTab & vbTab & vbTab & p.SettableFlowControl & vbCrLf
                    'myReport &= vbTab & "SettableParity:" & vbTab & vbTab & vbTab & vbTab & p.SettableParity & vbCrLf
                    'myReport &= vbTab & "SettableParityCheck:" & vbTab & vbTab & vbTab & vbTab & p.SettableParityCheck & vbCrLf
                    'myReport &= vbTab & "SettableRLSD:" & vbTab & vbTab & vbTab & vbTab & p.SettableRLSD & vbCrLf
                    'myReport &= vbTab & "Supports16BitMode:" & vbTab & vbTab & vbTab & vbTab & p.Supports16BitMode & vbCrLf
                    'myReport &= vbTab & "SupportsDTRDSR:" & vbTab & vbTab & vbTab & vbTab & p.SupportsDTRDSR & vbCrLf
                    'myReport &= vbTab & "SupportsElapsedTimeouts:" & vbTab & vbTab & vbTab & vbTab & p.SupportsElapsedTimeouts & vbCrLf
                    'myReport &= vbTab & "SupportsIntTimeouts:" & vbTab & vbTab & vbTab & vbTab & p.SupportsIntTimeouts & vbCrLf
                    'myReport &= vbTab & "SupportsParityCheck:" & vbTab & vbTab & vbTab & vbTab & p.SupportsParityCheck & vbCrLf
                    'myReport &= vbTab & "SupportsRLSD:" & vbTab & vbTab & vbTab & vbTab & p.SupportsRLSD & vbCrLf
                    'myReport &= vbTab & "SupportsRTSCTS:" & vbTab & vbTab & vbTab & vbTab & p.SupportsRTSCTS & vbCrLf
                    'myReport &= vbTab & "SupportsSpecialCharacters:" & vbTab & vbTab & vbTab & vbTab & p.SupportsSpecialCharacters & vbCrLf
                    'myReport &= vbTab & "SupportsXOnXOff:" & vbTab & vbTab & vbTab & vbTab & p.SupportsXOnXOff & vbCrLf
                    'myReport &= vbTab & "SupportsXOnXOffSet:" & vbTab & vbTab & vbTab & vbTab & p.SupportsXOnXOffSet & vbCrLf
                    myReport &= vbTab & "-----------------------------------------------------------------------------------" & vbCrLf
                Next


                'NOTE it can be added much more info
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
                Return ""
            End Try

            Return myReport

        End Function

        ''' <summary>
        ''' Exports the formatted test data to an external file
        ''' add to the appconfig "<add key="PCOSInfoFilePath" value=""/>"
        ''' </summary>
        ''' <param name="pData"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 07/10/10</remarks>
        Private Shared Function ExportFile(ByVal pData As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim TextFileWriter As StreamWriter = Nothing
            Try

                'Dim myExportFolder As String = ConfigurationManager.AppSettings("PCOSInfoFilePath").ToString()

                'TR 25/01/2011 -Replace by corresponding value on global base.
                Dim myExportFolder As String = GlobalBase.PCOSInfoFilePath
                If myExportFolder.StartsWith("\") And Not myExportFolder.StartsWith("\\") Then
                    myExportFolder = Application.StartupPath & myExportFolder
                End If

                Dim fileId As Integer = 1
                Dim myExportFile As String = "AX00PCOSInfo.txt"

                'validate path exist.
                If Not File.Exists(myExportFolder & myExportFile) Then
                    If Not Directory.Exists(myExportFolder) And _
                    Not myExportFolder.Trim = "" Then
                        Directory.CreateDirectory(myExportFolder)
                    End If

                    'if the file do not exist then create a new one.
                    TextFileWriter = New StreamWriter(myExportFolder & myExportFile)
                Else
                    'Set the file to a Stream Writer
                    TextFileWriter = New StreamWriter(myExportFolder & myExportFile)
                    TextFileWriter.Write(String.Empty)
                End If

                TextFileWriter.Write(pData)

                'TR 02/11/2011 -Save the msinfo32 report as extra information related to the PC.
                'myGlobalDataTO = SaveMSInfo32(myExportFolder) Commented PENDING WHERE TO IMPLEMENT.
                'TR 02/11/2011 -END.

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "PCInfoReader.ExportFile", EventLogEntryType.Error, False)
            Finally
                'close the file 
                TextFileWriter.Close()
            End Try
            Return myGlobalDataTO
        End Function


#End Region

#Region "Utilities"

        Private Shared Function ConvertMachineDateTime(ByVal pMachinneDT As String) As String
            Try
                Dim MT As String = pMachinneDT
                Dim myYear As String = MT.Substring(0, 4)
                MT = MT.Substring(4)
                Dim myMonth As String = MT.Substring(0, 2)
                MT = MT.Substring(2)
                Dim myDay As String = MT.Substring(0, 2)
                MT = MT.Substring(2)
                Dim myHour As String = MT.Substring(0, 2)
                MT = MT.Substring(2)
                Dim myMinutes As String = MT.Substring(0, 2)
                MT = MT.Substring(2)
                Dim mySeconds As String = MT.Substring(0, 2)

                Dim myDate As String = myYear & "/" & myMonth & "/" & myDay & " " & myHour & ":" & myMinutes & ":" & mySeconds
                Return myDate

            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Private Shared Function BytesToMegabytes(ByVal Bytes As Double) As Double
            Dim dblAns As Double
            dblAns = (Bytes / 1024) / 1024
            BytesToMegabytes = CDbl(Format(dblAns, "###,###,##0.00"))
            Return BytesToMegabytes
        End Function

#End Region

#Region "Static/Shared"

        ''' <summary>
        ''' Get the processor information. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 02/11/2011</remarks>
        Public Shared Function GetProcessorInfo() As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim strProcessorId As String = String.Empty
                Dim query As New SelectQuery("Win32_processor")
                Dim search As New ManagementObjectSearcher(query)
                Dim info As ManagementObject

                For Each info In search.Get()
                    strProcessorId = vbTab & "Name: " & vbTab & vbTab & vbTab & vbTab & info("Name").ToString() & Environment.NewLine
                    strProcessorId &= vbTab & "Clock Speed:" & vbTab & vbTab & vbTab & info("CurrentClockSpeed").ToString() & " GHz" & Environment.NewLine
                    strProcessorId &= vbTab & "Number of Cores:" & vbTab & vbTab & info("NumberOfCores").ToString() & Environment.NewLine
                Next

                myGlobalDataTO.SetDatos = strProcessorId

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try

            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Create a report generate from the MSinfo32 Windows library
        ''' </summary>
        ''' <param name="pExportFolder"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveMSInfo32(ByVal pExportFolder As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Process.Start("msinfo32", "/report" & " " & """" & pExportFolder & My.Settings.MSInfo32ReportName & """")
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Gets the information about Computer
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Shared Function GetComputerInfo() As GlobalDataTO
            Dim myResponse As New GlobalDataTO

            Try
                Dim objMSO As New ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem")
                Dim objMgmt As ManagementObject
                Dim myComputer As New AX00PCComputerInfo
                For Each objMgmt In objMSO.Get

                    With myComputer
                        If objMgmt("name") IsNot Nothing Then
                            .Name = objMgmt("name").ToString()
                        End If
                        If objMgmt("manufacturer") IsNot Nothing Then
                            .Manufacturer = objMgmt("manufacturer").ToString()
                        End If
                        If objMgmt("model") IsNot Nothing Then
                            .Model = objMgmt("model").ToString()
                        End If
                        If objMgmt("systemtype") IsNot Nothing Then
                            .SystemType = objMgmt("systemtype").ToString
                        End If
                        If objMgmt("Domain") IsNot Nothing Then
                            .Domain = objMgmt("Domain").ToString
                        End If
                    End With
                Next

                Dim a As New Devices.Computer


                Dim computer_info As New Devices.ComputerInfo
                With myComputer
                    .OSFullname = CStr(computer_info.OSFullName)
                    .OSPlatForm = CStr(computer_info.OSPlatform)
                    .OSVersion = CStr(computer_info.OSVersion)
                    .AvailablePhysicalRAM = CLng(computer_info.AvailablePhysicalMemory)
                    .AvailableVirtualRAM = CLng(computer_info.AvailableVirtualMemory)
                    .TotalPhysicalRAM = CLng(computer_info.TotalPhysicalMemory)
                    .TotalVirtualRAM = CLng(computer_info.TotalVirtualMemory)
                End With

                myResponse.SetDatos = myComputer

            Catch ex As Exception
                myResponse.HasError = True
                myResponse.ErrorCode = "SYSTEM_ERROR"
                myResponse.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return myResponse
        End Function

        ''' <summary>
        ''' Gets the information about the operative system
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Shared Function GetOSInfo() As GlobalDataTO

            Dim myResponse As New GlobalDataTO

            Try
                Dim objMOS As New ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")
                Dim objMgmt As ManagementObject
                Dim myOS As New AX00PCOSInfo
                For Each objMgmt In objMOS.Get

                    With myOS
                        If objMgmt("name") IsNot Nothing Then
                            .Name = objMgmt("name").ToString()
                        End If
                        If objMgmt("version") IsNot Nothing Then
                            .Version = objMgmt("version").ToString() & " " & Environment.OSVersion.ServicePack
                        End If
                        If objMgmt("csname") IsNot Nothing Then
                            .ComputerName = objMgmt("csname").ToString()
                        End If
                        If objMgmt("windowsdirectory") IsNot Nothing Then
                            .WindowsDirectory = objMgmt("windowsdirectory").ToString()
                        End If
                        If objMgmt("Caption") IsNot Nothing Then
                            .Caption = objMgmt("Caption").ToString()
                        End If
                        If objMgmt("CurrentTimeZone") IsNot Nothing Then
                            .CurrentTimeZone = CInt(objMgmt("CurrentTimeZone"))
                        End If
                        If objMgmt("LocalDateTime") IsNot Nothing Then
                            .LocalDateTime = objMgmt("LocalDateTime").ToString()
                        End If
                        If objMgmt("Manufacturer") IsNot Nothing Then
                            .Manufacturer = objMgmt("Manufacturer").ToString()
                        End If
                        If objMgmt("NumberOfUsers") IsNot Nothing Then
                            .NumberOfUsers = objMgmt("NumberOfUsers").ToString()
                        End If
                        If objMgmt("Organization") IsNot Nothing Then
                            .Organization = objMgmt("Organization").ToString()
                        End If
                        If objMgmt("OSLanguage") IsNot Nothing Then
                            .OSLanguage = CInt(objMgmt("OSLanguage"))
                        End If
                        If objMgmt("RegisteredUser") IsNot Nothing Then
                            .RegisteredUser = objMgmt("RegisteredUser").ToString()
                        End If
                        If objMgmt("SerialNumber") IsNot Nothing Then
                            .SerialNumber = objMgmt("SerialNumber").ToString()
                        End If
                        If objMgmt("LastBootUpTime") IsNot Nothing Then
                            .LastBootUpTime = objMgmt("LastBootUpTime").ToString
                        End If
                    End With
                Next

                myResponse.SetDatos = myOS

            Catch ex As Exception
                myResponse.HasError = True
                myResponse.ErrorCode = "SYSTEM_ERROR"
                myResponse.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return myResponse

        End Function

        '''' <summary>
        '''' Gets the information about the OS culture settings
        '''' </summary>
        '''' <returns></returns>
        '''' <remarks>Created by SG 08/10/10</remarks>
        'Public Shared Function GetOSCultureInfo() As GlobalDataTO

        '    Dim myResponse As New GlobalDataTO

        '    Try
        '        Dim myOSCultureInfo As New AX00PCOSCultureInfo
        '        With myOSCultureInfo
        '            .DecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator ' get the decimal Separator
        '            .GroupSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator ' get the group separtator
        '            .DateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern ' get the short pattern for date
        '            .TimeFormat = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern ' get the time format with seconds
        '            .TimeSeparator = CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator 'get the time separator 
        '            .DateSeparator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator 'get the date separator
        '            .Language = CultureInfo.CurrentCulture.DisplayName ' get the display language from system.

        '            'Get the Time Format removing seconds
        '            Dim timeComponents() As String = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.Split(CChar(.TimeSeparator))
        '            If (timeComponents.Count >= 2) Then
        '                .TimeFormatShort = timeComponents(0).Trim & .TimeSeparator & timeComponents(1).Trim
        '                If (timeComponents.Count = 3) Then
        '                    If (timeComponents(2).Substring(timeComponents(2).Trim.Length - 2) = "tt") Then
        '                        .TimeFormatShort &= " tt"   'Time is in format AM/PM
        '                    End If
        '                End If
        '            End If
        '        End With

        '        myResponse.SetDatos = myOSCultureInfo

        '    Catch ex As EvaluateException
        '        myResponse.HasError = True
        '        myResponse.ErrorCode = "SYSTEM_ERROR"
        '        myResponse.ErrorMessage = ex.Message
        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
        '    End Try

        '    Return myResponse

        'End Function

        ''' <summary>
        ''' Gets a list of installed applications
        ''' </summary>
        ''' <param name="pComputerName"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Shared Function GetInstalledApplications(ByVal pComputerName As String) As GlobalDataTO
            Dim myResponse As New GlobalDataTO

            Try
                Dim lbResults As New List(Of String)
                Dim strComputerName As String = pComputerName
                Dim strStringValue As String = ""
                Dim strKeyPath As String = "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\"
                Dim objManagementScope As ManagementScope
                Dim objManagementClass As ManagementClass
                Dim objManagementBaseObject As ManagementBaseObject
                Dim strkey As String
                Dim myKey As String
                Dim myValue As String
                Dim aSubKeys() As String

                objManagementScope = New ManagementScope
                objManagementScope.Path.Server = strComputerName
                objManagementScope.Path.NamespacePath = "root\default"
                objManagementScope.Options.EnablePrivileges = True
                objManagementScope.Options.Impersonation = ImpersonationLevel.Impersonate
                objManagementScope.Connect()


                objManagementClass = New ManagementClass("stdRegProv")
                objManagementClass.Scope = objManagementScope
                objManagementBaseObject = objManagementClass.GetMethodParameters("EnumKey")
                objManagementBaseObject.SetPropertyValue("hDefKey", CType("&H" & Hex(RegistryHive.LocalMachine), Long))
                objManagementBaseObject.SetPropertyValue("sSubKeyName", strKeyPath)
                aSubKeys = CType(objManagementClass.InvokeMethod("EnumKey", objManagementBaseObject, Nothing).Properties.Item("sNames").Value, String())



                For Each strkey In aSubKeys
                    myKey = strkey
                    objManagementBaseObject = objManagementClass.GetMethodParameters("GetStringValue")
                    objManagementBaseObject.SetPropertyValue("hDefKey", CType("&H" & Hex(RegistryHive.LocalMachine), Long))
                    objManagementBaseObject.SetPropertyValue("sSubKeyName", strKeyPath & strkey)
                    objManagementBaseObject.SetPropertyValue("sValueName", "DisplayName")
                    objManagementBaseObject = objManagementClass.InvokeMethod("GetStringValue", objManagementBaseObject, Nothing)

                    If objManagementBaseObject("sValue") IsNot Nothing Then
                        myValue = objManagementBaseObject("sValue").ToString
                        If Not myValue = "" Then
                            lbResults.Add(myValue)
                        End If
                    End If
                Next

                myResponse.SetDatos = lbResults.OrderBy(Function(a) a).ToList()

            Catch ex As Exception
                myResponse.HasError = True
                myResponse.ErrorCode = "SYSTEM_ERROR"
                myResponse.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return myResponse

        End Function

        ''' <summary>
        ''' Gets a list of installed services
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Shared Function GetInstalledServices() As GlobalDataTO

            Dim myResponse As New GlobalDataTO

            Try
                Dim class1 As ManagementClass = New ManagementClass("Win32_Service")
                Dim myServices As New List(Of AX00PCServiceInfo)
                For Each ob As ManagementObject In class1.GetInstances
                    Dim MyService As New AX00PCServiceInfo
                    If ob.GetPropertyValue("Name") IsNot Nothing Then
                        With MyService
                            .Name = CType(ob.GetPropertyValue("Name"), String)
                            .Caption = CType(ob.GetPropertyValue("Caption"), String)
                            .Description = CType(ob.GetPropertyValue("Description"), String)
                            .PathName = CType(ob.GetPropertyValue("PathName"), String)
                            .InstallDate = CType(ob.GetPropertyValue("InstallDate"), DateTime)
                            .ServiceType = CType(ob.GetPropertyValue("ServiceType"), String)
                            .Started = CType(ob.GetPropertyValue("Started"), Boolean)
                            .StartMode = CType(ob.GetPropertyValue("StartMode"), String)
                            .StartName = CType(ob.GetPropertyValue("StartName"), String)
                            .State = CType(ob.GetPropertyValue("State"), String)
                            .Status = CType(ob.GetPropertyValue("Status"), String)
                        End With
                    End If
                    myServices.Add(MyService)
                Next

                myResponse.SetDatos = myServices

            Catch ex As Exception
                myResponse.HasError = True
                myResponse.ErrorCode = "SYSTEM_ERROR"
                myResponse.ErrorMessage = ex.Message
            End Try

            Return myResponse

        End Function

        ''' <summary>
        ''' Checks if SQL Server process is currently running
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  SG 08/10/10
        ''' Modified by XB 01/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
        ''' </remarks>
        Public Shared Function CheckSQLServerProcessRunning() As Boolean
            'RH 12/11/2010
            'Note that now we can have SQL Server Express 2005, 2008 or 2008 R2, and with any instance name so,
            'perhaps this function needs some update
            Try
                For Each P As Process In System.Diagnostics.Process.GetProcesses
                    'If P.ProcessName.ToUpper.Contains("SQLSERVR") Then
                    If P.ProcessName.ToUpperBS.Contains("SQLSERVR") Then
                        Return True
                    End If
                Next
            Catch ex As Exception
                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return False
        End Function

        ''' <summary>
        ''' Checks if the SQL Server service is running
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by  SG 08/10/10
        ''' Modified by XB 01/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
        ''' </remarks>
        Public Shared Function CheckSQLServerServiceIsRunning(ByVal ServerName As String) As Boolean
            Try
                'Dim class1 As ManagementClass = New ManagementClass("Win32_Service")
                'For Each ob As ManagementObject In class1.GetInstances
                '    If ob.GetPropertyValue("Name").ToString = "MSSQL$SQLEXPRESS" Then
                '        Return CBool(ob.GetPropertyValue("State").ToString.ToUpper.Trim = "RUNNING")
                '    End If
                'Next

                'RH 11/11/2010
                'Now we can have SQL Server Express 2005, 2008 or 2008 R2, and with any instance name so,
                'we update this function for meeting the new requirements
                Dim class1 As ManagementClass = New ManagementClass("Win32_Service")

                If ServerName.Contains("\") Then
                    ServerName = ServerName.Substring(ServerName.LastIndexOf("\") + 1, ServerName.Length - ServerName.LastIndexOf("\") - 1)
                End If

                'Dim ServiceName As String = "MSSQL$" & ServerName.ToUpper()
                Dim ServiceName As String = "MSSQL$" & ServerName.ToUpperBS()

                For Each ob As ManagementObject In class1.GetInstances
                    If ob.GetPropertyValue("Name").ToString() = ServiceName Then
                        Return (ob.GetPropertyValue("State").ToString().ToUpper() = "RUNNING")
                    End If
                Next

            Catch ex As Exception
                'Throw ex  'Commented line RH 11/11/2010
                'Do prefer using an empty throw when catching and re-throwing an exception.
                'This is the best way to preserve the exception call stack.
                'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
                'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
                Throw

            End Try

            Return False
        End Function

        ''' <summary>
        ''' Checks if the SQL Server service is Local or Net
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Shared Function IsSQLServerLocal(ByVal ServerName As String) As Boolean
            Try
                'Dim class1 As ManagementClass = New ManagementClass("Win32_Service")
                'For Each ob As ManagementObject In class1.GetInstances
                '    If ob.GetPropertyValue("Name").ToString = "MSSQL$SQLEXPRESS" Then
                '        Return ob.GetPropertyValue("StartName").ToString
                '    End If
                'Next

                'RH 11/11/2010
                'Now we can have SQL Server Express 2005, 2008 or 2008 R2, and with any instance name so,
                'we update this function for meeting the new requirements

                'Server List 
                If ServersInfoAttr Is Nothing OrElse ServersInfoAttr.Count = 0 Then
                    Dim myResponse As GlobalDataTO
                    myResponse = GetSQLServersInfo()
                    If Not myResponse.HasError AndAlso Not myResponse Is Nothing Then
                        ServersInfoAttr = CType(myResponse.SetDatos, List(Of AX00PCServerInfo))
                    End If
                End If

                If Not ServersInfoAttr Is Nothing Then
                    For Each Server As AX00PCServerInfo In ServersInfoAttr
                        If ServerName.Equals(String.Format("{0}\{1}", Server.Name, Server.Instance), StringComparison.OrdinalIgnoreCase) Then
                            Return True
                        End If
                    Next
                End If

            Catch ex As Exception
                'Throw ex  'Commented line RH 11/11/2010
                'Do prefer using an empty throw when catching and re-throwing an exception.
                'This is the best way to preserve the exception call stack.
                'http://msdn.microsoft.com/en-us/library/ms229005(v=VS.90).aspx
                'http://exceptionalcode.wordpress.com/2010/04/13/net-exceptions-throw-ex-is-evil-but-throw-is-not-that-innocent/
                Throw

                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return False
        End Function

        ''' <summary>
        ''' Gets information about the user of the system
        ''' </summary>
        ''' <param name="domain"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Shared Function GetUsersInfo(ByVal domain As String) As GlobalDataTO

            Dim myResponse As New GlobalDataTO

            Try
                Dim objMOS As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_UserAccount") '  WHERE Domain='" & domain & "'")
                Dim objMgmt As ManagementObject
                Dim myUsers As New List(Of AX00PCUserInfo)
                For Each objMgmt In objMOS.Get
                    Dim myUser As New AX00PCUserInfo
                    With myUser
                        If objMgmt("Domain") IsNot Nothing Then
                            .Domain = objMgmt("Domain").ToString()
                        End If
                        If objMgmt("FullName") IsNot Nothing Then
                            .FullName = objMgmt("FullName").ToString()
                        End If
                        If objMgmt("LocalAccount") IsNot Nothing Then
                            .LocalAccount = CBool(objMgmt("LocalAccount"))
                        End If
                        If objMgmt("LockOut") IsNot Nothing Then
                            .LockOut = CBool(objMgmt("LockOut"))
                        End If
                        If objMgmt("AccountType") IsNot Nothing Then
                            .AccountType = objMgmt("AccountType").ToString()
                        End If
                        If objMgmt("Caption") IsNot Nothing Then
                            .Caption = objMgmt("Caption").ToString()
                        End If
                        If objMgmt("Description") IsNot Nothing Then
                            .Description = objMgmt("Description").ToString()
                        End If
                        If objMgmt("Status") IsNot Nothing Then
                            .Status = objMgmt("Status").ToString()
                        End If
                    End With
                    myUsers.Add(myUser)
                Next

                myResponse.SetDatos = myUsers

            Catch ex As Exception
                myResponse.HasError = True
                myResponse.ErrorCode = "SYSTEM_ERROR"
                myResponse.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return myResponse

        End Function

        ''' <summary>
        ''' Gets info about SQL servers
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by SG 08/10/10
        ''' Modified by: RH 11/11/2010 Get local SQL Servers only
        ''' </remarks>
        Private Shared Function GetSQLServersInfo() As GlobalDataTO
            Dim myResponse As New GlobalDataTO

            Try
                Dim hostname As String = System.Net.Dns.GetHostName

                Dim list_inst As SqlDataSourceEnumerator
                list_inst = Sql.SqlDataSourceEnumerator.Instance
                'list_inst.GetDataSources() 'RH 10/12/2010 Remove this line because it is a repeated one

                Dim myServerList As New List(Of AX00PCServerInfo)
                Dim myTable As DataTable = list_inst.GetDataSources()

                For Each dr As DataRow In myTable.Rows
                    Dim myServerInfo As New AX00PCServerInfo
                    With myServerInfo
                        If dr("ServerName") IsNot Nothing Then
                            If hostname.Equals(dr("ServerName").ToString(), StringComparison.OrdinalIgnoreCase) Then
                                .Name = dr("ServerName").ToString

                                If dr("InstanceName") IsNot Nothing Then
                                    .Instance = dr("InstanceName").ToString()
                                End If

                                If dr("Version") IsNot Nothing Then
                                    .Version = dr("Version").ToString()
                                End If

                                myServerList.Add(myServerInfo)
                            End If
                        End If
                    End With
                Next

                myResponse.SetDatos = myServerList

            Catch ex As Exception
                myResponse.HasError = True
                myResponse.ErrorCode = "SYSTEM_ERROR"
                myResponse.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return myResponse

        End Function

        ''' <summary>
        ''' Gets info about installed printers
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Shared Function GetPrintersInfo() As GlobalDataTO

            Dim myResponse As New GlobalDataTO

            Try
                'Use the ObjectQuery to get the list of configured printers
                Dim oquery As ObjectQuery = New ObjectQuery("SELECT * FROM Win32_Printer")
                Dim mosearcher As ManagementObjectSearcher = New ManagementObjectSearcher(oquery)
                Dim moc As ManagementObjectCollection = mosearcher.Get()

                Dim myPrinters As New List(Of AX00PCPrinterInfo)
                For Each mo As ManagementObject In moc
                    Dim myPrinter As New AX00PCPrinterInfo
                    With myPrinter
                        .Name = mo.Item("Name").ToString()
                        '.Attributes = mo.Item("Attributes")
                        .Caption = mo.Item("Caption").ToString()
                        If mo.Item("CurrentCharSet") IsNot Nothing Then
                            .CurrentCharSet = mo.Item("CurrentCharSet").ToString()
                        End If
                        If mo.Item("CurrentLanguage") IsNot Nothing Then
                            .CurrentLanguage = mo.Item("CurrentLanguage").ToString()
                        End If
                        If mo.Item("CurrentMimeType") IsNot Nothing Then
                            .CurrentMimeType = mo.Item("CurrentMimeType").ToString()
                        End If
                        If mo.Item("CurrentNaturalLanguage") IsNot Nothing Then
                            .CurrentNaturalLanguage = mo.Item("CurrentNaturalLanguage").ToString()
                        End If
                        If mo.Item("CurrentPaperType") IsNot Nothing Then
                            .CurrentPaperType = mo.Item("CurrentPaperType").ToString()
                        End If
                        If mo.Item("Description") IsNot Nothing Then
                            .Description = mo.Item("Description").ToString()
                        End If
                        If mo.Item("DeviceID") IsNot Nothing Then
                            .DeviceID = mo.Item("DeviceID").ToString()
                        End If
                        If mo.Item("DriverName") IsNot Nothing Then
                            .DriverName = mo.Item("DriverName").ToString()
                        End If
                        If mo.Item("Hidden") IsNot Nothing Then
                            .Hidden = CBool(mo.Item("Hidden"))
                        End If
                        If mo.Item("Local") IsNot Nothing Then
                            .Local = CBool(mo.Item("Local"))
                        End If
                        If mo.Item("Location") IsNot Nothing Then
                            .Location = mo.Item("Location").ToString()
                        End If
                        If mo.Item("Network") IsNot Nothing Then
                            .Network = CBool(mo.Item("Network"))
                        End If
                        If mo.Item("Local") IsNot Nothing Then
                            .PortName = mo.Item("PortName").ToString()
                        End If
                        If mo.Item("Status") IsNot Nothing Then
                            .Status = mo.Item("Status").ToString()
                        End If
                        If mo.Item("SystemName") IsNot Nothing Then
                            .SystemName = mo.Item("SystemName").ToString()
                        End If
                        If mo.Item("TimeOfLastReset") IsNot Nothing Then
                            .TimeOfLastReset = mo.Item("TimeOfLastReset").ToString()
                        End If
                    End With

                    myPrinters.Add(myPrinter)
                Next

                myResponse.SetDatos = myPrinters

            Catch ex As Exception
                myResponse.HasError = True
                myResponse.ErrorCode = "SYSTEM_ERROR"
                myResponse.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return myResponse

        End Function

        ''' <summary>
        ''' Gets info about drives
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Shared Function GetDiskDrivesInfo() As GlobalDataTO

            Dim myResponse As New GlobalDataTO

            Try
                Dim objMOS As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_DiskDrive")
                Dim objMgmt As ManagementObject
                Dim myDiskDrives As New List(Of AX00PCDiskDriveInfo)
                For Each objMgmt In objMOS.Get
                    Dim myDiskDrive As New AX00PCDiskDriveInfo
                    With myDiskDrive
                        If objMgmt("Name") IsNot Nothing Then
                            .Name = objMgmt("Name").ToString()
                            If objMgmt("Capabilities") IsNot Nothing Then
                                .Capabilities = objMgmt("Capabilities").ToString()
                            End If
                            If objMgmt("Caption") IsNot Nothing Then
                                .Caption = objMgmt("Caption").ToString()
                            End If
                            If objMgmt("Description") IsNot Nothing Then
                                .Description = objMgmt("Description").ToString()
                            End If
                            If objMgmt("DeviceID") IsNot Nothing Then
                                .DeviceID = objMgmt("DeviceID").ToString()
                            End If
                            If objMgmt("Index") IsNot Nothing Then
                                .Index = CInt(objMgmt("Index").ToString())
                            End If
                            If objMgmt("InterfaceType") IsNot Nothing Then
                                .InterfaceType = objMgmt("InterfaceType").ToString()
                            End If
                            If objMgmt("Manufacturer") IsNot Nothing Then
                                .Manufacturer = objMgmt("Manufacturer").ToString()
                            End If
                            If objMgmt("MediaLoaded") IsNot Nothing Then
                                .MediaLoaded = CBool(objMgmt("MediaLoaded"))
                            End If
                            If objMgmt("MediaType") IsNot Nothing Then
                                .MediaType = objMgmt("MediaType").ToString()
                            End If
                            If objMgmt("Model") IsNot Nothing Then
                                .Model = objMgmt("Model").ToString()
                            End If
                            If objMgmt("Partitions") IsNot Nothing Then
                                .Partitions = CInt(objMgmt("Partitions"))
                            End If
                            If objMgmt("PNPDeviceID") IsNot Nothing Then
                                .PNPDeviceID = objMgmt("PNPDeviceID").ToString()
                            End If
                            If objMgmt("Size") IsNot Nothing Then
                                .Size = CLng(objMgmt("Size"))
                            End If
                            If objMgmt("Status") IsNot Nothing Then
                                .Status = objMgmt("Status").ToString()
                            End If
                            If objMgmt("SystemName") IsNot Nothing Then
                                .SystemName = objMgmt("SystemName").ToString()
                            End If
                            If objMgmt("TotalCylinders") IsNot Nothing Then
                                .TotalCylinders = CLng(objMgmt("TotalCylinders"))
                            End If
                            If objMgmt("TotalHeads") IsNot Nothing Then
                                .TotalHeads = CLng(objMgmt("TotalHeads"))
                            End If
                            If objMgmt("TotalSectors") IsNot Nothing Then
                                .TotalSectors = CLng(objMgmt("TotalSectors"))
                            End If
                            If objMgmt("TotalTracks") IsNot Nothing Then
                                .TotalTracks = CLng(objMgmt("TotalTracks"))
                            End If
                        End If
                    End With
                    myDiskDrives.Add(myDiskDrive)
                Next

                myResponse.SetDatos = myDiskDrives

            Catch ex As Exception
                myResponse.HasError = True
                myResponse.ErrorCode = "SYSTEM_ERROR"
                myResponse.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return myResponse

        End Function

        ''' <summary>
        ''' Gets info about Serial Ports
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Shared Function GetSerialPortsInfo() As GlobalDataTO

            Dim myResponse As New GlobalDataTO

            Try
                Dim objMOS As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_SerialPort")
                Dim objMgmt As ManagementObject
                Dim myPorts As New List(Of AX00PCSerialPortInfo)
                For Each objMgmt In objMOS.Get
                    Dim myPort As New AX00PCSerialPortInfo
                    With myPort
                        If objMgmt("Name") IsNot Nothing Then
                            .Name = objMgmt("Name").ToString()
                            If objMgmt("DeviceID") IsNot Nothing Then
                                .DeviceID = objMgmt("DeviceID").ToString()
                            End If
                            If objMgmt("Description") IsNot Nothing Then
                                .Description = objMgmt("Description").ToString()
                            End If
                            If objMgmt("MaxBaudRate") IsNot Nothing Then
                                .MaxBaudRate = objMgmt("MaxBaudRate").ToString()
                            End If
                            If objMgmt("MaximumInputBufferSize") IsNot Nothing Then
                                .MaximumInputBufferSize = objMgmt("MaximumInputBufferSize").ToString()
                            End If
                            If objMgmt("MaximumOutputBufferSize") IsNot Nothing Then
                                .MaximumOutputBufferSize = objMgmt("MaximumOutputBufferSize").ToString()
                            End If
                            If objMgmt("OSAutoDiscovered") IsNot Nothing Then
                                .OSAutoDiscovered = objMgmt("OSAutoDiscovered").ToString()
                            End If
                            If objMgmt("PNPDeviceID") IsNot Nothing Then
                                .PNPDeviceID = objMgmt("PNPDeviceID").ToString()
                            End If
                            If objMgmt("ProviderType") IsNot Nothing Then
                                .ProviderType = objMgmt("ProviderType").ToString()
                            End If
                            If objMgmt("SettableBaudRate") IsNot Nothing Then
                                .SettableBaudRate = CBool(objMgmt("SettableBaudRate"))
                            End If
                            If objMgmt("SettableDataBits") IsNot Nothing Then
                                .SettableDataBits = CBool(objMgmt("SettableDataBits"))
                            End If
                            If objMgmt("SettableFlowControl") IsNot Nothing Then
                                .SettableFlowControl = CBool(objMgmt("SettableFlowControl"))
                            End If
                            If objMgmt("SettableParity") IsNot Nothing Then
                                .SettableParity = CBool(objMgmt("SettableParity"))
                            End If
                            If objMgmt("SettableParityCheck") IsNot Nothing Then
                                .SettableParityCheck = CBool(objMgmt("SettableParityCheck"))
                            End If
                            If objMgmt("SettableRLSD") IsNot Nothing Then
                                .SettableRLSD = CBool(objMgmt("SettableRLSD"))
                            End If
                            If objMgmt("SettableStopBits") IsNot Nothing Then
                                .SettableStopBits = CBool(objMgmt("SettableStopBits"))
                            End If
                            If objMgmt("Supports16BitMode") IsNot Nothing Then
                                .Supports16BitMode = CBool(objMgmt("Supports16BitMode"))
                            End If
                            If objMgmt("SupportsDTRDSR") IsNot Nothing Then
                                .SupportsDTRDSR = CBool(objMgmt("SupportsDTRDSR"))
                            End If
                            If objMgmt("SupportsElapsedTimeouts") IsNot Nothing Then
                                .SupportsElapsedTimeouts = CBool(objMgmt("SupportsElapsedTimeouts"))
                            End If
                            If objMgmt("SupportsIntTimeouts") IsNot Nothing Then
                                .SupportsIntTimeouts = CBool(objMgmt("SupportsIntTimeouts"))
                            End If
                            If objMgmt("SupportsParityCheck") IsNot Nothing Then
                                .SupportsParityCheck = CBool(objMgmt("SupportsParityCheck"))
                            End If
                            If objMgmt("SupportsRLSD") IsNot Nothing Then
                                .SupportsRLSD = CBool(objMgmt("SupportsRLSD"))
                            End If
                            If objMgmt("SupportsRTSCTS") IsNot Nothing Then
                                .SupportsRTSCTS = CBool(objMgmt("SupportsRTSCTS"))
                            End If
                            If objMgmt("SupportsSpecialCharacters") IsNot Nothing Then
                                .SupportsSpecialCharacters = CBool(objMgmt("SupportsSpecialCharacters"))
                            End If
                            If objMgmt("SupportsXOnXOff") IsNot Nothing Then
                                .SupportsXOnXOff = CBool(objMgmt("SupportsXOnXOff"))
                            End If
                            If objMgmt("SupportsXOnXOffSet") IsNot Nothing Then
                                .SupportsXOnXOffSet = CBool(objMgmt("SupportsXOnXOffSet"))
                            End If

                            myPorts.Add(myPort)

                        End If

                    End With
                    myPorts.Add(myPort)
                Next

                myResponse.SetDatos = myPorts

            Catch ex As Exception
                myResponse.HasError = True
                myResponse.ErrorCode = "SYSTEM_ERROR"
                myResponse.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                'GlobalBase.CreateLogActivity(ex.Message, "ApplicationLayer.SearchValidPorts", EventLogEntryType.Error, False)
            End Try

            Return myResponse

        End Function

#End Region


#End Region


    End Class




#Region "FUNCTIONALITY NOT USED YET AND CAN BE ADDED"
#Region "Not used structures"
    ''Not used yet
    'Public Structure AX00PCNetworkConnectionInfo
    '    Public Name As String
    '    Public RemotePath As String
    '    Public ConnectionType As String
    '    Public RemoteName As String
    '    Public Status As String
    '    Public UserName As String
    'End Structure

    'Public Structure AX00PCUSBInfo
    '    Public Caption As String
    '    Public Description As String
    '    Public DeviceID As String
    '    Public Manufacturer As String
    '    Public ProtocolSupported As String
    '    Public Status As String
    'End Structure

    'Public Structure AX00PCParallelPortInfo
    '    Public DeviceID As String
    '    Public Description As String
    '    Public Caption As String
    '    Public Name As String
    '    Public OSAutoDiscovered As String
    '    Public PNPDeviceID As String
    '    Public ProtocolSupported As String
    '    Public SystemName As String
    'End Structure

    'Public Structure AX00PCBaseBoardInfo
    '    Public HostingBoard As String
    '    Public Manufacturer As String
    '    Public PoweredOn As String
    '    Public Product As String
    '    Public SerialNumber As String
    '    Public Version As String
    'End Structure

    'Public Structure AX00PCGraphicsInfo
    '    Public Name As String
    '    Public AdapterDACType As String
    '    Public AdapterRAM As String
    '    Public Caption As String
    '    Public CurrentBitsPerPixel As String
    '    Public DeviceID As String
    '    Public CurrentHorizontalResolution As String
    '    Public CurrentVerticalResolution As String
    'End Structure

    'Public Structure AX00PCProcessorInfo
    '    Public Availability As String
    '    Public Caption As String
    '    Public CpuStatus As String
    '    Public CurrentClockSpeed As String
    '    Public DeviceID As String
    '    Public Level As String
    '    Public Name As String
    '    Public Id As String
    '    Public Type As String
    '    Public SystemName As String
    '    Public Architecture As String
    '    Public NumberOfCores As String
    '    Public NumberOfLogicalProcessors As String
    'End Structure
#End Region
#Region "Not used declarations"
    'not used yet
    'Public ProcessorsInfo As New List(Of AX00PCProcessorInfo)
    'Public MotherBoardInfo As AX00PCBaseBoardInfo
    'Public VideoControllerInfo As AX00PCGraphicsInfo
    'Public NetworkConnInfo As AX00PCNetworkConnectionInfo
    'Public USBControllerInfo As New List(Of AX00PCUSBInfo)
    'Public ParallelPortsInfo As New List(Of AX00PCParallelPortInfo)
#End Region
#Region "Not used methods"
    ''NOT USED YET
    ''*******************************************************************************************************************************
    'Public Shared Function GetGraphicsInfo() As MYDataTO

    '    Dim myResponse As New MYDataTO

    '    Try
    '        Dim objMOS As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_VideoController")
    '        Dim objMgmt As ManagementObject
    '        Dim myGraphics As New MyPCVideoControllerInfo
    '        For Each objMgmt In objMOS.Get
    '            With myGraphics
    '                If objMgmt("Name") IsNot Nothing Then
    '                    .Name = objMgmt("Name").ToString()

    '                    If objMgmt("AdapterDACType") IsNot Nothing Then
    '                        .AdapterDACType = objMgmt("AdapterDACType").ToString()
    '                    End If
    '                    If objMgmt("AdapterRAM") IsNot Nothing Then
    '                        .AdapterRAM = objMgmt("AdapterRAM").ToString()
    '                    End If
    '                    If objMgmt("Caption") IsNot Nothing Then
    '                        .Caption = objMgmt("Caption").ToString()
    '                    End If
    '                    If objMgmt("CurrentBitsPerPixel") IsNot Nothing Then
    '                        .CurrentBitsPerPixel = objMgmt("CurrentBitsPerPixel").ToString()
    '                    End If
    '                    If objMgmt("DeviceID") IsNot Nothing Then
    '                        .DeviceID = objMgmt("DeviceID").ToString()
    '                    End If
    '                    If objMgmt("CurrentHorizontalResolution") IsNot Nothing Then
    '                        .CurrentHorizontalResolution = objMgmt("CurrentHorizontalResolution").ToString()
    '                    End If
    '                    If objMgmt("CurrentVerticalResolution") IsNot Nothing Then
    '                        .CurrentVerticalResolution = objMgmt("CurrentVerticalResolution").ToString()
    '                    End If
    '                End If
    '            End With
    '        Next

    '        myResponse.SetDatos = myGraphics

    '    Catch ex As Exception
    '        myResponse.HasError = True
    '        myResponse.ErrorCode = "SYSTEM_ERROR"
    '        myResponse.ErrorMessage = ex.Message


    '    End Try

    '    Return myResponse

    'End Function

    ''DOES'NT WORK
    'Public Shared Function GetNetworkConnectionInfo() As MYDataTO

    '    Dim myResponse As New MYDataTO

    '    Try
    '        Dim objMOS As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_NetworkConnection")
    '        Dim objMgmt As ManagementObject
    '        Dim myNetInfo As New MyPCNetworkConnectionInfo
    '        For Each objMgmt In objMOS.Get
    '            With myNetInfo
    '                If objMgmt("Name") IsNot Nothing Then
    '                    .Name = objMgmt("Name").ToString()
    '                    If objMgmt("Name") IsNot Nothing Then
    '                        .RemotePath = objMgmt("RemotePath").ToString()
    '                    End If
    '                    If objMgmt("ConnectionType") IsNot Nothing Then
    '                        .ConnectionType = objMgmt("ConnectionType").ToString()
    '                    End If
    '                    If objMgmt("RemoteName") IsNot Nothing Then
    '                        .RemoteName = objMgmt("RemoteName").ToString()
    '                    End If
    '                    If objMgmt("Status") IsNot Nothing Then
    '                        .Status = objMgmt("Status").ToString()
    '                    End If
    '                    If objMgmt("UserName") IsNot Nothing Then
    '                        .UserName = objMgmt("UserName").ToString()
    '                    End If
    '                End If
    '            End With
    '        Next

    '        myResponse.SetDatos = myNetInfo

    '    Catch ex As Exception
    '        myResponse.HasError = True
    '        myResponse.ErrorCode = "SYSTEM_ERROR"
    '        myResponse.ErrorMessage = ex.Message


    '    End Try

    '    Return myResponse

    'End Function

    'Public Shared Function GetUSBControllerInfo() As MYDataTO

    '    Dim myResponse As New MYDataTO

    '    Try
    '        Dim objMOS As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_USBController")
    '        Dim objMgmt As ManagementObject
    '        Dim myUSB As New MyPCUSBControllerInfo
    '        For Each objMgmt In objMOS.Get
    '            With myUSB
    '                If objMgmt("DeviceID") IsNot Nothing Then
    '                    .DeviceID = objMgmt("DeviceID").ToString()
    '                End If
    '                If objMgmt("Caption") IsNot Nothing Then
    '                    .Caption = objMgmt("Caption").ToString()
    '                End If
    '                If objMgmt("Description") IsNot Nothing Then
    '                    .Description = objMgmt("Description").ToString()
    '                End If
    '                If objMgmt("Manufacturer") IsNot Nothing Then
    '                    .Manufacturer = objMgmt("Manufacturer").ToString()
    '                End If
    '                If objMgmt("ProtocolSupported") IsNot Nothing Then
    '                    .ProtocolSupported = objMgmt("ProtocolSupported").ToString()
    '                End If
    '                If objMgmt("Status") IsNot Nothing Then
    '                    .Status = objMgmt("Status").ToString()
    '                End If
    '            End With
    '        Next

    '        myResponse.SetDatos = myUSB

    '    Catch ex As Exception
    '        myResponse.HasError = True
    '        myResponse.ErrorCode = "SYSTEM_ERROR"
    '        myResponse.ErrorMessage = ex.Message


    '    End Try

    '    Return myResponse

    'End Function



    'Public Shared Function GetProcessorInfo() As MYDataTO

    '    Dim myResponse As New MYDataTO

    '    Try
    '        Dim objMOS As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_Processor")
    '        Dim objMgmt As ManagementObject
    '        Dim myProcessors As New List(Of MyPCProcessorInfo)
    '        For Each objMgmt In objMOS.Get
    '            Dim myprocessor As New MyPCProcessorInfo
    '            With myprocessor
    '                If objMgmt("Name") IsNot Nothing Then
    '                    .Name = objMgmt("Name").ToString()

    '                    If objMgmt("Availability") IsNot Nothing Then
    '                        .Availability = objMgmt("Availability").ToString()
    '                    End If
    '                    If objMgmt("Caption") IsNot Nothing Then
    '                        .Caption = objMgmt("Caption").ToString()
    '                    End If
    '                    If objMgmt("CpuStatus") IsNot Nothing Then
    '                        .CpuStatus = objMgmt("CpuStatus").ToString()
    '                    End If
    '                    If objMgmt("CurrentClockSpeed") IsNot Nothing Then
    '                        .CurrentClockSpeed = objMgmt("CurrentClockSpeed").ToString()
    '                    End If
    '                    If objMgmt("DeviceID") IsNot Nothing Then
    '                        .DeviceID = objMgmt("DeviceID").ToString()
    '                    End If
    '                    If objMgmt("Level") IsNot Nothing Then
    '                        .Level = objMgmt("Level").ToString()
    '                    End If
    '                    If objMgmt("ProcessorId") IsNot Nothing Then
    '                        .Id = objMgmt("ProcessorId").ToString()
    '                    End If
    '                    If objMgmt("ProcessorType") IsNot Nothing Then
    '                        .Type = objMgmt("ProcessorType").ToString()
    '                    End If
    '                    If objMgmt("SystemName") IsNot Nothing Then
    '                        .SystemName = objMgmt("SystemName").ToString()
    '                    End If
    '                    If objMgmt("Architecture") IsNot Nothing Then
    '                        .Architecture = objMgmt("Architecture").ToString()
    '                    End If
    '                    If objMgmt("NumberOfCores") IsNot Nothing Then
    '                        .NumberOfCores = objMgmt("NumberOfCores").ToString()
    '                    End If
    '                    If objMgmt("NumberOfLogicalProcessors") IsNot Nothing Then
    '                        .NumberOfLogicalProcessors = objMgmt("NumberOfLogicalProcessors").ToString()
    '                    End If

    '                    myProcessors.Add(myprocessor)

    '                End If
    '            End With

    '        Next

    '        myResponse.SetDatos = myProcessors

    '    Catch ex As Exception
    '        myResponse.HasError = True
    '        myResponse.ErrorCode = "SYSTEM_ERROR"
    '        myResponse.ErrorMessage = ex.Message


    '    End Try

    '    Return myResponse



    'End Function

    'Public Shared Function GetBaseBoardInfo() As MYDataTO

    '    Dim myResponse As New MYDataTO

    '    Try
    '        Dim objMSO As New ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard")
    '        Dim objMgmt As ManagementObject
    '        Dim myBaseBoard As New MyPCMotherBoardInfo
    '        For Each objMgmt In objMSO.Get

    '            With myBaseBoard
    '                If objMgmt("HostingBoard") IsNot Nothing Then
    '                    .HostingBoard = objMgmt("HostingBoard").ToString()
    '                End If
    '                If objMgmt("Manufacturer") IsNot Nothing Then
    '                    .Manufacturer = objMgmt("Manufacturer").ToString()
    '                End If
    '                If objMgmt("PoweredOn") IsNot Nothing Then
    '                    .PoweredOn = objMgmt("PoweredOn").ToString
    '                End If
    '                If objMgmt("Product") IsNot Nothing Then
    '                    .Product = objMgmt("Product").ToString()
    '                End If
    '                If objMgmt("SerialNumber") IsNot Nothing Then
    '                    .SerialNumber = objMgmt("SerialNumber").ToString()
    '                End If
    '                If objMgmt("Version") IsNot Nothing Then
    '                    .Version = objMgmt("Version").ToString()
    '                End If
    '            End With
    '        Next

    '        myResponse.SetDatos = myBaseBoard

    '    Catch ex As Exception
    '        myResponse.HasError = True
    '        myResponse.ErrorCode = "SYSTEM_ERROR"
    '        myResponse.ErrorMessage = ex.Message


    '    End Try

    '    Return myResponse
    'End Function

    'Public Shared Function GetParallelPortsInfo() As MYDataTO

    '    Dim myResponse As New MYDataTO

    '    Try
    '        Dim objMOS As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_ParallelPort")
    '        Dim objMgmt As ManagementObject
    '        Dim myPorts As New List(Of MyPCParallelPortInfo)
    '        For Each objMgmt In objMOS.Get
    '            Dim myPort As New MyPCParallelPortInfo
    '            With myPort
    '                If objMgmt("Name") IsNot Nothing Then
    '                    .Name = objMgmt("Name").ToString()

    '                    If objMgmt("DeviceID") IsNot Nothing Then
    '                        .DeviceID = objMgmt("DeviceID").ToString()
    '                    End If
    '                    If objMgmt("Description") IsNot Nothing Then
    '                        .Description = objMgmt("Description").ToString()
    '                    End If
    '                    If objMgmt("Caption") IsNot Nothing Then
    '                        .Caption = objMgmt("Caption").ToString()
    '                    End If
    '                    If objMgmt("OSAutoDiscovered") IsNot Nothing Then
    '                        .OSAutoDiscovered = objMgmt("OSAutoDiscovered").ToString()
    '                    End If
    '                    If objMgmt("PNPDeviceID") IsNot Nothing Then
    '                        .PNPDeviceID = objMgmt("PNPDeviceID").ToString()
    '                    End If
    '                    If objMgmt("ProtocolSupported") IsNot Nothing Then
    '                        .ProtocolSupported = objMgmt("ProtocolSupported").ToString()
    '                    End If
    '                    If objMgmt("SystemName") IsNot Nothing Then
    '                        .SystemName = objMgmt("SystemName").ToString()
    '                    End If
    '                    myPorts.Add(myPort)
    '                End If
    '            End With

    '        Next

    '        myResponse.SetDatos = myPorts

    '    Catch ex As Exception
    '        myResponse.HasError = True
    '        myResponse.ErrorCode = "SYSTEM_ERROR"
    '        myResponse.ErrorMessage = ex.Message
    '    End Try

    '    Return myResponse

    'End Function

#End Region
#Region "WQL WIN32 Reference"
    '1c3c0ld = "SELECT * FROM Win32_Account";
    '1c3c0ld = "SELECT * FROM Win32_BIOS";
    '1c3c0ld = "SELECT * FROM Win32_BootConfiguration";
    '1c3c0ld = "SELECT * FROM Win32_Bus";
    '1c3c0ld = "SELECT * FROM Win32_CacheMemory";
    '1c3c0ld = "SELECT * FROM Win32_CDROMDrive";
    '1c3c0ld = "SELECT * FROM Win32_ComputerSystem";
    '1c3c0ld = "SELECT * FROM Win32_DesktopMonitor";
    '1c3c0ld = "SELECT * FROM Win32_DeviceMemoryAddress";
    '1c3c0ld = "SELECT * FROM Win32_DiskDrive";
    '1c3c0ld = "SELECT * FROM Win32_DiskPartition";
    '1c3c0ld = "SELECT * FROM Win32_DMAChannel";
    '1c3c0ld = "SELECT * FROM Win32_Environment";
    '1c3c0ld = "SELECT * FROM Win32_Fan";
    '1c3c0ld = "SELECT * FROM Win32_IDEController";
    '1c3c0ld = "SELECT * FROM Win32_IRQResource";
    '1c3c0ld = "SELECT * FROM Win32_Keyboard";
    '1c3c0ld = "SELECT * FROM Win32_LoadOrderGroup";
    '1c3c0ld = "SELECT * FROM Win32_LogicalDisk";
    '1c3c0ld = "SELECT * FROM Win32_LogicalMemoryConfiguration";
    '1c3c0ld = "SELECT * FROM Win32_LogicalProgramGroup";
    '1c3c0ld = "SELECT * FROM Win32_MemoryArray";
    '1c3c0ld = "SELECT * FROM Win32_MemoryDevice";
    '1c3c0ld = "SELECT * FROM Win32_MotherBoardDevice";
    '1c3c0ld = "SELECT * FROM Win32_NetworkAdapter";
    '1c3c0ld = "SELECT * FROM Win32_NetworkConnections";
    '1c3c0ld = "SELECT * FROM Win32_NTEventLogFile";
    '1c3c0ld = "SELECT * FROM Win32_NTLogEvent";
    '1c3c0ld = "SELECT * FROM Win32_OperatingSystem";
    '1c3c0ld = "SELECT * FROM Win32_PCMCIAController";
    '1c3c0ld = "SELECT * FROM Win32_PnPEntity";
    '1c3c0ld = "SELECT * FROM Win32_PointingDevice";
    '1c3c0ld = "SELECT * FROM Win32_PortableBattery";
    '1c3c0ld = "SELECT * FROM Win32_PortResource";
    '1c3c0ld = "SELECT * FROM Win32_POTSModem";
    '1c3c0ld = "SELECT * FROM Win32_Printer";
    '1c3c0ld = "SELECT * FROM Win32_Process";
    '1c3c0ld = "SELECT * FROM Win32_Processor";
    '1c3c0ld = "SELECT * FROM Win32_SCSIController";
    '1c3c0ld = "SELECT * FROM Win32_SerialPort";
    '1c3c0ld = "SELECT * FROM Win32_Service";
    '1c3c0ld = "SELECT * FROM Win32_share";
    '1c3c0ld = "SELECT * FROM Win32_SoundDevice";
    '1c3c0ld = "SELECT * FROM Win32_SystemDriver";
    '1c3c0ld = "SELECT * FROM Win32_SystemUsers";
    '1c3c0ld = "SELECT * FROM Win32_TemperatureProbe";
    '1c3c0ld = "SELECT * FROM Win32_TimeZone";
    '1c3c0ld = "SELECT * FROM Win32_USBController";
    '1c3c0ld = "SELECT * FROM Win32_USBHub";
    '1c3c0ld = "SELECT * FROM Win32_UserAccount";
    '1c3c0ld = "SELECT * FROM Win32_VideoController"
#End Region
#End Region

End Namespace