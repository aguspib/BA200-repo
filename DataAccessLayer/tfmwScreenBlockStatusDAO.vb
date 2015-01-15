Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO


    Public Class tfmwScreenBlockStatusDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Get the status of the informed Screen Block when the WorkSession/Analyzer is in the specified state
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pScreenID">Screen Identifier</param>
        ''' <param name="pBlockID">Screen Block Identifier</param>
        ''' <param name="pAppStatus">WorkSession/Analyzer Status</param>
        ''' <param name="pDataLoaded">Flag indicating if there is data loaded (when 1) or not (when 0) in the Screen Block</param>
        ''' <returns>GlobalDataTO containing a typed Dataset ScreenBlockStatusDS with status information for the specified Screen Block when the 
        '''          WorkSession/Analyzer is in the informed state</returns>
        ''' <remarks>
        ''' Created by:  BK 10/12/2009 
        ''' Modified by: BK 21/12/2009 - pDataLoaded Type changed to Integer
        '''              VR 08/01/2010 - Return a GlobalDataTO
        '''              VR 12/01/2010 - Return a Boolean value inside the GlobalDataTO instead the typed DS returned by DAO function
        '''              AG 13/01/2010 - Use the DAO Template for SELECT
        '''              SA 15/10/2013 - Changed the function Template
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pScreenID As String, ByVal pBlockID As String, _
                             ByVal pAppStatus As String, ByVal pDataLoaded As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tfmwScreenBlockStatus " & vbCrLf & _
                                                " WHERE  ScreenID   = '" & pScreenID.Trim & "' " & vbCrLf & _
                                                " AND    BlockID    = '" & pBlockID.Trim & "' " & vbCrLf & _
                                                " AND    AppStatus  = '" & pAppStatus.Trim & "' " & vbCrLf & _
                                                " AND    DataLoaded = " & pDataLoaded.ToString

                        Dim myScreenBlockStatusDS As New ScreenBlockStatusDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myScreenBlockStatusDS.tfmwScreenBlockStatus)
                            End Using
                        End Using

                        resultData.SetDatos = myScreenBlockStatusDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwScreenBlockStatusDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
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
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT BS.ScreenID, BS.BlockID, BS.AppStatus, BS.BlockEnabled, BS.DataLoaded, BS.LowerUserLevel, BS.DefaultEnabled " & vbCrLf & _
                                                " FROM   tfmwScreenBlockStatus BS " & vbCrLf & _
                                                " WHERE  BS.ScreenID = '" & pScreenID.Trim & "' " & vbCrLf & _
                                                " AND    BS.AppStatus = '" & pAppStatus.Trim & "' " & vbCrLf & _
                                                " AND    BS.DataLoaded = " & pDataLoaded.ToString

                        Dim myScreenBlockStatusDS As New ScreenBlockStatusDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myScreenBlockStatusDS.tfmwScreenBlockStatus)
                            End Using
                        End Using

                        resultData.SetDatos = myScreenBlockStatusDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tfmwScreenBlockStatusDAO.ReadByScreenAndAppStatus", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
