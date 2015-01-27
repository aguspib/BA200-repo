Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tparTestProfilesDAO
          


#Region "CRUD Methods"

        ''' <summary>
        ''' Create a new Test Profile (only basic data, without the list of Tests)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfile">Dataset with structure of table tparTestProfiles</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 28/10/2010 - Add N preffix for multilanguage of field TS_User
        ''' AG 01/09/2014 - BA-1869 new columns CustomPosition, Available are informed!!
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfile As TestProfilesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf Not pTestProfile Is Nothing Then
                    'Get the next TextProfilePosition
                    resultData = GetNextPosition(pDBConnection)
                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                        Dim testProfilePosition As Integer = DirectCast(resultData.SetDatos, Integer)

                        Dim cmdText As String
                        cmdText = " INSERT INTO tparTestProfiles (TestProfileName, SampleType, TestProfilePosition, InUse, " & _
                                                                " TS_User, TS_DateTime, CustomPosition, Available ) " & _
                                  " VALUES(N'" & pTestProfile.tparTestProfiles(0).TestProfileName.Replace("'", "''") & "', " & _
                                         "  '" & pTestProfile.tparTestProfiles(0).SampleType.ToString & "', " & _
                                                testProfilePosition & ", 0, "

                        If (pTestProfile.tparTestProfiles(0).IsTS_UserNull) Then
                            'Dim objGlobal As New GlobalBase
                            cmdText &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                        Else
                            cmdText &= " N'" & pTestProfile.tparTestProfiles(0).TS_User.Trim.Replace("'", "''") & "', "
                        End If

                        If (pTestProfile.tparTestProfiles(0).IsTS_DateTimeNull) Then
                            cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                        Else
                            cmdText &= " '" & pTestProfile.tparTestProfiles(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                        End If

                        'AG 01/09/2014 - BA-1869
                        cmdText &= " , " & pTestProfile.tparTestProfiles(0).CustomPosition.ToString & " "
                        If pTestProfile.tparTestProfiles(0).IsAvailableNull OrElse pTestProfile.tparTestProfiles(0).Available Then
                            cmdText &= " , 1 )"
                        Else
                            cmdText &= " , 0 )"
                        End If
                        'AG 01/09/2014 - BA-1869

                        cmdText &= " SELECT SCOPE_IDENTITY() "

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        Dim nextTestProfileID As Integer
                        nextTestProfileID = CType(dbCmd.ExecuteScalar(), Integer)

                        If (nextTestProfileID > 0) Then
                            'Inform the automatically generated ID in the DataSet to return
                            pTestProfile.tparTestProfiles(0).BeginEdit()
                            pTestProfile.tparTestProfiles(0).TestProfileID = nextTestProfileID
                            pTestProfile.tparTestProfiles(0).EndEdit()

                            resultData.AffectedRecords = 1
                            resultData.SetDatos = pTestProfile
                        Else
                            resultData.HasError = True
                            resultData.AffectedRecords = 0
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined Test Profiles
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOrderByColumnName">Optional parameter to indicate the column to sort the reurned data</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfilesDS with the list of Test Profiles</returns>
        ''' <remarks></remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOrderByColumnName As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= " SELECT TestProfileID, TestProfileName, SampleType, TestProfilePosition, InUse, TS_User, TS_DateTime " & vbCrLf
                        cmdText &= " FROM   tparTestProfiles " & vbCrLf

                        If (pOrderByColumnName.Trim() <> "") Then
                            cmdText &= " ORDER BY " & pOrderByColumnName
                        End If

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myDS As New TestProfilesDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)

                        dbDataAdapter.Fill(myDS.tparTestProfiles)
                        resultData.SetDatos = myDS
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update basic data of an specific Test Profile 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfile">Typed DataSet TestProfilesDS containing data of the Test Profile to update</param>
        ''' <returns>GlobalDataTO containing the updated record and/or error information</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 28/10/2010 - Add N preffix for multilanguage of field TS_User
        ''' AG 02/09/2014 - BA-1869 update Available only when informed
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfile As TestProfilesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString

                ElseIf Not pTestProfile Is Nothing Then
                    Dim cmdText As String = ""
                    cmdText = " UPDATE tparTestProfiles " & _
                                 " SET TestProfileName     = N'" & pTestProfile.tparTestProfiles(0).TestProfileName.Replace("'", "''") & "', " & _
                                     " SampleType          = '" & pTestProfile.tparTestProfiles(0).SampleType.ToString & "', " & _
                                     " TestProfilePosition = " & pTestProfile.tparTestProfiles(0).TestProfilePosition & ", "

                    If (pTestProfile.tparTestProfiles(0).IsTS_UserNull) Then
                        'Dim objGlobal As New GlobalBase
                        cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                    Else
                        cmdText &= " TS_User = N'" & pTestProfile.tparTestProfiles(0).TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    If (pTestProfile.tparTestProfiles(0).IsTS_DateTimeNull) Then
                        cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                    Else
                        cmdText &= " TS_DateTime = '" & pTestProfile.tparTestProfiles(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                    End If

                    'AG 02/09/2014 - BA-1869
                    If Not pTestProfile.tparTestProfiles(0).IsAvailableNull Then
                        cmdText &= " , Available = " & CInt(IIf(pTestProfile.tparTestProfiles(0).Available, 1, 0))
                    End If
                    'AG 02/09/2014 - BA-1869

                    cmdText &= " WHERE TestProfileID = " & pTestProfile.tparTestProfiles(0).TestProfileID

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = (resultData.AffectedRecords <> 1)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update TestProfilePosition for the specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileId">Test Profile Identifier</param>
        ''' <param name="pNewPosition">Position value</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 07/07/2010
        ''' Modified by: SA 08/07/2010 - Update also the audit fields
        '''              SA 28/10/2010 - Add N preffix for multilanguage of field TS_User
        ''' </remarks>
        Public Function UpdateTestProfilePosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfileId As Integer, _
                                                  ByVal pNewPosition As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    'Dim objGlobal As New GlobalBase
                    cmdText = " UPDATE tparTestProfiles " & vbCrLf & _
                              "    SET TestProfilePosition = " & pNewPosition & ", " & vbCrLf & _
                                     " TS_User             = N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', " & vbCrLf & _
                                     " TS_DateTime         = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf & _
                              "  WHERE TestProfileId =  " & pTestProfileId

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    resultData.HasError = (resultData.AffectedRecords <> 1)
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.UpdateTestProfilePosition", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified Test Profile (the list of Tests included in the Test Profile should have 
        ''' been deleted previously)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileID">Identifier of the Test Profile</param>        
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Modified by:  SA 08/06/2010 - Changed entry parameter: receive the ID instead of a DataSet
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfileID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    cmdText = " DELETE FROM tparTestProfiles " & _
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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all empty Test Profiles (when all the Tests in the Profile has been deleted)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: SA 08/07/2010 - Changed the query: the LEFT JOIN is not needed in the subquery
        ''' </remarks>
        Public Function DeleteEmptyProfiles(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    'There is not an Opened Database Connection...
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    cmdText = " DELETE FROM tparTestProfiles " & _
                              " WHERE TestProfileID NOT IN (SELECT TestProfileID FROM tparTestProfileTests) "

                    'cmdText = " DELETE FROM tparTestProfiles" & vbCrLf & _
                    '          " WHERE  TestProfileID IN (SELECT tp.TestProfileID " & vbCrLf & _
                    '          "                            FROM tparTestProfiles tp" & vbCrLf & _
                    '          "                              LEFT JOIN tparTestProfileTests tpt" & vbCrLf & _
                    '          "                                ON tp.TestProfileID = tpt.TestProfileID" & vbCrLf & _
                    '          "                            WHERE tpt.TestID IS NULL)"
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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.DeleteEmptyProfiles", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileID">Test Profile Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfilesDS with the data of 
        '''          the specified Test Profile</returns>
        ''' <remarks>
        ''' Created by:  TR 18/05/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfileID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        cmdText &= " SELECT * "
                        cmdText &= " FROM tparTestProfiles "
                        cmdText &= " WHERE TestProfileID = " & pTestProfileID.ToString()

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myDS As New TestProfilesDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)

                        dbDataAdapter.Fill(myDS.tparTestProfiles)
                        resultData.SetDatos = myDS
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"

        ''' <summary>
        ''' Search the last position number in table tparTestProfiles and add one to it to generate
        ''' the next position number to use when Create a new Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an integer value with the calculated Next Position number</returns>
        ''' <remarks></remarks>
        Public Function GetNextPosition(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT MAX(TestProfilePosition) As NextPosition" & vbCrLf & _
                                  " FROM   tparTestProfiles"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        Dim dbDataReader As SqlClient.SqlDataReader
                        dbDataReader = dbCmd.ExecuteReader()

                        If (dbDataReader.HasRows) Then
                            dbDataReader.Read()
                            If (dbDataReader.IsDBNull(0)) Then
                                resultData.SetDatos = 1
                            Else
                                resultData.SetDatos = CInt(dbDataReader.Item("NextPosition")) + 1
                            End If
                        End If
                        dbDataReader.Close()
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.GetNextPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is a Test Profile with the informed Profile Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileName">Test Profile Name to be validated</param>
        ''' <param name="pTestProfileID">Test Profile Identifier. It is an optional parameter informed
        '''                              only in case of updation</param>
        ''' <returns>True if there is another Test Profile with the same name.
        '''          Otherwise, it returns False</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 28/10/2010 - Add N preffix for multilanguage of field TestProfileName when it is used as filter
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' </remarks>
        Public Function ExistsTestProfile(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfileName As String, _
                                          Optional ByVal pTestProfileID As Integer = 0) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT TestProfileID" & vbCrLf & _
                                  " FROM   tparTestProfiles" & vbCrLf & _
                                  " WHERE  UPPER(TestProfileName) = UPPER(N'" & pTestProfileName.Trim.Replace("'", "''") & "')" & vbCrLf
                        '" WHERE  UPPER(TestProfileName) = N'" & pTestProfileName.Trim.Replace("'", "''").ToUpper & "'" & vbCrLf

                        If (pTestProfileID <> 0) Then
                            cmdText &= " AND TestProfileID <> " & pTestProfileID
                        End If

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim myDS As New TestProfilesDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)

                        dbDataAdapter.Fill(myDS.tparTestProfiles)
                        resultData.SetDatos = (myDS.tparTestProfiles.Rows.Count > 0)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.ExistsTestProfile", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all Test Profiles defined for the specified SampleType plus the list of Tests included in each one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCustomizedTestSelection">FALSE same order as until 3.0.2 / When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfilesTestsDS with the list of all Test Profiles defined
        '''          for the specified SampleType plus the list of Tests included in each one</returns>
        ''' <remarks>
        ''' Created by:  TR 05/02/2010
        ''' Modified by: SA 19/10/2010 - Changed the SQL to get also Calculated and ISE Tests included in the returned Profiles
        '''              SA 02/12/2010 - Changed the SQL to get also OffSystem Tests included in the returned Profiles
        '''              XB 01/02/2013 - Upper conversions must be implemented in same environment (f.ex.SQL)  (Bugs tracking #1112)
        ''' AG 01/09/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function GetProfilesBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleType As String, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String
                        cmdText = " SELECT   TP.TestProfileID, TP.TestProfileName, TPT.TestType, T.TestID, T.TestName, " & _
                                           " T.TestPosition, TP.TestProfilePosition, 1 AS TestTypePos, TP.CustomPosition " & _
                                  " FROM     tparTestProfiles TP INNER JOIN tparTestProfileTests TPT ON TP.TestProfileID = TPT.TestProfileID " & _
                                                               " INNER JOIN tparTests T ON TPT.TestID = T.TestID " & _
                                  " WHERE    UPPER(TP.SampleType) = UPPER(N'" & pSampleType & "') " & _
                                  " AND      TPT.TestType = 'STD' "

                        'AG 01/09/2014 - BA-1869
                        If pCustomizedTestSelection Then
                            cmdText &= " AND TP.Available = 1 "
                        End If
                        'AG 01/09/2014 - BA-1869

                        cmdText &= " UNION " & _
                                  " SELECT   TP.TestProfileID, TP.TestProfileName, TPT.TestType, CT.CalcTestID AS TestID, CT.CalcTestLongName AS TestName, " & _
                                           " CT.CalcTestID AS TestPosition, TP.TestProfilePosition, 2 AS TestTypePos, TP.CustomPosition " & _
                                  " FROM     tparTestProfiles TP INNER JOIN tparTestProfileTests TPT ON TP.TestProfileID = TPT.TestProfileID " & _
                                                               " INNER JOIN tparCalculatedTests CT ON TPT.TestID = CT.CalcTestID " & _
                                  " WHERE    UPPER(TP.SampleType) = UPPER(N'" & pSampleType & "') " & _
                                  " AND      TPT.TestType = 'CALC' "

                        'AG 01/09/2014 - BA-1869
                        If pCustomizedTestSelection Then
                            cmdText &= " AND TP.Available = 1 "
                        End If
                        'AG 01/09/2014 - BA-1869

                        cmdText &= " UNION " & _
                                  " SELECT   TP.TestProfileID, TP.TestProfileName, TPT.TestType, IT.IseTestID AS TestID, IT.[Name] AS TestName, " & _
                                           " IT.IseTestID AS TestPosition,  TP.TestProfilePosition, 3 AS TestTypePos, TP.CustomPosition " & _
                                  " FROM     tparTestProfiles TP INNER JOIN tparTestProfileTests TPT ON TP.TestProfileID = TPT.TestProfileID " & _
                                                               " INNER JOIN tparISETests IT ON TPT.TestID = IT.IseTestID " & _
                                  " WHERE    UPPER(TP.SampleType) = UPPER(N'" & pSampleType & "') " & _
                                  " AND      TPT.TestType = 'ISE' "

                        'AG 01/09/2014 - BA-1869
                        If pCustomizedTestSelection Then
                            cmdText &= " AND TP.Available = 1 "
                        End If
                        'AG 01/09/2014 - BA-1869

                        cmdText &= " UNION " & _
                                  " SELECT   TP.TestProfileID, TP.TestProfileName, TPT.TestType, OT.OffSystemTestID AS TestID, OT.[Name] AS TestName, " & _
                                           " OT.OffSystemTestID AS TestPosition,  TP.TestProfilePosition, 4 AS TestTypePos, TP.CustomPosition " & _
                                  " FROM     tparTestProfiles TP INNER JOIN tparTestProfileTests TPT ON TP.TestProfileID = TPT.TestProfileID " & _
                                                               " INNER JOIN tparOffSystemTests OT ON TPT.TestID = OT.OffSystemTestID " & _
                                  " WHERE    UPPER(TP.SampleType) = UPPER(N'" & pSampleType & "') " & _
                                  " AND      TPT.TestType = 'OFFS' "

                        'AG 01/09/2014 - BA-1869
                        '" ORDER BY TP.TestProfilePosition, TestTypePos, TestPosition "
                        If Not pCustomizedTestSelection Then 'Use the old order by clause
                            cmdText &= " ORDER BY TP.TestProfilePosition, TestTypePos, TestPosition "
                        Else 'New order by clause by customized order
                            cmdText &= " AND TP.Available = 1 "
                            cmdText &= " ORDER BY TP.CustomPosition ASC "
                        End If
                        'AG 01/09/2014 - BA-1869

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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.GetProfilesBySampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Test Profiles added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for Test Profiles that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010 
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for Test Profiles 
        '''                               that have been excluded from the active WorkSession  
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
                        cmdText = " UPDATE tparTestProfiles " & _
                                  " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & _
                                  " WHERE  TestProfileID IN (SELECT DISTINCT WSOT.TestProfileID " & _
                                                           " FROM   vwksWSOrderTests WSOT " & _
                                                           " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & _
                                                           " AND    WSOT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                           " AND    WSOT.SampleClass = 'PATIENT' " & _
                                                           " AND    WSOT.TestProfileID IS NOT NULL) "
                    Else
                        cmdText = " UPDATE tparTestProfiles " & _
                                  " SET    InUse = 0 " & _
                                  " WHERE  TestProfileID NOT IN (SELECT DISTINCT WSOT.TestProfileID " & _
                                                               " FROM   vwksWSOrderTests WSOT " & _
                                                               " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & _
                                                               " AND    WSOT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                               " AND    WSOT.SampleClass = 'PATIENT' " & _
                                                               " AND    WSOT.TestProfileID IS NOT NULL) " & _
                                  " AND    InUse = 1 "
                    End If

                    'cmdText = "UPDATE tparTestProfiles" & vbCrLf & _
                    '          "  SET InUse = '" & IIf(pFlag, "True", "False").ToString & "'" & vbCrLf & _
                    '          "  WHERE TestProfileID IN (SELECT DISTINCT TestProfileID" & vbCrLf & _
                    '          "                            FROM vwksWSOrderTests" & vbCrLf & _
                    '          "                            WHERE WorkSessionID = '" & pWorkSessionID.Trim.Replace("'", "''") & "'" & vbCrLf & _
                    '          "                              AND AnalyzerID    = '" & pAnalyzerID.Trim.Replace("'", "''") & "'" & vbCrLf & _
                    '          "                              AND TestProfileID IS NOT NULL)"

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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.UpdateInUseFlag", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
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
            Dim myGlobalDataTO As New GlobalDataTO()
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                myGlobalDataTO = GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(CustomPosition) FROM tparTestProfiles "

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            myGlobalDataTO.SetDatos = dbCmd.ExecuteScalar()
                        End Using

                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.GetLastCustomPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Gets all profile tests order by CustomPosition (return columns: TestType, TestID, CustomPosition As TestPosition, PreloadedTest, Available)
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
                        Dim cmdText As String = " SELECT 'PROFILE' AS TestType, TestProfileID AS TestID, CustomPosition AS TestPosition, TestProfileName AS TestName, " & vbCrLf & _
                                                " 0 AS PreloadedTest, Available FROM tparTestProfiles ORDER BY CustomPosition ASC "

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
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "tparTestProfilesDAO.GetCustomizedSortedTestSelectionList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Update (only when informed) columns CustomPosition and Available for test profiles
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
                            cmdText.Append(" UPDATE tparTestProfiles SET ")

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

                            cmdText.Append(" WHERE TestProfileID  = " & testrow.TestID.ToString)
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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.UpdateCustomPositionAndAvailable", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Update profile Available value depending his components: All Available -- profile available // Some NOT available -- profile not available
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 17/09/2014 - BA-1869
        ''' </remarks>
        Public Function UpdateAvailableCascadeByComponents(ByVal pDBConnection As SqlClient.SqlConnection, pAvailableValue As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    If Not pAvailableValue Then
                        'Update to Available = 0 when some component not available
                        cmdText.Append(" UPDATE tparTestProfiles  SET Available = 0 WHERE TestProfileID IN ")
                    Else
                        'Update to Available = 1 when all components available
                        cmdText.Append(" UPDATE tparTestProfiles  SET Available = 1 WHERE TestProfileID NOT IN ")
                    End If

                    cmdText.Append(" (SELECT DISTINCT PT.testprofileID FROM tparTestProfileTests  PT ")
                    cmdText.Append(" LEFT OUTER JOIN tparTests T ON PT.TestType = 'STD' AND PT.TestID = T.TestID ")
                    cmdText.Append(" LEFT OUTER JOIN tparCalculatedTests  CT ON PT.TestType = 'CALC' AND PT.TestID = CT.CalcTestID  ")
                    cmdText.Append(" LEFT OUTER JOIN tparISETests IT ON PT.TestType = 'ISE' AND PT.TestID = IT.ISETestID  ")
                    cmdText.Append(" LEFT OUTER JOIN tparOffSystemTests OFT ON PT.TestType = 'OFFS' AND PT.TestID = OFT.OffSystemTestID  ")
                    cmdText.Append(" WHERE (CASE PT.TestType WHEN 'STD' THEN T.Available WHEN 'CALC' THEN CT.Available WHEN 'ISE' THEN IT.Available WHEN 'OFFS' THEN OFT.Available END) = 0) ")

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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestProfilesDAO.UpdateAvailableCascadeByComponents", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


#End Region

    End Class

End Namespace
