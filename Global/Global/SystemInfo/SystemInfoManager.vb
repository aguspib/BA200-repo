Option Explicit On
Option Strict On

Imports Microsoft.Win32
Imports System.Globalization
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

''' <summary>
''' Manages all the system information.
''' </summary>
''' <remarks></remarks>
Public Class SystemInfoManager
    Inherits GlobalBase

    'RH 15/02/2012
    Public Shared OSDecimalSeparator As String = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator ' gets the decimal Separator.
    Public Shared OSGroupSeparator As String = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator ' gets the group separtator.
    Public Shared OSDateFormat As String = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern ' gets the short pattern for date.
    Public Shared OSShortTimeFormat As String = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern ' gets the long time format.
    Public Shared OSLongTimeFormat As String = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern ' gets the short time format.
    Public Shared OSTimeSeparator As String = CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator 'gets the time separator .
    Public Shared OSDateSeparator As String = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator 'gets the date separator.
    Public Shared OSLanguage As String = CultureInfo.CurrentCulture.DisplayName ' gets the display language from system.
    Public Shared OSVersion As String = Environment.OSVersion.ToString() ' gets de Operating Systems Version.
    'END RH 15/02/2012

    'SGM 18/02/2013 - static variable for checking database update process is in progress
    Public Shared IsUpdateProcess As Boolean = False

    ''' <summary>
    ''' Get all the instaled application on the computer.
    ''' </summary>
    ''' <returns>Dataset with all the installed aplications</returns>
    ''' <remarks></remarks>
    Public Function GetAllInstallApplications() As InstalledApplicationDS
        Dim InstallAppDs As New InstalledApplicationDS ' dataset containing all the install application.
        'The registry key will be held in a string SoftwareKey.
        Dim SoftwareKey As String = "SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products"
        Using rk As RegistryKey = Registry.LocalMachine.OpenSubKey(SoftwareKey)
            For Each skName As String In rk.GetSubKeyNames
                'Get sub keys
                If Not Registry.LocalMachine.OpenSubKey(SoftwareKey).OpenSubKey(skName) Is Nothing Then
                    If Not Registry.LocalMachine.OpenSubKey(SoftwareKey).OpenSubKey(skName).OpenSubKey("InstallProperties") Is Nothing Then
                        If Not Registry.LocalMachine.OpenSubKey(SoftwareKey).OpenSubKey(skName).OpenSubKey("InstallProperties") Is Nothing Then
                            Dim name As String = Registry.LocalMachine.OpenSubKey(SoftwareKey).OpenSubKey(skName).OpenSubKey("InstallProperties").GetValue("DisplayName").ToString()
                            If name.ToString <> "" Then
                                'Declare new ListView Item
                                Dim myAppRow As InstalledApplicationDS.InstalleApplicationRow = InstallAppDs.InstalleApplication.NewInstalleApplicationRow()
                                myAppRow.ApplicationName = name.ToString()
                                InstallAppDs.InstalleApplication.AddInstalleApplicationRow(myAppRow)
                            End If
                        End If
                    End If
                End If
            Next
        End Using
        Return InstallAppDs
    End Function

    '''' <summary>
    '''' Create an SystemInfoSessionTO with all the information about the operating system.
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Function FillSystemInfoSessionTO() As SystemInfoTO
    '    'Declare application session Object 
    '    Dim MySystemInfoTO As New SystemInfoTO

    '    Try
    '        MySystemInfoTO.GetComputerInfo() ' dl 15/09/2010

    '    Catch ex As EvaluateException
    '        CreateLogActivity(ex.Message, "SystemInfo.LoadApplicationInfoSessionTO", EventLogEntryType.Error, GetSessionInfo().ActivateSystemLog)
    '    End Try
    '    Return MySystemInfoTO
    'End Function

End Class
