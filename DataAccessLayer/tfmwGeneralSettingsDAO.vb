Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tfmwGeneralSettingsDAO
        Inherits DAOBase

#Region "CRUD Methods"

        ''' <summary>
        ''' Update the Current value of an specific Setting ID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pSetingID">Setting ID. </param>
        ''' <param name="pCurrentValue">New Current Value.</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY TR 24/11/2011</remarks>
        Public Function UpdateCurrValBySettingID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pSetingID As String, ByVal pCurrentValue As String) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an opened Database Connection
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim myGlobalBase As New GlobalBase
                    Dim cmdText As String = String.Empty
                    cmdText &= " UPDATE tfmwGeneralSettings" & Environment.NewLine
                    cmdText &= " SET  CurrentValue = '" & pCurrentValue & "'" & Environment.NewLine
                    cmdText &= " WHERE SettingID = '" & pSetingID & "'" & Environment.NewLine

                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()

                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwGeneralSettingsDAO.UpdateCurrValBySettingID", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO

        End Function


        ''' <summary>
        ''' Get current value of the informed General Setting.  Application General Settings are stored in
        ''' preloaded table tfmwGeneralSetttings     
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pGeneralSettingID">General Setting Identifier</param>
        ''' <returns>GlobalDataTO containing a the current value of the specified Setting (if it exists 
        '''          and its status is active)
        ''' </returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: BK 07/12/2009
        '''              SA 18/12/2009 - Error fixed: Close of dbDataReader was missing 
        '''              SA 19/01/2010 - Changes in the way of open the DB Connection to fulfill the new template
        '''              SA 26/03/2010 - Changes to return Nothing when the setting is not enabled or it does not exist in DB.
        '''                              Changes to return Nothing plus HasError=True when the setting exists in DB and is active 
        '''                              but it has not value
        ''' </remarks>
        Public Function ReadBySettingIDAndStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pGeneralSettingID As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT CurrentValue " & _
                                  " FROM   tfmwGeneralSettings " & _
                                  " WHERE  SettingID = '" & pGeneralSettingID.Trim & "' " & _
                                  " AND    Status = 1 "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim dbDataReader As SqlClient.SqlDataReader
                        dbDataReader = dbCmd.ExecuteReader()

                        If (dbDataReader.HasRows) Then
                            dbDataReader.Read()
                            If (Not dbDataReader.IsDBNull(0)) Then
                                resultData.SetDatos = CType(dbDataReader.Item("CurrentValue"), String)
                            Else
                                resultData.HasError = True
                                resultData.SetDatos = Nothing
                            End If
                        Else
                            resultData.SetDatos = Nothing
                        End If
                        dbDataReader.Close()
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwGeneralSettingsDAO.ReadBySettingIDAndStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

        End Function

#End Region

    End Class

End Namespace