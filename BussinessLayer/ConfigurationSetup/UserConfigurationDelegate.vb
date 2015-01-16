Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL

    Public Class UserConfigurationDelegate

#Region "Public Methods"

        ''' <summary>
        ''' Get the list of all Users with all levels
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserName">Optional parameter. When informed, it allows get only the data of the specified User</param>
        ''' <returns>GlobalDataTO containing a typed DataSet UserDataDS with the list
        '''          of Users </returns>
        ''' <remarks>
        ''' Created by:  SG 28/07/2010
        ''' Modified by: SA 20/09/2010 - Added optional parameter to allow get data of the specified User
        ''' </remarks>
        Public Function GetAllList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pUserName As String = "", _
                                   Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim tcfgUserData As New tcfgUserDataDAO
                        resultData = tcfgUserData.ReadAll(dbConnection, pUserName, pIsService)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserConfigurationDelegate.GetAllList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Users with User Level OPERATOR or SUPERVISOR
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet UserDataDS with the list
        '''          of Users </returns>
        ''' <remarks>
        '''         Modified by: RH 22/11/2011 - New parameter pIsService. Code optimization (short circuit evaluation, remove unneeded and memory waste "New" instructions).
        ''' </remarks>
        Public Function GetNotInternalList(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim tcfgUserData As New tcfgUserDataDAO
                        resultData = tcfgUserData.ReadByInternalUser(dbConnection, pIsService)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserConfigurationDelegate.GetNotInternalList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Create a new User
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserDetails">Typed DataSet UserDataDS containing data of the new User</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Modified by: SA 23/06/2010 - Changed the way of manage the DB Transaction to fulfill the new template
        '''              SG 29/07/2010 - Check if the UserID is duplicated or corresponds to an Internal User
        '''              SA 22/09/2010 - When verify if the UserName already exists, filter ReadAll for the specific
        '''                              UserName instead of get all and search coincidences in a FOR loop
        ''' </remarks>
        Public Function Add(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserDetails As UserDataDS, _
                            Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myUsersDAO As New tcfgUserDataDAO

                        'Verify that there is not an User with the same UserName
                        resultData = myUsersDAO.ReadAll(dbConnection, pUserDetails.tcfgUserData(0).UserName, pIsService)
                        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                            Dim myUserDetails As New UserDataDS
                            myUserDetails = DirectCast(resultData.SetDatos, UserDataDS)

                            If (myUserDetails.tcfgUserData.Rows.Count = 1) Then
                                resultData.HasError = True
                                If (myUserDetails.tcfgUserData(0).InternalUser) Then
                                    resultData.ErrorCode = GlobalEnumerates.Messages.RESERVED_USERNAME.ToString
                                Else
                                    resultData.ErrorCode = GlobalEnumerates.Messages.DUPLICATED_USERNAME.ToString
                                End If
                            End If

                            If (Not resultData.HasError) Then
                                'Create the new User...
                                resultData = myUsersDAO.Create(dbConnection, pUserDetails, pIsService)
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserConfigurationDelegate.Add", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of the specified User
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserDetails">Typed DataSet UserDataDS containing data of the User to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Modified by: SA 23/06/2010 - Changed the way of manage the DB Transaction to fulfill the new template
        ''' </remarks>
        Public Function Modify(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserDetails As UserDataDS, _
                               Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Update the User data...
                        Dim myUsersDAO As New tcfgUserDataDAO
                        resultData = myUsersDAO.Update(dbConnection, pUserDetails, pIsService)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserConfigurationDelegate.Modify", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specfied User
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pUserDetails">Typed DataSet UserDataDS containing the list of Users to delete</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Modified by: SA 23/06/2010 - Changed the way of manage the DB Transaction to fulfill the new template 
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserDetails As UserDataDS, _
                               Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        For Each appUser As UserDataDS.tcfgUserDataRow In pUserDetails.tcfgUserData.Rows
                            'Delete the User
                            Dim myUsersDAO As New tcfgUserDataDAO
                            resultData = myUsersDAO.Delete(dbConnection, appUser.UserName, pIsService)

                            If (resultData.HasError) Then Exit For
                        Next

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserConfigurationDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

            'Dim resultData As New GlobalDataTO
            'Dim dataToReturn As New GlobalDataTO
            'Dim dbConnection As New SqlClient.SqlConnection(DAOBase.GetConnectionString())
            'Dim count As Integer = 0
            'Dim i As Integer = 0
            'Try
            '    dbConnection.Open()

            '    Dim dbTransaction As SqlClient.SqlTransaction
            '    dbTransaction = dbConnection.BeginTransaction()

            '    Dim userToDelete As New tcfgUserDataDAO

            '    For i = 0 To pUserDetails.tcfgUserData.Rows.Count - 1
            '        dataToReturn = userToDelete.Delete(dbConnection, dbTransaction, pUserDetails.tcfgUserData(i).UserName.ToString)
            '        If (Not dataToReturn.HasError) Then
            '            If (dataToReturn.AffectedRecords = 1) Then
            '                count = count + dataToReturn.AffectedRecords
            '            End If
            '        End If
            '    Next

            '    If (Not dataToReturn.HasError) Then
            '        If i = count Then
            '            'Successfully deletion
            '            dbTransaction.Commit()
            '        Else
            '            'Deletion was not possible
            '            dbTransaction.Rollback()

            '            'Concurrency conflict: another User updated the specified user
            '            'before the current User
            '            dataToReturn.HasError = True
            '            dataToReturn.ErrorCode = "CONCURRENCE_CONFLICT"
            '        End If
            '    Else
            '        'Deletion was not possible
            '        dbTransaction.Rollback()

            '        dataToReturn.HasError = True
            '        dataToReturn.ErrorCode = "SYSTEM_ERROR"
            '    End If

            'Catch ex As Exception
            '    dataToReturn.HasError = True
            '    dataToReturn.ErrorCode = "SYSTEM_ERROR"
            '    dataToReturn.ErrorMessage = ex.Message
            'Finally
            '    'The Database Connection is closed
            '    dbConnection.Close()
            'End Try

            'Return dataToReturn
        End Function

        ''' <summary>
        ''' Verify if the specified User (or the specified User and Password) corresponds to a
        ''' valid application User
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserID">User Identifier</param>
        ''' <param name="pPassword">User Password. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a boolean value indicating if the User is valid</returns>
        ''' <remarks>
        ''' Modified by: SA 23/06/2010 - Changed the way of opening the DB Connection to fulfill the new template
        ''' </remarks>
        Public Function UserValidation(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserID As String, _
                                       Optional ByVal pPassword As String = "", _
                                       Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myUsersDAO As New tcfgUserDataDAO

                        'Verify if the informed User exists
                        resultData = myUsersDAO.ExistsUser(dbConnection, pUserID, pIsService)
                        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                            If (DirectCast(resultData.SetDatos, Boolean)) Then
                                'Verify the UserName/Password is valid
                                resultData = myUsersDAO.ReadUserIDPassword(dbConnection, pUserID, pPassword, pIsService)

                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                    Dim resultDS As New UserDataDS
                                    resultDS = DirectCast(resultData.SetDatos, UserDataDS)

                                    If (resultDS.tcfgUserData.Rows.Count = 1) Then
                                        'The User is valid, set the User Level
                                        resultData.HasError = False
                                        resultData.SetUserLevel = resultDS.tcfgUserData(0).UserLevel
                                        resultData.SetDatos = True
                                    Else
                                        resultData.HasError = True
                                        resultData.ErrorCode = GlobalEnumerates.Messages.WRONG_USER_PASSWORD.ToString
                                        'resultData.ErrorCode = GlobalEnumerates.Messages.INVALID_APPLICATION_USER.ToString()
                                        resultData.SetDatos = False
                                    End If
                                End If
                            Else
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.WRONG_USERNAME.ToString
                                'resultData.ErrorCode = GlobalEnumerates.Messages.INVALID_APPLICATION_USER.ToString()
                                resultData.SetDatos = False
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message
                resultData.SetDatos = False

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserConfigurationDelegate.UserValidation", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update Password of the specified User
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserID">User Identifier</param>        
        ''' <param name="pUserNewPwd">New User Password</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Modified by: SA 23/06/2010 - Changed the way of manage the DB Transaction to fulfill the new template
        ''' </remarks>
        Public Function ChangePassword(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserID As String, _
                                       ByVal pUserNewPwd As String, _
                                       Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myUsersDAO As New tcfgUserDataDAO
                        resultData = myUsersDAO.UpdatePassword(dbConnection, pUserID, pUserNewPwd, pIsService)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserConfigurationDelegate.ChangePassword", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData

            'Dim returnedData As New GlobalDataTO
            'Dim dbConnection As New SqlClient.SqlConnection(DAOBase.GetConnectionString())
            'Try
            '    Dim userPwdToUpdate As New tcfgUserDataDAO

            '    If (userPwdToUpdate.ExistsUser(pChangePwd.tcfgUserData(0).UserName.ToString)) Then
            '        'Open a Database Connection and begin a Database Transaction
            '        dbConnection.Open()

            '        Dim dbTransaction As SqlClient.SqlTransaction
            '        dbTransaction = dbConnection.BeginTransaction()

            '        returnedData = userPwdToUpdate.UpdatePassword(dbConnection, dbTransaction, pChangePwd.tcfgUserData(0).UserName.ToString, pChangePwd.tcfgUserData(0).NewPassword.ToString)

            '        If (Not returnedData.HasError) Then
            '            'Successfully addition
            '            dbTransaction.Commit()

            '            ' returnedData.SetDatos = pUserDetails.Clone
            '            returnedData.SetDatos = "OK"
            '        Else
            '            'Addition was not possible
            '            dbTransaction.Rollback()

            '            'System Error (uncontrolled)
            '            returnedData.ErrorCode = "CONCURRENCE_CONFLICT"
            '        End If
            '    End If

            'Catch ex As Exception
            '    returnedData.HasError = True
            '    returnedData.ErrorCode = "SYSTEM_ERROR"
            '    returnedData.ErrorMessage = ex.Message
            'Finally
            '    'The Database Connection is closed
            '    dbConnection.Close()
            'End Try
            'Return returnedData
        End Function
#End Region

    End Class

End Namespace