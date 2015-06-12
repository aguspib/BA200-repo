Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Text
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tfmwAlarmsDAO
          

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
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        'cmdText = " SELECT * " & _
                        '          " FROM   tfmwAlarms " & _
                        '          " WHERE  AlarmID = '" & pAlarmID & "'"
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

                'Dim myLogAcciones As New ApplicationLogManager()
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
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        'cmdText &= "SELECT * " & vbCrLf
                        'cmdText &= "FROM tfmwAlarms " & vbCrLf

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

                'Dim myLogAcciones As New ApplicationLogManager()
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
                                               ByVal pLanguageID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    ''Get the connected Username from the current Application Session
                    ''Dim currentSession As New GlobalBase
                    'Dim currentUser As String = GlobalBase.GetSessionInfo().UserName.Trim.ToString
                    'Dim currentDateTime As Date = Now

                    resultData = MyClass.ReadAll(pDBConnection)
                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                        Dim myAlarmsDS As AlarmsDS = DirectCast(resultData.SetDatos, AlarmsDS)
                        Dim myAffectedRecords As Integer = 0

                        'Dim cmdText As String = ""
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

                                'Dim dbCmd As New SqlClient.SqlCommand
                                'dbCmd.Connection = pDBConnection
                                'dbCmd.CommandText = cmdText
                                'myAffectedRecords += dbCmd.ExecuteNonQuery()
                                SB.Append(vbNewLine)
                            End If
                            'cmdText &= vbNewLine

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

                                'Dim dbCmd As New SqlClient.SqlCommand
                                'dbCmd.Connection = pDBConnection
                                'dbCmd.CommandText = cmdText
                                'myAffectedRecords += dbCmd.ExecuteNonQuery()
                                'SB.Append(vbNewLine)
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

                                'Dim dbCmd As New SqlClient.SqlCommand
                                'dbCmd.Connection = pDBConnection
                                'dbCmd.CommandText = cmdText
                                'myAffectedRecords += dbCmd.ExecuteNonQuery()
                                SB.Append(vbNewLine)
                            End If
                            SB.Append(vbNewLine)

                            If (myAffectedRecords > 0) Then
                                resultData.AffectedRecords += 1
                            End If

                            'If resultData.AffectedRecords > 0 Then
                            '    resultData.HasError = False
                            'Else
                            '    resultData.HasError = True
                            '    resultData.AffectedRecords = 0
                            '    Exit For
                            'End If
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

                'Dim myLogAcciones As New ApplicationLogManager()
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
        Public Function ReadManagementAlarm(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarmID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        ' XBC 07/11/2012
                        'cmdText = " SELECT AlarmID, ErrorCode, ManagementID " & vbCrLf
                        'cmdText &= " FROM tfmwAlarmErrorCodes " & vbCrLf
                        'cmdText &= " WHERE ErrorCode = '" & pAlarmID & "'"

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwAlarmsDAO.ReadManagementAlarm", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "DO NOT USED, NOT UPDATED - These methods are not needed for Preloaded tables"
        ''' <summary>
        ''' Create a new Alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAlarm">Typed DataSet AlarmsDS with data of the Alarm to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AlarmsDS with data of the specified
        '''          Alarm</returns>
        ''' <remarks>
        ''' Created by:  SG 03/06/2010
        ''' Modified by : XBC 18/10/2010 - adding new fields
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarm As AlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (IsNothing(pDBConnection)) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"
                Else

                    For Each AlarmsRow As AlarmsDS.tfmwAlarmsRow In pAlarm.tfmwAlarms
                        Dim cmdText As String = ""

                        cmdText = " INSERT INTO tfmwAlarms(AlarmID, AlarmSource, AlarmType, Name, NameResourceID, Description,  " & _
                                  "                         DescResourceID, Solution, SolResourceID) " & _
                                  " VALUES('" & AlarmsRow.AlarmID.Trim & "', " & _
                                         " '" & AlarmsRow.AlarmSource.Trim & "', " & _
                                         " '" & AlarmsRow.AlarmType.Trim & "', " & _
                                         " N'" & AlarmsRow.Name.Trim.Replace("'", "''") & "', "

                        If AlarmsRow.IsNameResourceIDNull Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= " N'" & AlarmsRow.NameResourceID.Trim.Replace("'", "''") & "', "
                        End If

                        If AlarmsRow.IsDescriptionNull Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= " N'" & AlarmsRow.Description.Trim.Replace("'", "''") & "', "
                        End If

                        cmdText &= " N'" & AlarmsRow.DescResourceID.Trim.Replace("'", "''") & "', "

                        If AlarmsRow.IsSolutionNull Then
                            cmdText &= "NULL, "
                        Else
                            cmdText &= " N'" & AlarmsRow.Solution.Trim.Replace("'", "''") & "', "
                        End If

                        If AlarmsRow.IsSolResourceIDNull Then
                            cmdText &= "NULL) "
                        Else
                            cmdText &= " N'" & AlarmsRow.SolResourceID.Trim.Replace("'", "''") & "') "
                        End If


                        'Execute the SQL Sentence
                        Dim dbCmd As New SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords += dbCmd.ExecuteNonQuery
                        If (resultData.AffectedRecords > 0) Then
                            resultData.HasError = False
                            resultData.SetDatos = pAlarm.Clone
                        Else
                            resultData.HasError = True
                            resultData.ErrorCode = "SYSTEM_ERROR"
                        End If

                    Next
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwAlarmsDAO.Create", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Modify an alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAlarm">Typed DataSet AlarmsDS with data of the Alarm to update</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AlarmsDS with data of the specified
        '''          Alarm</returns>
        ''' <remarks>
        ''' Created by:  SG 03/06/2010
        ''' Modified by : XBC 18/10/2010 - adding new fields
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarm As AlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"
                Else
                    'Get the connected Username from the current Application Session
                    'Dim currentSession As New GlobalBase
                    Dim currentUser As String = GlobalBase.GetSessionInfo().UserName.Trim.ToString
                    Dim currentDateTime As Date = Now

                    For Each AlarmsRow As AlarmsDS.tfmwAlarmsRow In pAlarm.tfmwAlarms
                        Dim cmdText As String = ""

                        cmdText &= "UPDATE tfmwAlarms " & vbCrLf
                        cmdText &= "SET    AlarmSource = '" & AlarmsRow.AlarmSource.ToString.Replace("'", "''") & "'" & vbCrLf
                        cmdText &= "     , AlarmType   = '" & AlarmsRow.AlarmType.ToString & "'" & vbCrLf
                        cmdText &= "     , Name        = N'" & AlarmsRow.Name.ToString.Replace("'", "''") & "'" & vbCrLf

                        If AlarmsRow.IsNameResourceIDNull Then
                            cmdText &= " , NameResourceID = NULL"
                        Else
                            cmdText &= " , NameResourceID = N'" & AlarmsRow.NameResourceID.Trim.Replace("'", "''") & "'"
                        End If

                        If AlarmsRow.IsDescriptionNull Then
                            cmdText &= " , Description = NULL"
                        Else
                            cmdText &= " , Description = N'" & AlarmsRow.Description.Trim.Replace("'", "''") & "'"
                        End If

                        cmdText &= "     , DescResourceID = N'" & AlarmsRow.DescResourceID.ToString.Replace("'", "''") & "'" & vbCrLf

                        If AlarmsRow.IsSolutionNull Then
                            cmdText &= " , Solution = NULL"
                        Else
                            cmdText &= " , Solution = N'" & AlarmsRow.Solution.Trim.Replace("'", "''") & "'"
                        End If

                        If AlarmsRow.IsSolResourceIDNull Then
                            cmdText &= " , SolResourceID = NULL"
                        Else
                            cmdText &= " , SolResourceID = N'" & AlarmsRow.SolResourceID.Trim.Replace("'", "''") & "'"
                        End If

                        cmdText &= "    WHERE AlarmID = '" & AlarmsRow.AlarmID & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords += dbCmd.ExecuteNonQuery()

                        If resultData.AffectedRecords > 0 Then
                            resultData.HasError = False
                            resultData.SetDatos = pAlarm
                        Else
                            resultData.HasError = True
                            resultData.AffectedRecords = 0
                        End If

                    Next
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwAlarmsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Remove an alarm
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAlarm">Typed DataSet AlarmsDS with data of the Alarm to delete</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AlarmsDS with data of the specified
        '''          Alarm</returns>
        ''' <remarks>Created by:  SG 03/06/2010</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAlarm As AlarmsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = "DB_CONNECTION_ERROR"
                Else

                    For Each AlarmsRow As AlarmsDS.tfmwAlarmsRow In pAlarm.tfmwAlarms

                        Dim cmdText As String
                        cmdText = " DELETE FROM tfmwAlarms " & vbCrLf & _
                                  " WHERE  AlarmID = '" & AlarmsRow.AlarmID & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                        resultData.HasError = False

                    Next
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tfmwAlarmsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


#End Region

    End Class
End Namespace


