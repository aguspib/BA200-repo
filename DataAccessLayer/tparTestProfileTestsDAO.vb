Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Text

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tparTestProfileTestsDAO
          

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a list of Tests to an specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestList">Dataset with structure of table tparTestProfileTests</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  SA 
        ''' Modified by: DL 13/10/2010 - Changed the SQL to include new field TestType
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestList As TestProfileTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pTestList Is Nothing) Then
                    Dim i As Integer = 0
                    Dim recordOK As Boolean = True

                    For Each rowtparTestProfileTest As TestProfileTestsDS.tparTestProfileTestsRow In pTestList.tparTestProfileTests.Rows
                        Dim cmdText As String = ""
                        cmdText &= "INSERT INTO tparTestProfileTests (TestProfileID, TestID, TestType) " & vbCrLf
                        cmdText &= "VALUES ( " & rowtparTestProfileTest.TestProfileID & vbCrLf
                        cmdText &= "       , " & rowtparTestProfileTest.TestID & vbCrLf
                        cmdText &= "       , '" & rowtparTestProfileTest.TestType & "')"

                        'Execute the SQL Sentence
                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        recordOK = (resultData.AffectedRecords = 1)
                        i += 1

                        If (Not recordOK) Then Exit For
                    Next rowtparTestProfileTest

                    If (recordOK) Then
                        resultData.HasError = False
                        resultData.AffectedRecords = i
                        resultData.SetDatos = pTestList
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                        resultData.AffectedRecords = 0
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfileTestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read the TestProfiles in which the specified Test (STD,ISE,OFFS,CALC)) is included.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleTpe">Sample Type code. Optional parameter.</param>
        ''' <param name="pTestType">Type of Test (STD,ISE,OFFS,CALC). Optional parameter.</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfileTestsDS with the list
        '''          of Test Profiles in which the Test is included.</returns>
        ''' <remarks>
        ''' Created by:  TR 18/05/2010
        ''' Modified by: SA 18/10/2010 - Changed the SQL to use this function only for STANDARD TESTS
        '''              TR 25/11/2010 - Added optional parameters pSampleType and pTestType (with default value STD)
        '''              SA 04/01/2011 - For OFF-SYSTEM Tests, verify the TestProfile has the same SampleType defined for the the Test
        '''              WE 21/11/2014 - RQ00035C (BA-1867): change Summary and Parameter description.
        ''' </remarks>
        Public Function ReadByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                     Optional ByVal pSampleTpe As String = "", Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT TPT.TestProfileID AS TestProfileID, TPT.TestID AS TestID, " & _
                                         " T.TestProfileName AS TestProfileName " & _
                                  " FROM   tparTestProfileTests TPT INNER JOIN tparTestProfiles T ON T.TestProfileID = TPT.TestProfileID " & _
                                  " WHERE  TPT.TestID = " & pTestID & _
                                  " AND    TPT.TestType = '" & pTestType & "'"

                        If (pSampleTpe <> "") Then
                            cmdText &= " AND T.SampleType = '" & pSampleTpe & "'"

                        ElseIf (pTestType = "OFFS") Then
                            'For OFF-SYSTEM Tests, verify the TestProfile has the same SampleType defined for the the Test
                            cmdText &= " AND T.SampleType = (SELECT SampleType FROM tparOffSystemTestSamples WHERE OffSystemTestID = " & pTestID & ") "
                        End If

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myDS As New TestProfileTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)

                        dbDataAdapter.Fill(myDS.tparTestProfileTests)
                        resultData.SetDatos = myDS
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfileTestsDAO.ReadByTestProfileID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Profiles defined for SampleTypes diferent of the specified ones and containing the specified Test.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">List of SampleType to exclude from the search</param>
        ''' <param name="pTestType">Type of Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfileTestsDS with the list of Test Profiles</returns>
        ''' <remarks>
        ''' Created by: TR 13/01/2011
        ''' </remarks>
        Public Function ReadByTestIDSpecial(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                            Optional ByVal pSampleType As String = "", Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT TPT.TestProfileID AS TestProfileID, TPT.TestID AS TestID, T.TestProfileName AS TestProfileName " & _
                                  " FROM   tparTestProfileTests TPT INNER JOIN tparTestProfiles T ON T.TestProfileID = TPT.TestProfileID " & _
                                  " WHERE  TPT.TestID   = " & pTestID & _
                                  " AND    TPT.TestType = '" & pTestType & "'"

                        If (pSampleType <> "") Then
                            cmdText &= " AND T.SampleType NOT IN (" & pSampleType & ")"
                        End If

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myDS As New TestProfileTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)

                        dbDataAdapter.Fill(myDS.tparTestProfileTests)
                        resultData.SetDatos = myDS
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfileTestsDAO.ReadByTestIDSpecial", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Remove the list of Tests included in an specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Opened DB Connection</param>
        ''' <param name="pTestProfileID">Identifier of the Test Profile to delete</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks></remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfileID As Long) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    cmdText = " DELETE FROM tparTestProfileTests " & _
                              " WHERE  TestProfileID = " & pTestProfileID

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfileTestsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified Test (Standard, ISE, Off-System or Calculated Test) from all Test Profiles in which it is included.
        ''' When a SampleType is informed, it means that the Test only has to be deleted from all the Test Profiles defined for this SampleType.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Standard Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <param name="pTestType">Test Type Code (STD,ISE,OFFS,CALC). Optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: SA 08/07/2010 - Changed the query when the SampleType has been informed: the INNER JOIN is 
        '''                              not needed; call to DeleteEmptyProfiles moved to the Delegate.
        '''                              Function moved here from tparTestProfiles
        '''              SA 18/10/2010 - Changed the SQL to use this function only for STANDARD TESTS
        '''              WE 24/11/2014 - RQ00035C (BA-1867): Updated Summary and Parameters description.
        ''' </remarks>
        Public Function DeleteByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                 ByVal pTestID As Integer, _
                                                 Optional ByVal pSampleType As String = "", _
                                                 Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    If (pSampleType = "") Then

                        cmdText &= "DELETE FROM tparTestProfileTests " & vbCrLf
                        cmdText &= "WHERE  TestID = " & pTestID & vbCrLf
                        cmdText &= "  AND  TestType = '" & pTestType & "'"
                    Else
                        cmdText &= "DELETE FROM tparTestProfileTests " & vbCrLf
                        cmdText &= "WHERE  TestID = " & pTestID & vbCrLf
                        cmdText &= "  AND  TestType = '" & pTestType & "'" & vbCrLf
                        'AJG
                        'cmdText &= "  AND  TestProfileID IN (SELECT TP.TestProfileID " & vbCrLf
                        cmdText &= "  AND  EXISTS (SELECT TP.TestProfileID " & vbCrLf
                        cmdText &= "              FROM   tparTestProfiles TP " & vbCrLf
                        cmdText &= "              WHERE  TP.SampleType = '" & pSampleType.Trim & "' AND tparTestProfileTests.TestProfileID = TP.TestProfileID)"
                    End If

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfileTestsDAO.DeleteByTestIDSampleType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Remove an specific Test from the specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pTestProfileID">Test Profile Identifier</param>
        ''' <param name="pTestType">Test Type</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 13/01/2011
        ''' </remarks>
        Public Function DeleteByTestIDAndTestProfileID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                 ByVal pTestID As Integer, ByVal pTestProfileID As Integer, _
                                                 ByVal pTestType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    cmdText &= " DELETE FROM tparTestProfileTests " & vbCrLf
                    cmdText &= " WHERE  TestID = " & pTestID & vbCrLf
                    cmdText &= " AND  TestProfileID = " & pTestProfileID
                    cmdText &= " AND TestType = '" & pTestType & "'"


                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfileTestsDAO.DeleteByTestIDSampleType", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get all different Test Types included in the specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileID">Identifier of the Test Profile</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfileTestsDS with the list of 
        '''          different Test Types included in the specified Test Profile</returns>
        ''' <remarks>
        ''' Created by:  DL 13/10/2010
        ''' Modified by: SA 18/10/2010 - Changed the SQL by a SELECT DISTINCT; function moved to "Other Methods" Region
        ''' </remarks>
        Public Function GetTestTypes(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfileID As Integer) As GlobalDataTO

            Dim resultData As New GlobalDataTO()
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT DISTINCT TestType " & _
                                  " FROM   tparTestProfileTests " & _
                                  " WHERE  TestProfileID = " & pTestProfileID

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myTestProfileTestsDS As New TestProfileTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myTestProfileTestsDS.tparTestProfileTests)
                        resultData.SetDatos = myTestProfileTestsDS
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfileTestsDAO.GetTestTypes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Tests (all Test Types) included in a specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileID">Test Profile Identifier</param>
        ''' <returns>GlobalDataTO containing a typed Dataset TestProfileTestsDS with the list of Tests included
        '''          in the specified Test Profile plus the Name and Position of each one of them</returns>
        ''' <remarks>
        ''' Modified by: DL 14/10/2010 - Changed the SQL to get Tests of all Test Types included in the Profile
        '''              SA 19/10/2010 - Changed the SQL to get also the required details of all Tests included 
        '''                              in the specified Profile
        '''              SA 22/10/2010 - Filter ISE Tests by Enabled=True
        '''              DL 26/11/2010 - Get also Off-System Tests
        '''              TR 09/03/2011 - Add the FactoryCalib row on the Standard Test
        '''              DL 22/07/2013 - Filter by Sample type. Bug#1181
        '''              AG 01/09/2014 - BA-1869 Add the Available row
        ''' </remarks>
        Public Function ReadByTestProfileID(ByVal pDBConnection As SqlClient.SqlConnection, _
                                            ByVal pTestProfileID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        'DL 22/07/2013
                        cmdText &= "    SELECT TPT.TestProfileID, TPT.TestType, TPT.TestID, T.TestName, T.TestPosition, T.PreloadedTest, 1 AS TestTypePosition, TS.FactoryCalib, T.Available " & vbCrLf
                        cmdText &= "      FROM tparTestProfileTests TPT INNER JOIN tparTests T ON TPT.TesTID = T.TestID " & vbCrLf
                        cmdText &= "                                INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & vbCrLf
                        cmdText &= "                                INNER JOIN tparTestProfiles TP ON TPT.TestProfileID = TP.TestProfileID and tp.SampleType = ts.SampleType  " & vbCrLf
                        cmdText &= "    WHERE TPT.TestProfileID = " & pTestProfileID & vbCrLf
                        cmdText &= "      AND TPT.TestType = 'STD' " & vbCrLf
                        cmdText &= "UNION " & vbCrLf
                        cmdText &= "   SELECT TPT.TestProfileID, TPT.TestType, CT.CalcTestID AS TestID, CT.CalcTestLongName AS TestName, CT.CalcTestID AS TestPosition, 0 AS PreloadedTest, 2 AS TestTypePosition, 0 as FactoryCalib, CT.Available  " & vbCrLf
                        cmdText &= "     FROM tparTestProfileTests TPT INNER JOIN tparCalculatedTests CT ON TPT.TesTID = CT.CalcTestID " & vbCrLf
                        cmdText &= "    WHERE TPT.TestProfileID = " & pTestProfileID & vbCrLf
                        cmdText &= "      AND TPT.TestType = 'CALC' " & vbCrLf
                        cmdText &= "      AND CT.EnableStatus = 1 " & vbCrLf
                        cmdText &= "UNION " & vbCrLf
                        cmdText &= "   SELECT TPT.TestProfileID, TPT.TestType, IT.ISETestID AS TestID, IT.[Name] AS TestName,  IT.ISETestID AS TestPosition, 0 AS PreloadedTest, 3 AS TestTypePosition, 0 as FactoryCalib, IT.Available  " & vbCrLf
                        cmdText &= "     FROM tparTestProfileTests TPT INNER JOIN tparISETests IT ON TPT.TesTID = IT.ISETestID " & vbCrLf
                        cmdText &= "    WHERE TPT.TestProfileID = " & pTestProfileID & vbCrLf
                        cmdText &= "      AND TPT.TestType = 'ISE' " & vbCrLf
                        cmdText &= "      AND IT.Enabled = 1 " & vbCrLf
                        cmdText &= "UNION " & vbCrLf
                        cmdText &= "   SELECT TPT.TestProfileID, TPT.TestType, OT.OffSystemTestID AS TestID, OT.[Name] AS TestName, OT.OffSystemTestID AS TestPosition, 0 AS PreloadedTest, 4 AS TestTypePosition, 0 as FactoryCalib, OT.Available  " & vbCrLf
                        cmdText &= "     FROM tparTestProfileTests TPT INNER JOIN tparOffSystemTests OT ON TPT.TesTID = OT.OffSystemTestID " & vbCrLf
                        cmdText &= "    WHERE TPT.TestProfileID = " & pTestProfileID & vbCrLf
                        cmdText &= "      AND TPT.TestType = 'OFFS' " & vbCrLf
                        cmdText &= " ORDER BY TestTypePosition, TestPosition"

                        'cmdText = " SELECT TPT.TestProfileID, TPT.TestType, TPT.TestID, T.TestName, T.TestPosition, T.PreloadedTest, " & _
                        '                 " 1 AS TestTypePosition, TS.FactoryCalib " & _
                        '          " FROM   tparTestProfileTests TPT INNER JOIN tparTests T ON TPT.TesTID = T.TestID " & _
                        '          "        INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & _
                        '          "        INNER JOIN tparTestProfiles TP ON TPT.TestProfileID = TP.TestProfileID and tp.SampleType = ts.SampleType  " & _
                        '          " WHERE  TPT.TestProfileID = " & pTestProfileID & _
                        '          " AND    TPT.TestType = 'STD' " & _
                        '          " UNION " & _
                        '          " SELECT TPT.TestProfileID, TPT.TestType, CT.CalcTestID AS TestID, CT.CalcTestLongName AS TestName, " & _
                        '                 " CT.CalcTestID AS TestPosition, 0 AS PreloadedTest, 2 AS TestTypePosition, 0 as FactoryCalib " & _
                        '          " FROM   tparTestProfileTests TPT INNER JOIN tparCalculatedTests CT ON TPT.TesTID = CT.CalcTestID " & _
                        '          " WHERE  TPT.TestProfileID = " & pTestProfileID & _
                        '          " AND    TPT.TestType = 'CALC' " & _
                        '          " AND    CT.EnableStatus = 1 " & _
                        '          " UNION " & _
                        '          " SELECT TPT.TestProfileID, TPT.TestType, IT.ISETestID AS TestID, IT.[Name] AS TestName, " & _
                        '                 " IT.ISETestID AS TestPosition, 0 AS PreloadedTest, 3 AS TestTypePosition, 0 as FactoryCalib " & _
                        '          " FROM   tparTestProfileTests TPT INNER JOIN tparISETests IT ON TPT.TesTID = IT.ISETestID " & _
                        '          " WHERE  TPT.TestProfileID = " & pTestProfileID & _
                        '          " AND    TPT.TestType = 'ISE' " & _
                        '          " AND    IT.Enabled = 1 " & _
                        '          " UNION " & _
                        '          " SELECT TPT.TestProfileID, TPT.TestType, OT.OffSystemTestID AS TestID, OT.[Name] AS TestName, " & _
                        '                 " OT.OffSystemTestID AS TestPosition, 0 AS PreloadedTest, 4 AS TestTypePosition, 0 as FactoryCalib " & _
                        '          " FROM   tparTestProfileTests TPT INNER JOIN tparOffSystemTests OT ON TPT.TesTID = OT.OffSystemTestID " & _
                        '          " WHERE  TPT.TestProfileID = " & pTestProfileID & _
                        '          " AND    TPT.TestType = 'OFFS' " & _
                        '          " ORDER BY TestTypePosition, TestPosition "

                        'DL 22/07/2013

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myDS As New TestProfileTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)

                        dbDataAdapter.Fill(myDS.tparTestProfileTests)
                        resultData.SetDatos = myDS
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfileTestsDAO.ReadByTestProfileID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Tests (all Test Types) using the specified SampleType. If a TestProfile is specified,
        ''' Tests included in it are not returned
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type to filter the Tests</param>
        ''' <param name="pTestProfileID">Optional parameter. When informed, it indicates the Tests included
        '''                              in the informed Test Profile must not be returned</param>
        ''' <returns>GlobalDataTO containing a typed Dataset TestProfileTestsDS with the list of Tests (of all 
        '''          Test Types) not included in the specified Profile</returns>
        ''' <remarks>
        ''' Created by:  SA 20/10/2010
        ''' Modified by: SA 22/10/2010 - Filter ISE Tests by Enabled=True
        '''              DL 26/11/2010 - Get also Off-System Tests
        '''              TR 09/03/2011 - Add the FactoryCalib row on the Standard Test
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        '''              AG 01/09/2014 - BA-1869 Add the Available row
        ''' </remarks>
        Public Function ReadTestsNotInProfile(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleType As String, _
                                              Optional ByVal pTestProfileID As Integer = 0) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        'Get STANDARD Tests
                        cmdText = " SELECT 'STD' AS TestType, T.TestID, T.TestName, T.TestPosition, T.PreloadedTest, TS.FactoryCalib, T.Available  " & _
                                  " FROM   tparTests T INNER JOIN tparTestSamples TS ON T.TestID = TS.TestID " & _
                                  " WHERE  UPPER(TS.SampleType) = UPPER(N'" & pSampleType & "') "
                        '" WHERE  UPPER(TS.SampleType) = '" & pSampleType.ToUpper & "' "
                        If (pTestProfileID <> 0) Then
                            'AJG
                            'cmdText &= " AND T.TestID NOT IN (SELECT TestID FROM tparTestProfileTests " & _
                            '                                " WHERE  TestProfileID = " & pTestProfileID & _
                            '                                " AND    TestType = 'STD') "
                            cmdText &= " AND NOT EXISTS (SELECT TestID FROM tparTestProfileTests " & _
                                                            " WHERE  TestProfileID = " & pTestProfileID & _
                                                            " AND    TestType = 'STD' AND T.TestID = TestID) "
                        End If

                        'Get CALCULATED Tests
                        'AJG
                        'cmdText &= " UNION " & _
                        '           " SELECT 'CALC' AS TestType, CT.CalcTestID AS TestID, CT.CalcTestLongName AS TestName, " & _
                        '                  " CT.CalcTestID AS TestPosition, 0 AS PreloadedTest, 0 as FactoryCalib, CT.Available  " & _
                        '           " FROM   tparCalculatedTests CT " & _
                        '           " WHERE ((CT.UniqueSampleType = 1 AND UPPER(CT.SampleType) = UPPER(N'" & pSampleType & "')) " & _
                        '           " OR     (CT.UniqueSampleType = 0 AND CT.CalcTestID IN (SELECT CalcTestID FROM tparFormulas " & _
                        '                                                                 " WHERE  ValueType = 'TEST' " & _
                        '                                                                 " AND UPPER(SampleType) = UPPER(N'" & pSampleType & "')))) " & _
                        '           " AND    CT.EnableStatus = 1 "

                        cmdText &= " UNION " & _
                                   " SELECT 'CALC' AS TestType, CT.CalcTestID AS TestID, CT.CalcTestLongName AS TestName, " & _
                                          " CT.CalcTestID AS TestPosition, 0 AS PreloadedTest, 0 as FactoryCalib, CT.Available  " & _
                                   " FROM   tparCalculatedTests CT " & _
                                   " WHERE ((CT.UniqueSampleType = 1 AND UPPER(CT.SampleType) = UPPER(N'" & pSampleType & "')) " & _
                                   " OR     (CT.UniqueSampleType = 0 AND EXISTS (SELECT CalcTestID FROM tparFormulas " & _
                                                                                " WHERE  ValueType = 'TEST' " & _
                                                                                " AND UPPER(SampleType) = UPPER(N'" & pSampleType & "') " & _
                                                                                " AND CT.CalcTestID = CalcTestID))) " & _
                                   " AND    CT.EnableStatus = 1 "

                        If (pTestProfileID <> 0) Then
                            'AJG
                            'cmdText &= " AND CT.CalcTestID NOT IN (SELECT TestID FROM tparTestProfileTests " & _
                            '                                      " WHERE  TestProfileID = " & pTestProfileID & _
                            '                                      " AND    TestType = 'CALC') "

                            cmdText &= " AND NOT EXISTS (SELECT TestID FROM tparTestProfileTests " & _
                                                        " WHERE  TestProfileID = " & pTestProfileID & _
                                                        " AND    TestType = 'CALC' AND CT.CalcTestID = TestID) "
                        End If

                        'Get ISE Tests
                        cmdText &= " UNION " & _
                                   " SELECT 'ISE' AS TestType, IT.ISETestID AS TestID, IT.[Name] AS TestName,  " & _
                                          " IT.ISETestID AS TestPosition, 0 AS PreloadedTest, 0 as FactoryCalib, IT.Available  " & _
                                   " FROM   tparISETests IT INNER JOIN tparISETestSamples ITS ON IT.ISETestID = ITS.ISETestID " & _
                                   " WHERE  UPPER(ITS.SampleType) = UPPER(N'" & pSampleType & "') " & _
                                   " AND    IT.Enabled = 1 "

                        If (pTestProfileID <> 0) Then
                            'AJG
                            'cmdText &= " AND IT.ISETestID NOT IN (SELECT TestID FROM tparTestProfileTests " & _
                            '                                    " WHERE  TestProfileID = " & pTestProfileID & _
                            '                                    " AND    TestType = 'ISE') "
                            cmdText &= " AND NOT EXISTS (SELECT TestID FROM tparTestProfileTests " & _
                                                        " WHERE  TestProfileID = " & pTestProfileID & _
                                                        " AND    TestType = 'ISE' AND IT.ISETestID = TestID) "
                        End If

                        'Get OFF-SYSTEM Tests
                        cmdText &= " UNION " & _
                                   " SELECT 'OFFS' AS TestType, OT.OffSystemTestID AS TestID, OT.[Name] AS TestName,  " & _
                                          " OT.OffSystemTestID AS TestPosition, 0 AS PreloadedTest, 0 as FactoryCalib, OT.Available  " & _
                                   " FROM   tparOffSystemTests OT INNER JOIN tparOffSystemTestSamples OTS ON OT.OffSystemTestID = OTS.OffSystemTestID " & _
                                   " WHERE  UPPER(OTS.SampleType) = UPPER(N'" & pSampleType & "') " '& _
                        
                        If (pTestProfileID <> 0) Then
                            'cmdText &= " AND OT.OffSystemTestID NOT IN (SELECT TestID FROM tparTestProfileTests " & _
                            '                                          " WHERE  TestProfileID = " & pTestProfileID & _
                            '                                          " AND    TestType = 'OFFS') "
                            cmdText &= " AND NOT EXISTS (SELECT TestID FROM tparTestProfileTests " & _
                                                                      " WHERE  TestProfileID = " & pTestProfileID & _
                                                                      " AND    TestType = 'OFFS' AND OT.OffSystemTestID = TestID) "
                        End If

                        cmdText &= " ORDER BY TestType, TestPosition "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myDS As New TestProfileTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)

                        dbDataAdapter.Fill(myDS.tparTestProfileTests)
                        resultData.SetDatos = myDS
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfileTestsDAO.ReadTestsNotInProfile", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns selected Test Profiles info to show in a Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="AppLang">Application Language</param>
        ''' <param name="SelectedProfiles">List of selected profiles IDs</param>
        ''' <remarks>
        ''' Created by: RH 30/11/2011
        ''' </remarks>
        Public Function GetTestProfilesForReport(ByVal pDBConnection As SqlClient.SqlConnection, ByVal AppLang As String, _
                                                 Optional ByVal SelectedProfiles As List(Of Integer) = Nothing) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim StrSelectedProfiles As String = String.Empty

                        If Not SelectedProfiles Is Nothing AndAlso SelectedProfiles.Count > 0 Then
                            Dim Profiles As New StringBuilder()
                            For Each p As Integer In SelectedProfiles
                                Profiles.AppendFormat("{0},", p)
                            Next

                            StrSelectedProfiles = String.Format(" WHERE TP.TestProfileID IN ({0})", _
                                                                Profiles.ToString().Substring(0, Profiles.Length - 1))
                        End If

                        Dim cmdText As String = String.Format( _
                                " SELECT TP.TestProfileID, TP.TestProfileName," & _
                                " TP.SampleType + '-' + MR1.ResourceText AS SampleType," & _
                                " MR2.ResourceText AS TestType, (SELECT CASE TPT.TestType" & _
                                " WHEN 'STD' THEN T.TestName WHEN 'ISE' THEN IT.[Name]" & _
                                " WHEN 'CALC' THEN CT.CalcTestLongName WHEN 'OFFS' THEN OT.[Name] END) AS TestName" & _
                                " FROM tparTestProfiles TP INNER JOIN tparTestProfileTests TPT ON" & _
                                " TP.TestProfileID = TPT.TestProfileID INNER JOIN tcfgMasterData MD ON" & _
                                " TP.SampleType = MD.ItemID AND MD.SubTableID = 'SAMPLE_TYPES'" & _
                                " INNER JOIN tfmwMultiLanguageResources MR1 ON MD.ResourceID = MR1.ResourceID" & _
                                " AND MR1.LanguageID = '{0}'" & _
                                " INNER JOIN tfmwPreloadedMasterData PMD ON TPT.TestType = PMD.ItemID" & _
                                " AND PMD.SubTableID = 'TEST_TYPES' INNER JOIN tfmwMultiLanguageResources MR2 ON" & _
                                " PMD.ResourceID = MR2.ResourceID AND MR2.LanguageID = '{0}'" & _
                                " LEFT OUTER JOIN tparTests T ON TPT.TestID = T.TestID LEFT OUTER JOIN tparISETests IT ON" & _
                                " TPT.TestID = IT.ISETestID LEFT OUTER JOIN tparCalculatedTests CT ON" & _
                                " TPT.TestID = CT.CalcTestID LEFT OUTER JOIN tparOffSystemTests OT ON" & _
                                " TPT.TestID = OT.OffSystemTestID{1}" & _
                                " ORDER BY TP.TestProfilePosition, PMD.Position", AppLang, StrSelectedProfiles)

                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            'Fill the DataSet to return 
                            Dim myTestProfileTestsDS As New TestProfileTestsDS
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestProfileTestsDS.tparTestProfileTests)
                                resultData.SetDatos = myTestProfileTestsDS
                            End Using
                        End Using
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfileTestsDAO.GetTestProfilesForReport", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

#End Region

    End Class

End Namespace
