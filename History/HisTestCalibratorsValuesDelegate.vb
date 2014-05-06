Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Types


Namespace Biosystems.Ax00.BL
    Public Class HisTestCalibratorsValuesDelegate

#Region "Public Methods"
        ''' <summary>
        ''' For a Test/Sample Type using an Experimental Calibrator, export to Historic Module the value of all Calibrator Points
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">Test Identifier in Historic Module</param>
        ''' <param name="pTestVersionNum">Number of the open version of the Test/SampleType in Historic Module</param>
        ''' <param name="pSampleType">Sample Type code</param>
        ''' <param name="pHistCalibratorID">Calibrator Identifier in Historic Module</param>
        ''' <param name="pCalibrationValuesDS">Typed Dataset TestCalibratorValuesDS containing Calibration values for the specified
        '''                                    Test/SampleType and Experimental Calibrator</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 27/09/2012
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleType As String, ByVal pTestVersionNum As Integer, _
                               ByVal pHistCalibratorID As Integer, ByVal pCalibrationValuesDS As TestCalibratorValuesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisTestCalibratorsValuesDAO
                        resultData = myDAO.Create(dbConnection, pHistTestID, pSampleType, pTestVersionNum, pHistCalibratorID, pCalibrationValuesDS)

                        If (Not resultData.HasError) Then
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisTestCalibratorsValuesDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
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
        ''' Created by:  SG 02/07/2013
        ''' </remarks>
        Public Function DeleteByHistTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleType As String, _
                                           ByVal pTestVersionNum As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisTestCalibratorsValuesDAO
                        resultData = myDAO.DeleteByHistTestID(dbConnection, pHistTestID, pSampleType, pTestVersionNum)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "HisTestCalibratorsValuesDelegate.DeleteByHistTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region
    End Class
End Namespace
