Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL
    Public Class HisWSExecutionsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Insert a group of Executions in Historic Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSExecutionsDS">Typed DataSet HisWSExecutionsDS containing data of all Executions to add to 
        '''                                  Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 20/06/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSExecutionsDS As HisWSExecutionsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisWSExecutionsDAO As New thisWSExecutionsDAO
                        myGlobalDataTO = myHisWSExecutionsDAO.Create(dbConnection, pHisWSExecutionsDS)

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSExecutionsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "View to reuse IResultsCalibCurve screen in history"

        ''' <summary>
        ''' Get the execution results and alarms for use in IResultsCalibCurve
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHistOrderTestID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>GlobalDataTo with data as ExecutionsDS</returns>
        ''' <remarks>AG 17/10/2012 - Creation</remarks>
        Public Function GetExecutionResultsForCalibCurve(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As Integer, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSExecutionsDAO
                        '1) Get execution curve results
                        resultData = myDAO.GetExecutionResultsForCalibCurve(dbConnection, pHistOrderTestID, pAnalyzerID, pWorkSessionID)

                        '2) Get execution curve alarms
                        If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                            Dim toReturnData As New ExecutionsDS
                            toReturnData = CType(resultData.SetDatos, ExecutionsDS)

                            If toReturnData.vwksWSExecutionsResults.Rows.Count > 0 Then
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
                                '    For Each rowRes As ExecutionsDS.vwksWSExecutionsResultsRow In toReturnData.vwksWSExecutionsResults.Rows
                                '        If Not rowRes.IsAlarmListNull Then
                                '            myAlarms = Split(rowRes.AlarmList, GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR)

                                '            For Each ID As String In myAlarms
                                '                If ID <> "" Then
                                '                    list = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefinition.tfmwAlarms _
                                '                            Where a.AlarmID = ID Select a).ToList
                                '                    If list.Count > 0 Then
                                '                        Dim viewAlarmRow As ExecutionsDS.vwksWSExecutionsAlarmsRow
                                '                        viewAlarmRow = toReturnData.vwksWSExecutionsAlarms.NewvwksWSExecutionsAlarmsRow
                                '                        viewAlarmRow.SetExecutionIDNull()
                                '                        viewAlarmRow.HistOrderTestID = rowRes.OrderTestID
                                '                        viewAlarmRow.MultiPointNumber = rowRes.MultiItemNumber
                                '                        viewAlarmRow.ReplicateNumber = rowRes.ReplicateNumber
                                '                        viewAlarmRow.Description = list(0).Description
                                '                        viewAlarmRow.AlarmID = list(0).AlarmID
                                '                        toReturnData.vwksWSExecutionsAlarms.AddvwksWSExecutionsAlarmsRow(viewAlarmRow)
                                '                        toReturnData.AcceptChanges()
                                '                    End If
                                '                End If
                                '            Next
                                '        End If
                                '    Next
                                '    list = Nothing
                                '    Erase myAlarms
                                'End If
                                resultData = GetExecutionAlarmsForCalibCurve(dbConnection, toReturnData)

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSExecutionsDelegate.GetExecutionResultsForCalibCurve", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Transform the historic AlarmList into a ResultsDS.vwksResultsAlarms
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pExecDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 17/10/2012 Creation</remarks>
        Public Function GetExecutionAlarmsForCalibCurve(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pExecDS As ExecutionsDS) As GlobalDataTO
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
                            For Each rowRes As ExecutionsDS.vwksWSExecutionsResultsRow In pExecDS.vwksWSExecutionsResults.Rows
                                If Not rowRes.IsAlarmListNull Then
                                    myAlarms = Split(rowRes.AlarmList, GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR)

                                    For Each ID As String In myAlarms
                                        If ID <> "" Then
                                            list = (From a As AlarmsDS.tfmwAlarmsRow In alarmsDefinition.tfmwAlarms _
                                                    Where a.AlarmID = ID Select a).ToList
                                            If list.Count > 0 Then
                                                Dim viewAlarmRow As ExecutionsDS.vwksWSExecutionsAlarmsRow
                                                viewAlarmRow = pExecDS.vwksWSExecutionsAlarms.NewvwksWSExecutionsAlarmsRow
                                                viewAlarmRow.SetExecutionIDNull()
                                                viewAlarmRow.HistOrderTestID = rowRes.OrderTestID
                                                viewAlarmRow.MultiPointNumber = rowRes.MultiItemNumber
                                                viewAlarmRow.ReplicateNumber = rowRes.ReplicateNumber
                                                viewAlarmRow.Description = list(0).Description
                                                viewAlarmRow.AlarmID = list(0).AlarmID
                                                pExecDS.vwksWSExecutionsAlarms.AddvwksWSExecutionsAlarmsRow(viewAlarmRow)
                                                pExecDS.AcceptChanges()
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSExecutionsDelegate.GetExecutionAlarmsForCalibCurve", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' Delete all Executions saved in Historic Module for the specified AnalyzerID / WorkSessionID / OrderTestID
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
        '                Dim myDao As New thisWSExecutionsDAO
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

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "HisWSExecutionsDelegate.DeleteByHistOrderTestID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace
