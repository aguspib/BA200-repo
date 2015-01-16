Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class TestParametersUpdateDAO
        Inherits DAOBase

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS (NEW AND UPDATED FUNCTIONS)"
        ''' <summary>
        ''' Search in FACTORY DB all STD Tests that not exist in CUSTOMER DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a TestsDS with Test Identifiers of all STD Tests that exists in FACTORY DB but not in the CUSTOMER DB</returns>
        ''' <remarks>
        ''' Created by: SA 07/10/2014 - BA-1944 (SubTask BA- 1980)
        ''' </remarks>
        Public Function GetNewFactoryTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TestID FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTests] " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT TestID FROM [Ax00].[dbo].[tparTests] " & vbCrLf

                        Dim factoryTestsDS As New TestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryTestsDS.tparTests)
                            End Using
                        End Using

                        resultData.SetDatos = factoryTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetNewFactoryTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all data of the specified STD Test. If pMarkNew = True, the query includes 1 As NewTest.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">STD Test Identifier</param>
        ''' <param name="pMarkNew">Optional parameter to return value of field NewTest: when True, it means the function is used to add
        '''                        new elements and flag NewTest has to be set to TRUE in the DS to return</param>
        ''' <returns>GlobalDataTO containing a TestsDS with all data of the specified STD Test in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by: SA 07/10/2014 - BA-1944 (SubTask BA-1980)
        ''' </remarks>
        Public Function GetDataInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                           Optional ByVal pMarkNew As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT *, " & IIf(pMarkNew, 1, 0).ToString & "As NewTest " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTests] " & vbCrLf & _
                                                " WHERE  TestID = " & pTestID.ToString & vbCrLf

                        Dim factoryTestsDS As New TestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryTestsDS.tparTests)
                            End Using
                        End Using

                        resultData.SetDatos = factoryTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetDataInFactoryDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in tables tparTestReagents and tparReagents in FACTORY DB, all Reagents used for the informed STD TestID. 
        ''' If pMarkNew = True, the query includes 1 As IsNew.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">STD Test Identifier</param>
        ''' <param name="pMarkNew">Optional parameter to return value of field IsNew: when True, it means the function is used to add
        '''                        new elements and flag IsNew has to be set to TRUE in the DS to return</param>
        ''' <returns>GlobalDataTO containing a ReagentsDS with data of all Reagents linked to the specified STD TestID in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by: SA 07/10/2014 - BA-1944 (SubTask BA- 1980)
        ''' </remarks>
        Public Function GetFactoryReagentsByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                   Optional ByVal pMarkNew As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TR.TestID, TR.ReagentNumber, R.*, " & IIf(pMarkNew, 1, 0).ToString & "As IsNew " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTestReagents] TR INNER JOIN " & vbCrLf & _
                                                           GlobalBase.TemporalDBName & ".[dbo].[tparReagents] R ON TR.ReagentID = R.ReagentID " & vbCrLf & _
                                                " WHERE TR.TestID = " & pTestID.ToString & vbCrLf & _
                                                " ORDER BY TR.ReagentNumber "

                        Dim factoryTestReagentsDS As New ReagentsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryTestReagentsDS.tparReagents)
                            End Using
                        End Using

                        resultData.SetDatos = factoryTestReagentsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetFactoryReagentsByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in table tparTestSamples in FACTORY DB, data of all Sample Types for the informed STD TestID, or data for the specified STD TestID and Sample Type. 
        ''' If MarkNew = True, the query includes 1 As IsNew and, for those Sample Types with CalibratorType = EXPERIMENT, the query includes also 1 As FactoryCalib.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">STD Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter; when it is informed, only data of the informed Sample Type is obtained for the Test</param>
        ''' <param name="pMarkNew">Optional parameter to return value of field IsNew: when True, it means the function is used to add
        '''                        new elements and flag IsNew has to be set to TRUE in the DS to return</param>
        ''' <returns>GlobalDataTO containing a TestSamplesDS with data of all Sample Types defined for the specified STD TestID in FACTORY DB or,
        '''          if optional parameter pSampleType is informed, only the data of that specific SampleType</returns>
        ''' <remarks>
        ''' Created by: SA 07/10/2014 - BA-1944 (SubTask BA- 1980)
        ''' </remarks>
        Public Function GetFactoryTestSamplesByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                      Optional ByVal pSampleType As String = "", Optional ByVal pMarkNew As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = String.Empty

                        If (pMarkNew) Then
                            cmdText = " SELECT *, 1 AS IsNew, (CASE WHEN CalibratorType = 'EXPERIMENT' THEN 1 ELSE 0 END) AS FactoryCalib " & vbCrLf
                        Else
                            cmdText = " SELECT *, 0 AS IsNew, 0 AS FactoryCalib " & vbCrLf
                        End If

                        cmdText &= " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTestSamples] " & vbCrLf & _
                                   " WHERE TestID = " & pTestID.ToString

                        If (pSampleType.Trim <> String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' " & vbCrLf


                        Dim factoryTestSamplesDS As New TestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryTestSamplesDS.tparTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = factoryTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetFactoryTestSamplesByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in table tparTestCalibratorValues (joined with tparTestCalibrators by TestCalibratorID) in FACTORY DB, data of all points of the 
        ''' Experimental Calibrators used for the informed STD TestID (all Sample Types) or for the informed STD TestID and Sample Type. If 
        ''' MarkNew = True, the query includes 1 As IsNew.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">STD Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter; when it is informed, only data of the informed Sample Type is obtained for the Test</param>
        ''' <param name="pMarkNew">Optional parameter to return value of field IsNew: when True, it means the function is used to add
        '''                        new elements and flag IsNew has to be set to TRUE in the DS to return</param>
        ''' <returns>GlobalDataTO containing a TestCalibratorsValuesDS with values for all points of all Experimental Calibrators used for the informed STD TestID 
        '''          (all Sample Types) in FACTORY DB or, if optional parameter pSampleType is informed, for all points of the Experimental Calibrator used for 
        '''          the informed STD TestID and Sample Type</returns>
        ''' <remarks>
        ''' Created by: SA 07/10/2014 - BA-1944 (SubTask BA- 1980)
        ''' </remarks>
        Public Function GetFactoryTestCalibValuesByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                          Optional ByVal pSampleType As String = "", Optional ByVal pMarkNew As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TCV.*, " & IIf(pMarkNew, 1, 0).ToString & "As IsNew " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTestCalibratorValues] TCV INNER JOIN " & vbCrLf & _
                                                           GlobalBase.TemporalDBName & ".[dbo].[tparTestCalibrators] TC ON TCV.TestCalibratorID = TC.TestCalibratorID " & vbCrLf & _
                                                " WHERE TC.TestID = " & pTestID.ToString & vbCrLf

                        'When informed, get data only for the specified SampleType
                        If (pSampleType.Trim <> String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                        Dim factoryTestCalibValuesDS As New TestCalibratorValuesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryTestCalibValuesDS.tparTestCalibratorValues)
                            End Using
                        End Using

                        resultData.SetDatos = factoryTestCalibValuesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetFactoryTestCalibValuesByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in table tparTestReagentsVolumes in FACTORY DB all Reagents Volumes for the informed STD TestID (all Sample Types) or for the informed 
        ''' STD TestID and SampleType. If MarkNew = True, the query includes 1 As IsNew.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">STD Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter; when it is informed, only data of the informed Sample Type is obtained for the Test</param>
        ''' <param name="pMarkNew">Optional parameter to return value of field IsNew: when True, it means the function is used to add
        '''                        new elements and flag IsNew has to be set to TRUE in the DS to return</param>
        ''' <returns>GlobalDataTO containing a TestReagentsVolumesDS with all Reagents Volumes for the informed STD TestID (all Sample Types) in FACTORY DB or, 
        '''          if optional parameter pSampleType is informed, for the informed STD TestID and Sample Type</returns>
        ''' <remarks>
        ''' Created by:  TR 01/02/2013
        ''' Modified by: SA 07/10/2014 - BA-1944 (SubTask BA- 1980) ==> Parameter pSampleType changed to optional 
        '''                                                             Added new optional parameter pMarkNew with default value FALSE
        '''                                                             Changed the SQL to return value of IsNew flag depending on value of parameter pMarkNew
        ''' </remarks>
        Public Function GetFactoryReagentsVolumesByTesIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                                      Optional ByVal pSampleType As String = "", Optional ByVal pMarkNew As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TestID, ReagentID, ReagentNumber, SampleType, " & vbCrLf & _
                                                       " ReagentVolume, CONVERT(DECIMAL(20,12),ReagentVolumeSteps) AS ReagentVolumeSteps, " & vbCrLf & _
                                                       " RedPostReagentVolume, CONVERT(DECIMAL(20,12), RedPostReagentVolumeSteps) AS RedPostReagentVolumeSteps, " & vbCrLf & _
                                                       " IncPostReagentVolume, CONVERT(DECIMAL(20,12), IncPostReagentVolumeSteps) AS IncPostReagentVolumeSteps, " & vbCrLf & _
                                                       IIf(pMarkNew, 1, 0).ToString & " AS IsNew " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].tparTestReagentsVolumes " & vbCrLf & _
                                                " WHERE TestID = " & pTestID.ToString & vbCrLf

                        'When informed, get data only for the specified SampleType
                        If (pSampleType.Trim <> String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.ToString & "' "

                        Dim myTestReagentsVolsDS As New TestReagentsVolumesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestReagentsVolsDS.tparTestReagentsVolumes)
                            End Using
                        End Using

                        myGlobalDataTO.SetDatos = myTestReagentsVolsDS
                        myGlobalDataTO.HasError = False
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetReagentsVolumesByTesIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Search in table tparTestCalibrators in FACTORY DB data of the Experimental Calibrators used  for the informed STD TestID (all Sample Types) 
        ''' or for the informed STD TestID and Sample Type. If MarkNew = True, the query includes 1 As IsNew.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">STD Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter; when it is informed, only data of the informed Sample Type is obtained for the Test</param>
        ''' <param name="pMarkNew">Optional parameter to return value of field IsNew: when True, it means the function is used to add
        '''                        new elements and flag IsNew has to be set to TRUE in the DS to return</param>
        ''' <returns>GlobalDataTO containing a TestCalibratorsDS with data of the Experimental Calibrators used for the informed STD TestID (all Sample Types) 
        '''          or, if optional parameter pSampleType is informed, for the informed STD TestID and Sample Type</returns>
        ''' <remarks>
        ''' Created by:  TR
        ''' Modified by: SA 07/10/2014 - BA-1944 (SubTask BA- 1980) ==> Added new optional parameter pMarkNew with default value FALSE
        '''                                                             Changed the SQL to return value of IsNew flag depending on value of parameter pMarkNew
        ''' </remarks>
        Public Function GetFactoryTestCalibratorByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                         Optional ByVal pSampleType As String = "", Optional ByVal pMarkNew As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TC.*, C.CalibratorName, " & IIf(pMarkNew, 1, 0).ToString & " AS IsNew " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].tparTestCalibrators TC INNER JOIN " & vbCrLf & _
                                                           GlobalBase.TemporalDBName & ".[dbo].tparCalibrators C ON C.CalibratorID = TC.CalibratorID " & vbCrLf & _
                                                " WHERE  TC.TestID = " & pTestID.ToString & vbCrLf

                        'When informed, get data only for the specified SampleType
                        If (pSampleType.Trim <> String.Empty) Then cmdText &= " AND TC.SampleType ='" & pSampleType.Trim & "' " & vbCrLf

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
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetFactoryTestCalibratorByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Execute the query to search all STD Tests that should be deleted (those preloaded STD Tests that exist in CUSTOMER DB 
        ''' but not in FACTORY DB)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a TestsDS with the list of identifiers of STD Tests that have to be deleted from Customer DB</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (SubTask BA- 1983)
        ''' </remarks>
        Public Function GetDeletedPreloadedSTDTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TestID FROM [Ax00].[dbo].[tparTests] " & vbCrLf & _
                                                " WHERE  PreloadedTest = 1 " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT TestID FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTests] " & vbCrLf

                        Dim customerTestsDS As New TestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(customerTestsDS.tparTests)
                            End Using
                        End Using

                        resultData.SetDatos = customerTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetDeletedPreloadedSTDTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Execute the query to search in CUSTOMER DB all Sample Types for preloaded STS Tests that should be deleted (those Sample Types of preloaded 
        ''' STD Tests that exist in CUSTOMER DB but not in FACTORY DB)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a TestsDS with the list of pairs of STD TestID/SampleType that have to be deleted from Customer DB</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (SubTask BA- 1983)
        ''' </remarks>
        Public Function GetDeletedPreloadedSTDTestSamples(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TS.TestID, TS.SampleType " & vbCrLf & _
                                                " FROM [Ax00].[dbo].[tparTestSamples] TS INNER JOIN [Ax00].[dbo].[tparTests] T " & vbCrLf & _
                                                                                               " ON TS.TestID = T.TestID " & vbCrLf & _
                                                " WHERE  PreloadedTest = 1 " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT TestID, SampleType FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTestSamples] " & vbCrLf

                        Dim customerTestSamplesDS As New TestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(customerTestSamplesDS.tparTests)
                            End Using
                        End Using

                        resultData.SetDatos = customerTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetDeletedPreloadedSTDTestSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all STD Tests that exist in CUSTOMER DB but for which at least one of the relevant fields have been changed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a TestsDS with value of the relevant fields in FACTORY DB for modified STD Tests</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (SubTask BA- 1980)
        ''' </remarks>
        Public Function GetUpdatedFactoryTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TestID, AnalysisMode, ReactionType, ReadingMode, MainWaveLength, ReferenceWaveLength, " & vbCrLf & _
                                                       " FirstReadingCycle, SecondReadingCycle, KineticBlankLimit, ReagentsNumber " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTests] " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT TestID, AnalysisMode, ReactionType, ReadingMode, MainWaveLength, ReferenceWaveLength, " & vbCrLf & _
                                                       " FirstReadingCycle, SecondReadingCycle, KineticBlankLimit, ReagentsNumber " & vbCrLf & _
                                                " FROM   [Ax00].[dbo].[tparTests] " & vbCrLf

                        Dim factoryTestsDS As New TestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryTestsDS.tparTests)
                            End Using
                        End Using

                        resultData.SetDatos = factoryTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetUpdatedFactoryTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all new Sample Types added to preloaded STD Tests that do not exist in CUSTOMER DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a TestsDS with all pairs of TestID/SampleType that exists in FACTORY DB but not in the CUSTOMER DB</returns>
        ''' <remarks>
        ''' Created by: SA 09/10/2014 - BA-1944 (SubTask BA- 1981)
        ''' </remarks>
        Public Function GetNewFactoryTestSamples(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TestID, SampleType FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTestSamples] " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT TestID, SampleType FROM [Ax00].[dbo].[tparTestSamples] " & vbCrLf

                        Dim factoryTestsDS As New TestsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryTestsDS.tparTests)
                            End Using
                        End Using

                        resultData.SetDatos = factoryTestsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetNewFactoryTestSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all STD Test/SampleTypes that exist in CUSTOMER DB but for which at least one of the relevant Sample Type fields 
        ''' have been changed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a TestSamplesDS with value of the relevant Sample Type fields in FACTORY DB for all pairs of 
        ''' modified STD TestID/SampleTypes
        '''  </returns>
        ''' <remarks>
        ''' Created by: SA 10/10/2014 - BA-1944 (SubTask BA- 1985)
        ''' </remarks>
        Public Function GetUpdatedFactoryTestSamples(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT TestID, SampleType, SampleVolume, SampleVolumeSteps, PredilutionUseFlag, PredilutionMode, " & vbCrLf & _
                                                       " PredilutionFactor, PredilutedSampleVol, PredilutedSampleVolSteps, PredilutedDiluentVol, PreDiluentVolSteps, " & vbCrLf & _
                                                       " DiluentSolution, IncPostdilutionFactor, IncPostSampleVolume, IncPostSampleVolumeSteps, " & vbCrLf & _
                                                       " RedPostdilutionFactor, RedPostSampleVolume, RedPostSampleVolumeSteps, " & vbCrLf & _
                                                       " BlankAbsorbanceLimit, LinearityLimit, DetectionLimit, FactorLowerLimit, " & vbCrLf & _
                                                       " FactorUpperLimit, SubstrateDepletionValue,SlopeFactorA, SlopeFactorB, " & vbCrLf & _
                                                       " CalibratorType, CalibrationFactor, SampleTypeAlternative " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTestSamples] " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT TS.TestID, SampleType, SampleVolume, SampleVolumeSteps, PredilutionUseFlag, PredilutionMode, " & vbCrLf & _
                                                         " PredilutionFactor, PredilutedSampleVol, PredilutedSampleVolSteps, PredilutedDiluentVol, PreDiluentVolSteps, " & vbCrLf & _
                                                       " DiluentSolution, IncPostdilutionFactor, IncPostSampleVolume, IncPostSampleVolumeSteps, " & vbCrLf & _
                                                       " RedPostdilutionFactor, RedPostSampleVolume, RedPostSampleVolumeSteps, " & vbCrLf & _
                                                       " BlankAbsorbanceLimit, LinearityLimit, DetectionLimit, FactorLowerLimit, " & vbCrLf & _
                                                       " FactorUpperLimit, SubstrateDepletionValue,SlopeFactorA, SlopeFactorB, " & vbCrLf & _
                                                       " CalibratorType, CalibrationFactor, SampleTypeAlternative " & vbCrLf & _
                                                " FROM [Ax00].[dbo].[tparTestSamples] TS INNER JOIN [Ax00].[dbo].[tparTests] T ON TS.TestID = T.TestID " & vbCrLf & _
                                                " WHERE T.PreloadedTest = 1 "

                        Dim factoryTestSamplesDS As New TestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryTestSamplesDS.tparTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = factoryTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetUpdatedFactoryTestSamples", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Volumes of Reagents for preloaded STD Tests that have been changed (add or updated) in FACTORY DB. If optional parameters
        ''' TestID and SampleType are informed, then the changes verification is made only for the informed STD Test and SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">STD Test Identifier; optional parameter</param>
        ''' <param name="pSampleType">Sample Type Code; optional parameter</param>
        ''' <returns>GlobalDataTO containing a TestReagentsVolumesDS with all new or updated Reagents Volumes in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by: SA 10/10/2014 - BA-1944 (SubTask BA- 1985)
        ''' </remarks>
        Public Function GetUpdatedFactoryTestReagentsVolumes(ByVal pDBConnection As SqlClient.SqlConnection, Optional ByVal pTestID As Integer = 0, _
                                                             Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOptionalFilters As String = String.Empty
                        If (pTestID <> 0 AndAlso pSampleType <> String.Empty) Then
                            myOptionalFilters = " TRV.TestID = " & pTestID.ToString & vbCrLf & _
                                                " AND TRV.SampleType = '" & pSampleType.Trim & "' " & vbCrLf
                        End If

                        Dim cmdText As String = " SELECT TRV.TestID, TRV.SampleType, TRV.ReagentNumber, TRV.ReagentVolume " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparTestReagentsVolumes] TRV " & vbCrLf & _
                                                IIf(myOptionalFilters = String.Empty, String.Empty, " WHERE " & myOptionalFilters).ToString & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT TRV.TestID, TRV.SampleType, TRV.ReagentNumber, TRV.ReagentVolume " & vbCrLf & _
                                                " FROM   [Ax00].[dbo].[tparTestReagentsVolumes] TRV INNER JOIN [Ax00].[dbo].[tparTests] T ON TRV.TestID = T.TestID " & vbCrLf & _
                                                " WHERE  T.PreloadedTest = 1 " & vbCrLf & _
                                                IIf(myOptionalFilters = String.Empty, String.Empty, " AND " & myOptionalFilters).ToString

                        Dim factoryTestReagentsVolsDS As New TestReagentsVolumesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(factoryTestReagentsVolsDS.tparTestReagentsVolumes)
                            End Using
                        End Using

                        resultData.SetDatos = factoryTestReagentsVolsDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetUpdatedFactoryTestReagentsVolumes", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace

#Region "FUNCTIONS FOR OLD UPDATE VERSION PROCESS (NOT USED)"
' ''' <summary>
' ''' Get all elements that are not equal or not in local db.
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <returns></returns>
' ''' <remarks>CREATED BY: TR 31/01/2013</remarks>
'Public Function GetAffectedItemsFromFactory(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'    Dim dataToReturn As GlobalDataTO = Nothing
'    Dim dbConnection As SqlClient.SqlConnection = Nothing

