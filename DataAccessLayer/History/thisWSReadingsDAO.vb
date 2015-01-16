Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisWSReadingsDAO
        Inherits DAOBase

#Region "Other Methods"
        ''' <summary>
        ''' Get all Readings for the specified AnalyzerID/WorkSessionID/ExecutionID/MultiPointNumber/ReplicateNumber
        ''' and insert them in the corresponding table in Historic Module 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pExecutionID">Execution Identifier</param>
        ''' <param name="pMultiPointNumber">Multipoint Number (always 1 excepting for multipoint Calibrators)</param>
        ''' <param name="pReplicateNumber">Replicate Number</param>
        ''' <param name="pHistOrderTestID">Identifier of the OrderTest ID in Historic Module</param>
        ''' <param name="pHistWorkSessionID">Identifier of the WS to create in Historic Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: TR 26/06/2012
        ''' </remarks>
        Public Function InsertNewReadings(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                          ByVal pExecutionID As Integer, ByVal pMultiPointNumber As Integer, ByVal pReplicateNumber As Integer, _
                                          ByVal pHistOrderTestID As Integer, ByVal pHistWorkSessionID As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " INSERT INTO thisWSReadings (AnalyzerID, WorkSessionID, HistOrderTestID, MultiPointNumber, " & vbCrLf & _
                                                                        " ReplicateNumber, ReadingNumber, LedPosition, MainCounts, " & vbCrLf & _
                                                                        " RefCounts, ReadingDateTime) " & vbCrLf & _
                                            " SELECT AnalyzerID, " & pHistWorkSessionID.Trim & " AS WorkSessionID, " & vbCrLf & _
                                                     pHistOrderTestID & " AS HistOrderTestID, " & vbCrLf & _
                                                     pMultiPointNumber.ToString & " AS MultiPointNumber, " & vbCrLf & _
                                                     pReplicateNumber.ToString & " AS ReplicateNumber, " & vbCrLf & _
                                                   " ReadingNumber, LedPosition, MainCounts, RefCounts, [DateTime] AS ReadingDateTime " & vbCrLf & _
                                            " FROM   twksWSReadings " & vbCrLf & _
                                            " WHERE AnalyzerID       = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " AND   WorkSessionID    = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                            " AND   ExecutionID      = " & pExecutionID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSReadingsDAO.InsertNewReadings", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' Delete all Readings saved in Historic Module for the specified AnalyzerID / WorkSessionID / OrderTestID
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <param name="pHistOrderTestID">Identifier of the Order Test in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing success/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 01/07/2013
        ' ''' </remarks>
        'Public Function DeleteByHistOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                        ByVal pWorkSessionID As String, ByVal pHistOrderTestID As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
        '        Else
        '            Dim cmdText As String = " DELETE FROM thisWSReadings " & vbCrLf & _
        '                                    " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
        '                                    " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
        '                                    " AND    HistOrderTestID = " & pHistOrderTestID.ToString & vbCrLf
        '            Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
        '                resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
        '                resultData.HasError = False
        '            End Using
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "thisWSReadingsDAO.DeleteByHistOrderTestID", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace
