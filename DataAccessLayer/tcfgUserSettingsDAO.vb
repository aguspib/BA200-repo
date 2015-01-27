Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tcfgUserSettingsDAO
          

#Region "CRUD Methods"

        ''' <summary>
        ''' Get all data of the specified User Setting
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSettingID">User Setting Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet UserSettingDS with all data of the specified User Setting</returns>
        ''' <remarks>
        ''' Created by:  DL 06/05/2010
        ''' Modified by: SA 14/09/2010 - Removed field LangDescription from the SQL due to it was removed from the table
        '''              SA 17/04/2012 - Changed the function template         
        '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)       
        ''' </remarks>
        Public Function ReadBySettingID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSettingID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT SettingID, SettingDataType, CurrentValue, Status " & vbCrLf & _
                                                " FROM   tcfgUserSettings " & vbCrLf & _
                                                " WHERE  UPPER(SettingID) = '" & pSettingID & "' " & vbCrLf
                        '" WHERE  UPPER(SettingID) = '" & pSettingID.ToUpper & "' " & vbCrLf

                        Dim myUserSettingDS As New UserSettingDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myUserSettingDS.tcfgUserSettings)
                            End Using
                        End Using

                        resultData.SetDatos = myUserSettingDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserSettingsDAO.ReadBySettingID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get current value of the informed User Setting if it is active.  Application User Settings are stored in
        ''' table tcfgUserSetttings     
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSettingID">User Setting Identifier</param>
        ''' <returns>GlobalDataTO containing the current value of the specified Setting (if it exists 
        '''          and its status is active)
        ''' </returns>
        ''' <remarks>
        ''' Created by:  VR 17/11/2009 - Tested: OK
        ''' Modified by: SA 19/01/2010 - Changes in the way of open the DB Connection to fulfill the new template
        '''              SA 17/04/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadBySettingIDAndStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSettingID As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT currentValue FROM tcfgUserSettings " & vbCrLf & _
                                                " WHERE  SettingID = '" & pSettingID.Trim & "' " & vbCrLf & _
                                                " AND    Status    = 1 " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            dataToReturn.SetDatos = dbCmd.ExecuteScalar()
                            dataToReturn.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserSettingsDAO.ReadBySettingIDAndStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Update current value of the specified User Setting
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSettingID">User Setting Identifier</param>
        ''' <param name="pCurrentValue">Current Setting Value</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by : VR 14/05/2010 
        ''' Modified by: DL 25/05/2010
        '''              SA 06/07/2010 - Remove parameter pUserSettingDS, it is not used; remove Optional in parameters for
        '''                              the SettingID and the value to set 
        '''              SA 26/10/2010 - Add the N preffix for multilanguage of TS_User field
        '''              SA 17/04/2012 - Changed the function template
        '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSettingID As String, ByVal pCurrentValue As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    'Get the connected Username from the current Application Session
                    'Dim currentSession As New GlobalBase
                    Dim currentUser As String = GlobalBase.GetSessionInfo().UserName.Trim.ToString
                    pCurrentValue = pCurrentValue.Replace("'", "''") 'SGM 26/06/2013 - avoid sql syntax error
                    Dim cmdText As String = " UPDATE tcfgUserSettings " & vbCrLf & _
                                            " SET    CurrentValue = N'" & pCurrentValue & "', " & vbCrLf & _
                                                   " TS_User =      N'" & currentUser.Replace("'", "''") & "', " & vbCrLf & _
                                                   " TS_DateTime = '" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf & _
                                            " WHERE  UPPER(SettingID) = '" & pSettingID & "' " & vbCrLf
                    '" WHERE  UPPER(SettingID) = '" & pSettingID.ToUpper & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserSettingsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Reads the Barcode active settings from table tcfgUserSetttings
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>
        ''' GlobalDataTO containing the current values (table tcfgUserSettings) of the Barcode active settings
        ''' </returns>
        ''' <remarks>
        ''' Created by:  RH 08/07/2011
        ''' </remarks>
        Public Function ReadBarcodeSettings(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT SettingID, CurrentValue " & vbCrLf & _
                                                " FROM   tcfgUserSettings " & vbCrLf & _
                                                " WHERE  SettingID LIKE 'BARCODE_%' " & vbCrLf & _
                                                " AND    Status = 1 " & vbCrLf

                        Dim myUserSettingDS As New UserSettingDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myUserSettingDS.tcfgUserSettings)
                            End Using
                        End Using

                        resultData.SetDatos = myUserSettingDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserSettingsDAO.ReadBarcodeSettings", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Updates the Barcode settings to table tcfgUserSetttings
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserSettingDS">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  RH 08/07/2011
        ''' Modified by: SA 17/04/2012 - Changed the function template for the one used for Insert/Update/Delete in DAO functions
        '''                              Added N prefix to allow non-ascii characters in value assigned to CurrentValue field 
        ''' </remarks>
        Public Function UpdateBarcodeSettings(ByVal pDBConnection As SqlConnection, ByVal pUserSettingDS As UserSettingDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = String.Empty
                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                            For Each row As UserSettingDS.tcfgUserSettingsRow In pUserSettingDS.tcfgUserSettings
                                cmdText = String.Format(" UPDATE tcfgUserSettings SET CurrentValue =N'{0}' " + _
                                                        " WHERE SettingID = '{1}' ", row.CurrentValue, row.SettingID)

                                dbCmd.CommandText = cmdText
                                dbCmd.ExecuteNonQuery()
                            Next
                        End Using
                    End Using
                    resultData.HasError = False
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserSettingsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Read all user settings
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Creation - AG 25/02/2013 - Tested PENDING
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM   tcfgUserSettings "

                        Dim myUserSettingDS As New UserSettingDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myUserSettingDS.tcfgUserSettings)
                            End Using
                        End Using

                        resultData.SetDatos = myUserSettingDS
                        resultData.HasError = False
                    End If
                End If


            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserSettingsDAO.ReadAll", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


#End Region
    End Class
End Namespace