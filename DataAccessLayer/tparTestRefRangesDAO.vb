Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class tparTestRefRangesDAO
          

#Region "CRUD Methods"
        ''' <summary>
        ''' Get Reference Ranges defined by the specified Test, and optionally by an specific SampleType and/or RangeType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pTestType">Standard, Calculated or ISE</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pRangeType">Type of Range: Generic or Detailed</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with the Reference Ranges defined for the Test</returns>
        ''' <remarks>
        ''' Created by:   SG 16/06/2010 
        ''' Modified by:  SG 01/09/2010 - Added parameter pTestType
        '''               JB 31/01/2013 - Add optional parameter DataBaseName and use it in query
        ''' </remarks>
        Public Function ReadByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "",
                                     Optional ByVal pRangeType As String = "", Optional ByVal pTestType As String = "STD",
                                     Optional pDataBaseName As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim strFromLeft As String = ""
                        If (Not String.IsNullOrEmpty(pDataBaseName)) Then strFromLeft = pDataBaseName & ".dbo."

                        Dim cmdText As String = ""
                        cmdText = " SELECT RangeID, TestID, TestType, SampleType, RangeType, Gender, AgeUnit, " & _
                                         " AgeRangeFrom, AgeRangeTo, NormalLowerLimit, NormalUpperLimit, BorderLineLowerLimit, " & _
                                         " BorderLineUpperLimit, TS_User, TS_DateTime " & _
                                   " FROM  " & strFromLeft & "tparTestRefRanges " & _
                                   " WHERE TestID = " & pTestID & _
                                   " AND   TestType = '" & pTestType & "'"

                        If (pSampleType <> "") Then cmdText &= " AND  SampleType = '" & pSampleType & "'"
                        If (pRangeType <> "") Then cmdText &= "  AND  RangeType  = '" & pRangeType & "'"

                        'Dim dbCmd As New SqlClient.SqlCommand
                        'dbCmd.Connection = dbConnection
                        'dbCmd.CommandText = cmdText

                        ''Fill the DataSet to return 
                        'Dim TestRefRanges As New TestRefRangesDS
                        'Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        'dbDataAdapter.Fill(TestRefRanges.tparTestRefRanges)
                        Dim TestRefRanges As New TestRefRangesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(TestRefRanges.tparTestRefRanges)
                            End Using
                        End Using


                        resultData.SetDatos = TestRefRanges
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestRefRangesDAO.ReadByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Create a new Test Reference Range
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestRefRangesRow">Typed DataSet TestRefRangesDS ROW with data of the Reference Range to add</param>
        ''' <param name="pTestType">Standard, Calculated or ISE</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with data of the added Reference Range</returns>
        ''' <remarks>
        ''' Created by:  SG 16/06/2010 
        ''' Modified by: SG 01/09/2010 - Added parameter pTestType 
        '''              AG 26/10/2010 - Change parameter DS for Row
        '''              SA 28/10/2010 - Added N preffix for multilanguage of field TS_User. Use function ReplaceNumericString in 
        '''                              Single fields to avoid errors when value has decimals. Mandatory fields have to be always
        '''                              informed
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow, _
                               Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim keys As String = " (TestID, TestType, SampleType, RangeType, Gender," & _
                                         "  AgeUnit, AgeRangeFrom, AgeRangeTo, NormalLowerLimit, NormalUpperLimit," & _
                                         "  BorderLineLowerLimit, BorderLineUpperLimit, TS_User, TS_DateTime) "
                    Dim values As String = ""
                    values &= pTestRefRangesRow.TestID & ", "
                    values &= "'" & pTestType & "', "
                    values &= "'" & pTestRefRangesRow.SampleType.Trim & "', "
                    values &= "'" & pTestRefRangesRow.RangeType.Trim & "', "

                    If (pTestRefRangesRow.IsGenderNull) Then
                        values &= "NULL, "
                    Else
                        values &= "'" & pTestRefRangesRow.Gender.Trim & "', "
                    End If

                    If (pTestRefRangesRow.IsAgeUnitNull) Then
                        values &= "NULL, "
                    Else
                        values &= "'" & pTestRefRangesRow.AgeUnit.Trim & "', "
                    End If

                    If (pTestRefRangesRow.IsAgeRangeFromNull) Then
                        values &= "NULL, "
                    Else
                        values &= pTestRefRangesRow.AgeRangeFrom & ", "
                    End If

                    If (pTestRefRangesRow.IsAgeRangeToNull) Then
                        values &= "NULL, "
                    Else
                        values &= pTestRefRangesRow.AgeRangeTo & ", "
                    End If

                    values &= ReplaceNumericString(pTestRefRangesRow.NormalLowerLimit) & ", "
                    values &= ReplaceNumericString(pTestRefRangesRow.NormalUpperLimit) & ", "

                    If (pTestRefRangesRow.IsBorderLineLowerLimitNull) Then
                        values &= "NULL, "
                    Else
                        values &= ReplaceNumericString(pTestRefRangesRow.BorderLineLowerLimit) & ", "
                    End If

                    If (pTestRefRangesRow.IsBorderLineUpperLimitNull) Then
                        values &= "NULL, "
                    Else
                        values &= ReplaceNumericString(pTestRefRangesRow.BorderLineUpperLimit) & ", "
                    End If

                    If (pTestRefRangesRow.IsTS_UserNull) Then
                        values &= "N'" & GlobalBase.GetSessionInfo.UserName.ToString.Replace("'", "''") & "', " 'AG 25/10/2010
                    Else
                        values &= "N'" & pTestRefRangesRow.TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    If (pTestRefRangesRow.IsTS_DateTimeNull) Then
                        values &= "'" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                    Else
                        values &= "'" & pTestRefRangesRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                    End If

                    Dim cmdText As String = ""
                    cmdText = "INSERT INTO tparTestRefRanges  " & keys & " VALUES (" & values & ")"

                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords += dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords > 0) Then
                        resultData.HasError = False
                        resultData.SetDatos = pTestRefRangesRow
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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestRefRangesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Modify a Test Reference Range
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestRefRangesRow">Typed DataSet TestRefRanges ROW with data of the Reference Range to update</param>
        ''' <param name="pTestType">Standard, Calculated or ISE</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with the updated Reference Range</returns>
        ''' <remarks>
        ''' Created by:  SG 16/06/2010 
        ''' Modified by: SG 01/09/2010 - Added parameter pTestType 
        '''              AG 26/10/2010 - Change parameter DS for Row
        '''              SA 28/10/2010 - Added N preffix for multilanguage of field TS_User. Use function ReplaceNumericString in 
        '''                              Single fields to avoid errors when value has decimals. Mandatory fields have to be always
        '''                              informed. Removed error raising based in number of affected records
        '''              SA 04/01/2011 - For OFF-SYSTEM Tests: update also the SampleType field
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow, _
                               Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim values As String = ""
                    values &= " TestType  = '" & pTestType & "', "
                    values &= " RangeType = '" & pTestRefRangesRow.RangeType & "', "

                    'For OFF-SYSTEM Tests it is possible to change the SampleType...
                    If (pTestType = "OFFS") Then
                        values &= " SampleType = '" & pTestRefRangesRow.SampleType & "', "
                    End If

                    If (pTestRefRangesRow.IsGenderNull) Then
                        values &= " Gender = NULL, "
                    Else
                        values &= " Gender = '" & pTestRefRangesRow.Gender & "', "
                    End If

                    If (pTestRefRangesRow.IsAgeUnitNull) Then
                        values &= " AgeUnit = NULL, "
                    Else
                        values &= " AgeUnit = '" & pTestRefRangesRow.AgeUnit & "', "
                    End If

                    If (pTestRefRangesRow.IsAgeRangeFromNull) Then
                        values &= " AgeRangeFrom = NULL, "
                    Else
                        values &= " AgeRangeFrom = " & pTestRefRangesRow.AgeRangeFrom & ", "
                    End If

                    If (pTestRefRangesRow.IsAgeRangeToNull) Then
                        values &= " AgeRangeTo = NULL, "
                    Else
                        values &= " AgeRangeTo = " & pTestRefRangesRow.AgeRangeTo & ", "
                    End If

                    values &= "NormalLowerLimit = " & ReplaceNumericString(pTestRefRangesRow.NormalLowerLimit) & ", "
                    values &= "NormalUpperLimit = " & ReplaceNumericString(pTestRefRangesRow.NormalUpperLimit) & ", "

                    If (pTestRefRangesRow.IsBorderLineLowerLimitNull) Then
                        values &= " BorderLineLowerLimit = NULL, "
                    Else
                        values &= " BorderLineLowerLimit = " & ReplaceNumericString(pTestRefRangesRow.BorderLineLowerLimit) & ", "
                    End If

                    If (pTestRefRangesRow.IsBorderLineUpperLimitNull) Then
                        values &= " BorderLineUpperLimit = NULL, "
                    Else
                        values &= " BorderLineUpperLimit = " & ReplaceNumericString(pTestRefRangesRow.BorderLineUpperLimit) & ", "
                    End If

                    If (pTestRefRangesRow.IsTS_UserNull) Then
                        'Dim myLocalGlobalBase As New GlobalBase
                        values &= " TS_User = N'" & GlobalBase.GetSessionInfo.UserName.ToString.Replace("'", "''") & "', " 'AG 25/10/2010
                    Else
                        values &= " TS_User = N'" & pTestRefRangesRow.TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    If (pTestRefRangesRow.IsTS_DateTimeNull) Then
                        values &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                    Else
                        values &= " TS_DateTime = '" & pTestRefRangesRow.TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                    End If

                    Dim cmdText As String = ""
                    cmdText = " UPDATE tparTestRefRanges SET " & values & _
                              " WHERE  RangeID = " & pTestRefRangesRow.RangeID.ToString()

                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                    If (resultData.AffectedRecords > 0) Then
                        resultData.SetDatos = pTestRefRangesRow
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestRefRangesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified Reference Ranges
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestRefRanges">Typed DataSet TestRefRangesDS with the Test Reference Ranges to delete</param>
        ''' <param name="pTestType">Standard, Calculated or ISE</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 16/06/2010 
        ''' Modified by: SG 01/09/2010 - Added parameter pTestType 
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestRefRanges As TestRefRangesDS, _
                               Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    For Each TestRefRangesRow As TestRefRangesDS.tparTestRefRangesRow In pTestRefRanges.tparTestRefRanges
                        Dim cmdText As String
                        cmdText = " DELETE FROM tparTestRefRanges " & vbCrLf & _
                                  " WHERE  RangeID = '" & TestRefRangesRow.RangeID & "'" & _
                                  " AND    TestType = '" & pTestType & "'"

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection
                        dbCmd.CommandText = cmdText

                        resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                    Next
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestRefRangesDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete Reference Ranges defined for a Test and optionally, a SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pTestType">Standard, Calculated or ISE</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 16/06/2010
        ''' Modified by: SG 01/09/2010 - Added parameter pTestType 
        '''              XB 18/02/2013 - Fix the use of parameter pTestType which is not used in this function nowadays (BugsTracking #1136)
        ''' </remarks>
        Public Function DeleteByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, Optional ByVal pSampleType As String = "", _
                                       Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText &= " DELETE FROM  tparTestRefRanges "
                    cmdText &= " WHERE TestID = " & pTestID

                    If (pSampleType <> "") Then cmdText &= " AND SampleType = '" & pSampleType & "'"

                    If (pTestType <> "") Then cmdText &= " AND TestType = '" & pTestType & "'"

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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestRefRangesDAO.DeleteByTestID", EventLogEntryType.Error, False)
            End Try

            Return myGlobalDataTO
        End Function
#End Region

#Region "TO DELETE"
        '''' <summary>
        '''' Get detailed Reference Range defined for the specified Gender for a Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <param name="pGender">Gender Code to filter the detailed Reference Ranges</param>
        '''' <param name="pTestType">Standard, Calculated or ISE</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with data of the Test Reference Range</returns>
        '''' <remarks>
        '''' Created by:  DL 29/06/2010 
        '''' Modified by: SG 01/09/2010 - Added parameter pTestType
        ''''              RH 17/09/2010 - pTestType added to the query
        '''' </remarks>
        'Public Function GetDetailedByGender(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                                    ByVal pGender As String, Optional ByVal pTestType As String = "STD") As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText &= " SELECT RangeID, TestID, TestType, SampleType, RangeType, Gender, AgeUnit, AgeRangeFrom, AgeRangeTo, NormalLowerLimit,"
        '                cmdText &= "        NormalUpperLimit, BorderLineLowerLimit, BorderLineUpperLimit, TS_User, TS_DateTime "
        '                cmdText &= " FROM   tparTestRefRanges "
        '                cmdText &= " WHERE  TestID = " & pTestID
        '                cmdText &= "   AND  SampleType = '" & pSampleType & "'"
        '                cmdText &= "   AND  RangeType = 'DETAILED'"
        '                cmdText &= "   AND  Gender = '" & pGender & "'"
        '                cmdText &= "   AND  TestType = '" & pTestType & "'"

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim TestRefRanges As New TestRefRangesDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(TestRefRanges.tparTestRefRanges)

        '                resultData.SetDatos = TestRefRanges
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestRefRangesDAO.GetDetailedByGender", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get detailed Reference Range defined for the specified Gender and Age for a Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <param name="pGender">Gender Code to filter the detailed Reference Ranges</param>
        '''' <param name="pAge">Age to filter the detailed Reference Ranges</param>
        '''' <param name="pTestType">Standard, Calculated or ISE</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with data of the Test Reference Range</returns>
        '''' <remarks>
        '''' Created by:  DL 29/06/2010 
        '''' Modified by: SG 01/09/2010 - Added parameter pTestType
        ''''              RH 17/09/2010 - pTestType added to the query
        '''' </remarks>
        'Public Function GetDetailedByGenderAge(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                                       ByVal pGender As String, ByVal pAge As Single, Optional ByVal pTestType As String = "STD") As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText &= " SELECT RangeID, TestID, TestType, SampleType, RangeType, Gender, AgeUnit, AgeRangeFrom, AgeRangeTo, NormalLowerLimit,"
        '                cmdText &= "        NormalUpperLimit, BorderLineLowerLimit, BorderLineUpperLimit, TS_User, TS_DateTime "
        '                cmdText &= " FROM   tparTestRefRanges "
        '                cmdText &= " WHERE  TestID = " & pTestID
        '                cmdText &= "   AND  SampleType = '" & pSampleType & "'"
        '                cmdText &= "   AND  RangeType = 'DETAILED'" & vbCrLf
        '                cmdText &= "   AND  Gender = '" & pGender & "'"
        '                cmdText &= "   AND  AgeRangeFrom <= " & pAge & " and AgeRangeTo >= " & pAge
        '                cmdText &= "   AND  TestType = '" & pTestType & "'"

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim TestRefRanges As New TestRefRangesDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(TestRefRanges.tparTestRefRanges)

        '                resultData.SetDatos = TestRefRanges
        '                resultData.HasError = False
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestRefRangesDAO.GetDetailedByGenderAge", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get detailed Reference Range defined for the specified Age for a Test/SampleType
        '''' </summary>
        '''' <param name="pDBConnection">Open DB Connection</param>
        '''' <param name="pTestID">Test Identifier</param>
        '''' <param name="pSampleType">Sample Type Code</param>
        '''' <param name="pAge">Age to filter the detailed Reference Ranges</param>
        '''' <param name="pTestType">Standard, Calculated or ISE</param>
        '''' <returns>GlobalDataTO containing a typed DataSet TestRefRangesDS with data of the Test Reference Range</returns>
        '''' <remarks>
        '''' Created by:  DL 29/06/2010 
        '''' Modified by: SG 01/09/2010 - Added parameter pTestType
        ''''              RH 17/09/2010 - pTestType added to the query
        '''' </remarks>
        'Public Function GetDetailedByAge(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pSampleType As String, _
        '                                    ByVal pAge As Single, Optional ByVal pTestType As String = "STD") As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim cmdText As String = ""
        '                cmdText &= " SELECT RangeID, TestID, TestType, SampleType, RangeType, Gender, AgeUnit, AgeRangeFrom, AgeRangeTo, NormalLowerLimit,"
        '                cmdText &= "        NormalUpperLimit, BorderLineLowerLimit, BorderLineUpperLimit, TS_User, TS_DateTime "
        '                cmdText &= " FROM   tparTestRefRanges "
        '                cmdText &= " WHERE  TestID = " & pTestID
        '                cmdText &= "   AND  SampleType = '" & pSampleType & "'"
        '                cmdText &= "   AND  RangeType = 'DETAILED'"
        '                cmdText &= "   AND  AgeRangeFrom <= " & pAge & " AND AgeRangeTo >= " & pAge
        '                cmdText &= "   AND  TestType = '" & pTestType & "'"

        '                Dim dbCmd As New SqlClient.SqlCommand
        '                dbCmd.Connection = dbConnection
        '                dbCmd.CommandText = cmdText

        '                'Fill the DataSet to return 
        '                Dim TestRefRanges As New TestRefRangesDS
        '                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
        '                dbDataAdapter.Fill(TestRefRanges.tparTestRefRanges)

        '                resultData.SetDatos = TestRefRanges
        '                resultData.HasError = False
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "tparTestRefRangesDAO.GetDetailedByAge", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class

End Namespace
