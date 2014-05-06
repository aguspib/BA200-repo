Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types
Imports System.Text

Public Class tcfgReportsTestsSortingDAO
    Inherits DAOBase

#Region "Other Methods"
    ''' <summary>
    ''' Get the default sorting for all Tests selecting them from the correspondent table according its Test Type
    ''' Default sorting is the following one:
    ''' 1- Standard Tests:
    '''    a) Preloaded Test
    '''    b) UserTest
    ''' 2- Calculated Tests
    ''' 3- ISE Test
    ''' 4- OFF Systems Test
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ReportsTestsSortingDS with all Tests sorted according
    '''          the default criteria</returns>
    ''' <remarks>
    ''' Created by: TR 25/11/2011
    ''' </remarks>
    Public Function GetDefaultSortedTestList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim myGlobalDataTO As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT 'STD' AS  TestType, TestID, TestName, TestPosition, PreloadedTest " & vbCrLf & _
                                            " FROM   tparTests " & vbCrLf & _
                                            " UNION " & vbCrLf & _
                                            " SELECT 'CALC' AS TestType, CalcTestID AS TestID, CalcTestName AS TestName, CalcTestID AS TestPosition, " & vbCrLf & _
                                            "        PreloadedCalculatedTest AS PreloadedTest " & vbCrLf & _
                                            " FROM   tparCalculatedTests " & vbCrLf & _
                                            " UNION " & vbCrLf & _
                                            " SELECT 'ISE' AS TestType, ISETestID AS TestID, ShortName AS TestName, ISETestID AS TestPosition, " & vbCrLf & _
                                            "        0 AS PreloadedTest " & vbCrLf & _
                                            " FROM   tparISETests " & vbCrLf & _
                                            " UNION " & vbCrLf & _
                                            " SELECT 'OFFS' AS TestType, OffSystemTestID AS TestID, ShortName AS TestName, " & vbCrLf & _
                                            "        OffSystemTestID AS TestPosition, 0 AS PreloadedTest " & vbCrLf & _
                                            " FROM   tparOffSystemTests " & vbCrLf

                    Dim myReportsTestsSortingDS As New ReportsTestsSortingDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myReportsTestsSortingDS.tcfgReportsTestsSorting)
                            myGlobalDataTO.SetDatos = myReportsTestsSortingDS
                        End Using
                    End Using
                End If
            End If
        Catch ex As Exception
            myGlobalDataTO = New GlobalDataTO
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportsTestsSortingDAO.GetDefaultSortedTestList", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Get the list of Tests with the current saved sorting order
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <returns>GlobalDataTO containing a typed DataSet ReportsTestsSortingDS with all Tests sorted according 
    '''          the current saved criteria</returns>
    ''' <remarks>
    ''' Created by:  TR 24/11/2011
    ''' </remarks>
    Public Function GetSortedTestList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
        Dim myGlobalDataTO As GlobalDataTO = Nothing
        Dim dbConnection As SqlClient.SqlConnection = Nothing

        Try
            myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                If (Not dbConnection Is Nothing) Then
                    Dim cmdText As String = " SELECT RTS.TestType, RTS.TestID, RTS.TestPosition, T.TestName, T.PreloadedTest " & vbCrLf & _
                                            " FROM   tcfgReportsTestsSorting RTS INNER JOIN tparTests T ON RTS.TestID = T.TestID " & vbCrLf & _
                                            " WHERE  RTS.TestType = 'STD' " & vbCrLf & _
                                            " UNION " & vbCrLf & _
                                            " SELECT RTS.TestType, RTS.TestID, RTS.TestPosition, CT.CalcTestName AS TestName, " & vbCrLf & _
                                                   " CT.PreloadedCalculatedTest AS PreloadedTes " & vbCrLf & _
                                            " FROM   tcfgReportsTestsSorting RTS INNER JOIN tparCalculatedTests CT ON RTS.TestID = CT.CalcTestID " & vbCrLf & _
                                            " WHERE  RTS.TestType = 'CALC' " & vbCrLf & _
                                            " UNION " & vbCrLf & _
                                            " SELECT RTS.TestType, RTS.TestID, RTS.TestPosition, IT.ShortName AS TestName, 0 AS PreloadedTest " & vbCrLf & _
                                            " FROM   tcfgReportsTestsSorting RTS INNER JOIN tparISETests IT ON RTS.TestID = IT.ISETestID " & vbCrLf & _
                                            " WHERE  RTS.TestType = 'ISE' " & vbCrLf & _
                                            " UNION  " & vbCrLf & _
                                            " SELECT RTS.TestType, RTS.TestID, RTS.TestPosition, OST.ShortName AS TestName, 0 AS PreloadedTest  " & vbCrLf & _
                                            " FROM   tcfgReportsTestsSorting RTS INNER JOIN tparOffSystemTests OST ON RTS.TestID = OST.OffSystemTestID " & vbCrLf & _
                                            " WHERE  RTS.TestType = 'OFFS' " & vbCrLf & _
                                            " ORDER BY RTS.TestPosition  " & vbCrLf

                    Dim myReportsTestsSortingDS As New ReportsTestsSortingDS
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                        Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                            dbDataAdapter.Fill(myReportsTestsSortingDS.tcfgReportsTestsSorting)
                            myGlobalDataTO.SetDatos = myReportsTestsSortingDS
                        End Using
                    End Using
                End If
            End If
        Catch ex As Exception
            myGlobalDataTO = New GlobalDataTO()
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportsTestsSortingDAO.GetSortedTestList", EventLogEntryType.Error, False)
        Finally
            If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        End Try
        Return myGlobalDataTO
    End Function

    ''' <summary>
    ''' Update the position for reports of all Tests 
    ''' </summary>
    ''' <param name="pDBConnection">Open DB Connection</param>
    ''' <param name="pReportsTestsSortingDS">Typed DataSet ReportsTestsSortingDS containing all tests to update</param>
    ''' <returns>GlobalDataTO containing success/error information</returns>
    ''' <remarks>
    ''' Created by: TR 24/11/2011
    ''' </remarks>
    Public Function UpdateTestPosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReportsTestsSortingDS As ReportsTestsSortingDS) _
                                       As GlobalDataTO
        Dim myGlobalDataTO As New GlobalDataTO
        Try
            If (pDBConnection Is Nothing) Then
                'There is not an opened Database Connection
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
            Else
                Dim myGlobalBase As New GlobalBase
                Dim cmdText As New StringBuilder

                For Each testRow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In pReportsTestsSortingDS.tcfgReportsTestsSorting.Rows
                    cmdText.Append(" UPDATE tcfgReportsTestsSorting ")
                    cmdText.Append(" SET    TestPosition = " & testRow.TestPosition.ToString)
                    cmdText.Append(" WHERE  TestType = '" & testRow.TestType & "' ")
                    cmdText.Append(" AND    TestID  = " & testRow.TestID.ToString)
                    cmdText.Append(vbCrLf)
                Next

                'Execute the SQL sentence 
                Dim dbCmd As New SqlClient.SqlCommand
                dbCmd.Connection = pDBConnection
                dbCmd.CommandText = cmdText.ToString()

                myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                myGlobalDataTO.HasError = False
            End If
        Catch ex As Exception
            myGlobalDataTO.HasError = True
            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
            myGlobalDataTO.ErrorMessage = ex.Message

            Dim myLogAcciones As New ApplicationLogManager()
            myLogAcciones.CreateLogActivity(ex.Message, "tcfgReportTemplatesDAO.UpdateTestPosition", EventLogEntryType.Error, False)
        End Try
        Return myGlobalDataTO
    End Function
#End Region
End Class
