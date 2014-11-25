Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tparOffSystemTestsDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' Add a new OFF-SYSTEM Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestDS">Typed DataSet OffSystemTestsDS containing the basic data of the OFF-SYSTEM Test to add</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  DL 25/11/2010
        ''' Modified by: SA 03/01/2011
        '''              AG 01/09/2014 - BA-1869 ==> Changed the SQL to inform also new field CustomPosition
        '''              SA 21/11/2014 - BA-2105 ==> Changed the SQL to inform also new fields BiosystemsID and PreloadedOffSystemTest
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestDS As OffSystemTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " INSERT INTO tparOffSystemTests([Name], ShortName, ResultType, Decimals, CustomPosition, Units, " & vbCrLf & _
                                                                           " BiosystemsID, PreloadedOffSystemTest, TS_User, TS_DateTime) " & vbCrLf & _
                                            " VALUES (N'" & pOffSystemTestDS.tparOffSystemTests.First.Name.ToString.Replace("'", "''") & "', " & vbCrLf & _
                                                    " N'" & pOffSystemTestDS.tparOffSystemTests.First.ShortName.ToString.Replace("'", "''") & "', " & vbCrLf & _
                                                     " '" & pOffSystemTestDS.tparOffSystemTests.First.ResultType.ToString & "', " & vbCrLf & _
                                                            ReplaceNumericString(pOffSystemTestDS.tparOffSystemTests.First.Decimals) & ", " & vbCrLf & _
                                                            pOffSystemTestDS.tparOffSystemTests.First.CustomPosition.ToString & ", " & vbCrLf

                    If (pOffSystemTestDS.tparOffSystemTests.First.IsUnitsNull OrElse pOffSystemTestDS.tparOffSystemTests.First.Units = String.Empty) Then
                        cmdText &= " NULL, " & vbCrLf
                    Else
                        cmdText &= " '" & pOffSystemTestDS.tparOffSystemTests.First.Units.ToString & "', " & vbCrLf
                    End If

                    If (pOffSystemTestDS.tparOffSystemTests.First.IsBiosystemsIDNull) Then
                        cmdText &= " NULL, " & vbCrLf
                    Else
                        cmdText &= pOffSystemTestDS.tparOffSystemTests.First.BiosystemsID.ToString & ", " & vbCrLf
                    End If

                    If (pOffSystemTestDS.tparOffSystemTests.First.IsPreloadedOffSystemTestNull) Then
                        cmdText &= " 0, " & vbCrLf
                    Else
                        cmdText &= IIf(pOffSystemTestDS.tparOffSystemTests.First.PreloadedOffSystemTest, 1, 0).ToString & ", " & vbCrLf
                    End If

                    If (pOffSystemTestDS.tparOffSystemTests.First.IsTS_UserNull OrElse pOffSystemTestDS.tparOffSystemTests.First.TS_User = String.Empty) Then
                        'Get the connected Username from the current Application Session
                        Dim currentSession As New GlobalBase
                        cmdText &= " N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "', " & vbCrLf
                    Else
                        cmdText &= " N'" & pOffSystemTestDS.tparOffSystemTests.First.TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (pOffSystemTestDS.tparOffSystemTests.First.IsTS_DateTimeNull) Then
                        'Get the current DateTime
                        cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
                    Else
                        cmdText &= " '" & pOffSystemTestDS.tparOffSystemTests.First.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') " & vbCrLf
                    End If

                    'Finally, get the automatically generated ID for the created OFF-SYSTEM Test
                    cmdText &= " SELECT SCOPE_IDENTITY() "

                    Dim newOffSystemTestID As Integer = 0
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        newOffSystemTestID = CType(dbCmd.ExecuteScalar(), Integer)

                        If (newOffSystemTestID > 0) Then
                            pOffSystemTestDS.tparOffSystemTests(0).BeginEdit()
                            pOffSystemTestDS.tparOffSystemTests(0).SetField("OffSystemTestID", newOffSystemTestID)
                            pOffSystemTestDS.tparOffSystemTests(0).EndEdit()

                            resultData.HasError = False
                            resultData.AffectedRecords = 1
                            resultData.SetDatos = pOffSystemTestDS
                        Else
                            resultData.HasError = True
                            resultData.AffectedRecords = 0
                        End If
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update basic data of the informed OFF-SYSTEM Test 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestDS">Typed DataSet OffSystemTestsDS containing the basic data of the OFF-SYSTEM Test to update</param>
        ''' <returns>GlobalDataTO containing the updated record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  DL 25/11/2010
        ''' Modified by: SA 03/01/2011
        '''              SA 21/11/2014 - Implement Using for SqlCommand
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestDS As OffSystemTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= "UPDATE tparOffSystemTests " & vbCrLf
                    cmdText &= "   SET [Name]     = N'" & pOffSystemTestDS.tparOffSystemTests(0).Name.ToString.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= "     , ShortName  = N'" & pOffSystemTestDS.tparOffSystemTests(0).ShortName.ToString.Replace("'", "''") & "'" & vbCrLf

                    If (String.IsNullOrEmpty(pOffSystemTestDS.tparOffSystemTests(0).Units.ToString)) Then
                        cmdText &= " , Units =  NULL" & vbCrLf
                    Else
                        cmdText &= " , Units =  '" & pOffSystemTestDS.tparOffSystemTests(0).Units.ToString & "'" & vbCrLf
                    End If

                    cmdText &= " , ResultType =  '" & pOffSystemTestDS.tparOffSystemTests(0).ResultType.ToString & "'" & vbCrLf
                    cmdText &= " , Decimals   =   " & pOffSystemTestDS.tparOffSystemTests(0).Decimals & vbCrLf

                    If (String.IsNullOrEmpty(pOffSystemTestDS.tparOffSystemTests(0).TS_User.ToString)) Then
                        'Get the connected Username from the current Application Session
                        Dim currentSession As New GlobalBase
                        cmdText &= " , TS_User = N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "'" & vbCrLf
                    Else
                        cmdText &= " , TS_User = N'" & pOffSystemTestDS.tparOffSystemTests(0).TS_User.Trim.Replace("'", "''") & "'" & vbCrLf
                    End If

                    If (String.IsNullOrEmpty(pOffSystemTestDS.tparOffSystemTests(0).TS_DateTime.ToString)) Then
                        'Get the current DateTime
                        cmdText &= " , TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
                    Else
                        cmdText &= " , TS_DateTime = '" & pOffSystemTestDS.tparOffSystemTests(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
                    End If

                    cmdText &= " WHERE OffSystemTestID = " & pOffSystemTestDS.tparOffSystemTests(0).OffSystemTestID

                    'Execute the SQL Sentence
                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery

                        If (resultData.AffectedRecords = 1) Then
                            resultData.HasError = False
                            resultData.SetDatos = pOffSystemTestDS
                        End If
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified OFF-SYSTEM Test 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestID">Identifier of the OFF-SYSTEM Test to delete</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  DL 25/11/2010
        ''' Modified by: SA 21/11/2014 - Implement Using for SqlCommand
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparOffSystemTests " & vbCrLf & _
                                            " WHERE  OffSystemTestID = " & pOffSystemTestID.ToString

                    'Execute the SQL Sentence
                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified OFF-SYSTEM Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pID">Unique OffSystem Test Identifier (OffSystemTestID or BiosystemsID)</param>
        ''' <param name="pSearchByBiosystemsID">When TRUE, the search is executed by field BiosystemsID instead of by field OffSystemTestID.
        '''                                     Optional parameter with FALSE as default value</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsDS with data of the specified OffSystem Test</returns>
        ''' <remarks>
        ''' Created by:  DL 25/11/2010 
        ''' Modified by: SA 20/11/2014 - BA-2105 ==> Added optional parameter pSearchByBiosystemsID to allow search the OffSystem Test by 
        '''                                          BiosystemsID instead of by OffSystemTestID (needed in UpdateVersion process)
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pID As Integer, Optional ByVal pSearchByBiosystemsID As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OT.*, OTS.SampleType, OTS.DefaultValue, OTS.ActiveRangeType " & vbCrLf & _
                                                " FROM   tparOffSystemTests OT INNER JOIN tparOffSystemTestSamples OTS ON OT.OffSystemTestID = OTS.OffSystemTestID " & vbCrLf

                        If (Not pSearchByBiosystemsID) Then
                            cmdText &= " WHERE OT.OffSystemTestID = " & pID.ToString
                        Else
                            cmdText &= " WHERE OT.BiosystemsID = " & pID.ToString
                        End If

                        Dim myOffSystemTestsData As New OffSystemTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOffSystemTestsData.tparOffSystemTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOffSystemTestsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined OFF-SYSTEM Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffsystemTestsDS with the list of offsystemTests</returns>
        ''' <remarks>
        ''' Created by:  DL 25/11/2010
        ''' Modified by: SA 21/11/2014 - Implement Using for SqlCommand and SqlDataAdapter
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparOffSystemTests " & vbCrLf

                        Dim myOffSystemTestsData As New OffSystemTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOffSystemTestsData.tparOffSystemTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOffSystemTestsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined OFF-SYSTEM Tests using the specified SampleType 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCustomizedTestSelection">FALSE same order as until 3.0.2 / When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet offsystemTestsDS with data of the system Tests using
        '''          the specified SampleType</returns>
        ''' <remarks>
        ''' Created by:  DL 25/11/2010
        ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (BT #1112)
        '''              AG 01/09/2014 - BA-1869 ==> EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        '''              SA 21/11/2014 - Implement Using for SqlCommand and SqlDataAdapter 
        ''' </remarks>
        Public Function ReadBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleType As String, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OST.OffSystemTestID, OST.ShortName, OST.Name " & vbCrLf & _
                                                " FROM   tparOffSystemTests OST INNER JOIN tparOffSystemTestSamples OSTS ON OST.OffSystemTestID = OSTS.OffSystemTestID " & vbCrLf & _
                                                " WHERE  OSTS.SampleType = '" & pSampleType & "' " & vbCrLf

                        'AG 01/09/2014 - BA-1869
                        If (pCustomizedTestSelection) Then cmdText &= " AND OST.Available = 1 ORDER BY OST.CustomPosition ASC "

                        Dim myOffsystemTestsDS As New OffSystemTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOffsystemTestsDS.tparOffSystemTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOffsystemTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ReadBySampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field InUse for the informed OffSystem Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">OffSystem Test Identifier</param>
        ''' <param name="pInUseFlag">InUse value to update for the informed OffSystem Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 08/05/2013
        ''' Modified by: SA 21/11/2014 - Implement Using for SqlCommand
        ''' </remarks>
        Public Function UpdateInUseByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pInUseFlag As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tparOffSystemTests " & vbCrLf & _
                                            " SET    InUse = " & Convert.ToInt32(IIf(pInUseFlag, 1, 0)) & vbCrLf & _
                                            " WHERE  OffSystemTestID = " & pTestID.ToString

                    Using cmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.UpdateInUseByTestID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all OFF-SYSTEM Tests added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for OFF-SYSTEM test that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  AG 10/12/2010 - Based on UpdateInUseFlag from tparCalculatedTestDAO
        ''' Modified by: SA 21/11/2014 - Implement Using for SqlCommand
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                        ByVal pFlag As Boolean, Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    If (Not pUpdateForExcluded) Then
                        cmdText = " UPDATE tparOffSystemTests " & vbCrLf & _
                                  " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & vbCrLf & _
                                  " WHERE  OffSystemTestID IN (SELECT DISTINCT WSOT.TestID " & vbCrLf & _
                                                             " FROM   vwksWSOrderTests WSOT " & vbCrLf & _
                                                             " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                             " AND    WSOT.AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                             " AND    WSOT.SampleClass   = 'PATIENT' " & vbCrLf & _
                                                             " AND    WSOT.TestType      = 'OFFS') " & vbCrLf
                    Else
                        cmdText = " UPDATE tparOffSystemTests " & vbCrLf & _
                                  " SET    InUse = 0 " & vbCrLf & _
                                  " WHERE  OffSystemTestID NOT IN (SELECT DISTINCT WSOT.TestID " & vbCrLf & _
                                                                 " FROM   vwksWSOrderTests WSOT " & vbCrLf & _
                                                                 " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                                 " AND    WSOT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                                 " AND    WSOT.SampleClass = 'PATIENT' " & vbCrLf & _
                                                                 " AND    WSOT.TestType = 'OFFS') " & vbCrLf & _
                                  " AND    InUse = 1 " & vbCrLf
                    End If

                    Using cmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.UpdateInUseFlag", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Update field LISValue for the informed OffSystem Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOFFSystemTestID">OffSystem Test Identifier</param>
        ''' <param name="pLISValue">LIS mapping value to update for the informed OffSystem Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 04/03/2013
        ''' Modified by: SA 21/11/2014 - Implement Using for SqlCommand. Added Replace function for update of LISValue
        ''' </remarks>
        Public Function UpdateLISValueByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOFFSystemTestID As Integer, pLISValue As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim currentSession As New GlobalBase
                    Dim cmdText As String = " UPDATE tparOffSystemTests " & vbCrLf & _
                                            " SET    LISValue = N'" & pLISValue.Trim.Replace("'", "''") & "', " & vbCrLf & _
                                                   " TS_User  = N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "', " & vbCrLf & _
                                                   " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf & _
                                            " WHERE  OffSystemTestID = " & pOFFSystemTestID.ToString

                    Using cmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = cmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.UpdatepdateLISValueByTestID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update fields Name and ShortName for the informed OFF-SYSTEM Test 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestDS">Typed DataSet OffSystemTestsDS containing the data of the OFF-SYSTEM Test to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 20/11/2014 - BA-2105
        ''' </remarks>
        Public Function UpdateTestNames(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestDS As OffSystemTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " UPDATE tparOffSystemTests " & vbCrLf & _
                                            " SET    [Name]     = N'" & pOffSystemTestDS.tparOffSystemTests(0).Name.ToString.Replace("'", "''") & "', " & vbCrLf & _
                                                   " ShortName  = N'" & pOffSystemTestDS.tparOffSystemTests(0).ShortName.ToString.Replace("'", "''") & "', " & vbCrLf

                    If (pOffSystemTestDS.tparOffSystemTests.First.IsTS_UserNull OrElse pOffSystemTestDS.tparOffSystemTests.First.TS_User = String.Empty) Then
                        'Get the connected Username from the current Application Session
                        Dim currentSession As New GlobalBase
                        cmdText &= " TS_User = N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "', " & vbCrLf
                    Else
                        cmdText &= " TS_User = N'" & pOffSystemTestDS.tparOffSystemTests(0).TS_User.Trim.Replace("'", "''") & "', " & vbCrLf
                    End If

                    If (pOffSystemTestDS.tparOffSystemTests.First.IsTS_DateTimeNull) Then
                        'Get the current DateTime
                        cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf
                    Else
                        cmdText &= " TS_DateTime = '" & pOffSystemTestDS.tparOffSystemTests(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "'" & vbCrLf
                    End If

                    cmdText &= " WHERE OffSystemTestID = " & pOffSystemTestDS.tparOffSystemTests(0).OffSystemTestID

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.UpdateTestNames", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Verify if there is already a OFF-SYSTEM Test with the informed OFF SYSTEM Test Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestName">OFF-SYSTEM Test Name to be validated</param>
        ''' <param name="pNameToSearch">Value indicating which is the name to validate: the short name or the long one</param>
        ''' <param name="pOffSystemTestID">OFF-SYSTEM Test Identifier. It is an optional parameter informed
        '''                                only in case of updation</param>
        ''' <param name="pReturnOFFSTestsDS">When TRUE, the function will return an OffSystemTestsDS instead a Boolean value. It is 
        '''                                  an optional parameter with default value FALSE</param>
        ''' <returns>GlobalDataTO containing a Boolean value (True if there is another OFF-SYSTEM Test with the same 
        '''          name; otherwise, False) or an OffSystemTestsDS, depending on value of optional parameter pReturnOFFSTestsDS</returns>
        ''' <remarks>
        ''' Created by:  SA 03/01/2011
        ''' Modified by: XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        '''              SA 21/11/2014 - BA-2105 ==> Added optional parameter pReturnOFFSTestsDS to allow return an OffSystemTestsDS instead
        '''                                          of a Boolean value when the function is used by UpdateVersion process. Changed both SQL to get 
        '''                                          all fields, not only the OffSystemTestID
        ''' </remarks>
        Public Function ExistsOffSystemTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestName As String, ByVal pNameToSearch As String, _
                                            Optional ByVal pOffSystemTestID As Integer = 0, Optional ByVal pReturnOFFSTestsDS As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty

                        If (pNameToSearch = "NAME") Then
                            cmdText = " SELECT * FROM   tparOffSystemTests " & vbCrLf & _
                                      " WHERE  UPPER(ShortName) = UPPER(N'" & pOffSystemTestName.Trim.Replace("'", "''") & "') " & vbCrLf

                        ElseIf (pNameToSearch = "FNAME") Then
                            cmdText = " SELECT * FROM tparOffSystemTests " & vbCrLf & _
                                      " WHERE UPPER([Name]) = UPPER(N'" & pOffSystemTestName.Trim.Replace("'", "''") & "') " & vbCrLf
                        End If

                        'In case of update, exclude the OFF-SYSTEM Test from the validation
                        If (pOffSystemTestID <> 0) Then cmdText &= " AND OffSystemTestID <> " & pOffSystemTestID

                        Dim myOffSystemTestsDS As New OffSystemTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOffSystemTestsDS.tparOffSystemTests)
                            End Using
                        End Using

                        If (Not pReturnOFFSTestsDS) Then
                            resultData.SetDatos = (myOffSystemTestsDS.tparOffSystemTests.Rows.Count > 0)
                            resultData.HasError = False
                        Else
                            resultData.SetDatos = myOffSystemTestsDS
                            resultData.HasError = False
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ExistsOffSystemTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets all OFFS Tests ordered by CustomPosition (returned columns: TestType, TestID, CustomPosition As TestPosition, PreloadedTest, Available)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ReportsTestsSortingDS</returns>
        ''' <remarks>
        ''' Created by:  AG 02/09/2014 - BA-1869
        ''' Modified by: WE 25/11/2014 - RQ00035C (BA-1867): Change in query string due to new field 'PreloadedOffSystemTest'.
        ''' </remarks>
        Public Function GetCustomizedSortedTestSelectionList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Use ShortName as TestName in the same way as method tcfgReportsTestsSortingDAO.GetSortedTestList
                        Dim cmdText As String = " SELECT  'OFFS' AS TestType, OffSystemTestID AS TestID, CustomPosition AS TestPosition, " & vbCrLf & _
                                                         " ShortName AS TestName, PreloadedOffSystemTest AS PreloadedTest, Available FROM tparOffSystemTests " & vbCrLf & _
                                                " ORDER BY CustomPosition ASC "

                        Dim myDataSet As New ReportsTestsSortingDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDataSet.tcfgReportsTestsSorting)
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
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "tparOffSystemTestsDAO.GetCustomizedSortedTestSelectionList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the last Custom Position
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an integer value</returns>
        ''' <remarks>
        ''' Created by: AG 01/09/2014 - BA-1869
        ''' </remarks>
        Public Function GetLastCustomPosition(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(CustomPosition) FROM tparOffSystemTests "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                        End Using
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.GetLastCustomPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of the specified OFF-SYSTEM Test and, for the informed SampleType, value of field ActiveRangeType (type of defined Reference 
        ''' Ranges for the Test/SampleType)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of the OFF-SYSTEM Test</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsDS with data of the specified TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA 24/01/2011
        ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (BT #1112)
        '''              SA 21/11/2014 - Implement Using for SqlCommand and SqlDataAdapter
        ''' </remarks>
        Public Function ReadByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OST.*, OSTS.ActiveRangeType, OSTS.DefaultValue " & vbCrLf & _
                                                " FROM   tparOffSystemTests OST INNER JOIN tparOffSystemTestSamples OSTS ON OST.OffSystemTestID = OSTS.OffSystemTestID " & vbCrLf & _
                                                " WHERE  OST.OffSystemTestID = " & pTestID.ToString & vbCrLf & _
                                                " AND    OSTS.SampleType     = '" & pSampleType & "' " & vbCrLf

                        Dim myOffsystemTestsDS As New OffSystemTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOffsystemTestsDS.tparOffSystemTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOffsystemTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ReadByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified OFF-SYSTEM Test, including the Unit description for Quantitative Tests 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestID">Identifier of the OFF-SYSTEM Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsDS with data of the specified OFF-SYSTEM Test</returns>
        ''' <remarks>
        ''' Created by:  SA 31/01/2011 
        ''' Modified by: SA 21/11/2014 - Implement Using for SqlCommand and SqlDataAdapter
        ''' </remarks>
        Public Function ReadWithUnitDesc(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT OST.OffSystemTestID, OST.[Name], OST.ShortName, OST.Decimals, OST.ResultType, MD.FixedItemDesc AS Units " & vbCrLf & _
                                                " FROM   tparOffSystemTests OST INNER JOIN tcfgMasterData MD ON OST.Units = MD.ItemID " & vbCrLf & _
                                                " WHERE  OST.OffSystemTestID = " & pOffSystemTestID.ToString & vbCrLf & _
                                                " AND    OST.ResultType = 'QUANTIVE' " & vbCrLf & _
                                                " AND    MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.TEST_UNITS.ToString & "' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT OST.OffSystemTestID, OST.[Name], OST.ShortName, OST.Decimals, OST.ResultType, '' AS Units " & vbCrLf & _
                                                " FROM   tparOffSystemTests OST " & vbCrLf & _
                                                " WHERE  OST.OffSystemTestID = " & pOffSystemTestID.ToString & vbCrLf & _
                                                " AND    OST.ResultType = 'QUALTIVE' " & vbCrLf

                        Dim myOffSystemTestsData As New OffSystemTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myOffSystemTestsData.tparOffSystemTests)
                            End Using
                        End Using

                        resultData.SetDatos = myOffSystemTestsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ReadWithUnitDesc", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update (only when informed) columns CustomPosition and Available for OFFS Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestsSortingDS">Typed DataSet ReportsTestsSortingDS containing all Tests to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 03/09/2014 - BA-1869
        ''' </remarks>
        Public Function UpdateCustomPositionAndAvailable(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestsSortingDS As ReportsTestsSortingDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    For Each testrow As ReportsTestsSortingDS.tcfgReportsTestsSortingRow In pTestsSortingDS.tcfgReportsTestsSorting
                        'Check there is something to update in this row
                        If Not (testrow.IsTestPositionNull AndAlso testrow.IsAvailableNull) Then
                            cmdText.Append(" UPDATE tparOffSystemTests SET ")

                            'Update CustomPosition = TestPosition if informed
                            If Not testrow.IsTestPositionNull Then
                                cmdText.Append(" CustomPosition = " & testrow.TestPosition.ToString)
                            End If

                            'Update Available = Available if informed
                            If Not testrow.IsAvailableNull Then
                                'Add coma when required
                                If Not testrow.IsTestPositionNull Then
                                    cmdText.Append(" , ")
                                End If

                                cmdText.Append(" Available = " & CInt(IIf(testrow.Available, 1, 0)))
                            End If

                            cmdText.Append(" WHERE OffSystemTestID  = " & testrow.TestID.ToString)
                            cmdText.Append(vbCrLf)
                        End If
                    Next

                    If (cmdText.ToString.Length <> 0) Then
                        Using dbCmd As New SqlCommand(cmdText.ToString, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.UpdateCustomPositionAndAvailable", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region
    End Class
End Namespace
