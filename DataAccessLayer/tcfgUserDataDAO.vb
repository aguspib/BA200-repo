Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tcfgUserDataDAO
          

#Region "CRUD Methods"

        ''' <summary>
        ''' Create a new User
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserDetails">Typed DataSet UserDataDS containing data of the new User</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Modified by: SA 23/06/2010 - Changed the way of manage the DB Transaction to 
        '''                              fulfill the new template
        '''              SA 22/09/2010 - Changed verification of field TS_User and TS_DateTime NULLs
        '''              SA 26/10/2010 - Added N preffix for multilanguage of field TS_User
        '''              SG 05/04/2011 - Service Users
        '''              AG 02/06/2011 - Corregir Query ... al añadir Service Users peta!!!
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserDetails As UserDataDS, _
                                Optional ByVal pIsService As Boolean = False) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " INSERT INTO tcfgUsers (UserName, Password, UserLevel, InternalUser, MaxTestsNum, " & _
                                                      " UserFirstName, UserLastName, TS_User, TS_DateTime, IsService) " & _
                              " VALUES (N'" & pUserDetails.tcfgUserData(0).UserName.Trim.Replace("'", "''") & "', " & _
                                      " N'" & pUserDetails.tcfgUserData(0).Password.Trim.Replace("'", "''") & "', " & _
                                      " '" & pUserDetails.tcfgUserData(0).UserLevel.ToString & "', " & _
                                             IIf(pUserDetails.tcfgUserData(0).InternalUser, 1, 0).ToString & ", "

                    If (pUserDetails.tcfgUserData(0).IsMaxTestsNumNull) Then
                        cmdText &= "NULL, "
                    Else
                        cmdText &= pUserDetails.tcfgUserData(0).MaxTestsNum & ", "
                    End If

                    If (pUserDetails.tcfgUserData(0).IsUserFirstNameNull) Then
                        cmdText &= "NULL, "
                    Else
                        cmdText &= " N'" & pUserDetails.tcfgUserData(0).UserFirstName.Trim.Replace("'", "''") & "', "
                    End If

                    If (pUserDetails.tcfgUserData(0).IsUserLastNameNull) Then
                        cmdText &= "NULL, "
                    Else
                        cmdText &= " N'" & pUserDetails.tcfgUserData(0).UserLastName.Trim.Replace("'", "''") & "', "
                    End If

                    If (pUserDetails.tcfgUserData(0).IsTS_UserNull) Then
                        'Get the connected Username from the current Application Session
                        'Dim currentSession As New GlobalBase
                        cmdText &= " N'" & GlobalBase.GetSessionInfo().UserName.Replace("'", "''") & "', "
                    Else
                        cmdText &= " N'" & pUserDetails.tcfgUserData(0).TS_User.Replace("'", "''") & "', "
                    End If

                    If (pUserDetails.tcfgUserData(0).IsTS_DateTimeNull) Then
                        'Get the current DateTime
                        Dim currentDateTime As Date = Now
                        cmdText &= " '" & currentDateTime.ToString("yyyyMMdd HH:mm:ss") & "' , "
                    Else
                        cmdText &= " '" & pUserDetails.tcfgUserData(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' , "
                    End If

                    'SG 05/04/2011 - Service Users
                    If pIsService Then
                        cmdText &= " 1 ) "
                    Else
                        cmdText &= " 0 ) "
                    End If

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = (resultData.AffectedRecords = 0)
                    resultData.SetDatos = pUserDetails
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserDataDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all Users belonging to whatever level
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserName">Optional parameter. When informed, it allows get only the data of the specified User</param>
        ''' <returns>GlobalDataTO containing a typed DataSet UserDataDS with the list of Users </returns>
        ''' <remarks>
        ''' Created by:  SG 29/07/2010
        ''' Modified by: SA 20/09/2010 - Added optional parameter to allow get data of the specified User
        '''              SA 26/10/2010 - Added N preffix for multilanguage when comparing by UserName
        '''              SG 05/04/2011 - Service Users
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pUserName As String = "", _
                                Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT * FROM tcfgUsers "

                        If (pUserName.Trim <> "") Then
                            'cmdText &= " WHERE UPPER(UserName) = N'" & pUserName.Trim.Replace("'", "''").ToUpper & "'"
                            cmdText &= " WHERE UPPER(UserName) = UPPER(N'" & pUserName.Trim.Replace("'", "''") & "')"
                        End If

                        'SGM 05/04/11 Service Users
                        If pIsService Then
                            cmdText &= " AND IsService = 1 "
                        Else
                            cmdText &= " AND IsService = 0 "
                        End If

                        cmdText &= " ORDER BY UserName "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        Dim userData As New UserDataDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(userData.tcfgUserData)

                        resultData.SetDatos = userData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserDataDAO.ReadAll", EventLogEntryType.Error, False)
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
        ''' Modified by: SA 23/06/2010 - Query changed: UserLevel has to be in UpperCase and only 
        '''                              non internal Users have to be shown
        '''              SA 20/09/2010 - Changed the query to get also the multilanguage description of field UserLevel
        '''              SA 21/10/2010 - Changes to implement multilanguage
        '''              RH 22/11/2011 - New parameter pIsService. Code optimization (Using statement, short circuit evaluation, remove unneeded and memory waste "New" instructions).
        ''' </remarks>
        Public Function ReadByInternalUser(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty
                        'Dim myLocalBase As New GlobalBase

                        cmdText = " SELECT U.*, MR.ResourceText AS UserLevelDesc " & _
                                  " FROM   tcfgUsers U INNER JOIN tfmwUsersLevel UL ON U.UserLevel = UL.UserLevel " & _
                                                     " INNER JOIN tfmwMultiLanguageResources MR ON UL.ResourceID = MR.ResourceID " & _
                                  " WHERE  U.UserLevel IN ('SUPERVISOR', 'OPERATOR') " & _
                                  " AND    U.InternalUser = 0 " & _
                                  " AND    MR.LanguageID = '" & GlobalBase.GetSessionInfo.ApplicationLanguage & "' "

                        If pIsService Then
                            cmdText &= " AND IsService = 1 "
                        Else
                            cmdText &= " AND IsService = 0 "
                        End If

                        cmdText &= " ORDER BY U.UserLevel DESC, U.UserName "

                        Dim userData As New UserDataDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(userData.tcfgUserData)
                            End Using
                        End Using

                        resultData.SetDatos = userData
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserDataDAO.ReadByInternalUser", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified User, allowing also filtering by the specified Password
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserID">User Identifier</param>
        ''' <param name="pPassword">User Password. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet UserDataDS with all details of the specified User</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 26/10/2010 - Added N preffix for multilanguage when comparing by UserName and Password
        '''              SG 05/04/2011 - Service Users
        '''              XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Public Function ReadUserIDPassword(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserID As String, _
                                           Optional ByVal pPassword As String = "", _
                                           Optional ByVal pIsService As Boolean = False) As GlobalDataTO

            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        'If (pUserID <> "" And pPassword <> "") Then
                        '    cmdText = " SELECT * FROM tcfgUsers " & _
                        '              " WHERE  UPPER(Username) = N'" & pUserID.Trim.ToUpper.Replace("'", "''") & "' " & _
                        '              " AND    UPPER([Password]) = N'" & pPassword.Trim.ToUpper.Replace("'", "''") & "' "
                        'ElseIf (pPassword = "") Then
                        '    cmdText = " SELECT * FROM tcfgUsers " & _
                        '              " WHERE  UPPER(Username) = N'" & pUserID.Trim.ToUpper.Replace("'", "''") & "' "
                        'End If
                        If (pUserID <> "" And pPassword <> "") Then
                            cmdText = " SELECT * FROM tcfgUsers " & _
                                      " WHERE  UPPER(Username) = N'" & pUserID.Trim.Replace("'", "''") & "' " & _
                                      " AND    UPPER([Password]) = N'" & pPassword.Trim.Replace("'", "''") & "' "
                        ElseIf (pPassword = "") Then
                            cmdText = " SELECT * FROM tcfgUsers " & _
                                      " WHERE  UPPER(Username) = N'" & pUserID.Trim.Replace("'", "''") & "' "
                        End If

                        'SGM 05/04/11 Service Users
                        If pIsService Then
                            cmdText &= " AND IsService = 1 "
                        Else
                            cmdText &= " AND IsService = 0 "
                        End If

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim userData As New UserDataDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(userData.tcfgUserData)

                        resultData.SetDatos = userData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserDataDAO.ReadUserIDPassword", EventLogEntryType.Error, False)
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
        '''              SA 22/09/2010 - Changed verification of field TS_User and TS_DateTime NULLs
        '''              SA 26/10/2010 - Added N preffix for multilanguage of field TS_User and when comparing by UserName
        '''              SG 05/04/2011 - Service Users
        '''              AG 02/06/2011 - Corregir Query ... al añadir Service Users peta!!!
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserDetails As UserDataDS, _
                               Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " UPDATE tcfgUsers " & _
                              " SET    [Password]   = N'" & pUserDetails.tcfgUserData(0).Password.Trim.Replace("'", "''") & "', " & _
                                     " UserLevel    = '" & pUserDetails.tcfgUserData(0).UserLevel & "', " & _
                                     " InternalUser =  " & IIf(pUserDetails.tcfgUserData(0).InternalUser, 1, 0).ToString & ", "

                    If (pUserDetails.tcfgUserData(0).IsMaxTestsNumNull) Then
                        cmdText &= " MaxTestsNum = NULL, "
                    Else
                        cmdText &= " MaxTestsNum = " & pUserDetails.tcfgUserData(0).MaxTestsNum & ", "
                    End If

                    If (pUserDetails.tcfgUserData(0).IsUserFirstNameNull) Then
                        cmdText &= " UserFirstName = NULL, "
                    Else
                        cmdText &= " UserFirstName = N'" & pUserDetails.tcfgUserData(0).UserFirstName.Trim.Replace("'", "''") & "', "
                    End If

                    If (pUserDetails.tcfgUserData(0).IsUserLastNameNull) Then
                        cmdText &= " UserLastName = NULL, "
                    Else
                        cmdText &= " UserLastName = N'" & pUserDetails.tcfgUserData(0).UserLastName.Trim.Replace("'", "''") & "', "
                    End If

                    If (pUserDetails.tcfgUserData(0).IsTS_UserNull) Then
                        'Get the connected Username from the current Application Session
                        'Dim currentSession As New GlobalBase
                        cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo().UserName.Replace("'", "''") & "', "
                    Else
                        cmdText &= " TS_User = N'" & pUserDetails.tcfgUserData(0).TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    If (pUserDetails.tcfgUserData(0).IsTS_DateTimeNull) Then
                        'Get the current DateTime
                        cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', "
                    Else
                        cmdText &= " TS_DateTime = '" & pUserDetails.tcfgUserData(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                    End If

                    'SGM 05/04/11
                    If pIsService Then
                        cmdText &= " IsService = 1 "
                    Else
                        cmdText &= " IsService = 0 "
                    End If

                    'cmdText &= " WHERE UPPER(UserName) = N'" & pUserDetails.tcfgUserData(0).UserName.Trim.ToUpper.Replace("'", "''") & "' "
                    cmdText &= " WHERE UPPER(UserName) = UPPER(N'" & pUserDetails.tcfgUserData(0).UserName.Trim.Replace("'", "''") & "') "

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = (resultData.AffectedRecords = 0)
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserDataDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specfied User
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pUserID">User Identifier</param>        
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Modified by: SA 23/06/2010 - Changed the way of manage the DB Transaction to fulfill the new template 
        '''              SA 26/10/2010 - Added N preffix for multilanguage when comparing by UserName
        '''              SG 05/04/2011 - Service Users
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserID As String, _
                               Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE tcfgUsers " & _
                              " WHERE UPPER(UserName) = UPPER(N'" & pUserID.Trim.Replace("'", "''") & "') "
                    '" WHERE UPPER(UserName) = N'" & pUserID.Trim.ToUpper.Replace("'", "''") & "' "

                    'SGM 05/04/11 Service Users
                    If pIsService Then
                        cmdText &= " AND IsService = 1 "
                    Else
                        cmdText &= " AND IsService = 0 "
                    End If

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = (resultData.AffectedRecords = 0)
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserDataDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"

        ''' <summary>
        ''' Check if the informed User exists in the DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserID">User Identifier</param>
        ''' <returns>GlobalDataTO containing a Boolean value: True if the informed User exists; 
        '''          otherwise, False</returns>
        ''' <remarks>
        ''' Modified by: SA 23/06/2010 - Changed the way of opening the DB Connection to fulfill the new 
        '''                              template; return a GlobalDataTO instead a Boolean value
        '''              SA 26/10/2010 - Added N preffix for multilanguage when comparing by UserName 
        '''              SGM 05/04/11 Service Users
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function ExistsUser(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserID As String, _
                                   Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT UserName " & _
                                  " FROM   tcfgUsers " & _
                                  " WHERE  UPPER(UserName) = UPPER(N'" & pUserID.Trim.ToUpper.Replace("'", "''") & "') "
                        '" WHERE  UPPER(UserName) = N'" & pUserID.Trim.ToUpper.Replace("'", "''") & "' "

                        'SGM 05/04/11 Service Users
                        If pIsService Then
                            cmdText &= " AND IsService = 1 "
                        Else
                            cmdText &= " AND IsService = 0 "
                        End If

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        Dim dbDataReader As SqlClient.SqlDataReader
                        dbDataReader = dbCmd.ExecuteReader()

                        resultData.SetDatos = dbDataReader.HasRows
                        resultData.HasError = False

                        dbDataReader.Close()
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserDataDAO.ExistsUser", EventLogEntryType.Error, False)
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
        '''              SA 26/10/2010 - Added N preffix for multilanguage of fields Password and TS_User, and also 
        '''                              when comparing by UserName
        '''              SG 05/04/11 Service Users
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function UpdatePassword(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserID As String, _
                                       ByVal pUserNewPwd As String, _
                                       Optional ByVal pIsService As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    'Get the connected Username from the current Application Session
                    'Dim currentSession As New GlobalBase
                    Dim currentUser As String = GlobalBase.GetSessionInfo().UserName.Trim.ToString

                    Dim cmdText As String = ""
                    cmdText = " UPDATE tcfgUsers " & _
                              " SET    [Password] = N'" & pUserNewPwd.Trim.Replace("'", "''") & "', " & _
                                     " TS_User    = N'" & currentUser.Replace("'", "''") & "', " & _
                                     " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & _
                              " WHERE  UPPER(UserName) = UPPER(N'" & pUserID.Trim.ToUpper.Replace("'", "''") & "') "
                    '" WHERE  UPPER(UserName) = N'" & pUserID.Trim.ToUpper.Replace("'", "''") & "' "

                    'SGM 05/04/11 Service Users
                    If pIsService Then
                        cmdText &= " AND IsService = 1 "
                    Else
                        cmdText &= " AND IsService = 0 "
                    End If

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = (resultData.AffectedRecords = 0)
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgUserDataDAO.UpdatePassword", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace