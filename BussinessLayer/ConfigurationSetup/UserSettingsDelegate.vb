Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global.GlobalEnumerates ' Modified by : VR  29/12/2009

Namespace Biosystems.Ax00.BL

    Public Class UserSettingsDelegate

#Region "Other Methods"
        ''' <summary>
        ''' Get the default Sample Tube Type for the specified TubeContent (CTRL,CALIB or PATIENT) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTubeContent">Code of the Tube Content</param>
        ''' <returns>GlobalDataTO containing the default Sample Tube configured for the specified
        '''          Tube Content (if it exists and its status is active)</returns>
        ''' <remarks>
        ''' Created by:  VR 17/11/2009 - Tested: OK
        ''' Modified by: VR 29/12/2009 - Changed the Constant Values to Enum Values
        '''              SA 19/01/2010 - Changes in the way of open the DB Connection to fulfill the new template
        '''              RH 06/06/2011 - Add new TubeContent: BLANK
        '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Public Function GetDefaultSampleTube(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTubeContent As String) As GlobalDataTO
            Dim returnedData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not returnedData.HasError AndAlso Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = CType(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myUserSettingsDAO As New tcfgUserSettingsDAO

                        'If (pTubeContent.ToUpper = "CTRL") Then
                        '    returnedData = myUserSettingsDAO.ReadBySettingIDAndStatus(dbConnection, UserSettingsEnum.DEFAULT_TUBE_CTRL.ToString)
                        'ElseIf pTubeContent.ToUpper = "CALIB" Then
                        '    returnedData = myUserSettingsDAO.ReadBySettingIDAndStatus(dbConnection, UserSettingsEnum.DEFAULT_TUBE_CALIB.ToString)
                        'Else
                        '    returnedData = myUserSettingsDAO.ReadBySettingIDAndStatus(dbConnection, UserSettingsEnum.DEFAULT_TUBE_PATIENT.ToString)
                        'End If

                        Dim SettingID As String = UserSettingsEnum.DEFAULT_TUBE_PATIENT.ToString()

                        Select Case pTubeContent    '.ToUpper()
                            Case "BLANK"
                                SettingID = UserSettingsEnum.DEFAULT_TUBE_BLANK.ToString()

                            Case "CTRL"
                                SettingID = UserSettingsEnum.DEFAULT_TUBE_CTRL.ToString()

                            Case "CALIB"
                                SettingID = UserSettingsEnum.DEFAULT_TUBE_CALIB.ToString()

                        End Select

                        returnedData = myUserSettingsDAO.ReadBySettingIDAndStatus(dbConnection, SettingID)

                        If (Not returnedData.HasError) Then
                            If (returnedData.SetDatos Is Nothing) Then
                                returnedData.HasError = True
                                returnedData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString()
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                returnedData = New GlobalDataTO()
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserSettingsDelegate.GetDefaultSampleTube", EventLogEntryType.Error, False)
            Finally

                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try

            Return returnedData
        End Function


        ''' <summary>
        ''' Get all data of the specified User Setting
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSettingID">User Setting Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet UserSettingDS with all data of the specified User Setting</returns>
        ''' <remarks>
        ''' Created by:  DL 06/05/2010
        ''' </remarks>
        Public Function ReadBySettingID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSettingID As String) As GlobalDataTO
            Dim returnedData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                returnedData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not returnedData.HasError And Not returnedData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(returnedData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myUserSettingsDAO As New tcfgUserSettingsDAO
                        returnedData = myUserSettingsDAO.ReadBySettingID(dbConnection, pSettingID)

                        If (Not returnedData.HasError) Then
                            If (returnedData.SetDatos Is Nothing) Then
                                returnedData.HasError = True
                                returnedData.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                returnedData.HasError = True
                returnedData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                returnedData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserSettingsDelegate.ReadBySettingID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return returnedData
        End Function


        ''' <summary>
        ''' Get current value of the specified User Setting
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSettingID">User Setting Identifier</param>
        ''' <returns>GlobalDataTO containing a string value with the current value of the specified User Setting</returns>
        ''' <remarks>
        ''' Created by:  TR 12/05/2010
        ''' </remarks>
        Public Function GetCurrentValueBySettingID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSettingID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        myGlobalDataTO = ReadBySettingID(dbConnection, pSettingID)
                        If (Not myGlobalDataTO.HasError) Then
                            Dim myUserSettingDS As New UserSettingDS
                            myUserSettingDS = DirectCast(myGlobalDataTO.SetDatos, UserSettingDS)

                            If (myUserSettingDS.tcfgUserSettings.Rows.Count > 0) Then
                                myGlobalDataTO.SetDatos = myUserSettingDS.tcfgUserSettings(0).CurrentValue
                            Else
                                myGlobalDataTO.HasError = True
                                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.MASTER_DATA_MISSING.ToString
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserSettingsDelegate.GetCurrentValuBySettingID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
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
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSettingID As String, _
                               ByVal pCurrentValue As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myUserSettingsDAO As New tcfgUserSettingsDAO
                        resultData = myUserSettingsDAO.Update(dbConnection, pSettingID, pCurrentValue)

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
                If (pDBConnection Is Nothing) And Not (dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserSettingsDelegate.Update", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

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
        Public Function ReadBarcodeSettings(ByVal pDBConnection As SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tcfgUserSettingsDAO
                        resultData = myDAO.ReadBarcodeSettings(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserSettingsDelegate.ReadBarcodeSettings", EventLogEntryType.Error, False)

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
        ''' Created by: RH - 08/07/2011
        ''' Modified by: TR 31/08/2011 -Implement the update of the BarCode SampleTypes Mapping.
        ''' </remarks>
        Public Function UpdateBarcodeSettings(ByVal pDBConnection As SqlConnection, ByVal pUserSettingDS As UserSettingDS, _
                                              ByVal pBarCodeSampleTypesMappingDS As BarCodeSampleTypesMappingDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tcfgUserSettingsDAO
                        resultData = myDAO.UpdateBarcodeSettings(dbConnection, pUserSettingDS)

                        'TR 31/08/2011 -Update the Sample Type mapping.
                        If Not resultData.HasError Then
                            Dim myBarCodeSampleTypesMappingDelegate As New BarCodeSampleTypesMappingDelegate
                            resultData = myBarCodeSampleTypesMappingDelegate.Update(dbConnection, pBarCodeSampleTypesMappingDS)
                        End If
                        'TR 31/08/2011 -END.

                    End If

                    If (Not resultData.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
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
                GlobalBase.CreateLogActivity(ex.Message, "UserSettingsDelegate.UpdateBarcodeSettings", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Read all user settings
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo with UserSettingDS as data</returns>
        ''' <remarks>
        ''' Creation AG 25/02/2013 - Tested PENDING
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tcfgUserSettingsDAO
                        resultData = myDAO.ReadAll(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserSettingsDelegate.ReadAll", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region
    End Class
End Namespace