'    Try
'        dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
'        If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
'            dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                Dim cmdText As String = ""
'                cmdText &= "SELECT  tparTests.TestID, tparTests.TestName, tparTests.ShortName, tparTests.PreloadedTest, tparTests.AnalysisMode, tparTestSamples.SampleType, tparTests.MeasureUnit,  " & vbCrLf
'                cmdText &= "        tparTests.DecimalsAllowed, tparTests.ReplicatesNumber, tparTests.ReactionType, tparTests.ReadingMode, tparTests.MainWavelength, " & vbCrLf
'                cmdText &= "        tparTests.ReferenceWavelength, tparTestReagentsVolumes.ReagentNumber, tparTestReagentsVolumes.ReagentVolume, tparTestSamples.SampleVolume," & vbCrLf
'                cmdText &= "        tparTests.FirstReadingCycle, tparTests.SecondReadingCycle, tparTestSamples.PredilutionUseFlag, tparTestSamples.PredilutionMode, " & vbCrLf
'                cmdText &= "        tparTestSamples.PredilutionFactor, tparTestSamples.DiluentSolution, tparTestSamples.RedPostdilutionFactor, tparTestSamples.IncPostdilutionFactor, " & vbCrLf
'                cmdText &= "        tparTests.BlankMode, tparTestSamples.BlankAbsorbanceLimit, tparTests.KineticBlankLimit, tparTestSamples.LinearityLimit, tparTestSamples.DetectionLimit," & vbCrLf
'                cmdText &= "        tparTestSamples.FactorLowerLimit , tparTestSamples.FactorUpperLimit, tparTestSamples.SubstrateDepletionValue, " & vbCrLf
'                cmdText &= "        tparTestSamples.CalibrationFactor,tparTestSamples.SampleTypeAlternative, tparTestSamples.SlopeFactorA,tparTestSamples.SlopeFactorB, tparReagents.CodeTest "
'                cmdText &= "FROM    " & GlobalBase.TemporalDBName & ".[dbo].tparTests INNER JOIN" & vbCrLf
'                cmdText &= "               " & GlobalBase.TemporalDBName & ".[dbo].tparTestSamples ON tparTests.TestID = tparTestSamples.TestID INNER JOIN " & vbCrLf
'                cmdText &= "               " & GlobalBase.TemporalDBName & ".[dbo].tparTestReagentsVolumes ON tparTestSamples.TestID = tparTestReagentsVolumes.TestID AND " & vbCrLf
'                cmdText &= "               tparTestSamples.SampleType = tparTestReagentsVolumes.SampleType INNER JOIN" & vbCrLf
'                cmdText &= "               " & GlobalBase.TemporalDBName & ".[dbo].tparReagents ON tparTestReagentsVolumes.ReagentID =  tparReagents.ReagentID" & vbCrLf
'                cmdText &= "EXCEPT " & vbCrLf
'                cmdText &= "SELECT  tparTests.TestID, tparTests.TestName, tparTests.ShortName, tparTests.PreloadedTest, tparTests.AnalysisMode, tparTestSamples.SampleType, tparTests.MeasureUnit,  " & vbCrLf
'                cmdText &= "        tparTests.DecimalsAllowed, tparTests.ReplicatesNumber, tparTests.ReactionType, tparTests.ReadingMode, tparTests.MainWavelength, " & vbCrLf
'                cmdText &= "        tparTests.ReferenceWavelength, tparTestReagentsVolumes.ReagentNumber, tparTestReagentsVolumes.ReagentVolume, tparTestSamples.SampleVolume," & vbCrLf
'                cmdText &= "        tparTests.FirstReadingCycle, tparTests.SecondReadingCycle, tparTestSamples.PredilutionUseFlag, tparTestSamples.PredilutionMode, " & vbCrLf
'                cmdText &= "        tparTestSamples.PredilutionFactor, tparTestSamples.DiluentSolution, tparTestSamples.RedPostdilutionFactor, tparTestSamples.IncPostdilutionFactor, " & vbCrLf
'                cmdText &= "        tparTests.BlankMode, tparTestSamples.BlankAbsorbanceLimit, tparTests.KineticBlankLimit, tparTestSamples.LinearityLimit, tparTestSamples.DetectionLimit," & vbCrLf
'                cmdText &= "        tparTestSamples.FactorLowerLimit , tparTestSamples.FactorUpperLimit, tparTestSamples.SubstrateDepletionValue, " & vbCrLf
'                cmdText &= "        tparTestSamples.CalibrationFactor,tparTestSamples.SampleTypeAlternative, tparTestSamples.SlopeFactorA,tparTestSamples.SlopeFactorB, tparReagents.CodeTest "
'                cmdText &= "FROM    [Ax00].[dbo].tparTests INNER JOIN" & vbCrLf
'                cmdText &= "               [Ax00].[dbo].tparTestSamples ON tparTests.TestID = tparTestSamples.TestID INNER JOIN " & vbCrLf
'                cmdText &= "               [Ax00].[dbo].tparTestReagentsVolumes ON tparTestSamples.TestID = tparTestReagentsVolumes.TestID AND " & vbCrLf
'                cmdText &= "               tparTestSamples.SampleType = tparTestReagentsVolumes.SampleType INNER JOIN " & vbCrLf
'                cmdText &= "        [Ax00].[dbo].tparReagents ON tparTestReagentsVolumes.ReagentID =  tparReagents.ReagentID " & vbCrLf
'                cmdText &= "" & vbCrLf
'                cmdText &= "" & vbCrLf
'                cmdText &= "" & vbCrLf

