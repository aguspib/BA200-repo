Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tcfgGridColsConfigurationDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Get the last saved width of all columns in the specified Screen Grid
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pScreenID">Identifier of the screen in which the specified Grid is located</param>
        ''' <param name="pGridName">Name of the grid in the specified Screen for which the width of all columns is read</param>
        ''' <returns>GlobalDataTO containing a typed DataSet GridColsConfigDS with the list of columns for the specified Screen Grid
        '''          and the default and the last saved width of each one of them</returns>
        ''' <remarks>
        ''' Created by:  SA 31/07/2014 - BA-1861
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pScreenID As String, ByVal pGridName As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ScreenID, ColumnName, DefaultWidth, SavedWidth FROM tcfgGridColsConfiguration " & vbCrLf & _
                                                " WHERE  ScreenID = '" & pScreenID.Trim & "' " & vbCrLf & _
                                                " AND    GridName = '" & pGridName.Trim & "' " & vbCrLf

                        Dim myGridColsConfigDS As New GridColsConfigDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myGridColsConfigDS.tcfgGridColsConfiguration)
                            End Using
                        End Using

                        resultData.SetDatos = myGridColsConfigDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgGridColsConfigurationDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the last saved width of all columns in the specified Screen Grid  
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pGridColsConfigDS">Typed DataSet GridColsConfigDS containing data of all columns of the Screen Grid to update </param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 31/07/2014 - BA-1861
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pGridColsConfigDS As GridColsConfigDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim myGlobalBase As New GlobalBase
                    Dim cmdText As String = String.Empty

                    For Each row As GridColsConfigDS.tcfgGridColsConfigurationRow In pGridColsConfigDS.tcfgGridColsConfiguration
                        cmdText &= " UPDATE tcfgGridColsConfiguration " & vbCrLf & _
                                   " SET    SavedWidth  = " & row.SavedWidth.ToString & ", " & vbCrLf & _
                                          " TS_User     = N'" & GlobalBase.GetSessionInfo().UserName().Replace("'", "''") & "', " & vbCrLf & _
                                          " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf & _
                                   " WHERE  ScreenID = '" & row.ScreenID & "' " & vbCrLf & _
                                   " AND    GridName = '" & row.GridName & "' " & vbCrLf & _
                                   " AND    ColumnName = '" & row.ColumnName & "' " & vbCrLf
                    Next
                    
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tcfgGridColsConfigurationDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace