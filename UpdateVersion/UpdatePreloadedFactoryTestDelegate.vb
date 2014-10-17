Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants
Imports Biosystems.Ax00.BL.Framework
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Types
Imports Microsoft.SqlServer.Management.Smo

Namespace Biosystems.Ax00.BL.UpdateVersion

    Public Class UpdatePreloadedFactoryTestDelegate
        Private AnalyzerModelAttribute As String = ""
        Private AnalyzerIDAttribute As String = ""
        Private WorkSessionIDAttribute As String = ""

#Region "PUBLIC METHODS"

        ''' <summary>
        ''' Sets the factory test programming into the client database (STD, ISE, CALC) tests, calibrators and contaminations
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo</returns>
        ''' <remarks>
        ''' Created by  AG 25/01/2013
        ''' Modified by XB 13/05/2013  - Add the new parameter pusSwVersion to evaluate required updates.
        '''                              Add functionality to update data ISE information for fields with date time format
        ''' </remarks>
        Public Function SetFactoryTestProgramming(ByVal pDBConnection As SqlClient.SqlConnection, _
                                                  ByVal pusSwVersion As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        '''''''''''''''''
                        'Update ISE tests
                        Dim iseUpd As New ISETestUpdateData
                        'Step 1: Update and create ISE tests in local DB to match Factory DB
                        resultData = iseUpd.UpdateFromFactoryUpdates(dbConnection)
                        If resultData.HasError Then
                            'Process Error
                        Else
                            'Step 2: Remove ISE tests in local DB to match missing ones in Factory DB
                            resultData = iseUpd.UpdateFromFactoryRemoves(dbConnection)
                            If resultData.HasError Then
                                'Process Error
                            Else

                            End If
                        End If


                        '''''''''''''''''''
                        'Update Calibrators
                        Dim calibUpd As New CalibratorUpdateData
                        If Not resultData.HasError Then
                            'Step 1: Update and create calibrators in local DB to match Factory DB
                            resultData = calibUpd.UpdateFromFactoryUpdates(dbConnection)
                        End If

                        If resultData.HasError Then
                            'Process Error
                        Else
                            'Step 2: Remove calibrators in local DB to match missing ones in Factory DB
                            resultData = calibUpd.UpdateFromFactoryRemoves(dbConnection)
                            If resultData.HasError Then
                                'Process Error
                            Else

                            End If
                        End If

                        ''''''''''''''''''
                        'Update STD tests 
                        Dim testUpd As New TestParametersUpdateData
                        If Not resultData.HasError Then
                            'Step 1: Update and create STD tests in local DB to match Factory DB
                            resultData = testUpd.UpdateFromFactoryUpdates(dbConnection)
                        End If
                        If resultData.HasError Then
                            'Process Error
                        Else
                            'Step 2: Remove STD tests in local DB to match missing ones in Factory DB
                            resultData = testUpd.UpdateFromFactoryRemoves(dbConnection)
                            If resultData.HasError Then
                                'Process Error
                            Else

                            End If
                        End If

                        ' XB 04/06/2014 - BT #1646
                        If Not resultData.HasError Then
                            resultData = testUpd.DoFinalActions(dbConnection)
                        End If


                        ''''''''''''''''''
                        'Update CALC tests
                        Dim calcUpd As New CalculatedTestUpdateData
                        If Not resultData.HasError Then
                            'Step 1: Update and create Calc Tests in local DB to match Factory DB
                            resultData = calcUpd.UpdateFromFactoryUpdates(dbConnection)
                        End If

                        If resultData.HasError Then
                            'Process Error
                        Else
                            'Step 2: Remove Calc Tests in local DB to match missing ones in Factory DB
                            resultData = calcUpd.UpdateFromFactoryRemoves(dbConnection)
                            If resultData.HasError Then
                                'Process Error
                            Else

                            End If
                        End If

                        ' XB 21/02/2013 - (Bugs tracking #1134)
                        If Not resultData.HasError Then
                            resultData = calcUpd.DoFinalActions(dbConnection)
                        End If

                        ''''''''''''''''''''''''''''''''
                        'Update STD tests contaminations
                        Dim contaminationUpd As New ContaminationsUpdateData
                        If Not resultData.HasError Then
                            'Step 1: Update and create contaminations between STD tests in local DB to match Factory DB
                            resultData = contaminationUpd.UpdateFromFactoryUpdates(dbConnection)
                        End If
                        If resultData.HasError Then
                            'Process Error
                        Else
                            'Step 2: Remove contaminations between STD tests in local DB to match missing ones in Factory DB
                            resultData = contaminationUpd.UpdateFromFactoryRemoves(dbConnection)
                            If resultData.HasError Then
                                'Process Error
                            Else

                            End If
                        End If

                        ' '''''''''''''''''
                        'Update ISE data information - Only execute when Update From v1 - XB 13/05/2103
                        If pusSwVersion = "1.0.0" Then
                            Dim iseDataInfoUpd As New ISEInformationUpdateData
                            'Update data info ISE in local DB to match Factory DB
                            resultData = iseDataInfoUpd.UpdateDataFormatv1Tov2(dbConnection)
                            If resultData.HasError Then
                                'Process Error
                            End If
                        End If



                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            'resultData.SetDatos = <value to return; if any>
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
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryTestProgramming", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "PRIVATE METHODS"

#End Region

#Region "OLD METHODS commented- Initial development TR 2011"

        ' ''' <summary>
        ' ''' Updates tables where user can add or modify data.
        ' ''' </summary>
        ' ''' <returns></returns>
        ' ''' <remarks>
        ' ''' CREATED BY: TR 23/03/2011
        ' ''' </remarks>
        'Public Function SpecialsUpdates(Optional ByVal ServerConnection As Server = Nothing) As GlobalDataTO
        '    Dim myLogAcciones As New ApplicationLogManager()
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Dim dbLocalConnection As New SqlClient.SqlConnection
        '    Try

        '        dbLocalConnection = ServerConnection.ConnectionContext.SqlConnectionObject
        '        dbLocalConnection.ChangeDatabase("Ax00") 'TR 30/01/2012 -Set the database name'

        '        GetAnalyzerAndWorksessionInfo(dbLocalConnection)
        '        Dim myWS As New WorkSessionsDelegate
        '        'Reset any work session before starting update. Need to get the Worksession ID and the AnalyzerIDAttribute to reset the current WorkSession
        '        If (Not HISTWorkingMode) Then
        '            myGlobalDataTO = myWS.ResetWS(dbLocalConnection, AnalyzerIDAttribute, WorkSessionIDAttribute)
        '        Else
        '            myGlobalDataTO = myWS.ResetWSNEW(dbLocalConnection, AnalyzerIDAttribute, WorkSessionIDAttribute)
        '        End If

        '        If Not myGlobalDataTO.HasError Then
        '            'Get the connection to the temporal database. (Update database)
        '            dbConnection.ConnectionString = GlobalBase.UpdateDatabaseConnectionString
        '            'Open Connection 
        '            dbConnection.Open()


        '            'Star the update test process call the delegate in charge.
        '            Dim myTestParametersUpdateData As New TestParametersUpdateData
        '            myGlobalDataTO = myTestParametersUpdateData.UpdateUserTestData(dbConnection, dbLocalConnection)

        '            'UPDATE CALCULATED TEST.
        '            If Not myGlobalDataTO.HasError Then
        '                Dim myCalculatedTestUpdateData As New CalculatedTestUpdateData
        '                myGlobalDataTO = myCalculatedTestUpdateData.UpdateCalculatedTestData(dbConnection, dbLocalConnection)
        '            End If

        '            ''UPDATE CONTAMINATIONS.
        '            If Not myGlobalDataTO.HasError Then
        '                Dim myContaminationsUpdate As New ContaminationsUpdateData
        '                myGlobalDataTO = myContaminationsUpdate.UpdateContaminationsData(dbConnection, dbLocalConnection)
        '            End If

        '            'If there's no error then Update the database version on the user Data

        '        End If

        '    Catch ex As Exception
        '        myLogAcciones.CreateLogActivity("Backup Database Error ", "BiosystemUpdate.SpecialsUpdates", _
        '                                                                        EventLogEntryType.Information, False)
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message
        '    Finally
        '        dbConnection.Close()
        '    End Try

        '    Return myGlobalDataTO

        'End Function

        'Private Sub GetAnalyzerAndWorksessionInfo(ByVal dbConnection As SqlClient.SqlConnection)
        '    Dim myLogAcciones As New ApplicationLogManager()
        '    Dim myGlobalDataTO As New GlobalDataTO
        '    Dim myAnalyzerDelegate As New AnalyzersDelegate
        '    Try
        '        myGlobalDataTO = myAnalyzerDelegate.GetAnalyzer(dbConnection)
        '        If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
        '            Dim myAnalyzerData As New AnalyzersDS
        '            myAnalyzerData = DirectCast(myGlobalDataTO.SetDatos, AnalyzersDS)

        '            If (myAnalyzerData.tcfgAnalyzers.Rows.Count > 0) Then
        '                'Inform properties AnalyzerID and AnalyzerModel
        '                AnalyzerModelAttribute = myAnalyzerData.tcfgAnalyzers(0).AnalyzerModel.ToString
        '                AnalyzerIDAttribute = myAnalyzerData.tcfgAnalyzers(0).AnalyzerID.ToString
        '            End If
        '        End If

        '        Dim myWSAnalyzersDelegate As New WSAnalyzersDelegate

        '        myGlobalDataTO = myWSAnalyzersDelegate.GetActiveWSByAnalyzer(dbConnection, AnalyzerIDAttribute)

        '        Dim myWSAnalyzersDS As New WSAnalyzersDS
        '        myWSAnalyzersDS = DirectCast(myGlobalDataTO.SetDatos, WSAnalyzersDS)
        '        If (myWSAnalyzersDS.twksWSAnalyzers.Rows.Count > 0) Then
        '            WorkSessionIDAttribute = myWSAnalyzersDS.twksWSAnalyzers(0).WorkSessionID.ToString()
        '        End If

        '    Catch ex As Exception
        '        myLogAcciones.CreateLogActivity("Backup Database Error ", "BiosystemUpdate.SpecialsUpdates", _
        '                                                                        EventLogEntryType.Information, False)
        '        myGlobalDataTO.HasError = True
        '        myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        myGlobalDataTO.ErrorMessage = ex.Message

        '    End Try

        'End Sub