'                Dim resultData As New DataSet
'                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
'                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                        dbDataAdapter.Fill(resultData)
'                    End Using
'                End Using

'                dataToReturn.SetDatos = resultData
'                dataToReturn.HasError = False
'            End If
'        End If
'    Catch ex As Exception
'        dataToReturn = New GlobalDataTO()
'        dataToReturn.HasError = True
'        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        dataToReturn.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetAffectedItemsFromFactory", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return dataToReturn
'End Function

' ''' <summary>
' ''' Get all affected Test to be remove from Client DB.
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <returns></returns>
' ''' <remarks>CREATED BY: TR 06/02/2013</remarks>
'Public Function GetAffectedItemsFromFactoryRemoves(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
'    Dim dataToReturn As GlobalDataTO = Nothing
'    Dim dbConnection As SqlClient.SqlConnection = Nothing

'    Try
'        dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
'        If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
'            dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                Dim cmdText As String = ""

'                cmdText &= "SELECT  tparTests.TestID, tparTests.TestName, tparTests.ShortName, tparTests.PreloadedTest, tparTests.AnalysisMode, tparTestSamples.SampleType, tparTests.MeasureUnit,  " & vbCrLf
'                cmdText &= "        tparTests.DecimalsAllowed, tparTests.ReplicatesNumber, tparTests.ReactionType, tparTests.ReadingMode, tparTests.MainWavelength, " & vbCrLf
'                cmdText &= "        tparTests.ReferenceWavelength, tparTestReagentsVolumes.ReagentNumber, tparTestReagentsVolumes.ReagentVolume, tparTestSamples.SampleVolume," & vbCrLf
'                cmdText &= "        tparTests.FirstReadingCycle, tparTests.SecondReadingCycle, tparTestSamples.PredilutionUseFlag, tparTestSamples.PredilutionMode, " & vbCrLf
'                cmdText &= "        tparTestSamples.PredilutionFactor, tparTestSamples.DiluentSolution, tparTestSamples.RedPostdilutionFactor, tparTestSamples.IncPostdilutionFactor, " & vbCrLf
'                cmdText &= "        tparTests.BlankMode, tparTestSamples.BlankAbsorbanceLimit, tparTests.KineticBlankLimit, tparTestSamples.LinearityLimit, tparTestSamples.DetectionLimit," & vbCrLf
'                cmdText &= "        tparTestSamples.SlopeFactorA, tparTestSamples.SlopeFactorB, tparTestSamples.SubstrateDepletionValue, " & vbCrLf
'                cmdText &= "        tparTestSamples.CalibrationFactor,tparTestSamples.SampleTypeAlternative "
'                cmdText &= "FROM    [Ax00].[dbo].tparTests INNER JOIN" & vbCrLf
'                cmdText &= "               [Ax00].[dbo].tparTestSamples ON tparTests.TestID = tparTestSamples.TestID INNER JOIN " & vbCrLf
'                cmdText &= "               [Ax00].[dbo].tparTestReagentsVolumes ON tparTestSamples.TestID = tparTestReagentsVolumes.TestID AND " & vbCrLf
'                cmdText &= "               tparTestSamples.SampleType = tparTestReagentsVolumes.SampleType" & vbCrLf
'                cmdText &= "EXCEPT " & vbCrLf
'                cmdText &= "SELECT  tparTests.TestID, tparTests.TestName, tparTests.ShortName, tparTests.PreloadedTest, tparTests.AnalysisMode, tparTestSamples.SampleType, tparTests.MeasureUnit,  " & vbCrLf
'                cmdText &= "        tparTests.DecimalsAllowed, tparTests.ReplicatesNumber, tparTests.ReactionType, tparTests.ReadingMode, tparTests.MainWavelength, " & vbCrLf
'                cmdText &= "        tparTests.ReferenceWavelength, tparTestReagentsVolumes.ReagentNumber, tparTestReagentsVolumes.ReagentVolume, tparTestSamples.SampleVolume," & vbCrLf
'                cmdText &= "        tparTests.FirstReadingCycle, tparTests.SecondReadingCycle, tparTestSamples.PredilutionUseFlag, tparTestSamples.PredilutionMode, " & vbCrLf
'                cmdText &= "        tparTestSamples.PredilutionFactor, tparTestSamples.DiluentSolution, tparTestSamples.RedPostdilutionFactor, tparTestSamples.IncPostdilutionFactor, " & vbCrLf
'                cmdText &= "        tparTests.BlankMode, tparTestSamples.BlankAbsorbanceLimit, tparTests.KineticBlankLimit, tparTestSamples.LinearityLimit, tparTestSamples.DetectionLimit," & vbCrLf
'                cmdText &= "        tparTestSamples.SlopeFactorA, tparTestSamples.SlopeFactorB, tparTestSamples.SubstrateDepletionValue, " & vbCrLf
'                cmdText &= "        tparTestSamples.CalibrationFactor,tparTestSamples.SampleTypeAlternative "
'                cmdText &= "FROM    " & GlobalBase.TemporalDBName & ".[dbo].tparTests INNER JOIN" & vbCrLf
'                cmdText &= "               " & GlobalBase.TemporalDBName & ".[dbo].tparTestSamples ON tparTests.TestID = tparTestSamples.TestID INNER JOIN " & vbCrLf
'                cmdText &= "               " & GlobalBase.TemporalDBName & ".[dbo].tparTestReagentsVolumes ON tparTestSamples.TestID = tparTestReagentsVolumes.TestID AND " & vbCrLf
'                cmdText &= "               tparTestSamples.SampleType = tparTestReagentsVolumes.SampleType" & vbCrLf

'                Dim resultData As New DataSet
'                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
'                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                        dbDataAdapter.Fill(resultData)
'                    End Using
'                End Using

'                dataToReturn.SetDatos = resultData
'                dataToReturn.HasError = False
'            End If
'        End If
'    Catch ex As Exception
'        dataToReturn = New GlobalDataTO()
'        dataToReturn.HasError = True
'        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        dataToReturn.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetAffectedItemsFromFactory", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return dataToReturn
'End Function

' ''' <summary>
' ''' Get the test on the factory DB.
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pTestID">Test ID.</param>
' ''' <returns></returns>
' ''' <remarks>CREATE BY: TR 31/01/2013</remarks>
'Public Function GetDataInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, pTestID As Integer) As GlobalDataTO
'    Dim dataToReturn As GlobalDataTO = Nothing
'    Dim dbConnection As SqlClient.SqlConnection = Nothing

'    Try
'        dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
'        If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
'            dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                Dim cmdText As String = ""
'                cmdText &= "SELECT  *"
'                cmdText &= "FROM   " & GlobalBase.TemporalDBName & ".[dbo].tparTests "
'                cmdText &= "WHERE TestID = " & pTestID

'                Dim resultData As New TestsDS
'                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
'                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                        dbDataAdapter.Fill(resultData.tparTests)
'                    End Using
'                End Using

