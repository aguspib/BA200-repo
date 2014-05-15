Option Explicit On
Option Strict On


Imports System.IO
Imports System.Windows.Forms
Imports System.Configuration
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Text         'PG 23/11/2010
Imports System.Security     'PG 23/11/2010
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Security.Cryptography
Imports Biosystems.Ax00.Types
Imports System.Runtime.InteropServices
Imports System.Globalization    ' XBC 29/01/2013 - change IsNumeric function by Double.TryParse method for Decimal values (Bugs tracking #1122)
Imports Microsoft.Win32

Namespace Biosystems.Ax00.Global
    Public Class Utilities

#Region "Constants"
        'PG 23/11/2010. Add the CrypKey
        Private Const CryptKey As String = "Ax00Bios"  '// Key de encriptacion (deben ser 8 chars pq sino falla el algoritmo que usamos)

        ' XBC 02/08/2011 - Historics format data management 
        Private Const longText As Integer = 25
        Private Const longSeparate As Integer = 1
#End Region

#Region "API Functions"

        Private Declare Function FindWindow Lib "user32.dll" Alias _
    "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Int32 'Find External Window

        Private Declare Function FindWindowEx Lib "user32.dll" Alias _
        "FindWindowExA" (ByVal hWnd1 As Int32, ByVal hWnd2 As Int32, ByVal lpsz1 As String, _
        ByVal lpsz2 As String) As Int32 'Find Child Window Of External Window

        Private Declare Function ShowWindow Lib "user32.dll" (ByVal hwnd As Int32, _
        ByVal nCmdShow As Int32) As Int32 'Show A Window

        Private Declare Function PostMessage Lib "user32.dll" Alias _
        "PostMessageA" (ByVal hwnd As Int32, ByVal wMsg As Int32, ByVal wParam As Int32, _
        ByVal lParam As Int32) As Int32 'Post Message To Window

        Private Declare Function EnableWindow Lib "user32.dll" (ByVal hwnd As Int32, _
        ByVal fEnable As Int32) As Int32 'Enable A Window

        Private Declare Function SendMessageSTRING Lib "user32.dll" Alias "SendMessageA" (ByVal hwnd As Int32, _
        ByVal wMsg As Int32, ByVal wParam As Int32, ByVal lParam As String) As Int32 'SendMessage lParam = String

        Declare Auto Function SendMessageTimeout Lib "User32" ( _
        ByVal hWnd As Integer, _
        ByVal Msg As UInt32, _
        ByVal wParam As Integer, _
        ByVal lParam As Integer, _
        ByVal fuFlags As UInt32, _
        ByVal uTimeout As UInt32, _
        ByRef lpdwResult As IntPtr _
        ) As Long 'Send Message & Wait

        Private Declare Function SendMessage Lib "user32.dll" Alias "SendMessageA" (ByVal hwnd As  _
        Int32, ByVal wMsg As Int32, ByVal wParam As Int32, ByVal lParam As Int32) As Int32 'Normal SendMessage

        Private Declare Function GetDesktopWindow Lib "user32" () As IntPtr 'Get Handle To Desktop

        Private Declare Function GetAppBarMessage Lib "shell32" Alias "SHAppBarMessage" _
          (ByVal dwMessage As Integer, ByRef pData As APPBARDATA) As Integer 'Get Message Sent By App Bar

        Private Declare Function SetAppBarMessage Lib "shell32" Alias "SHAppBarMessage" _
           (ByVal dwMessage As Integer, ByRef pData As APPBARDATA) As Integer 'Send Message To App BAr

#End Region

