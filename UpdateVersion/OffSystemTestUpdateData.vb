Option Explicit On
Option Strict On

Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.DAL.DAO

Namespace Biosystems.Ax00.BL.UpdateVersion
    Public Class OffSystemTestUpdateData
        ''' <summary>
        ''' Search all OFFS Tests in FACTORY DB that do not exist in Customer DB and, for each one of them, execute the process of adding it to Customer DB
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection></param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 20/11/2014 - BA-2105
        ''' </remarks>
        Public Function CREATENewOFFSTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myNewOFFSTestDS As New OffSystemTestsDS
                Dim myNewOFFSTestSampleDS As New OffSystemTestSamplesDS

                Dim myNewOFFSTestsList As New OffSystemTestsDS
                Dim myOFFSTestUpdateDAO As New OffSystemTestUpdateDAO
                Dim myOFFSTestsDelegate As New OffSystemTestsDelegate
                Dim myUpdateVersionAddedElementsRow As UpdateVersionChangesDS.AddedElementsRow

                '(1) Search in Factory DB all new OFFS TESTS
                myGlobalDataTO = myOFFSTestUpdateDAO.GetNewFactoryTests(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myNewOFFSTestsList = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)

                    '(2) Process each new OFFS Test in Factory DB to add it to Customer DB
                    For Each newTest As OffSystemTestsDS.tparOffSystemTestsRow In myNewOFFSTestsList.tparOffSystemTests
                        '(2.1) Get all data in tables tparOffSystemTests and tparOffSystemTestSamples in Factory DB
                        myGlobalDataTO = myOFFSTestUpdateDAO.GetDataInFactoryDB(pDBConnection, newTest.BiosystemsID, True)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myNewOFFSTestDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)
                        End If

                        '(2.2) Verify if there is an User OFFS Test in Customer DB with the same Name and/or ShortName of the new Factory OFFS Test, and in this case, 
                        '      rename the User OFFS Test 
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = UpdateRenamedTest(pDBConnection, myNewOFFSTestDS.tparOffSystemTests.First.Name, _
                                                               myNewOFFSTestDS.tparOffSystemTests.First.ShortName, pUpdateVersionChangesList)
                        End If

                        '(2.3) Save the NEW OFFS Test in CUSTOMER DB 
                        If (Not myGlobalDataTO.HasError) Then
                            'Fill the OffSystemTestSamplesDS needed for the Add
                            myNewOFFSTestSampleDS.Clear()
                            Dim myOFFSTestSampleRow As OffSystemTestSamplesDS.tparOffSystemTestSamplesRow
                            myOFFSTestSampleRow = myNewOFFSTestSampleDS.tparOffSystemTestSamples.NewtparOffSystemTestSamplesRow()
                            myOFFSTestSampleRow.SampleType = myNewOFFSTestDS.tparOffSystemTests.First.SampleType
                            myOFFSTestSampleRow.SetDefaultValueNull()
                            If (Not myNewOFFSTestDS.tparOffSystemTests.First.IsDefaultValueNull) Then myOFFSTestSampleRow.DefaultValue = myNewOFFSTestDS.tparOffSystemTests.First.DefaultValue
                            myOFFSTestSampleRow.SetActiveRangeTypeNull()
                            myNewOFFSTestSampleDS.tparOffSystemTestSamples.AddtparOffSystemTestSamplesRow(myOFFSTestSampleRow)

                            'Add the new Factory OFFS Test/SampleType to Customer DB
                            myGlobalDataTO = myOFFSTestsDelegate.Add(pDBConnection, myNewOFFSTestDS, myNewOFFSTestSampleDS, New TestRefRangesDS)
                        End If

                        '(2.4) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table AddedElements) 
                        '      for the new Factory OFFS Test
                        If (Not myGlobalDataTO.HasError) Then
                            myUpdateVersionAddedElementsRow = pUpdateVersionChangesList.AddedElements.NewAddedElementsRow
                            myUpdateVersionAddedElementsRow.ElementType = "OFFS"
                            myUpdateVersionAddedElementsRow.ElementName = myNewOFFSTestDS.tparOffSystemTests.First.Name & " (" & myNewOFFSTestDS.tparOffSystemTests.First.ShortName & ")"
                            myUpdateVersionAddedElementsRow.SampleType = myNewOFFSTestDS.tparOffSystemTests.First.SampleType
                            pUpdateVersionChangesList.AddedElements.AddAddedElementsRow(myUpdateVersionAddedElementsRow)
                            pUpdateVersionChangesList.AddedElements.AcceptChanges()
                        End If

                        'If an error has been raised, then the process is finished
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("OFFS Test Update Error", "OffSystemTestUpdateData.CREATENewOFFSTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search all OFFS Tests that should be deleted from CUSTOMER DB (those preloaded OFFS Tests that exist in CUSTOMER DB but not in FACTORY DB) and 
        ''' remove them from CUSTOMER DB 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 20/11/2014 - BA-2105
        ''' </remarks>
        Public Function DELETERemovedOFFSTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim myDeletedOFFSTestDS As New OffSystemTestsDS
                Dim myRemovedOFFSTestsDS As New OffSystemTestsDS
                Dim myOFFSTestsDelegate As New OffSystemTestsDelegate
                Dim myOFFSTestUpdateDAO As New OffSystemTestUpdateDAO
                Dim myUpdateVersionDeletedElementsRow As UpdateVersionChangesDS.DeletedElementsRow

                '(1) Search in Customer DB all Preloaded OFFS TESTS that do not exist in Factory DB:
                myGlobalDataTO = myOFFSTestUpdateDAO.GetDeletedPreloadedOFFSTests(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myRemovedOFFSTestsDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)

                    '(2) Process each returned OFFS Test to delete it from CUSTOMER DB
                    For Each removedOFFSTest As OffSystemTestsDS.tparOffSystemTestsRow In myRemovedOFFSTestsDS.tparOffSystemTests
                        '(2.1) In CUSTOMER DB, search data of the OFF Test to delete 
                        myGlobalDataTO = myOFFSTestsDelegate.Read(pDBConnection, removedOFFSTest.BiosystemsID, True)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myDeletedOFFSTestDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)

                            If (myDeletedOFFSTestDS.tparOffSystemTests.Rows.Count = 0) Then
                                'This case is not possible...
                                myGlobalDataTO.HasError = True
                            End If
                        End If

                        '(2.2) Delete the OFFS Test from CUSTOMER DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myOFFSTestsDelegate.Delete(pDBConnection, myDeletedOFFSTestDS)
                        End If

                        '(2.3) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table DeletedElements) 
                        '      for the deleted OFFS Test
                        If (Not myGlobalDataTO.HasError) Then
                            myUpdateVersionDeletedElementsRow = pUpdateVersionChangesList.DeletedElements.NewDeletedElementsRow
                            myUpdateVersionDeletedElementsRow.ElementType = "OFFS"
                            myUpdateVersionDeletedElementsRow.ElementName = myDeletedOFFSTestDS.tparOffSystemTests.First.Name & " (" & myDeletedOFFSTestDS.tparOffSystemTests.First.ShortName & ")"
                            myUpdateVersionDeletedElementsRow.SampleType = myDeletedOFFSTestDS.tparOffSystemTests.First.SampleType
                            pUpdateVersionChangesList.DeletedElements.AddDeletedElementsRow(myUpdateVersionDeletedElementsRow)
                            pUpdateVersionChangesList.DeletedElements.AcceptChanges()
                        End If

                        'If an error has been raised, then the process is stopped
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("OFFS Test Update Error", "OffSystemTestUpdateData.DELETERemovedOFFSTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Execute the process to search in FACTORY DB all OFFS TESTS that exists in CUSTOMER DB but for which at least one of the relevant Test fields 
        ''' have changed and modify data in CUSTOMER DB (tables tparOffSystemTests, tparOffSystemTestSamples and tparTestRefRanges)
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 20/11/2014 - BA-2105
        ''' </remarks>
        Public Function UPDATEModifiedOFFSTests(ByVal pDBConnection As SqlClient.SqlConnection, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO

            Try
                Dim sampleTypeChanged As Boolean = False
                Dim testRefRangesDS As New TestRefRangesDS
                Dim myTestRefRangesDelegate As New TestRefRangesDelegate
                Dim myTestProfileDelegate As New TestProfilesDelegate
                Dim myCalcTestsDelegate As New CalculatedTestsDelegate
                Dim myCustomerOFFSTestDS As New OffSystemTestsDS
                Dim myOFFSTestSampleDS As New OffSystemTestSamplesDS
                Dim myFactoryUpdatedOFFSTestDS As New OffSystemTestsDS
                Dim myOFFSTestsDelegate As New OffSystemTestsDelegate
                Dim myOFFSTestUpdateDAO As New OffSystemTestUpdateDAO

                '(1)  Search in FACTORY DB all OFFS Tests that exist in CUSTOMER DB but for which at least one of the relevant fields have been changed
                myGlobalDataTO = myOFFSTestUpdateDAO.GetUpdatedFactoryTests(pDBConnection)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myFactoryUpdatedOFFSTestDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)

                    'Process each returned OFFS Test to update it in CUSTOMER DB
                    For Each updatedOFFSTest As OffSystemTestsDS.tparOffSystemTestsRow In myFactoryUpdatedOFFSTestDS.tparOffSystemTests
                        '(2) Get current data of the OFFS Test in CUSTOMER DB
                        myGlobalDataTO = myOFFSTestsDelegate.Read(pDBConnection, updatedOFFSTest.BiosystemsID, True)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            myCustomerOFFSTestDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)

                            'This case is not possible...
                            If (myCustomerOFFSTestDS.tparOffSystemTests.Rows.Count = 0) Then myGlobalDataTO.HasError = True
                        End If

                        '(3) Get all Reference Ranges defined for the OFFS Test/SampleType in CUSTOMER DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = myTestRefRangesDelegate.ReadByTestID(pDBConnection, myCustomerOFFSTestDS.tparOffSystemTests.First.OffSystemTestID, _
                                                                                  myCustomerOFFSTestDS.tparOffSystemTests.First.SampleType, String.Empty, "OFFS")
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                testRefRangesDS = DirectCast(myGlobalDataTO.SetDatos, TestRefRangesDS)
                            End If
                        End If

                        '(4) Update modified fields in the DS containing data of the OffSystem Test in Customer DB
                        If (Not myGlobalDataTO.HasError) Then
                            myGlobalDataTO = UpdateCustomerTest(updatedOFFSTest, myCustomerOFFSTestDS.tparOffSystemTests.First, testRefRangesDS, pUpdateVersionChangesList)
                            If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                                sampleTypeChanged = Convert.ToBoolean(myGlobalDataTO.SetDatos)
                                myCustomerOFFSTestDS.AcceptChanges()

                                '(4.1) Fill the OffSystemTestSamplesDS needed for the Add
                                Dim myOFFSTestSampleRow As OffSystemTestSamplesDS.tparOffSystemTestSamplesRow
                                myOFFSTestSampleRow = myOFFSTestSampleDS.tparOffSystemTestSamples.NewtparOffSystemTestSamplesRow()
                                myOFFSTestSampleRow.SampleType = myCustomerOFFSTestDS.tparOffSystemTests.First.SampleType
                                myOFFSTestSampleRow.SetDefaultValueNull()
                                If (Not myCustomerOFFSTestDS.tparOffSystemTests.First.IsDefaultValueNull) Then myOFFSTestSampleRow.DefaultValue = myCustomerOFFSTestDS.tparOffSystemTests.First.DefaultValue
                                myOFFSTestSampleRow.SetActiveRangeTypeNull()
                                myOFFSTestSampleDS.tparOffSystemTestSamples.ImportRow(myOFFSTestSampleRow)

                                '(4.2) If the SampleType has changed, delete the OFFS Test/SampleType from all Test Profiles in which it is included 
                                myGlobalDataTO = myTestProfileDelegate.DeleteByTestIDSampleType(pDBConnection, myCustomerOFFSTestDS.tparOffSystemTests(0).OffSystemTestID, "", "OFFS")

                                '(4.3) If the SampleType has changed, delete all Calculated Tests in which the OFFS Test/SampleType is included
                                If (Not myGlobalDataTO.HasError) Then
                                    myGlobalDataTO = myCalcTestsDelegate.DeleteCalculatedTestbyTestID(pDBConnection, myCustomerOFFSTestDS.tparOffSystemTests(0).OffSystemTestID, "", "OFFS")
                                End If

                                '(4.4.) Finally, update the OFFS Test/SampleType in Customer DB
                                myGlobalDataTO = myOFFSTestsDelegate.Modify(pDBConnection, myCustomerOFFSTestDS, myOFFSTestSampleDS, testRefRangesDS, New DependenciesElementsDS, True)
                            End If
                        End If
                       
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If

            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("OFFS Test Update Error", "OffSystemTestUpdateData.UPDATEModifiedOFFSTests", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' For a CALC Test, compare value of relevant fields in table tparCalculatedTests in CUSTOMER DB with value of the same fields in FACTORY DB, 
        ''' and update in CUSTOMER DB all modified fields.
        ''' </summary>
        ''' <param name="pFactoryOFFSTestRow">Row of OffSystemTestsDS containing data of the OFFS Test in FACTORY DB</param>
        ''' <param name="pCustomerOFFSTestRow">Row of OffSystemTestsDS containing data of the OFFS Test in CUSTOMER DB</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing a Boolean value: when TRUE, it means the SampleType of the OFFS Test has changed</returns>
        ''' <remarks>
        ''' Created by:  SA 20/11/2014 - BA-2105
        ''' </remarks>
        Private Function UpdateCustomerTest(ByVal pFactoryOFFSTestRow As OffSystemTestsDS.tparOffSystemTestsRow, ByVal pCustomerOFFSTestRow As OffSystemTestsDS.tparOffSystemTestsRow, _
                                            ByRef pTestRefRangesDS As TestRefRangesDS, ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim sampleTypeChanged As Boolean = False

            Try
                'Build the TestName to report changes...
                Dim myOFFSTestName As String = pCustomerOFFSTestRow.Name & " (" & pCustomerOFFSTestRow.ShortName & ") "

                'Verify if field SampleType has changed
                If (pFactoryOFFSTestRow.SampleType <> pCustomerOFFSTestRow.SampleType) Then
                    sampleTypeChanged = True

                    'Add a row for field SampleType in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "OFFS", myOFFSTestName, pFactoryOFFSTestRow.SampleType, "SampleType", pCustomerOFFSTestRow.SampleType, _
                                                        pFactoryOFFSTestRow.SampleType)

                    'Update field in the Customer DataSet 
                    pCustomerOFFSTestRow.SampleType = pFactoryOFFSTestRow.SampleType

                    'Update field SampleType for all References Ranges
                    For Each refRange As TestRefRangesDS.tparTestRefRangesRow In pTestRefRangesDS.tparTestRefRanges
                        refRange.BeginEdit()
                        refRange.SampleType = pFactoryOFFSTestRow.SampleType
                        refRange.EndEdit()
                    Next
                    pTestRefRangesDS.AcceptChanges()
                End If

                'Verify if field DefaultValue has changed
                Dim myDefaultValue As String = "--"
                If (Not pFactoryOFFSTestRow.IsDefaultValueNull) Then
                    If (pCustomerOFFSTestRow.IsDefaultValueNull OrElse pCustomerOFFSTestRow.DefaultValue <> pFactoryOFFSTestRow.DefaultValue) Then
                        If (Not pCustomerOFFSTestRow.IsDefaultValueNull AndAlso pCustomerOFFSTestRow.DefaultValue <> String.Empty) Then myDefaultValue = pCustomerOFFSTestRow.DefaultValue

                        'Add a row for field DefaultValue in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "OFFS", myOFFSTestName, pFactoryOFFSTestRow.SampleType, "DefaultValue", myDefaultValue, _
                                                            pFactoryOFFSTestRow.DefaultValue)

                        'Update field in the Customer DataSet
                        pCustomerOFFSTestRow.DefaultValue = pFactoryOFFSTestRow.DefaultValue
                    End If
                Else
                    If (Not pCustomerOFFSTestRow.IsSampleTypeNull) Then
                        'Add a row for field DefaultValue in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "OFFS", myOFFSTestName, pFactoryOFFSTestRow.SampleType, "DefaultValue", pCustomerOFFSTestRow.DefaultValue, _
                                                            myDefaultValue)

                        'Update field in the Customer DataSet 
                        pCustomerOFFSTestRow.SetSampleTypeNull()
                    End If
                End If

                'Verify if field ResultType has changed
                If (pFactoryOFFSTestRow.ResultType <> pCustomerOFFSTestRow.ResultType) Then
                    'Add a row for field ResultType in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                    AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "OFFS", myOFFSTestName, pFactoryOFFSTestRow.SampleType, "ResultType", pCustomerOFFSTestRow.ResultType, _
                                                        pFactoryOFFSTestRow.ResultType)

                    If (pFactoryOFFSTestRow.ResultType = "QUANTIVE") Then
                        'ResultType has changed from QUALITATIVE to QUANTITATIVE

                        'Add a row for field Decimals in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        'and update field in the Customer DataSet
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "OFFS", myOFFSTestName, pFactoryOFFSTestRow.SampleType, "Decimals", "--", _
                                                            pFactoryOFFSTestRow.Decimals.ToString)
                        pCustomerOFFSTestRow.Decimals = pFactoryOFFSTestRow.Decimals

                        If (Not pFactoryOFFSTestRow.IsUnitsNull) Then
                            'Add a row for field Units in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                            'and update field in the Customer DataSet
                            AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "OFFS", myOFFSTestName, pFactoryOFFSTestRow.SampleType, "Units", "--", _
                                                               pFactoryOFFSTestRow.Units)
                            pCustomerOFFSTestRow.Units = pFactoryOFFSTestRow.Units
                        End If

                    ElseIf (pFactoryOFFSTestRow.ResultType = "QUALTIVE") Then
                        'ResultType has changed from QUANTITATIVE to QUALITATIVE - Decimals and Units fields are NULL for QUALITATIVE OFFS Tests 

                        'Add a row for field Decimals in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                        'and update field in the Customer DataSet
                        AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "OFFS", myOFFSTestName, pFactoryOFFSTestRow.SampleType, "Decimals", pCustomerOFFSTestRow.Decimals.ToString, "--")
                        pCustomerOFFSTestRow.SetDecimalsNull()

                        If (Not pCustomerOFFSTestRow.IsUnitsNull) Then
                            'Add a row for field Units in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                            'and update field in the Customer DataSet
                            AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "OFFS", myOFFSTestName, pFactoryOFFSTestRow.SampleType, "Units", _
                                                               pCustomerOFFSTestRow.Units, "--")
                            pCustomerOFFSTestRow.SetUnitsNull()
                        End If

                        'Set to Null field ActiveRangeType and set to TRUE the boolean value to return (to indicate to the caller function the Reference Ranges for
                        'the OffSystem Test have to be deleted
                        If (Not pCustomerOFFSTestRow.IsActiveRangeTypeNull) Then
                            'Add a row for field ActiveRangeType in the DS containing all changes in Customer DB due to the Update Version Process (sub-table UpdatedElements) 
                            'and update field in the Customer DataSet
                            AddUpdatedElementToChangesStructure(pUpdateVersionChangesList, "OFFS", myOFFSTestName, pFactoryOFFSTestRow.SampleType, "ActiveRangeType", _
                                                               pCustomerOFFSTestRow.ActiveRangeType, "--")
                            pCustomerOFFSTestRow.SetActiveRangeTypeNull()
                        End If
                    End If
                End If

                myGlobalDataTO.SetDatos = sampleTypeChanged
                myGlobalDataTO.HasError = False
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("CALC Test Update Error", "CalculatedTestUpdateData.UpdateCustomerTest", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Verify if there is an User OFFS Test in Customer DB with the same Name and/or ShortName of a new Factory OFFS Test, and in this case, rename
        ''' the User Test by adding as many "R" letters at the beginning of the Name and ShortName as needed until get an unique Name and ShortName
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestName">Name of the new Factory OFFS Test to verify</param>
        ''' <param name="pShortName">ShortName of the new Factory OFFS Test to verify</param>
        ''' <param name="pUpdateVersionChangesList">Global structure to save all changes executed by the Update Version process in Customer DB</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by: SA 20/11/2014 - BA-2105
        ''' </remarks>
        Private Function UpdateRenamedTest(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestName As String, ByVal pShortName As String, _
                                           ByRef pUpdateVersionChangesList As UpdateVersionChangesDS) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myDuplicatedNameOFFSTestID As Integer = 0
                Dim myOFFSTestsDS As New OffSystemTestsDS
                Dim myAuxOFFSTestsDS As New OffSystemTestsDS
                Dim myOFFSTestsDelegate As New OffSystemTestsDelegate
                Dim myHistOFFSTestsDAO As New thisOffSystemTestSamplesDAO
                Dim myUpdateVersionRenamedElementsRow As UpdateVersionChangesDS.RenamedElementsRow

                '(1) Search if there is an User Test with the same Name...
                myGlobalDataTO = myOFFSTestsDelegate.ExistsOffSystemTest(pDBConnection, pTestName, "FNAME", 0, True)
                If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                    myOFFSTestsDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)

                    If (myOFFSTestsDS.tparOffSystemTests.Rows.Count > 0) Then
                        myDuplicatedNameOFFSTestID = myOFFSTestsDS.tparOffSystemTests.First.OffSystemTestID
                    End If
                End If

                '(2) Search if there is an User Test with the same ShortName (excluding the OffSystem Test with the same Name 
                '    found in the previous step)
                If (Not myGlobalDataTO.HasError) Then
                    myGlobalDataTO = myOFFSTestsDelegate.ExistsOffSystemTest(pDBConnection, pShortName, "NAME", myDuplicatedNameOFFSTestID, True)
                    If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                        myAuxOFFSTestsDS = DirectCast(myGlobalDataTO.SetDatos, OffSystemTestsDS)
                    End If
                End If

                '(3) Merge results of both searches
                If (Not myGlobalDataTO.HasError) Then
                    If (myOFFSTestsDS.tparOffSystemTests.Rows.Count > 0 AndAlso myAuxOFFSTestsDS.tparOffSystemTests.Rows.Count > 0) Then
                        If (myOFFSTestsDS.tparOffSystemTests.First.OffSystemTestID <> myAuxOFFSTestsDS.tparOffSystemTests.First.OffSystemTestID) Then
                            myOFFSTestsDS.tparOffSystemTests.ImportRow(myAuxOFFSTestsDS.tparOffSystemTests.First)
                        End If
                    ElseIf (myOFFSTestsDS.tparOffSystemTests.Rows.Count = 0 AndAlso myAuxOFFSTestsDS.tparOffSystemTests.Rows.Count > 0) Then
                        myOFFSTestsDS.tparOffSystemTests.ImportRow(myAuxOFFSTestsDS.tparOffSystemTests.First)
                    End If
                End If

                '(4) Process each one of the OFFS Tests found
                If (Not myGlobalDataTO.HasError) Then
                    For Each offsTestRow As OffSystemTestsDS.tparOffSystemTestsRow In myOFFSTestsDS.tparOffSystemTests
                        'Rename the OFFS Test (add as many "R" letters as needed at the beginning of Name and ShortName)
                        myGlobalDataTO = RenameOFFSTestName(pDBConnection, offsTestRow)
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            If (Convert.ToBoolean(myGlobalDataTO.SetDatos)) Then
                                '(4.1) Update the renamed OFFS Test in Customer DB
                                myAuxOFFSTestsDS.Clear()
                                myAuxOFFSTestsDS.tparOffSystemTests.ImportRow(offsTestRow)
                                myAuxOFFSTestsDS.AcceptChanges()

                                myGlobalDataTO = myOFFSTestsDelegate.UpdateTestNames(pDBConnection, myAuxOFFSTestsDS)

                                '(4.2) Add a row in the global DS containing all changes in Customer DB due to the Update Version Process (sub-table RenamedElements)
                                If (Not myGlobalDataTO.HasError) Then
                                    myUpdateVersionRenamedElementsRow = pUpdateVersionChangesList.RenamedElements.NewRenamedElementsRow
                                    myUpdateVersionRenamedElementsRow.ElementType = "OFFS"
                                    myUpdateVersionRenamedElementsRow.PreviousName = pTestName & " (" & pShortName & ")"
                                    myUpdateVersionRenamedElementsRow.UpdatedName = offsTestRow.Name & " (" & offsTestRow.ShortName & ")"
                                    pUpdateVersionChangesList.RenamedElements.AddRenamedElementsRow(myUpdateVersionRenamedElementsRow)
                                    pUpdateVersionChangesList.RenamedElements.AcceptChanges()
                                End If
                            Else
                                'If it was not possible to rename the Test (really unlikely case), it is considered an error
                                myGlobalDataTO.HasError = True
                            End If
                        End If

                        'If an error has been raised, then the process is finished
                        If (myGlobalDataTO.HasError) Then Exit For
                    Next
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("OFFS Test Update Error", "OffSystemTestUpdateData.UpdateRenamedTest", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add "R" letters at the beginning of an OFFS Test Name and ShortName until get an unique Name/ShortName.
        ''' Used during Update Version process when a new Factory OFFS Test is added and there is an User Test in 
        ''' Customer DB with the same Test Name and/or ShortName of the one to add
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pOFFSTestsRow">Row of OffSystemTestsDS containing the basic data of the OFFS Test to rename</param>
        ''' <returns>GlobalDataTO containing a Boolean value indicating if the OFFS Test has been renamed (when TRUE)</returns>
        ''' <remarks>
        ''' Created by:  SA 20/11/2014 - BA-2105
        ''' </remarks>
        Private Function RenameOFFSTestName(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pOFFSTestsRow As OffSystemTestsDS.tparOffSystemTestsRow) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Try
                Dim myOFFSTestDS As New OffSystemTestsDS
                Dim myOFFSTestsDelegate As New OffSystemTestsDelegate

                Dim isValidNewName As Boolean = False
                Dim isValidTestName As Boolean = False
                Dim isValidShortName As Boolean = False

                Dim numOfRs As Integer = 1
                Dim errorFound As Boolean = False
                Dim myValidTestName As String = pOFFSTestsRow.Name
                Dim myValidShortName As String = pOFFSTestsRow.ShortName

                While (Not isValidNewName AndAlso numOfRs < 16 AndAlso Not errorFound)
                    If (Not isValidTestName) Then
                        'Add an "R" at the beginning of the Test Name
                        myValidTestName = "R" & myValidTestName

                        'If the length of the new Test Name is greater than 16, remove the last character
                        If (myValidTestName.Length > 16) Then myValidTestName = myValidTestName.Remove(myValidTestName.Length - 1)

                        'Verify if the new Test Name is unique in Customer DB (there is not another Test with the same Name)
                        myGlobalDataTO = myOFFSTestsDelegate.ExistsOffSystemTest(pDBConnection, myValidTestName, "FNAME")
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            isValidTestName = (Not Convert.ToBoolean(myGlobalDataTO.SetDatos))
                        Else
                            errorFound = True
                        End If
                    End If

                    If (Not isValidShortName) Then
                        'Add an "R" at the beginning of the Test Short Name
                        myValidShortName = "R" & myValidShortName

                        'If the length of the new Test Short Name is greater than 8, remove the last character
                        If (myValidShortName.Length > 8) Then myValidShortName = myValidShortName.Remove(myValidShortName.Length - 1)

                        'Verify if the new Test Short Name is unique in Customer DB (there is not another Test with the same Short Name)
                        myGlobalDataTO = myOFFSTestsDelegate.ExistsOffSystemTest(pDBConnection, myValidShortName, "NAME")
                        If (Not myGlobalDataTO.HasError AndAlso Not myGlobalDataTO.SetDatos Is Nothing) Then
                            isValidShortName = (Not Convert.ToBoolean(myGlobalDataTO.SetDatos))
                        Else
                            errorFound = True
                        End If
                    End If

                    'The rename is accepted with the rename of both fields Name and ShortName is accepted
                    isValidNewName = (isValidShortName AndAlso isValidTestName)

                    'Just to avoid the very unlikely probability of endless loop 
                    numOfRs += 1
                End While

                'Finally, update the name in the OffSystemTestsDS row received as entry parameter
                If (isValidNewName) Then
                    pOFFSTestsRow.BeginEdit()
                    pOFFSTestsRow.Name = myValidTestName
                    pOFFSTestsRow.ShortName = myValidShortName
                    pOFFSTestsRow.EndEdit()
                End If

                myGlobalDataTO.SetDatos = isValidNewName
                myGlobalDataTO.HasError = errorFound
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("OFFS Test Update Error", "OffSystemTestUpdateData.RenameTestName", EventLogEntryType.Error, False)
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Add a new row with data of an updated element in the DS containing all changes in CUSTOMER DB due to the Update Version Process.
        ''' Data is added in sub-table UpdatedElements
        ''' </summary>
        ''' <param name="pUpdateVersionChangesList">DataSet containing all changes made in CUSTOMER DB for the Update Version Process</param>
        ''' <param name="pElementType">Type of affected Element: OFFS</param>
        ''' <param name="pElementName">Name of the affected Element</param>
        ''' <param name="pSampleType">Sample Type of the affected Element</param>
        ''' <param name="pUpdatedField">Name of the affected field</param>
        ''' <param name="pPreviousValue">Previous value of the field in CUSTOMER DB</param>
        ''' <param name="pNewValue">New value of the field in CUSTOMER DB (from FACTORY DB)</param>
        ''' <remarks>
        ''' Created by:  SA 20/11/2014 - BA-2105
        ''' </remarks>
        Private Sub AddUpdatedElementToChangesStructure(ByRef pUpdateVersionChangesList As UpdateVersionChangesDS, ByVal pElementType As String, _
                                                        ByVal pElementName As String, ByVal pSampleType As String, ByVal pUpdatedField As String, ByVal pPreviousValue As String, _
                                                        ByVal pNewValue As String)
            Try
                Dim myUpdateVersionChangedElementsRow As UpdateVersionChangesDS.UpdatedElementsRow
                myUpdateVersionChangedElementsRow = pUpdateVersionChangesList.UpdatedElements.NewUpdatedElementsRow
                myUpdateVersionChangedElementsRow.ElementType = pElementType
                myUpdateVersionChangedElementsRow.ElementName = pElementName
                myUpdateVersionChangedElementsRow.SampleType = pSampleType
                myUpdateVersionChangedElementsRow.UpdatedField = pUpdatedField
                myUpdateVersionChangedElementsRow.PreviousValue = pPreviousValue
                myUpdateVersionChangedElementsRow.NewValue = pNewValue
                pUpdateVersionChangesList.UpdatedElements.AddUpdatedElementsRow(myUpdateVersionChangedElementsRow)

            Catch ex As Exception
                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity("OFFS Test Update Error", "CalculatedTestUpdateData.AddUpdatedElementToChangesStructure", EventLogEntryType.Error, False)
                Throw
            End Try
        End Sub
    End Class
End Namespace