'                dataToReturn.SetDatos = resultData
'                dataToReturn.HasError = False
'            End If
'        End If
'    Catch ex As Exception
'        dataToReturn = New GlobalDataTO()
'        dataToReturn.HasError = True
'        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        dataToReturn.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetDataInFactoryDB", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return dataToReturn
'End Function

' ''' <summary>
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pTestName"></param>
' ''' <param name="pSampleType"></param>
' ''' <returns></returns>
' ''' <remarks>Modify AG 13/03/2014 - #1538 fix issue when name contains char ' (use .Replace("'", "''"))</remarks>
'Public Function GetDataInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, pTestName As String, Optional pSampleType As String = "") As GlobalDataTO
'    Dim dataToReturn As GlobalDataTO = Nothing
'    Dim dbConnection As SqlClient.SqlConnection = Nothing

'    Try
'        dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
'        If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
'            dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                Dim cmdText As String = ""
'                If pSampleType = "" Then
'                    cmdText &= "SELECT  *"
'                    cmdText &= "FROM   " & GlobalBase.TemporalDBName & ".[dbo].tparTests "
'                    cmdText &= "WHERE TestName = N'" & pTestName.Replace("'", "''") & "'"
'                Else
'                    cmdText &= "SELECT T.*, TS.SampleType " & vbCrLf
'                    cmdText &= "  FROM  " & GlobalBase.TemporalDBName & ".[dbo].tparTests T,  " & GlobalBase.TemporalDBName & ".[dbo].tparTestSamples TS " & vbCrLf
'                    cmdText &= " WHERE T.TestName = N'" & pTestName.Replace("'", "''") & "' " & vbCrLf
'                    cmdText &= "   AND TS.TestID = T.TestID" & vbCrLf

