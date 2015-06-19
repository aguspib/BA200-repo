
Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL
    Public Class AnalyzerSettingsDelegate

#Region "Public Methods"

#Region "C + R + U + D"

        ''' <summary>
        ''' Create the analyzer settings for a new analyzerID (serial number)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pAnalyzerSettingsDS"></param>
        ''' <returns></returns>
        ''' <remarks>AG 29/11/2011</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerSettingsDS As AnalyzerSettingsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        If Not pAnalyzerSettingsDS Is Nothing AndAlso pAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Count > 0 Then
                            Dim myDAO As New tcfgAnalyzerSettingsDAO
                            resultData = myDAO.Create(dbConnection, pAnalyzerSettingsDS)
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerSettingsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

#End Region

        ''' <summary>
        ''' Get current value of the specified Analyzer Setting
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pSettingID">Setting Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet XX with the Communication Settings 
        '''          currently defined for the specified Analyzer</returns>
        ''' <remarks>
        ''' Created by: DL 21/07/2011
        ''' </remarks>
        <Obsolete("Use typed version instead")>
        Public Function GetAnalyzerSetting(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pSettingID As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myAnalyzerSettingsDAO As New tcfgAnalyzerSettingsDAO
                        resultData = myAnalyzerSettingsDAO.Read(dbConnection, pAnalyzerID, pSettingID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerSettingsDelegate.GetAnalyzerSetting", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        Public Function GetAnalyzerSetting(ByVal pAnalyzerID As String, ByVal pSettingID As GlobalEnumerates.AnalyzerSettingsEnum) As TypedGlobalDataTo(Of AnalyzerSettingsDS)
            Dim connection As TypedGlobalDataTo(Of SqlConnection) = Nothing
            Try
                connection = GetSafeOpenDBConnection()
                If (Not connection Is Nothing AndAlso Not connection.SetDatos Is Nothing) Then
                    Dim myAnalyzerSettingsDAO As New tcfgAnalyzerSettingsDAO
                    Dim resultData = myAnalyzerSettingsDAO.Read(connection.SetDatos, pAnalyzerID, pSettingID.ToString)
                    Return New TypedGlobalDataTo(Of AnalyzerSettingsDS)() With {.SetDatos = TryCast(resultData.SetDatos, AnalyzerSettingsDS), .HasError = resultData.HasError}
                Else
                    Return New TypedGlobalDataTo(Of AnalyzerSettingsDS)() With {.HasError = True}
                End If
            Catch ex As Exception
                Dim resultData = New TypedGlobalDataTo(Of AnalyzerSettingsDS)
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message
                GlobalBase.CreateLogActivity(ex) '.Message, "AnalyzerSettingsDelegate.GetAnalyzerSetting", EventLogEntryType.Error, False)
                Return resultData
            Finally
                CloseConnection(connection)
            End Try
        End Function


        ''' <summary>
        ''' Save values of Analyzer Communication Settings and Session Settings
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer identifier</param> 
        ''' <param name="pAnalyzerSettings">Typed DataSet AnalyzerSettingsDS containing values of the Analyzer Communication Settings</param>
        ''' <param name="pSessionSettings">Typed DataSet UserSettingsDS containing values of the User Settings to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: DL 21/07/2011
        ''' </remarks>
        Public Function Save(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pAnalyzerSettings As AnalyzerSettingsDS, _
                             ByVal pSessionSettings As UserSettingDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Save the Analyzer Settings
                        If (Not pAnalyzerSettings Is Nothing) Then
                            Dim myAnalyzerSettingsDAO As New tcfgAnalyzerSettingsDAO
                            For Each myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In pAnalyzerSettings.tcfgAnalyzerSettings.Rows
                                resultData = myAnalyzerSettingsDAO.Update(dbConnection, myAnalyzerSettingsRow)
                                If (resultData.HasError) Then Exit For
                            Next myAnalyzerSettingsRow
                        End If

                        'Save the Session Settings
                        If (Not pSessionSettings Is Nothing AndAlso Not resultData.HasError) Then
                            Dim myUserSettingsDelegate As New UserSettingsDelegate
                            For Each userSetting As UserSettingDS.tcfgUserSettingsRow In pSessionSettings.tcfgUserSettings.Rows
                                resultData = myUserSettingsDelegate.Update(dbConnection, userSetting.SettingID, userSetting.CurrentValue)
                                If (resultData.HasError) Then Exit For
                            Next
                        End If

                        If (Not resultData.HasError) Then
                            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerSettingsDelegate.Save", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
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
                        Dim myDAO As New tcfgAnalyzerSettingsDAO
                        resultData = myDAO.ReadAll(dbConnection, pAnalyzerID)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "AnalyzerSettingsDelegate.ReadAll", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

#Region "TO REVIEW-DELETE"
        '''' <summary>
        ''''  Get current values of the Communication Settings defined for the specified Analyzer
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Analyzer Identifier</param>
        '''' <returns>GlobalDataTO containing a typed DataSet CommunicationConfigDS with the Communication Settings 
        ''''          currently defined for the specified Analyzer</returns>
        '''' <remarks>
        '''' Created by: DL 21/07/2011
        '''' </remarks>
        'Public Function GetAnalyzerSettingsByAnalyzerID(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                                ByVal pAnalyzerID As String) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myAnalyzerSettingsDAO As New tcfgAnalyzerSettingsDAO
        '                resultData = myAnalyzerSettingsDAO.ReadAnalyzerSettingsByAnalyzerID(dbConnection, pAnalyzerID)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerSettingsDelegate.GetAnalyzerSettingsByAnalyzerID", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()

        '    End Try

        '    Return resultData
        'End Function

        '''' <summary>
        '''' Save values of Analyzer Communication Settings and Session Settings
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pAnalyzerID">Typed DataSet AnalyzerSettingsDS containing values of the Analyzer Communication Settings</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by: DL 21/07/2011
        '''' </remarks>
        'Public Function CreateDefaultSettings(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                      ByVal pAnalyzerID As String) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                ' Create default settings for analyzer
        '                Dim myAnalyzerSettingsDS As New AnalyzerSettingsDS
        '                Dim myAnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow

        '                'COMM_PORT
        '                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
        '                With myAnalyzerSettingsRow
        '                    .AnalyzerID = pAnalyzerID
        '                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.COMM_PORT.ToString()
        '                    .SettingDataType = "INTEGER"
        '                    .CurrentValue = "0"
        '                End With
        '                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

        '                'COMM_AUTO
        '                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
        '                With myAnalyzerSettingsRow
        '                    .AnalyzerID = pAnalyzerID
        '                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.COMM_AUTO.ToString()
        '                    .SettingDataType = "BOOLEAN"
        '                    .CurrentValue = "0"
        '                End With
        '                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

        '                'COMM_SPEED
        '                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
        '                With myAnalyzerSettingsRow
        '                    .AnalyzerID = pAnalyzerID
        '                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.COMM_SPEED.ToString()
        '                    .SettingDataType = "INTEGER"
        '                    .CurrentValue = "0"
        '                End With
        '                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

        '                'REAGENT_BARCODE_STATUS
        '                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
        '                With myAnalyzerSettingsRow
        '                    .AnalyzerID = pAnalyzerID
        '                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.REAGENT_BARCODE_DISABLED.ToString()
        '                    .SettingDataType = "BOOLEAN"
        '                    .CurrentValue = "0"
        '                End With
        '                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

        '                'SAMPLE_BARCODE_STATUS
        '                myAnalyzerSettingsRow = myAnalyzerSettingsDS.tcfgAnalyzerSettings.NewtcfgAnalyzerSettingsRow
        '                With myAnalyzerSettingsRow
        '                    .AnalyzerID = pAnalyzerID
        '                    .SettingID = GlobalEnumerates.AnalyzerSettingsEnum.SAMPLE_BARCODE_DISABLED.ToString()
        '                    .SettingDataType = "BOOLEAN"
        '                    .CurrentValue = "0"
        '                End With
        '                myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows.Add(myAnalyzerSettingsRow)

        '                Dim myAnalyzerSettingsDAO As New tcfgAnalyzerSettingsDAO
        '                For Each AnalyzerSettingsRow As AnalyzerSettingsDS.tcfgAnalyzerSettingsRow In myAnalyzerSettingsDS.tcfgAnalyzerSettings.Rows
        '                    resultData = myAnalyzerSettingsDAO.Create(dbConnection, AnalyzerSettingsRow)
        '                    If (resultData.HasError) Then Exit For
        '                Next AnalyzerSettingsRow
        '            End If

        '            If (Not resultData.HasError) Then
        '                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '            Else
        '                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
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
        '        GlobalBase.CreateLogActivity(ex.Message, "AnalyzerSettingsDelegate.CreateDefaultSettings", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData
        'End Function
#End Region
    End Class

End Namespace

