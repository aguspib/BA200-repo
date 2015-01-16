Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
'Imports Biosystems.Ax00.DAL.DAO To remove
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types.BaseLinesDS
Imports Biosystems.Ax00.Types.ExecutionsDS
Imports Biosystems.Ax00.Types.twksWSReadingsDS
Imports Biosystems.Ax00.Calculations
Imports System.Reflection
'Imports System.Xml ' to remove
Imports System.IO
Imports System.ComponentModel




Namespace Biosystems.Ax00.BL

    Partial Public Class ResultsFileDelegate

#Region "Declarations"
        ' Modify by DL 09/06/2011
        ' Warning. Excel 97, Excel 2000, Excel XP y Excel 2003 only allow 256 (A to IV, 2^8) columns. In Excel 2007 allow 1024 columns
        '          Excel 97, Excel 2000, Excel XP y Excel 2003 only allow 65.536 (2^16) rows. In Excel 2007 allow 1.048.576 (2^20)
        '
        Private cExcel() As String = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", _
                                  "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ", _
                                  "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ", _
                                  "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ", _
                                  "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ", _
                                  "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ", _
                                  "FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FI", "FJ", "FK", "FL", "FM", "FN", "FO", "FP", "FQ", "FR", "FS", "FT", "FU", "FV", "FW", "FX", "FY", "FZ", _
                                  "GA", "GB", "GC", "GD", "GE", "GF", "GG", "GH", "GI", "GJ", "GK", "GL", "GM", "GN", "GO", "GP", "GQ", "GR", "GS", "GT", "GU", "GV", "GW", "GX", "GY", "GZ", _
                                  "HA", "HB", "HC", "HD", "HE", "HF", "HG", "HH", "HI", "HJ", "HK", "HL", "HM", "HN", "HO", "HP", "HQ", "HR", "HS", "HT", "HU", "HV", "HW", "HX", "HY", "HZ", _
                                  "IA", "IB", "IC", "ID", "IE", "IF", "IG", "IH", "II", "IJ", "IK", "IL", "IM", "IN", "IO", "IP", "IQ", "IR", "IS", "IT", "IU", "IV"}

        Private ReadOnly XlsSingleFormat As String = "0{0}0###" 'RH 15/02/2012
#End Region

