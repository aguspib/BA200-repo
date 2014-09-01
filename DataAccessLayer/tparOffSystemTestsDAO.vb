Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

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
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestDS As OffSystemTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""

                    cmdText &= "INSERT INTO tparOffSystemTests([Name], ShortName, Units, ResultType, Decimals, TS_User, TS_DateTime) " & vbCrLf
                    cmdText &= "     VALUES (N'" & pOffSystemTestDS.tparOffSystemTests(0).Name.ToString.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= "           , N'" & pOffSystemTestDS.tparOffSystemTests(0).ShortName.ToString.Replace("'", "''") & "'" & vbCrLf

                    If (String.IsNullOrEmpty(pOffSystemTestDS.tparOffSystemTests(0).Units.ToString)) Then
                        cmdText &= " , NULL" & vbCrLf
                    Else
                        cmdText &= " , '" & pOffSystemTestDS.tparOffSystemTests(0).Units.ToString & "'" & vbCrLf
                    End If

                    cmdText &= " , '" & pOffSystemTestDS.tparOffSystemTests(0).ResultType.ToString & "'" & vbCrLf
                    cmdText &= " , " & ReplaceNumericString(pOffSystemTestDS.tparOffSystemTests(0).Decimals) & vbCrLf


                    If (String.IsNullOrEmpty(pOffSystemTestDS.tparOffSystemTests(0).TS_User.ToString)) Then
                        'Get the connected Username from the current Application Session
                        Dim currentSession As New GlobalBase
                        cmdText &= " , N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "'" & vbCrLf
                    Else
                        cmdText &= " , N'" & pOffSystemTestDS.tparOffSystemTests(0).TS_User.Trim.Replace("'", "''") & "'" & vbCrLf
                    End If

                    If (String.IsNullOrEmpty(pOffSystemTestDS.tparOffSystemTests(0).TS_DateTime.ToString)) Then
                        'Get the current DateTime
                        cmdText &= " , '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                    Else
                        cmdText &= " , '" & pOffSystemTestDS.tparOffSystemTests(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') "
                    End If

                    'Finally, get the automatically generated ID for the created OFF-SYSTEM Test
                    cmdText &= " SELECT SCOPE_IDENTITY() "

                    Dim dbCmd As New SqlClient.SqlCommand() With {.Connection = pDBConnection, .CommandText = cmdText}

                    Dim newOffSystemTestID As Integer
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
                    Dim dbCmd As New SqlCommand() With {.Connection = pDBConnection, .CommandText = cmdText}

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                        resultData.SetDatos = pOffSystemTestDS
                    End If
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
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal pOffSystemTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE FROM tparOffSystemTests " & _
                              " WHERE  OffSystemTestID = " & pOffSystemTestID

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand() With {.Connection = pDBConnection, .CommandText = cmdText}

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
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
        ''' Get the list of all defined OFF-SYSTEM Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffsystemTestsDS with the list of offsystemTests</returns>
        ''' <remarks>
        ''' Created by: DL 25/11/2010
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= " SELECT * " & vbCrLf
                        cmdText &= " FROM   tparOffSystemTests " & vbCrLf

                        Dim dbCmd As New SqlClient.SqlCommand() With {.Connection = dbConnection, .CommandText = cmdText}

                        'Fill the DataSet to return 
                        Dim myOffSystemTestsData As New OffSystemTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)

                        dbDataAdapter.Fill(myOffSystemTestsData.tparOffSystemTests)
                        resultData.SetDatos = myOffSystemTestsData
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified OFF-SYSTEM Test 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestID">Identifier of the offsystem Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet offsystemTestsDS with data of the specified off-system Test</returns>
        ''' <remarks>
        ''' Created by: DL 25/11/2010 
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, _
                             ByVal pOffSystemTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= " SELECT *" & vbCrLf
                        cmdText &= " FROM   tparOffSystemTests  " & vbCrLf
                        cmdText &= " WHERE  OffSystemTestID = " & pOffSystemTestID

                        Dim dbCmd As New SqlClient.SqlCommand() With {.Connection = dbConnection, .CommandText = cmdText}

                        'Fill the DataSet to return 
                        Dim myOffSystemTestsData As New OffSystemTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myOffSystemTestsData.tparOffSystemTests)

                        resultData.SetDatos = myOffSystemTestsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' AG 01/09/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function ReadBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, _
                                         ByVal pSampleType As String, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= " SELECT OST.OffSystemTestID, OST.ShortName, OST.Name " & vbCrLf
                        cmdText &= " FROM   tparOffSystemTests OST INNER JOIN tparOffSystemTestSamples OSTS ON OST.OffSystemTestID = OSTS.OffSystemTestID " & vbCrLf
                        cmdText &= " WHERE  OSTS.SampleType = '" & pSampleType.Replace("'", "''") & "' "

                        'AG 01/09/2014 - BA-1869
                        If pCustomizedTestSelection Then
                            cmdText &= " AND OST.Available = 1 ORDER BY OST.CustomPosition ASC "
                        End If
                        'AG 01/09/2014 - BA-1869

                        Dim dbCmd As New SqlClient.SqlCommand() With {.Connection = dbConnection, .CommandText = cmdText}

                        'Fill the DataSet to return 
                        Dim myOffsystemTestsDS As New OffSystemTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myOffsystemTestsDS.tparOffSystemTests)

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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
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
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pAnalyzerID As String, ByVal pFlag As Boolean, _
                                        Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    If (Not pUpdateForExcluded) Then
                        cmdText = " UPDATE tparOffSystemTests " & _
                                  " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & _
                                  " WHERE  OffSystemTestID IN (SELECT DISTINCT WSOT.TestID " & _
                                                        " FROM   vwksWSOrderTests WSOT " & _
                                                        " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & _
                                                        " AND    WSOT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                        " AND    WSOT.SampleClass = 'PATIENT' " & _
                                                        " AND    WSOT.TestType = 'OFFS') "
                    Else
                        cmdText = " UPDATE tparOffSystemTests " & _
                                  " SET    InUse = 0 " & _
                                  " WHERE  OffSystemTestID NOT IN (SELECT DISTINCT WSOT.TestID " & _
                                                            " FROM   vwksWSOrderTests WSOT " & _
                                                            " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & _
                                                            " AND    WSOT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                            " AND    WSOT.SampleClass = 'PATIENT' " & _
                                                            " AND    WSOT.TestType = 'OFFS') " & _
                                  " AND    InUse = 1 "
                    End If

                    Dim cmd As New SqlCommand() With {.Connection = pDBConnection, .CommandText = cmdText}

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
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
        ''' Update the field InUse by TestID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID"></param>
        ''' <param name="pInUseFlag"></param>
        ''' <returns></returns>
        ''' <remarks>AG 08/05/2013</remarks>
        Public Function UpdateInUseByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pInUseFlag As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " UPDATE tparOffSystemTests " & vbCrLf & _
                              " SET    InUse = " & Convert.ToInt32(IIf(pInUseFlag, 1, 0)) & vbCrLf & _
                              " WHERE  OffSystemTestID = " & pTestID.ToString

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection
                    cmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
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
        ''' Update the LIS Value by the Off System test ID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOFFSystemTestID">Off System Test ID.</param>
        ''' <param name="pLISValue">LIS Value.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValueByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOFFSystemTestID As Integer, pLISValue As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    Dim currentSession As New GlobalBase

                    cmdText &= "UPDATE tparOffSystemTests " & vbCrLf
                    cmdText &= "   SET LISValue = N'" & pLISValue & "'"
                    cmdText &= " , TS_User = N'" & currentSession.GetSessionInfo().UserName.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= " , TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf

                    cmdText &= " WHERE OffSystemTestID = " & pOFFSystemTestID

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand() With {.Connection = pDBConnection, .CommandText = cmdText}

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                    End If
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
        ''' <returns>GlobalDataTO containing a boolean value: True if there is another OFF-SYSTEM Test with the same 
        '''          name; otherwise, False</returns>
        ''' <remarks>
        ''' Created by:  SA 03/01/2011
        ''' Modified by: XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function ExistsOffSystemTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestName As String, _
                                            ByVal pNameToSearch As String, Optional ByVal pOffSystemTestID As Integer = 0) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        If (pNameToSearch = "NAME") Then
                            cmdText = " SELECT OffSystemTestID " & vbCrLf & _
                                      " FROM   tparOffSystemTests " & vbCrLf & _
                                      " WHERE  UPPER(ShortName) = UPPER(N'" & pOffSystemTestName.Trim.Replace("'", "''") & "') " & vbCrLf
                            '" WHERE  UPPER(ShortName) = N'" & pOffSystemTestName.Trim.ToUpper.Replace("'", "''") & "' " & vbCrLf

                        ElseIf (pNameToSearch = "FNAME") Then
                            cmdText = " SELECT OffSystemTestID " & vbCrLf & _
                                      " FROM tparOffSystemTests " & vbCrLf & _
                                      " WHERE UPPER([Name]) = UPPER(N'" & pOffSystemTestName.Trim.Replace("'", "''") & "') " & vbCrLf
                            '" WHERE UPPER([Name]) = N'" & pOffSystemTestName.Trim.ToUpper.Replace("'", "''") & "' " & vbCrLf
                        End If

                        'In case of updation, exclude the OFF-SYSTEM Test from the validation
                        If (pOffSystemTestID <> 0) Then cmdText &= " AND OffSystemTestID <> " & pOffSystemTestID

                        Dim dbCmd As New SqlClient.SqlCommand() With {.Connection = dbConnection, .CommandText = cmdText}

                        'Fill the DataSet to return 
                        Dim myOffSystemTests As New OffSystemTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myOffSystemTests.tparOffSystemTests)

                        resultData.SetDatos = (myOffSystemTests.tparOffSystemTests.Rows.Count > 0)
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ExistsOffSystemTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified OFF-SYSTEM Test and, for the informed SampleType, value of field 
        ''' ActiveRangeType (type of defined Reference Ranges for the Test/SampleType)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of the OFF-SYSTEM Test</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsDS with data of the specified TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA 24/01/2011
        ''' Modified by: XB 04/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' </remarks>
        Public Function ReadByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                  ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT OST.*, OSTS.ActiveRangeType, OSTS.DefaultValue " & _
                                  " FROM   tparOffSystemTests OST INNER JOIN tparOffSystemTestSamples OSTS ON OST.OffSystemTestID = OSTS.OffSystemTestID " & _
                                  " WHERE  OST.OffSystemTestID = " & pTestID & _
                                  " AND    OSTS.SampleType = '" & pSampleType.Replace("'", "''") & "' "
                        '" AND    OSTS.SampleType = '" & pSampleType.ToUpper.Replace("'", "''") & "' "

                        Dim dbCmd As New SqlClient.SqlCommand() With {.Connection = dbConnection, .CommandText = cmdText}

                        'Fill the DataSet to return 
                        Dim myOffsystemTestsDS As New OffSystemTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myOffsystemTestsDS.tparOffSystemTests)

                        resultData.SetDatos = myOffsystemTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ReadByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Created by: SA 31/01/2011 
        ''' </remarks>
        Public Function ReadWithUnitDesc(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = " SELECT OST.OffSystemTestID, OST.[Name], OST.ShortName, OST.Decimals, OST.ResultType, MD.FixedItemDesc AS Units " & _
                                  " FROM   tparOffSystemTests OST INNER JOIN tcfgMasterData MD ON OST.Units = MD.ItemID " & _
                                  " WHERE  OST.OffSystemTestID = " & pOffSystemTestID & _
                                  " AND    OST.ResultType = 'QUANTIVE' " & _
                                  " AND    MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.TEST_UNITS.ToString & "' " & _
                                  " UNION " & _
                                  " SELECT OST.OffSystemTestID, OST.[Name], OST.ShortName, OST.Decimals, OST.ResultType, '' AS Units " & _
                                  " FROM   tparOffSystemTests OST " & _
                                  " WHERE  OST.OffSystemTestID = " & pOffSystemTestID & _
                                  " AND    OST.ResultType = 'QUALTIVE' "

                        Dim dbCmd As New SqlClient.SqlCommand()
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myOffSystemTestsData As New OffSystemTestsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(myOffSystemTestsData.tparOffSystemTests)

                        resultData.SetDatos = myOffSystemTestsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ReadWithUnitDesc", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "TO REVIEW-DELETE?"
        '''' <summary>
        '''' Get the list of all defined Off-system Tests using the specified ResultType 
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pResultType">Sample Type Code</param>
        '''' <returns>GlobalDataTO containing a typed DataSet offsystemTestsDS with data of the system Tests using
        ''''          the specified ResultType</returns>
        '''' <remarks>
        '''' Created by:  DL 25/11/2010
        '''' </remarks>
        'Public Function ReadByResultType(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                 ByVal pResultType As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""

        '                cmdText &= "SELECT *" & vbCrLf
        '                cmdText &= "  FROM tparOffSystemTests" & vbCrLf
        '                cmdText &= " WHERE ResultType = '" & pResultType.ToUpper.Replace("'", "''") & "'"

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim myOffsystemTestsDS As New OffSystemTestsDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(myOffsystemTestsDS.tparOffSystemTests)

        '                resultData.SetDatos = myOffsystemTestsDS
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ReadByResultType", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Search offsystem test data for the informed Test Name (for Import from LIMS process)
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="poffsystemTestName">Offsystem Test Name to search by</param>
        '''' <returns>GlobalDataTO containing a typed DataSet offsystemTestsDS containing data of the 
        ''''          informed offsystem Test</returns>
        '''' <remarks>
        '''' Created by:  DL 25/11/2010
        '''' </remarks>
        'Public Function ReadByName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal poffsystemTestName As String) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO()
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText &= "SELECT *" & vbCrLf
        '                cmdText &= "  FROM tparOffSystemTests " & vbCrLf
        '                cmdText &= " WHERE UPPER([Name]) = N'" & poffsystemTestName.Replace("'", "''").ToUpper & "' " & vbCrLf

        '                Dim cmd As SqlCommand = dbConnection.CreateCommand()
        '                cmd.CommandText = cmdText
        '                cmd.Connection = dbConnection

        '                Dim da As New SqlClient.SqlDataAdapter(cmd)
        '                Dim myOffSystemTestsDS As New OffSystemTestsDS
        '                da.Fill(myOffSystemTestsDS.tparOffSystemTests)

        '                myGlobalDataTO.SetDatos = myOffSystemTestsDS
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ReadByName", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        '''' <summary>
        '''' Search offsystem test data for the informed Test ShortName
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pOffSystemTestShortName">offsystem Test ShortName to search by</param>
        '''' <param name="pOffSystemTestIDToExclude">Identifier of the offsystem Test to exclude (to avoid errors in case of
        ''''                                   updation when the ShortName was not changed</param>
        '''' <returns>GlobalDataTO containing a typed DataSet offsystemTestsDS containing data of the 
        ''''          informed off-system Test</returns>
        '''' <remarks>
        '''' Created by:  DL 25/11/2010
        '''' </remarks>
        'Public Function ReadByShortName(ByVal pDBConnection As SqlClient.SqlConnection, _
        '                                ByVal pOffSystemTestShortName As String, _
        '                                Optional ByVal pOffSystemTestIDToExclude As Integer = -1) As GlobalDataTO
        '    Dim myGlobalDataTO As New GlobalDataTO()
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText &= "SELECT *" & vbCrLf
        '                cmdText &= "  FROM tparOffSystemTests " & vbCrLf
        '                cmdText &= " WHERE UPPER(ShortName) = N'" & pOffSystemTestShortName.Replace("'", "''").ToUpper & "' "

        '                If (pOffSystemTestIDToExclude <> -1) Then cmdText &= "   AND OffSystemTestID <> " & pOffSystemTestIDToExclude


        '                Dim cmd As SqlCommand = dbConnection.CreateCommand()
        '                cmd.CommandText = cmdText
        '                cmd.Connection = dbConnection

        '                Dim da As New SqlClient.SqlDataAdapter(cmd)
        '                Dim myOffSystemTestsDS As New OffSystemTestsDS
        '                da.Fill(myOffSystemTestsDS.tparOffSystemTests)

        '                myGlobalDataTO.SetDatos = myOffSystemTestsDS
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "tparOffSystemTestsDAO.ReadByShortName", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function
#End Region

    End Class

End Namespace