'                    cmdText &= "   AND TS.SampleType = '" & pSampleType & "'"

'                End If

'                Dim resultData As New TestsDS
'                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
'                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                        dbDataAdapter.Fill(resultData.tparTests)
'                    End Using
'                End Using

'                dataToReturn.SetDatos = resultData
'                dataToReturn.HasError = False
'            End If
'        End If
'    Catch ex As Exception
'        dataToReturn = New GlobalDataTO()
'        dataToReturn.HasError = True
'        dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        dataToReturn.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "TestParametersUpdateDAO.GetDataInFactoryDB", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return dataToReturn
'End Function

' ''' <summary>
' ''' Get the TestSample information by the Test ID and the Sample Type on Factory Table.
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pTestID">Test ID.</param>
' ''' <param name="pSampleType">Sample Type.</param>
' ''' <returns></returns>
' ''' <remarks>CREATED BY: TR 31/01/2013</remarks>
'Public Function GetFactoryTestSampleByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
'                                                                            Optional pSampleType As String = "") As GlobalDataTO
'    Dim myGlobalDataTO As New GlobalDataTO
'    Dim dbConnection As New SqlClient.SqlConnection

'    Try
'        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
'        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
'            dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                Dim cmdText As String = String.Empty

'                cmdText &= " SELECT * "
'                cmdText &= "FROM " & GlobalBase.TemporalDBName & ".[dbo].tparTestSamples "
'                cmdText &= " WHERE  TestID = " & pTestID
'                If Not pSampleType = "" Then
'                    cmdText &= " AND    SampleType = '" & pSampleType & "' "
'                End If

