Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class thisWSAnalyzerAlarmsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Create Analyzer alarms into thisWSAnalyzerAlarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisAnalyzerAlarmsDS">Typed DataSet HisWSAnalyzerAlarmsDS containing the group of Alarms to insert</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Createb by:  JB 19/09/2012
        ''' Modified by: TR 26/09/2012 - Fixed an error on the SQL Sentence (there was a comma in the last added value)
        '''              JB 09/11/2012 - Fixed copy/paste error: when verifying if field WorkSession has to be informed, instead of verifying 
        '''                              if field WorkSessionID in the DS is NULL, it was verified if field AdditionalInfo was NULL 
        '''              SA 12/11/2012 - If an error for PK Violation is raised, ignore it (no Alarms will be moved to Historic Module)
        '''              SA 10/02/2014 - BT 1496 ==> When an error for PK Violation is raised, do not write it in the Application Log 
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisAnalyzerAlarmsDS As HisWSAnalyzerAlarmsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    Dim keys As String = " (AlarmID, AnalyzerID, AlarmDateTime, AlarmItem, AlarmType, WorkSessionID, AdditionalInfo, AlarmStatus, OKDateTime) "
                    Dim values As String = String.Empty

                    For Each hisAnalyzerAlarmRow As HisWSAnalyzerAlarmsDS.thisWSAnalyzerAlarmsRow In pHisAnalyzerAlarmsDS.thisWSAnalyzerAlarms.Rows
                        With hisAnalyzerAlarmRow
                            values = String.Empty
                            values &= " '" & .AlarmID.Trim & "', "
                            values &= " '" & .AnalyzerID.Trim & "', "
                            values &= " '" & .AlarmDateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                            values &= " '" & .AlarmItem & "', "
                            values &= " '" & .AlarmType.Trim & "', "
                            If (.IsWorkSessionIDNull) Then values &= " NULL, " Else values &= " '" & .WorkSessionID.Trim & "', "
                            If (.IsAdditionalInfoNull) Then values &= " NULL, " Else values &= " '" & .AdditionalInfo.Trim & "', "
                            values &= " " & CStr(IIf(hisAnalyzerAlarmRow.AlarmStatus, 1, 0)) & ", "
                            If (.IsOKDateTimeNull OrElse .AlarmStatus) Then values &= " NULL " Else values &= " '" & .OKDateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                        End With

                        cmdText &= "INSERT INTO thisWSAnalyzerAlarms  " & keys & " VALUES (" & values & ") " & vbNewLine
                    Next

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If

            Catch mySQLError As SqlException
                If (mySQLError.Number = 2627) Then
                    'Ignore errors in INSERT sentences for Alarms
                    myGlobalDataTO.HasError = False
                Else
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    myGlobalDataTO.ErrorMessage = mySQLError.Message

                    Dim myLogAcciones As New ApplicationLogManager()
                    myLogAcciones.CreateLogActivity(mySQLError.Message, "thisWSAnalyzersAlarmsDAO.Create", EventLogEntryType.Error, False)
                End If
                Exit Try

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisWSAnalyzersAlarmsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete the Analyzer alarm from thisWSAnalyzerAlarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisAnalyzerAlarmDS">Typed DataSet HisWSAnalyzerAlarmsDS containing the group of Alarms to delete</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: JB 18/09/2012
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisAnalyzerAlarmDS As HisWSAnalyzerAlarmsDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdWhere As String = String.Empty
                    For Each hisWSAnalyzerAlarmsRow As HisWSAnalyzerAlarmsDS.thisWSAnalyzerAlarmsRow In pHisAnalyzerAlarmDS.thisWSAnalyzerAlarms
                        If (Not String.IsNullOrEmpty(cmdWhere)) Then cmdWhere &= " OR "
                        With hisWSAnalyzerAlarmsRow
                            cmdWhere &= " ( "
                            cmdWhere &= " AlarmID = '" & .AlarmID.Trim & "'"
                            cmdWhere &= " AND AnalyzerID = '" & .AnalyzerID.Trim & "'"
                            cmdWhere &= " AND AlarmDateTime = '" & .AlarmDateTime.ToString("yyyyMMdd hh:mm:ss") & "'"
                            cmdWhere &= " AND AlarmItem = " & .AlarmItem.ToString
                            cmdWhere &= " ) "
                        End With
                    Next

                    Dim cmdText As String = " DELETE thisWSAnalyzerAlarms WHERE " & cmdWhere
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisWSAnalyzerAlarmsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all alarms for the specified AnalyzerID / WorkSessionID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2013
        ''' </remarks>
        Public Function DeleteByAnalyzerWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM thisWSAnalyzerAlarms " & vbCrLf & _
                                            " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf 

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisWSAnalyzerAlarmsDAO.DeleteByAnalyzerWS", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get all Alarms in table thisWSAnalyzerAlarms that fulfill the informed search criteria (if any)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier. Optional parameter</param>
        ''' <param name="pDateFrom">Initial Alarm date. Optional parameter</param>
        ''' <param name="pDateTo">Final Alarm date. Optional parameter</param>
        ''' <param name="pAlarmType">Alarm Type. Optional parameter</param>
        ''' <param name="pWorkSessionID">Work Session Identifier. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSAnalyzerAlarmsDS with all alarms saved in Historic
        '''          Module that fulfill the specified search criteria</returns>
        ''' <remarks>
        ''' Created by: JB 19/09/2012
        ''' </remarks>
        Public Function GetAlarmsMonitorByFilter(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pAnalyzerID As String = "", _
                                                 Optional ByVal pDateFrom As Date = Nothing, Optional ByVal pDateTo As Date = Nothing, _
                                                 Optional ByVal pAlarmType As String = "", Optional ByVal pWorkSessionID As String = "") _
                                                 As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT H.AlarmID, H.AnalyzerID, H.AlarmDateTime, H.AlarmItem, A.AlarmType, H.WorkSessionID, " & vbCrLf & _
                                                       " H.AdditionalInfo, H.AlarmStatus, A.AlarmSource, A.Name, A.Description, A.Solution, H.OKDateTime " & vbCrLf & _
                                                " FROM   thisWSAnalyzerAlarms H INNER JOIN tfmwAlarms A ON H.AlarmID = A.AlarmID " & vbCrLf & _
                                                " WHERE  1 = 1 "

                        If (Not String.IsNullOrEmpty(pAnalyzerID)) Then cmdText &= " AND H.AnalyzerID = '" & pAnalyzerID.Trim & "' "
                        If (pDateFrom <> Nothing) Then cmdText &= " AND H.AlarmDateTime >= '" & pDateFrom.ToString("yyyyMMdd 00:00:00") & "' "
                        If (pDateTo <> Nothing) Then cmdText &= " AND H.AlarmDateTime <= '" & pDateTo.ToString("yyyyMMdd 00:00:00") & "' "
                        If (Not String.IsNullOrEmpty(pAlarmType)) Then cmdText &= " AND H.AlarmType = '" & pAlarmType.Trim & "' "
                        If (Not String.IsNullOrEmpty(pWorkSessionID)) Then cmdText &= " AND H.WorkSessionID = '" & pWorkSessionID.Trim & "' "
                        cmdText &= " ORDER BY H.AlarmDateTime DESC "

                        'Fill the DataSet to return 
                        Dim myHisWSAnalyzerAlarmsDS As New HisWSAnalyzerAlarmsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisWSAnalyzerAlarmsDS.vwksAlarmsMonitor)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHisWSAnalyzerAlarmsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisWSAnalyzerAlarmsDAO.GetAlarmsMonitorByFilter", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all Analyzers defined in the Historic Alarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an string list with the list of Analyzers</returns>
        ''' <remarks>
        ''' Created by:  IR 04/10/2012
        ''' </remarks>
        Public Function ReadAllDistinctAnalyzers(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT AnalyzerID " & vbCrLf & _
                                                " FROM   thisWSAnalyzerAlarms " & vbCrLf

                        Dim Ds As New DataSet()
                        Dim AllAnalyzers As New List(Of String)()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(Ds, "thisWSAnalyzerAlarms")
                            End Using
                        End Using

                        Dim dr As DataRow
                        For Each dr In Ds.Tables("thisWSAnalyzerAlarms").Rows
                            AllAnalyzers.Add(dr.Item("AnalyzerID").ToString)
                        Next

                        dataToReturn.SetDatos = AllAnalyzers
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisWSAnalyzerAlarmsDAO.ReadAllDistinctAnalyzers", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function
#End Region
    End Class
End Namespace