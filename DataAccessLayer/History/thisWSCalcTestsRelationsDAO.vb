Option Explicit On
Option Strict On

Imports System.Text
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisWSCalcTestsRelationsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Create for all results of Calculated Tests in the Analyzer/WorkSession, the relation with the results of the Standard and/or Calculated 
        ''' Tests used to get it
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSCalcTestsRelationsDS">Typed Dataset HisWSCalcTestRelations containing the relations of all Calculated Tests 
        '''                                          requested for Patients in the Analyzer WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/09/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSCalcTestsRelationsDS As HisWSCalcTestRelations) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As New StringBuilder
                    Using dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection

                        For Each row As HisWSCalcTestRelations.thisWSCalcTestsRelationsRow In pHisWSCalcTestsRelationsDS.thisWSCalcTestsRelations
                            cmdText.Append(" INSERT INTO thisWSCalcTestsRelations ")
                            cmdText.Append(" (AnalyzerID, WorkSessionID, HistOrderTestIDCALC, HistOrderTestID) ")
                            cmdText.AppendFormat(" VALUES(N'{0}', '{1}', {2}, {3}) ", row.AnalyzerID.Trim.Replace("'", "''"), row.WorkSessionID.Trim, _
                                                                                      row.HistOrderTestIDCALC, row.HistOrderTestID)

                            dbCmd.CommandText = cmdText.ToString()
                            resultData.AffectedRecords += dbCmd.ExecuteNonQuery()

                            cmdText.Length = 0
                        Next
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSCalcTestsRelationsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get value of PrintExpTests for all Tests included in the Formula of all selected Calculated Tests. 
        ''' ** For all Order Tests with a Calculated Test marked as CLOSED (ClosedCalcTest = True), the Experimental Tests will not be printed (PrintExpTests 
        '''    returned as FALSE)
        ''' ** For all Order Tests with a Calculated Test that is still open (ClosedCalcTest = False), the Experimental Tests will be printed only when the value 
        '''    currently assigned for the Calculated Test in Parameters Programming (table tparCalculatedTests) for field PrintExpTests is TRUE 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisCalcOrderTestIDs">List of Historic Order Tests of Calculated Tests that have been selected to be printed</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSCalcTestRelations containing, for each informed Historic Order Test of a Calculated Test, 
        '''          the Historic Order Test of all the Tests included in its Formula and the PrintExpTests for them</returns>
        ''' <remarks>
        ''' Created by:   SA 02/09/2014 - BA-1898 
        ''' Modified by:  IT 09/10/2014 - BA-1898: For all Order Tests with a Calculated Test marked as CLOSED (ClosedCalcTest = True), the Experimental Tests will  be printed (PrintExpTests 
        '''    returned as True)
        ''' </remarks>
        Public Function GetExpTestsToExclude(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisCalcOrderTestIDs As List(Of Integer), _
                                             ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Convert the list of IDs of Historic Order Tests of Calculated Tests in a String with values divided by commas
                        Dim lstCalcOrderTestsID As String = String.Join(",", pHisCalcOrderTestIDs.ToArray())

                        'Query to get the PrintExpTests of all Tests included in the Formula of all selected Calculated Tests
                        Dim cmdText As String = " SELECT CTR.AnalyzerID, CTR.WorkSessionID, CTR.HistOrderTestIDCALC, CTR.HistOrderTestID, " & vbCrLf & _
                                                      " (CASE WHEN C.ClosedCalcTest = 1 THEN 1 " & vbCrLf & _
                                                      "  ELSE (SELECT PrintExpTests FROM tparCalculatedTests CT WHERE C.CalcTestID = CT.CalcTestID) END) AS PrintExpTests " & vbCrLf & _
                                                " FROM   thisWSCalcTestsRelations CTR INNER JOIN thisWSOrderTests OT ON CTR.AnalyzerID = OT.AnalyzerID " & vbCrLf & _
                                                                                                                   " AND CTR.WorkSessionID = OT.WorkSessionID " & vbCrLf & _
                                                                                                                   " AND CTR.HistOrderTestIDCALC = OT.HistOrderTestID " & vbCrLf & _
                                                                                    " INNER JOIN thisCalculatedTests C ON OT.HistTestID = C.HistCalcTestID AND OT.TestType = 'CALC' " & vbCrLf & _
                                                " WHERE  CTR.AnalyzerID = '" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                                " AND    CTR.HistOrderTestIDCALC IN (" & lstCalcOrderTestsID & ") " & vbCrLf

                        Dim myDataSet As New HisWSCalcTestRelations
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.thisWSCalcTestsRelations)
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

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "thisWSCalcTestsRelationsDAO.GetExpTestsToExclude", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' When a result for a Calculated Test is deleted, delete the link between the informed HistOrderTestID and the HistOrderTestID of all Tests 
        ' ''' included in its formula
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <param name="pHistOrderTestIDCALC">Identifier of the Order Test of a Calculated Test in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing success/error information</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 01/07/2013
        ' ''' </remarks>
        'Public Function DeleteByHistOrderTestIDCALC(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                            ByVal pWorkSessionID As String, ByVal pHistOrderTestIDCALC As Integer) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            resultData.HasError = True
        '            resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
        '        Else
        '            Dim cmdText As String = " DELETE FROM thisWSCalcTestsRelations " & vbCrLf & _
        '                                    " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
        '                                    " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
        '                                    " AND    HistOrderTestIDCALC  = " & pHistOrderTestIDCALC.ToString & vbCrLf

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
        '        GlobalBase.CreateLogActivity(ex.Message, "thisWSCalcTestsRelationsDAO.DeleteByHistOrderTestIDCALC", EventLogEntryType.Error, False)
        '    End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' Get the HistOrderTestID of all Calculated Tests in which formula the informed HistOrderTestID is included
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <param name="pHistOrderTestID">Identifier of the Order Test in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing a typed DataSet HisWSCalcTestRelations with the list of HistOrderTestIDs of Calculated
        ' '''          Tests in which formula the informed HistOrderTestID is included</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 01/07/2013
        ' ''' </remarks>
        'Public Function ReadByHistOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                      ByVal pWorkSessionID As String, ByVal pHistOrderTestID As Integer) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT * FROM thisWSCalcTestsRelations " & vbCrLf & _
        '                                        " WHERE  AnalyzerID = N'" & pAnalyzerID.Trim.Replace("'", "''") & "' " & vbCrLf & _
        '                                        " AND    WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
        '                                        " AND    HistOrderTestID = " & pHistOrderTestID.ToString & vbCrLf

        '                Dim myWSCalcRelations As New HisWSCalcTestRelations
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myWSCalcRelations.thisWSCalcTestsRelations)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myWSCalcRelations
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "thisWSCalcTestsRelationsDAO.ReadByHistOrderTestID", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace

