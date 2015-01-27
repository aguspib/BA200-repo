Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Partial Public Class tparTestCalibratorsDAO
          

#Region "CRUD"
        ''' <summary>
        ''' Add all values of a link between an Experimental Calibrator and an specific TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorsDS">Typed DataSet TestCalibratorsDS containing the information to add</param>
        ''' <returns>GlobalDataTO containing success/error information. When success also return the identity value 
        '''          automatically generated for the DB for field TestCalibratorID</returns>
        ''' <remarks>
        ''' Created by : VR 31/05/2010 (Tested: Pending)
        ''' Modified by: TR 09/06/2010 - Removed columns MinFactorLimit, MaxFactorLimit
        '''              SA 27/10/2010 - Added N preffix for multilanguage of field TS_User. Other changes: if TS_User 
        '''                              is not informed in the DS, get the ID of the current logged user, if TS_DateTime 
        '''                              is not informed in the DS, use the current date and time
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorsDS As TestCalibratorsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " INSERT INTO tparTestCalibrators (TestID, SampleType, CalibratorID, CurveGrowthType, CurveType, " & _
                                                               " CurveAxisXType, CurveAxisYType, TS_User, TS_DateTime) " & _
                              " VALUES(" & pTestCalibratorsDS.tparTestCalibrators(0).TestID & ", " & _
                                    " '" & pTestCalibratorsDS.tparTestCalibrators(0).SampleType.ToString & "', " & _
                                           pTestCalibratorsDS.tparTestCalibrators(0).CalibratorID & ", "

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsCurveGrowthTypeNull Or pTestCalibratorsDS.tparTestCalibrators(0).CurveGrowthType = "") Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " '" & pTestCalibratorsDS.tparTestCalibrators(0).CurveGrowthType.ToString & "', "
                    End If

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsCurveTypeNull Or pTestCalibratorsDS.tparTestCalibrators(0).CurveType = "") Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " '" & pTestCalibratorsDS.tparTestCalibrators(0).CurveType.ToString & "', "
                    End If

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsCurveAxisXTypeNull Or pTestCalibratorsDS.tparTestCalibrators(0).CurveAxisXType = "") Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " '" & pTestCalibratorsDS.tparTestCalibrators(0).CurveAxisXType.ToString & "', "
                    End If

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsCurveAxisYTypeNull Or pTestCalibratorsDS.tparTestCalibrators(0).CurveAxisYType = "") Then
                        cmdText &= " NULL, "
                    Else
                        cmdText &= " '" & pTestCalibratorsDS.tparTestCalibrators(0).CurveAxisYType.ToString & "', "
                    End If

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsTS_UserNull) Then
                        'Dim myGlobalbase As New GlobalBase
                        cmdText &= " N'" & GlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                    Else
                        cmdText &= " N'" & pTestCalibratorsDS.tparTestCalibrators(0).TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsTS_DateTimeNull) Then
                        cmdText &= " '" & Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                    Else
                        cmdText &= " '" & pTestCalibratorsDS.tparTestCalibrators(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') "
                    End If
                    cmdText &= " SELECT SCOPE_IDENTITY()"

                    'Execute the SQL sentence 
                    Dim myTestCalibratorID As Integer = 0
                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        myTestCalibratorID = CType(dbCmd.ExecuteScalar(), Integer)
                    End Using
                    
                    If (myTestCalibratorID > 0) Then
                        'Set the calibrator ID to the received dataset
                        pTestCalibratorsDS.tparTestCalibrators(0).TestCalibratorID = myTestCalibratorID
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update values of fields containing the Curve definition for an Experimental Calibrator when it is used
        ''' for an specific TestID/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestCalibratorsDS">Typed DataSet TestCalibratorsDS containing the information to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: VR 31/01/2010 (Tested : Pending)
        '''             SA 27/10/2010 - Added N preffix for multilanguage of field TS_User
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorsDS As TestCalibratorsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " UPDATE tparTestCalibrators SET "

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsCurveGrowthTypeNull Or pTestCalibratorsDS.tparTestCalibrators(0).CurveGrowthType = "") Then
                        cmdText &= " CurveGrowthType = NULL, "
                    Else
                        cmdText &= " CurveGrowthType = '" & pTestCalibratorsDS.tparTestCalibrators(0).CurveGrowthType.ToString & "', "
                    End If

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsCurveTypeNull Or pTestCalibratorsDS.tparTestCalibrators(0).CurveType = "") Then
                        cmdText &= " CurveType = NULL, "
                    Else
                        cmdText &= " CurveType =  '" & pTestCalibratorsDS.tparTestCalibrators(0).CurveType.ToString & "', "
                    End If

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsCurveAxisXTypeNull Or pTestCalibratorsDS.tparTestCalibrators(0).CurveAxisXType = "") Then
                        cmdText &= " CurveAxisXType = NULL, "
                    Else
                        cmdText &= " CurveAxisXType = '" & pTestCalibratorsDS.tparTestCalibrators(0).CurveAxisXType.ToString & "', "
                    End If

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsCurveAxisYTypeNull Or pTestCalibratorsDS.tparTestCalibrators(0).CurveAxisYType = "") Then
                        cmdText &= " CurveAxisYType = NULL, "
                    Else
                        cmdText &= " CurveAxisYType = '" & pTestCalibratorsDS.tparTestCalibrators(0).CurveAxisYType.ToString & "', "
                    End If

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsTS_UserNull) Then
                        'Dim myGlobalbase As New GlobalBase
                        cmdText &= " TS_User = N'" & GlobalBase.GetSessionInfo().UserName().Replace("'", "''") & "', "
                    Else
                        cmdText &= " TS_User = N'" & pTestCalibratorsDS.tparTestCalibrators(0).TS_User.Replace("'", "''") & "', "
                    End If

                    If (pTestCalibratorsDS.tparTestCalibrators(0).IsTS_DateTimeNull) Then
                        cmdText &= " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' "
                    Else
                        cmdText &= " TS_DateTime = '" & pTestCalibratorsDS.tparTestCalibrators(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "' "
                    End If

                    cmdText += "  WHERE TestCalibratorID =" & pTestCalibratorsDS.tparTestCalibrators(0).TestCalibratorID & _
                               "  AND   TestID = " & pTestCalibratorsDS.tparTestCalibrators(0).TestID & _
                               "  AND   SampleType = '" & pTestCalibratorsDS.tparTestCalibrators(0).SampleType & "'" & _
                               "  AND   CalibratorID = " & pTestCalibratorsDS.tparTestCalibrators(0).CalibratorID

                    'Execute the SQL sentence 
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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Due continous SystemError i decide to create it and use it in TestCalibratorValues.Create method
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestCalibratorsID"></param>
        ''' <returns></returns>
        ''' <remarks>AG 12/11/2010</remarks>
        Public Function Exists(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorsID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText = "  SELECT TestCalibratorID "
                        cmdText &= " FROM  tparTestCalibrators "
                        cmdText &= " WHERE TestCalibratorID = " & pTestCalibratorsID

                        Dim dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = dbConnection
                        dbCmd.CommandText = cmdText

                        'Fill the DataSet to return 
                        Dim testCalibratorDS As New TestCalibratorsDS
                        Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                        dbDataAdapter.Fill(testCalibratorDS.tparTestCalibrators)

                        resultData.SetDatos = testCalibratorDS


                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.Exists", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "Other Methods"

        ''' <summary>
        ''' Get all data defined for a Single or Multipoint Calibrator used for the specified Test and Sample Type
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCalibratorIDs">List of Calibrator Identifiers that should be excluded of the possible results. 
        '''                              It is an optional parameter</param>
        ''' <returns>Dataset with structure of tables tparTestCalibrators and tparCalibrators</returns>
        ''' <remarks>
        ''' Created by:  
        ''' Modified by: SA 23/02/2010 - Get also fields TestName and TestVersionNumber
        '''              SA 31/08/2010 - Get also field SpecialCalib from tparCalibrators table (for management
        '''                              of special Tests HbA1C and HbTotal when a WS is created or updated)
        ''' </remarks>
        Public Function GetTestCalibratorData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                              ByVal pSampleType As String, Optional ByVal pCalibratorIDs As String = "") _
                                              As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TC.TestCalibratorID, TC.TestID, T.TestName, T.TestVersionNumber, TC.SampleType, TC.CalibratorID, " & _
                                                       " TC.CurveGrowthType, TC.CurveType, TC.CurveAxisXType, TC.CurveAxisYType, TC.TS_User, TC.TS_DateTime, " & _
                                                       " C.CalibratorName, C.LotNumber, C.ExpirationDate, C.NumberOfCalibrators, C.SpecialCalib " & _
                                                " FROM   tparTestCalibrators TC, tparCalibrators C, tparTests T " & _
                                                " WHERE  TC.CalibratorID = C.CalibratorID " & _
                                                " AND    TC.TestID = " & pTestID.ToString & _
                                                " AND    UPPER(TC.SampleType) = UPPER('" & pSampleType.Trim & "') " & _
                                                " AND    TC.TestID = T.TestID "

                        If (pCalibratorIDs.Trim <> String.Empty) Then cmdText += " AND TC.CalibratorID NOT IN (" & pCalibratorIDs & " ) "

                        Dim testSampleCalibratorData As New TestSampleCalibratorDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(testSampleCalibratorData.tparTestCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = testSampleCalibratorData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.GetTestCalibratorData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all data defined for a calibrator 
        ''' If pSampleType informed get all data using the SampleType parameter value)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>Dataset with structure of tables tparTestCalibratorsValues</returns>
        ''' <remarks>
        ''' Created  by: DL 23/02/2010
        ''' Modified by: AG 04/03/2010 - Added Order by CalibratorNum
        '''              AG 01/09/2010 - Added optinal parameter pSampleType
        '''              AG 01/02/2011 - Change Order DESC by ASC
        ''' </remarks>
        Public Function GetTestCalibratorValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TheoricalConcentration, CalibratorNum " & vbCrLf & _
                                                " FROM   tparTestCalibratorValues " & vbCrLf

                        'AG 01/09/2010 - use optional parameter and change query if needed
                        'cmdText += "WHERE testcalibratorid in (select testcalibratorid from tpartestcalibrators where testid = " & pTestID & ") "
                        If (pSampleType = "") Then
                            cmdText &= "WHERE TestCalibratorID IN (SELECT TestCalibratorID FROM tparTestCalibrators WHERE TestID = " & pTestID & ") "
                        Else
                            cmdText &= "WHERE TestCalibratorID IN (SELECT TestCalibratorID FROM tparTestCalibrators " & _
                                                                 " WHERE  TestID = " & pTestID & " AND SampleType = '" & pSampleType & "' ) "
                        End If

                        'AG 01/02/2011 - change DESC for ASC
                        cmdText &= "ORDER BY CalibratorNum ASC"

                        Dim testCalibratorValueData As New TestCalibratorValuesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(testCalibratorValueData.tparTestCalibratorValues)
                            End Using

                        End Using

                        resultData.SetDatos = testCalibratorValueData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.GetTestCalibratorValues", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Theorical Concentration values for all points of the Experimental Calibrator used for the specified Test/SampleType
        ''' Besides, if the Calibrator is multipoint, values for definition of the Calibration Curve are obtained (data will be duplicated
        ''' for each point, so it is enough get these values for the first point) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestCalibratorsValuesDS with all the information</returns>
        ''' <remarks>
        ''' Created  by: SA 12/07/2012 - Based in GetTestCalibratorValuesNEW but: changed the SQL Query to get also values of definition 
        '''                              of Calibration Curve; parameter SampleType is required, not optional
        ''' </remarks>
        Public Function GetTestCalibratorValuesNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                   ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TC.CurveGrowthType, TC.CurveType, TC.CurveAxisXType, TC.CurveAxisYType, " & vbCrLf & _
                                                       " TCV.CalibratorNum, TCV.TheoricalConcentration " & vbCrLf & _
                                                " FROM   tparTestCalibrators TC INNER JOIN tparTestCalibratorValues TCV ON TC.TestCalibratorID = TCV.TestCalibratorID " & vbCrLf & _
                                                " WHERE  TC.TestID = " & pTestID.ToString & vbCrLf & _
                                                " AND    TC.SampleType = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                " ORDER BY TCV.CalibratorNum ASC "

                        Dim testCalibratorValueData As New TestCalibratorValuesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(testCalibratorValueData.tparTestCalibratorValues)
                            End Using
                        End Using

                        resultData.SetDatos = testCalibratorValueData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.GetTestCalibratorValuesNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all links the specified Test has with Experimental Calibrators or, if a SampleType
        ''' is informed, only the link between the specified Test/SampleType and an Experimental Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">SampleT Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing typed DataSet TestCalibratorsDS with the links between 
        '''          the specified Test (and optionally SampleType) and Experimental Calibrators</returns>
        ''' <remarks>
        ''' Created by: TR 17/05/2010
        ''' </remarks>
        Public Function GetTestCalibratorByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                  Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM  tparTestCalibrators TC INNER JOIN tparCalibrators C ON C.CalibratorID = TC.CalibratorID " & vbCrLf & _
                                                " WHERE  TestID = " & pTestID.ToString
                        If (pSampleType.Trim <> String.Empty) Then cmdText &= " AND SampleType ='" & pSampleType & "'"

                        Dim testCalibratorDS As New TestCalibratorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(testCalibratorDS.tparTestCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = testCalibratorDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.GetTestCalibratorByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all links the specified Test has with experimental Calibrators or, if a SampleType
        ''' is informed, only the link between the specified Test/SampleType and an Experimental Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">SampleT Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: TR 17/05/2010
        ''' </remarks>
        Public Function DeleteByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                       Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    myGlobalDataTO.HasError = True
                    myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = " DELETE FROM  tparTestCalibrators " & vbCrLf & _
                                            " WHERE TestID = " & pTestID.ToString
                    If (pSampleType.Trim <> String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "'" & vbCrLf


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
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.DeleteByTestID", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get data of all Tests/SampleTypes linked to the specified Calibrator
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorID">Calibrator Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSampleCalibratorDS with data of all
        '''          Tests/SampleTypes linked to the specified Calibrator</returns>
        ''' <remarks>
        ''' Created by: TR 03/06/2010
        ''' </remarks>
        Public Function GetAllTestCalibratorByCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TC.TestCalibratorID, TC.TestID, T.TestName, T.TestVersionNumber, T.DecimalsAllowed, T.SpecialTest, " & vbCrLf & _
                                                       " TC.SampleType, TC.CalibratorID, TC.CurveGrowthType, TC.CurveType, TC.CurveAxisXType, TC.CurveAxisYType, " & vbCrLf & _
                                                       " T.InUse, TC.TS_User, TC.TS_DateTime " & vbCrLf & _
                                                " FROM   tparTestCalibrators TC INNER JOIN tparTests T ON TC.TestID = T.TestID " & vbCrLf & _
                                                " WHERE  TC.CalibratorID = " & pCalibratorID & vbCrLf

                        Dim testSampleCalibratorData As New TestSampleCalibratorDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(testSampleCalibratorData.tparTestCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = testSampleCalibratorData
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.GetAllTestCalibratorByCalibratorID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the Calibrator to which corresponds the specified TestID, SampleType
        ''' (Used in calculations class)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of an Standard Test</param>
        ''' <param name="pSampleType">Sample Type code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestSampleCalibratorDS containing the 
        '''          data of the Calibrator to which corresponds the specified TestID-SampleType</returns>
        ''' <remarks>
        ''' Created by:  AG 02/09/2010
        ''' Modified by: AG - SQL  return also ExpirationDate
        '''              SA 12/07/2012 - Changed the query to allow return value of the CalibrationFactor when the type of Calibration
        '''                              defined for the Test/SampleType is FACTOR
        ''' </remarks>
        Public Function GetCalibratorDataForCalculations(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                         ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT NULL AS CalibratorID, NULL AS NumberOfCalibrators, NULL AS SpecialCalib, " & vbCrLf & _
                                                       " TestID, SampleType, NULL AS ExpirationDate, CalibrationFactor AS CalibratorFactor " & vbCrLf & _
                                                " FROM   tparTestSamples " & vbCrLf & _
                                                " WHERE  TestID = " & pTestID.ToString & vbCrLf & _
                                                " AND    SampleType = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                " AND    CalibratorType = 'FACTOR' " & vbCrLf & _
                                                " UNION " & vbCrLf & _
                                                " SELECT C.CalibratorID, C.NumberOfCalibrators, C.SpecialCalib, TC.TestID, TC.SampleType, " & vbCrLf & _
                                                       " C.ExpirationDate, NULL AS CalibrationFactor " & vbCrLf & _
                                                " FROM   tparCalibrators C INNER JOIN tparTestCalibrators TC ON C.CalibratorID = TC.CalibratorID " & vbCrLf & _
                                                " WHERE  TC.TestID = " & pTestID.ToString & vbCrLf & _
                                                " AND    TC.SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                        Dim calibratorDataDS As New TestSampleCalibratorDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(calibratorDataDS.tparTestCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = calibratorDataDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.GetCalibratorDataForCalculations", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the maximum value of field TestCalibratorID in table tparTestCalibrators. Used in the Update Version process to assign a suitable temporary 
        ''' TestCalibratorID to new relations between Tests/SampleTypes and Calibrators (because function PrepareTestToSave needs it)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an integer value with the maximum value of field TestCalibratorID</returns>
        ''' <remarks>
        ''' Created by:  SA 09/10/20014 - BA-1944 
        ''' </remarks>
        Public Function GetMaxTestCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(TestCalibratorID) AS MaxTestCalibratorID " & vbCrLf & _
                                                " FROM   tparTestCalibrators  " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            resultData.SetDatos = dbCmd.ExecuteScalar()
                            resultData.HasError = False
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "tparTestCalibratorsDAO.GetMaxTestCalibratorID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace