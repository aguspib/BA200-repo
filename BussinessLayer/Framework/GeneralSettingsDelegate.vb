Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL

Namespace Biosystems.Ax00.BL

    Public Module GeneralSettingsDelegate

#Region "Other Methods"



        ''' <summary>
        '''  Update the Current value of an specific Setting ID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="setting">Setting ID.</param>
        ''' <param name="pCurrentValue">New Current Value</param>
        ''' <returns></returns>
        ''' <remarks>CREATED BY: TR 25/11/2011</remarks>
        Public Function UpdateCurrValBySettingID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                 ByVal setting As GlobalEnumerates.GeneralSettingsEnum, ByVal pCurrentValue As String) As GlobalDataTO
            Dim pSetingID As String = setting.ToString
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myGeneralSettingsDAO As New tfmwGeneralSettingsDAO
                        myGlobalDataTO = myGeneralSettingsDAO.UpdateCurrValBySettingID(dbConnection, pSetingID, pCurrentValue)

                    End If

                    If (Not myGlobalDataTO.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message
                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "GeneralSettingsDelegate.UpdateCurrValBySettingID", EventLogEntryType.Error, False)
            Finally

                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Get current value of the informed General Setting.  Application General Settings are stored in
        ''' preloaded table tfmwGeneralSetttings     
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSettingID">General Setting Identifier</param>
        ''' <returns>GlobalDataTO containing a the current value of the specified Setting (if it exists 
        '''          and its status is active). If the setting does not exist or exists but its status 
        '''          is not active, returns -1
        ''' </returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: BK 07/12/2009
        '''              SA 19/01/2010 - Changes in the way of open the DB Connection to fulfill the new template. Return error
        '''                              Master Data Missing when the Setting Value is not found
        '''              SA 26/03/2010 - Verification of Master Data Missing is done only when the DAO function return an error
        ''' </remarks>
        <Obsolete("Use the enum-based version instead")> _
        Public Function GetGeneralSettingValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSettingID As String) As GlobalDataTO
            Return GeneralSettingByStringValue(pDBConnection, pSettingID)
        End Function

        Private Function GeneralSettingByStringValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSettingID As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Dim conexion = DAOBase.GetSafeOpenDBConnection(pDBConnection)

            Try

                'dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not conexion.HasError And Not conexion.SetDatos Is Nothing) Then
                    dbConnection = conexion.SetDatos
                    If (Not conexion.SetDatos Is Nothing) Then
                        dataToReturn = tfmwGeneralSettingsDAO.ReadBySettingIDAndStatus(dbConnection, pSettingID).GetCompatibleGlobalDataTO

                        If (dataToReturn.HasError) Then
                            If (dataToReturn.SetDatos Is Nothing) Then
                                dataToReturn.HasError = True
                                dataToReturn.ErrorCode = "MASTER_DATA_MISSING"
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = "SYSTEM_ERROR"
                dataToReturn.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function


        ''' <summary>
        ''' Get current value of the informed General Setting.  Application General Settings are stored in
        ''' preloaded table tfmwGeneralSetttings     
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="setting">General Setting enumerator</param>
        ''' <returns>GlobalDataTO containing a the current value of the specified Setting (if it exists 
        '''          and its status is active). If the setting does not exist or exists but its status 
        '''          is not active, returns -1
        ''' </returns>
        ''' <remarks>
        ''' Created by: 
        ''' Modified by: BK 07/12/2009
        '''              SA 19/01/2010 - Changes in the way of open the DB Connection to fulfill the new template. Return error
        '''                              Master Data Missing when the Setting Value is not found
        '''              SA 26/03/2010 - Verification of Master Data Missing is done only when the DAO function return an error
        '''              MI 24/03/2015 - Made proper usage of existing enumerates.
        ''' </remarks>
        Public Function GetGeneralSettingValue(ByVal pDBConnection As SqlClient.SqlConnection, ByVal setting As GlobalEnumerates.GeneralSettingsEnum) As GlobalDataTO
            Dim pSettingID As String = setting.ToString()
            Return GeneralSettingByStringValue(pDBConnection, pSettingID)
        End Function

        ''' <summary>
        ''' This function returns General setting value by its enum
        ''' </summary>
        ''' <param name="pSettingID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetGeneralSettingValue(pSettingID As GlobalEnumerates.GeneralSettingsEnum) As TypedGlobalDataTo(Of String)

            Try
                Return tfmwGeneralSettingsDAO.ReadBySettingIDAndStatus(Nothing, pSettingID.ToString)
            Catch ex As Exception
                GlobalBase.CreateLogActivity(ex)
                Dim result = New TypedGlobalDataTo(Of String)
                result.HasError = True
                result.ErrorCode = "SYSTEM_ERROR"
                result.ErrorMessage = ex.Message
                Return result
            End Try

        End Function
#End Region
    End Module

End Namespace