#Region "Main Public Methods"

        'RH 15/02/2012
        Public Sub New()
            XlsSingleFormat = String.Format(XlsSingleFormat, SystemInfoManager.OSDecimalSeparator)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pPathName" ></param>
        ''' <param name="pFileName"></param>
        ''' <param name="pAnalyzerIDwhenWSNoExists" ></param>
        ''' <remarks>
        ''' AG 04/01/2011 - copied and adapted from ExcelWaitForm
        ''' RH 12/07/2011 - Some code optimization and corrections
        ''' AG 28/07/2011 - add parameter pAnalyzerIDwhenWSNoExists for case generate excel with no worksession (only adjust base line)
        ''' </remarks>
        Public Function ExportXLS(ByVal pWorkSessionID As String, ByVal pPathName As String, ByVal pFileName As String, ByVal pAnalyzerIDwhenWSNoExists As String) As GlobalDataTO
            'Dim resultdata As New GlobalDataTO
            Dim resultdata As GlobalDataTO = Nothing
            Dim DateIniProcess As Date = Now

            If Not Directory.Exists(pPathName) Then
                Directory.CreateDirectory(pPathName)
                pFileName = pPathName & pFileName
            End If

            Dim myTypeExcel As Type = Type.GetTypeFromProgID("Excel.Application")
            Dim myExcel As Object = Activator.CreateInstance(myTypeExcel)
            Dim myBook As Object = Nothing
            Dim myWorkSheets As Object = Nothing

            Try
                ' Create xls document and sheets
                resultdata = Me.NewExcelFile(pFileName, myTypeExcel, myExcel, myWorkSheets, myBook)
                If Not resultdata.HasError Then
                    ' Get data from work session
                    'Dim dbConnection As New SqlClient.SqlConnection
                    Dim dbConnection As SqlClient.SqlConnection

                    resultdata = DAOBase.GetOpenDBConnection(Nothing)

                    If (Not resultdata.HasError) Then
                        dbConnection = CType(resultdata.SetDatos, SqlClient.SqlConnection)

                        If (Not dbConnection Is Nothing) Then
                            Dim myWorkSession As New WorkSessionsDelegate

                            resultdata = myWorkSession.GetByWorkSession(dbConnection, pWorkSessionID)

                            If Not resultdata.HasError Then
                                'Dim myWorkSessionsDS As New WorkSessionsDS
                                Dim myWorkSessionsDS As WorkSessionsDS
                                Dim myAnalyzersDelegate As New WSAnalyzersDelegate
                                ' Get data for Analyzer
                                myWorkSessionsDS = DirectCast(resultdata.SetDatos, WorkSessionsDS)
                                resultdata = myAnalyzersDelegate.GetByWorkSession(dbConnection, pWorkSessionID)
                                If Not resultdata.HasError Then
                                    'Dim myAnalyzerDS As New WSAnalyzersDS
                                    Dim myAnalyzerDS As WSAnalyzersDS
                                    myAnalyzerDS = DirectCast(resultdata.SetDatos, WSAnalyzersDS)

                                    resultdata = SheetBaseLine(dbConnection, myAnalyzerDS, myWorkSessionsDS, myWorkSheets, pAnalyzerIDwhenWSNoExists)

                                    If Not resultdata.HasError AndAlso myWorkSessionsDS.twksWorkSessions.Rows.Count > 0 Then

                                        If Not resultdata.HasError Then
                                            resultdata = SheetBaseLinebyWell(dbConnection, myAnalyzerDS, myWorkSessionsDS, myWorkSheets)
                                        Else
                                            'Exit Function
                                            Return resultdata
                                        End If

                                        If Not resultdata.HasError Then
                                            resultdata = SheetCount(dbConnection, myAnalyzerDS, myWorkSessionsDS, myWorkSheets)
                                        Else
                                            'Exit Function
                                            Return resultdata
                                        End If

                                        If Not resultdata.HasError Then
                                            resultdata = SheetAbsorbance(dbConnection, myAnalyzerDS, myWorkSessionsDS, myWorkSheets)
                                        Else
                                            'Exit Function
                                            Return resultdata
                                        End If

                                        If Not resultdata.HasError Then
                                            resultdata = SheetComplete(dbConnection, myAnalyzerDS, myWorkSessionsDS, myWorkSheets)
                                        Else
                                            'Exit Function
                                            Return resultdata
                                        End If

                                        'RH 13/07/2011
                                        If Not resultdata.HasError Then
                                            resultdata = SheetResultsByReplicates(dbConnection, myAnalyzerDS, myWorkSessionsDS, myWorkSheets)
                                        Else
                                            Return resultdata
                                        End If

                                    End If

                                End If
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportCalculations.ExportXLS", EventLogEntryType.Error, False)

            Finally
                Me.CloseProcess(DateIniProcess, myTypeExcel, myExcel, myBook)

            End Try

            Return resultdata

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pOnlyProgrammedTestCycle"></param>
        ''' <returns>gLOBALDatato (AbsorbancesDS)</returns>
        ''' <remarks></remarks>
        Public Function GetReadingAbsorbancesByExecution(ByVal pDBConnection As SqlConnection, _
                                                         ByVal pExecutionID As Integer, _
                                                         ByVal pAnalyzerID As String, _
                                                         ByVal pWorkSessionID As String, _
                                                         ByVal pOnlyProgrammedTestCycle As Boolean) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim allOK As Boolean = False

            Try
                Dim myExecutionsDelegate As New ExecutionsDelegate

                ' GETEXECUTION
                resultData = myExecutionsDelegate.GetExecution(pDBConnection, pExecutionID, pAnalyzerID, pWorkSessionID)

                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                    Dim myExecutionDS As ExecutionsDS
                    Dim myWellBaseLineID As Integer
                    Dim myAdjustBaseLineID As Integer
                    Dim myWellUsed As Integer

                    myExecutionDS = CType(resultData.SetDatos, ExecutionsDS)

                    If myExecutionDS.twksWSExecutions.Rows.Count > 0 Then

                        If Not myExecutionDS.twksWSExecutions.Item(0).IsBaseLineIDNull Then
                            myWellBaseLineID = myExecutionDS.twksWSExecutions.Item(0).BaseLineID
                            myWellUsed = myExecutionDS.twksWSExecutions.Item(0).WellUsed
                            myAdjustBaseLineID = myExecutionDS.twksWSExecutions.Item(0).AdjustBaseLineID

                            If Not myExecutionDS.twksWSExecutions.Item(0).IsOrderTestIDNull Then
                                Dim myOrderTestID As Integer
                                myOrderTestID = myExecutionDS.twksWSExecutions.Item(0).OrderTestID

                                ' GETTESTID
                                Dim myOrderTestsDelegate As New OrderTestsDelegate
                                Dim myTestID As Integer
                                resultData = myOrderTestsDelegate.GetTestID(pDBConnection, myOrderTestID)

                                If Not resultData.HasError Then
                                    Dim myOrderTestDS As New OrderTestsDS
                                    myOrderTestDS = CType(resultData.SetDatos, OrderTestsDS)

                                    If Not myOrderTestDS.twksOrderTests.Item(0).IsTestIDNull Then
                                        myTestID = myOrderTestDS.twksOrderTests.Item(0).TestID

                                        ' READ
                                        Dim myTestsDS As New TestsDS
                                        Dim myTestsData As New TestsDelegate
                                        Dim myMainWaveLength As Integer = -1
                                        Dim myRefWaveLength As Integer = -1

                                        resultData = myTestsData.Read(pDBConnection, myTestID)

                                        If Not resultData.HasError Then
                                            myTestsDS = CType(resultData.SetDatos, TestsDS)

                                            Select Case myTestsDS.tparTests.Item(0).ReadingMode
                                                Case "MONO"
                                                    If Not myTestsDS.tparTests.Item(0).IsMainWavelengthNull Then myMainWaveLength = CInt(myTestsDS.tparTests.Item(0).MainWavelength)
                                                    If Not myTestsDS.tparTests.Item(0).IsReferenceWavelengthNull Then myRefWaveLength = CInt(myTestsDS.tparTests.Item(0).ReferenceWavelength)

                                                Case "BIC"
                                                    If Not myTestsDS.tparTests.Item(0).IsMainWavelengthNull Then myMainWaveLength = CInt(myTestsDS.tparTests.Item(0).MainWavelength)
                                                    If Not myTestsDS.tparTests.Item(0).IsReferenceWavelengthNull Then myRefWaveLength = CInt(myTestsDS.tparTests.Item(0).ReferenceWavelength)
                                            End Select

                                            Dim myNumCycles As Integer = -1

                                            'If pOnlyProgrammedTestCycle Then ' DL 17/05/2011
                                            Dim FirstCycle As Integer = -1
                                            Dim SecondCycle As Integer = -1

                                            If Not myTestsDS.tparTests.Item(0).IsFirstReadingCycleNull Then FirstCycle = myTestsDS.tparTests.Item(0).FirstReadingCycle
                                            If Not myTestsDS.tparTests.Item(0).IsSecondReadingCycleNull Then SecondCycle = myTestsDS.tparTests.Item(0).SecondReadingCycle

                                            If FirstCycle > SecondCycle Then
                                                myNumCycles = FirstCycle
                                            Else
                                                myNumCycles = SecondCycle
                                            End If
                                            'End If 'DL 17/05/2011

                                            allOK = True
                                            resultData = GetReadingAbsorbances(pDBConnection, _
                                                                               pAnalyzerID, _
                                                                               pExecutionID, _
                                                                               pWorkSessionID, _
                                                                               myAdjustBaseLineID, _
                                                                               myMainWaveLength, _
                                                                               myRefWaveLength, _
                                                                               myNumCycles, _
                                                                               myWellBaseLineID, _
                                                                               myWellUsed)

                                            'ag 15/10/2012 - WHEN SYStem error keep the error flag
                                            If resultData.HasError And resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString Then
                                                allOK = False
                                            End If

                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                If Not allOK Then
                    Dim noAbsorbances As New AbsorbanceDS
                    resultData.SetDatos = noAbsorbances
                    resultData.HasError = True
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "GetReadingAbsorbancesByExecution", EventLogEntryType.Error, False)
            End Try

            Return resultData

        End Function


#End Region


#Region "Main privtes Excel methods"

        '''' <summary>
        '''' Create new excel file
        '''' </summary>
        '''' <param name="pFileName">Open DB Connection</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created By: DL 08/06/2010
        '''' </remarks>
        Private Function NewExcelFile(ByVal pFileName As String, _
                                     ByRef ptypeExcel As Type, _
                                     ByRef pExcel As Object, _
                                     ByRef pWorkSheets As Object, _
                                     ByRef pBook As Object) As GlobalDataTO

            Dim resultdata As New GlobalDataTO

            Try
                Dim WorkSheet As Object
                Dim myPage As Object
                Dim myWorkbook As Object

                'DateIniProcess = Now

                ' Create new Excel application
                ptypeExcel.InvokeMember("Visible", Reflection.BindingFlags.SetProperty, Nothing, pExcel, New Object() {False}) 'To false
                ptypeExcel.InvokeMember("DisplayAlerts", Reflection.BindingFlags.SetProperty, Nothing, pExcel, New Object() {False})

                ' Add new workbook
                myWorkbook = ptypeExcel.InvokeMember("Workbooks", Reflection.BindingFlags.GetProperty, Nothing, pExcel, Nothing)
                pBook = myWorkbook.GetType.InvokeMember("Add", Reflection.BindingFlags.InvokeMethod, Nothing, myWorkbook, Nothing)
                pBook.GetType.InvokeMember("SaveAs", Reflection.BindingFlags.InvokeMethod, Nothing, pBook, New Object() {"" & pFileName & ""})
                pWorkSheets = myWorkbook.GetType().InvokeMember("Worksheets", BindingFlags.GetProperty, Nothing, pBook, Nothing)
                ' add new pages
                ' add page absorbances
                WorkSheet = pWorkSheets.GetType().InvokeMember("Add", BindingFlags.GetProperty, Nothing, pWorkSheets, Nothing)
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {4})
                ' add page Complete
                WorkSheet = pWorkSheets.GetType().InvokeMember("Add", BindingFlags.GetProperty, Nothing, pWorkSheets, Nothing)
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {5})

                'RH 13/07/2011 Add page Results
                WorkSheet = pWorkSheets.GetType().InvokeMember("Add", BindingFlags.GetProperty, Nothing, pWorkSheets, Nothing)
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {6})
                ' end add pages

                ' Rename Pages
                ' Rename Page 1
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {1})
                myPage.GetType().InvokeMember("Name", BindingFlags.SetProperty, Nothing, myPage, New Object() {"BaseLine"})
                ' Rename page 2
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {2})
                myPage.GetType().InvokeMember("Name", BindingFlags.SetProperty, Nothing, myPage, New Object() {"Base Line by Well"})
                ' Rename page 3
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {3})
                myPage.GetType().InvokeMember("Name", BindingFlags.SetProperty, Nothing, myPage, New Object() {"Counts"})
                ' Rename page 4
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {4})
                myPage.GetType.InvokeMember("Name", BindingFlags.SetProperty, Nothing, myPage, New Object() {"Absorbances"})
                ' Rename page 5
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {5})
                myPage.GetType.InvokeMember("Name", BindingFlags.SetProperty, Nothing, myPage, New Object() {"Complete"})
                'RH 13/07/2011 Rename page 6
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {6})
                myPage.GetType.InvokeMember("Name", BindingFlags.SetProperty, Nothing, myPage, New Object() {"Results"})


            Catch ex As Exception 'General Error
                resultdata.HasError = True
                resultdata.ErrorCode = "SYSTEM_ERROR"
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportCalculations.NewExcelFile", EventLogEntryType.Error, False)
            End Try

            Return resultdata

        End Function


        '''' <summary>
        '''' Create Sheet Base Line by well
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created By: DLM 08/06/2010
        '''' Modified AG: 04/01/2011
        ''''          AG: 05/09/2011 - also copy AbsValue
        ''''          RH 02/01/2011 Modify use of SetCellValue(). Pass original numeric value, not the string converted one.
        '''' </remarks>
        Private Function SheetBaseLinebyWell(ByVal pDBConnection As SqlConnection, _
                                            ByVal pAnalyzerDS As WSAnalyzersDS, _
                                            ByVal pWorkSessionDS As WorkSessionsDS, _
                                            ByVal pWorkSheets As Object) As GlobalDataTO

            Dim resultdata As New GlobalDataTO
            Dim myWSID As String = pWorkSessionDS.twksWorkSessions(0).WorkSessionID

            Try
                Dim myHeader As String
                Dim myPage As Object
                Dim myRango As String

                ' Selecet base line by well sheet
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {2})

                ' Ini Header 
                myHeader = "Base lines by well (without adjustments)"

                SetCellValue(myPage, "A1", myHeader, True)
                MergeCells(myPage, "A1:H1")

                myHeader = "WorkSessionID " & myWSID & " "
                myHeader &= "Creation Date: " & pWorkSessionDS.twksWorkSessions(0).TS_DateTime & " "
                myHeader &= "by " & pWorkSessionDS.twksWorkSessions(0).TS_User

                SetCellValue(myPage, "A3", myHeader)
                MergeCells(myPage, "A3:H3")
                SetCellColor(myPage, "A1:H3", 36)
                'End Header

                ' Header analyzer
                Dim myCellRow As Integer = 5
                Dim bAutoFit As Boolean = True
                For Each myRowAnalyzer As WSAnalyzersDS.twksWSAnalyzersRow In pAnalyzerDS.twksWSAnalyzers.Rows
                    SetCellValue(myPage, "A" & myCellRow, "AnalyzerID " & myRowAnalyzer.AnalyzerID)
                    myRango = "A" & myCellRow & ":H" & myCellRow
                    MergeCells(myPage, myRango)
                    SetCellColor(myPage, myRango, 19)

                    myCellRow += 2

                    Dim myWSBLinesbyWellDelegate As New WSBLinesByWellDelegate
                    resultdata = myWSBLinesbyWellDelegate.GetByWorkSession(pDBConnection, myRowAnalyzer.AnalyzerID, myWSID)
                    If Not resultdata.HasError Then
                        Dim myBaseLinesDS As New BaseLinesDS
                        myBaseLinesDS = DirectCast(resultdata.SetDatos, BaseLinesDS)

                        Dim qBaseLineID As New List(Of Integer)
                        ' Filter by baseline id
                        qBaseLineID = (From a In myBaseLinesDS.twksWSBaseLines _
                                       Select a.BaseLineID Distinct).ToList

                        For indexGroupBaseLine As Integer = 0 To qBaseLineID.Count - 1
                            Dim myBaseLineID As Integer = qBaseLineID.Item(indexGroupBaseLine)

                            ' Base line header
                            ' Well used
                            Dim qWellUsed As New List(Of Integer)
                            qWellUsed = (From a In myBaseLinesDS.twksWSBaseLines _
                                         Where a.BaseLineID = myBaseLineID _
                                         Select a.WellUsed Distinct).ToList

                            For myWellCount = 0 To qWellUsed.Count - 1
                                Dim myCurrentWell As Integer = qWellUsed(myWellCount)

                                Dim qDate As New List(Of twksWSBaseLinesRow)
                                'AG 28/07/2011
                                'qDate = (From a In myBaseLinesDS.twksWSBaseLines Where a.BaseLineID = myBaseLineID Order By a.DateTime Take 1 Select a).ToList
                                qDate = (From a In myBaseLinesDS.twksWSBaseLines Where a.BaseLineID = myBaseLineID And a.WellUsed = myCurrentWell Order By a.DateTime Take 1 Select a).ToList

                                ' Base line header
                                If qDate.Count > 0 Then
                                    myHeader = "BaseLineID: " & qBaseLineID.Item(indexGroupBaseLine).ToString & " Well used: " & myCurrentWell.ToString & " Date: " & qDate(0).DateTime
                                Else
                                    myHeader = "BaseLineID: " & qBaseLineID.Item(indexGroupBaseLine).ToString & " Well used: " & myCurrentWell.ToString & " Date: "
                                End If

                                myRango = "A" & myCellRow & ":H" & myCellRow

                                SetCellValue(myPage, "A" & myCellRow, myHeader)
                                SetCellColor(myPage, myRango, 6)

                                myCellRow += 1
                                SetCellValue(myPage, "A" & myCellRow, "Diode (POS)")
                                SetCellValue(myPage, "B" & myCellRow, "WaveLength")
                                SetCellValue(myPage, "C" & myCellRow, "Main Light")
                                SetCellValue(myPage, "D" & myCellRow, "Main Dark")
                                SetCellValue(myPage, "E" & myCellRow, "Reference Light")
                                SetCellValue(myPage, "F" & myCellRow, "Reference Dark")
                                SetCellValue(myPage, "G" & myCellRow, "Absorbance value") 'AG 05/09/2011

                                myRango = "A" & myCellRow & ":H" & myCellRow

                                SetCellColor(myPage, myRango, 15)

                                If bAutoFit Then
                                    SetHorizontalAlignment(myPage, myRango, 3, True)
                                    bAutoFit = False
                                Else
                                    SetHorizontalAlignment(myPage, myRango, 3, False)
                                End If

                                myCellRow += 1

                                ' Base line records
                                Dim qBaseLine As New List(Of twksWSBaseLinesRow)
                                qBaseLine = (From a In myBaseLinesDS.twksWSBaseLines _
                                             Where a.BaseLineID = myBaseLineID And a.WellUsed = myCurrentWell _
                                             Select a).ToList

                                For indexBaseLine As Integer = 0 To qBaseLine.Count - 1
                                    Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDelegate
                                    resultdata = myAnalyzerLedPositions.GetByLedPosition(pDBConnection, _
                                                                                         myRowAnalyzer.AnalyzerID, _
                                                                                         qBaseLine(indexBaseLine).Wavelength)

                                    If Not resultdata.HasError And Not resultdata.SetDatos Is Nothing Then
                                        Dim myAnalyzerLedPositionsDS As New AnalyzerLedPositionsDS
                                        myAnalyzerLedPositionsDS = CType(resultdata.SetDatos, AnalyzerLedPositionsDS)
                                        'SetCellValue(myPage, "A" & myCellRow, CType(myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions(0).LedPosition, String))
                                        'SetCellValue(myPage, "B" & myCellRow, CStr(myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions(0).WaveLength))

                                        'RH 02/01/2012
                                        SetCellValue(myPage, "A" & myCellRow, myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions(0).LedPosition)
                                        SetCellValue(myPage, "B" & myCellRow, myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions(0).WaveLength)

                                        If Not qBaseLine(indexBaseLine).IsMainLightNull Then
                                            'SetCellValue(myPage, "C" & myCellRow, CStr(qBaseLine(indexBaseLine).MainLight))
                                            SetCellValue(myPage, "C" & myCellRow, qBaseLine(indexBaseLine).MainLight)
                                        End If

                                        If Not qBaseLine(indexBaseLine).IsMainDarkNull Then
                                            'SetCellValue(myPage, "D" & myCellRow, CStr(qBaseLine(indexBaseLine).MainDark))
                                            SetCellValue(myPage, "D" & myCellRow, qBaseLine(indexBaseLine).MainDark)
                                        End If

                                        If Not qBaseLine(indexBaseLine).IsRefLightNull Then
                                            'SetCellValue(myPage, "E" & myCellRow, CStr(qBaseLine(indexBaseLine).RefLight))
                                            SetCellValue(myPage, "E" & myCellRow, qBaseLine(indexBaseLine).RefLight)
                                        End If

                                        If Not qBaseLine(indexBaseLine).IsRefDarkNull Then
                                            'SetCellValue(myPage, "F" & myCellRow, CStr(qBaseLine(indexBaseLine).RefDark))
                                            SetCellValue(myPage, "F" & myCellRow, qBaseLine(indexBaseLine).RefDark)
                                        End If

                                        'AG 05/09/2011
                                        If Not qBaseLine(indexBaseLine).IsABSvalueNull Then
                                            'SetCellValue(myPage, "G" & myCellRow, CStr(qBaseLine(indexBaseLine).ABSvalue))
                                            SetCellValue(myPage, "G" & myCellRow, qBaseLine(indexBaseLine).ABSvalue)
                                        End If
                                        'AG 05/09/2011

                                        If indexBaseLine = qBaseLine.Count - 1 Then
                                            myCellRow += 2
                                        Else
                                            myCellRow += 1
                                        End If
                                    End If
                                Next indexBaseLine

                            Next myWellCount
                        Next indexGroupBaseLine

                    End If
                Next myRowAnalyzer

            Catch ex As Exception
                resultdata.HasError = True
                resultdata.ErrorCode = "SYSTEM_ERROR"
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportCalculations.SheetBaseLinebyWell", EventLogEntryType.Error, False)
            End Try

            Return resultdata

        End Function



        '''' <summary>
        '''' Create Sheet Base Line 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created By: DL 08/06/2010
        '''' Modified AG 28/07/2011 - generate base lines sheet although no worksession exists (use pAnalyzerIDwhenWSNoExists)
        ''''                          parameter pAnalyzerDS ... byRef
        ''''          RH 02/01/2011 Modify use of SetCellValue(). Pass original numeric value, not the string converted one.
        '''' </remarks>
        Private Function SheetBaseLine(ByVal pDBConnection As SqlConnection, _
                                      ByRef pAnalyzerDS As WSAnalyzersDS, _
                                      ByVal pWorkSessionDS As WorkSessionsDS, _
                                      ByVal pWorksheets As Object, _
                                      ByVal pAnalyzerIDwhenWSNoExists As String) As GlobalDataTO

            Dim resultdata As New GlobalDataTO
            Dim myWSID As String = ""

            Try
                Dim myHeader As String
                Dim myPage As Object
                Dim myRango As String
                Dim myWSStatus As String = ""

                ' Selecet base line sheet
                myPage = pWorksheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorksheets, New Object() {1})

                ' Ini Header 
                myHeader = "Base lines with adjustments of integration time and DAC"

                SetCellValue(myPage, "A1", myHeader, True)
                MergeCells(myPage, "A1:H1")

                If pWorkSessionDS.twksWorkSessions.Rows.Count > 0 Then
                    myWSID = pWorkSessionDS.twksWorkSessions(0).WorkSessionID

                    'AG 24/10/2011 - Get the WS status
                    Dim linqRes As List(Of WSAnalyzersDS.twksWSAnalyzersRow)
                    linqRes = (From a As WSAnalyzersDS.twksWSAnalyzersRow In pAnalyzerDS.twksWSAnalyzers _
                               Where String.Compare(a.WorkSessionID, myWSID, False) = 0 Select a).ToList

                    If linqRes.Count > 0 AndAlso Not linqRes(0).IsWSStatusNull Then
                        myWSStatus = linqRes(0).WSStatus
                    End If
                    'AG 24/10/2011
                End If

                If myWSStatus <> "EMPTY" Then
                    myHeader = "WorkSessionID " & myWSID & " "
                    myHeader &= "Creation Date: " & pWorkSessionDS.twksWorkSessions(0).TS_DateTime & " "
                    myHeader &= "by " & pWorkSessionDS.twksWorkSessions(0).TS_User
                Else
                    myWSID = ""
                    myHeader = "WorkSessionID "
                    myHeader &= "Creation Date: "
                    myHeader &= "by "
                End If

                SetCellValue(myPage, "A3", myHeader)
                MergeCells(myPage, "A3:H3")
                SetCellColor(myPage, "A1:H3", 36)
                ' End Header

                'If no WorkSession exists the adjust base lines are get using parameter pAnalyzerIDwhenWSNoExists
                If myWSID = "" AndAlso pAnalyzerDS.twksWSAnalyzers.Rows.Count = 0 Then
                    Dim newRow As WSAnalyzersDS.twksWSAnalyzersRow
                    newRow = pAnalyzerDS.twksWSAnalyzers.NewtwksWSAnalyzersRow
                    newRow.AnalyzerID = pAnalyzerIDwhenWSNoExists
                    newRow.SetAnalyzerModelNull()
                    pAnalyzerDS.twksWSAnalyzers.AddtwksWSAnalyzersRow(newRow)
                    pAnalyzerDS.AcceptChanges()
                End If

                ' Header analyzer
                Dim myCellRow As Integer = 5
                Dim bAutoFit As Boolean = True
                For Each myRowAnalyzer As WSAnalyzersDS.twksWSAnalyzersRow In pAnalyzerDS.twksWSAnalyzers.Rows
                    Dim myWSBLinesDelegate As New WSBLinesDelegate

                    SetCellValue(myPage, "A" & myCellRow, "AnalyzerID " & myRowAnalyzer.AnalyzerID)

                    myRango = "A" & myCellRow & ":H" & myCellRow
                    MergeCells(myPage, myRango)
                    SetCellColor(myPage, myRango, 19)

                    myCellRow += 2
                    resultdata = myWSBLinesDelegate.GetByWorkSession(pDBConnection, myRowAnalyzer.AnalyzerID, myWSID)

                    If Not resultdata.HasError Then
                        Dim myBaseLinesDS As New BaseLinesDS
                        myBaseLinesDS = DirectCast(resultdata.SetDatos, BaseLinesDS)

                        Dim qBaseLineID As New List(Of Integer)
                        ' Filter by baseline id
                        qBaseLineID = (From a In myBaseLinesDS.twksWSBaseLines _
                                       Select a.BaseLineID Distinct).ToList

                        For indexGroupBaseLine As Integer = 0 To qBaseLineID.Count - 1
                            Dim myBaseLineID As Integer = qBaseLineID.Item(indexGroupBaseLine)

                            ' Base line header
                            ' Well used
                            Dim qWellUsed As New List(Of Integer)
                            qWellUsed = (From a In myBaseLinesDS.twksWSBaseLines _
                                         Where a.BaseLineID = myBaseLineID _
                                         Select a.WellUsed Distinct).ToList

                            Dim qDate As New List(Of twksWSBaseLinesRow)
                            qDate = (From a In myBaseLinesDS.twksWSBaseLines Where a.BaseLineID = myBaseLineID Order By a.DateTime Take 1 Select a).ToList

                            ' Base line header

                            If qDate.Count > 0 Then
                                myHeader = "BaseLineID: " & qBaseLineID.Item(indexGroupBaseLine).ToString & " Well used: " & qWellUsed.Item(0).ToString & " Date: " & qDate(0).DateTime
                            Else
                                myHeader = "BaseLineID: " & qBaseLineID.Item(indexGroupBaseLine).ToString & " Well used: " & qWellUsed.Item(0).ToString & " Date: "
                            End If

                            myRango = "A" & myCellRow & ":H" & myCellRow

                            SetCellValue(myPage, "A" & myCellRow, myHeader)
                            MergeCells(myPage, myRango)
                            SetCellColor(myPage, myRango, 6)

                            myCellRow += 1

                            SetCellValue(myPage, "A" & myCellRow, "Diode (POS)")
                            SetCellValue(myPage, "B" & myCellRow, "WaveLength")
                            SetCellValue(myPage, "C" & myCellRow, "Main Light")
                            SetCellValue(myPage, "D" & myCellRow, "Main Dark")
                            SetCellValue(myPage, "E" & myCellRow, "Reference Light")
                            SetCellValue(myPage, "F" & myCellRow, "Reference Dark")
                            SetCellValue(myPage, "G" & myCellRow, "  IT   ")
                            SetCellValue(myPage, "H" & myCellRow, "  DAC  ")

                            myRango = "A" & myCellRow & ":H" & myCellRow

                            SetCellColor(myPage, myRango, 15)

                            If bAutoFit Then
                                SetHorizontalAlignment(myPage, myRango, 3, True)
                                bAutoFit = False
                            Else
                                SetHorizontalAlignment(myPage, myRango, 3)
                            End If

                            myCellRow += 1

                            ' Base line records
                            Dim qBaseLine As New List(Of twksWSBaseLinesRow)
                            qBaseLine = (From a In myBaseLinesDS.twksWSBaseLines _
                                         Where a.BaseLineID = myBaseLineID _
                                         Select a).ToList

                            For indexBaseLine As Integer = 0 To qBaseLine.Count - 1
                                Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDelegate
                                resultdata = myAnalyzerLedPositions.GetByLedPosition(pDBConnection, _
                                                                                     myRowAnalyzer.AnalyzerID, _
                                                                                     qBaseLine(indexBaseLine).Wavelength)

                                If Not resultdata.HasError Then
                                    Dim myAnalyzerLedPositionsDS As New AnalyzerLedPositionsDS
                                    myAnalyzerLedPositionsDS = CType(resultdata.SetDatos, AnalyzerLedPositionsDS)

                                    If myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.Rows.Count > 0 Then
                                        'SetCellValue(myPage, "A" & myCellRow, CType(myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions(0).LedPosition, String))
                                        'SetCellValue(myPage, "B" & myCellRow, CType(myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions(0).WaveLength, String))
                                        'SetCellValue(myPage, "C" & myCellRow, CType(qBaseLine(indexBaseLine).MainLight, String))
                                        'SetCellValue(myPage, "D" & myCellRow, CType(qBaseLine(indexBaseLine).MainDark, String))
                                        'SetCellValue(myPage, "E" & myCellRow, CType(qBaseLine(indexBaseLine).RefLight, String))
                                        'SetCellValue(myPage, "F" & myCellRow, CType(qBaseLine(indexBaseLine).RefDark, String))

                                        'If Not qBaseLine(indexBaseLine).IsITNull Then SetCellValue(myPage, "G" & myCellRow, CType(qBaseLine(indexBaseLine).IT, String))
                                        'If Not qBaseLine(indexBaseLine).IsDACNull Then SetCellValue(myPage, "H" & myCellRow, CType(qBaseLine(indexBaseLine).DAC, String))

                                        SetCellValue(myPage, "A" & myCellRow, myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions(0).LedPosition)
                                        SetCellValue(myPage, "B" & myCellRow, myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions(0).WaveLength)
                                        SetCellValue(myPage, "C" & myCellRow, qBaseLine(indexBaseLine).MainLight)
                                        SetCellValue(myPage, "D" & myCellRow, qBaseLine(indexBaseLine).MainDark)
                                        SetCellValue(myPage, "E" & myCellRow, qBaseLine(indexBaseLine).RefLight)
                                        SetCellValue(myPage, "F" & myCellRow, qBaseLine(indexBaseLine).RefDark)

                                        If Not qBaseLine(indexBaseLine).IsITNull Then SetCellValue(myPage, "G" & myCellRow, qBaseLine(indexBaseLine).IT)
                                        If Not qBaseLine(indexBaseLine).IsDACNull Then SetCellValue(myPage, "H" & myCellRow, qBaseLine(indexBaseLine).DAC)

                                        If indexBaseLine = qBaseLine.Count - 1 Then
                                            myCellRow += 2
                                        Else
                                            myCellRow += 1
                                        End If
                                    End If
                                End If
                            Next indexBaseLine
                        Next indexGroupBaseLine
                    End If
                Next myRowAnalyzer

            Catch ex As Exception
                resultdata.HasError = True
                resultdata.ErrorCode = "SYSTEM_ERROR"
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportCalculations.SheetBaseLine", EventLogEntryType.Error, False)
            End Try

            Return resultdata

        End Function


        '''' <summary>
        '''' Create Sheet Base Line
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created By: DLM 08/06/2010
        '''' AG 04/01/2011
        ''''          RH 02/01/2011 Modify use of SetCellValue(). Pass original numeric value, not the string converted one.
        '''' </remarks>
        Private Function SheetCount(ByVal pDBConnection As SqlConnection, _
                                    ByVal pAnalyzerDS As WSAnalyzersDS, _
                                    ByVal pWorkSessionDS As WorkSessionsDS, _
                                    ByVal pWorkSheets As Object) As GlobalDataTO

            Dim resultdata As New GlobalDataTO
            Dim myWSID As String = pWorkSessionDS.twksWorkSessions(0).WorkSessionID

            Try
                Dim myHeader As String
                Dim myPage As Object
                Dim myRango As String

                ' Selecet count sheet
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {3})

                ' Ini Header 
                myHeader = "Results WorkSessionID " & myWSID & " " & _
                           "Creation Date: " & pWorkSessionDS.twksWorkSessions(0).TS_DateTime & " " & _
                           "by " & pWorkSessionDS.twksWorkSessions(0).TS_User

                SetCellValue(myPage, "A3", myHeader)
                SetCellColor(myPage, "A1:L3", 36) 'SetCellColor(myPage, "A1:K3", 36) 'DL 12/01/2012
                MergeCells(myPage, "A3:L3")       'MergeCells(myPage, "A3:K3") 'DL 12/01/2012
                'End Header

                ' Header analyzer
                Dim myCellRow As Integer = 5
                For Each myRowAnalyzer As WSAnalyzersDS.twksWSAnalyzersRow In pAnalyzerDS.twksWSAnalyzers.Rows
                    Dim myAnalyzerID As String = myRowAnalyzer.AnalyzerID

                    SetCellValue(myPage, "A" & myCellRow, "AnalyzerID " & myAnalyzerID)
                    myRango = "A" & myCellRow & ":L" & myCellRow  'myRango = "A" & myCellRow & ":K" & myCellRow 'DL 12/01/2012
                    SetCellColor(myPage, myRango, 19)
                    MergeCells(myPage, myRango)

                    myCellRow += 2

                    Dim myExecutionsDelegate As New ExecutionsDelegate
                    resultdata = myExecutionsDelegate.GetExecutionByWorkSession(pDBConnection, myAnalyzerID, myWSID, True)

                    If Not resultdata.HasError Then
                        Dim myExecutionDS As New ExecutionsDS
                        myExecutionDS = DirectCast(resultdata.SetDatos, ExecutionsDS)

                        Dim qExecutionsGrouped As New List(Of Integer)
                        'AG 06/07/2011 - add where clausule
                        qExecutionsGrouped = (From a In myExecutionDS.twksWSExecutions _
                                              Where String.Compare(a.ExecutionStatus, "PENDING", False) <> 0 And String.Compare(a.ExecutionStatus, "LOCKED", False) <> 0 _
                                              Order By a.OrderTestID _
                                              Group a By OrderTest_ID = a.OrderTestID Into Group _
                                              Select OrderTest_ID).ToList

                        For executionGroup As Integer = 0 To qExecutionsGrouped.Count - 1
                            Dim indexExecutionGroup = executionGroup
                            Dim myOrderTestData As New OrderTestsDelegate
                            Dim myTestID As Integer
                            resultdata = myOrderTestData.GetTestID(pDBConnection, qExecutionsGrouped.Item(indexExecutionGroup)) 'myExecutionDS.twksWSExecutions(0).OrderTestID)

                            If Not resultdata.HasError Then
                                Dim myOrderTestDS As New OrderTestsDS
                                myOrderTestDS = CType(resultdata.SetDatos, OrderTestsDS)
                                myTestID = myOrderTestDS.twksOrderTests.Item(0).TestID
                            End If

                            ' Get test data
                            Dim myTestsData As New TestsDelegate
                            Dim myTestsDS As New TestsDS
                            resultdata = myTestsData.Read(pDBConnection, myTestID)
                            If Not resultdata.HasError And Not resultdata.SetDatos Is Nothing Then myTestsDS = CType(resultdata.SetDatos, TestsDS)

                            ' Find position for main and reference wave lenght
                            Dim myTestName As String = myTestsDS.tparTests.Item(0).TestName
                            Dim myheadcell1 As String = ""

                            'DL 12/01/2012. Substitute OrderTestID by SampleID. Begin 
                            Dim Orderdata As New GlobalDataTO   'Call the Order to get the patient id or the Sample ID

                            Orderdata = myExecutionsDelegate.GetByOrderTestID(pDBConnection, _
                                                                              myAnalyzerID, _
                                                                              myWSID, _
                                                                              CType(qExecutionsGrouped(indexExecutionGroup).ToString, Integer))

                            Dim myElementID As String = ""
                            If (Not Orderdata.HasError AndAlso Not Orderdata.SetDatos Is Nothing) Then
                                Dim myExecutionsDS As ExecutionsDS
                                myExecutionsDS = DirectCast(Orderdata.SetDatos, ExecutionsDS)

                                If myExecutionsDS.vwksWSExecutionsMonitor.Rows.Count > 0 Then
                                    Select Case myExecutionsDS.vwksWSExecutionsMonitor.First.SampleClass
                                        Case "PATIENT"
                                            myElementID = myExecutionsDS.vwksWSExecutionsMonitor.First.PatientID

                                            'Case "BLANK"
                                            '   myElementID = ""

                                        Case "CTRL", "CALIB"
                                            myElementID = myExecutionsDS.vwksWSExecutionsMonitor.First.ElementName
                                    End Select
                                End If
                            End If


                            'Dim myOrdersDelegate As New OrdersDelegate
                            'Dim mySampleID As String = ""

                            'Orderdata = myOrdersDelegate.GetSampleIDbyOrderTestID(pDBConnection, CType(qExecutionsGrouped(indexExecutionGroup).ToString, Integer))
                            'If Not Orderdata.HasError Then
                            '    Dim myOrdersDS As New OrderTestsDetailsDS
                            '    myOrdersDS = DirectCast(Orderdata.SetDatos, OrderTestsDetailsDS)

                            '    If myOrdersDS.OrderTestsDetails.Rows.Count > 0 Then
                            '        If Not myOrdersDS.OrderTestsDetails.First.IsSampleIDNull Then
                            '            mySampleID = myOrdersDS.OrderTestsDetails.First.SampleID     'Set the patienID if not null
                            '        ElseIf Not myOrdersDS.OrderTestsDetails.First.IsPatientIDNull Then
                            '            mySampleID = myOrdersDS.OrderTestsDetails.First.PatientID     'Set the sampleID if not null
                            '        End If
                            '    End If
                            'End If

                            'myheadcell1 &= "OrderTest: " & qExecutionsGrouped(indexExecutionGroup).ToString & " ("  
                            myheadcell1 &= "SampleID: " & myElementID & " ("
                            'DL 12/01/2012. End

                            Dim qExecutionInfo As List(Of ExecutionsDS.twksWSExecutionsRow)
                            qExecutionInfo = (From a In myExecutionDS.twksWSExecutions _
                                              Where a.OrderTestID = qExecutionsGrouped(indexExecutionGroup) _
                                              Select a Order By a.RerunNumber, a.ReplicateNumber).ToList

                            If qExecutionInfo.Count > 0 Then
                                Dim myRerunNumber As Integer = 0
                                Dim MaxRowGroup As Integer = myCellRow 'DL 27.09.2012
                                For rerunPointer As Integer = 0 To qExecutionInfo.Count - 1
                                    If MaxRowGroup > myCellRow Then myCellRow = MaxRowGroup 'DL 27.09.2012
                                    If myRerunNumber <> qExecutionInfo(rerunPointer).RerunNumber Then
                                        Dim myheadcell2 As String = ""
                                        myRerunNumber = qExecutionInfo(rerunPointer).RerunNumber

                                        If Not qExecutionInfo(rerunPointer).IsSampleClassNull Then myheadcell2 &= qExecutionInfo(rerunPointer).SampleClass.Trim
                                        'myheadcell &= " ) Test " & myTestName & " Well / Rotor = "
                                        myheadcell2 &= " ) Test " & myTestName & " Rerun Number = "
                                        If Not qExecutionInfo(rerunPointer).IsRerunNumberNull Then
                                            myheadcell2 &= qExecutionInfo(rerunPointer).RerunNumber.ToString & " "
                                        Else
                                            myheadcell2 &= "? "
                                        End If

                                        Dim myFinalHeadCell As String = ""
                                        myFinalHeadCell = myheadcell1 & myheadcell2

                                        'Console.WriteLine(GetCellValue(myPage, "A22", Nothing))
                                        'GetNextEmptyRow()


                                        SetCellValue(myPage, "A" & myCellRow, myFinalHeadCell)
                                        'DL 12/01/2012. Begin
                                        'myRango = "A" & myCellRow & ":K" & myCellRow
                                        myRango = "A" & myCellRow & ":L" & myCellRow
                                        'DL 12/01/2012. End
                                        SetCellColor(myPage, myRango, 6)
                                        MergeCells(myPage, myRango)

                                        'AG 26/04/2011 - Well and Base lines headers depends on replicate number
                                        setReplicatesHeader(myCellRow, qExecutionInfo, myPage, "L")

                                        SetCellValue(myPage, "A" & myCellRow, "COUNTS")
                                        myRango = "A" & myCellRow & ":L" & myCellRow  ' myRango = "A" & myCellRow & ":K" & myCellRow 'DL 12/01/2012
                                        SetCellColor(myPage, myRango, 34)
                                        MergeCells(myPage, myRango)

                                        myCellRow += 1

                                        Dim qExecutionList As New List(Of twksWSExecutionsRow)
                                        qExecutionList = (From a In myExecutionDS.twksWSExecutions _
                                                              Where a.OrderTestID = qExecutionsGrouped(indexExecutionGroup) _
                                                                And a.AnalyzerID = myAnalyzerID _
                                                                And a.WorkSessionID = myWSID _
                                                                And a.RerunNumber = myRerunNumber _
                                                               Order By a.ReplicateNumber _
                                                              Select a).ToList

                                        Dim myCellCol As Integer

                                        For indexExecutionList As Integer = 0 To qExecutionList.Count - 1
                                            Dim tmpCellRow As Integer = myCellRow

                                            myCellCol = (indexExecutionList * 3) 'myCellCol = (indexExecutionList * 2) ' myCellCol = (indexExecutionList * 2) + 1 ''JVV 1330 - 15/10/2013

                                            SetCellValue(myPage, cExcel(myCellCol) & tmpCellRow, "  Rep" & indexExecutionList + 1 & "  ")
                                            SetCellValue(myPage, cExcel(myCellCol + 1) & tmpCellRow, "  Rep" & indexExecutionList + 1 & "  ")
                                            SetCellValue(myPage, cExcel(myCellCol + 2) & tmpCellRow, "  Rep" & indexExecutionList + 1 & "  ") ''JVV 1330 - 15/10/2013
                                            SetCellValue(myPage, cExcel(myCellCol) & tmpCellRow + 1, "  Main  ")
                                            SetCellValue(myPage, cExcel(myCellCol + 1) & tmpCellRow + 1, "  Reference")
                                            SetCellValue(myPage, cExcel(myCellCol + 2) & tmpCellRow + 1, "  Pause") ''JVV 1330 - 15/10/2013

                                            myRango = "A" & tmpCellRow & ":" & cExcel(myCellCol + 2) & (myCellRow + 1) 'myRango = "A" & tmpCellRow & ":" & cExcel(myCellCol + 1) & (myCellRow + 1) ''JVV 1330 - 15/10/2013
                                            SetCellColor(myPage, myRango, 19)
                                            SetHorizontalAlignment(myPage, myRango, 3, True)

                                            Dim auxCellRow As Integer = tmpCellRow + 2 ''JVV 1330 - 15/10/2013
                                            Dim myReadingsDelegate As New WSReadingsDelegate
                                            resultdata = myReadingsDelegate.GetByWorkSession(pDBConnection, myWSID, myAnalyzerID, qExecutionList(indexExecutionList).ExecutionID)

                                            If Not resultdata.HasError Then
                                                Dim myReadingDS As New twksWSReadingsDS
                                                myReadingDS = DirectCast(resultdata.SetDatos, twksWSReadingsDS)

                                                For Each myRowReadings As twksWSReadingsRow In myReadingDS.twksWSReadings.Rows
                                                    'SetCellValue(myPage, cExcel(myCellCol) & auxCellRow, CType(myRowReadings.MainCounts, String))
                                                    'SetCellValue(myPage, cExcel(myCellCol + 1) & auxCellRow, CType(myRowReadings.RefCounts, String))

                                                    SetCellValue(myPage, cExcel(myCellCol) & auxCellRow, myRowReadings.MainCounts)
                                                    SetCellValue(myPage, cExcel(myCellCol + 1) & auxCellRow, myRowReadings.RefCounts)
                                                    SetCellValue(myPage, cExcel(myCellCol + 2) & auxCellRow, IIf(myRowReadings.Pause = True, 1, 0)) ''JVV 1330 - 15/10/2013
                                                    auxCellRow += 1
                                                Next myRowReadings
                                            End If
                                            ''JVV 1330 - 15/10/2013 : Para que salga bien la línea de separación cuando hay más de 70 lecturas
                                            'If indexExecutionList = qExecutionList.Count - 1 Then
                                            '    myCellRow = auxCellRow + 1
                                            'End If
                                            'DL 27.09.2012
                                            If myCellRow > MaxRowGroup Or auxCellRow > MaxRowGroup Then
                                                MaxRowGroup = auxCellRow
                                            End If
                                            'DL 27.09.2012
                                            If indexExecutionList = qExecutionList.Count - 1 Then
                                                myCellRow = MaxRowGroup + 1
                                            End If
                                            ''JVV 1330 - 15/10/2013
                                        Next indexExecutionList

                                    End If ' If myRerunNumber <> qExecutionInfo(rerunPointer).RerunNumber then
                                Next rerunPointer
                            End If 'If qExecutionInfo.Count > 0 Then

                        Next
                    End If

                Next myRowAnalyzer

            Catch ex As Exception
                resultdata.HasError = True
                resultdata.ErrorCode = "SYSTEM_ERROR"
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportCalculations.SheetCount", EventLogEntryType.Error, False)
            End Try

            Return resultdata

        End Function

        '''' <summary>
        '''' Create Sheet absorbance
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created By: DL 08/06/2010
        '''' Modified By: RH 02/01/2011 Modify use of SetCellValue(). Pass original numeric value, not the string converted one.
        '''' </remarks>
        Private Function SheetAbsorbance(ByVal pDBConnection As SqlConnection, _
                                         ByVal pAnalyzerDS As WSAnalyzersDS, _
                                         ByVal pWorkSessionDS As WorkSessionsDS, _
                                         ByVal pWorkSheets As Object) As GlobalDataTO

            Dim resultdata As New GlobalDataTO
            Dim myWSID As String = pWorkSessionDS.twksWorkSessions(0).WorkSessionID

            Try
                Dim myPage As Object
                Dim myHeader As String
                Dim myRango As String

                ' Selecet Absorbance Sheet ( 4 )
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, _
                                                            Nothing, pWorkSheets, New Object() {4})

                ' Write Header 
                myHeader = "Results WorkSessionID " & myWSID & " " & _
                           "Creation Date: " & pWorkSessionDS.twksWorkSessions(0).TS_DateTime & " " & _
                           "by " & pWorkSessionDS.twksWorkSessions(0).TS_User

                SetCellValue(myPage, "A3", myHeader)
                MergeCells(myPage, "A3:M3")   'MergeCells(myPage, "A3:K3") 'dl 12/01/2012
                SetCellColor(myPage, "A1:M3", 36) 'SetCellColor(myPage, "A1:K3", 36) 'dl 12/01/2012

                ' Write Header Analyzer
                Dim myCellRow As Integer = 5
                For Each myRowAnalyzer As WSAnalyzersDS.twksWSAnalyzersRow In pAnalyzerDS.twksWSAnalyzers.Rows
                    Dim myAnalyzerID As String = myRowAnalyzer.AnalyzerID

                    SetCellValue(myPage, "A" & myCellRow, "AnalyzerID " & myAnalyzerID)
                    myRango = "A" & myCellRow & ":M" & myCellRow  'myRango = "A" & myCellRow & ":K" & myCellRow 'dl 12/01/2012
                    SetCellColor(myPage, myRango, 19)
                    MergeCells(myPage, myRango)

                    myCellRow += 2

                    Dim myExecutionsDelegate As New ExecutionsDelegate
                    resultdata = myExecutionsDelegate.GetExecutionByWorkSession(pDBConnection, myAnalyzerID, myWSID, True)

                    If Not resultdata.HasError Then
                        Dim myExecutionDS As New ExecutionsDS
                        myExecutionDS = DirectCast(resultdata.SetDatos, ExecutionsDS)

                        Dim qExecutionsGrouped As New List(Of Integer)
                        'AG 06/07/2011 - add where clausule
                        qExecutionsGrouped = (From a In myExecutionDS.twksWSExecutions _
                                              Where String.Compare(a.ExecutionStatus, "PENDING", False) <> 0 And a.ExecutionStatus <> "LOCKED" _
                                              Order By a.OrderTestID _
                                              Group a By OrderTest_ID = a.OrderTestID Into Group _
                                              Select OrderTest_ID).ToList

                        For execution = 0 To qExecutionsGrouped.Count - 1
                            Dim indexExecution = execution
                            ' Write execution header
                            Dim myOrderTestData As New OrderTestsDelegate
                            Dim myTestID As Integer

                            resultdata = myOrderTestData.GetTestID(pDBConnection, qExecutionsGrouped(indexExecution)) '0))

                            If Not resultdata.HasError Then
                                Dim myOrderTestDS As New OrderTestsDS
                                myOrderTestDS = CType(resultdata.SetDatos, OrderTestsDS)
                                myTestID = myOrderTestDS.twksOrderTests.Item(0).TestID
                            End If

                            ' Get test data
                            Dim myTestsData As New TestsDelegate
                            Dim myTestsDS As New TestsDS
                            resultdata = myTestsData.Read(pDBConnection, myTestID)
                            If Not resultdata.HasError And Not resultdata.SetDatos Is Nothing Then
                                myTestsDS = CType(resultdata.SetDatos, TestsDS)
                            End If

                            Dim myRefWaveLength As Integer = -1
                            Dim myMainWaveLength As Integer = -1

                            Select Case myTestsDS.tparTests.First.ReadingMode
                                Case "MONO"
                                    If Not myTestsDS.tparTests.First.IsMainWavelengthNull Then myMainWaveLength = CInt(myTestsDS.tparTests.Item(0).MainWavelength)
                                    If Not myTestsDS.tparTests.First.IsReferenceWavelengthNull Then myRefWaveLength = CInt(myTestsDS.tparTests.Item(0).ReferenceWavelength)

                                Case "BIC"
                                    If Not myTestsDS.tparTests.First.IsMainWavelengthNull Then myMainWaveLength = CInt(myTestsDS.tparTests.Item(0).MainWavelength)
                                    If Not myTestsDS.tparTests.First.IsReferenceWavelengthNull Then myRefWaveLength = CInt(myTestsDS.tparTests.Item(0).ReferenceWavelength)
                            End Select

                            ' Find position for main and reference wave lenght
                            Dim myTestName As String = myTestsDS.tparTests.First.TestName
                            Dim myheadcell1 As String = ""

                            'DL 12/01/2012. Substitute OrderTestID by SampleID. Begin 
                            'Dim myOrdersDelegate As New OrdersDelegate
                            'Dim Orderdata As New GlobalDataTO   'Call the Order to get the patient id or the Sample ID
                            'Dim mySampleID As String = ""

                            'Orderdata = myOrdersDelegate.GetSampleIDbyOrderTestID(pDBConnection, CType(qExecutionsGrouped(indexExecution).ToString, Integer))
                            'If Not Orderdata.HasError Then
                            '    Dim myOrdersDS As New OrderTestsDetailsDS
                            '    myOrdersDS = DirectCast(Orderdata.SetDatos, OrderTestsDetailsDS)

                            '    If myOrdersDS.OrderTestsDetails.Rows.Count > 0 Then
                            '        If Not myOrdersDS.OrderTestsDetails.First.IsSampleIDNull Then
                            '            mySampleID = myOrdersDS.OrderTestsDetails.First.SampleID     'Set the patienID if not null
                            '        ElseIf Not myOrdersDS.OrderTestsDetails.First.IsPatientIDNull Then
                            '            mySampleID = myOrdersDS.OrderTestsDetails.First.PatientID     'Set the sampleID if not null
                            '        End If
                            '    End If
                            'End If
                            Dim Orderdata As New GlobalDataTO   'Call the Order to get the patient id or the Sample ID

                            Orderdata = myExecutionsDelegate.GetByOrderTestID(pDBConnection, _
                                                                              myAnalyzerID, _
                                                                              myWSID, _
                                                                              CType(qExecutionsGrouped(indexExecution).ToString, Integer))

                            Dim myElementID As String = ""
                            If (Not Orderdata.HasError AndAlso Not Orderdata.SetDatos Is Nothing) Then
                                Dim myExecutionsDS As ExecutionsDS
                                myExecutionsDS = DirectCast(Orderdata.SetDatos, ExecutionsDS)

                                If myExecutionsDS.vwksWSExecutionsMonitor.Rows.Count > 0 Then
                                    Select Case myExecutionsDS.vwksWSExecutionsMonitor.First.SampleClass
                                        Case "PATIENT"
                                            myElementID = myExecutionsDS.vwksWSExecutionsMonitor.First.PatientID

                                            'Case "BLANK"
                                            '   myElementID = ""

                                        Case "CTRL", "CALIB"
                                            myElementID = myExecutionsDS.vwksWSExecutionsMonitor.First.ElementName
                                    End Select
                                End If
                            End If

                            'myheadcell1 &= "OrderTest: " & qExecutionsGrouped(indexExecution).ToString & " ("
                            myheadcell1 &= "SampleID: " & myElementID & " ("
                            'DL 12/01/2012. End

                            Dim qExecutionInfo As List(Of ExecutionsDS.twksWSExecutionsRow)
                            qExecutionInfo = (From a In myExecutionDS.twksWSExecutions _
                                              Where a.OrderTestID = qExecutionsGrouped(indexExecution) _
                                              Select a Order By a.RerunNumber, a.ReplicateNumber).ToList

                            If qExecutionInfo.Count > 0 Then
                                Dim myRerunNumber As Integer = 0
                                Dim myheadcell2 As String = ""
                                Dim MaxRowGroup As Integer = myCellRow 'DL 27.09.2012
                                For rerunPointer As Integer = 0 To qExecutionInfo.Count - 1
                                    If MaxRowGroup > myCellRow Then myCellRow = MaxRowGroup 'DL 27.09.2012
                                    If myRerunNumber <> qExecutionInfo(rerunPointer).RerunNumber Then
                                        myRerunNumber = qExecutionInfo(rerunPointer).RerunNumber

                                        myheadcell2 = ""
                                        If Not qExecutionInfo(rerunPointer).IsSampleClassNull Then myheadcell2 &= qExecutionInfo(rerunPointer).SampleClass.Trim
                                        'myheadcell2 &= " ) Test " & myTestName & " Well / Rotor "
                                        myheadcell2 &= " ) Test " & myTestName & " Rerun Number = "

                                        If Not qExecutionInfo(rerunPointer).IsRerunNumberNull Then
                                            myheadcell2 &= qExecutionInfo(rerunPointer).RerunNumber.ToString & ""
                                        Else
                                            myheadcell2 &= "? "
                                        End If

                                        Dim myFinalHeadCell As String = ""
                                        myFinalHeadCell = myheadcell1 & myheadcell2
                                        SetCellValue(myPage, "A" & myCellRow, myFinalHeadCell)

                                        'DL 12/01/2012. Begin
                                        'myRango = "A" & myCellRow & ":K" & myCellRow
                                        myRango = "A" & myCellRow & ":M" & myCellRow
                                        'DL 12/01/2012. End
                                        SetCellColor(myPage, myRango, 6)
                                        MergeCells(myPage, myRango)

                                        'AG 26/04/2011 - Well and Base lines headers depends on replicate number
                                        setReplicatesHeader(myCellRow, qExecutionInfo, myPage, "M")

                                        SetCellValue(myPage, "A" & myCellRow, "ABS")
                                        'DL 12/01/2012. Begin
                                        'myRango = "A" & myCellRow & ":K" & myCellRow
                                        myRango = "A" & myCellRow & ":M" & myCellRow
                                        'DL 12/01/2012. End
                                        SetCellColor(myPage, myRango, 34)
                                        MergeCells(myPage, myRango)

                                        myCellRow += 1

                                        Dim qExecutionList As New List(Of twksWSExecutionsRow)
                                        qExecutionList = (From a In myExecutionDS.twksWSExecutions _
                                                              Where a.OrderTestID = qExecutionsGrouped(indexExecution) _
                                                                And a.AnalyzerID = myAnalyzerID _
                                                                And a.WorkSessionID = myWSID _
                                                                And a.RerunNumber = myRerunNumber _
                                                               Order By a.ReplicateNumber _
                                                              Select a).ToList

                                        Dim myCellCol As Integer

                                        'DL 22/07/2011 add Diode column
                                        SetCellValue(myPage, cExcel(0) & myCellRow, "Diode")
                                        myRango = "A" & myCellRow & ":" & cExcel(0) & myCellRow
                                        SetCellColor(myPage, myRango, 19)
                                        SetHorizontalAlignment(myPage, myRango, 3, True)
                                        ' DL 22/07/2011

                                        For indexList As Integer = 0 To qExecutionList.Count - 1
                                            Dim tmpCellRow As Integer = myCellRow
                                            ''JVV 1330 - 15/10/2013
                                            'myCellCol = indexList + 1 ' myCellCol = indexList ' dl 22/07/2011
                                            myCellCol = CInt(IIf(qExecutionList.Count > 1, indexList * 2 + 1, indexList + 1))
                                            ''JVV 1330 - 15/10/2013

                                            ''JVV 1330 - 15/10/2013
                                            'SetCellValue(myPage, cExcel(myCellCol) & tmpCellRow, "  Rep" & indexList + 1 & "  ")
                                            'myRango = "A" & tmpCellRow & ":" & cExcel(myCellCol) & myCellRow
                                            'SetCellColor(myPage, myRango, 19)
                                            'SetHorizontalAlignment(myPage, myRango, 3, True)
                                            SetCellValue(myPage, cExcel(myCellCol) & tmpCellRow, "  Rep" & indexList + 1 & "  ")
                                            SetCellValue(myPage, cExcel(myCellCol + 1) & tmpCellRow, "  Pause" & indexList + 1 & "  ")
                                            myRango = "A" & tmpCellRow & ":" & cExcel(myCellCol + 1) & myCellRow
                                            SetCellColor(myPage, myRango, 19)
                                            SetHorizontalAlignment(myPage, myRango, 3, True)
                                            ''JVV 1330 - 15/10/2013
                                            Dim auxCellRow As Integer = tmpCellRow + 1

                                            'DL 25/07/2011
                                            Dim myNumCycles As Integer = -1
                                            Dim FirstCycle As Integer = -1
                                            Dim SecondCycle As Integer = -1

                                            If Not myTestsDS.tparTests.Item(0).IsFirstReadingCycleNull Then FirstCycle = myTestsDS.tparTests.First.FirstReadingCycle
                                            If Not myTestsDS.tparTests.Item(0).IsSecondReadingCycleNull Then SecondCycle = myTestsDS.tparTests.First.SecondReadingCycle

                                            If FirstCycle > SecondCycle Then
                                                myNumCycles = FirstCycle
                                            Else
                                                myNumCycles = SecondCycle
                                            End If
                                            'DL 25/07/2011


                                            If Not qExecutionList(indexList).IsAdjustBaseLineIDNull And Not qExecutionList(indexList).IsBaseLineIDNull Then
                                                resultdata = GetReadingAbsorbances(pDBConnection, _
                                                                                   myAnalyzerID, _
                                                                                   qExecutionList(indexList).ExecutionID, _
                                                                                   myWSID, _
                                                                                   qExecutionList(indexList).AdjustBaseLineID, _
                                                                                   myMainWaveLength, _
                                                                                   myRefWaveLength, _
                                                                                   myNumCycles, _
                                                                                   qExecutionList(indexList).BaseLineID, _
                                                                                   qExecutionList(indexList).WellUsed)

                                                Dim myabs As New AbsorbanceDS
                                                myabs = CType(resultdata.SetDatos, AbsorbanceDS)

                                                For Each AbsRow As AbsorbanceDS.twksAbsorbancesRow In myabs.twksAbsorbances.Rows
                                                    'DL 22/07/2011
                                                    If indexList = 0 Then
                                                        If myTestsDS.tparTests.First.ReadingMode = "BIC" Then
                                                            Select Case AbsRow.WaveLength
                                                                Case myMainWaveLength
                                                                    SetCellValue(myPage, cExcel(0) & auxCellRow, "M")
                                                                Case myRefWaveLength
                                                                    SetCellValue(myPage, cExcel(0) & auxCellRow, "R")
                                                            End Select

                                                        ElseIf myTestsDS.tparTests.First.ReadingMode = "MONO" Then
                                                            SetCellValue(myPage, cExcel(0) & auxCellRow, "M")
                                                        End If
                                                    End If
                                                    'END  DL 22/07/2011

                                                    'SetCellValue(myPage, cExcel(myCellCol) & auxCellRow, CStr(AbsRow.Absorbance))
                                                    SetCellValue(myPage, cExcel(myCellCol) & auxCellRow, AbsRow.Absorbance)
                                                    SetCellValue(myPage, cExcel(myCellCol + 1) & auxCellRow, IIf(AbsRow.Pause = True, 1, 0)) ''JVV 1330 - 15/10/2013
                                                    auxCellRow += 1
                                                Next AbsRow
                                            End If

                                            ''JVV 1330 - 15/10/2013 : Para que salga bien la línea de separación cuando hay más de 70 lecturas
                                            'If indexList = qExecutionList.Count - 1 Then myCellRow = auxCellRow + 1
                                            'DL 27.09.2012
                                            If myCellRow > MaxRowGroup Or auxCellRow > MaxRowGroup Then
                                                MaxRowGroup = auxCellRow
                                            End If
                                            'DL 27.09.2012
                                            If indexList = qExecutionList.Count - 1 Then
                                                myCellRow = MaxRowGroup + 1
                                            End If
                                            ''JVV 1330 - 15/10/2013


                                        Next indexList

                                    End If 'If myRerunNumber <> qExecutionInfo(rerunPointer).RerunNumber Then
                                Next rerunPointer
                            End If 'If qExecutionInfo.Count > 0 Then

                        Next
                    End If
                Next myRowAnalyzer

            Catch ex As Exception
                resultdata.HasError = True
                resultdata.ErrorCode = "SYSTEM_ERROR"
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportCalculations.SheetAbsorbance", EventLogEntryType.Error, False)
            End Try

            Return resultdata
        End Function

        '''' <summary>
        '''' Create Sheet complete
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created By: DL 08/06/2010
        '''' Modified By: RH 02/01/2011 Modify use of SetCellValue(). Pass original numeric value, not the string converted one.
        '''' </remarks>
        Private Function SheetComplete(ByVal pDBConnection As SqlConnection, _
                                       ByVal pAnalyzerDS As WSAnalyzersDS, _
                                       ByVal pWorkSessionDS As WorkSessionsDS, _
                                       ByVal pWorkSheets As Object) As GlobalDataTO
            Dim resultdata As New GlobalDataTO
            Dim myWSID As String = pWorkSessionDS.twksWorkSessions(0).WorkSessionID

            Try
                Dim myHeader As String
                Dim myPage As Object
                Dim myRango As String

                ' Selecet complete sheet
                myPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, Nothing, pWorkSheets, New Object() {5})

                ' Ini Header 
                myHeader = "Results WorkSessionID " & myWSID & " " & _
                           "Creation Date: " & pWorkSessionDS.twksWorkSessions(0).TS_DateTime & " " & _
                           "by " & pWorkSessionDS.twksWorkSessions(0).TS_User

                SetCellValue(myPage, "A3", myHeader, True)
                SetCellColor(myPage, "A1:M3", 36) 'SetCellColor(myPage, "A1:K3", 36) 'dl 12/01/2012
                MergeCells(myPage, "A3:M3") 'MergeCells(myPage, "A3:K3") 'dl 12/01/2012
                'End Header

                ' Header analyzer
                Dim myCellRow As Integer = 5
                For Each myRowAnalyzer As WSAnalyzersDS.twksWSAnalyzersRow In pAnalyzerDS.twksWSAnalyzers.Rows
                    Dim myAnalyzerID As String = myRowAnalyzer.AnalyzerID

                    SetCellValue(myPage, "A" & myCellRow, "AnalyzerID " & myAnalyzerID)
                    SetCellColor(myPage, "A" & myCellRow & ":M" & myCellRow, 19)  'SetCellColor(myPage, "A" & myCellRow & ":K" & myCellRow, 19) 'dl 12/01/2012
                    MergeCells(myPage, "A" & myCellRow & ":M" & myCellRow)  'MergeCells(myPage, "A" & myCellRow & ":K" & myCellRow) 'dl 12/01/2012

                    myCellRow += 2

                    Dim myExecutionsDelegate As New ExecutionsDelegate
                    resultdata = myExecutionsDelegate.GetExecutionByWorkSession(pDBConnection, myAnalyzerID, myWSID, True)

                    If Not resultdata.HasError Then
                        Dim myExecutionDS As New ExecutionsDS
                        myExecutionDS = DirectCast(resultdata.SetDatos, ExecutionsDS)

                        Dim qExecutionsGrouped As New List(Of Integer)
                        'AG 06/07/2011 - add where clausule
                        qExecutionsGrouped = (From a In myExecutionDS.twksWSExecutions _
                                              Where String.Compare(a.ExecutionStatus, "PENDING", False) <> 0 And String.Compare(a.ExecutionStatus, "LOCKED", False) <> 0 _
                                              Order By a.OrderTestID _
                                              Group a By OrderTest_ID = a.OrderTestID Into Group _
                                              Select OrderTest_ID).ToList
                        Dim MaxRowGroup As Integer
                        For executionGroup = 0 To qExecutionsGrouped.Count - 1
                            Dim indexExecutionGroup = executionGroup
                            Dim myOrderTestData As New OrderTestsDelegate
                            Dim myTestID As Integer
                            resultdata = myOrderTestData.GetTestID(pDBConnection, qExecutionsGrouped.Item(indexExecutionGroup))

                            If Not resultdata.HasError Then
                                Dim myOrderTestDS As New OrderTestsDS
                                myOrderTestDS = CType(resultdata.SetDatos, OrderTestsDS)
                                myTestID = myOrderTestDS.twksOrderTests.Item(0).TestID
                            End If

                            ' Get test data
                            Dim myTestsData As New TestsDelegate
                            Dim myTestsDS As New TestsDS
                            resultdata = myTestsData.Read(pDBConnection, myTestID)
                            If Not resultdata.HasError And Not resultdata.SetDatos Is Nothing Then myTestsDS = CType(resultdata.SetDatos, TestsDS)

                            ' Find position for main and reference wave lenght
                            Dim myTestName As String = myTestsDS.tparTests.Item(0).TestName
                            Dim myheadcell1 As String = ""

                            'DL 12/01/2012. Substitute OrderTestID by SampleID. Begin 
                            'Dim myOrdersDelegate As New OrdersDelegate
                            'Dim Orderdata As New GlobalDataTO   'Call the Order to get the patient id or the Sample ID
                            'Dim mySampleID As String = ""

                            'Orderdata = myOrdersDelegate.GetSampleIDbyOrderTestID(pDBConnection, CType(qExecutionsGrouped(indexExecutionGroup).ToString, Integer))
                            'If Not Orderdata.HasError Then
                            '    Dim myOrdersDS As New OrderTestsDetailsDS
                            '    myOrdersDS = DirectCast(Orderdata.SetDatos, OrderTestsDetailsDS)

                            '    If myOrdersDS.OrderTestsDetails.Rows.Count > 0 Then
                            '        If Not myOrdersDS.OrderTestsDetails.First.IsSampleIDNull Then
                            '            mySampleID = myOrdersDS.OrderTestsDetails.First.SampleID     'Set the patienID if not null
                            '        ElseIf Not myOrdersDS.OrderTestsDetails.First.IsPatientIDNull Then
                            '            mySampleID = myOrdersDS.OrderTestsDetails.First.PatientID     'Set the sampleID if not null
                            '        End If
                            '    End If
                            'End If

                            Dim Orderdata As New GlobalDataTO   'Call the Order to get the patient id or the Sample ID

                            Orderdata = myExecutionsDelegate.GetByOrderTestID(pDBConnection, _
                                                                              myAnalyzerID, _
                                                                              myWSID, _
                                                                              CType(qExecutionsGrouped(indexExecutionGroup).ToString, Integer))

                            Dim myElementID As String = ""
                            If (Not Orderdata.HasError AndAlso Not Orderdata.SetDatos Is Nothing) Then
                                Dim myExecutionsDS As ExecutionsDS
                                myExecutionsDS = DirectCast(Orderdata.SetDatos, ExecutionsDS)

                                If myExecutionsDS.vwksWSExecutionsMonitor.Rows.Count > 0 Then
                                    Select Case myExecutionsDS.vwksWSExecutionsMonitor.First.SampleClass
                                        Case "PATIENT"
                                            myElementID = myExecutionsDS.vwksWSExecutionsMonitor.First.PatientID

                                            'Case "BLANK"
                                            '   myElementID = ""

                                        Case "CTRL", "CALIB"
                                            myElementID = myExecutionsDS.vwksWSExecutionsMonitor.First.ElementName
                                    End Select
                                End If
                            End If
                            'myheadcell1 &= "OrderTest: " & qExecutionsGrouped(indexExecutionGroup).ToString & " ("
                            myheadcell1 &= "SampleID: " & myElementID & " ("
                            'DL 12/01/2012. End

                            Dim qExecutionInfo As List(Of ExecutionsDS.twksWSExecutionsRow)
                            qExecutionInfo = (From a In myExecutionDS.twksWSExecutions _
                                              Where a.OrderTestID = qExecutionsGrouped(indexExecutionGroup) _
                                              Select a Order By a.RerunNumber, a.ReplicateNumber).ToList

                            If qExecutionInfo.Count > 0 Then
                                Dim myRerunNumber As Integer = 0

                                For rerunPointer As Integer = 0 To qExecutionInfo.Count - 1
                                    'If MaxRowGroup > myCellRow Then myCellRow = MaxRowGroup 'DL 27.09.2012
                                    If myRerunNumber <> qExecutionInfo(rerunPointer).RerunNumber Then
                                        myRerunNumber = qExecutionInfo(rerunPointer).RerunNumber

                                        Dim myheadcell2 As String = ""

                                        If Not qExecutionInfo(rerunPointer).IsSampleClassNull Then myheadcell2 &= qExecutionInfo(rerunPointer).SampleClass.Trim
                                        'myheadcell &= " ) Test " & myTestName & " Well / Rotor "
                                        myheadcell2 &= " ) Test " & myTestName & " Rerun Number "

                                        If Not qExecutionInfo(rerunPointer).IsRerunNumberNull Then
                                            myheadcell2 &= qExecutionInfo(rerunPointer).RerunNumber.ToString & " "
                                        Else
                                            myheadcell2 &= "? "
                                        End If

                                        Dim myFinalHeadCell As String = ""
                                        myFinalHeadCell = myheadcell1 & myheadcell2

                                        SetCellValue(myPage, "A" & myCellRow, myFinalHeadCell)
                                        myRango = "A" & myCellRow & ":" & "M" & myCellRow ' myRango = "A" & myCellRow & ":" & "K" & myCellRow DL 12/01/2012
                                        SetCellColor(myPage, myRango, 6)
                                        MergeCells(myPage, myRango)

                                        'AG 26/04/2011 - Well and Base lines headers depends on replicate number
                                        setReplicatesHeader(myCellRow, qExecutionInfo, myPage, "M")


                                        SetCellValue(myPage, "A" & myCellRow, "COUNTS")
                                        SetCellColor(myPage, "A" & myCellRow & ":" & "M" & myCellRow, 34) 'SetCellColor(myPage, "A" & myCellRow & ":" & "K" & myCellRow, 34) 'dl 12/01/2012
                                        MergeCells(myPage, "A" & myCellRow & ":" & "M" & myCellRow) 'MergeCells(myPage, "A" & myCellRow & ":" & "K" & myCellRow) 'dl 12/01/2012

                                        myCellRow += 1

                                        Dim qExecutionList As New List(Of twksWSExecutionsRow)
                                        qExecutionList = (From a In myExecutionDS.twksWSExecutions _
                                                              Where a.OrderTestID = qExecutionsGrouped(indexExecutionGroup) _
                                                                And a.AnalyzerID = myAnalyzerID _
                                                                And a.WorkSessionID = myWSID _
                                                                And a.RerunNumber = myRerunNumber _
                                                               Order By a.ReplicateNumber _
                                                              Select a).ToList

                                        Dim myCellCol As Integer
                                        MaxRowGroup = myCellRow 'DL 27.09.2012
                                        For indexExecutionList As Integer = 0 To qExecutionList.Count - 1

                                            Dim tmpCellRow As Integer = myCellRow

                                            myCellCol = (indexExecutionList * 3) 'myCellCol = (indexExecutionList * 2) ''JVV 1330 - 15/10/2013

                                            SetCellValue(myPage, cExcel(myCellCol) & tmpCellRow, "  Rep" & indexExecutionList + 1 & "  ")
                                            SetCellValue(myPage, cExcel(myCellCol + 1) & tmpCellRow, "  Rep" & indexExecutionList + 1 & "  ")
                                            SetCellValue(myPage, cExcel(myCellCol + 2) & tmpCellRow, "  Rep" & indexExecutionList + 1 & "  ") ''JVV 1330 - 15/10/2013
                                            SetCellValue(myPage, cExcel(myCellCol) & tmpCellRow + 1, "  Main  ")
                                            SetCellValue(myPage, cExcel(myCellCol + 1) & tmpCellRow + 1, "  Reference")
                                            SetCellValue(myPage, cExcel(myCellCol + 2) & tmpCellRow + 1, "  Pause") ''JVV 1330 - 15/10/2013

                                            SetCellColor(myPage, "A" & tmpCellRow & ":" & cExcel(myCellCol + 2) & myCellRow + 1, 19)
                                            SetHorizontalAlignment(myPage, "A" & tmpCellRow & ":" & cExcel(myCellCol + 2) & myCellRow + 1, 3, True)

                                            Dim auxCellRow As Integer = tmpCellRow + 2 ''JVV 1330 - 15/10/2013
                                            Dim myReadingsDelegate As New WSReadingsDelegate
                                            resultdata = myReadingsDelegate.GetByWorkSession(pDBConnection, myWSID, myAnalyzerID, qExecutionList(indexExecutionList).ExecutionID)

                                            If Not resultdata.HasError Then
                                                Dim myReadingDS As New twksWSReadingsDS
                                                myReadingDS = DirectCast(resultdata.SetDatos, twksWSReadingsDS)

                                                For Each myRowReadings As twksWSReadingsRow In myReadingDS.twksWSReadings.Rows
                                                    'SetCellValue(myPage, cExcel(myCellCol) & auxCellRow, CType(myRowReadings.MainCounts, String))
                                                    'SetCellValue(myPage, cExcel(myCellCol + 1) & auxCellRow, CType(myRowReadings.RefCounts, String))

                                                    SetCellValue(myPage, cExcel(myCellCol) & auxCellRow, myRowReadings.MainCounts)
                                                    SetCellValue(myPage, cExcel(myCellCol + 1) & auxCellRow, myRowReadings.RefCounts)
                                                    SetCellValue(myPage, cExcel(myCellCol + 2) & auxCellRow, IIf(myRowReadings.Pause = True, 1, 0)) ''JVV 1330 - 15/10/2013
                                                    auxCellRow += 1
                                                Next myRowReadings
                                                'If MaxRowGroup < auxCellRow Then MaxRowGroup = auxCellRow 'DL 27.09.2012
                                            End If

                                            'If indexExecutionList = qExecutionList.Count - 1 Then
                                            '    myCellRow = auxCellRow + 1
                                            'End If
                                            ''JVV 1330 - 15/10/2013 : Para que salga bien la línea de separación cuando hay más de 70 lecturas
                                            'If indexExecutionList = qExecutionList.Count - 1 Then
                                            '    myCellRow = auxCellRow + 1
                                            'End If
                                            'DL 27.09.2012
                                            If myCellRow > MaxRowGroup Or auxCellRow > MaxRowGroup Then
                                                MaxRowGroup = auxCellRow
                                            End If
                                            'DL 27.09.2012
                                            If indexExecutionList = qExecutionList.Count - 1 Then
                                                myCellRow = MaxRowGroup + 1
                                            End If
                                            ''JVV 1330 - 15/10/2013
                                        Next indexExecutionList
                                        ''JVV 1330 - 15/10/2013
                                        'If MaxRowGroup > myCellRow Then
                                        '    myCellRow = MaxRowGroup 'DL 27.09.2012
                                        '    'Else
                                        '    'Dim myVal As String = "A" & myCellRow
                                        '    '   Dim myCell As String = ""
                                        '    '  myCell = GetCell(myPage, myCellRow, 1)
                                        'End If
                                        ''JVV 1330 - 15/10/2013

                                        ' Absorbance !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                                        Dim myRefWaveLength As Integer = -1
                                        Dim myMainWaveLength As Integer = -1

                                        Select Case myTestsDS.tparTests.Item(0).ReadingMode
                                            Case "MONO"
                                                If Not myTestsDS.tparTests.Item(0).IsMainWavelengthNull Then myMainWaveLength = CInt(myTestsDS.tparTests.Item(0).MainWavelength)
                                                If Not myTestsDS.tparTests.Item(0).IsReferenceWavelengthNull Then myRefWaveLength = CInt(myTestsDS.tparTests.Item(0).ReferenceWavelength)

                                            Case "BIC"
                                                If Not myTestsDS.tparTests.Item(0).IsMainWavelengthNull Then myMainWaveLength = CInt(myTestsDS.tparTests.Item(0).MainWavelength)
                                                If Not myTestsDS.tparTests.Item(0).IsReferenceWavelengthNull Then myRefWaveLength = CInt(myTestsDS.tparTests.Item(0).ReferenceWavelength)
                                        End Select

                                        SetCellValue(myPage, "A" & myCellRow, "ABS")
                                        SetCellColor(myPage, "A" & myCellRow & ":M" & myCellRow, 34)  'SetCellColor(myPage, "A" & myCellRow & ":K" & myCellRow, 34) 'dl 12/01/2012
                                        MergeCells(myPage, "A" & myCellRow & ":M" & myCellRow)  'MergeCells(myPage, "A" & myCellRow & ":K" & myCellRow) 'dl 12/01/2012
                                        myCellRow += 1

                                        Dim qExecutionList1 As New List(Of twksWSExecutionsRow)
                                        qExecutionList1 = (From a In myExecutionDS.twksWSExecutions _
                                                              Where a.OrderTestID = qExecutionsGrouped(indexExecutionGroup) _
                                                                And a.AnalyzerID = myAnalyzerID _
                                                                And a.WorkSessionID = myWSID _
                                                                And a.RerunNumber = myRerunNumber _
                                                               Order By a.ReplicateNumber _
                                                              Select a).ToList


                                        'DL 22/07/2011 add Diode column
                                        SetCellValue(myPage, cExcel(0) & myCellRow, "Diode")
                                        myRango = "A" & myCellRow & ":" & cExcel(0) & myCellRow
                                        SetCellColor(myPage, myRango, 19)
                                        SetHorizontalAlignment(myPage, myRango, 3, True)
                                        ' DL 22/07/2011

                                        For indexList As Integer = 0 To qExecutionList1.Count - 1
                                            Dim tmpCellRow As Integer = myCellRow

                                            ''JVV 1330 - 15/10/2013
                                            'myCellCol = indexList + 1 ' myCellCol = indexList
                                            myCellCol = CInt(IIf(qExecutionList1.Count > 1, indexList * 2 + 1, indexList + 1))
                                            ''JVV 1330 - 15/10/2013

                                            SetCellValue(myPage, cExcel(myCellCol) & tmpCellRow, "  Rep" & indexList + 1 & "  ")
                                            SetCellColor(myPage, "A" & tmpCellRow & ":" & cExcel(myCellCol) & myCellRow, 19)
                                            SetHorizontalAlignment(myPage, "A" & tmpCellRow & ":" & cExcel(myCellCol) & myCellRow, 3, True)
                                            ''JVV 1330 - 15/10/2013
                                            SetCellValue(myPage, cExcel(myCellCol + 1) & tmpCellRow, "  Pause" & indexList + 1 & "  ")
                                            SetCellColor(myPage, "A" & tmpCellRow & ":" & cExcel(myCellCol + 1) & myCellRow, 19)
                                            SetHorizontalAlignment(myPage, "A" & tmpCellRow & ":" & cExcel(myCellCol + 1) & myCellRow, 3, True)
                                            ''JVV 1330 - 15/10/2013

                                            Dim auxCellRow As Integer = tmpCellRow + 1

                                            'DL 25/07/2011
                                            Dim myNumCycles As Integer = -1
                                            Dim FirstCycle As Integer = -1
                                            Dim SecondCycle As Integer = -1

                                            If Not myTestsDS.tparTests.Item(0).IsFirstReadingCycleNull Then FirstCycle = myTestsDS.tparTests.First.FirstReadingCycle
                                            If Not myTestsDS.tparTests.Item(0).IsSecondReadingCycleNull Then SecondCycle = myTestsDS.tparTests.First.SecondReadingCycle

                                            If FirstCycle > SecondCycle Then
                                                myNumCycles = FirstCycle
                                            Else
                                                myNumCycles = SecondCycle
                                            End If
                                            'DL 25/07/2011

                                            If Not qExecutionList(indexList).IsAdjustBaseLineIDNull And Not qExecutionList(indexList).IsBaseLineIDNull Then
                                                resultdata = GetReadingAbsorbances(pDBConnection, _
                                                                                   myAnalyzerID, _
                                                                                   qExecutionList(indexList).ExecutionID, _
                                                                                   myWSID, _
                                                                                   qExecutionList(indexList).AdjustBaseLineID, _
                                                                                   myMainWaveLength, _
                                                                                   myRefWaveLength, _
                                                                                   myNumCycles, _
                                                                                   qExecutionList(indexList).BaseLineID, qExecutionList(indexList).WellUsed)


                                                Dim myabs As New AbsorbanceDS
                                                myabs = CType(resultdata.SetDatos, AbsorbanceDS)

                                                For Each AbsRow As AbsorbanceDS.twksAbsorbancesRow In myabs.twksAbsorbances.Rows

                                                    'DL 22/07/2011
                                                    If indexList = 0 Then
                                                        If myTestsDS.tparTests.First.ReadingMode = "BIC" Then
                                                            Select Case AbsRow.WaveLength
                                                                Case myMainWaveLength
                                                                    SetCellValue(myPage, cExcel(0) & auxCellRow, "M")
                                                                Case myRefWaveLength
                                                                    SetCellValue(myPage, cExcel(0) & auxCellRow, "R")
                                                            End Select

                                                        ElseIf myTestsDS.tparTests.First.ReadingMode = "MONO" Then
                                                            SetCellValue(myPage, cExcel(0) & auxCellRow, "M")
                                                        End If
                                                    End If
                                                    'END  DL 22/07/2011

                                                    'SetCellValue(myPage, cExcel(myCellCol) & auxCellRow, CStr(AbsRow.Absorbance))
                                                    SetCellValue(myPage, cExcel(myCellCol) & auxCellRow, AbsRow.Absorbance)
                                                    SetCellValue(myPage, cExcel(myCellCol + 1) & auxCellRow, IIf(AbsRow.Pause = True, 1, 0)) ''JVV 1330 - 15/10/2013
                                                    auxCellRow += 1
                                                Next AbsRow
                                                'If MaxRowGroup < auxCellRow Then MaxRowGroup = auxCellRow 'DL 27.09.2012 ''JVV 1330 - 15/10/2013
                                            End If

                                            ''JVV 1330 - 15/10/2013 : Para que salga bien la línea de separación cuando hay más de 70 lecturas
                                            'If indexList = qExecutionList.Count - 1 Then myCellRow = auxCellRow + 1
                                            'DL 27.09.2012
                                            If myCellRow > MaxRowGroup Or auxCellRow > MaxRowGroup Then
                                                MaxRowGroup = auxCellRow
                                            End If
                                            'DL 27.09.2012
                                            If indexList = qExecutionList.Count - 1 Then
                                                myCellRow = MaxRowGroup + 1
                                            End If
                                            ''JVV 1330 - 15/10/2013

                                        Next indexList

                                        'If MaxRowGroup > myCellRow Then myCellRow = MaxRowGroup 'DL 27.09.2012 ''JVV 1330 - 15/10/2013
                                    End If 'If myRerunNumber <> qExecutionInfo(rerunPointer).RerunNumber Then
                                Next rerunPointer
                            End If 'If qExecutionInfo.Count > 0 Then

                        Next

                    End If
                Next myRowAnalyzer

                '                myWorkbook.Save() DL 20/07/2010  to do

            Catch ex As Exception
                resultdata.HasError = True
                resultdata.ErrorCode = "SYSTEM_ERROR"
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportCalculations.SheetComplete", EventLogEntryType.Error, False)
            End Try

            Return resultdata

        End Function

        ''' <summary>
        ''' Gets the list of Order Tests Results from the Executions
        ''' </summary>
        ''' <remarks>
        ''' Created by:  RH 13/07/2011
        ''' </remarks>
        Private Function LoadExecutionsResults(ByVal pDBConnection As SqlConnection, _
                                               ByVal pActiveAnalyzer As String, ByVal pActiveWorkSession As String) _
                                               As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing

            Try
                Dim myExecutionDelegate As New ExecutionsDelegate
                Dim ExecutionsResultsDS As ExecutionsDS

                resultData = myExecutionDelegate.GetWSExecutionsResults(pDBConnection, pActiveAnalyzer, pActiveWorkSession)

                If (Not resultData.HasError) Then
                    ExecutionsResultsDS = CType(resultData.SetDatos, ExecutionsDS)
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportCalculations.LoadExecutionsResults", EventLogEntryType.Error, False)

            End Try

            Return resultData
        End Function

        '''' <summary>
        '''' Creates Sheet Results By Replicates
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <returns>GlobalDataTO containing sucess/error information</returns>
        '''' <remarks>
        '''' Created By: RH 13/07/2011
        '''' </remarks>
        Private Function SheetResultsByReplicates(ByVal pDBConnection As SqlConnection, _
                                         ByVal pAnalyzerDS As WSAnalyzersDS, _
                                         ByVal pWorkSessionDS As WorkSessionsDS, _
                                         ByVal pWorkSheets As Object) As GlobalDataTO

            Dim resultdata As GlobalDataTO = Nothing

            Try
                Dim XlsPage As Object
                Dim XlsPageHeader As String
                Dim XlsPageRange As String
                Const StrNull As String = "?"

                Dim WSData As WorkSessionsDS.twksWorkSessionsRow = pWorkSessionDS.twksWorkSessions(0)
                Dim ExecutionsResultsDS As ExecutionsDS

                ' Select Results By Replicates Sheet ( 6 )
                XlsPage = pWorkSheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, _
                                                            Nothing, pWorkSheets, New Object() {6})

                ' Write Header 
                XlsPageHeader = String.Format("Results WorkSessionID: {0} Creation Date: {1} by {2}", _
                                         WSData.WorkSessionID, WSData.TS_DateTime, WSData.TS_User)

                SetCellValue(XlsPage, "A3", XlsPageHeader, True)
                MergeCells(XlsPage, "A3:K3")
                SetCellColor(XlsPage, "A1:K3", 36)

                Dim CurrentXlsPageRow As Integer = 5

                Dim ReplicateNumber As String
                Dim WellUsed As String
                Dim AdjustBaseLineID As String
                Dim BaseLineID As String

                For Each AnalyzerRow As WSAnalyzersDS.twksWSAnalyzersRow In pAnalyzerDS.twksWSAnalyzers.Rows
                    SetCellValue(XlsPage, "A" & CurrentXlsPageRow, "AnalyzerID: " & AnalyzerRow.AnalyzerID)
                    XlsPageRange = String.Format("A{0}:K{0}", CurrentXlsPageRow)
                    SetCellColor(XlsPage, XlsPageRange, 19)
                    MergeCells(XlsPage, XlsPageRange)

                    CurrentXlsPageRow += 2

                    resultdata = LoadExecutionsResults(pDBConnection, AnalyzerRow.AnalyzerID, WSData.WorkSessionID)

                    If Not resultdata.HasError Then
                        ExecutionsResultsDS = CType(resultdata.SetDatos, ExecutionsDS)

                        ''JVV 1330 - 15/10/2013
                        Dim readingsDel As New WSReadingsDelegate
                        Dim readingsDS As twksWSReadingsDS = Nothing
                        resultdata = readingsDel.GetByWorkSession(pDBConnection, WSData.WorkSessionID, AnalyzerRow.AnalyzerID)
                        If Not resultdata.HasError Then
                            readingsDS = CType(resultdata.SetDatos, twksWSReadingsDS)
                        End If
                        ''JVV 1330 - 15/10/2013

                        Dim ExecutionRows As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)
                        Dim SampleClasses() As String = {"PATIENT", "CALIB", "CTRL", "BLANK"}
                        Dim Filter As String
                        Dim previousRow As ExecutionsDS.vwksWSExecutionsResultsRow = Nothing

                        For Each SampleClass In SampleClasses
                            Filter = String.Empty

                            ExecutionRows = (From row In ExecutionsResultsDS.vwksWSExecutionsResults _
                                             Where row.SampleClass = SampleClass _
                                             Order By row.OrderTestID _
                                             Select row).ToList()

                            For Each row As ExecutionsDS.vwksWSExecutionsResultsRow In ExecutionRows
                                If Filter <> String.Format("{0}{1}", row.OrderTestID, row.RerunNumber) Then
                                    If Filter <> String.Empty Then
                                        WriteFinalResults(XlsPage, CurrentXlsPageRow, ExecutionRows, _
                                                          previousRow.OrderTestID, previousRow.RerunNumber, SampleClass, readingsDS)
                                    End If

                                    Filter = String.Format("{0}{1}", row.OrderTestID, row.RerunNumber)
                                    SetCellValue(XlsPage, "A" & CurrentXlsPageRow, _
                                                 String.Format("OrderTest: {0} ({1}) Test {2} Rerun Number {3}", row.OrderTestID, SampleClass, row.TestName, row.RerunNumber))
                                    XlsPageRange = String.Format("A{0}:K{0}", CurrentXlsPageRow)
                                    SetCellColor(XlsPage, XlsPageRange, 6)
                                    MergeCells(XlsPage, XlsPageRange)

                                    CurrentXlsPageRow += 1
                                End If

                                If row.IsReplicateNumberNull Then
                                    ReplicateNumber = StrNull
                                Else
                                    ReplicateNumber = row.ReplicateNumber.ToString()
                                End If

                                If row.IsWellUsedNull Then
                                    WellUsed = StrNull
                                Else
                                    WellUsed = row.WellUsed.ToString()
                                End If

                                If row.IsAdjustBaseLineIDNull Then
                                    AdjustBaseLineID = StrNull
                                Else
                                    AdjustBaseLineID = row.AdjustBaseLineID.ToString()
                                End If

                                If row.IsBaseLineIDNull Then
                                    BaseLineID = StrNull
                                Else
                                    BaseLineID = row.BaseLineID.ToString()
                                End If

                                SetCellValue(XlsPage, "B" & CurrentXlsPageRow, _
                                                 String.Format("Replicate number = {0} Well / Rotor = {1} / ? - Adjust BaseLineID = {2} - Well BaseLineID = {3}", _
                                                               ReplicateNumber, WellUsed, AdjustBaseLineID, BaseLineID))

                                XlsPageRange = String.Format("A{0}:K{0}", CurrentXlsPageRow)
                                SetCellColor(XlsPage, XlsPageRange, 6)
                                XlsPageRange = String.Format("B{0}:K{0}", CurrentXlsPageRow)
                                MergeCells(XlsPage, XlsPageRange)

                                CurrentXlsPageRow += 1

                                previousRow = row
                            Next

                            If Filter <> String.Empty Then
                                WriteFinalResults(XlsPage, CurrentXlsPageRow, ExecutionRows, _
                                                          previousRow.OrderTestID, previousRow.RerunNumber, _
                                                          SampleClass, readingsDS)
                            End If
                        Next
                    End If
                Next AnalyzerRow

            Catch ex As Exception
                resultdata = New GlobalDataTO()
                resultdata.HasError = True
                resultdata.ErrorCode = "SYSTEM_ERROR"
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportCalculations.SheetResultsByReplicates", EventLogEntryType.Error, False)

            End Try

            Return resultdata
        End Function

