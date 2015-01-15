Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL


    Public Class WSRequiredElemByOrderTestDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Add relations with needed Calibrator Elements for Patient and/or Control Order Tests when for the Test/Sample
        ''' the needed Calibrator is an Alternative
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/02/2011
        ''' Modified by: SA 12/04/2012 - New implementation
        ''' </remarks>
        Public Function AddElementsForAlternativeCalibrators(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all Order Tests corresponding to Alternative Calibrators requested in the WS
                        Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate

                        resultData = myWSOrderTestsDelegate.ReadAllAlternativeOTs(dbConnection, pWorkSessionID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myOrderTestsDS As OrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                            Dim myWSReqElementsDS As New WSRequiredElementsDS
                            Dim myWSReqElementDelegate As New WSRequiredElementsDelegate

                            Dim affectedOTsDS As New ViewWSOrderTestsDS
                            Dim myElemByOTLinkDS As New WSRequiredElemByOrderTestDS
                            Dim requiredElemByOrderTestData As New twksWSRequiredElemByOrderTestDAO

                            For Each alternativeOT As OrderTestsDS.twksOrderTestsRow In myOrderTestsDS.twksOrderTests
                                'Get all Patient Samples and Controls requested for the Test/SampleType in the active WorkSession
                                resultData = myWSOrderTestsDelegate.ReadPatientsAndCtrlsByTestIDAndSampleType(dbConnection, pWorkSessionID, alternativeOT.TestID, alternativeOT.SampleType)
                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    affectedOTsDS = DirectCast(resultData.SetDatos, ViewWSOrderTestsDS)

                                    If (affectedOTsDS.vwksWSOrderTests.Rows.Count > 0) Then
                                        'Get the list of ElementIDs for the Calibrator used as alternative (there is a required Element Identifier for each Calibrator point)
                                        resultData = myWSReqElementDelegate.GetElemIDForAlternativeCalibrator(dbConnection, pWorkSessionID, alternativeOT.AlternativeOrderTestID)
                                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                            myWSReqElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                                            'Verify if the link between each requested Patient Sample/Control OrderTest and each required Calibrator Element 
                                            'already exists, and create it when not
                                            For Each affectedOT As ViewWSOrderTestsDS.vwksWSOrderTestsRow In affectedOTsDS.vwksWSOrderTests
                                                For Each reqElementID As WSRequiredElementsDS.twksWSRequiredElementsRow In myWSReqElementsDS.twksWSRequiredElements
                                                    resultData = requiredElemByOrderTestData.Read(dbConnection, affectedOT.OrderTestID, reqElementID.ElementID)

                                                    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                                        myElemByOTLinkDS = DirectCast(resultData.SetDatos, WSRequiredElemByOrderTestDS)
                                                        If (myElemByOTLinkDS.twksWSRequiredElemByOrderTest.Rows.Count = 0) Then
                                                            'The link does not exist; create it
                                                            resultData = requiredElemByOrderTestData.Create(dbConnection, affectedOT.OrderTestID, reqElementID.ElementID, affectedOT.StatFlag)
                                                        End If
                                                    Else
                                                        'Error verifying if the link already exists
                                                        Exit For
                                                    End If
                                                Next
                                                If (resultData.HasError) Then Exit For
                                            Next
                                        End If
                                    End If
                                End If
                                If (resultData.HasError) Then Exit For
                            Next

                            'For Each alternativeOT As OrderTestsDS.twksOrderTestsRow In myOrderTestsDS.twksOrderTests
                            '    Dim myWSReqElementDelegate As New WSRequiredElementsDelegate

                            '    'Get the list of ElementIDs for each point defined for the Alternative Calibrator
                            '    resultData = myWSReqElementDelegate.GetElemIDForAlternativeCalibrator(dbConnection, pWorkSessionID, alternativeOT.AlternativeOrderTestID)
                            '    If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            '        Dim myWSReqElementsDS As WSRequiredElementsDS = DirectCast(resultData.SetDatos, WSRequiredElementsDS)

                            '        If (myWSReqElementsDS.twksWSRequiredElements.Rows.Count > 0) Then
                            '            'Insert relations for the Alternative Calibrator
                            '            Dim requiredElemByOrderTestData As New twksWSRequiredElemByOrderTestDAO
                            '            resultData = requiredElemByOrderTestData.InsertRelationsForAlternativeCalibrators(dbConnection, pWorkSessionID, myWSReqElementsDS, _
                            '                                                                                              alternativeOT.TestID, alternativeOT.SampleType)

                            '        End If
                            '    End If
                            '    If (resultData.HasError) Then Exit For
                            'Next
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.HasError = False
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            resultData.HasError = True
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRequiredElemByOrderTestDelegate.AddElementsForAlternativeCalibrators", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add relations with needed Calibrator Elements for Patient and/or Control Order Tests for Tests marked as Special (HBTotal and HbA1C)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by: SA 16/02/2011
        ''' </remarks>
        Public Function AddElementsForSpecialTestsCalibrators(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get all Order Tests corresponding to Special Tests requested in the WS
                        Dim myWSOrderTestsDelegate As New WSOrderTestsDelegate
                        Dim requiredElemByOrderTestData As New twksWSRequiredElemByOrderTestDAO

                        resultData = myWSOrderTestsDelegate.ReadSpecialTests(dbConnection, pWorkSessionID)
                        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                            Dim myOrderTestsDS As OrderTestsDS = DirectCast(resultData.SetDatos, OrderTestsDS)

                            Dim mySettingsDS As New SpecialTestsSettingsDS
                            Dim mySettingsDelegate As New SpecialTestsSettingsDelegate
                            For Each specialTest As OrderTestsDS.twksOrderTestsRow In myOrderTestsDS.twksOrderTests
                                'Search if the Special Test has information for Setting CAL_POINT_USED and get its value
                                resultData = mySettingsDelegate.Read(dbConnection, specialTest.TestID, specialTest.SampleType, _
                                                                     GlobalEnumerates.SpecialTestsSettings.CAL_POINT_USED.ToString)

                                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                    mySettingsDS = DirectCast(resultData.SetDatos, SpecialTestsSettingsDS)

                                    If (mySettingsDS.tfmwSpecialTestsSettings.Rows.Count = 0) Then
                                        'There is not value for the Setting, all Calibrator points are used for the Test/SampleType
                                        resultData = requiredElemByOrderTestData.InsertRelationsForCalibrators(dbConnection, pWorkSessionID, -1, _
                                                                                                               specialTest.TestID, specialTest.SampleType)
                                    Else
                                        'There is a value for the Setting, only the specified Calibrator point is used for the Test/SampleType
                                        resultData = requiredElemByOrderTestData.InsertRelationsForCalibrators(dbConnection, pWorkSessionID, _
                                                                                                               Convert.ToInt32(mySettingsDS.tfmwSpecialTestsSettings(0).SettingValue), _
                                                                                                               specialTest.TestID, specialTest.SampleType)
                                    End If
                                End If
                                If (resultData.HasError) Then Exit For
                            Next
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.HasError = False
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            resultData.HasError = True
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRequiredElemByOrderTestDelegate.AddElementsForSpecialTestsCalibrators", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add Required Elements for a list of Order Tests included in a Work Session 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/02/2011 - New implementation to improve the speed when create/update a WorkSession
        ''' Modified by: AG 21/06/2011 - Added call to function InsertRelationsForWashingSolutions to add all relations between requested 
        '''                              Order Tests and Washing Solutions required to avoid contaminations
        '''              TR 13/03/2012 - Removed call to InsertRelationsForISEWashingSolutions due to the ISE Washing Solution 
        '''                              is not required for execution of ISE Tests requested for Patients
        '''              SA 03/06/2014 - BT #1519 ==> Removed call to function InsertRelationsForWashingSolutions due to not positioned Washing 
        '''                                           Solutions should not block executions 
        ''' </remarks>
        Public Function AddOrderTestElements(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim requiredElemByOrderTestData As New twksWSRequiredElemByOrderTestDAO

                        'Insert relations for Special Solutions used for Blanks and as Diluents for patient samples
                        resultData = requiredElemByOrderTestData.InsertRelationsForSpecialSolutions(dbConnection, pWorkSessionID)

                        'Insert relations for Reagents
                        If (Not resultData.HasError) Then resultData = requiredElemByOrderTestData.InsertRelationsForReagents(dbConnection, pWorkSessionID)

                        'Insert relations for Experimental Calibrators (excluding the ones for Special Tests)
                        If (Not resultData.HasError) Then resultData = requiredElemByOrderTestData.InsertRelationsForCalibrators(dbConnection, pWorkSessionID)

                        'Insert relations for Alternative Calibrators
                        If (Not resultData.HasError) Then resultData = AddElementsForAlternativeCalibrators(dbConnection, pWorkSessionID)

                        'Insert relations for Calibrators of Tests defined as Special
                        If (Not resultData.HasError) Then resultData = AddElementsForSpecialTestsCalibrators(dbConnection, pWorkSessionID)

                        'Insert relations for Controls
                        If (Not resultData.HasError) Then resultData = requiredElemByOrderTestData.InsertRelationsForControls(dbConnection, pWorkSessionID)

                        'Insert relations for Patient Samples 
                        If (Not resultData.HasError) Then resultData = requiredElemByOrderTestData.InsertRelationsForPatientSamples(dbConnection, pWorkSessionID)

                        'BT #1519 - Do not create relations between Order Tests of CONTAMINANT Tests and WASHING SOLUTIONS
                        'Insert relations for Washing Solutions 
                        'If (Not resultData.HasError) Then resultData = requiredElemByOrderTestData.InsertRelationsForWashingSolutions(dbConnection, pWorkSessionID)

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.HasError = False
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            resultData.HasError = True
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRequiredElemByOrderTestDelegate.AddOrderTestsElementsNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the details of the specified Required Element 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Required Element Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with all data of the specified
        '''          Required Element</returns>
        ''' <remarks>
        ''' Created by:  DL 05/01/2011
        ''' </remarks>
        Public Function ReadOrderTestByElementID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim requiredElemByOrderTestData As New twksWSRequiredElemByOrderTestDAO
                        resultData = requiredElemByOrderTestData.ReadByElementID(dbConnection, pElementID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRequiredElemByOrderTestDelegate.ReadOrderTestByElementID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' For the informed ElementID, get all related OrderTests of the specified SampleClass
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pElementID">Element Identifier</param>
        ''' <param name="pSampleClass">Sample Class Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet WSRequiredElementsDS with all OrderTests of the specified SampleClass
        '''          related with the informed Element</returns>
        ''' <remarks>
        ''' Created by:  AG 04/04/2011 - Based on ReadOrderTestByElementID</remarks>
        Public Function ReadOrderTestByElementIDAndSampleClass(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pElementID As Integer, _
                                                               ByVal pSampleClass As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim requiredElemByOrderTestData As New twksWSRequiredElemByOrderTestDAO
                        resultData = requiredElemByOrderTestData.ReadByElementIDAndSampleClass(dbConnection, pElementID, pSampleClass)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRequiredElemByOrderTestDelegate.ReadOrderTestByElementIDAndSampleClass", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all information for the Work Session
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 21/04/2010
        ''' </remarks>
        Public Function ResetWS(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) AndAlso (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New twksWSRequiredElemByOrderTestDAO
                        resultData = myDAO.ResetWS(dbConnection, pWorkSessionID)

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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "WSRequiredElemByOrderTestDelegate.ResetWS", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace