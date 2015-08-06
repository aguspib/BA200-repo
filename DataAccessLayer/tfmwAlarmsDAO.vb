Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Text
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwAlarmsDAO
        Implements ItfmwAlarms

#Region "CRUD Methods"
        ''' <summary>
        ''' Get data from the specified Alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAlarmID">Identifier of the Alarm</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AlarmsDS with data of the specified Alarm</returns>
        ''' <remarks>
        ''' Created by:  SG 03/06/2010
        ''' AG 05/03/2012 - left outer join to get also the alarm error codes
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmID As String) As GlobalDataTO Implements ItfmwAlarms.Read
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT a.*, aec.ErrorCode " & vbCrLf
                        cmdText &= " FROM tfmwAlarms a LEFT OUTER JOIN tfmwAlarmErrorCodes aec " & vbCrLf
                        cmdText &= " ON a.AlarmID = aec.AlarmID " & vbCrLf
                        cmdText &= " WHERE a.AlarmID = '" & pAlarmID & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim Alarms As New AlarmsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(Alarms.tfmwAlarms)

                        resultData.SetDatos = Alarms
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "tfmwAlarmsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined Alarms
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AlarmsDS with the list of Alarms</returns>
        ''' <remarks>
        ''' Created by: XBC 18/10/2010
        ''' AG 05/03/2012 - left outer join to get also the alarm error codes
        ''' RH 20/03/2012 - Bug correction (returns SetDatos as SqlClient.SqlConnection in case of exception) and code optimization
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO Implements ItfmwAlarms.ReadAll
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        'AJG AÑADIDO EL ISNULL OJO!!!!!
                        cmdText = " SELECT a.*, ISNULL(aec.ErrorCode,0) AS ERRORCODE " & vbCrLf
                        cmdText &= " FROM tfmwAlarms a LEFT OUTER JOIN tfmwAlarmErrorCodes aec " & vbCrLf
                        cmdText &= " ON a.AlarmID = aec.AlarmID "

                        Dim myDS As New AlarmsDS

                        'Fill the DataSet to return
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDS.tfmwAlarms)
                            End Using
                        End Using

                        resultData.SetDatos = myDS
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "tfmwAlarmsDAO.ReadAll", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Update the description fields in Alarms table with the translated texts according to the language set
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLanguageID">Language Identifier to be translate</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 18/10/2010
        ''' Modified by: SA  26/10/2010 - Changed the WHERE of the three updates: it should be by the PK AlarmID
        '''                               Removed the HasError = TRUE when affected records is zero
        ''' AG 23/07/2012 - execute query one time not 3*N
        ''' </remarks>
        Public Function UpdateLanguageResource(ByVal pDBConnection As SqlClient.SqlConnection, _
                                               ByVal pLanguageID As String) As GlobalDataTO Implements ItfmwAlarms.UpdateLanguageResource
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    resultData = MyClass.ReadAll(pDBConnection)
                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                        Dim myAlarmsDS As AlarmsDS = DirectCast(resultData.SetDatos, AlarmsDS)
                        Dim myAffectedRecords As Integer = 0

                        Dim SB As New StringBuilder
                        For Each AlarmsRow As AlarmsDS.tfmwAlarmsRow In myAlarmsDS.tfmwAlarms.Rows

                            'Translate NAME fields
                            If (Not AlarmsRow.IsNameResourceIDNull) Then
                                SB.Append(" UPDATE tfmwAlarms SET [Name] = ")
                                SB.Append("(")
                                SB.Append("  SELECT ResourceText AS translatedText ")
                                SB.Append("  FROM   tfmwMultiLanguageResources ")
                                SB.Append("  WHERE  ResourceID = '")
                                SB.Append(AlarmsRow.NameResourceID)
                                SB.Append("' ")
                                SB.Append("  AND    LanguageID = '")
                                SB.Append(pLanguageID)
                                SB.Append("'"c)
                                SB.Append(")")
                                SB.Append(" WHERE AlarmID = '")
                                SB.Append(AlarmsRow.AlarmID)
                                SB.Append("' ")
                                SB.Append(vbNewLine)
                            End If

                            'Translate DESCRIPTION fields
                            If (Not AlarmsRow.IsDescResourceIDNull) Then
                                SB.Append("UPDATE tfmwAlarms SET Description = ")
                                SB.Append("(")
                                SB.Append("  SELECT ResourceText AS translatedText ")
                                SB.Append("  FROM   tfmwMultiLanguageResources ")
                                SB.Append("  WHERE  ResourceID = '")
                                SB.Append(AlarmsRow.DescResourceID)
                                SB.Append("' ")
                                SB.Append("  AND     LanguageID = '")
                                SB.Append(pLanguageID)
                                SB.Append("' ")
                                SB.Append(") ")
                                SB.Append(" WHERE AlarmID = '")
                                SB.Append(AlarmsRow.AlarmID)
                                SB.Append("' ")

                            End If
                            SB.Append(vbNewLine)

                            'Translate SOLUTION fields
                            If (Not AlarmsRow.IsSolResourceIDNull) Then
                                SB.Append("UPDATE tfmwAlarms SET Solution = ")
                                SB.Append("(")
                                SB.Append("  SELECT ResourceText AS translatedText ")
                                SB.Append("  FROM   tfmwMultiLanguageResources ")
                                SB.Append("  WHERE   ResourceID = '")
                                SB.Append(AlarmsRow.SolResourceID)
                                SB.Append("' ")
                                SB.Append("  AND     LanguageID = '")
                                SB.Append(pLanguageID)
                                SB.Append("' ")
                                SB.Append(")")
                                SB.Append(" WHERE AlarmID = '")
                                SB.Append(AlarmsRow.AlarmID)
                                SB.Append("' ")
                                SB.Append(vbNewLine)
                            End If
                            SB.Append(vbNewLine)

                            If (myAffectedRecords > 0) Then
                                resultData.AffectedRecords += 1
                            End If

                        Next

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = SB.ToString()
                        myAffectedRecords += dbCmd.ExecuteNonQuery()
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "tfmwAlarmsDAO.UpdateLanguageResource", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get management information from the specified Alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAlarmID">Identifier of the Alarm</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AlarmsDS with data of the specified Alarm</returns>
        ''' <remarks>
        ''' Created by XBC 16/10/2012
        ''' </remarks>
        Public Function ReadManagementAlarm(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmID As String) As GlobalDataTO Implements ItfmwAlarms.ReadManagementAlarm
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText = " SELECT TOP 1 ec.AlarmID as AlarmID, ec.ErrorCode as ErrorCode, ec.ManagementID as ManagementID, ml.ResourceID as ResourceID " & vbCrLf
                        cmdText &= " FROM tfmwAlarmErrorCodes as ec " & vbCrLf
                        cmdText &= " INNER JOIN tfmwAlarms as a ON ec.AlarmID = a.AlarmID " & vbCrLf
                        cmdText &= " LEFT OUTER JOIN tfmwMultiLanguageResources as ml ON a.DescResourceID = ml.ResourceID " & vbCrLf
                        cmdText &= " WHERE ErrorCode = '" & pAlarmID & "'"
                        ' XBC 07/11/2012

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim Alarms As New AlarmErrorCodesDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(Alarms.tfmwAlarmErrorCodes)

                        resultData.SetDatos = Alarms
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                GlobalBase.CreateLogActivity(ex.Message, "tfmwAlarmsDAO.ReadManagementAlarm", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

    End Class
End Namespace