#End Region


#Region "Private Methods"

        '''' Modified By: RH 02/01/2011 Modify use of SetCellValue(). Pass original numeric value, not the string converted one.
        ''' <summary>
        ''' Modified by: JV 15/10/2013 Add the pReadingsDS parameter to update correctly the 'Pause' cell in the Results sheet
        ''' </summary>
        ''' <param name="XlsPage"></param>
        ''' <param name="CurrentXlsPageRow"></param>
        ''' <param name="ExecutionRows"></param>
        ''' <param name="OrderTestID"></param>
        ''' <param name="RerunNumber"></param>
        ''' <param name="SampleClass"></param>
        ''' <param name="pReadingsDS"></param>
        ''' <remarks></remarks>
        Private Sub WriteFinalResults(ByVal XlsPage As Object, ByRef CurrentXlsPageRow As Integer, _
                                      ByVal ExecutionRows As List(Of ExecutionsDS.vwksWSExecutionsResultsRow), _
                                      ByVal OrderTestID As Integer, ByVal RerunNumber As Integer, _
                                      ByVal SampleClass As String, _
                                      ByVal pReadingsDS As twksWSReadingsDS)
            Dim XlsPageRange As String
            Dim FilterRows As List(Of ExecutionsDS.vwksWSExecutionsResultsRow)

            SetCellValue(XlsPage, "A" & CurrentXlsPageRow, "FINAL RESULTS")
            XlsPageRange = String.Format("A{0}:K{0}", CurrentXlsPageRow)
            SetCellColor(XlsPage, XlsPageRange, 34)
            MergeCells(XlsPage, XlsPageRange)

            CurrentXlsPageRow += 2

            Dim CurrentCol As Integer
            Const Decimals As Integer = 4
            Dim StrNull As String = String.Empty
            Dim TmpValue As Single
            Dim iPaused As Integer = 0 ''JVV 1330 - 15/10/2013

            Select Case SampleClass
                Case "PATIENT", "CTRL"
                    FilterRows = (From fRow In ExecutionRows _
                       Where fRow.OrderTestID = OrderTestID AndAlso fRow.RerunNumber = RerunNumber _
                       Select fRow).ToList()

                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow), "Replicates")
                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow + 1), "Abs")
                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow + 2), "Conc")
                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow + 3), "Pause") ''JVV 1330 - 15/10/2013

                    SetCellColor(XlsPage, String.Format("A{0}:K{0}", CurrentXlsPageRow), 19)
                    SetCellColor(XlsPage, String.Format("A{0}:A{0}", CurrentXlsPageRow + 1), 19)
                    SetCellColor(XlsPage, String.Format("A{0}:A{0}", CurrentXlsPageRow + 2), 19)
                    SetCellColor(XlsPage, String.Format("A{0}:A{0}", CurrentXlsPageRow + 3), 19) ''JVV 1330 - 15/10/2013

                    CurrentCol = 1
                    Dim ABS_Value As String
                    Dim CONC_Value As String

                    For Each fRow In FilterRows
                        'RH 03/01/2011
                        SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow), String.Format("Rep{0}", fRow.ReplicateNumber))

                        If fRow.IsABS_ValueNull Then
                            ABS_Value = StrNull

                            'RH 03/01/2011
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 1), ABS_Value)
                        Else
                            ABS_Value = fRow.ABS_Value.ToStringWithDecimals(Decimals)

                            'RH 03/01/2011
                            TmpValue = Single.Parse(ABS_Value)
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 1), TmpValue)
                        End If

                        If fRow.IsCONC_ValueNull Then
                            CONC_Value = StrNull

                            'RH 03/01/2011
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 2), CONC_Value)
                        Else
                            CONC_Value = fRow.CONC_Value.ToStringWithDecimals(Decimals)

                            'RH 03/01/2011
                            TmpValue = Single.Parse(CONC_Value)
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 2), TmpValue)
                        End If

                        'JVV 1330 - 15/10/2013
                        'Assure there are some paused-readings and inform the Paused cell with 1 or 0
                        iPaused = 0
                        If Not pReadingsDS Is Nothing Then iPaused = (From p In pReadingsDS.twksWSReadings Where p.ExecutionID = fRow.ExecutionID And p.Pause = True Select p).Count()
                        SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 3), IIf(iPaused > 0, 1, 0)) ''JVV 1330 - 15/10/2013
                        'JVV 1330 - 15/10/2013

                        'SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow), String.Format("Rep{0}", fRow.ReplicateNumber))
                        'SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 1), ABS_Value)
                        'SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 2), CONC_Value)

                        CurrentCol += 1
                    Next

                    CurrentXlsPageRow += 5 'CurrentXlsPageRow += 4 'JVV 1330 - 15/10/2013

                Case "CALIB"
                    FilterRows = (From fRow In ExecutionRows _
                       Where fRow.OrderTestID = OrderTestID AndAlso fRow.RerunNumber = RerunNumber _
                       Select fRow).ToList()

                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow), "Replicates")
                    SetCellColor(XlsPage, String.Format("A{0}:K{0}", CurrentXlsPageRow), 19)

                    Dim MaxItemNumber As Integer = 0
                    Dim ABS_Value As String

                    For Each fRow In FilterRows
                        'If fRow.IsABS_ValueNull Then
                        '    ABS_Value = StrNull
                        'Else
                        '    ABS_Value = fRow.ABS_Value.ToStringWithDecimals(Decimals)
                        'End If

                        SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(fRow.ReplicateNumber), CurrentXlsPageRow), String.Format("Rep{0}", fRow.ReplicateNumber))
                        SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow + fRow.MultiItemNumber), String.Format("Abs (kit {0})", fRow.MultiItemNumber))
                        SetCellColor(XlsPage, String.Format("A{0}:A{0}", CurrentXlsPageRow + fRow.MultiItemNumber), 19)

                        'SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(fRow.ReplicateNumber), CurrentXlsPageRow + fRow.MultiItemNumber), ABS_Value)

                        'RH 03/01/2011
                        If fRow.IsABS_ValueNull Then
                            ABS_Value = StrNull
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(fRow.ReplicateNumber), CurrentXlsPageRow + fRow.MultiItemNumber), ABS_Value)
                        Else
                            ABS_Value = fRow.ABS_Value.ToStringWithDecimals(Decimals)
                            TmpValue = Single.Parse(ABS_Value)
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(fRow.ReplicateNumber), CurrentXlsPageRow + fRow.MultiItemNumber), TmpValue)
                        End If

                        ''JVV 1330 - 15/10/2013
                        SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow + fRow.MultiItemNumber + 1), "Pause")
                        SetCellColor(XlsPage, String.Format("A{0}:A{0}", CurrentXlsPageRow + fRow.MultiItemNumber + 1), 19)
                        'JVV 1330 - 15/10/2013
                        'Assure there are some paused-readings and inform the Paused cell with 1 or 0
                        iPaused = 0
                        If Not pReadingsDS Is Nothing Then iPaused = (From p In pReadingsDS.twksWSReadings Where p.ExecutionID = fRow.ExecutionID And p.Pause = True Select p).Count()
                        SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(fRow.ReplicateNumber), CurrentXlsPageRow + fRow.MultiItemNumber + 1), IIf(iPaused > 0, 1, 0))
                        'JVV 1330 - 15/10/2013
                        ''JVV 1330 - 15/10/2013

                        If MaxItemNumber < fRow.MultiItemNumber Then MaxItemNumber = fRow.MultiItemNumber
                    Next

                    CurrentXlsPageRow += 3 + MaxItemNumber 'CurrentXlsPageRow += 2 + MaxItemNumber ''JVV 1330 - 15/10/2013

                    If MaxItemNumber > 1 Then 'Multipoint Calibrator
                        CurrentXlsPageRow += 1

                        SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow), "Curve points")
                        SetCellColor(XlsPage, String.Format("A{0}:K{0}", CurrentXlsPageRow), 19)
                        CurrentXlsPageRow += 1

                        SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow), "Point")
                        SetCellValue(XlsPage, String.Format("B{0}", CurrentXlsPageRow), "X (Conc)")
                        SetCellValue(XlsPage, String.Format("C{0}", CurrentXlsPageRow), "Y (Abs)")
                        SetCellColor(XlsPage, String.Format("A{0}:K{0}", CurrentXlsPageRow), 19)
                        CurrentXlsPageRow += 1

                        Dim resultData As GlobalDataTO
                        Dim myResultsDelegate As New ResultsDelegate()

                        'Get all results for current OrderTestID.
                        resultData = myResultsDelegate.GetResults(Nothing, OrderTestID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim AverageResultsDS As ResultsDS
                            AverageResultsDS = CType(resultData.SetDatos, ResultsDS)

                            Dim CurveResultsID As Integer = AverageResultsDS.vwksResults(0).CurveResultsID

                            Dim myCurveResultsDelegate As New CurveResultsDelegate()
                            resultData = myCurveResultsDelegate.GetResults(Nothing, CurveResultsID)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim curveDS As CurveResultsDS
                                curveDS = DirectCast(resultData.SetDatos, CurveResultsDS)

                                For Each curveRow As CurveResultsDS.twksCurveResultsRow In curveDS.twksCurveResults.Rows
                                    'SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow), curveRow.CurvePoint.ToString())
                                    'SetCellValue(XlsPage, String.Format("B{0}", CurrentXlsPageRow), curveRow.CONCValue.ToStringWithDecimals(Decimals))
                                    'SetCellValue(XlsPage, String.Format("C{0}", CurrentXlsPageRow), curveRow.ABSValue.ToStringWithDecimals(Decimals))

                                    'RH 03/01/2011
                                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow), curveRow.CurvePoint)

                                    TmpValue = Single.Parse(curveRow.CONCValue.ToStringWithDecimals(Decimals))
                                    SetCellValue(XlsPage, String.Format("B{0}", CurrentXlsPageRow), TmpValue)

                                    TmpValue = Single.Parse(curveRow.ABSValue.ToStringWithDecimals(Decimals))
                                    SetCellValue(XlsPage, String.Format("C{0}", CurrentXlsPageRow), TmpValue)

                                    CurrentXlsPageRow += 1
                                Next
                            Else
                                SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow), resultData.ErrorMessage)
                                SetCellColor(XlsPage, String.Format("A{0}:K{0}", CurrentXlsPageRow), 3)
                                CurrentXlsPageRow += 1
                            End If
                        Else
                            SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow), resultData.ErrorMessage)
                            SetCellColor(XlsPage, String.Format("A{0}:K{0}", CurrentXlsPageRow), 3)
                            CurrentXlsPageRow += 1
                        End If

                        CurrentXlsPageRow += 1
                    End If

                Case "BLANK"
                    FilterRows = (From fRow In ExecutionRows _
                       Where fRow.OrderTestID = OrderTestID AndAlso fRow.RerunNumber = RerunNumber _
                       Select fRow).ToList()

                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow), "Replicates")
                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow + 1), "Abs")
                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow + 2), "Abs Initial")
                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow + 3), "Abs MainFilter")

                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow + 4), "Abs WorkReagent")
                    XlsPageRange = String.Format("A{0}:A{0}", CurrentXlsPageRow + 4)
                    SetAutofit(XlsPage, XlsPageRange)

                    SetCellValue(XlsPage, String.Format("A{0}", CurrentXlsPageRow + 5), "Pause") ''JVV 1330 - 15/10/2013

                    SetCellColor(XlsPage, String.Format("A{0}:K{0}", CurrentXlsPageRow), 19)
                    SetCellColor(XlsPage, String.Format("A{0}:A{0}", CurrentXlsPageRow + 1), 19)
                    SetCellColor(XlsPage, String.Format("A{0}:A{0}", CurrentXlsPageRow + 2), 19)
                    SetCellColor(XlsPage, String.Format("A{0}:A{0}", CurrentXlsPageRow + 3), 19)
                    SetCellColor(XlsPage, String.Format("A{0}:A{0}", CurrentXlsPageRow + 4), 19)
                    SetCellColor(XlsPage, String.Format("A{0}:A{0}", CurrentXlsPageRow + 5), 19) ''JVV 1330 - 15/10/2013

                    CurrentCol = 1
                    Dim ABS_Value As String
                    Dim ABS_Initial As String
                    Dim ABS_MainFilter As String
                    Dim Abs_WorkReagent As String = StrNull

                    For Each fRow In FilterRows
                        'RH 03/01/2011
                        SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow), String.Format("Rep{0}", fRow.ReplicateNumber))

                        If fRow.IsABS_ValueNull Then
                            ABS_Value = StrNull

                            'RH 03/01/2011
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 1), ABS_Value)
                        Else
                            ABS_Value = fRow.ABS_Value.ToStringWithDecimals(Decimals)

                            'RH 03/01/2011
                            TmpValue = Single.Parse(ABS_Value)
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 1), TmpValue)
                        End If

                        If fRow.IsABS_InitialNull Then
                            ABS_Initial = StrNull

                            'RH 03/01/2011
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 2), ABS_Initial)
                        Else
                            ABS_Initial = fRow.ABS_Initial.ToStringWithDecimals(Decimals)

                            'RH 03/01/2011
                            TmpValue = Single.Parse(ABS_Initial)
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 2), TmpValue)
                        End If

                        If fRow.IsABS_MainFilterNull Then
                            ABS_MainFilter = StrNull

                            'RH 03/01/2011
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 3), ABS_MainFilter)
                        Else
                            ABS_MainFilter = fRow.ABS_MainFilter.ToStringWithDecimals(Decimals)

                            'RH 03/01/2011
                            TmpValue = Single.Parse(ABS_MainFilter)
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 3), TmpValue)
                        End If

                        If fRow.IsAbs_WorkReagentNull Then
                            Abs_WorkReagent = StrNull

                            'RH 03/01/2011
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 4), Abs_WorkReagent)
                        Else
                            Abs_WorkReagent = fRow.Abs_WorkReagent.ToStringWithDecimals(Decimals)

                            'RH 03/01/2011
                            TmpValue = Single.Parse(Abs_WorkReagent)
                            SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 4), TmpValue)
                        End If

                        'JVV 1330 - 15/10/2013
                        'Assure there are some paused-readings and inform the Paused cell with 1 or 0
                        iPaused = 0
                        If Not pReadingsDS Is Nothing Then iPaused = (From p In pReadingsDS.twksWSReadings Where p.ExecutionID = fRow.ExecutionID And p.Pause = True Select p).Count()
                        SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 5), IIf(iPaused > 0, 1, 0)) ''JVV 1330 - 15/10/2013
                        'JVV 1330 - 15/10/2013

                        'SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow), String.Format("Rep{0}", fRow.ReplicateNumber))
                        'SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 1), ABS_Value)
                        'SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 2), ABS_Initial)
                        'SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 3), ABS_MainFilter)
                        'SetCellValue(XlsPage, String.Format("{0}{1}", cExcel(CurrentCol), CurrentXlsPageRow + 4), Abs_WorkReagent)

                        CurrentCol += 1
                    Next

                    CurrentXlsPageRow += 7 'CurrentXlsPageRow += 6 'JVV 1330 - 15/10/2013

            End Select
        End Sub

        Private Sub CloseProcess(ByVal pDateIniProcess As Date, _
                        ByVal pTypeExcel As Type, _
                        ByVal pExcel As Object, _
                        ByVal pBook As Object)

            ' save excel book
            pBook.GetType.InvokeMember("Save", BindingFlags.InvokeMethod, Nothing, pBook, Nothing)

            ' dlm 20/07/2010 Close the excel instance down ...
            pTypeExcel.InvokeMember("Quit", Reflection.BindingFlags.InvokeMethod, Nothing, pExcel, Nothing)
            ' dlm 20/07/2010 release it from memory ...
            Runtime.InteropServices.Marshal.ReleaseComObject(pExcel)

            ' kill excel process
            'DL code - doesnt work
            'Dim proceso As System.Diagnostics.Process()
            'proceso = System.Diagnostics.Process.GetProcessesByName("EXCEL")
            'For Each opro As System.Diagnostics.Process In proceso
            '    If opro.StartTime >= pDateIniProcess Then
            '        opro.Kill()
            '    End If
            'Next opro

            'AG 05/01/2011 - copied and adapted from iPRO comm dll            
            For Each p As Process In Process.GetProcessesByName("EXCEL")
                If p.CloseMainWindow() Then
                Else
                    p.Kill()
                End If
            Next
            ' end kill excel process
        End Sub


        ''' <summary>
        ''' Set M for main and R for reference wave length
        ''' </summary>
        ''' <param name="pArray"></param>
        ''' <param name="pRefWaveLength"></param>
        ''' <param name="pNumCycles"></param>
        ''' <returns>Array of string</returns>
        ''' <remarks></remarks>
        Private Function SetPositionsArrays(ByVal pArray() As String, _
                                            ByVal pRefWaveLength As Integer, _
                                            ByVal pNumCycles As Integer) As String()

            Try
                Dim index As Integer
                Dim maxLimit As Integer

                'DL 25/07/2011
                'If pNumCycles > 0 And pNumCycles < UBound(pArray) Then
                ' maxLimit = pNumCycles - 1
                'Else
                'maxLimit = UBound(pArray)
                'End If
                If pNumCycles > 0 And pNumCycles < UBound(pArray) + 1 Then
                    maxLimit = pNumCycles - 1
                Else
                    maxLimit = UBound(pArray)
                End If
                'END DL 25/07/2011

                If pRefWaveLength = -1 Then
                    For index = 0 To maxLimit
                        pArray(index) = "M"
                    Next index
                Else
                    pArray(maxLimit) = "M"

                    For index = (maxLimit - 1) To 0 Step -1
                        If pArray(index + 1) = "M" Then
                            pArray(index) = "R"
                        Else
                            pArray(index) = "M"
                        End If
                    Next index
                End If

                ' new dl 10/03/2011
                Dim MaxNumberCycles As Integer '= 67 ' poner como constante parametrizable por admin.

                'If pNumCycles = -1 Then
                MaxNumberCycles = UBound(pArray)
                'End If

                ReDim Preserve pArray(MaxNumberCycles)
                For i As Integer = (maxLimit + 1) To MaxNumberCycles

                    If pRefWaveLength = -1 Then
                        pArray(i) = "M"
                    Else
                        Select Case pArray(i - 1)
                            Case "M"
                                pArray(i) = "R"

                            Case "R"
                                pArray(i) = "M"
                        End Select
                    End If
                Next i
                ' end dl 10/03/2011

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultsFileDelegate.SetPositionsArrays", EventLogEntryType.Error, False)
            End Try

            Return pArray

        End Function

        ' DL
        'Modified by RH 27/02/2012 Code optimization.
        Public Function GetWaveLength(ByVal pdbconnection As SqlConnection, _
                                      ByVal pAnalyzerID As String, _
                                      ByVal pWavelengthPos As Integer, _
                                      ByVal pMainWaveLength As Integer, _
                                      ByVal pRefWaveLength As Integer) As GlobalDataTO

            Dim resultdata As GlobalDataTO = Nothing
            Dim myWaveLength As String = String.Empty

            Try
                Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDelegate

                resultdata = myAnalyzerLedPositions.GetByLedPosition(pdbconnection, pAnalyzerID, pWavelengthPos)

                If Not resultdata.HasError AndAlso Not resultdata.SetDatos Is Nothing Then
                    Dim myAnalyzerLedPositionsDS As AnalyzerLedPositionsDS
                    myAnalyzerLedPositionsDS = CType(resultdata.SetDatos, AnalyzerLedPositionsDS)

                    If myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.Rows.Count > 0 Then
                        If pMainWaveLength = myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.First.WaveLength Then
                            myWaveLength = "M"

                        ElseIf pRefWaveLength = myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.First.WaveLength Then
                            myWaveLength = "R"

                        Else
                            resultdata.HasError = True
                            resultdata.ErrorMessage = "ResultsFileDelegate.GetWaveLength: Led position not found: " & pWavelengthPos
                        End If
                    End If
                End If


            Catch ex As Exception
                resultdata = New GlobalDataTO()
                resultdata.HasError = True
                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultsFileDelegate.GetWaveLength", EventLogEntryType.Error, False)
            End Try

            resultdata.SetDatos = myWaveLength

            Return resultdata
        End Function

        ''' <summary>
        ''' Gets data for showing Absorbance Curve info in grid control
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestID">Order test identifier</param>
        ''' <param name="pRerunNumber">Rerun Number</param>
        ''' <param name="pMultiItemNumber">MultiItem Number</param>
        ''' <param name="pExecutions">A list of vwksWSAbsorbanceDS.vwksWSAbsorbanceRow</param>
        ''' <param name="pAllowDecimals">AllowDecimals for converting Single values into String</param>
        ''' <returns>GlobalDataTO containing a GraphDS Dataset if success, error message otherwise</returns>
        ''' <remarks>
        ''' Created by: RH 27/02/2012 Based on previous code by DL
        ''' </remarks>
        Public Function GetDataForAbsCurve(ByVal pDBConnection As SqlClient.SqlConnection, _
                                           ByVal pOrderTestID As Integer, _
                                           ByVal pRerunNumber As Integer, _
                                           ByVal pMultiItemNumber As Integer, _
                                           ByVal pExecutions As List(Of vwksWSAbsorbanceDS.vwksWSAbsorbanceRow), _
                                           ByVal pAllowDecimals As Integer) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim ReplicateDS = New GraphDS

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then

                        If pExecutions.Count > 0 Then
                            Dim myAbsorbancesDS As AbsorbanceDS
                            Dim myCycleRow As GraphDS.tReplicatesRow
                            Dim myTestsData As New TestsDelegate
                            Dim myTestsDS As TestsDS
                            Dim myTestsRow As TestsDS.tparTestsRow
                            Dim myWavelengthPos As String = String.Empty
                            Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDelegate
                            Dim myAnalyzerLedPositionsDS As AnalyzerLedPositionsDS
                            'Dim myAnalyzerLedPositionsRow As AnalyzerLedPositionsDS.tcfgAnalyzerLedPositionsRow
                            Dim myAnalyzerLedPositionsList As List(Of AnalyzerLedPositionsDS.tcfgAnalyzerLedPositionsRow)

                            resultData = myTestsData.GetList(dbConnection)

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                myTestsDS = CType(resultData.SetDatos, TestsDS)
                            Else
                                resultData.SetDatos = ReplicateDS
                                Return resultData
                            End If

                            resultData = myAnalyzerLedPositions.GetAllWaveLengths(dbConnection, pExecutions(0).AnalyzerID)

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                myAnalyzerLedPositionsDS = CType(resultData.SetDatos, AnalyzerLedPositionsDS)
                            Else
                                resultData.SetDatos = ReplicateDS
                                Return resultData
                            End If

                            For i As Integer = 0 To pExecutions.Count - 1
                                ' ReSharper disable once InconsistentNaming
                                Dim aux_i = i
                                resultData = GetReadingAbsorbancesByExecution( _
                                                dbConnection, pExecutions(aux_i).ExecutionID, pExecutions(aux_i).AnalyzerID, _
                                                pExecutions(aux_i).WorkSessionID, False) 'AG 09/03/2011 - change True for False

                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    myAbsorbancesDS = CType(resultData.SetDatos, AbsorbanceDS)

                                    For x As Integer = 0 To myAbsorbancesDS.twksAbsorbances.Count - 1
                                        Dim auxX = x
                                        myCycleRow = ReplicateDS.tReplicates.NewtReplicatesRow()

                                        If String.Compare(pExecutions(aux_i).ReadingMode, "BIC", False) = 0 Then
                                            myWavelengthPos = String.Empty

                                            'resultData = myTestsData.Read(dbConnection, pExecutions(i).TestID)

                                            'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                            'myTestsDS = CType(resultData.SetDatos, TestsDS)

                                            myTestsRow = (From row As TestsDS.tparTestsRow In myTestsDS.tparTests _
                                                          Where row.TestID = pExecutions(aux_i).TestID _
                                                          Select row).ToList().First()

                                            'resultData = GetWaveLength(dbConnection, _
                                            '                           myAbsorbancesDS.twksAbsorbances(x).AnalyzerID, _
                                            '                           myAbsorbancesDS.twksAbsorbances(x).WavelengthPos, _
                                            '                           myTestsDS.tparTests.Item(0).MainWavelength, _
                                            '                           myTestsDS.tparTests.Item(0).ReferenceWavelength)

                                            myAnalyzerLedPositionsList = _
                                                    (From row As AnalyzerLedPositionsDS.tcfgAnalyzerLedPositionsRow In myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions _
                                                     Where row.LedPosition = myAbsorbancesDS.twksAbsorbances(auxX).WavelengthPos _
                                                     Select row).ToList()

                                            'If Not resultData.HasError Then
                                            If myAnalyzerLedPositionsList.Count > 0 Then
                                                If myTestsRow.MainWavelength = myAnalyzerLedPositionsList.First().WaveLength Then
                                                    myWavelengthPos = "M"
                                                Else
                                                    myWavelengthPos = "R"
                                                End If

                                                'myWavelengthPos = CStr(resultData.SetDatos)

                                                Select Case myWavelengthPos
                                                    Case "M"
                                                        With myAbsorbancesDS
                                                            If .twksAbsorbances(auxX).Absorbance = -1 Then
                                                                'AG 15/10/2012
                                                                'myCycleRow.Abs1 = "Error"
                                                                myCycleRow.Abs1 = GlobalConstants.ABSORBANCE_INVALID_VALUE.ToString 'Error
                                                            Else
                                                                myCycleRow.Abs1 = .twksAbsorbances(auxX).Absorbance.ToStringWithDecimals(pAllowDecimals)
                                                            End If

                                                            If auxX > 0 AndAlso .twksAbsorbances(auxX - 1).WaveLength <> .twksAbsorbances(auxX).WaveLength Then
                                                                If .twksAbsorbances(auxX).Absorbance <> -1 AndAlso .twksAbsorbances(auxX - 1).Absorbance <> -1 Then
                                                                    'ToDo: Check how to show this value, in Abs value or the original signed value
                                                                    'myCycleRow.Diff = Math.Abs(.twksAbsorbances(x).Absorbance - .twksAbsorbances(x - 1).Absorbance).ToStringWithDecimals(pAllowDecimals)

                                                                    myCycleRow.Diff = (.twksAbsorbances(auxX).Absorbance - .twksAbsorbances(auxX - 1).Absorbance).ToStringWithDecimals(pAllowDecimals)
                                                                End If
                                                            End If
                                                        End With

                                                    Case "R"
                                                        If myAbsorbancesDS.twksAbsorbances(auxX).Absorbance = -1 Then
                                                            'AG 15/10/2012
                                                            'myCycleRow.Abs2 = "Error"
                                                            myCycleRow.Abs2 = GlobalConstants.ABSORBANCE_INVALID_VALUE.ToString 'Error
                                                        Else
                                                            myCycleRow.Abs2 = myAbsorbancesDS.twksAbsorbances(auxX).Absorbance.ToStringWithDecimals(pAllowDecimals)
                                                        End If

                                                End Select
                                            End If

                                            'End If

                                        ElseIf String.Compare(pExecutions(aux_i).ReadingMode, "MONO", False) = 0 Then

                                            If myAbsorbancesDS.twksAbsorbances(auxX).Absorbance = -1 Then
                                                'AG 15/10/2012
                                                'myCycleRow.Abs1 = "Error"
                                                myCycleRow.Abs1 = GlobalConstants.ABSORBANCE_INVALID_VALUE.ToString 'Error
                                            Else
                                                myCycleRow.Abs1 = myAbsorbancesDS.twksAbsorbances(auxX).Absorbance.ToStringWithDecimals(pAllowDecimals)
                                            End If

                                        End If

                                        myCycleRow.Replicate = pExecutions(aux_i).ReplicateNumber
                                        myCycleRow.Cycle = myAbsorbancesDS.twksAbsorbances(auxX).ReadingNumber
                                        myCycleRow.ExecutionID = pExecutions(aux_i).ExecutionID
                                        '//Changes for TASK + BUGS Tracking  #1331
                                        '// CF - Added the Pause Column to the dataset
                                        myCycleRow.Pause = myAbsorbancesDS.twksAbsorbances(auxX).Pause
                                        ReplicateDS.tReplicates.Rows.Add(myCycleRow)
                                    Next x

                                    'AG 15/10/2012 - keep error message until presentation layer
                                ElseIf resultData.HasError AndAlso resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString Then
                                    Exit For
                                End If
                            Next i
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultsFileDelegate.GetDataForAbsCurve", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            'resultData.HasError = False 'AG 15/10/2012 - WHY THE ERROR FLAG IS REMOVED???
            resultData.SetDatos = ReplicateDS

            Return resultData
        End Function

        ''' <summary>
        ''' Calculate absorbance for all reading by a execution identifier
        ''' </summary>
        ''' <param name="pdbconnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pExecutionID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <param name="pAdjustBaseLineID"></param>
        ''' <param name="pMainWaveLength"></param>
        ''' <param name="pNumCycles"></param>
        ''' <returns>GlobalDataTO</returns>
        ''' <remarks>Created DL
        ''' AG 04/01/2011 - add pBaseLineWellID and pWellUsed parameters and used into GetByWaveLength</remarks>
        Private Function GetReadingAbsorbances(ByVal pdbconnection As SqlConnection, _
                                               ByVal pAnalyzerID As String, _
                                               ByVal pExecutionID As Integer, _
                                               ByVal pWorkSessionID As String, _
                                               ByVal pAdjustBaseLineID As Integer, _
                                               ByVal pMainWaveLength As Integer, _
                                               ByVal pRefWaveLength As Integer, _
                                               ByVal pNumCycles As Integer, _
                                               ByVal pBaseLineWellID As Integer, _
                                               ByVal pWellUsed As Integer) As GlobalDataTO

            Dim resultdata As New GlobalDataTO

            Try
                Dim myAnalyzerModel As String
                Dim myAnalyzersDelegate As New AnalyzersDelegate
                Dim myAnalizerDS As New AnalyzersDS

                ' Get analyzer model
                resultdata = myAnalyzersDelegate.GetAnalyzerModel(pdbconnection, pAnalyzerID)
                If Not resultdata.HasError Then
                    myAnalizerDS = CType(resultdata.SetDatos, AnalyzersDS)
                    myAnalyzerModel = myAnalizerDS.tcfgAnalyzers.Item(0).AnalyzerModel

                    ' Get values for a PATH_LENGHT and LIMIT_ABS parameters
                    Dim mySwParametersDelegate As New SwParametersDelegate
                    resultdata = mySwParametersDelegate.ReadByParameterName(pdbconnection, GlobalEnumerates.SwParameters.PATH_LENGHT.ToString, Nothing)

                    If Not resultdata.HasError Then
                        Dim myParameterDS As New ParametersDS
                        Dim myPathLenght As Single

                        myParameterDS = DirectCast(resultdata.SetDatos, ParametersDS)
                        myPathLenght = myParameterDS.tfmwSwParameters(0).ValueNumeric

                        resultdata = mySwParametersDelegate.ReadByParameterName(pdbconnection, GlobalEnumerates.SwParameters.LIMIT_ABS.ToString, Nothing)

                        If Not resultdata.HasError Then
                            myParameterDS = DirectCast(resultdata.SetDatos, ParametersDS)

                            Dim myAbsLimit As Single = myParameterDS.tfmwSwParameters(0).ValueNumeric
                            Dim myReadingDS As New twksWSReadingsDS
                            Dim myReadingsDelegate As New WSReadingsDelegate

                            resultdata = myReadingsDelegate.GetByWorkSession(pdbconnection, pWorkSessionID, pAnalyzerID, pExecutionID)

                            If Not resultdata.HasError Then
                                Dim AbsorbanceTestFlag As Boolean = False

                                myReadingDS = DirectCast(resultdata.SetDatos, twksWSReadingsDS)

                                'Dim vArray(myReadingDS.twksWSReadings.Rows.Count - 1) As String
                                Dim index As Integer = 0
                                Dim myABS As Single
                                Dim myWaveLength As Integer
                                Dim myWaveLengthPos As Integer
                                Dim myAbsorbanceDS As New AbsorbanceDS
                                '//14/10/2013 - CF - v3.0.0 - This var will be added added to the resulting DS from this method. 
                                Dim pause As Boolean = False
                                ' Dim iNumCycles As Integer = 0
                                Dim myAnalyzerLedPositions As New AnalyzerLedPositionsDelegate
                                'Dim WLdata As GlobalDataTO
                                Dim myWL As String

                                For Each myRowReadings As twksWSReadingsRow In myReadingDS.twksWSReadings.Rows
                                    resultdata = GetWaveLength(pdbconnection, pAnalyzerID, myRowReadings.LedPosition, pMainWaveLength, pRefWaveLength)

                                    '//14/10/2013 - CF - v3.0.0 - Added the Pause column 
                                    pause = myRowReadings.Pause
                                    If Not resultdata.HasError Then
                                        myWL = CType(resultdata.SetDatos, String)

                                        Select Case myWL
                                            'Select Case vArray(index)
                                            Case "M"

                                                resultdata = myAnalyzerLedPositions.GetByWaveLength(pdbconnection, pAnalyzerID, CStr(pMainWaveLength))
                                                If Not resultdata.HasError Then
                                                    Dim myAnalyzerLedPositionsDS As New AnalyzerLedPositionsDS

                                                    myAnalyzerLedPositionsDS = CType(resultdata.SetDatos, AnalyzerLedPositionsDS)

                                                    myWaveLengthPos = myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.Item(0).LedPosition
                                                    myWaveLength = pMainWaveLength
                                                End If

                                            Case "R"

                                                resultdata = myAnalyzerLedPositions.GetByWaveLength(pdbconnection, pAnalyzerID, CStr(pRefWaveLength))
                                                If Not resultdata.HasError Then
                                                    Dim myAnalyzerLedPositionsDS As New AnalyzerLedPositionsDS

                                                    myAnalyzerLedPositionsDS = CType(resultdata.SetDatos, AnalyzerLedPositionsDS)

                                                    myWaveLengthPos = myAnalyzerLedPositionsDS.tcfgAnalyzerLedPositions.Item(0).LedPosition
                                                    myWaveLength = pRefWaveLength
                                                End If
                                        End Select

                                        Dim myBaseLineDelegate As New WSBLinesDelegate
                                        Dim myMainDark As Integer
                                        Dim myRefDark As Integer
                                        Dim myMainLight As Integer
                                        Dim myRefLight As Integer

                                        resultdata = myBaseLineDelegate.GetByWaveLength(pdbconnection, pAnalyzerID, pWorkSessionID, pAdjustBaseLineID, myWaveLengthPos, pBaseLineWellID, pWellUsed)
                                        If Not resultdata.HasError Then
                                            Dim myBaseLineDS As New BaseLinesDS

                                            myBaseLineDS = CType(resultdata.SetDatos, BaseLinesDS)

                                            If myBaseLineDS.twksWSBaseLines.Rows.Count > 0 Then
                                                myMainDark = myBaseLineDS.twksWSBaseLines(0).MainDark
                                                myRefDark = myBaseLineDS.twksWSBaseLines(0).RefDark
                                                myMainLight = myBaseLineDS.twksWSBaseLines(0).MainLight
                                                myRefLight = myBaseLineDS.twksWSBaseLines(0).RefLight
                                            End If
                                        End If

                                        Dim myCalculationsDelegate As New CalculationsDelegate()
                                        Dim myAbsorbanceRow As AbsorbanceDS.twksAbsorbancesRow = myAbsorbanceDS.twksAbsorbances.NewtwksAbsorbancesRow

                                        With myAbsorbanceRow
                                            .AnalyzerID = pAnalyzerID
                                            .AdjustBaseLineID = pAdjustBaseLineID
                                            .BaseLineWellID = pBaseLineWellID
                                            .WellUsed = pWellUsed
                                            .DarkMainCounts = myMainDark
                                            .DarkRefCounts = myRefDark
                                            .ExecutionID = pExecutionID
                                            .ExecutionMainCounts = myRowReadings.MainCounts
                                            .ExecutionRefCounts = myRowReadings.RefCounts
                                            .LightMainCounts = myMainLight
                                            .LightRefCounts = myRefLight
                                            .ReadingNumber = myRowReadings.ReadingNumber
                                            .WaveLength = myWaveLength
                                            .WavelengthPos = myWaveLengthPos
                                            .WorkSessionID = pWorkSessionID
                                            .Pause = pause '// 14/10/2013 - CF - v3.0.0 Added tge pause value to the DS. to show in the grid
                                            '//Changes for TASK + BUGS Tracking  #1331
                                            resultdata = myCalculationsDelegate.CalculateAbsorbance(.ExecutionMainCounts, _
                                                                                                    .ExecutionRefCounts, _
                                                                                                    .LightMainCounts, _
                                                                                                    .LightRefCounts, _
                                                                                                    .DarkMainCounts, _
                                                                                                    .DarkRefCounts, _
                                                                                                    0, _
                                                                                                    myPathLenght, _
                                                                                                    myAbsLimit, _
                                                                                                    AbsorbanceTestFlag)

                                            myABS = GlobalConstants.CALCULATION_ERROR_VALUE
                                            'AG 11/10/2012 - Once the abs cycle has been marked as invalid, remove the globalDataTo.HasError flag
                                            '                unless the error code = SYSTEM_ERROR, in this case return the error
                                            'If Not resultdata.HasError Then myABS = CType(resultdata.SetDatos, Single)
                                            If Not resultdata.HasError Then
                                                myABS = CType(resultdata.SetDatos, Single)
                                            ElseIf resultdata.ErrorCode <> GlobalEnumerates.AbsorbanceErrors.SYSTEM_ERROR.ToString Then
                                                resultdata.HasError = False 'Remove error, abs will show ERROR
                                            Else
                                                Exit For 'System Error
                                            End If

                                            .Absorbance = myABS
                                        End With

                                        myAbsorbanceDS.twksAbsorbances.AddtwksAbsorbancesRow(myAbsorbanceRow)

                                        index += 1
                                    End If

                                Next myRowReadings

                                resultdata.SetDatos = myAbsorbanceDS
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultdata.HasError = True
                resultdata.ErrorCode = "SYSTEM_ERROR"
                resultdata.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ResultsFileDelegate.GetReadingAbsorbances", EventLogEntryType.Error, False)
            End Try

            Return resultdata

        End Function

        'DL 21/07/2010
        'RH 02/01/2011 Modify pValue from String to Object
        'SGM 05/06/2013 - Inform the current UIculture 
        Private Sub SetCellValue(ByVal pPage As Object, ByVal pCell As String, ByVal pValue As Object, Optional ByVal pBold As Boolean = False)

            Try
                Dim myRange As Object = pPage.GetType().InvokeMember("Range", BindingFlags.GetProperty, Nothing, pPage, New Object() {pCell})

                myRange.GetType().InvokeMember("Value", BindingFlags.SetProperty, Nothing, myRange, New Object() {pValue})
                'RH 15/02/2012 Set the Number Format (Single)
                'myRange.GetType().InvokeMember("NumberFormat", BindingFlags.SetProperty, Nothing, myRange, New Object() {XlsSingleFormat}
                myRange.GetType().InvokeMember("NumberFormat", BindingFlags.SetProperty, Nothing, myRange, New Object() {XlsSingleFormat}, System.Globalization.CultureInfo.CurrentUICulture) 'SGM 05/06/2013 - Inform the current UIculture 


                If pBold Then
                    Dim myFont As Object

                    myFont = myRange.GetType().InvokeMember("Font", BindingFlags.GetProperty, Nothing, myRange, Nothing)
                    myRange.GetType().InvokeMember("Bold", BindingFlags.SetProperty, Nothing, myFont, New Object() {True})
                End If

            Catch ex As Exception
                Dim w32ex = TryCast(ex, Win32Exception)
                If w32ex Is Nothing Then
                    w32ex = TryCast(ex.InnerException, Win32Exception)
                End If
                If w32ex IsNot Nothing Then
                    ' do stuff
                    Dim code As Integer = w32ex.ErrorCode
                End If

            End Try


        End Sub

        'DL 27/09/2012. GetCell
        'Private Function GetCell(ByVal cells As Object, ByVal row As Integer, ByVal column As Integer) As String
        '    Dim parameters As Object() = New [Object](1) {}
        '    parameters(0) = row
        '    parameters(1) = column
        '    Dim myobj As Object
        '    'Return
        '    myobj = cells.[GetType]().InvokeMember("Item", BindingFlags.GetProperty, Nothing, cells, parameters).ToString()
        '    Return myobj
        'End Function




        ' Merge cells
        Private Sub MergeCells(ByVal pPage As Object, ByVal pRango As String)
            Dim myRange As Object

            myRange = pPage.GetType().InvokeMember("Range", BindingFlags.GetProperty, Nothing, pPage, New Object() {pRango})
            myRange.GetType().InvokeMember("MergeCells", BindingFlags.SetProperty, Nothing, myRange, New Object() {True})
        End Sub

        'Range.Interior.ColorIndex
        Private Sub SetCellColor(ByVal pPage As Object, ByVal pRango As String, ByVal pColor As Integer)
            Dim myRange As Object
            Dim myInterior As Object

            myRange = pPage.GetType().InvokeMember("Range", BindingFlags.GetProperty, Nothing, pPage, New Object() {pRango})
            myInterior = myRange.GetType().InvokeMember("Interior", BindingFlags.GetProperty, Nothing, myRange, Nothing)
            myRange.GetType().InvokeMember("ColorIndex", BindingFlags.SetProperty, Nothing, myInterior, New Object() {pColor})
        End Sub

        Private Sub SetHorizontalAlignment(ByVal pPage As Object, ByVal pRango As String, ByVal pAlign As Integer, Optional ByVal pAutofit As Boolean = False)
            Dim myRange As Object

            myRange = pPage.GetType().InvokeMember("Range", BindingFlags.GetProperty, Nothing, pPage, New Object() {pRango})
            myRange.GetType().InvokeMember("HorizontalAlignment", BindingFlags.SetProperty, Nothing, myRange, New Object() {pAlign})

            If pAutofit Then
                Dim myColumns As Object

                ' Get all the columns in the range of cells where the data are in
                myColumns = myRange.GetType().InvokeMember("Columns", BindingFlags.GetProperty, Nothing, myRange, Nothing)
                ' Set columns to auto-fit the actual contents
                myColumns.GetType().InvokeMember("Autofit", BindingFlags.InvokeMethod, Nothing, myColumns, Nothing)
            End If
        End Sub

        Private Sub SetAutofit(ByVal pPage As Object, ByVal pRango As String)
            Dim myRange As Object

            myRange = pPage.GetType().InvokeMember("Range", BindingFlags.GetProperty, Nothing, pPage, New Object() {pRango})

            Dim myColumns As Object

            ' Get all the columns in the range of cells where the data are in
            myColumns = myRange.GetType().InvokeMember("Columns", BindingFlags.GetProperty, Nothing, myRange, Nothing)

            ' Set columns to auto-fit the actual contents
            myColumns.GetType().InvokeMember("Autofit", BindingFlags.InvokeMethod, Nothing, myColumns, Nothing)
        End Sub

        Private Sub setReplicatesHeader(ByRef pCellRow As Integer, _
                                        ByVal pExecutionList As List(Of ExecutionsDS.twksWSExecutionsRow), _
                                        ByVal pPage As Object, _
                                        ByVal pcolLetter As String)

            'AG 26/04/2011
            pCellRow += 1
            Dim myreplhead As String = ""
            Dim myrango As String = ""

            For replicateNumber As Integer = 0 To pExecutionList.Count - 1
                'DL 12/01/2012. Begin
                myreplhead = "PreparationID = "
                If Not pExecutionList(replicateNumber).IsPreparationIDNull Then
                    myreplhead &= pExecutionList(replicateNumber).PreparationID
                Else
                    myreplhead &= "? "
                End If
                'myreplhead = "Replicate number = "
                myreplhead &= " Replicate number = "
                'DL 12/01/2012. End
                If Not pExecutionList(replicateNumber).IsReplicateNumberNull Then
                    myreplhead &= pExecutionList(replicateNumber).ReplicateNumber
                Else
                    myreplhead &= "? "
                End If

                myreplhead &= " Well / Rotor = "
                If Not pExecutionList(replicateNumber).IsWellUsedNull Then
                    myreplhead &= pExecutionList(replicateNumber).WellUsed & " / ?"
                Else
                    myreplhead &= "? / ?"
                End If

                myreplhead &= " - Adjust BaseLineID = "
                If Not pExecutionList(replicateNumber).IsAdjustBaseLineIDNull Then
                    myreplhead &= pExecutionList(replicateNumber).AdjustBaseLineID
                Else
                    myreplhead = "?"
                End If

                myreplhead &= " - Well BaseLineID = "
                If Not pExecutionList(replicateNumber).IsBaseLineIDNull Then
                    myreplhead &= pExecutionList(replicateNumber).BaseLineID
                Else
                    myreplhead = "?"
                End If

                SetCellValue(pPage, "B" & pCellRow, myreplhead)
                myrango = "A" & pCellRow & ":" & pcolLetter & pCellRow      'myrango = "A" & pCellRow & ":K" & pCellRow 'dl 12/01/2012
                SetCellColor(pPage, myrango, 6)
                myrango = "B" & pCellRow & ":" & pcolLetter & pCellRow      'myrango = "B" & pCellRow & ":K" & pCellRowdl 12/01/2012
                MergeCells(pPage, myrango)
                pCellRow += 1
            Next replicateNumber
            'AG 26/04/2011

        End Sub

#End Region

    End Class

End Namespace