'                Dim myTestSamplesDS As New TestSamplesDS
'                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
'                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                        dbDataAdapter.Fill(myTestSamplesDS.tparTestSamples)
'                    End Using
'                End Using

'                myGlobalDataTO.HasError = False
'                myGlobalDataTO.SetDatos = myTestSamplesDS
'            End If
'        End If

'    Catch ex As Exception
'        myGlobalDataTO.HasError = True
'        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        myGlobalDataTO.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "tparTestSamplesDAO.Read", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return myGlobalDataTO
'End Function

' ''' <summary>
' ''' Gete the TestReagents data from factory DB.
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pTestID">Test ID</param>
' ''' <returns></returns>
' ''' <remarks>
' ''' CREATED BY: TR 04/02/2013
' ''' </remarks>
'Public Function GetFactroyTestReagents(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
'    Dim myGlobalDataTO As GlobalDataTO = Nothing
'    Dim dbConnection As SqlClient.SqlConnection = Nothing

'    Try
'        myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
'        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
'            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                Dim cmdText As String = " SELECT T.TestID, T.TestName, T.ReagentsNumber, T.BlankReplicates, " & vbCrLf & _
'                                               " TR.ReagentID, TR.ReagentNumber, R.ReagentName, R.PreloadedReagent " & vbCrLf & _
'                                        " FROM " & GlobalBase.TemporalDBName & ".[dbo].tparTestReagents TR INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].tparReagents R ON TR.ReagentID = R.ReagentID " & vbCrLf & _
'                                                                   " INNER JOIN " & GlobalBase.TemporalDBName & ".[dbo].tparTests T ON TR.TestID = T.TestID " & vbCrLf & _
'                                        " WHERE  TR.TestID    = " & pTestID & vbCrLf & _
'                                        " ORDER BY TR.ReagentNumber "

'                Dim resultData As New TestReagentsDS
'                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
'                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                        dbDataAdapter.Fill(resultData.tparTestReagents)
'                    End Using
'                End Using

'                myGlobalDataTO.SetDatos = resultData
'                myGlobalDataTO.HasError = False
'            End If
'        End If
'    Catch ex As Exception
'        myGlobalDataTO = New GlobalDataTO
'        myGlobalDataTO.HasError = True
'        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        myGlobalDataTO.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "tparTestReagentsDAO.GetTestReagents", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return myGlobalDataTO
'End Function

' ''' <summary>
' ''' Get the reagent data from Factory DB.
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pReagentID">ReagentID</param>
' ''' <returns></returns>
' ''' <remarks>
' ''' CREATED BY: TR 04/02/2013
' ''' </remarks>
'Public Function GetFactoryReagent(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pReagentID As Integer) As GlobalDataTO
'    Dim resultData As New GlobalDataTO
'    Dim reagentsDataDS As New ReagentsDS
'    Dim dbConnection As New SqlClient.SqlConnection

'    Try
'        resultData = GetOpenDBConnection(pDBConnection)
'        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
'            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                Dim cmdText As String = ""
'                cmdText &= " SELECT *" & vbCrLf
'                cmdText &= " FROM  " & GlobalBase.TemporalDBName & ".[dbo].tparReagents " & vbCrLf
'                cmdText &= " WHERE  ReagentID = " & pReagentID.ToString

'                Dim dbCmd As New SqlClient.SqlCommand
'                dbCmd.Connection = dbConnection
'                dbCmd.CommandText = cmdText

'                'Fill the DataSet to return 
'                Dim dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                dbDataAdapter.Fill(reagentsDataDS.tparReagents)

'                resultData.SetDatos = reagentsDataDS
'                resultData.HasError = False
'            End If
'        End If
'    Catch ex As Exception
'        resultData.HasError = True
'        resultData.ErrorCode = "SYSTEM_ERROR"
'        resultData.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "tparReagentsDAO.GetFactoryReagent", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return resultData
'End Function

' ''' <summary>
' ''' Get Factory Calibrator data from Factory DB.
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pCalibratorID">Calibrator ID.</param>
' ''' <returns></returns>
' ''' <remarks>
' ''' CREATED BY: TR 04/02/2013
' ''' </remarks>
'Public Function GetFactoryCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorID As Integer) As GlobalDataTO
'    Dim resultData As GlobalDataTO = Nothing
'    Dim dbConnection As SqlClient.SqlConnection = Nothing

'    Try
'        resultData = GetOpenDBConnection(pDBConnection)
'        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
'            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then

'                Dim cmdText As String = ""
'                cmdText &= "SELECT CalibratorID" & vbCrLf
'                cmdText &= "      ,CalibratorName" & vbCrLf
'                cmdText &= "      ,LotNumber" & vbCrLf
'                cmdText &= "      ,ExpirationDate" & vbCrLf
'                cmdText &= "      ,NumberOfCalibrators" & vbCrLf
'                cmdText &= "      ,InUse" & vbCrLf
'                cmdText &= "      ,SpecialCalib" & vbCrLf
'                cmdText &= "      ,TS_User" & vbCrLf
'                cmdText &= "      ,TS_DateTime" & vbCrLf
'                cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparCalibrators" & vbCrLf
'                cmdText &= " WHERE CalibratorID = " & pCalibratorID.ToString

'                Dim calibratorsDataDS As New CalibratorsDS
'                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
'                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                        dbDataAdapter.Fill(calibratorsDataDS.tparCalibrators)
'                    End Using
'                End Using

'                resultData.SetDatos = calibratorsDataDS
'                resultData.HasError = False
'            End If
'        End If
'    Catch ex As Exception
'        resultData = New GlobalDataTO()
'        resultData.HasError = True
'        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        resultData.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "tparCalibratorsDAO.GetFactoryCalibrator", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return resultData
'End Function

' ''' <summary>
' ''' Get the factory test calibrator values.
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <param name="pTestCalibratorID">Test Calibrator ID</param>
' ''' <returns></returns>
' ''' <remarks>CREATED BY: TR 04/02/2013</remarks>
'Public Function GetFactoryTestCalibratorValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestCalibratorID As Integer) As GlobalDataTO
'    Dim myGlobalDataTO As GlobalDataTO = Nothing
'    Dim dbConnection As SqlClient.SqlConnection = Nothing

'    Try
'        myGlobalDataTO = GetOpenDBConnection(pDBConnection)
'        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
'            dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                Dim cmdText As String = " SELECT * FROM " & GlobalBase.TemporalDBName & ".dbo.tparTestCalibratorValues " & vbCrLf & _
'                                        " WHERE  TestCalibratorID = " & pTestCalibratorID & vbCrLf

'                Dim myTestCalibratorValueDS As New TestCalibratorValuesDS
'                Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
'                    Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
'                        dbDataAdapter.Fill(myTestCalibratorValueDS.tparTestCalibratorValues)
'                    End Using
'                End Using

'                myGlobalDataTO.SetDatos = myTestCalibratorValueDS
'                myGlobalDataTO.HasError = False
'            End If
'        End If
'    Catch ex As Exception
'        myGlobalDataTO = New GlobalDataTO()
'        myGlobalDataTO.HasError = True
'        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
'        myGlobalDataTO.ErrorMessage = ex.Message

'        'Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, " tparTestCalibratorsDAO.ReadByTestCalibratorID ", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return myGlobalDataTO
'End Function
#End Region