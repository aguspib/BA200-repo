Option Explicit On
Option Strict On

'THIS CLASS IS NOT USED
  
#Region "Libraries Used by this Class"
Imports System.Management
Imports System
#End Region

Namespace Biosystems.Ax00.Global.TO

    '''AUTOMATICALLY GENERATED CODE  
    ''' 
    Public Class SystemInfoTO

#Region "Fields"

        Private oComputerName As String
        Private oWindowsDirectory As String
        Private oPhysicalmemory As String
        Private oSystemType As String
        Private oName As String
        Private oModel As String
        Private oManufacturer As String
        Private oVersion As String

#End Region

#Region "Properties"

        ' dl 15/09/2010
        Public Property ComputerName() As String
            Get
                Return oComputerName
            End Get

            Set(ByVal Value As String)
                oComputerName = Value
            End Set

        End Property

        ' dl 15/09/2010
        Public Property WindowsDirectory() As String
            Get
                Return oWindowsDirectory
            End Get

            Set(ByVal Value As String)
                oWindowsDirectory = Value
            End Set

        End Property

        ' dl 15/09/2010
        Public Property Physicalmemory() As String
            Get
                Return oPhysicalmemory
            End Get

            Set(ByVal Value As String)
                oPhysicalmemory = Value
            End Set

        End Property

        ' dl 15/09/2010
        Public Property SystemType() As String
            Get
                Return oSystemType
            End Get

            Set(ByVal Value As String)
                oSystemType = Value
            End Set

        End Property

        ' dl 15/09/2010
        Public Property Name() As String
            Get
                Return oName
            End Get

            Set(ByVal Value As String)
                oName = Value
            End Set

        End Property

        ' dl 15/09/2010
        Public Property Model() As String
            Get
                Return oModel
            End Get

            Set(ByVal Value As String)
                oModel = Value
            End Set

        End Property

        ' dl 15/09/2010
        Public Property Manufacturer() As String
            Get
                Return oManufacturer
            End Get

            Set(ByVal Value As String)
                oManufacturer = Value
            End Set

        End Property

        ' dl 15/09/2010
        Public Property Version() As String
            Get
                Return oVersion
            End Get

            Set(ByVal Value As String)
                oVersion = Value
            End Set

        End Property



#End Region

#Region "Constructor"

        Public Sub New()
            'OSDateFormat = ""
            'OSTimeFormat = ""
            'OSDateSeparator = ""
            'OSTimeSeparator = ""
            'OSDecimalSeparator = ""
            'OSGroupSeparator = ""
            'OSLanguage = ""
            'OSVersion = ""
            ComputerName = "" ' dl 15/09/2010
            WindowsDirectory = "" ' dl 15/09/2010
            SystemType = "" ' dl 15/09/2010
            Physicalmemory = "" ' dl 15/09/2010
            Name = "" ' dl 15/09/2010
            Model = "" ' dl 15/09/2010
            Manufacturer = "" ' dl 15/09/2010
            Version = "" ' dl 15/09/2010
        End Sub

#End Region

#Region "Methods"

        ' modified by dl 15/09/2010 add computername, windows directory, systemtype, physical memory, name, model, manufacturer, os version
        Public Overrides Function ToString() As String
            Return "OSDateFormat=" & SystemInfoManager.OSDateFormat _
              & Environment.NewLine & "OSTimeFormat = " & SystemInfoManager.OSShortTimeFormat _
              & Environment.NewLine & "OSDateSeparator = " & SystemInfoManager.OSDateSeparator _
              & Environment.NewLine & "OSTimeSeparator = " & SystemInfoManager.OSTimeSeparator _
              & Environment.NewLine & "OSDecimalSeparator = " & SystemInfoManager.OSDecimalSeparator _
              & Environment.NewLine & "OSGroupSeparator = " & SystemInfoManager.OSGroupSeparator _
              & Environment.NewLine & "OSLanguage = " & SystemInfoManager.OSLanguage _
              & Environment.NewLine & "OSVersion = " & SystemInfoManager.OSVersion _
              & Environment.NewLine & "ComputerName = " & ComputerName _
              & Environment.NewLine & "WindowsDirectory = " & WindowsDirectory _
              & Environment.NewLine & "PhysicalMemory = " & Physicalmemory _
              & Environment.NewLine & "SystemType = " & SystemType _
              & Environment.NewLine & "Name = " & Name _
              & Environment.NewLine & "Model = " & Model _
              & Environment.NewLine & "Manufacturer = " & Manufacturer _
              & Environment.NewLine & "Version = " & Version
        End Function


        Public Sub GetComputerInfo()

            Dim objOS As New ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")
            Dim objCS As New ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem")
            Dim objMgmt As ManagementObject

            For Each objMgmt In objOS.Get
                Name = objMgmt("name").ToString()
                Version = objMgmt("version").ToString()
                ComputerName = objMgmt("csname").ToString()
                WindowsDirectory = objMgmt("windowsdirectory").ToString()
            Next

            For Each objMgmt In objCS.Get
                Manufacturer = objMgmt("manufacturer").ToString()
                Model = objMgmt("model").ToString()
                SystemType = objMgmt("systemtype").ToString
                Physicalmemory = objMgmt("totalphysicalmemory").ToString()
            Next objMgmt
        End Sub

#End Region

    End Class

End Namespace


    