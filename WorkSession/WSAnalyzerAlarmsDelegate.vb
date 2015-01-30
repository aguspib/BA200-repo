Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.TO
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global.GlobalEnumerates

Namespace Biosystems.Ax00.BL

    Public Class WSAnalyzerAlarmsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Creates analyzer alarms (alarms with status TRUE)
        ''' Updates analyzer alarms (alarms with status FALSE
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerAlarmsDS"></param>
        ''' <param name="pAlarmsDefinitionTable"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  ??
        ''' Modified by: AG 30/05/2012 - Always inform the WorkSessionID
        '''              AG 17/07/2012 - Modified DL code with functionality "Not duplicate active alarms in DataBase, neither solved alarms"
        '''              AG 24/07/2012 - New alarms with status TRUE (Insert), alarms solved who leave to be active (Update)
        ''' </remarks>
        Public Function Save(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerAlarmsDS As WSAnalyzerAlarmsDS, ByVal pAlarmsDefinitionTable As AlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSAnalyzersAlarmsDAO

                        If (pAnalyzerAlarmsDS.twksWSAnalyzerAlarms.Rows.Count > 0) Then
                            'AG 12/12/2011
                            'PROTECTION: Remove duplicities (sometimes working with break points the DS contains primary key duplicates
                            Dim distinctAlarms As List(Of String) = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms _
                                                                   Select a.AlarmID Distinct).ToList

                            Dim duplicatedAlarms As List(Of WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow)
                            For Each item As String In distinctAlarms
                                'Search for duplicate alarms
                                duplicatedAlarms = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms _
                                                   Where a.AlarmID = item Select a).ToList

                                'Treat only when alarms are duplicated
                                If (duplicatedAlarms.Count > 1) Then
                                    Dim removeInitialItemFlag As Boolean = False
                                    Dim alarmsNumber As Integer = duplicatedAlarms.Count

                                    For i As Integer = alarmsNumber - 1 To 1 Step -1
                                        'Same datetime & same alarm status (value TRUE alarm exists or FALSE alarm solved): remove duplicated alarm
                                        'AG 02/02/2012 - Added also AlarmItem condition
                                        If (duplicatedAlarms(i).AlarmDateTime = duplicatedAlarms(0).AlarmDateTime AndAlso _
                                            duplicatedAlarms(i).AlarmItem = duplicatedAlarms(0).AlarmItem AndAlso _
                                            duplicatedAlarms(i).AlarmStatus = duplicatedAlarms(0).AlarmStatus) Then
                                            'Remove duplicated alarm
                                            duplicatedAlarms(i).Delete()

                                            'Same datetime & same alarm but <> status value: remove both duplicated and original alarm
                                            'AG 02/02/2012 - Added also AlarmItem condition
                                        ElseIf (duplicatedAlarms(i).AlarmDateTime = duplicatedAlarms(0).AlarmDateTime AndAlso _
                                                duplicatedAlarms(i).AlarmItem = duplicatedAlarms(0).AlarmItem AndAlso _
                                                duplicatedAlarms(i).AlarmStatus <> duplicatedAlarms(0).AlarmStatus) Then
                                            'Remove both alarms the TRUE and the FALSE
                                            duplicatedAlarms(i).Delete()
                                            removeInitialItemFlag = True
                                        End If
                                    Next

                                    If (removeInitialItemFlag) Then
                                        duplicatedAlarms(0).Delete()
                                    End If
                                End If
                            Next
                            pAnalyzerAlarmsDS.AcceptChanges()

                            'AG 06/06/2012 - New functionality 
                            'Do not create alarms if already exists in database (used when app is closed + opened and similiar situations)
                            Dim updateAnalyzerAlarmsDS As New WSAnalyzerAlarmsDS 'AG 24/07/2012 Ds with alarms who change to status = False

                            If (pAnalyzerAlarmsDS.twksWSAnalyzerAlarms.Rows.Count > 0) Then
                                'Get the alarms defined with OKType = False (never are marked as solved)
                                Dim alarmsWithOKTypeFalse As List(Of String) = (From a As AlarmsDS.tfmwAlarmsRow In pAlarmsDefinitionTable.tfmwAlarms _
                                                                               Where a.OKType = False Select a.AlarmID).ToList

                                'DL 28/11/2012. BUG & TRACKING: 927. When alarms solved its appear in gray color but duplicate. BEGIN ¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡
                                'AG 23/11/2012 - comment this code START
                                'SA 12/11/2012
                                'Get the AnalyzerID 
                                'Dim myAnalyzerID As String = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms _
                                '                             Where Not a.IsAnalyzerIDNull _
                                '                            Select a.AnalyzerID Distinct).ToString

                                'Get the WorkSessionID 
                                'Dim myWorkSessionID As String = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms _
                                '                                Where Not a.IsWorkSessionIDNull _
                                '                               Select a.WorkSessionID Distinct).ToString

                                ''DL 06/06/2012. Begin
                                'Dim myAnalyzerID As String = ""
                                'Dim myWorkSessionID As String

                                'With = DirectCast(resultData.SetDatos, WSAnalyzerAlarmsDS)
                                '    If (.Rows.Count > 0) Then
                                '        If Not .First.IsAnalyzerIDNull Then myAnalyzerID = .First.AnalyzerID
                                '        If Not .First.IsWorkSessionIDNull Then myWorkSessionID = .First.WorkSessionID
                                '    End If
                                'End With
                                'SA 12/11/2012
                                'AG 23/11/2012 - comment this code END

                                'AG 23/11/2012 - activate this code START
                                Dim myAnalyzerID As String = String.Empty
                                Dim myWorkSessionID As String = String.Empty

                                If (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms _
                                    Where Not a.IsAnalyzerIDNull Select a.AnalyzerID Distinct).ToList.Count > 0 Then
                                    myAnalyzerID = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms _
                                                    Where Not a.IsAnalyzerIDNull Select a.AnalyzerID Distinct).First.ToString
                                End If

                                If (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms _
                                    Where Not a.IsWorkSessionIDNull Select a.WorkSessionID Distinct).ToList.Count > 0 Then
                                    myWorkSessionID = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms _
                                                    Where Not a.IsWorkSessionIDNull Select a.WorkSessionID Distinct).First.ToString
                                End If
                                'AG 23/11/2012 - activate this code END
                                'DL 28/11/2012. BUG & TRACKING: 927. When alarms solved its appear in gray color but duplicate. END ¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡

                                'Search for current active alarms (by AnalyzerID, WorkSessionID) and load them into currentActiveAlarms
                                resultData = myDAO.GetCurrentActiveAlarms(dbConnection, myAnalyzerID, myWorkSessionID)
                                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                                    Dim currentActiveAlarms As WSAnalyzerAlarmsDS = DirectCast(resultData.SetDatos, WSAnalyzerAlarmsDS)

                                    Dim myAlarmID As String = String.Empty
                                    For Each alarmID As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms
                                        Dim existAlarm As List(Of WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow)
                                        myAlarmID = alarmID.AlarmID.ToString

                                        'Search if exist into currentActiveAlarms
                                        existAlarm = (From a As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In currentActiveAlarms.twksWSAnalyzerAlarms _
                                                      Where a.AlarmID = myAlarmID Select a Order By a.AlarmDateTime Descending).ToList

                                        If (existAlarm.Count > 0) Then
                                            'Case A: If last (most recent) alarm in database is ACTIVE ALARM (alarm status = TRUE) -> Remove from DS and not insert (Do not duplicate, not insert again)
                                            If alarmID.AlarmStatus AndAlso existAlarm.First.AlarmStatus Then
                                                'Exception: Alarms with OKType = False
                                                If Not alarmsWithOKTypeFalse.Contains(alarmID.AlarmID.ToString) Then
                                                    alarmID.Delete() 'remove from pAnalyzerAlarmsDS
                                                End If

                                            ElseIf Not alarmID.AlarmStatus Then
                                                'Case B: If last (most recent) alarm in database is SOLVED ALARM (alarm status = FALSE) -> Remove from DS and not update (it has been already updated)
                                                If Not existAlarm.First.AlarmStatus Then
                                                    alarmID.Delete() 'remove from pAnalyzerAlarmsDS

                                                    'Case C: If last (most recent) alarm in database is ACTIVE ALARM (alarm status = TRUE) and now is SOLVED -> Update the alarmStatus and OKDateTime column (mark as not ACTIVE alarm)
                                                Else

                                                    'Inform on the updateAnalyzerAlarmsDS the solved alarm
                                                    existAlarm.First.BeginEdit()
                                                    existAlarm.First.AlarmStatus = False 'Alarm leave to be active
                                                    existAlarm.First.OKDateTime = Now 'DateTime when alarm leave to be active
                                                    existAlarm.First.EndEdit()
                                                    updateAnalyzerAlarmsDS.twksWSAnalyzerAlarms.ImportRow(existAlarm.First)

                                                    alarmID.Delete() 'remove from pAnalyzerAlarmsDS
                                                End If
                                            End If

                                        End If

                                        existAlarm = Nothing
                                    Next
                                End If

                                pAnalyzerAlarmsDS.AcceptChanges()
                                updateAnalyzerAlarmsDS.AcceptChanges()
                                alarmsWithOKTypeFalse = Nothing
                            End If

                            'AG 06/06/2012

                            If pAnalyzerAlarmsDS.twksWSAnalyzerAlarms.Rows.Count > 0 Then

                                'AG 30/05/2012 - look for WorkSessionID = NULL and update DS to current work session
                                Dim localAnalyzerID As String = ""
                                If Not pAnalyzerAlarmsDS.twksWSAnalyzerAlarms(0).IsAnalyzerIDNull Then
                                    localAnalyzerID = pAnalyzerAlarmsDS.twksWSAnalyzerAlarms(0).AnalyzerID
                                End If

                                Dim wsAnalyzerDelg As New WSAnalyzersDelegate
                                Dim myCurrentWS As String = ""
                                resultData = wsAnalyzerDelg.GetActiveWSByAnalyzer(dbConnection, localAnalyzerID)
                                If Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing Then
                                    Dim localWsAnalyzerDS As New WSAnalyzersDS
                                    localWsAnalyzerDS = CType(resultData.SetDatos, WSAnalyzersDS)
                                    If localWsAnalyzerDS.twksWSAnalyzers.Rows.Count > 0 Then
                                        If Not localWsAnalyzerDS.twksWSAnalyzers(0).IsWorkSessionIDNull Then
                                            myCurrentWS = localWsAnalyzerDS.twksWSAnalyzers(0).WorkSessionID
                                        End If
                                    End If
                                End If

                                If myCurrentWS <> "" Then
                                    For Each item As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms
                                        If item.IsWorkSessionIDNull Then
                                            item.BeginEdit()
                                            item.WorkSessionID = myCurrentWS
                                            item.EndEdit()
                                        End If
                                    Next
                                    pAnalyzerAlarmsDS.AcceptChanges()
                                End If
                                'AG 30/05/2012

                                resultData = myDAO.Create(dbConnection, pAnalyzerAlarmsDS)
                            End If
                            'AG 12/12/2011

                            If Not resultData.HasError AndAlso updateAnalyzerAlarmsDS.twksWSAnalyzerAlarms.Rows.Count > 0 Then
                                resultData = myDAO.Update(dbConnection, updateAnalyzerAlarmsDS)
                            End If

                            distinctAlarms = Nothing
                            duplicatedAlarms = Nothing


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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.Save", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create Analyzer alarm into twksWSAnalyzerAlarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerAlarmsDS">Typed DataSet WSAnalyzerAlarmsDS containing the group of alarms to insert</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 23/07/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerAlarmsDS As WSAnalyzerAlarmsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDao As New twksWSAnalyzersAlarmsDAO
                        resultData = myDao.Create(dbConnection, pAnalyzerAlarmsDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the twksWSAnalyzerAlarms (not primary key)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerAlarmsDS">Typed DataSet WSAnalyzerAlarmsDS containing the group of alarms to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 23/07/2012
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerAlarmsDS As WSAnalyzerAlarmsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDao As New twksWSAnalyzersAlarmsDAO
                        resultData = myDao.Update(dbConnection, pAnalyzerAlarmsDS)

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
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read from table twksWSAnalyzerAlarms by WorkSessionID and optionally, by AnalyzerID 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAnalyzerAlarmsDS with the list of Alarms returned</returns>
        ''' <remarks>
        ''' Created by:  TR 15/03/2011
        ''' </remarks>
        Public Function GetByWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                Optional ByVal pAnalyzerID As String = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSAnalyzersAlarmsDAO
                        resultData = myDAO.GetByWS(dbConnection, pWorkSessionID, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.GetByWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read from table twksWSAnalyzerAlarms by AnalyzerID and optionally, by WorkSessionID 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAnalyzerAlarmsDS with the list of Alarms returned</returns>
        ''' <remarks>
        ''' Created by:  TR 15/03/2011
        ''' </remarks>
        Public Function GetByAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                      Optional ByVal pWorkSessionID As String = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSAnalyzersAlarmsDAO
                        resultData = myDAO.GetByAnalyzer(dbConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.GetByAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read form table twksWSAnalyzerAlarms by AlarmDateTime and optionally, by AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pInitialDate">Initial Alarm Date(From)</param>
        ''' <param name="pFinalDate">Final Alarm Date (To)</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAnalyzerAlarmsDS with the list of Alarms returned</returns>
        ''' <remarks>
        ''' Created by:  TR 16/03/2011
        ''' </remarks>
        Public Function GetByTime(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pInitialDate As Date, _
                                  ByVal pFinalDate As Date, Optional ByVal pAnalyzerID As String = Nothing) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSAnalyzersAlarmsDAO
                        resultData = myDAO.GetByTime(dbConnection, pInitialDate, pFinalDate, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.GetByTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read from table twksWSAnalyzerAlarms by AlarmID and optionally by AlarmDateTime (InitialDate, FinalDate), 
        ''' AnalyzerID, and/or WorksessionID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAlarmID">Alarm Identifier</param>
        ''' <param name="pInitialDate">Initial Alarm Date(FROM)</param>
        ''' <param name="pFinalDate">Final Alarm Date (TO)</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAnalyzerAlarmsDS with the list of Alarms returned</returns>
        ''' <remarks>
        ''' Created by: TR 15/03/2011
        ''' </remarks>
        Public Function GetByAlarmID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmID As String, _
                                     Optional ByVal pInitialDate As Date = Nothing, Optional ByVal pFinalDate As Date = Nothing, _
                                     Optional ByVal pAnalyzerID As String = Nothing, Optional ByVal pWorkSessionID As String = Nothing) _
                                     As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSAnalyzersAlarmsDAO
                        resultData = myDAO.GetByAlarmID(dbConnection, pAlarmID, pInitialDate, pFinalDate, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.GetByAlarmID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Reads all Analyzer Alarms from view vwksAlarmsMonitor  
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAnalyzerAlarmsDS with the list of Alarms returned</returns>
        ''' <remarks>
        ''' Created by:  RH 11/04/2011
        ''' Modified by: AG 17/10/2011 - Added parameter pAnalyzerID 
        '''              SA 10/02/2014 - BT #1496 ==> Added optional parameter pWorkSessionID: when it is informed, besides by AnalyzerID,
        '''                                           Alarms are filtered also by WorkSessionID
        ''' </remarks>
        Public Function GetAlarmsMonitor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSAnalyzersAlarmsDAO
                        resultData = myDAO.GetAlarmsMonitor(dbConnection, pAnalyzerID, pWorkSessionID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.GetAlarmsMonitor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created By: GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSAnalyzersAlarmsDAO
                        resultData = myDAO.ResetWS(dbConnection, pAnalyzerID, pWorkSessionID)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' NOTE: This method do not use connection. All information is already into pAddtionalInfo
        ''' 
        ''' By now (june 2011) only the prep locked alarm generates additional info but in future must be more
        ''' </summary>
        ''' <param name="pAlarmID"></param>
        ''' <param name="pAdditionalInfo"></param>
        ''' <returns>GlobalDataTo (Data is a DataSET that depends on the pAlarmID)</returns>
        ''' <remarks>
        ''' AG 14/06/2011 creation - Testing PENDING
        ''' Modified by: RH 30/01/2012 New version
        ''' AG 15/01/2015 BA-2236 (6) add comments
        ''' </remarks>
        Public Function DecodeAdditionalInfo(ByVal pAlarmID As String, ByVal pAdditionalInfo As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                Dim Separators() As String = {GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR}
                Dim Info() As String
                Dim ColumnsInTable As Integer = 10 'NOTE: Update each time a new column is added!!!

                Info = pAdditionalInfo.Split(Separators, StringSplitOptions.None)

                'AG 15/01/2015 BA-2236 (6) - COMMENTS about existing code!!
                'This method is used for decode the current WS alarms and also the historicls
                'Current code works OK but if in the future there are new SPECS that implies use different number of separators
                'Could generate problems for show information in historics alarms generated in previous versions because they have number of separators different
                'AG 15/01/2015

                If Info.Length = ColumnsInTable Then
                    Dim additionalInfoDS As New WSAnalyzerAlarmsDS

                    additionalInfoDS.AdditionalInfoPrepLocked.AddAdditionalInfoPrepLockedRow( _
                            Info(0), Info(1), Info(2), Info(3), Info(4), Info(5), Info(6), Info(7), Info(8), Info(9))

                    resultData.SetDatos = additionalInfoDS
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.DecodeAdditionalInfo", EventLogEntryType.Error, False)

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="pAlarmID"></param>
        ''' <param name="pAdditionalInfo"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns></returns>
        ''' <remarks>Created by SGM 30/07/2012</remarks>
        Public Function DecodeISEAdditionalInfo(ByVal pAlarmID As String, ByVal pAdditionalInfo As String, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                Dim myAlarmsDelegate As New AlarmsDelegate

                Dim Separators() As String = {GlobalConstants.ADDITIONALINFO_ALARM_SEPARATOR}
                Dim Info() As String

                Info = pAdditionalInfo.Split(Separators, StringSplitOptions.None)

                Dim additionalInfoDS As New WSAnalyzerAlarmsDS

                'create connection
                Dim dbConnection As SqlClient.SqlConnection

                Dim myAlarmMessage As String = ""

                For i As Integer = 0 To Info.Count - 1 Step 1
                    If Info(i).Trim.Length > 0 Then
                        Dim InfoDetails() As String
                        InfoDetails = Info(i).Split(CChar(":"))
                        If InfoDetails.Count = 3 Then
                            Dim myCycle As String = InfoDetails(0).Trim
                            Dim myDetail As String = ""
                            Dim myError As String = InfoDetails(1).Trim
                            If myError.Length > 1 Then
                                If myError.StartsWith("R") Then
                                    Dim myISEErrorCode As ISEErrorTO.ISEResultErrorCodes = ISEErrorTO.ISEResultErrorCodes.None
                                    myError = myError.Substring(1)
                                    Dim myRemark As Alarms
                                    myISEErrorCode = CType(CInt(myError), ISEErrorTO.ISEResultErrorCodes)
                                    If CInt(myISEErrorCode) > 1 Then
                                        Select Case myISEErrorCode
                                            Case ISEErrorTO.ISEResultErrorCodes.Drift_CalASample : myRemark = Alarms.ISE_Drift_SER
                                            Case ISEErrorTO.ISEResultErrorCodes.mvNoise_CalBSample_CalBUrine : myRemark = Alarms.ISE_mVNoiseB_URI
                                            Case ISEErrorTO.ISEResultErrorCodes.mvNoise_CalBSample : myRemark = Alarms.ISE_mVNoiseA_SER
                                            Case ISEErrorTO.ISEResultErrorCodes.mvOut_CalASample_CalBUrine : myRemark = Alarms.ISE_mVOutB_URI
                                            Case ISEErrorTO.ISEResultErrorCodes.mvOut_CalBSample : myRemark = Alarms.ISE_mVOutA_SER
                                            Case ISEErrorTO.ISEResultErrorCodes.OutOfSlope_MachineRanges : myRemark = Alarms.ISE_OutSlope
                                        End Select
                                    End If

                                    Dim myAlarms As New AlarmsDS
                                    resultData = myAlarmsDelegate.Read(dbConnection, myRemark.ToString)
                                    If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                                        myAlarms = CType(resultData.SetDatos, AlarmsDS)
                                        If myAlarms.tfmwAlarms.Count > 0 Then
                                            myAlarmMessage = myAlarms.tfmwAlarms(0).Name
                                            myDetail = InfoDetails(2).Trim
                                        End If
                                    End If
                                ElseIf myError.StartsWith("E") Then
                                    Dim myISEErrorCode As ISEErrorTO.ISECancelErrorCodes = ISEErrorTO.ISECancelErrorCodes.None
                                    myError = myError.Substring(1)
                                    Dim myAlarm As Alarms
                                    myISEErrorCode = CType(CInt(myError), ISEErrorTO.ISECancelErrorCodes)
                                    If CInt(myISEErrorCode) > 1 Then
                                        Select Case myISEErrorCode
                                            Case ISEErrorTO.ISECancelErrorCodes.A : myAlarm = Alarms.ISE_ERROR_A
                                            Case ISEErrorTO.ISECancelErrorCodes.B : myAlarm = Alarms.ISE_ERROR_B
                                            Case ISEErrorTO.ISECancelErrorCodes.C : myAlarm = Alarms.ISE_ERROR_C
                                            Case ISEErrorTO.ISECancelErrorCodes.D : myAlarm = Alarms.ISE_ERROR_D
                                            Case ISEErrorTO.ISECancelErrorCodes.F : myAlarm = Alarms.ISE_ERROR_F
                                            Case ISEErrorTO.ISECancelErrorCodes.M : myAlarm = Alarms.ISE_ERROR_M
                                            Case ISEErrorTO.ISECancelErrorCodes.N : myAlarm = Alarms.ISE_ERROR_N
                                            Case ISEErrorTO.ISECancelErrorCodes.P : myAlarm = Alarms.ISE_ERROR_P
                                            Case ISEErrorTO.ISECancelErrorCodes.R : myAlarm = Alarms.ISE_ERROR_R
                                            Case ISEErrorTO.ISECancelErrorCodes.S : myAlarm = Alarms.ISE_ERROR_S
                                            Case ISEErrorTO.ISECancelErrorCodes.T : myAlarm = Alarms.ISE_ERROR_T
                                            Case ISEErrorTO.ISECancelErrorCodes.W : myAlarm = Alarms.ISE_ERROR_W
                                        End Select
                                    End If

                                    Dim myAlarms As New AlarmsDS
                                    resultData = myAlarmsDelegate.Read(dbConnection, myAlarm.ToString)
                                    If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
                                        myAlarms = CType(resultData.SetDatos, AlarmsDS)
                                        If myAlarms.tfmwAlarms.Count > 0 Then
                                            myCycle &= ": " & myAlarms.tfmwAlarms(0).Name
                                            myDetail = ""
                                        End If
                                    End If
                                End If
                            End If

                            If myCycle.Length > 0 Then

                                Dim myRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow = additionalInfoDS.twksWSAnalyzerAlarms.NewtwksWSAnalyzerAlarmsRow
                                With myRow
                                    .BeginEdit()
                                    .AlarmID = myCycle
                                    .AnalyzerID = pAnalyzerID
                                    .AlarmDateTime = DateTime.Now
                                    .AlarmItem = 1
                                    .AlarmStatus = True
                                    .AdditionalInfo = myAlarmMessage & " " & InfoDetails(2).Trim
                                    .EndEdit()
                                End With

                                additionalInfoDS.twksWSAnalyzerAlarms.AddtwksWSAnalyzerAlarmsRow(myRow)
                                additionalInfoDS.twksWSAnalyzerAlarms.AcceptChanges()
                            End If

                        End If

                    End If
                Next


                resultData.SetDatos = additionalInfoDS


            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSAnalyzerAlarmsDelegate.DecodeISEAdditionalInfo", EventLogEntryType.Error, False)

            End Try

            Return resultData
        End Function

#End Region
    End Class
End Namespace
