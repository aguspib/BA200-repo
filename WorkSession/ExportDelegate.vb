Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports System.IO
Imports System.Windows.Forms


Namespace Biosystems.Ax00.BL
    Partial Public Class ExportDelegate

#Region "Declarations"
        Private Enum ExportType
            ALL
            ORDER
            ORDERTEST
            NONE
        End Enum
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Manual Exportation of all Results accepted in the WorkSession
        ''' </summary>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pManualFileName">ONLINE or MANUAL file name</param>
        ''' <param name="pIncludeSentResults">Allow RESENT results (manual exportation) or not (online exportation)</param>
        ''' <returns>>GlobalDataTO containing a typed DataSet ExecutionsDS with the group of Executions of all OrderTests with Results exported</returns>
        ''' <remarks>
        ''' Created by: TR 12/07/2012
        ''' Modified by: SA 01/08/2012 - Changed the way of build the DS with the list of affected Executions; added searching of Patients with 
        '''                              an ExternalPID informed; removed parameter for open DB Connection
        '''              TR 28/08/2012 - Add parameter pIsResetWS To indicate the call is coming from reset WS.  
        '''              SG 10/04/2012 - Add parameter pIncludeSentResults
        '''                            - Inform this new parameter when call the method GetResultsToExport in ResultsDelegate
        '''                            - Read settings LIS_WITHFILES_MODE and LIS_ENABLE_COMMS
        '''                            - When setting LIS_WITHFILES_MODE = TRUE call the UpdateExportStatus with new parameter pAlternativeStatus = “”
        '''                              ElseIf setting LIS_ENABLE_COMMS = TRUE new parameter pAlternativeStatus = “SENDING”
        '''                              Else DO NOT CALL the UpdateExportStatus method
        '''              AG 14/02/2014 - #1505 Do not update ExportStatus = 'SENDING' when working with LIS, this status will be achieved when the xml is created!! - ACTIVATED 24/03/2014 (PAUSED 17/02/2014)
        '''              AG 26/08/2014 - #1886 Make code more readable: change the parameter name pIsResetWS for pManualFileName
        ''' </remarks>
        Public Function ExportToLISManualNEW(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                             Optional ByVal pManualFileName As Boolean = False, _
                                             Optional pIncludeSentResults As Boolean = False) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing

            Try

                'SG 10-04-2013
                Dim myUserSettingsDelegate As New UserSettingsDelegate
                Dim myUserSettingDS As New UserSettingDS

                'Read setting LIS_WITHFILES_MODE
                Dim isLISWithFilesMode As Boolean = False
                myGlobalDataTO = myUserSettingsDelegate.ReadBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WITHFILES_MODE.ToString())
                If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO IsNot Nothing Then
                    myUserSettingDS = DirectCast(myGlobalDataTO.SetDatos, UserSettingDS)
                    If myUserSettingDS.tcfgUserSettings.Count > 0 Then
                        'Set the value to my local variable.
                        isLISWithFilesMode = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                    End If
                End If

                'Read setting LIS_ENABLE_COMMS
                Dim isLISEnableComms As Boolean = False
                myGlobalDataTO = myUserSettingsDelegate.ReadBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString())
                If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO IsNot Nothing Then
                    myUserSettingDS = DirectCast(myGlobalDataTO.SetDatos, UserSettingDS)
                    If myUserSettingDS.tcfgUserSettings.Count > 0 Then
                        'Set the value to my local variable.
                        isLISEnableComms = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                    End If
                End If

                'end SG 10-04-2013

                Dim myResultsDelegate As New ResultsDelegate
                'myGlobalDataTO = myResultsDelegate.GetResultsToExport(Nothing, pWorkSessionID, pAnalyzerID)
                myGlobalDataTO = myResultsDelegate.GetResultsToExport(Nothing, pWorkSessionID, pAnalyzerID, String.Empty, String.Empty, -1, pIncludeSentResults)

                Dim myExportedExecutionsDS As New ExecutionsDS
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myResultsData As ResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)

                    'Continue only if there is not errors and there are Results to export
                    If (Not myGlobalDataTO.HasError AndAlso myResultsData.vwksResults.Count > 0) Then
                        'Get the list of different Patient Identifiers in the group of results 
                        Dim diffPatientsList As List(Of String) = (From a In myResultsData.vwksResults _
                                                              Where Not a.IsPatientIDNull _
                                                                 Select a.PatientID Distinct).ToList

                        Dim myPatientsDS As PatientsDS
                        Dim myPatientDelegate As New PatientDelegate
                        Dim lstPatientResults As List(Of ResultsDS.vwksResultsRow)

                        For Each patientID As String In diffPatientsList
                            'Verify if it has been sent by LIMS (field ExternalPID is informed) to return results using the same ID for the Patient
                            myGlobalDataTO = myPatientDelegate.GetPatientData(Nothing, patientID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myPatientsDS = DirectCast(myGlobalDataTO.SetDatos, PatientsDS)

                                If (myPatientsDS.tparPatients.Count > 0) Then
                                    If (myPatientsDS.tparPatients.First.ExternalPID.ToString <> String.Empty) Then
                                        'Search all Results to export for this Patient to replace the PatientID for the ExternalPID
                                        lstPatientResults = (From b As ResultsDS.vwksResultsRow In myResultsData.vwksResults _
                                                            Where b.PatientID = patientID _
                                                           Select b).ToList()

                                        For Each patientResult As ResultsDS.vwksResultsRow In lstPatientResults
                                            patientResult.BeginEdit()
                                            patientResult.PatientID = myPatientsDS.tparPatients.First.ExternalPID.ToString
                                            patientResult.EndEdit()
                                        Next
                                    End If
                                End If
                            Else
                                'Error verifying if the PatientID exists in DB and has informed an External ID
                                Exit For
                            End If
                        Next

                        lstPatientResults = Nothing
                        diffPatientsList = Nothing
                    End If

                    'TR 27/08/2012 -Validate if there are data to export.
                    If (Not myGlobalDataTO.HasError) AndAlso myResultsData.vwksResults.Count > 0 Then
                        'Write the Results in a TXT file and update export fields in myResultsData (ExportStatus, ExportDateTime)
                        Dim myResultsDataToFile As ResultsDS = TryCast(myResultsData.Copy, ResultsDS)

                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495 
                        Dim StartTime As DateTime = Now
                        Dim myLogAcciones As New ApplicationLogManager()
                        GlobalBase.CreateLogActivity("RESULTS NUMBER to EXPORT : " & myResultsData.vwksResults.Count.ToString(), _
                                 "ExportDelegate.ExportToLISManualNEW", EventLogEntryType.Information, False)
                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495

                        'DL 28/06/2013
                        If isLISWithFilesMode Then
                            'Write the Results in a TXT file and update export fields in myResultsData (ExportStatus, ExportDateTime)
                            myGlobalDataTO = CreateExportFileNEW(Nothing, myResultsDataToFile, pManualFileName) ' JB 22/10/2012 pWorkSessionID not needed
                        End If
                        'DL 28/06/2013

                        If (Not myGlobalDataTO.HasError) Then
                            'Update the Export Status of all exported Results

                            'SGM 10/04/2013
                            If isLISWithFilesMode Then
                                'When setting LIS_WITHFILES_MODE = TRUE call the UpdateExportStatus with new parameter pAlternativeStatus = “”
                                myGlobalDataTO = myResultsDelegate.UpdateExportStatus(Nothing, myResultsData, String.Empty)
                            ElseIf isLISEnableComms Then
                                'AG 24/03/2014 - AG 14/02/2014 - #1505 comment next line. The SENDING status will be updated once the xml message is generated - PAUSED 17/02/2014 (correction is comment next line)
                                ''Call function UpdateExportMASIVE to update the Export Status of all results in the DS to SENDING
                                'myGlobalDataTO = myResultsDelegate.UpdateExportStatusMASIVE(myResultsData, "SENDING")
                            End If

                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495
                            GlobalBase.CreateLogActivity("UpdateExportStatus Method: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                            "ExportDelegate.ExportToLISManualNEW", EventLogEntryType.Information, False)
                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495 


                            If (Not myGlobalDataTO.HasError) Then
                                Dim myExecutionDS As New ExecutionsDS
                                Dim myExecutionDelegate As New ExecutionsDelegate

                                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495
                                StartTime = Now
                                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495

                                'Get the different OrderTest for all exported Results and process them to get affected Executions
                                Dim diffOrderTests As List(Of Integer) = (From a In myResultsData.vwksResults _
                                                                        Select a.OrderTestID Distinct).ToList()

                                Dim diffRerunNumbers As List(Of Integer)
                                For Each otID As Integer In diffOrderTests
                                    'Get all different Reruns for the OrderTest
                                    diffRerunNumbers = (From b In myResultsData.vwksResults _
                                                       Where b.OrderTestID = otID _
                                                      Select b.RerunNumber Distinct).ToList()

                                    For Each reRunNum As Integer In diffRerunNumbers
                                        'Get all Executions for the OrderTestID and RerunNumber
                                        myGlobalDataTO = myExecutionDelegate.GetExportedExecutions(Nothing, pAnalyzerID, pWorkSessionID, otID, reRunNum)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            If (myExportedExecutionsDS.twksWSExecutions.Rows.Count = 0) Then
                                                myExportedExecutionsDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)
                                            Else
                                                'Get the Executions in an auxiliary DataSet and move them to the final DS
                                                myExecutionDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)
                                                For Each execRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions
                                                    myExportedExecutionsDS.twksWSExecutions.ImportRow(execRow)
                                                Next
                                            End If
                                        Else
                                            'Error getting affected Executions for the OrderTestID/RerunNumber
                                            Exit For
                                        End If
                                    Next

                                    'If there was an error in the internal loop, them EXIT
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Next
                                diffOrderTests = Nothing
                                diffRerunNumbers = Nothing

                                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495
                                GlobalBase.CreateLogActivity("Get OrderTest for all exported Results: " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                                "ExportDelegate.ExportToLISManualNEW", EventLogEntryType.Information, False)
                                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES *** XB 12/02/2014 - Task #1495

                            End If
                        End If
                    End If
                End If

                myGlobalDataTO.SetDatos = myExportedExecutionsDS
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ExportToLISManualNEW", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Manual Exportation of the Historical Results in parameter
        ''' </summary>
        ''' <param name="pHistOrderTestIDs">The list of HistOrderTestId to be exported</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  JB 22/10/2012 - Adapted from "ExportToLISManualNEW"
        ''' Modified by: TR 27/08/2012 - Before calling the function that generate the Export file, validate if there are data to export
        '''              SA 17/12/2012 - Changes due to query in function thisWSResultsDAO.GetResultsToExportFromHIST has been changed: results from 
        '''                              Patients saved in Historic Patients table have field SampleID informed; the rest of results have field PatientID
        '''                              informed. For results in the first group, if the saved Patient was sent from LIMS, field PatientID is informed
        '''                              with value of field ExternalPID; otherwise, it is informed with value of field PatientID
        '''              SG 06/03/2013 - If not error instead of setdatos as Boolean this method must return a ExecutionsDS.twksWSExecutions
        '''              SG 10/04/2013 - Read settings LIS_WITHFILES_MODE and LIS_ENABLE_COMMS
        '''              SG 25/04/2013 - Inform the results rows with TestType, SampleType, SampleID and LISRequest
        '''              SA 15/01/2014 - BT #1453 ==> Function has been re-written to optimize it and separate the code used for Export to LIS using Files
        '''                                           from code used to Export to LIS using the ES Library  
        '''                                           NOTE: CODE HAS BEEN COMMENTED TO GENERATE BETA 3 OF v3.0
        ''' Modified by: AG 14/02/2014 - #1505 Do not update ExportStatus = 'SENDING' when working with LIS, this status will be achieved when the xml is created!! - ACTIVATED 24/03/2014 (PAUSED 17/02/2014)
        ''' </remarks>
        Public Function ExportToLISManualFromHIST(ByVal pHistOrderTestIDs As List(Of Integer)) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing

            Try
                Dim myUserSettingDS As New UserSettingDS
                Dim myUserSettingsDelegate As New UserSettingsDelegate

                'Read setting LIS_WITHFILES_MODE
                Dim isLISWithFilesMode As Boolean = False
                myGlobalDataTO = myUserSettingsDelegate.ReadBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WITHFILES_MODE.ToString())
                If (Not myGlobalDataTO.HasError AndAlso myGlobalDataTO IsNot Nothing) Then
                    myUserSettingDS = DirectCast(myGlobalDataTO.SetDatos, UserSettingDS)
                    If (myUserSettingDS.tcfgUserSettings.Count > 0) Then
                        isLISWithFilesMode = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                    End If
                End If

                'Read setting LIS_ENABLE_COMMS
                Dim isLISEnableComms As Boolean = False
                myGlobalDataTO = myUserSettingsDelegate.ReadBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString())
                If (Not myGlobalDataTO.HasError AndAlso myGlobalDataTO IsNot Nothing) Then
                    myUserSettingDS = DirectCast(myGlobalDataTO.SetDatos, UserSettingDS)
                    If (myUserSettingDS.tcfgUserSettings.Count > 0) Then
                        isLISEnableComms = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                    End If
                End If

                'SGM 06/03/2013
                Dim myExecutionsDS As New ExecutionsDS
                Dim myHisResultsDelegate As New HisWSResultsDelegate

                myGlobalDataTO = myHisResultsDelegate.GetResultsToExportFromHIST(Nothing, pHistOrderTestIDs)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myResultsData As ResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)

                    'Continue only if there is not errors and there are Results to export
                    If (Not myGlobalDataTO.HasError AndAlso myResultsData.vwksResults.Count > 0) Then
                        'Get the list of different Patient Identifiers in the group of results 
                        Dim diffPatientsList As List(Of String) = (From a In myResultsData.vwksResults _
                                                                   Where Not a.IsSampleIDNull _
                                                                   Select a.SampleID Distinct).ToList

                        Dim myHisPatientsDS As HisPatientDS
                        Dim myHisPatientDelegate As New HisPatientsDelegate
                        Dim lstPatientResults As List(Of ResultsDS.vwksResultsRow)

                        For Each hisPatientID As String In diffPatientsList

                            'Verify if it has been sent by LIMS (field ExternalPID is informed) to return results using the same ID for the Patient
                            myGlobalDataTO = myHisPatientDelegate.GetPatientData(Nothing, hisPatientID)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                myHisPatientsDS = DirectCast(myGlobalDataTO.SetDatos, HisPatientDS)

                                'Search all Results to export for this Patient
                                lstPatientResults = (From b As ResultsDS.vwksResultsRow In myResultsData.vwksResults _
                                                Where Not b.IsSampleIDNull AndAlso b.SampleID = hisPatientID _
                                                   Select b).ToList()

                                If (myHisPatientsDS.thisPatients.Count > 0) Then
                                    'If the Patient was sent from LIMS, inform the PatientID field in the DS with value of field ExternalPID
                                    If (Not String.IsNullOrEmpty(myHisPatientsDS.thisPatients.First.ExternalPID.ToString)) Then
                                        For Each patientResult As ResultsDS.vwksResultsRow In lstPatientResults
                                            patientResult.BeginEdit()
                                            patientResult.PatientID = myHisPatientsDS.thisPatients.First.ExternalPID.ToString
                                            patientResult.EndEdit()
                                        Next
                                    Else
                                        'If the Patient was not sent from LIMS, inform the PatientID field in the DS with value of field SampleID
                                        For Each patientResult As ResultsDS.vwksResultsRow In lstPatientResults
                                            patientResult.BeginEdit()
                                            patientResult.PatientID = patientResult.SampleID
                                            patientResult.EndEdit()
                                        Next
                                    End If

                                End If

                            Else
                                'Error verifying if the PatientID exists in DB and has informed an External ID
                                Exit For
                            End If
                        Next

                        lstPatientResults = Nothing
                        diffPatientsList = Nothing
                    End If

                    If (Not myGlobalDataTO.HasError AndAlso myResultsData.vwksResults.Count > 0) Then
                        'SGM 06/03/2013
                        For Each OId As Integer In pHistOrderTestIDs
                            'search by informed OrderTestID
                            Dim lstResults As List(Of ResultsDS.vwksResultsRow)
                            lstResults = (From b As ResultsDS.vwksResultsRow In myResultsData.vwksResults _
                                         Where b.OrderTestID = OId Select b).ToList()
                            If (lstResults.Count > 0) Then
                                Dim myExeRow As ExecutionsDS.twksWSExecutionsRow = myExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow
                                With myExeRow
                                    .BeginEdit()
                                    .OrderTestID = OId
                                    .SampleClass = lstResults.First.SampleClass

                                    .AnalyzerID = lstResults.First.AnalyzerID
                                    .WorkSessionID = lstResults.First.WorkSessionID
                                    .RerunNumber = 1
                                    'SGM 25/04/2013
                                    If Not lstResults.First.IsLISRequestNull Then
                                        .LISRequest = lstResults.First.LISRequest
                                    Else
                                        .LISRequest = False
                                    End If
                                    .TestType = lstResults.First.TestType
                                    .SampleType = lstResults.First.SampleType

                                    If Not lstResults.First.IsPatientIDNull Then
                                        .SampleID = lstResults.First.PatientID
                                    Else
                                        .SampleID = lstResults.First.SampleID
                                    End If

                                    'end SGM 25/04/2013
                                    .EndEdit()
                                End With
                                myExecutionsDS.twksWSExecutions.AddtwksWSExecutionsRow(myExeRow)
                            End If
                        Next

                        myExecutionsDS.AcceptChanges()
                        'end SGM 06/03/2013

                        'DL 28/06/2013
                        If isLISWithFilesMode Then
                            'Write the Results in a TXT file and update export fields in myResultsData (ExportStatus, ExportDateTime)
                            myGlobalDataTO = CreateExportFileNEW(Nothing, myResultsData, True)
                        End If
                        'DL 28/06/2013

                        If (Not myGlobalDataTO.HasError) Then
                            'Update the Export Status of all exported Results

                            'SGM 10/04/2013
                            If isLISWithFilesMode Then
                                'When setting LIS_WITHFILES_MODE = TRUE call the UpdateExportStatus with new parameter pAlternativeStatus = “”
                                myGlobalDataTO = myHisResultsDelegate.UpdateExportStatus(Nothing, myResultsData, "")
                            ElseIf isLISEnableComms Then
                                'AG 24/03/2014 - AG 14/02/2014 - #1505 comment next line. The SENDING status will be updated once the xml message is generated - PAUSED 17/02/2014 (correction is comment next line)
                                ''setting LIS_ENABLE_COMMS = TRUE new parameter pAlternativeStatus = “SENDING”
                                'myGlobalDataTO = myHisResultsDelegate.UpdateExportStatus(Nothing, myResultsData, "SENDING")
                            Else
                                'DO NOT CALL the UpdateExportStatus method
                            End If
                        End If
                    End If
                End If
                myGlobalDataTO.SetDatos = myExecutionsDS

                '' ** BEGIN: NEW FUNCTION CODE FOR BT #1453 - FUNCTION WAS RE-WRITTEN AND OPTIMIZED
                'Dim myUserSettingDS As New UserSettingDS
                'Dim myUserSettingsDelegate As New UserSettingsDelegate

                ''Get current value of setting LIS_WITHFILES_MODE
                'Dim isLISWithFilesMode As Boolean = False
                'myGlobalDataTO = myUserSettingsDelegate.ReadBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_WITHFILES_MODE.ToString())
                'If (Not myGlobalDataTO.HasError AndAlso myGlobalDataTO IsNot Nothing) Then
                '    myUserSettingDS = DirectCast(myGlobalDataTO.SetDatos, UserSettingDS)
                '    If (myUserSettingDS.tcfgUserSettings.Count > 0) Then isLISWithFilesMode = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                'End If

                'If (isLISWithFilesMode) Then
                '    'EXPORT HISTORICAL PATIENT RESULTS USING FILES
                '    Dim myHisResultsDelegate As New HisWSResultsDelegate

                '    myGlobalDataTO = myHisResultsDelegate.GetResultsToExportFromHIST(Nothing, pHistOrderTestIDs)
                '    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '        Dim myResultsData As ResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)

                '        'Continue only if there is not errors and there are Results to export
                '        If (Not myGlobalDataTO.HasError AndAlso myResultsData.vwksResults.Count > 0) Then
                '            'Get the list of different Patient Identifiers in the group of results 
                '            Dim diffPatientsList As List(Of String) = (From a In myResultsData.vwksResults _
                '                                                      Where Not a.IsSampleIDNull _
                '                                                     Select a.SampleID Distinct).ToList

                '            Dim myHisPatientsDS As HisPatientDS
                '            Dim myHisPatientDelegate As New HisPatientsDelegate
                '            Dim lstPatientResults As List(Of ResultsDS.vwksResultsRow)

                '            For Each hisPatientID As String In diffPatientsList
                '                'Verify if it has been sent by LIMS (field ExternalPID is informed) to return results using the same ID for the Patient
                '                myGlobalDataTO = myHisPatientDelegate.GetPatientData(Nothing, hisPatientID)
                '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '                    myHisPatientsDS = DirectCast(myGlobalDataTO.SetDatos, HisPatientDS)

                '                    'Search all Results to export for this Patient
                '                    lstPatientResults = (From b As ResultsDS.vwksResultsRow In myResultsData.vwksResults _
                '                                        Where Not b.IsSampleIDNull AndAlso b.SampleID = hisPatientID _
                '                                       Select b).ToList()

                '                    If (myHisPatientsDS.thisPatients.Count > 0) Then
                '                        'If the Patient was sent from LIMS, inform the PatientID field in the DS with value of field ExternalPID
                '                        If (Not String.IsNullOrEmpty(myHisPatientsDS.thisPatients.First.ExternalPID.ToString)) Then
                '                            For Each patientResult As ResultsDS.vwksResultsRow In lstPatientResults
                '                                patientResult.BeginEdit()
                '                                patientResult.PatientID = myHisPatientsDS.thisPatients.First.ExternalPID.ToString
                '                                patientResult.EndEdit()
                '                            Next
                '                        Else
                '                            'If the Patient was not sent from LIMS, inform the PatientID field in the DS with value of field SampleID
                '                            For Each patientResult As ResultsDS.vwksResultsRow In lstPatientResults
                '                                patientResult.BeginEdit()
                '                                patientResult.PatientID = patientResult.SampleID
                '                                patientResult.EndEdit()
                '                            Next
                '                        End If
                '                    End If
                '                Else
                '                    'Error verifying if the PatientID exists in DB and has informed an External ID
                '                    Exit For
                '                End If
                '            Next

                '            lstPatientResults = Nothing
                '            diffPatientsList = Nothing

                '            If (Not myGlobalDataTO.HasError) Then
                '                'Write the Results in a TXT file and update export fields in myResultsData (ExportStatus, ExportDateTime)
                '                myGlobalDataTO = CreateExportFileNEW(Nothing, myResultsData, True)
                '            End If

                '            If (Not myGlobalDataTO.HasError) Then
                '                'When setting LIS_WITHFILES_MODE = TRUE call the UpdateExportStatus with new parameter pAlternativeStatus = “”
                '                myGlobalDataTO = myHisResultsDelegate.UpdateExportStatus(Nothing, myResultsData, String.Empty)
                '            End If

                '            myGlobalDataTO.SetDatos = New ExecutionsDS
                '        End If
                '    End If
                'Else
                '    'EXPORT HISTORICAL PATIENT RESULTS USING ES LIBRARY

                '    'Get current value of setting LIS_ENABLE_COMMS
                '    Dim isLISEnableComms As Boolean = False
                '    myGlobalDataTO = myUserSettingsDelegate.ReadBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString())
                '    If (Not myGlobalDataTO.HasError AndAlso myGlobalDataTO IsNot Nothing) Then
                '        myUserSettingDS = DirectCast(myGlobalDataTO.SetDatos, UserSettingDS)
                '        If (myUserSettingDS.tcfgUserSettings.Count > 0) Then isLISEnableComms = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                '    End If

                '    Dim myExecutionsDS As New ExecutionsDS
                '    If (isLISEnableComms) Then
                '        'Get all data needed 
                '        Dim myHisWSOTDelegate As New HisWSOrderTestsDelegate

                '        myGlobalDataTO = myHisWSOTDelegate.GetDataToExportFromHIST(Nothing, pHistOrderTestIDs)
                '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '            myExecutionsDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)

                '            If (myExecutionsDS.twksWSExecutions.Count > 0) Then
                '                Dim myHisResultsDelegate As New HisWSResultsDelegate
                '                myGlobalDataTO = myHisResultsDelegate.UpdateLISExportStatus(Nothing, pHistOrderTestIDs, "SENDING")
                '            End If
                '        End If
                '    End If
                '    myGlobalDataTO.SetDatos = myExecutionsDS
                'End If
                '' ** END: NEW FUNCTION CODE FOR BT #1453 - FUNCTION WAS RE-WRITTEN AND OPTIMIZED
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ExportToLISManualFromHIST", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Main method, checks the programmed frequency and launch the exportation when needed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pCalledForOFFSystem">When TRUE, it indicates the function has been called after adding Results for OFF-SYSTEM Tests and
        '''                                   in this case, the received open DB Connection has to be used to avoid death locks with the 
        '''                                   transaction opened in SaveOffSystemResults function</param>
        ''' <returns>GlobalDataTo with DS as ExecutionsDS with ordertestID DISTINCT exported</returns>
        ''' <remarks>
        ''' Created by:  TR 12/05/2010
        ''' Modified by: DL 17/03/2011 - Added parameters for SampleClass, TestType and ControlNumber
        '''              AG 21/05/2012 - Added parameter pResetWSFlag
        '''              TR 16/07/2012 - Method optimization 
        '''              SA 01/08/2012 - Removed the OpenDBTransaction due to it was opened but not used. Changed process when Export Frequency
        '''                              is by ORDER: call function OrdersDelegate.ReadByOrderTestID instead of OrderTestsDelegate.GetOrderTest +
        '''                              OrdersDelegate.ReadOrders; removed parameters pManualExportFlag, pResetWSFlag and all the optionals 
        '''                              due to they were not used; added new parameter to indicate the function has been called after adding Results
        '''                              for OFF-SYSTEM Tests, due to in that case, the received open DB Connection has to be used
        ''' </remarks>
        Public Function ManageLISExportation(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                             ByVal pOrderTestID As Integer, ByVal pCalledForOFFSystem As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Dim StartTime As DateTime = Now
            Dim myLogAcciones1 As New ApplicationLogManager()
            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

            Try
                'Special code for this function: the template can not be used....DO NOT CHANGE!!
                Dim dbConnection As SqlClient.SqlConnection = Nothing
                If (pCalledForOFFSystem AndAlso Not pDBConnection Is Nothing) Then dbConnection = pDBConnection

                Dim myExportType As New ExportType
                Dim IsAutomaticExport As Boolean = False
                Dim myExportFrecuency As String = String.Empty
                Dim myUserSettingDelegate As New UserSettingsDelegate()

                'Get the Exportation Type: Manual/Automatic
                myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUTOMATIC_EXPORT.ToString)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    If (CType(myGlobalDataTO.SetDatos, Integer) = 1) Then
                        IsAutomaticExport = True

                        'Exportation Type is Automatic, then get the programmed Export Frequency
                        myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.EXPORT_FREQUENCY.ToString)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myExportFrecuency = myGlobalDataTO.SetDatos.ToString()
                        End If
                    End If
                End If

                Dim myStatus As String = ""
                Dim exportedExecutionsDS As New ExecutionsDS
                If (Not myGlobalDataTO.HasError AndAlso IsAutomaticExport AndAlso myExportFrecuency <> String.Empty) Then
                    Select Case (myExportFrecuency)
                        '(1) EXPORT WHEN THE WORKSESSION HAS FINISHED
                        Case "END_WS"
                            'Get the Status of the WS
                            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate()
                            myGlobalDataTO = myWSAnalyzersDelegate.ReadWSAnalyzers(dbConnection, pAnalyzerID, pWorkSessionID)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myWSAnalyzersDS As WSAnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS)

                                If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
                                    myStatus = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus
                                    myExportType = ExportType.ALL
                                End If
                            End If
                            Exit Select

                            '(2) EXPORT WHEN ALL TESTS REQUESTED FOR THE PATIENT HAVE FINISHED
                        Case "ORDER"
                            'Get the Status of the Patient ORDER
                            Dim myOrdersDelegate As New OrdersDelegate
                            myGlobalDataTO = myOrdersDelegate.ReadByOrderTestID(dbConnection, pOrderTestID)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myOrdersDS As OrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)

                                If (myOrdersDS.twksOrders.Rows.Count > 0) Then
                                    myStatus = myOrdersDS.twksOrders(0).OrderStatus
                                    myExportType = ExportType.ORDER
                                End If
                            End If
                            Exit Select

                            '(3) EXPORT WHEN THE TEST HAS FINISHED
                        Case "ORDERTEST"
                            'Get the Status of the ORDER TEST
                            Dim myOrderTestsDelegate As New OrderTestsDelegate()
                            myGlobalDataTO = myOrderTestsDelegate.GetOrderTest(dbConnection, pOrderTestID)

                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                Dim myOrderTestsDS As OrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                                If (myOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
                                    myStatus = myOrderTestsDS.twksOrderTests(0).OrderTestStatus
                                    myExportType = ExportType.ORDERTEST
                                End If
                            End If
                            Exit Select
                    End Select

                    If (String.Equals(myStatus, "CLOSED")) Then
                        'Execute the Export process
                        myGlobalDataTO = ExecuteExportationNEW(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, myExportType)
                    Else
                        'The OrderTest, Order or WS is not CLOSED
                        myGlobalDataTO.SetDatos = exportedExecutionsDS
                    End If

                Else
                    'Exportation is configured as MANUAL: return an empty ExecutionsDS to avoid errors
                    myGlobalDataTO.SetDatos = exportedExecutionsDS
                End If

                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
                GlobalBase.CreateLogActivity("ManageLISExportation ExecuteExportation (Complete): " & Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
                                                 "ExportDelegate.ManageLISExportation", EventLogEntryType.Information, False)
                '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ManageLISExportation", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Create/open the export file and write all Results in the entry DS in it
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLimsResultsList">Typed DataSet ResultsDS containing in subtable vwksResults the group of Results to Export</param>
        ''' <param name="pManualFileName">ONLINE or MANUAL file name</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 12/07/2012 - Based on CreateExportFile
        ''' Modified by: SA 01/08/2012 - Added some changes in the 
        '''              TR 27/08/2012 - The new file name is get from the Now (File creation date) instead the 
        '''                              StartDateTime because NULL values are allowed. The date format change to (yy-MM-dd hh-mm-ss-fff)
        '''                              Depending if it is a reset worksession the file name prefix change from ONLINE to EXP.
        '''              JB 22/10/2012 - Removed parameter pWorkSessionID (not used)
        '''              SG 10/04/2012 - Get the filename from the SwParameters table in database not from the GlobalConstants
        '''              AG 26/08/2014 - #1886 Make code more readable: change the parameter name pIsResetWS for pManualFileName
        ''' </remarks>
        Private Function CreateExportFileNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLIMSResultsList As ResultsDS, _
                                             Optional ByVal pManualFileName As Boolean = False) As GlobalDataTO

            Dim TextFileWriter As StreamWriter = Nothing
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Special code for this function: the template can not be used....DO NOT CHANGE!!
                Dim dbConnection As SqlClient.SqlConnection = Nothing
                If (Not pDBConnection Is Nothing) Then dbConnection = pDBConnection

                Dim TextFileData As String
                Dim TextFileReader As StreamReader

                'Get the path for files to export to LIMS
                Dim myExportFolder As String = "\Export\"
                Dim myParams As New SwParametersDelegate

                myGlobalDataTO = myParams.ReadTextValueByParameterName(Nothing, GlobalEnumerates.SwParameters.LIMS_EXPORT_PATH.ToString, Nothing)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myExportFolder = CStr(myGlobalDataTO.SetDatos)
                End If

                If (myExportFolder.StartsWith("\") AndAlso Not myExportFolder.StartsWith("\\")) Then
                    myExportFolder = Application.StartupPath & myExportFolder
                End If

                'If the folder does not exist, create it
                If (Not Directory.Exists(myExportFolder) AndAlso Not myExportFolder.Trim = String.Empty) Then
                    Directory.CreateDirectory(myExportFolder)
                End If

                'Get the start date and time for the active WorkSession
                'Dim myWSDelegate As New WorkSessionsDelegate

                'myGlobalDataTO = myWSDelegate.GetByWorkSession(dbConnection, pWorkSessionID)

                'If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '    Dim myWSDataDS As WorkSessionsDS = DirectCast(myGlobalDataTO.SetDatos, WorkSessionsDS)

                '    If (myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsWSDateTimeNull)) Then
                '        'Set the value to local variable wsDateTime.
                '        wsDateTime = myWSDataDS.twksWorkSessions.First().WSDateTime
                '    End If
                'End If

                'TR 27/08/2012 -Set the Date current datetime 
                Dim wsDateTime As DateTime = Now

                'TR 27/08/2012 -To set the file prefix we need to now if the process is call from a reset worksession.
                Dim myPrefixFileName As String = "" ' Variable used to indicate the file prefix.

                'SG 10/04/2012 - Get the filename from the SwParameters table in database not from the GlobalConstants
                Dim mySWParametersDelegate As New SwParametersDelegate
                Dim mySWParametersDS As New ParametersDS
                If pManualFileName Then
                    myGlobalDataTO = mySWParametersDelegate.ReadByParameterName(dbConnection, GlobalEnumerates.SwParameters.MANUAL_EXPORT_FILENAME.ToString, Nothing)
                    If Not myGlobalDataTO.HasError Then
                        mySWParametersDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)
                        If mySWParametersDS.tfmwSwParameters.Count > 0 Then
                            myPrefixFileName = CStr(mySWParametersDS.tfmwSwParameters(0).ValueText)
                        End If
                    End If
                Else
                    myGlobalDataTO = mySWParametersDelegate.ReadByParameterName(dbConnection, GlobalEnumerates.SwParameters.ONLINE_EXPORT_FILENAME.ToString, Nothing)
                    If Not myGlobalDataTO.HasError Then
                        mySWParametersDS = DirectCast(myGlobalDataTO.SetDatos, ParametersDS)
                        If mySWParametersDS.tfmwSwParameters.Count > 0 Then
                            myPrefixFileName = CStr(mySWParametersDS.tfmwSwParameters(0).ValueText)
                        End If
                    End If
                End If

                'If pIsResetWS Then
                '    myPrefixFileName = GlobalConstants.RESET_EXPORT_FILENAME
                'Else
                '    myPrefixFileName = GlobalConstants.LIMS_EXPORT_FILE_NAME
                'End If
                'SG 10/04/2012

                'Set the name for the Export file the format is 
                Dim myExportFile As String = myPrefixFileName & _
                             " (" & wsDateTime.ToString("yy-MM-dd HH:mm:ss:fff").ToString.Replace(":", "-").ToString & ").txt"

                'Set the name for the Export file the format is 
                'Dim myExportFile As String = GlobalConstants.LIMS_EXPORT_FILE_NAME & _
                '                             " (" & wsDateTime.ToString("yy-MM-dd HH:mm").ToString.Replace(":", "-").ToString & ")_"

                'Get the last file number between all TXT files in the Export folder
                'Dim dra As IO.FileInfo
                'Dim fileId As Integer = 1
                'Dim di As New IO.DirectoryInfo(myExportFolder)
                'Dim diar1 As IO.FileInfo() = di.GetFiles(myExportFile & "*.txt")

                'Dim myValue As String
                'Dim myLength As Integer

                'For Each dra In diar1
                '    myLength = (dra.ToString.Length - 4) - myStartingPos
                '    myValue = dra.ToString.Substring(myStartingPos, myLength)

                '    If (IsNumeric(myValue) AndAlso fileId < CInt(myValue)) Then fileId = CInt(myValue)
                '    dra = Nothing
                'Next dra

                'di = Nothing
                'diar1 = Nothing
                'dra = Nothing

                'Get the name of the export file when the last generated file number
                'myExportFile &= fileId.ToString & ".txt"

                If (Not File.Exists(myExportFolder & myExportFile)) Then
                    'Validate the folder exists
                    If (Not Directory.Exists(myExportFolder) AndAlso Not myExportFolder.Trim = String.Empty) Then
                        Directory.CreateDirectory(myExportFolder)
                    End If

                    'If the file does not exist, then create a new one
                    TextFileWriter = New StreamWriter(myExportFolder & myExportFile)
                Else
                    'If the file exits then load it into a stream variable
                    TextFileReader = New StreamReader(myExportFolder & myExportFile)

                    'Copy all the lines into a text variable
                    TextFileData = TextFileReader.ReadToEnd.TrimEnd()

                    'Close the file to avoid errors
                    TextFileReader.Close()

                    ''Verify if the current stream supports writing
                    'Dim canWrite As Boolean = True
                    'Try
                    '    Dim fs As New FileStream(myExportFolder & myExportFile, FileMode.OpenOrCreate, FileAccess.Write)
                    '    If (Not fs.CanWrite) Then canWrite = False
                    '    fs.Close()
                    'Catch ex As Exception
                    '    canWrite = False
                    'End Try

                    'If (Not canWrite) Then
                    '    'A new export file has to be generated incrementing the file number
                    '    fileId += 1
                    '    myExportFile = myPrefixFileName & _
                    '                   " (" & wsDateTime.ToString("yy-MM-dd HH:mm").ToString.Replace(":", "-").ToString & ")_" & _
                    '                   fileId.ToString & ".txt"

                    '    'myExportFile = GlobalConstants.LIMS_EXPORT_FILE_NAME & _
                    '    '                " (" & CDate(wsDateTime).ToString("yy-MM-dd HH:mm").ToString.Replace(":", "-").ToString & ")_" & _
                    '    '                fileId.ToString & ".txt"

                    'End If

                    'Set the file to a Stream Writer
                    TextFileWriter = New StreamWriter(myExportFolder & myExportFile)

                    'if there are any data on the variable then load into the Stream writer variable.
                    If (TextFileData.ToString.Length > 0) Then
                        TextFileWriter.Write(TextFileData.ToString() & Environment.NewLine)
                    End If
                End If

                Dim myGlobalBase As New GlobalBase

                'Finally, write all Results in the TXT file
                Dim myTubeType As String
                Dim myPatientID As String
                Dim resultCONCValue As String
                For Each myResultsRow As ResultsDS.vwksResultsRow In pLIMSResultsList.vwksResults
                    resultCONCValue = String.Empty
                    If (myResultsRow.IsManualResultFlagNull OrElse Not myResultsRow.ManualResultFlag) Then
                        'It is not a manual RESULT, export the calculated CONCENTRATION value
                        resultCONCValue = myResultsRow.CONC_Value.ToString
                    Else
                        If (Not myResultsRow.IsManualResultNull) Then
                            'It is a quantitative manual RESULT
                            resultCONCValue = myResultsRow.ManualResult.ToString
                        ElseIf (Not myResultsRow.IsManualResultTextNull) Then
                            'It is a qualitative manual RESULT
                            resultCONCValue = myResultsRow.ManualResultText.ToString
                        End If
                    End If

                    myPatientID = String.Empty
                    If (Not myResultsRow.IsPatientIDNull) Then myPatientID = myResultsRow.PatientID

                    'TR 28/08/2012 -Set the Control name in case sample class is Q (Control).
                    If (myResultsRow.SampleClass = "Q") Then myPatientID = myResultsRow.ControlName

                    myTubeType = String.Empty
                    If (Not myResultsRow.IsTubeTypeNull) Then myTubeType = myResultsRow.TubeType

                    myResultsRow.ExportStatus = "SENT"
                    myResultsRow.ExportDateTime = DateTime.Now
                    myResultsRow.TS_User = GlobalBase.GetSessionInfo().UserName
                    myResultsRow.TS_DateTime = DateTime.Now

                    'TR 28/08/2012 -Add the Control name and lotNumber columns.
                    TextFileWriter.WriteLine(myResultsRow.SampleClass & vbTab & _
                                             String.Format("{0,-28}", myPatientID.ToString) & vbTab & _
                                             String.Format("{0,-3}", myResultsRow.SampleType.ToString) & vbTab & _
                                             String.Format("{0,-3}", myTubeType) & vbTab & _
                                             String.Format("{0,-16}", myResultsRow.TestName.ToString) & vbTab & _
                                             String.Format("{0,-4}", myResultsRow.TestType.ToString) & vbTab & _
                                             String.Format("{0,-20}", resultCONCValue.ToString) & vbTab & _
                                             String.Format("{0,-10}", myResultsRow.MeasureUnit.ToString) & vbTab & _
                                             String.Format("{0,-19}", myResultsRow.ResultDateTime.ToString()) & vbTab & _
                                             String.Format("{0,-20}", myResultsRow.LotNumber))
                Next

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.CreateExportFileNEW", EventLogEntryType.Error, False)
            Finally
                'Close the file 
                If (Not TextFileWriter Is Nothing) Then TextFileWriter.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Results to Export to LIMS, write them in a TXT file, update the Export fields in Results table and return a DataSet
        ''' with the affected Executions (those that have to be updated in Monitor screen)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pExportType">Export frequency code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with the group of Executions of all OrderTests with Results exported</returns>
        ''' <remarks>
        ''' Created by:  TR 16/07/2012 - Based on ExecuteExportation
        ''' Modified by: SA 01/08/2012 - Changed the way of build the DS with the list of affected Executions
        ''' Modified by: SG 10/04/2010 - Read settings LIS_WITHFILES_MODE and LIS_ENABLE_COMMS
        '''              TR 27/06/2013 - Pass the dbConnection use the same transaction instead of nothing, this is to avoid time out exception. 
        '''                              more info BugTracking # 1213.
        ''' Modified by: AG 14/02/2014 - #1505 Do not update ExportStatus = 'SENDING' when working with LIS, this status will be achieved when the xml is created!! - ACTIVATED 24/03/2014 (PAUSED 17/02/2014)
        ''' </remarks>
        Private Function ExecuteExportationNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                               ByVal pOrderTestID As Integer, ByVal pExportType As ExportType) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim myExportedExecutionsDS As New ExecutionsDS

            Try
                'Special code for this function: the template can not be used....DO NOT CHANGE!!
                Dim dbConnection As SqlClient.SqlConnection = Nothing
                If (Not pDBConnection Is Nothing) Then dbConnection = pDBConnection

                'SG 10-04-2013
                Dim myUserSettingsDelegate As New UserSettingsDelegate
                Dim myUserSettingDS As New UserSettingDS

                'Read setting LIS_WITHFILES_MODE
                Dim isLISWithFilesMode As Boolean = False
                myGlobalDataTO = myUserSettingsDelegate.ReadBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.LIS_WITHFILES_MODE.ToString())
                If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO IsNot Nothing Then
                    myUserSettingDS = DirectCast(myGlobalDataTO.SetDatos, UserSettingDS)
                    If myUserSettingDS.tcfgUserSettings.Count > 0 Then
                        'Set the value to my local variable.
                        isLISWithFilesMode = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                    End If
                End If

                'Read setting LIS_ENABLE_COMMS
                Dim isLISEnableComms As Boolean = False
                myGlobalDataTO = myUserSettingsDelegate.ReadBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.LIS_ENABLE_COMMS.ToString())
                If Not myGlobalDataTO.HasError AndAlso myGlobalDataTO IsNot Nothing Then
                    myUserSettingDS = DirectCast(myGlobalDataTO.SetDatos, UserSettingDS)
                    If myUserSettingDS.tcfgUserSettings.Count > 0 Then
                        'Set the value to my local variable.
                        isLISEnableComms = CBool(myUserSettingDS.tcfgUserSettings(0).CurrentValue)
                    End If
                End If

                'end SG 10-04-2013

                'Get all Results to export to LIMS
                myGlobalDataTO = GetDataToExport(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pExportType)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    Dim myResultsData As ResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)

                    If (myResultsData.vwksResults.Count > 0) Then
                        'DL 28/06/2013
                        If isLISWithFilesMode Then
                            'Write the Results in a TXT file and update export fields in myResultsData (ExportStatus, ExportDateTime)
                            myGlobalDataTO = CreateExportFileNEW(dbConnection, myResultsData) 'JB 22/10/2012 pWorkSessionID not needed
                        End If
                        'DL 28/06/2013

                        If (Not myGlobalDataTO.HasError) Then
                            'Update the Export Status of all exported Results
                            Dim myResultsDelegate As New ResultsDelegate()

                            'SGM 10/04/2013
                            If isLISWithFilesMode Then
                                'When setting LIS_WITHFILES_MODE = TRUE call the UpdateExportStatus with new parameter pAlternativeStatus = “”
                                myGlobalDataTO = myResultsDelegate.UpdateExportStatus(dbConnection, myResultsData, "") 'TR 27/06/2013 -use the dbconnection instead of nothing
                            ElseIf isLISEnableComms Then
                                'AG 24/03/2014 AG 14/02/2014 - #1505 comment next line. The SENDING status will be updated once the xml message is generated - PAUSED 17/02/2014 (correction is comment next line)
                                ''setting LIS_ENABLE_COMMS = TRUE new parameter pAlternativeStatus = “SENDING”
                                'myGlobalDataTO = myResultsDelegate.UpdateExportStatus(dbConnection, myResultsData, "SENDING") 'TR 27/06/2013 -use the dbconnection instead of nothing
                            Else
                                'DO NOT CALL the UpdateExportStatus method

                            End If
                            'end SGM 10/04/2013

                            If (Not myGlobalDataTO.HasError) Then
                                Dim myExecutionDS As New ExecutionsDS
                                Dim myExecutionDelegate As New ExecutionsDelegate

                                'Get the different OrderTest for all exported Results and process them to get affected Executions
                                Dim diffOrderTests As List(Of Integer) = (From a In myResultsData.vwksResults _
                                                                          Select a.OrderTestID Distinct).ToList()

                                Dim diffRerunNumbers As List(Of Integer)
                                For Each otID As Integer In diffOrderTests
                                    'Get all different Reruns for the OrderTest
                                    diffRerunNumbers = (From b In myResultsData.vwksResults _
                                                       Where b.OrderTestID = otID _
                                                      Select b.RerunNumber Distinct).ToList()

                                    For Each reRunNum As Integer In diffRerunNumbers
                                        'Get all Executions for the OrderTestID and RerunNumber
                                        myGlobalDataTO = myExecutionDelegate.GetExportedExecutions(dbConnection, pAnalyzerID, pWorkSessionID, otID, reRunNum)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            If (myExportedExecutionsDS.twksWSExecutions.Rows.Count = 0) Then
                                                myExportedExecutionsDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)
                                            Else
                                                'Get the Executions in an auxiliary DataSet and move them to the final DS
                                                myExecutionDS = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS)
                                                For Each execRow As ExecutionsDS.twksWSExecutionsRow In myExecutionDS.twksWSExecutions
                                                    myExportedExecutionsDS.twksWSExecutions.ImportRow(execRow)
                                                Next
                                            End If
                                        Else
                                            'Error getting affected Executions for the OrderTestID/RerunNumber
                                            Exit For
                                        End If
                                    Next
                                    diffOrderTests = Nothing
                                    diffRerunNumbers = Nothing

                                    'If there was an error in the internal loop, them EXIT
                                    If (myGlobalDataTO.HasError) Then Exit For
                                Next
                            End If
                        End If
                    End If
                    myGlobalDataTO.SetDatos = myExportedExecutionsDS
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorCode = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ExecuteExportationNEW", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all results to export to LIMS depending on the programmed frequency:  after WS finished/By Patient/By OrderTestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pOrderTestID">Order Test Identifier</param>
        ''' <param name="pExportType">Export Type (ENUM)</param>
        ''' <returns>GlobalDataTO containing a typed DS ResultsDS (subtable vwksResults) with the group of Patient and Control 
        '''          results to export to LIMS</returns>
        ''' <remarks>
        ''' Created by:  TR 13/07/2012 - Based on GetDataToExportOLD
        ''' Modified by: SA 01/08/2012 - Get all different Patients in the group of results to export and for each one of them, verify 
        '''                              if it has an External PatientID (which means the Patient was sent previously from LIMS) and in this
        '''                              case, replace value of field PatientID in the ResultsDS for value of field ExternalPID for all results
        '''                              to export for the patient
        ''' </remarks>
        Private Function GetDataToExport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                         ByVal pOrderTestID As Integer, ByVal pExportType As ExportType) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                'Special code for this function: the template can not be used....DO NOT CHANGE!!
                Dim dbConnection As SqlClient.SqlConnection = Nothing
                If (Not pDBConnection Is Nothing) Then dbConnection = pDBConnection

                Dim myTestID As Integer = 0
                Dim myTestType As String = String.Empty
                Dim myOrderID As String = String.Empty
                Dim mySampleType As String = String.Empty
                Dim mySampleClass As String = String.Empty

                Dim myResultsDS As New ResultsDS()
                Dim myResultsDelegate As New ResultsDelegate()

                Dim myOrderTestDS As New OrderTestsDS()
                Dim myOrderTestsDelegate As New OrderTestsDelegate()

                'Get needed OrderTest data: OrderID, SampleClass, TestType, TestID and SampleType
                myGlobalDataTO = myOrderTestsDelegate.GetOrderTest(dbConnection, pOrderTestID)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

                    If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
                        myOrderID = myOrderTestDS.twksOrderTests(0).OrderID
                        mySampleClass = myOrderTestDS.twksOrderTests(0).SampleClass

                        myTestType = myOrderTestDS.twksOrderTests(0).TestType
                        myTestID = myOrderTestDS.twksOrderTests(0).TestID
                        mySampleType = myOrderTestDS.twksOrderTests(0).SampleType

                        Select Case (mySampleClass)
                            Case "BLANK"
                                'Continue only if the OrderTestStatus is CLOSED
                                If (myOrderTestDS.twksOrderTests(0).OrderTestStatus = "CLOSED") Then
                                    'Get related CTRL and PATIENT Results to export (PATIENT and CONTROL Results for the same Standard Test)
                                    myGlobalDataTO = myResultsDelegate.GetResultsToExportForBLANKAndCALIB(dbConnection, pWorkSessionID, pAnalyzerID, "STD", myTestID)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)
                                    End If
                                End If
                                Exit Select

                            Case "CALIB"
                                'Continue only if the OrderTestStatus is CLOSED
                                If (myOrderTestDS.twksOrderTests(0).OrderTestStatus = "CLOSED") Then
                                    'Get related CTRL and PATIENT Results to export (PATIENT and CONTROL Results for the same Standard Test and Sample Type)
                                    myGlobalDataTO = myResultsDelegate.GetResultsToExportForBLANKAndCALIB(dbConnection, pWorkSessionID, pAnalyzerID, "STD", myTestID, mySampleType)
                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                        myResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)
                                    End If
                                End If
                                Exit Select

                            Case "CTRL", "PATIENT"
                                'Process will depend on the configured Export Frequency
                                Select Case (pExportType)
                                    Case (ExportType.ALL)
                                        'Get all CTRL and PATIENT Results to export
                                        myGlobalDataTO = myResultsDelegate.GetResultsToExport(dbConnection, pWorkSessionID, pAnalyzerID)
                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                            myResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)
                                        End If
                                        Exit Select

                                    Case (ExportType.ORDER)
                                        'Continue only if the Order is CLOSED
                                        If (myOrderTestDS.twksOrderTests(0).OrderStatus = "CLOSED") Then
                                            'Get all Results to export (all Results of the ORDER)
                                            myGlobalDataTO = myResultsDelegate.GetResultsToExport(dbConnection, pWorkSessionID, pAnalyzerID, mySampleClass, myOrderID)
                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                myResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)
                                            End If
                                        End If
                                        Exit Select

                                    Case (ExportType.ORDERTEST)
                                        'Continue only if the OrderTestStatus is CLOSED
                                        If (myOrderTestDS.twksOrderTests(0).OrderTestStatus = "CLOSED") Then
                                            'Get all results to export (the Result of the specific ORDER TEST)
                                            myGlobalDataTO = myResultsDelegate.GetResultsToExport(dbConnection, pWorkSessionID, pAnalyzerID, mySampleClass, "", myOrderTestDS.twksOrderTests(0).OrderTestID)
                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                                myResultsDS = DirectCast(myGlobalDataTO.SetDatos, ResultsDS)
                                            End If
                                        End If
                                        Exit Select
                                End Select
                                Exit Select
                        End Select
                    End If
                End If

                'Continue only if there is not errors and there are Results to export
                If (Not myGlobalDataTO.HasError AndAlso myResultsDS.vwksResults.Count > 0) Then
                    'Get the list of different Patient Identifiers in the group of results 
                    Dim diffPatientsList As List(Of String) = (From a In myResultsDS.vwksResults _
                                                          Where Not a.IsPatientIDNull _
                                                             Select a.PatientID Distinct).ToList

                    Dim myPatientsDS As PatientsDS
                    Dim myPatientDelegate As New PatientDelegate
                    Dim lstPatientResults As List(Of ResultsDS.vwksResultsRow)

                    For Each patientID As String In diffPatientsList
                        'Verify if it has been sent by LIMS (field ExternalPID is informed) to return results using the same ID for the Patient
                        myGlobalDataTO = myPatientDelegate.GetPatientData(dbConnection, patientID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myPatientsDS = DirectCast(myGlobalDataTO.SetDatos, PatientsDS)

                            If (myPatientsDS.tparPatients.Count > 0) Then
                                If (myPatientsDS.tparPatients.First.ExternalPID.ToString <> String.Empty) Then
                                    'Search all Results to export for this Patient to replace the PatientID for the ExternalPID
                                    lstPatientResults = (From b As ResultsDS.vwksResultsRow In myResultsDS.vwksResults _
                                                        Where b.PatientID = patientID _
                                                       Select b).ToList()

                                    For Each patientResult As ResultsDS.vwksResultsRow In lstPatientResults
                                        patientResult.BeginEdit()
                                        patientResult.PatientID = myPatientsDS.tparPatients.First.ExternalPID.ToString
                                        patientResult.EndEdit()
                                    Next
                                End If
                            End If
                        Else
                            'Error verifying if the PatientID exists in DB and has informed an External ID
                            Exit For
                        End If
                    Next
                    lstPatientResults = Nothing
                    diffPatientsList = Nothing

                    'Return the ResultsDS
                    myGlobalDataTO.SetDatos = myResultsDS
                End If

                '    For Each resultRow As ResultsDS.vwksResultsRow In myResultsDS.vwksResults.Rows
                '        'Mark the Result as Exported
                '        resultRow.ExportStatus = "SENT"
                '        resultRow.ExportDateTime = Now()

                '        'If there is a Patient result, verify if it has been sent by LIMS (field ExternalPID is informed) to return 
                '        'results using the same ID for the Patient
                '        If (Not resultRow.IsPatientIDNull) Then
                '            myGlobalDataTO = myPatientDelegate.GetPatientData(Nothing, resultRow.PatientID)
                '            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                '                myPatientsDS = DirectCast(myGlobalDataTO.SetDatos, PatientsDS)

                '                If (myPatientsDS.tparPatients.Count > 0) Then
                '                    If (myPatientsDS.tparPatients.First.ExternalPID.ToString <> String.Empty) Then
                '                        'PatientID is replaced by the LIMS PatientID
                '                        resultRow.PatientID = myPatientsDS.tparPatients.First.ExternalPID.ToString
                '                    End If
                '                End If
                '            End If
                '        Else
                '            resultRow.PatientID = String.Empty
                '        End If

                '        'If (Not resultRow.IsManualResultFlagNull AndAlso resultRow.ManualResultFlag) Then
                '        '    If (Not resultRow.IsManualResultNull) Then
                '        '        resultRow.CONC_Value = resultRow.ManualResult
                '        '    End If
                '        'End If
                '    Next

                '    myGlobalDataTO.SetDatos = myResultsDS

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.GetDataToExport", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO REVIEW - DELETE"
        '''' <summary>
        '''' Get all results to export to LIMS depending on the programmed frequency: After WS finished/By Patient/By OrderTestID
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">Work Session Identifier</param>
        '''' <param name="pOrderTestID">Order Test Identifier</param>
        '''' <param name="pExportType">Export Type (ENUM)</param>
        '''' <returns></returns>
        '''' <remarks>
        '''' Created by:  TR 13/05/2010
        '''' Modified by: AG 04/03/2011 - When the list of results to export is loaded, for Manual Results, inform field CONCValue if it
        ''''                              is Quantitative, and field QualitativeValue if it is Qualitative
        ''''              DL 18/05/2012 - Fields TestType, TestName, TubeType and MeasureUnit are get for each result, not for the informed
        ''''                              OrderTestID
        ''''              DL 21/05/2012 - When the list of results to export is loaded, inform N as SampleClass for Routine Patient Samples,
        ''''                              U as SampleClass for Stat Patient Samples, and Q as SampleClass for Controls. For Patient Samples, 
        ''''                              get the ExternalPatientID when it is informed (when the Patient has been imported from LIMS)
        ''''              AG 22/05/2012 - For Blanks and Calibrators, besides Patient Results for the TestID/SampleType, get also the Control Results
        ''''                            - Apply the same frequency criteria for Patient Samples and Controls 
        ''''              SA 18/06/2012 - From the first call to function GetOrderTest, get the Order SampleClass and the TestType, and then 
        ''''                              remove call to function ReadOrders in OrdersDelegate
        ''''                            - When Export Frequency is by OrderTest, remove call to function GetOrderTest, the needed data is already 
        ''''                              in the OrderTestsDS dataset filled before 
        '''' </remarks>
        'Private Function GetDataToExportOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '                                 ByVal pOrderTestID As Integer, ByVal pExportType As ExportType) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                Dim myTestType As String = String.Empty
        '                Dim myTestID As Integer = 0
        '                Dim mySampleType As String = String.Empty
        '                Dim myOrderID As String = String.Empty
        '                Dim mySampleClass As String = String.Empty
        '                Dim myResultsDS As New ResultsDS()
        '                Dim myOrderTestDS As New OrderTestsDS()
        '                Dim myResultsDelegate As New ResultsDelegate()
        '                Dim myOrderTestsDelegate As New OrderTestsDelegate()
        '                Dim myResultList As New List(Of ResultsDS.twksResultsRow)

        '                'VIEW Controls
        '                Dim myViewWSOrderTestDS As New ViewWSOrderTestsDS()
        '                Dim myViewWSOrderTestDAO As New vwksWSOrderTestsDAO()

        '                'Get the Order Test information
        '                myGlobalDataTO = myOrderTestsDelegate.GetOrderTest(dbConnection, pOrderTestID)
        '                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                    myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

        '                    'Set TestType, OrderID and SampleClass to local variables
        '                    If (myOrderTestDS.twksOrderTests.Rows.Count > 0) Then
        '                        myOrderID = myOrderTestDS.twksOrderTests(0).OrderID
        '                        mySampleClass = myOrderTestDS.twksOrderTests(0).SampleClass

        '                        myTestType = myOrderTestDS.twksOrderTests(0).TestType
        '                        myTestID = myOrderTestDS.twksOrderTests(0).TestID
        '                        mySampleType = myOrderTestDS.twksOrderTests(0).SampleType
        '                    End If

        '                    Select Case (mySampleClass)
        '                        Case "BLANK"
        '                            'TR 12/07/2012 -Validate the orderTeststatus before continue.
        '                            If Not myOrderTestDS.twksOrderTests(0).OrderTestStatus = "CLOSED" Then
        '                                'AG 22/05/2012 - Get ALL CONTROL OrderTests with this TestID on the WorkSession (only Standard Tests)
        '                                myGlobalDataTO = myViewWSOrderTestDAO.ReadByWorkSessionAndTestID(dbConnection, pWorkSessionID, myTestID, "CTRL", "", "CLOSED")
        '                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                    myViewWSOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, ViewWSOrderTestsDS)

        '                                    'For each CONTROL OrderTest returned, get all accepted Results
        '                                    For Each vWSOrderTRow As ViewWSOrderTestsDS.vwksWSOrderTestsRow In myViewWSOrderTestDS.vwksWSOrderTests
        '                                        myGlobalDataTO = myResultsDelegate.GetAcceptedResults(dbConnection, vWSOrderTRow.OrderTestID, , , vWSOrderTRow.TestType)

        '                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                            'Add all accepted Results for the OrderTestID
        '                                            For Each resultRow As ResultsDS.twksResultsRow In DirectCast(myGlobalDataTO.SetDatos, ResultsDS).twksResults.Rows
        '                                                myResultsDS.twksResults.ImportRow(resultRow)
        '                                            Next
        '                                        End If
        '                                    Next
        '                                End If
        '                                'AG 22/05/2012

        '                                'Get ALL PATIENT OrderTests with this TestID on the WorkSession (only Standard Tests)
        '                                myGlobalDataTO = myViewWSOrderTestDAO.ReadByWorkSessionAndTestID(dbConnection, pWorkSessionID, myTestID, "PATIENT", , "CLOSED")
        '                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                    myViewWSOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, ViewWSOrderTestsDS)

        '                                    'For each PATIENT OrderTest returned, get all accepted Results
        '                                    For Each vWSOrderTRow As ViewWSOrderTestsDS.vwksWSOrderTestsRow In myViewWSOrderTestDS.vwksWSOrderTests
        '                                        myGlobalDataTO = myResultsDelegate.GetAcceptedResults(dbConnection, vWSOrderTRow.OrderTestID, , , vWSOrderTRow.TestType)

        '                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                            'Add all accepted Results for the OrderTestID
        '                                            For Each resultRow As ResultsDS.twksResultsRow In DirectCast(myGlobalDataTO.SetDatos, ResultsDS).twksResults.Rows
        '                                                myResultsDS.twksResults.ImportRow(resultRow)
        '                                            Next
        '                                        End If
        '                                    Next
        '                                End If
        '                            End If

        '                            Exit Select

        '                        Case "CALIB"
        '                            'TR 12/07/2012 -Validate the orderTeststatus before continue.
        '                            If Not myOrderTestDS.twksOrderTests(0).OrderTestStatus = "CLOSED" Then
        '                                'AG 22/05/2012 - Get ALL CONTROL OrderTests with this TestID on the WorkSession (only Standard Tests)
        '                                myGlobalDataTO = myViewWSOrderTestDAO.ReadByWorkSessionAndTestID(dbConnection, pWorkSessionID, myTestID, "CTRL", mySampleType, "CLOSED")

        '                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                    myViewWSOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, ViewWSOrderTestsDS)

        '                                    'For each CONTROL OrderTest returned, get all accepted Results
        '                                    For Each vWSOrderTRow As ViewWSOrderTestsDS.vwksWSOrderTestsRow In myViewWSOrderTestDS.vwksWSOrderTests
        '                                        myGlobalDataTO = myResultsDelegate.GetAcceptedResults(dbConnection, vWSOrderTRow.OrderTestID, , , vWSOrderTRow.TestType)

        '                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                            'Add all accepted Results for the OrderTestID
        '                                            For Each resultRow As ResultsDS.twksResultsRow In DirectCast(myGlobalDataTO.SetDatos, ResultsDS).twksResults.Rows
        '                                                myResultsDS.twksResults.ImportRow(resultRow)
        '                                            Next
        '                                        End If
        '                                    Next
        '                                End If
        '                                'AG 22/05/2012

        '                                'Get ALL PATIENT OrderTests with this TestID on the WorkSession (only Standard Tests)
        '                                myGlobalDataTO = myViewWSOrderTestDAO.ReadByWorkSessionAndTestID(dbConnection, pWorkSessionID, myTestID, "PATIENT", mySampleType, "CLOSED")

        '                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                    myViewWSOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, ViewWSOrderTestsDS)

        '                                    'For each PATIENT OrderTest returned, get all accepted Results
        '                                    For Each vWSOrderTRow As ViewWSOrderTestsDS.vwksWSOrderTestsRow In myViewWSOrderTestDS.vwksWSOrderTests
        '                                        myGlobalDataTO = myResultsDelegate.GetAcceptedResults(dbConnection, vWSOrderTRow.OrderTestID, , , vWSOrderTRow.TestType)

        '                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                            'Add all accepted Results for the OrderTestID
        '                                            For Each resultRow As ResultsDS.twksResultsRow In DirectCast(myGlobalDataTO.SetDatos, ResultsDS).twksResults.Rows
        '                                                myResultsDS.twksResults.ImportRow(resultRow)
        '                                            Next
        '                                        End If
        '                                    Next
        '                                End If
        '                            End If

        '                            Exit Select

        '                            'DL 26/06/2012 
        '                        Case "ISE"

        '                            'AG 22/05/2012 - Apply the same frequency criteria for patients and controls
        '                        Case "CTRL", "PATIENT"
        '                            Select Case (pExportType)
        '                                Case (ExportType.ALL)
        '                                    'Get all CLOSED Order Tests (all TestTypes) for the specified WorkSession and SampleClass (CTRL or PATIENT)
        '                                    myGlobalDataTO = myViewWSOrderTestDAO.ReadByWorkSessionAndSampleClass(dbConnection, pWorkSessionID, mySampleClass, "CLOSED")
        '                                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                        Dim myTempViewOrderTestsDS As ViewWSOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, ViewWSOrderTestsDS)

        '                                        'For each OrderTest returned, get all accepted Results
        '                                        For Each wsOrderTestRow As ViewWSOrderTestsDS.vwksWSOrderTestsRow In myTempViewOrderTestsDS.vwksWSOrderTests.Rows
        '                                            myGlobalDataTO = myResultsDelegate.GetAcceptedResults(dbConnection, wsOrderTestRow.OrderTestID, , , wsOrderTestRow.TestType)

        '                                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                                'Add all accepted Results for the OrderTestID
        '                                                For Each resultRow As ResultsDS.twksResultsRow In DirectCast(myGlobalDataTO.SetDatos, ResultsDS).twksResults.Rows
        '                                                    myResultsDS.twksResults.ImportRow(resultRow)
        '                                                Next
        '                                            End If
        '                                        Next
        '                                        myGlobalDataTO.SetDatos = myResultsDS
        '                                    End If
        '                                    Exit Select

        '                                Case (ExportType.ORDER)
        '                                    'TR 12/07/2012 -Validate the orderstatus before continue.
        '                                    If myOrderTestDS.twksOrderTests(0).OrderStatus = "CLOSED" Then
        '                                        'Get all CLOSED Order Tests (all TestTypes) in the ORDER 
        '                                        myGlobalDataTO = myOrderTestsDelegate.GetOrderTestByOrderID(dbConnection, myOrderID, "CLOSED")
        '                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                            myOrderTestDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

        '                                            'For each OrderTest returned, get all accepted Results
        '                                            For Each orderTestRow As OrderTestsDS.twksOrderTestsRow In myOrderTestDS.twksOrderTests
        '                                                myGlobalDataTO = myResultsDelegate.GetAcceptedResults(dbConnection, orderTestRow.OrderTestID, , , orderTestRow.TestType)

        '                                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                                    'Add all accepted Results for the OrderTestID
        '                                                    For Each resultRow As ResultsDS.twksResultsRow In DirectCast(myGlobalDataTO.SetDatos, ResultsDS).twksResults.Rows
        '                                                        myResultsDS.twksResults.ImportRow(resultRow)
        '                                                    Next
        '                                                End If
        '                                            Next
        '                                            myGlobalDataTO.SetDatos = myResultsDS
        '                                        End If
        '                                    End If

        '                                    Exit Select

        '                                Case (ExportType.ORDERTEST)
        '                                    'TR 12/07/2012 -Validate the orderTeststatus before continue.
        '                                    If myOrderTestDS.twksOrderTests(0).OrderTestStatus = "CLOSED" Then
        '                                        'Get all accepted Results for the OrderTest
        '                                        myGlobalDataTO = myResultsDelegate.GetAcceptedResults(dbConnection, pOrderTestID, , , myTestType)
        '                                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                            'Add all accepted Results for the OrderTestID
        '                                            For Each resultRow As ResultsDS.twksResultsRow In DirectCast(myGlobalDataTO.SetDatos, ResultsDS).twksResults.Rows
        '                                                myResultsDS.twksResults.ImportRow(resultRow)
        '                                            Next
        '                                            myGlobalDataTO.SetDatos = myResultsDS
        '                                        End If
        '                                    End If

        '                                    Exit Select
        '                            End Select
        '                            Exit Select
        '                    End Select
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'Get only Results with ExportStatus <> SENT
        '                    myResultList = (From a In myResultsDS.twksResults _
        '                                    Where Not String.Equals(a.ExportStatus, "SENT") _
        '                                    Select a).ToList()
        '                    myGlobalDataTO.SetDatos = myResultList

        '                    'DL 21/05/2012
        '                    Dim myLimsResultTO As New LimsResultTO()
        '                    Dim MyLimsResultList As New List(Of LimsResultTO)

        '                    If (myResultList.Count > 0) Then
        '                        'Go throught each result to create the Lims result TO and add it to the list
        '                        For Each resultRow As ResultsDS.twksResultsRow In myResultList
        '                            myLimsResultTO.OrderTestID = resultRow.OrderTestID

        '                            'DL 21/05/2012
        '                            Select Case resultRow.TestType ' resultRow.SampleClass DL 27/06/2012
        '                                'Case "PATIENT"
        '                                Case "STD", "OFFS"
        '                                    myLimsResultTO.SampleClass = If(resultRow.StatFlag, "U", "N")

        '                                Case "ISE"
        '                                    myLimsResultTO.SampleClass = If(resultRow.StatFlag, "U", "N")
        '                                    'myLimsResultTO.SampleClass = "Q"

        '                                Case "CTRL"
        '                                    myLimsResultTO.SampleClass = "Q"

        '                            End Select

        '                            myLimsResultTO.MultiPointNumber = resultRow.MultiPointNumber
        '                            myLimsResultTO.RerunNumber = resultRow.RerunNumber
        '                            myLimsResultTO.TestType = resultRow.TestType
        '                            myLimsResultTO.TestName = resultRow.TestName
        '                            myLimsResultTO.SampleType = resultRow.SampleType
        '                            myLimsResultTO.TubeType = If(Not resultRow.IsTubeTypeNull, resultRow.TubeType, Space(3))
        '                            myLimsResultTO.Units = resultRow.MeasureUnit

        '                            If (Not resultRow.IsPatientIDNull) Then
        '                                myLimsResultTO.ExternalPatientID = resultRow.PatientID

        '                                Dim myPatientDelegate As New PatientDelegate
        '                                myGlobalDataTO = myPatientDelegate.GetPatientData(dbConnection, resultRow.PatientID)

        '                                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                                    Dim myPatientsDS As PatientsDS = DirectCast(myGlobalDataTO.SetDatos, PatientsDS)

        '                                    If (myPatientsDS.tparPatients.Count > 0) Then
        '                                        If (Not myPatientsDS.tparPatients.First.ExternalPID.ToString Is String.Empty) Then
        '                                            myLimsResultTO.ExternalPatientID = myPatientsDS.tparPatients.First.ExternalPID.ToString
        '                                        End If
        '                                    End If
        '                                End If

        '                            ElseIf (Not resultRow.IsSampleIDNull) Then
        '                                myLimsResultTO.ExternalPatientID = resultRow.SampleID
        '                            Else
        '                                myLimsResultTO.ExternalPatientID = String.Empty
        '                            End If
        '                            'DL 21/05/2012

        '                            'AG 04/03/2011
        '                            myLimsResultTO.QualitativeValue = String.Empty
        '                            If (Not resultRow.ManualResultFlag) Then
        '                                If Not resultRow.IsCONC_ValueNull Then myLimsResultTO.CONCValue = resultRow.CONC_Value
        '                            Else
        '                                If (Not resultRow.IsManualResultNull) Then
        '                                    myLimsResultTO.CONCValue = resultRow.ManualResult
        '                                ElseIf (Not resultRow.IsManualResultTextNull) Then
        '                                    myLimsResultTO.QualitativeValue = resultRow.ManualResultText
        '                                End If
        '                            End If

        '                            'AG 04/03/2011
        '                            myLimsResultTO.ExportDate = resultRow.ResultDateTime  'DateTime.Now
        '                            MyLimsResultList.Add(myLimsResultTO)
        '                            myLimsResultTO = New LimsResultTO()
        '                        Next
        '                    End If

        '                    myGlobalDataTO.SetDatos = MyLimsResultList
        '                    'DL 21/05/2012
        '                End If

        '                If (Not myGlobalDataTO.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.GetDataToExport", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' PENDING FOR DEFINITION
        '''' Method just create a txt file with temporal structure.
        '''' </summary>
        '''' <param name="pLimsResultsList"></param>
        '''' <returns></returns>
        '''' <remarks>
        '''' CREATE BY: TR 14/05/2010
        '''' MODIFIED BY AG 21/09/2010
        '''' AG 21/05/2012 - add parameters connection and resetflag and use correct template
        '''' </remarks>
        'Private Function CreateExportFile(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                  ByVal pResetWSFlag As Boolean, _
        '                                  ByVal pLimsResultsList As List(Of LimsResultTO), _
        '                                  ByVal pWorkSessionID As String) As GlobalDataTO

        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim TextFileWriter As StreamWriter
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        If pLimsResultsList.Count > 0 Then 'dl 31/05/2012

        '            myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)

        '            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '                If (Not dbConnection Is Nothing) Then
        '                    Dim TextFileReader As StreamReader
        '                    Dim TextFileData As String

        '                    'SGM 08/03/11 Get from SWParameters table
        '                    Dim myExportFolder As String
        '                    Dim myParams As New SwParametersDelegate

        '                    myGlobalDataTO = myParams.ReadTextValueByParameterName(dbConnection, GlobalEnumerates.SwParameters.LIMS_EXPORT_PATH.ToString, Nothing) 'nothing
        '                    If Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing Then
        '                        myExportFolder = CStr(myGlobalDataTO.SetDatos)
        '                    Else
        '                        myExportFolder = "\Export\"
        '                    End If

        '                    If myExportFolder.StartsWith("\") And Not myExportFolder.StartsWith("\\") Then
        '                        myExportFolder = Application.StartupPath & myExportFolder
        '                    End If


        '                    'DL 21/05/2012
        '                    Dim WSStartDateTime As String = String.Empty

        '                    'Get WSStartDateTime from DB
        '                    Dim myWSDelegate As New WorkSessionsDelegate
        '                    Dim resultData As New GlobalDataTO

        '                    resultData = myWSDelegate.GetByWorkSession(dbConnection, pWorkSessionID)

        '                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                        Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
        '                        If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsWSDateTimeNull) Then
        '                            WSStartDateTime = myWSDataDS.twksWorkSessions.First().WSDateTime.ToString
        '                            'WSStartDateTime = myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(DatePattern) & " " & _
        '                            'myWSDataDS.twksWorkSessions.First().StartDateTime.ToString(TimePattern)
        '                        End If
        '                    End If

        '                    Dim myExportFile As String = GlobalConstants.LIMS_EXPORT_FILE_NAME & " (" & _
        '                                                 CDate(WSStartDateTime).ToString("yy-MM-dd HH:mm").ToString.Replace(":", "-").ToString & ")_"
        '                    Dim fileId As Integer = 1

        '                    'DL 31/05/2012 Get last id by current work session

        '                    'Make a Reference to a Folder
        '                    'Validate if not exists the folder
        '                    If Not Directory.Exists(myExportFolder) And Not myExportFolder.Trim = "" Then
        '                        Directory.CreateDirectory(myExportFolder)
        '                    End If

        '                    Dim di As New IO.DirectoryInfo(myExportFolder)
        '                    Dim diar1 As IO.FileInfo() = di.GetFiles(myExportFile & "*.txt")
        '                    Dim dra As IO.FileInfo
        '                    Dim myValue As String
        '                    Dim myLength As Integer
        '                    'Dim myfile As IO.FileInfo

        '                    For Each dra In diar1
        '                        'myfile = New FileInfo(dra.FullName)

        '                        myLength = (dra.ToString.Length - 4) - 24
        '                        myValue = dra.ToString.Substring(24, myLength)

        '                        If IsNumeric(myValue) AndAlso fileId < CInt(myValue) Then fileId = CInt(myValue)

        '                        dra = Nothing
        '                    Next dra

        '                    di = Nothing
        '                    diar1 = Nothing
        '                    dra = Nothing
        '                    'myfile = Nothing

        '                    'DL 31/05/2012
        '                    myExportFile &= fileId.ToString & ".txt"

        '                    'Dim myExportFile As String = GlobalConstants.LIMS_EXPORT_FILE_NAME & " (" & pWorkSessionID & ")_" & fileId.ToString & ".txt"
        '                    '                        WSStartDateTime = WSStartDateTime.Replace("/", "-")
        '                    '                        WSStartDateTime = WSStartDateTime.Replace(":", "-")

        '                    'Dim myExportFile As String = GlobalConstants.LIMS_EXPORT_FILE_NAME & " (" & _
        '                    '                             CDate(WSStartDateTime).ToString("yy-MM-dd HH:mm").ToString.Replace(":", "-").ToString & ")_" & _
        '                    '                             fileId.ToString & _
        '                    '                             ".txt"

        '                    'DL 21/05/2012

        '                    Dim canWrite As Boolean = True
        '                    'validate path exist.
        '                    If Not File.Exists(myExportFolder & myExportFile) Then
        '                        'AG 20/09/2010 - validate if not exists the folder
        '                        If Not Directory.Exists(myExportFolder) And _
        '                        Not myExportFolder.Trim = "" Then
        '                            Directory.CreateDirectory(myExportFolder)
        '                        End If
        '                        'END AG 20/09/2010

        '                        'if the file do not exist then create a new one.
        '                        TextFileWriter = New StreamWriter(myExportFolder & myExportFile)

        '                    Else
        '                        'if the file exits then load it into a stream variable.
        '                        TextFileReader = New StreamReader(myExportFolder & myExportFile)
        '                        'Copy all the lines into a text variable.
        '                        TextFileData = TextFileReader.ReadToEnd.TrimEnd()
        '                        'Close the file to avoid error.
        '                        TextFileReader.Close()

        '                        'DL 31/05/2012
        '                        'Gets if the current stream supports writing.
        '                        Try
        '                            Dim fs As New FileStream(myExportFolder & myExportFile, FileMode.OpenOrCreate, FileAccess.Write)
        '                            If Not fs.CanWrite Then canWrite = False
        '                            fs.Close()

        '                        Catch ex As Exception
        '                            canWrite = False
        '                        End Try

        '                        'end gets if the current stream supports writing.

        '                        If Not canWrite Then
        '                            fileId += 1

        '                            myExportFile = GlobalConstants.LIMS_EXPORT_FILE_NAME & _
        '                                            " (" & _
        '                                            CDate(WSStartDateTime).ToString("yy-MM-dd HH:mm").ToString.Replace(":", "-").ToString & ")_" & _
        '                                            fileId.ToString & _
        '                                            ".txt"
        '                        End If

        '                        'DL 31/05/2012

        '                        'Set the file to a Stream Writer
        '                        TextFileWriter = New StreamWriter(myExportFolder & myExportFile)

        '                        'if there are any data on the variable then load into the Stream writer variable.
        '                        If TextFileData.ToString.Length > 0 Then
        '                            TextFileWriter.Write(TextFileData.ToString() & Environment.NewLine)
        '                        End If
        '                    End If

        '                    'go throug each result and add it into our Stream writer variable
        '                    Dim myResultValue As String = ""
        '                    For Each myResultLimsTO As LimsResultTO In pLimsResultsList

        '                        myResultValue = CStr(IIf(myResultLimsTO.QualitativeValue = "", _
        '                                                 myResultLimsTO.CONCValue.ToString, _
        '                                                 myResultLimsTO.QualitativeValue.ToString))

        '                        'DL 21/05/2012
        '                        'DL 27/06/2012
        '                        Dim mySampleClass As String
        '                        If myResultLimsTO.SampleClass Is Nothing Then
        '                            mySampleClass = Space(1)
        '                        Else
        '                            mySampleClass = String.Format("{0,-1}", myResultLimsTO.SampleClass.ToString)
        '                        End If
        '                        'DL 27/06/2012

        '                        TextFileWriter.WriteLine(mySampleClass & vbTab & _
        '                                                 CStr(IIf(myResultLimsTO.ExternalPatientID Is Nothing, Space(28), String.Format("{0,-28}", myResultLimsTO.ExternalPatientID.ToString))) & vbTab & _
        '                                                 CStr(IIf(myResultLimsTO.SampleType Is Nothing, Space(3), String.Format("{0,-3}", myResultLimsTO.SampleType.ToString))) & vbTab & _
        '                                                 CStr(IIf(myResultLimsTO.TubeType Is Nothing, Space(3), String.Format("{0,-3}", myResultLimsTO.TubeType.ToString))) & vbTab & _
        '                                                 CStr(IIf(myResultLimsTO.TestName Is Nothing, Space(16), String.Format("{0,-16}", myResultLimsTO.TestName.ToString))) & vbTab & _
        '                                                 CStr(IIf(myResultLimsTO.TestType Is Nothing, Space(4), String.Format("{0,-4}", myResultLimsTO.TestType.ToString))) & vbTab & _
        '                                                 String.Format("{0,-20}", myResultValue.ToString) & vbTab & _
        '                                                 CStr(IIf(myResultLimsTO.Units Is Nothing, Space(10), String.Format("{0,-10}", myResultLimsTO.Units.ToString))) & vbTab & _
        '                                                 String.Format("{0,-19}", myResultLimsTO.ExportDate.ToString()))

        '                        'TextFileWriter.WriteLine(myResultLimsTO.OrderTestID & " " & _
        '                        '     myResultValue & " " & myResultLimsTO.ExportDate.ToString() & " " _
        '                        '     & " JUST TESTING PENDING DEFINITION")
        '                        'DL 21/05/2012
        '                    Next myResultLimsTO
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.CreateExportFile", EventLogEntryType.Error, False)
        '    Finally
        '        'close the file 
        '        If Not TextFileWriter Is Nothing Then TextFileWriter.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        'Public Function ExportToLISManual(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO

        '    Try
        '        Dim myResultsDelegate As New ResultsDelegate
        '        resultData = myResultsDelegate.GetResultsByLIS(Nothing, pAnalyzerID, pWorkSessionID)

        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            Dim ResultsData As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

        '            Dim Results As List(Of ResultsDS.vwksResultsRow) = (From row In ResultsData.vwksResults _
        '                                                                Where String.Equals(row.ValidationStatus, "OK") _
        '                                                                Order By row.ResultDateTime _
        '                                                                Select row).ToList

        '            If Results.Count > 0 Then

        '                resultData = DAOBase.GetOpenDBConnection(Nothing)

        '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                    Dim dbConnection As New SqlClient.SqlConnection
        '                    Dim TextFileWriter As StreamWriter
        '                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '                    If (Not dbConnection Is Nothing) Then
        '                        Dim TextFileReader As StreamReader
        '                        Dim TextFileData As String

        '                        'Get from SWParameters table
        '                        Dim myExportFolder As String
        '                        Dim myParams As New SwParametersDelegate

        '                        resultData = myParams.ReadTextValueByParameterName(dbConnection, GlobalEnumerates.SwParameters.LIMS_EXPORT_PATH.ToString, Nothing) 'nothing
        '                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
        '                            myExportFolder = CStr(resultData.SetDatos)
        '                        Else
        '                            myExportFolder = "\Export\"
        '                        End If

        '                        If myExportFolder.StartsWith("\") And Not myExportFolder.StartsWith("\\") Then
        '                            myExportFolder = Application.StartupPath & myExportFolder
        '                        End If

        '                        Dim WSStartDateTime As String = String.Empty
        '                        Dim myWSDelegate As New WorkSessionsDelegate                        'Get WSStartDateTime from DB

        '                        resultData = myWSDelegate.GetByWorkSession(dbConnection, pWorkSessionID)

        '                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                            Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
        '                            If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsWSDateTimeNull) Then
        '                                WSStartDateTime = myWSDataDS.twksWorkSessions.First().WSDateTime.ToString
        '                            End If
        '                        End If

        '                        Dim myExportFile As String = GlobalConstants.LIMS_EXPORT_FILE_NAME & " (" & _
        '                                                     CDate(WSStartDateTime).ToString("yy-MM-dd HH:mm").ToString.Replace(":", "-").ToString & ")_"
        '                        Dim fileId As Integer = 1

        '                        'Make a Reference to a Folder. Validate if not exists the folder
        '                        If Not Directory.Exists(myExportFolder) And Not String.Equals(myExportFolder.Trim, String.Empty) Then Directory.CreateDirectory(myExportFolder)

        '                        Dim di As New IO.DirectoryInfo(myExportFolder)
        '                        Dim diar1 As IO.FileInfo() = di.GetFiles(myExportFile & "*.txt")
        '                        Dim dra As IO.FileInfo
        '                        Dim myValue As String
        '                        Dim myLength As Integer

        '                        For Each dra In diar1
        '                            myLength = (dra.ToString.Length - 4) - 24
        '                            myValue = dra.ToString.Substring(24, myLength)

        '                            If IsNumeric(myValue) AndAlso fileId < CInt(myValue) Then fileId = CInt(myValue)

        '                            dra = Nothing
        '                        Next dra

        '                        di = Nothing
        '                        diar1 = Nothing
        '                        dra = Nothing

        '                        myExportFile &= fileId.ToString & ".txt"

        '                        Dim canWrite As Boolean = True

        '                        If Not File.Exists(myExportFolder & myExportFile) Then 'validate path exist.
        '                            'Validate if not exists the folder
        '                            If Not Directory.Exists(myExportFolder) And Not String.Equals(myExportFolder.Trim, "") Then Directory.CreateDirectory(myExportFolder)

        '                            'if the file do not exist then create a new one.
        '                            TextFileWriter = New StreamWriter(myExportFolder & myExportFile)
        '                        Else
        '                            TextFileReader = New StreamReader(myExportFolder & myExportFile)  'if the file exits then load it into a stream variable.
        '                            TextFileData = TextFileReader.ReadToEnd.TrimEnd()                 'Copy all the lines into a text variable.
        '                            TextFileReader.Close()                                            'Close the file to avoid error.

        '                            Try  'Gets if the current stream supports writing.
        '                                Dim fs As New FileStream(myExportFolder & myExportFile, FileMode.OpenOrCreate, FileAccess.Write)
        '                                If Not fs.CanWrite Then canWrite = False
        '                                fs.Close()

        '                            Catch ex As Exception
        '                                canWrite = False
        '                            End Try  'End gets if the current stream supports writing.

        '                            If Not canWrite Then
        '                                fileId += 1

        '                                myExportFile = GlobalConstants.LIMS_EXPORT_FILE_NAME & " (" & _
        '                                                CDate(WSStartDateTime).ToString("yy-MM-dd HH:mm").ToString.Replace(":", "-").ToString & ")_" & _
        '                                                fileId.ToString & ".txt"
        '                            End If

        '                            TextFileWriter = New StreamWriter(myExportFolder & myExportFile)    'Set the file to a Stream Writer
        '                            'if there are any data on the variable then load into the Stream writer variable.
        '                            If TextFileData.ToString.Length > 0 Then TextFileWriter.Write(TextFileData.ToString() & Environment.NewLine)

        '                        End If

        '                        'go throug each result and add it into our Stream writer variable
        '                        Dim myResultValue As String = ""
        '                        Dim myTubeType As String
        '                        Dim mySampleClass As String
        '                        Dim myOrderTestDelegate As New OrderTestsDelegate
        '                        Dim myResultRow As ResultsDS.twksResultsRow
        '                        Dim myOrderTestDS As New OrderTestsDS
        '                        Dim tempResultDS As New ResultsDS
        '                        Dim myExecutionsRow As ExecutionsDS.twksWSExecutionsRow
        '                        Dim myGlobalBase As New GlobalBase
        '                        Dim myExecutionsDS As New ExecutionsDS
        '                        Dim myExecutionDelegate As New ExecutionsDelegate

        '                        For i As Integer = 0 To Results.Count - 1
        '                            If Not Results(i).IsCONC_ValueNull Then
        '                                myResultValue = CStr(Results(i).CONC_Value)
        '                            ElseIf Not Results(i).IsABSValueNull Then
        '                                myResultValue = CStr(Results(i).ABSValue)
        '                            Else
        '                                myResultValue = ""
        '                            End If

        '                            Select Case Results(i).TestType
        '                                Case "STD", "OFFS"
        '                                    If Results(i).StatFlag Then
        '                                        mySampleClass = "U"
        '                                    Else
        '                                        mySampleClass = "N"
        '                                    End If

        '                                Case "ISE"
        '                                    If Results(i).SampleClass = "CTRL" Then
        '                                        mySampleClass = "Q"
        '                                    Else
        '                                        If Results(i).StatFlag Then
        '                                            mySampleClass = "U"
        '                                        Else
        '                                            mySampleClass = "N"
        '                                        End If
        '                                    End If

        '                                Case "CTRL"
        '                                    mySampleClass = "Q"

        '                            End Select

        '                            myTubeType = Space(3)

        '                            resultData = myOrderTestDelegate.GetOrderTest(dbConnection, Results(i).OrderTestID)
        '                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                myOrderTestDS = DirectCast(resultData.SetDatos, OrderTestsDS)

        '                                If Not myOrderTestDS Is Nothing AndAlso myOrderTestDS.twksOrderTests.Rows.Count > 0 Then
        '                                    myTubeType = String.Format("{0,-3}", myOrderTestDS.twksOrderTests.First.TubeType)
        '                                End If
        '                            End If

        '                            TextFileWriter.WriteLine(mySampleClass & vbTab & _
        '                                                     CStr(IIf(Results(i).IsPatientIDNull, Space(28), String.Format("{0,-28}", Results(i).PatientID))) & vbTab & _
        '                                                     CStr(IIf(Results(i).IsSampleTypeNull, Space(3), String.Format("{0,-3}", Results(i).SampleType))) & vbTab & _
        '                                                     myTubeType & vbTab & _
        '                                                     CStr(IIf(Results(i).IsTestNameNull, Space(16), String.Format("{0,-16}", Results(i).TestName))) & vbTab & _
        '                                                     CStr(IIf(Results(i).IsTestTypeNull, Space(4), String.Format("{0,-4}", Results(i).TestType))) & vbTab & _
        '                                                     String.Format("{0,-20}", myResultValue.ToString) & vbTab & _
        '                                                     CStr(IIf(Results(i).IsMeasureUnitNull, Space(10), String.Format("{0,-10}", Results(i).MeasureUnit))) & vbTab & _
        '                                                     String.Format("{0,-19}", Results(i).ResultDateTime))


        '                            myResultRow = tempResultDS.twksResults.NewtwksResultsRow()
        '                            myResultRow.ExportStatus = "SENT"
        '                            myResultRow.ExportDateTime = DateTime.Now
        '                            myResultRow.TS_User = GlobalBase.GetSessionInfo().UserName
        '                            myResultRow.TS_DateTime = DateTime.Now
        '                            myResultRow.OrderTestID = Results(i).OrderTestID
        '                            myResultRow.MultiPointNumber = Results(i).MultiPointNumber
        '                            myResultRow.RerunNumber = Results(i).RerunNumber

        '                            tempResultDS.twksResults.AddtwksResultsRow(myResultRow)
        '                            resultData = myResultsDelegate.UpdateExportStatus(dbConnection, tempResultDS)

        '                            If Not resultData.HasError Then
        '                                'clear the table to continue.
        '                                tempResultDS.twksResults.Clear()

        '                                myExecutionsRow = myExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow
        '                                myExecutionsRow.OrderTestID = Results(i).OrderTestID

        '                                resultData = myExecutionDelegate.GetByOrderTest(dbConnection, pWorkSessionID, pAnalyzerID, Results(i).OrderTestID)

        '                                If Not resultData.HasError Then
        '                                    If DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions.Count > 0 AndAlso _
        '                                       Not DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions(0).IsExecutionIDNull Then
        '                                        myExecutionsRow.ExecutionID = DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions(0).ExecutionID
        '                                        'Add Execution Row.
        '                                        myExecutionsDS.twksWSExecutions.AddtwksWSExecutionsRow(myExecutionsRow)
        '                                    End If
        '                                End If

        '                            Else
        '                                Exit For
        '                            End If

        '                        Next i

        '                        TextFileWriter.Close()

        '                        If Not resultData Is Nothing AndAlso Not resultData.HasError Then resultData.SetDatos = myExecutionsDS

        '                    End If
        '                End If

        '            End If

        '        End If

        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportToLIS", EventLogEntryType.Error, False)

        '    End Try

        '    Return resultData

        'End Function

        '''' <summary>
        '''' Main method, checks the programmed frequency and launch the exportation when needed
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pAnalyzerID"></param>
        '''' <param name="pWorkSessionID">WorkSession ID</param>
        '''' <param name="pOrderTestID">Order Test ID</param>
        '''' <param name="pManualExportFlag"></param>
        '''' <param name="pResetWSFlag" ></param>
        '''' <param name="pSampleClass"></param> 
        '''' <param name="pTestType"></param> 
        '''' <param name="pControlNumber"></param> 
        '''' <param name="pPatientID" ></param>
        '''' <returns>GlobalDataTo with DS as ExecutionsDS with ordertestID DISTINCT exported</returns>
        '''' <remarks>
        '''' CREATE BY: TR 12/05/2010
        '''' Modified by: DL 17/03/2011 add sampleclass, testtype and controlnumber
        '''' Modified by: AG 21/05/2012 - add parameter pResetWSFlag
        '''' </remarks>
        'Public Function ManageLISExportation(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                     ByVal pAnalyzerID As String, _
        '                                     ByVal pWorkSessionID As String, _
        '                                     ByVal pOrderTestID As Integer, _
        '                                     ByVal pManualExportFlag As Boolean, _
        '                                     ByVal pResetWSFlag As Boolean, _
        '                                     Optional ByVal pSampleClass As String = "", _
        '                                     Optional ByVal pTestType As String = "", _
        '                                     Optional ByVal pControlNumber As Integer = 0, _
        '                                     Optional ByVal pPatientID As String = "") As GlobalDataTO

        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim myStatus As String = ""
        '    Dim myExportType As New ExportType
        '    Dim exportedExecutionsDS As New ExecutionsDS

        '    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '    Dim StartTime As DateTime = Now
        '    Dim myLogAcciones1 As New ApplicationLogManager()
        '    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myExportFrecuency As String = ""
        '                Dim myAutomaticExport As Boolean = False
        '                Dim myUserSettingDelegate As New UserSettingsDelegate()

        '                'Get the exportatio Type Manual/Automatic
        '                myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.AUTOMATIC_EXPORT.ToString) '"AUTOMATIC_EXPORT")
        '                If Not myGlobalDataTO.HasError Then
        '                    If CType(myGlobalDataTO.SetDatos, Integer) > 0 Then
        '                        myAutomaticExport = True
        '                    End If
        '                End If
        '                'If the automaticExport is true then get the export frequency.
        '                If myAutomaticExport Then
        '                    'Get the programmed export frequency.
        '                    myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.EXPORT_FREQUENCY.ToString) '"EXPORT_FREQUENCY")
        '                    If Not myGlobalDataTO.HasError Then
        '                        'set the found value to our local variable.
        '                        myExportFrecuency = myGlobalDataTO.SetDatos.ToString()
        '                    End If
        '                End If

        '                'If pManualExportFlag = True Then
        '                'AG 23/05/2011 - if no automatic exportation do not export
        '                'myGlobalDataTO = ExecuteExportation(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, ExportType.ORDER, _
        '                '                                    pManualExportFlag, pResetWSFlag, pSampleClass, pTestType, pControlNumber)
        '                'myGlobalDataTO = ExportToLISManual(pAnalyzerID, pWorkSessionID)

        '                'If error then send a empty execution dataset.
        '                ' If myGlobalDataTO.HasError Then
        '                '      myGlobalDataTO.SetDatos = New ExecutionsDS
        '                '   End If

        '                'Else
        '                If myAutomaticExport Then
        '                    'Validate the Export Frequency to determinate witch method is implemented.
        '                    Select Case myExportFrecuency

        '                        Case "END_WS" 'WORKSESSION
        '                            'Get the worksession status
        '                            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate()
        '                            'Get Worksession values.
        '                            myGlobalDataTO = myWSAnalyzersDelegate.ReadWSAnalyzers(dbConnection, pAnalyzerID, pWorkSessionID)
        '                            If Not myGlobalDataTO.HasError Then
        '                                Dim myWSAnalyzersDS As New WSAnalyzersDS
        '                                myWSAnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS)
        '                                If myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0 Then
        '                                    'validate if the worksession status is CLOSE.
        '                                    myStatus = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus
        '                                    myExportType = ExportType.ALL
        '                                End If
        '                            End If

        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                            GlobalBase.CreateLogActivity("ManageLISExportation END WS Export Type Automatic (Complete): " & _
        '                                                            Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
        '                                                            "ExportDelegate.ManageLISExportation", EventLogEntryType.Information, False)
        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                            Exit Select

        '                        Case "ORDER" ' ORDER
        '                            Dim myOrderID As String = ""
        '                            Dim myOrdersDelegate As New OrdersDelegate
        '                            Dim myOrderTestsDelegate As New OrderTestsDelegate()

        '                            'Get the order test value to get the order id
        '                            myGlobalDataTO = myOrderTestsDelegate.GetOrderTest(dbConnection, pOrderTestID)
        '                            If Not myGlobalDataTO.HasError Then
        '                                Dim myOrderTestsDS As New OrderTestsDS
        '                                myOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)
        '                                If myOrderTestsDS.twksOrderTests.Rows.Count > 0 Then
        '                                    myOrderID = myOrderTestsDS.twksOrderTests(0).OrderID
        '                                End If
        '                            End If

        '                            If Not String.Equals(myOrderID, String.Empty) Then
        '                                'get the order to set the status to the local variable.
        '                                myGlobalDataTO = myOrdersDelegate.ReadOrders(dbConnection, myOrderID)
        '                                If Not myGlobalDataTO.HasError Then
        '                                    Dim myOrdersDS As New OrdersDS()
        '                                    myOrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)
        '                                    If myOrdersDS.twksOrders.Rows.Count > 0 Then
        '                                        myStatus = myOrdersDS.twksOrders(0).OrderStatus
        '                                        myExportType = ExportType.ORDER
        '                                    End If
        '                                End If
        '                            End If

        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                            GlobalBase.CreateLogActivity("ManageLISExportation ORDER Export Type Automatic (Complete): " & _
        '                                                            Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
        '                                                            "ExportDelegate.ManageLISExportation", EventLogEntryType.Information, False)
        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        '                            Exit Select

        '                        Case "ORDERTEST" 'ORDERTEST
        '                            Dim myOrderTestsDelegate As New OrderTestsDelegate()
        '                            myGlobalDataTO = myOrderTestsDelegate.GetOrderTest(dbConnection, pOrderTestID)  'Get the order test to deteminate the status.
        '                            If Not myGlobalDataTO.HasError Then
        '                                Dim myOrderTestsDS As New OrderTestsDS
        '                                myOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)
        '                                If myOrderTestsDS.twksOrderTests.Rows.Count > 0 Then
        '                                    myStatus = myOrderTestsDS.twksOrderTests(0).OrderTestStatus
        '                                    myExportType = ExportType.ORDERTEST
        '                                End If
        '                            End If

        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                            GlobalBase.CreateLogActivity("ManageLISExportation ORDER TEST Export Type Automatic (Complete): " & _
        '                                                            Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
        '                                                            "ExportDelegate.ManageLISExportation", EventLogEntryType.Information, False)
        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        '                            Exit Select
        '                    End Select

        '                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                    StartTime = Now
        '                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                    'validate if the status is close to start the process 

        '                    If String.Equals(myStatus, "CLOSED") Then

        '                        'TR New method to exportation
        '                        'myGlobalDataTO = ExecuteExportationNEW(dbConnection, pAnalyzerID, pWorkSessionID, _
        '                        '                                    pOrderTestID, myExportType, pResetWSFlag)

        '                        myGlobalDataTO = ExecuteExportation(dbConnection, pAnalyzerID, pWorkSessionID, _
        '                                                           pOrderTestID, myExportType, pResetWSFlag)

        '                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                        GlobalBase.CreateLogActivity("ManageLISExportation ExecuteExportation (Complete): " & _
        '                                                        Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
        '                                                        "ExportDelegate.ManageLISExportation", EventLogEntryType.Information, False)
        '                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        '                    Else
        '                        myGlobalDataTO.SetDatos = exportedExecutionsDS
        '                    End If

        '                Else 'No automatic exportation is configured
        '                    myGlobalDataTO.SetDatos = exportedExecutionsDS
        '                End If

        '            End If 'If (Not dbConnection Is Nothing) Then
        '        End If 'If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then

        '        If (Not myGlobalDataTO.HasError) Then
        '            'When the Database Connection was opened locally, then the Commit is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '        Else
        '            'When the Database Connection was opened locally, then the Rollback is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ManageLISExportation", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Verify if the LIMS Export process is automatic, and in this case, get the Export Frequency and execute the exportation 
        '''' when it is possible:
        '''' ** If Export Frequency is "When WS finishes": export is launched if the WS Status is CLOSED
        '''' ** If Export Frequency is "When Patient or Control finishes": export is launched if the ORDER Status is CLOSED
        '''' ** If Export Frequency is "When Test finishes": export is launched if the ORDER TEST Status is CLOSED  
        '''' </summary>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">WorkSession Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with data of all exported executions</returns>
        '''' <remarks>
        '''' Created by:  SA 04/07/2012
        '''' </remarks>
        'Public Function ManageLISExportationNEW(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '                                        ByVal pOrderTestInfoRow As OrderTestsDS.twksOrderTestsRow) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        Dim automaticExport As Boolean = False
        '        Dim myUserSettingDelegate As New UserSettingsDelegate()

        '        'Verify if the export to LIS is defined as automatic
        '        myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUTOMATIC_EXPORT.ToString)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            automaticExport = (CType(myGlobalDataTO.SetDatos, Integer) = 1)
        '        End If

        '        'Continue only when the export is automatic....
        '        If (automaticExport) Then
        '            Dim myStatus As String = String.Empty
        '            Dim myExportType As New ExportType

        '            'Get the export frequency: When WS finishes / When Patient or Control finishes / When Test finishes 
        '            myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.EXPORT_FREQUENCY.ToString)
        '            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                Dim exportFrequency As String = myGlobalDataTO.SetDatos.ToString

        '                'Select Case (exportFrequency)
        '                '    Case "END_WS"
        '                '        'Export results when the WS finished: continue only if the WS Status is CLOSED
        '                '        Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate()

        '                '        myGlobalDataTO = myWSAnalyzersDelegate.ReadWSAnalyzers(Nothing, pAnalyzerID, pWorkSessionID)
        '                '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                '            Dim myWSAnalyzersDS As WSAnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS)

        '                '            If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
        '                '                myStatus = myWSAnalyzersDS.twksWSAnalyzers.First.WSStatus
        '                '                myExportType = ExportType.ALL
        '                '            End If
        '                '        End If
        '                '        Exit Select

        '                '    Case "ORDER"
        '                '        'Export results when all Tests requested for the Patient/Control have finished: continue only if the ORDER Status is CLOSED
        '                '        Dim myOrdersDelegate As New OrdersDelegate

        '                '        myGlobalDataTO = myOrdersDelegate.ReadByOrderTestID(Nothing, pOrderTestInfoRow.OrderTestID)
        '                '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                '            Dim myOrdersDS As OrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)

        '                '            If (myOrdersDS.twksOrders.Rows.Count > 0) Then
        '                '                myStatus = myOrdersDS.twksOrders.First.OrderStatus
        '                '                myExportType = ExportType.ORDER
        '                '            End If
        '                '        End If
        '                '        Exit Select

        '                '    Case "ORDERTEST"
        '                '        'Export results when each Test finished: continue only if the ORDER TEST Status is CLOSED
        '                '        If (pVerifyOTStatus) Then
        '                '            Dim myOrderTestDelegate As New OrderTestsDelegate

        '                '            myGlobalDataTO = myOrderTestDelegate.GetOrderTest(Nothing, pOrderTestInfoRow.OrderTestID)
        '                '            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                '                Dim myOrderTestsDS As OrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

        '                '                If (myOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
        '                '                    pOrderTestInfoRow.SampleClass = myOrderTestsDS.twksOrderTests.First.SampleClass
        '                '                    pOrderTestInfoRow.Statflag = myOrderTestsDS.twksOrderTests.First.StatFlag
        '                '                    pOrderTestInfoRow.TestType = myOrderTestsDS.twksOrderTests.First.TestType
        '                '                    pOrderTestInfoRow.TestID = myOrderTestsDS.twksOrderTests.First.TestID
        '                '                    pOrderTestInfoRow.SampleType = myOrderTestsDS.twksOrderTests.First.SampleType


        '                '                    myStatus = myOrderTestsDS.twksOrderTests.First.OrderTestStatus
        '                '                    myExportType = ExportType.ORDER
        '                '                End If
        '                '            End If
        '                '            Exit Select
        '                '        Else
        '                '            myStatus = "CLOSED"
        '                '            myExportType = ExportType.ORDERTEST
        '                '        End If
        '                'End Select

        '                'If (Not myGlobalDataTO.HasError AndAlso myStatus = "CLOSED") Then
        '                '    myGlobalDataTO = ExecuteExportationNEW(Nothing, pAnalyzerID, pWorkSessionID, pOrderTestID, myExportType, pResetWSFlag)
        '                'End If
        '            End If
        '        End If

        '        'If the Exportation is not automatic or if an error was raised in whatever of the executed functions, return an empty ExecutionsDS
        '        If (myGlobalDataTO.HasError OrElse Not automaticExport) Then myGlobalDataTO.SetDatos = New ExecutionsDS
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ManageLISExportationNEW", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Export results (according the informed Export Type) to an external LIMS system - Implementation using a TXT file
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">WorkSession Identifier</param>
        '''' <param name="pOrderTestID">Order Test Identifier</param>
        '''' <param name="pExportType">Type of Export: ALL, ORDER or ORDER TEST</param>
        '''' <param name="pReseWSFlag"></param>
        '''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with data of all exported executions</returns>
        '''' <remarks>
        '''' Created by:  SA 04/07/2012 
        '''' </remarks>
        'Private Function ExecuteExportationNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '                                       ByVal pOrderTestID As Integer, ByVal pExportType As ExportType, ByVal pReseWSFlag As Boolean) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        'Get all results to export according the informed Export Type
        '        myGlobalDataTO = GetDataToExport(Nothing, pAnalyzerID, pWorkSessionID, pOrderTestID, pExportType)
        '        If (Not myGlobalDataTO Is Nothing AndAlso Not myGlobalDataTO.HasError) Then
        '            Dim myResultsList As List(Of LimsResultTO) = DirectCast(myGlobalDataTO.SetDatos, List(Of LimsResultTO))

        '            'Move all results to export to the TXT file
        '            myGlobalDataTO = CreateExportFile(Nothing, pReseWSFlag, myResultsList, pWorkSessionID)
        '            If (Not myGlobalDataTO.HasError) Then

        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ExecuteExportationNEW", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function        'Public Function ExportToLISManual(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO

        '    Try
        '        Dim myResultsDelegate As New ResultsDelegate
        '        resultData = myResultsDelegate.GetResultsByLIS(Nothing, pAnalyzerID, pWorkSessionID)

        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            Dim ResultsData As ResultsDS = DirectCast(resultData.SetDatos, ResultsDS)

        '            Dim Results As List(Of ResultsDS.vwksResultsRow) = (From row In ResultsData.vwksResults _
        '                                                                Where String.Equals(row.ValidationStatus, "OK") _
        '                                                                Order By row.ResultDateTime _
        '                                                                Select row).ToList

        '            If Results.Count > 0 Then

        '                resultData = DAOBase.GetOpenDBConnection(Nothing)

        '                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                    Dim dbConnection As New SqlClient.SqlConnection
        '                    Dim TextFileWriter As StreamWriter
        '                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '                    If (Not dbConnection Is Nothing) Then
        '                        Dim TextFileReader As StreamReader
        '                        Dim TextFileData As String

        '                        'Get from SWParameters table
        '                        Dim myExportFolder As String
        '                        Dim myParams As New SwParametersDelegate

        '                        resultData = myParams.ReadTextValueByParameterName(dbConnection, GlobalEnumerates.SwParameters.LIMS_EXPORT_PATH.ToString, Nothing) 'nothing
        '                        If Not resultData.HasError And Not resultData.SetDatos Is Nothing Then
        '                            myExportFolder = CStr(resultData.SetDatos)
        '                        Else
        '                            myExportFolder = "\Export\"
        '                        End If

        '                        If myExportFolder.StartsWith("\") And Not myExportFolder.StartsWith("\\") Then
        '                            myExportFolder = Application.StartupPath & myExportFolder
        '                        End If

        '                        Dim WSStartDateTime As String = String.Empty
        '                        Dim myWSDelegate As New WorkSessionsDelegate                        'Get WSStartDateTime from DB

        '                        resultData = myWSDelegate.GetByWorkSession(dbConnection, pWorkSessionID)

        '                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                            Dim myWSDataDS As WorkSessionsDS = DirectCast(resultData.SetDatos, WorkSessionsDS)
        '                            If myWSDataDS.twksWorkSessions.Count > 0 AndAlso Not (myWSDataDS.twksWorkSessions.First.IsWSDateTimeNull) Then
        '                                WSStartDateTime = myWSDataDS.twksWorkSessions.First().WSDateTime.ToString
        '                            End If
        '                        End If

        '                        Dim myExportFile As String = GlobalConstants.LIMS_EXPORT_FILE_NAME & " (" & _
        '                                                     CDate(WSStartDateTime).ToString("yy-MM-dd HH:mm").ToString.Replace(":", "-").ToString & ")_"
        '                        Dim fileId As Integer = 1

        '                        'Make a Reference to a Folder. Validate if not exists the folder
        '                        If Not Directory.Exists(myExportFolder) And Not String.Equals(myExportFolder.Trim, String.Empty) Then Directory.CreateDirectory(myExportFolder)

        '                        Dim di As New IO.DirectoryInfo(myExportFolder)
        '                        Dim diar1 As IO.FileInfo() = di.GetFiles(myExportFile & "*.txt")
        '                        Dim dra As IO.FileInfo
        '                        Dim myValue As String
        '                        Dim myLength As Integer

        '                        For Each dra In diar1
        '                            myLength = (dra.ToString.Length - 4) - 24
        '                            myValue = dra.ToString.Substring(24, myLength)

        '                            If IsNumeric(myValue) AndAlso fileId < CInt(myValue) Then fileId = CInt(myValue)

        '                            dra = Nothing
        '                        Next dra

        '                        di = Nothing
        '                        diar1 = Nothing
        '                        dra = Nothing

        '                        myExportFile &= fileId.ToString & ".txt"

        '                        Dim canWrite As Boolean = True

        '                        If Not File.Exists(myExportFolder & myExportFile) Then 'validate path exist.
        '                            'Validate if not exists the folder
        '                            If Not Directory.Exists(myExportFolder) And Not String.Equals(myExportFolder.Trim, "") Then Directory.CreateDirectory(myExportFolder)

        '                            'if the file do not exist then create a new one.
        '                            TextFileWriter = New StreamWriter(myExportFolder & myExportFile)
        '                        Else
        '                            TextFileReader = New StreamReader(myExportFolder & myExportFile)  'if the file exits then load it into a stream variable.
        '                            TextFileData = TextFileReader.ReadToEnd.TrimEnd()                 'Copy all the lines into a text variable.
        '                            TextFileReader.Close()                                            'Close the file to avoid error.

        '                            Try  'Gets if the current stream supports writing.
        '                                Dim fs As New FileStream(myExportFolder & myExportFile, FileMode.OpenOrCreate, FileAccess.Write)
        '                                If Not fs.CanWrite Then canWrite = False
        '                                fs.Close()

        '                            Catch ex As Exception
        '                                canWrite = False
        '                            End Try  'End gets if the current stream supports writing.

        '                            If Not canWrite Then
        '                                fileId += 1

        '                                myExportFile = GlobalConstants.LIMS_EXPORT_FILE_NAME & " (" & _
        '                                                CDate(WSStartDateTime).ToString("yy-MM-dd HH:mm").ToString.Replace(":", "-").ToString & ")_" & _
        '                                                fileId.ToString & ".txt"
        '                            End If

        '                            TextFileWriter = New StreamWriter(myExportFolder & myExportFile)    'Set the file to a Stream Writer
        '                            'if there are any data on the variable then load into the Stream writer variable.
        '                            If TextFileData.ToString.Length > 0 Then TextFileWriter.Write(TextFileData.ToString() & Environment.NewLine)

        '                        End If

        '                        'go throug each result and add it into our Stream writer variable
        '                        Dim myResultValue As String = ""
        '                        Dim myTubeType As String
        '                        Dim mySampleClass As String
        '                        Dim myOrderTestDelegate As New OrderTestsDelegate
        '                        Dim myResultRow As ResultsDS.twksResultsRow
        '                        Dim myOrderTestDS As New OrderTestsDS
        '                        Dim tempResultDS As New ResultsDS
        '                        Dim myExecutionsRow As ExecutionsDS.twksWSExecutionsRow
        '                        Dim myGlobalBase As New GlobalBase
        '                        Dim myExecutionsDS As New ExecutionsDS
        '                        Dim myExecutionDelegate As New ExecutionsDelegate

        '                        For i As Integer = 0 To Results.Count - 1
        '                            If Not Results(i).IsCONC_ValueNull Then
        '                                myResultValue = CStr(Results(i).CONC_Value)
        '                            ElseIf Not Results(i).IsABSValueNull Then
        '                                myResultValue = CStr(Results(i).ABSValue)
        '                            Else
        '                                myResultValue = ""
        '                            End If

        '                            Select Case Results(i).TestType
        '                                Case "STD", "OFFS"
        '                                    If Results(i).StatFlag Then
        '                                        mySampleClass = "U"
        '                                    Else
        '                                        mySampleClass = "N"
        '                                    End If

        '                                Case "ISE"
        '                                    If Results(i).SampleClass = "CTRL" Then
        '                                        mySampleClass = "Q"
        '                                    Else
        '                                        If Results(i).StatFlag Then
        '                                            mySampleClass = "U"
        '                                        Else
        '                                            mySampleClass = "N"
        '                                        End If
        '                                    End If

        '                                Case "CTRL"
        '                                    mySampleClass = "Q"

        '                            End Select

        '                            myTubeType = Space(3)

        '                            resultData = myOrderTestDelegate.GetOrderTest(dbConnection, Results(i).OrderTestID)
        '                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '                                myOrderTestDS = DirectCast(resultData.SetDatos, OrderTestsDS)

        '                                If Not myOrderTestDS Is Nothing AndAlso myOrderTestDS.twksOrderTests.Rows.Count > 0 Then
        '                                    myTubeType = String.Format("{0,-3}", myOrderTestDS.twksOrderTests.First.TubeType)
        '                                End If
        '                            End If

        '                            TextFileWriter.WriteLine(mySampleClass & vbTab & _
        '                                                     CStr(IIf(Results(i).IsPatientIDNull, Space(28), String.Format("{0,-28}", Results(i).PatientID))) & vbTab & _
        '                                                     CStr(IIf(Results(i).IsSampleTypeNull, Space(3), String.Format("{0,-3}", Results(i).SampleType))) & vbTab & _
        '                                                     myTubeType & vbTab & _
        '                                                     CStr(IIf(Results(i).IsTestNameNull, Space(16), String.Format("{0,-16}", Results(i).TestName))) & vbTab & _
        '                                                     CStr(IIf(Results(i).IsTestTypeNull, Space(4), String.Format("{0,-4}", Results(i).TestType))) & vbTab & _
        '                                                     String.Format("{0,-20}", myResultValue.ToString) & vbTab & _
        '                                                     CStr(IIf(Results(i).IsMeasureUnitNull, Space(10), String.Format("{0,-10}", Results(i).MeasureUnit))) & vbTab & _
        '                                                     String.Format("{0,-19}", Results(i).ResultDateTime))


        '                            myResultRow = tempResultDS.twksResults.NewtwksResultsRow()
        '                            myResultRow.ExportStatus = "SENT"
        '                            myResultRow.ExportDateTime = DateTime.Now
        '                            myResultRow.TS_User = GlobalBase.GetSessionInfo().UserName
        '                            myResultRow.TS_DateTime = DateTime.Now
        '                            myResultRow.OrderTestID = Results(i).OrderTestID
        '                            myResultRow.MultiPointNumber = Results(i).MultiPointNumber
        '                            myResultRow.RerunNumber = Results(i).RerunNumber

        '                            tempResultDS.twksResults.AddtwksResultsRow(myResultRow)
        '                            resultData = myResultsDelegate.UpdateExportStatus(dbConnection, tempResultDS)

        '                            If Not resultData.HasError Then
        '                                'clear the table to continue.
        '                                tempResultDS.twksResults.Clear()

        '                                myExecutionsRow = myExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow
        '                                myExecutionsRow.OrderTestID = Results(i).OrderTestID

        '                                resultData = myExecutionDelegate.GetByOrderTest(dbConnection, pWorkSessionID, pAnalyzerID, Results(i).OrderTestID)

        '                                If Not resultData.HasError Then
        '                                    If DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions.Count > 0 AndAlso _
        '                                       Not DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions(0).IsExecutionIDNull Then
        '                                        myExecutionsRow.ExecutionID = DirectCast(resultData.SetDatos, ExecutionsDS).twksWSExecutions(0).ExecutionID
        '                                        'Add Execution Row.
        '                                        myExecutionsDS.twksWSExecutions.AddtwksWSExecutionsRow(myExecutionsRow)
        '                                    End If
        '                                End If

        '                            Else
        '                                Exit For
        '                            End If

        '                        Next i

        '                        TextFileWriter.Close()

        '                        If Not resultData Is Nothing AndAlso Not resultData.HasError Then resultData.SetDatos = myExecutionsDS

        '                    End If
        '                End If

        '            End If

        '        End If

        '    Catch ex As Exception
        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportToLIS", EventLogEntryType.Error, False)

        '    End Try

        '    Return resultData

        'End Function

        '''' <summary>
        '''' Main method, checks the programmed frequency and launch the exportation when needed
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pAnalyzerID"></param>
        '''' <param name="pWorkSessionID">WorkSession ID</param>
        '''' <param name="pOrderTestID">Order Test ID</param>
        '''' <param name="pManualExportFlag"></param>
        '''' <param name="pResetWSFlag" ></param>
        '''' <param name="pSampleClass"></param> 
        '''' <param name="pTestType"></param> 
        '''' <param name="pControlNumber"></param> 
        '''' <param name="pPatientID" ></param>
        '''' <returns>GlobalDataTo with DS as ExecutionsDS with ordertestID DISTINCT exported</returns>
        '''' <remarks>
        '''' CREATE BY: TR 12/05/2010
        '''' Modified by: DL 17/03/2011 add sampleclass, testtype and controlnumber
        '''' Modified by: AG 21/05/2012 - add parameter pResetWSFlag
        '''' </remarks>
        'Public Function ManageLISExportation(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                     ByVal pAnalyzerID As String, _
        '                                     ByVal pWorkSessionID As String, _
        '                                     ByVal pOrderTestID As Integer, _
        '                                     ByVal pManualExportFlag As Boolean, _
        '                                     ByVal pResetWSFlag As Boolean, _
        '                                     Optional ByVal pSampleClass As String = "", _
        '                                     Optional ByVal pTestType As String = "", _
        '                                     Optional ByVal pControlNumber As Integer = 0, _
        '                                     Optional ByVal pPatientID As String = "") As GlobalDataTO

        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim myStatus As String = ""
        '    Dim myExportType As New ExportType
        '    Dim exportedExecutionsDS As New ExecutionsDS

        '    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '    Dim StartTime As DateTime = Now
        '    Dim myLogAcciones1 As New ApplicationLogManager()
        '    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myExportFrecuency As String = ""
        '                Dim myAutomaticExport As Boolean = False
        '                Dim myUserSettingDelegate As New UserSettingsDelegate()

        '                'Get the exportatio Type Manual/Automatic
        '                myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.AUTOMATIC_EXPORT.ToString) '"AUTOMATIC_EXPORT")
        '                If Not myGlobalDataTO.HasError Then
        '                    If CType(myGlobalDataTO.SetDatos, Integer) > 0 Then
        '                        myAutomaticExport = True
        '                    End If
        '                End If
        '                'If the automaticExport is true then get the export frequency.
        '                If myAutomaticExport Then
        '                    'Get the programmed export frequency.
        '                    myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(dbConnection, GlobalEnumerates.UserSettingsEnum.EXPORT_FREQUENCY.ToString) '"EXPORT_FREQUENCY")
        '                    If Not myGlobalDataTO.HasError Then
        '                        'set the found value to our local variable.
        '                        myExportFrecuency = myGlobalDataTO.SetDatos.ToString()
        '                    End If
        '                End If

        '                'If pManualExportFlag = True Then
        '                'AG 23/05/2011 - if no automatic exportation do not export
        '                'myGlobalDataTO = ExecuteExportation(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, ExportType.ORDER, _
        '                '                                    pManualExportFlag, pResetWSFlag, pSampleClass, pTestType, pControlNumber)
        '                'myGlobalDataTO = ExportToLISManual(pAnalyzerID, pWorkSessionID)

        '                'If error then send a empty execution dataset.
        '                ' If myGlobalDataTO.HasError Then
        '                '      myGlobalDataTO.SetDatos = New ExecutionsDS
        '                '   End If

        '                'Else
        '                If myAutomaticExport Then
        '                    'Validate the Export Frequency to determinate witch method is implemented.
        '                    Select Case myExportFrecuency

        '                        Case "END_WS" 'WORKSESSION
        '                            'Get the worksession status
        '                            Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate()
        '                            'Get Worksession values.
        '                            myGlobalDataTO = myWSAnalyzersDelegate.ReadWSAnalyzers(dbConnection, pAnalyzerID, pWorkSessionID)
        '                            If Not myGlobalDataTO.HasError Then
        '                                Dim myWSAnalyzersDS As New WSAnalyzersDS
        '                                myWSAnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS)
        '                                If myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0 Then
        '                                    'validate if the worksession status is CLOSE.
        '                                    myStatus = myWSAnalyzersDS.twksWSAnalyzers(0).WSStatus
        '                                    myExportType = ExportType.ALL
        '                                End If
        '                            End If

        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                            GlobalBase.CreateLogActivity("ManageLISExportation END WS Export Type Automatic (Complete): " & _
        '                                                            Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
        '                                                            "ExportDelegate.ManageLISExportation", EventLogEntryType.Information, False)
        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                            Exit Select

        '                        Case "ORDER" ' ORDER
        '                            Dim myOrderID As String = ""
        '                            Dim myOrdersDelegate As New OrdersDelegate
        '                            Dim myOrderTestsDelegate As New OrderTestsDelegate()

        '                            'Get the order test value to get the order id
        '                            myGlobalDataTO = myOrderTestsDelegate.GetOrderTest(dbConnection, pOrderTestID)
        '                            If Not myGlobalDataTO.HasError Then
        '                                Dim myOrderTestsDS As New OrderTestsDS
        '                                myOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)
        '                                If myOrderTestsDS.twksOrderTests.Rows.Count > 0 Then
        '                                    myOrderID = myOrderTestsDS.twksOrderTests(0).OrderID
        '                                End If
        '                            End If

        '                            If Not String.Equals(myOrderID, String.Empty) Then
        '                                'get the order to set the status to the local variable.
        '                                myGlobalDataTO = myOrdersDelegate.ReadOrders(dbConnection, myOrderID)
        '                                If Not myGlobalDataTO.HasError Then
        '                                    Dim myOrdersDS As New OrdersDS()
        '                                    myOrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)
        '                                    If myOrdersDS.twksOrders.Rows.Count > 0 Then
        '                                        myStatus = myOrdersDS.twksOrders(0).OrderStatus
        '                                        myExportType = ExportType.ORDER
        '                                    End If
        '                                End If
        '                            End If

        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                            GlobalBase.CreateLogActivity("ManageLISExportation ORDER Export Type Automatic (Complete): " & _
        '                                                            Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
        '                                                            "ExportDelegate.ManageLISExportation", EventLogEntryType.Information, False)
        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        '                            Exit Select

        '                        Case "ORDERTEST" 'ORDERTEST
        '                            Dim myOrderTestsDelegate As New OrderTestsDelegate()
        '                            myGlobalDataTO = myOrderTestsDelegate.GetOrderTest(dbConnection, pOrderTestID)  'Get the order test to deteminate the status.
        '                            If Not myGlobalDataTO.HasError Then
        '                                Dim myOrderTestsDS As New OrderTestsDS
        '                                myOrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)
        '                                If myOrderTestsDS.twksOrderTests.Rows.Count > 0 Then
        '                                    myStatus = myOrderTestsDS.twksOrderTests(0).OrderTestStatus
        '                                    myExportType = ExportType.ORDERTEST
        '                                End If
        '                            End If

        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                            GlobalBase.CreateLogActivity("ManageLISExportation ORDER TEST Export Type Automatic (Complete): " & _
        '                                                            Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
        '                                                            "ExportDelegate.ManageLISExportation", EventLogEntryType.Information, False)
        '                            '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        '                            Exit Select
        '                    End Select

        '                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                    StartTime = Now
        '                    '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                    'validate if the status is close to start the process 

        '                    If String.Equals(myStatus, "CLOSED") Then

        '                        'TR New method to exportation
        '                        'myGlobalDataTO = ExecuteExportationNEW(dbConnection, pAnalyzerID, pWorkSessionID, _
        '                        '                                    pOrderTestID, myExportType, pResetWSFlag)

        '                        myGlobalDataTO = ExecuteExportation(dbConnection, pAnalyzerID, pWorkSessionID, _
        '                                                           pOrderTestID, myExportType, pResetWSFlag)

        '                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***
        '                        GlobalBase.CreateLogActivity("ManageLISExportation ExecuteExportation (Complete): " & _
        '                                                        Now.Subtract(StartTime).TotalMilliseconds.ToStringWithDecimals(0), _
        '                                                        "ExportDelegate.ManageLISExportation", EventLogEntryType.Information, False)
        '                        '*** TO CONTROL THE TOTAL TIME OF CRITICAL PROCESSES ***

        '                    Else
        '                        myGlobalDataTO.SetDatos = exportedExecutionsDS
        '                    End If

        '                Else 'No automatic exportation is configured
        '                    myGlobalDataTO.SetDatos = exportedExecutionsDS
        '                End If

        '            End If 'If (Not dbConnection Is Nothing) Then
        '        End If 'If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then

        '        If (Not myGlobalDataTO.HasError) Then
        '            'When the Database Connection was opened locally, then the Commit is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '        Else
        '            'When the Database Connection was opened locally, then the Rollback is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '        End If

        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ManageLISExportation", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Verify if the LIMS Export process is automatic, and in this case, get the Export Frequency and execute the exportation 
        '''' when it is possible:
        '''' ** If Export Frequency is "When WS finishes": export is launched if the WS Status is CLOSED
        '''' ** If Export Frequency is "When Patient or Control finishes": export is launched if the ORDER Status is CLOSED
        '''' ** If Export Frequency is "When Test finishes": export is launched if the ORDER TEST Status is CLOSED  
        '''' </summary>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">WorkSession Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with data of all exported executions</returns>
        '''' <remarks>
        '''' Created by:  SA 04/07/2012
        '''' </remarks>
        'Public Function ManageLISExportationNEW(ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '                                        ByVal pOrderTestInfoRow As OrderTestsDS.twksOrderTestsRow) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Try
        '        Dim automaticExport As Boolean = False
        '        Dim myUserSettingDelegate As New UserSettingsDelegate()

        '        'Verify if the export to LIS is defined as automatic
        '        myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.AUTOMATIC_EXPORT.ToString)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            automaticExport = (CType(myGlobalDataTO.SetDatos, Integer) = 1)
        '        End If

        '        'Continue only when the export is automatic....
        '        If (automaticExport) Then
        '            Dim myStatus As String = String.Empty
        '            Dim myExportType As New ExportType

        '            'Get the export frequency: When WS finishes / When Patient or Control finishes / When Test finishes 
        '            myGlobalDataTO = myUserSettingDelegate.GetCurrentValueBySettingID(Nothing, GlobalEnumerates.UserSettingsEnum.EXPORT_FREQUENCY.ToString)
        '            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                Dim exportFrequency As String = myGlobalDataTO.SetDatos.ToString

        '                'Select Case (exportFrequency)
        '                '    Case "END_WS"
        '                '        'Export results when the WS finished: continue only if the WS Status is CLOSED
        '                '        Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate()

        '                '        myGlobalDataTO = myWSAnalyzersDelegate.ReadWSAnalyzers(Nothing, pAnalyzerID, pWorkSessionID)
        '                '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                '            Dim myWSAnalyzersDS As WSAnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS)

        '                '            If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
        '                '                myStatus = myWSAnalyzersDS.twksWSAnalyzers.First.WSStatus
        '                '                myExportType = ExportType.ALL
        '                '            End If
        '                '        End If
        '                '        Exit Select

        '                '    Case "ORDER"
        '                '        'Export results when all Tests requested for the Patient/Control have finished: continue only if the ORDER Status is CLOSED
        '                '        Dim myOrdersDelegate As New OrdersDelegate

        '                '        myGlobalDataTO = myOrdersDelegate.ReadByOrderTestID(Nothing, pOrderTestInfoRow.OrderTestID)
        '                '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                '            Dim myOrdersDS As OrdersDS = DirectCast(myGlobalDataTO.SetDatos, OrdersDS)

        '                '            If (myOrdersDS.twksOrders.Rows.Count > 0) Then
        '                '                myStatus = myOrdersDS.twksOrders.First.OrderStatus
        '                '                myExportType = ExportType.ORDER
        '                '            End If
        '                '        End If
        '                '        Exit Select

        '                '    Case "ORDERTEST"
        '                '        'Export results when each Test finished: continue only if the ORDER TEST Status is CLOSED
        '                '        If (pVerifyOTStatus) Then
        '                '            Dim myOrderTestDelegate As New OrderTestsDelegate

        '                '            myGlobalDataTO = myOrderTestDelegate.GetOrderTest(Nothing, pOrderTestInfoRow.OrderTestID)
        '                '            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '                '                Dim myOrderTestsDS As OrderTestsDS = DirectCast(myGlobalDataTO.SetDatos, OrderTestsDS)

        '                '                If (myOrderTestsDS.twksOrderTests.Rows.Count > 0) Then
        '                '                    pOrderTestInfoRow.SampleClass = myOrderTestsDS.twksOrderTests.First.SampleClass
        '                '                    pOrderTestInfoRow.Statflag = myOrderTestsDS.twksOrderTests.First.StatFlag
        '                '                    pOrderTestInfoRow.TestType = myOrderTestsDS.twksOrderTests.First.TestType
        '                '                    pOrderTestInfoRow.TestID = myOrderTestsDS.twksOrderTests.First.TestID
        '                '                    pOrderTestInfoRow.SampleType = myOrderTestsDS.twksOrderTests.First.SampleType


        '                '                    myStatus = myOrderTestsDS.twksOrderTests.First.OrderTestStatus
        '                '                    myExportType = ExportType.ORDER
        '                '                End If
        '                '            End If
        '                '            Exit Select
        '                '        Else
        '                '            myStatus = "CLOSED"
        '                '            myExportType = ExportType.ORDERTEST
        '                '        End If
        '                'End Select

        '                'If (Not myGlobalDataTO.HasError AndAlso myStatus = "CLOSED") Then
        '                '    myGlobalDataTO = ExecuteExportationNEW(Nothing, pAnalyzerID, pWorkSessionID, pOrderTestID, myExportType, pResetWSFlag)
        '                'End If
        '            End If
        '        End If

        '        'If the Exportation is not automatic or if an error was raised in whatever of the executed functions, return an empty ExecutionsDS
        '        If (myGlobalDataTO.HasError OrElse Not automaticExport) Then myGlobalDataTO.SetDatos = New ExecutionsDS
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ManageLISExportationNEW", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Export results (according the informed Export Type) to an external LIMS system - Implementation using a TXT file
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <param name="pWorkSessionID">WorkSession Identifier</param>
        '''' <param name="pOrderTestID">Order Test Identifier</param>
        '''' <param name="pExportType">Type of Export: ALL, ORDER or ORDER TEST</param>
        '''' <param name="pReseWSFlag"></param>
        '''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with data of all exported executions</returns>
        '''' <remarks>
        '''' Created by:  SA 04/07/2012 
        '''' </remarks>
        'Private Function ExecuteExportationNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '                                       ByVal pOrderTestID As Integer, ByVal pExportType As ExportType, ByVal pReseWSFlag As Boolean) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        'Get all results to export according the informed Export Type
        '        myGlobalDataTO = GetDataToExport(Nothing, pAnalyzerID, pWorkSessionID, pOrderTestID, pExportType)
        '        If (Not myGlobalDataTO Is Nothing AndAlso Not myGlobalDataTO.HasError) Then
        '            Dim myResultsList As List(Of LimsResultTO) = DirectCast(myGlobalDataTO.SetDatos, List(Of LimsResultTO))

        '            'Move all results to export to the TXT file
        '            myGlobalDataTO = CreateExportFile(Nothing, pReseWSFlag, myResultsList, pWorkSessionID)
        '            If (Not myGlobalDataTO.HasError) Then

        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ExecuteExportationNEW", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Exportation process method
        '''' Execute the Results exportation into a TXT file.
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pAnalyzerID"></param>
        '''' <param name="pWorkSessionID"></param>
        '''' <param name="pOrderTestID"></param>
        '''' <param name="pExportType"></param>
        '''' <param name="pReseWSFlag"></param>
        '''' <returns>globalDataTo with DS ExecutionsDS with all executions exported - TODO TR 04/10/2011</returns>
        '''' <remarks>CREATE BY: TR 13/05/2010
        '''' AG 21/05/2012 add parameter pReseWSFlag</remarks>
        'Private Function ExecuteExportation(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                    ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
        '                                    ByVal pOrderTestID As Integer, ByVal pExportType As ExportType, _
        '                                    ByVal pReseWSFlag As Boolean, _
        '                                    Optional ByVal pSampleClass As String = "", _
        '                                    Optional ByVal pTestType As String = "", _
        '                                    Optional ByVal pControlNumber As Integer = 0) As GlobalDataTO

        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not myGlobalDataTO.HasError) And (Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                'Dim myLimsResultTO As New LimsResultTO()
        '                Dim MyLimsResultList As New List(Of LimsResultTO)
        '                Dim myResultsDelegate As New ResultsDelegate()
        '                'TR 04/10/2011 -DS ExecutionsDS with all executions exported.
        '                Dim myExecutionsDS As New ExecutionsDS

        '                'Get the Data to Export 
        '                myGlobalDataTO = GetDataToExport(dbConnection, pAnalyzerID, pWorkSessionID, pOrderTestID, pExportType)

        '                If Not myGlobalDataTO Is Nothing AndAlso Not myGlobalDataTO.HasError Then
        '                    MyLimsResultList = DirectCast(myGlobalDataTO.SetDatos, List(Of LimsResultTO))

        '                    'Create the Export File.
        '                    myGlobalDataTO = CreateExportFile(dbConnection, pReseWSFlag, MyLimsResultList, pWorkSessionID)

        '                    If Not myGlobalDataTO.HasError Then
        '                        Dim myGlobalBase As New GlobalBase
        '                        Dim tempResultDS As New ResultsDS

        '                        'TR 04/10/2011 -Create new Row
        '                        Dim myExecutionsRow As ExecutionsDS.twksWSExecutionsRow
        '                        Dim myExecutionDelegate As New ExecutionsDelegate

        '                        'Update the ExportStatus and ExportDateTime on the results Datatable.
        '                        Dim myResultRow As ResultsDS.twksResultsRow
        '                        'Dim myResultDS As New ResultsDS
        '                        For i As Integer = 0 To MyLimsResultList.Count - 1
        '                            'Set the SENT status to the export result.

        '                            myResultRow = tempResultDS.twksResults.NewtwksResultsRow()
        '                            myResultRow.ExportStatus = "SENT"
        '                            myResultRow.ExportDateTime = DateTime.Now
        '                            myResultRow.TS_User = GlobalBase.GetSessionInfo().UserName
        '                            myResultRow.TS_DateTime = DateTime.Now
        '                            myResultRow.OrderTestID = MyLimsResultList(i).OrderTestID
        '                            myResultRow.MultiPointNumber = MyLimsResultList(i).MultiPointNumber
        '                            myResultRow.RerunNumber = MyLimsResultList(i).RerunNumber

        '                            tempResultDS.twksResults.AddtwksResultsRow(myResultRow)
        '                            myGlobalDataTO = myResultsDelegate.UpdateExportStatus(dbConnection, tempResultDS)

        '                            If Not myGlobalDataTO.HasError Then
        '                                'clear the table to continue.
        '                                tempResultDS.twksResults.Clear()

        '                                myExecutionsRow = myExecutionsDS.twksWSExecutions.NewtwksWSExecutionsRow
        '                                myExecutionsRow.OrderTestID = MyLimsResultList(i).OrderTestID
        '                                'TR 04/10/2011 -Get the Execution ID 
        '                                myGlobalDataTO = myExecutionDelegate.GetByOrderTest(dbConnection, pWorkSessionID, pAnalyzerID, MyLimsResultList(i).OrderTestID)

        '                                If Not myGlobalDataTO.HasError Then
        '                                    If DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS).twksWSExecutions.Count > 0 AndAlso _
        '                                       Not DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS).twksWSExecutions(0).IsExecutionIDNull Then
        '                                        myExecutionsRow.ExecutionID = DirectCast(myGlobalDataTO.SetDatos, ExecutionsDS).twksWSExecutions(0).ExecutionID
        '                                        'Add Execution Row.
        '                                        myExecutionsDS.twksWSExecutions.AddtwksWSExecutionsRow(myExecutionsRow)
        '                                    End If
        '                                End If

        '                            Else
        '                                Exit For
        '                            End If
        '                        Next i

        '                    End If
        '                End If

        '                If Not myGlobalDataTO Is Nothing AndAlso Not myGlobalDataTO.HasError Then
        '                    'TR 04/10/2011 -Set the Executions DS value.
        '                    myGlobalDataTO.SetDatos = myExecutionsDS
        '                End If


        '            End If 'If (Not dbConnection Is Nothing) Then
        '        End If 'If (Not myGlobal.HasError) And (Not myGlobal.SetDatos Is Nothing) Then

        '        If (Not myGlobalDataTO Is Nothing AndAlso Not myGlobalDataTO.HasError) Then
        '            'When the Database Connection was opened locally, then the Commit is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '            'myGlobal.SetDatos = <value to return; if any>
        '        Else
        '            'When the Database Connection was opened locally, then the Rollback is executed
        '            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '        End If


        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "ExportDelegate.ExecuteExportation", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

#End Region

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class
End Namespace
