Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Windows.Forms
Imports System.IO

Public Class FwAdjustmentsDelegate

#Region "Declarations"
    Private myAdjustmentsDS As SRVAdjustmentsDS

    'SGM 28/11/2011
    Public Shared AdjustmentDecryptedFileExtension As String = ".tmp"
    Public Shared AdjustmentCryptedFileExtension As String = ".adj"
    Public Shared FirmwareFileExtension As String = ".fmw"
    'SGM 28/11/2011

#End Region

#Region "Attributes"

#End Region

#Region "Properties"

#End Region

#Region "Constructor"
    Public Sub New(ByRef pAdjustmentsDS As SRVAdjustmentsDS)
        myAdjustmentsDS = pAdjustmentsDS
    End Sub
#End Region

#Region "Methods"
    ''' <summary>
    ''' Gets a copy of Adjustment values from the Adjustments Dataset
    ''' </summary>
    ''' <param name="myAdjustmentsDS"></param>
    ''' <returns></returns>
    ''' <remarks>Created by XBC 01/02/2011</remarks>
    Public Function Clone(ByVal myAdjustmentsDS As SRVAdjustmentsDS) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            Dim myResultDS As New SRVAdjustmentsDS
            Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow
            For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows
                myNewRow = myResultDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
                With myNewRow
                    .AnalyzerID = R.AnalyzerID
                    .FwVersion = R.FwVersion
                    .GroupID = R.GroupID
                    .CodeFw = R.CodeFw
                    .Value = R.Value
                    .AreaFw = R.AreaFw
                    .DescriptionFw = R.DescriptionFw
                    .AxisID = R.AxisID
                    .CanSave = R.CanSave
                    .CanMove = R.CanMove
                    .InFile = R.InFile
                End With
                myResultDS.srv_tfmwAdjustments.Addsrv_tfmwAdjustmentsRow(myNewRow)
            Next

            myResultDS.AnalyzerModel = myAdjustmentsDS.AnalyzerModel
            myResultDS.AnalyzerID = myAdjustmentsDS.AnalyzerID
            myResultDS.FirmwareVersion = myAdjustmentsDS.FirmwareVersion
            myResultDS.ReadedDatetime = myAdjustmentsDS.ReadedDatetime

            myResultDS.AcceptChanges()

            resultData.SetDatos = myResultDS

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.Clone", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 18/01/2011</remarks>
    Public Function MakeAdjustmentsFileHeader() As String
        Dim myHeader As String = ""
        Try

            Dim myReadedDatetime As String = ""
            If myAdjustmentsDS.ReadedDatetime <> Nothing Then myReadedDatetime = myAdjustmentsDS.ReadedDatetime.ToString("hh:mm:ss yyyy/MM/dd")

            myHeader &= "--Adjustments: " & vbTab & myAdjustmentsDS.AnalyzerModel & " - SN" & myAdjustmentsDS.AnalyzerID & vbCrLf
            myHeader &= "--Fw version: " & vbTab & myAdjustmentsDS.FirmwareVersion & vbCrLf
            myHeader &= "--Date & Time: " & vbTab & myReadedDatetime & vbCrLf
            myHeader &= "---------------------------------------------------------------------------------" & vbCrLf & vbCrLf
        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.MakeAdjustmentsFileHeader", EventLogEntryType.Error, False)
        End Try
        Return myHeader
    End Function

    ''' <summary>
    ''' Export the Adjustments DS to the file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 18/01/2011</remarks>'QUITAR pFwVersion!!!
    Public Function ExportDSToFile(ByVal pAnalyzerSN As String, Optional ByVal pPath As String = "", _
                                   Optional ByVal pIsUserPath As Boolean = False, _
                                   Optional ByVal pEncrypt As Boolean = False) As GlobalDataTO

        Dim resultData As New GlobalDataTO
        Dim myStreamWriter As StreamWriter = Nothing

        Try


            resultData = MyClass.ConvertDSToString()
            If Not resultData.HasError And resultData.SetDatos IsNot Nothing Then
                Dim myText As String = CType(resultData.SetDatos, String)

                Dim myGlobalbase As New GlobalBase
                Dim myPath As String

                Dim myHeader As String = MyClass.MakeAdjustmentsFileHeader()

                myText = myHeader & myText

                If pIsUserPath Then

                    myPath = pPath

                Else
                    If pAnalyzerSN.Length > 0 Then
                        Dim myDir As String = Application.StartupPath & GlobalBase.FwAdjustmentsPath  'SGM 24/11/2011
                        myPath = myDir & "Adj_" & pAnalyzerSN & ".txt"
                    Else
                        If pPath.Length > 0 Then
                            myPath = pPath
                        Else
                            myPath = Application.StartupPath & GlobalBase.FwAdjustmentsPath & "FwAdjustments.txt"
                        End If
                    End If
                End If

                If Not Directory.Exists(Directory.GetParent(myPath).FullName) Then
                    Directory.CreateDirectory(Directory.GetParent(myPath).FullName)
                End If
                myStreamWriter = File.CreateText(myPath)
                myStreamWriter.Write(myText)
                myStreamWriter.Close()

                If pEncrypt Then
                    resultData = MyClass.EncryptAdjustmentsFile(myPath)
                End If

                resultData.SetDatos = myPath

            End If

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.ExportDSToFile", EventLogEntryType.Error, False)
        End Try

        If myStreamWriter IsNot Nothing Then
            myStreamWriter.Dispose()
            myStreamWriter = Nothing
        End If

        Return resultData

    End Function

    ''' <summary>
    ''' </summary>
    ''' <param name="pTextPath"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 29/11/2011</remarks>
    Public Function EncryptAdjustmentsFile(ByVal pTextPath As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        Dim myUtil As New Utilities

        Try

            'Encrypt SGM 29/11/2011
            If File.Exists(pTextPath) Then
                Dim myCryptFilePath As String
                Dim FileName As String = New FileInfo(pTextPath).Name
                Dim myDirPath As String = New FileInfo(pTextPath).DirectoryName
                If Not myDirPath.EndsWith("\") Then myDirPath &= "\"
                myCryptFilePath = myDirPath & FileName.Replace(FwAdjustmentsDelegate.AdjustmentDecryptedFileExtension, FwAdjustmentsDelegate.AdjustmentCryptedFileExtension)

                If File.Exists(pTextPath) Then

                    myGlobal = myUtil.EncryptFile(pTextPath, myCryptFilePath)
                Else
                    Throw New FileNotFoundException
                End If

                myGlobal.SetDatos = myCryptFilePath
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.EncryptAdjustmentsFile", EventLogEntryType.Error, False)
        End Try

        Return myGlobal

    End Function

    Public Function DecryptAdjustmentsFile(ByVal pCryptFilePath As String) As GlobalDataTO

        Dim myGlobal As New GlobalDataTO
        'Dim objReader As System.IO.StreamReader
        Dim myUtil As New Utilities

        Try
            'Dim myData As String
            Dim FileName As String = New FileInfo(pCryptFilePath).Name
            Dim myTempDirPath As String = New FileInfo(pCryptFilePath).DirectoryName
            If Not myTempDirPath.EndsWith("\") Then myTempDirPath &= "\"
            Dim myDecryptFilePath As String = myTempDirPath & FileName.Replace(FwAdjustmentsDelegate.AdjustmentCryptedFileExtension, FwAdjustmentsDelegate.AdjustmentDecryptedFileExtension)
            myGlobal = myUtil.DecryptFile(pCryptFilePath, myDecryptFilePath)

            If Not myGlobal.HasError And myGlobal.SetDatos IsNot Nothing Then
                If Not File.Exists(myDecryptFilePath) Then
                    Throw New FileNotFoundException
                End If
            End If

        Catch ex As Exception
            myGlobal.HasError = True
            myGlobal.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobal.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.DecryptAdjustmentsFile", EventLogEntryType.Error, False)
        End Try

        'If objReader IsNot Nothing Then
        '    objReader.Dispose()
        '    objReader = Nothing
        'End If

        Return myGlobal
    End Function




    ''' <summary>
    ''' Converts the DataSet to string format
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 18/01/2011</remarks>
    Public Function ConvertDSToString() As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            'first collect all the areas
            Dim myAreas As New List(Of String)
            For Each L As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows
                If L.InFile Then
                    If Not myAreas.Contains(L.AreaFw.Trim) Then
                        myAreas.Add(L.AreaFw.Trim)
                    End If
                End If
            Next

            Dim myText As String = ""
            Dim myValue As String       ' XBC 21/10/2011
            For Each A As String In myAreas
                myText &= vbCrLf & "{" & A.Trim & "}" & vbCrLf & vbCrLf
                For Each L As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows
                    If L.InFile Then
                        If A.Trim = L.AreaFw.Trim Then

                            ' XBC 21/10/2011
                            'myText &= L.CodeFw.Trim & ":" & L.Value.Trim & ";"

                            myValue = L.Value

                            'resultData = PCInfoReader.GetOSCultureInfo()
                            'Dim OSCultureInfo As New PCInfoReader.AX00PCOSCultureInfo
                            'If Not resultData.HasError And Not resultData Is Nothing Then
                            '    OSCultureInfo = CType(resultData.SetDatos, PCInfoReader.AX00PCOSCultureInfo)
                            '    myValue = myValue.Replace(OSCultureInfo.DecimalSeparator, ",")
                            'End If

                            'RH 15/02/2012 Get the static version of DecimalSeparator
                            myValue = myValue.Replace(SystemInfoManager.OSDecimalSeparator, ",")

                            myText &= L.CodeFw.Trim & ":" & myValue.Trim & ";"
                            ' XBC 21/10/2011

                            If L.DescriptionFw.Trim.Length > 0 Then
                                ' XBC 21/10/2011
                                'Dim myLine As String = L.CodeFw.Trim & ":" & L.Value.Trim
                                Dim myLine As String = L.CodeFw.Trim & ":" & myValue.Trim
                                ' XBC 21/10/2011
                                While myLine.Length < 12
                                    myLine &= " "
                                    myText &= " "
                                End While
                                myText &= "{" & L.DescriptionFw & "}" & vbCrLf
                            End If
                        End If
                    End If
                Next
            Next

            resultData.SetDatos = myText

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.ConvertDSToString", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Reads Adjustment value from the Adjustments Dataset
    ''' </summary>
    ''' <param name="pCodeFw"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SG 18/01/2011</remarks>
    Public Function ReadAdjustmentValueByCode(ByVal pCodeFw As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows
                If R.CodeFw.Trim = pCodeFw.Trim Then
                    resultData.SetDatos = R.Value
                    Exit For
                End If
            Next

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.ReadAdjustmentValueByCode", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Reads Adjustment value from the Adjustments Dataset
    ''' </summary>
    ''' <param name="pGroupID"></param>
    ''' <param name="pAxis"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SG 18/01/2011</remarks>
    Public Function ReadAdjustmentValueByGroupAndAxis(ByVal pGroupID As String, ByVal pAxis As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows
                If R.GroupID.Trim = pGroupID.Trim And R.AxisID.Trim = pAxis.Trim Then
                    resultData.SetDatos = R.Value
                    Exit For
                End If
            Next

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.ReadAdjustmentValueByGroupAndAxis", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    '''' <summary>
    '''' Reads Adjustment values related to specified Adjustments group from the Adjustments Dataset
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks>Created by SG 18/01/2011</remarks>
    'Public Function ReadAdjustmentsByGroupID(ByVal pGroupID As String) As GlobalDataTO
    '    Dim resultData As New GlobalDataTO

    '    Try
    '        Dim myResultDS As New SRVAdjustmentsDS
    '        Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow

    '        For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows
    '            If R.GroupID.Trim = pGroupID.Trim Then
    '                myNewRow = myResultDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
    '                With myNewRow
    '                    .GroupID = R.GroupID
    '                    .CodeFw = R.CodeFw
    '                    .Value = R.Value
    '                    .AreaFw = R.AreaFw
    '                    .DescriptionFw = R.DescriptionFw
    '                    .AxisID = R.AxisID
    '                    .CanSave = R.CanSave
    '                    .CanMove = R.CanMove
    '                    .InFile = R.InFile
    '                End With
    '                myResultDS.srv_tfmwAdjustments.Addsrv_tfmwAdjustmentsRow(myNewRow)
    '            End If
    '        Next R


    '        myResultDS.AcceptChanges()

    '        resultData.SetDatos = myResultDS

    '    Catch ex As Exception
    '        resultData.HasError = True
    '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        resultData.ErrorMessage = ex.Message

    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.ReadAdjustmentsByGroupID", EventLogEntryType.Error, False)
    '    End Try
    '    Return resultData
    'End Function

    ''' <summary>
    ''' Reads Adjustment values related to specified Adjustments groups from the Adjustments Dataset
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 18/01/2011</remarks>
    Public Function ReadAdjustmentsByGroupIDs(ByVal pGroupIDs As List(Of String)) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            Dim myResultDS As New SRVAdjustmentsDS
            Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow

            For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows
                For Each G As String In pGroupIDs
                    If R.GroupID.Trim = G.Trim Then
                        myNewRow = myResultDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow
                        With myNewRow
                            .AnalyzerID = R.AnalyzerID
                            .FwVersion = R.FwVersion
                            .GroupID = R.GroupID
                            .CodeFw = R.CodeFw
                            .Value = R.Value
                            .AreaFw = R.AreaFw
                            .DescriptionFw = R.DescriptionFw
                            .AxisID = R.AxisID
                            .CanSave = R.CanSave
                            .CanMove = R.CanMove
                            .InFile = R.InFile
                        End With
                        myResultDS.srv_tfmwAdjustments.Addsrv_tfmwAdjustmentsRow(myNewRow)
                    End If
                Next G
            Next R


            myResultDS.AcceptChanges()

            resultData.SetDatos = myResultDS

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.ReadAdjustmentsByGroupIDs", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function


    ''' <summary>
    ''' Updates the adjustments dataset
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 18/01/2011</remarks>
    Public Function UpdateAdjustments(ByVal pAdjustmentsDS As SRVAdjustmentsDS, _
                                      ByVal pAnalyzerID As String) As GlobalDataTO

        Dim resultData As New GlobalDataTO
        Dim CopyOfAdjustmentsDS As SRVAdjustmentsDS = myAdjustmentsDS
        Try
            Dim myFwVersion As String = ""
            For Each SR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In pAdjustmentsDS.srv_tfmwAdjustments.Rows
                For Each GR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows
                    If SR.CodeFw.Trim = GR.CodeFw.Trim Then
                        GR.Value = SR.Value
                        If myFwVersion.Length = 0 Then
                            myFwVersion = SR.FwVersion
                        End If
                    End If
                Next
            Next

            myAdjustmentsDS.AcceptChanges()

            'export the updated data to the external file
            resultData = MyClass.ExportDSToFile(pAnalyzerID)
            If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                resultData.SetDatos = myAdjustmentsDS
            End If

        Catch ex As Exception

            myAdjustmentsDS = CopyOfAdjustmentsDS

            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.UpdateAdjustments", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Updates the Firmware Version
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Created by SG 30/05/2012</remarks>
    Public Function UpdateFwVersion(ByVal pAdjustmentsDS As SRVAdjustmentsDS, _
                                      ByVal pAnalyzerID As String, ByVal pFwVersion As String) As GlobalDataTO

        Dim resultData As New GlobalDataTO

        Try

            For Each SR As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In pAdjustmentsDS.srv_tfmwAdjustments.Rows
                SR.BeginEdit()
                SR.FwVersion = pFwVersion
                SR.EndEdit()
            Next

            pAdjustmentsDS.AcceptChanges()

            resultData.SetDatos = pAdjustmentsDS
            
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.UpdateFwVersion", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Converts all Adjustments data to the Adjustments DS
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by SG 18/01/2011
    ''' Modified by XBC 03/05/2011 - ANSADJ just defined (function overloaded)
    ''' </remarks>
    Public Function ConvertReceivedDataToDS(ByVal pAdjustmentsData As InstructionParameterTO) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim CopyOfAdjustmentsDS As SRVAdjustmentsDS = myAdjustmentsDS
        Try
            'code
            Dim myAdjustmentCode As String = pAdjustmentsData.Parameter
            'value
            Dim myAdjustmentValue As String = pAdjustmentsData.ParameterValue

            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                In myAdjustmentsDS.srv_tfmwAdjustments _
                                Where a.CodeFw = myAdjustmentCode _
                                Select a).ToList

            If myAdjustmentRows.Count > 0 Then
                Dim myAdjustmentRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow = myAdjustmentRows(0)
                With myAdjustmentRow
                    .Value = myAdjustmentValue
                End With
            End If

            myAdjustmentsDS.AcceptChanges()

            resultData.SetDatos = myAdjustmentsDS

        Catch ex As Exception

            myAdjustmentsDS = CopyOfAdjustmentsDS

            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.ConvertReceivedDataToDS", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Converts String Adjustments to a Adjustments Dataset
    ''' </summary>
    ''' <param name="pAdjustmentsData"></param>
    ''' <param name="pAnalyzerID"></param>
    ''' <param name="pAnalyzerModel"></param>
    ''' <param name="pFwVersion"></param>
    ''' <returns></returns>
    ''' <remarks>Created by SGM 15/12/2011</remarks>
    Public Function ConvertStringDataToDS(ByVal pAdjustmentsData As InstructionParameterTO, ByVal pAnalyzerID As String, ByVal pAnalyzerModel As String, ByVal pFwVersion As String) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim CopyOfAdjustmentsDS As SRVAdjustmentsDS = myAdjustmentsDS
        Try
            'code
            Dim myAdjustmentCode As String = pAdjustmentsData.Parameter
            'value
            Dim myAdjustmentValue As String = pAdjustmentsData.ParameterValue

            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                In myAdjustmentsDS.srv_tfmwAdjustments _
                                Where a.CodeFw = myAdjustmentCode _
                                Select a).ToList

            If myAdjustmentRows.Count > 0 Then
                Dim myAdjustmentRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow = myAdjustmentRows(0)
                With myAdjustmentRow
                    .AnalyzerID = pAnalyzerID
                    .FwVersion = pFwVersion
                    .Value = myAdjustmentValue
                End With
            End If

            myAdjustmentsDS.AcceptChanges()

            'SGM 05/12/2011
            Dim myAnalyzerModel As String
            If pAnalyzerModel.Length > 0 Then
                myAnalyzerModel = pAnalyzerModel
            Else
                myAnalyzerModel = "Unknown"
            End If

            Dim myAnalyzerID As String
            If pAnalyzerID.Length > 0 Then
                myAnalyzerID = pAnalyzerID
            Else
                myAnalyzerID = "Unknown"
            End If

            myAdjustmentsDS.AnalyzerModel = myAnalyzerModel
            myAdjustmentsDS.AnalyzerID = myAnalyzerID
            myAdjustmentsDS.FirmwareVersion = pFwVersion
            myAdjustmentsDS.ReadedDatetime = DateTime.Now
            'end SGM 05/12/2011

            resultData.SetDatos = myAdjustmentsDS

        Catch ex As Exception

            myAdjustmentsDS = CopyOfAdjustmentsDS

            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.ConvertStringDataToDS", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function


    '''' <summary>
    '''' Converts all Adjustments data to the Adjustments DS
    '''' </summary>
    '''' <returns></returns>
    '''' <remarks>Created by SG 18/01/2011</remarks>
    'Public Function ConvertReceivedDataToDS(ByVal pAdjustmentsData As String) As GlobalDataTO
    '    Dim resultData As New GlobalDataTO
    '    Dim CopyOfAdjustmentsDS As SRVAdjustmentsDS = myAdjustmentsDS
    '    Dim myUtil As New Utilities
    '    Try

    '        resultData = myUtil.ConvertAdjustmentsTextToDS(pAdjustmentsData)

    '        If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then

    '            myAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)

    '        End If


    '    Catch ex As Exception

    '        myAdjustmentsDS = CopyOfAdjustmentsDS

    '        resultData.HasError = True
    '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
    '        resultData.ErrorMessage = ex.Message

    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.ConvertReceivedDataToDS", EventLogEntryType.Error, False)
    '    End Try
    '    Return resultData
    'End Function


    ''' <summary>
    ''' Converts all Adjustments data to the Adjustments DS
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by  SG 18/01/2011
    ''' Modified by XB 01/02/2013 - Upper conversions must use Invariant Culture Info (Bugs tracking #1112)
    ''' </remarks>
    Public Function ConvertReceivedDataToDS(ByVal pAdjustmentsData As String, ByVal pAnalyzerID As String, ByVal pFwVersion As String, Optional ByVal pExternalDS As SRVAdjustmentsDS = Nothing) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim CopyOfAdjustmentsDS As SRVAdjustmentsDS

        Try

            'SGM 01/11/2011
            resultData = MyClass.Clone(MyClass.myAdjustmentsDS)
            If Not resultData.HasError And resultData.SetDatos IsNot Nothing Then
                CopyOfAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)


                If pAdjustmentsData.Length > 0 Then

                    'split into lines
                    Dim myLines() As String = pAdjustmentsData.Split(vbCrLf.ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                    Dim myAdjustmentArea As String = ""

                    If myLines.Length = 1 Then
                        Dim myAdjs() As String = myLines(0).Split(";".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                        For i As Integer = 0 To myAdjs.Length - 1 Step 1
                            Dim LabelValue() As String = myAdjs(i).Split(":".ToCharArray, StringSplitOptions.RemoveEmptyEntries)
                            If LabelValue.Length = 2 Then
                                Dim AdjLabel As String = LabelValue(0)

                                'If AdjLabel.ToUpper.Trim <> "CHECKAJ" Then
                                If AdjLabel.ToUpperBS.Trim <> "CHECKAJ" Then

                                    Dim AdjValue As String = LabelValue(1)

                                    If Not IsNumeric(AdjValue) Then
                                        Throw New Exception
                                    End If


                                    Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                                    myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                                        In CopyOfAdjustmentsDS.srv_tfmwAdjustments _
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

                                    CopyOfAdjustmentsDS.AcceptChanges()

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

                                        'If myAdjustmentCode.ToUpper.Trim <> "CHECKAJ" Then
                                        If myAdjustmentCode.ToUpperBS.Trim <> "CHECKAJ" Then

                                            'value
                                            Dim myAdjustmentValue As String = ""
                                            myAdjustmentValue = myAdjustmentLine.Substring(myAdjustmentLine.IndexOf(":") + 1)
                                            myAdjustmentValue = myAdjustmentValue.Substring(0, myAdjustmentValue.IndexOf(";"))

                                            If myAdjustmentValue.Length > 0 Then
                                                If Not IsNumeric(myAdjustmentValue) Then
                                                    Throw New Exception
                                                End If
                                            Else
                                                'PDT empty values are accepted????
                                            End If

                                            'description
                                            Dim myAdjustmentDescription As String = ""
                                            If myAdjustmentLine.Contains("}") Then
                                                myAdjustmentDescription = myAdjustmentLine.Substring(myAdjustmentLine.IndexOf("{") + 1)
                                                myAdjustmentDescription = myAdjustmentDescription.Substring(0, myAdjustmentDescription.IndexOf("}"))
                                            End If

                                            Dim myAdjustmentRows As New List(Of SRVAdjustmentsDS.srv_tfmwAdjustmentsRow)
                                            myAdjustmentRows = (From a As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow _
                                                                In CopyOfAdjustmentsDS.srv_tfmwAdjustments _
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

                                            CopyOfAdjustmentsDS.AcceptChanges()

                                        End If
                                    End If
                                End If
                            End If
                        Next

                    End If

                    'SGM 05/12/2011
                    Dim myAnalyzerID As String = "Unknown"
                    Dim myAnalyzerModel As String = "Unknown"
                    If pAnalyzerID.Length > 0 Then
                        myAnalyzerID = pAnalyzerID
                    End If

                    myAnalyzerModel = "A400" 'PENDING

                    CopyOfAdjustmentsDS.AnalyzerModel = myAnalyzerModel
                    CopyOfAdjustmentsDS.FirmwareVersion = pFwVersion
                    CopyOfAdjustmentsDS.ReadedDatetime = DateTime.Now
                    'end SGM 05/12/2011

                    resultData.SetDatos = CopyOfAdjustmentsDS

                    'SGM 01/11/2011
                    resultData = MyClass.Clone(CopyOfAdjustmentsDS)
                    If Not resultData.HasError And resultData.SetDatos IsNot Nothing Then
                        If pExternalDS IsNot Nothing Then
                            pExternalDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
                        Else
                            MyClass.myAdjustmentsDS = CType(resultData.SetDatos, SRVAdjustmentsDS)
                        End If
                    End If

                Else

                    resultData.SetDatos = Nothing

                End If

            End If


        Catch ex As Exception

            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.ConvertReceivedDataToDS", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function


    ''' <summary>
    ''' Add new rows to current AdjustmentsDS object
    ''' </summary>
    ''' <param name="pNewRow">Row to add</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Created by XBC 27/02/2012
    ''' If row to add already exists just its value is updated
    ''' </remarks>
    Public Function AddNewRowToDS(ByVal pNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Try
            Dim NewValue As Boolean = True
            For Each R As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow In myAdjustmentsDS.srv_tfmwAdjustments.Rows
                If UCase(R.CodeFw.Trim) = UCase(pNewRow.CodeFw) Then
                    ' Already exists
                    NewValue = False
                    R.Value = pNewRow.Value
                    Exit For
                End If
            Next

            If NewValue Then
                ' New row
                Dim myNewRow As SRVAdjustmentsDS.srv_tfmwAdjustmentsRow
                myNewRow = myAdjustmentsDS.srv_tfmwAdjustments.Newsrv_tfmwAdjustmentsRow

                With myNewRow
                    .AnalyzerID = pNewRow.AnalyzerID
                    .FwVersion = pNewRow.FwVersion
                    .GroupID = pNewRow.GroupID
                    .CodeFw = pNewRow.CodeFw
                    .Value = pNewRow.Value
                    .AreaFw = pNewRow.AreaFw
                    .DescriptionFw = pNewRow.DescriptionFw
                    .AxisID = pNewRow.AxisID
                    .CanSave = pNewRow.CanSave
                    .CanMove = pNewRow.CanMove
                    .InFile = pNewRow.InFile
                End With

                myAdjustmentsDS.srv_tfmwAdjustments.Addsrv_tfmwAdjustmentsRow(myNewRow)

                myAdjustmentsDS.AcceptChanges()
            End If

        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "FwAdjustmentsDelegate.AddNewRowToDS", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

#End Region



#Region "Adjustment File Format"
    '    {Brazo Reactivo 1}

    'R1RV:200;   {Posicion referencia - Vertical}
    'R1H:364;    {Posición Parking - Horizontal}
    'R1V:1332;   {REF Posición Parking - Vertical}
    'R1WS:756;   {Estación de Lavado - Horizontal}
    'R1DH:364;   {Dispensación en Rotor - Horizontal}
    'R1DV:1332;  {REF Dispensación en Rotor - Vertical}
    'R1PH1:1460; {Rotor de Reactivos Corona 1 - Horizontal}
    'R1PH2:1460; {Rotor de Reactivos Corona 2 - Horizontal}
    'R1PV1:1900; {Rotor de Reactivos Corona 1 Punto máximo detección - Vertical}
    'R1PV2:1900; {Rotor de Reactivos Corona 2 Punto máximo detección - Vertical}
    'R1PI:1282;  {REF Punto Inicial Detección de nivel - Vertical}

    '{Brazo Reactivo 2}

    'R2RV:200;   {Posicion referencia - Vertical}
    'R2H:364;    {Posición Parking - Horizontal}
    'R2V:1332;   {REF Posición Parking - Vertical}
    'R2WS:1675;  {Estación de Lavado - Horizontal}
    'R2DH:2050;  {Dispensación en Rotor - Horizontal}
    'R2DV:1312;  {REF Dispensación en Rotor - Vertical}
    'R2PH1:1460; {Rotor de Reactivos Corona 1 - Horizontal}
    'R2PV1:1900; {Rotor de Reactivos Corona 1 Punto máximo detección - Vertical}
    'R2PH2:1460; {Rotor de Reactivos Corona 2 - Horizontal}
    'R2PV2:1900; {Rotor de Reactivos Corona 2 Punto máximo detección - Vertical}
    'R2PI:1282;  {REF Punto Inicial Detección de nivel - Vertical}

    '{Brazo Muestra}

    'M1RV:200;    {Posicion referencia - Vertical}
    'M1H:364;     {Posición Parking - Horizontal}
    'M1V:1332;    {REF Posición Parking - Vertical}
    'M1RS:2024;   {Estación de Lavado - Horizontal}
    'M1DH1:2718;  {Dispensación en Rotor Posicion 1 - Horizontal}
    'M1DV1:1382;  {REF Dispensación en Rotor Posicion 1 - Vertical}
    'M1DH2:2718;  {Dispensación en Rotor Posicion 2 - Horizontal}
    'M1DV2:1382;  {REF Dispensación en Rotor Posicion 2 - Vertical}
    'M1ISEH:-305; {Dispensación en ISE - Horizontal}
    'M1ISEV:2086; {Dispensación en ISE - Vertical}
    'M1PH1:1597;  {Rotor de Muestras Corona 1 - Horizontal}
    'M1PV1p:1850; {Rotor de Muestras Corona 1 Punto máximo detección pediátrico - Vertical}
    'M1PV1t:1950; {Rotor de Muestras Corona 1 Punto máximo detección tubo - Vertical}
    'M1PH2:1597;  {Rotor de Muestras Corona 2 - Horizontal}
    'M1PV2p:1850; {Rotor de Muestras Corona 2 Punto máximo detección pediátrico - Vertical}
    'M1PV2t:1950; {Rotor de Muestras Corona 2 Punto máximo detección tubo - Vertical}
    'M1PH3:1597;  {Rotor de Muestras Corona 3 - Horizontal}
    'M1PV3p:1850; {Rotor de Muestras Corona 3 Punto máximo detección pediátrico - Vertical}
    'M1PV3t:1950; {Rotor de Muestras Corona 3 Punto máximo detección tubo - Vertical}
    'M1PI:1560;   {REF Punto Inicial Detección de nivel - Vertical}

    '{Agitador 1}

    'A1RV:200;   {Posicion referencia - Vertical}
    'A1H:364;    {Posición Parking - Horizontal}
    'A1V:1332;   {REF Posición Parking - Vertical}
    'A1WS:1292;  {Estación de Lavado - Horizontal}
    'A1DH:2276;  {Agitación en rotor - Horizontal}
    'A1DV:1721;  {REF Agitación en rotor - Vertical}

    '{Agitador 2}

    'A2RV:200;   {Posicion referencia - Vertical}
    'A2H:364;    {Posición Parking - Horizontal}
    'A2V:1332;   {REF Posición Parking - Vertical}
    'A2WS:166;   {Estación de Lavado - Horizontal}
    'A2DH:1716;  {Agitación en rotor - Horizontal}
    'A2DV:1721;  {REF Agitación en rotor - Vertical}

    '{Rotor Reactivos}

    'RR1P1:324;  {Posición referencia Pipeteo - Pote 1 en Corona 1 Brazo R1} 
    'RR1P2:324;  {Posición referencia Pipeteo - Pote XX en Corona 2 Brazo R1} 
    'RR2P1:324;  {Posición referencia Pipeteo - Pote 1 en Corona 1 Brazo R2} 
    'RR2P2:324;  {Posición referencia Pipeteo - Pote XX en Corona 2 Brazo R2} 
    'RRCB:1236;  {Posición referencia Codigo de Barras - Pote 1 en C.B.}

    '{Rotor Muestras}

    'RM1P1:324;  {Posición referencia Pipeteo - Pocillo  1 en Corona 1} 
    'RM1P2:334;  {Posición referencia Pipeteo - Pocillo xx en Corona 2} 
    'RM1P3:344;  {Posición referencia Pipeteo - Pocillo xx en Corona 3} 
    'RMCB:1236;  {Posición referencia Codigo de Barras - Pocillo 1 en C.B.}

    '{Fotometria}

    'GFWR1:324;   {Posición referencia lectura - Pocillo 1} 

    '{Estación de Lavado}

    'WSRV:500;   {Posicion referencia - Vertical}
    'WSRR:650    {Posicion ready relativa a Posicion referencia - Vertical}
    'WSSV:1570;  {REF Posicion de inicio ciclo lavado - Vertical}
    'WSEV:2235;  {REF Posicion final ciclo lavado - Vertical}

    '{General Equipo - Termos}

    'GTR1:38,0; {Consigna Termo Punta Reactivo 1}
    'GTR2:38,2; {Consigna Termo Punta Reactivo 2}
    'GTN1:4,0;  {Consigna Termo Nevera 1}
    'GTGF:37,2; {Consigna Termo Fotometría}
    'GTH1:60,0; {Consigna Heater Estación de Lavado}


    'CHECKAJ:3C8E;

#End Region

End Class
