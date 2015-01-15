Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.DAL.DAO
    Public Class thisTestCalibratorsValuesDAO
        Inherits DAOBase

#Region "CRUD Methods"
        ''' <summary>
        ''' For a Test/Sample Type using an Experimental Calibrator, export to Historic Module the value of all Calibrator Points
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">Test Identifier in Historic Module</param>
        ''' <param name="pSampleType">Sample Type code</param>
        ''' <param name="pTestVersionNum">Number of the open version of the Test/SampleType in Historic Module</param>
        ''' <param name="pHistCalibratorID">Calibrator Identifier in Historic Module</param>
        ''' <param name="pCalibrationValuesDS">Typed Dataset TestCalibratorValuesDS containing Calibration values for the specified
        '''                                    Test/SampleType and Experimental Calibrator</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 27/09/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleType As String, ByVal pTestVersionNum As Integer, _
                               ByVal pHistCalibratorID As Integer, ByVal pCalibrationValuesDS As TestCalibratorValuesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String
                    Using dbCmd As New SqlClient.SqlCommand
                        dbCmd.Connection = pDBConnection

                        For Each row As TestCalibratorValuesDS.tparTestCalibratorValuesRow In pCalibrationValuesDS.tparTestCalibratorValues
                            cmdText = " INSERT INTO thisTestCalibratorsValues (HistTestID, SampleType, TestVersionNumber, HistCalibratorID, " & vbCrLf & _
                                                                             " CalibratorNum, TheoreticalConcentration, KitConcentrationRelation) " & vbCrLf & _
                                      " VALUES (" & pHistTestID.ToString & ", '" & pSampleType & "', " & pTestVersionNum.ToString & ", " & vbCrLf & _
                                                    pHistCalibratorID.ToString & ", " & row.CalibratorNum.ToString & ", " & vbCrLf & _
                                                    ReplaceNumericString(row.TheoricalConcentration) & ", " & ReplaceNumericString(row.KitConcentrationRelation) & ") " & vbCrLf

                            dbCmd.CommandText = cmdText.ToString()
                            resultData.AffectedRecords += dbCmd.ExecuteNonQuery()
                        Next
                    End Using
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestCalibratorsValuesDAO.Create", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all Test Calibrator values saved in Historic Module for the specified TestID / SampleType / TestVersionNumber
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">STD Test Identifier in Historic Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pTestVersionNum">Test Version Number</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 01/07/2013
        ''' </remarks>
        Public Function DeleteByHistTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleType As String, _
                                           ByVal pTestVersionNum As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO

            Try
                If (pDBConnection Is Nothing) Then
                    resultData.HasError = True
                    resultData.ErrorCode = GlobalEnumerates.Messages.DB_CONNECTION_ERROR.ToString()
                Else
                    Dim cmdText As String = " DELETE FROM thisTestCalibratorsValues " & vbCrLf & _
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
                myLogAcciones.CreateLogActivity(ex.Message, "thisTestCalibratorsValuesDAO.DeleteByHistTestID", EventLogEntryType.Error, False)
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace
