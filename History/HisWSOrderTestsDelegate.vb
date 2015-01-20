Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Types

Namespace Biosystems.Ax00.BL
    Public Class HisWSOrderTestsDelegate

#Region "Public Methods"
        ''' <summary>
        ''' After a result has been exported from Historical Results, the fields used for upload to LIS are cleared (to free database space)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pLISMessageID">LIS Message Identifier</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: DL 24/04/2013
        ''' </remarks>
        Public Function ClearIdentifiersForLIS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pLISMessageID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSOrderTestsDAO
                        resultData = myDAO.ClearIdentifiersForLIS(dbConnection, pLISMessageID)

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.ClearIdentifiersForLIS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Count the number of HistOrderTestIDs that exist in Historic Module for the specified TestID / SampleType / TestVersionNumber, to known 
        ''' if it can be deleted from thisTestSamples (if there are nor HistOrderTests for it, then it can be deleted)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">STD Test Identifier in Historic Module</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pTestVersionNum">Test Version Number</param>
        ''' <returns>GlobalDataTO containing an integer value with the number of HistOrderTestIDs that exist in Historic Module for 
        '''          the specified TestID / SampleType / TestVersionNumber</returns>
        ''' <remarks>
        ''' Created by:  SG 01/07/2013
        ''' </remarks>
        Public Function CountForSTDTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleType As String, _
                                        ByVal pTestVersionNum As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisTestSampleDAO As New thisWSOrderTestsDAO
                        resultData = myHisTestSampleDAO.CountForSTDTest(dbConnection, pHistTestID, pSampleType, pTestVersionNum)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.CountForSTDTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add to Historic Module all Order Tests having accepted and validated Results in the active Analyzer WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisWSOrderTestsDS">Typed DataSet HisWSOrderTestsDS containing the list of Order Tests to add 
        '''                                  to Historic Module</param>
        ''' <returns>GlobalDataTO containing the same entry DS but updated with the HistOrderTestID generated for each
        '''          added Order Test</returns>
        ''' <remarks>
        ''' Created by: TR 19/06/2012 
        ''' </remarks>
        Public Function Create(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisWSOrderTestsDS As HisWSOrderTestsDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisWSOrderTestsDAO As New thisWSOrderTestsDAO
                        myGlobalDataTO = myHisWSOrderTestsDAO.Create(dbConnection, pHisWSOrderTestsDS)

                        If (Not myGlobalDataTO.HasError) Then
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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.Create", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Delete all closed master data in Historic Module that is not in use anymore (all results using these master
        ''' data have been deleted)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 02/07/2013
        ''' </remarks>
        Public Function DeleteNotInUseMasterData(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete all NOT IN USE closed Calculated Tests
                        Dim histCalcTestsDelegate As New HisCalculatedTestsDelegate
                        resultData = histCalcTestsDelegate.DeleteClosedNotInUse(dbConnection)

                        If (Not resultData.HasError) Then
                            'Delete all NOT IN USE closed Off System Tests
                            Dim hisOffSytemTestsDelegate As New HisOFFSTestSamplesDelegate
                            resultData = hisOffSytemTestsDelegate.DeleteClosedNotInUse(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Delete all NOT IN USE closed Patients
                            Dim hisPatientsDelegate As New HisPatientsDelegate
                            resultData = hisPatientsDelegate.DeleteClosedNotInUse(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'Get all closed Standard Test / Sample Types
                            Dim hisTestSamplesDelegate As New HisTestSamplesDelegate
                            resultData = hisTestSamplesDelegate.GetClosedSTDTests(dbConnection)

                            If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                Dim myHisTestSamplesDS As HisTestSamplesDS = DirectCast(resultData.SetDatos, HisTestSamplesDS)

                                Dim numHistOTs As Integer = 0
                                Dim myHisWSOTDAO As New thisWSOrderTestsDAO
                                Dim histTestCalibValuesDelegate As New HisTestCalibratorsValuesDelegate

                                For Each row As HisTestSamplesDS.thisTestSamplesRow In myHisTestSamplesDS.thisTestSamples
                                    'Verify if there are HistOrderTestsIDs for the HistTestID, Sample Type and Test Version Number 
                                    resultData = myHisWSOTDAO.CountForSTDTest(dbConnection, row.HistTestID, row.SampleType, row.TestVersionNumber)
                                    If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                        numHistOTs = CType(resultData.SetDatos, Integer)

                                        If (numHistOTs = 0) Then
                                            'Delete all Calibrator Values defined for the Standard Test / Sample Type (if it uses an Experimental Calibrator)
                                            resultData = histTestCalibValuesDelegate.DeleteByHistTestID(dbConnection, row.HistTestID, row.SampleType, row.TestVersionNumber)
                                            If (resultData.HasError) Then Exit For

                                            'Delete the NOT IN USE closed Standard Test
                                            resultData = hisTestSamplesDelegate.Delete(dbConnection, row.HistTestID, row.SampleType, row.TestVersionNumber)
                                            If (resultData.HasError) Then Exit For
                                        End If
                                    Else
                                        Exit For
                                    End If
                                Next

                                If (Not resultData.HasError) Then
                                    'Delete all NOT IN USE Reagents
                                    Dim hisReagentsDelegate As New HisReagentsDelegate
                                    resultData = hisReagentsDelegate.DeleteClosedNotInUse(dbConnection)
                                End If

                                If (Not resultData.HasError) Then
                                    'Delete all NOT IN USE Calibrators
                                    Dim hisCalibratorsDelegate As New HisCalibratorsDelegate
                                    resultData = hisCalibratorsDelegate.DeleteClosedNotInUse(dbConnection)
                                End If
                            End If
                        End If

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.DeleteNotInUseMasterData", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get basic data of the Historical Patient Order Tests selected to be Export to LIS. Function created for BT #1453 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistOrderTestId">The list of HistOrderTestId to be exported</param>
        ''' <returns>GlobalDataTO containing a typed DataSet ExecutionsDS with basic data of the Historical Patient Order Tests to Export to LIS</returns>
        ''' <remarks>
        ''' Created by:  SA 16/01/2014
        ''' </remarks>
        Public Function GetDataToExportFromHIST(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As List(Of Integer)) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSOrderTestsDAO
                        resultData = myDAO.GetDataToExportFromHIST(dbConnection, pHistOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.GetDataToExportFromHIST", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get all Historic data saved for the specified HistOrderTestID/AnalyzerID/WorkSessionID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pHistWSID">Work Session Identifier in Historic Module</param>
        ''' <param name="pHistOTID">Order Test Identifier in Historic Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSOrderTestsDS with all data of the specified
        '''          Order Test in Historic Module</returns>
        ''' <remarks>
        ''' Created by:  SA 19/10/2012
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                             ByVal pHistWSID As String, ByVal pHistOTID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSOrderTestsDAO
                        resultData = myDAO.Read(dbConnection, pAnalyzerID, pHistWSID, pHistOTID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Read all Order Tests in Historic Module for the specified Analyzer (for all Work Sessions)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSOrderTestsDS with all HistOrderTestIDs for the specified Analyzer</returns>
        ''' <remarks>
        ''' Created by: SG 25/04/2013
        ''' </remarks>
        Public Function ReadAll(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSOrderTestsDAO
                        resultData = myDAO.ReadAll(dbConnection, pAnalyzerID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.ReadAll", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get Order Tests of the specified SampleClass that exist in Historic Module for the informed HistTestID and optionally, SampleType
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistTestID">Test Identifier in Historic Module</param>
        ''' <param name="pSampleClass">Sample Class code</param>
        ''' <param name="pSampleType">Sample Type code. Optional parameter, informed only when SampleClass is not BLANK</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisWSOrderTestsDS with the list of Order Tests</returns>
        ''' <remarks>
        ''' Created by:  SG 25/04/2013
        ''' </remarks>
        Public Function ReadByHistTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistTestID As Integer, ByVal pSampleClass As String, _
                                         Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSOrderTestsDAO
                        resultData = myDAO.ReadByHistTestID(dbConnection, pHistTestID, pSampleClass, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.ReadByHistTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For all Calibrators and Patient Results of an specific STD Test (all SampleTypes) executed in the informed Analyzer WorkSession,
        ''' update WorkSession/HistOrderTestID of the Blank used to calculate them (which can be from the same WorkSession or from a previous one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Identifier of the current WorkSession in Historic Module</param>
        ''' <param name="pHistOTBlankRow">Row of a typed DataSet HistWSOrderTestsDS containing data of the used Blank Order Test</param>
        ''' <param name="pIgnoreTestVersion">Optional parameter. When TRUE, it indicates fields of Blank used are updated for the affected 
        '''                                  Blank and Calibrator results although the TestVersionNumber is different (due to it is possible
        '''                                  having Blanks from previous versions still open when the version change was due to modifications
        '''                                  in Calibrator values)</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 23/10/2012
        ''' </remarks>
        Public Function UpdateBLANKFields(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
                                          ByVal pWorkSessionID As String, ByVal pHistOTBlankRow As HisWSOrderTestsDS.thisWSOrderTestsRow, _
                                          Optional ByVal pIgnoreTestVersion As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisWSOrderTestsDAO As New thisWSOrderTestsDAO
                        myGlobalDataTO = myHisWSOrderTestsDAO.UpdateBLANKFields(dbConnection, pAnalyzerID, pWorkSessionID, pHistOTBlankRow, pIgnoreTestVersion)

                        If (Not myGlobalDataTO.HasError) Then
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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.UpdateBLANKFields", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For all Patient Results of an specific STD Test/SampleType executed in the informed Analyzer WorkSession, update 
        ''' WorkSession/HistOrderTestID of the Calibrator used to calculate them (which can be from the same WorkSession or from a 
        ''' previous one)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pWorkSessionID">Identifier of the current WorkSession in Historic Module</param>
        ''' <param name="pHistOTCalibRow">Row of a typed DataSet HistWSOrderTestsDS containing data of the used Calibrator Order Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 23/10/2012
        ''' </remarks>
        Public Function UpdateCALIBFields(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, _
                                          ByVal pHistOTCalibRow As HisWSOrderTestsDS.thisWSOrderTestsRow) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myHisWSOrderTestsDAO As New thisWSOrderTestsDAO
                        myGlobalDataTO = myHisWSOrderTestsDAO.UpdateCALIBFields(dbConnection, pAnalyzerID, pWorkSessionID, pHistOTCalibRow)

                        If (Not myGlobalDataTO.HasError) Then
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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.UpdateCALIBFields", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Get TestSamples data information of the specified OrderHistTestID
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHistOrderTestID">Order Test Identifier in Historic Module</param>
        ''' <returns>GlobalDataTO containing a typed DataSet HisTestSamplesDS with the related data information</returns>
        ''' <remarks>
        ''' Created by XB 30/07/2014 - BT #1863
        ''' </remarks>
        Public Function ReadByOrderTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHistOrderTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New thisWSOrderTestsDAO
                        resultData = myDAO.ReadByOrderTestID(dbConnection, pHistOrderTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                'Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.ReadByOrderTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region

#Region "NOT USED"
        ' ''' <summary>
        ' ''' Count the number of HistOrderTestIDs that exist in Historic Module for the specified Analyzer Work Session, to known if it can be deleted
        ' ''' (if the Analyzer Work Session is empty - all its HistOrderTests have been deleted - it can be deleted)
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">Work Session Identifier in Historic Module</param>
        ' ''' <returns>GlobalDataTO containing an integer value with the number of HistOrderTestIDs that exist in Historic Module for 
        ' '''          the specified Analyzer Work Session</returns>
        ' ''' <remarks>
        ' ''' Created by:  SG 01/07/2013
        ' ''' </remarks>
        'Public Function CountByAnalyzerWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myHisTestSampleDAO As New thisWSOrderTestsDAO
        '                resultData = myHisTestSampleDAO.CountByAnalyzerWS(dbConnection, pAnalyzerID, pWorkSessionID)
        '            End If
        '        End If
        '    Catch ex As Exception
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.CountByAnalyzerWS", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' Receive a group of AnalyzerID/WorkSessionID/HistOrderTestID selected to be deleted in Historic Module and process each one of them.
        ' ''' Once the delete has finished, delete also all Adjust Base Lines that are not needed anymore and, if paramete pDeleteMasterData is TRUE,
        ' ''' delete also all closed Master Data (tests and patients) that are not needed anymore
        ' ''' </summary>
        ' ''' <param name="pHistWSOrderTestsDS">Typed DataSet HisWSOrderTestsDS containing the AnalyzerID/WorkSessionID/HistOrderTestID of 
        ' '''                                   all results selected to be deleted</param>
        ' ''' <param name="pDeleteMasterData">Flag indicating if closed master data that is not in use anymore after the deletion have to be also deleted.
        ' '''                                 This parameter is set to TRUE for manual deleting, an to FALSE for automatic deleting</param>
        ' ''' <returns>GlobalDataTO containing an integer value indicating if there are selected results that could not be deleted</returns>
        ' ''' <remarks>
        ' ''' Created by: SA 02/07/2013 
        ' ''' </remarks>
        'Public Function DeleteHistOrderTests(ByVal pHistWSOrderTestsDS As HisWSOrderTestsDS, ByVal pDeleteMasterData As Boolean) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        'Dim dbConnection As SqlClient.SqlConnection = Nothing

        'Try
        '    Dim numErrors As Integer = 0
        '    Debug.Print(DateTime.Now.ToString("H:mm:ss:fff") & " - start")
        '    For Each row As HisWSOrderTestsDS.thisWSOrderTestsRow In pHistWSOrderTestsDS.thisWSOrderTests
        '        'Delete all data for the AnalyzerID/WorkSessionID/HistOrderTestID in process (each row processed will have its own DB Transaction)
        '        resultData = DeleteResultData(Nothing, row.AnalyzerID, row.WorkSessionID, row.HistOrderTestID)
        '        If (resultData.HasError) Then numErrors += 1
        '    Next
        '    Debug.Print(DateTime.Now.ToString("H:mm:ss:fff") & " - end")

        '    'Get the list of different AnalyzerIDs in the entry DS
        '    Dim lstDiffAnalyzers As List(Of String) = (From a In pHistWSOrderTestsDS.thisWSOrderTests _
        '                                             Select a.AnalyzerID Distinct).ToList

        '    Dim hisAdjustBLDelegate As New HisAdjustBaseLinesDelegate
        '    For Each analyzerID As String In lstDiffAnalyzers
        '        'Delete all not in use Adjust Base Lines for the Analyzer (those that are not linked to any Execution) 
        '        resultData = hisAdjustBLDelegate.DeleteNotInUseAdjustBL(Nothing, analyzerID)
        '    Next
        '    lstDiffAnalyzers = Nothing

        '    If (pDeleteMasterData) Then
        '        'Delete all closed and NOT IN USE master data
        '        resultData = DeleteNotInUseMasterData(Nothing)
        '    End If

        '    resultData.SetDatos = numErrors
        'Catch ex As Exception
        '    resultData = New GlobalDataTO()
        '    resultData.HasError = True
        '    resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '    resultData.ErrorMessage = ex.Message

        '    'Dim myLogAcciones As New ApplicationLogManager()
        '    GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.DeleteHistOrderTests", EventLogEntryType.Error, False)
        'End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' 
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ' ''' <param name="pHistOrderTestID"></param>
        ' ''' <returns></returns>
        ' ''' <remarks>SG 02/07/2013</remarks>
        'Public Function DeleteResultData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, ByVal pWorkSessionID As String, ByVal pHistOrderTestID As Integer) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                'Delete all auxiliary result data (Readings and Executions) saved for the informed HistOrderTestID
        '                resultData = MyClass.DeleteAuxiliaryResultData(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestID)

        '                If Not resultData.HasError Then

        '                    'Delete all Curve Results saved for the informed HistOrderTestID (only when the HistOrderTestID corresponds to a multipoint Calibrator)
        '                    Dim myHisWSCurveResultsDelegate As New HisWSCurveResultsDelegate
        '                    resultData = myHisWSCurveResultsDelegate.DeleteByHistOrderTestID(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestID)
        '                    If Not resultData.HasError Then

        '                        'Delete all Results saved for the informed HistOrderTestID
        '                        Dim myHisWSResultsDelegate As New HisWSResultsDelegate
        '                        resultData = myHisWSResultsDelegate.DeleteByHistOrderTestID(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestID)
        '                        If Not resultData.HasError Then

        '                            'For Calculated Tests, delete the link between the informed HistOrderTestID and the HistOrderTestID of all Tests included in its formula
        '                            Dim myHisWSCalcTestsRelationsDelegate As New HisWSCalcTestsRelationsDelegate
        '                            resultData = myHisWSCalcTestsRelationsDelegate.DeleteByHistOrderTestIDCALC(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestID)
        '                            If Not resultData.HasError Then

        '                                'Get the HistOrderTestID of all Calculated Tests in which formula the informed HistOrderTestID is include
        '                                resultData = myHisWSCalcTestsRelationsDelegate.ReadByHistOrderTestID(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestID)
        '                                If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
        '                                    Dim myWSCalcRelations As HisWSCalcTestRelations = TryCast(resultData.SetDatos, HisWSCalcTestRelations)

        '                                    If myWSCalcRelations IsNot Nothing AndAlso myWSCalcRelations.thisWSCalcTestsRelations.Rows.Count > 0 Then

        '                                        'Delete all Historic Result data for the HistOrderTestIDCALC in process (recursive call)
        '                                        For Each R As HisWSCalcTestRelations.thisWSCalcTestsRelationsRow In myWSCalcRelations.thisWSCalcTestsRelations
        '                                            resultData = MyClass.DeleteResultData(dbConnection, pAnalyzerID, pWorkSessionID, myWSCalcRelations.thisWSCalcTestsRelations.First.HistOrderTestIDCALC)
        '                                        Next

        '                                    End If

        '                                    'Delete the informed HistOrderTestID
        '                                    Dim myHisWSOrderTestDAO As New thisWSOrderTestsDAO
        '                                    resultData = myHisWSOrderTestDAO.Delete(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestID)
        '                                    If Not resultData.HasError Then
        '                                        'Verify if the Analyzer WorkSession has other HistOrderTestsID
        '                                        resultData = myHisWSOrderTestDAO.CountByAnalyzerWS(dbConnection, pAnalyzerID, pWorkSessionID)
        '                                        If Not resultData.HasError AndAlso resultData.SetDatos IsNot Nothing Then
        '                                            Dim myNumLinkedHistOTs As Integer = CInt(resultData.SetDatos)
        '                                            If myNumLinkedHistOTs > 0 Then
        '                                                'Delete all Alarms for the Analyzer Work Session
        '                                                Dim myHisWSAnalyzerAlarmsDelegate As New HisWSAnalyzerAlarmsDelegate
        '                                                resultData = myHisWSAnalyzerAlarmsDelegate.DeleteByAnalyzerWS(dbConnection, pAnalyzerID, pWorkSessionID)

        '                                                If Not resultData.HasError Then
        '                                                    'Delete the Analyzer Work Session
        '                                                    Dim myHisAnalyzerWorkSessionsDelegate As New HisAnalyzerWorkSessionsDelegate
        '                                                    resultData = myHisAnalyzerWorkSessionsDelegate.Delete(dbConnection, pAnalyzerID, pWorkSessionID)
        '                                                End If

        '                                            End If
        '                                        End If
        '                                    End If


        '                                End If
        '                            End If
        '                        End If
        '                    End If
        '                End If

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.DeleteResultData", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        ' ''' <summary>
        ' ''' 
        ' ''' </summary>
        ' ''' <param name="pDBConnection">Open DB Connection</param>
        ' ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ' ''' <param name="pWorkSessionID">WorkSession Identifier</param>
        ' ''' <param name="pHistOrderTestID"></param>
        ' ''' <returns></returns>
        ' ''' <remarks>SG 02/07/2013</remarks>
        'Public Function DeleteAuxiliaryResultData(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pAnalyzerID As String, _
        '                                          ByVal pWorkSessionID As String, ByVal pHistOrderTestID As Integer) As GlobalDataTO
        '    Dim resultData As GlobalDataTO = Nothing
        '    Dim dbConnection As SqlClient.SqlConnection = Nothing

        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
        '        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                Dim myHisWSReadingsDelegate As New HisWSReadingsDelegate
        '                resultData = myHisWSReadingsDelegate.DeleteByHistOrderTestID(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestID)
        '                If Not resultData.HasError Then
        '                    Dim myHisWSExecutionsDelegate As New HisWSExecutionsDelegate
        '                    resultData = myHisWSExecutionsDelegate.DeleteByHistOrderTestID(dbConnection, pAnalyzerID, pWorkSessionID, pHistOrderTestID)
        '                End If

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '        resultData = New GlobalDataTO()
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
        '        resultData.ErrorMessage = ex.Message

        '        'Dim myLogAcciones As New ApplicationLogManager()
        '        GlobalBase.CreateLogActivity(ex.Message, "HisWSOrderTestsDelegate.DeleteAuxiliaryResultData", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function
#End Region
    End Class
End Namespace
