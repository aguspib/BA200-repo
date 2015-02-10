Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports System.Text
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisWSCurveResultsDAO
          

#Region "CRUD Methods"
        ''' <summary>
        ''' Move to Historic Module all Calibration Curve Results 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">The AnalyzerID</param>
        ''' <param name="pCurveResultsID">The CurveResultsID to move to Historical module</param>
        ''' <param name="pHistWorkSessionID">The HistWorkSessionID to set in the curve</param>
        ''' <param name="pHistOrderTestID">The HistOrderTestID</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  JB 15/10/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pCurveResultsID As Integer, _
                               ByVal pHistWorkSessionID As String, ByVal pHistOrderTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    Using dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        cmdText.Append(" INSERT INTO thisWSCurveResults ")
                        cmdText.Append(" (HistOrderTestID, AnalyzerID, WorkSessionID, CurvePoint, ABSValue, CONCValue) ")
                        cmdText.AppendFormat(" SELECT {0}, '{1}', '{2}', CurvePoint, ABSValue, CONCValue", _
                                             pHistOrderTestID, pAnalyzerID.Trim.Replace("'", "''"), pHistWorkSessionID.Trim)
                        cmdText.AppendFormat(" FROM twksCurveResults WHERE CurveResultsID = {0}", pCurveResultsID)

                        dbCmd.CommandText = cmdText.ToString()
                        myGlobalDataTO.AffectedRecords += dbCmd.ExecuteNonQuery()
                        cmdText.Length = 0
                    End Using

                    myGlobalDataTO.SetDatos = True
                    myGlobalDataTO.HasError = False
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSCurveResultsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get the historic curve points using the primary key parameters HistOrderTestID, AnalyzerID, WorkSessionID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pHistOrderTestID"></param>
        ''' <param name="pAnalyzerID"></param>
        ''' <param name="pWorkSessionID"></param>
        ''' <returns>GlobalDataTo with dataset as CurveResultsDS</returns>
        ''' <remarks>AG 17/10/2012 - Creation</remarks>
        Public Function GetResults(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As Integer, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = "SELECT CurvePoint" & _
                                       ", ABSValue" & _
                                       ", CONCValue" & _
                                  " FROM thisWSCurveResults" & _
                                  " WHERE HistOrderTestID = " & pHistOrderTestID & _
                                  " AND AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' " & _
                                  " AND WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "' " & _
                                  " ORDER BY CurvePoint ASC"

                        Dim myDataSet As New CurveResultsDS

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.twksCurveResults)
                            End Using
                        End Using

                        resultData.SetDatos = myDataSet
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSCurveResultsDAO.GetResults", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function
#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' Delete all Curve Results saved in Historic Module for the specified AnalyzerID / WorkSessionID / OrderTestID (only
        ' ''' if the OrderTestID corresponds to a multipoint Calibrator)
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
        '            Dim cmdText As String = " DELETE FROM thisWSCurveResults " & vbCrLf & _
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

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "thisWSCurveResultsDAO.DeleteByHistOrderTestID", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace


