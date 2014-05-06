Option Strict On
Option Explicit On

'Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO


    Public Class tcfgAnalyzerSettingsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Get the current value of an specific Analyzer Setting
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pSettingID">Setting Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AnalyzerSettingsDS with the current value of the specified
        '''          Analyzer Setting</returns>
        ''' <remarks>
        ''' Created by:  DL 21/07/2011
        ''' Modified by: SA 01/08/2011 - Name changed to Read
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pSettingID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT AnalyzerID, SettingID, SettingDataType, CurrentValue " & vbCrLf & _
                                                " FROM   tcfgAnalyzerSettings " & vbCrLf & _
                                                " WHERE  AnalyzerID       = '" & pAnalyzerID & "' " & vbCrLf & _
                                                " AND    UPPER(SettingID) = UPPER('" & pSettingID & "') "

                        Dim myDataSet As New AnalyzerSettingsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.tcfgAnalyzerSettings)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerSettingsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        '''' <summary>
        '''' Update the value of an specific Analyzer Setting
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerSettingsRow">Row of a typed DataSet AnalyzerSettingsDS containing the data to update</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: DL 21/07/2011
        '''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            'TR 19/09/2012
            Dim GenSQLInstruction As String = ""
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = String.Empty

                    cmdText &= "UPDATE tcfgAnalyzerSettings " & vbCrLf
                    cmdText &= "   SET CurrentValue = N'" & pAnalyzerSettingsRow.CurrentValue.ToString().Replace("'", "''") & "' " & vbCrLf
                    cmdText &= " WHERE AnalyzerID   = N'" & pAnalyzerSettingsRow.AnalyzerID.ToString().Replace("'", "''") & "'" & vbCrLf
                    cmdText &= "   AND SettingID    = N'" & pAnalyzerSettingsRow.SettingID.ToString().Replace("'", "''") & "' "

                    'Dim cmdText As String = " UPDATE tcfgAnalyzerSettings " & vbCrLf & _
                    '                        " SET    CurrentValue = N'" & pAnalyzerSettingsRow.CurrentValue.ToString().Replace("'", "''") & "' " & vbCrLf & _
                    '                        " WHERE  AnalyzerID   = N'" & pAnalyzerSettingsRow.AnalyzerID.ToString().Replace("'", "''") & "'" & vbCrLf & _
                    '                        " AND    SettingID    = N'" & pAnalyzerSettingsRow.SettingID.ToString().Replace("'", "''") & "' "

                    GenSQLInstruction = cmdText
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using
                    resultData.HasError = False
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerSettingsDAO.Update", EventLogEntryType.Error, False)
                myLogAcciones.CreateLogActivity("SQL INSTRUCTION--> " & GenSQLInstruction, "tcfgAnalyzerSettingsDAO.Update", EventLogEntryType.Error, False)

            End Try
            Return resultData
        End Function


        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerSettingsDS As AnalyzerSettingsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                ElseIf (Not pAnalyzerSettingsDS Is Nothing) Then
                    Dim cmdText As String = ""
                    Dim dbCmd As New SqlClient.SqlCommand
                    'Const STR_NULL As String = " NULL, "

                    dbCmd.Connection = pDBConnection

                    For Each localRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In pAnalyzerSettingsDS.tcfgAnalyzerSettings
                        cmdText &= " INSERT INTO tcfgAnalyzerSettings "
                        cmdText &= " (AnalyzerID, SettingID, SettingDataType, CurrentValue) VALUES ("

                        cmdText &= String.Format("'{0}',", localRow.AnalyzerID) 'Required
                        cmdText &= String.Format("'{0}',", localRow.SettingID) 'Required
                        cmdText &= String.Format("'{0}',", localRow.SettingDataType) 'Required
                        cmdText &= String.Format("'{0}' ", localRow.CurrentValue) 'Required but not comma is added after CurrentValue

                        cmdText &= String.Format("){0}", vbNewLine) 'insert line break

                    Next

                    dbCmd.CommandText = cmdText
                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerSettingsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Read all analyzer settings by AnalyzerID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <returns>GlobalDataTO (AnalyzerSettingsDS)</returns>
        ''' <remarks>AG 25/10/2011</remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT AnalyzerID, SettingID, SettingDataType, CurrentValue " & vbCrLf & _
                                                " FROM   tcfgAnalyzerSettings " & vbCrLf & _
                                                " WHERE  AnalyzerID       = '" & pAnalyzerID & "' "

                        Dim myDataSet As New AnalyzerSettingsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.tcfgAnalyzerSettings)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerSettingsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "TO REVIEW-DELETE"
        '''' <summary>
        '''' Insert the Communication Setting for an specific Analyzer
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pCommConfig">Typed DataSet CommunicationConfigDS containing the values to add</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  VR 28/04/2010 - Tested OK
        '''' Modified by: TR 03/05/2010 - Implement the DAO Pattern
        ''''              SA 06/07/2010 - DAO Pattern was bad applied (still open Connection in the old way, open Connection
        ''''                              when for CRUD methods the Connection containing the Open Transaction has to be received
        ''''                              as parameter)
        '''' </remarks>
        'Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCommConfig As CommunicationConfigDS) _
        '                       As GlobalDataTO
        '    Dim dataToReturn As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            dataToReturn.HasError = True
        '            dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = ""
        '            cmdText = " INSERT INTO tcfgAnalyzerSettings(AnalyzerID, Port, Speed, Automatic) " & _
        '                          " VALUES ('" & pCommConfig.tcfgAnalyzerSettings(0).AnalyzerID.ToString & "', " & _
        '                                  " '" & pCommConfig.tcfgAnalyzerSettings(0).Port.ToString & "', " & _
        '                                         pCommConfig.tcfgAnalyzerSettings(0).Speed & ", " & _
        '                                         IIf(pCommConfig.tcfgAnalyzerSettings(0).Automatic, 1, 0).ToString & ")"

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbcmd.CommandText = cmdText
        '            dbCmd.Connection = pDBConnection

        '            dataToReturn.AffectedRecords = dbcmd.ExecuteNonQuery()
        '            If (dataToReturn.AffectedRecords = 1) Then
        '                dataToReturn.HasError = False
        '            Else
        '                dataToReturn.HasError = True
        '                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '            End If
        '        End If
        '    Catch ex As Exception
        '        dataToReturn.HasError = True
        '        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        dataToReturn.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerSettingsDAO.Create", EventLogEntryType.Error, False)
        '    End Try
        '    Return dataToReturn
        'End Function

        '''' <summary>
        '''' Update Communication Settings for an specific Analyzer
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pCommConfig">Typed DataSet CommunicationConfigDS containing the values to update</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  VR 29/04/2010 - Tested: OK
        '''' Modified by: TR 03/05/2010 - Implement the DAO Pattern
        ''''              SA 06/07/2010 - DAO Pattern was bad applied (open Connection when for CRUD methods the Connection 
        ''''                              containing the Open Transaction has to be received as parameter)
        '''' </remarks>
        'Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCommConfig As CommunicationConfigDS) As GlobalDataTO
        '    Dim dataToReturn As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection()

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            dataToReturn.HasError = True
        '            dataToReturn.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = ""
        '            cmdText = " UPDATE tcfgAnalyzerSettings SET "

        '            If (pCommConfig.tcfgAnalyzerSettings(0).IsPortNull) Then
        '                cmdText &= " Port = NULL, "
        '            Else
        '                cmdText &= " Port = '" & pCommConfig.tcfgAnalyzerSettings(0).Port.ToString & "', "
        '            End If

        '            If (pCommConfig.tcfgAnalyzerSettings(0).IsSpeedNull) Then
        '                cmdText &= " Speed = NULL, "
        '            Else
        '                cmdText &= " Speed = " & pCommConfig.tcfgAnalyzerSettings(0).Speed & ", "
        '            End If

        '            cmdText &= " Automatic = " & IIf(pCommConfig.tcfgAnalyzerSettings(0).Automatic, 1, 0).ToString & _
        '                       " WHERE  AnalyzerID = '" & pCommConfig.tcfgAnalyzerSettings(0).AnalyzerID.ToString & "'"

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbcmd.CommandText = cmdText

        '            dataToReturn.AffectedRecords = dbcmd.ExecuteNonQuery()
        '            If (dataToReturn.AffectedRecords = 1) Then
        '                dataToReturn.HasError = False
        '            Else
        '                dataToReturn.HasError = True
        '                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '            End If
        '        End If
        '    Catch ex As Exception
        '        dataToReturn.HasError = True
        '        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        dataToReturn.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerSettingsDAO.Update", EventLogEntryType.Error, False)
        '    End Try
        '    Return dataToReturn
        'End Function

        '''' <summary>
        '''' Get current values of the Communication Settings defined for the specified Analyzer
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet CommunicationConfigDS with the Communication Settings 
        ''''          currently defined for the specified Analyzer</returns>
        '''' <remarks>
        '''' Created by:  VR 29/04/2010 - Tested: OK
        '''' </remarks>
        'Public Function ReadAnalyzerSettings(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
        '    Dim dataToReturn As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not dataToReturn.HasError And Not dataToReturn.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText = " SELECT * FROM tcfgAnalyzerSettings " & _
        '                          " WHERE  AnalyzerID = '" & pAnalyzerID & "' "

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim resultData As New CommunicationConfigDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(resultData.tcfgAnalyzerSettings)

        '                dataToReturn.SetDatos = resultData
        '                dataToReturn.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        dataToReturn.HasError = True
        '        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        dataToReturn.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerSettingsDAO.ReadAnalyzerSettings", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return dataToReturn
        'End Function



        '''' <summary>
        '''' Get current values of the Communication Settings defined for the specified Analyzer
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet CommunicationConfigDS with the Communication Settings 
        ''''          currently defined for the specified Analyzer</returns>
        '''' <remarks>
        '''' Created by: DL 22/07/2011
        '''' </remarks>
        'Public Function ReadAnalyzerSettingsByAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                                 ByVal pAnalyzerID As String) As GlobalDataTO

        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)

        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText &= "SELECT AnalyzerID, SettingID, SettingDataType, Currentvalue" & vbCrLf
        '                cmdText &= "  FROM tcfgAnalyzerSettings" & vbCrLf
        '                cmdText &= " WHERE AnalyzerID = '" & pAnalyzerID & "'" & vbCrLf

        '                Dim myDataSet As New AnalyzerSettingsDS

        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myDataSet.tcfgAnalyzerSettings)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myDataSet
        '                resultData.HasError = False
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerSettingsDAO.ReadAnalyzerSettingsByAnalyzerID", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        '    Return resultData
        'End Function





        ''''' <summary>
        ''''' Insert the Communication Setting for an specific Analyzer
        ''''' </summary>
        ''''' <param name="pDBConnection">Open DB Connection</param>
        ''''' <param name="pAnalyzerSettingsRow"></param>
        ''''' <returns>GlobalDataTO containing success/error information</returns>
        ''''' <remarks>
        ''''' Created by: DL 21/07/2011
        ''''' </remarks>
        'Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                       ByVal pAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
        '        Else

        '            Dim cmdText As String = ""
        '            cmdText &= "INSERT INTO tcfgAnalyzerSettings" & vbCrLf
        '            cmdText &= "           (AnalyzerID" & vbCrLf
        '            cmdText &= "          , SettingID" & vbCrLf
        '            cmdText &= "          , SettingDataType" & vbCrLf
        '            cmdText &= "          , CurrentValue)" & vbCrLf
        '            cmdText &= "    VALUES (" & vbCrLf

        '            cmdText &= "            N'" & pAnalyzerSettingsRow.AnalyzerID.ToString().Replace("'", "''") & "'" & vbCrLf
        '            cmdText &= "          , N'" & pAnalyzerSettingsRow.SettingID.ToString().Replace("'", "''") & "'" & vbCrLf

        '            If pAnalyzerSettingsRow.IsSettingDataTypeNull Then
        '                cmdText &= "          , ''" & vbCrLf
        '            Else
        '                cmdText &= "          , N'" & pAnalyzerSettingsRow.SettingDataType.ToString().Replace("'", "''") & "'" & vbCrLf
        '            End If

        '            If pAnalyzerSettingsRow.IsCurrentValueNull Then
        '                cmdText &= "          , ''" & vbCrLf
        '            Else
        '                cmdText &= "          , N'" & pAnalyzerSettingsRow.CurrentValue.ToString().Replace("'", "''") & "'" & vbCrLf
        '            End If

        '            cmdText &= ")"

        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '            End Using

        '            resultData.HasError = False
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tcfgAnalyzerSettingsDAO.Create", EventLogEntryType.Error, False)
        '    End Try

        '    Return resultData

        'End Function
#End Region
    End Class
End Namespace