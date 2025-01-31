Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global.AlarmEnumerates
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports System.Threading.Tasks

Namespace Biosystems.Ax00.BL

    Public MustInherit Class WSStatusRecalculatorForNotDeletedExecutions
        Protected myExecutionsDS As ExecutionsDS
        Protected reqElementsDS As WSOrderTestsForExecutionsDS
        Protected dbConnection As SqlConnection
        Protected AnalyzerID As String
        Protected WorkSessionID As String
        Protected myExecutionsDAO As twksWSExecutionsDAO

        Public Sub New(ByVal pMyExecutionsDAO As twksWSExecutionsDAO, ByVal pMyExecutionsDS As ExecutionsDS, ByVal pDbConnection As SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String)
            myExecutionsDS = pMyExecutionsDS
            dbConnection = pDbConnection
            AnalyzerID = pAnalyzerID
            WorkSessionID = pWorkSessionID
            myExecutionsDAO = pMyExecutionsDAO
        End Sub

        Public Function Recalculate() As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim noPOSElements As Integer
            Dim myWSOrderTests As New WSOrderTestsDelegate            
            Dim lstSampleClassExecutions As List(Of ExecutionsDS.twksWSExecutionsRow)
            Dim lstToPENDING As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514
            Dim lstToLOCKED As List(Of ExecutionsDS.twksWSExecutionsRow) 'AG 19/02/2014 - #1514


            'Get all BLANK Order Tests having Pending and/or Locked Executions
            lstSampleClassExecutions = GetExecutions()

            For Each element As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions
                'Get the list of Elements required for the Blank Order Test 
                resultData = myWSOrderTests.GetOrderTestsForExecutions(dbConnection, AnalyzerID, WorkSessionID, element.SampleClass, element.OrderTestID)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    reqElementsDS = DirectCast(resultData.SetDatos, WSOrderTestsForExecutionsDS)

                    'Verify if at least one of the required Elements is not positioned
                    noPOSElements = GetNumberElements()

                    'The Executions for the Blank Order Test will be marked as LOCKED if there are not positioned elements
                    Dim aux = FinalStatus(noPOSElements, element)
                    If (aux <> "EXIT") Then
                        element.ExecutionStatus = aux
                    Else
                        Exit For 
                    End If
                Else
                    Exit For
                End If
            Next

            'Finally, update the status of the Executions for each Blank OrderTest
            lstToPENDING = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "PENDING" Select a).ToList
            lstToLOCKED = (From a As ExecutionsDS.twksWSExecutionsRow In lstSampleClassExecutions Where a.ExecutionStatus = "LOCKED" Select a).ToList
            If (Not resultData.HasError) Then resultData = myExecutionsDAO.UpdateStatusByOTAndRerunNumber(dbConnection, lstToPENDING, lstToLOCKED)

            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Test and SampleType, verify if it is required or not
        ''' When required verify if it is locked or not:
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a boolean value: True if the Calibrator needed for the specified Test and 
        '''          SampleType is locked; otherwise, False</returns>
        ''' <remarks>
        ''' Created by:  AG 20/04/2012 - Based on VerifyLockedCalibrator
        ''' </remarks>
        Protected Function VerifyLockedBlank(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                ByVal pTestID As Integer, ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        Dim verifyBlankLocked As Boolean = False

                        '...BLANK, first verify it has to be executed in the current WorkSession
                        '(if a previous result was selected to be used it is not needed verify if the Blank is positioned)
                        Dim reqElemNoPos As Boolean = False
                        Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate
                        resultData = myWSOrderTestsDelegate.VerifyToSendFlag(dbConnection, pAnalyzerID, pWorkSessionID, "BLANK", _
                                                                             pTestID, pSampleType)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            verifyBlankLocked = Convert.ToBoolean(resultData.SetDatos)

                            '...If the Calibrator needed is Experimental and has to be executed,then verify if it is Locked
                            If (verifyBlankLocked) Then
                                Dim myExecutionsDAO As New twksWSExecutionsDAO
                                resultData = myExecutionsDAO.VerifyUnlockedExecution(dbConnection, pAnalyzerID, pWorkSessionID, "BLANK", _
                                                                                     pTestID, pSampleType)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    reqElemNoPos = (Not Convert.ToBoolean(resultData.SetDatos))
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then resultData.SetDatos = reqElemNoPos
                    End If
                End If

            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.VerifyLockedBlank", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the specified Test and SampleType, verify the type of the needed Calibrator and if it is  locked or not:
        ''' ** If CalibratorType = EXPERIMENTAL, verify if the Calibrator defined for the Test and SampleType is locked
        ''' ** If CalibratorType = FACTOR, returns the Calibrator is unlocked 
        ''' ** If CalibratorType = ALTERNATIVE:
        '''       If the Calibrator needed for the Test and the Alternative SampleType is EXPERIMENTAL, verify if it is locked 
        '''       If the Calibrator needed for the Test and the Alternative SampleType is FACTOR, returns that it is unlocked 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pAlternativeST">When the Calibrator needed for the specified Test and SampleType is an Alternative one,
        '''                              the Alternative SampleType is returned in this parameter. Optional parameter needed only
        '''                              when the verification is done to set the status of Executions for Patient Samples</param>
        ''' <returns>GlobalDataTO containing a boolean value: True if the Calibrator needed for the specified Test and 
        '''          SampleType is locked; otherwise, False</returns>
        ''' <remarks>
        ''' Created by:  SA 10/05/2010
        ''' Modified by: SA 01/09/2010 - Before verify if the needed Calibrator is positioned in the Analyzer Rotor, verify if it
        '''                              has to be executed in the active WS or if a previous result will be used; in case of reusing
        '''                              a result, it is not needed verifying if the Calibrator is positioned                            
        ''' </remarks>
        Protected Function VerifyLockedCalibrator(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, ByVal pAnalyzerID As String, _
                                                ByVal pTestID As Integer, ByVal pSampleType As String, Optional ByRef pAlternativeST As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        '...Verify the type of Calibrator required for the Test and SampleType
                        Dim sampleTypeToVerify As String = ""
                        Dim verifyCalibLocked As Boolean = False
                        Dim myTestCalibratorDelegate As New TestCalibratorsDelegate

                        resultData = myTestCalibratorDelegate.GetTestCalibratorData(dbConnection, pTestID, pSampleType)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myTestSampleCalibratorDS As TestSampleCalibratorDS
                            myTestSampleCalibratorDS = DirectCast(resultData.SetDatos, TestSampleCalibratorDS)

                            If (myTestSampleCalibratorDS.tparTestCalibrators.Rows.Count = 1) Then
                                verifyCalibLocked = (myTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorType = "EXPERIMENT")
                                sampleTypeToVerify = pSampleType
                            ElseIf (myTestSampleCalibratorDS.tparTestCalibrators.Rows.Count > 1) Then
                                'Calibrator is Alternative...verify if the SampleType Alternative needs an Experimental Calibrator
                                verifyCalibLocked = (myTestSampleCalibratorDS.tparTestCalibrators(0).CalibratorType = "EXPERIMENT")
                                sampleTypeToVerify = myTestSampleCalibratorDS.tparTestCalibrators(0).SampleType

                                'Set value of the parameter used to return the Alternative SampleType
                                pAlternativeST = myTestSampleCalibratorDS.tparTestCalibrators(0).SampleType
                            End If

                            '...If the Calibrator needed is Experimental, first verify it has to be executed in the current WorkSession
                            '(if a previous result was selected to be used it is not needed verify if the Calibrator is positioned)
                            Dim reqElemNoPos As Boolean = False
                            If (verifyCalibLocked) Then
                                Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate
                                resultData = myWSOrderTestsDelegate.VerifyToSendFlag(dbConnection, pAnalyzerID, pWorkSessionID, "CALIB", _
                                                                                     pTestID, sampleTypeToVerify)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    verifyCalibLocked = Convert.ToBoolean(resultData.SetDatos)

                                    '...If the Calibrator needed is Experimental and has to be executed,then verify if it is Locked
                                    If (verifyCalibLocked) Then
                                        Dim myExecutionsDAO As New twksWSExecutionsDAO
                                        resultData = myExecutionsDAO.VerifyUnlockedExecution(dbConnection, pAnalyzerID, pWorkSessionID, "CALIB", _
                                                                                             pTestID, sampleTypeToVerify)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            reqElemNoPos = (Not Convert.ToBoolean(resultData.SetDatos))
                                        End If
                                    End If
                                End If
                            End If

                            If (Not resultData.HasError) Then resultData.SetDatos = reqElemNoPos
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message + " ((" + ex.HResult.ToString + "))"

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "ExecutionsDelegate.VerifyLockedCalibrator", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function



        Protected MustOverride Function GetExecutions() As List(Of ExecutionsDS.twksWSExecutionsRow)
        Protected MustOverride Function GetNumberElements() As Integer
        Protected MustOverride Function FinalStatus(ByVal numElem As Integer, ByVal elem As ExecutionsDS.twksWSExecutionsRow) As String

    End Class
End Namespace
