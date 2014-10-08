Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO

    Public Class CalibratorUpdateDAO
        Inherits DAOBase

        ''' <summary>
        ''' Add a new Calibrator to Calibrators table
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorsDS">Typed DataSet CalibratorsDS containing the data of the Calibrator to add</param>
        ''' <returns>GlobalDataTO containing the entry typed DataSet CalibratorsDS updated with value
        '''          of the CalibratorID generated automatically for the DB when the insert was successful; 
        '''          otherwise, the error information is returned</returns>
        ''' <remarks>
        ''' Created by:  DL 30/01/2013
        ''' AG 31/01/2013 - use the proper template
        ''' </remarks>
        Public Shared Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorsDS As CalibratorsDS, pIsFactory As Boolean) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""
                    cmdText &= "INSERT INTO "
                    If pIsFactory Then cmdText &= GlobalBase.TemporalDBName & "."
                    cmdText &= "dbo.tparCalibrators" & vbCrLf
                    cmdText &= "           (CalibratorName" & vbCrLf
                    cmdText &= "           ,LotNumber" & vbCrLf
                    cmdText &= "           ,NumberOfCalibrators" & vbCrLf
                    cmdText &= "           ,InUse" & vbCrLf
                    cmdText &= "           ,ExpirationDate" & vbCrLf
                    cmdText &= "           ,TS_User" & vbCrLf
                    cmdText &= "           ,TS_DateTime)" & vbCrLf
                    cmdText &= " VALUES" & vbCrLf
                    cmdText &= "           (N'" & pCalibratorsDS.tparCalibrators(0).CalibratorName.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= "           ,N'" & pCalibratorsDS.tparCalibrators(0).LotNumber.Replace("'", "''") & "'" & vbCrLf
                    cmdText &= "           ," & pCalibratorsDS.tparCalibrators(0).NumberOfCalibrators & vbCrLf
                    cmdText &= "           ,0" & vbCrLf

                    If (pCalibratorsDS.tparCalibrators(0).IsExpirationDateNull) Then
                        cmdText &= "           ,NULL" & vbCrLf
                    Else
                        cmdText &= "           ,'" & pCalibratorsDS.tparCalibrators(0).ExpirationDate.ToString("yyyyMMdd") & "'" & vbCrLf
                    End If

                    If (pCalibratorsDS.tparCalibrators(0).IsTS_UserNull) Then
                        Dim myGlobalBase As New GlobalBase
                        cmdText &= "           ,N'" & myGlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "'" & vbCrLf
                    Else
                        cmdText &= "           ,N'" & pCalibratorsDS.tparCalibrators(0).TS_User.Trim.Replace("'", "''") & "'" & vbCrLf
                    End If

                    If (pCalibratorsDS.tparCalibrators(0).IsTS_DateTimeNull) Then
                        cmdText &= "           ,'" & Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                    Else
                        cmdText &= "           ,'" & pCalibratorsDS.tparCalibrators(0).TS_DateTime.ToString("yyyyMMdd HH:mm:ss") & "') "
                    End If

                    'Add the scope identity because the ID is generated automatically by the database
                    cmdText &= " SELECT SCOPE_IDENTITY()"

                    'Execute the SQL sentence 
                    Dim dbCmd As New SqlClient.SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    Dim calibratorID As Integer = 0
                    calibratorID = CType(dbCmd.ExecuteScalar(), Integer)
                    If (calibratorID > 0) Then
                        'Set the calibrator ID to the Recived dataset
                        pCalibratorsDS.tparCalibrators(0).CalibratorID = calibratorID
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorUpdateDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData

        End Function

        ''' <summary>
        ''' Gets the list of all calibrators that in local DB don't match with Factory DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>
        ''' All the results are the calibrators that can be removed or ignored
        ''' </returns>
        ''' <remarks>
        ''' Created by: DL - 29/01/2013
        ''' </remarks>
        Public Shared Function GetCalibratorsDistinctInFactory(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'CALC Tests
                        'All data in Ax00 DataBase different from data in Ax00_TEM
                        Dim cmdText As String = ""
                        cmdText &= "SELECT CalibratorName" & vbCrLf
                        cmdText &= "      ,NumberOfCalibrators" & vbCrLf
                        cmdText &= "      ,SpecialCalib " & vbCrLf
                        cmdText &= "  FROM dbo.tparCalibrators" & vbCrLf
                        cmdText &= "EXCEPT" & vbCrLf
                        cmdText &= "SELECT CalibratorName" & vbCrLf
                        cmdText &= "      ,NumberOfCalibrators"
                        cmdText &= "      ,SpecialCalib" & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparCalibrators" & vbCrLf

                        Dim myCalibrators As New CalibratorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalibrators.tparCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = myCalibrators
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorUpdateDAO.GetPreloadedCALCTestsDistinctInClient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets the list of all tests calibrators that in local DB don't match with Factory DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>
        ''' All the results are the test calibrators that can be removed or ignored
        ''' </returns>
        ''' <remarks>
        ''' Created by: DL - 30/01/2013
        ''' </remarks>
        Public Shared Function GetTestsCalibratorsDistinctInFactory(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Tests calibrators
                        'All data in Ax00 DataBase different from data in Ax00_TEM

                        Dim cmdText As String = ""
                        cmdText &= "SELECT TestCalibratorID" & vbCrLf
                        cmdText &= "      ,TestID" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,CalibratorID " & vbCrLf
                        cmdText &= "      ,CurveGrowthType " & vbCrLf
                        cmdText &= "      ,CurveType " & vbCrLf
                        cmdText &= "      ,CurveAxisXType " & vbCrLf
                        cmdText &= "      ,CurveAxisYType " & vbCrLf
                        cmdText &= "  FROM dbo.tparTestCalibrators" & vbCrLf
                        cmdText &= "EXCEPT" & vbCrLf
                        cmdText &= "SELECT TestCalibratorID" & vbCrLf
                        cmdText &= "      ,TestID" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,CalibratorID " & vbCrLf
                        cmdText &= "      ,CurveGrowthType " & vbCrLf
                        cmdText &= "      ,CurveType " & vbCrLf
                        cmdText &= "      ,CurveAxisXType " & vbCrLf
                        cmdText &= "      ,CurveAxisYType " & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparTestCalibrators" & vbCrLf

                        Dim myTestCalibrators As New TestCalibratorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestCalibrators.tparTestCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = myTestCalibrators
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorUpdateDAO.GetTestsCalibratorsDistinctInFactory", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get differents in tparcalibrators between local and temporal Db
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <returns>True if find element otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by:  DL 29/01/2013
        ''' </remarks>
        Public Shared Function GetTestsCalibratorsDistinctInClient(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Tests calibrators
                        'All data in Ax00 DataBase different from data in Ax00_TEM

                        Dim cmdText As String = ""
                        cmdText &= "SELECT TestCalibratorID" & vbCrLf
                        cmdText &= "      ,TestID" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,CalibratorID " & vbCrLf
                        cmdText &= "      ,CurveGrowthType " & vbCrLf
                        cmdText &= "      ,CurveType " & vbCrLf
                        cmdText &= "      ,CurveAxisXType " & vbCrLf
                        cmdText &= "      ,CurveAxisYType " & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparTestCalibrators" & vbCrLf
                        cmdText &= "EXCEPT" & vbCrLf
                        cmdText &= "SELECT TestCalibratorID" & vbCrLf
                        cmdText &= "      ,TestID" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,CalibratorID " & vbCrLf
                        cmdText &= "      ,CurveGrowthType " & vbCrLf
                        cmdText &= "      ,CurveType " & vbCrLf
                        cmdText &= "      ,CurveAxisXType " & vbCrLf
                        cmdText &= "      ,CurveAxisYType " & vbCrLf
                        cmdText &= "  FROM dbo.tparTestCalibrators" & vbCrLf

                        Dim myTestCalibrators As New TestCalibratorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestCalibrators.tparTestCalibrators)
                            End Using
                        End Using

                        resultData.SetDatos = myTestCalibrators
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorUpdateDAO.GetTestsCalibratorsDistinctInClient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function


        'SELECT TestCalibratorID, CalibratorNum, TheoricalConcentration, KitConcentrationRelation, BaseConcentration  
        'FROM Ax00_DEV.dbo.tparTestCalibratorValues 

        ''' <summary>
        ''' Gets the list of all tests calibrators that in local DB don't match with Factory DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>
        ''' All the results are the test calibrators that can be removed or ignored
        ''' </returns>
        ''' <remarks>
        ''' Created by: DL - 30/01/2013
        ''' </remarks>
        Public Shared Function GetTestsCalibratorsValuesDistinctInFactory(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Tests calibrators
                        'All data in Ax00 DataBase different from data in Ax00_TEM

                        Dim cmdText As String = ""
                        cmdText &= "SELECT TestCalibratorID" & vbCrLf
                        cmdText &= "      ,CalibratorNum" & vbCrLf
                        cmdText &= "      ,TheoricalConcentration" & vbCrLf
                        cmdText &= "      ,KitConcentrationRelation " & vbCrLf
                        cmdText &= "      ,BaseConcentration " & vbCrLf
                        cmdText &= "  FROM dbo.tparTestCalibratorValues" & vbCrLf
                        cmdText &= "EXCEPT" & vbCrLf
                        cmdText &= "SELECT TestCalibratorID" & vbCrLf
                        cmdText &= "      ,CalibratorNum" & vbCrLf
                        cmdText &= "      ,TheoricalConcentration" & vbCrLf
                        cmdText &= "      ,KitConcentrationRelation " & vbCrLf
                        cmdText &= "      ,BaseConcentration " & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparTestCalibratorValues" & vbCrLf

                        Dim myTestCalibratorsValues As New TestCalibratorValuesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestCalibratorsValues.tparTestCalibratorValues)
                            End Using
                        End Using

                        resultData.SetDatos = myTestCalibratorsValues
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorUpdateDAO.GetTestsCalibratorsValuesDistinctInFactory", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get differents in tparcalibrators between local and temporal Db
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <returns>True if find element otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by:  DL 29/01/2013
        ''' </remarks>
        Public Shared Function GetTestsCalibratorsValuesDistinctInClient(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Tests calibrators
                        'All data in Ax00 DataBase different from data in Ax00_TEM

                        Dim cmdText As String = ""
                        cmdText &= "SELECT TestCalibratorID" & vbCrLf
                        cmdText &= "      ,CalibratorNum" & vbCrLf
                        cmdText &= "      ,TheoricalConcentration" & vbCrLf
                        cmdText &= "      ,KitConcentrationRelation " & vbCrLf
                        cmdText &= "      ,BaseConcentration " & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparTestCalibratorValues" & vbCrLf
                        cmdText &= "EXCEPT" & vbCrLf
                        cmdText &= "SELECT TestCalibratorID" & vbCrLf
                        cmdText &= "      ,CalibratorNum" & vbCrLf
                        cmdText &= "      ,TheoricalConcentration" & vbCrLf
                        cmdText &= "      ,KitConcentrationRelation " & vbCrLf
                        cmdText &= "      ,BaseConcentration " & vbCrLf
                        cmdText &= "  FROM dbo.tparTestCalibratorValues" & vbCrLf

                        Dim myTestCalibratorsValues As New TestCalibratorValuesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myTestCalibratorsValues.tparTestCalibratorValues)
                            End Using
                        End Using

                        resultData.SetDatos = myTestCalibratorsValues
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorUpdateDAO.GetTestsCalibratorsDistinctInClient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function



        ''' <summary>
        ''' Get differents in tpartestcalibrators between local and temporal Db
        ''' </summary>
        ''' <param name="pDBConnection">Open Database Connection</param>
        ''' <returns>True if find element otherwise it returns False</returns>
        ''' <remarks>
        ''' Created by:  DL 29/01/2013
        ''' </remarks>
        Public Shared Function GetTestCalibratorsDistinctInClient(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        cmdText &= "SELECT TestCalibratorID" & vbCrLf
                        cmdText &= "      ,TestID" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,CalibratorID" & vbCrLf
                        cmdText &= "      ,CurveGrowthType" & vbCrLf
                        cmdText &= "      ,CurveType" & vbCrLf
                        cmdText &= "      ,CurveAxisXType" & vbCrLf
                        cmdText &= "      ,CurveAxisYType" & vbCrLf
                        cmdText &= "  FROM " & GlobalBase.TemporalDBName & ".dbo.tparTestCalibrators" & vbCrLf
                        cmdText &= "EXCEPT" & vbCrLf
                        cmdText &= "SELECT TestCalibratorID" & vbCrLf
                        cmdText &= "      ,TestID" & vbCrLf
                        cmdText &= "      ,SampleType" & vbCrLf
                        cmdText &= "      ,CalibratorID" & vbCrLf
                        cmdText &= "      ,CurveGrowthType" & vbCrLf
                        cmdText &= "      ,CurveType" & vbCrLf
                        cmdText &= "      ,CurveAxisXType" & vbCrLf
                        cmdText &= "      ,CurveAxisYType" & vbCrLf
                        cmdText &= "  FROM tparTestCalibrators" & vbCrLf

                        Dim myCalibrators As New CalibratorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalibrators.tparCalibratorsTests)
                            End Using
                        End Using

                        dataToReturn.SetDatos = myCalibrators
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorUpdateDAO.GetTestCalibratorsDistinctInClient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS (NEW AND UPDATED FUNCTIONS)"

        ''' <summary>
        ''' Search in FACTORY DB all data of the specified CALIBRATOR. The query includes also 1 As IsNew.
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCalibratorName">Name of the Calibrator to search</param>
        ''' <returns>GlobalDataTO containing a CalibratorsDS with all data of the informed Calibrator in Factory DB</returns>
        ''' <remarks>
        ''' Created by: SA 08/10/2014 - BA-1944 (Sub Task 1982)
        ''' </remarks>
        Public Function GetDataInFactoryDB(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCalibratorName As String) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT *, 1 AS IsNew FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparCalibrators] " & vbCrLf & _
                                                " WHERE  CalibratorName = N'" & pCalibratorName.Trim.Replace("'", "''") & "' " & vbCrLf

                        Dim myCalibrators As New CalibratorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalibrators.tparCalibrators)
                            End Using
                        End Using

                        dataToReturn.SetDatos = myCalibrators
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorUpdateDAO.GetDataInFactoryDB", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function

        ''' <summary>
        ''' Search in FACTORY DB all new or modified experimental Calibrators
        ''' </summary>
        ''' <param name="pDBConnection">Open BD Connection</param>
        ''' <returns>GlobalDataTO containing a CalibratorsDS with all experimental Calibrators added or updated in FACTORY DB</returns>
        ''' <remarks>
        ''' Created by:  DL 29/01/2013
        ''' Modified by: SA 08/10/2014 - BA-1944 (Sub Task 1982)
        ''' </remarks>
        Public Shared Function GetCalibratorsDistinctInClient(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim dataToReturn As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                dataToReturn = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not dataToReturn.HasError AndAlso Not dataToReturn.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(dataToReturn.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT CalibratorName, NumberOfCalibrators, SpecialCalib " & vbCrLf & _
                                                " FROM " & GlobalBase.TemporalDBName & ".[dbo].[tparCalibrators] " & vbCrLf & _
                                                " EXCEPT " & vbCrLf & _
                                                " SELECT CalibratorName, NumberOfCalibrators, SpecialCalib " & vbCrLf & _
                                                " FROM   [Ax00].[dbo].[tparCalibrators] " & vbCrLf

                        Dim myCalibrators As New CalibratorsDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myCalibrators.tparCalibrators)
                            End Using
                        End Using

                        dataToReturn.SetDatos = myCalibrators
                        dataToReturn.HasError = False
                    End If
                End If
            Catch ex As Exception
                dataToReturn = New GlobalDataTO()
                dataToReturn.HasError = True
                dataToReturn.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                dataToReturn.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "CalibratorUpdateDAO.GetCalibratorsDistinctInClient", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return dataToReturn
        End Function
#End Region

    End Class


End Namespace