Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tparISETestsDAO
          

#Region "CRUD"
        ''' <summary>
        ''' Add a new ISETest (NOT USED)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestRow">Typed DataSet ISETestsDS containing the data of the ISETest to add</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 15/10/2010
        ''' Modified by: AG  21/10/2010 - Added field Enabled
        ''' Modified by: AG  21/10/2010 - Parameter is a row not a DS
        '''            : TR  10/05/2013 - Add new field LISValue.
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestRow As ISETestsDS.tparISETestsRow, Optional pIsFactory As Boolean = False) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= "INSERT INTO "

                    If pIsFactory Then cmdText &= GlobalBase.TemporalDBName & "."

                    cmdText &= "dbo.tparISETests" & vbCrLf
                    cmdText &= "           (ISETestID" & vbCrLf
                    cmdText &= "           ,ISE_ResultID" & vbCrLf
                    cmdText &= "           ,Name" & vbCrLf
                    cmdText &= "           ,ShortName" & vbCrLf
                    cmdText &= "           ,Units" & vbCrLf
                    cmdText &= "           ,ISE_Units" & vbCrLf
                    cmdText &= "           ,InUse" & vbCrLf
                    cmdText &= "           ,Enabled" & vbCrLf
                    cmdText &= "           ,TS_User" & vbCrLf
                    cmdText &= "           ,TS_DateTime " & vbCrLf
                    cmdText &= "           ,LISValue) " & vbCrLf

                    cmdText &= "VALUES" & vbCrLf
                    cmdText &= "           (" & pISETestRow.ISETestID & vbCrLf
                    cmdText &= "           ,N'" & pISETestRow.ISE_ResultID.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= "           ,N'" & pISETestRow.Name.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= "           ,N'" & pISETestRow.ShortName.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= "           ,N'" & pISETestRow.Units.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= "           ,N'" & pISETestRow.ISE_Units.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= "           ," & Convert.ToInt32(IIf(pISETestRow.InUse, 1, 0)) & vbCrLf
                    cmdText &= "           ," & Convert.ToInt32(IIf(pISETestRow.Enabled, 1, 0)) & vbCrLf


                    If (pISETestRow.IsTS_UserNull) Then
                        'Get the logged User
                        'Dim currentSession As New GlobalBase
                        cmdText &= "           ,N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "'" & vbCrLf
                    Else
                        cmdText &= "           ,N'" & pISETestRow.TS_User.Trim.Replace("'", "''") & "'" & vbCrLf
                    End If

                    If (pISETestRow.IsTS_DateTimeNull) Then
                        cmdText &= "           ,'" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                    Else
                        cmdText &= "           ,'" & CType(pISETestRow.TS_DateTime.ToString(), DateTime).ToString("yyyyMMdd HH:mm:ss") & "', "
                    End If

                    If pISETestRow.IsLISValueNull Then
                        cmdText &= "           ,NULL"
                    Else
                        cmdText &= "           ,N'" & pISETestRow.LISValue & "') "
                    End If

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                        resultData.SetDatos = pISETestRow
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of an ISETest
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestsRow"></param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 15/10/2010
        ''' Modified by: AG  21/10/2010 - Added field Enabled 
        ''' Modified by: AG  21/10/2010 - Parameter is a row not a DS
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestsRow As ISETestsDS.tparISETestsRow) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " UPDATE tparISETests " & _
                              " SET    ISE_ResultID = N'" & pISETestsRow.ISE_ResultID.ToString.Replace("'", "''") & "', " & _
                              "        Name =  N'" & pISETestsRow.Name.ToString.Replace("'", "''") & "', " & _
                              "        ShortName =  N'" & pISETestsRow.ShortName.ToString.Replace("'", "''") & "', " & _
                              "        Units =  N'" & pISETestsRow.Units.ToString.Replace("'", "''") & "', " & _
                              "        ISE_Units =  N'" & pISETestsRow.ISE_Units.ToString.Replace("'", "''") & "', " & _
                              "        InUse =  " & Convert.ToInt32(IIf(pISETestsRow.InUse, 1, 0)) & ", " & _
                              "        Enabled =  " & Convert.ToInt32(IIf(pISETestsRow.Enabled, 1, 0)) & ", " 'AG 21/10/2010


                    If (pISETestsRow.IsTS_UserNull) Then
                        'Get the logged User
                        'Dim currentSession As New GlobalBase
                        cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                    Else
                        cmdText &= " TS_User = N'" & pISETestsRow.TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    If (pISETestsRow.IsTS_DateTimeNull) Then
                        cmdText &= " TS_DateTime = '" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                    Else
                        cmdText &= " TS_DateTime = '" & CType(pISETestsRow.TS_DateTime.ToString(), DateTime).ToString("yyyyMMdd HH:mm:ss") & "' "
                    End If

                    cmdText &= " WHERE ISETestID = " & pISETestsRow.ISETestID & " "

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                        resultData.SetDatos = pISETestsRow
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified ISETest (NOT USED)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISETests Identifier</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  XBC 15/10/2010
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE FROM tparISETests " & _
                              " WHERE  ISETestID = " & pISETestID

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined ISE Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS with the list of ISETests</returns>
        ''' <remarks>
        ''' Created by: XBC 15/10/2010
        ''' Modified by: SA 21/02/2012 - Changed the function template
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparISETests " & vbCrLf

                        Dim myDS As New ISETestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDS.tparISETests)
                            End Using
                        End Using

                        resultData.SetDatos = myDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified ISE Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">Identifier of the ISE Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS with data of the specified ISE Test</returns>
        ''' <remarks>
        ''' Modified by: SA 21/10/2010 - Name changed from GetByTestID to Read; SQL changed to get all fields (SELECT *)
        '''              SA 21/02/2012 - Changed the function template
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, Optional pIsFactory As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT ISETestID" & vbCrLf
                        cmdText &= "      ,ISE_ResultID" & vbCrLf
                        cmdText &= "      ,Name" & vbCrLf
                        cmdText &= "      ,ShortName" & vbCrLf
                        cmdText &= "      ,Units" & vbCrLf
                        cmdText &= "      ,ISE_Units" & vbCrLf
                        cmdText &= "      ,InUse" & vbCrLf
                        cmdText &= "      ,Enabled" & vbCrLf
                        cmdText &= "      ,TS_User" & vbCrLf
                        cmdText &= "      ,TS_DateTime" & vbCrLf
                        cmdText &= "  FROM "

                        If pIsFactory Then cmdText &= GlobalBase.TemporalDBName & ".dbo."

                        cmdText &= "tparISETests" & vbCrLf

                        cmdText &= " WHERE ISETestID = " & pISETestID & vbCrLf

                        Dim myISETestsData As New ISETestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myISETestsData.tparISETests)
                            End Using
                        End Using

                        resultData.SetDatos = myISETestsData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined ISE Tests using the specified SampleType 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCustomizedTestSelection">FALSE same order as until 3.0.2 / When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS with data of the ISE Tests using
        '''          the specified SampleType</returns>
        ''' <remarks>
        ''' Created by:  DL 21/10/2010
        ''' Modified by: AG 21/10/2010 - Added filter by Enabled = True
        '''              SA 21/02/2012 - Changed the function template
        '''              SA 19/06/2012 - Changed the query to get also field NumberOfControls
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' AG 01/09/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function ReadBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleType As String, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ISET.ISETestID, ISET.ShortName, ISET.Name, ISETS.NumberOfControls " & vbCrLf & _
                                                " FROM   tparISETests ISET INNER JOIN tparISETestSamples ISETS ON ISET.ISETestID = ISETS.ISETestID " & vbCrLf & _
                                                " WHERE  ISETS.SampleType = UPPER(N'" & pSampleType & "') " & vbCrLf & _
                                                " AND    ISET.Enabled = 1 " & vbCrLf

                        'AG 01/09/2014 - BA-1869
                        If pCustomizedTestSelection Then
                            cmdText &= " AND ISET.Available = 1 ORDER BY ISET.CustomPosition ASC "
                        End If
                        'AG 01/09/2014 - BA-1869

                        Dim myISETests As New ISETestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myISETests.tparISETests)
                            End Using
                        End Using

                        resultData.SetDatos = myISETests
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.ReadBySampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search ISE test data for the given Test Name.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection.</param>
        ''' <param name="pISETestName">ISE Test Name to search for.</param>
        ''' <param name="pNameType">Type of search to perform; possible values are: FNAME, NAME, LNAME. Calling method must pass one of these values, otherwise this function will result in an error.</param>
        ''' <param name="pISETestIDToExclude">Identifier of the ISE Test to exclude (to avoid errors in case of
        '''                                   updation when the ShortName was not changed.</param>
        ''' <param name="pIncludeDisabled">If or not to include ISE Test records that have been disabled.</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS containing data of the informed ISE Test.</returns>
        ''' <remarks>
        ''' Created by:  WE 27/08/2014 - #1865. New function to solve problem with incorrect determination of unique name of field 'Name' when using ReadByName.
        '''                              This function replaces the existing functions ReadByName and ReadByShortName.
        '''                              FNAME and NAME (param pNameType) are used for compatibility with former functionality provided by ReadByName and ReadByShortName respectively.
        '''                              LNAME has been introduced to correctly determine the uniqueness of an occurrence of field [Name] in [tparISETests].
        ''' </remarks>
        Public Function ReadName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestName As String, ByVal pNameType As String, _
                                 Optional ByVal pISETestIDToExclude As Integer = -1, Optional pIncludeDisabled As Boolean = False) As GlobalDataTO

            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Dim cmdText As String = ""

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Select Case pNameType

                            Case "FNAME"
                                cmdText = " SELECT * FROM tparISETests " & vbCrLf & _
                                               " WHERE  UPPER([Name]) = UPPER(N'" & pISETestName.Replace("'", "''") & "') " & vbCrLf

                                'SG 14/02/2013 Bug #1134
                                If Not pIncludeDisabled Then
                                    cmdText &= " AND    Enabled = 1 " & vbCrLf
                                End If

                            Case "NAME"
                                cmdText = " SELECT * FROM tparISETests " & vbCrLf & _
                                                        " WHERE  UPPER([ShortName]) = UPPER(N'" & pISETestName.Replace("'", "''") & "') " & vbCrLf

                                If (pISETestIDToExclude <> -1) Then
                                    cmdText &= " AND ISETestID <> " & pISETestIDToExclude & vbCrLf
                                End If

                            Case "LNAME"
                                cmdText = " SELECT * FROM tparISETests " & vbCrLf & _
                                                        " WHERE  UPPER([Name]) = UPPER(N'" & pISETestName.Replace("'", "''") & "') " & vbCrLf

                                If (pISETestIDToExclude <> -1) Then
                                    cmdText &= " AND ISETestID <> " & pISETestIDToExclude & vbCrLf
                                End If

                        End Select

                        Dim myISETests As New ISETestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myISETests.tparISETests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myISETests
                        myGlobalDataTO.HasError = False
                    End If
                End If

            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.ReadName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO

        End Function

        ''' <summary>
        ''' Get the list of all defined ISE Tests using the specified SampleType and ISETestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS with data of the ISE Tests using
        '''          the specified SampleType</returns>
        ''' <remarks>
        ''' Created by:  DL 23/02/2012
        ''' Modified by: XB 05/02/2013 - Upper conversions redundants because the value is already in UpperCase must delete to avoid Regional Settings problems (Bugs tracking #1112)
        ''' Modified by: WE 31/07/2014 - TestLongName added (#1865) to support new screen field Report Name in IProgISETest.
        '''              WE 25/08/2014 - SlopeFactorA2/B2 added (#1865).
        ''' </remarks>
        Public Function ReadByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT ISET.ISETestID, ISET.ShortName, ISET.Name, ISET.Units, ISETS.Decimals, ISETS.TestLongName, ISETS.SlopeFactorA2, ISETS.SlopeFactorB2 " & vbCrLf & _
                                                " FROM   tparISETests ISET INNER JOIN tparISETestSamples ISETS ON ISET.ISETestID = ISETS.ISETestID " & vbCrLf & _
                                                " WHERE  ISET.ISETestID = " & pISETestID & vbCrLf & _
                                                " AND    ISETS.SampleType = '" & pSampleType & "' " & vbCrLf
                        '" AND    ISETS.SampleType = '" & pSampleType.ToUpper & "' " & vbCrLf

                        Dim myISETests As New ISETestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myISETests.tparISETests)
                            End Using
                        End Using

                        resultData.SetDatos = myISETests
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.ReadByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update the LISValue by the ISETestID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pISETestID">ISE Test ID.</param>
        ''' <param name="pLISValue">LIS Value.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValueByTestID(ByVal pDBConnection As SqlClient.SqlConnection, pISETestID As Integer, pLISValue As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    'Dim currentSession As New GlobalBase

                    cmdText = " UPDATE tparISETests " & _
                              " SET    LISValue = N'" & pLISValue & "', "
                    'Get the logged User
                    cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                    'Get current date time
                    cmdText &= " TS_DateTime = '" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "' "

                    cmdText &= " WHERE ISETestID = " & pISETestID

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Other Methods"

        ''' <summary>
        ''' Get all ISE Tests currently linked to the specified Control 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pControlID">Control Identifier</param>
        ''' <remarks>
        ''' Created by:  RH 18/06/2012
        ''' Modified by: SA 18/06/2012 - Changed the query. Data returned in an ISETestsDS
        ''' </remarks>
        Public Function GetAllByControl(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pControlID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT DISTINCT IT.ISETestID, IT.ShortName, IT.Name, ITS.Decimals, IT.ISETestID AS TestPosition, " & vbCrLf & _
                                                                " IT.InUse, ITS.SampleType, ITS.RejectionCriteria, " & vbCrLf & _
                                                                " ITS.QCActive, TC.ActiveControl, MD.FixedItemDesc As Units " & vbCrLf & _
                                                " FROM   tparISETests IT INNER JOIN tparISETestSamples ITS ON IT.ISETestID = ITS.ISETestID " & vbCrLf & _
                                                                       " INNER JOIN tparTestControls TC ON ITS.ISETestID = TC.TestID AND ITS.SampleType = TC.SampleType " & vbCrLf & _
                                                                       " INNER JOIN tcfgMasterData MD  ON IT.Units = MD.ItemID " & vbCrLf & _
                                                " WHERE  TC.ControlID  = " & pControlID & vbCrLf & _
                                                " AND    TC.TestType   = 'ISE' " & vbCrLf & _
                                                " AND    MD.SubTableID = '" & MasterDataEnum.TEST_UNITS.ToString() & "' " & vbCrLf & _
                                                " ORDER BY ITS.SampleType, TestPosition "

                        Dim myISETestDataDS As New ISETestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myISETestDataDS.tparISETests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myISETestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.GetAllByControl", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get information of all ISE Tests (optionally, of the specified SampleType) using Quality Control (those with QCActive=True)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter; when informed,only ISE Tests using the SampleType
        '''                           are returned</param>
        ''' <param name="pCustomizedTestSelection">FALSE same order as until 3.0.2 / When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <remarks>
        ''' Created by:  RH 14/06/2012
        ''' Modified by: SA 18/06/2012 - Changed the query. Data returned in an ISETestsDS
        '''              RH 19/06/2012 - Added field ITS.Decimals to the query
        '''              SA 22/06/2012 - Get also field NumberOfControls from table tparISETestSamples; added optional parameter
        '''                              to allow filter the ISE Tests by SampleType when it is informed
        ''' AG 01/09/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function GetAllWithQCActive(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pSampleType As String = "", Optional ByVal pCustomizedTestSelection As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT IT.ISETestID, IT.ShortName, IT.Name, IT.ISETestID AS TestPosition, " & vbCrLf & _
                                                       " IT.InUse, ITS.SampleType, ITS.RejectionCriteria, ITS.QCActive, 0 AS ActiveControl, " & vbCrLf & _
                                                       " MD.FixedItemDesc As Units, ITS.Decimals, ITS.NumberOfControls " & vbCrLf & _
                                                " FROM   tparISETests IT INNER JOIN tparISETestSamples ITS ON IT.ISETestID = ITS.ISETestID " & vbCrLf & _
                                                                       " INNER JOIN tcfgMasterData MD  ON IT.Units = MD.ItemID " & vbCrLf & _
                                                " WHERE  ITS.QCActive  = 1 " & vbCrLf & _
                                                " AND    MD.SubTableID = '" & MasterDataEnum.TEST_UNITS.ToString() & "' " & vbCrLf

                        If (pSampleType.Trim <> String.Empty) Then cmdText &= " AND ITS.SampleType ='" & pSampleType.Trim & "' " & vbCrLf

                        'AG 01/09/2014 - BA-1869
                        'cmdText &= " ORDER BY ITS.SampleType, TestPosition "
                        If Not pCustomizedTestSelection Then 'Keep the old query with the same order by
                            cmdText &= " ORDER BY ITS.SampleType, TestPosition "
                        Else ' Filter also Available = 1 and order by SelectionPosition
                            cmdText &= " AND IT.Available = 1 ORDER BY ITS.SampleType, IT.CustomPosition "
                        End If
                        'AG 01/09/2014 - BA-1869

                        Dim myISETestDataDS As New ISETestsDS()
                        Using cmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using da As New SqlClient.SqlDataAdapter(cmd)
                                da.Fill(myISETestDataDS.tparISETests)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myISETestDataDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.GetAllWithQCActive", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all ISE Test added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for ISE Test that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 22/10/2010
        ''' Modified by: SA 19/06/2012 - Changed both query to include ISETests requested for SampleClass = CTRL
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
                        cmdText = " UPDATE tparISETests " & vbCrLf & _
                                  " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & vbCrLf & _
                                  " WHERE  ISETestID IN (SELECT DISTINCT WSOT.TestID " & vbCrLf & _
                                                       " FROM   vwksWSOrderTests WSOT " & vbCrLf & _
                                                       " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                       " AND    WSOT.AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                       " AND    WSOT.SampleClass  IN ('CTRL', 'PATIENT') " & vbCrLf & _
                                                       " AND    WSOT.TestType      = 'ISE') " & vbCrLf
                    Else
                        cmdText = " UPDATE tparISETests " & vbCrLf & _
                                  " SET    InUse = 0 " & vbCrLf & _
                                  " WHERE  ISETestID NOT IN (SELECT DISTINCT WSOT.TestID " & vbCrLf & _
                                                           " FROM   vwksWSOrderTests WSOT " & vbCrLf & _
                                                           " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & vbCrLf & _
                                                           " AND    WSOT.AnalyzerID    = '" & pAnalyzerID.Trim & "' " & vbCrLf & _
                                                           " AND    WSOT.SampleClass  IN ('CTRL', 'PATIENT') " & vbCrLf & _
                                                           " AND    WSOT.TestType      = 'ISE') " & vbCrLf & _
                                  " AND    InUse = 1 " & vbCrLf
                    End If

                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        myGlobalDataTO.AffectedRecords = dbCmd.ExecuteNonQuery()
                        myGlobalDataTO.HasError = False
                    End Using
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.UpdateInUseFlag", EventLogEntryType.Error, False)
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
                    cmdText = " UPDATE tparISETests " & vbCrLf & _
                              " SET    InUse = " & Convert.ToInt32(IIf(pInUseFlag, 1, 0)) & vbCrLf & _
                              " WHERE  ISETestID = " & pTestID.ToString

                    Dim cmd As New SqlCommand
                    cmd.Connection = pDBConnection
                    cmd.CommandText = cmdText

                    myGlobalDataTO.AffectedRecords = cmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.UpdateInUseByTestID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Gets all ISE tests order by CustomPosition (return columns: TestType, TestID, CustomPosition As TestPosition, PreloadedTest, Available)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo with setDatos ReportsTestsSortingDS</returns>
        ''' <remarks>
        ''' AG 02/09/2014 - BA-1869
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
                        Dim cmdText As String = " SELECT 'ISE' AS TestType, ISETestID AS TestID, CustomPosition AS TestPosition, ShortName AS TestName, " & vbCrLf & _
                                                " 1 AS PreloadedTest, Available FROM tparISETests ORDER BY CustomPosition ASC "

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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "tparISETestsDAO.GetCustomizedSortedTestSelectionList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Update (only when informed) columns CustomPosition and Available for ISE tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestsSortingDS">Typed DataSet ReportsTestsSortingDS containing all tests to update</param>
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
                            cmdText.Append(" UPDATE tparISETests SET ")

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

                            cmdText.Append(" WHERE ISETestID  = " & testrow.TestID.ToString)
                            cmdText.Append(vbCrLf)
                        End If
                    Next

                    If cmdText.ToString.Length <> 0 Then
                        Using dbCmd As New SqlCommand(cmdText.ToString, pDBConnection)
                            resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        End Using
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.UpdateCustomPositionAndAvailable", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region


#Region "TO DELETE-REVIEW"

        ' WE 27/08/2014 -  #1865. Function ReadByName and ReadByShortName have been replaced by new function ReadName.

        ' ''' <summary>
        ' ''' Search ISE test data for the informed Test Name (for Import from LIMS process)
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pISETestName">ISE Test Name to search by</param>
        ' ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS containing data of the 
        ' '''          informed ISE Test</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 25/10/2010
        ' ''' Modified by: XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ' '''              DL 05/02/2013 - Add optional parameter DataBaseName and use it in query
        ' '''              SG 14/02/2013 - Add optional parameter IncludeDisabled and use it in query Bug #1134
        ' ''' </remarks>
        'Public Function ReadByName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestName As String, Optional pIncludeDisabled As Boolean = False, Optional pDataBaseName As String = "") As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim strFromLeft As String = ""
        '                If (Not String.IsNullOrEmpty(pDataBaseName)) Then strFromLeft = pDataBaseName & ".dbo."

        '                Dim cmdText As String = " SELECT * FROM " & strFromLeft & "tparISETests " & vbCrLf & _
        '                                        " WHERE  UPPER([Name]) = UPPER(N'" & pISETestName.Replace("'", "''") & "') " & vbCrLf

        '                'SG 14/02/2013 Bug #1134
        '                If Not pIncludeDisabled Then
        '                    cmdText &= " AND    Enabled = 1 " & vbCrLf
        '                End If

        '                '" WHERE  UPPER([Name]) = N'" & pISETestName.Replace("'", "''").ToUpper & "' " & vbCrLf & _

        '                Dim myISETests As New ISETestsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myISETests.tparISETests)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myISETests
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.ReadByName", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

        ' ''' <summary>
        ' ''' Search ISE test data for the informed Test ShortName
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pISETestShortName">ISE Test ShortName to search by</param>
        ' ''' <param name="pISETestIDToExclude">Identifier of the ISE Test to exclude (to avoid errors in case of
        ' '''                                   updation when the ShortName was not changed</param>
        ' ''' <returns>GlobalDataTO containing a typed DataSet ISETestsDS containing data of the 
        ' '''          informed ISE Test</returns>
        ' ''' <remarks>
        ' ''' Created by:  SA 10/11/2010
        ' ''' Modified by: XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ' ''' </remarks>

        'Public Function ReadByShortName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestShortName As String, _
        '                                Optional ByVal pISETestIDToExclude As Integer = -1) As GlobalDataTO
        '    Dim myGlobalDataTO As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
        '        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = " SELECT * FROM tparISETests " & vbCrLf & _
        '                                        " WHERE  UPPER(ShortName) = UPPER(N'" & pISETestShortName.Replace("'", "''") & "') " & vbCrLf
        '                '" WHERE  UPPER(ShortName) = N'" & pISETestShortName.Replace("'", "''").ToUpper & "' " & vbCrLf
        '                If (pISETestIDToExclude <> -1) Then cmdText &= " AND ISETestID <> " & pISETestIDToExclude & vbCrLf

        '                Dim myISETests As New ISETestsDS
        '                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
        '                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                        dbDataAdapter.Fill(myISETests.tparISETests)
        '                    End Using
        '                End Using

        '                myGlobalDataTO.SetDatos = myISETests
        '                myGlobalDataTO.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        myGlobalDataTO = New GlobalDataTO()
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparISETestsDAO.ReadByShortName", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return myGlobalDataTO
        'End Function

#End Region


    End Class

End Namespace