#End Region

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

                        '(8) Execute the Update Version Process for CONTAMINATIONS
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
                myLogAcciones.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryTestProgramming", EventLogEntryType.Error, False)
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
                myLogAcciones.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryISETestsProgramming", EventLogEntryType.Error, False)
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
                myLogAcciones.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryCALCTestsProgramming", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' New implementation of the UPDATE VERSION Process. Modify CUSTOMER DB with changes found in FACTORY DB. 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pusSwVersion">Customer SW Version</param>
        ''' <returns>GlobalDataTO containing an UpdateVersionChangesDS with all changes the UPDATE VERSION Process has made in CUSTOMER DB</returns>
        ''' <remarks>
        ''' Created by:  SA 17/10/2014 - BA-1944
        ''' </remarks>
        Public Function SetFactoryTestProgrammingNEW(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pusSwVersion As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myUpdateVersionChangesList As New UpdateVersionChangesDS

                        'STEP 1 ==> Execute UPDATE VERSION Process for ISE TESTS
                        resultData = SetFactoryISETestsProgramming(dbConnection, myUpdateVersionChangesList)

                        'STEP 2 ==> Execute UPDATE VERSION Process for STD TESTS (including CALIBRATORS and CONTAMINATIONS)
                        If (Not resultData.HasError) Then resultData = SetFactorySTDTestsProgramming(dbConnection, myUpdateVersionChangesList)

                        'STEP 3 ==> Execute UPDATE VERSION Process for CALC TESTS 
                        If (Not resultData.HasError) Then resultData = SetFactoryCALCTestsProgramming(dbConnection, myUpdateVersionChangesList)

                        'STEP 4 ==> If SW VERSION is v1, execute UPDATE VERSION Process to update ISE DATA information
                        If (pusSwVersion = "1.0.0") Then
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
                myLogAcciones.CreateLogActivity(ex.Message, "UpdatePreloadedFactoryTestDelegate.SetFactoryTestProgrammingNEW", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function
#End Region
    End Class
End Namespace