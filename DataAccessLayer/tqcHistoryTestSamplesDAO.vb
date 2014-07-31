
Option Strict On
Option Explicit On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tqcHistoryTestSamplesDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Create a new Test/SampleType in the history table of QC Module 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing all data needed to create the 
        '''                                     Test/SampleType in the history table of QC Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS containing data of the created Test/SampleType
        '''          including the automatically generated QCTestSampleID</returns>
        ''' <remarks>
        ''' Created by:  TR 13/05/2011
        ''' Modified by: SA 03/06/2011 - Removed the For/Next loop
        '''              SA 21/05/2012 - Insert also TestType field (required)
        '''              WE 31/07/2014 - TestLongName added (#1865) to support new screen field Report Name (NULL allowed) in IProgISETest.
        ''' </remarks>
        Public Function CreateNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pHistoryTestSamplesDS Is Nothing) Then
                    Dim cmdText As String = " INSERT INTO tqcHistoryTestSamples(TestType, TestID, SampleType, CreationDate, TestName, TestShortName, " & vbCrLf & _
                                                                              " PreloadedTest, MeasureUnit, DecimalsAllowed, RejectionCriteria, CalculationMode, " & vbCrLf & _
                                                                              " NumberOfSeries, DeletedTest, DeletedSampleType, TestLongName) " & vbCrLf & _
                                             " VALUES ('" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestType & "', " & vbCrLf & _
                                                            pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestID & ", " & vbCrLf & _
                                                     " '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).SampleType & "', " & vbCrLf & _
                                                     " '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).CreationDate.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
                                                    " N'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestName.Replace("'", "''") & "', " & vbCrLf & _
                                                    " N'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestShortName.Replace("'", "''") & "', " & vbCrLf & _
                                                            Convert.ToInt32(IIf(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).PreloadedTest, 1, 0)) & ", " & vbCrLf & _
                                                     " '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).MeasureUnit & "', " & vbCrLf & _
                                                            pHistoryTestSamplesDS.tqcHistoryTestSamples(0).DecimalsAllowed.ToString() & ", " & vbCrLf & _
                                                            pHistoryTestSamplesDS.tqcHistoryTestSamples(0).RejectionCriteria.ToSQLString() & ", " & vbCrLf & _
                                                      "'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).CalculationMode & "', " & vbCrLf & _
                                                            pHistoryTestSamplesDS.tqcHistoryTestSamples(0).NumberOfSeries.ToString & ", " & vbCrLf & _
                                                            Convert.ToInt32(IIf(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).DeletedTest, 1, 0)) & ", " & vbCrLf & _
                                                            Convert.ToInt32(IIf(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).DeletedSampleType, 1, 0)) & ", " & vbCrLf & _
                                                            Convert.ToString(IIf(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).IsTestLongNameNull, "NULL", pHistoryTestSamplesDS.tqcHistoryTestSamples(0).IsTestLongNameNull)) & ") " & vbCrLf & _
                                             " SELECT SCOPE_IDENTITY() "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        'Get the generated ID and assign it to a local variable
                        Dim myNewQCTestSampleID As Integer = CType(dbCmd.ExecuteScalar(), Integer)
                        If (myNewQCTestSampleID > 0) Then
                            'Inform the field in the DS to return
                            pHistoryTestSamplesDS.tqcHistoryTestSamples(0).SetField("QCTestSampleID", myNewQCTestSampleID)
                        End If
                    End Using

                    myGlobalDataTO.SetDatos = pHistoryTestSamplesDS
                    myGlobalDataTO.HasError = False
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all data of the specified QCTestSampleID in the history Test/SampleTypes table in QC Module
        ''' Flags of DeletedSampleType and DeletedTest have to be FALSE, which mean it is the active Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pQCTestSampleID">Identifier of Test/SampleType in QC Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS with data of the Test/SampleType in
        '''          the history table in QC Module</returns>
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pQCTestSampleID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM  tqcHistoryTestSamples  " & vbCrLf & _
                                                " WHERE QCTestSampleID = " & pQCTestSampleID & vbCrLf & _
                                                " AND   DeletedSampleType = 0 " & vbCrLf & _
                                                " AND   DeletedTest = 0 "

                        Dim myHistoryTestSamplesDS As New HistoryTestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHistoryTestSamplesDS.tqcHistoryTestSamples)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myHistoryTestSamplesDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tqcHistoryTestSamplesDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get all TestTypes/TestIDs/SampleTypes in history table in QC Module or optionally, only those that are currently active
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pOnlyActive">Optional parameter. When True, only TestTypes/TestIDs/SampleTypes not marked as deleted and having
        '''                           QC Results pending to cumulate are returned. When False, only TestTypes/TestIDs/SampleTypes having
        '''                           Cumulated QC Results are returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS with all the Tests/SampleTypes</returns>
        ''' <remarks>
        ''' Created by:  TR 26/05/2011
        ''' Modified by: SA 15/06/2011 - When parameter pOnlyActive is False, get all Tests/Sample Types (marked as deleted or not) 
        '''                              having at least a saved Cumulated Serie. Sort returned Tests/Sample Types by Preloaded flag 
        '''                              (System Tests first), Name and SampleType
        '''              SA 05/06/2012 - Added parameter for AnalyzerID and filter subqueries by this value; sort returned data also by TestType DESC
        ''' </remarks>
        Public Function ReadAllNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, Optional ByVal pOnlyActive As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM  tqcHistoryTestSamples " & vbCrLf

                        If (pOnlyActive) Then
                            'Get only active Tests/SampleTypes having QC Results pending to Cumulate
                            cmdText &= " WHERE DeletedSampleType = 0 " & vbCrLf
                            cmdText &= " AND   DeletedTest       = 0 " & vbCrLf
                            cmdText &= " AND   QCTestSampleID IN (SELECT DISTINCT QCTestSampleID FROM tqcResults " & vbCrLf & _
                                                                " WHERE  AnalyzerID   = N'" & pAnalyzerID.Replace("'", "''").Trim & "' " & vbCrLf & _
                                                                " AND    ClosedResult = 0) " & vbCrLf
                        Else
                            'Get all Tests/Sample Types having at least a Cumulated Serie saved
                            cmdText &= " WHERE QCTestSampleID IN (SELECT DISTINCT QCTestSampleID FROM tqcCumulatedResults " & vbCrLf & _
                                                                " WHERE  AnalyzerID   = N'" & pAnalyzerID.Replace("'", "''").Trim & "') " & vbCrLf
                        End If

                        'Sort returned Tests/Sample Types by Preloaded flag (System Tests first), Name and SampleType
                        cmdText &= " ORDER BY TestType DESC, PreloadedTest DESC, TestName, SampleType "

                        Dim myHistoryTestSamplesDS As New HistoryTestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHistoryTestSamplesDS.tqcHistoryTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myHistoryTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.ReadAll ", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all data of the specified Test/SampleType in the history table in QC Module (field QCTestSampleID)
        ''' Flags of DeletedSampleType and DeletedTest have to be FALSE, which mean it is the active Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code: STD or ISE</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS with data of the Test/SampleType in
        '''          the history table in QC Module</returns>
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' Modified by: SA 20/05/2011 - Filter the select by DeletedSampleType=False and DeletedTest=False
        '''              SA 21/05/2012 - Added parameter and filter for TestType field
        ''' </remarks>
        Public Function ReadByTestIDAndSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                     ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * " & vbCrLf & _
                                                " FROM  tqcHistoryTestSamples  " & vbCrLf & _
                                                " WHERE TestType   = '" & pTestType.Trim & "' " & vbCrLf & _
                                                " AND   TestID     = " & pTestID.ToString & vbCrLf & _
                                                " AND   SampleType = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                " AND   DeletedSampleType = 0 " & vbCrLf & _
                                                " AND   DeletedTest = 0 "

                        Dim myHistoryTestSamplesDS As New HistoryTestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHistoryTestSamplesDS.tqcHistoryTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myHistoryTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.ReadByTestIDAndSampleType ", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When basic data of a Test (Name, ShortName, MeasureUnit, and/or DecimalsAllowed) is changed in Tests Programming Screen,
        ''' values are updated for all not delete records the Test has in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing the data of the Test
        '''                                     to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' Pre: TestType = ISE or STD
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' Modified by: SA 03/06/2011 - Removed the For/Next loop
        '''              SA 05/06/2012 - Filter the query also for field TestType
        '''              WE 31/07/2014 - Update DecimalsAllowed if TestType = STD (#1865).
        ''' </remarks>
        Public Function UpdateByTestIDNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pHistoryTestSamplesDS Is Nothing) Then
                    Dim cmdText As String = ""

                    If pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestType = "STD" Then
                        cmdText = " UPDATE tqcHistoryTestSamples " & vbCrLf & _
                                    " SET TestName        = N'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestName.Replace("'", "''") & "', " & vbCrLf & _
                                    " TestShortName   = N'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestShortName.Replace("'", "''") & "', " & vbCrLf & _
                                    " MeasureUnit     = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).MeasureUnit & "', " & vbCrLf & _
                                    " DecimalsAllowed = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).DecimalsAllowed & "' " & vbCrLf & _
                                    " WHERE TestType = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestType.Trim & "' " & vbCrLf & _
                                    " AND   TestID = " & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestID.ToString & vbCrLf & _
                                    " AND   DeletedTest = 0 " & vbCrLf & _
                                    " AND   DeletedSampleType = 0 "
                    ElseIf pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestType = "ISE" Then
                        cmdText = " UPDATE tqcHistoryTestSamples " & vbCrLf & _
                                  " SET TestName        = N'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestName.Replace("'", "''") & "', " & vbCrLf & _
                                  " TestShortName   = N'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestShortName.Replace("'", "''") & "', " & vbCrLf & _
                                  " MeasureUnit     = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).MeasureUnit & "' " & vbCrLf & _
                                  " WHERE TestType = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestType.Trim & "' " & vbCrLf & _
                                  " AND   TestID = " & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestID.ToString & vbCrLf & _
                                  " AND   DeletedTest = 0 " & vbCrLf & _
                                  " AND   DeletedSampleType = 0 "
                    End If

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                    End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.UpdateByTestID ", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When basic data of a Test/SampleType (RejectionCriteria, CalculationMode, and/or NumberOfSeries) is changed in Tests Programming Screen,
        ''' values are updated for all not delete records the Test/SampleType has in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing the data of the Test/SampleType
        '''                                     to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' Pre: TestType = ISE or STD
        ''' <remarks>
        ''' Created by:  TR 10/05/2011
        ''' Modified by: SA 03/06/2011 - Removed the For/Next loop
        '''              SA 15/06/2012 - Filter the query also for field TestType
        '''              WE 31/07/2014 - TestLongName added (#1865) to support new screen field Report Name (NULL allowed) in IProgISETest.
        '''                            - Update DecimalsAllowed if TestType = ISE (#1865).
        ''' </remarks>
        Public Function UpdateByTestIDAndSampleTypeNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf (Not pHistoryTestSamplesDS Is Nothing) Then
                    Dim tstLongName As String = CStr(IIf(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).IsTestLongNameNull, "NULL", pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestLongName))
                    Dim cmdText As String = ""

                    If pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestType = "ISE" Then
                        cmdText = " UPDATE tqcHistoryTestSamples " & vbCrLf & _
                                  " SET    RejectionCriteria = " & ReplaceNumericString(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).RejectionCriteria) & ", " & vbCrLf & _
                                         " CalculationMode   = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).CalculationMode & "', " & vbCrLf & _
                                         " NumberOfSeries    = " & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).NumberOfSeries & ", " & vbCrLf & _
                                         " DecimalsAllowed   = " & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).DecimalsAllowed & ", " & vbCrLf & _
                                         " TestLongName      = " & tstLongName & " " & vbCrLf & _
                                         " WHERE TestType = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestType & "' " & vbCrLf & _
                                         " AND   TestID = " & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestID & vbCrLf & _
                                         " AND   SampleType = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).SampleType & "' " & vbCrLf & _
                                         " AND   DeletedSampleType = 0 "
                    ElseIf pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestType = "STD" Then
                        cmdText = " UPDATE tqcHistoryTestSamples " & vbCrLf & _
                                  " SET    RejectionCriteria = " & ReplaceNumericString(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).RejectionCriteria) & ", " & vbCrLf & _
                                         " CalculationMode   = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).CalculationMode & "', " & vbCrLf & _
                                         " NumberOfSeries    = " & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).NumberOfSeries & ", " & vbCrLf & _
                                         " TestLongName      = " & tstLongName & " " & vbCrLf & _
                                         " WHERE TestType = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestType & "' " & vbCrLf & _
                                         " AND   TestID = " & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestID & vbCrLf & _
                                         " AND   SampleType = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).SampleType & "' " & vbCrLf & _
                                         " AND   DeletedSampleType = 0 "
                    End If

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.UpdateByTestIDAndSampleType ", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' When a SampleType is deleted for a Test or when a Test is deleted, the SampleType is marked as Deleted in 
        ''' the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 11/05/2011
        ''' Modified by: SA 21/05/2012 - Added parameter and filter for TestType field
        ''' </remarks>
        Public Function MarkSampleTypeValuesAsDeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, ByVal pTestID As Integer, _
                                                        ByVal pSampleType As String) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcHistoryTestSamples " & vbCrLf & _
                                            " SET    DeletedSampleType = 1 " & vbCrLf & _
                                            " WHERE  TestType = '" & pTestType.Trim & "' " & vbCrLf & _
                                            " AND    TestID = " & pTestID.ToString & vbCrLf & _
                                            " AND    SampleType = '" & pSampleType.Trim & "' " & vbCrLf & _
                                            " AND    DeletedSampleType = 0 " & vbCrLf & _
                                            " AND    DeletedTest = 0 "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.MarkSampleTypeAsDelete ", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When a Test is deleted, the Test and all its SampleTypes are marked as Deleted in the history table of QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestType">Test Type Code</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 11/05/2011
        ''' Modified by: SA 21/05/2012 - Added parameter and filter for TestType field
        ''' </remarks>
        Public Function MarkTestValuesAsDeleteNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestType As String, _
                                                  ByVal pTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tqcHistoryTestSamples " & vbCrLf & _
                                            " SET    DeletedTest = 1, " & vbCrLf & _
                                                   " DeletedSampleType = 1 " & vbCrLf & _
                                            " WHERE TestType    = '" & pTestType & "' " & vbCrLf & _
                                            " AND   TestID      = " & pTestID & vbCrLf & _
                                            " AND   DeletedTest = 0 "

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.MarkTestValuesAsDelete ", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region

