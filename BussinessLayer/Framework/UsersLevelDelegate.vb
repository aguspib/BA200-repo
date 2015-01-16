Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL

    Public Class UsersLevelDelegate

#Region "Other Methods"
        ''' <summary>
        ''' Get the numeric level related to the specified USER LEVEL
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUserLevel">Code of the User Level for which the numeric level value is searched</param>
        ''' <returns>GlobalDataTO containing an Integer value (the obtained Numeric Level)</returns>
        ''' <remarks>
        ''' Created by: AG 13/01/2010 (Tested OK) - Tested for BIOSYSTEMS, ADMINISTRATOR, SUPERVISOR, OPERATOR and others
        ''' </remarks>
        Public Function GetUserNumericLevel(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserLevel As String) As GlobalDataTO

            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim myUserLevelDAO As New tfmwUsersLevelDAO

                        myGlobalDataTO = myUserLevelDAO.Read(dbConnection, pUserLevel)

                        If Not myGlobalDataTO.HasError Then
                            Dim myUsersLevelDS As New UsersLevelDS
                            myUsersLevelDS = CType(myGlobalDataTO.SetDatos, UsersLevelDS)

                            If myUsersLevelDS.tfmwUsersLevel.Rows.Count > 0 Then
                                myGlobalDataTO.SetDatos = myUsersLevelDS.tfmwUsersLevel(0).NumericLevel
                            Else
                                myGlobalDataTO.HasError = True
                                myGlobalDataTO.ErrorCode = "MASTER_DATA_MISSING"
                            End If
                        End If

                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UsersLevelDelegate.GetUserNumericLevel", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get the list of preloaded User Levels that fulfill the specified filter (only Internal User
        ''' Level or only non Internal User Levels). Returned User Levels will be sorted by value of 
        ''' field NumericLevel in a descending way.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pInternalUse">When True, indicates the function will return only the list of
        '''                            internal Users; otherwise, the function will return the list of
        '''                            User Levels that can be used in the screen of Users Configuration</param>
        ''' <returns>GlobalDataTO containing a typed DataSet UsersLevelDS with the list of preloaded
        '''          User Levels that fulfill the specified filter (internal use or not)</returns>
        ''' <remarks>
        ''' Created by:  SA 19/01/2010
        ''' </remarks>
        Public Function GetLevelsByInternalUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                   ByVal pInternalUse As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myUserLevelDAO As New tfmwUsersLevelDAO
                        myGlobalDataTO = myUserLevelDAO.ReadByInternalUseFlag(dbConnection, pInternalUse)
                        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim myUsersLevelDS As New UsersLevelDS
                            myUsersLevelDS = CType(myGlobalDataTO.SetDatos, UsersLevelDS)
                            If (myUsersLevelDS.tfmwUsersLevel.Rows.Count = 0) Then
                                myGlobalDataTO.HasError = True
                                myGlobalDataTO.ErrorCode = "MASTER_DATA_MISSING"
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = "SYSTEM_ERROR"
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UsersLevelDelegate.GetLevelsByInternalUseFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO

        End Function


        ''' <summary>
        ''' Search the current user and return his numerical level. In case working without users consider Administrator user
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo with data set as integer</returns>
        ''' <remarks>Created by AG 10/03/10 (tested pending)</remarks>
        Public Function GetCurrentUserNumericalLevel(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim MyGlobalBase As New GlobalBase
                        Dim CurrentUserLevel As String
                        CurrentUserLevel = GlobalBase.GetSessionInfo.UserLevel

                        Dim myUsersLevel As New UsersLevelDelegate
                        Dim myTemporal As New GlobalDataTO
                        Dim CurrentUserNumericalLevel As Integer = 3   'If no user ... work as administrator user

                        If CurrentUserLevel <> "" Then  'When user level exists then find his numerical level
                            myTemporal = myUsersLevel.GetUserNumericLevel(Nothing, CurrentUserLevel)
                            If Not myTemporal.HasError Then
                                CurrentUserNumericalLevel = CType(myTemporal.SetDatos, Integer)
                            End If
                        End If

                        resultData = myTemporal
                        resultData.SetDatos = CurrentUserNumericalLevel
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = "SYSTEM_ERROR"
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UserLevelDelegate.GetCurrentUserNumericalLevel", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region
    End Class

End Namespace
