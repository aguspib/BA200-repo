Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports System.Text
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.BL
    Public Class HisWSResultsDelegate

#Region "Declarations"
        Private ReadOnly DatePattern As String = SystemInfoManager.OSDateFormat
        Private ReadOnly TimePattern As String = SystemInfoManager.OSShortTimeFormat
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Close all old Blank or Calibrator results
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  JB 10/10/2012
        ''' Modified by: SA 19/10/2012 - Removed parameter pTestVersionNum; it is not needed 
        ''' </remarks>
        Public Function CloseOLDBlankCalibResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                  ByVal pHistTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisWSResultsDAO As New thisWSResultsDAO
                        myGlobalDataTO = myHisWSResultsDAO.CloseOLDBlankCalibResults(dbConnection, pHistTestID, pSampleType, pAnalyzerID)

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.CloseOLDBlankCalibResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add a group of Results in the corresponding table in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSResultsDS">Typed DataSet HisWSResultsDS containing the group of Results to add</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 20/06/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSResultsDS As HisWSResultsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisWSResultsDAO As New thisWSResultsDAO
                        myGlobalDataTO = myHisWSResultsDAO.Create(dbConnection, pHisWSResultsDS)

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all Patient Results for the informed list of AnalyzerID / WorkSessionID / HistOrderTestID 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSOrderTestsDS">Typed DataSet HisWSOrderTestsDS with the list of AnalyzerID / WorkSessionID / HistOrderTestID 
        '''                                  selected to be deleted</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 03/07/2013
        ''' </remarks>
        Public Function DeleteResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSOrderTestsDS As HisWSOrderTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSResultsDAO
                        resultData = myDAO.DeleteResults(dbConnection, pHisWSOrderTestsDS)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.DeleteResults", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' After a result has been exported from historical results the fields used for upload to LIS are cleared (free database space)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pLISMessageID">The analyzerID to delete the results</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by: DL 24/04/2013
        ''' AG 25/04/2013 before call clear in my DAO call clear method in historic order tests delegate
        ''' </remarks>
        Public Function ClearIdentifiersForLIS(ByVal pDBConnection As SqlClient.SqlConnection,
                                               ByVal pLISMessageID As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHistOTDlg As New HisWSOrderTestsDelegate
                        resultData = myHistOTDlg.ClearIdentifiersForLIS(Nothing, pLISMessageID)

                        If Not resultData.HasError Then
                            Dim myDao As New thisWSResultsDAO
                            resultData = myDao.ClearIdentifiersForLIS(dbConnection, pLISMessageID)
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.ClearIdentifiersForLIS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "View to reuse IResultsCalibCurve screen in history"

        ''' <summary>
        ''' Get the AVG results and alarms for use in IResultsCalibCurve
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHistOrderTestID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>GlobalDataTo with data as ResultsDS</returns>
        ''' <remarks>AG 17/10/2012 - Creation</remarks>
        Public Function GetAvgResultsForCalibCurve(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As Integer, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSResultsDAO
                        '1) Get avg curve results
                        resultData = myDAO.GetAvgResultsForCalibCurve(dbConnection, pHistOrderTestID, pAnalyzerID, pWorkSessionID)

                        '2) Get avg curve alarms
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim toReturnData As New ResultsDS
                            toReturnData = CType(resultData.SetDatos, ResultsDS)

                            If toReturnData.vwksResults.Rows.Count > 0 Then
                                ''2.1) Get all alarms definition
                                'Dim alarmDlg As New AlarmsDelegate
                                'Dim alarmsDefinition As New AlarmsDS
                                'resultData = alarmDlg.ReadAll(dbConnection)
                                'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                '    alarmsDefinition = CType(resultData.SetDatos, AlarmsDS)
                                'End If

                                ''2.2) Convert alarmList into several vwksResultsAlarms rows
                                'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                '    Dim myAlarms() As String
                                '    Dim list As List(Of AlarmsDS.tfmwAlarmsRow)
                                '    For Each rowRes As ResultsDS.vwksResultsRow In toReturnData.vwksResults.Rows
                                '        If Not rowRes.IsAlarmListNull Then
                                '            myAlarms = Split(rowRes.AlarmList, GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR)

                                '            For Each ID As String In myAlarms
                                '                If ID <> "" Then
                                '                    list = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefinition.tfmwAlarms _
                                '                            Where a.AlarmID = ID Select a).ToList
                                '                    If list.Count > 0 Then
                                '                        Dim viewAlarmRow As ResultsDS.vwksResultsAlarmsRow
                                '                        viewAlarmRow = toReturnData.vwksResultsAlarms.NewvwksResultsAlarmsRow
                                '                        viewAlarmRow.OrderTestID = rowRes.OrderTestID
                                '                        viewAlarmRow.RerunNumber = rowRes.RerunNumber
                                '                        viewAlarmRow.MultiPointNumber = rowRes.MultiPointNumber
                                '                        viewAlarmRow.Description = list(0).Description
                                '                        viewAlarmRow.AcceptedResultFlag = True
                                '                        viewAlarmRow.AlarmID = list(0).AlarmID
                                '                        toReturnData.vwksResultsAlarms.AddvwksResultsAlarmsRow(viewAlarmRow)
                                '                        toReturnData.AcceptChanges()
                                '                    End If
                                '                End If
                                '            Next
                                '        End If
                                '    Next
                                '    list = Nothing
                                '    Erase myAlarms
                                'End If
                                resultData = GetAvgAlarmsForCalibCurve(dbConnection, toReturnData)

                            End If 'If toReturnData.vwksResults.Rows.Count > 0 Then

                            If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                resultData.SetDatos = toReturnData
                            End If

                        End If 'If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then (2)

                    End If 'If (Not dbConnection Is Nothing) Then
                End If ' If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then (1)

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.GetAvgResultsForCalibCurve", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Transform the historic AlarmList into a ResultsDS.vwksResultsAlarms
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pResultsAvg"></param>
        ''' <returns></returns>
        ''' <remarks>AG 17/10/2012 Creation</remarks>
        Public Function GetAvgAlarmsForCalibCurve(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pResultsAvg As ResultsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        '2.1) Get all alarms definition
                        Dim alarmDlg As New AlarmsDelegate
                        Dim alarmsDefinition As New AlarmsDS
                        resultData = alarmDlg.ReadAll(dbConnection)
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            alarmsDefinition = CType(resultData.SetDatos, AlarmsDS)
                        End If

                        '2.2) Convert alarmList into several vwksResultsAlarms rows
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim myAlarms() As String
                            Dim list As List(Of AlarmsDS.tfmwAlarmsRow)
                            For Each rowRes As ResultsDS.vwksResultsRow In pResultsAvg.vwksResults.Rows
                                If Not rowRes.IsAlarmListNull Then
                                    myAlarms = Split(rowRes.AlarmList, GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR)

                                    For Each ID As String In myAlarms
                                        If String.Compare(ID, "", False) <> 0 Then
                                            list = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefinition.tfmwAlarms _
                                                    Where String.Compare(a.AlarmID, ID, False) = 0 Select a).ToList
                                            If list.Count > 0 Then
                                                Dim viewAlarmRow As ResultsDS.vwksResultsAlarmsRow
                                                viewAlarmRow = pResultsAvg.vwksResultsAlarms.NewvwksResultsAlarmsRow
                                                viewAlarmRow.OrderTestID = rowRes.OrderTestID
                                                viewAlarmRow.RerunNumber = rowRes.RerunNumber
                                                viewAlarmRow.MultiPointNumber = rowRes.MultiPointNumber
                                                viewAlarmRow.Description = list(0).Description
                                                viewAlarmRow.AcceptedResultFlag = True
                                                viewAlarmRow.AlarmID = list(0).AlarmID
                                                pResultsAvg.vwksResultsAlarms.AddvwksResultsAlarmsRow(viewAlarmRow)
                                                pResultsAvg.AcceptChanges()
                                            End If
                                        End If
                                    Next
                                End If
                            Next
                            list = Nothing
                            Erase myAlarms
                        End If

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.GetAvgAlarmsForCalibCurve", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

#Region " Get Historical Results "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pDateFrom"></param>
        ''' <param name="pDateTo"></param>
        ''' <param name="pSamplePatientId"></param>
        ''' <param name="pSampleClasses"></param>
        ''' <param name="pSampleTypes"></param>
        ''' <param name="pStatFlag"></param>
        ''' <param name="pTestTypes"></param>
        ''' <param name="pTestStartName"></param>
        ''' <returns></returns>
        ''' <remarks>Modified by DL 23/10/2012 Add optional Worksession ID filter</remarks>
        Public Function GetHistoricalResultsByFilter(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                     Optional ByVal pDateFrom As Date = Nothing, Optional ByVal pDateTo As Date = Nothing, _
                                                     Optional ByVal pSamplePatientId As String = "", Optional ByVal pSampleClasses As List(Of String) = Nothing, _
                                                     Optional ByVal pSampleTypes As List(Of String) = Nothing, Optional ByVal pStatFlag As TriState = TriState.UseDefault, _
                                                     Optional ByVal pTestTypes As List(Of String) = Nothing, Optional ByVal pTestStartName As String = "", _
                                                     Optional ByVal pWorkSessionID As String = "") As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSResultsDAO
                        resultData = myDAO.GetHistoricalResultsByFilter(dbConnection, pAnalyzerID, _
                                                                        pDateFrom, pDateTo, _
                                                                        pSamplePatientId, pSampleClasses, pSampleTypes, pStatFlag, _
                                                                        pTestTypes, pTestStartName, pWorkSessionID)

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.GetHistoricalResults", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Transform the historic AlarmList into a HisWSResultsDS.vhisWSResultsRow.Remarks (description of alarms in alarmList)
        ''' in order to be use in the historic patient and blank and calibrator results screen
        ''' 
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAlarmsDefinition"></param>
        ''' <param name="pRow"></param>
        ''' <returns>GlobalDataTo with error or not. pRow.Remark and RemarkAlert updated</returns>
        ''' <remarks>AG 22/10/2012 Creation
        ''' AG 13/02/2014 - #1505 Historic improvments - Not open connection, it is not used and method is called inside a wide loop</remarks>
        Public Function GetAvgAlarmsForScreen(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pAlarmsDefinition As AlarmsDS, ByRef pRow As HisWSResultsDS.vhisWSResultsRow) As GlobalDataTO
            Dim resultData As New GlobalDataTO  'AG 13/02/2014 - #1505 'Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                'AG 13/02/2014 - #1505
                'resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                'If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                '    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                '    If (Not dbConnection Is Nothing) Then
                'AG 13/02/2014 - #1505

                '1) Convert alarmList into Remarks description with "," char as separator
                Dim myAlarms() As String
                Dim list As List(Of AlarmsDS.tfmwAlarmsRow)

                If Not pRow.IsAlarmListNull AndAlso Not String.IsNullOrEmpty(pRow.AlarmList) Then
                    pRow.BeginEdit()
                    'If not informed and exists alarms then modify remark alert column
                    'pRow.RemarkAlert = "*" 'Initial version this field was calculated here. Now it is informed with several values during Reset WS
                    If String.IsNullOrEmpty(pRow.RemarkAlert) Then pRow.RemarkAlert = "*"

                    myAlarms = Split(pRow.AlarmList, GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR)

                    For Each ID As String In myAlarms
                        If String.Compare(ID, "", False) <> 0 Then
                            list = (From a As AlarmsDS.tfmwAlarmsRow In pAlarmsDefinition.tfmwAlarms _
                                    Where String.Compare(a.AlarmID, ID, False) = 0 Select a).ToList
                            If list.Count > 0 Then
                                If pRow.IsRemarksNull Then
                                    pRow.Remarks = list(0).Description
                                Else
                                    pRow.Remarks &= ", " & list(0).Description
                                End If
                            End If
                        End If
                    Next

                    pRow.EndEdit()
                    pRow.AcceptChanges()
                End If

                list = Nothing
                Erase myAlarms

                'AG 13/02/2014 - #1505
                '    End If
                'End If
                'AG 13/02/2014 - #1505

            Catch ex As Exception
                'resultData = New GlobalDataTO() 'AG 13/02/2014 - #1505
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.GetAvgAlarmsForScreens", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Get the historical blank and calibrator results with the selected filter in screen
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pDateFrom"></param>
        ''' <param name="pDateTo"></param>
        ''' <param name="pTestNameContains"></param>
        ''' <returns>GlobalDataTo (data as HisWSResultsDS.vhisWSResults</returns>
        ''' <remarks>AG 22/10/2012</remarks>
        Public Function GetHistoricalBlankCalibResultsByFilter(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                                     Optional ByVal pDateFrom As Date = Nothing, Optional ByVal pDateTo As Date = Nothing, _
                                                     Optional ByVal pTestNameContains As String = "") As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSResultsDAO
                        resultData = myDAO.GetHistoricalBlankCalibResultsByFilter(dbConnection, pAnalyzerID, _
                                                                        pDateFrom, pDateTo, pTestNameContains)

                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.GetHistoricalBlankCalibResultsByFilter", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get all the Results for the specified OrderTestID (Blank or Calibrator Order Test)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderTestList">Order Test List</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS with all the Results (vwksResults) found 
        '''          for the informed OrderTestID</returns>
        ''' <remarks>
        ''' Created by:  DL 23/10/2012
        ''' </remarks>
        Public Function GetResultsForReports(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderTestList As List(Of Integer)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim i As Integer = 0
                        Dim myHisWSResultsDS As New HisWSResultsDS
                        Dim myOrderList As New StringBuilder
                        Dim AverageResultsDS As New HisWSResultsDS
                        Dim mythisWSResultsDAO As New thisWSResultsDAO

                        For Each orderTestID As Integer In pOrderTestList
                            If (i = 200) Then
                                i = 0
                                myOrderList.Append(orderTestID & ",")

                                'Get all results for current OrderTestID
                                resultData = mythisWSResultsDAO.GetResultsForReport(dbConnection, myOrderList.ToString().Remove(myOrderList.ToString().Length - 1, 1))
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    myHisWSResultsDS = DirectCast(resultData.SetDatos, HisWSResultsDS)

                                    For Each resultRow As ResultsDS.vwksResultsRow In myHisWSResultsDS.vhisWSResults.Rows
                                        AverageResultsDS.vhisWSResults.ImportRow(resultRow)
                                    Next resultRow
                                Else
                                    Exit For
                                End If

                                myOrderList.Length = 0
                            Else
                                myOrderList.Append(orderTestID & ",")
                            End If

                            If (pOrderTestList.Last() = orderTestID) Then
                                If (myOrderList.Length >= 0) Then
                                    'Get all results for current OrderTestID.
                                    resultData = mythisWSResultsDAO.GetResultsForReport(dbConnection, myOrderList.ToString().Remove(myOrderList.ToString().Length - 1, 1))
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        myHisWSResultsDS = DirectCast(resultData.SetDatos, HisWSResultsDS)

                                        For Each resultRow As HisWSResultsDS.vhisWSResultsRow In myHisWSResultsDS.vhisWSResults.Rows
                                            AverageResultsDS.vhisWSResults.ImportRow(resultRow)
                                        Next resultRow
                                    Else
                                        Exit For
                                    End If

                                    myOrderList.Length = 0
                                End If
                            End If

                            i += 1
                        Next orderTestID

                        If (Not resultData.HasError) Then
                            resultData.SetDatos = AverageResultsDS
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.GetResultsForReports", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Get Historic Results info by Patient Sample for Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSResults">Result to printAnalyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (ReportMaster and ReportDetails tables)</returns>
        ''' <remarks>
        ''' Created by:  DL 24/10/2012
        ''' Modified by: SA 29/04/2014 - BT #1608 ==> When filling the Report Details table, field TestLongName is used as Test Name if it is 
        '''                                           informed; otherwise, value of field TestName is used
        ''' </remarks>
        Public Function GetHistoricResultsByPatientSampleForReport(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                                   ByVal pHisWSResults As List(Of HisWSResultsDS.vhisWSResultsRow)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get labels for Header Report in the active language
                        Dim currentLanguageGlobal As New GlobalBase
                        Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage

                        Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                        Dim literalPatientID As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_PatientID", CurrentLanguage)
                        Dim literalFullName As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Summary_PatientName", CurrentLanguage)
                        Dim literalGender As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Gender", CurrentLanguage)
                        Dim literalBirthDate As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_DateOfBirth", CurrentLanguage)
                        Dim literalAge As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Age", CurrentLanguage)
                        Dim literalPerformedBy As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Patients_PerformedBy", CurrentLanguage)
                        Dim literalComments As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Remarks", CurrentLanguage)
                        
                        'Get Historic Patient data
                        Dim myHistPatientsDelegate As New HisPatientsDelegate
                        Dim SelectedPatients As List(Of String) = (From row In pHisWSResults _
                                                                   Where String.Compare(row.SampleClass, "PATIENT", False) = 0 _
                                                                   Select row.PatientID Distinct).ToList()

                        resultData = myHistPatientsDelegate.GetPatientsForReport(dbConnection, CurrentLanguage, SelectedPatients)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myHisPatient As HisPatientDS = DirectCast(resultData.SetDatos, HisPatientDS)

                            'Create info for not registered patients
                            Dim myPatientID As String
                            Dim newRow As HisPatientDS.thisPatientsRow

                            For Each PatientID As String In SelectedPatients
                                myPatientID = PatientID
                                If (From row In myHisPatient.thisPatients Where String.Compare(row.PatientID, myPatientID, False) = 0 Select row).Count = 0 Then
                                    newRow = myHisPatient.thisPatients.NewthisPatientsRow()
                                    newRow.PatientID = PatientID
                                    myHisPatient.thisPatients.AddthisPatientsRow(newRow)
                                End If
                            Next PatientID

                            Dim AgeUnitsListDS As New PreloadedMasterDataDS
                            Dim preloadedMasterConfig As New PreloadedMasterDataDelegate

                            resultData = preloadedMasterConfig.GetList(dbConnection, PreloadedMasterDataEnum.AGE_UNITS)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                AgeUnitsListDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                                'Dim ResultsForReportDS As New HisWSResultsDS
                                Dim ResultsForReportDS As New ResultsDS
                                Dim PatientIDList As New List(Of String)
                                Dim SamplesList As List(Of HisWSResultsDS.vhisWSResultsRow)
                                Dim StatFlag() As Boolean = {True, False}
                                Dim FullID As String
                                Dim FullName As String
                                Dim FullGender As String
                                Dim FullBirthDate As String
                                Dim FullAge As String
                                Dim FullPerformedBy As String
                                Dim FullComments As String
                                Dim LinqPat As HisPatientDS.thisPatientsRow

                                Dim ABSValue As String
                                Dim TestName As String
                                Dim CONC_Value As String
                                Dim SampleType As String
                                Dim ReferenceRanges As String
                                Dim ResultDate As String
                                Dim Unit As String
                                Dim Remarks As String
                                Dim myGender As String

                                'Fill ReportMaster table ********************************************************************************************
                                For i As Integer = 0 To 1
                                    SamplesList = (From row In pHisWSResults _
                                                   Where String.Compare(row.SampleClass, "PATIENT", False) = 0 AndAlso row.StatFlag = StatFlag(i) _
                                                   Select row).ToList()

                                    For Each sampleRow As HisWSResultsDS.vhisWSResultsRow In SamplesList
                                        myPatientID = sampleRow.PatientID

                                        If (Not PatientIDList.Contains(myPatientID)) Then
                                            PatientIDList.Add(myPatientID)

                                            LinqPat = (From row As HisPatientDS.thisPatientsRow In myHisPatient.thisPatients _
                                                       Where String.Compare(row.PatientID, myPatientID, False) = 0 _
                                                       Select row).First()

                                            If (Not LinqPat.IsDateOfBirthNull) Then
                                                LinqPat.AgeWithUnit = Utilities.GetAgeUnits(LinqPat.DateOfBirth, AgeUnitsListDS)
                                                LinqPat.FormatedDateOfBirth = LinqPat.DateOfBirth.ToString(DatePattern)
                                            Else
                                                LinqPat.FormatedDateOfBirth = String.Empty
                                                LinqPat.AgeWithUnit = String.Empty
                                            End If

                                            myGender = String.Empty

                                            If Not LinqPat.Gender Is String.Empty Then
                                                Dim myGlobalDataTO As New GlobalDataTO

                                                resultData = preloadedMasterConfig.GetList(Nothing, PreloadedMasterDataEnum.SEX_LIST)
                                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                                    Dim GenderListDS As New PreloadedMasterDataDS
                                                    Dim LinqGender As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)

                                                    GenderListDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                                                    If (GenderListDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                                        LinqGender = (From row In GenderListDS.tfmwPreloadedMasterData _
                                                                      Where row.ItemID = LinqPat.Gender _
                                                                      Select row).ToList()

                                                        myGender = LinqGender.First.FixedItemDesc.ToString
                                                    End If
                                                End If
                                            End If

                                            FullID = String.Format("{0}: {1}", literalPatientID, myPatientID)
                                            FullName = String.Format("{1} {2}", literalFullName, LinqPat.FirstName, LinqPat.LastName)
                                            FullGender = String.Format("{0}: {1}", literalGender, myGender) 'LinqPat.Gender)
                                            FullBirthDate = String.Format("{0}: {1}", literalBirthDate, LinqPat.FormatedDateOfBirth)
                                            FullAge = String.Format("{0}: {1}", literalAge, LinqPat.AgeWithUnit)
                                            FullPerformedBy = String.Format("{0}: {1}", literalPerformedBy, String.Empty) ' LinqPat.PerformedBy. NO IN V1
                                            FullComments = String.Format("{0}: {1}", literalComments, LinqPat.Comments)

                                            ResultsForReportDS.ReportSampleMaster.AddReportSampleMasterRow _
                                                    (myPatientID, FullID, FullName, FullGender, FullBirthDate, FullAge, FullPerformedBy, FullComments)
                                        End If
                                    Next sampleRow
                                Next i

                                'Fill ReportDetails table
                                Dim myTestName As String = String.Empty
                                Dim myTestLongName As String = String.Empty
                                Dim DetailsList As List(Of HisWSResultsDS.vhisWSResultsRow)

                                For Each SampleID As String In PatientIDList
                                    DetailsList = (From detail In pHisWSResults _
                                                   Where String.Compare(detail.SampleClass, "PATIENT", False) = 0 _
                                                   AndAlso String.Compare(detail.PatientID, SampleID, False) = 0 _
                                                   Select detail).ToList

                                    For Each detail As HisWSResultsDS.vhisWSResultsRow In DetailsList
                                        'BT #1608 - Check if fields TestName and TestLongName are informed. Always that field TestLongName is informed 
                                        '           it has to be used as Test Name in the Report
                                        myTestName = IIf(detail.IsTestNameNull, String.Empty, detail.TestName).ToString
                                        myTestLongName = IIf(detail.IsTestLongNameNull, String.Empty, detail.TestLongName).ToString
                                        TestName = IIf(myTestLongName <> String.Empty, myTestLongName, myTestName).ToString

                                        'Inform the rest of fields in the details row...
                                        SampleType = CStr(IIf(detail.IsSampleTypeNull, String.Empty, CStr(detail.SampleType)))

                                        ABSValue = String.Empty
                                        CONC_Value = detail.CONCValueString
                                        Unit = detail.MeasureUnit

                                        If (Not detail.IsMinRefRangeNull AndAlso Not detail.IsMaxRefRangeNull) Then
                                            ReferenceRanges = String.Format("{0} - {1}", detail.MinRefRange, detail.MaxRefRange)
                                        Else
                                            ReferenceRanges = String.Empty
                                        End If

                                        Remarks = String.Empty
                                        If (Not detail.IsRemarkAlertNull) Then Remarks = detail.RemarkAlert

                                        ResultDate = detail.ResultDateTime.ToString(DatePattern) & " " & detail.ResultDateTime.ToString(TimePattern)

                                        ResultsForReportDS.ReportSampleDetails.AddReportSampleDetailsRow(SampleID, TestName, SampleType, String.Empty, String.Empty, CONC_Value, _
                                                                                                         ReferenceRanges, Unit, ResultDate, Remarks)
                                    Next detail
                                Next SampleID

                                resultData.SetDatos = ResultsForReportDS
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.GetResultsByPatientSampleForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Historic Results info by Patient Sample for Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (ReportMaster and ReportDetails tables)</returns>
        ''' <remarks>
        ''' Created by:  DL 24/10/2012
        ''' </remarks>
        Public Function GetHistoricResultsByPatientSampleForReportOLD(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                                   ByVal pAnalyzerID As String, _
                                                                   ByVal pWorkSessionID As String, _
                                                                   ByVal myHisWSResultsDS As List(Of HisWSResultsDS.vhisWSResultsRow)) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        resultData = GetHistoricalResultsByFilter(dbConnection, pAnalyzerID, , , , , , , , , pWorkSessionID)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myHisWSResults As HisWSResultsDS = DirectCast(resultData.SetDatos, HisWSResultsDS)

                            'Get Literal for heaader report
                            'begin
                            Dim currentLanguageGlobal As New GlobalBase
                            Dim CurrentLanguage As String = currentLanguageGlobal.GetSessionInfo().ApplicationLanguage
                            Dim myMultiLangResourcesDelegate As New MultilanguageResourcesDelegate
                            Dim literalPatientID As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_PatientID", CurrentLanguage)
                            Dim literalFullName As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Summary_PatientName", CurrentLanguage)
                            Dim literalGender As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Gender", CurrentLanguage)
                            Dim literalBirthDate As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_DateOfBirth", CurrentLanguage)
                            Dim literalAge As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Age", CurrentLanguage)
                            Dim literalPerformedBy As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Patients_PerformedBy", CurrentLanguage)
                            Dim literalComments As String = myMultiLangResourcesDelegate.GetResourceText(dbConnection, "LBL_Remarks", CurrentLanguage)
                            'end

                            'Get Historic Patient data
                            Dim myHistPatientsDelegate As New HisPatientsDelegate
                            Dim SelectedPatients As List(Of String) = (From row In myHisWSResults.vhisWSResults _
                                                                       Where String.Compare(row.SampleClass, "PATIENT", False) = 0 _
                                                                       Select row.PatientID Distinct).ToList()
                            '.. AndAlso row.OrderToPrint = True AndAlso row.RerunNumber = 1 ..

                            resultData = myHistPatientsDelegate.GetPatientsForReport(dbConnection, CurrentLanguage, SelectedPatients)
                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                Dim myHisPatient As HisPatientDS = DirectCast(resultData.SetDatos, HisPatientDS)

                                'Create info for not registered patients
                                Dim myPatientID As String
                                Dim newRow As HisPatientDS.thisPatientsRow

                                For Each PatientID As String In SelectedPatients
                                    myPatientID = PatientID
                                    If (From row In myHisPatient.thisPatients Where String.Compare(row.PatientID, myPatientID, False) = 0 Select row).Count = 0 Then
                                        newRow = myHisPatient.thisPatients.NewthisPatientsRow()
                                        newRow.PatientID = PatientID
                                        myHisPatient.thisPatients.AddthisPatientsRow(newRow)
                                    End If
                                Next PatientID

                                Dim AgeUnitsListDS As New PreloadedMasterDataDS
                                Dim preloadedMasterConfig As New PreloadedMasterDataDelegate

                                resultData = preloadedMasterConfig.GetList(dbConnection, PreloadedMasterDataEnum.AGE_UNITS)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    AgeUnitsListDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                                    'Dim ResultsForReportDS As New HisWSResultsDS
                                    Dim ResultsForReportDS As New ResultsDS
                                    Dim PatientIDList As New List(Of String)
                                    Dim SamplesList As List(Of HisWSResultsDS.vhisWSResultsRow)
                                    Dim StatFlag() As Boolean = {True, False}
                                    Dim FullID As String
                                    Dim FullName As String
                                    Dim FullGender As String
                                    Dim FullBirthDate As String
                                    Dim FullAge As String
                                    Dim FullPerformedBy As String
                                    Dim FullComments As String
                                    Dim LinqPat As HisPatientDS.thisPatientsRow

                                    Dim ABSValue As String
                                    Dim TestName As String
                                    Dim CONC_Value As String
                                    Dim SampleType As String
                                    Dim ReferenceRanges As String
                                    Dim ResultDate As String
                                    Dim Unit As String
                                    Dim Remarks As String
                                    Dim myGender As String

                                    'Fill ReportMaster table
                                    For i As Integer = 0 To 1
                                        SamplesList = (From row In myHisWSResults.vhisWSResults _
                                                       Where String.Compare(row.SampleClass, "PATIENT", False) = 0 AndAlso row.StatFlag = StatFlag(i) _
                                                       Select row).ToList()

                                        For Each sampleRow As HisWSResultsDS.vhisWSResultsRow In SamplesList
                                            myPatientID = sampleRow.PatientID

                                            If (Not PatientIDList.Contains(myPatientID)) Then
                                                PatientIDList.Add(myPatientID)

                                                LinqPat = (From row As HisPatientDS.thisPatientsRow In myHisPatient.thisPatients _
                                                           Where String.Compare(row.PatientID, myPatientID, False) = 0 _
                                                           Select row).First()

                                                If (Not LinqPat.IsDateOfBirthNull) Then
                                                    LinqPat.AgeWithUnit = Utilities.GetAgeUnits(LinqPat.DateOfBirth, AgeUnitsListDS)
                                                    LinqPat.FormatedDateOfBirth = LinqPat.DateOfBirth.ToString(DatePattern)
                                                Else
                                                    LinqPat.FormatedDateOfBirth = String.Empty
                                                    LinqPat.AgeWithUnit = String.Empty
                                                End If

                                                myGender = String.Empty

                                                If Not LinqPat.Gender Is String.Empty Then
                                                    Dim myGlobalDataTO As New GlobalDataTO


                                                    resultData = preloadedMasterConfig.GetList(Nothing, PreloadedMasterDataEnum.SEX_LIST)
                                                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                                        Dim GenderListDS As New PreloadedMasterDataDS
                                                        Dim LinqGender As List(Of PreloadedMasterDataDS.tfmwPreloadedMasterDataRow)

                                                        GenderListDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)

                                                        If (GenderListDS.tfmwPreloadedMasterData.Rows.Count > 0) Then
                                                            LinqGender = (From row In GenderListDS.tfmwPreloadedMasterData _
                                                                          Where row.ItemID = LinqPat.Gender _
                                                                          Select row).ToList()

                                                            myGender = LinqGender.First.FixedItemDesc.ToString
                                                        End If
                                                    End If
                                                End If

                                                FullID = String.Format("{0}: {1}", literalPatientID, myPatientID)
                                                FullName = String.Format("{1}", literalFullName, LinqPat.FirstName, LinqPat.LastName)
                                                FullGender = String.Format("{0}: {1}", literalGender, myGender) 'LinqPat.Gender)
                                                FullBirthDate = String.Format("{0}: {1}", literalBirthDate, LinqPat.FormatedDateOfBirth)
                                                FullAge = String.Format("{0}: {1}", literalAge, LinqPat.AgeWithUnit)
                                                FullPerformedBy = String.Empty 'String.Format("{1}", literalPerformedBy, LinqPat.PerformedBy) NO IN V1
                                                FullComments = String.Format("{0}: {1}", literalComments, LinqPat.Comments)



                                                ResultsForReportDS.ReportSampleMaster.AddReportSampleMasterRow(myPatientID, _
                                                                                                               FullID, _
                                                                                                               FullName, _
                                                                                                               FullGender, _
                                                                                                               FullBirthDate, _
                                                                                                               FullAge, _
                                                                                                               FullPerformedBy, _
                                                                                                               FullComments)
                                            End If
                                        Next sampleRow
                                    Next i

                                    'Fill ReportDetails table
                                    Dim DetailsList As List(Of HisWSResultsDS.vhisWSResultsRow)

                                    For Each SampleID As String In PatientIDList
                                        DetailsList = (From detail In myHisWSResults.vhisWSResults _
                                                       Where String.Compare(detail.SampleClass, "PATIENT", False) = 0 _
                                                       AndAlso String.Compare(detail.PatientID, SampleID, False) = 0 _
                                                       Select detail).ToList
                                        'AndAlso String.Compare(detail.TestType, "CALC", False) <> 0 _

                                        For Each detail As HisWSResultsDS.vhisWSResultsRow In DetailsList
                                            Unit = String.Empty
                                            Remarks = String.Empty
                                            CONC_Value = String.Empty
                                            SampleType = String.Empty
                                            ReferenceRanges = String.Empty
                                            ResultDate = String.Empty
                                            ABSValue = String.Empty
                                            TestName = String.Empty

                                            'Insert Details row
                                            TestName = CStr(IIf(detail.IsTestNameNull, String.Empty, CStr(detail.TestName)))
                                            SampleType = CStr(IIf(detail.IsSampleTypeNull, String.Empty, CStr(detail.SampleType)))

                                            If String.Compare(detail.TestType, "OFFS", False) = 0 Then
                                                If Not detail.IsManualResultNull Then
                                                    CONC_Value = CStr(detail.ManualResult)
                                                    Unit = detail.MeasureUnit
                                                ElseIf Not detail.IsManualResultTextNull Then
                                                    CONC_Value = CStr(detail.ManualResultText)
                                                    Unit = String.Empty
                                                End If

                                            Else
                                                If Not detail.IsCONCValueNull Then CONC_Value = CStr(detail.CONCValue)
                                                'If Not detail.IsABSValueNull Then ABSValue = CStr(detail.ABSValue)

                                                If (Not detail.IsCONCValueNull) Then
                                                    'Dim hasConcentrationError As Boolean = False

                                                    'If (Not detail.iscon.IsCONC_ErrorNull) Then
                                                    'hasConcentrationError = Not String.IsNullOrEmpty(resultRow.CONC_Error)
                                                    'End If

                                                    'If (Not hasConcentrationError) Then
                                                    CONC_Value = detail.CONCValue.ToStringWithDecimals(detail.DecimalsAllowed)
                                                    'Else
                                                    'CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                                    'End If
                                                Else
                                                    'If (Not resultRow.IsManualResultTextNull) Then
                                                    'CONC_Value = resultRow.ManualResultText
                                                    'Else
                                                    CONC_Value = GlobalConstants.CONCENTRATION_NOT_CALCULATED
                                                    'End If
                                                End If
                                                Unit = detail.MeasureUnit
                                            End If

                                            If Not detail.IsMinRefRangeNull AndAlso Not detail.IsMaxRefRangeNull Then
                                                ReferenceRanges = String.Format("{0} - {1}", detail.MinRefRange, detail.MaxRefRange)
                                            Else
                                                ReferenceRanges = String.Empty
                                            End If

                                            'If (Not resultRow.IsABS_ErrorNull) Then
                                            '    If (Not String.IsNullOrEmpty(resultRow.ABS_Error)) Then
                                            '        ABSValue = GlobalConstants.ABSORBANCE_ERROR
                                            '        CONC_Value = GlobalConstants.CONC_DUE_ABS_ERROR
                                            '    End If
                                            'End If

                                            'If Not detail.IsAlarmListNull AndAlso Not detail.AlarmList Is String.Empty Then
                                            '    Dim AlarmList() As String

                                            '    'If detail.AlarmList.ToString.Contains(GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR) Then
                                            '    AlarmList = Split(detail.AlarmList, GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR)
                                            '    'Else
                                            '    '   ReDim AlarmList(1)
                                            '    '  AlarmList(0) = detail.AlarmList
                                            '    'End If

                                            '    If AlarmList.Count > 0 Then
                                            '        Dim alarmDlg As New AlarmsDelegate
                                            '        Dim alarmsDefinition As New AlarmsDS
                                            '        Dim myGlobal As GlobalDataTO = Nothing

                                            '        myGlobal = alarmDlg.ReadAll(dbConnection)
                                            '        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                            '            alarmsDefinition = CType(myGlobal.SetDatos, AlarmsDS)

                                            '            Dim list As List(Of AlarmsDS.tfmwAlarmsRow)

                                            '            For Each myAlarm As String In AlarmList
                                            '                list = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefinition.tfmwAlarms _
                                            '                        Where String.Compare(a.AlarmID, myAlarm, False) = 0 Select a).ToList

                                            '                If list.Count > 0 Then
                                            '                    Remarks &= list(0).Description
                                            '                End If
                                            '            Next

                                            '        End If
                                            '    End If

                                            'End If

                                            Remarks = String.Empty

                                            'If ((Not resultRow.IsActiveRangeTypeNull AndAlso Not String.Compare(resultRow.ActiveRangeType, String.Empty, False) = 0) AndAlso _
                                            '    (IsNumeric(CONC_Value))) Then
                                            If (Not detail.IsMinRefRangeNull AndAlso Not detail.IsMaxRefRangeNull) Then
                                                If (CSng(CONC_Value) < CSng(detail.MinRefRange)) OrElse _
                                                    CSng(CONC_Value) > CSng(detail.MaxRefRange) Then
                                                    Remarks = "*"
                                                End If
                                            End If
                                            'End If

                                            ResultDate = detail.ResultDateTime.ToString(DatePattern) & " " & detail.ResultDateTime.ToString(TimePattern)

                                            ResultsForReportDS.ReportSampleDetails.AddReportSampleDetailsRow _
                                                    (SampleID, TestName, SampleType, "", "", CONC_Value, ReferenceRanges, Unit, ResultDate, Remarks)

                                        Next detail

                                    Next SampleID

                                    resultData.SetDatos = ResultsForReportDS
                                End If
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "ResultsDelegate.GetResultsByPatientSampleForReport", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "METHODS FOR EXPORT RESULTS TO LIMS"
        ''' <summary>
        ''' Get Historical Patient and Historical Results to export in a ResultsDS 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistOrderTestId">The list of HistOrderTestId to be exported</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ResultsDS (subtable vwksResults) with the group of Results to export</returns>
        '''  <remarks>
        ''' Created by:  JB 22/10/2012
        ''' </remarks>
        Public Function GetResultsToExportFromHIST(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As List(Of Integer)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mythisWSResultsDAO As New thisWSResultsDAO()
                        resultData = mythisWSResultsDAO.GetResultsToExportFromHIST(dbConnection, pHistOrderTestId)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.GetResultsToExportFromHIST", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field ExportStatus for the group of results sent to and external LIMS system
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pResultsDS">Typed DataSet ResultsDS containing the exported results</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        '''Created by:  TR 22/10/2012
        '''Modified by: SG 10/04/2013 - Add new parameter "pAlternativeStatus"
        ''' </remarks>
        Public Function UpdateExportStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pResultsDS As ResultsDS, ByVal pAlternativeStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mythisResultDAO As New thisWSResultsDAO()
                        myGlobalDataTO = mythisResultDAO.UpdateExportStatus(dbConnection, pResultsDS, pAlternativeStatus)

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.UpdateExportStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update field ExportStatus for the group of Historical Order Test IDs sent to LIS using the ES Library
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistOrderTestID">The list of HistOrderTestId to be exported</param>
        ''' <param name="pExportStatus">Value of the Export Status to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/01/2014
        ''' </remarks>
        Public Function UpdateLISExportStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As List(Of Integer), _
                                              ByVal pExportStatus As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mythisResultDAO As New thisWSResultsDAO()
                        myGlobalDataTO = mythisResultDAO.UpdateLISExportStatus(dbConnection, pHistOrderTestID, pExportStatus)

                        If (Not myGlobalDataTO.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.UpdateLISExportStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Updates the LISMessageID (requires ExportStatus to "SENDING")
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSResultsDS"></param>
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by:  SG 10/04/2012
        ''' Modified by: DL 24/04/2013
        ''' Entry parameter HisWSResultsDS (data table hisWSResults) with several rows (informed columns HistOrdertestid, LISMessageID, ExportStatus)
        ''' Updates the columns:
        ''' -	ExportStatus (SENDING)
        ''' -	LISMessageID (guid number)
        '''Where HistOrderTestID = OrderTest in DS parameter
        ''' </remarks>
        Public Function UpdateLISMessageID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSResultsDS As HisWSResultsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSResultsDAO

                        resultData = myDAO.UpdateLISMessageID(dbConnection, pHisWSResultsDS)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.UpdateLISMessageID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Updates the LISMessageID depending on ExportStatus
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLISMessageID"></param> 
        ''' <param name="pNewExportStatus"></param> 
        ''' <returns>GlobalDataTo with error or not</returns>
        ''' <remarks>
        ''' Created by:  SG 10/04/2012
        ''' Modified by: DL 24/04/2013. Updates ExportStatus = NewExportStatus for those historical results with LISMessageID = parameter LISMEssageID
        ''' </remarks>
        Public Function UpdateExportStatusByMessageID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                      ByVal pLISMessageID As String, _
                                                      ByVal pNewExportStatus As String) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSResultsDAO

                        resultData = myDAO.UpdateExportStatusByMessageID(dbConnection, pLISMessageID, pNewExportStatus)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.UpdateExportStatusByMessageID", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function
#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' Deletes the results in table thisWSResults
        ' ''' </summary>
        ' ''' <param name="pDBConnection"></param>
        ' ''' <param name="pAnalyzerID">The analyzerID to delete the results</param>
        ' ''' <param name="pHistOrderTestList">List of the HistOrderTestID to delete</param>
        ' ''' <returns></returns>
        ' ''' <remarks>
        ' ''' Created by: JB 19/10/2012
        ' ''' </remarks>
        'Public Function DeleteResultsOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pHistOrderTestList As List(Of Integer)) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myDao As New thisWSResultsDAO
        '                resultData = myDao.DeleteResults(dbConnection, pAnalyzerID, pHistOrderTestList)

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.DeleteResults", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' Delete all Results saved in Historic Module for the specified AnalyzerID / WorkSessionID / OrderTestID 
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <param name="pHistOrderTestID">Identifier of the Order Test in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing success/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  SG 02/07/2013
        ' ''' </remarks>
        'Public Function DeleteByHistOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                        ByVal pWorkSessionID As String, ByVal pHistOrderTestID As Integer) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myDao As New thisWSResultsDAO
        '                resultData = myDao.DeleteByHistOrderTestID(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestID)

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "HisWSResultsDelegate.DeleteByHistOrderTestID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace


