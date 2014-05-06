Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tfmwUsersLevelDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Get the information by user evel
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pUserLevel"></param>
        ''' <returns>GlobalDataTO with DataSet = UsersLevelDS</returns>
        ''' <remarks>
        ''' Created by: AG 13/01/2010 (tested OK) - Tested for BIOSYSTEMS, ADMINISTRATOR, SUPERVISOR, OPERATOR and others
        ''' Modified by: SA 14/09/2010 - Removed field LangDescription from the SQL due to it was 
        '''                              removed from the table
        '''              AG 08/10/2010 - Adapt to multilanguage
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pUserLevel As String) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError) And (Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = CType(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        Dim myLocalBase As New GlobalBase

                        'AG 08/10/2010
                        'cmdText = " SELECT UserLevel, FixedUserLevelDesc,  NumericLevel, InternalUseFlag " & _
                        '          " FROM   tfmwUsersLevel " & _
                        '          " WHERE  UserLevel = '" & pUserLevel & "'"

                        cmdText = " SELECT UL.UserLevel, MR.ResourceText AS FixedUserLevelDesc, UL.NumericLevel, UL.InternalUseFlag " & _
                                  " FROM   tfmwUsersLevel UL INNER JOIN tfmwMultiLanguageResources MR ON UL.ResourceID = MR.ResourceID " & _
                                  " WHERE  UL.UserLevel = '" & pUserLevel & "'" & _
                                  " AND    MR.LanguageID = '" & myLocalBase.GetSessionInfo.ApplicationLanguage & "' "
                        'END AG 08/10/2010

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim resultData As New UsersLevelDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.tfmwUsersLevel)

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwUsersLevelDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
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
        ''' Modified by: AG 08/10/2010 - Adapt to multilanguage
        ''' </remarks>
        Public Function ReadByInternalUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pInternalUse As Boolean) As GlobalDataTO
            Dim dataToReturn As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError) And (Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = CType(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        Dim myLocalBase As New GlobalBase
                        Dim internalUse As Integer = CType(IIf(pInternalUse, 1, 0), Integer)

                        'AG 08/10/2010
                        'cmdText = " SELECT   UserLevel, FixedUserLevelDesc " & _
                        '          " FROM     tfmwUsersLevel " & _
                        '          " WHERE    InternalUseFlag = " & internalUse & _
                        '          " ORDER BY NumericLevel DESC "

                        cmdText = " SELECT UL.UserLevel, MR.ResourceText AS FixedUserLevelDesc " & _
                                  " FROM   tfmwUsersLevel UL INNER JOIN tfmwMultiLanguageResources MR ON UL.ResourceID = MR.ResourceID " & _
                                  " WHERE  UL.InternalUseFlag = " & internalUse & _
                                  " AND    MR.LanguageID = '" & myLocalBase.GetSessionInfo.ApplicationLanguage & "' " & _
                                  " ORDER BY NumericLevel DESC "
                        'END AG 08/10/2010

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim resultData As New UsersLevelDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(resultData.tfmwUsersLevel)

                        dataToReturn.SetDatos = resultData
                        dataToReturn.HasError = False
                    End If
                End If

            Catch ex As Exception
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwUsersLevelDAO.ReadByInternalUseFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

#End Region
    End Class
End Namespace
