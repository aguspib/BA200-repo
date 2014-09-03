Option Strict On
Option Explicit On

Imports Biosystems.Ax00.Types
Imports Biosystems.Ax00.DAL
Imports Biosystems.Ax00.DAL.DAO
Imports Biosystems.Ax00.Global
Imports Biosystems.Ax00.BL.Framework

Namespace Biosystems.Ax00.BL

    Public Class TestProfilesDelegate

#Region "Public Methods"
        ''' <summary>
        ''' Get the list of all Test Profiles defined for the specified SampleType plus the list of Tests included in each one
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pSampleType">Sample Type Code</param>
        ''' <param name="pCustomizedTestSelection">FALSE same order as until 3.0.2 / When TRUE the test are filtered by Available and order by CustomPosition ASC</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfilesTestsDS with the list of all Test Profiles defined
        '''          for the specified SampleType plus the list of Tests included in each one</returns>
        ''' <remarks>
        ''' Created by:  TR 05/02/2010
        ''' AG 01/09/2014 BA-1869 EUA can customize the test selection visibility and order in test keyboard auxiliary screen
        ''' </remarks>
        Public Function GetProfilesBySampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pSampleType As String, ByVal pCustomizedTestSelection As Boolean) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = CType(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfilesDAO As New TparTestProfilesDAO()
                        myGlobalDataTO = myTestProfilesDAO.GetProfilesBySampleType(dbConnection, pSampleType, pCustomizedTestSelection) 'AG 01/09/2014 - BA-1869 inform new parameter
                    End If
                End If
            Catch ex As Exception
                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.GetProfilesBySampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Get list of defined Test Profiles sorted by Position
        ''' </summary>
        ''' <returns>GlobalDataTO containing a typed Dataset TestProfilesDS containing the list of existing Test Profiles 
        '''          plus the path of the Icon defined for Test Profiles</returns>
        ''' <remarks>
        ''' Created by:  SA  
        ''' Modified by: GDS 15/05/2010 - For Test Profiles currently InUse, show a different ICON
        ''' </remarks>
        Public Function GetListByPosition(ByVal pDBConnection As SqlClient.SqlConnection) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get list of defined Test Profiles
                        Dim myTestProfilesDAO As New TparTestProfilesDAO
                        resultData = myTestProfilesDAO.ReadAll(dbConnection, "TestProfilePosition")

                        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                            Dim testProfilesData As New TestProfilesDS
                            testProfilesData = DirectCast(resultData.SetDatos, TestProfilesDS)

                            'Set the proper Icon for the
                            For Each testProfileRow As TestProfilesDS.tparTestProfilesRow In testProfilesData.tparTestProfiles.Rows
                                testProfileRow.BeginEdit()
                                testProfileRow.IconPath = IIf(testProfileRow.InUse, "INUSETPROF", "TPROFILES").ToString
                                testProfileRow.EndEdit()
                            Next
                            resultData.SetDatos = testProfilesData
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.GetListByPosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get the list of Tests included or not included in the specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileID">Test Profile Identifier. It can be zero when the function is used to get 
        '''                              the list of selectable Tests for a new Test Profile</param>
        ''' <param name="pTestsInProfile">When True, get Tests included in the Profile; when False, get Tests
        '''                               not included in the Profile</param>
        ''' <param name="pSampleType">Optional parameter. Sample Type Code to filter Tests that can be selected 
        '''                           for a Test Profile</param>
        ''' <returns>GlobalDataTO containing a typed Dataset with the list of Tests included in the 
        '''          specified Test Profile</returns>
        ''' <remarks>
        ''' Modified by: SA 27/06/2010 - Changed the way of getting the Icon Path for System and User Tests
        '''              DL 14/10/2010 - Get also data of Calculated and ISE Tests included in the specified Profile
        '''              SA 19/10/2010 - Removed the FOR, function ReadByTestProfileID now return all data of the Tests 
        '''                              in the Profile
        '''              SA 20/10/2010 - Added new parameter pTestsInProfile to get Tests included or not included in the Profile;
        '''                              added new optional parameter pSampleType to allow inform the SampleType to filter Tests 
        '''                              not included in the Profile
        ''' </remarks>
        Public Function GetTests(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfileID As Integer, _
                                 ByVal pTestsInProfile As Boolean, Optional ByVal pSampleType As String = "") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the list of Tests included in the specified Test Profile


                        If (pTestsInProfile) Then
                            'Get all Tests included in the specified Profile
                            resultData = TestProfileTestsDelegate.ReadByTestProfileID(dbConnection, pTestProfileID)
                        Else
                            'Get all Tests NOT included in the specified Profile
                            resultData = TestProfileTestsDelegate.ReadTestsNotInProfile(dbConnection, pSampleType, pTestProfileID)
                        End If

                        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                            Dim testProfileTestsData As New TestProfileTestsDS
                            testProfileTestsData = DirectCast(resultData.SetDatos, TestProfileTestsDS)

                            'Get the Icon Path in Application Configuration file
                            Dim appSessionMng As New ApplicationSessionManager
                            Dim iconPath As String = appSessionMng.GetSessionInfo.ApplicationIconPath

                            If (iconPath.Trim <> "") Then
                                Dim iconPathSystemTests As String = ""
                                Dim iconPathUserTests As String = ""
                                Dim iconPathCalcTests As String = ""
                                Dim iconPathISETests As String = ""
                                Dim iconPathOFFSTests As String = ""

                                'Get the Icon Name stored in table of Preloaded Master Data for preloaded and user-defined Tests
                                Dim myPreloadedMDDelegate As New PreloadedMasterDataDelegate

                                '...Get Full Icon Path for Standard System Tests
                                resultData = myPreloadedMDDelegate.GetSubTableItem(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "TESTICON")
                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                    Dim myPreloadedMDDS As New PreloadedMasterDataDS
                                    myPreloadedMDDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMDDS.tfmwPreloadedMasterData.Rows.Count = 1) Then iconPathSystemTests = iconPath & myPreloadedMDDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                End If

                                '...Get Full Icon Path for Standard User Tests
                                resultData = myPreloadedMDDelegate.GetSubTableItem(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "USERTEST")
                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                    Dim myPreloadedMDDS As New PreloadedMasterDataDS
                                    myPreloadedMDDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMDDS.tfmwPreloadedMasterData.Rows.Count = 1) Then iconPathUserTests = iconPath & myPreloadedMDDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                End If

                                '...Get Full Icon Path for Calculated Tests
                                resultData = myPreloadedMDDelegate.GetSubTableItem(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "TCALC")
                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                    Dim myPreloadedMDDS As New PreloadedMasterDataDS
                                    myPreloadedMDDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMDDS.tfmwPreloadedMasterData.Rows.Count = 1) Then iconPathCalcTests = iconPath & myPreloadedMDDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                End If

                                '...Get Full Icon Path for ISE Tests
                                resultData = myPreloadedMDDelegate.GetSubTableItem(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "TISE_SYS")
                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                    Dim myPreloadedMDDS As New PreloadedMasterDataDS
                                    myPreloadedMDDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMDDS.tfmwPreloadedMasterData.Rows.Count = 1) Then iconPathISETests = iconPath & myPreloadedMDDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                End If

                                '...Get Full Icon Path for Off-System Tests
                                resultData = myPreloadedMDDelegate.GetSubTableItem(Nothing, GlobalEnumerates.PreloadedMasterDataEnum.ICON_PATHS, "TOFF_SYS")
                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                    Dim myPreloadedMDDS As New PreloadedMasterDataDS
                                    myPreloadedMDDS = DirectCast(resultData.SetDatos, PreloadedMasterDataDS)
                                    If (myPreloadedMDDS.tfmwPreloadedMasterData.Rows.Count = 1) Then iconPathOFFSTests = iconPath & myPreloadedMDDS.tfmwPreloadedMasterData(0).FixedItemDesc
                                End If

                                'Assign the proper Icon to each Test included in the Profile according its Test Type
                                For Each testProfileTestRow As TestProfileTestsDS.tparTestProfileTestsRow In testProfileTestsData.tparTestProfileTests.Rows
                                    testProfileTestRow.BeginEdit()
                                    If (testProfileTestRow.TestType = "STD") Then
                                        If (Convert.ToBoolean(testProfileTestRow.PreloadedTest)) Then
                                            testProfileTestRow.IconPath = iconPathSystemTests
                                        Else
                                            testProfileTestRow.IconPath = iconPathUserTests
                                        End If
                                    ElseIf (testProfileTestRow.TestType = "CALC") Then
                                        testProfileTestRow.IconPath = iconPathCalcTests
                                    ElseIf (testProfileTestRow.TestType = "ISE") Then
                                        testProfileTestRow.IconPath = iconPathISETests
                                    ElseIf (testProfileTestRow.TestType = "OFFS") Then
                                        testProfileTestRow.IconPath = iconPathOFFSTests
                                    End If
                                    testProfileTestRow.EndEdit()
                                Next

                                resultData.SetDatos = testProfileTestsData
                                resultData.HasError = False
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.GetTests", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Add a new Test Profile
        ''' </summary>
        ''' <param name="pTestProfile">Typed Dataset TestProfilesDS with data of the Test Profile to add</param>
        ''' <param name="pTestList">Typed Dataset TestProfileTestsDS with the list of Tests to include in the Profile</param>
        ''' <returns>GlobalDataTO containing the added Profile and/or error information</returns>
        ''' <remarks>
        ''' Creation ?
        ''' AG 01/09/2014 - BA-1869 when new profile is created the CustomPosition informed = MAX current value + 1
        ''' </remarks>
        Public Function Add(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfile As TestProfilesDS, _
                            ByVal pTestList As TestProfileTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify that there is not another Test Profile with the same Name
                        Dim testProfileToAdd As New TparTestProfilesDAO
                        resultData = testProfileToAdd.ExistsTestProfile(dbConnection, pTestProfile.tparTestProfiles(0).TestProfileName.ToString)
                        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                            Dim profileExists As Boolean = Convert.ToBoolean(resultData.SetDatos)
                            If (profileExists) Then
                                'The Name is already assigned to another Test Profile
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.DUPLICATED_TEST_PROFILE_NAME.ToString
                            Else
                                'Get the Position that corresponds to the new Profile  
                                resultData = testProfileToAdd.GetNextPosition(dbConnection)
                                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                                    pTestProfile.tparTestProfiles(0).BeginEdit()
                                    pTestProfile.tparTestProfiles(0).TestProfilePosition = Convert.ToInt32(resultData.SetDatos)

                                    'AG 01/09/2014 - BA-1869 new calc test customposition value = MAX current value + 1
                                    'pTestProfile.tparTestProfiles(0).EndEdit()
                                    resultData = testProfileToAdd.GetLastCustomPosition(dbConnection)
                                    If Not resultData.HasError Then
                                        If resultData.SetDatos Is Nothing OrElse resultData.SetDatos Is DBNull.Value Then
                                            pTestProfile.tparTestProfiles(0).CustomPosition = 1
                                        Else
                                            pTestProfile.tparTestProfiles(0).CustomPosition = DirectCast(resultData.SetDatos, Integer) + 1
                                        End If
                                        pTestProfile.tparTestProfiles(0).EndEdit()
                                        'AG 01/09/2014 - BA-1869

                                        'Se agrega el nuevo Test Profile
                                        resultData = testProfileToAdd.Create(dbConnection, pTestProfile)
                                        If (Not resultData.HasError) Then
                                            'Get the generated TestProfileID from the dataset returned and insert it in 
                                            'the dataset containing the Tests List
                                            For i As Integer = 0 To pTestList.tparTestProfileTests.Rows.Count - 1
                                                pTestList.tparTestProfileTests(i).TestProfileID = pTestProfile.tparTestProfiles(0).TestProfileID
                                            Next

                                            'Insert the list of Tests 
                                            Dim testListToAdd As New tparTestProfileTestsDAO
                                            resultData = testListToAdd.Create(dbConnection, pTestList)
                                        End If


                                    Else
                                        pTestProfile.tparTestProfiles(0).EndEdit()
                                    End If

                                End If
                            End If

                            If (Not resultData.HasError) Then
                                'When the Database Connection was opened locally, then the Commit is executed
                                If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                                resultData.SetDatos = pTestProfile
                            Else
                                'When the Database Connection was opened locally, then the Rollback is executed
                                If (pDBConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)
                            End If
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
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.Add", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Update data of an specific Test Profile, including the list of Tests included in it
        ''' </summary>
        ''' <param name="pTestProfile">Dataset with structure of table tparTestProfiles</param>
        ''' <param name="pTestList">Dataset with structure of table tparTestProfileTests</param>
        ''' <returns>GlobalDataTO containing the updated Profile and/or error information</returns>
        ''' <remarks>
        ''' Modified by: SA 28/06/2010 - Call Modify in TestProfileTestsDelegate instead Update in tparTestProfileTestsDAO
        ''' </remarks>
        Public Function Modify(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfile As TestProfilesDS, _
                               ByVal pTestList As TestProfileTestsDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Verify that there is not another Test Profile with the same Name
                        Dim testProfileToUpdate As New TparTestProfilesDAO
                        resultData = testProfileToUpdate.ExistsTestProfile(dbConnection, pTestProfile.tparTestProfiles(0).TestProfileName.ToString, _
                                                                           pTestProfile.tparTestProfiles(0).TestProfileID)
                        If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                            Dim profileExists As Boolean = Convert.ToBoolean(resultData.SetDatos)
                            If (profileExists) Then
                                'The Name is already assigned to another Test Profile
                                resultData.HasError = True
                                resultData.ErrorCode = GlobalEnumerates.Messages.DUPLICATED_TEST_PROFILE_NAME.ToString
                            Else
                                resultData = testProfileToUpdate.Update(dbConnection, pTestProfile)
                                If (Not resultData.HasError) Then

                                    resultData = TestProfileTestsDelegate.Modify(dbConnection, pTestList)
                                End If
                            End If
                        End If

                        If (Not resultData.HasError) Then
                            'When the Database Connection was opened locally, then the Commit is executed
                            If (pDBConnection Is Nothing) Then DAOBase.CommitTransaction(dbConnection)
                            resultData.SetDatos = pTestProfile
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
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.Modify", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete an specific Test Profile with all the Tests included in it
        ''' </summary>
        ''' <param name="pTestProfile">Typed DataSet containing the list of Test Profiles to delete</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Modified by: SA 27/06/2010 - Changed function to allow deletion of several Test Profiles
        ''' </remarks>
        Public Function Delete(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfile As TestProfilesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete the list of Tests included in each one of the specified Test Profile
                        For Each testProfile As TestProfilesDS.tparTestProfilesRow In pTestProfile.tparTestProfiles.Rows
                            Dim testListToDelete As New tparTestProfileTestsDAO
                            resultData = testListToDelete.Delete(dbConnection, testProfile.TestProfileID)
                            If (resultData.HasError) Then Exit For

                            Dim profileToDelete As New TparTestProfilesDAO
                            resultData = profileToDelete.Delete(dbConnection, testProfile.TestProfileID)
                            If (resultData.HasError) Then Exit For
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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.Delete", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Delete the specified STANDARD Test from all Test Profiles in which it is included. When a SampleType is informed, 
        ''' it means that the STANDARD Test have to be deleted only of all the Test Profiles defined for this SampleType.
        ''' Besides, delete all empty Test Profiles (all Test Profiles without Tests) 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <param name="pSampleType">Sample Type Code. Optional parameter</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks></remarks>
        Public Function DeleteByTestIDSampleType(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer, _
                                                 Optional ByVal pSampleType As String = "", Optional ByVal pTestType As String = "STD") As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Delete the Test from all Profiles in which it is included

                        resultData = TestProfileTestsDelegate.DeleteByTestIDSampleType(dbConnection, pTestID, pSampleType, pTestType)

                        If (Not resultData.HasError) Then
                            'Verify if there are empty Profiles to delete them
                            Dim myTestProfilesDAO As New TparTestProfilesDAO
                            resultData = myTestProfilesDAO.DeleteEmptyProfiles(dbConnection)
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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.DeleteByTestIDSampleType", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Set value of flag InUse for all Test Profiles added/removed from the Active WorkSession 
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pWorkSessionID">Work Session Identifier</param>
        ''' <param name="pAnalyzerID">Analyzer Identifier</param>
        ''' <param name="pFlag">Value of the InUse Flag to set</param>
        ''' <param name="pUpdateForExcluded">Optional parameter. When True, it means the InUse will be set to False
        '''                                  only for Test Profiles that have been excluded from the active WorkSession</param>
        ''' <returns>GlobalDataTO containing success/error information</returns>
        ''' <remarks>
        ''' Created by:  GDS 10/05/2010 
        ''' Modified by: SA  09/06/2010 - Added new optional parameter to reuse this method to set InUse=False for Test Profiles 
        '''                               that have been excluded from the active WorkSession  
        ''' </remarks>
        Public Function UpdateInUseFlag(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pWorkSessionID As String, _
                                        ByVal pAnalyzerID As String, ByVal pFlag As Boolean, _
                                        Optional ByVal pUpdateForExcluded As Boolean = False) As GlobalDataTO
            Dim myGlobalDataTO As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                myGlobalDataTO = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not myGlobalDataTO.HasError And Not myGlobalDataTO.SetDatos Is Nothing) Then
                    dbConnection = DirectCast(myGlobalDataTO.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myDAO As New TparTestProfilesDAO
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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                myGlobalDataTO.HasError = True
                myGlobalDataTO.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                myGlobalDataTO.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.UpdateInUseFlag", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return myGlobalDataTO
        End Function

        ''' <summary>
        ''' Read the TestProfiles in which the specified STANDARD Test is included
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestID">Test Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfileTestsDS with the list
        '''          of Test Profiles in which the Test is included</returns>
        ''' <remarks>
        ''' Created by:  TR 18/05/2010
        ''' </remarks>
        Public Function GetTestsByTestID(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        'Get the list of Tests included in the specified Test Profile
                        Dim TestProfileTestsDAO As New tparTestProfileTestsDAO
                        resultData = TestProfileTestsDAO.ReadByTestID(dbConnection, pTestID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.GetTestsByTestID", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Get data of the specified Test Profile
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileID">Test Profile Identifier</param>
        ''' <returns>GlobalDataTO containing a typed DataSet TestProfilesDS with the data of 
        '''          the specified Test Profile</returns>
        ''' <remarks>
        ''' Created by:  TR 18/05/2010
        ''' </remarks>
        Public Function Read(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfileID As Integer) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection

            Try
                resultData = DAOBase.GetOpenDBConnection(pDBConnection)
                If (Not resultData.HasError And Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then
                        Dim myTestProfileDAO As New TparTestProfilesDAO
                        resultData = myTestProfileDAO.Read(dbConnection, pTestProfileID)
                    End If
                End If
            Catch ex As Exception
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.Read", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Sort the list of existing Test Profiles by updating field TestProfilePosition
        ''' </summary>
        ''' <param name="pDBConnection">Open DB Connection</param>
        ''' <param name="pTestProfileDS">Typed DataSet TestProfilesDS containing the list of Test Profiles in the 
        '''                              order in which the Position field should be updated</param>
        ''' <returns>GlobalDataTO containing sucess/error information</returns>
        ''' <remarks>
        ''' Created by:  SG 08/07/2010
        ''' Modified by: SA 08/07/2010 - Changed the code: it is not possible execute the Commit after the Rollback
        ''' </remarks>
        Public Function UpdateTestProfilePosition(ByVal pDBConnection As SqlClient.SqlConnection, ByVal pTestProfileDS As TestProfilesDS) As GlobalDataTO
            Dim resultData As New GlobalDataTO
            Dim dbConnection As New SqlClient.SqlConnection
            Try
                resultData = DAOBase.GetOpenDBTransaction(pDBConnection)
                If (Not resultData.HasError) And (Not resultData.SetDatos Is Nothing) Then
                    dbConnection = CType(resultData.SetDatos, SqlClient.SqlConnection)
                    If (Not dbConnection Is Nothing) Then

                        For Each testProfileRow As TestProfilesDS.tparTestProfilesRow In pTestProfileDS.tparTestProfiles.Rows
                            Dim myTestProfilesDAO As New TparTestProfilesDAO
                            resultData = myTestProfilesDAO.UpdateTestProfilePosition(dbConnection, testProfileRow.TestProfileID, testProfileRow.TestProfilePosition)
                            If resultData.HasError Then Exit For
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
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then DAOBase.RollbackTransaction(dbConnection)

                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.UpdateTestProfilePosition", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return resultData
        End Function

        ''' <summary>
        ''' Gets all profiles tests order by CustomPosition (return columns: TestType, TestID, CustomPosition As TestPosition, PreloadedTest, Available)
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
                        Dim myDAO As New TparTestProfilesDAO
                        resultData = myDAO.GetCustomizedSortedTestSelectionList(dbConnection)
                    End If
                End If

            Catch ex As Exception
                resultData = New GlobalDataTO()
                resultData.HasError = True
                resultData.ErrorCode = GlobalEnumerates.Messages.SYSTEM_ERROR.ToString()
                resultData.ErrorMessage = ex.Message

                Dim myLogAcciones As New ApplicationLogManager()
                myLogAcciones.CreateLogActivity(ex.Message + " ((" + ex.HResult.ToString + "))", "TestProfilesDelegate.GetCustomizedSortedTestSelectionList", EventLogEntryType.Error, False)

            Finally
                If (pDBConnection Is Nothing) AndAlso (Not dbConnection Is Nothing) Then dbConnection.Close()

            End Try

            Return resultData
        End Function


        ''' <summary>
        ''' Update (only when informed) columns CustomPosition and Available for test profiles
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
                        Dim myDAO As New TparTestProfilesDAO
                        myGlobalDataTO = myDAO.UpdateCustomPositionAndAvailable(dbConnection, pTestsSortingDS)
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
                myLogAcciones.CreateLogActivity(ex.Message, "TestProfilesDelegate.UpdateCustomPositionAndAvailable", EventLogEntryType.Error, False)
            Finally
                If (pDBConnection Is Nothing) And (Not dbConnection Is Nothing) Then dbConnection.Close()
            End Try
            Return (myGlobalDataTO)
        End Function

#End Region

    End Class

End Namespace