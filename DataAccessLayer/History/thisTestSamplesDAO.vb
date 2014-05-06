Option Explicit On
Option Strict On

Imports System.Text
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports System.Data.SqlClient

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisTestSamplesDAO
        Inherits DAOBase
#Region "CRUD Methods"

        ''' <summary>
        ''' When a Test/SampleType is deleted in Parameters Programming Screen, if it exists in the corresponding table in Historics Module, then it 
        ''' is marked as closed by updating field ClosedTestSample = TRUE. If there is an open TestVersion it is also marked as closed by updating 
        ''' field ClosedTestVersion = TRUE         
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">Test Identifier in Historics Module</param>
        ''' <param name="pSampleType">Sample Type Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/04/2012
        ''' </remarks>
        Public Function CloseTestSampleAndTestVersion(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, _
                                                      ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    cmdText.AppendLine("  UPDATE thisTestSamples SET ClosedTestSample = 1, ClosedTestVersion = 1 ")
                    cmdText.AppendFormat(" WHERE HistTestID = {0} AND SampleType = '{1}' ", pHistTestID, pSampleType)

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.CloseTestSampleAndTestVersion", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set field ClosedTestVersion = TRUE for the version currently active for an specific Test/SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">Test Identifier in Historics Module</param>
        ''' <param name="pSampleType">Sample Type Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 10/04/2012
        ''' Modified by: AG 08/10/2012 - SampleType can be empty when this function is called from HIST_CloseTestVersion and SampleClass = 'BLANK'
        ''' </remarks>
        Public Function CloseTestVersion(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder

                    If pSampleType <> "" Then
                        cmdText.AppendLine("  UPDATE thisTestSamples SET ClosedTestVersion = 1 ")
                        cmdText.AppendFormat(" WHERE HistTestID = {0} AND SampleType = '{1}' AND ClosedTestVersion = 0 ", pHistTestID, pSampleType)
                    Else
                        cmdText.AppendLine("  UPDATE thisTestSamples SET ClosedTestVersion = 1 ")
                        cmdText.AppendFormat(" WHERE HistTestID = {0} AND ClosedTestVersion = 0 ", pHistTestID)
                    End If

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.CloseTestVersion", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When an experimental Calibrator is marked as closed in Historics Module, the active TestVersion of all Test/Samples using 
        ''' that Calibrator have to be also marked as closed
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistCalibratorID">Calibrator Identifier in Historics Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 14/03/2012
        ''' </remarks>
        Public Function CloseTestVersionByHistCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistCalibratorID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As New StringBuilder
                    cmdText.AppendLine("  UPDATE thisTestSamples SET ClosedTestVersion = 1 ")
                    cmdText.AppendFormat(" WHERE CalibratorType = 'EXPERIMENT' AND HistCalibratorID = {0} AND ClosedTestVersion = 0 ", pHistCalibratorID)

                    Using dbCmd As New SqlClient.SqlCommand(cmdText.ToString, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.CloseTestVersionByHistCalibratorID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add a list of Standard Tests/SampleTypes to the corresponding table in Historics Module
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisTestSamplesDS">Typed DS containing data of all Test/SampleType to add to Historics Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/03/2012
        ''' Modified by: SA 21/09/2012 - Changed the SQL to remove field AbsorbanceDilutionFactor and all containing VolumeSteps
        '''              SA 27/09/2012 - Changed the SQL to add new field CalibPointUsed 
        '''              SA 18/10/2012 - Changed the SQL to remove fields BlankAbsorbanceLimit and KineticLimit
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisTestSamplesDS As HisTestSamplesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String
                    Using dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection

                        For Each row As HisTestSamplesDS.thisTestSamplesRow In pHisTestSamplesDS.thisTestSamples.Rows
                            cmdText = " INSERT INTO thisTestSamples (HistTestID, SampleType, TestVersionNumber, TestVersionDateTime, TestID, TestName, PreloadedTest, " & vbCrLf & _
                                                                   " MeasureUnit, AnalysisMode, ReactionType, DecimalsAllowed, ReadingMode, FirstReadingCycle, MainWavelength, " & vbCrLf & _
                                                                   " BlankMode, SampleVolume, CalibratorType, HistReagent1ID, Reagent1Volume, TestLongName, SecondReadingCycle, " & vbCrLf & _
                                                                   " ReferenceWavelength, ProzoneRatio, ProzoneTime1, ProzoneTime2, PredilutionFactor, PredilutionMode, " & vbCrLf & _
                                                                   " DiluentSolution, PredilutedSampleVol, PredilutedDiluentVol, RedPostdilutionFactor, " & vbCrLf & _
                                                                   " RedPostSampleVolume, IncPostdilutionFactor, IncPostSampleVolume, LinearityLimit, DetectionLimit, " & vbCrLf & _
                                                                   " SlopeFactorA, SlopeFactorB, SubstrateDepletionValue, HistCalibratorID, CalibPointUsed, " & vbCrLf & _
                                                                   " CurveGrowthType, CurveType, CurveAxisXType, CurveAxisYType, CalibratorFactor, RedPostReagent1Volume, " & vbCrLf & _
                                                                   " IncPostReagent1Volume, HistReagent2ID, Reagent2Volume, RedPostReagent2Volume, IncPostReagent2Volume) " & vbCrLf & _
                                      " VALUES (" & row.HistTestID.ToString & ", '" & row.SampleType & "', " & vbCrLf & _
                                                    row.TestVersionNumber.ToString & ", " & " '" & row.TestVersionDateTime.ToString("yyyyMMdd") & "', " & vbCrLf & _
                                                    row.TestID & ", N'" & row.TestName.Replace("'", "''") & "', " & vbCrLf & (IIf(row.PreloadedTest, 1, 0).ToString()) & ", " & vbCrLf & _
                                             " '" & row.MeasureUnit & "', '" & row.AnalysisMode & "', '" & row.ReactionType & "', " & row.DecimalsAllowed & ", '" & row.ReadingMode & "', " & vbCrLf & _
                                                    ReplaceNumericString(row.FirstReadingCycle) & ", " & row.MainWavelength & ", '" & row.BlankMode & "', " & vbCrLf & _
                                                    ReplaceNumericString(row.SampleVolume) & ", " & "'" & row.CalibratorType & "', " & row.HistReagent1ID.ToString & ", " & vbCrLf & _
                                                    ReplaceNumericString(row.Reagent1Volume) & ", " & vbCrLf

                            If (row.IsTestLongNameNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " N'" & row.TestLongName.Replace("'", "''") & "', " & vbCrLf
                            End If
                            If (row.IsSecondReadingCycleNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.SecondReadingCycle) & ", " & vbCrLf
                            End If
                            If (row.IsReferenceWavelengthNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= row.ReferenceWavelength & ", " & vbCrLf
                            End If
                            If (row.IsProzoneRatioNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.ProzoneRatio) & ", " & vbCrLf
                            End If
                            If (row.IsProzoneTime1Null) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.ProzoneTime1) & ", " & vbCrLf
                            End If
                            If (row.IsProzoneTime2Null) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.ProzoneTime2) & ", " & vbCrLf
                            End If
                            If (row.IsPredilutionFactorNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.PredilutionFactor) & ", " & vbCrLf
                            End If
                            If (row.IsPredilutionModeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " '" & row.PredilutionMode & "', " & vbCrLf
                            End If
                            If (row.IsDiluentSolutionNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " '" & row.DiluentSolution & "', " & vbCrLf
                            End If
                            If (row.IsPredilutedSampleVolNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.PredilutedSampleVol) & ", " & vbCrLf
                            End If
                            If (row.IsPredilutedDiluentVolNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.PredilutedDiluentVol) & ", " & vbCrLf
                            End If
                            If (row.IsRedPostdilutionFactorNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.RedPostdilutionFactor) & ", " & vbCrLf
                            End If
                            If (row.IsRedPostSampleVolumeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.RedPostSampleVolume) & ", " & vbCrLf
                            End If
                            If (row.IsIncPostdilutionFactorNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.IncPostdilutionFactor) & ", " & vbCrLf
                            End If
                            If (row.IsIncPostSampleVolumeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.IncPostSampleVolume) & ", " & vbCrLf
                            End If
                            If (row.IsLinearityLimitNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.LinearityLimit) & ", " & vbCrLf
                            End If
                            If (row.IsDetectionLimitNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.DetectionLimit) & ", " & vbCrLf
                            End If
                            If (row.IsSlopeFactorANull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.SlopeFactorA) & ", " & vbCrLf
                            End If
                            If (row.IsSlopeFactorBNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.SlopeFactorB) & ", " & vbCrLf
                            End If
                            If (row.IsSubstrateDepletionValueNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.SubstrateDepletionValue) & ", " & vbCrLf
                            End If
                            If (row.IsHistCalibratorIDNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= row.HistCalibratorID.ToString & ", " & vbCrLf
                            End If
                            If (row.IsCalibPointUsedNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= row.CalibPointUsed.ToString & ", " & vbCrLf
                            End If
                            If (row.IsCurveGrowthTypeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " '" & row.CurveGrowthType & "', " & vbCrLf
                            End If
                            If (row.IsCurveTypeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " '" & row.CurveType & "', " & vbCrLf
                            End If
                            If (row.IsCurveAxisXTypeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " '" & row.CurveAxisXType & "', " & vbCrLf
                            End If
                            If (row.IsCurveAxisYTypeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= " '" & row.CurveAxisYType & "', " & vbCrLf
                            End If
                            If (row.IsCalibratorFactorNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.CalibratorFactor) & ", " & vbCrLf
                            End If
                            If (row.IsRedPostReagent1VolumeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.RedPostReagent1Volume) & ", " & vbCrLf
                            End If
                            If (row.IsIncPostReagent1VolumeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.IncPostReagent1Volume) & ", " & vbCrLf
                            End If
                            If (row.IsHistReagent2IDNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= row.HistReagent2ID.ToString & ", " & vbCrLf
                            End If
                            If (row.IsReagent2VolumeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.Reagent2Volume) & ", " & vbCrLf
                            End If
                            If (row.IsRedPostReagent2VolumeNull) Then
                                cmdText &= " NULL, " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.RedPostReagent2Volume) & ", " & vbCrLf
                            End If
                            If (row.IsIncPostReagent2VolumeNull) Then
                                cmdText &= " NULL) " & vbCrLf
                            Else
                                cmdText &= ReplaceNumericString(row.IncPostReagent2Volume) & ") " & vbCrLf
                            End If
                            
                            dbCmd.CommandText = cmdText.ToString()
                            resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                        Next
                    End Using

                    resultData.SetDatos = pHisTestSamplesDS
                    resultData.HasError = False
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified TestID / SampleType / TestVersionNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">STD Test Identifier in Historic Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pTestVersionNum">Test Version Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2013
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleType As String, _
                                           ByVal pTestVersionNum As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM thisTestSamples " & vbCrLf & _
                                            " WHERE  HistTestID        = " & pHistTestID.ToString & vbCrLf & _
                                            " AND    SampleType        = '" & pSampleType.Trim & "' " & vbCrLf & _
                                            " AND    TestVersionNumber = " & pTestVersionNum.ToString & vbCrLf

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.AffectedRecords = dbCmd.ExecuteNonQuery()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.Delete", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all versions or the open version saved in Historics Module for the specified HistTestID/SampleType 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">Test Identifier in Historic Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pNotClosedTestVersion">When TRUE, it indicates that only the not closed TestVersion (if any) will be returned
        '''                                     When FALSE, all TestVersions that exist for the not closed Test/SampleType are returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisTestSamplesDS with data of all versions/the open version saved in Historics 
        '''          Module for the specified HistTestID and SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA 20/09/2012
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleType As String, _
                             Optional ByVal pNotClosedTestVersion As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM thisTestSamples " & vbCrLf & _
                                                " WHERE  HistTestID       = " & pHistTestID.ToString & vbCrLf & _
                                                " AND    SampleType       = '" & pSampleType.Trim & "' " & vbCrLf & _
                                                " AND    ClosedTestSample = 0 " & vbCrLf

                        If (pNotClosedTestVersion) Then cmdText &= " AND ClosedTestVersion = 0 " & vbCrLf
                        cmdText &= " ORDER BY TestVersionNumber DESC " & vbCrLf

                        Dim myHisTestSamplesDS As New HisTestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisTestSamplesDS.thisTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myHisTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all versions or the open version saved in Historics Module for the specified HistCalibratorID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistCalibratorID">Calibrator Identifier in Parameters Programming Module</param>
        ''' <param name="pNotClosedTestVersion">When TRUE, it indicates that only the not closed TestVersion (if any) will be returned
        '''                                     When FALSE, all TestVersions that exist for the not closed Test/SampleType are returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisTestSamplesDS with data of all versions/the open version saved in 
        '''          Historics Module for the specified HistCalibratorID</returns>
        ''' <remarks>
        ''' Created by:  AG 09/10/2012 
        ''' </remarks>
        Public Function ReadByCalibratorID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistCalibratorID As Integer, _
                                           Optional ByVal pNotClosedTestVersion As Boolean = False) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM thisTestSamples " & vbCrLf & _
                                                " WHERE  HistCalibratorID           = " & pHistCalibratorID.ToString & vbCrLf & _
                                                " AND    ClosedTestSample = 0 " & vbCrLf

                        If (pNotClosedTestVersion) Then cmdText &= " AND ClosedTestVersion = 0 " & vbCrLf
                        cmdText &= " ORDER BY TestVersionNumber DESC " & vbCrLf

                        Dim myHisTestSamplesDS As New HisTestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisTestSamplesDS.thisTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myHisTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.ReadByCalibratorID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all versions or the open version saved in Historics Module for the specified TestID/SampleType 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier in Parameters Programming Module</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <param name="pNotClosedTestVersion">When TRUE, it indicates that only the not closed TestVersion (if any) will be returned
        '''                                     When FALSE, all TestVersions that exist for the not closed Test/SampleType are returned</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisTestSamplesDS with data of all versions/the open version saved in 
        '''          Historics Module for the specified TestID and SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA 01/03/2012
        ''' Modified by: SA 25/06/2012 - Added optional parameter to get only the not closed TestVersion when its value is TRUE
        '''              AG 10/10/2012 - Parameter pSampleType changed to optional; changed the SQL
        ''' </remarks>
        Public Function ReadByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                  Optional ByVal pSampleType As String = "", Optional ByVal pNotClosedTestVersion As Boolean = False) _
                                                  As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = ""
                        If (pNotClosedTestVersion) Then
                            'Search only the not closed Test Version
                            If (pSampleType <> String.Empty) Then
                                'SAMPLE TYPE INFORMED -> Used for Calibration changes
                                'First UNION query works always for Experimental Calibrators in Historic results
                                cmdText = " SELECT TS.* " & vbCrLf & _
                                          " FROM thisTestSamples TS INNER JOIN thisWSOrderTests OT ON TS.HistTestID = OT.HistTestID " & vbCrLf & _
                                                                                                " AND TS.TestVersionNumber = OT.TestVersionNumber " & vbCrLf & _
                                                                                                " AND OT.SampleClass = 'CALIB' " & vbCrLf & _
                                                                  " INNER JOIN thisWSResults R ON OT.HistOrderTestID = R.HistOrderTestID " & vbCrLf & _
                                                                                            " AND R.ClosedResult =0 " & vbCrLf & _
                                          " WHERE  TS.TestID     = " & pTestID.ToString & vbCrLf & _
                                          " AND    TS.SampleType = '" & pSampleType.Trim & "' " & vbCrLf

                                'Second UNION query works always for Theoretical Factors in Historic results
                                cmdText &= " UNION " & vbCrLf & _
                                           " SELECT TS.* FROM thisTestSamples TS " & vbCrLf & _
                                           " WHERE  TS.TestID           = " & pTestID.ToString & vbCrLf & _
                                           " AND    TS.SampleType       = '" & pSampleType.Trim & "' " & vbCrLf & _
                                           " AND    TS.ClosedTestSample = 0 " & vbCrLf & _
                                           " AND    TS.ClosedTestVersion = 0 " & vbCrLf & _
                                           " ORDER BY TS.TestVersionNumber DESC " & vbCrLf
                            Else
                                'SAMPLE TYPE NOT INFORMED -> Used for Blank changes
                                'NOTE: the code for Test deletion removes first the Calibration, so when the Blank the thisTestSamples is already closed
                                '      and the Blank results are never closed

                                'First UNION query works always for Experimental Calibrators in Historic results
                                cmdText = " SELECT TS.* " & vbCrLf & _
                                          " FROM   thisTestSamples TS INNER JOIN thisWSOrderTests OT ON  TS.HistTestID = OT.HistTestID " & vbCrLf & _
                                                                                                  " AND  TS.TestVersionNumber = OT.TestVersionNumber " & vbCrLf & _
                                                                                                  " AND (OT.SampleClass = 'BLANK' OR OT.SampleClass = 'CALIB') " & vbCrLf & _
                                                                    " INNER JOIN thisWSResults R ON OT.HistOrderTestID = R.HistOrderTestID " & vbCrLf & _
                                                                                              " AND R.ClosedResult = 0 " & vbCrLf & _
                                          " WHERE  TS.TestID = " & pTestID.ToString & vbCrLf

                                'Second UNION query works always for Theoretical Factors in Historic results
                                cmdText &= " UNION " & vbCrLf & _
                                           " SELECT TS.* FROM thisTestSamples TS " & vbCrLf & _
                                           " WHERE  TS.TestID            = " & pTestID.ToString & vbCrLf & _
                                           " AND    TS.ClosedTestSample  = 0 " & vbCrLf & _
                                           " AND    TS.ClosedTestVersion = 0 " & vbCrLf & _
                                           " ORDER BY TS.TestVersionNumber DESC " & vbCrLf
                            End If

                        Else
                            'Search all Test Versions, including the closed ones
                            cmdText = " SELECT * FROM thisTestSamples " & vbCrLf & _
                                      " WHERE  TestID           = " & pTestID.ToString & vbCrLf & _
                                      " AND    ClosedTestSample = 0 " & vbCrLf

                            If (pSampleType <> String.Empty) Then cmdText &= " AND SampleType = '" & pSampleType.Trim & "' " & vbCrLf
                            cmdText &= " ORDER BY TestVersionNumber DESC " & vbCrLf
                        End If

                        Dim myHisTestSamplesDS As New HisTestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisTestSamplesDS.thisTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myHisTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.ReadByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' When a Test/SampleType is updated in Parameters Programming Module, if it already exists in the corresponding table in Historics 
        ''' Module, then data is also updated in this module. This function is applied only when the updated fields do not generate a new 
        ''' Test Version (in this case, the new version is created after close the active one)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisTestSamplesDS">Typed DataSet HisTestSamplesDS containing all data to update for the Test/SampleType in 
        '''                                 Historics Module</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/04/2012 
        ''' Modified by: SA 21/09/2012 - Changed the SQL to remove field AbsorbanceDilutionFactor and all containing VolumeSteps
        '''              SA 27/09/2012 - Changed the SQL to remove from the update all fields that generate a new TestVersion when they are changed
        '''              SA 18/10/2012 - Changed the SQL to remove from the update fields KineticBlankLimit and BlankAbsorbanceLimit
        '''              SA 22/10/2012 - Changed the filter: it should be by HistTestID = row.HistTestID instead of by HistTestID = row.TestID 
        ''' </remarks>
        Public Function Update(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisTestSamplesDS As HisTestSamplesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = ""
                    For Each row As HisTestSamplesDS.thisTestSamplesRow In pHisTestSamplesDS.thisTestSamples.Rows
                        cmdText &= " UPDATE thisTestSamples " & vbCrLf & _
                                                " SET    TestName            = N'" & row.TestName.Replace("'", "''").Trim & "', " & vbCrLf & _
                                                       " MeasureUnit         = '" & row.MeasureUnit.Trim & "', " & vbCrLf & _
                                                       " DecimalsAllowed     = " & row.DecimalsAllowed.ToString & ", " & vbCrLf

                        If (row.IsTestLongNameNull) Then
                            cmdText &= " TestLongName = NULL, " & vbCrLf
                        Else
                            cmdText &= " TestLongName = '" & row.TestLongName.Replace("'", "''").Trim & "', " & vbCrLf
                        End If
                        If (row.IsProzoneRatioNull) Then
                            cmdText &= " ProzoneRatio = NULL, " & vbCrLf
                        Else
                            cmdText &= " ProzoneRatio = " & ReplaceNumericString(row.ProzoneRatio) & ", " & vbCrLf
                        End If
                        If (row.IsProzoneTime1Null) Then
                            cmdText &= " ProzoneTime1 = NULL, " & vbCrLf
                        Else
                            cmdText &= " ProzoneTime1 = " & ReplaceNumericString(row.ProzoneTime1) & ", " & vbCrLf
                        End If
                        If (row.IsProzoneTime2Null) Then
                            cmdText &= " ProzoneTime2 = NULL, " & vbCrLf
                        Else
                            cmdText &= " ProzoneTime2 = " & ReplaceNumericString(row.ProzoneTime2) & ", " & vbCrLf
                        End If
                        If (row.IsLinearityLimitNull) Then
                            cmdText &= " LinearityLimit = NULL, " & vbCrLf
                        Else
                            cmdText &= " LinearityLimit = " & ReplaceNumericString(row.LinearityLimit) & ", " & vbCrLf
                        End If
                        If (row.IsDetectionLimitNull) Then
                            cmdText &= " DetectionLimit = NULL, " & vbCrLf
                        Else
                            cmdText &= " DetectionLimit = " & ReplaceNumericString(row.DetectionLimit) & ", " & vbCrLf
                        End If
                        If (row.IsSlopeFactorANull) Then
                            cmdText &= " SlopeFactorA = NULL, " & vbCrLf
                        Else
                            cmdText &= " SlopeFactorA = " & ReplaceNumericString(row.SlopeFactorA) & ", " & vbCrLf
                        End If
                        If (row.IsSlopeFactorBNull) Then
                            cmdText &= " SlopeFactorB = NULL, " & vbCrLf
                        Else
                            cmdText &= " SlopeFactorB = " & ReplaceNumericString(row.SlopeFactorB) & ", " & vbCrLf
                        End If
                        If (row.IsSubstrateDepletionValueNull) Then
                            cmdText &= " SubstrateDepletionValue = NULL " & vbCrLf
                        Else
                            cmdText &= " SubstrateDepletionValue = " & ReplaceNumericString(row.SubstrateDepletionValue) & vbCrLf
                        End If
                        cmdText &= " WHERE  HistTestID        = " & row.HistTestID.ToString & vbCrLf & _
                                   " AND    SampleType        = '" & row.SampleType & "' " & vbCrLf & _
                                   " AND    TestVersionNumber = " & row.TestVersionNumber & vbCrLf
                        cmdText &= vbNewLine
                    Next

                    Using dbCmd As New SqlClient.SqlCommand(cmdText, pDBConnection)
                        resultData.SetDatos = dbCmd.ExecuteScalar()
                        resultData.HasError = False
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.Update", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

#End Region

#Region "Other Methods"
        ''' <summary>
        ''' Get closed all STD Tests / Sample Types saved in Historic Module 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisTestSamplesDS with all closed STD Tests / SampleTypes in
        '''          Historic Module</returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2013
        ''' </remarks>
        Public Function GetClosedSTDTests(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT * FROM thisTestSamples " & vbCrLf & _
                                                " WHERE  ClosedTestSample = 1 OR ClosedTestVersion = 1 "

                        Dim myHisTestSamplesDS As New HisTestSamplesDS
                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Using dbDataAdapter As New SqlDataAdapter(dbCmd)
                                dbDataAdapter.Fill(myHisTestSamplesDS.thisTestSamples)
                            End Using
                        End Using

                        resultData.SetDatos = myHisTestSamplesDS
                        resultData.HasError = False
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.GetClosedSTDTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Generate the next HistTestID to assign to a new Test/SampleType moved to Historics Module 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing an integer value with the HistTestID to assign to a new Test/SampleType 
        '''          moved to Historics Module</returns>
        ''' <remarks>
        ''' Created by:  SA 01/03/2012
        ''' </remarks>
        Public Function GetNextHistTestID(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT MAX(HistTestID) AS NextHistTestID " & vbCrLf & _
                                                " FROM   thisTestSamples " & vbCrLf

                        Using dbCmd As New SqlClient.SqlCommand(cmdText, dbConnection)
                            Dim dbDataReader As SqlClient.SqlDataReader
                            dbDataReader = dbCmd.ExecuteReader()

                            If (dbDataReader.HasRows) Then
                                dbDataReader.Read()
                                If (dbDataReader.IsDBNull(0)) Then
                                    resultData.SetDatos = 1
                                Else
                                    resultData.SetDatos = CInt(dbDataReader.Item("NextHistTestID")) + 1
                                End If
                            End If
                            dbDataReader.Close()
                        End Using
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.GetNextHistTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if the informed Reagent is used by other Tests in Historics Module besides the informed Test 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistReagentID">Reagent Identifier in Historic Module</param>
        ''' <param name="pHistTestID">Test Identifier in Historic Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing an Integer value with the number of Tests using the specified Reagent</returns>
        ''' <remarks>
        ''' Created by:  SA    27/03/2012
        ''' Modified by: AG/SA 11/10/2012 - Changed the query
        ''' </remarks>
        Public Function VerifyReagentInUse(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistReagentID As Integer, ByVal pHistTestID As Integer, _
                                           ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim cmdText As String = " SELECT COUNT(*) FROM thisTestSamples " & vbCrLf & _
                                                " WHERE (HistReagent1ID = " & pHistReagentID.ToString & vbCrLf & _
                                                " OR    (HistReagent2ID IS NOT NULL AND HistReagent2ID = " & pHistReagentID.ToString & ")) " & vbCrLf & _
                                                " AND   HistTestID <> " & pHistTestID.ToString & vbCrLf & _
                                                " AND   ClosedTestVersion = 0 "

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

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestSamplesDAO.VerifyReagentInUse", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace