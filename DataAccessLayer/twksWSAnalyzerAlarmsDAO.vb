Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class twksWSAnalyzersAlarmsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Create Analyzer alarm into twksWSAnalyzerAlarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerAlarmsDS">Typed DataSet WSAnalyzerAlarmsDS containing the group of alarms to insert</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 15/03/2011
        ''' Modified by: AG 25/03/2011 - Added field AlarmStatus; allow NULL values in field WorkSessionID
        '''              AG 23/07/2012 - Added field OKDateTime
        '''              AG 04/12/2014 BA-2146 added field ErrorCode
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerAlarmsDS As WSAnalyzerAlarmsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim values As String = ""
                    Dim keys As String = "(AlarmID, AnalyzerID, AlarmDateTime, AlarmItem, WorkSessionID, AlarmStatus, OKDateTime, AdditionalInfo, ErrorCode)"

                    For Each AnalyzerAlarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms.Rows
                        values = ""
                        values = "'" & AnalyzerAlarmRow.AlarmID.Trim & "', "
                        values &= "'" & AnalyzerAlarmRow.AnalyzerID.Trim & "', "
                        values &= "'" & AnalyzerAlarmRow.AlarmDateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                        values &= AnalyzerAlarmRow.AlarmItem.ToString & ", "

                        If (AnalyzerAlarmRow.IsWorkSessionIDNull) Then
                            values &= " NULL, "
                        Else
                            values &= "'" & AnalyzerAlarmRow.WorkSessionID.Trim & "', "
                        End If

                        If (AnalyzerAlarmRow.IsAlarmStatusNull) Then
                            values &= " 1, " 'Default value for field AlarmStatus is True
                        Else
                            values &= " " & CStr(IIf(AnalyzerAlarmRow.AlarmStatus, 1, 0)) & ", "
                        End If

                        If (AnalyzerAlarmRow.IsOKDateTimeNull) Then
                            values &= " NULL, "
                        ElseIf (Not AnalyzerAlarmRow.IsAlarmStatusNull AndAlso AnalyzerAlarmRow.AlarmStatus) Then
                            values &= " NULL, "
                        Else
                            values &= " '" & AnalyzerAlarmRow.OKDateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                        End If

                        If (AnalyzerAlarmRow.IsAdditionalInfoNull) Then
                            values &= "NULL"
                        Else
                            values &= "'" & AnalyzerAlarmRow.AdditionalInfo.Trim & "'"
                        End If

                        'AG 04/12/2014 BA-2146 added column ErrorCode
                        If (AnalyzerAlarmRow.IsErrorCodeNull) Then
                            values &= ", NULL "
                        Else
                            values &= ", " & AnalyzerAlarmRow.ErrorCode.ToString & " "
                        End If
                        'AG 04/12/2014 BA-2146

                        cmdText &= "INSERT INTO twksWSAnalyzerAlarms  " & keys & " VALUES (" & values & ") " & vbNewLine
                    Next

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSAnalyzersAlarmsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
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
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    For Each AnalyzerAlarmRow As WSAnalyzerAlarmsDS.twksWSAnalyzerAlarmsRow In pAnalyzerAlarmsDS.twksWSAnalyzerAlarms.Rows
                        cmdText &= " UPDATE twksWSAnalyzerAlarms SET "

                        If (AnalyzerAlarmRow.IsWorkSessionIDNull) Then
                            cmdText &= " WorkSessionID = NULL, "
                        Else
                            cmdText &= " WorkSessionID = '" & AnalyzerAlarmRow.WorkSessionID.Trim & "', "
                        End If

                        If (AnalyzerAlarmRow.IsAlarmStatusNull) Then
                            cmdText &= " AlarmStatus = 1, " 'Default value for this field is TRUE
                        Else
                            cmdText &= " AlarmStatus = " & CStr(IIf(AnalyzerAlarmRow.AlarmStatus, 1, 0)) & ", "
                        End If

                        If (AnalyzerAlarmRow.IsOKDateTimeNull) Then
                            cmdText &= " OKDateTime = NULL, "
                        Else
                            cmdText &= " OKDateTime = '" & AnalyzerAlarmRow.OKDateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                        End If

                        If (AnalyzerAlarmRow.IsAdditionalInfoNull) Then
                            cmdText &= " AdditionalInfo = NULL "
                        Else
                            cmdText &= " AdditionalInfo = '" & AnalyzerAlarmRow.AdditionalInfo & "' "
                        End If

                        'Where (primary key)
                        cmdText &= " WHERE AlarmID = '" & AnalyzerAlarmRow.AlarmID.Trim & "' "
                        cmdText &= " AND   AnalyzerID = '" & AnalyzerAlarmRow.AnalyzerID.Trim & "' "
                        cmdText &= " AND   AlarmDateTime = '" & AnalyzerAlarmRow.AlarmDateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                        cmdText &= vbNewLine
                    Next

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSAnalyzersAlarmsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Read from table twksWSAnalyzerAlarms by WorkSessionID and optionally, by AnalyzerID 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier; optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAnalyzerAlarmsDS with the list of Alarms returned</returns>
        ''' <remarks>
        ''' Created by:  TR 15/03/2011
        ''' Modified by: AG 25/03/2011 - Added field AlarmStatus
        '''              AG 23/07/2012 - Added field OKDateTime
        ''' </remarks>
        Public Function GetByWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AlarmID, AnalyzerID, AlarmDateTime, AlarmItem, WorkSessionID, AdditionalInfo, " & vbCrLf & _
                                                       " AlarmStatus, OKDateTime " & vbCrLf & _
                                                " FROM   twksWSAnalyzerAlarms " & vbCrLf & _
                                                " WHERE  WorkSessionID = '" & pWorkSessionID.Trim & "' "
                        If (pAnalyzerID.Trim <> "") Then cmdText &= " AND AnalyzerID = '" & pAnalyzerID.Trim & "'"

                        Dim myWSAnalyzerAlamrsDS As New WSAnalyzerAlarmsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSAnalyzerAlamrsDS.twksWSAnalyzerAlarms)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myWSAnalyzerAlamrsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSAnalyzersAlarmsDAO.GetByWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
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
        ''' Modified by: AG 25/03/2011 - Added field AlarmStatus
        '''              AG 23/07/2012 - Added field OKDateTime
        ''' </remarks>
        Public Function GetByAnalyzer(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                      Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AlarmID, AnalyzerID, AlarmDateTime, AlarmItem, WorkSessionID, AdditionalInfo, " & vbCrLf & _
                                                      " AlarmStatus, OKDateTime " & vbCrLf & _
                                               " FROM   twksWSAnalyzerAlarms " & vbCrLf & _
                                               " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' "
                        If (pWorkSessionID.Trim <> "") Then cmdText &= " AND WorkSessionID = '" & pWorkSessionID.Trim & "'"

                        Dim myWSAnalyzerAlamrsDS As New WSAnalyzerAlarmsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSAnalyzerAlamrsDS.twksWSAnalyzerAlarms)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myWSAnalyzerAlamrsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSAnalyzersAlarmsDAO.GetByAnalyzer", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read from table twksWSAnalyzerAlarms by AnalyzerID and WorkSessionID all Alarms having the MAX DateTime
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSAnalyzerAlarmsDS with the list of Alarms returned</returns>
        ''' <remarks>
        ''' Created by:  DL 06/06/2012  
        ''' Modified by: AG 17/07/2012 - Changed the SQL 
        ''' </remarks>
        Public Function GetCurrentActiveAlarms(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                               ByVal pWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AlarmID, AlarmStatus, AnalyzerID, WorkSessionID, AlarmItem, AdditionalInfo, " & vbCrLf & _
                                                       " MAX(AlarmDateTime) As AlarmDateTime " & vbCrLf & _
                                                " FROM   twksWSAnalyzerAlarms " & vbCrLf & _
                                                " WHERE  AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                " GROUP BY AlarmID, AlarmStatus, AnalyzerID, WorkSessionID, AlarmItem, AdditionalInfo " & vbCrLf

                        Dim myWSAnalyzerAlamrsDS As New WSAnalyzerAlarmsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSAnalyzerAlamrsDS.twksWSAnalyzerAlarms)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myWSAnalyzerAlamrsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSAnalyzersAlarmsDAO.GetCurrentActiveAlarms", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
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
        ''' Modified by: AG 25/03/2011 - Added field AlarmStatus
        '''              AG 23/07/2012 - Added field OKDateTime
        ''' </remarks>
        Public Function GetByTime(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pInitialDate As Date, ByVal pFinalDate As Date, _
                                  Optional ByVal pAnalyzerID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AlarmID, AnalyzerID, AlarmDateTime, AlarmItem, WorkSessionID, AdditionalInfo, " & vbCrLf & _
                                                       " AlarmStatus, OKDateTime " & vbCrLf & _
                                                " FROM twksWSAnalyzerAlarms " & vbCrLf & _
                                                " WHERE AlarmDateTime >= '" & pInitialDate.ToString("yyyyMMdd 00:00:00") & "' " & vbCrLf & _
                                                " AND   AlarmDateTime <= '" & pFinalDate.ToString("yyyyMMdd 23:59:00") & "' " & vbCrLf

                        If (pAnalyzerID.Trim <> "") Then cmdText &= String.Format(" AND AnalyzerID = '{0}' ", pAnalyzerID.Trim)

                        Dim myWSAnalyzerAlamrsDS As New WSAnalyzerAlarmsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSAnalyzerAlamrsDS.twksWSAnalyzerAlarms)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myWSAnalyzerAlamrsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSAnalyzersAlarmsDAO.GetByTime", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
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
        ''' Modified by: AG 25/03/2011 - Added field AlarmStatus
        '''              AG 23/07/2012 - Added field OKDateTime
        ''' </remarks>
        Public Function GetByAlarmID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmID As String, _
                                     Optional ByVal pInitialDate As Date = Nothing, Optional ByVal pFinalDate As Date = Nothing, _
                                     Optional ByVal pAnalyzerID As String = "", Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AlarmID, AnalyzerID, AlarmDateTime, AlarmItem, WorkSessionID, AdditionalInfo, " & vbCrLf & _
                                                       " AlarmStatus, OKDateTime " & vbCrLf & _
                                                " FROM   twksWSAnalyzerAlarms " & vbCrLf & _
                                                String.Format(" WHERE AlarmID = '{0}' ", pAlarmID.Trim) & vbCrLf

                        If (Not pInitialDate = Nothing) Then
                            cmdText &= " AND AlarmDateTime >= '" & pInitialDate.ToString("yyyyMMdd 00:00:00") & "' "
                        End If

                        If (Not pFinalDate = Nothing) Then
                            cmdText &= " AND AlarmDateTime <= '" & pFinalDate.ToString("yyyyMMdd 23:59:00") & "' "
                        End If

                        If (pAnalyzerID.Trim <> "") Then
                            cmdText &= String.Format(" AND AnalyzerID = '{0}' ", pAnalyzerID.Trim)
                        End If

                        If (pWorkSessionID.Trim <> "") Then
                            cmdText &= " AND WorkSessionID = '" & pWorkSessionID.Trim & "' "
                        End If

                        Dim myWSAnalyzerAlamrsDS As New WSAnalyzerAlarmsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSAnalyzerAlamrsDS.twksWSAnalyzerAlarms)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myWSAnalyzerAlamrsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSAnalyzersAlarmsDAO.GetByAlarmID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
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
        '''              AG 07/02/2012 - Added field AlarmItem DESC to the ORDER BY 
        '''              AG 23/07/2012 - Added field OKDateTime
        '''              SA 23/10/2013 - BT #1355 ==> Added field AlarmPeriodSEC
        '''              SA 10/02/2014 - BT #1496 ==> Added optional parameter pWorkSessionID: when it is informed, besides by AnalyzerID,
        '''                                           Alarms are filtered also by WorkSessionID. Changed the SQL Query to apply the filter
        '''                                           when the optional parameter is informed
        '''              AG 04/12/2014 BA-2146 get also column ErrorCode form view
        ''' </remarks>
        Public Function GetAlarmsMonitor(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                         Optional ByVal pWorkSessionID As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AlarmID, AnalyzerID, AlarmDateTime, AlarmItem, WorkSessionID, AdditionalInfo, " & vbCrLf & _
                                                       " AlarmStatus, AlarmSource, AlarmType, Name, Description, Solution, OKDateTime, AlarmPeriodSEC, ErrorCode " & vbCrLf & _
                                                " FROM   vwksAlarmsMonitor " & vbCrLf & _
                                                " WHERE  AnalyzerID = " & String.Format("'{0}'", pAnalyzerID.Trim) & vbCrLf

                        'If optional parameter is informed, then apply the filter to get only Alarms of the active WorkSession
                        If (pWorkSessionID.Trim <> String.Empty) Then cmdText &= " AND WorkSessionID = " & String.Format("'{0}'", pWorkSessionID.Trim) & vbCrLf

                        'Finally, sort Alarms by DateTime and Item, both DESCENDING
                        cmdText &= " ORDER BY AlarmDateTime DESC, AlarmItem DESC " & vbCrLf

                        Dim myWSAnalyzerAlamrsDS As New WSAnalyzerAlarmsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myWSAnalyzerAlamrsDS.vwksAlarmsMonitor)
                            End Using
                        End Using

                        resultData.SetDatos = myWSAnalyzerAlamrsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSAnalyzersAlarmsDAO.GetAlarmsMonitor", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Alarms for the informed Analyzer
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:: GDS 21/04/2010
        ''' Modified by: SA 12/11/2012 - Changed the SQL to delete also Analyzer Alarms without WorkSessionID informed
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) _
                                As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE twksWSAnalyzerAlarms" & vbCrLf & _
                                            " WHERE  AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "'" & vbCrLf & _
                                            " AND   (WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " OR     WorkSessionID IS NULL) " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.SetDatos = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "twksWSAnalyzersAlarmsDAO.ResetWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace