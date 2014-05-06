Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalEnumerates
Imports System.Data.SqlTypes
Imports System.Text

Namespace Biosystems.Ax00.DAL.DAO

    Public Class tparISETestSamplesDAO
        Inherits DAOBase

#Region "CRUD"

        ''' <summary>
        ''' Add a new ISETestSample
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestSamplesRow">Typed DataSet ISETestSamplesDS containing the data of the ISETestSample to add</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>Created by: XBC 15/10/2010
        ''' AG 21/10/2010 - remove RangeLower and RangeUpper fields
        ''' AG - receive a row not a DS</remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, _
                               ByVal pISETestSamplesRow As ISETestSamplesDS.tparISETestSamplesRow) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    'cmdText = " INSERT INTO tparISETestSamples(ISETestID, SampleType, SampleType_ResultID, Decimals, ISE_Volume, ISE_DilutionFactor" & _
                    '"                              , ISE_RangeLower, ISE_RangeUpper, ActiveRangeType, TS_User, TS_DateTime) " & _
                    cmdText = " INSERT INTO tparISETestSamples(ISETestID, SampleType, SampleType_ResultID, Decimals, ISE_Volume, ISE_DilutionFactor" & _
                              "                              , ActiveRangeType, TS_User, TS_DateTime) " & _
                              " VALUES(" & pISETestSamplesRow.ISETestID & ", " & _
                              " N'" & pISETestSamplesRow.SampleType.ToString().Replace("'", "''") & "'," & _
                              " N'" & pISETestSamplesRow.SampleType_ResultID.ToString().Replace("'", "''") & "'," & _
                              " " & pISETestSamplesRow.Decimals.ToString & ", " '& _
                    '" " & ReplaceNumericString(pISETestSamplesRow.ISE_Volume) & ", " TR 01/02/2012 -Removed from Table

                    If pISETestSamplesRow.IsISE_DilutionFactorNull Then
                        cmdText &= "NULL, "
                    Else
                        cmdText &= ReplaceNumericString(pISETestSamplesRow.ISE_DilutionFactor) & ", "
                    End If

                    'cmdText &= " " & ReplaceNumericString(pISETestSamplesDetails.tparISETestSamples(0).ISE_RangeLower) & ", " & _
                    '           " " & ReplaceNumericString(pISETestSamplesDetails.tparISETestSamples(0).ISE_RangeUpper) & ", "

                    If pISETestSamplesRow.IsActiveRangeTypeNull Then
                        cmdText &= "NULL, "
                    Else
                        cmdText &= "N'" & pISETestSamplesRow.ActiveRangeType.ToString().Replace("'", "''") & "',"
                    End If

                    If (pISETestSamplesRow.IsTS_UserNull) Then
                        'Get the logged User
                        Dim currentSession As New GlobalBase
                        cmdText &= " N'" & currentSession.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                    Else
                        cmdText &= " N'" & pISETestSamplesRow.TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    If (pISETestSamplesRow.IsTS_DateTimeNull) Then
                        cmdText &= " '" & DateTime.Now.ToString("yyyyMMdd HH:mm:ss") & "') "
                    Else
                        cmdText &= " '" & CType(pISETestSamplesRow.TS_DateTime.ToString(), DateTime).ToString("yyyyMMdd HH:mm:ss") & "') "
                    End If

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                        resultData.SetDatos = pISETestSamplesRow
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparISETestSamplesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add a new ISETestSample
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestSamplesRow">Typed DataSet ISETestSamplesDS containing the data of the ISETestSample to add</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>Created by: XBC 15/10/2010
        ''' AG 21/10/2010 - remove RangeLower and RangeUpper fields
        ''' AG 27/10/2010 - parameter is row not DS</remarks>
        ''' RH 12/06/2012 - Add QCActive, ControlReplicates, NumberOfControls, RejectionCriteria, CalculationMode, 
        '''                 NumberOfSeries and TotalAllowedError fields. Code optimization.
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestSamplesRow As ISETestSamplesDS.tparISETestSamplesRow) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String
                    cmdText = " UPDATE tparISETestSamples " & _
                              " SET    SampleType_ResultID = N'" & pISETestSamplesRow.SampleType_ResultID & "', " & _
                              "        Decimals =  " & pISETestSamplesRow.Decimals & ", "

                    If pISETestSamplesRow.IsISE_DilutionFactorNull Then
                        cmdText &= " ISE_DilutionFactor = NULL, "
                    Else
                        cmdText &= " ISE_DilutionFactor = " & pISETestSamplesRow.ISE_DilutionFactor.ToSQLString() & ", "
                    End If

                    If pISETestSamplesRow.IsActiveRangeTypeNull Then
                        cmdText &= " ActiveRangeType = NULL, "
                    Else
                        cmdText &= " ActiveRangeType = N'" & pISETestSamplesRow.ActiveRangeType & "', "
                    End If

                    If (pISETestSamplesRow.IsTS_UserNull) Then
                        'Get the logged User
                        Dim currentSession As New GlobalBase
                        cmdText &= " TS_User = N'" & currentSession.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', "
                    Else
                        cmdText &= " TS_User = N'" & pISETestSamplesRow.TS_User.Trim.Replace("'", "''") & "', "
                    End If

                    If (pISETestSamplesRow.IsTS_DateTimeNull) Then
                        cmdText &= " TS_DateTime = '" & DateTime.Now.ToSQLString() & "', "
                    Else
                        cmdText &= " TS_DateTime = '" & pISETestSamplesRow.TS_DateTime.ToSQLString() & "', "
                    End If

                    If (pISETestSamplesRow.IsQCActiveNull) Then
                        cmdText &= " QCActive = 0, "
                    Else
                        cmdText &= " QCActive = " & IIf(pISETestSamplesRow.QCActive, 1, 0).ToString() & ", "
                    End If

                    If (pISETestSamplesRow.IsControlReplicatesNull) Then
                        cmdText &= " ControlReplicates = NULL, "
                    Else
                        cmdText &= " ControlReplicates = " & pISETestSamplesRow.ControlReplicates & ", "
                    End If

                    If (pISETestSamplesRow.IsNumberOfControlsNull) Then
                        cmdText &= " NumberOfControls = NULL, "
                    Else
                        cmdText &= " NumberOfControls = " & pISETestSamplesRow.NumberOfControls & ", "
                    End If

                    If (pISETestSamplesRow.IsRejectionCriteriaNull) Then
                        cmdText &= " RejectionCriteria = NULL, "
                    Else
                        cmdText &= " RejectionCriteria = " & pISETestSamplesRow.RejectionCriteria.ToSQLString() & ", "
                    End If

                    If (pISETestSamplesRow.IsCalculationModeNull) Then
                        cmdText &= " CalculationMode = '', "
                    Else
                        cmdText &= " CalculationMode = '" & pISETestSamplesRow.CalculationMode & "', "
                    End If

                    If (pISETestSamplesRow.IsNumberOfSeriesNull) Then
                        cmdText &= " NumberOfSeries = NULL, "
                    Else
                        cmdText &= " NumberOfSeries = " & pISETestSamplesRow.NumberOfSeries & ", "
                    End If

                    If (pISETestSamplesRow.IsTotalAllowedErrorNull) Then
                        cmdText &= " TotalAllowedError = NULL "
                    Else
                        cmdText &= " TotalAllowedError = " & pISETestSamplesRow.TotalAllowedError.ToSQLString() & " "
                    End If

                    cmdText &= " WHERE ISETestID = " & pISETestSamplesRow.ISETestID & " " & _
                               " AND   SampleType = '" & pISETestSamplesRow.SampleType & "' "

                    'Execute the SQL Sentence
                    Using dbCmd As New SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                    End Using

                    If (resultData.AffectedRecords = 1) Then
                        resultData.HasError = False
                        resultData.SetDatos = pISETestSamplesRow
                    Else
                        resultData.HasError = True
                        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                    End If

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparISETestSamplesDAO.Update", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified ISETestSample
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISETest Identifier</param>
        ''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        ''' <remarks>Created by: XBC 15/10/2010</remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim cmdText As String = ""
                    cmdText = " DELETE FROM tparISETestSamples " & _
                              " WHERE  ISETestID = " & pISETestID

                    'Execute the SQL Sentence
                    Dim dbCmd As New SqlCommand
                    dbCmd.Connection = pDBConnection
                    dbCmd.CommandText = cmdText

                    resultData.AffectedRecords = dbCmd.ExecuteNonQuery
                    If (resultData.AffectedRecords > 0) Then
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparISETestSamplesDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined ISE TestSamples for the specified ISE Test. Besides, if a SampleType is informed,
        ''' then verify if the specified SampleType exists for the ISE Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISETest Identifier</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ISETestSamplesDS with the list of ISE Test Samples</returns>
        ''' <remarks>
        ''' Created by:  XBC 18/10/2010
        ''' Modified by: AG  22/10/2010 - Added Order by SampleType Position field (MasterData table)
        '''              SA  25/10/2010 - Added optional parameter pSampleType, to allow verify if an specific SampleType is used
        '''                               for the informed ISE Test. Filter the query by SampleType when informed
        '''              SA  09/11/2010 - Get multilanguage description for field SampleType
        '''              SA  21/02/2012 - Changed the function template
        '''              XB  04/06/2013 - Update query to return also tparTestSamples without multilanguage translation.
        '''              TR  26/06/2013 - Add in the query sentence the IT.Name AS ISETestName, IT.ShortName as ISETestShortName, IT.Units as MeasureUnit 
        '''                               needed on the update process.
        ''' </remarks>
        Public Function GetByISETestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, _
                                       Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim myGlobalBase As New GlobalBase

                        ' XB  04/06/2013
                        'Dim cmdText As String = " SELECT ITS.*, ITS.SampleType + '-' + MR.ResourceText AS SampleTypeDesc " & vbCrLf & _
                        '                        " FROM   tparISETestSamples ITS INNER JOIN tcfgMasterData MD ON ITS.SampleType = MD.ItemID " & vbCrLf & _
                        '                                                      " INNER JOIN tfmwMultiLanguageResources MR ON MD.ResourceID = MR.ResourceID " & vbCrLf & _
                        '                        " WHERE  MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString & "' " & vbCrLf & _
                        '                        " AND    ITS.ISETestID = " & pISETestID & vbCrLf & _
                        '                        " AND    MR.LanguageID = '" & myGlobalBase.GetSessionInfo.ApplicationLanguage.Trim & "' " & vbCrLf

                        'If (pSampleType <> "") Then cmdText &= " AND ITS.SampleType = '" & pSampleType & "' " & vbCrLf
                        'cmdText &= " ORDER BY MD.Position " & vbCrLf
                        Dim cmdText As String
                        cmdText = " SELECT ITS.*, ITS.SampleType + '-' + MR.ResourceText AS SampleTypeDesc, MD.Position, IT.Name AS ISETestName, IT.ShortName as ISETestShortName, IT.Units as MeasureUnit " & vbCrLf & _
                        " FROM   tparISETestSamples ITS INNER JOIN tcfgMasterData MD ON ITS.SampleType = MD.ItemID " & vbCrLf & _
                                                      " INNER JOIN tfmwMultiLanguageResources MR ON MD.ResourceID = MR.ResourceID " & vbCrLf & _
                                                      " INNER JOIN tparISETests IT ON ITS.ISETestID =  IT.ISETestID " & vbCrLf & _
                        " WHERE  MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString & "' " & vbCrLf & _
                        " AND    ITS.ISETestID = " & pISETestID & vbCrLf & _
                        " AND    MR.LanguageID = '" & myGlobalBase.GetSessionInfo.ApplicationLanguage.Trim & "' " & vbCrLf
                        If (pSampleType <> "") Then cmdText &= " AND ITS.SampleType = '" & pSampleType & "' " & vbCrLf

                        cmdText &= " UNION " & vbCrLf

                        cmdText &= " SELECT ITS.*, ITS.SampleType + '-' + MD.FixedItemDesc AS SampleTypeDesc, MD.Position, IT.Name AS ISETestName, IT.ShortName as ISETestShortName, IT.Units as MeasureUnit  " & vbCrLf & _
                        " FROM   tparISETestSamples ITS INNER JOIN tcfgMasterData MD ON ITS.SampleType = MD.ItemID " & vbCrLf & _
                                                      " INNER JOIN tparISETests IT ON ITS.ISETestID =  IT.ISETestID " & vbCrLf & _
                        " WHERE  MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString & "' " & vbCrLf & _
                        " AND    ITS.ISETestID = " & pISETestID & vbCrLf & _
                        " AND    MD.MultiLanguageFlag = 0 " & vbCrLf
                        If (pSampleType <> "") Then cmdText &= " AND ITS.SampleType = '" & pSampleType & "' " & vbCrLf

                        cmdText &= " ORDER BY Position " & vbCrLf
                        ' XB  04/06/2013

                        Dim myDS As New ISETestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myDS.tparISETestSamples)
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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparISETestSamplesDAO.GetByISETestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Returns selected ISE info to show in a Report
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="SelectedISE">List of selected ise tests</param>
        ''' <remarks>
        ''' Created by: DL 27/06/2012
        ''' </remarks>
        Public Function GetISEForReport(ByVal pDBConnection As SqlClient.SqlConnection, _
                                        ByVal AppLang As String, _
                                        Optional ByVal SelectedISE As List(Of String) = Nothing) As GlobalDataTO

            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)

                    If (Not dbConnection Is Nothing) Then
                        Dim StrSelectedISE As String = String.Empty

                        If Not SelectedISE Is Nothing AndAlso SelectedISE.Count > 0 Then
                            Dim ISETests As New StringBuilder()
                            For Each p As String In SelectedISE
                                ISETests.AppendFormat("'{0}', ", p)
                            Next

                            StrSelectedISE = String.Format(" WHERE ISETestID IN ({0})", _
                                                                ISETests.ToString().Substring(0, ISETests.Length - 2))
                        End If

                        Dim cmdText As String = " SELECT IST.ISETestID, IT.Name as ISETestName, IST.SampleType, MD.FixedItemDesc as MeasureUnit " & vbCrLf & _
                                                " FROM tparISETestSamples IST inner join tparISETests IT on IST.ISETestID = IT.ISETestID " & vbCrLf & _
                                                                            " inner join tcfgMasterData MD ON MD.ItemID = IT.ISE_Units " & vbCrLf
                        '                      " WHERE  MD.SubTableID = '" & GlobalEnumerates.MasterDataEnum.SAMPLE_TYPES.ToString & "' " & vbCrLf & _
                        '                       " AND    ITS.ISETestID = " & pISETestID & vbCrLf & _
                        '                        " AND    MR.LanguageID = '" & myGlobalBase.GetSessionInfo.ApplicationLanguage.Trim & "' " & vbCrLf

                        'If (pSampleType <> "") Then cmdText &= " AND ITS.SampleType = '" & pSampleType & "' " & vbCrLf
                        'cmdText &= " ORDER BY MD.Position " & vbCrLf



                        Using dbCmd As New SqlCommand(cmdText, dbConnection)
                            'Fill the DataSet to return 
                            Dim ISEData As New ISETestSamplesDS
                            Using dbDataAdapter As New SqlClient.SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(ISEData.tparISETestSamples)
                                resultData.SetDatos = ISEData
                            End Using
                        End Using
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparISETestSamplesDAO.GetISEForReport", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function

        ''' <summary>
        ''' For the specified ISE Test/SampleType, get all data needed to export it to QC Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pISETestID">ISE Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HistoryQCTestSamples with all data needed to export the ISE Test/SampleType to QC Module</returns>
        ''' <remarks>
        ''' Created by:  SA 21/05/2012
        ''' </remarks>
        Public Function GetDefinitionForQCModule(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pISETestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT IT.ISETestID AS TestID, IT.Name AS TestName, IT.ShortName AS TestShortName, 1 AS PreloadedTest, " & vbCrLf & _
                                                       " IT.Units AS MeasureUnit, ITS.SampleType, ITS.Decimals AS DecimalsAllowed, ITS.RejectionCriteria, " & vbCrLf & _
                                                       " ITS.CalculationMode, ITS.NumberOfSeries " & vbCrLf & _
                                                " FROM   tparISETests IT INNER JOIN tparISETestSamples ITS ON IT.ISETestID = ITS.ISETestID " & vbCrLf & _
                                                " WHERE  ITS.ISETestID  = " & pISETestID.ToString & vbCrLf & _
                                                " AND    ITS.SampleType = '" & pSampleType & "' " & vbCrLf

                        Dim myQCTestSamplesDS As New HistoryTestSamplesDS()
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dataAdapter As New SqlDataAdapter(dbCmd)
                                dataAdapter.Fill(myQCTestSamplesDS.tqcHistoryTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myQCTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparISETestSamplesDAO.GetDefinitionForQCModule", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update QC values (CalculationMode, NumberOfSeries and RejectionCriteria) for an specific ISETest/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistoryTestSamplesDS">Typed Dataset HistoryTestSamplesDS containing the information to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 06/06/2012
        ''' </remarks>
        Public Function UpdateQCValues(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistoryTestSamplesDS As HistoryTestSamplesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString
                Else
                    Dim myRow As HistoryTestSamplesDS.tqcHistoryTestSamplesRow = pHistoryTestSamplesDS.tqcHistoryTestSamples.First
                    Dim cmdText As String = " UPDATE tparISETestSamples " & vbCrLf & _
                                            " SET    CalculationMode ='" & myRow.CalculationMode.Trim & "', " & vbCrLf & _
                                                  "  RejectionCriteria = " & ReplaceNumericString(myRow.RejectionCriteria) & ", " & vbCrLf

                    If (myRow.IsNumberOfSeriesNull) Then
                        cmdText &= " NumberOfSeries = NULL, " & vbCrLf
                    Else
                        cmdText &= " NumberOfSeries = " & myRow.NumberOfSeries & ", " & vbCrLf
                    End If

                    Dim myGlobalBase As New GlobalBase
                    cmdText &= " TS_User = N'" & myGlobalBase.GetSessionInfo.UserName.Trim.Replace("'", "''") & "', " & vbCrLf & _
                               " TS_DateTime = '" & Now.ToString("yyyyMMdd HH:mm:ss") & "' " & vbCrLf & _
                        " WHERE  ISETestID  = " & myRow.TestID.ToString & vbCrLf & _
                        " AND    SampleType = '" & myRow.SampleType.Trim & "' " & vbCrLf

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
                myLogAcciones.CreateLogActivity(ex.Message, "tparISETestSamplesDAO.UpdateQCValues", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update value of field NumberOfControls for the specified Test/SampleType according to the number
        ''' of Controls linked to it in table tparTestControls
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: RH 11/06/2012
        ''' </remarks>
        Public Function UpdateNumOfControls(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                            ByVal pSampleType As String) As GlobalDataTO

            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " UPDATE tparISETestSamples " & vbCrLf & _
                                            " SET    NumberOfControls = (SELECT COUNT(*) FROM tparTestControls " & vbCrLf & _
                                                                       " WHERE  TestID = " & pTestID & vbCrLf & _
                                                                       " AND    SampleType = '" & pSampleType & "' " & vbCrLf & _
                                                                       " AND    TestType = 'ISE') " & vbCrLf & _
                                            " WHERE  ISETestID     = " & pTestID & vbCrLf & _
                                            " AND    SampleType = '" & pSampleType & "' " & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = (resultData.AffectedRecords = 0)
                    End Using

                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "tparISETestSamplesDAO.UpdateNumOfControls", EventLogEntryType.Error, False)
            End Try

            Return resultData
        End Function

#End Region

    End Class

End Namespace
