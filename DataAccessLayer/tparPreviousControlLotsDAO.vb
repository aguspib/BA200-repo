Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Partial Public Class tparPreviousControlLotsDAO
      

#Region "CRUD Methods"
    ''' <summary>
    ''' Receive a Control Identifier and copy all data of the current Lot to the equivalent Previous Saved Lot structure
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlID">Control Identifier</param>
    ''' <returns>GlobalDataTO containing sucess/error information</returns>
    ''' <remarks>
    ''' Created by:  DL 04/05/2011
    ''' Modified by: SA 10/05/2011 - Removed the parameter for the LotNumber; changed the SQL 
    ''' </remarks>
    Public Function SavePreviousLot(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String
                cmdText = " INSERT INTO tparPreviousControlLots (ControlID, LotNumber, ExpirationDate, ActivationDate) " & vbCrLf & _
                               " SELECT ControlID, LotNumber, ExpirationDate, ActivationDate " & vbCrLf & _
                               " FROM   tparControls " & vbCrLf & _
                               " WHERE  ControlID = " & pControlID

                'cmdText &= "          ( ControlID" & vbCrLf
                'cmdText &= "          , LotNumber" & vbCrLf
                'cmdText &= "          , ExpirationDate" & vbCrLf
                'cmdText &= "          , ActivationDate)" & vbCrLf
                'cmdText &= "     SELECT ControlID " & vbCrLf
                'cmdText &= "          , '" & pLotNumber & "' AS LotNumber" & vbCrLf
                'cmdText &= "          , ExpirationDate" & vbCrLf
                'cmdText &= "          , ActivationDate" & vbCrLf
                'cmdText &= "       FROM tparControls" & vbCrLf
                'cmdText &= "      WHERE ControlID = " & pControlID

                Dim dbCmd As New SqlClient.SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparPreviousControlLotsDAO.SavePreviousLot", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Get data of the previous Lot saved for the specified Control  
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlID">Control Identifier</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ControlDS with data of the Previous Lot saved
    '''          for the specified Control</returns>
    ''' <remarks>
    ''' Created by:  DL
    ''' Modified by: SA 10/05/2011 - New implementation; the previous one was wrong
    ''' </remarks>
    Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO
        Dim dbConnection As New SqlClient.SqlConnection

        Try
            resultData = GetOpenDBConnection(pDBConnection)
            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT * FROM tparPreviousControlLots " & vbCrLf & _
                                            " WHERE  ControlID = " & pControlID

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = dbConnection
                    dbCmd.CommandText = cmdText

                    'Fill the DataSet to return 
                    Dim myControlDS As New ControlsDS
                    Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                    dbDataAdapter.Fill(myControlDS.tparControls)

                    resultData.SetDatos = myControlDS
                    resultData.HasError = False
                End If
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparPreviousControlLotsDAO.Read", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return resultData
    End Function

    ''' <summary>
    ''' Delete the previous saved Lot for the specified Control
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pControlID">Control Identifier</param>        
    ''' <returns>GlobalDataTO containing sucess/error information</returns>
    ''' <remarks>
    ''' Created by:  DL 01/04/2011
    ''' </remarks>
    Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
        Dim resultData As New GlobalDataTO

        Try
            If (pDBConnection Is Nothing) Then
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim cmdText As String = " DELETE tparPreviousControlLots " & vbCrLf & _
                                        " WHERE  ControlID = " & pControlID

                Dim dbCmd As New SqlClient.SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText

                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                resultData.HasError = False
            End If
        Catch ex As Exception
            resultData.HasError = True
            resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            resultData.ErrorMessage = ex.Message

            'Dim myLogAcciones As New ApplicationLogManager()
            GlobalBase.CreateLogActivity(ex.Message, "tparPreviousControlLotsDAO.Delete", EventLogEntryType.Error, False)
        End Try
        Return resultData
    End Function

#End Region
End Class
