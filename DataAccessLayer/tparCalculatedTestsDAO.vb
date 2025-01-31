﻿Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO
    Public Class tparCalculatedTestsDAO


#Region "CRUD Methods"
        ''' <summary>
        ''' Create a new Calculated Test (only basic data, without the list of components of its Formula)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTest">Typed DataSet CalculatedTestsDS with data of the Calculated Test to add</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with all data of the added Calculated Test
        '''          or error information</returns>
        ''' <remarks>
        ''' Modified by: DL 27/05/2010 - Added field InUse 
        '''              SG 03/09/2010 - Added field ActiveRangeType
        '''              SA 26/10/2010 - Added N preffix for multilanguage of fields FormulaText and TS_User; removed fields
        '''                              EnableStatus and InUse from the SQL (in creation they get the field default value defined
        '''                              in the table, True for EnableStatus and False for InUse)
        '''              AG 02/11/2010 - Changed verification of field ActiveRangeType: it fails when value is NULL
        '''              SA 05/10/2011 - In field FormulaText, replace commas by dots, to store operands with decimals in the string in 
        '''                              the same way they are saved in the Formula table  
        '''              XB 14/02/2013 - Add PreloadedCalculatedTest field on INSERT operation (Bugs tracking #1134)
        '''              SG 20/02/2013 - Add BiosystemsID field on INSERT operation (Bugs tracking #1134)
        '''              TR 10/05/2013 - Add LISValue field used on the update process.
        '''              AG 01/09/2014 - BA-1869 new column CustomPosition, Available are informed!!
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTest As CalculatedTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " INSERT INTO tparCalculatedTests(CalcTestName, CalcTestLongName, MeasureUnit, UniqueSampleType, " & _
                                                              " SampleType, Decimals, PrintExpTests, FormulaText, " & _
                                                              " ActiveRangeType, TS_User, TS_DateTime, PreloadedCalculatedTest, BiosystemsID, LISValue, CustomPosition, Available ) " & _
                              " VALUES (N'" & pCalcTest.tparCalculatedTests(0).CalcTestName.ToString.Replace("'", "''") & "', " & _
                                      " N'" & pCalcTest.tparCalculatedTests(0).CalcTestLongName.ToString.Replace("'", "''") & "', " & _
                                      " '" & pCalcTest.tparCalculatedTests(0).MeasureUnit.ToString & "', " & _
                                             Convert.ToInt32(pCalcTest.tparCalculatedTests(0).UniqueSampleType) & ", "

                    If (String.IsNullOrEmpty(pCalcTest.tparCalculatedTests(0).SampleType.ToString)) Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " '" & pCalcTest.tparCalculatedTests(0).SampleType.ToString.Replace("'", "''") & "', "
                    End If

                    If (String.IsNullOrEmpty(pCalcTest.tparCalculatedTests(0).Decimals.ToString)) Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= DAOBase.ReplaceNumericString(pCalcTest.tparCalculatedTests(0).Decimals) & ", "
                    End If

                    cmdText &= Convert.ToInt32(pCalcTest.tparCalculatedTests(0).PrintExpTests) & ", " & _
                               " N'" & pCalcTest.tparCalculatedTests(0).FormulaText.Replace("'", "''").Replace(",", ".") & "', "

                    If (pCalcTest.tparCalculatedTests(0).IsActiveRangeTypeNull) Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " '" & pCalcTest.tparCalculatedTests(0).ActiveRangeType & "', "
                    End If

                    If (String.IsNullOrEmpty(pCalcTest.tparCalculatedTests(0).TS_User.ToString)) Then
                        'Get the connected Username from the current Application Session
                        'Dim currentSession As New GlobalBase
                        cmdText &= " N'" & GlobalBase.GetSessionInfo().UserName.Replace("'", "''") & "', "
                    Else
                        cmdText &= " N'" & pCalcTest.tparCalculatedTests(0).TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    If (String.IsNullOrEmpty(pCalcTest.tparCalculatedTests(0).TS_DateTime.ToString)) Then
                        'Get the current DateTime
                        cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', "
                    Else
                        cmdText &= " '" & pCalcTest.tparCalculatedTests(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                    End If

                    cmdText &= Convert.ToInt32(pCalcTest.tparCalculatedTests(0).PreloadedCalculatedTest) & ", "

                    'SGM 20/02/2013
                    If (pCalcTest.tparCalculatedTests(0).IsBiosystemsIDNull) Then
                        cmdText &= " NULL,"
                    Else
                        cmdText &= " " & pCalcTest.tparCalculatedTests(0).BiosystemsID & ", "
                    End If
                    'end SGM 20/02/2013

                    'AG 01/09/2014 - BA-1869 - inform last column CustomPosition
                    'TR 10/05/2013 Add the LISValue Column.
                    If (pCalcTest.tparCalculatedTests(0).IsLISValueNull) Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " N'" & pCalcTest.tparCalculatedTests(0).LISValue & "', "
                    End If

                    cmdText &= pCalcTest.tparCalculatedTests(0).CustomPosition & " "
                    If pCalcTest.tparCalculatedTests(0).IsAvailableNull OrElse pCalcTest.tparCalculatedTests(0).Available Then
                        cmdText &= ", 1 )"
                    Else
                        cmdText &= ", 0 )"
                    End If
                    'AG 01/09/2014 - BA-1869

                    'Finally, get the automatically generated ID for the created Calculated Test
                    cmdText &= " SELECT SCOPE_IDENTITY() "

                    Dim newCalcTestID As Integer
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        newCalcTestID = CType(dbCmd.ExecuteScalar(), Integer)

                        If (newCalcTestID > 0) Then
                            pCalcTest.tparCalculatedTests(0).BeginEdit()
                            pCalcTest.tparCalculatedTests(0).SetField("CalcTestID", newCalcTestID)
                            pCalcTest.tparCalculatedTests(0).EndEdit()

                            resultData.HasError = False
                            resultData.AffectedRecords = 1
                            resultData.SetDatos = pCalcTest
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

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified Calculated Test (the Value of Formula included in the Calculated Test and all defined
        ''' Reference Ranges have to be deleted previously)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test to delete</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Modified by: SA 22/06/2010 - Changed the entry parameter to receive only the Calculated Test ID instead
        '''                              of a typed DataSet
        '''              SA 26/10/2010 - Removed set of HasError=True depending of value of AffectedRecords
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM tparCalculatedTests " & vbCrLf & _
                                            " WHERE  CalcTestID = " & pCalcTestID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified Calculated Test
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <param name="pID">Unique Calculated Test Identifier (CalcTestID or BiosystemsID)</param>
        ''' <param name="pSearchByBiosystemsID">When TRUE, the search is executed by field BiosystemsID instead of by field CalcTestID.
        '''                                     Optional parameter with FALSE as default value</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with data of the specified Calculated Test</returns>
        ''' <remarks>
        ''' Created by:  SA 07/05/2010
        ''' Modified by: SA 16/10/2014 - BA-1944 (SubTask BA-2017) ==> Added optional parameter pSearchByBiosystemsID to allow search the
        '''                                                            Calculated Test by BiosystemsID instead of by CalcTestID 
        '''                                                            (needed in UpdateVersion process)
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pID As Integer, _
                             Optional ByVal pSearchByBiosystemsID As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparCalculatedTests " & vbCrLf

                        If (Not pSearchByBiosystemsID) Then
                            cmdText &= " WHERE CalcTestID = " & pID.ToString
                        Else
                            cmdText &= " WHERE BiosystemsID = " & pID.ToString
                        End If

                        Dim myCalculatedTests As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalculatedTests.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = myCalculatedTests
                        resultData.HasError = False
                    End If
                    End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined Calculated Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with the list of all defined Calculated Tests</returns>
        ''' <remarks></remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM tparCalculatedTests " & vbCrLf & _
                                                " ORDER BY CalcTestLongName "

                        Dim myCalculatedTests As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalculatedTests.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = myCalculatedTests
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search test data for the informed Test Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestName">Calculated Test Name to search by</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS containing data of the 
        '''          informed Calculated Test</returns>
        ''' <remarks>
        ''' Created by:  SA 13/09/2010
        ''' Modified by: SA 26/10/2010 - Add the N preffix for multilanguage when comparing by CalcTestName
        '''              JB 31/01/2013 - Add optional parameter DataBaseName and use it in query
        '''              SG 19/02/2013 - Add new field BiosystemsID 
        ''' </remarks>
        Public Function ReadByCalcTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestName As String, Optional pDataBaseName As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim strFromLeft As String = ""
                        If (Not String.IsNullOrEmpty(pDataBaseName)) Then strFromLeft = pDataBaseName & ".dbo."

                        Dim cmdText As String = ""
                        cmdText &= "SELECT CalcTestID" & vbCrLf
                        cmdText &= "      ,CalcTestName" & vbCrLf
                        cmdText &= "      ,CalcTestLongName" & vbCrLf
                        cmdText &= "      ,MeasureUnit" & vbCrLf
                        cmdText &= "      ,UniqueSampleType" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,Decimals" & vbCrLf
                        cmdText &= "      ,PrintExpTests" & vbCrLf
                        cmdText &= "      ,FormulaText" & vbCrLf
                        cmdText &= "      ,ActiveRangeType" & vbCrLf
                        cmdText &= "      ,InUse" & vbCrLf
                        cmdText &= "      ,EnableStatus" & vbCrLf
                        cmdText &= "      ,TS_User" & vbCrLf
                        cmdText &= "      ,TS_DateTime" & vbCrLf
                        cmdText &= "      ,PreloadedCalculatedTest" & vbCrLf
                        cmdText &= "      ,BiosystemsID" & vbCrLf 'SGM 19/02/2013
                        cmdText &= "  FROM " & strFromLeft & "tparCalculatedTests " & vbCrLf
                        cmdText &= " WHERE UPPER(CalcTestName) =  UPPER(N'" & pCalcTestName.Replace("'", "''") & "') "
                        'cmdText &= " WHERE UPPER(CalcTestName) =  N'" & pCalcTestName.Replace("'", "''").ToUpper & "' "

                        'Dim cmdText As String = " SELECT * FROM " & strFromLeft & "tparCalculatedTests " & vbCrLf & _
                        '                        " WHERE  UPPER(CalcTestName) = N'" & pCalcTestName.Replace("'", "''").ToUpper & "' " & vbCrLf

                        Dim myCalculatedTests As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalculatedTests.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = myCalculatedTests
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.ReadByCalcTestName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search test data for the informed Test Long Name
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pCalcTestLongName"></param>
        ''' <param name="pDataBaseName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Created by:  SG 14/02/2013
        ''' Modify by:   SG 19/02/2013 - Add new field BiosystemsID
        ''' </remarks>
        Public Function ReadByCalcTestLongName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestLongName As String, Optional pDataBaseName As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim strFromLeft As String = ""
                        If (Not String.IsNullOrEmpty(pDataBaseName)) Then strFromLeft = pDataBaseName & ".dbo."

                        Dim cmdText As String = ""
                        cmdText &= "SELECT CalcTestID" & vbCrLf
                        cmdText &= "      ,CalcTestName" & vbCrLf
                        cmdText &= "      ,CalcTestLongName" & vbCrLf
                        cmdText &= "      ,MeasureUnit" & vbCrLf
                        cmdText &= "      ,UniqueSampleType" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,Decimals" & vbCrLf
                        cmdText &= "      ,PrintExpTests" & vbCrLf
                        cmdText &= "      ,FormulaText" & vbCrLf
                        cmdText &= "      ,ActiveRangeType" & vbCrLf
                        cmdText &= "      ,InUse" & vbCrLf
                        cmdText &= "      ,EnableStatus" & vbCrLf
                        cmdText &= "      ,TS_User" & vbCrLf
                        cmdText &= "      ,TS_DateTime" & vbCrLf
                        cmdText &= "      ,PreloadedCalculatedTest" & vbCrLf
                        cmdText &= "      ,BiosystemsID" & vbCrLf 'SG 19/02/2013
                        cmdText &= "  FROM " & strFromLeft & "tparCalculatedTests " & vbCrLf
                        cmdText &= " WHERE UPPER(CalcTestLongName) =  UPPER(N'" & pCalcTestLongName.Replace("'", "''") & "') "
                        'cmdText &= " WHERE UPPER(CalcTestLongName) =  N'" & pCalcTestLongName.Replace("'", "''").ToUpper & "' "

                        'Dim cmdText As String = " SELECT * FROM " & strFromLeft & "tparCalculatedTests " & vbCrLf & _
                        '                        " WHERE  UPPER(CalcTestName) = N'" & pCalcTestName.Replace("'", "''").ToUpper & "' " & vbCrLf

                        Dim myCalculatedTests As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalculatedTests.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = myCalculatedTests
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.ReadByCalcTestLongName", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined Calculated Tests using the specified SampleType (or having at least
        ''' a Test using the specified SampleType in the formula)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCustomizedTestSelection">FALSE same order as until 3.0.2 / When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with data of the CalculatedTests using
        '''          the specified SampleType</returns>
        ''' <remarks>
        ''' AG 29/08/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function ReadBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleType As String, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT CT.CalcTestID, CT.CalcTestName, CT.CalcTestLongName, CT.FormulaText " & vbCrLf & _
                                                " , CT.CustomPosition  " & vbCrLf & _
                                                " FROM   tparCalculatedTests CT " & vbCrLf & _
                                                " WHERE  CT.UniqueSampleType = 1 " & vbCrLf & _
                                                " AND    CT.SampleType = '" & pSampleType.ToUpper & "' " & vbCrLf & _
                                                " AND    CT.EnableStatus = 1 " & vbCrLf
                        'AG 29/08/2014 BA-1869
                        If pCustomizedTestSelection Then
                            cmdText &= " AND CT.Available = 1 "
                        End If
                        'AG 29/08/2014 BA-1869

                        cmdText &= " UNION " & vbCrLf & _
                                   " SELECT CT.CalcTestID, CT.CalcTestName, CT.CalcTestLongName, CT.FormulaText " & vbCrLf & _
                                   " , CT.CustomPosition  " & vbCrLf & _
                                   " FROM   tparCalculatedTests CT INNER JOIN tparFormulas F ON CT.CalcTestID = F.CalcTestID " & vbCrLf & _
                                   " WHERE  CT.UniqueSampleType = 0 " & vbCrLf & _
                                   " AND    F.SampleType = '" & pSampleType.ToUpper & "' " & vbCrLf & _
                                   " AND    CT.EnableStatus = 1 " & vbCrLf

                        'AG 29/08/2014 BA-1869
                        If pCustomizedTestSelection Then
                            cmdText &= " AND CT.Available = 1  ORDER BY CT.CustomPosition "
                        End If
                        'AG 29/08/2014 BA-1869

                        Dim myCalculatedTests As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalculatedTests.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = myCalculatedTests
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.ReadBySampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search test data for the informed BiosystemsID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pBiosystemsID">Calculated Test Name to search by</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS containing data of the 
        '''          informed Calculated Test</returns>
        ''' <remarks>
        ''' Created by:  SG 19/02/2013
        ''' </remarks>
        Public Function ReadByBiosystemsID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pBiosystemsID As Integer, Optional pDataBaseName As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim strFromLeft As String = ""
                        If (Not String.IsNullOrEmpty(pDataBaseName)) Then strFromLeft = pDataBaseName & ".dbo."

                        Dim cmdText As String = ""
                        cmdText &= "SELECT CalcTestID" & vbCrLf
                        cmdText &= "      ,CalcTestName" & vbCrLf
                        cmdText &= "      ,CalcTestLongName" & vbCrLf
                        cmdText &= "      ,MeasureUnit" & vbCrLf
                        cmdText &= "      ,UniqueSampleType" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,Decimals" & vbCrLf
                        cmdText &= "      ,PrintExpTests" & vbCrLf
                        cmdText &= "      ,FormulaText" & vbCrLf
                        cmdText &= "      ,ActiveRangeType" & vbCrLf
                        cmdText &= "      ,InUse" & vbCrLf
                        cmdText &= "      ,EnableStatus" & vbCrLf
                        cmdText &= "      ,TS_User" & vbCrLf
                        cmdText &= "      ,TS_DateTime" & vbCrLf
                        cmdText &= "      ,PreloadedCalculatedTest" & vbCrLf
                        cmdText &= "      ,BiosystemsID" & vbCrLf 'SGM 19/02/2013
                        cmdText &= "  FROM " & strFromLeft & "tparCalculatedTests " & vbCrLf
                        cmdText &= " WHERE BiosystemsID = " & pBiosystemsID.ToString

                        Dim myCalculatedTests As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalculatedTests.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = myCalculatedTests
                        resultData.HasError = False
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.ReadByBiosystemsID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of an specific Calculated Test (only basic data, without the list of components of its Formula)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTest">Typed DataSet CalculatedTestsDS with data of the Calculated Test to modify</param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with all data of the modified Calculated Test
        '''          or error information</returns>
        ''' <remarks>
        ''' Modified by: SA 29/06/2010 - Update also field EnableStatus = TRUE (when a disabled Calculated Test is updated, the
        '''                              Formula is also updated and it contains valid Tests)
        '''              SG 03/09/2010 - Added field ActiveRangeType
        '''              SA 26/10/2010 - Added N preffix for multilanguage of field TS_User; removed set of HasError=True depending
        '''                              of value of AffectedRecords. Update also field CalcTestName, it was missing in the query
        '''              SA 05/10/2011 - In field FormulaText, replace commas by dots, to store operands with decimals in the string in 
        '''                              the same way they are saved in the Formula table
        '''              AG 02/09/2014 BA-1869 if informed update also the Available column
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTest As CalculatedTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    'Get the connected Username from the current Application Session
                    Dim cmdText As String
                    cmdText = " UPDATE tparCalculatedTests " & _
                              " SET    CalcTestLongName = N'" & pCalcTest.tparCalculatedTests(0).CalcTestLongName.ToString.Replace("'", "''") & "', " & _
                                     " CalcTestName     = N'" & pCalcTest.tparCalculatedTests(0).CalcTestName.ToString.Replace("'", "''") & "', " & _
                                     " MeasureUnit      =  '" & pCalcTest.tparCalculatedTests(0).MeasureUnit.ToString & "', " & _
                                     " UniqueSampleType =   " & Convert.ToInt32(pCalcTest.tparCalculatedTests(0).UniqueSampleType) & ", " & _
                                     " PrintExpTests    =   " & Convert.ToInt32(pCalcTest.tparCalculatedTests(0).PrintExpTests) & ", " & _
                                     " FormulaText      = N'" & pCalcTest.tparCalculatedTests(0).FormulaText.ToString.Replace("'", "''").Replace(",", ".") & "', " & _
                                     " EnableStatus     = 1, "

                    If (pCalcTest.tparCalculatedTests(0).IsTS_UserNull) Then
                        'Dim currentSession As New GlobalBase
                        cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo().UserName.Trim.Replace("'", "''") & "', "
                    Else
                        cmdText &= " TS_User = N'" & pCalcTest.tparCalculatedTests(0).TS_User.Replace("'", "''") & "', "
                    End If

                    If (pCalcTest.tparCalculatedTests(0).IsTS_DateTimeNull) Then
                        cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "', "
                    Else
                        cmdText &= " TS_DateTime = '" & pCalcTest.tparCalculatedTests(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "', "
                    End If


                    'Verify if values allowing Nulls are informed...
                    If (pCalcTest.tparCalculatedTests(0).IsSampleTypeNull OrElse pCalcTest.tparCalculatedTests(0).SampleType.ToString = "") Then
                        cmdText &= " SampleType = NULL, "
                    Else
                        cmdText &= " SampleType = '" & Replace(pCalcTest.tparCalculatedTests(0).SampleType.ToString, "'", "''") & "', "
                    End If

                    If (pCalcTest.tparCalculatedTests(0).IsActiveRangeTypeNull OrElse pCalcTest.tparCalculatedTests(0).ActiveRangeType = "") Then
                        cmdText &= " ActiveRangeType = NULL, "
                    Else
                        cmdText &= " ActiveRangeType = '" & Replace(pCalcTest.tparCalculatedTests(0).ActiveRangeType, "'", "''") & "', "
                    End If

                    If (pCalcTest.tparCalculatedTests(0).IsDecimalsNull OrElse pCalcTest.tparCalculatedTests(0).Decimals.ToString = "") Then
                        cmdText &= " Decimals = NULL "
                    Else
                        cmdText &= " Decimals = " & DAOBase.ReplaceNumericString(pCalcTest.tparCalculatedTests(0).Decimals)
                    End If

                    'AG 02/09/2014 - BA-1869 - Update also Available when informed on dataset
                    If Not pCalcTest.tparCalculatedTests(0).IsAvailableNull Then
                        cmdText &= " , Available = " & CInt(IIf(pCalcTest.tparCalculatedTests(0).Available, 1, 0))
                    End If
                    'AG 02/09/2014 - BA-1869

                    cmdText &= " WHERE CalcTestID = " & pCalcTest.tparCalculatedTests(0).CalcTestID

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.SetDatos = pCalcTest
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update field FormulaText of a Calculated Test when the long name of a Standard, ISE, Off-System or Calculated Test included in its formula is changed 
        ''' in the corresponding Programming Screen.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test</param>
        ''' <param name="pNewFormulaText">New text of the Formula of the Calculated Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/09/2012
        ''' Modified by: WE 11/11/2014 - RQ00035C (BA-1867) - Updated Summary description with ISE and Off-System as possible sources for changing its name.
        ''' </remarks>
        Public Function UpdateFormulaText(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer, ByVal pNewFormulaText As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE tparCalculatedTests " & vbCrLf & _
                                            " SET    FormulaText = N'" & pNewFormulaText.Trim.Replace("'", "''") & "' " & vbCrLf & _
                                            " WHERE  CalcTestID = " & pCalcTestID.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.UpdateFormulaText", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        '''  Update the LISValue by the pCalcTestID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pCalcTestID">Calculated Test ID.</param>
        ''' <param name="pLISValue">LIS Value.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValueByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer, pLISValue As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    'Get the connected Username from the current Application Session
                    Dim cmdText As String
                    'Dim currentSession As New GlobalBase
                    cmdText = " UPDATE tparCalculatedTests " & _
                              " SET    LISValue = N'" & pLISValue & "',"
                    cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo().UserName.Trim.Replace("'", "''") & "', "
                    cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "

                    cmdText &= " WHERE CalcTestID = " & pCalcTestID

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.UpdatepdateLISValueByTestID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Verify if there is already a Calculated Test with the informed Calculated Test Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalTestName">Calculated Test Name to be validated</param>
        ''' <param name="pNameToSearch">Value indicating which is the name to validate: the short name or the long one</param>
        ''' <param name="pCalTestID">Calculated Test Identifier. It is an optional parameter informed
        '''                          only in case of updation</param>
        ''' <param name="pReturnBoolean">Flag indicating the type of value to return inside the GlobalDataTO. When TRUE (default value),
        '''                              the function returns True/False; when FALSE, the function returns the obtained CalculatedTestsDS</param>
        ''' <returns>If pReturnBoolean = TRUE ==> GlobalDataTO containing a boolean value: True if there is another Calculated Test with the same 
        '''                                       name; otherwise, False
        '''          If pReturnBoolean = FALSE ==> GlobalDataTO containing the obtained CalculatedTestsDS</returns>
        ''' <remarks>
        ''' Created by:
        ''' Modified by: SA 26/10/2010 - Added N preffix for multilanguage when comparing by fields CalcTestName or CalcTestLongName
        '''              SA 16/10/2014 - BA-1944 (SubTask BA-2017) ==> Added new optional parameter pReturnBoolean with default value TRUE.
        '''                                                            When its value is FALSE, instead of return True/False, the function
        '''                                                            will return the obtained CalculatedTestsDS inside the GlobalDataTO.
        '''                                                            Changed the SQL to return all data of the Calculated Test.
        ''' </remarks>
        Public Function ExistsCalculatedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalTestName As String, _
                                             ByVal pNameToSearch As String, Optional ByVal pCalTestID As Integer = 0, _
                                             Optional ByVal pReturnBoolean As Boolean = True) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        If (pNameToSearch = "NAME") Then
                            cmdText = " SELECT * " & vbCrLf & _
                                      " FROM   tparCalculatedTests " & vbCrLf & _
                                      " WHERE  UPPER(CalcTestName) = N'" & pCalTestName.Trim.ToUpper.Replace("'", "''") & "' " & vbCrLf

                        ElseIf (pNameToSearch = "FNAME") Then
                            cmdText = " SELECT * " & vbCrLf & _
                                      " FROM tparCalculatedTests " & vbCrLf & _
                                      " WHERE UPPER(CalcTestLongName) = N'" & pCalTestName.Trim.ToUpper.Replace("'", "''") & "' " & vbCrLf
                        End If

                        'In case of updation, exclude the Calculated Test from the validation
                        If (pCalTestID <> 0) Then cmdText &= " AND CalcTestID <> " & pCalTestID

                        Dim myCalculatedTests As New CalculatedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalculatedTests.tparCalculatedTests)
                            End Using
                        End Using

                        If (pReturnBoolean) Then
                            resultData.SetDatos = (myCalculatedTests.tparCalculatedTests.Rows.Count > 0)
                        Else
                            'BA-1944 (SubTask BA-2017): When called from UpdateVersion Process, then return the obtained CalculatedTestsDS
                            resultData.SetDatos = myCalculatedTests
                        End If
                        resultData.HasError = False
                    End If
                    End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.ExistsCalculatedTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Calculated Tests in which Formula the informed Calculated Test is included
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of the Calculated Test to search in formulas</param>
        ''' <param name="pTestType"></param>
        ''' <returns>GlobalDataTO containing a typed DataSet CalculatedTestsDS with the list of related Calculated Tests</returns>
        ''' <remarks>
        ''' Created by:  TR 25/11/2010
        ''' AG 04/09/2014 - BA-1869 add parameter testtype
        ''' </remarks>
        Public Function GetRelatedCalculatedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pTestType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'AG 04/09/2014 - ba-1869
                        'Dim cmdText As String = " SELECT * FROM tparCalculatedTests " & vbCrLf & _
                        '                        " WHERE  CalcTestID IN (SELECT CalcTestID FROM tparFormulas " & vbCrLf & _
                        '                                              " WHERE  ValueType = 'TEST' " & vbCrLf & _
                        '                                              " AND    TestType  = 'CALC' " & vbCrLf & _
                        '                                              " AND    [Value] = '" & pTestID.ToString & "') " & vbCrLf

                        'AJG
                        'Dim cmdText As String = " SELECT * FROM tparCalculatedTests " & vbCrLf & _
                        '" WHERE  CalcTestID IN (SELECT CalcTestID FROM tparFormulas " & vbCrLf & _
                        '                      " WHERE  ValueType = 'TEST' " & vbCrLf

                        Dim cmdText As String = " SELECT * FROM tparCalculatedTests " & vbCrLf & _
                        " WHERE EXISTS (SELECT CalcTestID FROM tparFormulas " & vbCrLf & _
                                       " WHERE  ValueType = 'TEST' AND tparCalculatedTests.CalcTestID = CalcTestID " & vbCrLf

                        cmdText &= " AND    TestType  = '" & pTestType & "' " & vbCrLf & _
                                   " AND    [Value] = '" & pTestID.ToString & "') " & vbCrLf
                        'AG 04/09/2014 - BA-1869

                        Dim myCalculatedTests As New CalculatedTestsDS
                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalculatedTests.tparCalculatedTests)
                            End Using
                        End Using

                        resultData.SetDatos = myCalculatedTests
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.GetRelatedCalculatedTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Tests of the specified Test Type that can be included in a formula of a Calculated Test.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTypeTest">Determines the Test Types to be applied to the list.</param>
        ''' <returns>GlobalDataTO containing a typed DataSet AllowedTestsDS with the list of Tests allowed in 
        '''          formulas of Calculated Tests</returns>
        ''' <remarks>
        ''' Modified by: DL 13/05/2010
        '''              TR 09/03/2011 - Added the FactoryCalib on the Case STD.
        '''              AG 02/09/2014 - BA-1869 add the Available column
        '''              WE 10/11/2014 - RQ00035C (BA-1867).
        '''              WE 11/11/2014 - RQ00035C (BA-1867) Added fixed default value of 0 (False) to field FactoryCalib for ISE and Off-Systems
        '''                              to solve issue with showing warning "It's recommended to change factory calibration values".
        '''                              As a result, this message will never show up for ISE and Off-System Tests.
        '''                              Note: FactoryCalib doesn’t intrinsically exist for these Test types (ISE/Off-System).
        ''' </remarks>
        Public Function ReadAllowedTestList(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTypeTest As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""

                        Select Case (pTypeTest)
                            'Case ""
                            '    cmdText &= " SELECT 'STD' AS TestTypeCode, TS.SampleType AS SampleTypeCode, TS.TestID AS TestCode, T.TestName AS TestName, T.PreloadedTest, T.Available " & vbCrLf
                            '    cmdText &= " FROM   tparTestSamples TS INNER JOIN tparTests T ON TS.TestID = T.TestID " & vbCrLf
                            '    cmdText &= " UNION " & vbCrLf
                            '    cmdText &= " SELECT 'CALC' AS TestTypeCode, SampleType AS SampleTypeCode, CalcTestID AS TestCode, CalcTestName AS TestName, 1, Available " & vbCrLf
                            '    cmdText &= " FROM   tparCalculatedTests" & vbCrLf
                            '    cmdText &= " WHERE  CalcTestID NOT IN (SELECT CalcTestID FROM tparFormulas " & vbCrLf
                            '    cmdText &= "                           WHERE ValueType = 'TEST' AND TestType  = 'CALC') " & vbCrLf
                            '    cmdText &= " ORDER BY TestTypeCode, SampleTypeCode, TestName"

                            Case "STD"
                                cmdText &= " SELECT 'STD' AS TestTypeCode, TS.SampleType AS SampleTypeCode, TS.TestID AS TestCode, T.TestName AS TestName, T.PreloadedTest, TS.FactoryCalib, T.Available " & vbCrLf
                                cmdText &= " FROM   tparTestSamples TS INNER JOIN tparTests T ON TS.TestID = T.TestID " & vbCrLf

                                ' Exclude those Calculated Tests that are already part of the formula of (an)other Calculated Test(s).
                            Case "CALC"
                                cmdText &= " SELECT 'CALC' AS TestTypeCode, SampleType AS SampleTypeCode, CalcTestID AS TestCode, CalcTestLongName AS TestName, Available " & vbCrLf
                                cmdText &= " FROM   tparCalculatedTests" & vbCrLf
                                'AJG
                                'cmdText &= " WHERE  CalcTestID NOT IN (SELECT CalcTestID FROM tparFormulas " & vbCrLf
                                'cmdText &= "                           WHERE ValueType = 'TEST' AND TestType  = 'CALC') " & vbCrLf
                                cmdText &= " WHERE NOT EXISTS (SELECT CalcTestID FROM tparFormulas " & vbCrLf
                                cmdText &= "                           WHERE ValueType = 'TEST' AND TestType  = 'CALC' AND tparCalculatedTests.CalcTestID = CalcTestID) " & vbCrLf
                                cmdText &= " ORDER BY TestTypeCode, SampleTypeCode, TestName"

                                ' ISE: field PreloadedTest doesn't exist in ISE table because all ISE Tests are intrinsically Factory Tests, so for ISE PreloadedTest is set to 1 as a fixed value.
                                ' ISE: field FactoryCalib doesn´t intrinsically exist for ISE (and field doesn't exist in ISE table), so set this value to 0 (False) as a fixed value that must never change. 
                            Case "ISE"
                                cmdText &= " SELECT 'ISE' AS TestTypeCode, ITS.SampleType AS SampleTypeCode, ITS.ISETestID AS TestCode, IT.Name AS TestName, 1 AS PreloadedTest, 0 AS FactoryCalib, IT.Available " & vbCrLf
                                cmdText &= " FROM   tparISETestSamples ITS INNER JOIN tparISETests IT ON ITS.ISETestID = IT.ISETestID " & vbCrLf

                                ' OFFS: field FactoryCalib doesn´t intrinsically exist for Off-System (and field doesn't exist in OFFS table), so set this value to 0 (False) as a fixed value that must never change. 
                            Case "OFFS"
                                cmdText &= " SELECT 'OFFS' AS TestTypeCode, OTS.SampleType AS SampleTypeCode, OTS.OffSystemTestID AS TestCode, OT.Name AS TestName, OT.PreloadedOffSystemTest AS PreloadedTest, 0 AS FactoryCalib, OT.Available " & vbCrLf
                                cmdText &= " FROM   tparOffSystemTestSamples OTS INNER JOIN tparOffSystemTests OT ON OTS.OffSystemTestID = OT.OffSystemTestID " & vbCrLf

                                ' ISE: field PreloadedTest doesn't exist in ISE table because all ISE Tests are intrinsically Factory Tests, so for ISE PreloadedTest is set to 1 as a fixed value.
                                ' ISE: field FactoryCalib doesn´t intrinsically exist for ISE (and field doesn't exist in ISE table), so set this value to 0 (False) as a fixed value that must never change.
                                ' OFFS: field FactoryCalib doesn´t intrinsically exist for Off-System (and field doesn't exist in OFFS table), so set this value to 0 (False) as a fixed value that must never change. 
                            Case "STD_ISE_OFFS"
                                cmdText &= " SELECT 'STD' AS TestTypeCode, TS.SampleType AS SampleTypeCode, TS.TestID AS TestCode, T.TestName AS TestName, T.PreloadedTest, TS.FactoryCalib, T.Available " & vbCrLf
                                cmdText &= " FROM   tparTestSamples TS INNER JOIN tparTests T ON TS.TestID = T.TestID " & vbCrLf
                                cmdText &= " UNION " & vbCrLf
                                cmdText &= " SELECT 'ISE' AS TestTypeCode, ITS.SampleType AS SampleTypeCode, ITS.ISETestID AS TestCode, IT.Name AS TestName, 1, 0, IT.Available " & vbCrLf
                                cmdText &= " FROM   tparISETestSamples ITS INNER JOIN tparISETests IT ON ITS.ISETestID = IT.ISETestID " & vbCrLf
                                cmdText &= " UNION " & vbCrLf
                                cmdText &= " SELECT 'OFFS' AS TestTypeCode, OTS.SampleType AS SampleTypeCode, OTS.OffSystemTestID AS TestCode, OT.Name AS TestName, OT.PreloadedOffSystemTest AS PreloadedTest, 0, OT.Available " & vbCrLf
                                cmdText &= " FROM   tparOffSystemTestSamples OTS INNER JOIN tparOffSystemTests OT ON OTS.OffSystemTestID = OT.OffSystemTestID " & vbCrLf
                                cmdText &= " ORDER BY TestTypeCode DESC, PreloadedTest DESC, SampleTypeCode ASC, TestName ASC"

                        End Select

                        Dim dsAllowedTests As New AllowedTestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(dsAllowedTests.tparAllowedTests)
                            End Using
                        End Using

                        resultData.SetDatos = dsAllowedTests
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.ReadAllowedTestList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Calculated Test added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False only for Calculated Test 
        '''                                  that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010 
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for Calculated Test 
        '''                               that have been excluded from the active WorkSession  
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
                        'AJG
                        'cmdText = " UPDATE tparCalculatedTests " & _
                        '          " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & _
                        '          " WHERE  CalcTestID IN (SELECT DISTINCT WSOT.TestID " & _
                        '                                " FROM   vwksWSOrderTests WSOT " & _
                        '                                " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & _
                        '                                " AND    WSOT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                        '                                " AND    WSOT.SampleClass = 'PATIENT' " & _
                        '                                " AND    WSOT.TestType = 'CALC') "
                        cmdText = " UPDATE tparCalculatedTests " & _
                                  " SET    InUse = " & Convert.ToInt32(IIf(pFlag, 1, 0)) & _
                                  " WHERE EXISTS (SELECT WSOT.TestID " & _
                                                 " FROM   vwksWSOrderTests WSOT " & _
                                                 " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & _
                                                 " AND    WSOT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                 " AND    WSOT.SampleClass = 'PATIENT' " & _
                                                 " AND    WSOT.TestType = 'CALC' AND tparCalculatedTests.CalcTestID = WSOT.TestID) "
                    Else
                        'AJG
                        'cmdText = " UPDATE tparCalculatedTests " & _
                        '          " SET    InUse = 0 " & _
                        '          " WHERE  CalcTestID NOT IN (SELECT DISTINCT WSOT.TestID " & _
                        '                                    " FROM   vwksWSOrderTests WSOT " & _
                        '                                    " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & _
                        '                                    " AND    WSOT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                        '                                    " AND    WSOT.SampleClass = 'PATIENT' " & _
                        '                                    " AND    WSOT.TestType = 'CALC') " & _
                        '          " AND    InUse = 1 "
                        cmdText = " UPDATE tparCalculatedTests " & _
                                  " SET    InUse = 0 " & _
                                  " WHERE NOT EXISTS (SELECT WSOT.TestID " & _
                                                            " FROM   vwksWSOrderTests WSOT " & _
                                                            " WHERE  WSOT.WorkSessionID = '" & pWorkSessionID.Trim & "' " & _
                                                            " AND    WSOT.AnalyzerID = '" & pAnalyzerID.Trim & "' " & _
                                                            " AND    WSOT.SampleClass = 'PATIENT' " & _
                                                            " AND    WSOT.TestType = 'CALC' AND tparCalculatedTests.CalcTestID = WSOT.TestID) " & _
                                  " AND    InUse = 1 "
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
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.UpdateInUseFlag", EventLogEntryType.Error, False)
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
                    cmdText = " UPDATE tparCalculatedTests " & vbCrLf & _
                              " SET    InUse = " & Convert.ToInt32(IIf(pInUseFlag, 1, 0)) & vbCrLf & _
                              " WHERE  CalcTestID = " & pTestID.ToString

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
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.UpdateInUseByTestID", EventLogEntryType.Error, False)
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
                        Dim cmdText As String = " SELECT MAX(CustomPosition) FROM tparCalculatedTests "

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
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.GetLastCustomPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Gets all CALC tests order by CustomPosition (return columns: TestType, TestID, CustomPosition As TestPosition, PreloadedTest, Available)
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
                        Dim cmdText As String = " SELECT 'CALC' AS TestType, CalcTestID AS TestID, CustomPosition AS TestPosition, CalcTestName AS TestName, " & vbCrLf & _
                                                " PreloadedCalculatedTest AS PreloadedTest, Available FROM tparCalculatedTests ORDER BY CustomPosition ASC "

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
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "tparCalculatedTestsDAO.GetCustomizedSortedTestSelectionList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Update (only when informed) columns CustomPosition and Available for CALC tests
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
                            cmdText.Append(" UPDATE tparCalculatedTests SET ")

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

                            cmdText.Append(" WHERE CalcTestID  = " & testrow.TestID.ToString)
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
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.UpdateCustomPositionAndAvailable", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function


        ''' <summary>
        ''' Update calculatedTest Available value depending his components: All Available -- calculatedTest available // Some NOT available -- calculatedTest not available
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 17/09/2014 - BA-1869
        ''' </remarks>
        Public Function UpdateAvailableCascadeByComponents(ByVal pDBConnection As SqlClient.SqlConnection, pAvailableValue As Boolean, Optional ByVal pExceptions As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    If Not pAvailableValue Then
                        'Update to Available = 0 when some component not available
                        cmdText.Append(" UPDATE tparCalculatedTests SET Available = 0 WHERE CalcTestID IN ")
                    Else
                        'Update to Available = 1 when all components available
                        cmdText.Append(" UPDATE tparCalculatedTests SET Available = 1 WHERE CalcTestID NOT IN ")
                    End If

                    cmdText.Append(" (SELECT DISTINCT F.CalcTestID FROM tparFormulas F ")
                    cmdText.Append(" LEFT OUTER JOIN tparTests T ON F.ValueType = 'TEST' AND F.TestType = 'STD' AND F.Value  = T.TestID ")
                    cmdText.Append(" LEFT OUTER JOIN tparCalculatedTests  CT ON F.ValueType = 'TEST' AND F.TestType = 'CALC' AND F.Value  = CT.CalcTestID ")
                    cmdText.Append(" LEFT OUTER JOIN tparISETests IT ON F.ValueType = 'TEST' AND F.TestType = 'ISE' AND F.Value  = IT.ISETestID ")
                    cmdText.Append(" LEFT OUTER JOIN tparOffSystemTests OFT ON F.ValueType = 'TEST' AND F.TestType = 'OFFS' AND F.Value  = OFT.OffSystemTestID ")
                    cmdText.Append(" WHERE F.ValueType = 'TEST' AND (CASE F.TestType WHEN 'STD' THEN T.Available WHEN 'CALC' THEN CT.Available WHEN 'ISE' THEN IT.Available WHEN 'OFFS' THEN OFT.Available END) = 0) ")

                    If pExceptions <> "" Then
                        cmdText.Append(" AND CalcTestID NOT IN ( " & pExceptions & " ) ")
                    End If

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
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.UpdateAvailableCascadeByComponents", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "TO REVIEW - DELETE"
        ''' <summary>
        ''' Update value of field EnableStatus for a specified Calculated Test or for all Calculated Tests
        ''' having the informed one included in their Formula  -- THIS FUNCTION IS NOT USED; NOW AFFECTED 
        ''' TESTS ARE DELETED!!
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalcTestID">Identifier of the Calculated Test</param>
        ''' <param name="pEnableStatus">Value to set for the Enable Status</param>
        ''' <param name="pIncludedInFormula">When True, it indicates that the EnableStatus have to be set
        '''                                  for all Calculated Tests having the informed one included in
        '''                                  their Formula. Optional parameter with default value False</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  TR 18/05/2010
        ''' Modified by: SA 29/06/2010 - Add new optional parameter to reuse this method to set EnableStatus=False for
        '''                              Calculated Tests having the informed Calculated Test included in their Formula
        ''' </remarks>
        Public Function UpdateEnableStatus(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalcTestID As Integer, _
                                           ByVal pEnableStatus As Boolean, Optional ByVal pIncludedInFormula As Boolean = False) _
                                           As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " UPDATE tparCalculatedTests " & _
                    " SET    EnableStatus = " & IIf(pEnableStatus, 1, 0).ToString

                    If (Not pIncludedInFormula) Then
                        'Set the EnableStatus of the specified Calculated Test
                        cmdText &= " WHERE CalcTestID =  " & pCalcTestID.ToString
                    Else
                        'Set the EnableStatus of all Calculated Tests having the specified one included in 
                        'their Formula
                        'AJG
                        'cmdText &= " WHERE CalcTestID IN (SELECT CalcTestID " & _
                        '                                 " FROM   tparFormulas " & _
                        '                                 " WHERE  ValueType = 'TEST' " & _
                        '                                 " AND    TestType  = 'CALC' " & _
                        '                                 " AND    [Value] = '" & pCalcTestID.ToString & "') "

                        cmdText &= " WHERE EXISTS (SELECT CalcTestID " & _
                                                  " FROM   tparFormulas " & _
                                                  " WHERE  ValueType = 'TEST' " & _
                                                  " AND    TestType  = 'CALC' " & _
                                                  " AND    [Value] = '" & pCalcTestID.ToString & "' AND tparCalculatedTests.CalcTestID = CalcTestID) "
                    End If

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
                GlobalBase.CreateLogActivity(ex.Message, "tparCalculatedTestsDAO.UpdateEnableStatus", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function
#End Region
    End Class

End Namespace