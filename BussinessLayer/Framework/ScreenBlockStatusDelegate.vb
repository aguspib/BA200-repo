Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL
    Public Class ScreenBlockStatusDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Get the status of the informed Screen Block when the WorkSession/Analyzer is in the specified state
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pScreenID">Screen Identifier</param>
        ''' <param name="pBlockID">Screen Block Identifier</param>
        ''' <param name="pAppStatus">WorkSession/Analyzer Status</param>
        ''' <param name="pDataLoaded">Flag indicating if there is data loaded (when 1) or not (when 0) in the Screen Block</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if the Screen Block is enabled or disabled</returns>
        ''' <remarks>
        ''' Created by:  BK 10/12/2009 
        ''' Modified by: BK 21/12/2009 - pDataLoaded Type changed to Integer
        '''              VR 08/01/2010 - Return a GlobalDataTO
        '''              VR 12/01/2010 - Return a Boolean value inside the GlobalDataTO instead the typed DS returned by DAO function
        '''              AG 15/01/2010 - Use the Delegate template for SELECT
        ''' </remarks>
        Public Function GetBlockStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pScreenID As String, ByVal pBlockID As String, ByVal pAppStatus As String, _
                                       ByVal pDataLoaded As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim mytfmwScreenBlockStatusDAO As New tfmwScreenBlockStatusDAO

                        resultData = mytfmwScreenBlockStatusDAO.Read(Nothing, pScreenID, pBlockID, pAppStatus, pDataLoaded)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim result As ScreenBlockStatusDS = DirectCast(resultData.SetDatos, ScreenBlockStatusDS)
                            If (result.tfmwScreenBlockStatus.Rows.Count > 0) Then
                                resultData.SetDatos = result.tfmwScreenBlockStatus(0).BlockEnabled
                            Else
                                resultData.HasError = True
                            End If
                        End If
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ScreenBlockStatusDelegate.GetBlockStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>        
        ''' Get the status of the informed Screen Block when the WorkSession/Analyzer is in the specified state and according the User Level of the
        ''' connected application User
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pScreenID">Screen Identifier</param>
        ''' <param name="pBlockID">Screen Block Identifier</param>
        ''' <param name="pAppStatus">WorkSession/Analyzer Status</param>
        ''' <param name="pDataLoaded">Flag indicating if there is data loaded (when 1) or not (when 0) in the Screen Block</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if the Screen Block is enabled or disabled</returns>
        ''' <remarks>
        ''' Created by:  BK 10/12/2009 
        ''' Modified by: BK 21/12/2009 - pDataLoaded Type changed to Integer
        '''              VR 08/01/2010 - Return a GlobalDataTO
        '''              VR 12/01/2010 - Return a Boolean value inside the GlobalDataTO instead the typed DS returned by DAO function
        '''              AG 15/01/2010 - Use the Delegate Template for SELECT
        ''' </remarks>
        Public Function GetUserLevelBlockStatus(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pScreenID As String, ByVal pBlockID As String, ByVal pAppStatus As String, _
                                                ByVal pDataLoaded As Integer) As GlobalDataTO
            Dim blockEnabled As Boolean = False
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDbConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the Screen Block status when the WorkSession/Analyzer is in the specified state
                        Dim mytfmwScreenBlockStatusDAO As New tfmwScreenBlockStatusDAO
                        resultData = mytfmwScreenBlockStatusDAO.Read(dbConnection, pScreenID, pBlockID, pAppStatus, pDataLoaded)

                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim result As ScreenBlockStatusDS = DirectCast(resultData.SetDatos, ScreenBlockStatusDS)

                            If (result.tfmwScreenBlockStatus.Rows.Count > 0) Then
                                'Get the User Level of the connected User
                                'Dim myGlobalbase As New GlobalBase
                                Dim currentUserLevel As String = GlobalBase.GetSessionInfo.UserLevel

                                If (currentUserLevel.Trim <> String.Empty) Then
                                    'Get the numeric value for the User Level
                                    Dim myUsersLevel As New UsersLevelDelegate
                                    Dim currentUserNumericalLevel As Integer = -1

                                    resultData = myUsersLevel.GetUserNumericLevel(Nothing, currentUserLevel)
                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                        currentUserNumericalLevel = CType(resultData.SetDatos, Integer)
                                    End If

                                    If (Not result.tfmwScreenBlockStatus(0).IsLowerUserLevelNull) AndAlso (Not result.tfmwScreenBlockStatus(0).IsDefaultEnabledNull) Then
                                        If (currentUserNumericalLevel < result.tfmwScreenBlockStatus(0).LowerUserLevel) Then
                                            result.tfmwScreenBlockStatus(0).BeginEdit()
                                            result.tfmwScreenBlockStatus(0).BlockEnabled = result.tfmwScreenBlockStatus(0).DefaultEnabled
                                            result.tfmwScreenBlockStatus(0).EndEdit()
                                        End If
                                    End If

                                    blockEnabled = result.tfmwScreenBlockStatus(0).BlockEnabled
                                End If
                            End If
                        End If
                    End If
                End If

                'Return the Screen Block Status 
                resultData.SetDatos = blockEnabled
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ScreenBlockStatusDelegate.GetUserLevelBlockStatus", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the informed Screen, get the status for all its Blocks when the WorkSession/Analyzer is in the specified state
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pScreenID">Screen Identifier</param>
        ''' <param name="pAppStatus">WorkSession/Analyzer Status</param>
        ''' <param name="pDataLoaded">Flag indicating if there is data loaded (when 1) or not (when 0) in the Screen Block</param>
        ''' <returns>GlobalDataTO containing a typed Dataset ScreenBlockStatusDS with status information for all Screen Blocks when the WorkSession/Analyzer is in 
        '''          the specified state</returns>
        ''' <remarks>
        ''' Created by: SA 15/10/2013
        ''' </remarks>
        Public Function ReadByScreenAndAppStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pScreenID As String, ByVal pAppStatus As String, _
                                                 ByVal pDataLoaded As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myScreenBlockStatusDAO As New tfmwScreenBlockStatusDAO
                        resultData = myScreenBlockStatusDAO.ReadByScreenAndAppStatus(dbConnection, pScreenID, pAppStatus, pDataLoaded)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "ScreenBlockStatusDelegate.ReadByScreenAndAppStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
