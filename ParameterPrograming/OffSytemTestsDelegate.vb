﻿Option Explicit On
Option Strict On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.Global.GlobalConstants

Namespace Biosystems.Ax00.BL

    Public Class OffSystemTestsDelegate

#Region "Public Methods"
        ''' <summary>
        '''  Create a new OFF-SYSTEM Test (basic data, sample type data and reference ranges)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestDS">Typed DataSet OffSystemTestsDS with basic data of the OFF-SYSTEM Test to add</param>
        ''' <param name="pTestSampleTypesDS">Typed DataSet OffSystemTestSamplesDS containing values defined for the OFF-SYSTEM
        '''                                  Test for the selected SampleType</param>
        ''' <param name="pRefRangesDS">Typed DataSet TestRefRangesDS containing all Reference Ranges defined for the OFF-SYSTEM Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsDS with all data of the added OFF-SYSTEM Test
        '''          or error information</returns>
        ''' <remarks>
        ''' Created by:  DL 29/11/2010
        ''' Modified by: SA 03/01/2011 - Function name changed to Add; function logic changed: add also the Sample Type information;
        '''                              call new function SaveReferenceRanges to save the Reference Ranges
        ''' AG 01/09/2014 - BA-1869 when new OFFS test is created the CustomPosition informed = MAX current value + 1
        ''' </remarks>
        Public Function Add(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestDS As OffSystemTestsDS, _
                            ByVal pTestSampleTypesDS As OffSystemTestSamplesDS, ByVal pRefRangesDS As TestRefRangesDS) _
                            As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Insert the new OFF-SYSTEM Test 
                        Dim offSystemTestToAdd As New tparOffSystemTestsDAO

                        'AG 01/09/2014 - BA-1869 new calc test customposition value = MAX current value + 1
                        resultData = offSystemTestToAdd.GetLastCustomPosition(dbConnection)
                        If Not resultData.HasError Then
                            If resultData.SetDatos Is Nothing OrElse resultData.SetDatos Is DBNull.Value Then
                                pOffSystemTestDS.tparOffSystemTests(0).CustomPosition = 1
                            Else
                                pOffSystemTestDS.tparOffSystemTests(0).CustomPosition = DirectCast(resultData.SetDatos, Integer) + 1
                            End If
                            'AG 01/09/2014 - BA-1869

                            resultData = offSystemTestToAdd.Create(dbConnection, pOffSystemTestDS)

                            If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                                'Get the generated OffSystemTestID from the dataset returned 
                                Dim generatedID As Integer = -1
                                generatedID = DirectCast(resultData.SetDatos, OffSystemTestsDS).tparOffSystemTests(0).OffSystemTestID

                                'Set value of OffSystemTestID in the dataset containing the data of the Selected Sample Type
                                For i As Integer = 0 To pTestSampleTypesDS.tparOffSystemTestSamples.Rows.Count - 1
                                    pTestSampleTypesDS.tparOffSystemTestSamples(i).OffSystemTestID = generatedID
                                Next i

                                'Set value of OffSystemTestID in the dataset containing the Reference Ranges
                                For i As Integer = 0 To pRefRangesDS.tparTestRefRanges.Rows.Count - 1
                                    pRefRangesDS.tparTestRefRanges(i).TestID = generatedID
                                Next

                                'Insert the Test Sample Type values
                                Dim sampleTypeToAdd As New OffSystemTestSamplesDelegate
                                resultData = sampleTypeToAdd.Add(dbConnection, pTestSampleTypesDS)
                                If (Not resultData.HasError) Then
                                    'Finally, insert the Reference Ranges
                                    If (pRefRangesDS.tparTestRefRanges.Rows.Count > 0) Then
                                        resultData = SaveReferenceRanges(dbConnection, pRefRangesDS)
                                    End If
                                End If
                            End If

                        End If 'AG 01/09/2014 - BA-1869

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)

                            'Basic data of the added OFF-SYSTEM Test is returned 
                            resultData.SetDatos = pOffSystemTestDS
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
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.Add", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete all the specified OFF-SYSTEM Tests (Test Definition, Sample Type data and Reference Ranges) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTests">Typed DataSet OffSystemTestsDS with the list of Off System Tests to delete</param>        
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  DL 01/12/2010
        ''' Modified by: SA 24/12/2010 - Code changed to allow deletion of several Off System Tests; before delete each Test,
        '''                              remove it from all Profiles in which it is included and remove also the Reference
        '''                              Ranges defined for it
        '''              TR 04/09/2012 - Added call to function HIST_CloseOFFSTestSample to verify for each deleted OffSystem Test if it
        '''                              has to be marked as closed in Historic Module
        '''              XB 18/02/2013 - Fix the use of parameter pTestType which is not used in this function nowadays (BugsTracking #1136)
        '''              AG 10/05/2013 - Mark as deleted test if this test form part of a LIS saved worksession
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTests As OffSystemTestsDS) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfileDelegate As New TestProfilesDelegate
                        Dim myTestRefRangesDelegate As New TestRefRangesDelegate

                        Dim myOffSystemTestsDelete As New tparOffSystemTestsDAO
                        Dim myOffSystemTestSamples As New OffSystemTestSamplesDelegate
                        Dim mySavedWS As New SavedWSOrderTestsDelegate

                        For Each offSystemTest As OffSystemTestsDS.tparOffSystemTestsRow In pOffSystemTests.tparOffSystemTests.Rows
                            'Mark as DeletedFlag if exists into a saved Worksession from LIS
                            resultData = mySavedWS.UpdateDeletedTestFlag(dbConnection, "OFFS", offSystemTest.OffSystemTestID)
                            If resultData.HasError Then Exit For

                            'Remove the Off System Test from all Profiles in which it is included 
                            resultData = myTestProfileDelegate.DeleteByTestIDSampleType(dbConnection, offSystemTest.OffSystemTestID, "", "OFFS")
                            If resultData.HasError Then Exit For

                            'Delete Reference Ranges for the affected OffSystem Test
                            resultData = myTestRefRangesDelegate.DeleteByTestID(dbConnection, offSystemTest.OffSystemTestID, "", "OFFS")
                            If (resultData.HasError) Then Exit For

                            'Delete all Sample Types defined for the Test (just one, the screen does not allow more)
                            resultData = myOffSystemTestSamples.DeleteByTestID(dbConnection, offSystemTest.OffSystemTestID)
                            If (resultData.HasError) Then Exit For

                            'Delete the Off-System Test
                            resultData = myOffSystemTestsDelete.Delete(dbConnection, offSystemTest.OffSystemTestID)
                            If (resultData.HasError) Then Exit For

                            'Verify if the OFF-SYSTEM Test/Sample Type exists in Historic Module to mark it as closed
                            If (HISTWorkingMode) Then
                                resultData = HIST_CloseOFFSTestSample(dbConnection, offSystemTest.OffSystemTestID)
                                If (resultData.HasError) Then Exit For
                            End If
                        Next

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
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Verify if there is already a OFF-SYSTEM Test with the informed OFF SYSTEM Test Name
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestName">OFF-SYSTEM Test Name to be validated</param>
        ''' <param name="pNameToSearch">Value indicating which is the name to validate: the short name or the long one</param>
        ''' <param name="pOffSystemTestID">OFF-SYSTEM Test Identifier. It is an optional parameter informed
        '''                                only in case of updation</param>
        ''' <returns>GlobalDataTO containing a boolean value: True if there is another OFF-SYSTEM Test with the same 
        '''          name; otherwise, False</returns>
        ''' <remarks>
        ''' Created by:  SA 03/01/2011 
        ''' </remarks>
        Public Function ExistsOffSystemTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestName As String, _
                                            ByVal pNameToSearch As String, Optional ByVal pOffSystemTestID As Integer = 0) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim offSystemTestDAO As New tparOffSystemTestsDAO
                        resultData = offSystemTestDAO.ExistsOffSystemTest(dbConnection, pOffSystemTestName, pNameToSearch, pOffSystemTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.ExistsOffSystemTest", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined Off-System Tests using the specified SampleType 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCustomizedTestSelection">FALSE same order as until 3.0.2 / When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsDS with data of the OffSystemTests using
        '''          the specified SampleType</returns>
        ''' <remarks>
        ''' Created by DL: 29/11/2010
        ''' AG 01/09/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function GetBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleType As String, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOffSystemTests As New tparOffSystemTestsDAO
                        resultData = myOffSystemTests.ReadBySampleType(dbConnection, pSampleType, pCustomizedTestSelection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.GetBySampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified OFF-SYSTEM Test and, for the informed SampleType, value of field 
        ''' ActiveRangeType (type of defined Reference Ranges for the Test/SampleType)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Identifier of the OFF-SYSTEM Test</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsDS with data of the specified TestID/SampleType</returns>
        ''' <remarks>
        ''' Created by:  SA 24/01/2011
        ''' </remarks>
        Public Function GetByTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                 ByVal pSampleType As String) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOffSystemTests As New tparOffSystemTestsDAO
                        resultData = myOffSystemTests.ReadByTestIDAndSampleType(dbConnection, pTestID, pSampleType)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.GetByTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of all defined OFF-SYSTEM Tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfilesDS with the list of Test Profiles</returns>
        ''' <remarks>
        ''' Created by: DL 25/11/2010
        ''' </remarks>
        Public Function GetList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOffSystemTestsDAO As New tparOffSystemTestsDAO
                        resultData = myOffSystemTestsDAO.ReadAll(dbConnection)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.GetList", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of the specified OFF-SYSTEM Test (basic data, sample type data and reference ranges)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestDS">Typed DataSet OffSystemTestsDS with basic data of the OFF-SYSTEM Test to add</param>
        ''' <param name="pTestSampleTypesDS">Typed DataSet OffSystemTestSamplesDS containing values defined for the OFF-SYSTEM
        '''                                  Test for the selected SampleType</param>
        ''' <param name="pRefRangesDS">Typed DataSet TestRefRangesDS containing all Reference Ranges defined for the OFF-SYSTEM Test</param>
        ''' <param name="pAffectedElementsDS">Typed DataSet DependenciesElementsDS containing the list of Profiles that will be changed/deleted when the 
        '''                                   SampleType of an existing OFF-SYSTEM Test has been changed</param>
        ''' <param name="pUpdateHistory">Optional parameter. It is set to TRUE when fields OffSystemTestName, MeasureUnit and/or DecimalsAllowed are updated, 
        '''                              and in this case, it is needed to verify if the OFF-SYSTEM Test/SampleType exists in Historic Module to update 
        '''                              the data also in it</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsDS with all data of the updated OFF-SYSTEM Test
        '''          or error information</returns>
        ''' <remarks>
        ''' Created by:  DL 29/11/2010
        ''' Modified by: SA 03/01/2011 - Function name changed to Modify; function logic changed: update also the Sample Type information;
        '''                              call new function SaveReferenceRanges to save the Reference Ranges
        '''              TR 04/09/2012 - Added optional parameter pUpdateHistory to indicate if data of the OffSystem Test has to be updated in Historic Module
        '''                            - Added optional parameter pCloseHistory to indicate if the OffSystem Test has to be marked as closed in Historic Module
        '''                            - Depending on value of the added parameters call function to update data or mark the Test as closed in Historic Module
        '''              SA 17/09/2012 - Removed parameter pCloseHistory and its related functionality; it is not needed due to function HIST_UpdateByOFFSTestIDAndSampleType 
        '''                              verifies if fields ResultType and/or SampleType have been changed and in this case close the OFFSystemTest 
        ''' </remarks>
        Public Function Modify(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestDS As OffSystemTestsDS, ByVal pTestSampleTypesDS As OffSystemTestSamplesDS, _
                               ByVal pRefRangesDS As TestRefRangesDS, ByVal pAffectedElementsDS As DependenciesElementsDS, Optional ByVal pUpdateHistory As Boolean = False) _
                               As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'If there are affected Elements, remove the OFF-SYSTEM Test from all Profiles in which it is included 
                        If (pAffectedElementsDS.DependenciesElements.Count > 0) Then
                            Dim myTestProfileDelegate As New TestProfilesDelegate
                            resultData = myTestProfileDelegate.DeleteByTestIDSampleType(dbConnection, pOffSystemTestDS.tparOffSystemTests(0).OffSystemTestID, "", "OFFS")
                        End If

                        'Update basic data of the OFF-SYSTEM Test
                        If (Not resultData.HasError) Then
                            Dim offSystemTestToUpdate As New tparOffSystemTestsDAO
                            resultData = offSystemTestToUpdate.Update(dbConnection, pOffSystemTestDS)
                        End If

                        'Update information of the SampleType used for the specified OFF-SYSTEM Test
                        If (Not resultData.HasError) Then
                            Dim myOffSystemTestSamplesDelegate As New OffSystemTestSamplesDelegate
                            resultData = myOffSystemTestSamplesDelegate.Modify(dbConnection, pTestSampleTypesDS)
                        End If

                        'Update the Reference Ranges
                        If (Not resultData.HasError) Then
                            If (pOffSystemTestDS.tparOffSystemTests(0).ResultType = "QUALTIVE") Then
                                'If the ResultType of the Test has been changed from Quantitative to Qualitative, delete existing Ranges (if any)
                                Dim testRefRangesDelegate As New TestRefRangesDelegate
                                resultData = testRefRangesDelegate.DeleteByTestID(dbConnection, pTestSampleTypesDS.tparOffSystemTestSamples(0).OffSystemTestID, _
                                                                                  pTestSampleTypesDS.tparOffSystemTestSamples(0).SampleType, "OFFS")
                            Else
                                'Update the Reference Ranges
                                If (pRefRangesDS.tparTestRefRanges.Rows.Count > 0) Then
                                    resultData = SaveReferenceRanges(dbConnection, pRefRangesDS)
                                End If
                            End If
                        End If

                        'Update data of the OFF-SYSTEM Test/SampleType in Historic Module
                        If (HISTWorkingMode) Then
                            If (Not resultData.HasError) Then
                                If (pUpdateHistory) Then
                                    'Prepare the DS needed to update data in Historic Module
                                    Dim myHisOFFSTestSamplesDS As New HisOFFSTestSamplesDS
                                    Dim myHisOFFSTestSamplesRow As HisOFFSTestSamplesDS.thisOffSystemTestSamplesRow

                                    myHisOFFSTestSamplesRow = myHisOFFSTestSamplesDS.thisOffSystemTestSamples.NewthisOffSystemTestSamplesRow
                                    myHisOFFSTestSamplesRow.OffSystemTestID = pOffSystemTestDS.tparOffSystemTests(0).OffSystemTestID
                                    myHisOFFSTestSamplesRow.SampleType = pTestSampleTypesDS.tparOffSystemTestSamples(0).SampleType
                                    myHisOFFSTestSamplesRow.OffSystemTestName = pOffSystemTestDS.tparOffSystemTests(0).Name
                                    myHisOFFSTestSamplesRow.DecimalsAllowed = CInt(pOffSystemTestDS.tparOffSystemTests(0).Decimals)
                                    myHisOFFSTestSamplesRow.MeasureUnit = pOffSystemTestDS.tparOffSystemTests(0).Units
                                    myHisOFFSTestSamplesRow.ResultType = pOffSystemTestDS.tparOffSystemTests(0).ResultType

                                    myHisOFFSTestSamplesDS.thisOffSystemTestSamples.AddthisOffSystemTestSamplesRow(myHisOFFSTestSamplesRow)

                                    'Update data of the OFF-SYSTEM Test/Sample Type if it exists in Historic Module
                                    resultData = HIST_UpdateByOFFSTestIDAndSampleType(dbConnection, myHisOFFSTestSamplesDS)
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
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.Modify", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified OFF-SYSTEM Test
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestID">Identifier of the OffSystem Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsDS with data of the specified offsystem Test</returns>
        ''' <remarks>
        ''' Created by: DL 29/11/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOffSystemTestsDAO As New tparOffSystemTestsDAO
                        resultData = myOffSystemTestsDAO.Read(dbConnection, pOffSystemTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified OFF-SYSTEM Test, including the Unit description for Quantitative Tests 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOffSystemTestID">Identifier of the OFF-SYSTEM Test</param>
        ''' <returns>GlobalDataTO containing a typed DataSet OffSystemTestsDS with data of the specified OFF-SYSTEM Test</returns>
        ''' <remarks>
        ''' Created by: SA 31/01/2011 
        ''' </remarks>
        Public Function ReadWithUnitDesc(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOffSystemTestID As Integer) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myOffSystemTestsDAO As New tparOffSystemTestsDAO
                        resultData = myOffSystemTestsDAO.ReadWithUnitDesc(dbConnection, pOffSystemTestID)
                    End If
                End If
            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.ReadWithUnitDesc", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all OFF-SYSTEM Tests added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for OFF-SYSTEM Test that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 10/12/2010 (Based on CalculatedTestDelegate.UpdateInUseFlag) 
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pAnalyzerID As String, ByVal pFlag As Boolean, _
                                        Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparOffSystemTestsDAO
                        myGlobalDataTO = myDAO.UpdateInUseFlag(dbConnection, pWorkSessionID, pAnalyzerID, pFlag, pUpdateForExcluded)

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
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.UpdateInUseFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Update the field InUse by TestID
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pTestID"></param>
        ''' <param name="pInUseFlag"></param>
        ''' <returns></returns>
        ''' <remarks>AG 08/05/2013</remarks>
        Public Function UpdateInUseByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, ByVal pInUseFlag As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparOffSystemTestsDAO
                        myGlobalDataTO = myDAO.UpdateInUseByTestID(dbConnection, pTestID, pInUseFlag)

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

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.UpdateInUseByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Validate if the deletion of the specified OFF-SYSTEM Tests affects Test Profiles in which they are included
        ''' </summary>
        ''' <returns>GlobalDataTO containing a typed DataSet DependenciesElementsDS with the list of affected elements</returns>
        ''' <remarks>
        ''' Created by: SA 03/11/2011
        ''' </remarks>
        Public Function ValidatedDependencies(ByVal pOffSystemTestsDS As OffSystemTestsDS) As GlobalDataTO
            Dim myResult As New GlobalDataTO
            Try
                Dim imageBytes As Byte()
                Dim preloadedDataConfig As New PreloadedMasterDataDelegate()

                Dim myDependeciesElementsDS As New DependenciesElementsDS
                Dim myDependenciesElementsRow As DependenciesElementsDS.DependenciesElementsRow

                Dim myGlobalDataTO As New GlobalDataTO
                Dim myTestProfileTestDS As New TestProfileTestsDS

                For Each offSystemTestRow As OffSystemTestsDS.tparOffSystemTestsRow In pOffSystemTestsDS.tparOffSystemTests.Rows
                    'Clean the Test Profiles DS for each new OFF-SYSTEM Test to process
                    myTestProfileTestDS.tparTestProfileTests.Clear()

                    'Get the Test Profiles in which the OFF-SYSTEM Test is included
                    Dim myTestProfilesTestDelegate As New TestProfileTestsDelegate
                    myGlobalDataTO = myTestProfilesTestDelegate.ReadByTestID(Nothing, offSystemTestRow.OffSystemTestID, "", "OFFS")

                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myTestProfileTestDS = DirectCast(myGlobalDataTO.SetDatos, TestProfileTestsDS)

                        'Get icon for TestProfiles; load the returned Test Profiles in the list of affected Elements to return
                        imageBytes = preloadedDataConfig.GetIconImage("PROFILES")
                        For Each testProfTest As TestProfileTestsDS.tparTestProfileTestsRow In myTestProfileTestDS.tparTestProfileTests.Rows
                            myDependenciesElementsRow = myDependeciesElementsDS.DependenciesElements.NewDependenciesElementsRow()

                            myDependenciesElementsRow.Type = imageBytes
                            myDependenciesElementsRow.Name = testProfTest.TestProfileName
                            myDependenciesElementsRow.FormProfileMember = offSystemTestRow.Name

                            'Insert on dependencies table
                            myDependeciesElementsDS.DependenciesElements.AddDependenciesElementsRow(myDependenciesElementsRow)
                        Next
                    Else
                        myResult = myGlobalDataTO
                        Exit For
                    End If
                Next
                myResult.SetDatos = myDependeciesElementsDS
            Catch ex As Exception
                myResult.HasError = True
                myResult.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myResult.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.ValidatedDependencies", EventLogEntryType.Error, False)
            End Try
            Return myResult
        End Function

        ''' <summary>
        ''' Udate the LIS value by the Off System Test ID.
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <param name="pOFFSystemTestID">Off System Test ID.</param>
        ''' <param name="pLISValue">LIS Value.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' CREATED BY: TR 04/03/2013
        ''' </remarks>
        Public Function UpdateLISValueByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOFFSystemTestID As Integer, pLISValue As String) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparOffSystemTestsDAO
                        myGlobalDataTO = myDAO.UpdateLISValueByTestID(dbConnection, pOFFSystemTestID, pLISValue)

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
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.UpdatepdateLISValueByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function


        ''' <summary>
        ''' Gets all OFFS tests order by CustomPosition (return columns: TestType, TestID, CustomPosition As TestPosition, PreloadedTest, Available)
        ''' </summary>
        ''' <param name="pDBConnection"></param>
        ''' <returns>GlobalDataTo with setDatos ReportsTestsSortingDS</returns>
        ''' <remarks>
        ''' AG 02/09/2014 - BA-1869
        ''' </remarks>
        Public Function GetCustomizedSortedTestSelectionList(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)

                If (Not resultData.HasError AndAlso Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparOffSystemTestsDAO
                        resultData = myDAO.GetCustomizedSortedTestSelectionList(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "OffSystemTestsDelegate.GetCustomizedSortedTestSelectionList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Update (only when informed) columns CustomPosition and Available for OFFS tests
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestsSortingDS">Typed DataSet ReportsTestsSortingDS containing all tests to update</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: AG 03/09/2014 - BA-1869
        ''' </remarks>
        Public Function UpdateCustomPositionAndAvailable(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestsSortingDS As ReportsTestsSortingDS) _
                                           As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New tparOffSystemTestsDAO
                        myGlobalDataTO = myDAO.UpdateCustomPositionAndAvailable(dbConnection, pTestsSortingDS)

                        'Update CALCTEST available in cascade depending their components (all components available -> CalcTest available // else CalcTest NOT available
                        If Not myGlobalDataTO.HasError Then
                            Dim myCalcTestDlg As New CalculatedTestsDelegate
                            myGlobalDataTO = myCalcTestDlg.UpdateAvailableCascadeByComponents(dbConnection)
                        End If

                        'Update PROFILE available in cascade depending their components (all components available -> Profile available // else Profile NOT available
                        If Not myGlobalDataTO.HasError Then
                            Dim myTestProfileDlg As New TestProfilesDelegate
                            myGlobalDataTO = myTestProfileDlg.UpdateAvailableCascadeByComponents(dbConnection)
                        End If

                    End If

                    If (Not myGlobalDataTO.HasError) Then
                        'When the Database Connection was opened locally, then the Commit is executed
                        If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                    Else
                        'When the Database Connection was opened locally, then the Rollback is executed
                        If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                    End If
                End If

            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.UpdateCustomPositionAndAvailable", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (myGlobalDataTO)
        End Function

#End Region

#Region "Private Methods"
        ''' <summary>
        ''' When an OffSystem Test is deleted in OFF-SYSTEM Tests Programming Screen, if it exists in the corresponding table in 
        ''' Historics Module, then it is marked as deleted by updating field ClosedOffSystemTest to True
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOFFSTestID">OFF-SYSTEM Test Identifier in Parameters Programming Module</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 24/02/2012
        ''' Modified by: TR 05/09/2012 - Method has been moved from class HisOFFTestSamplesDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisOffSystemTestSamples are called
        '''              SA 17/09/2012 - Removed parameter pSampleType and also the call to function ReadByOFFSTestIDAndSampleType
        ''' </remarks>
        Private Function HIST_CloseOFFSTestSample(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOFFSTestID As Integer) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there is an open version of the OffSystem Test/SampleType in Historics
                        Dim myHisOFFSTestsDAO As New thisOffSystemTestSamplesDAO

                        myGlobalDataTO = myHisOFFSTestsDAO.ReadByOFFSTestID(dbConnection, pOFFSTestID)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim auxiliaryDS As HisOFFSTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HisOFFSTestSamplesDS)

                            If (auxiliaryDS.thisOffSystemTestSamples.Rows.Count > 0) Then
                                'There is an open version of the OffSystem Test/SampleType in Historics module, mark it as Closed
                                myGlobalDataTO = myHisOFFSTestsDAO.CloseOFFSTestSample(dbConnection, auxiliaryDS.thisOffSystemTestSamples.First.HistOffSystemTestID)
                            End If
                        End If

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.HIST_CloseOFFSTestSample", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' When basic data of an OFFSTest/SampleType (OffSystemTestName, MeasureUnit and/or DecimalsAllowed) is changed in OFF-SYSTEM Tests 
        ''' Programming Screen,  values are updated for the corresponding record in the Historics Module table (the one having the same 
        ''' OffSystemTestID and SampleType,  and having ClosedOffSystemTest = False)But when besides basic data, the ResultType is changed,
        ''' But when besides basic data, the ResultType is changed, then the OffSystem Test is marked as closed and the data is not updated 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pHisOFFSTestsDS">Typed DataSet HisOFFSTestSamplesDS containing the OFFTest/SampleType to update</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 24/02/2012
        ''' Modified by: TR 05/09/2012 - Method has been moved from class HisOFFTestSamplesDelegate due to it is not possible to call functions in
        '''                              History project from Parameters Programming project (circular references are not allowed). Functions in DAO 
        '''                              Class for table thisOffSystemTestSamples are called
        '''              SA 17/09/2012 - To verify if the OffSystem Test already exists in Historic Module, call function ReadByOFFTestID instead of
        '''                              function ReadByOFFSTestIDAndSampleType. Besides verifying if field ResultType has been changed, verify also if 
        '''                              field SampleType has been changed, because in both cases the open version of the OffSystem Test in Historic 
        '''                              Module has to be marked as closed
        ''' </remarks>
        Private Function HIST_UpdateByOFFSTestIDAndSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pHisOFFSTestsDS As HisOFFSTestSamplesDS) As GlobalDataTO
            Dim myGlobalDataTO As GlobalDataTO = Nothing
            Dim dbConnection As SqlClient.SqlConnection = Nothing

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify if there is an open version of the OffSystem Test/SampleType in Historics
                        Dim myHisOFFSTestsDAO As New thisOffSystemTestSamplesDAO
                        myGlobalDataTO = myHisOFFSTestsDAO.ReadByOFFSTestID(dbConnection, pHisOFFSTestsDS.thisOffSystemTestSamples.First.OffSystemTestID)

                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            Dim auxiliaryDS As HisOFFSTestSamplesDS = DirectCast(myGlobalDataTO.SetDatos, HisOFFSTestSamplesDS)

                            If (auxiliaryDS.thisOffSystemTestSamples.Rows.Count > 0) Then
                                'There is an open version of the OffSystem Test in Historics module, verify if fields SampleType and/or ResultType 
                                'have been changed
                                If (auxiliaryDS.thisOffSystemTestSamples.First.SampleType = pHisOFFSTestsDS.thisOffSystemTestSamples.First.SampleType) AndAlso _
                                   (auxiliaryDS.thisOffSystemTestSamples.First.ResultType = pHisOFFSTestsDS.thisOffSystemTestSamples.First.ResultType) Then
                                    'Data of the current version of the OffSystem Test/SampleType is updated
                                    pHisOFFSTestsDS.thisOffSystemTestSamples.First.HistOffSystemTestID = auxiliaryDS.thisOffSystemTestSamples.First.HistOffSystemTestID
                                    myGlobalDataTO = myHisOFFSTestsDAO.Update(dbConnection, pHisOFFSTestsDS)
                                Else
                                    'The current version of the OffSystem Test/SampleType is marked as closed
                                    myGlobalDataTO = myHisOFFSTestsDAO.CloseOFFSTestSample(dbConnection, auxiliaryDS.thisOffSystemTestSamples.First.HistOffSystemTestID)
                                End If
                            End If
                        End If

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
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO = New GlobalDataTO()
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.HIST_UpdateByOFFSTestIDAndSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing AndAlso Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Receives all added/updated/deleted Reference Ranges for an OFF-SYSTEM Test, places them in a three separated 
        ''' DataSets and call the function that execute the saving of the Reference Ranges
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pRefRangesDS">Typed DataSet TestRefRangesDS containing all added/updated/deleted Reference
        '''                            Ranges for a OFF-SYSTEM Test</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 03/01/2011
        ''' </remarks>
        Private Function SaveReferenceRanges(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pRefRangesDS As TestRefRangesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Separate added, updated and deleted Reference Ranges in three different DS
                        Dim newRefRangesDS As New TestRefRangesDS
                        Dim updatedRefRangesDS As New TestRefRangesDS
                        Dim deletedRefRangesDS As New TestRefRangesDS

                        Dim myGlobalBase As New GlobalBase
                        Dim myTestRefRanges As New List(Of TestRefRangesDS.tparTestRefRangesRow)


                        'CREATE: Get all added Reference Ranges
                        myTestRefRanges = (From a In pRefRangesDS.tparTestRefRanges _
                                          Where a.IsNew = True _
                                         Select a).ToList()

                        For i As Integer = 0 To myTestRefRanges.Count - 1
                            myTestRefRanges(0).BeginEdit()
                            myTestRefRanges(0).TS_User = myGlobalBase.GetSessionInfo.UserName
                            myTestRefRanges(0).TS_DateTime = Now
                            myTestRefRanges(0).EndEdit()

                            newRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                        Next

                        'UPDATE: Get all updated Reference Ranges
                        myTestRefRanges = (From a In pRefRangesDS.tparTestRefRanges _
                                          Where a.IsNew = False _
                                            And a.IsDeleted = False _
                                         Select a).ToList()

                        For i As Integer = 0 To myTestRefRanges.Count - 1
                            myTestRefRanges(0).BeginEdit()
                            myTestRefRanges(0).TS_User = myGlobalBase.GetSessionInfo.UserName
                            myTestRefRanges(0).TS_DateTime = Now
                            myTestRefRanges(0).EndEdit()

                            updatedRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                        Next

                        'DELETE: Get all Reference Ranges marked to delete
                        myTestRefRanges = (From a In pRefRangesDS.tparTestRefRanges _
                                          Where a.IsDeleted = True _
                                         Select a).ToList()

                        For i As Integer = 0 To myTestRefRanges.Count - 1
                            deletedRefRangesDS.tparTestRefRanges.ImportRow(myTestRefRanges(i))
                        Next

                        'Save Reference Ranges
                        resultData = SaveTestRefRanges(Nothing, newRefRangesDS, updatedRefRangesDS, deletedRefRangesDS)
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
                If (pDBConnection Is Nothing And Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.SaveReferenceRanges", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Save Reference Ranges for an OFF-SYSTEM Test: add new ranges, update values of existing ranges, and/or remove deleted 
        ''' existing ranges 
        ''' </summary>
        ''' <param name="pDbConnection">Open DB Connection</param>
        ''' <param name="pNewRefRanges">Typed DataSet TestRefRangesDS containing all Reference Ranges to add</param>
        ''' <param name="pUpdatedRefRanges">Typed DataSet TestRefRangesDS containing all Reference Ranges to update</param>
        ''' <param name="pDeletedRefRanges">Typed DataSet TestRefRangesDS containing all Reference Ranges to delete</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SA 03/01/2011
        ''' </remarks>
        Private Function SaveTestRefRanges(ByVal pDbConnection As SqlClient.SqlConnection, ByVal pNewRefRanges As TestRefRangesDS, _
                                           ByVal pUpdatedRefRanges As TestRefRangesDS, ByVal pDeletedRefRanges As TestRefRangesDS) _
                                           As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myRefRangesDel As New TestRefRangesDelegate

                        'Remove deleted Ranges
                        If (pDeletedRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                            resultData = myRefRangesDel.Delete(dbConnection, pDeletedRefRanges, "OFFS")
                        End If

                        'Add new Ranges
                        If (Not resultData.HasError) Then
                            If (pNewRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                                resultData = myRefRangesDel.Create(dbConnection, pNewRefRanges, "OFFS")
                            End If
                        End If

                        'Update values of modified Ranges
                        If (Not resultData.HasError) Then
                            If (pUpdatedRefRanges.tparTestRefRanges.Rows.Count > 0) Then
                                resultData = myRefRangesDel.Update(dbConnection, pUpdatedRefRanges, "OFFS")
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                        Else
                            'When the Database Connection was opened locally, then the Rollback is executed
                            If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                        End If
                    End If
                End If
            Catch ex As Exception
                'When the Database Connection was opened locally, then the Rollback is executed
                If (pDbConnection Is Nothing And Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.SaveTestRefRanges", EventLogEntryType.Error, False)
            Finally
                If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

#End Region

#Region "TO DELETE??"
        '''' <summary>
        '''' Add a new offSystemTest 
        '''' </summary>
        '''' <param name="pDbConnection">Open DB Connection</param>
        '''' <param name="pOffSystemTestsDetails">Typed DataSet offSystemTestsDS containing the data of the offSystemTest to add</param>
        '''' <returns>GlobalDataTO containing the added record and/or error information</returns>
        '''' <remarks>Created by: DL 29/11/2010
        '''' </remarks>
        'Public Function Add(ByVal pDbConnection As SqlClient.SqlConnection, _
        '                       ByVal pOffSystemTestsDetails As OffSystemTestsDS) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim offSystemTestToAdd As New tparOffSystemTestsDAO

        '                For Each row As OffSystemTestsDS.tparOffSystemTestsRow In pOffSystemTestsDetails.tparOffSystemTests.Rows
        '                    'Validate if there is another offSystemTest with the same ID --> PDT !!!!!!!!!!!!!

        '                    If (Not resultData.HasError) Then
        '                        'Add the new offSystemTestSample
        '                        resultData = offSystemTestToAdd.Create(dbConnection, row)
        '                    End If

        '                    If resultData.HasError Then Exit For
        '                Next

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    'resultData.SetDatos = <value to return; if any>
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If

        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.Create", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData

        'End Function

        '''' <summary>
        '''' </summary>
        '''' <param name="pDbConnection"></param>
        '''' <param name="pNewOffSystemRefRanges"></param>
        '''' <param name="pUpdatedOffSystemRefRanges"></param>
        '''' <param name="pDeletedOffSystemRefRanges"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function SaveOffSystemRefRanges(ByVal pDbConnection As SqlClient.SqlConnection, _
        '                                       ByVal pNewOffSystemRefRanges As TestRefRangesDS, _
        '                                       ByVal pUpdatedOffSystemRefRanges As TestRefRangesDS, _
        '                                       ByVal pDeletedOffSystemRefRanges As TestRefRangesDS) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myDAO As New tparOffSystemTestsDAO

        '                If Not resultData.HasError Then
        '                    Dim myRefRangesDel As New TestRefRangesDelegate
        '                    If (pDeletedOffSystemRefRanges.tparTestRefRanges.Rows.Count > 0) Then
        '                        resultData = myRefRangesDel.Delete(dbConnection, pDeletedOffSystemRefRanges, "OFFS")
        '                    End If

        '                    If Not resultData.HasError Then
        '                        If pNewOffSystemRefRanges.tparTestRefRanges.Rows.Count > 0 Then
        '                            resultData = myRefRangesDel.Create(dbConnection, pNewOffSystemRefRanges, "OFFS")
        '                        End If

        '                        If Not resultData.HasError Then
        '                            If pUpdatedOffSystemRefRanges.tparTestRefRanges.Rows.Count > 0 Then
        '                                resultData = myRefRangesDel.Update(dbConnection, pUpdatedOffSystemRefRanges, "OFFS")
        '                            End If

        '                        End If
        '                    End If
        '                End If


        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    'resultData.SetDatos = <value to return; if any>
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.SaveOffSystemRefRanges", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' </summary>
        '''' <param name="pDbConnection"></param>
        '''' <param name="pNewOffSystemTests"></param>
        '''' <param name="pUpdatedOffSystemTests"></param>
        '''' <param name="pNewOffSystemTestSamples"></param>
        '''' <param name="pUpdatedOffSystemTestSamples"></param>
        '''' <param name="pNewOffSystemRefRanges"></param>
        '''' <param name="pUpdatedOffSystemRefRanges"></param>
        '''' <param name="pDeletedOffSystemRefRanges"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function SaveOffSystemTest(ByVal pDbConnection As SqlClient.SqlConnection, _
        '                                  ByVal pNewOffSystemTests As OffSystemTestsDS, _
        '                                  ByVal pUpdatedOffSystemTests As OffSystemTestsDS, _
        '                                  ByVal pNewOffSystemTestSamples As OffSystemTestSamplesDS, _
        '                                  ByVal pUpdatedOffSystemTestSamples As OffSystemTestSamplesDS, _
        '                                  ByVal pNewOffSystemRefRanges As TestRefRangesDS, _
        '                                  ByVal pUpdatedOffSystemRefRanges As TestRefRangesDS, _
        '                                  ByVal pDeletedOffSystemRefRanges As TestRefRangesDS) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim myDAO As New tparOffSystemTestsDAO

        '                'New offSystem tests
        '                If (pNewOffSystemTests.tparOffSystemTests.Rows.Count > 0) Then
        '                    resultData = Create(dbConnection, pNewOffSystemTests)
        '                End If

        '                If (Not resultData.HasError) Then '(1)
        '                    'Updated offSystem tests
        '                    If (pUpdatedOffSystemTests.tparOffSystemTests.Rows.Count > 0) Then
        '                        resultData = Modify(dbConnection, pUpdatedOffSystemTests)
        '                    End If

        '                    If Not resultData.HasError Then '(2)
        '                        'New offSystem test sample types
        '                        Dim myoffSystemSampleType As New OffSystemTestSamplesDelegate
        '                        If (pNewOffSystemTestSamples.tparOffSystemTestSamples.Rows.Count > 0) Then
        '                            resultData = myoffSystemSampleType.Add(dbConnection, pNewOffSystemTestSamples)
        '                        End If

        '                        If (Not resultData.HasError) Then '(3)
        '                            'Updated offSystem test sample types
        '                            If (pUpdatedOffSystemTestSamples.tparOffSystemTestSamples.Rows.Count > 0) Then
        '                                resultData = myoffSystemSampleType.Modify(dbConnection, pUpdatedOffSystemTestSamples)
        '                            End If

        '                            If Not resultData.HasError Then '(4)
        '                                Dim myRefRangesDel As New TestRefRangesDelegate
        '                                If (pDeletedOffSystemRefRanges.tparTestRefRanges.Rows.Count > 0) Then
        '                                    resultData = myRefRangesDel.Delete(dbConnection, pDeletedOffSystemRefRanges, "OFFS")
        '                                End If

        '                                If Not resultData.HasError Then '(5)
        '                                    If pNewOffSystemRefRanges.tparTestRefRanges.Rows.Count > 0 Then
        '                                        resultData = myRefRangesDel.Create(dbConnection, pNewOffSystemRefRanges, "OFFS")
        '                                    End If

        '                                    If Not resultData.HasError Then '(6)
        '                                        If pUpdatedOffSystemRefRanges.tparTestRefRanges.Rows.Count > 0 Then
        '                                            resultData = myRefRangesDel.Update(dbConnection, pUpdatedOffSystemRefRanges, "OFFS")
        '                                        End If

        '                                    End If '(6)
        '                                End If '(5)
        '                            End If '(4)
        '                        End If '(3)
        '                    End If '(2)
        '                End If '(1)

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    'resultData.SetDatos = <value to return; if any>
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.Add", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Update a offsystem test (affect to tparoffsystemtest,  tparoffsystemtestsamples, tpartestrefranges)
        '''' </summary>
        '''' <param name="pDbConnection"></param>
        '''' <param name="pUpdatedOffSystemTests"></param>
        '''' <param name="pUpdatedOffSystemTestSamples"></param>
        '''' <param name="pNewOffSystemRefRanges"></param>
        '''' <returns></returns>
        '''' <remarks>
        '''' Created by DL: 29/11/2010
        '''' </remarks>
        'Public Function ModifyOffSystemTest(ByVal pDbConnection As SqlClient.SqlConnection, _
        '                                    ByVal pUpdatedOffSystemTests As OffSystemTestsDS, _
        '                                    ByVal pUpdatedOffSystemTestSamples As OffSystemTestSamplesDS, _
        '                                    ByVal pNewOffSystemRefRanges As TestRefRangesDS) As GlobalDataTO

        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection
        '    Try
        '        resultData = DAOBase.GetOpenDBTransaction(pDbConnection)
        '        If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then

        '                'Updated offSystem tests
        '                If (pUpdatedOffSystemTests.tparOffSystemTests.Rows.Count > 0) Then
        '                    resultData = Modify(dbConnection, pUpdatedOffSystemTests)

        '                    If (Not resultData.HasError) Then
        '                        'Updated offSystem test sample types

        '                        If (pUpdatedOffSystemTestSamples.tparOffSystemTestSamples.Rows.Count > 0) Then
        '                            Dim myoffSystemTestSampleDelegate As New OffSystemTestSamplesDelegate
        '                            resultData = myoffSystemTestSampleDelegate.Modify(dbConnection, pUpdatedOffSystemTestSamples)
        '                        End If
        '                    End If
        '                End If

        '                If (Not resultData.HasError) Then
        '                    'When the Database Connection was opened locally, then the Commit is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
        '                    'resultData.SetDatos = <value to return; if any>
        '                Else
        '                    'When the Database Connection was opened locally, then the Rollback is executed
        '                    If (pDbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
        '                End If
        '            End If
        '        End If
        '    Catch ex As Exception
        '        'When the Database Connection was opened locally, then the Rollback is executed
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.SaveNewOffSystemTest", EventLogEntryType.Error, False)
        '    Finally
        '        If (pDbConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try
        '    Return resultData
        'End Function

        '''' <summary>
        '''' Get the offsystem by the shorname.
        '''' </summary>
        '''' <param name="pDBConnection"></param>
        '''' <param name="pShortName"></param>
        '''' <returns></returns>
        '''' <remarks>
        '''' Created BY: DL 30/11/2010
        '''' </remarks>
        'Public Function ReadByShortName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pShortName As String) As GlobalDataTO
        '    Dim resultData As New GlobalDataTO
        '    Dim dbConnection As New SqlClient.SqlConnection

        '    Try
        '        resultData = DAOBase.GetOpenDBConnection(pDBConnection)
        '        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
        '            dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
        '            If (Not dbConnection Is Nothing) Then
        '                Dim mytparOffSystemTestsDAO As New tparOffSystemTestsDAO
        '                resultData = mytparOffSystemTestsDAO.ReadByShortName(dbConnection, pShortName)
        '            End If
        '        End If

        '    Catch ex As Exception
        '        resultData.HasError = True
        '        resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
        '        resultData.ErrorMessage = ex.Message

        '        Dim myLogAcciones As New ApplicationLogManager()
        '        myLogAcciones.CreateLogActivity(ex.Message, "OffSystemTestsDelegate.ReadByShortName", EventLogEntryType.Error, False)

        '    Finally
        '        If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
        '    End Try

        '    Return resultData
        'End Function
#End Region
    End Class

End Namespace