#Region "API Constants"

        Private Const WM_WININICHANGE As Int32 = &H1A 'INI File Update
        Private Const HWND_BROADCAST As Int32 = &HFFFF& 'Send To All
        Private Const WM_SETTINGCHANGE As Int32 = &H1A 'Setting Change
        Private Const SMTO_ABORTIFHUNG As Int32 = &H2 'Stop If Hang
        Private Const WM_COMMAND As Int32 = &H111 'Send Command
        Private Const WM_USER As Int32 = &H400 'User
        Private Const WM_SETTEXT As Int32 = &HC 'Change Text
        Private Const WM_GETTEXT As Int32 = &HD 'Get Text

        Private Const ABM_GETSTATE As Int32 = &H4 'Get Current State
        Private Const ABM_GETTASKBARPOS As Int32 = &H5 'Get TaskBar Position
        Private Const ABM_SETSTATE As Int32 = &HA 'Apply Setting(s)

        'Private Const ABS_NO_SETTINGS As Int32 = &H8 'No Settings
        'Private Const ABS_AUTOHIDE_CLOCK As Int32 = &H1 'Auto Hide and Show Clock
        'Private Const ABS_ALWAYSONTOP_CLOCK As Int32 = &H2 'Always on Top and Show Clock
        'Private Const ABS_AUTOHIDE_ALWAYSONTOP_CLOCK As Int32 = &H3 'Auto Hide and Always on Top and Show Clock
        'Private Const ABS_AUTOHIDE As Int32 = &H9 'Auto Hide
        'Private Const ABS_ALWAYSONTOP As Int32 = &HA 'Always on Top
        'Private Const ABS_AUTOHIDE_ALWAYSONTOP As Int32 = &HB 'Auto Hide and Always on Top and Show Clock


        Public Enum TaskBarStates
            SHOW_CLOCK = &H0
            AUTOHIDE_CLOCK = &H1
            ALWAYSONTOP_CLOCK = &H2
            AUTOHIDE_ALWAYSONTOP_CLOCK = &H3
            NO_SETTINGS = &H8
            AUTOHIDE = &H9
            ALWAYSONTOP = &HA
            AUTOHIDE_ALWAYSONTOP = &HB
            SHOW_SMALL_ICONS = &HC
        End Enum

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pSeconds"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/02/2012</remarks>
        Private Function ConvertSecondsToDatetime(ByVal pSeconds As Single) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim formattedRemTime As New DateTime
                If pSeconds >= 0 Then
                    formattedRemTime = formattedRemTime.AddSeconds(pSeconds)
                    myGlobal.SetDatos = formattedRemTime
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertSecondsToDatetime", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pFormattedDate"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/02/2012</remarks>
        Public Function ConvertDatetimeToSeconds(ByVal pFormattedDate As DateTime) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim timeInSeconds As Single = 0

                Dim remHours As Integer = pFormattedDate.Hour
                Dim remMinutes As Integer = pFormattedDate.Minute
                Dim remSeconds As Integer = pFormattedDate.Hour

                timeInSeconds = (remHours * 3600) + (remMinutes * 60) + remSeconds

                myGlobal.SetDatos = timeInSeconds

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertDatetimeToSeconds", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pHex"></param>
        ''' <returns></returns>
        ''' <remarks>created by SGM 10/02/2012</remarks>
        Public Function ConvertHexToUInt64(ByVal pHex As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                Dim targetNumber As UInt64

                targetNumber = Convert.ToUInt64("0x" & pHex, 16)

                myGlobal.SetDatos = targetNumber


                'Dim myInteger As Long

                'If pHex.Trim.Length Mod 2 = 0 Then
                '    Dim myBytes As New List(Of String)
                '    For c As Integer = 0 To pHex.Trim.Length - 1 Step 2
                '        Dim myByte As String = pHex.Trim.Substring(pHex.Trim.Length - c - 2, 2)
                '        myBytes.Add(myByte)
                '    Next

                '    Dim p As Integer = 0
                '    For Each B As String In myBytes
                '        myInteger += CLng(Math.Pow(255, p) * (Convert.ToUInt16("&H" & B)))
                '        p += 1
                '    Next
                'End If
                'myGlobal.SetDatos = myInteger

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertHexToUInt32", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 'pending to test
        ''' </summary>
        ''' <param name="pHex"></param>
        ''' <returns></returns>
        ''' <remarks>created by SGM 10/02/2012</remarks>
        Public Function ConvertHexToUInt32(ByVal pHex As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim targetNumber As UInt32

                targetNumber = Convert.ToUInt32("0x" & pHex, 16)

                myGlobal.SetDatos = targetNumber


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertHexToUInt32", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pHex"></param>
        ''' <returns></returns>
        ''' <remarks>created by SGM 10/02/2012</remarks>
        Public Function ConvertHexToString(ByVal pHex As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim strResult As String = ""
            Try

                Dim myBytes As New List(Of String)
                For c As Integer = 0 To pHex.Length - 1 Step 2
                    myBytes.Add(pHex.Substring(c, 2))
                Next
                For Each b As String In myBytes
                    strResult &= Chr(CInt("&H" & b))
                Next
                myGlobal.SetDatos = strResult
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertHexToString", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Converts a hexadecimal string to a binary string 
        ''' Each character represents 4 bits
        ''' </summary>
        ''' <param name="pHex"></param>
        ''' <returns></returns>
        ''' <remarks>created by SGM 10/02/2012</remarks>
        Public Function ConvertHexToBinaryString(ByVal pHex As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim strResult As String = ""
            Try
                Dim myHex As String = pHex.Trim

                Dim myWords As New List(Of String)
                For h As Integer = 0 To myHex.Length - 1
                    myWords.Add(myHex.Substring(h, 1))
                Next

                For Each w As String In myWords
                    Dim i As Integer = Convert.ToInt32(w, 16)
                    Dim s As String = Convert.ToString(i, 2)
                    While s.Length < 4
                        s = "0" & s
                    End While

                    strResult &= s
                Next


                myGlobal.SetDatos = strResult

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertHexToBinary", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pBin"></param>
        ''' <returns></returns>
        ''' <remarks>created by SGM 10/02/2012</remarks>
        Public Function ConvertBinaryStringToDecimal(ByVal pBin As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myValue As Double = 0
                For b As Integer = 0 To pBin.Length - 1 Step 1
                    Dim myPow As Integer = pBin.Length - 1 - b
                    Dim myBit As String = pBin.Substring(b, 1)
                    If myBit.ToString = "1" Then
                        myValue += Math.Pow(2, myPow)
                    End If
                Next

                myGlobal.SetDatos = myValue

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertBinaryStringToDecimal", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDec"></param>
        ''' <returns></returns>
        ''' <remarks>created by SGM 10/02/2012</remarks>
        Public Function ConvertDecimalToHex(ByVal pDec As Long) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myDec As Long = pDec
                Dim remainder As Integer
                Dim HexStr As String = ""

                Do While myDec <> 0
                    remainder = CInt(myDec Mod 16)
                    If remainder <= 9 Then
                        HexStr = Chr(Asc(remainder.ToString)) & HexStr
                    Else
                        HexStr = Chr(Asc("A") + remainder - 10) & HexStr
                    End If
                    myDec = myDec \ 16
                Loop
                If HexStr = "" Then HexStr = "0"
                myGlobal.SetDatos = HexStr

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertDecimalToHex", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pUint32"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 29/05/2012</remarks>
        Public Function ConvertUint32ToHex(ByVal pUint32 As UInt32) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUint32 As UInt32 = pUint32
                Dim remainder As Integer
                Dim HexStr As String = ""

                Do While myUint32 <> 0
                    remainder = CInt(myUint32 Mod 16)
                    If remainder <= 9 Then
                        HexStr = Chr(Asc(remainder.ToString)) & HexStr
                    Else
                        HexStr = Chr(Asc("A") + remainder - 10) & HexStr
                    End If
                    myUint32 = CType((myUint32 \ 16), UInt32)
                Loop

                If HexStr = "" Then HexStr = "0"
                myGlobal.SetDatos = HexStr

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertUint32ToHex", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pUint64"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 29/05/2012</remarks>
        Public Function ConvertUint64ToHex(ByVal pUint64 As UInt64) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim myUint64 As UInt64 = pUint64
                Dim remainder As Integer
                Dim HexStr As String = ""

                Do While myUint64 <> 0
                    remainder = CInt(myUint64 Mod 16)
                    If remainder <= 9 Then
                        HexStr = Chr(Asc(remainder.ToString)) & HexStr
                    Else
                        HexStr = Chr(Asc("A") + remainder - 10) & HexStr
                    End If
                    Dim b As UInt64 = 16
                    myUint64 = Convert.ToUInt64((myUint64 \ b))
                Loop



                If HexStr = "" Then HexStr = "0"
                myGlobal.SetDatos = HexStr

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertUint64ToHex", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDec"></param>
        ''' <returns></returns>
        ''' <remarks>created by SGM 10/02/2012</remarks>
        Public Function ConvertDecimalToBinaryString(ByVal pDec As Long) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim DecNum As Long = pDec
                Dim BinStr As String = ""

                Do While DecNum <> 0
                    If (DecNum Mod 2) = 1 Then
                        BinStr = "1" & BinStr
                    Else
                        BinStr = "0" & BinStr
                    End If
                    DecNum = DecNum \ 2
                Loop
                myGlobal.SetDatos = BinStr
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertDecimalToBinaryString", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' </summary>
        ''' <param name="pInt"></param>
        ''' <returns></returns>
        ''' <remarks>created by SGM 10/02/2012</remarks>
        Public Function ConvertIntegerToBinaryString(ByVal pInt As UInt64) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim IntNum As Int64 = Convert.ToInt64(pInt)
                Dim BinStr As String = ""

                Do While IntNum <> 0
                    If (IntNum Mod 2) = 1 Then
                        BinStr = "1" & BinStr
                    Else
                        BinStr = "0" & BinStr
                    End If
                    Dim d As Double = Math.Abs(IntNum \ 2)
                    IntNum = (Convert.ToInt64(Math.Abs(d)))
                Loop
                myGlobal.SetDatos = BinStr
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertIntegerToBinaryString", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Converts the pLax00Instruction string into a byte array to send
        ''' 
        ''' Adapted from ConvertirStringAscii (Ax5 and iPRO (Utils.ConvertirStringToAscii))
        ''' </summary>
        ''' <param name="pLax00Instruction">String to convert</param>
        ''' <param name="pSignValue">TRUE if no consecutive frames / FALSE else</param>
        ''' <returns>GlobalDataTo with data as Byte()</returns>
        ''' <remarks>Created by AG 22/04/2010 (Tested Pending)</remarks>
        Public Function ConvertStringToAscii(ByVal pLax00Instruction As String, ByVal pSignValue As Boolean) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                Dim buffAscii() As Byte
                Dim lenToConvert As Integer = pLax00Instruction.Length
                ReDim buffAscii(lenToConvert - 1)

                buffAscii = Me.AscB(pLax00Instruction)

                'AG 28/10/2010 - Comment this Ax5 code. RPM say electronics dont implement it by now
                'He is not sure to do it or define other protocol for send fractional data (for instance load Firmware)
                ''Generate sign (-) (if no more continous frames)
                'If pSignValue And buffAscii.Length > 0 Then
                '    buffAscii(0) = Me.GenerateSignFrame(buffAscii(0), 0)
                'End If

                myGlobal.SetDatos = buffAscii

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertStringToAscii", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Converts Integer to Boolean
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 23/09/2011</remarks>
        Public Function ConvertIntegerToBoolean(ByVal pInteger As Integer) As Boolean

            Dim bool As Boolean = False

            Try

                bool = (pInteger > 0)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertIntegerToBoolean", EventLogEntryType.Error, False)
            End Try

            Return bool

        End Function

        ''' <summary>
        ''' Converts Boolean to Integer
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 23/09/2011</remarks>
        Public Function ConvertBooleanToInteger(ByVal pBool As Boolean) As Integer

            Dim int As Integer = 0

            Try

                If pBool Then
                    int = 1
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertBooleanToInteger", EventLogEntryType.Error, False)
            End Try

            Return int

        End Function

        ''' <summary>
        ''' Get the EXE version (adapted from iPRO Utils.GetVersionSw)
        ''' </summary>
        ''' <param name="pIgnoreRevision">Optional parameter. When its value is TRUE (default value), it means field Revision will not be 
        '''                               returned although its value is greater than zero. Function will be called with pIgnoreRevision = FALSE
        '''                               only when the function is called to write the Version.txt file (the file is written from IAx00Login if
        '''                               it is the first time the application is loaded and the file does not exist yet, and also from ISATReport
        '''                               when a RSAT is generated)
        ''' </param>
        ''' <param name="pForRSATFromServiceSW">Optional parameter. When TRUE, it indicates the RSAT was requested from Service SW. Default value
        '''                                     is FALSE</param>
        ''' <returns>GlobalDataTO containing an String value with the SW version (Major.Minor.Build). If field Revision is informed (greater than
        '''          zero) then it is also returned in the String value (Major.Minor.Build.Revision), but only when value of optional parameter
        '''          pIgnoreRevision is FALSE</returns>
        ''' <remarks>
        ''' Created by:  AG 22/04/2010 
        ''' Modified by: RH 12/11/2010
        '''              TR 07/02/2012 - New implementation. Get the information from the AssemblyInfo and show Revision field only when its value
        '''                              is greater than zero 
        '''              SA 15/05/2014 - BT #1617 ==> ** Added optional parameter pIgnoreRevision. When value of this parameter is TRUE, field Revision
        '''                                              will not be returned although it has a value greater than zero.
        '''                                           ** Added new optional parameter pForRSATFromServiceSW. When its value is TRUE, it means the function
        '''                                              has to return the ApplicationVersion of the User SW instead of the ApplicationVersion of the 
        '''                                              Service SW.
        '''                                           ** Function has been re-written
        ''' </remarks>
        Public Function GetSoftwareVersion(Optional ByVal pIgnoreRevision As Boolean = True, _
                                           Optional ByVal pForRSATFromServiceSW As Boolean = False) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                If (Not pForRSATFromServiceSW) Then
                    'Normal function use --> get the Application Version of the program being executed (UserSW or ServiceSW)
                    If (My.Application.Info.Version.Revision = 0 OrElse pIgnoreRevision) Then
                        'REVISION field is not returned when its value is zero or it has to be ignored (pIgnoreRevision = TRUE)
                        myGlobal.SetDatos = String.Format("{0}.{1}.{2}", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, _
                                                              My.Application.Info.Version.Build)
                    Else
                        'REVISION field is returned when its value is greater than zero and it has not to be ignored (pIgnoreRevision = FALSE)
                        myGlobal.SetDatos = String.Format("{0}.{1}.{2}.{3}", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, _
                                                              My.Application.Info.Version.Build, My.Application.Info.Version.Revision)
                    End If
                Else
                    'Special case --> function has been called for the RSAT generation process launched from ServiceSW. In this case, the value
                    '                 to return has to be the Application Version of the UserSW (different code is needed to get it)
                    Dim myGlobalBase As New GlobalBase
                    Dim userSwExeFullPath As String = myGlobalBase.UserSwExeFullPath()
                    Dim fvi As FileVersionInfo = FileVersionInfo.GetVersionInfo(userSwExeFullPath)

                    If (fvi.FilePrivatePart = 0 OrElse pIgnoreRevision) Then
                        'REVISION field is not returned when its value is zero or it has to be ignored (pIgnoreRevision = TRUE)
                        myGlobal.SetDatos = String.Format("{0}.{1}.{2}", fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart)
                    Else
                        'REVISION field is returned when its value is greater than zero and it has not to be ignored (pIgnoreRevision = FALSE)
                        myGlobal.SetDatos = String.Format("{0}.{1}.{2}.{3}", fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart)
                    End If
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.GetSoftwareVersion", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Get the item (pParamIndex) from a InstructionParameterTO list)
        ''' </summary>
        ''' <param name="pList"></param>
        ''' <param name="pParamIndex"></param>
        ''' <returns>GlobalDataTo with set data as InstructionParameterTO </returns>
        ''' <remarks>Creation AG 23/04/2010 (Tested pending)</remarks>
        Public Function GetItemByParameterIndex(ByVal pList As List(Of InstructionParameterTO), ByVal pParamIndex As Integer) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim query As List(Of InstructionParameterTO)
                query = (From a In pList Where a.ParameterIndex = pParamIndex Select a).ToList

                If query.Count > 0 Then
                    myGlobal.SetDatos = query.First 'return the first item
                Else
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = "NOT_CASE_FOUND"
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.GetItemByParameterIndex", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Create the Version.txt file in the specified directory
        ''' </summary>
        ''' <param name="pFilePath">Name of the Version.txt file that has to be created</param>
        ''' <param name="pActionProcess">Optional parameter to indicate in which type of process this function has been called: SAT Report generation 
        '''                              (SAT_REPORT, which is the parameter default value) or Application Update (SAT_UPDATE)</param>
        ''' <param name="pAppVersion">Optional parameter to indicate the previous Application version (the version installed before an Update Application
        '''                           process). When value of optional parameter pActionProcess is SAT_UPDATE, it is mandatory to inform this parameter</param>
        ''' <param name="pForRSATFromServiceSW">Optional parameter. When TRUE, it indicates the RSAT was requested from Service SW.
        '''                                     Default value is FALSE</param>
        ''' <returns>GlobalDataTO containing a Boolean value that indicates when the Version.txt file was succesfully created</returns>
        ''' <remarks>
        ''' Created by:  SG 13/10/10
        ''' Modified by: TR 12/06/2013 - Added new optional parameter pActionProcess to indicate in which type of process this function has been 
        '''                              called: SAT Report generation (SAT_REPORT, which is the parameter default value) or Application Update
        '''                              (SAT_UPDATE). When value of the parameter is SAT_UPDATE, it means a Restore Point of the previous Application 
        '''                              version is beign created, and then, the User Sw Version to save in the Version.txt file have to be the one
        '''                              of the previous Application version (new optional parameter pAppVersion has been also added to inform that 
        '''                              previous version)
        '''              SA 13/05/2014 - BT #1617 ==> ** Changed the call to function GetSoftwareVersion to inform optional parameter pIgnoreRevision = FALSE, 
        '''                                              which mean that field Revision will be also written in the Version.txt file when its value is greater 
        '''                                              than zero  
        '''                                           ** Added new optional parameter pForRSATFromServiceSW, that will be informed in the call to function 
        '''                                              GetSoftwareVersion 
        ''' </remarks>
        Public Function CreateVersionFile(ByVal pFilePath As String, Optional pActionProcess As GlobalEnumerates.SATReportActions = GlobalEnumerates.SATReportActions.SAT_REPORT, _
                                          Optional pAppVersion As String = "", Optional ByVal pForRSATFromServiceSW As Boolean = False) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                'Verify in which type of process this function has been called: SAT Report Generation or Application Update
                If (pActionProcess = GlobalEnumerates.SATReportActions.SAT_REPORT) Then
                    'Function has been called for SAT Report Generation ==> The SW Version is obtained from the DLL
                    Dim myUtil As New Utilities
                    myGlobal = myUtil.GetSoftwareVersion(False, pForRSATFromServiceSW)
                Else
                    'Function has been called for Application Update ==> The SW Version is the one informed in pAppVersion parameter
                    myGlobal.SetDatos = pAppVersion
                End If

                'Write the SW Version in the Version.txt file
                If (Not myGlobal.HasError AndAlso Not myGlobal Is Nothing) Then
                    Dim myAppVersion As String = CStr(myGlobal.SetDatos)
                    Dim VersionFileName As String = pFilePath

                    Dim myStreamWriter As StreamWriter = File.CreateText(VersionFileName)
                    myStreamWriter.Write(myAppVersion)
                    myStreamWriter.Close()

                    myGlobal.SetDatos = True
                End If
            Catch ex As Exception
                If (File.Exists(pFilePath)) Then File.Delete(pFilePath)

                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.CreateVersionFile", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        '''  This method shows how to move files from one folder to another folder.
        ''' </summary>
        ''' <param name="sourcePath"></param>
        ''' <param name="DestinationPath"></param>
        ''' <remarks>
        ''' Created by:  DL 17/05/2011
        ''' Modified by: DL 17/05/2011 add new parameter optional when only move one file can move with a destination file name
        ''' </remarks>
        Public Function MoveFiles(ByVal sourcePath As String,
                                  ByVal DestinationPath As String,
                                  Optional ByVal pFilter As String = "*",
                                  Optional ByVal pDestinationFile As String = "") As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Try
                If (Directory.Exists(sourcePath)) Then
                    For Each fName As String In Directory.GetFiles(sourcePath, pFilter)
                        If File.Exists(fName) Then
                            Dim dFile As String = String.Empty
                            dFile = Path.GetFileName(fName)
                            Dim dFilePath As String = String.Empty

                            'DL 31/05/2013
                            If pFilter <> "*" AndAlso pDestinationFile <> String.Empty Then dFile = pDestinationFile
                            dFilePath = DestinationPath + dFile
                            'TR 7/09/2012 Validate if exist and delete 
                            If File.Exists(dFilePath) Then
                                File.Delete(dFilePath)
                            End If

                            System.IO.File.Move(fName, dFilePath)
                        End If
                    Next
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.MoveFiles", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function


        ''' <summary>
        '''  This method shows how to move files from one folder to another folder.
        ''' </summary>
        ''' <param name="sourcePath"></param>
        ''' <param name="DestinationPath"></param>
        ''' <remarks>Created by DL 17/05/2011</remarks>
        Public Function CopyFiles(ByVal sourcePath As String, ByVal DestinationPath As String, Optional ByVal pFilter As String = "*") As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If (Directory.Exists(sourcePath)) Then
                    For Each fName As String In Directory.GetFiles(sourcePath, pFilter)
                        If File.Exists(fName) Then
                            Dim dFile As String = String.Empty
                            dFile = Path.GetFileName(fName)
                            Dim dFilePath As String = String.Empty
                            dFilePath = DestinationPath + dFile
                            File.Copy(fName, dFilePath)
                        End If
                    Next
                End If

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.MoveFiles", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' Creating a directory by specifying a path
        ''' </summary>
        ''' <param name="pFullPath"></param>
        ''' <returns></returns>
        ''' <remarks>Created by DL 17/05/2011</remarks>
        Public Function CreateFolder(ByVal pFullPath As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Directory.CreateDirectory(pFullPath)

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.CreateFolder", EventLogEntryType.Error, False)
            End Try
            Return myGlobal

        End Function

        ''' <summary>
        ''' Remove folder and subfolders by specifying a path
        ''' </summary>
        ''' <param name="pFullPath"></param>
        ''' <returns></returns>
        ''' <remarks>Created by DL 17/05/2011</remarks>
        Public Function RemoveFolder(ByVal pFullPath As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If Directory.Exists(pFullPath) Then
                    Directory.Delete(pFullPath, True)
                Else
                    Throw New DirectoryNotFoundException
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.RemoveFolder", EventLogEntryType.Error, False)
            End Try
            Return myGlobal

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pFullPath"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 29/11/2011</remarks>
        Public Function RemoveFolderAndContents(ByVal pFullPath As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If Directory.Exists(pFullPath) Then
                    If Directory.GetDirectories(pFullPath).Length > 0 Then
                        Dim myDirectories() As String = Directory.GetDirectories(pFullPath)
                        For D As Integer = 0 To myDirectories.Length - 1
                            MyClass.RemoveFolderAndContents(myDirectories(D))
                        Next
                    End If
                    If Directory.GetFiles(pFullPath).Length > 0 Then
                        Dim myFiles() As String = Directory.GetFiles(pFullPath)
                        For F As Integer = 0 To myFiles.Length - 1
                            If File.Exists(myFiles(F)) Then File.Delete(myFiles(F))
                        Next
                    End If
                    Directory.Delete(pFullPath, True)
                Else
                    Throw New DirectoryNotFoundException
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.RemoveFolderAndContents", EventLogEntryType.Error, False)
            End Try
            Return myGlobal

        End Function

        ''' <summary>
        ''' Compress a folder to a zip file
        ''' </summary>
        ''' <param name="psourceDir"></param>
        ''' <param name="ptargetName"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SG 13/10/10</remarks>
        Public Function CompressToZip(ByVal pSourceDir As String, ByVal pTargetName As String, Optional ByVal pPass As String = "") As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim fz As FastZip = New FastZip()
                If pPass.Length = 0 Then
                    fz.Password = "biosystems"
                Else
                    fz.Password = pPass
                End If
                'TR 02/02/2012 -Before Creating the Zip file validate the SourceDir if Exist.
                If Directory.Exists(pSourceDir) Then
                    fz.CreateZip(pTargetName, pSourceDir, True, "")
                    fz = Nothing
                    myGlobal.SetDatos = True
                Else
                    myGlobal.HasError = True
                    myGlobal.ErrorMessage = "SAT Directory no found"
                End If

            Catch ex As IOException
                'TR 02/02/2012 if IOExeption is produced is Because there is a problem with the source directory
                'Validate if there are information saved on Source directory this is corrupted data.
                myGlobal.HasError = True
                myGlobal.ErrorCode = "IO_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.CompressToZip", EventLogEntryType.Error, False)
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.CompressToZip", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Extract from a zip file to a folder
        ''' </summary>
        ''' <param name="pZipFilePath"></param>
        ''' <param name="ptargetDirPath"></param>
        ''' <remarks>Created by SG 08/10/10</remarks>
        Public Function ExtractFromZip(ByVal pZipFilePath As String, ByVal pTargetDirPath As String, Optional ByVal pPass As String = "") As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim fz As FastZip = New FastZip()
            Try
                If pPass.Length = 0 Then
                    fz.Password = "biosystems"
                Else
                    fz.Password = pPass
                End If

                Try 'DL 31/05/2013
                    fz.ExtractZip(pZipFilePath, pTargetDirPath, "")

                    'DL 31/05/2013
                Catch ex As ZipException
                    myGlobal.HasError = False
                    myGlobal.ErrorCode = GlobalEnumerates.Messages.ZIP_ERROR.ToString
                    myGlobal.ErrorMessage = ex.Message
                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ExtractFromZip", EventLogEntryType.Error, False)
                End Try
                'DL 31/05/2013

                fz = Nothing

                myGlobal.SetDatos = True

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ExtractFromZip", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Decrypt file. It is inherited of iPRO 
        ''' </summary>
        ''' <param name="CryptFile"></param>
        ''' <param name="pDecryptfile"></param>
        ''' <remarks>
        ''' Created by: PG 23/11/2010
        ''' Modified by XBC 03/12/2010
        ''' </remarks>
        Public Function DecryptFile(ByVal CryptFile As String, ByVal pDecryptfile As String) As GlobalDataTO
            '// Se copia de: http://support.microsoft.com/kb/301070
            Dim myGlobal As New GlobalDataTO
            Dim cryptostreamDecr As Cryptography.CryptoStream = Nothing
            Dim fsread As FileStream = Nothing
            Dim fsDecrypted As StreamWriter = Nothing
            Try
                Dim myGlobalbase As New GlobalBase
                Dim DES As New Cryptography.DESCryptoServiceProvider()

                DES.Key() = ASCIIEncoding.ASCII.GetBytes(CryptKey)
                DES.IV = ASCIIEncoding.ASCII.GetBytes(CryptKey)

                fsread = New FileStream(CryptFile, FileMode.Open, FileAccess.Read)

                cryptostreamDecr = New Cryptography.CryptoStream(fsread, DES.CreateDecryptor(), Cryptography.CryptoStreamMode.Read)
                fsDecrypted = New StreamWriter(pDecryptfile)
                fsDecrypted.Write(New StreamReader(cryptostreamDecr).ReadToEnd)
                fsDecrypted.Flush()

                myGlobal.SetDatos = True
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.DecryptFile", EventLogEntryType.Error, False)
            Finally
                '// Close the files
                If Not fsDecrypted Is Nothing Then
                    fsDecrypted.Close()
                    fsDecrypted.Dispose()
                    fsDecrypted = Nothing
                End If
                If Not fsread Is Nothing Then
                    fsread.Close()
                    fsread.Dispose()
                    fsread = Nothing
                End If
                If Not cryptostreamDecr Is Nothing Then
                    cryptostreamDecr = Nothing
                End If
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Encrypt file. It is inherited of iPRO 
        ''' </summary>
        ''' <param name="DecryptFile"></param>
        ''' <param name="Cryptfile"></param>
        ''' <remarks>
        ''' Created by: PG 23/11/2010
        ''' Modified by XBC 03/12/2010
        ''' </remarks>
        Public Function EncryptFile(ByVal DecryptFile As String, ByVal Cryptfile As String) As GlobalDataTO
            '// Se copia de: http://support.microsoft.com/kb/301070

            Dim myGlobal As New GlobalDataTO
            Dim fsInput As FileStream = Nothing
            Dim fsEncrypted As FileStream = Nothing
            Try
                Dim myGlobalbase As New GlobalBase

                If File.Exists(DecryptFile) Then
                    fsInput = New FileStream(DecryptFile, FileMode.Open, FileAccess.Read)

                    fsEncrypted = New FileStream(Cryptfile, FileMode.Create, FileAccess.Write)

                    Dim DES As New Cryptography.DESCryptoServiceProvider()
                    DES.CreateDecryptor()
                    DES.Key = ASCIIEncoding.ASCII.GetBytes(CryptKey)
                    DES.IV = ASCIIEncoding.ASCII.GetBytes(CryptKey)

                    Dim cryptostream As New Cryptography.CryptoStream(fsEncrypted, DES.CreateEncryptor, Cryptography.CryptoStreamMode.Write)
                    Dim bytearrayinput(CInt(fsInput.Length - 1)) As Byte
                    fsInput.Read(bytearrayinput, 0, bytearrayinput.Length)
                    cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length)

                    '// Close the files
                    cryptostream.Close()
                    fsInput.Close()
                    fsEncrypted.Close()
                Else
                    Throw New FileNotFoundException
                End If
                myGlobal.SetDatos = True
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.EncryptFile", EventLogEntryType.Error, False)
                fsInput.Close()
                fsEncrypted.Close()
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Create an EventLog instance and pass log name and MachineName on which the log resides
        ''' </summary>
        ''' <remarks>
        ''' Created by: DL 09/12/2010
        ''' </remarks>
        Public Function ClearWindowsApplicationLog() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO

            Try
                Dim evtLog As New EventLog("Application", System.Environment.MachineName)

                If evtLog.OverflowAction = OverflowAction.OverwriteOlder Then
                    evtLog.Clear()
                End If

                evtLog.Close()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ClearWindowsApplicationLog", EventLogEntryType.Error, False)
            End Try

            Return myGlobal
        End Function

        ''' <summary>
        ''' Serializes a Dataset to a Xml text file
        ''' </summary>
        ''' <param name="pDataset"></param>
        ''' <param name="pPath"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/12/2011</remarks>
        Public Function SerializeDataset(ByVal pDataset As DataSet, ByVal pPath As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myStreamWriter As StreamWriter

            Try
                Dim stream As New MemoryStream()
                pDataset.WriteXml(stream, XmlWriteMode.DiffGram)
                stream.Seek(0, SeekOrigin.Begin)
                Dim sr As New StreamReader(stream)
                Dim myDatasetString As String = sr.ReadToEnd()

                myStreamWriter = File.CreateText(pPath)
                myStreamWriter.Write(myDatasetString)
                myStreamWriter.Close()

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.SerializeDataset", EventLogEntryType.Error, False)

            Finally
                If myStreamWriter IsNot Nothing Then
                    myStreamWriter.Dispose()
                    myStreamWriter = Nothing
                End If
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' Deserialize from a xml text file to a dataset
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 07/12/2011</remarks>
        Public Function DeserializeDataset(ByVal pDatasetType As Type, ByVal pPath As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim myStringReader As System.IO.StringReader

            Try
                myStringReader = New System.IO.StringReader(pPath)

                Dim ds As New DataSet()
                ds.ReadXml(myStringReader, XmlReadMode.DiffGram)

                myGlobal.SetDatos = ds

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.DeserializeDataset", EventLogEntryType.Error, False)

            Finally
                If myStringReader IsNot Nothing Then
                    myStringReader.Dispose()
                    myStringReader = Nothing
                End If
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Binary formatter for serialization
        ''' </summary>
        ''' <param name="pObject">Source Object to serialze</param>
        ''' <param name="pPath">Destination of the object serialized</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/03/2011</remarks>
        Public Function Serialize(ByVal pObject As Object, ByVal pPath As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Dim formatter As IFormatter = New BinaryFormatter
            Try
                ' All you need is to create an instance of the stream and the formatter to use, 
                Dim stream As Stream = New FileStream(pPath, FileMode.Create, FileAccess.Write, FileShare.Read)
                'and then call the Serialize method on the formatter
                formatter.Serialize(stream, pObject)
                ' Ensure that everything is written.
                stream.Flush()
                ' Close the stream. 
                stream.Close()
                myGlobal.SetDatos = True
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.Serialize", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Binary formatter for recover a previous serialization
        ''' </summary>
        ''' <param name="pObject">Destination Object where deserialize</param>
        ''' <param name="pPath">Source path from deserialize</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/03/2011</remarks>
        Public Function DeSerialize(ByVal pObject As Object, ByVal pPath As String) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim formatter As IFormatter = New BinaryFormatter
                Dim stream As Stream = New FileStream(pPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                pObject = formatter.Deserialize(stream)
                stream.Close()
                myGlobal.SetDatos = pObject
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.DeSerialize", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Resizes an Image
        ''' </summary>
        ''' <param name="pNewSize"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 15/03/11</remarks>
        Public Function ResizeImage(ByVal pImage As System.Drawing.Image, ByVal pNewSize As System.Drawing.Size) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                If pImage IsNot Nothing Then
                    Dim myNewImage As New System.Drawing.Bitmap(pNewSize.Width, pNewSize.Height)
                    Dim GraphicsImage As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(myNewImage)
                    GraphicsImage.DrawImage(pImage, 0, 0, myNewImage.Width, myNewImage.Height)
                    myGlobal.SetDatos = myNewImage
                Else
                    myGlobal.HasError = True
                End If
            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ResizeImage", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Sets image black and white
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>SGM 15/03/11</remarks>
        Public Function SetImageBW(ByVal pImage As System.Drawing.Image) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim axisY As Integer, axisX As Integer
                Dim pxlColor As System.Drawing.Color


                Dim myBitmap As New System.Drawing.Bitmap(pImage)
                Dim myNewImage As New System.Drawing.Bitmap(pImage)
                Dim x As Integer = myBitmap.Width
                Dim y As Integer = myBitmap.Height
                '----------------------------------
                For axisX = 0 To x - 1
                    For axisY = 0 To y - 1
                        pxlColor = myBitmap.GetPixel(axisX, axisY) 'optiene el color para el picel de pic
                        Dim myNewColor As New System.Drawing.Color
                        Dim B As Single = pxlColor.GetBrightness
                        Dim A As Integer = pxlColor.A

                        If B = 0 Then
                            myNewColor = System.Drawing.Color.FromArgb(System.Drawing.Color.Transparent.ToArgb)
                        Else
                            myNewColor = System.Drawing.Color.FromArgb(A, CInt(B * 255), CInt(B * 255), CInt(B * 255))
                        End If

                        myNewImage.SetPixel(axisX, axisY, myNewColor)

                        'If pxlColor.GetBrightness > 0.5 Then
                        '    myNewImage.SetPixel(axisX, axisY, System.Drawing.Color.White)
                        'ElseIf pxlColor.GetBrightness > 0.25 Then
                        '    myNewImage.SetPixel(axisX, axisY, System.Drawing.Color.Gray)
                        'ElseIf pxlColor.GetBrightness = 0 Then
                        '    myNewImage.SetPixel(axisX, axisY, System.Drawing.Color.Transparent)
                        'Else
                        '    myNewImage.SetPixel(axisX, axisY, System.Drawing.Color.Black)
                        'End If
                    Next
                Next

                myGlobal.SetDatos = myNewImage

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = "SYSTEM_ERROR"
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.SetImageBW", EventLogEntryType.Error, False)
            End Try
            Return myGlobal
        End Function


        ''' <summary>
        ''' Returns the Percent value betwenn two limits
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <param name="pMin"></param>
        ''' <param name="pMax"></param>
        ''' <param name="pForceLimitsFlags" >TRUE the percent must be in interval 0 - 100 // If false (original code) no restriction</param>
        ''' <returns></returns>
        ''' <remarks>SGM 24/03/11
        ''' AG 27/09/2011 - add pForceLimitsFlags</remarks>
        Public Function CalculatePercent(ByVal pValue As Double, ByVal pMin As Double, ByVal pMax As Double, Optional ByVal pForceLimitsFlags As Boolean = False) As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try

                Dim myPercent As Double

                If pMin < pMax Then
                    myPercent = (100 * ((pValue - pMin) / (pMax - pMin)))

                    'AG 27/09/2011
                    If pForceLimitsFlags Then
                        If myPercent > 100 Then
                            myPercent = 100
                        ElseIf myPercent < 0 Then
                            myPercent = 0
                        End If
                    End If
                    'AG 27/09/2011

                    myGlobal.SetDatos = myPercent
                Else
                    myGlobal.HasError = True
                    myGlobal.ErrorCode = GlobalEnumerates.Messages.MINGREATHERMAX.ToString
                End If


            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.CalculatePercent", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function


        ''' <summary>
        ''' Format the Scales values. 
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 03/10/2011</remarks>
        Public Function ToStringWithFormat(ByVal pValue As Double, ByVal pDecimalsNumber As Integer) As String
            Dim myResult As String = ""
            Try
                If pValue Mod 2 = 0 Then
                    myResult = String.Format("{0}", CInt(pValue))
                Else
                    Dim myFormat As String = "F" & pDecimalsNumber.ToString
                    myResult = String.Format("{0}", pValue.ToString(myFormat))
                End If
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ToStringWithFormat", EventLogEntryType.Error, False)
            End Try
            Return myResult
        End Function


        Public Function ReadBinaryFile(ByVal pPath As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim input As New FileStream(pPath, FileMode.Open)
            Dim reader As New BinaryReader(input)

            Try


                Dim bytes() As Byte
                bytes = reader.ReadBytes(CInt(input.Length))
                myGlobal.SetDatos = bytes

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ReadBinaryFile", EventLogEntryType.Error, False)
            End Try

            input.Dispose()
            reader = Nothing

            Return myGlobal

        End Function

        Public Function WriteBinaryFile(ByVal pPath As String, ByVal pContent() As Byte) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myFileStream As FileStream

            Try

                myFileStream = New FileStream(pPath, FileMode.OpenOrCreate, FileAccess.Write)
                Dim myBinaryWriter As New BinaryWriter(myFileStream)
                myBinaryWriter.Write(pContent)
                myBinaryWriter.Write(1)

            Catch ex As Exception
                If Not (myFileStream Is Nothing) Then myFileStream.Close()
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.WriteBinaryFile", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pPath"></param>
        ''' <param name="pTextToAppend"></param>
        ''' <returns></returns>
        ''' <remarks>Created: SGM 29/07/2011</remarks>
        Public Function WriteTextFile(ByVal pPath As String, ByVal pTextToAppend As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myFileStream As New FileStream(pPath, FileMode.Create, FileAccess.Write)
            Dim myStreamWriter As New StreamWriter(myFileStream)

            Try
                myStreamWriter.BaseStream.Seek(0, SeekOrigin.End)
                myStreamWriter.WriteLine(pTextToAppend)
                myStreamWriter.Close() 'closing the file

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.WriteTextFile", EventLogEntryType.Error, False)
            End Try

            myFileStream.Dispose()
            myStreamWriter = Nothing

            Return myGlobal

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pPath"></param>
        ''' <param name="pText"></param>
        ''' <returns></returns>
        ''' <remarks>Created: SGM 29/07/2011</remarks>
        Public Function OverWriteTextFile(ByVal pPath As String, ByVal pText As String) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myFileStream As New FileStream(pPath, FileMode.Create, FileAccess.Write)
            Dim myStreamWriter As New StreamWriter(myFileStream)

            Try
                myStreamWriter.Write(pText)
                myStreamWriter.Close() 'closing the file

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.OverWriteTextFile", EventLogEntryType.Error, False)
            End Try

            myFileStream.Dispose()
            myStreamWriter = Nothing

            Return myGlobal

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pPath"></param>
        ''' <param name="pTextToAppend"></param>
        ''' <returns></returns>
        ''' <remarks>Created: SGM 29/07/2011</remarks>
        Public Function ReadTextFile(ByVal pPath As String, ByVal pAccess As FileAccess, Optional ByVal pTextToAppend As String = "") As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myFileStream As New FileStream(pPath, FileMode.Open, pAccess)
            Dim myStreamReader As StreamReader
            Dim myStreamWriter As StreamWriter

            Try
                Select Case pAccess
                    Case FileAccess.Read
                        myStreamReader = New StreamReader(myFileStream)
                        myStreamReader.BaseStream.Seek(0, SeekOrigin.Begin)
                        Dim myReadedText As String = ""
                        While myStreamReader.Peek() > -1
                            myReadedText &= myStreamReader.ReadLine()
                        End While

                        myGlobal.SetDatos = myReadedText

                        myStreamReader.Close()

                    Case FileAccess.Write
                        myStreamWriter = New StreamWriter(myFileStream)
                        myStreamWriter.WriteLine(pTextToAppend)
                        myStreamWriter.Close()


                    Case FileAccess.ReadWrite

                End Select

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ReadTextFile", EventLogEntryType.Error, False)
            End Try



            myFileStream.Dispose()
            myStreamWriter = Nothing
            myStreamReader = Nothing

            Return myGlobal

        End Function

        ''' <summary>
        ''' Data format for historical reports of Service's software  - this must be accessible for several screens of the application
        ''' </summary>
        ''' <param name="text1">part 1 of the same line</param>
        ''' <param name="text2">part 2 of the same line</param>
        ''' <param name="text3">part 3 of the same line</param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/08/2011</remarks>
        Public Function FormatLineHistorics(Optional ByVal text1 As String = "", Optional ByVal text2 As String = "", Optional ByVal text3 As String = "") As String
            Dim returnValue As String = ""
            Try
                returnValue += text1
                returnValue += SetSpaces(longText - text1.Length)
                returnValue += SetSpaces(longSeparate)
                returnValue += text2
                returnValue += SetSpaces(longText - text2.Length)
                returnValue += SetSpaces(longSeparate)
                returnValue += text3
                returnValue += Environment.NewLine

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utils.FormatLine", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function



        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pTextColumns"></param>
        ''' <param name="pColWidth"></param>
        ''' <param name="pAddLine"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 04/08/2011</remarks>
        Public Function FormatLineHistorics(ByVal pTextColumns As List(Of String), _
                                            ByVal pColWidth As Integer, _
                                            Optional ByVal pAddLine As Boolean = False) As String

            Dim returnValue As String = ""

            Try
                If pTextColumns IsNot Nothing AndAlso pTextColumns.Count > 0 Then

                    For c As Integer = 0 To pTextColumns.Count - 1

                        Dim myCol As String = pTextColumns(c).PadRight(pColWidth)

                        returnValue &= myCol

                    Next
                End If


                If returnValue.Length > 0 Then
                    'returnValue = returnValue.PadRight(pColWidth)
                    returnValue &= Environment.NewLine
                    If pAddLine Then
                        returnValue &= Environment.NewLine
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utils.FormatLineHistorics", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function

        ''' <summary>
        ''' For the use of formatting content data in historical reports
        ''' </summary>
        ''' <param name="numSpaces"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 02/08/2011</remarks>
        Public Function SetSpaces(ByVal numSpaces As Integer) As String
            Dim returnValue As String = ""
            Try
                For i As Integer = 0 To numSpaces - 1
                    returnValue += " "
                Next
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utils.SetSpaces", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function


        ''' <summary>
        ''' Converts seconds to HH:mm:ss
        ''' </summary>
        ''' <param name="pSeconds"></param>
        ''' <returns></returns>
        ''' <remarks>Created by XBC 05/09/2011</remarks>
        Public Function FormatToHHmmss(ByVal pSeconds As Long) As String
            Dim returnValue As String = ""
            Try
                Dim hours As Single
                Dim minutes As Single
                Dim seconds As Single

                hours = pSeconds \ 3600
                minutes = pSeconds \ 60 Mod 60
                seconds = pSeconds Mod 60
                returnValue = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00")

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utils.FormatToHHmmss", EventLogEntryType.Error, False)
            End Try
            Return returnValue
        End Function

        'Public Function FormatToSingle(ByVal pValue As String) As Single
        '    Dim myGlobal As New GlobalDataTO
        '    Dim returnValue As Single = -1
        '    Try

        '        myGlobal = PCInfoReader.GetOSCultureInfo()
        '        Dim OSCultureInfo As New PCInfoReader.AX00PCOSCultureInfo
        '        If Not myGlobal.HasError And Not myGlobal Is Nothing Then
        '            OSCultureInfo = CType(myGlobal.SetDatos, PCInfoReader.AX00PCOSCultureInfo)

        '            If pValue.Contains(",") Then
        '                pValue = pValue.Replace(",", OSCultureInfo.DecimalSeparator)
        '            ElseIf pValue.Contains(".") Then
        '                pValue = pValue.Replace(".", OSCultureInfo.DecimalSeparator)
        '            End If

        '            returnValue = CSng(pValue)
        '        End If

        '    Catch ex As Exception
        '        'myGlobal.HasError = True
        '        'myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        'myGlobal.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "Utilities.FormatToSingle ", EventLogEntryType.Error, False)
        '    End Try
        '    'Return myGlobal
        '    Return returnValue
        'End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pValue"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Modified by RH 21/10/2011 - Code optimization
        '''             XB 29/01/2013 - Change IsNumeric function by Double.TryParse method for Decimal values (Bugs tracking #1122)
        ''' </remarks>
        Public Function FormatToSingle(ByVal pValue As String) As Single
            Dim returnValue As Single = -1

            Try
                Dim DecimalSeparator As String = SystemInfoManager.OSDecimalSeparator

                If pValue.Contains(",") Then
                    pValue = pValue.Replace(",", DecimalSeparator)
                ElseIf pValue.Contains(".") Then
                    pValue = pValue.Replace(".", DecimalSeparator)
                End If

                'If pValue.Length > 0 AndAlso IsNumeric(pValue.ToString) Then
                If pValue.Length > 0 AndAlso Double.TryParse(pValue.ToString, NumberStyles.Any, CultureInfo.InvariantCulture, New Double) Then
                    'AG 13/12/2011 - sometimes this method produces an Overflow exception
                    'returnValue = CSng(pValue)
                    Dim maxSingleValue As Double = Single.MaxValue
                    Dim minSingleValue As Double = Single.MinValue
                    'Dim auxValue As Double = CDbl(pValue)'TR 11/01/2012 -Commented
                    Dim auxValue As Double = 0
                    Double.TryParse(pValue, auxValue) 'TR 00/01/2012 implement the try parse

                    If auxValue <= maxSingleValue Then
                        'returnValue = CSng(pValue)
                        Single.TryParse(pValue, returnValue)
                    Else
                        returnValue = Single.MaxValue
                    End If

                    If auxValue >= minSingleValue Then
                        'returnValue = CSng(pValue)
                        Single.TryParse(pValue, returnValue)
                    Else
                        returnValue = Single.MinValue
                    End If
                    'AG 13/12/2011
                Else
                    Dim myLogAcciones2 As New ApplicationLogManager()
                    myLogAcciones2.CreateLogActivity("Input parameter is not numeric", "Utilities.FormatToSingle ", EventLogEntryType.Error, False)
                    returnValue = 0
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.FormatToSingle ", EventLogEntryType.Error, False)
            End Try

            Return returnValue
        End Function

        ''' <summary>
        ''' Converts Adjustments in string format to an Adjustments in dataset format
        ''' </summary>
        ''' <param name="pAdjustmentsData"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 01/12/2011</remarks>
        Public Function ConvertAdjustmentsTextToDS(ByVal pAdjustmentsData As String, ByVal pAdjustmentsMasterDataDS As SRVAdjustmentsDS) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim myAdjustmentsDS As New SRVAdjustmentsDS

            Try

                If pAdjustmentsData.Length > 0 AndAlso pAdjustmentsMasterDataDS IsNot Nothing Then
                    'split into lines
                    Dim myLines() As String = pAdjustmentsData.Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                    Dim myAdjustmentArea As String = ""

                    If myLines.Length = 1 Then
                        Dim myAdjs() As String = myLines(0).Split(";".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                        For i As Integer = 0 To myAdjs.Length - 1 Step 1
                            Dim LabelValue() As String = myAdjs(i).Split(":".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                            If LabelValue.Length = 2 Then
                                Dim AdjLabel As String = LabelValue(0)

                                If AdjLabel.ToUpper.Trim <> "CHECKAJ" Then

                                    Dim AdjValue As String = LabelValue(1)

                                    If Not IsNumeric(AdjValue) Then
                                        Throw New Exception
                                    End If


                                    Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                                    myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                                        In pAdjustmentsMasterDataDS.srv_tfmwAdjustments _
                                                        Where a.CodeFw = AdjLabel _
                                                        Select a).ToList

                                    If myAdjustmentRows.Count > 0 Then
                                        Dim myAdjustmentRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow = myAdjustmentRows(0)
                                        With myAdjustmentRow
                                            .Value = AdjValue
                                            '.AreaFw = ""
                                            '.DescriptionFw = ""
                                            .InFile = True
                                        End With
                                    End If

                                    myAdjustmentsDS.AcceptChanges()

                                End If
                            End If
                        Next
                    Else
                        'the data comes with lines and comments
                        For L As Integer = 0 To myLines.Length - 1 Step 1
                            Dim myAdjustmentLine As String = ""

                            Dim myLine As String = myLines(L).Trim

                            If myLine.Length > 0 Then

                                If Not myLine.StartsWith("--") Then 'header, comments

                                    If myLine.StartsWith("{") Then 'group
                                        myAdjustmentArea = myLine.Replace("{", "").Replace("}", "")
                                    Else
                                        myAdjustmentLine = myLine

                                        'code
                                        Dim myAdjustmentCode As String = myAdjustmentLine.Substring(0, myAdjustmentLine.IndexOf(":"))

                                        If myAdjustmentCode.ToUpper.Trim <> "CHECKAJ" Then

                                            'value
                                            Dim myAdjustmentValue As String = ""
                                            myAdjustmentValue = myAdjustmentLine.Substring(myAdjustmentLine.IndexOf(":") + 1)
                                            myAdjustmentValue = myAdjustmentValue.Substring(0, myAdjustmentValue.IndexOf(";"))

                                            If Not IsNumeric(myAdjustmentValue) Then
                                                Throw New Exception
                                            End If


                                            'description
                                            Dim myAdjustmentDescription As String = ""
                                            If myAdjustmentLine.Contains("}") Then
                                                myAdjustmentDescription = myAdjustmentLine.Substring(myAdjustmentLine.IndexOf("{") + 1)
                                                myAdjustmentDescription = myAdjustmentDescription.Substring(0, myAdjustmentDescription.IndexOf("}"))
                                            End If

                                            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                                            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                                                In myAdjustmentsDS.srv_tfmwAdjustments _
                                                                Where a.CodeFw = myAdjustmentCode _
                                                                Select a).ToList

                                            If myAdjustmentRows.Count > 0 Then
                                                Dim myAdjustmentRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow = myAdjustmentRows(0)
                                                With myAdjustmentRow
                                                    .Value = myAdjustmentValue
                                                    .AreaFw = myAdjustmentArea
                                                    .DescriptionFw = myAdjustmentDescription
                                                    .InFile = True
                                                End With
                                            End If

                                            myAdjustmentsDS.AcceptChanges()

                                        End If
                                    End If
                                End If
                            End If
                        Next

                    End If


                    resultData.SetDatos = myAdjustmentsDS

                Else

                    resultData.SetDatos = Nothing

                End If




            Catch ex As Exception

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.ConvertAdjustmentsTextToDS ", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns the temperature sensor value to be shown or validated
        ''' </summary>
        ''' <param name="pFwValue"></param>
        ''' <param name="pSetpoint"></param>
        ''' <param name="pTarget"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 20/02/2012</remarks>
        Public Function MakeSensorValueCorrection(ByVal pFwValue As Single, ByVal pSetpoint As Single, ByVal pTarget As Single) As Single
            Try
                Dim myOffSet As Single

                myOffSet = pSetpoint - pTarget

                Return pFwValue - myOffSet

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.MakeSensorValueCorrection ", EventLogEntryType.Error, False)
            End Try
        End Function

        ''' <summary>
        ''' Calculates the age and age unit (Years, Months or Days) from a birth date
        ''' </summary>
        ''' <remarks>
        ''' Created by:  RH 05/12/2011    
        ''' Modified by: SA 02/08/2012 - Added code to calculate correctly the Patient age: if the Patient Birthday has not still passed,
        '''                              decrement the calculate age by one
        ''' </remarks>
        Public Shared Function GetAgeUnits(ByVal pDateOfBirth As Date, ByVal AgeUnitsListDS As PreloadedMasterDataDS) As String
            Dim ageUnitDesc As String = String.Empty

            Try
                Dim patientAge As Long = 0
                Dim ageUnitCode As String = String.Empty

                patientAge = DateDiff(DateInterval.Day, pDateOfBirth, Today)

                If (patientAge >= 365) Then
                    patientAge = DateDiff(DateInterval.Year, pDateOfBirth, Today)
                    ageUnitCode = "Y"

                    'Verify if the Patient Birthday has passed; if not, then decrement the age by one
                    If (pDateOfBirth.Month > Today.Month) Then
                        patientAge -= 1
                    ElseIf (pDateOfBirth.Month = Today.Month) Then
                        If (pDateOfBirth.Day > Today.Day) Then
                            patientAge -= 1
                        End If
                    End If

                ElseIf (patientAge >= 30) Then
                    patientAge = DateDiff(DateInterval.Month, pDateOfBirth, Today)
                    ageUnitCode = "M"

                    'Verify if the Patient Birthday has passed; if not, then decrement the age by one
                    If (pDateOfBirth.Day > Today.Day) Then
                        patientAge -= 1
                    End If
                Else
                    ageUnitCode = "D"
                End If

                'Get the multilanguage description for the Age Unit and return the value
                Dim lstAgeUnit As List(Of String) = (From a In AgeUnitsListDS.tfmwPreloadedMasterData _
                                                    Where a.ItemID = ageUnitCode _
                                                   Select a.FixedItemDesc).ToList()
                If (lstAgeUnit.Count = 1) Then ageUnitDesc = String.Format("{0} {1}", patientAge, lstAgeUnit.First())

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.GetAgeUnits", EventLogEntryType.Error, False)
            End Try
            Return ageUnitDesc
        End Function



        Function Display() As String
            With System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly.Location)
                Return .Comments
            End With
        End Function

        ''' <summary>
        ''' Alternative way for powering an integer to 2
        ''' </summary>
        ''' <param name="N"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 31/08/2012</remarks>
        Public Function PowUint64To2(ByVal N As UInt64) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO

            Try

                'example:
                '-----------------------------------------------
                '    decimal 	12 (12*12 = 144)
                '   hexadecimal(1100)

                '            1100:
                '           *
                '            1100:
                '           -------	
                '            0000:
                '           0000:
                '          1100:
                '         1100:
                '        ----------
                '        10010000 = 144
                '----------------------------------------------------

                '1-Se obtiene el valor binario
                myGlobal = MyClass.ConvertIntegerToBinaryString(N)
                Dim Nstr As String = CStr(myGlobal.SetDatos)
                Dim Nstr2 As String = Nstr

                '2-Se van generando los valores binarios a sumar
                Dim Sumatory As New List(Of String)
                For b As Integer = 0 To Nstr.Length - 1
                    Dim B1 As Integer = CInt(Nstr.Substring(b, 1))
                    If B1 = 1 Then
                        Sumatory.Add(Nstr)
                    ElseIf B1 = 0 Then
                        Dim zeros As String = ""
                        For z As Integer = 0 To Nstr.Length - 1
                            zeros &= "0"
                        Next z
                        Sumatory.Add(zeros)
                    End If
                Next

                '3-se añaden los ceors necesarios a cada sumando
                Dim Sumatory2 As New List(Of String)
                For i As Integer = 0 To Sumatory.Count - 1
                    Dim s As String = Sumatory(i)
                    For z As Integer = i To Sumatory.Count - 2
                        s &= "0"
                    Next
                    Sumatory2.Add(s)
                Next

                '4-se realiza la suma final
                Dim Sum As UInt64
                For Each s As String In Sumatory2
                    myGlobal = MyClass.ConvertBinaryStringToDecimal(s)
                    Dim D As UInt64 = Convert.ToUInt64(myGlobal.SetDatos)
                    Sum += D
                Next

                myGlobal.SetDatos = Sum

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.PowUint64To2", EventLogEntryType.Error, False)
            End Try

            Return myGlobal

        End Function


        ''' <summary>
        ''' Generate a new GUID string
        ''' http://www.techrepublic.com/article/generating-guids-with-vbnet/6177750
        ''' </summary>
        ''' <returns>GlobalDataTo (Data as String)</returns>
        ''' <remarks>AG 11/01/2012</remarks>
        Public Function GetNewGUID() As GlobalDataTO
            Dim myGlobal As New GlobalDataTO
            Try
                Dim sGUID As String
                sGUID = System.Guid.NewGuid.ToString()
                myGlobal.SetDatos = sGUID

            Catch ex As Exception
                myGlobal.HasError = True
                myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobal.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.GetNewGUID", EventLogEntryType.Error, False)

            End Try
            Return myGlobal
        End Function

        ''' <summary>
        ''' Get all the information related to the Synapse application from 
        ''' the System Event viewer. make a backup of synapse file Event Log.
        ''' </summary>
        '''<param name="pFileName">File Name</param>
        ''' <param name="pFilePath">Paht where the file will be saved</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 17/04/2013
        ''' modified by: TR 23/04/2012 -Add parameter NumberofDays. Used to indicate the number of days we want to get the informationn
        '''                             the information of this parameter is get for table parameters programming parametername 
        '''                             LIS_LOG_MAX_DAYS. 
        '''              TR 10/06/2013 -Create a backup of Synapse Event Log File. Now is not neede the Number of days, because we save 
        '''                             All the information on the Synapse Event log file.
        ''' </remarks>
        Public Function SaveSynapseEventLog(ByVal pFileName As String, ByVal pFilePath As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim hEventLog As IntPtr
                Dim lretv As Integer
                hEventLog = OpenEventLog(vbNullString, "UDC")
                If hEventLog = IntPtr.Zero Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                    myGlobalDataTO.ErrorMessage = "OpenEvent Log Failed"
                Else
                    lretv = BackupEventLog(hEventLog, pFilePath & "\" & pFileName & ".evt")
                    If lretv = 0 Then
                        myGlobalDataTO.HasError = True
                        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                        myGlobalDataTO.ErrorMessage = "BackupEventLog Failed"
                    End If
                End If


                'Dim eventCntr As Integer = 1
                'Dim sb As New StringBuilder()
                ''Variable that load all the eventlog related to de Synapse (UDC is the name on the application log for Synapse).
                'Dim eventLogApp As New System.Diagnostics.EventLog("UDC")
                'Dim myInitalDate As New DateTime
                'myInitalDate = DateTime.MinValue
                ''Get the information inverse to set the latest info first and the oldest last. (LIFO)
                'For i As Integer = eventLogApp.Entries.Count - 1 To 0 Step -1

                '    If myInitalDate = DateTime.MinValue Then
                '        myInitalDate = eventLogApp.Entries(i).TimeGenerated
                '        myInitalDate = myInitalDate.AddDays(-pNumberOfDays) 'Add the amount of days we want to save
                '    End If
                '    If eventLogApp.Entries(i).TimeGenerated > myInitalDate.Date Then
                '        sb.AppendLine("Event Number:" & eventCntr)
                '        sb.AppendLine("Entry Type: " & eventLogApp.Entries(i).EntryType.ToString)
                '        sb.AppendLine("Time Generated: " & eventLogApp.Entries(i).TimeGenerated.ToString)
                '        sb.AppendLine("Source: " & eventLogApp.Entries(i).Source.ToString)
                '        sb.AppendLine("Category: " & eventLogApp.Entries(i).Category.ToString)
                '        sb.AppendLine("Event ID: " & eventLogApp.Entries(i).EventID.ToString)
                '        sb.AppendLine("Machine Name: " & eventLogApp.Entries(i).MachineName.ToString)
                '        sb.AppendLine("Message: " & eventLogApp.Entries(i).Message.ToString)
                '        sb.AppendLine("---------------------------------------------------------------------------------")
                '        sb.AppendLine()
                '    Else
                '        Exit For
                '    End If

                '    eventCntr = eventCntr + 1
                'Next
                'validate if directory exist before saving the file.
                'If Directory.Exists(pFilePath) Then
                '    'Save the file on the recived Path with the name and set the extention (txt)
                '    Using outfile As New StreamWriter(pFilePath & "\" & pFileName & ".txt")
                '        outfile.Write(sb.ToString())
                '    End Using
                'End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.SaveSynapseEventLog", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        Private Declare Function BackupEventLog Lib "advapi32.dll" Alias "BackupEventLogA" (ByVal hEventLog As IntPtr, ByVal lpBackupFileName As String) As Integer
        Private Declare Function CloseEventLog Lib "advapi32.dll" (ByVal hEventLog As IntPtr) As IntPtr
        Private Declare Function OpenEventLog Lib "advapi32.dll" Alias "OpenEventLogA" (ByVal lpUNCServerName As String, ByVal lpSourceName As String) As IntPtr


        ''' <summary>
        ''' Whrite on the machine registy the LIS Trace level.
        ''' </summary>
        ''' <param name="pTraceLevel"></param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 24/05/2013</remarks>
        Public Function SetLISTraceLevel(pTraceLevel As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim TraceLevelValue As New Object

                'Set the hexadecimal value. 
                Select Case pTraceLevel
                    Case "NONE"
                        TraceLevelValue = &H0
                        Exit Select
                    Case "LOW"
                        TraceLevelValue = &H1000
                        Exit Select
                    Case "MEDIUM"
                        TraceLevelValue = &H1001
                        Exit Select
                    Case "HIGH"
                        TraceLevelValue = &H1005
                        Exit Select
                End Select

                Dim myRegistryPath As String = String.Empty
                'Get the system base 32bit or 64bit
                If Environment.Is64BitOperatingSystem Then
                    myRegistryPath = "SOFTWARE\Wow6432Node\NTE\communication\3"
                Else
                    myRegistryPath = "SOFTWARE\NTE\communication\3"
                End If

                Dim regKey As RegistryKey
                regKey = Registry.LocalMachine.OpenSubKey(myRegistryPath, True)

                If regKey.GetValue("traceEnable") Is Nothing Then
                    'If don't exist then create it and set the value.
                    regKey.SetValue("traceEnable", TraceLevelValue, RegistryValueKind.DWord)
                Else
                    'Set the value only
                    regKey.SetValue("traceEnable", TraceLevelValue)
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.SetLISTraceLevel", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the LIS Trace level from registry.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 24/05/2013
        ''' </remarks>
        Public Function GetLISTraceLevel() As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim TraceLevelValue As String = String.Empty
                Dim myRegistryPath As String = String.Empty

                'Get the system base 32bit or 64bit
                If Environment.Is64BitOperatingSystem Then
                    myRegistryPath = "SOFTWARE\Wow6432Node\NTE\communication\3"
                Else
                    myRegistryPath = "SOFTWARE\NTE\communication\3"
                End If

                Dim regKey As RegistryKey
                regKey = Registry.LocalMachine.OpenSubKey(myRegistryPath, True)

                Dim RegTraceLevel As Object = 0

                If Not (regKey.GetValue("traceEnable") Is Nothing) Then
                    RegTraceLevel = regKey.GetValue("traceEnable")
                End If

                'Get the integer value. 
                Select Case CInt(RegTraceLevel)
                    Case 0
                        TraceLevelValue = "NONE"
                        Exit Select
                    Case 4096
                        TraceLevelValue = "LOW"
                        Exit Select
                    Case 4097
                        TraceLevelValue = "MEDIUM"
                        Exit Select
                    Case 4101
                        TraceLevelValue = "HIGH"
                        Exit Select
                End Select
                myGlobalDataTO.SetDatos = TraceLevelValue
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utilities.GetLISTraceLevel", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function



#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Returns  byte array codes
        ''' 
        ''' Adapted from iPRO (Utils.AscB)
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks>Created by AG 22/04/2010 (Tested PENDING)</remarks>
        Private Function AscB(ByVal value As String) As Byte()
            Dim ae As New System.Text.ASCIIEncoding
            Dim b() As Byte

            Try
                b = ae.GetBytes(value)
                AscB = b

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utils.AscB", EventLogEntryType.Error, False)
                Throw ex
            End Try

        End Function

        ''' <summary>
        ''' Convert the first byte depending if we are sending consecutive frames
        ''' or if we are receiving consecutive frames
        ''' 
        ''' (Adapted from iPRO - Utils.GenerarSignoTrama)
        ''' </summary>
        ''' <param name="FirstByte"></param>
        ''' <param name="mode"></param>
        ''' <returns>A byte</returns>
        ''' <remarks>Created by AG 22/04/2010 (Tested PENDING)</remarks>
        Private Function GenerateSignFrame(ByVal FirstByte As Byte, ByVal mode As Integer) As Byte

            Dim txMoreFrames As Boolean = False
            Dim rxMoreFrames As Boolean = False

            '// Miramos si entramos en esta función para enviar una trama o porque hemos recibido una
            '// Para generar trama

            Try
                If mode = 0 Then    'Send
                    'If no sent more consecutive frame set the higher bit to 1
                    If txMoreFrames = False Then
                        FirstByte = CByte(FirstByte Or &H80)
                    End If


                Else    'Receive
                    'Check if we will receive more consecutive frames
                    If (FirstByte And &H80) = &H80 Then
                        'Reset the higher bit to 0
                        FirstByte = CByte(FirstByte Xor &H80)
                        rxMoreFrames = False

                    Else
                        rxMoreFrames = True
                    End If

                End If

                GenerateSignFrame = FirstByte

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "Utils.GenerateSignFrame", EventLogEntryType.Error, False)

                Throw ex
            End Try

        End Function

#End Region

#Region "Task Bar management"

        Private Structure APPBARDATA 'AppBar Structure
            Dim cbSize As Integer
            Dim hwnd As Integer
            Dim uCallbackMessage As Integer
            Dim uEdge As Integer
            Dim rc As System.Drawing.Rectangle
            Dim lParam As Integer
        End Structure

#Region "Public TaskBar Properties"

        Public Property TaskBarState() As TaskBarStates
            Get
                Dim myTaskBarState As TaskBarStates
                Try
                    Dim AppBarSetting As New APPBARDATA 'What Setting?
                    AppBarSetting.cbSize = Marshal.SizeOf(AppBarSetting) 'Initialise
                    myTaskBarState = CType(GetAppBarMessage(ABM_GETSTATE, AppBarSetting), TaskBarStates)
                Catch ex As Exception
                    Throw ex
                End Try
                Return myTaskBarState
            End Get
            Set(ByVal value As TaskBarStates)
                Dim AppBarSetting As New APPBARDATA 'Setting We Want To Apply
                AppBarSetting.cbSize = Marshal.SizeOf(AppBarSetting) 'Initialise

                AppBarSetting.lParam = CInt(value)
                SetAppBarMessage(ABM_SETSTATE, AppBarSetting)

            End Set
        End Property

        Public WriteOnly Property TaskBarReset() As Boolean
            Set(ByVal value As Boolean)
                Dim AppBarSetting As New APPBARDATA 'Setting We Want To Apply
                AppBarSetting.cbSize = Marshal.SizeOf(AppBarSetting) 'Initialise

                If value Then
                    AppBarSetting.lParam = CInt(TaskBarStates.NO_SETTINGS)
                    SetAppBarMessage(ABM_SETSTATE, AppBarSetting)
                End If
            End Set
        End Property

        Public WriteOnly Property TaskBarAutoHide() As Boolean
            Set(ByVal value As Boolean)
                Try
                    Dim AppBarSetting As New APPBARDATA 'Setting We Want To Apply
                    AppBarSetting.cbSize = Marshal.SizeOf(AppBarSetting) 'Initialise

                    If value Then
                        Select Case TaskBarState
                            Case TaskBarStates.NO_SETTINGS : AppBarSetting.lParam = CInt(TaskBarStates.AUTOHIDE)
                            Case TaskBarStates.SHOW_CLOCK : AppBarSetting.lParam = CInt(TaskBarStates.AUTOHIDE_CLOCK)
                            Case TaskBarStates.ALWAYSONTOP : AppBarSetting.lParam = CInt(TaskBarStates.AUTOHIDE_ALWAYSONTOP)
                            Case TaskBarStates.ALWAYSONTOP_CLOCK : AppBarSetting.lParam = CInt(TaskBarStates.AUTOHIDE_ALWAYSONTOP_CLOCK)
                            Case Else : AppBarSetting.cbSize = 0
                        End Select
                    Else
                        Select Case TaskBarState
                            Case TaskBarStates.AUTOHIDE : AppBarSetting.lParam = CInt(TaskBarStates.NO_SETTINGS)
                            Case TaskBarStates.AUTOHIDE_CLOCK : AppBarSetting.lParam = CInt(TaskBarStates.SHOW_CLOCK)
                            Case TaskBarStates.AUTOHIDE_ALWAYSONTOP : AppBarSetting.lParam = CInt(TaskBarStates.ALWAYSONTOP)
                            Case TaskBarStates.AUTOHIDE_ALWAYSONTOP_CLOCK : AppBarSetting.lParam = CInt(TaskBarStates.ALWAYSONTOP_CLOCK)
                            Case Else : AppBarSetting.cbSize = 0
                        End Select
                    End If

                    If AppBarSetting.cbSize > 0 Then
                        SetAppBarMessage(ABM_SETSTATE, AppBarSetting)
                    End If

                Catch ex As Exception
                    Throw ex
                End Try
            End Set
        End Property

        Public WriteOnly Property TaskBarAlwaysOnTop() As Boolean
            Set(ByVal value As Boolean)
                Try
                    Dim AppBarSetting As New APPBARDATA 'Setting We Want To Apply
                    AppBarSetting.cbSize = Marshal.SizeOf(AppBarSetting) 'Initialise

                    If value Then
                        Select Case TaskBarState
                            Case TaskBarStates.NO_SETTINGS : AppBarSetting.lParam = CInt(TaskBarStates.ALWAYSONTOP)
                            Case TaskBarStates.SHOW_CLOCK : AppBarSetting.lParam = CInt(TaskBarStates.ALWAYSONTOP_CLOCK)
                            Case TaskBarStates.AUTOHIDE : AppBarSetting.lParam = CInt(TaskBarStates.AUTOHIDE_ALWAYSONTOP)
                            Case TaskBarStates.AUTOHIDE_CLOCK : AppBarSetting.lParam = CInt(TaskBarStates.AUTOHIDE_ALWAYSONTOP_CLOCK)
                            Case Else : AppBarSetting.cbSize = 0
                        End Select
                    Else
                        Select Case TaskBarState
                            Case TaskBarStates.ALWAYSONTOP : AppBarSetting.lParam = CInt(TaskBarStates.NO_SETTINGS)
                            Case TaskBarStates.ALWAYSONTOP_CLOCK : AppBarSetting.lParam = CInt(TaskBarStates.SHOW_CLOCK)
                            Case TaskBarStates.AUTOHIDE_ALWAYSONTOP : AppBarSetting.lParam = CInt(TaskBarStates.AUTOHIDE)
                            Case TaskBarStates.AUTOHIDE_ALWAYSONTOP_CLOCK : AppBarSetting.lParam = CInt(TaskBarStates.AUTOHIDE_CLOCK)
                            Case Else : AppBarSetting.cbSize = 0
                        End Select

                    End If

                    If AppBarSetting.cbSize > 0 Then
                        SetAppBarMessage(ABM_SETSTATE, AppBarSetting)
                    End If

                Catch ex As Exception
                    Throw ex
                End Try
            End Set
        End Property




#End Region

#End Region

#Region "CRC32 Calculation"


        ''' <summary>
        ''' CRC32 table
        ''' </summary>
        ''' <remarks>SGM 29/05/2012</remarks>
        Private CrcTable() As UInt32 = {&H0, &H4C11DB7, &H9823B6E, &HD4326D9, _
                                            &H130476DC, &H17C56B6B, &H1A864DB2, &H1E475005, _
                                            &H2608EDB8, &H22C9F00F, &H2F8AD6D6, &H2B4BCB61, _
                                            &H350C9B64, &H31CD86D3, &H3C8EA00A, &H384FBDBD}

        ''' <summary>
        ''' Mask with Polinomial
        ''' </summary>
        ''' <param name="pCRC"></param>
        ''' <param name="pData"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 29/05/2012</remarks>
        Private Function CRC32Fast(ByVal pCRC As UInt32, ByVal pData As UInt32) As UInt32

            Try

                pCRC = pCRC Xor pData 'Apply all 32-bits

                'Process 32-bits, 4 at a time, or 8 rounds

                pCRC = (pCRC << 4) Xor MyClass.CrcTable(CInt(pCRC >> 28)) ' Assumes 32-bit reg, masking index to 4-bits
                pCRC = (pCRC << 4) Xor MyClass.CrcTable(CInt(pCRC >> 28)) '  0x04C11DB7 Polynomial used in STM32
                pCRC = (pCRC << 4) Xor MyClass.CrcTable(CInt(pCRC >> 28))
                pCRC = (pCRC << 4) Xor MyClass.CrcTable(CInt(pCRC >> 28))
                pCRC = (pCRC << 4) Xor MyClass.CrcTable(CInt(pCRC >> 28))
                pCRC = (pCRC << 4) Xor MyClass.CrcTable(CInt(pCRC >> 28))
                pCRC = (pCRC << 4) Xor MyClass.CrcTable(CInt(pCRC >> 28))
                pCRC = (pCRC << 4) Xor MyClass.CrcTable(CInt(pCRC >> 28))

                Return pCRC

            Catch ex As Exception
                Throw ex
            End Try
        End Function

        ''' <summary>
        ''' Returns the CRC32 value of the informed Byte array
        ''' </summary>
        ''' <param name="pBytes"></param>
        ''' <returns></returns>
        ''' <remarks>SGM 29/05/2012</remarks>
        Public Function CalculateCRC32(ByVal pBytes As Byte()) As GlobalDataTO

            Dim myGlobal As New GlobalDataTO
            Dim myBytes As Byte() = pBytes
            Dim myCRCResult As UInt32 = &HFFFFFFFFUI

            Try

                Dim counter As Long = 0
                Dim DataHighHigh As UInt32
                Dim DataMediumHigh As UInt32
                Dim DataMediumLow As UInt32
                Dim DataLowLow As UInt32
                Dim FinalData As UInt32

                Dim index As Integer = 0
                While index < myBytes.Length
                    If index < myBytes.Length Then
                        DataHighHigh = myBytes(index)
                        index += 1
                        If index < myBytes.Length Then
                            DataMediumHigh = myBytes(index)
                            index += 1
                            If index < myBytes.Length Then
                                DataMediumLow = myBytes(index)
                                index += 1
                                If index < myBytes.Length Then
                                    DataLowLow = myBytes(index)
                                    index += 1

                                    FinalData = (DataLowLow << 24) Or (DataMediumLow << 16) Or (DataMediumHigh << 8) Or (DataHighHigh) '// El micro de st tracta la posicio del bytes al reves

                                    'FinalData = Convert.ToInt32((DataLowLow << 24) Or (DataMediumLow << 16) Or (DataMediumHigh << 8) Or (DataHighHigh)) '// El micro de st tracta la posicio del bytes al reves

                                    myCRCResult = MyClass.CRC32Fast(myCRCResult, FinalData) '// Per calcular el CRC igual que el STM32

                                End If
                            End If
                        End If
                    End If
                End While

                myGlobal.SetDatos = myCRCResult

                'If myCRCResult <> &HFFFFFFFFUI Then
                '    MyClass.CRC32DecimalAttr = myCRCResult
                '    MyClass.CRC32HexAttr = MyClass.ConvertUint32ToHex(MyClass.CRC32DecimalAttr)
                'Else
                '    MyClass.CRC32DecimalAttr = 0
                '    MyClass.CRC32HexAttr = "0x00000000"
                'End If

            Catch ex As Exception
                Throw ex
            End Try

            Return myGlobal

        End Function

#End Region

#Region "FOR TESTING"

        Public Shared Sub GetExceptionData(ByVal ex As Exception, ByRef pModule As String, ByRef pMethod As String, ByRef pLine As Integer, ByRef pColumn As Integer)
            If ex IsNot Nothing Then
                Dim ExTrace As System.Diagnostics.StackTrace = New System.Diagnostics.StackTrace(ex, True)
                If ExTrace.GetFrames.Count > 0 Then
                    pModule = ExTrace.GetFrame(0).GetMethod.Module.Name
                    pMethod = ExTrace.GetFrame(0).GetMethod.Name
                    pLine = ExTrace.GetFrame(0).GetFileLineNumber()
                    pColumn = ExTrace.GetFrame(0).GetFileColumnNumber()
                End If
            End If
        End Sub

#End Region

    End Class
End Namespace
