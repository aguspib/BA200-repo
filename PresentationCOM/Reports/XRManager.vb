Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.BL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates

'Imports System.Globalization

Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraPrinting
'Imports DevExpress.XtraCharts

Public Class XRManager

#Region "Declarations"
    Private Shared mtPortrait As MASTERTEMPLATE = Nothing
    Private Shared mtPortraitPath As String = String.Empty

    Private Shared mtLandscape As MASTERTEMPLATE = Nothing
    Private Shared mtLandscapePath As String = String.Empty

    Private Shared ReadOnly DatePattern As String = SystemInfoManager.OSDateFormat
    Private Shared ReadOnly TimePattern As String = SystemInfoManager.OSShortTimeFormat

#End Region


#Region "Public Methods"

    ''' <summary>
    ''' 25/09/2013 - CF - v2.1.1
    ''' This function silently prints the PatientsFinalReport filtered by a List of OrderIDs
    ''' </summary>
    ''' <param name="pAnalyzerID"></param>
    ''' <param name="pWorkSessionID"></param>
    ''' <param name="pOrderList">List of orders to filter by. </param>
    ''' <remarks>
    ''' </remarks>
    Public Shared Sub PrintPatientsFinalReport(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pOrderList As List(Of String))
        Try
            'PrintCompactPatientsReport(pAnalyzerID, pWorkSessionID, New List(Of String))
            Dim resultData As GlobalDataTO
            Dim myResultsDelegate As New ResultsDelegate

            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            If (pOrderList Is Nothing) Then
                resultData = myResultsDelegate.GetResultsByPatientSampleForReport(Nothing, pAnalyzerID, pWorkSessionID, False)
            Else
                resultData = myResultsDelegate.GetResultsByPatientSampleForReportByOrderList(Nothing, pAnalyzerID, pWorkSessionID, pOrderList, False)
            End If



            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ResultsData As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                'TR 10/07/2012 -Validate if there are results to open report screen 
                If ResultsData.ReportSampleMaster.Count > 0 Then
                    Dim Report As New PatientsFinalReport

                    'Multilanguage. Get texts from DB.
                    Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurrentResults_Patient", CurrentLanguage)
                    Report.SetHeaderLabel(literalHeaderLabel)


              
                    'EF 04/06/2014 (labels for titles)
                    Report.XrLabel_PatientID.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", CurrentLanguage) & ":"
                    Report.XrLabel_PatientName.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Summary_PatientName", CurrentLanguage) & ":"
                    Report.XrLabel_Gender.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Gender", CurrentLanguage) & ":"
                    Report.XrLabel_DateBirth.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DateOfBirth", CurrentLanguage) & ":"
                    Report.XrLabel_Age.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Age", CurrentLanguage) & ":"
                    'EF 04/06/2014 END


                    Report.XrLabelTest.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", CurrentLanguage)
                    Report.XrLabelType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", CurrentLanguage)
                    Report.XrLabelConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", CurrentLanguage)
                    Report.XrLabelRefranges.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Short", CurrentLanguage)
                    Report.XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", CurrentLanguage)
                    Report.XrLabelFlags.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Flags", CurrentLanguage)  'EF 03/06/2014 (cargar texto LBL_FLags)

                    Dim WSStartDateTime As String = String.Empty

                    'Get WSStartDateTime from DB
                    Dim myWSDelegate As New WorkSessionsDelegate

                    resultData = myWSDelegate.GetByWorkSession(Nothing, pWorkSessionID)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                        If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                            WSStartDateTime = myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern) & " " & _
                                                myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern)
                        End If
                    End If

                    Report.XrWSStartDateTimeLabel.Text = WSStartDateTime
                    Report.DataSource = ResultsData
                    SilentReportPrinting(Report)
                    'ShowPortrait(Report)
                End If

                'Else
                'ToDo: Try the error
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowPatientsFinalReport", EventLogEntryType.Error, False)

        End Try
    End Sub


    ''' <summary>
    ''' CF 17/09/2013 - V2.1.1 / 3.0.0
    ''' Creation of the AutomaticPatientsReport, the user can configure automatic printing of this report
    ''' in the configuration screen. If selected, it can print at the end of each session, or after 
    ''' individual patient results are calculated. 
    ''' This function will automatically generate a report and print it without any user interaction. 
    ''' </summary>
    ''' <param name="pAnalyzerID">Analyzer ID parameter. </param>
    ''' <param name="pWorkSessionID">Worksession ID paramter</param>
    ''' <param name="pOrderList">OrderList to filter by</param>
    ''' <param name="SilentPrint">SilentPrint if true will print directly to the printer. If false, will show the report portrait on screen</param>
    ''' <remarks>TODO: make sure that the data comes from the correct place
    ''' AG 03/10/2013 - New optional parameter set to TRUE into methods GetResultsByPatientSampleForReport and GetResultsByPatientSampleForReportByOrderList
    ''' AG 07/10/2013 - Modify: For the compact report do not fill the header "Remarks", leave ""
    ''' </remarks>
    Public Shared Sub PrintCompactPatientsReport(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pOrderList As List(Of String), Optional ByVal SilentPrint As Boolean = True)
        Try
            Dim resultData As GlobalDataTO
            Dim myResultsDelegate As New ResultsDelegate
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            If (pOrderList Is Nothing) Then
                resultData = myResultsDelegate.GetResultsByPatientSampleForReport(Nothing, pAnalyzerID, pWorkSessionID, False, True)
            Else
                resultData = myResultsDelegate.GetResultsByPatientSampleForReportByOrderList(Nothing, pAnalyzerID, pWorkSessionID, pOrderList, False, True)
            End If

            'If there's on data to print, we'll get out of this sub.
            If resultData.HasError OrElse resultData.SetDatos Is Nothing Then
                Return
            End If

            Dim ResultsData As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)
            If ResultsData.ReportSampleMaster.Count <= 0 Then
                Return 'If there are no results, we get out of this sub. 
            End If
            '//We use the "using" statement for proper resource allocation and disposal. 
            Using automaticReport As New AutomaticPatientsReport()
                Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurrentResults_Patient", CurrentLanguage)
                'TODO: Get the correct resource for the new report text. 
                automaticReport.SetHeaderLabel(literalHeaderLabel)
                automaticReport.XrLabelPatientID.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", CurrentLanguage)
                automaticReport.XrLabelTest.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", CurrentLanguage)
                automaticReport.XrLabelType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", CurrentLanguage)
                automaticReport.XrLabelConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", CurrentLanguage)
                automaticReport.XrLabelRefranges.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Short", CurrentLanguage)
                automaticReport.XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", CurrentLanguage)
                'AG 07/10/2013
                'automaticReport.XrLabelRemarks.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Flags", CurrentLanguage)  'EF 03/06/2014 (cargar texto LBL_FLags)
                automaticReport.XrLabelRemarks.Text = ""
                'AG 07/10/2013
                automaticReport.XrLabelDate.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", CurrentLanguage)
                Dim WSStartDateTime As String = String.Empty
                Dim myWSDelegate As New WorkSessionsDelegate()
                resultData = myWSDelegate.GetByWorkSession(Nothing, pWorkSessionID)
                If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                    Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                    If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                        WSStartDateTime = String.Format("{0} {1}", _
                                              myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern), _
                                              myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern))
                    End If
                End If
                automaticReport.XrWSStartDateTimeLabel.Text = WSStartDateTime

                automaticReport.DataSource = Nothing
                automaticReport.DataMember = ""

                automaticReport.DataSource = ResultsData
                automaticReport.DataMember = "ReportSampleDetails"

                If (SilentPrint) Then
                    SilentReportPrinting(automaticReport)
                Else
                    ShowPortrait(automaticReport)
                End If

                ''this DevExpress object can send a report directly to the printer without user interaction necessary so long 
                '' as the "AutoShowParametersPanel is set to False. 
                'automaticReport.ShowPreview()
                'Dim reportPrintTool As New ReportPrintTool(automaticReport) With {.AutoShowParametersPanel = False}
                'reportPrintTool.ShowPreviewDialog()
                'reportPrintTool.Print()
            End Using

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.PrintAutomaticPatientsReport", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' CF - 18/09/2013 - v2.1.1 / 3.0.0
    ''' This method will take an instance of an object that implements the IReport interface from the DevExpress.XtraReports assembly.
    ''' If there are no errors in the type conversions, it will print silently to the workstation's default printer. 
    ''' </summary>
    ''' <param name="aReport">An object instance that implements the DevExpress.XtraReports.IReport interface</param>
    Public Shared Sub SilentReportPrinting(ByVal aReport As Object)

        Try

            'If String.IsNullOrEmpty(mtPortraitPath) Then
            If Not LoadDefaultPortraitTemplate() Then
                'No Reports template has been loaded.
                'Take the proper action!
                Return
            End If
            'End If
            Dim axTraReport = CType(aReport, XtraReport)

            Dim band As Band = axTraReport.Bands.GetBandByType(GetType(TopMarginBand))
            If Not (band Is Nothing) Then axTraReport.Bands.Remove(band)

            band = axTraReport.Bands.GetBandByType(GetType(PageHeaderBand))
            If Not (band Is Nothing) Then axTraReport.Bands.Remove(band)

            axTraReport.Landscape = False

            axTraReport.Bands.Add(mtPortrait.Bands.GetBandByType(GetType(TopMarginBand)).Band)
            axTraReport.Bands.Add(mtPortrait.Bands.GetBandByType(GetType(TopMarginBand)).Band)

            Dim reportPrintTool As ReportPrintTool = New ReportPrintTool(axTraReport)
            Try
                reportPrintTool.AutoShowParametersPanel = False
                reportPrintTool.Print()
            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "XRManager.SendToPrinter", EventLogEntryType.Error, False)
            Finally
                If reportPrintTool IsNot Nothing Then
                    reportPrintTool.Dispose()
                End If
            End Try
        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.SilentReportPrinting", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' Show the report according the rotor content positions.
    ''' </summary>
    ''' <param name="pCellPositionsInformation"></param>
    '''               Created by:  JV - 15/11/2013 - #1382 reagents report creation
    '''                            JV - 19/11/2013 - #1382 change some header descriptions and resize the report. Also, adjust the report colors.
    '''                            TR - 01/04/2014 - #1562 Add the Lot Number descriptionn and resize the report.
    ''' <remarks></remarks>
    Public Shared Sub ShowRotorContentByPositionReport(ByVal pCellPositionsInformation As CellPositionInformationDS)
        Try
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            If pCellPositionsInformation Is Nothing OrElse pCellPositionsInformation.Tables.Count = 0 OrElse pCellPositionsInformation.Tables("ReportTable").Rows.Count = 0 Then
                Return
            End If

            Using rotorReport As New RotorContentPositionsReport()
                Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents", CurrentLanguage)
                rotorReport.SetHeaderLabel(literalHeaderLabel)
                rotorReport.XrWSStartDateTimeLabel.Text = String.Format("{0} {1}", DateTime.Now.ToString(DatePattern), DateTime.Now.ToString(TimePattern))

                rotorReport.XrLabelCell.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_RotorCell", CurrentLanguage)
                rotorReport.XrLabelName.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", CurrentLanguage)
                rotorReport.XrLabelBarcode.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "MENU_BARCODE", CurrentLanguage)
                rotorReport.XrLabelExpDate.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Short", CurrentLanguage)
                rotorReport.XrLabelBottle.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", CurrentLanguage)
                rotorReport.XrLabelVolume.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Volumen", CurrentLanguage)
                rotorReport.XrLabelRemainingTests.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", CurrentLanguage)
                rotorReport.XrLabelStatus.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_MainMDI_Status", CurrentLanguage)
                rotorReport.XrLabel1.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", CurrentLanguage)

                rotorReport.DataSource = Nothing
                rotorReport.DataMember = ""
                rotorReport.DataSource = pCellPositionsInformation
                rotorReport.DataMember = "ReportTable"
                ShowPortrait(rotorReport)
            End Using
        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowRotorContentByPositionReport", EventLogEntryType.Error, False)
        End Try
    End Sub

    ''' <summary>
    ''' Loads the reports default Portrait template into cache
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 28/11/2011
    ''' </remarks>
    Public Shared Function LoadDefaultPortraitTemplate() As Boolean
        Try
            Dim resultData As GlobalDataTO
            Dim templateList As New ReportTemplatesDelegate

            resultData = templateList.GetDefaultTemplate(Nothing, Orientation.PORTRAIT)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ReportTemplate As ReportTemplatesDS = CType(resultData.SetDatos, ReportTemplatesDS)

                If ReportTemplate.tcfgReportTemplates.Rows.Count > 0 Then
                    Dim Row As ReportTemplatesDS.tcfgReportTemplatesRow
                    Row = ReportTemplate.tcfgReportTemplates(0)

                    Dim TemplatePath As String = GlobalBase.AppPath & GlobalBase.ReportPath & "\"
                    TemplatePath &= Row.TemplateFileName

                    mtPortraitPath = TemplatePath
                    mtPortrait = New MASTERTEMPLATE
                    mtPortrait.LoadLayout(TemplatePath)

                    Return True
                End If
            End If

            Return False

        Catch ex As Exception
            mtPortrait = Nothing
            mtPortraitPath = String.Empty

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.LoadDefaultPortraitTemplate", EventLogEntryType.Error, False)
        End Try

        Return False
    End Function

    ''' <summary>
    ''' Loads the reports default Landscape template into cache
    ''' </summary>
    ''' <remarks>
    ''' Created by: RH 28/11/2011
    ''' </remarks>
    Public Shared Function LoadDefaultLandscapeTemplate() As Boolean
        Try
            Dim resultData As GlobalDataTO
            Dim templateList As New ReportTemplatesDelegate

            resultData = templateList.GetDefaultTemplate(Nothing, Orientation.LANDSCAPE)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ReportTemplate As ReportTemplatesDS = CType(resultData.SetDatos, ReportTemplatesDS)

                If ReportTemplate.tcfgReportTemplates.Rows.Count > 0 Then
                    Dim Row As ReportTemplatesDS.tcfgReportTemplatesRow
                    Row = ReportTemplate.tcfgReportTemplates(0)

                    Dim TemplatePath As String = GlobalBase.AppPath & GlobalBase.ReportPath & "\"
                    TemplatePath &= Row.TemplateFileName

                    mtLandscapePath = TemplatePath
                    mtLandscape = New MASTERTEMPLATE
                    mtLandscape.LoadLayout(TemplatePath)

                    Return True
                End If
            End If

            Return False

        Catch ex As Exception
            mtLandscape = Nothing
            mtLandscapePath = String.Empty

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.LoadDefaultLandscapeTemplate", EventLogEntryType.Error, False)
        End Try

        Return False
    End Function

    Public Shared Sub ShowUsersReport()
        Try
            Dim resultData As GlobalDataTO
            Dim myUserConfigurationDelegate As New UserConfigurationDelegate

            'DL 13/01/2012. Begin
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Multilanguage. Get texts from DB.
            Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Users_List", CurrentLanguage)      'DL 13/01/2012
            Dim literalLevel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_UserLevel", CurrentLanguage)                'DL 13/01/2012
            Dim literalUserID As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_UserID", CurrentLanguage)                  'DL 13/01/2012
            Dim literalFirstName As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirstName", CurrentLanguage)            'DL 13/01/2012
            Dim literalLastName As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LastName", CurrentLanguage)              'DL 13/01/2012
            Dim literalTestsNumber As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Users_TestsNumber", CurrentLanguage)  'DL 13/01/2012
            'DL 13/01/2012. End

            resultData = myUserConfigurationDelegate.GetNotInternalList(Nothing)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim userData As UserDataDS = DirectCast(resultData.SetDatos, UserDataDS)

                'If userData.tcfgUserData.Count > 0 Then
                Dim Rows As IList = (From r In userData.tcfgUserData _
                                     Select r.UserLevelDesc, r.UserName, r.UserFirstName, r.UserLastName, r.MaxTestsNum).ToList()

                Dim detailsReport As New GenericTableReport

                detailsReport.SetDataSource(Rows)
                detailsReport.SetHeaderLabel(literalHeaderLabel)

                detailsReport.AddTableHeaderCells(New XRTableCell() { _
                            CreateTableCell(literalLevel), _
                            CreateTableCell(literalUserID), _
                            CreateTableCell(literalFirstName), _
                            CreateTableCell(literalLastName), _
                            CreateTableCell(literalTestsNumber, TextAlignment.MiddleRight)})

                detailsReport.AddTableRowCells(New XRTableCell() { _
                            CreateBindedTableCell("UserLevelDesc", True), _
                            CreateBindedTableCell("UserName"), _
                            CreateBindedTableCell("UserFirstName"), _
                            CreateBindedTableCell("UserLastName"), _
                            CreateBindedTableCell("MaxTestsNum", False, TextAlignment.MiddleRight)})

                ShowPortrait(detailsReport)
                'End If
                'Else
                'ToDo: Try the error
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowUsersReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    Public Shared Sub ShowTestProfilesReport(Optional ByVal SelectedProfiles As List(Of Integer) = Nothing)
        Try
            Dim resultData As GlobalDataTO
            Dim myTestProfileTestsDelegate As New TestProfileTestsDelegate

            'DL 13/01/2012. Begin
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Multilanguage. Get texts from DB.
            Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_WSTests_Profiles", CurrentLanguage)   'DL 13/01/2012 
            Dim literalName As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", CurrentLanguage)                      'DL 13/01/2012 
            Dim literalSampleType As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", CurrentLanguage)          'DL 13/01/2012 
            Dim literalTestType As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Profiles_TestType", CurrentLanguage)     'DL 13/01/2012 
            Dim literalTestName As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TestName", CurrentLanguage)              'DL 13/01/2012 
            'DL 13/01/2012. End

            resultData = myTestProfileTestsDelegate.GetTestProfilesForReport(Nothing, CurrentLanguage, SelectedProfiles)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim TestProfileData As TestProfileTestsDS = DirectCast(resultData.SetDatos, TestProfileTestsDS)

                'If TestProfileData.tparTestProfileTests.Count > 0 Then
                Dim Rows As IList = (From r In TestProfileData.tparTestProfileTests _
                                     Select r.TestProfileName, r.SampleType, r.TestType, r.TestName).ToList()

                Dim detailsReport As New GenericTableReport

                detailsReport.SetDataSource(Rows)
                detailsReport.SetHeaderLabel(literalHeaderLabel)

                detailsReport.AddTableHeaderCells(New XRTableCell() { _
                            CreateTableCell(literalName), _
                            CreateTableCell(literalSampleType), _
                            CreateTableCell(literalTestType), _
                            CreateTableCell(literalTestName)})

                detailsReport.AddTableRowCells(New XRTableCell() { _
                            CreateBindedTableCell("TestProfileName", True), _
                            CreateBindedTableCell("SampleType", True), _
                            CreateBindedTableCell("TestType", True), _
                            CreateBindedTableCell("TestName")})

                ShowPortrait(detailsReport)
                'End If
                'Else
                'ToDo: Try the error
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowTestProfilesReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    Public Shared Sub ShowPatientsReport(Optional ByVal SelectedPatients As List(Of String) = Nothing)
        Try
            Dim resultData As GlobalDataTO
            Dim myPatientsDelegate As New PatientDelegate

            'DL 13/01/2012. Begin
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Multilanguage. Get texts from DB.
            Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_Summary_PatientsList", CurrentLanguage)    'DL 13/01/2012 
            Const literalID As String = "ID"
            Dim literalFirstName As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_FirstName", CurrentLanguage)                    'DL 13/01/2012
            Dim literalLastName As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LastName", CurrentLanguage)                      'DL 13/01/2012
            Dim literalGender As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Gender", CurrentLanguage)                          'DL 13/01/2012
            Dim literalDateOfBirth As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DateOfBirth", CurrentLanguage)                'DL 13/01/2012
            Dim literalAge As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Age", CurrentLanguage)                                'DL 13/01/2012
            Dim literalRemmarks As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", CurrentLanguage)                      'DL 13/01/2012
            'DL 13/01/2012. End

            resultData = myPatientsDelegate.GetPatientsForReport(Nothing, CurrentLanguage, SelectedPatients)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim PatientsData As PatientsDS = DirectCast(resultData.SetDatos, PatientsDS)

                'If PatientsData.tparPatients.Count > 0 Then
                Dim preloadedMasterConfig As New PreloadedMasterDataDelegate
                Dim AgeUnitsListDS As New PreloadedMasterDataDS

                resultData = preloadedMasterConfig.GetList(Nothing, PreloadedMasterDataEnum.AGE_UNITS)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    AgeUnitsListDS = CType(resultData.SetDatos, PreloadedMasterDataDS)

                    For Each r In PatientsData.tparPatients
                        If Not r.IsDateOfBirthNull Then
                            r.AgeWithUnit = Utilities.GetAgeUnits(r.DateOfBirth, AgeUnitsListDS)
                            r.FormatedDateOfBirth = r.DateOfBirth.ToString(DatePattern)
                        End If
                    Next

                    Dim Rows As IList = (From r In PatientsData.tparPatients _
                                         Select r.PatientID, r.FirstName, r.LastName, r.Gender, r.FormatedDateOfBirth, r.AgeWithUnit, r.Comments).ToList()

                    Dim detailsReport As New GenericTableReport

                    detailsReport.SetDataSource(Rows)
                    detailsReport.SetHeaderLabel(literalHeaderLabel)

                    detailsReport.AddTableHeaderCells(New XRTableCell() { _
                                CreateTableCell(literalID), _
                                CreateTableCell(literalFirstName), _
                                CreateTableCell(literalLastName), _
                                CreateTableCell(literalGender), _
                                CreateTableCell(literalDateOfBirth), _
                                CreateTableCell(literalAge), _
                                CreateTableCell(literalRemmarks, Nothing, 1.7)})

                    detailsReport.AddTableRowCells(New XRTableCell() { _
                                CreateBindedTableCell("PatientID"), _
                                CreateBindedTableCell("FirstName"), _
                                CreateBindedTableCell("LastName"), _
                                CreateBindedTableCell("Gender"), _
                                CreateBindedTableCell("FormatedDateOfBirth"), _
                                CreateBindedTableCell("AgeWithUnit"), _
                                CreateBindedTableCell("Comments", False, Nothing, 1.7)})

                    'ShowLandscape(detailsReport)
                    ShowPortrait(detailsReport)
                End If
                'End If
                'Else
                'ToDo: Try the error
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowPatientsReport", EventLogEntryType.Error, False)

        End Try
    End Sub


    Public Shared Sub ShowISEReport(Optional ByVal SelectedISETest As List(Of String) = Nothing)
        Try
            Dim resultData As GlobalDataTO


            'DL 27/01/2012. Begin
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Multilanguage. Get texts from DB.
            Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_ISETests_Definition", CurrentLanguage)     'DL 13/01/2012 
            Const literalID As String = "ID"
            Dim literalFirstName As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", CurrentLanguage)                         'DL 13/01/2012
            Dim literalLastName As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_LastName", CurrentLanguage)                      'DL 13/01/2012
            Dim literalGender As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Gender", CurrentLanguage)                          'DL 13/01/2012
            Dim literalDateOfBirth As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_DateOfBirth", CurrentLanguage)                'DL 13/01/2012
            Dim literalAge As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Age", CurrentLanguage)                                'DL 13/01/2012
            Dim literalRemmarks As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Remarks", CurrentLanguage)                       'DL 13/01/2012
            'DL 27/06/2012. End

            'Dim myPatientsDelegate As New PatientDelegate
            'resultData = myPatientsDelegate.GetPatientsForReport(Nothing, CurrentLanguage, SelectedISETest)

            Dim ISETest As New ISETestsDelegate
            resultData = ISETest.GetISEForReport(Nothing, CurrentLanguage, SelectedISETest)


            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ISEData As ISETestSamplesDS = DirectCast(resultData.SetDatos, ISETestSamplesDS)

                'If PatientsData.tparPatients.Count > 0 Then
                Dim preloadedMasterConfig As New PreloadedMasterDataDelegate
                Dim AgeUnitsListDS As New PreloadedMasterDataDS

                resultData = preloadedMasterConfig.GetList(Nothing, PreloadedMasterDataEnum.AGE_UNITS)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    AgeUnitsListDS = CType(resultData.SetDatos, PreloadedMasterDataDS)

                    For Each r In ISEData.tparISETestSamples
                        'If Not r.IsDateOfBirthNull Then
                        ' r.AgeWithUnit = Utilities.GetAgeUnits(r.DateOfBirth, AgeUnitsListDS)
                        ' r.FormatedDateOfBirth = r.DateOfBirth.ToString(DatePattern)
                        'End If
                    Next

                    Dim Rows As IList = (From r In ISEData.tparISETestSamples _
                                         Select r).ToList() 'r.PatientID, r.FirstName, r.LastName, r.Gender, r.FormatedDateOfBirth, r.AgeWithUnit, r.Comments).ToList()

                    Dim detailsReport As New GenericTableReport

                    detailsReport.SetDataSource(Rows)
                    detailsReport.SetHeaderLabel(literalHeaderLabel)

                    detailsReport.AddTableHeaderCells(New XRTableCell() { _
                                CreateTableCell(literalID), _
                                CreateTableCell(literalFirstName), _
                                CreateTableCell(literalLastName), _
                                CreateTableCell(literalGender), _
                                CreateTableCell(literalDateOfBirth), _
                                CreateTableCell(literalAge), _
                                CreateTableCell(literalRemmarks, Nothing, 1.7)})

                    detailsReport.AddTableRowCells(New XRTableCell() { _
                                CreateBindedTableCell("PatientID"), _
                                CreateBindedTableCell("FirstName"), _
                                CreateBindedTableCell("LastName"), _
                                CreateBindedTableCell("Gender"), _
                                CreateBindedTableCell("FormatedDateOfBirth"), _
                                CreateBindedTableCell("AgeWithUnit"), _
                                CreateBindedTableCell("Comments", False, Nothing, 1.7)})

                    'ShowLandscape(detailsReport)
                    ShowPortrait(detailsReport)
                End If
                'End If
                'Else
                'ToDo: Try the error
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowISEReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    Public Shared Sub ShowCalibratorsReport(Optional ByVal SelectedCalibrators As List(Of Integer) = Nothing)
        Try
            Dim resultData As GlobalDataTO
            Dim myCalibratorsDelegate As New CalibratorsDelegate

            'DL 13/01/2012. Begin
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            'Multilanguage. Get texts from DB.
            resultData = myCalibratorsDelegate.GetCalibratorsForReport(Nothing, CurrentLanguage, SelectedCalibrators)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim CalibratorsData As CalibratorsDS = DirectCast(resultData.SetDatos, CalibratorsDS)

                'If (CalibratorsData.tparCalibrators.Count > 0) AndAlso (CalibratorsData.tparCalibratorsTests.Count > 0) Then
                'Set Concentration with decimals
                For Each row As CalibratorsDS.tparCalibratorsTestsRow In CalibratorsData.tparCalibratorsTests
                    row.ConcentrationWithDecimals = row.TheoricalConcentration.ToStringWithDecimals(row.DecimalsAllowed)
                Next

                Dim Report As New CalibratorsReport

                'Multilanguage. Get texts from DB.
                Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrator_Prog", CurrentLanguage)    'DL 13/01/2012
                Report.SetHeaderLabel(literalHeaderLabel)
                '
                Report.XrLabelName.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", CurrentLanguage)                   'DL 13/01/2012
                Report.XrLabelLotNumber.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Lot", CurrentLanguage)                    'DL 13/01/2012
                Report.XrLabelCalibratorsNum.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", CurrentLanguage)          'DL 13/01/2012
                Report.XrLabelExpirationDate.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ExpDate_Short", CurrentLanguage)      'DL 13/01/2012
                '
                Report.XrLabelTest.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", CurrentLanguage)            'DL 13/01/2012
                Report.XrLabelSampleType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleType", CurrentLanguage)    'DL 13/01/2012
                Report.XrLabelCurveType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveType", CurrentLanguage)           'DL 13/01/2012
                'Report.XrLabelCurveGrowth.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveGrowth", CurrentLanguage)           'DL 13/01/2012 
                Report.XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", CurrentLanguage)
                Report.XrLabelXAxis.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AxisX", CurrentLanguage)                      'DL 13/01/2012 
                Report.XrLabelYAxis.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AxisY", CurrentLanguage)                      'DL 13/01/2012
                Report.XrLabelNum.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", CurrentLanguage)                 'DL 13/01/2012
                Report.XrLabelConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", CurrentLanguage)        'DL 13/01/2012

                Report.XrLabelExpirationDateValue.DataBindings(0).FormatString = "{0:" & DatePattern & "}"

                'JC 12/11/2012
                Report.XrLabelUnit.WidthF = 150

                Report.DataSource = CalibratorsData
                ShowPortrait(Report)
                'End If
                'Else
                'ToDo: Try the error
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowCalibratorsReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    Public Shared Sub ShowContaminationsReport()
        Try
            Dim resultData As GlobalDataTO
            Dim myContaminationsDelegate As New ContaminationsDelegate

            'DL 14/01/2012. Begin
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            resultData = myContaminationsDelegate.GetContaminationsForReport(Nothing, CurrentLanguage)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ContaminationsData As ContaminationsDS = DirectCast(resultData.SetDatos, ContaminationsDS)

                Dim Report As New ContaminationsReport

                'Multilanguage. Get texts from DB.
                Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminations", CurrentLanguage)                     'DL 14/01/2012

                Report.SetHeaderLabel(literalHeaderLabel)

                Report.ReagentsContaminationsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Reagents_Contaminations", CurrentLanguage)     'DL 14/01/2012
                Report.ReagentContaminatorLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminator", CurrentLanguage)                   'DL 14/01/2012
                Report.ReagentContaminatedLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminated", CurrentLanguage)                   'DL 14/01/2012
                Report.WashingSolutionLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SRV_WashSolution", CurrentLanguage)                   'DL 14/01/2012

                Report.CuvettesContaminationsLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Cuvette_Contaminations", CurrentLanguage)        'DL 14/01/2012
                Report.CuvetteContaminatorLabel.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Contaminator", CurrentLanguage)                   'DL 14/01/2012
                Report.Step1Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Step_1", CurrentLanguage)                                       'DL 14/01/2012
                Report.Step2Label.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Step_2", CurrentLanguage)                                       'DL 14/01/2012

                Report.DataSource = ContaminationsData
                ShowPortrait(Report)
                'Else
                'ToDo: Try the error
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowContaminationsReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    Public Shared Sub ShowResultsByPatientSampleReport(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String)
        Try
            Dim myLogAcciones As New ApplicationLogManager()
            Dim StartTime As DateTime = Now 'AG 13/06/2012 - time estimation

            Dim myGlobalDataTO As GlobalDataTO
            Dim myResultsDelegate As New ResultsDelegate

            'DL 14/01/2012. Begin
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            myGlobalDataTO = myResultsDelegate.GetResultsByPatientSampleForReport(Nothing, pAnalyzerID, pWorkSessionID)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim ResultsData As ResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)

                'TR 10/07/2012 -Validate if there are results to open report screen 
                If ResultsData.ReportSampleMaster.Count > 0 Then
                    Dim Report As New ResultsByPatientSampleReport

                    'Multilanguage. Get texts from DB.
                    Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurrentResults_Patient", CurrentLanguage)     'DL 14/01/2012 '"Current Results by Patient Sample"
                    Report.SetHeaderLabel(literalHeaderLabel)

                    Report.XrLabelPatientID.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", CurrentLanguage)  'EF 04/05/2014 (etiqueta)
                    Report.XrLabelTest.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", CurrentLanguage)        'DL 14/01/2012
                    Report.XrLabelType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", CurrentLanguage)            'DL 14/01/2012
                    Report.XrLabelNumber.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", CurrentLanguage)  'DL 14/01/2012 
                    Report.XrLabelAbs.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", CurrentLanguage)     'DL 14/01/2012 "Abs."
                    Report.XrLabelConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", CurrentLanguage)     'DL 14/01/2012 "Conc."
                    Report.XrLabelRefranges.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Short", CurrentLanguage)     'DL 14/01/2012
                    Report.XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", CurrentLanguage)     'DL 14/01/2012
                    Report.XrLabelDate.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", CurrentLanguage)     'DL 14/01/2012

                    Dim WSStartDateTime As String = String.Empty

                    'Get WSStartDateTime from DB
                    Dim myWSDelegate As New WorkSessionsDelegate

                    myGlobalDataTO = myWSDelegate.GetByWorkSession(Nothing, pWorkSessionID)

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        Dim myWSDataDS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)
                        If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                            WSStartDateTime = myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern) & " " & _
                                                myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern)
                        End If
                    End If

                    Report.XrWSStartDateTimeLabel.Text = WSStartDateTime

                    Report.DataSource = ResultsData
                    ShowPortrait(Report)
                End If

                'Else
                'ToDo: Try the error
            End If

            myLogAcciones.CreateLogActivity("Patients Report (current results): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "XRManager.ShowResultsByPatientSampleReport", EventLogEntryType.Information, False) 'AG 04/07/2012

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowResultsByPatientSampleReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    ''' <summary>
    ''' Show preview historic results by patient sample
    ''' </summary>
    ''' <param name="pHisWSResults">The initial date to the results calculations</param>
    ''' <remarks>
    ''' Created by: DL 23/10/2012
    ''' </remarks>
    Public Shared Sub ShowHistoricResultsByPatientSampleReport(ByVal pHisWSResults As List(Of HisWSResultsDS.vhisWSResultsRow))

        Try
            Dim myLogAcciones As New ApplicationLogManager()
            Dim StartTime As DateTime = Now
            Dim myGlobalDataTO As GlobalDataTO
            Dim myHisWSResultsDelegate As New HisWSResultsDelegate
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
            'ShowHistoricByCompactPatientsSamplesResult(pHisWSResults)
            myGlobalDataTO = myHisWSResultsDelegate.GetHistoricResultsByPatientSampleForReport(Nothing, pHisWSResults)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim ResultsData As ResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)

                If ResultsData.ReportSampleMaster.Count > 0 Then
                    Dim Report As New HistoricResultsByPatientSampleReport

                    'Multilanguage. Get texts from DB.
                    Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurrentResults_Patient", CurrentLanguage)     'DL 14/01/2012 '"Current Results by Patient Sample" "*Historic Patient Results" '
                    Report.SetHeaderLabel(literalHeaderLabel)

                    Report.XrLabelTest.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", CurrentLanguage)
                    Report.XrLabelType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", CurrentLanguage)
                    Report.XrLabelFlags.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Flags", CurrentLanguage) 'EF 03/06/2014 (cambio texto por LBL_Flags)
                    Report.XrLabelConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", CurrentLanguage)
                    Report.XrLabelRefranges.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Short", CurrentLanguage)
                    Report.XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", CurrentLanguage)

                    'Get WSStartDateTime from DB
                    'Dim WSStartDateTime As String = String.Empty
                    'Dim myWSDelegate As New WorkSessionsDelegate

                    'myGlobalDataTO = myWSDelegate.GetByWorkSession(Nothing, pWorkSessionID)

                    'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    '    Dim myWSDataDS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)
                    '    If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                    '        WSStartDateTime = myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern) & " " & _
                    '                            myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern)
                    '    End If
                    'End If

                    ' EF 03/06/2014 En el informe de Resultados Historicos de Paciente puede haber resultados de varias sesiones (lo ideal es mostrar el rango de fechas OlderWSDate - NewerWSDate) 
                    ' Report.XrWSStartDateTimeLabel.Text = Now.ToShortDateString & " " & Now.ToShortTimeString  '"01/10/2012" 'WSStartDateTime

                    Report.DataSource = ResultsData
                    ShowPortrait(Report)
                End If

                'Else
                'ToDo: Try the error
            End If

            myLogAcciones.CreateLogActivity("Patients Report (historic results): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "XRManager.ShowHistoricResultsByPatientSampleReport", EventLogEntryType.Information, False) 'AG 04/07/2012

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowHistoricResultsByPatientSampleReport", EventLogEntryType.Error, False)

        End Try
    End Sub
    ''' <summary>
    ''' Saca el histórico del informe compacto de resultados de paciente. 
    ''' </summary>
    ''' <param name="pHisWSResults">Lista de resultados a imprimir en el informe. </param>
    ''' <remarks>
    ''' AG 07/10/2013 - Modify: For the compact report do not fill the header "Remarks", leave ""
    ''' </remarks>
    Public Shared Sub ShowHistoricByCompactPatientsSamplesResult(ByVal pHisWSResults As List(Of HisWSResultsDS.vhisWSResultsRow))
        Try
            Dim myLogAcciones As New ApplicationLogManager()
            Dim StartTime As DateTime = Now
            Dim myGlobalDataTO As GlobalDataTO
            Dim myHisWSResultsDelegate As New HisWSResultsDelegate
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            myGlobalDataTO = myHisWSResultsDelegate.GetHistoricResultsByPatientSampleForReport(Nothing, pHisWSResults)

            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim ResultsData As ResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)

                If ResultsData.ReportSampleMaster.Count > 0 Then
                    'Dim Report As New AutomaticPatientsReport
                    Using automaticReport As New AutomaticPatientsReport()
                        Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurrentResults_Patient", CurrentLanguage)
                        'TODO: Get the correct resource for the new report text. 
                        automaticReport.SetHeaderLabel(literalHeaderLabel)
                        automaticReport.XrLabelPatientID.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", CurrentLanguage)
                        automaticReport.XrLabelTest.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", CurrentLanguage)
                        automaticReport.XrLabelType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", CurrentLanguage)
                        automaticReport.XrLabelConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", CurrentLanguage)
                        automaticReport.XrLabelRefranges.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Short", CurrentLanguage)
                        automaticReport.XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", CurrentLanguage)
                        'ag 07/10/2013
                        'automaticReport.XrLabelRemarks.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Flags", CurrentLanguage)  'EF 03/06/2014 (cargar texto LBL_FLags)
                        automaticReport.XrLabelRemarks.Text = ""
                        'ag 07/10/2013
                        automaticReport.XrLabelDate.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", CurrentLanguage)

                        'Dim WSStartDateTime As String = String.Empty
                        'Dim myWSDelegate As New WorkSessionsDelegate()
                        'If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO.SetDatos IsNot Nothing Then
                        '    Dim myWSDataDS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)
                        '    If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                        '        WSStartDateTime = String.Format("{0} {1}", _
                        '                              myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern), _
                        '                              myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern))
                        '    End If
                        'End If
                        automaticReport.XrWSStartDateTimeLabel.Text = Now.ToShortDateString & " " & Now.ToShortTimeString
                        automaticReport.DataSource = Nothing
                        automaticReport.DataMember = ""

                        automaticReport.DataSource = ResultsData
                        automaticReport.DataMember = "ReportSampleDetails"
                        ShowPortrait(automaticReport)
                    End Using
                End If

                'Else
                'ToDo: Try the error
            End If

            myLogAcciones.CreateLogActivity("Patients Compact Report (historic results): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "XRManager.ShowHistoricByCompactPatientsSamplesResult", EventLogEntryType.Information, False) 'AG 04/07/2012

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowHistoricByCompactPatientsSamplesResult", EventLogEntryType.Error, False)

        End Try
    End Sub


    Public Shared Sub ShowPatientsFinalReport(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String)
        Try
            Dim resultData As GlobalDataTO
            Dim myResultsDelegate As New ResultsDelegate

            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            resultData = myResultsDelegate.GetResultsByPatientSampleForReport(Nothing, pAnalyzerID, pWorkSessionID, False)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ResultsData As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                'TR 10/07/2012 -Validate if there are results to open report screen 
                If ResultsData.ReportSampleMaster.Count > 0 Then
                    Dim Report As New PatientsFinalReport

                    'Multilanguage. Get texts from DB.
                    Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurrentResults_Patient", CurrentLanguage)
                    Report.SetHeaderLabel(literalHeaderLabel)

                    Report.XrLabelTest.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Test", CurrentLanguage)
                    Report.XrLabelType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", CurrentLanguage)
                    Report.XrLabelConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", CurrentLanguage)
                    Report.XrLabelRefranges.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_ReferenceRanges_Short", CurrentLanguage)
                    Report.XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", CurrentLanguage)
                    Report.XrLabelFlags.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Flags", CurrentLanguage)  'EF 03/06/2014 (cargar texto LBL_FLags)

                    Dim WSStartDateTime As String = String.Empty

                    'Get WSStartDateTime from DB
                    Dim myWSDelegate As New WorkSessionsDelegate

                    resultData = myWSDelegate.GetByWorkSession(Nothing, pWorkSessionID)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                        If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                            WSStartDateTime = myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern) & " " & _
                                                myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern)
                        End If
                    End If

                    Report.XrWSStartDateTimeLabel.Text = WSStartDateTime

                    Report.DataSource = ResultsData
                    ShowPortrait(Report)
                End If

                'Else
                'ToDo: Try the error
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowPatientsFinalReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    Public Shared Sub ShowResultsByTestReport(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String)
        Try
            Dim myLogAcciones As New ApplicationLogManager()
            Dim StartTime As DateTime = Now 'AG 13/06/2012 - time estimation

            Dim resultData As GlobalDataTO
            Dim myResultsDelegate As New ResultsDelegate

            'DL 14/01/2012. Begin
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            resultData = myResultsDelegate.GetResultsByTestForReport(Nothing, pAnalyzerID, pWorkSessionID)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ResultsData As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                If ResultsData.ReportTestMaster.Count > 0 Then
                    Dim Report As New ResultsByTestReport

                    'Multilanguage. Get texts from DB.
                    Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurrentResults_Test", CurrentLanguage)
                    Report.SetHeaderLabel(literalHeaderLabel)

                    Report.XrLabelName.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", CurrentLanguage)        'DL 14/01/2012"Name"
                    'Report.XrLabelPatientID.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", CurrentLanguage)
                    Report.XrLabelType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", CurrentLanguage)            'DL 14/01/2012
                    Report.XrLabelNumber.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", CurrentLanguage)  'DL 14/01/2012 
                    Report.XrLabelAbs.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", CurrentLanguage)     'DL 14/01/2012 
                    Report.XrLabelConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", CurrentLanguage)
                    Report.XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", CurrentLanguage)
                    Report.XrLabelClass.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleClass_Short", CurrentLanguage)     'DL 14/01/2012
                    Report.XrLabelFactor.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibFactor", CurrentLanguage)     'DL 14/01/2012
                    Report.XrLabelDate.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", CurrentLanguage)     'DL 14/01/2012

                    Dim WSStartDateTime As String = String.Empty

                    'Get WSStartDateTime from DB
                    Dim myWSDelegate As New WorkSessionsDelegate

                    resultData = myWSDelegate.GetByWorkSession(Nothing, pWorkSessionID)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                        If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                            WSStartDateTime = myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern) & " " & _
                                                myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern)
                        End If
                    End If

                    Report.XrWSStartDateTimeLabel.Text = WSStartDateTime

                    Report.DataSource = ResultsData
                    ShowPortrait(Report)
                End If

                'Else
                'ToDo: Try the error
            End If

            myLogAcciones.CreateLogActivity("Tests Report (current results): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "XRManager.ShowResultsByTestReport", EventLogEntryType.Information, False) 'AG 04/07/2012

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowResultsByTestReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pAnalyzerID"></param>
    ''' <param name="pWorkSessionID"></param>
    ''' <param name="pType"></param>
    ''' <remarks>
    ''' Created by:  JV #1502 20/02/2014 - new sub for new report creation
    ''' Updated by:  JV #1502 21/02/2014 - multilanguage header for ctrl and blank-calib reports
    ''' </remarks>
    Public Shared Sub ShowResultsByTestReportCompactBySampleType(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pType As String)
        Try
            Dim myLogAcciones As New ApplicationLogManager()
            Dim StartTime As DateTime = Now 'AG 13/06/2012 - time estimation

            Dim resultData As GlobalDataTO
            Dim myResultsDelegate As New ResultsDelegate

            'DL 14/01/2012. Begin
            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            resultData = myResultsDelegate.GetResultsByTestForReportCompactBySampleType(Nothing, pAnalyzerID, pWorkSessionID, pType)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ResultsData As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                If ResultsData.ReportTestMaster.Count > 0 Then
                    Dim Report As New ResultsByTestReportCompact

                    'Multilanguage. Get texts from DB.
                    Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurrentResults_Test", CurrentLanguage)
                    Select Case pType
                        Case "CTRL"
                            Report.SetHeaderLabel(literalHeaderLabel & " - " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CONTROLS", CurrentLanguage))
                        Case "BLANK"
                            Report.SetHeaderLabel(literalHeaderLabel & " - " & myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Blanks", CurrentLanguage) & ", " & _
                                                  myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Calibrators", CurrentLanguage))
                    End Select


                    Report.XrLabelName.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Name", CurrentLanguage)        'DL 14/01/2012"Name"
                    'Report.XrLabelPatientID.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_PatientID", CurrentLanguage)
                    Report.XrLabelType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Type", CurrentLanguage)            'DL 14/01/2012
                    Report.XrLabelNumber.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Number_Short", CurrentLanguage)  'DL 14/01/2012 
                    Report.XrLabelAbs.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Short", CurrentLanguage)     'DL 14/01/2012 
                    Report.XrLabelConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_Conc_Short", CurrentLanguage)
                    Report.XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", CurrentLanguage)
                    Report.XrLabelClass.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_SampleClass_Short", CurrentLanguage)     'DL 14/01/2012
                    Report.XrLabelFactor.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalibFactor", CurrentLanguage)     'DL 14/01/2012
                    Report.XrLabelDate.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Date", CurrentLanguage)     'DL 14/01/2012

                    Dim WSStartDateTime As String = String.Empty

                    'Get WSStartDateTime from DB
                    Dim myWSDelegate As New WorkSessionsDelegate

                    resultData = myWSDelegate.GetByWorkSession(Nothing, pWorkSessionID)

                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                        Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                        If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                            WSStartDateTime = myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern) & " " & _
                                                myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern)
                        End If
                    End If

                    Report.XrWSStartDateTimeLabel.Text = WSStartDateTime

                    Report.DataSource = ResultsData
                    ShowPortrait(Report)
                End If

                'Else
                'ToDo: Try the error
            End If

            myLogAcciones.CreateLogActivity("Tests Report Compact(current results): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), "XRManager.ShowResultsByTestReportCompactBySampleType", EventLogEntryType.Information, False) 'AG 04/07/2012

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowResultsByTestReportCompactBySampleType", EventLogEntryType.Error, False)

        End Try
    End Sub

    Public Shared Sub ShowSummaryResultsReport(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal Vertical As Boolean)
        Try
            Dim resultData As GlobalDataTO
            Dim myResultsDelegate As New ResultsDelegate

            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            resultData = myResultsDelegate.GetSummaryResultsByPatientSampleForReport(Nothing, pAnalyzerID, pWorkSessionID)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim dt As DataTable = DirectCast(resultData.SetDatos, DataTable)

                Dim Report As New SummaryResultsReport

                Report.SetDataSource(dt)

                'Multilanguage. Get texts from DB.
                Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "BTN_Results_OpenSummary", CurrentLanguage)

                Report.SetHeaderLabel(literalHeaderLabel)

                Dim LabelFont As System.Drawing.Font = New System.Drawing.Font("Verdana", 6.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                Dim CellFont As System.Drawing.Font = New System.Drawing.Font("Verdana", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))

                Dim cells(dt.Columns.Count - 1) As XRTableCell

                cells(0) = CreateTableCell(myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_PATIENT", CurrentLanguage))
                cells(0).Font = LabelFont

                For i As Integer = 1 To dt.Columns.Count - 1
                    cells(i) = CreateTableCell(dt.Columns(i).ColumnName)
                    cells(i).Font = LabelFont
                Next

                Report.AddTableHeaderCells(cells)

                cells(0) = CreateBindedTableCell(dt.Columns(0).ColumnName)
                cells(0).Font = CellFont

                For i As Integer = 1 To dt.Columns.Count - 1
                    cells(i) = CreateBindedTableCell(dt.Columns(i).ColumnName, False, TextAlignment.MiddleRight)
                    cells(i).Font = CellFont
                Next

                Report.AddTableRowCells(cells)

                Dim WSStartDateTime As String = String.Empty

                'Get WSStartDateTime from DB
                Dim myWSDelegate As New WorkSessionsDelegate

                resultData = myWSDelegate.GetByWorkSession(Nothing, pWorkSessionID)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                    If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                        WSStartDateTime = myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern) & " " & _
                                            myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern)
                    End If
                End If

                Report.XrWSStartDateTimeLabel.Text = WSStartDateTime

                If Vertical Then
                    ShowPortrait(Report)
                Else
                    ShowLandscape(Report)
                End If
                'Else
                'ToDo: Try the error
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowUsersReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    ''' <summary>
    ''' Calibration curve report
    ''' </summary>
    ''' <param name="pAnalyzerID"></param>
    ''' <param name="pWorkSessionID"></param>
    ''' <param name="TestName"></param>
    ''' <param name="AcceptedRerunNumber"></param>
    ''' <param name="pTestReportName">Test name for report (optional)</param>
    ''' <remarks> Creation ??
    ''' Modified AG - 30/04/2014 - #1608 add optional parameter for the test report name
    ''' </remarks>
    Public Shared Sub ShowResultsCalibCurveReport(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                                  ByVal TestName As String, ByVal AcceptedRerunNumber As Integer, Optional ByVal pTestReportName As String = "")
        Try
            Dim resultData As GlobalDataTO
            Dim myResultsDelegate As New ResultsDelegate

            Dim currentLanguageGlobal As New GlobalBase
            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate

            resultData = myResultsDelegate.GetResultsCalibCurveForReport(Nothing, pAnalyzerID, pWorkSessionID, TestName, AcceptedRerunNumber)

            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                Dim ResultsData As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

                If ResultsData.ReportCalibCurve.Rows.Count = 0 Then Return

                Dim Report As New ResultsCalibCurveReport

                'Multilanguage. Get texts from DB.
                Dim literalHeaderLabel As String = myMultiLangResourcesDelegate.GetResourceText(Nothing, "TITLE_CurveResultsScreen", CurrentLanguage)
                Report.SetHeaderLabel(literalHeaderLabel)

                'AG 30/04/2014 - #1608
                'Report.XrLabelTestName.Text = TestName
                Dim myTestReportName As String = pTestReportName
                If myTestReportName = "" Then myTestReportName = TestName
                Report.XrLabelTestName.Text = myTestReportName
                'AG 30/04/2014 - #1608

                Report.XrLabelXAxis.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AxisX", CurrentLanguage)
                Report.XrLabelYAxis.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_AxisY", CurrentLanguage)
                Report.XrLabelCurveType.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveType", CurrentLanguage)
                Report.XrLabelCurveGrowth.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveGrowth", CurrentLanguage)
                Report.XrLabelUnit.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Unit", CurrentLanguage)

                Report.XrLabelCalibNo.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveCalibratorNumber", CurrentLanguage)
                Report.XrLabelAbs.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", CurrentLanguage)
                Report.XrLabelTheorConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_TheoricalConc_Short", CurrentLanguage)
                Report.XrLabelCalcConc.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CalcConc", CurrentLanguage)
                Report.XrLabelError.Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_CurveRes_%Error", CurrentLanguage)

                Dim WSStartDateTime As String = String.Empty

                'Get WSStartDateTime from DB
                Dim myWSDelegate As New WorkSessionsDelegate

                resultData = myWSDelegate.GetByWorkSession(Nothing, pWorkSessionID)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
                    If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsStartDateTimeNull) Then
                        WSStartDateTime = myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern) & " " & _
                                            myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern)
                    End If
                End If

                Report.XrWSStartDateTimeLabel.Text = WSStartDateTime

                Dim CurveResultsID As Integer = 0
                'Dim ChartSampleType As String = String.Empty
                Dim CurveGrowthType As String = String.Empty
                Dim CurveType As String = String.Empty
                Dim CurveAxisXType As String = String.Empty
                Dim CurveAxisYType As String = String.Empty
                Dim MonotonousCurve As Boolean = True

                With ResultsData.ReportCalibCurve(0)
                    CurveResultsID = .CurveResultsID
                    'ChartSampleType = .SampleType
                    CurveGrowthType = .CurveGrowthType
                    CurveType = .CurveType
                    CurveAxisXType = .CurveAxisXType
                    CurveAxisYType = .CurveAxisYType
                    MonotonousCurve = String.IsNullOrEmpty(.CalibrationError)

                    Report.XrLabelXAxisField.Text = _
                            myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_CURVE_AXIS_TYPES_" & .CurveAxisXType, CurrentLanguage)

                    Report.XrLabelYAxisField.Text = _
                            myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_CURVE_AXIS_TYPES_" & .CurveAxisYType, CurrentLanguage)

                    Report.XrLabelCurveTypeField.Text = _
                            myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_CURVE_TYPES_" & .CurveType, CurrentLanguage)

                    Report.XrLabelUnitField.Text = .MeasureUnit

                    If String.Compare(.CurveGrowthType, "DEC", False) = 0 Then
                        Report.XrLabelCurveGrowthField.Text = _
                                myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Decreasing", CurrentLanguage)
                    Else
                        Report.XrLabelCurveGrowthField.Text = _
                                myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Increasing", CurrentLanguage)
                    End If
                End With

                Report.XrChart1.Titles(0).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Absorbance_Full", CurrentLanguage)
                Report.XrChart1.Titles(1).Text = myMultiLangResourcesDelegate.GetResourceText(Nothing, "LBL_Concentration_Long", CurrentLanguage)

                Dim myCurveResultsDelegate As New CurveResultsDelegate
                resultData = myCurveResultsDelegate.GetResults(Nothing, CurveResultsID)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    If (MonotonousCurve) Then
                        Dim curveDS As CurveResultsDS
                        curveDS = DirectCast(resultData.SetDatos, CurveResultsDS)

                        'Plot curve points
                        For i As Integer = 0 To curveDS.twksCurveResults.Rows.Count - 1
                            With curveDS.twksCurveResults(i)
                                Report.XrChart1.Series(0).Points.Add(New DevExpress.XtraCharts.SeriesPoint(.CONCValue, .ABSValue))
                            End With
                        Next i

                        Dim PointAbsorbance As Single
                        Dim PointConcentration As Single

                        PointAbsorbance = ResultsData.ReportCalibCurve(0).CalibratorBlankAbsUsed
                        PointConcentration = 0

                        'Plot CalibratorBlankAbsUsed
                        Report.XrChart1.Series(1).Points.Add(New DevExpress.XtraCharts.SeriesPoint(PointConcentration, PointAbsorbance))
                        Report.XrChart1.Series(1).Points(0).Tag = _
                                myMultiLangResourcesDelegate.GetResourceText(Nothing, "PMD_SAMPLE_CLASSES_BLANK", CurrentLanguage)

                        'Plot Result Points
                        For i As Integer = 0 To ResultsData.ReportCalibCurve.Rows.Count - 1
                            PointAbsorbance = Single.Parse(ResultsData.ReportCalibCurve(i).ABSValue)
                            PointConcentration = Single.Parse(ResultsData.ReportCalibCurve(i).TheoricalConcentration)

                            Report.XrChart1.Series(1).Points.Add(New DevExpress.XtraCharts.SeriesPoint(PointConcentration, PointAbsorbance))
                            Report.XrChart1.Series(1).Points(i + 1).Tag = (i + 1).ToString()
                        Next i

                        If curveDS.twksCurveResults.Rows.Count > 0 Then

                            If String.Compare(CurveType, "LINEAR", False) = 0 Then

                                Dim AverageList As List(Of ResultsDS.ReportCalibCurveRow) = _
                                                    (From row In ResultsData.ReportCalibCurve _
                                                     Where row.CurveOffset <> Single.MinValue _
                                                     AndAlso row.CurveSlope <> Single.MinValue _
                                                     AndAlso row.CurveCorrelation <> Single.MinValue _
                                                     Order By row.CurveSlope Descending _
                                                     Select row).ToList()

                                'Where row.SampleType = ChartSampleType _
                                'AndAlso row.CurveType = "LINEAR" _


                                If AverageList.Count > 0 Then
                                    Dim CurveSlope As String
                                    Dim CurveOffset As String
                                    Dim r As String
                                    Dim r2 As String

                                    CurveSlope = String.Format("Abs = {0}*Conc", ResultsData.ReportCalibCurve(0).CurveSlope.ToStringWithDecimals(4))

                                    If ResultsData.ReportCalibCurve(0).CurveOffset < 0 Then
                                        CurveOffset = " - " & ((-1) * ResultsData.ReportCalibCurve(0).CurveOffset).ToStringWithDecimals(4)
                                    Else
                                        CurveOffset = " + " & ResultsData.ReportCalibCurve(0).CurveOffset.ToStringWithDecimals(4)
                                    End If

                                    r = ResultsData.ReportCalibCurve(0).CurveCorrelation.ToStringWithDecimals(4)
                                    r2 = Math.Pow(ResultsData.ReportCalibCurve(0).CurveCorrelation, 2).ToStringWithDecimals(4)

                                    Report.XrChart1.Titles(2).Text = String.Format("{0}{1}   r = {2}   r² = {3}", CurveSlope, CurveOffset, r, r2)
                                    Report.XrChart1.Titles(2).Visible = True
                                End If
                            End If

                        Else
                            'CType(Report.XrChart1.Diagram, XYDiagram).DefaultPane.Visible = False
                            Report.XrLabelNoData.Visible = True
                            Report.XrLabelNoData.Text = _
                                    myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_CALIBCURVE_NOT_CALCULATED", CurrentLanguage)
                        End If

                    Else
                        'CType(Report.XrChart1.Diagram, XYDiagram).DefaultPane.Visible = False
                        Report.XrLabelNoData.Visible = True
                        Report.XrLabelNoData.Text = _
                                myMultiLangResourcesDelegate.GetResourceText(Nothing, "MSG_CALIBCURVE_NOT_CALCULATED", CurrentLanguage)
                    End If
                    'Else
                    'ToDo: Try the error
                End If

                Report.DataSource = ResultsData
                ShowPortrait(Report)
            End If

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowResultsByTestReport", EventLogEntryType.Error, False)

        End Try
    End Sub


    ''' <summary>
    ''' Shows the Quality Control-Individual Results by Test/Sample Type Report
    ''' </summary>
    ''' <param name="pQCTestSampleID">Id of the TestSample to get the Header data</param>
    ''' <param name="pDateFrom">The initial date to the results calculations</param>
    ''' <param name="pDateTo">The final date to the results calculations</param>
    ''' <param name="pOpenQCResults">The Controls Data</param>
    ''' <param name="pFilterQCResults">The processed Results</param>
    ''' <param name="pLocalDecimalAllow">The number of decimals to show in the report</param>
    ''' <param name="pGraphType">The type of graph to show</param>
    ''' <remarks>
    ''' The Report doesn't re-calculates all the results
    ''' The Controls data and processed results are the same used in GUI, the report filters and processes to show it
    ''' 
    ''' Created by:  JB 13/07/2012
    ''' </remarks>
    Public Shared Sub ShowQCIndividualResultsByTestReport(ByVal pQCTestSampleID As Integer, ByVal pDateFrom As Date, ByVal pDateTo As Date, _
                                                          ByVal pOpenQCResults As OpenQCResultsDS, ByVal pFilterQCResults As QCResultsDS, _
                                                          ByVal pLocalDecimalAllow As Integer, _
                                                          ByVal pGraphType As REPORT_QC_GRAPH_TYPE)
        Try
            Dim report As New QCIndividualResultsByTestReport()

            Dim testSampleRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow = Nothing

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myHistoryTestSamplesDelegate As New HistoryTestSamplesDelegate

            'Get the information from the History Tests/Sample Types table
            myGlobalDataTO = myHistoryTestSamplesDelegate.Read(Nothing, pQCTestSampleID)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myHistoryTestSampleDS As HistoryTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
                If (myHistoryTestSampleDS.tqcHistoryTestSamples.Count > 0) Then
                    testSampleRow = myHistoryTestSampleDS.tqcHistoryTestSamples(0)
                End If
            End If

            report.SetControlsAndResultsDatasource(testSampleRow, pOpenQCResults, pFilterQCResults, pLocalDecimalAllow, pDateFrom.ToString(DatePattern) & " - " & pDateTo.ToString(DatePattern), pGraphType)

            ShowPortrait(report)


        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowQCIndividualResultsByTestReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    ''' <summary>
    ''' Shows the Quality Control-Accumulated Results by Test/Sample Type Report
    ''' </summary>
    ''' <param name="pQCTestSampleID">Id of the TestSample to get the Header data</param>
    ''' <param name="pDateFrom">The initial date to the results calculations</param>
    ''' <param name="pDateTo">The final date to the results calculations</param>
    ''' <param name="pQCCumulatedSummaryDS">The Controls DataSet</param>
    ''' <param name="pQCCummulatedResultsDS">The processed Results DataSet</param>
    ''' <param name="pLocalDecimalAllow">The number of decimals to show in the report</param>
    ''' <remarks>
    ''' The Report doesn't re-calculates all the results
    ''' The Controls data and processed results are the same used in GUI, the report filters and processes to show it
    ''' 
    ''' Created by:  JB 17/07/2012
    ''' </remarks>
    Public Shared Sub ShowQCAccumulatedResultsByTestReport(ByVal pQCTestSampleID As Integer, ByVal pDateFrom As Date, ByVal pDateTo As Date, _
                                                           ByVal pQCCumulatedSummaryDS As QCCumulatedSummaryDS, _
                                                           ByVal pQCCummulatedResultsDS As CumulatedResultsDS, _
                                                           ByVal pLocalDecimalAllow As Integer)
        Try
            Dim report As New QCCumulatedResultsByTestReport()

            Dim testSampleRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow = Nothing

            Dim myGlobalDataTO As New GlobalDataTO
            Dim myHistoryTestSamplesDelegate As New HistoryTestSamplesDelegate

            'Get the information from the History Tests/Sample Types table
            myGlobalDataTO = myHistoryTestSamplesDelegate.Read(Nothing, pQCTestSampleID)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                Dim myHistoryTestSampleDS As HistoryTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HistoryTestSamplesDS)
                If (myHistoryTestSampleDS.tqcHistoryTestSamples.Count > 0) Then
                    testSampleRow = myHistoryTestSampleDS.tqcHistoryTestSamples(0)
                End If
            End If

            report.SetControlsAndResultsDatasource(testSampleRow, pQCCumulatedSummaryDS, pQCCummulatedResultsDS, pLocalDecimalAllow, pDateFrom.ToString(DatePattern) & " - " & pDateTo.ToString(DatePattern))

            ShowPortrait(report)

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.ShowQCAccumulatedResultsByTestReport", EventLogEntryType.Error, False)
        End Try
    End Sub


#End Region


#Region "Private Methods"

    '''' <summary>
    '''' Calculates the age and age unit (Years, Months or Days) from a birth date
    '''' </summary>
    '''' <remarks>
    '''' Created by:  RH 05/12/2011    
    '''' </remarks>
    'Private Shared Function GetAgeUnits(ByVal pDateOfBirth As Date, ByVal AgeUnitsListDS As PreloadedMasterDataDS) As String
    '    Dim ageUnitDesc As String = String.Empty

    '    Try
    '        Dim patientAge As Long = 0
    '        Dim ageUnitCode As String = String.Empty

    '        patientAge = DateDiff(DateInterval.Day, pDateOfBirth, Today)

    '        If (patientAge >= 365) Then
    '            patientAge = DateDiff(DateInterval.Year, pDateOfBirth, Today)
    '            ageUnitCode = "Y"
    '        ElseIf (patientAge >= 30) Then
    '            patientAge = DateDiff(DateInterval.Month, pDateOfBirth, Today)
    '            ageUnitCode = "M"
    '        Else
    '            ageUnitCode = "D"
    '        End If

    '        'Get the multilanguage description for the Age Unit and return the value
    '        Dim lstAgeUnit As List(Of String) = _
    '                (From a In AgeUnitsListDS.tfmwPreloadedMasterData _
    '                 Where a.ItemID = ageUnitCode _
    '                 Select a.FixedItemDesc).ToList()

    '        If (lstAgeUnit.Count = 1) Then ageUnitDesc = String.Format("{0} {1}", patientAge, lstAgeUnit.First())

    '    Catch ex As Exception
    '        Dim myLogAcciones As New ApplicationLogManager()
    '        myLogAcciones.CreateLogActivity(ex.Message, "XRManager.GetAgeUnits", EventLogEntryType.Error, False)

    '    End Try

    '    Return ageUnitDesc
    'End Function

    ''' <summary>
    ''' Creates a new binded XRTableCell
    ''' </summary>
    ''' <param name="dataMember">Cell Datasource Binded DataMember</param>
    ''' <param name="Merge">Cell should be merged</param>
    ''' <param name="TextAlignment">Cell Text Alignment</param>
    ''' <returns>
    ''' A new binded XRTableCell
    ''' </returns>
    ''' <remarks>
    ''' Created by: RH 24/11/2011
    ''' </remarks>
    Private Shared Function CreateBindedTableCell(ByVal dataMember As String, Optional ByVal Merge As Boolean = False, _
                                           Optional ByVal TextAlignment As TextAlignment = Nothing, Optional ByVal Weight As Double = 0.0) As XRTableCell
        Dim Cell As New XRTableCell

        Cell.CanGrow = True
        Cell.WordWrap = True
        Cell.Name = dataMember

        Cell.DataBindings.Add(New XRBinding("Text", Nothing, dataMember))

        If Merge Then Cell.ProcessDuplicates = ValueSuppressType.Suppress

        If TextAlignment <> Nothing Then Cell.TextAlignment = TextAlignment

        If Weight > 0.0 Then
            Cell.Weight = Weight
        Else
            Cell.Weight = 1.0
        End If

        Cell.Padding = New PaddingInfo(1, 3, 1, 1)

        Return Cell
    End Function

    ''' <summary>
    ''' Creates a new not binded XRTableCell with a literal text value
    ''' </summary>
    ''' <param name="aText">Literal text value</param>
    ''' <param name="TextAlignment">Cell Text Alignment</param>
    ''' <returns>
    ''' A new not binded XRTableCell with a literal text value
    ''' </returns>
    ''' <remarks>
    ''' Created by: RH 24/11/2011
    ''' </remarks>
    Private Shared Function CreateTableCell(ByVal aText As String, Optional ByVal TextAlignment As TextAlignment = Nothing, Optional ByVal Weight As Double = Nothing) As XRTableCell
        Dim Cell As New XRTableCell

        Cell.Text = aText

        If TextAlignment <> Nothing Then Cell.TextAlignment = TextAlignment

        If Weight > 0.0 Then
            Cell.Weight = Weight
        Else
            Cell.Weight = 1.0
        End If

        Cell.Padding = New PaddingInfo(1, 3, 1, 1)

        Return Cell
    End Function

    ''' <summary>
    ''' Shows a XtraReport
    ''' </summary>
    ''' <param name="aXtraReport">The XtraReport to be configured</param>
    ''' <remarks>
    ''' Created by: RH 29/11/2011
    ''' </remarks>
    Private Shared Sub ShowPortrait(ByVal aXtraReport As XtraReport)
        Dim bReturn As Boolean = False
        Try
            'Make sure the Reports template is loaded
            'If String.IsNullOrEmpty(mtPortraitPath) Then
            If Not LoadDefaultPortraitTemplate() Then
                'No Reports template has been loaded.
                'Take the proper action!
                Return
            End If
            'End If

            Dim band As Band = aXtraReport.Bands.GetBandByType(GetType(TopMarginBand))
            If Not (band Is Nothing) Then aXtraReport.Bands.Remove(band)

            band = aXtraReport.Bands.GetBandByType(GetType(PageHeaderBand))
            If Not (band Is Nothing) Then aXtraReport.Bands.Remove(band)

            band = aXtraReport.Bands.GetBandByType(GetType(PageFooterBand))
            If Not (band Is Nothing) Then aXtraReport.Bands.Remove(band)

            aXtraReport.Landscape = False

            aXtraReport.Bands.Add(mtPortrait.Bands.GetBandByType(GetType(TopMarginBand)).Band)
            aXtraReport.Bands.Add(mtPortrait.Bands.GetBandByType(GetType(PageHeaderBand)).Band)
            aXtraReport.Bands.Add(mtPortrait.Bands.GetBandByType(GetType(PageFooterBand)).Band)


            'aXtraReport.ShowPreviewDialog()

            Using MyXRForm As New XRMainForm
                MyXRForm.Report = aXtraReport
                ' MyXRForm.Report.PaperKind = System.Drawing.Printing.PaperKind.A4
                MyXRForm.ShowDialog()
                'JVV 02/10
                bReturn = MyXRForm.IsReportPrinted
                'JVV 02/10
            End Using

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.GetResultsByTestForReport", EventLogEntryType.Error, False)

        End Try
    End Sub

    ''' <summary>
    ''' Shows a XtraReport
    ''' </summary>
    ''' <param name="aXtraReport">The XtraReport to be configured</param>
    ''' <remarks>
    ''' Created by: RH 29/11/2011
    ''' </remarks>
    Private Shared Sub ShowLandscape(ByVal aXtraReport As XtraReport)
        Dim bReturn As Boolean = False
        Try
            'Make sure the Reports template is loaded
            If String.IsNullOrEmpty(mtLandscapePath) Then
                If Not LoadDefaultLandscapeTemplate() Then
                    'No Reports template has been loaded.
                    'Take the proper action!
                    Return
                End If
            End If

            Dim band As Band = aXtraReport.Bands.GetBandByType(GetType(TopMarginBand))
            If Not (band Is Nothing) Then aXtraReport.Bands.Remove(band)

            band = aXtraReport.Bands.GetBandByType(GetType(PageHeaderBand))
            If Not (band Is Nothing) Then aXtraReport.Bands.Remove(band)

            band = aXtraReport.Bands.GetBandByType(GetType(PageFooterBand))
            If Not (band Is Nothing) Then aXtraReport.Bands.Remove(band)

            aXtraReport.Landscape = True

            aXtraReport.Bands.Add(mtLandscape.Bands("TopMargin").Band)
            aXtraReport.Bands.Add(mtLandscape.Bands("PageHeader").Band)
            aXtraReport.Bands.Add(mtLandscape.Bands("PageFooter").Band)

            Using MyXRForm As New XRMainForm
                MyXRForm.Report = aXtraReport
                MyXRForm.ShowDialog()
                'JVV 02/10
                bReturn = MyXRForm.IsReportPrinted
                'JVV 02/10
            End Using

        Catch ex As Exception
            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "XRManager.GetResultsByTestForReport", EventLogEntryType.Error, False)

        End Try
    End Sub

#End Region

End Class