#Region "TO DELETE - OLD FUNCTIONS"
        '''' <summary>
        '''' Create a new Test/SampleType in the history table of QC Module 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing all data needed to create the 
        ''''                                     Test/SampleType in the history table of QC Module</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS containing data of the created Test/SampleType
        ''''          including the automatically generated QCTestSampleID</returns>
        '''' <remarks>
        '''' Created by:  TR 13/05/2011
        '''' Modified by: SA 03/06/2011 - Removed the For/Next loop
        '''' </remarks>
        'Public Function CreateOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

        '        ElseIf (Not pHistoryTestSamplesDS Is Nothing) Then
        '            Dim cmdText As String = " INSERT INTO tqcHistoryTestSamples(TestID, SampleType, CreationDate, TestName, TestShortName, PreloadedTest, " & vbCrLf & _
        '                                                                      " MeasureUnit, DecimalsAllowed, RejectionCriteria, CalculationMode, " & vbCrLf & _
        '                                                                      " NumberOfSeries, DeletedTest, DeletedSampleType) " & vbCrLf & _
        '                                    " VALUES (" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestID & ", " & vbCrLf & _
        '                                           " '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).SampleType & "', " & vbCrLf & _
        '                                           " '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).CreationDate.ToString("yyyyMMdd HH:mm:ss") & "', " & vbCrLf & _
        '                                          " N'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestName.Replace("'", "''") & "', " & vbCrLf & _
        '                                          " N'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestShortName.Replace("'", "''") & "', " & vbCrLf & _
        '                                                  Convert.ToInt32(IIf(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).PreloadedTest, 1, 0)) & ", " & vbCrLf & _
        '                                           " '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).MeasureUnit & "', " & vbCrLf & _
        '                                                  pHistoryTestSamplesDS.tqcHistoryTestSamples(0).DecimalsAllowed.ToString() & ", " & vbCrLf & _
        '                                                  pHistoryTestSamplesDS.tqcHistoryTestSamples(0).RejectionCriteria.ToSQLString() & ", " & vbCrLf & _
        '                                            "'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).CalculationMode & "', " & vbCrLf & _
        '                                                  pHistoryTestSamplesDS.tqcHistoryTestSamples(0).NumberOfSeries.ToString & ", " & vbCrLf & _
        '                                                  Convert.ToInt32(IIf(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).DeletedTest, 1, 0)) & ", " & vbCrLf & _
        '                                                  Convert.ToInt32(IIf(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).DeletedSampleType, 1, 0)) & ") " & vbCrLf & _
        '                                    " SELECT SCOPE_IDENTITY() "

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            'Get the generated ID and assign it to a local variable
        '            Dim myNewQCTestSampleID As Integer = CType(dbCmd.ExecuteScalar(), Integer)
        '            If (myNewQCTestSampleID > 0) Then
        '                'Inform the field in the DS to return
        '                pHistoryTestSamplesDS.tqcHistoryTestSamples(0).SetField("QCTestSampleID", myNewQCTestSampleID)
        '            End If
        '            myGlobalDataTO.SetDatos = pHistoryTestSamplesDS
        '            myGlobalDataTO.HasError = False
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.Create ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Get all Tests/SampleTypes in history table in QC Module or optionally, only those that are currently active
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pOnlyActive">Optional parameter. When True, only Tests/SampleTypes not marked as deleted and having
        ''''                          QC Results pending to cumulate are returned</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS with all the Tests/SampleTypes</returns>
        '''' <remarks>
        '''' Created by:  TR 26/05/2011
        '''' Modified by: SA 15/06/2011 - When parameter pOnlyActive is False, get all Tests/Sample Types (marked as deleted or not) 
        ''''                              having at least a saved Cumulated Serie. Sort returned Tests/Sample Types by Preloaded flag 
        ''''                              (System Tests first), Name and SampleType
        '''' </remarks>
        'Public Function ReadAllOLD(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pOnlyActive As Boolean = False) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT * FROM  tqcHistoryTestSamples " & vbCrLf

        '                If (pOnlyActive) Then
        '                    'Get only active Tests/SampleTypes having QC Results pending to Cumulate
        '                    cmdText &= " WHERE DeletedSampleType = 0 " & vbCrLf
        '                    cmdText &= " AND   DeletedTest       = 0 " & vbCrLf
        '                    cmdText &= " AND   QCTestSampleID IN (SELECT DISTINCT QCTestSampleID FROM tqcResults WHERE ClosedResult = 0) " & vbCrLf
        '                Else
        '                    'Get all Tests/Sample Types having at least a Cumulated Serie saved
        '                    cmdText &= " WHERE QCTestSampleID IN (SELECT QCTestSampleID FROM tqcCumulatedResults) " & vbCrLf
        '                End If

        '                'Sort returned Tests/Sample Types by Preloaded flag (System Tests first), Name and SampleType
        '                cmdText &= " ORDER BY PreloadedTest DESC, TestName, SampleType "

        '                Dim myHistoryTestSamplesDS As New HistoryTestSamplesDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myHistoryTestSamplesDS.tqcHistoryTestSamples)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myHistoryTestSamplesDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.ReadAll ", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get all data of the specified Test/SampleType in the history table in QC Module (field QCTestSampleID)
        '''' Flags of DeletedSampleType and DeletedTest have to be FALSE, which mean it is the active Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type</param>
        '''' <returns>GlobalDataTO containing a typed DataSet HistoryTestSamplesDS with data of the Test/SampleType in
        ''''          the history table in QC Module</returns>
        '''' <remarks>
        '''' Created by:  TR 10/05/2011
        '''' Modified by: SA 20/05/2011 - Filter the select by DeletedSampleType=False and DeletedTest=False
        '''' </remarks>
        'Public Function ReadByTestIDAndSampleTypeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT * " & vbCrLf & _
        '                                        " FROM  tqcHistoryTestSamples  " & vbCrLf & _
        '                                        " WHERE TestID = " & pTestID & vbCrLf & _
        '                                        " AND   SampleType = '" & pSampleType & "' " & vbCrLf & _
        '                                        " AND   DeletedSampleType = 0 " & vbCrLf & _
        '                                        " AND   DeletedTest = 0 "

        '                Dim myHistoryTestSamplesDS As New HistoryTestSamplesDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myHistoryTestSamplesDS.tqcHistoryTestSamples)
        '                    End Using
        '                End Using

        '                resultData.SetDatos = myHistoryTestSamplesDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.ReadByTestIDAndSampleType ", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' When basic data of a Test (Name, ShortName, MeasureUnit, and/or DecimalsAllowed) is changed in Tests Programming Screen,
        '''' values are updated for all not delete records the Test has in the history table of QC Module
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing the data of the Test
        ''''                                     to update</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 10/05/2011
        '''' Modified by: SA 03/06/2011 - Removed the For/Next loop
        '''' </remarks>
        'Public Function UpdateByTestIDOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

        '        ElseIf (Not pHistoryTestSamplesDS Is Nothing) Then
        '            Dim cmdText As String = " UPDATE tqcHistoryTestSamples " & vbCrLf & _
        '                                    " SET TestName        = N'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestName.Replace("'", "''") & "', " & vbCrLf & _
        '                                        " TestShortName   = N'" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestShortName.Replace("'", "''") & "', " & vbCrLf & _
        '                                        " MeasureUnit     = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).MeasureUnit & "', " & vbCrLf & _
        '                                        " DecimalsAllowed = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).DecimalsAllowed & "' " & vbCrLf & _
        '                                    " WHERE TestID = " & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestID & vbCrLf & _
        '                                    " AND   DeletedTest = 0 " & vbCrLf & _
        '                                    " AND   DeletedSampleType = 0 "

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '            myGlobalDataTO.HasError = False
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.UpdateByTestID ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' When basic data of a Test/SampleType (RejectionCriteria, CalculationMode, and/or NumberOfSeries) is changed in Tests Programming Screen,
        '''' values are updated for all not delete records the Test/SampleType has in the history table of QC Module
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pHistoryTestSamplesDS">Typed DataSet HistoryTestSamplesDS containing the data of the Test/SampleType
        ''''                                     to update</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 10/05/2011
        '''' Modified by: SA 03/06/2011 - Removed the For/Next loop
        '''' </remarks>
        'Public Function UpdateByTestIDAndSampleTypeOLD(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            'There is not an Opened Database Connection...
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

        '        ElseIf (Not pHistoryTestSamplesDS Is Nothing) Then
        '            Dim cmdText As String = " UPDATE tqcHistoryTestSamples " & vbCrLf & _
        '                                    " SET    RejectionCriteria = " & ReplaceNumericString(pHistoryTestSamplesDS.tqcHistoryTestSamples(0).RejectionCriteria) & ", " & vbCrLf & _
        '                                           " CalculationMode   = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).CalculationMode & "', " & vbCrLf & _
        '                                           " NumberOfSeries    = " & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).NumberOfSeries & " " & vbCrLf & _
        '                                    " WHERE TestID = " & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).TestID & vbCrLf & _
        '                                    " AND   SampleType = '" & pHistoryTestSamplesDS.tqcHistoryTestSamples(0).SampleType & "' " & vbCrLf & _
        '                                    " AND   DeletedSampleType = 0 "

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '            myGlobalDataTO.HasError = False
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.UpdateByTestIDAndSampleType ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' When a SampleType is deleted for a Test or when a Test is deleted, the SampleType is marked as Deleted in 
        '''' the history table of QC Module
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 11/05/2011
        '''' </remarks>
        'Public Function MarkSampleTypeValuesAsDelete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " UPDATE tqcHistoryTestSamples " & vbCrLf & _
        '                                    " SET    DeletedSampleType = 1 " & vbCrLf & _
        '                                    " WHERE  TestID = " & pTestID & vbCrLf & _
        '                                    " AND    SampleType = '" & pSampleType & "' " & vbCrLf & _
        '                                    " AND    DeletedSampleType = 0 " & vbCrLf & _
        '                                    " AND    DeletedTest = 0 "

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '            myGlobalDataTO.HasError = False
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.MarkSampleTypeAsDelete ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' When a Test is deleted, the Test and all its SampleTypes are marked as Deleted in the history table of QC Module
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <returns>GlobalDataTO containing success/error information</returns>
        '''' <remarks>
        '''' Created by:  TR 11/05/2011
        '''' </remarks>
        'Public Function MarkTestValuesAsDelete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO

        '    Try
        '        If (pDBConnection Is Nothing) Then
        '            myGlobalDataTO.HasError = True
        '            myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
        '        Else
        '            Dim cmdText As String = " UPDATE tqcHistoryTestSamples " & vbCrLf & _
        '                                    " SET    DeletedTest = 1, " & vbCrLf & _
        '                                           " DeletedSampleType = 1 " & vbCrLf & _
        '                                    " WHERE TestID = " & pTestID & vbCrLf & _
        '                                    " AND   DeletedTest = 0 "

        '            Dim dbCmd As New SqlClient.SqlCommand
        '            dbCmd.Connection = pDBConnection
        '            dbCmd.CommandText = cmdText

        '            myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
        '            myGlobalDataTO.HasError = False
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, " tqcHistoryTestSamplesDAO.MarkTestValuesAsDelete ", EventLogEntryType.Error, False)
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region
    End Class
End Namespace

