Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global

Namespace Biosystems.Ax00.BL.UpdateVersion
    Public Class UpdatePreloadedFactoryTestDelegate

#Region "FUNCTIONS FOR NEW UPDATE VERSION PROCESS (NEW AND UPDATED FUNCTIONS)"
        ''' <summary>
        ''' Execute the Upadate Version Process for STANDARD TESTS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 08/10/2014 - BA-1944
        ''' </remarks>
        Public Function SetFactorySTDTestsProgramming(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        '(1) Execute the Update Version Process for EXPERIMENTAL CALIBRATORS
                        Dim myCalibratorsUpdate As New CalibratorUpdateData
                        resultData = myCalibratorsUpdate.ProcessForCALIBRATORS(dbConnection, pUpdateVersionChangesList)

                        '(2) Execute the Update Version Process for DELETED STD TESTS
                        Dim mySTDTestsUpdate As New TestParametersUpdateData
                        If (Not resultData.HasError) Then
                            resultData = mySTDTestsUpdate.DELETERemovedSTDTests(dbConnection, pUpdateVersionChangesList)
                        End If

                        '(3) Execute the Update Version Process for DELETED STD TESTS / SAMPLE TYPES
                        If (Not resultData.HasError) Then
                            resultData = mySTDTestsUpdate.DELETERemovedSTDTestSamples(dbConnection, pUpdateVersionChangesList)
                        End If

                        '(4) Execute the Update Version Process for NEW STD TESTS
                        If (Not resultData.HasError) Then
                            resultData = mySTDTestsUpdate.CREATENewSTDTests(dbConnection, pUpdateVersionChangesList)
                        End If

                        '(5) Execute the Update Version Process for UPDATED STD TESTS
                        If (Not resultData.HasError) Then
                            resultData = mySTDTestsUpdate.UPDATEModifiedSTDTests(dbConnection, pUpdateVersionChangesList)
                        End If

                        '(6) Execute the Update Version Process for NEW STD TESTS / SAMPLE TYPES 
                        If (Not resultData.HasError) Then
                            resultData = mySTDTestsUpdate.CREATENewSamplesForSTDTests(dbConnection, pUpdateVersionChangesList)
                        End If

                        '(7) Execute the Update Version Process for UPDATED STD TESTS / SAMPLE TYPES 
                        If (Not resultData.HasError) Then
                            resultData = mySTDTestsUpdate.UPDATEModifiedSTDTestSamples(dbConnection, pUpdateVersionChangesList)
                        End If

                        '(8) Recalculate Test Position for Preloaded and User STD TESTS
                        If (Not resultData.HasError) Then
                            resultData = mySTDTestsUpdate.UpdateTestSortByTestName(pDBConnection)
                        End If

                        '(9) Execute the Update Version Process for CONTAMINATIONS
                        If (Not resultData.HasError) Then
                            Dim myContaminationsUpdate As New ContaminationsUpdateData
                            resultData = myContaminationsUpdate.ProcessCONTAMINATIONS(dbConnection, pUpdateVersionChangesList)
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

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryTestProgramming", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Execute the Upadate Version Process for ISE TESTS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 15/10/2014 - BA-1944
        ''' </remarks>
        Public Function SetFactoryISETestsProgramming(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myISETestUpdate As New ISETestUpdateData
                        resultData = myISETestUpdate.UPDATEModifiedISETestSamples(dbConnection, pUpdateVersionChangesList)

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

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryISETestsProgramming", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Execute the Upadate Version Process for CALCULATED TESTS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 16/10/2014 - BA-1944
        ''' </remarks>
        Public Function SetFactoryCALCTestsProgramming(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myCALCTestsUpdate As New CalculatedTestUpdateData

                        '(1) Execute the Update Version Process for DELETED CALC TESTS
                        resultData = myCALCTestsUpdate.DELETERemovedCALCTests(dbConnection, pUpdateVersionChangesList)

                        '(2) Execute the Update Version Process for UPDATED CALC TESTS
                        If (Not resultData.HasError) Then resultData = myCALCTestsUpdate.UPDATEModifiedCALCTests(dbConnection, pUpdateVersionChangesList)

                        '(3) Execute the Update Version Process for NEW CALC TESTS
                        If (Not resultData.HasError) Then resultData = myCALCTestsUpdate.CREATENewCALCTests(dbConnection, pUpdateVersionChangesList)

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

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryCALCTestsProgramming", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Execute the Upadate Version Process for OFF-SYSTEM TESTS
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCustomerSwVersion">SW Version in CUSTOMER DB</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 20/11/2014 - BA-2105
        ''' </remarks>
        Public Function SetFactoryOFFSTestsProgramming(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCustomerSwVersion As String, _
                                                       ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the SW Version in CUSTOMER DB to determine which update processes have to be executed: DELETE and UPDATE 
                        'can be executed only for SW Versions greater or equal to 3.1.1, due to fields that allow to identify Preloaded 
                        'OffSystem Tests do not exists in previous application versions
                        Dim myOFFSTestsUpdate As New OffSystemTestUpdateData
                        Dim myNumVersion As Integer = Convert.ToInt32(pCustomerSwVersion.Replace(".", ""))

                        If (myNumVersion >= 311) Then
                            '(1) Execute the Update Version Process for DELETED OFFS TESTS
                            resultData = myOFFSTestsUpdate.DELETERemovedOFFSTests(dbConnection, pUpdateVersionChangesList)
                        End If

                        '(2) Execute the Update Version Process for NEW OFFS TESTS
                        If (Not resultData.HasError) Then resultData = myOFFSTestsUpdate.CREATENewOFFSTests(dbConnection, pUpdateVersionChangesList)

                        If (myNumVersion >= 311) Then
                            '(3) Execute the Update Version Process for UPDATED OFFS TESTS
                            If (Not resultData.HasError) Then resultData = myOFFSTestsUpdate.UPDATEModifiedOFFSTests(dbConnection, pUpdateVersionChangesList)
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

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryOFFSTestsProgramming", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' New implementation of the UPDATE VERSION Process. Modify CUSTOMER DB with changes found in FACTORY DB. 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pCustomerSwVersion">SW Version in CUSTOMER DB</param>
        ''' <param name="pFactorySwVersion">SW Version in FACTORY DB</param>
        ''' <returns>GlobalDataTO containing an UpdateVersionChangesDS with all changes the UPDATE VERSION Process has made in CUSTOMER DB</returns>
        ''' <remarks>
        ''' Created by:  SA 17/10/2014 - BA-1944
        ''' Modified by: SA 20/11/2014 - BA-2105 ==> Included the call to the new function that executes the UpdateVersion Process for OffSystem Tests 
        ''' </remarks>
        Public Function SetFactoryTestProgrammingNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pCustomerSwVersion As String, _
                                                     ByVal pFactorySwVersion As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myUpdateVersionChangesList As New UpdateVersionChangesDS

                        'STEP 0 ==> Include information about VERSIONS in the structure containing all changes made in CUSTOMER DB
                        Dim myVersionInfoRow As UpdateVersionChangesDS.VersionDetailsRow = myUpdateVersionChangesList.VersionDetails.NewVersionDetailsRow()
                        myVersionInfoRow.UpdatedFROM = pCustomerSwVersion
                        myVersionInfoRow.UpdatedTO = pFactorySwVersion
                        myVersionInfoRow.UpdatedDateTime = Now.ToString("yyyyMMdd HH:mm")
                        myUpdateVersionChangesList.VersionDetails.AddVersionDetailsRow(myVersionInfoRow)

                        'STEP 1 ==> Execute UPDATE VERSION Process for ISE TESTS
                        resultData = SetFactoryISETestsProgramming(dbConnection, myUpdateVersionChangesList)

                        'STEP 2 ==> Execute UPDATE VERSION Process for STD TESTS (including CALIBRATORS and CONTAMINATIONS)
                        If (Not resultData.HasError) Then resultData = SetFactorySTDTestsProgramming(dbConnection, myUpdateVersionChangesList)

                        'STEP 3 ==> Execute UPDATE VERSION Process for OFFS TESTS 
                        If (Not resultData.HasError) Then resultData = SetFactoryOFFSTestsProgramming(dbConnection, pCustomerSwVersion, myUpdateVersionChangesList)

                        'STEP 4 ==> Execute UPDATE VERSION Process for CALC TESTS 
                        If (Not resultData.HasError) Then resultData = SetFactoryCALCTestsProgramming(dbConnection, myUpdateVersionChangesList)

                        'STEP 5 ==> If SW VERSION is v1, execute UPDATE VERSION Process to update ISE DATA information
                        If (pCustomerSwVersion = "1.0.0") Then
                            Dim iseDataInfoUpd As New ISEInformationUpdateData
                            resultData = iseDataInfoUpd.UpdateDataFormatv1Tov2(dbConnection)
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'Return the structure containing all changes made in CUSTOMER DB
                            resultData.SetDatos = myUpdateVersionChangesList
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

                Dim myLogAcciones As New ApplicationLogManager()
                GlobalBase.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryTestProgrammingNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace

#Region "FUNCTIONS FOR OLD UPDATE VERSION PROCESS (NOT USED)"
'Private AnalyzerModelAttribute As String = ""
'Private AnalyzerIDAttribute As String = ""
'Private WorkSessionIDAttribute As String = ""
' ''' <summary>
' ''' Sets the factory test programming into the client database (STD, ISE, CALC) tests, calibrators and contaminations
' ''' </summary>
' ''' <param name="pDBConnection"></param>
' ''' <returns>GlobalDataTo</returns>
' ''' <remarks>
' ''' Created by  AG 25/01/2013
' ''' Modified by XB 13/05/2013  - Add the new parameter pusSwVersion to evaluate required updates.
' '''                              Add functionality to update data ISE information for fields with date time format
' ''' </remarks>
'Public Function SetFactoryTestProgramming(ByVal pDBConnection As SqlClient.SqlConnection, _
'                                          ByVal pusSwVersion As String) As GlobalDataTO
'    Dim resultData As GlobalDataTO = Nothing
'    Dim dbConnection As SqlClient.SqlConnection = Nothing

'    Try
'        resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
'        If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
'            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
'            If (Not dbConnection Is Nothing) Then
'                '''''''''''''''''
'                'Update ISE tests
'                Dim iseUpd As New ISETestUpdateData
'                'Step 1: Update and create ISE tests in local DB to match Factory DB
'                resultData = iseUpd.UpdateFromFactoryUpdates(dbConnection)
'                If resultData.HasError Then
'                    'Process Error
'                Else
'                    'Step 2: Remove ISE tests in local DB to match missing ones in Factory DB
'                    resultData = iseUpd.UpdateFromFactoryRemoves(dbConnection)
'                    If resultData.HasError Then
'                        'Process Error
'                    Else

'                    End If
'                End If


'                '''''''''''''''''''
'                'Update Calibrators
'                Dim calibUpd As New CalibratorUpdateData
'                If Not resultData.HasError Then
'                    'Step 1: Update and create calibrators in local DB to match Factory DB
'                    resultData = calibUpd.UpdateFromFactoryUpdates(dbConnection)
'                End If

'                If resultData.HasError Then
'                    'Process Error
'                Else
'                    'Step 2: Remove calibrators in local DB to match missing ones in Factory DB
'                    resultData = calibUpd.UpdateFromFactoryRemoves(dbConnection)
'                    If resultData.HasError Then
'                        'Process Error
'                    Else

'                    End If
'                End If

'                ''''''''''''''''''
'                'Update STD tests 
'                Dim testUpd As New TestParametersUpdateData
'                If Not resultData.HasError Then
'                    'Step 1: Update and create STD tests in local DB to match Factory DB
'                    resultData = testUpd.UpdateFromFactoryUpdates(dbConnection)
'                End If
'                If resultData.HasError Then
'                    'Process Error
'                Else
'                    'Step 2: Remove STD tests in local DB to match missing ones in Factory DB
'                    resultData = testUpd.UpdateFromFactoryRemoves(dbConnection)
'                    If resultData.HasError Then
'                        'Process Error
'                    Else

'                    End If
'                End If

'                ' XB 04/06/2014 - BT #1646
'                If Not resultData.HasError Then
'                    resultData = testUpd.DoFinalActions(dbConnection)
'                End If


'                ''''''''''''''''''
'                'Update CALC tests
'                Dim calcUpd As New CalculatedTestUpdateData
'                If Not resultData.HasError Then
'                    'Step 1: Update and create Calc Tests in local DB to match Factory DB
'                    resultData = calcUpd.UpdateFromFactoryUpdates(dbConnection)
'                End If

'                If resultData.HasError Then
'                    'Process Error
'                Else
'                    'Step 2: Remove Calc Tests in local DB to match missing ones in Factory DB
'                    resultData = calcUpd.UpdateFromFactoryRemoves(dbConnection)
'                    If resultData.HasError Then
'                        'Process Error
'                    Else

'                    End If
'                End If

'                ' XB 21/02/2013 - (Bugs tracking #1134)
'                If Not resultData.HasError Then
'                    resultData = calcUpd.DoFinalActions(dbConnection)
'                End If

'                ''''''''''''''''''''''''''''''''
'                'Update STD tests contaminations
'                Dim contaminationUpd As New ContaminationsUpdateData
'                If Not resultData.HasError Then
'                    'Step 1: Update and create contaminations between STD tests in local DB to match Factory DB
'                    resultData = contaminationUpd.UpdateFromFactoryUpdates(dbConnection)
'                End If
'                If resultData.HasError Then
'                    'Process Error
'                Else
'                    'Step 2: Remove contaminations between STD tests in local DB to match missing ones in Factory DB
'                    resultData = contaminationUpd.UpdateFromFactoryRemoves(dbConnection)
'                    If resultData.HasError Then
'                        'Process Error
'                    Else

'                    End If
'                End If

'                ' '''''''''''''''''
'                'Update ISE data information - Only execute when Update From v1 - XB 13/05/2103
'                If pusSwVersion = "1.0.0" Then
'                    Dim iseDataInfoUpd As New ISEInformationUpdateData
'                    'Update data info ISE in local DB to match Factory DB
'                    resultData = iseDataInfoUpd.UpdateDataFormatv1Tov2(dbConnection)
'                    If resultData.HasError Then
'                        'Process Error
'                    End If
'                End If



'                If (Not resultData.HasError) Then
'                    'When the Database Connection was opened locally, then the Commit is executed
'                    If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
'                    'resultData.SetDatos = <value to return; if any>
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

'        Dim myLogAcciones As New ApplicationLogManager()
'        GlobalBase.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryTestProgramming", EventLogEntryType.Error, False)
'    Finally
'        If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
'    End Try
'    Return resultData
'End Function
#End Region